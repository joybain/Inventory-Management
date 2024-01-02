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
using Delve;
using System.Configuration;
using System.Data.SqlClient;
//using cins;


public partial class GlFinYear : System.Web.UI.Page
{
    public static Permis per;
    static string pageAction;
    //private static DataTable dtMonth = new DataTable();
    //private static DataTable dtMonthTmp = new DataTable();
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
            //if (dtMonth.Columns.Count == 0)
            //{
            //    dtMonth.Columns.Add("month_sl", typeof(int));
            //    dtMonth.Columns.Add("fin_mon", typeof(string));
            //    dtMonth.Columns.Add("quarter", typeof(string));
            //    dtMonth.Columns.Add("mon_start_dt", typeof(string));
            //    dtMonth.Columns.Add("mon_end_dt", typeof(string));
            //    dtMonth.Columns.Add("year_flag", typeof(string));
            //}
            pageAction = "ADD";
            txtStatus.Text = "U";
            //numbers(txtFinYear);
            // btnPopMonth.Attributes.Add("onclick", "javascript:if(confirm('Do you really wish to do this?')==false) return false;");
            LoadGrid();
            LoadGridBind();
            lblTranStatus.Visible = false;
            lblTranStatus.Text = "";
            btnAuth.Visible = false;
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (txtFinYear.Text != "")
        {
            if (per.AllowDelete == "Y")
            {
                FinYearManager.DeleteFinMonths(txtFinYear.Text);
                FinYearManager.DeleteFinYear(txtFinYear.Text);
                //lblTranStatus.Visible = true;
                //lblTranStatus.ForeColor = System.Drawing.Color.Green;
                //lblTranStatus.Text = "Record Deleted successfully.";
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record Deleted successfully!!');", true);
                btnClear_Click(sender, e);
            }
            else
            {
                //lblTranStatus.Visible = true;
                //lblTranStatus.ForeColor = System.Drawing.Color.Red;
                //lblTranStatus.Text = "You have no permission to delete these data!!!";
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You have no permission to delete these data!!');", true);
            }
        }
    }
    protected void btnPopMonth_Click(object sender, EventArgs e)
    {

        if (txtFinYear.Text == String.Empty)
        {
            //lblTranStatus.Visible = true;
            //lblTranStatus.ForeColor = System.Drawing.Color.Red;
            //lblTranStatus.Text = "Financial year cannot be null.";
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Financial year cannot be null!!');", true);
            return;
        }
        else
        {
            if (pageAction == "ADD")
            {
                DataTable dtMonth = new DataTable();
                dtMonth.Columns.Add("month_sl", typeof(int));
                dtMonth.Columns.Add("fin_mon", typeof(string));
                dtMonth.Columns.Add("quarter", typeof(string));
                dtMonth.Columns.Add("mon_start_dt", typeof(string));
                dtMonth.Columns.Add("mon_end_dt", typeof(string));
                dtMonth.Columns.Add("year_flag", typeof(string));
                string Yr1 = String.Empty;
                string Yr2 = String.Empty;
                string Yr = String.Empty;
                int iYear = int.Parse(txtFinYear.Text.Substring(0, 4));
                int iMonth = 7;
                if (txtFinYear.Text.Length == 9)
                {
                    Yr1 = txtFinYear.Text.Substring(2, 2);
                    Yr2 = txtFinYear.Text.Substring(7, 2);
                    Yr = "-" + Yr1 + "-" + Yr2;
                    iMonth = 7;
                }
                else if (txtFinYear.Text.Length == 4)
                {
                    Yr1 = txtFinYear.Text.Substring(2, 2);
                    Yr = "-" + Yr1;
                    iMonth = 1;
                }

                DateTime monthStart = new DateTime(iYear, iMonth, 1, 12, 0, 0);
                DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);
                DataRow row1;
                for (int i = 0; i <= 11; i++)
                {
                    row1 = dtMonth.NewRow();
                    row1["month_sl"] = i + 1;
                    String ms = monthStart.AddMonths(i).ToString("dd/MM/yyyy");
                    String me = monthEnd.AddMonths(i).ToString("dd/MM/yyyy");
                    row1["fin_mon"] = monthStart.AddMonths(i).ToString("M").Substring(0, 3).ToUpper() + Yr;
                    row1["quarter"] = ((i + 4) / 4).ToString("D");
                    row1["mon_start_dt"] = ms;
                    row1["mon_end_dt"] = me;
                    row1["year_flag"] = "N";
                    dtMonth.Rows.Add(row1);
                    iMonth = iMonth + i + 1;
                }
                dgFinYear.Visible = false;
                dgFinMonth.Visible = true;
                dgFinMonth.DataSource = dtMonth;
                Session["dtmon"] = dtMonth;
                dgFinMonth.DataBind();
                //lblTranStatus.Visible = true;
                //lblTranStatus.ForeColor = System.Drawing.Color.YellowGreen;
                //lblTranStatus.Text = "Financial year is created. To Save please click on save link.";
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Financial year is created. To Save please click on save link.');", true);
            }
            else
            {
                //lblTranStatus.Visible = true;
                //lblTranStatus.ForeColor = System.Drawing.Color.Red;
                //lblTranStatus.Text = "Financial year is already created.";
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Financial year is already created.');", true);
            }
        }
    }
    public void LoadGrid()
    {
        DataTable dtfnyr = FinYearManager.GetFinYears();
        dgFinYear.DataSource = dtfnyr;


    }
    public void LoadGridBind()
    {
        dgFinYear.DataBind();
    }

    public void numbers(TextBox txtbox)
    {
        txtbox.Attributes.Add("onkeypress", "return clickButton(event)");
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        //dtMonth.Clear();
        //dtMonthTmp.Clear();
        //pageAction = "ADD";        
        //ClearMaster();
        //MasterShow();
        //LoadGrid();
        //LoadGridBind();
        //lblTranStatus.Visible = false;
        //lblTranStatus.Text = "";
        Session.Remove("dtmon");
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void btnFind_Click(object sender, EventArgs e)
    {

    }
    protected void dgFinYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        pageAction = "EDIT";
        txtFinYear.Text = dgFinYear.SelectedRow.Cells[1].Text;
        FinYear finyear = FinYearManager.GetFinYear(txtFinYear.Text);
        txtDesc.Text = finyear.Description;
        txtStartDate.Text = finyear.StartDate;
        txtEndDate.Text = finyear.EndDate;
        dlYearFlag.SelectedValue = finyear.YearFlag;
        dlWeeklyFin.SelectedValue = finyear.WeeklyFin;
        txtStatus.Text = finyear.Status;
        DataTable dtMonth = FinYearManager.GetFinMonths(txtFinYear.Text);
        Session["dtmon"] = dtMonth;
        DetailShow();
        dgFinMonth.DataSource = dtMonth;
        dgFinMonth.DataBind();
        if (finyear.Status == "A")
        {
            btnAuth.Visible = false;
        }
        else
        {
            btnAuth.Visible = true;
        }
    }
    private void alerts()
    {
        ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script language=JavaScript>alert('Enter the Financial Year first');</script>");
    }
    private void alerts1()
    {
        ClientScript.RegisterStartupScript(this.GetType(), "return", "<script language=JavaScript> window.confirm('Do you really want to populate month?');</script>");
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (pageAction == "ADD")
        {
            if (per.AllowAdd == "Y")
            {
                FinYear finyear = new FinYear();
                finyear.FinYr = txtFinYear.Text;
                finyear.Description = txtDesc.Text;
                finyear.StartDate = txtStartDate.Text;
                finyear.EndDate = txtEndDate.Text;
                finyear.BookName = Session["book"].ToString();
                finyear.WeeklyFin = dlWeeklyFin.SelectedValue;
                finyear.Status = txtStatus.Text;
                finyear.YearFlag = dlYearFlag.SelectedValue;
                finyear.EntryUser = Session["user"].ToString();
                finyear.EntryDate = System.DateTime.Now.Date.ToString("dd/MM/yyyy");
                FinYearManager.CreateFinYear(finyear);
                FinMonth finmon;
                if (Session["dtmon"] != null)
                {
                    DataTable dtMonth = (DataTable)Session["dtmon"];
                    foreach (DataRow dr in dtMonth.Rows)
                    {
                        finmon = new FinMonth();
                        finmon.BookName = Session["book"].ToString();
                        finmon.MonthSl = dr["month_sl"].ToString();
                        finmon.FinYear = txtFinYear.Text;
                        finmon.FinMon = dr["fin_mon"].ToString();
                        finmon.MonStartDt = dr["mon_start_dt"].ToString();
                        finmon.MonEndDt = dr["mon_end_dt"].ToString();
                        finmon.Quarter = dr["quarter"].ToString();
                        finmon.YearFlag = dr["year_flag"].ToString();
                        FinYearManager.CreateFinMonth(finmon);
                    }
                }
                //lblTranStatus.Visible = true;
                //lblTranStatus.ForeColor = System.Drawing.Color.Green;
                //lblTranStatus.Text = "Record saved successfully.";
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record saved successfully!!');", true);
            }
            else
            {
                //lblTranStatus.Visible = true;
                //lblTranStatus.ForeColor = System.Drawing.Color.Red;
                //lblTranStatus.Text = "You have no permission to add in this form!!!";
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You have no permission to add in this form!!!');", true);
            }
        }
        else if (pageAction == "EDIT")
        {
            if (per.AllowEdit == "Y")
            {
                FinYear finyear = FinYearManager.GetFinYear(txtFinYear.Text);
                finyear.Description = txtDesc.Text;
                finyear.StartDate = txtStartDate.Text;
                finyear.EndDate = txtEndDate.Text;
                finyear.BookName = Session["book"].ToString();
                finyear.WeeklyFin = dlWeeklyFin.SelectedValue;
                finyear.Status = txtStatus.Text;
                finyear.YearFlag = dlYearFlag.SelectedValue;
                FinYearManager.UpdateFinYear(finyear);
                FinMonth finmon;
                if (Session["dtmon"] != null)
                {
                    DataTable dtMonth = (DataTable)Session["dtmon"];
                    foreach (DataRow dr in dtMonth.Rows)
                    {
                        finmon = FinYearManager.GetFinMonth(dr["fin_mon"].ToString());
                        if (finmon != null)
                        {
                            finmon.YearFlag = dr["year_flag"].ToString();
                            FinYearManager.UpdateFinMonth(finmon);
                        }
                    }
                }
                //lblTranStatus.Visible = true;
                //lblTranStatus.ForeColor = System.Drawing.Color.Green;
                //lblTranStatus.Text = "Record saved successfully.";
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record saved successfully!!');", true);
            }
            else
            {
                //lblTranStatus.Visible = true;
                //lblTranStatus.ForeColor = System.Drawing.Color.Red;
                //lblTranStatus.Text = "You have no permission to edit/update these data!!!";
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You have no permission to edit/update these data!!');", true);
            }
        }

    }

    protected void dgFinMonth_RowUpdating(Object sender, GridViewUpdateEventArgs e)
    {
        if (Session["dtmon"] != null)
        {
            DataTable dtMonth = (DataTable)Session["dtmon"];
            GridView gv = (GridView)sender;
            DataRow dr = dtMonth.Rows[gv.Rows[e.RowIndex].DataItemIndex];
            dr["month_sl"] = int.Parse(((Label)dgFinMonth.Rows[e.RowIndex].FindControl("lblMonthSl")).Text);
            dr["fin_mon"] = ((Label)dgFinMonth.Rows[e.RowIndex].FindControl("lblFinMon")).Text;
            dr["quarter"] = ((Label)dgFinMonth.Rows[e.RowIndex].FindControl("lblQuarter")).Text;
            dr["mon_start_dt"] = ((Label)dgFinMonth.Rows[e.RowIndex].FindControl("lblMonthStartDt")).Text;
            dr["mon_end_dt"] = ((Label)dgFinMonth.Rows[e.RowIndex].FindControl("lblMonthEndDt")).Text;
            dr["year_flag"] = ((DropDownList)dgFinMonth.Rows[e.RowIndex].FindControl("txtMonYearFlag")).SelectedValue;
            dgFinMonth.DataSource = dtMonth;
            dgFinMonth.EditIndex = -1;
            dgFinMonth.DataBind();
        }
        //lblTranStatus.Visible = true;
        //lblTranStatus.ForeColor = System.Drawing.Color.YellowGreen;
        //lblTranStatus.Text = "Record is updated in buffer. To save in database click on save link.";
        //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is updated in buffer. To save in database click on save link.');", true);
    }


    protected void dgFinMonth_CancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        if (Session["dtmon"] != null)
        {
            DataTable dtMonth = (DataTable)Session["dtmon"];
            dgFinMonth.EditIndex = -1;
            dgFinMonth.DataSource = dtMonth;
            dgFinMonth.DataBind();
        }
    }
    protected void dgFinMonth_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (Session["dtmon"] != null)
        {
            DataTable dtMonth = (DataTable)Session["dtmon"];
            string msr = ((Label)dgFinMonth.Rows[e.NewEditIndex].FindControl("lblMonYearFlag")).Text;
            dgFinMonth.EditIndex = e.NewEditIndex;
            dgFinMonth.DataSource = dtMonth;
            dgFinMonth.DataBind();
            ((DropDownList)dgFinMonth.Rows[e.NewEditIndex].FindControl("txtMonYearFlag")).SelectedIndex = ((DropDownList)dgFinMonth.Rows[e.NewEditIndex].FindControl("txtMonYearFlag")).Items.IndexOf(((DropDownList)dgFinMonth.Rows[e.NewEditIndex].FindControl("txtMonYearFlag")).Items.FindByValue(msr));
        }

    }
    public void MasterShow()
    {
        dgFinMonth.Visible = false;
        dgFinYear.Visible = true;
    }
    public void DetailShow()
    {
        dgFinYear.Visible = false;
        dgFinMonth.Visible = true;
    }
    public void ClearMaster()
    {
        txtFinYear.Text = String.Empty;
        txtDesc.Text = String.Empty;
        txtStartDate.Text = String.Empty;
        txtEndDate.Text = String.Empty;
        dlWeeklyFin.SelectedValue = "N";
        dlYearFlag.SelectedValue = "C";
        txtStatus.Text = String.Empty;
    }
    protected void txtFinYear_TextChanged(object sender, EventArgs e)
    {
        txtFinYear.Text = txtFinYear.Text.ToString().Trim().Replace(" ", "");
        if (txtFinYear.Text.Length == 9)
        {
            txtStartDate.Text = "01/07/" + txtFinYear.Text.ToString().Substring(0, 4);
            txtEndDate.Text = "30/06/" + txtFinYear.Text.ToString().Substring(5);

        }
        else if (txtFinYear.Text.Length == 4)
        {
            txtStartDate.Text = "01/01/" + txtFinYear.Text.ToString().Substring(0, 4);
            txtEndDate.Text = "31/12/" + txtFinYear.Text.ToString().Substring(0, 4);

        }
        txtDesc.Text = "Financial year of " + txtFinYear.Text;
        txtStatus.Text = "U";
    }
    protected void btnAuth_Click(object sender, EventArgs e)
    {
        if (txtFinYear.Text == String.Empty)
        {
            //lblTranStatus.Visible = true;
            //lblTranStatus.ForeColor = System.Drawing.Color.Red;
            //lblTranStatus.Text = "Financial year cannot be null.";
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Financial year cannot be null.');", true);
            return;
        }
        if (txtStatus.Text == "A")
        {
            //lblTranStatus.Visible = true;
            //lblTranStatus.ForeColor = System.Drawing.Color.Red;
            //lblTranStatus.Text = "You cannot change authorized records.";
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You cannot change authorized records.');", true);
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
                            FinYear finyr = FinYearManager.GetFinYear(txtFinYear.Text.ToString());
                            finyr.Status = "A";
                            finyr.AuthoUser = loginId.Text.ToString().ToUpper();
                            finyr.AuthoDate = System.DateTime.Now.Date.ToString("dd/MM/yyyy");
                            FinYearManager.UpdateFinYear(finyr);

                            //lblTranStatus.Visible = true;
                            //lblTranStatus.ForeColor = System.Drawing.Color.Green;
                            //lblTranStatus.Text = "Authorized successfully!!!";
                            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Authorized successfully!!!');", true);
                            txtStatus.Text = "A";
                            pwd.Text = "";

                        }
                        else
                        {
                            //lblTranStatus.Visible = true;
                            //lblTranStatus.ForeColor = System.Drawing.Color.Red;
                            //lblTranStatus.Text = "You Have No Enough Permission to Authorize!!?";
                            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You have no enough permission to authorize.');", true);
                            pwd.Text = "";
                        }
                    }
                    else
                    {
                        //lblTranStatus.Visible = true;
                        //lblTranStatus.Text = "Password Incorrect, Authentication Failed...";
                        //lblTranStatus.ForeColor = System.Drawing.Color.Red;
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Password Incorrect, Authentication Failed...');", true);
                        pwd.Text = "";
                    }
                dReader.Close();
                conn.Close();
            }
            else
            {
                //lblTranStatus.Visible = true;
                //lblTranStatus.Text = "No Such User.....";
                //lblTranStatus.ForeColor = System.Drawing.Color.Red;
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('No Such User.....');", true);
            }
        }
        else
        {
            //lblTranStatus.Visible = true;
            //lblTranStatus.ForeColor = System.Drawing.Color.Red;
            //lblTranStatus.Text = "You have no permission to authorize these data!!!";
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You have no permission to authorize these data!!!');", true);
        }
    }
    protected void CancelBtn_Click(object sender, EventArgs e)
    {

    }
}
