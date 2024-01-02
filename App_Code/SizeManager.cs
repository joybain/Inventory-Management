using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Delve;
using System.Data.SqlClient;

/// <summary>
/// Summary description for SizeManager
/// </summary>
public class SizeManager
{
	public SizeManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable GetShowSizeDetails()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"SELECT [ID],[SizeName] FROM [SizeInfo] where id!=0";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SizeInfo");
        return dt;
    }
    public static DataTable GetShowDeptDetails()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"SELECT * FROM [Depertment_Type] WHERE [Delete_By] IS NULL order by ID  ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Depertment_Type");
        return dt;
    }
    public static void SaveColorInfo(string ID, string SizeName)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"INSERT INTO [SizeInfo]
           ([SizeName])
     VALUES
           ('" + SizeName + "')";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void UpdateColorInfo(string ID, string SizeName)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"UPDATE [SizeInfo] SET [SizeName] ='" + SizeName + "'  WHERE ID='" + ID + "'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void DeleteColorInfo(string ID, string SizeName)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"DELETE FROM [SizeInfo]  WHERE ID='" + ID + "'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void UpdateDeptInfo(string ID, string Name, string LoginBy)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"UPDATE [Depertment_Type] SET [Dept_Name] ='" + Name + "' ,[Update_By]='" + LoginBy +
                       "',[Update_Date]=GETDATE() WHERE ID='" + ID + "'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }
    public static void SaveDeptInfo(string Name, string UserID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string ParameterField = "";
        string ParameterValue = "";
        string Code22 = IdManager.GetitemDEscSerial("", "ID", "Depertment_Type");
        string query = @"INSERT INTO Depertment_Type
           ([Dept_Name],[Entry_By],[Entry_Date]) VALUES ('" + Name + "','" + UserID +
                       "',GETDATE())";
        DataManager.ExecuteNonQuery(connectionString, query);
    }   

    public static void DeleteDeptInfo(string ID, string LoginBy)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"UPDATE [Depertment_Type] SET [Delete_By]='" + LoginBy +
                       "',[Delete_Date]=GETDATE() WHERE ID='" + ID + "'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }
}