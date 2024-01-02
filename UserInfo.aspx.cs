using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Delve;
using Delve;


public partial class UserInfo : System.Web.UI.Page
{
    public static Permis per;
    BranchSetupManager _branchMg=new BranchSetupManager();
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
            //ddlDept.Items.Clear();         
            //ddlDept.DataSource=clsBranchManager.GetBranchInfosGrid("");
            //ddlDept.DataTextField = "BranchName";
            //ddlDept.DataValueField = "ID";
            //ddlDept.DataBind();
          //  ddlDept.Items.Insert(0,"");

            //************** User Group **************//
            ddlUsrGrp.Items.Clear();
            string query = "SELECT USER_GRP,IsNull(GROUP_DESC,'') GROUP_DESC FROM UTL_GROUPINFO  order by 1 asc";
            util.PopulationDropDownList(ddlUsrGrp, "UTL_GROUPINFO", query, "GROUP_DESC", "USER_GRP");
           // ddlUsrGrp.Items.Insert(0, "");

            DataTable dt = UsersManager.GetShowUser("");
            GridView1.DataSource = dt;
            GridView1.DataBind();
            txtUserId.Focus();

            DataTable data = _branchMg.GetBranchData();
            ddlBranch.DataSource = data;
            this.ddlBranch.DataValueField = "Id";
            this.ddlBranch.DataTextField = "BranchName";
            this.ddlBranch.DataBind();
         
        }
    }
    private void clearFields()
    {
        txtUserId.Text = "";
        txtPassword.Text = "";
        txtDescription.Text = "";
        ddlUsrGrp.SelectedIndex =ddlUserType.SelectedIndex= -1;
        ddlStatus.SelectedIndex = -1;
        ddlDept.SelectedIndex = -1;
        txtEmpNo.Text = "";
        DataTable dt = UsersManager.GetShowUser("");
        GridView1.DataSource = dt;
        GridView1.DataBind();
        DataTable data =_branchMg.GetBranchData();
        ddlBranch.DataSource = data;
        this.ddlBranch.DataValueField = "Id";
        this.ddlBranch.DataTextField = "BranchName";
        
        this.ddlBranch.DataBind();
        
        txtUserId.Focus();
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        clearFields();
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (txtUserId.Text.ToString().Trim() != String.Empty)
        {
            Users usr = UsersManager.getUser(txtUserId.Text.ToString().ToUpper());
            if (usr != null)
            {
                
                if (string.IsNullOrEmpty(txtDescription.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Input Description.!!');", true);
                    return;
                }
                usr.Description = txtDescription.Text;
                if (!string.IsNullOrEmpty(txtPassword.Text))
                {
                    usr.Password = PasswordEncriptAndDecript.Encrypt(txtPassword.Text.Replace("'", "").Replace(" ", ""));
                }
                else
                {
                    usr.Password = txtPassword.Text;
                }
                usr.UserGrp = ddlUsrGrp.SelectedValue;
                usr.Status = ddlStatus.SelectedValue;
                usr.EmpNo = lblEmpID.Text;
                usr.Dept = ddlDept.SelectedValue;
                usr.UserType = ddlUserType.SelectedValue;
                usr.BranchId = ddlBranch.SelectedValue;

                UsersManager.UpdateUser(usr);
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('User updated successfully!!');", true);
                clearFields();               
            }
            else
            {
                usr = new Users();
                if (string.IsNullOrEmpty(txtUserId.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Input Password.!!');", true);
                    return;
                }
                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Input Password.!!');", true);
                    return;
                }
               
                if (string.IsNullOrEmpty(txtDescription.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Input Description.!!');", true);
                    return;
                }
                usr.UserName = txtUserId.Text;
                usr.Description = txtDescription.Text;
                usr.Password = PasswordEncriptAndDecript.Encrypt(txtPassword.Text.Replace("'", "").Replace(" ", "")); 
                usr.UserGrp = ddlUsrGrp.SelectedValue;
                usr.Status = ddlStatus.SelectedValue;
                usr.EmpNo = lblEmpID.Text;
                usr.Dept = ddlDept.SelectedValue;
                usr.UserType = ddlUserType.SelectedValue;
                usr.BranchId = ddlBranch.SelectedValue;
                UsersManager.CreateUser(usr);
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('User created successfully!!');", true);
                clearFields();                
            }
        }
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (txtUserId.Text.ToString().Trim() != String.Empty)
        {
            Users usr = UsersManager.getUser(txtUserId.Text.ToString().ToUpper());
            if (usr != null)
            {
                UsersManager.DeleteUser(usr);
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('User deleted successfully!!');", true);
                clearFields();              
            }
        }
    }
    protected void btnFind_Click(object sender, EventArgs e)
    {
        if (txtUserId.Text.ToString().Trim() != String.Empty)
        {
            Users usr = UsersManager.getUser(txtUserId.Text.ToString().ToUpper());
            if (usr != null)
            {
                txtUserId.Text = usr.UserName;
                txtDescription.Text = usr.Description;                
                ddlUsrGrp.SelectedValue = usr.UserGrp;
                ddlStatus.SelectedValue = usr.Status;
                //ddlDept.SelectedValue = usr.Dept;
                txtEmpNo.Text = usr.EmpNo;
                ddlUserType.SelectedValue = usr.UserType;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('No such user exists .... !!');", true);
                clearFields();             
            }
        }
    }   
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
     {
        clearFields(); 
        Users usr = UsersManager.getUser(GridView1.SelectedRow.Cells[1].Text);
        if (usr != null)
        {
            txtUserId.Text = usr.UserName;
            txtDescription.Text = usr.Description;
            ddlUsrGrp.SelectedValue = usr.UserGrp;
            ddlStatus.SelectedValue = usr.Status;
            if (string.IsNullOrEmpty(usr.BranchId))
            {
                usr.BranchId = "0";
            }
            this.ddlBranch.SelectedValue = usr.BranchId;
           // ddlDept.SelectedValue = usr.Dept;
            lblEmpID.Text = usr.EmpNo;
            string Name = IdManager.GetShowSingleValueString("dbo.InitCap(isnull([F_NAME],'')+' '+isnull([M_NAME],'')+' '+isnull([L_NAME],''))",
                "EMP_NO", "PMIS_PERSONNEL", usr.EmpNo);
            txtEmpNo.Text = Name;
            try
            {
                ddlUserType.SelectedValue = usr.UserType;
            }
            catch
            {
                ddlUserType.SelectedValue = null;
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('No such user exists .... !!');", true);
            clearFields();
        }
    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Reset")
        {
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            string UserName = gvr.Cells[1].Text.ToString().Trim();
            Users usr = UsersManager.getUser(UserName.ToUpper());
            if (usr != null)
            {
                usr.Password = PasswordEncriptAndDecript.Encrypt("123");
                UsersManager.UpdateUser(usr);               
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Password has changed .!!!');", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Old password is not correct .!!');", true);       
            }           
        }
    }
    protected void txtEmpNo_TextChanged(object sender, EventArgs e)
    {
        string ID = IdManager.GetShowSingleValueString("EMP_NO", "upper(EMP_NO+' - '+dbo.InitCap(isnull(F_NAME,'')+' '+isnull(M_NAME,'')+' '+isnull(L_NAME,'')))", "PMIS_PERSONNEL", txtEmpNo.Text.ToUpper());
        if (ID != "")
        {
            string EmpName = IdManager.GetShowSingleValueString("dbo.InitCap(isnull(F_NAME,'')+' '+isnull(M_NAME,'')+' '+isnull(L_NAME,''))", "upper(EMP_NO+' - '+dbo.InitCap(isnull(F_NAME,'')+' '+isnull(M_NAME,'')+' '+isnull(L_NAME,'')))", "PMIS_PERSONNEL", txtEmpNo.Text.ToUpper());
            lblEmpID.Text = ID;
            txtEmpNo.Text = EmpName;
            UP1.Update();
        }
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header |
            e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[3].Attributes.Add("style", "display:none");
            e.Row.Cells[4].Attributes.Add("style", "display:none");
        }
    }
}
