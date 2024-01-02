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
/// Summary description for TransfManager
/// </summary>
/// 
namespace Delve
{
    public class TransfManager
    {
        public static void CreateTransf(Transf trn)
        {
            String connectionString = DataManager.OraConnString();

            string QueryempID = "select top(1) ID from PMIS_PERSONNEL order by ID desc";
           // string empID = DataManager.ExecuteScalar(connectionString, QueryempID);

            string query = " insert into pmis_transfer (emp_no,order_no,trans_date,trans_prom,BRANCH_CODE,desig_code) values (" +
                "  '" + trn.EmpNo + "', '" + trn.OrderNo + "', convert(datetime,nullif('" + trn.TransDate + "',''),103), '" + trn.TransProm + "', " +
             "  '" + trn.BranchCode + "', '" + trn.DesigCode + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateTransf(Transf trn)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update pmis_transfer set order_no = '" + trn.OrderNo + "',trans_date= convert(datetime,nullif('" + trn.TransDate + "',''),103),trans_prom= '" + trn.TransProm + "', " +
             " BranchKey= '" + trn.BranchCode + "', desig_code = '" + trn.DesigCode + "' where emp_no='" + trn.EmpNo + "' and order_no='" + trn.OrderNo + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteTransf(string emp)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from pmis_transfer  where emp_no='" + emp + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteTransfer(string emp, string ord)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from pmis_transfer  where emp_no='" + emp + "' and order_no='"+ord+"' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static Transf getTransf(string empno, string trn)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select EMP_NO, convert(varchar,TRANS_DATE,103)trans_date, TRANS_PROM, BranchKey, DESIG_CODE, ORDER_NO from pmis_transfer where emp_no='" + empno + "' and rtrim(order_no)= rtrim('" + trn + "') ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Transfer");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Transf(dt.Rows[0]);
        }
        public static DataTable getTransfs(string empno)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select EMP_NO, convert(varchar,TRANS_DATE,103)trans_date, TRANS_PROM,BRANCH_CODE, DESIG_CODE, ORDER_NO from pmis_transfer where emp_no='" + empno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Transfer");
            return dt;
        }
        public static DataTable getTransfer(string empno)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select EMP_NO, convert(varchar,TRANS_DATE,103)trans_date, case when TRANS_PROM='Y' then 'Yes' when TRANS_PROM='N' then 'No' end trans_prom, " +
                " (select dbo.initcap(BranchName) from BranchInfo where BranchKey=a.BranchKey)BranchKey, "+
                " (select dbo.initcap(desig_name) from pmis_desig_code where desig_code=a.desig_code)desig_code, ORDER_NO from pmis_transfer a where emp_no='" + empno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Transfer");
            return dt;
        }
        public static DataTable getTransferRpt(string criteria)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select EMP_NO, convert(varchar,TRANS_DATE,103)trans_date, case when TRANS_PROM='Y' then 'Yes' when TRANS_PROM='N' then 'No' end trans_prom, " +
                " (select dbo.initcap(BranchName) from BranchInfo where BranchKey=a.BranchKey)BranchKey, "+
                " (select dbo.initcap(desig_name) from pmis_desig_code where desig_code=a.desig_code)desig_code, ORDER_NO from pmis_transfer a ";
            if (criteria.Length > 0)
            {
                query += " where " + criteria;
            }
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Transfer");
            return dt;
        }
        public static DataTable getTransRpt(string empno)
        {
            String connectionString = DataManager.OraConnString();
            //string query = "select ORDER_NO,convert(varchar,TRANS_DATE,103)trans_date, case when TRANS_PROM='Y' then 'Yes' when TRANS_PROM='N' then 'No' end trans_prom, " +
            //    " (select dbo.initcap(BranchName) from BranchInfo where BranchKey=a.BranchKey)BranchKey, " +
            //    " (select dbo.initcap(desig_name) from pmis_desig_code where desig_code=a.desig_code)desig_code from pmis_transfer a where emp_no='" + empno + "' ";
            string query = @"select ORDER_NO,convert(varchar,TRANS_DATE,103)trans_date, case when TRANS_PROM='Y' then 'Yes' when TRANS_PROM='N' then 'No' end trans_prom,dbo.initcap(t2.BranchName) BranchKey, dbo.initcap(t3.desig_name)desig_code from pmis_transfer t1
left join BranchInfo t2 on t1.BRANCH_CODE=t2.BranchKey
left join pmis_desig_code t3 on t1.DESIG_CODE=t3.DESIG_CODE where t1.emp_no='" + empno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Transfer");
            return dt;
        }
    }
}
