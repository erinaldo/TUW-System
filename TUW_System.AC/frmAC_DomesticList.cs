using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        DataTable dtCustomers;
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
            //Private Sub CmdSave_Click()
            //Dim i As Integer
            //Dim j As Integer
            //Dim zQty As Double
            //Dim zAmt As Double
            //Dim zVat As Double
            //Dim zAmount As Double

            //    Me.MousePointer = 11
            //    OpenDatabaseSys
            //    For i = 1 To FGrid.Rows - 1

            //        SqlStr = "update domesticinvmain set invoicedate = '" & Format(FGrid.TextMatrix(i, 1), "mm/dd/yyyy") & "'" & _
            //        ",descr = N'" & FGrid.TextMatrix(i, 3) & "',cust_no = '" & FGrid.TextMatrix(i, 4) & "',payment = '" & FGrid.TextMatrix(i, 6) & "'" & _
            //        ",credit = '" & ChkNumeric(FGrid.TextMatrix(i, 7)) & "'" & _
            //        ",section = '" & FGrid.TextMatrix(i, 8) & "',idno = '" & FGrid.TextMatrix(i, 9) & "',unit = N'" & FGrid.TextMatrix(i, 10) & "'" & _
            //        " where invoiceno = '" & FGrid.TextMatrix(i, 0) & "'"
            //        Dbs.Execute SqlStr
            //       SqlStr = "delete from domesticinvdept where invoiceno = '" & FGrid.TextMatrix(i, 0) & "'"
            //        Dbs.Execute SqlStr
            //        zQty = 0
            //        zAmt = 0
            //        zVat = 0
            //        zAmount = 0

            //        Select Case UCase(Trim(FGrid.TextMatrix(i, 8)))
            //            Case "NC-F" 'Inter Sales 80% ,Direct Sales 20%
            //                zQty = ChkNumeric(FGrid.TextMatrix(i, 9)) * 80 / 100
            //                zAmt = ChkNumeric(FGrid.TextMatrix(i, 11)) * 80 / 100
            //                zVat = ChkNumeric(FGrid.TextMatrix(i, 12)) * 80 / 100
            //                zAmount = ChkNumeric(FGrid.TextMatrix(i, 13)) * 80 / 100
            //                SqlStr = "insert into domesticinvdept (invoiceno,seq,section,qty,amt,vat,amount) values" & _
            //                "('" & FGrid.TextMatrix(i, 0) & "',1,'Inter Sales'," & zQty & "," & zAmt & "," & zVat & "," & zAmount & ")"
            //                Dbs.Execute SqlStr
            //                zQty = ChkNumeric(FGrid.TextMatrix(i, 9)) - zQty
            //                zAmt = ChkNumeric(FGrid.TextMatrix(i, 11)) - zAmt
            //                zVat = ChkNumeric(FGrid.TextMatrix(i, 12)) - zVat
            //                zAmount = ChkNumeric(FGrid.TextMatrix(i, 13)) - zAmount
            //                SqlStr = "insert into domesticinvdept (invoiceno,seq,section,qty,amt,vat,amount) values" & _
            //                "('" & FGrid.TextMatrix(i, 0) & "',2,'Direct Sales'," & zQty & "," & zAmt & "," & zVat & "," & zAmount & ")"
            //                Dbs.Execute SqlStr

            //            Case "PARFUN,RIKI", "SP-F" 'Riki 50%, Parfun 50%
            //                zQty = ChkNumeric(FGrid.TextMatrix(i, 9)) / 2
            //                zAmt = ChkNumeric(FGrid.TextMatrix(i, 11)) / 2
            //                zVat = ChkNumeric(FGrid.TextMatrix(i, 12)) / 2
            //                zAmount = ChkNumeric(FGrid.TextMatrix(i, 13)) / 2
            //                SqlStr = "insert into domesticinvdept (invoiceno,seq,section,qty,amt,vat,amount) values" & _
            //                "('" & FGrid.TextMatrix(i, 0) & "',1,'Riki'," & zQty & "," & zAmt & "," & zVat & "," & zAmount & ")"
            //                Dbs.Execute SqlStr
            //                SqlStr = "insert into domesticinvdept (invoiceno,seq,section,qty,amt,vat,amount) values" & _
            //                "('" & FGrid.TextMatrix(i, 0) & "',2,'Parfun'," & zQty & "," & zAmt & "," & zVat & "," & zAmount & ")"
            //                Dbs.Execute SqlStr
            //        End Select
            //    Next i
            //    CloseDB
            //    Me.MousePointer = 0
            //    MsgBox "Save Data Complete!"
            //End Sub
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
            string strSQL = "select distinct cust_no,custnamee from domesticinvmain inner join customeracc on domesticinvmain.cust_no = customeracc.cust_no order by custnamee";
            DataTable dt = db.GetDataTable(strSQL);
            RepositoryItemSearchLookUpEdit rp = new RepositoryItemSearchLookUpEdit();
            rp.DataSource = dt;
            rp.PopulateViewColumns();
            rp.DisplayMember = "custnamee";
            rp.ValueMember = "custnamee";
            rp.PopupSizeable = true;
            return rp;

        }
        private void GetInvoiceDetail(string strMonth,string strYear)
        {
            string strSQL = "select a.invoiceno,a.invoicedate,a.idno,a.descr,a.cust_no,b.custnamee" +
                ",a.payment,a.credit,a.section,a.qty,a.unit,a.amt,a.vat,a.amount " +
                "from domesticinvmain a inner join customeracc b on a.cust_no = b.cust_no " +
                "where datepart(mm,a.invoicedate) = '" + strMonth + "' " +
                "and datepart(yyyy,invoicedate) = '" + strYear + "' order by a.invoiceno";
            DataTable dt = db.GetDataTable(strSQL);
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

//Private Sub FGrid_KeyDownEdit(ByVal Row As Long, ByVal Col As Long, KeyCode As Integer, ByVal Shift As Integer)
//    If KeyCode = 13 Then
//        If FGrid.Col <> FGrid.Cols - 1 Then
//            FGrid.Col = FGrid.Col + 1
//        Else
//            FGrid.Row = FGrid.Row + 1
//            FGrid.Col = 0
//        End If
//    End If
//End Sub

