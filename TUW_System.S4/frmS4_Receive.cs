using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Linq;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System.S4
{
    public partial class frmS4_Receive : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        CultureInfo clinfo=new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        public LogIn User_Login { get; set; }

        public frmS4_Receive()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            chkRePrint.Checked = false;
            optSearch.SelectedIndex = 0;
            txtInvoice.Text = "";
            txtSupplier.Text = "";
            chkFinish.Checked=false;
            txtReceive.Text = "";
            speRow.Value = 16;
            speCopy.Value = 3;
            chkSelect.Checked = false;
            gridControl1.DataSource = null;
        }
        public void DisplayData()
        { 
            this.Cursor=Cursors.WaitCursor;
            try 
	        {	        
		        string strSQL="SELECT Convert(bit,0) AS 'Print',Parfun.XSACT.VENDOR,Parfun.XSECT.NAME AS SUPPLIERNAME,Parfun.XSACT.CODE,"+
                    "Parfun.XHEAD.NAME AS ITEMNAME,Parfun.XSACT.PORDER,Parfun.XSACT.EDA,Parfun.XSACT.BUN,Parfun.XHEAD.TANI1,"+
                    "Parfun.XSACT.JITU0,Parfun.XSACT.RECEIVENO,Left(Parfun.XSACT.FDATE,8) AS Fdate,Parfun.XSACT.DELIVERYNO,"+
                    "Parfun.XSACT.INVOICE,Parfun.XSACT.DELIVERYNOTE,Parfun.XSACT.SECTION "+
                    "FROM Parfun.XSACT INNER JOIN Parfun.XHEAD ON Parfun.XSACT.CODE = Parfun.XHEAD.CODE "+
                    "INNER JOIN Parfun.XSECT ON Parfun.XSACT.VENDOR = Parfun.XSECT.BUMO ";
                if(optSearch.SelectedIndex==0)
                {
                    if(txtInvoice.Text.Length>0)
                        strSQL+=" Where (Invoice ='"+ txtInvoice.Text+"')";
                    else
                        throw new ApplicationException("Please input delivery no.");
                }
                else if(optSearch.SelectedIndex==1)
                {
                    if(txtSupplier.Text.Length>0)
                        strSQL+=" Where (Vendor ='"+ txtSupplier.Text + "')";
                    else
                        throw new ApplicationException("Please input supplier code");

                    if(chkFinish.Checked) strSQL+=" And Substring(Fdate,1,8)='"+((DateTime)dtpFinish.EditValue).ToString("yyyyMMdd",dtfinfo)+"'";
                }
                else
                {
                    if(txtReceive.Text.Length>0)
                        strSQL+=" Where (Receiveno ='"+ txtReceive.Text + "')";
                    else
                        throw new ApplicationException("Please input Receive no.");
                }
                if(!chkRePrint.Checked)
                    strSQL+=" And (Parfun.XSACT.RECEIVENO='')";
                else
                    strSQL+=" And (Not Parfun.XSACT.RECEIVENO='')";

                DataTable dt=db.GetDataTable(strSQL);
                gridControl1.DataSource=dt;
                gridView1.PopulateColumns();
                gridView1.Columns["VENDOR"].Caption="Supplier";
                gridView1.Columns["SUPPLIERNAME"].Caption="Supplier Name";
                gridView1.Columns["CODE"].Caption="Item Code";
                gridView1.Columns["ITEMNAME"].Caption="Item Name";
                gridView1.Columns["PORDER"].Caption="P/O No.";
                gridView1.Columns["TANI1"].Caption="Unit";
                gridView1.Columns["JITU0"].Caption="Qty";
                gridView1.Columns["RECEIVENO"].Caption="Receive No.";
                gridView1.Columns["Fdate"].Caption="Receive Date";
                gridView1.Columns["DELIVERYNO"].Caption="Delivery No.";
                gridView1.Columns["INVOICE"].Caption="Invoice No.";
                gridView1.Columns["DELIVERYNOTE"].Caption="Delivery Note";
                gridView1.Columns["SECTION"].Caption = "Section";
                gridView1.OptionsView.EnableAppearanceEvenRow=true;
                gridView1.OptionsView.EnableAppearanceOddRow=true;
                gridView1.OptionsView.ColumnAutoWidth=false;
                gridView1.BestFitColumns();

                chkSelect.Checked = true;
                chkSelect_CheckedChanged(null, null);
            }
            catch(ApplicationException ex)
            {
                MessageBox.Show(ex.Message,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
	        }
	        catch (SystemException ex)
	        {
		        MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
	        }
            this.Cursor=Cursors.Default;

        }
        private void PrintOrPreview(bool toPrinter)
        {

            cCrystalReport crpReceive = new cCrystalReport(Application.StartupPath + @"\Report\S4\ReceiveNote-Parfun.rpt");
            if (crpReceive.SetPrinter(Convert.ToInt16(speCopy.Value)) == false) { return; }
            crpReceive.ReportTitle = "Receive Note";//sleRecNo.Text;

            //'เริ่มเก็บค่า PO,EDA,BUN,pageNo
            int rowPerPage = Convert.ToInt32(speRow.Value);
            List<ReceivePrint> lstOrder = new List<ReceivePrint>();
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                if ((bool)gridView1.GetRowCellValue(i, "Print") == false) continue;
                int j = i + 1;
                lstOrder.Add(new ReceivePrint {
                    Porder=gridView1.GetRowCellValue(i,"PORDER").ToString(), 
                    Eda=(int)gridView1.GetRowCellValue(i,"EDA"),
                    Bun=(int)gridView1.GetRowCellValue(i,"BUN"),
                    Page=(j%rowPerPage==0)?j/rowPerPage:1+(j/rowPerPage) 
                });
            }
            var pages = Convert.ToInt32(Math.Ceiling(lstOrder.Count / speRow.Value));
            for (int i = 1; i <= pages; i++)
            {
                var obj = lstOrder.Where(w => w.Page == i);
                string fmlText = "";
                foreach (var item in obj)
                {
                    fmlText += " OR " + "({XSACT.Porder}='" + item.Porder + "' And {XSACT.EDA}=" + item.Eda + " And {XSACT.BUN}=" + item.Bun + ")";
                }
                fmlText = cUtility.Right(fmlText, fmlText.Length - 4); //ตัดคำว่า " OR " ออกจากด้านหน้าสุด
                for (int j = 1; j <= speCopy.Value; j++)
                {
                    crpReceive.ClearParameters();
                    crpReceive.SetParameter("Copy", j);
                    if(toPrinter)
                        crpReceive.PrintReport(fmlText, true, "sa", "");
                    else
                        crpReceive.PrintReport(fmlText, false, "sa", "");
                }
            }
        }
        public void PrintPreview()
        {
            try
            {
                gridView1.UpdateCurrentRow();
                PrintOrPreview(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void Print()
        {
            try
            {
                gridView1.UpdateCurrentRow();
                if (!chkRePrint.Checked)
                {
                    db.ConnectionOpen();
                    GenReceiveNo();
                }
                PrintOrPreview(true);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
            db.ConnectionClose();
        }

        private void GenReceiveNo()
        {
            int intIntVal1=0;
            string strName1="";
            string strSQL = "Select Name as Name1,Intval1,convert(varchar(8),getdate(),112) as YYYYMMDD From Parfun.tWorkData Where Code='ReceiveN'";
            DataTable dt=db.GetDataTable(strSQL);
            db.BeginTrans();
            foreach(DataRow dr in dt.Rows)
            {
                strName1 = dr["YYYYMMDD"].ToString().Substring(0, 6);
                if(Equals(strName1,dr["Name1"].ToString()))
                {
                    intIntVal1 =Convert.ToInt32(dr["intval1"]) + 1;
                    strSQL = "Update Parfun.tWorkData Set registration=getdate(), intval1=" + intIntVal1 + " Where Code='ReceiveN'";
                }
                else
                {
                    intIntVal1 = 1;
                    strSQL = "Update Parfun.tWorkData Set registration=getdate(), intval1=1,Name='" + strName1 + "' Where Code='ReceiveN'";
                }
                db.Execute(strSQL);
            }


            for(int i=0;i<gridView1.DataRowCount;i++)
            {
                if((bool)gridView1.GetRowCellValue(i, "Print") == false) continue;
                gridView1.SetRowCellValue(i,"RECEIVENO",strName1+intIntVal1.ToString().PadLeft(3,'0'));
                strSQL = "Update Parfun.XSACT Set Receiveno='" + gridView1.GetRowCellValue(i, "RECEIVENO") + "' " +
                    "Where Porder='" + gridView1.GetRowCellValue(i, "PORDER") + "' And EDA=" + gridView1.GetRowCellValue(i, "EDA") + " And BUN=" + gridView1.GetRowCellValue(i, "BUN");
                db.Execute(strSQL);
            }
            db.CommitTrans();
        }

        private void frmS4_Receive_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            NewData();
        }
        private void chkFinish_CheckedChanged(object sender, EventArgs e)
        {
            if(chkFinish.Checked) 
                dtpFinish.Enabled=true;
            else
                dtpFinish.Enabled=false;
        }
        private void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelect.Checked)
            {
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    gridView1.SetRowCellValue(i, "Print", true);
                }
            }
            else
            {
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    gridView1.SetRowCellValue(i, "Print", false);
                }
            }
        }
        private void txtInvoice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) DisplayData();
        }
        private void txtReceive_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) DisplayData();
        }
        private void optSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (optSearch.SelectedIndex)
            { 
                case 0:
                    txtInvoice.Enabled = true;
                    txtSupplier.Enabled = false;
                    txtReceive.Enabled = false;
                    break;
                case 1:
                    txtInvoice.Enabled = false;
                    txtSupplier.Enabled = true;
                    txtReceive.Enabled = false;
                    break;
                case 2:
                    txtInvoice.Enabled = false;
                    txtSupplier.Enabled = false;
                    txtReceive.Enabled = true;
                    break;
            }
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            var grid = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            grid.IndicatorWidth = 40;
        }

    }

    public class ReceivePrint
    {
        public string Porder { get; set; }
        public int Eda { get; set; }
        public int Bun { get; set; }
        public int Page { get; set; }
    }
}