using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using autouniv;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using Delve;

public partial class UserGroupAndPrivilege : System.Web.UI.Page
{
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
                            Session["UserType"] = userType;
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
                        cmd.CommandText =
                            "Select book_desc,company_address1,company_address2,separator_type,ShotName from gl_set_of_books where book_name='" +
                            Session["bookMAN"] + "' ";

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
            RefreshAll();
        }
    }

    private void RefreshAll()
    {
        txtGroupID.Text = txtGroupName.Text = string.Empty;
        DataTable dt = UsersManager.getShowUserInfo(" WHERE [DeleteBy] IS NULL");
        dgUserGroup.DataSource = dt;
        dgUserGroup0.DataSource = dt;
        dgUserGroup.DataBind();
        dgUserGroup0.DataBind();

        dgUserGroup0.Visible = true;
        lblGroup.Visible = lblGroupName.Visible = ModelTab1.Visible = ModelTab2.Visible = false;

        DataTable dtModules = PermisManager.getModules();
        dgModel.DataSource = dtModules;
        ViewState["Histroy"] = dtModules;
        dgModel.DataBind();
        lblGroupID.Text = "";
        ViewState["Group"] = null;
        txtGroupName.Focus();
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        Users aUsers = new Users();
        if (!string.IsNullOrEmpty(txtGroupID.Text))
        {

            aUsers.GroupID = txtGroupID.Text;
            aUsers.UserGrp = txtGroupName.Text;
            aUsers.LoginBy = Session["user"].ToString();
            UsersManager.UpdateGroupInformation(aUsers);
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Record is/are update sucessfully..!!!');", true);
            RefreshAll();
        }
        else
        {
            aUsers.GroupID = txtGroupID.Text;
            aUsers.UserGrp = txtGroupName.Text;
            aUsers.LoginBy = Session["user"].ToString();
            UsersManager.SaveGroupInformation(aUsers);
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Record is/are saved sucessfully..!!!');", true);
            RefreshAll();
        }
    }
    protected void dgUserGroup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[2].Attributes.Add("style", "display:none");
        }
    }
    protected void dgUserGroup_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtGroupID.Text = dgUserGroup.SelectedRow.Cells[2].Text;
        txtGroupName.Text = dgUserGroup.SelectedRow.Cells[1].Text;
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        Users aUsers = new Users();
        if (!string.IsNullOrEmpty(txtGroupID.Text))
        {
            aUsers.GroupID = txtGroupID.Text;
            aUsers.UserGrp = txtGroupName.Text;
            aUsers.LoginBy = Session["user"].ToString();
            UsersManager.DeleteGroupInformation(aUsers);
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Record is/are delete sucessfully..!!!');", true);
            RefreshAll();
        }
    }
    //******************* Group Privilege ************//

    protected void dgUserGroup0_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["Group"] = null;
        dgModelAdd.DataSource = null;
        dgModelAdd.DataBind();
        lblGroupName.Text = dgUserGroup0.SelectedRow.Cells[1].Text;
        lblGroupID.Text = dgUserGroup0.SelectedRow.Cells[2].Text;
        dgUserGroup0.Visible = false;
        lblGroup.Visible = lblGroupName.Visible = true;
        ModelTab1.Visible = ModelTab2.Visible = true;
        DataTable dt = PermisManager.getShowGroupPriviledge(" where t1.USER_GRP='" + lblGroupID.Text + "' Order By t2.[DESCRIPTION] ");
        if (dt.Rows.Count > 0)
        {
            ViewState["Group"] = dt;
            dgModelAdd.DataSource = dt;
            dgModelAdd.DataBind();
        }
        DataTable dttmod = (DataTable)ViewState["Histroy"];
        if (dttmod.Rows.Count > 0)
        {           
            dgModel.DataSource = dttmod;
            dgModel.DataBind();
        }
        UP1.Update();
        //UP2.Update();
        UP3.Update();
    }
    protected void dgModel_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ADD")
        {
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            DataTable dt = (DataTable)ViewState["Group"];
            if (dt == null)
            {
                DataTable dtt1 = new DataTable();
                dtt1.Columns.Add("ID", typeof(string));
                dtt1.Columns.Add("ModelName", typeof(string));
                dtt1.Columns.Add("Add", typeof(string));
                dtt1.Columns.Add("Edit", typeof(string));
                dtt1.Columns.Add("View", typeof(string));
                dtt1.Columns.Add("Delete", typeof(string));
                dtt1.Columns.Add("Authoriz", typeof(string));
                ViewState["Group"] = dtt1;
            }
            DataTable dtt = (DataTable)ViewState["Group"];
            foreach (DataRow drr in dtt.Rows)
            {
                if (drr["ID"].ToString().Equals(gvr.Cells[0].Text.ToString().Trim()))
                {
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('alrady add this moduls..!!!');", true);
                    return;
                }
            }
            DataRow dr = null;
            dr = dtt.NewRow();
            dr["ID"] = gvr.Cells[0].Text.ToString().Trim();
            dr["ModelName"] = gvr.Cells[1].Text.ToString().Trim().Replace("&", " and ");
            dr["Add"] ="Y";
            dr["Edit"] = "Y";
            dr["View"] = "Y";
            dr["Delete"] = "Y";
            dr["Authoriz"] = "Y";
            dtt.Rows.Add(dr);
            ViewState["Group"] = dtt;
           
            dgModelAdd.DataSource = dtt;
            dgModelAdd.DataBind();
            dgModelAdd.Visible = true;
            UP1.Update();
           // UP2.Update();
            UP3.Update();

            DataTable dttmod = (DataTable)ViewState["Histroy"];
            if (dttmod.Rows.Count > 0)
            {
                dgModel.DataSource = dttmod;
                dgModel.DataBind();
            }
        }
    }
    protected void dgModel_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[0].Attributes.Add("style", "display:none");
        }
        else if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Attributes.Add("style", "display:none");
            DataTable dt = (DataTable)ViewState["Group"];
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["ID"].ToString() == e.Row.Cells[0].Text)
                    {
                        e.Row.BackColor = Color.Bisque;
                        e.Row.Enabled = false;
                    }
                }
            }
        }
    }
    protected void dgModelAdd_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");
        }
    }
    protected void dgModelAdd_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["Group"] != null)
        {
            DataTable dt = (DataTable)ViewState["Group"];
            DataRow dr1 = dt.Rows[dgModelAdd.Rows[e.RowIndex].DataItemIndex];           
            dt.Rows.Remove(dr1);           
            dgModelAdd.DataSource = dt;
            ViewState["Group"] = dt;
            dgModelAdd.DataBind();

            DataTable dttmod = (DataTable)ViewState["Histroy"];
            if (dttmod.Rows.Count > 0)
            {
                dgModel.DataSource = dttmod;
                dgModel.DataBind();
            }

            UP1.Update();
            // UP2.Update();
            UP3.Update();
        }
    }
    protected void lbSave_Click(object sender, EventArgs e)
    {
        if (ViewState["Group"] != null)
        {
            DataTable dt = (DataTable)ViewState["Group"];
            PermisManager.SavePermisPrivilidge(dt, lblGroupID.Text, Session["user"].ToString());
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Record is/are saved sucessfully..!!!');", true);
        }
    }
    protected void lbDelete1_Click(object sender, EventArgs e)
    {
        PermisManager.DeletePermisPrivilidge(lblGroupID.Text);
        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Record is/are Delete sucessfully..!!!');", true);
    }
    protected void lbClear_Click(object sender, EventArgs e)
    {
        lblGroup.Visible = lblGroupName.Visible =ModelTab1.Visible=ModelTab2.Visible= false;
        dgUserGroup0.Visible = true;
        ViewState["Group"] = null;
        lblGroupID.Text = "";
    }
    protected void ddlAllowAdd_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((DropDownList)sender).NamingContainer;
        DataTable dtdtl = (DataTable)ViewState["Group"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        dr["Add"] = ((DropDownList)gvr.FindControl("ddlAllowAdd")).SelectedValue;
        //dr["Edit"] = "N";
        //dr["View"] = "N";
        //dr["Delete"] = "N";
        //dr["Authoriz"] = "N";
    }
    protected void ddlAllowEdit_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((DropDownList)sender).NamingContainer;
        DataTable dtdtl = (DataTable)ViewState["Group"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        dr["Edit"] = ((DropDownList)gvr.FindControl("ddlAllowEdit")).SelectedValue;
    }
    protected void ddlAllowView_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((DropDownList)sender).NamingContainer;
        DataTable dtdtl = (DataTable)ViewState["Group"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        dr["View"] = ((DropDownList)gvr.FindControl("ddlAllowView")).SelectedValue;
    }
    protected void ddlAllowDelete_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((DropDownList)sender).NamingContainer;
        DataTable dtdtl = (DataTable)ViewState["Group"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        dr["Delete"] = ((DropDownList)gvr.FindControl("ddlAllowDelete")).SelectedValue;
    }
    protected void ddlAllowAutho_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((DropDownList)sender).NamingContainer;
        DataTable dtdtl = (DataTable)ViewState["Group"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        dr["Authoriz"] = ((DropDownList)gvr.FindControl("ddlAllowAutho")).SelectedValue;
    }
}