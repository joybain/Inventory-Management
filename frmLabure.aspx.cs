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
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;



public partial class frmLabure : System.Web.UI.Page
{
    private byte[] ItemsPhotoNID;
    private byte[] ItemsPhoto;
    private static Permis per;
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
            txtJoinDate.Attributes.Add("onBlur", "formatdate('" + txtJoinDate.ClientID + "')");
            txtRetairedDate.Attributes.Add("onBlur", "formatdate('" + txtRetairedDate.ClientID + "')");
            DataTable dt = LabureManager.GetLabour();
            Session["Lab"]=dt;
            dgSup.DataSource = dt;
            dgSup.DataBind();
            clearField();
            //string query2 = "select '' [ID],'' [Name]  union select [ID] ,[Name] from [SupplierGroup] where Active='True' order by 1";
            //util.PopulationDropDownList(ddlSupplierGroup, "SupplierGroup", query2, "Name", "ID");

            string query3 = "select '' [COUNTRY_CODE],'' [COUNTRY_DESC]  union select [COUNTRY_CODE] ,[COUNTRY_DESC] from [COUNTRY_INFO] order by 1";
            util.PopulationDropDownList(ddlCountry, "COUNTRY_INFO", query3, "COUNTRY_DESC", "COUNTRY_CODE");
            //ddlCountry.SelectedValue = "131";
            dgSup.Visible = true;
            txtCompanyName.Focus();
        }
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        clearField();
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        //string IdGlCoa = "";
        //string Desc = "";
        //string User = "";
        //string glCode = "";



        //if (ddlCountry.SelectedValue == "1")
        //{
        //    Desc = "BD-Accounts Receivable from-Customer-" + txtSupplierName.Text;
        //    User = "Ban";
        //    glCode = "1044000";


        //if (ddlSupplierGroup.SelectedValue.Trim() == "LP")
        //{
        //    IdGlCoa = IdManager.getAutoIdWithParameter("4100", "GL_SEG_COA", "SEG_COA_CODE", "4100105", "0000", "4");
        //    glCode = "4030000";
        //    NM = "Labour Person";
        //}
        //else
        //{
        //    IdGlCoa = IdManager.getAutoIdWithParameter("4100", "GL_SEG_COA", "SEG_COA_CODE", "4100106", "0000", "4");
        //    Parent_Code = "4040000";
        //    NM = "Carriage Person";
        //}
        //}
        //else if (ddlCountry.SelectedValue == "2")
        //{
        //    Desc = "PH_Accounts Receivable from-Customer-" + txtSupplierName.Text;
        //    User = "Man";
        //    glCode = "1046000";
        //}
        //else
        //{
        //    Desc = "Accounts Receivable from-Customer-" + txtSupplierName.Text;
        //    User = "All";
        //}



        string IdGlCoa = "";
        string Parent_Code = "";
        string NM = "";
        Labure Lab = LabureManager.GetLabure(lbLID.Text);
        if (Lab != null)
        {
            if (per.AllowEdit == "Y")
            {
                Lab.ID = lbLID.Text;
                Lab.SupCode = txtSupCode.Text;
                Lab.ComName = txtCompanyName.Text;
                Lab.SupAddr1 = txtAddress1.Text;
                Lab.SupName = txtSupplierName.Text;
                Lab.SupAddr2 = txtAddress2.Text;
                Lab.Designation = txtDesignation.Text;
                Lab.City = txtCity.Text;
                Lab.SupMobile = txtMobile.Text;
                Lab.State = txtState.Text;
                Lab.JoinDate = txtJoinDate.Text;
                Lab.RetairedDate = txtRetairedDate.Text;
                if (Session["LabNIDPhoto"] != null)
                {
                    Lab.NIDImage = (byte[])Session["LabNIDPhoto"];
                }
                if (Session["LabPhoto"] != null)
                {
                    Lab.CurrentImage = (byte[])Session["LabPhoto"];
                }
                Lab.SupPhone = txtPhone.Text;
                Lab.PostCode = txtPostalCode.Text;
                Lab.Fax = txtFax.Text;
                Lab.Country = ddlCountry.SelectedValue;
                Lab.Email = txtEmail.Text;
                Lab.SupGroup = ddlSupplierGroup.SelectedValue;
                if (CheckBox1.Checked)
                { Lab.Active = "True"; }
                else { Lab.Active = "False"; }
                Lab.LoginBy = Session["user"].ToString();
                Lab.CoaDesc = Session["org"].ToString().Substring(0, 5) + "," + txtSupplierName.Text + "," + txtMobile.Text;
                LabureManager.UpdateSupplier(Lab);
                clearField();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are update suceessfullly..!!');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
            }
        }
        else
        {
            if (txtSupplierName.Text == "") 
            { ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Name ..!!');", true); }
            else if (ddlSupplierGroup.SelectedItem.Text == "") 
            { ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Select type..!!');", true); }
            else
            {
                if (per.AllowAdd == "Y")
                {
                    Lab = new Labure();
                    Lab.SupCode = txtSupCode.Text;
                    Lab.ComName = txtCompanyName.Text;
                    Lab.SupAddr1 = txtAddress1.Text;
                    Lab.SupName = txtSupplierName.Text;
                    Lab.SupAddr2 = txtAddress2.Text;
                    Lab.Designation = txtDesignation.Text;
                    Lab.City = txtCity.Text;
                    Lab.SupMobile = txtMobile.Text;
                    Lab.State = txtState.Text;
                    Lab.SupPhone = txtPhone.Text;
                    Lab.PostCode = txtPostalCode.Text;
                    Lab.Fax = txtFax.Text;
                    Lab.JoinDate = txtJoinDate.Text;
                    Lab.RetairedDate = txtRetairedDate.Text;
                    if (Session["LabNIDPhoto"] != null)
                    {
                        Lab.NIDImage = (byte[])Session["LabNIDPhoto"];
                    }
                    if (Session["LabPhoto"] != null)
                    {
                        Lab.CurrentImage = (byte[])Session["LabPhoto"];
                    }
                    Lab.Country = ddlCountry.SelectedValue;
                    Lab.Email = txtEmail.Text;
                    Lab.SupGroup = ddlSupplierGroup.SelectedValue;
                    if (CheckBox1.Checked)
                    { Lab.Active = "True"; }
                    else { Lab.Active = "False"; }
                    Lab.SupCode = IdManager.GetNextID("Labure", "Code").ToString().PadLeft(7, '0');
                    Lab.LoginBy = Session["user"].ToString();
                    if (ddlSupplierGroup.SelectedValue.Trim() == "LP")
                    {
                        IdGlCoa = IdManager.getAutoIdWithParameter("4100", "GL_SEG_COA", "SEG_COA_CODE", "4100105", "0000", "4");
                        IdGlCoa = "4030000";
                        NM = "Labour Person";
                    }
                    else
                    {
                        IdGlCoa = IdManager.getAutoIdWithParameter("4100", "GL_SEG_COA", "SEG_COA_CODE", "4100106", "0000", "4");
                        Parent_Code = "4040000";
                        NM = "Carriage Person";
                    }
                    Lab.GlCoa = IdGlCoa;
                    LabureManager.CreateLabure(Lab);
                    //Gl_COA(IdGlCoa, Parent_Code, NM);
                    clearField();
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are saved suceessfullly...!!');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                }
            }
        }
    }
    //private void Gl_COA(string GlCoa,string PCode,string mm)
    //{
    //    SegCoa sg = new SegCoa();
    //    sg.GlSegCode = GlCoa;
    //    sg.SegCoaDesc = mm+","+txtSupplierName.Text;
    //    sg.LvlCode = "02";
    //    sg.ParentCode = PCode;
    //    sg.BudAllowed = "Y";
    //    sg.PostAllowed = "N";
    //    sg.AccType = "L";
    //    sg.OpenDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
    //    sg.RootLeaf = "L";
    //    sg.Status = "A";
    //    sg.Taxable = "N";
    //    sg.BookName = "AMB";
    //    sg.EntryUser = Session["user"].ToString();
    //    sg.EntryDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
    //    sg.AuthoDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
    //    sg.AuthoUser = "ACC";
    //    SegCoaManager.CreateSegCoa(sg);
    //    //string dept = SegCoaManager.GetSegCoaDesc(Session["dept"].ToString());
    //    GlCoa gl = new GlCoa();
    //    gl.GlCoaCode = "1-" + GlCoa;
    //    gl.CoaEnabled = "Y";
    //    gl.BudAllowed = "N";
    //    gl.PostAllowed = "Y";
    //    gl.Taxable = "N";
    //    gl.AccType = "L";
    //    gl.Status = "A";
    //    gl.BookName = "AMB";
    //    gl.CoaDesc = Session["org"].ToString().Substring(0, 5) + "," + mm + "," + txtSupplierName.Text + "-" + txtMobile.Text;
    //    gl.CoaCurrBal = "0.00";
    //    gl.CoaNaturalCode = GlCoa;
    //    GlCoaManager.CreateGlCoa(gl);
    //}
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
        lbLID.Text = "";
        txtJoinDate.Text = "";
        txtRetairedDate.Text = "";
        imgSup.ImageUrl = "img/noimage.jpg";

        imgSupNID.ImageUrl = "img/noimage.jpg";
        txtEmail.Text = "";
        ddlSupplierGroup.SelectedIndex = -1;
        dgSup.DataSource = LabureManager.GetLabour();
        dgSup.DataBind();
        dgSup.Visible = true;
        txtCompanyName.Focus();
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            Labure Lab = LabureManager.GetLabure(lbLID.Text);
            if (Lab != null)
            {
                Lab.ID = lbLID.Text;
                LabureManager.DeleteSupplier(Lab);                
                clearField();
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
        }
    }
    protected void dgSup_SelectedIndexChanged(object sender, EventArgs e)
    {
        Labure Lab = LabureManager.GetLabure(dgSup.SelectedRow.Cells[5].Text);
        if (Lab != null)
        {
            lbLID.Text = dgSup.SelectedRow.Cells[5].Text;
            txtSupCode.Text = Lab.SupCode;
            txtCompanyName.Text = Lab.ComName;
            txtAddress1.Text = Lab.SupAddr1;
            txtSupplierName.Text = Lab.SupName;
            txtAddress2.Text = Lab.SupAddr2;
            txtDesignation.Text = Lab.Designation;
            txtCity.Text = Lab.City;
            txtMobile.Text = Lab.SupMobile;
            txtState.Text = Lab.State;
            txtPhone.Text = Lab.SupPhone;
            txtPostalCode.Text = Lab.PostCode;
            txtFax.Text = Lab.Fax;
            txtJoinDate.Text = Lab.JoinDate;
            txtRetairedDate.Text = Lab.RetairedDate;
            ItemsPhotoNID = Lab.NIDImage;
            Session["LabNIDPhoto"] = ItemsPhotoNID;
            if (ItemsPhotoNID != null)
            {
                string base64String = Convert.ToBase64String(ItemsPhotoNID, 0, ItemsPhotoNID.Length);

                imgSupNID.ImageUrl = "data:image/png;base64," + base64String;
            }

            ItemsPhoto = (byte[])Lab.CurrentImage;
            Session["LabPhoto"] = ItemsPhoto;
            if (ItemsPhoto != null)
            {
                string base64String = Convert.ToBase64String(ItemsPhoto, 0, ItemsPhoto.Length);

                imgSup.ImageUrl = "data:image/png;base64," + base64String;
            }
            ddlCountry.SelectedValue = Lab.Country;
            txtEmail.Text = Lab.Email;
            ddlSupplierGroup.SelectedValue = Lab.SupGroup;
            if (Lab.Active == "True")
            { CheckBox1.Checked = true; }
            else { CheckBox1.Checked = true; }
            dgSup.Visible = false;
        }
    }
    protected void dgSup_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgSup.DataSource = Session["Lab"];
        dgSup.PageIndex = e.NewPageIndex;
        dgSup.DataBind();
    }
    protected void dgSup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[5].Attributes.Add("style", "display:none");
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
                    Session["LabNIDPhoto"] = ItemsPhotoNID;
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
                    Session["LabPhoto"] = ItemsPhoto;
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
        if (lbLID.Text != null)
        {
            getEmpRptPdf(lbLID.Text);
        }

    }
    private void getEmpRptPdf(string LABID)
    {
        DataTable dtlab = LabureManager.getLabRpt(LABID);
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=MypdfFile.pdf");
        Document document = new Document(PageSize.A4, 40f, 30f, 40f, 40f);
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
            cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase("Temporary Employee Resume", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
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
            if (dtlab.Rows.Count > 0)
            {
                Labure Lab = LabureManager.GetLabure(lbLID.Text);

                byte[] Nid = (byte[])Lab.NIDImage;
                byte[] Current = (byte[])Lab.CurrentImage;
                PdfPTable pdtphoto = new PdfPTable(2);
                pdtphoto.WidthPercentage = 100;
                if (Nid != null)
                {
                    iTextSharp.text.Image gifpht = iTextSharp.text.Image.GetInstance(Nid);
                    gifpht.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                    gifpht.ScalePercent(50f);                   
                   
                    cell = new PdfPCell(gifpht);

                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 0f;
                    cell.Padding = 2f;
                    cell.FixedHeight = 00f;
                    cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                    pdtphoto.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase("NID Images"));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 0f;
                    cell.Padding = 2f;
                    cell.FixedHeight = 50f;                   
                    pdtphoto.AddCell(cell);
                }
                if (Current != null)
                {
                    iTextSharp.text.Image gifsig = iTextSharp.text.Image.GetInstance(Current);
                    gifsig.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                    gifsig.ScalePercent(50f);                  
                    cell = new PdfPCell(gifsig);
                    cell.HorizontalAlignment = 1;                  
                    cell.Padding = 2f;
                    cell.FixedHeight = 50f;
                    cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;                    
                    pdtphoto.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase("Photo"));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_CENTER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.BorderWidth = 0f;
                    cell.Padding = 2f;
                    cell.FixedHeight = 50f;   
                    pdtphoto.AddCell(cell);
                }


                float[] widthemp = new float[7] { 30, 5, 50, 5, 30, 5, 50 };
                PdfPTable pdtemp = new PdfPTable(widthemp);
                pdtemp.WidthPercentage = 100;
                cell = new PdfPCell(new Phrase("Employee Name", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["Name"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(pdtphoto);
                cell.BorderWidth = 0;
                cell.Rowspan = 3;
                cell.Colspan = 4;
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["ContactName"].ToString()));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["Designation"].ToString()));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["COUNTRY_DESC"].ToString()));
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

                cell = new PdfPCell(FormatPhrase("National ID No"));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["Fax"].ToString()));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["City"].ToString()));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["State"].ToString()));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["Address1"].ToString()));
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

                cell = new PdfPCell(FormatPhrase("Date Of Join"));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["JoinDate"].ToString()));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["Address2"].ToString()));
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
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase("Date Of Retaired"));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["RetairedDate"].ToString()));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["Mobile"].ToString()));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["Phone"].ToString()));
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

                cell = new PdfPCell(FormatPhrase("Email"));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["Email"].ToString()));
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

                cell = new PdfPCell(FormatPhrase("Suplier Group"));
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
                cell = new PdfPCell(FormatPhrase(((DataRow)dtlab.Rows[0])["SupplierGroupID"].ToString()));
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

                float[] widtPhoto = new float[4] { 25, 35, 5, 35 };
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
                cell = new PdfPCell(new Phrase("Employee Name", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
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
                cell.FixedHeight = 30f;
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

                cell = new PdfPCell(FormatPhrase("National ID No"));
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

                cell = new PdfPCell(FormatPhrase("Date Of Join"));
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
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtemp.AddCell(cell);
                cell = new PdfPCell(FormatPhrase("Date Of Retaired"));
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

                cell = new PdfPCell(FormatPhrase("Email"));
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

                cell = new PdfPCell(FormatPhrase("Suplier Group"));
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
}
