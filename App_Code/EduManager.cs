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
/// Summary description for EduManager
/// </summary>
/// 
namespace Delve
{
    public class EduManager
    {
        public static void CreateEdu(Edu edu)
        {
            String connectionString = DataManager.OraConnString();

            string QueryempID = "select top(1) ID from PMIS_PERSONNEL order by ID desc";
            //string empID = DataManager.ExecuteScalar(connectionString, QueryempID);

            string query = " insert into pmis_education(emp_no,exam_code,group_name,institute,pass_year,main_sub,div_class) values (" +
                "  '" + edu.EmpNo + "', '" + edu.ExamCode + "', '" + edu.GroupName + "', '" + edu.Institute + "', " +
             "  '" + edu.PassYear + "', '" + edu.MainSub + "', '" + edu.DivClass + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateEdu(Edu edu)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update pmis_education set exam_code= '" + edu.ExamCode + "',group_name= '" + edu.GroupName + "',institute= '" + edu.Institute + "', " +
             " pass_year= '" + edu.PassYear + "',main_sub= '" + edu.MainSub + "',div_class= '" + edu.DivClass + "' where emp_no='" + edu.EmpNo + "' and exam_code='" + edu.ExamCode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteEdu(string emp)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from pmis_education where emp_no='" + emp + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static Edu getEdu(string empno,string exam)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select * from pmis_education where emp_no='" + empno + "' and exam_code='"+exam+"' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Education");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Edu(dt.Rows[0]);
        }
        public static DataTable getEdus(string empno)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select EMP_NO, t1.EXAM_CODE,t2.EXAM_NAME, PLACE_OF_POST, GROUP_NAME, PASS_YEAR, INSTITUTE, DIV_CLASS,t3.DIVISION_NAME, MAIN_SUB, REMARKS, BOARD from pmis_education t1 inner join PMIS_EXAM_CODE t2 on t1.EXAM_CODE=t2.EXAM_CODE inner join PMIS_DIVISION_CODE t3 on t1.DIV_CLASS=t3.DIVISION_CODE where emp_no='" + empno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Education");
            return dt;
        }

        public static DataTable getEducation(string empno)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select emp_no,exam_name,dbo.initcap(group_name)group_name,dbo.initcap(institute)institute,pass_year,dbo.initcap(main_sub)main_sub,decode(div_class,'1','1st Div.','2','2nd Div.','3','3rd Div.')div_class "+
            " from pmis_education a,pmis_exam_code b where a.exam_code=b.exam_code and emp_no='" + empno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Education");
            return dt;
        }
        public static DataTable getEducationRpt(string criteria)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select emp_no,exam_name,dbo.initcap(group_name)group_name,dbo.initcap(institute)institute,pass_year,dbo.initcap(main_sub)main_sub,decode(div_class,'1','1st Div.','2','2nd Div.','3','3rd Div.')div_class " +
            " from pmis_education a,pmis_exam_code b where a.exam_code=b.exam_code ";
            if (criteria.Length > 0)
            {
                query = query + " where " + criteria;
            }
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Education");
            return dt;
        }
        public static DataTable getEduRpt(string empno)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"select b.exam_name,dbo.initcap(group_name)group_name,dbo.initcap(institute)institute,pass_year,dbo.initcap(main_sub)main_sub,(case when div_class='1' then '1st Div.' when div_class = '2' then '2nd Div.' when div_class = '3'then '3rd Div.' end)div_class from pmis_education a,pmis_exam_code b where a.exam_code=b.exam_code and a.emp_no='" + empno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Education");
            return dt;
        }
    }
}
