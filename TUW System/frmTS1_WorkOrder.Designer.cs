namespace TUW_System
{
    partial class frmTS1_WorkOrder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTS1_WorkOrder));
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.dockPanel1 = new DevExpress.XtraBars.Docking.DockPanel();
            this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
            this.txtContract = new DevExpress.XtraEditors.TextEdit();
            this.txtCode = new DevExpress.XtraEditors.TextEdit();
            this.chkContract = new DevExpress.XtraEditors.CheckEdit();
            this.chkCode = new DevExpress.XtraEditors.CheckEdit();
            this.chkPO = new DevExpress.XtraEditors.CheckEdit();
            this.chkIssue = new DevExpress.XtraEditors.CheckEdit();
            this.txtPO2 = new DevExpress.XtraEditors.TextEdit();
            this.txtPO1 = new DevExpress.XtraEditors.TextEdit();
            this.dtpIssue = new DevExpress.XtraEditors.DateEdit();
            this.chkRePrint = new DevExpress.XtraEditors.CheckEdit();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.chkSelect = new DevExpress.XtraEditors.CheckEdit();
            this.Grid = new DevExpress.XtraGrid.GridControl();
            this.GridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.dockPanel1.SuspendLayout();
            this.dockPanel1_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtContract.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkContract.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPO.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIssue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPO2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPO1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssue.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRePrint.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkSelect.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).BeginInit();
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
            this.dockPanel1.ID = new System.Guid("7081fee9-7e5b-4f36-bbea-86ff08491ce3");
            this.dockPanel1.Location = new System.Drawing.Point(0, 0);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.OriginalSize = new System.Drawing.Size(200, 87);
            this.dockPanel1.Size = new System.Drawing.Size(792, 87);
            this.dockPanel1.Text = "Search :";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.txtContract);
            this.dockPanel1_Container.Controls.Add(this.txtCode);
            this.dockPanel1_Container.Controls.Add(this.chkContract);
            this.dockPanel1_Container.Controls.Add(this.chkCode);
            this.dockPanel1_Container.Controls.Add(this.chkPO);
            this.dockPanel1_Container.Controls.Add(this.chkIssue);
            this.dockPanel1_Container.Controls.Add(this.txtPO2);
            this.dockPanel1_Container.Controls.Add(this.txtPO1);
            this.dockPanel1_Container.Controls.Add(this.dtpIssue);
            this.dockPanel1_Container.Controls.Add(this.chkRePrint);
            this.dockPanel1_Container.Location = new System.Drawing.Point(3, 25);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(786, 59);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // txtContract
            // 
            this.txtContract.EditValue = "";
            this.txtContract.Enabled = false;
            this.txtContract.Location = new System.Drawing.Point(455, 7);
            this.txtContract.Name = "txtContract";
            this.txtContract.Size = new System.Drawing.Size(174, 20);
            this.txtContract.TabIndex = 42;
            // 
            // txtCode
            // 
            this.txtCode.EditValue = "";
            this.txtCode.Enabled = false;
            this.txtCode.Location = new System.Drawing.Point(178, 32);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(174, 20);
            this.txtCode.TabIndex = 41;
            // 
            // chkContract
            // 
            this.chkContract.Location = new System.Drawing.Point(367, 8);
            this.chkContract.Name = "chkContract";
            this.chkContract.Properties.Caption = "Contract :";
            this.chkContract.Size = new System.Drawing.Size(75, 19);
            this.chkContract.TabIndex = 40;
            this.chkContract.CheckedChanged += new System.EventHandler(this.chkContract_CheckedChanged);
            // 
            // chkCode
            // 
            this.chkCode.Location = new System.Drawing.Point(90, 32);
            this.chkCode.Name = "chkCode";
            this.chkCode.Properties.Caption = "Code :";
            this.chkCode.Size = new System.Drawing.Size(55, 19);
            this.chkCode.TabIndex = 39;
            this.chkCode.CheckedChanged += new System.EventHandler(this.chkCode_CheckedChanged);
            // 
            // chkPO
            // 
            this.chkPO.Location = new System.Drawing.Point(90, 7);
            this.chkPO.Name = "chkPO";
            this.chkPO.Properties.Caption = "PONUM :";
            this.chkPO.Size = new System.Drawing.Size(41, 19);
            this.chkPO.TabIndex = 37;
            this.chkPO.CheckedChanged += new System.EventHandler(this.chkPO_CheckedChanged);
            // 
            // chkIssue
            // 
            this.chkIssue.Location = new System.Drawing.Point(367, 32);
            this.chkIssue.Name = "chkIssue";
            this.chkIssue.Properties.Caption = "Issue Date :";
            this.chkIssue.Size = new System.Drawing.Size(82, 19);
            this.chkIssue.TabIndex = 36;
            this.chkIssue.CheckedChanged += new System.EventHandler(this.chkIssue_CheckedChanged);
            // 
            // txtPO2
            // 
            this.txtPO2.EditValue = "";
            this.txtPO2.Enabled = false;
            this.txtPO2.Location = new System.Drawing.Point(268, 7);
            this.txtPO2.Name = "txtPO2";
            this.txtPO2.Size = new System.Drawing.Size(84, 20);
            this.txtPO2.TabIndex = 35;
            // 
            // txtPO1
            // 
            this.txtPO1.Enabled = false;
            this.txtPO1.Location = new System.Drawing.Point(178, 7);
            this.txtPO1.Name = "txtPO1";
            this.txtPO1.Size = new System.Drawing.Size(84, 20);
            this.txtPO1.TabIndex = 34;
            // 
            // dtpIssue
            // 
            this.dtpIssue.EditValue = null;
            this.dtpIssue.Location = new System.Drawing.Point(455, 32);
            this.dtpIssue.Name = "dtpIssue";
            this.dtpIssue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtpIssue.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtpIssue.Size = new System.Drawing.Size(174, 20);
            this.dtpIssue.TabIndex = 33;
            // 
            // chkRePrint
            // 
            this.chkRePrint.Location = new System.Drawing.Point(9, 7);
            this.chkRePrint.Name = "chkRePrint";
            this.chkRePrint.Properties.Caption = "Re-Print";
            this.chkRePrint.Size = new System.Drawing.Size(75, 19);
            this.chkRePrint.TabIndex = 29;
            this.chkRePrint.ToolTip = "คลิกเครื่องหมายถูกเมื่อต้องการพิมพ์ข้อมูลจาก Order Balance";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.chkSelect);
            this.panelControl1.Controls.Add(this.Grid);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 87);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.panelControl1.Size = new System.Drawing.Size(792, 479);
            this.panelControl1.TabIndex = 1;
            // 
            // chkSelect
            // 
            this.chkSelect.Location = new System.Drawing.Point(12, 12);
            this.chkSelect.Name = "chkSelect";
            this.chkSelect.Properties.Caption = "Select All";
            this.chkSelect.Size = new System.Drawing.Size(75, 19);
            this.chkSelect.TabIndex = 2;
            this.chkSelect.CheckedChanged += new System.EventHandler(this.chkSelect_CheckedChanged);
            // 
            // Grid
            // 
            this.Grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Grid.Location = new System.Drawing.Point(2, 37);
            this.Grid.MainView = this.GridView;
            this.Grid.Name = "Grid";
            this.Grid.Size = new System.Drawing.Size(788, 440);
            this.Grid.TabIndex = 0;
            this.Grid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.GridView});
            // 
            // GridView
            // 
            this.GridView.GridControl = this.Grid;
            this.GridView.Name = "GridView";
            this.GridView.OptionsView.ShowAutoFilterRow = true;
            this.GridView.OptionsView.ShowGroupPanel = false;
            this.GridView.ColumnFilterChanged += new System.EventHandler(this.GridView_ColumnFilterChanged);
            // 
            // frmTS1_WorkOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.dockPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmTS1_WorkOrder";
            this.Text = "Work Order...";
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockPanel1.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtContract.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkContract.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPO.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIssue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPO2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPO1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssue.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRePrint.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkSelect.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private DevExpress.XtraBars.Docking.DockPanel dockPanel1;
        private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
        internal DevExpress.XtraEditors.TextEdit txtContract;
        internal DevExpress.XtraEditors.TextEdit txtCode;
        internal DevExpress.XtraEditors.CheckEdit chkContract;
        internal DevExpress.XtraEditors.CheckEdit chkCode;
        internal DevExpress.XtraEditors.CheckEdit chkPO;
        internal DevExpress.XtraEditors.CheckEdit chkIssue;
        internal DevExpress.XtraEditors.TextEdit txtPO2;
        internal DevExpress.XtraEditors.TextEdit txtPO1;
        internal DevExpress.XtraEditors.DateEdit dtpIssue;
        internal DevExpress.XtraEditors.CheckEdit chkRePrint;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl Grid;
        private DevExpress.XtraGrid.Views.Grid.GridView GridView;
        internal DevExpress.XtraEditors.CheckEdit chkSelect;
    }
}