using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using Delve;

/// <summary>
/// Summary description for BranchSetupManager
/// </summary>
public class BranchSetupManager
{
	public BranchSetupManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int Save(BranchModel branch)
    {
        var connectionString = DataManager.OraConnString();
        var connection=new SqlConnection(connectionString);
        connection.Open();
        string query =
            "insert into BranchInfo (BranchName,ShortName,Address1,Address2,Phone,Mobile,EMail,VatRegNo,Status) values ('"
            + branch.BranchName + "','" + branch.ShortName + "','" + branch.Address1 + "','" + branch.Address2 + "','"
            + branch.Phone + "','" + branch.Mobile + "','" + branch.EMail + "','" + branch.VatRegNo + "','"
            + branch.Status + "')";
        var command=new SqlCommand(query,connection);
        int save = command.ExecuteNonQuery();
        return save;
    }

    public DataTable GetBranchData(string Id)
    {
        string query = "";
        if (string.IsNullOrEmpty(Id))
        {
            query = "select * from BranchInfo where DeleteDate is null";
        }
        else
        {
            query = "select * from BranchInfo where DeleteDate is null and Id='"+Id+"'";
        }
       
        var data = DataManager.ExecuteQuery(DataManager.OraConnString(), query, "BranchInfo");
        return data;
    }

    public DataTable GetBranchData()
    {

        string query = "select 0 as Id,'' as BranchName union all (select Id,BranchName from BranchInfo where DeleteDate is null)";
        

        var data = DataManager.ExecuteQuery(DataManager.OraConnString(), query, "BranchInfo");
        return data;
    }

    public int Update(BranchModel _branch)
    {
        var connectionString = DataManager.OraConnString();
        var connection = new SqlConnection(connectionString);
        connection.Open();
        string query = "update BranchInfo set BranchName='"+_branch.BranchName+"' ,ShortName='"+_branch.ShortName+"',Address1='"+_branch.Address1+"',Address2='"+_branch.Address2+"',Phone='"+_branch.Phone+"',Mobile='"+_branch.Mobile+"',EMail='"+_branch.EMail+"',VatRegNo='"+_branch.VatRegNo+"',Status='"+_branch.Status+"' where ID='"+_branch.Id+"'";
        var command = new SqlCommand(query, connection);
        int save = command.ExecuteNonQuery();
        return save;  
    }

    public int Delete(string Id,string LoginBy)
    {
        var connectionString = DataManager.OraConnString();
        var connection = new SqlConnection(connectionString);
        connection.Open();
        string query = "update BranchInfo set DeleteBy='" + LoginBy + "' ,DeleteDate=GetDate() where ID='" + Id + "'";
        var command = new SqlCommand(query, connection);
        int delete = command.ExecuteNonQuery();
        return delete;  
    }
}