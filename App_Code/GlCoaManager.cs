using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Delve;

/// <summary>
/// Summary description for GlCoaManager
/// </summary>
/// 
namespace Delve
{
    public class GlCoaManager
    {
        public static void CreateGlCoa(GlCoa glcoa)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            
            string query = "insert into gl_coa (book_name,gl_coa_code,coa_enabled,effective_from,effective_to, "+
                " bud_allowed,post_allowed,taxable,acc_type,status,coa_desc,coa_curr_bal,coa_natural_code,ENTRY_USER,ENTRY_DATE,AUTHO_USER,AUTHO_DATE) values ( " +
                " '" + glcoa.BookName + "',  '" + glcoa.GlCoaCode + "',  '" + glcoa.CoaEnabled + "', "+
                "  convert(datetime,case '" + glcoa.EffectiveFrom + "' when '' then null else '" + glcoa.EffectiveFrom + "' end ,103), convert(datetime,case '" + glcoa.EffectiveTo + "' when '' then null else '" + glcoa.EffectiveTo + "' end ,103), '" + glcoa.BudAllowed + "', " +
                "  '" + glcoa.PostAllowed + "',  '" + glcoa.Taxable + "',  '" + glcoa.AccType + "', "+
                "  '" + glcoa.Status + "',  '" + glcoa.CoaDesc.Replace("'","") + "',  '" + glcoa.CoaCurrBal + "', "+
                "  '" + glcoa.CoaNaturalCode + "','" + glcoa.LoginBy + "',GETDATE(),'" + glcoa.LoginBy + "',GETDATE()) ";

            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void UpdateGlCoa(GlCoa glcoa)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = "update gl_coa set gl_coa_code= '" + glcoa.GlCoaCode + "', coa_enabled= '" + glcoa.CoaEnabled + "', " +
                " effective_from= convert(datetime,case '" + glcoa.EffectiveFrom + "' when '' then null else '" + glcoa.EffectiveFrom + "' end ,103), effective_to= convert(datetime,case '" + glcoa.EffectiveTo + "' when '' then null else '" + glcoa.EffectiveTo + "' end ,103), bud_allowed= '" + glcoa.BudAllowed + "', " +
                " post_allowed= '" + glcoa.PostAllowed + "', taxable= '" + glcoa.Taxable + "', acc_type= '" + glcoa.AccType + "', " +
                " status= '" + glcoa.Status + "', coa_desc= '" + glcoa.CoaDesc + "', coa_curr_bal= '" + glcoa.CoaCurrBal + "', " +
                " coa_natural_code= '" + glcoa.CoaNaturalCode + "',UPDATE_USER='" + glcoa.LoginBy + "',UPDATE_DATE=GETDATE() where gl_coa_code='" + glcoa.GlCoaCode + "' ";

            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static DataTable GetGlCoaCodes()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select * from gl_coa ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "GlCoa");
            return dt;
        }
        public static DataTable GetSegCodes(string book)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "SELECT SEG_COA_CODE,seg_coa_desc,parent_code,rootleaf FROM GL_SEG_COA S WHERE BOOK_NAME = '"+book+"' and  status='A' "+      
            " CONNECT BY PRIOR SEG_COA_CODE=PARENT_CODE  AND book_name='"+book+"' START WITH parent_code is null "+
             "AND book_name='"+book+"' ORDER BY lvl_code,SEG_COA_CODE ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SegmentCodes");
            return dt;
        }
        public static DataTable GetGlCoaCode(string criteria)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select GL_COA_CODE, COA_ENABLED, EFFECTIVE_FROM, EFFECTIVE_TO, BUD_ALLOWED, POST_ALLOWED, TAXABLE, ACC_TYPE, STATUS, BOOK_NAME, ENTRY_USER, ENTRY_DATE, "+
                " UPDATE_USER, UPDATE_DATE, AUTHO_USER, AUTHO_DATE, COA_DESC, COA_CURR_BAL, COA_NATURAL_CODE,'' INC from gl_coa ";
            if (criteria != "")
            {
                query = query + " where " + criteria + " order by gl_coa_code";
            }
            else
            {
                query = query + " order by convert(numeric,gl_coa_code) ";
            }
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "GlCoa");
            return dt;
        }

        public static DataTable GetGlCoaCodes(string query)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "GlCoa");
            return dt;
        }

        public static void DeleteGlCoa(GlCoa glcoa)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " delete from gl_coa where gl_coa_code='" + glcoa.GlCoaCode + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static GlCoa getGlCoa(string segcoacode)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = "select GL_COA_CODE, COA_ENABLED, convert(varchar,EFFECTIVE_FROM,103) EFFECTIVE_FROM, convert(varchar,EFFECTIVE_TO,103) EFFECTIVE_TO, BUD_ALLOWED, POST_ALLOWED, TAXABLE, ACC_TYPE, STATUS, BOOK_NAME, ENTRY_USER, convert(varchar,ENTRY_DATE,103) ENTRY_DATE, " +
                    " UPDATE_USER, convert(varchar,UPDATE_DATE,103) UPDATE_DATE, AUTHO_USER, convert(varchar,AUTHO_DATE,103) AUTHO_DATE, COA_DESC, COA_CURR_BAL, COA_NATURAL_CODE,'' INC from gl_coa where gl_coa_code='" + segcoacode + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Gl_Coa");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new GlCoa(dt.Rows[0]);
        }
        public static string getCoaBalance(string book,string glcoacode, string sepTyp,string vchno,string status,string fromdt,string todt)
        {
            char sep = Convert.ToChar(sepTyp);
            string[] segcode = glcoacode.Split(sep);
            int x = 0;
            int y = 0;
            string criteria = "";
            int[] lvlSize = new int[segcode.Length];
            string[] lvlcode = new string[segcode.Length];
            string query = "";
            for (int i = 0; i < segcode.Length; i++)
            {
                string connectionString = DataManager.OraConnString();
                SqlDataReader dReader;
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connectionString;                
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select lvl_code,lvl_max_size from gl_level_type where book_name='" + book + "' and lvl_code=(select lvl_code from gl_seg_coa where seg_coa_code ='" + segcode[i].ToString() + "')";
                conn.Open();
                dReader = cmd.ExecuteReader();
                if (dReader.HasRows == true)
                {
                    while (dReader.Read())
                    {
                        lvlSize[x] = int.Parse(dReader["lvl_max_size"].ToString());
                        lvlcode[x] = dReader["lvl_code"].ToString();
                    }
                }
                if (x == 0 && segcode[i].ToString() != "")
                {
                    query = "with t" + x.ToString() + "(seg_coa_code,seg_coa_desc,parent_code,rootleaf) as " +
                    " (select seg_coa_code,seg_coa_desc,PARENT_CODE,rootleaf from GL_SEG_COA where SEG_COA_CODE='" + segcode[i].ToString() + "' and LVL_CODE='" + lvlcode[x] + "' " +
                    " union all select tt" + x.ToString() + ".seg_coa_code,tt" + x.ToString() + ".seg_coa_desc,tt" + x.ToString() + ".parent_code,tt" + x.ToString() + ".ROOTLEAF from GL_SEG_COA as tt" + x.ToString() + " ,t" + x.ToString() + " " +
                    " where t" + x.ToString() + ".seg_coa_code=tt" + x.ToString() + ".PARENT_CODE and tt" + x.ToString() + ".LVL_CODE='" + lvlcode[x] + "') ";
                    criteria += " substring(gl_coa_code,1," + lvlSize[x].ToString() + ") in (SELECT SEG_COA_CODE  FROM t" + x.ToString() + " where rootleaf='L' )  ";
                }
                else if (x > 0 && segcode[i].ToString() != "")
                {
                    query += ", t" + x.ToString() + "(seg_coa_code,seg_coa_desc,parent_code,rootleaf) as " +
                    " (select seg_coa_code,seg_coa_desc,PARENT_CODE,rootleaf from GL_SEG_COA where SEG_COA_CODE='" + segcode[i].ToString() + "' and LVL_CODE='" + lvlcode[x] + "' " +
                    " union all select tt" + x.ToString() + ".seg_coa_code,tt" + x.ToString() + ".seg_coa_desc,tt" + x.ToString() + ".parent_code,tt" + x.ToString() + ".ROOTLEAF from GL_SEG_COA as tt" + x.ToString() + " ,t" + x.ToString() + " " +
                    " where t" + x.ToString() + ".seg_coa_code=tt" + x.ToString() + ".PARENT_CODE and tt" + x.ToString() + ".LVL_CODE='" + lvlcode[x] + "') ";
                    criteria += " and substring(gl_coa_code,charindex('" + sepTyp + "',gl_coa_code," + y + ")+1," + lvlSize[x].ToString() + ") in (SELECT SEG_COA_CODE  FROM t" + x.ToString() + " where rootleaf='L' )  ";
                }                
                y = y + lvlSize[x];
                x = x + 1;
            }
            if (criteria != "")
            {
                query = query + "select GL_COA_CODE, COA_ENABLED, convert(varchar,EFFECTIVE_FROM,103) EFFECTIVE_FROM, convert(varchar,EFFECTIVE_TO,103) EFFECTIVE_TO, BUD_ALLOWED, POST_ALLOWED, TAXABLE, ACC_TYPE, STATUS, BOOK_NAME, ENTRY_USER, convert(varchar,ENTRY_DATE,103) ENTRY_DATE, " +
                    " UPDATE_USER, convert(varchar,UPDATE_DATE,103) UPDATE_DATE, AUTHO_USER, convert(varchar,AUTHO_DATE,103) AUTHO_DATE, COA_DESC, COA_CURR_BAL, COA_NATURAL_CODE,'' INC from gl_coa where " + criteria;
            }
            else
            {
                query = "select GL_COA_CODE, COA_ENABLED, convert(varchar,EFFECTIVE_FROM,103) EFFECTIVE_FROM, convert(varchar,EFFECTIVE_TO,103) EFFECTIVE_TO, BUD_ALLOWED, POST_ALLOWED, TAXABLE, ACC_TYPE, STATUS, BOOK_NAME, ENTRY_USER, convert(varchar,ENTRY_DATE,103) ENTRY_DATE, " +
                    " UPDATE_USER, convert(varchar,UPDATE_DATE,103) UPDATE_DATE, AUTHO_USER, convert(varchar,AUTHO_DATE,103) AUTHO_DATE, COA_DESC, COA_CURR_BAL, COA_NATURAL_CODE,'' INC from gl_coa";
            }
            DataTable dtAlready = GlCoaManager.GetGlCoaCodes(query);
            DataTable dtVchDtl = VouchManager.GetVouchDtlForTotal(todt, status, book);
            DataTable dtdtl = new DataTable();
            decimal bal = decimal.Zero;
            foreach (DataRow drdtA in dtAlready.Rows)
            {
                DataRow[] drtmp = dtVchDtl.Select("gl_coa_code='" + drdtA["gl_coa_code"].ToString().Trim() + "'");
                dtdtl = dtVchDtl.Clone();
                foreach (DataRow dr in drtmp)
                {
                    dtdtl.ImportRow(dr);
                }
                if (dtdtl.Rows.Count > 0)
                {
                    foreach (DataRow drdtl in dtdtl.Rows)
                    {
                        if (drdtl["amount_dr"].ToString() == "")
                        {
                            drdtl["amount_dr"] = "0";
                        }
                        if (drdtl["amount_cr"].ToString() == "")
                        {
                            drdtl["amount_cr"] = "0";
                        }
                        if (drdtl["acc_type"].ToString() == "A" | drdtl["acc_type"].ToString() == "E")
                        {
                            bal += decimal.Parse(drdtl["amount_dr"].ToString()) - decimal.Parse(drdtl["amount_cr"].ToString());
                        }
                        else if (drdtl["acc_type"].ToString() == "L" | drdtl["acc_type"].ToString() == "I")
                        {
                            bal += decimal.Parse(drdtl["amount_cr"].ToString()) - decimal.Parse(drdtl["amount_dr"].ToString());
                        }
                    }
                }
            }
            dtAlready.Dispose();
            dtdtl.Dispose();
            dtVchDtl.Dispose();
            return bal.ToString() ;
        }
        public static decimal getCoaBalanceFromExistingDataTable(DataTable dt, string book, string glcoacode, string sepTyp, string status, string fromdt, string todt, string StartDt1,int? ReportType)
        {
            string criteria = getCoaCriteria(glcoacode, sepTyp, book);
            DataTable dtAlready = GlCoaManager.GetGlCoaCodes(criteria);
            DataTable dtNewTable;
            if (ReportType.Equals(7))
            {
                //string OpeningStock = IdManager.GetShowSingleValueString("Opening_Stock", "ID", "FixGlCoaCode", "1");
                DataTable dtFixCode = IdManager.GetShowDataTable("SELECT * FROM [FixGlCoaCode]");
                //dtAlready = dtAlready.AsEnumerable()
                //    .Where(r => r.Field<string>("gl_coa_code") != OpeningStock || r => r.Field<string>("gl_coa_code") != OpeningStock ||)
                //    .CopyToDataTable();
                dtNewTable = new DataTable();
                dtNewTable = dtAlready.Clone();
                foreach (DataRow dr in dtAlready.Rows)
                {
                    //if (dtFixCode.Rows[0]["Opening_Stock"].ToString().Trim().Equals(dr["gl_coa_code"].ToString().Trim()))
                    //{
                      
                    //}
                    if (dtFixCode.Rows[0]["Closing_Stock"].ToString().Trim().Equals(dr["gl_coa_code"].ToString().Trim()))
                    {

                    }
                    else if (dtFixCode.Rows[0]["ClosingStockInRDALipa"].ToString().Trim().Equals(dr["gl_coa_code"].ToString().Trim()))
                    {

                    }
                    else
                    {
                        dtNewTable.ImportRow(dr);
                    }
                }
                if (dtNewTable != null)
                {
                    dtAlready = dtNewTable;
                }
            }
            DataTable dtdtl;
            decimal bal = decimal.Zero;
            if (fromdt == "")
            {
                fromdt = DateTime.MinValue.ToString("dd/MM/yyyy");
            }
            if (dtAlready.Columns.Count > 0)
            {
                foreach (DataRow drdtA in dtAlready.Rows)
                {
                    string expr = "gl_coa_code='" + drdtA["gl_coa_code"].ToString().Trim() + "'";
                    DataRow[] drtmp = dt.Select(expr);
                    dtdtl = new DataTable();
                    dtdtl = dt.Clone();
                    foreach (DataRow dr in drtmp)
                    {
                        dtdtl.ImportRow(dr);
                    }
                    if (dtdtl.Rows.Count > 0)
                    {
                        foreach (DataRow drdtl in dtdtl.Rows)
                        {

                            if (StartDt1 == null)
                            {
                                //int a = DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(fromdt));
                                //int b = DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(todt));

                                //if (DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(fromdt)) >= 0 &&
                                //    DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(todt)) < 1 )
                                //{
                                if (DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(todt)) <= 0)
                                {
                                    if (drdtl["amount_dr"].ToString() == "")
                                    {
                                        drdtl["amount_dr"] = "0";
                                    }
                                    if (drdtl["amount_cr"].ToString() == "")
                                    {
                                        drdtl["amount_cr"] = "0";
                                    }
                                    if (drdtl["acc_type"].ToString() == "A" | drdtl["acc_type"].ToString() == "E")
                                    {
                                        bal += decimal.Parse(drdtl["amount_dr"].ToString()) - decimal.Parse(drdtl["amount_cr"].ToString());
                                    }
                                    else if (drdtl["acc_type"].ToString() == "L" | drdtl["acc_type"].ToString() == "I")
                                    {
                                        bal += decimal.Parse(drdtl["amount_cr"].ToString()) - decimal.Parse(drdtl["amount_dr"].ToString());
                                    }
                                }
                            }
                            else
                            {
                                //int a = DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(fromdt));
                                //int b = DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(todt));
                                if (DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(fromdt)) >= 0 &&
                                    DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(todt)) < 1)
                                {

                                    if (drdtl["amount_dr"].ToString() == "")
                                    {
                                        drdtl["amount_dr"] = "0";
                                    }
                                    if (drdtl["amount_cr"].ToString() == "")
                                    {
                                        drdtl["amount_cr"] = "0";
                                    }
                                    if (drdtl["acc_type"].ToString() == "A" | drdtl["acc_type"].ToString() == "E")
                                    {
                                        bal += decimal.Parse(drdtl["amount_dr"].ToString()) - decimal.Parse(drdtl["amount_cr"].ToString());
                                    }
                                    else if (drdtl["acc_type"].ToString() == "L" | drdtl["acc_type"].ToString() == "I")
                                    {
                                        bal += decimal.Parse(drdtl["amount_cr"].ToString()) - decimal.Parse(drdtl["amount_dr"].ToString());
                                    }
                                }
                            }
                        }
                    }
                    dtdtl.Dispose();
                }
            }
            dtAlready.Dispose();
            return bal;
        }

        public static decimal getCoaopeningBalanceFromExistingDataTable(DataTable dt, string book, string glcoacode, string sepTyp, string status, string fromdt, string todt)
        {
            string criteria = getCoaCriteria(glcoacode, sepTyp, book);
            DataTable dtAlready = GlCoaManager.GetGlCoaCodes(criteria);
            DataTable dtdtl;
            decimal bal = decimal.Zero;
            if (fromdt == "")
            {
                fromdt = DateTime.MinValue.ToString("dd/MM/yyyy");
            }
            if (dtAlready.Columns.Count > 0)
            {
                foreach (DataRow drdtA in dtAlready.Rows)
                {
                    string expr = "gl_coa_code='" + drdtA["gl_coa_code"].ToString().Trim() + "'";
                    DataRow[] drtmp = dt.Select(expr);
                    dtdtl = new DataTable();
                    dtdtl = dt.Clone();
                    foreach (DataRow dr in drtmp)
                    {
                        dtdtl.ImportRow(dr);
                    }
                    if (dtdtl.Rows.Count > 0)
                    {
                        foreach (DataRow drdtl in dtdtl.Rows)
                        {
                            int a = DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(fromdt));
                            int b = DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(todt));

                            //if (DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(fromdt)) >= 0 &&
                            //    DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(todt)) < 1 && drdtl["status"].ToString() == status)
                            //{
                            if (DateTime.Parse(drdtl["value_date"].ToString()).CompareTo(DataManager.DateEncode(fromdt)) < 0)
                            {
                                if (drdtl["amount_dr"].ToString() == "")
                                {
                                    drdtl["amount_dr"] = "0";
                                }
                                if (drdtl["amount_cr"].ToString() == "")
                                {
                                    drdtl["amount_cr"] = "0";
                                }
                                if (drdtl["acc_type"].ToString() == "A" | drdtl["acc_type"].ToString() == "E")
                                {
                                    bal += decimal.Parse(drdtl["amount_dr"].ToString()) - decimal.Parse(drdtl["amount_cr"].ToString());
                                }
                                else if (drdtl["acc_type"].ToString() == "L" | drdtl["acc_type"].ToString() == "I")
                                {
                                    bal += decimal.Parse(drdtl["amount_cr"].ToString()) - decimal.Parse(drdtl["amount_dr"].ToString());
                                }
                                else if (Convert.ToDouble(drdtl["amount_cr"]) == 0 && Convert.ToDouble(drdtl["amount_dr"]) == 0)
                                {
                                    bal = 0;
                                }
                            }
                        }
                    }
                    dtdtl.Dispose();
                }
            }
            dtAlready.Dispose();
            return bal;
        }
        public static string getCoaExistence(string glcoa)
        {
            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand();
            string coaexist = "";
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select 'Y' coaexist from gl_coa where gl_coa_code='" + glcoa + "' ";
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    coaexist = dReader["coaexist"].ToString();
                }
            }
            cmd.Dispose(); dReader.Dispose();
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                SqlConnection.ClearPool(conn);
            }
            return coaexist;
        }

        public static string getCoaCriteria(string glCode, string separ, string book)
        {
            char sep = Convert.ToChar(separ);
            string[] segcode = glCode.Split(sep);
            int x = 0;
            int y = 0;
            string criteria = "";
            int[] lvlSize = new int[segcode.Length];
            string[] lvlcode = new string[segcode.Length];
            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand();
            string query = "";
            for (int i = 0; i < segcode.Length; i++)
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select lvl_code,lvl_max_size from gl_level_type where book_name='" + book +
                                  "' and lvl_code=(select lvl_code from gl_seg_coa where seg_coa_code ='" +
                                  segcode[i].ToString() + "')";
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                dReader = cmd.ExecuteReader();
                if (dReader.HasRows == true)
                {
                    while (dReader.Read())
                    {
                        lvlSize[x] = int.Parse(dReader["lvl_max_size"].ToString());
                        lvlcode[x] = dReader["lvl_code"].ToString();
                    }
                }

                cmd.Dispose();
                dReader.Dispose();
                if (x == 0 && segcode[i].ToString() != "")
                {
                    query = "with t" + x.ToString() + "(seg_coa_code,seg_coa_desc,parent_code,rootleaf) as " +
                            " (select seg_coa_code,seg_coa_desc,PARENT_CODE,rootleaf from GL_SEG_COA where SEG_COA_CODE='" +
                            segcode[i].ToString() + "' and LVL_CODE='" + lvlcode[x] + "' " +
                            " union all select tt" + x.ToString() + ".seg_coa_code,tt" + x.ToString() +
                            ".seg_coa_desc,tt" + x.ToString() + ".parent_code,tt" + x.ToString() +
                            ".ROOTLEAF from GL_SEG_COA as tt" + x.ToString() + " ,t" + x.ToString() + " " +
                            " where t" + x.ToString() + ".seg_coa_code=tt" + x.ToString() + ".PARENT_CODE and tt" +
                            x.ToString() + ".LVL_CODE='" + lvlcode[x] + "') ";
                    criteria += " substring(gl_coa_code,1," + lvlSize[x].ToString() +
                                ") in (SELECT SEG_COA_CODE  FROM t" + x.ToString() + " where rootleaf='L' )  ";
                }
                else if (x > 0 && segcode[i].ToString() != "")
                {
                    query += ", t" + x.ToString() + "(seg_coa_code,seg_coa_desc,parent_code,rootleaf) as " +
                             " (select seg_coa_code,seg_coa_desc,PARENT_CODE,rootleaf from GL_SEG_COA where SEG_COA_CODE='" +
                             segcode[i].ToString() + "' and LVL_CODE='" + lvlcode[x] + "' " +
                             " union all select tt" + x.ToString() + ".seg_coa_code,tt" + x.ToString() +
                             ".seg_coa_desc,tt" + x.ToString() + ".parent_code,tt" + x.ToString() +
                             ".ROOTLEAF from GL_SEG_COA as tt" + x.ToString() + " ,t" + x.ToString() + " " +
                             " where t" + x.ToString() + ".seg_coa_code=tt" + x.ToString() + ".PARENT_CODE and tt" +
                             x.ToString() + ".LVL_CODE='" + lvlcode[x] + "') ";
                    criteria += " and substring(gl_coa_code,charindex('" + sep + "',gl_coa_code," + y + ")+1," +
                                lvlSize[x].ToString() + ") in (SELECT SEG_COA_CODE  FROM t" + x.ToString() +
                                " where rootleaf='L' ) ";
                }

                y = y + lvlSize[x];
                x = x + 1;
            }

            if (criteria != "")
            {
                query = query +
                        "select GL_COA_CODE, COA_ENABLED, convert(varchar,EFFECTIVE_FROM,103) EFFECTIVE_FROM, convert(varchar,EFFECTIVE_TO,103) EFFECTIVE_TO, BUD_ALLOWED, POST_ALLOWED, TAXABLE, ACC_TYPE, STATUS, BOOK_NAME, ENTRY_USER, convert(varchar,ENTRY_DATE,103) ENTRY_DATE, " +
                        " UPDATE_USER, convert(varchar,UPDATE_DATE,103) UPDATE_DATE, AUTHO_USER, convert(varchar,AUTHO_DATE,103) AUTHO_DATE, COA_DESC, COA_CURR_BAL, COA_NATURAL_CODE,'' INC from gl_coa where " +
                        criteria;
            }
            else
            {
                query =
                    "select GL_COA_CODE, COA_ENABLED, convert(varchar,EFFECTIVE_FROM,103) EFFECTIVE_FROM, convert(varchar,EFFECTIVE_TO,103) EFFECTIVE_TO, BUD_ALLOWED, POST_ALLOWED, TAXABLE, ACC_TYPE, STATUS, BOOK_NAME, ENTRY_USER, convert(varchar,ENTRY_DATE,103) ENTRY_DATE, " +
                    " UPDATE_USER, convert(varchar,UPDATE_DATE,103) UPDATE_DATE, AUTHO_USER, convert(varchar,AUTHO_DATE,103) AUTHO_DATE, COA_DESC, COA_CURR_BAL, COA_NATURAL_CODE,'' INC from gl_coa";
            }

            return query;
        }

        //************************** New METHORD *********************//
        public static string getNEWCoaCriteria(string glCode, string separ, string book)
        {
            char sep = Convert.ToChar(separ);
            string[] segcode = glCode.Split(sep);
            int x = 0;
            int y = 0;
            string criteria = "";
            int[] lvlSize = new int[segcode.Length];
            string[] lvlcode = new string[segcode.Length];
            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand();
            string query = "";
            for (int i = 0; i < segcode.Length; i++)
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select lvl_code,lvl_max_size from gl_level_type where book_name='" + book + "' and lvl_code=(select lvl_code from gl_seg_coa where seg_coa_code ='" + segcode[i].ToString() + "')";
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                dReader = cmd.ExecuteReader();
                if (dReader.HasRows == true)
                {
                    while (dReader.Read())
                    {
                        lvlSize[x] = int.Parse(dReader["lvl_max_size"].ToString());
                        lvlcode[x] = dReader["lvl_code"].ToString();
                    }
                }
                cmd.Dispose(); dReader.Dispose();
                if (x == 0 && segcode[i].ToString() != "")
                {
                    query = "with t" + x.ToString() + "(seg_coa_code,seg_coa_desc,parent_code,rootleaf) as " +
                    " (select seg_coa_code,seg_coa_desc,PARENT_CODE,rootleaf from GL_SEG_COA where SEG_COA_CODE='" + segcode[i].ToString() + "' and LVL_CODE='" + lvlcode[x] + "' " +
                    " union all select tt" + x.ToString() + ".seg_coa_code,tt" + x.ToString() + ".seg_coa_desc,tt" + x.ToString() + ".parent_code,tt" + x.ToString() + ".ROOTLEAF from GL_SEG_COA as tt" + x.ToString() + " ,t" + x.ToString() + " " +
                    " where t" + x.ToString() + ".seg_coa_code=tt" + x.ToString() + ".PARENT_CODE and tt" + x.ToString() + ".LVL_CODE='" + lvlcode[x] + "') ";
                    criteria += " substring(gl_coa_code,1," + lvlSize[x].ToString() + ") in (SELECT SEG_COA_CODE  FROM t" + x.ToString() + " where rootleaf='L' )  ";
                }
                else if (x > 0 && segcode[i].ToString() != "")
                {
                    query += ", t" + x.ToString() + "(seg_coa_code,seg_coa_desc,parent_code,rootleaf) as " +
                    " (select seg_coa_code,seg_coa_desc,PARENT_CODE,rootleaf from GL_SEG_COA where SEG_COA_CODE='" + segcode[i].ToString() + "' and LVL_CODE='" + lvlcode[x] + "' " +
                    " union all select tt" + x.ToString() + ".seg_coa_code,tt" + x.ToString() + ".seg_coa_desc,tt" + x.ToString() + ".parent_code,tt" + x.ToString() + ".ROOTLEAF from GL_SEG_COA as tt" + x.ToString() + " ,t" + x.ToString() + " " +
                    " where t" + x.ToString() + ".seg_coa_code=tt" + x.ToString() + ".PARENT_CODE and tt" + x.ToString() + ".LVL_CODE='" + lvlcode[x] + "') ";
                    criteria += " and substring(gl_coa_code,charindex('" + sep + "',gl_coa_code," + y + ")+1," + lvlSize[x].ToString() + ") in (SELECT SEG_COA_CODE  FROM t" + x.ToString() + " where rootleaf='L' )  and gl_coa_code in(select '1'+'-'+t.gl_coa_code from CLIENT_INFO t where t.gl_coa_code=gl_coa_code) ";
                }
                y = y + lvlSize[x];
                x = x + 1;
            }
            if (criteria != "")
            {
                query = query + "select GL_COA_CODE, COA_ENABLED, convert(varchar,EFFECTIVE_FROM,103) EFFECTIVE_FROM, convert(varchar,EFFECTIVE_TO,103) EFFECTIVE_TO, BUD_ALLOWED, POST_ALLOWED, TAXABLE, ACC_TYPE, STATUS, BOOK_NAME, ENTRY_USER, convert(varchar,ENTRY_DATE,103) ENTRY_DATE, " +
                    " UPDATE_USER, convert(varchar,UPDATE_DATE,103) UPDATE_DATE, AUTHO_USER, convert(varchar,AUTHO_DATE,103) AUTHO_DATE, COA_DESC, COA_CURR_BAL, COA_NATURAL_CODE,'' INC from gl_coa where " + criteria;
            }
            else
            {
                query = "select GL_COA_CODE, COA_ENABLED, convert(varchar,EFFECTIVE_FROM,103) EFFECTIVE_FROM, convert(varchar,EFFECTIVE_TO,103) EFFECTIVE_TO, BUD_ALLOWED, POST_ALLOWED, TAXABLE, ACC_TYPE, STATUS, BOOK_NAME, ENTRY_USER, convert(varchar,ENTRY_DATE,103) ENTRY_DATE, " +
                    " UPDATE_USER, convert(varchar,UPDATE_DATE,103) UPDATE_DATE, AUTHO_USER, convert(varchar,AUTHO_DATE,103) AUTHO_DATE, COA_DESC, COA_CURR_BAL, COA_NATURAL_CODE,'' INC from gl_coa";
            }
            return query;
        }

        public static string getCoaDesc(string coa)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select coa_desc from gl_coa where gl_coa_code='" + coa + "'";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            return maxValue.ToString();
        }
        public static DataTable getGlCoaDescs(string coa)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select gl_coa_code,seg_coa_desc coa_desc from gl_coa a,gl_seg_coa b where a.coa_natural_code=b.seg_coa_code and upper(coa_desc) like '%"+coa.ToUpper()+"%' order by 1 ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "GlCoa");
            return dt;
        }

        public static decimal GetShowOpeningBalance(string glcoacode)
        {
            
            return 0;  
        }

        public static void UpdateOpeningBalance(string GlCode, string OpeningBalance)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"UPDATE [GL_COA]
   SET [opening_balance] ='" + OpeningBalance + "' WHERE [GL_COA_CODE]='" + GlCode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static int GetDuplicatId(string Coa)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select COUNT(*) from gl_coa f where f.GL_COA_CODE='" + Coa + "'";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            return Convert.ToInt32(maxValue);
        }
    }
    //*U******************** Trail Blance ****************//
}