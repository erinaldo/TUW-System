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
    public partial class frmAC_DomesticSales : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;

        cDatabase db;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmAC_DomesticSales()
        {
            InitializeComponent();
        }
        public void ClearData()
        {
            cboMonth.SelectedIndex = DateTime.Today.Month - 1;
            cboYear.SelectedIndex = 0;
            sleCustomer.EditValue = null;
            sleDescription.EditValue = null;
            cboDepartment.Text = "";
            gridControl1.DataSource = null;
        }
        public void DisplayData()
        { 
        
        }
        public void PrintPreview()
        { 
        
        }
        public void Print()
        { 
        
        }

        private void GetInvoiceDetail()
        { 
        
        }

        private void frmAC_DomesticSales_Load(object sender, EventArgs e)
        {

        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {

        }
    
    
    
    
    
    
    
    }
}