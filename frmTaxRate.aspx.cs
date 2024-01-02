using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Delve;

public partial class frmTaxRate : System.Web.UI.Page
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
        if (!IsPostBack)
        {
            DataTable dt = TaxManager.GetShowTaxInformation();
            if (dt.Rows.Count > 0)
            {
                dgTax.DataSource = dt;
                dgTax.DataBind();
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
        dtDtlGrid.Columns.Add("TaxCode", typeof(string));
        dtDtlGrid.Columns.Add("TaxType", typeof(string));
        dtDtlGrid.Columns.Add("TaxRate", typeof(string));
        dtDtlGrid.Columns.Add("check", typeof(string));        
        DataRow dr = dtDtlGrid.NewRow();
       
        dtDtlGrid.Rows.Add(dr);
        dgTax.DataSource = dtDtlGrid;
        dgTax.DataBind();
    }
    protected void dgTax_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        dgTax.DataSource = TaxManager.GetShowTaxInformation();
        dgTax.EditIndex = -1;
        dgTax.DataBind();
        dgTax.FooterRow.Visible = false;
        dgTax.ShowFooter = false;
    }
    protected void dgTax_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Add")
        {
            dgTax.ShowFooter = true;
            dgTax.DataSource = TaxManager.GetShowTaxInformation();
            dgTax.DataBind();
        }
        else if (e.CommandName == "Insert")
        {
            int Count = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER([Name])", "[TaxCategory]", ((TextBox)dgTax.FooterRow.FindControl("txtTaxType")).Text.ToUpper());
             if (Count > 0)
             {
                 ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('you alrady saved this..!!');", true);
             }
             else if (((TextBox)dgTax.FooterRow.FindControl("txtTaxType")).Text == "")
             {
                 ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Tax Type..!!');", true);
             }
             else
             {
                 if (per.AllowAdd == "Y")
                 {
                     string Code, Type, Rate, Chk, LoginBy;
                     Code = ((TextBox)dgTax.FooterRow.FindControl("txtTaxCode")).Text;
                     Type = ((TextBox)dgTax.FooterRow.FindControl("txtTaxType")).Text;
                     Rate = ((TextBox)dgTax.FooterRow.FindControl("txtTaxRate")).Text;
                     if (((CheckBox)dgTax.FooterRow.FindControl("chkSelect")).Checked)
                     { Chk = "True"; }
                     else { Chk = "False"; }
                     LoginBy = "";
                     TaxManager.CreateTax(Code, Type, Rate, Chk, LoginBy);
                     dgTax.ShowFooter = false;
                     dgTax.DataSource = TaxManager.GetShowTaxInformation();
                     dgTax.DataBind();
                     ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Records are successfully saved!!');", true);
                 }
                 else
                 {
                     ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                 }
             }
        }
    }
    protected void dgTax_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            TaxManager.DeleteTax(((Label)dgTax.Rows[e.RowIndex].FindControl("lblTaxCode")).Text);
            DataTable dt = TaxManager.GetShowTaxInformation();
            if (dt.Rows.Count > 0)
            {
                dgTax.DataSource = dt;
                dgTax.DataBind();
                dgTax.ShowFooter = false;
            }
            else { getEmptyDtl(); }
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Records are successfully deleted!!');", true);
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
        }
    }
    protected void dgTax_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgTax.DataSource = TaxManager.GetShowTaxInformation();
        dgTax.EditIndex = e.NewEditIndex;
        dgTax.DataBind();
        dgTax.ShowFooter = false;
    }
    protected void dgTax_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (per.AllowEdit == "Y")
        {
            int Count = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER([Name])", "[TaxCategory]", ((TextBox)dgTax.FooterRow.FindControl("txtTaxType")).Text.ToUpper());
            if (Count > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('you alrady saved this..!!');", true);
            }
            else if (((TextBox)dgTax.Rows[e.RowIndex].FindControl("txtTaxType")).Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Tax Type..!!');", true);
            }
            else
            {
                string Code, Type, Rate, Chk, LoginBy;
                Code = ((TextBox)dgTax.Rows[e.RowIndex].FindControl("txtTaxCode")).Text;
                Type = ((TextBox)dgTax.Rows[e.RowIndex].FindControl("txtTaxType")).Text;
                Rate = ((TextBox)dgTax.Rows[e.RowIndex].FindControl("txtTaxRate")).Text;
                if (((CheckBox)dgTax.Rows[e.RowIndex].FindControl("chkSelect")).Checked)
                { Chk = "True"; }
                else { Chk = "False"; }
                LoginBy = "";
                TaxManager.UpdateTax(Code, Type, Rate, Chk, LoginBy);

                dgTax.DataSource = TaxManager.GetShowTaxInformation();
                dgTax.ShowFooter = false;
                dgTax.EditIndex = -1;
                dgTax.DataBind();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record are successfully updated!!');", true);
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
        }
    }
    protected void dgTax_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
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
}