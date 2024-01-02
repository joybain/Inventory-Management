using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using System.Net;

public partial class Default4 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        //string a = Request.QueryString["file"].ToString();
        string a = "";
        string FilePath = "";
        
        if (a == "Report_GISApplication_Final" | a == "User_Manual_GISApp")
        {
            Response.ContentType = "Application/vnd.ms-word";
            //Get the physical path to the file.
            FilePath = MapPath("Result/" + a + ".doc");
            FileInfo file = new FileInfo(FilePath);
            Response.AddHeader("Content-Disposition", "inline; filename=\"" + file.FullName + "\"");
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.TransmitFile(file.FullName);
        }
        else
        {
            /*
            //Set the appropriate ContentType.
            Response.ContentType = "Application/pdf";
            //Response.AppendHeader("Content-Disposition", "attachment; filename=files.pdf");
            //Get the physical path to the file.
            FilePath = MapPath("Resources/" + a + ".pdf");
            Response.WriteFile(FilePath);
            Response.End();
            */
            /*
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=files.pdf");
            Response.TransmitFile(Server.MapPath("Resources/" + a + ".pdf"));
            Response.End();
            */

            //WebClient User = new WebClient();
            //Byte[] FileBuffer = User.DownloadData(Server.MapPath("Result/" + a + ".pdf"));
            byte[] FileBuffer = (byte[])Session["rptbyte"];
            if (FileBuffer != null)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", FileBuffer.Length.ToString());
                Response.BinaryWrite(FileBuffer);
            }
             
        }
    }
}
