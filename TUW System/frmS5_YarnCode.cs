using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using System.Globalization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using myClass;

namespace TUW_System
{
    public partial class frmS5_YarnCode : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db = new cDatabase(Module.TUW99);
        //CultureInfo clinfo = new CultureInfo("en-US");
        //DateTimeFormatInfo dtfinfo;
        RepositoryItemComboBox rpYarnMixed,rpYarnNumber,rpUnit;
        RepositoryItemLookUpEdit rpYarnType,rpYarnSpecial,rpYarnColor,rpYarnSupplier; 
        string statusSave;//N=New data,E=Edit data
        int currentRowEdit;//แถวที่กำลังมีการแก้ไขข้อมูล
        mdiMain parent;

        public frmS5_YarnCode()
        {
            InitializeComponent();
        }
        public void NewData()
        {
            statusSave = "N";
            this.Text="YarnCode - New data";
            gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            gridView1.MoveLastVisible();
        }
        public void EditData()
        {
            statusSave = "E";
            this.Text = "YarnCode - Edit data";
            gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None;
            currentRowEdit = gridView1.FocusedRowHandle;
            parent.UpdateStatusBar("คุณกำลังแก้ไขข้อมูล ID: "+gridView1.GetRowCellDisplayText(currentRowEdit,"ID")+" CODE:"+gridView1.GetRowCellDisplayText(currentRowEdit,"CODE"));
        }
        public void SaveData()
        {
            db.ConnectionOpen();
            try 
	        {
                gridView1.CloseEditor();
                gridView1.UpdateCurrentRow();
	            db.BeginTrans();
                if(statusSave=="N")//Insert new record
                {
                    for (int i = 0; i < gridView1.DataRowCount; i++)
                    {
                        if (gridView1.GetRowCellDisplayText(i, "ID").Length == 0)
                        {
                            string errorText = CheckSaveData(i);
                            if (!string.IsNullOrEmpty(errorText)) throw new ApplicationException(errorText);
                            string strSQL = "SELECT ID,CODE,SPECIAL FROM YARNCODE WHERE TYPE='" + gridView1.GetRowCellDisplayText(i, "TYPE") + "'" +
                                " AND MIXED='" + gridView1.GetRowCellDisplayText(i, "MIXED") + "'" +
                                " AND YARNNO='" + gridView1.GetRowCellDisplayText(i, "NUMBER") + "'" +
                                " AND COLOR='" + gridView1.GetRowCellDisplayText(i, "COLOR") + "'" +
                                " AND SUPPLIER ='" + gridView1.GetRowCellDisplayText(i, "SUPPLIER") + "'" +
                                " AND DESCR='" + gridView1.GetRowCellDisplayText(i, "DESCRIPTION") + "'";
                            DataTable dt = db.GetDataTable(strSQL);
                            if (dt != null && dt.Rows.Count > 0)//ถ้าพบว่าเส้นด้ายชนิดนี้มีอยู่ในฐานข้อมูล(โดยดูจากส่วนประกอบต่างๆ)
                            {
                                if (Equals(dt.Rows[0]["SPECIAL"].ToString(), gridView1.GetRowCellDisplayText(i, "SPECIAL")))
                                    throw new ApplicationException("มีเส้นด้ายชนิดนี้อยู่แล้ว");
                                else//ถ้าองค์ประกอบของเส้นด้ายเหมือนกันแต่ต่างกันตรง special
                                    if(MessageBox.Show("มีเส้นด้ายชนิดนี้อยู่แล้วแต่ Special ต่างกัน คุณต้องการ Save หรือไม่","Caution",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.No)
                                        throw new ApplicationException("ยกเลิกการ Save");
                            }
                            strSQL = "INSERT INTO YARNCODE(CODE,TYPE,MIXED,YARNNO,COLOR,SPECIAL,UNIT,SUPPLIER,DESCR,RELEASE,BASE_ID)VALUES("+
                                "'"+gridView1.GetRowCellDisplayText(i,"CODE")+"','"+gridView1.GetRowCellDisplayText(i,"TYPE")+ "'"+
                                ",'"+gridView1.GetRowCellDisplayText(i,"MIXED")+"','"+gridView1.GetRowCellDisplayText(i,"NUMBER")+"'"+
                                ",'"+gridView1.GetRowCellDisplayText(i,"COLOR")+"','"+gridView1.GetRowCellDisplayText(i,"SPECIAL")+"'"+
                                ",'"+gridView1.GetRowCellDisplayText(i,"UNIT")+"','"+gridView1.GetRowCellDisplayText(i,"SUPPLIER")+"'"+
                                ",'"+gridView1.GetRowCellDisplayText(i,"DESCRIPTION")+"',"+gridView1.GetRowCellDisplayText(i,"RELEASE")+
                                ",'"+gridView1.GetRowCellDisplayText(i,"BASE_ID")+"')";
                            db.Execute(strSQL);
                            strSQL = "INSERT INTO YARNSTOCKBEGIN(MONTHYEAR,YARNID) SELECT CONVERT(CHAR(6),GETDATE(),112),A.ID FROM YARNCODE A "+
                                "WHERE A.CODE='"+gridView1.GetRowCellDisplayText(i,"CODE")+"' AND A.ID NOT IN (SELECT YARNID FROM YARNSTOCKBEGIN WHERE MONTHYEAR=CONVERT(CHAR(6),GETDATE(),112))";
                            db.Execute(strSQL);//เพิ่ม id ใหม่นี้ในตาราง yarnstockbegin เดือนปัจจุบันด้วย
                        }
                    }
                }
                if(statusSave=="E")//Update current record
                {
                    string errorText = CheckSaveData(currentRowEdit);
                    if(!string.IsNullOrEmpty(errorText)) throw new ApplicationException(errorText);
                    string strSQL = "UPDATE YARNCODE SET TYPE='" + gridView1.GetRowCellDisplayText(currentRowEdit, "TYPE") + "'" +
                        ",MIXED='" + gridView1.GetRowCellDisplayText(currentRowEdit, "MIXED") + "'" +
                        ",YARNNO='" + gridView1.GetRowCellDisplayText(currentRowEdit, "NUMBER") + "'" +
                        ",COLOR='" + gridView1.GetRowCellDisplayText(currentRowEdit, "COLOR") + "'" +
                        ",SPECIAL='" + gridView1.GetRowCellDisplayText(currentRowEdit, "SPECIAL") + "'" +
                        ",UNIT='" + gridView1.GetRowCellDisplayText(currentRowEdit, "UNIT") + "'" +
                        ",SUPPLIER='" + gridView1.GetRowCellDisplayText(currentRowEdit, "SUPPLIER") + "'" +
                        ",DESCR='" + gridView1.GetRowCellDisplayText(currentRowEdit, "DESCRIPTION") + "'" +
                        ",CODE='" + gridView1.GetRowCellDisplayText(currentRowEdit, "CODE") + "'" +
                        ",RELEASE=" + gridView1.GetRowCellDisplayText(currentRowEdit, "RELEASE") +
                        ",BASE_ID='" + gridView1.GetRowCellDisplayText(currentRowEdit, "BASE_ID") + "'" +
                        " WHERE ID='" + gridView1.GetRowCellDisplayText(currentRowEdit, "ID") + "'";
                    db.Execute(strSQL);
                }
                db.CommitTrans();
                MessageBox.Show("Save complete","Save",MessageBoxButtons.OK,MessageBoxIcon.Information);
                statusSave = "";
                this.Text = "YarnCode";
                gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None;
                parent.UpdateStatusBar("");
                GetYarnCodeTable();
                gridView1.MoveLastVisible();
	        }
	        catch (ApplicationException ex)
	        {
                db.RollbackTrans();
		        MessageBox.Show(ex.Message,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);		        
	        }
            catch(SystemException ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                statusSave = "";
                this.Text = "YarnCode";
                gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None;
                parent.UpdateStatusBar("");
            }
            db.ConnectionClose();
        }
        public void RefreshData()
        {
            GetYarnType();
            GetYarnSpecial();
            GetYarnColor();
            GetYarnSupplier();
            GetYarnMixed();
            GetYarnNumber();
            GetUnit();
            GetYarnCodeTable();
        }
        public void ExportExcel()
        {
            SaveFileDialog theOpenFile = new SaveFileDialog();
            string strTemp;
            theOpenFile.Filter = "Microsoft Excel Document|*.xls";
            if (theOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                strTemp = theOpenFile.FileName;
                gridView1.ExportToXls(strTemp);
            }
        }
        private string CheckSaveData(int row)//Check data before save
        {
            if (gridView1.GetRowCellDisplayText(row, "TYPE").Length == 0)
                return "Not found Yarn Type. Please Add Yarn Type.";
            else if (gridView1.GetRowCellDisplayText(row, "SUPPLIER").Length == 0)
                return "Please Input Supplier.";
            else if (gridView1.GetRowCellDisplayText(row, "MIXED").Length == 0)
                return "Please Input Mixed.";
            else if (gridView1.GetRowCellDisplayText(row, "NUMBER").Length == 0)
                return "Please Input Yarn Number.";
            else if (gridView1.GetRowCellDisplayText(row, "RELEASE").Length == 0)
                return "Please Input Release Period";
            else
                return "";
        }
        
        #region "Initialize form"

        public void GetYarnType()
        { 
            string strSQL="SELECT YARNCODE,YARNNAME FROM YARNNAME";
            DataTable dt = db.GetDataTable(strSQL);
            rpYarnType = new RepositoryItemLookUpEdit();
            rpYarnType.DisplayMember = "YARNNAME";
            rpYarnType.ValueMember = "YARNNAME";
            rpYarnType.NullText = "";
            rpYarnType.DataSource = dt;
        }
        public void GetYarnSpecial()
        {
            string strSQL = "SELECT SPECCODE,SPECIAL FROM YARNSPECIAL";
            DataTable dt = db.GetDataTable(strSQL);
            rpYarnSpecial = new RepositoryItemLookUpEdit();
            rpYarnSpecial.DisplayMember = "SPECIAL";
            rpYarnSpecial.ValueMember = "SPECIAL";
            rpYarnSpecial.NullText = "";
            rpYarnSpecial.DataSource = dt;
        }
        public void GetYarnColor()
        {
            string strSQL = "SELECT CODE,COLORNAME FROM YARNCOLORCODE";
            DataTable dt = db.GetDataTable(strSQL);
            rpYarnColor = new RepositoryItemLookUpEdit();
            rpYarnColor.DisplayMember = "COLORNAME";
            rpYarnColor.ValueMember = "COLORNAME";
            rpYarnColor.NullText = "";
            rpYarnColor.DataSource = dt;
        }
        public void GetYarnSupplier()
        {
            string strSQL = "SELECT SUPPLIERCODE,SUPPLIERNAME FROM YARNSUPPLIER";
            DataTable dt = db.GetDataTable(strSQL);
            rpYarnSupplier = new RepositoryItemLookUpEdit();
            rpYarnSupplier.DisplayMember = "SUPPLIERNAME";
            rpYarnSupplier.ValueMember = "SUPPLIERNAME";
            rpYarnSupplier.NullText = "";
            rpYarnSupplier.DataSource = dt;
        }
        public void GetYarnMixed()
        {
            string strSQL = "SELECT DISTINCT MIXED FROM YARNCODE";
            DataTable dt = db.GetDataTable(strSQL);
            rpYarnMixed = new RepositoryItemComboBox();
            rpYarnMixed.Items.Add("");
            foreach (DataRow dr in dt.Rows)
            {
                rpYarnMixed.Items.Add(dr["MIXED"].ToString());
            }
        }
        public void GetYarnNumber()
        {
            string strSQL = "SELECT DISTINCT YARNNO FROM YARNCODE";
            DataTable dt = db.GetDataTable(strSQL);
            rpYarnNumber = new RepositoryItemComboBox();
            rpYarnNumber.Items.Add("");
            foreach (DataRow dr in dt.Rows)
            {
                rpYarnNumber.Items.Add(dr["YARNNO"].ToString());
            }
        }
        public void GetUnit()
        {
            rpUnit = new RepositoryItemComboBox();
            rpUnit.Items.Add("Kgs.");
            rpUnit.Items.Add("Lbs.");
        }
        private void GetYarnCodeTable()
        {
            string strSQL = "SELECT ID,CODE,TYPE,YARNNO AS NUMBER,SPECIAL,DESCR AS DESCRIPTION"+
                ",COLOR,SUPPLIER,MIXED,UNIT,RELEASE,BASE_ID,SYSDELETE AS CANCEL"+
                " FROM YARNCODE";
            DataTable dt = db.GetDataTable(strSQL);
            gridControl1.DataSource = dt;
            gridView1.ActiveFilterString = "[CANCEL]<>1";
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.BestFitColumns();

            statusSave = "";
            this.Text = "YarnCode";
            gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None;
            parent.UpdateStatusBar("");
        }

        #endregion

        private void frmS5_YarnCode_Load(object sender, EventArgs e)
        {
            parent = (mdiMain)this.ParentForm;
            gridView1.OptionsView.ShowAutoFilterRow = true;
            gridView1.OptionsView.ShowGroupPanel = false;
            GetYarnType();
            GetYarnSpecial();
            GetYarnColor();
            GetYarnSupplier();
            GetYarnMixed();
            GetYarnNumber();
            GetUnit();
            GetYarnCodeTable();
        }
        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (gridView1.IsFilterRow(e.RowHandle))
                e.RepositoryItem = null;
            else
            { 
                switch (e.Column.FieldName)
                {
                    case "TYPE":
                        e.RepositoryItem = rpYarnType;
                        break;
                    case "SPECIAL":
                        e.RepositoryItem = rpYarnSpecial;
                        break;
                    case "COLOR":
                        e.RepositoryItem = rpYarnColor;
                        break;
                    case "SUPPLIER":
                        e.RepositoryItem = rpYarnSupplier;
                        break;
                    case "MIXED":
                        e.RepositoryItem = rpYarnMixed;
                        break;
                    case "NUMBER":
                        e.RepositoryItem = rpYarnNumber;
                        break;
                    case "UNIT":
                        e.RepositoryItem = rpUnit;
                        break;
                }            
            }

        }
        private void gridView1_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                if (gridView1.GetRowCellDisplayText(e.RowHandle, "CANCEL") == "1")
                {
                    Font newFont = new Font(gridView1.Appearance.Row.Font, FontStyle.Strikeout);
                    e.Appearance.Font = newFont;
                }
                if (statusSave == "E" && e.RowHandle==currentRowEdit)
                {
                    Font newFont = new Font(gridView1.Appearance.Row.Font, FontStyle.Bold);
                    e.Appearance.Font = newFont;
                }
            }
        }
        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (statusSave == "N")
            {
                if (gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, "ID").Length == 0)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }
            else if (statusSave == "E")
            {
                if (gridView1.FocusedRowHandle == currentRowEdit)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }
            else
                e.Cancel = true;
        }
        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            //สร้างสูตรเส้นด้ายจากองค์ประกอบต่างๆ
            string strType, strNumber, strSpecial, strColor, strSupplier;
            if (gridView1.FocusedColumn.FieldName == "TYPE")
                strType =(e.Value!=null)?rpYarnType.GetDataSourceValue("YARNCODE",rpYarnType.GetDataSourceRowIndex("YARNNAME",e.Value)).ToString():"";
            else
                strType = (gridView1.GetRowCellValue(gridView1.FocusedRowHandle,"TYPE")!=System.DBNull.Value) ?
                    rpYarnType.GetDataSourceValue("YARNCODE", rpYarnType.GetDataSourceRowIndex("YARNNAME", gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "TYPE"))).ToString() : "";
            if (gridView1.FocusedColumn.FieldName == "NUMBER")
                strNumber =(e.Value!=null)? "-"+e.Value.ToString():"";
            else
                strNumber=(gridView1.GetRowCellValue(gridView1.FocusedRowHandle,"NUMBER")!=null)? "-" + gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "NUMBER").ToString() : "";
            if (gridView1.FocusedColumn.FieldName == "SPECIAL")
                strSpecial =(e.Value!=null)? "-"+rpYarnSpecial.GetDataSourceValue("SPECCODE",rpYarnSpecial.GetDataSourceRowIndex("SPECIAL",e.Value)).ToString():"";
            else
                strSpecial = (gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "SPECIAL") != System.DBNull.Value) ?
                    "-" + rpYarnSpecial.GetDataSourceValue("SPECCODE", rpYarnSpecial.GetDataSourceRowIndex("SPECIAL", gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "SPECIAL"))).ToString() : "";
            if (gridView1.FocusedColumn.FieldName == "COLOR")
                strColor =(e.Value!=null)? "-"+rpYarnColor.GetDataSourceValue("CODE",rpYarnColor.GetDataSourceRowIndex("COLORNAME",e.Value)):"";
            else
                strColor = (gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "COLOR") != System.DBNull.Value) ?
                    "-" + rpYarnColor.GetDataSourceValue("CODE", rpYarnColor.GetDataSourceRowIndex("COLORNAME", gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "COLOR"))) : "";
            if (gridView1.FocusedColumn.FieldName == "SUPPLIER")
                strSupplier = (e.Value!=null)?"-"+rpYarnSupplier.GetDataSourceValue("SUPPLIERCODE",rpYarnSupplier.GetDataSourceRowIndex("SUPPLIERNAME",e.Value)):"";
            else
                strSupplier = (gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "SUPPLIER") != System.DBNull.Value) ?
                    "-" + rpYarnSupplier.GetDataSourceValue("SUPPLIERCODE", rpYarnSupplier.GetDataSourceRowIndex("SUPPLIERNAME", gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "SUPPLIER"))) : "";

            gridView1.SetRowCellValue(gridView1.FocusedRowHandle, "CODE", strType + strNumber + strSpecial + strColor + strSupplier);
        }
        private void btnType_Click(object sender, EventArgs e)
        {
            frmS5_YarnCode_Add frm = new frmS5_YarnCode_Add() { Owner = this };
            frm.FormType = "Type";
            frm.ShowDialog();
        }
        private void btnSpecial_Click(object sender, EventArgs e)
        {
            frmS5_YarnCode_Add frm = new frmS5_YarnCode_Add() { Owner = this };
            frm.FormType = "Special";
            frm.ShowDialog();
        }
        private void btnColor_Click(object sender, EventArgs e)
        {
            frmS5_YarnCode_Add frm = new frmS5_YarnCode_Add() { Owner = this };
            frm.FormType = "Color";
            frm.ShowDialog();
        }
        private void btnSupplier_Click(object sender, EventArgs e)
        {
            frmS5_YarnCode_Add frm = new frmS5_YarnCode_Add() { Owner = this };
            frm.FormType = "Supplier";
            frm.ShowDialog();
        }
        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (gridView1.IsEditing == false) { gridView1.DeleteSelectedRows(); }
            }
        }
    }

    public class frmS5_YarnCode_Add : DevExpress.XtraEditors.XtraForm
    {
        DevExpress.XtraEditors.LabelControl lbl1 = new DevExpress.XtraEditors.LabelControl();
        DevExpress.XtraEditors.LabelControl lbl2 = new DevExpress.XtraEditors.LabelControl();
        DevExpress.XtraEditors.TextEdit txt1 = new DevExpress.XtraEditors.TextEdit();
        DevExpress.XtraEditors.TextEdit txt2 = new DevExpress.XtraEditors.TextEdit();
        DevExpress.XtraEditors.SimpleButton btnClear = new DevExpress.XtraEditors.SimpleButton();
        DevExpress.XtraEditors.SimpleButton btnSave = new DevExpress.XtraEditors.SimpleButton();
        DevExpress.XtraEditors.SimpleButton btnExit = new DevExpress.XtraEditors.SimpleButton();

        string _formType;// ประเถทของฟอร์มที่ถูกเรียกมา 
        internal string FormType
        {
            set { _formType = value; }
        }

        public frmS5_YarnCode_Add()
        {
            InitializeComponent();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            switch (_formType)
            {
                case "Type":
                    ((frmS5_YarnCode)this.Owner).GetYarnType();
                    break;
                case "Special":
                    ((frmS5_YarnCode)this.Owner).GetYarnSpecial();
                    break;
                case "Color":
                    ((frmS5_YarnCode)this.Owner).GetYarnColor();
                    break;
                case "Supplier":
                    ((frmS5_YarnCode)this.Owner).GetYarnSupplier();
                    break;
            }
            this.Dispose();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            txt1.Text = "";
            txt2.Text = "";
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            cDatabase db= new cDatabase(Module.TUW99);
            db.ConnectionOpen();
            try
            { 
                string strSQL="";
                switch(_formType)
                {
                    case "Type":
                        strSQL = "SELECT COUNT(ID) FROM YARNNAME WHERE YARNNAME='"+ txt1.Text + "'";
                        if(db.ExecuteFirstValue(strSQL)== "0")
                        {
                            strSQL = "INSERT INTO YARNNAME(YARNNAME,YARNCODE)VALUES(N'" + txt1.Text + "','" + txt2.Text + "')";
                            db.Execute(strSQL);
                        }
                        else
                            throw new ApplicationException("มีชื่อเส้นด้ายนี้อยู่แล้ว");
                        break;
                    case "Special":
                        strSQL = "SELECT COUNT(ID) FROM YARNSPECIAL WHERE SPECIAL= N'" + txt1.Text + "'";
                        if(db.ExecuteFirstValue(strSQL) == "0")
                        {
                            strSQL = "INSERT INTO YARNSPECIAL(SPECIAL,SPECCODE)VALUES('" + txt1.Text + "','" + txt2.Text + "')";
                            db.Execute(strSQL);
                        }
                        else
                            throw new ApplicationException("มีรหัสนี้อยู่แล้ว");
                        break;
                    case "Color":
                        strSQL = "SELECT COUNT(ID) FROM YARNCOLORCODE WHERE COLORNAME = N'" + txt1.Text + "'";
                        if(db.ExecuteFirstValue(strSQL) == "0")
                        {
                            strSQL = "INSERT INTO YARNCOLORCODE(COLORNAME,CODE)VALUES(N'" + txt1.Text + "','" + txt2.Text + "')";
                            db.Execute(strSQL);
                        }
                        else
                            throw new ApplicationException("มีรหัสสีนี้อยู่แล้ว");
                        break;
                    case "Supplier":
                        strSQL = "SELECT COUNT(ID) FROM YARNSUPPLIER WHERE SUPPLIERNAME= N'" + txt1.Text + "'";
                        if(db.ExecuteFirstValue(strSQL)== "0")
                        {
                            strSQL = "INSERT INTO YARNSUPPLIER(SUPPLIERNAME,SUPPLIERCODE)VALUES(N'"+ txt1.Text + "','" + txt2.Text + "')";
                            db.Execute(strSQL);
                        }
                        else
                            throw new ApplicationException("มี Supplier นี้อยู่แล้ว");
                        break;
                }
                MessageBox.Show ("Save Complete","Save",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            catch(SystemException ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            db.ConnectionClose();
        }
        private void frmS5_YarnCode_Add_Load(object sender, EventArgs e)
        {
            switch (_formType)
            {
                case "Type":
                    lbl1.Text = "Yarn Type:";
                    this.Text = "Yarn Name...";
                    break;
                case "Special":
                    lbl1.Text = "Yarn Special:";
                    this.Text = "Yarn Special...";
                    break;
                case "Color":
                    lbl1.Text = "Color:";
                    this.Text = "Yarn Color...";
                    break;
                case "Supplier":
                    lbl1.Text = "Supplier:";
                    this.Text = "Yarn Supplier...";
                    break;
            }
        }
        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)txt1.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txt2.Properties).BeginInit();
            this.SuspendLayout();
            //lbl1
            lbl1.Appearance.Options.UseTextOptions = true;
            lbl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lbl1.Location = new System.Drawing.Point(33, 15);
            lbl1.Name = "lbl1";
            lbl1.Size = new System.Drawing.Size(16, 13);
            lbl1.TabIndex = 0;
            //lbl2
            lbl2.Location = new System.Drawing.Point(33, 41);
            lbl2.Name = "lbl2";
            lbl2.Size = new System.Drawing.Size(32, 13);
            lbl2.TabIndex = 1;
            lbl2.Text = "Code:";
            //txt2
            txt2.Location = new System.Drawing.Point(105, 38);
            txt2.Name = "txt2";
            txt2.Size = new System.Drawing.Size(206, 20);
            txt2.TabIndex = 3;
            //cmdClear
            btnClear.Location = new System.Drawing.Point(33, 73);
            btnClear.Name = "btnClear";
            btnClear.Size = new System.Drawing.Size(75, 23);
            btnClear.TabIndex = 4;
            btnClear.Text = "&Clear";
            //cmdSave
            btnSave.Location = new System.Drawing.Point(123, 73);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(75, 23);
            btnSave.TabIndex = 5;
            btnSave.Text = "&Save";
            //cmdExit
            btnExit.Location = new System.Drawing.Point(218, 73);
            btnExit.Name = "btnExit";
            btnExit.Size = new System.Drawing.Size(75, 23);
            btnExit.TabIndex = 6;
            btnExit.Text = "E&xit";
            //txt1
            txt1.Location = new System.Drawing.Point(105, 12);
            txt1.Name = "txt1";
            txt1.Size = new System.Drawing.Size(206, 20);
            txt1.TabIndex = 7;
            //frmYarnCode_Sub
            this.AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 13.0f);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 125);
            this.Controls.Add(txt1);
            this.Controls.Add(btnExit);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnClear);
            this.Controls.Add(txt2);
            this.Controls.Add(lbl2);
            this.Controls.Add(lbl1);
            this.Name = "frmYarnCode_Sub";
            ((System.ComponentModel.ISupportInitialize)txt2.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txt1.Properties).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            btnExit.Click += btnExit_Click;
            btnSave.Click += btnSave_Click;
            btnClear.Click += btnClear_Click;
            this.Load += frmS5_YarnCode_Add_Load;
        }

    }

}