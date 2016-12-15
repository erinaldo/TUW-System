using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TUW_System.S5_ReceiveByDate
{
    public partial class Form1 : Form
    {
        private frmS5_ReceiveByDate frmActive;

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
            frmActive.DisplayData();
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
            frmActive = new frmS5_ReceiveByDate();
            frmActive.ConnectionString = "Server=" + "tuwncbase" + ";uid=sa;pwd=;database=PurchaseOrder";
            frmActive.WindowState = FormWindowState.Maximized;
            frmActive.Show();
        }
    }
}
