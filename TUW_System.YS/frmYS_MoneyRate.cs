using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System.YS
{
    public partial class frmYS_MoneyRate : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        
        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        
        public frmYS_MoneyRate()
        {
            InitializeComponent();
        }
        public void NewData()
        { 
        
        }
        public void DisplayData()
        { 
        
        }
        public void SaveData()
        { 
        
        }

    }
}