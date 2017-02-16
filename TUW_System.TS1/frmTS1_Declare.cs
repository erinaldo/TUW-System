using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;
using System.Globalization;
using System.Data;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using myClass;

namespace TUW_System.TS1
{
    public partial class frmTS1_Declare : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        cDatabase db2;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        private RepositoryItemComboBox rpSize;
		private RepositoryItemComboBox rpColor;
		private RepositoryItemComboBox rpFabric;
		private string strFixCol; //ชื่อของคอลัมน์ที่มีการทำ freeze อยู่
        private object[] myArray;

        private bool _save;//เก็บสถานะว่าผู้ใช้สามารถเซฟ(export ข้อมูลไปยังทิปิก)ได้หรือไม่
        internal bool SaveStatus
        {
            set { _save = value; }
        }

        private string _connectionString;
        private string _connectionString2;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        public string ConnectionString2
        {
            set { _connectionString2 = value; }
        }

        public frmTS1_Declare()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            txtModel.Text = "";
            cboModel.Text = "";
            txtName.Text = "";
            memRemark.Text = "";
            ClearGrid(fgSize);
            ClearGrid(fgColor);
            ClearGrid(fgFabric);
            ClearGrid(fg);
            ClearGrid(tb);
        }
        public void RetriveData()
        {
            DataSet DS;
            string strSQL;

            try
            {
                strSQL = "SELECT Model,Name,Division,Style,Remark FROM TpicsModel WHERE Model = \'" + cboModel.Text + "\'";
                strSQL = strSQL + ";" + "SELECT Name FROM TPicsSize WHERE Model = \'" + cboModel.Text + "\' ORDER BY ID";
                strSQL = strSQL + ";" + "SELECT Name,FullName FROM TPicsColor WHERE Model = \'" + cboModel.Text + "\' ORDER BY ID";
                strSQL = strSQL + ";" + "SELECT Name,Code,WC,ColorID FROM TPicsFabric WHERE Model = \'" + cboModel.Text + "\' ORDER BY ID";
                strSQL = strSQL + ";" + "SELECT Code,ColorID,Size,Qty FROM TpicsFabricDetail WHERE Model = \'" + cboModel.Text + "\' ORDER BY ID";
                strSQL = strSQL + ";" + " SELECT TpicsAccessory.AccCode,AccName,AccOf,AccType,Remark1,Remark2,Qty,QtyDiv,Unit,TPicsSupplier.Code , " + " Name FROM TpicsAccessory INNER JOIN TpicsAccessoryDetail ON TpicsAccessory.AccCode = TpicsAccessoryDetail.AccCode " + " Inner Join TPicsSupplier ON TpicsAccessoryDetail.SupCode = TPicsSupplier.Code" + " WHERE (Model = N\'" + cboModel.Text + "\')";
                strSQL = strSQL + ";" + "SELECT ITEMCODE,NAME,WCCODE,WCNAME,SUPCODE,SUPNAME,STRGCODE,STRGNAME,UNIT,SEWORPACK,DEPENDENT,RELEASE" + ",FIX,MFG,DELIVERY,DISPATCHCLASS,ROUNDUP,DECIMALPOINT,CLASSIFICATION,STYLE,SIZE,COLOR FROM TpicsMaster " + "WHERE Model = \'" + cboModel.Text + "\' ORDER BY ID";
                strSQL = strSQL + ";" + "SELECT CODE AS PARENT,KCODE AS CHILD,SIYOU AS QTY,SIYOUW AS QTYDIV FROM TPICSBOM WHERE MODEL = \'" + cboModel.Text + "\'";
                DS = db.GetDataSet(strSQL);
                DS.Tables[0].TableName = "TPICSMODEL";
                DS.Tables[1].TableName = "TPICSSIZE";
                DS.Tables[2].TableName = "TPICSCOLOR";
                DS.Tables[3].TableName = "TPICSFABRIC";
                DS.Tables[4].TableName = "TPICSFABRICDETAIL";
                DS.Tables[5].TableName = "TPICSACCESSORY";
                DS.Tables[6].TableName = "TPICSMASTER";
                DS.Tables[7].TableName = "TPICSBOM";
                //model-----------------------------------------------------------------------------
                foreach (DataRow dr in DS.Tables["TPICSMODEL"].Rows)
                {
                    txtModel.Text = dr["Model"].ToString();
                    txtName.Text = dr["Name"].ToString();
                    if (dr["Division"].ToString() == "Sales1")
                    {
                        cboDivision.SelectedIndex = 0;
                    }
                    else if (dr["Division"].ToString() == "Sales2")
                    {
                        cboDivision.SelectedIndex = 1;
                    }
                    else if (dr["Division"].ToString() == "Sales3")
                    {
                        cboDivision.SelectedIndex = 2;
                    }
                    else
                    {
                        cboDivision.SelectedIndex = 3;
                    }
                    txtStyle.Text = dr["Style"].ToString();
                    memRemark.Text = dr["Remark"].ToString();
                }
                //size---------------------------------------------------------------------------------
                GridSize.DataSource = DS.Tables["TPICSSIZE"];
                //GridSize.RepositoryItems.Add(rpSize)
                //fgSize.Columns("Name").ColumnEdit = rpSize
                //color-------------------------------------------------------------------------------
                GridColor.DataSource = DS.Tables["TPICSCOLOR"];
                //GridColor.RepositoryItems.Add(rpColor)
                //fgColor.Columns("Name").ColumnEdit = rpColor
                //fabric------------------------------------------------------------------------------
                foreach (DataRow dr in DS.Tables["TPICSSIZE"].Rows)
                {
                    DS.Tables["TPICSFABRIC"].Columns.Add((string)(dr["Name"].ToString()), typeof(string)); //add column size to fabric table
                }
                foreach (DataRow dr in DS.Tables["TPICSFABRICDETAIL"].Rows)
                {
                    foreach (DataRow dr2 in DS.Tables["TPICSFABRIC"].Rows)
                    {
                        if (Equals(dr["Code"], dr2["Code"]) && Equals(dr["ColorID"],dr2["ColorID"]))
                        {
                            dr2[dr["Size"].ToString()] = dr["Qty"].ToString();
                        }
                    }
                }
                GridFabric.DataSource = DS.Tables["TPICSFABRIC"];
                GridFabric.MainView.PopulateColumns();
                GridFabric.RepositoryItems.Add(rpFabric);
                fgFabric.Columns["Name"].ColumnEdit = rpFabric;
                fgFabric.OptionsView.ColumnAutoWidth = false;
                fgFabric.BestFitColumns();
                //master---------------------------------------------------------------------------
                DS.Tables["TPICSMASTER"].Columns.Add("STATUS", typeof(bool));
                Gridfg.DataSource = DS.Tables["TPICSMASTER"];
                fg.OptionsView.ColumnAutoWidth = false;
                fg.BestFitColumns();
                //bom------------------------------------------------------------------------------
                DS.Tables["TPICSBOM"].Columns.Add("STATUS", typeof(bool));
                Gridtb.DataSource = DS.Tables["TPICSBOM"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ExportExcel()
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv;
            if (XtraTabControl1.SelectedTabPageIndex == 0)
            {
                gv = fg;
            }
            else
            {
                gv = tb;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Excel file(*.xls)|*.xls";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                gv.ExportToXls(dlg.FileName);
            }
        }

        //private enum eFabric
        //{
        //    FabricCode,
        //    Code,
        //    WC,
        //    Common,
        //    Screen,
        //    Print,
        //    Color
        //}
        //private enum eItem
        //{
        //    ItemCode,
        //    Name,
        //    WC,
        //    WCName,
        //    Sup,
        //    SupName,
        //    StrgWC,
        //    StrgWCName,
        //    Unit,
        //    SewOrPack,
        //    Dependent,
        //    Release,
        //    Fix,
        //    Mfg,
        //    Delivery,
        //    DispatchClass,
        //    RoundUp,
        //    DecimalPoint,
        //    Classification,
        //    Style,
        //    Size,
        //    Color,
        //    Status
        //}
        //private enum eBom
        //{
        //    Parent,
        //    Children,
        //    Qty,
        //    QtyDiv,
        //    Status
        //}
		
		#region Grid
		
		private void ClearGrid(DevExpress.XtraGrid.Views.Grid.GridView Grid)
		{
			for (int i = Grid.DataRowCount - 1; i >= 0; i--)
			{
				Grid.DeleteRow(i);
			}
		}
		
		private void PrepareGrid()
		{
			DataTable dtFG;
			DataTable dtTB;
			
			dtFG = new DataTable();
			dtFG.BeginInit();
			dtFG.Columns.Add("ITEMCODE", typeof(string));
			dtFG.Columns.Add("NAME", typeof(string));
			dtFG.Columns.Add("WCCODE", typeof(string));
			dtFG.Columns.Add("WCNAME", typeof(string));
			dtFG.Columns.Add("SUPCODE", typeof(string));
			dtFG.Columns.Add("SUPNAME", typeof(string));
			dtFG.Columns.Add("STRGCODE", typeof(string));
			dtFG.Columns.Add("STRGNAME", typeof(string));
			dtFG.Columns.Add("UNIT", typeof(string));
			dtFG.Columns.Add("SEWORPACK", typeof(string));
			dtFG.Columns.Add("DEPENDENT", typeof(byte));
			dtFG.Columns.Add("RELEASE", typeof(short));
			dtFG.Columns.Add("FIX", typeof(short));
			dtFG.Columns.Add("MFG", typeof(short));
			dtFG.Columns.Add("DELIVERY", typeof(short));
			dtFG.Columns.Add("DISPATCHCLASS", typeof(byte));
			dtFG.Columns.Add("ROUNDUP", typeof(byte));
			dtFG.Columns.Add("DECIMALPOINT", typeof(byte));
			dtFG.Columns.Add("CLASSIFICATION", typeof(string));
			dtFG.Columns.Add("STYLE", typeof(string));
			dtFG.Columns.Add("SIZE", typeof(string));
			dtFG.Columns.Add("COLOR", typeof(string));
			dtFG.Columns.Add("STATUS", typeof(bool));
			dtFG.EndInit();
			Gridfg.DataSource = dtFG;
			dtTB = new DataTable();
			dtTB.BeginInit();
			dtTB.Columns.Add("PARENT", typeof(string));
			dtTB.Columns.Add("CHILD", typeof(string));
			dtTB.Columns.Add("QTY", typeof(double));
			dtTB.Columns.Add("QTYDIV", typeof(double));
			dtTB.Columns.Add("STATUS", typeof(bool));
			dtTB.EndInit();
			Gridtb.DataSource = dtTB;
		}
		
		#endregion
		
		#region Initialize Form
		
		private void LoadRepositoryItem()
		{
			string strSQL;
			DataSet DS;
			
			strSQL = "SELECT DISTINCT Model FROM TpicsModel ORDER BY Model" + ";" + "SELECT DISTINCT Name FROM TpicsSize ORDER BY Name" + ";" + "SELECT DISTINCT Name FROM TpicsColor ORDER BY Name" + ";" + "SELECT DISTINCT Name FROM TpicsFabric Order by Name";
			DS = db.GetDataSet(strSQL);
			//Model
			foreach (DataRow dr in DS.Tables[0].Rows)
			{
				cboModel.Properties.Items.Add(dr["Model"].ToString());
			}
			//Size
			rpSize = new RepositoryItemComboBox();
			foreach (DataRow dr in DS.Tables[1].Rows)
			{
				rpSize.Items.Add(dr["Name"].ToString());
			}
			//Color
			rpColor = new RepositoryItemComboBox();
			foreach (DataRow dr in DS.Tables[2].Rows)
			{
				rpColor.Items.Add(dr["Name"].ToString());
			}
			//Fabric
			rpFabric = new RepositoryItemComboBox();
			foreach (DataRow dr in DS.Tables[3].Rows)
			{
				rpFabric.Items.Add(dr["Name"].ToString());
			}
		}
		
		private void MapRepositoryItem(DevExpress.XtraGrid.GridControl Grid, DevExpress.XtraGrid.Views.Grid.GridView GridView, string strFieldName, RepositoryItemComboBox rp)
		{
			DataTable DT = new DataTable();
			DT.Columns.Add(strFieldName, typeof(string));
			Grid.DataSource = DT;
			GridView.Columns[strFieldName].ColumnEdit = rp;
		}
		
		//Private Sub LoadModel()
		//    Dim Rs As ADODB.Recordset = New ADODB.Recordset
		//    Dim cmdSql As String = "SELECT DISTINCT Model FROM TpicsModel ORDER BY Model"
		
		//    cboModel.Properties.Items.Clear()
		//    Rs.Open(cmdSql, GDSN, CursorTypeEnum.adOpenForwardOnly, LockTypeEnum.adLockReadOnly)
		//    '=======================================================================================
		//    frmMDIParent.BarStaticItem.Caption = "LoadModel " & Rs.RecordCount & " Rows."
		//    frmMDIParent.Progressbar.Visibility = DevExpress.XtraBars.BarItemVisibility.Always
		//    frmMDIParent.Progressbar.EditValue = 0
		//    '=======================================================================================
		//    Do While Not Rs.EOF
		//        cboModel.Properties.Items.Add(Rs.Fields("Model").Value)
		//        Rs.MoveNext()
		//        '=============================================================
		//        'frmMDIParent.Progressbar.EditValue = (cboModel.Properties.Items.Count / Rs.RecordCount) * 100
		//        'frmMDIParent.Progressbar.Refresh()
		//        '=============================================================
		//    Loop
		//    Rs.Close()
		//    '=============================================================
		//    frmMDIParent.Progressbar.EditValue = 0
		//    frmMDIParent.Progressbar.Refresh()
		//    '=============================================================
		//End Sub
		
		//Private Sub LoadSize()
		//    'Dim Rs As ADODB.Recordset = New ADODB.Recordset
		//    Dim cmdSql As String = "SELECT DISTINCT Name FROM TpicsSize ORDER BY Name"
		
		//    'Rs.Open(cmdSql, GDSN, CursorTypeEnum.adOpenForwardOnly, LockTypeEnum.adLockReadOnly)
		//    ''=======================================================================================
		//    'frmMDIParent.BarStaticItem.Caption = "LoadSize " & Rs.RecordCount & " Rows."
		//    'frmMDIParent.Progressbar.Visibility = DevExpress.XtraBars.BarItemVisibility.Always
		//    'frmMDIParent.Progressbar.EditValue = 0
		//    ''=======================================================================================
		//    'If Rs.RecordCount > 0 Then
		//    '    Rs.MoveFirst()
		//    '    Dim i As Integer = 0
		//    '    Dim strCode As String = "|"
		//    '    While Not Rs.EOF
		//    '        strCode = strCode & Rs.Fields("Name").Value.ToString & "|"
		//    '        i = i + 1
		//    '        Rs.MoveNext()
		//    '        '=============================================================
		//    '        'frmMDIParent.Progressbar.EditValue = (i / Rs.RecordCount) * 100
		//    '        'frmMDIParent.Progressbar.Refresh()
		//    '        '=============================================================
		//    '    End While
		//    '    strCode = Strings.Left(strCode, Len(strCode) - 1)
		//    '    fgSize1.set_ColComboList(1, strCode)
		//    'End If
		//    'Rs.Close()
		//    ''=============================================================
		//    'frmMDIParent.Progressbar.EditValue = 0
		//    'frmMDIParent.Progressbar.Refresh()
		//    ''=============================================================
		//    Dim DB As New Database(ISODocument)
		//    Dim DT As DataTable
		//    Dim riCombo As New RepositoryItemComboBox
		
		//    DT = DB.GetDataTable(cmdSql)
		//    'For Each dr As DataRow In DT.Rows
		//    '    riCombo.Items.Add(dr("Name").ToString)
		//    'Next
		//    riCombo.Items.AddRange(New String() {"London", "Berlin", "Paris"})
		//    GridSize.DataSource = DT
		//    GridSize.RepositoryItems.Add(riCombo)
		//    fgSize.Columns("Name").ColumnEdit = riCombo
		
		//End Sub
		
		//Private Sub LoadColor()
		//    Dim Rs As ADODB.Recordset = New ADODB.Recordset
		//    Dim cmdSql As String = "SELECT DISTINCT Name FROM TpicsColor ORDER BY Name"
		
		//    Rs.Open(cmdSql, GDSN, CursorTypeEnum.adOpenForwardOnly, LockTypeEnum.adLockReadOnly)
		//    '=======================================================================================
		//    frmMDIParent.BarStaticItem.Caption = "LoadColor " & Rs.RecordCount & " Rows."
		//    frmMDIParent.Progressbar.Visibility = DevExpress.XtraBars.BarItemVisibility.Always
		//    frmMDIParent.Progressbar.EditValue = 0
		//    '=======================================================================================
		//    If Rs.RecordCount > 0 Then
		//        Rs.MoveFirst()
		//        Dim i As Integer = 0
		//        Dim strCode As String = "|"
		//        While Not Rs.EOF
		//            strCode = strCode & Rs.Fields("Name").Value.ToString & "|"
		//            i = i + 1
		//            Rs.MoveNext()
		//            '=============================================================
		//            'frmMDIParent.Progressbar.EditValue = (i / Rs.RecordCount) * 100
		//            'frmMDIParent.Progressbar.Refresh()
		//            '=============================================================
		//        End While
		//        strCode = Strings.Left(strCode, Len(strCode) - 1)
		//        fgColor1.set_ColComboList(1, strCode)
		//    End If
		//    Rs.Close()
		//    '=============================================================
		//    frmMDIParent.Progressbar.EditValue = 0
		//    frmMDIParent.Progressbar.Refresh()
		//    '=============================================================
		//End Sub
		
		//Private Sub LoadFabric()
		//    'Dim Rs As ADODB.Recordset = New ADODB.Recordset
		//    'Dim cmdSql As String = "SELECT DISTINCT Name FROM TpicsFabric Order by Name"
		
		//    'Rs.Open(cmdSql, GDSN, CursorTypeEnum.adOpenForwardOnly, LockTypeEnum.adLockReadOnly)
		//    ''=======================================================================================
		//    'frmMDIParent.BarStaticItem.Caption = "LoadFabric " & Rs.RecordCount & " Rows."
		//    'frmMDIParent.Progressbar.Visibility = DevExpress.XtraBars.BarItemVisibility.Always
		//    'frmMDIParent.Progressbar.EditValue = 0
		//    ''=======================================================================================
		//    'If Rs.RecordCount > 0 Then
		//    '    Rs.MoveFirst()
		//    '    Dim i As Integer = 0
		//    '    Dim strCode As String = "|"
		//    '    While Not Rs.EOF
		//    '        strCode = strCode & Rs.Fields("Name").Value.ToString & "|"
		//    '        i = i + 1
		//    '        Rs.MoveNext()
		//    '        '=============================================================
		//    '        'frmMDIParent.Progressbar.EditValue = (i / Rs.RecordCount) * 100
		//    '        'frmMDIParent.Progressbar.Refresh()
		//    '        '=============================================================
		//    '    End While
		//    '    strCode = Strings.Left(strCode, Len(strCode) - 1)
		//    '    fgFabric.set_ColComboList(1, strCode)
		//    'End If
		//    'Rs.Close()
		//    ''=============================================================
		//    'frmMDIParent.Progressbar.EditValue = 0
		//    'frmMDIParent.Progressbar.Refresh()
		//    ''=============================================================
		//End Sub
		
		//Private Sub LoadSupplier()
		//    Dim Rs As ADODB.Recordset = New ADODB.Recordset
		//    Dim cmdSql As String = "SELECT DISTINCT Code FROM TPicsSupplier ORDER BY Code"
		
		//    Rs.Open(cmdSql, GDSN, CursorTypeEnum.adOpenForwardOnly, LockTypeEnum.adLockReadOnly)
		//    '=======================================================================================
		//    frmMDIParent.BarStaticItem.Caption = "LoadSupplier " & Rs.RecordCount & " Rows."
		//    frmMDIParent.Progressbar.Visibility = DevExpress.XtraBars.BarItemVisibility.Always
		//    frmMDIParent.Progressbar.EditValue = 0
		//    '=======================================================================================
		//    If Rs.RecordCount > 0 Then
		//        Rs.MoveFirst()
		//        Dim i As Integer = 0
		//        Dim strCode As String = "|"
		//        While Not Rs.EOF
		//            strCode = strCode & Rs.Fields("Code").Value.ToString & "|"
		//            i = i + 1
		//            Rs.MoveNext()
		//            '=============================================================
		//            'frmMDIParent.Progressbar.EditValue = (i / Rs.RecordCount) * 100
		//            'frmMDIParent.Progressbar.Refresh()
		//            '=============================================================
		//        End While
		//        strCode = Strings.Left(strCode, Len(strCode) - 1)
		//        fgAcc.set_ColComboList(3, strCode)
		//    End If
		//    Rs.Close()
		//    '=============================================================
		//    frmMDIParent.Progressbar.EditValue = 0
		//    frmMDIParent.Progressbar.Refresh()
		//    '=============================================================
		//End Sub
		
		#endregion
				
		#region Save
        public void SaveData()
        {
            this.Cursor = Cursors.WaitCursor;
            if (txtModel.Text.Length == 0)
            {
                MessageBox.Show("คุณยังไม่ใส่ Model No.?","Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                this.Cursor = Cursors.Default;
                return;
            }
            if (txtName.Text.Length == 0)
            {
                MessageBox.Show("คุณยังไม่ใส่ชื่อ Model ?", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Cursor = Cursors.Default;
                return;
            }
            if (fg.RowCount == 0)
            {
                MessageBox.Show("คุณยังไม่ Gen Tpics Code ?", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Cursor = Cursors.Default;
                return;
            }
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                SaveModel(db);
                SaveSize(db);
                SaveColor(db);
                SaveFabric(db);
                SaveMaster(db);
                SaveBOM(db);
                db.CommitTrans();
                MessageBox.Show("Save Complete", "Save", MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
        }
		private void SaveModel(cDatabase db)
		{
			string cmdSql = "SELECT Count(Model) FROM TPicsModel WHERE Model = \'" + txtModel.Text + "\'";
			
			int intResult =  int.Parse(db.ExecuteFirstValue(cmdSql));
			if (intResult > 0)
			{
				cmdSql = "UPDATE TPicsModel SET Name = \'" + txtName.Text + "\'" + ",Division=\'" + cboDivision.Text + "\'" + 
                    ",Style=\'" + txtStyle.Text + "\'" +",Remark=N'"+memRemark.Text+"'"+ 
                    " WHERE Model = \'" + txtModel.Text + "\'";
				db.Execute(cmdSql);
			}
			else
			{
				cmdSql = "INSERT INTO TPicsModel (Model,Name,Division,Style,Remark) " + 
                    " VALUES ('" + txtModel.Text + "','" + txtName.Text + "','" + cboDivision.Text + "','" + txtStyle.Text +"',N'"+memRemark.Text+"')";
				db.Execute(cmdSql);
			}
		}
		private void SaveSize(cDatabase db)
		{
			string strSQL="DELETE FROM TPICSSIZE WHERE MODEL='"+txtModel.Text+"'";
            db.Execute(strSQL);
			for (int i = 0; i <= fgSize.DataRowCount - 1; i++)
			{
                if (fgSize.GetRowCellDisplayText(i, "Name").Length > 0)
                {
                    strSQL = "INSERT INTO TPICSSIZE(NAME,MODEL)VALUES('" + fgSize.GetRowCellDisplayText(i, "Name") + "','" + txtModel.Text + "')";
                    db.Execute(strSQL);
                }
                //if (fgSize.GetRowCellDisplayText(i, "Name").Length > 0)
                //{
                //    strSQL = "SELECT COUNT(NAME) FROM TpicsSize WHERE Name = \'" + fgSize.GetRowCellDisplayText(i, "Name") + "\' and Model = \'" + txtModel.Text + "\'";
                //    int intResult = int.Parse(db.ExecuteFirstValue(strSQL));
                //    if (intResult == 0)
                //    {
                //        strSQL = "INSERT INTO TpicsSize (Name,Model) VALUES (\'" + fgSize.GetRowCellDisplayText(i, "Name") + "\',\'" + txtModel.Text + "\')";
                //        db.Execute(strSQL);
                //    }
                //}
			}
		}
		private void SaveColor(cDatabase db)
		{
			string strSQL="DELETE FROM TPICSCOLOR WHERE MODEL='"+ txtModel.Text + "'";
            db.Execute(strSQL);
			for (int i = 0; i <= fgColor.DataRowCount - 1; i++)
			{
			    if(fgColor.GetRowCellDisplayText(i, "Name").Length > 0)
				{
                    strSQL="INSERT INTO TPICSCOLOR(NAME,FULLNAME,MODEL)VALUES('"+ fgColor.GetRowCellDisplayText(i, "Name") + "'" +
                        ",'" + fgColor.GetRowCellDisplayText(i, "FullName") + "','" + txtModel.Text + "')";
                    db.Execute(strSQL);
				}
			}
			return;
		}
		private void SaveFabric(cDatabase db)
		{
			string cmdSql = "delete FROM TpicsFabric WHERE Model = \'" + txtModel.Text + "\'";
			
			db.Execute(cmdSql);
			for (int r = 0; r <= fgFabric.DataRowCount - 1; r++)
			{
				if (fgFabric.GetRowCellDisplayText(r, "Name").Length > 0)
				{
					//If fgFabric1.get_TextMatrix(r, eFabric.Common) = "-1" Then
					//    strCommon = "1"
					//Else
					//    strCommon = "0"
					//End If
					//If fgFabric1.get_TextMatrix(r, eFabric.Screen) = "-1" Then
					//    strScn = "1"
					//Else
					//    strScn = "0"
					//End If
					//If fgFabric1.get_TextMatrix(r, eFabric.Print) = "1" Then
					//    strPrt = "1"
					//Else
					//    strPrt = "0"
					//End If
					cmdSql = "INSERT INTO TpicsFabric (Code,Name,Model,WC,ColorID) VALUES (" + "\'" + fgFabric.GetRowCellDisplayText(r, "Code") + "\'" + ",\'" + fgFabric.GetRowCellDisplayText(r, "Name") + "\'" + ",\'" + txtModel.Text + "\'" + ",\'" + fgFabric.GetRowCellDisplayText(r, "WC") + "\'" + ",\'" + fgFabric.GetRowCellDisplayText(r, "ColorID") + "\')";
					db.Execute(cmdSql);
				}
			}
			
			cmdSql = "delete FROM TpicsFabricDetail WHERE Model = \'" + txtModel.Text + "\'";
			db.Execute(cmdSql);
			// - - - save fabric detail
			for (int r = 0; r <= fgFabric.DataRowCount - 1; r++)
			{
				if (fgFabric.GetRowCellDisplayText(r, "Name").Length > 0)
				{
					for (int c = 4; c <= fgFabric.Columns.Count - 1; c++)
					{
						cmdSql = "INSERT INTO TpicsFabricDetail (Model,Code,ColorID,Size,Qty) VALUES ("+
                            "'" + txtModel.Text + "',"+
                            "'" + fgFabric.GetRowCellDisplayText(r, "Code") + "',"+
                            "'"+ fgFabric.GetRowCellDisplayText(r,"ColorID")+"',"+
                            "'" + fgFabric.Columns[c].FieldName + "',"+
                            "'" + fgFabric.GetRowCellDisplayText(r, fgFabric.Columns[c].FieldName) + "')";
						db.Execute(cmdSql);
					}
				}
			}
		}
		//Private Sub SaveAccessory()
		//    Dim Rs As ADODB.Recordset = New ADODB.Recordset
		//    Dim strSQL As String
		
		//    Try
		//        'TpicsAccessory
		//        For i As Integer = 1 To fgAcc.Rows - 1
		//            If Not Trim(fgAcc.get_TextMatrix(i, 1)) = Nothing Then
		//                strSQL = "Select AccCode From TpicsAccessory Where AccCode='" & fgAcc.get_TextMatrix(i, 1) & "'"
		//                Rs.Open(strSQL, GDSN, CursorTypeEnum.adOpenForwardOnly, LockTypeEnum.adLockReadOnly)
		//                If Rs.EOF Then
		//                    strSQL = "Insert Into TpicsAccessory(AccCode,AccName,AccOf,Unit)Values('" & fgAcc.get_TextMatrix(i, 1) & "','" & fgAcc.get_TextMatrix(i, 2) & "','" & fgAcc.get_TextMatrix(i, 3) & "','" & _
		//                                 fgAcc.get_TextMatrix(i, 9) & "')"
		//                    GDSN.Execute(strSQL)
		//                Else
		//                    strSQL = "Update TpicsAccessory Set AccName='" & fgAcc.get_TextMatrix(i, 2) & "',AccOf='" & fgAcc.get_TextMatrix(i, 3) & "',Unit='" & fgAcc.get_TextMatrix(i, 9) & _
		//                                "' Where AccCode='" & fgAcc.get_TextMatrix(i, 1) & "'"
		//                    GDSN.Execute(strSQL)
		//                End If
		//                Rs.Close()
		//            End If
		//        Next i
		//        'TpicsAccessoryDetail
		//        strSQL = "Delete From TpicsAccessoryDetail Where Model='" & txtModel.Text & "'"
		//        GDSN.Execute(strSQL)
		//        For i As Integer = 1 To fgAcc.Rows - 1
		//            If Not Trim(fgAcc.get_TextMatrix(i, 1)) = Nothing Then
		//                strSQL = "Insert Into TpicsAccessoryDetail(Model,AccCode,AccType,Remark1,Remark2,Qty,QtyDiv,SupCode)Values('" & txtModel.Text & "','" & _
		//                                  fgAcc.get_TextMatrix(i, 1) & "','" & fgAcc.get_TextMatrix(i, 4) & "','" & fgAcc.get_TextMatrix(i, 5) & "','" & fgAcc.get_TextMatrix(i, 6) & "'," & _
		//                                  fgAcc.get_TextMatrix(i, 7) & "," & fgAcc.get_TextMatrix(i, 8) & ",'" & fgAcc.get_TextMatrix(i, 10) & "')"
		//                GDSN.Execute(strSQL)
		//            End If
		//        Next i
		//    Catch
		//        MsgBox("Error ?... " & Err.Description, MsgBoxStyle.Critical, "Save Accessory")
		//    End Try
		//End Sub
		private void SaveMaster(cDatabase db)
		{
			string strSQL = "delete FROM TpicsMaster WHERE Model = \'" + txtModel.Text + "\'";
			
			db.Execute(strSQL);
			for (int r = 0; r <= fg.DataRowCount - 1; r++)
			{
				if (fg.GetRowCellDisplayText(r, "ITEMCODE").Length > 0)
				{
					strSQL = "INSERT INTO TPicsMaster (ItemCode,Name,WcCode,WcName,SupCode,SupName,StrgCode,StrgName,Unit,SewOrPack,Model" + ",Dependent,Release,Fix,Mfg,Delivery,DispatchClass,RoundUp,DecimalPoint,Classification,Style,Size,Color) VALUES (" + "\'" + fg.GetRowCellDisplayText(r, "ITEMCODE") + "\'" + ",\'" + fg.GetRowCellDisplayText(r, "NAME") + "\'" + ",\'" + fg.GetRowCellDisplayText(r, "WCCODE") + "\'" + ",\'" + fg.GetRowCellDisplayText(r, "WCNAME") + "\'" + ",\'" + fg.GetRowCellDisplayText(r, "SUPCODE") + "\'" + ",\'" + fg.GetRowCellDisplayText(r, "SUPNAME") + "\'" + ",\'" + fg.GetRowCellDisplayText(r, "STRGCODE") + "\'" + ",\'" + fg.GetRowCellDisplayText(r, "STRGNAME") + "\'" + ",\'" + fg.GetRowCellDisplayText(r, "UNIT") + "\'";
					strSQL = strSQL + ",\'" + fg.GetRowCellDisplayText(r, "SEWORPACK") + "\'";
					strSQL = strSQL + ",\'" + txtModel.Text + "\'";
					strSQL = strSQL + "," + fg.GetRowCellDisplayText(r, "DEPENDENT");
					strSQL = strSQL + "," + fg.GetRowCellDisplayText(r, "RELEASE");
					strSQL = strSQL + "," + fg.GetRowCellDisplayText(r, "FIX");
					strSQL = strSQL + "," + fg.GetRowCellDisplayText(r, "MFG");
					strSQL = strSQL + "," + fg.GetRowCellDisplayText(r, "DELIVERY");
					strSQL = strSQL + "," + fg.GetRowCellDisplayText(r, "DISPATCHCLASS");
					strSQL = strSQL + "," + fg.GetRowCellDisplayText(r, "ROUNDUP");
					strSQL = strSQL + "," + fg.GetRowCellDisplayText(r, "DECIMALPOINT");
					//Debug.Print(fg.GetRowCellDisplayText(r, "CLASSIFICATION"))
					strSQL = strSQL + ",\'" + fg.GetRowCellDisplayText(r, "CLASSIFICATION") + "\'";
					strSQL = strSQL + ",\'" + fg.GetRowCellDisplayText(r, "STYLE") + "\'";
					strSQL = strSQL + ",\'" + fg.GetRowCellDisplayText(r, "SIZE") + "\'";
					strSQL = strSQL + ",\'" + fg.GetRowCellDisplayText(r, "COLOR") + "\'";
					strSQL = strSQL + ")";
					db.Execute(strSQL);
				}
			}
		}
		private void SaveBOM(cDatabase db)
		{
			string SqlStr = "DELETE from tpicsbom where model = \'" + txtModel.Text + "\'";
			
			db.Execute(SqlStr);
			for (int i = 0; i <= tb.DataRowCount - 1; i++)
			{
				if (tb.GetRowCellDisplayText(i, "PARENT").Length > 0)
				{
					SqlStr = "insert into tpicsbom (model,CODE,KCODE,SIYOU,SIYOUW) values (" + "\'" + txtModel.Text + "\'" + ",\'" + tb.GetRowCellDisplayText(i, "PARENT") + "\'" + ",\'" + tb.GetRowCellDisplayText(i, "CHILD") + "\'" + "," + tb.GetRowCellDisplayText(i, "QTY") + "," + tb.GetRowCellDisplayText(i, "QTYDIV") + ")";
					db.Execute(SqlStr);
				}
			}
		}
		#endregion
		
		#region Gen Code
		
		private void GenCode2()
		{
			string strModel;
			string strSize;
			string strColor;
			string strColorFullName;
			string strFb;
			int rSize;
			int rColor;
			int rFb;
			int CurrentRow;
			
			try
			{
				
				
				if (txtModel.Text == "")
				{
					MessageBox.Show("คุณยังไม่ใส่รหัส model ?");
					return;
				}
				if (txtStyle.Text == "")
				{
					MessageBox.Show("คุณยังไม่ได้ใส่ style");
					return;
				}
				ClearGrid(fg);
				strModel = txtModel.Text;
				//-----------------------------------------------------------------------------Generate Item Master------------------------------------------------------------------------------------------------------
				for (rSize = 0; rSize <= fgSize.DataRowCount - 1; rSize++)
				{
					if (fgSize.GetRowCellDisplayText(rSize, "Name").Trim().Length > 0)
					{
						strSize = "";
						strSize = fgSize.GetRowCellDisplayText(rSize, "Name");
						for (rColor = 0; rColor <= fgColor.DataRowCount - 1; rColor++)
						{
							if (fgColor.GetRowCellDisplayText(rColor, "Name").Trim().Length > 0)
							{
								strColor = "";
								strColor = fgColor.GetRowCellDisplayText(rColor, "Name");
								strColorFullName = "";
								strColorFullName = fgColor.GetRowCellDisplayText(rColor, "FullName");
								//===================================Pack Process=============================================================
								fg.AddNewRow();
								CurrentRow = fg.FocusedRowHandle;
								fg.SetRowCellValue(CurrentRow, "ITEMCODE", strModel + "-" + strSize + "-" + strColor);
								fg.SetRowCellValue(CurrentRow, "NAME", txtName.Text + " Size : " + strSize + " Color : " + strColor);
								switch (cboDivision.Text)
								{
									case "Sales1":
										fg.SetRowCellValue(CurrentRow, "WCCODE", "PACK-S1");
										fg.SetRowCellValue(CurrentRow, "WCNAME", "PACKING SALE 1");
										fg.SetRowCellValue(CurrentRow, "SUPCODE", "PACK-S1");
										fg.SetRowCellValue(CurrentRow, "SUPNAME", "PACKING SALE 1");
										fg.SetRowCellValue(CurrentRow, "STRGCODE", "ST08-S1");
										fg.SetRowCellValue(CurrentRow, "STRGNAME", "INTER WAREHOUSE  SALE 1");
										break;
									case "Sales2":
										fg.SetRowCellValue(CurrentRow, "WCCODE", "PACK-S2");
										fg.SetRowCellValue(CurrentRow, "WCNAME", "PACKING SALE 2");
										fg.SetRowCellValue(CurrentRow, "SUPCODE", "PACK-S2");
										fg.SetRowCellValue(CurrentRow, "SUPNAME", "PACKING SALE 2");
										fg.SetRowCellValue(CurrentRow, "STRGCODE", "ST08-S2");
										fg.SetRowCellValue(CurrentRow, "STRGNAME", "INTER WAREHOUSE  SALE 2");
										break;
									case "Sales3":
										fg.SetRowCellValue(CurrentRow, "WCCODE", "PACK-S3");
										fg.SetRowCellValue(CurrentRow, "WCNAME", "PACKING SALE 3");
										fg.SetRowCellValue(CurrentRow, "SUPCODE", "PACK-S3");
										fg.SetRowCellValue(CurrentRow, "SUPNAME", "PACKING SALE 3");
										fg.SetRowCellValue(CurrentRow, "STRGCODE", "ST08-S3");
										fg.SetRowCellValue(CurrentRow, "STRGNAME", "INTER WAREHOUSE  SALE 3");
										break;
									default:
										fg.SetRowCellValue(CurrentRow, "WCCODE", "PACK");
										fg.SetRowCellValue(CurrentRow, "WCNAME", "PACKING");
										fg.SetRowCellValue(CurrentRow, "SUPCODE", "PACK");
										fg.SetRowCellValue(CurrentRow, "SUPNAME", "PACKING");
										fg.SetRowCellValue(CurrentRow, "STRGCODE", "ST08");
										fg.SetRowCellValue(CurrentRow, "STRGNAME", "INTER WAREHOUSE");
										break;
								}
								fg.SetRowCellValue(CurrentRow, "UNIT", "PCS");
								fg.SetRowCellValue(CurrentRow, "SEWORPACK", "F/G");
								fg.SetRowCellValue(CurrentRow, "DEPENDENT", "2"); //Dependent Level
								fg.SetRowCellValue(CurrentRow, "RELEASE", "32"); //Release Period
								fg.SetRowCellValue(CurrentRow, "FIX", "32"); //Fix Period
								fg.SetRowCellValue(CurrentRow, "MFG", "1"); //Mfg. LDs)
								fg.SetRowCellValue(CurrentRow, "DELIVERY", "0"); //Delivery LDs
								fg.SetRowCellValue(CurrentRow, "DISPATCHCLASS", "1");
								fg.SetRowCellValue(CurrentRow, "ROUNDUP", "0");
								fg.SetRowCellValue(CurrentRow, "DECIMALPOINT", "0");
								fg.SetRowCellValue(CurrentRow, "CLASSIFICATION", "C");
								fg.SetRowCellValue(CurrentRow, "STYLE", txtStyle.Text);
								fg.SetRowCellValue(CurrentRow, "SIZE", strSize);
								fg.SetRowCellValue(CurrentRow, "COLOR", strColorFullName);
								//'===========================================Sew Process=====================================================
								fg.AddNewRow();
								CurrentRow = fg.FocusedRowHandle;
								fg.SetRowCellValue(CurrentRow, "ITEMCODE", strModel + "-" + strSize + "-" + strColor + "-S");
								fg.SetRowCellValue(CurrentRow, "NAME", txtName.Text + "Size : " + strSize + " " + strColor + "-SEW");
								switch (cboDivision.Text)
								{
									case "Sales1":
										fg.SetRowCellValue(CurrentRow, "WCCODE", "SEW-S1");
										fg.SetRowCellValue(CurrentRow, "WCNAME", "SEWING SALE 1");
										fg.SetRowCellValue(CurrentRow, "SUPCODE", "SEW-S1");
										fg.SetRowCellValue(CurrentRow, "SUPNAME", "SEWING SALE 1");
										fg.SetRowCellValue(CurrentRow, "STRGCODE", "PACK-S1");
										fg.SetRowCellValue(CurrentRow, "STRGNAME", "PACKING SALE 1");
										break;
									case "Sales2":
										fg.SetRowCellValue(CurrentRow, "WCCODE", "SEW-S2");
										fg.SetRowCellValue(CurrentRow, "WCNAME", "SEWING SALE 2");
										fg.SetRowCellValue(CurrentRow, "SUPCODE", "SEW-S2");
										fg.SetRowCellValue(CurrentRow, "SUPNAME", "SEWING SALE 2");
										fg.SetRowCellValue(CurrentRow, "STRGCODE", "PACK-S2");
										fg.SetRowCellValue(CurrentRow, "STRGNAME", "PACKING SALE 2");
										break;
									case "Sales3":
										fg.SetRowCellValue(CurrentRow, "WCCODE", "SEW-S3");
										fg.SetRowCellValue(CurrentRow, "WCNAME", "SEWING SALE 3");
										fg.SetRowCellValue(CurrentRow, "SUPCODE", "SEW-S3");
										fg.SetRowCellValue(CurrentRow, "SUPNAME", "SEWING SALE 3");
										fg.SetRowCellValue(CurrentRow, "STRGCODE", "PACK-S3");
										fg.SetRowCellValue(CurrentRow, "STRGNAME", "PACKING SALE 3");
										break;
									default:
										fg.SetRowCellValue(CurrentRow, "WCCODE", "SEW");
										fg.SetRowCellValue(CurrentRow, "WCNAME", "SEWING");
										fg.SetRowCellValue(CurrentRow, "SUPCODE", "SEW");
										fg.SetRowCellValue(CurrentRow, "SUPNAME", "SEWING");
										fg.SetRowCellValue(CurrentRow, "STRGCODE", "PACK");
										fg.SetRowCellValue(CurrentRow, "STRGNAME", "PACKING");
										break;
								}
								fg.SetRowCellValue(CurrentRow, "UNIT", "PCS");
								fg.SetRowCellValue(CurrentRow, "SEWORPACK", "");
								fg.SetRowCellValue(CurrentRow, "DEPENDENT", "0"); //Dependent Level
								fg.SetRowCellValue(CurrentRow, "RELEASE", "31"); //Release Period
								fg.SetRowCellValue(CurrentRow, "FIX", "31"); //Fix Period
								fg.SetRowCellValue(CurrentRow, "MFG", "1"); //Mfg. LDs
								fg.SetRowCellValue(CurrentRow, "DELIVERY", "0"); //Delivery LDs
								fg.SetRowCellValue(CurrentRow, "DISPATCHCLASS", "1");
								fg.SetRowCellValue(CurrentRow, "ROUNDUP", "0");
								fg.SetRowCellValue(CurrentRow, "DECIMALPOINT", "0");
								fg.SetRowCellValue(CurrentRow, "CLASSIFICATION", "C");
								fg.SetRowCellValue(CurrentRow, "STYLE", txtStyle.Text);
								fg.SetRowCellValue(CurrentRow, "SIZE", strSize);
								fg.SetRowCellValue(CurrentRow, "COLOR", strColorFullName);
								//===========================================Cut Process=====================================================
								fg.AddNewRow();
								CurrentRow = fg.FocusedRowHandle;
								fg.SetRowCellValue(CurrentRow, "ITEMCODE", strModel + "-" + strSize + "-" + strColor + "-C");
								fg.SetRowCellValue(CurrentRow, "NAME", txtName.Text + "Size : " + strSize + " " + strColor + "-CUT");
								switch (cboDivision.Text)
								{
									case "Sales1":
										fg.SetRowCellValue(CurrentRow, "WCCODE", "CUT-S1");
										fg.SetRowCellValue(CurrentRow, "WCNAME", "CUTTING SALE 1");
										fg.SetRowCellValue(CurrentRow, "SUPCODE", "CUT-S1");
										fg.SetRowCellValue(CurrentRow, "SUPNAME", "CUTTING SALE 1");
										fg.SetRowCellValue(CurrentRow, "STRGCODE", "SEW-S1");
										fg.SetRowCellValue(CurrentRow, "STRGNAME", "SEWING SALE 1");
										break;
									case "Sales2":
										fg.SetRowCellValue(CurrentRow, "WCCODE", "CUT-S2");
										fg.SetRowCellValue(CurrentRow, "WCNAME", "CUTTING SALE 2");
										fg.SetRowCellValue(CurrentRow, "SUPCODE", "CUT-S2");
										fg.SetRowCellValue(CurrentRow, "SUPNAME", "CUTTING SALE 2");
										fg.SetRowCellValue(CurrentRow, "STRGCODE", "SEW-S2");
										fg.SetRowCellValue(CurrentRow, "STRGNAME", "SEWING SALE 2");
										break;
									case "Sales3":
										fg.SetRowCellValue(CurrentRow, "WCCODE", "CUT-S3");
										fg.SetRowCellValue(CurrentRow, "WCNAME", "CUTTING SALE 3");
										fg.SetRowCellValue(CurrentRow, "SUPCODE", "CUT-S3");
										fg.SetRowCellValue(CurrentRow, "SUPNAME", "CUTTING SALE 3");
										fg.SetRowCellValue(CurrentRow, "STRGCODE", "SEW-S3");
										fg.SetRowCellValue(CurrentRow, "STRGNAME", "SEWING SALE 3");
										break;
									default:
										fg.SetRowCellValue(CurrentRow, "WCCODE", "CUT");
										fg.SetRowCellValue(CurrentRow, "WCNAME", "CUTTING");
										fg.SetRowCellValue(CurrentRow, "SUPCODE", "CUT");
										fg.SetRowCellValue(CurrentRow, "SUPNAME", "CUTTING");
										fg.SetRowCellValue(CurrentRow, "STRGCODE", "SEW");
										fg.SetRowCellValue(CurrentRow, "STRGNAME", "SEWING");
										break;
								}
								fg.SetRowCellValue(CurrentRow, "UNIT", "PCS");
								fg.SetRowCellValue(CurrentRow, "SEWORPACK", "");
								//Debug.Print(fg.GetRowCellDisplayText(CurrentRow, "SEWORPACK"))
								fg.SetRowCellValue(CurrentRow, "DEPENDENT", "0"); //Dependent Level
								fg.SetRowCellValue(CurrentRow, "RELEASE", "30"); //Release Period
								fg.SetRowCellValue(CurrentRow, "FIX", "30"); //Fix Period
								fg.SetRowCellValue(CurrentRow, "MFG", "1"); //Mfg. LDs
								fg.SetRowCellValue(CurrentRow, "DELIVERY", "0"); //Delivery LDs
								fg.SetRowCellValue(CurrentRow, "DISPATCHCLASS", "1");
								fg.SetRowCellValue(CurrentRow, "ROUNDUP", "0");
								fg.SetRowCellValue(CurrentRow, "DECIMALPOINT", "0");
								fg.SetRowCellValue(CurrentRow, "CLASSIFICATION", "C");
								fg.SetRowCellValue(CurrentRow, "STYLE", txtStyle.Text);
								fg.SetRowCellValue(CurrentRow, "SIZE", strSize);
								fg.SetRowCellValue(CurrentRow, "COLOR", strColorFullName);
								//==================================================fabric==========================================================
							}
						}
					}
				}
				
				for (rFb = 0; rFb <= fgFabric.DataRowCount - 1; rFb++)
				{
					if (fgFabric.GetRowCellDisplayText(rFb, "Code").Trim().Length > 0)
					{
						strFb = "";
						strFb = fgFabric.GetRowCellDisplayText(rFb, "Code");
						fg.AddNewRow();
						CurrentRow = fg.FocusedRowHandle;
						fg.SetRowCellValue(CurrentRow, "ITEMCODE", strFb);
						fg.SetRowCellValue(CurrentRow, "NAME", fgFabric.GetRowCellDisplayText(rFb, "Name"));
						fg.SetRowCellValue(CurrentRow, "WCCODE", fgFabric.GetRowCellDisplayText(rFb, "WC"));
						fg.SetRowCellValue(CurrentRow, "WCNAME", "FABRIC DELIVERY");
						fg.SetRowCellValue(CurrentRow, "SUPCODE", fgFabric.GetRowCellDisplayText(rFb, "WC"));
						fg.SetRowCellValue(CurrentRow, "SUPNAME", "FABRIC DELIVERY");
						switch (cboDivision.Text)
						{
							case "Sales1":
								fg.SetRowCellValue(CurrentRow, "STRGCODE", "ST05-S1");
								fg.SetRowCellValue(CurrentRow, "STRGNAME", "INTER FABRIC STORE  SALE 1");
								break;
							case "Sales2":
								fg.SetRowCellValue(CurrentRow, "STRGCODE", "ST05-S2");
								fg.SetRowCellValue(CurrentRow, "STRGNAME", "INTER FABRIC STORE  SALE 2");
								break;
							case "Sales3":
								fg.SetRowCellValue(CurrentRow, "STRGCODE", "ST05-S3");
								fg.SetRowCellValue(CurrentRow, "STRGNAME", "INTER FABRIC STORE  SALE 3");
								break;
							default:
								fg.SetRowCellValue(CurrentRow, "STRGCODE", "ST05");
								fg.SetRowCellValue(CurrentRow, "STRGNAME", "INTER FABRIC STORE");
								break;
						}
						fg.SetRowCellValue(CurrentRow, "UNIT", "KGS");
						fg.SetRowCellValue(CurrentRow, "SEWORPACK", "");
						fg.SetRowCellValue(CurrentRow, "DEPENDENT", "0"); //Dependent Level
						fg.SetRowCellValue(CurrentRow, "RELEASE", "75"); //Release Period
						fg.SetRowCellValue(CurrentRow, "FIX", "75"); //Fix Period
						fg.SetRowCellValue(CurrentRow, "MFG", "0"); //Mfg. LDs
						if (fg.GetRowCellDisplayText(CurrentRow, "WCCODE") == "FDEL")
						{
							fg.SetRowCellValue(CurrentRow, "DELIVERY", "12"); //Delivery LDs ผ้าในจาก 6 เป็น 12
						}
						else
						{
							fg.SetRowCellValue(CurrentRow, "DELIVERY", "12"); //Delivery LDs ผ้านอกจาก 4 เป็น 12
						}
						fg.SetRowCellValue(CurrentRow, "DISPATCHCLASS", "0");
						fg.SetRowCellValue(CurrentRow, "ROUNDUP", "0");
						fg.SetRowCellValue(CurrentRow, "DECIMALPOINT", "4");
						fg.SetRowCellValue(CurrentRow, "CLASSIFICATION", "F");
					}
				}
				fg.UpdateCurrentRow();
				//=================================================Accessory========================================================
				//For rAcc = 1 To fgAcc.Rows - 1
				//    If Len(Trim(fgAcc.get_TextMatrix(rAcc, 1))) > 0 Then
				//        fg1.Rows = fg1.Rows + 1
				//        .SETROWCELLVALUE(CurrentRow, "ITEMCODE", fgAcc.get_TextMatrix(rAcc, 1))   'item code
				//        .SETROWCELLVALUE(CurrentRow, "NAME", fgAcc.get_TextMatrix(rAcc, 2))   'Name
				//        .SETROWCELLVALUE(CurrentRow, "WCCODE", fgAcc.get_TextMatrix(rAcc, 9))   'W/C
				//        .SETROWCELLVALUE(CurrentRow, "WCNAME", fgAcc.get_TextMatrix(rAcc, 10))  'W/C name
				//        .SETROWCELLVALUE(CurrentRow, "SUPCODE", fgAcc.get_TextMatrix(rAcc, 9))   'Sup
				//        .SETROWCELLVALUE(CurrentRow, "SUPNAME", fgAcc.get_TextMatrix(rAcc, 10))  'Sup name
				//        Select Case cboDivision.Text
				//            Case "Sales1"
				//                .SETROWCELLVALUE(CurrentRow, "STRGCODE", "ST01-S1")
				//                .SETROWCELLVALUE(CurrentRow, "STRGNAME", "INTER ACCESSORY STORE SALE 1")
				//            Case "Sales2"
				//                .SETROWCELLVALUE(CurrentRow, "STRGCODE", "ST01-S2")
				//                .SETROWCELLVALUE(CurrentRow, "STRGNAME", "INTER ACCESSORY STORE SALE 2")
				//            Case "Sales3"
				//                .SETROWCELLVALUE(CurrentRow, "STRGCODE", "ST01-S3")
				//                .SETROWCELLVALUE(CurrentRow, "STRGNAME", "INTER ACCESSORY STORE SALE 3")
				//            Case Else
				//                .SETROWCELLVALUE(CurrentRow, "STRGCODE", "ST01")
				//                .SETROWCELLVALUE(CurrentRow, "STRGNAME", "INTER ACCESSORY STORE")
				//        End Select
				//        .SETROWCELLVALUE(CurrentRow, "UNIT", fgAcc.get_TextMatrix(rAcc, 8))
				//        .SETROWCELLVALUE(CurrentRow, "SEWORPACK", "")
				//        .SETROWCELLVALUE(CurrentRow, "DEPENDENT", "0")     'Dependent Level
				//        .SETROWCELLVALUE(CurrentRow, "RELEASE", "1")     'Release Period
				//        .SETROWCELLVALUE(CurrentRow, "FIX", "1")     'Fix Period
				//        .SETROWCELLVALUE(CurrentRow, "MFG", "0")    'Mfg. LDs
				//        .SETROWCELLVALUE(CurrentRow, "DELIVERY", "1")    'Delivery LDs
				//        .SETROWCELLVALUE(CurrentRow, "DISPATCHCLASS", "2")
				//        .SETROWCELLVALUE(CurrentRow, "ROUNDUP", "1")
				//        .SETROWCELLVALUE(CurrentRow, "DECIMALPOINT", "4")
				//        .SETROWCELLVALUE(CurrentRow, "CLASSIFICATION", "")
				//    End If
				//Next rAcc
				//For i = 0 To fg1.Cols - 1
				//    fg1.AutoSize(i)
				//Next
				//-------------------------------------------------------------------------------------------- Generate BOM------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
				//tb1.Rows = 1
				ClearGrid(tb);
				strModel = txtModel.Text;
				for (rSize = 0; rSize <= fgSize.DataRowCount - 1; rSize++)
				{
					if (fgSize.GetRowCellDisplayText(rSize, "Name").Trim().Length > 0)
					{
						strSize = fgSize.GetRowCellDisplayText(rSize, "Name");
						for (rColor = 0; rColor <= fgColor.DataRowCount - 1; rColor++)
						{
							if (fgColor.GetRowCellDisplayText(rColor, "Name").Trim().Length > 0)
							{
								strColor = fgColor.GetRowCellDisplayText(rColor, "Name");
								//=============================================Sew==========================================================
								tb.AddNewRow();
								CurrentRow = tb.FocusedRowHandle;
								tb.SetRowCellValue(CurrentRow, "PARENT", strModel + "-" + strSize + "-" + strColor);
								tb.SetRowCellValue(CurrentRow, "CHILD", strModel + "-" + strSize + "-" + strColor + "-S");
								tb.SetRowCellValue(CurrentRow, "QTY", "1");
								tb.SetRowCellValue(CurrentRow, "QTYDIV", "1");
								//=============================================Cut==========================================================
								tb.AddNewRow();
								CurrentRow = tb.FocusedRowHandle;
								tb.SetRowCellValue(CurrentRow, "PARENT", strModel + "-" + strSize + "-" + strColor + "-S");
								tb.SetRowCellValue(CurrentRow, "CHILD", strModel + "-" + strSize + "-" + strColor + "-C");
								tb.SetRowCellValue(CurrentRow, "QTY", "1");
								tb.SetRowCellValue(CurrentRow, "QTYDIV", "1");
								//=============================================Accessory=====================================================
								//For rAcc = 1 To fgAcc.Rows - 1
								//    If Len(Trim(fgAcc.get_TextMatrix(rAcc, 1))) > 0 Then
								//        '***********************************************************Accessory Cut********************************************************************
								//        If Len(Trim(fgAcc.get_TextMatrix(rAcc, 1))) > 0 And fgAcc.get_TextMatrix(rAcc, 3) = "Cut" Then
								//            Select Case fgAcc.get_TextMatrix(rAcc, 4)
								//                Case "Common Part"
								//                    tb.AddNewRow()
								//                    CurrentRow = tb.FocusedRowHandle
								//                    tb.SetRowCellValue(currentrow, "PARENT", strModel & "-" & strSize & "-" & strColor & "-C")
								//                    tb.SetRowCellValue(currentrow, "CHILD", fgAcc.get_TextMatrix(rAcc, 1))
								//                    tb.SetRowCellValue(currentrow, "QTY", fgAcc.get_TextMatrix(rAcc, 7))
								//                    tb.SetRowCellValue(currentrow, "QTYDIV", fgAcc.get_TextMatrix(rAcc, 8))
								//                Case "Fix Size"
								//                    If fgAcc.get_TextMatrix(rAcc, 5) = strSize Then
								//                        tb.AddNewRow()
								//                        CurrentRow = tb.FocusedRowHandle
								//                        tb.SetRowCellValue(currentrow, "PARENT", strModel & "-" & strSize & "-" & strColor & "-C")
								//                        tb.SetRowCellValue(currentrow, "CHILD", fgAcc.get_TextMatrix(rAcc, 1))
								//                        tb.SetRowCellValue(currentrow, "QTY", fgAcc.get_TextMatrix(rAcc, 7))
								//                        tb.SetRowCellValue(currentrow, "QTYDIV", fgAcc.get_TextMatrix(rAcc, 8))
								//                    End If
								//                Case "Fix Color"
								//                    If fgAcc.get_TextMatrix(rAcc, 6) = strColor Then
								//                        tb.AddNewRow()
								//                        CurrentRow = tb.FocusedRowHandle
								//                        tb.SetRowCellValue(currentrow, "PARENT", strModel & "-" & strSize & "-" & strColor & "-C")
								//                        tb.SetRowCellValue(currentrow, "CHILD", fgAcc.get_TextMatrix(rAcc, 1))
								//                        tb.SetRowCellValue(currentrow, "QTY", fgAcc.get_TextMatrix(rAcc, 7))
								//                        tb.SetRowCellValue(currentrow, "QTYDIV", fgAcc.get_TextMatrix(rAcc, 8))
								//                    End If
								//                Case "Specified"
								//                    If (fgAcc.get_TextMatrix(rAcc, 5) = strSize) And (fgAcc.get_TextMatrix(rAcc, 6) = strColor) Then
								//                        tb.AddNewRow()
								//                        CurrentRow = tb.FocusedRowHandle
								//                        tb.SetRowCellValue(currentrow, "PARENT", strModel & "-" & strSize & "-" & strColor & "-C")
								//                        tb.SetRowCellValue(currentrow, "CHILD", fgAcc.get_TextMatrix(rAcc, 1))
								//                        tb.SetRowCellValue(currentrow, "QTY", fgAcc.get_TextMatrix(rAcc, 7))
								//                        tb.SetRowCellValue(currentrow, "QTYDIV", fgAcc.get_TextMatrix(rAcc, 8))
								//                    End If
								
								//            End Select
								//            '***********************************************************Accessory Sew********************************************************************
								//        ElseIf Len(Trim(fgAcc.get_TextMatrix(rAcc, 1))) > 0 And fgAcc.get_TextMatrix(rAcc, 3) = "Sew" Then
								//            Select Case fgAcc.get_TextMatrix(rAcc, 4)
								//                Case "Common Part"
								//                    tb.AddNewRow()
								//                    CurrentRow = tb.FocusedRowHandle
								//                    tb.SetRowCellValue(currentrow, "PARENT", strModel & "-" & strSize & "-" & strColor & "-S")
								//                    tb.SetRowCellValue(currentrow, "CHILD", fgAcc.get_TextMatrix(rAcc, 1))
								//                    tb.SetRowCellValue(currentrow, "QTY", fgAcc.get_TextMatrix(rAcc, 7))
								//                    tb.SetRowCellValue(currentrow, "QTYDIV", fgAcc.get_TextMatrix(rAcc, 8))
								//                Case "Fix Size"
								//                    If fgAcc.get_TextMatrix(rAcc, 5) = strSize Then
								//                        tb.AddNewRow()
								//                        CurrentRow = tb.FocusedRowHandle
								//                        tb.SetRowCellValue(currentrow, "PARENT", strModel & "-" & strSize & "-" & strColor & "-S")
								//                        tb.SetRowCellValue(currentrow, "CHILD", fgAcc.get_TextMatrix(rAcc, 1))
								//                        tb.SetRowCellValue(currentrow, "QTY", fgAcc.get_TextMatrix(rAcc, 7))
								//                        tb.SetRowCellValue(currentrow, "QTYDIV", fgAcc.get_TextMatrix(rAcc, 8))
								//                    End If
								//                Case "Fix Color"
								//                    If fgAcc.get_TextMatrix(rAcc, 6) = strColor Then
								//                        tb.AddNewRow()
								//                        CurrentRow = tb.FocusedRowHandle
								//                        tb.SetRowCellValue(currentrow, "PARENT", strModel & "-" & strSize & "-" & strColor & "-S")
								//                        tb.SetRowCellValue(currentrow, "CHILD", fgAcc.get_TextMatrix(rAcc, 1))
								//                        tb.SetRowCellValue(currentrow, "QTY", fgAcc.get_TextMatrix(rAcc, 7))
								//                        tb.SetRowCellValue(currentrow, "QTYDIV", fgAcc.get_TextMatrix(rAcc, 8))
								//                    End If
								//                Case "Specified"
								//                    If (fgAcc.get_TextMatrix(rAcc, 5) = strSize) And (fgAcc.get_TextMatrix(rAcc, 6) = strColor) Then
								//                        tb.AddNewRow()
								//                        CurrentRow = tb.FocusedRowHandle
								//                        tb.SetRowCellValue(currentrow, "PARENT", strModel & "-" & strSize & "-" & strColor & "-S")
								//                        tb.SetRowCellValue(currentrow, "CHILD", fgAcc.get_TextMatrix(rAcc, 1))
								//                        tb.SetRowCellValue(currentrow, "QTY", fgAcc.get_TextMatrix(rAcc, 7))
								//                        tb.SetRowCellValue(CurrentRow, "QTYDIV", fgAcc.get_TextMatrix(rAcc, 8))
								//                    End If
								
								//            End Select
								//            '***********************************************************Accessory Pack********************************************************************
								//        ElseIf Len(Trim(fgAcc.get_TextMatrix(rAcc, 1))) > 0 And fgAcc.get_TextMatrix(rAcc, 3) = "Pack" Then
								//            Select Case fgAcc.get_TextMatrix(rAcc, 4)
								//                Case "Common Part"
								//                    tb.AddNewRow()
								//                    CurrentRow = tb.FocusedRowHandle
								//                    tb.SetRowCellValue(CurrentRow, "PARENT", strModel & "-" & strSize & "-" & strColor)
								//                    tb.SetRowCellValue(CurrentRow, "CHILD", fgAcc.get_TextMatrix(rAcc, 1))
								//                    tb.SetRowCellValue(CurrentRow, "QTY", fgAcc.get_TextMatrix(rAcc, 7))
								//                    tb.SetRowCellValue(CurrentRow, "QTYDIV", fgAcc.get_TextMatrix(rAcc, 8))
								//                Case "Fix Size"
								//                    If fgAcc.get_TextMatrix(rAcc, 5) = strSize Then
								//                        tb.AddNewRow()
								//                        CurrentRow = tb.FocusedRowHandle
								//                        tb.SetRowCellValue(CurrentRow, "PARENT", strModel & "-" & strSize & "-" & strColor)
								//                        tb.SetRowCellValue(CurrentRow, "CHILD", fgAcc.get_TextMatrix(rAcc, 1))
								//                        tb.SetRowCellValue(CurrentRow, "QTY", fgAcc.get_TextMatrix(rAcc, 7))
								//                        tb.SetRowCellValue(CurrentRow, "QTYDIV", fgAcc.get_TextMatrix(rAcc, 8))
								//                    End If
								
								//                Case "Fix Color"
								//                    If fgAcc.get_TextMatrix(rAcc, 6) = strColor Then
								//                        tb.AddNewRow()
								//                        CurrentRow = tb.FocusedRowHandle
								//                        tb.SetRowCellValue(CurrentRow, "PARENT", strModel & "-" & strSize & "-" & strColor)
								//                        tb.SetRowCellValue(CurrentRow, "CHILD", fgAcc.get_TextMatrix(rAcc, 1))
								//                        tb.SetRowCellValue(CurrentRow, "QTY", fgAcc.get_TextMatrix(rAcc, 7))
								//                        tb.SetRowCellValue(CurrentRow, "QTYDIV", fgAcc.get_TextMatrix(rAcc, 8))
								//                    End If
								//                Case "Specified"
								//                    If (fgAcc.get_TextMatrix(rAcc, 5) = strSize) And (fgAcc.get_TextMatrix(rAcc, 6) = strColor) Then
								//                        tb.AddNewRow()
								//                        CurrentRow = tb.FocusedRowHandle
								//                        tb.SetRowCellValue(CurrentRow, "PARENT", strModel & "-" & strSize & "-" & strColor)
								//                        tb.SetRowCellValue(CurrentRow, "CHILD", fgAcc.get_TextMatrix(rAcc, 1))
								//                        tb.SetRowCellValue(CurrentRow, "QTY", fgAcc.get_TextMatrix(rAcc, 7))
								//                        tb.SetRowCellValue(CurrentRow, "QTYDIV", fgAcc.get_TextMatrix(rAcc, 8))
								//                    End If
								//            End Select
								//        End If
								
								//    End If
								//Next rAcc
								//================================================Fabric===================================================
								for (rFb = 0; rFb <= fgFabric.DataRowCount - 1; rFb++)
								{
									for (int c = 4; c <= fgFabric.Columns.Count - 1; c++)
									{
										if (fgFabric.GetRowCellDisplayText(rFb, fgFabric.Columns[c].FieldName).Length == 0)
										{
											continue;
										}
										if (double.Parse(fgFabric.GetRowCellDisplayText(rFb, fgFabric.Columns[c].FieldName)) > 0 && strSize == fgFabric.Columns[c].FieldName && (rColor + 1) == int.Parse(fgFabric.GetRowCellDisplayText(rFb, "ColorID")))
										{
											tb.AddNewRow();
											CurrentRow = tb.FocusedRowHandle;
											tb.SetRowCellValue(CurrentRow, "PARENT", strModel + "-" + fgFabric.Columns[c].FieldName + "-" + strColor + "-C");
											tb.SetRowCellValue(CurrentRow, "CHILD", fgFabric.GetRowCellDisplayText(rFb, "Code"));
											tb.SetRowCellValue(CurrentRow, "QTY", fgFabric.GetRowCellDisplayText(rFb, fgFabric.Columns[c].FieldName));
											tb.SetRowCellValue(CurrentRow, "QTYDIV", "1");
										}
									}
								}
								//==========================================================================================================
							}
						}
					}
				}
				tb.UpdateCurrentRow();
			}
			catch (Exception ex)
			{
                MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
		}
		
		public void cmdGenCode_Click(System.Object sender, System.EventArgs e)
		{
			GenCode2();
			for (int i = 0; i <= fg.DataRowCount - 1; i++) //เช็คความยาว code ในกริด item master
			{
				ChkCodeLen(i, true);
			}
			for (int i = 0; i <= tb.DataRowCount - 1; i++) //เช็คความยาว code ในกริด bom
			{
				ChkCodeLen(i, false);
			}
		}
		
		#endregion
		
		#region Export To TPiCS(Item)
		
		private void ExportItemMaster_FDI()
		{
			string SqlStr;
            string strCorrectDate = DateTime.Now.ToString("yyMMddHHmmss", dtfinfo);

			for (int i = 0; i <= fg.DataRowCount - 1; i++)
			{
				if (fg.GetRowCellDisplayText(i, "ITEMCODE").Trim().Length > 25)
				{
					throw new ApplicationException("Can not Export to TPICS" + '\r' + "Length of Item Code is over 25 charecter in Item Code " + fg.GetRowCellDisplayText(i, "ITEMCODE"));
				}
				if (fg.GetRowCellDisplayText(i, "NAME").Trim().Length > 50)
				{
					throw new ApplicationException("Can not Export to TPICS" + '\r' + "Length of Item Name is over 52 charecter  in Item Code " + fg.GetRowCellDisplayText(i, "ITEMCODE"));
				}
			}
			
			for (int i = 0; i <= fg.DataRowCount - 1; i++)
			{
				if (fg.GetRowCellDisplayText(i, "ITEMCODE").Length > 0)
				{
					SqlStr = "select count(code) from XHEAD where code = \'" + fg.GetRowCellDisplayText(i, "ITEMCODE") + "\'";
					int intResult = int.Parse(db2.ExecuteFirstValue(SqlStr));
					if (intResult == 0)
					{
						SqlStr = "insert into XHEAD (code,name,MAINBUMO,TANI1,InputDate,InputUser,STYLE,SIZE,COLOR) values(" + "\'" + fg.GetRowCellDisplayText(i, "ITEMCODE").Trim() + "\'" + ",\'" + fg.GetRowCellDisplayText(i, "NAME").Trim() + "\'" + ",\'" + fg.GetRowCellDisplayText(i, "WCCODE").Trim() + "\'" + ",\'" + fg.GetRowCellDisplayText(i, "UNIT") + "\'" + "," + strCorrectDate + ",\'" + System.Environment.MachineName + "\'" + ",\'" + fg.GetRowCellDisplayText(i, "STYLE") + "\'" + ",\'" + fg.GetRowCellDisplayText(i, "SIZE") + "\'" + ",\'" + fg.GetRowCellDisplayText(i, "COLOR") + "\')";
						db2.Execute(SqlStr);
						SqlStr = "insert into XITEM (code,BUMO,VENDOR,HOKAN,FIXLEVEL,DKAKU,KAKU,KOUKI,LEAD" + ",PICKKU,KURIAGE,PKET,BUNR,InputDate,InputUser) values(" + "\'" + fg.GetRowCellDisplayText(i, "ITEMCODE").Trim() + "\'" + ",\'" + fg.GetRowCellDisplayText(i, "WCCODE") + "\'" + ",\'" + fg.GetRowCellDisplayText(i, "SUPCODE") + "\'" + ",\'" + fg.GetRowCellDisplayText(i, "STRGCODE") + "\'" + "," + fg.GetRowCellDisplayText(i, "DEPENDENT") + "," + fg.GetRowCellDisplayText(i, "RELEASE") + "," + fg.GetRowCellDisplayText(i, "FIX") + "," + fg.GetRowCellDisplayText(i, "MFG") + "," + fg.GetRowCellDisplayText(i, "DELIVERY") + "," + fg.GetRowCellDisplayText(i, "DISPATCHCLASS") + "," + fg.GetRowCellDisplayText(i, "ROUNDUP") + "," + fg.GetRowCellDisplayText(i, "DECIMALPOINT") + ",\'" + fg.GetRowCellDisplayText(i, "CLASSIFICATION") + "\'" + "," + strCorrectDate + "," + "\'" + System.Environment.MachineName + "\')";
						db2.Execute(SqlStr);
					}
				}
			}
		}
		
		public void cmdTranItem_Click(System.Object sender, System.EventArgs e)
		{
            this.Cursor = Cursors.WaitCursor;
            db2.ConnectionOpen();
            try
            {
                db2.BeginTrans();
                ExportItemMaster_FDI();
                db2.CommitTrans();
                MessageBox.Show("Export complete", "Export to item master", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SystemException ex)
            {
                db2.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                db2.RollbackTrans();
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            db2.ConnectionClose();
            this.Cursor = Cursors.Default;
		}
		
		#endregion
		
		#region Export To TPiCS(BOM)
		
		private void ExportBOM_FDI()
		{
			string SqlStr;
            string strCorrectDate = DateTime.Now.ToString("yyMMddHHmmss", dtfinfo);

			for (int i = 0; i <= tb.DataRowCount - 1; i++)
			{
				if (tb.GetRowCellDisplayText(i, "PARENT").Length > 0)
				{
					SqlStr = "select count(code) from XPRTS where code = \'" + tb.GetRowCellDisplayText(i, "PARENT").Trim() + "\' AND KCODE = \'" + tb.GetRowCellDisplayText(i, "CHILD").Trim() + "\'";
					int intResult = int.Parse(db2.ExecuteFirstValue(SqlStr));
					if (intResult == 0)
					{
						SqlStr = "insert into XPRTS (CODE,KCODE,SIYOU,SIYOUW,InputDate,InputUser) values (" + "\'" + tb.GetRowCellDisplayText(i, "PARENT").Trim() + "\'" + ",\'" + tb.GetRowCellDisplayText(i, "CHILD").Trim() + "\'" + "," + tb.GetRowCellDisplayText(i, "QTY") + "," + tb.GetRowCellDisplayText(i, "QTYDIV") + "," + strCorrectDate + ",\'" + System.Environment.MachineName + "\')";
						db2.Execute(SqlStr);
					}
				}
			}
		}
		
		public void cmdTranBOM_Click(System.Object sender, System.EventArgs e)
		{
            this.Cursor = Cursors.WaitCursor;
            db2.ConnectionOpen();
            try
            {
                db2.BeginTrans();
                ExportBOM_FDI();
                db2.CommitTrans();
                MessageBox.Show("Export complete", "Export to bom", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db2.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db2.ConnectionClose();
            this.Cursor = Cursors.Default;
		}
		
		#endregion


        private void ChkCodeLen(int iRow, bool blnIsGridItem) //ตรวจสอบว่าความยาวของ code ต้องไม่เกิน 25 ตัวอักษรก่อนนำเข้า TPiCS
        {
            if (blnIsGridItem) //ถ้าต้องการเช็คจากกริด Item Master
            {
                //Item Master
                if (fg.GetRowCellDisplayText(iRow, "ITEMCODE").Length > 25)
                {
                    fg.SetRowCellValue(iRow, "STATUS", false);
                }
                else
                {
                    fg.SetRowCellValue(iRow, "STATUS", true);
                }
            }
            else //ถ้าต้องการเช็คจากกริด BOM
            {
                //Bom
                if ((tb.GetRowCellDisplayText(iRow, "PARENT").Length > 25) || (tb.GetRowCellDisplayText(iRow, "CHILD").Length > 25))
                {
                    tb.SetRowCellValue(iRow, "STATUS", false);
                }
                else
                {
                    tb.SetRowCellValue(iRow, "STATUS", true);
                }
            }
        }
        private void FindFabric(string strName)
        {
            string cmdSql = "SELECT Code,WC FROM TpicsFabric WHERE Name = \'" + strName + "\'";
            DataTable DT;
            try
            {
                DT = db.GetDataTable(cmdSql);
                if (DT.Rows.Count > 0)
                {
                    fgFabric.SetFocusedRowCellValue(fgFabric.Columns["Code"], DT.Rows[0]["Code"].ToString());
                    fgFabric.SetFocusedRowCellValue(fgFabric.Columns["WC"], DT.Rows[0]["WC"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

		private void FrmDeclare_Load(object sender, System.EventArgs e)
		{
            db = new cDatabase(_connectionString);
            db2 = new cDatabase(_connectionString2);
			try
			{
				this.Cursor = Cursors.WaitCursor;
                dtfinfo = clinfo.DateTimeFormat;
				txtModel.Text = "";
				cboModel.Text = "";
				cboDivision.SelectedIndex = 3; //Others
				txtName.Text = "";
				LoadRepositoryItem(); //โหลด repository เก็บไว้ในตัวแปร
				DataTable DT;
				//แมพ repository เข้ากับ gridsize
				DT = new DataTable();
				DT.Columns.Add("Name", typeof(string));
				GridSize.DataSource = DT;
				fgSize.Columns["Name"].ColumnEdit = rpSize;
				//แมพ repository เข้ากับ gridcolor
				DT = new DataTable();
				DT.Columns.Add("Name", typeof(string));
				DT.Columns.Add("FullName", typeof(string));
				GridColor.DataSource = DT;
				fgColor.Columns["Name"].ColumnEdit = rpColor;
				//Call MapRepositoryItem(GridSize, fgSize, "Name", rpSize)  'แมพ repository เข้ากับ gridsize
				//Call MapRepositoryItem(GridColor, fgColor, "Name", rpColor)   'แมพ repository เข้ากับ gridcolor
				PrepareGrid(); //เซตคอลัมน์ให้กับกริด fg,tb
			}
			catch(Exception ex)
			{
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			this.Cursor = Cursors.Default;
		}

		private void cmdClearFG_Click(System.Object sender, System.EventArgs e)
		{
			ClearGrid(fg);
			ClearGrid(tb);
		}
		private void cmdDisplay_Click(System.Object sender, System.EventArgs e)
		{
			if (cboModel.Text.Length > 0){RetriveData();}
		}
		private void cmdFreeze_Click(System.Object sender, System.EventArgs e)
		{
			if (strFixCol != null)
			{
				fg.Columns[strFixCol].Fixed = FixedStyle.None;
			}
			fg.Columns[fg.FocusedColumn.FieldName].Fixed = FixedStyle.Left;
			strFixCol = fg.FocusedColumn.FieldName;
		}
		private void cmdTable_Click(System.Object sender, System.EventArgs e)
		{
			ClearGrid(fgFabric);
			fgFabric.Columns.Clear();
			DataTable DT = new DataTable();
			DT.BeginInit();
			DT.Columns.Add("Name", typeof(string));
			DT.Columns.Add("Code", typeof(string));
			DT.Columns.Add("WC", typeof(string));
			DT.Columns.Add("ColorID", typeof(string));
			for (int i = 0; i <= fgSize.DataRowCount - 1; i++)
			{
				DT.Columns.Add(fgSize.GetRowCellDisplayText(i, "Name"), typeof(string));
			}
			DT.EndInit();
			GridFabric.DataSource = DT;
			GridFabric.RepositoryItems.Add(rpFabric);
			fgFabric.Columns["Name"].ColumnEdit = rpFabric;
		}
		private void fgSize_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
		{
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            fgSize.IndicatorWidth = 30;
		}
		private void fgColor_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
		{
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            fgColor.IndicatorWidth = 30;
		}
        private void fgFabric_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            fgFabric.IndicatorWidth = 30;
        }
        private void fg_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            fg.IndicatorWidth = 30;
        }
        private void tb_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            tb.IndicatorWidth = 30;
        }
		private void GridSize_ProcessGridKey(object sender, System.Windows.Forms.KeyEventArgs e)
		{
            if (e.KeyCode == Keys.Delete)
            {
                if (fgSize.IsEditing == false)
                {
                    try
                    {
                        if (MessageBox.Show("คุณต้องลบ Size:" + fgSize.GetFocusedRowCellDisplayText("Name") + " จาก Model:" + txtModel.Text+" ใช่หรือไม่ "
                            , "Delete size", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        {
                            //db.ConnectionOpen();
                            //string strSQL = "DELETE FROM TPICSSIZE WHERE MODEL='" + txtModel.Text + "' AND NAME='" + fgSize.GetFocusedRowCellDisplayText("Name")+"'";
                            //db.Execute(strSQL);
                            //db.ConnectionClose();
                            fgSize.DeleteSelectedRows();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                }
            }
		}
		private void GridColor_ProcessGridKey(object sender, System.Windows.Forms.KeyEventArgs e)
		{
            if (e.KeyCode == Keys.Delete)
            {
                if (fgColor.IsEditing == false)
                {
                    try
                    {
                        if (MessageBox.Show("คุณต้องลบ Color:" + fgColor.GetFocusedRowCellDisplayText("Name") + " จาก Model:" + txtModel.Text + " ใช่หรือไม่ "
                            , "Delete color", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        {
                            //db.ConnectionOpen();
                            //string strSQL = "DELETE FROM TPICSCOLOR WHERE MODEL='" + txtModel.Text + "' AND NAME='" + fgColor.GetFocusedRowCellDisplayText("Name") + "'";
                            //db.Execute(strSQL);
                            //db.ConnectionClose();
                            fgColor.DeleteSelectedRows();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
		}
        private void GridFabric_ProcessGridKey(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (fgFabric.IsEditing == false)
                {
                    fgFabric.DeleteSelectedRows();
                }
            }
        }
		private void fg_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
		{
            if (fg.FocusedColumn.FieldName == "STATUS")
            {
                e.Cancel = true;
            }
		}
        private void tb_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tb.FocusedColumn.FieldName == "STATUS")
            {
                e.Cancel = true;
            }
        }
        private void fg_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            ChkCodeLen(fg.FocusedRowHandle, true);
        }
        private void tb_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            ChkCodeLen(tb.FocusedRowHandle, false);
        }
		private void fgFabric_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
		{
            if (fgFabric.FocusedColumn.FieldName == "Name")
            {
                if (e.Value != null)
                {
                    FindFabric(e.Value.ToString());
                }
            }
		}
        private void frmDeclare_Shown(object sender, EventArgs e)
        {

        }
        private void fgFabric_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            { 
                GridView gv=(GridView)sender;
                if (gv.IsRowSelected(e.RowHandle)) popupMenu1.ShowPopup(new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y));
            }
        }
        private void bbiCopy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            myArray = new object[fgFabric.Columns.Count];
            for (int i = 0; i < fgFabric.Columns.Count; i++)
            {
                myArray[i] = fgFabric.GetRowCellValue(fgFabric.FocusedRowHandle, fgFabric.Columns[i].FieldName);
            }
        }
        private void bbiPaste_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                fgFabric.AddNewRow();
                for (int i = 0; i <= myArray.GetUpperBound(0); i++)
                {
                    fgFabric.SetRowCellValue(fgFabric.FocusedRowHandle,fgFabric.Columns[i],myArray[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void bbiDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fgFabric.DeleteRow(fgFabric.FocusedRowHandle);
        }
    }
}