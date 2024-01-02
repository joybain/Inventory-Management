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
/// Summary description for MembManager
/// </summary>
/// 
namespace Delve
{
    public class MembManager
    {
        //public static void CreateMemb(Memb mem)
        //{
        //    String connectionString = DataManager.OraConnString();
        //    string query = " insert into pmis_member (emp_no,ieb,ideb,bcs,bss,bea,prof_other,dwes,dwdea,dhauks,cba,wasa_other) values (" +
        //        "  '" + mem.EmpNo + "', '" + mem.Ieb + "', '" + mem.Ideb + "' , '" + mem.Bcs + "', '" + mem.Bss + "', " +
        //     "  '" + mem.Bea + "', '" + mem.ProfOther + "', '" + mem.Dwes + "', '" + mem.Dwdea + "', '" + mem.Dhauks + "', " +
        //     "  '" + mem.Cba + "', '" + mem.WasaOther + "')";
        //    DataManager.ExecuteNonQuery(connectionString, query);
        //}
        //public static void UpdateMemb(Memb mem)
        //{
        //    String connectionString = DataManager.OraConnString();
        //    string query = " update pmis_member set ieb= '" + mem.Ieb + "',ideb= '" + mem.Ideb + "' ,bcs= '" + mem.Bcs + "',bss= '" + mem.Bss + "', " +
        //     " bea= '" + mem.Bea + "',prof_other= '" + mem.ProfOther + "',dwes= '" + mem.Dwes + "',dwdea= '" + mem.Dwdea + "',dhauks= '" + mem.Dhauks + "', " +
        //     " cba= '" + mem.Cba + "',wasa_other= '" + mem.WasaOther + "' where emp_no='" + mem.EmpNo + "' ";
        //    DataManager.ExecuteNonQuery(connectionString, query);
        //}
        public static void DeleteMemb(string emp)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from pmis_member  where emp_no='" + emp + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        //public static Memb getMemb(string empno)
        //{
        //    String connectionString = DataManager.OraConnString();
        //    string query = "select * from pmis_member where emp_no='" + empno + "' ";
        //    DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Member");
        //    if (dt.Rows.Count == 0)
        //    {
        //        return null;
        //    }
        //    return new Memb(dt.Rows[0]);
        //}
        public static DataTable getMember(string empno)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select 'Member of :'+ case when coalesce(ieb,'1')='1' then '' else 'IEB (Membership No. '+ieb+'), ' end + " +
            " case when coalesce(ideb,'1')='1' then '' else 'IDEB (Membership No. '+ideb+'), ' end + " +
            " case when coalesce(bcs,'1')='1' then '' else 'BCS (Membership No. '+bcs+'), ' end + " +
            " case when coalesce(ieb,'1')='1' then '' else 'BSS (Membership No. '+bss+'), ' end + " +
            " case when coalesce(ieb,'1')='1' then '' else 'BEA (Membership No. '+bea+'), ' end + " +
            " case when dwes='1' then 'DWES, ' end +case when dwdea='1' then 'DWDEA, ' end +case when CBA='1' then 'CBA, ' end + " +
            " case when dwes='1' then 'DHAUKS, ' end +wasa_other memb " +
            " from pmis_member where emp_no='" + empno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Suspension");
            return dt;
        }
        public static DataTable getMemberRpt(string criteria)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select emp_no,'Member of :'+ case when coalesce(ieb,'1')='1' then '' else 'IEB (Membership No. '+ieb+'), ' end + " +
            " case when coalesce(ideb,'1')='1' then '' else 'IDEB (Membership No. '+ideb+'), ' end + " +
            " case when coalesce(bcs,'1')='1' then '' else 'BCS (Membership No. '+bcs+'), ' end + " +
            " case when coalesce(ieb,'1')='1' then '' else 'BSS (Membership No. '+bss+'), ' end + " +
            " case when coalesce(ieb,'1')='1' then '' else 'BEA (Membership No. '+bea+'), ' end + " +
            " case when dwes='1' then 'DWES, ' end +case when dwdea='1' then 'DWDEA, ' end +case when CBA='1' then 'CBA, ' end + " +
            " case when dwes='1' then 'DHAUKS, ' end +wasa_other memb " +
            " from pmis_member  ";
            if (criteria.Length > 0)
            {
                query += " where " + criteria;
            }
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Membership");
            return dt;
        }
        public static DataTable getMembRpt(string emp)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select (select dbo.initcap(asso_name)+' '+'('+asso_abvr+')' from association_info where asso_id=a.asso_id) asso_id,member_no  from pmis_member_info a " +
                " where emp_no='" + emp + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Membership");
            return dt;
        }
    }
}