using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Collections;
using Microsoft.Office.Interop.Excel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using myClass;

namespace DyeDailyReport
{
    public partial class frmDyeDaily : DevExpress.XtraEditors.XtraForm
    {
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        cDatabase db = new cDatabase(Module.TUW99);
        private System.Data.DataTable dtMain;
        private DateTime datStart, datEnd;//เก็บค่าจาก dtpStart และ dtpEnd เมื่อมีการกด display เพื่อป้องกัน user เปลี่ยน dtp แล้วไม่กด display
        private object[] myArray;
        private ArrayList aryRowSave = new ArrayList();//เก็บสถานะว่าแถวใดควรมีการเซฟบ้าง
        private Hashtable htDyeProblem=new Hashtable() ;//เก็บชื่อของปัญหาการย้อม
        private RepositoryItemComboBox rpMachine,rpFabric,rpColor;
        private const int firstColumnOfProblem = 12;//คอลัมน์แรกของปัญหา
        private const int LastColumnOfProblem = 38;//คอลัมน์สุดท้ายของปัญหา

        public frmDyeDaily()
        {
            InitializeComponent();
        }
        private void InitializeControl()
        {
            dtpStartDate.EditValue = DateTime.Today;
            dtpEndDate.EditValue = DateTime.Today;
        }
        private void LoadRepositoryItem(string strFieldName1,string strParam1,string strParam2)
        {
            string strSQL;
            switch (strFieldName1)
            { 
                case "MACHINE":
                    strSQL = "SELECT MACHINENO FROM MACHINEDYE ORDER BY ID";
                    System.Data.DataTable dt1 = db.GetDataTable(strSQL);
                    rpMachine = new RepositoryItemComboBox();
                    foreach (System.Data.DataRow dr in dt1.Rows)
                    {
                        rpMachine.Items.Add(dr["MACHINENO"].ToString());
                    }
                    break;
                case "LOTNO":
                    strSQL = "SELECT DISTINCT FABRIC FROM GREYORDERSHEET WHERE LOTNO='" + strParam1 + "'";
                    System.Data.DataTable dt2 = db.GetDataTable(strSQL);
                    rpFabric = new RepositoryItemComboBox();
                    foreach (System.Data.DataRow dr in dt2.Rows)
                    {
                        rpFabric.Items.Add(dr["FABRIC"]);
                    }
                    break;
                case "FABRIC":
                    strSQL = "SELECT DISTINCT DYECOLOR FROM GREYORDERSHEET WHERE LOTNO='"+strParam1+"' AND FABRIC='" + strParam2 + "'";
                    System.Data.DataTable dt3 = db.GetDataTable(strSQL);
                    rpColor = new RepositoryItemComboBox();
                    foreach (System.Data.DataRow dr in dt3.Rows)
                    {
                        rpColor.Items.Add(dr["DYECOLOR"]);
                    }
                    break;
            }
            
        }
        private System.Data.DataTable LoadFabricFromLotNo(string strLotNo)
        { 
            string strSQL="SELECT FABRIC FROM GREYORDERSHEET WHERE LOTNO='"+strLotNo+"'";
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            return dt;
        }
        private System.Data.DataTable LoadColorFromFabric(string strLotNo, string strFabric)
        {
            string strSQL = "SELECT DYECOLOR FROM GREYORDERSHEET WHERE LOTNO='" + strLotNo + "' AND FABRIC='" + strFabric + "'";
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            return dt;
        }
        private void LoadDyeProblem()
        {
            string strSQL = "SELECT * FROM TUW_DYEPROBLEM ORDER BY PROBLEM";
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            if (dt == null || dt.Rows.Count == 0) { return; }
            foreach (DataRow dr in dt.Rows)
            {
                htDyeProblem.Add(dr["PROBLEM"].ToString(), dr["NAME"].ToString() +"|"+ dr["REMARK"].ToString());
            }
        }
        private void DisplayData()
        {
            aryRowSave.Clear();//ก่อนแสดงข้อมูล เคลียร์ค่าในอาเรย์บันทึกแถวที่จะเซฟก่อน
            string strSQL = "EXEC spDyeDailyReport '" + datStart.ToString("yyyy-MM-dd", dtfinfo) + "','" +datEnd.ToString("yyyy-MM-dd",dtfinfo)+"'" ;
            dtMain = db.GetDataTable(strSQL);
            //GridView1
            gridControl1.DataSource = null;
            gridControl1.DataSource = dtMain;
            gridView1.PopulateColumns();
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.Columns["ID"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["IDATE"].Caption = "Date";
            gridView1.Columns["MACHINE"].Caption = "M/C";
            gridView1.Columns["LOTNO"].Caption = "LOT NO.";
            gridView1.Columns["FABRIC"].Caption = "Fabric Code.";
            gridView1.Columns["MAT_CONTENT"].Caption = "MAT.\nContent";
            gridView1.Columns["COLOR_TUW"].Caption = "Color Code.";
            gridView1.Columns["COLOR"].Caption = "Color.";
            gridView1.Columns["QTY"].Caption = "Pcs.";
            gridView1.Columns["WEIGHT"].Caption = "Kgs.";
            gridView1.Columns["SCHEDULE_TIME"].Caption = "Schedule\nTime";
            gridView1.Columns["ACTUAL_TIME"].Caption = "Actual\nTime";
            gridView1.Columns["REMARK_SHADE"].Caption = "SHADE\n(kgs.)";
            gridView1.Columns["REMARK_SOAP"].Caption = "SOAP\n(kgs.)";
            gridView1.Columns["REMARK_LAB"].Caption = "LAB\n(kgs.)";
            for (int i = firstColumnOfProblem; i <= LastColumnOfProblem; i++)
            {
                gridView1.Columns[i].Caption = gridView1.Columns[i].FieldName.Substring(1,2);
                gridView1.Columns[i].ToolTip = htDyeProblem[gridView1.Columns[i].FieldName].ToString();
            }
            //gridView1.Columns["ID"].Visible = false;
            for (int i = 0; i < firstColumnOfProblem; i++)
            {
                gridView1.Columns[i].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            }
            gridView1.Columns["REMARK_SHADE"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            gridView1.Columns["REMARK_SOAP"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            gridView1.Columns["REMARK_LAB"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            gridControl1.RepositoryItems.Add(rpMachine);
            gridView1.Columns["MACHINE"].ColumnEdit = rpMachine;
            gridView1.IndicatorWidth = 40;
            gridView1.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            gridView1.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridView1.ColumnPanelRowHeight = 40;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
        }
        private void SaveData()
        {
            string strSQL,strRowGuid;
            string strComputerName=System.Environment.MachineName;
            //for new
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                if (gridView1.GetRowCellValue(i, "ID") != System.DBNull.Value) { continue; }
                strRowGuid = System.Guid.NewGuid().ToString();
                System.Diagnostics.Debug.Print(strRowGuid);
                strSQL = "INSERT INTO DYEDAILYREPORT(IDATE,MACHINE,LOTNO,FABRIC,MAT_CONTENT,COLOR_TUW,COLOR,QTY,WEIGHT"+
                    ",SCHEDULE_TIME,ACTUAL_TIME,REMARK_SHADE,REMARK_SOAP,REMARK_LAB,INPUTUSER,ROW_GUID)VALUES(";
                if (gridView1.GetRowCellValue(i, "DUE_DATE") == System.DBNull.Value)
                {
                    strSQL += "NULL";
                }
                else
                {
                    strSQL += "'" + ((DateTime)gridView1.GetRowCellValue(i, "IDATE")).ToString("yyyy-MM-dd", dtfinfo) + "'";
                }
                strSQL += ",'" + gridView1.GetRowCellValue(i, "MACHINE").ToString() + "'";
                strSQL += ",'" + gridView1.GetRowCellValue(i, "LOTNO").ToString() + "'";
                strSQL += ",'" + gridView1.GetRowCellValue(i, "FABRIC").ToString() + "'";
                strSQL += ",'" + gridView1.GetRowCellValue(i, "MAT_CONTENT").ToString() + "'";
                strSQL += ",'" + gridView1.GetRowCellValue(i, "COLOR_TUW").ToString() + "'";
                strSQL += ",'" + gridView1.GetRowCellValue(i, "COLOR").ToString() + "'";
                strSQL += ",'" + gridView1.GetRowCellValue(i, "QTY").ToString() + "'";
                strSQL += ",'" + gridView1.GetRowCellValue(i, "WEIGHT").ToString() + "'";
                strSQL += ",'" + gridView1.GetRowCellValue(i, "SCHEDULE_TIME").ToString() + "'";
                strSQL += ",'" + gridView1.GetRowCellValue(i, "ACTUAL_TIME").ToString() + "'";
                strSQL += ",'" + gridView1.GetRowCellValue(i, "REMARK_SHADE").ToString() + "'";
                strSQL += ",'" + gridView1.GetRowCellValue(i, "REMARK_SOAP").ToString() + "'";
                strSQL += ",'" + gridView1.GetRowCellValue(i, "REMARK_LAB").ToString() + "'";
                strSQL += ",'" + strComputerName + "'";
                strSQL += ",'" + strRowGuid + "'";
                strSQL += ")";
                db.Execute(strSQL);

                int count=0;
                for(int j=firstColumnOfProblem;j<=LastColumnOfProblem;j++)
                {
                    count++;
                    if ((decimal)gridView1.GetRowCellValue(i, gridView1.Columns[j]) != 0)
                    {
                        strSQL = "INSERT INTO DYEDAILYPROBLEM(REFID,PROBLEM,LOSS_TIME)"+
                            "SELECT ID,'P" + count.ToString("00") + "','" + gridView1.GetRowCellValue(i, gridView1.Columns[j]) + "' FROM DYEDAILYREPORT WHERE ROW_GUID='" + strRowGuid + "'";
                        db.Execute(strSQL);
                    }
                }
                
            }

            //for update-----------------------------------------------------------------------------------------------------------------------------------------------------------------
            foreach (int intCurrentElement in aryRowSave)
            {
                strSQL = "UPDATE DYEDAILYREPORT SET MACHINE='" + gridView1.GetRowCellValue(intCurrentElement, "MACHINE") + "'";
                if (gridView1.GetRowCellValue(intCurrentElement, "IDATE") == System.DBNull.Value)
                {
                    strSQL += ",IDATE=NULL";
                }
                else
                {
                    strSQL += ",IDATE='" + ((DateTime)gridView1.GetRowCellValue(intCurrentElement, "IDATE")).ToString("yyyy-MM-dd", dtfinfo) + "'";
                }
                strSQL += ",LOTNO='" + gridView1.GetRowCellValue(intCurrentElement, "LOTNO") + "'";
                strSQL += ",FABRIC='" + gridView1.GetRowCellValue(intCurrentElement, "FABRIC") + "'";
                strSQL += ",MAT_CONTENT='" + gridView1.GetRowCellValue(intCurrentElement, "MAT_CONTENT") + "'";
                strSQL += ",COLOR_TUW='" + gridView1.GetRowCellValue(intCurrentElement, "COLOR_TUW") + "'";
                strSQL += ",COLOR='" + gridView1.GetRowCellValue(intCurrentElement, "COLOR") + "'";
                strSQL += ",QTY='" + gridView1.GetRowCellValue(intCurrentElement, "QTY") + "'";
                strSQL += ",WEIGHT='" + gridView1.GetRowCellValue(intCurrentElement, "WEIGHT") + "'";
                strSQL += ",SCHEDULE_TIME='" + gridView1.GetRowCellValue(intCurrentElement, "SCHEDULE_TIME") + "'";
                strSQL += ",ACTUAL_TIME='" + gridView1.GetRowCellValue(intCurrentElement, "ACTUAL_TIME") + "'";
                strSQL += ",REMARK_SHADE='" + gridView1.GetRowCellValue(intCurrentElement, "REMARK_SHADE") + "'";
                strSQL += ",REMARK_SOAP='" + gridView1.GetRowCellValue(intCurrentElement, "REMARK_SOAP") + "'";
                strSQL += ",REMARK_LAB='" + gridView1.GetRowCellValue(intCurrentElement, "REMARK_LAB") + "'";
                //strSQL += ",INPUTDATE='" + DateTime.Now.ToString("yyyyMMddHHmmssfff")+"'";
                strSQL += ",INPUTUSER='" + strComputerName + "'";
                strSQL += " WHERE ID=" + gridView1.GetRowCellValue(intCurrentElement, "ID");
                db.Execute(strSQL);

                strSQL = "DELETE FROM DYEDAILYPROBLEM WHERE REFID=" + gridView1.GetRowCellValue(intCurrentElement, "ID");
                db.Execute(strSQL);

                int count = 0;
                for (int i = firstColumnOfProblem; i < LastColumnOfProblem+1; i++)
                {
                    count++;
                    if ((decimal)gridView1.GetRowCellValue(intCurrentElement, gridView1.Columns[i].FieldName) > 0)
                    {
                        strSQL = "INSERT INTO DYEDAILYPROBLEM(REFID,PROBLEM,LOSS_TIME)VALUES(" +
                            gridView1.GetRowCellValue(intCurrentElement, "ID") + ",'P" + count.ToString("00") + "'," + gridView1.GetRowCellValue(intCurrentElement, gridView1.Columns[i].FieldName) + ")";
                        db.Execute(strSQL);
                    }
                }
            }
        }

        private void frmDyeDaily_Load(object sender, EventArgs e)
        {
            dtfinfo = clinfo.DateTimeFormat;
            InitializeControl();
            LoadRepositoryItem("MACHINE","","");
            LoadDyeProblem();
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            datStart = Convert.ToDateTime(dtpStartDate.EditValue);
            datEnd = Convert.ToDateTime(dtpEndDate.EditValue);
            try 
            {
                DisplayData(); 
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                SaveData();
                db.CommitTrans();
                MessageBox.Show("Save complete...", "DyeDailyReport", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayData();
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
        }
        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.AbsoluteIndex >= firstColumnOfProblem)
            {
                if (Convert.ToDecimal(e.Value) == 0)
                {
                    e.DisplayText = "";
                }
            }
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
        private void gridView1_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            GridView gv1 = (GridView)sender;
            for (int i = firstColumnOfProblem; i < gv1.Columns.Count; i++)
            {
                gv1.SetRowCellValue(e.RowHandle, gv1.Columns[i].FieldName, 0);
            }
        }
        private void gridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "DATE")
            {
                RepositoryItemDateEdit rpDate = new RepositoryItemDateEdit();
                rpDate.Mask.EditMask = "dd/MM/yyyy";
                rpDate.Mask.UseMaskAsDisplayFormat = true;
                e.RepositoryItem = rpDate;
            }
        }
        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView gv1 = (GridView)sender;
            if (gv1.IsNewItemRow(gv1.FocusedRowHandle) == false)//แถวเก่า
            {
                if (gv1.GetRowCellValue(gv1.FocusedRowHandle, "ID") != System.DBNull.Value)//แถวเก่ามี ID
                {
                    aryRowSave.Sort();
                    if (aryRowSave.BinarySearch(gv1.FocusedRowHandle) < 0)
                    {
                        aryRowSave.Add(gv1.FocusedRowHandle);//จะเลือกแถวเฉพาะที่เคยมีการเซฟข้อมูลไปแล้วเพ่่ือการอัพเดทเท่านั้น
                    }
                }
            }
         }
        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                switch (gridView1.FocusedColumn.FieldName)
                {
                    case "FABRIC":
                        LoadRepositoryItem("LOTNO", gridView1.GetFocusedRowCellValue("LOTNO").ToString(), "");
                        gridControl1.RepositoryItems.Add(rpFabric);
                        gridView1.Columns["FABRIC"].ColumnEdit = rpFabric;
                        break;
                    case "COLOR_TUW":
                        LoadRepositoryItem("FABRIC", gridView1.GetFocusedRowCellValue("LOTNO").ToString(), gridView1.GetFocusedRowCellValue("FABRIC").ToString());
                        gridControl1.RepositoryItems.Add(rpColor);
                        gridView1.Columns["COLOR_TUW"].ColumnEdit = rpColor;
                        break;
                }
            }
            catch { }
        }
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            e.Column.BestFit();
        }
        private void gridView1_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    GridView gv1 = (GridView)sender;
                    if(gv1.IsRowSelected(e.RowHandle))
                    {
                        popupMenu1.ShowPopup(new System.Drawing.Point(Cursor.Position.X,Cursor.Position.Y));
                    }
                }
                else
                {
                    barStaticItem1.Caption = e.CellValue.ToString();
                }
            }
            catch{}
        }
        private void bbiCopy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            myArray = new object[gridView1.Columns.Count];
            for (int i = 0; i < gridView1.Columns.Count; i++)
            {
                myArray[i] = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, gridView1.Columns[i].FieldName);
            }
        }
        private void bbiPaste_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = dtMain.NewRow();
                dr.ItemArray = myArray;
                dr["ID"] = System.DBNull.Value;
                dtMain.Rows.InsertAt(dr, gridView1.FocusedRowHandle);
                gridControl1.DataSource = dtMain;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void bbiDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("คุณต้องการลบข้อมูลแถวนี้หรือไม่. ID " + gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "ID"), "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                db.ConnectionOpen();
                try
                {
                    db.BeginTrans();
                    string strSQL="DELETE FROM DYEDAILYPROBLEM WHERE REFID="+gridView1.GetRowCellValue(gridView1.FocusedRowHandle,"ID");
                    db.Execute(strSQL);
                    strSQL = "DELETE FROM DYEDAILYREPORT WHERE ID=" + gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "ID");
                    db.Execute(strSQL);
                    gridView1.DeleteRow(gridView1.FocusedRowHandle);
                    db.CommitTrans();
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
        }
        private void bbiSummary_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Thread currentThread = System.Threading.Thread.CurrentThread;
            currentThread.CurrentCulture = clinfo;
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook xlBook = xlApp.Workbooks.Open(System.Windows.Forms.Application.StartupPath + "\\Report\\Dyeing_monthly_report.xlt", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet xlSheet = (_Worksheet)xlApp.ActiveSheet;
            int excelFirstDataRow = 5;
            int excelCurrentRow = excelFirstDataRow;

            this.Cursor = Cursors.WaitCursor;
            try
            {
                //set header cells
                xlSheet.get_Range("A1", "A1").Value2 = "DYEING   PASS  AND  NO PASS  MONTHLY  REPORT  IN " + datStart.ToString("MMMM", dtfinfo).ToUpper() + " " + datStart.ToString("yyyy", dtfinfo);
                //set detail cells
                string strSQL = "EXEC spDyeDailyReport_Summary '" + datStart.ToString("yyyy-MM-dd", dtfinfo) + "','" + datEnd.ToString("yyyy-MM-dd", dtfinfo) + "'";
                System.Data.DataSet ds = db.GetDataSet(strSQL);
                foreach (System.Data.DataTable dt in ds.Tables)
                {
                    if (dt != null || dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                xlSheet.Cells[excelCurrentRow, j + 1] = dt.Rows[i][j].ToString();
                            }
                            barStaticItem1.Caption = "Export to excel : " + (i + 1) + " of " + dt.Rows.Count.ToString();
                            this.Refresh();
                            excelCurrentRow++;
                        }
                        //create total row
                        xlSheet.Cells[excelCurrentRow, 1] = "total";
                        xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, 1], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, 13]).Interior.ColorIndex = 4;//ลงพื้นเขียว
                        xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, 1], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, 13]).BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium, XlColorIndex.xlColorIndexAutomatic, null);//ทำเส้นกรอบหนา
                        for (int k = 1; k < 7; k++)
                        {
                            object sumObject;
                            sumObject = dt.Compute("Sum(" + dt.Columns[k].ColumnName + ")", "");
                            xlSheet.Cells[excelCurrentRow, k + 1] = sumObject.ToString();
                        }
                        xlSheet.get_Range("J" + excelCurrentRow).Value2 = "";
                        xlSheet.get_Range("K" + excelCurrentRow).Value2 = "";
                        xlSheet.get_Range("L" + excelCurrentRow).Value2 = "";
                        xlSheet.get_Range("M" + excelCurrentRow).Value2 = "";
                        excelCurrentRow++;
                    }
                }
                //create grand total row
                decimal grandTotal;
                xlSheet.Cells[excelCurrentRow, 1] = "Total";
                for (int j = 2; j <= 7; j++)
                {
                    grandTotal = 0;
                    for (int i = excelFirstDataRow; i < excelCurrentRow; i++)
                    {
                        
                        if (xlSheet.get_Range("A"+i).Value2.ToString() == "total")
                        {
                            grandTotal += decimal.Parse(((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[i, j]).Value2.ToString());
                            //grandTotal += decimal.Parse(xlSheet.get_Range("B"+i).Value2.ToString());
                        }
                    }
                    xlSheet.Cells[excelCurrentRow, j] = grandTotal;
                }
                xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, 1], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, 13]).Font.Bold = true;
                xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, 1], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, 13]).Interior.ColorIndex = 35;//ลงพื้นเขียวอ่อน
                xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, 1], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, 13]).BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium, XlColorIndex.xlColorIndexAutomatic, null);//ทำเส้นกรอบหนา
                excelCurrentRow++;
                //กำจัดแถวที่เหลือทั้งหมดใน excel template
                for (int i = excelCurrentRow; i < xlSheet.UsedRange.Count; i++)
                {
                    xlSheet.get_Range("A" + excelCurrentRow).EntireRow.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                xlApp.Visible = true;
                this.Cursor = Cursors.Default;
            }
        }
        private void bbiDetail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Thread currentThread = System.Threading.Thread.CurrentThread;
            currentThread.CurrentCulture = clinfo;
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            //Microsoft.Office.Interop.Excel._Workbook xlBook = excel.Workbooks.Add(XlSheetType.xlWorksheet);
            Microsoft.Office.Interop.Excel._Workbook xlBook = excel.Workbooks.Open(System.Windows.Forms.Application.StartupPath + "\\Report\\DyeDailySheet.xlt", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet xlSheet = (_Worksheet)excel.ActiveSheet;
            int excelCurrentRow = 6;

            this.Cursor = Cursors.WaitCursor;
            try
            {
                //set header cells
                xlSheet.get_Range("D1", "D1").Value2 = "DAILY  PRODUCTION REPORT  ON  " + datStart.ToString("MMMM", dtfinfo).ToUpper() + "  " + datStart.ToString("yyyy", dtfinfo);
                //for (int i = 1; i < 32; i++)
                //{
                //    //xlSheet.Cells[6, i] = gridView1.Columns[i].HeaderText;
                //    //xlSheet.get_Range(xlSheet.Cells[6, i], xlSheet.Cells[6, i]).Font.Size = 16;
                //    //xlSheet.get_Range(xlSheet.Cells[6, i], xlSheet.Cells[6, i]).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                //    //xlSheet.Cells[7, i] = "Pcs";
                //    //xlSheet.get_Range(xlSheet.Cells[7, i], xlSheet.Cells[7, i]).Font.Size = 12;
                //    //xlSheet.get_Range(xlSheet.Cells[6, i], xlSheet.Cells[7, i]).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                //    //xlSheet.get_Range(xlSheet.Cells[6, i], xlSheet.Cells[7, i]).BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium, XlColorIndex.xlColorIndexAutomatic, null);
                //    try
                //    {
                //        DateTime datTemp = new DateTime(datStart.Year, datStart.Month, i);
                //        if (Module.IsHoliday(datTemp.ToString("yyyy", dtfinfo), datTemp.ToString("MM", dtfinfo), datTemp.ToString("dd", dtfinfo)))
                //        {
                //            xlSheet.get_Range(xlSheet.Cells[6, i + (firstColumnOfProblem - 1)], xlSheet.Cells[7 + gridView1.RowCount - 1, i + (firstColumnOfProblem - 1)]).Interior.ColorIndex = 40;
                //        }
                //        if (datTemp.DayOfWeek == DayOfWeek.Sunday)
                //        {
                //            xlSheet.get_Range(xlSheet.Cells[6, i + (firstColumnOfProblem - 1)], xlSheet.Cells[7 + gridView1.RowCount - 1, i + (firstColumnOfProblem - 1)]).Interior.ColorIndex = 40;
                //        }
                //    }
                //    catch { }
                //}
                //set detail cells
                //ปรับช่อง date ให้แสดงในแบบตัวอักษรแทนฟอร์แมตวันที่
                xlSheet.get_Range("A:G", "A:G").NumberFormat = "@";
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    for (int j = 1; j < gridView1.Columns.Count; j++)
                    {
                        if (gridView1.GetRowCellDisplayText(i, gridView1.Columns[j].FieldName) == "0")
                        {
                            xlSheet.Cells[excelCurrentRow, j] = "";
                        }
                        else
                        {
                            xlSheet.Cells[excelCurrentRow, j] = gridView1.GetRowCellDisplayText(i, gridView1.Columns[j].FieldName);
                        }
                    }
                    barStaticItem1.Caption = "Export to excel : " + (i + 1) + " of " + (gridView1.DataRowCount).ToString();
                    this.Refresh();
                    excelCurrentRow++;
                }
                //xlSheet.get_Range("A1", "AO" + xlSheet.UsedRange.Rows.Count+2).Font.Name = "Angsana New";
                //xlSheet.get_Range("A5", "J5").ColumnWidth = 8.43;
                //xlSheet.get_Range("K5", "AO5").ColumnWidth = 5;
                //autofit column
                //xlSheet.Columns.AutoFit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                excel.Visible = true;
                this.Cursor = Cursors.Default;
            }
        }
        private void bbiLossTime_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Thread currentThread = System.Threading.Thread.CurrentThread;
            currentThread.CurrentCulture = clinfo;
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook xlBook = xlApp.Workbooks.Open(System.Windows.Forms.Application.StartupPath + "\\Report\\LOSS_TIME.xlt", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet xlSheet = (_Worksheet)xlApp.ActiveSheet;
            int excelFirstDataRow = 6;
            int excelCurrentRow = excelFirstDataRow;

            this.Cursor = Cursors.WaitCursor;
            try
            {
                //set header cells
                xlSheet.get_Range("B1", "B1").Value2 = "DYEING  MACHINE  LOSS  TIME  ON  " + datStart.ToString("MMMM", dtfinfo).ToUpper() + "  " + datStart.ToString("yyyy", dtfinfo);
                //set detail cells
                string strSQL = "EXEC spDyeDailyReport_LossTime '" + datStart.ToString("yyyy-MM-dd", dtfinfo) + "','" + datEnd.ToString("yyyy-MM-dd", dtfinfo) + "'";
                System.Data.DataTable dt = db.GetDataTable(strSQL);
                if (dt != null || dt.Rows.Count > 0)
                {
                    string strMachine;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strMachine = dt.Rows[i]["MACHINE"].ToString();
                        xlSheet.get_Range("A" + excelCurrentRow).Value2 = strMachine;
                        xlSheet.get_Range("B" + excelCurrentRow).Value2 = dt.Rows[i]["SCHEDULE_TIME"].ToString();
                        switch (dt.Rows[i]["PROBLEM"].ToString())
                        { 
                            case "P01":
                                xlSheet.get_Range("G" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P02":
                                xlSheet.get_Range("I" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P03":
                                xlSheet.get_Range("K" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P05":
                                xlSheet.get_Range("M" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P06":
                                xlSheet.get_Range("O" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P07":
                                xlSheet.get_Range("Q" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P08":
                                xlSheet.get_Range("S" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P09":
                                xlSheet.get_Range("U" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P10":
                                xlSheet.get_Range("W" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P11":
                                xlSheet.get_Range("Y" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P12":
                                xlSheet.get_Range("AA" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P13":
                                xlSheet.get_Range("AC" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P14":
                                xlSheet.get_Range("AE" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P15":
                                xlSheet.get_Range("AG" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P16":
                                xlSheet.get_Range("AI" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P17":
                                xlSheet.get_Range("AK" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P18":
                                xlSheet.get_Range("AM" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P19":
                                xlSheet.get_Range("AO" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P20":
                                xlSheet.get_Range("AQ" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P21":
                                xlSheet.get_Range("AS" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P22":
                                xlSheet.get_Range("AU" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P23":
                                xlSheet.get_Range("AW" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P24":
                                xlSheet.get_Range("AY" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P25":
                                xlSheet.get_Range("BA" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                            case "P26":
                                xlSheet.get_Range("BC" + excelCurrentRow).Value2 = dt.Rows[i]["LOSS_TIME"].ToString();
                                break;
                        }
                        barStaticItem1.Caption = "Export to excel : " + (i + 1) + " of " + dt.Rows.Count.ToString();
                        this.Refresh();
                        if((i==dt.Rows.Count-1)||(Equals(strMachine,dt.Rows[i+1]["MACHINE"].ToString())==false)){excelCurrentRow++;}
                    }
                }
                //กำจัดแถวที่เหลือทั้งหมดใน excel template
                for (int i = xlSheet.UsedRange.Rows.Count;i>excelCurrentRow ; i--)
                {
                    xlSheet.get_Range("A" + excelCurrentRow).EntireRow.Delete();
                }
                //กำจัดคอลัมน์ที่มีผลรวมเป็นศูนย์
                for (int j = xlSheet.UsedRange.Columns.Count; j > 6; j--)
                {
                    if (Convert.ToDecimal(xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, j], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, j]).Value2)==0)
                    {
                        xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelFirstDataRow, j], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelFirstDataRow, j]).EntireColumn.Delete();
                    }
                }
                //เอาค่าคอลัมน์ที่ถูกลบออกๆจากสูตร sum ในช่อง total loss time
                for (int i = excelFirstDataRow; i < excelCurrentRow; i++)
                {
                    string strFormula;
                    strFormula = xlSheet.get_Range("D" + i).Formula.ToString();
                    xlSheet.get_Range("D" + i).Formula = strFormula.Replace("#REF!", "");
                }
                //กำจัดคอลัมน์ % ที่ให้ค่าเป็นให้ค่าเป็น #REF!
                for (int j = xlSheet.UsedRange.Columns.Count; j > 6; j--)
                {
                    if (xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, j], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, j]).Formula.ToString().Contains("#REF!"))
                    {
                        xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelFirstDataRow, j], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelFirstDataRow, j]).EntireColumn.Delete();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                xlApp.Visible = true;
                this.Cursor = Cursors.Default;
            }
        }


    }
}