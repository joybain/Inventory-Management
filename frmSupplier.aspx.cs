using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using sales;
using Delve;
using System.Globalization;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.draw;



public partial class frmSupplier : System.Web.UI.Page
{
    private byte [] ItemsPhotoNID;
    private byte[] ItemsPhoto;
    private static Permis per;
    VouchManager _aVouchManager=new VouchManager();
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
        if (!IsPostBack)
        {
            UPSearch.Update();
            DataTable dt = SupplierManager.GetSuppliers("");
            dgSup.DataSource = dt;
            Session["Sup"] = dt;
            dgSup.DataBind();

            string query2 = "select [ID] ,[Name] from [SupplierGroup] where Active='True' order by 1";
            util.PopulationDropDownList(ddlSupplierGroup, "SupplierGroup", query2, "Name", "ID");

            string query3 = "select [COUNTRY_CODE] ,[COUNTRY_DESC] from [COUNTRY_INFO] order by 1";
            util.PopulationDropDownList(ddlCountry, "COUNTRY_INFO", query3, "COUNTRY_DESC", "COUNTRY_CODE");
            ddlCountry.SelectedIndex = -1;
            imgSup.ImageUrl = "img/noimage.jpg";

            imgSupNID.ImageUrl = "img/noimage.jpg";
            
            dgSup.Visible = true;

            DataTable dtFixCode = _aVouchManager.getFixCode();
            ViewState["dtFixCode"] = dtFixCode;
            txtCompanyName.Focus();
        }
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    
    protected void btnSave_Click(object sender, EventArgs e)
    {
        string IdGlCoa = "";
        DataTable dtFixCode = (DataTable)ViewState["dtFixCode"];
        if (txtSupplierName.Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Supplier Name..!!');", true);
            return;
        }
        if(string.IsNullOrEmpty(ddlCountry.SelectedItem.Text))
        {

            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Select Country Name..!!');", true);
            return;
        }
        if (string.IsNullOrEmpty(txtCompanyName.Text))
        {

            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('input company Name..!!');", true);
            return;
        }
        Supplier sup = SupplierManager.GetSupplier(lbLID.Text);
        if (sup != null)
        {
            if (per.AllowEdit == "Y")
            {
                //int CheckCode = IdManager.GetShowSingleValueInt("COUNT(*)", "SEG_COA_CODE", "GL_SEG_COA", txtGlCoa.Text);
                //if (CheckCode <= 0)
                //{
                //    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Rong Segment code check this code.\\n Countract your account officer.!!');", true);
                //}

                int checkSupplier =
                    IdManager.GetShowSingleValueInt("COUNT(*)",
                        "[dbo].[Supplier] where upper([Name])=UPPER('" +
                        txtCompanyName.Text.Trim().Replace(" ", "").Replace("  ", "").Replace("   ", "") +
                        "') and ID!='" +
                        lbLID.Text + "' ");
                if (checkSupplier > 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('This Company already exist in database.!!');", true);
                    return;
                }

                int checkMobile =
                    IdManager.GetShowSingleValueInt("COUNT(*)",
                        "[dbo].[Supplier] where upper([Mobile])=UPPER('" +
                        txtMobile.Text.Trim().Replace(" ", "").Replace("  ", "").Replace("   ", "") + "') and ID!='" +
                        lbLID.Text + "' ");
                if (!string.IsNullOrEmpty(txtMobile.Text))
                {
                    if (checkMobile > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('This mobile number already exist in database.!!');", true);
                        return;
                    }
                }

                sup.ID = lbLID.Text;
                sup.SupCode = txtSupCode.Text;
                sup.ComName = txtCompanyName.Text;
                sup.SupAddr1 = txtAddress1.Text;
                sup.SupName = txtSupplierName.Text;
                sup.SupAddr2 = txtAddress2.Text;
                sup.Designation = txtDesignation.Text;
                sup.City = txtCity.Text;
                sup.SupMobile = txtMobile.Text;
                sup.State = txtState.Text;
                sup.SupPhone = txtPhone.Text;
                sup.PostCode = txtPostalCode.Text;
                sup.Fax = txtFax.Text;
                if (Session["SupNIDPhoto"] != null)
                {
                    sup.SupNIDImage = (byte[])Session["SupNIDPhoto"];
                }
                if (Session["SupPhoto"] != null)
                {
                    sup.SupCurrentImage = (byte[])Session["SupPhoto"];
                }
                sup.Country = ddlCountry.SelectedValue;
                sup.Email = txtEmail.Text;
                sup.SupGroup = ddlSupplierGroup.SelectedValue;
                sup.GlCoa = txtGlCoa.Text.Replace("'", "");
                if (CheckBox1.Checked)
                { sup.Active = "True"; }
                else { sup.Active = "False"; }
                sup.LoginBy = Session["user"].ToString();

                string NM = IdManager.GetAccCompanyName("")+ " , Accounts Payable To - " + txtCompanyName.Text;
                if (!string.IsNullOrEmpty(txtSupplierName.Text))
                {
                    NM = IdManager.GetAccCompanyName("") + " , Accounts Payable To - " + txtCompanyName.Text + " - " +
                         txtSupplierName.Text;
                }

                sup.CoaDesc = "Accounts Payable To - " + txtCompanyName.Text + " - " +
                         txtSupplierName.Text; ;
               // sup.CoaDesc = NM + " , Accounts Payable To - " + txtCompanyName.Text+" - ";
                SupplierManager.UpdateSupplier(sup);
                clearField();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are update successfully..!!');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
            }
        }
        else
        {
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('you not create on supplier.\\n you only udate supplier this from.\\n do you want to create supplier please call your account officer.!!');", true);
            //return;
            if (per.AllowAdd == "Y")
            {
                int checkSupplier =
                    IdManager.GetShowSingleValueInt("COUNT(*)",
                        "[dbo].[Supplier] where upper([Name])=UPPER('" + txtCompanyName.Text.Trim().Replace(" ", "")
                            .Replace("  ", "").Replace("   ", "") + "')");
                if (checkSupplier > 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('This Company already exist in database.!!');", true);
                    return;
                }

                int checkMobile =
                    IdManager.GetShowSingleValueInt("COUNT(*)",
                        "[dbo].[Supplier] where upper([Mobile])=UPPER('" +
                        txtMobile.Text.Replace(" ", "").Replace("  ", "").Replace("   ", "") + "')");
                if (!string.IsNullOrEmpty(txtMobile.Text))
                {
                    if (checkMobile > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('This mobile number already exist in database.!!');", true);
                        return;
                    }
                }

                sup = new Supplier();
                sup.SupCode = txtSupCode.Text;
                sup.ComName = txtCompanyName.Text; 
                sup.SupAddr1 = txtAddress1.Text;
                sup.SupName = txtSupplierName.Text;
                sup.SupAddr2 = txtAddress2.Text;
                sup.Designation = txtDesignation.Text;
                sup.City = txtCity.Text;
                sup.SupMobile = txtMobile.Text;
                sup.State = txtState.Text;
                sup.SupPhone = txtPhone.Text;
                sup.PostCode = txtPostalCode.Text;
                sup.Fax = txtFax.Text;
                sup.Country = ddlCountry.SelectedValue;
                sup.Email = txtEmail.Text;
                if (Session["SupNIDPhoto"] !=null)
                {
                     sup.SupNIDImage = (byte[])Session["SupNIDPhoto"];
                }
                if (Session["SupPhoto"] != null)
                {
                    sup.SupCurrentImage = (byte[])Session["SupPhoto"];
                }
               
                sup.SupGroup = ddlSupplierGroup.SelectedValue;
                if (CheckBox1.Checked)
                { sup.Active = "True"; }
                else { sup.Active = "False"; }
                sup.SupCode = IdManager.GetNextID("supplier", "Code").ToString().PadLeft(7, '0');
                sup.LoginBy = Session["user"].ToString();
                IdGlCoa = IdManager.getAutoIdWithParameter("401", "GL_SEG_COA", "SEG_COA_CODE",
                    "" + dtFixCode.Rows[0]["AccountsPayable"].ToString() + "", "0000", "4");
                sup.GlCoa = IdGlCoa;
                SegCoa sg = SegCoaSet(IdGlCoa);
                GlCoa gl = GLCoaSet(IdGlCoa);

                SupplierManager.CreateSupplier(sup, sg, gl);
                clearField();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are saved successfully...!!');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
            }
        }
    }


    private GlCoa GLCoaSet(string IdGlCoa)
    {
        GlCoa gl = new GlCoa();
        //gl.GlCoaCode = "1-" + GlCoa;
        gl.GlCoaCode = "1-" + IdGlCoa;
        gl.CoaEnabled = "Y";
        gl.BudAllowed = "N";
        gl.PostAllowed = "Y";
        gl.Taxable = "N";
        gl.AccType = "L";
        gl.Status = "A";
        gl.BookName = "AMB";
       // string NM = IdManager.GetAccCompanyName("");
      //  gl.CoaDesc = NM + " , Accounts Payable To - " + txtCompanyName.Text;

        string NM = IdManager.GetAccCompanyName("") + " , Accounts Payable To - " + txtCompanyName.Text;
        if (!string.IsNullOrEmpty(txtSupplierName.Text))
        {
            NM = IdManager.GetAccCompanyName("") + " , Accounts Payable To - " + txtCompanyName.Text + " - " +
                 txtSupplierName.Text;
        }

        gl.CoaDesc = NM;
        gl.CoaCurrBal = "0.00";
        //gl.CoaNaturalCode = GlCoa;
        gl.CoaNaturalCode = IdGlCoa;
        return gl;
    }

    private SegCoa SegCoaSet(string IdGlCoa)
    {
        DataTable dtFixCode = (DataTable)ViewState["dtFixCode"];
        SegCoa sg = new SegCoa();
        //sg.GlSegCode = GlCoa;
        sg.GlSegCode = IdGlCoa;
        sg.SegCoaDesc = "Accounts Payable To - " + txtCompanyName.Text;
        if (!string.IsNullOrEmpty(txtSupplierName.Text))
        {
            sg.SegCoaDesc = "Accounts Payable To - " + txtCompanyName.Text + " - " + txtSupplierName.Text;
        }
        sg.LvlCode = "02";
        sg.ParentCode = dtFixCode.Rows[0]["AccountsPayable"].ToString();
        sg.BudAllowed = "Y";
        sg.PostAllowed = "N";
        sg.AccType = "L";
        sg.OpenDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
        sg.RootLeaf = "L";
        sg.Status = "A";
        sg.Taxable = "N";
        sg.BookName = "AMB";
        sg.EntryUser = Session["user"].ToString();
        sg.EntryDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
        sg.AuthoDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
        sg.AuthoUser = "ACC";
        return sg;
    }    
    public void clearField()
    {
        txtSupCode.Text = "";
        txtCompanyName.Text = "";
        txtAddress1.Text = "";
        txtSupplierName.Text = "";
        txtAddress2.Text = "";
        txtDesignation.Text = "";
        txtCity.Text = "";
        txtMobile.Text = "";
        txtState.Text = "";
        txtPhone.Text = "";
        txtPostalCode.Text = "";
        txtFax.Text = "";
        txtGlCoa.Text = "";
       // ddlCountry.SelectedValue = "131";
        txtEmail.Text = "";
        lbLID.Text = "";
        ddlSupplierGroup.SelectedIndex = -1;
        dgSup.DataSource = SupplierManager.GetSuppliers("");
        dgSup.DataBind();
        dgSup.Visible = true;
        txtCompanyName.Focus();
        imgSup.ImageUrl = "S00003_guard1.jpg";

        imgSupNID.ImageUrl = "S00003_guard1.jpg";
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            Supplier sup = SupplierManager.GetSupplier(lbLID.Text);
            if (sup != null)
            {
                SupplierManager.DeleteSupplier(sup);
                clearField();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Delete Successfull...!!');", true);
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
        }
    }
    protected void dgSup_SelectedIndexChanged(object sender, EventArgs e)
    {
        Supplier sup = SupplierManager.GetSupplier(dgSup.SelectedRow.Cells[1].Text.Trim());
        if (sup != null)
        {
            lbLID.Text=sup.ID;
            txtSupCode.Text=sup.SupCode;
            txtCompanyName.Text=sup.ComName;
            txtAddress1.Text=sup.SupAddr1;
            txtSupplierName.Text=sup.SupName;
            txtAddress2.Text=sup.SupAddr2;
            txtDesignation.Text=sup.Designation;
            txtCity.Text=sup.City;
            txtMobile.Text=sup.SupMobile;
            txtState.Text=sup.State;
            txtPhone.Text=sup.SupPhone;
            txtPostalCode.Text=sup.PostCode;
            txtFax.Text=sup.Fax;
            ddlCountry.SelectedValue=sup.Country;
            txtEmail.Text=sup.Email;
            ddlSupplierGroup.SelectedValue = sup.SupGroup;
            
            ItemsPhotoNID = (byte[])sup.SupNIDImage;
            Session["SupNIDPhoto"] = ItemsPhotoNID;
            if (ItemsPhotoNID != null)
            {
                string base64String = Convert.ToBase64String(ItemsPhotoNID, 0, ItemsPhotoNID.Length);
                
                imgSupNID.ImageUrl = "data:image/png;base64," + base64String;
            }

            ItemsPhoto = (byte[])sup.SupCurrentImage;
            Session["SupPhoto"] = ItemsPhoto;
            if (ItemsPhoto != null)
            {
                string base64String = Convert.ToBase64String(ItemsPhoto, 0, ItemsPhoto.Length);

                imgSup.ImageUrl = "data:image/png;base64," + base64String;
            }

            txtGlCoa.Text = sup.GlCoa;
            if ( sup.Active == "True")
            { CheckBox1.Checked = true; }
            else { CheckBox1.Checked = true; }
            dgSup.Visible = false;
        }
    }
    protected void dgSup_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgSup.DataSource = Session["Sup"];
        dgSup.PageIndex = e.NewPageIndex;
        dgSup.DataBind();
    }
    protected void dgSup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");
            e.Row.Cells[2].Attributes.Add("style", "display:none");
        }
    }

    protected void lbImgUpload_Click(object sender, EventArgs e)
    {
        try
        {
            if (imgUploadNID.HasFile)
            {
                int width = 145;
                int height = 165;
                using (System.Drawing.Bitmap img = EmpManager.ResizeImage(new System.Drawing.Bitmap(imgUploadNID.PostedFile.InputStream), width, height, EmpManager.ResizeOptions.ExactWidthAndHeight))
                {
                    imgUploadNID.PostedFile.InputStream.Close();
                    ItemsPhotoNID = EmpManager.ConvertImageToByteArray(img, System.Drawing.Imaging.ImageFormat.Png);
                    Session["SupNIDPhoto"] = ItemsPhotoNID;
                    img.Dispose();
                }
                string base64String = Convert.ToBase64String(ItemsPhotoNID, 0, ItemsPhotoNID.Length);
                imgSupNID.ImageUrl = "data:image/png;base64," + base64String;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please input employee first name, birth date, and then browse a photograph image!!');", true);
            }
        }
        catch (FormatException fex)
        {
            ExceptionLogging.SendExcepToDB(fex);
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            ExceptionLogging.SendExcepToDB(ex);
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex.Message + "');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex.Message + "');", true);

        }
    }
    protected void imgBtnsup_Click(object sender, EventArgs e)
    {
        try
        {
            if (imgUploadsup.HasFile)
            {
                int width = 145;
                int height = 165;
                using (System.Drawing.Bitmap img = EmpManager.ResizeImage(new System.Drawing.Bitmap(imgUploadsup.PostedFile.InputStream), width, height, EmpManager.ResizeOptions.ExactWidthAndHeight))
                {
                    imgUploadsup.PostedFile.InputStream.Close();
                    ItemsPhoto = EmpManager.ConvertImageToByteArray(img, System.Drawing.Imaging.ImageFormat.Png);
                    Session["SupPhoto"] = ItemsPhoto;
                    img.Dispose();
                }
                string base64String = Convert.ToBase64String(ItemsPhoto, 0, ItemsPhoto.Length);                
                imgSup.ImageUrl = "data:image/png;base64," + base64String;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please input employee first name, birth date, and then browse a photograph image!!');", true);
            }
        }
        catch (FormatException fex)
        {
            ExceptionLogging.SendExcepToDB(fex);
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            ExceptionLogging.SendExcepToDB(ex);
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex.Message + "');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex.Message + "');", true);

        }
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (lbLID.Text !=null)
        {
            getEmpRptPdf(lbLID.Text);
        }
        
    }
    private void getEmpRptPdf(string SupID)
    {
        DataTable dtSup = SupplierManager.getSupRpt(SupID);
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename=SupplierInformation-(" + DateTime.Now.ToString("dd-MM-yyyy") + ").pdf");
        Document document = new Document(PageSize.A4, 20f, 20f, 20f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        PdfPCell cell;
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 20;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPTable foot = new PdfPTable(1);
        foot.TotalWidth = page.Width - 20;
        phrase = new Phrase((document.PageNumber + 1).ToString(), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        foot.AddCell(c);
        foot.WriteSelectedRows(0, -1, 0, 20, writer.DirectContent);

          byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());

            iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
            gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
            gif.ScalePercent(8f);

            float[] titwidth = new float[2] { 10, 200 };
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;
            cell = new PdfPCell(gif);
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 80f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase("Supplier Information", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 20f;
            dth.AddCell(cell);
            document.Add(dth);
            LineSeparator line = new LineSeparator(1f, 100, null, Element.ALIGN_CENTER, -2);
            document.Add(line);
            PdfPTable dtempty = new PdfPTable(1);
            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.BorderWidth = 0f;
            cell.FixedHeight = 10f;
            dtempty.AddCell(cell);
            document.Add(dtempty);

            if (dtSup.Rows.Count > 0)
            {
                Supplier Sup = SupplierManager.GetSupplier(lbLID.Text);
                byte[] Nid = (byte[])Sup.SupNIDImage;
                byte[] Current = (byte[])Sup.SupCurrentImage;
                float[] imgPdfTable = new float[2] { 50,50};
                PdfPTable pdtphoto = new PdfPTable(imgPdfTable);
                pdtphoto.WidthPercentage = 100;
                if (Nid != null)
                {
                    iTextSharp.text.Image gifpht = iTextSharp.text.Image.GetInstance(Nid);
                    gifpht.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                    gifpht.ScalePercent(50f);
                    cell = new PdfPCell(gifpht);
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0f;
                    pdtphoto.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    //cell.FixedHeight = 80f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtphoto.AddCell(cell);
                }
                if (Current != null)
                {
                    iTextSharp.text.Image gifsig = iTextSharp.text.Image.GetInstance(Current);
                    gifsig.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                    gifsig.ScalePercent(50f);
                    cell = new PdfPCell(gifsig);
                    //cell.FixedHeight = 80f;
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0f;
                    pdtphoto.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 1;
                    cell.Padding = 2f;
                    //cell.FixedHeight = 90f;
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtphoto.AddCell(cell);
                }


                float[] widthemp = new float[7] { 20, 5, 60, 5, 30, 5, 50 };
                PdfPTable pdtemp = new PdfPTable(widthemp);
                pdtemp.WidthPercentage = 100;
                cell = new PdfPCell(new Phrase("Supplier Name", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                
                cell = new PdfPCell(FormatPhrase(((DataRow)dtSup.Rows[0])["ContactName"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(pdtphoto);
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.Rowspan = 3;
                cell.Colspan = 4;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell((new Phrase("Company Name", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD))));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(((DataRow)dtSup.Rows[0])["Name"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);


                cell = new PdfPCell(new Phrase("Designation", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(((DataRow)dtSup.Rows[0])["Designation"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;


                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 9f;
                cell.Colspan = 7;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase("Nationality"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(((DataRow)dtSup.Rows[0])["COUNTRY_DESC"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("National /Any Valid ID"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(((DataRow)dtSup.Rows[0])["Fax"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);


                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 9f;
                cell.Colspan = 7;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("City"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(((DataRow)dtSup.Rows[0])["City"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("State"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(((DataRow)dtSup.Rows[0])["State"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 9f;
                cell.Colspan = 7;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase("Office Address"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(((DataRow)dtSup.Rows[0])["Address1"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("Residential Address"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(((DataRow)dtSup.Rows[0])["Address2"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);


                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 9f;
                cell.Colspan = 7;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);


                cell = new PdfPCell(FormatPhrase("Mobile"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(((DataRow)dtSup.Rows[0])["Mobile"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);


                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("Phone"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(((DataRow)dtSup.Rows[0])["Phone"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 9f;
                cell.Colspan = 7;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("E-mail"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(((DataRow)dtSup.Rows[0])["Email"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("Supplier Group"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(((DataRow)dtSup.Rows[0])["SuplierGroup"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);


                document.Add(pdtemp);

                document.Close();
                Response.Flush();
                Response.End();
            }
            else
            {
                float[] widtPhoto = new float[4] { 25,35, 5, 35};
                PdfPTable pdtphoto = new PdfPTable(widtPhoto);
                pdtphoto.WidthPercentage = 100;

                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.BLACK;
                cell.BorderWidth = 0f;
                cell.Padding = 2f;
                cell.FixedHeight = 80f;
                pdtphoto.AddCell(cell);
                  cell = new PdfPCell(FormatPhrase("NID Images"));
                  cell.HorizontalAlignment = 1;
                  cell.VerticalAlignment = 1;
                  cell.BorderColor = BaseColor.BLACK;
                  cell.BorderWidth = 1f;
                  cell.Padding = 2f;
                  cell.FixedHeight = 80f;
                    pdtphoto.AddCell(cell);

                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 0f;
                    cell.Padding = 2f;
                    cell.FixedHeight = 80f;
                    pdtphoto.AddCell(cell);

                    cell = new PdfPCell(FormatPhrase("Photo"));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 1f;
                    cell.Padding = 2f;
                    cell.FixedHeight = 80f;
                                   
                    pdtphoto.AddCell(cell);
             


                float[] widthemp = new float[7] { 30, 5, 50, 5, 30, 5, 50 };
                PdfPTable pdtemp = new PdfPTable(widthemp);
                pdtemp.WidthPercentage = 100;
                cell = new PdfPCell(new Phrase("Supplier Name", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(pdtphoto);
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.Rowspan = 3;
                cell.Colspan = 4;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell((new Phrase("Company Name", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD))));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);


                cell = new PdfPCell(new Phrase("Designation", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;


                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 9f;
                cell.Colspan = 7;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase("Nationality"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("National /Any Valid ID"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);


                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 9f;
                cell.Colspan = 7;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("City"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("State"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 9f;
                cell.Colspan = 7;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase("Office Address"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("Residential Address"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);


                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 9f;
                cell.Colspan = 7;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);


                cell = new PdfPCell(FormatPhrase("Mobile"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);


                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("Phone"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 9f;
                cell.Colspan = 7;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("E-mail"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("Supplier Group"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(":"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.MinimumHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);


                document.Add(pdtemp);

                document.Close();
                Response.Flush();
                Response.End();
            }
    }
   
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9));
    }
    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }
    protected void txtSupplierSearch_TextChanged(object sender, EventArgs e)
    {
        DataTable dtSupplier = PurchaseVoucherManager.GetSupplierInfo(txtSupplierSearch.Text);
        if (dtSupplier.Rows.Count > 0)
        {
            txtSupplierSearch.Text = dtSupplier.Rows[0]["SupplierSearch"].ToString();
            hfSupplierID.Value = dtSupplier.Rows[0]["ID"].ToString();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Not Found Supplier.!!');", true);
            txtSupplierSearch.Text = hfSupplierID.Value = "";
            txtSupplierSearch.Focus();
        }
    }
    protected void lbSearch_Click(object sender, EventArgs e)
    {
       string[] words = txtSupplierSearch.Text.Trim().Split('-');
        if (words.Length > 1)
        {
            DataTable dt = SupplierManager.GetSuppliers(" where ID="+hfSupplierID.Value);
            dgSup.DataSource = dt;
            Session["Sup"] = dt;
            dgSup.DataBind();
        }
        else
        {
            DataTable dt =
                SupplierManager.GetSuppliers(
                    " where UPPER(isnull(code,'')+t1.contactName+isnull(country_desc,'')+isnull(Phone,'')) LIKE '%" +
                    txtSupplierSearch.Text.ToUpper() + "%' ");
            dgSup.DataSource = dt;
            Session["Sup"] = dt;
            dgSup.DataBind();
        }
    }
    protected void lbClear_Click(object sender, EventArgs e)
    {
        txtSupplierSearch.Text = string.Empty;
        DataTable dt = SupplierManager.GetSuppliers("");
        dgSup.DataSource = dt;
        Session["Sup"] = dt;
        dgSup.DataBind();
        dgSup.Visible = true;
        UPSearch.Update();
    }
}
