using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Data.SqlClient;
/// <summary>
/// Summary description for IncBalManager
/// </summary>
/// 
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Delve
{
    public class IncBalManager
    {
        //private static DataTable dtMapDtl = new DataTable();
        //private static DataTable dtBreakDtl = new DataTable();
        //private static DataTable dtLeafCoa = new DataTable();

        public static DataTable getMapDtl(string typecode, string verno,string book)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "SELECT B.SL_NO, B.GL_SEG_CODE, B.DESCRIPTION ,B.ADD_LESS, B.BAL_FROM,convert(varchar, b.cons_amt) cons_amt FROM GL_ST_MAP_DTL B " +
            " WHERE B.TYPE_CODE = '"+typecode+"' AND B.VER_NO = '"+verno+"' AND B.BOOK_NAME = '"+book+"' "+
	        " ORDER BY B.SL_NO ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "GlMapDtl");
            return dt;
        }
        public static DataTable getLeafCoa(string segcode,string book)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "SELECT GL_SEG_COA.SEG_COA_CODE,GL_SEG_COA.SEG_COA_DESC,GL_SEG_COA.ROOTLEAF "+
			   " FROM GL_LEVEL_TYPE, GL_SEG_COA WHERE GL_LEVEL_TYPE.BOOK_NAME = '"+book+"' "+
			   " AND GL_SEG_COA.PARENT_CODE = '"+segcode+"' AND GL_LEVEL_TYPE.BOOK_NAME = GL_SEG_COA.BOOK_NAME "+
               " AND GL_LEVEL_TYPE.LVL_CODE = GL_SEG_COA.LVL_CODE AND GL_LEVEL_TYPE.LVL_SEG_TYPE = 'N' ORDER BY GL_SEG_COA.ROOTLEAF,GL_SEG_COA.SEG_COA_CODE";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "GlBreakDtl");
            return dt;
        }
        public static DataTable getBreakDtl(string typecode, string verno, int slno,string book)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "SELECT B.SL_NO ,a.GL_SEG_CODE,b.GL_SEG_CODE AS[Gl_Coa_Code_BR],(SELECT add_less FROM GL_ST_MAP_DTL WHERE TYPE_CODE = '" + typecode + "' AND sl_no=b.sl_no) add_less " +
            " FROM GL_ST_MAP_DTL A, GL_ST_BREAK_DTL B WHERE A.BOOK_NAME = B.BOOK_NAME AND A.TYPE_CODE = B.TYPE_CODE " +
            " AND A.VER_NO = B.VER_NO AND A.SL_NO = B.REF_SL_NO AND B.TYPE_CODE = '" + typecode + "' AND B.VER_NO = '" + verno + "' " +
            " AND B.BOOK_NAME = '"+book+"' AND b.REF_SL_NO = " + slno + " ORDER BY B.REF_SL_NO,B.SL_NO ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "GlBreakDtl");
            return dt;
        }
        public static DataSet generateCashFlow(DataTable dt, string book, string typecode, string sepTyp, string verno, string pglcode, string startDt,string enddt, int rptsysid)
        {
            DataTable dtTmpRptCash = new DataTable();
            dtTmpRptCash.Columns.Add("GL_SEG_CODE", typeof(string));
            dtTmpRptCash.Columns.Add("COA_DESC", typeof(string));
            dtTmpRptCash.Columns.Add("AMOUNT_A", typeof(decimal));
            dtTmpRptCash.Columns.Add("AMOUNT_B", typeof(decimal));
            dtTmpRptCash.Columns.Add("AMOUNT_A_DT", typeof(string));
            dtTmpRptCash.Columns.Add("AMOUNT_B_DT", typeof(string));
            dtTmpRptCash.Columns.Add("COL_NO", typeof(string));
            dtTmpRptCash.Columns.Add("NOTES", typeof(string));
            dtTmpRptCash.Columns.Add("RPT_DATE", typeof(string));
            dtTmpRptCash.Columns.Add("TYPE_CODE", typeof(string));
            dtTmpRptCash.Columns.Add("VER_NO", typeof(string));
            dtTmpRptCash.Columns.Add("RPT_SYS_ID", typeof(string));
            dtTmpRptCash.Columns.Add("BOOK_NAME", typeof(string));
            DataTable dtTmpRptNotesCash = new DataTable();
            string LastClosedDate="";
            string vNotes = "";
            int vNotesCount = 0;
            decimal lcCurBal = decimal.Zero;
            decimal lcPrvBal = decimal.Zero;
            decimal vAmountA = decimal.Zero;
            string vCoaDesc = "";
            string vRootleaf = "";
            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand();            
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select convert(varchar, end_date,103) end_date from (SELECT MAX(END_DATE) end_date FROM GL_FIN_YEAR WHERE BOOK_NAME = '"+book+"' AND YEAR_FLAG " +
             " IN ('C','P') AND END_DATE < (SELECT START_DATE FROM GL_FIN_YEAR WHERE BOOK_NAME = '"+book+"' " +
             " AND convert(datetime,'" + enddt + "',103) >= START_DATE AND convert(datetime,'" + enddt + "',103) <= END_DATE)) tot1 ";
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())

                    //if (dReader["end_date"].ToString() != "")
                    //{
                    //    LastClosedDate = DateTime.MinValue.ToString("dd/MM/yyyy");
                    //}
                    //else
                    //{

                        LastClosedDate = dReader["end_date"].ToString();
                   // }
            }
            else
            {
                LastClosedDate = DateTime.MinValue.ToString("dd/MM/yyyy");
            }
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                SqlConnection.ClearPool(conn);
            }
            int lvlsize = 0;
            int lvlorder = 0;
            string segcode = "";
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT lvl_code,lvl_max_size,lvl_order from gl_level_type where lvl_seg_type='N' and lvl_enabled='Y' and book_name='" + book + "'";
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            dReader = cmd.ExecuteReader();            
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    lvlsize = int.Parse(dReader["lvl_max_size"].ToString());
                    lvlorder = int.Parse(dReader["lvl_order"].ToString());
                }
            }
            cmd.Dispose(); dReader.Close();
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                SqlConnection.ClearPool(conn);
            }
            if (lvlorder - 1 == 0)
            {
                lvlorder = 2;
            }
            segcode = pglcode.Substring(pglcode.IndexOf(sepTyp, 1, lvlorder - 1) + 1, lvlsize);
            string bank = "";
            string cash = "";
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT bank_code,cash_code from gl_set_of_books where book_name='" + book + "'";
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    bank = dReader["bank_code"].ToString();
                    cash = dReader["cash_code"].ToString();
                }
            }
            cmd.Dispose(); dReader.Close();
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                SqlConnection.ClearPool(conn);
            }
            DataTable dtMapDtl = getMapDtl(typecode, verno,book);
            
            foreach (DataRow drM in dtMapDtl.Rows)
            {                
                vNotes = "";
                if (drM["gl_seg_code"].ToString() != "")
                {
                    if (drM["gl_seg_code"].ToString() != bank && drM["gl_seg_code"].ToString() != cash)
                    {
                        if (drM["bal_from"].ToString() == "P")
                        {
                            lcCurBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book, pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), sepTyp, "A", "", LastClosedDate, null, null);
                        }
                        else
                        {
                            lcCurBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book, pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), sepTyp, "A", DateTime.Parse(DataManager.DateEncode(LastClosedDate).AddDays(1).ToString()).ToString("dd/MM/yyyy"), enddt, null, null);
                        }
                    }
                    else
                    {
                        if (drM["bal_from"].ToString() == "P")
                        {
                            lcCurBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book, pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), sepTyp, "A", "", LastClosedDate, null, null);
                        }
                        else
                        {
                            lcCurBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book, pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), sepTyp, "A", "", enddt, null, null);
                        }
                    }
                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT GL_SEG_COA.SEG_COA_DESC,GL_SEG_COA.ROOTLEAF FROM GL_LEVEL_TYPE, GL_SEG_COA " +
                    " WHERE GL_LEVEL_TYPE.BOOK_NAME = '"+book+"' AND GL_SEG_COA.SEG_COA_CODE = '" + drM["gl_seg_code"].ToString() + "' " +
                    " AND GL_LEVEL_TYPE.BOOK_NAME = GL_SEG_COA.BOOK_NAME AND GL_LEVEL_TYPE.LVL_CODE = GL_SEG_COA.LVL_CODE " +
                    " AND GL_LEVEL_TYPE.LVL_SEG_TYPE = 'N'";
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    dReader = cmd.ExecuteReader();
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            vCoaDesc = dReader["seg_coa_desc"].ToString();
                            vRootleaf = dReader["rootleaf"].ToString();
                        }
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        SqlConnection.ClearPool(conn);
                    }
                    if (vRootleaf == "R")
                    {
                        vNotesCount = vNotesCount + 1;
                        vNotes = Math.Round(decimal.Parse(vNotesCount.ToString()), 2).ToString();
                        dtTmpRptNotesCash = generateIncBalNotes(dt, book,
                            pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), LastClosedDate, startDt, enddt,
                            rptsysid,
                            int.Parse(drM["sl_no"].ToString()), vNotes, sepTyp, 1, lcCurBal, lcPrvBal);
                    }
                }
                else
                {
                    lcCurBal = decimal.Zero;
                    DataTable dtBreakDtl = getBreakDtl(typecode, verno, int.Parse(drM["sl_no"].ToString()),book);
                    foreach (DataRow drB in dtBreakDtl.Rows)
                    {
                        if (drB["sl_no"].ToString() == "")
                        {
                            if (drM["gl_seg_code"].ToString() != bank && drM["gl_seg_code"].ToString() != cash)
                            {
                                if (drM["bal_from"].ToString() == "P")
                                {
                                    vAmountA = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book, pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), sepTyp, "A", "", LastClosedDate, null, null);
                                }
                                else
                                {
                                    vAmountA = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book, pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), sepTyp, "A", DateTime.Parse(DataManager.DateEncode(LastClosedDate).AddDays(1).ToString()).ToString("dd/MM/yyyy"), enddt, null, null);
                                }
                            }
                            else
                            {
                                if (drM["bal_from"].ToString() == "P")
                                {
                                    vAmountA = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book, pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), sepTyp, "A", "", LastClosedDate, null, null);
                                }
                                else
                                {
                                    vAmountA = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book, pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), sepTyp, "A", "", enddt, null, null);
                                }
                            }
                        }
                        else
                        {
                            DataTable dtCash = dtTmpRptCash.Clone();
                            DataRow[] drCash = dtTmpRptCash.Select("rpt_sys_id = '" + rptsysid + "' and col_no = '" + int.Parse(drB["sl_no"].ToString()) + "'");
                            foreach (DataRow dr in drCash)
                            {
                                dtCash.ImportRow(dr);
                            }
                            if (dtCash.Rows.Count > 0)
                            {
                                vAmountA = decimal.Parse(((DataRow)dtCash.Rows[0])["amount_a"].ToString());
                            }
                            else
                            {
                                vAmountA = decimal.Zero;
                            }
                        }
                        if (drB["add_less"].ToString().Trim() == "L")
                        {
                            lcCurBal = lcCurBal - vAmountA;
                        }
                        else
                        {
                            lcCurBal = lcCurBal + vAmountA;
                        }
                    }
                }
                vCoaDesc = drM["add_less"].ToString().Replace("L", "Less: ").Replace("A", "Add: ") + " " + drM["description"].ToString();
                DataRow drTR = dtTmpRptCash.NewRow();
                drTR["GL_SEG_CODE"] = drM["gl_seg_code"].ToString();
                drTR["COA_DESC"] = vCoaDesc;
                drTR["AMOUNT_A"] = lcCurBal;
                drTR["AMOUNT_B"] = lcPrvBal;
                drTR["AMOUNT_A_DT"] = enddt;
                drTR["AMOUNT_B_DT"] = LastClosedDate;
                drTR["COL_NO"] = drM["sl_no"].ToString();
                drTR["NOTES"] = vNotes;
                drTR["RPT_DATE"] = enddt;
                drTR["TYPE_CODE"] = typecode;
                drTR["VER_NO"] = verno;
                drTR["RPT_SYS_ID"] = rptsysid;
                drTR["BOOK_NAME"] = book;
                dtTmpRptCash.Rows.Add(drTR);
            }
            DataSet dsTmpRptCash = new DataSet();
            dsTmpRptCash.Tables.Add(dtTmpRptCash);
            dsTmpRptCash.Tables.Add(dtTmpRptNotesCash);
            return dsTmpRptCash;
        }

        public static  string Gl_Type = "";
        public const double ClosingStock = 9272053.896;

        //*********************** Blance Sheet & Income Statement *****************//

        public static DataSet generateBal(DataTable dt, string book, string typecode, string verno, string pglcode, string enddt, int rptsysid, string StartDt1)
        {
            DataTable dtTmpRpt = new DataTable();
            dtTmpRpt.Columns.Add("GL_SEG_CODE", typeof(string));
            dtTmpRpt.Columns.Add("COA_DESC", typeof(string));
            dtTmpRpt.Columns.Add("AMOUNT_A", typeof(decimal));
            dtTmpRpt.Columns.Add("AMOUNT_B", typeof(decimal));
            dtTmpRpt.Columns.Add("AMOUNT_A_DT", typeof(string));
            dtTmpRpt.Columns.Add("AMOUNT_B_DT", typeof(string));
            dtTmpRpt.Columns.Add("COL_NO", typeof(string));
            dtTmpRpt.Columns.Add("NOTES", typeof(string));
            dtTmpRpt.Columns.Add("RPT_DATE", typeof(string));
            dtTmpRpt.Columns.Add("TYPE_CODE", typeof(string));
            dtTmpRpt.Columns.Add("VER_NO", typeof(string));
            dtTmpRpt.Columns.Add("RPT_SYS_ID", typeof(string));
            dtTmpRpt.Columns.Add("BOOK_NAME", typeof(string));
            DataTable dtTmpRptNotes = new DataTable();
            string LastClosedDate = "";
            string RefVerNo = "";
            string RetdEarnCode = "";
            string SepType = "";
            string vNotes = "";
            int vNotesCount = 0;
            int lcRptSysId = 0;
            decimal lcCurBal = decimal.Zero;
            decimal lcPrvBal = decimal.Zero;
            string vCoaDesc = "";
            string vRootleaf = "";
            decimal lcRetErnAmtCur = decimal.Zero;
            decimal lcRetErnAmtPrv = decimal.Zero;
            decimal vAmountA = decimal.Zero;
            decimal vAmountB = decimal.Zero;

            decimal CurBalRetainArn = decimal.Zero;
            decimal CurBalProfit = decimal.Zero;

            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT RETD_EARN_ACC,Separator_type FROM GL_SET_OF_BOOKS WHERE BOOK_NAME = '" + book + "' ";
            conn.Open();
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    RetdEarnCode = dReader["retd_earn_acc"].ToString();
                    SepType = dReader["separator_type"].ToString();
                }
            }
            else
            {
                throw new ArgumentException("Process Stopped! Retain earning account is not found.");
            }
            cmd.Dispose(); dReader.Close();
            if (typecode == "B")
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT REF_VER_NO FROM GL_ST_MAP_MST WHERE BOOK_NAME = '" + book + "' AND TYPE_CODE = '" + typecode + "' AND VER_NO  = '" + verno + "' ";
                dReader = cmd.ExecuteReader();
                if (dReader.HasRows == true)
                {
                    while (dReader.Read())
                        RefVerNo = dReader["ref_ver_no"].ToString();
                }
                else
                {
                    throw new ArgumentException("Process Stopped! Please mention Income Statement Version reference for Balance Sheet in report setup form.");
                }
                cmd.Dispose(); dReader.Close();
            }
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select convert(varchar,end_date,103) end_date from (SELECT MAX(END_DATE) end_date FROM GL_FIN_YEAR WHERE BOOK_NAME = '" + book + "' AND YEAR_FLAG " +
             " IN ('C','P') AND END_DATE < (SELECT START_DATE FROM GL_FIN_YEAR WHERE BOOK_NAME = '" + book + "' " +
             " AND convert(datetime,'" + enddt + "',103) >= START_DATE AND convert(datetime,'" + enddt + "',103) <= END_DATE)) tot2 ";
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                    LastClosedDate = dReader["end_date"].ToString();
            }
            else
            {
                LastClosedDate = "";
            }
            cmd.Dispose(); dReader.Close();

            DataTable dtMapDtl = getMapDtl(typecode, verno, book);
            //if (typecode.Equals("B") && enddt.Equals("01/01/2017"))
            //{
            //    var rowsToUpdate = dtMapDtl.AsEnumerable().Where(r => r.Field<string>("GL_SEG_CODE") == "1030002");
            //    foreach (var row in rowsToUpdate)
            //    {
            //        row.SetField("GL_SEG_CODE", "1030001");
            //        //row.SetField("enddate", enDate);
            //    }
            //}
            //else if (Gl_Type.Equals("B"))
            //{
            //    var rowsToUpdate = dtMapDtl.AsEnumerable().Where(r => r.Field<string>("GL_SEG_CODE") == "1030002");
            //    foreach (var row in rowsToUpdate)
            //    {
            //        row.SetField("GL_SEG_CODE", "1030001");
            //        //row.SetField("enddate", enDate);
            //    }
            //}
            int lvlsize = 0;
            int lvlorder = 0;
            string segcode = "";
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT lvl_code,lvl_max_size,lvl_order from gl_level_type where lvl_seg_type='N' and lvl_enabled='Y' and book_name='" + book + "'";
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    lvlsize = int.Parse(dReader["lvl_max_size"].ToString());
                    lvlorder = int.Parse(dReader["lvl_order"].ToString());
                }
            }
            cmd.Dispose(); dReader.Close();
            if (lvlorder - 1 == 0)
            {
                lvlorder = 2;
            }
            segcode = pglcode.Substring(pglcode.IndexOf(SepType, 1, lvlorder - 1) + 1, lvlsize);
            string startdt;
            if (StartDt1 == null)
            {
                startdt = enddt;
            }
            else
            {
                startdt = StartDt1;
                LastClosedDate =DataManager.DateEncode(StartDt1).AddDays(-1).ToString("dd/MM/yyyy");
                //LastClosedDate = "01/01/2017";
            }
            //if (typecode == "I")
            //{
            //    if (LastClosedDate != "")
            //    {
            //        startdt = DateTime.Parse(DataManager.DateEncode(LastClosedDate).AddDays(1).ToString()).ToString("dd/MM/yyyy");
            //    }
            //}
            decimal OpeningStock = decimal.Zero;
            foreach (DataRow drM in dtMapDtl.Rows)
            {
                vNotes = "";
                if (drM["cons_amt"].ToString().Trim() != "")
                {
                    vCoaDesc = drM["description"].ToString();
                    DataRow drTR = dtTmpRpt.NewRow();
                    drTR["GL_SEG_CODE"] = drM["gl_seg_code"].ToString();
                    drTR["COA_DESC"] = vCoaDesc;
                    drTR["AMOUNT_A"] = drM["cons_amt"].ToString();
                    drTR["AMOUNT_B"] = drM["cons_amt"].ToString();
                    drTR["AMOUNT_A_DT"] = enddt;
                    drTR["AMOUNT_B_DT"] = LastClosedDate;
                    drTR["COL_NO"] = drM["sl_no"].ToString();
                    drTR["NOTES"] = vNotes;
                    drTR["RPT_DATE"] = enddt;
                    drTR["TYPE_CODE"] = typecode;
                    drTR["VER_NO"] = verno;
                    drTR["RPT_SYS_ID"] = rptsysid;
                    drTR["BOOK_NAME"] = book;
                    dtTmpRpt.Rows.Add(drTR);
                }
                else
                {
                    if (drM["gl_seg_code"].ToString() != "")
                    {
                        DataSet dsShoopingInc = null;

                             //******* Change Opening Stock *******//

                        if (drM["gl_seg_code"].ToString().Equals("1030001") && typecode.Equals("I") &&
                            (DataManager.DateEncode(startdt) > DataManager.DateEncode("31/12/2017")))
                        {
                            DateTime dtStartDate = DataManager.DateEncode(startdt).AddDays(-1);
                            lcCurBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                pglcode.Replace(segcode, "1030002"), SepType, "A", "01/01/2010",
                                dtStartDate.ToString("dd/MM/yyyy"),
                                StartDt1, null);

                            if (LastClosedDate == "")
                            {
                                lcPrvBal = 0;
                            }
                            else
                            {
                                lcPrvBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                    pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), SepType, "A", "",
                                    LastClosedDate, LastClosedDate, null);
                            }
                        }
                        //************ Closing main Stock  *************//
                        else if (drM["gl_seg_code"].ToString().Equals("1030002") && typecode.Equals("I") &&
                                 (DataManager.DateEncode(startdt) > DataManager.DateEncode("31/12/2017")))
                        {
                            DateTime dtStartDate = DataManager.DateEncode(startdt).AddDays(-1);
                            lcCurBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), SepType, "A", "10/10/2017",
                                enddt,
                                StartDt1, null);

                            if (LastClosedDate == "")
                            {
                                lcPrvBal = 0;
                            }
                            else
                            {
                                lcPrvBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                    pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), SepType, "A", "",
                                    LastClosedDate, LastClosedDate, null);
                            }
                        }
                                //************ ShoppingCenter Income (RDA)  *************//

                        else if (drM["gl_seg_code"].ToString().Equals("9999999") && typecode.Equals("I"))
                        {
                            if (DataManager.DateEncode(startdt) >= DataManager.DateEncode("01/01/2018"))
                            {
                                dsShoopingInc = generateShoppingCenterIncome(dt, book, "B1", verno,
                                    pglcode, enddt, rptsysid, startdt);
                                //"01/01/2018"
                            }
                            else
                            {
                                dsShoopingInc = generateShoppingCenterIncome(dt, book, "B1", verno,
                                   pglcode, enddt, rptsysid, null);
                            }
                            DataTable dtRpt = dsShoopingInc.Tables[0];
                            lcCurBal = decimal.Parse(((DataRow)dtRpt.Rows[dtRpt.Rows.Count - 1])["amount_a"].ToString());
                            lcPrvBal = decimal.Parse(((DataRow)dtRpt.Rows[dtRpt.Rows.Count - 1])["amount_b"].ToString());
                        }


                                //************ ShoppingCenter Income (LP-1)  *************//

                        else if (drM["gl_seg_code"].ToString().Equals("9999991") && typecode.Equals("I"))
                        {
                            if (DataManager.DateEncode(startdt) >= DataManager.DateEncode("17/01/2017"))
                            {
                                dsShoopingInc = generateShoppingCenterIncome(dt, book, "B2", verno,
                                    pglcode, enddt, rptsysid, startdt);
                                //"01/01/2018"
                            }
                            else
                            {
                                dsShoopingInc = generateShoppingCenterIncome(dt, book, "B2", verno,
                                    pglcode, enddt, rptsysid, null);
                            }
                            DataTable dtRpt = dsShoopingInc.Tables[0];
                            lcCurBal = decimal.Parse(((DataRow)dtRpt.Rows[dtRpt.Rows.Count - 1])["amount_a"].ToString());
                            lcPrvBal = decimal.Parse(((DataRow)dtRpt.Rows[dtRpt.Rows.Count - 1])["amount_b"].ToString());
                        }
                        else
                        {

                            lcCurBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), SepType, "A", startdt,
                                enddt,
                                StartDt1, null);

                            if (LastClosedDate == "")
                            {
                                lcPrvBal = 0;
                            }
                            else
                            {
                                lcPrvBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                    pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), SepType, "A", "",
                                    LastClosedDate, LastClosedDate, null);
                            }
                        }
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT GL_SEG_COA.SEG_COA_DESC,GL_SEG_COA.ROOTLEAF FROM GL_LEVEL_TYPE, GL_SEG_COA " +
                        " WHERE GL_LEVEL_TYPE.BOOK_NAME = '" + book + "' AND GL_SEG_COA.SEG_COA_CODE = '" + drM["gl_seg_code"].ToString() + "' " +
                        " AND GL_LEVEL_TYPE.BOOK_NAME = GL_SEG_COA.BOOK_NAME AND GL_LEVEL_TYPE.LVL_CODE = GL_SEG_COA.LVL_CODE " +
                        " AND GL_LEVEL_TYPE.LVL_SEG_TYPE = 'N'";
                        dReader = cmd.ExecuteReader();
                        if (dReader.HasRows == true)
                        {
                            while (dReader.Read())
                            {
                                vCoaDesc = dReader["seg_coa_desc"].ToString();
                                vRootleaf = dReader["rootleaf"].ToString().Replace("L", "R");
                            }
                        }
                        cmd.Dispose(); dReader.Close();
                        if (vRootleaf == "R")
                        {
                            vNotesCount = vNotesCount + 1;
                            vNotes = Math.Round(decimal.Parse(vNotesCount.ToString()), 2).ToString();
                            //******** Add Parameter (StartDt1)
                            dtTmpRptNotes.Merge(generateIncBalNotes(dt, book,
                                pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), LastClosedDate, StartDt1, enddt,
                                rptsysid,
                                int.Parse(drM["sl_no"].ToString()), vNotes, SepType, 1, lcCurBal, lcPrvBal));

                        }
                        if (typecode == "B" && RetdEarnCode == drM["gl_seg_code"].ToString())
                        {
                            lcRptSysId = rptsysid + 1;
                            //string EndDateCheck =null;
                            //if (enddt.Equals("01/01/2017") || enddt.Equals("02/01/2017"))
                            //{
                            //    EndDateCheck = enddt;
                            //}
                            DataSet ds = null;
                            if (DataManager.DateEncode(startdt) >= DataManager.DateEncode("01/01/2018"))
                            {
                                 ds = generateBal(dt, book, "I", RefVerNo, pglcode, enddt, lcRptSysId,
                                    "01/01/2018");
                            }
                            else
                            {
                                 ds = generateBal(dt, book, "I", RefVerNo, pglcode, enddt, lcRptSysId,
                                    null);
                            }
                            DataTable dtRpt = ds.Tables[0];
                            if (dtRpt.Rows.Count > 0)
                            {
                                lcRetErnAmtCur = decimal.Parse(((DataRow)dtRpt.Rows[dtRpt.Rows.Count - 1])["amount_a"].ToString());
                                //if (DataManager.DateEncode(startdt) >= DataManager.DateEncode("01/01/2018"))
                                //{
                                //    lcRetErnAmtPrv = Convert.ToDecimal(-10156573.083);
                                //}
                                //else
                                //{
                                    lcRetErnAmtPrv = decimal.Parse(((DataRow)dtRpt.Rows[dtRpt.Rows.Count - 1])["amount_b"].ToString());
                                //}
                                
                            }
                            else
                            {
                                lcRetErnAmtCur = 0; lcRetErnAmtPrv = 0;
                            }

                            //if (DataManager.DateEncode(startdt) >= DataManager.DateEncode("01/01/2018"))
                            //{
                            //    CurBalRetainArn = Convert.ToDecimal(-6875403.433);
                            //}
                            //else
                            //{
                            CurBalRetainArn = lcCurBal + lcRetErnAmtPrv;
                            //}

                            if (enddt.Equals("02/01/2017"))
                            {
                                lcRetErnAmtCur = -Convert.ToDecimal(ConfigurationManager.AppSettings["CurBalRetainArn"]);
                                // 9260583.694;
                                //EndDateCheck = enddt;
                            }
                            CurBalProfit = lcRetErnAmtCur;
                            lcCurBal = lcCurBal + lcRetErnAmtCur+ lcRetErnAmtPrv; // Add This Previous
                            lcPrvBal = lcPrvBal+ lcRetErnAmtPrv;
                        }
                    }
                    else
                    {
                        lcCurBal = 0;
                        lcPrvBal = 0;
                        DataTable dtBreakDtl = getBreakDtl(typecode, verno, int.Parse(drM["sl_no"].ToString()), book);
                        foreach (DataRow drB in dtBreakDtl.Rows)
                        {
                            //*********************** Add Code *********************//
                            //if (drB["Gl_Coa_Code_BR"].ToString().Equals("1030002") && typecode.Equals("B"))
                            //{
                            //    drB["Gl_Coa_Code_BR"] = "1030001";
                            //}
                            if (drB["sl_no"].ToString().Equals("0"))
                            {

                                if (drB["Gl_Coa_Code_BR"].ToString().Equals("1030002"))
                                {
                                    if (enddt.Equals("01/01/2017"))
                                    {
                                        vAmountA = (decimal)ClosingStock;
                                    }
                                    else
                                    {
                                        vAmountA = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                            pglcode.Replace(segcode, drB["Gl_Coa_Code_BR"].ToString()), SepType, "A",
                                            startdt,
                                            enddt, null, null);
                                    }
                                }
                                
                                else
                                {
                                    vAmountA = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                        pglcode.Replace(segcode, drB["Gl_Coa_Code_BR"].ToString()), SepType, "A",
                                        startdt,
                                        enddt, null, null);
                                }
                                if (LastClosedDate == "")
                                {
                                    vAmountB = 0;
                                }
                                else
                                {

                                    vAmountB = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                        pglcode.Replace(segcode, drB["Gl_Coa_Code_BR"].ToString()), SepType, "A", "",
                                        LastClosedDate, null, null);
                                }

                                //*********************** Retained Earning *********//

                                if (typecode == "B" && RetdEarnCode == drB["Gl_Coa_Code_BR"].ToString())
                                {
                                    lcRptSysId = rptsysid + 1;
                                    DataSet ds = null;
                                    if (DataManager.DateEncode(startdt) >= DataManager.DateEncode("01/01/2018"))
                                    {
                                        ds = generateBal(dt, book, "I", RefVerNo, pglcode, enddt, lcRptSysId,
                                           "01/01/2018");
                                    }
                                    else
                                    {
                                        ds = generateBal(dt, book, "I", RefVerNo, pglcode, enddt, lcRptSysId,
                                           null);
                                    }
                                    DataTable dtRpt = ds.Tables[0];
                                    if (dtRpt.Rows.Count > 0)
                                    {
                                        lcRetErnAmtCur = decimal.Parse(((DataRow)dtRpt.Rows[dtRpt.Rows.Count - 1])["amount_a"].ToString());
                                        lcRetErnAmtPrv = decimal.Parse(((DataRow)dtRpt.Rows[dtRpt.Rows.Count - 1])["amount_b"].ToString());
                                    }
                                    else
                                    {
                                        lcRetErnAmtCur = 0; lcRetErnAmtPrv = 0;
                                    }
                                    //CurBalRetainArn = lcCurBal + lcRetErnAmtPrv;
                                    CurBalRetainArn = vAmountA + lcRetErnAmtPrv;
                                    if (enddt.Equals("02/01/2017"))
                                    {
                                        lcRetErnAmtCur = -Convert.ToDecimal(ConfigurationManager.AppSettings["CurBalRetainArn"]);
                                    }
                                    CurBalProfit = lcRetErnAmtCur;
                                    vAmountA = vAmountA + lcRetErnAmtCur + lcRetErnAmtPrv; // Add This Previous
                                    vAmountB = vAmountB + lcRetErnAmtPrv;
                                }
                            }
                            //******************************* ##### ***************************//
                            else if (drB["sl_no"].ToString() == "")
                            {

                                vAmountA = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                    pglcode.Replace(segcode, drB["gl_seg_code"].ToString()), SepType, "A", startdt,
                                    enddt, null, null);
                                if (LastClosedDate == "")
                                {
                                    vAmountB = 0;
                                }
                                else
                                {
                                    vAmountB = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                        pglcode.Replace(segcode, drB["gl_seg_code"].ToString()), SepType, "A", "",
                                        LastClosedDate, null, null);
                                }
                            }
                            else
                            {
                                DataTable dtBal = dtTmpRpt.Clone();
                                DataRow[] drBal =
                                    dtTmpRpt.Select("RPT_SYS_ID = '" + rptsysid + "' AND COL_NO = '" +
                                                    int.Parse(drB["sl_no"].ToString()) + "'");
                                if (drBal.Length > 0)
                                {
                                    foreach (DataRow dr in drBal)
                                    {
                                        if (string.IsNullOrEmpty(dr["amount_a"].ToString()))
                                        {
                                            dr["amount_a"] = "0";
                                        }
                                        dtBal.ImportRow(dr);
                                    }
                                }
                                if (dtBal.Rows.Count > 0)
                                {
                                    vAmountA = decimal.Parse(((DataRow)dtBal.Rows[0])["amount_a"].ToString());
                                    vAmountB = decimal.Parse(((DataRow)dtBal.Rows[0])["amount_b"].ToString());
                                }
                                else
                                {
                                    vAmountA = 0;
                                    vAmountB = 0;
                                }
                            }
                            if (drB["add_less"].ToString() == "L")
                            {
                                lcCurBal = lcCurBal - vAmountA;
                                lcPrvBal = lcPrvBal - vAmountB;
                            }
                            else
                            {
                                lcCurBal = lcCurBal + vAmountA;
                                lcPrvBal = lcPrvBal + vAmountB;
                            }
                        }
                    }
                    //vCoaDesc = drM["add_less"].ToString().Replace("L", "Less: ").Replace("A", "Add: ") + " " + drM["description"].ToString();
                    // Change This....
                    vCoaDesc = drM["add_less"].ToString().Replace("L", "").Replace("A", "") + " " + drM["description"].ToString();
                    //***********
                    DataRow drTR = dtTmpRpt.NewRow();
                    drTR["GL_SEG_CODE"] = drM["gl_seg_code"].ToString();
                    if (typecode == "B" & RetdEarnCode == drM["gl_seg_code"].ToString())
                    {
                        drTR["COA_DESC"] = vCoaDesc + "( Prv : " + CurBalRetainArn.ToString("N3") + " , Cur : " +
                                           CurBalProfit.ToString("N3") + " ).";
                    }
                    else
                    {
                        drTR["COA_DESC"] = vCoaDesc;
                    }

                    if (enddt.Equals("01/01/2017"))
                    {
                        //if (typecode.Equals("I") && vCoaDesc.Trim().Equals("Net Purchase"))
                        //{
                        //if (!string.IsNullOrEmpty(drTR["AMOUNT_A"].ToString()))
                        //{
                        //OpeningStock = lcCurBal;
                        //drTR["AMOUNT_A"] = OpeningStock;
                        // }
                        //}
                        if (typecode.Equals("I") && vCoaDesc.Trim().Equals("Closing Stock"))
                        {
                            //  OpeningStock = lcCurBal;
                            drTR["AMOUNT_A"] = ClosingStock;
                        }
                        else if (typecode.Equals("B") && vCoaDesc.Trim().Equals("Inventory Stock"))
                        {
                            drTR["AMOUNT_A"] = ClosingStock;
                        }
                        else
                        {
                            drTR["AMOUNT_A"] = lcCurBal;
                        }
                    }
                    else if (enddt.Equals("02/01/2017"))
                    {
                        if (typecode.Equals("I") && vCoaDesc.Trim().Equals("Opening Stock"))
                        {
                            //  OpeningStock = lcCurBal;
                            drTR["AMOUNT_A"] = ClosingStock;
                        }
                        else
                        {
                            drTR["AMOUNT_A"] = lcCurBal;
                        }
                    }
                    else
                    {
                        drTR["AMOUNT_A"] = lcCurBal;
                    }
                    drTR["AMOUNT_B"] = lcPrvBal;
                    drTR["AMOUNT_A_DT"] = enddt;
                    drTR["AMOUNT_B_DT"] = LastClosedDate;
                    drTR["COL_NO"] = drM["sl_no"].ToString();
                    drTR["NOTES"] = vNotes;
                    drTR["RPT_DATE"] = enddt;
                    drTR["TYPE_CODE"] = typecode;
                    drTR["VER_NO"] = verno;
                    drTR["RPT_SYS_ID"] = rptsysid;
                    drTR["BOOK_NAME"] = book;
                    dtTmpRpt.Rows.Add(drTR);
                }
            }
            DataSet dsTmpRpt = new DataSet();
            dsTmpRpt.Tables.Add(dtTmpRpt);
            dsTmpRpt.Tables.Add(dtTmpRptNotes);
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                SqlConnection.ClearPool(conn);
            }
            return dsTmpRpt; 
        }



        //*********************** Income Statement for Shopping Center *****************//

        public static DataSet generateShoppingCenterIncome(DataTable dt, string book, string typecode, string verno,
            string pglcode, string enddt, int rptsysid, string StartDt1)
        {
            DataTable dtTmpRpt = new DataTable();
            dtTmpRpt.Columns.Add("GL_SEG_CODE", typeof (string));
            dtTmpRpt.Columns.Add("COA_DESC", typeof (string));
            dtTmpRpt.Columns.Add("AMOUNT_A", typeof (decimal));
            dtTmpRpt.Columns.Add("AMOUNT_B", typeof (decimal));
            dtTmpRpt.Columns.Add("AMOUNT_A_DT", typeof (string));
            dtTmpRpt.Columns.Add("AMOUNT_B_DT", typeof (string));
            dtTmpRpt.Columns.Add("COL_NO", typeof (string));
            dtTmpRpt.Columns.Add("NOTES", typeof (string));
            dtTmpRpt.Columns.Add("RPT_DATE", typeof (string));
            dtTmpRpt.Columns.Add("TYPE_CODE", typeof (string));
            dtTmpRpt.Columns.Add("VER_NO", typeof (string));
            dtTmpRpt.Columns.Add("RPT_SYS_ID", typeof (string));
            dtTmpRpt.Columns.Add("BOOK_NAME", typeof (string));
            DataTable dtTmpRptNotes = new DataTable();
            string LastClosedDate = "";
            string RefVerNo = "";
            string RetdEarnCode = "";
            string SepType = "";
            string vNotes = "";
            int vNotesCount = 0;
            int lcRptSysId = 0;
            decimal lcCurBal = decimal.Zero;
            decimal lcPrvBal = decimal.Zero;
            string vCoaDesc = "";
            string vRootleaf = "";
            decimal lcRetErnAmtCur = decimal.Zero;
            decimal lcRetErnAmtPrv = decimal.Zero;
            decimal vAmountA = decimal.Zero;
            decimal vAmountB = decimal.Zero;

            decimal CurBalRetainArn = decimal.Zero;
            decimal CurBalProfit = decimal.Zero;

            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT RETD_EARN_ACC,Separator_type FROM GL_SET_OF_BOOKS WHERE BOOK_NAME = '" + book +
                              "' ";
            conn.Open();
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    RetdEarnCode = dReader["retd_earn_acc"].ToString();
                    SepType = dReader["separator_type"].ToString();
                }
            }
            else
            {
                throw new ArgumentException("Process Stopped! Retain earning account is not found.");
            }
            cmd.Dispose();
            dReader.Close();
            if (typecode == "B")
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT REF_VER_NO FROM GL_ST_MAP_MST WHERE BOOK_NAME = '" + book +
                                  "' AND TYPE_CODE = '" + typecode + "' AND VER_NO  = '" + verno + "' ";
                dReader = cmd.ExecuteReader();
                if (dReader.HasRows == true)
                {
                    while (dReader.Read())
                        RefVerNo = dReader["ref_ver_no"].ToString();
                }
                else
                {
                    throw new ArgumentException(
                        "Process Stopped! Please mention Income Statement Version reference for Balance Sheet in report setup form.");
                }
                cmd.Dispose();
                dReader.Close();
            }
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                "select convert(varchar,end_date,103) end_date from (SELECT MAX(END_DATE) end_date FROM GL_FIN_YEAR WHERE BOOK_NAME = '" +
                book + "' AND YEAR_FLAG " +
                " IN ('C','P') AND END_DATE < (SELECT START_DATE FROM GL_FIN_YEAR WHERE BOOK_NAME = '" + book + "' " +
                " AND convert(datetime,'" + enddt + "',103) >= START_DATE AND convert(datetime,'" + enddt +
                "',103) <= END_DATE)) tot2 ";
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                    LastClosedDate = dReader["end_date"].ToString();
            }
            else
            {
                LastClosedDate = "";
            }
            cmd.Dispose();
            dReader.Close();
            DataTable dtMapDtl = getMapDtl(typecode, verno, book);
            int lvlsize = 0;
            int lvlorder = 0;
            string segcode = "";
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                "SELECT lvl_code,lvl_max_size,lvl_order from gl_level_type where lvl_seg_type='N' and lvl_enabled='Y' and book_name='" +
                book + "'";
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    lvlsize = int.Parse(dReader["lvl_max_size"].ToString());
                    lvlorder = int.Parse(dReader["lvl_order"].ToString());
                }
            }
            cmd.Dispose();
            dReader.Close();
            if (lvlorder - 1 == 0)
            {
                lvlorder = 2;
            }
            segcode = pglcode.Substring(pglcode.IndexOf(SepType, 1, lvlorder - 1) + 1, lvlsize);
            string startdt;
            if (StartDt1 == null)
            {
                startdt = enddt;
            }
            else
            {
                startdt = StartDt1;
                LastClosedDate = DataManager.DateEncode(StartDt1).AddDays(-1).ToString("dd/MM/yyyy");
                //LastClosedDate = "01/01/2017";
            }
            decimal OpeningStock = decimal.Zero;
            foreach (DataRow drM in dtMapDtl.Rows)
            {
                vNotes = "";
                if (drM["cons_amt"].ToString().Trim() != "")
                {
                    vCoaDesc = drM["description"].ToString();
                    DataRow drTR = dtTmpRpt.NewRow();
                    drTR["GL_SEG_CODE"] = drM["gl_seg_code"].ToString();
                    drTR["COA_DESC"] = vCoaDesc;
                    drTR["AMOUNT_A"] = drM["cons_amt"].ToString();
                    drTR["AMOUNT_B"] = drM["cons_amt"].ToString();
                    drTR["AMOUNT_A_DT"] = enddt;
                    drTR["AMOUNT_B_DT"] = LastClosedDate;
                    drTR["COL_NO"] = drM["sl_no"].ToString();
                    drTR["NOTES"] = vNotes;
                    drTR["RPT_DATE"] = enddt;
                    drTR["TYPE_CODE"] = typecode;
                    drTR["VER_NO"] = verno;
                    drTR["RPT_SYS_ID"] = rptsysid;
                    drTR["BOOK_NAME"] = book;
                    dtTmpRpt.Rows.Add(drTR);
                }
                else
                {
                    if (drM["gl_seg_code"].ToString() != "")
                    {
                      //************************************** RDA ***************************************//
                        //** Change Opening Stock RDA ***//

                        if (drM["gl_seg_code"].ToString().Equals("1030003") && typecode.Equals("B1"))
                        {
                            DateTime dtStartDate = DataManager.DateEncode(startdt).AddDays(-1);
                            if (startdt.Equals("21/07/2018"))
                            {
                                lcCurBal = Convert.ToDecimal(ConfigurationManager.AppSettings["BranchRDAOpening"]);
                            }
                            else
                            {
                                lcCurBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                    pglcode.Replace(segcode, "1030004"), SepType, "A", "01/01/2010",
                                    dtStartDate.ToString("dd/MM/yyyy"),
                                    StartDt1, null);
                            }
                        }
                        //*** Closing main Stock RDA  ***//
                        else if (drM["gl_seg_code"].ToString().Equals("1030004") && typecode.Equals("B1"))
                        {
                            DateTime dtStartDate = DataManager.DateEncode(startdt).AddDays(-1);
                            lcCurBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), SepType, "A", "10/10/2017",
                                enddt,
                                StartDt1, null);
                        }
                        //************************************** LP-1 ***************************************//
                                                    //** Change Opening Stock lp-1 ***//
                        else if (drM["gl_seg_code"].ToString().Equals("1030008") && typecode.Equals("B2"))
                        {
                            DateTime dtStartDate = DataManager.DateEncode(startdt).AddDays(-1);
                            //if (DataManager.DateEncode(startdt) <= DataManager.DateEncode(("17/01/2019")))
                            //{
                            //    lcCurBal = Convert.ToDecimal(ConfigurationManager.AppSettings["BranchLpaOpening"]);
                            //}
                            //if (startdt.Equals("17/01/2019"))
                            //{
                            //    lcCurBal = Convert.ToDecimal(ConfigurationManager.AppSettings["BranchLpaOpening"]);
                            //}
                            //else
                            //{
                                lcCurBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                    pglcode.Replace(segcode, "1030009"), SepType, "A", "01/01/2010",
                                    dtStartDate.ToString("dd/MM/yyyy"),
                                    StartDt1, null);
                            //}
                        }
                                              //*** Closing main Stock lp-2  ***//
                        else if (drM["gl_seg_code"].ToString().Equals("1030009") && typecode.Equals("B2"))
                        {
                           // DateTime dtStartDate = DataManager.DateEncode(startdt).AddDays(-1);
                            lcCurBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), SepType, "A", "17/01/2019",
                                enddt,
                                StartDt1, null);
                        }
                        else
                        {

                            lcCurBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), SepType, "A", startdt,
                                enddt,
                                StartDt1, null);
                        }

                        if (LastClosedDate == "")
                        {
                            lcPrvBal = 0;
                        }
                        else
                        {
                            lcPrvBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), SepType, "A", "",
                                LastClosedDate, LastClosedDate, null);
                            //lcPrvBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                            //    pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), SepType, "A", "",
                            //    "31/12/2017", "31/12/2017", null);
                        }
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText =
                            "SELECT GL_SEG_COA.SEG_COA_DESC,GL_SEG_COA.ROOTLEAF FROM GL_LEVEL_TYPE, GL_SEG_COA " +
                            " WHERE GL_LEVEL_TYPE.BOOK_NAME = '" + book + "' AND GL_SEG_COA.SEG_COA_CODE = '" +
                            drM["gl_seg_code"].ToString() + "' " +
                            " AND GL_LEVEL_TYPE.BOOK_NAME = GL_SEG_COA.BOOK_NAME AND GL_LEVEL_TYPE.LVL_CODE = GL_SEG_COA.LVL_CODE " +
                            " AND GL_LEVEL_TYPE.LVL_SEG_TYPE = 'N'";
                        dReader = cmd.ExecuteReader();
                        if (dReader.HasRows == true)
                        {
                            while (dReader.Read())
                            {
                                vCoaDesc = dReader["seg_coa_desc"].ToString();
                                vRootleaf = dReader["rootleaf"].ToString().Replace("L", "R");
                            }
                        }
                        cmd.Dispose();
                        dReader.Close();
                        if (vRootleaf == "R")
                        {
                            vNotesCount = vNotesCount + 1;
                            vNotes = Math.Round(decimal.Parse(vNotesCount.ToString()), 2).ToString();
                            //******** Add Parameter (StartDt1)
                            dtTmpRptNotes.Merge(generateIncBalNotes(dt, book,
                                pglcode.Replace(segcode, drM["gl_seg_code"].ToString()), LastClosedDate, StartDt1, enddt,
                                rptsysid,
                                int.Parse(drM["sl_no"].ToString()), vNotes, SepType, 1, lcCurBal, lcPrvBal));
                        }
                    }
                    else
                    {
                        lcCurBal = 0;
                        lcPrvBal = 0;
                        DataTable dtBreakDtl = getBreakDtl(typecode, verno, int.Parse(drM["sl_no"].ToString()), book);
                        foreach (DataRow drB in dtBreakDtl.Rows)
                        {
                            //*********************** Add Code *********************//
                            //if (drB["Gl_Coa_Code_BR"].ToString().Equals("1030002") && typecode.Equals("B"))
                            //{
                            //    drB["Gl_Coa_Code_BR"] = "1030001";
                            //}
                            if (drB["sl_no"].ToString().Equals("0"))
                            {
                                vAmountA = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                    pglcode.Replace(segcode, drB["Gl_Coa_Code_BR"].ToString()), SepType, "A",
                                    startdt,
                                    enddt, null, null);

                                if (LastClosedDate == "")
                                {
                                    vAmountB = 0;
                                }
                            }
                                //******************************* ##### ***************************//
                            else if (drB["sl_no"].ToString() == "")
                            {

                                vAmountA = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                    pglcode.Replace(segcode, drB["gl_seg_code"].ToString()), SepType, "A", startdt,
                                    enddt, null, null);
                                if (LastClosedDate == "")
                                {
                                    vAmountB = 0;
                                }
                                else
                                {
                                    vAmountB = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                                        pglcode.Replace(segcode, drB["gl_seg_code"].ToString()), SepType, "A", "",
                                        LastClosedDate, null, null);
                                }
                            }
                            else
                            {
                                DataTable dtBal = dtTmpRpt.Clone();
                                DataRow[] drBal =
                                    dtTmpRpt.Select("RPT_SYS_ID = '" + rptsysid + "' AND COL_NO = '" +
                                                    int.Parse(drB["sl_no"].ToString()) + "'");
                                if (drBal.Length > 0)
                                {
                                    foreach (DataRow dr in drBal)
                                    {
                                        if (string.IsNullOrEmpty(dr["amount_a"].ToString()))
                                        {
                                            dr["amount_a"] = "0";
                                        }
                                        dtBal.ImportRow(dr);
                                    }
                                }
                                if (dtBal.Rows.Count > 0)
                                {
                                    vAmountA = decimal.Parse(((DataRow) dtBal.Rows[0])["amount_a"].ToString());
                                    vAmountB = decimal.Parse(((DataRow) dtBal.Rows[0])["amount_b"].ToString());
                                }
                                else
                                {
                                    vAmountA = 0;
                                    vAmountB = 0;
                                }
                            }
                            if (drB["add_less"].ToString() == "L")
                            {
                                lcCurBal = lcCurBal - vAmountA;
                                lcPrvBal = lcPrvBal - vAmountB;
                            }
                            else
                            {
                                lcCurBal = lcCurBal + vAmountA;
                                lcPrvBal = lcPrvBal + vAmountB;
                            }
                        }
                    }
                    vCoaDesc = drM["add_less"].ToString().Replace("L", "").Replace("A", "") + " " +
                               drM["description"].ToString();
                    //***********
                    DataRow drTR = dtTmpRpt.NewRow();
                    drTR["GL_SEG_CODE"] = drM["gl_seg_code"].ToString();
                    drTR["COA_DESC"] = vCoaDesc;
                    drTR["AMOUNT_A"] = lcCurBal;
                    drTR["AMOUNT_B"] = lcPrvBal;
                    drTR["AMOUNT_A_DT"] = enddt;
                    drTR["AMOUNT_B_DT"] = LastClosedDate;
                    drTR["COL_NO"] = drM["sl_no"].ToString();
                    drTR["NOTES"] = vNotes;
                    drTR["RPT_DATE"] = enddt;
                    drTR["TYPE_CODE"] = typecode;
                    drTR["VER_NO"] = verno;
                    drTR["RPT_SYS_ID"] = rptsysid;
                    drTR["BOOK_NAME"] = book;
                    dtTmpRpt.Rows.Add(drTR);
                }
            }
            DataSet dsTmpRpt = new DataSet();
            dsTmpRpt.Tables.Add(dtTmpRpt);
            dsTmpRpt.Tables.Add(dtTmpRptNotes);
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                SqlConnection.ClearPool(conn);
            }
            return dsTmpRpt;
        }

        public static DataTable generateIncBalNotes(DataTable dt, string book, string pglcode, string lastclosedt,
            string startDt, string enddt,
            int rptsysid, int colno, string pNotes, string septyp, int plevel, decimal curbal, decimal prvbal)
        {
            DataTable dtTmpRptNotes = new DataTable();
            dtTmpRptNotes.Columns.Add("RPT_SYS_ID", typeof (string));
            dtTmpRptNotes.Columns.Add("COL_NO", typeof (string));
            dtTmpRptNotes.Columns.Add("GL_SEG_CODE", typeof (string));
            dtTmpRptNotes.Columns.Add("COA_DESC", typeof (string));
            dtTmpRptNotes.Columns.Add("NOTES", typeof (string));
            dtTmpRptNotes.Columns.Add("Parent_NOTES", typeof (string));
            dtTmpRptNotes.Columns.Add("AMOUNT_A", typeof (decimal));
            dtTmpRptNotes.Columns.Add("AMOUNT_B", typeof (decimal));
            dtTmpRptNotes.Columns.Add("AMOUNT_A_DT", typeof (string));
            dtTmpRptNotes.Columns.Add("AMOUNT_B_DT", typeof (string));
            int vNotesCount = 0;
            string vNotes = "";
            decimal lcCurBal = decimal.Zero;
            decimal lcPrvBal = decimal.Zero;
            string vCoaDesc = "";
            string segcode = "";
            int lvlsize = 0;
            int lvlorder = 0;
            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd;
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                "SELECT lvl_max_size,lvl_order from gl_level_type where lvl_seg_type='N' and lvl_enabled='Y' and book_name='" +
                book + "'";
            conn.Open();
            dReader = cmd.ExecuteReader();

            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    lvlsize = int.Parse(dReader["lvl_max_size"].ToString());
                    lvlorder = int.Parse(dReader["lvl_order"].ToString());
                }
            }
            cmd.Dispose();
            dReader.Close();
            if (lvlorder - 1 == 0)
            {
                lvlorder = 2;
            }
            segcode = pglcode.Substring(pglcode.IndexOf(septyp, 1, lvlorder - 1) + 1, lvlsize);

            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                "SELECT GL_SEG_COA.SEG_COA_DESC FROM GL_LEVEL_TYPE,GL_SEG_COA WHERE GL_LEVEL_TYPE.BOOK_NAME = '" + book +
                "' " +
                " AND GL_SEG_COA.seg_coa_code    = '" + segcode +
                "' AND GL_LEVEL_TYPE.BOOK_NAME = GL_SEG_COA.BOOK_NAME " +
                " AND GL_LEVEL_TYPE.LVL_CODE = GL_SEG_COA.LVL_CODE AND GL_LEVEL_TYPE.LVL_SEG_TYPE = 'N' ";
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                    vCoaDesc = dReader["seg_coa_desc"].ToString();
            }
            cmd.Dispose();
            dReader.Close();

            DataRow drTR = dtTmpRptNotes.NewRow();
            drTR["RPT_SYS_ID"] = rptsysid;
            drTR["COL_NO"] = colno;
            drTR["GL_SEG_CODE"] = segcode;
            drTR["COA_DESC"] = vCoaDesc;
            drTR["NOTES"] = pNotes;
            drTR["Parent_NOTES"] = "";
            drTR["AMOUNT_A"] = curbal;
            drTR["AMOUNT_B"] = prvbal;
            drTR["AMOUNT_A_DT"] = enddt;
            drTR["AMOUNT_B_DT"] = lastclosedt;
            dtTmpRptNotes.Rows.Add(drTR);

            DataTable dtLeafCoa = getLeafCoa(segcode, book);
            foreach (DataRow dr in dtLeafCoa.Rows)
            {
                vNotes = "";
                string StartDate = "";
                if (startDt == null)
                {
                    StartDate = DateTime.Now.ToString("dd/MM/yyyy");
                }
                else
                {
                    StartDate = startDt;
                   // lastclosedt = startDt;
                }
                lcCurBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                    pglcode.Replace(segcode, dr["seg_coa_code"].ToString()), septyp, "A", StartDate, enddt, startDt, null);
                if (lastclosedt == "")
                {
                    lcPrvBal = 0;
                }
                else
                {
                    lcPrvBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dt, book,
                        pglcode.Replace(segcode, dr["seg_coa_code"].ToString()), septyp, "A", "", lastclosedt, null, null);
                }
                if (dr["rootleaf"].ToString() == "R")
                {
                    vNotesCount = vNotesCount + 1;
                    vNotes = Math.Round(double.Parse(pNotes) + (vNotesCount/Math.Pow(10, plevel)), 2).ToString();
                }
                drTR = dtTmpRptNotes.NewRow();
                drTR["RPT_SYS_ID"] = rptsysid;
                drTR["COL_NO"] = colno;
                drTR["GL_SEG_CODE"] = dr["seg_coa_code"].ToString();
                drTR["COA_DESC"] = dr["seg_coa_desc"].ToString();
                drTR["NOTES"] = vNotes;
                drTR["Parent_NOTES"] = pNotes;
                drTR["AMOUNT_A"] = lcCurBal;
                drTR["AMOUNT_B"] = lcPrvBal;
                drTR["AMOUNT_A_DT"] = enddt;
                drTR["AMOUNT_B_DT"] = lastclosedt;
                dtTmpRptNotes.Rows.Add(drTR);
                if (dr["rootleaf"].ToString() == "R")
                {
                    DataTable dtRptNotes = generateIncBalNotes(dt, book,
                        pglcode.Replace(segcode, dr["seg_coa_code"].ToString()), lastclosedt, startDt, enddt, rptsysid,
                        colno,
                        vNotes, septyp, plevel + 1, lcCurBal, lcPrvBal);
                    foreach (DataRow drRptNotes in dtRptNotes.Rows)
                    {
                        dtTmpRptNotes.ImportRow(drRptNotes);
                    }
                }
            }
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                SqlConnection.ClearPool(conn);
            }
            return dtTmpRptNotes;
        }
    }
}