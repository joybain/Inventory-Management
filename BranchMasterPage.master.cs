using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Delve;
using System.Data;

public partial class BranchMasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {

        Session["Changed"] = false;
        Page.Header.DataBind();
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
                            wnot = "Welcome " + dReader["description"].ToString();
                        }

                        Session["wnote"] = wnot;
                        Session["LoginCountry"] = "";
                        //Session["LoginCountry"] = dReader["LoginCountry"].ToString();
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText =
                            "Select book_desc,company_address1,company_address2,separator_type,ShotName from gl_set_of_books where book_name='" +
                            Session["book"].ToString() + "' ";
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
                    }

                    dReader.Close();
                    conn.Close();
                }
            }
        }
    }

    protected override void AddedControl(Control control, int index)
    {
        if (Request.ServerVariables["http_user_agent"].IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) !=
            -1)
            this.Page.ClientTarget = "uplevel";

        base.AddedControl(control, index);
    }

    protected void lbLogout_Click(object sender, EventArgs e)
    {
        clsSessionManager.DeleteSession(Session.SessionID);
        Session.RemoveAll();
        Session.Abandon();
        Response.Redirect("~/default.aspx?sid=sam");
    }

    protected void lbHome_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/BranchHome.aspx");
    }
}

