namespace TUW_System
{
    partial class frmFS_InsertComment
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFS_InsertComment));
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.txtBarcode = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtColor = new DevExpress.XtraEditors.TextEdit();
            this.chkColor = new DevExpress.XtraEditors.CheckEdit();
            this.txtFabricCode = new DevExpress.XtraEditors.TextEdit();
            this.chkFabricCode = new DevExpress.XtraEditors.CheckEdit();
            this.chkLotNo = new DevExpress.XtraEditors.CheckEdit();
            this.txtLotNo = new DevExpress.XtraEditors.TextEdit();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBarcode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtColor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkColor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFabricCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFabricCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkLotNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLotNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dockManager1
            // 
            this.dockManager1.Form = this;
            this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockPanel1});
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"});
            // 
            // dockPanel1
            // 
            this.dockPanel1.Controls.Add(this.dockPanel1_Container);
            this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Top;
            this.dockPanel1.ID = new System.Guid("b574acf2-5a84-4f3d-a54f-238b089cecf7");
            this.dockPanel1.Location = new System.Drawing.Point(0, 0);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Options.ShowCloseButton = false;
            this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 76);
            this.dockPanel1.Size = new System.Drawing.Size(1012, 76);
            this.dockPanel1.Text = "Search from :";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.txtBarcode);
            this.dockPanel1_Container.Controls.Add(this.labelControl1);
            this.dockPanel1_Container.Controls.Add(this.txtColor);
            this.dockPanel1_Container.Controls.Add(this.chkColor);
            this.dockPanel1_Container.Controls.Add(this.txtFabricCode);
            this.dockPanel1_Container.Controls.Add(this.chkFabricCode);
            this.dockPanel1_Container.Controls.Add(this.chkLotNo);
            this.dockPanel1_Container.Controls.Add(this.txtLotNo);
            this.dockPanel1_Container.Location = new System.Drawing.Point(3, 25);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(1006, 48);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // txtBarcode
            // 
            this.txtBarcode.Location = new System.Drawing.Point(670, 14);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(129, 20);
            this.txtBarcode.TabIndex = 7;
            this.txtBarcode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBarcode_KeyPress);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(618, 17);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(46, 13);
            this.labelControl1.TabIndex = 7;
            this.labelControl1.Text = "Barcode :";
            // 
            // txtColor
            // 
            this.txtColor.Location = new System.Drawing.Point(471, 14);
            this.txtColor.Name = "txtColor";
            this.txtColor.Size = new System.Drawing.Size(129, 20);
            this.txtColor.TabIndex = 6;
            this.txtColor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtColor_KeyPress);
            // 
            // chkColor
            // 
            this.chkColor.Location = new System.Drawing.Point(410, 14);
            this.chkColor.Name = "chkColor";
            this.chkColor.Properties.Caption = "Color :";
            this.chkColor.Size = new System.Drawing.Size(55, 19);
            this.chkColor.TabIndex = 5;
            // 
            // txtFabricCode
            // 
            this.txtFabricCode.Location = new System.Drawing.Point(275, 14);
            this.txtFabricCode.Name = "txtFabricCode";
            this.txtFabricCode.Size = new System.Drawing.Size(129, 20);
            this.txtFabricCode.TabIndex = 4;
            this.txtFabricCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFabricCode_KeyPress);
            // 
            // chkFabricCode
            // 
            this.chkFabricCode.Location = new System.Drawing.Point(215, 14);
            this.chkFabricCode.Name = "chkFabricCode";
            this.chkFabricCode.Properties.Caption = "Code :";
            this.chkFabricCode.Size = new System.Drawing.Size(54, 19);
            this.chkFabricCode.TabIndex = 3;
            // 
            // chkLotNo
            // 
            this.chkLotNo.Location = new System.Drawing.Point(9, 14);
            this.chkLotNo.Name = "chkLotNo";
            this.chkLotNo.Properties.Caption = "Lot No. :";
            this.chkLotNo.Size = new System.Drawing.Size(65, 19);
            this.chkLotNo.TabIndex = 2;
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(80, 14);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.Size = new System.Drawing.Size(129, 20);
            this.txtLotNo.TabIndex = 1;
            this.txtLotNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLotNo_KeyPress);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.gridControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 76);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(0, 30, 0, 0);
            this.panelControl1.Size = new System.Drawing.Size(1012, 490);
            this.panelControl1.TabIndex = 1;
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(2, 32);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1008, 456);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
            this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
            this.gridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridView1_KeyDown);
            // 
            // frmFS_InsertComment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 566);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.dockPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFS_InsertComment";
            this.Text = "frmFSIC_InsertComment";
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockPanel1.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            this.dockPanel1_Container.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBarcode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtColor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkColor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFabricCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFabricCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkLotNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLotNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        private DevExpress.XtraEditors.TextEdit txtBarcode;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtColor;
        private DevExpress.XtraEditors.CheckEdit chkColor;
        private DevExpress.XtraEditors.TextEdit txtFabricCode;
        private DevExpress.XtraEditors.CheckEdit chkFabricCode;
        private DevExpress.XtraEditors.CheckEdit chkLotNo;
        private DevExpress.XtraEditors.TextEdit txtLotNo;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
    }
}