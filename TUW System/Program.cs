using System;
using System.Windows.Forms;

namespace TUW_System
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Skin registration
            DevExpress.UserSkins.BonusSkins.Register();
            //Enable title bar skinning 
            DevExpress.Skins.SkinManager.EnableFormSkins();
            //DevExpress.Utils.AppearanceObject.DefaultFont = new System.Drawing.Font("Tahoma", 9);
            DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont = new System.Drawing.Font("Tahoma", 9);
            Application.Run(new frmLogin());
        }
    }

}
