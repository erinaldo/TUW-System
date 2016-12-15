using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.Globalization;
using myClass;

namespace TUW_System
{
    public partial class frmS5_POSupplier : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db=new cDatabase(Module.TUW99);
        //cDatabase db_TPiCS=new cDatabase(Module.Fabric);
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        string strSupplierID;
        List<string> countries;
        
        public frmS5_POSupplier()
        {
            InitializeComponent();
        }

        //private void Displaydata()
        //{
        //    //Dim strSQL As String = "SELECT ID,NAME,ADDRESS1,ADDRESS2,COUNTRY,TEL,FAX,PAYMENT FROM FabricSupplier WHERE NAME=N'" & cboSupplier.Text & "'"
        //    string strSQL = "SELECT BUMO,NAME,ADR1,ADR2,ZIP,COUNTRY,MAIL,TEL,FAX,PAYTERM,CURRE,HITO FROM XSECT WHERE NAME='"+ cboSupplier.Text + "'";
        //    DataTable dt = db_TPiCS.GetDataTable(strSQL);
        //    ClearTextBox();
        //    foreach(DataRow dr in dt.Rows)
        //    {
        //        txtSupplierID.Text = dr["BUMO"].ToString();
        //        cboSupplier.Text = dr["NAME"].ToString();
        //        txtAD1.Text = dr["ADR1"].ToString();
        //        txtAD2.Text = dr["ADR2"].ToString();
        //        txtZip.Text = dr["ZIP"].ToString();
        //        txtCountry.Text = dr["COUNTRY"].ToString();
        //        txtMail.Text = dr["MAIL"].ToString();
        //        txtTel.Text = dr["TEL"].ToString();
        //        txtFax.Text = dr["FAX"].ToString();
        //        txtPayment.Text = dr["PAYTERM"].ToString();
        //        cboCur.SelectedIndex = cboCur.Properties.Items.IndexOf(dr["CURRE"].ToString());
        //        txtPerson.Text = dr["HITO"].ToString();
        //    }
        //}
        private void SaveData()
        {
            if (txtSupplier.Text.Length==0)
            {
                MessageBox.Show("โปรดระบุ Supplier Name", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (luePayterm.EditValue == null)
            { 
                MessageBox.Show("โปรดเลือก Payment Term", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            db.ConnectionOpen();
            string strSQL;
            if (sleSupplierID.EditValue == null)
            {
                strSQL = "EXEC spTUWSystem_RunNo 'PO_SUPPLIER',''";
                strSupplierID = db.ExecuteFirstValue(strSQL);
            }
            else
            {
                strSupplierID = sleSupplierID.EditValue.ToString();
            }
            try
            {
                db.BeginTrans();
                strSQL = "UPDATE PO_SUPPLIER SET NAME=N'"+txtSupplier.Text.Replace("'","''")+"'";
                strSQL += (txtAD1.Text.Length == 0) ? ",ADDRESS1=''" : ",ADDRESS1=N'" + txtAD1.Text.Replace("'","''") + "'";
                strSQL += (txtAD2.Text.Length == 0) ? ",ADDRESS2=''" : ",ADDRESS2=N'" + txtAD2.Text.Replace("'","''") + "'";
                strSQL += (txtZip.Text.Length == 0) ? ",ZIP=NULL" : ",ZIP='" + txtZip.Text + "'";
                strSQL += (lueCountry.EditValue==null) ? ",COUNTRY=NULL" : ",COUNTRY=N'" + lueCountry.EditValue.ToString().Replace("'","''") + "'";
                strSQL += (txtTel.Text.Length == 0) ? ",TELEPHONE=''" : ",TELEPHONE='" + txtTel.Text + "'";
                strSQL += (txtFax.Text.Length == 0) ? ",FAX=''" : ",FAX='" + txtFax.Text + "'";
                strSQL += (txtMail.Text.Length == 0) ? ",MAIL=NULL" : ",MAIL='" + txtMail.Text + "'";
                strSQL += ",IDPAYMENT='" + luePayterm.EditValue.ToString() + "'";
                strSQL += ",SECTION='FABRIC CONTROL'";
                strSQL += " WHERE IDSUP='" + strSupplierID +"'";
                db.Execute(strSQL);
                db.CommitTrans();
                MessageBox.Show("Save complete...", "Supplier", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GetSupplier();
                sleSupplierID.EditValue = strSupplierID;
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();

        }
        private void NewData()
        {
            if (sleSupplierID.EditValue == null) sleSupplierID_EditValueChanged(null, null);
            else sleSupplierID.EditValue = null;
        }

        #region "Initialize form"
        private void GetPaymentTerm()
        {
            string strSQL = "SELECT IDPAY,PAYMENT FROM PO_PAYMENT";
            DataTable dt = db.GetDataTable(strSQL);
            luePayterm.Properties.DataSource = dt;
            luePayterm.Properties.PopulateColumns();
            luePayterm.Properties.DisplayMember = "PAYMENT";
            luePayterm.Properties.ValueMember = "IDPAY";
            foreach (LookUpColumnInfo col in luePayterm.Properties.Columns)
            {
                if (col.FieldName == "IDPAY") { col.Visible = false; }
            }
        }
        private void GetSupplier()
        {
            string strSQL = "SELECT IDSUP,NAME FROM PO_SUPPLIER";
            DataTable dt = db.GetDataTable(strSQL);
            sleSupplierID.Properties.DataSource = dt;
            sleSupplierID.Properties.PopulateViewColumns();
            sleSupplierID.Properties.DisplayMember = "IDSUP";
            sleSupplierID.Properties.ValueMember = "IDSUP";
        }
        private void GetSupplierDetail(string supplierID)
        {
            string strSQL = "SELECT IDSUP,NAME,ADDRESS1,ADDRESS2,ZIP,COUNTRY,TELEPHONE,FAX,IDPAYMENT "+
                "FROM PO_SUPPLIER WHERE IDSUP='" + supplierID + "'";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                txtSupplier.Text  = dr["NAME"].ToString();
                txtAD1.Text = dr["ADDRESS1"].ToString();
                txtAD2.Text = dr["ADDRESS2"].ToString();
                txtZip.Text = dr["ZIP"].ToString();
                lueCountry.EditValue = dr["COUNTRY"].ToString();
                txtTel.Text = dr["TELEPHONE"].ToString();
                txtFax.Text = dr["FAX"].ToString();
                luePayterm.EditValue = dr["IDPAYMENT"].ToString();
            }
        }
        #endregion

        private void frmTS5_POSupplier_Load(object sender, EventArgs e)
        {
            dtfinfo=clinfo.DateTimeFormat;
            countries = Module.GetCountryList();
            lueCountry.Properties.DataSource = countries;
            GetPaymentTerm();
            GetSupplier();
        }
        private void sleSupplierID_EditValueChanged(object sender, EventArgs e)
        {
            txtSupplier.Text = "";
            txtAD1.Text = "";
            txtAD2.Text = "";
            txtZip.Text = "";
            lueCountry.EditValue = null;
            txtMail.Text = "";
            txtTel.Text = "";
            txtFax.Text = "";
            luePayterm.EditValue = null;
            if (sleSupplierID.EditValue == null) return;
            else GetSupplierDetail(sleSupplierID.EditValue.ToString());
        }
        private void cmdClear_Click(object sender ,System.EventArgs e)
        {
            NewData();
        }
        private void cmdAdd_Click(object sender,System.EventArgs e)
        {
            NewData();
        }
        private void cmdClose_Click(object sender,System.EventArgs e)
        {
            this.Dispose();
        }
        private void cmdSave_Click(object sender,System.EventArgs e)
        {
            SaveData();
        }
        private void txtAddr1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtAddr2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtZip_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtCountry_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtMail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtTel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtFax_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }

    }
      
}