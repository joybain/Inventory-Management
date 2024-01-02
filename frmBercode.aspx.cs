using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;
using System.Data;
using Delve;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.IO;
using iTextSharp.text.html.simpleparser;
using System.Text;
using BarcodeLib;
using sales;
using Image = System.Drawing.Image;

public partial class frmBercode : System.Web.UI.Page
{
    public static Permis per;
    //public static ReportDocument rpt;
    //public static decimal priceDr = 0;
    //public static decimal priceCr = 0;
    private int dtlRecordNum = 0;
    private BarcodeLib.Barcode barcode = new BarcodeLib.Barcode();
    SupplierManager _aSupplierManager=new SupplierManager();
    PurchaseVoucherManager _aPurchaseVoucherManager=new PurchaseVoucherManager();
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
                    string wnot = "";
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            Session["userlevel"] = int.Parse(dReader["user_grp"].ToString());
                            //Session["dept"] = dReader["dept"].ToString();
                            wnot = "Welcome " + dReader["description"].ToString();
                        }
                        Session["wnote"] = wnot;
                       // Session["LoginCountry"] = dReader["LoginCountry"].ToString();
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        //cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type from gl_set_of_books where book_name='" + Session["book"].ToString() + "' ";
                        //if (Convert.ToInt32(dReader["UserType"].ToString()) == 2)
                        //{

                        //    Session["bookMAN"] = "MAN";
                        //}
                        //else
                        //{
                        //    Session["bookMAN"] = Session["book"].ToString();
                        //}

                        cmd.CommandText =
                            "Select book_desc,company_address1,company_address2,separator_type,ShotName from gl_set_of_books where book_name='" +
                            Session["bookMAN"] + "' ";

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
           
            getEmptyDtl();
        }
    }
    private void getEmptyDtl()
    {
        dgPODetailsDtl.Visible = true;
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("ID", typeof(string));
        dtDtlGrid.Columns.Add("item_code", typeof(string));
        dtDtlGrid.Columns.Add("item_desc", typeof(string));     
        dtDtlGrid.Columns.Add("item_rate", typeof(string));
        dtDtlGrid.Columns.Add("qnty", typeof(string));
        dtDtlGrid.Columns.Add("StkQty", typeof(string));
        dtDtlGrid.Columns.Add("Tax", typeof(string));
        dtDtlGrid.Columns.Add("Barcode", typeof(string));   
        DataRow dr = dtDtlGrid.NewRow();
        dtDtlGrid.Rows.Add(dr);
        dgPODetailsDtl.DataSource = dtDtlGrid;
        ViewState["purdtl"] = dtDtlGrid;
        dgPODetailsDtl.DataBind();
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        try
        {
            string filename = "Barcode";
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
            Document document = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();
            PdfPCell cell;

            float[] widthdtl = new float[6] { 16, 16, 16, 16, 16, 16 };
            PdfPTable pdtdtl = new PdfPTable(widthdtl);
            pdtdtl.WidthPercentage = 100;


            barcode.Alignment = AlignmentPositions.CENTER;
            int W = 550;
            int H = 160;

            TYPE type = TYPE.CODE128;
            barcode.IncludeLabel = false;
            barcode.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
            barcode.LabelPosition = LabelPositions.BOTTOMCENTER;
            DataTable dt = (DataTable)ViewState["purdtl"];
            decimal totalCell = 0;
            int Countcell = 0;

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["item_code"].ToString() != "")
                {
                    string BarcodeID = dr["Barcode"].ToString();
                    Image generatedBarcode = barcode.Encode(type, BarcodeID, Color.Black, Color.White, W, H);
                    MemoryStream stream = new MemoryStream();
                    generatedBarcode.Save(stream, ImageFormat.Png);

                    byte[] logo = stream.ToArray();
                    iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
                    gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                    gif.ScalePercent(11f);

                    decimal tot = Convert.ToDecimal(dr["qnty"].ToString());
                    totalCell = totalCell + tot;
                    for (int i = 0; i < Convert.ToInt32(tot); i = i + 1)
                    {

                        string gg = Session["org"].ToString();
                        var subTable = new PdfPTable(new float[2] {60, 20});
                        subTable.AddCell(new PdfPCell(FormatPhrase8(gg))
                            {Colspan = 2, Border = 0, HorizontalAlignment = 1});


                        //subTable.AddCell(new PdfPCell(FormatPhrase7(dr["item_desc"].ToString())) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                       subTable.AddCell(new PdfPCell(FormatPhrase7(dr["item_desc"].ToString())) { Colspan = 2, Border = 0, HorizontalAlignment = 1, FixedHeight = 18 });

                        subTable.AddCell(new PdfPCell(FormatPhrase7(dr["item_code"].ToString()))
                            {Colspan = 2, Border = 0, HorizontalAlignment = 1});

                      

                        subTable.AddCell(new PdfPCell(gif) {Colspan = 2, Border = 0, HorizontalAlignment = 1});
                        subTable.AddCell(
                            new PdfPCell(FormatPhrase7("TK :" + Convert.ToDecimal(dr["item_rate"]).ToString("N2")))
                                {Colspan = 2, Border = 0, HorizontalAlignment = 1});

                        if (i == 18)
                        {

                        }

                        PdfPCell cellBody = new PdfPCell(subTable);
                        cellBody.BorderWidth = 0; //<--- This is what sets the border for the nested table
                        pdtdtl.AddCell(cellBody);
                        Countcell++;
                        if (Countcell == 6)
                        {
                            cell = new PdfPCell(FormatPhrase(""));
                            cell.Border = 0;
                            cell.Colspan = 6;
                            cell.FixedHeight = 7f;
                            pdtdtl.AddCell(cell);
                            Countcell = 0;
                        }
                    }

                }
            }

            if (Convert.ToInt32(totalCell % 6) != 0)
            {
                cell = new PdfPCell(FormatPhrase(""));
                cell.Border = 0;
                cell.Colspan = 6 - Convert.ToInt32(totalCell % 6);
                //cell.FixedHeight = 15f;
                pdtdtl.AddCell(cell);
            }

            document.Add(pdtdtl);
            document.Close();
            Response.Flush();
            Response.End();
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
    private static Phrase FormatPhrase9(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9));
    }
    private static Phrase FormatPhrase8(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8));
    }

    private static Phrase FormatPhrase7(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7));
    }
    private static Phrase FormatPhrase6(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 6));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }

    protected void txtItemDesc_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow) ((TextBox) sender).NamingContainer;
        DataTable dtdtl = (DataTable) ViewState["purdtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        string[] splitCode = ((TextBox)gvr.FindControl("txtItemDesc")).Text.ToUpper().Split('-');
        DataTable dt = ItemManager.GetItemsBercode(((TextBox)gvr.FindControl("txtItemDesc")).Text.ToUpper(), splitCode.Length);
        if (dt.Rows.Count > 0)
        {
            
            DataRow[] rows = dtdtl.Select("ID = " + ((DataRow) dt.Rows[0])["ID"].ToString() + " ");
            // DataRow drr = dtdtl.AsEnumerable().SingleOrDefault(r => r.Field<int?>("ItemsID") ==Convert.ToInt32(((DataRow)dt.Rows[0])["ItemsID"].ToString()));
            if (rows != null)
            {
                if (rows.Length > 0)
                {
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                        "alert('This items already added...!!!');", true);
                    ((TextBox) gvr.FindControl("txtItemDesc")).Text = "";
                    ((TextBox) gvr.FindControl("txtItemDesc")).Focus();
                    return;
                }
            }

            dtdtl.Rows.Remove(dr);
            dr = dtdtl.NewRow();
            dr["ID"] = ((DataRow) dt.Rows[0])["ID"].ToString();
            dr["Barcode"] = ((DataRow)dt.Rows[0])["Barcode"].ToString();
            dr["item_desc"] = ((DataRow) dt.Rows[0])["Items_Code_Name"].ToString();
            dr["item_code"] = ((DataRow) dt.Rows[0])["Code"].ToString();
            dr["item_rate"] = ((DataRow) dt.Rows[0])["ItemsPrice"].ToString();
            dr["Tax"] = "0";
            
            dr["StkQty"] = ((DataRow) dt.Rows[0])["ClosingStock"].ToString();
            dr["qnty"] = "0";
            dtdtl.Rows.InsertAt(dr, gvr.DataItemIndex);

            string found = "";
            foreach (DataRow drd in dtdtl.Rows)
            {
                if (drd["item_code"].ToString() == "" && drd["item_desc"].ToString() == "")
                {
                    found = "Y";
                }
            }
            if (found == "")
            {
                DataRow drd = dt.NewRow();
                dt.Rows.Add(drd);
            }
        }

        dgPODetailsDtl.DataSource = dtdtl;
        dgPODetailsDtl.DataBind();
        //ShowFooterTotal();
        ((TextBox) dgPODetailsDtl.Rows[dgPODetailsDtl.Rows.Count - 1].FindControl("txtQnty")).Focus();
    }

    protected void dgPurDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.Footer)
            {                
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[6].Attributes.Add("style", "display:none");

                e.Row.Cells[7].Attributes.Add("style", "display:none");
                
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
    protected void dgPurDtl_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["purdtl"] != null)
        {
            DataTable dtDtlGrid = (DataTable)ViewState["purdtl"];
            dtDtlGrid.Rows.RemoveAt(dgPODetailsDtl.Rows[e.RowIndex].DataItemIndex);
            if (dtDtlGrid.Rows.Count > 0)
            {
                string found = "";
                foreach (DataRow drf in dtDtlGrid.Rows)
                {
                    if (drf["item_code"].ToString() == "" && drf["item_desc"].ToString() == "")
                    {
                        found = "Y";
                    }
                }
                if (found == "")
                {
                    DataRow dr = dtDtlGrid.NewRow();
                    dtDtlGrid.Rows.Add(dr);
                }
                dgPODetailsDtl.DataSource = dtDtlGrid;
                dgPODetailsDtl.DataBind();
            }
            else
            {
                getEmptyDtl();
            }           
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Your session is over. Try it again!!');", true);
        }
    }
    protected void txtQnty_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        DataTable dt = (DataTable)ViewState["purdtl"];
        // DataTable dt = ItemManager.GetItems(((TextBox)gvr.FindControl("txtItemDesc")).Text);
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[gvr.DataItemIndex];        
            dr["qnty"] = ((TextBox)gvr.FindControl("txtQnty")).Text;
        }
        string found = "";
        foreach (DataRow drd in dt.Rows)
        {
            if (drd["item_code"].ToString() == "" && drd["item_desc"].ToString() == "")
            {
                found = "Y";
            }
        }
        if (found == "")
        {
            DataRow drd = dt.NewRow();
            dt.Rows.Add(drd);
        }
        dgPODetailsDtl.DataSource = dt;
        dgPODetailsDtl.DataBind();      
        ((TextBox)dgPODetailsDtl.Rows[dgPODetailsDtl.Rows.Count - 1].FindControl("txtItemDesc")).Focus();
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }

    private void Bercode()
    {
        string filename = "Bercode";
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
        Document document = new Document(PageSize.A4, 50f, 50f, 40f, 40f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        PdfPCell cell;

        float[] widthdtl = new float[3] { 30, 30, 30 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;


        barcode.Alignment = BarcodeLib.AlignmentPositions.CENTER;
        int W = 550;
        int H = 160;

        BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128;
        barcode.IncludeLabel = false;
        barcode.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
        barcode.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
        DataTable dt = (DataTable)ViewState["purdtl"];

        foreach (DataRow dr in dt.Rows)
        {
            if (dr["item_code"].ToString() != "")
            {
                decimal tot = Convert.ToDecimal(dr["qnty"].ToString());
                for (int i = 0; i < Convert.ToInt32(tot); i = i + 3)
                {

                    System.Drawing.Image generatedBarcode = barcode.Encode(type, dr["item_code"].ToString(), Color.Black, Color.White, W, H);
                    System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    generatedBarcode.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                    byte[] logo = stream.ToArray();
                    iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
                    gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                    gif.ScalePercent(20f);
                    //new PdfPCell(FormatPhrase(""));
                    var subTable = new PdfPTable(new float[2] { 60, 20 });
                    //subTable.AddCell(new PdfPCell(FormatPhrase("Netsoft Solution Ltd.")) { Border = 0, HorizontalAlignment = 1 });
                    //subTable.AddCell(new PdfPCell(FormatPhrase(dr["Tax"].ToString())) { Border = 0, HorizontalAlignment = 0 });
                    //subTable.AddCell(new PdfPCell(gif) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                    //subTable.AddCell(new PdfPCell(FormatPhrase(dr["item_desc"].ToString())) { Border = 0, HorizontalAlignment = 1 });
                    //subTable.AddCell(new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK")) { Border = 0, HorizontalAlignment = 0 });                             
                    ////pdtdtl.AddCell(subTable);
                    subTable.AddCell(new PdfPCell(FormatPhrase(Session["org"].ToString().Substring(0, 5) + "    " + Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK")) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                    // subTable.AddCell(new PdfPCell(FormatPhrase()) { Border = 0, HorizontalAlignment = 0 });
                    subTable.AddCell(new PdfPCell(gif) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                    subTable.AddCell(new PdfPCell(FormatPhrase(dr["item_code"].ToString() + "-" + dr["item_desc"].ToString())) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });

                    PdfPCell cellBody = new PdfPCell(subTable);
                    cellBody.BorderWidth = 0; //<--- This is what sets the border for the nested table
                    pdtdtl.AddCell(cellBody);

                    if (i + 2 <= Convert.ToInt32(tot))
                    {
                        subTable = new PdfPTable(new float[2] { 60, 20 });
                        subTable.AddCell(new PdfPCell(FormatPhrase(Session["org"].ToString().Substring(0, 5) + "    " + Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK")) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                        //subTable.AddCell(new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK")) { Border = 0, HorizontalAlignment = 0 });
                        subTable.AddCell(new PdfPCell(gif) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                        subTable.AddCell(new PdfPCell(FormatPhrase(dr["item_code"].ToString() + "-" + dr["item_desc"].ToString())) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                        cellBody = new PdfPCell(subTable);
                        cellBody.BorderWidth = 0; //<--- This is what sets the border for the nested table
                        pdtdtl.AddCell(cellBody);
                    }
                    else
                    {
                        cell = new PdfPCell(FormatPhrase(""));
                        cell.Border = 0;
                        pdtdtl.AddCell(cell);
                    }
                    if (i + 3 <= Convert.ToInt32(tot))
                    {
                        subTable = new PdfPTable(new float[2] { 60, 20 });
                        subTable.AddCell(new PdfPCell(FormatPhrase(Session["org"].ToString().Substring(0, 5) + "    " + Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK")) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                        // subTable.AddCell(new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["item_rate"]).ToString("N0") + "TK")) { Border = 0, HorizontalAlignment = 0 });
                        subTable.AddCell(new PdfPCell(gif) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                        subTable.AddCell(new PdfPCell(FormatPhrase(dr["item_code"].ToString() + "-" + dr["item_desc"].ToString())) { Colspan = 2, Border = 0, HorizontalAlignment = 1 });
                        subTable.HorizontalAlignment = 1;
                        cellBody = new PdfPCell(subTable);
                        cellBody.BorderWidth = 0; //<--- This is what sets the border for the nested table
                        pdtdtl.AddCell(cellBody);
                    }
                    else
                    {
                        cell = new PdfPCell(FormatPhrase(""));
                        cell.Border = 0;
                        pdtdtl.AddCell(cell);
                    }

                    cell = new PdfPCell(FormatPhrase(""));
                    cell.Border = 0;
                    cell.Colspan = 3;
                    cell.FixedHeight = 15f;
                    pdtdtl.AddCell(cell);
                }

            }
        }
        document.Add(pdtdtl);
        document.Close();
        Response.Flush();
        Response.End();
    }
    protected void btnPrint0_Click(object sender, EventArgs e)
    {
        string companyName = "করমি";
        int orderNo = 1234;
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[5] {
                            new DataColumn("ProductId", typeof(string)),
                            new DataColumn("Product", typeof(string)),
                            new DataColumn("Price", typeof(int)),
                            new DataColumn("Quantity", typeof(int)),
                            new DataColumn("Total", typeof(int))});
        dt.Rows.Add(101, "করমি", 500000, 1, 500000);
        dt.Rows.Add(102, "School Management System", 1500000, 2, 3000000);
        dt.Rows.Add(103, "Shop Management System", 2000000, 1, 2000000);
        dt.Rows.Add(104, "Bank Management System", 3000000, 1, 3000000);
        using (StringWriter sw = new StringWriter())
        {
            using (HtmlTextWriter hw = new HtmlTextWriter(sw))
            {
                StringBuilder sb = new StringBuilder();

                //Generate Invoice (Bill) Header.
                sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                sb.Append("<tr><td align='center' style='background-color: green;' colspan = '2'><b>করমি</b></td></tr>");
                sb.Append("<tr><td colspan = '2'></td></tr>");
                sb.Append("<tr><td><b>Order No: </b>");
                sb.Append(orderNo);
                sb.Append("</td><td align = 'right'><b>করমি : </b>");
                sb.Append(DateTime.Now);
                sb.Append(" </td></tr>");
                sb.Append("<tr><td colspan = '2'><b> করমি : </b>");
                sb.Append(companyName);
                sb.Append("</td></tr>");
                sb.Append("</table>");
                sb.Append("<br />");

                //Generate Invoice (Bill) Items Grid.
                sb.Append("<table border = '1'>");
                sb.Append("<tr>");
                foreach (DataColumn column in dt.Columns)
                {
                    sb.Append("<th style = 'background-color: red;color:#ffffff'>");
                    sb.Append(column.ColumnName);
                    sb.Append("</th>");
                }
                sb.Append("</tr>");
                foreach (DataRow row in dt.Rows)
                {
                    sb.Append("<tr>");
                    foreach (DataColumn column in dt.Columns)
                    {
                        sb.Append("<td>");
                        sb.Append(row[column]);
                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append("<tr><td align = 'center' colspan = '4'");
                //sb.Append(dt.Columns.Count - 1);
                sb.Append("'>Total</td>");
                //sb.Append("<td></td><td></td><td></td>");
                sb.Append("<td>");
                sb.Append(dt.Compute("sum(Total)", ""));
                sb.Append("</td>");
                sb.Append("</tr></table>");

                //Export HTML String as PDF.
                StringReader sr = new StringReader(sb.ToString());
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();
                htmlparser.Parse(sr);
                pdfDoc.Close();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=Invoice_" + orderNo + ".pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();
            }
        }
    }

    protected void lbSearch_Click(object sender, EventArgs e)
    {
        DataTable dataTable = _aSupplierManager.GetPurchaseMst(txtSupplierSearch.Text,0);
        if (dataTable.Rows.Count > 0)
        {
            DataTable dtDtl = _aPurchaseVoucherManager.GetShowPurchaseDetails(dataTable.Rows[0]["ID"].ToString());
            dgPODetailsDtl.DataSource = dtDtl;
            ViewState["purdtl"] = dtDtl;
            dgPODetailsDtl.DataBind();
        }
    }
    protected void lbClear_Click(object sender, EventArgs e)
    {
        txtSupplierSearch.Text = hfPurchaseID.Value = string.Empty;
        txtSupplierSearch.Focus();
    }


    
}