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
using ClosedXML.Excel;
using System.IO;
using Delve;

public partial class frmItemStatus : System.Web.UI.Page
{

    ClsItemDetailsManager _aClsItemDetailsManager = new ClsItemDetailsManager();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ClearAll();
        }
    }

    private void ClearAll()
    {
        txtName.Text = "";
        DataTable dt = _aClsItemDetailsManager.GetItemStatus("");
        dgItems.DataSource = dt;
        dgItems.DataBind();
        ViewState["STKS"] = dt;
        
    }
    protected void txtName_TextChanged(object sender, EventArgs e)
    {


        DataTable dtItem = _aClsItemDetailsManager.GetItemsDetailsOnSearch(txtName.Text.ToUpper());
        if (dtItem.Rows.Count > 0)
        {
            hfItemsID.Value = dtItem.Rows[0]["ID"].ToString();
            txtName.Text = dtItem.Rows[0]["Name"].ToString();

            DataTable dt = _aClsItemDetailsManager.GetItemStatus(hfItemsID.Value);
            dgItems.DataSource = dt;
            dgItems.DataBind();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                "alert('items not found..!!');", true);
        }
       
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        DataTable dt = (DataTable)ViewState["STKS"];
        Print(dt, rbReportType.SelectedValue, " Item Status");
    }

    private void Print(DataTable dt, string ReportType, string Heade)
    {
        if (dt != null)
        {
            if (ReportType.Equals("P"))
            {
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment; filename='Stock-Items'.pdf");
                Document document = new Document(PageSize.LEGAL.Rotate(), 50f, 50f, 40f, 40f);
                PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
                document.Open();

                PdfPCell cell;
                byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
                gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                gif.ScalePercent(15f);

                float[] titwidth = new float[2] { 10, 200 };
                PdfPTable dth = new PdfPTable(titwidth);
                dth.WidthPercentage = 100;

                cell = new PdfPCell(gif);
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Rowspan = 4;
                cell.BorderWidth = 0f;
                dth.AddCell(cell);
                cell =
                    new PdfPCell(new Phrase(Session["org"].ToString(),
                        FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Colspan = 7;
                cell.BorderWidth = 0f;
                cell.FixedHeight = 20f;
                dth.AddCell(cell);
                cell =
                    new PdfPCell(new Phrase(Session["add1"].ToString(),
                        FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Colspan = 7;
                cell.BorderWidth = 0f;
                cell.FixedHeight = 20f;
                dth.AddCell(cell);
                cell =
                    new PdfPCell(new Phrase(Session["add2"].ToString(),
                        FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Colspan = 7;
                cell.BorderWidth = 0f;
                cell.FixedHeight = 20f;
                dth.AddCell(cell);
                cell =
                    new PdfPCell(new Phrase(Heade,
                        FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Colspan = 7;
                cell.BorderWidth = 0f;
                cell.FixedHeight = 30f;
                dth.AddCell(cell);
                document.Add(dth);
                LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
                document.Add(line);

                PdfPTable emt = new PdfPTable(1);
                emt.WidthPercentage = 100;

                cell = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
                cell.BorderWidth = 0f;
                cell.HorizontalAlignment = 1;
                cell.FixedHeight = 30f;
                emt.AddCell(cell);
                document.Add(emt);
                // float[] widthdtl = new float[18] {4, 15, 25, 9, 7, 9, 9, 7, 7, 7, 7, 7, 7, 7,7,7,7,7 };
                //PdfPTable pdtdtl = new PdfPTable(widthdtl);
               // pdtdtl.WidthPercentage = 100;


                int coun = dt.Columns.Count;
                float[] titW = new float[coun + 1];
                for (int x = 0; x < coun + 1; x++)
                {
                    if (x == 0) { titW[0] = (90 / coun) / 2; }
                    else if (x ==2)
                    {
                        titW[x] =2+ (90 / coun)*2; 
                    }
                    else if (x == 1)
                    {
                        titW[x] = 4 + (90 / coun);
                    }
                    else
                    {
                        titW[x] = 100 / coun;
                    }
                }

                PdfPTable pdtdtl = new PdfPTable(titW);
                pdtdtl.WidthPercentage = 100;


                {
                    cell = new PdfPCell(new Phrase("SL", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD)));
                    //cell.BorderWidth = 0f;
                    cell.FixedHeight = 20f;
                    pdtdtl.AddCell(cell);
                }
                for (int x = 0; x < coun; x++)
                {



                    cell = new PdfPCell(new Phrase(dt.Columns[x].ColumnName, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD)));
                    //cell.BorderWidth = 0f;
                    cell.HorizontalAlignment = 1;
                    //cell.FixedHeight = 20f;
                    pdtdtl.AddCell(cell);

                }




                int serial = 0;


                decimal quantity = 0;
                decimal discount = 0;
                decimal TotalPrice = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    serial = serial + 1;
                    cell = new PdfPCell(new Phrase(serial.ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 6, iTextSharp.text.Font.NORMAL)));
                    //cell.BorderWidth = 0f;
                    cell.HorizontalAlignment = 1;
                    cell.FixedHeight = 20f;
                    pdtdtl.AddCell(cell);

                    for (int x = 0; x < coun; x++)
                    {


                        cell = new PdfPCell(new Phrase(dr[dt.Columns[x].ColumnName].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 6, iTextSharp.text.Font.NORMAL)));
                        //cell.BorderWidth = 0f;
                        cell.HorizontalAlignment = 1;
                        //cell.FixedHeight = 20f;
                        pdtdtl.AddCell(cell);

                        if (x == coun - 1)
                        {
                            quantity = quantity + Convert.ToDecimal(dr[dt.Columns[x].ColumnName].ToString());
                        }
                    }



                }

                


                int Serial = 1;
                decimal totcostprice = 0;
                decimal totSaleprice = 0;
                decimal totSaleAmt = 0;
                decimal totCloseStk = 0;
                decimal totCloseAmt = 0;
                // DataTable dtdtl = (DataTable)ViewState["STK"];
                


                document.Add(pdtdtl);
                document.Close();
                Response.Flush();
                Response.End();
            }
            if (ReportType.Equals("E"))
            {
                // DataTable dtdtl = (DataTable)ViewState["STK"];

                string filename = "Item Status-" + Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "Iotal_Items_Stock");
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
        }
    }
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 6));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD));
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearAll();
    }
    protected void dgItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        DataTable dt = (DataTable)ViewState["STKS"];
        dgItems.DataSource = dt;
        dgItems.PageIndex = e.NewPageIndex;
        dgItems.DataBind();
    }
}