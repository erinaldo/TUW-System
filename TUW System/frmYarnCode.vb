Option Explicit On
Option Strict On

Imports System.Globalization

Public Class frmYarnCode
    Private DB As New cDatabase(TUW99)
    Private DB_TPiCS As New cDatabase(Fabric)
    Private blnEditMode As Boolean = True
    Private htWorkcenter As Hashtable
    Private Eng_clinfo As New CultureInfo("en-US")
    Private Eng_dtfinfo As DateTimeFormatInfo = Eng_clinfo.DateTimeFormat
    Private strDate As String = Now.ToString("yyMMddHHmmss", Eng_dtfinfo)
    Private strUser As String = Strings.Left(My.Computer.Name, 10)

    Private Sub ClearForm(ByVal blnInput As Boolean, ByVal blnGrid As Boolean)
        If blnInput = True Then
            For Each ctl As Control In Me.LayoutControl1.Controls
                If TypeOf (ctl) Is DevExpress.XtraEditors.ComboBoxEdit Then ctl.Text = ""
                If TypeOf (ctl) Is DevExpress.XtraEditors.TextEdit Then ctl.Text = ""
            Next
        End If
        If blnGrid = True Then
            Dim DT As New DataTable
            With DT
                .BeginInit()

                .EndInit()
            End With
        End If
    End Sub

#Region "Load"

    Private Sub LoadYarnType()
        Dim DT As DataTable = DB.GetDataTable("SELECT YARNNAME FROM YARNNAME ORDER BY YARNNAME")

        cboType.Properties.Items.Clear()
        cboType.Properties.Items.Add("")
        For Each DR As DataRow In DT.Rows
            cboType.Properties.Items.Add(DR("YARNNAME").ToString)
        Next
        DT = Nothing
    End Sub

    Private Sub LoadYarnSpecial()
        Dim DT As DataTable = DB.GetDataTable("SELECT SPECIAL FROM YARNSPECIAL ORDER BY SPECIAL")

        cboSpecial.Properties.Items.Clear()
        cboSpecial.Properties.Items.Add("")
        For Each DR As DataRow In DT.Rows
            cboSpecial.Properties.Items.Add(DR("SPECIAL").ToString)
        Next
        DT = Nothing
    End Sub

    Private Sub LoadColor()
        Dim DT As DataTable = DB.GetDataTable("SELECT COLORNAME FROM YARNCOLORCODE ORDER BY COLORNAME")

        cboColor.Properties.Items.Clear()
        cboColor.Properties.Items.Add("")
        For Each DR As DataRow In DT.Rows
            cboColor.Properties.Items.Add(DR("COLORNAME").ToString)
        Next
        DT = Nothing
    End Sub

    Private Sub LoadSupplier()
        Dim DT As DataTable = DB.GetDataTable("SELECT SUPPLIERNAME FROM YARNSUPPLIER ORDER BY SUPPLIERNAME")

        cboSupplier.Properties.Items.Clear()
        cboSupplier.Properties.Items.Add("")
        For Each DR As DataRow In DT.Rows
            cboSupplier.Properties.Items.Add(DR("SUPPLIERNAME").ToString)
        Next
        DT = Nothing
    End Sub

    Private Sub LoadMixed()
        Dim DT As DataTable = DB.GetDataTable("SELECT DISTINCT MIXED FROM YARNCODE ORDER BY MIXED")

        cboMixed.Properties.Items.Clear()
        cboMixed.Properties.Items.Add("")
        For Each DR As DataRow In DT.Rows
            cboMixed.Properties.Items.Add(DR("MIXED").ToString)
        Next
        DT = Nothing
    End Sub

    Private Sub LoadYarnno()
        Dim DT As DataTable = DB.GetDataTable("SELECT DISTINCT YARNNO FROM YARNCODE ORDER BY YARNNO")

        cboYarnno.Properties.Items.Clear()
        cboYarnno.Properties.Items.Add("")
        For Each DR As DataRow In DT.Rows
            cboYarnno.Properties.Items.Add(DR("YARNNO").ToString)
        Next
        DT = Nothing
    End Sub

    Private Sub LoadMainWorkCenter()
        Dim DT As DataTable = DB_TPiCS.GetDataTable("SELECT BUMO,NAME FROM XSECT WHERE BUNR='YARN' ORDER BY NAME")

        cboTPiCSSupplier.Properties.Items.Clear()
        htWorkcenter = New Hashtable
        For Each DR As DataRow In DT.Rows
            cboTPiCSSupplier.Properties.Items.Add(DR("NAME").ToString)
            Try
                htWorkcenter.Add(DR("NAME").ToString, DR("BUMO").ToString)
            Catch ex As Exception
            End Try
        Next
        DT = Nothing
    End Sub

#End Region

#Region "Find"

    Private Function FindType() As String
        Dim strSQL As String = "SELECT YARNCODE FROM YARNNAME WHERE YARNNAME='" & cboType.Text & "'"
        Return DB.ExecuteFirstValue(strSQL)
    End Function

    Private Function FindSpecial() As String
        Dim strSQL As String = "SELECT SPECCODE FROM YARNSPECIAL WHERE SPECIAL='" & cboSpecial.Text & "'"
        Return DB.ExecuteFirstValue(strSQL)
    End Function

    Private Function FindColor() As String
        Dim strSQL As String = "SELECT CODE FROM YARNCOLORCODE WHERE COLORNAME='" & cboColor.Text & "'"
        Return DB.ExecuteFirstValue(strSQL)
    End Function

    Private Function FindSupplier() As String
        Dim strSQL As String = "SELECT SUPPLIERCODE FROM YARNSUPPLIER WHERE SUPPLIERNAME='" & cboSupplier.Text & "'"
        Return DB.ExecuteFirstValue(strSQL)
    End Function

#End Region

    Private Sub DataToGrid()
        Dim strSQL As String = "SELECT ID,CODE,TYPE,YARNNO AS NUMBER,SPECIAL,DESCR AS DESCRIPTION,COLOR,SUPPLIER,MIXED,UNIT,RELEASE,BASE_ID " & _
            "FROM YARNCODE WHERE FLAG=0 ORDER BY CODE"
        Dim DT As DataTable = DB.GetDataTable(strSQL)

        txtCode.DataBindings.Add(New Binding("Text", DT, "CODE"))
        cboType.DataBindings.Add(New Binding("Text", DT, "TYPE"))
        cboYarnno.DataBindings.Add(New Binding("Text", DT, "NUMBER"))
        cboSpecial.DataBindings.Add(New Binding("Text", DT, "SPECIAL"))
        txtDescription.DataBindings.Add(New Binding("Text", DT, "DESCRIPTION"))
        cboColor.DataBindings.Add(New Binding("Text", DT, "COLOR"))
        cboSupplier.DataBindings.Add(New Binding("Text", DT, "SUPPLIER"))
        cboMixed.DataBindings.Add(New Binding("Text", DT, "MIXED"))
        cboUnit.DataBindings.Add(New Binding("Text", DT, "UNIT"))
        txtRelease.DataBindings.Add(New Binding("Text", DT, "RELEASE"))
        txtBaseID.DataBindings.Add(New Binding("Text", DT, "BASE_ID"))
        Grid.DataSource = DT

        With GridView
            .OptionsView.EnableAppearanceEvenRow = True
            .OptionsView.EnableAppearanceOddRow = True
            .OptionsView.ColumnAutoWidth = False
            .BestFitColumns()
        End With
        DT = Nothing
    End Sub

#Region "Save"

    Private Sub CheckSaveData()
        If cboType.Text.Length = 0 Then Throw New Exception("Not found Yarn Type. Please Add Yarn Type.")
        'If cboSpecial.Text.Length = 0 Then Throw New Exception("Not found Special. Please Add Special.")
        If cboMixed.Text.Length = 0 Then Throw New Exception("Please Input Mixed.")
        If cboYarnno.Text.Length = 0 Then Throw New Exception("Please Input Yarn Number.")
        If cboSupplier.Text.Length = 0 Then Throw New Exception("Please Input Supplier.")
        If txtSupplier.Text.Length = 0 Then Throw New Exception("Please Input Main Work center")
        If txtRelease.Text.Length = 0 Then Throw New Exception("Please Input Release Period")
    End Sub

    Private Sub SaveData()

        Dim strSQL As String
        Dim strID As String = ""
        '-----------------------------------------------------------------------------------------------------------------------------------------------

        '------------------------------------------------------------------------------------------------------------------------------------------------
        If blnEditMode = True Then 'Update current record
            Dim zType As String = FindType()
            Dim zSpecial As String = FindSpecial()
            Dim zColor As String = FindColor()
            Dim zSupplier As String = FindSupplier()
            'สร้างสูตรเส้นด้ายจากองค์ประกอบต่างๆ
            If zSpecial <> "" Then
                If zColor <> "" Then
                    txtCode.Text = zType & cboYarnno.Text & "-" & zSpecial & "-" & zColor & "-" & zSupplier
                Else
                    txtCode.Text = zType & cboYarnno.Text & "-" & zSpecial & "-" & zSupplier
                End If
            Else
                If zColor <> "" Then
                    txtCode.Text = zType & cboYarnno.Text & "-" & zColor & "-" & zSupplier
                Else
                    txtCode.Text = zType & cboYarnno.Text & "-" & zSupplier
                End If
            End If
            strSQL = "UPDATE YARNCODE SET  TYPE='" & cboType.Text & "',MIXED='" & cboMixed.Text & "'" & _
                ",YARNNO='" & cboYarnno.Text & "',COLOR='" & cboColor.Text & "'" & _
                ",SPECIAL='" & cboSpecial.Text & "',UNIT='" & cboUnit.Text & "'" & _
                ",SUPPLIER='" & cboSupplier.Text & "',DESCR='" & txtDescription.Text & "'" & _
                ",CODE='" & txtCode.Text & "',RELEASE=" & txtRelease.Text & _
                ",BASE_ID='" & txtBaseID.Text & "'" & _
                " WHERE (FLAG = 0) AND ID='" & GridView.GetRowCellDisplayText(GridView.FocusedRowHandle, "ID") & "'"
            DB.Execute(strSQL)
            'update to tpics
            strSQL = "UPDATE XHEAD SET NAME='" & txtCode.Text & "'" & _
                ",MAINBUMO='" & txtSupplier.Text & "'" & _
                ",TANI1='KGS',BARCODE='" & txtCode.Text & "'" & _
                ",INPUTDATE='" & strDate & "',INPUTUSER='" & strUser & "'" & _
                ",MIXING='" & cboMixed.Text & "'" & _
                " WHERE CODE='" & Strings.Right("00000" & GridView.GetRowCellDisplayText(GridView.FocusedRowHandle, "ID"), 5) & "'"
            DB_TPiCS.Execute(strSQL)
            strSQL = "UPDATE XITEM SET BUMO='" & txtSupplier.Text & "'" & _
                ",VENDOR='" & txtSupplier.Text & "'" & _
                ",BARCODE='" & txtCode.Text & "'" & _
                ",INPUTDATE='" & strDate & "',INPUTUSER='" & strUser & "'" & _
                " WHERE CODE='" & Strings.Right("00000" & GridView.GetRowCellDisplayText(GridView.FocusedRowHandle, "ID"), 5) & "'"
            DB_TPiCS.Execute(strSQL)
        Else  'Insert new reccord
            strSQL = "SELECT ID,CODE,SPECIAL FROM YARNCODE WHERE TYPE='" & cboType.Text & "' AND MIXED='" & cboMixed.Text & "'" & _
                                    "AND YARNNO='" & cboYarnno.Text & "' AND COLOR='" & cboColor.Text & "' AND SUPPLIER ='" & cboSupplier.Text & "'" & _
                                    "AND DESCR='" & txtDescription.Text & "'"
            Dim DT As DataTable = DB.GetDataTable(strSQL)
            If DT.Rows.Count > 0 Then 'ถ้าพบว่าเส้นด้ายชนิดนี้มีอยู่ในฐานข้อมูล(โดยดูจากส่วนประกอบต่างๆ)
                If Equals(DT.Rows(0)("SPECIAL").ToString, cboSpecial.Text) Then
                    Throw New Exception("มีเส้นด้ายชนิดนี้อยู่แล้ว")
                Else  'ถ้าองค์ประกอบของเส้นด้ายเหมือนกันแต่ต่างกันตรง special
                    If MsgBox("มีเส้นด้ายชนิดนี้อยู่แล้วแต่ Special ต่างกัน คุณต้องการ Save หรือไม่", MsgBoxStyle.YesNo, "Caution") = MsgBoxResult.No Then Throw New Exception("ยกเลิกการ Save")
                End If
            End If
            Dim zType As String = FindType()
            Dim zSpecial As String = FindSpecial()
            Dim zColor As String = FindColor()
            Dim zSupplier As String = FindSupplier()
            'สร้างสูตรเส้นด้ายจากองค์ประกอบต่างๆ
            If zSpecial <> "" Then
                If zColor <> "" Then
                    txtCode.Text = zType & cboYarnno.Text & "-" & zSpecial & "-" & zColor & "-" & zSupplier
                Else
                    txtCode.Text = zType & cboYarnno.Text & "-" & zSpecial & "-" & zSupplier
                End If
            Else
                If zColor <> "" Then
                    txtCode.Text = zType & cboYarnno.Text & "-" & zColor & "-" & zSupplier
                Else
                    txtCode.Text = zType & cboYarnno.Text & "-" & zSupplier
                End If
            End If

            strSQL = "INSERT INTO YARNCODE(CODE,TYPE,MIXED,YARNNO,COLOR,SPECIAL,UNIT,SUPPLIER,DESCR,RELEASE,BASE_ID)VALUES(" & _
                "'" & txtCode.Text & "','" & cboType.Text & "','" & cboMixed.Text & "','" & cboYarnno.Text & "'" & _
                ",'" & cboColor.Text & "','" & cboSpecial.Text & "','" & cboUnit.Text & "','" & cboSupplier.Text & "'" & _
                ",'" & txtDescription.Text & "'," & txtRelease.Text & ",'" & txtBaseID.Text & "')"
            DB.Execute(strSQL)
            'หาค่า ID จากเรคคอร์ดที่เพิ่งบันทึกลงไปเพื่อนำไปตั้งเป็น code ใน tpics
            strSQL = "SELECT ID FROM YARNCODE WHERE CODE='" & txtCode.Text & "'"
            strID = DB.ExecuteFirstValue(strSQL)
            GridView.SetRowCellValue(GridView.FocusedRowHandle, "ID", strID)

            'Insert to TPiCS
            strSQL = "SELECT COUNT(CODE) FROM XHEAD WHERE CODE='" & Strings.Right("00000" & strID, 5) & "'"
            If DB_TPiCS.ExecuteFirstValue(strSQL) = "0" Then
                strSQL = "INSERT INTO XHEAD(CODE,NAME,MAINBUMO,TANI1,BARCODE,INPUTDATE,INPUTUSER,MIXING)VALUES(" & _
                    "'" & Strings.Right("00000" & strID, 5) & "'" & _
                    ",'" & txtCode.Text & "'" & _
                    ",'" & txtSupplier.Text & "'" & _
                    ",'KGS','" & txtCode.Text & "'" & _
                    ",'" & strDate & "','" & strUser & "','" & cboMixed.Text & "')"
                DB_TPiCS.Execute(strSQL)
                strSQL = "INSERT INTO XITEM(CODE,BUMO,VENDOR,FIXLEVEL,DKAKU,KAKU,LEAD,KOUKI,HOKAN,LOTS,BARCODE,COLOR,PKET,HIKIKU,KURIAGE,CONT,INPUTDATE,INPUTUSER)VALUES(" & _
                    "'" & Strings.Right("00000" & strID, 5) & "'" & _
                    ",'" & txtSupplier.Text & "'" & _
                    ",'" & txtSupplier.Text & "'" & _
                    "," & "0" & _
                    "," & txtRelease.Text & _
                    "," & txtRelease.Text & _
                    "," & "1" & _
                    "," & "0" & _
                    ",'" & "ST02" & "'" & _
                    "," & "1" & _
                    ",'" & txtCode.Text & "'" & _
                    ",'" & "" & "'" & _
                    "," & "2" & _
                    "," & "0" & _
                    "," & "0" & _
                    ",'" & "" & "'" & _
                    ",'" & strDate & "','" & strUser & "')"
                DB_TPiCS.Execute(strSQL)
            End If
        End If
    End Sub

#End Region

    Private Sub frmYarnCode_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        cmdNew.Image = My.Resources.Resource1.file_new.ToBitmap
        cmdSave.Image = My.Resources.Resource1.save.ToBitmap
        cmdExit.Image = My.Resources.Resource1._exit.ToBitmap

        Me.Cursor = Cursors.WaitCursor
        Call LoadYarnType()
        Call LoadYarnSpecial()
        Call LoadColor()
        Call LoadSupplier()
        Call LoadMixed()
        Call LoadYarnno()
        Call LoadMainWorkCenter()
        Call DataToGrid()
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub cmdExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdExit.Click
        Me.Dispose()
    End Sub

    Private Sub cmdClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Call ClearForm(True, False)
    End Sub

    Private Sub cboType_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboType.KeyPress, cboMixed.KeyPress, cboSpecial.KeyPress, cboSupplier.KeyPress, cboType.KeyPress, cboYarnno.KeyPress
        If e.KeyChar = Chr(13) Then My.Computer.Keyboard.SendKeys("{TAB}")
    End Sub

    Private Sub txtDescription_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtDescription.KeyPress
        If e.KeyChar = Chr(13) Then My.Computer.Keyboard.SendKeys("{TAB}")
    End Sub

    Private Sub cmdType_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdType.Click
        Dim frm As New frmYarnCode_Sub(frmYarnCode_Sub.eType.YarnType, Me.Handle)
        With frm
            .Text = "Yarn Name..."
            .lbl1.Text = "Yarn Type :"
            .Show()
        End With
    End Sub

    Private Sub cmdSupplier_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSupplier.Click
        Dim frm As New frmYarnCode_Sub(frmYarnCode_Sub.eType.YarnSupplier, Me.Handle)
        With frm
            .Text = "Yarn Supplier..."
            .lbl1.Text = "Supplier :"
            .Show()
        End With
    End Sub

    Private Sub cmdSpecial_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSpecial.Click
        Dim frm As New frmYarnCode_Sub(frmYarnCode_Sub.eType.YarnSpecial, Me.Handle)
        With frm
            .Text = "Yarn Special..."
            .lbl1.Text = "Yarn Special :"
            .Show()
        End With
    End Sub

    Private Sub cmdColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdColor.Click
        Dim frm As New frmYarnCode_Sub(frmYarnCode_Sub.eType.YarnColor, Me.Handle)
        With frm
            .Text = "Yarn Color..."
            .lbl1.Text = "Color :"
            .Show()
        End With
    End Sub

    Private Sub cmdSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSave.Click
        Me.Cursor = Cursors.WaitCursor
        Try
            DB.ConnectionOpen()
            DB.BeginTrans()
            DB_TPiCS.ConnectionOpen()
            DB_TPiCS.BeginTrans()
            Call CheckSaveData()
            Call SaveData()
            DB.CommitTrans()
            DB_TPiCS.CommitTrans()
            GridView.UpdateCurrentRow()
            MsgBox("Save Complete", MsgBoxStyle.Information)
        Catch ex As Exception
            DB.RollbackTrans()
            DB_TPiCS.RollbackTrans()
            GridView.CancelUpdateCurrentRow()
            MsgBox(ex.Message.ToString, MsgBoxStyle.Critical)
        Finally
            DB.ConnectionClose()
            DB_TPiCS.ConnectionClose()
            Me.Cursor = Cursors.Default

            blnEditMode = True
            cmdNew.Text = "New"
            cmdNew.Image = My.Resources.Resource1.file_new.ToBitmap
            GridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None
            Grid.Enabled = True
        End Try
    End Sub

    Private Sub GridView_ShowingEditor(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles GridView.ShowingEditor
        e.Cancel = True
    End Sub

    Private Sub cmdNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdNew.Click
        If blnEditMode = True Then 'ทำงานเมื่อกดปุ่ม new
            blnEditMode = False
            cmdNew.Text = "Cancel New"
            cmdNew.Image = My.Resources.Resource1.delete.ToBitmap
            GridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top
            GridView.AddNewRow()
            Grid.Enabled = False

        Else  'ทำงานเมื่อกดปุ่ม cancel new
            blnEditMode = True
            cmdNew.Text = "New"
            cmdNew.Image = My.Resources.Resource1.file_new.ToBitmap
            GridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None
            GridView.CancelUpdateCurrentRow()
            Grid.Enabled = True
        End If

    End Sub

    Private Sub cboTPiCSSupplier_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboTPiCSSupplier.SelectedIndexChanged
        txtSupplier.Text = htWorkcenter(cboTPiCSSupplier.Text).ToString
    End Sub

End Class