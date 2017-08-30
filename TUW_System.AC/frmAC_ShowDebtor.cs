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
    public partial class frmAC_ShowDebtor : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;

        cDatabase db;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmAC_ShowDebtor()
        {
            InitializeComponent();
        }
        public void ClearData() 
        {
            cboMonth.SelectedIndex = DateTime.Today.Month - 1;
            cboYear.SelectedIndex = 0;
            sleCustomer.EditValue = null;
            cboDepartment.Text = "";
            cboCredit.Text = "";
            gridControl1.DataSource = null;
        }
        public void DisplayData()
        { 
            try 
	        {	        
		        GetInvoiceDetail();
	        }
	        catch (Exception ex)
	        {
		        MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
	        }
            
        }
        public void PrintPreview()
        { 
        
        }
        public void Print()
        { 
        
        }

        private void GetInvoiceDetail()
        {
            //Sub ExportBalance()
            //Dim zmonth As String
            //Dim i As Integer
            //Dim zDate As String
            //Dim zCurtype As String
            //Dim zAmtAll  As Double
            //Dim zAmt As Double
            //Dim zAmount As Double
            //Dim zBegin As Double
            //Dim zSales As Double
            //Dim zReceive As Double
            //Dim zEnding As Double
            //Dim zRate_s As Double
            //Dim zRate_r As Double
            //Dim zRateR As Double
            //Dim zRateS As Double
            //Dim zLastYear As String
            //Dim zLastMonth As String
            //Dim zTBegin As Double
            //Dim zTSales As Double
            //Dim zTReceive As Double
            //Dim zTEnding As Double
            //Dim zTOverdue As Double
            //Dim zBeginT As Double
            //Dim zSalesT As Double
            //Dim zReceiveT As Double
            //Dim zEndingT As Double
            //Dim zLastDate As String
            //Dim zOverdue As Double
            //Dim zNextdate As Date
            //Dim zCust As String
            //Dim zBLDate As Date
            //Dim ChkBL As Boolean
            //Dim zRowDept As Integer
            //Dim zRowCust As Integer
            //Dim zRowCr As Integer
            //Dim zBL As String
            //Dim MMYY As String
            //Dim zCr As String
            //Dim zNegDate As String
            //Dim DebNo As String
            //Dim BLDate As String

            //    zmonth = ChkMonthString(CBMonth.Text)
            //    zBLDate = DateSerial(Int(TBYear.Text), Int(zmonth), 1)
            //    Select Case zmonth
            //        Case "09"
            //            zLastDate = "10/01/" & TBYear
            //        Case "10"
            //            zLastDate = "11/01/" & TBYear
            //        Case "11"
            //            zLastDate = "12/01/" & TBYear
            //        Case "12"
            //            zLastDate = "01/01/" & Int(TBYear) + 1
            //        Case Else
            //            zLastDate = "0" & Trim(Str(Int(zmonth) + 1)) & "/01/" & TBYear.Text
            //    End Select
            //    MMYY = zmonth & "/" & TBYear.Text
            //    zDate = zmonth & "/01/" & TBYear.Text
            //    If CBMonth.ListIndex + 1 = 1 Then
            //        zLastMonth = "12/" & CLng(TBYear) - 1
            //    Else
            //        If CBMonth.ListIndex < 10 Then
            //            zLastMonth = "0" & CBMonth.ListIndex & "/" & TBYear
            //        Else
            //            zLastMonth = CBMonth.ListIndex & "/" & TBYear
            //        End If
            //    End If
            //    i = 0
            //    Me.MousePointer = 11
            //    If ChkSelect.Value = 1 Then
            //        SqlStr = "SELECT invoice_no,inv_date,custname,dept_id,collect_date,inv_grp FROM expbillrecord" & _
            //        " WHERE DATEPART(MM,inv_DATE) = '" & zmonth & "' and DATEPART(YYYY,inv_DATE) = '" & TBYear.Text & "'" & _
            //        " AND COLLECT_DATE IS NULL and nodebtor = 0 "
            //        If Len(CBCust.Text) > 0 Then
            //            SqlStr = SqlStr & " and custname = '" & Replace(CBCust.Text, "'", "''") & "'"
            //        End If
            //        If Len(CBDept.Text) > 0 Then
            //            SqlStr = SqlStr & " and dept_id = '" & CBDept.Text & "'"
            //        End If
            //        SqlStr = SqlStr & " Union select invoice_no,inv_date,custname,dept_id,min(collect_date) as collect_date,inv_grp from expbillreceive" & _
            //        " where DATEPART(MM,inv_DATE) = '" & zmonth & "' and DATEPART(YYYY,inv_DATE) = '" & TBYear.Text & "'" & _
            //        " and COLLECT_DATE >= '" & zLastDate & "'"
            //        If Len(CBCust.Text) > 0 Then
            //            SqlStr = SqlStr & " and custname = '" & Replace(CBCust.Text, "'", "''") & "'"
            //        End If
            //        If Len(CBDept.Text) > 0 Then
            //            SqlStr = SqlStr & " and dept_id = '" & CBDept.Text & "'"
            //        End If
            //        SqlStr = SqlStr & " group by invoice_no,inv_date,custname,dept_id,inv_grp ORDER BY custname,inv_date"
        
            //    Else
            //        SqlStr = "SELECT invoice_no,inv_date,custname,dept_id,collect_date,inv_grp FROM expbillrecord" & _
            //        " WHERE ((INV_DATE < '" & zLastDate & "' AND COLLECT_DATE IS NULL) " & _
            //        "or (INV_DATE < '" & zDate & "' and amt > usamt+yenamt+sgamt)) and nodebtor = 0 "
            //        If Len(CBCust.Text) > 0 Then
            //            SqlStr = SqlStr & " and custname = '" & Replace(CBCust.Text, "'", "''") & "'"
            //        End If
            //        If Len(CBDept.Text) > 0 Then
            //            SqlStr = SqlStr & " and dept_id = '" & CBDept.Text & "'"
            //        End If
            //        SqlStr = SqlStr & " Union select invoice_no,inv_date,custname,dept_id,min(collect_date) as collect_date,inv_grp from expbillreceive" & _
            //        " where ((DATEPART(MM,inv_DATE) = '" & zmonth & "' and DATEPART(YYYY,inv_DATE) = '" & TBYear.Text & "'" & _
            //        " and COLLECT_DATE >= '" & zLastDate & "') OR (INV_DATE < '" & zDate & "' AND COLLECT_DATE >= '" & zLastDate & "'))"
            //        If Len(CBCust.Text) > 0 Then
            //            SqlStr = SqlStr & " and custname = '" & Replace(CBCust.Text, "'", "''") & "'"
            //        End If
            //        If Len(CBDept.Text) > 0 Then
            //            SqlStr = SqlStr & " and dept_id = '" & CBDept.Text & "'"
            //        End If
            //        SqlStr = SqlStr & " group by invoice_no,inv_date,custname,dept_id,inv_grp ORDER BY custname,inv_date"
            //    End If
        
        
            //        Set Rs = Dbs.Execute(SqlStr)
            //        If Not Rs.EOF Then
            //            zTEnding = 0
            //            zTOverdue = 0
            //            SqlStr = "delete from acctmpreport where mmyy = '" & MMYY & "1'"
            //            Dbs.Execute SqlStr

            //            Do While Not Rs.EOF
            //                If Rs!invoice_no = "DEB NO.05/088-1P" Then
            //                    MsgBox Rs!invoice_no
            //                End If
            //                zAmtAll = 0
            //                ChkBL = False
            //                SqlStr = "select invoice_no,exinv_no,cr,amt,curtype,bl_date,neg_date,debno from expbillrecord where invoice_no = '" & Rs!invoice_no & "'"
            //                Set Rt = Dbs.Execute(SqlStr)
            //                If Not Rt.EOF Then
            //                    zAmtAll = Rt!amt
            //                    zCurtype = Rt!curtype
            //                    zCr = Rt!cr
            //                    DebNo = Rt!DebNo
            //                    If Not IsNull(Rt!neg_date) Then
            //                        zNegDate = ChangeDate(Format(Rt!neg_date, "dd/mm/yyyy"))
            //                    Else
            //                        zNegDate = ""
            //                    End If
            //                    zBL = IIf(IsNull(Rt!bl_date), "", Format(Rt!bl_date, "mm/dd/yyyy"))
            //                    BLDate = IIf(IsNull(Rt!bl_date), "", Format(Rt!bl_date, "dd/mm/yyyy"))
            //                    If Not IsNull(Rt!bl_date) Then
            //                        If Rt!bl_date < zBLDate Then
            //                            ChkBL = True
            //                        End If
            //                    Else
            //                        If Rs!inv_date < zBLDate Then
            //                            ChkBL = True
            //                        End If
            //                    End If
            //                End If
                
            //                zAmt = 0
            //                zAmount = 0
            //                zBegin = 0
            //                zSales = 0
            //                zReceive = 0
            //                zEnding = 0
            //                zRate_s = 0
            //                zRate_r = 0
            //                zOverdue = 0
                                            
            //               If CBLC.Text = "" Or CBLC.Text = zCr Then
                                   
            //                    ' Find Amount for Receive and Total Amount
            //                    SqlStr = "select sum(usamt+yenamt+sgamt+euramt) as amt from expbillreceive where invoice_no = '" & Rs!invoice_no & "'" & _
            //                    " and collect_date <= '" & Format(Rs!collect_date, "mm/dd/yyyy") & "'"
            //                    Set Rt = Dbs.Execute(SqlStr)
            //                    If Not Rt.EOF Then
            //                        If Not IsNull(Rt!amt) Then
            //                            If zAmtAll > Rt!amt Then
            //                                zAmount = zAmtAll - Rt!amt
            //                            Else
            //                                zAmount = zAmtAll
            //                            End If
            //                        Else
            //                            zAmount = zAmtAll
            //                        End If
            //                    End If
            //                    Rt.Close
            //                    If Year(Rs!inv_date) = Int(TBYear.Text) Then     ' Same Year
            //                        zRate_s = ChkRateSale(Month(Rs!inv_date), Year(Rs!inv_date), zCurtype)
            //                        If Month(Rs!inv_date) = Int(zmonth) Then        ' Same Month
            //                            zSales = zAmtAll * zRate_s
            //                            If zmonth = "12" Then
            //                                zRate_r = ChkRateRecv(Int(zmonth), TBYear.Text, zCurtype)
            //                                zEnding = zAmtAll * zRate_r
            //                            Else
            //                                zRate_r = 0
            //                                zEnding = zAmtAll * zRate_s     '**** Not Sure
            //                            End If
                            
            //                        Else        'Not Same Month     ================     Edit < ...<=
            //                            ' Find Beginning
            //                            SqlStr = "select sum(usamt+yenamt+sgamt+euramt) as amt from expbillreceive where invoice_no = '" & Rs!invoice_no & "'" & _
            //                            " and collect_date < '" & Format(Rs!collect_date, "mm/dd/yyyy") & "'"
            //                            Set Rt = Dbs.Execute(SqlStr)
            //                            If Not Rt.EOF Then
            //                                If Not IsNull(Rt!amt) Then
            //                                    zBegin = (zAmtAll - Rt!amt) * zRate_s
            //                                    If zmonth = "12" Then
            //                                        zRate_r = ChkRateRecv(Int(zmonth), TBYear.Text, zCurtype)
            //                                        zEnding = (zAmtAll - Rt!amt) * zRate_r
            //                                    Else
            //                                        zRate_r = 0
            //                                        zEnding = zAmount * zRate_s     '**** Not Sure
            //                                    End If
                                    
            //                                Else
            //                                    zBegin = zAmtAll * zRate_s
            //                                    If zmonth = "12" Then
            //                                        zRate_r = ChkRateRecv(Int(zmonth), TBYear.Text, zCurtype)
            //                                        zEnding = zAmount * zRate_r
            //                                    Else
            //                                        zRate_r = 0
            //                                        zEnding = zAmount * zRate_s
            //                                    End If
                                    
            //                                End If
            //                            End If
            //                            Rt.Close
            //                        End If
            //                    Else    ' Not Same Year
                   
            //                            zRateS = ChkRateRecvy(Year(Rs!inv_date), zCurtype)
            //                            zRate_s = ChkRateRecvy(Year(Rs!inv_date), zCurtype)

            //                        ' Find Beginning
            //                            SqlStr = "select sum(usamt+yenamt+sgamt+euramt) as amt from expbillreceive where invoice_no = '" & Rs!invoice_no & "'" & _
            //                            " and collect_date < '" & Format(Rs!collect_date, "mm/dd/yyyy") & "'"
            //                            Set Rt = Dbs.Execute(SqlStr)
            //                            If Not Rt.EOF Then
            //                                If Not IsNull(Rt!amt) Then
            //                                    zBegin = (zAmtAll - Rt!amt) * zRateS
            //                                    If zmonth = "12" Then
            //                                        zRate_r = ChkRateRecv(Int(zmonth), TBYear.Text, zCurtype)
            //                                        zEnding = (zAmtAll - Rt!amt) * zRate_r
            //                                    Else
            //                                        zRate_r = 0
            //                                        zEnding = (zAmtAll - Rt!amt) * zRateS     '**** Not Sure
            //                                    End If
                                    
            //                                Else
            //                                    zBegin = zAmtAll * zRateS
            //                                    If zmonth = "12" Then
            //                                        zRate_r = ChkRateRecv(Int(zmonth), TBYear.Text, zCurtype)
            //                                        zEnding = zAmount * zRate_r
            //                                    Else
            //                                        zRate_r = 0
            //                                        zEnding = zAmount * zRateS
            //                                    End If
                                    
            //                                End If
            //                            End If
            //                            Rt.Close
            //                    End If  'Year

            //                    If zSales > 0 And zmonth <> "12" Then
            //                        zEnding = CDbl(Format(zEnding, "###,###,###,##0.00"))
            //                    Else
            //                        If Year(Rs!inv_date) = CLng(TBYear) Then
            //                            zEnding = CDbl(Format(zEnding, "###,###,###,##0.00"))
            //                        End If
            //                    End If
                    
            //                    If zEnding > 0 And ChkBL = True Then
            //                        zOverdue = zEnding
            //                    Else
            //                        zOverdue = 0
            //                    End If
            //                    zTEnding = zTEnding + zEnding
            //                    zTOverdue = zTOverdue + zOverdue
                    
            //                    i = i + 1
            //                    FGrid.Rows = i + 1
            //                    FGrid.TextMatrix(i, 0) = i
            //                    FGrid.TextMatrix(i, 1) = Format(Rs!inv_date, "yyyy/mm/dd")
            //                    FGrid.TextMatrix(i, 2) = ChangeDate(Format(BLDate, "dd/mm/yyyy"))
            //                    FGrid.TextMatrix(i, 3) = zNegDate
            //                    FGrid.TextMatrix(i, 4) = zCr
            //                    FGrid.TextMatrix(i, 5) = Rs!custname
            //                    FGrid.TextMatrix(i, 6) = Rs!invoice_no
            //                    FGrid.TextMatrix(i, 7) = DebNo
            //                    FGrid.TextMatrix(i, 8) = Rs!dept_id
            //                    FGrid.TextMatrix(i, 9) = zCurtype
            //                    FGrid.TextMatrix(i, 10) = Format(zAmount, "###,###,###,##0.00")
            //                     If zRate_s > 0 Then
            //                        FGrid.TextMatrix(i, 11) = zRate_s
            //                    End If
            //                    If zEnding > 0 Then
            //                        FGrid.TextMatrix(i, 12) = Format(zEnding, "###,###,###,##0.00")
            //                    End If
            //                    If zOverdue > 0 Then
            //                        FGrid.TextMatrix(i, 13) = Format(zOverdue, "###,###,###,##0.00")
            //                    End If
            //                    zCust = Rs!custname
            //                    SqlStr = "insert into acctmpreport (mmyy,invoice_no,inv_date,invdate,bl_date,bldate,custname,dept_id,curtype,rates,amt" & _
            //                    ",balance,overdue,neg_date,cr,debno) values ('" & MMYY & "1','" & Rs!invoice_no & "','" & Format(Rs!inv_date, "mm/dd/yyyy") & "'" & _
            //                    ",'" & ChangeDate(Format(Rs!inv_date, "dd/mm/yyyy")) & "','" & zBL & "'" & _
            //                    ",'" & ChangeDate(Format(BLDate, "dd/mm/yyyy")) & "','" & Replace(zCust, "'", "''") & "','" & Rs!dept_id & "','" & zCurtype & "'" & _
            //                    "," & zRate_s & "," & zAmount & "," & zEnding & "," & zOverdue & ",'" & zNegDate & "','" & zCr & "','" & DebNo & "')"
            //                    Dbs.Execute SqlStr
                
            //                End If
                
            //                Rs.MoveNext
            //            Loop
            //            i = i + 1
            //            FGrid.Rows = i + 1
            //            FGrid.Cell(flexcpBackColor, i, 1, i, 13) = RGB(245, 219, 163)
            //            FGrid.Cell(flexcpForeColor, i, 1, i, 13) = RGB(0, 0, 255)
            //            FGrid.Cell(flexcpFontBold, i, 1, i, 13) = True
            //            FGrid.MergeRow(i) = True
            //            FGrid.MergeCells = flexMergeFree
            //            If Len(CBCust.Text) > 0 Then
            //            FGrid.Cell(flexcpText, i, 5, i, 7) = "Total         " & zCust
            //            Else
            //            FGrid.Cell(flexcpText, i, 5, i, 7) = "Total"
            //            End If
            //            FGrid.TextMatrix(i, 12) = Format(zTEnding, "###,###,###,##0.00")
            //            If zTOverdue > 0 Then
            //                FGrid.TextMatrix(i, 13) = Format(zTOverdue, "###,###,###,##0.00")
            //            End If
            //        End If
        
            //        Me.MousePointer = 0
        
            //End Sub
        }

        private void frmAC_ShowDebtor_Load(object sender, EventArgs e)
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
            ClearData();
        }



    }
}

/*
 

Private Sub CmdPrint_Click()
Dim Fmlatext As String
Dim zmonth As String
Dim MMYY As String
Dim zDay As String

Dim crxApplication As New CRAXDRT.Application
Dim crxReport As CRAXDRT.Report
Dim crxReport1 As CRAXDRT.Report
Dim crxReport2 As CRAXDRT.Report
Dim strReportTitle As String

    On Error GoTo ErrLabel
    zmonth = ChkMonthString(CBMonth.Text)
    MMYY = zmonth & "/" & TBYear.Text
    Select Case Int(zmonth)
        Case 1, 3, 5, 7, 8, 10, 12
            zDay = "31"
        Case 4, 6, 9, 11
            zDay = "30"
        Case 2
            zDay = "28"
    End Select
'    AccRep.Destination = crptToWindow
'    Accrep1.Destination = crptToWindow
    If RepId = 1 Then
        Fmlatext = "{ACCTMPREPORT.MMYY} = '" & MMYY & "1'"
        If Len(CBDept.Text) > 0 Then
'            AccRep.ReportFileName = App.Path & "/accunreceivec.rpt"
'            AccRep.WindowTitle = "AccUnReceivec.rpt"
'            AccRep.Formulas(0) = "Scope='AS OF " & UCase(CBMonth.Text) & " " & zDay & ", " & TBYear.Text & "'"
'            AccRep.SelectionFormula = Fmlatext
'            AccRep.Action = 1
            Set crxReport = crxApplication.OpenReport(App.Path & "/accunreceivec.rpt", 1)
            strReportTitle = "AccUnReceivec.rpt"
            crxReport.FormulaFields.GetItemByName("Scope").Text = "'AS OF " & UCase(CBMonth.Text) & " " & zDay & ", " & TBYear.Text & "'"
            crxReport.Database.Tables.Item(1).ConnectionProperties.Item("password") = "ZAQ113m4tuw"
            crxReport.RecordSelectionFormula = Fmlatext
            Call PrintCrystalReport(crxReport, strReportTitle)
        Else
'            AccRep.ReportFileName = App.Path & "/accunreceivec.rpt"
'            AccRep.WindowTitle = "AccUnReceived.rpt"
'            AccRep.Formulas(0) = "Scope='AS OF " & UCase(CBMonth.Text) & " " & zDay & ", " & TBYear.Text & "'"
'            AccRep.SelectionFormula = Fmlatext
'            AccRep.Action = 1
            Set crxReport = crxApplication.OpenReport(App.Path & "/accunreceivec.rpt", 1)
            strReportTitle = "AccUnReceived.rpt"
            crxReport.FormulaFields.GetItemByName("Scope").Text = "'AS OF " & UCase(CBMonth.Text) & " " & zDay & ", " & TBYear.Text & "'"
            crxReport.Database.Tables.Item(1).ConnectionProperties.Item("password") = "ZAQ113m4tuw"
            crxReport.RecordSelectionFormula = Fmlatext
            Call PrintCrystalReport(crxReport, strReportTitle)
        End If
    Else
    
        
        If Len(CBDept.Text) = 0 Then
'                AccRep.ReportFileName = App.Path & "/Domesticunrecv.rpt"
'                AccRep.WindowTitle = "DomesticUnRecv.rpt"
'                AccRep.Formulas(0) = "Scope='AS OF " & UCase(CBMonth.Text) & " " & zDay & ", " & TBYear.Text & "'"
'                AccRep.SelectionFormula = Fmlatext
'                AccRep.Action = 1
                Set crxReport = crxApplication.OpenReport(App.Path & "/Domesticunrecv.rpt", 1)
                strReportTitle = "DomesticUnRecv.rpt"
                crxReport.FormulaFields.GetItemByName("Scope").Text = "'AS OF " & UCase(CBMonth.Text) & " " & zDay & ", " & TBYear.Text & "'"
                crxReport.Database.Tables.Item(1).ConnectionProperties.Item("password") = "ZAQ113m4tuw"
                crxReport.RecordSelectionFormula = Fmlatext
                Call PrintCrystalReport(crxReport, strReportTitle)
            
'                Accrep1.ReportFileName = App.Path & "/Domesticunrecv2.rpt"
'                Accrep1.WindowTitle = "DomesticUnRecv2.rpt"
'                Accrep1.Formulas(0) = "Scope='AS OF " & UCase(CBMonth.Text) & " " & zDay & ", " & TBYear.Text & "'"
'                Accrep1.SelectionFormula = Fmlatext
'                Accrep1.Action = 1
                Set crxReport1 = crxApplication.OpenReport(App.Path & "/Domesticunrecv2.rpt", 1)
                strReportTitle = "DomesticUnRecv2.rpt"
                crxReport1.FormulaFields.GetItemByName("Scope").Text = "'AS OF " & UCase(CBMonth.Text) & " " & zDay & ", " & TBYear.Text & "'"
                crxReport1.Database.Tables.Item(1).ConnectionProperties.Item("password") = "ZAQ113m4tuw"
                crxReport1.RecordSelectionFormula = Fmlatext
                Call PrintCrystalReport(crxReport1, strReportTitle)
        Else
'            AccRep.ReportFileName = App.Path & "/domesticunrecv1.rpt"
'            AccRep.WindowTitle = "DomesticUnrecv1.rpt"
'            AccRep.Formulas(0) = "Scope='AS OF " & UCase(CBMonth.Text) & " " & zDay & ", " & TBYear.Text & "'"
'            AccRep.SelectionFormula = Fmlatext
'            AccRep.Action = 1
            Set crxReport = crxApplication.OpenReport(App.Path & "/domesticunrecv1.rpt", 1)
            strReportTitle = "DomesticUnrecv1.rpt"
            crxReport.FormulaFields.GetItemByName("Scope").Text = "'AS OF " & UCase(CBMonth.Text) & " " & zDay & ", " & TBYear.Text & "'"
            crxReport.Database.Tables.Item(1).ConnectionProperties.Item("password") = "ZAQ113m4tuw"
            crxReport.RecordSelectionFormula = Fmlatext
            Call PrintCrystalReport(crxReport, strReportTitle)
        End If
    
    End If
    
ErrLabel:
    If Err > 0 Then
        MsgBox Err.Description
    End If
End Sub





Function ChkMonthString(zMon As String) As String
    Select Case zMon
        Case "January"
            ChkMonthString = "01"
        Case "February"
            ChkMonthString = "02"
        Case "March"
            ChkMonthString = "03"
        Case "April"
            ChkMonthString = "04"
        Case "May"
            ChkMonthString = "05"
        Case "June"
            ChkMonthString = "06"
        Case "July"
            ChkMonthString = "07"
        Case "August"
            ChkMonthString = "08"
        Case "September"
            ChkMonthString = "09"
        Case "October"
            ChkMonthString = "10"
        Case "November"
            ChkMonthString = "11"
        Case "December"
            ChkMonthString = "12"
    End Select
End Function


Function ChkRateSale(zmonth As Integer, zYear As String, zCur As String) As Double
    Dim zTime As Integer
    Dim Rn As ADODB.Recordset
    Dim zDate As String
    
    If zYear <= TBYear.Text Then
        If zmonth >= 1 And zmonth <= 6 Then
            zTime = 1
        Else
            zTime = 2
        End If
    Else
        zTime = 2
    End If
    
'    SqlStr = "select * from moneyrate where seq =" & zTime & " and rateyear  = '" & zYear & "'"
    zDate = zYear & Format(zmonth, "00")
    SqlStr = "Select * From MoneyRate Where (SUBSTRING(PERIOD, 1, 6) <= '" & zDate & "') AND (SUBSTRING(PERIOD, 8, 13) >= '" & zDate & "')"
    Set Rn = Dbs.Execute(SqlStr)
    If Not Rn.EOF Then
        Select Case UCase(zCur)
            Case "YEN"
                If zYear < TBYear.Text Then
                    ChkRateSale = Rn!yenrater
                Else
                    ChkRateSale = Rn!yenrates
                End If
            Case "US$"
                If zYear < TBYear.Text Then
                    ChkRateSale = Rn!usrater
                Else
                    ChkRateSale = Rn!usrates
                End If
            Case "S$"
                If zYear < TBYear.Text Then
                    ChkRateSale = Rn!sgrater
                Else
                    ChkRateSale = Rn!sgrates
                End If
            Case "EUR"
                If zYear < TBYear.Text Then
                    ChkRateSale = Rn!eurrater
                Else
                    ChkRateSale = Rn!eurrates
                End If
        End Select
               
    End If
    
End Function

Function ChkRateRecv(zmonth As Integer, zYear As String, zCur As String) As Double
Dim zTime As Integer
Dim Rn As ADODB.Recordset
    
        zTime = 1
    
    SqlStr = "select * from moneyrate where seq =" & zTime & " and rateyear  = '" & zYear & "'"
    Set Rn = Dbs.Execute(SqlStr)
    If Not Rn.EOF Then
        Select Case UCase(zCur)
            Case "YEN"
                ChkRateRecv = Rn!yenrater
            Case "US$"
                ChkRateRecv = Rn!usrater
            Case "S$"
                ChkRateRecv = Rn!sgrater
            Case "EUR"
                ChkRateRecv = Rn!eurrater
        End Select
    Else

    End If
        
End Function

Function ChangeSection(zSection As String, zType As Integer) As String
    If zType = 1 Then
        Select Case zSection
            Case "10"
                ChangeSection = "Inter Sales"
            Case "20"
                ChangeSection = "Parfun"
            Case "30"
                ChangeSection = "Riki"
            Case "41"
                ChangeSection = "Dyeing"
            Case "42"
                ChangeSection = "Knitting"
            Case "50"
                ChangeSection = "Direct Sales"
            Case "55"
                ChangeSection = "NC-F"
            Case "56"
                ChangeSection = "Parfun,Riki"
            Case Else
                ChangeSection = "Other"
        End Select
    Else
        If zType = 2 Then
            Select Case zSection
                Case "Inter Sales"
                    ChangeSection = "10"
                Case "Parfun"
                    ChangeSection = "20"
                Case "Riki"
                    ChangeSection = "30"
                Case "Dyeing"
                    ChangeSection = "41"
                Case "Knitting"
                    ChangeSection = "42"
                Case "Direct Sales"
                    ChangeSection = "50"
                Case "NC-F"
                    ChangeSection = "55"
                Case "Parfun,Riki"
                    ChangeSection = "56"
                Case Else
                    ChangeSection = "60"
            End Select
        Else
            Select Case zSection
                Case "Inter Sales"
                    ChangeSection = "10"
                Case "Parfun"
                    ChangeSection = "20"
                Case "Riki"
                    ChangeSection = "30"
                Case "Dyeing"
                    ChangeSection = "41"
                Case "Knitting"
                    ChangeSection = "42"
                Case "Direct Sales"
                    ChangeSection = "50"
                Case Else
                    ChangeSection = "60"
            End Select
        End If
    End If
End Function

Sub DomesticBalance()
Dim Rt As ADODB.Recordset
Dim i As Integer
Dim zBegin, zSales, zEnding, zCollect, zOverdue As Double
Dim zmonth As String
Dim zMMDD As String
Dim zDate As String
Dim zLastDate As String
Dim zAmt As Double
Dim zDescr As String
Dim zCust As String
Dim zPayment As String
Dim zCredit, zNum As Integer
Dim zInDate As Date
Dim zBillDate As String
Dim zSection As String
Dim zRv As String
Dim zAmount, zAmtCollect As Double
Dim zOver As Boolean
Dim zRvNo As String
Dim zAmtCol As Double
Dim zSalesDebt, zDebitDebt, zCreditDebt As Double
'============================================
   Me.MousePointer = 11
    zmonth = ChkMonthString(CBMonth.Text)
    zMMDD = zmonth & "/" & TBYear
    zDate = zmonth & "/01/" & TBYear.Text
    zInDate = DateSerial(Int(TBYear.Text), Int(zmonth), 1)
    If Int(zmonth) < 10 Then
        If zmonth = "09" Then
            zLastDate = "10/01/" & TBYear
            zInDate = DateSerial(Int(TBYear.Text), 10, 1) - 1
        Else
            zLastDate = "0" & Int(zmonth) + 1 & "/01/" & TBYear.Text
            zInDate = DateSerial(Int(TBYear.Text), Int(zmonth) + 1, 1) - 1
        End If
        
    Else
        If zmonth = "12" Then
            zLastDate = "01/01/" & Int(TBYear) + 1
            zInDate = DateSerial(Int(TBYear.Text), 12, 31)
        Else
            zLastDate = Int(zmonth) + 1 & "/01/" & TBYear.Text
            zInDate = DateSerial(Int(TBYear.Text), Int(zmonth) + 1, 1) - 1
        End If
    End If
    i = 0
    
           
    '============================================================
    SqlStr = "SELECT invoiceno,invoicedate,collectdate,custnamee,rvno FROM domesticinvmain left join customeracc" & _
        " on domesticinvmain.cust_no = customeracc.cust_no" & _
        " WHERE ((invoicedate < '" & zLastDate & "' and collectdate IS NULL) " & _
        "or (invoicedate < '" & zDate & "' and amount > amtcollect)) and InvCancel=0"
        If Len(CBCust.Text) > 0 Then
            SqlStr = SqlStr & " and custnamee = N'" & CBCust.Text & "'"
        End If
        If Len(CBDept.Text) > 0 Then
            SqlStr = SqlStr & " and section = '" & CBDept.Text & "'"
        End If
        
        SqlStr = SqlStr & " Union" & _
        " select invoiceno,invoicedate,max(collectdate) as collectdate,custnamee,rvno from domesticcollect left join customeracc" & _
        " on domesticcollect.cust_no = customeracc.cust_no" & _
        " where ((datepart(MM,collectdate) = '" & zmonth & "'" & _
        " and datepart(yyyy,collectdate) = '" & TBYear.Text & "')" & _
        " or (datepart(MM,invoicedate) = '" & zmonth & "' and datepart(YYYY,invoicedate) = '" & TBYear.Text & "')" & _
        " or (invoicedate < '" & zDate & "' and collectdate >= '" & zLastDate & "'))"
        '" select invoiceno,invoicedate,min(collectdate) as collectdate,custnamee,rvno as rvno from domesticcollect left join customeracc" & _
        '" on domesticcollect.cust_no = customeracc.cust_no" & _
        '" where ((DATEPART(MM,invoicedate) = '" & zmonth & "' and DATEPART(YYYY,invoicedate) = '" & TBYear.Text & "'" & _
        '" or (invoicedate < '" & zDate & "' and collectdate >= '" & zLastDate & "')"
        '" and collectdate >= '" & zLastDate & "') OR (invoicedate < '" & zDate & "' and collectdate >= '" & zLastDate & "'))"
        If Len(CBCust.Text) > 0 Then
            SqlStr = SqlStr & " and custnamee = N'" & CBCust.Text & "'"
        End If
        If Len(CBDept.Text) > 0 Then
            SqlStr = SqlStr & " and section = '" & CBDept.Text & "'"
        End If
        
        SqlStr = SqlStr & " group by invoiceno,invoicedate,custnamee,rvno"
    Set Rs = Dbs.Execute(SqlStr)
    SqlStr = "delete from domesticunrecv"
    Dbs.Execute SqlStr
    If Not Rs.EOF Then
        ProgressBar1.Value = 0
        ProgressBar1.Min = 0
        ProgressBar1.Max = Rs.RecordCount
        Do While Not Rs.EOF
            zBegin = 0
            zSales = 0
            zEnding = 0
            zCollect = 0
            zOverdue = 0
            zOver = False
            zCust = IIf(IsNull(Rs!custnamee), "", Rs!custnamee)
            SqlStr = "select invoiceno,billdate,descr,domesticinvmain.payment,domesticinvmain.credit,section,amount" & _
            " from domesticinvmain where invoiceno = '" & Rs!invoiceno & "'"
            Set Rt = Dbs.Execute(SqlStr)
            If Not Rt.EOF Then
                
                zDescr = Rt!descr
                zPayment = Rt!payment
                zCredit = Int(Rt!credit)
                
                zSection = Rt!Section
                zAmount = Rt!amount
                If Not IsNull(Rt!billdate) Then
                    zBillDate = Format(Rt!billdate, "dd/mm/yyyy")
                    If zInDate - Rt!billdate > zCredit Then
                        zOver = True
                    End If
                Else
                    zBillDate = ""
                    If zInDate - Rs!invoicedate > zCredit Then
                        zOver = True
                    End If
                End If
            End If
            Rt.Close
            If Not IsNull(Rs!collectdate) Then
                If Month(Rs!collectdate) = Int(zmonth) Then
                    zRvNo = Rs!rvno
                    
                    'Check for Collect by one invoice No. and many RV. No. in same Month
                    zNum = 0
                    zAmtCol = 0
                    SqlStr = "select sum(amtcollect) as amtcollect ,count(invoiceno) as num from domesticcollect where invoiceno = '" & Rs!invoiceno & "'" & _
                    " and datepart(mm,collectdate) = '" & zmonth & "'"
                    Set Rt = Dbs.Execute(SqlStr)
                    If Not Rt.EOF Then
                        zNum = Rt!num
                        zAmtCollect = Rt!amtcollect
                    End If
                    Rt.Close
                    If zNum > 1 Then
                        'Receive = amount
                        'If zAmount = zAmtCollect Then
                            SqlStr = "select amtcollect from domesticcollect where invoiceno = '" & Rs!invoiceno & "'" & _
                            " and collectdate = '" & Format(Rs!collectdate, "mm/dd/yyyy") & "' and rvno = '" & zRvNo & "'"
                            Set Rt = Dbs.Execute(SqlStr)
                            If Not Rt.EOF Then
                                zAmt = Rt!amtcollect
                            End If
                            Rt.Close
                    
                        'End If
                    Else
                        SqlStr = "select sum(amtcollect) as amtcollect from domesticcollect where invoiceno = '" & Rs!invoiceno & "'" & _
                        " and collectdate <= '" & Format(Rs!collectdate, "mm/dd/yyyy") & "' and rvno <> '" & zRvNo & "'"
                        Set Rt = Dbs.Execute(SqlStr)
                        If Not Rt.EOF Then
                            If Not IsNull(Rt!amtcollect) Then
                                zAmt = zAmount - Rt!amtcollect
                            Else
                                zAmt = zAmount
                            End If
                        Else
                            zAmt = zAmount
                        End If
                        Rt.Close
                    End If
                                
                    SqlStr = "select rvno,amtcollect from domesticcollect where invoiceno = '" & Rs!invoiceno & "'" & _
                    " and collectdate = '" & Format(Rs!collectdate, "mm/dd/yyyy") & "' and rvno = '" & zRvNo & "'"
                    Set Rt = Dbs.Execute(SqlStr)
                    If Not Rt.EOF Then
                        
                        zCollect = Rt!amtcollect
                    End If
                    Rt.Close
                Else
                    If Month(Rs!collectdate) < Int(zmonth) Then
                        SqlStr = "select sum(amtcollect) as amtcollect from domesticcollect where invoiceno = '" & Rs!invoiceno & "'"
                        SqlStr = SqlStr & " and collectdate <= '" & zmonth & "/01/" & TBYear.Text & "'"
                    Else
                    SqlStr = "select sum(amtcollect) as amtcollect from domesticcollect where invoiceno = '" & Rs!invoiceno & "'" & _
                    " and collectdate < '" & zmonth & "/01/" & TBYear.Text & "'"
                    End If
                    Set Rt = Dbs.Execute(SqlStr)
                    If Not Rt.EOF Then
                        If Not IsNull(Rt!amtcollect) Then
                            zAmt = zAmount - Rt!amtcollect
                        Else
                            zAmt = zAmount
                        End If
                    Else
                        zAmt = zAmount
                    End If
                    Rt.Close
                    'zAmt = zAmount
                    zCollect = 0
                End If
            Else
                zCollect = 0
                zAmt = zAmount
            End If
            
            
'            If Not IsNull(Rs!collectdate) Then
'                SqlStr = "select sum(amtcollect) as amtcollect from domesticcollect where invoiceno = '" & Rs!invoiceno & "'" & _
'                " and collectdate <= '" & Format(Rs!collectdate, "mm/dd/yyyy") & "' and rvno <> '" & Rs!rvno & "'"
'                Set Rt = Dbs.Execute(SqlStr)
'                If Not Rt.EOF Then
'                    If Not IsNull(Rt!amtcollect) Then
'                        zAmt = Rt!amtcollect
'                    Else
'                        zAmt = zAmount
'                    End If
'                Else
'                    zAmt = zAmount
'                End If
'                Rt.Close
'                'zRvNo = ""
'                zRvNo = Rs!rvno
'                SqlStr = "select rvno,amtcollect from domesticcollect where invoiceno = '" & Rs!invoiceno & "'" & _
'                " and collectdate = '" & Format(Rs!collectdate, "mm/dd/yyyy") & "' and rvno = '" & Rs!rvno & "'"
'                Set Rt = Dbs.Execute(SqlStr)
'                If Not Rt.EOF Then
'
'                    zCollect = Rt!amtcollect
'                End If
'                Rt.Close
'            Else
'                zAmt = zAmount
'            End If
'
            If Rs!invoicedate < DateSerial(Int(TBYear.Text), Int(zmonth), 1) Then
                zBegin = zAmt
                zEnding = CDbl(Format(zBegin, "#,##0.00##")) - CDbl(Format(zCollect, "#,##0.00##"))
            Else
                zBegin = 0
                zSales = zAmt
                zEnding = CDbl(Format(zSales, "#,##0.00##")) - CDbl(Format(zCollect, "#,##0.00##"))
            End If
            If zEnding > 0 And zOver = True Then
                zOverdue = zEnding
            End If
            
            If zPayment = CBLC.Text Or CBLC.Text = "" And zEnding <> 0 Then         'âªÇìà©¾ÒÐ balance<>0
                i = i + 1
                FGrid.Rows = i + 1
                FGrid.TextMatrix(i, 0) = i
                FGrid.TextMatrix(i, 1) = Format(Rs!invoicedate, "yyyy/mm/dd")
                FGrid.TextMatrix(i, 2) = ChangeDate(zBillDate)
                Select Case zPayment
                    Case "Credit"
                        FGrid.TextMatrix(i, 3) = "Credit " & zCredit & " Days"
                    Case "T/T."
                        FGrid.TextMatrix(i, 3) = "T/T. " & zCredit & " Days"
                    Case Else
                        FGrid.TextMatrix(i, 3) = "CASH"
                End Select
                
                FGrid.TextMatrix(i, 4) = zCust
                FGrid.TextMatrix(i, 5) = zDescr
                FGrid.TextMatrix(i, 6) = Rs!invoiceno
                FGrid.TextMatrix(i, 7) = zSection
                FGrid.TextMatrix(i, 8) = Format(zAmount, "###,###,###,##0.00")
                If zEnding <> 0 Then
                    FGrid.TextMatrix(i, 9) = Format(zEnding, "###,###,###,##0.00")
                End If
                If zOverdue > 0 Then
                    FGrid.TextMatrix(i, 10) = Format(zOverdue, "###,###,###,##0.00")
                End If
                                      
                SqlStr = "insert into domesticunrecv (invoiceno,invoicedate,payment,credit,custname,descr,section,amount,beginning" & _
                ",ending,overdue) values ('" & Rs!invoiceno & "','" & Format(Rs!invoicedate, "mm/dd/yyyy") & "'" & _
                ",'" & zPayment & "','" & zCredit & "',N'" & zCust & "',N'" & zDescr & "','" & zSection & "'" & _
                "," & zAmount & "," & zBegin & "," & zEnding & "," & zOverdue & ")"
                Dbs.Execute SqlStr
            End If
            Rs.MoveNext
            ProgressBar1.Value = ProgressBar1.Value + 1
        Loop
    Else
        'Do nothing
        
    End If
    Rs.Close
    
    Me.MousePointer = 0
End Sub

'Function ChkRateRecvy(zYear As String, zCur As String) As Double
'Dim zTime As Integer
'Dim Rn As ADODB.Recordset
'
'        zTime = 0
'
'    SqlStr = "select * from moneyrate where seq =" & zTime & " and rateyear  = '" & zYear & "'"
'    Set Rn = Dbs.Execute(SqlStr)
'    If Not Rn.EOF Then
'        Select Case UCase(zCur)
'            Case "YEN"
'                ChkRateRecvy = Rn!yenrates
'            Case "US$"
'                ChkRateRecvy = Rn!usrates
'            Case "S$"
'                ChkRateRecvy = Rn!sgrates
'            Case "EUR"
'                ChkRateRecvy = Rn!eurrates
'        End Select
'
'    End If
'
'End Function

Sub CalReceive()
Dim zmonth, zDate, zBL, BLDate As String
Dim zBLDate As Date
Dim zMMDD, zLastMonth, zNextdate As String
Dim zno, zId, zCr, zCust As String
Dim zRateS, zSales, zRateR, zReceive, zBal As Double
Dim ChkBL As Boolean
Dim zAmt, zOverdue, zBalAmt, zAmount, zAmount_tmp As Double
    
    If CBMonth.ListIndex < 0 Then
        zmonth = Month(Date)
    Else
        zmonth = Trim(Str(CBMonth.ListIndex + 1))
    End If
    
    If CInt(zmonth) = 1 Then
        zLastMonth = "12/" & CLng(TBYear) - 1
        zmonth = "0" & zmonth
        zNextdate = "02/01/" & TBYear.Text
        
    Else
        If CInt(zmonth) < 10 Then
            zLastMonth = "0" & CInt(zmonth) & "/" & TBYear
            zmonth = "0" & zmonth
            If zmonth = "09" Then
                zNextdate = "10/01/" & TBYear.Text
            Else
                zNextdate = "0" & Trim(Str(CInt(zmonth) + 1)) & "/01/" & TBYear.Text
            End If
        Else
            zLastMonth = CBMonth.ListIndex & "/" & TBYear
            If zmonth = "12" Then
                zNextdate = "01/01/" & CInt(TBYear.Text) + 1
            Else
                zNextdate = Trim(Str(CInt(zmonth) + 1)) & "/01/" & TBYear.Text
            End If
        End If
    End If
    zBLDate = DateSerial(Int(TBYear.Text), CBMonth.ListIndex + 1, 1)
    zMMDD = zmonth & "/" & TBYear

    SqlStr = "delete acctmpreport  where mmyy = '" & zMMDD & "1'"
    Dbs.Execute SqlStr
    
    'Begin
    
    SqlStr = "insert into acctmpreport (mmyy,invoice_no,inv_date,bl_date,cr,custname,dept_id,curtype,id,inv_no,invdate" & _
    ",bldate,beginning) select '" & zMMDD & "1',invoice_no,inv_date,bl_date,cr,custname,dept_id,curtype,id,inv_no,invdate" & _
    ",bldate,balance from acctmpreport where mmyy = '" & zLastMonth & "' and balance > 0"
    Dbs.Execute SqlStr
    
    'Sales
    
    SqlStr = "select invoice_no,inv_date,exinv_no,cr,amt,curtype,bl_date,custname,dept_id from expbillrecord" & _
    " where datepart(mm,inv_date) = '" & zmonth & "' and datepart(yyyy,inv_date) ='" & TBYear.Text & "' and nodebtor = 0"
    Set Rt = Dbs.Execute(SqlStr)
    If Not Rt.EOF Then
        Do While Not Rt.EOF
            zId = IIf(IsNull(Rt!exinv_no), "", Rt!exinv_no)
            zCr = IIf(IsNull(Rt!cr), "", Rt!cr)
            If Left(Rt!invoice_no, 3) = "TUW" Then
                zno = Right(Rt!invoice_no, Len(Rt!invoice_no) - InStr(1, Rt!invoice_no, " "))
                zno = Left(zno, InStr(1, zno, "/") - 1)
            Else
                If Left(Rt!invoice_no, 7) = "DEB NO." Then
                    zno = Right(Rt!invoice_no, Len(Rt!invoice_no) - 7)
                Else
                    zno = Rt!invoice_no
                End If
            End If
            If Not IsNull(Rt!bl_date) Then
                zBL = Format(Rt!bl_date, "mm/dd/yyyy")
                BLDate = Format(Rt!bl_date, "dd/mm/yyyy")
            Else
                zBL = Format(Rt!inv_date, "mm/dd/yyyy")
                BLDate = Format(Rt!inv_date, "dd/mm/yyyy")
            End If
            zRateS = ChkRateSale(Month(Rt!inv_date), Year(Rt!inv_date), Rt!curtype)
            zSales = CDbl(Format(Rt!amt * zRateS, "#,##0.00"))
            
            SqlStr = "insert into acctmpreport (mmyy,invoice_no,inv_date,bl_date,cr,custname,dept_id,curtype,id,inv_no,amt,invdate" & _
            ",bldate,sales) values ('" & zMMDD & "1','" & Rt!invoice_no & "','" & Format(Rt!inv_date, "mm/dd/yyyy") & "'" & _
            ",'" & zBL & "','" & zCr & "','" & Replace(Rt!custname, "'", "''") & "','" & Rt!dept_id & "','" & Rt!curtype & "'" & _
            ",'" & zId & "','" & zno & "'," & Rt!amt & ",'" & ChangeDate(Format(Rt!inv_date, "dd/mm/yyyy")) & "','" & ChangeDate(BLDate) & "'" & _
            "," & zSales & ")"
            Dbs.Execute SqlStr
            Rt.MoveNext
        Loop
    End If
    Rt.Close

    'Receive
    SqlStr = "select invoice_no,inv_date,collect_date,rate,rv_no,curtype,(usamt+yenamt+sgamt+euramt) as amt from expbillreceive" & _
    " where datepart(mm,collect_date) = '" & zmonth & "' and datepart(yyyy,collect_date) ='" & TBYear.Text & "'"
    Set Rs = Dbs.Execute(SqlStr)
    If Not Rs.EOF Then
        Do While Not Rs.EOF
            If TBYear.Text = Year(Rs!inv_date) Then
                zRateS = ChkRateSale(Month(Rs!inv_date), Year(Rs!inv_date), Rs!curtype)
            Else
                zRateS = ChkRateRecvy(Year(Rs!inv_date), Rs!curtype)
            End If
            If zmonth = "12" Then
                zRateR = ChkRateRecvy(TBYear.Text, Rs!curtype)
            Else
                zRateR = Rs!Rate
            End If
            zReceive = CDbl(Format(Rs!amt * zRateS, "#,##0.00"))
            SqlStr = "update acctmpreport set rater = " & zRateR & ",collect_date = '" & Format(Rs!collect_date, "mm/dd/yyyy") & "'" & _
            ",recvdate = '" & ChangeDate(Format(Rs!collect_date, "dd/mm/yyyy")) & "',rv_no = '" & Rs!rv_no & "'" & _
            ",receive = " & zReceive & " where invoice_no = '" & Rs!invoice_no & "' and mmyy = '" & zMMDD & "1'"
            Dbs.Execute SqlStr
            
            Rs.MoveNext
        Loop
    End If
    Rs.Close
    
    'Balance
    zDate = zmonth & "/01/" & TBYear.Text
    SqlStr = "select * from acctmpreport where mmyy = '" & zMMDD & "1' order by dept_id,custname,invoice_no"
    Set Rs = Dbs.Execute(SqlStr)
    If Not Rs.EOF Then
        Do While Not Rs.EOF
            If TBYear.Text = Year(Rs!invdate) Then
                zRateS = ChkRateSale(Month(Rs!inv_date), Year(Rs!inv_date), Rs!curtype)
            Else
                zRateS = ChkRateRecvy(Year(Rs!inv_date), Rs!curtype)
            End If
            
            SqlStr = "select amt,(SELECT SUM(usamt + yenamt + sgamt + euramt) From expbillreceive" & _
            " WHERE invoice_no = '" & Rs!invoice_no & "' and collect_date < '" & zDate & "') as amtrec" & _
            " from expbillrecord where invoice_no = '" & Rs!invoice_no & "'"
            Set Rt = Dbs.Execute(SqlStr)
            If Not Rt.EOF Then
                zAmt = Rt!amt
                If Not IsNull(Rt!amtrec) Then
                    zBalAmt = zAmt - Rt!amtrec
                Else
                    zBalAmt = zAmt
                End If
            End If
            Rt.Close
            If zmonth = "12" Then
                zRateR = ChkRateRecvy(TBYear.Text, Rs!curtype)
            Else
                zRateR = zRateS
            End If
            If Rs!Beginning > 0 Then
                If Rs!receive = Rs!Beginning Then
                    zBal = 0
                Else
                    If zmonth = "12" Then
                        SqlStr = " SELECT SUM(usamt + yenamt + sgamt + euramt) as amt From expbillreceive" & _
                        " WHERE invoice_no = '" & Rs!invoice_no & "' and collect_date < '" & zNextdate & "'"
                        Set Rt = Dbs.Execute(SqlStr)
                        If Not Rt.EOF Then
                            If IsNull(Rt!amt) Then
                                zAmount = zAmt
                            Else
                                zAmount = zAmt - Rt!amt
                            End If
                        Else
                            zAmount = zAmt
                        End If
                        Rt.Close
                        zBal = CDbl(Format(zAmount * zRateR, "#,##0.00"))
                    Else
                        zBal = CDbl(Format((Rs!Beginning + Rs!Sales) - Rs!receive, "#,##0.00"))
                    End If
                End If
            ElseIf Rs!Sales > 0 Then
                If Rs!receive = Rs!Sales Then
                    zBal = 0
                Else
                    If zmonth = "12" Then
                        SqlStr = " SELECT SUM(usamt + yenamt + sgamt + euramt) as amt From expbillreceive" & _
                        " WHERE invoice_no = '" & Rs!invoice_no & "' and collect_date < '" & zNextdate & "'"
                        Set Rt = Dbs.Execute(SqlStr)
                        If Not Rt.EOF Then
                            If IsNull(Rt!amt) Then
                                zAmount = zAmt
                            Else
                                zAmount = zAmt - Rt!amt
                            End If
                        Else
                            zAmount = zAmt
                        End If
                        Rt.Close
                        
                        zBal = CDbl(Format(zAmount * zRateR, "#,##0.00"))
                    Else
                        zBal = CDbl(Format((Rs!Beginning + Rs!Sales) - Rs!receive, "#,##0.00"))
                    End If
                End If
            End If
            
            If Rs!bl_date < zBLDate Then
                zOverdue = zBal
            Else
                zOverdue = 0
            End If
            
            SqlStr = "update acctmpreport set amt = " & zBalAmt & ",rates = " & zRateS & ",balance = " & zBal & _
            ",overdue = " & zOverdue & " where mmyy = '" & zMMDD & "1' and invoice_no = '" & Rs!invoice_no & "'"
            Dbs.Execute SqlStr
            
            
            Rs.MoveNext
        Loop
    End If
    Rs.Close
    
End Sub



Function ChangeDate(zDate As String) As String
Dim zM As String
    zM = Mid(zDate, 4, 2)
    Select Case zM
        Case "01"
            ChangeDate = Left(zDate, 2) & "-Jan-" & Right(zDate, 2)
        Case "02"
            ChangeDate = Left(zDate, 2) & "-Feb-" & Right(zDate, 2)
        Case "03"
            ChangeDate = Left(zDate, 2) & "-Mar-" & Right(zDate, 2)
        Case "04"
            ChangeDate = Left(zDate, 2) & "-Apr-" & Right(zDate, 2)
        Case "05"
            ChangeDate = Left(zDate, 2) & "-May-" & Right(zDate, 2)
        Case "06"
            ChangeDate = Left(zDate, 2) & "-Jun-" & Right(zDate, 2)
        Case "07"
            ChangeDate = Left(zDate, 2) & "-Jul-" & Right(zDate, 2)
        Case "08"
            ChangeDate = Left(zDate, 2) & "-Aug-" & Right(zDate, 2)
        Case "09"
            ChangeDate = Left(zDate, 2) & "-Sep-" & Right(zDate, 2)
        Case "10"
            ChangeDate = Left(zDate, 2) & "-Oct-" & Right(zDate, 2)
        Case "11"
            ChangeDate = Left(zDate, 2) & "-Nov-" & Right(zDate, 2)
        Case "12"
            ChangeDate = Left(zDate, 2) & "-Dec-" & Right(zDate, 2)
        
    End Select
End Function



 */
