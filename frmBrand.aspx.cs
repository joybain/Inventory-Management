using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Delve;

public partial class frmBrand : System.Web.UI.Page
{
    private static Permis per;
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
            DataTable dt = BrandManage.GetCompanies();
            if (dt.Rows.Count > 0)
            {
                dgMsr.DataSource = BrandManage.GetCompanies();
                dgMsr.DataBind();
            }
            else
            {
                getEmptyDtl();
            }
            
        }
    }
    private void getEmptyDtl()
    {
        
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("com_id", typeof(string));
        dtDtlGrid.Columns.Add("com_desc", typeof(string));
        dtDtlGrid.Columns.Add("check", typeof(string));   
        DataRow dr = dtDtlGrid.NewRow();        
        dtDtlGrid.Rows.Add(dr);
        dgMsr.DataSource = dtDtlGrid;       
        dgMsr.DataBind();
        dgMsr.FooterRow.Visible = true;
       // dgMsr.ShowFooter = false;
    }
    protected void dgMsr_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        dgMsr.DataSource = BrandManage.GetCompanies();
        dgMsr.EditIndex = -1;
        dgMsr.DataBind();
        dgMsr.FooterRow.Visible = false;
        dgMsr.ShowFooter = false;
    }
    protected void dgMsr_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Add")
        {
            
            dgMsr.ShowFooter = true;
            dgMsr.DataSource = BrandManage.GetCompanies();
            dgMsr.DataBind();
            ((TextBox)dgMsr.FooterRow.FindControl("txtComDesc")).Focus();
        }
        else if (e.CommandName == "Insert")
        {
            int Count = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER([BrandName])", "[Brand]", "Upper "+((TextBox)dgMsr.FooterRow.FindControl("txtComDesc")).Text.Replace("'","").ToUpper());
            if (Count > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('you alrady saved this..!!');", true);
            }
            else if (((TextBox)dgMsr.FooterRow.FindControl("txtComDesc")).Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Brand Name..!!');", true);
            }            
            else
            {
                if (per.AllowAdd == "Y")
                {
                    Brand msr = new Brand();
                    msr.CompanyId = ((TextBox)dgMsr.FooterRow.FindControl("txtComId")).Text;
                    msr.CompanyDesc = ((TextBox)dgMsr.FooterRow.FindControl("txtComDesc")).Text.Replace("'", "’");
                    if (((CheckBox)dgMsr.FooterRow.FindControl("chkSelect")).Checked)
                    { msr.Active = "True"; }
                    else { msr.Active = "False"; }
                    BrandManage.CreateCompany(msr);
                    dgMsr.ShowFooter = false;
                    dgMsr.DataSource = BrandManage.GetCompanies();
                    dgMsr.DataBind();
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Records are successfully saved!!');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                }
            }
        }
    }
    protected void dgMsr_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Brand msr = BrandManage.GetCompany(((Label)dgMsr.Rows[e.RowIndex].FindControl("lblComId")).Text);
        if (per.AllowDelete == "Y")
        {
            if (msr != null)
            {
                BrandManage.DeleteCompany(msr);
            }
            dgMsr.DataSource = BrandManage.GetCompanies();
            dgMsr.DataBind();
            dgMsr.ShowFooter = false;
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Records are successfully deleted!!');", true);
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
        }
    }
    protected void dgMsr_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgMsr.DataSource = BrandManage.GetCompanies();
        dgMsr.EditIndex = e.NewEditIndex;
        dgMsr.DataBind();
        dgMsr.ShowFooter = false;
    }
    protected void dgMsr_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int Count = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER([BrandName])", "[Brand]", ((TextBox)dgMsr.FooterRow.FindControl("txtComDesc")).Text.ToUpper());
        if (Count > 0)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('you alrady saved this..!!');", true);
        }
        else if (((TextBox)dgMsr.Rows[e.RowIndex].FindControl("txtComDesc")).Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Brand Name..!!');", true);
        }
        else
        {
            if (per.AllowEdit == "Y")
            {
                Brand msr = BrandManage.GetCompany(((TextBox)dgMsr.Rows[e.RowIndex].FindControl("txtComId")).Text);
                msr.CompanyDesc = ((TextBox)dgMsr.Rows[e.RowIndex].FindControl("txtComDesc")).Text.Replace("'", "’");
                if (((CheckBox)dgMsr.Rows[e.RowIndex].FindControl("chkSelect")).Checked)
                { msr.Active = "True"; }
                else { msr.Active = "False"; }
                BrandManage.UpdateCompany(msr);
                dgMsr.DataSource = BrandManage.GetCompanies();
                dgMsr.ShowFooter = false;
                dgMsr.EditIndex = -1;
                dgMsr.DataBind();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record are successfully updated...!!');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
            }
         }
    }
    protected void dgMsr_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((DataRowView)e.Row.DataItem)[1].ToString() == String.Empty)
            {
                e.Row.Visible = false;
            }
        }
    }
}