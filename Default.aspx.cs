using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Delve;

public partial class _Default : System.Web.UI.Page
{
    public int userLvl = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["sid"] != null)
        {
            string RepType = Request.QueryString["sid"].ToString();
            if (RepType != "sam")
            {
                Response.Redirect("Default.aspx?sid=sam");
                //ClientScript.RegisterStartupScript(this.GetType(), "ale", "closeWindowNoPrompt();", true);
            }
        }
        else
        {
            Response.Redirect("Default.aspx?sid=sam");
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "closeWindowNoPrompt();", true);
        }

        if (!Page.IsPostBack)
        {
            ddlBook.Items.Clear();
            string queryBook =
                "select 'AMB' book_name, '' book_desc   union select '*' book_name, 'New GL' book_desc  union select book_name,book_name book_desc from gl_set_of_books where book_status='A' order by 2 desc";
            util.PopulationDropDownList(ddlBook, "level", queryBook, "book_desc", "book_name");
            ddlBook.SelectedValue = "AMB";
            ddlBook.Visible = false;
            //lbClose.Attributes.Add("onclick", "opener=self;window.close()");
            System.Type oType = System.Type.GetTypeFromProgID("InternetExplorer.Application");
            txtUserName.Focus();
        }
    }

    protected void LoginBtn_Click(object sender, EventArgs e)
    {
        System.Threading.Thread.Sleep(3000);
        if (ddlBook.SelectedValue != "")
        {
            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                "Select ID,password,user_grp,description,BranchId,UserType,case when UserType=1 then 'Bangladesh' else 'Philippine' end AS[LoginCountry] from utl_userinfo where upper(user_name)=upper('" +
                txtUserName.Text.Trim() + "') and status='A'";
            conn.Open();
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())

                    if (txtPassword.Text != "" && txtPassword.Text.Trim() ==
                        PasswordEncriptAndDecript.Decrypt(dReader["password"].ToString()))
                    {
                        Session["user"] = txtUserName.Text;
                        Session["pass"] = txtPassword.Text;
                        userLvl = int.Parse(dReader["user_grp"].ToString());
                        Session["userlevel"] = userLvl.ToString();
                        Session["userID"] = dReader["ID"].ToString();
                        Session["BranchId"] = dReader["BranchId"].ToString();
                        Session["book"] = ddlBook.SelectedValue;
                        string wnote = dReader["description"].ToString();
                        Session["wnote"] = wnote;
                        Session["LoginCountry"] = dReader["LoginCountry"].ToString();
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        string book = "";
                        int flag = IdManager.GetShowSingleValueInt("UserType", "USER_NAME", "UTL_USERINFO",
                            Session["user"].ToString());
                        Session["UserType"] = flag;
                        if (flag == 2)
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
                                Session["ShotName"] = dReader["ShotName"].ToString();
                            }
                        }

                        dReader.Close();
                        clsSession ses = clsSessionManager.getLoginSession(txtUserName.Text.ToUpper(), "");
                        if (ses == null)
                        {
                            ses = new clsSession();
                            ses.UserId = txtUserName.Text.ToUpper();
                            ses.SessionTime = System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                            ses.SessionId = Session.SessionID;

                            ses.Mac = Server.HtmlEncode(Request.UserHostAddress);
                            clsSessionManager.CreateSession(ses);
                        }

                        if (Session["userlevel"].ToString().Contains("6"))
                        {
                            Response.Redirect("~/BranchHome.aspx");
                        }
                        else
                        {
                            Response.Redirect("~/Home.aspx");
                        }

                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                            "alert('Incorrect Password....!!!!');", true);
                    }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                    "alert('Incorrect User ID ...!!!!');", true);
                Session["user"] = "";
                Session["pass"] = "";
                Session["EMPNO"] = "";
                txtUserName.Focus();
            }
        }
        else
        {
            Session["user"] = "";
            Session["pass"] = "";
            txtUserName.Focus();
        }

    }
}