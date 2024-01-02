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
//using Delve;
using System.Data.SqlClient;
using Delve;
using ClosedXML.Excel;
using System.IO;
using System.Drawing;

public partial class NewTrialBlance : System.Web.UI.Page
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
            lblStartDate.Text = StartDt;
            lblEndDate.Text = EndDt;
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
                   // lblTitle.Text = RepType.Replace("3", "Cash book statement").Replace("4", "Bank book statement") + " as on " + EndDt;
                    lblTitle.Text = NotesNo;
                    lblDate.Text = " As on date (" + StartDt + " To " + EndDt + ")";

                    string criteria = GlCoaManager.getCoaCriteria(glCode, Session["septype"].ToString(), book);
                    DataTable dtAlready = GlCoaManager.GetGlCoaCodes(criteria);
                    string glcoacode = "";
                    
                    DataTable dt = new DataTable();
                    dt.Columns.Add("gl_coa_code");
                    dt.Columns.Add("coa_desc");
                    dt.Columns.Add("Openin", typeof(System.Double));
                    //dt.Columns.Add("Openin_Dr");
                    dt.Columns.Add("Period_amount_Cr", typeof(System.Double));
                    dt.Columns.Add("Period_amount_Dr", typeof(System.Double));
                    dt.Columns.Add("Closing", typeof(System.Double));
                    //dt.Columns.Add("Closing_Dr");

                    int i = 0;
                    foreach (DataRow drA in dtAlready.Rows)
                    {
                        glcoacode = "'" + drA["gl_coa_code"].ToString() + "'";

                        //string query = @"select isnull(SUM(Coa_Code1.Debit_amt),0)  - isnull(SUM(Coa_Code1.Credit_amt),0) as Opening from (select coalesce(b.amount_dr,0) AS Debit_amt,coalesce(b.amount_cr,0) AS Credit_amt  from gl_trans_mst a ,gl_trans_dtl b where a.vch_sys_no=b.vch_sys_no  and a.book_name='" + book + "'  and b.gl_coa_code in (" + glcoacode + ") and a.value_date < convert(datetime,nullif('" + StartDt + "',''),103) and a.[STATUS]='A' union all SELECT case when t1.opening_balance  >= 0 then t1.opening_balance else 0 end , case when t1.opening_balance  <= 0 then t1.opening_balance else 0 end FROM GL_COA T1 where t1.opening_balance !=0 and t1.book_name='" + book + "' and t1.GL_COA_CODE in (" + glcoacode + ")) as Coa_Code1 ";
                        
                        /*************** Purpose for User wise Currency show *****************/
                        string query = @"select isnull(SUM(Coa_Code1.Debit_amt),0)  - isnull(SUM(Coa_Code1.Credit_amt),0) as Opening from (select case when '" + Session["UserType"].ToString() + "'=1 then b.Amount_DR_BD when '" + Session["UserType"].ToString() + "'=2 then b.Amount_DR_PH else isnull(b.amount_dr,0) end as Debit_amt,case when '" + Session["UserType"].ToString() + "'=1 then b.Amount_CR_BD when '" + Session["UserType"].ToString() + "'=2 then b.Amount_CR_PH else isnull(b.amount_cr,0) end AS Credit_amt  from gl_trans_mst a ,gl_trans_dtl b where a.vch_sys_no=b.vch_sys_no  and a.book_name='" + book + "'  and b.gl_coa_code in (" + glcoacode + ") and a.value_date < convert(datetime,nullif('" + StartDt + "',''),103) and a.[STATUS]='A' union all SELECT case when t1.opening_balance  >= 0 then t1.opening_balance else 0 end , case when t1.opening_balance  <= 0 then t1.opening_balance else 0 end FROM GL_COA T1 where t1.opening_balance !=0 and t1.book_name='" + book + "' and t1.GL_COA_CODE in (" + glcoacode + ")) as Coa_Code1 ";

                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        DataSet Ds = new DataSet();
                        adapter.Fill(Ds, "dt");

                        DataRow dr = dt.NewRow();
                        dr["gl_coa_code"] = drA["gl_coa_code"].ToString();
                        dr["coa_desc"] = drA["coa_desc"].ToString();
                        foreach (DataRow drdtl in Ds.Tables["dt"].Rows)
                        {
                            dr["Openin"] = drdtl["Opening"];
                            //dr["Openin_Cr"] = drdtl["Credit_amt"];
                        }

                        //string query1 = @"select isnull(SUM(Coa_Code1.Debit_amt),0) as Debit_amt , isnull(SUM(Coa_Code1.Credit_amt),0) as Credit_amt from (select coalesce(b.amount_dr,0) AS Debit_amt,coalesce(b.amount_cr,0) AS Credit_amt  from gl_trans_mst a ,gl_trans_dtl b where a.vch_sys_no=b.vch_sys_no and a.book_name='" + book + "'  and b.gl_coa_code in (" + glcoacode + ") and a.value_date between convert(datetime,nullif('" + StartDt + "',''),103) and convert(datetime,nullif('" + EndDt + "',''),103) and a.[STATUS]='A' ) as Coa_Code1  ";

                        /*************** Purpose for User wise Currency show *****************/
                        string query1 = @"select isnull(SUM(Coa_Code1.Debit_amt),0) as Debit_amt , isnull(SUM(Coa_Code1.Credit_amt),0) as Credit_amt from (select case when '" + Session["UserType"].ToString() + "'=1 then b.Amount_DR_BD when '" + Session["UserType"].ToString() + "'=2 then b.Amount_DR_PH else isnull(b.amount_dr,0) end as Debit_amt,case when '" + Session["UserType"].ToString() + "'=1 then b.Amount_CR_BD when '" + Session["UserType"].ToString() + "'=2 then b.Amount_CR_PH else isnull(b.amount_cr,0) end AS Credit_amt  from gl_trans_mst a ,gl_trans_dtl b where a.vch_sys_no=b.vch_sys_no and a.book_name='" + book + "'  and b.gl_coa_code in (" + glcoacode + ") and a.value_date between convert(datetime,nullif('" + StartDt + "',''),103) and convert(datetime,nullif('" + EndDt + "',''),103) and a.[STATUS]='A' ) as Coa_Code1  ";

                        SqlDataAdapter adapter1 = new SqlDataAdapter(query1, conn);

                        DataSet Ds1= new DataSet();
                        adapter1.Fill(Ds1 ,"dt1");

                        
                       
                        foreach (DataRow drdtl in Ds1.Tables["dt1"].Rows)
                        {
                            dr["Period_amount_Dr"] = drdtl["Debit_amt"];
                            dr["Period_amount_Cr"] = drdtl["Credit_amt"];
                        }
                        dr["Closing"] = Convert.ToDecimal(dr["Openin"]) + Convert.ToDecimal(dr["Period_amount_Dr"]) - Convert.ToDecimal(dr["Period_amount_Cr"]); 
                       // dr["Closing_Cr"] = Convert.ToDecimal(dr["Openin_Cr"]) + 
                        if (Convert.ToDecimal(dr["Openin"]) == 0 && Convert.ToDecimal(dr["Period_amount_Dr"]) == 0 && Convert.ToDecimal(dr["Period_amount_Cr"]) == 0 && Convert.ToDecimal(dr["Closing"]) == 0)
                        {
                           
                        }
                        else 
                        {
                            dt.Rows.Add(dr);
                        }
                    }
                    dgLedger.Visible = true;
                    dgLedger.DataSource = dt;
                    Session["dtledger1"] = dt;
                    dt.DefaultView.Sort = "coa_desc";
                    // DataTable dt = dtledg.DefaultView.ToTable(); 
                    dt = dt.DefaultView.ToTable();
                    ViewState["Excel"] = dt;
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
            decimal Openning = decimal.Zero;
            //decimal OpenningDr = decimal.Zero;
            decimal PreoidAmountCr = decimal.Zero;
            decimal PreoidAmountDr = decimal.Zero;            
            decimal Closing = decimal.Zero;
            //decimal ClosingDr = decimal.Zero;
            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
            TableCell cell;
            DataTable dt = (DataTable)Session["dtledger1"];
            foreach (DataRow row1 in dt.Rows)
            {
                Openning += Convert.ToDecimal(row1["Openin"].ToString());
                //OpenningDr += Convert.ToDecimal(row1["Openin_Dr"].ToString());
                PreoidAmountCr += Convert.ToDecimal(row1["Period_amount_Cr"].ToString());
                PreoidAmountDr += Convert.ToDecimal(row1["Period_amount_Dr"].ToString());
                Closing += Convert.ToDecimal(row1["Closing"].ToString());
                //ClosingDr += Convert.ToDecimal(row1["Closing_Dr"].ToString());
            }

            cell = new TableCell();
            cell.Text = "Total";
            cell.ColumnSpan = 3;
            cell.HorizontalAlign =HorizontalAlign.Right;
            row.Cells.Add(cell);
            //cell = new TableCell();
            //cell.Text = OpenningCr.ToString("N3");
            //cell.HorizontalAlign = HorizontalAlign.Right;
            //row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = Openning.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);
            
            cell = new TableCell();
            cell.Text = PreoidAmountDr.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Text = PreoidAmountCr.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);

            cell = new TableCell();
            //cell.Text = ClosingCr.ToString("N3");
            //cell.HorizontalAlign = HorizontalAlign.Right;
            //row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = Closing.ToString("N3");
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

    protected void dgLedger_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    e.Row.Cells[0].Text = ((DataRowView)e.Row.DataItem)["vch_sys_no"].ToString();
        //    e.Row.Cells[2].Text = e.Row.Cells[2].Text.Replace("linebreak", "<br />");
        //    e.Row.Cells[1].Text = e.Row.Cells[1].Text.Replace("linebreak", "<br />");
            
        //}
      //  e.Row.Cells[5].Attributes.Add("style", "display:none");
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
        Response.AddHeader("content-disposition", "attachment; filename=RootLeafSummery("+DateTime.Now.ToString("dd/MM/yyyy")+").pdf");
        Document document = new Document();
        document = new Document(PageSize.A4.Rotate(), 40f, 30f, 30f, 30f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        pdfPage page = new pdfPage();
        writer.PageEvent = page;
        document.Open();       
      
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
        cell = new PdfPCell(new Phrase(("Root/Leaf "+lblTitle.Text+" Summery. "+lblDate.Text).ToUpper(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
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
       // dtledg
        dtledg.DefaultView.Sort = "coa_desc";
       // DataTable dt = dtledg.DefaultView.ToTable(); 
        dtledg = dtledg.DefaultView.ToTable(); 
        
        float[] width = new float[5] { 60, 30, 30, 30,30 };
        PdfPTable pdtledg = new PdfPTable(width);
        pdtledg.WidthPercentage = 100;
        pdtledg.HeaderRows = 1;
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
        cell = new PdfPCell(FormatHeaderPhrase("Opening Balance"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        //cell = new PdfPCell(FormatHeaderPhrase("Opening Blance Dr"));
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Period Amount(Dr)"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Period Amount(Cr)"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

       
        cell = new PdfPCell(FormatHeaderPhrase("Closing Amount"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("Closing Dr"));
        //cell.MinimumHeight = 15f;
        //cell.HorizontalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtledg.AddCell(cell);

        decimal dtot = 0;
        decimal ctot = 0;
        decimal tot = 0;
        decimal PCtot = 0;
        int Index1;
        int Index;
        foreach (DataRow dr in dtledg.Rows)
        {
            //Index1 = row["GL_COA_CODE"].ToString().IndexOf("-") + 1;
            //int l = row["GL_COA_CODE"].ToString().Length;
            //int t = (l - (Index1));

            //dgSeg.Rows[1].Cells[1].Text = row["GL_COA_CODE"].ToString().Substring(Index1, t);
            int totalLenth = dr["coa_desc"].ToString().Length;
            Index = dr["coa_desc"].ToString().IndexOf(",") + 1;
            int totIndex = (totalLenth - (Index));
           // dgSeg.Rows[1].Cells[2].Text = row["COA_DESC"].ToString().Substring(Index, tot);

            cell = new PdfPCell(FormatPhrase(dr["coa_desc"].ToString().Substring(Index, totIndex)));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

            cell = new PdfPCell(FormatPhrase( Convert.ToDecimal(dr["Openin"]).ToString("N3")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);            


            cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["Period_amount_Dr"]).ToString("N3")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["Period_amount_Cr"]).ToString("N3")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);


            if (Convert.ToDecimal(dr["Closing"]) < 0)
            {
                cell = new PdfPCell(FormatPhrase("("+(Convert.ToDecimal(dr["Closing"])*-1).ToString("N3")+")"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtledg.AddCell(cell);
            }
            else
            {
                cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["Closing"]).ToString("N3")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtledg.AddCell(cell);

            }
            dtot += decimal.Parse(dr["Openin"].ToString());
            ctot += decimal.Parse(dr["Period_amount_Cr"].ToString());
            PCtot += decimal.Parse(dr["Period_amount_Dr"].ToString());
            tot += decimal.Parse(dr["Closing"].ToString());

        }


        cell = new PdfPCell(FormatPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase(dtot.ToString("N3")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(PCtot.ToString("N3")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(ctot.ToString("N3")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);        
        
       
        cell = new PdfPCell(FormatHeaderPhrase( tot.ToString("N3")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);     


      
        document.Add(pdtledg);

        document.Close();
        Response.Flush();
        Response.End();
    }
    protected void lbExpExcel_Click(object sender, EventArgs e)
    {
        DataTable ddt = (DataTable)ViewState["Excel"];
        if (ddt.Rows.Count > 0)
        {
            ddt.Columns["gl_coa_code"].SetOrdinal(0);
            ddt.Columns["coa_desc"].SetOrdinal(1);
            ddt.Columns["Openin"].SetOrdinal(2);
            ddt.Columns["Period_amount_Dr"].SetOrdinal(3);
            ddt.Columns["Period_amount_Cr"].SetOrdinal(4);
            ddt.Columns["Closing"].SetOrdinal(5);
            if (!ddt.Columns[0].ColumnName.Equals("Acc. Code"))
            {
                ddt.Columns["gl_coa_code"].ColumnName = "Account Code";
                ddt.Columns["coa_desc"].ColumnName = "Chart Of Account";
                ddt.Columns["Openin"].ColumnName = "Opening Blance";
                ddt.Columns["Period_amount_Dr"].ColumnName = "Period Amount(Dr)";
                ddt.Columns["Period_amount_Cr"].ColumnName = "Period Amount (Cr)";
                ddt.Columns["Closing"].ColumnName = "Closing Amount"; 
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ddt, "RooLeafSummery");
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=" + lblTitle.Text.Replace(" ", "") + "-RootLeafSummer-"+lblDate.Text.Replace(" ","")+".xlsx");

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

    protected void dgLedger_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void dgLedger_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("View"))
        {
            //Response.Write("<script>");
            //Response.Write("window.open('rptLedgerStat.aspx?RepID='Rakib','_blank')");
            //Response.Write("</script>");
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            gvr.Cells[2].BackColor = gvr.Cells[3].BackColor = gvr.Cells[0].BackColor = gvr.Cells[1].BackColor = Color.Bisque;
            Response.Write("<script>");
            Response.Write("window.open('rptLedgerStat.aspx?reptype=5&startdt=" + lblStartDate.Text + "&enddt=" +
                           lblEndDate.Text + "&glcoa=" + gvr.Cells[1].Text +
                           " &replvl=&seglvl=&vchtyp=&rptsysid=&notes=" + gvr.Cells[2].Text + " ','_blank')");
            Response.Write("</script>");
           
        }
    }
}