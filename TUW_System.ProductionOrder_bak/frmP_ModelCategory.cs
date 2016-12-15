using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System
{
    public partial class frmP_ModelCategory : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db = new cDatabase(Module.Sale);

        public frmP_ModelCategory()
        {
            InitializeComponent();
        }

        public void NewData()
        {
            foreach (Control ctl in layoutControl1.Controls)
            {
                if (ctl is DevExpress.XtraEditors.TextEdit) { ctl.Text = ""; }
            }
        }
        public void DisplayData()
        {
            string strSQL = "SELECT * FROM XITEM_DESC ORDER BY ITEM";
            DataTable dt = db.GetDataTable(strSQL);
            gridControl1.DataSource = null;
            gridControl1.DataSource = dt;
            gridView1.Columns["DID"].Caption = "ID";
            gridView1.Columns["ITEM"].Caption = "Item name";
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
        }
        public void SaveData()
        {
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                string strSQL;
                //----------------------Save data--------------------------------------
                if (txtID.Text.Length == 0)
                {
                    strSQL = "SELECT CASE WHEN MAX(DID)IS NULL THEN 'D000001' ELSE 'D'+RIGHT('000000'+LTRIM(STR(RIGHT(MAX(DID),6)+1)),6) END FROM XITEM_DESC";
                    string strNewID = db.ExecuteFirstValue(strSQL);
                    strSQL = "INSERT INTO XITEM_DESC(DID,ITEM,INPUTUSER)VALUES(";
                    strSQL += "'" + strNewID + "','" + txtItem.Text.Replace("'", "''") + "','" + System.Environment.MachineName + "')";
                    db.Execute(strSQL);
                }
                else
                {
                    strSQL = "UPDATE XITEM_DESC SET " +
                        "ITEM='" + txtItem.Text.Replace("'", "''") + "'," +
                        "INPUTDATE=GETDATE(),INPUTUSER='" + System.Environment.MachineName + "' " +
                        "WHERE DID='" + txtID.Text + "'";
                    db.Execute(strSQL);
                }
                //-----------------------------------------------------------------------
                db.CommitTrans();
                MessageBox.Show("Save complete...", "Production Order", MessageBoxButtons.OK, MessageBoxIcon.Information);

                NewData();
                DisplayData();
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
        }        

        private void frmItemDescripton_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                DisplayData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                txtID.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "DID");
                txtItem.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "ITEM");
            }
            catch (Exception){}
        }
    }
}