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
    public partial class frmAC_LoadDomestic : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }


        public frmAC_LoadDomestic()
        {
            InitializeComponent();
        }
    }
}