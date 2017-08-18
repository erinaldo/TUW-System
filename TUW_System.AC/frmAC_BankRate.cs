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
    public partial class frmAC_BankRate : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        bool flagEntered;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmAC_BankRate()
        {
            InitializeComponent();
        }
        public void ClearData(bool allControls)
        {
            if (allControls)
            {
                cboYear.Text = "";
            }
            txtPeriod.Text = "";
            txtUSD.Text = "";
            txtYEN.Text = "";
            txtSGD.Text = "";
            txtEUR.Text = "";
        }
        public void SaveData()
        {
            //if (txtPeriod.Text.Length == 0)
            //{
            //    MessageBox.Show("Please input period: yyyyMM-yyyyMM", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                string strSQL = "select count(*) from moneyrate where seq = 0 and rateyear = '" + cboYear.Text + "'";
                if (db.ExecuteFirstValue(strSQL) == "0")
                {
                    strSQL = "insert into moneyrate (seq,rateyear,usrates,yenrates,sgrates,eurrates,period) values (" +
                        "0,'" + cboYear.Text + "'";
                    strSQL += (txtUSD.Text.Length > 0) ? "," + txtUSD.Text : ",0";
                    strSQL += (txtYEN.Text.Length > 0) ? "," + txtYEN.Text : ",0";
                    strSQL += (txtSGD.Text.Length > 0) ? "," + txtSGD.Text : ",0";
                    strSQL += (txtEUR.Text.Length > 0) ? "," + txtEUR.Text : ",0";
                    strSQL += (txtPeriod.Text.Length>0)? ",'" + txtPeriod.Text + "')":",'')";
                    db.Execute(strSQL);
                }
                else
                {
                    strSQL = "update moneyrate set ";
                    strSQL += (txtUSD.Text.Length > 0) ? "usrates=" + txtUSD.Text : "usrates=0";
                    strSQL += (txtYEN.Text.Length > 0) ? ",yenrates=" + txtYEN.Text : ",yenrates=0";
                    strSQL += (txtSGD.Text.Length > 0) ? ",sgrates=" + txtSGD.Text : ",sgrates=0";
                    strSQL += (txtEUR.Text.Length > 0) ? ",eurrates=" + txtEUR.Text : ",eurrates=0";
                    strSQL += ",period='" + txtPeriod.Text + "'" +
                        " where seq = 0 and rateyear ='" + cboYear.Text + "'";
                    db.Execute(strSQL);
                }
                db.CommitTrans();
                MessageBox.Show("Save complete.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
        }

        private void GetRateDetail(string strYear)
        {
            string strSQL = "select * from moneyrate where seq = 0 and rateyear = '" + strYear + "'";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                txtPeriod.Text = dr["Period"].ToString();
                txtUSD.EditValue = dr["usrates"];
                txtYEN.EditValue = dr["yenrates"];
                txtSGD.EditValue = dr["sgrates"];
                txtEUR.EditValue = dr["eurrates"];
            }
        }

        private void frmAC_BankRate_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            string strSQL = "select distinct rateyear from moneyrate where seq = 0 order by rateyear";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                cboYear.Properties.Items.Add(dr["rateyear"]);
            }
        }
        private void cboYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ClearData(false);
                GetRateDetail(cboYear.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtUSD_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtYEN.Focus();
        }
        private void txtYEN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtSGD.Focus();
        }
        private void txtSGD_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtEUR.Focus();
        }
        private void txtUSD_Leave(object sender, EventArgs e)
        {
            flagEntered = false;
        }
        private void txtYEN_Leave(object sender, EventArgs e)
        {
            flagEntered = false;
        }
        private void txtSGD_Leave(object sender, EventArgs e)
        {
            flagEntered = false;
        }
        private void txtEUR_Leave(object sender, EventArgs e)
        {
            flagEntered = false;
        }
        private void txtUSD_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab) flagEntered = true;
        }
        private void txtYEN_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab) flagEntered = true;
        }
        private void txtSGD_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab) flagEntered = true;
        }
        private void txtEUR_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab) flagEntered = true;
        }
        private void txtUSD_MouseUp(object sender, MouseEventArgs e)
        {
            if (txtUSD.SelectedText == "" && !flagEntered)
            {
                txtUSD.SelectAll();
                flagEntered = true;
            }
        }
        private void txtYEN_MouseUp(object sender, MouseEventArgs e)
        {
            if (txtYEN.SelectedText == "" && !flagEntered)
            {
                txtYEN.SelectAll();
                flagEntered = true;
            }
        }
        private void txtSGD_MouseUp(object sender, MouseEventArgs e)
        {
            if (txtSGD.SelectedText == "" && !flagEntered)
            {
                txtSGD.SelectAll();
                flagEntered = true;
            }
        }
        private void txtEUR_MouseUp(object sender, MouseEventArgs e)
        {
            if (txtEUR.SelectedText == "" && !flagEntered)
            {
                txtEUR.SelectAll();
                flagEntered = true;
            }
        }

    }
}