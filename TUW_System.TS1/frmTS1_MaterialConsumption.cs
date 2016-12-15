using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Globalization;
using myClass;

namespace TUW_System.TS1
{
    public partial class frmTS1_MaterialConsumption : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;

        cDatabase db;
        CultureInfo clinfo=new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        //private DevExpress.XtraBars.BarStaticItem _bsiStatusBar;
        //public DevExpress.XtraBars.BarStaticItem bsiStatusbar
        //{
        //    get { return _bsiStatusBar; }
        //    set { _bsiStatusBar = value; }
        //}

        public frmTS1_MaterialConsumption()
        {
            InitializeComponent();
        }
        public void ExportExcel()
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv;
            if(xtraTabControl1.SelectedTabPageIndex==0)
                gv=gridView1;
            else
                gv=gridView2;
            SaveFileDialog theOpenFile=new SaveFileDialog();
            theOpenFile.Filter="Excel file (*.xls)|*.xls";
            if(theOpenFile.ShowDialog()==System.Windows.Forms.DialogResult.OK) gv.ExportToXls(theOpenFile.FileName);
        }
        public void DisplayData()
        {
            this.Cursor=Cursors.WaitCursor;
            try 
	        {
                if (chkSummary.Checked)
                    DisplayData_Summary(cboMaterial.SelectedIndex);
                else
                    gridControl1.DataSource = null;
                if (chkDetail.Checked)
                    DisplayData_Detail(cboMaterial.SelectedIndex);
                else
                    gridControl2.DataSource = null;
	        }
	        catch (SystemException ex)
	        {
                MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);		
	        }
            catch(ApplicationException)
            {
                //Do nothing
            }
            this.Cursor=Cursors.Default;
        }
        public void ClearData()
        {
            cboMaterial.SelectedIndex = 0;
            chkSummary.Checked = false;
            chkDetail.Checked = false;
            dtpFrom.EditValue = null;
            dtpFrom2.EditValue = null;
            dtpTo.EditValue = null;
            dtpTo2.EditValue = null;
            txtContract.Text = "";
            chkMerge.Checked = false;
            chkMerge2.Checked = false;
            gridControl1.DataSource = null;
            gridControl2.DataSource = null; 
        }

        private void DisplayData_Summary(int intType)
        {
            string strSQL="";
            string strSQL1;//Fabric
            string strSQL2;//Accessory Pack
            string strSQL3;//Accessory Sew
            //------------------------------------------------------------Fabric---------------------------------------------------------------------
            strSQL1 = "SELECT SUBSTRING(XRECE.CDATE, 1, 8) AS SHIP_DATE" +
                    ", SUBSTRING(XRECE.NDATE, 1, 8) AS PACK_DATE,SUBSTRING(XRECE.PDATE, 1, 8) AS SEW_DATE" +
                    ",XRECE.CUST,XRECE.CONTRACT, XPRTS_2.KCODE AS MATERIAL" +
                    ",SUM(XRECE.KVOL * XPRTS_2.SIYOU) AS USED,DistinctInventory.CUR_INV AS INVENTORY" +
                    ",MAX(XHEAD.NAME) AS MATERIAL_NAME,MAX(XCUST.SECTION) AS DIVISION " +
                    ",MAX(DistinctInventory.ALREADY_USED) AS ALREADY_USED " +
                    ",MAX(DistinctInventory.COST_NAME) AS COST_NAME " +
                    ",MAX(DistinctInventory.PUTAWAY) AS PUTAWAY " +
                    ",MAX(DistinctInventory.PREV_INV) AS PREV_INV " +
                    "FROM XCUST RIGHT OUTER JOIN " +
                    "DistinctInventory RIGHT OUTER JOIN " +
                    "XPRTS INNER JOIN " +
                    "XRECE ON XPRTS.CODE = XRECE.CODE INNER JOIN " +
                    "XPRTS XPRTS_1 ON XPRTS.KCODE = XPRTS_1.CODE INNER JOIN " +
                    "XPRTS XPRTS_2 ON XPRTS_1.KCODE = XPRTS_2.CODE LEFT OUTER JOIN " +
                    "XHEAD ON XPRTS_2.KCODE = XHEAD.CODE ON DistinctInventory.CODE = XPRTS_2.KCODE ON " +
                    "XCUST.CUST = XRECE.CUST ";
            switch(optSearch.SelectedIndex)
            {
                case 0:
                    strSQL1+="WHERE (XRECE.CDATE Between '"+((DateTime)dtpFrom.EditValue).ToString("yyyyMMdd",dtfinfo)+"1' And '"+((DateTime)dtpTo.EditValue).ToString("yyyyMMdd",dtfinfo)+"1') ";
                    break;
                case 1:
                    strSQL1+="WHERE (XRECE.PDATE Between '"+((DateTime)dtpFrom2.EditValue).ToString("yyyyMMdd",dtfinfo)+"1' And '"+((DateTime)dtpTo2.EditValue).ToString("yyyyMMdd",dtfinfo)+"1') ";
                    break;
                case 2:
                    strSQL1+="Where (XRECE.CONTRACT Like '"+txtContract.Text+"%') ";
                    break;
            }
            strSQL1+="GROUP BY XRECE.CDATE,XRECE.NDATE,XRECE.PDATE,XRECE.CUST,XRECE.CONTRACT,XPRTS_2.KCODE,DistinctInventory.CUR_INV "+
                "ORDER BY XRECE.CDATE,XRECE.NDATE,XRECE.PDATE,XRECE.CUST,XRECE.CONTRACT, XPRTS_2.KCODE";
            //------------------------------------------------------------Accessory Pack---------------------------------------------------------------------
            strSQL2 = "SELECT SUBSTRING(XRECE.CDATE, 1, 8) AS SHIP_DATE"+
                ",SUBSTRING(XRECE.NDATE, 1, 8) AS PACK_DATE, SUBSTRING(XRECE.PDATE,1, 8) AS SEW_DATE"+
                ",XRECE.CUST,XRECE.CONTRACT, XPRTS.KCODE AS MATERIAL"+
                ",SUM(XRECE.KVOL * (XPRTS.SIYOU / XPRTS.SIYOUW)) AS USED,DistinctInventory.CUR_INV AS INVENTORY"+
                ",MAX(XHEAD.NAME) AS MATERIAL_NAME, MAX(XCUST.[SECTION]) AS DIVISION"+
                ",MAX(DistinctInventory.ALREADY_USED) AS ALREADY_USED"+
                ",MAX(DistinctInventory.COST_NAME) AS COST_NAME"+
                ",MAX(DistinctInventory.PUTAWAY) AS PUTAWAY"+
                ",MAX(DistinctInventory.PREV_INV) AS PREV_INV "+
                "FROM XCUST RIGHT OUTER JOIN "+
                "XPRTS INNER JOIN "+
                "XRECE ON XPRTS.CODE = XRECE.CODE LEFT OUTER JOIN "+
                "XHEAD ON XPRTS.KCODE = XHEAD.CODE LEFT OUTER JOIN "+
                "DistinctInventory ON XPRTS.KCODE = DistinctInventory.CODE ON XCUST.CUST = XRECE.CUST ";
            switch(optSearch.SelectedIndex)
            {
                case 0:
                    strSQL2 +="WHERE (XRECE.CDATE Between '"+((DateTime)dtpFrom.EditValue).ToString("yyyyMMdd",dtfinfo)+ "1' And '"+((DateTime)dtpTo.EditValue).ToString("yyyyMMdd",dtfinfo)+ "1')";
                    break;
                case 1:
                    strSQL2+="WHERE (XRECE.PDATE Between '"+((DateTime)dtpFrom2.EditValue).ToString("yyyyMMdd",dtfinfo)+ "1' And '"+((DateTime)dtpTo2.EditValue).ToString("yyyyMMdd",dtfinfo)+ "1')";
                    break;
                case 2:
                    strSQL2+="Where (XRECE.CONTRACT Like '" +txtContract.Text+ "%')";
                    break;
            }
            strSQL2+=" AND (DistinctInventory.COST_NAME NOT LIKE 'SEW%') ";
            strSQL2+="GROUP BY XRECE.CDATE,XRECE.NDATE,XRECE.PDATE,XRECE.CUST,XRECE.CONTRACT,XPRTS.KCODE,DistinctInventory.CUR_INV"+
                " ORDER BY XRECE.CDATE,XRECE.NDATE,XRECE.PDATE,XRECE.CUST,XRECE.CONTRACT,XPRTS.KCODE";
            //------------------------------------------------------------Accessory Sew---------------------------------------------------------------------
            strSQL3 = "SELECT SUBSTRING(XRECE.CDATE, 1, 8) AS SHIP_DATE"+
                ",SUBSTRING(XRECE.NDATE, 1, 8) AS PACK_DATE, SUBSTRING(XRECE.PDATE,1, 8) AS SEW_DATE" +
                ",XRECE.CUST,XRECE.CONTRACT, XPRTS_1.KCODE AS MATERIAL" +
                ", SUM(XRECE.KVOL * (XPRTS_1.SIYOU / XPRTS_1.SIYOUW)) AS USED,DistinctInventory.CUR_INV AS INVENTORY" +
                ", MAX(XHEAD.NAME) AS MATERIAL_NAME, MAX(XCUST.[SECTION]) AS DIVISION" +
                ", MAX(DistinctInventory.ALREADY_USED) AS ALREADY_USED" +
                ", MAX(DistinctInventory.COST_NAME) AS COST_NAME" +
                ", MAX(DistinctInventory.PUTAWAY) AS PUTAWAY" +
                ", MAX(DistinctInventory.PREV_INV) AS PREV_INV " +
                "FROM XCUST RIGHT OUTER JOIN " +
                "XPRTS INNER JOIN " +
                "XRECE ON XPRTS.CODE = XRECE.CODE INNER JOIN " +
                "XPRTS XPRTS_1 ON XPRTS.KCODE = XPRTS_1.CODE LEFT OUTER JOIN " +
                "XHEAD ON XPRTS_1.KCODE = XHEAD.CODE LEFT OUTER JOIN " +
                "DistinctInventory ON XPRTS_1.KCODE = DistinctInventory.CODE ON XCUST.CUST = XRECE.CUST ";
            switch(optSearch.SelectedIndex)
            {
                case 0:
                    strSQL3+="WHERE (XRECE.CDATE Between '" +((DateTime)dtpFrom.EditValue).ToString("yyyyMMdd",dtfinfo)+ "1' And '" +((DateTime)dtpTo.EditValue).ToString("yyyyMMdd",dtfinfo)+ "1')";
                    break;
                case 1:
                    strSQL3+="WHERE (XRECE.PDATE Between '" +((DateTime)dtpFrom2.EditValue).ToString("yyyyMMdd",dtfinfo) +"1' And '" +((DateTime)dtpTo2.EditValue).ToString("yyyyMMdd",dtfinfo)+ "1')";
                    break;
                case 2:
                    strSQL3+="Where (XRECE.CONTRACT Like '"+txtContract.Text+"%')";
                    break;
            }
            strSQL3+=" AND (DistinctInventory.COST_NAME NOT LIKE 'CUT%') ";
            strSQL3 += "GROUP BY XRECE.CDATE,XRECE.NDATE,XRECE.PDATE,XRECE.CUST,XRECE.CONTRACT,XPRTS_1.KCODE,DistinctInventory.CUR_INV" +
                " ORDER BY XRECE.CDATE,XRECE.NDATE,XRECE.PDATE,XRECE.CUST,XRECE.CONTRACT,XPRTS_1.KCODE";
            //---------------------------------------------------------------------------------------------------------------------------------
            switch(intType)
            {
                case 0://Fabric
                    strSQL = strSQL1;
                    break;
                case 1://Accessory
                    strSQL = strSQL2 + ";" + strSQL3;
                    break;
                case 2://Accessory Pack
                    strSQL = strSQL2;
                    break;
                case 3://Acessory Sew
                    strSQL = strSQL3;
                    break;
            }

            DataSet ds = db.GetDataSet(strSQL);
            DataTable dt = ds.Tables[0];
            if(intType==1){ dt.Merge(ds.Tables[1]);}
            gridControl1.DataSource=dt;
            gridView1.Columns["ALREADY_USED"].Visible = false;
            gridView1.Columns["COST_NAME"].Visible = false;
            gridView1.Columns["PUTAWAY"].Visible = false;
            gridView1.Columns["PREV_INV"].Visible = false;
            if(chkMerge2.Checked)
                gridView1.OptionsView.AllowCellMerge = true;
            else
                gridView1.OptionsView.AllowCellMerge = false;
            gridView1.OptionsView.ShowFooter = true;
            gridView1.Columns["USED"].SummaryItem.FieldName = "USED";
            gridView1.Columns["USED"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n4}");
            gridView1.Columns["INVENTORY"].SummaryItem.FieldName = "INVENTORY";
            gridView1.Columns["INVENTORY"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n4}");
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            StatusBarEvent(gridView1.DataRowCount+" Rows.");
        }
        private void DisplayData_Detail(int intType)
        {
            string strSQL= "";
            string strSQL1;//Fabric
            string strSQL2;//Accessory Pack
            string strSQL3;//Acessory Sew
            //------------------------------------------------------Fabric---------------------------------------------------------------------------------------------------------------------------------------------------
            strSQL1 = "SELECT SUBSTRING(XRECE.CDATE, 1, 8) AS SHIP_DATE,SUBSTRING(XRECE.NDATE, 1, 8) AS PACK_DATE,SUBSTRING(XRECE.PDATE, 1, 8) AS SEW_DATE,XRECE.CUST,XRECE.CONTRACT" +
                ",XRECE.EDA AS BRANCH,XRECE.CODE AS STYLE ,XRECE.KVOL AS QTY" +
                ",XPRTS_3.KCODE AS MATERIAL,MAX(XHEAD.NAME) AS MATERIAL_NAME,XPRTS_3.SIYOU AS BOM, XPRTS_3.SIYOUW AS BOM_DIV"+
                ", SUM(XZAIK.ZAIK) AS INVENTORY,XHEAD.MAINBUMO " +
                "FROM XZAIK INNER JOIN " +
                "XPRTS XPRTS_3 ON XZAIK.CODE = XPRTS_3.KCODE INNER JOIN " +
                "XRECE INNER JOIN " +
                "XPRTS XPRTS_2 INNER JOIN " +
                "XPRTS XPRTS_1 ON XPRTS_2.CODE = XPRTS_1.KCODE ON XRECE.CODE = XPRTS_1.CODE ON XPRTS_3.CODE = XPRTS_2.KCODE " +
                " INNER JOIN XHEAD ON XPRTS_3.KCODE = XHEAD.CODE ";
            switch(optSearch.SelectedIndex)
            {
                case 0:  //Due date
                    strSQL1+= "WHERE (XRECE.CDATE Between '" +((DateTime)dtpFrom.EditValue).ToString("yyyyMMdd",dtfinfo) + "1' And '" +((DateTime)dtpTo.EditValue).ToString("yyyyMMdd",dtfinfo) + "1')";
                    break;
                case 1:  //Sew plan
                    throw new ApplicationException("");
                    //strSQL = strSQL & "WHERE " &  & ".XRECE.SEWPLAN Between '" & Format(dtpFrom2.EditValue, "yyyyMMdd") & "' And '" & Format(dtpTo2.EditValue, "yyyyMMdd") & "')"""
                    //break;
                case 2://Contract
                    strSQL1 += "Where (XRECE.CONTRACT Like '" + txtContract.Text + "%')";
                    break;
            }
            strSQL1 += " AND (XZAIK.JYOGAI = 0) " +
                "GROUP BY XRECE.CDATE,XRECE.NDATE,XRECE.PDATE,XRECE.CUST,XRECE.CONTRACT,XRECE.EDA,XRECE.CODE,XRECE.KVOL,XPRTS_3.KCODE,XPRTS_3.SIYOU,XPRTS_3.SIYOUW"+
                ",XHEAD.MAINBUMO ";
            strSQL1 += "ORDER BY XRECE.CDATE,XRECE.NDATE,XRECE.PDATE,XRECE.CUST,XRECE.CONTRACT,XRECE.EDA";
            //------------------------------------------------------------Accessory Pack----------------------------------------------------------------------------------------------------------------------------------
            strSQL2 = "SELECT SUBSTRING(XRECE.CDATE, 1, 8) AS SHIP_DATE,SUBSTRING(XRECE.NDATE, 1, 8) AS PACK_DATE,SUBSTRING(XRECE.PDATE, 1, 8) AS SEW_DATE,XRECE.CUST,XRECE.CONTRACT" +
                ",XRECE.EDA AS BRANCH,XRECE.CODE AS STYLE,MAX(XRECE.KVOL) AS QTY" +
                ",XPRTS_1.KCODE AS MATERIAL,MAX(XHEAD.NAME) AS MATERIAL_NAME, MAX(XPRTS_1.SIYOU) AS BOM,MAX(XPRTS_1.SIYOUW) AS BOM_DIV, SUM(XZAIK.ZAIK) AS INVENTORY " +
                ",(SELECT CASE WHEN B.KANZANV=1 THEN A.PRICE/B.KANZANK "+
		        "              WHEN B.KANZANV=2 THEN A.PRICE*B.KANZANK "+
		        "              ELSE A.PRICE END "+
                "  FROM XTANK A INNER JOIN XITEM B ON A.CODE=B.CODE AND A.VENDOR=B.BUMO INNER JOIN XHEAD C ON A.CODE=C.CODE AND A.VENDOR=C.MAINBUMO "+
                "  WHERE A.CODE=XPRTS_1.KCODE) AS LATEST_PRICE " +
                ",(SELECT B.CURRE FROM XHEAD A INNER JOIN XSECT B ON A.MAINBUMO=B.BUMO WHERE A.CODE=XPRTS_1.KCODE) AS CURRE "+
                ",(SELECT CASE WHEN B.KANZANV=1 THEN B.TANI2 "+
		        "              WHEN B.KANZANV=2 THEN B.TANI2 "+
		        "              ELSE C.TANI1 END "+
                "  FROM XTANK A	INNER JOIN XITEM B ON A.CODE=B.CODE AND A.VENDOR=B.BUMO INNER JOIN XHEAD C ON A.CODE=C.CODE AND A.VENDOR=C.MAINBUMO "+
                "  WHERE A.CODE=XPRTS_1.KCODE) AS UNIT "+
                ",XITEM.FURYOU AS DEFFECT,XHEAD.MAINBUMO " +
                "FROM XRECE "+
                "INNER JOIN XPRTS XPRTS_1 ON XRECE.CODE = XPRTS_1.CODE "+
                "INNER JOIN XZAIK ON XPRTS_1.KCODE = XZAIK.CODE " +
                "INNER JOIN XHEAD ON XPRTS_1.KCODE = XHEAD.CODE "+
                "INNER JOIN XITEM ON XHEAD.CODE=XITEM.CODE AND XHEAD.MAINBUMO=XITEM.BUMO ";
            switch(optSearch.SelectedIndex)
            {
                case 0:  //Due date
                    strSQL2 += "WHERE (XRECE.CDATE Between '" +((DateTime)dtpFrom.EditValue).ToString("yyyyMMdd",dtfinfo) + "1' And '" +((DateTime)dtpTo.EditValue).ToString("yyyyMMdd",dtfinfo) + "1')";
                    break;
                case 1:  //Sew plan
                    throw new ApplicationException("");
                    //strSQL = strSQL & "WHERE " &  & ".XRECE.SEWPLAN Between '" & Format(dtpFrom2.EditValue, "yyyyMMdd") & "' And '" & Format(dtpTo2.EditValue, "yyyyMMdd") & "')"""
                    //break;  
                case 2:  //Contract
                    strSQL2 += "Where (XRECE.CONTRACT Like '" + txtContract.Text + "%') ";
                    break;
            }
            strSQL2 += " AND (XZAIK.JYOGAI = 0) AND (NOT (SUBSTRING(XZAIK.GENKA, 1, 3) = 'SEW')) ";
            strSQL2 += "GROUP BY XRECE.CDATE,XRECE.NDATE,XRECE.PDATE,XRECE.CUST,XRECE.CONTRACT,XRECE.EDA,XRECE.CODE, XPRTS_1.KCODE,XPRTS_1.EDA,XITEM.FURYOU,XHEAD.MAINBUMO ";
            strSQL2 += "ORDER BY XRECE.CDATE,XRECE.NDATE,XRECE.PDATE,XRECE.CUST,XRECE.CONTRACT,XRECE.EDA";
            //------------------------------------------------------------Accessory Sew----------------------------------------------------------------------------------------------------------------------------------
            strSQL3 = "SELECT SUBSTRING(XRECE.CDATE, 1, 8) AS SHIP_DATE,SUBSTRING(XRECE.NDATE, 1, 8) AS PACK_DATE,SUBSTRING(XRECE.PDATE, 1, 8) AS SEW_DATE,XRECE.CUST,XRECE.CONTRACT" +
                ",XRECE.EDA AS BRANCH,XRECE.CODE AS STYLE,MAX(XRECE.KVOL) AS QTY" +
                ",XPRTS_2.KCODE AS MATERIAL,MAX(XHEAD.NAME) AS MATERIAL_NAME, MAX(XPRTS_2.SIYOU) AS BOM,MAX(XPRTS_2.SIYOUW) AS BOM_DIV, SUM(XZAIK.ZAIK) AS INVENTORY " +
                ",(SELECT CASE WHEN B.KANZANV=1 THEN A.PRICE/B.KANZANK "+
		        "              WHEN B.KANZANV=2 THEN A.PRICE*B.KANZANK "+
		        "              ELSE A.PRICE END "+
                "  FROM XTANK A INNER JOIN XITEM B ON A.CODE=B.CODE AND A.VENDOR=B.BUMO INNER JOIN XHEAD C ON A.CODE=C.CODE AND A.VENDOR=C.MAINBUMO "+
                "  WHERE A.CODE=XPRTS_2.KCODE) AS LATEST_PRICE " +
                ",(SELECT B.CURRE FROM XHEAD A INNER JOIN XSECT B ON A.MAINBUMO=B.BUMO WHERE A.CODE=XPRTS_2.KCODE) AS CURRE " +
                ",(SELECT CASE WHEN B.KANZANV=1 THEN B.TANI2 " +
                "              WHEN B.KANZANV=2 THEN B.TANI2 " +
                "              ELSE C.TANI1 END " +
                "  FROM XTANK A	INNER JOIN XITEM B ON A.CODE=B.CODE AND A.VENDOR=B.BUMO INNER JOIN XHEAD C ON A.CODE=C.CODE AND A.VENDOR=C.MAINBUMO " +
                "  WHERE A.CODE=XPRTS_2.KCODE) AS UNIT " +
                ",XITEM.FURYOU AS DEFFECT,XHEAD.MAINBUMO "+
                "FROM XRECE "+
                "INNER JOIN XPRTS XPRTS_2 "+
                "INNER JOIN XPRTS XPRTS_1 ON XPRTS_2.CODE = XPRTS_1.KCODE ON XRECE.CODE = XPRTS_1.CODE "+
                "INNER JOIN XZAIK ON XPRTS_2.KCODE = XZAIK.CODE " +
                "INNER JOIN XHEAD ON XPRTS_2.KCODE = XHEAD.CODE "+
                "INNER JOIN XITEM ON XHEAD.CODE=XITEM.CODE AND XHEAD.MAINBUMO=XITEM.BUMO ";
            switch(optSearch.SelectedIndex)
            {
                case 0:  //Due date
                    strSQL3 += "WHERE (XRECE.CDATE Between '" + ((DateTime)dtpFrom.EditValue).ToString("yyyyMMdd",dtfinfo) + "1' And '" +((DateTime)dtpTo.EditValue).ToString("yyyyMMdd",dtfinfo) + "1')";
                    break;
                case 1:  //Sew plan
                    throw new ApplicationException("");
                    //strSQL = strSQL & "WHERE " &  & ".XRECE.SEWPLAN Between '" & Format(dtpFrom2.EditValue, "yyyyMMdd") & "' And '" & Format(dtpTo2.EditValue, "yyyyMMdd") & "')"""
                    //break;
                case 2:  //Contract
                    strSQL3 +=  "Where (XRECE.CONTRACT Like '" + txtContract.Text + "%') ";
                    break;
            }
            strSQL3 += " AND (XZAIK.JYOGAI = 0) AND (NOT (SUBSTRING(XZAIK.GENKA, 1, 3) = 'CUT')) ";
            strSQL3 += "GROUP BY XRECE.CDATE,XRECE.NDATE,XRECE.PDATE,XRECE.CUST,XRECE.CONTRACT,XRECE.EDA,XRECE.CODE, XPRTS_2.KCODE,XPRTS_2.EDA,XITEM.FURYOU,XHEAD.MAINBUMO ";
            strSQL3 += "ORDER BY XRECE.CDATE,XRECE.NDATE,XRECE.PDATE,XRECE.CUST,XRECE.CONTRACT,XRECE.EDA";
            //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            switch(intType)
            {
                case 0:  //Fabric
                    strSQL = strSQL1;
                    break;  
                case 1:  //Accessory
                    strSQL = strSQL2 + ";" + strSQL3;
                    break;
                case 2: //Accessory Pack
                    strSQL = strSQL2;
                    break;
                case 3:  //Acessory Sew
                    strSQL = strSQL3;
                    break;
            }

            DataSet ds = db.GetDataSet(strSQL);
            DataTable dt = ds.Tables[0];
            if(intType == 1){ dt.Merge(ds.Tables[1]);}
            dt.Columns.Add("TOTAL",typeof(double), "QTY * (BOM/BOM_DIV)");
            dt.Columns.Add("TOTAL+DF", typeof(double), "TOTAL+(0.01*DEFFECT*TOTAL)");
            gridControl2.DataSource = dt;
            gridView2.Columns["TOTAL"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView2.Columns["TOTAL"].DisplayFormat.FormatString = "n4";
            gridView2.Columns["TOTAL"].VisibleIndex = 10;
            gridView2.Columns["TOTAL+DF"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView2.Columns["TOTAL+DF"].DisplayFormat.FormatString = "n4";
            gridView2.Columns["TOTAL+DF"].VisibleIndex = 11;
            gridView2.Columns["DEFFECT"].VisibleIndex = 12;
            gridView2.OptionsView.AllowCellMerge =(chkMerge.Checked)? true:false;
            gridView2.OptionsView.ShowFooter = true;
            gridView2.Columns["TOTAL"].SummaryItem.FieldName = "TOTAL";
            gridView2.Columns["TOTAL"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n4}");
            gridView2.Columns["TOTAL+DF"].SummaryItem.FieldName = "TOTAL+DF";
            gridView2.Columns["TOTAL+DF"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n4}");
            //gridView2.Columns["DEFFECT"].Visible = false;
            gridView2.OptionsView.EnableAppearanceOddRow = true;
            gridView2.OptionsView.ColumnAutoWidth = false;
            gridView2.BestFitColumns();
            StatusBarEvent(gridView2.DataRowCount + " Rows.");
        }
        private Color SetGridColor(string iDate)
        {
            DateTime datTemp=new DateTime(Convert.ToInt16(iDate.Substring(0,4)),Convert.ToInt16(iDate.Substring(4,2)),Convert.ToInt16(iDate.Substring(6,2)));
            switch(datTemp.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return Color.Red;
                    //break;
                case DayOfWeek.Monday:
                    return Color.LemonChiffon;
                    //break;
                case DayOfWeek.Tuesday:
                    return Color.Pink;
                    //break;
                case DayOfWeek.Wednesday:
                    return Color.LightGreen;
                    //break;
                case DayOfWeek.Thursday:
                    return Color.LightSalmon;
                    //break;
                case DayOfWeek.Friday:
                    return Color.LightSkyBlue;
                    //break;
                case DayOfWeek.Saturday:
                    return Color.Plum;
                    //break;
                default:
                    return Color.Black;
                    //break;
            }
        }

        private void frmMaterial_Consumption_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo=clinfo.DateTimeFormat;
            dtpFrom.EditValue =DateTime.Today;
            dtpFrom2.EditValue = DateTime.Today;
            dtpTo.EditValue = DateTime.Today;
            dtpTo2.EditValue = DateTime.Today;
            cboMaterial.SelectedIndex = 0;
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if(e.Info.IsRowIndicator){e.Info.DisplayText=(e.RowHandle+1).ToString();}
            gridView1.IndicatorWidth=45;
        }
        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if(e.Info.IsRowIndicator){e.Info.DisplayText=(e.RowHandle+1).ToString();}        
            gridView2.IndicatorWidth=45;
        }
        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if(e.Column.FieldName=="SHIP_DATE"||e.Column.FieldName=="PACK_DATE"||e.Column.FieldName=="SEW_DATE")
            {
                e.Column.AppearanceCell.BackColor=SetGridColor(e.CellValue.ToString());
            }
        }
        private void gridView2_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "SHIP_DATE" || e.Column.FieldName == "PACK_DATE" || e.Column.FieldName == "SEW_DATE")
            {
                e.Column.AppearanceCell.BackColor=SetGridColor(e.CellValue.ToString());
            }
        }
   









    }
}