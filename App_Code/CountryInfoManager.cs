using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Delve;

/// <summary>
/// Summary description for CountryInfoManager
/// </summary>
public class CountryInfoManager
{
	public CountryInfoManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}


    public int Save(CountryInfoModel country)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        connection.Open();
        string query = "insert into COUNTRY_INFO (COUNTRY_DESC,Country_ABVR) values('" + country.Name + "','" + country.ShortName + "')";
        var command = new SqlCommand(query, connection);
        int save = command.ExecuteNonQuery();
        return save;

    }

    public int Update(CountryInfoModel country)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        connection.Open();
        string query = "Update COUNTRY_INFO set COUNTRY_DESC='" + country.Name + "',Country_ABVR='" + country.ShortName + "' where COUNTRY_Code='" + country.Id + "'";
        var command = new SqlCommand(query, connection);
        int save = command.ExecuteNonQuery();
        return save;
    }
    public int Delete(CountryInfoModel country)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        connection.Open();
        string query = "delete from COUNTRY_INFO where COUNTRY_Code='" + country.Id + "'";
        var command = new SqlCommand(query, connection);
        int save = command.ExecuteNonQuery();
        return save;
    }

    public DataTable GetData(string Id, string Name, string Path, string type)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        sqlCon.Open();
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = new SqlCommand("SP_CountryInfo", sqlCon);
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
            da.SelectCommand.Parameters.AddWithValue("@ShortName", Path);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@ShortName", null);
        }
        da.SelectCommand.Parameters.AddWithValue("@TypeID", type);
        da.SelectCommand.CommandTimeout = 500;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_CountryInfo");
        DataTable dt = ds.Tables["SP_CountryInfo"];
        return dt;
    }
}