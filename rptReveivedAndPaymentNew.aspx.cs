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
using System.Collections.Generic;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Office2010.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.pdf.draw;
using System.Reflection;
using Delve;
using Color = System.Drawing.Color;
using ClosedXML.Excel;
using System.IO;

public partial class rptReveivedAndPaymentNew : System.Web.UI.Page
{
    private static string book = string.Empty;
    private static string UserType = string.Empty;
    VouchManager _aVouchManager=new VouchManager();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            txtStartDt.Attributes.Add("onBlur", "formatdate('" + txtStartDt.ClientID + "')");
            txtEndDt.Attributes.Add("onBlur", "formatdate('" + txtEndDt.ClientID + "')");
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
            hfRepLvl.Value = RepLvl;
            hfSegLvl.Value = SegLvl;
            string VchType = Request.QueryString["vchtyp"].ToString();
            string RptSysId = Request.QueryString["rptsysid"].ToString();
            string NotesNo = Request.QueryString["notes"].ToString();
            RepType = RepType.Substring(0, 1).Trim();
            
            DataTable dtRp;
            DataTable dtVch = _aVouchManager.GetShowPaymentAndReceived(glCode, StartDt, EndDt,
                Session["UserType"].ToString());
            string connectionString = DataManager.OraConnString();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            lblDate.Text = "";
            lblTitle.Text = "";
            if (EndDt == "" | DataManager.DateEncode(EndDt).CompareTo(System.DateTime.Now) > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    "alert('Please select the Upto Date or Upto Date cannot be greater than System Date!!');", true);
            }
            else
            {
                lblDate.Text = " For the Period " + StartDt + " TO " + EndDt;
                lblTitle.Text = "Receipts & Payments";
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
                    else if (RepLvl == "" | SegLvl == "")
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('Please select Report Level and Segment Level!!');", true);
                    }
                    else
                    {
                        string query = "with t(seg_coa_code,seg_coa_desc,parent_code,rootleaf,levels,acc_type) as " +
                                       " (select seg_coa_code,seg_coa_desc,PARENT_CODE,rootleaf,0 levels,ACC_TYPE from GL_SEG_COA where SEG_COA_CODE=(SELECT SEG_COA_CODE FROM GL_SEG_COA WHERE BOOK_NAME  = " +
                                       " '" + book + "' AND LVL_CODE   = '" + SegLvl +
                                       "' AND PARENT_CODE IS NULL) and LVL_CODE='" + SegLvl + "' " +
                                       " union all select tt.seg_coa_code,tt.seg_coa_desc,tt.parent_code,tt.ROOTLEAF,t.levels+1 levels,t.acc_type from GL_SEG_COA as tt ,t " +
                                       " where t.seg_coa_code=tt.PARENT_CODE and tt.LVL_CODE='" + SegLvl + "') " +
                                       " SELECT s.SEG_COA_CODE,s.SEG_COA_DESC,'0' p_db_amt,'0' p_cr_amt, '0' u_db_amt,'0' u_cr_amt,s.acc_type,s.rootleaf  FROM  gl_seg_coa s where seg_coa_code in (" +
                                       " select seg_coa_code from t where ((LEVELs = " + RepLvl + ") OR (LEVELs < " +
                                       RepLvl +
                                       " AND  ROOTLEAF = 'L')))  and seg_coa_code not in(1010000,1020000,1030000) order by 1";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        DataSet Ds = new DataSet();
                        adapter.Fill(Ds, "dtAccountBal");
                        DataView view = Ds.Tables[0].DefaultView;
                        view.Sort = "acc_type ASC";
                        DataTable sortedDate = view.ToTable();
                        if (sortedDate.Rows.Count == 0)
                        {
                            lblTitle.Text = "A";
                            return;
                        }
                        else
                        {
                            string mainseg = "";
                            dtRp = sortedDate.Clone();
                            dtRp.Columns.Add("Status", typeof (int));
                            DataRow drRp;
                            decimal totalAmountPayment = decimal.Zero;
                            decimal totalAmountReceived = decimal.Zero;
                            double OpBlanceBank = BankAndCashBlanceCheck.GetCheckBlanceInBankAndCash("1-1020000", 1,
                                Session["UserType"].ToString(),
                                DataManager.DateEncode(StartDt).AddDays(-1).ToString("dd/MM/yyyy"),
                                DataManager.DateEncode(StartDt).AddDays(-1).ToString("dd/MM/yyyy"));
                            double OpBlanceCashInHand = BankAndCashBlanceCheck.GetCheckBlanceInBankAndCash("1-1010000",
                                1,
                                Session["UserType"].ToString(),
                                DataManager.DateEncode(StartDt).AddDays(-1).ToString("dd/MM/yyyy"),
                                DataManager.DateEncode(StartDt).AddDays(-1).ToString("dd/MM/yyyy"));

                            drRp = dtRp.NewRow();
                            drRp["SEG_COA_CODE"] = "";
                            drRp["SEG_COA_DESC"] = "Opening Cash at Bank : ";
                            drRp["p_cr_amt"] = OpBlanceBank.ToString("N3");
                            drRp["Status"] = "11";
                            dtRp.Rows.Add(drRp);

                            drRp = dtRp.NewRow();
                            drRp["SEG_COA_CODE"] = "";
                            drRp["SEG_COA_DESC"] = "Opening Cash in Hand : ";
                            drRp["p_cr_amt"] = OpBlanceCashInHand.ToString("N3");
                            drRp["Status"] = "11";
                            dtRp.Rows.Add(drRp);

                            for (int ij = 1; ij < 3; ij++)
                            {

                                if (ij.Equals(1))
                                {
                                    drRp = dtRp.NewRow();
                                    drRp["SEG_COA_CODE"] = "";
                                    drRp["SEG_COA_DESC"] = "Receipts : ";
                                    drRp["Status"] = "11";
                                    dtRp.Rows.Add(drRp);
                                }
                                else if (ij.Equals(2))
                                {
                                    drRp = dtRp.NewRow();
                                    drRp["SEG_COA_CODE"] = "";
                                    drRp["SEG_COA_DESC"] = "Payments : ";
                                    drRp["Status"] = "11";
                                    dtRp.Rows.Add(drRp);
                                }
                                foreach (DataRow dr in sortedDate.Rows)
                                {
                                    drRp = dtRp.NewRow();
                                    string segcoacode = "";
                                    string[] segcode = glCode.Split(Convert.ToChar(Session["septype"].ToString()));
                                    for (int i = 0; i < segcode.Length; i++)
                                    {
                                        if (segcode[i].Length == dr["seg_coa_code"].ToString().Trim().Length)
                                        {
                                            segcode[i] = dr["seg_coa_code"].ToString().Trim();
                                            mainseg = dr["seg_coa_code"].ToString().Trim();
                                        }
                                        segcoacode += segcode[i] + Session["septype"].ToString();
                                    }
                                    segcoacode = segcoacode.Trim().Remove(segcoacode.Length - 1, 1).ToString();

                                    decimal totAmount = GlCoaManagerRP.getCoaBalanceFromExistingDataTable(dtVch, book,
                                        segcoacode,
                                        Session["septype"].ToString(), "A", StartDt, EndDt, "", ij);
                                    if (totAmount > 0)
                                    {
                                        drRp["SEG_COA_CODE"] = dr["SEG_COA_CODE"].ToString();
                                        drRp["SEG_COA_DESC"] = dr["SEG_COA_DESC"].ToString();
                                        drRp["p_db_amt"] = totAmount.ToString("N3");
                                        drRp["acc_type"] = dr["acc_type"].ToString();
                                        drRp["Status"] = ij;
                                        drRp["rootleaf"] = dr["rootleaf"].ToString();
                                        dtRp.Rows.Add(drRp);
                                        if (ij.Equals(1))
                                        {
                                            totalAmountPayment += totAmount;
                                        }
                                        else
                                        {
                                            totalAmountReceived += totAmount;
                                        }
                                        //dr.BeginEdit();
                                        //dr.AcceptChanges();
                                    }
                                }
                                if (ij.Equals(1))
                                {
                                    drRp = dtRp.NewRow();
                                    drRp["SEG_COA_CODE"] = "";
                                    drRp["SEG_COA_DESC"] = "Net Receipts : ";
                                    drRp["p_cr_amt"] = totalAmountPayment.ToString("N3");
                                    drRp["Status"] = "11";
                                    dtRp.Rows.Add(drRp);

                                    drRp = dtRp.NewRow();
                                    drRp["SEG_COA_CODE"] = "";
                                    drRp["SEG_COA_DESC"] = "Total : ";
                                    drRp["p_db_amt"] = "";
                                    drRp["p_cr_amt"] = (Convert.ToDouble(totalAmountPayment) + OpBlanceBank + OpBlanceCashInHand).ToString("N2");
                                    drRp["Status"] = "11";
                                    dtRp.Rows.Add(drRp);
                                }
                                else if (ij.Equals(2))
                                {
                                    drRp = dtRp.NewRow();
                                    drRp["SEG_COA_CODE"] = "";
                                    drRp["SEG_COA_DESC"] = "Payments : ";
                                    drRp["p_cr_amt"] = totalAmountReceived.ToString("N3");
                                    drRp["Status"] = "11";
                                    dtRp.Rows.Add(drRp);
                                }
                            }
                            //drRp = dtRp.NewRow();
                            //drRp["SEG_COA_CODE"] = "";
                            //drRp["SEG_COA_DESC"] = "";
                            //drRp["p_cr_amt"] = "";
                            //drRp["Status"] = "11";
                            //dtRp.Rows.Add(drRp);
                            //drRp = dtRp.NewRow();
                            //drRp["SEG_COA_CODE"] = "";
                            //drRp["SEG_COA_DESC"] = "Closing Balance : ";
                            //drRp["p_db_amt"] = (totalAmountPayment - totalAmountReceived).ToString("N3");
                            //drRp["Status"] = "11";
                            //dtRp.Rows.Add(drRp);
                            double BlanceBank = BankAndCashBlanceCheck.GetCheckBlanceInBankAndCash("1-1020000", 1,
                                Session["UserType"].ToString(), StartDt, EndDt);
                            drRp = dtRp.NewRow();
                            drRp["SEG_COA_CODE"] = "1020000";
                            drRp["SEG_COA_DESC"] = "Closing Cash at Bank : ";
                            drRp["p_cr_amt"] = BlanceBank.ToString("N3");
                            drRp["Status"] = "11";
                            dtRp.Rows.Add(drRp);
                            double BlanceCashInHand = BankAndCashBlanceCheck.GetCheckBlanceInBankAndCash("1-1010000", 1,
                                Session["UserType"].ToString(), StartDt, EndDt);
                            drRp = dtRp.NewRow();
                            drRp["SEG_COA_CODE"] = "1010000";
                            drRp["SEG_COA_DESC"] = "Closing Cash in Hand : ";
                            drRp["p_cr_amt"] = BlanceCashInHand.ToString("N3");
                            drRp["Status"] = "11";
                            dtRp.Rows.Add(drRp);
                            drRp = dtRp.NewRow();
                            drRp["SEG_COA_CODE"] = "";
                            drRp["SEG_COA_DESC"] = "Total : ";
                            drRp["p_db_amt"] = "";
                            drRp["p_cr_amt"] = (BlanceCashInHand + BlanceBank + Convert.ToDouble(totalAmountReceived)).ToString("N2");
                            drRp["Status"] = "11";
                            dtRp.Rows.Add(drRp);

                            dgAccountBal.Columns[1].ShowHeader = true;
                            dgAccountBal.Columns[1].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgAccountBal.Columns[1].HeaderText = "Head Of Account";
                            dgAccountBal.Columns[2].ShowHeader = true;
                            dgAccountBal.Columns[2].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgAccountBal.Columns[2].HeaderText = "Amount";
                            dgAccountBal.Columns[3].ShowHeader = true;
                            dgAccountBal.Columns[3].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgAccountBal.Columns[3].HeaderText = "Total";
                            dgAccountBal.Columns[4].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgAccountBal.Columns[4].ShowHeader = true;
                            dgAccountBal.Columns[4].HeaderText = "Cumulative Debit Amount";
                            dgAccountBal.Columns[5].ShowHeader = true;
                            dgAccountBal.Columns[5].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgAccountBal.Columns[5].HeaderText = "Cumulative Credit Amount";
                            dgAccountBal.Visible = true;
                            dgAccountBal.DataSource = dtRp;
                            Session["dtaccountbal"] = dtRp;
                            dgAccountBal.DataBind();
                        }
                    }
            }
        }
    }
    //*************8 Trail Blance ************//
    protected void dgAccountBal_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[4].Attributes.Add("style", "display:none");
            e.Row.Cells[5].Attributes.Add("style", "display:none");
            e.Row.Cells[6].Attributes.Add("style", "display:none");
            e.Row.Cells[7].Attributes.Add("style", "display:none");
        }
        else if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[4].Attributes.Add("style", "display:none");
            e.Row.Cells[5].Attributes.Add("style", "display:none");
            e.Row.Cells[6].Attributes.Add("style", "display:none");
            e.Row.Cells[7].Attributes.Add("style", "display:none");
            if (e.Row.Cells[6].Text.Equals("11"))
            {
                e.Row.Cells[1].Attributes.Add("style", "font-size: 12px");
                //e.Row.Cells[1].Font.Bold = true;
            }
        }
    }
    protected void lbExp_Click(object sender, EventArgs e)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=MypdfFile.pdf");
        Document document = new Document();
        document = new Document(PageSize.A4, 40f, 30f, 30f, 30f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        pdfPage page = new pdfPage();
        writer.PageEvent = page;
        document.Open();

        string RepType = Request.QueryString["reptype"].ToString();
        string StartDt = "01/01/2015";
        string EndDt = Request.QueryString["enddt"].ToString();
        string glCode = Request.QueryString["glcoa"].ToString();
        string RepLvl = Request.QueryString["replvl"].ToString();
        string SegLvl = Request.QueryString["seglvl"].ToString();
        string VchType = Request.QueryString["vchtyp"].ToString();
        string RptSysId = Request.QueryString["rptsysid"].ToString();
        string NotesNo = Request.QueryString["notes"].ToString();
        RepType = RepType.Substring(0, 1).Trim();
        string title = "";
        PdfPCell cell;
        if (RepType == "B" | RepType == "I" | RepType == "C")
        {
            //if (NotesNo == "")
            //{
            //    lblDate.Text = RepType.Replace("B", " AS ON " + EndDt).Replace("I", " AS ON " + StartDt + " TO " + EndDt).Replace("C", " AS ON " + EndDt).ToUpper();
            //    title = RepType.Replace("B", "Balanace Sheet Statement").Replace("I", "Income Statement").Replace("C", "Cash Flow Statement") + " as on " + EndDt;
            //}
            //else
            //{
            //    lblDate.Text = RepType.Replace("B", " AS ON " + EndDt).Replace("I", " AS ON " + StartDt + " TO " + EndDt).Replace("C", " AS ON " + EndDt).ToUpper();
            //    title = RepType.Replace("B", "Balanace Sheet Statement Notes").Replace("I", "Income Statement Notes").Replace("C", "Cash Flow Statement Notes") + " as on " + EndDt;
            //}
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
           // cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(lblTitle.Text + " " + lblDate.Text, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
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

            if (NotesNo == "")
            {
                DataTable dtbi = (DataTable)Session["dtrptbalsheet"];
                string adt = "";
                string bdt = "";
                foreach (DataRow dr in dtbi.Rows)
                {
                    if (dr["amount_a_dt"].ToString() != "")
                    {
                        adt = dr["amount_a_dt"].ToString();
                        break;
                    }
                }
                foreach (DataRow dr in dtbi.Rows)
                {
                    if (dr["amount_b_dt"].ToString() != "")
                    {
                        bdt = dr["amount_b_dt"].ToString();
                        break;
                    }
                }
                float[] widthbis = new float[4] { 10, 60, 10, 30 };
                float[] widthbi = new float[5] { 10, 60, 10, 30, 30 };
                PdfPTable pdtbi;
                int colno = 0;
                if (bdt == "")
                {
                    pdtbi = new PdfPTable(widthbis);
                     colno = dtbi.Columns.Count - 5;

                }
                else
                {
                    // pdtbi = new PdfPTable(widthbi);
                    pdtbi = new PdfPTable(widthbis);
                    colno = dtbi.Columns.Count - 5;
                   // colno = dtbi.Columns.Count - 4;
                }
                pdtbi.WidthPercentage = 100;
                cell = new PdfPCell(FormatPhrase(""));
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbi.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Particulars"));
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbi.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Notes"));
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbi.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Amount"));
                cell.HorizontalAlignment = 1;
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbi.AddCell(cell);
                //if (bdt != "")
                //{
                //    cell = new PdfPCell(FormatHeaderPhrase(bdt));
                //    cell.HorizontalAlignment = 1;
                //    cell.FixedHeight = 15f;
                //    cell.BorderColor = BaseColor.LIGHT_GRAY;
                //    pdtbi.AddCell(cell);
                //}
                pdtbi.HeaderRows = 1;
                for (int i = 0; i < dtbi.Rows.Count; i++)
                {
                    for (int j = 0; j < colno; j++)
                    {
                        string val = "";
                        if (j == 2)
                        {
                            if (((DataRow)dtbi.Rows[i])[j].ToString().Trim().Replace("&nbsp;", "") != "")
                            {
                                val = decimal.Parse(((DataRow)dtbi.Rows[i])[j].ToString()).ToString("N3");
                            }
                        }
                        else if (j > 2)
                        {
                            if (((DataRow)dtbi.Rows[i])[1].ToString().Trim().Replace("&nbsp;", "") != "")
                            {
                                if (((DataRow)dtbi.Rows[i])[1].ToString().Trim().Replace("&nbsp;", "").Substring(((DataRow)dtbi.Rows[i])[1].ToString().Trim().Replace("&nbsp;", "").Length - 3).Contains(':') == false)
                                {
                                    val = decimal.Parse(((DataRow)dtbi.Rows[i])[j].ToString()).ToString("N3");
                                }
                            }
                        }
                        else
                        {
                            val = ((DataRow)dtbi.Rows[i])[j].ToString();
                        }
                        cell = new PdfPCell(FormatPhrase(val));
                        cell.FixedHeight = 15f;
                        if (j == 2 | j == 0)
                        {
                            cell.HorizontalAlignment = 1;
                        }
                        else if (j > 2)
                        {
                            cell.HorizontalAlignment = 2;
                        }
                        cell.BorderColor = BaseColor.LIGHT_GRAY;
                        pdtbi.AddCell(cell);
                    }
                }
                document.Add(pdtbi);
            }
            else if (NotesNo == "ALL")
            {
                DataTable dtbin = (DataTable)Session["dtbalnotesall"];
                string adt = "";
                string bdt = "";
                foreach (DataRow dr in dtbin.Rows)
                {
                    if (dr["amount_a_dt"].ToString() != "")
                    {
                        adt = dr["amount_a_dt"].ToString();
                        break;
                    }
                }
                foreach (DataRow dr in dtbin.Rows)
                {
                    if (dr["amount_b_dt"].ToString() != "")
                    {
                        bdt = dr["amount_b_dt"].ToString();
                        break;
                    }
                }
                float[] widthbisn = new float[5] { 10, 10, 60, 10, 30 };
                float[] widthbin = new float[6] { 10, 10, 60, 10, 30, 30 };
                PdfPTable pdtbin;
                int colno = 0;
                if (bdt == "")
                {
                    pdtbin = new PdfPTable(widthbisn);
                    colno = dtbin.Columns.Count - 6;
                }
                else
                {
                    pdtbin = new PdfPTable(widthbin);
                    colno = dtbin.Columns.Count - 5;
                }
                pdtbin.WidthPercentage = 100;
                cell = new PdfPCell(FormatPhrase("Notes"));
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbin.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbin.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbin.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Parent Notes"));
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbin.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(adt));
                cell.HorizontalAlignment = 1;
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbin.AddCell(cell);
                if (bdt != "")
                {
                    cell = new PdfPCell(FormatHeaderPhrase(bdt));
                    cell.HorizontalAlignment = 1;
                    cell.FixedHeight = 15f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtbin.AddCell(cell);
                }
                for (int i = 0; i < dtbin.Rows.Count; i++)
                {
                    for (int j = 0; j < colno; j++)
                    {
                        string val = "";
                        if (j == 3 | j == 0)
                        {
                            if (((DataRow)dtbin.Rows[i])[j].ToString().Trim().Replace("&nbsp;", "") != "")
                            {
                                val = decimal.Parse(((DataRow)dtbin.Rows[i])[j].ToString()).ToString("N3");
                            }
                        }
                        else if (j > 3)
                        {
                            if (((DataRow)dtbin.Rows[i])[2].ToString().Trim().Replace("&nbsp;", "") != "")
                            {
                                if (((DataRow)dtbin.Rows[i])[2].ToString().Trim().Replace("&nbsp;", "").Substring(((DataRow)dtbin.Rows[i])[2].ToString().Trim().Replace("&nbsp;", "").Length - 3).Contains(':') == false)
                                {
                                    val = decimal.Parse(((DataRow)dtbin.Rows[i])[j].ToString()).ToString("N3");
                                }
                            }
                        }
                        else
                        {
                            val = ((DataRow)dtbin.Rows[i])[j].ToString();
                        }
                        cell = new PdfPCell(FormatPhrase(val));
                        cell.FixedHeight = 15f;
                        if (j == 3 | j == 0 | j == 1)
                        {
                            cell.HorizontalAlignment = 1;
                        }
                        else if (j > 3)
                        {
                            cell.HorizontalAlignment = 2;
                        }
                        cell.BorderColor = BaseColor.LIGHT_GRAY;
                        pdtbin.AddCell(cell);
                    }
                }
                document.Add(pdtbin);
            }
            else
            {
                DataTable dtbin = (DataTable)Session["dtbalnotes"];
                string adt = "";
                string bdt = "";
                foreach (DataRow dr in dtbin.Rows)
                {
                    if (dr["amount_a_dt"].ToString() != "")
                    {
                        adt = dr["amount_a_dt"].ToString();
                        break;
                    }
                }
                foreach (DataRow dr in dtbin.Rows)
                {
                    if (dr["amount_b_dt"].ToString() != "")
                    {
                        bdt = dr["amount_b_dt"].ToString();
                        break;
                    }
                }
                float[] widthbisn = new float[5] { 10, 10, 60, 10, 30 };
                float[] widthbin = new float[6] { 10, 10, 60, 10, 30, 30 };
                PdfPTable pdtbin;
                int colno = 0;
                if (bdt == "")
                {
                    pdtbin = new PdfPTable(widthbisn);
                    colno = dtbin.Columns.Count - 6;
                }
                else
                {
                    pdtbin = new PdfPTable(widthbin);
                    colno = dtbin.Columns.Count - 5;
                }
                pdtbin.WidthPercentage = 100;
                cell = new PdfPCell(FormatPhrase("Notes"));
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbin.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbin.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbin.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Parent Notes"));
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbin.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(adt));
                cell.HorizontalAlignment = 1;
                cell.FixedHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtbin.AddCell(cell);
                if (bdt != "")
                {
                    cell = new PdfPCell(FormatHeaderPhrase(bdt));
                    cell.HorizontalAlignment = 1;
                    cell.FixedHeight = 15f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtbin.AddCell(cell);
                }
                for (int i = 0; i < dtbin.Rows.Count; i++)
                {
                    for (int j = 0; j < colno; j++)
                    {
                        string val = "";
                        if (j == 3 | j == 0)
                        {
                            if (((DataRow)dtbin.Rows[i])[j].ToString().Trim().Replace("&nbsp;", "") != "")
                            {
                                val = decimal.Parse(((DataRow)dtbin.Rows[i])[j].ToString()).ToString("N3");
                            }
                        }
                        else if (j > 3)
                        {
                            if (((DataRow)dtbin.Rows[i])[2].ToString().Trim().Replace("&nbsp;", "") != "")
                            {
                                if (((DataRow)dtbin.Rows[i])[2].ToString().Trim().Replace("&nbsp;", "").Substring(((DataRow)dtbin.Rows[i])[2].ToString().Trim().Replace("&nbsp;", "").Length - 3).Contains(':') == false)
                                {
                                    val = decimal.Parse(((DataRow)dtbin.Rows[i])[j].ToString()).ToString("N3");
                                }
                            }
                        }
                        else
                        {
                            val = ((DataRow)dtbin.Rows[i])[j].ToString();
                        }
                        cell = new PdfPCell(FormatPhrase(val));
                        cell.FixedHeight = 15f;
                        if (j == 3 | j == 0 | j == 1)
                        {
                            cell.HorizontalAlignment = 1;
                        }
                        else if (j > 3)
                        {
                            cell.HorizontalAlignment = 2;
                        }
                        cell.BorderColor = BaseColor.LIGHT_GRAY;
                        pdtbin.AddCell(cell);
                    }
                }
                document.Add(pdtbin);
            }
        }
        else if (RepType == "7")
        {
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
            string typeseg = Session["typeseg"].ToString();
            PdfPTable pdtaccbal;
            int acolno = 0;
            if (typeseg == "N")
            {
                float[] width = new float[6] { 15, 60, 40, 40, 40, 40 };
                pdtaccbal = new PdfPTable(width);
                acolno = 6;
                cell = new PdfPCell(FormatPhrase(""));
                cell.MinimumHeight = 15f;
                cell.HorizontalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Segment Description"));
                cell.MinimumHeight = 15f;
                cell.HorizontalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Current Period Debit Amount"));
                cell.MinimumHeight = 15f;
                cell.HorizontalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Currrent Period Credit Amount"));
                cell.MinimumHeight = 15f;
                cell.HorizontalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Cumulative Debit Amount"));
                cell.MinimumHeight = 15f;
                cell.HorizontalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Cumulative Credit Amount"));
                cell.MinimumHeight = 15f;
                cell.HorizontalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);                
            }
            else
            {
                float[] width = new float[4] { 15, 60, 40, 40 };
                pdtaccbal = new PdfPTable(width);
                acolno = 4;
                cell = new PdfPCell(FormatPhrase(""));
                cell.MinimumHeight = 15f;
                cell.HorizontalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Segment Description"));
                cell.MinimumHeight = 15f;
                cell.HorizontalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Current Period Balance"));
                cell.MinimumHeight = 15f;
                cell.HorizontalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Cumulative Balance"));
                cell.MinimumHeight = 15f;
                cell.HorizontalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
            }  
            DataTable dtaccbal = (DataTable)Session["dtaccountbal"];
            for (int i = 0; i < dtaccbal.Rows.Count; i++)
            {
                for (int j = 0; j < acolno; j++)
                {                    
                    string val = "";
                    if (j > 1)
                    {
                        if (((DataRow)dtaccbal.Rows[i])[j].ToString() != "")
                        {
                            val = decimal.Parse(((DataRow)dtaccbal.Rows[i])[j].ToString()).ToString("N3");
                        }
                    }
                    else
                    {
                        val = ((DataRow)dtaccbal.Rows[i])[j].ToString();
                    }
                    cell = new PdfPCell(FormatPhrase(val));
                    if (j > 1)
                    {
                        cell.HorizontalAlignment = 2;                        
                    }
                    else if (j == 0)
                    {
                        cell.HorizontalAlignment = 1;
                    }
                    cell.VerticalAlignment = 1;
                    cell.MinimumHeight = 15f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtaccbal.AddCell(cell);
                }                
            }
            pdtaccbal.WidthPercentage = 100;
            document.Add(pdtaccbal);
        }
        document.Close();
        Response.Flush();
        Response.End();
    }
    
    public override void VerifyRenderingInServerForm(Control control)
    {

    }
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8));
    }
    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD));
    }
    protected void dgAccountBal_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        if (Session["dtaccountbal"] != null)
        {
            DataTable dt = (DataTable)Session["dtaccountbal"];
            dgAccountBal.DataSource = dt;
            dgAccountBal.PageIndex = e.NewPageIndex;
            dgAccountBal.DataBind();
        }
    }
    protected void dgBalNotes_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "View")
        {
            GridViewRow gvr = (GridViewRow) (((LinkButton) e.CommandSource).NamingContainer);
            gvr.Cells[2].BackColor =
                gvr.Cells[3].BackColor = gvr.Cells[0].BackColor = gvr.Cells[1].BackColor = Color.Bisque;
            hfRootLeft.Value = IdManager.GetShowSingleValueString("[ROOTLEAF]", "[SEG_COA_CODE]", "[GL_SEG_COA]",
                gvr.Cells[1].Text);
            lblCoaCode.Text ="1-"+ gvr.Cells[1].Text;
            lblCoaName.Text = gvr.Cells[2].Text.ToUpper();
            txtEndDt.Text = txtStartDt.Text = string.Empty;
            txtStartDt.Focus();
            //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "text", "LoadModalDiv()", true);
           // ClientScript.RegisterStartupScript(this.GetType(), "print", "<script>LoadModalDiv();</script>");

            string str = "<script>function LoadModalDiv() {var bcgDiv = document.getElementById('divBackground');bcgDiv.style.display = 'block';}</script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Script", str, false);

            ModalPopupExtenderLogin.Show();
        }
    }
    protected void btnShow_Click(object sender, EventArgs e)
    {
        Response.Write("<script>");
        Response.Write("window.open('rptLedgerReveivedAndPayment.aspx?reptype=TB&glcoa=" + lblCoaCode.Text +
                       "&replvl=" + hfRepLvl.Value + "&seglvl=" + hfSegLvl.Value + "&vchtyp=&startdt=" + txtStartDt.Text +
                       "&enddt=" + txtEndDt.Text +
                       "&rptsysid=&notes=" + lblCoaName.Text.Replace("&", "And") + "','_blank')");
        Response.Write("</script>");
    }
    protected void dgAccountBal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "View")
        {
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            gvr.Cells[2].BackColor =
                gvr.Cells[3].BackColor = gvr.Cells[0].BackColor = gvr.Cells[1].BackColor = Color.Bisque;
            hfRootLeft.Value = gvr.Cells[7].Text.Trim();
            lblCoaCode.Text = gvr.Cells[0].Text;
            lblCoaName.Text = gvr.Cells[1].Text.ToUpper();
            txtEndDt.Text = txtStartDt.Text = string.Empty;
            txtStartDt.Focus();
            //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "text", "LoadModalDiv()", true);
            // ClientScript.RegisterStartupScript(this.GetType(), "print", "<script>LoadModalDiv();</script>");

            string str = "<script>function LoadModalDiv() {var bcgDiv = document.getElementById('divBackground');bcgDiv.style.display = 'block';}</script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Script", str, false);

            ModalPopupExtenderLogin.Show();
        }
    }
    protected void lbExpExcel_Click(object sender, EventArgs e)
    {
        DataTable dtAllPR = (DataTable)Session["dtaccountbal"];
        if (dtAllPR.Rows.Count > 0)
        {
            DataTable dtAccPR = new DataTable();
            dtAccPR = dtAllPR;
            dtAccPR.Columns.Remove("rootleaf");
            dtAccPR.Columns.Remove("acc_type");
            dtAccPR.Columns.Remove("u_cr_amt");
            dtAccPR.Columns.Remove("u_db_amt");
            dtAccPR.Columns.Remove("Status");
                // dtlist.Columns["coa_desc"].ColumnName = "Chart Of Account(COA)";
            dtAccPR.Columns["SEG_COA_CODE"].ColumnName = "Chart of Account";
            dtAccPR.Columns["SEG_COA_DESC"].ColumnName = "Particular";
            dtAccPR.Columns["p_db_amt"].ColumnName = "Amount";
            dtAccPR.Columns["p_cr_amt"].ColumnName = "Total";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dtAccPR, "Account-RP");
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition",
                    "attachment;filename=" + lblTitle.Text.Replace(" ", "") + "-AccountReceived&Payment-(" +
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

    protected void lbPrint_Click(object sender, EventArgs e)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename=Receipts&Payments-(" + DateTime.Now.ToString("dd-MM-yyyy") + ").pdf");
        Document document = new Document();
        document = new Document(PageSize.A4, 40f, 30f, 30f, 30f);
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

        float[] width = new float[4] {10, 40, 10, 10};
        pdtaccbal = new PdfPTable(width);
        pdtaccbal.WidthPercentage = 100;
        pdtaccbal.HeaderRows = 1;
        cell = new PdfPCell(FormatHeaderPhrase("Account Code"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtaccbal.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Head Name "));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtaccbal.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtaccbal.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.MinimumHeight = 15f;
        cell.HorizontalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtaccbal.AddCell(cell);

        DataTable dtaccbal = (DataTable) Session["dtaccountbal"];
        foreach (DataRow drRP in dtaccbal.Rows)
        {
            try
            {
                cell = new PdfPCell(FormatPhrase(drRP["SEG_COA_CODE"].ToString()));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.MinimumHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(drRP["SEG_COA_DESC"].ToString()));
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(drRP["p_db_amt"].ToString()));
                cell.VerticalAlignment = 0;
                cell.HorizontalAlignment = 2;
                cell.MinimumHeight = 15f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtaccbal.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(drRP["p_cr_amt"].ToString()));
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
}