using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;
using Microsoft.Office.Interop.Excel;

namespace TUW_System
{
    public partial class frmFS_ChangePrice : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db = new cDatabase(Module.TUW99);
        System.Data.DataTable dtMain;
        mdiMain parent;

        public frmFS_ChangePrice()
        {
            InitializeComponent();
        }
        public void NewData()
        { 
            gridControl1.DataSource = null;
            dtMain = null;
            parent.UpdateStatusBar("");
        }
        public void SaveData()
        {
            this.Cursor = Cursors.WaitCursor;
            parent.Progressbar_Initialize(0, gridView1.DataRowCount-1);
            try
            {
                db.ConnectionOpen();
                db.BeginTrans();
                for(int i=0;i<gridView1.DataRowCount;i++)
                {
                    //ยิงออก
                    string strSQL = "UPDATE FINISHFABRICNC SET CUTTINGORDER='"+gridView1.GetRowCellValue(i,"BOOKING")+"'" +
                        ",SYSDELETE=1,OUTEMPNAME='" + parent.User_Login.FirstName + " " + parent.User_Login.LastName + "',OUTDATE=GETDATE() " +
                        ",BOOKING='"+gridView1.GetRowCellValue(i,"BOOKING")+"'"+
                        "WHERE CODE ='" +gridView1.GetRowCellValue(i,"CODE") + "' AND SERIAL ='" +gridView1.GetRowCellValue(i,"SERIAL") + "'";
                    db.Execute(strSQL);
                    strSQL = "UPDATE FINISHFABRICNCDETAIL SET OUTTYPE=3,OUTDATE=GETDATE()" +
                        ",OUTEMP='" + parent.User_Login.FirstName +" "+ parent.User_Login.LastName + "'"+
                        ",CUTTINGORDER='"+gridView1.GetRowCellValue(i,"BOOKING")+"'" +
                        ",SYSDELETE=1 WHERE ID=" +gridView1.GetRowCellValue(i,"ID");
                    db.Execute(strSQL);
                    //ยิงเข้า
                    strSQL="UPDATE FINISHFABRICNC SET INDATE=GETDATE(),OUTDATE=NULL,CUTTINGORDER=''"+
                        ",SYSDELETE=0,RECEIVENO='Return',PRICE="+gridView1.GetRowCellValue(i,"PRICE")+
                        ",EMPNAME='" + parent.User_Login.FirstName +" "+ parent.User_Login.LastName + "'"+
                        ",DELIVERYNO='CHANGEPRICE " +gridView1.GetRowCellValue(i,"PRICE") + " B'" +
                        ",COMNAME='"+System.Environment.MachineName+"'"+
                        " WHERE CODE='"+gridView1.GetRowCellValue(i,"CODE")+"' AND SERIAL='"+gridView1.GetRowCellValue(i,"SERIAL")+"'";
                    db.Execute(strSQL);
                    strSQL="INSERT INTO FINISHFABRICNCDETAIL(CODE,SERIAL,PIECENO,LOTNO,RECEIVENO,ORDERNO,COLORNO,DEFFECT,PRICE"+
                        ",SECTION,CUSTOMERID,WIDHT,COURSE,INTYPE,INDATE,WEIGHT,LENGTH,QTY,LENGTH_UNIT,UNIT,INEMP,LOCATION"+
                        ",DELIVERYNO,COMNAME,FABRICTYPE)VALUES("+
                        "'"+gridView1.GetRowCellValue(i,"CODE")+"','"+gridView1.GetRowCellValue(i,"SERIAL")+"','"+gridView1.GetRowCellValue(i,"PIECENO")+"'"+
                        ",'"+gridView1.GetRowCellValue(i,"LOTNO")+"','Return','"+gridView1.GetRowCellValue(i,"ORDERNO")+"','"+gridView1.GetRowCellValue(i,"COLORNO")+"'"+
                        ",'"+gridView1.GetRowCellValue(i,"DEFFECT")+"',"+gridView1.GetRowCellValue(i,"PRICE")+",'"+gridView1.GetRowCellValue(i,"SECTION")+"'"+
                        ",'"+gridView1.GetRowCellValue(i,"CUSTOMERID")+"'";
                    strSQL += (gridView1.GetRowCellDisplayText(i, "WIDHT").Length == 0) ? ",'0'" : ",'" + gridView1.GetRowCellValue(i, "WIDHT") + "'";
                    strSQL += (gridView1.GetRowCellDisplayText(i, "COURSE").Length == 0) ? ",0" : "," + gridView1.GetRowCellValue(i, "COURSE");
                    strSQL+=",5,GETDATE(),"+gridView1.GetRowCellValue(i,"WEIGHT")+","+gridView1.GetRowCellValue(i,"LENGTH")+","+gridView1.GetRowCellValue(i,"QTY")+
                        ",'"+gridView1.GetRowCellValue(i,"LENGTH_UNIT")+"','"+gridView1.GetRowCellValue(i,"UNIT")+"'"+
                        ",'"+ parent.User_Login.FirstName +" "+ parent.User_Login.LastName + "','"+gridView1.GetRowCellValue(i,"LOCATION")+"'"+
                        ",'CHANGEPRICE " +gridView1.GetRowCellValue(i,"PRICE")+" B','" +System.Environment.MachineName+"'"+
                        ",'"+gridView1.GetRowCellValue(i,"FABRICTYPE")+"')";
                    db.Execute(strSQL);
                    parent.Progressbar_Update(i);
                    parent.Refresh();
                }
                db.CommitTrans();
                MessageBox.Show("Save complete...", "ChangePrice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                NewData();
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
            parent.Progressbar_Hide();
            this.Cursor = Cursors.Default;

        }
        private void ImportExcel()
        {
            db.ConnectionOpen();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Excel files|*.xls;*.xlsx";
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                db.ConnectionClose();
                return;
            }
            Microsoft.Office.Interop.Excel.Application xlApp=null;
            Microsoft.Office.Interop.Excel._Workbook xlBook=null;
            Microsoft.Office.Interop.Excel.Sheets xlSheets=null;
            Microsoft.Office.Interop.Excel._Worksheet xlSheet = null;
            object misVal = System.Reflection.Missing.Value;
            try
            {
                string strValue = "1";//กำหนดค่าชีตแรกเป็นค่าเริ่มต้น
                int firstRow = 2;//แถวที่จะอ่านข้อมูล
                
                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlBook = xlApp.Workbooks.Open(dlg.FileName);
                xlSheets = xlBook.Worksheets;
                if (Module.InputBox("Sheet", "โปรดระบุ sheet ลำดับที่ต้องการอิมพอร์ต", ref strValue) != DialogResult.OK) throw new ApplicationException("Cancel");
                xlSheet = (_Worksheet)xlSheets.get_Item(Convert.ToInt16(strValue));
                parent.Progressbar_Initialize(minValue: 0, maxValue: xlSheet.UsedRange.EntireRow.Count);
                for (int i = firstRow; i <= xlSheet.UsedRange.EntireRow.Count;i++ )
                {
                    string strSQL = "SELECT CODE,LOTNO,COLORNO,'' AS BOOKING,PRICE,ID,SERIAL,PIECENO,ORDERNO,DEFFECT,SECTION,CUSTOMERID" +
                        ",WIDHT,COURSE,WEIGHT,LENGTH,QTY,LENGTH_UNIT,UNIT,LOCATION,FABRICTYPE "+
                        "FROM FINISHFABRICNCDETAIL WHERE LOTNO='" + xlSheet.Range[xlSheet.Cells[i, 2], xlSheet.Cells[i, 2]].Value + "' " +
                        "AND CODE='" + xlSheet.Range[xlSheet.Cells[i, 1], xlSheet.Cells[i, 1]].Value + "' " +
                        "AND COLORNO='" + xlSheet.Range[xlSheet.Cells[i, 3], xlSheet.Cells[i, 3]].Value + "' AND SYSDELETE=0";
                    if (dtMain == null)
                    {
                        dtMain = db.GetDataTable(strSQL);
                        foreach (DataRow dr in dtMain.Rows)
                        {
                            dr["BOOKING"] = xlSheet.Range[xlSheet.Cells[i, 4], xlSheet.Cells[i, 4]].Value;
                            dr["PRICE"] = xlSheet.Range[xlSheet.Cells[i, 5], xlSheet.Cells[i, 5]].Value;
                        }
                    }
                    else
                    {
                        System.Data.DataTable dt = db.GetDataTable(strSQL);
                        foreach (DataRow dr in dt.Rows)
                        {
                            dr["BOOKING"] = xlSheet.Range[xlSheet.Cells[i, 4], xlSheet.Cells[i, 4]].Value;
                            dr["PRICE"] = xlSheet.Range[xlSheet.Cells[i, 5], xlSheet.Cells[i, 5]].Value;
                            DataRow drMain = dtMain.NewRow();
                            drMain.ItemArray = dr.ItemArray;
                            dtMain.Rows.InsertAt(drMain, dtMain.Rows.Count);
                        }
                    }

                    parent.Progressbar_Update(i);
                    parent.Refresh();
                }
                gridControl1.DataSource = dtMain;
                gridView1.PopulateColumns();
                gridView1.OptionsView.EnableAppearanceEvenRow = true;
                gridView1.OptionsView.EnableAppearanceOddRow = true;
                gridView1.OptionsView.ColumnAutoWidth = false;
                gridView1.BestFitColumns();
                foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gridView1.Columns)
                {
                    dc.OptionsColumn.ReadOnly = true;
                }
                //gridView1.Columns["Comment"].OptionsColumn.AllowEdit = true;
                //gridView1.Columns["Booking_Date"].OptionsColumn.AllowEdit = true;
                parent.UpdateStatusBar("("+dtMain.Rows.Count+" row(s) affected)");
                parent.Progressbar_Hide();
            }
            catch (ApplicationException) { }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheet);
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
            xlBook.Close(false,dlg.FileName,null);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlBook);
            xlApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            db.ConnectionClose();
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
        private void btnImport_Click(object sender, EventArgs e)
        {
            NewData();
            ImportExcel();
        }
        private void frmFS_ChangePrice_Load(object sender, EventArgs e)
        {
            parent = (mdiMain)this.ParentForm;
        }
        private void frmFS_ChangePrice_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.UpdateStatusBar("");
        }
        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && !gridView1.IsEditing)
            {
                if (MessageBox.Show("คุณต้องการลบข้อมูลแถวที่ "+(gridView1.FocusedRowHandle+1)+" ใช่หรือไม่", "Delete row", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    gridView1.DeleteSelectedRows();
                    
                }
            }
        }


    }
}