using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;
using sales;
using iTextSharp.text;
using System.Data.SqlClient;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System.Data;

public partial class frmBranchClient : System.Web.UI.Page
{
    private byte[] ItemsPhotoNID;
    private byte[] ItemsPhoto;
    private static Permis per;
    clsClientInfoManager _aclsClientInfoManager = new clsClientInfoManager();
    VouchManager _aVouchManager = new VouchManager();
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
                        "Select BranchId, user_grp,[description],UserType,case when UserType=1 then 'Bangladesh' else 'Philippine' end AS[LoginCountry] from utl_userinfo where upper(user_name)=upper('" +
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

                            Session["BranchId"] = dReader["BranchId"].ToString();
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
                Response.Redirect("BranchHome.aspx?sid=sam");
            }
        }
        catch
        {
            Response.Redirect("Default.aspx?sid=sam");
        }
        if (!Page.IsPostBack)
        {

            var BranchId = Session["BranchId"].ToString();

            clearFields();

            DataTable dtFixCode = _aVouchManager.getFixCode();
            ViewState["dtFixCode"] = dtFixCode;

        }
    }
    protected void dgClient_SelectedIndexChanged(object sender, EventArgs e)
    {
        clsClientInfo ci = clsClientInfoManager.GetBranchClientInfo(dgClient.SelectedRow.Cells[5].Text.Trim(), Session["BranchId"].ToString());
        if (ci != null)
        {
            lbLId.Text = dgClient.SelectedRow.Cells[5].Text.Trim();
            lblGlCoa.Text = ci.GlCoa;
            txtGlCoa.Text = ci.GlCoa;
            txtClientId.Text = ci.Code;
            txtClientName.Text = ci.CustomerName;
            txtNationalId.Text = ci.NationalId;
            txtAddress1.Text = ci.Address1;
            txtAddress2.Text = ci.Address2;
            txtPhone.Text = ci.Phone;
            txtCity.Text = ci.City;
            txtState.Text = ci.State;
            txtPessoRate.Text = ci.PessoRate;
            ItemsPhotoNID = (byte[])ci.NIDImage;
            Session["ciNIDPhoto"] = ItemsPhotoNID;
            if (ItemsPhotoNID != null)
            {
                string base64String = Convert.ToBase64String(ItemsPhotoNID, 0, ItemsPhotoNID.Length);

                imgSupNID.ImageUrl = "data:image/png;base64," + base64String;
            }

            ItemsPhoto = (byte[])ci.CurrentImage;
            Session["ciPhoto"] = ItemsPhoto;
            if (ItemsPhoto != null)
            {
                string base64String = Convert.ToBase64String(ItemsPhoto, 0, ItemsPhoto.Length);

                imgSup.ImageUrl = "data:image/png;base64," + base64String;
            }
            txtMobile.Text = ci.Mobile;
            txtFax.Text = ci.Fax;
            txtEmail.Text = ci.Email;
            txtPostalCode.Text = ci.PostalCode;
            ddlCountry.SelectedValue = ci.Country;
            txtGlCoa.Text = ci.GlCoa;
            if (ci.Active == "True") { CheckBox1.Checked = true; } else { CheckBox1.Checked = false; }
            if (ci.CommonCus == "1") { CheckBox2.Checked = true; } else { CheckBox2.Checked = false; }
            dgClient.Visible = false;
        }
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        clearFields();
    }

    private void clearFields()
    {
        txtGlCoa.Text = "";
        txtClientId.Text = "";
        txtClientName.Text = "";
        txtNationalId.Text = "";
        txtAddress1.Text = "";
        txtAddress2.Text = "";
        txtPhone.Text = "";
        txtMobile.Text = "";
        txtFax.Text = "";
        txtEmail.Text = "";
        txtCity.Text = "";
        txtState.Text = "";
        txtPessoRate.Text = "";
        imgSup.ImageUrl = "img/noimage.jpg";

        imgSupNID.ImageUrl = "img/noimage.jpg";
        txtPostalCode.Text = "";
        lbLId.Text = txtGlCoa.Text = string.Empty;
        CheckBox2.Checked = false;
        DataTable dtCustomer = clsClientInfoManager.GetClientInfosGrid("where a.CommonCus='1' or a.BranchId='" + Session["BranchId"].ToString() + "'");
        dgClient.DataSource = dtCustomer;
        ViewState["SupHistory"] = dtCustomer;
        dgClient.DataBind();
        string query3 = "select [COUNTRY_CODE] ,[COUNTRY_DESC] from [COUNTRY_INFO] order by 1";
        util.PopulationDropDownList(ddlCountry, "COUNTRY_INFO", query3, "COUNTRY_DESC", "COUNTRY_CODE");
        dgClient.Visible = true;
        txtClientName.Focus();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string IdGlCoa = "";
        DataTable dtFixCode = (DataTable)ViewState["dtFixCode"];
        if (string.IsNullOrEmpty(ddlCountry.SelectedItem.Text))
        {

            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Select Country Name..!!');", true);
            return;
        }

        clsClientInfo ci = clsClientInfoManager.GetClientInfo(lbLId.Text);
        if (ci != null)
        {
            if (per.AllowEdit == "Y")
            {
                int checkSupplier =
                    IdManager.GetShowSingleValueInt("COUNT(*)",
                        "[dbo].[Customer] where  BranchId='" + Session["BranchId"].ToString() + "' and upper([ContactName])=UPPER('" + txtClientName.Text.Trim()
                            .Replace(" ", "")
                            .Replace("  ", "").Replace("   ", "") + "') and ID!='" + lbLId.Text + "' ");
                if (checkSupplier > 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('This Customer already exist in database.!!');", true);
                    return;
                }

                int checkMobile =
                    IdManager.GetShowSingleValueInt("COUNT(*)",
                        "[dbo].[Customer] where  BranchId='" + Session["BranchId"].ToString() + "' and upper([Mobile])=UPPER('" +
                        txtMobile.Text.Replace(" ", "").Replace("  ", "").Replace("   ", "") + "')  and ID!='" +
                        lbLId.Text + "' ");
                if (!string.IsNullOrEmpty(txtMobile.Text))
                {
                    if (checkMobile > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('This mobile number already exist in database.!!');", true);
                        return;
                    }
                }

                ci.ID = lbLId.Text;
                ci.Code = txtClientId.Text;
                ci.CustomerName = txtClientName.Text;
                ci.NationalId = txtNationalId.Text;
                ci.Address1 = txtAddress1.Text;
                ci.Address2 = txtAddress2.Text;
                ci.City = txtCity.Text;
                ci.State = txtState.Text;
                ci.Phone = txtPhone.Text;
                ci.Mobile = txtMobile.Text;
                ci.Fax = txtFax.Text;
                ci.Email = txtEmail.Text;
                ci.PostalCode = txtPostalCode.Text;
                ci.PessoRate = txtPessoRate.Text;
                ci.BranchId = Session["BranchId"].ToString();
                if (Session["ciNIDPhoto"] != null)
                {
                    ci.NIDImage = (byte[])Session["ciNIDPhoto"];
                }

                if (Session["ciPhoto"] != null)
                {
                    ci.CurrentImage = (byte[])Session["ciPhoto"];
                }

                ci.Country = ddlCountry.SelectedValue;
                ci.GlCoa = txtGlCoa.Text;
                ci.LoginBy = Session["user"].ToString();
                if (CheckBox1.Checked == true)
                {
                    ci.Active = "True";
                }
                else
                {
                    ci.Active = "False";
                }

                if (CheckBox2.Checked == true)
                {
                    ci.CommonCus = "1";
                }
                else
                {
                    ci.CommonCus = "0";
                }

                ci.GlCoa = txtGlCoa.Text;
                string NM = IdManager.GetAccCompanyName("");
                ci.GlCoaDesc = NM + " , Accounts Receivable from " + txtClientName.Text;
                clsClientInfoManager.UpdateClientInfo(ci);
                clearFields();
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    "alert('Record(s) is/are update successfully..!!');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    "alert('You are not Permitted this Step...!!');", true);
            }
        }
        else
        {
            if (txtClientName.Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter customer name!!');", true);
            }
            else
            {
                if (per.AllowAdd == "Y")
                {
                    int checkSupplier =
                        IdManager.GetShowSingleValueInt("COUNT(*)",
                            "[dbo].[Customer] where BranchId='" + Session["BranchId"].ToString() + "' and upper([ContactName])=UPPER('" + txtClientName.Text.Trim()
                                .Replace(" ", "")
                                .Replace("  ", "").Replace("   ", "") + "')");
                    if (checkSupplier > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('This Customer already exist in database.!!');", true);
                        return;
                    }

                    int checkMobile =
                        IdManager.GetShowSingleValueInt("COUNT(*)",
                            "[dbo].[Customer] where BranchId='" + Session["BranchId"].ToString() + "' and upper([Mobile])=UPPER('" +
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

                    ci = new clsClientInfo();
                    ci.CustomerName = txtClientName.Text;
                    ci.NationalId = txtNationalId.Text;
                    ci.Address1 = txtAddress1.Text;
                    ci.Address2 = txtAddress2.Text;
                    ci.Phone = txtPhone.Text;
                    ci.Mobile = txtMobile.Text;
                    ci.Fax = txtFax.Text;
                    ci.Email = txtEmail.Text;
                    ci.City = txtCity.Text;
                    ci.State = txtState.Text;
                    ci.PostalCode = txtPostalCode.Text;
                    ci.Country = ddlCountry.SelectedValue;
                    ci.PessoRate = txtPessoRate.Text;
                    ci.BranchId = Session["BranchId"].ToString();
                    if (Session["ciNIDPhoto"] != null)
                    {
                        ci.NIDImage = (byte[])Session["ciNIDPhoto"];
                    }

                    if (Session["ciPhoto"] != null)
                    {
                        ci.CurrentImage = (byte[])Session["ciPhoto"];
                    }

                    ci.Code = IdManager.GetNextID("Customer", "Code").ToString().PadLeft(7, '0');
                    txtClientId.Text = ci.Code;
                    if (CheckBox1.Checked == true)
                    {
                        ci.Active = "True";
                    }
                    else
                    {
                        ci.Active = "False";
                    }

                    if (CheckBox2.Checked == true)
                    {
                        ci.CommonCus = "1";
                    }
                    else
                    {
                        ci.CommonCus = "0";
                    }

                    ci.LoginBy = Session["user"].ToString();
                    ci.GlCoa = txtGlCoa.Text;

                    IdGlCoa = IdManager.getAutoIdWithParameter("1051", "GL_SEG_COA", "SEG_COA_CODE",
                        "" + dtFixCode.Rows[0]["AccountsReceivable"].ToString() + "", "000", "3");
                    ci.GlCoa = IdGlCoa;
                    //clsClientInfoManager.CreateClientInfo(ci);
                    SegCoa sg = new SegCoa();
                    sg.GlSegCode = IdGlCoa;
                    sg.SegCoaDesc = "Accounts Receivable from " + txtClientName.Text;
                    sg.LvlCode = "02";
                    sg.ParentCode = dtFixCode.Rows[0]["AccountsReceivable"].ToString();
                    sg.BudAllowed = "Y";
                    sg.PostAllowed = "N";
                    sg.AccType = "A";
                    sg.OpenDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                    sg.RootLeaf = "L";
                    sg.Status = "A";
                    sg.Taxable = "N";
                    sg.BookName = "AMB";
                    sg.EntryUser = Session["user"].ToString();
                    sg.EntryDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                    sg.AuthoDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                    sg.AuthoUser = "ACC";
                    //SegCoaManager.CreateSegCoa(sg);
                    //string dept = SegCoaManager.GetSegCoaDesc(Session["dept"].ToString());
                    GlCoa gl = new GlCoa();
                    gl.GlCoaCode = "1-" + IdGlCoa;
                    gl.CoaEnabled = "Y";
                    gl.BudAllowed = "N";
                    gl.PostAllowed = "Y";
                    gl.Taxable = "N";
                    gl.AccType = "A";
                    gl.Status = "A";
                    gl.BookName = "AMB";
                    string NM = IdManager.GetAccCompanyName("");
                    gl.CoaDesc = NM + " , Accounts Receivable from " + txtClientName.Text;
                    gl.CoaCurrBal = "0.00";
                    gl.CoaNaturalCode = IdGlCoa;
                    //GlCoaManager.CreateGlCoa(gl);

                    clsClientInfoManager.CreateClientInfo(ci, gl, sg);
                    clearFields();
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('Record(s) is/are saved successfully..!!');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('You are not Permitted this Step...!!');", true);
                }
            }
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            clsClientInfo ci = clsClientInfoManager.GetClientInfo(lbLId.Text);
            if (ci != null)
            {
                clsClientInfoManager.DeleteClientInfo(ci);
                clearFields();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are delete suceessfullly..!!');", true);
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
        }
    }
    protected void dgClient_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgClient.DataSource = ViewState["SupHistory"];
        dgClient.PageIndex = e.NewPageIndex;
        dgClient.DataBind();
    }
    protected void dgClient_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[5].Attributes.Add("style", "display:none");
            e.Row.Cells[1].Attributes.Add("style", "display:none");
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
                    Session["ciNIDPhoto"] = ItemsPhotoNID;
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
                    Session["ciPhoto"] = ItemsPhoto;
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
        if (lbLId.Text != null)
        {
            getEmpRptPdf(lbLId.Text);
        }

    }
    private void getEmpRptPdf(string CiID)
    {
        DataTable dtCi = EmpManager.getCiRpt(CiID);
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename=BuyerInformation-(" + DateTime.Now.ToString("dd-MM-yyyy") + ").pdf");
        Document document = new Document(PageSize.A4, 40f, 30f, 20f, 20f);
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
        cell = new PdfPCell(new Phrase("Buyer Information", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
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
        if (dtCi.Rows.Count > 0)
        {
            clsClientInfo Sup = clsClientInfoManager.GetClientInfo(lbLId.Text);
            byte[] Nid = (byte[])Sup.NIDImage;
            byte[] Current = (byte[])Sup.CurrentImage;
            PdfPTable pdtphoto = new PdfPTable(2);
            pdtphoto.WidthPercentage = 100;
            if (Nid != null)
            {
                iTextSharp.text.Image gifpht = iTextSharp.text.Image.GetInstance(Nid);
                gifpht.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                gifpht.ScalePercent(40f);
                cell = new PdfPCell(gifpht);
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                cell.BorderColorLeft = BaseColor.BLACK;
                cell.BorderColorRight = BaseColor.BLACK;
                cell.BorderColorTop = BaseColor.BLACK;
                cell.BorderColorBottom = BaseColor.RED;
                pdtphoto.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                cell.BorderColorLeft = BaseColor.BLACK;
                cell.BorderColorRight = BaseColor.BLACK;
                cell.BorderColorTop = BaseColor.BLACK;
                cell.BorderColorBottom = BaseColor.BLACK;
                //cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtphoto.AddCell(cell);
            }
            if (Current != null)
            {
                iTextSharp.text.Image gifsig = iTextSharp.text.Image.GetInstance(Current);
                gifsig.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                gifsig.ScalePercent(40f);
                cell = new PdfPCell(gifsig);
                cell.BorderColorLeft = BaseColor.BLACK;
                cell.BorderColorRight = BaseColor.BLACK;
                cell.BorderColorTop = BaseColor.BLACK;
                cell.BorderColorBottom = BaseColor.BLACK;
                cell.HorizontalAlignment = 1;
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
                cell.BorderColorLeft = BaseColor.BLACK;
                cell.BorderColorRight = BaseColor.BLACK;
                cell.BorderColorTop = BaseColor.BLACK;
                cell.BorderColorBottom = BaseColor.BLACK;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtphoto.AddCell(cell);
            }

            float[] widthemp = new float[7] { 30, 5, 50, 5, 30, 5, 50 };
            PdfPTable pdtemp = new PdfPTable(widthemp);
            pdtemp.WidthPercentage = 100;

            cell = new PdfPCell(new Phrase("Buyer Name", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(":"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(((DataRow)dtCi.Rows[0])["ContactName"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(pdtphoto);
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            cell.BorderColorLeft = BaseColor.BLACK;
            cell.BorderColorRight = BaseColor.BLACK;
            cell.BorderColorTop = BaseColor.BLACK;
            cell.BorderColorBottom = BaseColor.RED;
            cell.Rowspan = 2;
            cell.Colspan = 4;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(new Phrase("E-mail", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(":"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(((DataRow)dtCi.Rows[0])["Email"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.FixedHeight = 18f;
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
            cell = new PdfPCell(FormatPhrase(((DataRow)dtCi.Rows[0])["COUNTRY_DESC"].ToString()));
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
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(":"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(((DataRow)dtCi.Rows[0])["NationalID"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);


            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 9f;
            cell.Colspan = 7;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(FormatPhrase("City"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(":"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(((DataRow)dtCi.Rows[0])["City"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(FormatPhrase("State"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(":"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(((DataRow)dtCi.Rows[0])["State"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 9f;
            cell.Colspan = 7;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase("Office Address"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(":"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(((DataRow)dtCi.Rows[0])["Address1"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(FormatPhrase("Residential Address"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(":"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(((DataRow)dtCi.Rows[0])["Address2"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);


            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 9f;
            cell.Colspan = 7;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);


            cell = new PdfPCell(FormatPhrase("Mobile"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(":"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(((DataRow)dtCi.Rows[0])["Mobile"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);


            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(FormatPhrase("Phone"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(":"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(((DataRow)dtCi.Rows[0])["Phone"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 9f;
            cell.Colspan = 7;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(FormatPhrase("Email"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(":"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(((DataRow)dtCi.Rows[0])["Email"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.MinimumHeight = 18f;
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
            cell = new PdfPCell(new Phrase("Buyer Name", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
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
            cell.BorderColorLeft = BaseColor.BLACK;

            cell.BorderColorRight = BaseColor.BLACK;
            cell.BorderColorTop = BaseColor.BLACK;
            cell.BorderColorBottom = BaseColor.RED;
            cell.Rowspan = 1;
            cell.Colspan = 4;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(new Phrase("E-mail", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
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

    protected void txtCustomer_TextChanged(object sender, EventArgs e)
    {
        DataTable dtCustomer = _aclsClientInfoManager.GetCustomerOnSearch("where UPPER(SearchName) = UPPER('" + txtCustomer.Text + "')", 0);
        if (dtCustomer.Rows.Count > 0)
        {
            hfCustomerID.Value = dtCustomer.Rows[0]["ID"].ToString();
            // txtCustomer.Text = dtCustomer.Rows[0]["ContactName"].ToString();
            //Session["Customer_COA"] = dtCustomer.Rows[0]["Gl_CoaCode"].ToString();
        }
    }
    protected void lbClear_Click(object sender, EventArgs e)
    {
        dgClient.DataSource = clsClientInfoManager.GetClientInfosGrid("");
        dgClient.DataBind();
        txtCustomer.Text = string.Empty;
        txtCustomer.Focus();
    }
    protected void lbSearch_Click(object sender, EventArgs e)
    {
        string[] words = txtCustomer.Text.Trim().Split('-');
        if (words.Length > 1)
        {
            DataTable dt = clsClientInfoManager.GetClientInfosGrid(" where ID=" + hfCustomerID.Value);
            dgClient.DataSource = dt;
            ViewState["SupHistory"] = dt;
            dgClient.DataBind();
        }
        else
        {
            DataTable dt =
                SupplierManager.GetSuppliers(
                    " where UPPER(isnull(code,'')+t1.contactName+isnull(country_desc,'')+isnull(Phone,'')) LIKE '%" +
                    txtCustomer.Text.ToUpper() + "%' ");
            dgClient.DataSource = dt;
            ViewState["SupHistory"] = dt;
            dgClient.DataBind();
        }
    }
}