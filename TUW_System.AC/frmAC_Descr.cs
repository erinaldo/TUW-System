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
    public partial class frmAC_Descr : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        DataTable dtDescr;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmAC_Descr()
        {
            InitializeComponent();
        }
        public void ClearData(bool allControls)
        {
            if (allControls)
            {
                sleDesc.EditValueChanged -= sleDesc_EditValueChanged;
                sleDesc.EditValue = null;
                sleDesc.EditValueChanged += sleDesc_EditValueChanged;
            }
            txtCode.Text = "";
        }
        public void DeleteData()
        {
            if (MessageBox.Show("Do you want to delete this description " + sleDesc.Text + " ?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                string strSQL = "delete from domesticdesc where descr = N'" + sleDesc.Text + "'";
                db.Execute(strSQL);
                db.CommitTrans();
                sleDesc.Properties.View.DeleteSelectedRows();
                sleDesc.EditValue = null;
                MessageBox.Show("Delete complete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
        }
        public void SaveData()
        {
            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                string strSQL = "select count(descr) from domesticdesc where descr='" + sleDesc.Text + "'";
                if (db.ExecuteFirstValue(strSQL) == "0")
                {
                    strSQL = "insert into domesticdesc (descr,desccode) values ('" + sleDesc.Text + "','" + txtCode.Text + "')";
                    db.Execute(strSQL);
                }
                else
                {
                    strSQL = "update domesticdesc set desccode = '" + txtCode.Text + "' where descr = '" + sleDesc.Text + "'";
                    db.Execute(strSQL);
                }
                db.CommitTrans();
                MessageBox.Show("Save complete", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
        }

        private DataTable GetDescriptions()
        {
            string strSQL = "select descr,descCode from domesticdesc order by descr";
            dtDescr = db.GetDataTable(strSQL);
            return dtDescr;
        }
        private void GetDescriptionDetail(string strDescr)
        {
            string strSQL = "select * from domesticdesc where descr = '" + sleDesc.Text + "'";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                txtCode.Text = dr["desccode"].ToString();
            }
        }

        private void frmAC_Descr_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            sleDesc.Properties.DataSource = GetDescriptions();
            sleDesc.Properties.DisplayMember = "descr";
            sleDesc.Properties.ValueMember = "descr";
            sleDesc.Properties.View.OptionsView.ColumnAutoWidth = true;
        }

        private void sleDesc_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ClearData(false);
                GetDescriptionDetail(sleDesc.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string strVal = "";
            if (myClass.cUtility.InputBox("Add", "Description", ref strVal) == DialogResult.OK)
            {
                dtDescr.BeginInit();
                DataRow dr = dtDescr.NewRow();
                dr["descr"] = strVal;
                dtDescr.Rows.Add(dr);
                dtDescr.EndInit();
                sleDesc.EditValue = strVal;
            }
        }

    }

}