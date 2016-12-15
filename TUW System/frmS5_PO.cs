using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using myClass;
using System.Linq;

namespace TUW_System
{
    public partial class frmS5_PO : DevExpress.XtraEditors.XtraForm
    {   
        cDatabase db=new cDatabase(Module.TUW99);
        CultureInfo clinfo=new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        RepositoryItemLookUpEdit rpUnit;
        RepositoryItemSearchLookUpEdit rpCode;
        List<ProductCode> productCodes=new List<ProductCode>();
        //DataTable dtPONo;//สำหรับ slePONo
        DataTable dtHeader,dtRemark,dtDetail;// po header remark and detail
        string strPO;//PO No ใหม่ที่ได้จากการรันใน database

        internal string PO_Remark 
        {
            set { txtRemark.Text = value; }
        }
        internal LogIn User_Login { get; set; }

        public frmS5_PO()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            if (slePONo.EditValue == null) slePONo_EditValueChanged(null, null);
            else slePONo.EditValue = null;
            //slePONo_EditValueChanged(null,null);
            //slePONo.EditValue = null;
            //if (slePONo.EditValue == null) slePONo_EditValueChanged(null, null);
            //sleSupplier.EditValue = null;
            //ClearForm(true,true,true,true);
            //popupMenu1.ShowPopup(Control.MousePosition);
        }
        public void SaveData()
        {
            if (lueCurrency.EditValue==null)//  cboCur.SelectedIndex == 0)
            {
                MessageBox.Show("โปรดเลือก Currency Symbol", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (cboPOType.SelectedIndex == 0)
            {
                MessageBox.Show("โปรดเลือก P/O Type", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (lueSection.EditValue==null)// cboSection.SelectedIndex == 0)
            {
                MessageBox.Show("โปรดเลือก Section", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //else if (cboRemark.SelectedIndex == 0)
            //{
            //    MessageBox.Show("โปรดเลือก Remark", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            else if (dtpPODate.EditValue == null)
            {
                MessageBox.Show("โปรดระบุ PO Date", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (sleSupplier.EditValue == null)
            {
                MessageBox.Show("โปรดระบุ Supplier", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (luePayterm.EditValue == null)
            {
                MessageBox.Show("โปรดระบุ Payment term", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            string strSQL="";
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            //run pono ใหม่กรณีช่อง slePO ไม่มีค่าแสดงว่าเป็นข้อมูลใหม่
            db.ConnectionOpen();
            //db_TPiCS.ConnectionOpen();
            if (slePONo.EditValue == null)
            {
                switch (txtRemark.Text)
                { 
                    case "RAW YARN":
                        strSQL = "EXEC spTUWSystem_RunNo 'SALES5_PO_YARN',''";
                        break;
                    case "DYEING FEE":
                        strSQL = "EXEC spTUWSystem_RunNo 'SALES5_PO_DYE',''";
                        break;
                    case "KNITTING FEE":
                        strSQL = "EXEC spTUWSystem_RunNo 'SALES5_PO_KNIT',";
                        if (lueSection.EditValue.ToString() == "D-0083")
                            strSQL += "'1'";
                        else if (lueSection.EditValue.ToString() == "D-0085")
                            strSQL += "'3'";
                        else if (lueSection.EditValue.ToString() == "D-0088")
                            strSQL += "'4'";
                        else if (lueSection.EditValue.ToString() == "D-0089")
                            strSQL += "'5'";
                        else if (lueSection.EditValue.ToString() == "D-0091")
                            strSQL += "'6'";
                        else if (lueSection.EditValue.ToString() == "D-0126")
                            strSQL += "'5'";
                        else if (lueSection.EditValue.ToString() == "D-0127")
                            strSQL += "'4'";
                    //lueSection.Properties.GetDisplayValueByKeyValue(lueSection.EditValue).ToString().Substring(6, 1) + "'";
                    break;
                }
                strPO = db.ExecuteFirstValue(strSQL);//สร้าง strPO มารับค่า PONo ใหม่เพราะถ้าใส่ไปที่ slePONo ตรงๆจะทำให้เกิด event editvaluechanged
            }
            else
            {
                strPO = slePONo.EditValue.ToString();
            }
            try
            {
                db.BeginTrans();
                //db_TPiCS.BeginTrans();
                //Main---------------------------------------
                strSQL = "UPDATE PO_PURCHASE SET "+
                    "PODATE='"+((DateTime)dtpPODate.EditValue).ToString("yyyy-MM-dd",dtfinfo)+"',"+
                    "IDSUP='"+sleSupplier.EditValue.ToString() +"',"+
                    "IDDEPT='"+lueSection.EditValue.ToString()+"'";
                strSQL+=(cboCustomer.Text=="")?",CUSTOMER=NULL":",CUSTOMER=N'"+cboCustomer.Text+"'";
                strSQL+=(cboSaleOrder.Text == "")?",SALESNO=NULL":",SALESNO=N'" + cboSaleOrder.Text + "'";
                strSQL+=(txtReq.Text == "")?",REQUISITIONNO=NULL":",REQUISITIONNO=N'" + txtReq.Text + "'";
                strSQL+=(dtpReqDate.EditValue == null)?",REQUISITIONDATE=NULL":",REQUISITIONDATE='"+((DateTime)dtpReqDate.EditValue).ToString("yyyy-MM-dd",dtfinfo) +"'";
                strSQL+=(txtRef.Text == "")?",REFNO=NULL":",REFNO=N'" + txtRef.Text.Replace("'","''") + "'";
                strSQL += ",CURRENCYUNIT='" + lueCurrency.EditValue.ToString() + "'";
                strSQL+=(txtTotal.Text == "")?",TOTALAMOUNT=0":",TOTALAMOUNT=" + txtTotal.Text;
                strSQL+=(txtVat.Text == "")?",VAT=0":",VAT=" + txtVat.Text;
                strSQL += ",IDPAY='" + luePayterm.EditValue.ToString() + "'";
                strSQL+=(dtpDelivery.EditValue == null)?",DELIVERYDATE=NULL":",DELIVERYDATE='" + ((DateTime)dtpDelivery.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                strSQL += ",DEPARTMENT='" + txtDept.Text + "'";
                strSQL+=",POTYPE='"+cboPOType.Text+"'";
                strSQL +=(chkCancel.Checked)?",CANCEL=1":",CANCEL=0";
                strSQL+=",EMPCODE='"+User_Login.EmployeeCode +"'";
                strSQL+=" WHERE PONO='"+strPO+"'";
                db.Execute(strSQL);
                //Detail---------------------------------------
                strSQL = "DELETE FROM PO_PURCHASEDETAIL WHERE PONO='" + strPO + "'";
                db.Execute(strSQL);
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    strSQL = "INSERT INTO PO_PURCHASEDETAIL(PONO,PRODUCTCODE,PRODUCTNAME,DESCRIPTION,QTY,IDUNIT,UNITPRICE,LINE)VALUES(" +
                        "'" + strPO + "'" +
                        ",N'" + gridView1.GetRowCellDisplayText(i, "CODE") + "'" +
                        ",N'" + gridView1.GetRowCellDisplayText(i, "NAME") + "'" +
                        ",N'" + gridView1.GetRowCellDisplayText(i, "DESCRIPTION").Replace("'","''") + "'" +
                        "," + gridView1.GetRowCellDisplayText(i, "QTY") +
                        ",N'" + gridView1.GetRowCellValue(i,"UNIT") + "'" +
                        "," + gridView1.GetRowCellDisplayText(i, "PRICE") +
                        "," + (i+1) + ")";
                    db.Execute(strSQL);
                }
                //Remark---------------------------------------
                strSQL="DELETE FROM PO_REMARK WHERE PONO='"+strPO+"'";
                db.Execute(strSQL);
                if (txtRemark1.Text.Length > 0)
                { 
                    strSQL = "INSERT INTO PO_REMARK(PONO,REMARK,LINE)VALUES('" + strPO + "',N'" + txtRemark1.Text.Replace("'","''") + "',1)";
                    db.Execute(strSQL);
                }
                if (txtRemark2.Text.Length > 0)
                {
                    strSQL = "INSERT INTO PO_REMARK(PONO,REMARK,LINE)VALUES('" + strPO + "',N'" + txtRemark2.Text.Replace("'","''") + "',2)";
                    db.Execute(strSQL);
                }
                if(txtRemark3.Text.Length> 0)
                {
                strSQL = "INSERT INTO PO_REMARK(PONO,REMARK,LINE)VALUES('" + strPO + "',N'" + txtRemark3.Text.Replace("'","''") + "',3)";
                db.Execute(strSQL);
                }

                //strSQL = "SELECT COUNT(PONO) FROM THPO WHERE PONO='" + strPO + "'";
                //if (Convert.ToInt32(db_TPiCS.ExecuteFirstValue(strSQL)) == 0)
                //{
                //    //Insert TPiCS
                //    strSQL = "INSERT INTO THPO(PONO,EDA,PODATE,SUPID,POTYPE,SECTID,DEPTID,CURRE,POTITLE,CUSTNO,SORDNO,REQNO,REQDATE,REFNO,PAYTERM" +
                //        ",TIMEDLY,TYPE,TQTY,TAMT,VAT,TVAT,GTOTAL,REMARK1,REMARK2,REMARK3,UPDBY,UPDDATE,CANCEL)VALUES('" + strPO + "',0" +
                //        ",'" + ((DateTime)dtpPODate.EditValue).ToString("yyyyMMdd", dtfinfo) + "','" + txtSupplierID.Text + "','" + cboPOType.Text + "','" + cboSection.Text + "'" +
                //        ",'" + txtDept.Text + "','" + cboCur.Text + "','" + cboRemark.Text + "','" + txtCustomer.Text + "','" + txtSaleOrder.Text + "','" + txtReq.Text + "'" +
                //        ",'" + ((DateTime)dtpReqDate.EditValue).ToString("yyyyMMdd", dtfinfo) + "','" + txtRef.Text + "','" + txtPayTerm.Text + "'" +
                //        ",'" + ((DateTime)dtpDelivery.EditValue).ToString("yyyyMMdd", dtfinfo) + "','" + optProduct.SelectedIndex + "'" +
                //        "," + Convert.ToDouble(gridView1.Columns["QTY"].SummaryItem.SummaryValue) + "," + txtTotal.Text + "," + txtVat.Text + "," + txtVatTotal.Text + "," + txtGrand.Text +
                //        ",N'" + txtRemark1.Text + "',N'" + txtRemark2.Text + "',N'" + txtRemark3.Text + "','" + Environment.MachineName + "','" + DateTime.Today.ToString("yyyyMMdd", dtfinfo) + "'";
                //    strSQL += (chkCancel.Checked) ? ",'1'" : ",'0'";
                //    strSQL += ")";
                //    db_TPiCS.Execute(strSQL);
                //    //Insert PurchaseOrder
                //    //strSQL = "INSERT INTO FABRICNORMALPO(PONO,PODATE,SUPPLIERID,DEPARTMENT,DEPARTMENTID,TYPEID,CUSTOMERNAME,SALENO,REQNO,REQDATE" +
                //    //    ",REFNO,REM1,REM2,REM3,TOTALAMOUNT,VAT,MONEYTYPE,PAYTERM,DATEDELIVERY,DISCOUNT,STATUS,POTYPE,REMARK,REVISED,REVISEDATE)VALUES(" +
                //    //    "'" + strPO + "','" + ((DateTime)dtpPODate.EditValue).ToString("dd/MM/yy", dtfinfo) + "','" + txtSupplierID.Text + "','" + txtDept.Text + "'" +
                //    //    ",'" + cboSection.Text + "','" + CheckTypeID(cboPOType.Text) + "','" + txtCustomer.Text + "','" + txtSaleOrder.Text + "','" + txtReq.Text + "'" +
                //    //    ",'" + ((DateTime)dtpReqDate.EditValue).ToString("dd/MM/yy", dtfinfo) + "','" + txtRef.Text + "',N'" + txtRemark1.Text + "',N'" + txtRemark2.Text + "'" +
                //    //    ",N'" + txtRemark3.Text + "'," + txtTotal.Text + "," + txtVat.Text + "," + CheckMoneyType(cboCur.Text) + ",'" + txtPayTerm.Text + "'" +
                //    //    ",'" + ((DateTime)dtpDelivery.EditValue).ToString("dd/MM/yy", dtfinfo) + "',0,0," + optProduct.SelectedIndex + ",'" + cboRemark.Text + "',0,'')";
                //    //db.Execute(strSQL);
                //}
                //else
                //{
                //    //Update TPiCS
                //    strSQL = "UPDATE THPO SET PODATE='" + ((DateTime)dtpPODate.EditValue).ToString("yyyyMMdd", dtfinfo) + "'" +
                //        ",SUPID='" + txtSupplierID.Text + "',POTYPE='" + cboPOType.Text + "',SECTID='" + cboSection.Text + "'" +
                //        ",DEPTID='" + txtDept.Text + "',CURRE='" + cboCur.Text + "',POTITLE='" + cboRemark.Text + "'" +
                //        ",CUSTNO='" + txtCustomer.Text + "',SORDNO='" + txtSaleOrder.Text + "',REQNO='" + txtReq.Text + "'" +
                //        ",REQDATE='" + ((DateTime)dtpReqDate.EditValue).ToString("yyyyMMdd", dtfinfo) + "',REFNO='" + txtRef.Text + "'" +
                //        ",PAYTERM='" + txtPayTerm.Text + "',TIMEDLY='" + ((DateTime)dtpDelivery.EditValue).ToString("yyyyMMdd", dtfinfo) + "'" +
                //        ",TYPE='" + optProduct.SelectedIndex + "',TQTY=" + Convert.ToDouble(gridView1.Columns["QTY"].SummaryItem.SummaryValue) +
                //        ",TAMT=" + txtTotal.Text + ",VAT=" + txtVat.Text + ",TVAT=" + txtVatTotal.Text + ",GTOTAL=" + txtGrand.Text +
                //        ",REMARK1=N'" + txtRemark1.Text + "',REMARK2=N'" + txtRemark2.Text + "',REMARK3=N'" + txtRemark3.Text + "' " +
                //        ",UPDBY='" + Environment.MachineName + "',UPDDATE='" + DateTime.Today.ToString("yyyyMMdd", dtfinfo) + "'";
                //    strSQL += (chkCancel.Checked) ? ",CANCEL='1'" : ",CANCEL='0'";
                //    strSQL += " WHERE PONO='" + strPO + "'";
                //    db_TPiCS.Execute(strSQL);
                //    //Update PurchaseOrder
                //    //strSQL = "UPDATE FABRICNORMALPO SET PODATE='" + ((DateTime)dtpPODate.EditValue).ToString("dd/MM/yy", dtfinfo) + "'" +
                //    //    ",SUPPLIERID='" + txtSupplierID.Text + "',DEPARTMENT='" + txtDept.Text + "',DEPARTMENTID='" + cboSection.Text + "'" +
                //    //    ",TYPEID='" + CheckTypeID(cboPOType.Text) + "',CUSTOMERNAME='" + txtCustomer.Text + "',SALENO='" + txtSaleOrder.Text + "'" +
                //    //    ",REQNO='" + txtReq.Text + "',REQDATE='" + ((DateTime)dtpReqDate.EditValue).ToString("dd/MM/yy", dtfinfo) + "'" +
                //    //    ",REFNO='" + txtRef.Text + "',REM1=N'" + txtRemark1.Text + "',REM2=N'" + txtRemark2.Text + "',REM3=N'" + txtRemark3.Text + "'" +
                //    //    ",TOTALAMOUNT=" + txtTotal.Text + ",VAT=" + txtVat.Text + ",MONEYTYPE=" + CheckMoneyType(cboCur.Text) +
                //    //    ",PAYTERM='" + txtPayTerm.Text + "',DATEDELIVERY='" + ((DateTime)dtpDelivery.EditValue).ToString("dd/MM/yy", dtfinfo) + "'" +
                //    //    ",DISCOUNT=0,STATUS=0,POTYPE=" + optProduct.SelectedIndex + ",REMARK='" + cboRemark.Text + "'" +
                //    //    ",REVISED=0,REVISEDATE='' WHERE PONO='" + strPO + "'";
                //    //db.Execute(strSQL);
                //}
                ////PO DETAIL TPICS
                //strSQL = "DELETE FROM TDPO WHERE PONO='" + strPO + "'";
                //db_TPiCS.Execute(strSQL);
                //for (int i = 0; i < gridView1.DataRowCount; i++)
                //{
                //    strSQL = "INSERT INTO TDPO(PONO,EDA,ROWNO,TPICS_ORDER,CODE,BARCODE,DESCR,QTY,UNIT,UPRC,AMT)VALUES(" +
                //        "'" + strPO + "',0," + i + ",'" + gridView1.GetRowCellDisplayText(i, "TPICS_ORDER") + "'" +
                //        ",'" + gridView1.GetRowCellDisplayText(i, "CODE") + "','" + gridView1.GetRowCellDisplayText(i, "BARCODE") + "'" +
                //        ",N'" + gridView1.GetRowCellDisplayText(i, "DESCRIPTION") + "'," + gridView1.GetRowCellDisplayText(i, "QTY") +
                //        ",'" + gridView1.GetRowCellDisplayText(i, "UNIT") + "'," + gridView1.GetRowCellDisplayText(i, "PRICE") +
                //        "," + gridView1.GetRowCellDisplayText(i, "AMOUNT") + ")";
                //    db_TPiCS.Execute(strSQL);
                //}
                //PO DETAIL PURCHASEORDER
                //strSQL = "DELETE FROM FABRICNORMALPODETAIL WHERE PONO='" + strPO + "'";
                //db.Execute(strSQL);
                //int intRow = -1;
                //for (int i = 0; i < gridView1.DataRowCount; i++)
                //{
                //    intRow += 1;
                //    strSQL = "INSERT INTO FABRICNORMALPODETAIL(PONO,LINE,PRODUCTCODE,DESCRIPTION,QTY,UNIT,UNITPRICE,TPICS_ORDER)VALUES('" + strPO + "'" +
                //        "," + intRow;
                //    if (cboRemark.Text == "RAW YARN")
                //    {
                //        strSQL = strSQL + ",'" + Convert.ToInt32(gridView1.GetRowCellDisplayText(i, "CODE")) + "'";
                //    }
                //    else
                //    {
                //        strSQL = strSQL + ",'" + gridView1.GetRowCellDisplayText(i, "CODE") + "'";
                //    }
                //    strSQL = strSQL + ",N'" + gridView1.GetRowCellDisplayText(i, "BARCODE") + "'" +
                //        "," + gridView1.GetRowCellDisplayText(i, "QTY") + ",'" + gridView1.GetRowCellDisplayText(i, "UNIT") + "'" +
                //        "," + gridView1.GetRowCellDisplayText(i, "PRICE") + ",'" + gridView1.GetRowCellDisplayText(i, "TPICS_ORDER") + "')";
                //    db.Execute(strSQL);
                //    intRow += 1;
                //    strSQL = "INSERT INTO FABRICNORMALPODETAIL(PONO,LINE,DESCRIPTION)VALUES('" + strPO + "'," + intRow + ",'" + gridView1.GetRowCellDisplayText(i, "DESCRIPTION") + "')";
                //    db.Execute(strSQL);
                //}
                //Update PONO in XSLIP
                //for (int i = 0; i < gridView1.DataRowCount; i++)
                //{
                //    strSQL = "UPDATE XSLIP SET CONT='SECTION " + cboSection.Text + "',PO='" + strPO + "',VENDOR2='" + sleSupplier.EditValue + "' " +
                //        "WHERE PORDER='" + gridView1.GetRowCellDisplayText(i, "TPICS_ORDER") + "'";
                //    db_TPiCS.Execute(strSQL);
                //}
                db.CommitTrans();
                //db_TPiCS.CommitTrans();
                MessageBox.Show("Save complete...", "Purchase Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GetPO(txtRemark.Text);
                slePONo.EditValue = strPO;
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                //db_TPiCS.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            //db_TPiCS.ConnectionClose();
        }
        public void PrintPreview()
        {
            try
            {
                cCrystalReport crpPO = new cCrystalReport(Application.StartupPath + @"\Report\S5_PO.rpt");
                if (crpPO.SetPrinter() == false) { return; }
                crpPO.ReportTitle = slePONo.EditValue.ToString();
                for (int i = 1; i <= crpPO.ReportCopy; i++)
                {
                    crpPO.ClearParameters();
                    crpPO.SetParameter("Copy", i.ToString());
                    if (chkRevise.Checked)
                    {
                        crpPO.SetParameter("Revise", "REVISE");
                    }
                    else
                    {
                        crpPO.SetParameter("Revise", "");
                    }
                    string fmlText = "{PO_PURCHASE.PONO}='" + slePONo.EditValue.ToString() + "'";
                    crpPO.PrintReport(fmlText, false);
                }
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
                cCrystalReport crpPO = new cCrystalReport(Application.StartupPath + @"\Report\S5_PO.rpt");
                int intTemp = 0;
                string strInput = "1";
                try
                {
                    if (Module.InputBox("จำนวนสำเนา", "คุณต้องการพิมพ์กี่ copy", ref strInput) == DialogResult.OK)
                    {
                        intTemp = int.Parse(strInput);
                    }
                }
                catch
                {
                    intTemp = 1;
                }
                if (crpPO.SetPrinter() == false) { return; }
                crpPO.ReportTitle = slePONo.EditValue.ToString();
                for (int i = 1; i <= intTemp; i++)
                {
                    crpPO.ClearParameters();
                    crpPO.SetParameter("Copy", i.ToString());
                    if (chkRevise.Checked)
                    {
                        crpPO.SetParameter("Revise", "REVISE");
                    }
                    else
                    {
                        crpPO.SetParameter("Revise", "");
                    }
                    string fmlText = "{PO_PURCHASE.PONO}='" + slePONo.EditValue.ToString() + "'";
                    crpPO.PrintReport(fmlText, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void RefreshData()
        {
            GetSupplier();
            rpCode = GetProductCode(txtRemark.Text);
        }
        //public void AddRowsFromPONewGridView(ref DevExpress.XtraGrid.Views.Grid.GridView gvPONew)
        //{ 
        //    for(int i=0;i<gvPONew.RowCount;i++)
        //    {
        //        if(Convert.ToBoolean(gvPONew.GetRowCellValue(i, "Check")) == false){continue;}
        //        if(CheckDuplicatePORDER(gvPONew.GetRowCellDisplayText(i, "PORDER"))){continue;}
        //        gridView1.AddNewRow();
        //        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "TPICS_ORDER", gvPONew.GetRowCellDisplayText(i, "PORDER"));
        //        if(cboRemark.Text == "DYEING FEE")
        //        {
        //            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "CODE", Module.Right(gvPONew.GetRowCellDisplayText(i, "CODE"), gvPONew.GetRowCellDisplayText(i, "CODE").Length - 9)); //,RIGHT(XSLIP.CODE,LEN(XSLIP.CODE)-9) AS CODE
        //        }
        //        else
        //        {
        //            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "CODE", gvPONew.GetRowCellDisplayText(i, "CODE"));
        //        }
        //        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "BARCODE", gvPONew.GetRowCellDisplayText(i, "BARCODE"));
        //        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "DESCRIPTION", gvPONew.GetRowCellDisplayText(i, "MIXING"));
        //        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "QTY", gvPONew.GetRowCellDisplayText(i, "QTY"));
        //        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "UNIT", gvPONew.GetRowCellDisplayText(i, "UNIT"));
        //        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "PRICE", gvPONew.GetRowCellDisplayText(i, "PRICE"));
        //        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "AMOUNT", gvPONew.GetRowCellDisplayText(i, "AMOUNT"));
        //        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "BUMO", gvPONew.GetRowCellDisplayText(i, "BUMO"));
        //        gridView1.UpdateCurrentRow();
        //    }
        //    if(cboRemark.Text == "RAW YARN")
        //    {
        //        LoadSupplierName_Detail(gvPONew.GetRowCellDisplayText(gvPONew.FocusedRowHandle, "BUMO") );
        //    }
        //}
        private bool CheckDuplicatePORDER(string strPORDER)
        {
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                if (Equals(strPORDER, gridView1.GetRowCellDisplayText(i, "TPICS_ORDER"))) { return true; }
            }
            return false;
        }
        //private void ClearForm(bool clearPrimaryKey_PO,bool clearPO,bool clearPrimaryKey_Supplier,bool clearSupplier)
        //{
        //    //======Clear supplier section====================
        //    if (clearPrimaryKey_Supplier)
        //    {
        //        sleSupplier.EditValue = null;
        //    }
        //    if(clearSupplier)
        //    {
        //        txtSupplierID.Text = "";
        //        txtAD1.Text = "";
        //        txtAD2.Text = "";
        //        txtZip.Text = "";
        //        txtCountry.Text = "";
        //        txtTel.Text = "";
        //        txtFax.Text = "";
        //    }
        //    //===========Clear PO==============================
        //    if (clearPrimaryKey_PO)
        //    {
        //        slePONo.EditValue=null;
        //    }
        //    if(clearPO)
        //    { 
        //        txtDept.Text = "FABRIC CONTROL";
        //        txtCustomer.Text = "";
        //        txtSaleOrder.Text = "";
        //        dtpPODate.EditValue = DateTime.Today;
        //        txtReq.Text = "";
        //        dtpReqDate.EditValue = "";
        //        txtRef.Text = "";
        //        txtPayTerm.Text = "";
        //        luePayterm.EditValue = null;
        //        dtpDelivery.EditValue = DateTime.Today;
        //        txtRemark1.Text = "";
        //        txtRemark2.Text = "";
        //        txtRemark3.Text = "";
        //        txtTotal.Text = "";
        //        txtVat.Text = "7";
        //        txtVatTotal.Text = "";
        //        txtGrand.Text = "";
        //        cboCur.SelectedIndex = 0;
        //        lueCurrency.EditValue = null;
        //        cboPOType.SelectedIndex = 0;
        //        cboSection.SelectedIndex = 0;
        //        optProduct.SelectedIndex = 0;
        //        cboRemark.SelectedIndex = 0;
        //        chkRevise.Checked = false;
        //        chkCancel.Checked = false;

        //        dtDetail=new DataTable();
        //        dtDetail.BeginInit();
        //        dtDetail.Columns.Add("CODE", typeof(string));
        //        dtDetail.Columns.Add("NAME", typeof(string));
        //        dtDetail.Columns.Add("DESCRIPTION", typeof(string));
        //        dtDetail.Columns.Add("QTY", typeof(decimal));
        //        dtDetail.Columns.Add("UNIT", typeof(string));
        //        dtDetail.Columns.Add("PRICE", typeof(decimal));
        //        dtDetail.Columns.Add("AMOUNT", typeof(decimal));
        //        dtDetail.EndInit();
        //        gridControl1.DataSource = dtDetail;
        //        gridView1.Columns["QTY"].SummaryItem.FieldName = "QTY";
        //        gridView1.Columns["QTY"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
        //        gridView1.Columns["AMOUNT"].SummaryItem.FieldName = "AMOUNT";
        //        gridView1.Columns["AMOUNT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
        //        gridView1.OptionsView.EnableAppearanceEvenRow = true;
        //        gridView1.OptionsView.EnableAppearanceOddRow = true;
        //        //gridView1.OptionsView.RowAutoHeight = true;
        //        gridView1.OptionsView.ColumnAutoWidth = true;
        //        gridView1.BestFitColumns();
        //    }
                   
        //}
        //private void LoadPONo()
        //{
        //    //string strSQL ="SELECT TOP 1000 PONO FROM THPO ORDER BY PODATE DESC" ;
        //    string strSQL = "SELECT PONO FROM THPO WHERE LEFT(PODATE,4)IN('"+(DateTime.Today.Year-1).ToString()+"','"+DateTime.Today.Year+"')";
        //    dtPONo = db_TPiCS.GetDataTable(strSQL);
        //    slePONo.Properties.DataSource = dtPONo;
        //    slePONo.Properties.DisplayMember = "PONO";
        //    slePONo.Properties.ValueMember = "PONO";
        //}
        private void LoadSupplierName()
        {
            string strSQL= "SELECT BUMO,NAME FROM XSECT WHERE DESNAME IN ('Order1','Outsource1') ORDER BY NAME";
            //DataTable dt=db_TPiCS.GetDataTable(strSQL);
            //sleSupplier.Properties.DataSource=dt;
            sleSupplier.Properties.DisplayMember="NAME";
            sleSupplier.Properties.ValueMember="BUMO";
        }
        private void LoadSupplierName_Detail(string strID="",string strSupplier="")
        {
            string strSQL="SELECT BUMO,NAME,ADR1,ADR2,ZIP,COUNTRY,MAIL,TEL,FAX,PAYTERM,CURRE,HITO FROM XSECT ";
            if(strID == "")
            {
                strSQL += "WHERE NAME='" + strSupplier + "'";
            }
            else
            {
                strSQL += "WHERE BUMO='" + strID + "'";
            }
            //DataTable dt = db_TPiCS.GetDataTable(strSQL);
            //foreach(DataRow dr in dt.Rows)
            //{
            //    txtSupplierID.Text = dr["BUMO"].ToString();
            //    //cboSupplier.Text = dr["NAME"].ToString();
            //    txtAD1.Text = dr["ADR1"].ToString();
            //    txtAD2.Text = dr["ADR2"].ToString();
            //    txtZip.Text = dr["ZIP"].ToString();
            //    txtCountry.Text = dr["COUNTRY"].ToString();
            //    txtTel.Text = dr["TEL"].ToString();
            //    txtFax.Text = dr["FAX"].ToString();
            //    txtPayTerm.Text = dr["PAYTERM"].ToString();
            //}
        }
        //private void DisplayData(string strPO)
        //{
        //    string strCurr = "";
        //    string strSQL  = "SELECT * FROM THPO WHERE PONO='" + strPO +"'";
        //    DataTable dt  = db_TPiCS.GetDataTable(strSQL);
        //    foreach(DataRow dr in dt.Rows)
        //    {
        //        txtDept.Text = dr["DeptID"].ToString();
        //        txtCustomer.Text = dr["CustNo"].ToString();
        //        txtSaleOrder.Text = dr["SOrdNo"].ToString();
        //        dtpPODate.EditValue =new DateTime(Convert.ToInt16(dr["PODate"].ToString().Substring(0, 4)),Convert.ToInt16(dr["PODate"].ToString().Substring(4,2)),Convert.ToInt16(dr["PODate"].ToString().Substring(6,2)));
        //        txtReq.Text = dr["ReqNo"].ToString();
        //        dtpReqDate.EditValue = new DateTime(Convert.ToInt16(dr["ReqDate"].ToString().Substring(0, 4)),Convert.ToInt16(dr["ReqDate"].ToString().Substring(4,2)),Convert.ToInt16(dr["ReqDate"].ToString().Substring(6,2)));
        //        txtRef.Text = dr["RefNo"].ToString();
        //        txtPayTerm.Text = dr["PayTerm"].ToString();
        //        dtpDelivery.EditValue = new DateTime(Convert.ToInt16(dr["TimeDly"].ToString().Substring(0, 4)),Convert.ToInt16(dr["TimeDly"].ToString().Substring(4,2)),Convert.ToInt16(dr["TimeDly"].ToString().Substring(6,2)));
        //        txtRemark1.Text = dr["Remark1"].ToString();
        //        txtRemark2.Text = dr["Remark2"].ToString();
        //        txtRemark3.Text = dr["Remark3"].ToString();
        //        txtTotal.Text = dr["TAmt"].ToString();
        //        txtVat.Text = dr["Vat"].ToString();
        //        txtVatTotal.Text = dr["TVat"].ToString();
        //        txtGrand.Text = dr["GTotal"].ToString();
        //        strCurr = dr["CURRE"].ToString();
        //        cboPOType.SelectedIndex = cboPOType.Properties.Items.IndexOf(dr["POType"]);
        //        cboSection.SelectedIndex = cboSection.Properties.Items.IndexOf(dr["SectID"]);
        //        cboRemark.SelectedIndex = cboRemark.Properties.Items.IndexOf(dr["POTitle"]);
        //        optProduct.SelectedIndex =Convert.ToInt16(dr["Type"]);
        //        if(dr["Cancel"].ToString()== "1")
        //        {
        //            chkCancel.Checked = true;
        //        }
        //        else
        //        {
        //            chkCancel.Checked = false;
        //        }
        //        sleSupplier.EditValue = dr["SUPID"].ToString();
        //        //LoadSupplierName_Detail(strID:dr["SUPID"].ToString());
        //    }
        //    gridControl1.DataSource = DataToGrid(strPO);
        //    cboCur.SelectedIndex = cboCur.Properties.Items.IndexOf(strCurr);
        //}
        //private DataTable DataToGrid(string strPO )
        //{
        //    string strSQL = "SELECT * FROM TDPO WHERE PONO='" + strPO + "' ORDER BY ROWNO";
        //    dtGridView = db_TPiCS.GetDataTable(strSQL);
        //    dtGridView.Columns["Code"].ColumnName = "CODE";
        //    dtGridView.Columns["Barcode"].ColumnName = "BARCODE";
        //    dtGridView.Columns["Descr"].ColumnName = "DESCRIPTION";
        //    dtGridView.Columns["Qty"].ColumnName = "QTY";
        //    dtGridView.Columns["Unit"].ColumnName = "UNIT";
        //    dtGridView.Columns["UPrc"].ColumnName = "PRICE";
        //    dtGridView.Columns["Amt"].ColumnName = "AMOUNT";
        //    return dtGridView;
        //}
        private void CalculateTotal()
        {
            try
            {
                double dblTotal = 0.0;
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    dblTotal += Convert.ToDouble(gridView1.GetRowCellValue(i, "AMOUNT"));
                }
                txtTotal.Text = dblTotal.ToString();
                txtVatTotal.Text = (dblTotal * (Convert.ToDouble(txtVat.Text) / 100)).ToString();
                txtGrand.Text = (dblTotal + (dblTotal * (Convert.ToDouble(txtVat.Text) / 100))).ToString();
            }
            catch
            {

            }
        }
        private void LoadfrmTS5_PONew()
        {
            //foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            //{
            //    if (frmActive.Name == "frmTS5_PONew")
            //    {
            //        frmActive.Activate();
            //        return;
            //    }
            //}
            using(frmTS5_PONew frm1 = new frmTS5_PONew())
            {
                //frm1.cboRemark = cboRemark.Text;
                //frm1.MdiParent = mdiMain;
                frm1.ShowDialog();
            }
            
        }
        private void LoadfrmTS5_POSupplier()
        {
            //foreach (DevExpress.XtraEditors.XtraForm frmActive in this.MdiChildren)
            //{
            //    if (frmActive.Name == "frmTS5_POSupplier")
            //    {
            //        frmActive.Activate();
            //        return;
            //    }
            //}
            using (frmS5_POSupplier frm2 = new frmS5_POSupplier())
            {
                frm2.ShowDialog();
            }
            //frmTS5_POSupplier frm2 = new frmTS5_POSupplier();
            //frm2.MdiParent = this;
            //frm2.WindowState = FormWindowState.Maximized;
            //frm2.Show();
        }
        private void PostEditValueChanged(object sender, EventArgs e)
        {
            gridView1.PostEditor();
        }
        #region "Initialize form"
        private void GetCustomer()
        {
            DataTable dt = db.GetDataTable("SELECT DISTINCT CUSTOMER FROM PO_PURCHASE");
            foreach (DataRow dr in dt.Rows)
            {
                cboCustomer.Properties.Items.Add(dr["CUSTOMER"]);
            }
        }
        private void GetSaleOrder()
        { 
            DataTable dt=db.GetDataTable("SELECT DISTINCT SALESNO FROM PO_PURCHASE");
            foreach (DataRow dr in dt.Rows)
            {
                cboSaleOrder.Properties.Items.Add(dr["SALESNO"]);
            }
        }
        private void GetCurrency()
        {
            string strSQL = "SELECT IDCUR,CURCODE FROM PO_CURRENCY";
            DataTable dt = db.GetDataTable(strSQL);
            lueCurrency.Properties.DataSource = dt;
            lueCurrency.Properties.PopulateColumns();
            lueCurrency.Properties.DisplayMember = "CURCODE";
            lueCurrency.Properties.ValueMember = "IDCUR";
            foreach (LookUpColumnInfo col in lueCurrency.Properties.Columns)
            {
                if (col.FieldName == "IDCUR") { col.Visible = false; }            
            }
        }
        private void GetSection()
        {
            string strSQL = "SELECT IDDEPT,NAME FROM PO_DEPARTMENT WHERE IDDEPT IN "+
                "('D-0083','D-0085','D-0088','D-0089','D-0091','D-0126','D-0127')";
            DataTable dt = db.GetDataTable(strSQL);
            lueSection.Properties.DataSource = dt;
            lueSection.Properties.PopulateColumns();
            lueSection.Properties.DisplayMember = "NAME";
            lueSection.Properties.ValueMember = "IDDEPT";
            foreach (LookUpColumnInfo col in lueSection.Properties.Columns)
            {
                if (col.FieldName == "IDDEPT") col.Visible = false;
            }
        }
        private void GetPaymentTerm()
        {
            string strSQL = "SELECT IDPAY,PAYMENT FROM PO_PAYMENT";
            DataTable dt = db.GetDataTable(strSQL);
            luePayterm.Properties.DataSource = dt;
            luePayterm.Properties.PopulateColumns();
            luePayterm.Properties.DisplayMember = "PAYMENT";
            luePayterm.Properties.ValueMember = "IDPAY";
            foreach (LookUpColumnInfo col in luePayterm.Properties.Columns)
            {
                if (col.FieldName == "IDPAY") { col.Visible = false; }
            }
        }
        private void GetSupplier()
        {
            string strSQL = "SELECT IDSUP,NAME FROM PO_SUPPLIER";
            DataTable dt = db.GetDataTable(strSQL);
            sleSupplier.Properties.DataSource = dt;
            sleSupplier.Properties.PopulateViewColumns();
            sleSupplier.Properties.DisplayMember = "NAME";
            sleSupplier.Properties.ValueMember = "IDSUP";
            sleSupplier.Properties.View.Columns["IDSUP"].Visible = false;
        }
        private void GetSupplierDetail(string supplierID)
        {
            string strSQL = "SELECT IDSUP,ADDRESS1,ADDRESS2,ZIP,COUNTRY,TELEPHONE,FAX,IDPAYMENT "+
                "FROM PO_SUPPLIER WHERE IDSUP='"+supplierID+"'";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                txtSupplierID.Text =dr["IDSUP"].ToString();
                txtAD1.Text = dr["ADDRESS1"].ToString();
                txtAD2.Text = dr["ADDRESS2"].ToString();
                txtZip.Text = dr["ZIP"].ToString();
                txtCountry.Text = dr["COUNTRY"].ToString();
                txtTel.Text = dr["TELEPHONE"].ToString();
                txtFax.Text = dr["FAX"].ToString();
                luePayterm.EditValue = dr["IDPAYMENT"].ToString();
            }
        }
        private void GetPO(string strRemark)
        {
            string strSQL="";
            switch(strRemark)
            {
                case "RAW YARN":
                    strSQL= "SELECT PONO FROM PO_PURCHASE WHERE PONO LIKE 'FX%'";
                    break;
                case "KNITTING FEE":
                    strSQL = "SELECT PONO FROM PO_PURCHASE WHERE PONO LIKE 'FB.S%'";
                    break;
                case "DYEING FEE":
                    strSQL = "SELECT PONO FROM PO_PURCHASE WHERE PONO LIKE 'FD%'";
                    break;  
            }
            DataTable dt = db.GetDataTable(strSQL);
            slePONo.Properties.DataSource = dt;
            slePONo.Properties.DisplayMember = "PONO";
            slePONo.Properties.ValueMember = "PONO";
        }
        private void GetPODetail(string pono)
        {
            string strSQL = "SELECT PONO,PODATE,IDSUP,CUSTOMER,SALESNO,REQUISITIONNO,REQUISITIONDATE,REFNO,CURRENCYUNIT "
                + ",TOTALAMOUNT,VAT,IDPAY,DELIVERYDATE,DEPARTMENT,POTYPE,CANCEL,IDDEPT FROM PO_PURCHASE WHERE PONO='" + pono + "'";
            dtHeader = db.GetDataTable(strSQL);
            foreach (DataRow dr in dtHeader.Rows)
            {
                sleSupplier.EditValue = dr["IDSUP"];
                cboCustomer.EditValue = dr["CUSTOMER"];
                cboSaleOrder.EditValue = dr["SALESNO"];
                dtpPODate.EditValue = (DateTime)dr["PODATE"];                
                txtReq.EditValue = dr["REQUISITIONNO"];
                dtpReqDate.EditValue =(dr["REQUISITIONDATE"]==System.DBNull.Value)?(DateTime?)null:(DateTime)dr["REQUISITIONDATE"];
                txtRef.EditValue = dr["REFNO"];
                luePayterm.EditValue = dr["IDPAY"];
                dtpDelivery.EditValue =(dr["DELIVERYDATE"]==System.DBNull.Value)?(DateTime?)null:(DateTime)dr["DELIVERYDATE"];
                txtTotal.EditValue = dr["TOTALAMOUNT"];
                txtVat.EditValue = dr["VAT"];
                lueCurrency.EditValue = dr["CURRENCYUNIT"];
                cboPOType.EditValue = dr["POTYPE"];
                lueSection.EditValue = dr["IDDEPT"];
                chkCancel.Checked = (bool)dr["CANCEL"];
                strSQL = "SELECT LINE,REMARK FROM PO_REMARK WHERE PONO='" + pono + "' ORDER BY LINE";
                dtRemark = db.GetDataTable(strSQL);
                foreach (DataRow drRemark in dtRemark.Rows)
                {
                    switch (Convert.ToInt16(drRemark["LINE"]))
                    { 
                        case 1:
                            txtRemark1.EditValue = drRemark["REMARK"];
                            break;
                        case 2:
                            txtRemark2.EditValue = drRemark["REMARK"];
                            break;
                        case 3:
                            txtRemark3.EditValue = drRemark["REMARK"];
                            break;
                    }
                }
                strSQL = "SELECT PRODUCTCODE AS CODE,PRODUCTNAME AS NAME,DESCRIPTION,QTY,IDUNIT AS UNIT,UNITPRICE AS PRICE"+
                    ",CONVERT(DECIMAL(19,2),QTY*UNITPRICE) AS AMOUNT " +
                    " FROM PO_PURCHASEDETAIL WHERE PONO='"+pono+"' ORDER BY LINE";
                dtDetail = db.GetDataTable(strSQL);
                gridControl1.DataSource = dtDetail;
                gridView1.Columns["UNIT"].ColumnEdit = rpUnit;
                RepositoryItemMemoExEdit memo = gridControl1.RepositoryItems.Add("MemoExEdit") as RepositoryItemMemoExEdit;
                gridView1.Columns["DESCRIPTION"].ColumnEdit = memo;
                gridView1.OptionsView.EnableAppearanceEvenRow = true;
                gridView1.OptionsView.EnableAppearanceOddRow = true;
            }

        }
        private RepositoryItemLookUpEdit GetUnit()
        {
            string strSQL = "SELECT IDUNIT,UNIT FROM PO_UNIT WHERE IDUNIT IN ('U-0020','U-0035','U-0124')";
            DataTable dt = db.GetDataTable(strSQL);
            RepositoryItemLookUpEdit rp = new RepositoryItemLookUpEdit();
            rp.DataSource = dt;
            rp.PopulateColumns();
            rp.DisplayMember = "UNIT";
            rp.ValueMember = "IDUNIT";
            foreach (LookUpColumnInfo col in rp.Columns)
            {
                if (col.FieldName == "IDUNIT") { col.Visible = false; }
            }
            return rp;
        }
        private RepositoryItemSearchLookUpEdit GetProductCode(string strRemark)
        {
            string strSQL = "";
            switch (strRemark)
            { 
                case "RAW YARN":
                    strSQL = "SELECT ID AS CODE,CODE AS NAME,MIXED AS DESCRIPTION FROM YARNCODE WHERE SYSDELETE=0";
                    break;
                case "KNITTING FEE":
                    strSQL = "SELECT REPLACE(STR(A.ID,5),SPACE(1),'0')+'KNT' AS CODE,A.CODE AS NAME," +
                        " '' AS DESCRIPTION " +
                        " FROM GREYFABRIC A	INNER JOIN FABRICRAWYARN B ON A.CODE=B.CODE WHERE A.DELETEFLAG=0";
                    break;
                case "DYEING FEE":
                    strSQL = "SELECT REPLACE(STR(ID,5),SPACE(1),'0') AS CODE,CODE AS NAME,'' AS DESCRIPTION FROM GREYFABRIC WHERE DELETEFLAG=0";
                    break;
            }
            
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                productCodes.Add(new ProductCode 
                {
                    Code=dr["CODE"].ToString(),
                    Name=dr["NAME"].ToString(),
                    Description=dr["DESCRIPTION"].ToString()
                });
            }
            RepositoryItemSearchLookUpEdit rp = new RepositoryItemSearchLookUpEdit();
            rp.DataSource = dt;
            rp.PopulateViewColumns();
            rp.DisplayMember = "CODE";
            rp.ValueMember = "CODE";
            rp.PopupSizeable = true;
            rp.View.Columns["DESCRIPTION"].Visible = false;
            rp.EditValueChanged += this.PostEditValueChanged;
            return rp;
        }
        private string GetYarnByFabric(string strFabric)
        {
            string strSQL = "SELECT dbo.udf_JoinString('PO SALES5',',','"+ strFabric +"') AS DESCRIPTION";
            return db.ExecuteFirstValue(strSQL);
        }
        #endregion
        #region "For FabricNormalPO"
        private string CheckTypeID(string strType)
        {
            switch(strType)
            {
                case "Local":
                    return "3";
                    //break;
                case "Import":
                    return "4";
                    //break;
                case "Sub Contract":
                    return "5";
                    //break;
                default:
                    return "";
                    //break;
            }
        }
        private Int16 CheckMoneyType(string strType)
        {
            switch(strType)
            {
                case "Baht":
                    return 0;
                    //break;
                case "EUR":
                    return -1;
                    //break;
                case "GBP":
                    return 5;
                    //break;
                case "HKD":
                    return 4;
                    //break;
                case "SGD":
                    return -1;
                    //break;
                case "USD":
                    return 1;
                    //break;
                case "YEN":
                    return 2;
                    //break;
                default:
                    return -1;
                    //break;
            }
        }
        #endregion
        //private void SaveData(string strPO)
        //{
        //    string strSQL = "SELECT COUNT(PONO) FROM THPO WHERE PONO='" + strPO + "'";
        //    if(Convert.ToInt32(db_TPiCS.ExecuteFirstValue(strSQL))== 0)
        //    {
        //        //Insert TPiCS
        //        strSQL = "INSERT INTO THPO(PONO,EDA,PODATE,SUPID,POTYPE,SECTID,DEPTID,CURRE,POTITLE,CUSTNO,SORDNO,REQNO,REQDATE,REFNO,PAYTERM"+
        //            ",TIMEDLY,TYPE,TQTY,TAMT,VAT,TVAT,GTOTAL,REMARK1,REMARK2,REMARK3,UPDBY,UPDDATE,CANCEL)VALUES('" + strPO + "',0" +
        //            ",'" + ((DateTime)dtpPODate.EditValue).ToString("yyyyMMdd",dtfinfo) + "','" + txtSupplierID.Text + "','" + cboPOType.Text + "','" + cboSection.Text + "'" +
        //            ",'" + txtDept.Text + "','" + cboCur.Text + "','" + cboRemark.Text + "','" + txtCustomer.Text + "','" + txtSaleOrder.Text + "','" + txtReq.Text + "'" +
        //            ",'" + ((DateTime)dtpReqDate.EditValue).ToString("yyyyMMdd", dtfinfo) + "','" + txtRef.Text + "','" + txtPayTerm.Text + "'" +
        //            ",'" + ((DateTime)dtpDelivery.EditValue).ToString("yyyyMMdd", dtfinfo) + "','" + optProduct.SelectedIndex + "'" +
        //            "," + Convert.ToDouble(gridView1.Columns["QTY"].SummaryItem.SummaryValue) + "," + txtTotal.Text + "," + txtVat.Text + "," + txtVatTotal.Text + "," + txtGrand.Text +
        //            ",N'" + txtRemark1.Text + "',N'" + txtRemark2.Text + "',N'" + txtRemark3.Text + "','" + Environment.MachineName + "','" + DateTime.Today.ToString("yyyyMMdd", dtfinfo) + "'";
        //        strSQL+=(chkCancel.Checked)?",'1'":",'0'";
        //        strSQL += ")";
        //        db_TPiCS.Execute(strSQL);
        //        //Insert PurchaseOrder
        //        strSQL = "INSERT INTO FABRICNORMALPO(PONO,PODATE,SUPPLIERID,DEPARTMENT,DEPARTMENTID,TYPEID,CUSTOMERNAME,SALENO,REQNO,REQDATE" + 
        //            ",REFNO,REM1,REM2,REM3,TOTALAMOUNT,VAT,MONEYTYPE,PAYTERM,DATEDELIVERY,DISCOUNT,STATUS,POTYPE,REMARK,REVISED,REVISEDATE)VALUES(" + 
        //            "'" + strPO + "','" + ((DateTime)dtpPODate.EditValue).ToString("dd/MM/yy", dtfinfo) + "','" + txtSupplierID.Text + "','" + txtDept.Text + "'" + 
        //            ",'" + cboSection.Text + "','" + CheckTypeID(cboPOType.Text) + "','" + txtCustomer.Text + "','" + txtSaleOrder.Text + "','" + txtReq.Text + "'" + 
        //            ",'" + ((DateTime)dtpReqDate.EditValue).ToString("dd/MM/yy", dtfinfo) + "','" + txtRef.Text + "',N'" + txtRemark1.Text + "',N'" + txtRemark2.Text + "'" + 
        //            ",N'" + txtRemark3.Text + "'," + txtTotal.Text + "," + txtVat.Text + "," + CheckMoneyType(cboCur.Text) + ",'" + txtPayTerm.Text + "'" + 
        //            ",'" + ((DateTime)dtpDelivery.EditValue).ToString("dd/MM/yy", dtfinfo) + "',0,0," + optProduct.SelectedIndex + ",'" + cboRemark.Text + "',0,'')";
        //        db.Execute(strSQL);
        //        cboPO.Properties.Items.Add(strPO);
        //    }
        //    else
        //    {
        //        //Update TPiCS
        //        strSQL = "UPDATE THPO SET PODATE='" + ((DateTime)dtpPODate.EditValue).ToString("yyyyMMdd", dtfinfo) + "'" + 
        //            ",SUPID='" + txtSupplierID.Text + "',POTYPE='" + cboPOType.Text + "',SECTID='" + cboSection.Text + "'" + 
        //            ",DEPTID='" + txtDept.Text + "',CURRE='" + cboCur.Text + "',POTITLE='" + cboRemark.Text + "'" + 
        //            ",CUSTNO='" + txtCustomer.Text + "',SORDNO='" + txtSaleOrder.Text + "',REQNO='" + txtReq.Text + "'" + 
        //            ",REQDATE='" + ((DateTime)dtpReqDate.EditValue).ToString("yyyyMMdd", dtfinfo) + "',REFNO='" + txtRef.Text + "'" + 
        //            ",PAYTERM='" + txtPayTerm.Text + "',TIMEDLY='" + ((DateTime)dtpDelivery.EditValue).ToString("yyyyMMdd", dtfinfo) + "'" + 
        //            ",TYPE='" + optProduct.SelectedIndex + "',TQTY=" + Convert.ToDouble(gridView1.Columns["QTY"].SummaryItem.SummaryValue) + 
        //            ",TAMT=" + txtTotal.Text + ",VAT=" + txtVat.Text + ",TVAT=" + txtVatTotal.Text + ",GTOTAL=" + txtGrand.Text + 
        //            ",REMARK1=N'" + txtRemark1.Text + "',REMARK2=N'" + txtRemark2.Text + "',REMARK3=N'" + txtRemark3.Text + "' " + 
        //            ",UPDBY='" + Environment.MachineName + "',UPDDATE='" + DateTime.Today.ToString("yyyyMMdd",dtfinfo) + "'";
        //        strSQL+=(chkCancel.Checked)?",CANCEL='1'":",CANCEL='0'";
        //        strSQL += " WHERE PONO='" + strPO + "'";
        //        db_TPiCS.Execute(strSQL);
        //        //Update PurchaseOrder
        //        strSQL = "UPDATE FABRICNORMALPO SET PODATE='" + ((DateTime)dtpPODate.EditValue).ToString("dd/MM/yy", dtfinfo) + "'" + 
        //            ",SUPPLIERID='" + txtSupplierID.Text + "',DEPARTMENT='" + txtDept.Text + "',DEPARTMENTID='" + cboSection.Text + "'" + 
        //            ",TYPEID='" + CheckTypeID(cboPOType.Text) + "',CUSTOMERNAME='" + txtCustomer.Text + "',SALENO='" + txtSaleOrder.Text + "'" + 
        //            ",REQNO='" + txtReq.Text + "',REQDATE='" + ((DateTime)dtpReqDate.EditValue).ToString("dd/MM/yy", dtfinfo) + "'" + 
        //            ",REFNO='" + txtRef.Text + "',REM1=N'" + txtRemark1.Text + "',REM2=N'" + txtRemark2.Text + "',REM3=N'" + txtRemark3.Text + "'" + 
        //            ",TOTALAMOUNT=" + txtTotal.Text + ",VAT=" + txtVat.Text + ",MONEYTYPE=" + CheckMoneyType(cboCur.Text) + 
        //            ",PAYTERM='" + txtPayTerm.Text + "',DATEDELIVERY='" + ((DateTime)dtpDelivery.EditValue).ToString("dd/MM/yy", dtfinfo) + "'" + 
        //            ",DISCOUNT=0,STATUS=0,POTYPE=" + optProduct.SelectedIndex + ",REMARK='" + cboRemark.Text + "'" + 
        //            ",REVISED=0,REVISEDATE='' WHERE PONO='" + strPO + "'";
        //        db.Execute(strSQL);
        //    }
        //    //PO DETAIL TPICS
        //    strSQL = "DELETE FROM TDPO WHERE PONO='" + strPO + "'";
        //    db_TPiCS.Execute(strSQL);
        //    for(int i=0;i<gridView1.DataRowCount;i++)
        //    {
        //        strSQL = "INSERT INTO TDPO(PONO,EDA,ROWNO,TPICS_ORDER,CODE,BARCODE,DESCR,QTY,UNIT,UPRC,AMT)VALUES(" + 
        //            "'" + strPO + "',0," + i + ",'" + gridView1.GetRowCellDisplayText(i, "TPICS_ORDER") + "'" + 
        //            ",'" + gridView1.GetRowCellDisplayText(i, "CODE") + "','" + gridView1.GetRowCellDisplayText(i, "BARCODE") + "'" + 
        //            ",N'" + gridView1.GetRowCellDisplayText(i, "DESCRIPTION") + "'," + gridView1.GetRowCellDisplayText(i, "QTY") + 
        //            ",'" + gridView1.GetRowCellDisplayText(i, "UNIT") + "'," + gridView1.GetRowCellDisplayText(i, "PRICE") + 
        //            "," + gridView1.GetRowCellDisplayText(i, "AMOUNT") + ")";
        //        db_TPiCS.Execute(strSQL);
        //    }
        //    //PO DETAIL PURCHASEORDER
        //    strSQL = "DELETE FROM FABRICNORMALPODETAIL WHERE PONO='" + strPO + "'";
        //    db.Execute(strSQL);
        //    int intRow= -1;
        //    for(int i=0;i<gridView1.DataRowCount;i++)
        //    {
        //        intRow += 1;
        //        strSQL = "INSERT INTO FABRICNORMALPODETAIL(PONO,LINE,PRODUCTCODE,DESCRIPTION,QTY,UNIT,UNITPRICE,TPICS_ORDER)VALUES('" + strPO + "'" + 
        //            "," + intRow;
        //        if(cboRemark.Text== "RAW YARN")
        //        {
        //            strSQL = strSQL + ",'" + Convert.ToInt32(gridView1.GetRowCellDisplayText(i, "CODE")) + "'";
        //        }
        //        else
        //        {
        //            strSQL = strSQL + ",'" + gridView1.GetRowCellDisplayText(i, "CODE") + "'";
        //        }
        //        strSQL = strSQL + ",N'" + gridView1.GetRowCellDisplayText(i, "BARCODE") + "'" + 
        //            "," + gridView1.GetRowCellDisplayText(i, "QTY") + ",'" + gridView1.GetRowCellDisplayText(i, "UNIT") + "'" + 
        //            "," + gridView1.GetRowCellDisplayText(i, "PRICE") + ",'" + gridView1.GetRowCellDisplayText(i, "TPICS_ORDER") + "')";
        //        db.Execute(strSQL);
        //        intRow += 1;
        //        strSQL = "INSERT INTO FABRICNORMALPODETAIL(PONO,LINE,DESCRIPTION)VALUES('" + strPO + "'," + intRow + ",'" + gridView1.GetRowCellDisplayText(i, "DESCRIPTION") + "')";
        //        db.Execute(strSQL);
        //    }
        //    //Update PONO in XSLIP
        //    for(int i=0;i<gridView1.DataRowCount;i++)
        //    {
        //        strSQL = "UPDATE XSLIP SET CONT='SECTION " + cboSection.Text + "',PO='" + strPO + "',VENDOR2='" + sleSupplier.EditValue + "' " +
        //            "WHERE PORDER='" + gridView1.GetRowCellDisplayText(i, "TPICS_ORDER") + "'";
        //        db_TPiCS.Execute(strSQL);
        //    }
        //}

        //private void NewPO(string strType )
        //{
        //    try
        //    {
        //        this.Cursor = Cursors.WaitCursor;
        //        db_TPiCS.ConnectionOpen();
        //        string strSQL;
        //        string strTemp;
        //        int intTemp;
        //        switch(strType)
        //        {
        //            case "RAW YARN":
        //                strSQL = "SELECT MAX(PONO) AS PONO FROM THPO WHERE POTITLE='RAW YARN'";
        //                strTemp=db_TPiCS.ExecuteFirstValue(strSQL);
        //                if(strTemp == ""){strTemp = "000000";}
        //                intTemp= Convert.ToInt32(Module.Right(strTemp, 6)) + 1;
        //                cboPO.Text = "FX"+intTemp.ToString().PadLeft(6,'0');
        //                break;
        //            case "DYEING FEE":
        //                strSQL = "SELECT MAX(PONO) AS PONO FROM THPO WHERE POTITLE='DYEING FEE'";
        //                strTemp=db_TPiCS.ExecuteFirstValue(strSQL);
        //                if(strTemp==""){strTemp = "000000";}
        //                intTemp= Convert.ToInt32(Module.Right(strTemp, 6)) + 1;
        //                cboPO.Text = "FD"+intTemp.ToString().PadLeft(6,'0');
        //                break;
        //            case "KNITTING FEE":
        //                strSQL = "SELECT MAX(SUBSTRING(PONO,6,4)) AS PONO FROM THPO "+
        //                    "WHERE POTITLE='KNITTING FEE' AND SUBSTRING(REQDATE,1,4)='" +DateTime.Today.ToString("yyyy",dtfinfo)+"'";
        //                strTemp=db_TPiCS.ExecuteFirstValue(strSQL);
        //                if(strTemp==""){strTemp = "0000";}
        //                intTemp=Convert.ToInt32(Module.Right(strTemp, 4)) + 1;
        //                cboPO.Text = "FB.S";
        //                if(cboSection.Text.Length > 0){ cboPO.Text+=Module.Right(cboSection.Text, 1);}
        //                cboPO.Text+=intTemp.ToString().PadLeft(4,'0')+ "-"+DateTime.Today.ToString("yy",dtfinfo);
        //                if(txtRef.Text=="SAMPLE"){cboPO.Text+="-S";}
        //                break;
        //            case "SOAPING":
        //                break;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        db_TPiCS.ConnectionClose();
        //        this.Cursor = Cursors.Default;
        //    }

        //    LoadfrmTS5_PONew();
        //}

        private void frmPO_Load(object sender,EventArgs e)
        {
            dtfinfo=clinfo.DateTimeFormat;
            try
            {
                PictureEdit1.Image = Resource1.tuw_logo1;
                PictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
                PictureEdit1.Properties.PictureAlignment = ContentAlignment.MiddleCenter;
                PictureEdit1.BackColor = Color.White;
                PictureEdit2.Image = Resource1.purchase_pic;
                PictureEdit2.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
                PictureEdit2.Properties.PictureAlignment = ContentAlignment.MiddleCenter;
                txtTotal.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                txtVat.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
 
                //ClearForm(true,true,true,true);
                //LoadSupplierName();
                //LoadPONo();
                GetPO(txtRemark.Text);
                GetCustomer();
                GetSaleOrder();
                GetCurrency();
                GetSection();
                GetPaymentTerm();
                GetSupplier();
                rpUnit=GetUnit();
                rpCode=GetProductCode(txtRemark.Text);
                NewData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cboPO_CloseUp(object sender,DevExpress.XtraEditors.Controls.CloseUpEventArgs e)
        {
            try
            {
                //DisplayData(cboPO.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private void cboPO_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e)
        {
            if(e.KeyChar ==13)
            {
                try
                {
                    // DisplayData(cboPO.Text);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }
        private void txtDept_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtCustomer_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtSaleOrder_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtReq_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtRef_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtRemark1_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtRemark2_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void txtRemark3_KeyPress(object sender,System.Windows.Forms.KeyPressEventArgs e)
        {
            if(e.KeyChar==13){SendKeys.Send("{TAB}");}
        }
        private void gridView1_CustomRowCellEdit(object sender,DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            switch (e.Column.FieldName)
            { 
                case "CODE":
                    if (gridView1.IsNewItemRow(e.RowHandle) && (txtRemark.Text!="DYEING FEE"))
                    {
                        e.RepositoryItem = rpCode;
                    }
                    break;
                //case "DESCRIPTION":
                //    //RepositoryItemMemoEdit memo = new RepositoryItemMemoEdit();
                //    //memo.AutoHeight = true;
                //    //memo.WordWrap = true;
                //    RepositoryItemMemoExEdit memo = new RepositoryItemMemoExEdit();
                //    e.RepositoryItem = memo;
               //     break;
               // case "UNIT":
               //     e.RepositoryItem = rpUnit;
               //     break;
            }
        }
        private void gridView1_RowUpdated(object sender,DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            CalculateTotal();
        }
        private void gridView1_ValidatingEditor(object sender,DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                switch(gridView1.FocusedColumn.FieldName)
                {
                    case "CODE":
                        if (!gridView1.IsNewItemRow(gridView1.FocusedRowHandle)) { return; }
                        switch (txtRemark.Text)
                        {
                            case "KNITTING FEE":
                                string strName = (from p in productCodes where p.Code == e.Value.ToString() select p.Name).First().ToString();
                                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "NAME",strName );
                                db.ConnectionOpen();
                                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "DESCRIPTION", GetYarnByFabric(strName));
                                db.ConnectionClose();
                                break;
                            case "DYEING FEE":
                                string strID = e.Value.ToString().Substring(0, 5);
                                string strColor = e.Value.ToString().Substring(5, e.Value.ToString().Length - 5);
                                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "NAME",(from p in productCodes where p.Code==e.Value.ToString().Substring(0,5) select p.Name).First().ToString()+" "+strColor);
                                break;
                            default:
                                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "NAME",(from p in productCodes where p.Code == e.Value.ToString() select p.Name).First().ToString() );
                                string strDescription = (from p in productCodes where p.Code == e.Value.ToString() select p.Description).First().ToString();
                                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "DESCRIPTION", strDescription);
                                break;
                        }
                        break;
                    case "QTY":
                        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "AMOUNT",
                            String.Format("{0:0,0.00}",Convert.ToDouble(e.Value) * Convert.ToDouble(gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "PRICE"))));
                        break;
                    case "PRICE":
                        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "AMOUNT",
                            String.Format("{0:0,0.00}", Convert.ToDouble(e.Value) * Convert.ToDouble(gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, "QTY"))));
                    break;
                }
                CalculateTotal();
            }
            catch{}
        }
        //private void cmdPO_Click(object sender ,EventArgs e)
        //{
        //    popupMenu1.ShowPopup(barManager1, Cursor.Position);
        //}
        private void gridControl1_ProcessGridKey(object sender,System.Windows.Forms.KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            { 
                if(gridView1.IsEditing == false){gridView1.DeleteSelectedRows();}
            }
        }
        //private void cboRemark_SelectedIndexChanged(object sender,EventArgs e)
        //{
        //    switch(cboRemark.Text)
        //    {
        //        case "RAW YARN":
        //            optProduct.SelectedIndex = optProduct.Properties.Items.GetItemIndexByValue("Yarn");
        //            break;
        //        case "DYEING FEE":
        //        case "KNITTING FEE":
        //        case "SOAPING":
        //            optProduct.SelectedIndex = optProduct.Properties.Items.GetItemIndexByValue("Fabric");
        //            break;
        //    }
        //}
        //private void cboCur_SelectedIndexChanged(object sender,EventArgs e)
        //{
        //    if(cboCur.SelectedIndex == cboCur.Properties.Items.IndexOf("Baht"))
        //    {
        //        txtVat.Text = "7";
        //    }
        //    else
        //    {
        //        txtVat.Text = "0";
        //    }
        //    CalculateTotal();
        //}
        //private void mnuYarn_ItemClick(object sender,DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    dtpReqDate.EditValue = DateTime.Today;
        //    cboRemark.SelectedIndex = cboRemark.Properties.Items.IndexOf(e.Item.Caption.ToString());
        //    LoadfrmTS5_PONew();
        //}
        //private void mnuKnit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    if (cboSection.SelectedIndex <= 0)
        //    {
        //        MessageBox.Show("กรุณาเลือกแผนกก่อนจากช่อง Section", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //        return;
        //    }
        //    cboRemark.SelectedIndex = cboRemark.Properties.Items.IndexOf(e.Item.Caption.ToString());
        //    LoadfrmTS5_PONew();
        //   // NewPO(e.Item.Caption);
        //}
        //private void mnuDye_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    dtpReqDate.EditValue = DateTime.Today;
        //    cboRemark.SelectedIndex = cboRemark.Properties.Items.IndexOf(e.Item.Caption.ToString());
        //    LoadfrmTS5_PONew();
        //    //NewPO(e.Item.Caption);
        //}
        private void sleSupplier_EditValueChanged(object sender, EventArgs e)
        {
            txtSupplierID.Text = "";
            txtAD1.Text = "";
            txtAD2.Text = "";
            txtZip.Text = "";
            txtCountry.Text = "";
            txtTel.Text = "";
            txtFax.Text = "";
            luePayterm.EditValue = null;
            if (sleSupplier.EditValue == null) return;
            else GetSupplierDetail(sleSupplier.EditValue.ToString());
        }
        private void slePONo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                lueCurrency.EditValue = null;
                cboPOType.SelectedIndex = 0;
                lueSection.EditValue = null;
                chkRevise.Checked = false;
                chkCancel.Checked = false;

                cboCustomer.Text = "";
                cboSaleOrder.Text = "";
                dtpPODate.EditValue = DateTime.Today;
                txtReq.Text = "";
                dtpReqDate.EditValue = null;
                txtRef.Text = "";
                dtpDelivery.EditValue = null;
                txtRemark1.Text = "";
                txtRemark2.Text = "";
                txtRemark3.Text = "";
                txtTotal.Text = "";
                txtVat.Text = "7";
                txtVatTotal.Text = "";
                txtGrand.Text = "";

                if (sleSupplier.EditValue == null) sleSupplier_EditValueChanged(null, null);
                else sleSupplier.EditValue = null;

                if (slePONo.EditValue == null)
                {
                    dtDetail = new DataTable();
                    dtDetail.BeginInit();
                    dtDetail.Columns.Add("CODE", typeof(string));
                    dtDetail.Columns.Add("NAME", typeof(string));
                    dtDetail.Columns.Add("DESCRIPTION", typeof(string));
                    dtDetail.Columns.Add("QTY", typeof(decimal));
                    dtDetail.Columns.Add("UNIT", typeof(string));
                    dtDetail.Columns.Add("PRICE", typeof(decimal));
                    dtDetail.Columns.Add("AMOUNT", typeof(decimal));
                    dtDetail.EndInit();
                    gridControl1.DataSource = dtDetail;
                    gridView1.Columns["UNIT"].ColumnEdit = rpUnit;
                    RepositoryItemMemoExEdit memo = gridControl1.RepositoryItems.Add("MemoExEdit") as RepositoryItemMemoExEdit;
                    gridView1.Columns["DESCRIPTION"].ColumnEdit = memo;
                    gridView1.Columns["QTY"].SummaryItem.FieldName = "QTY";
                    gridView1.Columns["QTY"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
                    gridView1.Columns["AMOUNT"].SummaryItem.FieldName = "AMOUNT";
                    gridView1.Columns["AMOUNT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
                    gridView1.OptionsView.EnableAppearanceEvenRow = true;
                    gridView1.OptionsView.EnableAppearanceOddRow = true;
                    gridView1.OptionsView.ColumnAutoWidth = true;
                    gridView1.BestFitColumns();
                }
                else
                    GetPODetail(slePONo.EditValue.ToString());
                    CalculateTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
            
            //try
            //{
            //    ClearForm(false, true, false, false);
            //    //DisplayData(slePONo.EditValue.ToString());
            //    GetPODetail(slePONo.EditValue.ToString());
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
        }
        private void btnSupplier_Click(object sender, EventArgs e)
        {
            LoadfrmTS5_POSupplier();
        }
        private void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                CalculateTotal();
            }
            catch { }
        }
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(lueSection.Properties.GetDisplayValueByKeyValue(lueSection.EditValue).ToString().Substring(6,1));
        }
        private void lueSection_EditValueChanged(object sender, EventArgs e)
        {
            cboCustomer.Text = lueSection.Text;
        }

        private void cboPOType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPOType.Text == "Local")
                txtVat.Text = "7";
            else
                txtVat.Text = "0";
            CalculateTotal();
        }
        //private void cmdClear_Click(object sender,EventArgs e)
        //{
        //    cboPO.Text = "";
        //    ClearForm(true,true,true,true);
        //}
        //private void cmdSave_Click(object sender,EventArgs e)
        //{
        //    if(cboCur.SelectedIndex == 0)
        //    {
        //        MessageBox.Show("โปรดเลือก Currency Symbol","Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
        //        return;
        //    }
        //    else if(cboPOType.SelectedIndex== 0)
        //    {
        //        MessageBox.Show("โปรดเลือก P/O Type","Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
        //        return;
        //    }
        //    else if(cboSection.SelectedIndex == 0)
        //    {
        //        MessageBox.Show("โปรดเลือก Section","Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
        //        return;
        //    }
        //    else if(cboRemark.SelectedIndex == 0)
        //    {
        //        MessageBox.Show("โปรดเลือก Remark","Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
        //        return;
        //    }
        //    else if(cboPO.Text.Length == 0)
        //    {
        //        MessageBox.Show("โปรดระบุ PO No.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
        //        return;
        //    }
        //    else if(dtpPODate.EditValue==null)
        //    {
        //        MessageBox.Show("โปรดระบุ PO Date","Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
        //        return;
        //    }
        //    else if(dtpReqDate.EditValue==null)
        //    {
        //        MessageBox.Show("โปรดระบุ Requisition Date.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
        //        return;
        //    }
        //    try
        //    {
        //        this.Cursor = Cursors.WaitCursor;
        //        db.ConnectionOpen();
        //        db.BeginTrans();
        //        db_TPiCS.ConnectionOpen();
        //        db_TPiCS.BeginTrans();
        //        SaveData(cboPO.Text);
        //        db.CommitTrans();
        //        db_TPiCS.CommitTrans();
        //        MessageBox.Show("SAVE COMPLETE","Save...",MessageBoxButtons.OK,MessageBoxIcon.Information);
        //    }
        //    catch(Exception ex)
        //    {
        //        db.RollbackTrans();
        //        db_TPiCS.RollbackTrans();
        //        MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        db.ConnectionClose();
        //        db_TPiCS.ConnectionClose();
        //        this.Cursor = Cursors.Default;
        //    }
        //}

    }
    class ProductCode
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}