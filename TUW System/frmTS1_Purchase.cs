using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Globalization;
using System.Linq;
using Microsoft.VisualBasic;
using myClass;

namespace TUW_System
{
    public partial class frmTS1_Purchase : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db = new cDatabase(Module.Sewing);
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        DataTable dtSupplier;//เก็บค่า supplier ในคอมโบบ็อกซ์
        DataTable dtOrder;//เก็บค่า order balance ในกริดวิวทั้งหมด
        int intCountSelect;//เก็บค่าการคลิกเลือก "Select" ในกริด
        decimal sumQTY, sumAmount;//เก็บค่าผลรวมเวลาคลิกเลือก "Select" ในกริด
        int iCan;//เก็บเลขที่การ Revise
        private enum enumOptStatus
        {
            REPRINT,
            REVISE 
        }
        private enum enumOptClassification
        { 
            NONE,
            ACCESSORY,
            FABRIC,
            FABRIC_OUTSIDE
        }

        public frmTS1_Purchase()
        {
            InitializeComponent();
        }
        public void NewData(bool clearHeader,bool clearDetail,bool clearComboboxPO)
        {
            if (clearHeader)
            {
                dtpStart.EditValue = DateTime.Today;
                dtpEnd.EditValue = DateTime.Today;
                optClassification.EditValue = false;
                cboSection.Text = "";
                optStatus.EditValue = false;
            }
            if (clearDetail)
            {
                cboSupCode.EditValue = "";
                cboSupCode.Properties.DataSource = null;
                dtpPO.EditValue = DateTime.Today;
                foreach (Control ctrl in layoutControl3.Controls)
                {
                    switch (ctrl.GetType().ToString())
                    { 
                        case "DevExpress.XtraEditors.TextEdit":
                            ctrl.Text = "";
                            break;
                    }
                }
                if (clearComboboxPO) {cboPO.EditValue = "";}
                cboItem.EditValue = "";
                cboItem.Properties.Items.Clear();
                gridControl1.DataSource = null;
                gridView1.PopulateColumns();
                lblCrtUser.Text = "";
                lblCrtDate.Text = "";
                lblUpdUser.Text = "";
                lblUpdDate.Text = "";
            }
        }
        public void EditData()
        {
            if (optClassification.SelectedIndex == -1)
            {
                MessageBox.Show("You must select classification(Mat. Type)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (cboSection.EditValue.ToString().Length == 0)
            {
                MessageBox.Show("You must select section", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (optStatus.SelectedIndex != (int)enumOptStatus.REVISE)
            {
                MessageBox.Show("You must select revise status", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return; 
            }
            if (cboSupCode.EditValue.ToString().Length == 0)
            {
                MessageBox.Show("You must select supplier code", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            this.Cursor = Cursors.WaitCursor;
            try
            {
                DisplayOrderBalance();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load order balance", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        public void SaveData()
        {
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            this.Cursor = Cursors.WaitCursor;
            string strPO = "";//เก็บค่าเลขที่ PO
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();               
                if (optStatus.SelectedIndex ==(int)enumOptStatus.REPRINT  ){throw new System.ArgumentException("Can not save in Re-Print mode");}
                if (cboSupCode.EditValue.ToString().Length == 0) { throw new System.ArgumentException("Please! Select Supplier Code."); }
                bool chCol=false;
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    if ((bool)gridView1.GetRowCellValue(i, "SELECT")) 
                    {
                        chCol = true;
                        break;
                    }
                }
                if (chCol == false) { throw new System.ArgumentException("Please Select Data For Save !!!"); }
                switch (optStatus.SelectedIndex)
                { 
                    case (int)enumOptStatus.REPRINT://Re-Print
                        if(cboPO.EditValue.ToString().Length==0)
                        {
                             if(cboSupCode.EditValue.ToString()=="FDEL")
                            {
                                strPO=GenIF("IF",false);
                            }
                            else
                            {
                                strPO=GenPONo("SX",false);
                            }                       
                        }
                        break;
                    case (int)enumOptStatus.REVISE://Revise
                        strPO=cboPO.EditValue.ToString().Trim();
                        break;
                    default://New
                        if(cboSupCode.EditValue.ToString()=="FDEL")
                        {
                            strPO=GenIF("IF",false);
                        }
                        else
                        {
                            strPO=GenPONo("SX",false);
                        }
                        break;
                }
                if (MessageBox.Show("Do you want to Save : " + strPO + " : (Y/N) ", "confirm data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    db.RollbackTrans();
                    db.ConnectionClose();
                    this.Cursor = Cursors.Default;
                    return;
                }
                cboPO.EditValue = strPO;
                string strSQL=" SELECT COUNT(PONO) FROM THPurchase WHERE PONO ='" + cboPO.EditValue +"'";
                int rowCount=int.Parse(db.ExecuteFirstValue(strSQL));
                if(rowCount>0 && optStatus.SelectedIndex!=(int)enumOptStatus.REVISE)
                {
                    throw new System.ArgumentException(" You Have Data In Database AlReady ");
                }
                SaveTHPurchase();
                switch(xtraTabControl1.SelectedTabPageIndex)
                {
                    case 0:
                        SaveTDPurchase();
                        break;
                    case 1:
                        SaveTDPurchase2();
                        break;
                }
                SaveXSLIPPONUM();
                db.CommitTrans();
                MessageBox.Show("Update Complete!","Save data",MessageBoxButtons.OK,MessageBoxIcon.Information);
                for (int i = gridView1.DataRowCount - 1; i > -1; i--)
                {
                    if ((bool)gridView1.GetRowCellValue(i, "SELECT"))
                    {
                        gridView1.DeleteRow(i);
                    }
                }
             }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
        }
        public void ExportExcel()
        {
            this.Cursor = Cursors.WaitCursor;
            this.Cursor = Cursors.Default;
        }
        public void PrintPreview()
        { 
            this.Cursor=Cursors.WaitCursor;
            try
            {
                switch (xtraTabControl1.SelectedTabPageIndex)
                {
                    case 0:
                        cCrystalReport ctr = new cCrystalReport(Application.StartupPath + "\\Report\\PURCHASE ORDER_N.rpt");
                        SaveXSLIPUpdFlgPrint(cboPO.EditValue.ToString());
                        int intCopy;
                        if (optClassification.SelectedIndex == (int)enumOptClassification.FABRIC_OUTSIDE)
                        {
                            intCopy = 2;//4;
                        }
                        else
                        {
                            intCopy = 2;
                        }
                        for (int i = 1; i <= intCopy; i++)
                        {
                            ctr.ReportTitle = "Print P/O";
                            ctr.ClearParameters();
                            ctr.SetParameter("PAGE", i.ToString());
                            if (optStatus.SelectedIndex == (int)enumOptStatus.REPRINT)
                            {
                                ctr.SetParameter("REPRINT", "1");
                            }
                            else
                            {
                                ctr.SetParameter("REPRINT", "0");
                            }
                            string fmlText = "{THPURCHASE.PONO}='" + cboPO.EditValue + "'";
                            //ctr.ReportFileName = Application.StartupPath + "\\Report\\PURCHASE ORDER_N.rpt";
                            ctr.PrintReport(fmlText, false);
                        }
                        break;
                    case 1:
                        throw new ApplicationException("NOT TO PRINT REPORT !!! YOU MUST CLICK SENT IF");
                        //break;
                }
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error Print P/O", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            this.Cursor=Cursors.Default;
        }
        public void Print()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                switch (xtraTabControl1.SelectedTabPageIndex)
                {
                    case 0:
                        cCrystalReport ctr = new cCrystalReport(Application.StartupPath + "\\Report\\PURCHASE ORDER_N.rpt");
                        if (!ctr.SetPrinter())
                        {
                            this.Cursor = Cursors.Default;
                            return;
                        }
                        SaveXSLIPUpdFlgPrint(cboPO.EditValue.ToString());
                        int intCopy;
                        if (optClassification.SelectedIndex == (int)enumOptClassification.FABRIC_OUTSIDE)
                        {
                            intCopy = 2;// 4;
                        }
                        else
                        {
                            intCopy = 2;
                        }
                        for (int i = 1; i <= intCopy; i++)
                        {
                            ctr.ReportTitle = "Print P/O";
                            ctr.ClearParameters();
                            ctr.SetParameter("PAGE", i.ToString());
                            if (optStatus.SelectedIndex == (int)enumOptStatus.REPRINT)
                            {
                                ctr.SetParameter("REPRINT", "1");
                            }
                            else
                            {
                                ctr.SetParameter("REPRINT", "0");
                            }
                            string fmlText = "{THPURCHASE.PONO}='" + cboPO.EditValue + "'";
                            //ctr.ReportFileName = Application.StartupPath + "\\Report\\PURCHASE ORDER_N.rpt";
                            ctr.PrintReport(fmlText,true);
                        }
                        break;
                    case 1:
                        throw new ApplicationException("NOT TO PRINT REPORT !!! YOU MUST CLICK SENT IF");
                        //break;
                }
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error Print P/O", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            this.Cursor = Cursors.Default;
        }

        private void LoadPO()
        {
            //PO No.
            string strSQL = "SELECT PONO FROM THPURCHASE WHERE PODATE LIKE '" + ((DateTime)dtpStart.EditValue).ToString("yyyy") + "%' AND CANORNO = '0' ORDER BY PONO";
            DataTable dt = db.GetDataTable(strSQL);
            cboPO.Properties.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                cboPO.Properties.Items.Add(dr["PONO"].ToString());
            }
        }
        private void DisplayPO()
        {
            string strSQL = "Exec spTPiCSSubsystem_PurchaseOrder_DisplayPO '" + cboPO.EditValue + "'";
            DataSet ds = db.GetDataSet(strSQL);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                optClassification.SelectedIndex =Convert.ToInt16(ds.Tables[0].Rows[0]["FLAGMAT"]);
                cboSection.EditValue = ds.Tables[0].Rows[0]["FSECTION"];

                DataTable dt = new DataTable();
                dt.Columns.Add("SupCode", typeof(System.String));
                DataRow dr = dt.NewRow();
                dr["SupCode"] = ds.Tables[0].Rows[0]["SUPCODE"];
                dt.Rows.Add(dr);
                cboSupCode.Properties.DataSource = dt;
                cboSupCode.Properties.DisplayMember = "SupCode";
                cboSupCode.Properties.ValueMember = "SupCode";
                cboSupCode.EditValue = ds.Tables[0].Rows[0]["SUPCODE"];

                txtAttn.Text = ds.Tables[0].Rows[0]["ATTN"].ToString();
                txtReqNo.Text = ds.Tables[0].Rows[0]["REQNO"].ToString();
                txtReqDate.Text = ds.Tables[0].Rows[0]["REQDATE"].ToString();
                txtRefNo.Text = ds.Tables[0].Rows[0]["REFNO"].ToString();
                txtRemark.Text = ds.Tables[0].Rows[0]["REMARK"].ToString();
                if(optStatus.SelectedIndex==(int)enumOptStatus.REVISE){
                    dtpPO.EditValue=DateTime.Today;
                }
                else{
                    dtpPO.EditValue = ds.Tables[0].Rows[0]["PODATE"];
                }
                txtPlant.Text = ds.Tables[0].Rows[0]["PLANT"].ToString();
                lblCrtUser.Text = ds.Tables[0].Rows[0]["CRTUSER"].ToString();
                lblCrtDate.Text = ds.Tables[0].Rows[0]["CRTDATE"].ToString();
                lblUpdUser.Text = ds.Tables[0].Rows[0]["UPDUSER"].ToString();
                lblUpdDate.Text = ds.Tables[0].Rows[0]["UPDDATE"].ToString();
            }
            if (ds != null && ds.Tables[1].Rows.Count > 0)
            {
                DataColumn dc = new DataColumn();

                dc.ColumnName = "SELECT";
                dc.DataType = typeof(bool);
                dc.DefaultValue = true;
                ds.Tables[1].Columns.Add(dc);
                ds.Tables[1].Columns["SELECT"].SetOrdinal(0);

                dc = new DataColumn();
                dc.ColumnName = "CANCEL";
                dc.DataType = typeof(bool);
                dc.DefaultValue = false;
                ds.Tables[1].Columns.Add(dc);
                ds.Tables[1].Columns["CANCEL"].SetOrdinal(1);

                ds.Tables[1].Columns["SELECT"].Caption = "Select";
                ds.Tables[1].Columns["CANCEL"].Caption = "Cancel";
                ds.Tables[1].Columns["BUMO"].Caption = "Supplier";
                ds.Tables[1].Columns["PORDER"].Caption = "Order No.";
                ds.Tables[1].Columns["SEIBAN"].Caption = "Seiban No.";
                ds.Tables[1].Columns["CODE"].Caption = "Part No.";
                ds.Tables[1].Columns["NAME"].Caption = "Part Description";
                ds.Tables[1].Columns["UNIT"].Caption = "Unit";
                ds.Tables[1].Columns["QTY"].Caption = "Quantity";
                ds.Tables[1].Columns["PRICE"].Caption = "Unit Price";
                ds.Tables[1].Columns["AMOUNT"].Caption = "Amount";
                gridControl1.DataSource = null;
                gridView1.PopulateColumns();
                gridControl1.DataSource = ds.Tables[1];
                gridView1.OptionsView.EnableAppearanceEvenRow = true;
                gridView1.OptionsView.EnableAppearanceOddRow = true;
                gridView1.OptionsView.ColumnAutoWidth = false;
                for (int i = 0; i < gridView1.Columns.Count; i++)
                {
                    if (gridView1.Columns[i].FieldName != "SEIBAN")
                    {
                        gridView1.Columns[i].BestFit();
                    }
                }
                gridView1.Columns["EDA"].Visible = false;
            }
//            string strSQL=" SELECT      dbo.TDPurchase.PONo, dbo.XITEM.VENDOR, dbo.TDPurchase.TPICSNO, dbo.TDPurchase.Contract , dbo.TDPurchase.SBNo, dbo.TDPurchase.PartNo,dbo.TDPurchase.PartDel,  dbo.XHEAD.NAME, " & _
//                          "  dbo.TDPurchase.Unit, dbo.TDPurchase.Qty, dbo.TDPurchase.UPrc, dbo.TDPurchase.Amt, dbo.TDPurchase.EDA ,dbo.TDPurchase.Same3,dbo.TDPurchase.Same4 ,dbo.TDPurchase.SBNO1 ,dbo.TDPurchase.SBNO2 " & _
//                " FROM         dbo.TDPurchase INNER JOIN dbo.XITEM ON dbo.TDPurchase.PartNo = dbo.XITEM.CODE INNER JOIN dbo.XHEAD ON dbo.XITEM.CODE = dbo.XHEAD.CODE " & _
//                " WHERE     dbo.TDPurchase.PONo = '" & Trim(cboNo.Text) & "' AND dbo.XITEM.BUMO = '" & cboSupCode.Text & "' " & _
//                " ORDER BY  dbo.XITEM.VENDOR, dbo.TDPurchase.PartNo, dbo.TDPurchase.TPICSNO"
//'        Debug.Print s_SQL
//        ConnectX (s_SQL)
//        If Rs.RecordCount <> 0 Then
//            Rs.MoveFirst
//            r = 0
//            While Not Rs.EOF
//                r = r + 1
//                With fg
//                    If .Rows <= r Then .Rows = .Rows + 1
//                    .TextMatrix(r, 1) = "-1"
//                    .TextMatrix(r, 2) = "0"
//                    .TextMatrix(r, 3) = IIf(IsNull(Rs!Vendor) = True, "", Rs!Vendor)
//                    .TextMatrix(r, 4) = IIf(IsNull(Rs!TPICSNO) = True, "", Rs!TPICSNO)
//                    .TextMatrix(r, 5) = IIf(IsNull(Rs!SBNO) = True, "", Rs!SBNO & Rs!SBNO1 & Rs!SBNO2)
//                    .TextMatrix(r, 6) = IIf(IsNull(Rs!PartNo) = True, "", Rs!PartNo)
//                    .TextMatrix(r, 7) = IIf(IsNull(Rs!PartDel) = True, "", Rs!PartDel)
//                    .TextMatrix(r, 8) = IIf(IsNull(Rs!Unit) = True, "", Rs!Unit)
//                    .TextMatrix(r, 9) = IIf(IsNull(Rs!Qty) = True, 0, Format(Rs!Qty, "#,##0"))
//                    .TextMatrix(r, 10) = IIf(IsNull(Rs!UPrc) = True, 0, Format(Rs!UPrc, "#,##0.0000"))
//                    .TextMatrix(r, 11) = IIf(IsNull(Rs!Amt) = True, 0, Format(Rs!Amt, "#,##0.00"))
//                    .TextMatrix(r, 12) = IIf(IsNull(Rs!EDA) = True, 0, Rs!EDA)
//                    Rs.MoveNext
                    
//                End With
//            Wend
//    End If
        //Rs.Close
        }

        private void LoadSupplier()
        {
            string strSQL = "EXEC spTPiCSSubsystem_PurchaseOrder_Load '" + ((DateTime)dtpStart.EditValue).ToString("yyyyMMdd", dtfinfo) + "'" +
                    ",'" + ((DateTime)dtpEnd.EditValue).ToString("yyyyMMdd", dtfinfo) + "'" +
                    ",'" + optClassification.SelectedIndex.ToString() + "','" + cboSection.Text + "'" +
                    ",'" + optStatus.SelectedIndex.ToString() + "',''";
            DataSet ds = db.GetDataSet(strSQL);
            dtSupplier = ds.Tables[0];
            dtOrder = ds.Tables[1];
            //Datatable[0]
            cboSupCode.Properties.DataSource = dtSupplier;
            cboSupCode.Properties.DisplayMember = "BUMO";
            cboSupCode.Properties.ValueMember = "BUMO";
            //Datatable[1]
            DataColumn dc = new DataColumn();
            dc.ColumnName="SELECT";
            dc.DataType=typeof(bool);
            dc.DefaultValue = false;
            dtOrder.Columns.Add(dc);
            dtOrder.Columns["SELECT"].SetOrdinal(0);
            dc = new DataColumn();
            dc.ColumnName = "CANCEL";
            dc.DataType = typeof(bool);
            dc.DefaultValue = false;
            dtOrder.Columns.Add(dc);
            dtOrder.Columns["CANCEL"].SetOrdinal(1);
            dtOrder.Columns.Add("AMOUNT", typeof(decimal),"QTY*PRICE");
            dtOrder.Columns["SELECT"].Caption = "Select";
            dtOrder.Columns["CANCEL"].Caption = "Cancel";
            dtOrder.Columns["BUMO"].Caption = "Supplier";
            dtOrder.Columns["PORDER"].Caption = "Order No.";
            dtOrder.Columns["SEIBAN"].Caption = "Seiban No.";
            dtOrder.Columns["CODE"].Caption = "Part No.";
            dtOrder.Columns["NAME"].Caption = "Part Description";
            dtOrder.Columns["UNIT"].Caption = "Unit";
            dtOrder.Columns["QTY"].Caption = "Quantity";
            dtOrder.Columns["PRICE"].Caption = "Unit Price";
            dtOrder.Columns["AMOUNT"].Caption = "Amount";

        }
        private void DisplaySupplier(string strSupplierCode)
        {

                string strSQL = "SELECT NAME,ADR1,ADR2,ZIP,TEL,FAX,CURRE FROM XSECT WHERE BUMO='" + strSupplierCode + "'";
                DataTable dt = db.GetDataTable(strSQL);
                if (dt != null && dt.Rows.Count > 0)
                {
                    txtSupplierName.Text = dt.Rows[0]["NAME"].ToString();
                    txtAddress1.Text = dt.Rows[0]["ADR1"].ToString();
                    txtAddress2.Text = dt.Rows[0]["ADR2"].ToString();
                    txtAddress3.Text = dt.Rows[0]["ZIP"].ToString();
                    txtTelephone.Text = dt.Rows[0]["TEL"].ToString();
                    txtFax.Text = dt.Rows[0]["FAX"].ToString();
                    txtCurrency.Text = dt.Rows[0]["CURRE"].ToString();
                }

         }
        private void LoadItemCodeAndGridView(string strSupplierCode)//Distinct Itemcode from Gridview to cboItem
        {
            DataView dv = new DataView();
            dv.Table = dtOrder;
            if (strSupplierCode.Length > 0){dv.RowFilter = "BUMO='" + strSupplierCode + "'";}
            gridControl1.DataSource = dv;
            gridView1.Columns["SELECT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom, "{0:n0}");
            gridView1.Columns["SELECT"].SummaryItem.Tag = 1;
            gridView1.Columns["QTY"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom, "{0:n2}");
            gridView1.Columns["QTY"].SummaryItem.Tag = 2;
            gridView1.Columns["AMOUNT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom, "{0:n2}");
            gridView1.Columns["AMOUNT"].SummaryItem.Tag = 3;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            for (int i = 0; i < gridView1.Columns.Count; i++)
            {
                if (gridView1.Columns[i].FieldName != "SEIBAN")
                {
                    gridView1.Columns[i].BestFit();
                }
            }
            cboItem.Properties.Items.Clear();
            cboItem.EditValue = "";
            foreach (DataRowView drv in dv)
            {
                if (!cboItem.Properties.Items.Contains(drv["CODE"]))
                {
                    cboItem.Properties.Items.Add(drv["CODE"].ToString());
                }
            }
        }

        private void DisplayOrderBalance()
        {
            string strSQL = "EXEC spTPiCSSubsystem_PurchaseOrder_Load ''" +
                    ",''" +
                    ",'" + optClassification.SelectedIndex.ToString() + "','" + cboSection.Text + "'" +
                    ",'" + optStatus.SelectedIndex.ToString() + "','"+cboSupCode.EditValue+"'";
            DataSet ds = db.GetDataSet(strSQL);
            //Datatable[1]
            DataColumn dc = new DataColumn();
            dc.ColumnName = "SELECT";
            dc.DataType = typeof(bool);
            dc.DefaultValue = false;
            ds.Tables[1].Columns.Add(dc);
            ds.Tables[1].Columns["SELECT"].SetOrdinal(0);
            ds.Tables[1].Columns.Add("AMOUNT", typeof(decimal), "QTY*PRICE");
            ds.Tables[1].Columns["SELECT"].Caption = "Select";
            ds.Tables[1].Columns["BUMO"].Caption = "Supplier";
            ds.Tables[1].Columns["PORDER"].Caption = "Order No.";
            ds.Tables[1].Columns["SEIBAN"].Caption = "Seiban No.";
            ds.Tables[1].Columns["CODE"].Caption = "Part No.";
            ds.Tables[1].Columns["NAME"].Caption = "Part Description";
            ds.Tables[1].Columns["UNIT"].Caption = "Unit";
            ds.Tables[1].Columns["QTY"].Caption = "Quantity";
            ds.Tables[1].Columns["PRICE"].Caption = "Unit Price";
            ds.Tables[1].Columns["AMOUNT"].Caption = "Amount";
            gridControl3.DataSource = ds.Tables[1];
            gridView3.OptionsView.EnableAppearanceEvenRow = true;
            gridView3.OptionsView.EnableAppearanceOddRow = true;
            gridView3.OptionsView.ColumnAutoWidth = false;
            for (int i = 0; i < gridView3.Columns.Count; i++)
            {
                if (gridView3.Columns[i].FieldName != "SEIBAN")
                {
                    gridView3.Columns[i].BestFit();
                }
            }
        }
        private string GenIF(string pGenNo,bool Fupd)
        { 
            string sGenNo,sRun;
            string strSQL = "SELECT * FROM MGenOrdNo WHERE GenNo= '"+ pGenNo.Trim() + "'";
            DataTable dt=db.GetDataTable(strSQL);
            if(dt!=null && dt.Rows.Count>0)
            {
                DataRow dr=dt.Rows[0];
                sRun=(Convert.ToInt32(dr["RunNo"])+1).ToString("000000");
                sGenNo=pGenNo+"-"+sRun;
                if (Fupd)
                {
                    strSQL = "UPDATE MGenOrdNo SET RunNo='" + sRun + "',OrdNo='" + sGenNo + "' WHERE GenNo='" + pGenNo.Trim() + "'";
                    db.Execute(strSQL);
                }
            }
            else
            {
                sRun="000001";
                sGenNo=pGenNo+"-"+sRun;
                if (Fupd)
                {
                    strSQL = "INSERT INTO MGenOrdNo(GenNo,xYear,RunNo,OrdNo)VALUES("+
                        "'"+pGenNo.Trim()+"','"+DateTime.Today.ToString("yyyy",dtfinfo)+"'"+
                        ",'"+sRun+"','"+sGenNo+"')";
                    db.Execute(strSQL);
                }
            }
            return(sGenNo);
        }
        private string GenPONo(string pGenNo,bool Fupd)
        {
            string sGenNo,sRun,sYear,sMonth;
            string strSQL="SELECT * FROM MGenRecNo WHERE GenData ='"+pGenNo.Trim()+"'"+
                " AND xYear='"+DateTime.Today.ToString("yyyy",dtfinfo)+"' AND xMonth='"+DateTime.Today.ToString("MM",dtfinfo)+"'";
            DataTable dt=db.GetDataTable(strSQL);
            if(dt!=null && dt.Rows.Count>0)
            {
                DataRow dr=dt.Rows[0];
                sYear=dr["xYear"].ToString();
                sMonth=dr["xMonth"].ToString();
                sRun=(Convert.ToInt32(dr["RunNo"])+1).ToString("0000");
                sGenNo=pGenNo+sYear.Substring(2,2)+sMonth+"-"+sRun;
                if(Fupd)
                {
                    strSQL="UPDATE MGenRecNo SET RunNo='"+sRun+"',RecNo='"+sGenNo+"'"+ 
                        " WHERE GenData='"+pGenNo.Trim()+"' AND xYear='"+sYear+"' AND xMonth='"+sMonth+"'";
                    db.Execute(strSQL);
                }
            }
            else
            {
                sYear=DateTime.Today.ToString("yyyy",dtfinfo);
                sMonth=DateTime.Today.ToString("MM",dtfinfo);
                sRun="0001";
                sGenNo=pGenNo+sYear.Substring(2,2)+sMonth+"-"+sRun;
                if(Fupd)
                {
                    strSQL="INSERT INTO MGenRecNo(GenData,xYear,xMonth,RunNo,RecNo)VALUES("+
                        "'"+pGenNo.Trim()+"','"+sYear+"','"+sMonth+"','"+sRun+"','"+sGenNo+"')";
                    db.Execute(strSQL);
                }
            }
            return (sGenNo);
        }
        private void CheckREV()
        {
            iCan = 0;
            string noPO=cboPO.EditValue.ToString();
            string strSQL = "SELECT RevNo FROM THPurchase WHERE PONo = '"+ cboPO.EditValue +"'";
            DataTable dt=db.GetDataTable(strSQL);
            if(dt!=null && dt.Rows.Count>0 && optStatus.SelectedIndex==(int)enumOptStatus.REVISE)
            {
                int rNum=Convert.ToInt32(dt.Rows[0]["RevNo"]);
                if(rNum>=0)
                {
                    iCan = rNum + 1;
                    if(optClassification.SelectedIndex==0)
                    {
                        cboPO.EditValue=cboPO.EditValue.ToString().Substring(0,9)+"-"+"Rev."+iCan;
                    }
                    else
                    {
                        cboPO.EditValue=cboPO.EditValue.ToString().Substring(0,11)+"-"+"Rev."+iCan;
                    }
                }
            }
            if(optStatus.SelectedIndex==(int)enumOptStatus.REVISE)
            {
                strSQL = "UPDATE THPurchase SET CANORNO = '1' WHERE PONo ='"+ noPO+"'";
                db.Execute(strSQL);
            }
            if(optClassification.SelectedIndex==0)
            {
                strSQL = "UPDATE TDPOFB SET STATUS  = '3' WHERE PONo ='"+noPO+"'";
                db.Execute(strSQL);
            }
        }
        private void SaveTHPurchase()
        {
            string GPo;
            string fMat = "0";
            string pPrint = "";

            fMat = optClassification.SelectedIndex.ToString();
            //switch(optClassification.SelectedIndex)
            //{
            //    case 0:
            //        fMat="2";
            //        break;
            //    case 1:
            //        fMat="3";
            //        break;
            //    case 2:
            //        fMat="1";
            //        break;
            //}
            CheckREV();
            string strSQL = "SELECT COUNT(PONO) FROM THPurchase WHERE PONo ='"+ cboPO.EditValue + "'";
            if(Convert.ToInt32(db.ExecuteFirstValue(strSQL))==0)
            {
                if(optStatus.SelectedIndex==(int)enumOptStatus.REVISE)
                {
                    GPo = cboPO.EditValue.ToString();
                }
                else
                {    
                    if(cboSupCode.EditValue.ToString()=="FDEL")
                    {
                        GPo = GenIF("IF", true);
                    }
                    else
                    {
                        GPo = GenPONo("SX", true);
                    }
                    cboPO.EditValue = GPo;
                }
                strSQL = "INSERT INTO THPurchase (PONo,RevNo,PODate,SupCode,FlagMat,ReqNo,ReqDate,RefNo,Attn,Plant" +
                    ",Remark,FlgPrn,CANORNO,CrtUser,CrtDate,UpdUser,UpdDate,FSECTION,SCONT,SCONT1,SCONT2,FQTY" +
                    ",YQTY,FINISH,WEIGHT,MAXIMUM,SPIRAL,WASHING,WATER,RUBBING,PHVALUE) VALUES (" +
                    "'" + GPo + "','" + iCan + "','" + ((DateTime)dtpPO.EditValue).ToString("yyyyMMdd") + "'"+
                    ",'" + cboSupCode.Text + "'" +
                    //",'"+gridView1.GetRowCellValue(0,"BUMO")+"'"+
                    ",'" + fMat + "'" +
                    ",'" + txtReqNo.Text + "','" + txtReqDate.Text + "','" + txtRefNo.Text + "' ,'" + txtAttn.Text + "'" +
                    ",'" + txtPlant.Text + "',N'" + txtRemark.Text + "','" + pPrint + "','0','" + Module.strUserName + "'" +
                    ",'" + DateTime.Now.ToString("yyyy-MM-dd",dtfinfo) + "','" + lblUpdUser.Text + "','" + lblUpdDate.Text + 
                    "','" + cboSection.EditValue + "'" +
                    ",'','','','','','','','','','','','','')";
                    //",'" + txtSale.Text + "','" + txtSale1.Text + "','" + txtSale2.Text + "', '" + txtA.Text + "', '" + txtB.Text + "'"+
                    //",'" + txtC.Text + "', '" + txtD.Text + "','" + txtF.Text + "','" + txtG.Text + "','" + txtH1.Text & "'"+
                    //",'" + txtH2.Text + "','" + txtH3.Text + "','" + txtI.Text + "'
                db.Execute(strSQL);
            }
            else
            {
                strSQL = "UPDATE THPurchase SET Attn='" + txtAttn.Text + "',ReqNo='" + txtReqNo.Text + "'" +
                    ",ReqDate='" + txtReqDate.Text + "',RefNo ='" + txtRefNo.Text + "',Plant='" + txtPlant.Text + "'" +
                    ",Remark='" + txtRemark.Text + "',FSECTION='" + cboSection.Text + "',UpdUser='" + Module.strUserName + "'" +
                    ",UpdDate='" + DateTime.Now.ToString("yyyy-MM-dd",dtfinfo) + "'"+
                    //,SCONT='"+txtSale.Text+"',SCONT1='"+txtSale1.Text+"'"+
                    //",SCONT2='"+txtSale2.Text+"',FQTY='"+txtA.Text+"',YQTY='"+txtB.Text+"',FINISH='"+txtC.Text+"'"+
                    //",WEIGHT='"+txtD.Text+"',MAXIMUM='"+txtF.Text+"',SPIRAL='"+txtG.Text+"',WASHING='"+txtH1.Text+"'"+
                    //",WATER='"+txtH2.Text+"',RUBBING='"+txtH3.Text+"',PHVALUE='"+txtI.Text+"',FFAB='"+fFab+"'"+
                    " WHERE PONo='"+cboPO.EditValue+"'";
                db.Execute(strSQL);
            }
         }
        private void SaveTDPurchase()
        {
            string strSQL = "DELETE TDPurchase WHERE PONo = '" + cboPO.EditValue + "'";
            db.Execute(strSQL);
            for(int i=0;i<gridView1.DataRowCount;i++)
            {
                if((bool)gridView1.GetRowCellValue(i,"SELECT"))
                {
                    string[] arySeiban = (Module.SplitByLength(gridView1.GetRowCellValue(i, "SEIBAN").ToString(), 254)).ToArray();
                    string seiban0="";
                    string seiban1="";
                    string seiban2="";
                    switch (arySeiban.Length)
                    {
                        case 1: seiban0 = arySeiban[0]; break;
                        case 2: seiban0 = arySeiban[0]; seiban1 = arySeiban[1]; break;
                        case 3: seiban0 = arySeiban[0]; seiban1 = arySeiban[1]; seiban2 = arySeiban[2]; break;
                    }
                    strSQL = "INSERT INTO TDPurchase (PONo,TpicsNo,SBNo,PartNo,PartDel,Unit,Qty,UPrc,Amt,CrtUser,CrtDate" +
                        ",UpdUser,UpdDate,EDA,Contract,same3,same4,FlagSend,SendDate,SBNO1,SBNO2)VALUES(" +
                        "'" + cboPO.EditValue + "','" + gridView1.GetRowCellValue(i, "PORDER") + "'";
                    strSQL+=",'" + seiban0 + "'";
                    strSQL += ",'" + gridView1.GetRowCellValue(i, "CODE") + "','" + gridView1.GetRowCellValue(i, "NAME") + "'" +
                        ",'" + gridView1.GetRowCellValue(i, "UNIT") + "','" + (decimal)gridView1.GetRowCellValue(i, "QTY") + "'" +
                        ",'" + (decimal)gridView1.GetRowCellValue(i, "PRICE") + "','" + (decimal)gridView1.GetRowCellValue(i, "AMOUNT") + "'" +
                        ",'" + Module.strUserName + "','" + DateTime.Now.ToString("yyyy-MM-dd",dtfinfo) + "','" + lblUpdUser.Text + "','" + lblUpdDate.Text + "'" +
                        ",'" + gridView1.GetRowCellValue(i, "EDA") + "' ,'','','','',''";
                    strSQL += ",'" + seiban1 + "'";
                    strSQL+=",'"+seiban2+"')";
                    db.Execute(strSQL);
                }
            }
        }
        private void SaveTDPurchase2()
        {
            //Dim r, x As Integer
            //Dim sRemark As String
            //Dim sRemark1 As String
            //Dim sRemark2 As String
            //Dim sSei As String
            //Dim sDetail As String
            //string sStatus;
            //Dim sShow3, sShow4 As String
            //    x = 0
            //for(int i=0;i<gridView2.DataRowCount;i++)
            //{
            //    if((bool)gridView2.GetRowCellValue(i,"SELECT"))
            //    {
            //        string strSQL="INSERT INTO TDPOFB (PONo,RevNo,PODate,Cust,SupCode,DueDate,ShipDate,SewDate,POrder, " & _
            //                                 " EDA, SBNo, Code,Qty,POQty,CNFCode,CNFColor,Status,FSECTION,Remark,CrtBy,CrtDate,UpdBy,UpdDate,SBNO1,SBNO2, " & _
            //                                 " CODE2,SENDDATE,CNFDATE,INQTY,FBCODE,FBCOLOR) " & _
            //                                 " VALUES ('" & Trim(txtNo.Text) & "','" & CInt(iCan) & "','" & txtPODate.Text & "','" & .TextMatrix(r, 15) & "','" & cboSupCode.Text & "' " & _
            //                                 ",'" & .TextMatrix(r, 17) & "','" & .TextMatrix(r, 18) & "' ,'" & .TextMatrix(r, 19) & "','" & .TextMatrix(r, 4) & "'" & _
            //                                 ",'" & CInt(.TextMatrix(r, 14)) & "' , '" & Mid(.TextMatrix(r, 5), 1, 254) & "', '" & .TextMatrix(r, 6) & "' , '" & CDbl(.TextMatrix(r, 9)) & "' " & _
            //                                 ", '" & CDbl(.TextMatrix(r, 10)) & "' , '" & .TextMatrix(r, 21) & "' , '" & .TextMatrix(r, 22) & "' ,'" & sStatus & "','" & cbSection.Text & "' " & _
            //                                 ",'" & txtRemark.Text & "' , '" & lblCrtUser.Caption & "','" & Format(lblCrtDate.Caption, "yyyyMMdd") & "' " & _
            //                                 ",'" & lblUpdUser.Caption & "','" & Format(lblUpdDate.Caption, "yyyyMMdd") & "', '" & Mid(.TextMatrix(r, 5), 255, 254) & "', '" & Mid(.TextMatrix(r, 5), 509, 254) & "' " & _
            //                                 " ,  '" & .TextMatrix(r, 20) & "','' ,'" & .TextMatrix(r, 23) & "','" & CDbl(.TextMatrix(r, 10)) & "', '" & .TextMatrix(r, 21) & "' , '" & .TextMatrix(r, 22) & "' ) ";
            //        db.Execute(strSQL);
            //    }
            //}
        }
        private void SaveXSLIPPONUM()
        {
            if(optClassification.SelectedIndex==0)//Fabric
            {
                for(int i=0;i<gridView2.DataRowCount;i++)
                {
                    if((bool)gridView2.GetRowCellValue(i,"SELECT"))
                    {
                        string strSQL ="UPDATE XSLIP SET PONUM = '"+ cboPO.EditValue.ToString().Trim() +"'"+
                                " WHERE PORDER = '"+gridView2.GetRowCellValue(i,"PORDER")+"' AND CODE='"+gridView2.GetRowCellValue(i,"CODE")+"' AND PORDER LIKE 'XX%' ";
                        db.Execute(strSQL);
                    }
                }
            }
            else//Fabric Outside,Accessory
            {
                for(int i=0;i<gridView1.DataRowCount;i++)
                {
                    if((bool)gridView1.GetRowCellValue(i,"SELECT"))
                    {
                        string strSQL ="UPDATE XSLIP SET PONUM = '"+ cboPO.EditValue.ToString().Trim() +"'"+
                                " WHERE PORDER = '"+gridView1.GetRowCellValue(i,"PORDER")+"' AND CODE='"+gridView1.GetRowCellValue(i,"CODE")+"' AND PORDER LIKE 'XX%' ";
                        db.Execute(strSQL);
                    }
                }
            }

        }
        private void LoadPOBUMO(string strSupplierCode)
        {
            string strSQL = "EXEC spTPiCSFDI_PurchaseOrder_Load '" + ((DateTime)dtpStart.EditValue).ToString("yyyyMMdd", dtfinfo) + "'" +
                    ",'" + ((DateTime)dtpEnd.EditValue).ToString("yyyyMMdd", dtfinfo) + "'" +
                    ",'" + optClassification.SelectedIndex.ToString() + "','" + cboSection.Text + "'" +
                    ",'" + optStatus.SelectedIndex.ToString() + "','"+strSupplierCode+"'";
            DataSet ds = db.GetDataSet(strSQL);
            dtSupplier = ds.Tables[0];
            dtOrder = ds.Tables[1];
            //Datatable[0]
            cboSupCode.Properties.DataSource = dtSupplier;
            cboSupCode.Properties.DisplayMember = "BUMO";
            cboSupCode.Properties.ValueMember = "BUMO";
            //Datatable[1]
            DataColumn dc = new DataColumn();
            dc.ColumnName = "SELECT";
            dc.DataType = typeof(bool);
            dc.DefaultValue = false;
            dtOrder.Columns.Add(dc);
            dtOrder.Columns["SELECT"].SetOrdinal(0);
            dtOrder.Columns.Add("CANCEL", typeof(bool));
            dtOrder.Columns["CANCEL"].SetOrdinal(1);
            dtOrder.Columns.Add("AMOUNT", typeof(decimal), "QTY*PRICE");
            dtOrder.Columns["SELECT"].Caption = "Select";
            dtOrder.Columns["CANCEL"].Caption = "Cancel";
            dtOrder.Columns["BUMO"].Caption = "Supplier";
            dtOrder.Columns["PORDER"].Caption = "Order No.";
            dtOrder.Columns["SEIBAN"].Caption = "Seiban No.";
            dtOrder.Columns["CODE"].Caption = "Part No.";
            dtOrder.Columns["NAME"].Caption = "Part Description";
            dtOrder.Columns["UNIT"].Caption = "Unit";
            dtOrder.Columns["QTY"].Caption = "Quantity";
            dtOrder.Columns["PRICE"].Caption = "Unit Price";
            dtOrder.Columns["AMOUNT"].Caption = "Amount";
            gridControl1.DataSource = dtOrder;
            gridView1.Columns["SELECT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom, "{0:n0}");
            gridView1.Columns["SELECT"].SummaryItem.Tag = 1;
            gridView1.Columns["QTY"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom, "{0:n2}");
            gridView1.Columns["QTY"].SummaryItem.Tag = 2;
            gridView1.Columns["AMOUNT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom, "{0:n2}");
            gridView1.Columns["AMOUNT"].SummaryItem.Tag = 3;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            //gridView1.OptionsView.ColumnAutoWidth = true;
            //gridView1.BestFitColumns();
            LoadItemCodeAndGridView(strSupplierCode);
        }
        private void LoadGridCode()
        {
        
        }
        private void Load_ItemCode()
        {
        
        }
        private void SaveXSLIPUpdFlgPrint(string strPO)
        {

            db.ConnectionOpen();
            try
            {
                if (strPO.Length == 0) { throw new ApplicationException("ไม่ปรากฏหมายเลข PO flag print จะไม่ถูกเซฟ"); }
                db.BeginTrans();
                string strSQL = "UPDATE XSLIP SET ISSUE = 'Y'  WHERE PONUM = '" + strPO + "' AND PORDER LIKE 'XX%' ";
                db.Execute(strSQL);
                strSQL = "UPDATE THPurchase SET FlgPrn = 'Y'  WHERE PONo = '" + strPO + "'";
                db.Execute(strSQL);
                db.CommitTrans();
            }
            catch (SystemException ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error:flag print not save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();

        }
        private void frmPurchase_Load(object sender, EventArgs e)
        {
            dtfinfo = clinfo.DateTimeFormat;
            btnLoad.Enabled = false;
            dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            NewData(clearHeader:true,clearDetail:true,clearComboboxPO:true);
        }
        private void optClassification_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSection.SelectedIndex > -1) { btnLoad.Enabled = true; }
        }
        private void cboSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (optClassification.SelectedIndex > -1) { btnLoad.Enabled = true; }
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            this.Cursor=Cursors.WaitCursor;
            try 
	        {
                NewData(clearHeader: false, clearDetail: true, clearComboboxPO: true);
                LoadSupplier();
                LoadItemCodeAndGridView(strSupplierCode:"");
	        }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        private void cboSupCode_EditValueChanged(object sender, EventArgs e)
        {
            if (cboSupCode.EditValue.ToString().Length == 0) { return; }
            this.Cursor = Cursors.WaitCursor;
            try
            {
                DisplaySupplier(strSupplierCode: cboSupCode.EditValue.ToString());
                if (optStatus.SelectedIndex < 0) { LoadItemCodeAndGridView(strSupplierCode: cboSupCode.EditValue.ToString());}//ฟังก์ชั่นนี้ไม่ทำงานเมื่ออยู่ในสถานะ revise,reprint
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            switch (gv.FocusedColumn.FieldName)
            { 
                case "SELECT":
                case "SEIBAN":
                    e.Cancel = false;
                    break;
                case "CANCEL":
                    if (optStatus.SelectedIndex == (int)enumOptStatus.REVISE)
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                    break;
                default:
                    e.Cancel = true;
                    break;
            }
        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i=0; i < gridView1.DataRowCount; i++)
            {
                gridView1.SetRowCellValue(i, "SELECT", true);
            }
        }
        private void btnUnselect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                gridView1.SetRowCellValue(i, "SELECT", false);
            }
        }
        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            db.ConnectionOpen();
            try 
	        {	        
                for(int i=0;i<gridView1.DataRowCount;i++)
                {
                    if((bool)gridView1.GetRowCellValue(i,"CANCEL"))
                    {
                        if(MessageBox.Show("Do you want to Delete : '"+gridView1.GetRowCellValue(i,"PORDER")+ "'  : (Y/N) ","confirm data",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
                        {
                            //string strSQL= "DELETE TDPurchase WHERE PONo = '"+cboPO.EditValue+"' AND TPICSNO = '"+gridView1.GetRowCellValue(i,"PORDER")+"' AND PartNo ='"+gridView1.GetRowCellValue(i,"CODE")+"'";
                            //db.Execute(strSQL);
                            gridView1.DeleteRow(i);
                        }
                    }
                }
	        }
	        catch (Exception ex)
	        {
		        MessageBox.Show(ex.Message,"Delete row error",MessageBoxButtons.OK,MessageBoxIcon.Error);
	        }
            db.ConnectionClose();
        }
        private void gridView1_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            //Get the summary ID
            int summaryID = Convert.ToInt32(((DevExpress.XtraGrid.GridSummaryItem)e.Item).Tag);
            //Console.Write(summaryID);
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            ////Initialization
            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
            {
                switch (summaryID)
                { 
                    case 1:
                        intCountSelect = 0;
                        break;
                    case 2:
                        sumQTY = 0;
                        break;
                    case 3:
                        sumAmount = 0;
                        break;
                }
            }
            //Calculation
            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
            {
                bool isSelect = (bool)gv.GetRowCellValue(e.RowHandle, "SELECT");
                switch (summaryID)
                {
                    case 1:
                        if (isSelect)
                        {
                            intCountSelect++;
                        }
                        break;
                    case 2:
                        if (isSelect)
                        {
                            sumQTY += (decimal)gv.GetRowCellValue(e.RowHandle, "QTY");
                        }
                        break;
                    case 3:
                        if (isSelect)
                        {
                            sumAmount += (decimal)gv.GetRowCellValue(e.RowHandle, "AMOUNT");
                        }
                        break;
                }
            }
            //Finalization
            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
            {
                switch (summaryID)
                {
                    case 1:
                        e.TotalValue = intCountSelect;
                        break;
                    case 2:
                        e.TotalValue = sumQTY;
                        break;
                    case 3:
                        e.TotalValue = sumAmount;
                        break;
                }
            }
        }
        private void cboItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboItem.EditValue.ToString().Length > 0)
            {
                DataView dv = new DataView();
                dv.Table = dtOrder;
                dv.RowFilter = "BUMO='"+cboSupCode.EditValue.ToString()+"' AND CODE='"+cboItem.EditValue.ToString()+"'";
                gridControl1.DataSource = dv;
            }
        }
        private void optStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(optStatus.SelectedIndex)
            {
                case (int)enumOptStatus.REPRINT:
                    btnChangeSup.Enabled = false;
                    btnDeleteRow.Enabled = false;
                    btnDelete.Enabled = false;
                    NewData(clearHeader: false, clearDetail: true, clearComboboxPO: true);
                    LoadPO();
                    break;
                case (int)enumOptStatus.REVISE:
                    btnChangeSup.Enabled = true;
                    btnDeleteRow.Enabled = true;
                    btnDelete.Enabled = true;
                    NewData(clearHeader: false, clearDetail: true, clearComboboxPO: true);
                    LoadPO();
                    break;
                default:
                    btnChangeSup.Enabled = false;
                    btnDeleteRow.Enabled = false;
                    btnDelete.Enabled = false;
                    cboPO.Properties.Items.Clear();
                    break;
            }
          }
        private void cboPO_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPO.EditValue.ToString().Length == 0) { return; }//ในกรณีเคลียร์ combobox ให้ออกจากสับทันที
            if (!cboPO.Properties.Items.Contains(cboPO.EditValue)) { return; }//ค่าใน editvalue ไม่ได้อยู่ในลิสต์ของ combobox ให้ออกจากสับทันที
            this.Cursor = Cursors.WaitCursor;
            try
            {
                NewData(clearHeader: false, clearDetail: true, clearComboboxPO: false);
                DisplayPO();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        private void ActiveEditor_MouseUp(object sender, MouseEventArgs e)
        {
            if (gridView3.FocusedColumn.FieldName == "SELECT" && e.Button==MouseButtons.Left)
            {
                if (!(bool)gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "SELECT"))
                {
                    gridView1.AddNewRow();
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle,"BUMO",gridView3.GetRowCellValue(gridView3.FocusedRowHandle,"BUMO"));
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "PORDER", gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "PORDER"));
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "SEIBAN", gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "SEIBAN"));
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "CODE", gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "CODE"));
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "NAME", gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "NAME"));
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "UNIT", gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "UNIT"));
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "QTY", gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "QTY"));
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "PRICE", gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "PRICE"));
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "AMOUNT", gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "AMOUNT"));
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "EDA", gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "EDA"));
                }
            }
        }
        private void gridView3_ShownEditor(object sender, EventArgs e)
        {
            gridView3.ActiveEditor.MouseUp+=new MouseEventHandler(ActiveEditor_MouseUp);
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if((optStatus.SelectedIndex!=(int)enumOptStatus.REVISE)||(cboPO.EditValue.ToString().Length==0)){return;}
            this.Cursor=Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                if (optClassification.SelectedIndex == (int)enumOptClassification.FABRIC)
                {
                    if (MessageBox.Show("Do you want to cancel : '" + cboPO.EditValue + "'  : (Y/N) ", "Cancel P/O", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        string strSQL = " UPDATE TDPOFB SET STATUS = '3' WHERE PONo = '" + cboPO.EditValue + "'";
                        db.Execute(strSQL);
                        strSQL = " UPDATE THPurchase SET CANORNO = '1' WHERE PONo = '" + cboPO.EditValue + "'";
                        db.Execute(strSQL);
                    }
                    else
                    {
                        throw new ApplicationException();
                    }
                }
                else
                {
                    if (MessageBox.Show("Do you want to cancel : '" + cboPO.EditValue + "'  : (Y/N) ", "Cancel P/O", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        string strSQL = "DELETE THPurchase WHERE PONo = '" + cboPO.EditValue + "'";
                        db.Execute(strSQL);
                        strSQL = "DELETE TDPurchase WHERE PONo = '" + cboPO.EditValue + "'";
                        db.Execute(strSQL);
                        strSQL = "UPDATE XSLIP SET ISSUE = 'N' , PONUM = '' WHERE PONUM = '" + cboPO.EditValue + "'";
                        db.Execute(strSQL);
                    }
                    else
                    {
                        throw new ApplicationException();
                    }
                }
                db.CommitTrans();
                NewData(false, true, true);
            }
            catch (SystemException ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException)
            {
                db.RollbackTrans();
            }
            db.ConnectionClose();
            this.Cursor=Cursors.Default;

            //Dim r As Integer
    //On Error GoTo ErrHdl
    //If opRevice.Value = True And txtNo.Text <> "" Then
    //    ConnectSQLIN
    //    If chFabric.Value = True Then
    //        If MsgBox("Do you want to Cancel : '" & txtNo.Text & "'  : (Y/N) ", vbYesNo, "confirm data") = vbNo Then Exit Sub
    //        s_SQL = " UPDATE TDPOFB SET STATUS = '3' WHERE PONo = '" & txtNo.Text & "'"
    //        ConnectX (s_SQL)
    //        s_SQL = " UPDATE THPurchase SET CANORNO = '1' WHERE PONo = '" & txtNo.Text & "'"
    //        ConnectX (s_SQL)
    //    Else
    //        If MsgBox("Do you want to Delete : '" & txtNo.Text & "'  : (Y/N) ", vbYesNo, "confirm data") = vbNo Then Exit Sub
    //        s_SQL = "DELETE THPurchase WHERE PONo = '" & txtNo.Text & "' "
    //        ConnectX (s_SQL)
    //        s_SQL = "DELETE TDPurchase WHERE PONo = '" & txtNo.Text & "' "
    //        ConnectX (s_SQL)
    //        s_SQL = "UPDATE XSLIP SET ISSUE = 'N' , PONUM = '' WHERE PONUM = '" & txtNo.Text & "' "
    //        ConnectX (s_SQL)
    //    End If
    //    cmdClear_Click
    //End If
//ErrHdl:
//    If Err.Number <> 0 Then MsgBox Err.Description, vbCritical + vbOKOnly, "Error Save TDPurchase"
        }
        private void btnChangeSup_Click(object sender, EventArgs e)
        {
            try
            {
                string result=Interaction.InputBox("input new supplier code.", "Change supplier", cboSupCode.EditValue.ToString());
                if (result.Length == 0) { throw new ApplicationException("no supplier code!"); }
                string strSQL = "SELECT BUMO,NAME FROM XSECT WHERE BUMO='" + result + "'";
                dtSupplier= db.GetDataTable(strSQL);
                if (dtSupplier != null && dtSupplier.Rows.Count > 0)
                {
                    cboSupCode.Properties.DataSource = dtSupplier;
                    cboSupCode.Properties.DisplayMember = "BUMO";
                    cboSupCode.Properties.ValueMember = "BUMO";
                }
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }



        }

    }
}