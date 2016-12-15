using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System.TS1
{
    public partial class frmTS1_FindFabricCode : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;

        private DevExpress.XtraBars.BarStaticItem _bsiStatusBar;
        private string _connectionString;
        public DevExpress.XtraBars.BarStaticItem bsiStatusbar
        {
            get { return _bsiStatusBar; }
            set { _bsiStatusBar = value; }
        }
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmTS1_FindFabricCode()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            cboSearch.Text = "";
            txtSearch.Text = "";
            Grid.DataSource = null;
        }
        public void DisplayData()
        {
            string strSQL = "Select ID,CODE,SECTION,REGISTER AS REGISTER_DATE From GreyFabric Where Deleteflag=0";
            try
            {
                switch (cboSearch.Text)
                {
                    case "ID":
                        strSQL = strSQL + "And ID=" + txtSearch.Text;
                        break;
                    case "Code":
                        strSQL = strSQL + "And Code Like \'" + txtSearch.Text + "%\'";
                        break;
                }
                DataTable dt = db.GetDataTable(strSQL);
                Grid.DataSource = dt;
                gridView1.OptionsView.EnableAppearanceOddRow = true;
                gridView1.OptionsView.EnableAppearanceEvenRow = true;
                gridView1.OptionsView.ColumnAutoWidth = false;
                gridView1.BestFitColumns();
                this.bsiStatusbar.Caption=gridView1.RowCount.ToString() + " Rows.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }
        public void ExportExcel()
        {
            SaveFileDialog theOpenFile = new SaveFileDialog();
            string strTemp;
            theOpenFile.Filter = "Microsoft Excel Document|*.xls";
            if (theOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                strTemp = theOpenFile.FileName;
                gridView1.ExportToXls(strTemp);
            }
        }

        private void frmTS1_FindFabricCode_Load(System.Object sender, System.EventArgs e)
        {
            db = new cDatabase(_connectionString);
            cboSearch.SelectedIndex = 0;
        }
        private void cmdSearch_Click(System.Object sender, System.EventArgs e)
        {
            DisplayData();
        }
        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                DisplayData();
            }
        }
    }
}