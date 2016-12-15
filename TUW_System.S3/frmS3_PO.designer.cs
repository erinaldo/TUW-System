namespace TUW_System.S3
{
    partial class frmS3_PO
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
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.chkRePrint = new DevExpress.XtraEditors.CheckEdit();
            this.lblDate = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtSupplier1 = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtSupplier2 = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtPO1 = new DevExpress.XtraEditors.TextEdit();
            this.txtPO2 = new DevExpress.XtraEditors.TextEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.speRow = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.speCopy = new DevExpress.XtraEditors.SpinEdit();
            ((System.ComponentModel.ISupportInitialize)(this.chkRePrint.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSupplier1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSupplier2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPO1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPO2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speRow.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speCopy.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Times New Roman", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Location = new System.Drawing.Point(154, 12);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(187, 31);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Purchase Order";
            // 
            // chkRePrint
            // 
            this.chkRePrint.Location = new System.Drawing.Point(12, 78);
            this.chkRePrint.Name = "chkRePrint";
            this.chkRePrint.Properties.Caption = "Re-Print";
            this.chkRePrint.Size = new System.Drawing.Size(75, 19);
            this.chkRePrint.TabIndex = 2;
            // 
            // lblDate
            // 
            this.lblDate.Location = new System.Drawing.Point(419, 81);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(33, 13);
            this.lblDate.TabIndex = 3;
            this.lblDate.Text = "lblDate";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(90, 130);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(45, 13);
            this.labelControl2.TabIndex = 39;
            this.labelControl2.Text = "Supplier :";
            // 
            // txtSupplier1
            // 
            this.txtSupplier1.Location = new System.Drawing.Point(141, 127);
            this.txtSupplier1.Name = "txtSupplier1";
            this.txtSupplier1.Size = new System.Drawing.Size(100, 20);
            this.txtSupplier1.TabIndex = 40;
            this.txtSupplier1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSupplier1_KeyPress);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(247, 130);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(10, 13);
            this.labelControl3.TabIndex = 41;
            this.labelControl3.Text = "to";
            // 
            // txtSupplier2
            // 
            this.txtSupplier2.Location = new System.Drawing.Point(263, 127);
            this.txtSupplier2.Name = "txtSupplier2";
            this.txtSupplier2.Size = new System.Drawing.Size(100, 20);
            this.txtSupplier2.TabIndex = 42;
            this.txtSupplier2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSupplier2_KeyPress);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(90, 163);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(25, 13);
            this.labelControl4.TabIndex = 43;
            this.labelControl4.Text = "P/O :";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(247, 163);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(10, 13);
            this.labelControl5.TabIndex = 44;
            this.labelControl5.Text = "to";
            // 
            // txtPO1
            // 
            this.txtPO1.Location = new System.Drawing.Point(141, 160);
            this.txtPO1.Name = "txtPO1";
            this.txtPO1.Size = new System.Drawing.Size(100, 20);
            this.txtPO1.TabIndex = 45;
            this.txtPO1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPO1_KeyPress);
            // 
            // txtPO2
            // 
            this.txtPO2.Location = new System.Drawing.Point(263, 160);
            this.txtPO2.Name = "txtPO2";
            this.txtPO2.Size = new System.Drawing.Size(100, 20);
            this.txtPO2.TabIndex = 46;
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(106, 219);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(151, 13);
            this.labelControl6.TabIndex = 47;
            this.labelControl6.Text = "The Number of record per page";
            // 
            // speRow
            // 
            this.speRow.EditValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.speRow.Location = new System.Drawing.Point(263, 216);
            this.speRow.Name = "speRow";
            this.speRow.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.speRow.Size = new System.Drawing.Size(100, 20);
            this.speRow.TabIndex = 48;
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(118, 265);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(139, 13);
            this.labelControl7.TabIndex = 49;
            this.labelControl7.Text = "The Number of sheet of print";
            // 
            // speCopy
            // 
            this.speCopy.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.speCopy.Location = new System.Drawing.Point(263, 262);
            this.speCopy.Name = "speCopy";
            this.speCopy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.speCopy.Size = new System.Drawing.Size(100, 20);
            this.speCopy.TabIndex = 50;
            // 
            // frmS3_PO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 442);
            this.Controls.Add(this.speCopy);
            this.Controls.Add(this.labelControl7);
            this.Controls.Add(this.speRow);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.txtPO2);
            this.Controls.Add(this.txtPO1);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.txtSupplier2);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.txtSupplier1);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.chkRePrint);
            this.Controls.Add(this.labelControl1);
            this.Name = "frmS3_PO";
            this.Text = "frmS3_Purchase";
            this.Load += new System.EventHandler(this.frmS3_PO_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chkRePrint.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSupplier1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSupplier2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPO1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPO2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speRow.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speCopy.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckEdit chkRePrint;
        private DevExpress.XtraEditors.LabelControl lblDate;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtSupplier1;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtSupplier2;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.TextEdit txtPO1;
        private DevExpress.XtraEditors.TextEdit txtPO2;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.SpinEdit speRow;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.SpinEdit speCopy;
    }
}