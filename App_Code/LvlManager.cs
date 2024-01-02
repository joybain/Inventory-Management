using System;
using System.Data;
using System.Configuration;
using System.Linq;

using System.Xml.Linq;
using System.Data.SqlClient;
using Delve;

/// <summary>
/// Summary description for LvlManager
/// </summary>
/// 
namespace Delve
{
    public class LvlManager
    {
        public static DataTable GetLevels()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "SELECT book_name,lvl_code,lvl_desc,lvl_max_size,lvl_enabled,lvl_seg_type,lvl_order,status,entry_user,convert(varchar,entry_date,103) entry_date,autho_user,convert(varchar,autho_date,103) autho_date FROM GL_level_type order by lvl_order";
            
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Levels");
            return dt;
        }
        public static DataTable GetEnabledLevels()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "SELECT book_name,lvl_code,lvl_desc,lvl_max_size,lvl_enabled,lvl_seg_type,lvl_order,status,entry_user,convert(varchar,entry_date,103) entry_date,autho_user,convert(varchar,autho_date,103) autho_date  FROM GL_level_type  where lvl_enabled='Y' order by lvl_order";

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Levels");
            return dt;
        }
        public static DataTable GetActiveLevels()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "SELECT book_name,lvl_code,lvl_desc,lvl_max_size,lvl_enabled,lvl_seg_type,lvl_order,status,entry_user,convert(varchar,entry_date,103) entry_date,autho_user,convert(varchar,autho_date,103) autho_date  FROM GL_level_type where lvl_enabled='Y' order by lvl_order";

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Levels");
            return dt;
        }
        public static DataTable GetLevelsGrid()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select lvl.lvl_desc,(select seg_coa_code from gl_seg_coa where lvl_code=lvl.lvl_code and parent_code is null ) as seg_code, 'All '+ "+
            " (select seg_coa_desc from gl_seg_coa where lvl_code=lvl.lvl_code and parent_code is null) as seg_desc,status,entry_user,convert(varchar,entry_date,103) entry_date,autho_user,convert(varchar,autho_date,103) autho_date  " +
            " from gl_level_type as lvl order by lvl_order";

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Levels");
            return dt;
        }

        public static DataTable GetLevelCode_Dropdown()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "SELECT [LVL_CODE], [LVL_DESC] FROM [GL_level_type]";

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Levels");
            return dt;
        }

        public static DataTable GetLevelName()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "SELECT lvl_desc FROM GL_level_type order by lvl_order";

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Levels");
            return dt;
        }

        public static void UpdateLevel(GlLevel glevel)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "update gl_level_type set lvl_desc='"+glevel.LvlDesc+"',lvl_max_size='"+glevel.LvlMaxSize+"', "+
                " lvl_enabled='" + glevel.LvlEnabled + "', lvl_seg_type='" + glevel.LvlSegType + "', lvl_order=convert(numeric,case '" + glevel.LvlOrder + "' when '' then null else '" + glevel.LvlOrder + "' end) "+
                " where lvl_code='" + glevel.LvlCode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateLevelStatus(GlLevel glevel)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "update gl_level_type set status='" + glevel.Status + "', autho_user='"+glevel.AuthoUser+"',autho_date=convert(datetime,'"+glevel.AuthoDate+"',103) where lvl_code='" + glevel.LvlCode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void DeleteLevel(string glevel)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "delete from gl_level_type where lvl_code='" + glevel + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void InsertLevel(GlLevel glevel)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "insert into gl_level_type (lvl_code, lvl_desc,lvl_max_size,lvl_enabled,lvl_order,lvl_seg_type,book_name,status,entry_user,entry_date) values ('"+glevel.LvlCode+"', '" + glevel.LvlDesc + "', '" + glevel.LvlMaxSize + "', " +
                " '" + glevel.LvlEnabled + "', convert(numeric,case '" + glevel.LvlOrder + "' when '' then null else '" + glevel.LvlOrder + "' end), '" + glevel.LvlSegType + "', '" + glevel.BookName + "','"+glevel.Status+"','"+glevel.EntryUser+"',convert(datetime,'"+glevel.EntryDate+"',103) )";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static GlLevel getLevel(string lvlcode)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = "select book_name,lvl_code,lvl_desc,lvl_max_size,lvl_enabled,lvl_seg_type,lvl_order,status,entry_user,convert(varchar,entry_date,103) entry_date,autho_user,convert(varchar,autho_date,103) autho_date  from gl_level_type where lvl_code='" + lvlcode + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Gl_Level");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new GlLevel(dt.Rows[0]);
        }
    }
}