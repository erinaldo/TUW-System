using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TUW_System.TS1_Receive
{
    public partial class Form1 : Form
    {
        private frmTS1_Receive frmActive;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            frmActive.NewData();
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //frmActive.DisplayData();
        }
        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            frmActive.PrintPreview();
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            frmActive.Print();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            frmActive = new frmTS1_Receive();
            frmActive.ConnectionString = "Server=" + "(local)" + ";uid=sa;pwd=;database=Sewing";
            frmActive.UserName = "Pratheep";
            frmActive.WindowState = FormWindowState.Maximized;
            frmActive.Show();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            frmActive.SaveData();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            frmActive.ClearData();
        }
    }
}
