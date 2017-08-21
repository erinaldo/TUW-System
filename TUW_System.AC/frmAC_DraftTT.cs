using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System.AC
{
    public partial class frmAC_DraftTT : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        CultureInfo clinfo=new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        
        public frmAC_DraftTT()
        {
            InitializeComponent();
        }
        public void ClearData(bool allControls)
        { 
            if(allControls)
            {
                sleInvoice.EditValueChanged-=sleInvoice_EditValueChanged;
                sleInvoice.EditValue=null;
                sleInvoice.EditValueChanged+=sleInvoice_EditValueChanged;
            }
            dtpDate.EditValue=null;
            cboDepartment.Text="";
            txtLC.Text="";
            cboCredit.Text="";
            txtCustomer.Text="";
            txtAmount.Text="";
            cboCurrency.Text="";
            cboTransport.Text="";
            checkEdit1.Checked=false;
            txtLoading.Text="";
            txtDescription.Text="";
            txtTotal.Text="";
            txtQty.Text="";
            txtExport.Text="";
            dtpExport.EditValue=null;
            txtDebit.Text="";
            dtpBL.EditValue=null;
        }
        public void SaveData()
        {
            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                string strSQL = "select count(invoice_no) from expbillrecord where invoice_no = '" + sleInvoice.Text + "'";
                if (db.ExecuteFirstValue(strSQL) == "0")
                {
                    strSQL = "insert into expbillrecord (invoice_no,inv_date,dept_id,lc_no,cr,custname,amt,curtype,tran_by,exinv_no,inv_desc" +
                        ",ctn,qty,exp_no,exp_date,debno,bl_date,nodebtor) " +
                        " values ('" + sleInvoice.Text + "'";
                    if (dtpDate.EditValue == null)
                        strSQL += ",null";
                    else
                        strSQL += ",'" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    strSQL+=",'" + cboDepartment.Text + "'" +
                        ",'" + txtLC.Text + "'" +
                        ",'" + cboCredit.Text + "'" +
                        ",'" + txtCustomer.Text.Replace("'","''") + "',";
                    strSQL += (txtAmount.Text.Length > 0) ? txtAmount.Text : "0";
                    strSQL += ",'" + cboCurrency.Text + "'" +
                        ",'" + cboTransport.Text + "'" +
                        ",'" + txtLoading.Text + "'" +
                        ",'" + txtDescription.Text.Replace("'", "''") + "',";
                    strSQL += (txtTotal.Text.Length > 0) ? txtTotal.Text : "0";
                    strSQL += ",";
                    strSQL += (txtQty.Text.Length > 0) ? txtQty.Text : "0";
                    strSQL += ",'" + txtExport.Text + "'";
                    if (dtpExport.EditValue == null)
                        strSQL += ",null";
                    else
                        strSQL += ",'" + ((DateTime)dtpExport.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    strSQL += ",'" + txtDebit.Text + "'";
                    if (dtpBL.EditValue == null)
                        strSQL += ",null";
                    else
                        strSQL+=",'" + ((DateTime)dtpBL.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    strSQL+= (checkEdit1.Checked) ? ",1" : ",0";
                    db.Execute(strSQL);
                }
                else
                { 
                    strSQL="update expbillrecord set ";
                    if (dtpDate.EditValue == null)
                        strSQL += "inv_date =null";
                    else
                        strSQL+="inv_date = '"+((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd",dtfinfo)+ "'";
                    strSQL+= ",dept_id = '" + cboDepartment.Text + "'";
                    strSQL+=",lc_no = '"+txtLC.Text+"'";
                    strSQL+=",cr = '"+cboCredit.Text+"'";
                    strSQL+=",custname = '"+txtCustomer.Text.Replace("'","''")+"'";
                    strSQL+=(txtAmount.Text.Length>0)?",amt="+txtAmount.Text:",amt=0";
                    strSQL+= ",curtype = '" + cboCurrency.Text + "'";
                    strSQL+=",tran_by = '"+cboTransport.Text+ "'";
                    strSQL+=",exinv_no = '"+txtLoading.Text+"'";
                    strSQL+=",inv_desc = '"+txtDescription.Text.Replace("'","''")+"'";
                    strSQL+=(txtTotal.Text.Length>0)?",ctn ="+txtTotal.Text:",ctn =0";
                    strSQL+=(txtQty.Text.Length>0)?",qty = "+txtQty.Text:",qty =0";
                    strSQL += ",exp_no = '" + txtExport.Text + "'";
                    if (dtpExport.EditValue == null)
                        strSQL += ",exp_date =null";
                    else
                        strSQL+=",exp_date = '"+((DateTime)dtpExport.EditValue).ToString("yyyy-MM-dd",dtfinfo)+"'";
                    strSQL += ",debno = '" + txtDebit.Text + "'";
                    if (dtpBL.EditValue == null)
                        strSQL += ",bl_date = null";
                    else
                        strSQL += ",bl_date = '" + ((DateTime)dtpBL.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    strSQL += (checkEdit1.Checked) ? ",nodebtor=1" : ",nodebtor=0";
                    strSQL += " where invoice_no = '" + sleInvoice.Text + "'";
                    db.Execute(strSQL);
                }
//    If Not Rs.EOF Then
//        Ans = MsgBox("Data is already Exist. Do you want Save?", vbYesNo, "Warning")
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
        public void DeleteData()
        { 
            if(MessageBox.Show("Do you want to delete invoice "+sleInvoice.Text+" ?","Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
            {
                db.ConnectionOpen();
                try
                {
                    db.BeginTrans();
                    string strSQL = "delete from expbillrecord where invoice_no = '" + sleInvoice.Text + "'";
                    db.Execute(strSQL);
                    db.CommitTrans();
                    sleInvoice.Properties.View.DeleteSelectedRows();
                    ClearData(true);
                    MessageBox.Show("Delete complete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    db.RollbackTrans();
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                db.ConnectionClose();
            }
        }


        private DataTable GetInvoices()
        {
            string strSQL = "select invoice_no from expbillrecord order by invoice_no ";
            DataTable dt = db.GetDataTable(strSQL);
            return dt;

        }
        private void GetInvoiceDetail(string strInvoice)
        {
            string strSQL="select * from expbillrecord where invoice_no = '"+ sleInvoice.Text+"'";
            DataTable dt=db.GetDataTable(strSQL);
            foreach(DataRow dr in dt.Rows)
            {
                dtpDate.EditValue=dr["inv_date"];
                cboDepartment.Text= dr["dept_id"]==System.DBNull.Value?"":dr["dept_id"].ToString();
                txtLC.Text=dr["lc_no"]==System.DBNull.Value?"":dr["lc_no"].ToString();
                cboCredit.Text=dr["cr"]==System.DBNull.Value?"":dr["cr"].ToString();
                txtCustomer.Text=dr["custname"]==System.DBNull.Value?"":dr["custname"].ToString();
                txtAmount.Text=dr["amt"]==System.DBNull.Value?"":dr["amt"].ToString();
                cboCurrency.Text=dr["curtype"]==System.DBNull.Value?"":dr["curtype"].ToString();
                cboTransport.Text=dr["tran_by"]==System.DBNull.Value?"":dr["tran_by"].ToString();
                if (dr["NoDebtor"] == System.DBNull.Value)
                    checkEdit1.Checked = false;
                else
                    checkEdit1.Checked =Convert.ToInt16(dr["NoDebtor"]) == 1 ? true : false;
                txtLoading.Text=dr["exinv_no"]==System.DBNull.Value?"":dr["exinv_no"].ToString();
                txtDescription.Text=dr["inv_desc"]==System.DBNull.Value?"":dr["inv_desc"].ToString().Replace("|","'");
                txtTotal.Text=dr["ctn"]==System.DBNull.Value?"":dr["ctn"].ToString();
                txtQty.Text=dr["qty"]==System.DBNull.Value?"":dr["qty"].ToString();
                txtExport.Text=dr["exp_no"]==System.DBNull.Value?"":dr["exp_no"].ToString();
                dtpExport.EditValue=dr["exp_date"];
                txtDebit.Text=dr["DebNo"]==System.DBNull.Value?"":dr["DebNo"].ToString();
                dtpBL.EditValue=dr["bl_date"];
            }

        }

        private void frmAC_DraftTT_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo=clinfo.DateTimeFormat;
            sleInvoice.Properties.DataSource = GetInvoices();
            sleInvoice.Properties.DisplayMember = "invoice_no";
            sleInvoice.Properties.ValueMember = "invoice_no";
            sleInvoice.Properties.View.OptionsView.ColumnAutoWidth = true;
        }
        private void sleInvoice_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ClearData(false);
                GetInvoiceDetail(sleInvoice.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }
        private void cboDepartment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtLC.Focus();
        }
        private void txtLC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) cboCredit.Focus();
        }
        private void cboCredit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtCustomer.Focus();
        }
        private void txtCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtAmount.Focus();
        }
        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) cboCurrency.Focus();
        }
        private void cboCurrency_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) cboTransport.Focus();
        }
        private void cboTransport_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtLoading.Focus();
        }
        private void txtLoading_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtDescription.Focus();
        }
        private void txtDescription_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtTotal.Focus();
        }
        private void txtTotal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtQty.Focus();
        }
        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtExport.Focus();
        }
        private void txtExport_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) dtpExport.Focus();
        }
        private void txtDebit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) dtpBL.Focus();
        }

    }
}