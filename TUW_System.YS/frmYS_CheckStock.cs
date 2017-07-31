using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Linq;
using DevExpress.XtraEditors;
using myClass;


namespace TUW_System.YS
{
    public partial class frmYS_CheckStock : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;
        public delegate void ProgressBarHandler(int minValue, int maxValue, int value, string status);
        public event ProgressBarHandler ProgressBarEvent;

        cDatabase db;
        DataTable dtScan;
        CultureInfo clinfo_th = new CultureInfo("th-TH");
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo,dtfinfo_th;
        string strSerial;//เก็บซีเรียลก่อนเซฟเพื่อใช้ในกรณีแสดง error

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmYS_CheckStock()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            //Left panel
            txtBarcode.Text = "";
            lblCarton.Text = "0";
            dtScan = new DataTable();
            DataColumn dc = new DataColumn();
            dc.ColumnName = "SERIAL";
            dc.DataType = typeof(string);
            dtScan.Columns.Add(dc);
            dc = new DataColumn();
            dc.ColumnName = "CARTON_NO";
            dc.DataType = typeof(string);
            dtScan.Columns.Add(dc);
            dc = new DataColumn();
            dc.ColumnName = "WEIGHT";
            dc.DataType = typeof(decimal);
            dtScan.Columns.Add(dc);
            gridControl1.DataSource = dtScan;
            gridView1.PopulateColumns();
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            //Right panel
            cboMonth.SelectedIndex = -1;
            cboYear.SelectedIndex = -1;
        }
        public void SaveData()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                db.ConnectionOpen();
                db.BeginTrans();
                foreach (DataRow dr in dtScan.Rows)
                {
                    strSerial = dr["SERIAL"].ToString();
                    string strSQL = "INSERT INTO YARNSTOCKBEGINDETAIL(MONTHYEAR,SERIAL)VALUES('"+cboYear.Text+cboMonth.Tag.ToString()+"','"+strSerial+"')";
                    db.Execute(strSQL);
                }
                db.CommitTrans();
                MessageBox.Show(dtScan.Rows.Count+"  carton(s).", "ํSave complete...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                for (int i = gridView1.DataRowCount-1; i >=0; i--)
                {
                    gridView1.DeleteRow(i);
                }
                lblCarton.Text = gridView1.DataRowCount.ToString();
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message+" "+strSerial, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.ConnectionClose();
            }
            //สรุปข้อมูลใน gridView2 ใหม่อีกครั้ง
            GetCarton(cboMonth.Tag.ToString(), cboYear.Text);
            this.Cursor = Cursors.Default;
        }
        public void ExportExcel()
        {
            SaveFileDialog theOpenFile = new SaveFileDialog();
            string strTemp;
            theOpenFile.Filter = "Microsoft Excel Document|*.xls";
            if (theOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                strTemp = theOpenFile.FileName;
                gridView2.ExportToXls(strTemp);
            }
        }

        private void GetCarton(string strMonth,string strYear)
        {
            string strSQL = "SELECT * FROM YARNSTOCKBEGINDETAIL WHERE MONTHYEAR='" + strYear + strMonth + "'";
            DataTable dt = db.GetDataTable(strSQL);
            lblCartonAll.Text = dt.Rows.Count.ToString();
            var weight=dt
                .AsEnumerable()
                .Where(w=>w.Field<string>("MonthYear")==strYear+strMonth)
                .Sum(x=>x.Field<decimal>("Weight"));
            lblWeightAll.Text = weight.ToString("#,0.0");

            gridControl2.DataSource = dt;
            gridView2.Columns["Register"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            gridView2.Columns["Register"].DisplayFormat.FormatString = "g";
            gridView2.Columns["Serial"].SummaryItem.FieldName = "Serial";
            gridView2.Columns["Serial"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Count, "{0:n0}");
            gridView2.Columns["Weight"].SummaryItem.FieldName = "Weight";
            gridView2.Columns["Weight"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n1}");
            gridView2.OptionsView.EnableAppearanceEvenRow = true;
            gridView2.OptionsView.EnableAppearanceOddRow = true;
            gridView2.OptionsView.ColumnAutoWidth = false;
            gridView2.BestFitColumns();

            strSQL = "SELECT * FROM YARNSTOCKBEGIN WHERE MONTHYEAR='" + strYear + strMonth + "'";
            dt = db.GetDataTable(strSQL);
            gridControl5.DataSource = dt;
            gridView5.OptionsView.EnableAppearanceEvenRow = true;
            gridView5.OptionsView.EnableAppearanceOddRow = true;
            gridView5.OptionsView.ColumnAutoWidth = false;
            gridView5.BestFitColumns();
        }
        private void CalculateCostAverage(bool allItem)
        {
            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                string monthYear, previousMonth, cost,strSQL;
                monthYear = cboYear.Text + (cboMonth.SelectedIndex + 1).ToString().PadLeft(2, '0');
                previousMonth = (new DateTime(Convert.ToInt32(monthYear.Substring(0, 4)), Convert.ToInt32(monthYear.Substring(4, 2)), 1)).AddMonths(-1).ToString("yyyyMM", dtfinfo);
                /*คำนวนคอสเฉลี่ยของด้ายดิบก่อน(baseid)**************************************************************************************************************************/
                strSQL = "select distinct BaseID from PO_PurchaseDetail where BaseID is not null and BaseID<>''";
                DataTable dt = db.GetDataTable(strSQL);
                int count = 0;
                ProgressBarEvent(0, dt.Rows.Count, count, "Initialize");
                foreach (DataRow dr in dt.Rows)
                {
                    strSQL = "exec sptuwsystem_ys_costaverage '" + previousMonth + "','" + dr["BaseID"].ToString() + "'";
                    cost = db.ExecuteFirstValue(strSQL);
                    if (cost.Length > 0 && Convert.ToDecimal(cost) > 0)
                    {
                        strSQL = "update yarnstockbegin set cost=" + cost + " where monthyear='" + monthYear + "' and yarnid=" + dr["BaseID"].ToString();
                        db.Execute(strSQL);
                    }
                    count += 1;
                    StatusBarEvent(count.ToString() + "/" + dt.Rows.Count);
                    ProgressBarEvent(0, dt.Rows.Count, count, "Update");
                }
                /*คำนวนคอสเฉลี่ยของเส้นด้ายทั้งหมด**************************************************************************************************************************/
                if (allItem)
                    strSQL = "select yarnid from yarnstockbegin where monthyear='" + monthYear + "'";
                else
                    //strSQL = "select yarnid from yarnstockbegin where monthyear='" + monthYear + "' and weight>0";
                    strSQL = "select yarnid from yarnstockbegin where monthyear='"+monthYear+"' and weight>0 union "+
                        "select distinct b.YarnID  from YarnReceive a inner join  YarnReceiveDetail b on a.RecNo=b.RecNo where CONVERT(char(6),a.RecDate,112)='"+previousMonth+"'";
                dt = db.GetDataTable(strSQL);
                count = 0;
                ProgressBarEvent(0, dt.Rows.Count, count, "Initialize");
                foreach (DataRow dr in dt.Rows)
                {
                    strSQL = "exec sptuwsystem_ys_costaverage '" + previousMonth + "','" + dr["yarnid"].ToString() + "'";
                    cost = db.ExecuteFirstValue(strSQL);
                    if (cost.Length > 0 && Convert.ToDecimal(cost) > 0)
                    {
                        strSQL = "update yarnstockbegin set cost=" + cost + " where monthyear='" + monthYear + "' and yarnid=" + dr["yarnid"].ToString();
                        db.Execute(strSQL);
                    }
                    count += 1;
                    StatusBarEvent(count.ToString() + "/" + dt.Rows.Count);
                    ProgressBarEvent(0, dt.Rows.Count, count, "Update");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
        }
        private void GetBarcodeHistory()
        {
            string strSQL = "SELECT * FROM YARNGENBARCODE WHERE SERIAL='"+txtSerial.Text+"'";
            DataTable dt = db.GetDataTable(strSQL);
            gridControl6.DataSource = dt;
            gridView6.OptionsView.EnableAppearanceEvenRow = true;
            gridView6.OptionsView.EnableAppearanceOddRow = true;
            gridView6.OptionsView.ColumnAutoWidth = false;
            gridView6.BestFitColumns();

            strSQL = "SELECT A.MONTHYEAR,A.WEIGHT,A.REGISTER,B.COST FROM YARNSTOCKBEGINDETAIL A LEFT OUTER JOIN YARNSTOCKBEGIN B " +
                "ON A.MONTHYEAR=B.MONTHYEAR AND A.YARNID=B.YARNID " +
                "WHERE A.SERIAL='" + txtSerial.Text + "' ORDER BY A.MONTHYEAR DESC";
            dt = db.GetDataTable(strSQL);
            gridControl7.DataSource = dt;
            gridView7.OptionsView.EnableAppearanceEvenRow = true;
            gridView7.OptionsView.EnableAppearanceOddRow = true;
            gridView7.OptionsView.ColumnAutoWidth = false;
            gridView7.BestFitColumns();

            strSQL = "SELECT A.*,B.* FROM YARNRECEIVEDETAIL A LEFT OUTER JOIN YARNRECEIVE B ON A.RECNO=B.RECNO WHERE A.YARNSERIAL='"+txtSerial.Text+"'";
            dt = db.GetDataTable(strSQL);
            gridControl8.DataSource = dt;
            gridView8.OptionsView.EnableAppearanceEvenRow = true;
            gridView8.OptionsView.EnableAppearanceOddRow = true;
            gridView8.OptionsView.ColumnAutoWidth = false;
            gridView8.BestFitColumns();

            strSQL = "SELECT A.*,B.* FROM YARNRETURNDETAIL A LEFT OUTER JOIN YARNRETURN B ON A.RETNO=B.RETNO WHERE A.YARNSERIAL='" + txtSerial.Text + "'";
            dt = db.GetDataTable(strSQL);
            gridControl9.DataSource = dt;
            gridView9.OptionsView.EnableAppearanceEvenRow = true;
            gridView9.OptionsView.EnableAppearanceOddRow = true;
            gridView9.OptionsView.ColumnAutoWidth = false;
            gridView9.BestFitColumns();

            strSQL = "SELECT A.*,B.* FROM YARNISSUEDETAIL A LEFT OUTER JOIN YARNISSUE B ON A.ISSNO=B.ISSNO WHERE A.SERIAL='" + txtSerial.Text + "'";
            dt = db.GetDataTable(strSQL);
            gridControl10.DataSource = dt;
            gridView10.OptionsView.EnableAppearanceEvenRow = true;
            gridView10.OptionsView.EnableAppearanceOddRow = true;
            gridView10.OptionsView.ColumnAutoWidth = false;
            gridView10.BestFitColumns();

        }

        private void frmTS5_YarnCheckStock_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            txtBarcode.Properties.MaxLength = 8;
            cboMonth.Properties.TextEditStyle=DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboYear.Properties.TextEditStyle=DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
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
            NewData();

        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            gridView1.IndicatorWidth = 30;
        }
        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;

            }
            gridView2.IndicatorWidth = 40;
        }
        private void gridView3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;

            }
            gridView3.IndicatorWidth = 40;
        }
        private void gridView4_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;

            }
            gridView4.IndicatorWidth = 40;
        }
        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)13) { return; }
            txtBarcode.Text = txtBarcode.Text.ToUpper();
            try
            {
                //ป้องกันการยิงซ้ำ   
                foreach (DataRow dr in dtScan.Rows)
                    if (Equals(txtBarcode.Text, dr["SERIAL"].ToString())) { throw new ApplicationException("Duplicate"); }
                //เช็คว่ากล่องนี้มีอยู่ในเอกสารการรับเข้าหรือไม่ ใช้ในกรณีสร้างบาร์โค๊ดแต่ลืมเซฟเอกสาร
                db.ConnectionOpen();
                string strSQL = "SELECT COUNT(ID) AS TOTAL FROM YARNRECEIVEDETAIL WHERE YARNSERIAL='"+txtBarcode.Text+"';"+
                    "SELECT COUNT(ID) AS TOTAL FROM YARNRETURNDETAIL WHERE YARNSERIAL='"+txtBarcode.Text+"';"+
                    "SELECT CTNNO,NETWEIGHT,SYSDELETE FROM YARNGENBARCODE WHERE SERIAL='" + txtBarcode.Text + "'";
                DataSet ds = db.GetDataSet(strSQL);
                if (ds.Tables[0].Rows[0]["TOTAL"].ToString() == "0" && ds.Tables[1].Rows[0]["TOTAL"].ToString() == "0")
                    throw new ApplicationException("NoBarcodeInDoc");
                //เช็คว่ากล่องนี้ยังมีอยู่ในสต็อกหรือไม่
                if (ds.Tables[2] == null || ds.Tables[2].Rows.Count == 0)
                    throw new ApplicationException("NoBarcode");
                else
                {
                    if (ds.Tables[2].Rows[0]["SYSDELETE"].ToString() == "1")
                        throw new ApplicationException("CartonOut");
                    else
                    {
                        DataRow drNew = dtScan.NewRow();
                        drNew["SERIAL"] = txtBarcode.Text;
                        drNew["CARTON_NO"] = ds.Tables[2].Rows[0]["CTNNO"];
                        drNew["WEIGHT"] = ds.Tables[2].Rows[0]["NETWEIGHT"];
                        dtScan.Rows.Add(drNew);
                        gridControl1.DataSource = dtScan;
                        gridView1.Columns["WEIGHT"].SummaryItem.FieldName = "WEIGHT";
                        gridView1.Columns["WEIGHT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n1}");
                        lblCarton.Text = gridView1.DataRowCount.ToString();
                    }
                }
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                switch (ex.Message)
                { 
                    case "Duplicate":
                        break;
                    case "CartonOut":
                        MessageBox.Show("เส้นด้ายกล่องนี้ได้ทำการถูกยิงออกจากสต็อกแล้ว","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        break;
                    case "NoBarcodeInDoc":
                        MessageBox.Show("ไม่พบซีเรียลนี้ในเอกสารการรับเข้าหรือรับคืน", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "NoBarcode":
                        MessageBox.Show("ไม่พบซีเรียลนี้ในประวัติการทำบาร์โค๊ด","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        break;
                }
            }
            db.ConnectionClose();
            txtBarcode.Text = "";
        }
        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && !gridView1.IsEditing)
            {
                if (MessageBox.Show("คุณต้องการลบ serial: " + gridView1.GetFocusedRowCellDisplayText("SERIAL") + " หรือไม่", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    gridView1.DeleteSelectedRows();
                    lblCarton.Text = gridView1.DataRowCount.ToString();
                }
            }        
        }
        private void gridControl2_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete && !gridView2.IsEditing)
                {
                    if (MessageBox.Show("คุณต้องการลบ serial: " + gridView2.GetFocusedRowCellDisplayText("Serial") + " หรือไม่", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        db.ConnectionOpen();
                        db.BeginTrans();
                        cboMonth.Tag = (cboMonth.SelectedIndex + 1).ToString().PadLeft(2, '0');
                        string strSQL="DELETE FROM YARNSTOCKBEGINDETAIL WHERE MONTHYEAR='"+cboYear.Text+(cboMonth.SelectedIndex+1).ToString().PadLeft(2,'0')+"'"+
                            " AND SERIAL='"+gridView2.GetFocusedRowCellDisplayText("Serial")+"'";
                        db.Execute(strSQL);
                        gridView2.DeleteSelectedRows();
                        lblCartonAll.Text =gridView2.DataRowCount.ToString();
                        decimal decWeight = 0;
                        for (int i = 0; i < gridView2.DataRowCount; i++)
                        {
                            decWeight += (decimal)gridView2.GetRowCellValue(i, "Weight");
                        }
                        lblWeightAll.Text = decWeight.ToString("#,0.0");
                        db.CommitTrans();
                    }
                }
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
        }
        private void cboMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            try 
	        {	        
		        cboMonth.Tag=(cboMonth.SelectedIndex+1).ToString().PadLeft(2,'0');
                GetCarton(cboMonth.Tag.ToString(),cboYear.Text);
	        }
	        catch{}
        }
        private void cboYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            try 
	        {	        
		        GetCarton(cboMonth.Tag.ToString(),cboYear.Text);
	        }
	        catch{}
        }
        private void btnTransfer_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string monthYear;
            string previousMonth;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                monthYear=cboYear.Text+(cboMonth.SelectedIndex+1).ToString().PadLeft(2,'0');
                previousMonth = (new DateTime(Convert.ToInt32(monthYear.Substring(0, 4)), Convert.ToInt32(monthYear.Substring(4, 2)), 1)).AddMonths(-1).ToString("yyyyMM", dtfinfo);
                string strSQL = "INSERT INTO YARNSTOCKBEGIN " +
                    "SELECT '" + monthYear + "',ID,0,COST,0 FROM YARNCODE WHERE SYSDELETE=0 AND ID NOT IN (SELECT YARNID FROM YARNSTOCKBEGIN WHERE MONTHYEAR='" + monthYear + "')";
                db.Execute(strSQL);
                //******************update yarnid,weight table yarnstockbegindetail***********************************
                db.Execute("update a set a.yarnid=b.yarnid,a.weight=b.netweight from yarnstockbegindetail a inner join yarngenbarcode b on a.serial=b.serial where a.monthyear='"+monthYear+"'");
                //******************update table yarnstockbegin
                db.Execute("update yarnstockbegin set weight=0,carton=0 where monthyear='"+monthYear+"'");//กำหนดให้เป็น 0 ทุกตัวก่อน
                db.Execute("update yarnstockbegin  set weight=(	select isnull(sum(weight),0)from yarnstockbegindetail  where monthyear='"+monthYear+"' and yarnid=yarnstockbegin.yarnid) where monthyear='"+monthYear+"'");
                db.Execute("update a set a.carton=b.carton from yarnstockbegin a inner join (select yarnid,count(serial)as carton from yarnstockbegindetail where monthyear='"+monthYear+"' group by yarnid)b on a.yarnid=b.yarnid where a.monthyear='"+monthYear+"'");
                //*******************update table yarncode************************************************************
                //db.Execute("update a set a.wgt_begin=b.weight,a.wgt=b.weight from yarncode a inner join yarnstockbegin b on  a.id=b.yarnid and b.monthyear='" + monthYear + "'");
                db.CommitTrans();
                MessageBox.Show("ยกยอดเสร็จสมบูรณ์", "ยกยอด...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                db.ConnectionClose();
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            db.ConnectionClose();
            this.Cursor = Cursors.Default;
            
            CalculateCostAverage(allItem:false);
            GetCarton(cboMonth.Tag.ToString(), cboYear.Text);
        }
        private void btnCheckStock_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string nextMonthYear = cboYear.Text + (cboMonth.SelectedIndex + 1).ToString().PadLeft(2, '0');//เดือนเริ่มต้นใหม่
                string monthYear = (new DateTime(Convert.ToInt32(cboYear.Text), cboMonth.SelectedIndex + 1, 1)).AddMonths(-1).ToString("yyyyMM",clinfo);//  nextMonthYear + "01";
                string strSQL = "EXEC spTUWSystem_S5_YarnCheckStock '" + monthYear + "','" + nextMonthYear + "'";
                DataSet ds = db.GetDataSet(strSQL);
                gridControl3.DataSource = ds.Tables[0];
                gridView3.OptionsView.EnableAppearanceEvenRow = true;
                gridView3.OptionsView.EnableAppearanceOddRow = true;
                gridView3.OptionsView.ColumnAutoWidth = false;
                gridView3.BestFitColumns();
                gridControl4.DataSource = ds.Tables[1];
                gridView4.OptionsView.EnableAppearanceEvenRow = true;
                gridView4.OptionsView.EnableAppearanceOddRow = true;
                gridView4.OptionsView.ColumnAutoWidth = false;
                gridView4.BestFitColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        private void btnExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog theOpenFile = new SaveFileDialog();
            string strTemp;
            theOpenFile.Filter = "Microsoft Excel Document|*.xls";
            if (theOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                strTemp = theOpenFile.FileName;
                gridView3.ExportToXls(strTemp);
            }
        }
        private void btnExcel2_Click(object sender, EventArgs e)
        {
            SaveFileDialog theOpenFile = new SaveFileDialog();
            string strTemp;
            theOpenFile.Filter = "Microsoft Excel Document|*.xls";
            if (theOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                strTemp = theOpenFile.FileName;
                gridView4.ExportToXls(strTemp);
            }
        }
        private void btnReCalculateCost_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("การคำนวนนี้ใช้ระยะเวลานานเนื่องจากต้องคำนวนทุกไอเท็ม ท่าต้องการคำนวนหรือไม่","Calculate Cost",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)            CalculateCostAverage(allItem: true);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                GetBarcodeHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                throw;
            }
        }
        private void txtSerial_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(char)13) GetBarcodeHistory();
        }

    }
}