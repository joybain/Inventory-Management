<%@ WebHandler Language="C#" Class="getImageUrl" %>

using System;
using System.Web;
using Delve;

public class getImageUrl : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        string book = "";
        if (context.Request.QueryString["img"] != null)
        {
            book = context.Request.QueryString["img"].ToString();
            byte[] img1 = Delve.GlBookManager.GetGlLogo(book);
            System.Drawing.Image img = DataManager.byteArrayToImage(img1);
            byte[] btp = DataManager.ConvertImageToByteArray(img, System.Drawing.Imaging.ImageFormat.Jpeg);
            context.Response.ContentType = "image/jpeg";
            context.Response.BinaryWrite(btp); 
        }               
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}