Option Explicit On
Option Strict On

Imports System.Globalization

Public Class frmReceiveNote
    Private DB As New cDatabase(Fabric)
    Private Eng_clinfo As New CultureInfo("en-US")
    Private Eng_dtfinfo As DateTimeFormatInfo = Eng_clinfo.DateTimeFormat
    Private strStatus As String

    Public Sub ClearForm(ByVal blnHeader As Boolean, ByVal blnDetail As Boolean, ByVal blnReceiveNo As Boolean)
        If blnHeader = True Then
            cboPO.Text = ""
            cboDelivery.Text = ""
            cboDelivery.Properties.Items.Clear()

            txtSupplierID.Text = ""
            txtSupplier.Text = ""
            txtAD1.Text = ""
            txtAD2.Text = ""
            txtZip.Text = ""
            txtCountry.Text = ""
            txtTel.Text = ""
            txtFax.Text = ""

            dtpReceive.EditValue = Today

            txtRemark1.Text = ""
            txtRemark2.Text = ""
            txtRemark3.Text = ""
            txtTotal.Text = ""
            txtVat.Text = "7"
            txtVatTotal.Text = ""
            txtGrand.Text = ""

            txtCur.Text = ""
            txtPOType.Text = ""
            txtSection.Text = ""
            optProduct.SelectedIndex = 0
            txtRemark.Text = ""
            picPerson.Image = Nothing
            chkRevise.Checked = False
        End If
        If blnDetail = True Then
            Dim DT As New DataTable
            With DT
                DT.BeginInit()
                DT.Columns.Add("TPICS_ORDER", GetType(String))
                DT.Columns.Add("CODE", GetType(String))
                DT.Columns.Add("BARCODE", GetType(String))
                DT.Columns.Add("DESCRIPTION", GetType(String))
                DT.Columns.Add("P/O_QTY", GetType(Double))
                DT.Columns.Add("RECEIVE", GetType(Double))
                DT.Columns.Add("UNIT", GetType(String))
                DT.Columns.Add("PRICE", GetType(Double))
                DT.Columns.Add("AMOUNT", GetType(Double))
                DT.EndInit()
            End With
            Grid.DataSource = DT
            With GridView
                .Columns("P/O_QTY").SummaryItem.FieldName = "P/O_QTY"
                .Columns("P/O_QTY").SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
                .Columns("RECEIVE").SummaryItem.FieldName = "RECEIVE"
                .Columns("RECEIVE").SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
                .Columns("AMOUNT").SummaryItem.FieldName = "AMOUNT"
                .Columns("AMOUNT").SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
                .OptionsView.EnableAppearanceEvenRow = True
                .OptionsView.EnableAppearanceOddRow = True
                .OptionsView.ColumnAutoWidth = True
                .BestFitColumns()
            End With
        End If
        If blnReceiveNo = True Then
            cboReceive.Text = ""
        End If
    End Sub

    Private Sub ImportData(ByVal strPO As String, ByVal strDelivery As String)
        Dim strSQL As String = "SELECT A.PONo,A.SUPID,B.NAME,B.ADR1,B.ADR2,B.ZIP,B.COUNTRY,B.TEL," & _
            "B.FAX,A.Curre,A.SectID,A.POTitle,A.POType,A.Type " & _
            "FROM THPO A INNER JOIN XSECT B ON A.SUPID = B.BUMO WHERE (A.PONo = '" & strPO & "')" & _
            ";SELECT  TDPO.TPICS_ORDER, TDPO.CODE, TDPO.Barcode AS BARCODE, TDPO.Descr AS DESCRIPTION, TDPO.Qty AS [P/O_QTY]" & _
            ",TDPO.Unit AS UNIT" & _
            ",CASE WHEN XITEM.KANZANZA=0 THEN XSACT.APRICE " & _
            "WHEN XITEM.KANZANZA=1 THEN XSACT.APRICE/XITEM.KANZANK " & _
            "WHEN XITEM.KANZANZA=2 THEN XSACT.APRICE*XITEM.KANZANK END AS PRICE" & _
            ",CASE WHEN XITEM.KANZANZA=0 THEN XSACT.APRICE " & _
            "WHEN XITEM.KANZANZA=1 THEN XSACT.APRICE/XITEM.KANZANK " & _
            "WHEN XITEM.KANZANZA=2 THEN XSACT.APRICE*XITEM.KANZANK END AS PRICE" & _
            ",XSACT.KOUNYUUGAKU AS AMOUNT" & _
            ",CASE WHEN XITEM.KANZANZA=0 THEN XSACT.JITU0 " & _
            "WHEN XITEM.KANZANZA=1 THEN XSACT.JITU0*XITEM.KANZANK " & _
            "WHEN XITEM.KANZANZA=2 THEN XSACT.JITU0/XITEM.KANZANK END AS RECEIVE " & _
            "FROM TDPO INNER JOIN XSACT ON TDPO.TPICS_ORDER = XSACT.PORDER INNER JOIN XITEM ON XSACT.CODE = XITEM.CODE " & _
            "AND XSACT.BUMO=XITEM.BUMO " & _
            "WHERE (TDPO.PONo = '" & strPO & "') AND (XSACT.DELIVERYNO = '" & strDelivery & "')"
        '    ";SELECT TDPO.TPICS_ORDER,TDPO.CODE,TDPO.Barcode AS BARCODE,TDPO.Descr AS DESCRIPTION,TDPO.Qty AS [P/O_QTY], " & _
        '    "TDPO.Unit AS UNIT,XSACT.APRICE AS PRICE,XSACT.KOUNYUUGAKU AS AMOUNT,XSACT.JITU0 AS RECEIVE " & _
        '    "FROM TDPO INNER JOIN XSACT ON TDPO.TPICS_ORDER = XSACT.PORDER WHERE PONO='" & strPO & "' AND DELIVERYNO='" & strDelivery & "' "
        Dim DS As DataSet = DB.GetDataSet(strSQL)
        For Each DR As DataRow In DS.Tables(0).Rows
            txtSupplier.Text = DR("NAME").ToString
            txtSupplierID.Text = DR("SUPID").ToString
            txtAD1.Text = DR("ADR1").ToString
            txtAD2.Text = DR("ADR2").ToString
            txtZip.Text = DR("ZIP").ToString
            txtCountry.Text = DR("COUNTRY").ToString
            txtTel.Text = DR("TEL").ToString
            txtFax.Text = DR("FAX").ToString
            txtCur.Text = DR("Curre").ToString
            txtSection.Text = DR("SectID").ToString
            txtRemark.Text = DR("POTitle").ToString
            txtPOType.Text = DR("POType").ToString
            optProduct.SelectedIndex = CInt(DR("Type"))
        Next
        Grid.DataSource = DS.Tables(1)
    End Sub

    Private Sub DisplayData(ByVal strReceive As String)
        Dim strSQL As String = "SELECT * FROM THRECEIVE LEFT OUTER JOIN XSECT ON THRECEIVE.SUPID=XSECT.BUMO " & _
            "LEFT OUTER JOIN THPO ON THRECEIVE.PONO=THPO.PONO WHERE RECNO='" & strReceive & "'"
        strSQL = strSQL & ";SELECT TPICS_ORDER,CODE,BARCODE,DESCR AS DESCRIPTION,POQTY AS [P/O_QTY],RECQTY AS RECEIVE" & _
            ",UNIT,UPRC AS PRICE,AMT AS AMOUNT FROM TDRECEIVE WHERE RECNO='" & strReceive & "' ORDER BY ROWNO"
        Dim DS As DataSet = DB.GetDataSet(strSQL)
        For Each DR As DataRow In DS.Tables(0).Rows
            dtpReceive.EditValue = New Date(CInt(Strings.Left(DR("RECDATE").ToString, 4)), CInt(Strings.Mid(DR("RECDATE").ToString, 5, 2)), CInt(Strings.Right(DR("RECDATE").ToString, 2)))
            cboPO.Text = DR("PONO").ToString
            cboDelivery.Text = DR("DlyNo").ToString
            txtSupplierID.Text = DR("SupID").ToString
            txtSupplier.Text = DR("NAME").ToString
            txtAD1.Text = DR("ADR1").ToString
            txtAD2.Text = DR("ADR2").ToString
            txtZip.Text = DR("ZIP").ToString
            txtCountry.Text = DR("COUNTRY").ToString
            txtTel.Text = DR("TEL").ToString
            txtFax.Text = DR("FAX").ToString
            txtRemark1.Text = DR("Remark1").ToString
            txtRemark2.Text = DR("Remark2").ToString
            txtRemark3.Text = DR("Remark3").ToString
            txtTotal.Text = DR("TAmt").ToString
            txtVat.Text = DR("Vat").ToString
            txtVatTotal.Text = DR("TVat").ToString
            txtGrand.Text = DR("GTotal").ToString
            txtCur.Text = DR("Curre").ToString
            txtPOType.Text = DR("POType").ToString
            txtSection.Text = DR("SectID").ToString
            optProduct.SelectedIndex = CInt(DR("Type"))
            txtRemark.Text = DR("POTitle").ToString
            Try
                picPerson.Image = Image.FromFile(Application.StartupPath & "\Images\" & DR("UPDBY").ToString & ".jpg")
            Catch ex As Exception
                'Do nothing
            End Try
        Next
        Grid.DataSource = DS.Tables(1)
    End Sub

    Private Function RunReceiveNo() As String
        Dim Eng_clinfo As New CultureInfo("en-US")
        Dim Eng_dtfinfo As DateTimeFormatInfo = Eng_clinfo.DateTimeFormat
        Dim strToday As String = Today.ToString("yyyyMM", Eng_dtfinfo)
        Dim strSQL As String = "SELECT MAX(RECNO) AS RECEIVENO FROM THRECEIVE WHERE RECNO LIKE 'FI" & strToday & "%'"
        Dim strLastNo As String = DB.ExecuteFirstValue(strSQL)

        If strLastNo.Length = 0 Then
            Return "FI" & strToday & "0001"
        Else
            Return "FI" & strToday & Format(CInt(Strings.Right(strLastNo, 4)) + 1, "0000")
        End If
    End Function

    Private Sub SaveData(ByVal strReceive As String)
        Dim strSQL As String = "SELECT COUNT(RECNO) FROM THRECEIVE WHERE RECNO='" & strReceive & "'"

        If CInt(DB.ExecuteFirstValue(strSQL)) = 0 Then
            'INSERT
            strSQL = "INSERT INTO THRECEIVE(RECNO,EDA,RECDATE,PONO,SUPID,DLYNO,SORDNO,TYPE,TQTY,TAMT,VAT,TVAT,GTOTAL" & _
                ",REMARK1,REMARK2,REMARK3,UPDBY,UPDDATE)VALUES('" & strReceive & "',0,'" & CDate(dtpReceive.EditValue).ToString("yyyyMMdd", Eng_dtfinfo) & "'" & _
                ",'" & cboPO.Text & "','" & txtSupplierID.Text & "','" & cboDelivery.Text & "','','" & optProduct.SelectedIndex & "'" & _
                "," & CDbl(GridView.Columns("RECEIVE").SummaryItem.SummaryValue) & "," & txtTotal.Text & "," & txtVat.Text & _
                "," & txtVatTotal.Text & "," & txtGrand.Text & ",N'" & txtRemark1.Text & "',N'" & txtRemark2.Text & "',N'" & txtRemark3.Text & "'" & _
                ",'" & Strings.Left(My.Computer.Name.ToString, 10) & "','" & Today.ToString("yyyyMMdd", Eng_dtfinfo) & "')"
            DB.Execute(strSQL)
            cboReceive.Properties.Items.Add(strReceive)
        Else
            'UPDATE
            strSQL = "UPDATE THRECEIVE SET RECDATE='" & CDate(dtpReceive.EditValue).ToString("yyyyMMdd", Eng_dtfinfo) & "'" & _
                ",PONO='" & cboPO.Text & "',SUPID='" & txtSupplierID.Text & "',DLYNO='" & cboDelivery.Text & "',SORDNO=''" & _
                ",TYPE='" & optProduct.SelectedIndex & "',TQTY=" & CDbl(GridView.Columns("RECEIVE").SummaryItem.SummaryValue) & _
                ",TAMT=" & txtTotal.Text & ",VAT=" & txtVat.Text & ",TVAT=" & txtVatTotal.Text & ",GTOTAL=" & txtGrand.Text & _
                ",REMARK1=N'" & txtRemark1.Text & "',REMARK2=N'" & txtRemark2.Text & "',REMARK3=N'" & txtRemark3.Text & "'" & _
                ",UPDBY='" & Strings.Left(My.Computer.Name.ToString, 10) & "',UPDDATE='" & Today.ToString("yyyyMMdd", Eng_dtfinfo) & "'" & _
                " WHERE RECNO='" & strReceive & "'"
            DB.Execute(strSQL)
        End If
        strSQL = "DELETE FROM TDRECEIVE WHERE RECNO='" & strReceive & "'"
        DB.Execute(strSQL)
        For i As Integer = 0 To GridView.DataRowCount - 1
            strSQL = "INSERT INTO TDRECEIVE(RECNO,EDA,ROWNO,PONO,TPICS_ORDER,DESCR,CODE,BARCODE,POQTY,DIFFQTY,RECQTY" & _
                ",BALQTY,UNIT,UPRC,AMT)VALUES(" & _
                "'" & strReceive & "','0'," & i & ",'" & cboPO.Text & "','" & GridView.GetRowCellDisplayText(i, "TPICS_ORDER") & "'" & _
                ",N'" & GridView.GetRowCellDisplayText(i, "DESCRIPTION") & "','" & GridView.GetRowCellDisplayText(i, "CODE") & "'" & _
                ",'" & GridView.GetRowCellDisplayText(i, "BARCODE") & "'," & GridView.GetRowCellDisplayText(i, "P/O_QTY") & _
                "," & Math.Abs(CDbl(GridView.GetRowCellDisplayText(i, "P/O_QTY")) - CDbl(GridView.GetRowCellDisplayText(i, "RECEIVE"))) & _
                "," & GridView.GetRowCellDisplayText(i, "RECEIVE") & _
                "," & Math.Abs(CDbl(GridView.GetRowCellValue(i, "P/O_QTY")) - CDbl(GridView.GetRowCellValue(i, "RECEIVE"))) & _
                ",'" & GridView.GetRowCellDisplayText(i, "UNIT") & "'," & GridView.GetRowCellDisplayText(i, "PRICE") & _
                "," & GridView.GetRowCellDisplayText(i, "AMOUNT") & ")"
            DB.Execute(strSQL)
            'Update ReceiveNo in Xslip
            strSQL = "UPDATE XSACT SET RECEIVENO='" & strReceive & "' WHERE PORDER='" & GridView.GetRowCellDisplayText(i, "TPICS_ORDER") & "' AND DELIVERYNO='" & cboDelivery.Text & "'"
            DB.Execute(strSQL)
        Next
    End Sub

    Private Sub LoadReceiveNo()
        Dim strSQL As String = "SELECT RECNO FROM THRECEIVE WHERE RECNO LIKE 'FI%'" ' & Today.ToString("yyyy", Eng_dtfinfo) & "%'"
        Dim DT As DataTable = DB.GetDataTable(strSQL)

        cboReceive.Properties.Items.Clear()
        For Each DR As DataRow In DT.Rows
            cboReceive.Properties.Items.Add(DR("RECNO").ToString)
        Next
        DT = Nothing
    End Sub

    Private Sub ChangeStatusTo(ByVal _Status As String)
        Select Case _Status
            Case "NEW"
                strStatus = "NEW"
                cmdNew.Image = My.Resources.Resource1.delete.ToBitmap
                cmdNew.Text = "Cancel"
                cboReceive.Properties.ReadOnly = True
                cboPO.Properties.ReadOnly = False
                cboDelivery.Properties.ReadOnly = False
                cmdImport.Enabled = True
                Call ClearForm(True, True, True)
            Case "EDIT"
                strStatus = "EDIT"
                cmdNew.Image = My.Resources.Resource1.file_new.ToBitmap
                cmdNew.Text = "New"
                cboReceive.Properties.ReadOnly = False
                cboPO.Properties.ReadOnly = True
                cboDelivery.Properties.ReadOnly = True
                cmdImport.Enabled = False
        End Select
    End Sub

    Private Sub CalculateTotal()
        Dim dblTotal As Double = 0.0
        For i As Integer = 0 To GridView.RowCount - 1
            dblTotal += CDbl(GridView.GetRowCellValue(i, "AMOUNT"))
        Next
        txtTotal.Text = dblTotal.ToString
        txtVatTotal.Text = CStr(dblTotal * (CDbl(txtVat.Text) / 100))
        txtGrand.Text = CStr(dblTotal + (dblTotal * (CDbl(txtVat.Text) / 100)))
    End Sub

    Private Sub SearchDeliveryNo(ByVal strPO As String)
        Dim strSQL As String = "SELECT DISTINCT DELIVERYNO FROM XSACT INNER JOIN XSLIP ON XSACT.PORDER=XSLIP.PORDER WHERE XSLIP.PO='" & strPO & "'"
        Dim DT As DataTable = DB.GetDataTable(strSQL)

        cboDelivery.Properties.Items.Clear()
        For Each DR As DataRow In DT.Rows
            cboDelivery.Properties.Items.Add(DR("DELIVERYNO").ToString)
        Next
    End Sub

    Private Function SearchItemMasterDetail(ByVal strCode As String) As String()
        Dim strSQL As String = "SELECT NAME AS BARCODE,MIXING AS DESCRIPTION FROM XHEAD WHERE CODE='" & strCode & "'"
        Dim DT As DataTable = DB.GetDataTable(strSQL)
        Dim aryTemp(1) As String

        If DT.Rows.Count = 0 Then
            aryTemp(0) = ""
            aryTemp(1) = ""
        Else
            aryTemp(0) = DT.Rows(0)("BARCODE").ToString
            aryTemp(1) = DT.Rows(0)("DESCRIPTION").ToString
        End If
        Return aryTemp
    End Function

    Private Sub CalculateAverageCost()
        Dim db1 As cDatabase = New cDatabase(TUW99)
        Dim strSQL As String

        db1.ConnectionOpen()
        For i As Integer = 0 To GridView.DataRowCount - 1
            strSQL = "UPDATE YARNCODE SET COST=(SELECT DBO.UDF_YARNCOSTCALCULATE(" & _
                GridView.GetRowCellDisplayText(i, "CODE") & _
                ",(SELECT CONVERT(CHAR(7),GETDATE(),120)+'-01')" & _
                ",(SELECT CONVERT(CHAR(10),DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,GETDATE())+1,0)),120)))) " & _
                " WHERE ID=" & GridView.GetRowCellDisplayText(i, "CODE")
            db1.Execute(strSQL)
        Next
        db1.ConnectionClose()
    End Sub

    Private Sub frmReceiveNote_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Me.Cursor = Cursors.WaitCursor
            PictureEdit1.Image = My.Resources.Resource1.tuw_logo1
            cmdImport.Image = My.Resources.Resource1.tab_center.ToBitmap
            cmdPreview.Image = My.Resources.Resource1.print_preview.ToBitmap
            cmdPrint.Image = My.Resources.Resource1.print.ToBitmap
            cmdExit.Image = My.Resources.Resource1._exit.ToBitmap
            cmdSave.Image = My.Resources.Resource1.save.ToBitmap
            cmdClear.Image = My.Resources.Resource1.clear.ToBitmap
            cmdNew.Image = My.Resources.Resource1.file_new.ToBitmap
            cmdDelete.Image = My.Resources.Resource1.delete.ToBitmap
            cmdCalculate.Image = My.Resources.Resource1.calculator.ToBitmap
            Call LoadReceiveNo()
            Call ChangeStatusTo("EDIT")
        Catch ex As Exception
            MsgBox(ex.Message.ToString, MsgBoxStyle.Critical)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub cmdImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdImport.Click
        Try
            Call ImportData(cboPO.Text, cboDelivery.Text)
            Call CalculateTotal()
        Catch ex As Exception
            MsgBox(ex.Message.ToString, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub cboPO_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboPO.KeyPress, cboDelivery.KeyPress, cboReceive.KeyPress
        If e.KeyChar = Chr(13) Then My.Computer.Keyboard.SendKeys("{TAB}")
    End Sub

    Private Sub cboPO_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboPO.LostFocus
        cboPO.Text = Strings.UCase(cboPO.Text)
        Call SearchDeliveryNo(cboPO.Text)
    End Sub

    Private Sub GridView_CustomRowCellEdit(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs) Handles GridView.CustomRowCellEdit
        If e.Column.FieldName = "DESCRIPTION" Then
            Dim rpDescription As New DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit
            e.RepositoryItem = rpDescription
        End If
    End Sub

    Private Sub GridView_ShowingEditor(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles GridView.ShowingEditor
        'e.Cancel = True
    End Sub

    Private Sub cmdSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSave.Click
        Try
            Me.Cursor = Cursors.WaitCursor
            DB.ConnectionOpen()
            DB.BeginTrans(IsolationLevel.Serializable)
            If cboReceive.Text.Length = 0 Then
                Dim strReceive As String = RunReceiveNo()
                cboReceive.Text = strReceive
                Call SaveData(strReceive)
            Else
                Call SaveData(cboReceive.Text)
            End If
            Call ChangeStatusTo("EDIT")
            DB.CommitTrans()
            MsgBox("SAVE COMPLETE", MsgBoxStyle.Information)
        Catch ex As Exception
            DB.RollbackTrans()
            If strStatus = "NEW" Then cboReceive.Text = ""
            MsgBox(ex.Message.ToString, MsgBoxStyle.Critical, "Error")
        Finally
            DB.ConnectionClose()
            Me.Cursor = Cursors.Default
        End Try
        Try
            If optProduct.SelectedIndex = 1 Then   ' เลือก yarn
                Call CalculateAverageCost()
            End If
        Catch ex As Exception
            MsgBox(ex.Message.ToString, MsgBoxStyle.Critical, "CalculateAverageCost")
        End Try
    End Sub

    Private Sub cmdNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdNew.Click
        If strStatus = "NEW" Then
            Call ChangeStatusTo("EDIT")
        Else
            Call ChangeStatusTo("NEW")
        End If
    End Sub

    Private Sub cmdClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClear.Click
        Call ClearForm(True, True, True)
    End Sub

    Private Sub cboReceive_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboReceive.SelectedIndexChanged
        Try
            Call ClearForm(True, False, False)
            Call DisplayData(cboReceive.Text)
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try
    End Sub

    Private Sub cmdPreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPreview.Click
        Dim crpReceive As New cCrystalReport(Application.StartupPath & "\Report\ReceiveNote.rpt")
        With crpReceive
            If .SetPrinter = False Then Exit Sub
            .ReportTitle = cboReceive.Text
            For i As Integer = 1 To .ReportCopy
                .ClearParameters()
                .SetParameter("Copy", i.ToString)
                If chkRevise.Checked Then
                    .SetParameter("Revise", "REVISE")
                Else
                    .SetParameter("Revise", "")
                End If
                Dim fmlText As String = "{THRECEIVE.RECNO}='" & cboReceive.Text & "'"
                .PrintReport(fmlText, False)
            Next
        End With
    End Sub

    Private Sub cmdPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPrint.Click
        Dim crpReceive As New cCrystalReport(Application.StartupPath & "\Report\ReceiveNote.rpt")
        With crpReceive
            Dim intTemp As Integer
            Try
                intTemp = Integer.Parse(InputBox("คุณต้องการพิมพ์กี่ copy", "จำนวนสำเนา", "3")) 'เปิ้ลขอไว้ให้มี 3 ก๊อบปี้อัตโนมัติ
            Catch ex As Exception
                intTemp = 1
            End Try
            If .SetPrinter() = False Then Exit Sub
            .ReportTitle = cboReceive.Text
            For i As Integer = 1 To intTemp
                .ClearParameters()
                .SetParameter("Copy", i.ToString)
                If chkRevise.Checked Then
                    .SetParameter("Revise", "REVISE")
                Else
                    .SetParameter("Revise", "")
                End If
                Dim fmlText As String = "{THRECEIVE.RECNO}='" & cboReceive.Text & "'"
                .PrintReport(fmlText, True)
            Next
        End With
    End Sub

    Private Sub cmdExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdExit.Click
        Me.Dispose()
    End Sub

    Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
        If MsgBox("คุณต้องการลบเลขที่รับ " & cboReceive.Text & " หรือไม่", MsgBoxStyle.YesNo) = MsgBoxResult.No Then Exit Sub
        If MsgBox("คุณแน่ใจจริงๆนะที่จะลบเลขที่รับ " & cboReceive.Text, MsgBoxStyle.YesNo) = MsgBoxResult.No Then Exit Sub
        If MsgBox("กด Yes อีกครั้งเพื่อยืนยันว่าคุณต้องการลบเลขที่รับ " & cboReceive.Text, MsgBoxStyle.YesNo) = MsgBoxResult.No Then Exit Sub
        Try
            DB.ConnectionOpen()
            DB.BeginTrans()
            Dim strSQL As String = "DELETE FROM THRECEIVE WHERE RECNO='" & cboReceive.Text & "'"
            DB.Execute(strSQL)
            strSQL = "DELETE FROM TDRECEIVE WHERE RECNO='" & cboReceive.Text & "'"
            DB.Execute(strSQL)
            DB.CommitTrans()
            cboReceive.Properties.Items.RemoveAt(cboReceive.Properties.Items.IndexOf(cboReceive.Text))
            Call cmdClear_Click(Nothing, Nothing)
        Catch ex As Exception
            DB.RollbackTrans()
            MsgBox(ex.Message.ToString, MsgBoxStyle.Critical)
        Finally
            DB.ConnectionClose()
        End Try
    End Sub

    Private Sub cmdCalculate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCalculate.Click
        Try
            Call CalculateTotal()
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try
    End Sub

    Private Sub GridView_ValidatingEditor(ByVal sender As Object, ByVal e As DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs) Handles GridView.ValidatingEditor
        Try
            Select Case GridView.FocusedColumn.FieldName
                Case "CODE"
                    Dim aryTemp As String() = SearchItemMasterDetail(e.Value.ToString)
                    GridView.SetRowCellValue(GridView.FocusedRowHandle, "BARCODE", aryTemp(0).ToString)
                    GridView.SetRowCellValue(GridView.FocusedRowHandle, "DESCRIPTION", aryTemp(1).ToString)
                Case "RECEIVE"
                    GridView.SetRowCellValue(GridView.FocusedRowHandle, "AMOUNT", _
                        FormatNumber(CDbl(e.Value) * CDbl(GridView.GetRowCellValue(GridView.FocusedRowHandle, "PRICE")), 2))
                Case "PRICE"
                    GridView.SetRowCellValue(GridView.FocusedRowHandle, "AMOUNT", _
                        FormatNumber(CDbl(e.Value) * CDbl(GridView.GetRowCellDisplayText(GridView.FocusedRowHandle, "RECEIVE")), 2))
            End Select
            Call CalculateTotal()
        Catch ex As Exception
            'DO NOTHING
        End Try
    End Sub

    Private Sub Grid_ProcessGridKey(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Grid.ProcessGridKey
        If e.KeyCode = Keys.Delete Then If GridView.IsEditing = False Then GridView.DeleteSelectedRows()
    End Sub

End Class