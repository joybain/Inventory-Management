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
using Delve;
using System.Data.SqlClient;

public partial class TrialBlanceUI : System.Web.UI.Page
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

            DataTable dtVch = VouchManager.GetVouchDtlForTotal(EndDt, "A", book, Session["UserType"].ToString());
            string connectionString = DataManager.OraConnString();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            lblTranStatus.Text = "";

            if (EndDt == "" | DataManager.DateEncode(EndDt).CompareTo(System.DateTime.Now) > 0)
            {
                lblTranStatus.Text = "Please select the Upto Date or Upto Date cannot be greater than System Date.";
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                if (RepType == "T")
                {
                    lblTitle.Text = RepType.Replace("3", "Cash book statement").Replace("4", "Bank book statement") + " as on " + EndDt;
                    string criteria = GlCoaManager.getCoaCriteria(glCode, Session["septype"].ToString(), book);
                    DataTable dtAlready = GlCoaManager.GetGlCoaCodes(criteria);
                    string glcoacode = "";
                    
                    DataTable dt = new DataTable();
                    dt.Columns.Add("gl_coa_code");
                    dt.Columns.Add("coa_desc");
                    dt.Columns.Add("Openin_Cr");
                    dt.Columns.Add("Openin_Dr");
                    dt.Columns.Add("Period_amount_Cr");
                    dt.Columns.Add("Period_amount_Dr");
                    dt.Columns.Add("Closing_Cr");
                    dt.Columns.Add("Closing_Dr");

                    int i = 0;
                    foreach (DataRow drA in dtAlready.Rows)
                    {
                        glcoacode += "'" + drA["gl_coa_code"].ToString() + "'";

                        string query = @"select isnull(SUM(Coa_Code1.Debit_amt),0) as Debit_amt , isnull(SUM(Coa_Code1.Credit_amt),0) as Credit_amt from (select coalesce(b.amount_dr,0) AS Debit_amt,coalesce(b.amount_cr,0) AS Credit_amt  from gl_trans_mst a ,gl_trans_dtl b where a.vch_sys_no=b.vch_sys_no and a.status='A' and a.book_name='" + book + "'  and b.gl_coa_code in (" + glcoacode + ") and a.value_date < convert(datetime,nullif('" + StartDt + "',''),103) union all SELECT case when t1.opening_balance  >= 0 then t1.opening_balance else 0 end , case when t1.opening_balance  <= 0 then t1.opening_balance else 0 end FROM GL_COA T1 where t1.opening_balance !=0 and t1.status='A' and t1.book_name='" + book + "' and t1.GL_COA_CODE in (" + glcoacode + ")) as Coa_Code1 ";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);

                        DataSet Ds = new DataSet();
                        adapter.Fill(Ds, "dt");

                        DataRow dr = dt.NewRow();
                        dr["gl_coa_code"] = drA["gl_coa_code"].ToString();
                        dr["coa_desc"] = drA["coa_desc"].ToString();
                        foreach (DataRow drdtl in Ds.Tables["dt"].Rows)
                        {
                            dr["Openin_Dr"] = drdtl["Debit_amt"];
                            dr["Openin_Cr"] = drdtl["Credit_amt"];
                        }

                        string query1 = @"select isnull(SUM(Coa_Code1.Debit_amt),0) as Debit_amt , isnull(SUM(Coa_Code1.Credit_amt),0) as Credit_amt from (select coalesce(b.amount_dr,0) AS Debit_amt,coalesce(b.amount_cr,0) AS Credit_amt  from gl_trans_mst a ,gl_trans_dtl b where a.vch_sys_no=b.vch_sys_no and a.status='A' and a.book_name='" + book + "'  and b.gl_coa_code in (" + glcoacode + ") and a.value_date between convert(datetime,nullif('" + StartDt + "',''),103) and convert(datetime,nullif('" + EndDt + "',''),103) ) as Coa_Code1  ";

                        SqlDataAdapter adapter1 = new SqlDataAdapter(query1, conn);

                        DataSet Ds1= new DataSet();
                        adapter1.Fill(Ds1 ,"dt1");

                        
                       
                        foreach (DataRow drdtl in Ds1.Tables["dt1"].Rows)
                        {
                            dr["Period_amount_Dr"] = drdtl["Debit_amt"];
                            dr["Period_amount_Cr"] = drdtl["Credit_amt"];
                        }
                        dr["Closing_Dr"] = Convert.ToDecimal( dr["Openin_Dr"]) + Convert.ToDecimal( dr["Period_amount_Dr"]);
                        dr["Closing_Cr"] = Convert.ToDecimal(dr["Openin_Cr"]) + Convert.ToDecimal(dr["Period_amount_Cr"]);
                        
                        dt.Rows.Add(dr);

                    }

                    dgLedger.Visible = true;
                    dgLedger.DataSource = dt;
                    Session["dtledger1"] = dt;

                    dgLedger.DataBind();
                    ShowFooterTotal();
                }
            }
        }

    }

    private void ShowFooterTotal()
    {
        if (Session["dtledger1"] != null)
        {
            decimal OpenningCr = decimal.Zero;
            decimal OpenningDr = decimal.Zero;
            decimal PreoidAmountCr = decimal.Zero;
            decimal PreoidAmountDr = decimal.Zero;            
            decimal ClosingCr = decimal.Zero;
            decimal ClosingDr = decimal.Zero;
            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
            TableCell cell;
            DataTable dt = (DataTable)Session["dtledger1"];
            foreach (DataRow row1 in dt.Rows)
            {
                OpenningCr += Convert.ToDecimal(row1["Openin_Cr"].ToString());
                OpenningDr += Convert.ToDecimal(row1["Openin_Dr"].ToString());
                PreoidAmountCr += Convert.ToDecimal(row1["Period_amount_Cr"].ToString());
                PreoidAmountDr += Convert.ToDecimal(row1["Period_amount_Dr"].ToString());
                ClosingCr += Convert.ToDecimal(row1["Closing_Cr"].ToString());
                ClosingDr += Convert.ToDecimal(row1["Closing_Dr"].ToString());
            }

            cell = new TableCell();
            cell.Text = "Total";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = OpenningCr.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = OpenningDr.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = PreoidAmountCr.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = PreoidAmountDr.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = ClosingCr.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = ClosingDr.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);
            row.Font.Bold = true;            
            if (dgLedger.Rows.Count > 0)
            {
                dgLedger.Controls[0].Controls.Add(row);
            }

        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        return;
    }
    private void ExportToExcel(string strFileName, GridView dg)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.ContentType = "application/vnd.ms-excel";
        Response.Charset = "";
        this.EnableViewState = false;
        System.IO.StringWriter oStringWriter = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter oHtmlTextWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);
        dg.RenderControl(oHtmlTextWriter);
        Response.Write(oStringWriter.ToString());
        Response.End();
    }

    protected void dgLedger_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Text = ((DataRowView)e.Row.DataItem)["vch_sys_no"].ToString();
            e.Row.Cells[2].Text = e.Row.Cells[2].Text.Replace("linebreak", "<br />");
            e.Row.Cells[1].Text = e.Row.Cells[1].Text.Replace("linebreak", "<br />");
        }
    }

    protected void dgLedger_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        if (Session["dtledger1"] != null)
        {
            DataTable dt = (DataTable)Session["dtledger1"];
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
        Response.AddHeader("content-disposition", "attachment; filename=MypdfFile.pdf");
        Document document = new Document();
        document = new Document(PageSize.A4.Rotate(), 40f, 30f, 30f, 30f);
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
        string title = "";
        if (RepType == "3" | RepType == "4")
        {
            title = RepType.Replace("3", "Cash book statement").Replace("4", "Bank book statement") + " as on " + EndDt;

        }
        else if (RepType == "5")
        {
            string coaDesc = GlCoaManager.getCoaDesc(glCode);
            title = "Ledger statement of " + coaDesc + " from " + StartDt + " to " + EndDt + "";
        }
        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(10f);

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

        DataTable dtledg = (DataTable)Session["dtledger1"];
        float[] width = new float[4] { 60, 30, 30, 30 };
        PdfPTable pdtledg = new PdfPTable(width);
        pdtledg.WidthPercentage = 100;

        //if (dtledg.Columns.Count > 6)
        //{
        //    dtledg.Columns.Remove("v_date");
        //    dtledg.Columns.Remove("value_date");
        //    dtledg.Columns.Remove("acc_type");
        //    dtledg.Columns.Remove("coa_desc");
        //}
        cell = new PdfPCell(FormatHeaderPhrase("Accounts"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Opening Balance Cr"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Opening Blance Dr"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Period Amount Cr"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Period Amount Dr"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Closing Cr"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Closing Dr"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        decimal dtot = 0;
        decimal ctot = 0;
        decimal tot = 0;

        foreach (DataRow dr in dtledg.Rows)
        {
            cell = new PdfPCell(FormatPhrase(dr["coa_desc"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

            if (Convert.ToDecimal(dr["Openin_Cr"]) > 0)
            {
                cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["Openin_Cr"]).ToString("N3")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtledg.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(FormatPhrase("(" + Convert.ToDecimal(dr["Openin_Cr"]).ToString("N3") + ")"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtledg.AddCell(cell);
            }
            if (Convert.ToDecimal(dr["Period_amount"]) > 0)
            {
                cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["Period_amount"]).ToString("N3")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtledg.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(FormatPhrase("(" + Convert.ToDecimal(dr["Period_amount"]).ToString("N3") + ")"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtledg.AddCell(cell);
            }
            if (Convert.ToDecimal(dr["Closing"]) > 0)
            {

                cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["Closing"]).ToString("N3") + " Dr"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtledg.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(FormatPhrase("(" + Convert.ToDecimal(dr["Closing"]).ToString("N3") + ") Cr"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtledg.AddCell(cell);
            }

            dtot += decimal.Parse(dr["Openin"].ToString());
            ctot += decimal.Parse(dr["Period_amount"].ToString());
            tot += decimal.Parse(dr["Closing"].ToString());

        }


        cell = new PdfPCell(FormatPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        if (Convert.ToDecimal(tot) > 0)
        {
            cell = new PdfPCell(FormatHeaderPhrase(dtot.ToString("N3") + " Dr."));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);
        }
        else
        {
            cell = new PdfPCell(FormatHeaderPhrase("(" + dtot.ToString("N3") + ") Cr."));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);
        }
        if (Convert.ToDecimal(tot) > 0)
        {

            cell = new PdfPCell(FormatHeaderPhrase(ctot.ToString("N3") + " Dr."));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

        }
        else
        {
            cell = new PdfPCell(FormatHeaderPhrase("(" + ctot.ToString("N3") + ") Cr."));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);
        }

        if (Convert.ToDecimal(tot) > 0)
        {

            cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N3") + " Dr."));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

        }
        else
        {
            cell = new PdfPCell(FormatHeaderPhrase("(" + tot.ToString("N3") + ") Cr."));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);
        }


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
        //        else if (j == 0)
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
        document.Add(pdtledg);

        document.Close();
        Response.Flush();
        Response.End();
    }
}