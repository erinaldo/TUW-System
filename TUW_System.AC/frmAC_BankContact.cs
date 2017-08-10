using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System.AC
{
    public partial class frmAC_BankContact : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        DataTable dtInvoiceDetail;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }

        public frmAC_BankContact()
        {
            InitializeComponent();
        }
        public void NewData()
        { 
            sleInvoice.EditValueChanged -= sleInvoice_EditValueChanged;
            sleInvoice.EditValue = null;
            sleInvoice.EditValueChanged += sleInvoice_EditValueChanged;
            dtpDate.EditValue = DateTime.Today;
            dtInvoiceDetail = new DataTable();
            dtInvoiceDetail.BeginInit();
            dtInvoiceDetail.Columns.Add("invoice_no", typeof(string));
            dtInvoiceDetail.Columns.Add("amt", typeof(decimal));
            dtInvoiceDetail.Columns.Add("inv_grp", typeof(string));
            dtInvoiceDetail.EndInit();
            gridControl1.DataSource = dtInvoiceDetail;
            gridView1.Columns["invoice_no"].Caption = "Invoice No.";
            gridView1.Columns["amt"].Caption = "Amt";
            gridView1.Columns["inv_grp"].Caption = "Invoice Group";
            gridView1.Columns["invoice_no"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom,"Amount");
            gridView1.Columns["amt"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridView1.Columns["inv_grp"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom, "");
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
        }
        public void ClearData(bool allControls)
        {
            if (allControls)
            {
                sleInvoice.EditValueChanged -= sleInvoice_EditValueChanged;
                sleInvoice.EditValue = null;
                sleInvoice.EditValueChanged += sleInvoice_EditValueChanged;
            }
            dtpDate.EditValue = DateTime.Today;
            gridControl1.DataSource = null;
        }
        public void SaveData()
        {
            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                string strSQL;
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    if (string.IsNullOrEmpty(gridView1.GetRowCellValue(i,"invoice_no").ToString())) continue;
                    if (dtpDate.EditValue == null)
                    {
                        strSQL = "update expbillrecord set neg_date = null ,inv_grp = '" + gridView1.GetRowCellValue(i,"inv_grp").ToString() + "' where invoice_no = '" + gridView1.GetRowCellValue(i,"invoice_no").ToString() + "'";
                        db.Execute(strSQL);
                    }
                    else
                    {
                        strSQL = "update expbillrecord set neg_date = '" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd") + "',inv_grp = '" + gridView1.GetRowCellValue(i, "inv_grp").ToString() + "'" +
                            " where invoice_no = '" + gridView1.GetRowCellValue(i, "invoice_no").ToString() + "'";
                        db.Execute(strSQL);
                    }
                }
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
            string strSQL = "select  invoice_no from expbillrecord where exp_type >= '1' and exp_type <= '6' order by invoice_no";
            DataTable dt = db.GetDataTable(strSQL);
            return dt;
        }
        private void GetInvoiceDetail(string strInvoice)
        {
            string strSQL = "select neg_date,curtype,inv_grp from expbillrecord where invoice_no = '" + sleInvoice.Text+"'";
            DataTable dt = db.GetDataTable(strSQL);
            string strInvoiceGroup="";
            string strCurrency = "";
            foreach (DataRow dr in dt.Rows)
            {
                dtpDate.EditValue = dr["neg_date"];
                strInvoiceGroup =  dr["inv_grp"]!=System.DBNull.Value?dr["inv_grp"].ToString():"";
                strCurrency = dr["curtype"].ToString();
                //            Set Rs = Dbs.Execute(SqlStr)
                //            If Not Rs.EOF Then
                //                TB.Text = IIf(IsNull(Rs!neg_date), "", Rs!neg_date)
                //                LBAmt.Caption = Rs!curtype
                //                TBgrp.Text = IIf(IsNull(Rs!inv_grp), "", Rs!inv_grp)
                //                DataToGrid
                //            Else
                //                TB.Text = ""
                //            End If

                //        Else
                //            MsgBox "Not Found This Invoice!"
                //            CBInv.Text = ""
            }
            if(string.IsNullOrEmpty(strInvoiceGroup))
                strSQL="select invoice_no,amt,inv_grp from expbillrecord where invoice_no = '"+ sleInvoice.Text + "'";
            else
                strSQL="select invoice_no,amt,inv_grp from expbillrecord where inv_grp = '"+ strInvoiceGroup +"' order by invoice_no";
            dt = db.GetDataTable(strSQL);
            gridControl1.DataSource = dt;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["inv_grp"] == System.DBNull.Value) dr["inv_grp"] = dr["invoice_no"];
            }
            gridView1.PopulateColumns();
            gridView1.Columns["invoice_no"].Caption = "Invoice No.";
            gridView1.Columns["amt"].Caption = "Amt";
            gridView1.Columns["inv_grp"].Caption = "Invoice Group";
            gridView1.Columns["amt"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView1.Columns["amt"].DisplayFormat.FormatString = "n2";
            gridView1.Columns["invoice_no"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom, "Amount");
            gridView1.Columns["amt"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:#,0.00}");
            gridView1.Columns["inv_grp"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Custom, strCurrency);

            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();




            //    SqlStr = "select inv_grp from expbillrecord where invoice_no = '" & CBInv.Text & "'"
            //    Set Rt = Dbs.Execute(SqlStr)
            //    If Not Rt.EOF Then
            //        If IsNull(Rt!inv_grp) Or Len(Rt!inv_grp) = 0 Then
            //            SqlStr = "select invoice_no,amt,inv_grp from expbillrecord where invoice_no = '" & CBInv.Text & "'"
            //        Else
            //            SqlStr = "select invoice_no,amt,inv_grp from expbillrecord where inv_grp = '" & Rt!inv_grp & "' order by invoice_no"
            //        End If
            //            Set Rn = Dbs.Execute(SqlStr)
            //            If Not Rn.EOF Then
            //                i = 0
            //                zAmt = 0
            //                Do While Not Rn.EOF
            //                    i = i + 1
            //                    FGrid.TextMatrix(i, 1) = Rn!invoice_no
            //                    FGrid.TextMatrix(i, 2) = Format(Rn!amt, "###,###,###,##0.00")
            //                    If IsNull(Rt!inv_grp) Or Len(Rt!inv_grp) = 0 Then
            //                        FGrid.TextMatrix(i, 3) = UCase(CBInv.Text)
            //                    Else
            //                        FGrid.TextMatrix(i, 3) = UCase(Rt!inv_grp)
            //                    End If
            //                    zAmt = zAmt + Rn!amt
            //                    Rn.MoveNext
            //                Loop
            //            End If
            //            LBAmt.Caption = Format(zAmt, "###,###,###,##0.00") & " " & LBAmt.Caption
            //    End If

            
        }

        private void frmAC_BankContact_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            sleInvoice.Properties.DataSource = GetInvoices();
            sleInvoice.Properties.DisplayMember = "invoice_no";
            sleInvoice.Properties.ValueMember = "invoice_no";
            sleInvoice.Properties.View.OptionsView.ColumnAutoWidth = true;
            NewData();
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
        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (!gridView1.IsEditing) gridView1.DeleteSelectedRows();
            }
            else if(e.KeyCode==Keys.Insert)
            {
                if(!gridView1.IsEditing)
                {
                    DataRow dr=dtInvoiceDetail.NewRow();
                    int pos=gridView1.GetDataSourceRowIndex(gridView1.FocusedRowHandle);
                    dtInvoiceDetail.Rows.InsertAt(dr,pos);
                    dtInvoiceDetail.AcceptChanges();
                    gridView1.FocusedRowHandle=gridView1.GetRowHandle(pos);
                }
            }
        }
        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try 
	        {	        
		        DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                if (view.FocusedColumn.FieldName == "invoice_no")
                { 
                    string strSQL="select lc_no,custname,bl_date,neg_date,amt,inv_grp from expbillrecord "+
                        "where invoice_no = '" + e.Value.ToString() + "'";
                    DataTable dt = db.GetDataTable(strSQL);
                    foreach (DataRow dr in dt.Rows)
                    { 
                        gridView1.SetRowCellValue(gridView1.FocusedRowHandle,"invoice_no",e.Value.ToString().ToUpper());
                        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "amt",  dr["amt"]);
                        if (dr["inv_grp"] == System.DBNull.Value)
                            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "inv_grp", sleInvoice.Text);
                        else
                            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "inv_grp", dr["inv_grp"]);
                    }
                }
	        }
	        catch (Exception ex)
	        {
		        MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
	        }
            
        }
    }
}


//Private Sub FGrid_AfterEdit(ByVal Row As Long, ByVal Col As Long)
//Dim j As Integer
//Dim zAmt As Double

//    If Col = 1 Then
//        OpenDatabaseSys
//        SqlStr = "select lc_no,custname,bl_date,neg_date,amt,inv_grp from expbillrecord where invoice_no = '" & FGrid.TextMatrix(Row, 1) & "'"
//        Set Rs = Dbs.Execute(SqlStr)
//        If Not Rs.EOF Then
//            FGrid.TextMatrix(Row, 1) = UCase(FGrid.TextMatrix(Row, 1))
//            FGrid.TextMatrix(Row, 2) = Format(Rs!amt, "###,###,###,##0.00")
//            FGrid.TextMatrix(Row, 3) = IIf(IsNull(Rs!inv_grp) Or Len(Rs!inv_grp) = 0, CBInv.Text, Rs!inv_grp)
//        End If
//        Rs.Close
//        CloseDB
//        zAmt = 0
//        For j = 1 To FGrid.Rows - 1
//            If Len(FGrid.TextMatrix(j, 1)) > 0 And Len(FGrid.TextMatrix(j, 2)) > 0 Then
//                zAmt = zAmt + FGrid.TextMatrix(j, 2)
//            Else
//                Exit For
//            End If
//        Next j
//        LBAmt.Caption = Format(zAmt, "###,###,###,##0.00")
//    End If
//End Sub


//End Sub





