using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Delve;

/// <summary>
/// Summary description for TaxManager
/// </summary>
public class TaxManager
{
	public TaxManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable GetShowTaxInformation()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"SELECT [ID] AS TaxCode ,[Name] AS TaxType ,[Rate] AS TaxRate ,[Active] AS [check] FROM [TaxCategory]";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "TaxCategory");
        return dt;
    }

    public static void CreateTax(string Code, string Type, string Rate, string Chk,string Login)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"INSERT INTO [TaxCategory]
           ([Name],[Rate],[Active],[CreatedBy],[CreatedDate])
     VALUES
           ( '" + Type + "','" + Rate + "' ,'" + Chk + "' , '" + Login + "', GETDATE())";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void DeleteTax(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"DELETE FROM [TaxCategory] WHERE ID='"+ID+"'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void UpdateTax(string Code, string Type, string Rate, string Chk, string LoginBy)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"UPDATE [TaxCategory] SET [Name] ='" + Type + "',[Rate] ='" + Rate + "' ,[Active] ='" + Chk + "' ,[ModifiedBy] ='" + LoginBy + "' ,[ModifiedDate] =GETDATE()  WHERE ID='" + Code + "'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }
}