<%@ WebHandler Language="C#" Class="imgHandler" %>

using System;
using System.Web;
using System.Data;
using Delve;

public class imgHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) 
    {
        
        string book;
        if (context.Request.QueryString["ImID"] != null)
        {
            book = context.Request.QueryString["ImID"].ToString();
        }
        else
        {
            throw new ArgumentException("No parameter specified");
        }
        DataTable dt = DataManager.getLogo(book);
        if (dt.Rows.Count > 0)
        {
            if (dt.Rows[0][0] != DBNull.Value)
            {
                System.Drawing.Image img = DataManager.byteArrayToImage((byte[])dt.Rows[0][0]);
                byte[] btp = DataManager.ConvertImageToByteArray(img, System.Drawing.Imaging.ImageFormat.Jpeg);
                context.Response.ContentType = "image/jpeg";
                context.Response.BinaryWrite(btp);
            }
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}