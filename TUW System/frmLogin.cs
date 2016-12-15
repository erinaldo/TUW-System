using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Win32;
using myClass;

namespace TUW_System
{
    public partial class frmLogin : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db = new cDatabase(Module.tuwCenter);
        private LogIn User_Login;

        public frmLogin()
        {
            InitializeComponent();
        }
        private void LoadRegistry()
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\TUW\TUW System");
                if (regKey != null)
                {
                    object keyValue = regKey.GetValue("Username");
                    if (keyValue != null)
                    {
                        txtUsername.Text = regKey.GetValue("Username").ToString();
                    }
                    regKey.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load registry error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveRegistry()
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\TUW\TUW System", true);
                if (regKey == null)
                {
                    regKey = Registry.CurrentUser.CreateSubKey(@"Software\TUW\TUW System");
                }
                regKey.SetValue("Username", txtUsername.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save to registry error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool VerifyLogin(string userName, string passWord)
        {
            string strSQL = "select emp_code,emp_name,emp_lastname,emp_frmname,emp_save,emp_print from view_tuw_login where emp_username='"+userName+"'"+
                " and emp_password='"+passWord+"' and emp_programname='TUW System'";
            DataTable dtLogin = db.GetDataTable(strSQL);
            if (dtLogin == null)
            {
                MessageBox.Show("dtLogin==null");
                return false;
            }
            else if(dtLogin.Rows.Count == 0)
            {
                MessageBox.Show("dtLogin.Rows.Count==0");
                return false;
            }
            else
            {
                User_Login = new LogIn();
                User_Login.UserName = userName;
                var forms = new List<LogIn_Form>();
                foreach (DataRow dr in dtLogin.Rows)
                {
                    User_Login.EmployeeCode = dr["emp_code"].ToString();
                    User_Login.FirstName = dr["emp_name"].ToString();
                    User_Login.LastName = dr["emp_lastname"].ToString();
                    forms.Add(new LogIn_Form
                        {
                            FormName = dr["emp_frmname"].ToString(),
                            CanSave=(dr["emp_save"].ToString()=="1")?true:false,
                            CanPrint=(dr["emp_print"].ToString()=="1")?true:false
                        });
                }
                User_Login.Forms=forms;
                return true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Application.Exit();
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (VerifyLogin(txtUsername.Text, txtPassword.Text))
                {
                    Module.strUserName = txtUsername.Text;
                    SaveRegistry();
                    this.Hide();
                    mdiMain frmMain = new mdiMain();
                    //frmMain.SetdtLogin = dtLogin;
                    frmMain.User_Login = User_Login;
                    frmMain.WindowState = FormWindowState.Maximized;
                    frmMain.Show();
                }
                else
                {
                    throw new ApplicationException("Username or password not correct.");
                }
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void frmLogin_Load(object sender, EventArgs e)
        {
            LoadRegistry();
        }
        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(char)13){ btnOK_Click(sender,e);}
        }

        private void frmLogin_Shown(object sender, EventArgs e)
        {
            if (txtUsername.Text.Length == 0)
                txtUsername.Focus();
            else
                txtPassword.Focus();
        }
    }

}