using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Globalization;
using Microsoft.Office.Interop.Excel;
using System.Linq;
using myClass;
using System.Reflection;

namespace TUW_System.TS1
{
    public partial class frmTS1_Holding : DevExpress.XtraEditors.XtraForm
    {
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        cDatabase db;
        System.Data.DataTable dtResult;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmTS1_Holding()
        {
            InitializeComponent();
        }
        public void ClearData()
        {
            gridControl1.DataSource = null;
        }
        public void SaveData()
        { 
        
        }
        #region "DisplayData"
        public void DisplayData()
        {
            switch (cboProcess.Text)
            { 
                case "FG":
                    DisplayData_FG();
                    break;
                case "CUT":
                    DisplayData_Process("CUT");
                    break;
                case "SEW":
                    DisplayData_Process("SEW");
                    break;
                case "PACK":
                    DisplayData_Process("PACK");
                    break;
                default:
                    break;
            }
        }
        private void DisplayData_FG()
        {
            this.Cursor = Cursors.WaitCursor;
            //try
            //{
            string strSQL;
            if (cboCustomer.Text == "BROOKS BROTHERS")
            {
                strSQL = "EXEC spTPiCSSubsystem_HoldingStockFG";
            }
            else//C&A
            {
                strSQL = "EXEC spTPiCSSubsystem_HoldingStockFG_CA";
            }
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            if (dt != null)
            {
                //เอาคอลัมน์วันที่ที่มีค่าจำนวนเป็น null ในทุกๆไอเท็มออก
                for (int i = dt.Columns["31"].Ordinal; i > dt.Columns["PACK_INV"].Ordinal; i--)//    dt.Columns.Count-1;i>11; i--)
                {
                    bool isRemove = true;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[i] != System.DBNull.Value)
                        {
                            isRemove = false;
                            break;
                        }
                    }
                    if (isRemove == true) { dt.Columns.RemoveAt(i); }
                }
                ////เพิ่มคอลัมน์จำนวนที่ต้องเปิดบิลสั่งซื้อ
                //DataColumn dc = new DataColumn();
                //dc.ColumnName = "BALANCE_ORDER";
                //dc.DataType = typeof(System.Decimal);
                //dt.Columns.Add(dc);
                ////เพิ่มคอลัมน์แสดงสถานะให้ฝ่ายจัดซื้อ
                //dc = new DataColumn();
                //dc.ColumnName = "STATUS";
                //dc.DataType = typeof(System.Decimal);
                //dt.Columns.Add(dc);
                ////เพิ่มคอลัมน์ cut balance,sew balance,pack balance
                //dc = new DataColumn();
                //dc.ColumnName = "CUT_BALANCE";
                //dc.DataType = typeof(System.Decimal);
                //dt.Columns.Add(dc);
                //dc = new DataColumn();
                //dc.ColumnName = "SEW_BALANCE";
                //dc.DataType = typeof(System.Decimal);
                //dt.Columns.Add(dc);
                //คำนวนค่าในคอลัมน์ Result,Status
                foreach (DataRow dr in dt.Rows)
                {
                    decimal zForecast, zForecast2, zForecast3, zForecast4, zForecast5, zForecast6;
                    decimal zConfirm, zConfirm2, zConfirm3, zConfirm4, zConfirm5, zConfirm6;
                    decimal zDelay, zBalance_ship, zCut_Inv, zSew_Inv, zPack_Inv, zCut_WO, zSew_WO, zPack_WO;
                    zForecast = (dr["FORECAST"] == System.DBNull.Value) ? 0 : (decimal)dr["FORECAST"];
                    zForecast2 = (dr["FORECAST2"] == System.DBNull.Value) ? 0 : (decimal)dr["FORECAST2"];
                    zForecast3 = (dr["FORECAST3"] == System.DBNull.Value) ? 0 : (decimal)dr["FORECAST3"];
                    zForecast4 = (dr["FORECAST4"] == System.DBNull.Value) ? 0 : (decimal)dr["FORECAST4"];
                    zForecast5 = (dr["FORECAST5"] == System.DBNull.Value) ? 0 : (decimal)dr["FORECAST5"];
                    zForecast6 = (dr["FORECAST6"] == System.DBNull.Value) ? 0 : (decimal)dr["FORECAST6"];

                    zConfirm = (dr["CONFIRM"] == System.DBNull.Value) ? 0 : (decimal)dr["CONFIRM"];
                    zConfirm2 = (dr["CONFIRM2"] == System.DBNull.Value) ? 0 : (decimal)dr["CONFIRM2"];
                    zConfirm3 = (dr["CONFIRM3"] == System.DBNull.Value) ? 0 : (decimal)dr["CONFIRM3"];
                    zConfirm4 = (dr["CONFIRM4"] == System.DBNull.Value) ? 0 : (decimal)dr["CONFIRM4"];
                    zConfirm5 = (dr["CONFIRM5"] == System.DBNull.Value) ? 0 : (decimal)dr["CONFIRM5"];
                    zConfirm6 = (dr["CONFIRM6"] == System.DBNull.Value) ? 0 : (decimal)dr["CONFIRM6"];

                    zDelay = (dr["DELAY"] == System.DBNull.Value) ? 0 : (decimal)dr["DELAY"];
                    zBalance_ship = (dr["BALANCE_SHIP"] == System.DBNull.Value) ? 0 : (decimal)dr["BALANCE_SHIP"];
                    zCut_Inv = (dr["CUT_INV"] == System.DBNull.Value) ? 0 : (decimal)dr["CUT_INV"];
                    zSew_Inv = (dr["SEW_INV"] == System.DBNull.Value) ? 0 : (decimal)dr["SEW_INV"];
                    zPack_Inv = (dr["PACK_INV"] == System.DBNull.Value) ? 0 : (decimal)dr["PACK_INV"];
                    zCut_WO = (dr["CUT_WO"] == System.DBNull.Value) ? 0 : (decimal)dr["CUT_WO"];
                    zSew_WO = (dr["SEW_WO"] == System.DBNull.Value) ? 0 : (decimal)dr["SEW_WO"];
                    zPack_WO = (dr["PACK_WO"] == System.DBNull.Value) ? 0 : (decimal)dr["PACK_WO"];


                    dr["BALANCE_AFTER_SHIP"] = ((zCut_Inv + zSew_Inv + zPack_Inv) - (zDelay + zBalance_ship)) + zCut_WO;
                    dr["CUT_BALANCE"] = ((zCut_Inv + zSew_Inv + zPack_Inv) - (zDelay + zForecast + zBalance_ship)) + zCut_WO;
                    dr["SEW_BALANCE"] = ((zCut_Inv + zSew_Inv + zPack_Inv) - (zDelay + zForecast + zBalance_ship)) + zSew_WO;
                    dr["BALANCE_STOCK"] = (zCut_Inv + zSew_Inv + zPack_Inv) - (zDelay + zForecast + zBalance_ship);
                    dr["BALANCE_STOCK2"] = (zConfirm2 > 0) ? Convert.ToDecimal(dr["BALANCE_AFTER_SHIP"]) - (zForecast2 + zConfirm2) : Convert.ToDecimal(dr["BALANCE_AFTER_SHIP"]) - zForecast2;
                    dr["BALANCE_STOCK3"] = (zConfirm3 > 0) ? Convert.ToDecimal(dr["BALANCE_STOCK2"]) - (zForecast3 + zConfirm3) : Convert.ToDecimal(dr["BALANCE_STOCK2"]) - zForecast3;
                    dr["BALANCE_STOCK4"] = (zConfirm4 > 0) ? Convert.ToDecimal(dr["BALANCE_STOCK3"]) - (zForecast4 + zConfirm4) : Convert.ToDecimal(dr["BALANCE_STOCK3"]) - zForecast4;
                    dr["BALANCE_STOCK5"] = (zConfirm5 > 0) ? Convert.ToDecimal(dr["BALANCE_STOCK4"]) - (zForecast5 + zConfirm5) : Convert.ToDecimal(dr["BALANCE_STOCK4"]) - zForecast5;
                    dr["BALANCE_STOCK6"] = (zConfirm6 > 0) ? Convert.ToDecimal(dr["BALANCE_STOCK5"]) - (zForecast6 + zConfirm6) : Convert.ToDecimal(dr["BALANCE_STOCK5"]) - zForecast6;
                }
                ////เซตตำแหน่งคอลัมน์
                //dt.Columns["STATUS"].SetOrdinal(7);
                //dt.Columns["CUT_BALANCE"].SetOrdinal(10);
                //dt.Columns["SEW_BALANCE"].SetOrdinal(13);
                //เซตหัวคอลัมน์
                dt.Columns["BALANCE_STOCK"].Caption = "ต้องผลิต";
                dt.Columns["BALANCE_STOCK2"].Caption = "ต้องผลิต";
                dt.Columns["BALANCE_STOCK3"].Caption = "ต้องผลิต";
                dt.Columns["BALANCE_STOCK4"].Caption = "ต้องผลิต";
                dt.Columns["BALANCE_STOCK5"].Caption = "ต้องผลิต";
                dt.Columns["BALANCE_STOCK6"].Caption = "ต้องผลิต";
                gridControl1.DataSource = dt;
                gridView1.PopulateColumns();
                gridView1.OptionsView.EnableAppearanceEvenRow = true;
                gridView1.OptionsView.EnableAppearanceOddRow = true;
                gridView1.OptionsView.ColumnAutoWidth = false;
                gridView1.BestFitColumns();
            }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            this.Cursor = Cursors.Default;
        }
        private void DisplayData_Process(string strProcess)
        {
            this.Cursor = Cursors.WaitCursor;
            //try
            //{
            string strSQL="";
            if (cboCustomer.Text == "BROOKS BROTHERS")
            {
                switch (strProcess)
                { 
                    case "CUT":
                        strSQL = "EXEC spTUWSystem_HoldingStockCut";
                        break;
                    case "SEW":
                        strSQL = "EXEC spTUWSystem_HoldingStockSew";
                        break;
                    case "PACK":
                        strSQL = "EXEC spTUWSystem_HoldingStockPack";
                        break;
                }
            }
            else//C&A
            {
                switch (strProcess)
                {
                    case "CUT":
                        strSQL = "EXEC spTUWSystem_HoldingStockCut_CA";
                        break;
                    case "SEW":
                        strSQL = "EXEC spTUWSystem_HoldingStockSew_CA";
                        break;
                    case "PACK":
                        strSQL = "EXEC spTUWSystem_HoldingStockPack_CA";
                        break;
                }
            }
            dtResult= db.GetDataTable(strSQL);
            if (dtResult != null)
            {
                //คำนวนค่าในคอลัมน์ Result,Status
                foreach (DataRow dr in dtResult.Rows)
                {
                    //-------------------------------FG-------------------------------------------------
                    decimal zForecast, zForecast2, zForecast3, zForecast4, zForecast5;
                    decimal zConfirm2, zConfirm3, zConfirm4, zConfirm5;
                    decimal zDelay,zCut_Inv, zSew_Inv, zPack_Inv,zBalance_ship;
                    decimal zCut_WO=0;
                    decimal zSew_WO=0;
                    decimal zPack_WO=0;
                    zForecast = (dr["FORECAST"] == System.DBNull.Value) ? 0 : (decimal)dr["FORECAST"];
                    zForecast2 = zForecast;
                    zForecast3 = zForecast;
                    zForecast4 = zForecast;
                    zForecast5 = zForecast;
                    zConfirm2 = (dr["CONFIRM2"] == System.DBNull.Value) ? 0 : (decimal)dr["CONFIRM2"];
                    zConfirm3 = (dr["CONFIRM3"] == System.DBNull.Value) ? 0 : (decimal)dr["CONFIRM3"];
                    zConfirm4 = (dr["CONFIRM4"] == System.DBNull.Value) ? 0 : (decimal)dr["CONFIRM4"];
                    zConfirm5 = (dr["CONFIRM5"] == System.DBNull.Value) ? 0 : (decimal)dr["CONFIRM5"];
                    
                    zDelay = (dr["DELAY"] == System.DBNull.Value) ? 0 : (decimal)dr["DELAY"];
                    zBalance_ship = (dr["BALANCE_SHIP"] == System.DBNull.Value) ? 0 : (decimal)dr["BALANCE_SHIP"];
                    zCut_Inv = (dr["CUT_INV"] == System.DBNull.Value) ? 0 : (decimal)dr["CUT_INV"];
                    zSew_Inv = (dr["SEW_INV"] == System.DBNull.Value) ? 0 : (decimal)dr["SEW_INV"];
                    zPack_Inv = (dr["PACK_INV"] == System.DBNull.Value) ? 0 : (decimal)dr["PACK_INV"];
                    switch (strProcess)
                    { 
                        case "CUT":
                            zCut_WO = (dr["CUT_WO"] == System.DBNull.Value) ? 0 : (decimal)dr["CUT_WO"];
                            dr["BALANCE_AFTER_SHIP"] = ((zCut_Inv + zSew_Inv + zPack_Inv) - (zDelay + zBalance_ship)) + zCut_WO;
                            break;
                        case "SEW":
                            zSew_WO = (dr["SEW_WO"] == System.DBNull.Value) ? 0 : (decimal)dr["SEW_WO"];
                            dr["BALANCE_AFTER_SHIP"] = ((zCut_Inv + zSew_Inv + zPack_Inv) - (zDelay + zBalance_ship)) + zSew_WO;
                            break;
                        case "PACK":
                            zPack_WO = (dr["PACK_WO"] == System.DBNull.Value) ? 0 : (decimal)dr["PACK_WO"];
                            break;
                    }

                    //dr["BALANCE_STOCK"] = (zCut_Inv + zSew_Inv + zPack_Inv) - (zDelay + zForecast + zBalance_ship);
                    //dr["CUT_BALANCE"] = ((zCut_Inv + zSew_Inv + zPack_Inv) - (zDelay + zForecast + zBalance_ship))+zCut_WO;
                    //dr["SEW_BALANCE"] = ((zCut_Inv + zSew_Inv + zPack_Inv) - (zDelay + zForecast + zBalance_ship)) + zSew_WO;
                    //-------------------------------Mat month 1-------------------------------------------------
                    //if (Convert.ToDecimal(dr["CUT_BALANCE"]) < 0)
                    //{
                    //    dr["MAT_PARENT_ORDER"] = zCut_WO + Math.Abs(Convert.ToDecimal(dr["CUT_BALANCE"]));
                    //}
                    //else
                    //{
                    switch (strProcess)
                    { 
                        case "CUT":
                            dr["MAT_PARENT_ORDER"] = zCut_WO;
                            break;
                        case "SEW":
                            dr["MAT_PARENT_ORDER"] = zSew_WO;
                            break;
                        case "PACK":
                            dr["MAT_PARENT_ORDER"] = zBalance_ship+zDelay;
                            break;
                    }
                    dr["MAT_USE_PLAN"] = Convert.ToDecimal(dr["MAT_PARENT_ORDER"]) * Convert.ToDecimal(dr["CONSUMPTION"]);
                    //-------------------------------Mat month 2-------------------------------------------------
                    switch (strProcess)
                    { 
                        case "CUT":
                        case "SEW":
                            dr["BALANCE_STOCK2"] = (zConfirm2 > 0) ? Convert.ToDecimal(dr["BALANCE_AFTER_SHIP"]) - (zForecast2 + zConfirm2) : Convert.ToDecimal(dr["BALANCE_AFTER_SHIP"]) - zForecast2;
                            if ((decimal)dr["BALANCE_STOCK2"] < 0)
                            {
                                dr["MAT_PARENT_ORDER2"] =Math.Abs((decimal)dr["BALANCE_STOCK2"]);
                            }
                            break;
                        case "PACK":
                            dr["BALANCE_STOCK2"] = zConfirm2;
                            dr["MAT_PARENT_ORDER2"] = dr["BALANCE_STOCK2"];
                            break;
                    }
                    dr["MAT_USE_PLAN2"] =Convert.ToDecimal(dr["MAT_PARENT_ORDER2"]) * Convert.ToDecimal(dr["CONSUMPTION"]);
                    //-------------------------------Mat month 3-------------------------------------------------
                    switch (strProcess)
                    { 
                        case "CUT":
                        case "SEW":
                            dr["BALANCE_STOCK3"] = (zConfirm3 > 0) ? (decimal)dr["BALANCE_STOCK2"] - (zForecast3 + zConfirm3) : (decimal)dr["BALANCE_STOCK2"] - zForecast3;
                            if ((decimal)dr["BALANCE_STOCK3"] < 0)
                            {
                                dr["MAT_PARENT_ORDER3"] =Math.Abs((decimal)dr["BALANCE_STOCK3"]);
                            }
                            break;
                        case "PACK":
                            dr["BALANCE_STOCK3"] = zConfirm3;
                            dr["MAT_PARENT_ORDER3"] = dr["BALANCE_STOCK3"];
                            break;
                    }
                    dr["MAT_USE_PLAN3"] =Convert.ToDecimal(dr["MAT_PARENT_ORDER3"]) *Convert.ToDecimal(dr["CONSUMPTION"]);
                    //-------------------------------Mat month 4-------------------------------------------------
                    switch (strProcess)
                    {
                        case "CUT":
                        case "SEW":
                            dr["BALANCE_STOCK4"] = (zConfirm4 > 0) ? (decimal)dr["BALANCE_STOCK3"] - (zForecast4 + zConfirm4) : (decimal)dr["BALANCE_STOCK3"] - zForecast4;
                            if ((decimal)dr["BALANCE_STOCK4"] < 0)
                            {
                                dr["MAT_PARENT_ORDER4"] =Math.Abs((decimal)dr["BALANCE_STOCK4"]);
                            }
                            break;
                        case "PACK":
                            dr["BALANCE_STOCK4"] = zConfirm4;
                            dr["MAT_PARENT_ORDER4"] = dr["BALANCE_STOCK4"];
                            break;
                    }
                    dr["MAT_USE_PLAN4"] =Convert.ToDecimal(dr["MAT_PARENT_ORDER4"]) *Convert.ToDecimal(dr["CONSUMPTION"]);
                    //-------------------------------Mat month 5-------------------------------------------------
                    switch (strProcess)
                    {
                        case "CUT":
                        case "SEW":
                            dr["BALANCE_STOCK5"] = (zConfirm5 > 0) ? (decimal)dr["BALANCE_STOCK4"] - (zForecast5 + zConfirm5) : (decimal)dr["BALANCE_STOCK4"] - zForecast5;
                            if ((decimal)dr["BALANCE_STOCK5"] < 0)
                            {
                                dr["MAT_PARENT_ORDER5"] =Math.Abs((decimal)dr["BALANCE_STOCK5"]);
                            }
                            break;
                        case "PACK":
                            dr["BALANCE_STOCK5"] = zConfirm5;
                            dr["MAT_PARENT_ORDER5"] = dr["BALANCE_STOCK5"];
                            break;
                    }
                    dr["MAT_USE_PLAN5"] =Convert.ToDecimal(dr["MAT_PARENT_ORDER5"]) *Convert.ToDecimal(dr["CONSUMPTION"]);
                }
                gridControl1.DataSource = dtResult;
                gridView1.PopulateColumns();
                gridView1.Columns["DELAY"].Visible = false;
                gridView1.Columns["FORECAST"].Visible = false;
                gridView1.Columns["BALANCE_SHIP"].Visible = false;
                gridView1.Columns["BALANCE_AFTER_SHIP"].Visible = false;
                switch (cboProcess.Text)
                { 
                    case "CUT":
                        gridView1.Columns["CUT_WO"].Visible = false;
                        break;
                    case "SEW":
                        gridView1.Columns["SEW_WO"].Visible = false;
                        break;
                }
                gridView1.Columns["CUT_INV"].Visible = false;                
                gridView1.Columns["SEW_INV"].Visible = false;
                gridView1.Columns["PACK_INV"].Visible = false;
                gridView1.Columns["BALANCE_STOCK"].Visible = false;
                gridView1.Columns["CONFIRM2"].Visible = false;
                gridView1.Columns["BALANCE_STOCK2"].Visible = false;
                gridView1.Columns["CONFIRM3"].Visible = false;
                gridView1.Columns["BALANCE_STOCK3"].Visible = false;
                gridView1.Columns["CONFIRM4"].Visible = false;
                gridView1.Columns["BALANCE_STOCK4"].Visible = false;
                gridView1.Columns["CONFIRM5"].Visible = false;
                gridView1.Columns["BALANCE_STOCK5"].Visible = false;

                gridView1.OptionsView.EnableAppearanceEvenRow = true;
                gridView1.OptionsView.EnableAppearanceOddRow = true;
                gridView1.OptionsView.ColumnAutoWidth = false;
                gridView1.BestFitColumns();
                //-----------------------------grid sum--------------------------------------------------
                //ปรับค่าในฟิลด์ MAT_PO จากค่า null ให้เป็น 0 เพ่ื่อไม่ให้เกิด error
                foreach (DataRow dr in dtResult.Rows)
                {
                    if (dr["MAT_PO"] == System.DBNull.Value)
                    {
                        dr["MAT_PO"] = 0;
                    }
                }
                var result = from t in dtResult.AsEnumerable()
                             group t by new
                             {
                                 ITEM = t.Field<string>("ITEM"),
                                 ITEM_NAME = t.Field<string>("ITEM_NAME")
                             } into g
                             select new
                             {
                                 Item = g.Key.ITEM,
                                 Item_Name = g.Key.ITEM_NAME,
                                 Mat_Stock = g.Max(s => s.Field<decimal>("MAT_STOCK")),
                                 Mat_PO = g.Max(p => p.Field<decimal>("MAT_PO")),
                                 Mat_Parent_Order1 = g.Sum(o=>o.Field<decimal>("MAT_PARENT_ORDER")),
                                 Mat_Parent_Order2 = g.Sum(o2 => o2.Field<decimal>("MAT_PARENT_ORDER2")),
                                 Mat_Parent_Order3 = g.Sum(o3=> o3.Field<decimal>("MAT_PARENT_ORDER3")),
                                 Mat_Parent_Order4 = g.Sum(o4=> o4.Field<decimal>("MAT_PARENT_ORDER4")),
                                 Mat_Parent_Order5 = g.Sum(o5=> o5.Field<decimal>("MAT_PARENT_ORDER5")),
                                 Mat_Use_Plan1 = g.Sum(u => u.Field<decimal>("MAT_USE_PLAN")),
                                 Mat_Use_Plan2 = g.Sum(u2 => u2.Field<decimal>("MAT_USE_PLAN2")),
                                 Mat_Use_Plan3 = g.Sum(u3 => u3.Field<decimal>("MAT_USE_PLAN3")),
                                 Mat_Use_Plan4 = g.Sum(u4 => u4.Field<decimal>("MAT_USE_PLAN4")),
                                 Mat_Use_Plan5 = g.Sum(u5 => u5.Field<decimal>("MAT_USE_PLAN5"))
                             };

                System.Data.DataTable dtSum = result.CopyToDataTable2();
                DataColumn dc = new DataColumn();
                for (int i = 1; i < 6; i++)
                {
                    dc = new DataColumn();
                    dc.ColumnName = "Mat_Balance" + i;
                    dc.DataType = typeof(decimal);
                    dtSum.Columns.Add(dc);
                }
                foreach (DataRow dr in dtSum.Rows)
                {
                    dr["Mat_Balance1"] = ((decimal)dr["Mat_Stock"] + (decimal)dr["Mat_PO"]) - (decimal)dr["Mat_Use_Plan1"];
                    dr["Mat_Balance2"] = (decimal)dr["Mat_Balance1"] - (decimal)dr["Mat_Use_Plan2"];
                    dr["Mat_Balance3"] = (decimal)dr["Mat_Balance2"] - (decimal)dr["Mat_Use_Plan3"];
                    dr["Mat_Balance4"] = (decimal)dr["Mat_Balance3"] - (decimal)dr["Mat_Use_Plan4"];
                    dr["Mat_Balance5"] = (decimal)dr["Mat_Balance4"] - (decimal)dr["Mat_Use_Plan5"];
                }
                gridControl2.DataSource = dtSum;
                gridView2.PopulateColumns();
                gridView2.OptionsView.EnableAppearanceEvenRow = true;
                gridView2.OptionsView.EnableAppearanceOddRow = true;
                gridView2.OptionsView.ColumnAutoWidth = false;
                gridView2.BestFitColumns();
                //---------------------------------------------------------------------------------------

            }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            this.Cursor = Cursors.Default;
        }
        #endregion

        public void ExportExcel()
        {
            if (cboProcess.Text == "FG")
            {
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel._Workbook xlBook = xlApp.Workbooks.Open(System.Windows.Forms.Application.StartupPath + "\\Report\\TS1\\FG_C&A_HOLDING.xlt");
                Microsoft.Office.Interop.Excel.Sheets xlSheets = xlBook.Worksheets;
                Microsoft.Office.Interop.Excel._Worksheet xlSheet = (_Worksheet)xlSheets.get_Item(1);
                //xlApp.Visible = true;
                //ใส่วันที่ ship ครั้งที่ 1 และ 2
                xlSheet.Cells[9, 25] = gridView1.Columns[16].FieldName;
                xlSheet.Cells[9, 26] = gridView1.Columns[17].FieldName;
                int currentRow = 11;
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    currentRow++;
                    xlSheet.Cells[currentRow, 1] = gridView1.GetRowCellDisplayText(i, "STYLE") + "-" + gridView1.GetRowCellDisplayText(i, "SIZE") + "-" + gridView1.GetRowCellDisplayText(i, "COLOR");
                    xlSheet.Cells[currentRow, 2] = gridView1.GetRowCellDisplayText(i, "STYLE");
                    xlSheet.Cells[currentRow, 3] = gridView1.GetRowCellDisplayText(i, "COLOR");
                    xlSheet.Cells[currentRow, 4] = gridView1.GetRowCellDisplayText(i, "SIZE");
                    xlSheet.Cells[currentRow, 12] = gridView1.GetRowCellDisplayText(i, "DELAY");
                    xlSheet.Cells[currentRow, 13] = gridView1.GetRowCellDisplayText(i, "FORECAST");
                    xlSheet.Cells[currentRow, 14] = gridView1.GetRowCellDisplayText(i, "CONFIRM");
                    xlSheet.Cells[currentRow, 15] = gridView1.GetRowCellDisplayText(i, "BALANCE_SHIP");
                    xlSheet.Cells[currentRow, 16] = gridView1.GetRowCellDisplayText(i, "BALANCE_AFTER_SHIP");
                    xlSheet.Cells[currentRow, 17] = gridView1.GetRowCellDisplayText(i, "CUT_WO");
                    xlSheet.Cells[currentRow, 18] = gridView1.GetRowCellDisplayText(i, "CUT_INV");
                    xlSheet.Cells[currentRow, 19] = gridView1.GetRowCellDisplayText(i, "CUT_BALANCE");
                    xlSheet.Cells[currentRow, 20] = gridView1.GetRowCellDisplayText(i, "SEW_WO");
                    xlSheet.Cells[currentRow, 21] = gridView1.GetRowCellDisplayText(i, "SEW_INV");
                    xlSheet.Cells[currentRow, 22] = gridView1.GetRowCellDisplayText(i, "SEW_BALANCE");
                    xlSheet.Cells[currentRow, 23] = gridView1.GetRowCellDisplayText(i, "PACK_WO");
                    xlSheet.Cells[currentRow, 24] = gridView1.GetRowCellDisplayText(i, "PACK_INV");
                    xlSheet.Cells[currentRow, 25] = gridView1.GetRowCellDisplayText(i, gridView1.Columns[16]);
                    xlSheet.Cells[currentRow, 26] = gridView1.GetRowCellDisplayText(i, gridView1.Columns[17]);
                    xlSheet.Cells[currentRow, 27] = gridView1.GetRowCellDisplayText(i, "BALANCE_STOCK");
                    xlSheet.Cells[currentRow, 28] = gridView1.GetRowCellDisplayText(i, "FORECAST2");
                    xlSheet.Cells[currentRow, 29] = gridView1.GetRowCellDisplayText(i, "CONFIRM2");
                    xlSheet.Cells[currentRow, 32] = gridView1.GetRowCellDisplayText(i, "BALANCE_STOCK2");
                    xlSheet.Cells[currentRow, 33] = gridView1.GetRowCellDisplayText(i, "FORECAST3");
                    xlSheet.Cells[currentRow, 34] = gridView1.GetRowCellDisplayText(i, "CONFIRM3");
                    xlSheet.Cells[currentRow, 37] = gridView1.GetRowCellDisplayText(i, "BALANCE_STOCK3");
                    xlSheet.Cells[currentRow, 38] = gridView1.GetRowCellDisplayText(i, "FORECAST4");
                    xlSheet.Cells[currentRow, 39] = gridView1.GetRowCellDisplayText(i, "CONFIRM4");
                    xlSheet.Cells[currentRow, 42] = gridView1.GetRowCellDisplayText(i, "BALANCE_STOCK4");
                    xlSheet.Cells[currentRow, 43] = gridView1.GetRowCellDisplayText(i, "FORECAST5");
                    xlSheet.Cells[currentRow, 44] = gridView1.GetRowCellDisplayText(i, "CONFIRM5");
                    xlSheet.Cells[currentRow, 47] = gridView1.GetRowCellDisplayText(i, "BALANCE_STOCK5");


                }
                xlApp.Visible = true;
            }
            else
            {
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel._Workbook xlBook = xlApp.Workbooks.Open(System.Windows.Forms.Application.StartupPath + "\\Report\\TS1\\FG_C&A_HOLDING.xlt");
                Microsoft.Office.Interop.Excel.Sheets xlSheets = xlBook.Worksheets;
                Microsoft.Office.Interop.Excel._Worksheet xlSheet = (_Worksheet)xlSheets.get_Item(2);
                int currentRow = 9;
                for (int i = 0; i < gridView2.RowCount; i++)
                {
                    currentRow++;
                    //xlSheet.Cells[currentRow, 1] = gridView2.GetRowCellDisplayText(i, "CODE");
                    //xlSheet.Cells[currentRow, 2] = gridView2.GetRowCellDisplayText(i, "STYLE");
                    //xlSheet.Cells[currentRow, 3] = gridView2.GetRowCellDisplayText(i, "COLOR");
                    //xlSheet.Cells[currentRow, 4] = gridView2.GetRowCellDisplayText(i, "SIZE");
                    xlSheet.Cells[currentRow, 5] = gridView2.GetRowCellDisplayText(i, "Item");
                    xlSheet.Cells[currentRow, 6] = gridView2.GetRowCellDisplayText(i, "Item_Name");
                    //xlSheet.Cells[currentRow, 7] = gridView2.GetRowCellDisplayText(i, "CONSUMPTION");
                    //xlSheet.Cells[currentRow, 8] = gridView2.GetRowCellDisplayText(i, "PURCHASE_LEAD_TIME");
                    xlSheet.Cells[currentRow, 15] = gridView2.GetRowCellDisplayText(i, "Mat_Parent_Order1");
                    xlSheet.Cells[currentRow, 16] = gridView2.GetRowCellDisplayText(i, "Mat_Stock");
                    xlSheet.Cells[currentRow, 17] = gridView2.GetRowCellDisplayText(i, "Mat_PO");
                    xlSheet.Cells[currentRow, 18] = gridView2.GetRowCellDisplayText(i, "Mat_Use_Plan1");
                    xlSheet.Cells[currentRow, 19] = gridView2.GetRowCellDisplayText(i, "Mat_Balance1");
                    xlSheet.Cells[currentRow, 20] = gridView2.GetRowCellDisplayText(i, "Mat_Parent_Order2");
                    //xlSheet.Cells[currentRow, 21] = gridView2.GetRowCellDisplayText(i, "MAT_PO2");
                    xlSheet.Cells[currentRow, 22] = gridView2.GetRowCellDisplayText(i, "Mat_Use_Plan2");
                    xlSheet.Cells[currentRow, 23] = gridView2.GetRowCellDisplayText(i, "Mat_Balance2");
                    xlSheet.Cells[currentRow, 24] = gridView2.GetRowCellDisplayText(i, "Mat_Parent_Order3");
                    //xlSheet.Cells[currentRow, 25] = gridView2.GetRowCellDisplayText(i, "MAT_PO3");
                    xlSheet.Cells[currentRow, 26] = gridView2.GetRowCellDisplayText(i, "Mat_Use_Plan3");
                    xlSheet.Cells[currentRow, 27] = gridView2.GetRowCellDisplayText(i, "Mat_Balance3");
                    xlSheet.Cells[currentRow, 28] = gridView2.GetRowCellDisplayText(i, "Mat_Parent_Order4");
                    //xlSheet.Cells[currentRow, 29] = gridView2.GetRowCellDisplayText(i, "MAT_PO4");
                    xlSheet.Cells[currentRow, 30] = gridView2.GetRowCellDisplayText(i, "Mat_Use_Plan4");
                    xlSheet.Cells[currentRow, 31] = gridView2.GetRowCellDisplayText(i, "Mat_Balance4");
                    xlSheet.Cells[currentRow, 32] = gridView2.GetRowCellDisplayText(i, "Mat_Parent_Order5");
                    //xlSheet.Cells[currentRow, 33] = gridView2.GetRowCellDisplayText(i, "MAT_PO5");
                    xlSheet.Cells[currentRow, 34] = gridView2.GetRowCellDisplayText(i, "Mat_Use_Plan5");
                    xlSheet.Cells[currentRow, 35] = gridView2.GetRowCellDisplayText(i, "Mat_Balance5");
                }
                xlApp.Visible = true;
            }
            //else if (cboProcess.Text == "SEW")
            //{ 
            
            //}
            //else if (cboProcess.Text == "PACK")
            //{ 
            
            //}
            //else
            //{
                //xlSheet.get_Range(r1, r2).EntireColumn.AutoFit();
                //SaveFileDialog theOpenFile = new SaveFileDialog();
                //string strTemp;
                //theOpenFile.Filter = "Microsoft Excel Document|*.xls";
                //if (theOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                //{
                //    strTemp = theOpenFile.FileName;
                //    gridView1.ExportToXls(strTemp, true);
                //}

                ////using (MemoryStream gridLayout = new MemoryStream())
                ////{
                ////    // Saves the layout.
                ////    gridView1.SaveLayoutToStream(gridLayout);
                ////    gridLayout.Seek(0, SeekOrigin.Begin);
                ////    // Disables the automatic width calculation (if enabled).
                ////    ((DevExpress.Xpf.Grid.TableView)gridView1.View).AutoWidth = false;
                ////    ((DevExpress.Xpf.Grid.TableView)gridView1.View).BestFitColumns();
                ////    ((DevExpress.Xpf.Grid.TableView)gridView1.View).ExportToXls(@"c:\gridexport.xls", new DevExpress.XtraPrinting.XlsExportOptions() { TextExportMode = TextExportMode.Value });
                ////    // Restores the layout.
                ////    gridView1.RestoreLayoutFromStream(gridLayout);
                ////}
            //}
        }

        private void frmHolding_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
        }
        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            switch(e.Column.FieldName)
            {
                case "FORECAST2":
                case "CONFIRM2":
                case "BALANCE_STOCK2":
                    e.Column.AppearanceHeader.BackColor=Color.Pink;
                    break;
                case "FORECAST3":
                case "CONFIRM3":
                case "BALANCE_STOCK3":
                    e.Column.AppearanceHeader.BackColor=Color.LightGreen;
                    break;
                case "FORECAST4":
                case "CONFIRM4":
                case "BALANCE_STOCK4":
                    e.Column.AppearanceHeader.BackColor=Color.LightSalmon;
                    break;
                case "FORECAST5":
                case "CONFIRM5":
                case "BALANCE_STOCK5":
                    e.Column.AppearanceHeader.BackColor=Color.LightSkyBlue;
                    break;
                case "FORECAST6":
                case "CONFIRM6":
                case "BALANCE_STOCK6":
                    e.Column.AppearanceHeader.BackColor=Color.Plum;
                    break;
            }
        }

    }

    public static class CustomLINQtoDataSetMethods
    {
        public static System.Data.DataTable CopyToDataTable2<T>(this IEnumerable<T> source)
        {
            return new ObjectShredder<T>().Shred(source, null, null);
        }

        public static System.Data.DataTable CopyToDataTable2<T>(this IEnumerable<T> source,
                                                    System.Data.DataTable table, LoadOption? options)
        {
            return new ObjectShredder<T>().Shred(source, table, options);
        }

    }

    public class ObjectShredder<T>
    {
        private System.Reflection.FieldInfo[] _fi;
        private System.Reflection.PropertyInfo[] _pi;
        private System.Collections.Generic.Dictionary<string, int> _ordinalMap;
        private System.Type _type;

        // ObjectShredder constructor.
        public ObjectShredder()
        {
            _type = typeof(T);
            _fi = _type.GetFields();
            _pi = _type.GetProperties();
            _ordinalMap = new Dictionary<string, int>();
        }

        /// <summary>
        /// Loads a DataTable from a sequence of objects.
        /// </summary>
        /// <param name="source">The sequence of objects to load into the DataTable.</param>
        /// <param name="table">The input table. The schema of the table must match that 
        /// the type T.  If the table is null, a new table is created with a schema 
        /// created from the public properties and fields of the type T.</param>
        /// <param name="options">Specifies how values from the source sequence will be applied to 
        /// existing rows in the table.</param>
        /// <returns>A DataTable created from the source sequence.</returns>
        public System.Data.DataTable Shred(IEnumerable<T> source, System.Data.DataTable table, LoadOption? options)
        {
            // Load the table from the scalar sequence if T is a primitive type.
            if (typeof(T).IsPrimitive)
            {
                return ShredPrimitive(source, table, options);
            }

            // Create a new table if the input table is null.
            if (table == null)
            {
                table = new System.Data.DataTable(typeof(T).Name);
            }

            // Initialize the ordinal map and extend the table schema based on type T.
            table = ExtendTable(table, typeof(T));

            // Enumerate the source sequence and load the object values into rows.
            table.BeginLoadData();
            using (IEnumerator<T> e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (options != null)
                    {
                        table.LoadDataRow(ShredObject(table, e.Current), (LoadOption)options);
                    }
                    else
                    {
                        table.LoadDataRow(ShredObject(table, e.Current), true);
                    }
                }
            }
            table.EndLoadData();

            // Return the table.
            return table;
        }

        public System.Data.DataTable ShredPrimitive(IEnumerable<T> source, System.Data.DataTable table, LoadOption? options)
        {
            // Create a new table if the input table is null.
            if (table == null)
            {
                table = new System.Data.DataTable(typeof(T).Name);
            }

            if (!table.Columns.Contains("Value"))
            {
                table.Columns.Add("Value", typeof(T));
            }

            // Enumerate the source sequence and load the scalar values into rows.
            table.BeginLoadData();
            using (IEnumerator<T> e = source.GetEnumerator())
            {
                Object[] values = new object[table.Columns.Count];
                while (e.MoveNext())
                {
                    values[table.Columns["Value"].Ordinal] = e.Current;

                    if (options != null)
                    {
                        table.LoadDataRow(values, (LoadOption)options);
                    }
                    else
                    {
                        table.LoadDataRow(values, true);
                    }
                }
            }
            table.EndLoadData();

            // Return the table.
            return table;
        }

        public object[] ShredObject(System.Data.DataTable table, T instance)
        {

            FieldInfo[] fi = _fi;
            PropertyInfo[] pi = _pi;

            if (instance.GetType() != typeof(T))
            {
                // If the instance is derived from T, extend the table schema
                // and get the properties and fields.
                ExtendTable(table, instance.GetType());
                fi = instance.GetType().GetFields();
                pi = instance.GetType().GetProperties();
            }

            // Add the property and field values of the instance to an array.
            Object[] values = new object[table.Columns.Count];
            foreach (FieldInfo f in fi)
            {
                values[_ordinalMap[f.Name]] = f.GetValue(instance);
            }

            foreach (PropertyInfo p in pi)
            {
                values[_ordinalMap[p.Name]] = p.GetValue(instance, null);
            }

            // Return the property and field values of the instance.
            return values;
        }

        public System.Data.DataTable ExtendTable(System.Data.DataTable table, Type type)
        {
            // Extend the table schema if the input table was null or if the value 
            // in the sequence is derived from type T.            
            foreach (FieldInfo f in type.GetFields())
            {
                if (!_ordinalMap.ContainsKey(f.Name))
                {
                    // Add the field as a column in the table if it doesn't exist
                    // already.
                    DataColumn dc = table.Columns.Contains(f.Name) ? table.Columns[f.Name]
                        : table.Columns.Add(f.Name, f.FieldType);

                    // Add the field to the ordinal map.
                    _ordinalMap.Add(f.Name, dc.Ordinal);
                }
            }
            //foreach (PropertyInfo p in type.GetProperties())
            //{
            //    if (!_ordinalMap.ContainsKey(p.Name))
            //    {
            //        // Add the property as a column in the table if it doesn't exist
            //        // already.
            //        DataColumn dc = table.Columns.Contains(p.Name) ? table.Columns[p.Name]
            //            : table.Columns.Add(p.Name, p.PropertyType);

            //        // Add the property to the ordinal map.
            //        _ordinalMap.Add(p.Name, dc.Ordinal);
            //    }
            //}
            foreach (PropertyInfo p in type.GetProperties())
            {

                if (!_ordinalMap.ContainsKey(p.Name))
                {

                    Type colType = p.PropertyType;

                    if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {

                        colType = colType.GetGenericArguments()[0];

                    }

                    DataColumn dc = table.Columns.Contains(p.Name) ? table.Columns[p.Name]

                        : table.Columns.Add(p.Name, colType);

                    _ordinalMap.Add(p.Name, dc.Ordinal);

                }

            }

            // Return the table.
            return table;
        }
    }

}