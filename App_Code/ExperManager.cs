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
/// Summary description for ExperManager
/// </summary>
/// 
namespace Delve
{
    public class ExperManager
    {
        public static void CreateExper(Exper exp)
        {
            String connectionString = DataManager.OraConnString();
            string query = " insert into pmis_prev_exp(emp_code,orga_name,position_held,from_dt,to_dt,pay_scale) values (" +
                "  '" + exp.EmpNo + "', '" + exp.OrgaName + "', '" + exp.PositionHeld + "', convert(datetime,nullif('" + exp.FromDt + "',''),103), " +
             "  convert(datetime,nullif('" + exp.ToDt + "',''),103), '" + exp.PayScale + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateExper(Exper exp)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update pmis_prev_exp set orga_name= '" + exp.OrgaName + "',position_held= '" + exp.PositionHeld + "',from_dt= convert(datetime,nullif('" + exp.FromDt + "',''),103), " +
             " to_dt= convert(datetime,nullif('" + exp.ToDt + "',''),103), pay_scale = '" + exp.PayScale + "' where emp_code='" + exp.EmpNo + "' and orga_name='" + exp.OrgaName + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteExper(string emp)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from pmis_prev_exp where emp_code='" + emp + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static Edu getExper(string empno, string org)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select EMP_CODE, PLACE_OF_POST, ORGA_NAME, POSITION_HELD, convert(varchar,FROM_DT,103) from_dt, convert(varchar,TO_DT,103) to_dt, "+
                " EXPE_YEAR, AREA_EXPE, PAY_SCALE, PROPER_CHN, GRADE from pmis_prev_exp where emp_code='" + empno + "' and rtrim(orga_name)= rtrim('" + org + "') ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Experience");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Edu(dt.Rows[0]);
        }
        public static DataTable getExpers(string empno)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select ORGA_NAME, POSITION_HELD, convert(varchar,FROM_DT,103)from_dt, convert(varchar,TO_DT,103)to_dt, " +
                " PAY_SCALE from pmis_prev_exp where emp_code='" + empno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Experience");
            return dt;
        }
        public static DataTable getExperience(string empno)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select emp_code emp_no,dbo.initcap(ORGA_NAME)orga_name, dbo.initcap(POSITION_HELD)position_held, convert(varchar,FROM_DT,103)from_dt, convert(varchar,TO_DT,103)to_dt, " +
                " PAY_SCALE from pmis_prev_exp where emp_code='" + empno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Experience");
            return dt;
        }
        public static DataTable getExperienceRpt(string criteria)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select emp_code emp_no,dbo.initcap(ORGA_NAME)orga_name, dbo.initcap(POSITION_HELD)position_held, convert(varchar,FROM_DT,103)from_dt, convert(varchar,TO_DT,103)to_dt, " +
                " PAY_SCALE from pmis_prev_exp  ";
            if (criteria.Length > 0)
            {
                query += " where " + criteria;
            }
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Experience");
            return dt;
        }
        public static DataTable getExpRpt(string empno)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select dbo.initcap(ORGA_NAME)orga_name, dbo.initcap(POSITION_HELD)position_held, convert(varchar,FROM_DT,103)from_dt, convert(varchar,TO_DT,103)to_dt, " +
                " PAY_SCALE from pmis_prev_exp where emp_code='" + empno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Experience");
            return dt;
        }

        public static DataTable GetBranchShowExpensesReport(string StartDate, string EndDate,string BranchId)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"SELECT [ID],[MstID],[ExpensesHeadID],[ExpName],[Amount],[ExpDate],[Code],[Remarks]
            FROM [dbo].[View_Expenses_Report] where  BranchId='"+BranchId+"' and  convert(date,ExpDate,103) between convert(date,'" + StartDate +
                           "',103) and convert(date,'" + EndDate + "',103) order by  convert(date,ExpDate,103) ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Experience");
            return dt;
        }

        public static DataTable GetShowExpensesReport(string StartDate, string EndDate)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"SELECT [ID],[MstID],[ExpensesHeadID],[ExpName],[Amount],[ExpDate],[Code],[Remarks]
            FROM [dbo].[View_Expenses_Report] where  (date,ExpDate,103) between convert(date,'" + StartDate +
                           "',103) and convert(date,'" + EndDate + "',103) order by  convert(date,ExpDate,103) ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Experience");
            return dt;
        }
    }
}