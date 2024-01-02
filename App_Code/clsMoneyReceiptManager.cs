using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Delve;

/// <summary>
/// Summary description for clsMoneyReceiptManager
/// </summary>
/// 
namespace Delve
{
    public class clsMoneyReceiptManager
    {
        public static void CreateMoneyReceipt(clsMoneyReceipt pay)
        {
            String connectionString = DataManager.OraConnString();
            //string query = " insert into MoneyReceipt(MrCode,MrDate,IssueId,DeedId,ClientId,PayAmt,PayMethod,ChequeNo,ChequeDate,BankName,advDeedId,used_advance_payment,aDeedId) values (" +
            //    " '"+pay.MrCode+"',  "+
            //    " convert(datetime,nullif('"+pay.MrDate+"',''),103), "+
            //    " '" + pay.IssueId + "',  " +
            //    " '"+pay.DeedId+"',  "+
            //    " '"+pay.ClientId+"',  "+
            //    " convert(numeric,nullif(replace('"+pay.PayAmt+"',',',''),'')), "+
            //    " '"+pay.PayMethod+"',  "+
            //    " '"+pay.ChequeNo+"',  "+
            //    " convert(datetime,nullif('"+pay.ChequeDate+"',''),103), "+
            //    " '" + pay.BankName + "', " +
            //    " '" + pay.AdvDeedId + "', " +
            //    " '" + pay.UsedAdvance + "'"+
            //    " '" + pay.aDeedId + "',  )";
            string InsertQuery = @"INSERT INTO [coldstorage].[dbo].[MoneyReceipt]
           ([MrCode],[MrDate],[IssueId],[DeedId],[advDeedId],[ClientId],[PayAmt],[PayMethod],[ChequeNo],[ChequeDate],[BankName],[used_advance_payment],[cancel_flg],[aDeedId])
     VALUES
           ('" + pay.MrCode + "',convert(datetime,nullif('" + pay.MrDate + "',''),103),'" + pay.IssueId + "','" + pay.DeedId + "','','" + pay.ClientId + "',convert(numeric,nullif(replace('" + pay.PayAmt + "',',',''),'')),'" + pay.PayMethod + "','" + pay.ChequeNo + "',convert(datetime,nullif('" + pay.MrDate + "',''),103),'" + pay.BankName + "',0.00,0,'" + pay.aDeedId + "')";
            DataManager.ExecuteNonQuery(connectionString, InsertQuery);
        }
        public static void UpdateMoneyReceipt(clsMoneyReceipt pay)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update MoneyReceipt set " +
                " MrDate= convert(datetime,nullif('" + pay.MrDate + "',''),103), " +
                " IssueId= '" + pay.IssueId + "',  " +
                " DeedId= '" + pay.DeedId + "',  " +
                " ClientId= '" + pay.ClientId + "',  " +
                " PayAmt= convert(numeric,nullif(replace('" + pay.PayAmt + "',',',''),'')), " +
                " PayMethod= '" + pay.PayMethod + "',  " +
                " ChequeNo= '" + pay.ChequeNo + "',  " +
                " ChequeDate= convert(datetime,nullif('" + pay.ChequeDate + "',''),103), " +
                " BankName= '" + pay.BankName + "' , " +
                " advDeedId= '" + pay.AdvDeedId + "', " +
                " '" + pay.UsedAdvance + "',"+
                "aDeedId='"+pay.aDeedId+"' where MrCode='" + pay.MrCode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        //internal static void DeleteMoneyReceipt(string brn)
        //{
        //    String connectionString = DataManager.OraConnString();
        //    string query = " delete from MoneyReceipt where MrCode='" + brn + "'";
        //    DataManager.ExecuteNonQuery(connectionString, query);
        //}
        public static void DeleteMoneyReceipt(string brn)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from MoneyReceipt where MrCode='" + brn + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static clsMoneyReceipt getMoneyReceipt(string brn)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select MrCode,convert(varchar,MrDate,103) MrDate,IssueId,DeedId,ClientId,PayAmt,PayMethod,ChequeNo,convert(varchar,ChequeDate,103) ChequeDate, BankName,advDeedId,used_advance_payment,aDeedId from MoneyReceipt where MrCode='" + brn + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "MoneyReceipt");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsMoneyReceipt(dt.Rows[0]);
        }
        public static DataTable getMoneyReceipts(string deed)
        {
            String connectionString = DataManager.OraConnString();
            string query = " Select MrCode,convert(varchar,MrDate,103) MrDate,IssueId,DeedId,ClientId,client_name,PayAmt,PayMethod,ChequeNo,convert(varchar,ChequeDate,103) ChequeDate, BankName,aDeedId from MoneyReceipt a, client_info b where a.ClientId=b.client_id and a.DeedId like '%" + deed + "%' order by MrCode desc,MrDate desc";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "MoneyReceipts");
            return dt;
        }

        public static string GetPreviousPayment(string itm)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select (coalesce(sum(PayAmt),0) + coalesce(SUM(used_advance_payment),0))  from  MoneyReceipt where  DeedId='" + itm + "' and cancel_flg=0 ";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            if (maxValue != null)
            {
                return maxValue.ToString();
            }
            return "0";
        }


        public static string GetPreviousPaymentForAdvanced(string itm)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select (coalesce(sum(PayAmt),0) + coalesce(SUM(used_advance_payment),0))  from  MoneyReceipt where  aDeedId='" + itm + "' and cancel_flg=0 ";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            if (maxValue != null)
            {
                return maxValue.ToString();
            }
            return "0";
        }


        public static object GetShowwAllManeyReciptInformation()
        {
            String connectionString = DataManager.OraConnString();
            string query = @"SELECT [MrCode]
      ,[MrDate]
      ,[IssueId]
      ,[DeedId]
      ,[advDeedId]
      ,[ClientId]
      ,[PayAmt]
      ,[PayMethod]
      ,[ChequeNo]
      ,[ChequeDate]
      ,[BankName]
,[aDeedId]
  FROM [coldstorage].[dbo].[MoneyReceipt]";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "MoneyReceipts");
            return dt;
        }

        //***************************** Advance amount Calculation *******************//

        public static decimal GetAdvanceAmount(string ClientId)
        {
            String connectionString = DataManager.OraConnString();
            string selectQuery = @"SELECT isnull(sum(isnull([PayAmt],0))-sum(isnull([used_advance_payment],0)),0)advance_payment FROM [MoneyReceipt] where [ClientId]='"+ClientId+"' and cancel_flg=0";
            return Convert.ToDecimal(DataManager.ExecuteScalar(connectionString, selectQuery));
  
        }

        public static decimal getBookingDtlsPrevious(string bookid)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            connection.Open();
            string selectQuery = @"SELECT isnull(sum([PayAmt]),0) FROM  [MoneyReceipt] where [IssueId]='"+bookid+"'";
            SqlCommand command = new SqlCommand(selectQuery,connection);
            return Convert.ToDecimal(command.ExecuteScalar());
            
        }
    }
}