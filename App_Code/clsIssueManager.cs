using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Delve;

/// <summary>
/// Summary description for clsIssueManager
/// </summary>
/// 
namespace Delve
{
    public class clsIssueManager
    {
        public static void CreateIssueMst(clsIssueMst mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = " insert into IssueMst(IssueId,IssueDate,BookId,DeedId,ClientId,Remarks) values (" +
                "  '" + mst.IssueId + "', convert(datetime,nullif('" + mst.IssueDate + "',''),103),'" + mst.BookId + "','" + mst.DeedId + "', '" + mst.ClientId + "','" + mst.Remarks + "' )";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateIssueMst(clsIssueMst mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update IssueMst set IssueDate = convert(datetime,nullif('" + mst.IssueDate + "',''),103),BookId='" + mst.BookId + "', DeedId='"+mst.DeedId+"', ClientId='" + mst.ClientId + "', Remarks= '" + mst.Remarks + "'  where IssueId= '" + mst.IssueId + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteIssueMst(clsIssueMst mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from IssueMst where IssueId= '" + mst.IssueId + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static clsIssueMst getIssueMst(string mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select IssueId, convert(varchar,IssueDate,103) IssueDate,BookId,DeedId, ClientId,Remarks from IssueMst where IssueId='" + mst + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "IssueMaster");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsIssueMst(dt.Rows[0]);
        }
        public static DataTable getIssueMsts()
        {
            String connectionString = DataManager.OraConnString();
            string query = "select IssueId, convert(varchar,IssueDate,103) IssueDate,BookId,a.DeedId,a.ClientId,client_name ClientName, Remarks from IssueMst a,client_info b where a.ClientId=b.client_id";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "IssueMasters");
            return dt;
        }
        public static DataTable getIssueMstClients(string client)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select IssueId, convert(varchar,IssueDate,103) IssueDate,BookId,DeedId,ClientId,client_name ClientName, Remarks from IssueMst a,client_info b where a.ClientId=b.client_id and upper(client_name) like '%" + client + "%'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "IssueMasters");
            return dt;
        }
        public static void CreateIssueDtl(clsIssueDtl dtl)
        {
            String connectionString = DataManager.OraConnString();
            string query = "insert into IssueDtl (IssueId,item_code,msr_unit_code,qnty) values (" +
                "  '" + dtl.IssueId + "', '" + dtl.ItemCode + "', '" + dtl.MsrUnitCode + "', convert(numeric,nullif('" + dtl.Qnty + "','')))";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void DeleteIssueDtl(string dtl)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from IssueDtl where IssueId= '" + dtl + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }


        public static clsIssueDtl getIssueDtl(string mst, string dtl)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select IssueId, item_code ,msr_unit_code ,qnty  from IssueDtl where IssueId='" + mst + "' and item_code='" + dtl + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "IssueDtl");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsIssueDtl(dt.Rows[0]);
        }
        public static DataTable getIssueDtls(string mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select IssueId,a.item_code, b.item_desc,a.msr_unit_code, c.msr_unit_desc,convert(varchar,a.qnty) qnty from IssueDtl a,item_mst b,measure_unit c where a.item_code=b.item_code and a.msr_unit_code=c.msr_unit_code and IssueId='" + mst + "' order by item_code";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "IssueDtls");
            return dt;
        }

        public static DataTable getIssueDtlsReport(string mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"SELECT a.[item_code]
      ,b.ITEM_DESC
      ,a.[msr_unit_code]
      ,c.MSR_UNIT_DESC
      ,a.[qnty]
      ,b.ITEM_RATE
      ,c.MSR_UNIT_DESC
      ,(b.ITEM_RATE*a.qnty) total     
  FROM [coldStorage].[dbo].[BookingDtl] a
  inner join measure_unit c  on (a.msr_unit_code=c.msr_unit_code)
  inner join item_mst b on (a.item_code=b.item_code) where [BookId]='" + mst+"'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "IssueDtls");
            return dt;
        }

        //public static DataTable getIssueDtlsReport(string mst)
        //{
        //    String connectionString = DataManager.OraConnString();
        //    string query = "select distinct a.IssueId,a.item_code, b.item_desc,a.msr_unit_code, c.msr_unit_desc,convert(varchar,a.qnty) qnty,f.item_rate,convert(decimal(13,2),a.qnty*f.item_rate) total " +
        //    " from IssueDtl a inner join item_mst b on (a.item_code=b.item_code) inner join measure_unit c  on (a.msr_unit_code=c.msr_unit_code) " +
        //    " inner join IssueMst d on (a.IssueId=d.IssueId) inner join BookingMst e on (d.BookId=e.BookId) inner join DeedDtl f on (e.DeedId= f.DeedId and a.item_code=f.item_code) " +
        //    " where a.IssueId='" + mst + "' order by a.item_code";
        //    DataTable dt = DataManager.ExecuteQuery(connectionString, query, "IssueDtls");
        //    return dt;
        //}

        public static DataTable getIssueDtlsAll(string deed)
        {
            String connectionString = DataManager.OraConnString();
            string query = "select distinct a.IssueId,convert(varchar,d.IssueDate,103) IssueDate,a.item_code, b.item_desc,a.msr_unit_code, c.msr_unit_desc,convert(varchar,a.qnty) qnty,f.item_rate,convert(decimal(13,2),a.qnty*f.item_rate) total " +
            " from IssueDtl a inner join item_mst b on (a.item_code=b.item_code) inner join measure_unit c  on (a.msr_unit_code=c.msr_unit_code) " +
            " inner join IssueMst d on (a.IssueId=d.IssueId) inner join BookingMst e on (d.BookId=e.BookId) inner join DeedDtl f on (e.DeedId= f.DeedId and a.item_code=f.item_code) " +
            " where d.DeedId='" + deed + "' order by a.IssueId,a.item_code";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "IssueDtlsAll");
            return dt;
        }
         
        //********************** MoneyReceipt Chaque *****************************//

        public static int moneyreceiptchaque(string mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"SELECT case when  t4.aDeedId !='' then 1 else 0 end as ID
                            from [BookingMst] t1 
                            inner join DeedMst t3 on t1.DeedId=t3.DeedId and t1.BookId='" + mst + "' inner join dbo.SlipMst t4 on t4.SlipId=t3.SlipId ";
            int ID;
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter myAdapter = new SqlDataAdapter(query, myConnection))
                {
                    DataSet ds = new DataSet();
                    myAdapter.Fill(ds, "BookingMst");
                    ds.Tables[0].TableName = "BookingMst";
                    return ID = Convert.ToInt32( ds.Tables[0].Rows[0][0]);
                }
            }
            
            
          
            
        }



        public static int AdvanceDeedCheck(string Deed)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"SELECT case when  t2.aDeedId!='' then 1 else 0 end as[status]
  FROM [DeedMst] t1
  inner join SlipMst t2 on t2.SlipId=t1.SlipId where t1.[DeedId]='" + Deed + "'";
            int ID;
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter myAdapter = new SqlDataAdapter(query, myConnection))
                {
                    DataSet ds = new DataSet();
                    myAdapter.Fill(ds, "DeedMst");
                    ds.Tables[0].TableName = "DeedMst";
                    return ID = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                }
            }
        }
    }
}