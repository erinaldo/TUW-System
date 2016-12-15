using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using Microsoft.Win32;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using myClass;

namespace TUW_System.YS
{
    public partial class frmYS_BarcodeText : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        string barcodePrinter;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        public LogIn User_Login { get; set; }
        
        public frmYS_BarcodeText()
        {
            InitializeComponent();
        }
        public void Print()
        {
            
        }

        private void LoadRegistry()
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\TUW\TUW System");
                if (regKey != null)
                {
                    object keyValue;
                    keyValue = regKey.GetValue("YS_Receive - Barcode Printer");
                    if (keyValue != null)
                        barcodePrinter = regKey.GetValue("YS_Receive - Barcode Printer").ToString();
                    else
                        MessageBox.Show("Not found barcode printer.", "No Printer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    regKey.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load registry error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Z4Mprint()
        {
            try
            {
                string s = "^XA^PRA^FS";
                s += "^FO100,60^BY3,,150^BCN,,Y,Y^FD" + gridView1.GetFocusedRowCellDisplayText("CODE") + "^FS";
                s += "^FO100,380^A0,45^FD" + gridView1.GetFocusedRowCellDisplayText("NAME") + "^FS";
                s += "^PQ1";
                s += "^XZ";
                //System.Drawing.Printing.PrinterSettings settings = new System.Drawing.Printing.PrinterSettings();
                RawPrinterHelper.SendStringToPrinter(barcodePrinter, s, gridView1.GetFocusedRowCellDisplayText("NAME"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmYS_BarcodeText_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            LoadRegistry();
        }
        private void btnPrintBarcode_Click(object sender, EventArgs e)
        {

        }
        private void optType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try 
	        {	        
		        string strSQL="SELECT CODE,NAME FROM BARCODETEXT WHERE TYPE='"+optType.SelectedIndex.ToString()+"' ORDER BY CODE";
                DataTable dt=db.GetDataTable(strSQL);
                DataColumn dc = new DataColumn();
                dc.Caption = "Print";
                dc.ColumnName = "PRINT";
                dc.DataType = typeof(string);
                dt.Columns.Add(dc);
                gridControl1.DataSource = dt;
                gridView1.PopulateColumns();

                RepositoryItemButtonEdit rpPrint = gridControl1.RepositoryItems.Add("ButtonEdit") as RepositoryItemButtonEdit;
                rpPrint.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
                rpPrint.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;
                rpPrint.Buttons[0].Image = Resource1.barcode;
                rpPrint.ButtonClick += this.gridView_ButtonClick;
                gridView1.Columns["PRINT"].ColumnEdit = rpPrint;

                gridView1.OptionsView.EnableAppearanceEvenRow = true;
                gridView1.OptionsView.EnableAppearanceOddRow = true;
                gridView1.OptionsView.ColumnAutoWidth = false;
                gridView1.BestFitColumns();
	        }
	        catch (Exception ex)
	        {
		        MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
	        }
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            var grid = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            grid.IndicatorWidth = 40;
        }
        private void gridView_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                Z4Mprint();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}