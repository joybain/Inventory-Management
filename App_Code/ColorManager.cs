using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Delve;
using System.Data;

/// <summary>
/// Summary description for ColorManager
/// </summary>
public class ColorManager
{
	public ColorManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static void SaveColorInfo(string ColorID, string ColorName)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"INSERT INTO [ColorInfo]
           ([ColorName])
     VALUES
           ('" + ColorName + "')";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static DataTable GetColorDetails()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"SELECT [ID],[ColorName] FROM [ColorInfo]";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ColorInfo");
        return dt;
    }

    public static void DeleteColorInfo(string ID, string p_2)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"DELETE FROM [ColorInfo]  WHERE ID='" + ID + "'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void UpdateColorInfo(string ID, string ColorName)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"UPDATE [ColorInfo] SET [ColorName] ='" + ColorName + "'  WHERE ID='" + ID + "'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }
}