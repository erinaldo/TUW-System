using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;
using System.Globalization;

namespace TUW_System.S5
{
    public partial class frmS5_ReceiveByDate : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;

        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;

        private string _connectionString;
        public string ConnectionString 
        {
            set { _connectionString = value; }    
        }

        public frmS5_ReceiveByDate()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            dtpReceive.EditValue=DateTime.Today;
            chkSelectAll.Checked=false;
            gridControl1.DataSource=null;
        }
        public void PrintPreview()
        {
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            this.Cursor = Cursors.WaitCursor;
            try
            {
                cCrystalReport crpPO = new cCrystalReport(Application.StartupPath + @"\Report\S5\S5_ReceiveNoteByDate.rpt");
                if (crpPO.SetPrinter() == false) { return; }
                string strReceiveDate = ((DateTime)dtpReceive.EditValue).ToString("dd/MM/yyyy", dtfinfo);
                for (int i = 1; i <= crpPO.ReportCopy; i++)
                {
                    for (int j = 0; j < gridView1.RowCount; j++)
                    {
                        if ((bool)gridView1.GetRowCellValue(j, "SELECT") == false) continue;
                        crpPO.ReportTitle = strReceiveDate + " " + gridView1.GetRowCellValue(j, "IDSUP");
                        crpPO.ClearParameters();
                        crpPO.SetParameter("Copy", i.ToString());
                        string fmlText = "{PO_Receive.ReceiveDate}=DateTime(" + strReceiveDate.Substring(6, 4) + "," +
                            strReceiveDate.Substring(3, 2) + "," + strReceiveDate.Substring(0, 2) + ",00,00,00) " +
                            "and {PO_Receive.IDSup} = '" + gridView1.GetRowCellValue(j, "IDSUP") + "'";
                        crpPO.PrintReport(fmlText, false,"sa","ZAQ113m4tuw");
                    }
                }
            }
            catch (Exception ex)
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
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            this.Cursor = Cursors.WaitCursor;
            try
            {
                cCrystalReport crpPO = new cCrystalReport(Application.StartupPath + @"\Report\S5\S5_ReceiveNoteByDate.rpt");
                if (crpPO.SetPrinter() == false) { return; }
                string strReceiveDate = ((DateTime)dtpReceive.EditValue).ToString("dd/MM/yyyy", dtfinfo);
                for (int i = 1; i <= crpPO.ReportCopy; i++)
                {
                    for (int j = 0; j < gridView1.RowCount; j++)
                    {
                        if ((bool)gridView1.GetRowCellValue(j, "SELECT") == false) continue;
                        crpPO.ReportTitle = strReceiveDate + " " + gridView1.GetRowCellValue(j, "IDSUP");
                        crpPO.ClearParameters();
                        crpPO.SetParameter("Copy", i.ToString());
                        string fmlText = "{PO_Receive.ReceiveDate}=DateTime(" + strReceiveDate.Substring(6, 4) + "," +
                            strReceiveDate.Substring(3, 2) + "," + strReceiveDate.Substring(0, 2) + ",00,00,00) " +
                            "and {PO_Receive.IDSup} = '" + gridView1.GetRowCellValue(j, "IDSUP") + "'";
                        crpPO.PrintReport(fmlText, true,"sa","ZAQ113m4tuw");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        public void DisplayData()
        {
            this.Cursor = Cursors.WaitCursor;
            try 
	        {
                DisplayData2((DateTime)dtpReceive.EditValue, cboType.Text);
	        }
	        catch (Exception ex)
	        {
			    MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
	        }
            this.Cursor = Cursors.Default;
        }

        private void DisplayData2(DateTime dtpReceive,string strType)
        {
            string strSQL = "SELECT DISTINCT A.IDSUP,B.NAME AS SUPPLIER "+
                " FROM PO_RECEIVE A LEFT OUTER JOIN PO_SUPPLIER B ON A.IDSUP=B.IDSUP"+
                " WHERE A.RECEIVEDATE='"+dtpReceive.ToString("yyyy-MM-dd",dtfinfo)+"';"+
                " SELECT A.IDSUP,A.PONO,A.DELIVERYNO,A.RECEIVENO,B.PRODUCTCODE,"+
	            " B.QTY,D.UNIT,B.UNITPRICE AS PRICE,B.QTY*B.UNITPRICE AS AMOUNT "+
                " FROM PO_RECEIVE A "+
	            " LEFT OUTER JOIN PO_RECEIVEDETAIL B ON A.RECEIVENO=B.RECEIVENO "+
	            " LEFT OUTER JOIN PO_SUPPLIER C ON A.IDSUP=C.IDSUP "+ 
	            " LEFT OUTER JOIN PO_UNIT D ON B.IDUNIT=D.IDUNIT "+
                " WHERE A.RECEIVEDATE='"+dtpReceive.ToString("yyyy-MM-dd",dtfinfo)+"' ";
            switch (strType)
            { 
                case "Yarn":
                    strSQL += "AND LEFT(A.PONO,2)='FX'";
                    break;
                case "Knitting":
                    strSQL += "AND LEFT(A.PONO,2)='FB'";
                    break;
                case "Dyeing":
                    strSQL += "AND LEFT(A.PONO,2)='FD'";
                    break;
            }
            strSQL+=" ORDER BY C.NAME ";
            DataSet ds=db.GetDataSet(strSQL);
            if (ds == null) return;
            DataColumn dc = new DataColumn();
            dc.ColumnName = "SELECT";
            dc.DataType = typeof(System.Boolean);
            dc.DefaultValue = false;
            ds.Tables[0].Columns.Add(dc);
            DataColumn keyColumn = ds.Tables[0].Columns["IDSUP"];
            DataColumn foreignKeyColumn = ds.Tables[1].Columns["IDSUP"];
            ds.Relations.Add("SupplierID", keyColumn, foreignKeyColumn);
            gridControl1.DataSource = ds.Tables[0];
            gridView1.PopulateColumns();
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            StatusBarEvent(ds.Tables[0].Rows.Count.ToString());
        }

        private void frmS5_ReceiveByDate_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            cboType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboType.Properties.Items.Add("All");
            cboType.Properties.Items.Add("Yarn");
            cboType.Properties.Items.Add("Knitting");
            cboType.Properties.Items.Add("Dyeing");
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            gridView1.IndicatorWidth = 30;
        }
        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelectAll.Checked)
            {
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    gridView1.SetRowCellValue(i, "SELECT", true);
                }
            }
            else
            {
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    gridView1.SetRowCellValue(i, "SELECT", false);
                }
            }
        }
        private void gridView1_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView master = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            DevExpress.XtraGrid.Views.Grid.GridView detail = (DevExpress.XtraGrid.Views.Grid.GridView)master.GetDetailView(e.RowHandle, e.RelationIndex);
            detail.Columns["IDSUP"].Visible = false;
            detail.OptionsView.EnableAppearanceEvenRow = true;
            detail.OptionsView.EnableAppearanceOddRow = true;
            detail.OptionsView.ColumnAutoWidth = false;
            detail.BestFitColumns();
        }


    }
}