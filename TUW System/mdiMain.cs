using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
//using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using Microsoft.Win32;
using System.Linq;
using TUW_System.ProductionOrder;
using TUW_System.TS1;
using TUW_System.S3;
using TUW_System.S4;
using TUW_System.S5;
using TUW_System.YS;
using TUW_System.FS;
using TUW_System.AC;
using myClass;

namespace TUW_System
{
    public partial class mdiMain : DevExpress.XtraEditors.XtraForm
    {
        public mdiMain()
        {
            InitializeComponent();
        }

        internal LogIn User_Login { get; set; }

        private void LoadfrmTS1_Declare()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_Declare")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_Declare frm1 = new frmTS1_Declare();
            frm1.ConnectionString = Module.ISODocument;
            frm1.ConnectionString2 = Module.Sewing;
            frm1.MdiParent = this;
            frm1.WindowState = FormWindowState.Maximized;
            frm1.Show();
        }
        private void LoadfrmTS1_EditTPiCSCode()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_EditTPiCSCode")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_EditTPiCSCode frm2 = new frmTS1_EditTPiCSCode();
            frm2.ConnectionString = Module.TUW99;
            frm2.MdiParent = this;
            frm2.WindowState = FormWindowState.Maximized;
            frm2.Show();
        }
        private void LoadfrmTS1_FindFabricCode()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_FindFabricCode")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_FindFabricCode frm3 = new frmTS1_FindFabricCode();
            frm3.ConnectionString = Module.TUW99;
            frm3.MdiParent = this;
            frm3.bsiStatusbar = this.bsiStatus;
            frm3.WindowState = FormWindowState.Maximized;
            frm3.Show();
        }
        private void LoadfrmTS1_TPiCSContract()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_TPiCSContract")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_TPiCSContract frm4 = new frmTS1_TPiCSContract();
            frm4.ConnectionString = Module.Sewing;
            frm4.MdiParent = this;
            frm4.WindowState = FormWindowState.Maximized;
            frm4.Show();
        }
        private void LoadfrmTS1_WorkOrder()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_WorkOrder")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_WorkOrder frm5 = new frmTS1_WorkOrder();
            frm5.ConnectionString = Module.Sewing;
            frm5.StatusBarEvent += new frmTS1_WorkOrder.StatusBarHandler(UpdateStatusBar);
            frm5.MdiParent = this;
            frm5.WindowState = FormWindowState.Maximized;
            frm5.Show();
        }
        private void LoadfrmTS1_Purchase()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_Purchase")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_Purchase frm6 = new frmTS1_Purchase();
            frm6.ConnectionString = Module.Sewing;
            frm6.User_Login = User_Login;
            frm6.MdiParent = this;
            frm6.WindowState = FormWindowState.Maximized;
            frm6.Show();
        }
        private void LoadfrmTS1_PurchaseSummary()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_PurchaseSummary")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_PurchaseSummary frm7 = new frmTS1_PurchaseSummary();
            frm7.ConnectionString = Module.Sewing;
            frm7.MdiParent = this;
            frm7.WindowState = FormWindowState.Maximized;
            frm7.bsiStatusbar = this.bsiStatus;
            frm7.Show();
        }
        private void LoadfrmTS1_MaterialConsumption()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_MaterialConsumption")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_MaterialConsumption frm8 = new frmTS1_MaterialConsumption();
            frm8.ConnectionString = Module.Sewing;
            frm8.StatusBarEvent+=new frmTS1_MaterialConsumption.StatusBarHandler(UpdateStatusBar);
            frm8.MdiParent = this;
            frm8.WindowState = FormWindowState.Maximized;
            frm8.Show();
        }
        private void LoadfrmTS1_ImportIF()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_ImportIF")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_ImportIF frm9 = new frmTS1_ImportIF();
            frm9.ConnectionString = Module.TUW99;
            frm9.ConnectionString2 = Module.Fabric;
            frm9.MdiParent = this;
            frm9.WindowState = FormWindowState.Maximized;

            frm9.Show();
        }
        private void LoadfrmTS1_Holding()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_Holding")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_Holding frm10 = new frmTS1_Holding();
            frm10.ConnectionString = Module.Sewing;
            frm10.MdiParent = this;
            frm10.WindowState = FormWindowState.Maximized;
            frm10.Show();
        }
        public void LoadfrmP_Customer()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmP_Customer")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmP_Customer frm11 = new frmP_Customer();
            frm11.ConnectionString = Module.Sale;
            frm11.StatusBarEvent+=new frmP_Customer.StatusBarHandler(UpdateStatusBar);
            frm11.MdiParent = this;
            frm11.WindowState = FormWindowState.Maximized;
            frm11.Show();
        }
        public void LoadfrmP_Model()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmP_Model")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmP_Model frm12 = new frmP_Model();
            frm12.ConnectionString = Module.Sale;
            frm12.LoadfrmP_ModelCategoryEvent+=new frmP_Model.LoadfrmP_ModelCategoryHandler(LoadfrmP_ModelCategory);
            frm12.StatusBarEvent+=new frmP_Model.StatusBarHandler(UpdateStatusBar);
            frm12.MdiParent = this;
            frm12.WindowState = FormWindowState.Maximized;
            frm12.Show();
        }
        public void LoadfrmP_ModelCategory()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmP_ModelCategory")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmP_ModelCategory frm13 = new frmP_ModelCategory();
            frm13.ConnectionString = Module.Sale;
            frm13.StatusBarEvent+=new frmP_ModelCategory.StatusBarHandler(UpdateStatusBar);
            frm13.MdiParent = this;
            frm13.WindowState = FormWindowState.Maximized;
            frm13.Show();
        }
        private void LoadfrmP_Production()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmP_Production")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmP_Production frm14 = new frmP_Production();
            frm14.ConnectionString = Module.Sale;
            frm14.LoadfrmP_CustomerEvent+=new frmP_Production.LoadfrmP_CustomerHandler(LoadfrmP_Customer);
            frm14.LoadfrmP_ModelEvent+=new frmP_Production.LoadfrmP_ModelHandler(LoadfrmP_Model);
            frm14.MdiParent = this;
            frm14.WindowState = FormWindowState.Maximized;
            frm14.Show();
        }
        private void LoadfrmS5_ReceiveSummary()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmS5_ReceiveSummary")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmS5_ReceiveSummary frm15 = new frmS5_ReceiveSummary();
            frm15.User_Login = User_Login;
            frm15.ConnectionString = Module.TUW99;
            frm15.MdiParent = this;
            frm15.WindowState = FormWindowState.Maximized;
            frm15.Show();
        }
        private void LoadfrmS5_PO(string poType)
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmS5_PO")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmS5_PO frm16 = new frmS5_PO();
            frm16.User_Login = User_Login;
            frm16.ConnectionString = Module.TUW99;
            frm16.MdiParent = this;
            frm16.WindowState = FormWindowState.Maximized;
            frm16.PO_Remark = poType;
            frm16.Show();
        }
        private void LoadfrmS5_FabricOrderSheet()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmS5_FabricOrderSheet")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmS5_FabricOrderSheet frm17 = new frmS5_FabricOrderSheet();
            frm17.ConnectionString = Module.tuwCenter;
            frm17.MdiParent = this;
            frm17.WindowState = FormWindowState.Maximized;
            frm17.Show();
        }
        private void LoadfrmFSIC_InsertComment()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmFSIC_InsertComment")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmFS_InsertComment frm18 = new frmFS_InsertComment();
            frm18.ConnectionString = Module.TUW99;
            frm18.StatusBarEvent+=new frmFS_InsertComment.StatusBarHandler(UpdateStatusBar);
            frm18.MdiParent = this;
            frm18.WindowState = FormWindowState.Maximized;
            frm18.Show();
        }
        private void LoadfrmYS_CheckStock()
        { 
            foreach(DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmYS_CheckStock")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmYS_CheckStock frm19 = new frmYS_CheckStock();
            frm19.ConnectionString = Module.TUW99;
            frm19.StatusBarEvent += new frmYS_CheckStock.StatusBarHandler(UpdateStatusBar);
            frm19.ProgressBarEvent += new frmYS_CheckStock.ProgressBarHandler(UpdateProgressBar);
            frm19.MdiParent = this;
            frm19.WindowState = FormWindowState.Maximized;
            frm19.Show();
        }
        private void LoadfrmYS_Code()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmYS_Code")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmYS_Code frm20 = new frmYS_Code();
            frm20.ConnectionString = Module.TUW99;
            frm20.StatusBarEvent+=new frmYS_Code.StatusBarHandler(UpdateStatusBar);
            frm20.MdiParent = this;
            frm20.WindowState = FormWindowState.Maximized;
            frm20.Show(); 
        }
        private void LoadfrmS5_DyeingSchedule()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmS5_DyeingSchedule")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmS5_DyeingSchedule frm21 = new frmS5_DyeingSchedule();
            frm21.ConnectionString = Module.TUW99;
            frm21.StatusBarEvent+=new frmS5_DyeingSchedule.StatusBarHandler(UpdateStatusBar);
            frm21.MdiParent = this;
            frm21.WindowState = FormWindowState.Maximized;
            frm21.Show();
        }
        private void LoadfrmFS_ChangePrice()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmFS_ChangePrice")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmFS_ChangePrice frm22 = new frmFS_ChangePrice();
            frm22.ConnectionString = Module.TUW99;
            frm22.StatusBarEvent+=new frmFS_ChangePrice.StatusBarHandler(UpdateStatusBar);
            frm22.ProgressBarEvent+=new frmFS_ChangePrice.ProgressBarHandler(UpdateProgressBar);
            frm22.User_Login = User_Login;
            frm22.MdiParent = this;
            frm22.WindowState = FormWindowState.Maximized;
            frm22.Show();
        }
        private void LoadfrmTS1_Receive()
        { 
            foreach(DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_Receive")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_Receive frm23 = new frmTS1_Receive();
            frm23.ConnectionString = Module.Sewing;
            frm23.User_Login = User_Login;
            frm23.MdiParent = this;
            frm23.WindowState = FormWindowState.Maximized;
            frm23.Show();
        }
        private void LoadfrmS5_ReceiveByDate()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmS5_ReceiveByDate")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmS5_ReceiveByDate frm24 = new frmS5_ReceiveByDate();
            frm24.ConnectionString = Module.TUW99;
            frm24.StatusBarEvent+=new frmS5_ReceiveByDate.StatusBarHandler(UpdateStatusBar);
            frm24.MdiParent = this;
            frm24.WindowState = FormWindowState.Maximized;
            frm24.Show();
        }
        private void LoadfrmS5_DyeDaily()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmS5_DyeDaily")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmS5_DyeDaily frm26 = new frmS5_DyeDaily();
            frm26.ConnectionString = Module.TUW99;
            frm26.StatusBarEvent += new frmS5_DyeDaily.StatusBarHandler(UpdateStatusBar);
            frm26.MdiParent = this;
            frm26.WindowState = FormWindowState.Maximized;
            frm26.Show();
        }
        private void LoadfrmTS1_ReceiveCSV()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_ReceiveCSV")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_ReceiveCSV frm27 = new frmTS1_ReceiveCSV();
            frm27.ConnectionString = Module.Sewing;
            frm27.StatusBarEvent += new frmTS1_ReceiveCSV.StatusBarHandler(UpdateStatusBar);
            frm27.MdiParent = this;
            frm27.WindowState = FormWindowState.Maximized;
            frm27.Show();
        }
        private void LoadfrmTS1_PurchaseClose()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_PurchaseClose")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_PurchaseClose frm28 = new frmTS1_PurchaseClose() ;
            frm28.ConnectionString = Module.Sewing;
            frm28.StatusBarEvent += new frmTS1_PurchaseClose.StatusBarHandler(UpdateStatusBar);
            frm28.MdiParent = this;
            frm28.WindowState = FormWindowState.Maximized;
            frm28.Show();
        }
        private void LoadfrmYS_Receive()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmYS_Receive")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmYS_Receive frm29 = new frmYS_Receive();
            frm29.ConnectionString = Module.TUW99;
            frm29.User_Login = User_Login;
            //frm29.StatusBarEvent += new frmYS_Receive.StatusBarHandler(UpdateStatusBar);
            frm29.MdiParent = this;
            frm29.WindowState = FormWindowState.Maximized;
            frm29.Show();
        }
        private void LoadfrmYS_Cost()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmYS_Cost")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmYS_Cost frm30 = new frmYS_Cost();
            frm30.ConnectionString = Module.TUW99;
            frm30.User_Login = User_Login;
            frm30.StatusBarEvent += new frmYS_Cost.StatusBarHandler(UpdateStatusBar);
            frm30.ProgressBarEvent += new frmYS_Cost.ProgressBarHandler(UpdateProgressBar);
            frm30.MdiParent = this;
            frm30.WindowState = FormWindowState.Maximized;
            frm30.Show();
        }
        private void LoadfrmYS_Issue()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmYS_Issue")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmYS_Issue frm31 = new frmYS_Issue();
            frm31.ConnectionString = Module.TUW99;
            frm31.User_Login = User_Login;
            frm31.StatusBarEvent+=new frmYS_Issue.StatusBarHandler(UpdateStatusBar);
            frm31.MdiParent = this;
            frm31.WindowState = FormWindowState.Maximized;
            frm31.Show();
        }
        private void LoadfrmYS_IssueSubcontract()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmYS_IssueSubcontract")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmYS_IssueSubcontract frm32 = new frmYS_IssueSubcontract();
            frm32.ConnectionString = Module.TUW99;
            frm32.User_Login = User_Login;
            frm32.StatusBarEvent+=new frmYS_IssueSubcontract.StatusBarHandler(UpdateStatusBar);
            frm32.MdiParent = this;
            frm32.WindowState = FormWindowState.Maximized;
            frm32.Show();
        }
        private void LoadfrmYS_Report()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmYS_Report")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmYS_Report frm33 = new frmYS_Report();
            frm33.ConnectionString = Module.TUW99;
            frm33.User_Login = User_Login;
            frm33.MdiParent = this;
            frm33.WindowState = FormWindowState.Maximized;
            frm33.Show();
        }
        private void LoadfrmYS_CheckCarton()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmYS_CheckCarton")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmYS_CheckCarton frm34 = new frmYS_CheckCarton();
            frm34.ConnectionString = Module.TUW99;
            frm34.User_Login = User_Login;
            frm34.MdiParent = this;
            frm34.WindowState = FormWindowState.Maximized;
            frm34.Show();
        }
        private void LoadfrmYS_BarcodeText()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmYS_BarcodeText")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmYS_BarcodeText frm35 = new frmYS_BarcodeText();
            frm35.ConnectionString = Module.TUW99;
            frm35.User_Login = User_Login;
            frm35.MdiParent = this;
            frm35.WindowState = FormWindowState.Maximized;
            frm35.Show();
        }
        private void LoadfrmSetting()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmSetting")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmSetting frm36 = new frmSetting();
            frm36.SkinEvent+=new frmSetting.SkinHandler(UpdateSkin);
            frm36.MdiParent = this;
            frm36.WindowState = FormWindowState.Maximized;
            frm36.Show();
        }
        private void LoadfrmYS_ReceiveForm()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmYS_ReceiveForm")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmYS_ReceiveForm frm37 = new frmYS_ReceiveForm();
            frm37.ConnectionString = Module.TUW99;
            frm37.User_Login = User_Login;
            frm37.MdiParent = this;
            frm37.WindowState = FormWindowState.Maximized;
            frm37.Show();
        }
        private void LoadfrmS4_Receive()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmS4_Receive")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmS4_Receive frm38 = new frmS4_Receive();
            frm38.ConnectionString = Module.Parfun;
            frm38.User_Login = User_Login;
            frm38.MdiParent = this;
            frm38.WindowState = FormWindowState.Maximized;
            frm38.Show();
        }
        private void LoadfrmS4_PO()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmS4_PO")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmS4_PO frm39 = new frmS4_PO();
            frm39.ConnectionString = Module.Parfun;
            frm39.User_Login = User_Login;
            frm39.MdiParent = this;
            frm39.WindowState = FormWindowState.Maximized;
            frm39.Show();
        }
        private void LoadfrmS3_Receive()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmS3_Receive")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmS3_Receive frm39 = new frmS3_Receive();
            frm39.ConnectionString = Module.Riki;
            frm39.User_Login = User_Login;
            frm39.MdiParent = this;
            frm39.WindowState = FormWindowState.Maximized;
            frm39.Show();
        }
        private void LoadfrmS3_PO()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmS3_PO")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmS3_PO frm40 = new frmS3_PO();
            frm40.ConnectionString = Module.Riki;
            frm40.User_Login = User_Login;
            frm40.MdiParent = this;
            frm40.WindowState = FormWindowState.Maximized;
            frm40.Show();
        }
        private void LoadfrmAC_Cust()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_Cust")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_Cust  frm41 = new frmAC_Cust();
            frm41.ConnectionString = Module.DBExim;
            //frm41.User_Login = User_Login;
            frm41.MdiParent = this;
            frm41.WindowState = FormWindowState.Maximized;
            frm41.Show();
        }
        private void LoadfrmAC_Descr()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_Descr")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_Descr frm42 = new frmAC_Descr();
            frm42.ConnectionString = Module.DBExim;
            //frm41.User_Login = User_Login;
            frm42.MdiParent = this;
            frm42.WindowState = FormWindowState.Maximized;
            frm42.Show();
        }
        private void LoadfrmAC_LoadDomestic()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_LoadDomestic")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_LoadDomestic frm43 = new frmAC_LoadDomestic();
            frm43.ConnectionString = Module.DBExim;
            //frm41.User_Login = User_Login;
            frm43.MdiParent = this;
            frm43.WindowState = FormWindowState.Maximized;
            frm43.Show();
        }
        private void LoadfrmAC_BankContact()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_BankContact")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_BankContact frm44 = new frmAC_BankContact();
            frm44.ConnectionString = Module.DBExim;
            //frm41.User_Login = User_Login;
            frm44.MdiParent = this;
            frm44.WindowState = FormWindowState.Maximized;
            frm44.Show();
        }
        private void LoadfrmAC_DraftTT()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_DraftTT")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_DraftTT frm45 = new frmAC_DraftTT();
            frm45.ConnectionString = Module.DBExim;
            //frm41.User_Login = User_Login;
            frm45.MdiParent = this;
            frm45.WindowState = FormWindowState.Maximized;
            frm45.Show();
        }
        private void LoadfrmAC_Draft()
        { 
        
        }
        private void LoadfrmAC_Rate()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_Rate")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_Rate frm47 = new frmAC_Rate();
            frm47.ConnectionString = Module.DBExim;
            //frm41.User_Login = User_Login;
            frm47.MdiParent = this;
            frm47.WindowState = FormWindowState.Maximized;
            frm47.Show();
        }
        private void LoadfrmAC_BankRate()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_BankRate")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_BankRate frm48 = new frmAC_BankRate();
            frm48.ConnectionString = Module.DBExim;
            //frm41.User_Login = User_Login;
            frm48.MdiParent = this;
            frm48.WindowState = FormWindowState.Maximized;
            frm48.Show();
        }
        private void LoadfrmAC_UpdateData()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_UpdateData")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_UpdateData frm49 = new frmAC_UpdateData();
            frm49.ConnectionString = Module.DBExim;
            frm49.StatusBarEvent +=new frmAC_UpdateData.StatusBarHandler(UpdateStatusBar);
            frm49.MdiParent = this;
            frm49.WindowState = FormWindowState.Maximized;
            frm49.Show();
        }
        private void LoadfrmAC_AccSales()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_AccSales")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_AccSales frm50 = new frmAC_AccSales();
            frm50.ConnectionString = Module.DBExim;
            frm50.StatusBarEvent += new frmAC_AccSales.StatusBarHandler(UpdateStatusBar);
            frm50.MdiParent = this;
            frm50.WindowState = FormWindowState.Maximized;
            frm50.Show();
        }
        private void LoadfrmAC_ShowDebtor()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_ShowDebtor")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_ShowDebtor frm51 = new frmAC_ShowDebtor();
            frm51.ConnectionString = Module.DBExim;
            frm51.StatusBarEvent += new frmAC_ShowDebtor.StatusBarHandler(UpdateStatusBar);
            frm51.MdiParent = this;
            frm51.WindowState = FormWindowState.Maximized;
            frm51.Show();
        }
        private void LoadfrmAC_Domestic()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_Domestic")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_Domestic frm52 = new frmAC_Domestic();
            frm52.ConnectionString = Module.DBExim;
            frm52.MdiParent = this;
            frm52.WindowState = FormWindowState.Maximized;
            frm52.Show();
        }
        private void LoadfrmAC_DomesticList()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_DomesticList")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_DomesticList frm53 = new frmAC_DomesticList();
            frm53.ConnectionString = Module.DBExim;
            frm53.StatusBarEvent += new frmAC_DomesticList.StatusBarHandler(UpdateStatusBar);
            frm53.MdiParent = this;
            frm53.WindowState = FormWindowState.Maximized;
            frm53.Show();
        }
        private void LoadfrmAC_ColDomestic()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_ColDomestic")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_ColDomestic frm54 = new frmAC_ColDomestic();
            frm54.ConnectionString = Module.DBExim;
            frm54.StatusBarEvent += new frmAC_ColDomestic.StatusBarHandler(UpdateStatusBar);
            frm54.MdiParent = this;
            frm54.WindowState = FormWindowState.Maximized;
            frm54.Show();
        }
        private void LoadfrmAC_DomesticSales()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmAC_DomesticSales")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmAC_DomesticSales frm55 = new frmAC_DomesticSales();
            frm55.ConnectionString = Module.DBExim;
            frm55.StatusBarEvent += new frmAC_DomesticSales.StatusBarHandler(UpdateStatusBar);
            frm55.MdiParent = this;
            frm55.WindowState = FormWindowState.Maximized;
            frm55.Show();
        }
        private void LoadfrmTS1_Declare40()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_Declare40")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_Declare40 frm56 = new frmTS1_Declare40();
            frm56.ConnectionString = Module.ISODocument;
            frm56.ConnectionString2 = Module.TxDemoData40;
            frm56.User_Login = User_Login;
            frm56.MdiParent = this;
            frm56.WindowState = FormWindowState.Maximized;
            frm56.Show();
        }
        private void LoadfrmTS1_CalSeiban()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_CalSeiban")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_CalSeiban frm57 = new frmTS1_CalSeiban();
            frm57.ConnectionString = Module.Sewing;
            frm57.MdiParent = this;
            frm57.WindowState = FormWindowState.Maximized;
            frm57.Show();
        }
        private void LoadfrmTS1_TPiCSContract40()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_TPiCSContract40")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_TPiCSContract40 frm58 = new frmTS1_TPiCSContract40();
            frm58.ConnectionString = Module.TxDemoData40;
            frm58.MdiParent = this;
            frm58.WindowState = FormWindowState.Maximized;
            frm58.Show();
        }
        private void LoadfrmTS1_MaterialConsumption40()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_MaterialConsumption40")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_MaterialConsumption40 frm59 = new frmTS1_MaterialConsumption40();
            frm59.ConnectionString = Module.TxDemoData40;
            frm59.StatusBarEvent += new frmTS1_MaterialConsumption40.StatusBarHandler(UpdateStatusBar);
            frm59.MdiParent = this;
            frm59.WindowState = FormWindowState.Maximized;
            frm59.Show();
        }
        private void LoadfrmTS1_CalSeiban40()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_CalSeiban40")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_CalSeiban40 frm60 = new frmTS1_CalSeiban40();
            frm60.ConnectionString = Module.TxDemoData40;
            frm60.MdiParent = this;
            frm60.WindowState = FormWindowState.Maximized;
            frm60.Show();
        }
        private void LoadfrmTS1_WorkOrder40()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_WorkOrder40")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_WorkOrder40 frm61 = new frmTS1_WorkOrder40();
            frm61.ConnectionString = Module.TxDemoData40;
            frm61.StatusBarEvent += new frmTS1_WorkOrder40.StatusBarHandler(UpdateStatusBar);
            frm61.MdiParent = this;
            frm61.WindowState = FormWindowState.Maximized;
            frm61.Show();
        }
        private void LoadfrmTS1_Purchase40()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_Purchase40")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_Purchase40 frm62 = new frmTS1_Purchase40();
            frm62.ConnectionString = Module.TxDemoData40;
            frm62.User_Login = User_Login;
            frm62.MdiParent = this;
            frm62.WindowState = FormWindowState.Maximized;
            frm62.Show();
        }
        private void LoadfrmTS1_Receive40()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_Receive40")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_Receive40 frm63 = new frmTS1_Receive40();
            frm63.ConnectionString = Module.TxDemoData40;
            frm63.User_Login = User_Login;
            frm63.MdiParent = this;
            frm63.WindowState = FormWindowState.Maximized;
            frm63.Show();
        }
        private void LoadfrmTS1_PurchaseSummary40()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_PurchaseSummary40")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_PurchaseSummary40 frm64 = new frmTS1_PurchaseSummary40();
            frm64.ConnectionString = Module.TxDemoData40;
            frm64.MdiParent = this;
            frm64.WindowState = FormWindowState.Maximized;
            frm64.bsiStatusbar = this.bsiStatus;
            frm64.Show();
        }
        private void LoadfrmTS1_ReceiveCSV40()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmTS1_ReceiveCSV40")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmTS1_ReceiveCSV40 frm65 = new frmTS1_ReceiveCSV40();
            frm65.ConnectionString = Module.TxDemoData40;
            frm65.StatusBarEvent += new frmTS1_ReceiveCSV40.StatusBarHandler(UpdateStatusBar);
            frm65.MdiParent = this;
            frm65.WindowState = FormWindowState.Maximized;
            frm65.Show();
        }

        private void LoadRegistry()
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\TUW\\TUW System");
                if (regKey != null)
                {
                    object keyValue = regKey.GetValue("Skin");
                    if (keyValue != null)
                    {
                        defaultLookAndFeel1.LookAndFeel.SkinName = regKey.GetValue("Skin").ToString();
                    }
                    regKey.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load registry error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveRegistry(string key,object value)
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\TUW\TUW System", true);
                if (regKey == null)
                {
                    regKey = Registry.CurrentUser.CreateSubKey(@"Software\TUW\TUW System");
                }
                regKey.SetValue(key, value);
                //foreach (DevExpress.XtraBars.BarCheckItemLink itemLink in bsiSkin.ItemLinks)
                //{
                //    if (((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked == true)
                //    {
                //        regKey.SetValue("Skin", skinName);
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save to registry error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CheckPermissionForm2(BarItemLinkCollection links)
        {
            foreach (BarItemLink link in links)
            {
                if (link.Item is BarButtonItem)
                {
                    //if (!lstLogin.Contains(link.Item.Tag.ToString())) { link.Item.Enabled = false; }
                    var havePermission = User_Login.Forms.Any(p => p.FormName == link.Item.Tag.ToString());
                    if (!havePermission) { link.Item.Enabled = false; }
                }
                else if (link.Item is BarSubItem)
                {

                    CheckPermissionForm2(((BarSubItem)link.Item).ItemLinks);
                }
            }
        }
        private void CheckPermissionForm()//เช็คว่าผู้ใช้คนนี้สามารถเปิดฟอร์มไหนได้บ้าง
        {
            //lstLogin = (from p in _dtLogin.AsEnumerable() select p["emp_frmname"].ToString()).ToList();
            CheckPermissionForm2(bsiS1.ItemLinks);
            CheckPermissionForm2(bsiS3.ItemLinks);
            CheckPermissionForm2(bsiS4.ItemLinks);
            CheckPermissionForm2(bsiS5.ItemLinks);
            CheckPermissionForm2(bsiFS.ItemLinks);
            CheckPermissionForm2(bsiYS.ItemLinks);
            CheckPermissionForm2(bsiAC.ItemLinks);
        }
        public void UpdateStatusBar(string strInput)
        {
            bsiStatus.Caption = strInput;
        }
        public void UpdateProgressBar(int minValue, int maxValue, int value, string status)
        {
            switch (status)
            {
                case "Initialize":
                    repositoryItemProgressBar1.Minimum=minValue;
                    repositoryItemProgressBar1.Maximum = maxValue;
                    beiProgress.EditValue = minValue;
                    beiProgress.Visibility = BarItemVisibility.Always;
                    break;
                case "Update":
                    beiProgress.EditValue = value;
                    beiProgress.Refresh();
                    break;
                case "Hide":
                    beiProgress.Visibility = BarItemVisibility.Never;
                    break;
            }
        }
        public void UpdateSkin(string skinName)
        {
            defaultLookAndFeel1.LookAndFeel.SkinName = skinName;
        }
        //#region "Progressbar"
        //public void Progressbar_Initialize(int minValue,int maxValue)
        //{ 
        //    repositoryItemProgressBar1.Minimum=minValue;
        //    repositoryItemProgressBar1.Maximum = maxValue;
        //    beiProgress.EditValue = minValue;
        //    beiProgress.Visibility = BarItemVisibility.Always;
        //}
        //public void Progressbar_Update(int value)
        //{
        //    beiProgress.EditValue = value;
        //}
        //public void Progressbar_Hide()
        //{
        //    beiProgress.Visibility = BarItemVisibility.Never;
        //}
        //#endregion
        
        #region "Bar"
        private void bbiNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmActive = (DevExpress.XtraEditors.XtraForm)this.ActiveMdiChild;
            if (frmActive != null)
            {
                switch (frmActive.Name)
                {
                    case "frmTS1_FindFabricCode":
                        ((frmTS1_FindFabricCode)frmActive).NewData();
                        break;
                    case "frmTS1_Declare":
                        ((frmTS1_Declare)frmActive).NewData();
                        break;
                    case "frmTS1_Declare40":
                        ((frmTS1_Declare40)frmActive).NewData();
                        break;
                    case "frmTS1_EditTPiCSCode":
                        ((frmTS1_EditTPiCSCode)frmActive).ClearData();
                        break;
                    case "frmTS1_Purchase":
                        ((frmTS1_Purchase)frmActive).NewData(clearHeader: true, clearDetail: true, clearComboboxPO: true);
                        break;
                    case "frmTS1_Purchase40":
                        ((frmTS1_Purchase40)frmActive).NewData(clearHeader: true, clearDetail: true, clearComboboxPO: true);
                        break;
                    case "frmTS1_PurchaseSummary":
                        ((frmTS1_PurchaseSummary)frmActive).NewData();
                        break;
                    case "frmTS1_PurchaseSummary40":
                        ((frmTS1_PurchaseSummary40)frmActive).NewData();
                        break;
                    case "frmTS1_TPiCSContract":
                        ((frmTS1_TPiCSContract)frmActive).NewData();
                        break;
                    case "frmTS1_TPiCSContract40":
                        ((frmTS1_TPiCSContract40)frmActive).NewData();
                        break;
                    case "frmTS1_MaterialConsumption":
                        ((frmTS1_MaterialConsumption)frmActive).ClearData();
                        break;
                    case "frmTS1_MaterialConsumption40":
                        ((frmTS1_MaterialConsumption40)frmActive).ClearData();
                        break;
                    case "frmTS1_ImportIF":
                        ((frmTS1_ImportIF)frmActive).ClearData();
                        break;
                    case "frmTS1_Holding":
                        ((frmTS1_Holding)frmActive).ClearData();
                        break;
                    case "frmP_Customer":
                        ((frmP_Customer)frmActive).NewData();
                        break;
                    case "frmP_Model":
                        ((frmP_Model)frmActive).NewData();
                        break;
                    case "frmP_ModelCategory":
                        ((frmP_ModelCategory)frmActive).NewData();
                        break;
                    case "frmP_Production":
                        ((frmP_Production)frmActive).NewData();
                        break;
                    case "frmS3_PO":
                        ((frmS3_PO)frmActive).NewData();
                        break;
                    case "frmS3_Receive":
                        ((frmS3_Receive)frmActive).NewData();
                        break;
                    case "frmS4_PO":
                        ((frmS4_PO)frmActive).NewData();
                        break;
                    case "frmS4_Receive":
                        ((frmS4_Receive)frmActive).NewData();
                        break;
                    case "frmS5_Receive":
                        ((frmS5_Receive)frmActive).NewData();
                        break;
                    case "frmS5_ReceiveByDate":
                        ((frmS5_ReceiveByDate)frmActive).NewData();
                        break;
                    case "frmS5_PO":
                        ((frmS5_PO)frmActive).NewData();
                        break;
                    case "frmFS_InsertComment":
                        ((frmFS_InsertComment)frmActive).NewData();
                        break;
                    case "frmFS_ChangePrice":
                        ((frmFS_ChangePrice)frmActive).NewData();
                        break;
                    case "frmYS_CheckStock":
                        ((frmYS_CheckStock)frmActive).NewData();
                        break;
                    case "frmYS_Code":
                        ((frmYS_Code)frmActive).NewData();
                        break;
                    case "frmTS1_Receive":
                        ((frmTS1_Receive)frmActive).NewData();
                        break;
                    case "frmTS1_Receive40":
                        ((frmTS1_Receive40)frmActive).NewData();
                        break;
                    case "frmYS_Receive":
                        ((frmYS_Receive)frmActive).NewData();
                        break;
                    case "frmYS_Cost":
                        ((frmYS_Cost)frmActive).NewData();
                        break;
                    case "frmYS_Issue":
                        ((frmYS_Issue)frmActive).NewData();
                        break;
                    case "frmYS_IssueSubcontract":
                        ((frmYS_IssueSubcontract)frmActive).NewData();
                        break;
                    case "frmYS_Report":
                        ((frmYS_Report)frmActive).NewData();
                        break;
                    case "frmYS_CheckCarton":
                        ((frmYS_CheckCarton)frmActive).NewData();
                        break;
                    case "frmAC_BankContact":
                        ((frmAC_BankContact)frmActive).NewData();
                        break;
                    case "frmAC_Domestic":
                        ((frmAC_Domestic)frmActive).NewData();
                        break;
                }
            }
        }
        private void bbiEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmActive = (DevExpress.XtraEditors.XtraForm)this.ActiveMdiChild;
            if (frmActive != null)
            {
                switch (frmActive.Name)
                {
                    case "frmTS1_Purchase":
                        ((frmTS1_Purchase)frmActive).EditData();
                        break;
                    case "frmTS1_Purchase40":
                        ((frmTS1_Purchase40)frmActive).EditData();
                        break;
                    case "frmTS1_Receive":
                        ((frmTS1_Receive)frmActive).EditData();
                        break;
                    case "frmTS1_Receive40":
                        ((frmTS1_Receive40)frmActive).EditData();
                        break;
                    case "frmTS5_Receive":
                        ((frmS5_Receive)frmActive).EditData();
                        break;
                    case "frmYS_Code":
                        ((frmYS_Code)frmActive).EditData();
                        break;
                }
            }
        }
        private void bbiSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmActive = (DevExpress.XtraEditors.XtraForm)this.ActiveMdiChild;
            if (frmActive != null)
            {
                switch (frmActive.Name)
                {
                    case "frmTS1_Declare":
                        ((frmTS1_Declare)frmActive).SaveData();
                        break;
                    case "frmTS1_Declare40":
                        ((frmTS1_Declare40)frmActive).SaveData();
                        break;
                    case "frmTS1_EditTPiCSCode":
                        ((frmTS1_EditTPiCSCode)frmActive).SaveData();
                        break;
                    case "frmTS1_Purchase":
                        ((frmTS1_Purchase)frmActive).SaveData();
                        break;
                    case "frmTS1_Purchase40":
                        ((frmTS1_Purchase40)frmActive).SaveData();
                        break;
                    case "frmTS1_ImportIF":
                        ((frmTS1_ImportIF)frmActive).SaveData();
                        break;
                    case "frmP_Customer":
                        ((frmP_Customer)frmActive).SaveData();
                        break;
                    case "frmP_Model":
                        ((frmP_Model)frmActive).SaveData();
                        break;
                    case "frmP_ModelCategory":
                        ((frmP_ModelCategory)frmActive).SaveData();
                        break;
                    case "frmP_Production":
                        ((frmP_Production)frmActive).SaveData();
                        break;
                    case "frmS5_Receive":
                        ((frmS5_Receive)frmActive).SaveData();
                        break;
                    case "frmS5_PO":
                        ((frmS5_PO)frmActive).SaveData();
                        break;
                    case "frmFS_InsertComment":
                        ((frmFS_InsertComment)frmActive).SaveData();
                        break;
                    case "frmFS_ChangePrice":
                        ((frmFS_ChangePrice)frmActive).SaveData();
                        break;  
                    case "frmYS_CheckStock":
                        ((frmYS_CheckStock)frmActive).SaveData();
                        break;
                    case "frmYS_Code":
                        ((frmYS_Code)frmActive).SaveData();
                        break;
                    case "frmS5_DyeingSchedule":
                        ((frmS5_DyeingSchedule)frmActive).SaveData();
                        break;
                    case "frmTS1_Receive":
                        ((frmTS1_Receive)frmActive).SaveData();
                        break;
                    case "frmTS1_Receive40":
                        ((frmTS1_Receive40)frmActive).SaveData();
                        break;
                    case "frmS5_DyeDaily":
                        ((frmS5_DyeDaily)frmActive).SaveData();
                        break;
                    case "frmYS_Receive":
                        ((frmYS_Receive)frmActive).SaveData();
                        break;
                    case "frmYS_Cost":
                        ((frmYS_Cost)frmActive).SaveData();
                        break;
                    case "frmYS_Issue":
                        ((frmYS_Issue)frmActive).SaveData();
                        break;
                    case "frmYS_IssueSubcontract":
                        ((frmYS_IssueSubcontract)frmActive).SaveData();
                        break;
                    case "frmYS_ReceiveForm":
                        ((frmYS_ReceiveForm)frmActive).SaveData();
                        break;
                    case "frmAC_Cust":
                        ((frmAC_Cust)frmActive).SaveData();
                        break;
                    case "frmAC_Descr":
                        ((frmAC_Descr)frmActive).SaveData();
                        break;
                    case "frmAC_BankContact":
                        ((frmAC_BankContact)frmActive).SaveData();
                        break;
                    case "frmAC_DraftTT":
                        ((frmAC_DraftTT)frmActive).SaveData();
                        break;
                    case "frmAC_Rate":
                        ((frmAC_Rate)frmActive).SaveData();
                        break; 
                    case "frmAC_BankRate":
                        ((frmAC_BankRate)frmActive).SaveData();
                        break;
                    case "frmAC_UpdateData":
                        ((frmAC_UpdateData)frmActive).SaveData();
                        break;
                    case "frmAC_Domestic":
                        ((frmAC_Domestic)frmActive).SaveData();
                        break;
                    case "frmAC_DomesticList":
                        ((frmAC_DomesticList)frmActive).SaveData();
                        break;
                    case "frmAC_ColDomestic":
                        ((frmAC_ColDomestic)frmActive).SaveData();
                        break;
                }
            }
        }
        private void bbiSaveAs_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmActive = (DevExpress.XtraEditors.XtraForm)this.ActiveMdiChild;
            if (frmActive != null)
            {
                switch (frmActive.Name)
                {
                    case "frmP_Production":
                        ((frmP_Production)frmActive).SaveAsData();
                        break;
                }
            }
        }
        private void bbiDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmActive = (DevExpress.XtraEditors.XtraForm)this.ActiveMdiChild;
            if (frmActive != null)
            {
                switch (frmActive.Name)
                {
                    case "frmTS5_Receive":
                        ((frmS5_Receive)frmActive).DeleteData();
                        break;
                    case "frmAC_Cust":
                        ((frmAC_Cust)frmActive).DeleteData();
                        break;
                    case "frmAC_Descr":
                        ((frmAC_Descr)frmActive).DeleteData();
                        break;
                    case "frmAC_DraftTT":
                        ((frmAC_DraftTT)frmActive).DeleteData();
                        break;
                }
            }
        }
        private void bbiCancel_ItemClick(object sender, ItemClickEventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmActive = (DevExpress.XtraEditors.XtraForm)this.ActiveMdiChild;
            if (frmActive != null)
            {
                switch (frmActive.Name)
                {
                    case "frmTS1_Receive":
                        ((frmTS1_Receive)frmActive).CancelData();
                        break;
                    case "frmTS1_Receive40":
                        ((frmTS1_Receive40)frmActive).CancelData();
                        break;
                }
            }
        }
        private void bbiClear_ItemClick(object sender, ItemClickEventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmActive = (DevExpress.XtraEditors.XtraForm)this.ActiveMdiChild;
            if (frmActive != null)
            {
                switch (frmActive.Name)
                { 
                    case "frmTS1_Receive":
                        ((frmTS1_Receive)frmActive).ClearData();
                        break;
                    case "frmTS1_Receive40":
                        ((frmTS1_Receive40)frmActive).ClearData();
                        break;
                    case "frmTS1_ReceiveCSV":
                        ((frmTS1_ReceiveCSV)frmActive).ClearData();
                        break;
                    case "frmTS1_ReceiveCSV40":
                        ((frmTS1_ReceiveCSV40)frmActive).ClearData();
                        break;
                    case "frmAC_Cust":
                        ((frmAC_Cust)frmActive).ClearData(true);
                        break;
                    case "frmAC_Descr":
                        ((frmAC_Descr)frmActive).ClearData(true);
                        break;
                }
            }
        }
        private void bbiRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmActive = (DevExpress.XtraEditors.XtraForm)this.ActiveMdiChild;
            if (frmActive != null)
            {
                switch (frmActive.Name)
                {
                    case "frmTS1_FindFabricCode":
                        ((frmTS1_FindFabricCode)frmActive).DisplayData();
                        break;
                    case "frmTS1_EditTPiCSCode":
                        ((frmTS1_EditTPiCSCode)frmActive).DataToGrid();
                        break;
                    case "frmTS1_WorkOrder":
                        ((frmTS1_WorkOrder)frmActive).DisplayData();
                        break;
                    case "frmTS1_WorkOrder40":
                        ((frmTS1_WorkOrder40)frmActive).DisplayData();
                        break;
                    case "frmTS1_Purchase":
                        //((frmTS1_Purchase)frmActive).DisplayData();
                        break;
                    case "frmTS1_Purchase40":
                        //((frmTS1_Purchase40)frmActive).DisplayData();
                        break;
                    case "frmTS1_PurchaseSummary":
                        ((frmTS1_PurchaseSummary)frmActive).LoadData();
                        break;
                    case "frmTS1_PurchaseSummary40":
                        ((frmTS1_PurchaseSummary40)frmActive).LoadData();
                        break;
                    case "frmTS1_TPiCSContract":
                        ((frmTS1_TPiCSContract)frmActive).DisplayData();
                        break;
                    case "frmTS1_TPiCSContract40":
                        ((frmTS1_TPiCSContract40)frmActive).DisplayData();
                        break;
                    case "frmTS1_MaterialConsumption":
                        ((frmTS1_MaterialConsumption)frmActive).DisplayData();
                        break;
                    case "frmTS1_MaterialConsumption40":
                        ((frmTS1_MaterialConsumption40)frmActive).DisplayData();
                        break;
                    case "frmTS1_ImportIF":
                        ((frmTS1_ImportIF)frmActive).DisplayData();
                        break;
                    case "frmTS1_Holding":
                        ((frmTS1_Holding)frmActive).DisplayData();
                        break;
                    case "frmP_Customer":
                        ((frmP_Customer)frmActive).NewData();
                        ((frmP_Customer)frmActive).DisplayData();
                        break;
                    case "frmP_Model":
                        ((frmP_Model)frmActive).NewData();
                        ((frmP_Model)frmActive).DisplayData();
                        break;
                    case "frmP_ModelCategory":
                        ((frmP_ModelCategory)frmActive).NewData();
                        ((frmP_ModelCategory)frmActive).DisplayData();
                        break;
                    case "frmP_Production":
                        ((frmP_Production)frmActive).ClearForm(false);
                        ((frmP_Production)frmActive).DisplayData2();
                        break;
                    case "frmS3_PO":
                        ((frmS3_PO)frmActive).DisplayData();
                        break;
                    case "frmS3_Receive":
                        ((frmS3_Receive)frmActive).DisplayData();
                        break;
                    case "frmS4_PO":
                        ((frmS4_PO)frmActive).DisplayData();
                        break;
                    case "frmS4_Receive":
                        ((frmS4_Receive)frmActive).DisplayData();
                        break;
                    case "frmS5_FabricOrderSheet":
                        ((frmS5_FabricOrderSheet)frmActive).NewData();
                        ((frmS5_FabricOrderSheet)frmActive).DisplayData();
                        break;
                    case "frmFS_InsertComment":
                        ((frmFS_InsertComment)frmActive).DisplayData();
                        break;
                    case "frmS5_ReceiveSummary":
                        ((frmS5_ReceiveSummary)frmActive).DisplayData();
                        break;
                    case "frmS5_ReceiveByDate":
                        ((frmS5_ReceiveByDate)frmActive).DisplayData();
                        break;
                    case "frmYS_Code":
                        ((frmYS_Code)frmActive).RefreshData();
                        break;
                    case "frmS5_PO":
                        ((frmS5_PO)frmActive).RefreshData();
                        break;
                    case "frmS5_DyeingSchedule":
                        ((frmS5_DyeingSchedule)frmActive).DisplayData();
                        break; 
                    case "frmS5_DyeDaily":
                        ((frmS5_DyeDaily)frmActive).DisplayData();
                        break;
                    case "frmTS1_ReceiveCSV":
                        ((frmTS1_ReceiveCSV)frmActive).DisplayData();
                        break;
                    case "frmTS1_ReceiveCSV40":
                        ((frmTS1_ReceiveCSV40)frmActive).DisplayData();
                        break;
                    case "frmTS1_PurchaseClose":
                        ((frmTS1_PurchaseClose)frmActive).DisplayData();
                        break;
                    case "frmYS_Cost":
                        ((frmYS_Cost)frmActive).DisplayData();
                        break;
                    case "frmYS_Report":
                        ((frmYS_Report)frmActive).DisplayData();
                        break;
                    case "frmYS_CheckCarton":
                        ((frmYS_CheckCarton)frmActive).DisplayData();
                        break;
                    case "frmAC_UpdateData":
                        ((frmAC_UpdateData)frmActive).DisplayData();
                        break;
                    case "frmAC_AccSales":
                        ((frmAC_AccSales)frmActive).DisplayData();
                        break;
                    case "frmAC_ShowDebtor":
                        ((frmAC_ShowDebtor)frmActive).DisplayData();
                        break;
                    case "frmAC_DomesticList":
                        ((frmAC_DomesticList)frmActive).DisplayData();
                        break;
                }
            }

        }
        private void bbiExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmActive = (DevExpress.XtraEditors.XtraForm)this.ActiveMdiChild;
            if (frmActive != null)
            {
                switch (frmActive.Name)
                {
                    case "frmTS1_FindFabricCode":
                        ((frmTS1_FindFabricCode)frmActive).ExportExcel();
                        break;
                    case "frmTS1_EditTPiCSCode":
                        ((frmTS1_EditTPiCSCode)frmActive).ExportExcel();
                        break;
                    case "frmTS1_Purchase":
                        ((frmTS1_Purchase)frmActive).ExportExcel();
                        break;
                    case "frmTS1_Purchase40":
                        ((frmTS1_Purchase40)frmActive).ExportExcel();
                        break;
                    case "frmTS1_PurchaseSummary":
                        ((frmTS1_PurchaseSummary)frmActive).ExportExcel();
                        break;
                    case "frmTS1_PurchaseSummary40":
                        ((frmTS1_PurchaseSummary40)frmActive).ExportExcel();
                        break;
                    case "frmTS1_WorkOrder":
                        ((frmTS1_WorkOrder)frmActive).ExportExcel();
                        break;
                    case "frmTS1_WorkOrder40":
                        ((frmTS1_WorkOrder40)frmActive).ExportExcel();
                        break;
                    case "frmTS1_TPiCSContract":
                        ((frmTS1_TPiCSContract)frmActive).ExportExcel();
                        break;
                    case "frmTS1_TPiCSContract40":
                        ((frmTS1_TPiCSContract40)frmActive).ExportExcel();
                        break;
                    case "frmTS1_MaterialConsumption":
                        ((frmTS1_MaterialConsumption)frmActive).ExportExcel();
                        break;
                    case "frmTS1_MaterialConsumption40":
                        ((frmTS1_MaterialConsumption40)frmActive).ExportExcel();
                        break;
                    case "frmTS1_Declare":
                        ((frmTS1_Declare)frmActive).ExportExcel();
                        break;
                    case "frmTS1_Declare40":
                        ((frmTS1_Declare40)frmActive).ExportExcel();
                        break;
                    case "frmTS1_ImportIF":
                        ((frmTS1_ImportIF)frmActive).ExportExcel();
                        break;
                    case "frmTS1_Holding":
                        ((frmTS1_Holding)frmActive).ExportExcel();
                        break;
                    case "frmP_Production":
                        ((frmP_Production)frmActive).ExportExcel();
                        break;
                    case "frmYS_CheckStock":
                        ((frmYS_CheckStock)frmActive).ExportExcel();
                        break;
                    case "frmS5_ReceiveSummary":
                        ((frmS5_ReceiveSummary)frmActive).ExportExcel();
                        break;
                    case "frmYS_Code":
                        ((frmYS_Code)frmActive).ExportExcel();
                        break;
                    case "frmS5_DyeingSchedule":
                        ((frmS5_DyeingSchedule)frmActive).ExportExcel();
                        break;
                    case "frmS5_DyeDaily":
                        ((frmS5_DyeDaily)frmActive).ExportExcel();
                        break;
                    case "frmTS1_ReceiveCSV":
                        ((frmTS1_ReceiveCSV)frmActive).ExportExcel();
                        break;
                    case "frmTS1_ReceiveCSV40":
                        ((frmTS1_ReceiveCSV40)frmActive).ExportExcel();
                        break;
                    case "frmYS_Cost":
                        ((frmYS_Cost)frmActive).ExportExcel();
                        break;
                    case "frmYS_CheckCarton":
                        ((frmYS_CheckCarton)frmActive).ExportExcel();
                        break;
                }
            }
        }
        private void bbiCSV_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmActive = (DevExpress.XtraEditors.XtraForm)this.ActiveMdiChild;
            if (frmActive != null)
            {
                switch (frmActive.Name)
                {
                    case "frmTS1_ImportIF":
                        ((frmTS1_ImportIF)frmActive).ExportCSV();
                        break;
                    case "frmTS1_Receive":
                        ((frmTS1_Receive)frmActive).ExportCSV();
                        break;
                    case "frmTS1_Receive40":
                        ((frmTS1_Receive40)frmActive).ExportCSV();
                        break;
                    case "frmTS1_ReceiveCSV":
                        ((frmTS1_ReceiveCSV)frmActive).ExportCSV();
                        break;
                    case "frmTS1_ReceiveCSV40":
                        ((frmTS1_ReceiveCSV40)frmActive).ExportCSV();
                        break;
                }
            }
        }
        private void bbiPrintPreview_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmActive = (DevExpress.XtraEditors.XtraForm)this.ActiveMdiChild;
            if (frmActive != null)
            {
                switch (frmActive.Name)
                {
                    case "frmTS1_WorkOrder":
                        ((frmTS1_WorkOrder)frmActive).PrintPreview();
                        break;
                    case "frmTS1_WorkOrder40":
                        ((frmTS1_WorkOrder40)frmActive).PrintPreview();
                        break;
                    case "frmTS1_Purchase":
                        ((frmTS1_Purchase)frmActive).PrintPreview();
                        break;
                    case "frmTS1_Purchase40":
                        ((frmTS1_Purchase40)frmActive).PrintPreview();
                        break;
                    case "frmTS1_PurchaseSummary":
                        ((frmTS1_PurchaseSummary)frmActive).PrintPreview();
                        break;
                    case "frmTS1_PurchaseSummary40":
                        ((frmTS1_PurchaseSummary40)frmActive).PrintPreview();
                        break;
                    case "frmTS1_Receive":
                        ((frmTS1_Receive)frmActive).PrintPreview();
                        break;
                    case "frmTS1_Receive40":
                        ((frmTS1_Receive40)frmActive).PrintPreview();
                        break;
                    case "frmS3_PO":
                        ((frmS3_PO)frmActive).PrintPreview();
                        break;
                    case "frmS3_Receive":
                        ((frmS3_Receive)frmActive).PrintPreview();
                        break;
                    case "frmS4_PO":
                        ((frmS4_PO)frmActive).PrintPreview();
                        break;
                    case "frmS4_Receive":
                        ((frmS4_Receive)frmActive).PrintPreview();
                        break;
                    case "frmS5_PO":
                        ((frmS5_PO)frmActive).PrintPreview();
                        break;
                    case "frmS5_Receive":
                        ((frmS5_Receive)frmActive).PrintPreview();
                        break;
                    case "frmS5_ReceiveByDate":
                        ((frmS5_ReceiveByDate)frmActive).PrintPreview();
                        break;
                    case "frmYS_Receive":
                        ((frmYS_Receive)frmActive).PrintPreview();
                        break;
                    case "frmYS_Cost":
                        ((frmYS_Cost)frmActive).PrintPreview();
                        break;
                    case "frmYS_Issue":
                        ((frmYS_Issue)frmActive).PrintPreview();
                        break;
                    case "frmYS_IssueSubcontract":
                        ((frmYS_IssueSubcontract)frmActive).PrintPreview();
                        break;
                    case "frmYS_Report":
                        ((frmYS_Report)frmActive).PrintPreview();
                        break;
                    case "frmYS_CheckCarton":
                        ((frmYS_CheckCarton)frmActive).PrintPreview();
                        break;
                    case "frmYS_ReceiveForm":
                        ((frmYS_ReceiveForm)frmActive).PrintPreview();
                        break;
                    case "frmAC_AccSales":
                        ((frmAC_AccSales)frmActive).PrintPreview();
                        break;
                    case "frmAC_ShowDebtor":
                        ((frmAC_ShowDebtor)frmActive).PrintPreview();
                        break;
                }
            }
        }
        private void bbiPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmActive = (DevExpress.XtraEditors.XtraForm)this.ActiveMdiChild;
            if (frmActive != null)
            {
                switch (frmActive.Name)
                {
                    case "frmTS1_WorkOrder":
                        ((frmTS1_WorkOrder)frmActive).Print();
                        break;
                    case "frmTS1_WorkOrder40":
                        ((frmTS1_WorkOrder40)frmActive).Print();
                        break;
                    case "frmTS1_Purchase":
                        ((frmTS1_Purchase)frmActive).Print();
                        break;
                    case "frmTS1_Purchase40":
                        ((frmTS1_Purchase40)frmActive).Print();
                        break;
                    case "frmTS1_PurchaseSummary":
                        ((frmTS1_PurchaseSummary)frmActive).Print();
                        break;
                    case "frmTS1_PurchaseSummary40":
                        ((frmTS1_PurchaseSummary40)frmActive).Print();
                        break;
                    case "frmTS1_Receive":
                        ((frmTS1_Receive)frmActive).Print();
                        break;
                    case "frmTS1_Receive40":
                        ((frmTS1_Receive40)frmActive).Print();
                        break;
                    case "frmS3_PO":
                        ((frmS3_PO)frmActive).Print();
                        break;
                    case "frmS3_Receive":
                        ((frmS3_Receive)frmActive).Print();
                        break;
                    case "frmS4_PO":
                        ((frmS4_PO)frmActive).Print();
                        break;
                    case "frmS4_Receive":
                        ((frmS4_Receive)frmActive).Print();
                        break;
                    case "frmS5_PO":
                        ((frmS5_PO)frmActive).Print();
                        break;
                    case "frmS5_Receive":
                        ((frmS5_Receive)frmActive).Print();
                        break;
                    case "frmS5_ReceiveByDate":
                        ((frmS5_ReceiveByDate)frmActive).Print();
                        break;
                    case "frmYS_Receive":
                        ((frmYS_Receive)frmActive).Print();
                        break;
                    case "frmYS_Cost":
                        ((frmYS_Cost)frmActive).Print();
                        break;
                    case "frmYS_Issue":
                        ((frmYS_Issue)frmActive).Print();
                        break;
                    case "frmYS_IssueSubcontract":
                        ((frmYS_IssueSubcontract)frmActive).Print();
                        break;
                    case "frmYS_Report":
                        ((frmYS_Report)frmActive).Print();
                        break;
                    case "frmYS_CheckCarton":
                        ((frmYS_CheckCarton)frmActive).Print();
                        break;
                    case "frmYS_BarcodeText":
                        ((frmYS_BarcodeText)frmActive).Print();
                        break;
                    case "frmYS_ReceiveForm":
                        ((frmYS_ReceiveForm)frmActive).Print();
                        break;
                    case "frmAC_AccSales":
                        ((frmAC_AccSales)frmActive).Print();
                        break;
                    case "frmAC_ShowDebtor":
                        ((frmAC_ShowDebtor)frmActive).Print();
                        break;
                }
            }
        }
        #endregion

        private void mdiMain_MdiChildActivate(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmActive = (DevExpress.XtraEditors.XtraForm)this.ActiveMdiChild;
            if (frmActive != null)
            {
                //New------------------------------------------------------------------------------------
                switch (frmActive.Name)
                {
                    case "frmTS1_FindFabricCode":
                    case "frmTS1_Declare":
                    case "frmTS1_Declare40":
                    case "frmTS1_EditTPiCSCode":
                    case "frmTS1_TPiCSContract":
                    case "frmTS1_TPiCSContract40":
                    case "frmTS1_Purchase":
                    case "frmTS1_Purchase40":
                    case "frmTS1_Receive":
                    case "frmTS1_Receive40":
                    case "frmTS1_PurchaseSummary":
                    case "frmTS1_PurchaseSummary40":
                    case "frmTS1_MaterialConsumption":
                    case "frmTS1_MaterialConsumption40":
                    case "frmTS1_ImportIF":
                    case "frmTS1_Holding":
                    case "frmP_Customer":
                    case "frmP_Model":
                    case "frmP_ModelCategory":
                    case "frmP_Production":
                    case "frmS3_PO":
                    case "frmS3_Receive":
                    case "frmS4_PO":
                    case "frmS4_Receive":
                    case "frmS5_Receive":
                    case "frmS5_ReceiveByDate":
                    case "frmS5_PO":
                    case "frmS5_FabricOrderSheet":
                    case "frmFS_InsertComment":
                    case "frmFS_ChangePrice":
                    case "frmYS_CheckStock":
                    case "frmYS_Code":
                    case "frmYS_Receive":
                    case "frmYS_Cost":
                    case "frmYS_Issue":
                    case "frmYS_IssueSubcontract":
                    case "frmYS_Report":
                    case "frmYS_CheckCarton":
                    case "frmAC_BankContact":
                    case "frmAC_Domestic":
                        bbiNew.Enabled = true;
                        break;
                    default:
                        bbiNew.Enabled = false;
                        break;
                }
                //Edit------------------------------------------------------------------------------------
                switch (frmActive.Name)
                {
                    case "frmTS1_Purchase":
                    case "frmTS1_Purchase40":
                    case "frmTS1_Receive":
                    case "frmTS1_Receive40":
                    case "frmYS_Code":
                        bbiEdit.Enabled = true;
                        break;
                    default:
                        bbiEdit.Enabled = false;
                        break;
                }
                //Save------------------------------------------------------------------------------------
                switch (frmActive.Name)
                {
                    case "frmTS1_Declare":
                    case "frmTS1_Declare40":
                    case "frmTS1_EditTPiCSCode":
                    case "frmTS1_Purchase":
                    case "frmTS1_Purchase40":
                    case "frmTS1_Receive":
                    case "frmTS1_Receive40":
                    case "frmTS1_ImportIF":
                    case "frmP_Customer":
                    case "frmP_Model":
                    case "frmP_ModelCategory":
                    case "frmP_Production":
                    case "frmS5_Receive":
                    case "frmS5_PO":
                    case "frmFS_InsertComment":
                    case "frmFS_ChangePrice":
                    case "frmYS_CheckStock":
                    case "frmYS_Code":
                    case "frmS5_DyeingSchedule":
                    case "frmS5_DyeDaily":
                    case "frmYS_Receive":
                    case "frmYS_Cost":
                    case "frmYS_Issue":
                    case "frmYS_IssueSubcontract":
                    case "frmYS_ReceiveForm":
                    case "frmAC_Cust":
                    case "frmAC_Descr":
                    case "frmAC_BankContact":
                    case "frmAC_DraftTT":
                    case "frmAC_Rate":
                    case "frmAC_BankRate":
                    case "frmAC_UpdateData":
                    case "frmAC_Domestic":
                    case "frmAC_DomesticList":
                    case "frmAC_ColDomestic":
                        var isSave=(from p in User_Login.Forms where p.FormName==frmActive.Name select p.CanSave).First();
                        bbiSave.Enabled = (isSave) ? true : false;
                        //foreach (DataRow dr in _dtLogin.Rows)
                        //{
                        //    if (Equals(dr["emp_frmname"].ToString(), frmActive.Name))
                        //    {
                        //        if (dr["emp_save"].ToString() == "1")bbiSave.Enabled = true;
                        //        else bbiSave.Enabled = false;
                        //    }
                        //}
                        break;
                    default:
                        bbiSave.Enabled = false;
                        break;
                }
                //Save as------------------------------------------------------------------------------------
                switch (frmActive.Name)
                {
                    case "frmTS1_Declare":
                    case "frmTS1_Declare40":
                    case "frmTS1_EditTPiCSCode":
                    case "frmTS1_Purchase":
                    case "frmTS1_ImportIF":
                    case "frmP_Customer":
                    case "frmP_Model":
                    case "frmP_ModelCategory":
                    case "frmP_Production":
                        var isSave=(from p in User_Login.Forms where p.FormName==frmActive.Name select p.CanSave).First();
                        bbiSaveAs.Enabled = (isSave) ? true : false;
                        //foreach (DataRow dr in _dtLogin.Rows)
                        //{
                        //    if (Equals(dr["emp_frmname"].ToString(), frmActive.Name))
                        //    {
                        //        if (dr["emp_save"].ToString() == "1") bbiSaveAs.Enabled = true;
                        //        else  bbiSaveAs.Enabled = false;
                        //    }
                        //}
                        break;
                    default:
                        bbiSaveAs.Enabled = false;
                        break;
                }
                //Delete------------------------------------------------------------------------------------
                switch (frmActive.Name)
                {
                    case "frmTS5_Receive":
                    case "frmAC_Cust":
                    case "frmAC_Descr":
                    case "frmAC_DraftTT":
                        bbiDelete.Enabled = true;
                        break;
                    default:
                        bbiDelete.Enabled = false;
                        break;
                }
                //Cancel------------------------------------------------------------------------------------
                switch (frmActive.Name)
                { 
                    case "frmTS1_Receive":
                        bbiCancel.Enabled = true;
                        break;
                    case "frmTS1_Receive40":
                        bbiCancel.Enabled = true;
                        break;
                    default:
                        bbiCancel.Enabled = false;
                        break;
                }
                //Clear------------------------------------------------------------------------------------
                switch (frmActive.Name)
                { 
                    case "frmTS1_Receive":
                    case "frmTS1_Receive40":
                    case "frmTS1_ReceiveCSV":
                    case "frmTS1_ReceiveCSV40":
                    case "frmAC_Cust":
                    case "frmAC_Descr":
                        bbiClear.Enabled = true;
                        break;
                    default:
                        bbiClear.Enabled = false;
                        break;
                }
                //Refresh------------------------------------------------------------------------------------
                switch (frmActive.Name)
                {
                    case "frmTS1_FindFabricCode":
                    case "frmTS1_Declare":
                    case "frmTS1_Declare40":
                    case "frmTS1_EditTPiCSCode":
                    case "frmTS1_TPiCSContract":
                    case "frmTS1_TPiCSContract40":
                    case "frmTS1_WorkOrder":
                    case "frmTS1_WorkOrder40":
                    case "frmTS1_Purchase":
                    case "frmTS1_Purchase40":
                    case "frmTS1_PurchaseSummary":
                    case "frmTS1_PurchaseSummary40":
                    case "frmTS1_MaterialConsumption":
                    case "frmTS1_MaterialConsumption40":
                    case "frmTS1_ImportIF":
                    case "frmTS1_Holding":
                    case "frmP_Customer":
                    case "frmP_Model":
                    case "frmP_ModelCategory":
                    case "frmP_Production":
                    case "frmS3_PO":
                    case "frmS3_Receive":
                    case "frmS4_PO":
                    case "frmS4_Receive":
                    case "frmS5_FabricOrderSheet":
                    case "frmFS_InsertComment":
                    case "frmS5_ReceiveSummary":
                    case "frmS5_ReceiveByDate":
                    case "frmYS_Code":
                    case "frmS5_PO":
                    case "frmS5_DyeingSchedule":
                    case "frmS5_DyeDaily":
                    case "frmTS1_ReceiveCSV":
                    case "frmTS1_ReceiveCSV40":
                    case "frmTS1_PurchaseClose":
                    case "frmYS_Cost":
                    case "frmYS_MoneyRate":
                    case "frmYS_Report":
                    case "frmYS_CheckCarton":
                    case "frmAC_UpdateData":
                    case "frmAC_AccSales":
                    case "frmAC_ShowDebtor":
                    case "frmAC_DomesticList":
                        bbiRefresh.Enabled = true;
                        break;
                    default:
                        bbiRefresh.Enabled = false;
                        break;
                }
                //Excel------------------------------------------------------------------------------------
                switch (frmActive.Name)
                {
                    case "frmTS1_FindFabricCode":
                    case "frmTS1_EditTPiCSCode":
                    case "frmTS1_TPiCSContract":
                    case "frmTS1_TPiCSContract40":
                    case "frmTS1_WorkOrder":
                    case "frmTS1_WorkOrder40":
                    case "frmTS1_Purchase":
                    case "frmTS1_Purchase40":
                    case "frmTS1_PurchaseSummary":
                    case "frmTS1_PurchaseSummary40":
                    case "frmTS1_MaterialConsumption":
                    case "frmTS1_MaterialConsumption40":
                    case "frmTS1_Declare":
                    case "frmTS1_Declare40":
                    case "frmTS1_ImportIF":
                    case "frmTS1_Holding":
                    case "frmP_Production":
                    case "frmYS_CheckStock":
                    case "frmS5_ReceiveSummary":
                    case "frmYS_Code":
                    case "frmS5_DyeingSchedule":
                    case "frmS5_DyeDaily":
                    case "frmYS_Cost":
                    case "frmYS_CheckCarton":
                        bbiExcel.Enabled = true;
                        break;
                    default:
                        bbiExcel.Enabled = false;
                        break;
                }
                //PrintPreview------------------------------------------------------------------------------------
                switch (frmActive.Name)
                {
                    case "frmTS1_WorkOrder":
                    case "frmTS1_WorkOrder40":
                    case "frmTS1_Purchase":
                    case "frmTS1_Purchase40":
                    case "frmTS1_Receive":
                    case "frmTS1_Receive40":
                    case "frmTS1_PurchaseSummary":
                    case "frmTS1_PurchaseSummary40":
                    case "frmS3_PO":
                    case "frmS3_Receive":
                    case "frmS4_PO":
                    case "frmS4_Receive":
                    case "frmS5_Receive":
                    case "frmS5_ReceiveByDate":
                    case "frmS5_PO":
                    case "frmYS_Cost":
                    case "frmYS_Issue":
                    case "frmYS_IssueSubcontract":
                    case "frmYS_Report":
                    case "frmYS_CheckCarton":
                    case "frmYS_Receive":
                    case "frmYS_ReceiveForm":
                    case "frmAC_AccSales":
                    case "frmAC_ShowDebtor":
                    
                        var isPrint=(from p in User_Login.Forms where p.FormName==frmActive.Name select p.CanPrint).First();
                        bbiPrintPreview.Enabled = (isPrint) ? true : false;
                        //foreach (DataRow dr in _dtLogin.Rows)
                        //{
                        //    if (Equals(dr["emp_frmname"].ToString(), frmActive.Name))
                        //    {
                        //        if (dr["emp_print"].ToString() == "1")
                        //        {
                        //            bbiPrintPreview.Enabled = true;
                        //        }
                        //        else
                        //        {
                        //            bbiPrintPreview.Enabled = false;
                        //        }
                        //    }
                        //}
                        break;
                    default:
                        bbiPrintPreview.Enabled = false;
                        break;
                }
                //Print------------------------------------------------------------------------------------
                switch (frmActive.Name)
                {
                    case "frmTS1_WorkOrder":
                    case "frmTS1_WorkOrder40":
                    case "frmTS1_Purchase":
                    case "frmTS1_Purchase40":
                    case "frmTS1_PurchaseSummary":
                    case "frmTS1_PurchaseSummary40":
                    case "frmTS1_Receive":
                    case "frmTS1_Receive40":
                    case "frmS3_PO":
                    case "frmS3_Receive":
                    case "frmS4_PO":
                    case "frmS4_Receive":
                    case "frmS5_Receive":
                    case "frmS5_ReceiveByDate":
                    case "frmS5_PO":
                    case "frmYS_Cost":
                    case "frmYS_Issue":
                    case "frmYS_IssueSubcontract":
                    case "frmYS_Report":
                    case "frmYS_CheckCarton":
                    case "frmYS_Receive":
                    case "frmYS_BarcodeText":
                    case "frmYS_ReceiveForm":
                    case "frmAC_AccSales":
                    case "frmAC_ShowDebtor":
                    
                        var isPrint=(from p in User_Login.Forms where p.FormName==frmActive.Name select p.CanPrint).First();
                        bbiPrint.Enabled = (isPrint) ? true : false;
                        //foreach (DataRow dr in _dtLogin.Rows)
                        //{
                        //    if (Equals(dr["emp_frmname"].ToString(), frmActive.Name))
                        //    {
                        //        if (dr["emp_print"].ToString() == "1")
                        //        {
                        //            bbiPrint.Enabled = true;
                        //        }
                        //        else
                        //        {
                        //            bbiPrint.Enabled = false;
                        //        }
                        //    }
                        //}
                        break;
                    default:
                        bbiPrint.Enabled = false;
                        break;
                }
                //CSV------------------------------------------------------------------------------------
                switch (frmActive.Name)
                {
                    case "frmTS1_ImportIF":
                    case "frmTS1_ReceiveCSV":
                    case "frmTS1_ReceiveCSV40":
                        bbiCSV.Enabled = true;
                        break;
                    default:
                        bbiCSV.Enabled = false;
                        break;
                }
            }
            else
            {
                foreach (BarItemLink itemLink in ribbonPageGroup1.ItemLinks) { itemLink.Item.Enabled = false; }//Disable ribbonpagegroup File
                foreach (BarItemLink itemLink in ribbonPageGroup2.ItemLinks) { itemLink.Item.Enabled = false; }//Disable ribbonpagegroup Print
                foreach (BarItemLink itemLink in ribbonPageGroup3.ItemLinks) { itemLink.Item.Enabled = false; }//Disable ribbonpagegroup Export
                foreach (BarItemLink itemLink in ribbonPageGroup4.ItemLinks) { itemLink.Item.Enabled = false; }//Disable ribbonpagegroup Refresh
                foreach (BarItemLink itemLink in ribbonPageGroup5.ItemLinks) { itemLink.Item.Enabled = false; }//Disable ribbonpagegroup Edit
            }

        }
        private void mdiMain_Load(object sender, EventArgs e)
        {
            //LoadSkinIcons();
            SetSkinIcons();
            LoadRegistry();
            //foreach (BarItemLink itemLink in bsiS1.ItemLinks) { itemLink.Item.Enabled = false; }//Disable menu Sales1
            //foreach (BarItemLink itemLink in bsiS5.ItemLinks) { itemLink.Item.Enabled = false; }//Disable menu Sales5
            //foreach (BarItemLink itemLink in bsiFS.ItemLinks) { itemLink.Item.Enabled = false; }//Disable menu Fabric Stock
            foreach (BarItemLink itemLink in ribbonPageGroup1.ItemLinks) { itemLink.Item.Enabled = false; }//Disable ribbonpagegroup File
            foreach (BarItemLink itemLink in ribbonPageGroup2.ItemLinks) { itemLink.Item.Enabled = false; }//Disable ribbonpagegroup Print
            foreach (BarItemLink itemLink in ribbonPageGroup3.ItemLinks) { itemLink.Item.Enabled = false; }//Disable ribbonpagegroup Export
            foreach (BarItemLink itemLink in ribbonPageGroup4.ItemLinks) { itemLink.Item.Enabled = false; }//Disable ribbonpagegroup Refresh
            foreach (BarItemLink itemLink in ribbonPageGroup5.ItemLinks) { itemLink.Item.Enabled = false; }//Disable ribbonpagegroup Edit
            CheckPermissionForm();
            //beiProgress.Visibility = BarItemVisibility.Never;
        }
        private void mdiMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
            Application.Exit();
        }

        #region "Sales1"

        #region "Production Order"

        private void bbiP_Production_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmP_Production();
        }
        private void bbiP_Customer_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmP_Customer();
        }
        private void bbiP_Model_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmP_Model();
        }
        private void bbiP_ModelCategory_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmP_ModelCategory();
        }

        #endregion

        #region "TPiCS Subprogram"

        private void bbiCalSeiban_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_CalSeiban();
        }
        private void bbiTS1_FindFabric_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmTS1_FindFabricCode();
        }
        private void bbiTS1_Declare_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmTS1_Declare();
        }
        private void bbiTS1_EditTPiCSCode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmTS1_EditTPiCSCode();
        }
        private void bbiTS1_TPiCSContract_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmTS1_TPiCSContract();
        }
        private void bbiTS1_WorkOrder_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmTS1_WorkOrder();
        }
        private void bbiTS1_Purchase_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmTS1_Purchase();
        }
        private void bbiTS1_PurchaseClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_PurchaseClose();
        }
        private void bbiReceive_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_Receive();
        }
        private void bbiGenCSV_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_ReceiveCSV();
        }
        private void bbiTS1_PurchaseSummary_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmTS1_PurchaseSummary();
        }
        private void bbiTS1_MaterialConsumption_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmTS1_MaterialConsumption();
        }
        private void bbiTS1_ImportIF_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmTS1_ImportIF();
        }
        private void bbiTS1_Holding_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmTS1_Holding();
        }

        #endregion

        #region "TPiCS 4.0"

        private void bbiCalSeiban40_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_CalSeiban40();
        }
        private void bbiDeclareCodeTPiCS40_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_Declare40();
        }
        private void bbiTPiCSContract40_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_TPiCSContract40();
        }
        private void bbiBOMUse40_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_MaterialConsumption40();
        }
        private void bbiWorkOrder40_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_WorkOrder40();
        }
        private void bbiTS1_Purchase40_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_Purchase40();
        }
        private void bbiTS1_Receive40_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_Receive40();
        }
        private void bbiSummaryPO40_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_PurchaseSummary40();
        }
        private void bbiTS1_ReceiveCSV40_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_ReceiveCSV40();
        }

        #endregion

        #endregion

        #region "Sales3"

        private void bbiS3_Purchase_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmS3_PO();
        }
        private void bbiS3_Receive_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmS3_Receive();
        }        
        
        #endregion

        #region "Sales4"

        private void bbiS4_Purchase_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmS4_PO();
        }
        private void bbiS4_Receive_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmS4_Receive();
        }
        
        #endregion

        #region "Sales5"

        private void bbiFindFabric5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmTS1_FindFabricCode();
        }
        private void bbiGenCode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
        private void bbiS5_ReceiveSummary_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmS5_ReceiveSummary();
        }
        private void bbiS5_ReceiveByDate_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmS5_ReceiveByDate();
        }
        private void bbiTS5_PO_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmS5_PO("RAW YARN");
        }
        private void bbiS5_FabricOrderSheet_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmS5_FabricOrderSheet();
        }
        private void bbiS5_POYarn_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmS5_PO("RAW YARN");
        }
        private void bbiS5_POKnit_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmS5_PO("KNITTING FEE");
        }
        private void bbiS5_PODye_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmS5_PO("DYEING FEE");
        }
        private void bbiS5_Receive_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmS5_ReceiveSummary();
        }
        private void bbiS5_DyeingSchedule_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmS5_DyeingSchedule();
        }
        private void bbiS5_DyeDaily_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmS5_DyeDaily();
        }
        
        #endregion

        #region "Fabric Stock"

        private void bbiFS_InsertComment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmFSIC_InsertComment();
        }     
        private void bbiFS_ChangePrice_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmFS_ChangePrice();
        }
        
        #endregion

        #region "Yarn Stock"

        private void bbiYS_CheckStock_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmYS_CheckStock();
        }
        private void bbiYS_Code_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmYS_Code();
        }
        private void bbiYS_Receive_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmYS_Receive();
        }
        private void bbiYS_Cost_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmYS_Cost();
        }
        private void bbiYS_Issue_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmYS_Issue();
        }
        private void bbiYS_IssueSubcontract_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmYS_IssueSubcontract();
        }
        private void bbiYS_Report_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmYS_Report();
        }
        private void bbiYS_CheckCarton_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmYS_CheckCarton();
        }
        private void bbiYS_BarcodeText_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmYS_BarcodeText();
        }
        
        #endregion

        #region "Account"

        private void bbiAC_Cust_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_Cust();
        }
        private void bbiAC_Descr_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_Descr();
        }
        private void bbiAC_LoadDomestic_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_LoadDomestic();
        }
        private void bbiAC_BankContact_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_BankContact();
        }
        private void bbiAC_DraftTT_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_DraftTT();
        }
        private void bbiAC_Draft_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_Draft();
        }
        private void bbiAC_Rate_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_Rate();
        }
        private void bbiAC_BankRate_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_BankRate();
        }
        private void bbiAC_UpdateData_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_UpdateData();
        }
        private void bbiAC_AccSales_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_AccSales();
        }
        private void bbiAC_ShowDebtor_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_ShowDebtor();
        }
        private void bbiAC_Domestic_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_Domestic();
        }
        private void bbiAC_DomesticList_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_DomesticList();
        }
        private void bbiAC_ColDomestic_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_ColDomestic();
        }
        private void bbiAC_DomesticSales_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmAC_DomesticSales();
        }

        #endregion

        #region "Skin"

        private void SetSkinIcons()
        {
            foreach (GalleryItem item in galleryDropDown1.Gallery.Groups[0].Items)
            {
                item.Image = SkinCollectionHelper.GetSkinIcon(item.Value.ToString(), SkinIconsSize.Large);
            }
            foreach (GalleryItem item in galleryDropDown1.Gallery.Groups[1].Items)
            {
                item.Image = SkinCollectionHelper.GetSkinIcon(item.Value.ToString(), SkinIconsSize.Large);
            }
            foreach (GalleryItem item in galleryDropDown1.Gallery.Groups[2].Items)
            {
                item.Image = SkinCollectionHelper.GetSkinIcon(item.Value.ToString(), SkinIconsSize.Large);
            }
        }
        private void galleryDropDown1_GalleryItemClick(object sender, GalleryItemClickEventArgs e)
        {
            defaultLookAndFeel1.LookAndFeel.SkinName = e.Item.Value.ToString();
            SaveRegistry("Skin",e.Item.Value.ToString());
        }
        
        #endregion

        #region "Help"

        private void bbiSetting_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmSetting();
        }
        
        #endregion

        private void ribbonControl1_MinimizedChanged(object sender, EventArgs e)
        {
            if (ribbonControl1.ToolbarLocation == DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Below)
                ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            else
                ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Below;
        }

        


        

        

        

        

        

        

        

        

        

        



        

        

        

        

        

        

        


        















     
    }
}