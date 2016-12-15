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
    public partial class frmP_Customer : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmP_Customer()
        {
            InitializeComponent();
        }

        public void NewData()
        {
            foreach (Control ctl in layoutControl1.Controls)
            {
                if (ctl is DevExpress.XtraEditors.TextEdit) { ctl.Text = ""; }
                if (ctl is DevExpress.XtraEditors.ComboBoxEdit) { ctl.Text = ""; }
            }
        }
        public void DisplayData()
        {
            string strSQL = "SELECT * FROM XCUSTOMER ORDER BY CUST";
            DataTable dt = db.GetDataTable(strSQL);
            gridControl1.DataSource = null;
            gridControl1.DataSource = dt;
            gridView1.Columns["CUST"].Caption = "ID";
            gridView1.Columns["NAME"].Caption = "Customer name";
            gridView1.Columns["ADR1"].Caption = "Address1";
            gridView1.Columns["ADR2"].Caption = "Address2";
            gridView1.Columns["COUNTRY"].Caption = "Country";
            gridView1.Columns["ZIP"].Caption = "Zip code";
            gridView1.Columns["TEL"].Caption = "Telephone";
            gridView1.Columns["MOBILE"].Caption = "Mobile";
            gridView1.Columns["FAX"].Caption = "Fax";
            gridView1.Columns["MAIL"].Caption = "Mail";
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
                if (txtCust.Text.Length == 0)
                {
                    strSQL = "SELECT MAX(CUST)+1 FROM XCUSTOMER";
                    string strNewID = db.ExecuteFirstValue(strSQL);
                    strSQL = "INSERT INTO XCUSTOMER(CUST,NAME,ADR1,ADR2,COUNTRY,ZIP,TEL,MOBILE,FAX,MAIL,INPUTUSER)VALUES(";
                    strSQL += "'" + strNewID + "','" + txtCustName.Text + "','" + txtAddress1.Text.Replace("'","''") + "','" + txtAddress2.Text.Replace("'","''") + "','" + cboCountry.Text +
                        "','" + txtZip.Text + "','" + txtTel.Text + "','" + txtMobile.Text + "','" + txtFax.Text + "','" + txtEmail.Text +
                        "','" + System.Environment.MachineName + "')";
                    db.Execute(strSQL);
                }
                else
                {
                    strSQL = "UPDATE XCUSTOMER SET " +
                        "NAME='" + txtCustName.Text + "'," +
                        "ADR1='" + txtAddress1.Text.Replace("'","''") + "'," +
                        "ADR2='" + txtAddress2.Text.Replace("'","''") + "'," +
                        "COUNTRY='" + cboCountry.Text + "'," +
                        "ZIP='" + txtZip.Text + "'," +
                        "TEL='" + txtTel.Text + "'," +
                        "MOBILE='" + txtMobile.Text + "'," +
                        "FAX='" + txtFax.Text + "'," +
                        "MAIL='" + txtEmail.Text + "'," +
                        "INPUTDATE=GETDATE(),INPUTUSER='" + System.Environment.MachineName + "' "+
                        "WHERE CUST='"+txtCust.Text+"'";
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

        private void frmCustomer_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            try
            {
                this.Cursor = Cursors.WaitCursor;
                foreach (string country in cUtility.GetCountryList())
                {
                    cboCountry.Properties.Items.Add(country);
                }
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
                txtCust.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "CUST");
                txtCustName.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "NAME");
                txtAddress1.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "ADR1");
                txtAddress2.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "ADR2");
                txtZip.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "ZIP");
                cboCountry.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "COUNTRY");
                txtTel.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "TEL");
                txtMobile.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "MOBILE");
                txtFax.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "FAX");
                txtEmail.Text = gridView1.GetRowCellDisplayText(e.RowHandle, "MAIL");
            }
            catch (Exception)
            {
                
                
            }
        }

    
    
    
    
    
    
    
    
    
    
    
    
    
    }
}