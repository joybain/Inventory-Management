using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Data.SqlClient; 


/// <summary>
/// Summary description for BudgetManager
/// </summary>
/// 
namespace Delve
{
    public class BudgetManager
    {
        public static void DeleteBudgetMst(string budid)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "delete from sgl_budget where bud_sys_id='" + budid + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void CreateBudgetMst(BudgetMst budmst)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "insert into sgl_budget(bud_sys_id,bud_desc,fin_year,fin_start_dt,fin_end_dt,bud_type_code,bud_open,status,book_name,entry_date,entry_user) values "+
            " ( '" + budmst.BudSysId + "', '" + budmst.BudDesc + "', '" + budmst.FinYear + "', "+
            "  convert(datetime,case '" + budmst.FinStartDt + "' when '' then null else '" + budmst.FinStartDt + "' end,103), convert(datetime, case '" + budmst.FinEndDt + "' when '' then null else '" + budmst.FinEndDt + "' end,103), " +
            "  '" + budmst.BudTypeCode + "', '" + budmst.BudOpen + "', '" + budmst.Status + "', '" + budmst.BookName + "', '" + budmst.EntryUser + "', convert(datetime,case '" + budmst.EntryDate + "' when '' then null else '" + budmst.EntryDate + "' end,103))";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateBudgetMst(BudgetMst budmst)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "update sgl_budget set bud_desc= '" + budmst.BudDesc + "',fin_year= '" + budmst.FinYear + "', " +
            " fin_start_dt= convert(datetime,case '" + budmst.FinStartDt + "' when '' then null else '" + budmst.FinStartDt + "' end,103),fin_end_dt= convert(datetime,case '" + budmst.FinEndDt + "' when '' then null else '" + budmst.FinEndDt + "' end,103), " +
            " bud_type_code= '" + budmst.BudTypeCode + "',bud_open= '" + budmst.BudOpen + "',status= '" + budmst.Status + "',autho_user= '" + budmst.AuthoUser + "',autho_date= convert(datetime,case '" + budmst.AuthoDate + "' when '' then null else '" + budmst.AuthoDate + "' end,103) where bud_sys_id= + '" + budmst.BudSysId + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static BudgetMst GetBudgetMst(string typecode)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "select bud_sys_id,bud_desc,fin_year,convert(varchar,fin_start_dt,103) fin_start_dt,convert(varchar,fin_end_dt,103) fin_end_dt,bud_type_code,bud_open,status,book_name,entry_user,convert(varchar,entry_date,103) entry_date,autho_user,convert(varchar,autho_date,103) autho_date from sgl_budget where bud_sys_id= + '" + typecode + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "budget_mst");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new BudgetMst(dt.Rows[0]);
        }
        public static DataTable GetBudgetMsts()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "select bud_sys_id,bud_desc,fin_year,convert(varchar,fin_start_dt,103) fin_start_dt,convert(varchar,fin_end_dt,103) fin_end_dt,bud_type_code,bud_open,status,book_name,entry_user,convert(varchar,entry_date,103) entry_date,autho_user,convert(varchar,autho_date,103) autho_date from sgl_budget";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "budget_mst");
            return dt;
        }

        public static void DeleteBudgetDtls(string budid)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "delete from sgl_budget_amnt where bud_sys_id='" + budid + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void DeleteBudgetDtl(string budid,string glcode)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "delete from sgl_budget_amnt where bud_sys_id='" + budid + "' and gl_coa_code ='" + glcode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }  

        public static void CreateBudgetDtl(BudgetDtl buddtl)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "insert into sgl_budget_amnt(bud_sys_id,gl_coa_code,bud_inc_pct,bud_tol_pct,bud_tol_amnt,bud_override_amnt,bud_amnt,status,book_name) values " +
            " ( '" + buddtl.BudSysId + "', '" + buddtl.GlCoaCode + "', convert(numeric,case '" + buddtl.BudIncPct + "' when '' then null else '" + buddtl.BudIncPct + "' end), " +
            "  convert(numeric,case '" + buddtl.BudTolPct + "' when '' then null else '" + buddtl.BudTolPct + "' end), convert(numeric,case '" + buddtl.BudTolAmnt + "' when '' then null else '" + buddtl.BudTolAmnt + "' end), " +
            "  convert(numeric,case '" + buddtl.BudOverrideAmnt + "' when '' then null else '" + buddtl.BudOverrideAmnt + "' end), convert(numeric,case '" + buddtl.BudAmnt + "' when '' then null else '" + buddtl.BudAmnt + "' end), '" + buddtl.Status + "', '" + buddtl.BookName + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateBudgetDtl(BudgetDtl buddtl)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "update sgl_budget_amnt set bud_inc_pct=  convert(numeric,case '" + buddtl.BudIncPct + "' when '' then null else '" + buddtl.BudIncPct + "' end), " +
            " bud_tol_pct= convert(numeric,case '" + buddtl.BudTolPct + "' when '' then null else '" + buddtl.BudTolPct + "' end),bud_tol_amnt= convert(numeric,case '" + buddtl.BudTolAmnt + "' when '' then null else '" + buddtl.BudTolAmnt + "' end), " +
            " bud_override_amnt= convert(numeric,case '" + buddtl.BudOverrideAmnt + "' when '' then null else '" + buddtl.BudOverrideAmnt + "' end),bud_amnt= convert(numeric,case '" + buddtl.BudAmnt + "' when '' then null else '" + buddtl.BudAmnt + "' end),status= '" + buddtl.Status + "' where bud_sys_id='" + buddtl.BudSysId + "' and gl_coa_code = '" + buddtl.GlCoaCode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static BudgetDtl GetBudgetDtl(string budSysid,string glcode)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "select bud_sys_id,gl_coa_code,convert(varchar,bud_inc_pct) bud_inc_pct,convert(varchar,bud_tol_pct) bud_tol_pct,convert(varchar,bud_tol_amnt) bud_tol_amnt,convert(varchar,bud_override_amnt) bud_override_amnt, convert(varchar,bud_amnt) bud_amnt,status,book_name from sgl_budget_amnt where bud_sys_id='" + budSysid + "' and gl_coa_code = '" + glcode + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "budget_dtl");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new BudgetDtl(dt.Rows[0]);
        }
        public static DataTable GetBudgetDtls(string budSysid)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "select bud_sys_id,a.gl_coa_code,convert(varchar,bud_inc_pct) bud_inc_pct,convert(varchar,bud_tol_pct) bud_tol_pct,convert(varchar,bud_tol_amnt) bud_tol_amnt,convert(varchar,bud_override_amnt) bud_override_amnt, convert(varchar,bud_amnt) bud_amnt,a.status,b.coa_desc seg_coa_desc from sgl_budget_amnt a,gl_coa b where a.gl_coa_code=b.gl_coa_code and bud_sys_id='" + budSysid + "' order by a.gl_coa_code";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "budget_dtl");
            return dt;
        }        

        public static DataTable GetSegCoa(string psegcode,string lvl, string book)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = " with t1(seg_coa_code,seg_coa_desc,parent_code,rootleaf,book_name) as "+
            " (select seg_coa_code,seg_coa_desc,PARENT_CODE,rootleaf,BOOK_NAME from GL_SEG_COA where SEG_COA_CODE='"+psegcode+"' and LVL_CODE='"+lvl+"' "+
            " union all select tt1.seg_coa_code,tt1.seg_coa_desc,tt1.parent_code,tt1.ROOTLEAF,tt1.BOOK_NAME from GL_SEG_COA as tt1 ,t1 "+
            " where t1.seg_coa_code=tt1.PARENT_CODE and tt1.LVL_CODE='"+lvl+"') "+
            " select SEG_COA_CODE,seg_coa_desc from GL_SEG_COA where SEG_COA_CODE in (select SEG_COA_CODE from t1) and ROOTLEAF='L' "+
            " and BOOK_NAME='"+book+"' order by 1";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "segcoa");
            return dt;
        }  
    }
}