using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System.YS
{
    public partial class frmYS_IssueSubcontract : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;

        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        CultureInfo clinfo_th = new CultureInfo("th-TH");
        DateTimeFormatInfo dtfinfo, dtfinfo_th;
        DataTable dtDelNo, dtDelDetail;

        private const decimal lbs = 2.2046M;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        public LogIn User_Login { get; set; }

        public frmYS_IssueSubcontract()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            try
            {
                cboType.SelectedIndexChanged -= cboType_SelectedIndexChanged;
                sleDelNo.EditValueChanged -= sleDelNo_EditValueChanged;
                slePO.EditValueChanged -= slePO_EditValueChanged;

                cboType.Text = "";
                sleDelNo.EditValue = null;
                slePO.EditValue = null;
                ClearData();
                sleDelNo.Properties.DataSource = null;
                slePO.Properties.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
            cboType.SelectedIndexChanged += cboType_SelectedIndexChanged;
            sleDelNo.EditValueChanged += sleDelNo_EditValueChanged;
            slePO.EditValueChanged += slePO_EditValueChanged;
        }
        public void SaveData()
        { 
            this.Cursor=Cursors.WaitCursor;
            try
            {
                if (cboType.Text.Length == 0) return;
                if (sleDelNo.Text.Length != 11) throw new ApplicationException("เลขที่เอกสารไม่ถูกต้อง");

                db.ConnectionOpen();
                db.BeginTrans();
                string strType = "";
                switch (cboType.Text)
                {
                    case "จ้างทอ":
                        strType = "Subcontract Knit";
                        break;
                    case "จ้างย้อม":
                        strType = "Subcontract Dye";
                        break;
                    case "จ้างกรอด้าย":
                        strType = "Subcontract Wind";
                        break;
                    case "จ้างทอตัวอย่าง":
                        strType = "Subcontract Knit Sample";
                        break;
                    case "อื่นๆ":
                        strType = "Other";
                        break;
                }

                string strSQL = "SELECT COUNT(IssNo) FROM YarnIssue WHERE IssNo = '" + sleDelNo.Text + "'";
                if (db.ExecuteFirstValue(strSQL) == "0")
                {
                    strSQL = "INSERT INTO YarnIssue (IssRun,IssNo,IssDate,IssType,PoNo_LotNo,RequestBy,IssueBy," +
                        "Supplier,Remark) VALUES ('" + sleDelNo.Text.Substring(5, 3) + "','" + sleDelNo.Text + "'," +
                        "'" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "',N'" + strType + "'," +
                        "'" + slePO.Text + "','" + txtUDel.Text + "','" + txtIss.Text + "','" + txtSupplier1.Text + "'," +
                        "N'" + txtRemark.Text + "')";
                    db.Execute(strSQL);
                }
                else
                {
                    strSQL = "UPDATE YarnIssue SET PoNo_LotNo ='" + slePO.Text + "',RequestBy='" + txtUDel.Text + "'," +
                        "IssueBy='" + txtIss.Text + "',IssDate='" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'" +
                        ",Remark = N'" + txtRemark.Text + "',Supplier='"+txtSupplier1.Text+"' WHERE IssNo = '" + sleDelNo.Text + "'";
                    db.Execute(strSQL);
                }
                //SaveYarnIssueDetail
                int count = 0;
                for (int i = 0; i < gridView2.DataRowCount; i++)
                {
                    strSQL = "SELECT COUNT(Serial) FROM YarnIssueDetail " +
                        "WHERE Serial='" + gridView2.GetRowCellDisplayText(i, "SERIAL") + "'";
                    if (db.ExecuteFirstValue(strSQL) == "0")
                        count += 1;
                    else
                    {
                        string strSOL = "DELETE YarnIssueDetail WHERE IssNo = '" + sleDelNo.Text + "' " +
                        "and Serial='" + gridView2.GetRowCellDisplayText(i, "SERIAL") + "'";
                        db.Execute(strSOL);
                    }
                    strSQL = "INSERT INTO YarnIssueDetail(IssNo,YarnID,Serial,Code,NetWeight,LotNo)" +
                            "VALUES ('" + sleDelNo.Text + "','" + gridView2.GetRowCellDisplayText(i, "SERIAL").Substring(0, 4) + "'," +
                            "'" + gridView2.GetRowCellDisplayText(i, "SERIAL") + "'," +
                            "'" + gridView2.GetRowCellDisplayText(i, "CODE") + "'," +
                            "'" + gridView2.GetRowCellValue(i, "NETWEIGHT") + "'," +
                            "'" + gridView2.GetRowCellDisplayText(i, "LOTNO") + "')";
                    db.Execute(strSQL);
                    UpdateStock("Insert", gridView2.GetRowCellDisplayText(i, "SERIAL"));
                }
                db.CommitTrans();
                lblCtn.Text = count.ToString();
                MessageBox.Show("บันทึกข้อมูลเลขที่ " + sleDelNo.Text + " เรียบร้อย", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (SystemException ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor=Cursors.Default;
            }
            db.ConnectionClose();
        }
        public void PrintPreview()
        {
            try
            {
                cCrystalReport crpIssue = new cCrystalReport(Application.StartupPath + @"\Report\YS_Issue.rpt");
                if (crpIssue.SetPrinter() == false) return;
                crpIssue.ReportTitle = sleDelNo.Text;
                for (int i = 1; i <= crpIssue.ReportCopy; i++)
                {
                    string fmlText = "{YARNISSUEDETAIL.ISSNO}='" + sleDelNo.Text + "'";
                    crpIssue.ReportSize = cCrystalReport.Paper.Letter;
                    crpIssue.PrintReport(fmlText, false, "sa", "ZAQ113m4tuw");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Print()
        {
            try
            {
                cCrystalReport crpIssue = new cCrystalReport(Application.StartupPath + @"\Report\YS_Issue.rpt");
                if (crpIssue.SetPrinter() == false) return;
                crpIssue.ReportTitle = sleDelNo.Text;
                for (int i = 1; i <= crpIssue.ReportCopy; i++)
                {
                    string fmlText = "{YARNISSUEDETAIL.ISSNO}='" + sleDelNo.Text + "'";
                    crpIssue.ReportSize = cCrystalReport.Paper.Letter;
                    crpIssue.PrintReport(fmlText, true, "sa", "ZAQ113m4tuw");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearData()
        {
            //if (clearDate) dtpDate.EditValue = DateTime.Today;
            dtpDatePO.EditValue = null;
            txtUDel.Text = "";
            txtIss.Text = "";
            txtRemark.Text = "";
            lblTCarton.Text = "";
            lblTNweight.Text = "";
            lblCtn.Text = "";
            txtBarcode.Text = "";
            txtSupplier1.Text = "";
            txtSupplier2.Text = "";
            txtSupplier3.Text = "";
            txtSupplier4.Text = "";

            gridControl1.DataSource = null;

            gridControl2.DataSource = null;
            dtDelDetail = new DataTable();
            DataColumn dc = new DataColumn();
            dc.Caption = "Delete";
            dc.ColumnName = "DEL";
            dc.DataType = typeof(bool);
            dc.DefaultValue = false;
            dtDelDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Yarn Code";
            dc.ColumnName = "CODE";
            dc.DataType = typeof(string);
            dtDelDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Color";
            dc.ColumnName = "COLOR";
            dc.DataType = typeof(string);
            dtDelDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Special";
            dc.ColumnName = "SPECIAL";
            dc.DataType = typeof(string);
            dtDelDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Supplier";
            dc.ColumnName = "SUPPLIER";
            dc.DataType = typeof(string);
            dtDelDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Ctn No.";
            dc.ColumnName = "CTNNO";
            dc.DataType = typeof(int);
            dtDelDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Net Weight";
            dc.ColumnName = "NETWEIGHT";
            dc.DataType = typeof(decimal);
            dtDelDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Serial";
            dc.ColumnName = "SERIAL";
            dc.DataType = typeof(string);
            dtDelDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Lot No.";
            dc.ColumnName = "LOTNO";
            dc.DataType = typeof(string);
            dtDelDetail.Columns.Add(dc);
            gridControl2.DataSource = dtDelDetail;
            gridView2.OptionsView.EnableAppearanceEvenRow = true;
            gridView2.OptionsView.EnableAppearanceOddRow = true;
            gridView2.OptionsView.ColumnAutoWidth = false;
            gridView2.BestFitColumns();
        }
        private DataTable GetDelNo(string strType, DateTime datSearch)
        { 
            string strSQL="";
            switch(strType)
            {
                case "จ้างทอ":
                    strSQL = "SELECT ISSNO FROM YarnIssue WHERE IssNo like 'IF%' and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo)+ "' and IssType = 'Subcontract Knit' ORDER BY IssNo";
                    break;
                case "จ้างย้อม":
                    strSQL = "SELECT ISSNO FROM YarnIssue WHERE IssNo like 'ID%' and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo)+ "' ORDER BY IssNo";
                    break;
                case "จ้างกรอด้าย":   
                    strSQL = "SELECT ISSNO FROM YarnIssue WHERE IssNo like 'IW%' and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo)+ "' ORDER BY IssNo";
                    break;
                case "จ้างทอตัวอย่าง":
                    strSQL = "SELECT ISSNO FROM YarnIssue WHERE IssNo like 'IF%' and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo)+ "' and IssType = 'Subcontract Knit Sample' ORDER BY IssNo";
                    break;
                case "อื่นๆ":
                    strSQL = "SELECT ISSNO FROM YarnIssue WHERE IssNo like 'IE%' and CONVERT(char(7), IssDate, 120) = '" + datSearch.ToString("yyyy-MM", dtfinfo) + "' and IssType = 'Other' ORDER BY IssNo";
                    break;
            }
            dtDelNo = db.GetDataTable(strSQL);
            return dtDelNo;
        }
        private string RunDelNONew(string strDelType,DateTime datSearch)
        { 
            string strSQL="";
            string strType="";
            switch(strDelType)
            {
                case "จ้างทอ":
                case "จ้างทอตัวอย่าง":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(IssRun)+1,1))),3) as IssRun FROM YarnIssue WHERE IssNo like 'IF%' and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo) + "'";
                    strType = "IF";
                    break;
                case "จ้างย้อม":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(IssRun)+1,1))),3) as IssRun FROM YarnIssue WHERE IssNo like 'ID%' and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo) + "'";
                    strType = "ID";
                    break;
                case "จ้างกรอด้าย":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(IssRun)+1,1))),3) as IssRun FROM YarnIssue WHERE IssNo like 'IW%' and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo) + "'";
                    strType = "IW";
                    break;
                case "อื่นๆ":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(IssRun)+1,1))),3) as IssRun FROM YarnIssue WHERE IssNo like 'IE%' and CONVERT(char(7), IssDate, 120) = '" + datSearch.ToString("yyyy-MM", dtfinfo) + "'";
                    strType = "IE";
                    break;
            }
            db.ConnectionOpen();
            string strNo=db.ExecuteFirstValue(strSQL);
            string strDelRun=strType + datSearch.ToString("MM", dtfinfo) + "-" + strNo + "/" + datSearch.ToString("yy", dtfinfo);
            db.ConnectionClose();
            return strDelRun;
        }
        private void GetDelDetail(string strDelNo,string strType)
        { 
            switch(strType)
            {
                case "จ้างทอ":
                    strType = "Subcontract Knit";
                    break;
                case "จ้างย้อม":
                    strType = "Subcontract Dye";
                    break;
                case "จ้างกรอด้าย":
                    strType = "Subcontract Wind";
                    break;
                case "จ้างทอตัวอย่าง":
                    strType = "Subcontract Knit Sample";
                    break;
                case "อื่นๆ":
                    strType = "Other";
                    break;
            }
            string strSQL = "SELECT ISSRUN,ISSNO,ISSDATE,ISSTYPE,PONO_LOTNO,REQUESTBY,ISSUEBY,SUPPLIER,"+
                "Remark FROM YarnIssue WHERE IssNo = '"+sleDelNo.Text+"' AND IssType='"+strType+"'";
            DataTable dt=db.GetDataTable(strSQL);
            foreach(DataRow dr in dt.Rows)
            {
                dtpDate.EditValue=dr["ISSDATE"];
                slePO.EditValue=dr["PONO_LOTNO"];
                txtUDel.Text = dr["REQUESTBY"].ToString();
                txtIss.Text = dr["ISSUEBY"].ToString();
                txtRemark.Text=dr["REMARK"].ToString();
            }
            strSQL = "SELECT CONVERT(BIT,0) as DEL, A.CODE,C.COLOR,C.SPECIAL,C.SUPPLIER,B.CTNNO," +
                "A.NETWEIGHT,A.SERIAL,A.LOTNO FROM YarnIssueDetail A " +
                "LEFT OUTER JOIN YARNGENBARCODE B ON A.SERIAL=B.SERIAL " +
                "LEFT OUTER JOIN YARNCODE C ON A.YARNID=C.ID " +
                "WHERE A.IssNo = '" + strDelNo + "' ORDER BY A.ID";
            dtDelDetail = db.GetDataTable(strSQL);
            gridControl2.DataSource = dtDelDetail;
            gridView2.PopulateColumns();
            gridView2.Columns["DEL"].Caption = "Delete";
            gridView2.Columns["CODE"].Caption = "Yarn Code";
            gridView2.Columns["COLOR"].Caption = "Color";
            gridView2.Columns["SPECIAL"].Caption = "Special";
            gridView2.Columns["SUPPLIER"].Caption = "Supplier";
            gridView2.Columns["CTNNO"].Caption = "Ctn No.";
            gridView2.Columns["NETWEIGHT"].Caption = "Net Weight";
            gridView2.Columns["SERIAL"].Caption = "Yarn Serial";
            gridView2.Columns["LOTNO"].Caption = "Lot No.";

            gridView2.Columns["NETWEIGHT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView2.Columns["NETWEIGHT"].DisplayFormat.FormatString = "n2";

            gridView2.OptionsView.EnableAppearanceEvenRow = true;
            gridView2.OptionsView.EnableAppearanceOddRow = true;
            gridView2.OptionsView.ColumnAutoWidth = false;
            gridView2.BestFitColumns();

        }
        private DataTable GetPONo(string strType,DateTime datSearch)
        { 
            string strSQL="";
            switch (strType)
            {
                case "จ้างทอ":
                    strSQL = "SELECT PONO FROM PO_PURCHASE WHERE YEAR(PODATE) IN ('" + datSearch.ToString("yyyy", dtfinfo) + "'," +
                        "'" + datSearch.AddYears(-1).ToString("yyyy", dtfinfo) + "') AND PONO LIKE 'FB.S%'";
                    break;
                case "จ้างทอตัวอย่าง":
                    strSQL = "SELECT PONO FROM PO_PURCHASE WHERE YEAR(PODATE) IN ('" + datSearch.ToString("yyyy", dtfinfo) + "'," +
                        "'" + datSearch.AddYears(-1).ToString("yyyy", dtfinfo) + "') AND PONO LIKE 'FBS%'";
                    break;
                case "อื่นๆ":
                    strSQL = "SELECT PONO FROM PO_PURCHASE WHERE YEAR(PODATE) IN ('" + datSearch.ToString("yyyy", dtfinfo) + "'," +
                        "'" + datSearch.AddYears(-1).ToString("yyyy", dtfinfo) + "') AND PONO LIKE 'FD%'";
                    break;
                default:
                    strSQL = "SELECT PONO FROM PO_PURCHASE WHERE YEAR(PODATE) IN ('" + datSearch.ToString("yyyy", dtfinfo) + "'," +
                        "'" + datSearch.AddYears(-1).ToString("yyyy", dtfinfo) + "') AND PONO LIKE 'FX%'";
                    break;
            }
            DataTable dt = db.GetDataTable(strSQL);
            return dt;
        }
        private void GetPODetail(string strPO,string strType)
        { 
            string strSQL= "SELECT A.IDSUP AS SupplierID,A.PODATE,"+
                "A.CURRENCYUNIT AS MoneyType,B.NAME AS DepartmentID, "+
                "C.NAME,C.ADDRESS1,C.ADDRESS2,'TEL. '+C.telephone+' FAX. '+C.fax as ADDRESS3 "+
                "FROM PO_PURCHASE A LEFT OUTER JOIN PO_DEPARTMENT B ON A.IDDEPT=B.IDDEPT "+
                "LEFT OUTER JOIN PO_SUPPLIER C ON A.IDSUP=C.IDSUP "+
                "WHERE A.PONO='"+strPO+"'";
            DataTable dt=db.GetDataTable(strSQL);
            foreach(DataRow dr in dt.Rows)
            {
                dtpDatePO.EditValue=dr["PODATE"];
                txtSupplier1.Text=dr["NAME"].ToString();
                txtSupplier2.Text=dr["ADDRESS1"].ToString();
                txtSupplier3.Text=dr["ADDRESS2"].ToString();
                txtSupplier4.Text=dr["ADDRESS3"].ToString();
            }
            strSQL="SELECT A.PRODUCTCODE,A.PRODUCTNAME,A.DESCRIPTION,A.QTY,B.UNIT,A.UNITPRICE "+
                " FROM PO_PURCHASEDETAIL A LEFT OUTER JOIN PO_UNIT B ON A.IDUNIT=B.IDUNIT "+
                "WHERE A.PONO = '"+strPO+"' "+
                "ORDER BY A.PRODUCTCODE DESC";
            dt=db.GetDataTable(strSQL);
            DataColumn dc=new DataColumn();
            dc.ColumnName="AMOUNT";
            dc.DataType=typeof(decimal);
            dc.Expression="QTY*UNITPRICE";
            dt.Columns.Add(dc);
            gridControl1.DataSource=dt;

            gridView1.PopulateColumns();
            gridView1.Columns["PRODUCTCODE"].Caption="Product Code";
            gridView1.Columns["PRODUCTNAME"].Caption="Product Name";
            gridView1.Columns["DESCRIPTION"].Caption="Description";
            gridView1.Columns["QTY"].Caption="Quantity";
            gridView1.Columns["UNIT"].Caption="Unit";
            gridView1.Columns["UNITPRICE"].Caption="Unit Price";

            if (strType == "จ้างทอ") 
            {
                var memoEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();//.RepositoryItemMemoExEdit();
                memoEdit.WordWrap = true;
                memoEdit.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                memoEdit.Appearance.Options.UseTextOptions = true;
                gridView1.Columns["DESCRIPTION"].ColumnEdit = memoEdit;
                gridView1.Columns["DESCRIPTION"].AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            }
            
            gridView1.Columns["QTY"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["QTY"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["AMOUNT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["AMOUNT"].DisplayFormat.FormatString = "n2";

            gridView1.OptionsBehavior.ReadOnly = true;
            gridView1.OptionsView.RowAutoHeight = true;
            gridView1.OptionsView.EnableAppearanceEvenRow=true;
            gridView1.OptionsView.EnableAppearanceOddRow=true;
            gridView1.OptionsView.ColumnAutoWidth=false;
            gridView1.BestFitColumns();
        }
        private void SumTotal()
        {
            decimal decWeight = 0;
            for (int i = 0; i < gridView2.DataRowCount; i++)
            {
                decWeight += (decimal)gridView2.GetRowCellValue(i, "NETWEIGHT");
            }
            lblTCarton.Text = gridView2.DataRowCount.ToString();
            lblTNweight.Text = decWeight.ToString("#,0.00");
        }
        private string GetUserIssue(string strID)
        {
            string strSQL = "SELECT Name FROM BarcodeText WHERE Code = '" + strID + "'";
            return db.ExecuteFirstValue(strSQL);
        }
        private void UpdateStock(string operate, string serial)
        {
            string strSQL = "";
            if (operate == "Insert")
                strSQL = "UPDATE YARNGENBARCODE SET SYSDELETE=1,OUTDATE=GETDATE() WHERE SERIAL='" + serial + "'";
            else
                strSQL = "UPDATE YARNGENBARCODE SET SYSDELETE=0,OUTDATE=NULL WHERE SERIAL='" + serial + "'";
            db.Execute(strSQL);
        }

        private void frmYS_IssueSubcontract_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            dtfinfo_th = clinfo_th.DateTimeFormat;
            List<string> lstMonth = new List<string>(dtfinfo_th.MonthNames);
            foreach (string strName in lstMonth)
            {
                cboMonth.Properties.Items.Add(strName);
            }
            cboYear.Properties.Items.Add(DateTime.Today.AddYears(-1).Year);
            cboYear.Properties.Items.Add(DateTime.Today.AddYears(0).Year);
            cboYear.Properties.Items.Add(DateTime.Today.AddYears(1).Year);
            cboMonth.SelectedIndex = DateTime.Today.Month - 1;
            cboYear.SelectedIndex = 1;
            NewData();
        }
        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                sleDelNo.EditValueChanged -= sleDelNo_EditValueChanged;
                sleDelNo.EditValue = null;
                sleDelNo.Properties.DataSource = null;
                slePO.EditValueChanged -= slePO_EditValueChanged;
                slePO.EditValue = null;
                ClearData();

                sleDelNo.Properties.DataSource = GetDelNo(cboType.Text, new DateTime(Convert.ToInt32(cboYear.Text), cboMonth.SelectedIndex + 1, 1));
                sleDelNo.Properties.DisplayMember = "ISSNO";
                sleDelNo.Properties.ValueMember = "ISSNO";
                string strDelNoNew = RunDelNONew(cboType.Text, new DateTime(Convert.ToInt32(cboYear.Text), cboMonth.SelectedIndex + 1, 1));
                int monthYear = Convert.ToInt32(cboYear.Text + (cboMonth.SelectedIndex + 1).ToString().PadLeft(2, '0'));
                if (monthYear < Convert.ToInt32(DateTime.Today.ToString("yyyyMM", dtfinfo)))
                    dtpDate.EditValue = new DateTime(Convert.ToInt32(cboYear.Text), cboMonth.SelectedIndex + 1, DateTime.DaysInMonth(Convert.ToInt32(cboYear.Text), cboMonth.SelectedIndex + 1));
                else if (monthYear > Convert.ToInt32(DateTime.Today.ToString("yyyyMM", dtfinfo)))
                    dtpDate.EditValue = new DateTime(Convert.ToInt32(cboYear.Text), cboMonth.SelectedIndex + 1, 1);
                else
                    dtpDate.EditValue = DateTime.Today;
                dtDelNo.BeginInit();
                DataRow dr = dtDelNo.NewRow();
                dr["ISSNO"] = strDelNoNew;
                dtDelNo.Rows.Add(dr);
                dtDelNo.EndInit();
                sleDelNo.EditValue = strDelNoNew;

                slePO.Properties.DataSource = GetPONo(cboType.Text, new DateTime(Convert.ToInt32(cboYear.Text), cboMonth.SelectedIndex + 1, 1));
                slePO.Properties.DisplayMember = "PONO";
                slePO.Properties.ValueMember = "PONO";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            sleDelNo.EditValueChanged += sleDelNo_EditValueChanged;
            slePO.EditValueChanged += slePO_EditValueChanged; 
        }
        private void sleDelNo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ClearData();
                slePO.EditValueChanged -= slePO_EditValueChanged;
                slePO.EditValue = null;
                slePO.EditValueChanged += slePO_EditValueChanged;
                GetDelDetail(sleDelNo.Text,cboType.Text);
                SumTotal();
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
		        db.ConnectionOpen();
                GetPODetail(slePO.Text,cboType.Text);
	        }
	        catch (Exception ex)
	        {
		        MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
	        }
            db.ConnectionClose();
            txtUDel.Focus();
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
        private void btnDelRow_Click(object sender, EventArgs e)
        {
            try
            {
                db.ConnectionOpen();
                db.BeginTrans();
                for (int i = gridView2.DataRowCount - 1; i >= 0; i--)
                {
                    if ((bool)gridView2.GetRowCellValue(i, "DEL") == false) continue;
                    if (MessageBox.Show("คุณต้องการที่จะลบข้อมูลเส้นด้าย " + gridView2.GetRowCellDisplayText(i, "CODE") +
                        Environment.NewLine + "Serial: " + gridView2.GetRowCellDisplayText(i, "SERIAL"),
                        "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        string strSQL = "DELETE FROM YarnIssueDetail WHERE IssNo='" + sleDelNo.Text + "' " +
                            "AND Serial='" + gridView2.GetRowCellDisplayText(i, "SERIAL") + "'";
                        db.Execute(strSQL);
                        UpdateStock("Delete", gridView2.GetRowCellDisplayText(i, "SERIAL"));
                        gridView2.DeleteRow(i);
                    }
                }
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
        private void txtUDel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar!=(char)Keys.Return) return;
            try 
	        {	        
		        db.ConnectionOpen();
                txtUDel.Text = GetUserIssue(txtUDel.Text);
	        }
	        catch (Exception ex)
	        {
                MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);		
	        }
            db.ConnectionClose();
            txtIss.Focus();
        }
        private void txtIss_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar!=(char)Keys.Return)return;
            try 
	        {	        
		        db.ConnectionOpen();
                txtIss.Text = GetUserIssue(txtIss.Text);
	        }
	        catch (Exception ex)
	        {
		        MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
	        }
            db.ConnectionClose();
            txtBarcode.Focus();
        }
        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Return) return;
            try
            {
                //Check duplicate
                for (int i = 0; i < gridView2.DataRowCount; i++)
                {
                    if (txtBarcode.Text.ToUpper() == gridView2.GetRowCellDisplayText(i, "SERIAL"))
                    {
                        txtBarcode.Text = "";
                        StatusBarEvent("");
                        return;
                    }
                }

                db.ConnectionOpen();
                if (cboType.Text.Length == 0) throw new SystemException("คุณยังไม่ได้เลือกประเภทการเบิก!");
                string strSQL = "SELECT COUNT(ID) FROM YARNISSUEDETAIL WHERE SERIAL='" + txtBarcode.Text + "'";
                if (db.ExecuteFirstValue(strSQL) == "0")
                {
                    strSQL = "SELECT 0 AS DEL, A.YARNCODE AS CODE,C.COLOR,C.SPECIAL,C.SUPPLIER,A.CTNNO,A.NETWEIGHT," +
                        "A.SERIAL,A.LOTNO " +
                        "FROM YARNGENBARCODE A LEFT OUTER JOIN YARNCODE C ON A.YARNID=C.ID " +
                        "WHERE A.SERIAL = '" + txtBarcode.Text + "' AND A.SYSDELETE=0";
                    DataTable dt = db.GetDataTable(strSQL);
                    if (dt == null || dt.Rows.Count == 0) throw new ApplicationException("Serial " + txtBarcode.Text + " ยังไม่ได้ถูกบันทึกในเอกสารรับเข้าหรือถูกปรับสต็อก");
                    dtDelDetail.BeginInit();
                    DataRow dr = dtDelDetail.NewRow();
                    dr.ItemArray = dt.Rows[0].ItemArray;
                    dtDelDetail.Rows.Add(dr);
                    dtDelDetail.EndInit();
                    gridView2.Columns["NETWEIGHT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    gridView2.Columns["NETWEIGHT"].DisplayFormat.FormatString = "n2";
                    gridView2.BestFitColumns();
                    gridView2.OptionsNavigation.AutoFocusNewRow = true;
                    SumTotal();
                    txtBarcode.Text = "";
                    StatusBarEvent("");
                }
                else
                {
                    throw new ApplicationException("Serial " + txtBarcode.Text + " นี้มีการยิงออกแล้ว ? Check Stock ก่อน...มีรับคืนหรือไม่");
                }
            }
            catch (ApplicationException ex)
            {
                StatusBarEvent(ex.Message.ToString());
                txtBarcode.Text = "";
                //MessageBox.Show(ex.Message,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    sleDelNo.EditValueChanged -= sleDelNo_EditValueChanged;
            //    ClearData();
            //    sleDelNo.Properties.DataSource = GetDelNo(cboType.Text, (DateTime)dtpDate.EditValue);
            //    sleDelNo.Properties.DisplayMember = "ISSNO";
            //    sleDelNo.Properties.ValueMember = "ISSNO";
            //    string strDelNoNew = RunDelNONew(cboType.Text);
            //    dtDelNo.BeginInit();
            //    DataRow dr = dtDelNo.NewRow();
            //    dr["ISSNO"] = strDelNoNew;
            //    dtDelNo.Rows.Add(dr);
            //    dtDelNo.EndInit();
            //    sleDelNo.EditValue = strDelNoNew;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //sleDelNo.EditValueChanged += sleDelNo_EditValueChanged;
        }
        private void dtpDate_EditValueChanged(object sender, EventArgs e)
        {

        }
        private void cboMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewData();
        }
        private void cboYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewData();
        }
    }
}