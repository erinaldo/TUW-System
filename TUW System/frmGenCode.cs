using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Globalization;
using System.Collections;
using myClass;

namespace TUW_System
{
    public partial class frmGenCode : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db=new cDatabase(Module.Fabric);
        cDatabase db2=new cDatabase(Module.Sewing);
        CultureInfo clinfo=new CultureInfo("en-US");
        DateTimeFormatInfo dtfinfo;
        DataSet dsItemMaster;
        Hashtable htCalendar;

        public frmGenCode()
        {
            InitializeComponent();
        }

        private void LoadItemMasterConfig()
        {
            try 
	        {	        
		        dsItemMaster=new DataSet();
                dsItemMaster.ReadXml(Application.StartupPath+"\\ItemMasterConfig.xml");
	        }
	        catch (Exception ex)
	        {
		        MessageBox.Show(ex.Message,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
	        }
        }
        private void LoadCalendar()
        {
            string strSQL= "SELECT * FROM XCALE WHERE CALENO=0 ORDER BY CALENAME;SELECT * FROM XCALE WHERE CALENO=1 ORDER BY CALENAME";
            DataSet ds=db.GetDataSet(strSQL);
            htCalendar=new Hashtable();
            for(int i=0;i<ds.Tables[0].Rows.Count;i++)
            {
                DataRow dr=ds.Tables[0].Rows[i];
                for(int j=1;j<=Convert.ToInt16(dr["DAYN"]);j++)
                {
                    htCalendar.Add(dr["WDAYNUM"+j],ds.Tables[1].Rows[i]["WDAYNUM"+j]);
                }
            }

        }
        private int CalculateWorkDay(DateTime datFrom,DateTime datTo)
        {
            TimeSpan diffResult=datFrom-datTo;
            int intPeriod=diffResult.Days;
            int intWorkday=-1;
            for(int i=0;i<=intPeriod;i++)
            {
                DateTime datTemp=datFrom.AddDays(i);
                string strTemp=datTemp.ToString("yyyyMMdd",dtfinfo);
                if(!htCalendar.ContainsKey(strTemp))
                {
                    MessageBox.Show("ไม่พบวันใน Work Calendar","Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                    return 0;
                }
                else
                {
                    if(htCalendar[strTemp].ToString()=="1")
                    {
                        intWorkday+=1;
                    }
                }
            }
            return intWorkday-1; //พี่หนิงขอเพิ่มพิเศษให้ลบออกจากที่คำนวนได้อีก 1
        }
        private string SearchInterCode(string strOrder)
        {
            string strSQL= "SELECT MAX(CODE) AS CODE2 FROM XSLIP WHERE XSLIP.PONUM='"+strOrder+"'";
            db2.ConnectionOpen();
            string strCode2=db2.ExecuteFirstValue(strSQL);
            db2.ConnectionClose();
            return strCode2;
        }
        private void ClearGridView(DevExpress.XtraGrid.Views.Grid.GridView g)
        {
            for(int i=g.RowCount-1;i>=0;i--)
            {
                g.DeleteRow(i);
            }
        }
        private void SearchYarn(string strFabricCode)
        {
            cDatabase db3=new cDatabase(Module.TUW99);
            string strSQL= "SELECT RIGHT('0000'+CONVERT(VARCHAR,YARNCODE),5) AS CODE,YARNNAME AS NAME,(PERCENTUSE/100) AS PERCENT_USE"+
                ",WEIGHT,RIGHT('000'+CONVERT(VARCHAR,YARNSUPPLIER.ID),4) AS SUP_ID"+
                " FROM FabricRawYarn INNER JOIN FabricRawYarnDetail ON FabricRawYarn.Code = FabricRawYarnDetail.Code INNER JOIN "+
                "YarnCode ON FabricRawYarnDetail.YarnCode = YarnCode.Id LEFT OUTER JOIN "+
                "YarnSupplier ON YarnCode.Supplier = YarnSupplier.SupplierName "+
                "WHERE FABRICRAWYARN.CODE='"+ strFabricCode +"'";
            DataTable dt=db3.GetDataTable(strSQL);
            gridYarn.DataSource=dt;
            txtWeight.Text =gridYarnView.GetRowCellDisplayText(0, "WEIGHT");
            gridYarnView.Columns["WEIGHT"].Visible = false;
            gridYarnView.OptionsView.EnableAppearanceEvenRow = true;
            gridYarnView.OptionsView.EnableAppearanceOddRow = true;
            gridYarnView.OptionsView.ColumnAutoWidth = false;
            gridYarnView.BestFitColumns();
        }
        private string SearchFabricID(string strFabricCode)
        {
            cDatabase db3=new cDatabase(Module.TUW99);
            string strSQL="SELECT RIGHT('0000'+CONVERT(VARCHAR,ID),5) AS ID FROM GREYFABRIC WHERE CODE='"+ strFabricCode+"'";
            db3.ConnectionOpen();
            string strTemp=db3.ExecuteFirstValue(strSQL);
            //================Search Machine===================================
            strSQL = "SELECT KnittingWeekly.[Size],KnittingWeekly.G FROM KnittingWeekly INNER JOIN "+
                       "KnittingWeeklyDetail ON KnittingWeekly.MachineNo = KnittingWeeklyDetail.MachineNo "+
                       "WHERE (KnittingWeeklyDetail.OrderNo = N'"+txtOrder.Text+"') "+
                       "And (KnittingWeeklyDetail.Fabric=N'"+txtFabric.Text+"')";
            DataTable dt=db3.GetDataTable(strSQL);
            if(dt.Rows.Count>0)
            {
                string strTemp2=dt.Rows[0]["SIZE"].ToString().Trim();
                strSQL = "SELECT BUMO FROM XSECT WHERE SUBSTRING(NAME,3,2)='"+
                    strTemp2.Substring(strTemp2.Length-3, 3).Substring(0, 2)+
                    "' AND SUBSTRING(NAME,11,2)='"+dt.Rows[0]["G"].ToString().Substring(0, 2)+"'";
                txtMachine.Text = db.ExecuteFirstValue(strSQL);
            }
            db3.ConnectionClose();
            return strTemp;
        }
        private StringBuilder InsertToMixing()//ฟังก์ชั่นในการนำเอาค่าชื่อเส้นด้ายและเปอร์เซนต์การใช้ใส่ลงในฟิลด์ Mixing เฉพาะ KNT เท่านั้น
        {
            StringBuilder strTemp=new StringBuilder(254);
            for(int i=0;i<gridYarnView.DataRowCount;i++)
            {
                strTemp.Append(gridYarnView.GetRowCellDisplayText(i, "NAME") + " - " + (Convert.ToDecimal(gridYarnView.GetRowCellValue(i, "PERCENT_USE")) * 100).ToString() + "%");
                strTemp.AppendLine();
            }
            return strTemp;
        }
        private void GenCode()
        {
            string strItemCode = "";
            string strItemCodeDye = "";
            DataTable dtItem = new DataTable();
            DataTable dtBOM=new DataTable();
            //order no จำนวน 8 หลัก
            switch (cboDivision.SelectedIndex)
            {
                case 0://Sales1,2
                    strItemCode = strItemCode + "I" + txtOrder.Text.Substring(txtOrder.Text.Length - 6, 6);//  string.Right(txtOrder.Text, 6);
                    break;
                case 1://Sales3
                    strItemCode = strItemCode + "R" + txtOrder.Text.Substring(txtOrder.Text.Length - 6, 6); //string.Right(txtOrder.Text, 6);
                    break;
                case 2://Salses4
                    strItemCode = strItemCode + "P" + txtOrder.Text.Substring(txtOrder.Text.Length - 6, 6);//string.Right(txtOrder.Text, 6);
                    break;
                case 3://Salses5
                    strItemCode = strItemCode + "F" + txtOrder.Text.Substring(0, 4);   //string.Left(txtOrder.Text , 4);
                    break;
                case 4://Sales6
                    strItemCode = strItemCode + "O" + txtOrder.Text.Substring(0, 4);// string.Left(txtOrder.Text, 4);
                    break;
                case 5://Riki Garment
                    strItemCode = strItemCode + "G" + txtOrder.Text.Substring(0, 4);
                    break;
                case 6://Thai Parfun
                    strItemCode = strItemCode + "T" + txtOrder.Text.Substring(0, 4);
                    break;
                case 7://Thai Parfun sai4
                    strItemCode = strItemCode + "S" + txtOrder.Text.Substring(0, 4);
                    break;
            }
            switch (cboDivision.SelectedIndex)
            {
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    strItemCode += ("000" + txtBranch.Text).Substring(("000" + txtBranch.Text).Length - 3, 3);
                    break;
                default:
                    strItemCode += txtBranch.Text;
                    break;
            }
            //order add เพิ่ม X ข้างหน้าจากนั้นตัดให้เหลือ 8 ตัว
            if (chkAdd.Checked)
            {
                strItemCode = ("X" + strItemCode).Substring(0, 8);
            }
            //เพิ่ม - กับ รหัสผ้า รวมเป็น 6 หลัก
            strItemCode += "-" + txtFabricID.Text;
            //เพิ่มรหัสสีกับ special
            if (txtSpecial.Text.Length > 0)
            {
                strItemCodeDye = (strItemCode + txtColor.Text).PadRight(8 - txtColor.Text.Length) + txtSpecial.Text;
            }
            else
            {
                strItemCodeDye = strItemCode + txtColor.Text + txtSpecial.Text;
            }
            //============================ITEM MASTER===========================
            dtItem.BeginInit();
            dtItem.Columns.Add("CODE", typeof(string));
            dtItem.Columns.Add("NAME", typeof(string));
            dtItem.Columns.Add("WC", typeof(string));
            dtItem.Columns.Add("STORAGE", typeof(string));
            dtItem.Columns.Add("FIXLEVEL", typeof(Int16));
            dtItem.Columns.Add("FIX", typeof(Int16));
            dtItem.Columns.Add("MFG", typeof(Int16));
            dtItem.Columns.Add("LEAD", typeof(Int16));
            dtItem.Columns.Add("LOTS", typeof(Int16));
            dtItem.Columns.Add("BARCODE", typeof(Int16));
            dtItem.Columns.Add("COLOR", typeof(string));
            dtItem.Columns.Add("PQTY", typeof(Int16));
            dtItem.Columns.Add("WCLASS", typeof(Int16));
            dtItem.Columns.Add("ROUNDUP", typeof(Int16));
            dtItem.Columns.Add("PCS", typeof(string));
            dtItem.Columns.Add("MIXING", typeof(StringBuilder));
            dtItem.Columns.Add("INTER_CODE", typeof(string));
            dtItem.EndInit();
            gridControl1.DataSource=dtItem;
            gridView1.AddNewRow();
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "CODE", strItemCodeDye);
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "NAME", txtFabric.Text + " " + txtColor.Text);
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "WC", "DYE");
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "STORAGE", "FDEL");
            foreach(DataRow dr in dsItemMaster.Tables[0].Rows)
            {
                if(dr["MATERIAL"].ToString()=="DYE")
                {
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "FIXLEVEL", dr["FIXLEVEL"].ToString());
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "FIX", dr["FIX"].ToString());
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "LEAD", dr["LEAD"].ToString());
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "LOTS", dr["LOTS"].ToString());
                    break;
                }
            }
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "MFG", CalculateWorkDay((DateTime)dtpDFrom.EditValue, (DateTime)dtpDTo.EditValue));
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "BARCODE", txtFabric.Text);
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "COLOR", txtColor.Text);
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "PQTY", 2);
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "WCLASS", 0);
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "ROUNDUP", 0);
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "PCS", txtPCS.Text + " PCS");
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "MIXING",new StringBuilder(""));
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "INTER_CODE", txtFabric.Text + " " + txtColor.Text);
            gridView1.UpdateCurrentRow();

            gridView1.AddNewRow();
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "CODE",strItemCode.Substring(9,strItemCode.Length - 9) + "KNT"); //strItemCode)
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "NAME", txtFabric.Text);
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "WC", txtMachine.Text);
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "STORAGE", "KNT");
            foreach(DataRow dr in dsItemMaster.Tables[0].Rows)
            {
                if(dr["MATERIAL"].ToString()== "KNIT")
                {
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "FIXLEVEL", dr["FIXLEVEL"].ToString());
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "FIX", dr["FIX"].ToString());
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "LEAD", dr["LEAD"].ToString());
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "LOTS", dr["LOTS"].ToString());
                    break;
                }
            }
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "MFG", CalculateWorkDay((DateTime)dtpKFrom.EditValue,(DateTime)dtpKTo.EditValue));
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "BARCODE", txtFabric.Text);
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "COLOR", "");
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "PQTY", 2);
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "WCLASS", 0);
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "ROUNDUP", 0);
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "PCS", txtPCS.Text + " PCS");
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "MIXING", InsertToMixing());
            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "INTER_CODE", txtInterCode.Text);
            gridView1.UpdateCurrentRow();

            for(int i=0;i<gridYarnView.RowCount;i++)
            {
                gridView1.AddNewRow();
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "CODE", gridYarnView.GetRowCellDisplayText(i, "CODE"));
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "NAME", gridYarnView.GetRowCellDisplayText(i, "NAME"));
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "WC", gridYarnView.GetRowCellDisplayText(i, "SUP_ID"));
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "STORAGE", "ST02");
                foreach(DataRow dr in dsItemMaster.Tables[0].Rows)
                {
                    if(dr["MATERIAL"].ToString()== "YARN")
                    {
                        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "FIXLEVEL", dr["FIXLEVEL"].ToString());
                        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "FIX", dr["FIX"].ToString());
                        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "MFG", dr["MFG"].ToString());
                        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "LEAD", dr["LEAD"].ToString());
                        gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "LOTS", dr["LOTS"].ToString());
                        break;
                    }
                }
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "BARCODE", gridYarnView.GetRowCellDisplayText(i, "NAME"));
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "COLOR", "");
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "PQTY", 2);
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "WCLASS", 0);
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "ROUNDUP", 0);
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "PCS", "");
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "MIXING",new StringBuilder(""));
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "INTER_CODE", "");
                gridView1.UpdateCurrentRow();
            }
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();
            //============================BOM================================
            dtBOM.BeginInit();
            dtBOM.Columns.Add("CODE", typeof(string));
            dtBOM.Columns.Add("KCODE", typeof(string));
            dtBOM.Columns.Add("SIYOU", typeof(decimal));
            dtBOM.Columns.Add("SIYOUW", typeof(decimal));
            dtBOM.EndInit();
            gridControl2.DataSource = dtBOM;
            gridView2.AddNewRow();
            gridView2.SetRowCellValue(gridView2.FocusedRowHandle, "CODE", strItemCodeDye);
            gridView2.SetRowCellValue(gridView2.FocusedRowHandle, "KCODE", strItemCode.Substring(9, strItemCode.Length - 9) + "KNT"); //strItemCode)
            gridView2.SetRowCellValue(gridView2.FocusedRowHandle, "SIYOU", 1);
            gridView2.SetRowCellValue(gridView2.FocusedRowHandle, "SIYOUW", 1);
            gridView2.UpdateCurrentRow();
            for(int i=0;i<gridYarnView.RowCount;i++)
            {
                gridView2.AddNewRow();
                gridView2.SetRowCellValue(gridView2.FocusedRowHandle, "CODE", strItemCode.Substring(9, strItemCode.Length - 9) + "KNT"); //strItemCode)
                gridView2.SetRowCellValue(gridView2.FocusedRowHandle, "KCODE", gridYarnView.GetRowCellDisplayText(i, "CODE"));
                gridView2.SetRowCellValue(gridView2.FocusedRowHandle, "SIYOU", gridYarnView.GetRowCellDisplayText(i, "PERCENT_USE"));
                gridView2.SetRowCellValue(gridView2.FocusedRowHandle, "SIYOUW", 1);
                gridView2.UpdateCurrentRow();
            }
            gridView2.OptionsView.EnableAppearanceEvenRow = true;
            gridView2.OptionsView.EnableAppearanceOddRow = true;
            gridView2.OptionsView.ColumnAutoWidth = false;
            gridView2.BestFitColumns();

        }
        private void ExportToTPiCS()
        {
            string strSQL = "";
            string strDate=DateTime.Now.ToString("yyMMddHHmmss",dtfinfo); 
            string strUser=System.Environment.MachineName.Substring(0,10);
            db.ConnectionOpen();
            try 
	        {	        
		        db.BeginTrans();
                //Item Master
                for(int i=0;i<gridView1.RowCount;i++)
                {
                    strSQL="SELECT COUNT(CODE) FROM XHEAD WHERE CODE='"+gridView1.GetRowCellDisplayText(i, "CODE")+"'";
                    if(Convert.ToInt16(db.ExecuteFirstValue(strSQL))>0)
                    {
                        continue;
                    }
                    strSQL = "INSERT INTO XHEAD(CODE,NAME,MAINBUMO,TANI1,BARCODE,INPUTDATE,INPUTUSER,MIXING,CODE2)VALUES("+
                        "'"+gridView1.GetRowCellDisplayText(i, "CODE")+"'"+
                        ",'"+gridView1.GetRowCellDisplayText(i, "NAME")+"'"+
                        ",'"+gridView1.GetRowCellDisplayText(i, "WC")+"'"+
                        ",'KGS','"+gridView1.GetRowCellDisplayText(i, "BARCODE")+"'"+
                        ",'"+strDate+"','"+strUser+"','"+gridView1.GetRowCellDisplayText(i, "MIXING")+"','"+gridView1.GetRowCellDisplayText(i, "INTER_CODE")+"')";
                    db.Execute(strSQL);
                    strSQL = "INSERT INTO XITEM(CODE,BUMO,VENDOR,FIXLEVEL,DKAKU,KAKU,LEAD,KOUKI,HOKAN,LOTS,BARCODE,COLOR,PKET"+
                        ",HIKIKU,KURIAGE,CONT,CODE2,INPUTDATE,INPUTUSER)VALUES("+
                        "'"+gridView1.GetRowCellDisplayText(i, "CODE")+"'"+
                        ",'"+gridView1.GetRowCellDisplayText(i, "WC")+"'"+
                        ",'"+gridView1.GetRowCellDisplayText(i, "WC")+"'"+
                        ","+gridView1.GetRowCellDisplayText(i, "FIXLEVEL")+
                        ","+gridView1.GetRowCellDisplayText(i, "FIX")+
                        ","+gridView1.GetRowCellDisplayText(i, "FIX")+
                        ","+gridView1.GetRowCellDisplayText(i, "LEAD")+
                        ","+gridView1.GetRowCellDisplayText(i, "MFG")+
                        ",'"+gridView1.GetRowCellDisplayText(i, "STORAGE")+"'"+
                        ","+gridView1.GetRowCellDisplayText(i, "LOTS")+
                        ",'"+gridView1.GetRowCellDisplayText(i, "BARCODE")+"'"+
                        ",'"+gridView1.GetRowCellDisplayText(i, "COLOR")+"'"+
                        ","+gridView1.GetRowCellDisplayText(i, "PQTY")+
                        ","+gridView1.GetRowCellDisplayText(i, "WCLASS")+
                        ","+gridView1.GetRowCellDisplayText(i, "ROUNDUP")+
                        ",'"+gridView1.GetRowCellDisplayText(i, "PCS")+"'"+
                        ",'"+gridView1.GetRowCellDisplayText(i, "INTER_CODE")+"'"+
                        ",'"+strDate+"','"+strUser+"')";
                    db.Execute(strSQL);
                }
                //BOM---------------------------------------------------------------------------------------------------------------------------------------------------
                //เนื่องจากมีปัญหามีการเพิ่ม knt ที่มีลูกเป็นเส้นด้ายเข้าไปเพิ่มโดยพลการจึงจำเป็นต้องมีการตรวจสอบ parent code ใน bom ก่อนว่ามีอยู่แล้วหรือเปล่า
                //ถ้ามีอยู่แล้วจึงทำการลบออกทั้งหมดก่อนแล้วจึงจะ import ใหม่ได้
                for(int i=0;i<gridView2.RowCount;i++)
                {
                    strSQL = "DELETE FROM XPRTS WHERE CODE='"+gridView2.GetRowCellDisplayText(i, "CODE")+"'";
                    db.Execute(strSQL);
                }
                for(int i=0;i<gridView2.RowCount;i++)
                {
                    strSQL = "SELECT COUNT(CODE) FROM XPRTS WHERE CODE='"+gridView2.GetRowCellDisplayText(i, "CODE")+"'"+
                        " AND KCODE='"+gridView2.GetRowCellDisplayText(i, "KCODE")+"'";
                    if(Convert.ToInt16(db.ExecuteFirstValue(strSQL)) > 0)
                    {
                        continue;
                    }
                    strSQL = "INSERT INTO XPRTS(CODE,KCODE,SIYOU,SIYOUW,INPUTDATE,INPUTUSER)VALUES("+
                        "'"+gridView2.GetRowCellDisplayText(i, "CODE")+"'"+
                        ",'"+gridView2.GetRowCellDisplayText(i, "KCODE")+"'"+
                        ","+gridView2.GetRowCellDisplayText(i, "SIYOU")+
                        ","+gridView2.GetRowCellDisplayText(i, "SIYOUW")+
                        ",'"+strDate+"','"+strUser+"')";
                    db.Execute(strSQL);
                }
                //-------------------------------------------------------------------------------------------------------------------------------------------------------
                db.CommitTrans();
                MessageBox.Show("EXPORT COMPLETE","Export",MessageBoxButtons.OK,MessageBoxIcon.Information);
	        }
	        catch (Exception ex)
	        {
		        db.RollbackTrans();
                MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
	        }
            db.ConnectionClose();
        }

        private void frmGenCode_Load(object sender, EventArgs e)
        {
            dtfinfo=clinfo.DateTimeFormat;
            LoadItemMasterConfig();
            LoadCalendar();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            cboDivision.SelectedIndex = 0;
            txtOrder.Text = "";
            txtBranch.Text = "0";
            txtFabric.Text = "";
            txtColor.Text = "";
            txtSpecial.Text = "";
            txtMachine.Text = "";
            txtPCS.Text = "";
            txtFabricID.Text = "";
            txtWeight.Text = "";
            txtInterCode.Text = "";
            chkAdd.Checked =false; 
            ClearGridView(gridYarnView);
            ClearGridView(gridView1);
            dtpDFrom.EditValue =null;
            dtpDTo.EditValue = null;
            dtpKFrom.EditValue = null;
            dtpKTo.EditValue = null;
        }
        private void btnGenCode_Click(object sender, EventArgs e)
        {
            try
            {
                SearchYarn(txtFabric.Text);
                txtFabricID.Text = SearchFabricID(txtFabric.Text);
                txtInterCode.Text = SearchInterCode(txtOrder.Text);
                GenCode();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"ERROR",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportToTPiCS();      
        }
        private void txtOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar ==(char)13)
            {
                SendKeys.Send("{TAB}");
            }
        }
        private void txtBranch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(char)13)
            {
                SendKeys.Send("{TAB}");
            }
        }
        private void txtFabric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(char)13)
            {
                SendKeys.Send("{TAB}");
            }
        }
        private void txtColor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(char)13)
            {
                SendKeys.Send("{TAB}");
            }
        }
        private void txtSpecial_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(char)13)
            {
                SendKeys.Send("{TAB}");
            }
        }
        private void txtPCS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(char)(13))
            {
                SendKeys.Send("{TAB}");
            }
        }
        private void txtMachine_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==(char)13)
            {
                SendKeys.Send("{TAB}");
            }
        }

    }



}
