Option Explicit On
Option Compare Text
Option Strict On
Public Class frmPO_New

    Private Function CheckDuplicatePORDER(ByVal strPORDER As String) As Boolean
        For i As Integer = 0 To frmPO.GridView.DataRowCount - 1
            If Equals(strPORDER, frmPO.GridView.GetRowCellDisplayText(i, "TPICS_ORDER")) Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub frmPO_New_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        cmdImport.Image = My.Resources.Resource1.tab_center.ToBitmap
        cmdClose.Image = My.Resources.Resource1._exit.ToBitmap
        Me.Location = frmMDIParent.Location
        Dim DB As New cDatabase(Fabric)
        Dim strSQL As String = ""
        Select Case frmPO.cboRemark.Text
            Case "RAW YARN"
                'strSQL = "SELECT PORDER,XSLIP.CODE,NAME AS BARCODE,MIXING,KVOL AS QTY,TANI1 AS UNIT" & _
                '    ",PRICE,BUMO FROM XSLIP INNER JOIN XHEAD ON XSLIP.CODE=XHEAD.CODE WHERE (PORDER LIKE 'XX%') "
                strSQL = "SELECT XSLIP.PORDER, XSLIP.CODE, XHEAD.NAME AS BARCODE, XHEAD.MIXING" & _
                    ",CASE WHEN XITEM.KANZANZA=0 THEN XSLIP.KVOL " & _
                    "WHEN XITEM.KANZANZA=1 THEN XSLIP.KVOL*XITEM.KANZANK " & _
                    "WHEN XITEM.KANZANZA=2 THEN XSLIP.KVOL/XITEM.KANZANK END AS QTY " & _
                    ",CASE WHEN XITEM.KANZANZA=0 THEN XHEAD.TANI1	ELSE XITEM.TANI2 END AS UNIT " & _
                    ",XSLIP.PRICE, XSLIP.BUMO " & _
                    "FROM XSLIP INNER JOIN XHEAD ON XSLIP.CODE = XHEAD.CODE INNER JOIN XITEM ON XSLIP.CODE = XITEM.CODE AND XSLIP.BUMO=XITEM.BUMO " & _
                    "WHERE (XSLIP.PORDER LIKE 'XX%') "
            Case "DYEING FEE"
                'strSQL = "SELECT PORDER,XSLIP.CODE,NAME AS BARCODE,MIXING,KVOL AS QTY,TANI1 AS UNIT" & _
                '    ",PRICE,BUMO FROM XSLIP INNER JOIN XHEAD ON XSLIP.CODE=XHEAD.CODE WHERE (PORDER LIKE 'FW%') AND (VENDOR LIKE 'DYE%') "
                strSQL = "SELECT XSLIP.PORDER, XSLIP.CODE, XHEAD.NAME AS BARCODE, XHEAD.MIXING" & _
                    ",CASE WHEN XITEM.KANZANZA=0 THEN XSLIP.KVOL " & _
                    "WHEN XITEM.KANZANZA=1 THEN XSLIP.KVOL*XITEM.KANZANK " & _
                    "WHEN XITEM.KANZANZA=2 THEN XSLIP.KVOL/XITEM.KANZANK END AS QTY " & _
                    ",CASE WHEN XITEM.KANZANZA=0 THEN XHEAD.TANI1	ELSE XITEM.TANI2 END AS UNIT " & _
                    ",XSLIP.PRICE, XSLIP.BUMO " & _
                    "FROM XSLIP INNER JOIN XHEAD ON XSLIP.CODE = XHEAD.CODE INNER JOIN XITEM ON XSLIP.CODE = XITEM.CODE AND XSLIP.BUMO=XITEM.BUMO  " & _
                    "WHERE (XSLIP.PORDER LIKE 'FW%') AND (XSLIP.VENDOR LIKE 'DYE%') "
            Case "KNITTING FEE"
                'strSQL = "SELECT PORDER,XSLIP.CODE,NAME AS BARCODE,MIXING,KVOL AS QTY,TANI1 AS UNIT" & _
                '    ",PRICE,BUMO FROM XSLIP INNER JOIN XHEAD ON XSLIP.CODE=XHEAD.CODE WHERE (PORDER LIKE 'FW%') AND (VENDOR LIKE 'KNT%') "
                strSQL = "SELECT XSLIP.PORDER, XSLIP.CODE, XHEAD.NAME AS BARCODE, XHEAD.MIXING" & _
                    ",CASE WHEN XITEM.KANZANZA=0 THEN XSLIP.KVOL " & _
                    "WHEN XITEM.KANZANZA=1 THEN XSLIP.KVOL*XITEM.KANZANK " & _
                    "WHEN XITEM.KANZANZA=2 THEN XSLIP.KVOL/XITEM.KANZANK END AS QTY " & _
                    ",CASE WHEN XITEM.KANZANZA=0 THEN XHEAD.TANI1	ELSE XITEM.TANI2 END AS UNIT " & _
                    ",XSLIP.PRICE, XSLIP.BUMO " & _
                    "FROM XSLIP INNER JOIN XHEAD ON XSLIP.CODE = XHEAD.CODE INNER JOIN XITEM ON XSLIP.CODE = XITEM.CODE AND XSLIP.BUMO=XITEM.BUMO  " & _
                    "WHERE (XSLIP.PORDER LIKE 'FW%') AND (XSLIP.VENDOR LIKE 'KNT%') "
            Case "SOAPING"
        End Select
        strSQL = strSQL & "AND (TJITU=0) AND (PO='')"
        Dim DT As DataTable = DB.GetDataTable(strSQL)

        With DT
            Dim DC As DataColumn
            DC = New DataColumn
            DC.ColumnName = "Check"
            DC.DataType = GetType(Boolean)
            DC.ReadOnly = False
            DC.DefaultValue = False
            .Columns.Add(DC)
            .Columns.Add("AMOUNT", GetType(Double), "QTY*PRICE")
        End With
        Grid.DataSource = DT
        With GridView
            .Columns("BUMO").VisibleIndex = 10
            .Columns("Check").VisibleIndex = 0
            .Columns("QTY").SummaryItem.FieldName = "QTY"
            .Columns("QTY").SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}")
            .OptionsView.EnableAppearanceEvenRow = True
            .OptionsView.EnableAppearanceOddRow = True
            .OptionsView.ColumnAutoWidth = False
            .BestFitColumns()
        End With
    End Sub

    Private Sub GridView_RowStyle(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs) Handles GridView.RowStyle
        If CBool(GridView.GetRowCellValue(e.RowHandle, "Check")) = True Then
            e.Appearance.Font = New Font(e.Appearance.Font, FontStyle.Strikeout)
            e.Appearance.ForeColor = Color.Gray
        End If
    End Sub

    Private Sub cmdImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdImport.Click
        For i As Integer = 0 To GridView.RowCount - 1
            If CBool(GridView.GetRowCellValue(i, "Check")) = False Then Continue For
            If CheckDuplicatePORDER(GridView.GetRowCellDisplayText(i, "PORDER")) Then Continue For
            With frmPO.GridView
                .AddNewRow()
                .SetRowCellValue(.FocusedRowHandle, "TPICS_ORDER", GridView.GetRowCellDisplayText(i, "PORDER"))
                If frmPO.cboRemark.Text = "DYEING FEE" Then
                    .SetRowCellValue(.FocusedRowHandle, "CODE", Strings.Right(GridView.GetRowCellDisplayText(i, "CODE"), GridView.GetRowCellDisplayText(i, "CODE").Length - 9)) ',RIGHT(XSLIP.CODE,LEN(XSLIP.CODE)-9) AS CODE
                Else
                    .SetRowCellValue(.FocusedRowHandle, "CODE", GridView.GetRowCellDisplayText(i, "CODE"))
                End If
                .SetRowCellValue(.FocusedRowHandle, "BARCODE", GridView.GetRowCellDisplayText(i, "BARCODE"))
                .SetRowCellValue(.FocusedRowHandle, "DESCRIPTION", GridView.GetRowCellDisplayText(i, "MIXING"))
                .SetRowCellValue(.FocusedRowHandle, "QTY", GridView.GetRowCellDisplayText(i, "QTY"))
                .SetRowCellValue(.FocusedRowHandle, "UNIT", GridView.GetRowCellDisplayText(i, "UNIT"))
                .SetRowCellValue(.FocusedRowHandle, "PRICE", GridView.GetRowCellDisplayText(i, "PRICE"))
                .SetRowCellValue(.FocusedRowHandle, "AMOUNT", GridView.GetRowCellDisplayText(i, "AMOUNT"))
                .SetRowCellValue(.FocusedRowHandle, "BUMO", GridView.GetRowCellDisplayText(i, "BUMO"))
                .UpdateCurrentRow()
            End With
        Next
        If frmPO.cboRemark.Text = "RAW YARN" Then frmPO.LoadSupplierName_Detail(GridView.GetRowCellDisplayText(GridView.FocusedRowHandle, "BUMO"), )
    End Sub

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        Me.Dispose()
    End Sub
End Class