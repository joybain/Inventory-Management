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
/// Summary description for empAssoManager
/// </summary>
/// 
namespace Delve
{
    public class empAssoManager
    {
        public static void CreateEmpAsso(empAsso asso)
        {
            String connectionString = DataManager.OraConnString();
            string query = " insert into pmis_member_info (emp_no,asso_id,member_no) values (" +
                "  '" + asso.EmpNo + "', convert(numeric,nullif('" + asso.AssoId + "','')), '" + asso.MemberNo + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateEmpAsso(empAsso asso)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update pmis_member_info set asso_id= convert(numeric,nullif('" + asso.AssoId + "','')),member_no= '" + asso.MemberNo + "' where emp_='" + asso.EmpNo + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteEmpAsso(string rowid)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from pmis_member_info  where rowid='" + rowid + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteEmpAssos(string empno)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from pmis_member_info  where emp_no='" + empno + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static empAsso getEmpAsso(string rowid)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select emp_no,convert(varchar,asso_id) asso_id,member_no from pmis_member_info  where rowid='"+rowid+"'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "empAssociation");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new empAsso(dt.Rows[0]);
        }
        public static DataTable getEmpAssos(string empno)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select emp_no,convert(varchar,asso_id)asso_id,member_no from pmis_member_info  where emp_no = '" + empno + "' order by convert(decimal(13,2),asso_id)";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "empAssociations");
            return dt;
        }
    }
}