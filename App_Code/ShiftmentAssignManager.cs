using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Delve;
using System.Data;

/// <summary>
/// Summary description for ShiftmentAssign
/// </summary>
public class ShiftmentAssignManager
{
    public ShiftmentAssignManager()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static void SaveShiftmentAssigInfo(string ShiftmentNO, string ShiftmentDate, string LoginBy, string Status,
        DataTable dtShipmentSender, string Note)
    {

        string connectionString = DataManager.OraConnString();
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;

        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"INSERT INTO [ShiftmentAssigen]
           ([ShiftmentNO]
           ,[ShiftmentDate]
           ,[AddBy]
           ,[AddDate],[Status],Note) VALUES ('" + ShiftmentNO +
                                  "',convert(date,'" + ShiftmentDate + "',103),'" + LoginBy + "',GETDATE(),'" + Status +
                                  "','" + Note + "')";
            command.ExecuteNonQuery();

            command.CommandText = @"SELECT top(1) [ID]  FROM [dbo].[ShiftmentAssigen] order by id desc";
            int MstID = Convert.ToInt32(command.ExecuteScalar());

            foreach (DataRow dr in dtShipmentSender.Rows)
            {
                command.CommandText = @"INSERT INTO [dbo].[ShiftmentSenderList]
                ([SenderID],[ShipmentID],[AddBy],[AddDate])
                 VALUES
                ('" + dr["SenderID"].ToString() + "','" + MstID + "','" + LoginBy + "',GETDATE())";
                command.ExecuteNonQuery();
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

    public static void UpdateShiftmentAssigInfo(string ID, string ShiftmentNO, string ShiftmentDate, string LoginBy,
        string Status, DataTable dtShipmentSender, string Note)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;

        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"UPDATE [ShiftmentAssigen]
            SET [ShiftmentNO] ='" + ShiftmentNO + "',[ShiftmentDate] =convert(date,'" + ShiftmentDate +
                                  "',103) ,[UpdateBy] ='" +
                                  LoginBy + "' ,[UpdateDate] =GETDATE(),[Status]='" + Status + "',Note='" + Note +
                                  "'  WHERE ID='" + ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE [dbo].[ShiftmentSenderList]
             SET [DeleteBy] ='" + LoginBy + "' ,[DeleteDate] =GETDATE() WHERE [ShipmentID]='" + ID + "'";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dtShipmentSender.Rows)
            {
                if (Convert.ToInt32(dr["ID"]) > 0)
                {
                    command.CommandText = @"UPDATE [dbo].[ShiftmentSenderList]
                  SET [DeleteBy] =NULL ,[DeleteDate] =NULL WHERE [ID]='" + dr["ID"].ToString() + "'";
                    command.ExecuteNonQuery();
                }
                else
                {
                    command.CommandText = @"INSERT INTO [dbo].[ShiftmentSenderList]
                ([SenderID],[ShipmentID],[AddBy],[AddDate])
                 VALUES
                ('" + dr["SenderID"].ToString() + "','" + ID + "','" + LoginBy + "',GETDATE())";
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

    public static void DeleteShiftmentAssigInfo(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        DataTable dtAllCtn = IdManager.GetShowDataTable(
            "SELECT t1.[ID],t1.[CartoonNo] ,t1.[ShiftmentID]  FROM [ShiftmentBoxingMst] t1 where t1.[ShiftmentID]='" +
            ID + "' ");

        DataTable dtAllShipItem = IdManager.GetShowDataTable(
            "SELECT [ID],[ItemsID] FROM [dbo].[ShiftmentItems] where [ShiftmentID]='" +
            ID + "' ");

        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"DELETE FROM [ShiftmentAssigen] WHERE ID='" + ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"DELETE FROM [ShiftmentSenderList] WHERE ShipmentID='" + ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"DELETE FROM [ShiftmentSenderListDetails] WHERE ShipmentID='" + ID + "'";
            command.ExecuteNonQuery();

            foreach (DataRow drRow in dtAllCtn.Rows)
            {
                command.CommandText = @"delete from [ShiftmentBoxingMst] WHERE ID='" + drRow["ID"].ToString() + "'";
                command.ExecuteNonQuery();
                command.CommandText = @"delete from ShiftmentBoxingItemsDtl where MasterID='" + drRow["ID"].ToString() + "'";
                command.ExecuteNonQuery();
                command.CommandText = @"delete from ShiftmentItemsBoxingColorSize where BoxingItemsID='" + drRow["ID"].ToString() + "'";
                command.ExecuteNonQuery();
                command.CommandText = @"delete from ShiftmentBoxingItemsQuantity where BoxingItemsID='" + drRow["ID"].ToString() + "'";
                command.ExecuteNonQuery();
                command.CommandText = @"delete from ShiftmentBoxingItemsImage where BoxingItemsID='" + drRow["ID"].ToString() + "'";
                command.ExecuteNonQuery();
            }
            foreach (DataRow dr in dtAllShipItem.Rows)
            {
                command.CommandText = @"DELETE FROM [ShiftmentItems]   WHERE ID='" + dr["ID"].ToString() + "' ";
                command.ExecuteNonQuery();
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

    public static DataTable GetShowShiftmentAssignDetails()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query =
            @"SELECT top(50) t1.[ID],t1.[ShiftmentNO], CONVERT(NVARCHAR,t1.[ShiftmentDate],103)ShiftmentDate,t1.[Status],t1.CustomerID,t3.ContactName,t4.ShiftmentNO AS[ParentShipmentNo],t1.ParentShiftmentNO
 FROM [ShiftmentAssigen] t1
 left join Customer t3 on t3.ID=t1.CustomerID 
 left join ShiftmentAssigen t4 on t4.ID=t1.ParentShiftmentNO
 order by ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ShiftmentAssigen");
        return dt;
    }

    public static DataTable GetShowShiftmentAssignOnSearch(string Shiftment)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query =
            @"SELECT [ID],[ShiftmentNO], CONVERT(NVARCHAR,[ShiftmentDate],103)ShiftmentDate FROM [ShiftmentAssigen] WHERE UPPER([ShiftmentNO]+' - '+CONVERT(NVARCHAR,[ShiftmentDate],103)) like upper('%" +
            Shiftment + "%')";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ShiftmentAssigen");
        return dt;
    }

    public DataTable GetShipmentSenderInfo(string ShipID, string ShiftmentNO, string SenderID, string Type)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        sqlCon.Open();
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = new SqlCommand("SP_Search_ShipmentInfo", sqlCon);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        if (string.IsNullOrEmpty(ShipID))
        {
            da.SelectCommand.Parameters.AddWithValue("@ID", null);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@ID", ShipID);
        }

        if (string.IsNullOrEmpty(ShiftmentNO))
        {
            da.SelectCommand.Parameters.AddWithValue("@ShiftmentNO", null);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@ShiftmentNO", ShiftmentNO);
        }

        if (string.IsNullOrEmpty(SenderID))
        {
            da.SelectCommand.Parameters.AddWithValue("@SenderID", null);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@SenderID", SenderID);
        }

        da.SelectCommand.Parameters.AddWithValue("@type", Type);
        da.SelectCommand.CommandTimeout = 500;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_Search_ShipmentInfo");
        DataTable dt = ds.Tables["SP_Search_ShipmentInfo"];
        return dt;
    }

    public static DataTable GetShowShiftmentSenderSearch(string ShipmentID, string ShipSender)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query =
            @"SELECT [ID],[ShipmentID],[SenderID],[SenderName],[ShiftmentNO],[SearchSender]  FROM [dbo].[View_Search_ShipmentSender] where ShipmentID='" +
            ShipmentID + "' and SearchSender='" + ShipSender + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ShiftmentAssigen");
        return dt;
    }

    public void ShipmentSenderCustomerDetaile(string ShipmentID, string SenderID, string LoginBy, DataTable dtDoc,string Name)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;

        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"UPDATE [dbo].[ShiftmentSenderListDetails]
            SET [DeleteBy] ='" + LoginBy + "' ,[DeleteDate] =GETDATE()  WHERE [SenderID]='" + SenderID +
                                  "' and [ShipmentID]='" + ShipmentID + "' ";
            command.ExecuteNonQuery();

            foreach (DataRow drRow in dtDoc.Rows)
            {
                if (Convert.ToInt32(drRow["ID"]) > 0)
                {
                    command.CommandText = @"UPDATE [dbo].[ShiftmentSenderListDetails]
                    SET [DeleteBy] =NULL ,[DeleteDate] =NULL  WHERE ID= " + drRow["ID"].ToString();
                    command.ExecuteNonQuery();
                }
                else
                {
                    command.CommandText = @"INSERT INTO [dbo].[ShiftmentSenderListDetails]
                    ([SenderID],[ShipmentID],[CustomerID],[AddBy],[AddDate],Name)
                    VALUES
                    ('" + SenderID + "','" + ShipmentID + "','" + drRow["CustomerID"].ToString() + "','" + LoginBy +
                                          "',GETDATE(),'" + drRow["SubShiperName"].ToString() + "')";
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

    public void DeleteShipmentSenderCustomerDetaile(string ShipmentID, string SenderID, string LoginBy, DataTable dtDoc)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;

        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"UPDATE [dbo].[ShiftmentSenderListDetails]
            SET [DeleteBy] ='" + LoginBy + "' ,[DeleteDate] =GETDATE()  WHERE [SenderID]='" + SenderID +
                                  "' and [ShipmentID]='" + ShipmentID + "' ";
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

    public static DataTable GetShowAllSender(string Sender)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query =
            @"SELECT [ID],[Name],[Search] FROM [dbo].[View_Search_Sender] where upper([Search])=upper('" + Sender + "') ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ShiftmentAssigen");
        return dt;
    }

    public DataTable GetShipmentSenderInfoSummery(string ID, string ShipmentID, string SenderID, string CustomerID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        sqlCon.Open();
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = new SqlCommand("SP_Search_ShipmentSennderSummery", sqlCon);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        if (string.IsNullOrEmpty(ID))
        {
            da.SelectCommand.Parameters.AddWithValue("@ID", null);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@ID", ID);
        }

        if (string.IsNullOrEmpty(ShipmentID))
        {
            da.SelectCommand.Parameters.AddWithValue("@ShipmentID", null);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@ShipmentID", ShipmentID);
        }
        if (string.IsNullOrEmpty(SenderID))
        {
            da.SelectCommand.Parameters.AddWithValue("@SenderID", null);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@SenderID", SenderID);
        }
        if (string.IsNullOrEmpty(CustomerID))
        {
            da.SelectCommand.Parameters.AddWithValue("@CustomerID", null);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@CustomerID", CustomerID);
        }
        
        da.SelectCommand.CommandTimeout = 500;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_Search_ShipmentSennderSummery");
        DataTable dt = ds.Tables["SP_Search_ShipmentSennderSummery"];
        return dt;
    }
}