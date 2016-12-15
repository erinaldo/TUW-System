using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using myClass;

namespace TUW_System.TS1
{
    public partial class frmTS1_PurchaseClose : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;
        
        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        DevExpress.Utils.ImageCollection imagesFlag;
        DataSet dsMain;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmTS1_PurchaseClose()
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

        #region "Initialize form"
        private Image GetImageFromResource(string fileName)//ดึงรูปจาก resource
        {
            Stream resource = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("TUW_System.TS1.Resources." + fileName);
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
            string strSQL = "EXEC spTUWSystem_TS1_PurchaseClose '"+cboYear.Text+"'";
            dsMain = db.GetDataSet(strSQL);
            if (dsMain != null)
            {
                //เพิ่มคอลัมน์ flag
                DataColumn dc = new DataColumn();
                dc.ColumnName = "FLAG";
                dc.DataType = typeof(System.String);
                dsMain.Tables[1].Columns.Add(dc);
                dsMain.Tables[1].Columns["FLAG"].SetOrdinal(1);

                dc = new DataColumn();
                dc.ColumnName = "CLOSE_PO";
                dc.DataType = typeof(System.String);
                dsMain.Tables[1].Columns.Add(dc);

                foreach (DataRow dr in dsMain.Tables[1].Rows)
                {
                    decimal qty = (decimal)dr["QTY"];
                    decimal receive = (decimal)dr["RECEIVE"];
                    if (receive >= qty) dr["FLAG"] = "flag_green";
                    else if (receive < qty && receive > 0) dr["FLAG"] = "flag_yellow";
                    else dr["FLAG"] = "flag_red";
                }

                DataColumn keyColumn = dsMain.Tables[0].Columns["PONO"];
                DataColumn foreignKeyColumn = dsMain.Tables[1].Columns["PONO"];
                dsMain.Relations.Add("PO_Detail", keyColumn, foreignKeyColumn);
                dsMain.Relations.Add("Receive_Detail",
                    new DataColumn[] { dsMain.Tables[1].Columns["PONO"], dsMain.Tables[1].Columns["TPICSNO"] },
                    new DataColumn[] { dsMain.Tables[2].Columns["PONO"], dsMain.Tables[2].Columns["ORDNO"] });
                gridControl1.DataSource = dsMain.Tables[0];
            }
            else
                gridControl1.DataSource = null;
            gridControl1.ForceInitialize();
            gridView1.PopulateColumns();

            gridView1.Columns["SUPPLIER"].GroupIndex = 0;
            gridView1.Columns["CANORNO"].Visible = false;
            gridView1.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            gridView1.RowStyle += this.gridView_RowStyle;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
        }
        #endregion

        private void frmTS1_PurchaseClose_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            cboYear.Properties.Items.Add(DateTime.Today.AddYears(0).ToString("yyyy", dtfinfo));
            cboYear.Properties.Items.Add(DateTime.Today.AddYears(-1).ToString("yyyy", dtfinfo));
            cboYear.Properties.Items.Add(DateTime.Today.AddYears(-2).ToString("yyyy", dtfinfo));
            cboYear.Properties.Items.Add(DateTime.Today.AddYears(-3).ToString("yyyy", dtfinfo));
            cboYear.Properties.Items.Add(DateTime.Today.AddYears(-4).ToString("yyyy", dtfinfo));
            cboYear.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            imagesFlag = new DevExpress.Utils.ImageCollection();
            imagesFlag.TransparentColor = System.Drawing.Color.White;
            imagesFlag.Images.Add(GetImageFromResource("flag_green.png"), "flag_green");
            imagesFlag.Images.Add(GetImageFromResource("flag_yellow.png"), "flag_yellow");
            imagesFlag.Images.Add(GetImageFromResource("flag_red.png"), "flag_red");
        }
        private void gridView1_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView master = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            DevExpress.XtraGrid.Views.Grid.GridView detail = (DevExpress.XtraGrid.Views.Grid.GridView)master.GetDetailView(e.RowHandle, e.RelationIndex);
            detail.Columns["PONO"].Visible = false;
            detail.Columns["CANORNO"].Visible = false;
            detail.Columns["ID"].Visible = false;
            
            detail.OptionsView.ShowFooter = true;
            detail.Columns["AMOUNT"].SummaryItem.FieldName = "AMOUNT";
            detail.Columns["AMOUNT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
                
            RepositoryItemImageComboBox rpFlag = gridControl1.RepositoryItems.Add("ImageComboBoxEdit") as RepositoryItemImageComboBox;
            rpFlag.LargeImages = imagesFlag;
            rpFlag.ReadOnly = true;
            rpFlag.Buttons.Clear();
            rpFlag.Items.Add(new ImageComboBoxItem("", "flag_green", 0));
            rpFlag.Items.Add(new ImageComboBoxItem("", "flag_yellow", 1));
            rpFlag.Items.Add(new ImageComboBoxItem("", "flag_red", 2));
            detail.Columns["FLAG"].ColumnEdit = rpFlag;

            RepositoryItemButtonEdit rpClosePO = gridControl1.RepositoryItems.Add("ButtonEdit") as RepositoryItemButtonEdit;
            rpClosePO.Buttons[0].Kind = ButtonPredefines.Delete;
            rpClosePO.ButtonClick += this.PODetail_ButtonClick;
            detail.Columns["CLOSE_PO"].ColumnEdit = rpClosePO;
            detail.MasterRowExpanded += this.gridViewPDetail_MasterRowExpanded;
            detail.ShowingEditor += this.gridView1_ShowingEditor;
            detail.CustomRowCellEdit += this.gridViewPDetail_CustomRowCellEdit;
            
            detail.OptionsView.EnableAppearanceEvenRow = true;
            detail.OptionsView.EnableAppearanceOddRow = true;
            detail.OptionsView.ColumnAutoWidth = false;
            detail.BestFitColumns();
        }
        private void gridViewPDetail_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView master = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            DevExpress.XtraGrid.Views.Grid.GridView detail = (DevExpress.XtraGrid.Views.Grid.GridView)master.GetDetailView(e.RowHandle, e.RelationIndex);
            detail.Columns["CANORNO"].Visible = false;
            detail.OptionsView.ShowFooter = true;
            detail.Columns["AMOUNT"].SummaryItem.FieldName = "AMOUNT";
            detail.Columns["AMOUNT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            detail.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            detail.RowStyle += this.gridView_RowStyle;
            detail.OptionsView.EnableAppearanceEvenRow = true;
            detail.OptionsView.EnableAppearanceOddRow = true;
            detail.OptionsView.ColumnAutoWidth = false;
            detail.BestFitColumns();
        }
        private void gridViewPDetail_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                if (e.Column.FieldName == "CLOSE_PO")
                {
                    if ((decimal)gv.GetRowCellValue(e.RowHandle, "RECEIVE") >= (decimal)gv.GetRowCellValue(e.RowHandle, "QTY")
                        ||gv.GetRowCellValue(e.RowHandle, "CANORNO").ToString() =="1")
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
                if (view.GetRowCellDisplayText(e.RowHandle, "CANORNO") == "1")
                {
                    Font newFont = new Font(view.Appearance.Row.Font, FontStyle.Strikeout);
                    e.Appearance.Font = newFont;
                }
            }
        }
        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv=(DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (gv.FocusedColumn.FieldName == "CLOSE_PO")
                e.Cancel = false;
            else
                e.Cancel = true;
        }
        private void PODetail_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (MessageBox.Show("Do you want to close this order?", "Close PO", MessageBoxButtons.YesNo,MessageBoxIcon.Warning ) == DialogResult.No) return;
            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)gridControl1.FocusedView;
                DataRow dr= gv.GetFocusedDataRow();
                decimal newQty=(decimal)dr["RECEIVE"];
                string strSQL = "UPDATE TDPURCHASE SET QTY="+newQty+" WHERE ID='"+dr["ID"].ToString()+"'";
                db.Execute(strSQL);
                //MessageBox.Show(strSQL);
                dr["FLAG"] = "flag_green";
                dr["QTY"] = newQty;
                gv.CloseEditor();
                gv.UpdateCurrentRow();
                db.CommitTrans();
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
        }



    }
}