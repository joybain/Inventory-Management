using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using iTextSharp.text;
using System.Data.SqlClient;
using Delve;

public partial class rptScheduleOfCashatBank : System.Web.UI.Page
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
                if (RepType == "S")
                {
                    lblTitle.Text = NotesNo.ToUpper();
                    lblDate.Text = " As on " + EndDt;
                    string criteria = GlCoaManager.getCoaCriteria(glCode, Session["septype"].ToString(), book);
                    DataTable dtAlready = GlCoaManager.GetGlCoaCodes(criteria);
                    string glcoacode = "";
                   // string glcoacode1 = "";
                    //decimal OpeningBalance = decimal.Zero;
                    DataTable dt=new DataTable();
                    dt.Columns.Add("gl_coa_code");
                    dt.Columns.Add("coa_desc");
                    dt.Columns.Add("Openin");
                    dt.Columns.Add("Period_amount");
                    dt.Columns.Add("Closing");                   
                    int i=0;
                    foreach (DataRow drA in dtAlready.Rows)
                    {
                        glcoacode += "'" + drA["gl_coa_code"].ToString() + "'" ;
                        //OpeningBalance += GlCoaManager.GetShowOpeningBalance(drA["gl_coa_code"].ToString());


                       // glcoacode = drA["gl_coa_code"].ToString();
                            //glcoacode = glcoacode.Remove(glcoacode.Length - 1, 1);//startIndex Parameter can't be lessthen zero
                            //string query = "select a.vch_sys_no AS Vch_sys_no, 'A' acc_type,a.value_date v_date, convert(varchar,a.value_date,103) AS value_date, " +
                            //" b.gl_coa_code+' : '+(select coa_desc from gl_coa where gl_coa_code=b.gl_coa_code) as coa_desc, " +
                            //" case when SIGN(b.amount_cr)=1 then 'TO   ' when SIGN(b.amount_cr)=-1 then 'BY   ' else  " +
                            //" (case when SIGN(b.amount_dr)=1 then 'BY   ' when SIGN(b.amount_dr)=-1 then 'TO   ' end) end +b.particulars+'  (REF dt. '+convert(varchar,b.vch_ref_date,103) " +
                            //" +')'+' linebreak '+a.particulars AS particulars, " +
                            //" (select dbo.initcap(vch_desc) from gl_voucher_type where vch_code=a.vch_code)+' linebreak '+a.vch_ref_no+' linebreak '+ " +
                            //" 'Ref# '+ref_file_no+', Vol# '+volume_no+', Sl#'+serial_no AS vch_manual_no, " +
                            //" coalesce(b.amount_dr,0) AS Debit_amt,coalesce(b.amount_cr,0) AS Credit_amt,0 bal " +
                            //" from gl_trans_mst a ,gl_trans_dtl b where a.vch_sys_no=b.vch_sys_no and a.status='A' and a.book_name='" + book + "' " +
                            //" and b.gl_coa_code in (" + glcoacode + ") and a.value_date between convert(datetime,nullif('" + StartDt + "',''),103) and convert(datetime,nullif('" + EndDt + "',''),103) order by v_date,a.vch_sys_no ";

                        string query = @"select isnull(SUM(Coa_Code1.Debit_amt) - SUM(Coa_Code1.Credit_amt),0) as Period_amount from (select coalesce(b.amount_dr,0) AS Debit_amt,coalesce(b.amount_cr,0) AS Credit_amt  from gl_trans_mst a ,gl_trans_dtl b where a.vch_sys_no=b.vch_sys_no and a.book_name='" + book + "'  and b.gl_coa_code in (" + glcoacode + ") and a.value_date between convert(datetime,nullif('" + StartDt + "',''),103) and convert(datetime,nullif('" + EndDt + "',''),103) and a.[STATUS]='A' ) as Coa_Code1  ";

                            Decimal pa;
                            conn.Open();
                           
                            SqlCommand command = new SqlCommand(query, conn);
                            pa = Convert.ToDecimal(command.ExecuteScalar());
                            conn.Close();
                            SqlConnection.ClearPool(conn);
                           
                           DataRow dr = dt.NewRow();
                           dr["gl_coa_code"] = drA["gl_coa_code"].ToString();
                           dr["coa_desc"] = drA["coa_desc"].ToString();

                           decimal SumOpBal = GlCoaManager.getCoaopeningBalanceFromExistingDataTable(dtVch, book, drA["gl_coa_code"].ToString(), Session["septype"].ToString(), "A", StartDt, StartDt);
                           
                        dr["Openin"] =Convert.ToDecimal(SumOpBal);
                        dr["Closing"] = Convert.ToDecimal(SumOpBal + pa);
                        dr["Period_amount"] =Convert.ToDecimal(pa);
                        dt.Rows.Add(dr);
                           

                        }

                    dgLedger.Visible = true;
                    dgLedger.DataSource = dt;
                    Session["dtledger"] = dt;
                    dgLedger.DataBind();
                    ShowFooterTotal();
                }              
            }
        }
    }
    private void ShowFooterTotal()
    {
        if (Session["dtledger"] != null)
        {
            decimal Openning = decimal.Zero;
            decimal Period = decimal.Zero;
            decimal Closing = decimal.Zero;
            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
            TableCell cell;
            DataTable dt = (DataTable)Session["dtledger"];
            foreach (DataRow row1 in dt.Rows)
            {
                Openning += Convert.ToDecimal(row1["Openin"].ToString());
                Period += Convert.ToDecimal(row1["Period_amount"].ToString());
                Closing += Convert.ToDecimal(row1["Closing"].ToString());
            }
            
            cell = new TableCell();
            cell.Text = "Total";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = Openning.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = Period.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = Closing.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);
            row.Font.Bold = true;
            cell = new TableCell();
            cell = new TableCell();
            cell.Text = "";
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
        if (Session["dtledger"] != null)
        {
            DataTable dt = (DataTable)Session["dtledger"];
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
        float[] width = new float[4] {  60,  30, 30, 30 };
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
        cell = new PdfPCell(FormatHeaderPhrase("Previous Balance"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("This Period"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Total"));
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

            if (Convert.ToDecimal(dr["Openin"]) > 0)
            {
                cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["Openin"]).ToString("N3")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtledg.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(FormatPhrase("("+Convert.ToDecimal(dr["Openin"]).ToString("N3")+")"));
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
                cell = new PdfPCell(FormatPhrase("("+Convert.ToDecimal(dr["Period_amount"]).ToString("N3")+")"));
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
                cell = new PdfPCell(FormatPhrase("("+Convert.ToDecimal(dr["Closing"]).ToString("N3") + ") Cr"));
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