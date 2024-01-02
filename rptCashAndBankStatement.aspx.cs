using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data;
using iTextSharp.text.pdf.draw;
using Delve;

public partial class rptCashAndBankStatement : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PrintBankAndCashCode();
    }
    public void PrintBankAndCashCode()
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=MypdfFile.pdf");
        Document document = new Document();
        document = new Document(PageSize.A4.Rotate(), 10f, 10f, 30f, 30f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        pdfPage page = new pdfPage();
        writer.PageEvent = page;
        document.Open();

        string title = "";

        //  title = "Ledger statement of " + coaDesc + " from " + StartDt + " to " + EndDt + "";

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
        //cell.FixedHeight = 80f;
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
        cell = new PdfPCell(new Phrase(title, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
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

        DataTable dtledg = (DataTable)Session["dtledger"];
        float[] width = new float[11] { 10, 10, 10, 20, 30, 15, 15, 10, 15, 15, 10 };
        PdfPTable pdtledg = new PdfPTable(width);
        pdtledg.WidthPercentage = 100;
        pdtledg.HeaderRows = 1;

        cell = new PdfPCell(FormatHeaderPhrase("DATE"));//Voucher#
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        pdtledg.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("V.NO."));//Voucher#
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.Colspan = 2;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("HEAD OF ACCOUNTS"));//Particulars
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.Rowspan = 2;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("NARRATION"));//Vch Ref No
        cell.MinimumHeight = 15f;
        cell.Rowspan = 2;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("CASH"));//Debit
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.Colspan = 3;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("BANK"));//Credit
        cell.MinimumHeight = 15f;
        cell.Colspan = 3;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("DR"));//Balance
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("CR"));//Balance
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("DEBIT"));//Balance
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("CREDIT"));//Balance
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("BLANCE"));//Balance
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("DEBIT"));//Balance
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("CREDIT"));//Balance
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("BLANCE"));//Balance
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        decimal dtot = 0;
        decimal ctot = 0;
        //foreach (DataRow dr in dtledg.Rows)
        //{
        //    cell = new PdfPCell(FormatPhrase(dr["vch_sys_no"].ToString()));
        //    cell.HorizontalAlignment = 1;
        //    cell.VerticalAlignment = 1;
        //    cell.FixedHeight = 18f;
        //    cell.BorderColor = BaseColor.LIGHT_GRAY;
        //    pdtledg.AddCell(cell);

        //    cell = new PdfPCell(FormatPhrase(dr["value_date"].ToString()));
        //    cell.HorizontalAlignment = 1;
        //    cell.VerticalAlignment = 1;
        //    cell.FixedHeight = 18f;
        //    cell.BorderColor = BaseColor.LIGHT_GRAY;
        //    pdtledg.AddCell(cell);

        //    cell = new PdfPCell(FormatPhrase(dr["vch_manual_no"].ToString()));
        //    cell.HorizontalAlignment = 1;
        //    cell.VerticalAlignment = 1;
        //    cell.FixedHeight = 18f;
        //    cell.BorderColor = BaseColor.LIGHT_GRAY;
        //    pdtledg.AddCell(cell);

        //    cell = new PdfPCell(FormatPhrase(dr["PARTICULARS"].ToString()));
        //    cell.HorizontalAlignment = 0;
        //    cell.VerticalAlignment = 1;
        //    //cell.FixedHeight = 18f;
        //    cell.BorderColor = BaseColor.LIGHT_GRAY;
        //    pdtledg.AddCell(cell);

        //    cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Debit_amt"]).ToString("N3")));
        //    cell.HorizontalAlignment = 2;
        //    cell.VerticalAlignment = 1;
        //    cell.FixedHeight = 18f;
        //    cell.BorderColor = BaseColor.LIGHT_GRAY;
        //    pdtledg.AddCell(cell);

        //    cell = new PdfPCell(FormatPhrase(Convert.ToDouble( dr["Credit_amt"]).ToString("N3")));
        //    cell.HorizontalAlignment = 2;
        //    cell.VerticalAlignment = 1;
        //    cell.FixedHeight = 18f;
        //    cell.BorderColor = BaseColor.LIGHT_GRAY;
        //    pdtledg.AddCell(cell);

        //    cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["bal"]).ToString("N3")));
        //    cell.HorizontalAlignment = 2;
        //    cell.VerticalAlignment = 1;
        //    cell.FixedHeight = 18f;
        //    cell.BorderColor = BaseColor.LIGHT_GRAY;
        //    pdtledg.AddCell(cell);

        //    dtot += decimal.Parse(dr["Debit_amt"].ToString());
        //    ctot += decimal.Parse(dr["Credit_amt"].ToString());

        //}


        //cell = new PdfPCell(FormatPhrase("Total"));
        //cell.HorizontalAlignment = 2;
        //cell.VerticalAlignment = 1;
        ////cell.FixedHeight = 18f;
        //cell.Colspan = 4;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);

        //cell = new PdfPCell(FormatPhrase(dtot.ToString("N3")));
        //cell.HorizontalAlignment = 2;
        //cell.VerticalAlignment = 1;
        ////cell.FixedHeight = 18f;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);

        //cell = new PdfPCell(FormatPhrase(ctot.ToString("N3")));
        //cell.HorizontalAlignment = 2;
        //cell.VerticalAlignment = 1;
        ////cell.FixedHeight = 18f;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);

        //cell = new PdfPCell(FormatPhrase(""));
        //cell.HorizontalAlignment = 2;
        //cell.VerticalAlignment = 1;
        //cell.Border = 0;        
        //pdtledg.AddCell(cell);         
        document.Add(pdtledg);

        document.Close();
        Response.Flush();
        Response.End();

    }
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8));
    }
    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD));
    }
}