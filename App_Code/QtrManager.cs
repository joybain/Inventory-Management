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
/// Summary description for QtrManager
/// </summary>
/// 
namespace Delve
{
    public class QtrManager
    {
        public static void CreateQtr(Qtr qtr)
        {
            String connectionString = DataManager.OraConnString();

            string QueryempID = "select top(1) ID from PMIS_PERSONNEL order by ID desc";
           // string empID = DataManager.ExecuteScalar(connectionString, QueryempID);

            string query = " insert into pmis_quarter (emp_no,allot_ref,ref_date,post_date,locat,road,build,flat,flat_typ,sizee) values (" +
                "  '" + qtr.EmpNo + "', '" + qtr.AllotRef + "', convert(datetime,nullif('" + qtr.RefDate + "',''),103), " +
                "  convert(datetime,nullif('" + qtr.PostDate + "',''),103), '" + qtr.Locat + "',  '" + qtr.Road + "'," +
             "  '" + qtr.Build + "', '" + qtr.Flat + "', '" + qtr.FlatTyp + "', '" + qtr.Sizee + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateQtr(Qtr qtr)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update pmis_quarter set allot_ref= '" + qtr.AllotRef + "',ref_date= convert(datetime,nullif('" + qtr.RefDate + "',''),103), " +
                " post_date= convert(datetime,nullif('" + qtr.PostDate + "',''),103),locat= '" + qtr.Locat + "', road= '" + qtr.Road + "'," +
             " build= '" + qtr.Build + "',flat= '" + qtr.Flat + "',flat_typ= '" + qtr.FlatTyp + "',sizee= '" + qtr.Sizee + "' where emp_no='" + qtr.EmpNo + "' and allot_ref='" + qtr.AllotRef + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteQtr(string emp)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from pmis_quarter where emp_no='" + emp + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static Qtr getQtr(string empno, string qtr)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select EMP_NO, ALLOT_REF, convert(varchar,REF_DATE,103)ref_date, convert(varchar,POST_DATE,103)post_date, LOCAT, ROAD, BUILD, FLAT, FLAT_TYP, SIZEE from pmis_quarter where emp_no='" + empno + "' and rtrim(allot_ref)= rtrim('" + qtr + "') ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Quarter");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Qtr(dt.Rows[0]);
        }
        public static DataTable getQtrs(string empno)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select emp_no,ALLOT_REF, convert(varchar,REF_DATE,103)ref_date, convert(varchar,POST_DATE,103)post_date, LOCAT, ROAD, BUILD, FLAT, FLAT_TYP, SIZEE from pmis_quarter where emp_no='" + empno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Quarter");
            return dt;
        }
        public static DataTable getQtrsRpt(string criteria)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select emp_no,ALLOT_REF, convert(varchar,REF_DATE,103)ref_date, convert(varchar,POST_DATE,103)post_date, LOCAT, ROAD, BUILD, FLAT, FLAT_TYP, SIZEE from pmis_quarter ";
            if (criteria.Length > 0)
            {
                query += " where " + criteria;
            }
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Quarter");
            return dt;
        }
        public static DataTable getQtrRpt(string emp)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select ALLOT_REF, convert(varchar,REF_DATE,103)ref_date, convert(varchar,POST_DATE,103)post_date, LOCAT, ROAD, BUILD, FLAT, FLAT_TYP, SIZEE from pmis_quarter where emp_no='" + emp + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Quarter");
            return dt;
        }
    }
}