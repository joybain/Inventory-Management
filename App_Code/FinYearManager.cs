using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


/// <summary>
/// Summary description for FinYearManager
/// </summary>
/// 
namespace Delve
{
    public class FinYearManager
    {
        public static void DeleteFinYear(string FnYr)
        {
            string connectionString = DataManager.OraConnString();
            string query = "delete from gl_fin_year where fin_year='"+ FnYr + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        
        public static void CreateFinYear(FinYear FnYr)
        {
            string connectionString = DataManager.OraConnString();
            string query = " insert into gl_fin_year(book_name,fin_year,start_date,end_date,description, "+
                " weekly_fin,year_flag,status,entry_date,entry_user) values ( '"+FnYr.BookName+"', '"+FnYr.FinYr+"',"+
                " convert(datetime,case '" + FnYr.StartDate + "' when '' then null else '" + FnYr.StartDate + "' end ,103),  convert(datetime,case '" + FnYr.EndDate + "' when '' then null else '" + FnYr.EndDate + "' end,103),  '" + FnYr.Description + "', " +
                " '" + FnYr.WeeklyFin + "',  '" + FnYr.YearFlag + "', '"+FnYr.Status+"', convert(datetime,case '" + FnYr.EntryDate + "' when '' then null else '" + FnYr.EntryDate + "' end,103),'" + FnYr.EntryUser + "' )";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateFinYear(FinYear FnYr)
        {
            string connectionString = DataManager.OraConnString();
            string query = " update gl_fin_year set fin_year='"+FnYr.FinYr+"', book_name='"+FnYr.BookName+"', "+
                " start_date=convert(datetime,case '" + FnYr.StartDate + "' when '' then null else '" + FnYr.StartDate + "' end ,103), end_date=convert(datetime,case '" + FnYr.EndDate + "' when '' then null else '" + FnYr.EndDate + "' end,103), " +
                " description='" + FnYr.Description + "', " +
                " weekly_fin='N', year_flag='" + FnYr.YearFlag + "',status='"+FnYr.Status+"', autho_user='" + FnYr.AuthoUser + "', "+
                " autho_date=convert(datetime,case '" + FnYr.AuthoDate + "' when '' then null else '" + FnYr.AuthoDate + "' end,103) where fin_year= '" + FnYr.FinYr + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static FinYear GetFinYear(string finyear)
        {
            string connectionString = DataManager.OraConnString();
            string query = "select fin_year, convert(varchar,start_date,103) start_date, convert(varchar,end_date,103) end_date, description, status, adj_fin, weekly_fin, entry_user, entry_date, update_user, convert(varchar,update_date,103) update_date, autho_user, convert(varchar, autho_date,103) autho_date, year_flag, status,op_user, convert(varchar,op_date,103) op_date, close_user, convert(varchar,close_date,103) close_date, reopen_user, convert(varchar,reopen_date,103) reopen_date, per_close_user, convert(varchar,per_close_date,103) per_close_date, book_name from gl_fin_year where fin_year='" + finyear + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "gl_fin_year");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new FinYear(dt.Rows[0]);
        }
        public static DataTable GetFinYears()
        {
            string connectionString = DataManager.OraConnString();
            string query = "select fin_year, convert(varchar,start_date,103) start_date, convert(varchar,end_date,103) end_date, description, status, adj_fin, weekly_fin, entry_user, entry_date, update_user, convert(varchar,update_date,103) update_date, autho_user, convert(varchar, autho_date,103) autho_date, year_flag, status,op_user, convert(varchar,op_date,103) op_date, close_user, convert(varchar,close_date,103) close_date, reopen_user, convert(varchar,reopen_date,103) reopen_date, per_close_user, convert(varchar,per_close_date,103) per_close_date, book_name from gl_fin_year order by fin_year asc ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "gl_fin_year");
            return dt;
        }
        public static void CreateFinMonth(FinMonth finMon)
        {
            string connectionString = DataManager.OraConnString();
            string query = "insert into gl_fin_month(book_name,fin_mon,fin_year,month_sl,quarter,mon_start_dt, " +
                " mon_end_dt,year_flag) values('" + finMon.BookName + "', '" + finMon.FinMon + "', '" + finMon.FinYear + "'," +
                "  '" + finMon.MonthSl + "', '" + finMon.Quarter + "',  convert(datetime,case '" + finMon.MonStartDt + "' when '' then null else '" + finMon.MonStartDt + "' end,103),  convert(datetime,case '" + finMon.MonEndDt + "' when '' then null else '" + finMon.MonEndDt + "' end,103)," +
                "  '" + finMon.YearFlag + "' )";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateFinMonth(FinMonth finMon)
        {
            string connectionString = DataManager.OraConnString();
            string query = "update gl_fin_month set fin_mon='" + finMon.FinMon + "',fin_year='" + finMon.FinYear + "', " +
                " month_sl=" + finMon.MonthSl + ",quarter='" + finMon.Quarter + "',mon_start_dt=convert(datetime,case '" + finMon.MonStartDt + "' when '' then null else '" + finMon.MonStartDt + "' end,103), " +
                " mon_end_dt=convert(datetime,case '" + finMon.MonEndDt + "' when '' then null else '" + finMon.MonEndDt + "' end,103),year_flag='" + finMon.YearFlag + "' where fin_mon='" + finMon.FinMon + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void UpdateFinMonthYearFlag(string finMon,string yrflag)
        {
            string connectionString = DataManager.OraConnString();
            string query = "update gl_fin_month set year_flag='" + yrflag + "' where fin_mon='" + finMon + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteFinMonth(String finMon)
        {
            string connectionString = DataManager.OraConnString();
            string query = "delete from gl_fin_month where  fin_year='" + finMon + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteFinMonths(String finYear)
        {
            string connectionString = DataManager.OraConnString();
            string query = "delete from gl_fin_month where  fin_year='" + finYear + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static FinMonth GetFinMonth(string finMon)
        {
            string connectionString = DataManager.OraConnString();
            string query = "select fin_year, fin_mon, month_sl, quarter, convert(varchar,mon_start_dt,103) mon_start_dt, convert(varchar,mon_end_dt,103) mon_end_dt, year_flag, entry_user, convert(varchar,entry_date,103) entry_date, update_user, convert(varchar,update_date,103) update_date, autho_user, convert(varchar,autho_date,103) autho_date, book_name, op_user, convert(varchar,op_date,103) op_date, close_user, convert(varchar,close_date,103) close_date, reopen_user, convert(varchar,reopen_date,103) reopen_date, per_close_user, convert(varchar,per_close_date,103) per_close_date from gl_fin_month where fin_mon='" + finMon + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "gl_fin_month");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new FinMonth(dt.Rows[0]);
        }
        public static DataTable GetFinMonths(string finyr)
        {
            string connectionString = DataManager.OraConnString();
            string query = "select month_sl,fin_mon,quarter,convert(varchar,mon_start_dt,103) mon_start_dt,convert(varchar,mon_end_dt,103) mon_end_dt,year_flag from gl_fin_month where  fin_year='" + finyr + "'";
            DataTable dt= DataManager.ExecuteQuery(connectionString, query, "gl_fin_month");
            return dt;
        }
        public static DataTable GetOpenFinMonths()
        {
            string connectionString = DataManager.OraConnString();
            string query = "select month_sl,fin_mon,quarter,convert(varchar,mon_start_dt,103) mon_start_dt,convert(varchar,mon_end_dt,103) mon_end_dt,year_flag from gl_fin_month where  year_flag='O' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "gl_fin_month");
            return dt;
        }
        public static string getFinMonthByDate(string dt)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select fin_mon from gl_fin_month where convert(datetime,'"+dt+"',103) between mon_start_dt and mon_end_dt";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            if (maxValue == null)
            {
                maxValue = "0";
            }
            return maxValue.ToString();
        }

        public static object getFinMonthByDateCheckClose(string p)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select YEAR_FLAG from gl_fin_month where convert(datetime,'" + p + "',103) between mon_start_dt and mon_end_dt";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            if (maxValue == null)
            {
                maxValue = "0";
            }
            return maxValue.ToString();
        }

        public static string getFinMonthByDateNeverOpen(string p)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select YEAR_FLAG from gl_fin_month where convert(datetime,'" + p + "',103) between mon_start_dt and mon_end_dt";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            if (maxValue == null)
            {
                maxValue = "0";
            }
            return maxValue.ToString();
        }
    }
}