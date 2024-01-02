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
/// Summary description for DivDisThanaManager
/// </summary>
/// 
namespace Delve
{
    public class DivDisThanaManager
    {
        public static DataTable getDivision()
        {
            String connectionString = DataManager.OraConnString();
            string query = "select division_code,dbo.initcap(division_name)division_name from pmis_division_code";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Division");
            return dt;
        }
        public static DataTable getDistrict(string criteria)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select dbo.initcap(b.division_name)division_name,a.district_code,dbo.initcap(a.district_name)district_name from pmis_district_code a,pmis_division_code b "+
                " where a.division_code=b.division_code";
            if (criteria != "")
            {
                query = query + " and " + criteria;
            }
            query = query + " order by convert(decimal(13,2),a.division_code),convert(decimal(13,2),a.district_code)";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Division");
            return dt;
        }
        public static DataTable getThana(string criteria)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select b.division_code,dbo.initcap(c.division_name)division_name,a.district_code,dbo.initcap(b.district_name)district_name,a.thana_code,dbo.initcap(a.thana_name)thana_name "+
            " from pmis_thana_code a,pmis_district_code b,pmis_division_code c "+
            " where a.district_code=b.district_code and b.division_code=c.division_code";
            if (criteria != String.Empty)
            {
                query = query + " and " + criteria;
            }
            query = query + " order by convert(decimal(13,2),b.division_code),convert(decimal(13,2),a.district_code),convert(decimal(13,2),a.thana_code)";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Division");
            return dt;
        }
        public static DataTable getBranch()
        {
            String connectionString = DataManager.OraConnString();
            string query = "select ID,BranchKey,dbo.initcap(BranchName)BranchName from BranchInfo order by ID";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Branch");
            return dt;
        }
        public static DataTable getDesignation()
        {
            String connectionString = DataManager.OraConnString();
            string query = "select DESIG_CODE,dbo.initcap(desig_name)desig_name from pmis_desig_code";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Designation");
            return dt;
        }
    }
}