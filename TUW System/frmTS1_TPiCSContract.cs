using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Globalization;
using Microsoft.Office.Interop.Excel;
using myClass;

namespace TUW_System
{
    public partial class frmTS1_TPiCSContract : DevExpress.XtraEditors.XtraForm
    {
        CultureInfo clinfo = new CultureInfo("en-US");
        cDatabase db;

        public frmTS1_TPiCSContract()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            if (xtraTabControl1.SelectedTabPageIndex == 0)
            {
                gridFabricOrder.DataSource = null;
                gridFabric.DataSource = null;
                foreach (Control ctrl in LayoutControl1.Controls)
                {
                    switch (ctrl.GetType().ToString())
                    { 
                        case "DevExpress.XtraEditors.TextEdit":
                            ((DevExpress.XtraEditors.TextEdit)ctrl).Text = "";
                            break;
                        case "DevExpress.XtraEditors.DateEdit":
                            ((DevExpress.XtraEditors.DateEdit)ctrl).EditValue = null;
                            break;
                    }
                }
            }
            else
            {
                gridAccOrder.DataSource = null;
                gridAcc.DataSource = null;
                foreach (Control ctrl in LayoutControl2.Controls)
                {
                    switch (ctrl.GetType().ToString())
                    { 
                        case "DevExpress.XtraEditors.TextEdit":
                            ((DevExpress.XtraEditors.TextEdit)ctrl).Text = "";
                            break;
                        case "DevExpress.XtraEditors.DateEdit":
                            ((DevExpress.XtraEditors.DateEdit)ctrl).EditValue = null;
                            break;
                    }
                }
            }
        }
        public void ExportExcel()
        {
            if (xtraTabControl1.SelectedTabPageIndex == 0)
            {
                Fabric_Excel();
            }
            else
            {
                Accessory_Excel();
            }
        }
        public void DisplayData()
        {
            if(xtraTabControl1.SelectedTabPageIndex==0)
            {
                Fabric_Search();
            }
            else
            {
                Accessory_Search();
            }
        }

        #region "Fabric"

        private string SearchInv(string Code1)
        {
            string strSQL = "Select Sum(zaik) as xx From XZAIK Where Code='" + Code1 + "'";
            return db.ExecuteFirstValue(strSQL);
        }
        private void SearchFabric(string Search1, string CodeOrContract)
        {
            string strSQL = "";
            if (CodeOrContract == "Style")
            {
                if (chkCut.Checked == false)
                {
                    //strSQL = "SELECT * FROM T_Model2Fabric WHERE (code = \'" + Search1.Trim() + " \') ORDER BY CDATE,CONTRACT";
                    strSQL = "spTPiCSFDI_TPiCSContract_SearchFabric 'Style','" + Search1.Trim() + "'";
                }
                else
                {
                    strSQL = "select * from T_Contract2Fabricwithoutcut WHERE (code = \'" + Search1.Trim() + " \') ORDER BY CDATE,CONTRACT";
                }
            }
            else if (CodeOrContract == "Contract")
            {
                if (chkCut.Checked == false)
                {
                    //strSQL = "SELECT * FROM T_Model2Fabric WHERE (CONTRACT LIKE \'" + Search1.Trim() + "%\') ORDER BY CDATE,CONTRACT";
                    strSQL = "spTPiCSFDI_TPiCSContract_SearchFabric 'Contract','" + Search1.Trim() + "'";
                }
                else
                {
                    strSQL = "select * from T_Contract2Fabricwithoutcut WHERE (CONTRACT LIKE \'" + Search1.Trim() + "%\') ORDER BY CDATE,CONTRACT";
                }
            }
            else if (CodeOrContract == "Code")
            {
                if (chkCut.Checked == false)
                {
                    //strSQL = "SELECT * FROM T_Model2Fabric WHERE (Fabriccode = \'" + Search1.Trim() + " \') ORDER BY CDATE,CONTRACT";
                    strSQL = "spTPiCSFDI_TPiCSContract_SearchFabric 'Code','" + Search1.Trim() + "'";
                }
                else
                {
                    //In case there is no cutting process
                    strSQL = "select * from T_Contract2Fabricwithoutcut WHERE (kcode = \'" + Search1.Trim() + " \') ORDER BY CDATE,CONTRACT";
                }
            }
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            gridFabric.DataSource = dt;

            gridViewFabric.OptionsView.ShowFooter = true;
            gridViewFabric.Columns["Style"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom, "Total PCS");
            gridViewFabric.Columns["Sched(PCS)"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0}");
            gridViewFabric.Columns["Actual(PCS)"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0}");
            gridViewFabric.Columns["Due Date"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom, "Total KGS");
            gridViewFabric.Columns["Fabric(kgs.)"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,##0.00}");

            gridViewFabric.OptionsView.EnableAppearanceEvenRow = true;
            gridViewFabric.OptionsView.EnableAppearanceOddRow = true;
            gridViewFabric.OptionsView.ColumnAutoWidth = false;
            gridViewFabric.BestFitColumns();
        }
        private void SearchFabricOrder(string Search1, string CodeOrOrder)
        {
            string strSQL = "";
            if (CodeOrOrder == "Code")
            {
                strSQL = "EXEC spTPiCSFDI_TPiCSContract_SearchFabricOrder 'Code','" + Search1 + "'";
            }
            else if (CodeOrOrder == "Order")
            {
                strSQL = "EXEC spTPiCSFDI_TPiCSContract_SearchFabricOrder 'Order','" + Search1 + "'";
            }
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            gridFabricOrder.DataSource = dt;

            gridViewFabricOrder.OptionsView.ShowFooter = true;
            gridViewFabricOrder.Columns["Finish date"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom, "Total");
            gridViewFabricOrder.Columns["SchedQty"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridViewFabricOrder.Columns["ActualQty"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridViewFabricOrder.Columns["CODE"].Visible = false;
            gridViewFabricOrder.Columns["NAME"].Visible = false;
            gridViewFabricOrder.OptionsView.EnableAppearanceEvenRow = true;
            gridViewFabricOrder.OptionsView.EnableAppearanceOddRow = true;
            gridViewFabricOrder.OptionsView.ColumnAutoWidth = false;
            gridViewFabricOrder.BestFitColumns();

            txtCode.Text = gridViewFabricOrder.GetRowCellDisplayText(gridViewFabricOrder.RowCount - 1, "CODE");
            txtName.Text = gridViewFabricOrder.GetRowCellDisplayText(gridViewFabricOrder.RowCount - 1, "NAME");
            dtpPDate.EditValue = DateTime.ParseExact(gridViewFabricOrder.GetRowCellDisplayText(gridViewFabricOrder.RowCount - 1, "Start date"), "yyyyMMdd", clinfo);
            dtpNDate.EditValue = DateTime.ParseExact(gridViewFabricOrder.GetRowCellDisplayText(gridViewFabricOrder.RowCount - 1, "Finish date"), "yyyyMMdd", clinfo);
            txtSchedQty.Text = gridViewFabricOrder.GetRowCellDisplayText(gridViewFabricOrder.RowCount - 1, "SchedQty");
            txtSpecQty.Text = gridViewFabricOrder.GetRowCellDisplayText(gridViewFabricOrder.RowCount - 1, "ActualQty");
        }
        private void Fabric_Search()//Fabric
        {
            db.ConnectionOpen();
            try
            {
                switch (optFabric.SelectedIndex)
                {
                    case 0: //Contract
                        if (txtContract.Text.Trim().Length == 0) { return; }
                        gridFabricOrder.DataSource = null;
                        SearchFabric(txtContract.Text.Trim(), "Contract");
                        break;
                    case 1: //Order
                        if (txtOrder.Text.Trim().Length == 0) { return; }
                        SearchFabricOrder(txtOrder.Text, "Order");
                        SearchFabric(txtCode.Text.Trim(), "Code");
                        GroupControl3.Text = "Contract No. according to " + txtName.Text + " : Current Inventory = " + SearchInv(txtCode.Text) + " Kg.";
                        break;
                    case 2: //Style
                        if (txtStyle.Text.Trim().Length == 0) { return; }
                        gridFabricOrder.DataSource = null;
                        SearchFabric(txtStyle.Text.Trim(), "Style");
                        break;
                    case 3: //Code
                        if (txtCode.Text.Trim().Length == 0) { return; }
                        SearchFabricOrder(txtCode.Text.Trim(), "Code");
                        SearchFabric(txtCode.Text.Trim(), "Code");
                        GroupControl3.Text = "Contract No. according to " + txtName.Text + " : Current Inventory = " + SearchInv(txtCode.Text) + " Kg.";
                        break;
                    case 4: //Name
                        if (txtName.Text.Trim().Length == 0) { return; }
                        SearchFabricOrder(txtName.Text.Trim(), "Name");
                        SearchFabric(txtName.Text.Trim(), "Name");
                        GroupControl3.Text = "Contract No. according to " + txtName.Text + " : Current Inventory = " + SearchInv(txtCode.Text) + " Kg.";
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();

        }
        private void txtContract_Enter(object sender, EventArgs e)
        {
            optFabric.SelectedIndex = 0;
            GroupControl3.Text = "Contract";
        }
        private void txtContract_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Fabric_Search();
            }
        }
        private void txtOrder_Enter(object sender, EventArgs e)
        {
            optFabric.SelectedIndex = 1;
        }
        private void txtOrder_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Fabric_Search();
            }
        }
        private void txtStyle_Enter(object sender, EventArgs e)
        {
            optFabric.SelectedIndex = 2;
        }
        private void txtStyle_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Fabric_Search();
            }
        }
        private void txtCode_Enter(object sender, EventArgs e)
        {
            optFabric.SelectedIndex = 3;
        }
        private void txtCode_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Fabric_Search();
            }
        }
        private void txtName_Enter(object sender, EventArgs e)
        {
            optFabric.SelectedIndex = 4;
        }
        private void txtName_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Fabric_Search();
            }
        }
        private void Fabric_Excel()
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook xlBook = xlApp.Workbooks.Add();
            Microsoft.Office.Interop.Excel.Sheets xlSheets = xlBook.Worksheets;
            Microsoft.Office.Interop.Excel._Worksheet xlSheet = (_Worksheet)xlSheets.get_Item(1);
            Microsoft.Office.Interop.Excel.Range r1;
            Microsoft.Office.Interop.Excel.Range r2;

            xlApp.Visible = true;
            int currentRow = 1;
            xlSheet.Cells[currentRow, 1] = "Order No.";
            xlSheet.Cells[currentRow, 2] = "Order Issue";
            xlSheet.Cells[currentRow, 3] = "Start date";
            xlSheet.Cells[currentRow, 4] = "Finish date";
            xlSheet.Cells[currentRow, 5] = "SchedQty";
            xlSheet.Cells[currentRow, 6] = "ActualQty";
            xlSheet.Cells[currentRow, 7] = "Code";
            xlSheet.Cells[currentRow, 8] = "Name";
            r1 = xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[1, 1], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[1, 1]);
            r2 = xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[1, 8], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[1, 8]);
            xlSheet.get_Range(r1, r2).Interior.ColorIndex = 39;
            xlSheet.get_Range(r1, r2).BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Type.Missing);

            for (int i = 0; i < gridViewFabricOrder.RowCount; i++)
            {
                currentRow++;
                for (int j = 0; j < gridViewFabricOrder.Columns.Count; j++)
                {
                    xlSheet.Cells[currentRow, j + 1] = gridViewFabricOrder.GetRowCellDisplayText(i, gridViewFabricOrder.Columns[j]);
                }
            }
            currentRow++;
            //xlSheet.get_Range(r1,r2).EntireColumn.AutoFit();
            //
            currentRow++;
            xlSheet.Cells[currentRow, 1] = "Contract No.";
            xlSheet.Cells[currentRow, 2] = "Style";
            xlSheet.Cells[currentRow, 3] = "Sched(PCS)";
            xlSheet.Cells[currentRow, 4] = "Actual(PCS)";
            xlSheet.Cells[currentRow, 5] = "Fabric Code";
            xlSheet.Cells[currentRow, 6] = "Fabric Name";
            xlSheet.Cells[currentRow, 7] = "Usage/PCS";
            xlSheet.Cells[currentRow, 8] = "Prod.Date";
            xlSheet.Cells[currentRow, 9] = "Due Date";
            xlSheet.Cells[currentRow, 10] = "Fabric(KGS)";
            r1 = xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[currentRow, 1], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[currentRow, 1]);
            r2 = xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[currentRow, 10], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[currentRow, 10]);
            xlSheet.get_Range(r1, r2).Interior.ColorIndex = 39;
            xlSheet.get_Range(r1, r2).BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Type.Missing);
            for (int i = 0; i < gridViewFabric.RowCount; i++)
            {
                currentRow++;
                for (int j = 0; j < gridViewFabric.Columns.Count; j++)
                {
                    xlSheet.Cells[currentRow, j + 1] = gridViewFabric.GetRowCellDisplayText(i, gridViewFabric.Columns[j]);
                }
            }
            currentRow++;
            //xlSheet.get_Range(r1, r2).EntireColumn.AutoFit();
        }
        private void gridViewFabric_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                txtContract.Text = gridViewFabric.GetDataRow(e.RowHandle)["Contract No."].ToString();
                txtStyle.Text = gridViewFabric.GetDataRow(e.RowHandle)["Style"].ToString();
                txtCode.Text = gridViewFabric.GetDataRow(e.RowHandle)["Fabric Code"].ToString();
                txtName.Text = gridViewFabric.GetDataRow(e.RowHandle)["Fabric Name"].ToString();
                SearchFabricOrder(gridViewFabric.GetDataRow(e.RowHandle)["Fabric Code"].ToString(), "Code");
                db.ConnectionOpen();
                GroupControl3.Text = "Contract No. according to " + txtName.Text + " : Current Inventory = " + SearchInv(txtCode.Text) + " Kg.";
                db.ConnectionClose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void gridViewFabricOrder_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (Equals(gridViewFabricOrder.GetRowCellValue(e.RowHandle, "Order Issue"), gridViewFabricOrder.GetRowCellValue(e.RowHandle, "Start date")))
            {
                if (e.Column.FieldName == "Order Issue" || e.Column.FieldName == "Start date")
                {
                    e.Appearance.BackColor = Color.Green;
                    e.Appearance.BackColor2 = Color.LightGreen;
                }

            }

        }

        #endregion

        #region "Accessory"

        private void SearchAcc(string Search1, string CodeOrContract)
        {
            string strSQL = "";
            if (CodeOrContract == "Style")
            {
                strSQL = String.Format("spTPiCSFDI_TPiCSContract_SearchAcc 'Style','{0}'", Search1.Trim());
            }
            else if (CodeOrContract == "Contract")
            {
                strSQL = String.Format("spTPiCSFDI_TPiCSContract_SearchAcc 'Contract','{0}'", Search1.Trim());
            }
            else if (CodeOrContract == "Code")
            {
                strSQL = String.Format("spTPiCSFDI_TPiCSContract_SearchAcc 'Code','{0}'", Search1.Trim());
            }
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            gridAcc.DataSource = dt;

            gridViewAcc.OptionsView.ShowFooter = true;
            gridViewAcc.Columns["Amount Acc"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,##0.00}");

            gridViewAcc.OptionsView.EnableAppearanceEvenRow = true;
            gridViewAcc.OptionsView.EnableAppearanceOddRow = true;
            gridViewAcc.OptionsView.ColumnAutoWidth = false;
            gridViewAcc.BestFitColumns();
        }
        private void SearchAccOrder(string Search1, string CodeOrOrder)
        {
            string strSQL = "";
            if (CodeOrOrder == "Code")
            {
                strSQL = String.Format("EXEC spTPiCSFDI_TPiCSContract_SearchAccOrder 'Code','{0}'", Search1);
            }
            else if (CodeOrOrder == "Order")
            {
                strSQL = String.Format("EXEC spTPiCSFDI_TPiCSContract_SearchAccOrder 'Order','{0}'", Search1);
            }
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            if (dt != null && dt.Rows.Count > 0)
            {
                gridAccOrder.DataSource = dt;
                gridViewAccOrder.OptionsView.ShowFooter = true;
                gridViewAccOrder.Columns["Finish date"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom, "Total");
                gridViewAccOrder.Columns["SchedQty"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
                gridViewAccOrder.Columns["ActualQty"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
                gridViewAccOrder.Columns["ActualInv"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
                gridViewAccOrder.Columns["CODE"].Visible = false;
                gridViewAccOrder.Columns["NAME"].Visible = false;
                gridViewAccOrder.OptionsView.EnableAppearanceEvenRow = true;
                gridViewAccOrder.OptionsView.EnableAppearanceOddRow = true;
                gridViewAccOrder.OptionsView.ColumnAutoWidth = false;
                gridViewAccOrder.BestFitColumns();

                txtCodeAcc.Text = gridViewAccOrder.GetRowCellDisplayText(gridViewAccOrder.RowCount - 1, "CODE");
                txtNameAcc.Text = gridViewAccOrder.GetRowCellDisplayText(gridViewAccOrder.RowCount - 1, "NAME");
                dtpPDateAcc.EditValue = DateTime.ParseExact(gridViewAccOrder.GetRowCellDisplayText(gridViewAccOrder.RowCount - 1, "Start date"), "yyyyMMdd", clinfo);
                dtpNDateAcc.EditValue = DateTime.ParseExact(gridViewAccOrder.GetRowCellDisplayText(gridViewAccOrder.RowCount - 1, "Finish date"), "yyyyMMdd", clinfo);
                txtSchedQtyAcc.Text = gridViewAccOrder.GetRowCellDisplayText(gridViewAccOrder.RowCount - 1, "SchedQty");
                txtSpecQtyAcc.Text = gridViewAccOrder.GetRowCellDisplayText(gridViewAccOrder.RowCount - 1, "ActualQty");
            }
            
        }
        private void Accessory_Search()
        {
            db.ConnectionOpen();
            try
            {
                switch (optAcc.SelectedIndex)
                {
                    case 0: //Contract
                        if (txtContractAcc.Text.Trim().Length == 0) { return; }
                        gridAccOrder.DataSource = null;
                        SearchAcc(txtContractAcc.Text.Trim(), "Contract");
                        break;
                    case 1: //Order
                        if (txtOrderAcc.Text.Trim().Length == 0) { return; }
                        SearchAccOrder(txtOrderAcc.Text, "Order");
                        SearchAcc(txtCodeAcc.Text.Trim(), "Code");
                        GroupControl6.Text = "Contract No. according to " + txtNameAcc.Text + " : Current Inventory = " + SearchInv(txtCodeAcc.Text) + " Kg.";
                        break;
                    case 2: //Style
                        if (txtStyleAcc.Text.Trim().Length == 0) { return; }
                        gridAccOrder.DataSource = null;
                        SearchAcc(txtStyleAcc.Text.Trim(), "Style");
                        break;
                    case 3: //Code
                        if (txtCodeAcc.Text.Trim().Length == 0) { return; }
                        SearchAccOrder(txtCodeAcc.Text.Trim(), "Code");
                        SearchAcc(txtCodeAcc.Text.Trim(), "Code");
                        GroupControl6.Text = "Contract No. according to " + txtNameAcc.Text + " : Current Inventory = " + SearchInv(txtCodeAcc.Text) + " Kg.";
                        break;
                    case 4: //Name
                        if (txtNameAcc.Text.Trim().Length == 0) { return; }
                        SearchAccOrder(txtNameAcc.Text.Trim(), "Name");
                        SearchAcc(txtNameAcc.Text.Trim(), "Name");
                        GroupControl6.Text = "Contract No. according to " + txtNameAcc.Text + " : Current Inventory = " + SearchInv(txtCodeAcc.Text) + " Kg.";
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
        }
        private void txtContractAcc_Enter(object sender, EventArgs e)
        {
            optAcc.SelectedIndex = 0;
        }
        private void txtContractAcc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Accessory_Search();
            }
        }
        private void txtOrderAcc_Enter(object sender, EventArgs e)
        {
            optAcc.SelectedIndex = 1;
        }
        private void txtOrderAcc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Accessory_Search();
            }
        }
        private void txtStyleAcc_Enter(object sender, EventArgs e)
        {
            optAcc.SelectedIndex = 2;
        }
        private void txtStyleAcc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Accessory_Search();
            }
        }
        private void txtCodeAcc_Enter(object sender, EventArgs e)
        {
            optAcc.SelectedIndex = 3;
        }
        private void txtCodeAcc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Accessory_Search();
            }
        }
        private void txtNameAcc_Enter(object sender, EventArgs e)
        {
            optAcc.SelectedIndex = 4;
        }
        private void txtNameAcc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Accessory_Search();
            }
        }
        private void Accessory_Excel()
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook xlBook = xlApp.Workbooks.Add();
            Microsoft.Office.Interop.Excel.Sheets xlSheets = xlBook.Worksheets;
            Microsoft.Office.Interop.Excel._Worksheet xlSheet = (_Worksheet)xlSheets.get_Item(1);
            Microsoft.Office.Interop.Excel.Range r1;
            Microsoft.Office.Interop.Excel.Range r2;

            xlApp.Visible = true;
            int currentRow = 1;
            xlSheet.Cells[currentRow, 1] = "Order No.";
            xlSheet.Cells[currentRow, 2] = "Start date";
            xlSheet.Cells[currentRow, 3] = "Finish date";
            xlSheet.Cells[currentRow, 4] = "SchedQty";
            xlSheet.Cells[currentRow, 5] = "ActualQty";
            xlSheet.Cells[currentRow, 6] = "ActualInv";
            xlSheet.Cells[currentRow, 7] = "Code";
            xlSheet.Cells[currentRow, 8] = "Name";
            r1 = xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[1, 1], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[1, 1]);
            r2 = xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[1, 8], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[1, 8]);
            xlSheet.get_Range(r1, r2).Interior.ColorIndex = 39;
            xlSheet.get_Range(r1, r2).BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Type.Missing);

            for (int i = 0; i < gridViewAccOrder.RowCount; i++)
            {
                currentRow++;
                for (int j = 0; j < gridViewAccOrder.Columns.Count; j++)
                {
                    xlSheet.Cells[currentRow, j + 1] = gridViewAccOrder.GetRowCellDisplayText(i, gridViewAccOrder.Columns[j]);
                }
            }
            currentRow++;
            //xlSheet.get_Range(r1, r2).EntireColumn.AutoFit();
            //
            currentRow++;
            xlSheet.Cells[currentRow, 1] = "Contract No.";
            xlSheet.Cells[currentRow, 2] = "Model";
            xlSheet.Cells[currentRow, 3] = "PCS";
            xlSheet.Cells[currentRow, 4] = "Acc Code";
            xlSheet.Cells[currentRow, 5] = "Accessory";
            xlSheet.Cells[currentRow, 6] = "Prod.Date";
            xlSheet.Cells[currentRow, 7] = "Due Date";
            xlSheet.Cells[currentRow, 8] = "Amount Acc";
            r1 = xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[currentRow, 1], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[currentRow, 1]);
            r2 = xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[currentRow, 8], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[currentRow, 8]);
            xlSheet.get_Range(r1, r2).Interior.ColorIndex = 39;
            xlSheet.get_Range(r1, r2).BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Type.Missing);
            for (int i = 0; i < gridViewAcc.RowCount; i++)
            {
                currentRow++;
                for (int j = 0; j < gridViewAcc.Columns.Count; j++)
                {
                    xlSheet.Cells[currentRow, j + 1] = gridViewAcc.GetRowCellDisplayText(i, gridViewAcc.Columns[j]);
                }
            }
            currentRow++;
            //xlSheet.get_Range(r1, r2).EntireColumn.AutoFit();
        }
        private void gridViewAcc_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                txtContractAcc.Text = gridViewAcc.GetDataRow(e.RowHandle)["Contract No."].ToString();
                txtStyleAcc.Text = gridViewAcc.GetDataRow(e.RowHandle)["Model"].ToString();
                txtCodeAcc.Text = gridViewAcc.GetDataRow(e.RowHandle)["Acc Code"].ToString();
                txtNameAcc.Text = gridViewAcc.GetDataRow(e.RowHandle)["Accessory"].ToString();
                SearchAccOrder(gridViewAcc.GetDataRow(e.RowHandle)["Acc Code"].ToString(), "Code");
                db.ConnectionOpen();
                GroupControl6.Text = "Contract No. according to " + txtNameAcc.Text + " : Current Inventory = " + SearchInv(txtCodeAcc.Text) + " Kg.";
                db.ConnectionClose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        private void fgAccOrder_ClickEvent(object sender, System.EventArgs e)
        {
            //If fgAccOrder.Rows > 1 Then
            //    I2Acc.Rows = 1
            //    txtOrderAcc.Text = fgAccOrder.get_TextMatrix(fgAccOrder.Row, 0)
            //    Call ConnectTPICS()
            //    Call SearchAccOrder(txtOrderAcc.Text.Trim, "Order")
            //    Call SearchAcc(txtCodeAcc.Text.Trim, "Code")
            //    Call DisConnectTPICS()
            //End If
        }
        private void I2Acc_ClickEvent(object sender, System.EventArgs e)
        {
            //if (I2Acc.Rows > 1)
            //{
            //    txtContractAcc.Text = I2Acc.get_TextMatrix(I2Acc.Row, 0);
            //    txtStyleAcc.Text = I2Acc.get_TextMatrix(I2Acc.Row, 1);
            //    txtCodeAcc.Text = I2Acc.get_TextMatrix(I2Acc.Row, 3);
            //    txtNameAcc.Text = I2Acc.get_TextMatrix(I2Acc.Row, 4);
            //    Module.ConnectTPICS();
            //    SearchAccOrder(I2Acc.get_TextMatrix(I2Acc.Row, 3), "Code");
            //    Module.DisConnectTPICS();
            //    GroupControl6.Text = "Contact <<= according to " + I2Acc.get_TextMatrix(I2Acc.Row, 4) + "  : Current Inventory = " + Strings.Format(SearchInv(I2Acc.get_TextMatrix(I2Acc.Row, 3)), "#,##0.00") + " PCs. =>>";
            //}
        }
        private void XtraTabControl1_Click(object sender, System.EventArgs e)
        {
            if (xtraTabControl1.SelectedTabPageIndex == 0)
            {
                txtOrder.Text = txtOrderAcc.Text;
                txtContract.Text = txtContractAcc.Text;
                txtStyle.Text = txtStyleAcc.Text;
                optFabric.SelectedIndex = optAcc.SelectedIndex;
            }
            else if (xtraTabControl1.SelectedTabPageIndex == 1)
            {
                txtOrderAcc.Text = txtOrder.Text;
                txtContractAcc.Text = txtContract.Text;
                txtStyleAcc.Text = txtStyle.Text;
                optAcc.SelectedIndex = optFabric.SelectedIndex;
            }
        }
        private void gridAccOrder_CellClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            ////If fgAccOrder.Rows > 1 Then
            //I2Acc.Rows = 1;
            ////txtOrderAcc.Text = fgAccOrder.get_TextMatrix(fgAccOrder.Row, 0)
            //txtOrderAcc.Text = gridAccOrder.Rows[e.RowIndex].Cells[0].Value.ToString();
            //Module.ConnectTPICS();
            //SearchAccOrder(txtOrderAcc.Text.Trim(), "Order");
            //SearchAcc(txtCodeAcc.Text.Trim(), "Code");
            //Module.DisConnectTPICS();
            ////End If
        }
        private void frmTPiCSContract_Load(object sender, EventArgs e)
        {
            db = new cDatabase(Module.Sewing);
        }



























    }


}