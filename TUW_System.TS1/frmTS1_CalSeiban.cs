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

namespace TUW_System.TS1
{
    public partial class frmTS1_CalSeiban : DevExpress.XtraEditors.XtraForm
    {
        cDatabase db;

        private string _connectionString;
        public string ConnectionString
        {
            set { _connectionString = value; }
        }


        public frmTS1_CalSeiban()
        {
            InitializeComponent();
        }

        private void ProgressStatus(object sender, System.Data.SqlClient.SqlInfoMessageEventArgs e)
        {
           if (e.Errors.Count>0)
           {
               string message = e.Errors[0].Message;
               int state = e.Errors[0].State;
               switch (state) 
               { 
                   case 1:
                       progressBarControl2.Properties.Minimum = 0;
                       progressBarControl2.Properties.Maximum = Convert.ToInt32(message);
                       progressBarControl2.EditValue = 0;
                       break;
                   case 2:
                       //System.Diagnostics.Debug.Print(message);
                       var text = message.Split(',');
                       progressBarControl2.EditValue = text[0];
                       progressBarControl2.Update();
                       listBoxControl1.Items.Insert(0,text[1]);
                       listBoxControl1.Update();
                       //progressBarControl2.PerformStep();
                       
                       break;
               }
           }
        }
        private void UpdateCalendar()
        {
            string strSQL = "SELECT * FROM XCALE WHERE CALENO=1";
            DataTable dt = db.GetDataTable(strSQL);
            progressBarControl1.Properties.Maximum = dt.Rows.Count;
            progressBarControl1.EditValue = 0;
            foreach (DataRow dr in dt.Rows)
            { 
                int days=Convert.ToInt32(dr["DAYN"]);
                for (int i = 1; i <= days; i++)
                {
                    string cday=dr["CALENAME"].ToString() + i.ToString().PadLeft(2, '0');
                    strSQL = "SELECT COUNT(CDAY) FROM ZCALENDAR WHERE CDAY='" + cday  + "'";
                    if (db.ExecuteFirstValue(strSQL) == "0")
                    {
                        strSQL="INSERT INTO ZCALENDAR(CALENAME,CDAY,CALENO,USED,INPUTDATE)VALUES(";
                        strSQL += "'" + dr["CALENAME"].ToString() + "'";
                        strSQL += ",'" + cday + "'";
                        strSQL += "," + dr["CALENO"];
                        strSQL += "," + dr["WDAYNUM" + i.ToString()];
                        strSQL += "," + dr["INPUTDATE"] + ")";
                        db.Execute(strSQL);
                    }
                    else
                    {
                        strSQL = "UPDATE ZCALENDAR SET USED=" + dr["WDAYNUM" + i.ToString()]+",INPUTDATE="+dr["INPUTDATE"];
                        strSQL += " WHERE CDAY='" + cday + "'";
                        db.Execute(strSQL);
                    }
                }
                progressBarControl1.PerformStep();
                progressBarControl1.Update();
            }
        }
        private void CalSeiban()
        {
            string strSQL = "EXEC spCalSubsystem ''";
            db.ExecuteReader(strSQL,CommandType.Text);
            //strSQL = "SELECT PORDER, CODE, NDATE FROM XSLIP WHERE ISSUE = 'N' AND PONUM = '' AND PORDER LIKE 'XX%'";
            //DataTable dt = db.GetDataTable(strSQL);
            //progressBarControl2.Properties.Minimum=0;
            //progressBarControl2.Properties.Maximum=dt.Rows.Count;
            //progressBarControl2.EditValue=0;
            //foreach (DataRow dr in dt.Rows)
            //{ 
            //    strSQL="SELECT DISTINCT SBNO,USEDQTY,SBNOQTY FROM ZBOM WHERE KCODE = '"+dr["CODE"].ToString()+"'"+
            //        " AND PDATE = '" + dr["NDATE"].ToString() + "'";
            //    lblSQL.Text = strSQL;
            //    //DataTable dt2 = db.GetDataTable(strSQL);
            //    db.
            //    progressBarControl2.PerformStep();
            //    progressBarControl2.Update();
            //}

            
        }

        private void frmTS1_CalSeiban_Load(object sender, EventArgs e)
        {
            db = new cDatabase(_connectionString);
        }
        private void btnCalSeiban_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            listBoxControl1.Items.Clear();
            db.ConnectionOpen();
            db.Connection.InfoMessage += new System.Data.SqlClient.SqlInfoMessageEventHandler(ProgressStatus);
            string strSQL = "EXEC spCalSubsystem ''";
            db.ExecuteReader(strSQL, CommandType.Text);
            db.ConnectionClose();
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                strSQL = "SELECT PORDER, CODE, NDATE FROM XSLIP WHERE ISSUE = 'N' AND PONUM = '' AND PORDER LIKE 'XX%'";
                DataTable dt = db.GetDataTable(strSQL);
                progressBarControl2.Properties.Minimum=0;
                progressBarControl2.Properties.Maximum=dt.Rows.Count;
                int count = 0;
                progressBarControl2.EditValue=count;
                foreach (DataRow dr in dt.Rows)
                { 
                    strSQL="SELECT DISTINCT SBNO,USEDQTY,SBNOQTY FROM ZBOM WHERE KCODE = '"+dr["CODE"].ToString()+"'"+
                        " AND PDATE = '" + dr["NDATE"].ToString().Substring(0,8) + "'";
                    listBoxControl1.Items.Insert(0,strSQL);
                    listBoxControl1.Update();
                    DataTable dt2 = db.GetDataTable(strSQL);
                    if (dt2 != null && dt2.Rows.Count > 0)
                    {
                        StringBuilder strSBNo = new StringBuilder();
                        foreach (DataRow dr2 in dt2.Rows)
                        { 
                            strSBNo.Append(dr2["SBNO"].ToString()+"="+dr2["USEDQTY"].ToString()+" ; ");
                        }
                        string strSeiban = strSBNo.ToString().Remove(strSBNo.Length - 3, 3);
                        if (strSeiban.Length > 254) strSeiban = strSeiban.Substring(0, 254);
                        strSQL = "UPDATE XSLIP SET CONT='" +strSeiban +"' WHERE PORDER='"+dr["PORDER"].ToString()+"'";
                        db.Execute(strSQL);
                        listBoxControl1.Items.Insert(0, strSQL);
                        listBoxControl1.Update();
                    }

                    count += 1;
                    progressBarControl2.EditValue = count;
                    progressBarControl2.Update();
                }
                db.CommitTrans();
                MessageBox.Show("Cal seiban complete.", "Cal Seiban", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
   
        }
        private void btnUpdateCalendar_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            db.ConnectionOpen();
            try
            {
                db.BeginTrans();
                UpdateCalendar();
                db.CommitTrans();
                MessageBox.Show("Update calendar complete.", "Update calendar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                db.RollbackTrans();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            db.ConnectionClose();
            this.Cursor = Cursors.Default;
        }
    }
}