using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

public partial class frmDailySalesSummery : System.Web.UI.Page
{


    SalesManager _aSalesManager = new SalesManager();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtStartDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtDate.Text))
        {
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
        else if (string.IsNullOrEmpty(txtStartDate.Text))
        {
             txtStartDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
        double totSaleQty = IdManager.GetShowSingleValueCurrency(
            @"select isnull(sum(isnull(t1.Quantity,0)),0) AS[totQty] from OrderDetail t1
        inner join [Order] t2 on t2.ID=t1.OrderID where (convert(date,t2.OrderDate,103) between convert(date,'" +
            txtStartDate.Text + "',103) and convert(date,'" + txtDate.Text + "',103))");

        double totReturnQty = IdManager.GetShowSingleValueCurrency(
            @"select isnull(sum(isnull(t1.Quantity,0)),0) AS[totQty] from OrderReturnDetail t1
inner join OrderReturn t2 on t2.ID=t1.OrderReturnMstID
where (convert(date,t2.ReturnDate,103) between convert(date,'" + txtStartDate.Text + "',103) and convert(date,'" +
            txtDate.Text + "',103))");
        lblTotSaleQty.Text = totSaleQty.ToString("N2");
        lblTotReturnQty.Text = totReturnQty.ToString("N2");
        DataTable dtPayment = _aSalesManager.GetPaymentSummery(txtStartDate.Text, txtDate.Text, "1");
        DataTable dt = ExperManager.GetShowExpensesReport(txtStartDate.Text, txtDate.Text);
        if (dtPayment.Rows.Count > 0)
        {
            dgPayment.DataSource = dtPayment;
            dgPayment.DataBind();
            ShowFooterTotal(dtPayment);
        }
    }

    private void ShowFooterTotal(DataTable DT1)
    {
        decimal tot = 0;
        foreach (DataRow dr in DT1.Rows)
        {
            if (!string.IsNullOrEmpty(dr["Amount"].ToString()))
            {
                tot += Convert.ToDecimal(dr["Amount"]);
            }

        }
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
        TableCell cell;
        cell = new TableCell();
        cell.Text = "<h4>Total Received Amount</h4>";
        cell.ColumnSpan = 2;
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        cell = new TableCell();
        cell.Text = "<h4>" + tot.ToString("N2") + "</h4>";
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        if (dgPayment.Rows.Count > 0)
        {
            dgPayment.Controls[0].Controls.Add(row);
        }
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtDate.Text))
        {
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
         if (string.IsNullOrEmpty(txtStartDate.Text))
        {
            txtStartDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
        double totSaleQty = IdManager.GetShowSingleValueCurrency(
            @"select isnull(sum(isnull(t1.Quantity,0)),0) AS[totQty] from OrderDetail t1
        inner join [Order] t2 on t2.ID=t1.OrderID where (convert(date,t2.OrderDate,103) between convert(date,'" +
            txtStartDate.Text + "',103) and convert(date,'" + txtDate.Text + "',103))");

        double totReturnQty = IdManager.GetShowSingleValueCurrency(
            @"select isnull(sum(isnull(t1.Quantity,0)),0) AS[totQty] from OrderReturnDetail t1
inner join OrderReturn t2 on t2.ID=t1.OrderReturnMstID
where (convert(date,t2.ReturnDate,103) between convert(date,'" + txtStartDate.Text + "',103) and convert(date,'" +
            txtDate.Text + "',103))");
        lblTotSaleQty.Text = totSaleQty.ToString("N2");
        lblTotReturnQty.Text = totReturnQty.ToString("N2");
       
        DataTable dt = ExperManager.GetShowExpensesReport(txtStartDate.Text, txtDate.Text);
       
         var   DDT = "Report of (" +txtStartDate.Text+" to "+ txtDate.Text+") ";
       

        
            DailyExpenseReport(DDT, dt);
        
    }





    private void DailyExpenseReport(string p, DataTable dt)
    {
        //if (dt.Rows.Count <= 0) { return; }
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename='ItemsPurchase'.pdf");
        Document document = new Document(PageSize.A4);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
      
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(25f);

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
        cell.Colspan = 2;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 2;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 2;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase("Daily Sales && Expenses Report ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 2;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell); cell = new PdfPCell(new Phrase(p, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 2;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] widthdtl = new float[3] { 5, 25, 10 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        




        // seals ///
      int  Serial = 1;
        cell = new PdfPCell(FormatHeaderPhrase("SL."));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase("Amount Type"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        DataTable dtPayment = _aSalesManager.GetPaymentSummery(txtStartDate.Text, txtDate.Text, "1");
        decimal totsQty = 0;
        foreach (DataRow dr in dtPayment.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;





            cell = new PdfPCell(FormatPhrase(dr["Name"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Amount"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);




            totsQty += Convert.ToDecimal(dr["Amount"]);
        }

        cell = new PdfPCell(FormatHeaderPhrase("Total "));
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 2;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totsQty.ToString("N2")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

      


        /////



         Serial = 1;

        cell = new PdfPCell(FormatHeaderPhrase("SL."));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase("Expenses Head"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        decimal totQty = 0;
        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;





            cell = new PdfPCell(FormatPhrase(dr["ExpName"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Amount"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);




            totQty += Convert.ToDecimal(dr["Amount"]);
        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 2;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totQty.ToString("N2")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase("Total Amount"));
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 2;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase((totsQty-totQty).ToString("N2")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);

        //cell = SignatureFormat(document, cell);


        PdfPCell cells = new PdfPCell();
        iTextSharp.text.Rectangle page1 = document.PageSize;
        float[] FootWth = new float[] { 5, 30, 10, 30, 10, 30 };
        PdfPTable Fdth = new PdfPTable(FootWth);
        Fdth.TotalWidth = page1.Width - 10;
        Fdth.HorizontalAlignment = Element.ALIGN_CENTER;


        cells = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cells.HorizontalAlignment = 1;
        cells.Border = 0;
        cells.VerticalAlignment = 1;
        Fdth.AddCell(cells);


        cells = new PdfPCell(new Phrase("Received by", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cells.HorizontalAlignment = 1;
        cells.Border = 1;
        cells.VerticalAlignment = 1;
        Fdth.AddCell(cells);


        cells = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cells.HorizontalAlignment = 1;
        cells.Border = 0;
        cells.VerticalAlignment = 1;
        Fdth.AddCell(cells);


        cells = new PdfPCell(new Phrase("Checked by", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cells.HorizontalAlignment = 1;
        cells.Border = 1;
        cells.VerticalAlignment = 1;
        Fdth.AddCell(cells);
        cells = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cells.HorizontalAlignment = 1;
        cells.Border = 0;
        cells.VerticalAlignment = 1;
        Fdth.AddCell(cells);
        cells = new PdfPCell(new Phrase("Accounts Officer", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cells.HorizontalAlignment = 1;
        cells.Border = 1;
        cells.VerticalAlignment = 1;
        Fdth.AddCell(cells);
        Fdth.WriteSelectedRows(0, 5, 0, 30, writer.DirectContent);







        document.Close();
        Response.Flush();
        Response.End();
    }

    private static PdfPCell SignatureFormat(Document document, PdfPCell cell)
    {




       



        //



        float[] widtl = new float[5] { 20, 20, 20, 20, 20 };
        PdfPTable pdtsig = new PdfPTable(widtl);
        pdtsig.WidthPercentage = 100;
        var Border = PdfPCell.BOTTOM_BORDER;
        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 5;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);


        cell = new PdfPCell(FormatPhrase("Received by"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.FixedHeight =20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Checked by")) ;
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Authorised by"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);
        document.Add(pdtsig);
        return cell;
    }


    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11));
    }

   
    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.BOLD));
    }
}