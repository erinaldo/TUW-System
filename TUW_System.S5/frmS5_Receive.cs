using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using System.Globalization;
using System.Linq;
using myClass;

namespace TUW_System.S5
{
    public partial class frmS5_Receive : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        DataTable dtHeader, dtRemark, dtDetail;
        List<Unit> Units=new List<Unit>();
        string strReceive;//Receive No ใหม่ที่ได้จากการรันใน database
        string _strPONO;//PONO รับค่าจากฟอร์มอื่น
        string _strReceiveNO;//ReceiveNO รับค่าจากฟอร์มอื่น

        public string PONO 
        {
            set { _strPONO = value; }
        }
        public string ReceiveNO
        {
            set { _strReceiveNO = value; }
        }
        public LogIn User_Login { get; set; }
        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }
        
        public frmS5_Receive()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            if (sleReceiveNo.EditValue == null) sleReceiveNo_EditValueChanged(null, null);
            else sleReceiveNo.EditValue = null;
            //ClearForm(clearPrimaryKey: true);

        }
        public void EditData()
        { 
        
        }
        private void DisplayData(string strReceive)
        {
            string strSQL = "SELECT * FROM THRECEIVE LEFT OUTER JOIN XSECT ON THRECEIVE.SUPID=XSECT.BUMO " +
                "LEFT OUTER JOIN THPO ON THRECEIVE.PONO=THPO.PONO WHERE RECNO='" + strReceive + "'" +
                ";SELECT TPICS_ORDER,CODE,BARCODE,DESCR AS DESCRIPTION,POQTY AS [P/O_QTY],RECQTY AS RECEIVE" +
                ",UNIT,UPRC AS PRICE,AMT AS AMOUNT FROM TDRECEIVE WHERE RECNO='" + strReceive + "' ORDER BY ROWNO";
            DataSet ds = db.GetDataSet(strSQL);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dtpReceive.EditValue = new DateTime(Convert.ToInt16(dr["RECDATE"].ToString().Substring(0, 4)), Convert.ToInt16(dr["RECDATE"].ToString().Substring(4, 2)), Convert.ToInt16(dr["RECDATE"].ToString().Substring(6, 2)));
                //txtPO.Text = dr["PONO"].ToString();
                //cboDelivery.Text = dr["DlyNo"].ToString();
                txtSupplierID.Text = dr["SupID"].ToString();
                txtSupplier.Text = dr["NAME"].ToString();
                txtAD1.Text = dr["ADR1"].ToString();
                txtAD2.Text = dr["ADR2"].ToString();
                txtZip.Text = dr["ZIP"].ToString();
                txtCountry.Text = dr["COUNTRY"].ToString();
                txtTel.Text = dr["TEL"].ToString();
                txtFax.Text = dr["FAX"].ToString();
                txtRemark1.Text = dr["Remark1"].ToString();
                txtRemark2.Text = dr["Remark2"].ToString();
                txtRemark3.Text = dr["Remark3"].ToString();
                txtTotal.Text = dr["TAmt"].ToString();
                txtVat.Text = dr["Vat"].ToString();
                txtVatTotal.Text = dr["TVat"].ToString();
                txtGrand.Text = dr["GTotal"].ToString();
                txtCur.Text = dr["Curre"].ToString();
                txtPOType.Text = dr["POType"].ToString();
                txtSection.Text = dr["SectID"].ToString();
                optProduct.SelectedIndex = Convert.ToInt16(dr["Type"]);
                txtRemark.Text = dr["POTitle"].ToString();
                try
                {
                    //picPerson.Image = Image.FromFile(Application.StartupPath + "\\Images\\" + dr["UPDBY"].ToString() + ".jpg");
                }
                catch { } //Do nothing
            }
            gridControl1.DataSource = ds.Tables[1];
        }
        public void DisplayData2()
        {
            DisplayData(sleReceiveNo.EditValue.ToString());
        }
        public void SaveData()
        {
            if (dtpReceive.EditValue == null)
            {
                MessageBox.Show("โปรดระบุ Receive date", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            string strSQL;
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            //run receiveno ใหม่กรณีช่อง sleReceive ไม่มีค่าแสดงว่าเป็นข้อมูลใหม่
            db.ConnectionOpen();
            if (sleReceiveNo.EditValue == null)
            {
                strSQL = "EXEC spTUWSystem_RunNo 'SALES5_RECEIVE',''";
                strReceive = db.ExecuteFirstValue(strSQL);//สร้าง strReceive มารับค่า ReceiveNo ใหม่เพราะถ้าใส่ไปที่ sleReceive ตรงๆจะทำให้เกิด event editvaluechanged
            }
            else
            {
                strReceive = sleReceiveNo.EditValue.ToString();
            }
            try
            {
                db.BeginTrans();
                //Main------------------------------
                strSQL = "UPDATE PO_RECEIVE SET " +
                    "PONO='" + txtPONO.Text + "'" +
                    ",RECEIVEDATE='" + ((DateTime)dtpReceive.EditValue).ToString("yyyy-MM-dd", dtfinfo)+"'";
                strSQL += (txtInvoice.Text == "") ? ",INVOICENO=NULL" : ",INVOICENO=N'" + txtInvoice.Text + "'";
                strSQL += (dtpInvoice.Text == "") ? ",INVOICEDATE=NULL" : ",INVOICEDATE='" + ((DateTime)dtpInvoice.EditValue).ToString("yyyy-MM-dd", dtfinfo)+"'";
                strSQL += (txtDelivery.Text == "") ? ",DELIVERYNO=NULL" : ",DELIVERYNO=N'" + txtDelivery.Text + "'";
                strSQL += (dtpDelivery.Text == "") ? ",DELIVERYDATE=NULL" : ",DELIVERYDATE='" + ((DateTime)dtpDelivery.EditValue).ToString("yyyy-MM-dd", dtfinfo)+"'";
                strSQL += (txtCur.Text == "") ? ",CURRENCYUNIT=NULL" : ",CURRENCYUNIT='" + txtCur.Tag.ToString() + "'";
                strSQL += (txtVat.Text == "") ? ",VAT=0" : ",VAT=" + txtVat.Text;
                strSQL += (chkCancel.Checked) ? ",CANCEL='1'" : ",CANCEL='0'";
                strSQL += (txtSupplierID.Text == "") ? ",IDSUP=NULL" : ",IDSUP='" + txtSupplierID.Text + "'";
                strSQL += (txtDepartment.Text == "") ? ",DEPARTMENT=NULL" : ",DEPARTMENT='" + txtDepartment.Text + "'";
                strSQL += ",EMPCODE='" + User_Login.EmployeeCode + "'";
                strSQL += " WHERE RECEIVENO='" + strReceive + "'";
                db.Execute(strSQL);
                //Detail------------------------------
                strSQL = "DELETE FROM PO_RECEIVEDETAIL WHERE RECEIVENO='" + strReceive + "'";
                db.Execute(strSQL);
                for (int i = 0; i < gridView1.DataRowCount; i++)
                { 
                    strSQL="INSERT INTO PO_RECEIVEDETAIL(PONO,RECEIVENO,PRODUCTCODE,PRODUCTNAME,DESCRIPTION,IDUNIT,SCHEDULEQTY,QTY,UNITPRICE,AMOUNT,LINE)VALUES("+
                        "'"+txtPONO.Text+"'"+
                        ",'"+strReceive+"'"+
                        ",N'" + gridView1.GetRowCellDisplayText(i, "CODE") + "'" +
                        ",N'" + gridView1.GetRowCellDisplayText(i, "NAME") + "'" +
                        ",N'" + gridView1.GetRowCellDisplayText(i, "DESCRIPTION").Replace("'","''") + "'" +
                        ",N'" + (from p in Units where p.Name== gridView1.GetRowCellDisplayText(i, "UNIT") select p.ID).First() + "'" +
                        ","+gridView1.GetRowCellValue(i,"SCHEDULE") +
                        "," + gridView1.GetRowCellValue(i, "RECEIVE") +
                        "," + gridView1.GetRowCellValue(i, "PRICE") +
                        "," + gridView1.GetRowCellValue(i, "AMOUNT") +
                        ","+ gridView1.GetRowCellValue(i,"LINE") + ")";
                    db.Execute(strSQL);
                }
                //Remark------------------------------
                strSQL = "DELETE FROM PO_RECEIVEREMARK WHERE RECEIVENO='" + strReceive + "'";
                db.Execute(strSQL);
                if (txtRemark1.Text.Length > 0)
                {
                    strSQL = "INSERT INTO PO_RECEIVEREMARK(RECEIVENO,REMARK,LINE)VALUES(" +
                        "'" + strReceive + "',N'" + txtRemark1.Text.Replace("'", "''") + "',1)";
                    db.Execute(strSQL);
                }
                if (txtRemark2.Text.Length > 0)
                {
                    strSQL = "INSERT INTO PO_RECEIVEREMARK(RECEIVENO,REMARK,LINE)VALUES(" +
                        "'" + strReceive + "',N'" + txtRemark2.Text.Replace("'", "''") + "',2)";
                    db.Execute(strSQL);
                }
                if (txtRemark3.Text.Length > 0)
                {
                    strSQL = "INSERT INTO PO_RECEIVEREMARK(RECEIVENO,REMARK,LINE)VALUES(" +
                        "'" + strReceive + "',N'" + txtRemark3.Text.Replace("'", "''") + "',3)";
                    db.Execute(strSQL);
                }

              
                    //strSQL = "SELECT COUNT(RECNO) FROM THRECEIVE WHERE RECNO='" + strReceive + "'";
                //if (Convert.ToInt16(db.ExecuteFirstValue(strSQL)) == 0)
               // {
                    //INSERT
                    //strSQL = "INSERT INTO THRECEIVE(RECNO,EDA,RECDATE,PONO,SUPID,DLYNO,SORDNO,TYPE,TQTY,TAMT,VAT,TVAT,GTOTAL" +
                    //    ",REMARK1,REMARK2,REMARK3,UPDBY,UPDDATE)VALUES('" + strReceive + "',0,'" + ((DateTime)dtpReceive.EditValue).ToString("yyyyMMdd", dtfinfo) + "'" +
                    //    ",'" + txtPO.Text + "','" + txtSupplierID.Text + "','" + cboDelivery.Text + "','','" + optProduct.SelectedIndex + "'" +
                    //    "," + Convert.ToDouble(gridView1.Columns["RECEIVE"].SummaryItem.SummaryValue) + "," + txtTotal.Text + "," + txtVat.Text +
                    //    "," + txtVatTotal.Text + "," + txtGrand.Text + ",N'" + txtRemark1.Text + "',N'" + txtRemark2.Text + "',N'" + txtRemark3.Text +
                    //    "'" + ",'" + Environment.MachineName + "','" + DateTime.Today.ToString("yyyyMMdd", dtfinfo) + "')";
                    //db.Execute(strSQL);
               // }
                //else
               // {
                    //UPDATE
                    //strSQL = "UPDATE THRECEIVE SET RECDATE='" + ((DateTime)dtpReceive.EditValue).ToString("yyyyMMdd", dtfinfo) + "'" +
                    //    ",PONO='" + txtPO.Text + "',SUPID='" + txtSupplierID.Text + "',DLYNO='" + cboDelivery.Text + "',SORDNO=''" +
                    //    ",TYPE='" + optProduct.SelectedIndex + "',TQTY=" + Convert.ToDouble(gridView1.Columns["RECEIVE"].SummaryItem.SummaryValue) +
                    //    ",TAMT=" + txtTotal.Text + ",VAT=" + txtVat.Text + ",TVAT=" + txtVatTotal.Text + ",GTOTAL=" + txtGrand.Text +
                    //    ",REMARK1=N'" + txtRemark1.Text + "',REMARK2=N'" + txtRemark2.Text + "',REMARK3=N'" + txtRemark3.Text + "'" +
                    //    ",UPDBY='" + Environment.MachineName + "',UPDDATE='" + DateTime.Today.ToString("yyyyMMdd", dtfinfo) + "'" +
                    //    " WHERE RECNO='" + strReceive + "'";
                    //db.Execute(strSQL);
              //  }
              //  strSQL = "DELETE FROM TDRECEIVE WHERE RECNO='" + strReceive + "'";
              //  db.Execute(strSQL);
              //  for (int i = 0; i < gridView1.DataRowCount; i++)
              //  {
                    //strSQL = "INSERT INTO TDRECEIVE(RECNO,EDA,ROWNO,PONO,TPICS_ORDER,DESCR,CODE,BARCODE,POQTY,DIFFQTY,RECQTY" +
                    //    ",BALQTY,UNIT,UPRC,AMT)VALUES(" +
                    //    "'" + strReceive + "','0'," + i + ",'" + txtPO.Text + "','" + gridView1.GetRowCellDisplayText(i, "TPICS_ORDER") + "'" +
                    //    ",N'" + gridView1.GetRowCellDisplayText(i, "DESCRIPTION") + "','" + gridView1.GetRowCellDisplayText(i, "CODE") + "'" +
                    //    ",'" + gridView1.GetRowCellDisplayText(i, "BARCODE") + "'," + gridView1.GetRowCellDisplayText(i, "P/O_QTY") +
                    //    "," + Math.Abs(Convert.ToDouble(gridView1.GetRowCellDisplayText(i, "P/O_QTY")) - Convert.ToDouble(gridView1.GetRowCellDisplayText(i, "RECEIVE"))) +
                    //    "," + gridView1.GetRowCellDisplayText(i, "RECEIVE") +
                    //    "," + Math.Abs(Convert.ToDouble(gridView1.GetRowCellValue(i, "P/O_QTY")) - Convert.ToDouble(gridView1.GetRowCellValue(i, "RECEIVE"))) +
                    //    ",'" + gridView1.GetRowCellDisplayText(i, "UNIT") + "'," + gridView1.GetRowCellDisplayText(i, "PRICE") +
                    //    "," + gridView1.GetRowCellDisplayText(i, "AMOUNT") + ")";
                    //db.Execute(strSQL);
                    ////Update ReceiveNo in Xslip
                    //strSQL = "UPDATE XSACT SET RECEIVENO='" + strReceive + "' WHERE PORDER='" + gridView1.GetRowCellDisplayText(i, "TPICS_ORDER") + "' AND DELIVERYNO='" + cboDelivery.Text + "'";
                    //db.Execute(strSQL);
              //  }
                db.CommitTrans();
                MessageBox.Show("Save complete...", "Receive Note", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GetReceive();
                sleReceiveNo.EditValue = strReceive;
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
        }

        public void DeleteData()
        {
            if (sleReceiveNo.EditValue == null) { return; }
            if (MessageBox.Show("คุณต้องการลบเลขที่รับ " + sleReceiveNo.EditValue.ToString() + " หรือไม่", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) { return; }
            if (MessageBox.Show("คุณแน่ใจจริงๆนะที่จะลบเลขที่รับ " + sleReceiveNo.EditValue.ToString(), "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) { return; }
            if (MessageBox.Show("กด Yes อีกครั้งเพื่อยืนยันว่าคุณต้องการลบเลขที่รับ " + sleReceiveNo.EditValue.ToString(), "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) { return; }
            try
            {
                db.ConnectionOpen();
                db.BeginTrans();
                string strSQL = "DELETE FROM THRECEIVE WHERE RECNO='" + sleReceiveNo.EditValue.ToString() + "'";
                db.Execute(strSQL);
                strSQL = "DELETE FROM TDRECEIVE WHERE RECNO='" + sleReceiveNo.EditValue.ToString() + "'";
                db.Execute(strSQL);
                
                //var query = from p in dtReceiveNo.AsEnumerable()
                //            where p.Field<string>("RECNO") != sleReceiveNo.EditValue.ToString()
                //            select p;
                //dtReceiveNo = query.CopyToDataTable();
                //sleReceiveNo.Properties.DataSource = dtReceiveNo;
                //ClearForm(clearPrimaryKey: true);
                db.CommitTrans();
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.ConnectionClose();
            }
        }
        public void PrintPreview()
        {
            cCrystalReport crpReceive = new cCrystalReport(Application.StartupPath + @"\Report\S5\S5_ReceiveNote.rpt");
            if (crpReceive.SetPrinter() == false) { return; }
            crpReceive.ReportTitle = sleReceiveNo.EditValue.ToString();
            for (int i = 1; i <= crpReceive.ReportCopy; i++)
            {
                crpReceive.ClearParameters();
                crpReceive.SetParameter("Copy", i.ToString());
                if (chkCancel.Checked)
                {
                    crpReceive.SetParameter("Revise", "REVISE");
                }
                else
                {
                    crpReceive.SetParameter("Revise", "");
                }
                string fmlText = "{PO_RECEIVE.RECEIVENO}='" + sleReceiveNo.EditValue.ToString() + "'";
                crpReceive.PrintReport(fmlText, false,"sa","ZAQ113m4tuw");
            }
        }
        public void Print()
        {
            cCrystalReport crpReceive = new cCrystalReport(Application.StartupPath + @"\Report\S5\S5_ReceiveNote.rpt");
            Int16 intTemp = 1;
            try
            {
                string strValue = "3";
                if (cUtility.InputBox("คุณต้องการพิมพ์กี่ copy", "จำนวนสำเนา", ref strValue) == DialogResult.OK)//เปิ้ลขอไว้ให้มี 3 ก๊อบปี้อัตโนมัติ
                {
                    intTemp = Int16.Parse(strValue);
                }
            }
            catch
            {
                intTemp = 1;
            }
            if (crpReceive.SetPrinter() == false) { return; }
            crpReceive.ReportTitle = sleReceiveNo.EditValue.ToString();
            for (int i = 1; i <= intTemp; i++)
            {
                crpReceive.ClearParameters();
                crpReceive.SetParameter("Copy", i.ToString());
                if (chkCancel.Checked)
                {
                    crpReceive.SetParameter("Revise", "REVISE");
                }
                else
                {
                    crpReceive.SetParameter("Revise", "");
                }
                string fmlText = "{PO_RECEIVE.RECEIVENO}='" + sleReceiveNo.EditValue.ToString() + "'";
                crpReceive.PrintReport(fmlText, true,"sa","ZAQ113m4tuw");
            }
        }


        private void ClearForm(bool clearPrimaryKey)
        {
            foreach (Control ctl in LayoutControl1.Controls)
            {
                if (clearPrimaryKey == false)
                {
                    if (ctl.Name == "sleReceiveNo") { continue; }
                }

                switch (ctl.GetType().ToString())
                {
                    case "DevExpress.XtraEditors.TextEdit":
                        ((DevExpress.XtraEditors.TextEdit)ctl).Text = "";
                        break;
                    case "DevExpress.XtraEditors.DateEdit":
                        ((DevExpress.XtraEditors.DateEdit)ctl).EditValue =DateTime.Today;                        break;
                    case "DevExpress.XtraGrid.GridControl":
                        ((DevExpress.XtraGrid.GridControl)ctl).DataSource = null;
                        break;
                    case "DevExpress.XtraEditors.SearchLookUpEdit":
                        ((DevExpress.XtraEditors.SearchLookUpEdit)ctl).EditValue = null;
                        break;
                    case "DevExpress.XtraEditors.ComboBoxEdit":
                        ((DevExpress.XtraEditors.ComboBoxEdit)ctl).Text = "";
                        break;
                }
            }
            txtVat.Text = "7";
            //picPerson.EditValue = null;
            //cboDelivery.Properties.Items.Clear();
            txtCur.Text = "";
            txtPOType.Text = "";
            txtSection.Text = "";
            optProduct.SelectedIndex = 0;
            txtRemark.Text = "";
            chkCancel.Checked = false;
            //gridView1------------------------------------
            DataTable dt = new DataTable();
            dt.BeginInit();
            dt.Columns.Add("TPICS_ORDER", typeof(string));
            dt.Columns.Add("CODE", typeof(string));
            dt.Columns.Add("BARCODE", typeof(string));
            dt.Columns.Add("DESCRIPTION", typeof(string));
            dt.Columns.Add("P/O_QTY", typeof(double));
            dt.Columns.Add("RECEIVE", typeof(double));
            dt.Columns.Add("UNIT", typeof(string));
            dt.Columns.Add("PRICE", typeof(double));
            dt.Columns.Add("AMOUNT", typeof(double));
            dt.EndInit();
            gridControl1.DataSource = dt;
            gridView1.Columns["P/O_QTY"].SummaryItem.FieldName = "P/O_QTY";
            gridView1.Columns["P/O_QTY"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView1.Columns["RECEIVE"].SummaryItem.FieldName = "RECEIVE";
            gridView1.Columns["RECEIVE"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView1.Columns["AMOUNT"].SummaryItem.FieldName = "AMOUNT";
            gridView1.Columns["AMOUNT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = true;
            gridView1.BestFitColumns();
            //if (blnHeader)
            //{
            //    cboPO.Text = "";
            //    cboDelivery.Text = "";
            //    cboDelivery.Properties.Items.Clear();

            //    txtSupplierID.Text = "";
            //    txtSupplier.Text = "";
            //    txtAD1.Text = "";
            //    txtAD2.Text = "";
            //    txtZip.Text = "";
            //    txtCountry.Text = "";
            //    txtTel.Text = "";
            //    txtFax.Text = "";

            //    dtpReceive.EditValue = DateTime.Today;

            //    txtRemark1.Text = "";
            //    txtRemark2.Text = "";
            //    txtRemark3.Text = "";
            //    txtTotal.Text = "";
            //    txtVat.Text = "7";
            //    txtVatTotal.Text = "";
            //    txtGrand.Text = "";

            //    txtCur.Text = "";
            //    txtPOType.Text = "";
            //    txtSection.Text = "";
            //    optProduct.SelectedIndex = 0;
            //    txtRemark.Text = "";
            //    picPerson.Image = null;
            //    chkRevise.Checked = false;
            //}
            //if (blnDetail)
            //{
            //    DataTable dt = new DataTable();
            //    dt.BeginInit();
            //    dt.Columns.Add("TPICS_ORDER", typeof(string));
            //    dt.Columns.Add("CODE", typeof(string));
            //    dt.Columns.Add("BARCODE", typeof(string));
            //    dt.Columns.Add("DESCRIPTION", typeof(string));
            //    dt.Columns.Add("P/O_QTY", typeof(double));
            //    dt.Columns.Add("RECEIVE", typeof(double));
            //    dt.Columns.Add("UNIT", typeof(string));
            //    dt.Columns.Add("PRICE", typeof(double));
            //    dt.Columns.Add("AMOUNT", typeof(double));
            //    dt.EndInit();
            //    gridControl1.DataSource = dt;
            //    gridView1.Columns["P/O_QTY"].SummaryItem.FieldName = "P/O_QTY";
            //    gridView1.Columns["P/O_QTY"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            //    gridView1.Columns["RECEIVE"].SummaryItem.FieldName = "RECEIVE";
            //    gridView1.Columns["RECEIVE"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            //    gridView1.Columns["AMOUNT"].SummaryItem.FieldName = "AMOUNT";
            //    gridView1.Columns["AMOUNT"].SummaryItem.SetSummary(DevExpress.Data.SummaryItemType.Sum, "{0:n2}");
            //    gridView1.OptionsView.EnableAppearanceEvenRow = true;
            //    gridView1.OptionsView.EnableAppearanceOddRow = true;
            //    gridView1.OptionsView.ColumnAutoWidth = true;
            //    gridView1.BestFitColumns();
            //}
            //if (blnReceiveNo) { cboReceive.Text = ""; }
        }
        private void ImportData(string strPO,string strDelivery)
        {
            string strSQL= "SELECT A.PONo,A.SUPID,B.NAME,B.ADR1,B.ADR2,B.ZIP,B.COUNTRY,B.TEL,"+
                "B.FAX,A.Curre,A.SectID,A.POTitle,A.POType,A.Type "+
                "FROM THPO A INNER JOIN XSECT B ON A.SUPID = B.BUMO WHERE (A.PONo = '" + strPO + "')"+
                ";SELECT  TDPO.TPICS_ORDER, TDPO.CODE, TDPO.Barcode AS BARCODE, TDPO.Descr AS DESCRIPTION, TDPO.Qty AS [P/O_QTY]"+
                ",TDPO.Unit AS UNIT" +
                ",CASE WHEN XITEM.KANZANZA=0 THEN XSACT.APRICE " +
                "WHEN XITEM.KANZANZA=1 THEN XSACT.APRICE/XITEM.KANZANK " +
                "WHEN XITEM.KANZANZA=2 THEN XSACT.APRICE*XITEM.KANZANK END AS PRICE" +
                ",CASE WHEN XITEM.KANZANZA=0 THEN XSACT.APRICE " +
                "WHEN XITEM.KANZANZA=1 THEN XSACT.APRICE/XITEM.KANZANK " +
                "WHEN XITEM.KANZANZA=2 THEN XSACT.APRICE*XITEM.KANZANK END AS PRICE" +
                ",XSACT.KOUNYUUGAKU AS AMOUNT" +
                ",CASE WHEN XITEM.KANZANZA=0 THEN XSACT.JITU0 " +
                "WHEN XITEM.KANZANZA=1 THEN XSACT.JITU0*XITEM.KANZANK " +
                "WHEN XITEM.KANZANZA=2 THEN XSACT.JITU0/XITEM.KANZANK END AS RECEIVE " +
                "FROM TDPO INNER JOIN XSACT ON TDPO.TPICS_ORDER = XSACT.PORDER INNER JOIN XITEM ON XSACT.CODE = XITEM.CODE " +
                "AND XSACT.BUMO=XITEM.BUMO " +
                "WHERE (TDPO.PONo = '" + strPO + "') AND (XSACT.DELIVERYNO = '" + strDelivery + "')";
            DataSet ds  = db.GetDataSet(strSQL);
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                txtSupplier.Text = dr["NAME"].ToString();
                txtSupplierID.Text = dr["SUPID"].ToString();
                txtAD1.Text = dr["ADR1"].ToString();
                txtAD2.Text = dr["ADR2"].ToString();
                txtZip.Text = dr["ZIP"].ToString();
                txtCountry.Text = dr["COUNTRY"].ToString();
                txtTel.Text = dr["TEL"].ToString();
                txtFax.Text = dr["FAX"].ToString();
                txtCur.Text = dr["Curre"].ToString();
                txtSection.Text = dr["SectID"].ToString();
                txtRemark.Text = dr["POTitle"].ToString();
                txtPOType.Text = dr["POType"].ToString();
                optProduct.SelectedIndex =Convert.ToInt16(dr["Type"]);
            }
            gridControl1.DataSource = ds.Tables[1];
        }
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
            catch{}
                    
    }
        private string[] SearchItemMasterDetail(string strCode)
        {
            string strSQL = "SELECT NAME AS BARCODE,MIXING AS DESCRIPTION FROM XHEAD WHERE CODE='" + strCode + "'";
            DataTable dt = db.GetDataTable(strSQL);
            string[] aryTemp=new string[1];
            if(dt.Rows.Count==0)
            {
                aryTemp[0] = "";
                aryTemp[1] = "";
            }
            else
            {
                aryTemp[0] = dt.Rows[0]["BARCODE"].ToString();
                aryTemp[1] = dt.Rows[0]["DESCRIPTION"].ToString();
            }
            return aryTemp;
        }
        private void CalculateAverageCost()
        {
            string strSQL;
            db.ConnectionOpen();
            for(int i=0;i<gridView1.DataRowCount;i++)
            {
                strSQL = "UPDATE YARNCODE SET COST=(SELECT DBO.UDF_YARNCOSTCALCULATE(" +
                    gridView1.GetRowCellDisplayText(i, "CODE") +
                    ",(SELECT CONVERT(CHAR(7),GETDATE(),120)+'-01')" +
                    ",(SELECT CONVERT(CHAR(10),DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,GETDATE())+1,0)),120)))) " +
                    " WHERE ID=" + gridView1.GetRowCellDisplayText(i, "CODE");
                db.Execute(strSQL);
            }
            db.ConnectionClose();
        }

        #region "Initialize form"
        private void GetUnit()
        {
            string strSQL = "SELECT IDUNIT,UNIT FROM PO_UNIT";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                Units.Add(new Unit { ID = dr["IDUNIT"].ToString(), Name = dr["UNIT"].ToString() });
            }
        }
        private void GetReceive()
        {
            string strSQL = "SELECT RECEIVENO FROM PO_RECEIVE";
            DataTable dt = db.GetDataTable(strSQL);
            sleReceiveNo.Properties.DataSource = dt;
            sleReceiveNo.Properties.DisplayMember = "RECEIVENO";
            sleReceiveNo.Properties.ValueMember = "RECEIVENO";
        }
        private void GetPODetail(string pono)
        {
            string strSQL = "SELECT	A.PONO,CONVERT(CHAR(10),A.PODATE,103) AS PODATE,A.POTYPE,A.IDSUP,"+
                "CONVERT(CHAR(10),A.DELIVERYDATE,103) AS DELIVERYDATE,A.CURRENCYUNIT,A.VAT,A.DEPARTMENT,"+
	            "B.CURCODE,C.NAME AS SECTION,D.NAME AS SUPPLIER,D.ADDRESS1,D.ADDRESS2,D.ZIP,D.COUNTRY,D.TELEPHONE,D.FAX,E.PAYMENT"+
                " FROM PO_PURCHASE A LEFT OUTER JOIN PO_CURRENCY B ON A.CURRENCYUNIT=B.IDCUR "+
	            " LEFT OUTER JOIN PO_DEPARTMENT C ON A.IDDEPT=C.IDDEPT "+
	            " LEFT OUTER JOIN PO_SUPPLIER D ON A.IDSUP=D.IDSUP "+
	            " LEFT OUTER JOIN PO_PAYMENT E ON A.IDPAY=E.IDPAY WHERE A.PONO='"+pono+"'";
            dtHeader = db.GetDataTable(strSQL);
            foreach (DataRow dr in dtHeader.Rows)
            {
                txtDepartment.EditValue = dr["DEPARTMENT"];
                txtPONO.EditValue = dr["PONO"];
                txtPODate.EditValue = dr["PODATE"];
                txtCur.EditValue = dr["CURCODE"];
                txtCur.Tag = dr["CURRENCYUNIT"];
                txtPOType.EditValue = dr["POTYPE"];
                txtSection.EditValue = dr["SECTION"];
                txtSupplierID.EditValue = dr["IDSUP"];
                txtSupplier.EditValue = dr["SUPPLIER"];
                txtAD1.EditValue = dr["ADDRESS1"];
                txtAD2.EditValue = dr["ADDRESS2"];
                txtZip.EditValue = dr["ZIP"];
                txtCountry.EditValue = dr["COUNTRY"];
                txtTel.EditValue = dr["TELEPHONE"];
                txtFax.EditValue = dr["FAX"];
                txtPayterm.EditValue = dr["PAYMENT"];
                dtpReceive.EditValue = DateTime.Today;
                //txtDelivery.EditValue = dr["DELIVERYDATE"];
                txtVat.EditValue = dr["VAT"];
                //strSQL = "SELECT A.LINE,A.PRODUCTCODE AS CODE,A.PRODUCTNAME AS NAME,A.DESCRIPTION,A.QTY,"+
                //    "ISNULL(SUM(C.SCHEDULEQTY),0) AS ACTUAL,B.UNIT,A.UNITPRICE AS PRICE,CONVERT(DECIMAL(19,2),A.QTY*A.UNITPRICE) AS AMOUNT "+  
                //    "FROM PO_PURCHASEDETAIL A LEFT OUTER JOIN PO_UNIT B ON  A.IDUNIT=B.IDUNIT LEFT OUTER JOIN "+
                //    "PO_RECEIVEDETAIL C ON A.PONO=C.PONO AND A.LINE=C.LINE " +
                //    "INNER JOIN PO_RECEIVE D ON C.RECEIVENO=D.RECEIVENO AND D.CANCEL=0 WHERE A.PONO='" + pono + "' " + 
                //    "GROUP BY A.LINE,A.PRODUCTCODE,A.PRODUCTNAME,A.DESCRIPTION,A.QTY,B.UNIT,A.UNITPRICE ORDER BY A.LINE";
                strSQL = "SELECT " +
                            "A.LINE," +
                            "A.PRODUCTCODE AS CODE," +
                            "A.PRODUCTNAME AS NAME," +
                            "A.DESCRIPTION,A.QTY," +
                            "(	SELECT ISNULL(SUM(C.SCHEDULEQTY),0) " +
                                "FROM PO_RECEIVEDETAIL C " +
                                    "INNER JOIN PO_RECEIVE D ON C.RECEIVENO=D.RECEIVENO " +
                                "WHERE C.PONO=A.PONO AND C.LINE=A.LINE AND D.CANCEL=0 " +
                            ")AS ACTUAL," +
                            "B.UNIT," +
                            "A.UNITPRICE AS PRICE," +
                            "CONVERT(DECIMAL(19,2),A.QTY*A.UNITPRICE) AS AMOUNT " +
                        "FROM PO_PURCHASEDETAIL A " +
                            "LEFT OUTER JOIN PO_UNIT B ON  A.IDUNIT=B.IDUNIT " +
                        "WHERE A.PONO='" + pono + "' " +
                        "GROUP BY A.PONO,A.LINE,A.PRODUCTCODE,A.PRODUCTNAME,A.DESCRIPTION,A.QTY,B.UNIT,A.UNITPRICE " +
                        "ORDER BY A.LINE";
                dtDetail = db.GetDataTable(strSQL);
                DataColumn dc = new DataColumn();
                dc.ColumnName = "SCHEDULE";
                dc.DataType = typeof(System.Decimal);
                dtDetail.Columns.Add(dc);
                dc = new DataColumn();
                dc.ColumnName = "RECEIVE";
                dc.DataType = typeof(System.Decimal);
                dtDetail.Columns.Add(dc);

                dtDetail.Columns["SCHEDULE"].SetOrdinal(4);
                dtDetail.Columns["RECEIVE"].SetOrdinal(5);
                gridControl1.DataSource = dtDetail;
                gridView1.Columns["LINE"].Caption = "#";
                gridView1.Columns["LINE"].BestFit();
                gridView1.Columns["LINE"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["CODE"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["NAME"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["DESCRIPTION"].ColumnEdit = gridControl1.RepositoryItems.Add("MemoExEdit") as RepositoryItemMemoExEdit;
                gridView1.Columns["DESCRIPTION"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["UNIT"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["QTY"].Visible = false;
                gridView1.Columns["ACTUAL"].Visible = false;
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    gridView1.SetRowCellValue(i, "SCHEDULE", (decimal)gridView1.GetRowCellValue(i, "QTY") - (decimal)gridView1.GetRowCellValue(i, "ACTUAL"));
                    gridView1.SetRowCellValue(i, "RECEIVE", (decimal)gridView1.GetRowCellValue(i, "QTY") - (decimal)gridView1.GetRowCellValue(i, "ACTUAL"));
                }
                gridView1.Columns["SCHEDULE"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gridView1.Columns["SCHEDULE"].DisplayFormat.FormatString = "n2";
                gridView1.Columns["RECEIVE"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gridView1.Columns["RECEIVE"].DisplayFormat.FormatString = "n2";
                gridView1.Columns["AMOUNT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gridView1.Columns["AMOUNT"].DisplayFormat.FormatString = "n2";
                gridView1.OptionsView.EnableAppearanceEvenRow = true;
                gridView1.OptionsView.EnableAppearanceOddRow = true;
            }
        }
        private void GetReceiveDetail(string receiveNo)
        {
            string strSQL = "SELECT	A.RECEIVEDATE,A.INVOICENO,A.INVOICEDATE,A.DELIVERYNO,A.CANCEL,"+
                "A.DELIVERYDATE,A.PONO,B.PODATE,A.CURRENCYUNIT,C.CURCODE,A.VAT,A.IDSUP,A.DEPARTMENT,"+
                "D.NAME AS SUPPLIER,D.ADDRESS1,D.ADDRESS2,D.TELEPHONE,D.FAX,D.ZIP,D.COUNTRY,"+
                "B.DELIVERYDATE AS TIMEDELIVERY,E.PAYMENT,B.POTYPE,F.NAME AS SECTION "+
                "FROM PO_RECEIVE A LEFT OUTER JOIN PO_PURCHASE B ON A.PONO=B.PONO "+
	            "LEFT OUTER JOIN PO_CURRENCY C ON A.CURRENCYUNIT=C.IDCUR "+
	            "LEFT OUTER JOIN PO_SUPPLIER D ON A.IDSUP=D.IDSUP "+
	            "LEFT OUTER JOIN PO_PAYMENT E ON B.IDPAY=E.IDPAY "+
                "LEFT OUTER JOIN PO_DEPARTMENT F ON B.IDDEPT=F.IDDEPT "+
                "WHERE A.RECEIVENO='"+receiveNo+"'";
            dtHeader = db.GetDataTable(strSQL);
            foreach (DataRow dr in dtHeader.Rows)
            {
                txtDepartment.EditValue = dr["DEPARTMENT"];
                txtPONO.EditValue = dr["PONO"];
                txtPODate.EditValue = dr["PODATE"];
                dtpReceive.EditValue = dr["RECEIVEDATE"];
                txtInvoice.EditValue = dr["INVOICENO"];
                dtpInvoice.EditValue = dr["INVOICEDATE"];
                txtDelivery.EditValue = dr["DELIVERYNO"];
                dtpDelivery.EditValue = dr["DELIVERYDATE"];
                txtCur.EditValue = dr["CURCODE"];
                txtCur.Tag = dr["CURRENCYUNIT"];
                txtPOType.EditValue = dr["POTYPE"];
                txtSection.EditValue = dr["SECTION"];
                chkCancel.Checked = (dr["CANCEL"].ToString()=="1")?true:false;
                txtSupplierID.EditValue = dr["IDSUP"];
                txtSupplier.EditValue = dr["SUPPLIER"];
                txtAD1.EditValue = dr["ADDRESS1"];
                txtAD2.EditValue = dr["ADDRESS2"];
                txtZip.EditValue = dr["ZIP"];
                txtCountry.EditValue = dr["COUNTRY"];
                txtTel.EditValue = dr["TELEPHONE"];
                txtFax.EditValue = dr["FAX"];
                txtPayterm.EditValue = dr["PAYMENT"];
                txtTimeDelivery.EditValue = dr["TIMEDELIVERY"];
                txtVat.EditValue = dr["VAT"];
                strSQL = "SELECT LINE,REMARK FROM PO_RECEIVEREMARK WHERE RECEIVENO='"+receiveNo+"' ORDER BY LINE";
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
                strSQL = "SELECT A.LINE,A.PRODUCTCODE AS CODE,A.PRODUCTNAME AS NAME,A.DESCRIPTION,A.SCHEDULEQTY AS SCHEDULE,A.QTY AS RECEIVE,B.UNIT,"+
	                "A.UNITPRICE AS PRICE,A.AMOUNT "+
                    "FROM PO_RECEIVEDETAIL A LEFT OUTER JOIN PO_UNIT B ON A.IDUNIT=B.IDUNIT "+
                    "WHERE A.RECEIVENO='"+receiveNo+"' ORDER BY A.LINE";
                dtDetail = db.GetDataTable(strSQL);
                gridControl1.DataSource = dtDetail;
                gridView1.PopulateColumns();
                gridView1.Columns["LINE"].Caption = "#";
                gridView1.Columns["LINE"].BestFit();
                gridView1.Columns["LINE"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["CODE"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["NAME"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["DESCRIPTION"].ColumnEdit = gridControl1.RepositoryItems.Add("MemoExEdit") as RepositoryItemMemoExEdit;
                gridView1.Columns["DESCRIPTION"].OptionsColumn.ReadOnly = true;
                gridView1.Columns["SCHEDULE"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gridView1.Columns["SCHEDULE"].DisplayFormat.FormatString = "n2";
                gridView1.Columns["RECEIVE"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gridView1.Columns["RECEIVE"].DisplayFormat.FormatString = "n2";
                gridView1.Columns["AMOUNT"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gridView1.Columns["AMOUNT"].DisplayFormat.FormatString = "n2";

                gridView1.OptionsView.EnableAppearanceEvenRow = true;
                gridView1.OptionsView.EnableAppearanceOddRow = true;

            }
        }
        #endregion

        private void frmTS5_Receive_Load(object sender,EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            txtDepartment.Properties.ReadOnly=true;
            txtPONO.Properties.ReadOnly = true;
            txtPODate.Properties.ReadOnly = true;
            txtCur.Properties.ReadOnly = true;
            txtPOType.Properties.ReadOnly = true;
            txtSection.Properties.ReadOnly = true;
            txtSupplierID.Properties.ReadOnly = true;
            txtSupplier.Properties.ReadOnly = true;
            txtAD1.Properties.ReadOnly = true;
            txtAD2.Properties.ReadOnly = true;
            txtZip.Properties.ReadOnly = true;
            txtCountry.Properties.ReadOnly = true;
            txtTel.Properties.ReadOnly = true;
            txtFax.Properties.ReadOnly = true;
            txtPayterm.Properties.ReadOnly = true;
            txtTimeDelivery.Properties.ReadOnly = true;
            GetUnit();
            GetReceive();
            if (string.IsNullOrEmpty(_strPONO))
                sleReceiveNo.EditValue = _strReceiveNO;
            else
                GetPODetail(_strPONO);
            CalculateTotal();
        }
        //private void cmdSave_Click(object sender,System.EventArgs e)
        //{
        //    try 
        //    {	        
        //        this.Cursor = Cursors.WaitCursor;
        //        db.ConnectionOpen();
        //        db.BeginTrans(IsolationLevel.Serializable);
        //        if(cboReceive.Text.Length == 0)
        //        {
        //            string strReceive = RunReceiveNo();
        //            cboReceive.Text = strReceive;
        //            SaveData(/*strReceive*/);
        //        }
        //        else
        //        {
        //            SaveData(/*cboReceive.Text*/);
        //        }
        //        ChangeStatusTo("EDIT");
        //        db.CommitTrans();
        //        MessageBox.Show("SAVE COMPLETE","Save",MessageBoxButtons.OK,MessageBoxIcon.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        db.RollbackTrans();
        //        if(strStatus == "NEW"){cboReceive.Text = "";}  
        //        MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        db.ConnectionClose();
        //        this.Cursor = Cursors.Default;
        //    }
        //    try 
        //    {	        
        //        if(optProduct.SelectedIndex == 1)   // เลือก yarn
        //        {
        //            CalculateAverageCost();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message,"CalculateAverageCost",MessageBoxButtons.OK,MessageBoxIcon.Error);
        //    }
        //}
        private void cmdCalculate_Click(object sender,EventArgs e)
        {
            CalculateTotal();
        }
        private void gridView1_ValidatingEditor(object sender,DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                switch (gridView1.FocusedColumn.FieldName)
                {
                    case "RECEIVE":
                        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "AMOUNT",
                            String.Format("{0:0,0.00}", Convert.ToDouble(e.Value) * Convert.ToDouble(gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "PRICE"))));
                        break;
                    case "PRICE":
                        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "AMOUNT",
                            String.Format("{0:0,0.00}", Convert.ToDouble(e.Value) * Convert.ToDouble(gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, "RECEIVE"))));
                        break;
                }
                CalculateTotal();
            }
            catch{}
        }
        private void gridControl1_ProcessGridKey(object sender,System.Windows.Forms.KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                if(gridView1.IsEditing==false){gridView1.DeleteSelectedRows();}
            }
        }
        private void sleReceiveNo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtCur.EditValue = null;
                txtPOType.EditValue = null;
                txtSection.EditValue = null;
                chkCancel.Checked = false;

                txtDepartment.EditValue = null;
                txtPONO.EditValue = null;
                txtPODate.EditValue = null;
                dtpReceive.EditValue = DateTime.Today;
                txtInvoice.EditValue = null;
                dtpInvoice.EditValue = null;
                txtDelivery.EditValue = null;
                dtpDelivery.EditValue = null;
                txtRemark1.EditValue = null;
                txtRemark2.EditValue = null;
                txtRemark3.EditValue = null;
                txtTotal.EditValue = null;
                txtVat.EditValue = 7;
                txtGrand.EditValue = null;

                txtSupplierID.EditValue = null;
                txtSupplier.EditValue = null;
                txtAD1.EditValue = null;
                txtAD2.EditValue = null;
                txtZip.EditValue = null;
                txtCountry.EditValue = null;
                txtTel.EditValue = null;
                txtFax.EditValue = null; ;
                txtPayterm.EditValue = null;
                txtTimeDelivery.EditValue = null;

                if (sleReceiveNo.EditValue == null)
                    MessageBox.Show("dd");
                else
                    GetReceiveDetail(sleReceiveNo.EditValue.ToString());
                CalculateTotal();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }
        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            CalculateTotal();
        }

    }
    class Unit
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}