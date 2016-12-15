using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using TUW_System.S5_ReceiveByDate;

namespace S5_ReceiveByDate
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {
        DevExpress.XtraEditors.XtraForm frmActive=new DevExpress.XtraEditors.XtraForm();
        
        public XtraForm1()
        {
            InitializeComponent();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {

            ((frmS5_ReceiveByDate)frmActive).NewData();
        }
    }
}