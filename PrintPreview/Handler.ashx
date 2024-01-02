<%@ WebHandler Language="C#" Class="Handler" %> 
using System;
using System.Drawing;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Web.SessionState;
using Delve;
using iTextSharp.text;
using iTextSharp.text.pdf;
public class Handler : IHttpHandler, IRequiresSessionState
{

    PdfPCell cell;
    private BarcodeLib.Barcode barcode = new BarcodeLib.Barcode();
    public void ProcessRequest(HttpContext context)
    {
        string ShipmentID = "", CartonNo = "", ShipmentName = "", SupplierID = "", SupplierName = "", ItemSearchID = "", ItemID = "", ItemName = "", CartonNo1 = "", CartonNo2 = "";
        string Type = "";
        string id = context.Request.QueryString["Id"];
        Type = id;
        byte[] bytes;
        string fileName, contentType;
        fileName = "ShipmentReport" + ".pdf";
        try
        {
             ShipmentID = context.Session["ShipmentID"].ToString();
             CartonNo = context.Session["CartonNo"].ToString();
             ShipmentName = context.Session["ShipmentName"].ToString();
             SupplierID = context.Session["SupplierID"].ToString();
             SupplierName = context.Session["SupplierName"].ToString();
             ItemSearchID = context.Session["ItemSearchID"].ToString();
             ItemID = context.Session["ItemID"].ToString();
             ItemName = context.Session["ItemName"].ToString();
             CartonNo1 = context.Session["CartonNo1"].ToString();
             CartonNo2 = context.Session["CartonNo2"].ToString();
        }
        catch { }
        //string SubjetText = txtRfNo.Text + txtName.Text + " Report ";
        //string BodyText = "Please see the attached file for " + txtRfNo.Text + txtName.Text + ". From Meem Medical Center. Thanks. ";
        using (StringWriter sw = new StringWriter())
        {
            using (System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw))
            {
              
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    if (Type == "SWR")
                    {
                        ShipmentWiseCartonReport(context, CartonNo, ShipmentID, memoryStream);
                    }
                    else if (Type == "PSWR")
                    {

                        SupplierAndStyleWiseReport(context, SupplierName, SupplierID, ShipmentID, memoryStream, ShipmentName);
                    }
                    else if (Type == "SWRC")
                    {
                        
                        StyleWiseReport(context, memoryStream, ShipmentName, ItemID, ItemSearchID);
                    }
                    else if (Type == "SBR" || Type == "SBRI")
                    {
                        ShipmentBarcodeReport(context, memoryStream, CartonNo1, CartonNo2, ShipmentID, Type);
                    }
                    else if (Type == "PLR")
                    {
                        PackingListReport(context, SupplierName, SupplierID, ShipmentID, memoryStream, ShipmentName);
                    }
                    //***************** Send Cartoon List || Cartoon Received List **************//
                    else if (Type == "SCL" || Type == "SRL" || Type == "SRLT")
                    {
                        GoodsReceivedCheckList(context, Type, memoryStream, ShipmentID, CartonNo1, CartonNo2);
                    }
                    //*************** Shiftment Wise All Items Details *****//
                    else if (Type == "ALD")
                    {
                        AllShipmentDetaile(context, ShipmentName, ShipmentID, memoryStream);
                    }
                     //*************** Total Items Stock  *****//
                    else if (Type == "TIS")
                    {
                        TotalClosingStock(context, memoryStream);
                    }
                    bytes = memoryStream.ToArray();
                    memoryStream.Close();
                }
            }
        }
        context.Response.Buffer = true;
        context.Response.Charset = "";
        if (context.Request.QueryString["download"] == "1")
        {
            context.Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
        }
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        context.Response.ContentType = "application/pdf";
        context.Response.BinaryWrite(bytes);
        context.Response.Flush();
        context.Response.End();
    }

    private static void TotalClosingStock(HttpContext context, MemoryStream memoryStream)
    {
        DataTable dt = ShiftmentItemsCartoonManager.GetShowAllItemsStock("");
        if (dt.Rows.Count > 0)
        {
            string filename = "TotalItemsStock-" + DateTime.Now.ToString("dd/MM/yyyy");

            Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();
            iTextSharp.text.Rectangle page = document.PageSize;
            PdfPTable head = new PdfPTable(1);
            head.TotalWidth = page.Width - 50;

            PdfPCell cell;
            byte[] logo = GlBookManager.GetGlLogo(context.Session["book"].ToString());
            iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
            gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
            gif.ScalePercent(8f);

            float[] titwidth = new float[3] {15, 40, 10};
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

            cell = new PdfPCell(gif);
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.HorizontalAlignment = Element.ALIGN_TOP;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase(context.Session["org"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);

            cell = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase(context.Session["add1"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase(context.Session["add2"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase("Total Items Stock.\n BD,PH,In-Tran.",
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 30f;
            dth.AddCell(cell);
            document.Add(dth);

            iTextSharp.text.pdf.draw.LineSeparator line = new iTextSharp.text.pdf.draw.LineSeparator(0f, 100, null,
                Element.ALIGN_CENTER, -2);
            document.Add(line);

            PdfPTable dtempty = new PdfPTable(1);
            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.BorderWidth = 0f;
            cell.FixedHeight = 10f;
            dtempty.AddCell(cell);
            document.Add(dtempty);


            float[] widthdtl = new float[6] {8, 14, 30, 15, 15, 15};
            PdfPTable pdtdtl = new PdfPTable(widthdtl);
            pdtdtl.WidthPercentage = 100;
            pdtdtl.HeaderRows = 1;
            cell = new PdfPCell(FormatHeaderPhrase("SL No."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Items Code"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Items Name"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("BD-Stock"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("In Transit"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("PH-Stock"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);

            int serial1 = 1;
            double totBD = 0, totPH = 0, totTr = 0;

            foreach (DataRow drr in dt.Rows)
            {
                cell = new PdfPCell(FormatFontPhrase(serial1.ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatFontPhrase(drr["Code"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatFontPhrase(drr["Name"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatFontPhrase(drr["BD_Stock"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatFontPhrase(drr["In_Tr"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatFontPhrase(drr["PH_Stock"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                pdtdtl.AddCell(cell);

                if (!string.IsNullOrEmpty(drr["BD_Stock"].ToString()))
                    totBD += Convert.ToDouble(drr["BD_Stock"]);
                if (!string.IsNullOrEmpty(drr["PH_Stock"].ToString()))
                    totPH += Convert.ToDouble(drr["PH_Stock"]);
                if (!string.IsNullOrEmpty(drr["In_Tr"].ToString()))
                    totTr += Convert.ToDouble(drr["In_Tr"]);

                serial1++;
            }
            cell = new PdfPCell(FormatHeaderPhrase("Total"));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Colspan = 3;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatFontPhrase(totBD.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatFontPhrase(totTr.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatFontPhrase(totPH.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);

            document.Add(pdtdtl);
            document.Close();
        }
    }

    private static void AllShipmentDetaile(HttpContext context, string ShipmentName, string ShipmentID,
        MemoryStream memoryStream)
    {
        string ShipNo = "";
        if (!string.IsNullOrEmpty(ShipmentName))
        {
            ShipNo = "where ID='" + ShipmentID + "'";
        }
        string SelectShiftment =
            @"Select ID,ShiftmentNO,CONVERT(NVARCHAR,ShiftmentDate,103)ShiftmentDate from ShiftmentAssigen " + ShipNo;
        DataTable dtShift = DataManager.ExecuteQuery(DataManager.OraConnString(), SelectShiftment, "ShiftmentAssigen");
        if (dtShift.Rows.Count > 0)
        {
            double allTot = 0;
            decimal totAmt = 0;
            string parameter = "";
            Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();
            iTextSharp.text.Rectangle page = document.PageSize;
            PdfPTable head = new PdfPTable(1);
            head.TotalWidth = page.Width - 50;

            PdfPCell cell;
            byte[] logo = GlBookManager.GetGlLogo(context.Session["book"].ToString());
            iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
            gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
            gif.ScalePercent(8f);

            float[] titwidth = new float[3] {15, 40, 10};
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

            cell = new PdfPCell(gif);
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.HorizontalAlignment = Element.ALIGN_TOP;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase(context.Session["org"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);

            cell = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase(context.Session["add1"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase(context.Session["add2"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase("Days Duration In Transit",
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 30f;
            dth.AddCell(cell);
            document.Add(dth);

            iTextSharp.text.pdf.draw.LineSeparator line = new iTextSharp.text.pdf.draw.LineSeparator(0f, 100, null,
                Element.ALIGN_CENTER, -2);
            document.Add(line);

            PdfPTable dtempty = new PdfPTable(1);
            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.BorderWidth = 0f;
            cell.FixedHeight = 10f;
            dtempty.AddCell(cell);
            document.Add(dtempty);

            int serial = 1;
            float[] widthdtl = new float[7] {8, 14, 20, 15, 15, 15, 10};
            PdfPTable pdtdtl = new PdfPTable(widthdtl);
            pdtdtl.WidthPercentage = 100;

            cell = new PdfPCell(FormatHeaderPhrase("SL No."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("DELIVERY DATE"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("SHIPMENT NO."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("TOTAL CTN NO"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("TOTAL PCS"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("RECEIVED DATE"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("DAYS DURATION"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);
            int serial1 = 1;
            double atTot = 0;
            double rcvTotal = 0;

            foreach (DataRow drr in dtShift.Rows)
            {
                allTot = 0;
                string SelectQuery =
                    @"SELECT t1.[ID],t1.[ShiftmentID],t2.Name,t3.ShiftmentNO,t1.PartyRate,t1.Quantity,(t1.PartyRate*t1.Quantity) as totAmount,t1.Label,t4.Name as supplier FROM ShiftmentItems t1 inner join Item t2 on t2.ID=t1.ItemsID inner join ShiftmentAssigen t3 on t3.ID=t1.ShiftmentID inner join Supplier t4 on t4.ID=t1.SupplierID WHERE t1.ShiftmentID='" +
                    drr["ID"].ToString() + "' order by t1.PartyID  ";
                DataTable dtCartoon = DataManager.ExecuteQuery(DataManager.OraConnString(), SelectQuery,
                    "Temp_ShiftmentItemsColorSize");
                if (dtCartoon.Rows.Count > 0)
                {
                    foreach (DataRow drtt in dtCartoon.Rows)
                    {
                        double tot = ItemsTotal(allTot, drtt);
                        allTot += tot;
                        atTot += tot;
                    }
                }
                cell = new PdfPCell(FormatFontPhrase(serial1.ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                pdtdtl.AddCell(cell);
                cell =
                    new PdfPCell(
                        FormatFontPhrase(
                            (DataManager.DateEncode(drr["ShiftmentDate"].ToString())).ToString(IdManager.DateFormat())));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatFontPhrase(drr["ShiftmentNO"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                pdtdtl.AddCell(cell);
                int CTN = IdManager.GetShowSingleValueInt("COUNT(*)", "t2.ShiftmentID",
                    "[ShiftmentBoxingItemsDtl] t1 INNER JOIN ShiftmentBoxingMst t2 on t2.ID=t1.MasterID",
                    drr["ID"].ToString());
                cell = new PdfPCell(FormatFontPhrase(CTN.ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatFontPhrase(allTot.ToString("N0")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                pdtdtl.AddCell(cell);
                Double DaysNull = 0;
                double Days = 0;
                string rcvDate = IdManager.GetShowSingleValueString("CONVERT(nvarchar,Max(ReceivedDate),103)",
                    "t1.ReceiveFlag=1 and t1.ShiftmentID", "ShiftmentBoxingMst t1", drr["ID"].ToString());
                string rcvDateNULL = IdManager.GetShowSingleValueString("CONVERT(nvarchar,Max(GETDATE()),103)",
                    "t1.ShiftmentID", "ShiftmentBoxingMst t1", drr["ID"].ToString());
                if (string.IsNullOrEmpty(rcvDate))
                {
                    DaysNull =
                        (DataManager.DateEncode(rcvDateNULL) - DataManager.DateEncode(drr["ShiftmentDate"].ToString()))
                            .TotalDays;
                    cell = new PdfPCell(FormatFontPhrase("In Transit"));
                }
                else
                {
                    Days =
                        (DataManager.DateEncode(rcvDate) - DataManager.DateEncode(drr["ShiftmentDate"].ToString()))
                            .TotalDays;

                    cell = new PdfPCell(FormatFontPhrase((DataManager.DateEncode(rcvDate)).ToString(IdManager.DateFormat())));
                }
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                pdtdtl.AddCell(cell);
                if (Days > 0)
                {
                    cell = new PdfPCell(FormatFontPhrase(Days.ToString("N0")));
                }
                else
                {
                    cell = new PdfPCell(FormatFontPhrase(DaysNull.ToString("N0")));
                }
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                pdtdtl.AddCell(cell);
                serial1++;
            }
            cell = new PdfPCell(FormatHeaderPhrase("Total"));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Colspan = 4;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatFontPhrase(atTot.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatFontPhrase(""));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Colspan = 2;
            pdtdtl.AddCell(cell);

            document.Add(pdtdtl);
            document.Close();
        }
    }

    private static void GoodsReceivedCheckList(HttpContext context, string Type, MemoryStream memoryStream,
        string ShipmentID, string CartonNo1, string CartonNo2)
    {
        string Flag = "";
        string filename = "";
        if (Type == "SCL")
        {
            Flag = "0";
            filename = "GoodsReceivedCheckList-" + DateTime.Now.ToString("dd/MM/yyyy");
        }
        else if (Type == "SRL")
        {
            Flag = "1";
            filename = "GoodsReceivedCheckList-" + DateTime.Now.ToString("dd/MM/yyyy");
        }
        else
        {
            Flag = "0";
            filename = "GoodsRecevedReport(Cartonwise)-" + DateTime.Now.ToString("dd/MM/yyyy");
        }
        Document document = new Document(PageSize.A4, 10f, 10f, 30f, 50f);
        PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
        pdfPage page1 = new pdfPage();
        writer.PageEvent = page1;
        document.Open();
        iTextSharp.text.Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;

        PdfPCell cell;

        byte[] logo = GlBookManager.GetGlLogo(context.Session["bookMAN"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(8f);

        float[] titwidth = new float[3] {15, 40, 10};
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.VerticalAlignment = Element.ALIGN_TOP;
        cell.HorizontalAlignment = Element.ALIGN_TOP;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);

        cell =
            new PdfPCell(new Phrase(context.Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        dth.AddCell(cell);

        cell = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);

        cell =
            new PdfPCell(new Phrase(context.Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(context.Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        dth.AddCell(cell);

        cell = new PdfPCell(new Phrase(Type, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);

        iTextSharp.text.pdf.draw.LineSeparator line = new iTextSharp.text.pdf.draw.LineSeparator(0f, 100, null,
            Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        int serial = 1;
        float[] widthdtl;
        if (Type.Equals("SRLT"))
        {
            widthdtl = new float[10] {8, 14, 9, 12, 22, 11, 10, 10, 10, 10};
        }
        else
        {
            widthdtl = new float[6] {8, 14, 15, 20, 15, 15};
        }

        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        cell = new PdfPCell(FormatHeaderPhrase("SL No."));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.Colspan = 6;
        //cell.FixedHeight = 20f;
        //cell.BorderWidth = 0f;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Shipment Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        // cell.Colspan = 2;
        pdtdtl.AddCell(cell);
        if (Type.Equals("SRLT"))
        {
            cell = new PdfPCell(FormatHeaderPhrase("Barcode"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            //cell.Colspan = 6;
            //cell.FixedHeight = 20f;
            //cell.BorderWidth = 0f;
            pdtdtl.AddCell(cell);
        }
        cell = new PdfPCell(FormatHeaderPhrase("CTN No."));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.Colspan = 6;
        //cell.FixedHeight = 20f;
        //cell.BorderWidth = 0f;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Shipment No"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        // cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        //cell.Colspan = 2;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Send Qty."));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 30f;
        //cell.Colspan = 2;
        pdtdtl.AddCell(cell);
        if (Type.Equals("SRLT"))
        {
            cell = new PdfPCell(FormatHeaderPhrase("Reject Qty"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            // cell.BorderWidth = 0f;
            // cell.FixedHeight = 30f;
            //cell.Colspan = 2;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Short Qty."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            // cell.BorderWidth = 0f;
            // cell.FixedHeight = 30f;
            //cell.Colspan = 2;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Excess Qty."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            // cell.BorderWidth = 0f;
            // cell.FixedHeight = 30f;
            //cell.Colspan = 2;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Receive Qty"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            // cell.BorderWidth = 0f;
            // cell.FixedHeight = 30f;
            //cell.Colspan = 2;
            pdtdtl.AddCell(cell);
        }
        else
        {
            cell = new PdfPCell(FormatHeaderPhrase("Remark's"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            // cell.BorderWidth = 0f;
            // cell.FixedHeight = 30f;
            //cell.Colspan = 2;
            pdtdtl.AddCell(cell);
        }
        DataTable dtCartoon = ShiftmentItemsCartoonManager.GetShowSendAnReceiveItemsList(ShipmentID, CartonNo1, CartonNo2,
            Flag);
        decimal tot = decimal.Zero;
        foreach (DataRow drtt in dtCartoon.Rows)
        {
            cell = new PdfPCell(FormatFontPhrase(serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            //cell.BorderWidth = 0f;
            // cell.Colspan = 2;
            pdtdtl.AddCell(cell);

            cell =
                new PdfPCell(
                    FormatFontPhrase((DataManager.DateEncode(drtt["AddDate"].ToString())).ToString(IdManager.DateFormat())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            //cell.BorderWidth = 0f;
            // cell.Colspan = 2;
            pdtdtl.AddCell(cell);
            if (Type.Equals("SRLT"))
            {
                cell = new PdfPCell(FormatHeaderPhrase(drtt["ID"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.Colspan = 6;
                //cell.FixedHeight = 20f;
                //cell.BorderWidth = 0f;
                pdtdtl.AddCell(cell);
            }
            cell = new PdfPCell(FormatFontPhrase(drtt["CartoonNo"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            //cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            //cell.Colspan = 2;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatFontPhrase(drtt["ShiftmentNO"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            //cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            //cell.Colspan = 2;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatFontPhrase(drtt["tot_Qty"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            //cell.BorderWidth = 0f;
            // cell.Colspan = 2;
            pdtdtl.AddCell(cell);
            if (Type.Equals("SRLT"))
            {
                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0f;
                // cell.FixedHeight = 30f;
                //cell.Colspan = 2;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0f;
                // cell.FixedHeight = 30f;
                //cell.Colspan = 2;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0f;
                // cell.FixedHeight = 30f;
                //cell.Colspan = 2;
                pdtdtl.AddCell(cell);
            }
            cell = new PdfPCell(FormatFontPhrase(""));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            //cell.BorderWidth = 0f;
            // cell.Colspan = 2;
            pdtdtl.AddCell(cell);
            tot += Convert.ToDecimal(drtt["tot_Qty"].ToString());
            serial++;
        }

        cell = new PdfPCell(FormatHeaderPhrase("Total "));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N3")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        //cell.Colspan = 2;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        //cell.Colspan = 2;
        pdtdtl.AddCell(cell);
        document.Add(pdtdtl);

        if (Type.Equals("SRLT"))
        {
            float[] widtl = new float[5] {20, 20, 20, 20, 20};
            PdfPTable pdtsig = new PdfPTable(widtl);
            pdtsig.WidthPercentage = 100;

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            cell.Colspan = 5;
            cell.FixedHeight = 40f;
            pdtsig.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            cell.FixedHeight = 20f;
            pdtsig.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            cell.FixedHeight = 20f;
            pdtsig.AddCell(cell);
            cell = new PdfPCell(FormatPhrase("Approved by"));
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
            cell = new PdfPCell(FormatPhrase("Checked by"));
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
            cell.Colspan = 5;
            //cell.FixedHeight = 40f;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            //cell.FixedHeight = 20f;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            // cell.FixedHeight = 20f;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);
            cell = new PdfPCell(FormatPhrase("Name :"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            // cell.FixedHeight = 20f;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            //cell.FixedHeight = 20f;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);
            cell = new PdfPCell(FormatPhrase("Name :"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            //cell.FixedHeight = 20f;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            cell.Colspan = 5;
            // cell.FixedHeight = 40f;
            // cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            // cell.FixedHeight = 20f;
            // cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            //cell.FixedHeight = 20f;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);
            cell = new PdfPCell(FormatPhrase("Date :"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            //cell.FixedHeight = 20f;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            //cell.FixedHeight = 20f;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);
            cell = new PdfPCell(FormatPhrase("Date :"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            // cell.FixedHeight = 20f;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);
            document.Add(pdtsig);
        }
        document.Close();
    }

    private static void PackingListReport(HttpContext context, string SupplierName, string SupplierID, string ShipmentID,
        MemoryStream memoryStream, string ShipmentName)
    {
        double allTot = 0;
        decimal totAmt = 0;
        string parameter = "";
        if (SupplierName != "")
        {
            parameter = " AND t1.SupplierID='" + SupplierID + "'";
        }
        //string SelectQuery =
        //    @"SELECT t1.[ID],t1.[ShiftmentID],t2.Name,t3.ShiftmentNO,t1.PartyRate,t1.Quantity,(t1.PartyRate*t1.Quantity) as totAmount,t1.Label,t4.ContactName as supplier FROM ShiftmentItems t1 inner join Item t2 on t2.ID=t1.ItemsID inner join ShiftmentAssigen t3 on t3.ID=t1.ShiftmentID inner join Supplier t4 on t4.ID=t1.SupplierID WHERE t1.ShiftmentID='" +
        //    ShipmentID + "' " + parameter + " order by t1.PartyID  ";
        //DataTable dtCartoon = DataManager.ExecuteQuery(DataManager.OraConnString(), SelectQuery,
        //    "Temp_ShiftmentItemsColorSize");

        DataTable dtShipment = null;
        int Count = IdManager.GetShowSingleValueInt("COUNT(*)",
            "ShiftmentAssigen where ParentShiftmentNO='" + ShipmentID + "' ");
        if (Count > 0)
        {
            string ShipmentQuery =
                "SELECT [ID] ,[ShiftmentNO]    FROM [dbo].[ShiftmentAssigen] where [ParentShiftmentNO]='" +
                ShipmentID + "' ";
            dtShipment = DataManager.ExecuteQuery(DataManager.OraConnString(), ShipmentQuery, "ShiftmentAssigen");
        }
        else
        {
            string ShipmentQuery =
                "SELECT [ID] ,[ShiftmentNO]    FROM [dbo].[ShiftmentAssigen] where [ID]='" +
                ShipmentID + "' ";
            dtShipment = DataManager.ExecuteQuery(DataManager.OraConnString(), ShipmentQuery, "ShiftmentAssigen");
        }

        if (dtShipment.Rows.Count > 0)
        {
            Document document = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();
            iTextSharp.text.Rectangle page = document.PageSize;
            PdfPTable head = new PdfPTable(1);
            head.TotalWidth = page.Width - 50;

            PdfPCell cell;
            byte[] logo = GlBookManager.GetGlLogo(context.Session["book"].ToString());
            iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
            gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
            gif.ScalePercent(8f);

            float[] titwidth = new float[3] {15, 40, 10};
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

            cell = new PdfPCell(gif);
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.HorizontalAlignment = Element.ALIGN_TOP;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase(context.Session["org"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);

            cell = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase(context.Session["add1"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase(context.Session["add2"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase("Shipment No. : " + ShipmentName + " - Supplier & Style Wise Packing List.",
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 30f;
            dth.AddCell(cell);
            document.Add(dth);

            iTextSharp.text.pdf.draw.LineSeparator line = new iTextSharp.text.pdf.draw.LineSeparator(0f, 100, null,
                Element.ALIGN_CENTER, -2);
            document.Add(line);

            PdfPTable dtempty = new PdfPTable(1);
            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.BorderWidth = 0f;
            cell.FixedHeight = 10f;
            dtempty.AddCell(cell);
            document.Add(dtempty);

           // int serial = 1;
            float[] widthdtl = new float[10] {8, 30, 10, 20, 20, 20, 10, 10, 15, 15};
            PdfPTable pdtdtl = new PdfPTable(widthdtl);
            pdtdtl.WidthPercentage = 100;

            int serial = 1;
            foreach (DataRow drMst in dtShipment.Rows)
            {
                cell = new PdfPCell(FormatHeaderPhrase12("Shipment No. : " + drMst["ShiftmentNO"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Colspan = 10;
                cell.FixedHeight = 20f;
                //cell.BorderWidth = 0f;
                pdtdtl.AddCell(cell);
                
                cell = new PdfPCell(FormatHeaderPhrase("SL No."));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                //cell.Colspan = 6;
                //cell.FixedHeight = 20f;
                //cell.BorderWidth = 0f;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("CTN NO."));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                //cell.Colspan = 6;
                //cell.FixedHeight = 20f;
                //cell.BorderWidth = 0f;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("T.CTN"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                //cell.Colspan = 6;
                //cell.FixedHeight = 20f;
                //cell.BorderWidth = 0f;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Supplier Name"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.BorderWidth = 0f;
                //cell.FixedHeight = 20f;
                // cell.Colspan = 2;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Items Name"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0f;
                //cell.FixedHeight = 20f;
                //cell.Colspan = 2;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Label"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0f;
                // cell.FixedHeight = 30f;
                //cell.Colspan = 2;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Rate"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0f;
                // cell.FixedHeight = 30f;
                //cell.Colspan = 2;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Quantity"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0f;
                // cell.FixedHeight = 30f;
                //cell.Colspan = 2;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Total"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0f;
                // cell.FixedHeight = 30f;
                //cell.Colspan = 2;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Remark's"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0f;
                // cell.FixedHeight = 30f;
                //cell.Colspan = 2;
                pdtdtl.AddCell(cell);

                string SelectQuery =
                    @"SELECT t1.[ID],t1.[ShiftmentID],t2.Name,t3.ShiftmentNO,t1.PartyRate,t1.Quantity,(t1.PartyRate*t1.Quantity) as totAmount,t1.Label,t4.ContactName as supplier FROM ShiftmentItems t1 inner join Item t2 on t2.ID=t1.ItemsID inner join ShiftmentAssigen t3 on t3.ID=t1.ShiftmentID inner join Supplier t4 on t4.ID=t1.SupplierID WHERE t1.ShiftmentID='" +
                    drMst["ID"].ToString() + "' " + parameter + " order by t1.PartyID  ";
                DataTable dtCartoon = DataManager.ExecuteQuery(DataManager.OraConnString(), SelectQuery,
                    "ShiftmentItems");
                
                foreach (DataRow drtt in dtCartoon.Rows)
                {
                    string Cortoon = "";
                    int tt = 0;
                    string SelectQuery1 =
                        @"SELECT DISTINCT t2.CartoonNo FROM [ShiftmentBoxingItemsDtl] t1 INNER JOIN ShiftmentBoxingMst t2 on t2.ID=t1.MasterID WHERE t1.ItemsID='" +
                        drtt["ID"].ToString() + "' ";
                    DataTable dttCartoon = DataManager.ExecuteQuery(DataManager.OraConnString(), SelectQuery1,
                        "ShiftmentBoxingItemsDtl");
                    foreach (DataRow drr in dttCartoon.Rows)
                    {
                        if (Cortoon == "")
                        {
                            Cortoon = drr["CartoonNo"].ToString();
                        }
                        else
                        {
                            Cortoon += " , " + drr["CartoonNo"].ToString();
                        }

                        tt++;
                    }

                    cell = new PdfPCell(FormatFontPhrase(serial.ToString()));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    //cell.BorderWidth = 0f;
                    // cell.Colspan = 2;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatFontPhrase(Cortoon));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    //cell.BorderWidth = 0f;
                    // cell.Colspan = 2;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatFontPhrase(tt.ToString()));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    //cell.BorderWidth = 0f;
                    // cell.Colspan = 2;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatFontPhrase(drtt["supplier"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    //cell.BorderWidth = 0f;
                    //cell.FixedHeight = 20f;
                    //cell.Colspan = 2;
                    pdtdtl.AddCell(cell);
                    cell = new PdfPCell(FormatFontPhrase(drtt["Name"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    //cell.BorderWidth = 0f;
                    //cell.FixedHeight = 20f;
                    //cell.Colspan = 2;
                    pdtdtl.AddCell(cell);
                    cell = new PdfPCell(FormatFontPhrase(drtt["Label"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    //cell.BorderWidth = 0f;
                    // cell.Colspan = 2;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatFontPhrase(drtt["PartyRate"].ToString()));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    //cell.BorderWidth = 0f;
                    // cell.Colspan = 2;
                    pdtdtl.AddCell(cell);

                    double tot = ItemsTotal(allTot, drtt);
                    cell = new PdfPCell(FormatFontPhrase(tot.ToString("N0")));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    //cell.BorderWidth = 0f;
                    // cell.Colspan = 2;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatFontPhrase((Convert.ToDouble(drtt["PartyRate"]) * tot).ToString("N3")));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    //cell.BorderWidth = 0f;
                    // cell.Colspan = 2;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatFontPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    //cell.BorderWidth = 0f;
                    // cell.Colspan = 2;
                    pdtdtl.AddCell(cell);

                    totAmt += Convert.ToDecimal(drtt["totAmount"]);
                    allTot += tot;
                    serial++;
                }

                cell = new PdfPCell(FormatHeaderPhrase("Total "));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Colspan = 7;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(allTot.ToString("N0")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                //cell.Colspan = 2;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(totAmt.ToString("N2")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                //cell.Colspan = 2;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                //cell.Colspan = 2;
                pdtdtl.AddCell(cell);
            }

            document.Add(pdtdtl);
            document.Close();
        }
    }

    //************* Check Courtoon Quantity ***************//
    private static double ItemsTotal(double allTot, DataRow drtt)
    {

        //double tot1 = 0;
        double tot2 = 0;
        DataTable dt11 = ShiftmentItemsCartoonManager.getShiftmentItemsCartoonItemsQuantity(drtt["ID"].ToString());
        if (dt11 != null)
        {
            for (int i = 0; i < dt11.Rows.Count; i++)
            {
                int ff = 0;
                for (int j = 1; j < dt11.Columns.Count; j++)
                {
                    if (dt11.Rows[i][j].ToString() != "" && ff > 0)
                    {
                        //tot1 += Convert.ToDouble(dt11.Rows[i][j].ToString());
                        tot2 += Convert.ToDouble(dt11.Rows[i][j].ToString());
                        //allTot += Convert.ToDouble(dt11.Rows[i][j].ToString());
                    }
                    ff++;
                }

            }
        }
        return tot2;
    }
    private void ShipmentBarcodeReport(HttpContext context, MemoryStream memoryStream, string CartonNo1, string CartonNo2,
        string ShipmentID, string Type)
    {
        Document document = new Document(PageSize.A4, 30f, 30f, 10f, 10f);
        PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
        document.Open();
        iTextSharp.text.Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;

        barcode.Alignment = BarcodeLib.AlignmentPositions.CENTER;
        int W = 550;
        int H = 160;

        BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128;
        barcode.IncludeLabel = false;
        barcode.RotateFlipType = (RotateFlipType) Enum.Parse(typeof (RotateFlipType), "RotateNoneFlipNone", true);
        barcode.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;

        DataTable dtMst = ShiftmentItemsManager.GetShowItemsInfo_Barcode(CartonNo1, CartonNo2, ShipmentID);
        if (dtMst.Rows.Count > 0)
        {
            int Page1 = 0;
            foreach (DataRow row in dtMst.Rows)
            {
                System.Drawing.Image generatedBarcode = barcode.Encode(type, row["MasterID"].ToString(), Color.Black,
                    Color.White, W, H);
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                generatedBarcode.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                byte[] logo2 = stream.ToArray();
                iTextSharp.text.Image gif2 = iTextSharp.text.Image.GetInstance(logo2);
                gif2.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                gif2.ScalePercent(12f);

                PdfPCell cell;
                byte[] logo = GlBookManager.GetGlLogo(context.Session["book"].ToString());
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
                gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                gif.ScalePercent(8f);

                float[] titwidth = new float[3] {15, 40, 15};
                PdfPTable dth = new PdfPTable(titwidth);
                dth.WidthPercentage = 100;


                cell = new PdfPCell(gif);
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.VerticalAlignment = Element.ALIGN_TOP;
                cell.HorizontalAlignment = Element.ALIGN_TOP;
                cell.Rowspan = 4;
                cell.BorderWidth = 0f;
                dth.AddCell(cell);

                cell =
                    new PdfPCell(new Phrase(context.Session["org"].ToString(),
                        FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                // cell.FixedHeight = 20f;
                dth.AddCell(cell);

                cell =
                    new PdfPCell(new Phrase(row["MasterID"].ToString(),
                        FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                // cell.FixedHeight = 20f;
                dth.AddCell(cell);

                cell =
                    new PdfPCell(new Phrase(context.Session["add1"].ToString(),
                        FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                // cell.FixedHeight = 20f;
                dth.AddCell(cell);

                cell = new PdfPCell(gif2);
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                cell.Rowspan = 3;
                cell.BorderWidth = 0f;
                dth.AddCell(cell);


                cell =
                    new PdfPCell(new Phrase(context.Session["add2"].ToString(),
                        FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                //cell.FixedHeight = 20f;
                dth.AddCell(cell);
                cell =
                    new PdfPCell(new Phrase("Items Image",
                        FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                //cell.FixedHeight = 30f;
                dth.AddCell(cell);
                document.Add(dth);
                //LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
                //document.Add(line);

                PdfPTable dtempty = new PdfPTable(1);
                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.BorderWidth = 0f;
                cell.FixedHeight = 10f;
                dtempty.AddCell(cell);
                document.Add(dtempty);
                int serial = 0;
                //foreach (DataRow drr in dtMst.Rows)
                //{
                float[] MB = new float[1] {100};
                PdfPTable pdMB = new PdfPTable(MB);
                pdMB.WidthPercentage = 100;

                float[] widthdtl = new float[6] {20, 20, 20, 20, 20, 20};
                PdfPTable pdtdtl = new PdfPTable(widthdtl);
                pdtdtl.WidthPercentage = 100;

                if (serial == 0)
                {
                    cell = new PdfPCell(FormatHeaderPhrase("CTN No : " + row["CartoonNo"].ToString()));
                }
                else
                {
                    cell = new PdfPCell(FormatHeaderPhrase(""));
                }
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Colspan = 2;
                cell.BorderWidth = 0f;
                pdtdtl.AddCell(cell);

                if (serial == 0)
                {
                    cell = new PdfPCell(FormatHeaderPhrase("Shiftment No. : " + row["ShiftmentNO"].ToString()));
                }
                else
                {
                    cell = new PdfPCell(FormatHeaderPhrase(""));
                }
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Colspan = 2;
                cell.BorderWidth = 0f;
                pdtdtl.AddCell(cell);

                if (serial == 0)
                {
                    cell =
                        new PdfPCell(
                            FormatHeaderPhrase("Date : " +
                                               (Delve.DataManager.DateEncode(row["ShiftmentDate"].ToString())).ToString(
                                                   IdManager.DateFormat())));
                }
                else
                {
                    cell = new PdfPCell(FormatHeaderPhrase(""));
                }
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Colspan = 2;
                cell.BorderWidth = 0f;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Colspan = 6;
                //cell.FixedHeight = 8f;
                cell.BorderWidth = 0f;
                pdtdtl.AddCell(cell);

                //cell = new PdfPCell(FormatHeaderPhrase("Supplier Name"));
                //cell.HorizontalAlignment = 1;
                //cell.VerticalAlignment = 1;
                ////cell.BorderWidth = 0f;
                ////cell.FixedHeight = 20f;
                //cell.Colspan = 2;
                //pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Items Name"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0f;
                //cell.FixedHeight = 20f;
                cell.Colspan = 4;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Label"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0f;
                // cell.FixedHeight = 30f;
                cell.Colspan = 2;
                pdtdtl.AddCell(cell);
                if (dtMst.Rows.Count > 0)
                {
                    //cell = new PdfPCell(FormatFontPhrase(row["SupplierName"].ToString()));
                    //cell.HorizontalAlignment = 0;
                    //cell.VerticalAlignment = 1;
                    ////cell.BorderWidth = 0f;
                    ////cell.FixedHeight = 20f;
                    //cell.Colspan = 2;
                    //pdtdtl.AddCell(cell);
                    cell = new PdfPCell(FormatFontPhrase(row["Name"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    //cell.BorderWidth = 0f;
                    //cell.FixedHeight = 20f;
                    cell.Colspan = 4;
                    pdtdtl.AddCell(cell);
                    cell = new PdfPCell(FormatFontPhrase(row["Label"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    //cell.BorderWidth = 0f;
                    cell.Colspan = 2;
                    pdtdtl.AddCell(cell);
                }
                cell = new PdfPCell(FormatFontPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                //cell.FixedHeight = 8;
                cell.Colspan = 6;
                pdtdtl.AddCell(cell);
                if (Type.Equals("SBRI"))
                {
                    DataTable dt =
                        Delve.IdManager.GetShowDataTable(
                            "SELECT '' AS imagename,[ID] AS ImageID, [Image] AS Image from [ShiftmentBoxingItemsImage] where [BoxingItemsID] ='" +
                            row["MasterID"].ToString() + "' and ItemsID='" + row["SHITEMSid"].ToString() + "' ");
                    if (dt.Rows.Count > 0)
                    {
                        PdfPTable pdtclient = new PdfPTable(dt.Rows.Count);
                        pdtclient.WidthPercentage = 100;
                        decimal tt = decimal.Zero;
                        decimal tot = (6/dt.Rows.Count);
                        tt = 6*Math.Ceiling(tot);
                        for (int i = 0; i < tt; i++)
                        {
                            if (dt.Rows.Count - 1 < i)
                            {
                                cell = new PdfPCell(FormatHeaderPhrase(""));
                                cell.HorizontalAlignment = 1;
                                cell.VerticalAlignment = 1;
                                cell.BorderWidth = 0f;
                                pdtdtl.AddCell(cell);
                            }
                            else
                            {
                                DataRow dr = dt.Rows[i];
                                byte[] logo1 = (byte[]) dr["Image"];
                                iTextSharp.text.Image gif1 = iTextSharp.text.Image.GetInstance(logo1);
                                gif1.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                                gif1.ScalePercent(30f);
                                cell = new PdfPCell(gif1);
                                cell.PaddingBottom = 10f;
                                cell.HorizontalAlignment = 1;
                                cell.VerticalAlignment = 1;
                                cell.BorderWidth = 0f;
                                pdtdtl.AddCell(cell);
                            }
                        }
                    }
                }
                //DataTable dt11 = ShiftmentItemsManager.GetShiftmentItemsQuantity(txtID.Text);
                DataTable dt11 =
                    ShiftmentItemsCartoonManager.getShiftmentItemsCartoonItemsQuantity(row["MasterID"].ToString(),
                        row["SHITEMSid"].ToString());
                if (dt11 != null)
                {
                    float[] widthbi = new float[dt11.Columns.Count];
                    for (int i = 0; i < dt11.Columns.Count; i++)
                    {
                        widthbi[i] = 100/dt11.Columns.Count;
                    }
                    PdfPTable with1 = new PdfPTable(widthbi);
                    with1.WidthPercentage = 100;

                    //cell = new PdfPCell(FormatHeaderPhrase("Color name"));
                    //cell.HorizontalAlignment = 1;
                    //cell.VerticalAlignment = 1;
                    //with1.AddCell(cell);

                    for (int i = 1; i < dt11.Columns.Count; i++)
                    {
                        cell = new PdfPCell(FormatHeaderPhrase(dt11.Columns[i].ColumnName));
                        cell.HorizontalAlignment = 1;
                        cell.VerticalAlignment = 1;
                        with1.AddCell(cell);
                    }
                    cell = new PdfPCell(FormatHeaderPhrase("Total"));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    with1.AddCell(cell);
                    int ff = 0;
                    double tot1 = 0;
                    double tot2 = 0;
                    for (int i = 0; i < dt11.Rows.Count; i++)
                    {
                        for (int j = 1; j < dt11.Columns.Count; j++)
                        {
                            cell = new PdfPCell(FormatFontPhrase(dt11.Rows[i][j].ToString()));
                            if (ff == 0)
                            {
                                cell.HorizontalAlignment = 0;
                            }
                            else
                            {
                                cell.HorizontalAlignment = 1;
                            }
                            cell.VerticalAlignment = 1;
                            with1.AddCell(cell);
                            if (dt11.Rows[i][j].ToString() != "" && ff > 0)
                            {
                                tot1 += Convert.ToDouble(dt11.Rows[i][j].ToString());
                                tot2 += Convert.ToDouble(dt11.Rows[i][j].ToString());
                            }
                            ff++;
                        }
                        cell = new PdfPCell(FormatFontPhrase(tot1.ToString("N0")));
                        cell.HorizontalAlignment = 2;
                        cell.VerticalAlignment = 1;
                        with1.AddCell(cell);
                        ff = 0;
                        tot1 = 0;
                    }
                    cell = new PdfPCell(FormatHeaderPhrase("Total"));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = dt11.Columns.Count - 1;
                    with1.AddCell(cell);
                    cell = new PdfPCell(FormatHeaderPhrase(tot2.ToString("N0")));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = dt11.Columns.Count - 1;
                    with1.AddCell(cell);

                    string Remarks = Delve.IdManager.GetShowSingleValueString("Remarks", "ID", "ShiftmentBoxingMst",
                        row["MasterID"].ToString());
                    cell = new PdfPCell(FormatHeaderPhrase("Remarks : " + Remarks));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = dt11.Columns.Count - 1 + dt11.Columns.Count - 1;
                    with1.AddCell(cell);

                    cell = new PdfPCell(pdtdtl);
                    cell.BorderWidth = 1f;
                    pdMB.AddCell(cell);

                    cell = new PdfPCell(with1);
                    cell.BorderWidth = 1f;
                    //cell.FixedHeight = 20;
                    pdMB.AddCell(cell);
                    document.Add(pdMB);

                    PdfPTable dtempty1 = new PdfPTable(1);
                    cell = new PdfPCell(FormatHeaderPhrase(""));
                    cell.BorderWidth = 0f;
                    cell.FixedHeight = 10f;
                    dtempty1.AddCell(cell);
                    document.Add(dtempty1);

                    serial++;
                }
                //}

                Page1++;
                if (Page1 == 3)
                {
                    document.NewPage();
                    Page1 = 0;
                }
            }

            document.Close();
        }
    }

    private static void StyleWiseReport(HttpContext context, MemoryStream memoryStream, string ShipmentName, string ItemID,
        string ItemSearchID)
    {
        Document document = new Document(PageSize.A4);
        PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
        document.Open();
        iTextSharp.text.Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;

        PdfPCell cell;
        byte[] logo = Delve.GlBookManager.GetGlLogo(context.Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(8f);

        float[] titwidth = new float[3] {15, 40, 15};
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.VerticalAlignment = Element.ALIGN_TOP;
        cell.HorizontalAlignment = Element.ALIGN_TOP;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);

        cell =
            new PdfPCell(new Phrase(context.Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        dth.AddCell(cell);

        cell = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);

        cell =
            new PdfPCell(new Phrase(context.Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(context.Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        dth.AddCell(cell);

        cell =
            new PdfPCell(new Phrase("Shipment No. :  " + ShipmentName + "  - Style Wise Items List.",
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        int serial = 0;
        //foreach (DataRow drtt in dtCartoon.Rows)
        //{

        //DataTable dtMst = ShiftmentItemsManager.GetShowItemsInfo(drtt["ID"].ToString());
        //foreach (DataRow drr in dtMst.Rows)
        //{
        float[] MB = new float[1] {100};
        PdfPTable pdMB = new PdfPTable(MB);
        pdMB.WidthPercentage = 100;

        float[] widthdtl = new float[6] {20, 20, 20, 20, 20, 20};
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;

        if (serial > 0)
        {
            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.Colspan = 6;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 15f;
            pdtdtl.AddCell(cell);
        }

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Colspan = 6;
        cell.FixedHeight = 8f;
        cell.BorderWidth = 0f;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Items Name"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        pdtdtl.AddCell(cell);
        string Name = Delve.IdManager.GetShowSingleValueString("t2.Code+' - '+t2.Name", "t2.ID", "Item t2", ItemID);
        cell = new PdfPCell(FormatFontPhrase(Name));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Colspan = 5;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatFontPhrase(""));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 8;
        cell.Colspan = 6;
        pdtdtl.AddCell(cell);
        DataTable dt =
            Delve.IdManager.GetShowDataTable(
                "SELECT DISTINCT t1.ImageName AS imagename, t1.[Image] AS Image from [ShiftmentBoxingItemsImage] t1 inner join ShiftmentItems t2 on t2.ID=t1.ItemsID WHERE t2.ItemsID = '" +
                ItemSearchID + "'");
        if (dt.Rows.Count > 0)
        {
            PdfPTable pdtclient = new PdfPTable(dt.Rows.Count);
            pdtclient.WidthPercentage = 100;
            decimal tt = decimal.Zero;
            decimal tot = (6/dt.Rows.Count);
            tt = 6*Math.Ceiling(tot);
            for (int i = 0; i < tt; i++)
            {
                if (dt.Rows.Count - 1 < i)
                {
                    cell = new PdfPCell(FormatHeaderPhrase(""));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0f;
                    pdtdtl.AddCell(cell);
                }
                else
                {
                    DataRow dr = dt.Rows[i];
                    byte[] logo1 = (byte[]) dr["Image"];
                    iTextSharp.text.Image gif1 = iTextSharp.text.Image.GetInstance(logo1);
                    gif1.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                    gif1.ScalePercent(30f);
                    cell = new PdfPCell(gif1);
                    cell.PaddingBottom = 10f;
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0f;
                    pdtdtl.AddCell(cell);
                }
            }
        }
        DataTable dt11 = ShiftmentItemsCartoonManager.getShiftmentItemsCartoonStyleWise(ItemSearchID);
        if (dt11 != null)
        {
            float[] widthbi = new float[dt11.Columns.Count];
            for (int i = 0; i < dt11.Columns.Count; i++)
            {
                widthbi[i] = 100/dt11.Columns.Count;
            }
            PdfPTable with1 = new PdfPTable(widthbi);
            with1.WidthPercentage = 100;

            cell = new PdfPCell(FormatHeaderPhrase("Color Name"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            with1.AddCell(cell);

            for (int i = 2; i < dt11.Columns.Count; i++)
            {
                cell = new PdfPCell(FormatHeaderPhrase(dt11.Columns[i].ColumnName));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                with1.AddCell(cell);
            }
            cell = new PdfPCell(FormatHeaderPhrase("Total"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            with1.AddCell(cell);
            int ff = 0;
            double tot1 = 0;
            double tot2 = 0;
            for (int i = 0; i < dt11.Rows.Count; i++)
            {
                for (int j = 1; j < dt11.Columns.Count; j++)
                {
                    cell = new PdfPCell(FormatFontPhrase(dt11.Rows[i][j].ToString()));
                    if (ff == 0)
                    {
                        cell.HorizontalAlignment = 0;
                    }
                    else
                    {
                        cell.HorizontalAlignment = 1;
                    }
                    cell.VerticalAlignment = 1;
                    with1.AddCell(cell);
                    if (dt11.Rows[i][j].ToString() != "" && ff > 0)
                    {
                        tot1 += Convert.ToDouble(dt11.Rows[i][j].ToString());
                        tot2 += Convert.ToDouble(dt11.Rows[i][j].ToString());
                    }
                    ff++;
                }
                cell = new PdfPCell(FormatFontPhrase(tot1.ToString("N0")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                with1.AddCell(cell);
                ff = 0;
                tot1 = 0;
            }
            cell = new PdfPCell(FormatHeaderPhrase("Total"));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Colspan = dt11.Columns.Count - 1;
            with1.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(tot2.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Colspan = dt11.Columns.Count - 1;
            with1.AddCell(cell);

            cell = new PdfPCell(pdtdtl);
            cell.BorderWidth = 1f;
            pdMB.AddCell(cell);

            cell = new PdfPCell(with1);
            cell.BorderWidth = 1f;
            pdMB.AddCell(cell);
            document.Add(pdMB);
            serial++;
        }
        document.Close();
    }

    private static void SupplierAndStyleWiseReport(HttpContext context, string SupplierName, string SupplierID,
        string ShipmentID, MemoryStream memoryStream, string ShipmentName)
    {
        double allTot = 0;
        string parameter = "";
        if (SupplierName != "")
        {
            parameter = " AND t1.[SupplierID]='" + SupplierID + "'";
        }

        string SelectQuery =
            @"SELECT t1.[ID],t1.[ShiftmentID],t2.Name,t3.ShiftmentNO,t1.Label,t4.ContactName AS PartyName FROM ShiftmentItems t1 inner join Item t2 on t2.ID=t1.ItemsID inner join ShiftmentAssigen t3 on t3.ID=t1.ShiftmentID inner join dbo.Supplier t4 on t4.id=t1.SupplierID WHERE t1.ShiftmentID='" +
            ShipmentID + "' " + parameter + " order by t1.PartyID  ";
        DataTable dtCartoon = Delve.DataManager.ExecuteQuery(Delve.DataManager.OraConnString(), SelectQuery,
            "Temp_ShiftmentItemsColorSize");
        if (dtCartoon.Rows.Count > 0)
        {
            Document document = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();
            iTextSharp.text.Rectangle page = document.PageSize;
            PdfPTable head = new PdfPTable(1);
            head.TotalWidth = page.Width - 50;

            PdfPCell cell;
            byte[] logo = Delve.GlBookManager.GetGlLogo(context.Session["book"].ToString());
            iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
            gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
            gif.ScalePercent(8f);

            float[] titwidth = new float[3] {15, 40, 15};
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

            cell = new PdfPCell(gif);
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.HorizontalAlignment = Element.ALIGN_TOP;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase(context.Session["org"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);

            cell = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase(context.Session["add1"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase(context.Session["add2"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase("Shipment No. : ( " + ShipmentName + " ) - Supplier & Style Wise Items List.",
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 30f;
            dth.AddCell(cell);
            document.Add(dth);

            iTextSharp.text.pdf.draw.LineSeparator line = new iTextSharp.text.pdf.draw.LineSeparator(0f, 100, null,
                Element.ALIGN_CENTER, -2);
            document.Add(line);

            PdfPTable dtempty = new PdfPTable(1);
            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.BorderWidth = 0f;
            cell.FixedHeight = 10f;
            dtempty.AddCell(cell);
            document.Add(dtempty);

            int serial = 0;

            foreach (DataRow drtt in dtCartoon.Rows)
            {
                string Cortoon = "";
                //DataTable dtMst = ShiftmentItemsManager.GetShowItemsInfo(drtt["ID"].ToString());
                //foreach (DataRow drr in dtMst.Rows)
                //{
                float[] MB = new float[1] {100};
                PdfPTable pdMB = new PdfPTable(MB);
                pdMB.WidthPercentage = 100;

                float[] widthdtl = new float[6] {20, 20, 20, 20, 20, 20};
                PdfPTable pdtdtl = new PdfPTable(widthdtl);
                pdtdtl.WidthPercentage = 100;

                if (serial > 0)
                {
                    cell = new PdfPCell(FormatHeaderPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = 6;
                    cell.BorderWidth = 0f;
                    cell.FixedHeight = 15f;
                    pdtdtl.AddCell(cell);
                }
                string SelectQuery1 =
                    @"SELECT DISTINCT t2.CartoonNo FROM [ShiftmentBoxingItemsDtl] t1 INNER JOIN ShiftmentBoxingMst t2 on t2.ID=t1.MasterID WHERE t1.ItemsID='" +
                    drtt["ID"].ToString() + "' ";
                DataTable dttCartoon = Delve.DataManager.ExecuteQuery(Delve.DataManager.OraConnString(), SelectQuery1,
                    "ShiftmentBoxingItemsDtl");
                foreach (DataRow drr in dttCartoon.Rows)
                {
                    if (Cortoon == "")
                    {
                        Cortoon = drr["CartoonNo"].ToString();
                    }
                    else
                    {
                        Cortoon += " , " + drr["CartoonNo"].ToString();
                    }
                }
                cell = new PdfPCell(FormatHeaderPhrase("Carton No. : " + Cortoon));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Colspan = 6;
                //cell.FixedHeight = 20f;
                cell.BorderWidth = 0f;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Supplier Name"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.BorderWidth = 0f;
                //cell.FixedHeight = 20f;
                cell.Colspan = 2;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Items Name"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0f;
                //cell.FixedHeight = 20f;
                cell.Colspan = 2;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Label"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0f;
                // cell.FixedHeight = 30f;
                cell.Colspan = 2;
                pdtdtl.AddCell(cell);
                //if (dtMst.Rows.Count > 0)
                //{
                cell = new PdfPCell(FormatFontPhrase(drtt["PartyName"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                //cell.BorderWidth = 0f;
                //cell.FixedHeight = 20f;
                cell.Colspan = 2;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatFontPhrase(drtt["Name"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                //cell.BorderWidth = 0f;
                //cell.FixedHeight = 20f;
                cell.Colspan = 2;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatFontPhrase(drtt["Label"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                //cell.BorderWidth = 0f;
                cell.Colspan = 2;
                pdtdtl.AddCell(cell);
                //}
                cell = new PdfPCell(FormatFontPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                cell.FixedHeight = 8;
                cell.Colspan = 6;
                pdtdtl.AddCell(cell);
                DataTable dt =
                    Delve.IdManager.GetShowDataTable(
                        "SELECT DISTINCT ImageName AS imagename, [Image] AS Image from [ShiftmentBoxingItemsImage] t1 where  t1.ItemsID='" +
                        drtt["ID"].ToString() + "'");
                if (dt.Rows.Count > 0)
                {
                    PdfPTable pdtclient = new PdfPTable(dt.Rows.Count);
                    pdtclient.WidthPercentage = 100;
                    decimal tt = decimal.Zero;
                    decimal tot = (6/dt.Rows.Count);
                    tt = 6*Math.Ceiling(tot);
                    for (int i = 0; i < tt; i++)
                    {
                        if (dt.Rows.Count - 1 < i)
                        {
                            cell = new PdfPCell(FormatHeaderPhrase(""));
                            cell.HorizontalAlignment = 1;
                            cell.VerticalAlignment = 1;
                            cell.BorderWidth = 0f;
                            pdtdtl.AddCell(cell);
                        }
                        else
                        {
                            DataRow dr = dt.Rows[i];
                            byte[] logo1 = (byte[]) dr["Image"];
                            iTextSharp.text.Image gif1 = iTextSharp.text.Image.GetInstance(logo1);
                            gif1.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                            gif1.ScalePercent(30f);
                            cell = new PdfPCell(gif1);
                            cell.PaddingBottom = 10f;
                            cell.HorizontalAlignment = 1;
                            cell.VerticalAlignment = 1;
                            cell.BorderWidth = 0f;
                            pdtdtl.AddCell(cell);
                        }
                    }
                }
                DataTable dt11 = ShiftmentItemsCartoonManager.getShiftmentItemsCartoonItemsQuantity(drtt["ID"].ToString());
                if (dt11 != null)
                {
                    float[] widthbi = new float[dt11.Columns.Count];
                    for (int i = 0; i < dt11.Columns.Count; i++)
                    {
                        widthbi[i] = 100/dt11.Columns.Count;
                    }
                    PdfPTable with1 = new PdfPTable(widthbi);
                    with1.WidthPercentage = 100;

                    cell = new PdfPCell(FormatHeaderPhrase("Color Name"));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    with1.AddCell(cell);

                    for (int i = 2; i < dt11.Columns.Count; i++)
                    {
                        cell = new PdfPCell(FormatHeaderPhrase(dt11.Columns[i].ColumnName));
                        cell.HorizontalAlignment = 1;
                        cell.VerticalAlignment = 1;
                        with1.AddCell(cell);
                    }
                    cell = new PdfPCell(FormatHeaderPhrase("Total (Pch)"));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    with1.AddCell(cell);
                    int ff = 0;
                    double tot1 = 0;
                    double tot2 = 0;
                    for (int i = 0; i < dt11.Rows.Count; i++)
                    {
                        for (int j = 1; j < dt11.Columns.Count; j++)
                        {
                            cell = new PdfPCell(FormatFontPhrase(dt11.Rows[i][j].ToString()));
                            if (ff == 0)
                            {
                                cell.HorizontalAlignment = 0;
                            }
                            else
                            {
                                cell.HorizontalAlignment = 1;
                            }
                            cell.VerticalAlignment = 1;
                            with1.AddCell(cell);
                            if (dt11.Rows[i][j].ToString() != "" && ff > 0)
                            {
                                tot1 += Convert.ToDouble(dt11.Rows[i][j].ToString());
                                tot2 += Convert.ToDouble(dt11.Rows[i][j].ToString());
                                allTot += Convert.ToDouble(dt11.Rows[i][j].ToString());
                            }
                            ff++;
                        }
                        cell = new PdfPCell(FormatFontPhrase(tot1.ToString("N0")));
                        cell.HorizontalAlignment = 2;
                        cell.VerticalAlignment = 1;
                        with1.AddCell(cell);
                        ff = 0;
                        tot1 = 0;
                    }
                    cell = new PdfPCell(FormatHeaderPhrase("Total"));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = dt11.Columns.Count - 1;
                    with1.AddCell(cell);
                    cell = new PdfPCell(FormatHeaderPhrase(tot2.ToString("N0")));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = dt11.Columns.Count - 1;
                    with1.AddCell(cell);

                    cell = new PdfPCell(pdtdtl);
                    cell.BorderWidth = 1f;
                    pdMB.AddCell(cell);

                    cell = new PdfPCell(with1);
                    cell.BorderWidth = 1f;
                    pdMB.AddCell(cell);
                    document.Add(pdMB);
                    serial++;
                }
                //}
            }
            float[] widthtt = new float[6] {20, 20, 20, 20, 20, 20};
            PdfPTable pdtdtltt = new PdfPTable(widthtt);
            pdtdtltt.WidthPercentage = 100;
            cell = new PdfPCell(FormatHeaderPhrase("All Total "));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Colspan = 5;
            pdtdtltt.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(allTot.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.Colspan = 2;
            pdtdtltt.AddCell(cell);
            document.Add(pdtdtltt);
            document.Close();
        }
    }

    //***************** ShipmentWiseCartonReport ************************//
    
    private static void ShipmentWiseCartonReport(HttpContext context, string CartonNo, string ShipmentID,
        MemoryStream memoryStream)
    {
        string parameter = "";
        if (CartonNo != "")
        {
            parameter = " AND CartoonNo='" + CartonNo + "'";
        }
        string SelectQuery = @"SELECT [ID],[CartoonNo],[ShiftmentID] FROM [ShiftmentBoxingMst] WHERE ShiftmentID='" +
                             ShipmentID + "' " + parameter + " ";
        DataTable dtCartoon = Delve.DataManager.ExecuteQuery(Delve.DataManager.OraConnString(), SelectQuery,
            "Temp_ShiftmentItemsColorSize");
        if (dtCartoon.Rows.Count > 0)
        {
            string filename = "ShiftmentWiseCartoonReport-" + DateTime.Now.ToString("dd/MM/yyyy");
            Document document = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();
            iTextSharp.text.Rectangle page = document.PageSize;
            PdfPTable head = new PdfPTable(1);
            head.TotalWidth = page.Width - 50;

            PdfPCell cell;
            byte[] logo = Delve.GlBookManager.GetGlLogo(context.Session["book"].ToString());
            iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
            gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
            gif.ScalePercent(8f);

            float[] titwidth = new float[3] {15, 40, 15};
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

            cell = new PdfPCell(gif);
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.HorizontalAlignment = Element.ALIGN_TOP;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase(context.Session["org"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);

            cell = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase(context.Session["add1"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase(context.Session["add2"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell =
                new PdfPCell(
                    new Phrase(
                        " Shipment  No. : ( " + context.Session["ShipmentName"].ToString() + " ) No Wise Product List.",
                        FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 30f;
            dth.AddCell(cell);
            document.Add(dth);

            PdfPTable dtempty = new PdfPTable(1);
            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.BorderWidth = 0f;
            cell.FixedHeight = 10f;
            dtempty.AddCell(cell);
            document.Add(dtempty);

            foreach (DataRow drtt in dtCartoon.Rows)
            {
                int serial = 0;
                DataTable dtMst = ShiftmentItemsManager.GetShowItemsInfo(drtt["ID"].ToString(), "");
                foreach (DataRow drr in dtMst.Rows)
                {
                    float[] MB = new float[1] {100};
                    PdfPTable pdMB = new PdfPTable(MB);
                    pdMB.WidthPercentage = 100;

                    float[] widthdtl = new float[6] {20, 20, 20, 20, 20, 20};
                    PdfPTable pdtdtl = new PdfPTable(widthdtl);
                    pdtdtl.WidthPercentage = 100;

                    if (serial == 0)
                    {
                        cell = new PdfPCell(FormatHeaderPhrase("CTN No : " + drr["CartoonNo"].ToString()));
                    }
                    else
                    {
                        cell = new PdfPCell(FormatHeaderPhrase(""));
                    }
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = 2;
                    cell.BorderWidth = 0f;
                    pdtdtl.AddCell(cell);

                    if (serial == 0)
                    {
                        cell = new PdfPCell(FormatHeaderPhrase(""));
                    }
                    else
                    {
                        cell = new PdfPCell(FormatHeaderPhrase(""));
                    }
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = 2;
                    cell.BorderWidth = 0f;
                    pdtdtl.AddCell(cell);

                    if (serial == 0)
                    {
                        cell = new PdfPCell(FormatHeaderPhrase(""));
                    }
                    else
                    {
                        cell = new PdfPCell(FormatHeaderPhrase(""));
                    }
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = 2;
                    cell.BorderWidth = 0f;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatHeaderPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = 6;
                    cell.FixedHeight = 8f;
                    cell.BorderWidth = 0f;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatHeaderPhrase("Party Name"));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    //cell.BorderWidth = 0f;
                    //cell.FixedHeight = 20f;
                    cell.Colspan = 2;
                    pdtdtl.AddCell(cell);
                    cell = new PdfPCell(FormatHeaderPhrase("Items Name"));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    // cell.BorderWidth = 0f;
                    //cell.FixedHeight = 20f;
                    cell.Colspan = 2;
                    pdtdtl.AddCell(cell);
                    cell = new PdfPCell(FormatHeaderPhrase("Label"));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    // cell.BorderWidth = 0f;
                    // cell.FixedHeight = 30f;
                    cell.Colspan = 2;
                    pdtdtl.AddCell(cell);
                    if (dtMst.Rows.Count > 0)
                    {
                        cell = new PdfPCell(FormatFontPhrase(drr["PartyName"].ToString()));
                        cell.HorizontalAlignment = 0;
                        cell.VerticalAlignment = 1;
                        //cell.BorderWidth = 0f;
                        //cell.FixedHeight = 20f;
                        cell.Colspan = 2;
                        pdtdtl.AddCell(cell);
                        cell = new PdfPCell(FormatFontPhrase(drr["Name"].ToString()));
                        cell.HorizontalAlignment = 0;
                        cell.VerticalAlignment = 1;
                        //cell.BorderWidth = 0f;
                        //cell.FixedHeight = 20f;
                        cell.Colspan = 2;
                        pdtdtl.AddCell(cell);
                        cell = new PdfPCell(FormatFontPhrase(drr["Label"].ToString()));
                        cell.HorizontalAlignment = 0;
                        cell.VerticalAlignment = 1;
                        //cell.BorderWidth = 0f;
                        cell.Colspan = 2;
                        pdtdtl.AddCell(cell);
                    }
                    cell = new PdfPCell(FormatFontPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0f;
                    cell.FixedHeight = 8;
                    cell.Colspan = 6;
                    pdtdtl.AddCell(cell);
                    DataTable dt =
                        Delve.IdManager.GetShowDataTable(
                            "SELECT '' AS imagename,[ID] AS ImageID, [Image] AS Image from [ShiftmentBoxingItemsImage] where [BoxingItemsID] ='" +
                            drtt["ID"].ToString() + "' and ItemsID='" + drr["SHITEMSid"].ToString() + "' ");
                    if (dt.Rows.Count > 0)
                    {
                        PdfPTable pdtclient = new PdfPTable(dt.Rows.Count);
                        pdtclient.WidthPercentage = 100;
                        decimal tt = decimal.Zero;
                        decimal tot = (6/dt.Rows.Count);
                        tt = 6*Math.Ceiling(tot);
                        for (int i = 0; i < tt; i++)
                        {
                            if (dt.Rows.Count - 1 < i)
                            {
                                cell = new PdfPCell(FormatHeaderPhrase(""));
                                cell.HorizontalAlignment = 1;
                                cell.VerticalAlignment = 1;
                                cell.BorderWidth = 0f;
                                pdtdtl.AddCell(cell);
                            }
                            else
                            {
                                DataRow dr = dt.Rows[i];
                                byte[] logo1 = (byte[]) dr["Image"];
                                iTextSharp.text.Image gif1 = iTextSharp.text.Image.GetInstance(logo1);
                                gif1.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                                gif1.ScalePercent(30f);
                                cell = new PdfPCell(gif1);
                                cell.PaddingBottom = 10f;
                                cell.HorizontalAlignment = 1;
                                cell.VerticalAlignment = 1;
                                cell.BorderWidth = 0f;
                                pdtdtl.AddCell(cell);
                            }
                        }
                    }
                    DataTable dt11 =
                        ShiftmentItemsCartoonManager.getShiftmentItemsCartoonItemsQuantity(drtt["ID"].ToString(),
                            drr["SHITEMSid"].ToString());
                    if (dt11 != null)
                    {
                        float[] widthbi = new float[dt11.Columns.Count];
                        for (int i = 0; i < dt11.Columns.Count; i++)
                        {
                            widthbi[i] = 100/dt11.Columns.Count;
                        }
                        PdfPTable with1 = new PdfPTable(widthbi);
                        with1.WidthPercentage = 100;

                        for (int i = 1; i < dt11.Columns.Count; i++)
                        {
                            cell = new PdfPCell(FormatHeaderPhrase(dt11.Columns[i].ColumnName));
                            cell.HorizontalAlignment = 1;
                            cell.VerticalAlignment = 1;
                            with1.AddCell(cell);
                        }
                        cell = new PdfPCell(FormatHeaderPhrase("Total"));
                        cell.HorizontalAlignment = 1;
                        cell.VerticalAlignment = 1;
                        with1.AddCell(cell);
                        int ff = 0;
                        double tot1 = 0;
                        double tot2 = 0;
                        for (int i = 0; i < dt11.Rows.Count; i++)
                        {
                            for (int j = 1; j < dt11.Columns.Count; j++)
                            {
                                cell = new PdfPCell(FormatFontPhrase(dt11.Rows[i][j].ToString()));
                                if (ff == 0)
                                {
                                    cell.HorizontalAlignment = 0;
                                }
                                else
                                {
                                    cell.HorizontalAlignment = 1;
                                }
                                cell.VerticalAlignment = 1;
                                with1.AddCell(cell);
                                if (dt11.Rows[i][j].ToString() != "" && ff > 0)
                                {
                                    tot1 += Convert.ToDouble(dt11.Rows[i][j].ToString());
                                    tot2 += Convert.ToDouble(dt11.Rows[i][j].ToString());
                                }
                                ff++;
                            }
                            cell = new PdfPCell(FormatFontPhrase(tot1.ToString("N3")));
                            cell.HorizontalAlignment = 2;
                            cell.VerticalAlignment = 1;
                            with1.AddCell(cell);
                            ff = 0;
                            tot1 = 0;
                        }
                        cell = new PdfPCell(FormatHeaderPhrase("Total"));
                        cell.HorizontalAlignment = 2;
                        cell.VerticalAlignment = 1;
                        cell.Colspan = dt11.Columns.Count - 1;
                        with1.AddCell(cell);
                        cell = new PdfPCell(FormatHeaderPhrase(tot2.ToString("N3")));
                        cell.HorizontalAlignment = 2;
                        cell.VerticalAlignment = 1;
                        cell.Colspan = dt11.Columns.Count - 1;
                        with1.AddCell(cell);

                        cell = new PdfPCell(pdtdtl);
                        cell.BorderWidth = 1f;
                        pdMB.AddCell(cell);

                        cell = new PdfPCell(with1);
                        cell.BorderWidth = 1f;
                        pdMB.AddCell(cell);
                        document.Add(pdMB);
                        serial++;
                    }
                }
            }
            document.Close();
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
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
    private static Phrase FormatHeaderPhrase12(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD));
    }
    private static Phrase FormatFontPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL));
    }
}
