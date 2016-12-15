using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System
{
    public partial class frmTS1_Receive : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        bool canEditQuantity;//ตัวแปรที่กำหนดว่าจะสามารถแก้ไขตัวเลขในกริดช่อง Quantity ได้ก็ต่่อเมื่อกดปุ่ม Refresh
        bool isFromReceive;//ตัวแปรที่เป็นตัวบอกว่าข้อมูลที่แสดงในกริดเป็นผลมาจากการเลือก ReceiveNo หรือ SupplierCode

        private string _connectionString;
        public string ConnectionString 
        {
            set{_connectionString=value;} 
        }
        private string _userName;
        public string UserName
        {
            set { _userName = value; }
        }

        public frmTS1_Receive()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            try
            {
                if (cboSection.SelectedIndex == -1) throw new ApplicationException("Please select Section");
                if (optMatType.SelectedIndex == -1) throw new ApplicationException("Please select classification (Mat. Type)");
                sleSupplier.EditValue = null;
                sleSupplier.Properties.DataSource = null;
                sleReceive.EditValue = null;
                sleReceive.Properties.DataSource = null;
                gridControl1.DataSource = null;
                ClearFooter();
                canEditQuantity = false;
                isFromReceive = false;
                GetSupplier(optMatType.SelectedIndex,cboSection.Text,cboRec.Text);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void SaveData()
        { 
        
        }
        public void ClearData()
        {
            dtpStart.EditValue = DateTime.Today;
            dtpEnd.EditValue = DateTime.Today;
            optMatType.SelectedIndex = -1;
            cboSection.SelectedIndex = -1;
            cboRec.SelectedIndex = 0;
            optReprint.SelectedIndex = -1;
            sleSupplier.EditValue = null;
            sleSupplier.Properties.DataSource = null;
            sleReceive.EditValue = null;
            sleReceive.Properties.DataSource = null;
            ClearFooter();
        }
        public void PrintPreview()
        {
            try
            {
                cCrystalReport crpPO = new cCrystalReport(Application.StartupPath + @"\Report\TS1_ReceiveNote.rpt");
                if (crpPO.SetPrinter() == false) { return; }
                crpPO.ReportTitle = sleReceive.EditValue.ToString();
                for (int i = 1; i <= crpPO.ReportCopy; i++)
                {
                    crpPO.ClearParameters();
                    crpPO.SetParameter("Copy", i.ToString());
                    string fmlText = "{PO_PURCHASE.PONO}='" + sleReceive.EditValue.ToString() + "'";
                    crpPO.PrintReport(fmlText, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Print()
        { 
        
        }

        private void ClearFooter()//เคลียข้อมูลในส่วน footer ของหน้า
        {
            //mdiMain parent = (mdiMain)this.ParentForm;
            optCommercial.SelectedIndex = -1;
            lblAmountText.Text = "";
            txtCreateUser.Text = _userName;
            txtCreateDate.Text = DateTime.Today.ToString("yyyyMMdd", dtfinfo);
            txtUpdateUser.Text = _userName;
            txtUpdateDate.Text = DateTime.Today.ToString("yyyyMMdd", dtfinfo);
            txtTotal.Text = "0";
            txtDiscount.Text = "0";
            txtDiscountAmt.Text = "0";
            txtVat.Text = (cboRec.Text == "I/I") ? "0" : "7";
            txtVatAmt.Text = "0";
            txtGrandTotal.Text = "0";
        }
        private void GetSupplier(int intMatType,string strSection,string strRecType)
        {
            string strSQL = "";
            sleReceive.EditValue = GetLatestReceive(strRecType);
            switch (intMatType)
            {
                case 0://Accessory Import
                    strSQL = "SELECT DISTINCT A.BUMO AS SUPCODE,B.NAME "+//,B.BUNR,C.OYAK " +
                        " FROM XSLIP A	INNER JOIN  XSECT B ON A.BUMO = B.BUMO " +
                        " INNER JOIN XHEAD C ON A.CODE = C.CODE " +
                        " INNER JOIN XITEM D ON C.CODE = D.CODE " +
                        " WHERE (B.BUNR LIKE 'I%') " +
                        " AND (A.NDATE >= '" + ((DateTime)dtpStart.EditValue).ToString("yyyyMMdd", dtfinfo) + "') " +
                        " AND (A.NDATE <= '" + ((DateTime)dtpEnd.EditValue).ToString("yyyyMMdd", dtfinfo) + "') ";
                    strSQL += " AND (C.OYAK = '255') AND SUBSTRING(D.HOKAN, 6, 2) LIKE ";
                    switch (strSection)
                    {
                        case "SALES1":
                            strSQL += "'%S1'";
                            break;
                        case "SALES2":
                            strSQL += "'%S2'";
                            break;
                    }
                    strSQL += " ORDER BY A.BUMO ";
                    break;
                case 1://Accessory Local
                    strSQL = "SELECT DISTINCT A.SUPCODE,B.NAME FROM THPURCHASE A "+
                        " INNER JOIN XSECT B ON A.SUPCODE = B.BUMO WHERE A.FSECTION='"+strSection+"'"+
                        " AND A.FLAGMAT='1' ORDER BY A.SUPCODE";
                    break;
                case 2://Out of Schedule
                    strSQL = "SELECT DISTINCT A.BUMO AS SUPCODE,B.NAME " +
                        "FROM XSACT A " +
                        "LEFT OUTER JOIN XSECT B ON A.BUMO = B.BUMO " +
                        "WHERE (A.FDATE >='" +((DateTime)dtpStart.EditValue).ToString("yyyyMMdd",dtfinfo) + "') "+
                        "AND (A.FDATE <='" +((DateTime)dtpEnd.EditValue).ToString("yyyyMMdd",dtfinfo)+ "') " +
                        "AND A.AKUBU = 'T' ORDER BY A.BUMO";
                    break;
                case 3://Fabric
                    strSQL = "SELECT DISTINCT A.SUPCODE,B.NAME FROM THPURCHASE A "+ 
	                    " INNER JOIN XSECT B ON A.SUPCODE = B.BUMO WHERE A.FSECTION ='"+strSection+"'"+ 
	                    " AND A.FLAGMAT ='3' ORDER BY A.SUPCODE";
                    break;
            }
            DataTable dt = db.GetDataTable(strSQL);
            sleSupplier.Properties.DataSource = dt;
            sleSupplier.Properties.PopulateViewColumns();
            sleSupplier.Properties.DisplayMember = "SUPCODE";
            sleSupplier.Properties.ValueMember = "SUPCODE";
        }
        private void GetSupplierDetail(string strSupplierCode)
        {
            string strSQL = "SELECT NAME,ADR1,ADR2,ZIP,TEL,FAX,PAYTERM,CURRE FROM XSECT A "+
                "WHERE BUMO='" + strSupplierCode + "'";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                txtSupplierID.Text = strSupplierCode;
                txtSupplier.Text = dr["NAME"].ToString();
                txtAddress1.Text = dr["ADR1"].ToString();
                txtAddress2.Text = dr["ADR2"].ToString();
                txtAddress3.Text = dr["ZIP"].ToString();
                txtTel.Text = dr["TEL"].ToString();
                txtFax.Text = dr["FAX"].ToString();
                txtPayTerm.Text = dr["PAYTERM"].ToString();
                txtCurrency.Text = dr["CURRE"].ToString();
            }
        }
        private string GetLatestReceive(string strGenData)
        {
            string xYear=DateTime.Today.ToString("yyyy",dtfinfo);
            string xMonth=DateTime.Today.ToString("MM",dtfinfo);
            string runNo="";
            string strSQL = "SELECT * FROM MGenRecNo WHERE GenData ='" + strGenData + "'" +
                " AND xYear ='" + xYear + "'"+
                " AND xMonth ='" + xMonth + "'";
            DataTable dt = db.GetDataTable(strSQL);
            if (dt==null || dt.Rows.Count == 0)
            {
                runNo = "0001";
            }
            else
            {
                runNo = (Convert.ToInt16(dt.Rows[0]["RunNo"])+1).ToString().PadLeft(4,'0');    
            }
            return strGenData + cUtility.Right(xYear, 2) + xMonth + "-" + runNo;
        }
        private void GetReceive()
        {
            string strSQL = "SELECT RECNO FROM RHRECEIVE WHERE SUBSTRING(RECNO,4,2) IN ("+
                "'"+DateTime.Today.AddYears(-1).ToString("yy")+"','"+DateTime.Today.ToString("yy")+"')"+
                " AND CANORNO ='0' ORDER BY RECNO,RECDATE";
            DataTable dt = db.GetDataTable(strSQL);
            sleReceive.Properties.DataSource = dt;
            sleReceive.Properties.PopulateViewColumns();
            sleReceive.Properties.DisplayMember = "RECNO";
            sleReceive.Properties.ValueMember = "RECNO";
        }
        private void GetReceiveDetail(string strReceiveNo)
        {
            string strSQL = "SELECT RECDATE,INVNO,DELNO,TOTAL,DISCOUNT,VAT,ATTN,REMARKS,GTOTAL,DICTOTAL,SUPCODE "+
                ",CRTUSER,CRTDATE,UPDUSER,UPDDATE,COMORNON,NAME,ADR1,ADR2,ZIP,TEL,FAX,PAYTERM,CURRE "+
                " FROM RHRECEIVE INNER JOIN XSECT ON RHRECEIVE.SUPCODE=XSECT.BUMO "+
                " WHERE RECNO ='" + strReceiveNo + "'";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                dtpRecDate.EditValue = dr["RECDATE"];
                txtInvoice.Text = dr["INVNO"].ToString();
                txtDelivery.Text = dr["DELNO"].ToString();
                txtAttn.Text = dr["ATTN"].ToString();
                txtRemark.Text = dr["REMARKS"].ToString();
                optCommercial.SelectedIndex = optCommercial.Properties.Items.GetItemIndexByValue(dr["COMORNON"]);
                txtTotal.Text = dr["TOTAL"].ToString();
                txtDiscount.Text = dr["DISCOUNT"].ToString();
                txtVat.Text = dr["VAT"].ToString();
                txtGrandTotal.Text = dr["GTOTAL"].ToString();
                lblAmountText.Text = dr["DICTOTAL"].ToString();
                txtCreateUser.Text = dr["CRTUSER"].ToString();
                txtCreateDate.Text = dr["CRTDATE"].ToString();
                txtUpdateUser.Text = dr["UPDUSER"].ToString();
                txtUpdateDate.Text = dr["UPDDATE"].ToString();
                txtSupplierID.Text = dr["SUPCODE"].ToString();
                txtSupplier.Text = dr["NAME"].ToString();
                txtAddress1.Text = dr["ADR1"].ToString();
                txtAddress2.Text = dr["ADR2"].ToString();
                txtAddress3.Text = dr["ZIP"].ToString();
                txtTel.Text = dr["TEL"].ToString();
                txtFax.Text = dr["FAX"].ToString();
                txtPayTerm.Text = dr["PAYTERM"].ToString();
                txtCurrency.Text = dr["CURRE"].ToString();
            }
            strSQL = "SELECT A.PONO,A.ORDNO AS TPICSNO,A.ITEMCODE AS PARTNO,A.ITEMNAME AS PARTDEL,A.UNIT,"+
                "(B.QTY-A.QTY) AS REMAIN,A.QTY AS QUANTITY,A.UPRC AS PRICE,A.AMT AS AMOUNT,A.FDATE,"+
                "A.SBNO AS SEIBAN "+
	            "FROM RDReceive A LEFT OUTER JOIN TDPURCHASE B ON A.PONO=A.PONO AND A.ORDNO=B.TPICSNO "+ 
                "WHERE A.RECNO ='"+strReceiveNo+"'";
            dt = db.GetDataTable(strSQL);
            if (dt == null || dt.Rows.Count == 0) return;
            dt.BeginInit();
            dt.Columns.Add("SELECT", typeof(System.Boolean),"1");
            dt.Columns.Add("CANCEL", typeof(System.Boolean), "0");
            dt.EndInit();
            gridControl1.DataSource = dt;
            gridView1.PopulateColumns();
            gridView1.Columns["SELECT"].Caption = "Select";
            gridView1.Columns["SELECT"].VisibleIndex = 0;
            gridView1.Columns["CANCEL"].Caption = "Cancel";
            gridView1.Columns["CANCEL"].VisibleIndex = 1;
            gridView1.Columns["PONO"].Caption = "P/O No.";
            gridView1.Columns["TPICSNO"].Caption = "Order No.";
            gridView1.Columns["PARTNO"].Caption = "Part No.";
            gridView1.Columns["PARTDEL"].Caption = "Part Description";
            gridView1.Columns["UNIT"].Caption = "Unit";
            gridView1.Columns["REMAIN"].Caption = "Remain Qty";
            gridView1.Columns["QUANTITY"].Caption = "Quantity";
            gridView1.Columns["PRICE"].Caption = "Unit Price";
            gridView1.Columns["AMOUNT"].Caption = "Amount";
            gridView1.Columns["FDATE"].Caption = "FDate";
            gridView1.Columns["SEIBAN"].Caption = "Seiban No.";

            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();

        }
        private void GetPO(string strSupplierCode,string strMatType)
        {
            canEditQuantity = false;
            gridControl1.DataSource = null;
            String strSQL = "EXEC SPTUWSYSTEM_RECEIVE_LOAD '"+strMatType+"','"+ strSupplierCode + "'";
            DataTable dt = db.GetDataTable(strSQL);
            if (dt == null||dt.Rows.Count==0) return;
            dt.BeginInit();
            DataColumn dc = new DataColumn();
            dc.ColumnName = "SELECT";
            dc.DataType = typeof(System.Boolean);
            dc.DefaultValue = false;
            dt.Columns.Add(dc);
            dc = new DataColumn();
            dc.ColumnName = "CANCEL";
            dc.DataType = typeof(System.Boolean);
            dc.DefaultValue = false;
            dt.Columns.Add(dc);
            dt.Columns.Add("REMAIN",typeof(System.Decimal),"QTY-RECEIVE");
            dc = new DataColumn();
            dc.ColumnName = "QUANTITY";
            dc.DataType = typeof(System.Decimal);
            dc.DefaultValue = "0";
            dt.Columns.Add(dc);
            dc = new DataColumn();
            dc.ColumnName = "AMOUNT";
            dc.DataType = typeof(System.Decimal);
            dc.DefaultValue = "0";
            dt.Columns.Add(dc);
            dt.Columns.Add("FDATE", typeof(System.String));
            dt.EndInit();
            gridControl1.DataSource = dt;
            gridView1.PopulateColumns();
            gridView1.Columns["SELECT"].Caption = "Select";
            gridView1.Columns["SELECT"].VisibleIndex = 0;
            gridView1.Columns["CANCEL"].Caption = "Cancel";
            gridView1.Columns["CANCEL"].VisibleIndex = 1;
            gridView1.Columns["PONO"].Caption = "P/O No.";
            gridView1.Columns["TPICSNO"].Caption = "Order No.";
            gridView1.Columns["PARTNO"].Caption = "Part No.";
            gridView1.Columns["PARTDEL"].Caption = "Part Description";
            gridView1.Columns["UNIT"].Caption = "Unit";
            gridView1.Columns["QTY"].Visible = false;
            gridView1.Columns["RECEIVE"].Visible = false;
            gridView1.Columns["REMAIN"].Caption = "Remain Qty";
            gridView1.Columns["REMAIN"].VisibleIndex = 7;
            gridView1.Columns["QUANTITY"].Caption = "Quantity";
            gridView1.Columns["QUANTITY"].VisibleIndex = 8;
            gridView1.Columns["PRICE"].Caption = "Unit Price";
            gridView1.Columns["PRICE"].VisibleIndex = 9;
            gridView1.Columns["AMOUNT"].Caption = "Amount";
            gridView1.Columns["AMOUNT"].VisibleIndex = 10;
            gridView1.Columns["FDATE"].Caption = "FDate";
            gridView1.Columns["FDATE"].VisibleIndex = 11;
            gridView1.Columns["SEIBAN"].Caption = "Seiban No.";
            gridView1.Columns["SEIBAN"].VisibleIndex = 12;

            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
        }
        private void CalculateGrandTotal(decimal total,decimal discount,decimal vat)
        {
            decimal discountAmount = total * (decimal)0.01 * discount;
            txtDiscountAmt.Text = discountAmount.ToString();
            decimal vatAmount = (total - discountAmount) * (decimal)0.01 * vat;
            txtVatAmt.Text = vatAmount.ToString();
            decimal grandTotal = (total - discountAmount) + vatAmount;
            txtGrandTotal.Text = grandTotal.ToString();
            lblAmountText.Text = cUtility.ThaiBaht(txtGrandTotal.Text);
        }

        private void optMatType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (optMatType.SelectedIndex)
            { 
                case 0://Accessory import
                    cboRec.SelectedIndex=cboRec.Properties.Items.IndexOf("I/I");
                    cboRec.Properties.ReadOnly = true;
                    break;
                case 1://Accessory local
                    cboRec.SelectedIndex = cboRec.Properties.Items.IndexOf("I/D");
                    cboRec.Properties.ReadOnly = true;
                    break;
                case 2://Out of schedule
                    cboRec.SelectedIndex = cboRec.Properties.Items.IndexOf("I/I");
                    cboRec.Properties.ReadOnly = false;
                    break;
                case 3://Fabric
                    cboRec.SelectedIndex = cboRec.Properties.Items.IndexOf("I/D");
                    cboRec.Properties.ReadOnly = false;
                    break;
            }
        }
        private void frmTS1_Receive_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            cboSection.Properties.Items.Add("SALES1");
            cboSection.Properties.Items.Add("SALES2");
            cboSection.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboRec.Properties.Items.Add("I/I");
            cboRec.Properties.Items.Add("I/D");
            cboRec.Properties.ReadOnly = true;
            cboRec.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        }
        private void sleSupplier_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtSupplierID.Text = "";
                txtSupplier.Text = "";
                txtAddress1.Text = "";
                txtAddress2.Text = "";
                txtAddress3.Text = "";
                txtTel.Text = "";
                txtFax.Text = "";
                txtPayTerm.Text = "";
                txtCurrency.Text = "";

                ClearFooter();
                canEditQuantity = false;
                if (sleSupplier.EditValue == null)
                {
                    GetPO(null,null);
                }
                else
                {
                    GetSupplierDetail(sleSupplier.EditValue.ToString());
                    GetPO(sleSupplier.EditValue.ToString(),optMatType.SelectedIndex.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
            
        }
        private void sleReceive_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                dtpRecDate.EditValue = DateTime.Today;
                txtInvoice.Text = "";
                txtDelivery.Text = "";
                txtAttn.Text = "";
                txtFreight.Text = "";
                txtShipVia.Text = "";
                txtRemark.Text = "";

                txtSupplierID.Text = "";
                txtSupplier.Text = "";
                txtAddress1.Text = "";
                txtAddress2.Text = "";
                txtAddress3.Text = "";
                txtTel.Text = "";
                txtFax.Text = "";
                txtPayTerm.Text = "";
                txtCurrency.Text = "";

                gridControl1.DataSource = null;

                ClearFooter();
                if (sleReceive.EditValue == null)
                    GetReceiveDetail(null);
                else
                    GetReceiveDetail(sleReceive.EditValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void sleSupplier_Properties_Popup(object sender, EventArgs e)
        {
            sleSupplierView.BestFitColumns();
        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                gridView1.SetRowCellValue(i, "SELECT", true);
            }
        }
        private void btnUnSelect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                gridView1.SetRowCellValue(i, "SELECT", false);
            }
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            for (int i = gridView1.RowCount - 1; i >= 0; i--)
            {
                if ((bool)gridView1.GetRowCellValue(i, "SELECT") == false) gridView1.DeleteRow(i);
            }
            canEditQuantity = true;
        }
        private void btnCancelTable_Click(object sender, EventArgs e)
        {

        }
        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (canEditQuantity)
                e.Cancel = false;
            else
                e.Cancel=(gridView1.FocusedColumn.FieldName=="SELECT")?false:true;
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            gridView1.IndicatorWidth = 35;
        }
        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                int row=gridView1.FocusedRowHandle;
                switch (gridView1.FocusedColumn.FieldName)
                { 
                    case "QUANTITY":
                        gridView1.SetRowCellValue(row,"AMOUNT",Convert.ToDecimal(e.Value)*(decimal)gridView1.GetRowCellValue(row,"PRICE"));
                        break;
                }
                decimal total = 0;
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    total += (decimal)gridView1.GetRowCellValue(i, "AMOUNT");
                }
                txtTotal.Text = total.ToString();
            }
            catch{}
        }
        private void txtDiscount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CalculateGrandTotal(Convert.ToDecimal(txtTotal.Text),Convert.ToDecimal(txtDiscount.Text),Convert.ToDecimal(txtVat.Text));
            }
            catch {}
        }
        private void txtVat_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CalculateGrandTotal(Convert.ToDecimal(txtTotal.Text), Convert.ToDecimal(txtDiscount.Text), Convert.ToDecimal(txtVat.Text));
            }
            catch{}
        }
        private void optReprint_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                sleSupplier.EditValue = null;
                sleSupplier.Properties.DataSource = null;
                sleReceive.EditValue = null;
                sleReceive.Properties.DataSource = null;
                gridControl1.DataSource = null;
                ClearFooter();
                canEditQuantity = false;
                isFromReceive = true;
                GetReceive();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}