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
using AMBs;
using Delve;

public partial class frmDesignation : System.Web.UI.Page
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
            DataTable dt = clsDesigManager.getDesigDetails("");
            dgDesig.DataSource = dt;
            dgDesig.DataBind();
            
            ddlMgrCode.Items.Clear();
            string queryDesig = "select '' desig_code, '' desig_name union select desig_code,dbo.initcap(desig_name) desig_name from pmis_desig_code order by 2 desc";
            util.PopulationDropDownList(ddlMgrCode, "Designation", queryDesig, "desig_name", "desig_code");

            //ddlGradeCode.Items.Clear();
            //string queryScale = "select '' scale_detail_id, '' scale union select convert(varchar,scale_detail_id),scale from v_scale order by 2 desc";
            //util.PopulationDropDownList(ddlGradeCode, "Scale", queryScale, "scale", "scale_detail_id");

            ddlClass.Items.Clear();
            string queryClass = "select '' class_id, '' class_name union select convert(varchar,class_id),class_name from pmis_class order by 2 desc";
            util.PopulationDropDownList(ddlClass, "Scale", queryClass, "class_name", "class_id");
            DesignationID.Visible = false;
            txtDesigName.Focus();
        }
    }
    protected void dgDesig_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        DataTable dt = clsDesigManager.getDesigDetails(txtDesigName.Text);
        dgDesig.DataSource = dt;
        dgDesig.PageIndex = e.NewPageIndex;
        dgDesig.DataBind();
    }
    protected void dgDesig_SelectedIndexChanged(object sender, EventArgs e)
    {
        clsDesig des = clsDesigManager.getDesig(dgDesig.SelectedRow.Cells[1].Text.Trim());
        if (des != null)
        {
            txtDesigCode.Text = des.DesigCode;
            txtSerial.Text = des.Serial;
            txtDesigName.Text = des.DesigName;
            txtDesigAbb.Text = des.DesigAbb;
            ddlClass.SelectedValue = des.Class;
            ddlGradeCode.SelectedValue = des.GradeCode;
            ddlMgrCode.SelectedValue = des.MgrCode;
            //ddlOfficerOrStaff.SelectedValue = des.OfficerStaff;
            //ddlTectNtech.SelectedValue = des.TechNtech;
        }
    }
    protected void BtnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtDesigName.Text))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('input designation.!!');", true);
            return;
        }
        clsDesig des = clsDesigManager.getDesig(txtDesigCode.Text);
        if (des != null)
        {
            des.DesigName = txtDesigName.Text.Replace("'", ""); ;
            des.DesigAbb = txtDesigAbb.Text;
            des.Serial = txtSerial.Text;
            des.Class = ddlClass.SelectedValue;
            des.GradeCode = ddlGradeCode.SelectedValue;
            des.MgrCode = ddlMgrCode.SelectedValue;
            des.OfficerStaff = ddlOfficerOrStaff.SelectedValue;
            des.TechNtech = ddlTectNtech.SelectedValue;
            clsDesigManager.UpdateDesig(des);
            //DataTable dt = clsDesigManager.getDesigDetails("");
            //dgDesig.DataSource = dt;
            //dgDesig.DataBind();
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Record updated successfully!!');", true);
            clearFields();
        }
        else
        {
            des = new clsDesig();
            des.DesigName = txtDesigName.Text.Replace("'", "");
            des.Serial = txtSerial.Text;
            des.DesigAbb = txtDesigAbb.Text;
            des.Class = ddlClass.SelectedValue;
            des.GradeCode = ddlGradeCode.SelectedValue;
            des.MgrCode = ddlMgrCode.SelectedValue;
            des.OfficerStaff = ddlOfficerOrStaff.SelectedValue;
            des.TechNtech = ddlTectNtech.SelectedValue;
            des.DesigCode = IdManager.GetNextID("pmis_desig_code", "desig_code").ToString();
            clsDesigManager.CreateDesig(des);
            //DataTable dt = clsDesigManager.getDesigDetails("");
            //dgDesig.DataSource = dt;
            //dgDesig.DataBind();
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Record inserted successfully!!');", true);
            clearFields();
        }
    }
    protected void BtnFind_Click(object sender, EventArgs e)
    {
        if (txtDesigCode.Text != "" && txtDesigName.Text == "")
        {
            clsDesig des = clsDesigManager.getDesig(txtDesigCode.Text);
            if (des != null)
            {
                txtDesigCode.Text = des.DesigCode;
                txtDesigName.Text = des.DesigName;
                txtDesigAbb.Text = des.DesigAbb;
                ddlClass.SelectedValue = des.Class;
                ddlGradeCode.SelectedValue = des.GradeCode;
                ddlMgrCode.SelectedValue = des.MgrCode;
                ddlOfficerOrStaff.SelectedValue = des.OfficerStaff;
                ddlTectNtech.SelectedValue = des.TechNtech;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No data found!!');", true);
            }
        }
        else if (txtDesigCode.Text == "" && txtDesigName.Text != "")
        {
            DataTable dt = clsDesigManager.getDesigDetails(txtDesigName.Text);
            dgDesig.DataSource = dt;
            dgDesig.DataBind();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select designation ID or name first!!');", true);
        }
    }
    protected void BtnDelete_Click(object sender, EventArgs e)
    {
        if (txtDesigCode.Text != "")
        {
            clsDesigManager.DeleteDesig(txtDesigCode.Text);
            clearFields();
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Record deleted successfully!!');", true);
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select designation first!!');", true);
        }
    }
    protected void BtnReset_Click(object sender, EventArgs e)
    {
        clearFields();
    }
    private void clearFields()
    {
        txtDesigCode.Text = "";
        txtDesigName.Text = "";
        txtDesigAbb.Text = "";
        ddlClass.SelectedIndex = -1;
        ddlGradeCode.SelectedIndex = -1;
        ddlMgrCode.SelectedIndex = -1;
        ddlOfficerOrStaff.SelectedIndex = -1;
        ddlTectNtech.SelectedIndex = -1;
        DataTable dt = clsDesigManager.getDesigDetails("");
        dgDesig.DataSource = dt;
        dgDesig.DataBind();
    }
    protected void dgDesig_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");
        }
    }
}
