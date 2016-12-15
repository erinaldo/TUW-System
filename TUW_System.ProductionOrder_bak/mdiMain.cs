using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.Skins;
using Microsoft.Win32;
using System.Linq;
using TUW_System.S5_ReceiveByDate;

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
            frm5.MdiParent = this;
            frm5.bsiStatusbar = this.bsiStatus;
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
            frm18.MdiParent = this;
            frm18.WindowState = FormWindowState.Maximized;
            frm18.Show();
        }
        private void LoadfrmS5_YarnCheckStock()
        { 
            foreach(DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmS5_YarnCheckStock")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmS5_YarnCheckStock frm19 = new frmS5_YarnCheckStock();
            frm19.MdiParent = this;
            frm19.WindowState = FormWindowState.Maximized;
            frm19.Show();
        }
        private void LoadfrmS5_YarnCode()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmS5_YarnCode")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmS5_YarnCode frm20 = new frmS5_YarnCode(); 
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
            frm24.StatusBarText = bsiStatus.Caption;
            frm24.MdiParent = this;
            frm24.WindowState = FormWindowState.Maximized;
            frm24.Show();
        }

        private void LoadfrmGenCode()
        {
            foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            {
                if (frmActive.Name == "frmGenCode")
                {
                    frmActive.Activate();
                    return;
                }
            }
            frmGenCode frm15 = new frmGenCode();
            frm15.MdiParent = this;
            frm15.WindowState = FormWindowState.Maximized;
            frm15.Show();
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
        private void SaveRegistry()
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\TUW\\TUW System", true);
                if (regKey == null)
                {
                    regKey = Registry.CurrentUser.CreateSubKey("Software\\TUW\\TUW System");
                }
                foreach (DevExpress.XtraBars.BarCheckItemLink itemLink in bsiSkin.ItemLinks)
                {
                    if (((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked == true)
                    {
                        regKey.SetValue("Skin", itemLink.Caption);
                    }
                }
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
            CheckPermissionForm2(bsiS5.ItemLinks);
            CheckPermissionForm2(bsiFS.ItemLinks);
        }
        public void UpdateStatusBar(string strInput)
        {
            bsiStatus.Caption = strInput;
        }
        #region "Progressbar"
        public void Progressbar_Initialize(int minValue,int maxValue)
        { 
            repositoryItemProgressBar1.Minimum=minValue;
            repositoryItemProgressBar1.Maximum = maxValue;
            beiProgress.EditValue = minValue;
            beiProgress.Visibility = BarItemVisibility.Always;
        }
        public void Progressbar_Update(int value)
        {
            beiProgress.EditValue = value;
        }
        public void Progressbar_Hide()
        {
            beiProgress.Visibility = BarItemVisibility.Never;
        }
        #endregion
        

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
                    case "frmTS1_EditTPiCSCode":
                        ((frmTS1_EditTPiCSCode)frmActive).ClearData();
                        break;
                    case "frmTS1_Purchase":
                        ((frmTS1_Purchase)frmActive).NewData(clearHeader: true, clearDetail: true, clearComboboxPO: true);
                        break;
                    case "frmTS1_PurchaseSummary":
                        ((frmTS1_PurchaseSummary)frmActive).NewData();
                        break;
                    case "frmTS1_TPiCSContract":
                        ((frmTS1_TPiCSContract)frmActive).NewData();
                        break;
                    case "frmTS1_MaterialConsumption":
                        ((frmTS1_MaterialConsumption)frmActive).ClearData();
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
                    case "frmS5_YarnCheckStock":
                        ((frmS5_YarnCheckStock)frmActive).NewData();
                        break;
                    case "frmS5_YarnCode":
                        ((frmS5_YarnCode)frmActive).NewData();
                        break;
                    case "frmTS1_Receive":
                        ((frmTS1_Receive)frmActive).NewData();
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
                    case "frmTS5_Receive":
                        ((frmS5_Receive)frmActive).EditData();
                        break;
                    case "frmS5_YarnCode":
                        ((frmS5_YarnCode)frmActive).EditData();
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
                    case "frmTS1_EditTPiCSCode":
                        ((frmTS1_EditTPiCSCode)frmActive).SaveData();
                        break;
                    case "frmTS1_Purchase":
                        ((frmTS1_Purchase)frmActive).SaveData();
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
                    case "frmS5_YarnCheckStock":
                        ((frmS5_YarnCheckStock)frmActive).SaveData();
                        break;
                    case "frmS5_YarnCode":
                        ((frmS5_YarnCode)frmActive).SaveData();
                        break;
                    case "frmS5_DyeingSchedule":
                        ((frmS5_DyeingSchedule)frmActive).SaveData();
                        break;
                    case "frmTS1_Receive":
                        ((frmTS1_Receive)frmActive).SaveData();
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
                        ((frmTS1_WorkOrder)frmActive).LoadData();
                        break;
                    case "frmTS1_Purchase":
                        //((frmTS1_Purchase)frmActive).DisplayData();
                        break;
                    case "frmTS1_PurchaseSummary":
                        ((frmTS1_PurchaseSummary)frmActive).LoadData();
                        break;
                    case "frmTS1_TPiCSContract":
                        ((frmTS1_TPiCSContract)frmActive).DisplayData();
                        break;
                    case "frmTS1_MaterialConsumption":
                        ((frmTS1_MaterialConsumption)frmActive).DisplayData();
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
                    case "frmS5_YarnCode":
                        ((frmS5_YarnCode)frmActive).RefreshData();
                        break;
                    case "frmS5_PO":
                        ((frmS5_PO)frmActive).RefreshData();
                        break;
                    case "frmS5_DyeingSchedule":
                        ((frmS5_DyeingSchedule)frmActive).DisplayData();
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
                    case "frmTS1_PurchaseSummary":
                        ((frmTS1_PurchaseSummary)frmActive).ExportExcel();
                        break;
                    case "frmTS1_WorkOrder":
                        ((frmTS1_WorkOrder)frmActive).ExportExcel();
                        break;
                    case "frmTS1_TPiCSContract":
                        ((frmTS1_TPiCSContract)frmActive).ExportExcel();
                        break;
                    case "frmTS1_MaterialConsumption":
                        ((frmTS1_MaterialConsumption)frmActive).ExportExcel();
                        break;
                    case "frmTS1_Declare":
                        ((frmTS1_Declare)frmActive).ExportExcel();
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
                    case "frmS5_YarnCheckStock":
                        ((frmS5_YarnCheckStock)frmActive).ExportExcel();
                        break;
                    case "frmS5_ReceiveSummary":
                        ((frmS5_ReceiveSummary)frmActive).ExportExcel();
                        break;
                    case "frmS5_YarnCode":
                        ((frmS5_YarnCode)frmActive).ExportExcel();
                        break;
                    case "frmS5_DyeingSchedule":
                        ((frmS5_DyeingSchedule)frmActive).ExportExcel();
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
                    case "frmTS1_Purchase":
                        ((frmTS1_Purchase)frmActive).PrintPreview();
                        break;
                    case "frmTS1_PurchaseSummary":
                        ((frmTS1_PurchaseSummary)frmActive).PrintPreview();
                        break;
                    case "frmTS1_Receive":
                        ((frmTS1_Receive)frmActive).PrintPreview();
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
                    case "frmTS1_Purchase":
                        ((frmTS1_Purchase)frmActive).Print();
                        break;
                    case "frmTS1_PurchaseSummary":
                        ((frmTS1_PurchaseSummary)frmActive).Print();
                        break;
                    case "frmTS1_Receive":
                        ((frmTS1_Receive)frmActive).Print();
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
                }
            }
        }
        #endregion
        #region "Skin"
        private void LoadSkinIcons()
        {
            barCheckItem1.Glyph = SkinCollectionHelper.GetSkinIcon("DevExpress Style", SkinIconsSize.Small);
            barCheckItem2.Glyph = SkinCollectionHelper.GetSkinIcon("Caramel", SkinIconsSize.Small);
            barCheckItem3.Glyph = SkinCollectionHelper.GetSkinIcon("Money Twins", SkinIconsSize.Small);
            barCheckItem4.Glyph = SkinCollectionHelper.GetSkinIcon("Lilian", SkinIconsSize.Small);
            barCheckItem5.Glyph = SkinCollectionHelper.GetSkinIcon("DevExpress Dark Style", SkinIconsSize.Small);
            barCheckItem6.Glyph = SkinCollectionHelper.GetSkinIcon("iMaginary", SkinIconsSize.Small);
            barCheckItem7.Glyph = SkinCollectionHelper.GetSkinIcon("Black", SkinIconsSize.Small);
            barCheckItem8.Glyph = SkinCollectionHelper.GetSkinIcon("Blue", SkinIconsSize.Small);
            barCheckItem9.Glyph = SkinCollectionHelper.GetSkinIcon("Coffee", SkinIconsSize.Small);
            barCheckItem10.Glyph = SkinCollectionHelper.GetSkinIcon("Liquid Sky", SkinIconsSize.Small);
            barCheckItem11.Glyph = SkinCollectionHelper.GetSkinIcon("London Liquid Sky", SkinIconsSize.Small);
            barCheckItem12.Glyph = SkinCollectionHelper.GetSkinIcon("Glass Oceans", SkinIconsSize.Small);
            barCheckItem13.Glyph = SkinCollectionHelper.GetSkinIcon("Stardust", SkinIconsSize.Small);
            barCheckItem14.Glyph = SkinCollectionHelper.GetSkinIcon("Xmas 2008 Blue", SkinIconsSize.Small);
            barCheckItem15.Glyph = SkinCollectionHelper.GetSkinIcon("Valentine", SkinIconsSize.Small);
            barCheckItem16.Glyph = SkinCollectionHelper.GetSkinIcon("McSkin", SkinIconsSize.Small);
            barCheckItem17.Glyph = SkinCollectionHelper.GetSkinIcon("Summer 2008", SkinIconsSize.Small);
            barCheckItem18.Glyph = SkinCollectionHelper.GetSkinIcon("Pumpkin", SkinIconsSize.Small);
            barCheckItem19.Glyph = SkinCollectionHelper.GetSkinIcon("Dark Side", SkinIconsSize.Small);
            barCheckItem20.Glyph = SkinCollectionHelper.GetSkinIcon("Springtime", SkinIconsSize.Small);
            barCheckItem21.Glyph = SkinCollectionHelper.GetSkinIcon("Darkroom", SkinIconsSize.Small);
            barCheckItem22.Glyph = SkinCollectionHelper.GetSkinIcon("Foggy", SkinIconsSize.Small);
            barCheckItem23.Glyph = SkinCollectionHelper.GetSkinIcon("High Contrast", SkinIconsSize.Small);
            barCheckItem24.Glyph = SkinCollectionHelper.GetSkinIcon("Seven", SkinIconsSize.Small);
            barCheckItem25.Glyph = SkinCollectionHelper.GetSkinIcon("Seven Classic", SkinIconsSize.Small);
            barCheckItem26.Glyph = SkinCollectionHelper.GetSkinIcon("Sharp", SkinIconsSize.Small);
            barCheckItem27.Glyph = SkinCollectionHelper.GetSkinIcon("Sharp Plus", SkinIconsSize.Small);
            barCheckItem28.Glyph = SkinCollectionHelper.GetSkinIcon("The Asphalt World", SkinIconsSize.Small);
            barCheckItem29.Glyph = SkinCollectionHelper.GetSkinIcon("Blueprint", SkinIconsSize.Small);
            barCheckItem30.Glyph = SkinCollectionHelper.GetSkinIcon("Whiteprint", SkinIconsSize.Small);
            barCheckItem31.Glyph = SkinCollectionHelper.GetSkinIcon("VS2010", SkinIconsSize.Small);
        }
        private void barCheckItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem1.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "DevExpress Style";
        }
        private void barCheckItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem2.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Caramel";
        }
        private void barCheckItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem3.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Money Twins";
        }
        private void barCheckItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem4.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Lilian";
        }
        private void barCheckItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem5.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "DevExpress Dark Style";
        }
        private void barCheckItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem6.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "iMaginary";
        }
        private void barCheckItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem7.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Black";
        }
        private void barCheckItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem8.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Blue";
        }
        private void barCheckItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem9.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Coffee";
        }
        private void barCheckItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem10.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Liquid Sky";
        }
        private void barCheckItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem11.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "London Liquid Sky";
        }
        private void barCheckItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem12.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Glass Oceans";
        }
        private void barCheckItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem13.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Stardust";
        }
        private void barCheckItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem14.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Xmas 2008 Blue";
        }
        private void barCheckItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem15.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Valentine";
        }
        private void barCheckItem16_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem16.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "McSkin";
        }
        private void barCheckItem17_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem17.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Summer 2008";
        }
        private void barCheckItem18_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem18.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Pumpkin";
        }
        private void barCheckItem19_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem19.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Dark Side";
        }
        private void barCheckItem20_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem20.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Springtime";
        }
        private void barCheckItem21_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem21.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Darkroom";
        }
        private void barCheckItem22_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem22.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Foggy";
        }
        private void barCheckItem23_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem23.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "High Contrast";
        }
        private void barCheckItem24_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem24.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Seven";
        }
        private void barCheckItem25_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem25.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Seven Classic";
        }
        private void barCheckItem26_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem26.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Sharp";
        }
        private void barCheckItem27_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem27.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Sharp Plus";
        }
        private void barCheckItem28_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem28.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "The Asphalt World";
        }
        private void barCheckItem29_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem29.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Blueprint";
        }
        private void barCheckItem30_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem30.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "Whiteprint";
        }
        private void barCheckItem31_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (DevExpress.XtraBars.BarItemLink itemLink in bsiSkin.ItemLinks)
            {
                ((DevExpress.XtraBars.BarCheckItem)itemLink.Item).Checked = false;
            }
            barCheckItem31.Checked = true;
            defaultLookAndFeel1.LookAndFeel.SkinName = "VS2010";
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
                    case "frmTS1_EditTPiCSCode":
                    case "frmTS1_TPiCSContract":
                    case "frmTS1_Purchase":
                    case "frmTS1_Receive":
                    case "frmTS1_PurchaseSummary":
                    case "frmTS1_MaterialConsumption":
                    case "frmTS1_ImportIF":
                    case "frmTS1_Holding":
                    case "frmP_Customer":
                    case "frmP_Model":
                    case "frmP_ModelCategory":
                    case "frmP_Production":
                    case "frmS5_Receive":
                    case "frmS5_ReceiveByDate":
                    case "frmS5_PO":
                    case "frmS5_FabricOrderSheet":
                    case "frmFS_InsertComment":
                    case "frmFS_ChangePrice":
                    case "frmS5_YarnCheckStock":
                    case "frmS5_YarnCode":
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
                    case "frmS5_YarnCode":
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
                    case "frmTS1_EditTPiCSCode":
                    case "frmTS1_Purchase":
                    case "frmTS1_Receive":
                    case "frmTS1_ImportIF":
                    case "frmP_Customer":
                    case "frmP_Model":
                    case "frmP_ModelCategory":
                    case "frmP_Production":
                    case "frmS5_Receive":
                    case "frmS5_PO":
                    case "frmFS_InsertComment":
                    case "frmFS_ChangePrice":
                    case "frmS5_YarnCheckStock":
                    case "frmS5_YarnCode":
                    case "frmS5_DyeingSchedule":
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
                        bbiDelete.Enabled = true;
                        break;
                    default:
                        bbiDelete.Enabled = false;
                        break;
                }
                //Clear------------------------------------------------------------------------------------
                switch (frmActive.Name)
                { 
                    case "frmTS1_Receive":
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
                    case "frmTS1_EditTPiCSCode":
                    case "frmTS1_TPiCSContract":
                    case "frmTS1_WorkOrder":
                    case "frmTS1_Purchase":
                    case "frmTS1_PurchaseSummary":
                    case "frmTS1_MaterialConsumption":
                    case "frmTS1_ImportIF":
                    case "frmTS1_Holding":
                    case "frmP_Customer":
                    case "frmP_Model":
                    case "frmP_ModelCategory":
                    case "frmP_Production":
                    case "frmS5_FabricOrderSheet":
                    case "frmFS_InsertComment":
                    case "frmS5_ReceiveSummary":
                    case "frmS5_ReceiveByDate":
                    case "frmS5_YarnCode":
                    case "frmS5_PO":
                    case "frmS5_DyeingSchedule":
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
                    case "frmTS1_WorkOrder":
                    case "frmTS1_Purchase":
                    case "frmTS1_PurchaseSummary":
                    case "frmTS1_MaterialConsumption":
                    case "frmTS1_Declare":
                    case "frmTS1_ImportIF":
                    case "frmTS1_Holding":
                    case "frmP_Production":
                    case "frmS5_YarnCheckStock":
                    case "frmS5_ReceiveSummary":
                    case "frmS5_YarnCode":
                    case "frmS5_DyeingSchedule":
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
                    case "frmTS1_Purchase":
                    case "frmTS1_Receive":
                    case "frmTS1_PurchaseSummary":
                    case "frmS5_Receive":
                    case "frmS5_ReceiveByDate":
                    case "frmS5_PO":
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
                    case "frmTS1_Purchase":
                    case "frmTS1_PurchaseSummary":
                    case "frmTS1_Receive":
                    case "frmS5_Receive":
                    case "frmS5_ReceiveByDate":
                    case "frmS5_PO":
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
            LoadSkinIcons();
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
            beiProgress.Visibility = BarItemVisibility.Never;
        }
        private void mdiMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveRegistry();
            this.Dispose();
            Application.Exit();
        }

        #region "Sales1"
        #region "Menu"
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
        private void bbiReceive_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmTS1_Receive();
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
        private void bbiS5_YarnCheckStock_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmS5_YarnCheckStock();
        }
        private void bbiS5_YarnCode_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmS5_YarnCode();
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
        #endregion

        #region "Fabric Stock"
        #region "Menu"
        private void bbiFS_InsertComment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadfrmFSIC_InsertComment();
        }     
        private void bbiFS_ChangePrice_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadfrmFS_ChangePrice();
        }
        #endregion
        #endregion





   







    }
}