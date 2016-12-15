Option Compare Text
Option Explicit On
Option Strict On
Imports System.Globalization
Public Class frmPO
    Private DB As New cDatabase(TUW99)
    Private DB_TPiCS As New cDatabase(Fabric)
    Private Eng_Clinfo As New CultureInfo("en-US")
    Private Eng_dtfinfo As DateTimeFormatInfo = Eng_Clinfo.DateTimeFormat

    Public Sub ClearForm(ByVal blnCust As Boolean, ByVal blnPO As Boolean, ByVal blnGrid As Boolean)
        If blnCust = True Then
            txtSupplierID.Text = ""
            cboSupplier.Text = ""
            txtAD1.Text = ""
            txtAD2.Text = ""
            txtZip.Text = ""
            txtCountry.Text = ""
            txtTel.Text = ""
            txtFax.Text = ""
        End If
        If blnPO = True Then
            txtDept.Text = "FABRIC CONTROL"
            txtCustomer.Text = ""
            txtSaleOrder.Text = ""
            'cboPO.Text = ""
            dtpPODate.EditValue = Today
            txtReq.Text = ""
            dtpReqDate.EditValue = ""
            txtRef.Text = ""
            txtPayTerm.Text = ""
            dtpDelivery.EditValue = Today
            txtRemark1.Text = ""
            txtRemark2.Text = ""
            txtRemark3.Text = ""
            txtTotal.Text = ""
            txtVat.Text = "7"
            txtVatTotal.Text = ""
            txtGrand.Text = ""
            cboCur.SelectedIndex = 0
            cboPOType.SelectedIndex = 0
            cboSection.SelectedIndex = 0
            optProduct.SelectedIndex = 0
            cboRemark.SelectedIndex = 0
            'picPerson.Image = Nothing
            chkRevise.Checked = False
            chkCancel.Checked = False
        End If
        If blnGrid = True Then
            Dim DT As New DataTable
            With DT
                DT.BeginInit()
                DT.Columns.Add("TPICS_ORDER", GetType(String))
                DT.Columns.Add("CODE", GetType(String))
                DT.Columns.Add("BARCODE", GetType(String))
                DT.Columns.Add("DESCRIPTION", GetType(String))
                DT.Columns.Add("QTY", GetType(Double))
                DT.Columns.Add("UNIT", GetType(String))
                DT.Columns.Add("PRICE", GetType(Double))
                DT.Columns.Add("AMOUNT", GetType(Double))
                DT.Columns.Add("BUMO", GetType(String))
                DT.EndInit()
            End With
            Grid.DataSource = DT
            With GridView
                .Columns("BUMO").Visible = False
                .Columns("QTY").SummaryItem.FieldName = "QTY"
                .Columns("QTY").SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
                .Columns("AMOUNT").SummaryItem.FieldName = "AMOUNT"
                .Columns("AMOUNT").SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
                .OptionsView.EnableAppearanceEvenRow = True
                .OptionsView.EnableAppearanceOddRow = True
                .OptionsView.ColumnAutoWidth = True
                .BestFitColumns()
            End With
        End If
    End Sub

    Public Sub LoadSupplierName()
        Dim strSQL As String = "SELECT NAME FROM XSECT WHERE DESNAME IN ('Order1','Outsource1') ORDER BY NAME"
        Dim DT As DataTable = DB_TPiCS.GetDataTable(strSQL)
        For Each DR As DataRow In DT.Rows
            cboSupplier.Properties.Items.Add(DR("NAME").ToString)
        Next
        DT = Nothing
    End Sub

    Public Sub LoadSupplierName_Detail(Optional ByVal strID As String = "", Optional ByVal strSupplier As String = "")
        'Dim strSQL As String = "SELECT ID,NAME,ADDRESS1,ADDRESS2,COUNTRY,TEL,FAX,PAYMENT FROM FabricSupplier WHERE NAME=N'" & cboSupplier.Text & "'"
        Dim strSQL As String = "SELECT BUMO,NAME,ADR1,ADR2,ZIP,COUNTRY,MAIL,TEL,FAX,PAYTERM,CURRE,HITO FROM XSECT "
        If strID = "" Then
            strSQL = strSQL & "WHERE NAME='" & strSupplier & "'"
        Else
            strSQL = strSQL & "WHERE BUMO='" & strID & "'"
        End If
        Dim DT As DataTable = DB_TPiCS.GetDataTable(strSQL)
        'Call ClearForm(True, False, False)
        For Each DR As DataRow In DT.Rows
            txtSupplierID.Text = DR("BUMO").ToString
            cboSupplier.Text = DR("NAME").ToString
            txtAD1.Text = DR("ADR1").ToString
            txtAD2.Text = DR("ADR2").ToString
            txtZip.Text = DR("ZIP").ToString
            txtCountry.Text = DR("COUNTRY").ToString
            txtTel.Text = DR("TEL").ToString
            txtFax.Text = DR("FAX").ToString
            txtPayTerm.Text = DR("PAYTERM").ToString
            'cboCur.SelectedIndex = cboCur.Properties.Items.IndexOf(DR("CURRE").ToString)
        Next
    End Sub

    Private Sub LoadPONo()
        'Dim strSQL As String = "SELECT PONo FROM FabricNormalPO where right(podate,2) = '" & Strings.Right(Today.Year.ToString("00"), 2) & "' order by pono"
        Dim strSQL As String = "SELECT PONO FROM  THPO ORDER BY PONO"
        Dim DT As DataTable = DB_TPiCS.GetDataTable(strSQL)
        cboPO.Properties.Items.Clear()
        For Each DR As DataRow In DT.Rows
            cboPO.Properties.Items.Add(DR("PONO").ToString)
        Next
    End Sub

    Private Sub DisplayData(ByVal strPO As String)
        Dim strCurr As String = ""
        Dim strSQL As String = "SELECT * FROM THPO WHERE PONO='" & strPO & "'"
        Dim DT As DataTable = DB_TPiCS.GetDataTable(strSQL)

        Call ClearForm(False, True, False)
        For Each DR As DataRow In DT.Rows
            txtDept.Text = DR("DeptID").ToString
            txtCustomer.Text = DR("CustNo").ToString
            txtSaleOrder.Text = DR("SOrdNo").ToString
            dtpPODate.EditValue = New Date(CInt(Strings.Left(DR("PODate").ToString, 4)), CInt(Strings.Mid(DR("PODate").ToString, 5, 2)), CInt(Strings.Right(DR("PODate").ToString, 2))).ToShortDateString
            txtReq.Text = DR("ReqNo").ToString
            dtpReqDate.EditValue = New Date(CInt(Strings.Left(DR("ReqDate").ToString, 4)), CInt(Strings.Mid(DR("ReqDate").ToString, 5, 2)), CInt(Strings.Right(DR("ReqDate").ToString, 2))).ToShortDateString
            txtRef.Text = DR("RefNo").ToString
            txtPayTerm.Text = DR("PayTerm").ToString
            dtpDelivery.EditValue = New Date(CInt(Strings.Left(DR("TimeDly").ToString, 4)), CInt(Strings.Mid(DR("TimeDly").ToString, 5, 2)), CInt(Strings.Right(DR("TimeDly").ToString, 2))).ToShortDateString
            txtRemark1.Text = DR("Remark1").ToString
            txtRemark2.Text = DR("Remark2").ToString
            txtRemark3.Text = DR("Remark3").ToString
            txtTotal.Text = DR("TAmt").ToString
            txtVat.Text = DR("Vat").ToString
            txtVatTotal.Text = DR("TVat").ToString
            txtGrand.Text = DR("GTotal").ToString
            strCurr = DR("CURRE").ToString
            cboPOType.SelectedIndex = cboPOType.Properties.Items.IndexOf(DR("POType").ToString)
            cboSection.SelectedIndex = cboSection.Properties.Items.IndexOf(DR("SectID").ToString)
            cboRemark.SelectedIndex = cboRemark.Properties.Items.IndexOf(DR("POTitle").ToString)
            optProduct.SelectedIndex = CInt(DR("Type"))
            If DR("Cancel").ToString = "1" Then
                chkCancel.Checked = True
            Else
                chkCancel.Checked = False
            End If
            'Try
            '    picPerson.Image = Image.FromFile(Application.StartupPath & "\Images\" & DR("UPDBY").ToString & ".jpg")
            'Catch ex As Exception
            '    'Do nothing
            'End Try
            Call LoadSupplierName_Detail(DR("SUPID").ToString, )
        Next
        Grid.DataSource = DataToGrid(strPO)
        cboCur.SelectedIndex = cboCur.Properties.Items.IndexOf(strCurr)
        DT = Nothing
    End Sub

    Private Function DataToGrid(ByVal strPO As String) As DataTable
        Dim strSQL As String = "SELECT * FROM TDPO WHERE PONO='" & strPO & "' ORDER BY ROWNO"
        Dim DT As DataTable = DB_TPiCS.GetDataTable(strSQL)
        Call ClearForm(False, False, True)
        With DT
            .Columns("Code").ColumnName = "CODE"
            .Columns("Barcode").ColumnName = "BARCODE"
            .Columns("Descr").ColumnName = "DESCRIPTION"
            .Columns("Qty").ColumnName = "QTY"
            .Columns("Unit").ColumnName = "UNIT"
            .Columns("UPrc").ColumnName = "PRICE"
            .Columns("Amt").ColumnName = "AMOUNT"
        End With
        Return DT
    End Function

    Private Sub CalculateTotal()
        Dim dblTotal As Double = 0.0
        For i As Integer = 0 To GridView.RowCount - 1
            dblTotal += CDbl(GridView.GetRowCellValue(i, "AMOUNT"))
        Next
        txtTotal.Text = dblTotal.ToString
        txtVatTotal.Text = CStr(dblTotal * (CDbl(txtVat.Text) / 100))
        txtGrand.Text = CStr(dblTotal + (dblTotal * (CDbl(txtVat.Text) / 100)))
    End Sub

#Region "For FabricNormalPO"
    Private Function CheckTypeID(ByVal strType As String) As String
        Select Case strType
            Case "Local"
                Return "3"
            Case "Import"
                Return "4"
            Case "Sub Contract"
                Return "5"
        End Select
    End Function

    Private Function CheckMoneyType(ByVal strType As String) As Integer
        Select Case strType
            Case "Baht"
                Return 0
            Case "EUR"
                Return -1
            Case "GBP"
                Return 5
            Case "HKD"
                Return 4
            Case "SGD"
                Return -1
            Case "USD"
                Return 1
            Case "YEN"
                Return 2
        End Select
    End Function
#End Region

    Private Sub SaveData(ByVal strPO As String)
        Dim strSQL As String = "SELECT COUNT(PONO) FROM THPO WHERE PONO='" & strPO & "'"
        If CInt(DB_TPiCS.ExecuteFirstValue(strSQL)) = 0 Then
            'Insert TPiCS
            strSQL = "INSERT INTO THPO(PONO,EDA,PODATE,SUPID,POTYPE,SECTID,DEPTID,CURRE,POTITLE,CUSTNO,SORDNO,REQNO,REQDATE,REFNO,PAYTERM" & _
                ",TIMEDLY,TYPE,TQTY,TAMT,VAT,TVAT,GTOTAL,REMARK1,REMARK2,REMARK3,UPDBY,UPDDATE,CANCEL)VALUES('" & strPO & "',0" & _
                ",'" & CDate(dtpPODate.EditValue).ToString("yyyyMMdd", Eng_dtfinfo) & "','" & txtSupplierID.Text & "','" & cboPOType.Text & "','" & cboSection.Text & "'" & _
                ",'" & txtDept.Text & "','" & cboCur.Text & "','" & cboRemark.Text & "','" & txtCustomer.Text & "','" & txtSaleOrder.Text & "','" & txtReq.Text & "'" & _
                ",'" & CDate(dtpReqDate.EditValue).ToString("yyyyMMdd", Eng_dtfinfo) & "','" & txtRef.Text & "','" & txtPayTerm.Text & "'" & _
                ",'" & CDate(dtpDelivery.EditValue).ToString("yyyyMMdd", Eng_dtfinfo) & "','" & optProduct.SelectedIndex & "'" & _
                "," & CDbl(GridView.Columns("QTY").SummaryItem.SummaryValue) & "," & txtTotal.Text & "," & txtVat.Text & "," & txtVatTotal.Text & "," & txtGrand.Text & _
                ",N'" & txtRemark1.Text & "',N'" & txtRemark2.Text & "',N'" & txtRemark3.Text & "','" & My.Computer.Name & "','" & Today.ToString("yyyyMMdd", Eng_dtfinfo) & "'"
            If chkCancel.Checked = True Then
                strSQL += ",'1'"
            Else
                strSQL += ",'0'"
            End If
            strSQL += ")"
            DB_TPiCS.Execute(strSQL)
            'Insert PurchaseOrder
            strSQL = "INSERT INTO FABRICNORMALPO(PONO,PODATE,SUPPLIERID,DEPARTMENT,DEPARTMENTID,TYPEID,CUSTOMERNAME,SALENO,REQNO,REQDATE" & _
                ",REFNO,REM1,REM2,REM3,TOTALAMOUNT,VAT,MONEYTYPE,PAYTERM,DATEDELIVERY,DISCOUNT,STATUS,POTYPE,REMARK,REVISED,REVISEDATE)VALUES(" & _
                "'" & strPO & "','" & CDate(dtpPODate.EditValue).ToString("dd/MM/yy", Eng_dtfinfo) & "','" & txtSupplierID.Text & "','" & txtDept.Text & "'" & _
                ",'" & cboSection.Text & "','" & CheckTypeID(cboPOType.Text) & "','" & txtCustomer.Text & "','" & txtSaleOrder.Text & "','" & txtReq.Text & "'" & _
                ",'" & CDate(dtpReqDate.EditValue).ToString("dd/MM/yy", Eng_dtfinfo) & "','" & txtRef.Text & "',N'" & txtRemark1.Text & "',N'" & txtRemark2.Text & "'" & _
                ",N'" & txtRemark3.Text & "'," & txtTotal.Text & "," & txtVat.Text & "," & CheckMoneyType(cboCur.Text) & ",'" & txtPayTerm.Text & "'" & _
                ",'" & CDate(dtpDelivery.EditValue).ToString("dd/MM/yy", Eng_dtfinfo) & "',0,0," & optProduct.SelectedIndex & ",'" & cboRemark.Text & "',0,'')"
            DB.Execute(strSQL)
            cboPO.Properties.Items.Add(strPO)
        Else
            'Update TPiCS
            strSQL = "UPDATE THPO SET PODATE='" & CDate(dtpPODate.EditValue).ToString("yyyyMMdd", Eng_dtfinfo) & "'" & _
                ",SUPID='" & txtSupplierID.Text & "',POTYPE='" & cboPOType.Text & "',SECTID='" & cboSection.Text & "'" & _
                ",DEPTID='" & txtDept.Text & "',CURRE='" & cboCur.Text & "',POTITLE='" & cboRemark.Text & "'" & _
                ",CUSTNO='" & txtCustomer.Text & "',SORDNO='" & txtSaleOrder.Text & "',REQNO='" & txtReq.Text & "'" & _
                ",REQDATE='" & CDate(dtpReqDate.EditValue).ToString("yyyyMMdd", Eng_dtfinfo) & "',REFNO='" & txtRef.Text & "'" & _
                ",PAYTERM='" & txtPayTerm.Text & "',TIMEDLY='" & CDate(dtpDelivery.EditValue).ToString("yyyyMMdd", Eng_dtfinfo) & "'" & _
                ",TYPE='" & optProduct.SelectedIndex & "',TQTY=" & CDbl(GridView.Columns("QTY").SummaryItem.SummaryValue) & _
                ",TAMT=" & txtTotal.Text & ",VAT=" & txtVat.Text & ",TVAT=" & txtVatTotal.Text & ",GTOTAL=" & txtGrand.Text & _
                ",REMARK1=N'" & txtRemark1.Text & "',REMARK2=N'" & txtRemark2.Text & "',REMARK3=N'" & txtRemark3.Text & "' " & _
                ",UPDBY='" & My.Computer.Name & "',UPDDATE='" & Today.ToString("yyyyMMdd", Eng_dtfinfo) & "'"
            If chkCancel.Checked Then
                strSQL += ",CANCEL='1'"
            Else
                strSQL += ",CANCEL='0'"
            End If
            strSQL += " WHERE PONO='" & strPO & "'"
            'MsgBox(strSQL)
            DB_TPiCS.Execute(strSQL)
            'Update PurchaseOrder
            strSQL = "UPDATE FABRICNORMALPO SET PODATE='" & CDate(dtpPODate.EditValue).ToString("dd/MM/yy", Eng_dtfinfo) & "'" & _
                ",SUPPLIERID='" & txtSupplierID.Text & "',DEPARTMENT='" & txtDept.Text & "',DEPARTMENTID='" & cboSection.Text & "'" & _
                ",TYPEID='" & CheckTypeID(cboPOType.Text) & "',CUSTOMERNAME='" & txtCustomer.Text & "',SALENO='" & txtSaleOrder.Text & "'" & _
                ",REQNO='" & txtReq.Text & "',REQDATE='" & CDate(dtpReqDate.EditValue).ToString("dd/MM/yy", Eng_dtfinfo) & "'" & _
                ",REFNO='" & txtRef.Text & "',REM1=N'" & txtRemark1.Text & "',REM2=N'" & txtRemark2.Text & "',REM3=N'" & txtRemark3.Text & "'" & _
                ",TOTALAMOUNT=" & txtTotal.Text & ",VAT=" & txtVat.Text & ",MONEYTYPE=" & CheckMoneyType(cboCur.Text) & _
                ",PAYTERM='" & txtPayTerm.Text & "',DATEDELIVERY='" & CDate(dtpDelivery.EditValue).ToString("dd/MM/yy", Eng_dtfinfo) & "'" & _
                ",DISCOUNT=0,STATUS=0,POTYPE=" & optProduct.SelectedIndex & ",REMARK='" & cboRemark.Text & "'" & _
                ",REVISED=0,REVISEDATE='' WHERE PONO='" & strPO & "'"
            'MsgBox(strSQL)
            DB.Execute(strSQL)
        End If
        'PO DETAIL TPICS
        strSQL = "DELETE FROM TDPO WHERE PONO='" & strPO & "'"
        DB_TPiCS.Execute(strSQL)
        For i As Integer = 0 To GridView.DataRowCount - 1
            strSQL = "INSERT INTO TDPO(PONO,EDA,ROWNO,TPICS_ORDER,CODE,BARCODE,DESCR,QTY,UNIT,UPRC,AMT)VALUES(" & _
                "'" & strPO & "',0," & i & ",'" & GridView.GetRowCellDisplayText(i, "TPICS_ORDER") & "'" & _
                ",'" & GridView.GetRowCellDisplayText(i, "CODE") & "','" & GridView.GetRowCellDisplayText(i, "BARCODE") & "'" & _
                ",N'" & GridView.GetRowCellDisplayText(i, "DESCRIPTION") & "'," & GridView.GetRowCellDisplayText(i, "QTY") & _
                ",'" & GridView.GetRowCellDisplayText(i, "UNIT") & "'," & GridView.GetRowCellDisplayText(i, "PRICE") & _
                "," & GridView.GetRowCellDisplayText(i, "AMOUNT") & ")"
            'MsgBox(strSQL)
            DB_TPiCS.Execute(strSQL)
        Next
        'PO DETAIL PURCHASEORDER
        strSQL = "DELETE FROM FABRICNORMALPODETAIL WHERE PONO='" & strPO & "'"
        DB.Execute(strSQL)
        Dim intRow As Integer = -1
        For i As Integer = 0 To GridView.DataRowCount - 1
            intRow += 1
            strSQL = "INSERT INTO FABRICNORMALPODETAIL(PONO,LINE,PRODUCTCODE,DESCRIPTION,QTY,UNIT,UNITPRICE,TPICS_ORDER)VALUES('" & strPO & "'" & _
                "," & intRow
            If cboRemark.Text = "RAW YARN" Then
                strSQL = strSQL & ",'" & CInt(GridView.GetRowCellDisplayText(i, "CODE")) & "'"
            Else
                strSQL = strSQL & ",'" & GridView.GetRowCellDisplayText(i, "CODE") & "'"
            End If
            strSQL = strSQL & ",N'" & GridView.GetRowCellDisplayText(i, "BARCODE") & "'" & _
                "," & GridView.GetRowCellDisplayText(i, "QTY") & ",'" & GridView.GetRowCellDisplayText(i, "UNIT") & "'" & _
                "," & GridView.GetRowCellDisplayText(i, "PRICE") & ",'" & GridView.GetRowCellDisplayText(i, "TPICS_ORDER") & "')"
            'MsgBox(strSQL)
            DB.Execute(strSQL)
            intRow += 1
            strSQL = "INSERT INTO FABRICNORMALPODETAIL(PONO,LINE,DESCRIPTION)VALUES('" & strPO & "'," & intRow & ",'" & GridView.GetRowCellDisplayText(i, "DESCRIPTION") & "')"
            'MsgBox(strSQL)
            DB.Execute(strSQL)
        Next
        'Update PONO in XSLIP
        For i As Integer = 0 To GridView.DataRowCount - 1
            strSQL = "UPDATE XSLIP SET CONT='SECTION " & cboSection.Text & "',PO='" & strPO & "',VENDOR2='" & cboSupplier.Text & "' " & _
                "WHERE PORDER='" & GridView.GetRowCellDisplayText(i, "TPICS_ORDER") & "'"
            'MsgBox(strSQL)
            DB_TPiCS.Execute(strSQL)
        Next
    End Sub

    Private Sub NewPO(ByVal strType As String)
        Try
            Me.Cursor = Cursors.WaitCursor
            DB_TPiCS.ConnectionOpen()
            Dim strSQL As String
            Select Case strType
                Case "RAW YARN"
                    strSQL = "SELECT MAX(PONO) AS PONO FROM THPO WHERE POTITLE='RAW YARN'"
                    Dim strTemp As String = DB_TPiCS.ExecuteFirstValue(strSQL)
                    If strTemp = "" Then strTemp = "000000"
                    Dim intTemp As Integer = CInt(Strings.Right(strTemp, 6)) + 1
                    cboPO.Text = "FX" & Format(intTemp, "000000")
                Case "DYEING FEE"
                    strSQL = "SELECT MAX(PONO) AS PONO FROM THPO WHERE POTITLE='DYEING FEE'"
                    Dim strTemp As String = DB_TPiCS.ExecuteFirstValue(strSQL)
                    If strTemp = "" Then strTemp = "000000"
                    Dim intTemp As Integer = CInt(Strings.Right(strTemp, 6)) + 1
                    cboPO.Text = "FD" & Format(intTemp, "000000")
                Case "KNITTING FEE"
                    strSQL = "SELECT MAX(SUBSTRING(PONO,6,4)) AS PONO FROM THPO " & _
                        "WHERE POTITLE='KNITTING FEE' AND SUBSTRING(REQDATE,1,4)='" & Today.ToString("yyyy", Eng_dtfinfo) & "'"
                    Dim strTemp As String = DB_TPiCS.ExecuteFirstValue(strSQL)
                    If strTemp = "" Then strTemp = "0000"
                    Dim intTemp As Integer = CInt(Strings.Right(strTemp, 4)) + 1
                    cboPO.Text = "FB.S"
                    If cboSection.Text.Length > 0 Then cboPO.Text = cboPO.Text & Strings.Right(cboSection.Text, 1)
                    cboPO.Text = cboPO.Text & Format(intTemp, "0000") & "-" & Today.ToString("yy", Eng_dtfinfo)
                    If txtRef.Text = "SAMPLE" Then cboPO.Text = cboPO.Text & "-S"
                Case "SOAPING"

            End Select
        Catch ex As Exception
            MsgBox(ex.Message.ToCharArray, MsgBoxStyle.Critical)
        Finally
            DB_TPiCS.ConnectionClose()
            Me.Cursor = Cursors.Default
        End Try

        frmPO_New.Show()
    End Sub

    Private Sub frmPO_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        With PictureEdit1
            .Image = My.Resources.Resource1.tuw_logo1
            .Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze
            .Properties.PictureAlignment = ContentAlignment.MiddleCenter
        End With
        With PictureEdit2
            .Image = My.Resources.Resource1.purchase_pic
            .Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze
            .Properties.PictureAlignment = ContentAlignment.MiddleCenter
        End With
        cmdPO.Image = My.Resources.Resource1.file_new.ToBitmap
        cmdSave.Image = My.Resources.Resource1.save.ToBitmap
        cmdPreview.Image = My.Resources.Resource1.print_preview.ToBitmap
        cmdPrint.Image = My.Resources.Resource1.print.ToBitmap
        cmdClear.Image = My.Resources.Resource1.clear.ToBitmap
        cmdCalculate.Image = My.Resources.Resource1.calculator.ToBitmap
        cmdExit.Image = My.Resources.Resource1._exit.ToBitmap
        With PopupMenu1
            .AddItem(mnuYarn)
            .AddItem(mnuKnit)
            .AddItem(mnuDye)
        End With
        Call ClearForm(True, True, True)
        Call LoadSupplierName()
        Call LoadPONo()
    End Sub

    'Private Sub cboSupplier_CloseUp(ByVal sender As Object, ByVal e As DevExpress.XtraEditors.Controls.CloseUpEventArgs) Handles cboSupplier.CloseUp
    '    Try
    '        Call LoadSupplierName_Detail(, cboSupplier.Text)
    '    Catch ex As Exception
    '        MsgBox(ex.Message.ToString, MsgBoxStyle.Exclamation)
    '    End Try
    'End Sub

    'Private Sub cboSupplier_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboSupplier.KeyPress
    '    If e.KeyChar = Chr(13) Then
    '        Try
    '            Call LoadSupplierName_Detail(, cboSupplier.Text)
    '        Catch ex As Exception
    '            MsgBox(ex.Message.ToString, MsgBoxStyle.Exclamation)
    '        End Try
    '    End If
    'End Sub

    Private Sub cmdSupplier_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSupplier.Click
        frmPO_Supplier.Show()
    End Sub

    Private Sub txtSupplierID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtSupplierID.KeyPress
        If e.KeyChar = Chr(13) Then
            Try
                LoadSupplierName_Detail(txtSupplierID.Text, )
            Catch ex As Exception
                MsgBox(ex.Message.ToString, MsgBoxStyle.Exclamation)
            End Try
        End If
    End Sub

    Private Sub cboPO_CloseUp(ByVal sender As Object, ByVal e As DevExpress.XtraEditors.Controls.CloseUpEventArgs) Handles cboPO.CloseUp
        Try
            Call DisplayData(cboPO.Text)
        Catch ex As Exception
            MsgBox(ex.Message.ToString, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub cboPO_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboPO.KeyPress
        If e.KeyChar = Chr(13) Then
            Try
                Call DisplayData(cboPO.Text)
            Catch ex As Exception
                MsgBox(ex.Message.ToString)
            End Try
        End If
    End Sub

    Private Sub txtDept_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtDept.KeyPress _
        , txtCustomer.KeyPress, txtSaleOrder.KeyPress, txtReq.KeyPress, txtRef.KeyPress, txtRemark1.KeyPress, txtRemark2.KeyPress, txtRemark3.KeyPress
        If e.KeyChar = Chr(13) Then My.Computer.Keyboard.SendKeys("{TAB}")
    End Sub

    Private Sub GridView_CustomRowCellEdit(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs) Handles GridView.CustomRowCellEdit
        If e.Column.FieldName = "DESCRIPTION" Then
            Dim rpDescription As New DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit
            e.RepositoryItem = rpDescription
        End If
    End Sub

    Private Sub GridView_RowUpdated(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Base.RowObjectEventArgs) Handles GridView.RowUpdated
        Call CalculateTotal()
    End Sub

    Private Sub GridView_ValidatingEditor(ByVal sender As Object, ByVal e As DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs) Handles GridView.ValidatingEditor
        Try
            Select Case GridView.FocusedColumn.FieldName
                Case "QTY"
                    GridView.SetRowCellValue(GridView.FocusedRowHandle, "AMOUNT", _
                        FormatNumber(CDbl(e.Value) * CDbl(GridView.GetRowCellValue(GridView.FocusedRowHandle, "PRICE")), 2))
                Case "PRICE"
                    GridView.SetRowCellValue(GridView.FocusedRowHandle, "AMOUNT", _
                        FormatNumber(CDbl(e.Value) * CDbl(GridView.GetRowCellDisplayText(GridView.FocusedRowHandle, "QTY")), 2))
            End Select
            Call CalculateTotal()
        Catch ex As Exception

        End Try
        'If GridView.FocusedColumn.FieldName = "AMOUNT" Then Call CalculateTotal()
    End Sub

    Private Sub cmdClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClear.Click
        cboPO.Text = ""
        Call ClearForm(True, True, True)
    End Sub

    Private Sub cmdSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSave.Click
        If cboCur.SelectedIndex = 0 Then
            MsgBox("โปรดเลือก Currency Symbol", MsgBoxStyle.Exclamation)
            Exit Sub
        ElseIf cboPOType.SelectedIndex = 0 Then
            MsgBox("โปรดเลือก P/O Type", MsgBoxStyle.Exclamation)
            Exit Sub
        ElseIf cboSection.SelectedIndex = 0 Then
            MsgBox("โปรดเลือก Section", MsgBoxStyle.Exclamation)
            Exit Sub
        ElseIf cboRemark.SelectedIndex = 0 Then
            MsgBox("โปรดเลือก Remark", MsgBoxStyle.Exclamation)
            Exit Sub
        ElseIf cboPO.Text.Length = 0 Then
            MsgBox("โปรดระบุ PO No.", MsgBoxStyle.Exclamation)
            Exit Sub
        ElseIf dtpPODate.EditValue Is Nothing Then
            MsgBox("โปรดระบุ PO Date", MsgBoxStyle.Exclamation)
            Exit Sub
        ElseIf dtpReqDate.EditValue Is Nothing Then
            MsgBox("โปรดระบุ Requisition Date.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If
        Try
            Me.Cursor = Cursors.WaitCursor
            DB.ConnectionOpen()
            DB.BeginTrans()
            DB_TPiCS.ConnectionOpen()
            DB_TPiCS.BeginTrans()
            Call SaveData(cboPO.Text)
            DB.CommitTrans()
            DB_TPiCS.CommitTrans()
            MsgBox("SAVE COMPLETE", MsgBoxStyle.Information)
        Catch ex As Exception
            DB.RollbackTrans()
            DB_TPiCS.RollbackTrans()
            MsgBox(ex.Message.ToString, MsgBoxStyle.Critical, "Error")
        Finally
            DB.ConnectionClose()
            DB_TPiCS.ConnectionClose()
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub cmdPO_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPO.Click
        PopupMenu1.ShowPopup(BarManager1, Windows.Forms.Cursor.Position)
    End Sub

    Private Sub Grid_ProcessGridKey(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Grid.ProcessGridKey
        If e.KeyCode = Keys.Delete Then If GridView.IsEditing = False Then GridView.DeleteSelectedRows()
    End Sub

    Private Sub cboRemark_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboRemark.SelectedIndexChanged
        Select Case cboRemark.Text
            Case "RAW YARN"
                optProduct.SelectedIndex = optProduct.Properties.Items.GetItemIndexByValue("Yarn")
            Case "DYEING FEE", "KNITTING FEE", "SOAPING"
                optProduct.SelectedIndex = optProduct.Properties.Items.GetItemIndexByValue("Fabric")
        End Select
    End Sub

    Private Sub cmdPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPrint.Click
        Try
            Dim crpPO As New cCrystalReport(Application.StartupPath & "\Report\PO.rpt")
            With crpPO
                Dim intTemp As Integer
                Try
                    intTemp = Integer.Parse(InputBox("คุณต้องการพิมพ์กี่ copy", "จำนวนสำเนา", "1"))
                Catch ex As Exception
                    intTemp = 1
                End Try
                If .SetPrinter() = False Then Exit Sub
                .ReportTitle = cboPO.Text
                For i As Integer = 1 To intTemp
                    .ClearParameters()
                    .SetParameter("Copy", i.ToString)
                    If chkRevise.Checked Then
                        .SetParameter("Revise", "REVISE")
                    Else
                        .SetParameter("Revise", "")
                    End If
                    Dim fmlText As String = "{THPO.PONO}='" & cboPO.Text & "'"
                    .PrintReport(fmlText, True)
                Next
            End With
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try

    End Sub

    Private Sub cboCur_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboCur.SelectedIndexChanged
        If cboCur.SelectedIndex = cboCur.Properties.Items.IndexOf("Baht") Then
            txtVat.Text = "7"
        Else
            txtVat.Text = "0"
        End If
        Call CalculateTotal()
    End Sub

    Private Sub SimpleButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCalculate.Click
        Try
            Call CalculateTotal()
        Catch ex As Exception
            'Do nothing
        End Try
    End Sub

    Private Sub cmdPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPreview.Click
        Try
            Dim crpPO As New cCrystalReport(Application.StartupPath & "\Report\PO.rpt")
            With crpPO
                If .SetPrinter() = False Then Exit Sub
                .ReportTitle = cboPO.Text
                For i As Integer = 1 To .ReportCopy
                    .ClearParameters()
                    .SetParameter("Copy", i.ToString)
                    If chkRevise.Checked Then
                        .SetParameter("Revise", "REVISE")
                    Else
                        .SetParameter("Revise", "")
                    End If
                    Dim fmlText As String = "{THPO.PONO}='" & cboPO.Text & "'"
                    .PrintReport(fmlText, False)
                Next
            End With
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try

    End Sub

    Private Sub mnuYarn_ItemClick(ByVal sender As System.Object, ByVal e As DevExpress.XtraBars.ItemClickEventArgs) Handles mnuYarn.ItemClick, mnuKnit.ItemClick, mnuDye.ItemClick
        If e.Item.Caption = "KNITTING FEE" Then
            If cboSection.SelectedIndex = 0 Then
                MsgBox("กรุณาเลือกแผนกก่อนจากช่อง Section", MsgBoxStyle.Exclamation)
                Exit Sub
            End If
        Else
            dtpReqDate.EditValue = Today
        End If
        cboRemark.SelectedIndex = cboRemark.Properties.Items.IndexOf(e.Item.Caption.ToString)
        Call NewPO(e.Item.Caption)
    End Sub

    Private Sub cboSupplier_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboSupplier.SelectedIndexChanged
        Try
            Call LoadSupplierName_Detail(, cboSupplier.Text)
        Catch ex As Exception
            MsgBox(ex.Message.ToString, MsgBoxStyle.Exclamation)
        End Try
    End Sub

    Private Sub SimpleButton1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Debug.Print(frmMDIParent.PointToScreen(frmMDIParent.Location).ToString)
        frmPO_New.Location = frmMDIParent.Location
    End Sub

    Private Sub cmdExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdExit.Click
        Me.Dispose()
    End Sub
End Class