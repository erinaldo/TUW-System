using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using DevExpress.XtraEditors;
using myClass;

namespace TUW_System.TS1
{
    public partial class frmTS1_Receive : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        bool canEditQuantity;//ตัวแปรที่กำหนดว่าจะสามารถแก้ไขตัวเลขในกริดช่อง Quantity ได้ก็ต่่อเมื่อกดปุ่ม Refresh
        bool isFromReceive;//ตัวแปรที่เป็นตัวบอกว่าข้อมูลที่แสดงในกริดเป็นผลมาจากการเลือก ReceiveNo หรือ SupplierCode

        private string _connectionString;
        public string ConnectionString 
        {
            set{_connectionString=value;} 
        }
        public LogIn User_Login { get; set; }

        public frmTS1_Receive()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            try
            {
                if (cboSection.SelectedIndex == -1) throw new ApplicationException("Please select Section");
                if (optMatType.SelectedIndex == -1) throw new ApplicationException("Please select classification (Mat. Type)");

                dtpStart.Enabled = false;
                dtpEnd.Enabled = false;
                optMatType.Enabled = false;
                cboSection.Enabled = false;
                cboRec.Enabled = false;
                optReprint.Enabled = false;

                sleSupplier.EditValueChanged -= sleSupplier_EditValueChanged;
                sleSupplier.EditValue = null;
                sleSupplier.Properties.DataSource = null;
                ClearSupplier();
                sleReceive.EditValueChanged -= sleReceive_EditValueChanged;
                sleReceive.EditValue = null;
                sleReceive.Properties.DataSource = null;
                ClearReceive();
                ClearGridAndTotal();

                canEditQuantity = false;
                isFromReceive = false;
                GetSupplier(optMatType.SelectedIndex.ToString(),cboSection.Text,cboRec.Text);
                sleSupplier.EditValueChanged+=sleSupplier_EditValueChanged;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void EditData()
        {
            dockPanel1.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            try
            {
                if (optReprint.SelectedIndex == -1) throw new System.SystemException("Can edit data on re-print mode");
                GetPO(optMatType.SelectedIndex.ToString(),cboSection.Text,txtSupplierID.Text,cboRec.Text, gridView2, gridControl2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void SaveData()
        { 
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                if (txtSupplierID.Text.Length == 0) { throw new ApplicationException("Please select Supplier code."); }
                if (optCommercial.SelectedIndex == -1) { throw new ApplicationException("Please select Commercial/Non-Commercial"); }
                //check actual=0
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    if ((decimal)gridView1.GetRowCellValue(i, "ACTUAL") <= 0m) 
                    { 
                        throw new ApplicationException("Please input actual > 0"); 
                    }
                }
                //
                bool chCol = false;
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    if ((bool)gridView1.GetRowCellValue(i, "SELECT"))
                    {
                        chCol = true;
                        break;
                    }
                }
                if (!chCol) { throw new ApplicationException("Please select data for save."); }
                //MGenRecNo--------------------------------------------------------------------------------------------------------------
                string xYear = DateTime.Today.ToString("yyyy", dtfinfo);
                string xMonth = DateTime.Today.ToString("MM", dtfinfo);
                string runNo = "";
                string strReceive = "";
                int strRev=0;

                string strSQL = "SELECT * FROM MGenRecNo WHERE GenData ='" + cboRec.Text + "'" +
                    " AND xYear ='" + xYear + "' AND xMonth ='" + xMonth + "'";
                DataTable dt = db.GetDataTable(strSQL);
                if (dt == null || dt.Rows.Count == 0)
                {
                    runNo = "0001";
                    strReceive = cboRec.Text + cUtility.Right(xYear, 2) + xMonth + "-" + runNo;
                    strSQL = "INSERT INTO MGenRecNo(GenData,xYear,xMonth,RunNo,RecNo)Values(" +
                        "'" + cboRec.Text + "','" + xYear + "','" + xMonth + "','" + runNo + "','" + strReceive + "')";
                    db.Execute(strSQL);
                }
                else
                {
                    if (sleReceive.EditValue == null)
                    {
                        runNo = (Convert.ToInt16(dt.Rows[0]["RunNo"]) + 1).ToString().PadLeft(4, '0');
                        strReceive = cboRec.Text + cUtility.Right(xYear, 2) + xMonth + "-" + runNo;
                        strSQL = "UPDATE MGenRecNo SET RunNo='" + runNo + "',RecNo='" + strReceive + "' " +
                            "WHERE GenData='" + cboRec.Text + "' AND xYear='" + xYear + "' AND xMonth='" + xMonth + "'";
                        db.Execute(strSQL);
                    }
                    else
                    {
                        strReceive = sleReceive.EditValue.ToString();
                        strSQL = "SELECT RevNo FROM RHRECEIVE WHERE RecNo ='" + strReceive + "'";
                        string strTemp = db.ExecuteFirstValue(strSQL);
                        strRev = int.Parse(strTemp) + 1;
                        strSQL = "UPDATE RHRECEIVE SET CANORNO=REVNO+1 WHERE RECNO='" + strReceive + "'";
                        db.Execute(strSQL);
                        if (strTemp == "0")
                            strReceive += "-Rev." + strRev;
                        else
                            strReceive = strReceive.Substring(0, 12) + "-Rev." + strRev;
                    }
                }

                //-----------------------------------------------------------------------------------------------------------------------

                if (MessageBox.Show("Do you want to save : " + strReceive + " : (Y/N)", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) throw new ApplicationException();

                strSQL = "SELECT Count(RecNo) FROM RHReceive WHERE RecNo = '" + strReceive + "'";
                if (db.ExecuteFirstValue(strSQL) == "0")
                {
                    strSQL = "INSERT INTO RHReceive (RecNo,RevNo,RecDate,SupCode,Attn,Remarks,InvNo,DelNo,Total,Discount," +
                        "Vat,GTotal,DicTotal,ImpOrOut,ComOrNon,CSVOK,CANORNO,FSECTION,CrtUser,CrtDate, UpdUser,UpdDate) VALUES ("+
                        "'" + strReceive + "',"+
                        "'" + strRev + "',"+
                        "'" + ((DateTime)dtpRecDate.EditValue).ToString("yyyyMMdd") + "',"+
                        "'" + txtSupplierID.Text + "'," +
                        "'" + txtAttn.Text + "'," +
                        "'" + txtRemark.Text + "'," +
                        "'" + txtInvoice.Text + "'," +
                        "'" + txtDelivery.Text + "',"+
                        "'" + txtTotal.Text + "',"+
                        "'" + txtDiscount.Text + "'," +
                        "'" + txtVat.Text + "'," +
                        "'" + txtGrandTotal.Text + "'," +
                        "N'" + lblAmountText.Text + "'," +
                        "'" + cUtility.Right(cboRec.Text, 1) + "','";
                    strSQL += (optCommercial.SelectedIndex == 0) ? "C" : "N";
                    strSQL +=  "',";
                    strSQL += (optMatType.SelectedIndex == 2) ? "'Y'," : "'N',";
                    strSQL +=    "'0',"+
                        "'" + cboSection.Text + "',"+
                        "'" + txtCreateUser.Text + "',"+
                        "'" + txtCreateDate.Text + "'," +
                        "'" + txtUpdateUser.Text + "',"+
                        "'" + txtUpdateDate.Text + "')";
                    db.Execute(strSQL);
                }
                else { }

                strSQL = "DELETE RDReceive WHERE RecNo = '" + strReceive + "'";
                db.Execute(strSQL);

                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    if ((bool)gridView1.GetRowCellValue(i, "SELECT"))
                    {
                        strSQL = "INSERT INTO RDReceive (RecNo,FDate,PONo,OrdNo,SBNo,SBNOKEY,ItemCode,ItemName,Unit,Qty,Uprc,Amt,Scarp," +
                            "AlreadyPO,POKEY,BUN,CSVOK)  VALUES (" +
                            "'" + strReceive + "','" + gridView1.GetRowCellValue(i, "FDATE") + "'," +
                            "'" + gridView1.GetRowCellValue(i, "PONO") + "','" + gridView1.GetRowCellValue(i, "TPICSNO") + "'," +
                            "'" + gridView1.GetRowCellValue(i, "SEIBAN") + "','" + gridView1.GetRowCellValue(i, "SEIBAN") + "'," +
                            "'" + gridView1.GetRowCellValue(i, "PARTNO") + "','" + gridView1.GetRowCellValue(i, "PARTDEL") + "'," +
                            "'" + gridView1.GetRowCellValue(i, "UNIT") + "','" + gridView1.GetRowCellValue(i, "ACTUAL") + "'," +
                            "'" + gridView1.GetRowCellValue(i, "PRICE") + "','" + gridView1.GetRowCellValue(i, "AMOUNT") + "'," +
                            "'0', '1','N' ,'0',";
                        strSQL += (optMatType.SelectedIndex == 2) ? "'Y'" : "'N'";
                        strSQL +=")";
                        db.Execute(strSQL);

                        //กรณีรับนอกแผน เวลารับของไม่ต้อง csv ให้บันทีกค่า Receiveno,seiban no เข้าไปเลย
                        if(optMatType.SelectedIndex==2)
                        {
                            strSQL = "UPDATE XSACT SET RECEIVENO='"+ strReceive+"',CONTRACT='" + gridView1.GetRowCellValue(i, "SEIBAN") +"',ORDERNO='"+txtInvoice.Text+"'"+
                                " WHERE PORDER='"+ gridView1.GetRowCellValue(i, "TPICSNO")+"' AND BUN="+ gridView1.GetRowCellValue(i, "BUN");
                            db.Execute(strSQL);
                            //strSQL = "UPDATE A SET A.RECEIVENO='"+ strReceive+"',A.CONTRACT=B.SBNO,A.ORDERNO='"+txtInvoice.Text+"'"+
                            //    " FROM XSACT A LEFT OUTER JOIN TDPURCHASE B ON A.PORDER=B.PONO AND A.CODE=B.PARTNO"+
                            //    " WHERE A.PORDER='"+ gridView1.GetRowCellValue(i, "TPICSNO")+"' AND A.BUN='"+ gridView1.GetRowCellValue(i, "BUN") +"'";
                            //db.Execute(strSQL);
                        }
                    }
                }

                db.CommitTrans();
                GetReceive(strReceive);
                sleReceive.EditValue = strReceive;
                GetSupplier("","","",txtSupplierID.Text);
                sleSupplier.EditValueChanged -= sleSupplier_EditValueChanged;
                sleSupplier.EditValue = txtSupplierID.Text;
                MessageBox.Show("Save complete", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SystemException ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                db.RollbackTrans();
                if(ex.Message!="Error in the application.") MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
        }
        public void CancelData()
        {
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                if (MessageBox.Show("Do you want to cancel : "+sleReceive.EditValue.ToString(), "Confirm data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) throw new ApplicationException();
                string strSQL=" UPDATE RHReceive SET CANORNO = '1' WHERE RecNo = '"+ sleReceive.EditValue.ToString() +"'";
                db.Execute(strSQL);
                db.CommitTrans();
                MessageBox.Show("Cancel complete", "Cancel", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SystemException ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                db.RollbackTrans();
                if (ex.Message != "Error in the application.") MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
        }
        #region "ClearData"
            private void ClearSupplier()
            {
                txtSupplier.Text = "";
                txtSupplierID.Text = "";
                txtAddress1.Text = "";
                txtAddress2.Text = "";
                txtAddress3.Text = "";
                txtTel.Text = "";
                txtFax.Text = "";
                txtPayTerm.Text = "";
                txtCurrency.Text = "";
            }
            private void ClearReceive()
            {
                txtAttn.Text = "";
                txtFreight.Text = "";
                txtShipVia.Text = "";
                txtRemark.Text = "";
                dtpRecDate.EditValue = DateTime.Today;
                txtInvoice.Text = "";
                txtDelivery.Text = "";
            }
            private void ClearGridAndTotal()
            {
                optCommercial.SelectedIndex = 0;
                gridControl1.DataSource = null;
                lblAmountText.Text = "";
                txtCreateUser.Text = User_Login.UserName;
                txtCreateDate.Text = DateTime.Today.ToString("yyyyMMdd", dtfinfo);
                txtUpdateUser.Text = User_Login.UserName;
                txtUpdateDate.Text = DateTime.Today.ToString("yyyyMMdd", dtfinfo);
                txtTotal.Text = "0";
                txtDiscount.Text = "0";
                txtDiscountAmt.Text = "0";
                txtVat.Text = (cboRec.Text == "I/I") ? "0" : "7";
                txtVatAmt.Text = "0";
                txtGrandTotal.Text = "0";
            }
            public void ClearData()
            {
                dtpStart.EditValue = DateTime.Today;
                //dtpStart.Enabled = true;
                dtpEnd.EditValue = DateTime.Today;
                //dtpEnd.Enabled = true;
                optMatType.SelectedIndex = -1;
                optMatType.Enabled = true;
                cboSection.SelectedIndex = 0;
                cboSection.Enabled = true;
                cboRec.SelectedIndex = 0;
                cboRec.Enabled = true;
                optReprint.SelectedIndexChanged -= optReprint_SelectedIndexChanged;
                optReprint.SelectedIndex = -1;
                optReprint.SelectedIndexChanged += optReprint_SelectedIndexChanged;
                optReprint.Enabled = true;
                
                sleSupplier.EditValueChanged -= sleSupplier_EditValueChanged;
                sleSupplier.EditValue = null;
                sleSupplier.Properties.DataSource = null;
                ClearSupplier();

                sleReceive.EditValueChanged -= sleReceive_EditValueChanged;
                sleReceive.EditValue = null;
                sleReceive.Properties.DataSource = null;
                ClearReceive();

                ClearGridAndTotal();
            }
        #endregion

        public void PrintPreview()
        {
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                cCrystalReport crpPO = new cCrystalReport(Application.StartupPath + @"\Report\TS1\TS1_ReceiveNote.rpt");
                if (crpPO.SetPrinter(4))
                {
                    string strSQL = "UPDATE RHReceive SET FlgPrn='Y' WHERE RecNo='" + sleReceive.EditValue.ToString() + "'";
                    db.Execute(strSQL);
                }
                else
                    throw new ApplicationException("");
                db.CommitTrans();
                
                for (int i = 1; i <= crpPO.ReportCopy; i++)
                {
                    crpPO.ReportTitle = sleReceive.EditValue.ToString();
                    crpPO.ClearParameters();
                    crpPO.SetParameter("Copy", i.ToString());
                    string fmlText = "{RHRECEIVE.RECNO}='" + sleReceive.EditValue.ToString() + "'";
                    crpPO.PrintReport(fmlText, false,"sa","");
                }
            }
            catch (SystemException ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                db.RollbackTrans();
                if (ex.Message != "Error in the application.") MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();

        }
        public void Print()
        {
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                cCrystalReport crpPO = new cCrystalReport(Application.StartupPath + @"\Report\TS1\TS1_ReceiveNote.rpt");
                if (crpPO.SetPrinter(4))
                {
                    string strSQL = "UPDATE RHReceive SET FlgPrn='Y' WHERE RecNo='" + sleReceive.EditValue.ToString() + "'";
                    db.Execute(strSQL);
                }
                else
                    throw new ApplicationException("");
                db.CommitTrans();

                for (int i = 1; i <= crpPO.ReportCopy; i++)
                {
                    crpPO.ReportTitle = sleReceive.EditValue.ToString();
                    crpPO.ClearParameters();
                    crpPO.SetParameter("Copy", i.ToString());
                    string fmlText = "{RHRECEIVE.RECNO}='" + sleReceive.EditValue.ToString() + "'";
                    crpPO.PrintReport(fmlText, true,"sa","");
                }
            }
            catch (SystemException ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException ex)
            {
                db.RollbackTrans();
                if (ex.Message != "Error in the application.") MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
        }
        public void ExportCSV()
        { 
        
        }

        private void GetSupplier(string strMatType,string strSection,string strRecType,string strSupplier="")
        {
            //string strSQL = "";
            //switch (intMatType)
            //{
            //    case 0://Accessory Import
            //        strSQL = "SELECT DISTINCT " +
            //            "A.VENDOR AS 'SUPCODE',C.NAME " +
            //            "FROM XSLIP A " +
            //                "INNER JOIN XHEAD B ON A.CODE=B.CODE " +
            //                "INNER JOIN XSECT C ON A.VENDOR=C.BUMO " +
            //                "INNER JOIN RDRECEIVE D ON A.PONUM=D.PONO AND A.PORDER=D.ORDNO " +
            //                "INNER JOIN RHRECEIVE E ON D.RECNO=E.RECNO " +
            //            "WHERE B.OYAK = '255' " +
            //                "AND C.BUNR LIKE 'I%' " +
            //                "AND E.CANORNO = '0' " +
            //            "GROUP BY A.VENDOR,A.KVOL,C.NAME " +
            //            "HAVING A.KVOL>SUM(D.QTY) " +
            //            "ORDER BY A.VENDOR";
            //        break;
            //    case 1://Accessory Local
            //        strSQL = "SELECT DISTINCT " +
            //            "A.SUPCODE,E.NAME " +
            //            "FROM THPURCHASE A " +
            //                "LEFT OUTER JOIN TDPURCHASE B ON A.PONO=B.PONO " +
            //                "LEFT OUTER JOIN RDRECEIVE C ON A.PONO=C.PONO AND  B.TPICSNO=C.ORDNO " +
            //                "LEFT OUTER JOIN RHRECEIVE D ON C.RECNO=D.RECNO "+
            //                "LEFT OUTER JOIN XSECT E ON A.SUPCODE=E.BUMO " +
            //            "WHERE A.CANORNO='0' "+
            //                "AND A.FLAGMAT='1' "+
            //                "AND A.FSECTION='"+strSection+"' " +
            //                "AND (D.CANORNO IS NULL OR D.CANORNO='0' OR D.CANORNO='X') "+
            //            "GROUP BY A.SUPCODE,B.QTY,D.CANORNO,E.NAME " +
            //            "HAVING SUM(C.QTY) IS NULL OR (D.CANORNO='0' AND SUM(C.QTY)<B.QTY) OR (D.CANORNO='X') " +
            //            "ORDER BY A.SUPCODE";
            //        break;
            //    case 2://Out of Schedule
            //        strSQL = "SELECT DISTINCT A.BUMO AS SUPCODE,B.NAME " +
            //            "FROM XSACT A " +
            //            "LEFT OUTER JOIN XSECT B ON A.BUMO = B.BUMO " +
            //            "WHERE A.AKUBU = 'T' AND A.FLGN = 'N'";
            //        strSQL += (strRecType == "I/D") ? "AND B.BUNR LIKE 'D%' " : "AND D.BUNR LIKE 'I%' ";
            //        break;
            //    case 3://Fabric
            //        strSQL = "SELECT DISTINCT " +
            //            "A.SUPCODE,E.NAME " +
            //            "FROM THPURCHASE A " +
            //                "LEFT OUTER JOIN TDPURCHASE B ON A.PONO=B.PONO " +
            //                "LEFT OUTER JOIN RDRECEIVE C ON A.PONO=C.PONO AND  B.TPICSNO=C.ORDNO " +
            //                "LEFT OUTER JOIN RHRECEIVE D ON C.RECNO=D.RECNO " +
            //                "LEFT OUTER JOIN XSECT E ON A.SUPCODE=E.BUMO " +
            //            "WHERE A.CANORNO='0' "+
            //                "AND A.FLAGMAT='3' "+
            //                "AND A.FSECTION='" + strSection + "' "+
            //                "AND (D.CANORNO IS NULL OR D.CANORNO='0' OR D.CANORNO='X') ";
            //        strSQL += (strRecType == "I/D") ? "AND E.BUNR='D-F' " : "AND E.BUNR='D-I' ";
            //        strSQL+="GROUP BY A.SUPCODE,B.QTY,D.CANORNO,E.NAME " +
            //            "HAVING SUM(C.QTY) IS NULL OR (D.CANORNO='0' AND SUM(C.QTY)<B.QTY) OR (D.CANORNO='X') " +
            //            "ORDER BY A.SUPCODE";
            //        break;
            //}
            string strSQL;
            if (strSupplier == "")
                strSQL = "EXEC spTUWSystem_TS1_Receive_LoadSupplier '" + strMatType + "','" + strSection + "','" + strRecType + "'";
            else
                strSQL = "SELECT BUMO AS SUPCODE,NAME FROM XSECT WHERE BUMO='" + strSupplier + "'";
            DataTable dt = db.GetDataTable(strSQL);
            sleSupplier.Properties.DataSource = dt;
            sleSupplier.Properties.PopulateViewColumns();
            sleSupplier.Properties.DisplayMember = "SUPCODE";
            sleSupplier.Properties.ValueMember = "SUPCODE";
        }
        private void GetSupplierDetail(string strSupplierCode)
        {
            string strSQL = "SELECT NAME,ADR1,ADR2,ZIP,TEL,FAX,PAYTERM,CURRE FROM XSECT A "+
                "WHERE BUMO='" + strSupplierCode + "'";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                txtSupplierID.Text = strSupplierCode;
                txtSupplier.Text = dr["NAME"].ToString();
                txtAddress1.Text = dr["ADR1"].ToString();
                txtAddress2.Text = dr["ADR2"].ToString();
                txtAddress3.Text = dr["ZIP"].ToString();
                txtTel.Text = dr["TEL"].ToString();
                txtFax.Text = dr["FAX"].ToString();
                txtPayTerm.Text = dr["PAYTERM"].ToString();
                txtCurrency.Text = dr["CURRE"].ToString();
            }
        }
        //private string GetLatestReceive(string strGenData)
        //{
        //    string xYear=DateTime.Today.ToString("yyyy",dtfinfo);
        //    string xMonth=DateTime.Today.ToString("MM",dtfinfo);
        //    string runNo="";
        //    string strSQL = "SELECT * FROM MGenRecNo WHERE GenData ='" + strGenData + "'" +
        //        " AND xYear ='" + xYear + "'"+
        //        " AND xMonth ='" + xMonth + "'";
        //    DataTable dt = db.GetDataTable(strSQL);
        //    if (dt==null || dt.Rows.Count == 0)
        //    {
        //        runNo = "0001";
        //    }
        //    else
        //    {
        //        runNo = (Convert.ToInt16(dt.Rows[0]["RunNo"])+1).ToString().PadLeft(4,'0');    
        //    }
        //    string strReceive= strGenData + cUtility.Right(xYear, 2) + xMonth + "-" + runNo;
        //    return strReceive;
        //}
        private void GetReceive(string strReceiveNo="")
        {
            string strSQL;
            if (strReceiveNo == "")
                strSQL = "SELECT RECNO FROM RHRECEIVE WHERE SUBSTRING(RECNO,4,2) IN (" +
                    "'" + DateTime.Today.AddYears(-1).ToString("yy") + "','" + DateTime.Today.ToString("yy") + "')" +
                    " AND CANORNO ='0' ORDER BY RECNO,RECDATE";
            else
                strSQL = "SELECT RECNO FROM RHRECEIVE WHERE RECNO='"+strReceiveNo+"'";
            DataTable dt = db.GetDataTable(strSQL);
            sleReceive.Properties.DataSource = dt;
            sleReceive.Properties.PopulateViewColumns();
            sleReceive.Properties.DisplayMember = "RECNO";
            sleReceive.Properties.ValueMember = "RECNO";
        }
        private void GetReceiveDetail(string strReceiveNo)
        {
            string strSQL = "SELECT CONVERT(DATETIME,RECDATE,112) AS RECDATE,INVNO,DELNO,TOTAL,DISCOUNT,VAT,ATTN,REMARKS,GTOTAL,DICTOTAL,SUPCODE "+
                ",CRTUSER,CRTDATE,UPDUSER,UPDDATE,COMORNON,NAME,ADR1,ADR2,ZIP,TEL,FAX,PAYTERM,CURRE,FSECTION "+
                " FROM RHRECEIVE INNER JOIN XSECT ON RHRECEIVE.SUPCODE=XSECT.BUMO "+
                " WHERE RECNO ='" + strReceiveNo + "'";
            DataTable dt = db.GetDataTable(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                cboSection.SelectedIndex = cboSection.Properties.Items.IndexOf(dr["FSECTION"].ToString());
                dtpRecDate.EditValue = dr["RECDATE"];
                txtInvoice.Text = dr["INVNO"].ToString();
                txtDelivery.Text = dr["DELNO"].ToString();
                txtAttn.Text = dr["ATTN"].ToString();
                txtRemark.Text = dr["REMARKS"].ToString();
                optCommercial.SelectedIndex = optCommercial.Properties.Items.GetItemIndexByValue(dr["COMORNON"]);
                txtTotal.Text = dr["TOTAL"].ToString();
                txtDiscount.Text = dr["DISCOUNT"].ToString();
                txtVat.Text = dr["VAT"].ToString();
                txtGrandTotal.Text = dr["GTOTAL"].ToString();
                lblAmountText.Text = dr["DICTOTAL"].ToString();
                txtCreateUser.Text = dr["CRTUSER"].ToString();
                txtCreateDate.Text = dr["CRTDATE"].ToString();
                txtUpdateUser.Text = dr["UPDUSER"].ToString();
                txtUpdateDate.Text = dr["UPDDATE"].ToString();
                txtSupplierID.Text = dr["SUPCODE"].ToString();
                txtSupplier.Text = dr["NAME"].ToString();
                txtAddress1.Text = dr["ADR1"].ToString();
                txtAddress2.Text = dr["ADR2"].ToString();
                txtAddress3.Text = dr["ZIP"].ToString();
                txtTel.Text = dr["TEL"].ToString();
                txtFax.Text = dr["FAX"].ToString();
                txtPayTerm.Text = dr["PAYTERM"].ToString();
                txtCurrency.Text = dr["CURRE"].ToString();
            }
            strSQL = "SELECT CAST(1 AS BIT) AS 'SELECT',CAST(0 AS BIT) AS 'CANCEL',A.PONO,"+
                "A.ORDNO AS 'TPICSNO',A.ITEMCODE AS 'PARTNO',A.ITEMNAME AS 'PARTDEL',A.UNIT,"+
                "0 AS 'QTY',0 AS 'RECEIVE',0 AS 'REMAIN',A.QTY AS 'ACTUAL',A.UPRC AS 'PRICE',"+
                "A.AMT AS 'AMOUNT',A.FDATE,A.SBNO AS 'SEIBAN',C.FLAGMAT "+
	            "FROM RDReceive A LEFT OUTER JOIN TDPURCHASE B ON A.PONO=A.PONO AND A.ORDNO=B.TPICSNO "+
                "LEFT OUTER JOIN THPURCHASE C ON B.PONO=C.PONO "+
                "WHERE A.RECNO ='"+strReceiveNo+"'";
            dt = db.GetDataTable(strSQL);
            if (dt == null || dt.Rows.Count == 0) return;
            //dt.BeginInit();
            //dt.Columns.Add("SELECT", typeof(System.Boolean),"1");
            //dt.Columns.Add("CANCEL", typeof(System.Boolean), "0");
            //dt.EndInit();
            gridControl1.DataSource = dt;
            gridView1.PopulateColumns();
            gridView1.Columns["SELECT"].Caption = "Select";
            //gridView1.Columns["SELECT"].VisibleIndex = 0;
            gridView1.Columns["CANCEL"].Caption = "Cancel";
            //gridView1.Columns["CANCEL"].VisibleIndex = 1;
            gridView1.Columns["PONO"].Caption = "P/O No.";
            gridView1.Columns["TPICSNO"].Caption = "Order No.";
            gridView1.Columns["PARTNO"].Caption = "Part No.";
            gridView1.Columns["PARTDEL"].Caption = "Part Description";
            gridView1.Columns["UNIT"].Caption = "Unit";
            gridView1.Columns["QTY"].Visible = false;
            gridView1.Columns["RECEIVE"].Visible = false;
            gridView1.Columns["REMAIN"].Caption = "Remain Qty";
            gridView1.Columns["ACTUAL"].Caption = "Actual";
            gridView1.Columns["PRICE"].Caption = "Unit Price";
            gridView1.Columns["AMOUNT"].Caption = "Amount";
            gridView1.Columns["FDATE"].Caption = "FDate";
            gridView1.Columns["SEIBAN"].Caption = "Seiban No.";
            gridView1.Columns["FLAGMAT"].Visible = false;

            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();

            if(strReceiveNo.Substring(0,3)=="I/I")
                optMatType.SelectedIndex = 0;
            else
                optMatType.SelectedIndex = Convert.ToInt16(gridView1.GetRowCellValue(0, "FLAGMAT"));
        }
        private void GetPO(string strMatType,string strSection,string strSupplierCode,string strRecType
            ,DevExpress.XtraGrid.Views.Grid.GridView gridView,DevExpress.XtraGrid.GridControl gridControl)
        {
            canEditQuantity = false;
            gridControl.DataSource = null;
            String strSQL = "EXEC spTUWSystem_TS1_Receive_LoadPO '"+strMatType+"','"+strSection+"','"+ 
                strSupplierCode + "'";
            DataTable dt = db.GetDataTable(strSQL);
            if (dt == null||dt.Rows.Count==0) return;
            dt.Columns.Add("REMAIN", typeof(System.Decimal),"QTY-RECEIVE");
            DataRow[] rows = dt.Select("REMAIN<=0");
            foreach (DataRow dr in rows)
                dr.Delete();
            dt.AcceptChanges();
            gridControl.DataSource = dt;
            gridView.PopulateColumns();
            gridView.Columns["SELECT"].Caption = "Select";
            gridView.Columns["CANCEL"].Caption = "Cancel";
            gridView.Columns["PONO"].Caption = "P/O No.";
            gridView.Columns["PONO"].OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            gridView.Columns["TPICSNO"].Caption = "Order No.";
            gridView.Columns["TPICSNO"].OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            gridView.Columns["PARTNO"].Caption = "Part No.";
            gridView.Columns["PARTNO"].OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            gridView.Columns["PARTDEL"].Caption = "Part Description";
            gridView.Columns["PARTDEL"].OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            gridView.Columns["UNIT"].Caption = "Unit";
            gridView.Columns["QTY"].Visible = false;
            gridView.Columns["RECEIVE"].Visible = false;
            gridView.Columns["REMAIN"].Caption = "Remain Qty";
            gridView.Columns["REMAIN"].VisibleIndex = 7;
            gridView.Columns["ACTUAL"].Caption = "Actual";
            gridView.Columns["PRICE"].Caption = "Unit Price";
            gridView.Columns["AMOUNT"].Caption = "Amount";
            gridView.Columns["FDATE"].Caption = "FDate   ";
            gridView.Columns["SEIBAN"].Caption = "Seiban No.";
            gridView.Columns["SEIBAN"].OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
            gridView.Columns["FLAGMAT"].Visible = false;

            gridView.OptionsView.EnableAppearanceEvenRow = true;
            gridView.OptionsView.EnableAppearanceOddRow = true;
            gridView.OptionsView.ColumnAutoWidth = false;
            gridView.BestFitColumns();
            gridView.Columns["SEIBAN"].Width = 100;
        }
        private void CalculateGrandTotal(decimal total,decimal discount,decimal vat)
        {
            total = decimal.Round(total,2);
            decimal discountAmount = decimal.Round(total * (decimal)0.01 * discount,2);
            txtDiscountAmt.Text = discountAmount.ToString();
            decimal vatAmount = decimal.Round((total - discountAmount) * (decimal)0.01 * vat,2);
            txtVatAmt.Text = vatAmount.ToString();
            decimal grandTotal = (total - discountAmount) + vatAmount;
            txtGrandTotal.Text = grandTotal.ToString();
            lblAmountText.Text = cUtility.ThaiBaht(txtGrandTotal.Text);
        }

        private void optMatType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (optMatType.SelectedIndex)
            { 
                case 0://Accessory import
                    cboRec.SelectedIndex=cboRec.Properties.Items.IndexOf("I/I");
                    cboRec.Properties.ReadOnly = true;
                    break;
                case 1://Accessory local
                    cboRec.SelectedIndex = cboRec.Properties.Items.IndexOf("I/D");
                    cboRec.Properties.ReadOnly = true;
                    break;
                case 2://Out of schedule
                    cboRec.SelectedIndex = cboRec.Properties.Items.IndexOf("I/I");
                    cboRec.Properties.ReadOnly = false;
                    break;
                case 3://Fabric
                    cboRec.SelectedIndex = cboRec.Properties.Items.IndexOf("I/D");
                    cboRec.Properties.ReadOnly = false;
                    break;
            }
        }
        private void frmTS1_Receive_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
            dtfinfo = clinfo.DateTimeFormat;
            cboSection.Properties.Items.Add("SALES1");
            cboSection.Properties.Items.Add("SALES2");
            cboSection.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboRec.Properties.Items.Add("I/I");
            cboRec.Properties.Items.Add("I/D");
            cboRec.Properties.ReadOnly = true;
            cboRec.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        }
        private void sleSupplier_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ClearSupplier();
                ClearGridAndTotal();
                canEditQuantity = false;
                
                GetSupplierDetail(sleSupplier.EditValue.ToString());
                GetPO(optMatType.SelectedIndex.ToString(),cboSection.Text,sleSupplier.EditValue.ToString(),cboRec.Text
                    ,gridView1,gridControl1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }
        private void sleReceive_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ClearReceive();
                ClearGridAndTotal();
                GetReceiveDetail(sleReceive.EditValue.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void sleSupplier_Properties_Popup(object sender, EventArgs e)
        {
            sleSupplierView.BestFitColumns();
        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                gridView1.SetRowCellValue(i, "SELECT", true);
            }
        }
        private void btnUnSelect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                gridView1.SetRowCellValue(i, "SELECT", false);
            }
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            for (int i = gridView1.RowCount - 1; i >= 0; i--)
            {
                if ((bool)gridView1.GetRowCellValue(i, "SELECT") == false) gridView1.DeleteRow(i);
            }
            gridView1.BestFitColumns();
            gridView1.Columns["SEIBAN"].Width = 100;
            canEditQuantity = true;
        }
        private void btnCancelTable_Click(object sender, EventArgs e)
        {
            for (int i = gridView1.RowCount - 1; i >= 0; i--)
            {
                if ((bool)gridView1.GetRowCellValue(i, "CANCEL") == true) gridView1.DeleteRow(i);
            }
        }
        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (canEditQuantity)
                e.Cancel = false;
            else
                e.Cancel=(gridView1.FocusedColumn.FieldName=="SELECT")?false:true;
        }
        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
                e.Info.ImageIndex = -1;
            }
            gridView1.IndicatorWidth = 40;
        }
        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                int row = gridView1.FocusedRowHandle;
                switch (gridView1.FocusedColumn.FieldName)
                {
                    case "ACTUAL":
                        var amount = decimal.Round(Convert.ToDecimal(e.Value) * Convert.ToDecimal(gridView1.GetRowCellValue(row, "PRICE")),2);
                        gridView1.SetRowCellValue(row, "AMOUNT",amount);
                        gridView1.SetRowCellValue(row,"FDATE",((DateTime)dtpRecDate.EditValue).ToString("yyyyMMdd",dtfinfo));
                        break;
                }
                var total = 0.00m;
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    total +=Convert.ToDecimal(gridView1.GetRowCellValue(i, "AMOUNT"));
                }
                txtTotal.Text = total.ToString();
                CalculateGrandTotal(Convert.ToDecimal(total), Convert.ToDecimal(txtDiscount.Text), Convert.ToDecimal(txtVat.Text));
            }
            catch { }
        }
        private void txtDiscount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CalculateGrandTotal(Convert.ToDecimal(txtTotal.Text),Convert.ToDecimal(txtDiscount.Text),Convert.ToDecimal(txtVat.Text));
            }
            catch {}
        }
        private void txtVat_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CalculateGrandTotal(Convert.ToDecimal(txtTotal.Text), Convert.ToDecimal(txtDiscount.Text), Convert.ToDecimal(txtVat.Text));
            }
            catch{}
        }
        private void optReprint_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //sleSupplier.EditValue = null;
                //sleSupplier.Properties.DataSource = null;
                //sleReceive.EditValue = null;
                //sleReceive.Properties.DataSource = null;
                //gridControl1.DataSource = null;
                
                canEditQuantity = false;
                isFromReceive = true;
                GetReceive();
                sleReceive.EditValueChanged += sleReceive_EditValueChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dockPanel1_ClosingPanel(object sender, DevExpress.XtraBars.Docking.DockPanelCancelEventArgs e)
        {
            gridView2.CloseEditor();
            gridView2.UpdateCurrentRow();
            for (int i = 0; i < gridView2.RowCount; i++)
            {
                if ((bool)gridView2.GetRowCellValue(i, "SELECT"))
                {
                    gridView1.AddNewRow();
                    for(int j=0;j<gridView2.Columns.Count;j++)
                    {
                        gridView1.SetRowCellValue(gridView1.FocusedRowHandle,gridView1.Columns[j].FieldName,gridView2.GetRowCellValue(i,gridView2.Columns[j].FieldName) );
                    }
                    gridView1.CloseEditor();
                    gridView1.UpdateCurrentRow();
                }
            }
        }
        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (canEditQuantity)
            {
                DevExpress.XtraGrid.Views.Grid.GridView gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo = gridView.CalcHitInfo(e.Location);
                if (hitInfo.Column == null || hitInfo.Column.FieldName != "ACTUAL") return;
                this.BeginInvoke(new MethodInvoker(delegate { gridView.ShowEditorByMouse(); }));
            }
        }

    }
}