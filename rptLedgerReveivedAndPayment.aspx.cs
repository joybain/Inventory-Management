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

public partial class rptLedgerReveivedAndPayment : System.Web.UI.Page
{
    private static string book = string.Empty;
    VouchManager _aVouchManager=new VouchManager();
    private string UserType;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            try
            {
                book = Session["book"].ToString();
                lblOrg.Text = Session["org"].ToString();
                lblAddress1.Text = Session["add1"].ToString();
                lblAddress2.Text = Session["add2"].ToString();
                UserType = Session["UserType"].ToString();
            }
            catch (NullReferenceException neException)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                    "alert('Session Time Out.Please login again.!!');", true);
                return;
            }
            catch (Exception exception)
            {

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                    "alert('Some Problems hear.check Properly and try again.!!');", true);
                return;
            }
            string RepType = Request.QueryString["reptype"].ToString();
            string StartDt = Request.QueryString["startdt"].ToString();
            string EndDt = Request.QueryString["enddt"].ToString();
            string glCode = Request.QueryString["glcoa"].ToString();
            //string glCode ="1-0000000";
            string RepLvl = Request.QueryString["replvl"].ToString();
            string SegLvl = Request.QueryString["seglvl"].ToString();
            string VchType = Request.QueryString["vchtyp"].ToString();
            string RptSysId = Request.QueryString["rptsysid"].ToString();
            string NotesNo = Request.QueryString["notes"].ToString();

            RepType = "8";

            DataTable dtRp;
            DataTable dtVch = _aVouchManager.GetShowPaymentAndReceived(glCode, StartDt, EndDt,
                Session["UserType"].ToString());
            string connectionString = DataManager.OraConnString();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            lblDate.Text = "";
            lblTitle.Text = NotesNo;
            if (EndDt == "" | DataManager.DateEncode(EndDt).CompareTo(System.DateTime.Now) > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    "alert('Please select the Upto Date or Upto Date cannot be greater than System Date!!');", true);
            }
            else
            {
                lblDate.Text = " For the Period " + StartDt + " TO " + EndDt;
                //lblTitle.Text = "Receipts & Payments";
                if (RepType == "8")
                    if (EndDt == "" | DataManager.DateEncode(EndDt).CompareTo(System.DateTime.Now) > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('Please select the Upto Date or Upto Date cannot be greater than System Date!!');",
                            true);
                    }
                    else if (DataManager.DateEncode(StartDt).CompareTo(System.DateTime.Now) > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('Start Date cannot be greater than System Date!!');", true);
                    }
                    else if (DataManager.DateEncode(StartDt).CompareTo(DataManager.DateEncode(EndDt)) > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('Start Date cannot be greater than System Date!!');", true);
                    }
                    else
                    {
                        
                            string segcoacode = "";
                            segcoacode = "0-" + glCode;
                            DataTable dtLadger =
                                GlCoaManagerRP.getCoaBalanceFromExistingDataTable_WithSingleLedger(dtVch, book,
                                    segcoacode,
                                    Session["septype"].ToString(), "A", StartDt, EndDt, "", 1);
                        dgLedger.DataSource = dtLadger;
                        ViewState["dtLadger"] = dtLadger;
                        dgLedger.DataBind();

                    }
            }
        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        return;
    }
    protected void dgLedger_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //e.Row.Cells[1].Attributes.Add("style", "display:none");
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
            //e.Row.Cells[2].Text = ((DataRowView) e.Row.DataItem)["vch_sys_no"].ToString();
            //e.Row.Cells[3].Text = e.Row.Cells[2].Text.Replace("linebreak", "<br />");
            //e.Row.Cells[4].Text = e.Row.Cells[1].Text.Replace("linebreak", "<br />");
            //LinkButton lb = e.Row.FindControl("lblSelect") as LinkButton;
            //AjaxControlToolkit.ToolkitScriptManager.GetCurrent(this).RegisterAsyncPostBackControl(lb);
       // }
        //LinkButton lnkBtn6 = (LinkButton)e.Row.FindControl("lblSelect");
        //lnkBtn6.Text = "Some Text Here";
        //if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header |
        //    e.Row.RowType == DataControlRowType.Footer)
        //{
        //    if (rbType.SelectedValue.Equals("D"))
        //    {
        //        e.Row.Cells[8].Attributes.Add("style", "display:none");
        //        e.Row.Cells[9].Attributes.Add("style", "display:none");
        //    }
        //    else if (rbType.SelectedValue.Equals("C"))
        //    {
        //        e.Row.Cells[7].Attributes.Add("style", "display:none");
        //        e.Row.Cells[9].Attributes.Add("style", "display:none");
        //    }
        //}
    }

    protected void dgLedger_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        if (ViewState["dtLadger"] != null)
        {
            DataTable dt = (DataTable)ViewState["dtLadger"];
            dgLedger.DataSource = dt;
            dgLedger.PageIndex = e.NewPageIndex;
            dgLedger.DataBind();
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
        Response.AddHeader("content-disposition",
            "attachment; filename=Received&Payment_DetailsList-(" + DateTime.Now.ToString("dd-MM-yyyy") + ").pdf");
        Document document = new Document();
        document = new Document(PageSize.A4, 30f, 30f, 30f, 30f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        pdfPage page = new pdfPage();
        writer.PageEvent = page;
        document.Open();
        PdfPCell cell;
        //title = "Account balance from " + StartDt + " to " + EndDt;
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
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(lblTitle.Text + " \n " + lblDate.Text,
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
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
        //string typeseg = Session["typeseg"].ToString();
        PdfPTable pdtaccbal;
        //int acolno = 0;

        float[] width = new float[6] { 8,10,10, 10, 40, 15};
        pdtaccbal = new PdfPTable(width);
        pdtaccbal.WidthPercentage = 100;
        pdtaccbal.HeaderRows = 1;
        cell = new PdfPCell(FormatHeaderPhrase("SL."));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtaccbal.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Voucher No"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtaccbal.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Voucher Date"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtaccbal.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("COA Code"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtaccbal.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("COA Description"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtaccbal.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtaccbal.AddCell(cell);
        int sl = 1;
        DataTable dtaccbal =(DataTable) ViewState["dtLadger"];
        foreach (DataRow drRP in dtaccbal.Rows)
        {
            try
            {
                cell = new PdfPCell(FormatPhrase(sl.ToString()));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.MinimumHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                sl++;
                cell = new PdfPCell(FormatPhrase(drRP["VCH_SYS_NO"].ToString()));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.MinimumHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(drRP["VALUE_DATE"].ToString()));
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(drRP["COA_NATURAL_CODE"].ToString()));
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(drRP["SEG_COA_DESC"].ToString()));
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(drRP["amount_dr"].ToString()));
                cell.VerticalAlignment = 0;
                cell.HorizontalAlignment = 2;
                cell.MinimumHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Run Again.then click Export Report to PDF Button... !!');", true);
                return;
            }
        }
        document.Add(pdtaccbal);
        document.Close();
        Response.Flush();
        Response.End();
    }

    protected void lbExpExcel_Click(object sender, EventArgs e)
    {
        DataTable ddt = (DataTable)ViewState["dtLadger"];
        if (ddt.Rows.Count > 0)
        {
            if (!ddt.Columns[0].ColumnName.Equals("Voucher No."))
            {
                //ddt.Columns.Remove("v_date");
                ddt.Columns.Remove("Flag");
                ddt.Columns.Remove("gl_coa_code");
                ddt.Columns["Vch_sys_no"].ColumnName = "Voucher No.";
                ddt.Columns["value_date"].ColumnName = "Voucher Date";
                ddt.Columns["SEG_COA_DESC"].ColumnName = "COA Description";
               // ddt.Columns["Part"].ColumnName = "Head Name";
               // ddt.Columns["VCH_REF_NO"].ColumnName = "Voucher Ref No.";
                //ddt.Columns["particulars"].ColumnName = "Particulars";
                ddt.Columns["Coa_Natural_Code"].ColumnName = "COA Cod";
                ddt.Columns["Amount_dr"].ColumnName = "Amount";
                //ddt.Columns["Credit_amt"].ColumnName = "Amount(Cr)";
               // ddt.Columns["bal"].ColumnName = "Balance";
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ddt, "LedgerStatement");
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition",
                    "attachment;filename=" + lblTitle.Text.Replace(" ", "") + "-Received&Payment_DetailsList-(" +
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
}