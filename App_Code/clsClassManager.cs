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

/// <summary>
/// Summary description for clsClassManager
/// </summary>
/// 
namespace Delve
{
    public class clsClassManager
    {
        public static DataTable getClasss()
        {
            String connectionString = DataManager.OraConnString();
            string query = " select convert(varchar,class_id) class_id,class_name from pmis_class order by class_id";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Classes");
            return dt;
        }
        public static DataTable getClasses(string criteria)
        {            
            String connectionString = DataManager.OraConnString();
            string query = " select convert(varchar,class_id) class_id,class_name from pmis_class ";
            if (criteria.Length > 0)
            {
                query += " where class_name in (" + criteria + ")";
            }
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Classes");
            return dt;
        }
        public static clsClass getClass(string clsid)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select convert(varchar,class_id) class_id,class_name from pmis_class where class_id=convert(numeric,nullif('"+clsid+"','')) ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Classes");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsClass(dt.Rows[0]);
        }
        public static void CreateClass(clsClass cls)
        {
            String connectionString = DataManager.OraConnString();
            string query = " insert into pmis_class(class_id,class_name) values (" +
                "  convert(numeric,nullif('" + cls.ClassId + "','')), '" + cls.ClassName + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateClass(clsClass cls)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update pmis_class set class_name= '" + cls.ClassName + "' where class_id='"+cls.ClassId+"' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteClass(string clsid)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from pmis_class where class_id='" + clsid + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static string getColumn(string colname)
        {
            string col = "";
            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select 'Y' colexist from user_tab_columns where table_name='FUND_INFO' and column_name=upper('" + colname + "') ";
            conn.Open();
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())

                    col = dReader["colexist"].ToString();
            }
            return col;
        }
    }
}