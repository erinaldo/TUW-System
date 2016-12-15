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
    public partial class frmS4_PO : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        public LogIn User_Login { get; set; }

        public frmS4_PO()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            chkRePrint.Checked = false;
            lblDate.Text = DateTime.Now.ToString();
            txtSupplier1.Text = "";
            txtSupplier2.Text = "";
            txtPO1.Text = "";
            txtPO2.Text = "";
            speRow.Value = 10;
            speCopy.Value = 1;
        }
        public void DisplayData()
        { 
        
        }
        public void PrintPreview()
        {
            try
            {
                if(chkRePrint.Checked)
                    PrintOrderBalance(false);
                else
                    PrintOrder(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        //Dim i As Integer
        //Call ConnectTPICS()
        //ctrPO.PrinterSelect()
        //For i = 1 To nudCopy.Value
        //    If chkRePrint.Checked = True Then
        //        Call PrintOrderBalance(True, i)
        //    Else
        //        Call PrintOrder(True, i)
        //    End If
        //Next
        //Call DisConnectTPICS()
        }
        public void Print()
        {
            try
            {
                if(chkRePrint.Checked)
                    PrintOrderBalance(true);
                else
                    PrintOrder(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        //    Dim i As Integer
        //Call ConnectTPICS()
        //ctrPO.PrinterSelect()
        //For i = 1 To nudCopy.Value
        //    If chkRePrint.Checked = True Then
        //        Call PrintOrderBalance(False, i)
        //    Else
        //        Call PrintOrder(False, i)
        //    End If
        //Next
        //Call DisConnectTPICS()
        }

        private void PrintOrder(bool toPrinter)
        {
            cCrystalReport crpPO = new cCrystalReport(Application.StartupPath + @"\Report\XDENP-Parfun.rpt");
            if (crpPO.SetPrinter(Convert.ToInt16(speCopy.Value)) == false) { return; }
            crpPO.ReportTitle = "Purchase Order";

            int rowPerPage = Convert.ToInt32(speRow.Value);
            //หาค่า supplier,duedate
            string strSQL = "Select Distinct Vendor,Ndate From Parfun.XDENP Where left(Porder,2)='PX' ";
            if (txtSupplier1.Text.Trim().Length > 0 && txtSupplier2.Text.Trim().Length > 0)
                strSQL += " And (Vendor Between '" + txtSupplier1.Text + "' And '" + txtSupplier2.Text + "')"; //'ถ้าไม่มีการระบุ supplier แสดงว่าต้องการเลือกทั้งหมด
            if (txtPO1.Text.Trim().Length > 0 && txtPO2.Text.Trim().Length > 0)
                strSQL += " And (Porder Between '" + txtPO1.Text + "' And '" + txtPO2.Text + "')";
            DataTable dt = db.GetDataTable(strSQL);

            foreach (DataRow dr in dt.Rows)
            {
                strSQL = "Select Porder,Eda From Parfun.XDENP Where (Vendor='" + dr["vendor"].ToString() + "') And (Ndate='" + dr["ndate"].ToString() + "')";
                if (txtPO1.Text.Trim().Length > 0 && txtPO2.Text.Trim().Length > 0) strSQL += " And (Porder Between '" + txtPO1.Text + "' And '" + txtPO2.Text + "')";
                strSQL += " Order by Porder,Eda";
                DataTable dtOrder = db.GetDataTable(strSQL);
                int count = 0;
                List<ReceivePrint> lstOrder = new List<ReceivePrint>();
                foreach (DataRow drOrder in dtOrder.Rows)
                {
                    count += 1;
                    lstOrder.Add(new ReceivePrint
                    {
                        Porder = drOrder["Porder"].ToString(),
                        Eda = (int)drOrder["Eda"],
                        Page = (count % rowPerPage == 0) ? count / rowPerPage : 1 + (count / rowPerPage)
                    });
                }
                var pages = Convert.ToInt32(Math.Ceiling(lstOrder.Count / speRow.Value));
                for (int i = 1; i <= pages; i++)
                {
                    var obj = lstOrder.Where(w => w.Page == i);
                    string fmlText = "({XDENP.vendor}='" + dr["vendor"].ToString() + "' And {XDENP.ndate}='" + dr["ndate"].ToString() + "') And (";
                    foreach (var item in obj)
                    {
                        fmlText += "({XDENP.Porder}='" + item.Porder + "' And {XDENP.EDA}=" + item.Eda + ") OR ";
                    }
                    fmlText = fmlText.Substring(0, fmlText.Length - 4);//ตัดคำว่า " OR " ออกจากด้านหลังสุด
                    fmlText += ")";//ปิดท้ายด้วยวงเล็บ
                    for (int j = 1; j <= speCopy.Value; j++)
                    {
                        crpPO.ClearParameters();
                        crpPO.SetParameter("Copy", j);
                        if (toPrinter)
                            crpPO.PrintReport(fmlText, true, "sa", "");
                        else
                            crpPO.PrintReport(fmlText, false, "sa", "");
                    }
                }
            }
        }
        private void PrintOrderBalance(bool toPrinter)
        {
            cCrystalReport crpPO = new cCrystalReport(Application.StartupPath + @"\Report\XSLIP-Parfun.rpt");
            if (crpPO.SetPrinter(Convert.ToInt16(speCopy.Value)) == false) { return; }
            crpPO.ReportTitle = "Purchase Order";

            int rowPerPage = Convert.ToInt32(speRow.Value);
            //หาค่า supplier,duedate
            string strSQL = "Select Distinct Vendor,Ndate From Parfun.Xslip Where left(Porder,2)='PX' ";
            if (txtSupplier1.Text.Trim().Length > 0 && txtSupplier2.Text.Trim().Length > 0)
                strSQL += " And (Vendor Between '" + txtSupplier1.Text + "' And '" + txtSupplier2.Text + "')"; //'ถ้าไม่มีการระบุ supplier แสดงว่าต้องการเลือกทั้งหมด
            if (txtPO1.Text.Trim().Length > 0 && txtPO2.Text.Trim().Length > 0)
                strSQL += " And (Porder Between '" + txtPO1.Text + "' And '" + txtPO2.Text + "')";
            DataTable dt = db.GetDataTable(strSQL);
            
            foreach (DataRow dr in dt.Rows)
            {
                strSQL = "Select Porder,Eda From Parfun.Xslip Where (Vendor='" + dr["vendor"].ToString() + "') And (Ndate='" + dr["ndate"].ToString() + "')";
                if (txtPO1.Text.Trim().Length > 0 && txtPO2.Text.Trim().Length > 0) strSQL += " And (Porder Between '" + txtPO1.Text + "' And '" + txtPO2.Text + "')";
                strSQL += " Order by Porder,Eda";
                DataTable dtOrder = db.GetDataTable(strSQL);
                int count = 0;
                List<ReceivePrint> lstOrder = new List<ReceivePrint>();
                foreach (DataRow drOrder in dtOrder.Rows)
                {
                    count += 1;
                    lstOrder.Add(new ReceivePrint
                    {
                        Porder = drOrder["Porder"].ToString(),
                        Eda = (int)drOrder["Eda"],
                        Page = (count % rowPerPage == 0) ? count / rowPerPage : 1 + (count / rowPerPage)
                    });
                }
                var pages = Convert.ToInt32(Math.Ceiling(lstOrder.Count / speRow.Value));
                for (int i = 1; i <= pages; i++)
                {
                    var obj = lstOrder.Where(w => w.Page == i);
                    string fmlText = "({Xslip.vendor}='" + dr["vendor"].ToString() + "' And {Xslip.ndate}='" + dr["ndate"].ToString() + "') And (";
                    foreach (var item in obj)
                    {
                        fmlText += "({XSLIP.Porder}='" + item.Porder + "' And {XSLIP.EDA}=" + item.Eda + ") OR ";
                    }
                    fmlText = fmlText.Substring(0, fmlText.Length - 4);//ตัดคำว่า " OR " ออกจากด้านหลังสุด
                    fmlText += ")";//ปิดท้ายด้วยวงเล็บ
                    for (int j = 1; j <= speCopy.Value; j++)
                    {
                        crpPO.ClearParameters();
                        crpPO.SetParameter("Copy", j);
                        if (toPrinter)
                            crpPO.PrintReport(fmlText, true, "sa", "");
                        else
                            crpPO.PrintReport(fmlText, false, "sa", "");
                    }
                }
            }
        }

        private void frmS4_PO_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            NewData();
        }
        private void txtSupplier1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txtSupplier2.Focus();
                txtSupplier2.SelectAll();
            }
        }
        private void txtSupplier2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txtPO1.Focus();
                txtPO1.SelectAll();
            }
        }
        private void txtPO1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txtPO2.Focus();
                txtPO2.SelectAll();
            }
        }

    }

}