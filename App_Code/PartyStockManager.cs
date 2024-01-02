using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Delve;
using System.Data;

/// <summary>
/// Summary description for ItemPartyStockManager
/// </summary>
public class ItemPartyStockManager
{
	public ItemPartyStockManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static void SaveItemPartyStock(ItemPartyStockInfo aItemPartyStockInfo)
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

            command.CommandText = @"SELECT COUNT(*) FROM [ItemPartyStock] WHERE [PartyID]='" + aItemPartyStockInfo.PartyID + "' and [ItemsID]='"+aItemPartyStockInfo.ItemsID+"' ";
            int count = Convert.ToInt32(command.ExecuteScalar());
            if (count > 0)
            {
                command.CommandText = @"UPDATE [ItemPartyStock]
                SET  [Quantity] =[Quantity]+('" + aItemPartyStockInfo.Quantity + "')  WHERE [PartyID]='" + aItemPartyStockInfo.PartyID + "' and [ItemsID]='" + aItemPartyStockInfo.ItemsID + "'";
                command.ExecuteNonQuery();
            }
            else
            {
                command.CommandText = @"INSERT INTO [ItemPartyStock]
           ([PartyID],[ItemsID],[Quantity],[AddDate],[AddBy])
     VALUES
           ('" + aItemPartyStockInfo.PartyID + "','" + aItemPartyStockInfo.ItemsID + "','" + aItemPartyStockInfo.Quantity + "',GETDATE(),'" + aItemPartyStockInfo.LoginBy + "')";
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

    public static void DeleteItemPartyStock(ItemPartyStockInfo aItemPartyStockInfo)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"DELETE FROM [Royal_DB].[dbo].[ItemPartyStock] WHERE ID='" + aItemPartyStockInfo.ID + "'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public DataTable GetShowItemPartyStockDetails(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Brand");
        return dt;
    }

    public static void UpdateItemPartyStock(ItemPartyStockInfo aItemPartyStockInfo)
    {
        throw new NotImplementedException();
    }

    public static DataTable GetItemPartyStockDetails(string p)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"SELECT t1.[ID]
      ,t1.[PartyID]
      ,t1.[ItemsID]
      ,t1.[Quantity] 
      ,t2.Name
      ,t3.ContactName     
  FROM [ItemPartyStock] t1 inner join Item t2 on t2.ID=t1.ItemsID inner join Customer t3 on t3.ID=t1.PartyID";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPartyStock");
        return dt;
    }
}