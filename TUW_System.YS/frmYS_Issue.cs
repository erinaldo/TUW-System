using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using myClass;
using DevExpress.XtraEditors;

namespace TUW_System.YS
{
    public partial class frmYS_Issue : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;

        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        CultureInfo clinfo_th = new CultureInfo("th-TH");
        DateTimeFormatInfo dtfinfo,dtfinfo_th;
        DataTable dtIssNo,dtIssDetail;

        private const decimal lbs = 2.2046M;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        public LogIn User_Login { get; set; }

        public frmYS_Issue()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            try
            {
                cboType.SelectedIndexChanged -= cboType_SelectedIndexChanged;
                sleIssNo.EditValueChanged -= sleIssNo_EditValueChanged;

                cboType.Text = "";
                sleIssNo.EditValue = null;
                ClearData();
                sleIssNo.Properties.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
            cboType.SelectedIndexChanged += cboType_SelectedIndexChanged;
            sleIssNo.EditValueChanged += sleIssNo_EditValueChanged;
        }
        public void SaveData()
        { 
            this.Cursor=Cursors.WaitCursor;
            try 
	        {	     
                if(cboType.Text.Length==0) return;
                if(sleIssNo.Text.Length!=11) throw new ApplicationException("เลขที่เอกสารไม่ถูกต้อง");
                db.ConnectionOpen();
                db.BeginTrans();
                //SaveYarnIssue
                string strType="";
                switch(cboType.Text)
                {
                    case "เบิกเพื่อทอ":
                        strType = "Issue for Knitting";
                        break;
                    case "เบิกเพื่อทำตัวอย่าง":
                        strType = "Issue for Sample";
                        break;
                    case "เบิกชดเชย":
                        strType = "Issue for Reparation";
                        break;
                    case "เบิกเพื่อส่งคืน":
                        strType = "Issue for Return";
                        break;
                    case "เตรียมขาย & ย้าย Stock":
                        strType = "Adjust Stock";
                        break;
                }
    
                string strSOL = "SELECT COUNT(IssNo) FROM YarnIssue WHERE IssNo = '"+sleIssNo.Text+ "'";
                if(db.ExecuteFirstValue(strSOL)=="0")
                {
                    strSOL = "INSERT INTO YarnIssue (IssRun,IssNo,IssDate,IssType,PoNo_LotNo,RequestBy,IssueBy,Remark)"+
                        "VALUES("+sleIssNo.Text.Substring(5,3)+",'"+sleIssNo.Text+"','"+
                        ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd",dtfinfo)+"',N'"+strType+"',"+
                        "'"+ txtLot.Text + "','" + txtUIss.Text + "','" + txtIss.Text + "',N'" + txtRemark.Text + "')";
                    db.Execute(strSOL);
                }
                else
                {
                    strSOL = "UPDATE YarnIssue SET PoNo_LotNo = '"+ txtLot.Text + "',RequestBy = '" + txtUIss.Text + "'," +
                        "IssueBy = '"+ txtIss.Text+"',Remark = N'"+txtRemark.Text+"' "+
                        "WHERE IssNo = '"+sleIssNo.Text+"'";
                    db.Execute(strSOL);
                }
                //SaveYarnIssueDetail
                int count = 0;
                for(int i=0;i<gridView1.DataRowCount;i++)
                {
                    strSOL = "SELECT COUNT(Serial) FROM YarnIssueDetail " +
                        "WHERE Serial='" + gridView1.GetRowCellDisplayText(i, "SERIAL") + "'";
                    if (db.ExecuteFirstValue(strSOL) == "0")
                        count += 1;
                    else
                    {
                        strSOL = "DELETE YarnIssueDetail WHERE IssNo = '" + sleIssNo.Text + "' " +
                        "and Serial='" + gridView1.GetRowCellDisplayText(i, "SERIAL") + "'";
                        db.Execute(strSOL);
                    }
                    strSOL = "INSERT INTO YarnIssueDetail (IssNo,YarnID,Serial,Code,NetWeight,LotNo)VALUES(" +
                            "'" + sleIssNo.Text + "','" + gridView1.GetRowCellDisplayText(i, "SERIAL").Substring(0, 4) + "'," +
                            "'" + gridView1.GetRowCellDisplayText(i, "SERIAL") + "'," +
                            "'" + gridView1.GetRowCellDisplayText(i, "CODE") + "'," +
                            "'" + gridView1.GetRowCellValue(i, "NETWEIGHT") + "'," +
                            "N'" + gridView1.GetRowCellDisplayText(i, "LOTNO") + "')";
                    db.Execute(strSOL);
                    UpdateStock("Insert", gridView1.GetRowCellDisplayText(i, "SERIAL"));
                }
                //Call SaveYarnStock
                db.CommitTrans();
                lblCtn.Text = count.ToString();
                MessageBox.Show("บันทึกข้อมูลเลขที่ " + sleIssNo.Text + " เรียบร้อย", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
	        }
            catch(ApplicationException ex)
            {
                MessageBox.Show(ex.Message,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
	        catch (SystemException ex)
	        {
		        db.RollbackTrans();
                MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
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
                crpIssue.ReportTitle = sleIssNo.Text;
                for (int i = 1; i <= crpIssue.ReportCopy; i++)
                {
                    string fmlText = "{YARNISSUEDETAIL.ISSNO}='" + sleIssNo.Text + "'";
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
                crpIssue.ReportTitle = sleIssNo.Text;
                for (int i = 1; i <= crpIssue.ReportCopy; i++)
                {
                    string fmlText = "{YARNISSUEDETAIL.ISSNO}='" + sleIssNo.Text + "'";
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
            txtLot.Text = "";
            txtUIss.Text = "";
            txtIss.Text = "";
            txtRemark.Text = "";
            lblTCarton.Text = "";
            lblTNweight.Text = "";
            lblCtn.Text = "";
            txtBarcode.Text = "";
            gridControl1.DataSource = null;
            dtIssDetail = new DataTable();
            DataColumn dc = new DataColumn();
            dc.Caption = "Delete";
            dc.ColumnName = "DEL";
            dc.DataType = typeof(bool);
            dc.DefaultValue = false;
            dtIssDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Yarn Code";
            dc.ColumnName = "CODE";
            dc.DataType = typeof(string);
            dtIssDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Color";
            dc.ColumnName = "COLOR";
            dc.DataType = typeof(string);
            dtIssDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Special";
            dc.ColumnName = "SPECIAL";
            dc.DataType = typeof(string);
            dtIssDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Supplier";
            dc.ColumnName = "SUPPLIER";
            dc.DataType = typeof(string);
            dtIssDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Ctn No.";
            dc.ColumnName = "CTNNO";
            dc.DataType = typeof(int);
            dtIssDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Net Weight";
            dc.ColumnName = "NETWEIGHT";
            dc.DataType = typeof(decimal);
            dtIssDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Serial";
            dc.ColumnName = "SERIAL";
            dc.DataType = typeof(string);
            dtIssDetail.Columns.Add(dc);
            dc = new DataColumn();
            dc.Caption = "Lot No.";
            dc.ColumnName = "LOTNO";
            dc.DataType = typeof(string);
            dtIssDetail.Columns.Add(dc);
            gridControl1.DataSource = dtIssDetail;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
        }
        private DataTable GetIssNo(string strType,DateTime datSearch)
        {
            string strSQL="";
            switch(strType)
            {
                case "เบิกเพื่อทอ":
                    strSQL = "SELECT ISSNO FROM YarnIssue WHERE IssNo like 'IK%' "+
                        "and CONVERT(char(7), IssDate, 120) = '"+ datSearch.ToString("yyyy-MM",dtfinfo)+ "' ORDER BY IssNo";
                        break;
                case "เบิกเพื่อทำตัวอย่าง":
                    strSQL = "SELECT ISSNO FROM YarnIssue WHERE IssNo like 'IS%' "+
                        "and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo)+ "' ORDER BY IssNo";
                        break;
                case "เบิกชดเชย":
                case "เบิกเพื่อส่งคืน":
                    strSQL = "SELECT ISSNO FROM YarnIssue WHERE IssNo like 'IR%' "+
                        "and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo)+ "' ORDER BY IssNo";
                        break;
                case "เตรียมขาย & ย้าย Stock":
                    strSQL = "SELECT ISSNO FROM YarnIssue WHERE IssNo like 'AJ%' "+
                        "and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo)+"' ORDER BY IssNo";
                        break;
            }
            dtIssNo=db.GetDataTable(strSQL);
            return dtIssNo;
        }
        private string RunIssNoNew(string strIssType,DateTime datSearch)
        { 
            string strSQL="";
            string strType="";
            switch(strIssType)
            {
                case "เบิกเพื่อทอ":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(IssRun)+1,1))),3) as IssRun FROM YarnIssue WHERE IssNo like 'IK%' and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo) + "'";
                    strType = "IK";
                    break;
                case "เบิกเพื่อทำตัวอย่าง":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(IssRun)+1,1))),3) as IssRun FROM YarnIssue WHERE IssNo like 'IS%' and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo) + "'";
                    strType = "IS";
                    break;
                case "เบิกชดเชย":
                case "เบิกเพื่อส่งคืน":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(IssRun)+1,1))),3) as IssRun FROM YarnIssue WHERE IssNo like 'IR%' and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo) + "'";
                    strType = "IR";
                    break;
                case "เตรียมขาย & ย้าย Stock":
                    strSQL = "SELECT right('000'+ltrim(str(isnull(Max(IssRun)+1,1))),3) as IssRun FROM YarnIssue WHERE IssNo like 'AJ%' and CONVERT(char(7), IssDate, 120) = '"+datSearch.ToString("yyyy-MM",dtfinfo) + "'";
                    strType = "AJ";
                    break;
            }
            db.ConnectionOpen();
            string strNo = db.ExecuteFirstValue(strSQL);
            string strIssRun = strType + datSearch.ToString("MM", dtfinfo) + "-" + strNo + "/" + datSearch.ToString("yy", dtfinfo);
            db.ConnectionClose();
            return strIssRun;
        }
        private void GetIssueDetail(string strIssNo,string strType)
        { 
            switch(strType)
            {
                case "เบิกเพื่อทอ":
                    strType = "Issue for Knitting";
                    break;
                case "เบิกเพื่อทำตัวอย่าง":
                    strType = "Issue for Sample";
                    break;
                case "เบิกชดเชย":
                    strType = "Issue for Reparation";
                    break;
                case "เบิกเพื่อส่งคืน":
                    strType = "Issue for Return";
                    break;
                case "เตรียมขาย & ย้าย Stock":
                    strType = "Adjust Stock";
                    break;
            }
            string strSQL = "SELECT ISSRUN,ISSNO,ISSDATE,ISSTYPE,PONO_LOTNO,REQUESTBY,ISSUEBY,"+
                "REMARK FROM YarnIssue"+
                " WHERE IssNo = '"+ strIssNo+ "' AND IssType = N'"+ strType + "'";
            DataTable dt=db.GetDataTable(strSQL);
            foreach(DataRow dr in dt.Rows)
            {
                dtpDate.EditValue = dr["ISSDATE"];
                txtLot.Text =dr["PONO_LOTNO"].ToString();
                txtUIss.Text =dr["REQUESTBY"].ToString();
                txtIss.Text = dr["ISSUEBY"].ToString();
                txtRemark.Text=dr["REMARK"].ToString();
            }
            strSQL = "SELECT CONVERT(BIT,0) as DEL, A.CODE,C.COLOR,C.SPECIAL,C.SUPPLIER,B.CTNNO,"+
                "A.NETWEIGHT,A.SERIAL,A.LOTNO FROM YarnIssueDetail A "+
                "LEFT OUTER JOIN YARNGENBARCODE B ON A.SERIAL=B.SERIAL "+
                "LEFT OUTER JOIN YARNCODE C ON A.YARNID=C.ID "+
                "WHERE A.IssNo = '"+strIssNo + "' ORDER BY A.ID";
            dtIssDetail=db.GetDataTable(strSQL);
            gridControl1.DataSource = dtIssDetail;
            gridView1.PopulateColumns();
            gridView1.Columns["DEL"].Caption = "Delete";
            gridView1.Columns["CODE"].Caption="Yarn Code";
            gridView1.Columns["COLOR"].Caption="Color";
            gridView1.Columns["SPECIAL"].Caption="Special";
            gridView1.Columns["SUPPLIER"].Caption="Supplier";
            gridView1.Columns["CTNNO"].Caption="Ctn No.";
            gridView1.Columns["NETWEIGHT"].Caption="Net Weight";
            gridView1.Columns["SERIAL"].Caption="Yarn Serial";
            gridView1.Columns["LOTNO"].Caption="Lot No.";

            gridView1.Columns["NETWEIGHT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["NETWEIGHT"].DisplayFormat.FormatString = "n2";
            
            gridView1.OptionsView.EnableAppearanceEvenRow=true;
            gridView1.OptionsView.EnableAppearanceOddRow=true;
            gridView1.OptionsView.ColumnAutoWidth=false;
            gridView1.BestFitColumns();

        }
        private void SumTotal()
        {
            decimal decWeight=0;
            for(int i=0;i<gridView1.DataRowCount;i++)
            {
                decWeight+=(decimal)gridView1.GetRowCellValue(i,"NETWEIGHT");
            }
            lblTCarton.Text = gridView1.DataRowCount.ToString();
            lblTNweight.Text = decWeight.ToString("#,0.00");
        }
        private string GetEmployee(string strID)
        {
            string strSQL="SELECT Name FROM BarcodeText WHERE Code = '"+strID+"'";
            return db.ExecuteFirstValue(strSQL);
        }
        private void UpdateStock(string operate, string serial)
        { 
            string strSQL="";
            if(operate=="Insert")
                strSQL="UPDATE YARNGENBARCODE SET SYSDELETE=1,OUTDATE=GETDATE() WHERE SERIAL='"+serial+"'";
            else
                strSQL="UPDATE YARNGENBARCODE SET SYSDELETE=0,OUTDATE=NULL WHERE SERIAL='"+serial+"'";
            db.Execute(strSQL);
        }

        private void frmYS_Issue_Load(object sender, EventArgs e)
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
                sleIssNo.EditValueChanged -= sleIssNo_EditValueChanged;
                sleIssNo.EditValue = null;
                sleIssNo.Properties.DataSource = null;
                ClearData();
                sleIssNo.Properties.DataSource=GetIssNo(cboType.Text,new DateTime(Convert.ToInt32(cboYear.Text),cboMonth.SelectedIndex+1,1));
                sleIssNo.Properties.DisplayMember = "ISSNO";
                sleIssNo.Properties.ValueMember = "ISSNO";
                string strIssNoNew = RunIssNoNew(cboType.Text, new DateTime(Convert.ToInt32(cboYear.Text), cboMonth.SelectedIndex + 1, 1));
                int monthYear = Convert.ToInt32(cboYear.Text + (cboMonth.SelectedIndex + 1).ToString().PadLeft(2,'0'));
                if (monthYear < Convert.ToInt32(DateTime.Today.ToString("yyyyMM", dtfinfo)))
                    dtpDate.EditValue = new DateTime(Convert.ToInt32(cboYear.Text),cboMonth.SelectedIndex+1,DateTime.DaysInMonth(Convert.ToInt32(cboYear.Text),cboMonth.SelectedIndex+1));
                else if (monthYear > Convert.ToInt32(DateTime.Today.ToString("yyyyMM", dtfinfo)))
                    dtpDate.EditValue = new DateTime(Convert.ToInt32(cboYear.Text),cboMonth.SelectedIndex+1,1);
                else
                    dtpDate.EditValue = DateTime.Today;
                dtIssNo.BeginInit();
                DataRow dr = dtIssNo.NewRow();
                dr["ISSNO"] = strIssNoNew;
                dtIssNo.Rows.Add(dr);
                dtIssNo.EndInit();
                sleIssNo.EditValue = strIssNoNew;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            sleIssNo.EditValueChanged += sleIssNo_EditValueChanged;
        }
        private void sleIssNo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ClearData();
                GetIssueDetail(sleIssNo.Text, cboType.Text);
                SumTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    sleIssNo.EditValueChanged -= sleIssNo_EditValueChanged;
            //    ClearData();
            //    sleIssNo.Properties.DataSource = GetIssNo(cboType.Text, (DateTime)dtpDate.EditValue);
            //    sleIssNo.Properties.DisplayMember = "ISSNO";
            //    sleIssNo.Properties.ValueMember = "ISSNO";
            //    string strIssNoNew = RunIssNoNew(cboType.Text, new DateTime(Convert.ToInt32(cboYear.Text), cboMonth.SelectedIndex + 1, 1));
            //    dtIssNo.BeginInit();
            //    DataRow dr = dtIssNo.NewRow();
            //    dr["ISSNO"] = strIssNoNew;
            //    dtIssNo.Rows.Add(dr);
            //    dtIssNo.EndInit();
            //    sleIssNo.EditValue = strIssNoNew;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //sleIssNo.EditValueChanged += sleIssNo_EditValueChanged;
        }
        private void btnDelRow_Click(object sender, EventArgs e)
        {
            try 
	        {
                db.ConnectionOpen();
                db.BeginTrans();
                for(int i=gridView1.DataRowCount-1;i>=0;i--)
                {
                    if((bool)gridView1.GetRowCellValue(i,"DEL")==false) continue;
                    if(MessageBox.Show("คุณต้องการที่จะลบข้อมูลเส้นด้าย "+gridView1.GetRowCellDisplayText(i,"CODE")+
                        Environment.NewLine+"Serial: "+gridView1.GetRowCellDisplayText(i,"SERIAL"),
                        "Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.Yes)
                    {
                        string strSQL="DELETE FROM YarnIssueDetail WHERE IssNo='"+sleIssNo.Text+"' "+
                            "AND Serial='"+gridView1.GetRowCellDisplayText(i,"SERIAL")+"'";
                        db.Execute(strSQL);
                        UpdateStock("Delete", gridView1.GetRowCellDisplayText(i, "SERIAL"));
                        gridView1.DeleteRow(i);
                    }
                }
                db.CommitTrans();
                SumTotal();
	        }
	        catch (Exception ex)
	        {
                db.RollbackTrans();
		        MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
	        }
            db.ConnectionClose();
        }
        private void txtLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(char)Keys.Return)  txtUIss.Focus();
        }
        private void txtUIss_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Return) return;
            try 
	        {	        
		        db.ConnectionOpen();
                txtUIss.Text=GetEmployee(txtUIss.Text);
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
            if (e.KeyChar != (char)Keys.Return) return;
            try 
	        {	        
		        db.ConnectionOpen();
                txtIss.Text=GetEmployee(txtIss.Text);
	        }
	        catch (Exception ex)
	        {
		        MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
	        }
            db.ConnectionClose();
            txtBarcode.Focus();
        }
        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(char)Keys.Return) txtBarcode.Focus();
        }
        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar!=(char)Keys.Return) return;
            try 
	        {	        
                //Check duplicate
                for(int i=0;i<gridView1.DataRowCount;i++)
                {
                    if (txtBarcode.Text.ToUpper() == gridView1.GetRowCellDisplayText(i, "SERIAL"))
                    {
                        txtBarcode.Text = "";
                        StatusBarEvent("");
                        return;
                    }
                }

                db.ConnectionOpen();
                if (cboType.Text.Length == 0) throw new SystemException("คุณยังไม่ได้เลือกประเภทการเบิก!");
                string strSQL="SELECT COUNT(ID) FROM YARNISSUEDETAIL WHERE SERIAL='"+txtBarcode.Text+"'";
                if(db.ExecuteFirstValue(strSQL)=="0")
                {
                    strSQL = "SELECT 0 AS DEL, A.YARNCODE AS CODE,C.COLOR,C.SPECIAL,C.SUPPLIER,A.CTNNO,A.NETWEIGHT,"+
                        "A.SERIAL,A.LOTNO "+
                        "FROM YARNGENBARCODE A LEFT OUTER JOIN YARNCODE C ON A.YARNID=C.ID " +
                        "WHERE A.SERIAL = '" +txtBarcode.Text + "' AND A.SYSDELETE=0";
                    DataTable dt = db.GetDataTable(strSQL);
                    if (dt == null || dt.Rows.Count == 0) throw new ApplicationException("Serial " + txtBarcode.Text + " ยังไม่ได้ถูกบันทึกในเอกสารรับเข้าหรือถูกปรับสต็อก");
                    dtIssDetail.BeginInit();
                    DataRow dr=dtIssDetail.NewRow();
                    dr.ItemArray = dt.Rows[0].ItemArray;
                    dtIssDetail.Rows.Add(dr);
                    dtIssDetail.EndInit();
                    gridView1.Columns["NETWEIGHT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    gridView1.Columns["NETWEIGHT"].DisplayFormat.FormatString = "n2";
                    gridView1.BestFitColumns();
                    gridView1.OptionsNavigation.AutoFocusNewRow = true;
                    SumTotal();
                    txtBarcode.Text="";
                    StatusBarEvent("");
                }
                else
                {
                    throw new ApplicationException("Serial "+txtBarcode.Text+" นี้มีการยิงออกแล้ว ? Check Stock ก่อน...มีรับคืนหรือไม่");
                }
	        }
            catch(ApplicationException ex)
            {
                StatusBarEvent(ex.Message.ToString());
                txtBarcode.Text = "";
                //MessageBox.Show(ex.Message,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
	        catch (SystemException ex)
	        {
		        MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
	        }
            db.ConnectionClose();   

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