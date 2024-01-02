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
using Delve;
using System.Data.SqlClient;
//using cins;


public partial class Budget : System.Web.UI.Page
{
    private static DataTable dtPopBud = new DataTable();
    private static DataTable dtPopBudTmp = new DataTable();
    private static string budamnt = "";
    public static Permis per;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null)
        {
            if (Session.SessionID != "" | Session.SessionID != null)
            {
                clsSession ses = clsSessionManager.getSession(Session.SessionID);
                if (ses != null)
                {
                    Session["user"] = ses.UserId;
                    Session["book"] = "AMB";
                    string connectionString = DataManager.OraConnString();
                    SqlDataReader dReader;
                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = connectionString;
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "Select user_grp,[description],UserType,case when UserType=1 then 'Bangladesh' else 'Philippine' end AS[LoginCountry] from utl_userinfo where upper(user_name)=upper('" +
                        Session["user"].ToString().ToUpper() + "') and status='A'";
                    conn.Open();
                    dReader = cmd.ExecuteReader();
                    string wnot = "", userType = "";
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            Session["userlevel"] = int.Parse(dReader["user_grp"].ToString());
                            //Session["dept"] = dReader["dept"].ToString();
                            wnot = "Welcome " + dReader["description"].ToString();
                            Session["LoginCountry"] = dReader["LoginCountry"].ToString();
                            userType = dReader["UserType"].ToString();
                        }
                        Session["wnote"] = wnot;
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        //cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type from gl_set_of_books where book_name='" + Session["book"].ToString() + "' ";
                        if (Convert.ToInt32(userType) == 2)
                        {

                            Session["bookMAN"] = "MAN";
                        }
                        else
                        {
                            Session["bookMAN"] = Session["book"].ToString();
                        }
                        cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type,ShotName from gl_set_of_books where book_name='" + Session["bookMAN"] + "' ";

                        if (dReader.IsClosed == false)
                        {
                            dReader.Close();
                        }
                        dReader = cmd.ExecuteReader();
                        if (dReader.HasRows == true)
                        {
                            while (dReader.Read())
                            {
                                Session["septype"] = dReader["separator_type"].ToString();
                                Session["org"] = dReader["book_desc"].ToString();
                                Session["add1"] = dReader["company_address1"].ToString();
                                Session["add2"] = dReader["company_address2"].ToString();
                            }
                        }
                    }
                    dReader.Close();
                    conn.Close();
                }
            }
        }
        try
        {
            string pageName = DataManager.GetCurrentPageName();
            string modid = PermisManager.getModuleId(pageName);
            per = PermisManager.getUsrPermis(Session["user"].ToString().Trim().ToUpper(), modid);
            if (per != null && per.AllowView == "Y")
            {
                ((Label)Page.Master.FindControl("lblLogin")).Text = Session["wnote"].ToString();
                ((Label)Page.Master.FindControl("lblCountryName")).Text = Session["LoginCountry"].ToString();
                ((LinkButton)Page.Master.FindControl("lbLogout")).Visible = true;
            }
            else
            {
                Response.Redirect("Home.aspx?sid=sam");
            }
        }
        catch
        {
            Response.Redirect("Default.aspx?sid=sam");
        }
        if (!Page.IsPostBack)
        {
            txtFinStartDt.Attributes.Add("onBlur", "formatdate('" + txtFinStartDt.ClientID + "')");
            txtFinEndDt.Attributes.Add("onBlur", "formatdate('" + txtFinEndDt.ClientID + "')");
            dtPopBud.Clear();
            if (dtPopBud.Columns.Count == 0)
            {
                dtPopBud.Columns.Add("gl_coa_code", typeof(string));
                dtPopBud.Columns.Add("bud_inc_pct", typeof(string));
                dtPopBud.Columns.Add("bud_tol_pct", typeof(string));
                dtPopBud.Columns.Add("bud_tol_amnt", typeof(string));
                dtPopBud.Columns.Add("bud_override_amnt", typeof(string));
                dtPopBud.Columns.Add("bud_amnt", typeof(string));
                dtPopBud.Columns.Add("status", typeof(string));
                dtPopBud.Columns.Add("seg_coa_desc", typeof(string));
            }
            dgBudMst.Visible = true;
            dgBudget.Visible = false;
            LoadBudget();
            dgBudMst.DataBind();
            lblTranStatus.Visible = false;
            lblTranStatus.Text = "";
            btnAuth.Visible = false;
        }
        txtFinYear.Focus();
    }
    public void LoadBudget()
    {
        DataTable dt = BudgetManager.GetBudgetMsts();
        dgBudMst.DataSource = dt;
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtBudgetSysId.Text = "";
        txtDesc.Text = "";
        txtFinYear.Text = "";
        txtFinStartDt.Text = "";
        txtFinEndDt.Text = "";
        ddlBudTypeCode.SelectedIndex = 0;
        ddlBudOpen.SelectedIndex = 0;
        txtStatus.Text = "";
        txtPCostCenter.Text = "";
        txtPSegCode.Text = "";
        dtPopBud.Clear();
        dtPopBudTmp.Clear();
        dgBudget.Visible = false;
        dgBudMst.Visible = true;
        LoadBudget();
        dgBudMst.DataBind();
        pnlPopBud.Visible = true;
        lblTranStatus.Visible = false;
        btnAuth.Visible = false;
    }
    protected void btnFind_Click(object sender, EventArgs e)
    {

    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (txtStatus.Text == "A")
        {
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
            lblTranStatus.Text = "You cannot change authorized records.";
            return;
        }
        if (txtBudgetSysId.Text == "")
        {
            if (per.AllowAdd == "Y")
            {
                if (txtFinYear.Text == "")
                {
                    lblTranStatus.Visible = true;
                    lblTranStatus.ForeColor = System.Drawing.Color.Red;
                    lblTranStatus.Text = "Financial year cannot be null.";
                    return;
                }
                BudgetMst budmst = new BudgetMst();
                budmst.BudDesc = txtDesc.Text;
                budmst.BookName = Session["book"].ToString();
                budmst.BudOpen = ddlBudOpen.SelectedValue.ToString();
                budmst.BudTypeCode = ddlBudTypeCode.SelectedValue.ToString();
                budmst.FinEndDt = txtFinEndDt.Text.ToString();
                budmst.FinStartDt = txtFinStartDt.Text.ToString();
                budmst.FinYear = txtFinYear.Text.ToString();
                budmst.Status = "U";
                budmst.BudSysId = IdManager.GetNextID("sgl_budget", "bud_sys_id").ToString();
                txtBudgetSysId.Text = budmst.BudSysId;
                BudgetManager.CreateBudgetMst(budmst);
                BudgetDtl buddtl;
                foreach (DataRow dr in dtPopBud.Rows)
                {
                    buddtl = new BudgetDtl();
                    buddtl.BudSysId = budmst.BudSysId;
                    buddtl.BookName = Session["book"].ToString();
                    buddtl.BudAmnt = dr["bud_amnt"].ToString();
                    buddtl.BudIncPct = dr["bud_inc_pct"].ToString();
                    buddtl.BudOverrideAmnt = dr["bud_override_amnt"].ToString();
                    buddtl.BudTolAmnt = dr["bud_tol_amnt"].ToString();
                    buddtl.BudTolPct = dr["bud_tol_pct"].ToString();
                    buddtl.GlCoaCode = dr["gl_coa_code"].ToString();
                    buddtl.Status = "U";
                    BudgetManager.CreateBudgetDtl(buddtl);
                }
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Green;
                lblTranStatus.Text = "Successfully Saved...";
            }
            else
            {
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
                lblTranStatus.Text = "You have no permission to add these data!!!";
            }
        }
        else
        {
            if (per.AllowEdit == "Y")
            {
                if (txtFinYear.Text == "")
                {
                    lblTranStatus.Visible = true;
                    lblTranStatus.ForeColor = System.Drawing.Color.Red;
                    lblTranStatus.Text = "Financial year cannot be null.";
                    return;
                }
                BudgetMst budmst = BudgetManager.GetBudgetMst(txtBudgetSysId.Text);
                if (budmst != null)
                {
                    budmst.BudDesc = txtDesc.Text;
                    budmst.BookName = Session["book"].ToString();
                    budmst.BudOpen = ddlBudOpen.SelectedValue.ToString();
                    budmst.BudTypeCode = ddlBudTypeCode.SelectedValue.ToString();
                    budmst.FinEndDt = txtFinEndDt.Text.ToString();
                    budmst.FinStartDt = txtFinStartDt.Text.ToString();
                    budmst.FinYear = txtFinYear.Text.ToString();
                    budmst.Status = "U";
                    BudgetManager.UpdateBudgetMst(budmst);
                }
                BudgetDtl buddtl;
                if (dtPopBud.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtPopBud.Rows)
                    {
                        buddtl = BudgetManager.GetBudgetDtl(txtBudgetSysId.Text.ToString(), dr["gl_coa_code"].ToString());
                        if (buddtl != null)
                        {
                            if (buddtl.BudAmnt != dr["bud_amnt"].ToString() | buddtl.BudIncPct != dr["bud_inc_pct"].ToString() |
                                buddtl.BudOverrideAmnt != dr["bud_override_amnt"].ToString() |
                                buddtl.BudTolAmnt != dr["bud_tol_amnt"].ToString() |
                                buddtl.BudTolPct != dr["bud_tol_pct"].ToString())
                            {
                                buddtl.BudAmnt = dr["bud_amnt"].ToString();
                                buddtl.BudIncPct = dr["bud_inc_pct"].ToString();
                                buddtl.BudOverrideAmnt = dr["bud_override_amnt"].ToString();
                                buddtl.BudTolAmnt = dr["bud_tol_amnt"].ToString();
                                buddtl.BudTolPct = dr["bud_tol_pct"].ToString();
                                BudgetManager.UpdateBudgetDtl(buddtl);
                            }
                        }
                        else
                        {
                            buddtl = new BudgetDtl();
                            buddtl.BudSysId = budmst.BudSysId;
                            buddtl.BookName = Session["book"].ToString();
                            buddtl.BudAmnt = dr["bud_amnt"].ToString();
                            buddtl.BudIncPct = dr["bud_inc_pct"].ToString();
                            buddtl.BudOverrideAmnt = dr["bud_override_amnt"].ToString();
                            buddtl.BudTolAmnt = dr["bud_tol_amnt"].ToString();
                            buddtl.BudTolPct = dr["bud_tol_pct"].ToString();
                            buddtl.GlCoaCode = dr["gl_coa_code"].ToString();
                            buddtl.Status = "U";
                            BudgetManager.CreateBudgetDtl(buddtl);
                        }
                    }
                    string found = "N";
                    for (int i = 0; i < dtPopBudTmp.Rows.Count; i++)
                    {
                        for (int j = 0; j < dtPopBud.Rows.Count; j++)
                        {
                            if (((DataRow)dtPopBudTmp.Rows[i])["gl_coa_code"].ToString() == ((DataRow)dtPopBud.Rows[j])["gl_coa_code"].ToString())
                            {
                                found = "Y";
                            }
                        }
                        if (found == "N")
                        {
                            BudgetManager.DeleteBudgetDtl(txtBudgetSysId.Text.ToString(), ((DataRow)dtPopBudTmp.Rows[i])["gl_coa_code"].ToString());
                        }
                        found = "N";
                    }
                    lblTranStatus.Visible = true;
                    lblTranStatus.ForeColor = System.Drawing.Color.Green;
                    lblTranStatus.Text = "Successfully Saved...";
                    //lblTranStatus.Text = dtPopBudTmp.Rows.Count.ToString();
                }
            }
            else
            {
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
                lblTranStatus.Text = "You have no permission to edit/update these data!!!";
            }
        }
        
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            if (txtStatus.Text == "A")
            {
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
                lblTranStatus.Text = "You cannot change authorized records.";
                return;
            }
            if (txtBudgetSysId.Text == "")
            {
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
                lblTranStatus.Text = "Budget System ID cannot be null.";
                return;
            }
            if (txtFinYear.Text == "")
            {
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
                lblTranStatus.Text = "Financial year cannot be null.";
                return;
            }
            BudgetManager.DeleteBudgetMst(txtBudgetSysId.Text.ToString());
            BudgetManager.DeleteBudgetDtls(txtBudgetSysId.Text.ToString());
            pnlPopBud.Visible = true;
            btnClear_Click(sender, e);
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Green;
            lblTranStatus.Text = "Record is deleted from database.";
        }
        else
        {
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
            lblTranStatus.Text = "You have no permission to delete these data!!!";
        }
    }
    protected void dgBudget_CancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        dgBudget.EditIndex = -1;
        dgBudget.DataSource = dtPopBud;        
        dgBudget.DataBind();
    }
    protected void dgBudget_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }
    protected void dgBudget_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        dtPopBud.Rows.RemoveAt(e.RowIndex);
        dgBudget.DataSource = dtPopBud;
        dgBudget.DataBind();
        lblTranStatus.Visible = true;
        lblTranStatus.ForeColor = System.Drawing.Color.YellowGreen;
        lblTranStatus.Text = "Record is deleted from buffer. To save in database click on save link.";
    }
    protected void dgBudget_RowEditing(object sender, GridViewEditEventArgs e)
    {
        //DataTable dt = dtPopBud.Clone();
        //foreach (DataRow drow in dtPopBud.Rows)
        //{
        //    dt.ImportRow(drow);
        //}
        budamnt = ((decimal.Parse(((Label)dgBudget.Rows[e.NewEditIndex].FindControl("lblBudAmnt")).Text.ToString())
            * 100) / (decimal.Parse(((Label)dgBudget.Rows[e.NewEditIndex].FindControl("lblBudIncPct")).Text.ToString()) + 100)).ToString();
        dgBudget.DataSource = dtPopBud;
        dgBudget.EditIndex = e.NewEditIndex;
        dgBudget.DataBind();
    }
    protected void dgBudget_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        string gldesc = ((DataRow)dtPopBud.Rows[e.RowIndex])["seg_coa_desc"].ToString();
        dtPopBud.Rows.RemoveAt(e.RowIndex);
        DataRow dr = dtPopBud.NewRow();
        dr["gl_coa_code"] = ((TextBox)dgBudget.Rows[e.RowIndex].FindControl("txtGlCoaCode")).Text.ToString().Trim();
        dr["bud_inc_pct"] = ((TextBox)dgBudget.Rows[e.RowIndex].FindControl("txtBudIncPct")).Text.ToString().Trim();
        dr["bud_tol_pct"] = ((TextBox)dgBudget.Rows[e.RowIndex].FindControl("txtBudTolPct")).Text.ToString().Trim();
        dr["bud_tol_amnt"] = decimal.Parse(((TextBox)dgBudget.Rows[e.RowIndex].FindControl("txtBudTolAmnt")).Text.ToString().Trim()).ToString();
        dr["bud_override_amnt"] = decimal.Parse(((TextBox)dgBudget.Rows[e.RowIndex].FindControl("txtBudOverrideAmnt")).Text.ToString().Trim()).ToString();
        dr["bud_amnt"] = decimal.Parse(((TextBox)dgBudget.Rows[e.RowIndex].FindControl("txtBudAmnt")).Text.ToString().Trim()).ToString();
        dr["status"] = ((TextBox)dgBudget.Rows[e.RowIndex].FindControl("txtStatus")).Text.ToString().Trim();
        dr["seg_coa_desc"] = gldesc;
        dtPopBud.Rows.InsertAt(dr, e.RowIndex);
        dgBudget.DataSource = dtPopBud;
        dgBudget.EditIndex = -1;
        dgBudget.DataBind();
        lblTranStatus.Visible = true;
        lblTranStatus.ForeColor = System.Drawing.Color.YellowGreen;
        lblTranStatus.Text = "Record is updated in buffer. To save in database click on save link.";

    }
    protected void dgBudget_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {

    }
    protected void dgBudget_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
            {
                ((TextBox)e.Row.FindControl("txtGlCoaCode")).ToolTip = ((Label)e.Row.FindControl("lblDesc")).Text.ToString();
            }
            else
            {
                ((Label)e.Row.FindControl("lblGlCoaCode")).ToolTip = ((Label)e.Row.FindControl("lblDesc")).Text.ToString();
            }
        }
    }
    protected void btnPopBudget_Click(object sender, EventArgs e)
    {
        dgBudMst.Visible = false;
        dgBudget.Visible = true;
        //dtPopBud.Clear();
        if (dtPopBud.Columns.Count == 0)
        {
            dtPopBud.Columns.Add("gl_coa_code", typeof(string));
            dtPopBud.Columns.Add("bud_inc_pct", typeof(string));
            dtPopBud.Columns.Add("bud_tol_pct", typeof(string));
            dtPopBud.Columns.Add("bud_tol_amnt", typeof(string));
            dtPopBud.Columns.Add("bud_override_amnt", typeof(string));
            dtPopBud.Columns.Add("bud_amnt", typeof(string));
            dtPopBud.Columns.Add("status", typeof(string));
            dtPopBud.Columns.Add("seg_coa_desc", typeof(string));
        }

        //DataTable dt = BudgetManager.GetSegCode(txtPSegCode.Text.ToString(),txtPCostCenter.Text.ToString(),
        //    DateTime.Parse(txtFinStartDt.Text.ToString()).AddYears(-1).Year.ToString(),
        //    DateTime.Parse(txtFinStartDt.Text.ToString()).Year.ToString());
        //foreach (DataRow dr in dt.Rows)
        //{
        //    DataRow drBud = dtPopBud.NewRow();
        //    drBud["gl_coa_code"] = dr[0].ToString();
        //    drBud["bud_inc_pct"] = "5";
        //    drBud["bud_tol_pct"] = "0";
        //    drBud["bud_tol_amnt"] = "0.00";
        //    drBud["bud_override_amnt"] = "0.00";
        //    drBud["bud_amnt"] = decimal.Parse(dr[2].ToString()).ToString("N3");
        //    drBud["status"] = "U";
        //    drBud["seg_coa_desc"] = dr[1].ToString();
        //    dtPopBud.Rows.Add(drBud);
        //}
        //dgBudget.DataSource = dtPopBud;
        //dgBudget.DataBind();

        string slvl = SegCoaManager.GetLvlCode(txtPSegCode.Text);
        string clvl = SegCoaManager.GetLvlCode(txtPCostCenter.Text);
        DataTable dtSeg = BudgetManager.GetSegCoa(txtPSegCode.Text,slvl,Session["book"].ToString());
        DataTable dtCost = BudgetManager.GetSegCoa(txtPCostCenter.Text,clvl,Session["book"].ToString());
        foreach (DataRow drCost in dtCost.Rows)
        {
            foreach (DataRow drSeg in dtSeg.Rows)
            {
                string budst = VouchManager.getBudStatus("5-" + drSeg["seg_coa_code"].ToString() + "-" + drCost["seg_coa_code"].ToString(), DateTime.Parse(txtFinStartDt.Text.ToString()).AddDays(1).ToString("dd-MMM-yyyy"));
                if (budst == "N")
                {
                    DataRow drBud = dtPopBud.NewRow();
                    drBud["gl_coa_code"] = "5-" + drSeg["seg_coa_code"].ToString() + "-" + drCost["seg_coa_code"].ToString();
                    drBud["bud_inc_pct"] = "5";
                    drBud["bud_tol_pct"] = "0";
                    drBud["bud_tol_amnt"] = "0.00";
                    drBud["bud_override_amnt"] = "0.00";
                    decimal bal = VouchManager.getBal(drSeg["seg_coa_code"].ToString(),
                        DateTime.Parse(txtFinStartDt.Text).AddYears(-1).ToString("dd-MMM-yyyy"),
                        DateTime.Parse(txtFinStartDt.Text).AddDays(-1).ToString("dd-MMM-yyyy"),
                        drCost["seg_coa_code"].ToString(), Session["book"].ToString());
                    decimal incBud = bal + bal * 5 / 100;
                    drBud["bud_amnt"] = incBud.ToString("N3");
                    drBud["status"] = "U";
                    drBud["seg_coa_desc"] = drSeg["seg_coa_desc"].ToString() + " - " + drCost["seg_coa_desc"].ToString();
                    dtPopBud.Rows.Add(drBud);
                }
            }
        }
        dgBudget.DataSource = dtPopBud;
        dgBudget.DataBind();

        if (dtPopBud.Rows.Count > 0)
        {
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.YellowGreen;
            lblTranStatus.Text = "Now you can finalize your budget. After finalize please click on save link.";
        }
        else
        {
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
            lblTranStatus.Text = "No budget is populated. Check the segment code or cost center.";
        }
        //lblTranStatus.Text = dtSeg.Rows.Count.ToString() + "-" + dtCost.Rows.Count.ToString() + "=" + dtPopBud.Columns.Count.ToString() ;
    }
    protected void txtFinYear_TextChanged(object sender, EventArgs e)
    {
        txtFinYear.Text = txtFinYear.Text.ToString().Trim().Replace(" ","");        
        if (txtFinYear.Text.Length == 9)
        {
            if ((int.Parse(txtFinYear.Text.ToString().Substring(0, 4)) + 1) != int.Parse(txtFinYear.Text.ToString().Substring(5, 4)))
            {
                txtFinYear.Text = "";
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
                lblTranStatus.Text = "Financial year is not in correct format. Financial year should be 2009-2010 format.";
            }
            else if (VouchManager.getBudYn(txtFinYear.Text.ToString())=="Y")
            {
                txtFinYear.Text = "";
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
                lblTranStatus.Text = "Budget for this financial year is already created.";
            }
            else
            {
                txtFinStartDt.Text = "01-jul-" + txtFinYear.Text.ToString().Substring(0, 4);
                txtFinEndDt.Text = "30-jun-" + txtFinYear.Text.ToString().Substring(5);
                txtStatus.Text = "U";
                txtDesc.Text = "Budget for AMBAN in the financial year of " + txtFinYear.Text.ToString();
            }
        }
    }
    protected void txtBudIncPct_TextChanged(object sender, EventArgs e)
    {
        //lblTranStatus.Text = decimal.Parse(budamnt).ToString("N3") ;
        TextBox tb = (TextBox)sender;
        GridViewRow gvr = (GridViewRow)tb.NamingContainer;
        //decimal bamnt = decimal.Parse(budamnt) * 100 / (int.Parse(((TextBox)gvr.FindControl("txtBudIncPct")).Text.ToString()) + 100);
        ((TextBox)gvr.FindControl("txtBudAmnt")).Text = (decimal.Parse(budamnt) + ((decimal.Parse(budamnt) * decimal.Parse(((TextBox)gvr.FindControl("txtBudIncPct")).Text.ToString())) / 100)).ToString("N3");
    }
    protected void txtBudTolPct_TextChanged(object sender, EventArgs e)
    {
        //lblTranStatus.Text = decimal.Parse(budamnt).ToString("N3");
        TextBox tb = (TextBox)sender;
        GridViewRow gvr = (GridViewRow)tb.NamingContainer;
        //decimal bamnt = decimal.Parse(budamnt) * 100 / (int.Parse(((TextBox)gvr.FindControl("txtBudIncPct")).Text.ToString()) + 100);
        ((TextBox)gvr.FindControl("txtBudTolAmnt")).Text = decimal.Parse(((decimal.Parse(budamnt) * decimal.Parse(((TextBox)gvr.FindControl("txtBudTolPct")).Text.ToString())) / 100).ToString()).ToString("N3");
    }
    protected void dgBudMst_SelectedIndexChanged(object sender, EventArgs e)
    {
        BudgetMst budmst = BudgetManager.GetBudgetMst(dgBudMst.SelectedRow.Cells[1].Text.ToString().Trim());
        if (budmst != null)
        {
            txtBudgetSysId.Text = budmst.BudSysId;
            txtDesc.Text = budmst.BudDesc;
            ddlBudOpen.SelectedValue = budmst.BudOpen;
            ddlBudTypeCode.SelectedValue = budmst.BudTypeCode;
            txtFinEndDt.Text = DateTime.Parse(budmst.FinEndDt.ToString()).ToString("dd-MMM-yyyy");
            txtFinStartDt.Text = DateTime.Parse(budmst.FinStartDt.ToString()).ToString("dd-MMM-yyyy");
            txtFinYear.Text = budmst.FinYear;
            txtStatus.Text = budmst.Status;
        }
        dtPopBud.Clear();
        if (dtPopBud.Columns.Count == 0)
        {
            dtPopBud.Columns.Add("gl_coa_code", typeof(string));
            dtPopBud.Columns.Add("bud_inc_pct", typeof(string));
            dtPopBud.Columns.Add("bud_tol_pct", typeof(string));
            dtPopBud.Columns.Add("bud_tol_amnt", typeof(string));
            dtPopBud.Columns.Add("bud_override_amnt", typeof(string));
            dtPopBud.Columns.Add("bud_amnt", typeof(string));
            dtPopBud.Columns.Add("status", typeof(string));
            dtPopBud.Columns.Add("seg_coa_desc", typeof(string));
        }
        txtBudgetSysId.Enabled = true;
        dtPopBud = BudgetManager.GetBudgetDtls(txtBudgetSysId.Text);
        dtPopBudTmp = dtPopBud.Clone();
        dtPopBudTmp = BudgetManager.GetBudgetDtls(txtBudgetSysId.Text);
        dgBudget.DataSource = dtPopBud;
        dgBudget.Visible = true;
        dgBudMst.Visible = false;
        dgBudget.DataBind();
        
        if (txtStatus.Text == "A")
        {
            VaDisable();
            btnAuth.Visible = false;
        }
        else
        {
            VaEnable();
            btnAuth.Visible = true;
        }
    }
    protected void btnAuth_Click(object sender, EventArgs e)
    {
        if (txtBudgetSysId.Text == "")
        {
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
            lblTranStatus.Text = "No budget to authorize";
            return;
        }        
    }
    protected void LoginBtn_Click(object sender, EventArgs e)
    {
        if (per.AllowAutho == "Y")
        {
            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select password,user_grp from utl_userinfo where upper(user_name)=upper('" + loginId.Text + "')";
            conn.Open();
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())

                    if (pwd.Text.Trim() == dReader["password"].ToString())
                    {
                        if (int.Parse(dReader["user_grp"].ToString()) > 1)
                        {
                            BudgetMst budmst = BudgetManager.GetBudgetMst(txtBudgetSysId.Text.ToString());
                            budmst.Status = "A";
                            budmst.AuthoUser = loginId.Text.ToString().ToUpper();
                            budmst.AuthoDate = System.DateTime.Now.Date.ToString("dd/MM/yyyy");
                            BudgetManager.UpdateBudgetMst(budmst);
                            BudgetDtl buddtl;
                            if (dtPopBud.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dtPopBud.Rows)
                                {
                                    buddtl = BudgetManager.GetBudgetDtl(txtBudgetSysId.Text.ToString(), dr["gl_coa_code"].ToString());
                                    if (buddtl != null)
                                    {
                                        buddtl.Status = "A";
                                        BudgetManager.UpdateBudgetDtl(buddtl);
                                    }
                                }
                            }
                            foreach (GridViewRow gr in dgBudget.Rows)
                            {
                                ((Label)gr.FindControl("lblStatus")).Text = "A";
                            }
                            lblTranStatus.Visible = true;
                            lblTranStatus.ForeColor = System.Drawing.Color.Green;
                            lblTranStatus.Text = "Authorized successfully!!!";
                            txtStatus.Text = "A";
                            pwd.Text = "";
                            LoginPanel.Visible = false;
                            VaDisable();
                        }
                        else
                        {
                            lblTranStatus.Visible = true;
                            lblTranStatus.ForeColor = System.Drawing.Color.Red;
                            lblTranStatus.Text = "You Have No Enough Permission to Authorize!!?";
                            pwd.Text = "";
                        }
                    }
                    else
                    {
                        lblTranStatus.Visible = true;
                        lblTranStatus.Text = "Password Incorrect, Authentication Failed...";
                        lblTranStatus.ForeColor = System.Drawing.Color.Red;
                        pwd.Text = "";
                    }
            }
            else
            {
                lblTranStatus.Visible = true;
                lblTranStatus.Text = "No Such User.....";
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
            }
            dReader.Close();
            conn.Close();
        }
        else
        {
            lblTranStatus.Visible = true;
            lblTranStatus.Text = "You have no permission to authorize these data!!";
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
        }        
    }
    protected void CancelBtn_Click(object sender, EventArgs e)
    {
        
    }
    public void VaDisable()
    {
        txtDesc.Enabled = false;
        txtFinYear.Enabled = false;
        txtFinStartDt.Enabled = false;
        txtFinEndDt.Enabled = false;
        ddlBudOpen.Enabled = false;
        ddlBudTypeCode.Enabled = false;
        dgBudget.Columns[0].Visible = false;
        pnlPopBud.Visible = false;
        btnAuth.Visible = false;
        
    }
    public void VaEnable()
    {
        txtDesc.Enabled = true;
        txtFinYear.Enabled = true;
        txtFinStartDt.Enabled = true;
        txtFinEndDt.Enabled = true;
        ddlBudOpen.Enabled = true;
        ddlBudTypeCode.Enabled = true;
        dgBudget.Columns[0].Visible = true;
        pnlPopBud.Visible = true;
        btnAuth.Visible = true;
    }
    protected void dgBudget_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgBudget.DataSource = dtPopBud;
        dgBudget.PageIndex = e.NewPageIndex;
        dgBudget.DataBind();
    }
}
