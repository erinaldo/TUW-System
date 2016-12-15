using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using Microsoft.Win32;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System.YS
{
    public partial class frmYS_CheckCarton : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        string barcodePrinter;

        private const decimal lbs = 2.2046M;//2262185M;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        public LogIn User_Login { get; set; }

        public frmYS_CheckCarton()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            sleCode.Text = "";
            optCategory.SelectedIndex = -1;
            gridControl1.DataSource = null;
        }
        public void DisplayData()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strSQL = "";
                strSQL = "EXEC spTUWSystem_YS_CheckCarton " + optCategory.SelectedIndex.ToString() + ",'" + sleCode.Text + "'";
                DataTable dt = db.GetDataTable(strSQL);
                DataColumn dc = new DataColumn();
                dc.Caption = "Print";
                dc.ColumnName = "PRINT";
                dc.DataType = typeof(string);
                dt.Columns.Add(dc);
                gridControl1.DataSource = dt;
                gridView1.PopulateColumns();
                gridView1.Columns["SERIAL"].OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                gridView1.Columns["SERIAL"].SummaryItem.FieldName = "SERIAL";
                gridView1.Columns["SERIAL"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Count, "{0:n0}");
                gridView1.Columns["NETWEIGHT"].SummaryItem.FieldName = "NETWEIGHT";
                gridView1.Columns["NETWEIGHT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
                RepositoryItemButtonEdit rpPrint = gridControl1.RepositoryItems.Add("ButtonEdit") as RepositoryItemButtonEdit;
                rpPrint.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
                rpPrint.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;
                rpPrint.Buttons[0].Image = Resource1.barcode;
                rpPrint.ButtonClick += this.gridView_ButtonClick;
                gridView1.Columns["PRINT"].ColumnEdit = rpPrint;
                gridView1.Columns["PONO"].Visible = false;
                gridView1.Columns["SECTION"].Visible = false;
                gridView1.Columns["MIXED"].Visible = false;
                gridView1.Columns["COLOR"].Visible = false;
                gridView1.Columns["SPECIAL"].Visible = false;
                gridView1.Columns["TYPE"].Visible = false;
                gridView1.Columns["DESCR"].Visible = false;
                gridView1.Columns["ORDER_SAMPLE"].Visible = false;
                gridView1.Columns["BOI"].Visible = false;

                gridView1.OptionsView.EnableAppearanceEvenRow = true;
                gridView1.OptionsView.EnableAppearanceOddRow = true;
                gridView1.OptionsView.ColumnAutoWidth = false;
                gridView1.BestFitColumns();
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
        public void PrintPreview()
        {
            cCrystalReport crpCheckCarton = new cCrystalReport(Application.StartupPath + @"\Report\YS_CheckCarton.rpt");
            if (crpCheckCarton.SetPrinter(1) == false) { return; }
            crpCheckCarton.ReportTitle = sleCode.Text;
            for (int i = 1; i <= crpCheckCarton.ReportCopy; i++)
            {
                crpCheckCarton.ClearParameters();
                crpCheckCarton.SetParameter("@category",optCategory.SelectedIndex);
                crpCheckCarton.SetParameter("@code", sleCode.Text);
                string fmlText = "";
                crpCheckCarton.PrintReport(fmlText, false, "sa", "ZAQ113m4tuw");
            }
        }
        public void Print()
        {
            cCrystalReport crpCheckCarton = new cCrystalReport(Application.StartupPath + @"\Report\YS_CheckCarton.rpt");
            if (crpCheckCarton.SetPrinter(1) == false) { return; }
            crpCheckCarton.ReportTitle = sleCode.Text;
            for (int i = 1; i <= crpCheckCarton.ReportCopy; i++)
            {
                crpCheckCarton.ClearParameters();
                crpCheckCarton.SetParameter("category", optCategory.SelectedIndex);
                crpCheckCarton.SetParameter("code", sleCode.Text);
                string fmlText = "";
                crpCheckCarton.PrintReport(fmlText, true, "sa", "ZAQ113m4tuw");
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
                gridView1.ExportToXlsx(strTemp);
            }
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
        
        private DataTable GetYarnCode()
        { 
            string strSQL="SELECT DISTINCT YARNID,YARNCODE FROM YARNGENBARCODE WHERE SYSDELETE=0";
            DataTable dt=db.GetDataTable(strSQL);
            return dt;
        }
        private void Z4Mprint()
        {
            string s = "^XA^PRA^FS";
            s += "^FO50,60^BY3,,60^BCN,,Y,Y^FD" + gridView1.GetFocusedRowCellDisplayText("SERIAL") + "^FS";                                 //||||||||||||||||||||||||||||||||
            s += "^FO480,60^A0,45^FD" + gridView1.GetFocusedRowCellDisplayText("SERIAL") + "^FS";
            s += "^FO50,150^A0,45^FD" + sleCode.Text + "^FS";                                         //AC/ACL1/80XEKS175G

            s += "^FO50,210^A0,25^FD" + gridView1.GetFocusedRowCellDisplayText("MIXED") + "^FS";                                          //Acrylic 70%, Acrylate 30% (Acrylic/Rayon)

            s += "^FO50,260^A0,25^FDColor : ^FS";
            s += "^FO170,250^A0,35^FD" + gridView1.GetFocusedRowCellDisplayText("COLOR") + "^FS";                                        //Color : CMT-0020 NAVY (RT3752)
            //s+= "^FO600,200^A0,30^FD" + chkRemain.Checked?"Remain":"" + "^FS";                                          //เอาออกเพื่อเพิ่ม description yarn
            if (gridView1.GetFocusedRowCellDisplayText("BOI")=="1") s += "^FO600,250^A0,40^FD" + "BOI" + "^FS";                                          //เปลี่ยนเพื่อเพิ่ม description yarn                                                                                                                'BOI

            s += "^FO50,306^A0,25^FDSpecial : ^FS";
            s += "^FO170,300^A0,35^FD" + gridView1.GetFocusedRowCellDisplayText("SPECIAL") + "^FS";                                      //Special :          Z-TWIST
            if (gridView1.GetFocusedRowCellDisplayText("ORDER_SAMPLE") == "1") s += "^FO600,200^A0,40^FD" + "Sample" + "^FS";                                       //เปลี่ยเพิ่ม description yarn                                                                                                              'SAMPLE

            s += "^FO50,350^A0,25^FDType : ^FS";                                                       //Type :               ACRYLIC/ACRYLATE
            s += "^FO170,350^A0,25^FD" + gridView1.GetFocusedRowCellDisplayText("TYPE") + "^FS";
            s += "^FO480,350^A0,25^FDID : ^FS";                                                        //ID :
            s += "^FO600,350^A0,25^FD" + gridView1.GetFocusedRowCellDisplayText("YARNID") + "^FS";

            s += "^FO480,320 ^A0,25^FDDescr : ^FS";                                                    //เพิ่ม description yarn
            s += "^FO600,320^A0,25^FD" + gridView1.GetFocusedRowCellDisplayText("DESCR") + "^FS";                                  //เพิ่ม description yarn                                                                                                          'SAMPLE

            s += "^FO50,400^A0,25^FDSup. : ^FS";
            s += "^FO170,400^A0,30^FD" + gridView1.GetFocusedRowCellDisplayText("PUR_SUPPLIER") + "^FS";                                     //Sup :                ITOCHU
            s += "^FO480,400 ^A0,25^FDDate : ^FS";
            s += "^FO600,400 ^A0,25^FD" + DateTime.Today.ToString("dd/MM/yyyy", dtfinfo) + "^FS";

            s += "^FO50,450^A0,25^FDProducer : ^FS";
            if (gridView1.GetFocusedRowCellDisplayText("PUR_SUPPLIER") == "")
                s += "^FO170,450^A0,30^FD^FS";
            else
                s += (gridView1.GetFocusedRowCellDisplayText("PUR_SUPPLIER").Length > 19) ? "^FO170,450^A0,30^FD" + gridView1.GetFocusedRowCellDisplayText("PUR_SUPPLIER").Substring(0, 19) + "^FS" : "^FO170,450^A0,30^FD" + gridView1.GetFocusedRowCellDisplayText("PUR_SUPPLIER") + "^FS";                                     //Producer  :    ITOCHU                                        Section :      PARFUN
            s += "^FO480,450^A0,25^FDSection : ^FS";
            s += "^FO600,450^A0,25^FD" + gridView1.GetFocusedRowCellDisplayText("SECTION") + "^FS";

            s += "^FO50,500^A0,25^FDP/O No.: ^FS";
            s += "^FO170,500^A0,25^FD" + gridView1.GetFocusedRowCellDisplayText("PONO") + "^FS";                                           //P/O No  :       FB. 1234/04                                  Ctn No. :      7
            s += "^FO480,505^A0,25^FDCtn No.: ^FS";
            s += "^FO600,500^A0,35^FD" + gridView1.GetFocusedRowCellDisplayText("CTNNO") + "^FS";

            s += "^FO50,550^A0,25^FDLot No.: ^FS";
            s += "^FO170,550^A0,25^FD" + gridView1.GetFocusedRowCellDisplayText("LOTNO") + "^FS";                                         //Lot No.  :        3456/04                                          Weight :      23.5
            s += "^FO480,560^A0,25^FDWeight : ^FS";
            s += "^FO580,550^A0,50^FD" + gridView1.GetFocusedRowCellDisplayText("NETWEIGHT") + "^FS";
            s += "^PQ1";
            s += "^XZ";
            // Send a printer-specific to the printer.
            //System.Drawing.Printing.PrinterSettings settings = new System.Drawing.Printing.PrinterSettings();
            RawPrinterHelper.SendStringToPrinter(barcodePrinter/*settings.PrinterName*/, s, gridView1.GetFocusedRowCellDisplayText("SERIAL"));
        }

        private void frmYS_CheckCarton_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            NewData();
            sleCode.Properties.DataSource = GetYarnCode();
            sleCode.Properties.DisplayMember = "YARNCODE";
            sleCode.Properties.ValueMember = "YARNCODE";
            LoadRegistry();
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
                //ButtonEdit editor = (ButtonEdit)sender;
                //LoadfrmYS_ReceiveForm(editor.Text, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}