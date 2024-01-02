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
/// Summary description for PromManager
/// </summary>
/// 
namespace Delve
{
    public class PromManager
    {
        public static void CreateProm(Prom prom)
        {
            String connectionString = DataManager.OraConnString();

            string QueryempID = "select top(1) ID from PMIS_PERSONNEL order by ID desc";
           // string empID = DataManager.ExecuteScalar(connectionString, QueryempID);

            string query = " insert into pmis_promotion (emp_no,off_ord_no,joining_date,joining_branch,joining_desig,basic_pay,pay_scale) values (" +
                "  '" + prom.EmpNo + "', '" + prom.OffOrdNo + "', convert(datetime,nullif('" + prom.JoiningDate + "',''),103), '" + prom.JoiningBranch + "','" + prom.JoiningDesig + "', convert(decimal(13,2),nullif('" + prom.BasicPay + "','')), '" + prom.PayScale + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateProm(Prom prom)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update pmis_promotion set off_ord_no= '" + prom.OffOrdNo + "',joining_date= convert(datetime,nullif('" + prom.JoiningDate + "',''),103),joining_branch= '" + prom.JoiningBranch + "', " +
             " joining_desig= '" + prom.JoiningDesig + "',basic_pay= convert(decimal(13,2),nullif('" + prom.BasicPay + "','')),pay_scale= '" + prom.PayScale + "' where emp_no='" + prom.EmpNo + "' and off_ord_no='" + prom.OffOrdNo + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteProm(string emp)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from pmis_promotion where emp_no='" + emp + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteSingleProm(string emp,string offord)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from pmis_promotion where emp_no='" + emp + "' and off_ord_no='"+offord+"' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static Prom getProm(string empno, string prom)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select emp_no,off_ord_no,convert(varchar,JOINING_DATE,103)joining_date,joining_branch,joining_desig,convert(varchar,basic_pay)basic_pay,pay_scale from pmis_promotion where emp_no='" + empno + "' and rtrim(off_ord_no)= rtrim('" + prom + "') ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Promotion");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Prom(dt.Rows[0]);
        }
        public static DataTable getProms(string empno)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select off_ord_no,convert(varchar,JOINING_DATE,103)joining_date,joining_branch,joining_desig,convert(varchar,basic_pay)basic_pay,pay_scale from pmis_promotion where emp_no='" + empno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Promotion");
            return dt;
        }
        public static DataTable getPromsRpt(string criteria)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select emp_no,off_ord_no,convert(varchar,JOINING_DATE,103)joining_date,joining_branch,joining_desig,convert(varchar,basic_pay)basic_pay,pay_scale from pmis_promotion ";
            if (criteria.Length > 0)
            {
                query += " where " + criteria;
            }
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Promotion");
            return dt;
        }
        public static DataTable getPromRpt(string empno)
        {
            String connectionString = DataManager.OraConnString();
            //string query = "select off_ord_no,convert(varchar,JOINING_DATE,103)joining_date, "+
            //" (select dbo.initcap(BranchName) from BranchInfo where BranchKey=a.joining_branch) joining_branch, "+
            //" (select dbo.initcap(desig_name) from pmis_desig_code where desig_code=a.joining_desig) joining_desig,convert(varchar,basic_pay)basic_pay from pmis_promotion a where emp_no='" + empno + "' ";
            string query = @"select off_ord_no,convert(varchar,JOINING_DATE,103)joining_date, dbo.initcap(t2.BranchName) joining_branch, dbo.initcap(t3.desig_name) joining_desig,convert(varchar,basic_pay)basic_pay 
from pmis_promotion t1
Left join BranchInfo t2 on t1.JOINING_BRANCH=t2.BranchKey
Left join pmis_desig_code t3 on t1.JOINING_DESIG=t3.DESIG_CODE where t1.emp_no='" + empno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "pmis_promotion");
            return dt;
        }
    }
}
