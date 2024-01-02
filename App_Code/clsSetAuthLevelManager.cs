using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for clsSetAuthLevelManager
/// </summary>
/// 
namespace Delve
{
    public class clsSetAuthLevelManager
    {
        public static clsSetAuthLevel GetAuthLevel(string mod,string dept)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select dept,mod_id, auth_level from set_auth_level a where dept='" + dept + "' and mod_id='" + mod + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SetAuthLevel");
            sqlCon.Close();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsSetAuthLevel(dt.Rows[0]);
        }
        public static DataTable GetAuthLevels()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select dept,mod_id,(select description from utl_modules where mod_id=a.mod_id) mod_desc, auth_level a from set_auth_level order by 2";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "PurMstInfo");
            sqlCon.Close();
            return dt;
        }
        public static DataTable GetAuthLevelGrid(string dept)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select dept,mod_id,(select description from utl_modules where mod_id=a.mod_id) mod_desc, auth_level, case when auth_level=4 then 'Administrator' when auth_level=2 then 'Supervisor' when auth_level=3 then 'Evaluator' " +
            " when auth_level=1 then 'Operator' end auth_level_desc   from set_auth_level a where dept like '%" + dept + "%' order by 2";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "PurMstInfo");
            sqlCon.Close();
            return dt;
        }
        public static void CreateAuthLevel(clsSetAuthLevel auth)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = " insert into set_auth_level (mod_id,auth_level,dept) values ('"+auth.ModId+"','"+auth.AuthLevel+"','"+auth.Dept+"')";
            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }
        public static void UpdateAuthLevel(clsSetAuthLevel auth)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = " update set_auth_level set auth_level='" + auth.AuthLevel + "' where dept='" + auth.Dept + "' and mod_id= '" + auth.ModId + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }
        public static void DeleteAuthLevel(clsSetAuthLevel auth)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = " delete from set_auth_level  where dept='" + auth.Dept + "' and mod_id= '" + auth.ModId + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }
    }
}