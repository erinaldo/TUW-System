using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Globalization;
using System.Threading;
using Microsoft.Office.Interop.Excel;
using myClass;

namespace TUW_System
{
    public partial class frmP_Production : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db = new cDatabase(Module.Sale);
        CultureInfo clinfo = new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        System.Data.DataTable dtSize,dtQty,dtCost,dtSample;
        string strPorder;//production order ใหม่ที่ได้จากการรันใน database
        string strImagePath="";//เก็บทั้งตำแหน่งและชื่อไฟล์
        string strImageFileNameBeforeSave="";//ใช้เก็บเฉพาะตอนโหลดรูปเท่านั้น
        const int intNumberOfSizes = 8;//จำนวน size ที่มีได้สูงสุดเท่ากับจำนวนฟิลด์ size ในฐานข้อมูล
        const int intFirstColumnSize = 4;//คอลัมน์แรกของตำแหน่ง size

        public frmP_Production()
        {
            InitializeComponent();
        }

        public void NewData()
        {
            ClearForm(clearPrimaryKey: true);
            //cboProduction.Properties.Items.Clear();
            //cboProduction.Properties.AutoComplete = false;//ถ้าอยู่ในโหมด new เวลาพิมพ์หมายเลข production order ไม่ต้อง autocomplete
        }
        public void EditData()
        {
            ClearForm(clearPrimaryKey: true);
            LoadProductionOrder();
        }
        private void DisplayData(string strProduction,string strRevise)
        {
            //Size-----------------------------------------------------------------------------------------------------------------------------------------------------------
            string strSQL = "SELECT SIZE_CUST,SIZE_TUW FROM XMAIN_SIZE WHERE PORDER='"+strProduction+"' AND REVISE="+strRevise+" ORDER BY LINE";
            dtSize = db.GetDataTable(strSQL);
            dtSize.Columns["SIZE_CUST"].Caption = "Size";
            dtSize.Columns["SIZE_TUW"].Caption = "TUW Size";
            gridSize.DataSource = dtSize;
            gridViewSize.OptionsView.EnableAppearanceEvenRow = true;
            gridViewSize.OptionsView.EnableAppearanceOddRow = true;
            gridViewSize.OptionsView.ColumnAutoWidth = false;
            gridViewSize.BestFitColumns();
            //Main
            strSQL = "SELECT A.*,B.ARTICLE FROM XMAIN A LEFT OUTER JOIN XMODEL B ON A.MID=B.MID " +
                "WHERE A.PORDER='"+strProduction+"' AND REVISE="+strRevise;
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                if (dr["DATEISSUE"] != DBNull.Value) { dtpDate.EditValue = Convert.ToDateTime(dr["DATEISSUE"]); }
                if (dr["INCHARGE"] != DBNull.Value) { txtIncharge.Text = dr["INCHARGE"].ToString(); }
                if (dr["MERCHANDISER"] != DBNull.Value) { txtMerchadiser.Text = dr["MERCHANDISER"].ToString(); }
                if (dr["INQUIRY_MEMO"] != DBNull.Value) { txtInquiry.Text = dr["INQUIRY_MEMO"].ToString(); }
                if (dr["CUST"] != DBNull.Value) { cboCustomer.EditValue = dr["CUST"].ToString(); }
                if (dr["MID"] != DBNull.Value) { cboModel.EditValue = dr["MID"].ToString(); }
                if (dr["ORDERNO"] != DBNull.Value) { txtOrderNo.Text = dr["ORDERNO"].ToString(); }
                if (dr["CONTRACT"] != DBNull.Value) { txtContractNo.Text = dr["CONTRACT"].ToString(); }
                if (dr["ARTICLE"] != System.DBNull.Value) { txtArticle.Text = dr["ARTICLE"].ToString(); }
                if (dr["QTY"] != DBNull.Value) { txtQuantity.Text = dr["QTY"].ToString(); }
                if (dr["SHIPMENT"] != DBNull.Value) { dtpShipment.EditValue = Convert.ToDateTime(dr["SHIPMENT"]); }
                if (dr["EXFACTORY"] != DBNull.Value) { dtpExFty.EditValue = Convert.ToDateTime(dr["EXFACTORY"]); }
                if (dr["SEASON"] != DBNull.Value) { txtSeason.Text = dr["SEASON"].ToString(); }
                if (dr["REMARK"] != DBNull.Value) { txtRemark.Text = dr["REMARK"].ToString(); }
                //if (dr["PLANNING_BY"] != DBNull.Value) { txtBy.Text = dr["PLANNING_BY"].ToString(); }
                //if (dr["SEWING_FROM"] != DBNull.Value) { dtpSeiwngFrom.EditValue = Convert.ToDateTime(dr["SEWING_FROM"]); }
                //if (dr["SEWING_TO"] != DBNull.Value) { dtpSewingTo.EditValue = Convert.ToDateTime(dr["SEWING_TO"]); }
                //if (dr["SEWING_LINE"] != DBNull.Value) { txtSetUpLine.Text = dr["SEWING_LINE"].ToString(); }
                //if (dr["SEWING_DAY_LINE"] != DBNull.Value) { txtSewDayLine.Text = dr["SEWING_DAY_LINE"].ToString(); }
                //if (dr["SEWING_TIME"] != DBNull.Value) { txtSewingTime.Text = dr["SEWING_TIME"].ToString(); }
                if (dr["FABRIC1"] != DBNull.Value) { txtFabric1.Text = dr["FABRIC1"].ToString(); }
                if (dr["FABRIC2"] != DBNull.Value) { txtFabric2.Text = dr["FABRIC2"].ToString(); }
                if (dr["FABRIC3"] != DBNull.Value) { txtFabric3.Text = dr["FABRIC3"].ToString(); }
                if (dr["SIZE_SET"] != DBNull.Value) { chkSizeSetStatus.Checked = Convert.ToBoolean(dr["SIZE_SET"]); }
                if (dr["SIZE_SET_DATE"] != DBNull.Value) { dtpSizeSetStatus.EditValue = Convert.ToDateTime(dr["SIZE_SET_DATE"]); }
                if (Convert.ToBoolean(dr["TOP_SAMPLE"]) == true)
                {
                    optTopSample.SelectedIndex = 0;
                }
                else
                {
                    optTopSample.SelectedIndex = 1;
                }
                if (Convert.ToBoolean(dr["SHIPMENT_SAMPLE"]) == true)
                {
                    optShipmentSample.SelectedIndex = 0;
                }
                else
                {
                    optShipmentSample.SelectedIndex = 1;
                }
                strImagePath = dr["IMAGE"].ToString();
                strImageFileNameBeforeSave = "";
                if (dr["IMAGE"] != DBNull.Value)
                {
                    pictureBox1.ImageLocation = dr["IMAGE"].ToString();
                }
            }
            //Spec----------------------------------------------------------------------------------------------------------------------------------------------------------
            strSQL = "SELECT * FROM XMAIN_SPEC WHERE PORDER='"+strProduction+"' AND REVISE="+strRevise;
            dt = db.GetDataTable(strSQL);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr1 in dt.Rows)
                {
                    switch (dr1["LINE"].ToString())
                    {
                        case "1":
                            txtSpecName1.Text = dr1["SPEC"].ToString();
                            txtSpec1.Text = dr1["SPEC_LENGTH"].ToString();
                            break;
                        case "2":
                            txtSpecName2.Text = dr1["SPEC"].ToString();
                            txtSpec2.Text = dr1["SPEC_LENGTH"].ToString();
                            break;
                        case "3":
                            txtSpecName3.Text = dr1["SPEC"].ToString();
                            txtSpec3.Text = dr1["SPEC_LENGTH"].ToString();
                            break;
                        case "4":
                            txtSpecName4.Text = dr1["SPEC"].ToString();
                            txtSpec4.Text = dr1["SPEC_LENGTH"].ToString();
                            break;
                        case "5":
                            txtSpecName5.Text = dr1["SPEC"].ToString();
                            txtSpec5.Text = dr1["SPEC_LENGTH"].ToString();
                            break;
                        case "6":
                            txtSpecName6.Text = dr1["SPEC"].ToString();
                            txtSpec6.Text = dr1["SPEC_LENGTH"].ToString();
                            break;
                    }
                }
            }
            //Detail---------------------------------------------------------------------------------------------------------------------------------------------------------
            SetGrid();
            strSQL = "SELECT SEQ,COLOR_CUST,COLOR_TUW,SHADE FROM XMAIN_COLOR WHERE PORDER='"+strProduction+"' AND REVISE="+strRevise+" ORDER BY LINE";
            dt = db.GetDataTable(strSQL);
            if (dt != null && dt.Rows.Count > 0)
            {
                //Qty
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow drQty = dtQty.NewRow();
                    drQty["SEQ"] = dr["SEQ"];
                    drQty["COLOR_CUST"] = dr["COLOR_CUST"];
                    drQty["COLOR_TUW"] = dr["COLOR_TUW"];
                    drQty["SHADE"] = dr["SHADE"];
                    dtQty.Rows.Add(drQty);
                }
                //Cost
                dtCost = dtQty.Copy();
                //Sample
                dtSample = dtQty.Copy();
            }
            strSQL = "SELECT LINE_COLOR,LINE_SIZE,QTY,COST,SAMPLE FROM XMAIN_DETAIL WHERE PORDER='"+strProduction+"' AND REVISE="+strRevise;
            dt = db.GetDataTable(strSQL);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dtQty.Rows[Convert.ToInt16(dr["LINE_COLOR"])][dr["LINE_SIZE"].ToString()] = dr["QTY"].ToString();
                    dtCost.Rows[Convert.ToInt16(dr["LINE_COLOR"])][dr["LINE_SIZE"].ToString()] = dr["COST"].ToString();
                    dtSample.Rows[Convert.ToInt16(dr["LINE_COLOR"])][dr["LINE_SIZE"].ToString()] = dr["SAMPLE"].ToString();
                }
            }
            gridControl1.DataSource = dtQty;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            //gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            gridControl2.DataSource = dtCost;
            gridView2.OptionsView.EnableAppearanceEvenRow = true;
            gridView2.OptionsView.EnableAppearanceOddRow = true;
            gridView2.OptionsView.ColumnAutoWidth = false;
            gridView2.BestFitColumns();
            gridControl3.DataSource = dtSample;
            gridView3.OptionsView.EnableAppearanceEvenRow = true;
            gridView3.OptionsView.EnableAppearanceOddRow = true;
            gridView3.OptionsView.ColumnAutoWidth = false;
            gridView3.BestFitColumns();
        }
        public void DisplayData2()
        {
            DisplayData(sleProduction.EditValue.ToString(),txtRevise.Text);
        }
        public void SaveData()
        {
            string strSQL;
            if(strImagePath.Length>0){SaveImage();}
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();
            gridView2.CloseEditor();
            gridView2.UpdateCurrentRow();
            gridView3.CloseEditor();
            gridView3.UpdateCurrentRow();
            db.ConnectionOpen();
            //run production on ใหม่กรณีช่อง sleProduction ไม่มีค่าแสดงว่าเป็นข้อมูลใหม่
            if (sleProduction.EditValue == null)
            {
                strSQL = "EXEC SPPRODUCTIONORDER_RUNPORDER";
                strPorder = db.ExecuteFirstValue(strSQL);//สร้าง strPorder มารับค่า porder ใหม่เพราะถ้าใส่ไปที่ sleproduction ตรงๆจะทำให้เกิด event editvaluechanged
                txtRevise.Text = "0";
            }
            else
            {
                strPorder = sleProduction.EditValue.ToString();
            }
            try
            {
                db.BeginTrans();
                //Main-----------------------------------------------------------------------------------------------------------------------------------------------------
                strSQL = "SELECT COUNT(PORDER) FROM XMAIN WHERE PORDER='" + strPorder + "' AND REVISE="+txtRevise.Text;
                if (db.ExecuteFirstValue(strSQL) == "0")
                {
                    //Insert
                    strSQL = "INSERT INTO XMAIN(PORDER,REVISE,CUST,MID,ORDERNO,CONTRACT,QTY,SHIPMENT,EXFACTORY,SEASON,DATEISSUE" +
                        ",INCHARGE,MERCHANDISER,INQUIRY_MEMO"+
                        ",FABRIC1,FABRIC2,FABRIC3,SIZE_SET,SIZE_SET_DATE,TOP_SAMPLE,SHIPMENT_SAMPLE,REMARK,IMAGE,INPUTUSER)VALUES(" +
                        "'" + strPorder + "',"+txtRevise.Text+",'" + cboCustomer.EditValue + "','" + cboModel.EditValue + "','" + txtOrderNo.Text + "','" + txtContractNo.Text + "'";
                    if (txtQuantity.Text.Trim().Length == 0) { strSQL += ",0"; } else { strSQL += "," + txtQuantity.Text; }
                    if (dtpShipment.EditValue == null)
                    {
                        strSQL += ",null";
                    }
                    else
                    {
                        strSQL += ",'" + ((DateTime)dtpShipment.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    }
                    if (dtpExFty.EditValue == null)
                    {
                        strSQL += ",null";
                    }
                    else
                    {
                        strSQL += ",'" + ((DateTime)dtpExFty.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    }
                    strSQL += ",'" + txtSeason.Text.Replace("'", "''") + "'";
                    if (dtpDate.EditValue == null)
                    {
                        strSQL += ",null";
                    }
                    else
                    {
                        strSQL += ",'" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    }
                    strSQL += ",'" + txtIncharge.Text + "','" + txtMerchadiser.Text + "',N'" + txtInquiry.Text + "'";
                    //if (dtpSeiwngFrom.EditValue == null)
                    //{
                    //    strSQL += ",null";
                    //}
                    //else
                    //{
                    //    strSQL += ",'" + ((DateTime)dtpSeiwngFrom.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    //}
                    //if (dtpSewingTo.EditValue == null)
                    //{
                    //    strSQL += ",null";
                    //}
                    //else
                    //{
                    //    strSQL += ",'" + ((DateTime)dtpSewingTo.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    //}
                    //strSQL += ",'" + txtSetUpLine.Text + "','" + txtSewDayLine.Text + "','" + txtSewingTime.Text + "'";
                    strSQL += ",'" + txtFabric1.Text + "','" + txtFabric2.Text + "','" + txtFabric3.Text + "'";
                    if (chkSizeSetStatus.Checked == true) { strSQL += ",1"; } else { strSQL += ",0"; }
                    if (dtpSizeSetStatus.EditValue == null)
                    {
                        strSQL += ",null";
                    }
                    else
                    {
                        strSQL += ",'" + ((DateTime)dtpSizeSetStatus.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    }
                    if (optTopSample.SelectedIndex == 0) { strSQL += ",1"; } else { strSQL += ",0"; }
                    if (optShipmentSample.SelectedIndex == 0) { strSQL += ",1"; } else { strSQL += ",0"; }
                    strSQL += ",N'" + txtRemark.Text + "'";
                    if (strImagePath.Length == 0)
                    {
                        strSQL += ",null";
                    }
                    else
                    {
                        strSQL += ",'" + strImagePath + "'";
                    }
                    strSQL += ",'" + System.Environment.MachineName + "')";
                }
                else
                {
                    //Update
                    strSQL = "UPDATE XMAIN SET "+"CUST='" + cboCustomer.EditValue + "'" +
                        ",MID='" + cboModel.EditValue + "'" +
                        ",ORDERNO='" + txtOrderNo.Text + "'" +
                        ",CONTRACT='" +strPorder.Replace("P","E") + "'";
                    if (txtQuantity.Text.Trim().Length == 0)
                    {
                        strSQL += ",QTY=0";
                    }
                    else
                    {
                        strSQL += ",QTY='" + txtQuantity.Text + "'";
                    }

                    if (dtpShipment.EditValue == null)
                    {
                        strSQL += ",SHIPMENT=null";
                    }
                    else
                    {
                        strSQL += ",SHIPMENT='" + ((DateTime)dtpShipment.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    }
                    if (dtpExFty.EditValue == null)
                    {
                        strSQL += ",EXFACTORY=null";
                    }
                    else
                    {
                        strSQL += ",EXFACTORY='" + ((DateTime)dtpExFty.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    }
                    strSQL += ",SEASON='" + txtSeason.Text.Replace("'", "''") + "'";
                    if (dtpDate.EditValue == null)
                    {
                        strSQL += ",DATEISSUE=null";
                    }
                    else
                    {
                        strSQL += ",DATEISSUE='" + ((DateTime)dtpDate.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    }
                    strSQL += ",INCHARGE='" + txtIncharge.Text + "',MERCHANDISER='" + txtMerchadiser.Text + "'" +
                        ",INQUIRY_MEMO=N'" + txtInquiry.Text + "',REMARK=N'" + txtRemark.Text + "'";
                    //if (dtpSeiwngFrom.EditValue == null)
                    //{
                    //    strSQL += ",SEWING_FROM=null";
                    //}
                    //else
                    //{
                    //    strSQL += ",SEWING_FROM='" + ((DateTime)dtpSeiwngFrom.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    //}
                    //if (dtpSewingTo.EditValue == null)
                    //{
                    //    strSQL += ",SEWING_TO=null";
                    //}
                    //else
                    //{
                    //    strSQL += ",SEWING_TO='" + ((DateTime)dtpSewingTo.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    //}
                    //strSQL += ",SEWING_LINE='" + txtSetUpLine.Text + "',SEWING_DAY_LINE='" + txtSewDayLine.Text + "'" +
                    //    ",SEWING_TIME='" + txtSewingTime.Text + "'";
                    strSQL += ",FABRIC1='" + txtFabric1.Text + "',FABRIC2='" + txtFabric2.Text + "',FABRIC3='" + txtFabric3.Text + "'";
                    if (chkSizeSetStatus.Checked == true) { strSQL += ",SIZE_SET=1"; } else { strSQL += ",SIZE_SET=0"; }
                    if (dtpSizeSetStatus.EditValue == null)
                    {
                        strSQL += ",SIZE_SET_DATE=null";
                    }
                    else
                    {
                        strSQL += ",SIZE_SET_DATE='" + ((DateTime)dtpSizeSetStatus.EditValue).ToString("yyyy-MM-dd", dtfinfo) + "'";
                    }
                    if (optTopSample.SelectedIndex == 0)
                    { strSQL += ",TOP_SAMPLE=1"; }
                    else { strSQL += ",TOP_SAMPLE=0"; }
                    if (optShipmentSample.SelectedIndex == 0)
                    { strSQL += ",SHIPMENT_SAMPLE=1"; }
                    else { strSQL += ",SHIPMENT_SAMPLE=0"; }
                    if (strImagePath.Length == 0)
                    {
                        strSQL += ",IMAGE=null";
                    }
                    else
                    {
                        strSQL += ",IMAGE='" + strImagePath + "'";
                    }
                    strSQL += ",INPUTDATE=GETDATE(),INPUTUSER='" + System.Environment.MachineName + "'";
                    strSQL += " WHERE PORDER='" + strPorder + "' AND REVISE="+txtRevise.Text;
                }
                db.Execute(strSQL);
                //Size--------------------------------------------------------------------------------------------------------------------------------------------------------
                strSQL = "DELETE FROM XMAIN_SIZE WHERE PORDER='" + strPorder + "' AND REVISE="+txtRevise.Text;
                db.Execute(strSQL);
                for (int i = 0; i < gridViewSize.DataRowCount; i++)
                {
                    strSQL = "INSERT INTO XMAIN_SIZE(PORDER,REVISE,LINE,SIZE_CUST,SIZE_TUW,INPUTUSER)VALUES(" +
                        "'" + strPorder + "',"+txtRevise.Text+"," + i + ",'" + gridViewSize.GetRowCellDisplayText(i, "SIZE_CUST") + "'" +
                        ",'" + gridViewSize.GetRowCellDisplayText(i, "SIZE_TUW") + "','" + System.Environment.MachineName + "')";
                    db.Execute(strSQL);
                }
                //SPEC-------------------------------------------------------------------------------------------------------------------------------------------------------
                strSQL = "DELETE FROM XMAIN_SPEC WHERE PORDER='"+strPorder+"' AND REVISE="+txtRevise.Text;
                db.Execute(strSQL);
                if (txtSpecName1.Text.Trim().Length > 0)
                {
                    if (txtSpec1.Text.Trim().Length == 0) { txtSpec1.Text = "0"; }
                    strSQL = "INSERT INTO XMAIN_SPEC(PORDER,REVISE,SPEC,SPEC_LENGTH,LINE)VALUES(" +
                    "'" + strPorder + "',"+txtRevise.Text+",'" + txtSpecName1.Text + "'," + txtSpec1.Text + ",1)";
                    db.Execute(strSQL);
                }
                if (txtSpecName2.Text.Trim().Length > 0)
                {
                    if (txtSpec2.Text.Trim().Length == 0) { txtSpec2.Text = "0"; }
                    strSQL = "INSERT INTO XMAIN_SPEC(PORDER,REVISE,SPEC,SPEC_LENGTH,LINE)VALUES(" +
                    "'" + strPorder + "',"+txtRevise.Text+",'" + txtSpecName2.Text + "'," + txtSpec2.Text + ",2)";
                    db.Execute(strSQL);
                }
                if (txtSpecName3.Text.Trim().Length > 0)
                {
                    if (txtSpec3.Text.Trim().Length == 0) { txtSpec3.Text = "0"; }
                    strSQL = "INSERT INTO XMAIN_SPEC(PORDER,REVISE,SPEC,SPEC_LENGTH,LINE)VALUES(" +
                    "'" + strPorder + "',"+txtRevise.Text+",'" + txtSpecName3.Text + "'," + txtSpec3.Text + ",3)";
                    db.Execute(strSQL);
                }
                if (txtSpecName4.Text.Trim().Length > 0)
                {
                    if (txtSpec4.Text.Trim().Length == 0) { txtSpec4.Text = "0"; }
                    strSQL = "INSERT INTO XMAIN_SPEC(PORDER,REVISE,SPEC,SPEC_LENGTH,LINE)VALUES(" +
                    "'" + strPorder + "',"+txtRevise.Text+",'" + txtSpecName4.Text + "'," + txtSpec4.Text + ",4)";
                    db.Execute(strSQL);
                }
                if (txtSpecName5.Text.Trim().Length > 0)
                {
                    if (txtSpec5.Text.Trim().Length == 0) { txtSpec5.Text = "0"; }
                    strSQL = "INSERT INTO XMAIN_SPEC(PORDER,REVISE,SPEC,SPEC_LENGTH,LINE)VALUES(" +
                    "'" + strPorder + "',"+txtRevise.Text+",'" + txtSpecName5.Text + "'," + txtSpec5.Text + ",5)";
                    db.Execute(strSQL);
                }
                if (txtSpecName6.Text.Trim().Length > 0)
                {
                    if (txtSpec6.Text.Trim().Length == 0) { txtSpec6.Text = "0"; }
                    strSQL = "INSERT INTO XMAIN_SPEC(PORDER,REVISE,SPEC,SPEC_LENGTH,LINE)VALUES(" +
                    "'" + strPorder + "',"+txtRevise.Text+",'" + txtSpecName6.Text + "'," + txtSpec6.Text + ",6)";
                    db.Execute(strSQL);
                }
                //Color----------------------------------------------------------------------------------------------------------------------
                strSQL = "DELETE FROM XMAIN_COLOR WHERE PORDER='" + strPorder + "' AND REVISE="+txtRevise.Text;
                db.Execute(strSQL);
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    strSQL = "INSERT INTO XMAIN_COLOR(PORDER,REVISE,LINE,SEQ,COLOR_CUST,COLOR_TUW,SHADE,INPUTUSER)VALUES(" +
                        "'" + strPorder + "',"+txtRevise.Text+"," + i + ",'" + gridView1.GetRowCellDisplayText(i, "SEQ") + "'" +
                        ",'" + gridView1.GetRowCellDisplayText(i, "COLOR_CUST") + "','" + gridView1.GetRowCellDisplayText(i, "COLOR_TUW") + "'" +
                        ",'" + gridView1.GetRowCellDisplayText(i, "SHADE") + "','" + System.Environment.MachineName + "')";
                    db.Execute(strSQL);
                }
                //Detail----------------------------------------------------------------------------------------------------------------------
                strSQL = "DELETE FROM XMAIN_DETAIL WHERE PORDER='" + strPorder + "' AND REVISE="+txtRevise.Text;
                db.Execute(strSQL);
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    for (int j = 0; j < gridViewSize.DataRowCount; j++)
                    {
                        if ((Convert.ToDecimal(gridView1.GetRowCellValue(i, j.ToString())) == 0) &&
                            (Convert.ToDecimal(gridView2.GetRowCellValue(i, j.ToString())) == 0) &&
                            (Convert.ToDecimal(gridView3.GetRowCellValue(i, j.ToString())) == 0))
                        { continue; }
                        strSQL = "INSERT INTO XMAIN_DETAIL(PORDER,REVISE,LINE_COLOR,LINE_SIZE,QTY,COST,SAMPLE,INPUTUSER)VALUES(" +
                        "'" + strPorder + "',"+txtRevise.Text+"," + i + "," + j +
                        "," + Convert.ToDecimal(gridView1.GetRowCellValue(i, j.ToString())) +
                        "," + Convert.ToDecimal(gridView2.GetRowCellValue(i, j.ToString())) +
                        "," + Convert.ToDecimal(gridView3.GetRowCellValue(i, j.ToString())) +
                        ",'" + System.Environment.MachineName + "')";
                        db.Execute(strSQL);
                    }
                }

                db.CommitTrans();
                MessageBox.Show("Save complete...", "Production Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadProductionOrder();
                sleProduction.EditValue = strPorder;
            }

            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
        }
        public void SaveAsData()
        {
            try
            {
                DialogResult result = MessageBox.Show("คุณต้องการเซฟ production order นี้ไปเป็นอันใหม่ใช่หรือไม่", "Save as", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    sleProduction.EditValue = null;
                    SaveData();
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ExportExcel()
        {
            try
            {
                Thread currentThread = System.Threading.Thread.CurrentThread;
                currentThread.CurrentCulture = clinfo;
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel._Workbook xlBook = xlApp.Workbooks.Open(System.Windows.Forms.Application.StartupPath + "\\Report\\Blank Form_New Format.xlt", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                Microsoft.Office.Interop.Excel._Worksheet xlSheet = (Microsoft.Office.Interop.Excel._Worksheet)xlApp.ActiveSheet;
                Microsoft.Office.Interop.Excel.Shape xlShape;
                int excelCurrentRow = 7;

                xlSheet.get_Range("J3").Value2 = "NO." + sleProduction.EditValue.ToString();
                xlSheet.get_Range("S3").Value2 = txtIncharge.Text;
                xlSheet.get_Range("U2").Value2 = ((DateTime)dtpDate.EditValue).ToString("dd-MMM-yyyy", dtfinfo);//ต้องส่งค่าวันที่เป็นแบบอังกฤษเข้าไป จะสามารถแปลงสลัยได้ทั้งแบบอังกฤษและไทย
                xlSheet.get_Range("U2").NumberFormat = "B1dd-MMM-yyyy";
                xlSheet.get_Range("U3").Value2 = txtMerchadiser.Text;

                xlSheet.get_Range("B5").Value2 = cboCustomer.Text;
                xlSheet.get_Range("B6").Value2 = cboModel.Text;
                xlSheet.get_Range("B7").Value2 = txtOrderNo.Text;
                xlSheet.get_Range("B8").Value2 = txtContractNo.Text;
                xlSheet.get_Range("B9").Value2 = txtArticle.Text;
                xlSheet.get_Range("B10").Value2 = txtQuantity.Text;
                if (dtpShipment.EditValue != null)
                {
                    xlSheet.get_Range("B11").Value2 = ((DateTime)dtpShipment.EditValue).ToString("MMMM dd, yyyy", dtfinfo);
                    xlSheet.get_Range("B11").NumberFormat = "B1MMMM dd, yyyy";
                }
                xlSheet.get_Range("B12").Value2 = txtSeason.Text;

                //xlSheet.get_Range("D13").Value2 = "BY : " + txtBy.Text;
                //if (dtpSeiwngFrom.EditValue != null)
                //{
                //    xlSheet.get_Range("B15").Value2 = ((DateTime)dtpSeiwngFrom.EditValue).ToString("MMMM dd, yyyy", dtfinfo);
                //    xlSheet.get_Range("B15").NumberFormat = "B1MMMM dd, yyyy";
                //}
                //if (dtpSewingTo.EditValue != null)
                //{
                //    xlSheet.get_Range("D15").Value2 = "to  " + ((DateTime)dtpSewingTo.EditValue).ToString("MMMM dd, yyyy", dtfinfo);
                //}
                //xlSheet.get_Range("B17").Value2 = txtSetUpLine.Text;
                //xlSheet.get_Range("B19").Value2 = txtSewDayLine.Text;
                //xlSheet.get_Range("B21").Value2 = txtSewingTime.Text;

                xlSheet.get_Range("G6").Value2 = txtSpecName1.Text;
                xlSheet.get_Range("G7").Value2 = txtSpecName2.Text;
                xlSheet.get_Range("G8").Value2 = txtSpecName3.Text;
                xlSheet.get_Range("G9").Value2 = txtSpecName4.Text;
                xlSheet.get_Range("G10").Value2 = txtSpecName5.Text;
                xlSheet.get_Range("G11").Value2 = txtSpecName6.Text;
                xlSheet.get_Range("H6").Value2 = txtSpec1.Text;
                xlSheet.get_Range("H7").Value2 = txtSpec2.Text;
                xlSheet.get_Range("H8").Value2 = txtSpec3.Text;
                xlSheet.get_Range("H9").Value2 = txtSpec4.Text;
                xlSheet.get_Range("H10").Value2 = txtSpec5.Text;
                xlSheet.get_Range("H11").Value2 = txtSpec6.Text;

                xlSheet.get_Range("G15").Value2 = txtFabric1.Text;
                xlSheet.get_Range("G16").Value2 = txtFabric2.Text;
                xlSheet.get_Range("G17").Value2 = txtFabric3.Text;

                if (chkSizeSetStatus.Checked == true)
                {
                    xlShape = xlSheet.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeOval, 458, 252, 58, 14);
                    xlShape.Fill.Transparency = 1;
                }
                if (optTopSample.SelectedIndex == 0)
                {
                    xlShape = xlSheet.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeOval, 458, 278, 41, 14);
                    xlShape.Fill.Transparency = 1;
                }
                else
                {
                    xlShape = xlSheet.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeOval, 505, 278, 41, 14);
                    xlShape.Fill.Transparency = 1;
                }
                if (pictureBox1.Image != null)
                {
                    xlShape = xlSheet.Shapes.AddPicture(strImagePath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 540, 75, 125, 100);
                }

                for (int j = intFirstColumnSize; j < gridView1.Columns.Count; j++)
                {
                    xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[5, j + 10], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[5, j + 10]).Value2 = gridView1.Columns[j].GetTextCaption();
                }
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    for (int j = 1; j < gridView1.Columns.Count; j++)
                    {
                        if (gridView1.GetRowCellValue(i, gridView1.Columns[j].FieldName).ToString() == "0") { continue; }
                        xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, j + 10], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, j + 10]).Value2 = gridView1.GetRowCellValue(i, gridView1.Columns[j].FieldName);
                    }
                    excelCurrentRow += 2;
                }
                //Top sample
                excelCurrentRow = 17;
                for (int i = 0; i < gridView3.DataRowCount; i++)
                {
                    for (int j = 2; j < gridView3.Columns.Count; j++)
                    {
                        if (gridView3.GetRowCellValue(i, gridView3.Columns[j].FieldName).ToString() == "0") { continue; }
                        xlSheet.get_Range((Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, j + 10], (Microsoft.Office.Interop.Excel.Range)xlSheet.Cells[excelCurrentRow, j + 10]).Value2 = gridView3.GetRowCellValue(i, gridView3.Columns[j].FieldName);
                    }
                    excelCurrentRow++;
                }




                xlApp.Visible = true;
                this.Cursor = Cursors.WaitCursor;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export excel error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }





        }

        public void ClearForm(bool clearPrimaryKey)
        {
            strImagePath = "";
            strImageFileNameBeforeSave = "";
            pictureBox1.ImageLocation = null;
            foreach (Control ctl in layoutControl1.Controls)
            {
                if (clearPrimaryKey==false)
                {
                    //if (ctl.Name == "cboProduction") { continue; }//clearPrimaryKey ถ้าเป็น false หมายถึง จะไม่มีการเคลียค่าใน cboProduction 
                    if (ctl.Name == "sleProduction"||ctl.Name=="txtRevise") //clearPrimaryKey ถ้าเป็น false หมายถึง จะไม่มีการเคลียค่าใน sleProduction และ txtRevise
                    { 
                        continue; 
                    }
                }
                switch (ctl.GetType().ToString())
                {
                    case "DevExpress.XtraEditors.ComboBoxEdit":
                        ((DevExpress.XtraEditors.ComboBoxEdit)ctl).Text = "";
                        break;
                    case "DevExpress.XtraEditors.TextEdit":
                        ((DevExpress.XtraEditors.TextEdit)ctl).Text = "";
                        break;
                    case "DevExpress.XtraEditors.DateEdit":
                        ((DevExpress.XtraEditors.DateEdit)ctl).EditValue = null;
                        break;
                    case "DevExpress.XtraEditors.CheckEdit":
                        ((DevExpress.XtraEditors.CheckEdit)ctl).Checked = false;
                        break;
                    case "DevExpress.XtraGrid.GridControl":
                        ((DevExpress.XtraGrid.GridControl)ctl).DataSource = null;
                        break;
                    case "DevExpress.XtraEditors.GridLookUpEdit":
                        ((DevExpress.XtraEditors.GridLookUpEdit)ctl).EditValue = null;
                        break;
                    case "DevExpress.XtraEditors.SearchLookUpEdit":
                        ((DevExpress.XtraEditors.SearchLookUpEdit)ctl).EditValue = null;
                        break;
                    case "DevExpress.XtraEditors.RadioGroup":
                        ((DevExpress.XtraEditors.RadioGroup)ctl).EditValue = null;
                        break;
                    case "DevExpress.XtraEditors.MemoEdit":
                        ((DevExpress.XtraEditors.MemoEdit)ctl).EditValue = null;
                        break;

                }
            }

            //Grid Size
            dtSize = new System.Data.DataTable();
            dtSize.Columns.Add(AddDataColumn("SIZE_CUST","Customer Size", typeof(System.String)));
            dtSize.Columns.Add(AddDataColumn("SIZE_TUW", "TUW Size", typeof(System.String)));
            gridSize.DataSource = dtSize;
            gridViewSize.OptionsView.EnableAppearanceEvenRow = true;
            gridViewSize.OptionsView.EnableAppearanceOddRow = true;
            gridViewSize.OptionsView.ColumnAutoWidth = false;
            gridViewSize.BestFitColumns();
            //Main detail--------------------------------------------------------------------------------------------------
            dtQty = new System.Data.DataTable();
            dtQty.Columns.Add(AddDataColumn("SEQ", "No", typeof(System.String)));
            dtQty.Columns.Add(AddDataColumn("COLOR_CUST", " Customer Color", typeof(System.String)));
            dtQty.Columns.Add(AddDataColumn("COLOR_TUW", "TUW Color", typeof(System.String)));
            dtQty.Columns.Add(AddDataColumn("SHADE", "Shade", typeof(System.String)));
            gridControl1.DataSource = dtQty;
            gridView1.PopulateColumns();
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            dtCost = new System.Data.DataTable();
            dtCost.Columns.Add(AddDataColumn("SEQ", "No", typeof(System.String)));
            dtCost.Columns.Add(AddDataColumn("COLOR_CUST", "Customer Color", typeof(System.String)));
            dtCost.Columns.Add(AddDataColumn("COLOR_TUW", "TUW Color", typeof(System.String)));
            dtCost.Columns.Add(AddDataColumn("SHADE", "Shade", typeof(System.String)));
            gridControl2.DataSource = dtCost;
            gridView2.PopulateColumns();
            gridView2.OptionsView.EnableAppearanceEvenRow = true;
            gridView2.OptionsView.EnableAppearanceOddRow = true;
            gridView2.OptionsView.ColumnAutoWidth = false;
            gridView2.BestFitColumns();
            dtSample = new System.Data.DataTable();
            dtSample.Columns.Add(AddDataColumn("SEQ", "No", typeof(System.String)));
            dtSample.Columns.Add(AddDataColumn("COLOR_CUST", "Customer Color", typeof(System.String)));
            dtSample.Columns.Add(AddDataColumn("COLOR_TUW", "TUW Color", typeof(System.String)));
            dtSample.Columns.Add(AddDataColumn("SHADE", "Shade", typeof(System.String)));
            gridControl3.DataSource = dtSample;
            gridView3.PopulateColumns();
            gridView3.OptionsView.EnableAppearanceEvenRow = true;
            gridView3.OptionsView.EnableAppearanceOddRow = true;
            gridView3.OptionsView.ColumnAutoWidth = false;
            gridView3.BestFitColumns();
        }

        private void LoadProductionOrder()
        { 
            string strSQL = "SELECT TOP 1000 PORDER,REVISE FROM XMAIN ORDER BY INPUTDATE DESC";
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            sleProduction.Properties.DataSource = dt;
            sleProduction.Properties.DisplayMember = "PORDER";
            sleProduction.Properties.ValueMember = "PORDER";
            
        }
        private void LoadCustomer()
        {
            string strSQL = "SELECT CUST,NAME FROM XCUSTOMER";
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            cboCustomer.Properties.DataSource = dt;
            cboCustomer.Properties.PopulateViewColumns();
            cboCustomer.Properties.DisplayMember = "NAME";
            cboCustomer.Properties.ValueMember = "CUST";
            cboCustomer.Properties.View.Columns["CUST"].Visible = false;
            
        }
        private void LoadModel()
        {
            string strSQL = "SELECT MID,MODEL,ARTICLE FROM XMODEL";
            System.Data.DataTable dt = db.GetDataTable(strSQL);
            cboModel.Properties.DataSource = dt;
            cboModel.Properties.PopulateViewColumns();
            cboModel.Properties.DisplayMember = "MODEL";
            cboModel.Properties.ValueMember = "MID";
            cboModel.Properties.View.Columns["MID"].Visible = false;
            cboModel.Properties.View.Columns["ARTICLE"].Visible = false;
        }
        private void DrawColumnHeaderGridForSize()//ฟังก์ชั่นในการวาดหัวคอลัมน์ที่เป็นส่วนของ size ใน gridviewdetail
        {
            try
            {
                for (int i = 0; i < intNumberOfSizes; i++)
                {
                    if (i >= gridViewSize.DataRowCount)
                    {
                        gridView1.Columns["SIZE" + i.ToString()].Caption = " ";
                    }
                    else
                    {
                        gridView1.Columns["SIZE" + i.ToString()].Caption = gridViewSize.GetRowCellDisplayText(i, "SIZE_CUST");
                    }
                }
            }
            catch { }
        }
        private void MoveCellFocus(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (!gv.IsNewItemRow(gv.FocusedRowHandle))
            {
                //SendKeys.Send("{RIGHT}");
                if (gv.FocusedColumn.VisibleIndex == gv.Columns.Count - 1)
                {
                    gv.FocusedRowHandle++;
                    gv.FocusedColumn = gv.VisibleColumns[0];
                }
                else
                {
                    gv.FocusedColumn = gv.VisibleColumns[gv.FocusedColumn.VisibleIndex + 1];
                }
                //gv.ShowEditor();
            }
        }
        private void SetGrid()
        {
            dtQty = new System.Data.DataTable();
            dtQty.Columns.Add(AddDataColumn("SEQ", "No", typeof(System.String)));
            dtQty.Columns.Add(AddDataColumn("COLOR_CUST", "Customer Color", typeof(System.String)));
            dtQty.Columns.Add(AddDataColumn("COLOR_TUW", "TUW Color", typeof(System.String)));
            dtQty.Columns.Add(AddDataColumn("SHADE", "Shade", typeof(System.String)));
            for (int i = 0; i < gridViewSize.DataRowCount; i++)
            {
                dtQty.Columns.Add(AddDataColumn(i.ToString(), gridViewSize.GetRowCellDisplayText(i, "SIZE_TUW"), typeof(System.Decimal), 0));
            }
            gridControl1.DataSource = dtQty;
            gridView1.PopulateColumns();
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            dtCost = dtQty.Copy();
            gridControl2.DataSource = dtCost;
            gridView2.PopulateColumns();
            gridView2.OptionsView.EnableAppearanceEvenRow = true;
            gridView2.OptionsView.EnableAppearanceOddRow = true;
            gridView2.OptionsView.ColumnAutoWidth = false;
            gridView2.BestFitColumns();
            dtSample = dtQty.Copy();
            gridControl3.DataSource = dtSample;
            gridView3.PopulateColumns();
            gridView3.OptionsView.EnableAppearanceEvenRow = true;
            gridView3.OptionsView.EnableAppearanceOddRow = true;
            gridView3.OptionsView.ColumnAutoWidth = false;
            gridView3.BestFitColumns();
        }

        #region "Manage image"
        public void LoadImage()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Choose image...";
            dlg.Filter = "Image File (*.jpg;*.bmp;*.gif;*.tif;*.png)|*.jpg;*.bmp;*.gif;*.tif;*.png";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                strImagePath = dlg.FileName;
                strImageFileNameBeforeSave = dlg.SafeFileName;
                pictureBox1.ImageLocation = dlg.FileName;
            }
        }
        private void SaveImage()
        {
            string strDestination;
            if (strImageFileNameBeforeSave.Length > 0)
            {
                strDestination = "\\\\192.168.101.3\\Sales\\ProductionOrder_Images\\" + strImageFileNameBeforeSave;
            }
            else
            {
                strDestination = strImagePath;
            }
            if (System.IO.File.Exists(strDestination) == false) { System.IO.File.Copy(strImagePath, strDestination, true); }
            strImagePath = strDestination;
            strImageFileNameBeforeSave = "";
        }
        public void DeleteImage()
        {
            if (MessageBox.Show("คุณต้องการลบรูปสินค้า(Illustration)ใช่หรือไม่", "Illustration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (strImageFileNameBeforeSave.Length == 0)//ถ้ามีค่าแสดงว่าอยู่ในขั้นตอนการโหลดรูป ห้ามลบ เพราะ strImagePath เก็บค่าไฟล์ต้นฉบับอยู่
                {
                    System.IO.File.Delete(strImagePath);
                }
                strImagePath = "";
                strImageFileNameBeforeSave = "";
                pictureBox1.Image = null;
            }
        }
        #endregion
        #region "AddDataColumn"
        private DataColumn AddDataColumn(string columnName,string caption,Type dataType)
        {
            DataColumn dc = new DataColumn();
            dc.ColumnName = columnName;
            dc.Caption = caption;
            dc.DataType = dataType;
            return dc;
        }
        private DataColumn AddDataColumn(string columnName, string caption, Type dataType,object defaultValue)
        {
            DataColumn dc = new DataColumn();
            dc.ColumnName = columnName;
            dc.Caption = caption;
            dc.DataType = dataType;
            dc.DefaultValue = defaultValue;
            return dc;
        }
        #endregion

        private void frmProduction_Load(object sender, EventArgs e)
        {
            dtfinfo = clinfo.DateTimeFormat;
            LoadProductionOrder();
            LoadCustomer();
            LoadModel();
            ClearForm(true);
        }
        private void btnModel_Click(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmMain = (DevExpress.XtraEditors.XtraForm)MdiParent;
            ((mdiMain)frmMain).LoadfrmP_Model();
        }
        private void btnCustomer_Click(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.XtraForm frmMain = (DevExpress.XtraEditors.XtraForm)MdiParent;
            ((mdiMain)frmMain).LoadfrmP_Customer();
        }
        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (e.Column.AbsoluteIndex >= intFirstColumnSize)
            {
                if (Convert.ToDecimal(e.Value) == 0) { e.DisplayText = ""; }
            }
        }
        private void gridView2_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (e.Column.AbsoluteIndex >= intFirstColumnSize)
            {
                if (Convert.ToDecimal(e.Value) == 0) { e.DisplayText = ""; }
            }
        }
        private void gridView3_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (e.Column.AbsoluteIndex >= intFirstColumnSize)
            {
                if (Convert.ToDecimal(e.Value) == 0) { e.DisplayText = ""; }
            }
        }
        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            //ปรับข้อมูลในกริดให้เหมือนกันทั้งสามตัว
            if (gv.FocusedColumn.AbsoluteIndex < intFirstColumnSize)
            {
                gridView2.SetRowCellValue(gv.FocusedRowHandle, gv.FocusedColumn.FieldName, gv.EditingValue);
                gridView3.SetRowCellValue(gv.FocusedRowHandle, gv.FocusedColumn.FieldName, gv.EditingValue);
            }
            
        }
        private void gridView2_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (gv.FocusedColumn.AbsoluteIndex < intFirstColumnSize)
            {
                gridView1.SetRowCellValue(gv.FocusedRowHandle, gv.FocusedColumn.FieldName, gv.EditingValue);
                gridView3.SetRowCellValue(gv.FocusedRowHandle, gv.FocusedColumn.FieldName, gv.EditingValue);
            }
        }
        private void gridView3_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (gv.FocusedColumn.AbsoluteIndex < intFirstColumnSize)
            {
                gridView1.SetRowCellValue(gv.FocusedRowHandle, gv.FocusedColumn.FieldName, gv.EditingValue);
                gridView2.SetRowCellValue(gv.FocusedRowHandle, gv.FocusedColumn.FieldName, gv.EditingValue);
            }
        }
        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && gridView1.IsEditing == false)
            {
                if (MessageBox.Show("คุณต้องการลบข้อมูลบรรทัดนี้หรือไม่", "Delete row", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int rowFocused = gridView1.FocusedRowHandle;
                    gridView1.DeleteRow(rowFocused);
                    gridView2.DeleteRow(rowFocused);
                    gridView3.DeleteRow(rowFocused);
                    e.Handled = true;
                }
            }

        }
        private void gridControl2_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && gridView2.IsEditing == false)
            {
                if (MessageBox.Show("คุณต้องการลบข้อมูลบรรทัดนี้หรือไม่", "Delete row", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int rowFocused = gridView2.FocusedRowHandle;
                    gridView1.DeleteRow(rowFocused);
                    gridView2.DeleteRow(rowFocused);
                    gridView3.DeleteRow(rowFocused);
                    e.Handled = true;
                }
            }
        }
        private void gridControl3_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && gridView3.IsEditing == false)
            {
                if (MessageBox.Show("คุณต้องการลบข้อมูลบรรทัดนี้หรือไม่", "Delete row", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int rowFocused = gridView3.FocusedRowHandle;
                    gridView1.DeleteRow(rowFocused);
                    gridView2.DeleteRow(rowFocused);
                    gridView3.DeleteRow(rowFocused);
                    e.Handled = true;
                }
            }
        }
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            e.Column.BestFit();
        }
        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            e.Column.BestFit();
        }
        private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            e.Column.BestFit();
        }
        private void btnCustomer_refresh_Click(object sender, EventArgs e)
        {
            LoadCustomer();
        }
        private void btnModel_refresh_Click(object sender, EventArgs e)
        {
            LoadModel();
        }
        private void txtSpec1_MouseEnter(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit txt = (DevExpress.XtraEditors.TextEdit)sender;
            txt.SelectAll();
        }
        private void txtSpec2_MouseEnter(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit txt = (DevExpress.XtraEditors.TextEdit)sender;
            txt.SelectAll();
        }
        private void txtSpec3_MouseEnter(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit txt = (DevExpress.XtraEditors.TextEdit)sender;
            txt.SelectAll();
        }
        private void txtSpec4_MouseEnter(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit txt = (DevExpress.XtraEditors.TextEdit)sender;
            txt.SelectAll();
        }
        private void txtSpec5_MouseEnter(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit txt = (DevExpress.XtraEditors.TextEdit)sender;
            txt.SelectAll();
        }
        private void txtSpec6_MouseEnter(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit txt = (DevExpress.XtraEditors.TextEdit)sender;
            txt.SelectAll();
        }
        private void gridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar== (char)Keys.Return)
            {
                MoveCellFocus(sender);
            }
        }
        private void gridView2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                MoveCellFocus(sender);
            }
        }
        private void gridView3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                MoveCellFocus(sender);
            }
        }
        private void cboModel_EditValueChanged(object sender, EventArgs e)
        {
            if (cboModel.EditValue == null) { return; }
            txtArticle.Text = cboModel.Properties.View.GetRowCellDisplayText(cboModel.Properties.View.FocusedRowHandle,"ARTICLE");
        }
        private void sleProduction_EditValueChanged(object sender, EventArgs e)
        {
            if (sleProduction.EditValue == null) { return; }
            txtRevise.Text = sleProduction.Properties.View.GetRowCellDisplayText(sleProduction.Properties.View.FocusedRowHandle, "REVISE");
            try
            {
                ClearForm(clearPrimaryKey: false);
                DisplayData(sleProduction.EditValue.ToString(),txtRevise.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSetGrid_Click(object sender, EventArgs e)
        {
            SetGrid();
        }

    }
}