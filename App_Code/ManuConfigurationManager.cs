using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Delve;
using DocumentFormat.OpenXml.Office.CustomUI;

/// <summary>
/// Summary description for ConfigurationManager
/// </summary>
public class ManuConfigurationManager
{
	public ManuConfigurationManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}


    public int Save(ManuConfigurationModel _menue)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        connection.Open();
        string query = "insert into UTL_MenuNameDetails (MenuID,ParentMenuId,Name,Path,Priority,AddBy,AddDate) values('" + _menue.MenuId + "','"+_menue.ParentMenuId+"','" + _menue.Name + "','" + _menue.Path + "','" + _menue.Priority + "','" + _menue.LoginBy + "',GetDate())";
        var command = new SqlCommand(query, connection);
        int save = command.ExecuteNonQuery();
        return save;

    }

    public int Update(ManuConfigurationModel menu)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        connection.Open();
        string query = "Update UTL_MenuNameDetails set MenuID='" + menu.MenuId + "',ParentMenuId='"+menu.ParentMenuId+"', Name='" + menu.Name + "',Path='" + menu.Path + "',Priority='" + menu.Priority + "',UpdateBy='" + menu.LoginBy + "',UpdateDate=GetDate() where Id='" + menu.Id + "'";
        var command = new SqlCommand(query, connection);
        int save = command.ExecuteNonQuery();
        return save;
    }
    public int Delete(ManuConfigurationModel menu)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        connection.Open();
        string query = "Update UTL_MenuNameDetails set DeleteBy='" + menu.LoginBy + "',DeleteDate=GetDate() where Id='" + menu.Id + "'";
        var command = new SqlCommand(query, connection);
        int save = command.ExecuteNonQuery();
        return save;
    }

    public DataTable GetData(string Id, string Name, string Path,string MenuId,string Priority, string type)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        sqlCon.Open();
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = new SqlCommand("SP_ManuConfiguration", sqlCon);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        if (!string.IsNullOrEmpty(Id))
        {
            da.SelectCommand.Parameters.AddWithValue("@Id", Id);

        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@Id", null);

        }
        if (!string.IsNullOrEmpty(Name))
        {
            da.SelectCommand.Parameters.AddWithValue("@Name", Name);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@Name", null);
        }
        if (!string.IsNullOrEmpty(Path))
        {
            da.SelectCommand.Parameters.AddWithValue("@Path", Path);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@Path", null);
        }
        if (!string.IsNullOrEmpty(MenuId))
        {
            da.SelectCommand.Parameters.AddWithValue("@MenuId",MenuId);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@MenuId", null);
        }
        if (!string.IsNullOrEmpty(Priority))
        {
            da.SelectCommand.Parameters.AddWithValue("@Priority", Priority);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@Priority", null);
        }

        da.SelectCommand.Parameters.AddWithValue("@TypeID", type);
        da.SelectCommand.CommandTimeout = 500;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_ManuConfiguration");
        DataTable dt = ds.Tables["SP_ManuConfiguration"];
        return dt;
    }
}