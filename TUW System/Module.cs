using System;
using System.Data;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.Reflection;
using myClass;

/// <summary>
/// Summary description for Module
/// </summary>
public static class Module
{
    [DllImport("kernel32")]
    private static extern long WritePrivateProfileString(string section,string key, string val, string filePath);
    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section,string key, string def, StringBuilder retVal,int size, string filePath);

    
    //public static string Sewing = "Server=" + "(local)" + ";uid=sa;pwd=;database=Sewing";
    //public static string Sewing_Master = "Server=" + "(local)" + ";uid=sa;pwd=;database=master";
    //public static string Fabric = "Server=" + "(local)" + ";uid=sa;pwd=;database=Fabric";
    //public static string Parfun = "Server=" + "(local)" + ";uid=sa;pwd=;database=Parfun";
    //public static string Riki = "Server=" + "(local)" + ";uid=sa;pwd=;database=Riki";
    //public static string Sale = "Server=" + "(local)" + ";uid=sa;pwd=;database=Sale";
    //public static string DBExim = "Server=" + "(local)" + ";uid=sa;pwd=;database=Exdbboi";
    //public static string ISODocument = "Server=" + "(local)" + ";uid=sa;pwd=;database=IsoDocument";
    //public static string Attendance = "Server=" + "(local)" + ";uid=sa;pwd=;database=Attendance";
    //public static string tuwCenter = "Server=" + "(local)" + ";uid=sa;pwd=;database=tuwCenter";

    public static string TUW99 = "Server=" + "tuwncbase" + ";uid=sa;pwd=ZAQ113m4tuw;database=PurchaseOrder";
    //public static string TUW99 = "Server=" + "tuwncbase" + ";uid=sa;pwd=ZAQ113m4tuw;database=xxx";
    public static string Sewing = "Server=" + "tpics_server" + ";uid=sa;pwd=;database=Sewing";
    //public static string Sewing = "Server=" + "system_02" + ";uid=sa;pwd=ZAQ113m4tuw;database=Sewing";
    public static string Sewing_Master = "Server=" + "tpics_server" + ";uid=sa;pwd=;database=master";
    public static string Fabric = "Server=" + "tpics_server" + ";uid=sa;pwd=;database=Fabric";
    public static string Parfun = "Server=" + "tpics_server" + ";uid=sa;pwd=;database=Parfun";
    //public static string Parfun = "Server=" + "(local)" + ";uid=sa;pwd=ZAQ113m4tuw;database=Parfun";
    public static string Riki = "Server=" + "tpics_server" + ";uid=sa;pwd=;database=Riki";
    public static string Sale = "Server=" + "tuwncbaseii" + ";uid=sa;pwd=;database=Sale";
    public static string DBExim = "Server=" + "tuwncbaseii" + ";uid=sa;pwd=ZAQ113m4tuw;database=Exdbboi";
    public static string ISODocument = "Server=" + "tuwncbaseii" + ";uid=sa;pwd=ZAQ113m4tuw;database=IsoDocument";
    public static string Attendance = "Server=" + "hrbase" + ";uid=sa;pwd=;database=Attendance";
    public static string tuwCenter = "Server=" + "tuwncbaseii" + ";uid=sa;pwd=ZAQ113m4tuw;database=tuwCenter";
    public static string SmartAdminMvc = "Server=" + "tuwncbaseii" + ";uid=sa;pwd=ZAQ113m4tuw;database=SmartAdminMvc";

    public static string strSection;
    public static string strUserName;

    public static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
    public static bool IsTableLock(string tableName)//คืนค่าเป็น true เมื่อตารางมีการถูกล็อคการใช้
    {
        bool blnLock;
        string strsql = "SELECT  COUNT(*) FROM SYSLOCKS WHERE ID=OBJECT_ID('SEWING.DBO." + tableName + "')";
        cDatabase db = new cDatabase(Module.Sewing_Master);
        db.ConnectionOpen();
        string strResult = db.ExecuteFirstValue(strsql);
        if (int.Parse(strResult) > 0)
        { blnLock = true; }
        else
        { blnLock = false; }
        db.ConnectionClose();
        return blnLock;
    }
    public static bool IsHoliday(string strYear,string strMonth,string strDay)
    {
        cDatabase db = new cDatabase(TUW99);
        string strSQL = "SELECT HDATE FROM HOLIDAY WHERE CONVERT(VARCHAR(8),HDATE,112)='"+strYear+strMonth+strDay+"'";
        DataTable dt = db.GetDataTable(strSQL);
        if (dt != null && dt.Rows.Count > 0) { return true; }
        else { return false; }
    }
    public static string ReadINI(string Section, string KeyName, string FileName)
    {
        System.Text.StringBuilder strData = new System.Text.StringBuilder(255);
        GetPrivateProfileString(Section, KeyName, "", strData, strData.Capacity, FileName);
        return strData.ToString();
    }
    //public static int WriteINI(string sSection, string sKeyName, string sNewString, string sFileName)
    //{
    //    dynamic r ;
    //    r = WritePrivateProfileString(sSection, sKeyName, sNewString, sFileName);
    //}
    //public static void ConnectTPICS()
    //{
    //    try
    //    {
    //        strSection = "dbo";
    //        DSN = new ADODB.Connection();
    //        DSN.ConnectionString = "DRIVER={SQL Server}; Data Source='Sewing';Initial Catalog='Sewing';User ID='sa';Password='';";
    //        DSN.CursorLocation = ADODB.CursorLocationEnum.adUseClient;
    //        DSN.CommandTimeout = 0;
    //        DSN.Mode = ADODB.ConnectModeEnum.adModeReadWrite;
    //        DSN.Open();
    //    return;
    //    }
    //    catch (Exception)
    //    {
    //        MessageBox.Show("Sorry, Can't Connect SQL Server ","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
    //        System.Environment.Exit(0);
    //    }
    // }
    //public static void DisConnectTPICS()
    //{
    //    //if (DSN.State == ADODB.ObjectStateEnum.adStateOpen)
    //   //     DSN.Close();
    //}
    //public static void ConnectPurchase()
    //{
    //    try
    //    {
    //        strSection = "dbo";
    //        DSN = new ADODB.Connection();
    //        DSN.ConnectionString = "DRIVER={SQL Server}; Data Source='TUW99';Initial Catalog='PurchaseOrder';User ID='sa';Password='';";
    //        DSN.CursorLocation = ADODB.CursorLocationEnum.adUseClient;
    //        DSN.CommandTimeout = 0;
    //        DSN.Mode = ADODB.ConnectModeEnum.adModeReadWrite;
    //        DSN.Open();
    //        return;
    //    }
    //    catch (Exception)
    //    {
    //        MessageBox.Show("Sorry, Can't Connect SQL Server ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //        System.Environment.Exit(0);
    //    }
    //}
    //public static void DisConnectPurchase()
    //{
    //    GDSN.Close();
    //}
    public static IEnumerable<string> SplitByLength(this string str, int maxLength)//ฟังก์ชั่นตัดคำตามความยาวที่กำหนด
    {
        for (int index = 0; index < str.Length; index += maxLength)
        {
            yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
        }
    }
    public static List<string> GetCountryList()
    {
        //create a new Generic list to hold the country names returned
        List<string> cultureList = new List<string>();

        //create an array of CultureInfo to hold all the cultures found, these include the users local cluture, and all the
        //cultures installed with the .Net Framework
        CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

        //loop through all the cultures found
        foreach (CultureInfo culture in cultures)
        {
            //pass the current culture's Locale ID (http://msdn.microsoft.com/en-us/library/0h88fahh.aspx)
            //to the RegionInfo contructor to gain access to the information for that culture
            RegionInfo region = new RegionInfo(culture.LCID);

            //make sure out generic list doesnt already
            //contain this country
            if (!(cultureList.Contains(region.EnglishName)))
                //not there so add the EnglishName (http://msdn.microsoft.com/en-us/library/system.globalization.regioninfo.englishname.aspx)
                //value to our generic list
                cultureList.Add(region.EnglishName);
        }
        return cultureList;
    }//คืนค่าเป็นรายชื่อประเทศ
    public static DialogResult InputBox(string title, string promptText, ref string value)//สร้างอินพุทบ็อกซ์
    {
        Form form = new Form();
        Label label = new Label();
        TextBox textBox = new TextBox();
        Button buttonOk = new Button();
        Button buttonCancel = new Button();

        form.Text = title;
        label.Text = promptText;
        textBox.Text = value;

        buttonOk.Text = "OK";
        buttonCancel.Text = "Cancel";
        buttonOk.DialogResult = DialogResult.OK;
        buttonCancel.DialogResult = DialogResult.Cancel;

        label.SetBounds(9, 20, 372, 13);
        textBox.SetBounds(12, 36, 372, 20);
        buttonOk.SetBounds(228, 72, 75, 23);
        buttonCancel.SetBounds(309, 72, 75, 23);

        label.AutoSize = true;
        textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
        buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

        form.ClientSize =  new Size(396, 107);
        form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
        form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.StartPosition = FormStartPosition.CenterScreen;
        form.MinimizeBox = false;
        form.MaximizeBox = false;
        form.AcceptButton = buttonOk;
        form.CancelButton = buttonCancel;

        DialogResult dialogResult = form.ShowDialog();
        value = textBox.Text;
        return dialogResult;
    }
    public static DataTable ConvertToDataTable<T>(IEnumerable<T> varlist)
    {
        DataTable dtReturn = new DataTable();

        // column names 
        PropertyInfo[] oProps = null;

        if (varlist == null) return dtReturn;

        foreach (T rec in varlist)
        {
            // Use reflection to get property names, to create table, Only first time, others will follow 
            if (oProps == null)
            {
                oProps = ((Type)rec.GetType()).GetProperties();
                foreach (PropertyInfo pi in oProps)
                {
                    Type colType = pi.PropertyType;

                    if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }

                    dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                }
            }

            DataRow dr = dtReturn.NewRow();

            foreach (PropertyInfo pi in oProps)
            {
                dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                (rec, null);
            }

            dtReturn.Rows.Add(dr);
        }
        return dtReturn;
    }
    public static string Right(string strInput, int intLength)//ฟังก์ชั่นตัดข้อความจากทางด้านขวา
    {
        try
        {
            return strInput.Substring(strInput.Length - intLength, intLength);
        }
        catch (Exception)
        {
            return "";
        }
    }
    public static string ThaiBaht(string txt)//ฟังก์ชั่นอ่านค่าเงินบาทเป็นภาษาไทย
    {
        string bahtTxt, n, bahtTH = "";
        double amount;
        try { amount = Convert.ToDouble(txt); }
        catch { amount = 0; }
        bahtTxt = amount.ToString("####.00");
        string[] num = { "ศูนย์", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า", "สิบ" };
        string[] rank = { "", "สิบ", "ร้อย", "พัน", "หมื่น", "แสน", "ล้าน" };
        string[] temp = bahtTxt.Split('.');
        string intVal = temp[0];
        string decVal = temp[1];
        if (Convert.ToDouble(bahtTxt) == 0)
            bahtTH = "ศูนย์บาทถ้วน";
        else
        {
            for (int i = 0; i < intVal.Length; i++)
            {
                n = intVal.Substring(i, 1);
                if (n != "0")
                {
                    if ((i == (intVal.Length - 1)) && (n == "1"))
                        bahtTH += "เอ็ด";
                    else if ((i == (intVal.Length - 2)) && (n == "2"))
                        bahtTH += "ยี่";
                    else if ((i == (intVal.Length - 2)) && (n == "1"))
                        bahtTH += "";
                    else
                        bahtTH += num[Convert.ToInt32(n)];
                    bahtTH += rank[(intVal.Length - i) - 1];
                }
            }
            bahtTH += "บาท";
            if (decVal == "00")
                bahtTH += "ถ้วน";
            else
            {
                for (int i = 0; i < decVal.Length; i++)
                {
                    n = decVal.Substring(i, 1);
                    if (n != "0")
                    {
                        if ((i == decVal.Length - 1) && (n == "1"))
                            bahtTH += "เอ็ด";
                        else if ((i == (decVal.Length - 2)) && (n == "2"))
                            bahtTH += "ยี่";
                        else if ((i == (decVal.Length - 2)) && (n == "1"))
                            bahtTH += "";
                        else
                            bahtTH += num[Convert.ToInt32(n)];
                        bahtTH += rank[(decVal.Length - i) - 1];
                    }
                }
                bahtTH += "สตางค์";
            }
        }
        return bahtTH;
    }
    #region "EnglishDollar//ฟังก์ชั่นอ่านค่าเงินดอลล่าร์เป็นภาษาอังกฤษ
    static string[] Tens = new string[] { 
			"Ten",
			"Twenty", 
			"Thirty", 
			"Forty", 
			"Fifty", 
			"Sixty", 
			"Seventy", 
			"Eighty", 
			"Ninety" };
    static string[] Ones = new string[] { 
			"One",
			"Two",
			"Three",
			"Four",
			"Five",
			"Six",
			"Seven",
			"Eight",
			"Nine",
			"Ten",
			"Eleven",
			"Twelve", 
			"Thirteen", 
			"Fourteen", 
			"Fifteen", 
			"Sixteen", 
			"Seventeen", 
			"Eighteen", 
			"Nineteen" };
    private static string HundredsText(string value)
    {
        char Val_1;
        char Val_2;

        string returnValue = "";
        bool IsSingleDigit = true;
        char Val = value[0];
        if (int.Parse(Val.ToString()) != 0)
        {
            Val_1 = value[0];
            returnValue = returnValue + Ones[int.Parse(Val_1.ToString()) - 1] + " Hundred ";
            IsSingleDigit = false;
        }
        Val_1 = value[1];
        if (int.Parse(Val_1.ToString()) > 1)
        {
            Val = value[1];
            returnValue = returnValue + Tens[int.Parse(Val.ToString()) - 1] + " ";
            Val_1 = value[2];
            if (int.Parse(Val_1.ToString()) != 0)
            {
                Val = value[2];
                returnValue = returnValue + Ones[int.Parse(Val.ToString()) - 1] + " ";
            }
            return returnValue;
        }
        Val_1 = value[1];
        if (int.Parse(Val_1.ToString()) == 1)
        {
            Val = value[1];
            Val_2 = value[2];
            return (returnValue + Ones[int.Parse(Val.ToString() + Val_2.ToString()) - 1] + " ");
        }
        Val_2 = value[2];
        if (int.Parse(Val_2.ToString()) == 0)
        {
            return returnValue;
        }
        if (!IsSingleDigit)
        {
            returnValue = returnValue + "and ";
        }
        Val_2 = value[2];
        return (returnValue + Ones[int.Parse(Val_2.ToString()) - 1] + " ");
    }
    public static string EnglishDollar(string value)
    {
        value = value.Replace(",", "").Replace("$", "");
        int decimalCount = 0;
        int Val = value.Length - 1;
        for (int x = 0; x <= Val; x++)
        {
            char Val2 = value[x];
            if (Val2.ToString() == ".")
            {
                decimalCount++;
                if (decimalCount > 1)
                {
                    throw new ArgumentException("Only monetary values are accepted");
                }
            }
            Val2 = value[x];
            char Valtemp = value[x];
            if (!(char.IsDigit(value[x]) | (Val2.ToString() == ".")) & !((x == 0) & (Valtemp.ToString() == "-")))
            {
                throw new ArgumentException("Only monetary values are accepted");
            }
        }
        string returnValue = "";
        string[] parts;
        if (value.Contains("."))
            parts = value.Split(new char[] { '.' });
        else
            parts = (value + ".00").Split(new char[] { '.' });


        parts[1] = new string((parts[1] + "00").Substring(0, 2).ToCharArray());
        bool IsNegative = parts[0].Contains("-");
        if (parts[0].Replace("-", "").Length > 0x12)
        {
            throw new ArgumentException("Maximum value is $999,999,999,999,999,999.99");
        }
        if (IsNegative)
        {
            parts[0] = parts[0].Replace("-", "");
            returnValue = returnValue + "Minus ";
        }
        if (parts[0].Length > 15)
        {
            returnValue = ((((returnValue + HundredsText(parts[0].PadLeft(0x12, '0').Substring(0, 3)) + "Quadrillion ")
                + HundredsText(parts[0].PadLeft(0x12, '0').Substring(3, 3)) + "Trillion ") +
                HundredsText(parts[0].PadLeft(0x12, '0').Substring(6, 3)) + "Billion ") +
                HundredsText(parts[0].PadLeft(0x12, '0').Substring(9, 3)) + "Million ") +
                HundredsText(parts[0].PadLeft(0x12, '0').Substring(12, 3)) + "Thousand ";
        }
        else if (parts[0].Length > 12)
        {
            returnValue = (((returnValue + HundredsText(parts[0].PadLeft(15, '0').Substring(0, 3)) +
                "Trillion ") + HundredsText(parts[0].PadLeft(15, '0').Substring(3, 3)) + "Billion ") +
                HundredsText(parts[0].PadLeft(15, '0').Substring(6, 3)) + "Million ") +
                HundredsText(parts[0].PadLeft(15, '0').Substring(9, 3)) + "Thousand ";
        }
        else if (parts[0].Length > 9)
        {
            returnValue = ((returnValue + HundredsText(parts[0].PadLeft(12, '0').Substring(0, 3)) +
                "Billion ") + HundredsText(parts[0].PadLeft(12, '0').Substring(3, 3)) + "Million ") +
                HundredsText(parts[0].PadLeft(12, '0').Substring(6, 3)) + "Thousand ";
        }
        else if (parts[0].Length > 6)
        {
            returnValue = (returnValue + HundredsText(parts[0].PadLeft(9, '0').Substring(0, 3)) +
                "Million ") + HundredsText(parts[0].PadLeft(9, '0').Substring(3, 3)) + "Thousand ";
        }
        else if (parts[0].Length > 3)
        {
            returnValue = returnValue + HundredsText(parts[0].PadLeft(6, '0').Substring(0, 3)) +
                "Thousand ";
        }
        string hundreds = parts[0].PadLeft(3, '0');
        int tempInt = 0;
        hundreds = hundreds.Substring(hundreds.Length - 3, 3);
        if (int.TryParse(hundreds, out tempInt) == true)
        {
            if (int.Parse(hundreds) < 100)
            {
                //returnValue = returnValue + "and ";
            }
            returnValue = returnValue + HundredsText(hundreds) + "Dollar";
            if (int.Parse(hundreds) != 1)
            {
                returnValue = returnValue + "s";
            }
            if (int.Parse(parts[1]) != 0)
            {
                returnValue = returnValue + " and ";
            }
        }
        if ((parts.Length == 2) && (int.Parse(parts[1]) != 0))
        {
            returnValue = returnValue + HundredsText(parts[1].PadLeft(3, '0')) + "Cent";
            if (int.Parse(parts[1]) != 1)
            {
                returnValue = returnValue + "s";
            }
        }
        return returnValue;
    }
    #endregion



    








}
