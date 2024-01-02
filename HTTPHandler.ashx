<%@ WebHandler Language="C#" Class="HTTPHandler" %>

using System;
using System.Web;
using System.Data.SqlClient;
using Delve;
public class HTTPHandler : IHttpHandler {

public void ProcessRequest(HttpContext context)
{
    SqlConnection connection = new SqlConnection(DataManager.OraConnString());

string imageid = context.Request.QueryString["ImID"];

connection.Open();
SqlCommand command = new SqlCommand("select Image from TemporaryImage where ImageID=" + imageid, connection);
SqlDataReader dr = command.ExecuteReader();
dr.Read();
context.Response.BinaryWrite((Byte[])dr[0]);
connection.Close();
context.Response.End();
}
    
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}