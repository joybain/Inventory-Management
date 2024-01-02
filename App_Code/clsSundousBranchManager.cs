using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Delve;
using System.Data.SqlClient;
using System.Data;

/// <summary>
/// Summary description for clsSundousBranchManager
/// </summary>
public class clsSundousBranchManager
{
	public clsSundousBranchManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public DataTable GetBranchInfo()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT ID 
	  ,[BranchName]
      ,[Address]
      ,[MobileNumber]
      ,[Email]
      FROM [SundousBranchSetup] where DeleteBy Is Null ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SundousBranchSetup");
        return dt;
    }

    public clsSundousBranch GetBranchInfo(string BranchId)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT ID
      ,[BranchName]
      ,[Address]
      ,[MobileNumber]
      ,[Email]
      FROM [SundousBranchSetup] where ID='" + BranchId + "' and DeleteBy IS NULL";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SundousBranchSetup");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new clsSundousBranch(dt.Rows[0]);
    }

    public void SaveInformation(clsSundousBranch aclsSundousBranch)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = @"INSERT INTO [SundousBranchSetup]
           ([BranchName]
           ,[Address]
           ,[MobileNumber]
           ,[Email]
           ,[AddBy]
           ,[AddDate])
            Values('" + aclsSundousBranch.BranchName + "','" + aclsSundousBranch.Address + "','" + aclsSundousBranch.Mobile + "','" + aclsSundousBranch.Email + "','" + aclsSundousBranch.LoginBy + "',GetDate())";

        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public void UpdateBranchInfo(clsSundousBranch aclsSundousBranch)
    {
        string con = DataManager.OraConnString();
        string query = @"UPDATE [SundousBranchSetup]
   SET [BranchName] = '" + aclsSundousBranch.BranchName + "',[Address] = '" + aclsSundousBranch.Address + "',[MobileNumber] = '" + aclsSundousBranch.Mobile + "',[Email] = '" + aclsSundousBranch.Email + "',[UpdateBy] = '" + aclsSundousBranch.LoginBy + "',[UpdateDate] = GetDate() WHERE ID='"+aclsSundousBranch.BranchId+"'";

        DataManager.ExecuteNonQuery(con, query);
    }

    public void DeleteInfo(clsSundousBranch aclsSundousBranch)
    {
        string con = DataManager.OraConnString();
        string query = @"UPDATE [SundousBranchSetup]
   SET [DeleteBy] = '" + aclsSundousBranch.LoginBy + "',[DeleteDate] = GetDate() WHERE ID='" + aclsSundousBranch.BranchId + "'";
        DataManager.ExecuteNonQuery(con, query);
    }

    
}