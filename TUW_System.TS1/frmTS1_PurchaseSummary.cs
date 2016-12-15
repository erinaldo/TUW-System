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


namespace TUW_System.TS1
{
    public partial class frmTS1_PurchaseSummary : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        CultureInfo clinfo=new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        cCrystalReport ctr;

        private string _connectionString;
        private DevExpress.XtraBars.BarStaticItem _bsiStatusBar;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        public DevExpress.XtraBars.BarStaticItem bsiStatusbar
        {
            get { return _bsiStatusBar; }
            set { _bsiStatusBar = value; }
        }

        public frmTS1_PurchaseSummary()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            chkSelect.Checked = false;
            chkFreeze.Checked = false;
            chkReprint.Checked = false;
            gridControl1.DataSource = null;
            gridView1.PopulateColumns();
            _bsiStatusBar.Caption = "";
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
        public void PrintPreview()
        {
            this.Cursor = Cursors.WaitCursor;
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                if (chkReprint.Checked)
                {
                    SaveSummary(reprint:true);
                }
                else
                {
                    SaveSummary(reprint: false);
                }
                db.CommitTrans();
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            PrintReport(false);
            this.Cursor = Cursors.Default;
        }
        public void Print()
        { 
        this.Cursor = Cursors.WaitCursor;
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                if (chkReprint.Checked)
                {
                    SaveSummary(reprint: true);
                }
                else
                {
                    SaveSummary(reprint: false);
                }
                db.CommitTrans();
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            PrintReport(true);
            this.Cursor = Cursors.Default;
        }
        public void LoadData()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                LoadSummaryPO();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        private void LoadSummaryPO()
        {

            string strSQL ="EXEC spTPiCSSubsystem_SummaryPO_Load '"+Convert.ToInt16(chkReprint.EditValue)+"'" ;
            DataTable dt=db.GetDataTable(strSQL);
            if(dt!=null)
            {
                DataColumn dc=new DataColumn();

                dc.ColumnName="SELECT";
                dc.DataType=typeof(bool);
                dc.DefaultValue=false;
                dt.Columns.Add(dc);
                dt.Columns["SELECT"].SetOrdinal(0);

                dc=new DataColumn();
                dc.ColumnName="AMOUNT";
                dc.DataType=typeof(decimal);
                dc.Expression = "QTY*PRICE";
                dt.Columns.Add(dc);
                dt.Columns["AMOUNT"].SetOrdinal(9);

                gridControl1.DataSource=dt;
                gridView1.OptionsView.EnableAppearanceEvenRow=true;
                gridView1.OptionsView.EnableAppearanceOddRow=true;
                gridView1.OptionsView.ColumnAutoWidth=false;
                for (int i = 0; i < gridView1.Columns.Count; i++)
                {
                    if (gridView1.Columns[i].FieldName == "SEIBAN") { continue; }
                    gridView1.Columns[i].BestFit();
                }
                _bsiStatusBar.Caption = "("+dt.Rows.Count+" row(s) affected)";
            }
  
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
        private void SaveSummary(bool reprint)
        {
            string ponum="";
            string strSQL="DELETE FROM MSUMMARYPO ";
            db.Execute(strSQL);
            if(!reprint){ponum = GenPONo("SX", true);}
            for(int i=0;i<gridView1.DataRowCount;i++)
            {
                if (!(bool)gridView1.GetRowCellValue(i, "SELECT")) { continue; }
                string seiban0, seiban1, seiban2;
                string[] arySeiban = (cUtility.SplitByLength(gridView1.GetRowCellValue(i, "SEIBAN").ToString(), 254)).ToArray();
                seiban0 = "";
                seiban1 = "";
                seiban2 = "";
                switch (arySeiban.Length)
                {
                    case 1: seiban0 = arySeiban[0]; break;
                    case 2: seiban0 = arySeiban[0]; seiban1 = arySeiban[1]; break;
                    case 3: seiban0 = arySeiban[0]; seiban1 = arySeiban[1]; seiban2 = arySeiban[2]; break;
                }
                //---------------------------------------new------------------------------------
                if ((!reprint) && (gridView1.GetRowCellDisplayText(i, "PONUM").Length == 0))
                {
                    gridView1.SetRowCellValue(i, "PONUM", ponum);
                    strSQL = "INSERT INTO MSummaryPOIM (PONUM,PORDER,CODE,NAME,SUBNAME,JITU,UNIT,NDATE,PRICE,AMOUNT,CURRE,FSECTION,SBNO ,SBNO1,SBNO2)" +
                        "VALUES('" + gridView1.GetRowCellValue(i, "PONUM") + "'" +
                        ",'" + gridView1.GetRowCellValue(i, "PORDER") + "'" +
                        ",'" + gridView1.GetRowCellValue(i, "CODE") + "'" +
                        ",'" + gridView1.GetRowCellValue(i, "CODE_NAME").ToString().Replace("'", "''") + "'" +
                        ",'" + gridView1.GetRowCellValue(i, "SUPPLIER_NAME").ToString().Replace("'", "''") + "'" +
                        "," + gridView1.GetRowCellValue(i, "QTY") +
                        ",'"+gridView1.GetRowCellValue(i,"UNIT")+"'"+
                        ",'" + gridView1.GetRowCellValue(i, "DUE_DATE").ToString().Substring(0, 8) + "'" +
                        "," + gridView1.GetRowCellValue(i, "PRICE") +
                        "," + gridView1.GetRowCellValue(i, "AMOUNT") +
                        ",'" + gridView1.GetRowCellValue(i, "CURRE") + "'" +
                        ",'" + gridView1.GetRowCellValue(i, "SECTION") + "'" +
                        ",'" + seiban0 + "'" +
                        ",'" + seiban1 + "'" +
                        ",'" + seiban2 + "')";
                    db.Execute(strSQL);
                    strSQL = "UPDATE XSLIP SET PONUM = '" + gridView1.GetRowCellValue(i, "PONUM") + "',ISSUE ='Y' " +
                        " WHERE CODE ='" + gridView1.GetRowCellValue(i, "CODE") + "'" +
                        " AND PORDER = '" + gridView1.GetRowCellValue(i, "PORDER") + "'" +
                        " AND  BUMO = '" + gridView1.GetRowCellValue(i,"SUPPLIER") + "'";
                    db.Execute(strSQL);
                }
                //--------------------------------------------------reprint--------------------------------------------
                strSQL = "INSERT INTO MSummaryPO (PONUM,PORDER,CODE,NAME,SUBNAME,JITU,UNIT,NDATE,PRICE,AMOUNT,CURRE,FSECTION,SBNO,SBNO1, SBNO2 )" +
                    "VALUES('" + gridView1.GetRowCellValue(i, "PONUM") + "'" +
                    ",'" + gridView1.GetRowCellValue(i, "PORDER") + "'" +
                    ",'" + gridView1.GetRowCellValue(i, "CODE") + "'" +
                    ",'" + gridView1.GetRowCellValue(i, "CODE_NAME").ToString().Replace("'", "''") + "'" +
                    ",'" + gridView1.GetRowCellValue(i, "SUPPLIER_NAME").ToString().Replace("'", "''") + "'" +
                    "," + gridView1.GetRowCellValue(i, "QTY") +
                    ",'"+gridView1.GetRowCellValue(i,"UNIT")+"'"+
                    ",'" + gridView1.GetRowCellValue(i, "DUE_DATE").ToString().Substring(0, 8) + "'" +
                    "," + gridView1.GetRowCellValue(i, "PRICE") +
                    "," + gridView1.GetRowCellValue(i, "AMOUNT") +
                    ",'" + gridView1.GetRowCellValue(i, "CURRE") + "'" +
                    ",'" + gridView1.GetRowCellValue(i, "SECTION") + "'" +
                    ",'" + seiban0 + "'" +
                    ",'" + seiban1 + "'" +
                    ",'" + seiban2 + "')";
                db.Execute(strSQL);
                //-----------------------------------------------------------------------------------------------------------
            }
        }
        private void PrintReport(bool toPrinter)
        {
            ctr = new cCrystalReport(Application.StartupPath + "\\Report\\SUMMARY REPORT.rpt");
            if (toPrinter)
            {
                if (!ctr.SetPrinter()){return;}
            }
            ctr.ReportTitle="Summary Purchase";
            //ctr.ReportFileName=Application.StartupPath+"\\Report\\SUMMARY REPORT.rpt";
            string fmlText = "";//"{spSummaryReportRpt;1.PONUM} = '"+xx+"'";
            ctr.PrintReport(fmlText,toPrinter,"sa","");
        }

        private void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelect.Checked)
            {
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    gridView1.SetRowCellValue(i, "SELECT", true);
                }
            }
            else
            {
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    gridView1.SetRowCellValue(i, "SELECT", false);
                }
            }
        }
        private void chkFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFreeze.Checked)
            {
                for (int i = 0; i < gridView1.FocusedColumn.AbsoluteIndex; i++)
                {
                    gridView1.Columns[i].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                }
            }
            else
            {
                for (int i = gridView1.Columns.Count - 1;i>=0 ; i--)
                {
                    gridView1.Columns[i].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.None;
                }
            }
        }
        private void frmPurchase_Summary_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
        }
 
    }
}