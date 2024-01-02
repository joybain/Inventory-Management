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

public partial class rptLedgerStat : System.Web.UI.Page
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

                DataTable dtVch = VouchManager.GetVouchDtlForTotal(EndDt, "A", book, "");
                string connectionString = DataManager.OraConnString();
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connectionString;
                lblTranStatus.Text = "";
                // | DataManager.DateEncode(EndDt).CompareTo(System.DateTime.Now) > 0
                if (EndDt == "")
                {
                    lblTranStatus.Text = "Please select the Upto Date or Upto Date cannot be greater than System Date.";
                    lblTranStatus.ForeColor = System.Drawing.Color.Red;
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Please Input End Date.!!');",
                        true);
                }
                else
                {
                    if (RepType == "3" | RepType == "4")
                    {
                        lblTitle.Text = RepType.Replace("3", "CASH BOOK").Replace("4", "BANK BOOK");
                        lblDate.Text = " AS ON " + StartDt + " TO " + EndDt;
                        string criteria = GlCoaManager.getCoaCriteria(glCode, Session["septype"].ToString(), book);
                        DataTable dtAlready = GlCoaManager.GetGlCoaCodes(criteria);
                        string glcoacode = "";
                        //decimal OpeningBalance=decimal.Zero;
                        foreach (DataRow drA in dtAlready.Rows)
                        {
                            glcoacode += "'" + drA["gl_coa_code"].ToString() + "'" + ",";
                            //OpeningBalance += GlCoaManager.GetShowOpeningBalance(drA["gl_coa_code"].ToString());
                        }
                        if (glcoacode != "")
                        {
                            glcoacode = glcoacode.Remove(glcoacode.Length - 1, 1);
                            string query =
                                @"select * from ( select a.vch_sys_no AS Vch_sys_no, 'A' acc_type,a.value_date v_date, convert(varchar,a.value_date,103) AS value_date,  b.gl_coa_code+' : '+(select coa_desc from gl_coa where gl_coa_code=b.gl_coa_code) as coa_desc,a.VCH_REF_NO,
(case when (select COUNT(*) from GL_TRANS_DTL tt inner join gl_coa tt1 on tt1.GL_COA_CODE=tt.GL_COA_CODE where tt.VCH_SYS_NO=a.VCH_SYS_NO and (tt.AMOUNT_DR=CASE WHEN b.AMOUNT_DR !=0 then 0 else -1 end or tt.AMOUNT_CR=CASE WHEN b.AMOUNT_CR !=0 then 0 else -1 end))=1 then(select top(1) tt.PARTICULARS from GL_TRANS_DTL tt inner join gl_coa tt1 on tt1.GL_COA_CODE=tt.GL_COA_CODE where tt.VCH_SYS_NO=a.VCH_SYS_NO and (tt.AMOUNT_DR=CASE WHEN b.AMOUNT_DR !=0 then 0 else -1 end or tt.AMOUNT_CR=CASE WHEN b.AMOUNT_CR !=0 then 0 else -1 end))else (select top(1) tt.PARTICULARS from GL_TRANS_DTL tt inner join gl_coa tt1 on tt1.GL_COA_CODE=tt.GL_COA_CODE where tt.VCH_SYS_NO=a.VCH_SYS_NO and (tt.AMOUNT_DR=CASE WHEN b.AMOUNT_DR !=0 then 0 else -1 end or tt.AMOUNT_CR=CASE WHEN b.AMOUNT_CR !=0 then 0 else -1 end))+','+char(13)+ (select top(1) tt.PARTICULARS from GL_TRANS_DTL tt inner join gl_coa tt1 on tt1.GL_COA_CODE=tt.GL_COA_CODE where tt.VCH_SYS_NO=a.VCH_SYS_NO and (tt.AMOUNT_DR=CASE WHEN b.AMOUNT_DR !=0 then 0 else -1 end or tt.AMOUNT_CR=CASE WHEN b.AMOUNT_CR !=0 then 0 else -1 end) order by tt.ID desc)  end  ) AS[Part]
,a.PARTICULARS,  serial_no AS vch_manual_no,isnull(b.amount_dr,0) AS Debit_amt,isnull(b.amount_cr,0) AS Credit_amt,0 bal " +
                                " ,case when isnull(b.amount_dr,0) >0 then isnull(b.amount_dr,0) else isnull(b.amount_cr,0) end AS Amount from gl_trans_mst a ,gl_trans_dtl b where a.vch_sys_no=b.vch_sys_no and a.book_name='" +
                                book + "' " + " and b.gl_coa_code in (" + glcoacode +
                                ") and a.value_date between convert(datetime,nullif('" + StartDt +
                                "',''),103) and convert(datetime,nullif('" + EndDt +
                                "',''),103) and a.[STATUS]='A' ) tot where tot.Amount>0 order by convert(date,tot.VALUE_DATE,103),convert(int,tot.vch_sys_no) asc   ";
                            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                            DataSet Ds = new DataSet();
                            adapter.Fill(Ds, "dtLedger");
                            DataRow dr = Ds.Tables[0].NewRow();
                            dr["vch_sys_no"] = "99999999";
                            dr["acc_type"] = "";
                            dr["coa_desc"] = "";
                            dr["v_date"] = DataManager.DateEncode(StartDt);
                            dr["value_date"] = StartDt;
                            dr["vch_manual_no"] = "";
                            dr["particulars"] = "Opening Balance (REF. dt. " + StartDt + ")";
                            decimal SumOpBal = GlCoaManager.getCoaopeningBalanceFromExistingDataTable(dtVch, book,
                                glCode,
                                Session["septype"].ToString(), "A", StartDt, StartDt);
                            if (SumOpBal > 0)
                            {
                                dr["debit_amt"] = SumOpBal;
                                dr["credit_amt"] = decimal.Zero;
                            }
                            else
                            {
                                dr["debit_amt"] = decimal.Zero;
                                dr["credit_amt"] = SumOpBal*-1;
                            }
                            dr["bal"] = decimal.Zero;
                            Ds.Tables[0].Rows.InsertAt(dr, 0);
                            decimal bal = decimal.Zero;
                            foreach (DataRow drLedg in Ds.Tables[0].Rows)
                            {
                                drLedg.BeginEdit();
                                bal += decimal.Parse(drLedg["debit_amt"].ToString()) -
                                       decimal.Parse(drLedg["credit_amt"].ToString());
                                drLedg["bal"] = bal;
                                drLedg.AcceptChanges();
                            }
                            dgLedger.Visible = true;
                            dgLedger.DataSource = Ds.Tables[0];
                            Session["dtledger"] = Ds.Tables[0];
                            ViewState["dtledgerExcel"] = Ds.Tables[0];

                            dgLedger.DataBind();
                            ShowFooterTotal();
                            ShowFooterTotalWithOpenning();
                        }
                    }
                    else if (RepType == "5")
                    {
                        string coaexist = GlCoaManager.getCoaExistence(glCode);
                        if (coaexist != "Y")
                        {
                            lblTranStatus.Visible = true;
                            lblTranStatus.Text = "Please select any leaf/child segment code.";
                            lblTranStatus.ForeColor = System.Drawing.Color.Red;
                            return;
                        }

                        string query =
                            "select * from ( select a.vch_sys_no AS Vch_sys_no, a.value_date v_date, convert(varchar,a.value_date,103) AS value_date, b.gl_coa_code+' : '+(select coa_desc from gl_coa where gl_coa_code=b.gl_coa_code) as coa_desc,a.VCH_REF_NO,REPLACE((case when (select COUNT(*) from GL_TRANS_DTL tt inner join gl_coa tt1 on tt1.GL_COA_CODE=tt.GL_COA_CODE where tt.VCH_SYS_NO=a.VCH_SYS_NO and (tt.AMOUNT_DR=CASE WHEN b.AMOUNT_DR !=0 then 0 else -1 end or tt.AMOUNT_CR=CASE WHEN b.AMOUNT_CR !=0 then 0 else -1 end))=1 then(select top(1) tt.PARTICULARS from GL_TRANS_DTL tt inner join gl_coa tt1 on tt1.GL_COA_CODE=tt.GL_COA_CODE where tt.VCH_SYS_NO=a.VCH_SYS_NO and (tt.AMOUNT_DR=CASE WHEN b.AMOUNT_DR !=0 then 0 else -1 end or tt.AMOUNT_CR=CASE WHEN b.AMOUNT_CR !=0 then 0 else -1 end))" +
                            " when (select COUNT(*) from GL_TRANS_DTL tt inner join gl_coa tt1 on tt1.GL_COA_CODE=tt.GL_COA_CODE where tt.VCH_SYS_NO=a.VCH_SYS_NO and (tt.AMOUNT_DR=CASE WHEN b.AMOUNT_DR !=0 then 0 else -1 end or tt.AMOUNT_CR=CASE WHEN b.AMOUNT_CR !=0 then 0 else -1 end))=2 then (select top(1) tt.PARTICULARS from GL_TRANS_DTL tt inner join gl_coa tt1 on tt1.GL_COA_CODE=tt.GL_COA_CODE where tt.VCH_SYS_NO=a.VCH_SYS_NO and (tt.AMOUNT_DR=CASE WHEN b.AMOUNT_DR !=0 then 0 else -1 end or tt.AMOUNT_CR=CASE WHEN b.AMOUNT_CR !=0 then 0 else -1 end) and tt.AUTHO_USER<>'CS') " +
                            "else (select top(1) tt.PARTICULARS from GL_TRANS_DTL tt inner join gl_coa tt1 on tt1.GL_COA_CODE=tt.GL_COA_CODE where tt.VCH_SYS_NO=a.VCH_SYS_NO and (tt.AMOUNT_DR=CASE WHEN b.AMOUNT_DR !=0 then 0 else -1 end or tt.AMOUNT_CR=CASE WHEN b.AMOUNT_CR !=0 then 0 else -1 end))+','+char(13)+ (select top(1) tt.PARTICULARS from GL_TRANS_DTL tt inner join gl_coa tt1 on tt1.GL_COA_CODE=tt.GL_COA_CODE where tt.VCH_SYS_NO=a.VCH_SYS_NO and (tt.AMOUNT_DR=CASE WHEN b.AMOUNT_DR !=0 then 0 else -1 end or tt.AMOUNT_CR=CASE WHEN b.AMOUNT_CR !=0 then 0 else -1 end) order by tt.ID desc)  end  ),'Closing Stock','') AS[Part],a.particulars, serial_no AS vch_manual_no, isnull(b.amount_dr,0) AS Debit_amt,isnull(b.amount_cr,0) AS Credit_amt,0 bal,b.acc_type" +
                            " ,case when isnull(b.amount_dr,0) >0 then isnull(b.amount_dr,0) else isnull(b.amount_cr,0) end AS Amount" +
                            " from gl_trans_mst a ,gl_trans_dtl b where a.vch_sys_no=b.vch_sys_no  and a.book_name='" +
                            book + "' and b.gl_coa_code='" + glCode +
                            "'  and a.value_date BETWEEN convert(datetime,nullif('" + StartDt +
                            "',''),103) AND convert(datetime,nullif('" + EndDt +
                            "',''),103) AND a.[STATUS]='A' ) tot where tot.Amount>0 order by convert(date,tot.VALUE_DATE,103),convert(int,tot.vch_sys_no) asc  ";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        DataSet Ds = new DataSet();
                        adapter.Fill(Ds, "dtLedger");
                        string coaDesc = "";
                        string accTyp = "";
                        if (Ds.Tables[0].Rows.Count > 0)
                        {
                            accTyp = ((DataRow) Ds.Tables[0].Rows[0])["acc_type"].ToString();
                            coaDesc = ((DataRow) Ds.Tables[0].Rows[0])["coa_desc"].ToString();
                        }
                        else
                        {
                            accTyp = VouchManager.getAccType(glCode);
                            coaDesc = glCode;
                        }
                        DataRow dr = Ds.Tables[0].NewRow();
                        dr["vch_sys_no"] = "99999999";
                        dr["acc_type"] = accTyp;
                        dr["v_date"] = DataManager.DateEncode(StartDt);
                        dr["value_date"] = StartDt;
                        dr["coa_desc"] = "";
                        dr["vch_manual_no"] = "";
                        dr["particulars"] = "Opening Balance (REF. dt. " + StartDt + ")";
                        if (accTyp == "A" | accTyp == "E")
                        {
                            dr["debit_amt"] =
                                GlCoaManager.getCoaopeningBalanceFromExistingDataTable(dtVch, book, glCode,
                                    Session["septype"].ToString(), "A", StartDt, EndDt).ToString();
                            
                                dr["debit_amt"] = Convert.ToDouble(dr["debit_amt"]).ToString();
                            
                            dr["credit_amt"] = decimal.Zero;
                        }
                        else if (accTyp == "L" | accTyp == "I")
                        {
                            dr["debit_amt"] = decimal.Zero;
                            dr["credit_amt"] =
                                GlCoaManager.getCoaopeningBalanceFromExistingDataTable(dtVch, book, glCode,
                                    Session["septype"].ToString(), "A", StartDt, EndDt).ToString();
                        }
                        dr["bal"] = decimal.Zero;
                        Ds.Tables[0].Rows.InsertAt(dr, 0);
                        decimal bal = decimal.Zero;
                        foreach (DataRow drLedg in Ds.Tables[0].Rows)
                        {
                            if (drLedg["debit_amt"].ToString() != "")
                            {
                                drLedg.BeginEdit();
                                //if (accTyp == "A" | accTyp == "E")
                                //{
                                bal += decimal.Parse(drLedg["debit_amt"].ToString()) -
                                       decimal.Parse(drLedg["credit_amt"].ToString());
                                //}
                                //else if (accTyp == "L" | accTyp == "I")
                                //{
                                //bal += decimal.Parse(drLedg["credit_amt"].ToString()) - decimal.Parse(drLedg["debit_amt"].ToString());
                                //}
                                drLedg["bal"] = bal;
                                drLedg.AcceptChanges();
                            }
                        }
                        //} 
                        lblTitle.Text = NotesNo.ToUpper();
                        lblDate.Text = "FROM " + StartDt + " TO " + EndDt + "";
                        dgLedger.Visible = true;
                        dgLedger.DataSource = Ds.Tables[0];
                        Session["dtledger"] = Ds.Tables[0];
                        ViewState["dtledgerExcel"] = Ds.Tables[0];
                        dgLedger.DataBind();
                        ShowFooterTotal();
                        ShowFooterTotalWithOpenning();
                    }
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
        if (Session["dtledger"] != null)
        {
            decimal Dramount = decimal.Zero,totDrCurrent=decimal.Zero;
            decimal CrAmounr = decimal.Zero, totCrCurrent = decimal.Zero;
            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
            TableCell cell;
            DataTable dt = (DataTable) Session["dtledger"];
            foreach (DataRow row1 in dt.Rows)
            {
                if (!string.IsNullOrEmpty(row1["Debit_amt"].ToString()))
                {
                    Dramount += Convert.ToDecimal(row1["Debit_amt"]);
                }
                if (!string.IsNullOrEmpty(row1["Credit_amt"].ToString()))
                {
                    CrAmounr += Convert.ToDecimal(row1["Credit_amt"]);  
                }
               
               
                if (!row1["Vch_sys_no"].ToString().Trim().Equals("99999999"))
                {
                    totDrCurrent += Convert.ToDecimal(row1["Debit_amt"]);
                    totCrCurrent += Convert.ToDecimal(row1["Credit_amt"]);
                }
            }
            ViewState["CurrentTotalDr"] = totDrCurrent;
            ViewState["CurrentTotalCr"] = totCrCurrent;
            cell = new TableCell();
            cell.Text = "Current Total";
            cell.ColumnSpan = 7;
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
            cell = new TableCell();
            cell.Text = "";
            row.Cells.Add(cell);
            dgLedger.Controls[0].Controls.Add(row);
        }
    }
    private void ShowFooterTotalDebidOrCradit(string Type)
    {
        if (Session["dtledger"] != null)
        {
            decimal Dramount = decimal.Zero, totDrCurrent = decimal.Zero;
            decimal CrAmounr = decimal.Zero, totCrCurrent = decimal.Zero;
            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
            TableCell cell;
            DataTable dt = (DataTable)Session["dtledger"];
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
            cell = new TableCell();
            cell.Text = "Current Total";
            cell.ColumnSpan = 7;
            row.Cells.Add(cell);
            row.HorizontalAlign = HorizontalAlign.Right;
            row.Font.Bold = true;
            if (Type.Equals("D"))
            {
                cell = new TableCell();
                cell.Text = totDrCurrent.ToString("N3");
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);
                row.Font.Bold = true;
            }
            if (Type.Equals("C"))
            {
                cell = new TableCell();
                cell.Text = totCrCurrent.ToString("N3");
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);
                row.Font.Bold = true;
            }
            dgLedger.Controls[0].Controls.Add(row);
        }
    }

    private void ShowFooterTotalWithOpenning()
    {
        if (Session["dtledger"] != null)
        {
            decimal Dramount = decimal.Zero, totDrCurrent = decimal.Zero;
            decimal CrAmounr = decimal.Zero, totCrCurrent = decimal.Zero;
            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
            TableCell cell;
            DataTable dt = (DataTable) Session["dtledger"];
            foreach (DataRow row1 in dt.Rows)
            {
                if (!string.IsNullOrEmpty(row1["Debit_amt"].ToString()))
                {
                    Dramount += Convert.ToDecimal(row1["Debit_amt"]);
                    
                }
                if (!string.IsNullOrEmpty(row1["Credit_amt"].ToString()))
                {
                    CrAmounr += Convert.ToDecimal(row1["Credit_amt"]);
                }
                
                if (!row1["Vch_sys_no"].ToString().Trim().Equals("99999999"))
                {
                    totDrCurrent += Convert.ToDecimal(row1["Debit_amt"]);
                    totCrCurrent += Convert.ToDecimal(row1["Credit_amt"]);
                }
            }
            ViewState["TotalWithOpeningDr"] = Dramount;
            ViewState["TotalWithOpeningCr"] = CrAmounr;
            if (!string.IsNullOrEmpty(dt.Rows[0]["Debit_amt"].ToString()) && !string.IsNullOrEmpty(dt.Rows[0]["Credit_amt"].ToString()))
            {
                if (!Convert.ToDecimal(dt.Rows[0]["Debit_amt"]).Equals(0) ||
                 !Convert.ToDecimal(dt.Rows[0]["Credit_amt"]).Equals(0))
                {
                    cell = new TableCell();
                    cell.Text = "Total With Opening";
                    cell.ColumnSpan = 7;
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    row.Font.Bold = true;
                    row.Cells.Add(cell);
                    cell = new TableCell();
                    cell.Text = Dramount.ToString("N3");
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    row.Font.Bold = true;
                    row.Cells.Add(cell);
                    cell = new TableCell();
                    cell.Text = CrAmounr.ToString("N3");
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    row.Font.Bold = true;
                    row.Cells.Add(cell);
                    row.Font.Bold = true;
                    cell = new TableCell();
                    cell.Text = "";
                    row.Cells.Add(cell);
                }  
            }

           
            dgLedger.Controls[0].Controls.Add(row);
        }
    }
    private void ShowFooterTotalWithOpenningDebidOrCradit(string Type)
    {
        if (Session["dtledger"] != null)
        {
            decimal Dramount = decimal.Zero, totDrCurrent = decimal.Zero;
            decimal CrAmounr = decimal.Zero, totCrCurrent = decimal.Zero;
            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
            TableCell cell;
            DataTable dt = (DataTable)Session["dtledger"];
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

            if (!Convert.ToDecimal(dt.Rows[0]["Debit_amt"]).Equals(0) ||
                !Convert.ToDecimal(dt.Rows[0]["Credit_amt"]).Equals(0))
            {
                cell = new TableCell();
                cell.Text = "Total With Opening";
                cell.ColumnSpan = 7;
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);
                row.Font.Bold = true;
                if (Type.Equals("D"))
                {
                    cell = new TableCell();
                    cell.Text = Dramount.ToString("N3");
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    row.Cells.Add(cell);
                    row.Font.Bold = true;
                }
                if (Type.Equals("C"))
                {
                    cell = new TableCell();
                    cell.Text = CrAmounr.ToString("N3");
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    row.Cells.Add(cell);
                    row.Font.Bold = true;
                }
                //cell = new TableCell();
                //cell.Text = "";
                //row.Cells.Add(cell);
            }
            dgLedger.Controls[0].Controls.Add(row);
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
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header |
            e.Row.RowType == DataControlRowType.Footer)
        {
            if (rbType.SelectedValue.Equals("D"))
            {
                e.Row.Cells[8].Attributes.Add("style", "display:none");
                e.Row.Cells[9].Attributes.Add("style", "display:none");
            }
            else if (rbType.SelectedValue.Equals("C"))
            {
                e.Row.Cells[7].Attributes.Add("style", "display:none");
                e.Row.Cells[9].Attributes.Add("style", "display:none");
            }
        }
    }

    protected void dgLedger_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        if (Session["dtledger"] != null)
        {
            DataTable dt = (DataTable) Session["dtledger"];
            dgLedger.DataSource = dt;
            dgLedger.PageIndex = e.NewPageIndex;
            dgLedger.DataBind();
            if (rbType.SelectedValue.Equals("A"))
            {
                ShowFooterTotal();
                ShowFooterTotalWithOpenning();
            }
            else
            {
                ShowFooterTotalDebidOrCradit(rbType.SelectedValue);
                ShowFooterTotalWithOpenningDebidOrCradit(rbType.SelectedValue);
            }
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
        string title = "";
        if (RepType == "3" | RepType == "4")
        {
            title = RepType.Replace("3", "Cash book statement").Replace("4", "Bank book statement") + " as on " +
                    StartDt + " TO " + EndDt;

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
        cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(title, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
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
        float[] width = new float[9] {8,15, 15, 20, 35,50, 20, 20, 20};
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
        cell = new PdfPCell(FormatHeaderPhrase("Balance")); //Balance
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

            cell = new PdfPCell(FormatPhrase(dr["vch_sys_no"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["value_date"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["vch_manual_no"].ToString()));
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

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["bal"]).ToString("N3")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtledg.AddCell(cell);

            dtot += decimal.Parse(dr["Debit_amt"].ToString());
            ctot += decimal.Parse(dr["Credit_amt"].ToString());

        }


        cell = new PdfPCell(FormatHeaderPhrase("Current Total"));
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

        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Total With Opening"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.Colspan = 6;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(dtot.ToString("N3")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(ctot.ToString("N3")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtledg.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        pdtledg.AddCell(cell);

        document.Add(pdtledg);
        document.Close();
        Response.Flush();
        Response.End();
    }

    protected void lbExpExcel_Click(object sender, EventArgs e)
    {
        DataTable ddt = (DataTable) ViewState["dtledgerExcel"];
        if (ddt.Rows.Count > 0)
        {
            if (!ddt.Columns[0].ColumnName.Equals("Voucher No."))
            {
                ddt.Columns.Remove("v_date");
                ddt.Columns.Remove("acc_type");
                ddt.Columns.Remove("coa_desc");
                ddt.Columns.Remove("Amount");
                ddt.Columns["Vch_sys_no"].ColumnName = "Voucher No.";
                ddt.Columns["value_date"].ColumnName = "Voucher Date";
                // dtlist.Columns["coa_desc"].ColumnName = "Chart Of Account(COA)";
                ddt.Columns["Part"].ColumnName = "Head Name";
                ddt.Columns["VCH_REF_NO"].ColumnName = "Voucher Ref No.";
                ddt.Columns["particulars"].ColumnName = "Particulars";
                ddt.Columns["vch_manual_no"].ColumnName = "Manual Voucher No.";
                ddt.Columns["Debit_amt"].ColumnName = "Amount(Dr)";
                ddt.Columns["Credit_amt"].ColumnName = "Amount(Cr)";
                ddt.Columns["bal"].ColumnName = "Balance";
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ddt, "LedgerStatement");
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition",
                    "attachment;filename=" + lblTitle.Text.Replace(" ", "") + "-LedgerStatement-(" +
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
        if (rbType.SelectedValue.Equals("A"))
        {
            ShowFooterTotal();
            ShowFooterTotalWithOpenning();
        }
        else
        {
            ShowFooterTotalDebidOrCradit(rbType.SelectedValue);
            ShowFooterTotalWithOpenningDebidOrCradit(rbType.SelectedValue);
        }
    }
}