using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;
using System.Data;
using System.Data.SqlClient;

public partial class rpt_Due_List : System.Web.UI.Page
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
                    lblTitle.Text = RepType.Replace("3", "Cash book statement").Replace("4", "Bank book statement") + " as on " + EndDt;
                    string criteria = GlCoaManager.getNEWCoaCriteria(glCode, Session["septype"].ToString(), book);
                    DataTable dtAlready = GlCoaManager.GetGlCoaCodes(criteria);
                    string glcoacode = "";
                    // string glcoacode1 = "";
                    //decimal OpeningBalance = decimal.Zero;
                    DataTable dt = new DataTable();
                    dt.Columns.Add("gl_coa_code");
                    dt.Columns.Add("coa_desc");
                    dt.Columns.Add("Openin");
                    dt.Columns.Add("Period_amount");
                    dt.Columns.Add("Closing");
                    int i = 0;
                    foreach (DataRow drA in dtAlready.Rows)
                    {
                        glcoacode = drA["gl_coa_code"].ToString();


                        string coaexist = GlCoaManager.getCoaExistence(glcoacode);
                        if (coaexist != "Y")
                        {
                            lblTranStatus.Visible = true;
                            lblTranStatus.Text = "Please select any leaf/child segment code.";
                            lblTranStatus.ForeColor = System.Drawing.Color.Red;
                            return;
                        }
                        //string query = "select a.vch_sys_no AS Vch_sys_no, a.value_date v_date, convert(varchar,a.value_date,103) AS value_date, " +
                        //" b.gl_coa_code+' : '+(select coa_desc from gl_coa where gl_coa_code=b.gl_coa_code) as coa_desc,a.VCH_REF_NO,a.particulars, " +
                        //" (select dbo.initcap(vch_desc) from gl_voucher_type where vch_code=a.vch_code)+' linebreak '+a.vch_ref_no+' linebreak '+ " +
                        //" 'Ref# '+ref_file_no+', Vol# '+volume_no+', Sl#'+serial_no AS vch_manual_no, " +
                        //" coalesce(b.amount_dr,0) AS Debit_amt,coalesce(b.amount_cr,0) AS Credit_amt,0 bal,b.acc_type " +
                        //" from gl_trans_mst a ,gl_trans_dtl b where a.vch_sys_no=b.vch_sys_no  and a.book_name='" + book + "' " +
                        //" and b.gl_coa_code='" + glcoacode + "' " +
                        //" and a.value_date BETWEEN convert(datetime,nullif('" + StartDt + "',''),103) AND convert(datetime,nullif('" + EndDt + "',''),103) " +
                        //" order by vch_sys_no asc";
                        /*************** Purpose for User wise Currency show *****************/
                        string query = @"select a.vch_sys_no AS Vch_sys_no, a.value_date v_date, convert(varchar,a.value_date,103) AS value_date, b.gl_coa_code+' : '+(select coa_desc from gl_coa where gl_coa_code=b.gl_coa_code) as coa_desc,a.VCH_REF_NO,a.particulars,(select dbo.initcap(vch_desc) from gl_voucher_type where vch_code=a.vch_code)+' linebreak '+a.vch_ref_no+' linebreak '+ 'Ref# '+ref_file_no+', Vol# '+volume_no+', Sl#'+serial_no AS vch_manual_no, case when '" + Session["UserType"].ToString() + "'=1 then b.Amount_DR_BD when '" + Session["UserType"].ToString() + "'=2 then b.Amount_DR_PH else isnull(b.amount_dr,0) end as Debit_amt,case when '" + Session["UserType"].ToString() + "'=1 then b.Amount_CR_BD when '" + Session["UserType"].ToString() + "'=2 then b.Amount_CR_PH else isnull(b.amount_cr,0) end AS Credit_amt,0 bal,b.acc_type from gl_trans_mst a ,gl_trans_dtl b where a.vch_sys_no=b.vch_sys_no  and a.book_name='" + book + "' and b.gl_coa_code='" + glcoacode + "' and a.value_date BETWEEN convert(datetime,nullif('" + StartDt + "',''),103) AND convert(datetime,nullif('" + EndDt + "',''),103) order by vch_sys_no asc";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        DataSet Ds = new DataSet();
                        adapter.Fill(Ds, "dtLedger");
                        string coaDesc = "";
                        string accTyp = "";
                                           
                        if (Ds.Tables[0].Rows.Count > 0)
                        {
                            accTyp = ((DataRow)Ds.Tables[0].Rows[0])["acc_type"].ToString();
                            coaDesc = ((DataRow)Ds.Tables[0].Rows[0])["coa_desc"].ToString();
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
                            dr["debit_amt"] = GlCoaManager.getCoaopeningBalanceFromExistingDataTable(dtVch, book, glcoacode, Session["septype"].ToString(), "A", StartDt, EndDt).ToString();
                            dr["credit_amt"] = decimal.Zero;
                        }
                        else if (accTyp == "L" | accTyp == "I")
                        {
                            dr["debit_amt"] = decimal.Zero;
                            dr["credit_amt"] = GlCoaManager.getCoaopeningBalanceFromExistingDataTable(dtVch, book, glcoacode, Session["septype"].ToString(), "A", StartDt, EndDt).ToString();
                        }
                        dr["bal"] = decimal.Zero;
                        Ds.Tables[0].Rows.InsertAt(dr, 0);
                        decimal bal = decimal.Zero;
                        foreach (DataRow drLedg in Ds.Tables[0].Rows)
                        {
                            if (drLedg["debit_amt"].ToString() != "")
                            {
                                drLedg.BeginEdit();                                
                                bal += decimal.Parse(drLedg["debit_amt"].ToString()) - decimal.Parse(drLedg["credit_amt"].ToString());                               
                                drLedg["bal"] = bal;
                                drLedg.AcceptChanges();
                            }
                        }                     
                        lblTitle.Text = "Ledger statement of " + coaDesc + " from " + StartDt + " to " + EndDt + "";
                        //dgLedger.Visible = true;
                        //dgLedger.DataSource = Ds.Tables[0];
                        //Session["dtledger"] = Ds.Tables[0];
                        //dgLedger.DataBind();                       
                    }     
                }
            }
        }
    }
}