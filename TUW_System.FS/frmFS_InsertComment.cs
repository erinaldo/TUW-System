using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;
using Microsoft.Office.Interop.Excel;

namespace TUW_System.FS
{
    public partial class frmFS_InsertComment : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;

        cDatabase db;
        System.Data.DataTable dtMain;
        List<Int32> lstRowSave=new List<Int32>();//เอาไว้เก็บว่าแถวไหนที่ควารต้องเซฟบ้าง

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmFS_InsertComment()
        {
            InitializeComponent();
        }
        public void NewData()
        { 
            chkLotNo.Checked = false;
            txtLotNo.Text = "";
            chkFabricCode.Checked = false;
            txtFabricCode.Text = "";
            chkColor.Checked = false;
            txtColor.Text = "";
            txtBarcode.Text = "";
            gridControl1.DataSource = null;
            dtMain = null;
        }
        public void DisplayData()
        {
            if (!chkLotNo.Checked && !chkFabricCode.Checked && !chkColor.Checked) { return; }
            try
            {
                this.Cursor = Cursors.WaitCursor;
                string strLotNo, strFabricCode, strColor;
                if (chkLotNo.Checked == true) { strLotNo = txtLotNo.Text; } else { strLotNo = null; }
                if (chkFabricCode.Checked == true) { strFabricCode = txtFabricCode.Text; } else { strFabricCode = null; }
                if (chkColor.Checked == true) { strColor = txtColor.Text; } else { strColor = null; }
                DisplayData(strLotNo, strFabricCode, strColor);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        public void SaveData()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                db.ConnectionOpen();
                db.BeginTrans();
                foreach(int i in lstRowSave)
                {
                    string strSQL = "Update FinishFabricNC Set SystemRemark=N'" + gridView1.GetRowCellDisplayText(i,"Comment")+ "'"+
                        ",Booking=N'" + gridView1.GetRowCellDisplayText(i,"Booking_Date")+"'"+
                        " Where Code='" +gridView1.GetRowCellDisplayText(i,"Fabric_Code")+"' And Serial='" + gridView1.GetRowCellDisplayText(i,"Serial")+"'";
                    db.Execute(strSQL);
                }
                db.CommitTrans();
                lstRowSave.Clear();
                MessageBox.Show("Save complete...", "InsertFabricComment", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.ConnectionClose();
            }
            this.Cursor = Cursors.Default;
            //btnSearch_Click(sender, e);
        }

        private string FindFabricCode(string strID)
        {
            string strSQL = "SELECT CODE FROM GREYFABRIC WHERE ID='" + strID + "'";
            return db.ExecuteFirstValue(strSQL);
        }
        private void DisplayData(string strLotNo,string strFabricCode,string strColor)
        {
            string strSQL = "Select Top 10000 LotNo,Code as Fabric_Code,ColorNo,Serial,PieceNo as Piece_No,Qty,Unit" +
                ",SystemRemark as Comment,Booking as Booking_Date,Price From FinishFabricNC Where SysDelete=0 ";
            if (strLotNo != null) { strSQL += "And LotNo Like '" + strLotNo + "%' "; }
            if (strFabricCode != null) { strSQL += "And Code Like '" + strFabricCode + "%' "; }
            if (strColor != null) { strSQL += "And ColorNo Like '" + strColor + "%' "; }
            dtMain = db.GetDataTable(strSQL);
            gridControl1.DataSource = null;
            gridControl1.DataSource = dtMain;
            gridView1.PopulateColumns();
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gridView1.Columns)
            {
                dc.OptionsColumn.AllowEdit = false;
            }
            gridView1.Columns["Comment"].OptionsColumn.AllowEdit = true;
            gridView1.Columns["Booking_Date"].OptionsColumn.AllowEdit = true;
            lstRowSave.Clear();
            StatusBarEvent(gridView1.RowCount.ToString() + " row(s) affected");
            //StatusLabel1.Text = gridView1.Rows.Count.ToString() + " row(s) affected";
        }
        private void DisplayData2(string strBarcode)//ใช้สำหรับช่อง scan barcode
        {
            string strSQL = "Select Top 10000 LotNo,Code as Fabric_Code,ColorNo,Serial,PieceNo as Piece_No,Qty,Unit" +
                ",SystemRemark as Comment,Booking as Booking_Date,Price " +
                "From FinishFabricNC Where SysDelete=0 ";
            if (strBarcode.Length == 8)
            {
                strSQL += "And Code='" + FindFabricCode(strBarcode.Substring(0, 5)) + "' And Serial='" + strBarcode.Substring(5, 3) + "'";
            }
            else
            {
                strSQL += "And Code='" + FindFabricCode(strBarcode.Substring(0, 5)) + "' And Serial='" + strBarcode.Substring(5, 4) + "'";
            }
            if (gridControl1.DataSource == null)
            {
                dtMain = db.GetDataTable(strSQL);
            }
            else
            {
                System.Data.DataTable dt = db.GetDataTable(strSQL);
                DataRow dr = dtMain.NewRow();
                dr.ItemArray = dt.Rows[0].ItemArray;
                dtMain.Rows.InsertAt(dr, dtMain.Rows.Count);
            }
            gridControl1.DataSource = dtMain;
            gridView1.PopulateColumns();
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            foreach (DevExpress.XtraGrid.Columns.GridColumn dc in gridView1.Columns)
            {
                dc.OptionsColumn.AllowEdit = false;
            }
            gridView1.Columns["Comment"].OptionsColumn.AllowEdit = true;
            gridView1.Columns["Booking_Date"].OptionsColumn.AllowEdit = true;
            StatusBarEvent(gridView1.RowCount.ToString() + " row(s) affected");
            //StatusLabel1.Text = gridView1.Rows.Count.ToString() + " row(s) affected";
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            gridView1.IndicatorWidth = 30;
        }
        private void txtLotNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                DisplayData();
            }
        }
        private void txtFabricCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                DisplayData();
            }
        }
        private void txtColor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                DisplayData();
            }
        }
        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)13) { return; }
            try
            {
                db.ConnectionOpen();
                DisplayData2(txtBarcode.Text);
            }
            catch
            {
                //MessageBox.Show(ex.Message, "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.ConnectionClose();
            }
            txtBarcode.Text = "";
        }
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "Comment"||e.Column.FieldName=="Booking_Date")
            {
                
                //e.Column.AppearanceCell.BackColor = Color.LightGreen;
                lstRowSave.Add(e.RowHandle);
            }
        }
        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                string strFieldName=gridView1.FocusedColumn.FieldName;
                if (strFieldName == "Comment" || strFieldName == "Booking_Date")
                {
                    try
                    {
                        gridView1.SetFocusedRowCellValue(strFieldName,gridView1.GetRowCellValue(gridView1.FocusedRowHandle-1,strFieldName));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void frmFS_InsertComment_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
        }

    }
}