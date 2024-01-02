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
/// Summary description for OtManager
/// </summary>
/// 
namespace Delve
{
    public class OtManager
    {
        public static void CreateOtMst(OtMst otm)
        {
            String connectionString = DataManager.OraConnString();
            string query = " insert into overtime_mst(over_id,o_date,BranchKey,over_mon) values (" +
                "  convert(numeric,nullif('" + otm.OverId + "','')), convert(datetime,nullif('" + otm.ODate + "',''),103), '" + otm.BranchId + "', '" + otm.OverMon + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateOtMst(OtMst otm)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update overtime_mst set o_date= convert(datetime,nullif('" + otm.ODate + "',''),103),BranchKey= '" + otm.BranchId + "',over_mon= '" + otm.OverMon + "' where over_id = convert(numeric,nullif('"+otm.OverId+"',''))";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteOtMst(OtMst otm)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from overtime_mst where over_id = convert(numeric,nullif('" + otm.OverId + "',''))";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static DataTable getOtMsts(string otid,string mon,string branch)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select over_id,convert(varchar,o_date,103)o_date,BranchKey, " +
                " (select BranchName from BranchInfo where BranchKey=a.BranchKey)BranchName,over_mon " +
                " from overtime_mst a where convert(varchar,over_id) like '%" + otid + "%' and over_mon like '%" + mon + "%' and BranchKey like '%" + branch + "%'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query,"OtMaster");
            return dt;
        }
        public static OtMst getOtMst(string otid)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select over_id,convert(varchar,o_date,103)o_date,BranchKey,over_mon from overtime_mst where over_id = convert(numeric,nullif('" + otid + "','')) ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query,"OtMaster");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new OtMst(dt.Rows[0]);
        }
        public static void CreateOtDtl(OtDtl otd)
        {
            String connectionString = DataManager.OraConnString();
            string query = " insert into overtime_dtl(over_id,employee_id,ot_single,ot_double,ot_amount) values (" +
                "  convert(numeric,nullif('" + otd.OverId + "','')), '" + otd.EmployeeId + "', convert(decimal(13,2),nullif('" + otd.OtSingle + "','')), convert(decimal(13,2),'" + otd.OtDouble + "'), convert(decimal(13,2),nullif('" + otd.OtAmount + "','')))";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateOtDtl(OtDtl otd)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update overtime_dtl set ot_single= convert(decimal(13,2),nullif('" + otd.OtSingle + "','')),ot_double= convert(decimal(13,2),nullif('" + otd.OtDouble + "','')),ot_amount= convert(decimal(13,2),nullif('" + otd.OtAmount + "','')) where over_id = convert(numeric,nullif('" + otd.OverId + "','')) and employee_id='"+otd.EmployeeId+"'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteOtDtls(string otid)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from overtime_dtl where over_id = convert(numeric,nullif('" + otid + "',''))";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteOtDtl(OtDtl otd)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from overtime_dtl where over_id = convert(numeric,nullif('" + otd.OverId + "','')) and employee_id='"+otd.EmployeeId+"'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static DataTable getOtDtls(string otid)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select over_id,employee_id,(select name from pay_employee where employee_id=a.employee_id)employee_name,"+
                " (select dbo.initcap(desig_name) from pmis_desig_code where desig_code=(select designation_id from pay_employee where employee_id=a.employee_id))designation_id,convert(varchar,ot_single)ot_single, "+
                " convert(varchar,ot_double)ot_double,convert(varchar,ot_amount)ot_amount from overtime_dtl a where over_id = convert(numeric,nullif('" + otid + "','')) ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OtDetail");
            return dt;
        }
        public static DataTable getOvertimeRpt(string mon,string branch)
        {
            String connectionString = DataManager.OraConnString();
            string query = " SELECT b.bank_acc_no,e.employee_id,b.ename,dbo.initcap(c.desig_name)desig_name, " +
            " convert(varchar,e.Ot_single)ot_single,convert(varchar,e.OT_double)ot_double,4 AS Rev_stamp,convert(decimal(13,2),e.ot_amount)-4 net_pay  " +
            " FROM OVERTIME_MST a,OVERTIME_DTL e,v_employeename b,pmis_desig_code c,BranchInfo d " +
            " WHERE e.employee_id=b.employee_id AND a.over_id=e.over_id AND b.designation_id=c.desig_code AND a.over_mon='" + mon + "' " +
            " AND b.BranchKey=d.BranchKey AND a.BranchKey=d.BranchKey AND b.BranchKey like '%" + branch + "%' order by b.BranchKey,desig_name ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OtDetail");
            return dt;
        }
        public static OtDtl getOtDtl(string otid,string emp)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select over_id,employee_id,convert(varchar,ot_single)ot_single,convert(varchar,ot_double)ot_double,convert(varchar,ot_amount)ot_amount from overtime_dtl where over_id = convert(numeric,nullif('" + otid + "','')) and employee_id='" + emp + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OtMaster");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new OtDtl(dt.Rows[0]);
        }
        public static string getOtAmt(string otmon, string emp)
        {
            string rcol = "";
            string connectionString = DataManager.OraConnString();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "select sum(ot_amount)ot_amount from overtime_dtl where over_id=(select over_id from overtime_mst where over_mon='" + otmon + "') and employee_id= '" + emp + "' ";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader dReader = cmd.ExecuteReader())
                    {
                        if (dReader.HasRows == true)
                        {
                            while (dReader.Read())
                            {
                                rcol = dReader["ot_amount"].ToString();
                            }
                        }
                    }
                }
            }
            if (rcol.Trim() == null | rcol.Trim() == "")
            {
                rcol = "0";
            }
            return rcol;
        }
        public static string getOtAmtTotal( string otmon, string branch)
        {
            string rcol = "";
            string connectionString = DataManager.OraConnString();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "select sum(ot_amount)ot_amount from overtime_dtl where over_id=(select over_id from overtime_mst where over_mon='" + otmon + "' and BranchKey= '" + branch + "') ";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader dReader = cmd.ExecuteReader())
                    {

                        if (dReader.HasRows == true)
                        {
                            while (dReader.Read())
                            {
                                rcol = dReader["ot_amount"].ToString();
                            }
                        }
                    }
                }
            }
            if (rcol.Trim() == null | rcol.Trim() == "")
            {
                rcol = "0";
            }
            return rcol;
        } 
    }
}