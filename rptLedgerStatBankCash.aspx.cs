using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;

using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.pdf.draw;
using System.Reflection;
using Delve;
using ClosedXML.Excel;
using System.IO;
using Color = System.Drawing.Color;
using Control = System.Web.UI.Control;

//using cins;

public partial class rptLedgerStatBankCash : System.Web.UI.Page
{
    private static string book = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsPostBack)
            {
                ViewState["CurrentTotalDr"] = 0;
                ViewState["CurrentTotalCr"] = 0;
                book = Session["book"].ToString();
                lblOrg.Text = Session["org"].ToString();
                lblAddress1.Text = Session["add1"].ToString();
                lblAddress2.Text = Session["add2"].ToString();
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
                lblTitle.Text = NotesNo;
                lblDate.Text = " AS ON " + StartDt + " TO " + EndDt;

                DataTable dtList = VouchManager.GetShowBankCashStatement(glCode, "", StartDt, EndDt, Session["UserType"].ToString(),"1");
                ViewState["dtList"] = dtList;
                if (dtList.Rows.Count > 0)
                {
                    dgLedger.DataSource = dtList;
                    dgLedger.DataBind();
                    ShowFooterTotal();
                }
            }
        }
        catch (NullReferenceException neException)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Session Time Out.Please login again.!!');", true);
            return;
        }
        catch (Exception exception)
        {

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Some Problems hear.check Properly and try again.!!');", true);
            return;
        }
    }

    private void ShowFooterTotal()
    {
        if (ViewState["dtList"] != null)
        {
            decimal Dramount = decimal.Zero,totDrCurrent=decimal.Zero;
            decimal CrAmounr = decimal.Zero, totCrCurrent = decimal.Zero;
            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
            TableCell cell;
            DataTable dt = (DataTable)ViewState["dtList"];
            foreach (DataRow row1 in dt.Rows)
            {
                Dramount += Convert.ToDecimal(row1["Debit_amt"]);
                CrAmounr += Convert.ToDecimal(row1["Credit_amt"]);
                if (!row1["Vch_sys_no"].ToString().Trim().Equals("99999999"))
                {
                    totDrCurrent += Convert.ToDecimal(row1["Debit_amt"]);
                    totCrCurrent += Convert.ToDecimal(row1["Credit_amt"]);
                }
            }
            ViewState["CurrentTotalDr"] = totDrCurrent;
            ViewState["CurrentTotalCr"] = totCrCurrent;
            cell = new TableCell();
            cell.Text = "Total";
            cell.ColumnSpan = 6;
            row.Cells.Add(cell);
            row.HorizontalAlign = HorizontalAlign.Right;
            row.Font.Bold = true;
            cell = new TableCell();
            cell.Text = totDrCurrent.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Font.Bold = true;
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = totCrCurrent.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);
            row.Font.Bold = true;
           
            dgLedger.Controls[0].Controls.Add(row);
        }
    }
    

    
    protected void dgLedger_RowDataBound(object sender, GridViewRowEventArgs e)
    {
       
    }

    protected void dgLedger_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        if (ViewState["dtList"] != null)
        {
            DataTable dt = (DataTable)ViewState["dtList"];
            dgLedger.DataSource = dt;
            dgLedger.PageIndex = e.NewPageIndex;
            dgLedger.DataBind();
            ShowFooterTotal();
        }
    }

    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD));
    }

    protected void lbExp_Click(object sender, EventArgs e)
    {
       
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=LedgerStatement-" + lblTitle.Text.Replace(" ","").Replace("  ","") + "-("+DateTime.Now.ToString("dd-MMM-yyyy")+").pdf");
        Document document = new Document();
        document = new Document(PageSize.A4.Rotate(), 10f, 10f, 30f, 30f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        pdfPage page = new pdfPage();
        writer.PageEvent = page;
        document.Open();
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
        //cell.FixedHeight = 80f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
       // cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
       // cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
       // cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(NotesNo + "\nAS ON " + StartDt + " TO "+EndDt, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
       // cell.FixedHeight = 20f;
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

        DataTable dtledg = (DataTable)ViewState["dtList"];
        float[] width = new float[8] {8,15, 15, 20, 35,50, 20, 20};
        PdfPTable pdtledg = new PdfPTable(width);
        pdtledg.WidthPercentage = 100;
        pdtledg.HeaderRows = 1;
        if (dtledg.Columns.Count > 6)
        {
            //dtledg.Columns.Remove("acc_type");
            //dtledg.Columns.Remove("v_date");
            //dtledg.Columns.Remove("coa_desc");
            //dtledg.Columns.Remove("vch_manual_no");
            // dtledg.Columns.Remove("Vch_sys_no");          
        }

        cell = new PdfPCell(FormatHeaderPhrase("SL.")); //Voucher#
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Voucher No.")); //Voucher#
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Date")); //Voucher#
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("M.V.NO")); //Particulars
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Head Name")); //Vch Ref No
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Particulars")); //Vch Ref No
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Amount(Dr)")); //Debit
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Amount(Cr)")); //Credit
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        decimal dtot = 0;
        decimal ctot = 0;
        int SL = 1;
        foreach (DataRow dr in dtledg.Rows)
        {
            cell = new PdfPCell(FormatPhrase(SL.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);
            SL++;

            cell = new PdfPCell(FormatPhrase(dr["VCH_SYS_NO"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["VALUE_DATE"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["SERIAL_NO"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Part"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            //cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["PARTICULARS"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            //cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Debit_amt"]).ToString("N3")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Credit_amt"]).ToString("N3")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);
           

            dtot += decimal.Parse(dr["Debit_amt"].ToString());
            ctot += decimal.Parse(dr["Credit_amt"].ToString());

        }


        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.Colspan = 6;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(Convert.ToDouble(ViewState["CurrentTotalDr"]).ToString("N3")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(Convert.ToDouble(ViewState["CurrentTotalCr"]).ToString("N3")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
       

        document.Add(pdtledg);
        document.Close();
        Response.Flush();
        Response.End();
    }

    protected void lbExpExcel_Click(object sender, EventArgs e)
    {
        DataTable ddt = (DataTable)ViewState["dtList"];
        if (ddt.Rows.Count > 0)
        {

            ddt.Columns.Remove("gl_coa_code");
            ddt.Columns.Remove("COA_NATURAL_CODE");

            ddt.Columns["VCH_SYS_NO"].ColumnName = "Voucher No.";
            ddt.Columns["VALUE_DATE"].ColumnName = "Voucher Date";
                // dtlist.Columns["coa_desc"].ColumnName = "Chart Of Account(COA)";
            ddt.Columns["Part"].ColumnName = "Head Name";
                ddt.Columns["PARTICULARS"].ColumnName = "Particulars";
                ddt.Columns["SERIAL_NO"].ColumnName = "Manual Voucher No.";
                ddt.Columns["Debit_amt"].ColumnName = "Amount(Dr)";
                ddt.Columns["Credit_amt"].ColumnName = "Amount(Cr)";
              
           
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ddt, "ContraStatement");
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition",
                    "attachment;filename=" + lblTitle.Text.Replace(" ", "") + "-ContraStatement-(" +
                    DateTime.Now.ToString("dd/MM/yyyy") + ").xlsx");

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

    protected void dgLedger_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "View")
        {
            GridViewRow gvr = (GridViewRow) (((LinkButton) e.CommandSource).NamingContainer);
            gvr.Cells[2].BackColor = gvr.Cells[3].BackColor =gvr.Cells[0].BackColor = gvr.Cells[1].BackColor= Color.Bisque;
            ModalPopupExtenderLogin.Show();
            //UP1.Update();
            UP2.Update();
            DataTable dt = VouchManager.GetVouchDtl(gvr.Cells[2].Text.ToString().Trim(), "");
            if (dt.Rows.Count > 0)
            {
                dgVoucherDtl.Caption = "Voucher No. : " + gvr.Cells[2].Text.ToString().Trim();
                dgVoucherDtl.DataSource = dt;
                ViewState["vouchdtl"] = dt;
                dgVoucherDtl.DataBind();
                GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
                TableCell cell;
                int j;
                if (dgVoucherDtl.Columns[0].Visible == true)
                {
                    j = dgVoucherDtl.Columns.Count - 3;
                }
                else
                {
                    j = dgVoucherDtl.Columns.Count - 4;
                }

                for (int i = 0; i < j; i++)
                {
                    cell = new TableCell();
                    cell.Text = "";
                    row.Cells.Add(cell);
                }
                cell = new TableCell();
                cell.Text = "Total";
                row.Cells.Add(cell);
                cell = new TableCell();
                decimal priceCr = decimal.Zero;
                decimal priceDr = decimal.Zero;

                cell = new TableCell();
                priceCr = GetTotal("amount_cr");
                cell.Text = priceCr.ToString("N3");
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);
                row.Font.Bold = true;

                cell = new TableCell();
                priceDr = GetTotal("amount_dr");
                cell.Text = priceDr.ToString("N3");
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);
                row.Font.Bold = true;
                dgVoucherDtl.Controls[0].Controls.Add(row);
            }
        }
        else if (e.CommandName == "Select")
        {
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            gvr.Cells[2].BackColor = gvr.Cells[3].BackColor = gvr.Cells[0].BackColor = gvr.Cells[1].BackColor = Color.Bisque;
            VouchMst vchmst = VouchManager.GetVouchMaster(gvr.Cells[1].Text.ToString().Trim(),
                (Session["userlevel"].ToString()).ToString());
            if (vchmst != null)
            {
                if (vchmst.VchRefNo.Contains("DV"))
                {
                    //Response.Redirect("DebitVoucherUI.aspx");
                    Session["DV_ID"] = gvr.Cells[1].Text.ToString();
                    Response.Write("<script>");
                    Response.Write("window.open('DebitVoucherUI.aspx?mno=0.0','_blank')");
                    Response.Write("</script>");

                   // Response.Write("<script>window.open ('DebitVoucherUI.aspx?VoucherID=" + dgLedger.SelectedRow.Cells[2].Text.Trim() + "','_blank');</script>");
                }
                else if (vchmst.VchRefNo.Contains("COV"))
                {
                    Session["COV_ID"] = gvr.Cells[1].Text.ToString();
                   // Response.Redirect("window.open('ContraVoucherUI.aspx?mno=0.3','_blank')");
                    Response.Write("<script>");
                    Response.Write("window.open('ContraVoucherUI.aspx?mno=0.3','_blank')");
                    Response.Write("</script>");
                }
                else if (vchmst.VchRefNo.Contains("CV"))
                {
                    Session["CV_ID"] = gvr.Cells[1].Text.ToString();
                    Response.Write("<script>");
                    Response.Write("window.open('CreditVoucherUI.aspx?mno=0.1','_blank')");
                    Response.Write("</script>");
                }
                else if (vchmst.VchRefNo.Contains("JV"))
                {
                    Session["JV_ID"] = gvr.Cells[1].Text.ToString();
                    Response.Write("<script>");
                    Response.Write("window.open('JournalVoucherUI.aspx?mno=0.2','_blank')");
                    Response.Write("</script>");
                }
            }
        }
    }

    private decimal GetTotal(string ctrl)
    {
        decimal drt = 0;
        DataTable dt = (DataTable) ViewState["vouchdtl"];

        foreach (DataRow rowst in dt.Rows)
        {
            drt += decimal.Parse(string.IsNullOrEmpty(rowst[ctrl].ToString()) ? "0" : rowst[ctrl].ToString());
        }
        return drt;
    }


    protected void dgLedger_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void dgVoucherDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                //e.Row.Cells[0].Attributes.Add("style", "display:none");
            }
        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }
    protected void rbType_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable dtledg = (DataTable)Session["dtledger"];
        dgLedger.DataSource = dtledg;
        dgLedger.DataBind();
        
            ShowFooterTotal();
            
       
    }
    protected void txtSearchCoa_TextChanged(object sender, EventArgs e)
    {
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
        DataTable dtList = null;
        string[] Data = txtSearchCoa.Text.Split('-');
        //string SerchCoa ="1-"+ Data[0].ToString();
        if (string.IsNullOrEmpty(txtSearchCoa.Text))
        {
            dtList = VouchManager.GetShowBankCashStatement(glCode, "", StartDt, EndDt,
                Session["UserType"].ToString(), "1");
        }
        else
        {
            dtList = VouchManager.GetShowBankCashStatement(glCode, "1-" + Data[0], StartDt, EndDt, Session["UserType"].ToString(),
                "2");
        }
        ViewState["dtList"] = dtList;
        if (dtList != null)
        {
            if (dtList.Rows.Count > 0)
            {
                dgLedger.DataSource = dtList;
                dgLedger.DataBind();
                ShowFooterTotal();
            }
        }
    }
}