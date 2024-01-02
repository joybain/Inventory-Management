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

public partial class rptBalSheet : System.Web.UI.Page
{
    private static string book = string.Empty;
    private static string UserType = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        txtStartDt.Attributes.Add("onBlur", "formatdate('" + txtStartDt.ClientID + "')");
        txtEndDt.Attributes.Add("onBlur", "formatdate('" + txtEndDt.ClientID + "')");
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
            string RepLvl = Request.QueryString["replvl"].ToString();
            string SegLvl = Request.QueryString["seglvl"].ToString();
            string VchType = Request.QueryString["vchtyp"].ToString();
            string RptSysId = Request.QueryString["rptsysid"].ToString();
            string NotesNo = Request.QueryString["notes"].ToString();
            hfRepType.Value = RepType;
            //RepType = RepType.Substring(0, 1).Trim();
            if (RepType == "B" | RepType == "I" | RepType == "C" | RepType == "B1")
            {
                lbExp1.Visible = true;
                lbExpNodeExcel.Visible = true;
            }
            else
            {
                lbExp1.Visible = lbExpNodeExcel.Visible = false;
            }

            DataTable dtVch = VouchManager.GetVouchDtlForTotal(EndDt, "A", book, Session["UserType"].ToString());
            string connectionString = DataManager.OraConnString();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            lblTranStatus.Text = "";
            lblDate.Text = "";
            lblTitle.Text = "";
            if (EndDt == "" | DataManager.DateEncode(EndDt).CompareTo(System.DateTime.Now) > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    "alert('Please select the Upto Date or Upto Date cannot be greater than System Date!!');", true);
            }
            else
            {
                if (RepType.Equals("B"))
                {
                    lblDate.Text = " AS ON " + EndDt;
                    lblTitle.Text = "Balance Sheet";
                    Page.Title = "Balance Sheet.-SDL";
                }
                else if (RepType.Equals("I"))
                {
                    lblDate.Text = " For the Period " + StartDt + " TO " + EndDt;
                    lblTitle.Text = "Income Statement";
                    Page.Title = "Income Statement.-SDL";
                }
                else if (RepType.Equals("B1"))
                {
                    lblDate.Text = " For the Period " + StartDt + " TO " + EndDt;
                    lblTitle.Text = "Income Statement from shopping center(Rey De Aves)";
                    Page.Title = "Income Statement for Shopping Center.-SDL";
                }
                else if (RepType.Equals("B2"))
                {
                    lblDate.Text = " For the Period " + StartDt + " TO " + EndDt;
                    lblTitle.Text = "Income Statement from shopping center(LP-2)";
                    Page.Title = "Income Statement for Shopping Center.-SDL";
                }
                else if (RepType.Equals("C"))
                {
                    lblDate.Text = " AS ON " + EndDt;
                    lblTitle.Text = "Cash Flow Statement";
                    Page.Title = "Cash Flow.-SDL";
                }
                else if (RepType.Equals("7"))
                {
                    lblDate.Text = " For the Period " + StartDt + " TO " + EndDt;
                    lblTitle.Text = "Trial Balance";
                    Page.Title = "Trial Balance.-SDL";
                }

                // lblDate.Text = RepType.Replace("B", " AS ON " + EndDt).Replace("I", ).Replace("C", ).Replace("7", " For the Period " + StartDt + " TO " + EndDt).ToUpper();
                // lblTitle.Text = RepType.Replace("B", "").Replace("I", "").Replace("C", "").Replace("7", "Trial Balance").ToUpper();
                if (RepType == "B" | RepType == "I" | RepType == "C" | RepType == "B1" | RepType == "B2")
                {
                    string verno = string.Empty;
                    SqlDataReader dReader;
                    SqlCommand cmd = new SqlCommand();
                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select ver_no from gl_st_map_mst where type_code='" + RepType + "'";
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    dReader = cmd.ExecuteReader();
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            verno = dReader["ver_no"].ToString();
                        }
                    }
                    dReader.Dispose();
                    cmd.Dispose();
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        SqlConnection.ClearPool(conn);
                    }
                    if (verno == "")
                    {
                        //lblTranStatus.Visible = true;
                        //lblTranStatus.ForeColor = System.Drawing.Color.Red;
                        //lblTranStatus.Text = "Please Prepare The Report Settings First in Dynamic Report Settings Form";
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('Please prepare the Report Settings first in Dynamic Report Settings form!!');", true);
                        return;
                    }
                    DataSet Ds = new DataSet();
                    DataTable dtRptBalSheet = Ds.Tables.Add("dtRptBalSheet");
                    dtRptBalSheet.Columns.Add("gl_seg_code", typeof (string));
                    dtRptBalSheet.Columns.Add("coa_desc", typeof (string));
                    dtRptBalSheet.Columns.Add("notes", typeof (string));
                    dtRptBalSheet.Columns.Add("amount_a", typeof (decimal));
                    dtRptBalSheet.Columns.Add("amount_b", typeof (decimal));
                    dtRptBalSheet.Columns.Add("amount_a_dt", typeof (string));
                    dtRptBalSheet.Columns.Add("amount_b_dt", typeof (string));
                    dtRptBalSheet.Columns.Add("type_code", typeof (string));
                    dtRptBalSheet.Columns.Add("ver_no", typeof (string));
                    DataTable dtBalNotes = Ds.Tables.Add("dtBalNotes");
                    dtBalNotes.Columns.Add("notes", typeof (string));
                    dtBalNotes.Columns.Add("gl_seg_code", typeof (string));
                    dtBalNotes.Columns.Add("coa_desc", typeof (string));
                    dtBalNotes.Columns.Add("parent_notes", typeof (string));
                    dtBalNotes.Columns.Add("amount_a", typeof (decimal));
                    dtBalNotes.Columns.Add("amount_b", typeof (decimal));
                    dtBalNotes.Columns.Add("amount_a_dt", typeof (string));
                    dtBalNotes.Columns.Add("amount_b_dt", typeof (string));
                    dtBalNotes.Columns.Add("type_code", typeof (string));
                    dtBalNotes.Columns.Add("rpt_sys_id", typeof (string));
                    dtBalNotes.Columns.Add("col_no", typeof (string));
                    if (NotesNo == "" | NotesNo == "ALL")
                    {
                        decimal OpeningBlance = 0;
                        if (RepType == "B")
                        {
                            Ds = IncBalManager.generateBal(dtVch, book, RepType, verno, glCode, EndDt, 1, null);
                        }
                        else if (RepType == "I")
                        {
                            Ds = IncBalManager.generateBal(dtVch, book, RepType, verno, glCode, EndDt, 1, StartDt);

                        }
                        else if (RepType == "B1")
                        {
                            Ds = IncBalManager.generateShoppingCenterIncome(dtVch, book, RepType, verno, glCode, EndDt,
                                1, StartDt);

                        }
                        else if (RepType == "B2")
                        {
                            Ds = IncBalManager.generateShoppingCenterIncome(dtVch, book, RepType, verno, glCode, EndDt,
                                1, StartDt);

                        }
                        else if (RepType == "C")
                        {
                            Ds = IncBalManager.generateCashFlow(dtVch, book, RepType, Session["septype"].ToString(),
                                verno, glCode, StartDt, EndDt, 1);
                        }
                        foreach (DataRow dr in Ds.Tables[0].Rows)
                        {

                            DataRow drBal = dtRptBalSheet.NewRow();
                            drBal["gl_seg_code"] = dr["gl_seg_code"].ToString();
                            drBal["coa_desc"] = dr["coa_desc"].ToString();
                            if (string.IsNullOrEmpty(dr["notes"].ToString()))
                            {
                                drBal["notes"] = dr["notes"].ToString();
                            }
                            else
                            {
                                drBal["notes"] = Convert.ToInt32(dr["notes"]).ToString("N0");
                            }

                            //if (RepType.Equals("I") && dr["coa_desc"].ToString().Trim().Equals("Net Purchase"))
                            //{
                            //    OpeningBlance = decimal.Parse(dr["amount_a"].ToString());
                            //    drBal["amount_a"] = decimal.Parse(dr["amount_a"].ToString());
                            //}
                            //else if (RepType.Equals("I") && dr["coa_desc"].ToString().Trim().Equals("Closing Stock"))
                            //{
                            //    drBal["amount_a"] = OpeningBlance;
                            //}
                            //else
                            //{
                            drBal["amount_a"] = decimal.Parse(dr["amount_a"].ToString());
                            //}
                            drBal["amount_b"] = decimal.Parse(dr["amount_b"].ToString());
                            drBal["amount_a_dt"] = dr["amount_a_dt"].ToString();
                            drBal["amount_b_dt"] = dr["amount_b_dt"].ToString();
                            drBal["type_code"] = RepType;
                            drBal["ver_no"] = verno;
                            dtRptBalSheet.Rows.Add(drBal);
                        }
                        foreach (DataRow drN in Ds.Tables[1].Rows)
                        {
                            DataRow drNotes = dtBalNotes.NewRow();
                            drNotes["notes"] = drN["notes"].ToString();
                            drNotes["gl_seg_code"] = drN["gl_seg_code"].ToString();
                            drNotes["coa_desc"] = drN["coa_desc"].ToString();
                            drNotes["parent_notes"] = drN["parent_notes"].ToString();
                            drNotes["amount_a"] = decimal.Parse(drN["amount_a"].ToString());
                            drNotes["amount_b"] = decimal.Parse(drN["amount_b"].ToString());
                            drNotes["amount_a_dt"] = drN["amount_a_dt"].ToString();
                            drNotes["amount_b_dt"] = drN["amount_b_dt"].ToString();
                            drNotes["type_code"] = RepType;
                            drNotes["rpt_sys_id"] = drN["rpt_sys_id"].ToString();
                            drNotes["col_no"] = drN["col_no"].ToString();
                            dtBalNotes.Rows.Add(drNotes);
                        }
                        if (dtRptBalSheet.Rows.Count > 0)
                        {
                            dgBal.Columns[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgBal.Columns[0].HeaderText = "Acc. Code";
                            dgBal.Columns[1].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgBal.Columns[1].HeaderText = "Particulars";
                            dgBal.Columns[2].ShowHeader = true;
                            dgBal.Columns[2].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgBal.Columns[2].HeaderText = "Notes";
                            dgBal.Columns[3].ShowHeader = true;
                            dgBal.Columns[3].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgBal.Columns[3].HeaderText = "Amount";
                            dgBal.Columns[4].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgBal.Columns[4].ShowHeader = true;
                            dgBal.Columns[4].HeaderText = ((DataRow) dtRptBalSheet.Rows[0])[6].ToString();

                            dgBalNotes.Columns[0].ShowHeader = true;
                            dgBalNotes.Columns[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgBalNotes.Columns[0].HeaderText = "Notes";
                            dgBalNotes.Columns[3].ShowHeader = true;
                            dgBalNotes.Columns[3].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgBalNotes.Columns[3].HeaderText = "Parent Notes";
                            dgBalNotes.Columns[4].ShowHeader = true;
                            dgBalNotes.Columns[4].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgBalNotes.Columns[4].HeaderText = ((DataRow) dtRptBalSheet.Rows[0])[5].ToString();
                            dgBalNotes.Columns[5].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                            dgBalNotes.Columns[5].ShowHeader = true;
                            dgBalNotes.Columns[5].HeaderText = ((DataRow) dtRptBalSheet.Rows[0])[6].ToString();
                            Session["dtrptbalsheet"] = dtRptBalSheet;
                            ViewState["dtrptbalsheetExcel"] = dtRptBalSheet;
                            Session["dtbalnotesall"] = dtBalNotes;
                            ViewState["dtbalnotesallExcel"] = dtBalNotes;
                        }
                        if (NotesNo == "")
                        {
                            dgBal.Visible = true;
                            dgBalNotes.Visible = false;
                            dgBal.DataSource = dtRptBalSheet;
                            dgBal.DataBind();
                            if (RepType == "C")
                            {
                                dgBal.Columns[4].Visible = false;
                            }
                            else
                            {
                                dgBal.Columns[4].Visible = true;
                            }
                        }
                        else if (NotesNo == "All")
                        {
                            dgBal.Visible = false;
                            dgBalNotes.Visible = true;
                            dgAccountBal.Visible = false;
                            dgBalNotes.DataSource = dtBalNotes;
                            dgBalNotes.DataBind();
                        }
                    }
                    if (NotesNo != "")
                    {
                        dtBalNotes = (DataTable) Session["dtbalnotesall"];
                        DataTable dt = dtBalNotes.Clone();
                        foreach (DataRow drt in dtBalNotes.Rows)
                        {
                            if (drt["notes"].ToString() != "" && drt["parent_notes"].ToString() == "")
                            {
                                if (decimal.Parse(drt["notes"].ToString()) >= decimal.Parse(NotesNo) &&
                                    decimal.Parse(drt["notes"].ToString()) <
                                    decimal.Parse((decimal.Parse(NotesNo) + decimal.One).ToString()))
                                {
                                    dt.ImportRow(drt);
                                }
                            }
                                //else if (drt["notes"].ToString() != "" && drt["parent_notes"].ToString() != "")
                                //{
                                //    if (decimal.Parse(drt["notes"].ToString()) >= decimal.Parse(NotesNo) && decimal.Parse(drt["notes"].ToString()) < decimal.Parse((decimal.Parse(NotesNo) + decimal.One).ToString()))
                                //    {
                                //        dt.ImportRow(drt);
                                //    }
                                //}
                            else if (drt["notes"].ToString() == "" && drt["parent_notes"].ToString() != "")
                            {
                                if (decimal.Parse(drt["parent_notes"].ToString()) >= decimal.Parse(NotesNo) &&
                                    decimal.Parse(drt["parent_notes"].ToString()) <
                                    decimal.Parse((decimal.Parse(NotesNo) + decimal.One).ToString()))
                                {
                                    dt.ImportRow(drt);
                                }
                            }
                        }
                        //lblAddress1.Text = Math.Round(decimal.Parse(NotesNo), 2).ToString() +"-"+dt.Rows.Count.ToString();
                        dgBal.Visible = false;
                        dgBalNotes.Visible = true;
                        dgAccountBal.Visible = false;
                        dgBalNotes.DataSource = dt;
                        Session["dtbalnotes"] = dt;
                        dgBalNotes.DataBind();
                    }
                }
                else if (RepType == "7")
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
                                       " SELECT s.SEG_COA_CODE,s.SEG_COA_DESC,'0' p_db_amt,'0' p_cr_amt, '0' u_db_amt,'0' u_cr_amt,s.acc_type FROM  gl_seg_coa s where seg_coa_code in (" +
                                       " select seg_coa_code from t where ((LEVELs = " + RepLvl + ") OR (LEVELs < " +
                                       RepLvl + " AND  ROOTLEAF = 'L'))) order by 1";
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
                            //lblTitle.Text = "TRIAL BALANCE";
                            //lblDate.Text= "As on " + EndDt;
                            /* this code will generate gl_coa_code */
                            string mainseg = "";
                            foreach (DataRow dr in sortedDate.Rows)
                            {
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
                                dr.BeginEdit();
                                if (dr["acc_type"].ToString().Equals("A") || dr["acc_type"].ToString().Equals("E"))
                                {
                                    dr["p_db_amt"] =
                                        GlCoaManager.getCoaBalanceFromExistingDataTable(dtVch, book, segcoacode,
                                            Session["septype"].ToString(), "A", StartDt, EndDt, "", 7).ToString("N3");
                                    dr["u_db_amt"] =
                                        GlCoaManager.getCoaBalanceFromExistingDataTable(dtVch, book, segcoacode,
                                            Session["septype"].ToString(), "A", "", EndDt, "", 7).ToString("N3");
                                    //dr["p_cr_amt"] = "";
                                    //dr["u_cr_amt"] = "";
                                }
                                else
                                {
                                    //dr["p_db_amt"] = "";
                                    //dr["u_db_amt"] = "";
                                    dr["p_cr_amt"] =
                                        GlCoaManager.getCoaBalanceFromExistingDataTable(dtVch, book, segcoacode,
                                            Session["septype"].ToString(), "A", StartDt, EndDt, "", 7).ToString("N3");
                                    dr["u_cr_amt"] =
                                        GlCoaManager.getCoaBalanceFromExistingDataTable(dtVch, book, segcoacode,
                                            Session["septype"].ToString(), "A", "", EndDt, "", 7).ToString("N3");
                                }
                                dr.AcceptChanges();
                            }
                            decimal p_db_amt = decimal.Zero;
                            decimal u_db_amt = decimal.Zero;
                            decimal p_cr_amt = decimal.Zero;
                            decimal u_cr_amt = decimal.Zero;
                            foreach (DataRow dr1 in sortedDate.Rows)
                            {
                                if (!string.IsNullOrEmpty(dr1["p_db_amt"].ToString()))
                                {
                                    p_db_amt += decimal.Parse(dr1["p_db_amt"].ToString());
                                }
                                if (!string.IsNullOrEmpty(dr1["u_db_amt"].ToString()))
                                {
                                    u_db_amt += decimal.Parse(dr1["u_db_amt"].ToString());
                                }
                                if (!string.IsNullOrEmpty(dr1["p_cr_amt"].ToString()))
                                {
                                    p_cr_amt += decimal.Parse(dr1["p_cr_amt"].ToString());
                                }
                                if (!string.IsNullOrEmpty(dr1["u_cr_amt"].ToString()))
                                {
                                    u_cr_amt += decimal.Parse(dr1["u_cr_amt"].ToString());
                                }
                            }
                            DataRow dr2 = sortedDate.NewRow();
                            dr2["seg_coa_desc"] = "Total";
                            dr2["p_db_amt"] = p_db_amt.ToString("N3");
                            dr2["u_db_amt"] = u_db_amt.ToString("N3");
                            dr2["p_cr_amt"] = p_cr_amt.ToString("N3");
                            dr2["u_cr_amt"] = u_cr_amt.ToString("N3");
                            sortedDate.Rows.Add(dr2);
                            string typeSeg = SegCoaManager.getMainSeg(mainseg);
                            Session["typeseg"] = typeSeg;
                            if (typeSeg == "N")
                            {
                                dgAccountBal.Columns[1].ShowHeader = true;
                                dgAccountBal.Columns[1].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                                dgAccountBal.Columns[1].HeaderText = "Head Of Account";
                                dgAccountBal.Columns[2].ShowHeader = true;
                                dgAccountBal.Columns[2].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                                dgAccountBal.Columns[2].HeaderText = "Current Period Debit Amount";
                                dgAccountBal.Columns[3].ShowHeader = true;
                                dgAccountBal.Columns[3].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                                dgAccountBal.Columns[3].HeaderText = "Current Period Credit Amount";
                                dgAccountBal.Columns[4].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                                dgAccountBal.Columns[4].ShowHeader = true;
                                dgAccountBal.Columns[4].HeaderText = "Cumulative Debit Amount";
                                dgAccountBal.Columns[5].ShowHeader = true;
                                dgAccountBal.Columns[5].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                                dgAccountBal.Columns[5].HeaderText = "Cumulative Credit Amount";
                            }
                            else
                            {
                                dgAccountBal.Columns[1].ShowHeader = true;
                                dgAccountBal.Columns[1].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                                dgAccountBal.Columns[1].HeaderText = "Head Of Account";
                                dgAccountBal.Columns[3].ShowHeader = true;
                                dgAccountBal.Columns[3].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                                dgAccountBal.Columns[3].HeaderText = "Current Period Balance";
                                dgAccountBal.Columns[5].ShowHeader = true;
                                dgAccountBal.Columns[5].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                                dgAccountBal.Columns[5].HeaderText = "Cumulative Balance";
                                dgAccountBal.Columns[2].Visible = false;
                                dgAccountBal.Columns[4].Visible = false;
                            }
                            dgBal.Visible = false;
                            dgBalNotes.Visible = false;
                            dgAccountBal.Visible = true;
                            dgAccountBal.DataSource = sortedDate;
                            Session["dtaccountbal"] = sortedDate;
                            ViewState["dtaccountbalExcel"] = sortedDate;
                            dgAccountBal.DataBind();
                        }
                    }
            }
        }
    }

    protected void dgBal_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string eventHandler="";
        e.Row.Cells[4].Attributes.Add("style", "display:none");
        e.Row.Cells[0].Attributes.Add("style", "display:none");
        
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            
            if (e.Row.Cells[2].Text.Trim().Replace("&nbsp;", "") != "")
            {
                string dataFieldValue = e.Row.Cells[2].Text.Trim().Replace("&nbsp;", "");
                e.Row.Cells[2].Text = decimal.Parse(e.Row.Cells[2].Text).ToString("N0");
                //e.Row.Cells[2].Style.Add("cursor", "hand");
                e.Row.Cells[2].Font.Underline = true;
                e.Row.Cells[2].ForeColor = System.Drawing.Color.Blue;
                eventHandler = string.Format(System.Globalization.CultureInfo.InvariantCulture, "javascript:getNewUrl({0});", dataFieldValue);
                e.Row.Cells[2].Attributes.Add("onclick", eventHandler);
            }
            else
            {
                //e.Row.Cells[4].ForeColor = Color.CornflowerBlue;
                //e.Row.Cells[4].Attributes.Add("style", "font-size: 16px");

               //e.Row.Cells[2].Font.Underline = false;
                //e.Row.Cells[2].Style.Add("cursor", "normal");
            }
            if (string.IsNullOrEmpty(e.Row.Cells[0].Text.Replace("&nbsp;","").Replace(" ", "")))
            {
                e.Row.Cells[2].ForeColor = Color.Maroon;
                e.Row.Cells[0].ForeColor = Color.Maroon;
                e.Row.Cells[1].ForeColor = Color.Maroon;
                e.Row.Cells[3].ForeColor = Color.Maroon;

                //e.Row.Cells[4].ForeColor = Color.CornflowerBlue;

                //e.Row.Cells[2].Attributes.Add("style", "font-size: 15px");
                e.Row.Cells[2].Attributes.Add("style", "font-size: 15px");
                e.Row.Cells[1].Attributes.Add("style", "font-size: 15px");
                e.Row.Cells[3].Attributes.Add("style", "font-size: 15px");
            }
            if (e.Row.Cells[1].Text.Trim().Replace("&nbsp;", "").Length > 0)
            {
                if (e.Row.Cells[1].Text.Trim().Replace("&nbsp;", "").Substring(e.Row.Cells[1].Text.Trim().Replace("&nbsp;", "").Length - 3).Contains(':') == true)
                {
                    e.Row.Cells[3].Text = "";
                    e.Row.Cells[4].Text = "";
                }
            }
            else
            {
                e.Row.Cells[3].Text = "";
                e.Row.Cells[4].Text = "";
            }
        }
        e.Row.Cells[2].Attributes.Add("style", "font-size: 12px");
        e.Row.Cells[2].Style["cursor"] = "pointer";
    }
    protected void dgBalNotes_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        e.Row.Cells[3].Attributes.Add("style", "display:none");
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[0].Text.Trim().Replace("&nbsp;", "") != "")
            {
                e.Row.Cells[0].Text = decimal.Parse(e.Row.Cells[0].Text).ToString("N3");
                e.Row.Font.Bold = true;
            }
            if (e.Row.Cells[3].Text.Trim().Replace("&nbsp;", "") != "")
            {
                e.Row.Cells[3].Text = decimal.Parse(e.Row.Cells[3].Text).ToString("N3");
            }

            if (e.Row.Cells[0].Text.Trim().Replace("&nbsp;", "") != "" && e.Row.Cells[3].Text.Trim().Replace("&nbsp;", "") != "")
            {
                e.Row.Visible = false;
            }
        }
    }

    //************* Trail Blance ************//

    protected void dgAccountBal_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[4].Attributes.Add("style", "display:none");
            e.Row.Cells[5].Attributes.Add("style", "display:none");
        }
        //if (e.Row.RowType == DataControlRowType.Header)
        //{
        //    e.Row.Cells[4].Attributes.Add("style", "display:none");
        //    e.Row.Cells[5].Attributes.Add("style", "display:none");
        //}
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
        if (RepType == "B" | RepType == "I" | RepType == "C" | RepType == "B1" | RepType == "B2")
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
    protected void lbExp1_Click(object sender, EventArgs e)
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
        //if (NotesNo == "")
        //{
        //    title = RepType.Replace("B", "Balanace Sheet Statement").Replace("I", "Income Statement").Replace("C", "Cash Flow Statement") + " as on " + EndDt;
        //}
        //else
        //{
        //    title = RepType.Replace("B", "Balanace Sheet Statement Notes").Replace("I", "Income Statement Notes").Replace("C", "Cash Flow Statement Notes") + " as on " + EndDt;
        //}

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
        cell = new PdfPCell(new Phrase(lblTitle.Text+" "+lblDate.Text, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
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
        float[] widthbis = new float[4] { 10, 60, 8, 30 };
        float[] widthbi = new float[5] { 10, 60, 8, 30, 30 };
        PdfPTable pdtbi;
        int colno = 0;
        if (bdt == "")
        {
            pdtbi = new PdfPTable(widthbis);
            colno = dtbi.Columns.Count - 5;

        }
        else
        {
          //  pdtbi = new PdfPTable(widthbi);
           // colno = dtbi.Columns.Count - 4;
            pdtbi = new PdfPTable(widthbis);
            colno = dtbi.Columns.Count - 5;
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

        document.NewPage();
        writer.ResetPageCount();
        title = RepType.Replace("B", "Balanace Sheet Statement Notes").Replace("I", "Income Statement Notes").Replace("C", "Cash Flow Statement Notes") + " as on " + EndDt;
        PdfPTable dthn = new PdfPTable(titwidth);
        dthn.WidthPercentage = 100;
        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dthn.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dthn.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dthn.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dthn.AddCell(cell);
        cell = new PdfPCell(new Phrase(title, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dthn.AddCell(cell);
        document.Add(dthn);
        document.Add(line);
        document.Add(dtempty);

        DataTable dtbin = (DataTable)Session["dtbalnotesall"];
        //string adt = "";
        //string bdt = "";
        //foreach (DataRow dr in dtbin.Rows)
        //{
        //    if (dr["amount_a_dt"].ToString() != "")
        //    {
        //        adt = dr["amount_a_dt"].ToString();
        //        break;
        //    }
        //}
        //foreach (DataRow dr in dtbin.Rows)
        //{
        //    if (dr["amount_b_dt"].ToString() != "")
        //    {
        //        bdt = dr["amount_b_dt"].ToString();
        //        break;
        //    }
        //}
        float[] widthbisn = new float[5] { 10, 10, 60, 8, 30 };
        float[] widthbin = new float[6] { 10, 10, 60, 8, 30, 30 };
        PdfPTable pdtbin;
        int ncolno = 0;
        if (bdt == "")
        {
            pdtbin = new PdfPTable(widthbisn);
            ncolno = dtbin.Columns.Count - 6;
        }
        else
        {
            pdtbin = new PdfPTable(widthbin);
            ncolno = dtbin.Columns.Count - 5;
        }
        pdtbin.WidthPercentage = 100;
        cell = new PdfPCell(FormatHeaderPhrase("Notes"));
        cell.FixedHeight = 15f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtbin.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(""));
        cell.FixedHeight = 15f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtbin.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Particulars"));
        cell.FixedHeight = 15f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtbin.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Parent Notes"));
        cell.FixedHeight = 15f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtbin.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Current Amount"));
        cell.HorizontalAlignment = 1;
        cell.FixedHeight = 15f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtbin.AddCell(cell);
        if (bdt != "")
        {
            cell = new PdfPCell(FormatHeaderPhrase("Previous Amount"));
            cell.HorizontalAlignment = 1;
            cell.FixedHeight = 15f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtbin.AddCell(cell);
        }

        for (int i = 0; i < dtbin.Rows.Count; i++)
        {
            for (int j = 0; j < ncolno; j++)
            {
                string val = "";
                if (j == 0 | j==3 )
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

       // Response.Write("window.open('rptLedgerStat.aspx?reptype=5&glcoa=" + lblCoaCode.Text +" ','_blank')");
        if (hfRootLeft.Value.ToString().ToUpper().Equals("L"))
        {
            Response.Write("window.open('rptLedgerStat.aspx?reptype=5&glcoa=" + lblCoaCode.Text +
                           "&replvl=&seglvl=&vchtyp=&startdt=" + txtStartDt.Text + "&enddt=" + txtEndDt.Text +
                           "&rptsysid=&notes=" + lblCoaName.Text.Replace("&", "And") + "','_blank')");
        }
        else
        {
            Response.Write("window.open('NewTrialBlance.aspx?reptype=TB&glcoa=" + lblCoaCode.Text +
                           "&replvl=&seglvl=&vchtyp=&startdt=" + txtStartDt.Text + "&enddt=" + txtEndDt.Text +
                           "&rptsysid=&notes=" + lblCoaName.Text.Replace("&", "And") + "','_blank')");
        }
        Response.Write("</script>");
    }
    protected void dgAccountBal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "View")
        {
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            gvr.Cells[2].BackColor =
                gvr.Cells[3].BackColor = gvr.Cells[0].BackColor = gvr.Cells[1].BackColor = Color.Bisque;
            hfRootLeft.Value = IdManager.GetShowSingleValueString("[ROOTLEAF]", "[SEG_COA_CODE]", "[GL_SEG_COA]",
                gvr.Cells[0].Text);
            lblCoaCode.Text = "1-" + gvr.Cells[0].Text;
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

    private DataTable dtTable = null;
    protected void lbExpExcel_Click(object sender, EventArgs e)
    {

        if (hfRepType.Value.Equals("7"))
        {
            DataTable dtList = (DataTable)ViewState["dtaccountbalExcel"];
            if (dtList.Rows.Count > 0)
            {
                dtList.Columns.Remove("u_db_amt");
                dtList.Columns.Remove("u_cr_amt");
                dtList.Columns.Remove("acc_type");
                //dtTable.Columns.Remove("amount_b_dt");
                //dtTable.Columns.Remove("type_code");
                //dtTable.Columns.Remove("ver_no");
                dtList.Columns["SEG_COA_CODE"].ColumnName = "Account Code";
                dtList.Columns["SEG_COA_DESC"].ColumnName = "Head Of Account";
                dtList.Columns["p_db_amt"].ColumnName = "Current Period Debit Amount";
                dtList.Columns["p_cr_amt"].ColumnName = "Current Period Credit Amount";
                //dtList.Columns["particulars"].ColumnName = "Particulars";
                //dtList.Columns["vch_manual_no"].ColumnName = "Manual Voucher No.";
                //dtList.Columns["Debit_amt"].ColumnName = "Amount(Dr)";
                //dtList.Columns["Credit_amt"].ColumnName = "Amount(Cr)";
                //dtList.Columns["bal"].ColumnName = "Balance";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtList, lblTitle.Text.Replace(" ", ""));
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition",
                        "attachment;filename=" + lblTitle.Text.Replace(" ", "") + "-(" +
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
        else
        {
            DataTable dtList = (DataTable) ViewState["dtrptbalsheetExcel"];
            dtTable = dtList;
            if (dtList.Rows.Count > 0)
            {
                dtTable.Columns.Remove("gl_seg_code");
                dtTable.Columns.Remove("amount_b");
                dtTable.Columns.Remove("amount_a_dt");
                dtTable.Columns.Remove("amount_b_dt");
                dtTable.Columns.Remove("type_code");
                dtTable.Columns.Remove("ver_no");
                dtTable.Columns["coa_desc"].ColumnName = "Particulars";
                dtTable.Columns["amount_a"].ColumnName = "Amount";
                dtTable.Columns["notes"].ColumnName = "Notes";
                //dtList.Columns["VCH_REF_NO"].ColumnName = "Voucher Ref No.";
                //dtList.Columns["particulars"].ColumnName = "Particulars";
                //dtList.Columns["vch_manual_no"].ColumnName = "Manual Voucher No.";
                //dtList.Columns["Debit_amt"].ColumnName = "Amount(Dr)";
                //dtList.Columns["Credit_amt"].ColumnName = "Amount(Cr)";
                //dtList.Columns["bal"].ColumnName = "Balance";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtList, lblTitle.Text.Replace(" ", ""));
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition",
                        "attachment;filename=" + lblTitle.Text.Replace(" ", "") + "-(" +
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
        //  DataTable dtbin = (DataTable)Session["dtbalnotesall"];
    }
    protected void lbExpNodeExcel_Click(object sender, EventArgs e)
    {
        DataTable dtList = (DataTable)ViewState["dtbalnotesallExcel"];
        
        if (dtList.Rows.Count > 0)
        {
            dtList.Columns.Remove("col_no");
            dtList.Columns.Remove("rpt_sys_id");
            dtList.Columns.Remove("type_code");
            dtList.Columns.Remove("amount_b_dt");
            dtList.Columns.Remove("amount_a_dt");
            dtList.Columns["notes"].ColumnName = "Notes";
            dtList.Columns["gl_seg_code"].ColumnName = "Account Code";
            dtList.Columns["coa_desc"].ColumnName = "Particulars";
            dtList.Columns["parent_notes"].ColumnName = "Paren";
            dtList.Columns["amount_a"].ColumnName = "Current Amount ";
            dtList.Columns["amount_b"].ColumnName = "Previous Amount";
          
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dtList, lblTitle.Text.Replace(" ", ""));
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition",
                    "attachment;filename=" + lblTitle.Text.Replace(" ", "") + "-(" +
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
}