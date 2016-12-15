using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System
{
    public partial class frmTS1_EditTPiCSCode : DevExpress.XtraEditors.XtraForm
    {
        private const short intFirstRow = 1; //First row in grid.
        private const short intTotalCol = 9; //Columns in grid.
        cDatabase db=new cDatabase(Module.TUW99);

        public frmTS1_EditTPiCSCode()
        {
            InitializeComponent();
        }
        public void ClearData()
        {
            txtTPiCSOrder.Text = "";
            txtTPiCSCode.Text = "";
            txtCode.Text = "";
            txtLot.Text = "";
            txtColor.Text = "";
            cboSysdelete.SelectedIndex = 1;
            Grid1.DataSource = null;
        }
        public void DataToGrid()
        {
            if (chkTPiCSOrder.Checked || chkTPiCSCode.Checked || chkCode.Checked || chkLot.Checked || chkColor.Checked)
            {
                if (chkCode.Checked && txtCode.Text.Trim().Length == 0)
                {
                    MessageBox.Show("กรุณาระบุ Fabric Code ในการค้นหา","Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                    return;
                }
                if (chkLot.Checked && txtLot.Text.Trim().Length == 0)
                {
                    MessageBox.Show("กรุณาระบุ Lot No ในการค้นหา","Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (chkColor.Checked && txtColor.Text.Trim().Length == 0)
                {
                    MessageBox.Show("กรุณาระบุ ColorNo ในการค้นหา", "Warning",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                this.Cursor = Cursors.WaitCursor;
                try
                {
                    string strSQL = "SELECT Top 10000 FabOrderNo,TPiCSCode,Code,LotNo,ColorNo,Serial,Division " +
                "FROM FinishFabricNC Left Join CustomerNew On FinishFabricNC.CustomerID=CustomerNew.ID " +
                "Where Sysdelete In";
                    switch (cboSysdelete.Text)
                    {
                        case "All":
                            strSQL = strSQL + "(0,1)";
                            break;
                        case "In Stock":
                            strSQL = strSQL + "(0)";
                            break;
                        case "Out Stock":
                            strSQL = strSQL + "(1)";
                            break;
                    }
                    if (chkCode.Checked) { strSQL = strSQL + " And Code Like '" + txtCode.Text.Trim() + "%'"; }
                    if (chkTPiCSOrder.Checked)
                    {
                        if (txtTPiCSOrder.Text == "")
                        {
                            strSQL = strSQL + " And FabOrderNo =N''";
                        }
                        else
                        {
                            strSQL = strSQL + " And FabOrderNo Like '" + txtTPiCSOrder.Text.Trim() + "%'";
                        }
                    }
                    if (chkTPiCSCode.Checked)
                    {
                        if (txtTPiCSCode.Text == "")
                        {
                            strSQL = strSQL + " And TPiCSCode =N''";
                        }
                        else
                        {
                            strSQL = strSQL + " And TPiCSCode Like '" + txtTPiCSCode.Text.Trim() + "%'";
                        }
                    }
                    if (chkLot.Checked) { strSQL = strSQL + " And Lotno Like '" + txtLot.Text.Trim() + "%'"; }
                    if (chkColor.Checked) { strSQL = strSQL + " And ColorNo Like '" + txtColor.Text.Trim() + "%'"; }

                    DataTable dt = db.GetDataTable(strSQL);
                    if (dt != null && dt.Rows.Count > 10000) { MessageBox.Show("Maximum rows to display is 10,000", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
                    dt.Columns.Add("Edit", typeof(bool));
                    Grid1.DataSource = dt;
                    gridView1.Columns["Serial"].Visible = false;
                    gridView1.Columns["FabOrderNo"].OptionsColumn.AllowEdit = false;
                    gridView1.Columns["Code"].OptionsColumn.AllowEdit = false;
                    gridView1.Columns["LotNo"].OptionsColumn.AllowEdit = false;
                    gridView1.Columns["ColorNo"].OptionsColumn.AllowEdit = false;
                    gridView1.Columns["Division"].OptionsColumn.AllowEdit = false;
                    gridView1.OptionsView.EnableAppearanceEvenRow = true;
                    gridView1.OptionsView.EnableAppearanceOddRow = true;
                    gridView1.OptionsView.ColumnAutoWidth = false;
                    gridView1.BestFitColumns();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
                this.Cursor = Cursors.Default;
            }
            else
            {
                MessageBox.Show("กรุณาระบุเงื่อนไขในการค้นหา", "Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
        }
        public void SaveData()
        {
            this.Cursor = Cursors.WaitCursor;
            db = new cDatabase(Module.TUW99);
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                string strSQL;
                for (int i = 0; i <= gridView1.RowCount - 1; i++)
                {
                    if (gridView1.GetRowCellValue(i, "Edit") != null)
                    {
                        strSQL = "Update FinishFabricNC Set TPiCSCode='" + gridView1.GetRowCellDisplayText(i, "TPiCSCode") + "' Where Code='" + gridView1.GetRowCellDisplayText(i, "Code") + "' And Serial='" + gridView1.GetRowCellDisplayText(i, "Serial") + "'";
                        db.Execute(strSQL);
                    }
                }
                db.CommitTrans();
                this.Cursor = Cursors.Default;
                MessageBox.Show("Save Complete", "Save data", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.ConnectionClose();
                this.Cursor = Cursors.Default;
            }
        }
        public void ExportExcel()
        {
            SaveFileDialog theOpenFile = new SaveFileDialog();
            string strTemp;
            theOpenFile.Filter = "Microsoft Excel Document|*.xls";
            if (theOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                strTemp = theOpenFile.FileName;
                gridView1.ExportToXls(strTemp);
            }
        }

        private void frmMain_Load(System.Object sender, System.EventArgs e)
        {
            ClearData();
        }
        private void chkTPiCSOrder_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (chkTPiCSOrder.Checked)
            {
                txtTPiCSOrder.Enabled = true;
            }
            else
            {
                txtTPiCSOrder.Enabled = false;
            }
        }
        private void chkTPiCSCode_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (chkTPiCSCode.Checked)
            {
                txtTPiCSCode.Enabled = true;
            }
            else
            {
                txtTPiCSCode.Enabled = false;
            }
        }
        private void chkCode_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (chkCode.Checked)
            {
                txtCode.Enabled = true;
            }
            else
            {
                txtCode.Enabled = false;
            }
        }
        private void chkLot_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (chkLot.Checked)
            {
                txtLot.Enabled = true;
            }
            else
            {
                txtLot.Enabled = false;
            }
        }
        private void chkColor_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (chkColor.Checked)
            {
                txtColor.Enabled = true;
            }
            else
            {
                txtColor.Enabled = false;
            }
        }
        private void cmdSearch_Click(System.Object sender, System.EventArgs e)
        {
            DataToGrid();
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = System.Convert.ToString(e.RowHandle + 1);
            }
            gridView1.IndicatorWidth = 45;
        }
        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gridView1.FocusedColumn.FieldName == "TPiCSCode")
            {
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "Edit", true);
            }
        }






    }
}