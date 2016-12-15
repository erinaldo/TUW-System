using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using myClass;

namespace TUW_System
{
    public partial class frmS5_ReceiveSummary : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db = new cDatabase(Module.TUW99);
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        DevExpress.Utils.ImageCollection imagesFlag;

        internal LogIn User_Login { get; set; }

        public frmS5_ReceiveSummary()
        {
            InitializeComponent();
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
        public void DisplayData()
        {
            try
            {
                GetPOSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }
        private void LoadfrmS5_Receive(string strDocNo,bool isPONo)
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.ParentForm.MdiChildren)
            {
                if (frmActive.Name == "frmS5_Receive") frmActive.Dispose();
            }
            frmS5_Receive frm = new frmS5_Receive();
            if (isPONo)
            {
                frm.PONO = strDocNo;
                frm.ReceiveNO = "";
            }
            else
            {
                frm.PONO = "";
                frm.ReceiveNO = strDocNo;
            }
            frm.User_Login = User_Login;
            frm.MdiParent = this.ParentForm;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }

        #region "Initialize form"
        private Image GetImageFromResource(string fileName)//ดึงรูปจาก resource
        {
            Stream resource = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("TUW_System.Resources." + fileName);
            string[] resourceNames = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
            return Image.FromStream(resource);
        }
        private byte[] GetImageData(string fileName)//คืนค่ารูปข้อมูลเป็น byte จาก resource
        {
            Image img = GetImageFromResource(fileName);
            MemoryStream mem = new MemoryStream();
            img.Save(mem, System.Drawing.Imaging.ImageFormat.Bmp);
            return mem.GetBuffer();
        }
        private void GetPOSummary()
        {
            string strSQL = "EXEC spTUWSystem_S5_ReceiveSummary '"+
                cboYear.Text+(cboMonth.SelectedIndex+1).ToString().PadLeft(2,'0')+"','"+cboDepartment.Text+"'";
            DataSet ds = db.GetDataSet(strSQL);
            if (ds != null)
            {
                //เพิ่มคอลัมน์ flag
                DataColumn dc = new DataColumn();
                dc.ColumnName = "FLAG";
                dc.DataType = typeof(System.String);
                
                ds.Tables[0].Columns.Add(dc);
                ds.Tables[0].Columns["FLAG"].SetOrdinal(1);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    decimal qty =  (decimal)dr["QTY"];
                    decimal actual = (decimal)dr["ACTUAL"];
                    if (actual >= qty) dr["FLAG"] = "flag_green";//GetImageData("flag_green.png");//GetImageData(imagesFlag.Images["flag_green"]);
                    else if (actual < qty && actual > 0) dr["FLAG"] = "flag_yellow";//GetImageData("flag_yellow.png");//GetImageData(imagesFlag.Images["flag_yellow"]);
                    else dr["FLAG"] = "flag_red";//GetImageData(imagesFlag.Images["flag_red"]);
                }

                DataColumn keyColumn = ds.Tables[0].Columns["PONO"];
                DataColumn foreignKeyColumn = ds.Tables[1].Columns["PONO"];
                DataColumn foreignKeyColumn2 = ds.Tables[2].Columns["PONO"];
                ds.Relations.Add("PO Detail", keyColumn, foreignKeyColumn);
                ds.Relations.Add("Receive", keyColumn, foreignKeyColumn2);
                
                ds.Relations.Add("Receive Detail", 
                    new DataColumn[]{ds.Tables[2].Columns["PONO"],ds.Tables[2].Columns["RECEIVENO"]}, 
                    new DataColumn[]{ds.Tables[3].Columns["PONO"], ds.Tables[3].Columns["RECEIVENO"]});
                gridControl1.DataSource = ds.Tables[0];
            }
            else
                gridControl1.DataSource = null;
            gridControl1.ForceInitialize();
            gridView1.PopulateColumns();

            RepositoryItemImageComboBox rpFlag = gridControl1.RepositoryItems.Add("ImageComboBoxEdit") as RepositoryItemImageComboBox;
            rpFlag.LargeImages=imagesFlag;
            rpFlag.ReadOnly = true;
            rpFlag.Buttons.Clear();
            rpFlag.Items.Add(new ImageComboBoxItem("", "flag_green", 0));
            rpFlag.Items.Add(new ImageComboBoxItem("", "flag_yellow", 1));
            rpFlag.Items.Add(new ImageComboBoxItem("", "flag_red", 2));
            gridView1.Columns["FLAG"].ColumnEdit = rpFlag;

            RepositoryItemButtonEdit rpPONO = gridControl1.RepositoryItems.Add("ButtonEdit") as RepositoryItemButtonEdit;
            rpPONO.ButtonClick += this.buttonEdit_ButtonClick;
            gridView1.Columns["PONO"].ColumnEdit = rpPONO;
            gridView1.Columns["PONO"].OptionsColumn.ReadOnly= true;

            gridView1.Columns["SUPPLIER"].GroupIndex = 0;
            gridView1.Columns["CANCEL"].Visible = false;
            gridView1.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
        }
        #endregion

        private void frmS5_ReceiveSummary_Load(object sender, EventArgs e)
        {
            dtfinfo=clinfo.DateTimeFormat;
            foreach (var month in clinfo.DateTimeFormat.MonthGenitiveNames)
            {
                if(month.Length>0) cboMonth.Properties.Items.Add(month.ToString());
            }
            cboMonth.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboYear.Properties.Items.Add(DateTime.Today.AddYears(0).ToString("yyyy",dtfinfo));
            cboYear.Properties.Items.Add(DateTime.Today.AddYears(-1).ToString("yyyy",dtfinfo));
            cboYear.Properties.Items.Add(DateTime.Today.AddYears(-2).ToString("yyyy",dtfinfo));
            cboYear.Properties.Items.Add(DateTime.Today.AddYears(-3).ToString("yyyy",dtfinfo));
            cboYear.Properties.Items.Add(DateTime.Today.AddYears(-4).ToString("yyyy",dtfinfo));
            cboYear.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboDepartment.Properties.Items.Add("FABRIC CONTROL");
            cboDepartment.Properties.ReadOnly = true;
            cboDepartment.SelectedIndex = 0;
            imagesFlag = new DevExpress.Utils.ImageCollection();
            imagesFlag.TransparentColor = System.Drawing.Color.White;
            imagesFlag.Images.Add(GetImageFromResource("flag_green.png"), "flag_green");
            imagesFlag.Images.Add(GetImageFromResource("flag_yellow.png"), "flag_yellow");
            imagesFlag.Images.Add(GetImageFromResource("flag_red.png"),"flag_red");

            //GetPOSummary();
        }
        private void buttonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = (ButtonEdit)sender;
            //EditorButton button = e.Button;
            LoadfrmS5_Receive(editor.Text,true);
        }
        private void gridViewReceive_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = (ButtonEdit)sender;
            LoadfrmS5_Receive(editor.Text,false);
        }
        private void gridView1_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView master = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            DevExpress.XtraGrid.Views.Grid.GridView detail = (DevExpress.XtraGrid.Views.Grid.GridView)master.GetDetailView(e.RowHandle, e.RelationIndex);
            detail.Columns["PONO"].Visible = false;
            detail.OptionsView.ShowFooter = true;
            if (e.RelationIndex == 0)//PO DETAIL
            {
                detail.Columns["AMOUNT"].SummaryItem.FieldName = "AMOUNT";
                detail.Columns["AMOUNT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            }
            else//Receive
            {
                detail.Columns["ACTUAL"].SummaryItem.FieldName = "ACTUAL";
                detail.Columns["ACTUAL"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
                detail.Columns["INVENTORY"].SummaryItem.FieldName = "INVENTORY";
                detail.Columns["INVENTORY"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");

                RepositoryItemButtonEdit rpReceive = gridControl1.RepositoryItems.Add("ButtonEdit") as RepositoryItemButtonEdit;
                rpReceive.ButtonClick += this.gridViewReceive_ButtonClick;
                detail.Columns["RECEIVENO"].ColumnEdit = rpReceive;
                detail.Columns["RECEIVENO"].OptionsColumn.ReadOnly = true;
                detail.Columns["CANCEL"].Visible = false;
                detail.MasterRowExpanded += this.gridViewReceive_MasterRowExpanded;
                detail.RowStyle += this.gridView_RowStyle;
            }
            detail.OptionsView.EnableAppearanceEvenRow = true;
            detail.OptionsView.EnableAppearanceOddRow = true;
            detail.OptionsView.ColumnAutoWidth = false;
            detail.BestFitColumns();
        }
        private void gridViewReceive_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView master = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            DevExpress.XtraGrid.Views.Grid.GridView detail = (DevExpress.XtraGrid.Views.Grid.GridView)master.GetDetailView(e.RowHandle, e.RelationIndex);
            detail.OptionsView.ShowFooter = true;
            detail.Columns["AMOUNT"].SummaryItem.FieldName = "AMOUNT";
            detail.Columns["AMOUNT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            detail.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            detail.OptionsView.EnableAppearanceEvenRow = true;
            detail.OptionsView.EnableAppearanceOddRow = true;
            detail.OptionsView.ColumnAutoWidth = false;
            detail.BestFitColumns();
        }
        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "PONO")
                {
                    if ((decimal)gridView1.GetRowCellValue(e.RowHandle, "ACTUAL") >= (decimal)gridView1.GetRowCellValue(e.RowHandle, "QTY")
                        ||(bool)gridView1.GetRowCellValue(e.RowHandle,"CANCEL")==true )
                    {
                        RepositoryItemTextEdit rpText = new RepositoryItemTextEdit();
                        e.RepositoryItem = rpText;
                    }
                }
            }
            catch { }
        }
        private void gridView_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                if (view.GetRowCellDisplayText(e.RowHandle, "CANCEL") == "Checked"||view.GetRowCellDisplayText(e.RowHandle, "CANCEL") == "1")
                {
                    Font newFont = new Font(view.Appearance.Row.Font, FontStyle.Strikeout);
                    e.Appearance.Font = newFont;
                }
            }
        }
        






    }
}