namespace TUW_System.TS1
{
    partial class frmTS1_PurchaseSummary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTS1_PurchaseSummary));
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chkSelect = new DevExpress.XtraEditors.CheckEdit();
            this.chkFreeze = new DevExpress.XtraEditors.CheckEdit();
            this.chkReprint = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSelect.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFreeze.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkReprint.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 35);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(846, 531);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowAutoFilterRow = true;
            // 
            // chkSelect
            // 
            this.chkSelect.Location = new System.Drawing.Point(3, 10);
            this.chkSelect.Name = "chkSelect";
            this.chkSelect.Properties.Caption = "Select All";
            this.chkSelect.Size = new System.Drawing.Size(75, 19);
            this.chkSelect.TabIndex = 1;
            this.chkSelect.CheckedChanged += new System.EventHandler(this.chkSelect_CheckedChanged);
            // 
            // chkFreeze
            // 
            this.chkFreeze.Location = new System.Drawing.Point(84, 10);
            this.chkFreeze.Name = "chkFreeze";
            this.chkFreeze.Properties.Caption = "Freeze Pans";
            this.chkFreeze.Size = new System.Drawing.Size(84, 19);
            this.chkFreeze.TabIndex = 2;
            this.chkFreeze.CheckedChanged += new System.EventHandler(this.chkFreeze_CheckedChanged);
            // 
            // chkReprint
            // 
            this.chkReprint.Location = new System.Drawing.Point(186, 10);
            this.chkReprint.Name = "chkReprint";
            this.chkReprint.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.chkReprint.Properties.Appearance.Options.UseFont = true;
            this.chkReprint.Properties.Caption = "Reprint";
            this.chkReprint.Size = new System.Drawing.Size(75, 19);
            this.chkReprint.TabIndex = 3;
            // 
            // frmPurchase_Summary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 566);
            this.Controls.Add(this.chkReprint);
            this.Controls.Add(this.chkFreeze);
            this.Controls.Add(this.chkSelect);
            this.Controls.Add(this.gridControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmTS1_PurchaseSummary";
            this.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.Text = "Summary Purchase Order Report...";
            this.Load += new System.EventHandler(this.frmPurchase_Summary_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSelect.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFreeze.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkReprint.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.CheckEdit chkSelect;
        private DevExpress.XtraEditors.CheckEdit chkFreeze;
        private DevExpress.XtraEditors.CheckEdit chkReprint;
    }
}