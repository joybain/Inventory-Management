using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;
using System.Data;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.draw;

public partial class rptItemsAndStock : System.Web.UI.Page
{
    private static Permis per;
    ClsItemDetailsManager _aClsItemDetailsManager = new ClsItemDetailsManager();
    MajorCategoryManager _aMajorCategoryManager = new MajorCategoryManager();
    SubMajorCategoryManager _aSubMajorCategoryManager = new SubMajorCategoryManager();
    BrandManage _aBrandManage = new BrandManage();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlDepart.DataSource = SizeManager.GetShowDeptDetails();
            ddlDepart.DataTextField = "Dept_Name";
            ddlDepart.DataValueField = "ID";
            ddlDepart.DataBind();
            ddlDepart.Items.Insert(0, "");
            UP1.Update();
            UP2.Update();
            lblDate.Visible = false;
            Session["Dept_Search"] = "";
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

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (rbAttType.SelectedValue.Equals("1"))
        {
            DataTable dtItems = _aClsItemDetailsManager.getShowItemsInfoDetails(hfItemsID.Value, hfCatagoryID.Value,
                hfSubCatagoryID.Value, hfBrand.Value, ddlDepart.SelectedValue, txtFormDate.Text, txtToDate.Text);
            ViewState["STK"] = dtItems;

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename=" + rbAttType.SelectedItem.Text.Replace(" ","") + ".pdf");
            Document document = new Document(PageSize.A4.Rotate(), 20f, 20f, 40f, 20f);
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
            cell = new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);

            string Heading = "Total Items Stock ( " + dtItems.Rows.Count.ToString() + " )\n";
            if (!string.IsNullOrEmpty(ddlDepart.SelectedValue))
            {
                Heading += "Department : " + ddlDepart.SelectedItem.Text + "\n ";
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
                Heading += "Expiration date : " + txtFormDate.Text + " To " + txtToDate.Text;
            }

            cell = new PdfPCell(new Phrase(Heading,
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 30f;
            dth.AddCell(cell);
            document.Add(dth);
            LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
            document.Add(line);

            float[] widthdtl = new float[10] {10, 15, 20, 50, 20, 15, 20, 20, 20, 20};
            PdfPTable pdtdtl = new PdfPTable(widthdtl);
            pdtdtl.WidthPercentage = 100;
            pdtdtl.HeaderRows = 2;

            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 15f;
            cell.Border = 0;
            cell.Colspan = 10;
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

            cell = new PdfPCell(FormatHeaderPhrase("Code"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Items Name"));
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
            cell = new PdfPCell(FormatHeaderPhrase("Size"));
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

            cell = new PdfPCell(FormatHeaderPhrase("M.O.U"));
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
            

            int Serial = 1;
            decimal totOPStk = 0;
            decimal totOpAmt = 0;
            decimal totCloseStk = 0;
            decimal totCloseAmt = 0;
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

                cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Name"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["SizeName"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["CategoryName"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["SubCategoryName"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);


                cell = new PdfPCell(FormatPhrase(dr["UMOName"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["UnitPrice"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
               
            }
            document.Add(pdtdtl);
            document.Close();
            Response.Flush();
            Response.End();

        }
        else
        {
            DataTable dtItems = _aClsItemDetailsManager.getShowItemsInfo(hfItemsID.Value, hfCatagoryID.Value,
                hfSubCatagoryID.Value, hfBrand.Value, ddlDepart.SelectedValue, txtFormDate.Text, txtToDate.Text,"","");
                ViewState["STK"] = dtItems;
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename=" + rbAttType.SelectedItem.Text.Replace(" ", "") + ".pdf");
            Document document = new Document(PageSize.A4.Rotate(), 50f, 50f, 40f, 40f);
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();

            PdfPCell cell;
            byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
            iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
            gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
            gif.ScalePercent(8f);

            float[] titwidth = new float[2] {10, 200};
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

            cell = new PdfPCell(gif);
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);

            string Heading = "Total Items Stock\n";
            if (!string.IsNullOrEmpty(ddlDepart.SelectedValue))
            {
                Heading += "Department : " + ddlDepart.SelectedItem.Text + "\n ";
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
                Heading += "Expiration date : " + txtFormDate.Text + " To " + txtToDate.Text;
            }

            cell = new PdfPCell(new Phrase(Heading,
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 30f;
            dth.AddCell(cell);
            document.Add(dth);
            LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
            document.Add(line);

            float[] widthdtl = new float[10] {10, 15, 50, 20, 15, 20, 20, 20, 20, 20};
            PdfPTable pdtdtl = new PdfPTable(widthdtl);
            pdtdtl.WidthPercentage = 100;
            pdtdtl.HeaderRows = 2;

            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 15f;
            cell.Border = 0;
            cell.Colspan = 10;
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
            cell = new PdfPCell(FormatHeaderPhrase("Size"));
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


            int Serial = 1;
            decimal totOPStk = 0;
            decimal totOpAmt = 0;
            decimal totCloseStk = 0;
            decimal totCloseAmt = 0;
            DataTable dtdtl = (DataTable) ViewState["STK"];
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

                cell = new PdfPCell(FormatPhrase(dr["SizeName"].ToString()));
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

                cell = new PdfPCell(FormatPhrase(dr["ItemsPrice"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["ClosingStock"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                totCloseStk += Convert.ToDecimal(dr["ClosingStock"]);
            }

            cell = new PdfPCell(FormatHeaderPhrase("Total"));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 9;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(totCloseStk.ToString("N2")));
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
    }
    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9));
    }
    protected void rbAttType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rbAttType.SelectedValue.Trim().Equals("1"))
        {
            lblDate.Visible = false;
        }
        else
        {
            lblDate.Visible = true;
        }
    }
    protected void ddlDepart_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlDepart.SelectedItem.Text))
            Session["Dept_Search"] = "";
        else
        Session["Dept_Search"] = ddlDepart.SelectedValue;
    }
}