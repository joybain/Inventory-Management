using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

/// <summary>
/// Summary description for pdfPage
/// </summary>
/// 
namespace Delve
{
    public class pdfPage : iTextSharp.text.pdf.PdfPageEventHelper
    {
        //I create a font object to use within my footer
        protected Font footer
        {
            get
            {
                // create a basecolor to use for the footer font, if needed.
                BaseColor grey = new BaseColor(128, 128, 128);
                Font font = FontFactory.GetFont("Arial", 9, Font.NORMAL, grey);
                return font;
            }
        }
        //override the OnStartPage event handler to add our header
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            Rectangle page = document.PageSize;
            PdfPTable head = new PdfPTable(1);
            head.TotalWidth = page.Width - 20;
            Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
            PdfPCell c = new PdfPCell(phrase);
            c.Border = Rectangle.NO_BORDER;
            c.VerticalAlignment = Element.ALIGN_BOTTOM;
            c.HorizontalAlignment = Element.ALIGN_RIGHT;
            head.AddCell(c);
            head.WriteSelectedRows(0, -1, 0, page.Height - 10, writer.DirectContent);
        }

        //override the OnPageEnd event handler to add our footer
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            Rectangle page = document.PageSize;
            PdfPTable foot = new PdfPTable(1);
            foot.TotalWidth = page.Width - 20;
            Phrase phrase = new Phrase((writer.CurrentPageNumber).ToString(), new Font(Font.FontFamily.TIMES_ROMAN, 8));
            PdfPCell c = new PdfPCell(phrase);
            c.Border = Rectangle.NO_BORDER;
            c.VerticalAlignment = Element.ALIGN_BOTTOM;
            c.HorizontalAlignment = Element.ALIGN_RIGHT;
            foot.AddCell(c);
            foot.WriteSelectedRows(0, -1, 0, 20, writer.DirectContent);
        }
    }
}