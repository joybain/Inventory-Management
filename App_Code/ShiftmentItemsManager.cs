using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Delve;
using System.Data.SqlClient;
using System.Data;

/// <summary>
/// Summary description for ShiftmentItemsManager
/// </summary>
public class ShiftmentItemsManager
{
	public ShiftmentItemsManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static void SaveImageTemporary(int ID, byte[] Photo)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string variables = "[ID]";
        string values = " '" + ID + "' ";
        string query = "";

        if (Photo != null)
        {
            if (Photo.Length > 0)
            {
                variables = variables + ",Img";
                values = values + ",@img";
            }
        }
        query = " insert into [TemporaryImage] (" + variables + ")  values ( " + values + " )";
        SqlCommand cmnd;
        cmnd = new SqlCommand(query, sqlCon);
        SqlParameter img = new SqlParameter();
        img.SqlDbType = SqlDbType.VarBinary;
        img.ParameterName = "img";
        img.Value = Photo;
        cmnd.Parameters.Add(img);
        if (Photo == null)
        {
            cmnd.Parameters.Remove(img);
        }
        else
        {
            if (Photo.Length == 0)
            {
                cmnd.Parameters.Remove(img);
            }
        } 
        sqlCon.Open();
        cmnd.ExecuteNonQuery();
        sqlCon.Close();
    }

    public static void SaveItemShiftmentInfo(ShiftmentItems aShiftmentItems)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transection;
        try
        {     
            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;
            string Field = "", Value = "";
            if (!string.IsNullOrEmpty(aShiftmentItems.CustomerID))
            {
                Field = ",CustomerID"; Value = " ,'" + aShiftmentItems.CustomerID + "' ";
            }
            command.CommandText = @"INSERT INTO [ShiftmentItems]
            ([ShiftmentID],[ItemsID],[Label],[PartyID],[SupplierID],[PartyRate],[Quantity],[Remarks],[AddDate],[AddBy],SenderID " +
                                  Field + ") VALUES ('" + aShiftmentItems.ShiftmentNO + "','" +
                                  aShiftmentItems.ItemsID + "','" + aShiftmentItems.Label + "','" +
                                  aShiftmentItems.PartyID + "','" + aShiftmentItems.SupplierID + "','" +
                                  aShiftmentItems.PartyRate.Replace(",", "") + "','" +
                                  aShiftmentItems.Quantity.Replace(",", "") + "','" + aShiftmentItems.Remarks +
                                  "',GETDATE(),'" + aShiftmentItems.LoginBy + "','" + aShiftmentItems.SenderID + "' " +
                                  Value + ")";
            command.ExecuteNonQuery();

//            command1.CommandText = @"SELECT top(1)[ID]  FROM [ShiftmentItems] order by  [ID] desc";
//            int MstID=Convert.ToInt32(command1.ExecuteScalar());
//            foreach (DataRow dr in dtColor.Rows)
//            {
//                command.CommandText = @"INSERT INTO [ShiftmentItemsColorSize]
//               ([MasterId],[ColorID_SizeID],[Type])
//               VALUES ('" + MstID + "','" + dr["ID"].ToString() + "',0)";
//                command.ExecuteNonQuery();
//            }
//            foreach (DataRow dr in dtSize.Rows)
//            {
//                command.CommandText = @"INSERT INTO [ShiftmentItemsColorSize]
//               ([MasterId],[ColorID_SizeID],[Type])
//               VALUES ('" + MstID + "','" + dr["ID"].ToString() + "',1)";
//                command.ExecuteNonQuery();
//            }
//            foreach (DataRow dr in dtQt.Rows)
//            {
//                if (dr["Quantity"].ToString() != "")
//                {
//                    command1.CommandText = @"INSERT INTO [ShiftmentItemsQuantity]
//                ([MasterID],[ColorID],[SizeID],[Quantity])
//                VALUES
//                ('" + MstID + "','" + dr["ColorID"].ToString() + "','" + dr["SizeID"].ToString() + "','" + dr["Quantity"].ToString() + "')";
//                    command1.ExecuteNonQuery();
//                }
//            }           
            transection.Commit();
            connection.Close();            
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        
    }
    public static void UpdateItemShiftmentInfo(ShiftmentItems aShiftmentItems)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transection;
        try
        {
            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;

            string Value = "";
            if (!string.IsNullOrEmpty(aShiftmentItems.CustomerID))
            {
                Value = ",CustomerID='" + aShiftmentItems.CustomerID + "' ";
            }

            command.CommandText = @"UPDATE [ShiftmentItems]
             SET ShiftmentID='" + aShiftmentItems.ShiftmentNO + "',[ItemsID] ='" + aShiftmentItems.ItemsID +
                                  "' ,[Label] ='" +
                                  aShiftmentItems.Label + "',[PartyID] ='" + aShiftmentItems.PartyID +
                                  "',[SupplierID] ='" + aShiftmentItems.SupplierID + "' ,[PartyRate] ='" +
                                  aShiftmentItems.PartyRate.Replace(",", "") + "' ,[Quantity] ='" +
                                  aShiftmentItems.Quantity.Replace(",", "") + "',[Remarks] ='" +
                                  aShiftmentItems.Remarks +
                                  "' ,[UpdateDate] =GETDATE() ,[UpdteBy] ='" + aShiftmentItems.LoginBy + "' " + Value +
                                  "  WHERE ID='" +
                                  aShiftmentItems.ID + "' ";
            command.ExecuteNonQuery();
            
            transection.Commit();
            connection.Close();
            
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static void UpdateItemShiftmentInfoQty(string ID,string Qty)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transection;
        try
        {
            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;

            command.CommandText = @"UPDATE [ShiftmentItems]
             SET [Quantity] ='" + Qty.Replace(",", "") + "'  WHERE ID='" + ID + "' ";
            command.ExecuteNonQuery();

            transection.Commit();
            connection.Close();

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static DataTable GetShowShiftmentItemsDetails(string ShiftmentID)
    {
        String connectionString = DataManager.OraConnString();
        string Parameter = "";
        if (ShiftmentID != "")
        {
            Parameter = "Where t1.[ID]='" + ShiftmentID + "' order by ID desc ";
        }
        else { Parameter = "  order by ID desc "; }
        string query = @"SELECT top(200) t1.[ID]
      ,t2.ShiftmentNO
      ,t1.ItemsID
      ,t5.Code+' - '+t5.Name AS[Name]
      ,t1.[Label]
      ,t3.PartyName AS[PartyName]
      ,t4.ContactName AS [Supplier Name]
      ,Convert(decimal(18,2),t1.[PartyRate]) as PartyRate
      ,t1.[Quantity] 
	  ,t6.Name AS[SenderName]
	  ,t9.ContactName AS[CustomerName]     
  FROM [ShiftmentItems] t1 inner join ShiftmentAssigen t2 on t2.ID=t1.ShiftmentID left join PartyInfo t3 on t3.ID=t1.PartyID inner join Supplier t4 on t4.ID=t1.SupplierID inner join Item t5 on  t5.ID=t1.ItemsID
  left join ShiftmentSender t6 on t6.ID=t1.SenderID and t6.DeleteBy IS NULL
  left join [dbo].[ShiftmentSenderListDetails] t7 on t7.ID=t1.CustomerID and t7.DeleteBy IS NULL
  left join Customer t9 on t9.ID=t7.CustomerID " + Parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ShiftmentItems");
        return dt;
    }
    public static DataTable GetShiftmentItemsQuantity(string MasterID)
    {       
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlDataAdapter da = new SqlDataAdapter();
        connection.Open();
        da.SelectCommand = new SqlCommand("SP_ShiftmentItemsQuantity", connection);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.AddWithValue("@MasterID", MasterID);       
        //da.SelectCommand.CommandTimeout = 120;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_ShiftmentItemsQuantity");
        DataTable table = ds.Tables["SP_ShiftmentItemsQuantity"];       
        connection.Close();
        return table;
    }

    public static DataTable GetShowItemsInfo(string ID)
    {
        String connectionString = DataManager.OraConnString();
//        string query = @"SELECT tt1.ID AS SHITEMSid,t1.[MasterID],t1.[ItemsID],t1.[Quantity],t1.[Color],tt1.[ShiftmentID],t2.ShiftmentNO ,CONVERT(NVARCHAR,t2.ShiftmentDate,103) AS ShiftmentDate ,t1.[ItemsID] ,t3.Name,tt1.[Label] ,tt1.[PartyID],t4.PartyName AS PartyName ,tt1.[SupplierID] ,t5.ContactName AS SupplierName ,tt1.[PartyRate],tt1.[Quantity]
//,tt1.[Remarks],tm.CartoonNo    
//  FROM [ShiftmentBoxingItemsDtl] t1 inner join dbo.ShiftmentBoxingMst tm on tm.ID=t1.MasterID  inner join [ShiftmentItems] tt1 on tt1.ID=t1.ItemsID Left join ShiftmentAssigen t2 on t2.ID=tt1.ShiftmentID Left join Item t3 on t3.ID=tt1.ItemsID Left join PartyInfo t4 on t4.ID=tt1.PartyID Left join Supplier t5 on t5.ID=tt1.SupplierID where t1.MasterID='" + ID + "'";
        string query = @"Select tt1.[ShiftmentID],t2.ShiftmentNO ,CONVERT(NVARCHAR,t2.ShiftmentDate,103) AS ShiftmentDate ,tt1.[ItemsID] ,t3.Name,tt1.[Label] ,tt1.[PartyID],t4.PartyName AS PartyName ,tt1.[SupplierID] ,t5.ContactName AS SupplierName ,tt1.[PartyRate],tt1.[Quantity]
,tt1.[Remarks],tt1.SenderID,tt1.CustomerID from  [ShiftmentItems] tt1 Left join ShiftmentAssigen t2 on t2.ID=tt1.ShiftmentID Left join Item t3 on t3.ID=tt1.ItemsID Left join PartyInfo t4 on t4.ID=tt1.PartyID Left join Supplier t5 on t5.ID=tt1.SupplierID where tt1.ID='" + ID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ShiftmentItems");
        return dt;
    }
    //*********************** Singel Curtton ****************//
    public static DataTable GetShowItemsInfo(string ID,string SingelCurtton)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT tt1.ID AS SHITEMSid,t1.[MasterID],t1.[ItemsID],t1.[Quantity],t1.[Color],tt1.[ShiftmentID],t2.ShiftmentNO ,CONVERT(NVARCHAR,t2.ShiftmentDate,103) AS ShiftmentDate,t3.Name,tt1.[Label] ,tt1.[PartyID],t4.PartyName AS PartyName ,tt1.[SupplierID] ,t5.ContactName AS SupplierName ,tt1.[PartyRate],tt1.[Quantity]
        ,tt1.[Remarks],tm.CartoonNo,t3.ID AS[ProductID]    
          FROM [ShiftmentBoxingItemsDtl] t1 inner join dbo.ShiftmentBoxingMst tm on tm.ID=t1.MasterID  inner join [ShiftmentItems] tt1 on tt1.ID=t1.ItemsID Left join ShiftmentAssigen t2 on t2.ID=tt1.ShiftmentID Left join Item t3 on t3.ID=tt1.ItemsID Left join PartyInfo t4 on t4.ID=tt1.PartyID Left join Supplier t5 on t5.ID=tt1.SupplierID where t1.MasterID='" + ID + "'";
//        string query = @"Select tt1.[ShiftmentID],t2.ShiftmentNO ,CONVERT(NVARCHAR,t2.ShiftmentDate,103) AS ShiftmentDate ,tt1.[ItemsID] ,t3.Name,tt1.[Label] ,tt1.[PartyID],t4.PartyName AS PartyName ,tt1.[SupplierID] ,t5.ContactName AS SupplierName ,tt1.[PartyRate],tt1.[Quantity]
//,tt1.[Remarks] from  [ShiftmentItems] tt1 Left join ShiftmentAssigen t2 on t2.ID=tt1.ShiftmentID Left join Item t3 on t3.ID=tt1.ItemsID Left join PartyInfo t4 on t4.ID=tt1.PartyID Left join Supplier t5 on t5.ID=tt1.SupplierID where tt1.ID='" + ID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ShiftmentItems");
        return dt;
    }
    public static DataTable GetShowItemsInfoReport(string ID, string SingelCurtton, string SenderID, string CustomerID)
    {
        // and 
        string Parameter = "";
        String connectionString = DataManager.OraConnString();
        if (!string.IsNullOrEmpty(SenderID) && string.IsNullOrEmpty(CustomerID))
        {
            Parameter = " and tt1.SenderID='" + SenderID + "' ";
        }
        else if (!string.IsNullOrEmpty(SenderID) && !string.IsNullOrEmpty(CustomerID))
        {
            Parameter = " and tt1.SenderID='" + SenderID + "' and  tt1.CustomerID='" + CustomerID + "' ";
        }

        string query = @"SELECT tt1.ID AS SHITEMSid,t1.[MasterID],t1.[ItemsID],t1.[Quantity],t1.[Color],tt1.[ShiftmentID],t2.ShiftmentNO ,CONVERT(NVARCHAR,t2.ShiftmentDate,103) AS ShiftmentDate,t3.Name,tt1.[Label] ,tt1.[PartyID],t4.PartyName AS PartyName ,tt1.[SupplierID] ,t5.ContactName AS SupplierName ,tt1.[PartyRate],tt1.[Quantity]
        ,tt1.[Remarks],tm.CartoonNo,t3.ID AS[ProductID],t7.ContactName AS[CustomerName]    
          FROM [ShiftmentBoxingItemsDtl] t1 inner join dbo.ShiftmentBoxingMst tm on tm.ID=t1.MasterID  inner join [ShiftmentItems] tt1 on tt1.ID=t1.ItemsID Left join ShiftmentAssigen t2 on t2.ID=tt1.ShiftmentID Left join Item t3 on t3.ID=tt1.ItemsID Left join PartyInfo t4 on t4.ID=tt1.PartyID Left join Supplier t5 on t5.ID=tt1.SupplierID
		  left join [dbo].[ShiftmentSenderListDetails] t6 on t6.ID=tt1.CustomerID
		  left join Customer t7 on t7.ID=t6.CustomerID where t1.MasterID='" + ID + "' " + Parameter + " ";
        //        string query = @"Select tt1.[ShiftmentID],t2.ShiftmentNO ,CONVERT(NVARCHAR,t2.ShiftmentDate,103) AS ShiftmentDate ,tt1.[ItemsID] ,t3.Name,tt1.[Label] ,tt1.[PartyID],t4.PartyName AS PartyName ,tt1.[SupplierID] ,t5.ContactName AS SupplierName ,tt1.[PartyRate],tt1.[Quantity]
        //,tt1.[Remarks] from  [ShiftmentItems] tt1 Left join ShiftmentAssigen t2 on t2.ID=tt1.ShiftmentID Left join Item t3 on t3.ID=tt1.ItemsID Left join PartyInfo t4 on t4.ID=tt1.PartyID Left join Supplier t5 on t5.ID=tt1.SupplierID where tt1.ID='" + ID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ShiftmentItems");
        return dt;
    }
    public static DataTable GetShowItemsShiftmentReportInfo(string ID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]
      ,t1.[ShiftmentID]
      ,t2.ShiftmentNO
      ,CONVERT(NVARCHAR,t2.ShiftmentDate,103) AS ShiftmentDate      
      ,t1.[ItemsID]
      ,t3.Name
      ,t1.[Label]
      ,t1.[PartyID]
      ,t4.PartyName AS PartyName
      ,t1.[SupplierID]
      ,t5.ContactName AS SupplierName
      ,t1.[PartyRate]
      ,t1.[Quantity]
      ,t1.[Remarks]      
  FROM [ShiftmentItems] t1 inner join ShiftmentAssigen t2 on t2.ID=t1.ShiftmentID inner join Item t3 on t3.ID=t1.ItemsID Left join PartyInfo t4 on t4.ID=t1.PartyID inner join Supplier t5 on t5.ID=t1.SupplierID where t1.ShiftmentID='" + ID + "' order by t4.PartyName,t1.[ID]";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ShiftmentItems");
        return dt;
    }

    public static void GetDeleteShiftmentInformation(ShiftmentItems aShiftmentItems)
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

            command.CommandText = @"DELETE FROM [ShiftmentItems]   WHERE ID='" + aShiftmentItems.ID + "' ";
            command.ExecuteNonQuery();           

            transaction.Commit();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    public static DataTable GetShowShiftmentItemsOnShoftmentAssignID(string ShiftmentAssID,string SenderID,string CustomerID)
    {
//        String connectionString = DataManager.OraConnString();
//        string query = @"SELECT t1.ID AS item_code
//        ,t5.Name+' - '+t4.ContactName+' - '+CONVERT(nvarchar,t1.Quantity)  AS item_desc       
//        FROM [ShiftmentItems] t1 inner join ShiftmentAssigen t2 on t2.ID=t1.ShiftmentID Left join PartyInfo t3 on t3.ID=t1.PartyID inner join Supplier t4 on t4.ID=t1.SupplierID inner join Item t5 on  t5.ID=t1.ItemsID  where t2.ID='" + ShiftmentAssID + "'";
//        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ShiftmentItems");
//        return dt;

        using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
        {
            SqlCommand sqlComm = new SqlCommand("SP_Search_ShipmentItems", conn);
            if (!string.IsNullOrEmpty(ShiftmentAssID))
            {
                sqlComm.Parameters.AddWithValue("@ShipmentID", ShiftmentAssID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@ShipmentID", null);
            }
            if (string.IsNullOrEmpty(SenderID))
            {
                sqlComm.Parameters.AddWithValue("@SenderID", null);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@SenderID", SenderID);
            }
            if (string.IsNullOrEmpty(CustomerID))
            {
                sqlComm.Parameters.AddWithValue("@CustomerID", null);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@CustomerID", CustomerID);
            }
            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "SP_Search_ShipmentItems");
            DataTable dtItems = ds.Tables["SP_Search_ShipmentItems"];
            return dtItems;
        }
    }

    public static DataTable GetShowItemsInfo_Barcode(string Cartoon1, string Cartoon2,string shiftmentID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT tt1.ID AS SHITEMSid,t1.[MasterID],t1.[ItemsID],t1.[Quantity],t1.[Color],tt1.[ShiftmentID],t2.ShiftmentNO ,CONVERT(NVARCHAR,t2.ShiftmentDate,103) AS ShiftmentDate ,t1.[ItemsID] ,t3.Name,tt1.[Label] ,tt1.[PartyID],t4.PartyName AS PartyName ,tt1.[SupplierID] ,t5.ContactName AS SupplierName ,tt1.[PartyRate],tt1.[Quantity]
        ,tt1.[Remarks],tm.CartoonNo    
          FROM [ShiftmentBoxingItemsDtl] t1 inner join dbo.ShiftmentBoxingMst tm on tm.ID=t1.MasterID  inner join [ShiftmentItems] tt1 on tt1.ID=t1.ItemsID Left join ShiftmentAssigen t2 on t2.ID=tt1.ShiftmentID Left join Item t3 on t3.ID=tt1.ItemsID Left join PartyInfo t4 on t4.ID=tt1.PartyID Left join Supplier t5 on t5.ID=tt1.SupplierID where convert(int,tm.CartoonNo) between convert(int,'" + Cartoon1 + "') and convert(int,'" + Cartoon2 + "') AND tm.ShiftmentID='" + shiftmentID + "' order by convert(int,tm.CartoonNo) asc ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ShiftmentItems");
        return dt;
    }

    public static DataTable GetShiftmetBySearch(string ShipmentNo, string ItemNo)
    {
        String connectionString = DataManager.OraConnString();
        string Parameter = "";
        if (!string.IsNullOrEmpty(ShipmentNo) && string.IsNullOrEmpty(ItemNo))
        {
            Parameter = "where t2.ShiftmentNO='" + ShipmentNo + "' ";
        }
        else if (string.IsNullOrEmpty(ShipmentNo) && !string.IsNullOrEmpty(ItemNo))
        {
            Parameter = " where t5.Name='" + ItemNo + "' ";
        }
        else if (!string.IsNullOrEmpty(ShipmentNo) && !string.IsNullOrEmpty(ItemNo))
        {
            Parameter = " where t2.ShiftmentNO='" + ShipmentNo + "' and t5.Name='" + ItemNo + "' ";
        }
        else { Parameter = "  order by ID desc "; }
        string query = @"SELECT top(200) t1.[ID]
      ,t2.ShiftmentNO
      ,t1.ItemsID
      ,t5.Code+' - '+t5.Name AS[Name]
      ,t1.[Label]
      ,t3.PartyName AS[PartyName]
      ,t4.ContactName AS [Supplier Name]
      ,Convert(decimal(18,2),t1.[PartyRate]) as PartyRate
      ,t1.[Quantity] 
	  ,t6.Name AS[SenderName]
	  ,t9.ContactName AS[CustomerName]     
  FROM [ShiftmentItems] t1 inner join ShiftmentAssigen t2 on t2.ID=t1.ShiftmentID left join PartyInfo t3 on t3.ID=t1.PartyID inner join Supplier t4 on t4.ID=t1.SupplierID inner join Item t5 on  t5.ID=t1.ItemsID
  left join ShiftmentSender t6 on t6.ID=t1.SenderID and t6.DeleteBy IS NULL
  left join [dbo].[ShiftmentSenderListDetails] t7 on t7.ID=t1.CustomerID and t7.DeleteBy IS NULL
  left join Customer t9 on t9.ID=t7.CustomerID " + Parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ShiftmentItems");
        return dt;
    }

    public DataTable GetShowSipmentItemsStatus(string ShipmentID, string ItemsID)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlDataAdapter da = new SqlDataAdapter();
        connection.Open();
        da.SelectCommand = new SqlCommand("SP_ShipmentItemsStatus", connection);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        if (string.IsNullOrEmpty(ShipmentID))
        {
            da.SelectCommand.Parameters.AddWithValue("@ShipmentID", null);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@ShipmentID", ShipmentID);
        }

        if (string.IsNullOrEmpty(ItemsID))
        {
            da.SelectCommand.Parameters.AddWithValue("@ItemsID", null);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@ItemsID", ItemsID);
        }

       
        //da.SelectCommand.CommandTimeout = 120;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_ShipmentItemsStatus");
        DataTable table = ds.Tables["SP_ShipmentItemsStatus"];
        connection.Close();
        return table;
    }

    public static void UpdateChangerate(string Rate, string LoginBy,string ID)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transection;
        try
        {
            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;

            command.CommandText = @"UPDATE [ShiftmentItems]
             SET [PartyRate] ='" +
                                  Rate.Replace(",", "") + "' ,ChangeRateBy='" + LoginBy +
                                  "',ChangeDate=GETDATE()  WHERE ID='" +
                                  ID + "' ";
            command.ExecuteNonQuery();

            transection.Commit();
            connection.Close();

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}