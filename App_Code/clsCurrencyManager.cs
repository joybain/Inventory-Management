using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Delve;
using System.Data.SqlClient;
using System.Data;

/// <summary>
/// Summary description for clsCurrencyManager
/// </summary>
public class clsCurrencyManager
{
	public clsCurrencyManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    /************* For Home page save Reason Static**************/
    public static void SaveInfo(clsCurrency aclsCurrency)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = @"INSERT INTO [CurrencySet]
           ([CurrencyDate]
           ,[CurrencyRate]
           ,[AddBy]
           ,[AddDate])
     VALUES(convert(date,'" + aclsCurrency.Date + "',103),'" + aclsCurrency.Rate + "','" + aclsCurrency.LoginBy + "',GetDate())";

        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static DataTable GetCurrencyDetails()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT [ID]
      ,convert(date,[CurrencyDate],103) as CurrencyDate
      ,[CurrencyRate]
      ,[AddBy]
      ,[AddDate]
      ,[DeleteBy]
      ,[DeleteDate]
  FROM [CurrencySet] where DeleteBy IS NULL order by convert(date,[CurrencyDate],103) desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "CurrencySet");
        return dt;
    }

    public clsCurrency GetCurrencyInfo(string CurrencyId)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT [ID]
      ,convert(nvarchar,[CurrencyDate],103) as CurrencyDate
      ,[CurrencyRate]
  FROM [CurrencySet] where "+CurrencyId+" and DeleteBy IS NULL";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "CurrencySet");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new clsCurrency(dt.Rows[0]);
    }


    /***************** Currency Form Reason Void **************/
    public void SaveInformation(clsCurrency aclsCurrency)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = @"INSERT INTO [CurrencySet]
           ([CurrencyDate]
           ,[CurrencyRate]
           ,[AddBy]
           ,[AddDate])
     VALUES(convert(date,'" + aclsCurrency.Date + "',103),'" + aclsCurrency.Rate + "','" + aclsCurrency.LoginBy + "',GetDate())";

        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public void UpdateUniversityInfo(clsCurrency aclsCurrency)
    {
        string con = DataManager.OraConnString();
        string query = @"UPDATE [CurrencySet]
   SET [CurrencyDate] = convert(date,'" + aclsCurrency.Date + "',103),[CurrencyRate] = '" + aclsCurrency.Rate + "',[UpdateBy] = '" + aclsCurrency.LoginBy + "',[UpdateDate] = GetDate() WHERE ID='" + aclsCurrency.CurrencyId + "'";
        DataManager.ExecuteNonQuery(con, query);

        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlCommand da = new SqlCommand();
        connection.Open();
        da.CommandText = "SP_CurrencyUpdate";
        da.CommandType = CommandType.StoredProcedure;
        da.Connection = connection;
        da.Parameters.AddWithValue("@CurrencyDate", DataManager.DateEncode(aclsCurrency.Date));
        da.ExecuteNonQuery();
        connection.Close();
    }

    public void DeleteInfo(clsCurrency aclsCurrency)
    {
        string con = DataManager.OraConnString();
        string query = @"UPDATE [CurrencySet]
   SET [DeleteBy] = '" + aclsCurrency.LoginBy + "',[DeleteDate] = GetDate() WHERE ID='" + aclsCurrency.CurrencyId + "'";
        DataManager.ExecuteNonQuery(con, query);
    }

    public DataTable GetCurrencyInformation()
    {
        string con = DataManager.OraConnString();

        string query = @"SELECT top(100)[ID]
      ,convert(nvarchar,[CurrencyDate],103) as CurrencyDate
      ,[CurrencyRate]
  FROM [CurrencySet] where DeleteBy IS NULL order by convert(date,[CurrencyDate],103) desc";

        DataTable dt = DataManager.ExecuteQuery(con, query, "CurrencySet");
        return dt;
    }

    public void getUpdateStatus(int Status)
    {
        string con = DataManager.OraConnString();
        string query = @"UPDATE [dbo].[FixGlCoaCode] SET [AutoAuthoriz] =" + Status + " ";
        DataManager.ExecuteNonQuery(con, query);
    }
}