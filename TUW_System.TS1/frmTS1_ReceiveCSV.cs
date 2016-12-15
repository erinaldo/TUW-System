using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System.TS1
{
    public partial class frmTS1_ReceiveCSV : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;

        cDatabase db;
        DevExpress.XtraGrid.Views.Grid.GridView gridViewActive;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmTS1_ReceiveCSV()
        {
            InitializeComponent();
        }
        public void ClearData()
        { 
        
        }
        public void DisplayData()
        {
            string strSQL ="EXEC spTUWSystem_TS1_ReceiveCSV";
            DataSet ds = db.GetDataSet(strSQL);
            var remain = new Dictionary<string, decimal>();
            decimal temp;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["PO"] == DBNull.Value) continue;
                if (!remain.ContainsKey(dr["PORDER"].ToString()))
                {
                    temp=(decimal)dr["PO"] - (decimal)dr["CSV"] - (decimal)dr["JITU"];
                    remain.Add(dr["PORDER"].ToString(),temp );
                }
                else
                {
                    temp = remain[dr["PORDER"].ToString()]- (decimal)dr["JITU"];
                    remain[dr["PORDER"].ToString()] = temp;
                }
                if (temp < 0)
                {
                    dr["REMAIN"] = temp;
                    dr["JITU"] = (decimal)dr["JITU"]+temp;
                    if((int)dr["KANZANV"]==0)
                        dr["JITU0"]=dr["JITU"];
                    else if((int)dr["KANZANV"]==1)
                        dr["JITU0"]=(decimal)dr["JITU"]/(decimal)dr["KANZANK"];
			        else if((int)dr["KANZANV"]==2)
                        dr["JITU0"]=(decimal)dr["JITU"]*(decimal)dr["KANZANK"];
		        }
            }
            gridControl1.DataSource = null;
            gridControl1.DataSource = ds.Tables[0];
            gridView1.PopulateColumns();
            gridView1.Columns["KEY"].Visible = false;
            gridView1.Columns["PORDER"].OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            gridView1.Columns["RECEIVENO"].OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            gridView1.Columns["CONT"].Width = 100;
            gridView1.Columns["CONTRACT"].Width = 100;
            for (int i = 0; i < gridView1.Columns.Count; i++)
            {
                gridView1.Columns[i].OptionsColumn.ReadOnly = true;
            }
            gridView1.Columns["SELECT"].OptionsColumn.ReadOnly = false;
            StatusBarEvent(gridView1.DataRowCount + " Rows");
            ds.Tables[1].Rows.Clear();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if ((decimal)dr["REMAIN"] < 0)
                {
                    DataRow drNew = ds.Tables[1].NewRow();
                    drNew["STATUS"] = "";
                    drNew["SELECT"] = false;
                    drNew["PORDER"] = dr["PONO"];
                    drNew["EDA"] = dr["EDA"];
                    drNew["CODE"] = dr["CODE"];
                    drNew["BUMO"] = dr["BUMO"];
                    drNew["JITU"] = Math.Abs((decimal)dr["REMAIN"]);

                    if ((int)dr["KANZANV"] == 0)
                        drNew["JITU0"] = drNew["JITU"];
                    else if ((int)dr["KANZANV"] == 1)
                        drNew["JITU0"] = (decimal)drNew["JITU"] / (decimal)dr["KANZANK"];
                    else if ((int)dr["KANZANV"] == 2)
                        drNew["JITU0"] = (decimal)drNew["JITU"] * (decimal)dr["KANZANK"];

                    drNew["UNIT"] = dr["UNIT"];
                    drNew["APRICE"] = dr["APRICE"];
                    drNew["VENDOR"] = dr["VENDOR"];
                    drNew["CONT"] = dr["CONT"];
                    drNew["CODESUB"] = dr["CODESUB"];
                    drNew["CUSTOMER"] = dr["CUSTOMER"];
                    drNew["LABNO"] = dr["LABNO"];
                    drNew["GM2"] = dr["GM2"];
                    drNew["WIDTH"] = dr["WIDTH"];
                    drNew["CONTRACT"] = dr["CONTRACT"];
                    drNew["NOUBA"] = dr["NOUBA"];
                    drNew["RECEIVENO"] = dr["RECEIVENO"];
                    drNew["INVNO"] = dr["INVNO"];
                    drNew["AKUBU"] = "T";
                    drNew["FDATE"] = dr["FDATE"];
                    drNew["HOKAN"] = dr["HOKAN"];
                    drNew["IMPOROUT"] = dr["IMPOROUT"];
                    ds.Tables[1].Rows.Add(drNew);
                }
            }
            ds.Tables[1].AcceptChanges();
            gridControl2.DataSource = null;
            gridControl2.DataSource = ds.Tables[1];
            gridView2.PopulateColumns();
            gridView2.Columns["PORDER"].OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            gridView2.Columns["RECEIVENO"].OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            for (int i = 0; i < gridView2.Columns.Count; i++)
            {
                gridView2.Columns[i].OptionsColumn.ReadOnly = true;
            }
            gridView2.Columns["SELECT"].OptionsColumn.ReadOnly = false;
            gridView2.OptionsView.EnableAppearanceEvenRow = true;
            gridView2.OptionsView.EnableAppearanceOddRow = true;
            gridView2.OptionsView.ColumnAutoWidth = false;
            gridView2.BestFitColumns();
            gridView2.Columns["CONT"].Width = 100;
            gridView2.Columns["CONTRACT"].Width = 100;
        }
        public void ExportExcel()
        { 
        
        }
        public void ExportCSV()
        {
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "CSV files|*.csv";
                if (dlg.ShowDialog() != DialogResult.OK) return;
                this.Cursor = Cursors.WaitCursor;
                TextWriter tw=new StreamWriter(dlg.FileName);
                tw.WriteLine("PORDER,EDA,CODE,BUMO,JITU,JITU0,VENDOR,APRICE,CONT,CODESUB,CUSTOMER,LABNO,GM2,WIDTH," +
                    "CONTRACT,NOUBA,RECEIVENO,ORDERNO,AKUBU,FDATE,HOKAN");
                gridViewActive.CloseEditor();
                gridViewActive.UpdateCurrentRow();
                for (int i = 0; i < gridViewActive.DataRowCount; i++)
                {
                    if ((bool)gridViewActive.GetRowCellValue(i, "SELECT") == false) continue;
                    tw.Write(gridViewActive.GetRowCellValue(i, "PORDER") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "EDA") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "CODE") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "BUMO") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "JITU") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "JITU0") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "VENDOR") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "APRICE") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "CONT") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "CODESUB") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "CUSTOMER") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "LABNO") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "GM2") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "WIDTH") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "CONTRACT") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "NOUBA") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "RECEIVENO") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "INVNO") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "AKUBU") + ",");
                    tw.Write(gridViewActive.GetRowCellValue(i, "FDATE") + ",");
                    tw.WriteLine(gridViewActive.GetRowCellValue(i, "HOKAN") + ",");
                    string strSQL = "UPDATE RDRECEIVE SET CSVOK='1' WHERE EDA=" + gridView1.GetRowCellDisplayText(i, "KEY");
                    db.Execute(strSQL);
                }
                tw.Close();
                db.CommitTrans();
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
        }

        private DevExpress.XtraGrid.Views.Grid.GridView GetActiveGridView(DevExpress.XtraTab.XtraTabControl currentTab)
        {
            foreach (Control ctr in currentTab.SelectedTabPage.Controls)
            {
                if (ctr is DevExpress.XtraGrid.GridControl)
                {
                    DevExpress.XtraGrid.GridControl gc = (DevExpress.XtraGrid.GridControl)ctr;
                    for (int i = 0; i < gc.ViewCollection.Count; i++)
                    {
                        if (gc.ViewCollection[i].GetType() == typeof(DevExpress.XtraGrid.Views.Grid.GridView))
                            return gridViewActive = (DevExpress.XtraGrid.Views.Grid.GridView)gc.ViewCollection[i];
                    }
                }
            }
            return null;
        }

        private void frmTS1_ReceiveCSV_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            if (xtraTabControl1.SelectedTabPageIndex == 0)//No generate CSV
                gridViewActive = GetActiveGridView(xtraTabControl2);
            else//Generate CSV already
                gridViewActive = GetActiveGridView(xtraTabControl3);
        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridViewActive.RowCount; i++)
            {
                gridViewActive.SetRowCellValue(i, "SELECT", true);
            }
        }
        private void btnUnSelect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridViewActive.RowCount; i++)
            {
                gridViewActive.SetRowCellValue(i, "SELECT", false);
            }
        }
        private void xtraTabControl2_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            gridViewActive=GetActiveGridView(xtraTabControl2);
            StatusBarEvent(gridViewActive.DataRowCount.ToString() + " Rows");
        }
        private void xtraTabControl3_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            gridViewActive=GetActiveGridView(xtraTabControl3);
            StatusBarEvent(gridViewActive.DataRowCount.ToString() + " Rows");
        }
        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (xtraTabControl1.SelectedTabPageIndex == 0)//No generate CSV
                gridViewActive = GetActiveGridView(xtraTabControl2);
            else//Generate CSV already
                gridViewActive = GetActiveGridView(xtraTabControl3);
            StatusBarEvent(gridViewActive.DataRowCount.ToString() + " Rows");
        }
        private void gridView_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            DevExpress.XtraGrid.Views.Grid.GridView gv=(DevExpress.XtraGrid.Views.Grid.GridView)sender;
            gv.IndicatorWidth = 40;
        }
    }
}