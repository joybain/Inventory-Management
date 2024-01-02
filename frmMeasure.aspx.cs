using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Delve;


public partial class frmMeasure : System.Web.UI.Page
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
            DataTable dt = clsMeasureManager.GetMeasures();
            if (dt.Rows.Count > 0)
            {
                dgMsr.DataSource = dt;
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
        dtDtlGrid.Columns.Add("msr_unit_code", typeof(string));
        dtDtlGrid.Columns.Add("msr_unit_desc", typeof(string));        
        DataRow dr = dtDtlGrid.NewRow();
        dtDtlGrid.Rows.Add(dr);
        dgMsr.DataSource = dtDtlGrid;
        dgMsr.DataBind();
        dgMsr.FooterRow.Visible = true;        
    }
    protected void dgMsr_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        dgMsr.DataSource = clsMeasureManager.GetMeasures();
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
            dgMsr.DataSource = clsMeasureManager.GetMeasures();
            dgMsr.DataBind();
        }
        else if (e.CommandName == "Insert")
        {
            int Count = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER([Name])", "[UOM]", ((TextBox)dgMsr.FooterRow.FindControl("txtMsrUnitDesc")).Text.Replace("'","").ToUpper());
             if (Count > 0)
             {
                 ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('you alrady saved this..!!');", true);
             }
             else if (((TextBox)dgMsr.FooterRow.FindControl("txtMsrUnitDesc")).Text == "")
             {
                 ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter UMO Name...!!');", true);
             }
             else
             {
                 if (per.AllowAdd == "Y")
                 {
                     clsMeasure msr = new clsMeasure();
                     msr.MsrUnitCode = ((TextBox)dgMsr.FooterRow.FindControl("txtMsrUnitCode")).Text;
                     msr.MsrUnitDesc = ((TextBox)dgMsr.FooterRow.FindControl("txtMsrUnitDesc")).Text;
                     clsMeasureManager.CreateMeasure(msr);
                     dgMsr.ShowFooter = false;
                     dgMsr.DataSource = clsMeasureManager.GetMeasures();
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
        if (per.AllowDelete == "Y")
        {
            clsMeasure msr = clsMeasureManager.GetMeasure(((Label)dgMsr.Rows[e.RowIndex].FindControl("lblMsrUnitCode")).Text);
            if (msr != null)
            {
                clsMeasureManager.DeleteMeasure(msr);
            }
            dgMsr.DataSource = clsMeasureManager.GetMeasures();
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
        dgMsr.DataSource = clsMeasureManager.GetMeasures();
        dgMsr.EditIndex = e.NewEditIndex;
        dgMsr.DataBind();
        dgMsr.ShowFooter = false;
    }
    protected void dgMsr_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int Count = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER([Name])", "[UOM]", ((TextBox)dgMsr.FooterRow.FindControl("txtMsrUnitDesc")).Text.ToUpper());
        if (Count > 0)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('you alrady saved this..!!');", true);
        }
        else if (((TextBox)dgMsr.Rows[e.RowIndex].FindControl("txtMsrUnitDesc")).Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter UMO Name...!!');", true);
        }
        else
        {
            if (per.AllowEdit == "Y")
            {
                clsMeasure msr = clsMeasureManager.GetMeasure(((TextBox)dgMsr.Rows[e.RowIndex].FindControl("txtMsrUnitCode")).Text);
                msr.MsrUnitCode = ((TextBox)dgMsr.Rows[e.RowIndex].FindControl("txtMsrUnitCode")).Text;
                msr.MsrUnitDesc = ((TextBox)dgMsr.Rows[e.RowIndex].FindControl("txtMsrUnitDesc")).Text;

                clsMeasureManager.UpdateMeasure(msr);
                dgMsr.DataSource = clsMeasureManager.GetMeasures();
                dgMsr.ShowFooter = false;
                dgMsr.EditIndex = -1;
                dgMsr.DataBind();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record are successfully updated!!');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
            }
         }
    }
}
