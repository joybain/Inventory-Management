using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Delve;
using System.Data.SqlClient;
using System.Data;

/// <summary>
/// Summary description for PurchaseOrderManager
/// </summary>
public class PurchaseOrderManager
{
	public PurchaseOrderManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static PurchaseOrderInfo GetPurchaseOrderMst(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT [ID]
      ,[PO]
      ,CONVERT(NVARCHAR,[PODate],103)PODate
      ,[SupplierID]
      ,[TermsOfDelivery]
      ,[TermsOfPayment]
      ,CONVERT(NVARCHAR,[ExpDelDate],103)ExpDelDate
      ,[OrderStatus]
      ,[CreatedBy]
      ,[CreatedDate]
      ,[ModifiedBy]
      ,[ModifiedDate],POCode
  FROM [ItemPurOrderMst] where ID='" + ID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurOrderMst");
        sqlCon.Close();
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new PurchaseOrderInfo(dt.Rows[0]);
    }

    public static void SavePurchaseOrder(PurchaseOrderInfo pomst, DataTable dt)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"INSERT INTO [ItemPurOrderMst]
           ([PO],[PODate],[SupplierID],[TermsOfDelivery],[TermsOfPayment],[ExpDelDate],[OrderStatus],[CreatedBy],[CreatedDate],POCode)
     VALUES
           ('" + pomst.PO + "',convert(date,'" + pomst.PODate + "',103),'" + pomst.SupplierID + "','" + pomst.TermsOfDelivery + "','" + pomst.TermsOfPayment + "',convert(date,'" + pomst.ExpDelDate + "',103),'" + pomst.OrderStatus + "','" + pomst.LoginBy + "',GETDATE(),'" + pomst.POCode + "')";
            command.ExecuteNonQuery();

            command.CommandText = @"SELECT top(1) [ID]  FROM [ItemPurOrderMst] order by ID desc";
            string OrderMstID = command.ExecuteScalar().ToString();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["item_code"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [ItemPurOrderDtl]
           ([ItemOrderMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],[MsrUnitCode])
     VALUES
           ('" + OrderMstID + "','" + dr["ID"].ToString() + "','" + dr["item_rate"].ToString() + "','" + dr["qnty"].ToString() + "','" + Convert.ToDouble(dr["item_rate"].ToString()) * Convert.ToDouble(dr["qnty"].ToString()) + "','" + pomst.LoginBy + "',GETDATE(),'" + dr["msr_unit_code"].ToString() + "')";
                    command.ExecuteNonQuery();
                }
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    public static void UpdatePurchaseOrder(PurchaseOrderInfo pomst, DataTable dt)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;           

            command.CommandText = @"UPDATE [ItemPurOrderMst]
   SET [PODate] =convert(date,'" + pomst.PODate + "',103) ,[SupplierID] ='" + pomst.SupplierID + "',[TermsOfDelivery] ='" + pomst.TermsOfDelivery + "' ,[TermsOfPayment] = '" + pomst.TermsOfPayment + "',[ExpDelDate] =convert(date,'" + pomst.ExpDelDate + "',103) ,[OrderStatus] ='" + pomst.OrderStatus + "',[ModifiedBy] ='" + pomst.LoginBy + "' ,[ModifiedDate] =GETDATE(),POCode='" + pomst.POCode+ "' WHERE ID='" + pomst.ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"delete from ItemPurOrderDtl where ItemOrderMstID='" + pomst.ID + "'";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["item_code"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [ItemPurOrderDtl]
           ([ItemOrderMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],[MsrUnitCode])
     VALUES
           ('" + pomst.ID + "','" + dr["ID"].ToString() + "','" + dr["item_rate"].ToString() + "','" + dr["qnty"].ToString() + "','" + Convert.ToDouble(dr["item_rate"].ToString()) * Convert.ToDouble(dr["qnty"].ToString()) + "','" + pomst.LoginBy + "',GETDATE(),'" + dr["msr_unit_code"].ToString() + "')";
                    command.ExecuteNonQuery();
                }
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    public static void DeletePurchaseOrder(PurchaseOrderInfo pomst)
    {

        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"delete from ItemPurOrderMst where ID='" + pomst.ID + "'";
            command.ExecuteNonQuery();   

            command.CommandText = @"delete from ItemPurOrderDtl where ItemOrderMstID='" + pomst.ID + "'";
            command.ExecuteNonQuery();            

            transaction.Commit();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }
    }

    public static DataTable GetShowPurchaseOrder(string ID,string Flag)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = "";
        if (Flag.Equals("1"))
        {
             query = @"SELECT t1.ID
      ,t1.[PO]
      ,CONVERT(NVARCHAR,t1.[PODate],103) AS PODate
      ,CONVERT(NVARCHAR,t2.ContactName,103) AS  Supplier_Name    
       ,CONVERT(NVARCHAR,t1.[ExpDelDate],103) AS ExpDelDate
      ,case when t1.[OrderStatus]='P' then 'Pending' 
       when t1.[OrderStatus]='C' then 'Completed' 
       when t1.[OrderStatus]='CA' then 'Cancel' else '' end AS [Status]
       ,isnull(t1.POCode,'') AS POCode
       ,t2.Mobile
  FROM [ItemPurOrderMst] t1 inner join Supplier t2 on t2.ID=t1.[SupplierID] order by t1.[OrderStatus] desc,t1.ID desc";
        }
        else if (Flag.Equals("2"))
        {
            query = @"SELECT t1.ID
      ,t1.[PO]
      ,CONVERT(NVARCHAR,t1.[PODate],103) AS PODate
      ,CONVERT(NVARCHAR,t2.ContactName,103) AS  Supplier_Name    
      ,CONVERT(NVARCHAR,t1.[ExpDelDate],103) AS ExpDelDate
      ,case when t1.[OrderStatus]='P' then 'Pending' 
       when t1.[OrderStatus]='C' then 'Completed' 
       when t1.[OrderStatus]='CA' then 'Cancel' else '' end AS [Status]
       ,isnull(t1.POCode,'') AS POCode,t2.Mobile
          FROM [ItemPurOrderMst] t1 inner join Supplier t2 on t2.ID=t1.[SupplierID] and t1.ApprovedBy IS NULL order by t1.[OrderStatus] desc,t1.ID desc";
        }
        else if (Flag.Equals("3"))
        {
            query = @"SELECT t1.ID
      ,t1.[PO]
      ,CONVERT(NVARCHAR,t1.[PODate],103) AS PODate
      ,CONVERT(NVARCHAR,t2.ContactName,103) AS  Supplier_Name    
       ,CONVERT(NVARCHAR,t1.[ExpDelDate],103) [ExpDelDate]
      ,[TermsOfDelivery]
	  ,[TermsOfPayment]
      ,case when t1.[OrderStatus]='P' then 'Pending' 
       when t1.[OrderStatus]='C' then 'Completed' 
       when t1.[OrderStatus]='CA' then 'Cancel' else '' end AS [Status]
       ,isnull(t1.POCode,'') AS POCode,t2.Mobile
          FROM [ItemPurOrderMst] t1 inner join Supplier t2 on t2.ID=t1.[SupplierID] and t1.ApprovedBy IS NULL where t1.ID='" +
                    ID + "' order by t1.[OrderStatus] desc,t1.ID desc";
        }

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurOrderMst");
        return dt;       
    }

    public static DataTable GetPurchaseOrderItemsDetails(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.[ItemID] AS ID
      ,t2.Code AS item_code
      ,t2.Name AS item_desc
      ,t1.[UnitPrice] AS item_rate
      ,t1.[Quantity] AS qnty
      ,REPLACE(CONVERT(varchar(20), (CAST (t1.Total as money)), 1), '.00', '')+'.00' as Total     
      ,t1.[MsrUnitCode] AS msr_unit_code
      ,t3.Name AS UMO
      ,'0' AS Additional,t2.ItemImage
      ,convert(nvarchar,t1.[ExpireDate],103) as [ExpireDate]
         FROM ItemPurOrderDtl t1 inner join Item t2 on t2.ID=t1.ItemID inner join UOM t3 on t3.ID=t1.MsrUnitCode where t1.ItemOrderMstID='" + ID + "' " +
                       "union all select '','','',NULL,NULL,NULL,NULL,'','0',NULL ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurOrderDtl");
        return dt;    
    }

    public static DataTable GetShowOrder(string OrderId)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query =
            @"SELECT [ID],[PO],[PODate],[SupplierID],[TermsOfDelivery],[TermsOfPayment],[ExpDelDate],[OrderStatus],[POCode],[SearchSupplerDate],[SearchPoCode] FROM [View_SearchPurchaseOrder] where UPPER(SearchPoCode)='" +
            OrderId.ToUpper() + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurOrderDtl");
        return dt; 
    }

    public static DataTable GetShowPurchaseMst(string GrNo, string SupplierCode, string ReceiveFromDate,
        string ReceiveToDate, string Flag)
    {
        string per = "", query = "";
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        if (Flag.Equals("1"))
        {
            if (!string.IsNullOrEmpty(GrNo))
            {
                per = "where  t1.PO='" + GrNo + "' ";
            }

            if (!string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) &&
                !string.IsNullOrEmpty(ReceiveFromDate) && !string.IsNullOrEmpty(ReceiveToDate))
            {

                per = "where t2.ID='" + SupplierCode +
                      "' and  (CONVERT(date,t1.[PODate],103) between CONVERT(date,'" + ReceiveFromDate +
                      "',103) and CONVERT(date,'" + ReceiveToDate + "',103))";
            }

            //&& (string.IsNullOrEmpty(GrNo) | string.IsNullOrEmpty(ReceiveFromDate) | string.IsNullOrEmpty(ReceiveFromDate)))
            if (!string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) &&
                (string.IsNullOrEmpty(ReceiveFromDate) | string.IsNullOrEmpty(ReceiveToDate)))
            {

                per = "where t2.ID='" + SupplierCode + "'";
            }

            if (string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) &&
                !string.IsNullOrEmpty(ReceiveFromDate) && !string.IsNullOrEmpty(ReceiveToDate))
            {

                per = "where  (CONVERT(date,t1.[PODate],103) between CONVERT(date,'" + ReceiveFromDate +
                      "',103) and CONVERT(date,'" + ReceiveToDate + "',103))";
            }

            query = @"SELECT t1.ID
      ,t1.[PO]
      ,CONVERT(NVARCHAR,t1.[PODate],103) AS PODate
      ,CONVERT(NVARCHAR,t2.ContactName,103) AS  Supplier_Name    
      ,t1.[ExpDelDate]
      ,case when t1.[OrderStatus]='P' then 'Pending' 
       when t1.[OrderStatus]='C' then 'Completed' 
       when t1.[OrderStatus]='CA' then 'Cancel' else '' end AS [Status]
       ,isnull(t1.POCode,'') AS POCode
  FROM [ItemPurOrderMst] t1 inner join Supplier t2 on t2.ID=t1.[SupplierID]  " + per + " order By t1.ID desc";
        }
        else
        {
            if (!string.IsNullOrEmpty(GrNo))
            {
                per = "where  t1.PO='" + GrNo + "' and ApprovedBy IS NULL ";
            }

            if (!string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) &&
                !string.IsNullOrEmpty(ReceiveFromDate) && !string.IsNullOrEmpty(ReceiveToDate))
            {

                per = "where t2.Code='" + SupplierCode +
                      "' and  (CONVERT(date,t1.[PODate],103) between CONVERT(date,'" + ReceiveFromDate +
                      "',103) and CONVERT(date,'" + ReceiveToDate + "',103)) and ApprovedBy IS NULL ";
            }

            //&& (string.IsNullOrEmpty(GrNo) | string.IsNullOrEmpty(ReceiveFromDate) | string.IsNullOrEmpty(ReceiveFromDate)))
            if (!string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) &&
                (string.IsNullOrEmpty(ReceiveFromDate) | string.IsNullOrEmpty(ReceiveToDate)))
            {

                per = "where t2.Code='" + SupplierCode + "' and ApprovedBy IS NULL ";
            }

            if (string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) &&
                !string.IsNullOrEmpty(ReceiveFromDate) && !string.IsNullOrEmpty(ReceiveToDate))
            {

                per = "where  (CONVERT(date,t1.[PODate],103) between CONVERT(date,'" + ReceiveFromDate +
                      "',103) and CONVERT(date,'" + ReceiveToDate + "',103)) and ApprovedBy IS NULL ";
            }

            query = @"SELECT t1.ID
      ,t1.[PO]
      ,CONVERT(NVARCHAR,t1.[PODate],103) AS PODate
      ,CONVERT(NVARCHAR,t2.ContactName,103) AS  Supplier_Name    
      ,t1.[ExpDelDate]
      ,case when t1.[OrderStatus]='P' then 'Pending' 
       when t1.[OrderStatus]='C' then 'Completed' 
       when t1.[OrderStatus]='CA' then 'Cancel' else '' end AS [Status]
       ,isnull(t1.POCode,'') AS POCode
  FROM [ItemPurOrderMst] t1 inner join Supplier t2 on t2.ID=t1.[SupplierID]  " + per + " order By t1.ID desc";
        }

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurOrderMst");
        return dt;
    }

    public static DataTable GetSupplierInfo(string Supplieer)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @" select ID,Code,Name,ContactName,Phone from Supplier where Upper(isnull(Code,'')+' - '+ContactName)=UPPER('" + Supplieer + "')";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
        return dt;
    }

    public static void GetApproved(string ID,string LoginBy)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"UPDATE [dbo].[ItemPurOrderMst]
         SET [ApprovedBy] ='" + LoginBy + "' ,[ApprovedDate] =GETDATE() WHERE ID='" + ID + "' ";
        DataManager.ExecuteNonQuery(connectionString, query);
    }
}