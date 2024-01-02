using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Delve;

public partial class IteamCreate : System.Web.UI.Page
{

    private byte[] ItemsPhoto;
    ClsItemDetailsManager _aClsItemDetailsManager = new ClsItemDetailsManager();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            RefreshAll();
        }
    }

    private void RefreshAll()
    {

        chkActiveSetup.Checked = true;
        //chkFabrics.Checked = false;
        rdbItemType.SelectedValue = "0";
        txtName.Text = txtShortName.Text = lblItemSetupID.Text = txtCode.Text = "";
        DataTable dtSetupInfo = ClsItemDetailsManager.GetItemsSetupInfo("Where DeleteDate is null");
        dgItemSetupHistory.DataSource = dtSetupInfo;
        dgItemSetupHistory.DataBind();
        dgItemSetupHistory.Caption = "<h1>Total Product : " + (Convert.ToInt32(dgItemSetupHistory.Rows.Count)).ToString() + "</h1>";
        txtName.Focus();
        UP1.Update();
    }
  
    private void RefreshSetupInfo()
    {
        chkActiveSetup.Checked = true;
        txtName.Text = txtShortName.Text = "";
        DataTable dtSetupInfo = ClsItemDetailsManager.GetItemsSetupInfo("Where DeleteDate is null");
        dgItemSetupHistory.DataSource = dtSetupInfo;
        dgItemSetupHistory.DataBind();
        dgItemSetupHistory.Caption = "<h1>Total Product : " + (Convert.ToInt32(dgItemSetupHistory.Rows.Count)).ToString() + "</h1>";
        txtCode.Text = IdManager.GetDateTimeWiseSerial("", "Code", "Item");
        txtName.Focus();
    }

    protected void lblSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input Items Name...!!');", true);
            }
            else
            {

                var aClsItemDetailsInfoObj1 = ClsItemDetailsManager.GetShowItemSetUpDetails(lblItemSetupID.Text);
                if (aClsItemDetailsInfoObj1.Rows.Count <= 0)
                {

                    int count1 = IdManager.GetShowSingleValueInt("Count(*)",
                        "t1.Code", "Item t1", txtCode.Text);
                    if (count1 > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('Already this Code is Exist...!!');", true);
                        return;
                    }
                    ClsItemDetailsInfo aClsItemDetailsInfoObj = new ClsItemDetailsInfo();
                    txtCode.Text = IdManager.GetDateTimeWiseSerial("PI", "Code", "ItemSetup");
                    aClsItemDetailsInfoObj.ItemsCode = txtCode.Text;
                    aClsItemDetailsInfoObj.ItemsName = txtName.Text.Replace("'", "");
                    if (rdbItemType.SelectedValue != "")
                    {
                        aClsItemDetailsInfoObj.Type = rdbItemType.SelectedValue;
                    }
                    else
                    {
                        aClsItemDetailsInfoObj.Type = "0";
                    }
                    aClsItemDetailsInfoObj.ShortName = txtShortName.Text.Replace("'", "");
                    if (chkActiveSetup.Checked)
                    {
                        aClsItemDetailsInfoObj.Active = true;
                    }
                    else
                    {
                        aClsItemDetailsInfoObj.Active = false;
                    }

                    aClsItemDetailsInfoObj.ModelNo = "";
                    ClsItemDetailsManager.SaveItemsSetupInformation(aClsItemDetailsInfoObj, null);

                    RefreshSetupInfo();
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('Record is/are saved  successfully....!!');", true);
                }
                else
                {

                    ClsItemDetailsInfo aClsItemDetailsInfoObj = new ClsItemDetailsInfo();
                    aClsItemDetailsInfoObj.ItemId = lblItemSetupID.Text;
                    aClsItemDetailsInfoObj.ItemsCode = txtCode.Text;
                    aClsItemDetailsInfoObj.ItemsName = txtName.Text.Replace("'", "");
                    if (rdbItemType.SelectedValue != "")
                    {
                        aClsItemDetailsInfoObj.Type = rdbItemType.SelectedValue;
                    }
                    else
                    {
                        aClsItemDetailsInfoObj.Type = "0";
                    }

                    aClsItemDetailsInfoObj.ShortName = txtShortName.Text.Replace("'", "");

                    if (chkActiveSetup.Checked)
                    {
                        aClsItemDetailsInfoObj.Active = true;
                    }
                    else
                    {
                        aClsItemDetailsInfoObj.Active = false;
                    }
                    aClsItemDetailsInfoObj.ModelNo = "";
                    ClsItemDetailsManager.UpdateItemsSetupInformation(aClsItemDetailsInfoObj, null, null);
                    RefreshAll();
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('Record is/are Update successfully....!!');", true);
                }
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
    private void getColor()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("ID", typeof(string));
        dt.Columns.Add("ColorName", typeof(string));
        //DataRow dr = dt.NewRow();
        //dt.Rows.Add(dr);
        ViewState["ColorInfo"] = dt;
    }
    private void getSize()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("ID", typeof(string));
        dt.Columns.Add("SizeName", typeof(string));
        ViewState["SizeInfo"] = dt;
    }

    protected void ddlCatagory_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //ddlSubCatagory.DataSource = SubMajorCategoryManager.GetSubMajorCategories(ddlCatagory.SelectedValue);
            //ddlSubCatagory.DataTextField = "Name";
            //ddlSubCatagory.DataValueField = "ID";
            //ddlSubCatagory.DataBind();
            //ddlSubCatagory.Items.Insert(0, "");

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
 
    protected void dgHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[4].Attributes.Add("style", "display:none");

            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[4].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[4].Attributes.Add("style", "display:none");
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
    protected void dgHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {

        dgItemSetupHistory.DataSource = ClsItemDetailsManager.GetSetupItemInfo("");
        dgItemSetupHistory.PageIndex = e.NewPageIndex;
        dgItemSetupHistory.DataBind();
    }
    protected void dgHistory_SelectedIndexChanged(object sender, EventArgs e)
    {

        lblItemSetupID.Text = dgItemSetupHistory.SelectedRow.Cells[1].Text.Trim();

        DataTable Items = ClsItemDetailsManager.GetShowItemSetupDetails(lblItemSetupID.Text);
        if (Items != null)
        {
            txtCode.Text = Items.Rows[0]["Code"].ToString();
            txtName.Text = Items.Rows[0]["Name"].ToString();
            txtShortName.Text = Items.Rows[0]["ShortName"].ToString();
            try
            {
                rdbItemType.SelectedValue = Items.Rows[0]["Type"].ToString();
            }
            catch
            {
                rdbItemType.SelectedValue = null;
            }

        }
    }
   
    protected void BtnResetSetup_Click1(object sender, EventArgs e)
    {
        RefreshAll();
    }
    protected void btnDeleteSetup_Click(object sender, EventArgs e)
    {
        try
        {
            if (lblItemSetupID.Text != "")
            {
                string UserID = Session["userID"].ToString();

                ClsItemDetailsManager.DeleteItemSetupInfo(lblItemSetupID.Text, UserID);

                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record is/are Delete Succesfully');", true);
                RefreshAll();
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
  
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }

    //protected void btnSetupPrint_Click(object sender, EventArgs e)
    //{

    //    {
    //        string filename = "Item Information";
    //        Response.Clear();
    //        Response.ContentType = "application/pdf";
    //        Response.AddHeader("content-disposition", "attachment; filename=" + filename + DateTime.Now.ToString("dd/MM/yyyy") + ".pdf");
    //        Document document = new Document(PageSize.LEGAL.Rotate(), 50f, 50f, 40f, 40f);
    //        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
    //        document.Open();
    //        Rectangle page = document.PageSize;
    //        PdfPTable head = new PdfPTable(1);
    //        head.TotalWidth = page.Width - 50;
    //        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
    //        PdfPCell c = new PdfPCell(phrase);
    //        c.Border = Rectangle.NO_BORDER;
    //        c.VerticalAlignment = Element.ALIGN_BOTTOM;
    //        c.HorizontalAlignment = Element.ALIGN_RIGHT;
    //        head.AddCell(c);
    //        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

    //        PdfPCell cell;
    //        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
    //        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
    //        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
    //        gif.ScalePercent(30f);

    //        float[] titwidth = new float[2] { 10, 200 };
    //        PdfPTable dth = new PdfPTable(titwidth);
    //        dth.WidthPercentage = 100;

    //        cell = new PdfPCell(gif);
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 1;
    //        cell.Rowspan = 4;
    //        cell.BorderWidth = 0f;
    //        dth.AddCell(cell);
    //        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 1;
    //        cell.Colspan = 7;
    //        cell.BorderWidth = 0f;

    //        dth.AddCell(cell);
    //        string StatusChk = "";

    //        cell = new PdfPCell(new Phrase("Item Information", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 1;
    //        cell.Colspan = 7;
    //        cell.BorderWidth = 0f;

    //        dth.AddCell(cell);
    //        cell = new PdfPCell(new Phrase(" Address: " + ViewState["Address"], FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 1;
    //        cell.Colspan = 7;
    //        cell.BorderWidth = 0f;

    //        dth.AddCell(cell);
    //        cell = new PdfPCell(new Phrase(" Mobile Number : " + ViewState["Phone"], FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 1;
    //        cell.Colspan = 7;
    //        cell.BorderWidth = 0f;

    //        dth.AddCell(cell);
    //        document.Add(dth);
    //        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
    //        document.Add(line);



    //        PdfPTable dtempty = new PdfPTable(1);
    //        cell = new PdfPCell(FormatHeaderPhrase(""));
    //        cell.BorderWidth = 0f;
    //        cell.FixedHeight = 10f;
    //        dtempty.AddCell(cell);
    //        document.Add(dtempty);

    //        cell = new PdfPCell(FormatHeaderPhrase(""));
    //        cell.BorderWidth = 0f;
    //        cell.FixedHeight = 10f;
    //        dtempty.AddCell(cell);
    //        document.Add(dtempty);

    //        float[] widthdtl = new float[3] { 8, 30, 50 };
    //        PdfPTable pdtdtl = new PdfPTable(widthdtl);
    //        pdtdtl.WidthPercentage = 100;

    //        cell = new PdfPCell(FormatHeaderPhrase("SL"));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 0;
    //        cell.BorderColor = BaseColor.BLACK;
    //        cell.Rowspan = 2;
    //        cell.PaddingTop = 5;
    //        cell.PaddingBottom = 5;
    //        pdtdtl.AddCell(cell);

    //        cell = new PdfPCell(FormatHeaderPhrase("Code Number"));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 0;
    //        cell.BorderColor = BaseColor.BLACK;
    //        cell.Rowspan = 2;
    //        cell.PaddingTop = 5;
    //        cell.PaddingBottom = 5;
    //        pdtdtl.AddCell(cell);
    //        cell = new PdfPCell(FormatHeaderPhrase("Name"));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 0;
    //        cell.BorderColor = BaseColor.BLACK;
    //        cell.Rowspan = 2;
    //        cell.PaddingTop = 5;
    //        cell.PaddingBottom = 5;
    //        pdtdtl.AddCell(cell);
    //        //cell = new PdfPCell(FormatHeaderPhrase("Category"));
    //        //cell.HorizontalAlignment = 1;
    //        //cell.VerticalAlignment = 0;
    //        //cell.BorderColor = BaseColor.BLACK;
    //        //cell.Rowspan = 2;
    //        //cell.PaddingTop = 5;
    //        //cell.PaddingBottom = 5;
    //        //pdtdtl.AddCell(cell);
    //        //cell = new PdfPCell(FormatHeaderPhrase("Sub category"));
    //        //cell.HorizontalAlignment = 1;
    //        //cell.VerticalAlignment = 0;
    //        //cell.BorderColor = BaseColor.BLACK;
    //        //cell.Rowspan = 2;
    //        //cell.PaddingTop = 5;
    //        //cell.PaddingBottom = 5;
    //        //pdtdtl.AddCell(cell);

    //        //cell = new PdfPCell(FormatHeaderPhrase("Brand"));
    //        //cell.HorizontalAlignment = 1;
    //        //cell.VerticalAlignment = 0;
    //        //cell.BorderColor = BaseColor.BLACK;
    //        //cell.Rowspan = 2;
    //        //cell.PaddingTop = 5;
    //        //cell.PaddingBottom = 5;
    //        //pdtdtl.AddCell(cell);

    //        //cell = new PdfPCell(FormatHeaderPhrase("Size"));
    //        //cell.HorizontalAlignment = 1;
    //        //cell.VerticalAlignment = 0;
    //        //cell.BorderColor = BaseColor.BLACK;
    //        //cell.Rowspan = 2;
    //        //cell.PaddingTop = 5;
    //        //cell.PaddingBottom = 5;
    //        //pdtdtl.AddCell(cell);


    //        DataTable dt = ClsItemDetailsManager.GetSetupItemInfoForReport(lblItemSetupID.Text);
    //        //DataRow dr1 = dt.NewRow();
    //        //dt.Rows.Add(dr1);
    //        int Serial = 1;
    //        decimal totQty = 0;
    //        decimal tot = 0;
    //        foreach (DataRow dr in dt.Rows)
    //        {
    //            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
    //            cell.HorizontalAlignment = 1;
    //            cell.VerticalAlignment = 1;
    //            cell.BorderColor = BaseColor.BLACK;
    //            pdtdtl.AddCell(cell);

    //            cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
    //            cell.HorizontalAlignment = 1;
    //            cell.VerticalAlignment = 1;
    //            cell.BorderColor = BaseColor.BLACK;
    //            pdtdtl.AddCell(cell);

    //            cell = new PdfPCell(FormatPhrase(dr["Name"].ToString()));
    //            cell.HorizontalAlignment = 0;
    //            cell.VerticalAlignment = 1;
    //            cell.BorderColor = BaseColor.BLACK;
    //            pdtdtl.AddCell(cell);

    //            //cell = new PdfPCell(FormatPhrase(dr["Category"].ToString()));
    //            //cell.HorizontalAlignment = 1;
    //            //cell.VerticalAlignment = 1;
    //            //cell.BorderColor = BaseColor.BLACK;
    //            //pdtdtl.AddCell(cell);

    //            //cell = new PdfPCell(FormatPhrase(dr["SubCategory"].ToString()));
    //            //cell.HorizontalAlignment = 1;
    //            //cell.VerticalAlignment = 1;
    //            //cell.BorderColor = BaseColor.BLACK;
    //            //pdtdtl.AddCell(cell);

    //            //cell = new PdfPCell(FormatPhrase(dr["Brand"].ToString()));
    //            //cell.HorizontalAlignment = 1;
    //            //cell.VerticalAlignment = 1;
    //            //cell.BorderColor = BaseColor.BLACK;

    //            //pdtdtl.AddCell(cell);

    //            //cell = new PdfPCell(FormatPhrase(dr["ItemSize"].ToString()));
    //            //cell.HorizontalAlignment = 1;
    //            //cell.VerticalAlignment = 1;
    //            //cell.BorderColor = BaseColor.BLACK;

    //            //pdtdtl.AddCell(cell);

    //            //tot += Convert.ToDecimal(dr["Total"]);
    //            //totQty += Convert.ToDecimal(dr["qnty"]);

    //            Serial++;

    //        }

    //        document.Add(pdtdtl);

    //        document.Close();
    //        Response.Flush();
    //        Response.End();
    //    }
    //}
    protected void lbSearch_Click(object sender, EventArgs e)
    {
        DataTable dtSetupInfo = ClsItemDetailsManager.GetItemsSetupInfo(
            "Where DeleteBy is null and  upper(t1.Code+'-'+t1.Name) like upper('%" + txtSearchSetupItem.Text + "%')");

        dgItemSetupHistory.DataSource = dtSetupInfo;

        dgItemSetupHistory.DataBind();
        dgItemSetupHistory.Caption = "<h1>Total Product : " +
                                     (Convert.ToInt32(dgItemSetupHistory.Rows.Count)).ToString() + "</h1>";
    }

    protected void lbClear_Click(object sender, EventArgs e)
    {

        txtSearchSetupItem.Text = "";
        DataTable dtSetupInfo = ClsItemDetailsManager.GetItemsSetupInfo("");
        dgItemSetupHistory.DataSource = dtSetupInfo;
        dgItemSetupHistory.DataBind();
        dgItemSetupHistory.Caption = "<h1>Total Product : " +
                                     (Convert.ToInt32(dgItemSetupHistory.Rows.Count)).ToString() + "</h1>";
    }

}