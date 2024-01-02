using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using Delve;

/// <summary>
/// Summary description for clsSessionManager
/// </summary>
/// 
namespace Delve
{
    public class clsSessionManager
    {
        public static void CreateSession(clsSession ses)
        {
            String connectionString = DataManager.OraConnString();
            string query = " insert into user_session (session_id,session_time,mac,user_id) values (" +
                "  '" + ses.SessionId + "', convert(datetime,'" + ses.SessionTime + "'), '" + ses.Mac + "', '" + ses.UserId + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteSession(string ses)
        {
            String connectionString = DataManager.OraConnString();
            string query = "delete from user_session where session_id='" + ses + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static string getSessionInfo(string user,string mac)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select 'Y'  from user_session where user_id='" + user + " and mac='"+mac+"' and convert(datetime,session_time,103)=sysdate";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            string a = "";
            if (maxValue != null)
            {
                a = maxValue.ToString();
            }
            return a;
        }
        public static clsSession getSession(string ses)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select session_id,convert(varchar,session_time,120)session_time,mac,user_id from user_session where session_id='" + ses + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Employee");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsSession(dt.Rows[0]);
        }
        public static clsSession getLoginSession(string ses,string mac)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select session_id,convert(varchar,session_time,120) session_time,mac,user_id from user_session where user_id='" + ses + "' and mac='"+mac+"'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Employee");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsSession(dt.Rows[0]);
        }
    }
}