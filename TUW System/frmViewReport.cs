using System.Collections.Generic;
using System;
//using ADODB;
using System.Drawing;
using System.Diagnostics;
//using VSFlex8L;
using Microsoft.VisualBasic;
//using AxVSFlex8L;
//using AxCrystal;
using System.Collections;
using System.Windows.Forms;
using CrystalDecisions;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.CrystalReports;
using CrystalDecisions.ReportSource;
using CrystalDecisions.Shared;

namespace TPiCS_Subsystem
{
	public partial class frmViewReport
	{
		public frmViewReport()
		{
			InitializeComponent();
		}
		//Protected crxPrinterName As String 'Crystal Report's Printer name
		//Protected crxPrinterDriver As String   'Crystal Report's Printer driver
		//Protected crxPrinterPort As String 'Crystal Report's Printer port
		//Protected crxPaperSize As Integer  'Crystal Report's Paper size
		//Sub PrintCrystalReport(ByVal crxReport As CRAXDRT.Report, ByVal strReportTitle As String, Optional ByVal ShowDialog As Boolean = True, Optional ByVal PrintOut As Boolean = False)
		//    Dim frmReport As frmViewReport
		
		//    'กำหนดว่าจะให้พิมพ์ออกกระดาษหรือว่าออกฟอร์ม
		//    If PrintOut = True Then
		//        'แสดงหน้าต่างให้เลือก printer ที่ต้องการพิมพ์
		//        If ShowDialog = True Then
		//            frmReport = New frmViewReport
		//            crxReport.PrinterSetup(frmReport.hWnd)
		//            crxPrinterName = crxReport.PrinterName
		//            crxPrinterDriver = crxReport.DriverName
		//            crxPrinterPort = crxReport.PortName
		//            crxPaperSize = crxReport.PaperSize
		//        Else
		//            crxReport.SelectPrinter(crxPrinterDriver, crxPrinterName, crxPrinterPort)
		//            crxReport.PaperSize = crxPaperSize
		//        End If
		//        crxReport.PrintOut(False)
		//    Else
		//        'แสดงหน้าต่างให้เลือก printer ที่ต้องการพิมพ์
		//        If ShowDialog = True Then
		//            frmReport = New frmViewReport
		//            crxReport.PrinterSetup(frmReport.hWnd)
		//            crxPrinterName = crxReport.PrinterName
		//            crxPrinterDriver = crxReport.DriverName
		//            crxPrinterPort = crxReport.PortName
		//            crxPaperSize = crxReport.PaperSize
		//        Else
		//            frmReport = New frmViewReport
		//            crxReport.SelectPrinter(crxPrinterDriver, crxPrinterName, crxPrinterPort)
		//            crxReport.PaperSize = crxPaperSize
		//        End If
		//        frmReport.CRViewer91.ReportSource = crxReport
		//        frmReport.CRViewer91.ViewReport()
		//        frmReport.Caption = strReportTitle
		//        frmReport.Show()
		//    End If
		//End Sub
		public void PreviewCrystalReport(ReportDocument oReport, string strReportTitle)
		{
			//CRV.ParameterFieldInfo = pFields
			CRV.ReportSource = oReport;
			this.Text = strReportTitle;
		}
		
		public void frmPrinter_Shown(System.Object sender, System.EventArgs e)
		{
			//'Prepare Database Login Info
			//Dim LogInfo As New TableLogOnInfo()
			//LogInfo.ConnectionInfo.ServerName = "mnlho8ap33"
			//LogInfo.ConnectionInfo.DatabaseName = "BS"
			//LogInfo.ConnectionInfo.UserID = "sa"
			//LogInfo.ConnectionInfo.Password = "12345678"
			//Dim oReport As New ReportDocument
			//oReport.Load(Application.StartupPath & "\KnittingWO.rpt")
			//Connect to database
			//SetConnectionInfo(oReport, LogInfo)
			//Set Parameters
			//oReport.SetParameterValue("Parameter1", _Parameter1)
			//Open preview
			//Me.WindowState = FormWindowState.Maximized
			//CRV.ReportSource = oReport
		}
		
	}
}
