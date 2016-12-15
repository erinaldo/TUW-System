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
using System.IO;
using myClass;

namespace TUW_System
{
    public partial class frmTS1_ImportIF : DevExpress.XtraEditors.XtraForm
    {
        CultureInfo clinto = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        cDatabase db = new cDatabase(Module.TUW99);
        cDatabase db2 = new cDatabase(Module.Fabric);

        public frmTS1_ImportIF()
        {
            InitializeComponent();
        }
        public void ClearData()
        {
            dtpCheckIn.EditValue = "";
            gridControl1.DataSource = null;
        }
        public void SaveData()
        { 
        
        }
        public void DisplayData()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strSQL="EXEC spTPiCSSubsystem_ImportIF '"+((DateTime)dtpCheckIn.EditValue).ToString("yyyy-MM-dd",dtfinfo)+"'";
                System.Data.DataTable dt = db.GetDataTable(strSQL);
                if (dt != null)
                {
                    gridControl1.DataSource = dt;
                    gridView1.OptionsView.EnableAppearanceEvenRow = true;
                    gridView1.OptionsView.EnableAppearanceOddRow = true;
                    gridView1.OptionsView.ColumnAutoWidth = false;
                    gridView1.BestFitColumns();
                }
            }
            catch ( Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                gridView1.ExportToXls(strTemp,false);
            }
        }
        public void ExportCSV()
        {
            try
            {
                SaveFileDialog theOpenFile = new SaveFileDialog();
                string strTemp;
                theOpenFile.Filter = "CSV (Comma delimited)|*.csv";
                if (theOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    strTemp = theOpenFile.FileName + ".csv";
                    TextWriter tw = new StreamWriter(strTemp);
                    //Header column
                    tw.WriteLine("DataClass,Order#,Branch#,Partial#,ItemCode,Name,WorkCenter,Supplier,Act.Class,Act.QtyCorresp.Schd.,Act.QtyCorresp.Inv.,FinishDate,RemarkA,Pur.OrderCode,Act.Pur.U/Cost,Putaway location,LOTNO");
                    //Detail
                    for (int i = 0; i < gridView1.DataRowCount; i++)
                    {
                        string strTPiCSCode = GetFabricID(gridView1.GetRowCellDisplayText(i, "FABRICCODE")) + "-" + gridView1.GetRowCellDisplayText(i, "COLOR");
                        tw.Write("," + gridView1.GetRowCellDisplayText(i, "FABORDERNO") + ",,");
                        tw.Write("," + strTPiCSCode);
                        tw.Write(","+gridView1.GetRowCellDisplayText(i,"CODE_NEW"));
                        tw.Write(",FDEL,FDEL,T");
                        tw.Write("," + gridView1.GetRowCellDisplayText(i, "QTY") + "," + gridView1.GetRowCellDisplayText(i, "QTY"));
                        tw.Write("," + gridView1.GetRowCellDisplayText(i, "INDATE"));
                        tw.Write(","+gridView1.GetRowCellDisplayText(i,"CUSTOMER"));
                        tw.Write("," + strTPiCSCode);
                        tw.Write(","+gridView1.GetRowCellDisplayText(i,"PRICE"));
                        if (gridView1.GetRowCellDisplayText(i, "LOTNO").Substring(0, 3) == "S1T")
                        {
                            tw.Write(",ST05-S1-S");
                        }
                        else
                        {
                            tw.Write(",ST05-S1");
                        }
                        tw.WriteLine(","+gridView1.GetRowCellDisplayText(i,"LOTNO"));
                    }
                    tw.Close();
                }
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private string GetFabricID(string strInput)
        {
            string strResult = "";
            string strSQL = "SELECT ID FROM GREYFABRIC WHERE CODE='" + strInput + "'";
            db.ConnectionOpen();
            strResult=db.ExecuteFirstValue(strSQL);
            db.ConnectionClose();
            return strResult;
        }

        private void frmImportIF_Load(object sender, EventArgs e)
        {
            dtfinfo = clinto.DateTimeFormat;

        }
        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "CODE_NEW")
            {
                if (!Equals(e.CellValue.ToString(), gridView1.GetRowCellDisplayText(e.RowHandle, "CODE_OLD")))
                {
                    e.Appearance.ForeColor = Color.Red;
                }
 
            }
        }




    }
}