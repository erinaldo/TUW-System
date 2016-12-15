using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System
{
    public partial class frmTS5_PONew : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db=new cDatabase(Module.Fabric);
        DataTable dtPONew;

        public frmTS5_PONew()
        {
            InitializeComponent();
        }

        internal string cboRemark { get; set; }

        //private bool CheckDuplicatePORDER(string strPORDER)
        //{
        //    for(int i=0;i<frmPO.gridView1.DataRowCount;i++)
        //    {
        //        if(Equals(strPORDER, frmPO.GridView.GetRowCellDisplayText(i, "TPICS_ORDER"))){return true;}
        //    }
        //    return false;
        //}

        private void frmPO_New_Load(Object sender,System.EventArgs e)
        {
            //cmdImport.Image = My.Resources.Resource1.tab_center.ToBitmap
            //cmdClose.Image = My.Resources.Resource1._exit.ToBitmap
            //this.CenterToScreen();
            //this.TopMost = true;
            string strSQL="";
            switch(cboRemark)
            {
                case "RAW YARN":
                    strSQL = "SELECT XSLIP.PORDER, XSLIP.CODE, XHEAD.NAME AS BARCODE, XHEAD.MIXING" + 
                        ",CASE WHEN XITEM.KANZANZA=0 THEN XSLIP.KVOL " + 
                        "WHEN XITEM.KANZANZA=1 THEN XSLIP.KVOL*XITEM.KANZANK " + 
                        "WHEN XITEM.KANZANZA=2 THEN XSLIP.KVOL/XITEM.KANZANK END AS QTY " + 
                        ",CASE WHEN XITEM.KANZANZA=0 THEN XHEAD.TANI1	ELSE XITEM.TANI2 END AS UNIT " + 
                        ",XSLIP.PRICE, XSLIP.BUMO " + 
                        "FROM XSLIP INNER JOIN XHEAD ON XSLIP.CODE = XHEAD.CODE INNER JOIN XITEM ON XSLIP.CODE = XITEM.CODE AND XSLIP.BUMO=XITEM.BUMO " + 
                        "WHERE (XSLIP.PORDER LIKE 'XX%') ";
                    break;
                case "DYEING FEE":
                    strSQL = "SELECT XSLIP.PORDER, XSLIP.CODE, XHEAD.NAME AS BARCODE, XHEAD.MIXING" + 
                        ",CASE WHEN XITEM.KANZANZA=0 THEN XSLIP.KVOL " + 
                        "WHEN XITEM.KANZANZA=1 THEN XSLIP.KVOL*XITEM.KANZANK " + 
                        "WHEN XITEM.KANZANZA=2 THEN XSLIP.KVOL/XITEM.KANZANK END AS QTY " + 
                        ",CASE WHEN XITEM.KANZANZA=0 THEN XHEAD.TANI1	ELSE XITEM.TANI2 END AS UNIT " + 
                        ",XSLIP.PRICE, XSLIP.BUMO " + 
                        "FROM XSLIP INNER JOIN XHEAD ON XSLIP.CODE = XHEAD.CODE INNER JOIN XITEM ON XSLIP.CODE = XITEM.CODE AND XSLIP.BUMO=XITEM.BUMO  " + 
                        "WHERE (XSLIP.PORDER LIKE 'FW%') AND (XSLIP.VENDOR LIKE 'DYE%') ";
                    break;
                case "KNITTING FEE":
                    strSQL = "SELECT XSLIP.PORDER, XSLIP.CODE, XHEAD.NAME AS BARCODE, XHEAD.MIXING" + 
                        ",CASE WHEN XITEM.KANZANZA=0 THEN XSLIP.KVOL " + 
                        "WHEN XITEM.KANZANZA=1 THEN XSLIP.KVOL*XITEM.KANZANK " + 
                        "WHEN XITEM.KANZANZA=2 THEN XSLIP.KVOL/XITEM.KANZANK END AS QTY " + 
                        ",CASE WHEN XITEM.KANZANZA=0 THEN XHEAD.TANI1	ELSE XITEM.TANI2 END AS UNIT " + 
                        ",XSLIP.PRICE, XSLIP.BUMO " + 
                        "FROM XSLIP INNER JOIN XHEAD ON XSLIP.CODE = XHEAD.CODE INNER JOIN XITEM ON XSLIP.CODE = XITEM.CODE AND XSLIP.BUMO=XITEM.BUMO  " + 
                        "WHERE (XSLIP.PORDER LIKE 'FW%') AND (XSLIP.VENDOR LIKE 'KNT%') ";
                    break;
                case "SOAPING":
                    break;
            }
            strSQL+="AND (TJITU=0) AND (PO='')";
            dtPONew = db.GetDataTable(strSQL);
            DataColumn dc=new DataColumn();
            dc.ColumnName = "Check";
            dc.DataType = typeof(bool);
            dc.ReadOnly = false;
            dc.DefaultValue = false;
            dtPONew.Columns.Add(dc);
            dtPONew.Columns.Add("AMOUNT",typeof(double), "QTY*PRICE");
            gridControl1.DataSource = dtPONew;
            gridView1.Columns["BUMO"].VisibleIndex = 10;
            gridView1.Columns["Check"].VisibleIndex = 0;
            gridView1.Columns["QTY"].SummaryItem.FieldName = "QTY";
            gridView1.Columns["QTY"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
        }
        private void gridView1_RowStyle(object sender,DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if(Convert.ToBoolean((gridView1.GetRowCellValue(e.RowHandle, "Check"))))
            {
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Strikeout);
                e.Appearance.ForeColor = Color.Gray;
            }
        }
        //private void btnImport_Click(object sender,EventArgs e)
        //{
        //    frmS5_PO parent=(frmS5_PO)this.Owner;
        //    parent.AddRowsFromPONewGridView(ref gridView1);
           
        //}
        private void btnClose_Click(object sender,EventArgs e)
        {
            this.Dispose();
        }


    }




}