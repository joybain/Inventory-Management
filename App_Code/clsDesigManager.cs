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
/// Summary description for clsDesigManager
/// </summary>
/// 
namespace AMBs
{
    public class clsDesigManager
    {
        public static void CreateDesig(clsDesig des)
        {
            String connectionString = DataManager.OraConnString();
            string query = " insert into pmis_desig_code (desig_code,desig_name,desig_abb,mgr_code,grade_code,tech_ntech,class,officer_staff,Serial) values (" +
                "  '" + des.DesigCode + "', '" + des.DesigName + "', '" + des.DesigAbb + "', '" + des.MgrCode + "', "+
                "  '" + des.GradeCode + "', '" + des.TechNtech + "', '" + des.Class + "', '" + des.OfficerStaff + "' , '" + des.Serial + "' )";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateDesig(clsDesig des)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update pmis_desig_code set desig_name= '" + des.DesigName + "',desig_abb= '" + des.DesigAbb + "',mgr_code= '" + des.MgrCode + "', " +
                " grade_code= '" + des.GradeCode + "',tech_ntech= '" + des.TechNtech + "',class= '" + des.Class + "',officer_staff= '" + des.OfficerStaff + "',Serial='" + des.Serial + "' " +
                " where desig_code= '" + des.DesigCode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteDesig(string desigcode)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from pmis_desig_code where desig_code= '" + desigcode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static DataTable getDesigs(string designame)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select desig_code,dbo.initcap(desig_name)desig_name,dbo.initcap(desig_abb)desig_abb,mgr_code,grade_code,class,tech_ntech,officer_staff from pmis_desig_code where lower(desig_name) like '%"+designame+"%' order by convert(numeric,desig_code)";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Designations");
            return dt;
        }
        public static DataTable getDesigDetails(string designame)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select desig_code,Serial,dbo.initcap(desig_name)desig_name,dbo.initcap(desig_abb)desig_abb,(select dbo.initcap(desig_name) from pmis_desig_code where desig_code=a.mgr_code) mgr_code from pmis_desig_code a where lower(desig_name) like '%" + designame + "%' order by Serial asc ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Designations");
            return dt;
        }
        public static clsDesig getDesig(string desigcode)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select desig_code,Serial,dbo.initcap(desig_name)desig_name,desig_abb,mgr_code,grade_code,class,tech_ntech,officer_staff from pmis_desig_code where desig_code='" + desigcode + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Designation");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsDesig (dt.Rows[0]);
        }
    }
}