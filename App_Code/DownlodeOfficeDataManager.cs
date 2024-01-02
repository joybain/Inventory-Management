using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using Delve;

using DocumentFormat.OpenXml.Office2010.Word;

/// <summary>
/// Summary description for DownlodeOfficeDataManager
/// </summary>
public class DownlodeOfficeDataManager
{
	public DownlodeOfficeDataManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public  DataTable GetDownlodeHeadOfficeData()
    {
        string query = "";

        String connectionString = DataManager.OraConnString();
        //        string query = @"SELECT [ID],[BranchID],TransferFromBranchID,convert(nvarchar,[TransferDate],103)TransferDate,convert(datetime,[TransferDate],103)TransferDate2,[Remark],TransferType,Code ,ChallanNo    
        //            FROM " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[ItemStockTransferMst] WHERE [ReceivedBy] IS NULL AND [DeleteBy] IS NULL and BranchID=" + MainBranchID + " and (LocalUpload is null or LocalUpload!=1) ";

        //if (string.IsNullOrEmpty(ByDate) || string.IsNullOrEmpty(ToDate))
        //{
        query = @"SELECT * FROM " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[ExpenseHead] WHERE DeleteDate is null ";



        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ExpenseHead");
        return dt;
    }

    public int SaveExpenseHead(DataTable data, string LoginBy)
    {
        var connection=new  SqlConnection(DataManager.OraConnString());
        connection.Open();
        string query = "";
        int Value = 0;
        foreach (DataRow dr in data.Rows)
        {
            var IsExist = IsExistServerId(dr["Id"].ToString());
            if (IsExist)
            {
                query = "update ExpenseHead set Name='" + dr["Name"].ToString() + "',UpdateBy='" + LoginBy + "',UpdateDate=GetDate(),GL_COA_CODE='" + dr["GL_COA_CODE"].ToString() + "' where ServerMstId='" + dr["Id"].ToString() + "'";
            }
            else
            {
                 query = "Insert into ExpenseHead(Name,AddBy,AddDate,GL_COA_CODE,ServerMstId) values('" + dr["Name"].ToString() + "','" + LoginBy + "',GetDate(),'" + dr["GL_COA_CODE"].ToString() + "','" + dr["Id"].ToString() + "')"; 
            }

            var command=new SqlCommand(query,connection);
            Value = command.ExecuteNonQuery();

        }

        return Value;
    }

    private bool IsExistServerId(string ServerMstId)
    {

        bool rtValu = false;
        int value = IdManager.GetShowSingleValueIntNotParameter(
            "Count(Id)",
            "ExpenseHead where DeleteDate Is NULL and ServerMstId='" + ServerMstId + "' ");
        if (value>0)
        {
            rtValu = true;
        }

        return rtValu;

    }
}