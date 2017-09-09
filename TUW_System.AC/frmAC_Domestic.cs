using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System.AC
{
    public partial class frmAC_Domestic : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        DataTable dtInvoice, dtDescription, dtCustomer,dtAmount;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        
        public frmAC_Domestic()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            ClearData(true);
        }
        public void ClearData(bool allControls)
        {
            if (allControls)
            {
                sleInvoice.EditValueChanged -= sleInvoice_EditValueChanged;
                sleInvoice.EditValue = null;
                sleInvoice.EditValueChanged += sleInvoice_EditValueChanged;
            }
            chkCancel.Checked = false;
            chkDebtor.Checked = false;
            chkSales.Checked = false;
            dtpDate.EditValue = null;
            sleDescription.EditValue = null;
            sleCustomer.EditValue = null;
            txtCustomer.Text = "";
            cboPayment.Text = "";
            txtCredit.Text = "";
            cboDepartment.Text = "";
            txtId.Text = "";
            txtQty.Text = "";
            cboUnit.Text = "";
            txtAmount.Text = "";
            txtVat.Text = "";
            txtSales.Text = "";
            dtpBillDate.EditValue = null;
            txtRV.Text = "";

            gridControl1.DataSource = null;
            dtAmount = new DataTable();
            dtAmount.BeginInit();
            DataColumn dc = new DataColumn();//,qty,amt,vat,amount
            dc.ColumnName = "section";
            dc.DataType = typeof(string);
            dtAmount.Columns.Add(dc);
            dc = new DataColumn();
            dc.ColumnName = "qty";
            dc.DataType = typeof(decimal);
            dtAmount.Columns.Add(dc);
            dc = new DataColumn();
            dc.ColumnName = "amt";
            dc.DataType = typeof(decimal);
            dtAmount.Columns.Add(dc);
            dc = new DataColumn();
            dc.ColumnName = "vat";
            dc.DataType = typeof(decimal);
            dtAmount.Columns.Add(dc);
            dc = new DataColumn();
            dc.ColumnName = "amount";
            dc.DataType = typeof(decimal);
            dtAmount.Columns.Add(dc);
            dtAmount.EndInit();
            gridControl1.DataSource = dtAmount;
            gridView1.PopulateColumns();
            gridView1.Columns["section"].Caption = "Department";
            gridView1.Columns["qty"].Caption = "Quantity";
            gridView1.Columns["amt"].Caption = "Amount";
            gridView1.Columns["vat"].Caption = "Vat";
            gridView1.Columns["amount"].Caption = "Sales";
            gridView1.OptionsView.ShowFooter = true;
            gridView1.Columns["qty"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridView1.Columns["amt"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridView1.Columns["vat"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridView1.Columns["amount"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
        }
        public void SaveData()
        {
            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                string strSQL = "select count(invoiceno) from domesticinvmain where invoiceno = '" + sleInvoice.Text + "'";
                if(db.ExecuteFirstValue(strSQL)=="0")
                {
                    strSQL="insert into domesticinvmain (invoiceno,invoicedate,descr,cust_no,payment,credit,"+
                        "section,idno,qty,unit,amt,vat,amount,billdate,InvCancel,Nodebtor,Nosalereport) values ("+
                        "'"+sleInvoice.Text+"'";
                    strSQL+=(dtpDate.EditValue==null)?",null":",'"+((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd",dtfinfo)+"'";
                    strSQL+=",N'"+sleDescription.Text+"',N'"+txtCustomer.Text+"','"+cboPayment.Text+"'";
                    strSQL+=(txtCredit.Text.Length==0)?",0":","+txtCredit.Text;
                    strSQL+=",'"+cboDepartment.Text+"','"+txtId.Text+"'";
                    strSQL+=(txtQty.Text.Length==0)?",0":","+txtQty.Text;
                    strSQL+=",'"+cboUnit.Text+"'";
                    strSQL+=(txtAmount.Text.Length==0)?",0":","+txtAmount.Text;
                    strSQL+=(txtVat.Text.Length==0)?",0":","+txtVat.Text;
                    strSQL+=(txtSales.Text.Length==0)?",0":","+txtSales.Text;
                    strSQL+=(dtpBillDate.EditValue==null)?",null":",'"+((DateTime)dtpBillDate.EditValue).ToString("yyyy-MM-dd",dtfinfo)+"'";
                    strSQL+=(chkCancel.Checked)?",1":",0";
                    strSQL+=(chkDebtor.Checked)?",1":",0";
                    strSQL+=(chkSales.Checked)?",1)":",0)";
                    db.Execute(strSQL);
                }
                else
                {
                    strSQL="update domesticinvmain set ";
                    strSQL+=(dtpDate.EditValue==null)?"invoicedate=null":"invoicedate='"+((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd",dtfinfo)+"'";
                    strSQL+=",descr=N'"+sleDescription.Text+"',cust_no = N'"+txtCustomer.Text+"',payment='"+cboPayment.Text+"'";
                    strSQL+=(txtCredit.Text.Length==0)?",credit=0":",credit='"+txtCredit.Text+"'";
                    strSQL += ",section='" + cboDepartment.Text + "',idno='" + txtId.Text + "'";
                    strSQL += (txtQty.Text.Length == 0) ? ",qty=0" : ",qty=" + txtQty.Text;
                    strSQL += ",unit = '" + cboUnit.Text + "'";
                    strSQL+=(txtAmount.Text.Length==0)?",amt=0":",amt="+txtAmount.Text;
                    strSQL += (txtVat.Text.Length == 0) ? ",vat=0" : ",vat=" + txtVat.Text;
                    strSQL += (txtSales.Text.Length == 0) ? ",amount=0" : ",amount=" + txtSales.Text;
                    strSQL += (dtpBillDate.EditValue == null) ? ",billdate = null" : ",billdate='" + ((DateTime)dtpBillDate.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    strSQL += (chkCancel.Checked) ? ",InvCancel=1" : ",InvCancel=0";
                    strSQL += (chkDebtor.Checked) ? ",Nodebtor=1" : ",NoDebtor=0";
                    strSQL += (chkSales.Checked) ? ",Nosalereport=1" : ",Nosalereport=0";
                    strSQL += " where invoiceno = '"+ sleInvoice.Text + "'";
                    db.Execute(strSQL);
                }
                GridToData();
                db.CommitTrans();
                MessageBox.Show("Save complete", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
        }

        private DataTable GetInvoices()
        {
            string strSQL = "select invoiceno from domesticinvmain order by invoiceno";
            dtInvoice = db.GetDataTable(strSQL);
            return dtInvoice;
        }
        private DataTable GetCustomers()
        {
            string strSQL = "select distinct cust_no,custnamee from customeracc order by custnamee";
            dtCustomer = db.GetDataTable(strSQL);
            return dtCustomer;
        }
        private DataTable GetDescriptions()
        {
            string strSQL = "select descr from domesticdesc order by descr";
            dtDescription = db.GetDataTable(strSQL);
            return dtDescription;
        }
        private void GetInvoiceDetail(string strInvoice)
        { 
            string strSQL="select invoiceno,invoicedate,a.descr,a.payment,a.credit,a.cust_no,b.custnamee"+
                ",section,qty,amt,unit,vat,amount,idno,billdate,InvCancel,NoDebtor,Nosalereport,rvno "+
                "from domesticInvmain a left join customeracc b on a.cust_no = b.cust_no "+
                "where invoiceno = '"+sleInvoice.Text+"'";
            DataTable dt=db.GetDataTable(strSQL);
            foreach(DataRow dr in dt.Rows)
            {
                dtpDate.EditValue=(dr["invoicedate"]==System.DBNull.Value)?(DateTime?)null:(DateTime)dr["invoicedate"];
                sleDescription.EditValue=dr["descr"];
                sleCustomer.EditValue=dr["cust_no"];
                txtCustomer.Text=(dr["cust_no"]==System.DBNull.Value)?"":dr["cust_no"].ToString();
                cboPayment.Text=(dr["payment"]==System.DBNull.Value)?"":dr["payment"].ToString();
                txtCredit.Text=(dr["credit"]==System.DBNull.Value)?"":dr["credit"].ToString();
                cboDepartment.Text=(dr["Section"]==System.DBNull.Value)?"":dr["Section"].ToString();
                txtId.Text=(dr["idno"]==System.DBNull.Value)?"":dr["idno"].ToString();
                txtQty.Text = (dr["qty"] == System.DBNull.Value) ? "" : dr["qty"].ToString();
                cboUnit.Text = (dr["unit"] == System.DBNull.Value) ? "" : dr["unit"].ToString();
                txtAmount.Text = (dr["amt"] == System.DBNull.Value) ? "" : dr["amt"].ToString();
                txtVat.Text = (dr["vat"] == System.DBNull.Value) ? "" : dr["vat"].ToString();
                txtSales.Text = (dr["amount"] == System.DBNull.Value) ? "" : dr["amount"].ToString();
                dtpBillDate.EditValue =(dr["billdate"]==System.DBNull.Value)?(DateTime?)null:(DateTime)dr["billdate"];
                txtRV.Text = (dr["rvno"] == System.DBNull.Value) ? "" : dr["rvno"].ToString();
                if (dr["InvCancel"] == System.DBNull.Value)
                    chkCancel.Checked = false;
                else
                    chkCancel.Checked = Convert.ToInt16(dr["InvCancel"]) == 1 ? true : false;
                if (dr["NoDebtor"] == System.DBNull.Value)
                    chkDebtor.Checked = false;
                else
                    chkDebtor.Checked = Convert.ToInt16(dr["NoDebtor"]) == 1 ? true : false;
                if (dr["nosalereport"] == System.DBNull.Value)
                    chkSales.Checked = false;
                else
                    chkSales.Checked = Convert.ToInt16(dr["nosalereport"]) == 1 ? true : false;
            }
               
            strSQL="select section,qty,amt,vat,amount from domesticinvdept where invoiceno = '"+sleInvoice.Text+"' order by seq";
            DataTable dt2=db.GetDataTable(strSQL);
            gridControl1.DataSource=dt2;
            gridView1.PopulateColumns();

            gridView1.Columns["section"].Caption = "Department";
            gridView1.Columns["qty"].Caption = "Quantity";
            gridView1.Columns["amt"].Caption = "Amount";
            gridView1.Columns["vat"].Caption = "Vat";
            gridView1.Columns["amount"].Caption = "Sales";

            gridView1.OptionsView.ShowFooter = true;
            gridView1.Columns["qty"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridView1.Columns["amt"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridView1.Columns["vat"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridView1.Columns["amount"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");

            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();

        }
        private void GridToData()
        {
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            string strSQL = "delete from domesticinvdept where invoiceno = '"+sleInvoice.Text+"'";
            db.Execute(strSQL);
            for (int i = 0; i < gridView1.DataRowCount; i++)
            { 
                strSQL = "insert into domesticinvdept (invoiceno,seq,section,qty,amt,vat,amount) values ("+
                    "'"+sleInvoice.Text+"','"+(i+1).ToString()+"','"+gridView1.GetRowCellValue(i,"section")+"'"+
                    ","+gridView1.GetRowCellValue(i,"qty")+","+gridView1.GetRowCellValue(i,"amt")+
                    ","+gridView1.GetRowCellValue(i,"vat")+","+gridView1.GetRowCellValue(i,"amount")+")";
                db.Execute(strSQL);
            }
        }

        private void frmAC_Domestic_Load(object sender, EventArgs e)
        {
            db=new cDatabase(_connectionString);
            dtfinfo=clinfo.DateTimeFormat;

            sleInvoice.Properties.DataSource = GetInvoices();
            sleInvoice.Properties.DisplayMember = "invoiceno";
            sleInvoice.Properties.ValueMember = "invoiceno";
            sleInvoice.Properties.View.OptionsView.ColumnAutoWidth = true;
            sleDescription.Properties.DataSource = GetDescriptions();
            sleDescription.Properties.DisplayMember = "descr";
            sleDescription.Properties.ValueMember = "descr";
            sleDescription.Properties.View.OptionsView.ColumnAutoWidth = true;
            sleCustomer.Properties.DataSource = GetCustomers();
            sleCustomer.Properties.DisplayMember = "custnamee";
            sleCustomer.Properties.ValueMember = "cust_no";
            sleCustomer.Properties.View.OptionsView.ColumnAutoWidth = true;
            ClearData(true);
        }
        private void sleInvoice_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ClearData(false);
                GetInvoiceDetail(sleInvoice.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnInvoice_Click(object sender, EventArgs e)
        {
            string strVal = "";
            if (myClass.cUtility.InputBox("Add", "Invoice No.", ref strVal) == DialogResult.OK)
            {
                dtInvoice.BeginInit();
                DataRow dr = dtInvoice.NewRow();
                dr["invoiceno"] = strVal;
                dtInvoice.Rows.Add(dr);
                dtInvoice.EndInit();
                sleInvoice.EditValue = strVal;
            }
        }
        private void btnDescription_Click(object sender, EventArgs e)
        {
            string strVal = "";
            if (myClass.cUtility.InputBox("Add", "Description", ref strVal) == DialogResult.OK)
            {
                dtDescription.BeginInit();
                DataRow dr = dtDescription.NewRow();
                dr["descr"] = strVal;
                dtDescription.Rows.Add(dr);
                dtDescription.EndInit();
                sleDescription.EditValue = strVal;
            }
        }
        private void btnCustomer_Click(object sender, EventArgs e)
        {
            string strVal_Name = "";
            string strVal_Code="";
            if (myClass.cUtility.InputBox("Add", "Customer - Name", ref strVal_Name) == DialogResult.OK)
            {
                if (myClass.cUtility.InputBox("Add", "Customer - Code", ref strVal_Code) == DialogResult.OK)
                {
                    dtCustomer.BeginInit();
                    DataRow dr = dtCustomer.NewRow();
                    dr["custnamee"] = strVal_Name;
                    dr["cust_no"] = strVal_Code;
                    dtCustomer.Rows.Add(dr);
                    dtCustomer.EndInit();
                    sleCustomer.EditValue = strVal_Code;
                }
            }
        }
        private void sleCustomer_EditValueChanged(object sender, EventArgs e)
        {
            txtCustomer.EditValue = sleCustomer.EditValue;
        }
        private void cboPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtCredit.Focus();
        }
        private void txtCredit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) cboDepartment.Focus();
        }
        private void cboDepartment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtId.Focus();
        }
        private void txtId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtQty.Focus();
        }
        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) cboUnit.Focus();
        }
        private void cboUnit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtAmount.Focus();
        }
        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtVat.Focus();
        }
        private void txtVat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) txtSales.Focus();
        }
        private void txtSales_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) dtpBillDate.Focus();
        }
        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            switch (gridView1.FocusedColumn.FieldName)
            { 
                case "amt":
                    decimal vat = Convert.ToDecimal(e.Value) * 0.07m;
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "vat", vat);
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "amount",Convert.ToDecimal(e.Value) + vat);
                    break;
            }
        }
    }
    /*

  


Private Sub FGrid_AfterEdit(ByVal Row As Long, ByVal Col As Long)
Dim i As Integer
Dim zTotal As Double
Dim zVat, zAmt As Double

    If Row = FGrid.Rows - 1 Then
        FGrid.Rows = FGrid.Rows + 1
    End If
    Select Case Col
        Case 2
            zTotal = 0
            For i = 1 To FGrid.Rows - 1
                zTotal = zTotal + ChkNumeric(FGrid.TextMatrix(i, 2))
            Next i
            TBSum(0).Text = Format(zTotal, "#,##0.00")
            FGrid.TextMatrix(Row, 2) = Format(FGrid.TextMatrix(Row, 2), "#,##0.00")
        Case 3
            zTotal = 0
            For i = 1 To FGrid.Rows - 1
                zTotal = zTotal + ChkNumeric(FGrid.TextMatrix(i, 3))
            Next i
            TBSum(1).Text = Format(zTotal, "#,##0.00")
            zVat = ChkNumeric(FGrid.TextMatrix(Row, 3)) * 7 / 100
            zAmt = ChkNumeric(FGrid.TextMatrix(Row, 3)) + zVat
            FGrid.TextMatrix(Row, 4) = Format(zVat, "#,##0.00")
            FGrid.TextMatrix(Row, 5) = Format(zAmt, "#,##0.00")
            FGrid.TextMatrix(Row, 3) = Format(FGrid.TextMatrix(Row, 3), "#,##0.00")
            zTotal = 0
            For i = 1 To FGrid.Rows - 1
                zTotal = zTotal + ChkNumeric(FGrid.TextMatrix(i, 4))
            Next i
            TBSum(2).Text = Format(zTotal, "#,##0.00")
            zTotal = 0
            For i = 1 To FGrid.Rows - 1
                zTotal = zTotal + ChkNumeric(FGrid.TextMatrix(i, 5))
            Next i
            TBSum(3).Text = Format(zTotal, "#,##0.00")
        Case 4
            zTotal = 0
            For i = 1 To FGrid.Rows - 1
                zTotal = zTotal + ChkNumeric(FGrid.TextMatrix(i, 4))
            Next i
            TBSum(2).Text = Format(zTotal, "#,##0.00")
            FGrid.TextMatrix(Row, 4) = Format(FGrid.TextMatrix(Row, 4), "#,##0.00")
            zVat = ChkNumeric(FGrid.TextMatrix(Row, 4))
            zAmt = ChkNumeric(FGrid.TextMatrix(Row, 3)) + ChkNumeric(FGrid.TextMatrix(Row, 4))
            FGrid.TextMatrix(Row, 5) = Format(zAmt, "#,##0.00")
            zTotal = 0
            For i = 1 To FGrid.Rows - 1
                zTotal = zTotal + ChkNumeric(FGrid.TextMatrix(i, 5))
            Next i
            TBSum(3).Text = Format(zTotal, "#,##0.00")
        Case 5
            zTotal = 0
            For i = 1 To FGrid.Rows - 1
                zTotal = zTotal + ChkNumeric(FGrid.TextMatrix(i, 5))
            Next i
            TBSum(3).Text = Format(zTotal, "#,##0.00")
            FGrid.TextMatrix(Row, 5) = Format(FGrid.TextMatrix(Row, 5), "#,##0.00")
    End Select
End Sub

Private Sub FGrid_KeyDown(KeyCode As Integer, Shift As Integer)
    Select Case KeyCode
        Case 46
            If FGrid.Col = 1 And FGrid.ColSel = FGrid.Cols - 1 Then
                If FGrid.RowSel <> FGrid.Rows - 1 Then
                    FGrid.RemoveItem (FGrid.Row)
                End If
            Else
                FGrid.TextMatrix(FGrid.RowSel, FGrid.ColSel) = ""
            End If
        End Select
End Sub

     */
}