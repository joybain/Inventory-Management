using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Dorjibari;
using System.Data.SqlClient;
using System.Data;
using Delve;

public partial class frmDepertmentEntry : System.Web.UI.Page
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
                    cmd.CommandText = "Select ID,user_grp,description from utl_userinfo where upper(user_name)=upper('" + Session["user"].ToString().ToUpper() + "') and status='A'";
                    conn.Open();
                    dReader = cmd.ExecuteReader();
                    string wnot = "";
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            Session["userlevel"] = int.Parse(dReader["user_grp"].ToString());
                            wnot = dReader["description"].ToString();
                            Session["userID"] = dReader["ID"].ToString();
                        }
                        Session["wnote"] = wnot;

                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type from gl_set_of_books where book_name='" + Session["book"].ToString() + "' ";
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
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('" + Session.SessionID + "');", true);
            string pageName = DataManager.GetCurrentPageName();
            string modid = PermisManager.getModuleId(pageName);
            per = PermisManager.getUsrPermis(Session["user"].ToString().Trim().ToUpper(), modid);
            if (per != null && per.AllowView == "Y")
            {
                ((Label)Page.Master.FindControl("lblLogin")).Text = Session["wnote"].ToString();
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
         txtDeptName.Text = lbDeptID.Text = "";
        dgDept.DataSource = SizeManager.GetShowDeptDetails();
        dgDept.DataBind();
        txtDeptName.Focus();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        int count = IdManager.GetShowSingleValueInt("COUNT(*)", "ID", "Depertment_Type", lbDeptID.Text);
        string lo = Session["user"].ToString();
        string Log = Session["userID"].ToString();
        if (count > 0)
        {
            if (txtDeptName.Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Department..!!');", true);
                return;
            }

            int countS = IdManager.GetShowSingleValueInt1("COUNT(*)", "Delete_By is null and UPPER(Dept_Name)",
                "Depertment_Type", txtDeptName.Text.ToUpper(), "ID", Convert.ToInt32(lbDeptID.Text));
            if (countS > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('alrady saved ..!!');", true);
                return;
            }

            SizeManager.UpdateDeptInfo(lbDeptID.Text, txtDeptName.Text.Replace("'", "’"), Session["userID"].ToString());
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('update Department Information..!!');",
                true);
            RefreshAll();
        }
        else
        {
            if (txtDeptName.Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Department..!!');", true);
                return;
            }

            int countS = IdManager.GetShowSingleValueInt("COUNT(*)", "Delete_By is null and UPPER(Dept_Name)",
                " Depertment_Type", txtDeptName.Text.ToUpper());
            if (countS > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('alrady saved ..!!');", true);
                return;
            }

            SizeManager.SaveDeptInfo(txtDeptName.Text.Replace("'", "’"), Log);
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Save Department Information..!!');",
                true);
            RefreshAll();
        }
    }

    protected void dgDept_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");
        }
    }
    protected void dgDept_SelectedIndexChanged(object sender, EventArgs e)
    {
        lbDeptID.Text = dgDept.SelectedRow.Cells[1].Text;
        txtDeptName.Text = dgDept.SelectedRow.Cells[2].Text;
      
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (lbDeptID.Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Dept ID..!!');", true);
            return;
        }
        if (txtDeptName.Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Department..!!');", true);
            return;
        }

        int CheckUsedDepartment =
            IdManager.GetShowSingleValueInt("COUNT(*)", " [Item] where [DeptID]='" + lbDeptID.Text + "' ");
        if (CheckUsedDepartment > 0)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('this department you already used you can't delete this..!!');", true);
            return;
        }
        SizeManager.DeleteDeptInfo(lbDeptID.Text, Session["userID"].ToString());
        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Delete  Information..!!');", true);
        RefreshAll();
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        RefreshAll();
    }
}