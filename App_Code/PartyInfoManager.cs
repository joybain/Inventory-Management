using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Delve;
using System.Data;

/// <summary>
/// Summary description for PartyInfoManager
/// </summary>
public class PartyInfoManager
{
   
	public PartyInfoManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public void SavePartyInfo(PartyInfo aPartyInfo)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        connection.Open();
        string insertQuery = @"INSERT INTO [PartyInfo]
           ([PartyCode]
           ,[PartyName]
           ,[Address]
           ,[Mobile]
           ,[Phone]
           ,[Email]
           ,[AddBy]
           ,[AddDate],Gl_CoaCode,CountryID)
     VALUES
           ('" + aPartyInfo.PartyCode + "','" + aPartyInfo.PartyName + "','" + aPartyInfo.Address + "','" + aPartyInfo.Mobile + "','" + aPartyInfo.Phone + "','" + aPartyInfo.Email + "','" + aPartyInfo.LoginBy + "',GETDATE(),'" + aPartyInfo.GlCoa + "','" + aPartyInfo.Country + "')";
        SqlCommand cmd = new SqlCommand(insertQuery,connection);
        cmd.ExecuteNonQuery();
        connection.Close();
    }
    public void UpadatePartyInfo(PartyInfo aPartyInfo)
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

            command.CommandText = @"UPDATE [PartyInfo]
  SET [PartyCode] ='" + aPartyInfo.PartyCode + "' ,[PartyName] ='" + aPartyInfo.PartyName + "',[Address] ='" + aPartyInfo.Address + "',[Mobile] ='" + aPartyInfo.Mobile + "',[Phone] = '" + aPartyInfo.Phone + "',[Email] ='" + aPartyInfo.Email + "' ,[UpdateBy] ='',[UpdateDate]=GETDATE(),CountryID='" + aPartyInfo.Country + "'  WHERE ID='" + aPartyInfo.ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE [GL_SEG_COA] SET [SEG_COA_DESC] ='Accounts Receivable from-Party-" + aPartyInfo.PartyName + "'  WHERE [SEG_COA_CODE]='" + aPartyInfo.GlCoa + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE [GL_COA] SET [COA_DESC] ='" + aPartyInfo.GlCoaDesc + "' where [GL_COA_CODE]='1-" + aPartyInfo.GlCoa + "'";
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

    public void DeletePartyInfo(PartyInfo aPartyInfo)
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

            command.CommandText = @"Delete from PartyInfo WHERE ID='" + aPartyInfo.ID + "' ";
            command.ExecuteNonQuery();

            command.CommandText = @"Delete from [GL_SEG_COA] WHERE [SEG_COA_CODE]='" + aPartyInfo.GlCoa + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"Delete from [GL_COA] where [GL_COA_CODE]='1-" + aPartyInfo.GlCoa + "'";
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

    public object getPartyInfoDetails()
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        
        string Query = @"SELECT [ID]
      ,[PartyCode]
      ,[PartyName]
      ,[Address]
      ,[Mobile]
      ,[Phone]
      ,[Email]     
  FROM [PartyInfo] where ID != 1 ";
        SqlDataAdapter da = new SqlDataAdapter(Query, connection);
        DataSet ds = new DataSet();
        da.Fill(ds, "PartyInfo");
        DataTable td = ds.Tables["PartyInfo"];
        connection.Close();
        return td;
      
    }

    public DataTable ShowPartyInfo(string ID)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());

        string Query = @"SELECT [ID] ,[PartyCode] ,[PartyName]   ,[Address] ,[Mobile]
      ,[Phone]
      ,[Email],Gl_CoaCode,CountryID  
  FROM [PartyInfo] WHERE ID='" + ID + "'";
        SqlDataAdapter da = new SqlDataAdapter(Query, connection);
        DataSet ds = new DataSet();
        da.Fill(ds, "PartyInfo");
        DataTable td = ds.Tables["PartyInfo"];
        connection.Close();
        return td;
    }

    public int GetPartyInfoCount(string code)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        connection.Open();
        string selectQuery = @"SELECT COUNT (*) FROM [PartyInfo] where ID='" + code + "'";
        SqlCommand command = new SqlCommand(selectQuery, connection);
        int count = Convert.ToInt32(command.ExecuteScalar());
        connection.Close();
        return count;
    }

    public static DataTable GetParty(string Party)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        string Query = @"SELECT [ID] ,[PartyCode] ,[PartyName] ,[Address] ,[Mobile] ,[Phone] ,[Email],Gl_CoaCode FROM [PartyInfo] WHERE  upper([PartyCode]+' - '+[PartyName]) like upper('%" + Party + "%') ";
        SqlDataAdapter da = new SqlDataAdapter(Query, connection);
        DataSet ds = new DataSet();
        da.Fill(ds, "PartyInfo");
        DataTable td = ds.Tables["PartyInfo"];
        connection.Close();
        return td;
    }
    public static DataTable getShowPartyPaymentSupperyInReport(string PartyID, string FromDate, string ToDate)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlDataAdapter da = new SqlDataAdapter();
        connection.Open();
        da.SelectCommand = new SqlCommand("SP_PartyPaymentSummery", connection);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        if (PartyID != "0")
        {
            da.SelectCommand.Parameters.AddWithValue("@partyID", PartyID);
            da.SelectCommand.Parameters.AddWithValue("@StartDate", DataManager.DateEncode(FromDate));
            da.SelectCommand.Parameters.AddWithValue("@EndDate", DataManager.DateEncode(ToDate));
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@partyID", null);
            da.SelectCommand.Parameters.AddWithValue("@StartDate",DataManager.DateEncode(FromDate));
            da.SelectCommand.Parameters.AddWithValue("@EndDate", DataManager.DateEncode(ToDate));
        }
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_ShiftmentCartoonItemsQuantityPartyWise");
        DataTable table = ds.Tables["SP_ShiftmentCartoonItemsQuantityPartyWise"];
        connection.Close();
        return table;
    }
}