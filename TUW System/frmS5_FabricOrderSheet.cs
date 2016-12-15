using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using System.Globalization;
using myClass;

namespace TUW_System
{
    public partial class frmS5_FabricOrderSheet : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db = new cDatabase(Module.TUW99);
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        DataSet dsMain;
        BindingSource bsMain;
        RepositoryItemDateEdit rpDate;
        RepositoryItemCheckEdit rpCancel, rpConfirm;


        public frmS5_FabricOrderSheet()
        {
            InitializeComponent();
        }
        public void NewData()
        { 
        
        }
        public void DisplayData()
        {
            try
            {
                DisplayData(DateTime.Today, cceSection.EditValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayData(DateTime datYear,string strSection)
        {
            string strSQL = "EXEC spTUWSystem_FabricOrderSheet '" + datYear.ToString("yyyy", dtfinfo) + "','" + strSection + "'";
            dsMain = db.GetDataSet(strSQL);
            
            bsMain=new BindingSource();
            bsMain.DataSource=dsMain;
            bsMain.DataMember=dsMain.Tables[0].TableName;
            //gridControl1.DataSource = null;
            gridControl1.DataSource = bsMain;
            //gridView1.PopulateColumns();
            DevExpress.XtraGrid.Columns.GridColumn gc;
            gc = gridView1.Columns["APPVDATE"];//Approved Date
            gc.Caption = "Approved Date";
            gc.ColumnEdit=rpDate;
            gc = gridView1.Columns["ORDERYEAR"];//Year
            gc.Caption = "Year";
            gc = gridView1.Columns["ORDERNO"];//Order No
            gc.Caption = "Order No.";
            gc = gridView1.Columns["ROWDATA"];//Branch
            gc.Caption = "#";
            gc = gridView1.Columns["STATUS"];//Cancel
            gc.Caption = "Cancel";
            gc.ColumnEdit = rpCancel;
            gc=gridView1.Columns["CODE"];//Fabric Name
            gc.VisibleIndex = 5;
            gc.Caption = "Fabric Name";
            gc=gridView1.Columns["PCS"];//PCS
            gc.Caption = "PCS";
            gc = gridView1.Columns["KGS"];//KGS
            gc.Caption = "KGS";
            gc=gridView1.Columns["COLOR"];//Color
            gc.Caption = "Color";
            gc = gridView1.Columns["LOTNO"];//Lot No
            gc.Caption = "Lot No.";
            gc=gridView1.Columns["PCSPERKGS"];//Price/Kgs
            gc.Caption = "Price/Kgs";
            gc=gridView1.Columns["DUEDATE"];//Order DueDate
            gc.Caption = "Order DueDate";
            //gc.ColumnEdit=rpDate;
            gc=gridView1.Columns["DYEDUEDATE1"];//From DueDate
            gc.Caption = "From DueDate";
            gc.ColumnEdit=rpDate;
            gc=gridView1.Columns["DYEDUEDATE2"];//To DueDate
            gc.Caption = "To DueDate";
            gc.ColumnEdit=rpDate;
            gc=gridView1.Columns["CHKDUEDATE"];//Confirm
            gc.Caption = "Confirm";
            gc.ColumnEdit = rpConfirm;
            gc=gridView1.Columns["PCSDELIVERY"];//Pcs Delivery
            gc.Caption = "Pcs Delivery";
            gc=gridView1.Columns["FIRSTDELIVERY"];//First Delivery
            gc.Caption="First Delivery";
            gc.ColumnEdit=rpDate;
            gc=gridView1.Columns["LASTDELIVERY"];//Last Delivery
            gc.Caption = "Last Delivery";
            gc.ColumnEdit=rpDate;
            gc=gridView1.Columns["DELIVERYDATE"];//Fabric Delivery
            gc.Caption = "Fabric Delivery";
            gc.ColumnEdit=rpDate;
            gc =gridView1.Columns["DAY"];//Day
            gc.Caption = "Day";
            gc=gridView1.Columns["SEWDATE"];//Sew Date
            gc.Caption = "Sew Date";
            gc.ColumnEdit=rpDate;
            gc=gridView1.Columns["LOADDATE"];//Load Date
            gc.Caption = "Load Date";
            gc.ColumnEdit=rpDate;
            gc=gridView1.Columns["YARNDUEDATE"];//Yarn DueDate
            gc.Caption = "Yarn DueDate";
            gc.ColumnEdit=rpDate;
            gc=gridView1.Columns["REMARK"];//Remark
            gc.Caption = "Remark";
            gc=gridView1.Columns["FABRICREMARK"];//Fabric Remark
            gc.Caption = "Fabric Remark";
            gc = gridView1.Columns["SECTION"];//Section
            gc.Caption = "Section";

            //gridView1.OptionsBehavior.ReadOnly = true;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            

            foreach (Control ctl in layoutControl1.Controls)
            {
                ctl.DataBindings.Clear();
            }
            
            txtYear.DataBindings.Add(new Binding("Text", bsMain, "OrderYear",true));
            txtOrder.DataBindings.Add(new Binding("Text", bsMain, "OrderNo", true));
            txtBranch.DataBindings.Add(new Binding("Text", bsMain, "RowData", true));
            txtFabricCode.DataBindings.Add(new Binding("Text", bsMain, "Code", true));
            txtColor.DataBindings.Add(new Binding("Text", bsMain, "Color", true));
            txtQty.DataBindings.Add(new Binding("Text", bsMain, "Kgs", true));
            txtPcs.DataBindings.Add(new Binding("Text", bsMain, "Pcs", true));
            txtPrice.DataBindings.Add(new Binding("Text", bsMain, "PcsPerKgs", true));
            txtLotno.DataBindings.Add(new Binding("Text", bsMain, "LotNo", true));
            cboSection.DataBindings.Add(new Binding("Text", bsMain, "Section", true));
            chkCancel.DataBindings.Add(new Binding("EditValue", bsMain, "Status", true));
            chkConfirm.DataBindings.Add(new Binding("EditValue", bsMain, "Chkduedate", true));
            dtpCustomerDueDate.DataBindings.Add(new Binding("Text", bsMain, "DueDate", true));
            dtpCustomerLoadDate.DataBindings.Add(new Binding("Text", bsMain, "LoadDate", true));
            dtpFDueDate.DataBindings.Add(new Binding("Text", bsMain, "DyeDuedate1", true));
            dtpTDueDate.DataBindings.Add(new Binding("Text", bsMain, "DyeDuedate2", true));
            dtpFDelivery.DataBindings.Add(new Binding("Text", bsMain, "firstDelivery", true));
            dtpLDelivery.DataBindings.Add(new Binding("Text", bsMain, "lastDelivery", true));
            dtpSew.DataBindings.Add(new Binding("Text", bsMain, "SewDate", true));
            dtpYarn.DataBindings.Add(new Binding("Text", bsMain, "YarnDueDate", true));
            dtpApprove.DataBindings.Add(new Binding("Text", bsMain, "APPVDATE", true));
            dtpFabricDelivery.DataBindings.Add(new Binding("Text", bsMain, "DELIVERYDATE", true));
            memRemark.DataBindings.Add(new Binding("Text", bsMain, "Remark", true));
            memFabricRemark.DataBindings.Add(new Binding("Text", bsMain, "FabricRemark", true));

            
        }


        private void frmS5_FabricOrderSheet_Load(object sender, EventArgs e)
        {
            dtfinfo = clinfo.DateTimeFormat;
            //Create RepositoryItem-------------------------------
            rpDate= new RepositoryItemDateEdit();
            rpDate.Mask.EditMask = "dd/MMM/yyyy";
            rpDate.Mask.UseMaskAsDisplayFormat = true;
            rpCancel = new RepositoryItemCheckEdit();
            rpCancel.ValueChecked = "C";
            rpCancel.ValueUnchecked = "N";
            rpCancel.AllowGrayed = false;
            rpCancel.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            rpConfirm = new RepositoryItemCheckEdit();
            rpConfirm.ValueChecked = Convert.ToInt16(1);
            rpConfirm.ValueUnchecked = Convert.ToInt16(0);
            rpConfirm.AllowGrayed = false;
            rpConfirm.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            //-------------------------------------------------
            chkCancel.Properties.ValueChecked = "C";
            chkCancel.Properties.ValueUnchecked = "N";
            chkCancel.Properties.AllowGrayed = false;
            chkCancel.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            chkConfirm.Properties.ValueChecked = Convert.ToInt16(1);
            chkConfirm.Properties.ValueUnchecked = Convert.ToInt16(0);
            chkConfirm.Properties.AllowGrayed = false;
            chkConfirm.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;

        }

        private void cceSection_EditValueChanged(object sender, EventArgs e)
        {
            if (cceSection.EditValue == null) { return; }

        }
        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            decimal pcs=(decimal)gv.GetRowCellValue(e.RowHandle,"PCS");//จำนวนพับที่สั่ง
            decimal pcsDelivery=(decimal)gv.GetRowCellValue(e.RowHandle,"PCSDELIVERY");//จำนวนพับที่ส่งออก
            DateTime deliveryDate=(DateTime)gv.GetRowCellValue(e.RowHandle,"DELIVERYDATE");//วันที่ส่งออกจริง
            DateTime dyeDueDate2=(DateTime)gv.GetRowCellValue(e.RowHandle,"DYEDUEDATE2");//กำหนดส่งออกวันสุดท้าย
            if (gv.GetRowCellValue(e.RowHandle,"STATUS").ToString() == "C")//Cancel order
            {
                e.Appearance.ForeColor = Color.Yellow;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if(pcs>pcsDelivery)//ส่งออกยังไม่ครบและเกินดิวแล้ว
            {
                if (dyeDueDate2 < DateTime.Today && deliveryDate == null)//วันส่งออกเกินวันนี้และยังไม่มีวันส่งออกเลย
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else if (dyeDueDate2 < DateTime.Today && deliveryDate > dyeDueDate2)//วันส่งออกเกินวันนี้และวันส่งออกจริงเกินกว่ากำหนดส่งออกวันสุดท้าย
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
            else if(pcs<=pcsDelivery)//ส่งออกครบแต่เกินดิว
            {
                
            }
         }

    }
}