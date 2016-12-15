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

namespace TUW_System.YS
{
    public partial class frmYS_Cost : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;
        public delegate void ProgressBarHandler(int minValue, int maxValue, int value, string status);
        public event ProgressBarHandler ProgressBarEvent;

        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        DataSet dsCost;
        DataTable dtMoneyRate;

        private const decimal lbs = 2.2046M;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        public LogIn User_Login { get; set; }

        public frmYS_Cost()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            cboMonth.SelectedIndex = DateTime.Today.Month - 1;
            cboYear.SelectedIndex = 0;
            gridControl1.DataSource = null;
        }
        public void SaveData()
        {
            try
            {
                db.ConnectionOpen();
                db.BeginTrans();
                if (tabbedControlGroup1.SelectedTabPageIndex == 1) SaveMoneyRate();
                MessageBox.Show("Save complete", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }
        public void DisplayData()
        {
            if (tabbedControlGroup1.SelectedTabPageIndex == 0)
            {
                GetCost(cboYear.Text, (cboMonth.SelectedIndex + 1).ToString().PadLeft(2, '0'));
                gridView1.Focus();
            }
            else
            {
                GetMoneyRate(cboYear.Text, (cboMonth.SelectedIndex + 1).ToString().PadLeft(2, '0'));
                gridView3.Focus();
            }
        }
        public void PrintPreview()
        { 
        
        }
        public void Print()
        { 
        
        }
        public void ExportExcel()
        {
            if (tabbedControlGroup1.SelectedTabPageIndex == 0)
            {
                SaveFileDialog theOpenFile = new SaveFileDialog();
                string strTemp;
                theOpenFile.Filter = "Microsoft Excel Document|*.xls;*.xlsx";
                if (theOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    strTemp = theOpenFile.FileName;
                    gridView1.ExportToXlsx(strTemp);
                }
            }
            else
            {
                SaveFileDialog theOpenFile = new SaveFileDialog();
                string strTemp;
                theOpenFile.Filter = "Microsoft Excel Document|*.xls;*.xlsx";
                if (theOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    strTemp = theOpenFile.FileName;
                    gridView3.ExportToXlsx(strTemp);
                }
            }
        }

        private void GetCost(string strYear, string strMonth)
        {
            string strSQL = "EXEC spTUWSystem_YS_YarnCost '" + strYear + "','" + strMonth + "'"; 
            dsCost =db.GetDataSet(strSQL);
            dsCost.Tables[0].BeginInit();
            DataColumn dc = new DataColumn();
            dc.ColumnName = "COST";
            dc.DataType = typeof(decimal);
            dsCost.Tables[0].Columns.Add(dc);
            dsCost.Tables[0].Columns["COST"].SetOrdinal(1);
            dc = new DataColumn();
            dc.ColumnName = "BAL_CTN";
            dc.DataType = typeof(decimal);
            dsCost.Tables[0].Columns.Add(dc);
            dc = new DataColumn();
            dc.ColumnName = "BAL_KGS";
            dc.DataType = typeof(decimal);
            dsCost.Tables[0].Columns.Add(dc);
            dc = new DataColumn();
            dc.ColumnName = "BAL_AMT";
            dc.DataType = typeof(decimal);
            dsCost.Tables[0].Columns.Add(dc);
            foreach (DataRow dr in dsCost.Tables[0].Rows)
            {
                decimal beginCtn = (decimal)dr["BEGIN_CTN"];
                decimal beginKgs=(decimal)dr["BEGIN_KGS"];
                decimal beginAmount=(decimal)dr["BEGIN_AMT"];
                decimal beginCost=(decimal)dr["BEGIN_PRICE"];
                decimal inCtn=Convert.ToDecimal(dr["IN_CTN"]);
                decimal inKgs=(decimal)dr["IN_KGS"];
                decimal inAmount = (decimal)dr["IN_AMT"];
                decimal returnCtn=Convert.ToDecimal(dr["RETURN_CTN"]);
                decimal returnKgs = (decimal)dr["RETURN_KGS"];
                decimal outCtn=Convert.ToDecimal(dr["OUT_CTN"]);
                decimal outKgs = (decimal)dr["OUT_KGS"];
                decimal cost=0;

                if ((beginKgs + inKgs) == 0)
                    cost = beginCost;
                else
                    cost = (beginAmount + inAmount) / (beginKgs + inKgs);
                //dr["COST"] = cost;
                if(inKgs!=0m) dr["IN_PRICE"] = inAmount /inKgs;
                dr["OUT_AMT"] = cost*outKgs;
                dr["BAL_CTN"] = (beginCtn + inCtn + returnCtn) - outCtn;
                dr["BAL_KGS"] = (beginKgs + inKgs+returnKgs) - outKgs;
                dr["BAL_AMT"] = ((beginKgs + inKgs + returnKgs) - outKgs)*cost;
            }
            dsCost.Tables[0].EndInit();
            dsCost.Relations.Add("IN",
                new DataColumn[] { dsCost.Tables[0].Columns["ID"]},
                new DataColumn[] {dsCost.Tables[1].Columns["ID"]});
            dsCost.Relations.Add("RETURN",
                new DataColumn[] { dsCost.Tables[0].Columns["ID"] },
                new DataColumn[] { dsCost.Tables[2].Columns["ID"]});
            dsCost.Relations.Add("OUT",
                new DataColumn[] { dsCost.Tables[0].Columns["ID"]},
                new DataColumn[] { dsCost.Tables[3].Columns["ID"] });
            gridControl1.DataSource = dsCost.Tables[0];
            gridView1.PopulateColumns();
            gridView1.Columns["ID"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            //gridView1.Columns["SAMPLE"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            gridView1.Columns["COST"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            gridView1.Columns["BAL_AMT"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            gridView1.Columns["BAL_KGS"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            gridView1.Columns["BAL_CTN"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            //---Display format---
            //gridView1.Columns["COST"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            //gridView1.Columns["COST"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["BEGIN_CTN"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["BEGIN_CTN"].DisplayFormat.FormatString = "n0";
            gridView1.Columns["BEGIN_KGS"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["BEGIN_KGS"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["BEGIN_AMT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["BEGIN_AMT"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["IN_CTN"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["IN_CTN"].DisplayFormat.FormatString = "n0";
            gridView1.Columns["IN_KGS"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["IN_KGS"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["IN_PRICE"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["IN_PRICE"].DisplayFormat.FormatString = "n4";
            gridView1.Columns["IN_AMT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["IN_AMT"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["OUT_CTN"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["OUT_CTN"].DisplayFormat.FormatString = "n0";
            gridView1.Columns["OUT_KGS"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["OUT_KGS"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["OUT_AMT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["OUT_AMT"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["BAL_CTN"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["BAL_CTN"].DisplayFormat.FormatString = "n0";
            gridView1.Columns["BAL_KGS"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["BAL_KGS"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["BAL_AMT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["BAL_AMT"].DisplayFormat.FormatString = "n2";
            //---GridSummary---
            gridView1.OptionsView.ShowFooter = true;
            gridView1.Columns["BEGIN_CTN"].SummaryItem.FieldName = "BEGIN_CTN";
            gridView1.Columns["BEGIN_CTN"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n0}");
            gridView1.Columns["BEGIN_KGS"].SummaryItem.FieldName = "BEGIN_KGS";
            gridView1.Columns["BEGIN_KGS"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView1.Columns["BEGIN_AMT"].SummaryItem.FieldName = "BEGIN_AMT";
            gridView1.Columns["BEGIN_AMT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView1.Columns["IN_CTN"].SummaryItem.FieldName = "IN_CTN";
            gridView1.Columns["IN_CTN"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n0}");
            gridView1.Columns["IN_KGS"].SummaryItem.FieldName = "IN_KGS";
            gridView1.Columns["IN_KGS"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView1.Columns["IN_AMT"].SummaryItem.FieldName = "IN_AMT";
            gridView1.Columns["IN_AMT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView1.Columns["OUT_CTN"].SummaryItem.FieldName = "OUT_CTN";
            gridView1.Columns["OUT_CTN"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n0}");
            gridView1.Columns["OUT_KGS"].SummaryItem.FieldName = "OUT_KGS";
            gridView1.Columns["OUT_KGS"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView1.Columns["OUT_AMT"].SummaryItem.FieldName = "OUT_AMT";
            gridView1.Columns["OUT_AMT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView1.Columns["BAL_CTN"].SummaryItem.FieldName = "BAL_CTN";
            gridView1.Columns["BAL_CTN"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n0}");
            gridView1.Columns["BAL_KGS"].SummaryItem.FieldName = "BAL_KGS";
            gridView1.Columns["BAL_KGS"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView1.Columns["BAL_AMT"].SummaryItem.FieldName = "BAL_AMT";
            gridView1.Columns["BAL_AMT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            
            
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
        }
        //private void ClosePeriod(string strYear,string strMonth)
        //{
        //    if (MessageBox.Show("คุณต้องการยกยอดข้อมูลนี้ไปเป็น Begin เดือน " + strYear + strMonth + " หรือไม่", "Close Period", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) throw new Exception("Cancel");
        //    StatusBarEvent("");
        //    ProgressBarEvent(0, gridView1.DataRowCount-1, 0, "Initialize");
        //    gridView1.UpdateCurrentRow();
        //    gridView1.ClearColumnsFilter();
        //    string strSQL;
        //    strSQL = "Delete From YarnCost Where MonthYear='" + strYear + strMonth + "'";
        //    db.Execute(strSQL);
        //    for (int i = 0; i < gridView1.DataRowCount; i++)
        //    {
        //        strSQL = "Insert Into YarnCost(MonthYear,YarnID,Sample,BeginCost,BeginWeight)Values('" + strYear + strMonth + "'," +
        //            gridView1.GetRowCellValue(i, "ID") + ",";
        //        strSQL += ((bool)gridView1.GetRowCellValue(i, "SAMPLE") == true) ? "1" : "0";
        //        strSQL+=",'"+gridView1.GetRowCellValue(i, "COST") + "','" + gridView1.GetRowCellValue(i, "BAL_KGS") + "')";
        //        //System.Diagnostics.Debug.WriteLine(strSQL);
        //        db.Execute(strSQL);
        //        StatusBarEvent(i + "/" + gridView1.DataRowCount);
        //        ProgressBarEvent(0, gridView1.DataRowCount - 1, i, "Update");
        //    }
        //    db.CommitTrans();
        //}
        private void GetMoneyRate(string strYear, string strMonth)
        {
            string strSQL = "SELECT	B.PRODUCTCODE AS CODE,A.PONO,A.RECEIVENO AS REFNO,A.RECEIVEDATE AS DATE,"+
	            "A.DELIVERYNO AS INVNO,C.NAME AS SUPPLIER,D.CURCODE AS CUR,"+
                "CASE WHEN B.IDUNIT='U-0020' THEN 'KGS' ELSE 'LBS' END AS UNIT,"+
	            "CASE WHEN B.IDUNIT<>'U-0020' THEN B.QTY ELSE B.QTY*2.2046 END AS QTY_LBS,"+
	            "CASE WHEN B.IDUNIT='U-0020' THEN B.QTY ELSE B.QTY/2.2046 END AS QTY_KGS,"+
                "CASE WHEN B.IDUNIT<>'U-0020' THEN B.UNITPRICE ELSE (B.QTY*B.UNITPRICE)/NULLIF((B.QTY*2.2046),0)  END PRICE_LBS," +
                "CASE WHEN B.IDUNIT='U-0020' THEN B.UNITPRICE ELSE (B.QTY*B.UNITPRICE)/(B.QTY/2.2046) END PRICE_KGS," +
                "ROUND(B.QTY*B.UNITPRICE,2) AS AMOUNT,"+
                "ROUND(B.QTY*B.UNITPRICE,2)*A.MONEYRATE AS AMOUNTxRATE," +
                "ROUND(B.QTY*B.UNITPRICE,2)*A.MONEYRATE*0.025 AS IMPORT,"+
                "(ROUND(B.QTY*B.UNITPRICE,2)*A.MONEYRATE)+(ROUND(B.QTY*B.UNITPRICE,2)*A.MONEYRATE*0.025) AS TOTAL," +
                "A.MONEYRATE AS RATE " +
                "FROM PO_RECEIVE A	INNER JOIN PO_RECEIVEDETAIL B ON A.RECEIVENO=B.RECEIVENO "+
	            "LEFT OUTER JOIN PO_SUPPLIER C ON A.IDSUP=C.IDSUP "+
	            "LEFT OUTER JOIN PO_CURRENCY D ON A.CURRENCYUNIT=D.IDCUR "+
                "WHERE YEAR(A.RECEIVEDATE)='"+ strYear +"' AND MONTH(A.RECEIVEDATE)='"+ strMonth +" '"+
                "AND A.PONO LIKE 'FX%' AND A.CURRENCYUNIT<>'C-0001' ORDER BY C.NAME,A.RECEIVEDATE";
            dtMoneyRate = db.GetDataTable(strSQL);
            dtMoneyRate.BeginInit();
            DataColumn dc = new DataColumn();
            dc.ColumnName = "EDIT";
            dc.DataType = typeof(System.Boolean);
            dc.DefaultValue = false;
            dtMoneyRate.Columns.Add(dc);
            dtMoneyRate.EndInit();
            gridControl3.DataSource = dtMoneyRate;
            gridView3.PopulateColumns();
            gridView3.Columns["UNIT"].Visible = false;
            gridView3.Columns["QTY_LBS"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView3.Columns["QTY_LBS"].DisplayFormat.FormatString = "n2";
            gridView3.Columns["QTY_KGS"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView3.Columns["QTY_KGS"].DisplayFormat.FormatString = "n2";
            gridView3.Columns["PRICE_LBS"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView3.Columns["PRICE_LBS"].DisplayFormat.FormatString = "n2";
            gridView3.Columns["PRICE_KGS"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView3.Columns["PRICE_KGS"].DisplayFormat.FormatString = "n2";
            gridView3.Columns["AMOUNT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView3.Columns["AMOUNT"].DisplayFormat.FormatString = "n2";
            gridView3.Columns["AMOUNTxRATE"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView3.Columns["AMOUNTxRATE"].DisplayFormat.FormatString = "n2";
            gridView3.Columns["IMPORT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView3.Columns["IMPORT"].DisplayFormat.FormatString = "n2";
            gridView3.Columns["TOTAL"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView3.Columns["TOTAL"].DisplayFormat.FormatString = "n2";
            gridView3.Columns["RATE"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            gridView3.Columns["EDIT"].Visible = false;
            //---GridGroupSummry---
            gridView3.OptionsView.GroupFooterShowMode= DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
            gridView3.Columns["SUPPLIER"].GroupIndex = 0;
            //DevExpress.XtraGrid.GridGroupSummaryItem item = new DevExpress.XtraGrid.GridGroupSummaryItem();
            //item.FieldName = "REFNO";
            //item.SummaryType = DevExpress.Data.SummaryItemType.Count;
            //item.DisplayFormat = "(Count = {0:n0})";
            //gridView3.GroupSummary.Add(item);
            DevExpress.XtraGrid.GridGroupSummaryItem item1 = new DevExpress.XtraGrid.GridGroupSummaryItem();
            item1.FieldName = "REFNO";
            item1.SummaryType = DevExpress.Data.SummaryItemType.Count;
            item1.DisplayFormat = "(Count={0:n0})";
            item1.ShowInGroupColumnFooter = gridView3.Columns["CODE"];
            gridView3.GroupSummary.Add(item1);
            item1 = new DevExpress.XtraGrid.GridGroupSummaryItem();
            item1.FieldName = "QTY_LBS";
            item1.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            item1.DisplayFormat = "{0:n2}";
            item1.ShowInGroupColumnFooter = gridView3.Columns["QTY_LBS"];
            gridView3.GroupSummary.Add(item1);
            item1 = new DevExpress.XtraGrid.GridGroupSummaryItem();
            item1.FieldName = "QTY_KGS";
            item1.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            item1.DisplayFormat = "{0:n2}";
            item1.ShowInGroupColumnFooter = gridView3.Columns["QTY_KGS"];
            gridView3.GroupSummary.Add(item1);
            item1 = new DevExpress.XtraGrid.GridGroupSummaryItem();
            item1.FieldName = "AMOUNT";
            item1.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            item1.DisplayFormat = "{0:n2}";
            item1.ShowInGroupColumnFooter = gridView3.Columns["AMOUNT"];
            gridView3.GroupSummary.Add(item1);
            item1 = new DevExpress.XtraGrid.GridGroupSummaryItem();
            item1.FieldName = "AMOUNTxRATE";
            item1.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            item1.DisplayFormat = "{0:n2}";
            item1.ShowInGroupColumnFooter = gridView3.Columns["AMOUNTxRATE"];
            gridView3.GroupSummary.Add(item1);
            item1 = new DevExpress.XtraGrid.GridGroupSummaryItem();
            item1.FieldName = "IMPORT";
            item1.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            item1.DisplayFormat = "{0:n2}";
            item1.ShowInGroupColumnFooter = gridView3.Columns["IMPORT"];
            gridView3.GroupSummary.Add(item1);
            item1 = new DevExpress.XtraGrid.GridGroupSummaryItem();
            item1.FieldName = "TOTAL";
            item1.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            item1.DisplayFormat = "{0:n2}";
            item1.ShowInGroupColumnFooter = gridView3.Columns["TOTAL"];
            gridView3.GroupSummary.Add(item1);
            gridView3.ExpandAllGroups();
            //---GridSummry---
            gridView3.OptionsView.ShowFooter = true;
            gridView3.Columns["QTY_LBS"].SummaryItem.FieldName = "QTY_LBS";
            gridView3.Columns["QTY_LBS"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView3.Columns["QTY_KGS"].SummaryItem.FieldName = "QTY_KGS";
            gridView3.Columns["QTY_KGS"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView3.Columns["AMOUNT"].SummaryItem.FieldName = "AMOUNT";
            gridView3.Columns["AMOUNT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView3.Columns["AMOUNTxRATE"].SummaryItem.FieldName = "AMOUNTxRATE";
            gridView3.Columns["AMOUNTxRATE"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView3.Columns["IMPORT"].SummaryItem.FieldName = "IMPORT";
            gridView3.Columns["IMPORT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView3.Columns["TOTAL"].SummaryItem.FieldName = "TOTAL";
            gridView3.Columns["TOTAL"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            //---Read Only-------------
            for (int i = 0; i < gridView3.Columns.Count - 2; i++)
            {
                gridView3.Columns[i].OptionsColumn.ReadOnly = true;
            }
            //-------------------------
            gridView3.OptionsView.EnableAppearanceEvenRow = true;
            gridView3.OptionsView.EnableAppearanceOddRow = true;
            gridView3.OptionsView.ColumnAutoWidth = false;
            gridView3.BestFitColumns();
        }
        private void SaveMoneyRate()
        {
            gridView3.UpdateCurrentRow();
            gridView3.ClearColumnsFilter();
            string strSQL;
            for (int i = 0; i < gridView3.DataRowCount; i++)
            {
                if ((bool)(gridView3.GetRowCellValue(i, "EDIT")) == false) continue;
                //save ไปยัง po_receive
                strSQL = "UPDATE PO_RECEIVE SET MONEYRATE=" + gridView3.GetRowCellValue(i, "RATE") + "," +
                    "INPUTDATE=(GETDATE()),INPUTUSER='" + User_Login.UserName +"'"+
                    " WHERE RECEIVENO='" + gridView3.GetRowCellDisplayText(i, "REFNO") + "'";
                db.Execute(strSQL);
                //save ไปยัง po_purchase
                strSQL = "UPDATE PO_PURCHASE SET MONEYRATE=" + gridView3.GetRowCellValue(i, "RATE") + "," +
                    "INPUTDATE=(GETDATE()),INPUTUSER='" + User_Login.UserName + "'" +
                    " WHERE PONO='" + gridView3.GetRowCellDisplayText(i, "PONO") + "'";
                db.Execute(strSQL);
                //ใช้ rate ใหม่นี้เพื่อคำนวนและเซฟไปยัง yangenbarcode และ yarnreceivedetail
                strSQL="UPDATE A "+
                    "SET A.UNITPRICE=CASE WHEN C.IDUNIT='U-0124' THEN C.UNITPRICE*B.MONEYRATE*2.2046 ELSE C.UNITPRICE*B.MONEYRATE END "+
                    "FROM YARNRECEIVEDETAIL A "+
	                    "INNER JOIN PO_RECEIVE B ON A.RECNO=B.RECEIVENO "+
	                    "INNER JOIN PO_RECEIVEDETAIL C ON B.RECEIVENO=C.RECEIVENO "+
                    "WHERE A.RECNO='"+gridView3.GetRowCellDisplayText(i,"REFNO")+"'";
                db.Execute(strSQL);
                strSQL = "UPDATE A " +
                    "SET A.PRICE=CASE WHEN D.IDUNIT='U-0124' THEN D.UNITPRICE*C.MONEYRATE*2.2046 ELSE D.UNITPRICE*C.MONEYRATE END " +
                    "FROM YARNGENBARCODE A " +
                        "INNER JOIN YARNRECEIVEDETAIL B ON A.SERIAL=B.YARNSERIAL " +
                        "INNER JOIN PO_RECEIVE C ON B.RECNO=C.RECEIVENO " +
                        "INNER JOIN PO_RECEIVEDETAIL D ON C.RECEIVENO=D.RECEIVENO " +
                    "WHERE B.RECNO='" + gridView3.GetRowCellDisplayText(i, "REFNO") + "'";
                db.Execute(strSQL);

                SetAveragePrice(cboYear.Text+(cboMonth.SelectedIndex + 1).ToString().PadLeft(2, '0'),Convert.ToInt32(gridView3.GetRowCellValue(i,"CODE")));
            }
            db.CommitTrans();
            GetMoneyRate(cboYear.Text, (cboMonth.SelectedIndex + 1).ToString().PadLeft(2, '0'));
        }
        private void SetAveragePrice(string monthYear, int yarnID)//monthyear=yyyyMMdd,คำนวน cost เฉลี่ยแล้วเอาไปใส่ในตาราง yarncode
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
                        strSQL = "select count(yarnid) from yarnstockbegin where monthyear='" + nextMonth + "' and yarnid=" + yarnID;
                        if (Convert.ToInt32(db.ExecuteFirstValue(strSQL)) > 0)
                        {
                            strSQL = "update yarnstockbegin set cost=" + cost + " where monthyear='" + nextMonth + "' and yarnid=" + yarnID;
                            db.Execute(strSQL);
                            if(nextMonth==DateTime.Today.ToString("yyyyMM",dtfinfo))//ถ้าย้อนหลังไปเดือนที่แล้วจากปัจจุบันให้ทำการอัพเดท cost ใน yarncode ด้วย
                                SetAveragePrice(nextMonth,yarnID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmYS_Cost_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            for (int i = 0; i > -10; i--)
            {
                cboYear.Properties.Items.Add(DateTime.Today.AddYears(i).Year);
            }
            NewData();

        }
        //private void cmdClosePeriod_Click(object sender, EventArgs e)
        //{
        //    this.Cursor=Cursors.WaitCursor;
        //    try
        //    {
        //        db.ConnectionOpen();
        //        db.BeginTrans();
        //        ClosePeriod(cboYear.Text, (cboMonth.SelectedIndex + 2).ToString().PadLeft(2, '0'));
        //        MessageBox.Show("Save complete", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        db.RollbackTrans();
        //        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        db.ConnectionClose();
        //    }
        //    this.Cursor = Cursors.Default;
        //}
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
        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            switch (e.Column.FieldName)
            { 
                case "COST":
                    Font newFont=new Font(e.Appearance.Font,FontStyle.Bold);
                    e.Appearance.Font = newFont;
                    break;
                case "BEGIN_CTN":
                case "BEGIN_KGS":
                case "BEGIN_PRICE":
                case "BEGIN_AMT":
                    e.Column.AppearanceCell.BackColor = Color.Plum;
                    e.Column.AppearanceCell.ForeColor = Color.Indigo;
                    break;
                case "IN_CTN":
                case "IN_KGS":
                case "IN_PRICE":
                case "IN_AMT":
                    e.Column.AppearanceCell.BackColor = Color.LightGreen;
                    e.Column.AppearanceCell.ForeColor = Color.DarkGreen;
                    break;
                case "RETURN_CTN":
                case "RETURN_KGS":
                case "RETURN_PRICE":
                case "RETURN_AMT":
                    e.Column.AppearanceCell.BackColor = Color.LightYellow;
                    e.Column.AppearanceCell.ForeColor = Color.SaddleBrown;
                    break;
                case "OUT_CTN":
                case "OUT_KGS":
                case "OUT_PRICE":
                case "OUT_AMT":
                    e.Column.AppearanceCell.BackColor = Color.LightPink;
                    e.Column.AppearanceCell.ForeColor = Color.Maroon;
                    break;
                case "BAL_CTN":
                case "BAL_KGS":
                case "BAL_AMT":
                    e.Column.AppearanceCell.BackColor = Color.LightSkyBlue;
                    e.Column.AppearanceCell.ForeColor = Color.DarkBlue;
                    break;
            }
        }
        private void gridView3_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            decimal rate=(decimal)gv.GetRowCellValue(e.RowHandle,"RATE");
            if (rate == 0m)
                e.Appearance.ForeColor = Color.Red;
            else if((bool)gv.GetRowCellValue(e.RowHandle,"EDIT"))
                e.Appearance.ForeColor = Color.Green;
        }
        private void gridViewOut_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if ((decimal)gv.GetRowCellValue(e.RowHandle, "MONEYRATE") == 0m && gv.GetRowCellValue(e.RowHandle,"CURCODE").ToString()!="THB") e.Appearance.ForeColor = Color.Red;
        }
        private void gridView1_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {

        }
        private void gridView3_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (gv.GetRowCellDisplayText(e.RowHandle, "UNIT") == "LBS")
            {
                Font newFont = new Font(gv.Appearance.Row.Font, FontStyle.Bold);
                e.Appearance.Font = newFont;
            }
        }
        private void gridView3_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            try
            {
                if (gv.FocusedColumn.FieldName == "RATE")
                {
                    if (gv.GetFocusedRowCellDisplayText("UNIT") == "KGS")
                    {
                        gv.SetFocusedRowCellValue("AMOUNTxRATE",Math.Round(Convert.ToDecimal(gv.GetFocusedRowCellValue("QTY_KGS")) * Convert.ToDecimal(gv.GetFocusedRowCellValue("PRICE_KGS")),2) * Convert.ToDecimal(e.Value));
                        gv.SetFocusedRowCellValue("IMPORT", Convert.ToDecimal(gv.GetFocusedRowCellValue("AMOUNTxRATE")) * 0.025m);
                        gv.SetFocusedRowCellValue("TOTAL", Convert.ToDecimal(gv.GetFocusedRowCellValue("AMOUNTxRATE")) + Convert.ToDecimal(gv.GetFocusedRowCellValue("IMPORT")));
                    }
                    else
                    {
                        gv.SetFocusedRowCellValue("AMOUNTxRATE",Math.Round(Convert.ToDecimal(gv.GetFocusedRowCellValue("QTY_LBS")) * Convert.ToDecimal(gv.GetFocusedRowCellValue("PRICE_LBS")),2) * Convert.ToDecimal(e.Value));
                        gv.SetFocusedRowCellValue("IMPORT", Convert.ToDecimal(gv.GetFocusedRowCellValue("AMOUNTxRATE")) * 0.025m);
                        gv.SetFocusedRowCellValue("TOTAL", Convert.ToDecimal(gv.GetFocusedRowCellValue("AMOUNTxRATE")) + Convert.ToDecimal(gv.GetFocusedRowCellValue("IMPORT")));
                    }
                    gv.BestFitColumns();
                    gv.SetFocusedRowCellValue("RATE", e.Value);
                }    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "RATE") gridView3.SetRowCellValue(e.RowHandle, "EDIT", true);
        }
        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if(e.Column.FieldName=="ID"||e.Column.FieldName=="COST"||e.Column.FieldName=="BASEID") return;
            if(Convert.ToDecimal(e.Value) == 0) e.DisplayText = "";
        }
        //private void tabbedControlGroup1_SelectedPageChanged(object sender, DevExpress.XtraLayout.LayoutTabPageChangedEventArgs e)
        //{
        //    if (e.Page.Name == "layoutControlGroup3")
        //        btnClosePeriod.Enabled = true;
        //    else
        //        btnClosePeriod.Enabled = false;
        //}
        private void cboYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (tabbedControlGroup1.SelectedTabPageIndex == 0)
            //    GetCost(cboYear.Text, (cboMonth.SelectedIndex + 1).ToString().PadLeft(2, '0'));
            //else
            //    GetMoneyRate(cboYear.Text, (cboMonth.SelectedIndex + 1).ToString().PadLeft(2, '0'));
        }
        private void cboMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (tabbedControlGroup1.SelectedTabPageIndex == 0)
            //    GetCost(cboYear.Text, (cboMonth.SelectedIndex + 1).ToString().PadLeft(2, '0'));
            //else
            //    GetMoneyRate(cboYear.Text, (cboMonth.SelectedIndex + 1).ToString().PadLeft(2, '0'));
        }
        private void gridView1_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView master = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            DevExpress.XtraGrid.Views.Grid.GridView detail = (DevExpress.XtraGrid.Views.Grid.GridView)master.GetDetailView(e.RowHandle, e.RelationIndex);
            detail.Columns["ID"].Visible = false;
            detail.Columns["SAMPLE"].Visible = false;
            if (e.RelationIndex == 0)
            {
                detail.Columns["QTY"].SummaryItem.FieldName = "QTY";
                detail.Columns["QTY"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
                detail.RowCellStyle += this.gridViewOut_RowCellStyle;
            }
            else
            {
                detail.Columns["CARTON"].SummaryItem.FieldName = "CARTON";
                detail.Columns["CARTON"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n0}");
                detail.Columns["WEIGHT"].SummaryItem.FieldName = "WEIGHT";
                detail.Columns["WEIGHT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
                detail.OptionsView.AllowCellMerge = true;
                detail.CellMerge += this.gridViewOut_CellMerge;
            }
            detail.OptionsView.EnableAppearanceEvenRow = true;
            detail.OptionsView.EnableAppearanceOddRow = true;
            detail.OptionsView.ColumnAutoWidth = false;
            detail.BestFitColumns();
        }
        private void gridViewOut_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView grid=(DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (e.Column == grid.Columns["PONO_LOTNO"] || e.Column == grid.Columns["ISSNO"]||e.Column==grid.Columns["DATE"]||e.Column==grid.Columns["CODE"]||e.Column==grid.Columns["RETNO"])
            {
                string value1 = grid.GetRowCellValue(e.RowHandle1, e.Column).ToString();
                string value2 = grid.GetRowCellValue(e.RowHandle2, e.Column).ToString();
                if (value1 == value2)
                {
                    e.Merge = true;
                    e.Handled = true;
                }
            }
            else
            {
                e.Merge = false;
                e.Handled = true;
            }

        }
        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            StatusBarEvent(e.CellValue.ToString());
        }



        

    }

}