using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using myClass;
using Microsoft.Win32;

namespace TUW_System.YS
{
    public partial class frmYS_Receive : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        DataTable dtRecNo,dtSerial,dtPO;
        string receiveType;//ประเภทการรับ Receive,Return
        string strNo;//เลขรันแสดงลำดับเลขที่เอกสารล่าสุด
        string barcodePrinter;
        short printCopy;
        decimal price;
        bool checkBalance=false;//ตัวแปรบอกสถานะว่าต้องทำการเช็ค balance ใน slepo หรือไม่

        private const decimal lbs = 2.2046M;//2262185M;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        public LogIn User_Login { get; set; }

        public frmYS_Receive()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            try
            {
                cboType.SelectedIndexChanged -= cboType_SelectedIndexChanged;
                cboType.Text = "";
                cboType.SelectedIndexChanged += cboType_SelectedIndexChanged;
                sleRecNo.EditValueChanged -= sleRecNo_EditValueChanged;
                sleRecNo.EditValue = null;
                sleRecNo.Properties.DataSource = null;
                sleRecNo.EditValueChanged += sleRecNo_EditValueChanged;
                slePO.EditValueChanged -= slePO_EditValueChanged;
                slePO.EditValue = null;
                slePO.Properties.DataSource = null;
                slePO.EditValueChanged += slePO_EditValueChanged;
                ClearData();
                ClearYarnDetail();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public bool SaveData()
        {
            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            bool success;
            try
            {
                //SumTotal();
                db.BeginTrans();
                string strSQL;
                string strType = "";
                int count = 0;

                switch (cboType.Text)
                {
                    case "รับจากซื้อภายในประเทศ":
                        strType = "Local";
                        break;
                    case "รับจากซื้อต่างประเทศ":
                        strType = "Import";
                        break;
                    case "รับจากการจ้างย้อม":
                    case "รับจากการจ้างกรอ":
                        strType = "Sub Contract";
                        break;
                    case "รับคืนจากโรงทอ":
                        strType = "Return from Knitting";
                        break;
                    case "รับคืนจากจ้างทอ":
                        strType = "Return from Subcontract";
                        break;
                    case "รับคืนจากจ้างย้อม":
                    case "รับคืนจากจ้างกรอ":
                        strType = "Return from Subcontract";
                        break;
                    case "รับคืนจากทำตัวอย่าง":
                        strType = "Return from Sample";
                        break;
                    case "รับสินค้าตัวอย่าง":
                        strType = "Receive for Sample";
                        break;
                    case "รับคืนจาก Supplier":
                        strType = "Return from Vender";
                        break;
                }

                switch (cboType.Text)
                {
                    case "รับจากซื้อภายในประเทศ":
                    case "รับจากซื้อต่างประเทศ":
                    case "รับจากการจ้างย้อม":
                    case "รับสินค้าตัวอย่าง":
                    case "รับจากการจ้างกรอ":
                        //YarnReceive
                        strSQL = "SELECT Count(RecNo) FROM YarnReceive WHERE RecNo = '" + sleRecNo.Text + "'";
                        if (db.ExecuteFirstValue(strSQL) == "0")
                        {
                            //if (txtReceiveNo.Text.Length == 0)
                            //{
                            //    strSQL = "EXEC spTUWSystem_RunNo 'SALES5_RECEIVE',''";
                            //    string strReceive = db.ExecuteFirstValue(strSQL);
                            //    txtReceiveNo.Text = strReceive;
                            //}
                            strSQL = "INSERT INTO YarnReceive (RecNo,RecRun,RecDate,RecFrom,RecBy,PONo,RecType,DelNo,InvNo,GoodsUnit" +
                                ",CurrencyUnit,Discount) VALUES ('" + sleRecNo.Text + "','" + strNo + "'," +
                                "'" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd") + "'," +
                                "'" + txtSupplier.Text + "','" + txtURec.Text + "','" + slePO.Text + "','" + strType + "',"+
                                "'" + txtDelNo.Text + "','" + txtInv.Text + "','" + txtUnit.Text + "','" + txtCurrency.Text + "',"+
                                "'" + txtDiscount.Text + "')";
                            db.Execute(strSQL);
                        }
                        else
                        {
                            strSQL = "UPDATE YarnReceive SET DelNo = '" + txtDelNo.Text + "',InvNo = '" + txtInv.Text + "',PONO='"+slePO.Text+"' "+
                                "WHERE RecNo = '" + sleRecNo.Text + "'";
                            db.Execute(strSQL);
                        }
                        //YarnReceiveDetail
                        for (int i = 0; i < gridView3.DataRowCount; i++)
                        {
                            strSQL = "Select Count(YarnSerial) From YarnReceiveDetail Where RecNo = '" + sleRecNo.Text + "' " +
                                "and YarnSerial = '" + gridView3.GetRowCellDisplayText(i, "YARNSERIAL") + "'";
                            if (db.ExecuteFirstValue(strSQL) == "0")
                            {
                                strSQL = "INSERT INTO YarnReceiveDetail (RecNo,YarnID,YarnSerial,Code,NetWeight,LotNo,UnitPrice) VALUES (" +
                                    "'" + sleRecNo.Text + "','" + gridView3.GetRowCellDisplayText(i, "YARNID") + "'," +
                                    "'" + gridView3.GetRowCellDisplayText(i, "YARNSERIAL") + "','" + gridView3.GetRowCellDisplayText(i, "CODE") + "'," +
                                    "'" + gridView3.GetRowCellValue(i, "NETWEIGHT") + "','" + gridView3.GetRowCellDisplayText(i, "LOTNO") + "',"+
                                    "'" + gridView3.GetRowCellValue(i,"UNITPRICE")+"')";
                                db.Execute(strSQL);
                                strSQL = "UPDATE YARNGENBARCODE SET SYSDELETE=0 WHERE SERIAL='" + gridView3.GetRowCellDisplayText(i, "YARNSERIAL") + "'";
                                db.Execute(strSQL);
                                count += 1;
                            }
                            else
                            {

                            }
                        }
                        //PO_Receive
                        strSQL = "SELECT COUNT(RECEIVENO) FROM PO_RECEIVE WHERE RECEIVENO='" + sleRecNo.Text + "'";
                        if (db.ExecuteFirstValue(strSQL) == "0")
                        {
                            strSQL = "INSERT INTO PO_RECEIVE(PONO,RECEIVENO,RECEIVEDATE,INVOICENO,DELIVERYNO,DELIVERYDATE,CURRENCYUNIT," +
                                "VAT,DISCOUNT,CANCEL,IDSUP,EMPCODE,DEPARTMENT,MONEYRATE,INPUTDATE,INPUTUSER) ";
                            strSQL += "SELECT PONO,'" + sleRecNo.Text + "','" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'," +
                                "N'" + txtInv.Text + "',N'" + txtDelNo.Text + "','" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "',";
                            strSQL += "CURRENCYUNIT,VAT,'";
                            strSQL += (txtDiscount.Text.Length == 0) ? 0 : Convert.ToDecimal(txtDiscount.Text);
                            strSQL += "',0,IDSUP,'" + User_Login.EmployeeCode + "','FABRIC CONTROL','";
                            strSQL += (txtRate.Text.Length == 0) ? 0 : Convert.ToDecimal(txtRate.Text);
                            strSQL += "',GETDATE(),'" + User_Login.UserName + "' " +
                                "FROM PO_PURCHASE WHERE PONO='" + slePO.Text + "'";
                            db.Execute(strSQL);
                        }
                        else
                        {
                             strSQL = "UPDATE PO_RECEIVE SET " +
                                "PONO='" + slePO.Text + "'," +
                                "RECEIVEDATE='" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd", dtfinfo)+"',";
                            strSQL += (txtInv.Text == "") ? "INVOICENO=NULL," : "INVOICENO=N'" + txtInv.Text + "',";
                            strSQL += "INVOICEDATE=NULL,";
                            strSQL += (txtDelNo.Text == "") ? "DELIVERYNO=NULL," : "DELIVERYNO=N'" + txtDelNo.Text + "',";
                            strSQL += "DELIVERYDATE=NULL,";
                            strSQL += (txtCurrency.Text == "") ? "CURRENCYUNIT=NULL," : "CURRENCYUNIT='" + txtCurrency.Tag.ToString() + "',";
                            strSQL += (txtVat.Text == "") ? "VAT=0," : "VAT=" + txtVat.Text+",";
                            //strSQL += (chkCancel.Checked) ? "CANCEL='1'," : ",CANCEL='0',";
                            strSQL += (txtSupplier1.Text == "") ? "IDSUP=NULL," : "IDSUP='" + txtSupplier1.Tag.ToString() + "',";
                            strSQL += "EMPCODE='" + User_Login.EmployeeCode + "',DEPARTMENT='FABRIC CONTROL',";
                            strSQL += "INPUTDATE=GETDATE(),INPUTUSER='" + User_Login.UserName + "' ";
                            strSQL += "WHERE RECEIVENO='" + sleRecNo.Text + "'";
                            db.Execute(strSQL);

                            //strSQL += (txtDiscount.Text.Length == 0) ? "A.DISCOUNT=0" : "A.DISCOUNT='" + txtDiscount.Text + "'";
                            //strSQL += ",A.CANCEL='0'," +
                            //strSQL += (txtRate.Text.Length == 0) ? "A.MONEYRATE=0" : "A.MONEYRATE='" + txtRate.Text + "'";
                        }
                        //PO_ReceiveDetail
                        strSQL = "DELETE FROM PO_RECEIVEDETAIL WHERE RECEIVENO='" + sleRecNo.Text + "'";
                        db.Execute(strSQL);
                        for (int i = 0; i < gridView1.DataRowCount; i++)
                        {
                            strSQL = "INSERT INTO PO_RECEIVEDETAIL(PONO,RECEIVENO,PRODUCTCODE,PRODUCTNAME,DESCRIPTION,IDUNIT,SCHEDULEQTY," +
                                "QTY,UNITPRICE,AMOUNT,LINE) SELECT PONO,'" + sleRecNo.Text + "',PRODUCTCODE,PRODUCTNAME,DESCRIPTION,IDUNIT,";
                            strSQL += "'" + (decimal)gridView1.GetRowCellValue(i, "SCHEDULE") + "',";
                            strSQL += "'" + gridView1.GetRowCellValue(i, "INVENTORY") + "',";
                            strSQL += "'" + gridView1.GetRowCellValue(i, "PRICE") + "',";
                            strSQL += "'" + (decimal)gridView1.GetRowCellValue(i, "INVENTORY") * (decimal)gridView1.GetRowCellValue(i, "PRICE") + "',";
                            strSQL+="1 FROM PO_PURCHASEDETAIL WHERE PONO='" + slePO.Text + "'";
                            db.Execute(strSQL);
                        }
                        break;
                    case "รับคืนจากโรงทอ":
                    case "รับคืนจากจ้างทอ":
                    case "รับคืนจากจ้างย้อม":
                    case "รับคืนจากทำตัวอย่าง":
                    case "รับคืนจากจ้างกรอ":
                    case "รับคืนจาก Supplier":
                        //YarnReturn
                        strSQL = "SELECT Count(RetNo) FROM YarnReturn WHERE RetNo = '" + sleRecNo.Text + "'";
                        if (db.ExecuteFirstValue(strSQL) == "0")
                        {
                            strSQL = "INSERT INTO YarnReturn (RetNo,RetRun,RetDate,RetFrom,RetUse,RetType,PoNo_LotNo,DelNo)VALUES(" +
                                "'" + sleRecNo.Text + "','" + strNo + "','" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd") + "'," +
                                "'" + cboUDel.Text + "','" + txtURec.Text + "','" + strType + "','" + txtLot.Text + "','" + txtDelNo.Text + "')";
                            db.Execute(strSQL);
                        }
                        else
                        {
                            strSQL = "UPDATE YarnReturn SET PONo_LotNo = '" + txtLot.Text + "',RetFrom = '" + cboUDel.Text + "'," +
                                "RetUse = '" + txtURec.Text + "',DelNo = '" + txtDelNo.Text + "' WHERE RetNo = '" + sleRecNo.Text + "'";
                            db.Execute(strSQL);
                        }
                        //YarnReturnDetail
                        for (int i = 0; i < gridView3.DataRowCount; i++)
                        {
                            strSQL = "Select Count(YarnSerial) From YarnReturnDetail Where RetNo = '" + sleRecNo.Text + "' " +
                                "and YarnSerial = '" + gridView3.GetRowCellDisplayText(i, "YARNSERIAL") + "'";
                            db.Execute(strSQL);
                            if (db.ExecuteFirstValue(strSQL) == "0")
                            {
                                strSQL = "INSERT INTO YarnReturnDetail (RetNo,YarnID,YarnSerial,Code,NetWeight,LotNo,UnitPrice)VALUES(" +
                                "'" + sleRecNo.Text + "','" + gridView3.GetRowCellDisplayText(i, "YARNID") + "'," +
                                "'" + gridView3.GetRowCellDisplayText(i, "YARNSERIAL") + "','" + gridView3.GetRowCellDisplayText(i, "CODE") + "'," +
                                "'" + gridView3.GetRowCellValue(i, "NETWEIGHT") + "','" + gridView3.GetRowCellDisplayText(i, "LOTNO") + "',"+ 
                                "'"+ gridView3.GetRowCellValue(i,"UNITPRICE")+ "')";
                                db.Execute(strSQL);
                                strSQL = "UPDATE YARNGENBARCODE SET SYSDELETE=0 WHERE SERIAL='" + gridView3.GetRowCellDisplayText(i, "YARNSERIAL") + "'";
                                db.Execute(strSQL);
                                count += 1;
                            }
                            else
                            {

                            }
                        }
                        break;
                }
                db.CommitTrans();
                lblCtn.Text = count.ToString();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (receiveType == "Receive" && success==true)
            {
                SetAveragePrice(((DateTime)dtpDate.EditValue).ToString("yyyyMM",dtfinfo),Convert.ToInt32(gridView1.GetRowCellDisplayText(0, "CODE")));
                string po = slePO.Text;
                //refresh po list
                dtPO = GetPO(cboType.Text);
                slePO.Properties.DataSource = null;
                slePO.Properties.DataSource = dtPO;
                slePO.Properties.DisplayMember = "PONO";
                slePO.Properties.ValueMember = "PONO";
                slePO.Properties.PopulateViewColumns();
                slePO.Properties.View.Columns["QTY"].Visible = false;
                slePO.Properties.View.Columns["ACTUAL"].Visible = false;
                slePO.Properties.View.OptionsView.ShowAutoFilterRow = true;
                slePO.Properties.View.OptionsView.ColumnAutoWidth = false;
                slePO.Properties.View.BestFitColumns();
                //refresh po detail in gridView1
                slePO.EditValue = "";
                slePO.EditValue = po;
            }

            db.ConnectionClose();
            this.Cursor = Cursors.Default;
            return success;
        }
        public void PrintPreview()
        {
            cCrystalReport crpReceive = new cCrystalReport(Application.StartupPath + @"\Report\S5\S5_ReceiveNote.rpt");   //ReceiveNote-Parfun.rpt");
            if (crpReceive.SetPrinter(printCopy) == false) { return; }
            crpReceive.ReportTitle = sleRecNo.Text;
            for (int i = 1; i <= crpReceive.ReportCopy; i++)
            {
                crpReceive.ClearParameters();
                crpReceive.SetParameter("Copy", i.ToString());
                string fmlText = "{PO_RECEIVE.RECEIVENO}='" + sleRecNo.Text + "'";
                crpReceive.PrintReport(fmlText, false, "sa", "ZAQ113m4tuw");
            }
        }
        public void Print()
        {
            cCrystalReport crpReceive = new cCrystalReport(Application.StartupPath + @"\Report\S5\S5_ReceiveNote.rpt");
            if (crpReceive.SetPrinter(printCopy) == false) { return; }
            crpReceive.ReportTitle = sleRecNo.Text;
            for (int i = 1; i <= crpReceive.ReportCopy; i++)
            {
                crpReceive.ClearParameters();
                crpReceive.SetParameter("Copy", i.ToString());
                string fmlText = "{PO_RECEIVE.RECEIVENO}='" + sleRecNo.Text + "'";
                crpReceive.PrintReport(fmlText, true, "sa", "ZAQ113m4tuw");
            }
        }
        #region "ClearData"
            private void ClearYarnDetail()
            {
                txtSerial.Text = "";
                txtID.Text = "";
                txtCode.Text = "";
                txtMixed.Text = "";
                txtType.Text = "";
                txtColor.Text = "";
                txtSpecial.Text = "";
                txtSupplier.Text = "";
                txtSection.Text = "";
                txtBarID.Text = "";
                txtBarSerial.Text = "";
                txtBCode.Text = "";
                txtBarCtn.Text = "";
                txtBarWeight.Text = "";
            }
            public void ClearData()
            {
                dtpDate.EditValueChanging -= dtpDate_EditValueChanging;
                dtpDate.EditValue = DateTime.Today;
                dtpDate.EditValueChanging += dtpDate_EditValueChanging;
                txtLot.Text = "";
                //sleSupplier.EditValue = null;
                cboSupplier.Text = "";
                txtInv.Text = "";
                txtDelNo.Text = "";
                cboUDel.Text = "";
                txtURec.Text = "";

                optOrder.SelectedIndex = -1;
                chkBOI.Checked = false;
                chkCancel.Checked = false;
                txtCtn.Text = "";
                txtWeight.Text = "";
                optWeightType.SelectedIndex = 0;
                txtErase.Text = "";
                optKgs.SelectedIndex = 0;
                chkGW.Checked = false;

                tabbedControlGroup1.SelectedTabPageIndex = 0;

                gridControl1.DataSource = null;

                gridControl3.DataSource = null;
                dtSerial = new DataTable();
                DataColumn dc = new DataColumn();
                dc.Caption = "Delete";
                dc.ColumnName = "DEL";
                dc.DataType = typeof(bool);
                dtSerial.Columns.Add(dc);
                dc = new DataColumn();
                dc.Caption = "Yarn ID";
                dc.ColumnName = "YARNID";
                dc.DataType = typeof(int);
                dtSerial.Columns.Add(dc);
                dc = new DataColumn();
                dc.Caption = "Code";
                dc.ColumnName = "CODE";
                dc.DataType = typeof(string);
                dtSerial.Columns.Add(dc);
                dc = new DataColumn();
                dc.Caption = "Ctn No.";
                dc.ColumnName = "CTNNO";
                dc.DataType = typeof(string);
                dtSerial.Columns.Add(dc);
                dc = new DataColumn();
                dc.Caption = "Serial";
                dc.ColumnName = "YARNSERIAL";
                dc.DataType = typeof(string);
                dtSerial.Columns.Add(dc);
                dc = new DataColumn();
                dc.Caption = "Weight(Kgs.)";
                dc.ColumnName = "NETWEIGHT";
                dc.DataType = typeof(decimal);
                dtSerial.Columns.Add(dc);
                dc = new DataColumn();
                dc.Caption = "Lot No.";
                dc.ColumnName = "LOTNO";
                dc.DataType = typeof(string);
                dtSerial.Columns.Add(dc);
                dc = new DataColumn();
                dc.Caption = "Price";
                dc.ColumnName = "UNITPRICE";
                dc.DataType = typeof(decimal);
                dtSerial.Columns.Add(dc);
                gridControl3.DataSource = dtSerial;
                gridView3.Columns["NETWEIGHT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gridView3.Columns["NETWEIGHT"].DisplayFormat.FormatString = "n2";
                gridView3.OptionsView.EnableAppearanceEvenRow = true;
                gridView3.OptionsView.EnableAppearanceOddRow = true;
                gridView3.OptionsView.ColumnAutoWidth = false;

                txtSupplier1.Text = "";
                txtSupplier2.Text = "";
                txtSupplier3.Text = "";
                txtSupplier4.Text = "";
                txtCurrency.Text = "";
                txtRate.Text = "";
                txtVat.Text = "";
                txtUnit.Text = "";

                txtTCtn.Text = "";
                txtTKgs.Text = "";
                txtTLbs.Text = "";
                lblCtn.Text = "";
            }
        #endregion
        
        private DataTable GetReceiveNo(string strType,DateTime datSearch)
        {
            string strSQL = "";
            switch (strType)
            { 
                case "รับจากซื้อภายในประเทศ":
                    strSQL= "SELECT RECNO FROM YarnReceive WHERE RecNo like 'Y%' AND LEN(RECNO)=10 "+
                        "and CONVERT(char(7), RecDate, 120) = '"+ datSearch.ToString("yyyy-MM",dtfinfo) + "' Order by RecNo";
                    receiveType = "Receive";
                    break;
                case "รับจากซื้อต่างประเทศ":
                    strSQL = "SELECT RECNO FROM YarnReceive WHERE RecNo like 'IMY%' "+
                        "and CONVERT(char(7), RecDate, 120) = '"+ datSearch.ToString("yyyy-MM",dtfinfo)+ "' Order by RecNo";
                    receiveType = "Receive";
                    break;
                case "รับจากการจ้างย้อม":
                case "รับจากการจ้างกรอ":
                    strSQL = "SELECT RECNO FROM YarnReceive WHERE RecNo like 'SY%' "+
                        "and CONVERT(char(7), RecDate, 120) = '" + datSearch.ToString("yyyy-MM",dtfinfo) + "' Order by RecNo";
                    receiveType = "Receive";
                    break;
                case "รับคืนจากโรงทอ":
                    strSQL = "SELECT RetNo AS RECNO FROM YarnReturn WHERE RetNo like 'RK%' "+
                        "and CONVERT(char(7), RetDate, 120) = '" + datSearch.ToString("yyyy-MM",dtfinfo) + "' Order by RetNo";
                    receiveType = "Return";
                    break;
                case "รับคืนจากจ้างทอ":
                    strSQL = "SELECT RetNo AS RECNO FROM YarnReturn WHERE RetNo like 'RC%' " +
                        "and CONVERT(char(7), RetDate, 120) = '" + datSearch.ToString("yyyy-MM", dtfinfo) + "' Order by RetNo";
                    receiveType = "Return";
                    break;
                case "รับคืนจากจ้างย้อม":
                case "รับคืนจากจ้างกรอ":
                    strSQL = "SELECT RetNo AS RECNO FROM YarnReturn WHERE RetNo like 'RC%' "+
                        "and CONVERT(char(7), RetDate, 120) = '" + datSearch.ToString("yyyy-MM",dtfinfo) + "' Order by RetNo";
                    receiveType = "Return";
                    break;
                case "รับคืนจากทำตัวอย่าง":
                    strSQL = "SELECT RetNo AS RECNO FROM YarnReturn WHERE RetNo like 'RS%' "+
                        "and CONVERT(char(7), RetDate, 120) = '" + datSearch.ToString("yyyy-MM",dtfinfo) + "' Order by RetNo";
                    receiveType = "Return";
                    break;
                case "รับสินค้าตัวอย่าง":
                    strSQL = "SELECT RECNO FROM YarnReceive WHERE RecNo LIKE 'YS%' "+ 
                        "and CONVERT(char(7), RecDate, 120) = '" + datSearch.ToString("yyyy-MM",dtfinfo) + "' Order by RecNo";
                    receiveType = "Receive";
                    break; 
                case "รับคืนจาก Supplier":
                    strSQL = "SELECT RetNo AS RECNO FROM YarnReturn WHERE RetNo LIKE 'RV%' "+
                        "and CONVERT(char(7), RetDate, 120) = '" + datSearch.ToString("yyyy-MM",dtfinfo) + "' Order by RetNo";
                    receiveType = "Return";
                    break;
            }
            dtRecNo = db.GetDataTable(strSQL);
            return dtRecNo;
        }
        private string RunReceiveNoNew(string strRecType)
        {
            string strSQL="";
            string strType="";
            switch (strRecType)
            { 
                case "รับจากซื้อภายในประเทศ":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(RecRun)+1,1))),3) as RecRun FROM YarnReceive WHERE RecNo like 'Y%' and CONVERT(char(7), RecDate, 120) = '" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM", dtfinfo) + "'";
                    strType = "Y";
                    break;
                case "รับจากซื้อต่างประเทศ":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(RecRun)+1,1))),3) as RecRun FROM YarnReceive WHERE RecNo like 'IMY%' and CONVERT(char(7), RecDate, 120) = '" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM", dtfinfo) + "'";
                    strType = "IMY";
                    break;
                case "รับจากการจ้างย้อม":
                case "รับจากการจ้างกรอ":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(RecRun)+1,1))),3) as RecRun FROM YarnReceive WHERE RecNo like 'SY%' and CONVERT(char(7), RecDate, 120) = '" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM", dtfinfo) + "'";
                    strType = "SY";
                    break;
                case "รับคืนจากโรงทอ":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(RetRun)+1,1))),3) as RecRun FROM YarnReturn WHERE RetNo like 'RK%' and CONVERT(char(7), RetDate, 120) = '" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM", dtfinfo) + "'";
                    strType = "RK";
                    break;
                case "รับคืนจากจ้างทอ":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(RetRun)+1,1))),3) as RecRun FROM YarnReturn WHERE RetNo like 'RC%' and CONVERT(char(7), RetDate, 120) = '" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM", dtfinfo) + "'";
                    strType = "RC";
                    break;
                case "รับคืนจากจ้างย้อม":
                case "รับคืนจากจ้างกรอ":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(RetRun)+1,1))),3) as RecRun FROM YarnReturn WHERE RetNo like 'RC%' and CONVERT(char(7), RetDate, 120) = '" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM", dtfinfo) + "'";
                    strType = "RC";
                    break;
                case "รับคืนจากทำตัวอย่าง":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(RetRun)+1,1))),3) as RecRun FROM YarnReturn WHERE RetNo like 'RS%' and CONVERT(char(7), RetDate, 120) = '" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM", dtfinfo) + "'";
                    strType = "RS";
                    break;
                case "รับสินค้าตัวอย่าง":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(RecRun)+1,1))),3) as RecRun FROM YarnReceive WHERE RecNo LIKE 'YS%' and CONVERT(char(7), RecDate, 120) = '" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM", dtfinfo) + "'";
                strType = "YS";
                    break;
                case "รับคืนจาก Supplier":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(RetRun)+1,1))),3) as RecRun FROM YarnReturn WHERE RetNo LIKE 'RV%' and CONVERT(char(7), RetDate, 120) = '" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM", dtfinfo) + "'";
                    strType = "RV";
                    break;
            }
            db.ConnectionOpen();
            strNo = db.ExecuteFirstValue(strSQL);
            string strRecRun =  strType + ((DateTime)dtpDate.EditValue).ToString("MM", dtfinfo) + "-" + strNo + "/" + ((DateTime)dtpDate.EditValue).ToString("yy",dtfinfo);
            db.ConnectionClose();
            return strRecRun;
        }
        private void GetReceiveDetail(string strRecNo,string strType)
        {
            string strSQL = "";
            DataTable dt = null;
            switch (strType)
            {
                case "รับจากซื้อภายในประเทศ":
                case "รับจากซื้อต่างประเทศ":
                case "รับจากการจ้างย้อม":
                case "รับสินค้าตัวอย่าง":
                case "รับจากการจ้างกรอ":
                    strSQL = "SELECT A.RECFROM,A.RECBY,A.PONO,A.DELNO,A.INVNO,A.RECDATE,A.CANCEL FROM YARNRECEIVE A WHERE A.RECNO = '" + strRecNo + "'";
                    dt = db.GetDataTable(strSQL);
                    foreach (DataRow dr in dt.Rows)
                    {
                        dtpDate.EditValue = dr["RECDATE"];
                        slePO.EditValue = dr["PONO"];
                        cboSupplier.EditValue = dr["RECFROM"];
                        txtInv.Text = dr["INVNO"].ToString();
                        txtDelNo.Text = dr["DELNO"].ToString();
                        txtURec.Text = dr["RECBY"].ToString();
                        chkCancel.EditValue = dr["CANCEL"];
                    }
                    strSQL = "SELECT CONVERT(BIT,0) AS DEL,A.YARNID,A.CODE,A.YARNSERIAL,B.CTNNO,A.NETWEIGHT,A.UNITPRICE,A.LOTNO "+
                        "FROM YarnReceiveDetail A LEFT OUTER JOIN YARNGENBARCODE B " +
                        "ON A.YARNSERIAL=B.SERIAL "+
                        "WHERE A.RecNo = '" + sleRecNo.EditValue.ToString() + "' ORDER BY A.ID";
                    break;
                case "รับคืนจากโรงทอ":
                case "รับคืนจากจ้างทอ":
                case "รับคืนจากจ้างย้อม":
                case "รับคืนจากทำตัวอย่าง":
                case "รับคืนจากจ้างกรอ":
                case "รับคืนจาก Supplier":
                    strSQL = "SELECT RETFROM,RETUSE,PoNo_LotNo as PONO,DELNO,RETDATE " +
                        "FROM YarnReturn WHERE RetNo = '" + strRecNo + "'";
                    dt = db.GetDataTable(strSQL);
                    foreach (DataRow dr in dt.Rows)
                    {
                        dtpDate.EditValue = dr["RETDATE"];
                        txtLot.Text = dr["PONO"].ToString();
                        txtDelNo.Text = dr["DELNO"].ToString();
                        cboUDel.Text = dr["RETFROM"].ToString();
                        txtURec.Text = dr["RETUSE"].ToString();
                    }
                    strSQL = "SELECT CONVERT(BIT,0) AS DEL,A.YARNID,A.CODE,A.YARNSERIAL,B.CTNNO,A.NETWEIGHT,A.UNITPRICE,A.LOTNO "+
                        "FROM YarnReturnDetail A LEFT OUTER JOIN YARNGENBARCODE B " +
                        "ON A.YARNSERIAL=B.SERIAL "+ 
                        "WHERE A.RetNo = '" + sleRecNo.EditValue.ToString() + "' ORDER BY A.ID ";
                    break;

            }
            dt = db.GetDataTable(strSQL);
            decimal totalCtn = 0;
            decimal totalKgs = 0;
            decimal totalLbs = 0;
            foreach (DataRow dr in dt.Rows) 
            {
                totalCtn += 1;
                totalKgs += (decimal)dr["NETWEIGHT"];
                totalLbs += (decimal)dr["NETWEIGHT"] * lbs;
            }
            txtTCtn.Text = totalCtn.ToString();
            txtTKgs.Text = String.Format("{0:0,0.00}",totalKgs);
            txtTLbs.Text = String.Format("{0:0,0.00}",totalLbs);
            gridControl3.DataSource = dt;
            gridView3.PopulateColumns();
            gridView3.Columns["DEL"].Caption = "Delete";
            gridView3.Columns["YARNID"].Caption = "ID";
            gridView3.Columns["CODE"].Caption = "Code";
            gridView3.Columns["CTNNO"].Caption = "Ctn No.";
            gridView3.Columns["YARNSERIAL"].Caption = "Serial";
            gridView3.Columns["NETWEIGHT"].Caption = "Weight";
            gridView3.Columns["UNITPRICE"].Caption="Unit Price";
            gridView3.Columns["LOTNO"].Caption = "Lot No.";

            gridView3.Columns["YARNID"].OptionsColumn.AllowEdit = false;
            gridView3.Columns["CODE"].OptionsColumn.AllowEdit = false;
            gridView3.Columns["CTNNO"].OptionsColumn.AllowEdit = false;
            gridView3.Columns["YARNSERIAL"].OptionsColumn.AllowEdit = false;
            //gridView3.Columns["NETWEIGHT"].OptionsColumn.AllowEdit = false;
            gridView3.Columns["UNITPRICE"].OptionsColumn.AllowEdit = false;
            gridView3.Columns["LOTNO"].OptionsColumn.AllowEdit = false;

            gridView3.OptionsView.EnableAppearanceEvenRow = true;
            gridView3.OptionsView.EnableAppearanceOddRow = true;
            gridView3.OptionsView.ColumnAutoWidth = false;
            gridView3.BestFitColumns();
            //gridView3.OptionsBehavior.Editable = false;

        }
        private DataTable GetPO(string strType)
        {
            string strSQL = "SELECT A.PONO,SUM(B.QTY)AS QTY," +
                "(SELECT ISNULL(SUM(C.SCHEDULEQTY),0)FROM PO_RECEIVEDETAIL C INNER JOIN PO_RECEIVE D ON C.RECEIVENO=D.RECEIVENO " +
                "WHERE C.PONO=A.PONO AND D.CANCEL=0)AS ACTUAL,A.SAMPLE,B.PRODUCTCODE AS YARN_ID,B.PRODUCTNAME AS CODE,C.REMARK,B.BASEID " +
                "FROM PO_PURCHASE A INNER JOIN PO_PURCHASEDETAIL B ON A.PONO=B.PONO " +
                "LEFT OUTER JOIN PO_REMARK C ON A.PONO=C.PONO AND C.LINE=1 "+
                "WHERE YEAR(A.PODATE) IN ('" + DateTime.Today.Year + "','" + DateTime.Today.AddYears(-1).Year + "') ";
            switch(strType)
            {
                case "รับจากซื้อภายในประเทศ":
                    strSQL += "AND A.PONO LIKE 'FX%' AND A.POTYPE='Local' AND B.BASEID IS NULL ";
                    break;
                case "รับจากซื้อต่างประเทศ":
                    strSQL += "AND A.PONO LIKE 'FX%' AND A.POTYPE='Import' AND B.BASEID IS NULL ";
                    break;
                case "รับจากการจ้างย้อม":
                    strSQL+="AND A.PONO LIKE 'FX%' AND B.BASEID IS NOT NULL ";
                    break;
                default:
                    strSQL+="AND (A.PONO LIKE 'FX%' OR A.PONO LIKE 'FB.S%') ";
                    break;
            }
            strSQL += "GROUP BY A.PONO,A.SAMPLE,B.PRODUCTCODE,B.PRODUCTNAME,B.BASEID,C.REMARK";
            DataTable dt = db.GetDataTable(strSQL);
            DataColumn dc = new DataColumn();
            dc.ColumnName = "BALANCE";
            dc.DataType = typeof(System.Decimal);
            dc.Expression = "QTY-ACTUAL";
            dt.Columns.Add(dc);
            dt.Columns["BALANCE"].SetOrdinal(1);
            return dt;
        }
        private void GetPODetail(string strPO)
        {
            string strSQL = "SELECT A.IDSUP AS SUPPLIER_ID,CONVERT(CHAR(8),A.PODATE,3) AS PoDate,A.CURRENCYUNIT,A.MONEYRATE,"+
                "B.NAME AS DEPARTMENT_ID,A.SAMPLE,C.CURCODE,A.VAT "+
                "FROM PO_PURCHASE A LEFT OUTER JOIN PO_DEPARTMENT B ON A.IDDEPT=B.IDDEPT "+
                "LEFT OUTER JOIN PO_CURRENCY C ON A.CURRENCYUNIT=C.IDCUR "+
                "WHERE A.PONO='"+ strPO + "'";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                optOrder.SelectedIndex = Convert.ToInt16(dr["SAMPLE"]);
                GetSupplierDetail(dr["SUPPLIER_ID"].ToString());
                txtSection.Text = dr["DEPARTMENT_ID"].ToString();
                txtCurrency.Text = dr["CURCODE"].ToString();
                txtCurrency.Tag = dr["CURRENCYUNIT"];
                txtRate.Text = dr["MONEYRATE"].ToString();
                txtVat.Text = dr["VAT"].ToString();
            }

            strSQL = "SELECT A.PONO,A.PRODUCTCODE AS CODE,A.PRODUCTNAME AS NAME,A.DESCRIPTION,A.QTY," +
                "(	SELECT ISNULL(SUM(C.SCHEDULEQTY),0) " +
                    "FROM PO_RECEIVEDETAIL C " +
                        "INNER JOIN PO_RECEIVE D ON C.RECEIVENO=D.RECEIVENO " +
                    "WHERE C.PONO=A.PONO AND C.LINE=A.LINE AND D.CANCEL=0 "+
                ")AS ACTUAL," +
                "(  SELECT ISNULL(SUM(E.SCHEDULEQTY),0) "+
                    "FROM PO_RECEIVEDETAIL E "+
                        "INNER JOIN PO_RECEIVE F ON E.RECEIVENO=F.RECEIVENO "+
                    "WHERE E.RECEIVENO='"+sleRecNo.Text+"' AND F.CANCEL=0 "+
                ") AS SCHEDULE,"+
                "(  SELECT ISNULL(SUM(G.QTY),0) " +
                    "FROM PO_RECEIVEDETAIL G " +
                        "INNER JOIN PO_RECEIVE H ON G.RECEIVENO=H.RECEIVENO " +
                    "WHERE G.RECEIVENO='" +sleRecNo.Text + "' AND H.CANCEL=0 " +
                ") AS INVENTORY," +
                "B.UNIT,A.UNITPRICE AS PRICE,CONVERT(DECIMAL(19,2),A.QTY*A.UNITPRICE) AS AMOUNT " +
            "FROM PO_PURCHASEDETAIL A " +
                "LEFT OUTER JOIN PO_UNIT B ON  A.IDUNIT=B.IDUNIT " +
            "WHERE A.PONO='" + strPO + "' " +
            "GROUP BY A.PONO,A.LINE,A.PRODUCTCODE,A.PRODUCTNAME,A.DESCRIPTION,A.QTY,B.UNIT,A.UNITPRICE " +
            "ORDER BY A.LINE;";
            strSQL += "SELECT	A.PONO,B.RECEIVENO,CONVERT(CHAR(10),B.RECEIVEDATE,103) AS RECEIVEDATE,B.DELIVERYNO," +
                "CONVERT(CHAR(10),B.DELIVERYDATE,103) AS DELIVERYDATE,B.CANCEL,SUM(C.SCHEDULEQTY) AS SCHEDULE,"+
                "SUM(C.QTY) AS INVENTORY,SUM(C.QTY*C.UNITPRICE) AS AMOUNT " +
            "FROM PO_PURCHASE A INNER JOIN PO_RECEIVE B ON A.PONO=B.PONO " +
                "LEFT OUTER JOIN PO_RECEIVEDETAIL C ON B.RECEIVENO=C.RECEIVENO " +
            "WHERE A.PONO = '" + strPO + "' GROUP BY A.PONO,B.RECEIVENO,B.RECEIVEDATE,B.DELIVERYNO,B.DELIVERYDATE,B.CANCEL;";
            strSQL+="SELECT	A.PONO,B.RECEIVENO,	B.PRODUCTCODE AS CODE,B.PRODUCTNAME AS NAME,B.DESCRIPTION," +
                "B.SCHEDULEQTY AS SCHEDULE,B.QTY AS INVENTORY,C.UNIT,B.UNITPRICE AS PRICE,CONVERT(DECIMAL(19,2),B.QTY*B.UNITPRICE) AS AMOUNT " +
            "FROM PO_PURCHASE A	INNER JOIN PO_RECEIVEDETAIL B ON A.PONO=B.PONO " +
                "LEFT OUTER JOIN PO_UNIT C ON B.IDUNIT=C.IDUNIT " +
            "WHERE A.PONO = '" + strPO + "' ORDER BY B.RECEIVENO,B.LINE";
            DataSet ds = db.GetDataSet(strSQL);
            ds.Tables[0].BeginInit();
            DataColumn dc = new DataColumn();
            dc.ColumnName = "PAST";
            dc.DataType = typeof(decimal);
            dc.Expression = "(QTY-ACTUAL)+SCHEDULE";
            ds.Tables[0].Columns.Add(dc);
            ds.Tables[0].Columns["PAST"].SetOrdinal(6);
            dc=new DataColumn();
            dc.ColumnName = "BALANCE";
            dc.DataType = typeof(decimal);
            ds.Tables[0].Columns.Add(dc);
            ds.Tables[0].Columns["BALANCE"].SetOrdinal(7);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dr["BALANCE"] = (decimal)dr["QTY"] - (decimal)dr["ACTUAL"];
            }
            ds.Tables[0].EndInit();

            DataColumn keyColumn = ds.Tables[0].Columns["PONO"];
            DataColumn foreignKeyColumn = ds.Tables[1].Columns["PONO"];
            ds.Relations.Add("Receive", keyColumn, foreignKeyColumn);
            ds.Relations.Add("Receive Detail",
                new DataColumn[] { ds.Tables[1].Columns["PONO"],ds.Tables[1].Columns["RECEIVENO"]},
                new DataColumn[] { ds.Tables[2].Columns["PONO"],ds.Tables[2].Columns["RECEIVENO"]});
            gridControl1.DataSource = ds.Tables[0];

            gridView1.PopulateColumns();
            gridView1.Columns["PONO"].Visible = false;
            gridView1.Columns["ACTUAL"].Visible = false;
            gridView1.Columns["PAST"].Visible = false;
            gridView1.Columns["CODE"].Caption = "Code";
            gridView1.Columns["NAME"].Caption = "Name";
            gridView1.Columns["DESCRIPTION"].Caption = "Description";
            gridView1.Columns["PAST"].Caption = "Past";
            gridView1.Columns["BALANCE"].Caption = "Balance";
            gridView1.Columns["SCHEDULE"].Caption = "Schedule";
            gridView1.Columns["INVENTORY"].Caption = "Inventory";
            gridView1.Columns["UNIT"].Caption = "Unit";
            gridView1.Columns["PRICE"].Caption = "Price";
            gridView1.Columns["AMOUNT"].Caption = "Amount";
            
            gridView1.Columns["CODE"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["NAME"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["QTY"].OptionsColumn.AllowEdit=false;
            gridView1.Columns["PAST"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["BALANCE"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["SCHEDULE"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["UNIT"].OptionsColumn.AllowEdit = false;
            //gridView1.Columns["PRICE"].OptionsColumn.AllowEdit = false;
            //gridView1.Columns["AMOUNT"].OptionsColumn.AllowEdit = false;

            gridView1.Columns["DESCRIPTION"].ColumnEdit = gridControl1.RepositoryItems.Add("MemoExEdit") as DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit;
            gridView1.Columns["QTY"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["QTY"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["PAST"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["PAST"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["BALANCE"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["BALANCE"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["SCHEDULE"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["SCHEDULE"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["INVENTORY"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["INVENTORY"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["AMOUNT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["AMOUNT"].DisplayFormat.FormatString = "n2";
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                txtUnit.Text=gridView1.GetRowCellDisplayText(i,"UNIT");
            }
            //optWeightType.SelectedIndex = (txtUnit.Text == "LBS.") ? 1 : 0;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
        }
        private void GetSupplier()
        {
            string strSQL = "SELECT DISTINCT Pur_Supplier as SUPPLIER FROM YarnGenBarcode ORDER BY Pur_Supplier";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                cboSupplier.Properties.Items.Add(dr["SUPPLIER"]);
            }
            //sleSupplier.Properties.DataSource = dt;
            //sleSupplier.Properties.DisplayMember = "SUPPLIER";
            //sleSupplier.Properties.ValueMember = "SUPPLIER";
        }
        private void GetSupplierDetail(string strSupplierID)
        {
            string strSQL = "SELECT NAME,ADDRESS1,ADDRESS2,'TEL. '+telephone+'  FAX. '+fax as ADDRESS3 FROM PO_SUPPLIER " +
                "WHERE IDSUP='" + strSupplierID + "'";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                txtSupplier1.Text = dr["NAME"].ToString();
                txtSupplier1.Tag = strSupplierID;
                txtSupplier.Text = dr["NAME"].ToString();
                cboSupplier.Text = dr["NAME"].ToString();
                txtSupplier2.Text = dr["ADDRESS1"].ToString();
                txtSupplier3.Text = dr["ADDRESS2"].ToString();
                txtSupplier4.Text = dr["ADDRESS3"].ToString();
            }
        }
        private void GetYarn()
        {
            string strSQL = "Select SUPPLIER,ID,CODE,MIXED,TYPE,YARNNO,COLOR,SPECIAL,DESCR" +
                ",(Select Top 1 section From yarngenbarcode Where  yarnid=YarnCode.id and id=(select max(id)from yarngenbarcode where yarnid=YarnCode.id)  )as SECTION "+
                "From YarnCode WHERE SYSDELETE=0 ORDER BY SUPPLIER,CODE";
            DataTable dt=db.GetDataTable(strSQL);
            gridControl2.DataSource=dt;
            gridView2.PopulateColumns();
            gridView2.Columns["COLOR"].Visible=false;
            gridView2.Columns["SPECIAL"].Visible=false;
            gridView2.Columns["DESCR"].Visible = false;
            gridView2.Columns["CODE"].OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            gridView2.Columns["MIXED"].OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            gridView2.OptionsView.ShowAutoFilterRow = true;
            gridView2.OptionsView.EnableAppearanceEvenRow=true;
            gridView2.OptionsView.EnableAppearanceOddRow=true;
            gridView2.OptionsView.ColumnAutoWidth=false;
            gridView2.BestFitColumns();
            gridView2.OptionsBehavior.Editable = false;
        }
        private DataTable GetYarnDetail(int yarnid)
        {
            string strSQL = "SELECT ID,CODE,SUPPLIER,TYPE,YARNNO,MIXED,COLOR,SPECIAL,DESCR" +
                    ",(Select Top 1 section From yarngenbarcode WHERE yarnid=YarnCode.id and id=(select max(id)from yarngenbarcode where yarnid=YarnCode.id)  )as SECTION " +
                    "FROM YarnCode WHERE ID="+yarnid;
            DataTable dt = db.GetDataTable(strSQL);
            return dt;
        }
        private string GenBarcode(int intID)
        {
            string strYarnID=intID.ToString().PadLeft(4,'0');
            string strSerial;
            string ser1;
            string ser2;
            string ser3;
            string ser4;
            string nSer1;
            string nSer2;
            string nSer3;
            string nSer4;
            string strSQL = "SELECT SERIAL,CTNNO FROM YARNGENBARCODE WHERE YARNID="+intID+
                " AND ID=(SELECT MAX(ID) FROM YARNGENBARCODE WHERE YARNID="+intID+")";
            DataTable dt=db.GetDataTable(strSQL);
            if(dt==null ||dt.Rows.Count==0)
            {
                txtCtn.Text = "1";
                return strYarnID + "AAAA";
            }
            else
            {
                txtCtn.Text = (Convert.ToInt32(dt.Rows[0]["CTNNO"]) + 1).ToString();
                strSerial=dt.Rows[0]["SERIAL"].ToString();
                ser1 = strSerial.Substring(4,1);
                ser2 = strSerial.Substring(5,1);
                ser3 = strSerial.Substring(6,1);
                ser4 = strSerial.Substring(7, 1);

                if(ser4!="Z")
                {
                    nSer4 = Encoding.ASCII.GetString(new byte[] { Convert.ToByte(Convert.ToChar(ser4) + 1) });
                    nSer3 = ser3;
                    nSer2 = ser2;
                    nSer1 = ser1;
                }
                else
                {
                    nSer4 = "A";
                    if(ser3!="Z")
                    {
                        nSer3 = Encoding.ASCII.GetString(new byte[] { Convert.ToByte(Convert.ToChar(ser3) + 1) });
                        nSer2 = ser2;
                        nSer1 = ser1;
                    }
                    else
                    {
                        nSer3 = "A";
                        if(ser2!="Z")
                        {
                            nSer2 = Encoding.ASCII.GetString(new byte[] { Convert.ToByte(Convert.ToChar(ser2) + 1) });
                            nSer1 = ser1;
                        }
                        else
                        {
                            nSer2 = "A";
                            if(ser1!="Z")
                                nSer1 = Encoding.ASCII.GetString(new byte[] { Convert.ToByte(Convert.ToChar(ser1) + 1) });
                            else
                                nSer1 = "A";
                        }
                    }
                }
                return (strYarnID + nSer1 + nSer2 + nSer3 + nSer4);
            }

                

        }
        private string Z4Mprint(bool batchFile)
        {
            //string s = "^XA^LH30,30\n^FO20,10^ADN,90,50^AD^FDHello World^FS\n^XZ";
            decimal decGrossWeight = Convert.ToDecimal(txtWeight.Text);
            decimal decErase = (txtErase.Text.Length > 0) ? Convert.ToDecimal(txtErase.Text) : 0;
            decimal decNetWeight = decGrossWeight - decErase;
            if (optWeightType.SelectedIndex == 1)
            {
                decGrossWeight = decGrossWeight / lbs;
                decNetWeight = decNetWeight / lbs;
            }

            string s = "^XA^PRA^FS";
            s+="^FO50,60^BY3,,60^BCN,,Y,Y^FD"+ txtSerial.Text+ "^FS";                                 //||||||||||||||||||||||||||||||||
            s += "^FO480,60^A0,45^FD" + txtSerial.Text + "^FS";
            s += "^FO50,150^A0,45^FD" + txtCode.Text + "^FS";                                         //AC/ACL1/80XEKS175G

            s+="^FO50,210^A0,25^FD" + txtMixed.Text + "^FS";                                          //Acrylic 70%, Acrylate 30% (Acrylic/Rayon)

            s += "^FO50,260^A0,25^FDColor : ^FS";
            s+= "^FO170,250^A0,35^FD" + txtColor.Text + "^FS";                                        //Color : CMT-0020 NAVY (RT3752)
            //s+= "^FO600,200^A0,30^FD" + chkRemain.Checked?"Remain":"" + "^FS";                                          //เอาออกเพื่อเพิ่ม description yarn
            if(chkBOI.Checked)  s+= "^FO600,250^A0,40^FD" + "BOI" + "^FS";                                          //เปลี่ยนเพื่อเพิ่ม description yarn                                                                                                                'BOI

            s += "^FO50,306^A0,25^FDSpecial : ^FS";
            s+= "^FO170,300^A0,35^FD" + txtSpecial.Text + "^FS";                                      //Special :          Z-TWIST
            if(optOrder.SelectedIndex==1) s+= "^FO600,200^A0,40^FD" + "Sample" + "^FS";                                       //เปลี่ยเพิ่ม description yarn                                                                                                              'SAMPLE

            s+= "^FO50,350^A0,25^FDType : ^FS";                                                       //Type :               ACRYLIC/ACRYLATE
            s+= "^FO170,350^A0,25^FD" + txtType.Text + "^FS";
            s+= "^FO480,350^A0,25^FDID : ^FS";                                                        //ID :
            s+= "^FO600,350^A0,25^FD" + txtID.Text + "^FS";

            s+= "^FO480,320 ^A0,25^FDDescr : ^FS";                                                    //เพิ่ม description yarn
            s+= "^FO600,320^A0,25^FD" + txtDescription.Text + "^FS";                                  //เพิ่ม description yarn                                                                                                          'SAMPLE

            s+= "^FO50,400^A0,25^FDSup. : ^FS";
            s+= "^FO170,400^A0,30^FD" + txtSupplier.Text + "^FS";                                     //Sup :                ITOCHU
            s+= "^FO480,400 ^A0,25^FDDate : ^FS";
            s+= "^FO600,400 ^A0,25^FD" + ((DateTime)dtpDate.EditValue).ToString("dd/MM/yyyy",dtfinfo) + "^FS";
            
            s+= "^FO50,450^A0,25^FDProducer : ^FS";
            if(cboSupplier.Text.Length==0)
                s+="^FO170,450^A0,30^FD^FS";
            else
                s += (cboSupplier.Text.Length > 19) ? "^FO170,450^A0,30^FD" + cboSupplier.Text.Substring(0, 19) + "^FS" : "^FO170,450^A0,30^FD" + cboSupplier.Text.Substring(0,cboSupplier.Text.Length) + "^FS";                                     //Producer  :    ITOCHU                                        Section :      PARFUN
            s+= "^FO480,450^A0,25^FDSection : ^FS";
            s+= "^FO600,450^A0,25^FD" + txtSection.Text + "^FS";

            s+= "^FO50,500^A0,25^FDP/O No.: ^FS";
            s+= (slePO.EditValue==null)?"^FO170,500^A0,25^FD^FS":"^FO170,500^A0,25^FD"+slePO.EditValue.ToString() + "^FS";                                           //P/O No  :       FB. 1234/04                                  Ctn No. :      7
            s+= "^FO480,505^A0,25^FDCtn No.: ^FS";
            s+= "^FO600,500^A0,35^FD" + txtCtn.Text + "^FS";

            s+= "^FO50,550^A0,25^FDLot No.: ^FS";
            s+= "^FO170,550^A0,25^FD" + txtLot.Text + "^FS";                                         //Lot No.  :        3456/04                                          Weight :      23.5
            s+= "^FO480,560^A0,25^FDWeight : ^FS";
            if(optKgs.SelectedIndex==0)
                s+= "^FO580,550^A0,50^FD" + string.Format("{0:0,0.00}",decNetWeight) + "^FS";
            else
            {
                s+= "^FO300,560^A0,30^FDLbs. = " + string.Format("{0:0,0.00}",decNetWeight*lbs) + "^FS";
                s+= "^FO580,550^A0,50^FD" + string.Format("{0:0,0.00}",decNetWeight) + "^FS";
            }
            if(chkGW.Checked) s += "^FO300,560^A0,30^FDGW. = " + string.Format("{0:0,0.00}", decGrossWeight) + "^FS";
            s+= "^PQ1";
            s+= "^XZ";
            // Send a printer-specific to the printer.
            //System.Drawing.Printing.PrinterSettings settings = new System.Drawing.Printing.PrinterSettings();
            if (!batchFile) RawPrinterHelper.SendStringToPrinter(barcodePrinter/*settings.PrinterName*/, s, txtCtn.Text);
            return s;
        }
        private void SaveBarcode()
        {
            if(txtID.Text.Trim().Length==0) return;
            if(txtSerial.Text.Trim().Length==0) return;
            if(txtWeight.Text.Trim().Length==0) return;

            decimal decGrossWeight = Convert.ToDecimal(txtWeight.Text);
            decimal decErase = (txtErase.Text.Length > 0) ? Convert.ToDecimal(txtErase.Text) : 0;
            decimal decNetWeight = decGrossWeight - decErase;
            if (optWeightType.SelectedIndex == 1)
            {
                decGrossWeight = decGrossWeight / lbs;
                decNetWeight = decNetWeight / lbs;
            }
        
            string strSQL = "SELECT COUNT(SERIAL) FROM YARNGENBARCODE WHERE SERIAL = '" + txtSerial.Text + "'";
            if(Convert.ToInt32(db.ExecuteFirstValue(strSQL))==0)
            {
                strSQL = "INSERT INTO YarnGenBarcode (YarnID,Serial,YarnCode,Date,Pur_Supplier,Section,Order_Sample,LotNo," +
                    "CtnNo,NetWeight,Unit,BOI,PoNo,InDate2,DocNo) VALUES ('" + txtID.Text + "','" + txtSerial.Text + "'," +
                    "'" + txtCode.Text + "','" + DateTime.Today.ToString("dd/MM/yyyy", dtfinfo) + "','" + txtSupplier.Text + "'," +
                    "'" + txtSection.Text + "','" + optOrder.SelectedIndex + "','" + txtLot.Text + "','" + txtCtn.Text + "'";
                strSQL+=","+decNetWeight; 
                strSQL+=",'" + txtUnit.Text + "',";
                strSQL += (chkBOI.Checked) ? "1" : "0";
                strSQL+=(slePO.EditValue==null)?",''":",'" + slePO.EditValue.ToString()+ "'";
                strSQL+=",'" +((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd",dtfinfo) + "'";
                strSQL += ",'" + sleRecNo.Text + "'";
                //if (receiveType == "Receive")
                //    strSQL +="," + price;
                //else
                //    strSQL +=  "," + GetAveragePrice(Convert.ToInt32(txtID.Text));
                strSQL+=")";
                db.Execute(strSQL);
            }
        }
        private void SumTotal()
        {
            txtTCtn.Text = gridView3.DataRowCount.ToString();
            decimal totalKgs = 0;
            for (int i = 0; i < gridView3.DataRowCount; i++)
            {
                totalKgs += Convert.ToDecimal(gridView3.GetRowCellValue(i, "NETWEIGHT"));
            }
            txtTKgs.Text = Math.Round(totalKgs, 2).ToString("0,0.00",clinfo);// string.Format("{0:0,0.00}", totalKgs);
            txtTLbs.Text = Math.Round(totalKgs * lbs, 2).ToString("0,0.00",clinfo);// string.Format("{0:0,0.00}", (totalKgs * lbs));
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                decimal decTemp=0;
                if (gridView1.GetRowCellDisplayText(i, "UNIT") == "LBS.")
                    decTemp =Math.Round(totalKgs * lbs,2);
                else
                    decTemp=Math.Round(totalKgs,2);
                if (decTemp > (decimal)gridView1.GetRowCellValue(0, "PAST"))
                {
                    gridView1.SetRowCellValue(i, "BALANCE",0);
                    gridView1.SetRowCellValue(i, "SCHEDULE", gridView1.GetRowCellValue(0, "PAST"));
                }
                else
                {
                    gridView1.SetRowCellValue(i, "BALANCE", (decimal)gridView1.GetRowCellValue(0, "PAST") - decTemp);
                    gridView1.SetRowCellValue(i, "SCHEDULE", decTemp);
                }
                
                gridView1.SetRowCellValue(i, "INVENTORY", decTemp);
            }
        }
        private string GetEmployee(string strID)
        {
            string strSQL = "SELECT Name FROM BarcodeText WHERE Code = '" + strID + "'";
            return db.ExecuteFirstValue(strSQL);
        }
        private void LoadRegistry()
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\TUW\TUW System");
                if (regKey != null)
                {
                    object keyValue;
                    keyValue = regKey.GetValue("YS_Receive - Barcode Printer");
                    if (keyValue != null)
                        barcodePrinter = regKey.GetValue("YS_Receive - Barcode Printer").ToString();
                    else
                        MessageBox.Show("Not found barcode printer.", "No Printer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    keyValue = regKey.GetValue("YS_Receive - Print Copy");
                    if (keyValue != null)
                        printCopy = Convert.ToInt16(regKey.GetValue("YS_Receive - Print Copy"));
                    else
                    {
                        printCopy = 3;
                        SaveRegistry("YS_Receive - Print Copy",printCopy);
                    }

                    regKey.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load registry error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveRegistry(string key, object value)
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\TUW\TUW System", true);
                if (regKey == null)
                {
                    regKey = Registry.CurrentUser.CreateSubKey(@"Software\TUW\TUW System");
                }
                regKey.SetValue(key, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save to registry error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SetAveragePrice(string monthYear,int yarnID)//monthyear=yyyyMMdd,คำนวน cost เฉลี่ยแล้วเอาไปใส่ในตาราง yarncode
        {
            try
            {
                string nextMonth, cost;
                nextMonth = (new DateTime(Convert.ToInt32(monthYear.Substring(0, 4)), Convert.ToInt32(monthYear.Substring(4, 2)), 1)).AddMonths(1).ToString("yyyyMM", dtfinfo);

                    string strSQL = "exec sptuwsystem_ys_costaverage '" + monthYear + "','" + yarnID + "'";
                    cost = db.ExecuteFirstValue(strSQL);
                    if (cost.Length > 0 && Convert.ToDecimal(cost) > 0)//ต้องไม่เป็นค่า null หรือ 0
                    {
                        if (monthYear == DateTime.Today.ToString("yyyyMM", dtfinfo))//ถ้าอยู่ในเดือนนี้จะ update เฉพาะ cost ใน yarncode
                        {
                            strSQL = "update yarncode set cost=" + cost + " where id=" + yarnID;
                            db.Execute(strSQL);
                        }
                        else//ถ้าย้อนหลังต้อง update yarnstockbegin ของเดือนถัดมา
                        {
                            strSQL = "select count(yarnid) from yarnstockbegin where monthyear='" + nextMonth + "' and yarnid="+yarnID;
                            if (Convert.ToInt32(db.ExecuteFirstValue(strSQL)) > 0)
                            {
                                strSQL = "update yarnstockbegin set cost=" + cost + " where monthyear='" + nextMonth + "' and yarnid=" + yarnID;
                                db.Execute(strSQL);
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private decimal GetAveragePrice(int yarnID)
        {
            string strSQL = "SELECT COST FROM YARNCODE WHERE ID=" + yarnID;
            return Convert.ToDecimal(db.ExecuteFirstValue(strSQL));
        }
        private void LoadfrmYS_ReceiveForm(string strDocNo)
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.ParentForm.MdiChildren)
            {
                if (frmActive.Name == "frmYS_ReceiveForm") frmActive.Dispose();
            }
            frmYS_ReceiveForm frm = new frmYS_ReceiveForm();
            frm.ReceiveNO = strDocNo;
            frm.User_Login = User_Login;
            frm.ConnectionString = _connectionString;
            frm.MdiParent = this.ParentForm;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }
        private void ManualInputWeight(decimal totalWeight,string unit)
        {
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                if (gridView1.GetRowCellDisplayText(i, "UNIT") == "LBS.")
                {
                    decimal decTemp =(unit=="LBS")?totalWeight:totalWeight * lbs;
                    if (decTemp > (decimal)gridView1.GetRowCellValue(0, "BALANCE"))
                        gridView1.SetRowCellValue(i, "SCHEDULE", gridView1.GetRowCellValue(0, "BALANCE"));
                    else
                        gridView1.SetRowCellValue(i, "SCHEDULE", decTemp);
                    gridView1.SetRowCellValue(i, "INVENTORY", decTemp);
                }
                else
                {
                    decimal decTemp = (unit == "KGS") ? totalWeight : totalWeight / lbs;
                    if (decTemp > (decimal)gridView1.GetRowCellValue(0, "BALANCE"))
                        gridView1.SetRowCellValue(i, "SCHEDULE", gridView1.GetRowCellValue(0, "BALANCE"));
                    else
                        gridView1.SetRowCellValue(i, "SCHEDULE", decTemp);
                    gridView1.SetRowCellValue(i, "INVENTORY", decTemp);
                }
            }
        }

        private void frmYS_Receive_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            LoadRegistry();
            NewData();
            GetSupplier();
        }
        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                sleRecNo.EditValueChanged -= sleRecNo_EditValueChanged;
                slePO.EditValueChanged -= slePO_EditValueChanged;

                sleRecNo.EditValue = null;
                sleRecNo.Properties.DataSource = null;
                slePO.EditValue = null;
                slePO.Properties.DataSource = null;
                ClearData();
                ClearYarnDetail();
                sleRecNo.Properties.DataSource = GetReceiveNo(cboType.Text, (DateTime)dtpDate.EditValue);
                sleRecNo.Properties.DisplayMember = "RECNO";
                sleRecNo.Properties.ValueMember = "RECNO";
                string strReceiveNoNew = RunReceiveNoNew(cboType.Text);
                dtRecNo.BeginInit();
                DataRow dr = dtRecNo.NewRow();
                dr["RECNO"] = strReceiveNoNew;
                dtRecNo.Rows.Add(dr);
                dtRecNo.EndInit();
                sleRecNo.EditValue = strReceiveNoNew;
                switch (cboType.Text)
                {
                    case "รับจากซื้อภายในประเทศ":
                    case "รับจากซื้อต่างประเทศ":
                    case "รับจากการจ้างย้อม":
                    case "รับจากการจ้างกรอ":
                    case "รับสินค้าตัวอย่าง":
                        dtPO = GetPO(cboType.Text);
                        slePO.Properties.DataSource = dtPO;
                        slePO.Properties.DisplayMember = "PONO";
                        slePO.Properties.ValueMember = "PONO";
                        slePO.Properties.PopulateViewColumns();
                        slePO.Properties.View.Columns["QTY"].Visible = false;
                        slePO.Properties.View.Columns["ACTUAL"].Visible = false;
                        slePO.Properties.View.Columns["CODE"].OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                        

                        slePO.Properties.View.OptionsView.ShowAutoFilterRow = true;
                        //slePO.Properties.View.OptionsView.ColumnAutoWidth = true;
                        //slePO.Properties.View.BestFitColumns();
                        tabbedControlGroup2.SelectedTabPageIndex=0;
                        //xtraTabControl1.SelectedTabPageIndex = 0;
                        break;
                    default://Do nothing
                        GetYarn();
                        tabbedControlGroup2.SelectedTabPageIndex = 1;
                        //xtraTabControl1.SelectedTabPageIndex = 1;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            sleRecNo.EditValueChanged += sleRecNo_EditValueChanged;
            slePO.EditValueChanged += slePO_EditValueChanged;          
        }
        private void sleRecNo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ClearData();
                ClearYarnDetail();
                slePO.EditValueChanged -= slePO_EditValueChanged;
                slePO.EditValue = null;
                slePO.EditValueChanged += slePO_EditValueChanged;
                GetReceiveDetail(sleRecNo.Text, cboType.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var grid = (DevExpress.XtraGrid.Views.Grid.GridView)sender;

                price = (decimal)grid.GetRowCellValue(e.RowHandle, "PRICE");//เก็บค่า
                if(txtCurrency.Text != "THB") price= price * Convert.ToDecimal(txtRate.Text);//แล้วแปลงเป็น Baht
                if (txtUnit.Text == "LBS.") price = price * lbs;//แปลงเป็นราคาต่อกิโลกรัม

                DataTable dt= GetYarnDetail(Convert.ToInt32(grid.GetRowCellDisplayText(e.RowHandle,"CODE")));
                foreach (DataRow dr in dt.Rows)
                {
                    txtID.Text = dr["ID"].ToString();
                    txtCode.Text = dr["CODE"].ToString();
                    txtMixed.Text =dr["MIXED"].ToString();
                    txtType.Text = dr["TYPE"].ToString();
                    txtColor.Text = dr["COLOR"].ToString();
                    txtSpecial.Text = dr["SPECIAL"].ToString();
                    txtSupplier.Text = dr["SUPPLIER"].ToString();
                    txtSection.Text = dr["SECTION"].ToString();
                }
                tabbedControlGroup1.SelectedTabPageIndex = 0;
                txtWeight.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            db.ConnectionOpen();
            try
            {
                var grid = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                txtID.Text = grid.GetRowCellDisplayText(e.RowHandle, "ID");
                txtCode.Text = grid.GetRowCellDisplayText(e.RowHandle, "CODE");
                txtMixed.Text = grid.GetRowCellDisplayText(e.RowHandle, "MIXED");
                txtType.Text = grid.GetRowCellDisplayText(e.RowHandle, "TYPE");
                txtColor.Text = grid.GetRowCellDisplayText(e.RowHandle, "COLOR");
                txtSpecial.Text = grid.GetRowCellDisplayText(e.RowHandle, "SPECIAL");
                txtSupplier.Text = grid.GetRowCellDisplayText(e.RowHandle, "SUPPLIER");
                txtSection.Text = grid.GetRowCellDisplayText(e.RowHandle, "SECTION");
                var yarnid = Convert.ToInt32(txtID.Text);
                //CalculateAveragePrice(yarnid);
                price = GetAveragePrice(yarnid);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
        }
        private void gridView3_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    txtBarID.Text = gridView3.GetFocusedRowCellDisplayText("YARNID");
                    txtBarSerial.Text = gridView3.GetFocusedRowCellDisplayText("YARNSERIAL");
                    txtBCode.Text = gridView3.GetFocusedRowCellDisplayText("CODE");
                    txtBarCtn.Text = gridView3.GetFocusedRowCellDisplayText("CTNNO");
                    txtBarWeight.Text = gridView3.GetFocusedRowCellDisplayText("NETWEIGHT");
                    tabbedControlGroup1.SelectedTabPageIndex = 1;
                }
                //else if (e.Button == MouseButtons.Right)
                //{
                //    popupMenu1.ShowPopup(new Point(Cursor.Position.X, Cursor.Position.Y));
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void slePO_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBalance)
                {
                    checkBalance = false;//ค่า slePO มีการเปลี่ยนค่าที่เกิดจากการโหลดของข้อมูลที่เคยบันทึกไว้ฉะนั้นไม่ต้องเช็คหา Balance
                    var row = dtPO.AsEnumerable().Where(w => w.Field<string>("PONO") == slePO.Text);
                    decimal balance = 0;
                    string baseId = null;
                    foreach (var item in row)
                    {
                        balance = item.Field<decimal>("BALANCE");
                        baseId = item.Field<string>("BASEID");
                    }
                    if (balance <= 0M)
                        if (MessageBox.Show("P/O นี้เปิดรับของเต็มจำนวนแล้วท่านต้องการเปิดเพิ่มอีกหรือไม่", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        {
                            slePO.EditValue = null;
                            return;
                        }
                    //if (cboType.Text == "รับจากการจ้างย้อม" && String.IsNullOrEmpty(baseId))
                    //    throw new ApplicationException("P/O นี้ไม่ใช่ P/O จ้างย้อม");
                    //else if (cboType.Text != "รับจากการจ้างย้อม" && !String.IsNullOrEmpty(baseId))
                    //    throw new ApplicationException("P/O นี้เป็น P/O จ้างย้อม กรุณาเปิดใบรับของให้ถูกประเภท");
                }
                ClearYarnDetail();
                GetPODetail(slePO.Text);
            }
            catch (ApplicationException ex) 
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                slePO.EditValue = null;
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            var grid = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            grid.IndicatorWidth = 40;
        }
        private void gridView3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            var grid = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            grid.IndicatorWidth = 40;
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                sleRecNo.EditValueChanged -= sleRecNo_EditValueChanged;
                slePO.EditValueChanged -= slePO_EditValueChanged;

                sleRecNo.Properties.DataSource = GetReceiveNo(cboType.Text, (DateTime)dtpDate.EditValue);
                sleRecNo.Properties.DisplayMember = "RECNO";
                sleRecNo.Properties.ValueMember = "RECNO";
                string strReceiveNoNew = RunReceiveNoNew(cboType.Text);
                dtRecNo.BeginInit();
                DataRow dr = dtRecNo.NewRow();
                dr["RECNO"] = strReceiveNoNew;
                dtRecNo.Rows.Add(dr);
                dtRecNo.EndInit();
                sleRecNo.EditValue = strReceiveNoNew;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            sleRecNo.EditValueChanged += sleRecNo_EditValueChanged;
            slePO.EditValueChanged += slePO_EditValueChanged;
        }
        private void btnPrintTotal_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboType.Text.Trim().Length == 0) return;
                if (txtID.Text.Length == 0) throw new ApplicationException ("ไม่มี Yarn ID ให้เช็คกับทางแผนก Fabric อีกทีว่ามีการเปลี่ยนรหัสเส้นด้ายใน P/O หรือไม่");
                if (optOrder.SelectedIndex == -1) throw new ApplicationException("คุณยังไม่ระบุเส้นด้ายว่าเป็น Order หรือ Sample ?");

                db.ConnectionOpen();
                StringBuilder batchFile=new StringBuilder();
                decimal decGrossWeight = Convert.ToDecimal(txtWeight.Text);
                decimal decErase = (txtErase.Text.Length > 0) ? Convert.ToDecimal(txtErase.Text) : 0;
                decimal decNetWeight = decGrossWeight - decErase;
                if (optWeightType.SelectedIndex == 1)
                {
                    decGrossWeight = decGrossWeight / lbs;
                    decNetWeight = decNetWeight / lbs;
                }

                int totalCarton = Convert.ToInt32(txtCtn.Text);
                for (int i = 0; i < totalCarton; i++)
                {
                    txtSerial.Text = GenBarcode(Convert.ToInt32(txtID.Text));

                    gridView3.AddNewRow();

                    int newRowHandle = gridView3.FocusedRowHandle;
                    if (gridView3.IsNewItemRow(newRowHandle))
                    {
                        gridView3.SetRowCellValue(newRowHandle, "DEL", false);
                        gridView3.SetRowCellValue(newRowHandle, "YARNID", txtID.Text);
                        gridView3.SetRowCellValue(newRowHandle, "CODE", txtCode.Text);
                        gridView3.SetRowCellValue(newRowHandle, "CTNNO", txtCtn.Text);
                        gridView3.SetRowCellValue(newRowHandle, "YARNSERIAL", txtSerial.Text);
                        gridView3.SetRowCellValue(newRowHandle, "NETWEIGHT", decNetWeight);
                        gridView3.SetRowCellValue(newRowHandle, "LOTNO", txtLot.Text);
                        gridView3.SetRowCellValue(newRowHandle, "UNITPRICE", price);
                    }
                    gridView3.UpdateCurrentRow();
                    SaveBarcode();
                    SumTotal();
                    batchFile.Append(Z4Mprint(batchFile:true));
                    txtSerial.Text = "";
                }
                RawPrinterHelper.SendStringToPrinter(barcodePrinter, batchFile.ToString());
                gridView3.Columns["NETWEIGHT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gridView3.Columns["NETWEIGHT"].DisplayFormat.FormatString = "n2";
                gridView3.OptionsView.EnableAppearanceEvenRow = true;
                gridView3.OptionsView.EnableAppearanceOddRow = true;
                gridView3.OptionsView.ColumnAutoWidth = false;
                gridView3.BestFitColumns();

            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            db.ConnectionClose();
        }
        private void gridView1_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView master = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            DevExpress.XtraGrid.Views.Grid.GridView detail = (DevExpress.XtraGrid.Views.Grid.GridView)master.GetDetailView(e.RowHandle, e.RelationIndex);
            detail.Columns["PONO"].Visible = false;
            detail.OptionsView.ShowFooter = true;
            
            RepositoryItemButtonEdit rpReceive = gridControl1.RepositoryItems.Add("ButtonEdit") as RepositoryItemButtonEdit;
            //rpReceive.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            rpReceive.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;
            rpReceive.Buttons[0].Image = Resource1.magnifier;
            rpReceive.ButtonClick += this.gridViewReceive_ButtonClick;
            detail.Columns["RECEIVENO"].ColumnEdit = rpReceive;
            detail.Columns["RECEIVENO"].OptionsColumn.ReadOnly = true;
            detail.Columns["AMOUNT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            detail.Columns["AMOUNT"].DisplayFormat.FormatString = "n2";
            detail.Columns["SCHEDULE"].SummaryItem.FieldName = "SCHEDULE";
            detail.Columns["SCHEDULE"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            detail.Columns["INVENTORY"].SummaryItem.FieldName = "INVENTORY";
            detail.Columns["INVENTORY"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            detail.Columns["AMOUNT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");

            detail.Columns["CANCEL"].Visible = false;
            detail.MasterRowExpanded += this.gridViewReceive_MasterRowExpanded;
            detail.RowStyle += this.gridView_RowStyle;
            detail.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            detail.OptionsView.EnableAppearanceEvenRow = true;
            detail.OptionsView.EnableAppearanceOddRow = true;
            detail.OptionsView.ColumnAutoWidth = false;
            detail.BestFitColumns();
        }
        private void gridViewReceive_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView master = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            DevExpress.XtraGrid.Views.Grid.GridView detail = (DevExpress.XtraGrid.Views.Grid.GridView)master.GetDetailView(e.RowHandle, e.RelationIndex);
            detail.OptionsView.ShowFooter = true;
            detail.Columns["AMOUNT"].SummaryItem.FieldName = "AMOUNT";
            detail.Columns["AMOUNT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            detail.OptionsView.EnableAppearanceEvenRow = true;
            detail.OptionsView.EnableAppearanceOddRow = true;
            detail.OptionsView.ColumnAutoWidth = false;
            detail.BestFitColumns();
        }
        private void gridView_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                bool bold = (view.GetRowCellDisplayText(e.RowHandle, "RECEIVENO") == sleRecNo.Text) ? true : false;
                bool strikeOut = (view.GetRowCellDisplayText(e.RowHandle, "CANCEL") == "Checked" || view.GetRowCellDisplayText(e.RowHandle, "CANCEL") == "1") ? true : false;
                
                if (bold && strikeOut)
                {
                    Font newFont = new Font(view.Appearance.Row.Font, FontStyle.Bold|FontStyle.Strikeout);
                    e.Appearance.Font = newFont;
                }
                else if (bold)
                {
                    Font newFont = new Font(view.Appearance.Row.Font, FontStyle.Bold);
                    e.Appearance.Font = newFont;
                }
                else if (strikeOut)
                {
                    Font newFont = new Font(view.Appearance.Row.Font, FontStyle.Strikeout);
                    e.Appearance.Font = newFont;
                }
            }
        }
        private void btnPrintNew_Click(object sender, EventArgs e)
        {
            try
            {
                db.ConnectionOpen();
                db.BeginTrans();
                string strSQL = "SELECT Date,Pur_Supplier,Section,LotNo,Order_Sample,PoNo,Remaining,BOI FROM YarnGenBarcode "+
                    "WHERE Serial = '"+ gridView3.GetFocusedRowCellDisplayText("YARNSERIAL") +"'";
                DataTable dt = db.GetDataTable(strSQL);
                foreach (DataRow dr in dt.Rows)
                {
                    txtSerial.Text = txtBarSerial.Text;
                    txtCode.Text = txtBCode.Text;
                    txtWeight.Text = txtBarWeight.Text;
                }
                dt = GetYarnDetail(Convert.ToInt32(txtBarID.Text));
                foreach (DataRow dr in dt.Rows)
                { 
                    //ID,CODE,SUPPLIER,TYPE,YARNNO,MIXED,COLOR,SPECIAL,DESCR
                    txtMixed.Text = dr["MIXED"].ToString();
                    txtColor.Text = dr["COLOR"].ToString();
                    txtSpecial.Text = dr["SPECIAL"].ToString();
                    txtType.Text = dr["TYPE"].ToString();
                    txtDescription.Text = dr["DESCR"].ToString();
                    txtSupplier.Text = dr["SUPPLIER"].ToString();
                    txtSection.Text = dr["SECTION"].ToString();
                    txtCtn.Text = txtBarCtn.Text;
                    txtLot.Text = gridView3.GetFocusedRowCellDisplayText("LOTNO");
                }
                //update yarn gen barcode
                strSQL = "UPDATE YarnGenBarcode Set NetWeight = '" + txtBarWeight.Text + "',Indate2='" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd",dtfinfo) + "' WHERE Serial = '" + txtBarSerial.Text + "'";
                db.Execute(strSQL);
                //update yarn receive detail stock
                switch (cboType.Text)
                {
                    case "รับจากซื้อภายในประเทศ":
                    case "รับจากซื้อต่างประเทศ":
                    case "รับจากการจ้างย้อม":
                    case "รับจากการจ้างกรอ":
                    case "รับสินค้าตัวอย่าง":
                        strSQL = "delete from yarnreceivedetail where recno='"+sleRecNo.EditValue.ToString()+ "' and yarnserial='"+ txtBarSerial.Text+ "'";
                        db.Execute(strSQL);
                        strSQL = "INSERT INTO YarnReceiveDetail (RecNo,YarnID,YarnSerial,Code,NetWeight,UnitPrice,LotNo) VALUES ("+
                            "'"+sleRecNo.EditValue.ToString()+"','"+txtBarID.Text+"','"+txtBarSerial.Text+"','"+txtBCode.Text+"'"+
                            ",'"+ txtBarWeight.Text+ "','"+ gridView3.GetFocusedRowCellDisplayText("UNITPRICE") + "'"+
                            ",N'"+ gridView3.GetFocusedRowCellDisplayText("LOTNO")+ "')";
                        db.Execute(strSQL);
                        gridView3.SetFocusedRowCellValue("NETWEIGHT",txtBarWeight.Text);
                        break;
                    case "รับคืนจากโรงทอ":
                    case "รับคืนจากจ้างทอ":
                    case "รับคืนจากจ้างย้อม":
                    case "รับคืนจากทำตัวอย่าง":
                    case "รับคืนจากจ้างกรอ":
                    case "รับคืนจาก Supplier":
                        strSQL = "delete from yarnreturndetail where retno='"+sleRecNo.EditValue.ToString()+"' and yarnserial='"+ txtBarSerial.Text+ "'";
                        db.Execute(strSQL);
                        strSQL = "INSERT INTO YarnReturnDetail (RetNo,YarnID,YarnSerial,Code,NetWeight,UnitPrice,LotNo) VALUES ("+
                            "'"+ sleRecNo.EditValue.ToString()+"','"+txtBarID.Text+"','"+txtBarSerial.Text+"'"+
                            ",'"+ txtBCode.Text+ "','"+ txtBarWeight.Text + "','"+gridView3.GetFocusedRowCellDisplayText("UNITPRICE")+"'"+
                            ",N'"+gridView3.GetFocusedRowCellDisplayText("LOTNO")+"')";
                        db.Execute(strSQL);
                        gridView3.SetFocusedRowCellValue("NETWEIGHT",txtBarWeight.Text);
                        break;
                }
                Z4Mprint(batchFile:false);
                db.CommitTrans();
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
        }
        private void txtWeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar!=(char)13) return;
            try
            {
                if (optOrder.SelectedIndex == -1) throw new ApplicationException("คุณยังไม่ระบุเส้นด้ายว่าเป็น Order หรือ Sample ?");
                if (sleRecNo.EditValue == null) throw new ApplicationException("คุณยังไม่ระบุเลขที่รับเข้า?");
                if (txtID.Text.Trim().Length == 0) throw new ApplicationException("ไม่มี Yarn ID ให้เช็คกับแผนกผ้าว่ามีการเปลี่ยนเส้นด้ายใน P/O หรือไม่ ?");
                if (txtWeight.Text.Trim().Length == 0) throw new ApplicationException("คุณยังไม่ได้ระบุน้ำหนัก");

                db.ConnectionOpen();
                txtSerial.Text = GenBarcode(Convert.ToInt32(txtID.Text));

                decimal decGrossWeight = Convert.ToDecimal(txtWeight.Text);
                decimal decErase = (txtErase.Text.Length > 0) ? Convert.ToDecimal(txtErase.Text) : 0;
                decimal decNetWeight = decGrossWeight - decErase;
                if (optWeightType.SelectedIndex == 1)
                {
                    decGrossWeight = decGrossWeight / lbs;
                    decNetWeight = decNetWeight / lbs;
                }
                //txtWeight.Text = decGrossWeight.ToString();

                gridView3.AddNewRow();
                int newRowHandle = gridView3.FocusedRowHandle;
                gridView3.SetRowCellValue(newRowHandle, "DEL", false);
                gridView3.SetRowCellValue(newRowHandle, "YARNID", txtID.Text);
                gridView3.SetRowCellValue(newRowHandle, "CODE", txtCode.Text);
                gridView3.SetRowCellValue(newRowHandle, "CTNNO", txtCtn.Text);
                gridView3.SetRowCellValue(newRowHandle, "YARNSERIAL", txtSerial.Text);
                gridView3.SetRowCellValue(newRowHandle, "NETWEIGHT", decNetWeight);
                gridView3.SetRowCellValue(newRowHandle, "UNITPRICE", price);
                gridView3.SetRowCellValue(newRowHandle, "LOTNO", txtLot.Text);
                gridView3.UpdateCurrentRow();

                gridView3.Columns["NETWEIGHT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gridView3.Columns["NETWEIGHT"].DisplayFormat.FormatString = "n2";
                gridView3.OptionsView.EnableAppearanceEvenRow = true;
                gridView3.OptionsView.EnableAppearanceOddRow = true;
                gridView3.OptionsView.ColumnAutoWidth = false;
                gridView3.BestFitColumns();

                SaveBarcode();
                SumTotal();
                Z4Mprint(batchFile:false);

                txtWeight.Text = "";
                txtSerial.Text = "";
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            db.ConnectionClose();
        }
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "INVENTORY")
                gridView1.SetRowCellValue(e.RowHandle,"AMOUNT",(decimal)e.Value*(decimal)gridView1.GetRowCellValue(e.RowHandle,"PRICE"));
            else if (e.Column.FieldName=="PRICE")
                gridView1.SetRowCellValue(e.RowHandle, "AMOUNT", (decimal)gridView1.GetRowCellValue(e.RowHandle, "INVENTORY") * (decimal)e.Value);
        }
        private void bbiDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("คุณต้องการลบข้อมูลแถวนี้หรือไม่. Serial " + gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "YARNSERIAL"), "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                db.ConnectionOpen();
                try
                {
                    db.BeginTrans();
                    string serial=gridView3.GetRowCellDisplayText(gridView3.FocusedRowHandle, "YARNSERIAL");
                    string strSQL = "Delete From yarngenbarcode Where serial='" +serial+ "'";
                    db.Execute(strSQL);
                    strSQL = "Delete YarnReceiveDetail Where YarnSerial='"+serial+"' AND RecNo = '"+ sleRecNo.Text+ "'";
                    db.Execute(strSQL);
                    strSQL = "Delete YarnReturnDetail Where YarnSerial = '"+serial+"' AND RetNo = '"+ sleRecNo.Text+ "'";
                    db.Execute(strSQL);
                    gridView3.DeleteRow(gridView3.FocusedRowHandle);
                    db.CommitTrans();
                    SumTotal();
                }
                catch (Exception ex)
                {
                    db.RollbackTrans();
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                db.ConnectionClose();
            }
        }
        private void txtURec_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Return) return;
            try
            {
                db.ConnectionOpen();
                txtURec.Text = GetEmployee(txtURec.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();

        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("คุณต้องการที่จะลบข้อมูลเส้นด้ายนี้ใช่หรือไม่?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
            bool success = false;
            try
            {
                db.ConnectionOpen();
                db.BeginTrans();
                string serial = "";
                string strSQL = "";
                for (int i = gridView3.DataRowCount - 1; i >= 0; i--)
                {
                    if ((bool)gridView3.GetRowCellValue(i, "DEL") == false) continue;
                    serial = gridView3.GetRowCellDisplayText(i, "YARNSERIAL");
                    strSQL = "Update YarnGenBarcode Set sysDelete=1 Where Serial='" + serial + "'"; 
                    //strSQL = "Delete From yarngenbarcode Where serial='" + serial + "'";
                    db.Execute(strSQL);
                    strSQL = "Delete YarnReceiveDetail Where YarnSerial='" + serial + "' AND RecNo = '" + sleRecNo.Text + "'";
                    db.Execute(strSQL);
                    strSQL = "Delete YarnReturnDetail Where YarnSerial = '" + serial + "' AND RetNo = '" + sleRecNo.Text + "'";
                    db.Execute(strSQL);
                    strSQL = "Delete YarnIssueDetail Where Serial='" + serial + "'";
                    db.Execute(strSQL);
                    gridView3.DeleteRow(i);
                }
                SumTotal();
                db.CommitTrans();
                success = true;
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.ConnectionClose();
            }
            if (success)
            {
                //ถ้ามีการลบแถวจะยกเลิกการปิดออเดอร์แล้วปรับค่า schedule ใหม่
                if (receiveType == "Receive")
                { 
                    gridView1.SetRowCellValue(0,"THIS_SCHEDULE",0);
                    gridView1.SetRowCellValue(0, "SCHEDULE", gridView1.GetRowCellValue(0, "INVENTORY"));
                }
                SaveData();
            }
        }
        private void gridViewReceive_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = (ButtonEdit)sender;
            LoadfrmYS_ReceiveForm(editor.Text);
        }
        private void gridView1_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            try
            {
            //    DevExpress.XtraGrid.Views.Grid.GridView grid = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            //    switch(grid.FocusedColumn.FieldName)
            //    {
            //    if (grid.Columns e.Column.FieldName == "SCHEDULE")
            //{
            //    if ((decimal)e.Value > (decimal)gridView1.GetRowCellValue(e.RowHandle, "BALANCE"))
            //        gridView1.SetRowCellValue(e.RowHandle, "SCHEDULE", gridView1.GetRowCellValue(e.RowHandle, "BALANCE"));
            //}
            //else if (e.Column.FieldName == "INVENTORY")
            //    gridView1.SetRowCellValue(e.RowHandle,"AMOUNT",(decimal)e.Value*(decimal)gridView1.GetRowCellValue(e.RowHandle,"PRICE"));
            //else if (e.Column.FieldName=="PRICE")
            //    gridView1.SetRowCellValue(e.RowHandle, "AMOUNT", (decimal)gridView1.GetRowCellValue(e.RowHandle, "INVENTORY") * (decimal)e.Value);
            //    }
                
            }
            catch (Exception ex )
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtTLbs_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar != (char)Keys.Return) return;
            //try
            //{
            //    ManualInputWeight(Convert.ToDecimal(txtTLbs.Text), "LBS");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            //}
        }
        private void txtTKgs_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar != (char)Keys.Return) return;
            //try
            //{
            //    ManualInputWeight(Convert.ToDecimal(txtTKgs.Text),"KGS");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }
        private void btnClosePO_Click(object sender, EventArgs e)
        {
            //if (!SaveData()) return;//สั่งเซฟก่อนเพื่อบันทึกจำนวนทั้งหมดของใบรับนี้
            try
            {
                gridView1.SetRowCellValue(0, "SCHEDULE", (decimal)gridView1.GetRowCellValue(0, "PAST") );
                gridView1.SetRowCellValue(0, "BALANCE",0);//ปรับค่าช่อง balance ให้เป็น 0
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void slePO_QueryCloseUp(object sender, CancelEventArgs e)
        {
            try
            {
                checkBalance = true;//ผู้ใช้เป็นคนเลือก PO เองไม่ได้เกิดจาก event EditValueChanged ของ PO ที่มันผูกอยู่กับ Receive No. ฉะนั้นต้องทำการเช็คค่าก่อนว่ามีการเลือกตัวที่เป็น 0 หรือไม่
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dtpDate_QueryPopUp(object sender, CancelEventArgs e)
        {
            System.Diagnostics.Debug.Print("querypopup");
        }
        private void dtpDate_QueryCloseUp(object sender, CancelEventArgs e)
        {
            System.Diagnostics.Debug.Print("close");
        }
        private void dtpDate_EditValueChanging(object sender, ChangingEventArgs e)
        {
            
            //try
            //{
            //    sleRecNo.EditValueChanged -= sleRecNo_EditValueChanged;
            //    slePO.EditValueChanged -= slePO_EditValueChanged;

            //    if (e.OldValue == null) throw new ApplicationException();
            //    DateTime oldMonth = ((DateTime)e.OldValue);
            //    DateTime newMonth = ((DateTime)e.NewValue);
            //    if (Equals(oldMonth.Month, newMonth.Month)) throw new ApplicationException();
            //    MessageBox.Show(e.OldValue + "  " + e.NewValue);

            //    //sleRecNo.EditValue = null;
            //    //sleRecNo.Properties.DataSource = null;
            //    //ClearData();
            //    //ClearYarnDetail();
            //    sleRecNo.Properties.DataSource = GetReceiveNo(cboType.Text, newMonth);
            //    sleRecNo.Properties.DisplayMember = "RECNO";
            //    sleRecNo.Properties.ValueMember = "RECNO";
            //    string strReceiveNoNew = RunReceiveNoNew(cboType.Text);
            //    dtRecNo.BeginInit();
            //    DataRow dr = dtRecNo.NewRow();
            //    dr["RECNO"] = strReceiveNoNew;
            //    dtRecNo.Rows.Add(dr);
            //    dtRecNo.EndInit();
            //    sleRecNo.EditValue = strReceiveNoNew;
            //}
            //catch (SystemException ex)
            //{
            //    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //catch(ApplicationException ex)
            //{
            //}
            //sleRecNo.EditValueChanged += sleRecNo_EditValueChanged;
            //slePO.EditValueChanged += slePO_EditValueChanged;
        }
        private void dtpDate_Popup(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("popup");
        }
        private void dtpDate_Leave(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("leave");
        }






    }

}