using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;
using System.IO;

public partial class frmImageView : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string ID = Request.QueryString["ID"].ToString();
            string Name = Request.QueryString["ItemsName"].ToString();
            lblImage.Text = "Items Code & Name : " + Name;
            byte[] Image = IdManager.GetShowImage("ItemImage", "ID", "Item",ID);
            if (Image != null)
            {
                MemoryStream ms = new MemoryStream(Image);
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                Session["byt"] = Image;
                string base64String = Convert.ToBase64String(Image, 0, Image.Length);
                Image1.ImageUrl = "data:image/jpeg;base64," + base64String;
            }
            
        }
    }
}