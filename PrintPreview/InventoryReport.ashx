<%@ WebHandler Language="C#" Class="InventoryReport" %>

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
using iTextSharp.text.pdf.draw;

public class InventoryReport : IHttpHandler, IRequiresSessionState {
    PdfPCell cell;
    private BarcodeLib.Barcode barcode = new BarcodeLib.Barcode();
    private string SupplierID = string.Empty;
    string SupplierName = string.Empty;
    string Heading = string.Empty;
    string rbReportType = string.Empty;
    string SelectReport = string.Empty;
    string StartDate = string.Empty;
    string EndDate = string.Empty;
    string ReportType = string.Empty;
    //string CustomerID = string.Empty;
     clsItemTransferStockManager _aclsItemTransferStockManager=new clsItemTransferStockManager();
    ShiftmentItemsManager _aShiftmentItemsManager=new ShiftmentItemsManager();
    public void ProcessRequest (HttpContext context) 
    {
        string fileName, contentType;
        fileName = "oort" + ".pdf";
        byte[] bytes;
       
         SupplierID = context.Session["SupplierID"].ToString();
         SupplierName = context.Session["SupplierName"].ToString();
         Heading = context.Session["Heading"].ToString();
         rbReportType = context.Session["rbReportType"].ToString();
         SelectReport = context.Session["SelectReport"].ToString();
         StartDate = context.Session["StartDate"].ToString();
         EndDate = context.Session["txtEndDate"].ToString();
         ReportType = context.Session["ReportType"].ToString();
         //CustomerID = context.Session["CustomerID"].ToString();
        using (StringWriter sw = new StringWriter())
        {
            using (System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw))
            {

                using (MemoryStream memoryStream = new MemoryStream())
                {

                    if (ReportType.Equals("Purchase"))
                    {
                        DataTable dt = null;
                        //if (ReportType.Equals("C"))
                        //{
                        //    dt = PurchaseVoucherManager.GetShowPurchaeReport(CurrentDate, DateTime.Now.ToString("dd/MMM/yyyy"), DateTime.Now.ToString("dd/MMM/yyyy"),
                        //        ddlSupplier.SelectedValue, rbReportType.SelectedValue);
                        //}
                        //else
                        //{
                            dt = PurchaseVoucherManager.GetShowPurchaeReport(DateTime.Now.ToString("dd/MM/yyyy"), StartDate, EndDate,
                               SupplierID, rbReportType);
                       // }
                        PurchaseReport(Heading, dt, context, memoryStream);
                    }
                    else if (ReportType.Equals("Sales"))
                    {
                         DataTable dt = null;
                        //if (Type.Equals("C"))
                        //{
                        //    dt = PurchaseVoucherManager.GetShowSalesReportReport(rbReportType.SelectedValue,
                        //        DateTime.Now.ToString("dd/MMM/yyyy"), DateTime.Now.ToString("dd/MMM/yyyy"),
                        //        hfCustomerID.Value);
                        //}
                        //else
                        //{
                         dt = PurchaseVoucherManager.GetShowSalesReportReport1(rbReportType,
                             StartDate,
                             EndDate,
                             SupplierID);
                        //}
                        //if (rbPrintType.SelectedValue.Equals("P"))
                        //{
                            SalesReport(Heading, dt, context, memoryStream);
                       // }
                    }
                    else if (ReportType.Equals("TI"))
                    {
                        DataTable dt = null;
                        //dt = PurchaseVoucherManager.GetShowSalesReportReport(rbReportType,
                        //    StartDate,
                        //    EndDate,
                        //    SupplierID);
                        dt = _aclsItemTransferStockManager.GetShowTransferItemReport("", StartDate, EndDate,
                            SupplierID, "");
                        TransferReport(Heading, dt, context, memoryStream);
                    }
                    else if (ReportType.Equals("SWIS"))
                    {
                        DataTable dtItemsList = _aShiftmentItemsManager.GetShowSipmentItemsStatus(SupplierID,
            SupplierName);
                        ShipmentWiseItemsStatus(dtItemsList, context, memoryStream);
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

    private void ShipmentWiseItemsStatus(DataTable dtItemsList, HttpContext context, MemoryStream memoryStream)
    {
        
        if (dtItemsList.Rows.Count > 0)
        {
            //Response.Clear();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition",
            //    "attachment; filename=ShipmentItemsPurchase(" + DateTime.Now.ToString("dd-MMM-yyyy") + ").pdf");
            Document document = new Document(PageSize.LEGAL.Rotate(), 20f, 20f, 30f, 20f);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            document.Open();
            iTextSharp.text.Rectangle page = document.PageSize;
            PdfPTable head = new PdfPTable(1);
            head.TotalWidth = page.Width - 50;
            Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 8));
            PdfPCell c = new PdfPCell(phrase);
            c.Border = iTextSharp.text.Rectangle.NO_BORDER;
            c.VerticalAlignment = Element.ALIGN_BOTTOM;
            c.HorizontalAlignment = Element.ALIGN_RIGHT;
            head.AddCell(c);
            head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

            PdfPCell cell;
            byte[] logo = GlBookManager.GetGlLogo(context.Session["book"].ToString());
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
            cell =
                new PdfPCell(new Phrase(context.Session["org"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;

            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase(context.Session["add1"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;

            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase(context.Session["add2"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase("Shipment Wise Item's Status.",
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
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

            float[] widthdtl = new float[13] { 8, 15, 15, 25, 15, 15, 15, 15, 15, 15, 15, 15, 15 };
            PdfPTable pdtdtl = new PdfPTable(widthdtl);
            pdtdtl.WidthPercentage = 100;
            pdtdtl.HeaderRows = 1;
            int Serial = 1;
            cell = new PdfPCell(FormatHeaderPhrase("SL."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Shipment No."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Items Code"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Name"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Label"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Supplier"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Ship. Rate"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Ship. Qty."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Sales Qty."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Sales Rtn."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Transfer Qty."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Transfer Rtn."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Closing Stock"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            decimal ShipQty = 0;
            decimal SalesQty = 0;
            decimal RtnQty = 0;
            decimal transferQty = 0;
            decimal TransRtnQty = 0;
            decimal ClosingStock = 0;

            string itemsID = string.Empty;
            string ItemsIdShip = string.Empty;
            int chk = 0, borderChk = 0, subChk = 0;
            foreach (DataRow dr in dtItemsList.Rows)
            {
                if (chk.Equals(0))
                {
                    itemsID = dr["ShipItemID"].ToString();
                    ItemsIdShip = dr["StkItemsID"].ToString();
                }
                cell = new PdfPCell(FormatPhrase(Serial.ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                Serial++;

                cell = new PdfPCell(FormatPhrase(dr["ShiftmentNO"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["ItemName"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Label"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Supplier"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["PartyRate"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["ShipQty"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);


                if (subChk == 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["SalesQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;

                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else if (!ItemsIdShip.Equals(dr["StkItemsID"].ToString()) && dtItemsList.Rows.Count > 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["SalesQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;

                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthBottom = .5f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;


                    //cell.BorderWidthBottom = .5f;
                }
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0;
                pdtdtl.AddCell(cell);


                if (subChk == 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["RtnQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;

                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else if (!ItemsIdShip.Equals(dr["StkItemsID"].ToString()) && dtItemsList.Rows.Count > 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["RtnQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthBottom = .5f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                }
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                //cell.BorderWidth = 0;
                pdtdtl.AddCell(cell);

                if (subChk == 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["transferQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;

                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else if (!ItemsIdShip.Equals(dr["StkItemsID"].ToString()) && dtItemsList.Rows.Count > 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["transferQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthBottom = .5f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                }
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0;
                pdtdtl.AddCell(cell);

                if (subChk == 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["TransRtnQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;

                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else if (!ItemsIdShip.Equals(dr["StkItemsID"].ToString()) && dtItemsList.Rows.Count > 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["TransRtnQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthBottom = .5f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                }
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                pdtdtl.AddCell(cell);

                if (subChk == 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["ClosingStock"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;

                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else if (!ItemsIdShip.Equals(dr["StkItemsID"].ToString()) && dtItemsList.Rows.Count > 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["ClosingStock"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthBottom = .5f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                }
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                pdtdtl.AddCell(cell);

                if (chk > 0 && !ItemsIdShip.Equals(dr["StkItemsID"].ToString()))
                {
                    itemsID = dr["ShipItemID"].ToString();
                    ItemsIdShip = dr["StkItemsID"].ToString();
                }
                chk++;
                borderChk++;
                subChk++;
                //if (subChk == 0)
                //{
                //    if (borderChk > 19)
                //    {

                //        borderChk = 0;  document.NewPage();
                //        subChk++;
                //    }

                //}
                //else
                //{
                //    if (borderChk > 23)
                //    {

                //        borderChk = 0;  document.NewPage();
                //    }
                //}
                if (!string.IsNullOrEmpty(dr["ShipQty"].ToString()))
                    ShipQty += Convert.ToDecimal(dr["ShipQty"].ToString());
                if (!string.IsNullOrEmpty(dr["SalesQty"].ToString()))
                    SalesQty += Convert.ToDecimal(dr["SalesQty"].ToString());
                if (!string.IsNullOrEmpty(dr["RtnQty"].ToString()))
                    RtnQty += Convert.ToDecimal(dr["RtnQty"].ToString());
                if (!string.IsNullOrEmpty(dr["transferQty"].ToString()))
                    transferQty += Convert.ToDecimal(dr["transferQty"].ToString());
                if (!string.IsNullOrEmpty(dr["TransRtnQty"].ToString()))
                    TransRtnQty += Convert.ToDecimal(dr["TransRtnQty"].ToString());
                if (!string.IsNullOrEmpty(dr["ClosingStock"].ToString()))
                    ClosingStock += Convert.ToDecimal(dr["ClosingStock"].ToString());
            }

            cell = new PdfPCell(FormatHeaderPhrase("Total : "));
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 7;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(ShipQty.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(SalesQty.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(RtnQty.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(transferQty.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(TransRtnQty.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(ClosingStock.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            document.Add(pdtdtl);
            cell = SignatureFormat(document, cell);
            document.Close();
           
        }
    }
    private void TransferReport(string DDT, DataTable dt, HttpContext context, MemoryStream memoryStream)
    {
        string filename = "ItemsTransfer-" + DateTime.Now.ToString("dd/MM/yyyy");
        Document document = new Document(PageSize.LEGAL.Rotate(), 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
        document.Open();
        iTextSharp.text.Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = iTextSharp.text.Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(context.Session["book"].ToString());
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
        cell =
            new PdfPCell(new Phrase(context.Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(context.Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(context.Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);

        cell = new PdfPCell(new Phrase(DDT, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
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

        float[] widthdtl = new float[9] { 8, 15, 15, 15, 25, 15, 15, 15, 15 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;
        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Ship/Local"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Transfer Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("GRN"));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Items Code"));
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
        //cell = new PdfPCell(FormatHeaderPhrase("UOM"));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        decimal totQty = 0;
        decimal tot = 0;
        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell = new PdfPCell(FormatPhrase(dr["Ship_Local"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell =
                new PdfPCell(
                    FormatPhrase((DataManager.DateEncode(dr["TransferDate"].ToString())).ToString(IdManager.DateFormat())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Items"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["UnitPrice"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Qty"]).ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Total"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            totQty += Convert.ToDecimal(dr["Qty"]);
            tot += Convert.ToDecimal(dr["Total"]);
        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 7;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totQty.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N2")));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        //cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 9;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
       
    }

    public void PurchaseReport(string p, DataTable dt, HttpContext context, MemoryStream memoryStream)
    {
        string filename = "ItemsPurchas-" + DateTime.Now.ToString("dd/MM/yyyy");

        Document document = new Document(PageSize.LEGAL.Rotate(), 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
        document.Open();
        iTextSharp.text.Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = iTextSharp.text.Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(context.Session["book"].ToString());
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
        cell =
            new PdfPCell(new Phrase(context.Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(context.Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(context.Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        if (rbReportType.Equals("1"))
        {
            cell =
                new PdfPCell(new Phrase("BD Purchase " + p,
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        }
        else
        {
            cell =
                new PdfPCell(new Phrase("Local Purchase " + p,
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        }
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
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

        float[] widthdtl = new float[11] { 8, 15, 15, 20, 10, 25, 15, 10, 15, 15, 15 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;
        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Supplier"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Received Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("GRN"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Items Code"));
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
        cell = new PdfPCell(FormatHeaderPhrase("UOM"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        decimal totQty = 0;
        decimal tot = 0;
        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell = new PdfPCell(FormatPhrase(dr["ContactName"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell =
                new PdfPCell(
                    FormatPhrase((DataManager.DateEncode(dr["ReceivedDate"].ToString())).ToString(IdManager.DateFormat())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["GRN"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Name"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["UOM"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["PurchasePrice"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Quantity"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Total"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            totQty += Convert.ToDecimal(dr["Quantity"]);
            tot += Convert.ToDecimal(dr["Total"]);
        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 9;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totQty.ToString("N0")));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N2")));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 11;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);
        document.Close();
      
    }
    private void SalesReport(string Heading, DataTable dt, HttpContext context, MemoryStream memoryStream)
    {

        Document document = new Document(PageSize.LEGAL.Rotate(), 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
        document.Open();
        iTextSharp.text.Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = iTextSharp.text.Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(context.Session["book"].ToString());
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
        cell =
            new PdfPCell(new Phrase(context.Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(context.Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(context.Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        string Head = "";
        if (rbReportType.Equals("2"))
        {
            Head = "PH";
        }
        else
        {
            Head = "BD";
        }
        cell =
            new PdfPCell(new Phrase(Head + " - Sales " + Heading,
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        iTextSharp.text.pdf.draw.LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);
        float[] widthdtl = null;
        if (rbReportType.Equals("2"))
        {
            widthdtl = new float[12] { 8, 13, 20, 14, 16, 14, 20, 10, 10, 10, 10, 10 };
        }
        else
        {
            widthdtl = new float[11] { 8, 20, 10, 16, 14, 20, 10, 10, 10, 10, 10 };
        }
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        if (rbReportType.Equals("2"))
        {
            cell = new PdfPCell(FormatHeaderPhrase("Ship/Local"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
        }
        cell = new PdfPCell(FormatHeaderPhrase("Customer Name"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Sales Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Invoice No"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Items Code"));
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
        cell = new PdfPCell(FormatHeaderPhrase("UOM"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        double totQty = 0;
        double tot = 0;
        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;
            if (rbReportType.Equals("2"))
            {
                cell = new PdfPCell(FormatPhrase(dr["ShiftmentNO"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
            }
            cell = new PdfPCell(FormatPhrase(dr["ContactName"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell =
                new PdfPCell(
                    FormatPhrase((DataManager.DateEncode(dr["OrderDate"].ToString())).ToString(IdManager.DateFormat())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["InvoiceNo"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["Name"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["UOM"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["SalesPrice"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Quantity"]).ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["TotalPrice"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            totQty += Convert.ToDouble(dr["Quantity"]);
            tot += Convert.ToDouble(dr["TotalPrice"]);
        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 9;
        if (rbReportType.Equals("2"))
        {
            cell.Colspan = 10;
        }
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totQty.ToString("N0")));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N2")));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 11;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
    }

    private static PdfPCell SignatureFormat(Document document, PdfPCell cell)
    {
        float[] widtl = new float[5] { 20, 20, 20, 20, 20 };
        PdfPTable pdtsig = new PdfPTable(widtl);
        pdtsig.WidthPercentage = 100;
        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 5;
        cell.FixedHeight = 40f;
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
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}