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
    public partial class frmAC_ColDomestic : DevExpress.XtraEditors.XtraForm
    {
        public delegate void StatusBarHandler(string strInput);
        public event StatusBarHandler StatusBarEvent;

        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        DataTable dtVoucherDetail;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmAC_ColDomestic()
        {
            InitializeComponent();
        }
        public void ClearData(bool allControls)
        {
            if (allControls)
            {
                sleVoucher.EditValueChanged -= sleVoucher_EditValueChanged;
                sleVoucher.EditValue = null;
                sleVoucher.EditValueChanged += sleVoucher_EditValueChanged;
            }
            else
            {
                dtpCollect.EditValue = null;
                chkCredit.Checked = false;
                chkDebit.Checked = false;
                sleInvoice.EditValue = null;
                lblRvNo.Text = "";
                gridControl1.DataSource = null;
            }

            //Sub ClearData()
            //    TB(0).Text = ""
            //    TB(1).Text = ""
            //    TBAmt.Text = ""
            //    FGrid.Rows = 2
            //    FGrid.Cell(flexcpText, 1, 1, 1, FGrid.Cols - 1) = ""
            //    chkCredit.Value = vbUnchecked
            //    chkDebit.Value = vbUnchecked
            //End Sub
        }
        public void SaveData()
        {
            if (dtpCollect.EditValue == null)
            {
                MessageBox.Show("Can not Save you don't Insert Collect Date!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            decimal sumBalance=Convert.ToDecimal(gridView1.Columns["balance"].SummaryItem.SummaryValue);
            decimal sumReceive = Convert.ToDecimal(gridView1.Columns["amtcollect"].SummaryItem.SummaryValue);
            if ((sumBalance!=0 &&sumReceive==0) || sumReceive > sumBalance)
            {
                MessageBox.Show("Can not Save Collect Money more than Balance", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (chkCredit.Checked && chkDebit.Checked)
            {
                MessageBox.Show("Please select credit or debit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                gridView1.CloseEditor();
                gridView1.UpdateCurrentRow();
                string strSQL = "delete from domesticcollect where rvno = '"+sleVoucher.Text+"'";
                db.Execute(strSQL);
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    if (gridView1.GetRowCellValue(i, "invoiceno").ToString().Length == 0) continue;
                    strSQL="insert into domesticcollect (invoiceno,invoicedate,collectdate,amtcollect,rvno,cust_no,section,tocredit,todebit) values (";
                    strSQL +="'"+gridView1.GetRowCellValue(i,"invoiceno")+ "'";
                    strSQL +=",'"+((DateTime)gridView1.GetRowCellValue(i,"invoicedate")).ToString("yyyy-MM-dd",dtfinfo)+"'";
                    strSQL += (dtpCollect.EditValue == null) ? ",null" : ",'" + ((DateTime)dtpCollect.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    strSQL +=(gridView1.GetRowCellValue(i,"amtcollect").ToString().Length==0)?",0":","+gridView1.GetRowCellValue(i,"amtcollect");
                    strSQL +=",'"+sleVoucher.Text+"'";
                    strSQL +=",'"+gridView1.GetRowCellValue(i,"cust_no")+"'";
                    strSQL += ",'" + gridView1.GetRowCellValue(i, "section") + "'";
                    strSQL += (chkCredit.Checked) ? ",'1'" : ",'0'";
                    strSQL += (chkDebit.Checked) ? ",'1'" : ",'0'";
                    strSQL += ")";
                    db.Execute(strSQL);

                    strSQL="update domesticinvmain set collectdate = '"+((DateTime)dtpCollect.EditValue).ToString("yyyy-MM-dd",dtfinfo)+ "'";
                    strSQL+=",rvno = '"+sleVoucher.Text+ "'";
                    strSQL+=(chkCredit.Checked)?",tocredit='1'":",tocredit='0'";
                    strSQL+=(chkDebit.Checked)?",todebit='1'":",todebit='0'";
                    strSQL+=",amtcollect = (select isnull(sum(amtcollect),0) from domesticcollect " +
                        "where invoiceno = '" + gridView1.GetRowCellValue(i, "invoiceno") + "'" +
                        " and collectdate <= '" + ((DateTime)dtpCollect.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "')";
                    strSQL+=" where invoiceno = '"+gridView1.GetRowCellValue(i,"invoiceno")+ "'";
                    db.Execute(strSQL);





                //        Set Rst = Dbs.Execute(SqlStr)
                //        If Not Rst.EOF Then
                //            If Not IsNull(Rst!amount) Then
                //                zAmt = Rst!amount
                //            Else
                //                zAmt = 0
                //            End If
                //        Else
                //            zAmt = 0
                //        End If
                
                }

                
                //    Next i
                //    MsgBox "Complete", vbInformation
                //    CloseDB
                //    ClearData
                //    CBRv.Text = ""
                //    CBRv.SetFocus
                //End Sub



                db.CommitTrans();
                MessageBox.Show("Save complete.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;

            
            
            
            
            
        }

        private DataTable GetVouchers()
        {
            string strSQL = "select distinct rvno from domesticcollect order by rvno";
            DataTable dt = db.GetDataTable(strSQL);
            return dt;
        }
        private DataTable GetInvoices()
        {
            string strSQL = "select invoiceno from domesticinvmain order by invoiceno";
            DataTable dt = db.GetDataTable(strSQL);
            return dt;
        }
        private void GetVoucherDetail(string strVoucher)
        {
            string strSQL = "select a.collectdate,a.tocredit,a.todebit from domesticcollect a where a.rvno = '"+sleVoucher.Text+"'";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                dtpCollect.EditValue = (dr["collectdate"] == System.DBNull.Value) ? (DateTime?)null : (DateTime)dr["collectdate"];
                chkCredit.Checked = (dr["tocredit"].ToString() == "1") ? true : false;
                chkDebit.Checked = (dr["todebit"].ToString() == "1") ? true : false; 
            }
            strSQL = "select b.invoiceno,b.invoicedate,c.custnamee,b.descr,b.section,b.amount,"+
                "b.amount-(select isnull(sum(amtcollect),0) as amount from domesticcollect where invoiceno =a.InvoiceNo and collectdate <= a.collectdate and rvno <> '"+sleVoucher.Text+"') as balance,"+
                "a.amtcollect,b.cust_no " +
                "from domesticcollect a inner join domesticinvmain b on a.invoiceno = b.invoiceno " +
                "inner join customeracc c on b.cust_no = c.cust_no " +
                "where a.rvno = '" + sleVoucher.Text + "' order by b.invoiceno";
            dtVoucherDetail=db.GetDataTable(strSQL);
            gridControl1.DataSource = dtVoucherDetail;
            gridView1.PopulateColumns();
            gridView1.Columns["invoiceno"].Caption = "Invoice No.";
            gridView1.Columns["invoicedate"].Caption = "Invoice Date";
            gridView1.Columns["custnamee"].Caption = "Customer";
            gridView1.Columns["descr"].Caption = "Description";
            gridView1.Columns["section"].Caption = "Department";
            gridView1.Columns["amount"].Caption = "Amount";
            gridView1.Columns["balance"].Caption = "Balance";
            gridView1.Columns["amtcollect"].Caption = "Received";

            gridView1.Columns["cust_no"].Visible = false;

            gridView1.Columns["amount"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["amount"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["balance"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["balance"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["amtcollect"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["amtcollect"].DisplayFormat.FormatString = "n2";

            gridView1.Columns["balance"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridView1.Columns["amtcollect"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");

            gridView1.OptionsView.ShowFooter=true;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            StatusBarEvent(gridView1.DataRowCount + " Rows.");

            //Dim zAmt As Double
            //Dim i As Integer
            //Dim Rst As ADODB.Recordset
            //Dim zBalance As Double
            //Dim zSumBal As Double
            //Dim zSumCollect As Double

            //    ClearData
            
            //    If Not Rs.EOF Then
            //        i = 0
            //        zSumBal = 0
            //        zSumCollect = 0
            //        Do While Not Rs.EOF
            //            i = i + 1
            //            If i = FGrid.Rows - 1 Then
            //                FGrid.Rows = FGrid.Rows + 1
            //            End If
            //            FGrid.TextMatrix(i, 1) = Rs!invoiceno
            //            FGrid.TextMatrix(i, 2) = Format(Rs!invoicedate, "dd/mm/yyyy")
            //            FGrid.TextMatrix(i, 3) = Rs!custnamee
            //            FGrid.TextMatrix(i, 4) = Rs!descr
            //            FGrid.TextMatrix(i, 5) = Rs!Section
            //            FGrid.TextMatrix(i, 6) = Format(Rs!amount, "#,##0.00")
            //            FGrid.TextMatrix(i, 9) = Rs!cust_no
            //            SqlStr = "select sum(amtcollect) as amount from domesticcollect where invoiceno = '" & Rs!invoiceno & "'" & _
            //            " and collectdate <= '" & Format(TB(0).Text, "mm/dd/yyyy") & "' and rvno <> '" & CBRv.Text & "'"
            //            Set Rst = Dbs.Execute(SqlStr)
            //            If Not Rst.EOF Then
            //                If Not IsNull(Rst!amount) Then
            //                    zAmt = Rst!amount
            //                Else
            //                    zAmt = 0
            //                End If
            //            Else
            //                zAmt = 0
            //            End If
            //            Rst.Close
            //            zBalance = Rs!amount - zAmt
            //            FGrid.TextMatrix(i, 7) = Format(zBalance, "#,##0.00")
            //            FGrid.TextMatrix(i, 8) = Format(Rs!amtcollect, "#,##0.00")
            //            zSumBal = zSumBal + zBalance
            //            zSumCollect = zSumCollect + Rs!amtcollect
            //            Rs.MoveNext
            //        Loop
            //        TBAmt.Text = Format(zSumBal, "#,##0.00")
            //        TB(1).Text = Format(zSumCollect, "#,##0.00")
            //    Else
            //        TB(0).Text = Format(Date, "dd/mm/yyyy")
            //    End If
            //    Rs.Close

            //End Sub
        }
        private void AddInvoice()
        {
            string strSQL = "select a.invoiceno,max(a.invoicedate) as invoicedate ,max(descr) as descr," +
                "max(b.section) as section,a.rvno,sum(a.amtcollect) as amtcollect," +
                "max(b.amount) as amount,b.cust_no,max(custnamee) as custnamee" +
                " from domesticcollect a inner join domesticinvmain b on a.invoiceno = b.invoiceno" +
                " inner join customeracc c on b.cust_no = c.cust_no " +
                " where a.invoiceno = '" + sleInvoice.Text + "'" +
                " and a.collectdate <= '" + ((DateTime)dtpCollect.EditValue).ToString("yyyy-MM-dd",dtfinfo) + "'"+
                " and a.rvno <> '" + sleVoucher.Text+ "'" +
                " group by a.invoiceno,b.cust_no,a.rvno";
            DataTable dt = db.GetDataTable(strSQL);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow drNew =dtVoucherDetail.NewRow();
                    drNew["invoiceno"] = dr["invoiceno"];
                    drNew["invoicedate"] = dr["invoicedate"];
                    drNew["custnamee"] = dr["custnamee"];
                    drNew["descr"] = dr["descr"];
                    drNew["section"] = dr["section"];
                    drNew["amount"] = dr["amount"];
                    drNew["balance"] =Convert.ToDecimal(dr["amount"])-Convert.ToDecimal(dr["amtcollect"]);
                    dtVoucherDetail.Rows.Add(drNew);
                    dtVoucherDetail.AcceptChanges();
                    lblRvNo.Text=dr["rvno"].ToString();
                }
             //   FGrid.Rows = FGrid.Rows + 1
            //        FGrid.TextMatrix(zRow, 1) = Rs!invoiceno
            //        FGrid.TextMatrix(zRow, 2) = Format(Rs!invoicedate, "dd/mm/yyyy")
            //        FGrid.TextMatrix(zRow, 3) = Rs!custnamee
            //        FGrid.TextMatrix(zRow, 4) = Rs!descr
            //        FGrid.TextMatrix(zRow, 5) = Rs!Section
            //        FGrid.TextMatrix(zRow, 6) = Format(Rs!amount, "#,##0.00")
            //        zBalance = Rs!amount - Rs!amtcollect
            //        FGrid.TextMatrix(zRow, 7) = Format(zBalance, "#,##0.00")
            //        FGrid.TextMatrix(zRow, 9) = Rs!cust_no
            //        LB1(3).Caption = Rs!rvno
            //        Rs.Close
            }
            else
            {
                strSQL = "select invoiceno,invoicedate,descr,section,amount,domesticinvmain.cust_no,custnamee,rvno" +
                    " from domesticinvmain inner join customeracc on domesticinvmain.cust_no = customeracc.cust_no " +
                    "where invoiceno = '" + sleInvoice.Text + "'";
                dt = db.GetDataTable(strSQL);
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow drNew = dtVoucherDetail.NewRow();
                    drNew["invoiceno"] = dr["invoiceno"];
                    drNew["invoicedate"] = dr["invoicedate"];
                    drNew["descr"] = dr["descr"];
                    drNew["custnamee"] = dr["custnamee"];
                    drNew["section"] = dr["section"];
                    drNew["amount"] = dr["amount"];
                    drNew["balance"] = dr["amount"];
                    dtVoucherDetail.Rows.Add(drNew);
                    dtVoucherDetail.AcceptChanges();
                    lblRvNo.Text = dr["rvno"].ToString();
                }
            //        Set Rs = Dbs.Execute(SqlStr)
            //        If Not Rs.EOF Then
            //            FGrid.Rows = FGrid.Rows + 1
            //            FGrid.TextMatrix(zRow, 1) = Rs!invoiceno
            //            FGrid.TextMatrix(zRow, 2) = Format(Rs!invoicedate, "dd/mm/yyyy")
            //            FGrid.TextMatrix(zRow, 3) = Rs!descr
            //            FGrid.TextMatrix(zRow, 4) = Rs!custnamee
            //            FGrid.TextMatrix(zRow, 5) = Rs!Section
            //            FGrid.TextMatrix(zRow, 6) = Format(Rs!amount, "#,##0.00")
            //            FGrid.TextMatrix(zRow, 7) = Format(Rs!amount, "#,##0.00")
            //            FGrid.TextMatrix(zRow, 9) = Rs!cust_no
            //            LB1(3).Caption = Rs!rvno
            //        End If
            }
            
            //    LB1(3).Caption = ""


            //    zSumBal = 0
            //    zSumCollect = 0
            //    For i = 1 To FGrid.Rows - 1
            //        If Len(FGrid.TextMatrix(i, 1)) = 0 Then
            //            Exit For
            //        End If
            //        zSumBal = zSumBal + ChkNumeric(FGrid.TextMatrix(i, 7))
            //        zSumCollect = zSumCollect + ChkNumeric(FGrid.TextMatrix(i, 8))
            //    Next i
            //    TBAmt.Text = Format(zSumBal, "#,##0.00")
            //    TB(1).Text = Format(zSumCollect, "#,##0.00")
        }

        private void frmAC_ColDomestic_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            sleVoucher.Properties.DataSource = GetVouchers();
            sleVoucher.Properties.DisplayMember = "rvno";
            sleVoucher.Properties.ValueMember = "rvno";
            sleVoucher.Properties.View.OptionsView.ColumnAutoWidth = true;
            sleInvoice.Properties.DataSource = GetInvoices();
            sleInvoice.Properties.DisplayMember = "invoiceno";
            sleInvoice.Properties.ValueMember = "invoiceno";
            sleInvoice.Properties.View.OptionsView.ColumnAutoWidth = true;
            


        }
        private void sleVoucher_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ClearData(false);
                GetVoucherDetail(sleVoucher.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //Private Sub CmdReceived_Click()
            //Dim i As Integer
            //Dim zSumCollect As Double
            //    zSumCollect = 0
            //    For i = 1 To FGrid.Rows - 1
            //        If Len(FGrid.TextMatrix(i, 1)) = 0 Then
            //            Exit For
            //        End If
            //        FGrid.TextMatrix(i, 8) = FGrid.TextMatrix(i, 7)
            //        zSumCollect = zSumCollect + ChkNumeric(FGrid.TextMatrix(i, 8))
            //    Next i
            //    TB(1).Text = Format(zSumCollect, "#,##0.00")
            //End Sub
        }
        private void sleInvoice_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                AddInvoice();
                gridView1.BestFitColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }
        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && !gridView1.IsEditing ) gridView1.DeleteSelectedRows();
        }
    }
}