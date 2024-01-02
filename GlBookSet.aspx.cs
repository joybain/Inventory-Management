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
//using AMBs;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Delve;
using Delve;

public partial class GlBookSet : System.Web.UI.Page
{
    public static Permis per;
    public byte[] ImageLogo;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null)
        {
            if (Session.SessionID != "" | Session.SessionID != null)
            {
                clsSession ses = clsSessionManager.getSession(Session.SessionID);
                if (ses != null)
                {
                    Session["user"] = ses.UserId;
                    Session["book"] = "AMB";
                    string connectionString = DataManager.OraConnString();
                    SqlDataReader dReader;
                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = connectionString;
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "Select user_grp,[description],UserType,case when UserType=1 then 'Bangladesh' else 'Philippine' end AS[LoginCountry] from utl_userinfo where upper(user_name)=upper('" +
                        Session["user"].ToString().ToUpper() + "') and status='A'";
                    conn.Open();
                    dReader = cmd.ExecuteReader();
                    string wnot = "", userType = "";
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            Session["userlevel"] = int.Parse(dReader["user_grp"].ToString());
                            //Session["dept"] = dReader["dept"].ToString();
                            wnot = "Welcome " + dReader["description"].ToString();
                            Session["LoginCountry"] = dReader["LoginCountry"].ToString();
                            userType = dReader["UserType"].ToString();
                        }
                        Session["wnote"] = wnot;
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        //cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type from gl_set_of_books where book_name='" + Session["book"].ToString() + "' ";
                        if (Convert.ToInt32(userType) == 2)
                        {

                            Session["bookMAN"] = "MAN";
                        }
                        else
                        {
                            Session["bookMAN"] = Session["book"].ToString();
                        }
                        cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type,ShotName from gl_set_of_books where book_name='" + Session["bookMAN"] + "' ";

                        if (dReader.IsClosed == false)
                        {
                            dReader.Close();
                        }
                        dReader = cmd.ExecuteReader();
                        if (dReader.HasRows == true)
                        {
                            while (dReader.Read())
                            {
                                Session["septype"] = dReader["separator_type"].ToString();
                                Session["org"] = dReader["book_desc"].ToString();
                                Session["add1"] = dReader["company_address1"].ToString();
                                Session["add2"] = dReader["company_address2"].ToString();
                            }
                        }
                    }
                    dReader.Close();
                    conn.Close();
                }
            }
        }
        try
        {
            string pageName = DataManager.GetCurrentPageName();
            string modid = PermisManager.getModuleId(pageName);
            per = PermisManager.getUsrPermis(Session["user"].ToString().Trim().ToUpper(), modid);
            if (per != null && per.AllowView == "Y")
            {
                ((Label)Page.Master.FindControl("lblLogin")).Text = Session["wnote"].ToString();
                ((Label)Page.Master.FindControl("lblCountryName")).Text = Session["LoginCountry"].ToString();
                ((LinkButton)Page.Master.FindControl("lbLogout")).Visible = true;
            }
            else
            {
                Response.Redirect("Home.aspx?sid=sam");
            }
        }
        catch
        {
            Response.Redirect("Default.aspx?sid=sam");
        }

        if (!Page.IsPostBack)
        {
            string criteria = "";
            dgGlBook.DataSource = GlBookManager.GetGlBooks(criteria);
            dgGlBook.DataBind();
        }

    }
    private void clearFields()
    {
        txtBookName.Text = "";
        txtBookDesc.Text = "";
        ddlBookStatus.SelectedIndex = -1;
        txtSeparatorType.Text = "";
        txtCompanyAddress1.Text = "";
        txtCompanyAddress2.Text = "";
        txtRetdEarnAcc.Text = "";
        txtTaxNo.Text = "";
        txtPhone.Text = "";
        txtFax.Text = "";
        txtUrl.Text = "";
        txtBankCode.Text = "";
        txtCashCode.Text = "";
        imgLogo.ImageUrl = "";
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (txtBookName.Text != String.Empty)
        {
            


                GlBook book = GlBookManager.getBook(txtBookName.Text);
                if (book != null)
                {
                   
                    if (per.AllowEdit == "Y" && imgUpload.HasFile)
                    {
                        int length = imgUpload.PostedFile.ContentLength;
                        byte[] imgbyte = new byte[length];
                        HttpPostedFile img = imgUpload.PostedFile;
                        string date = System.DateTime.UtcNow.ToLocalTime().ToString();
                        Stream fs = imgUpload.PostedFile.InputStream;
                        BinaryReader br = new BinaryReader(fs);
                        Byte[] bytesLogo = br.ReadBytes((Int32)fs.Length);

                        book.BookDesc = txtBookDesc.Text.Replace("'", "’");
                        book.BookStatus = ddlBookStatus.SelectedValue;
                        book.SeparatorType = txtSeparatorType.Text;
                        book.CompanyAddress1 = txtCompanyAddress1.Text;
                        book.CompanyAddress2 = txtCompanyAddress2.Text;
                        book.RetdEarnAcc = txtRetdEarnAcc.Text;
                        book.TaxNo = txtTaxNo.Text;
                        book.Phone = txtPhone.Text;
                        book.Fax = txtFax.Text;
                        book.Url = txtUrl.Text;
                        book.BankCode = txtBankCode.Text;
                        book.CashCode = txtCashCode.Text;
                        book.logo = bytesLogo;
                       // book.logo = (byte[])Session["imglogo"];
                        //book.logo = (byte[])ViewState["imglogo"];
                        GlBookManager.UpdateGlBook(book);
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record updated successfully!!');", true);

                    }
                    else if (per.AllowEdit == "Y")
                    {
                        book.BookDesc = txtBookDesc.Text.Replace("'", "’");
                        book.BookStatus = ddlBookStatus.SelectedValue;
                        book.SeparatorType = txtSeparatorType.Text;
                        book.CompanyAddress1 = txtCompanyAddress1.Text;
                        book.CompanyAddress2 = txtCompanyAddress2.Text;
                        book.RetdEarnAcc = txtRetdEarnAcc.Text;
                        book.TaxNo = txtTaxNo.Text;
                        book.Phone = txtPhone.Text;
                        book.Fax = txtFax.Text;
                        book.Url = txtUrl.Text;
                        book.BankCode = txtBankCode.Text;
                        book.CashCode = txtCashCode.Text;
                        //book.logo = bytesLogo;
                        book.logo = (byte[])Session["imglogo"];
                        //book.logo = (byte[])ViewState["imglogo"];
                        GlBookManager.UpdateGlBook(book);
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record updated successfully!!');", true);
                        
                    }
                    else {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You have no permission to edit/update these data!!');", true);
                    }
                }
                else
                {
                    if (per.AllowAdd == "Y" && imgUpload.HasFile)
                    {

                        int length = imgUpload.PostedFile.ContentLength;
                        byte[] imgbyte = new byte[length];
                        HttpPostedFile img = imgUpload.PostedFile;
                        string date = System.DateTime.UtcNow.ToLocalTime().ToString();
                        Stream fs = imgUpload.PostedFile.InputStream;
                        BinaryReader br = new BinaryReader(fs);
                        Byte[] bytesLogo = br.ReadBytes((Int32)fs.Length);

                        book = new GlBook();
                        book.BookName = txtBookName.Text;
                        book.BookDesc = txtBookDesc.Text.Replace("'", "’");
                        book.BookStatus = ddlBookStatus.SelectedValue;
                        book.SeparatorType = txtSeparatorType.Text;
                        book.CompanyAddress1 = txtCompanyAddress1.Text;
                        book.CompanyAddress2 = txtCompanyAddress2.Text;
                        book.RetdEarnAcc = txtRetdEarnAcc.Text;
                        book.TaxNo = txtTaxNo.Text;
                        book.Phone = txtPhone.Text;
                        book.Fax = txtFax.Text;
                        book.Url = txtUrl.Text;
                        book.BankCode = txtBankCode.Text;
                        book.CashCode = txtCashCode.Text;
                        book.logo = bytesLogo;
                        //book.logo = (byte[])Session["imglogo"];
                       // book.logo = (byte[])ViewState["imglogo"];
                        GlBookManager.CreateGlBook(book);
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record created successfully!!');", true);
                    }
                    else if (per.AllowAdd == "Y")
                    {
                        book = new GlBook();
                        book.BookName = txtBookName.Text;
                        book.BookDesc = txtBookDesc.Text.Replace("'", "’");
                        book.BookStatus = ddlBookStatus.SelectedValue;
                        book.SeparatorType = txtSeparatorType.Text;
                        book.CompanyAddress1 = txtCompanyAddress1.Text;
                        book.CompanyAddress2 = txtCompanyAddress2.Text;
                        book.RetdEarnAcc = txtRetdEarnAcc.Text;
                        book.TaxNo = txtTaxNo.Text;
                        book.Phone = txtPhone.Text;
                        book.Fax = txtFax.Text;
                        book.Url = txtUrl.Text;
                        book.BankCode = txtBankCode.Text;
                        book.CashCode = txtCashCode.Text;
                        //book.logo = bytesLogo;
                        book.logo = (byte[])Session["imglogo"];
                        // book.logo = (byte[])ViewState["imglogo"];
                        GlBookManager.CreateGlBook(book);
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record created successfully!!');", true);
                        

                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You have no permission to add data in this form!!');", true);
                    }
                }
            
        }
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            if (txtBookName.Text != String.Empty)
            {
                GlBook book = GlBookManager.getBook(txtBookName.Text);
                if (book != null)
                {
                    GlBookManager.DeleteGlBook(book);
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record deleted successfully!!');", true);
                }
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You have no permission to delete these data!!');", true);
        }
    }
    protected void btnAuth_Click(object sender, EventArgs e)
    {

    }
    protected void dgGlBook_SelectedIndexChanged(object sender, EventArgs e)
    {
        clearFields();
        GlBook book = GlBookManager.getBook(dgGlBook.SelectedRow.Cells[1].Text.ToString().Trim());
        if (book != null)
        {
            txtBookName.Text = book.BookName;
            txtBookDesc.Text = book.BookDesc;
            ddlBookStatus.SelectedValue = book.BookStatus;
            txtSeparatorType.Text = book.SeparatorType;
            txtCompanyAddress1.Text = book.CompanyAddress1;
            txtCompanyAddress2.Text = book.CompanyAddress2;
            txtRetdEarnAcc.Text = book.RetdEarnAcc ;
            txtTaxNo.Text = book.TaxNo;
            txtPhone.Text = book.Phone;
            txtFax.Text = book.Fax;
            txtUrl.Text = book.Url;
            txtBankCode.Text = book.BankCode;
            txtCashCode.Text = book.CashCode;            
            ImageLogo = (byte[])book.logo;
            ViewState["imglogo"] = ImageLogo;
            if (ImageLogo != null)
            {
                string base64String = Convert.ToBase64String(ImageLogo, 0, ImageLogo.Length);
                imgLogo.ImageUrl = "data:image/png;base64," + base64String;
            }
        }
    }
    protected void btnFind_Click(object sender, EventArgs e)
    {

    }
    protected void lbImgUpload_Click(object sender, EventArgs e)
    {
        if (txtBookName.Text != "" && imgUpload.HasFile)
        {            
            int width = 145;
            int height = 165;
            using (System.Drawing.Bitmap img = DataManager.ResizeImage(new System.Drawing.Bitmap(imgUpload.PostedFile.InputStream), width, height, DataManager.ResizeOptions.ExactWidthAndHeight))
            {
                imgUpload.PostedFile.InputStream.Close();
                ImageLogo = DataManager.ConvertImageToByteArray(img, System.Drawing.Imaging.ImageFormat.Png);
                img.Dispose();
            }
            ViewState["imglogo"] = ImageLogo;
            if (ImageLogo != null)
            {
                string base64String = Convert.ToBase64String(ImageLogo, 0, ImageLogo.Length);
                imgLogo.ImageUrl = "data:image/png;base64," + base64String;
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please input book name, and then browse an image!!');", true);
        }
    }
}
