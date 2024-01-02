using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using Delve;

/// <summary>
/// Summary description for AgencyManagers
/// </summary>
public class ShipmentSenderManagers
{
    public ShipmentSenderManagers()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public int Save(ShipmentSender aShipmentSender)
    {
        string ConnectionString = DataManager.OraConnString();
        string query = "Insert Into ShiftmentSender(Name,AddBy,AddDate) values('" + aShipmentSender.Name + "','" + aShipmentSender.LoginBy + "',GetDate())";
        int success = DataManager.SaveUpdateDelete(query, ConnectionString);
        return success;
    }
    public int Update(ShipmentSender aShipmentSender)
    {
        string ConnectionString = DataManager.OraConnString();
        string query = "Update ShiftmentSender set Name='" + aShipmentSender.Name + "',UpdateBy='" + aShipmentSender.LoginBy + "',UpdateDate=GETDATE() WHERE ID='" + aShipmentSender.Id + "'";
        int success = DataManager.SaveUpdateDelete(query, ConnectionString);
        return success;
    }
    public int Delete(ShipmentSender aShipmentSender)
    {
        string ConnectionString = DataManager.OraConnString();
        string query = "Update ShiftmentSender set DeleteBy='" + aShipmentSender.LoginBy + "',DeleteDate=GETDATE() WHERE ID='" + aShipmentSender.Id + "'";
        int success = DataManager.SaveUpdateDelete(query, ConnectionString);
        return success;
    }
  

    public DataTable GetAgencyDetails(string ID, string Name)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        sqlCon.Open();
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = new SqlCommand("SP_GetShiftmentSender", sqlCon);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        if (string.IsNullOrEmpty(ID))
        {
            da.SelectCommand.Parameters.AddWithValue("@ID", null);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@ID", ID);
        }
        if (string.IsNullOrEmpty(Name))
        {
            da.SelectCommand.Parameters.AddWithValue("@Name", null);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@Name", Name);
        }
        da.SelectCommand.CommandTimeout = 6000;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_GetShiftmentSender");
        DataTable dt = ds.Tables["SP_GetShiftmentSender"];
        return dt;
    }

  
}