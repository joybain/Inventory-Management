using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Delve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using Delve;

/// <summary>
/// Summary description for MenuManager
/// </summary>
public class MenuManager
{
	public MenuManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int Save(MenuModel _menue)
    {
       SqlConnection connection=new SqlConnection(DataManager.OraConnString());
        connection.Open();
        string query = "insert into UTL_MenuName (Name,Path,AddBy,AddDate) values('"+_menue.Name+"','"+_menue.Path+"','"+_menue.LoginBy+"',GetDate())";
        var command = new SqlCommand(query,connection);
        int save = command.ExecuteNonQuery();
        return save;

    }

    public int Update(MenuModel menu)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        connection.Open();
        string query = "Update UTL_MenuName set Name='"+menu.Name+"',Path='"+menu.Path+"',UpdateBy='"+menu.LoginBy+"',UpdateDate=GetDate() where Id='"+menu.Id+"'";
        var command = new SqlCommand(query, connection);
        int save = command.ExecuteNonQuery();
        return save;
    }
    public int Delete(MenuModel menu)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        connection.Open();
        string query = "Update UTL_MenuName set DeleteBy='" + menu.LoginBy + "',DeleteDate=GetDate() where Id='" + menu.Id + "'";
        var command = new SqlCommand(query, connection);
        int save = command.ExecuteNonQuery();
        return save;
    }

    public DataTable GetData(string Id, string Name, string Path,string type)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        sqlCon.Open();
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = new SqlCommand("SP_MenuSetting", sqlCon);
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
        da.SelectCommand.Parameters.AddWithValue("@TypeID", type);        
        da.SelectCommand.CommandTimeout = 500;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_MenuSetting");
        DataTable dt = ds.Tables["SP_MenuSetting"];
        return dt;
    }
}