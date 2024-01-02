using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;

public partial class frmMenuConfiguration : System.Web.UI.Page
{
    private static Permis per;
    ManuConfigurationManager _manuConfigurationMg=new ManuConfigurationManager();
    MenuManager menuManager = new MenuManager();
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
                        "Select user_grp,BranchId,[description],UserType,case when UserType=1 then 'Bangladesh' else 'Philippine' end AS[LoginCountry] from utl_userinfo where upper(user_name)=upper('" +
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
                            Session["BranchId"] = dReader["BranchId"].ToString();
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
        if (!IsPostBack)
        {
            try
            {
               Clear();



            }
            catch (FormatException fex)
            {
                ExceptionLogging.SendExcepToDB(fex);
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
            }
            catch (Exception ex)
            {
                Response.Redirect("Default.aspx?sid=sam");

            }

        }
       
    }
    protected void CloseButton_Click(object sender, EventArgs e)
    {
        Clear();
    }

    public void Clear()
    {
        
        ddlManuName.DataSource = menuManager.GetData("", "", "", "1");
        ddlManuName.DataValueField = "Id";
        ddlManuName.DataTextField = "Name";
        ddlManuName.DataBind();
        ddlManuName.Items.Insert(0, "");
        hfId.Value = "";
        txtName.Text = "";
        txtPath.Text = "";
        txtPriority.Text = "";
        gdvConfigruation.DataSource = _manuConfigurationMg.GetData("", "", "", "", "", "1");
        gdvConfigruation.DataBind();
        ddlParentMenuId.DataSource = _manuConfigurationMg.GetData("", "", "", "", "", "1");
        ddlParentMenuId.DataValueField = "Id";
        ddlParentMenuId.DataTextField = "Name";
        ddlParentMenuId.DataBind();
        ddlParentMenuId.Items.Insert(0, "");
    }

    public ManuConfigurationModel InputList()
    {
        ManuConfigurationModel menuModel=new ManuConfigurationModel();
        if (!string.IsNullOrEmpty(hfId.Value))
        {
            menuModel.Id = Convert.ToInt32(hfId.Value);
        }
       
        if (!string.IsNullOrEmpty(ddlManuName.SelectedValue))
        {
            menuModel.MenuId = Convert.ToInt32(ddlManuName.SelectedValue);
        }
        if (!string.IsNullOrEmpty(ddlParentMenuId.SelectedValue))
        {
            menuModel.ParentMenuId = Convert.ToInt32(ddlParentMenuId.SelectedValue);
        }
        menuModel.Name = txtName.Text;
        menuModel.Path = txtPath.Text;
       menuModel.Priority= txtPriority.Text;
        menuModel.LoginBy = "";
        return menuModel;
    }
    protected void dgColor_RowDataBound(object sender, GridViewRowEventArgs e)
    {
         try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[2].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[2].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[2].Attributes.Add("style", "display:none");
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
    protected void dgColor_SelectedIndexChanged(object sender, EventArgs e)
    {
     try
        {
         hfId.Value=gdvConfigruation.SelectedRow.Cells[1].Text.Trim();
            var data = _manuConfigurationMg.GetData(hfId.Value, "", "","","", "1");
            if (data.Rows.Count>0)
            {
                ddlManuName.DataSource = menuManager.GetData("", "", "", "1");
                ddlManuName.DataValueField = "Id";
                ddlManuName.DataTextField = "Name";
                ddlManuName.DataBind();
                ddlManuName.Items.Insert(0, "");
                try
                {
                    ddlManuName.SelectedValue = data.Rows[0]["MenuId"].ToString();
                }
                catch
                {
                  
                }
                txtName.Text = data.Rows[0]["Name"].ToString();
                txtPath.Text = data.Rows[0]["Path"].ToString();
                txtPriority.Text = data.Rows[0]["Priority"].ToString();
                ddlParentMenuId.DataSource = _manuConfigurationMg.GetData(hfId.Value, "", "", "", "", "0");
                ddlParentMenuId.DataValueField = "Id";
                ddlParentMenuId.DataTextField = "Name";
                ddlParentMenuId.DataBind();
                ddlParentMenuId.Items.Insert(0, "");

                try
                {
                    ddlParentMenuId.SelectedValue = data.Rows[0]["ParentMenuId"].ToString();

                }
                catch 
                {
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
    protected void DeleteButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (per.AllowDelete=="Y")
            {
                if (string.IsNullOrEmpty(hfId.Value))
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please Select Ficed Row..!!','INDIANRED',3);", true);

                    return;
                }
                else
                {
                    int delete = _manuConfigurationMg.Delete(InputList());
                    if (delete > 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Delete Success..!!','Green',1);", true);
                        Clear();
                        return;
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Delete Fail..!!','red',0);", true);
                        return;
                    }
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                           "jsAlert('You are not Permitted this Step..!!','red',0);", true);
                return; 
            }

        }
        catch (FormatException fex)
        {
            ExceptionLogging.SendExcepToDB(fex);
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            ExceptionLogging.SendExcepToDB(ex);
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex.Message + "');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex.Message + "');", true);

        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
         try
        {
            if (string.IsNullOrEmpty(ddlManuName.SelectedValue))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please Select Manu Name..!!','INDIANRED',3);", true);

                return; 
            }
            if (string.IsNullOrEmpty(txtName.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please Entry Name..!!','INDIANRED',3);", true);

                return;
            }
            if (string.IsNullOrEmpty(txtPath.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please Entry Path..!!','INDIANRED',3);", true);

                return;
                
            }
            //if (string.IsNullOrEmpty(txtPriority.Text))
            //{
            //    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please Entry Priority..!!','INDIANRED',3);", true);

            //    return;

            //}
            if (string.IsNullOrEmpty(hfId.Value))
            {
                if (per.AllowAdd == "Y")
                {

                    DataTable IsExist = _manuConfigurationMg.GetData("", txtName.Text, txtPath.Text, "", "", "0");
                    if (IsExist.Rows.Count > 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                            "jsAlert('This Name Or this Path  Already Exist..!!','INDIANRED',3);", true);
                        return;
                    }
                    int Save = _manuConfigurationMg.Save(InputList());
                    if (Save > 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                            "jsAlert('Save Success..!!','Green',1);", true);
                        Clear();
                        return;
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                              "jsAlert('Save Fail..!!','red',0);", true);
                        return;
                    }
                }
                else
                {
                    
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                           "jsAlert('You are not Permitted this Step..!!','red',0);", true);
                    return;
                }
            }
            else
            {
                if (per.AllowEdit=="Y")
                {
                    var IsExist = _manuConfigurationMg.GetData(hfId.Value, txtName.Text, txtPath.Text, "", "", "0");
                    if (IsExist.Rows.Count > 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('This Name Or this Path  Already Exist..!!','INDIANRED',3);", true);
                        return;
                    }
                    int Save = _manuConfigurationMg.Update(InputList());
                    if (Save > 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Update Success..!!','Green',1);", true);
                        Clear();
                        return;
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Update Fail..!!','red',0);", true);
                        return;
                    }  
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                              "jsAlert('You are not Permitted this Step..!!','red',0);", true);
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