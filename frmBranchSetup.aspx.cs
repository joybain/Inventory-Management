using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Delve;

public partial class frmBranchSetup : System.Web.UI.Page
{
    private static Permis per;
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
                    cmd.CommandText = "Select BranchId,ID,user_grp,description from utl_userinfo where upper(user_name)=upper('" + Session["user"].ToString().ToUpper() + "') and status='A'";
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
                            Session["BranchId"] = dReader["BranchId"].ToString();
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

            try
            {
                string branchId = Session["BranchId"].ToString();
                 if (string.IsNullOrEmpty(branchId))
                 {
                     Response.Redirect("Default.aspx?sid=sam");

                 }
                 else
                 {
                     if (branchId!="0")
                     {
                         Response.Redirect("Default.aspx?sid=sam");

                     }
                     else
                     {
                         this.Clear();
                     }
                 }
                
            }
            catch 
            {
            Response.Redirect("Default.aspx?sid=sam");

            }
         
        }

    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(txtBranchName.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please Entry Branch Name!!','orange',2);", true);
                return;
            }
            else if (string.IsNullOrEmpty(txtAddress1.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please Entry Branch Address!!','orange',2);", true);
                return;
            }
            else
            {
             
               
                    BranchModel _branch=new BranchModel();
                    _branch.Id = this.hfId.Value;
                    _branch.BranchName=txtBranchName.Text;
                    _branch.ShortName = txtShortName.Text;
                    _branch.Address1 = txtAddress1.Text;
                    _branch.Address2 = txtAddress2.Text;
                    _branch.Phone = txtPhoneNo.Text;
                    _branch.Mobile = txtMobile.Text;
                    _branch.EMail = txtEmail.Text;
                    _branch.VatRegNo =txtVatRegNo.Text;

                    if (this.CheckBox1.Checked)
                    {
                        _branch.Status = true;
                    }
                    else
                    {
                        _branch.Status = false;
                    }

                    if (string.IsNullOrEmpty(this.hfId.Value))
                    {
                        if (per.AllowAdd == "Y")
                        {
                        int IsExist = IdManager.GetShowSingleValueInt1(
                            "Count(*)",
                            "BranchName",
                            "BranchInfo",
                            txtBranchName.Text);
                        if (IsExist > 0)
                        {
                            ScriptManager.RegisterClientScriptBlock(
                                this.Page,
                                this.Page.GetType(),
                                "alert",
                                "jsAlert('The Same Name Already Exist!!','orange',2);",
                                true);
                            return;
                        }
                        else
                        {


                            int save = _branchMg.Save(_branch);
                            if (save > 0)
                            {
                                ScriptManager.RegisterClientScriptBlock(
                                    this.Page,
                                    this.Page.GetType(),
                                    "alert",
                                    "jsAlert('Save Success!!','Green',1);",
                                    true);
                                Clear();
                                return;
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(
                                    this.Page,
                                    this.Page.GetType(),
                                    "alert",
                                    "jsAlert('Save Fail!!','red',0);",
                                    true);
                                return;
                            }
                        }

                    }
                        else
                        {
                             ScriptManager.RegisterClientScriptBlock(
                                this.Page,
                                this.Page.GetType(),
                                "alert",
                                "jsAlert('You Are Not permitted!!','red',0);",
                                true);
                            return;
                        }
            }




                    ////Update
                    else
                    {
                        if (per.AllowEdit == "Y")
                        {
                        var isexist = IdManager.GetShowSingleValueInt1(
                            "count(Id)",
                            "BranchName",
                            "BranchInfo",
                            this.txtBranchName.Text,
                            Convert.ToInt32(hfId.Value));
                        if (isexist > 0)
                        {
                            ScriptManager.RegisterClientScriptBlock(
                                this.Page,
                                this.Page.GetType(),
                                "alert",
                                "jsAlert('The Same Name Already Exist!!','orange',2);",
                                true);
                            return;
                        }
                        else
                        {
                            int save = _branchMg.Update(_branch);
                            if (save > 0)
                            {
                                ScriptManager.RegisterClientScriptBlock(
                                    this.Page,
                                    this.Page.GetType(),
                                    "alert",
                                    "jsAlert('Update Success!!','Green',1);",
                                    true);
                                Clear();
                                return;
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(
                                    this.Page,
                                    this.Page.GetType(),
                                    "alert",
                                    "jsAlert('Update Fail!!','red',0);",
                                    true);
                                return;
                            }
                        }
                    }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(
                                this.Page,
                                this.Page.GetType(),
                                "alert",
                                "jsAlert('You Are Not permitted!!','red',0);",
                                true);
                            return;
                        }
            }
                  
                
            }

        }
        catch (FormatException fex)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('" + fex.Message + "','red',0);", true);

        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Database Maintain Error. Contact to the Software Provider..!!','red',0);", true);
            else;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
        }
    }

    private void Clear()
    {
        this.gdvBranch.DataSource = this._branchMg.GetBranchData("");
        this.gdvBranch.DataBind();
        this.hfId.Value = "";
        this.txtBranchName.Text = "";
        this.txtShortName.Text = "";
        this.txtAddress1.Text = "";
        this.txtAddress2.Text = "";
        this.txtPhoneNo.Text = "";
        this.txtMobile.Text = "";
        this.txtEmail.Text = "";
        this.txtVatRegNo.Text = "";
        this.CheckBox1.Checked = true;
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        this.Clear();
    }
    protected void gdvBranch_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");
        }
    }
    protected void gdvBranch_SelectedIndexChanged(object sender, EventArgs e)
    {
        var data = this._branchMg.GetBranchData(this.gdvBranch.SelectedRow.Cells[1].Text);
        if (data.Rows.Count>0)
        {
            this.hfId.Value = gdvBranch.SelectedRow.Cells[1].Text;
            this.txtBranchName.Text = data.Rows[0]["BranchName"].ToString();
            this.txtShortName.Text = data.Rows[0]["ShortName"].ToString();

            this.txtAddress1.Text = data.Rows[0]["Address1"].ToString();
            this.txtAddress2.Text = data.Rows[0]["Address2"].ToString();

            this.txtPhoneNo.Text = data.Rows[0]["Phone"].ToString();
            this.txtMobile.Text = data.Rows[0]["Mobile"].ToString();

            this.txtEmail.Text = data.Rows[0]["EMail"].ToString();
            this.txtVatRegNo.Text = data.Rows[0]["VatRegNo"].ToString();
            var status = data.Rows[0]["Status"].ToString();
            if (status=="True")
            {
                this.CheckBox1.Checked = true;
            }
            else
            {
                this.CheckBox1.Checked = false;

            }
        }
    }
    protected void Delete_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(this.hfId.Value))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please First Select Branch!!','orange',2);", true);
                return;
            }

            else
            {
                int Delete = this._branchMg.Delete(hfId.Value,"");
                if (Delete > 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Delete Success!!','Green',1);", true);
                    Clear();
                    return;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Delete Fail!!','red',0);", true);
                    return;
                }
            }
        }
        catch (FormatException fex)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('" + fex.Message + "','red',0);", true);

        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Database Maintain Error. Contact to the Software Provider..!!','red',0);", true);
            else;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
        }
    }
}