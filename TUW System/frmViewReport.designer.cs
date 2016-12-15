using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;

namespace TPiCS_Subsystem
{
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]public partial class frmViewReport : System.Windows.Forms.Form
		{
		
		//Form overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]protected override void Dispose(bool disposing)
			{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		
		//Required by the Windows Form Designer
		private System.ComponentModel.Container components = null;
		
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
			{
			this.CRV = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
			base.Shown += new System.EventHandler(frmPrinter_Shown);
			this.SuspendLayout();
			//
			//CRV
			//
			this.CRV.ActiveViewIndex = -1;
			this.CRV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.CRV.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CRV.Location = new System.Drawing.Point(0, 0);
			this.CRV.Name = "CRV";
			this.CRV.SelectionFormula = "";
			this.CRV.Size = new System.Drawing.Size(590, 412);
			this.CRV.TabIndex = 0;
			this.CRV.ViewTimeSelectionFormula = "";
			//
			//frmViewReport
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6.0F, 13.0F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(590, 412);
			this.Controls.Add(this.CRV);
			this.Name = "frmViewReport";
			this.Text = "frmPrinter";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.ResumeLayout(false);
			
		}
		internal CrystalDecisions.Windows.Forms.CrystalReportViewer CRV;
	}
	
}
