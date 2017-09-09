using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System.AC
{
    public partial class frmAC_UpdateData : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;

        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmAC_UpdateData()
        {
            InitializeComponent();
        }
        public void ClearData()
        { 
        
        }
        public void SaveData()
        {
            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                gridView1.CloseEditor();
                gridView1.UpdateCurrentRow();
                string strSQL;
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    if (((bool)gridView1.GetRowCellValue(i, "EDIT")) == false) continue;
                    strSQL = "update expbillrecord set " +
                        "inv_desc = '" + gridView1.GetRowCellValue(i, "inv_desc").ToString().Replace("'", "''") + "'" +
                        ",custname = '" + gridView1.GetRowCellValue(i, "custname").ToString().Replace("'", "''") + "'" +
                        ",exinv_no = '" + gridView1.GetRowCellValue(i, "exinv_no") + "' " +
                        "where invoice_no = '" + gridView1.GetRowCellValue(i, "invoice_no") + "'";
                    db.Execute(strSQL);
                    strSQL = "update expbillreceive set custname = '" + gridView1.GetRowCellValue(i, "custname").ToString().Replace("'", "''") +
                        "' where invoice_no = '" + gridView1.GetRowCellValue(i, "invoice_no") + "'";
                    db.Execute(strSQL);
                }
                db.CommitTrans();
                MessageBox.Show("Save complete.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
            //Dim i As Integer
            //Dim j As Integer
            //    OpenDatabaseSys
            //    For i = 1 To FGrid.Rows - 1
            //        SqlStr = "update expbillrecord set inv_desc = '" & Replace(FGrid.TextMatrix(i, 1), "'", "|") & "'" & _
            //        ",custname = '" & Replace(FGrid.TextMatrix(i, 2), "'", "''") & "',exinv_no = '" & FGrid.TextMatrix(i, 3) & "'" & _
            //        " where invoice_no = '" & FGrid.TextMatrix(i, 0) & "'"
            //        Dbs.Execute SqlStr
            //        SqlStr = "update expbillreceive set custname = '" & Replace(FGrid.TextMatrix(i, 2), "'", "''") & "' where invoice_no = '" & FGrid.TextMatrix(i, 0) & "'"
            //        Dbs.Execute SqlStr
            //    Next i
            //    CloseDB
            //    MsgBox "Save Data Complete!"
        }
        public void DisplayData()
        {
            try
            {
                GetInvoiceDetail((cboMonth.SelectedIndex+1).ToString(),cboYear.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetInvoiceDetail(string strMonth,string strYear)
        {
            string strSQL = "select invoice_no,inv_desc,custname,exinv_no,dept_id from expbillrecord " +
                " where datepart(mm,inv_date) = '"+ strMonth+"'"+
                " and datepart(yyyy,inv_date) = '"+strYear+"' order by invoice_no";
            DataTable dt = db.GetDataTable(strSQL);
            dt.BeginInit();
            DataColumn dc = new DataColumn();
            dc.DataType = typeof(System.Boolean);
            dc.ColumnName = "EDIT";
            dc.DefaultValue = false;
            dt.Columns.Add(dc);
            dt.EndInit();
            gridControl1.DataSource = dt;
            gridView1.PopulateColumns();
            gridView1.Columns["invoice_no"].Caption = "Invoice No.";
            gridView1.Columns["inv_desc"].Caption = "Description";
            gridView1.Columns["custname"].Caption = "Customer";
            gridView1.Columns["exinv_no"].Caption = "No.";
            gridView1.Columns["dept_id"].Caption = "Department";
            gridView1.Columns["invoice_no"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["dept_id"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["EDIT"].Visible = false;

            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            StatusBarEvent(gridView1.DataRowCount + " Rows.");
        }

        private void frmAC_UpdateData_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            cboMonth.SelectedIndex = DateTime.Today.Month - 1;
            for (int i = 0; i < 10; i++)
            {
                cboYear.Properties.Items.Add(DateTime.Today.AddYears(-i).Year);
            }
            cboYear.SelectedIndex = 0;
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator) e.Info.DisplayText = (e.RowHandle + 1).ToString(); 
            gridView1.IndicatorWidth = 45;
        }
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            string[] field=new string[]{"inv_desc","custname","exinv_no"};
            if(field.Contains(e.Column.FieldName)) gridView1.SetRowCellValue(e.RowHandle, "EDIT", true);
        }
    }
}





//Private Sub FGrid_KeyDown(KeyCode As Integer, Shift As Integer)
//    Select Case KeyCode
//        Case 67
//            'If FGrid.Col = 0 Then
//                If Shift = 2 Then
//                    CRow = FGrid.Row
//                    CCol = FGrid.Col
//                End If
//            'End If
//        Case 86
//            'If FGrid.Col = 0 Then
//                If Shift = 2 Then
//                    'For i = 1 To FGrid.Cols - 1
//                        FGrid.TextMatrix(FGrid.Row, FGrid.Col) = FGrid.TextMatrix(CRow, CCol)
//                    'Next i
//                End If
//            'End If
//    End Select
//End Sub



//Function ChkMonthString(zMon As String) As String
//    Select Case zMon
//        Case "January"
//            ChkMonthString = "01"
//        Case "February"
//            ChkMonthString = "02"
//        Case "March"
//            ChkMonthString = "03"
//        Case "April"
//            ChkMonthString = "04"
//        Case "May"
//            ChkMonthString = "05"
//        Case "June"
//            ChkMonthString = "06"
//        Case "July"
//            ChkMonthString = "07"
//        Case "August"
//            ChkMonthString = "08"
//        Case "September"
//            ChkMonthString = "09"
//        Case "October"
//            ChkMonthString = "10"
//        Case "November"
//            ChkMonthString = "11"
//        Case "December"
//            ChkMonthString = "12"
//    End Select
//End Function


