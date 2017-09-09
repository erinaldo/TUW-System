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
using DevExpress.XtraEditors.Repository;
using myClass;

namespace TUW_System.AC
{
    public partial class frmAC_DomesticList : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;
        
        cDatabase db;
        CultureInfo clinfo=new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        RepositoryItemSearchLookUpEdit rpCustomers;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmAC_DomesticList()
        {
            InitializeComponent();
        }
        public void ClearData()
        {
            cboMonth.SelectedIndex = DateTime.Today.Month - 1;
            cboYear.SelectedIndex = 0;
            gridControl1.DataSource = null;
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
                    strSQL = "update domesticinvmain set ";
                    if(gridView1.GetRowCellValue(i,"invoicedate")==System.DBNull.Value)
                        strSQL+="invoicedate=null";
                    else
                        strSQL+="invoicedate ='"+((DateTime)gridView1.GetRowCellValue(i,"invoicedate")).ToString("yyyy-MM-dd",dtfinfo)+"'";
                    strSQL+=",descr = N'"+gridView1.GetRowCellValue(i,"descr")+"'";
                    strSQL+=",cust_no = '"+gridView1.GetRowCellValue(i,"cust_no")+"'";
                    strSQL+=",payment = '"+gridView1.GetRowCellValue(i,"payment")+"'";
                    strSQL+= ",credit = '" + gridView1.GetRowCellValue(i, "credit") + "'";
                    strSQL+=",section = '"+gridView1.GetRowCellValue(i,"section")+"'";
                    strSQL+=",idno = '"+gridView1.GetRowCellValue(i,"idno")+"'";
                    strSQL += ",unit = N'" + gridView1.GetRowCellValue(i, "unit") + "'";
                    strSQL += " where invoiceno = '" + gridView1.GetRowCellValue(i, "invoiceno") + "'";
                    db.Execute(strSQL);
                    strSQL = "delete from domesticinvdept where invoiceno='" + gridView1.GetRowCellValue(i, "invoiceno") + "'";
                    db.Execute(strSQL);
                    decimal qty,amt,vat,amount;
                    switch (gridView1.GetRowCellValue(i, "section").ToString().ToUpper())
                    { 
                        case "NC-F"://Inter Sales 80% ,Direct Sales 20%
                            qty=Convert.ToDecimal(gridView1.GetRowCellValue(i,"qty"))*0.8m;
                            amt = Convert.ToDecimal(gridView1.GetRowCellValue(i, "amt")) * 0.8m;
                            vat = Convert.ToDecimal(gridView1.GetRowCellValue(i, "vat")) * 0.8m;
                            amount = Convert.ToDecimal(gridView1.GetRowCellValue(i, "amount")) * 0.8m;
                            strSQL = "insert into domesticinvdept (invoiceno,seq,section,qty,amt,vat,amount) values" +
                                "('" + gridView1.GetRowCellValue(i, "invoiceno") + "',1,'Inter Sales'," + qty + "," + amt + "," + vat + "," + amount + ")";
                            db.Execute(strSQL);
                            qty=Convert.ToDecimal(gridView1.GetRowCellValue(i,"qty"))*0.2m;
                            amt = Convert.ToDecimal(gridView1.GetRowCellValue(i, "amt")) * 0.2m;
                            vat = Convert.ToDecimal(gridView1.GetRowCellValue(i, "vat")) * 0.2m;
                            amount = Convert.ToDecimal(gridView1.GetRowCellValue(i, "amount")) * 0.2m;
                            strSQL = "insert into domesticinvdept (invoiceno,seq,section,qty,amt,vat,amount) values" +
                                "('" + gridView1.GetRowCellValue(i, "invoiceno") + "',2,'Direct Sales'," + qty + "," + amt + "," + vat + "," + amount + ")";
                            db.Execute(strSQL);
                            break;
                        case "PARFUN,RIKI":
                        case "SP-F": //Riki 50%, Parfun 50%
                            qty =Convert.ToDecimal(gridView1.GetRowCellValue(i,"qty"))/2m;
                            amt=Convert.ToDecimal(gridView1.GetRowCellValue(i,"amt"))/2m;
                            vat=Convert.ToDecimal(gridView1.GetRowCellValue(i,"vat"))/2m;
                            amount=Convert.ToDecimal(gridView1.GetRowCellValue(i,"amount"))/2m;
                            strSQL="insert into domesticinvdept (invoiceno,seq,section,qty,amt,vat,amount) values"+
                                "('"+gridView1.GetRowCellValue(i,"invoiceno")+"',1,'Riki',"+qty+","+amt+","+vat+","+amount+")";
                            db.Execute(strSQL);
                            strSQL="insert into domesticinvdept (invoiceno,seq,section,qty,amt,vat,amount) values"+
                                "('"+gridView1.GetRowCellValue(i,"invoiceno")+"',2,'Parfun',"+qty+","+amt+","+vat+","+amount+")";
                            db.Execute(strSQL);
                            break;
                    }

                    //MessageBox.Show(strSQL);
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
        }
        public void DisplayData()
        {
            try
            {
                GetInvoiceDetail((cboMonth.SelectedIndex+1).ToString() ,cboYear.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }

        private RepositoryItemSearchLookUpEdit GetCustomers()
        {
            //string strSQL = "select distinct cust_no,custnamee from domesticinvmain inner join customeracc on domesticinvmain.cust_no = customeracc.cust_no order by custnamee";
            string strSQL = "select cust_no,custnamee from customeracc";
            DataTable dt = db.GetDataTable(strSQL);
            RepositoryItemSearchLookUpEdit rp = new RepositoryItemSearchLookUpEdit();
            rp.DataSource = dt;
            rp.PopulateViewColumns();
            rp.DisplayMember = "custnamee";
            rp.ValueMember = "cust_no";
            rp.PopupSizeable = true;
            return rp;

        }
        private void GetInvoiceDetail(string strMonth,string strYear)
        {
            string strSQL = "select a.invoiceno,a.invoicedate,a.idno,a.descr,a.cust_no,a.cust_no as custnamee" +
                ",a.payment,a.credit,a.section,a.qty,a.unit,a.amt,a.vat,a.amount " +
                "from domesticinvmain a " +
                "where datepart(mm,a.invoicedate) = '" + strMonth + "' " +
                "and datepart(yyyy,invoicedate) = '" + strYear + "' order by a.invoiceno";
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
            gridView1.Columns["invoiceno"].Caption = "Invoice No.";
            gridView1.Columns["invoicedate"].Caption = "Inv. Date";
            gridView1.Columns["idno"].Caption = "Id No.";
            gridView1.Columns["descr"].Caption = "Description";
            gridView1.Columns["cust_no"].Caption = "Customer No.";
            gridView1.Columns["custnamee"].Caption = "Customer";
            gridView1.Columns["payment"].Caption = "Credit";
            gridView1.Columns["credit"].Caption = "Days";
            gridView1.Columns["section"].Caption = "Department";
            gridView1.Columns["qty"].Caption = "Quantity";
            gridView1.Columns["unit"].Caption = "Unit";
            gridView1.Columns["amt"].Caption = "Amount";
            gridView1.Columns["vat"].Caption = "Vat";
            gridView1.Columns["amount"].Caption = "Sales";

            gridView1.Columns["EDIT"].Visible = false;

            gridView1.Columns["qty"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["qty"].DisplayFormat.FormatString = "n1";
            gridView1.Columns["amt"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["amt"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["vat"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["vat"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["amount"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["amount"].DisplayFormat.FormatString = "n2";

            gridView1.Columns["qty"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.0}");
            gridView1.Columns["amt"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridView1.Columns["vat"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridView1.Columns["amount"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            
            gridView1.OptionsView.ShowFooter = true;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            StatusBarEvent(gridView1.DataRowCount + " Rows.");

        }

        private void frmAC_DomesticList_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            for (int i = 0; i < 10; i++)
            {
                cboYear.Properties.Items.Add(DateTime.Today.AddYears(-i).Year);
            }
            rpCustomers = GetCustomers();
            ClearData();

            //    SqlStr = 
            //        Set Rs = Dbs.Execute(SqlStr)
            //        If Not Rs.EOF Then
            //            zCust = ""
            //            Do While Not Rs.EOF
            //                If zCust = "" Then
            //                    zCust = Rs!custnamee
            //                Else
            //                    zCust = zCust & "|" & Rs!custnamee
            //                End If
            //                Rs.MoveNext
            //            Loop
            //            FGrid.ColComboList(5) = zCust
            //        End If
        
        }
        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "custnamee")
            {
                e.RepositoryItem = rpCustomers;
            }
        }
        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gridView1.FocusedColumn.FieldName == "custnamee")
            {
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "cust_no",e.Value);
            }
        }
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            string[] field = new string[] { "invoicedate", "descr", "cust_no", "payment", "credit", "section", "idno", "unit" };
            if (field.Contains(e.Column.FieldName)) gridView1.SetRowCellValue(e.RowHandle, "EDIT", true);
        }
    }
}



//Private Sub FGrid_AfterEdit(ByVal Row As Long, ByVal Col As Long)
//    Select Case Col
//        Case 4
//            OpenDatabaseSys
//            SqlStr = "select custnamee from customeracc where cust_no = N'" & FGrid.TextMatrix(Row, 4) & "'"
//            Set Rs = Dbs.Execute(SqlStr)
//            If Not Rs.EOF Then
//                FGrid.TextMatrix(Row, 5) = Rs!custnamee
//            End If
//            Rs.Close
//            CloseDB
//        Case 5
//            OpenDatabaseSys
//            SqlStr = "select cust_no from customeracc where custnamee = N'" & FGrid.TextMatrix(Row, 5) & "'"
//            Set Rs = Dbs.Execute(SqlStr)
//            If Not Rs.EOF Then
//                FGrid.TextMatrix(Row, 4) = Rs!cust_no
//            End If
//            Rs.Close
//            CloseDB
//        Case 9, 11, 12, 13
//            FGrid.TextMatrix(Row, Col) = Format(FGrid.TextMatrix(Row, Col), "#,##0.00")
//    End Select
//End Sub

//Private Sub FGrid_KeyDown(KeyCode As Integer, Shift As Integer)
//    Select Case KeyCode
//        Case 46
//            FGrid.TextMatrix(FGrid.RowSel, FGrid.ColSel) = ""
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
//        Case 13
//            If FGrid.Col <> FGrid.Cols - 1 Then
//                FGrid.Col = FGrid.Col + 1
//            Else
//                FGrid.Row = FGrid.Row + 1
//                FGrid.Col = 0
//            End If
//    End Select
//End Sub


