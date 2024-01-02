using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Delve;
using iTextSharp.text.pdf.draw;
using System.Data.SqlClient;
using System.Drawing;

public partial class StockItemsDetails : System.Web.UI.Page
{
    private static Permis per;
    ClsItemDetailsManager _aClsItemDetailsManager=new ClsItemDetailsManager();
    MajorCategoryManager _aMajorCategoryManager=new MajorCategoryManager();
    SubMajorCategoryManager _aSubMajorCategoryManager=new SubMajorCategoryManager();
    BrandManage _aBrandManage=new BrandManage();
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
            DataTable dtItems = _aClsItemDetailsManager.getShowItemsInfo(hfItemsID.Value, hfCatagoryID.Value,
                hfSubCatagoryID.Value, hfBrand.Value, ddlDepart.SelectedValue, txtFormDate.Text, txtToDate.Text,"",txtItemDesc.Text);
            if (dtItems.Rows.Count > 0)
            {
                dgItems.DataSource = dtItems;
                ViewState["STK"] = dtItems;
                dgItems.DataBind();
                ShowFooterTotal();
            }
            Session["ItemIDOnly"] = "0";
            ddlDepart.DataSource = SizeManager.GetShowDeptDetails();
            ddlDepart.DataTextField = "Dept_Name";
            ddlDepart.DataValueField = "ID";
            ddlDepart.DataBind();
            ddlDepart.Items.Insert(0, "");
            DataTable dtBranch = ClsItemDetailsManager.GetBranchInfo();
            util.PopulationDropDownList(ddlBranch, "Name", "ID", dtBranch);
            txtName.Focus();
        }
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void txtName_TextChanged(object sender, EventArgs e)
    {
        DataTable dtItem = _aClsItemDetailsManager.GetItemsDetailsOnSearch(txtName.Text.ToUpper());
        if (dtItem.Rows.Count > 0)
        {
            hfItemsID.Value = dtItem.Rows[0]["ID"].ToString();
            txtName.Text = dtItem.Rows[0]["Name"].ToString();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                "alert('items not found..!!');", true);
        }
    }
    protected void txtCatagory_TextChanged(object sender, EventArgs e)
    {
        DataTable dtCatagory = _aMajorCategoryManager.GetCatagoryOnSearch(txtCatagory.Text.ToUpper());
        if (dtCatagory.Rows.Count > 0)
        {
            txtCatagory.Text = dtCatagory.Rows[0]["Name"].ToString();
            hfCatagoryID.Value = dtCatagory.Rows[0]["ID"].ToString();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                "alert('category not found..!!');", true);
        }
    }
    protected void txtSubCatagory_TextChanged(object sender, EventArgs e)
    {
        DataTable dtSubCatagory = _aSubMajorCategoryManager.GetSubCatagoryOnSearch(txtSubCatagory.Text.ToUpper());
        if (dtSubCatagory.Rows.Count > 0)
        {
            txtSubCatagory.Text = dtSubCatagory.Rows[0]["Name"].ToString();
            hfSubCatagoryID.Value = dtSubCatagory.Rows[0]["ID"].ToString();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                "alert('sub-category not found..!!');", true);
        }
    }
    protected void txtBrand_TextChanged(object sender, EventArgs e)
    {
        DataTable dtBrand = _aBrandManage.GetBrandOnSearch(txtBrand.Text.ToUpper());
        if (dtBrand.Rows.Count > 0)
        {
            txtBrand.Text = dtBrand.Rows[0]["BrandName"].ToString();
            hfBrand.Value = dtBrand.Rows[0]["ID"].ToString();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                "alert('brand not found..!!');", true);
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtFormDate.Text))
        {
            if (!string.IsNullOrEmpty(txtToDate.Text))
            {
                if (DataManager.DateEncode(txtFormDate.Text) > DataManager.DateEncode(txtToDate.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                        "alert('From date not upper then to date.!!');", true);
                    return;
                }
            }
        }
        DataTable dtItems =new DataTable();
        if (string.IsNullOrEmpty(ddlBranch.SelectedValue))
        {
             dtItems = _aClsItemDetailsManager.getShowItemsInfo(hfItemsID.Value, hfCatagoryID.Value,
                hfSubCatagoryID.Value, hfBrand.Value, ddlDepart.SelectedValue, txtFormDate.Text, txtToDate.Text, hfItemsIDOnly.Value, txtItemDesc.Text);
        }
        else
        {
            dtItems = _aClsItemDetailsManager.getShowBranchItemsInfo(hfItemsID.Value, hfCatagoryID.Value,
                hfSubCatagoryID.Value, hfBrand.Value, ddlDepart.SelectedValue, txtFormDate.Text, txtToDate.Text, hfItemsIDOnly.Value, txtItemDesc.Text,ddlBranch.SelectedValue);
        }
        //if (dtItems.Rows.Count > 0)
        //{
            dgItems.DataSource = dtItems;
            ViewState["STK"] = dtItems;
            dgItems.DataBind();
        //}
        //else
        //{
        //    dgItems.DataSource = dtItems;
        //    ViewState["STK"] = dtItems;
        //    dgItems.DataBind();
        //}
    }

    protected void ddlDepart_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable dtItems = _aClsItemDetailsManager.getShowItemsInfo(hfItemsID.Value, hfCatagoryID.Value,
            hfSubCatagoryID.Value, hfBrand.Value, ddlDepart.SelectedValue, txtFormDate.Text, txtToDate.Text, hfItemsIDOnly.Value,txtItemDesc.Text);
        if (dtItems.Rows.Count > 0)
        {
            dgItems.DataSource = dtItems;
            ViewState["STK"] = dtItems;
            dgItems.DataBind();
            ShowFooterTotal();
        }
    }
    private void ShowFooterTotal()
    {
      
        decimal totQty = 0;
        DataTable dtStock = (DataTable) ViewState["STK"];
        totQty = Convert.ToDecimal(dtStock.Compute("Sum(TotalClosingStock)", ""));
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
        TableCell cell;
        cell = new TableCell();
        cell.Text = "<h3>Total Stock</h3>";
        cell.ColumnSpan = 9;
        cell.Font.Bold = true;
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        cell = new TableCell();
        cell.Text ="<h3>"+ totQty.ToString("N2")+"</h3>";
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
      
        row.Font.Bold = true;
        row.BackColor = System.Drawing.Color.LightGray;
        if (dgItems.Rows.Count > 0)
        {
            dgItems.Controls[0].Controls.Add(row);
        }
        
    }
    protected void dgItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgItems.DataSource = ViewState["STK"];
        dgItems.PageIndex = e.NewPageIndex;
        dgItems.DataBind();
    }

    protected void dgItems_SelectedIndexChanged(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "newWindow",
            "window.open('frmImageView.aspx?ID=" + dgItems.SelectedRow.Cells[7].Text + " &ItemsName=" +
            dgItems.SelectedRow.Cells[1].Text +
            "','_blank','status=1,toolbar=0,menubar=0,location=1,top=250,left=250px,width=500px,height=250px,directories=no,status=no, linemenubar=no,scrollbars=no,resizable=no ,modal=yes');",
            true);
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename='Stock-Items'.pdf");
        Document document = new Document(PageSize.A4.Rotate(), 50f, 50f, 40f, 40f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();      

        PdfPCell cell;
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
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
       // cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
       // cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        dth.AddCell(cell);

        string Heading = "Total Items Stock\n";
        if (!string.IsNullOrEmpty(ddlDepart.SelectedValue))
        {
            Heading += "Department : " + ddlDepart.SelectedItem.Text+"\n ";
        }
        if (!string.IsNullOrEmpty(txtName.Text))
        {
            Heading += "Category : " + txtCatagory.Text + "\n";
        }
        if (!string.IsNullOrEmpty(txtBrand.Text))
        {
            Heading += "Brand : " + txtBrand.Text + "\n";
        }
        if (!string.IsNullOrEmpty(txtFormDate.Text))
        {
            Heading += "Expiration date : " + txtFormDate.Text + " To "+txtToDate.Text;
        }
        cell = new PdfPCell(new Phrase(Heading, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
       // cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        float[] widthdtl = new float[12] {10, 15, 50,20, 20, 20, 20, 20, 20, 20,20,20};
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 2;

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 15f;
        cell.Border = 0;
        cell.Colspan = 12;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Department"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Items Name & Code"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Brand"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Category"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Sub category"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Expiration date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Cost price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Sales price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Closing stock"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Total CostPrice Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Total SalePrice Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

      
        int Serial = 1;
        decimal totOPStk = 0; decimal totOpAmt = 0; decimal totCloseStk = 0; decimal totCloseAmt = 0, totCost=0,TotalSles=0,totalCostPrice=0,totalSalesPrice=0;
        DataTable dtdtl = (DataTable)ViewState["STK"];
        foreach (DataRow dr in dtdtl.Rows)
        {            
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1; 
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell = new PdfPCell(FormatPhrase(dr["Dept_Name"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Items_Code_Name"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;         
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Category"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;           
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["SubCategory"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;          
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["ExpireDate"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["CostPrice"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            totCost += Convert.ToDecimal(dr["CostPrice"].ToString());
            cell = new PdfPCell(FormatPhrase(dr["ItemsPrice"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            TotalSles += Convert.ToDecimal(dr["ItemsPrice"].ToString());

            cell = new PdfPCell(FormatPhrase(dr["ClosingStock"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            totCloseStk += Convert.ToDecimal(dr["ClosingStock"]);


            cell = new PdfPCell(FormatPhrase(dr["totalCostPrice"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            totalCostPrice += Convert.ToDecimal(dr["totalCostPrice"]);


            cell = new PdfPCell(FormatPhrase(dr["totalSalesPrice"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            totalSalesPrice += Convert.ToDecimal(dr["totalSalesPrice"]);



           
        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 7;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(totCost.ToString("N2")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(TotalSles.ToString("N2")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totCloseStk.ToString("N2")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totalCostPrice.ToString("N2")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(totalSalesPrice.ToString("N2")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);        
        document.Close();
        Response.Flush();
        Response.End();
    }
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }

    protected void dgItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[7].Attributes.Add("style", "display:none");
                e.Row.Cells[5].Attributes.Add("style", "display:none");
                //e.Row.Cells[8].Attributes.Add("style", "display:none");
                //e.Row.Cells[9].Attributes.Add("style", "display:none");
            }
        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }
    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgItems.DataSource = ViewState["ddt"];
        dgItems.PageIndex = e.NewPageIndex;
        dgItems.DataBind();
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow )
            {
                e.Row.Cells[0].Attributes.Add("style", "display:none");
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                try
                {
                    if (!string.IsNullOrEmpty(e.Row.Cells[8].Text))
                    {
                        int totdate = (DataManager.DateEncode(e.Row.Cells[8].Text) -
                                       DataManager.DateEncode(DateTime.Now.ToString("dd/MM/yyyy"))).Days;
                        if (totdate < 0)
                        {
                            e.Row.Cells[8].BackColor = Color.OrangeRed;
                            e.Row.Cells[8].ForeColor = Color.White;
                        }
                        else if (totdate > 0 && totdate <= 7)
                        {
                            e.Row.Cells[8].BackColor = Color.Yellow;
                        }
                    }
                }
                catch
                {

                }

            }
            else if (e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].Attributes.Add("style", "display:none");
                e.Row.Cells[1].Attributes.Add("style", "display:none");
            }
        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }

    protected void txtNameOnly_TextChanged(object sender, EventArgs e)
    {
        DataTable dtSetupInfo = ClsItemDetailsManager.GetItemsSetupInfo(
           "Where DeleteBy is null and  upper(t1.Code+'-'+t1.Name) like upper('%" + txtNameOnly.Text + "%')");
        if (dtSetupInfo.Rows.Count > 0)
        {
            Session["ItemIDOnly"] = dtSetupInfo.Rows[0]["ID"].ToString();
            hfItemsIDOnly.Value = dtSetupInfo.Rows[0]["ID"].ToString();
            txtNameOnly.Text = dtSetupInfo.Rows[0]["Name"].ToString();
        }
        else
        {
            Session["ItemIDOnly"] ="0";
        }

    }
    //protected void txtSupplierSearch_TextChanged(object sender, EventArgs e)
    //{
    //    DataTable dtSupplier = PurchaseVoucherManager.GetSupplierInfo(txtSupplierSearch.Text);
    //    if (dtSupplier.Rows.Count > 0)
    //    {
           
    //        txtSupplierSearch.Text = dtSupplier.Rows[0]["SupplierSearch"].ToString();
           
    //        hfSupplierID.Value = dtSupplier.Rows[0]["ID"].ToString();
    //        txtSupplierSearch.Text = dtSupplier.Rows[0]["Name"].ToString();
      
    //    }
    //    else
    //    {
    //        ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Not Found Supplier.!!');", true);
    //        txtSupplierSearch.Text = hfSupplierID.Value = "";
    //        txtSupplierSearch.Focus();
    //    }
    //}
    protected void txtItemDesc_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string[] splitCode = txtItemDesc.Text.Trim().Split('-');
            DataTable dt = ItemManager.GetItemsBercode(txtItemDesc.Text,splitCode.Length);

            if (dt.Rows.Count > 0)
            {
               
                txtItemDesc.Text = dt.Rows[0]["Barcode"].ToString();

               

            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Not Found Barcode.!!');", true);
                txtItemDesc.Text ="";
                txtItemDesc.Focus();
            }
        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }
}