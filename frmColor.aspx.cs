using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;

public partial class frmColor : System.Web.UI.Page
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
                    string wnot = "";
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            Session["userlevel"] = int.Parse(dReader["user_grp"].ToString());
                            //Session["dept"] = dReader["dept"].ToString();
                            wnot = "Welcome " + dReader["description"].ToString();
                        }
                        Session["wnote"] = wnot;
                        Session["LoginCountry"] = dReader["LoginCountry"].ToString();
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        //cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type from gl_set_of_books where book_name='" + Session["book"].ToString() + "' ";
                        if (Convert.ToInt32(dReader["UserType"].ToString()) == 2)
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
            RefreshAll();
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        int count = IdManager.GetShowSingleValueInt("COUNT(*)", "ID", "ColorInfo", txtColorID.Text);
        if (count > 0)
        {
            if (txtColorName.Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Color..!!');", true);
                return;
            }
            int countN = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER(ColorName)", "ColorInfo", txtColorName.Text.ToUpper());
            if (countN > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('alrady saved ..!!');", true);
                return;
            }
            ColorManager.UpdateColorInfo(txtColorID.Text, txtColorName.Text);
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('update Color Information..!!');", true);
            RefreshAll();
        }
        else
        {
            if (txtColorName.Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Color..!!');", true);
                return;
            }
            ColorManager.SaveColorInfo(txtColorID.Text, txtColorName.Text);
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Save Color Information..!!');", true);
            RefreshAll();
        }
    }
    protected void DeleteButton_Click(object sender, EventArgs e)
    {
        if (txtColorID.Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter ColorID..!!');", true);
            return;
        }
        if (txtColorName.Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Color..!!');", true);
            return;
        }
        ColorManager.DeleteColorInfo(txtColorID.Text, txtColorName.Text);
        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Delete Color Information..!!');", true);
        RefreshAll();
    }
    protected void dgColor_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtColorID.Text = dgColor.SelectedRow.Cells[1].Text;
        txtColorName.Text = dgColor.SelectedRow.Cells[2].Text;
    }
    protected void CloseButton_Click(object sender, EventArgs e)
    {
        RefreshAll();
    }

    private void RefreshAll()
    {
        txtColorID.Text = txtColorName.Text = "";
        dgColor.DataSource = ColorManager.GetColorDetails();
        dgColor.DataBind();
        txtColorName.Focus();
    }
    protected void dgColor_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgColor.PageIndex = e.NewPageIndex;
        dgColor.DataSource = ColorManager.GetColorDetails();
        dgColor.DataBind();
    }
}