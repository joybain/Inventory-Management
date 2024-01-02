﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Delve;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.pdf.draw;
using System.Reflection;

public partial class rptDayBook : System.Web.UI.Page
{
    private static string book = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            book = Session["book"].ToString();
            lblOrg.Text = Session["org"].ToString();
            lblAddress1.Text = Session["add1"].ToString();
            lblAddress2.Text = Session["add2"].ToString();
            lblTitle.Text = "Day book";
            string RepType = Request.QueryString["reptype"].ToString();
            string StartDt = Request.QueryString["startdt"].ToString();
            string EndDt = Request.QueryString["enddt"].ToString();
            string glCode = Request.QueryString["glcoa"].ToString();
            string RepLvl = Request.QueryString["replvl"].ToString();
            string SegLvl = Request.QueryString["seglvl"].ToString();
            string VchType = Request.QueryString["vchtyp"].ToString();
            string RptSysId = Request.QueryString["rptsysid"].ToString();
            string NotesNo = Request.QueryString["notes"].ToString();
            RepType = RepType.Substring(0, 1).Trim();
            lblTranStatus.Text = "";

            if (StartDt == "" | EndDt == "" | DataManager.DateEncode(EndDt).CompareTo(System.DateTime.Now) > 0)
            {
                lblTranStatus.Text = "Please select the Start Date and Upto Date, or Upto Date cannot be greater than System Date.";
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                DataTable dtVch = Delve.VouchManager.getDayBook(StartDt, EndDt, VchType, book, Session["UserType"].ToString());
                string connectionString = DataManager.OraConnString();
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connectionString;
                dgDb.DataSource = dtVch;
                dgDb.DataBind();
                ShowFooterTotal(dtVch);
            }
        }
    }

    private void ShowFooterTotal(DataTable dt)
    {
        decimal dr = decimal.Zero;
        decimal cr = decimal.Zero;
        foreach (DataRow drs in dt.Rows)
        {
            dr += decimal.Parse(drs["amount_dr"].ToString());
            cr += decimal.Parse(drs["amount_cr"].ToString());
        }
        if (dt.Rows.Count > 0)
        {
            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
            TableCell cell;
            cell = new TableCell();
            cell.Text = "";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = "";
            row.Cells.Add(cell);
            //cell = new TableCell();
            //cell.Text = "";
            //row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = "";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = "Total";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = dr.ToString("N3");
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = cr.ToString("N3");
            row.Cells.Add(cell);
            row.Font.Bold = true;
            dgDb.Controls[0].Controls.Add(row);
        }
    }
    protected void dgDb_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }
    protected void dgDb_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {

    }
    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD));
    }
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8));
    }
    protected void lbExp_Click(object sender, EventArgs e)
    {
        //Response.Clear();
        //Response.ContentType = "application/pdf";
        //Response.AddHeader("content-disposition", "attachment; filename=MypdfFile.pdf");
        //Document document = new Document();
        //document = new Document(PageSize.A4.Rotate(), 40f, 30f, 30f, 30f);
        //PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        //pdfPage page = new pdfPage();
        //writer.PageEvent = page;
        //document.Open();
        //string RepType = Request.QueryString["reptype"].ToString();
        //string StartDt = Request.QueryString["StartDt"].ToString();
        //string EndDt = Request.QueryString["EndDt"].ToString();
        //string glCode = Request.QueryString["GlCoaCode"].ToString();
        //string RepLvl = Request.QueryString["RepLvl"].ToString();
        //string SegLvl = Request.QueryString["SegLvl"].ToString();
        //string VchType = Request.QueryString["VchType"].ToString();
        //string RptSysId = Request.QueryString["rptsysid"].ToString();
        //string RptId = Request.QueryString["RptId"].ToString();
        //RepType = RepType.Substring(0, 1).Trim();
        //string title = "";
        //if (RepType == "3" | RepType == "4")
        //{
        //    title = RepType.Replace("3", "Cash book statement").Replace("4", "Bank book statement") + " as on " + EndDt;

        //}
        //else if (RepType == "5")
        //{
        //    string coaDesc = GlCoaManager.getCoaDesc(glCode);
        //    title = "Day Book " + coaDesc + " from " + StartDt + " to " + EndDt + "";
        //}
        //PdfPCell cell;
        //byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        //iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        //gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        //gif.ScalePercent(30f);
        //float[] titwidth = new float[2] { 10, 200 };
        //PdfPTable dth = new PdfPTable(titwidth);
        //dth.WidthPercentage = 100;
        //cell = new PdfPCell(gif);
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.Rowspan = 4;
        //cell.BorderWidth = 0f;
        ////cell.FixedHeight = 80f;
        //dth.AddCell(cell);
        //cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        //dth.AddCell(cell);
        //cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        //dth.AddCell(cell);
        //cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        //dth.AddCell(cell);
        //cell = new PdfPCell(new Phrase(title, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        //dth.AddCell(cell);
        //document.Add(dth);
        //LineSeparator line = new LineSeparator(1f, 100, null, Element.ALIGN_CENTER, -2);
        //document.Add(line);
        //PdfPTable dtempty = new PdfPTable(1);
        //cell = new PdfPCell(FormatHeaderPhrase(""));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 10f;
        //dtempty.AddCell(cell);
        //document.Add(dtempty);

        //DataTable dtledg = (DataTable)Session["dtledger"];
        //float[] width = new float[6] { 15, 60, 40, 30, 30, 30 };
        //PdfPTable pdtledg = new PdfPTable(width);
        //pdtledg.WidthPercentage = 100;

        //if (dtledg.Columns.Count > 6)
        //{
        //    dtledg.Columns.Remove("v_date");
        //    dtledg.Columns.Remove("value_date");
        //    dtledg.Columns.Remove("acc_type");
        //    dtledg.Columns.Remove("coa_desc");
        //}
        //cell = new PdfPCell(FormatHeaderPhrase("Voucher#"));
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("Particulars"));
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("Vch Ref No"));
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("Debit"));
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("Credit"));
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("Balance"));
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);
        //for (int i = 0; i < dtledg.Rows.Count; i++)
        //{
        //    for (int j = 0; j < dtledg.Columns.Count; j++)
        //    {
        //        string val = "";
        //        if (j > 2)
        //        {
        //            if (((DataRow)dtledg.Rows[i])[j].ToString() != "")
        //            {
        //                val = decimal.Parse(((DataRow)dtledg.Rows[i])[j].ToString()).ToString("N3");
        //            }
        //        }
        //        else
        //        {
        //            val = ((DataRow)dtledg.Rows[i])[j].ToString().Replace("linebreak", "\n");
        //        }
        //        cell = new PdfPCell(FormatPhrase(val));
        //        cell.MinimumHeight = 15f;
        //        if (j > 2)
        //        {
        //            cell.HorizontalAlignment = 2;
        //        }
        //        else if (j ==0)
        //        {
        //            cell.HorizontalAlignment = 1;
        //        }
        //        else
        //        {
        //            cell.HorizontalAlignment = 0;
        //        }
        //        cell.BorderColor = BaseColor.LIGHT_GRAY;
        //        pdtledg.AddCell(cell);
        //    }
        //}
        //document.Add(pdtledg);

        //document.Close();
        //Response.Flush();
        //Response.End();
    
    }
}