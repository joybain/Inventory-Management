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
using Delve;

public partial class AssociatSetup : System.Web.UI.Page
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
            DataTable dt = clsAssoManager.getAssos("");
            if (dt.Rows.Count > 0)
            {
                dgAss.DataSource = dt;
                dgAss.DataBind();
            }
            else
            {
                getEmptyGrid();
            }
        }
    }
    private void getEmptyGrid()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("asso_id", typeof(string));
        dt.Columns.Add("asso_name", typeof(string));
        dt.Columns.Add("asso_abvr", typeof(string));
        DataRow dr = dt.NewRow();
        dt.Rows.Add(dr);
        dgAss.DataSource = dt;
        dgAss.DataBind();
        dgAss.ShowFooter = true;
        dgAss.FooterRow.Visible = true;
        ((TextBox)dgAss.FooterRow.FindControl("txtAssoId")).Text = "1";
    }
    protected void dgAss_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        DataTable dt = clsAssoManager.getAssos("");
        if (dt.Rows.Count > 0)
        {
            dgAss.DataSource = dt;
            dgAss.EditIndex = -1;
            dgAss.DataBind();
            dgAss.ShowFooter = false;
            dgAss.FooterRow.Visible = false;
        }
        else
        {
            getEmptyGrid();
        }
    }
    protected void dgAss_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "AddNew")
        {
            DataTable dt = clsAssoManager.getAssos("");
            dgAss.DataSource = dt;
            dgAss.DataBind();
            dgAss.ShowFooter = true;
            dgAss.FooterRow.Visible = true;
            ((TextBox)dgAss.FooterRow.FindControl("txtAssoId")).Text = (int.Parse(((Label)dgAss.Rows[dgAss.Rows.Count - 1].Cells[0].FindControl("lblAssoId")).Text) + 1).ToString();
            ((TextBox)dgAss.FooterRow.FindControl("txtAssoName")).Focus();
        }
        else if (e.CommandName == "Insert")
        {
            if (per.AllowAdd == "Y")
            {
                clsAsso asso = new clsAsso();
                asso.AssoId = ((TextBox)dgAss.FooterRow.FindControl("txtAssoId")).Text;
                asso.AssoName = ((TextBox)dgAss.FooterRow.FindControl("txtAssoName")).Text;
                asso.AssoAbvr = ((TextBox)dgAss.FooterRow.FindControl("txtAssoAbvr")).Text;
                clsAssoManager.CreateAsso(asso);
                DataTable dt = clsAssoManager.getAssos("");
                if (dt.Rows.Count > 0)
                {
                    dgAss.DataSource = dt;
                    dgAss.EditIndex = -1;
                    dgAss.DataBind();
                }
                else
                {
                    getEmptyGrid();
                }
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Green;
                lblTranStatus.Text = "Records Created Successfully";
            }
            else
            {
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
                lblTranStatus.Text = "You have no enough permission to create record here.";
            }
        }
    }
    protected void dgAss_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            clsAssoManager.DeleteAsso(((Label)dgAss.Rows[e.RowIndex].FindControl("lblAssoId")).Text);
            DataTable dt = clsAssoManager.getAssos("");
            if (dt.Rows.Count > 0)
            {
                dgAss.DataSource = dt;
                dgAss.EditIndex = -1;
                dgAss.DataBind();
                dgAss.ShowFooter = false;
                dgAss.FooterRow.Visible = false;
            }
            else
            {
                getEmptyGrid();
            }
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Records Deleted Successfully!!');", true);
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You have no enough permission to delete record here!!');", true);
        }
    }
    protected void dgAss_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DataTable dt = clsAssoManager.getAssos("");
        dgAss.DataSource = dt;
        dgAss.EditIndex = e.NewEditIndex;
        dgAss.DataBind();
    }
    protected void dgAss_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (per.AllowEdit == "Y")
        {
            clsAsso asso = clsAssoManager.getAsso(((TextBox)dgAss.Rows[e.RowIndex].FindControl("txtAssoId")).Text);
            if (asso != null)
            {
                asso.AssoName = ((TextBox)dgAss.Rows[e.RowIndex].FindControl("txtAssoName")).Text;
                asso.AssoAbvr = ((TextBox)dgAss.Rows[e.RowIndex].FindControl("txtAssoAbvr")).Text;
                clsAssoManager.UpdateAsso(asso);
            }
            DataTable dt = clsAssoManager.getAssos("");
            if (dt.Rows.Count > 0)
            {
                dgAss.DataSource = dt;
                dgAss.EditIndex = -1;
                dgAss.DataBind();
            }
            else
            {
                getEmptyGrid();
            }
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Green;
            lblTranStatus.Text = "Records Update Successfully";
        }
        else
        {
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
            lblTranStatus.Text = "You have no enough permission to change record here.";
        }
    }
    protected void dgAss_RowDataBound(object sender, GridViewRowEventArgs e)
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
