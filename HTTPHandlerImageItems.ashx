<%@ WebHandler Language="C#" Class="HTTPHandlerImageItems" %>

using System;
//using System.Data.Metadata.Edm;
using System.Diagnostics.Eventing.Reader;
using System.Web;
using System.Data.SqlClient;
using Delve;
using System.Web.SessionState;
public class HTTPHandlerImageItems : IHttpHandler, IReadOnlySessionState
{

public void ProcessRequest(HttpContext context)
{
    SqlConnection connection = new SqlConnection(DataManager.OraConnString());

string imageid = context.Request.QueryString["ImID"];
    string ItemsID = context.Session["ItemID"].ToString();
connection.Open();
SqlCommand command = new SqlCommand("select Image from ItemImage where MstID='" + ItemsID + "' and ImageID='" + imageid+"'", connection);
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