using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System.ProductionOrder
{
    public partial class frmP_Model : DevExpress.XtraEditors.XtraForm
    {
        public delegate void LoadfrmP_ModelCategoryHandler();
        public event LoadfrmP_ModelCategoryHandler LoadfrmP_ModelCategoryEvent;
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;
        
        cDatabase db;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmP_Model()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            foreach (Control ctl in layoutControl1.Controls)
            {
                if (ctl is DevExpress.XtraEditors.TextEdit) { ctl.Text = ""; }
                cboCategory.Text = "";
                sleItem.Text = "";
            }           
        }
        public void DisplayData()
        {
            string strSQL = "SELECT A.MID,A.MODEL,A.MODEL_TUW,A.ARTICLE,A.CATEGORY,A.DID,B.ITEM,A.INPUTDATE,A.INPUTUSER FROM XMODEL A LEFT OUTER JOIN XITEM_DESC B ON A.DID=B.DID ORDER BY MODEL";
            DataTable dt = db.GetDataTable(strSQL);
            //gridControl1.DataSource = null;
            gridControl1.DataSource = dt;
            gridView1.Columns["MID"].Caption = "ID";
            gridView1.Columns["MODEL"].Caption = "Model";
            gridView1.Columns["MODEL_TUW"].Caption = "Model_TUW";
            gridView1.Columns["ARTICLE"].Caption = "Article";
            gridView1.Columns["CATEGORY"].Caption = "Category";
            gridView1.Columns["DID"].Visible = false;
            gridView1.Columns["ITEM"].Caption = "Item";
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            StatusBarEvent(gridView1.DataRowCount + " Rows");
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
                    strSQL = "SELECT CASE WHEN MAX(MID)IS NULL THEN 'M000001' ELSE 'M'+RIGHT('000000'+LTRIM(STR(RIGHT(MAX(MID),6)+1)),6) END FROM XMODEL";
                    string strNewID = db.ExecuteFirstValue(strSQL);
                    strSQL = "INSERT INTO XMODEL(MID,MODEL,MODEL_TUW,ARTICLE,CATEGORY,DID,INPUTUSER)VALUES(";
                    strSQL += "'" + strNewID + "','" + txtModel.Text.Replace("'","''") + "','" + txtModel_TUW.Text.Replace("'","''") + "','" + txtArticle.Text.Replace("'", "''") + "','" + cboCategory.Text +
                        "','" + sleItem.EditValue.ToString() + "','" + System.Environment.MachineName + "')";
                    db.Execute(strSQL);
                }
                else
                {
                    strSQL = "UPDATE XMODEL SET " +
                        "MODEL='" + txtModel.Text.Replace("'","''") + "'," +
                        "MODEL_TUW='" + txtModel_TUW.Text.Replace("'","''") + "'," +
                        "ARTICLE='" + txtArticle.Text + "'," +
                        "CATEGORY='" + cboCategory.Text + "'," +
                        "DID='" + sleItem.EditValue.ToString() + "'," +
                        "INPUTDATE=GETDATE(),INPUTUSER='" + System.Environment.MachineName + "' " +
                        "WHERE MID='" + txtID.Text + "'";
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
        private void LoadItemDescription()
        {
            string strSQL = "SELECT DID AS ID,ITEM FROM XITEM_DESC ORDER BY ITEM";
            DataTable dt=db.GetDataTable(strSQL);
            sleItem.Properties.DataSource = dt;
            sleItem.Properties.DisplayMember = "ITEM";
            sleItem.Properties.ValueMember = "ID";
            sleItem.Properties.View.OptionsView.EnableAppearanceEvenRow = true;
            sleItem.Properties.View.OptionsView.EnableAppearanceOddRow = true;
            sleItem.Properties.View.OptionsView.ColumnAutoWidth = false;
            sleItem.Properties.View.BestFitColumns();
        }

        private void frmModel_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            try
            {
                this.Cursor = Cursors.WaitCursor;
                LoadItemDescription();
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
                txtID.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "MID");
                txtModel.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "MODEL");
                txtModel_TUW.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "MODEL_TUW");
                txtArticle.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "ARTICLE");
                cboCategory.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "CATEGORY");
                sleItem.EditValue= gridView1.GetRowCellDisplayText(e.RowHandle, "DID");
            }
            catch (Exception){}
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            LoadfrmP_ModelCategoryEvent();
        }
        private void btnItem_refresh_Click(object sender, EventArgs e)
        {

        }

    }
}