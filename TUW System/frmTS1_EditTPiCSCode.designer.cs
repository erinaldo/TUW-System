namespace TUW_System
{
    partial class frmTS1_EditTPiCSCode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTS1_EditTPiCSCode));
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.cmdSearch = new DevExpress.XtraEditors.SimpleButton();
            this.LabelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.chkTPiCSOrder = new DevExpress.XtraEditors.CheckEdit();
            this.cboSysdelete = new DevExpress.XtraEditors.ComboBoxEdit();
            this.txtTPiCSOrder = new DevExpress.XtraEditors.TextEdit();
            this.chkTPiCSCode = new DevExpress.XtraEditors.CheckEdit();
            this.chkColor = new DevExpress.XtraEditors.CheckEdit();
            this.txtTPiCSCode = new DevExpress.XtraEditors.TextEdit();
            this.chkLot = new DevExpress.XtraEditors.CheckEdit();
            this.txtCode = new DevExpress.XtraEditors.TextEdit();
            this.chkCode = new DevExpress.XtraEditors.CheckEdit();
            this.txtLot = new DevExpress.XtraEditors.TextEdit();
            this.txtColor = new DevExpress.XtraEditors.TextEdit();
            this.Grid1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkTPiCSOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSysdelete.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTPiCSOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTPiCSCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkColor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTPiCSCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkLot.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLot.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtColor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Grid1)).BeginInit();
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
            this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
            this.dockPanel1.ID = new System.Guid("f9a2f1f4-b077-494d-993f-4d55df9f3d38");
            this.dockPanel1.Location = new System.Drawing.Point(0, 0);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 200);
            this.dockPanel1.Size = new System.Drawing.Size(200, 566);
            this.dockPanel1.Text = "Search...";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.cmdSearch);
            this.dockPanel1_Container.Controls.Add(this.LabelControl1);
            this.dockPanel1_Container.Controls.Add(this.chkTPiCSOrder);
            this.dockPanel1_Container.Controls.Add(this.cboSysdelete);
            this.dockPanel1_Container.Controls.Add(this.txtTPiCSOrder);
            this.dockPanel1_Container.Controls.Add(this.chkTPiCSCode);
            this.dockPanel1_Container.Controls.Add(this.chkColor);
            this.dockPanel1_Container.Controls.Add(this.txtTPiCSCode);
            this.dockPanel1_Container.Controls.Add(this.chkLot);
            this.dockPanel1_Container.Controls.Add(this.txtCode);
            this.dockPanel1_Container.Controls.Add(this.chkCode);
            this.dockPanel1_Container.Controls.Add(this.txtLot);
            this.dockPanel1_Container.Controls.Add(this.txtColor);
            this.dockPanel1_Container.Location = new System.Drawing.Point(3, 25);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(194, 538);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // cmdSearch
            // 
            this.cmdSearch.Image = ((System.Drawing.Image)(resources.GetObject("cmdSearch.Image")));
            this.cmdSearch.Location = new System.Drawing.Point(45, 352);
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.Size = new System.Drawing.Size(87, 23);
            this.cmdSearch.TabIndex = 13;
            this.cmdSearch.Text = "&Search";
            this.cmdSearch.Click += new System.EventHandler(this.cmdSearch_Click);
            // 
            // LabelControl1
            // 
            this.LabelControl1.Location = new System.Drawing.Point(11, 291);
            this.LabelControl1.Name = "LabelControl1";
            this.LabelControl1.Size = new System.Drawing.Size(63, 13);
            this.LabelControl1.TabIndex = 23;
            this.LabelControl1.Text = "Fabric Status";
            // 
            // chkTPiCSOrder
            // 
            this.chkTPiCSOrder.Location = new System.Drawing.Point(9, 18);
            this.chkTPiCSOrder.Name = "chkTPiCSOrder";
            this.chkTPiCSOrder.Properties.Caption = "TPiCS Order:";
            this.chkTPiCSOrder.Size = new System.Drawing.Size(97, 19);
            this.chkTPiCSOrder.TabIndex = 11;
            this.chkTPiCSOrder.CheckedChanged += new System.EventHandler(this.chkTPiCSOrder_CheckedChanged);
            // 
            // cboSysdelete
            // 
            this.cboSysdelete.Location = new System.Drawing.Point(80, 288);
            this.cboSysdelete.Name = "cboSysdelete";
            this.cboSysdelete.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboSysdelete.Properties.Items.AddRange(new object[] {
            "All",
            "In Stock",
            "Out Stock"});
            this.cboSysdelete.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboSysdelete.Size = new System.Drawing.Size(100, 20);
            this.cboSysdelete.TabIndex = 22;
            // 
            // txtTPiCSOrder
            // 
            this.txtTPiCSOrder.Enabled = false;
            this.txtTPiCSOrder.Location = new System.Drawing.Point(11, 43);
            this.txtTPiCSOrder.Name = "txtTPiCSOrder";
            this.txtTPiCSOrder.Size = new System.Drawing.Size(169, 20);
            this.txtTPiCSOrder.TabIndex = 12;
            // 
            // chkTPiCSCode
            // 
            this.chkTPiCSCode.Location = new System.Drawing.Point(9, 69);
            this.chkTPiCSCode.Name = "chkTPiCSCode";
            this.chkTPiCSCode.Properties.Caption = "TPiCS Code:";
            this.chkTPiCSCode.Size = new System.Drawing.Size(97, 19);
            this.chkTPiCSCode.TabIndex = 14;
            this.chkTPiCSCode.CheckedChanged += new System.EventHandler(this.chkTPiCSCode_CheckedChanged);
            // 
            // chkColor
            // 
            this.chkColor.Location = new System.Drawing.Point(9, 222);
            this.chkColor.Name = "chkColor";
            this.chkColor.Properties.Caption = "Color:";
            this.chkColor.Size = new System.Drawing.Size(97, 19);
            this.chkColor.TabIndex = 21;
            this.chkColor.CheckedChanged += new System.EventHandler(this.chkColor_CheckedChanged);
            // 
            // txtTPiCSCode
            // 
            this.txtTPiCSCode.Enabled = false;
            this.txtTPiCSCode.Location = new System.Drawing.Point(11, 94);
            this.txtTPiCSCode.Name = "txtTPiCSCode";
            this.txtTPiCSCode.Size = new System.Drawing.Size(169, 20);
            this.txtTPiCSCode.TabIndex = 15;
            // 
            // chkLot
            // 
            this.chkLot.Location = new System.Drawing.Point(9, 171);
            this.chkLot.Name = "chkLot";
            this.chkLot.Properties.Caption = "Lot No:";
            this.chkLot.Size = new System.Drawing.Size(97, 19);
            this.chkLot.TabIndex = 20;
            this.chkLot.CheckedChanged += new System.EventHandler(this.chkLot_CheckedChanged);
            // 
            // txtCode
            // 
            this.txtCode.Enabled = false;
            this.txtCode.Location = new System.Drawing.Point(11, 145);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(169, 20);
            this.txtCode.TabIndex = 16;
            // 
            // chkCode
            // 
            this.chkCode.Location = new System.Drawing.Point(9, 120);
            this.chkCode.Name = "chkCode";
            this.chkCode.Properties.Caption = "Fabric Code:";
            this.chkCode.Size = new System.Drawing.Size(97, 19);
            this.chkCode.TabIndex = 19;
            this.chkCode.CheckedChanged += new System.EventHandler(this.chkCode_CheckedChanged);
            // 
            // txtLot
            // 
            this.txtLot.Enabled = false;
            this.txtLot.Location = new System.Drawing.Point(11, 196);
            this.txtLot.Name = "txtLot";
            this.txtLot.Size = new System.Drawing.Size(169, 20);
            this.txtLot.TabIndex = 17;
            // 
            // txtColor
            // 
            this.txtColor.Enabled = false;
            this.txtColor.Location = new System.Drawing.Point(11, 247);
            this.txtColor.Name = "txtColor";
            this.txtColor.Size = new System.Drawing.Size(169, 20);
            this.txtColor.TabIndex = 18;
            // 
            // Grid1
            // 
            this.Grid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grid1.Location = new System.Drawing.Point(200, 0);
            this.Grid1.MainView = this.gridView1;
            this.Grid1.Name = "Grid1";
            this.Grid1.Size = new System.Drawing.Size(592, 566);
            this.Grid1.TabIndex = 1;
            this.Grid1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.Grid1;
            this.gridView1.Name = "gridView1";
            this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
            this.gridView1.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gridView1_ValidatingEditor);
            // 
            // frmTS1_EditTPiCSCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Controls.Add(this.Grid1);
            this.Controls.Add(this.dockPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmTS1_EditTPiCSCode";
            this.Text = "EditTPiCSCode";
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockPanel1.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            this.dockPanel1_Container.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkTPiCSOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSysdelete.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTPiCSOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkTPiCSCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkColor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTPiCSCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkLot.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLot.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtColor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Grid1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        internal DevExpress.XtraEditors.SimpleButton cmdSearch;
        internal DevExpress.XtraEditors.LabelControl LabelControl1;
        internal DevExpress.XtraEditors.CheckEdit chkTPiCSOrder;
        internal DevExpress.XtraEditors.ComboBoxEdit cboSysdelete;
        internal DevExpress.XtraEditors.TextEdit txtTPiCSOrder;
        internal DevExpress.XtraEditors.CheckEdit chkTPiCSCode;
        internal DevExpress.XtraEditors.CheckEdit chkColor;
        internal DevExpress.XtraEditors.TextEdit txtTPiCSCode;
        internal DevExpress.XtraEditors.CheckEdit chkLot;
        internal DevExpress.XtraEditors.TextEdit txtCode;
        internal DevExpress.XtraEditors.CheckEdit chkCode;
        internal DevExpress.XtraEditors.TextEdit txtLot;
        internal DevExpress.XtraEditors.TextEdit txtColor;
        private DevExpress.XtraGrid.GridControl Grid1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;

    }
}