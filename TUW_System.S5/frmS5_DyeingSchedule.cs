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
using DevExpress.XtraGrid.Views.Base;
using myClass;

namespace TUW_System.S5
{
    public partial class frmS5_DyeingSchedule : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;

        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        private System.Data.DataSet ds1;
        private DateTime datStart, datEnd;//เก็บค่าจาก dtpStart และ dtpEnd เมื่อมีการกด display เพื่อป้องกัน user เปลี่ยน dtp แล้วไม่กด display
        private const int firstColumnOfDay = 11;//คอลัมน์วันแรกใน gridView2
        private decimal oldValue;
        private RepositoryItemComboBox rpMachine;
        private Hashtable htHoliday=new Hashtable();
        private object[] myArray;
        private ArrayList aryRowSave = new ArrayList();//เก็บสถานะว่าแถวใดควรมีการเซฟบ้าง

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmS5_DyeingSchedule()
        {
            InitializeComponent();
        }
        public void ExportExcel()
        {
            Thread currentThread = System.Threading.Thread.CurrentThread;
            currentThread.CurrentCulture = clinfo;
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook xlBook = excel.Workbooks.Open(System.Windows.Forms.Application.StartupPath + @"\Report\PLAN-DYEING.xlt");//, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet xlSheet = (_Worksheet)excel.ActiveSheet;
            int excelCurrentRow = 8;

            this.Cursor = Cursors.WaitCursor;
            //set header cells
            xlSheet.get_Range("D3", "D3").Value2 = "Month " + datStart.ToString("MMMM-yyyy", dtfinfo);
            for (int i = 1; i < 32; i++)
            {
                //xlSheet.Cells[6, i] = GridView2.Columns[i].HeaderText;
                //xlSheet.get_Range(xlSheet.Cells[6, i], xlSheet.Cells[6, i]).Font.Size = 16;
                //xlSheet.get_Range(xlSheet.Cells[6, i], xlSheet.Cells[6, i]).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                //xlSheet.Cells[7, i] = "Pcs";
                //xlSheet.get_Range(xlSheet.Cells[7, i], xlSheet.Cells[7, i]).Font.Size = 12;
                //xlSheet.get_Range(xlSheet.Cells[6, i], xlSheet.Cells[7, i]).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                //xlSheet.get_Range(xlSheet.Cells[6, i], xlSheet.Cells[7, i]).BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium, XlColorIndex.xlColorIndexAutomatic, null);
                try
                {
                    DateTime datTemp = new DateTime(datStart.Year, datStart.Month, i);
                    if (IsHoliday(datTemp.ToString("yyyy", dtfinfo), datTemp.ToString("MM", dtfinfo), datTemp.ToString("dd", dtfinfo)))
                    {
                        xlSheet.get_Range(xlSheet.Cells[6, i + (firstColumnOfDay - 1)], xlSheet.Cells[7 + gridView3.RowCount - 1, i + (firstColumnOfDay - 1)]).Interior.ColorIndex = 40;
                    }
                    if (datTemp.DayOfWeek == DayOfWeek.Sunday)
                    {
                        xlSheet.get_Range(xlSheet.Cells[6, i + (firstColumnOfDay - 1)], xlSheet.Cells[7 + gridView3.RowCount - 1, i + (firstColumnOfDay - 1)]).Interior.ColorIndex = 40;
                    }
                }
                catch { }
            }
            //set detail cells
            //ปรับช่อง due_date ให้แสดงในแบบตัวอักษรแทนฟอร์แมตวันที่
            xlSheet.get_Range("A:E", "A:E").NumberFormat = "@";
            xlSheet.get_Range("H:J", "H:J").NumberFormat = "@";
            for (int i = 0; i < gridView3.DataRowCount; i++)
            {
                //if (gridView2.Rows[i].IsNewRow) { break; }
                //รายละเอียดหลัก
                for (int j = 1; j < firstColumnOfDay - 1; j++)
                {
                    //if (gridView2.Rows[i].Cells[j].Value != null)
                    //{
                    xlSheet.Cells[excelCurrentRow, j] = gridView3.GetRowCellDisplayText(i, gridView3.Columns[j].FieldName);//Rows[i].Cells[j].Value.ToString();
                    //}
                }
                //รายละเอียดการลงจำนวนพับในแต่ละวัน
                for (int j = firstColumnOfDay; j < gridView3.Columns.Count; j++)
                {
                    //ในกรณีที่ผู้ใช้ไม่ได้เลือกวันเริ่มต้นเป็นวันที่ 1 จำเป็นต้องบวกวันเพิ่มเข้าไปเวลาลงข้อมูลใน excel เนื่องจาก excel มีการกำหนดตำแหน่งที่ชัดเจน
                    int excelCurrentCol = j + (datStart.Day - 1);
                    if (gridView3.GetRowCellDisplayText(i, gridView3.Columns[j].FieldName) == "0")
                    {
                        xlSheet.Cells[excelCurrentRow, excelCurrentCol] = "";
                    }
                    else
                    {
                        xlSheet.Cells[excelCurrentRow, excelCurrentCol] = gridView3.GetRowCellDisplayText(i, gridView3.Columns[j].FieldName);
                    }
                }
                StatusBarEvent("Export to excel : " + (i + 1) + " of " + (gridView3.DataRowCount).ToString());
                this.Refresh();
                excelCurrentRow += 1;
            }
            //xlSheet.get_Range("A1", "AO" + xlSheet.UsedRange.Rows.Count+2).Font.Name = "Angsana New";
            //xlSheet.get_Range("A5", "J5").ColumnWidth = 8.43;
            //xlSheet.get_Range("K5", "AO5").ColumnWidth = 5;
            //autofit column
            //xlSheet.Columns.AutoFit();
            excel.Visible = true;
            this.Cursor = Cursors.Default;
        }
        public void DisplayData()
        {
            try
            {
                datStart = Convert.ToDateTime(dtpStartDate.EditValue);
                datEnd = Convert.ToDateTime(dtpEndDate.EditValue);
                if (!datStart.Month.Equals(datEnd.Month)) throw new ApplicationException("กรุณาเลือกช่วงเดือนภายในเดือนเดียวกัน...!!!");
                aryRowSave.Clear();//ก่อนแสดงข้อมูล เคลียร์ค่าในอาเรย์บันทึกแถวที่จะเซฟก่อน
                string strSQL = "EXEC spDyeingSchedule1 '" + datStart.ToString("yyyyMM", dtfinfo) + "'," + datStart.Day + "," + datEnd.Day;
                ds1 = db.GetDataSet(strSQL);
                //GridView1--date
                gridControl1.DataSource = null;
                gridControl1.DataSource = ds1.Tables[0];
                gridView1.PopulateColumns();
                gridView1.OptionsView.EnableAppearanceEvenRow = true;
                gridView1.OptionsView.EnableAppearanceOddRow = true;
                gridView1.OptionsBehavior.Editable = false;
                chkShowFooter_CheckedChanged(null, null);
                for (int i = datStart.Day; i <= datEnd.Day; i++)
                {
                    gridView1.Columns[i.ToString()].SummaryItem.FieldName = i.ToString();
                    gridView1.Columns[i.ToString()].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n0}");
                }
                gridView1.OptionsView.ColumnAutoWidth = false;
                gridView1.BestFitColumns();
                ////GridView2--Duedate
                //gridControl2.DataSource = null;
                //gridControl2.DataSource = ds1.Tables[1];
                //gridView2.PopulateColumns();
                //gridView2.OptionsView.EnableAppearanceEvenRow = true;
                //gridView2.OptionsView.EnableAppearanceOddRow = true;
                //gridView2.OptionsView.ColumnAutoWidth = false;
                //gridView2.BestFitColumns();
                //gridView2.OptionsBehavior.Editable = false;
                //chkShowFooter_CheckedChanged(null, null);
                //for (int i = datStart.Day; i <= datEnd.Day; i++)
                //{
                //    gridView2.Columns[i.ToString()].SummaryItem.FieldName = i.ToString();
                //    gridView2.Columns[i.ToString()].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n0}");
                //}
                //GridView3
                gridControl3.DataSource = null;
                gridControl3.DataSource = ds1.Tables[1];
                gridView3.PopulateColumns();
                gridView3.OptionsView.EnableAppearanceEvenRow = true;
                gridView3.OptionsView.EnableAppearanceOddRow = true;
                gridView3.OptionsView.ColumnAutoWidth = false;
                gridView3.BestFitColumns();
                gridView3.Columns["ID"].OptionsColumn.AllowEdit = false;
                //gridView3.Columns["ID"].Visible = false;
                gridView3.Columns["MACHINE"].Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                gridControl3.RepositoryItems.Add(rpMachine);
                gridView3.Columns["MACHINE"].ColumnEdit = rpMachine;
                gridView3.IndicatorWidth = 40;
                gridView3.Columns["PCS"].SummaryItem.FieldName = "PCS";
                gridView3.Columns["PCS"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n0}");
                gridView3.Columns["PCS"].Width = 50;
                gridView3.Columns["KGS"].SummaryItem.FieldName = "KGS";
                gridView3.Columns["KGS"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n0}");
                gridView3.Columns["KGS"].Width = 50;
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            LoadHoliday((DateTime)dtpStartDate.EditValue);
        }
        public void SaveData()
        {
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                string strSQL;  
                //for new
                for (int i = 0; i < gridView3.DataRowCount; i++)
                {
                    if (gridView3.GetRowCellValue(i, "ID") != System.DBNull.Value){continue;}
                    strSQL = "Insert Into DyePlan(OrderNo,Color,MachineNo,Fabric,Section,QtyOrder,Weight,Knitting,Week,DueDate,iDate";
                    for (int j = firstColumnOfDay; j < gridView3.Columns.Count; j++)
                    { 
                        strSQL+=",d"+gridView3.Columns[j].FieldName;
                    }
                    strSQL += ")Values('" + gridView3.GetRowCellValue(i,"PO").ToString() + "'";
                    strSQL += ",'" + gridView3.GetRowCellValue(i,"COLOR").ToString() + "'";
                    strSQL += ",'" + gridView3.GetRowCellValue(i,"MACHINE").ToString() + "'";
                    strSQL += ",'" + gridView3.GetRowCellValue(i,"FABRIC").ToString() + "'";
                    strSQL += ",'" + gridView3.GetRowCellValue(i,"SECTION").ToString() + "'";
                    strSQL += ",'" + gridView3.GetRowCellValue(i,"PCS").ToString() + "'";
                    strSQL += ",'" + gridView3.GetRowCellValue(i,"KGS").ToString() + "'";
                    strSQL += ",'" + gridView3.GetRowCellValue(i,"KNITTING").ToString() + "'";
                    strSQL += ",'" + gridView3.GetRowCellValue(i,"WEEK").ToString() + "'";
                    if (gridView3.GetRowCellValue(i,"DUE_DATE") == System.DBNull.Value)
                    {
                        strSQL += ",null";
                    }
                    else
                    {
                        strSQL +=",'"+((DateTime)gridView3.GetRowCellValue(i,"DUE_DATE")).ToString("yyyy-MM-dd",dtfinfo)+"'";
                    }
                    strSQL += ",'" + datStart.ToString("yyyyMM", dtfinfo) + "'";
                    for (int j = firstColumnOfDay; j <gridView3.Columns.Count; j++)
                    {
                        strSQL += ",'" + gridView3.GetRowCellValue(i,gridView3.Columns[j].FieldName) + "'";
                    }
                    strSQL += ")";
                    db.Execute(strSQL);
                }
                //for update
                foreach (int intCurrentElement in aryRowSave)
                {
                    strSQL = "Update DyePlan Set MachineNo='" + gridView3.GetRowCellValue(intCurrentElement,"MACHINE") + "'";
                    strSQL += ",OrderNo='" + gridView3.GetRowCellValue(intCurrentElement,"PO") + "'";
                    strSQL += ",Knitting='" + gridView3.GetRowCellValue(intCurrentElement,"KNITTING") + "'";
                    strSQL += ",Fabric='" + gridView3.GetRowCellValue(intCurrentElement,"FABRIC") + "'";
                    strSQL += ",Color='" + gridView3.GetRowCellValue(intCurrentElement,"COLOR") + "'";
                    strSQL += ",QtyOrder='" + gridView3.GetRowCellValue(intCurrentElement,"PCS") + "'";
                    strSQL += ",Weight='" + gridView3.GetRowCellValue(intCurrentElement,"KGS") + "'";
                    if (gridView3.GetRowCellValue(intCurrentElement,"DUE_DATE") == System.DBNull.Value)
                    {
                        strSQL += ",DueDate=null";
                    }
                    else
                    {
                        strSQL += ",DueDate='" + ((DateTime)gridView3.GetRowCellValue(intCurrentElement, "DUE_DATE")).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    }
                    strSQL += ",iDate='" + datStart.ToString("yyyyMM", dtfinfo) + "'";
                    strSQL += ",Week='" + gridView3.GetRowCellValue(intCurrentElement,"WEEK") + "'";
                    strSQL += ",Section='" + gridView3.GetRowCellValue(intCurrentElement,"SECTION") + "'";
                    for (int j = firstColumnOfDay; j < gridView3.Columns.Count; j++)
                    {
                        strSQL += ",d" + gridView3.Columns[j].FieldName + "='" + gridView3.GetRowCellValue(intCurrentElement,gridView3.Columns[j].FieldName) + "'";
                    }
                    strSQL += " Where ID=" + gridView3.GetRowCellValue(intCurrentElement,"ID");
                    db.Execute(strSQL);                
                }

                db.CommitTrans();
                MessageBox.Show("Save complete...", "DyeingSchedule", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DisplayData();
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();

            
 
        }

        private void InitializeControl()
        {
            dtpStartDate.EditValue = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpEndDate.EditValue = DateTime.Today;
        }
        private void LoadRepositoryItem()
        {
            string strSQL = "SELECT DISTINCT MACHINENO FROM DYEPLAN ORDER BY MACHINENO";
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            rpMachine = new RepositoryItemComboBox();
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                rpMachine.Items.Add(dr["MACHINENO"].ToString());
            }
        }
        private void LoadHoliday(DateTime datMonth)
        {
            string strSQL = "SELECT HDATE,REMARK FROM HOLIDAY WHERE CONVERT(CHAR(6),HDATE,112)='"+ datMonth.ToString("yyyyMM",dtfinfo)  +"'";
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            htHoliday.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                htHoliday.Add(Convert.ToDateTime(dr["HDATE"]).Day.ToString(), dr["REMARK"].ToString());
            }
        }
        private bool IsHoliday(string strYear, string strMonth, string strDay)
        {
            string strSQL = "SELECT HDATE FROM HOLIDAY WHERE CONVERT(VARCHAR(8),HDATE,112)='" + strYear + strMonth + strDay + "'";
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            if (dt != null && dt.Rows.Count > 0) { return true; }
            else { return false; }
        }

        private void frmSchedule_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            gridView3.OptionsView.ShowGroupPanel = false;
            InitializeControl();
            LoadRepositoryItem();
        }
        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if(e.Column.AbsoluteIndex>0)
                if (Convert.ToDecimal(e.Value) == 0) { e.DisplayText = ""; }            
        }
        //private void gridView2_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        //{
        //    if(e.Column.AbsoluteIndex>0)
        //        if (Convert.ToDecimal(e.Value) == 0) { e.DisplayText = ""; }
        //}
        private void gridView3_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            GridView gv = (GridView)sender;
            if (e.Column.AbsoluteIndex >= firstColumnOfDay && gv.IsNewItemRow(e.GroupRowHandle) == false)// e.RowHandle)==false)
            {
                if (Convert.ToDecimal(e.Value) == 0) { e.DisplayText = ""; }
            }
        }
        private void gridView3_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView gv=(GridView)sender;
            if (gv.IsNewItemRow(gv.FocusedRowHandle) == false)//แถวเก่า
            {
                if(gv.GetRowCellValue(gv.FocusedRowHandle,"ID")!=System.DBNull.Value)//แถวเก่ามี ID
                {
                    aryRowSave.Sort();
                    if (aryRowSave.BinarySearch(gv.FocusedRowHandle) < 0) 
                    {
                        aryRowSave.Add(gv.FocusedRowHandle);//จะเลือกแถวเฉพาะที่เคยมีการเซฟข้อมูลไปแล้วเพื่อการอัพเดทเท่านั้น 
                    }
                }
                //คำนวนจำนวนพับ-----------------------
                if (gv.FocusedColumn.AbsoluteIndex < firstColumnOfDay) { return; }
                try
                {
                    string machineNo = gv.GetRowCellValue(gv.FocusedRowHandle, "MACHINE").ToString();
                    string dueDate = ((DateTime)gv.GetRowCellValue(gv.FocusedRowHandle, "DUE_DATE")).Day.ToString();
                    string dayEdit = gv.FocusedColumn.FieldName;
                    decimal newValue = Convert.ToDecimal(e.Value);

                    for (int i=0; i < gridView1.RowCount; i++)
                    {
                        if (gridView1.GetRowCellValue(i, "MACHINE").ToString() == machineNo)
                        { 
                            decimal sumPCS=Convert.ToDecimal(gridView1.GetRowCellValue(i,dayEdit));
                            gridView1.SetRowCellValue(i, dayEdit, sumPCS + (newValue-oldValue));
                        }
                    }
                    //for (int i = 0; i < gridView2.RowCount; i++)
                    //{
                    //    if (gridView2.GetRowCellValue(i, "MACHINE").ToString() == machineNo)
                    //    {
                    //        decimal sumPCS = Convert.ToDecimal(gridView2.GetRowCellValue(i, dueDate));
                    //        gridView2.SetRowCellValue(i, dueDate, sumPCS + (newValue - oldValue));
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                //--------------------------------------
            }
            else//แถวใหม่
            {
                //คำนวนจำนวนพับ-----------------------
                if (gv.FocusedColumn.AbsoluteIndex < firstColumnOfDay) { return; }
                try
                {
                    string machineNo = gv.GetRowCellValue(gv.FocusedRowHandle, "MACHINE").ToString();
                    string dayEdit = gv.FocusedColumn.FieldName;
                    decimal newValue = Convert.ToDecimal(e.Value);

                    for (int i = 0; i < gridView1.RowCount; i++)
                    {
                        if (gridView1.GetRowCellValue(i, "MACHINE").ToString() == machineNo)
                        {
                            decimal sumPCS = Convert.ToDecimal(gridView1.GetRowCellValue(i, dayEdit));
                            gridView1.SetRowCellValue(i, dayEdit, sumPCS + (newValue - oldValue));
                        }
                    }
                }
                catch{}
                //--------------------------------------

            }
        }
        private void gridView3_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView gv = (GridView)sender;
            if (gv.FocusedColumn.AbsoluteIndex >= firstColumnOfDay)
            {
                oldValue = Convert.ToDecimal(gv.GetFocusedValue());
            }
        }
        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.AbsoluteIndex < 1) { return; }
            DateTime datTemp = new DateTime(datStart.Year, datStart.Month, int.Parse(e.Column.FieldName));
            //ตรวจสอบวันอาทิตย์
            if (datTemp.DayOfWeek == DayOfWeek.Sunday)
            {
                e.Column.AppearanceCell.BackColor = ColorTranslator.FromHtml("#FFFFCC");
            }
            //ตรวจสอบวันหยุด
            else if (htHoliday.ContainsKey(e.Column.FieldName))
            {
                e.Column.AppearanceCell.BackColor = ColorTranslator.FromHtml("#FFFFCC");
            }
            //ตรวจสอบตรงกับวันนี้
            else if (datTemp == DateTime.Today)
            {
                e.Column.AppearanceCell.BackColor = Color.Gainsboro;
            }
            
        }
        //private void gridView2_RowCellStyle(object sender, RowCellStyleEventArgs e)
        //{
        //    if (e.Column.AbsoluteIndex < 1) { return; }
        //    DateTime datTemp = new DateTime(datStart.Year, datStart.Month, int.Parse(e.Column.FieldName));
        //    //ตรวจสอบวันอาทิตย์
        //    if (datTemp.DayOfWeek == DayOfWeek.Sunday)
        //    {
        //        e.Column.AppearanceCell.BackColor = ColorTranslator.FromHtml("#FFFFCC");
        //    }
        //    //ตรวจสอบวันหยุด
        //    else if (htHoliday.ContainsKey(e.Column.FieldName))
        //    {
        //        e.Column.AppearanceCell.BackColor = ColorTranslator.FromHtml("#FFFFCC");
        //    }
        //    //ตรวจสอบตรงกับวันนี้
        //    else if (datTemp == DateTime.Today)
        //    {
        //        e.Column.AppearanceCell.BackColor = Color.Gainsboro;
        //    }
        //}
        private void gridView3_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.AbsoluteIndex < firstColumnOfDay) { return; }
            DateTime datTemp = new DateTime(datStart.Year, datStart.Month, int.Parse(e.Column.FieldName));
            //ตรวจสอบวันอาทิตย์
            if (datTemp.DayOfWeek == DayOfWeek.Sunday)
            {
                e.Column.AppearanceCell.BackColor = ColorTranslator.FromHtml("#FFFFCC");
            }
            //ตรวจสอบวันหยุด
            else if (htHoliday.ContainsKey(e.Column.FieldName))
            {
                e.Column.AppearanceCell.BackColor = ColorTranslator.FromHtml("#FFFFCC");
            }
            //ตรวจสอบตรงกับวันนี้
            else if (datTemp == DateTime.Today)
            {
                e.Column.AppearanceCell.BackColor = Color.Gainsboro;
            }
        
        }
        private void gridView3_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "DUE_DATE")
            {
                RepositoryItemDateEdit rpDueDate = new RepositoryItemDateEdit();
                rpDueDate.Mask.EditMask = "dd/MM/yyyy";
                rpDueDate.Mask.UseMaskAsDisplayFormat = true;
                e.RepositoryItem = rpDueDate;
            }
        }
        private void gridView3_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    StatusBarEvent(e.CellValue.ToString());
                }
                else if (e.Button == MouseButtons.Right)
                { 
                    GridView gv=(GridView)sender;
                    if (gv.IsRowSelected(e.RowHandle))
                    {
                        popupMenu1.ShowPopup(new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y));
                    }
                }
            }
            catch{}
        }
        private void bbiCopy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            myArray = new object[gridView3.Columns.Count];
            for (int i = 0; i < gridView3.Columns.Count; i++)
            {
                myArray[i] = gridView3.GetRowCellValue(gridView3.FocusedRowHandle, gridView3.Columns[i].FieldName);
            }
        }
        private void bbiPaste_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = ds1.Tables[1].NewRow();
                dr.ItemArray = myArray;
                dr["ID"] = System.DBNull.Value;
                ds1.Tables[1].Rows.InsertAt(dr, gridView3.FocusedRowHandle);
                gridControl3.DataSource = ds1.Tables[1];
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void bbiDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("คุณต้องการลบข้อมูลแถวนี้หรือไม่. ID " + gridView3.GetRowCellValue(gridView3.FocusedRowHandle,"ID"), "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                db.ConnectionOpen();
                try
                {
                    db.BeginTrans();
                    string strSQL = "Delete From DyePlan Where ID='" + gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "ID") + "'";
                    db.Execute(strSQL);
                    gridView3.DeleteRow(gridView3.FocusedRowHandle);
                    db.CommitTrans();
                }
                catch (Exception ex)
                {
                    db.RollbackTrans();
                    MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                { 
                    db.ConnectionClose();
                }
              }
        }
        private void gridView3_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            GridView gv=(GridView)sender;
            for (int i = firstColumnOfDay; i < gv.Columns.Count; i++)
            {
                gv.SetRowCellValue(e.RowHandle, gv.Columns[i].FieldName, 0);
            }
        }
        private void gridView3_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0) e.Info.DisplayText = (e.RowHandle + 1).ToString();
        }
        private void chkShowFooter_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowFooter.Checked)
            {
                gridView1.OptionsView.ShowFooter = true;
                //gridView2.OptionsView.ShowFooter = true;
            }
            else
            {
                gridView1.OptionsView.ShowFooter = false;
                //gridView2.OptionsView.ShowFooter = false;
            }
        }

    }
}