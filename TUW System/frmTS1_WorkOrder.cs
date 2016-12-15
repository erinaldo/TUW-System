using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Globalization;
using myClass;

namespace TUW_System
{
    public partial class frmTS1_WorkOrder : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db = new cDatabase(Module.Sewing);
		private CultureInfo clinfo = new CultureInfo("en-US");
        private DateTimeFormatInfo dtfinfo;
        cCrystalReport ctr;

        private DevExpress.XtraBars.BarStaticItem _bsiStatusBar;
        public DevExpress.XtraBars.BarStaticItem bsiStatusbar
        {
            get { return _bsiStatusBar; }
            set { _bsiStatusBar = value; }
        }

        public frmTS1_WorkOrder()
        {
            InitializeComponent();
        }
		public void LoadData()
		{
            try
            {
                this.Cursor = Cursors.WaitCursor;
                string strSQL = "EXEC spTPiCSFDI_WorkOrder ";
                if (chkPO.Checked)
                {
                    strSQL += "\'" + txtPO1.Text + "\',\'" + txtPO2.Text + "\'";
                }
                else
                {
                    strSQL += "\'\',\'\'";
                }
                if (chkCode.Checked)
                {
                    strSQL += ",\'" + txtCode.Text + "\'";
                }
                else
                {
                    strSQL += ",\'\'";
                }
                if (chkContract.Checked)
                {
                    strSQL += ",\'" + txtContract.Text + "\'";
                }
                else
                {
                    strSQL += ",\'\'";
                }
                if (chkIssue.Checked)
                {
                    string strTemp = (System.Convert.ToDateTime(dtpIssue.EditValue)).ToString("yyyyMMdd", clinfo);
                    strSQL += ",\'" + strTemp + "\'";
                }
                else
                {
                    strSQL += ",\'\'";
                }
                if (chkRePrint.Checked)
                {
                    strSQL += ",\'Y\'";
                }
                else
                {
                    strSQL += ",\'N\'";
                }
                DataTable dt = db.GetDataTable(strSQL);
                DataColumn col = new DataColumn();
                col.ColumnName = "PRINT";
                col.DataType = typeof(bool);
                col.ReadOnly = false;
                col.DefaultValue = false;
                dt.Columns.Add(col);
                Grid.DataSource = dt;
                GridView.Columns["PRINT"].VisibleIndex = 0;
                GridView.OptionsView.EnableAppearanceOddRow = true;
                GridView.OptionsView.EnableAppearanceEvenRow = true;
                GridView.OptionsView.ColumnAutoWidth = false;
                GridView.BestFitColumns();
                this.bsiStatusbar.Caption=GridView.RowCount.ToString() + " Rows.";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
		}
        public void Print()
        {
            GridView.CloseEditor();
            GridView.UpdateCurrentRow();
            this.Cursor = Cursors.WaitCursor;
            try
            {
                ctr = new cCrystalReport("");
                if (ctr.SetPrinter() == false)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }
                db.ConnectionOpen();
                for (int i = 0; i <= GridView.DataRowCount - 1; i++)
                {
                    if (System.Convert.ToBoolean(GridView.GetRowCellValue(i, "PRINT")) == false)
                    {
                        continue;
                    }
                    if (GridView.GetRowCellDisplayText(i, "PONUM").Length == 0)
                    {
                        GridView.SetRowCellValue(i, "PONUM", SearchCuttingOrder(GridView.GetRowCellDisplayText(i, "CONTRACT"), GridView.GetRowCellDisplayText(i, "ORDER")));
                    }
                    ctr.ReportTitle = GridView.GetRowCellDisplayText(i, "ORDER");
                    ctr.ClearParameters();
                    ctr.SetParameter("SHIPPING_DATE", GridView.GetRowCellDisplayText(i, "DUE_DATE"));
                    string fmlText = "{XSLIP.PORDER}=\'" + GridView.GetRowCellDisplayText(i, "ORDER") + "\'";
                    switch (GridView.GetRowCellDisplayText(i, "WORK_CENTER").Substring(0, 4))
                    {
                        case "CUT-":
                            ctr.ReportFileName = Application.StartupPath + "\\Report\\CUTTING_ORDER.RPT";
                            ctr.PrintReport(fmlText, true);
                            SaveData(GridView.GetRowCellDisplayText(i, "ORDER"));
                            break;
                        case "SEW-":
                            ctr.ReportFileName = Application.StartupPath + "\\Report\\SEWING_ORDER.RPT";
                            ctr.PrintReport(fmlText, true);
                            //ctr.ClearParameters();
                            ctr.ReportFileName = Application.StartupPath + "\\Report\\ACCTransferOrd_SEW.RPT";
                            ctr.PrintReport(fmlText, true);
                            SaveData(GridView.GetRowCellDisplayText(i, "ORDER"));
                            break;
                        case "PACK":
                            ctr.ReportFileName = Application.StartupPath + "\\Report\\PACKING_ORDER.RPT";
                            ctr.PrintReport(fmlText, true);
                            //ctr.ClearParameters();
                            ctr.ReportFileName = Application.StartupPath + "\\Report\\ACCTransferOrd_PACK.RPT";
                            ctr.PrintReport(fmlText, true);
                            SaveData(GridView.GetRowCellDisplayText(i, "ORDER"));
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.ConnectionClose();
            }
            this.Cursor = Cursors.Default;
        }
        public void PrintPreview()
        {
            GridView.CloseEditor();
            GridView.UpdateCurrentRow();
            this.Cursor = Cursors.WaitCursor;
            try
            {
                ctr = new cCrystalReport("");
                int intCount = 0;
                for (int i = 0; i <= GridView.DataRowCount - 1; i++)
                {
                    if (System.Convert.ToBoolean(GridView.GetRowCellValue(i, "PRINT")) == false)
                    {
                        continue;
                    }
                    if (intCount > 4)
                    {
                        throw (new Exception("จำกัดให้ preview ได้ไม่เกิน 5 หน้า"));
                    }
                    ctr.ReportTitle = GridView.GetRowCellDisplayText(i, "ORDER");
                    ctr.ClearParameters();
                    ctr.SetParameter("SHIPPING_DATE", GridView.GetRowCellDisplayText(i, "DUE_DATE"));
                    string fmlText = "{XSLIP.PORDER}=\'" + GridView.GetRowCellDisplayText(i, "ORDER") + "\'";
                    switch (GridView.GetRowCellDisplayText(i, "WORK_CENTER").Substring(0, 4))
                    {
                        case "CUT-":
                            ctr.ReportFileName = Application.StartupPath + "\\Report\\CUTTING_ORDER.RPT";
                            ctr.PrintReport(fmlText, false);
                            break;
                        case "SEW-":
                            ctr.ReportFileName = Application.StartupPath + "\\Report\\SEWING_ORDER.RPT";
                            ctr.PrintReport(fmlText, false);
                            //ctr.ClearParameters();
                            ctr.ReportFileName = Application.StartupPath + "\\Report\\ACCTransferOrd_SEW.RPT";
                            ctr.PrintReport(fmlText, false);
                            break;
                        case "PACK":
                            ctr.ReportFileName = Application.StartupPath + "\\Report\\PACKING_ORDER.RPT";
                            ctr.PrintReport(fmlText, false);
                            //ctr.ClearParameters();
                            ctr.ReportFileName = (string)(Application.StartupPath + "\\Report\\ACCTransferOrd_PACK.RPT");
                            ctr.PrintReport(fmlText, false);
                            break;
                    }
                    intCount++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        public void ExportExcel()
        {
            SaveFileDialog theOpenFile = new SaveFileDialog();
            string strTemp;
            theOpenFile.Filter = "Microsoft Excel Document|*.xls";
            if (theOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                strTemp = theOpenFile.FileName;
                GridView.ExportToXls(strTemp);
            }
        }

		private string SearchCuttingOrder(string strContract, string strPO)
		{
			string strPonum;
			if (strContract.Length > 0)
			{
				string strSQL = "SELECT MAX(XSLIP.PORDER) AS PORDER FROM XSLIP WHERE CONTRACT=\'" + strContract + "\' AND BUMO LIKE \'CUT%\'";
				strPonum = db.ExecuteFirstValue(strSQL);
				strSQL = "UPDATE XSLIP SET XSLIP.PONUM=\'" + strPonum + "\' WHERE XSLIP.PORDER=\'" + strPO + "\'";
				db.Execute(strSQL);
			}
			else
			{
				strPonum = "";
			}
			return strPonum;
		}
		private void SaveData(string strPO)
		{
			string strSQL = "UPDATE XSLIP SET XSLIP.ISSUE=\'Y\' WHERE XSLIP.PORDER=\'" + strPO + "\'";
			db.Execute(strSQL);
		}
		private void frmWorkOrder_Load(System.Object sender, System.EventArgs e)
		{
            dtfinfo = clinfo.DateTimeFormat;
            dtpIssue.EditValue = DateTime.Today;
		}
		private void chkSelect_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			if (chkSelect.Checked)
			{
				for (int i = 0; i <= GridView.DataRowCount - 1; i++)
				{
					GridView.SetRowCellValue(i, "PRINT", true);
				}
			}
			else
			{
				for (int i = 0; i <= GridView.DataRowCount - 1; i++)
				{
					GridView.SetRowCellValue(i, "PRINT", false);
				}
			}
		}
		private void chkPO_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			if (chkPO.Checked)
			{
				txtPO1.Enabled = true;
				txtPO2.Enabled = true;
			}
			else
			{
				txtPO1.Enabled = false;
				txtPO2.Enabled = false;
			}
		}
		private void chkIssue_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			if (chkIssue.Checked)
			{
				dtpIssue.Enabled = true;
			}
			else
			{
				dtpIssue.Enabled = false;
			}
		}
		private void GridView_ColumnFilterChanged(object sender, System.EventArgs e)
		{
			this.bsiStatusbar.Caption = GridView.RowCount.ToString() + " Rows.";
		}
		private void cmdEnd_Click(System.Object sender, System.EventArgs e)
		{
			this.Dispose();
		}
		private void chkCode_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			if (chkCode.Checked)
			{
				txtCode.Enabled = true;
			}
			else
			{
				txtCode.Enabled = false;
			}
		}
		private void chkContract_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			if (chkContract.Checked)
			{
				txtContract.Enabled = true;
			}
			else
			{
				txtContract.Enabled = false;
			}
		}
 
    }
}