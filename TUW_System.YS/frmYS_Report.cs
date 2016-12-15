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
    public partial class frmYS_Report : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        string strType;
        string statusType;//In,Out,Return,In All,Out All

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        public LogIn User_Login { get; set; }

        public frmYS_Report()
        {
            InitializeComponent();
        }
        public void NewData()
        { 
            cboType.Text = "";
            dtpDate.EditValue=DateTime.Today;
            checkEdit1.Checked = false;
            checkEdit2.Checked = false;
            gridControl1.DataSource=null;
        }
        public void DisplayData()
        {
            this.Cursor=Cursors.WaitCursor;
            try 
	        {	        
		        if(cboType.Text.Length==0) return;
                
                string strSQL="";
                GetStatusType(cboType.Text);
                strSQL = "EXEC SPTUWSYSTEM_YS_REPORT '" + statusType + "','" + strType + "'," +
                    "'" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "',";
                strSQL+=(checkEdit1.Checked)?"1,":"0,";
                strSQL+=(checkEdit2.Checked)?"1":"0";
                DataTable dt=db.GetDataTable(strSQL);
                gridControl1.DataSource=dt;
                gridView1.PopulateColumns();
                
                gridView1.Columns["YARNID"].Caption="Yarn ID";
                gridView1.Columns["CODE"].Caption="Yarn Code";
                gridView1.Columns["CARTON"].Caption="Ctn.";
                gridView1.Columns["WEIGHT"].Caption="Kgs.";
                gridView1.Columns["REMARK"].Caption="Remark";

                gridView1.Columns["CARTON"].DisplayFormat.FormatType=DevExpress.Utils.FormatType.Numeric;
                gridView1.Columns["CARTON"].DisplayFormat.FormatString="n0";
                gridView1.Columns["WEIGHT"].DisplayFormat.FormatType=DevExpress.Utils.FormatType.Numeric;
                gridView1.Columns["WEIGHT"].DisplayFormat.FormatString="n2";

                gridView1.Columns["CARTON"].SummaryItem.FieldName="CARTON";
                gridView1.Columns["CARTON"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum,"{0:n0}");
                gridView1.Columns["WEIGHT"].SummaryItem.FieldName="WEIGHT";
                gridView1.Columns["WEIGHT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum,"{0:n2}");

                gridView1.OptionsBehavior.ReadOnly=true;
                gridView1.OptionsView.EnableAppearanceEvenRow=true;
                gridView1.OptionsView.EnableAppearanceOddRow=true;
                gridView1.OptionsView.ColumnAutoWidth=false;
                gridView1.BestFitColumns();
            }
	        catch (Exception ex)
	        {
		        MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
	        }
            finally
            {
                this.Cursor=Cursors.Default;
            }
        }
        public void PrintPreview()
        {
            try
            {
                cCrystalReport crpIssue = new cCrystalReport(Application.StartupPath + @"\Report\YS_Report.rpt");
                if (crpIssue.SetPrinter() == false) return;
                if (statusType == "In All")
                    crpIssue.ReportTitle = "ใบสรุปการรับเส้นด้ายประจำวัน";
                else
                    if (statusType == "Out All")
                        crpIssue.ReportTitle = "ใบสรุปการเบิกเส้นด้ายประจำวัน";
                    else
                        crpIssue.ReportTitle = "ใบสรุป" + strType + "ประจำวัน";
                crpIssue.SetParameter("@statusType", statusType);
                crpIssue.SetParameter("@strType", strType);
                crpIssue.SetParameter("@strType_thai", cboType.Text);
                crpIssue.SetParameter("@date", ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd", dtfinfo));
                crpIssue.SetParameter("@notIncludeEndOfMonth", checkEdit1.Checked);
                crpIssue.SetParameter("@isOrganic",checkEdit2.Checked);
                string fmlText = "";// "{CALL PurchaseOrder.dbo.spTUWSystem_YS_Report;1}";
                crpIssue.ReportSize = cCrystalReport.Paper.Letter;
                crpIssue.PrintReport(fmlText, false, "sa", "ZAQ113m4tuw");
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
                cCrystalReport crpIssue = new cCrystalReport(Application.StartupPath + @"\Report\YS_Report.rpt");
                if (crpIssue.SetPrinter() == false) return;
                if (statusType == "In All")
                    crpIssue.ReportTitle = "ใบสรุปการรับเส้นด้ายประจำวัน";
                else
                    if (statusType == "Out All")
                        crpIssue.ReportTitle = "ใบสรุปการเบิกเส้นด้ายประจำวัน";
                    else
                        crpIssue.ReportTitle = "ใบสรุป" + strType + "ประจำวัน";
                crpIssue.SetParameter("@statusType", statusType);
                crpIssue.SetParameter("@strType", strType);
                crpIssue.SetParameter("@strType_thai", cboType.Text);
                crpIssue.SetParameter("@date", ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd", dtfinfo));
                crpIssue.SetParameter("@notIncludeEndOfMonth", checkEdit1.Checked);
                crpIssue.SetParameter("@isOrganic", checkEdit2.Checked);
                string fmlText = "";// "{CALL PurchaseOrder.dbo.spTUWSystem_YS_Report;1}";
                crpIssue.ReportSize = cCrystalReport.Paper.Letter;
                crpIssue.PrintReport(fmlText, true, "sa", "ZAQ113m4tuw");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetStatusType(string strType_thai)
        {
            switch (cboType.Text)
            {
                case "เบิกเพื่อทอ":
                    strType = "Issue for Knitting";
                    statusType = "Out";
                    break;
                case "เบิกเพื่อทำตัวอย่าง":
                    strType = "Issue for Sample";
                    statusType = "Out";
                    break;
                case "เบิกชดเชย":
                    strType = "Issue for Reparation";
                    statusType = "Out";
                    break;
                case "เบิกเพื่อส่งคืน":
                    strType = "Issue for Return";
                    statusType = "Out";
                    break;
                case "เตรียมขาย & ย้าย Stock":
                    strType = "Adjust Stock";
                    statusType = "Out";
                    break;
                case "จ้างทอ":
                    strType = "Subcontract Knit";
                    statusType = "Out";
                    break;
                case "จ้างย้อม":
                    strType = "Subcontract Dye";
                    statusType = "Out";
                    break;
                case "จ้างกรอด้าย":
                    strType = "Subcontract Wind";
                    statusType = "Out";
                    break;
                case "จ้างทอตัวอย่าง":
                    strType = "Subcontract Knit Sample";
                    statusType = "Out";
                    break;
                case "รับจากซื้อภายในประเทศ":
                    strType = "Local";
                    statusType = "In";
                    break;
                case "รับจากซื้อต่างประเทศ":
                    strType = "Import";
                    statusType = "In";
                    break;
                case "รับจากการจ้างย้อม":
                case "รับจากการจ้างกรอ":
                    strType = "Sub Contract";
                    statusType = "In";
                    break;
                case "รับคืนจากโรงทอ":
                    strType = "Return from Knitting";
                    statusType = "Return";
                    break;
                case "รับคืนจากจ้างทอ":
                    strType = "Return from Subcontract";
                    statusType = "Return";
                    break;
                case "รับคืนจากจ้างย้อม":
                case "รับคืนจากจ้างกรอ":
                    strType = "Return from Subcontract";
                    statusType = "Return";
                    break;
                case "รับคืนจากทำตัวอย่าง":
                    strType = "Return from Sample";
                    statusType = "Return";
                    break;
                case "รับสินค้าตัวอย่าง":
                    strType = "Receive for Sample";
                    statusType = "In";
                    break;
                case "รับคืนจาก Supplier":
                    strType = "Return from Vender";
                    statusType = "Return";
                    break;
                case "เบิกทั้งหมด":
                    statusType = "Out All";
                    break;
                case "รับทั้งหมด":
                    statusType = "In All";
                    break;
            }
        }

        private void frmYS_Report_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            NewData();
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            var grid = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            grid.IndicatorWidth = 40;
        }
    
    }
}