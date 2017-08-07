using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System.AC
{
    public partial class frmAC_Cust : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        DataTable dtCustNo;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmAC_Cust()
        {
            InitializeComponent();
        }
        public void ClearData(bool allControls)
        {
            if (allControls)
            {
                sleCustNo.EditValueChanged -= sleCustNo_EditValueChanged;
                sleCustNo.EditValue = null;
                sleCustNo.EditValueChanged += sleCustNo_EditValueChanged;
            }

            textEdit2.Text = "";
            textEdit3.Text = "";
            textEdit4.Text = "";
            textEdit5.Text = "";
            textEdit6.Text = "";
            textEdit7.Text = "";
            textEdit8.Text = "";
            textEdit9.Text = "";
            textEdit10.Text = "";
        }
        public void DeleteData()
        {
            if (MessageBox.Show("Do you want to delete this customer no. " + sleCustNo.Text + " ?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                string strSQL = "delete from customeracc where cust_no = N'" + sleCustNo.Text + "'";
                db.Execute(strSQL);
                db.CommitTrans();
                sleCustNo.Properties.View.DeleteSelectedRows();
                sleCustNo.EditValue = null;
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
                string strSQL = "select count(cust_no) from customeracc where cust_no='" + sleCustNo.Text + "'";
                if (db.ExecuteFirstValue(strSQL) == "0")
                {
                    strSQL = "insert into customeracc (cust_no,code,cust_name,cus_add1,cus_add2,cus_add3,custnamee,cusadde1,cusadde2,cusadde3)"+
                        " values (N'"+ sleCustNo.Text + "',N'" + textEdit2.Text + "',N'" + textEdit3.Text + "'" +
                        ",N'" + textEdit4.Text + "',N'" + textEdit5.Text + "',N'" + textEdit6.Text + "',N'" + textEdit7.Text + "'" +
                        ",N'" + textEdit8.Text + "',N'" + textEdit9.Text + "',N'" + textEdit10.Text + "')";
                    db.Execute(strSQL);
                }
                else
                {
                    strSQL = "update customeracc set code = N'" + textEdit2.Text + "',cust_name = N'" + textEdit3.Text + "'" +
                        ",cus_add1 = N'" + textEdit4.Text + "',cus_add2 = N'" + textEdit5.Text + "',cus_add3 = N'" + textEdit6.Text + "'" +
                        ",custnamee = N'" + textEdit7.Text + "',cusadde1 = N'" + textEdit8.Text + "',cusadde2 = N'" + textEdit9.Text + "'" +
                        ",cusadde3 = N'" + textEdit10.Text + "' where cust_no = N'" + sleCustNo.Text + "'";
                    db.Execute(strSQL);
                }
                db.CommitTrans();
                MessageBox.Show("Save complete","Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
        }
    
        private DataTable GetCustomers()
        {
            string strSQL="select distinct cust_no,cust_name from customeracc order by cust_no";
            dtCustNo=db.GetDataTable(strSQL);
            return dtCustNo;
        }
        private void GetCustomerDetail(string strCustNo)
        {
            string strSQL = "select * from customeracc where cust_no=N'" + sleCustNo.Text + "'";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                textEdit2.Text = dr["code"].ToString();
                textEdit3.Text = dr["cust_name"].ToString();
                textEdit4.Text = dr["cus_add1"].ToString();
                textEdit5.Text = dr["cus_add2"].ToString();
                textEdit6.Text = dr["cus_add3"].ToString();
                textEdit7.Text = dr["custnamee"].ToString();
                textEdit8.Text = dr["cusadde1"].ToString();
                textEdit9.Text = dr["cusadde2"].ToString();
                textEdit10.Text = dr["cusadde3"].ToString();
            }

        }

        private void frmAC_Cust_Load(object sender, EventArgs e)
        {
            db=new cDatabase(_connectionString);

            sleCustNo.Properties.DataSource = GetCustomers();
            sleCustNo.Properties.DisplayMember = "cust_no";
            sleCustNo.Properties.ValueMember = "cust_no";
            sleCustNo.Properties.View.OptionsView.ColumnAutoWidth = true;
        }
        private void textEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) textEdit2.Focus();
        }
        private void textEdit2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) textEdit3.Focus();
        }
        private void textEdit3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) textEdit4.Focus();
        }
        private void textEdit4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) textEdit5.Focus();
        }
        private void textEdit5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) textEdit6.Focus();
        }
        private void textEdit6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) textEdit7.Focus();
        }
        private void textEdit7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) textEdit8.Focus();
        }
        private void textEdit8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) textEdit9.Focus();
        }
        private void textEdit9_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) textEdit10.Focus();
        }
        private void sleCustNo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ClearData(false);
                GetCustomerDetail(sleCustNo.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string strVal="";
            if(myClass.cUtility.InputBox("Add","Cust No.",ref strVal)==DialogResult.OK)
            {
                //sleCustNo.EditValueChanged -= sleCustNo_EditValueChanged;
                dtCustNo.BeginInit();
                DataRow dr = dtCustNo.NewRow();
                dr["cust_no"] = strVal;
                dtCustNo.Rows.Add(dr);
                dtCustNo.EndInit();
                sleCustNo.EditValue = strVal;

            }
            

            
            
        }
     
    }
}