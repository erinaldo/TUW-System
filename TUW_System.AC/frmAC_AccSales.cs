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
using myClass;

namespace TUW_System.AC
{
    public partial class frmAC_AccSales : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;

        cDatabase db;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmAC_AccSales()
        {
            InitializeComponent();
        }
        public void ClearData()
        {
            cboMonth.SelectedIndex = DateTime.Today.Month - 1;
            cboYear.SelectedIndex = 0;
            sleCustomer.EditValue = null;
            cboDepartment.Text = "";
            gridControl1.DataSource = null;
        }
        public void DisplayData()
        {
            try
            {
                GetInvoiceDetail((cboMonth.SelectedIndex+1).ToString(),cboYear.Text,sleCustomer.EditValue,cboDepartment.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void PrintPreview()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                cCrystalReport crp=new cCrystalReport(Application.StartupPath+"\\Report\\Account\\accountsales2.rpt");
                if (!crp.SetPrinter()) return;
                crp.ReportTitle = "accountsales2.rpt";
                crp.SetParameter("@scope", "'AS OF "+ cboMonth.Text.ToUpper()+ " "+ cboYear.Text + "'");
                string thisMonth=cboYear.Text+(cboMonth.SelectedIndex+1).ToString().PadLeft(2,'0');
                crp.SetParameter("@monthYear", thisMonth);
                crp.SetParameter("@customer",(sleCustomer.EditValue==null)?null:sleCustomer.Text);
                crp.SetParameter("@department",cboDepartment.Text);
                string fmlText = "";
                crp.PrintReport(fmlText, false, "sa", "ZAQ113m4tuw");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;

        }
        public void Print()
        {
            cCrystalReport crp = new cCrystalReport(Application.StartupPath + "\\Report\\Account\\accountsales2.rpt");
            if (!crp.SetPrinter()) return;
            crp.ReportTitle = "accountsales2.rpt";
            crp.SetParameter("@scope", "'AS OF " + cboMonth.Text.ToUpper() + " " + cboYear.Text + "'");
            string thisMonth = cboYear.Text + (cboMonth.SelectedIndex + 1).ToString().PadLeft(2, '0');
            crp.SetParameter("@monthYear", thisMonth);
            crp.SetParameter("@customer", (sleCustomer.EditValue == null) ? null : sleCustomer.Text);
            crp.SetParameter("@department", cboDepartment.Text);
            string fmlText = "";
            crp.PrintReport(fmlText, true, "sa", "ZAQ113m4tuw");
        }

        private void GetInvoiceDetail(string strMonth,string strYear,object strCustomer,string strDepartment)
        {
            string thisMonth=strYear+strMonth.PadLeft(2,'0');
            string strSQL = "EXEC spAC_AccSales '"+thisMonth+"'";
            strSQL += (sleCustomer.EditValue == null) ? ",null" : ",'" + sleCustomer.Text + "'";
            strSQL += (cboDepartment.Text.Length == 0) ? ",''" : ",'" + cboDepartment.Text + "'";
            DataTable dt = db.GetDataTable(strSQL);
            dt.BeginInit();
            DataColumn dc=new DataColumn();
            dc.DataType=typeof(decimal);
            dc.ColumnName="sales";
            dt.Columns.Add(dc);
            dt.EndInit();
            foreach (DataRow dr in dt.Rows)
            {
                dr["sales"] = Math.Round(Convert.ToDecimal(dr["rate"])*Convert.ToDecimal(dr["amt"]) ,2);
            }
            gridControl1.DataSource = dt;
            gridView1.PopulateColumns();
            gridView1.Columns["inv_date"].Caption = "DATE S.";
            gridView1.Columns["bl_date"].Caption = "BILL DATE";
            gridView1.Columns["custname"].Caption="CUSTOMER";
            gridView1.Columns["invoice_no"].Caption="INVOICE NO.";
            gridView1.Columns["dept_id"].Caption="DEPT.";
            gridView1.Columns["curtype"].Caption="CUR.";
            gridView1.Columns["amt"].Caption="AMOUNT";
            gridView1.Columns["rate"].Caption="RATE S";
            gridView1.Columns["sales"].Caption = "SALES";

            gridView1.Columns["amt"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["amt"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["sales"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["sales"].DisplayFormat.FormatString = "n2";

            gridView1.Columns["amt"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridView1.Columns["sales"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");

            gridView1.OptionsView.ShowFooter = true;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            StatusBarEvent(gridView1.DataRowCount + " Rows.");


            
            //    If Not Rs.EOF Then
            //        i = 0
            //        Do While Not Rs.EOF
            //            i = i + 1
            //            FGrid.Rows = FGrid.Rows + 1
            //            zRateS = CheckRate(Month(Rs!inv_date), TBYear.Text, Rs!curtype)
            //            zSales = Rs!amt * zRateS
            //            FGrid.TextMatrix(i, 1) = ChangeDate(Rs!inv_date)
            //            If Not IsNull(Rs!bl_date) Then
            //                FGrid.TextMatrix(i, 2) = ChangeDate(Rs!bl_date)
            //            End If
            //            FGrid.TextMatrix(i, 3) = Rs!custname
            //            FGrid.TextMatrix(i, 4) = Rs!invoice_no
            //            FGrid.TextMatrix(i, 5) = Rs!dept_id
            //            FGrid.TextMatrix(i, 6) = Rs!curtype
            //            FGrid.TextMatrix(i, 7) = Format(Rs!amt, "###,###,##0.00")
            //            FGrid.TextMatrix(i, 8) = Format(zRateS, "###,##0.0000")
            //            FGrid.TextMatrix(i, 9) = Format(zSales, "###,###,##0.00")
            //            zTotal = zTotal + FGrid.TextMatrix(i, 9)
            //            SqlStr = "select invoice_no from accsalestmp where invoice_no = '" & Rs!invoice_no & "'"
            //            Set Rt = Dbs.Execute(SqlStr)
            //            If Rt.EOF Then
            //                Rt.Close
            //                SqlStr = "insert into accsalestmp (invoice_no,rates) values ('" & Rs!invoice_no & "'," & zRateS & ")"
            //                Dbs.Execute SqlStr
            //            Else
            //                Rt.Close
            //                SqlStr = " Update accsalestmp set rates=" & zRateS & " where invoice_no='" & Rs!invoice_no & "'"
            //                'insert into accsalestmp (invoice_no,rates) values ('" & Rs!invoice_no & "'," & zRateS & ")"
            //                Dbs.Execute SqlStr
            //            End If
            //            Rs.MoveNext
            //        Loop
            //        i = i + 1
            //        FGrid.Cell(flexcpBackColor, i, 1, i, 9) = RGB(245, 219, 163)
            //        FGrid.Cell(flexcpForeColor, i, 1, i, 9) = RGB(0, 0, 255)
            //        FGrid.Cell(flexcpFontBold, i, 1, i, 9) = True
            //        FGrid.TextMatrix(i, 1) = "TOTAL"
            //        FGrid.TextMatrix(i, 9) = Format(zTotal, "###,###,##0.00")
            //    End If
            //    Rs.Close

            //End Sub
        }

        private void frmAC_AccSales_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            for (int i = 0; i < 10; i++)
            {
                cboYear.Properties.Items.Add(DateTime.Today.AddYears(-i).Year);
            }
            string strSQL = "select distinct custname from expbillrecord order by custname";
            DataTable dt = db.GetDataTable(strSQL);
            sleCustomer.Properties.DataSource = dt;
            sleCustomer.Properties.DisplayMember = "custname";
            sleCustomer.Properties.ValueMember = "custname";
            sleCustomer.Properties.View.OptionsView.ColumnAutoWidth = true;
            strSQL = "select distinct dept_id from expbillrecord order by dept_id";
            dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                cboDepartment.Properties.Items.Add(dr["dept_id"].ToString());
            }
            ClearData();
            //[EditValue is null]

            //Private Sub Form_Load()
            //    CBMonth.Text = CBMonth.List(Month(Date))
            //    TBYear.Text = Year(Date)
            //    OpenDatabaseSys
            //    If SaleId = 1 Then
            //    Else
            //        FGrid.ColWidth(3) = 1200
            //        FGrid.ColWidth(4) = 3000
            //        FGrid.ColWidth(5) = 2500
            //        FGrid.ColWidth(6) = 1500
            //        FGrid.ColWidth(7) = 1200
            //        FGrid.ColWidth(8) = 1000
            //        FGrid.ColWidth(9) = 1500
            //        FGrid.ColWidth(10) = 1500
            //        FGrid.ColWidth(11) = 1500
            //        FGrid.TextMatrix(0, 1) = "DATE S."
            //        FGrid.TextMatrix(0, 2) = "BILL DATE"
            //        FGrid.TextMatrix(0, 3) = "CREDIT"
            //        FGrid.TextMatrix(0, 4) = "CUSTOMER"
            //        FGrid.TextMatrix(0, 5) = "DESCRIPTION"
            //        FGrid.TextMatrix(0, 6) = "INVOICE NO."
            //        FGrid.TextMatrix(0, 7) = "DEPT."
            //        FGrid.TextMatrix(0, 8) = "QTY"
            //        FGrid.TextMatrix(0, 9) = "SALES"
            //        FGrid.TextMatrix(0, 10) = "VAT"
            //        FGrid.TextMatrix(0, 11) = "AMOUNT"

            //        CBDept.AddItem "Direct Sales"
            //        CBDept.AddItem "Dyeing,Fabric"
            //        CBDept.AddItem "Inter Sales"
            //        CBDept.AddItem "Knitting"
            //        CBDept.AddItem "NC -F"
            //        CBDept.AddItem "Other"
            //        CBDept.AddItem "PARFUN"
            //        CBDept.AddItem "Parfun,Riki"
            //        CBDept.AddItem "RIKI"
            //        CBDept.AddItem "Sales1"
            //        CBDept.AddItem "Sales2"
            //        CBDept.AddItem "Sales3"
            //        CBDept.AddItem "Sales4"
            //        CBDept.AddItem "Sales5"
            //        CBDept.AddItem "Sales6"

            //        SqlStr = "select distinct custnamee from domesticinvmain inner join customeracc on domesticinvmain.cust_no = customeracc.cust_no" & _
            //        " order by custnamee"
            //        Set Rs = Dbs.Execute(SqlStr)
            //        If Not Rs.EOF Then
            //            Do While Not Rs.EOF
            //                CBCust.AddItem Rs!custnamee
            //                Rs.MoveNext
            //            Loop
            //        End If
            //        Rs.Close
            //        SqlStr = "select distinct descr from domesticinvmain order by descr"
            //        Set Rs = Dbs.Execute(SqlStr)
            //        If Not Rs.EOF Then
            //            Do While Not Rs.EOF
            //                CBDescr.AddItem Rs!descr
            //                Rs.MoveNext
            //            Loop
            //        End If
            //        Rs.Close
            //    End If
            //    CloseDB
            //End Sub
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator) e.Info.DisplayText = (e.RowHandle + 1).ToString();
            gridView1.IndicatorWidth = 45;
        }
    }
}




//Sub PrintDExcel()
//Dim zmonth As String
//Dim zRow As Integer

//    Me.MousePointer = 11
//    zmonth = ChkMonthString(CBMonth.Text)
//    Set ExcelApp = CreateObject("Excel.application")
//    With ExcelApp
//        .Workbooks.Open App.Path & "\DomesticSales.xlt"
//        .ActiveWorkbook.Sheets("Sheet1").Activate
//        .Range("A3").Value = "AS OF " & UCase(CBMonth.Text) & " " & TBYear.Text
//        zRow = 4
//        SqlStr = "select invoiceno,invoicedate,billdate,customeracc.custnamee,domesticinvmain.payment,domesticinvmain.credit " & _
//        ",section,domesticinvmain.descr,qty,amt,vat,amount,invcancel " & _
//        " from domesticinvmain left join customeracc on domesticinvmain.cust_no = customeracc.cust_no " & _
//        " left join domesticdesc on domesticinvmain.descr = domesticdesc.descr where "
//        If Len(CBMonth.Text) = 0 Or CBMonth.Text = "All" Then
//            SqlStr = SqlStr & " datepart(yyyy,invoicedate) = '" & TBYear.Text & "'"
//        Else
//            SqlStr = SqlStr & " datepart(mm,invoicedate) = '" & zmonth & "' and datepart(yyyy,invoicedate) = '" & TBYear.Text & "'"
//        End If
//        If Len(CBCust.Text) > 0 Then
//            SqlStr = SqlStr & " and customeracc.custnamee = N'" & CBCust.Text & "'"
//        End If
//        If Len(CBDescr.Text) > 0 Then
//            SqlStr = SqlStr & " and domesticinvmain.descr = N'" & CBDescr.Text & "'"
//        End If

//        If Len(CBDept.Text) > 0 Then
//            If CBDept.Text = "Dyeing,Fabric" Then
//                SqlStr = SqlStr & " and (section = 'Dyeing' or section = 'Fabric')"
//            Else
//                SqlStr = SqlStr & " and section = '" & CBDept.Text & "'"
//            End If
//        End If
//        '====================================theep 21/06/47===================================================
//        SqlStr = SqlStr & "  order by invoicedate,invoiceno "
//        '=====================================================================================================
//        Set Rs = Dbs.Execute(SqlStr)
//        If Not Rs.EOF Then
//            Do While Not Rs.EOF
//                zRow = zRow + 1
//                .Range("A" & zRow).Value = ChangeDate(Rs!invoicedate)
//                If Not IsNull(Rs!billdate) Then
//                .Range("B" & zRow).Value = ChangeDate(Rs!billdate)
//                End If
//                Select Case Rs!payment
//                    Case "Credit"
//                        .Range("C" & zRow).Value = "Credit " & Rs!credit & " Days"
//                    Case "T/T."
//                        .Range("C" & zRow).Value = "T/T. " & Rs!credit & " Days"
//                    Case Else
//                        .Range("C" & zRow).Value = "CASH"
//                End Select
                
//                If Rs!InvCancel = 1 Then
//                    .Range("A" & zRow & ":" & "K" & zRow).Font.Color = &HFF
//                    .Range("C" & zRow).Value = "- - "
//                    .Range("D" & zRow).Value = "INVOICE CANCELLED"
//                    .Range("E" & zRow).Value = "- -"
//                    .Range("F" & zRow).Value = Rs!invoiceno
//                    .Range("G" & zRow).Value = ""
//                    .Range("H" & zRow).Value = "0.00"
//                    .Range("I" & zRow).Value = "0.00"
//                    .Range("J" & zRow).Value = "0.00"
//                    .Range("K" & zRow).Value = "0.00"
//                Else
//                    .Range("D" & zRow).Value = Rs!custnamee
//                    .Range("E" & zRow).Value = Rs!descr
//                    .Range("F" & zRow).Value = Rs!invoiceno
//                    .Range("G" & zRow).Value = ChkDeptDomestic(Rs!Section)
//                    .Range("H" & zRow).Value = Rs!qty
//                    .Range("I" & zRow).Value = Rs!amt
//                    .Range("J" & zRow).Value = Rs!vat
//                    .Range("K" & zRow).Value = Rs!amount
//                End If
//                Rs.MoveNext
//            Loop
//        End If
//        SetTable ExcelApp, "A5", "K" & zRow, True, True, True, True, True, True
//    End With
//    ExcelApp.Visible = True
//    Set ExcelApp = Nothing
//    Me.MousePointer = 0
//End Sub

//Private Sub CmdPrint_Click()
//    OpenDatabaseSys
//    If SaleId = 1 Then
//        PrintReport
//    Else
//        PrintDExcel
//    End If
//    CloseDB
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

//Function CheckRate(zmonth As Integer, zYear As String, zCur As String) As Double
//    Dim zDate As String
//    Dim Rn As ADODB.Recordset
//    zDate = zYear & Format(zmonth, "00")
//    SqlStr = "Select * From MoneyRate Where (SUBSTRING(PERIOD, 1, 6) <= '" & zDate & "') AND (SUBSTRING(PERIOD, 8, 13) >= '" & zDate & "')"

//    Set Rn = Dbs.Execute(SqlStr)
//    If Not Rn.EOF Then
//        Select Case UCase(zCur)
//            Case "YEN"
//                CheckRate = Rn!yenrates
//            Case "US$"
//                CheckRate = Rn!usrates
//            Case "S$"
//                CheckRate = Rn!sgrates
//            Case "EUR"
//                CheckRate = Rn!eurrates
//        End Select
//    End If
//End Function

//Sub DomesticSales()
//Dim i As Integer
//Dim zCTotal, zTotal As Double
//Dim zTtlQty, zTtlAmt, zTtlVat As Double
//Dim zmonth As String
//Dim zCust As String
//Dim zCTtlQty, zCTtlAmt, zCTtlVat As Double

//    zmonth = ChkMonthString(CBMonth.Text)
//    SqlStr = "select invoiceno,invoicedate,billdate,domesticinvmain.payment,domesticinvmain.credit" & _
//    ",custnamee,descr,section,qty,amt,vat,amount from domesticinvmain left join customeracc on domesticinvmain.cust_no = customeracc.cust_no" & _
//    " where "
//    If Len(CBMonth.Text) = 0 Or CBMonth.Text = "All" Then
//        SqlStr = SqlStr & " datepart(yyyy,invoicedate) = '" & TBYear.Text & "'"
//    Else
//        SqlStr = SqlStr & " datepart(mm,invoicedate) = '" & zmonth & "' and datepart(yyyy,invoicedate) = '" & TBYear.Text & "'"
//    End If
//    If Len(CBCust.Text) > 0 Then
//        SqlStr = SqlStr & " and custnamee = N'" & CBCust.Text & "'"
//    End If
//    If Len(CBDescr.Text) > 0 Then
//        SqlStr = SqlStr & " and descr = N'" & CBDescr.Text & "'"
//    End If
//    If Len(CBDept.Text) > 0 Then
//        If CBDept.Text = "Dyeing,Fabric" Then
//            SqlStr = SqlStr & " and (section = 'Dyeing' or section = 'Fabric')"
//        Else
//            SqlStr = SqlStr & " and section = '" & CBDept.Text & "'"
//        End If
//    End If
//    SqlStr = SqlStr & " order by custnamee,invoiceno"
//    Set Rs = Dbs.Execute(SqlStr)
//    If Not Rs.EOF Then
//        i = 0
//        zTotal = 0
//        zTtlAmt = 0
//        zTtlVat = 0
//        zCust = ""
//        Do While Not Rs.EOF
//            i = i + 1
//            FGrid.Rows = i + 1
//            FGrid.TextMatrix(i, 1) = ChangeDate(Rs!invoicedate)
//            If Not IsNull(Rs!billdate) Then
//                FGrid.TextMatrix(i, 2) = ChangeDate(Rs!billdate)
//            End If
//            Select Case Rs!payment
//                Case "Credit"
//                    FGrid.TextMatrix(i, 3) = "Credit " & Rs!credit & " Days"
//                Case "T/T."
//                    FGrid.TextMatrix(i, 3) = "T/T. " & Rs!credit & " Days"
//                Case Else
//                    FGrid.TextMatrix(i, 3) = "CASH"
//            End Select
            
//            FGrid.TextMatrix(i, 4) = IIf(IsNull(Rs!custnamee), "", Rs!custnamee)
//            FGrid.TextMatrix(i, 5) = Rs!descr
//            FGrid.TextMatrix(i, 6) = Rs!invoiceno
//            FGrid.TextMatrix(i, 7) = Rs!Section
//            FGrid.TextMatrix(i, 8) = Format(Rs!qty, "#,##0")
//            FGrid.TextMatrix(i, 9) = Format(Rs!amt, "###,###,##0.00")
//            FGrid.TextMatrix(i, 10) = Format(Rs!vat, "###,###,##0.00")
//            FGrid.TextMatrix(i, 11) = Format(Rs!amount, "###,###,##0.00")
//            zTtlQty = zTtlQty + Rs!qty
//            zTtlAmt = zTtlAmt + Rs!amt
//            zTtlVat = zTtlVat + Rs!vat
//            zTotal = zTotal + Rs!amount
//            zCTtlQty = zCTtlQty + Rs!qty
//            zCTtlAmt = zCTtlAmt + Rs!amt
//            zCTtlVat = zCTtlVat + Rs!vat
//            zCTotal = zCTotal + Rs!amount
//            zCust = Rs!custnamee
//            Rs.MoveNext
//            If Not Rs.EOF Then
//                If zCust <> Rs!custnamee Then
//                    i = i + 1
//                    FGrid.Rows = i + 1
//                    FGrid.Cell(flexcpBackColor, i, 1, i, 11) = RGB(234, 250, 251)
//                    FGrid.Cell(flexcpFontBold, i, 1, i, 11) = True
//                    FGrid.TextMatrix(i, 3) = "TOTAL"
//                    FGrid.TextMatrix(i, 4) = zCust
//                    FGrid.TextMatrix(i, 8) = Format(zCTtlQty, "###,###,##0.00")
//                    FGrid.TextMatrix(i, 9) = Format(zCTtlAmt, "###,###,##0.00")
//                    FGrid.TextMatrix(i, 10) = Format(zCTtlVat, "###,###,##0.00")
//                    FGrid.TextMatrix(i, 11) = Format(zCTotal, "###,###,##0.00")
//                    zCTtlQty = 0
//                    zCTtlAmt = 0
//                    zCTtlVat = 0
//                    zCTotal = 0
//                End If
//            End If
//        Loop
//        i = i + 1
//        FGrid.Rows = i + 1
//        FGrid.Cell(flexcpBackColor, i, 1, i, 11) = RGB(234, 250, 251)
//        FGrid.Cell(flexcpFontBold, i, 1, i, 11) = True
//        FGrid.TextMatrix(i, 3) = "TOTAL"
//        FGrid.TextMatrix(i, 4) = zCust
//        FGrid.TextMatrix(i, 8) = Format(zCTtlQty, "###,###,##0.00")
//        FGrid.TextMatrix(i, 9) = Format(zCTtlAmt, "###,###,##0.00")
//        FGrid.TextMatrix(i, 10) = Format(zCTtlVat, "###,###,##0.00")
//        FGrid.TextMatrix(i, 11) = Format(zCTotal, "###,###,##0.00")
//        i = i + 1
//        FGrid.Rows = i + 1
//        FGrid.Cell(flexcpBackColor, i, 1, i, 11) = RGB(245, 219, 163)
//        FGrid.Cell(flexcpForeColor, i, 1, i, 11) = RGB(0, 0, 255)
//        FGrid.Cell(flexcpFontBold, i, 1, i, 11) = True
//        FGrid.TextMatrix(i, 1) = "TOTAL"
//        FGrid.TextMatrix(i, 8) = Format(zTtlQty, "###,###,##0.00")
//        FGrid.TextMatrix(i, 9) = Format(zTtlAmt, "###,###,##0.00")
//        FGrid.TextMatrix(i, 10) = Format(zTtlVat, "###,###,##0.00")
//        FGrid.TextMatrix(i, 11) = Format(zTotal, "###,###,##0.00")
//    End If
//    Rs.Close
    
//End Sub



//Function ChangeDate(zDate As String) As String
//Dim zM As String
//    zM = Mid(zDate, 4, 2)
//    Select Case zM
//        Case "01"
//            ChangeDate = Left(zDate, 2) & "-Jan-" & Right(zDate, 4)
//        Case "02"
//            ChangeDate = Left(zDate, 2) & "-Feb-" & Right(zDate, 4)
//        Case "03"
//            ChangeDate = Left(zDate, 2) & "-Mar-" & Right(zDate, 4)
//        Case "04"
//            ChangeDate = Left(zDate, 2) & "-Apr-" & Right(zDate, 4)
//        Case "05"
//            ChangeDate = Left(zDate, 2) & "-May-" & Right(zDate, 4)
//        Case "06"
//            ChangeDate = Left(zDate, 2) & "-Jun-" & Right(zDate, 4)
//        Case "07"
//            ChangeDate = Left(zDate, 2) & "-Jul-" & Right(zDate, 4)
//        Case "08"
//            ChangeDate = Left(zDate, 2) & "-Aug-" & Right(zDate, 4)
//        Case "09"
//            ChangeDate = Left(zDate, 2) & "-Sep-" & Right(zDate, 4)
//        Case "10"
//            ChangeDate = Left(zDate, 2) & "-Oct-" & Right(zDate, 4)
//        Case "11"
//            ChangeDate = Left(zDate, 2) & "-Nov-" & Right(zDate, 4)
//        Case "12"
//            ChangeDate = Left(zDate, 2) & "-Dec-" & Right(zDate, 4)
        
//    End Select
//End Function


