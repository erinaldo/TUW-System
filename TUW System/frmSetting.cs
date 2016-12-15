using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Skins;
using Microsoft.Win32;

namespace TUW_System
{
    public partial class frmSetting : DevExpress.XtraEditors.XtraForm
    {
        public delegate void SkinHandler(string skinName);
        public event SkinHandler SkinEvent;

        public frmSetting()
        {
            InitializeComponent();
        }
        private void LoadRegistry()
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\TUW\TUW System");
                if (regKey != null)
                {
                    object keyValue ;
                    keyValue = regKey.GetValue("Skin");
                    cboSkin.SelectedIndex = cboSkin.Properties.Items.IndexOf(keyValue);
                    keyValue = regKey.GetValue("YS_Receive - Barcode Printer");
                    txtBarcodePrinter.Text = (keyValue != null) ? regKey.GetValue("YS_Receive - Barcode Printer").ToString() : "";
                    keyValue = regKey.GetValue("YS_Receive - Print Copy");
                    spinEdit1.EditValue = (keyValue != null) ? regKey.GetValue("YS_Receive - Print Copy") : 3;

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
                regKey.SetValue(key,value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save to registry error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void GetSkinList()
        {
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("DevExpress Style", SkinIconsSize.Large),"DevExpress Style");
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Caramel", SkinIconsSize.Large), "Caramel");
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Money Twins", SkinIconsSize.Large), "Money Twins");
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Lilian", SkinIconsSize.Large),"Lilian" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("DevExpress Dark Style", SkinIconsSize.Large),"DevExpress Dark Style" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("iMaginary", SkinIconsSize.Large),"iMaginary" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Black", SkinIconsSize.Large),"Black" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Blue", SkinIconsSize.Large),"Blue" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Coffee", SkinIconsSize.Large),"Coffee" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Liquid Sky", SkinIconsSize.Large),"Liquid Sky" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("London Liquid Sky", SkinIconsSize.Large),"London Liquid Sky" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Glass Oceans", SkinIconsSize.Large),"Glass Oceans" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Stardust", SkinIconsSize.Large),"Stardust" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Xmas 2008 Blue", SkinIconsSize.Large),"Xmas 2008 Blue" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Valentine", SkinIconsSize.Large),"Valentine" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("McSkin", SkinIconsSize.Large),"McSkin" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Summer 2008", SkinIconsSize.Large),"Summer 2008" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Pumpkin", SkinIconsSize.Large),"Pumpkin" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Dark Side", SkinIconsSize.Large),"Dark Side" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Springtime", SkinIconsSize.Large), "Springtime");
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Darkroom", SkinIconsSize.Large),"Darkroom" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Foggy", SkinIconsSize.Large),"Foggy" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("High Contrast", SkinIconsSize.Large),"High Contrast" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Seven", SkinIconsSize.Large),"Seven" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Seven Classic", SkinIconsSize.Large),"Seven Classic" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Sharp", SkinIconsSize.Large),"Sharp" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Sharp Plus", SkinIconsSize.Large),"Sharp Plus" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("The Asphalt World", SkinIconsSize.Large), "The Asphalt World");
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Blueprint", SkinIconsSize.Large),"Blueprint" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("Whiteprint", SkinIconsSize.Large),"Whiteprint" );
            imageCollection1.AddImage(SkinCollectionHelper.GetSkinIcon("VS2010", SkinIconsSize.Large), "VS2010");
            for (int i = 0; i < imageCollection1.Images.Count; i++)
            {
                cboSkin.Properties.Items.Add(new ImageComboBoxItem(imageCollection1.Images.Keys[i].ToString() ,i));
            }
            cboSkin.Properties.LargeImages = imageCollection1;
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            GetSkinList();
            LoadRegistry();
        }
        private void btnBarcodePrinter_Click(object sender, EventArgs e)
        {
            PrintDialog dlg = new PrintDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtBarcodePrinter.Text = dlg.PrinterSettings.PrinterName;
                SaveRegistry("YS_Receive - Barcode Printer", txtBarcodePrinter.Text);
            }
        }
        private void cboSkin_SelectedIndexChanged(object sender, EventArgs e)
        {
            SkinEvent(cboSkin.Text);
            SaveRegistry("Skin", cboSkin.Text);
        }
        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            SaveRegistry("YS_Receive - Print Copy", spinEdit1.EditValue);
        }


    }
}