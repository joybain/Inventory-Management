using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;

public partial class ItemsCatagoryInformation : System.Web.UI.Page
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

        if (!IsPostBack)
        {
            try
            {
                ddlDepart.DataSource = SizeManager.GetShowDeptDetails();
                ddlDepart.DataTextField = "Dept_Name";
                ddlDepart.DataValueField = "ID";
                ddlDepart.DataBind();
                ddlDepart.Items.Insert(0, "");
                Session["TreeNode"] = "";
                txtDesc.Enabled = ddlCatagory.Enabled = txtItemDesc.Enabled = CheckBox1.Enabled = false;
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
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (Session["TreeNode"].ToString() == "C")
            {
                MajorCategory aMajorCategory = MajorCategoryManager.GetMajorCat(lblID.Text);
               
                if (!string.IsNullOrEmpty(lblID.Text))
                {
                    if (txtItemDesc.Text == "")
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Category Name..!!');", true);
                    }

                    if (per.AllowEdit == "Y")
                    {
                        int Counts = IdManager.GetShowSingleValueInt1("COUNT(*)", "DeleteBy is null and UPPER([Name])",
                            "[Category]", txtItemDesc.Text.ToUpper(), "ID", Convert.ToInt32(lblID.Text));
                        if (Counts > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('this category already exist..!!');", true);
                        }
                        aMajorCategory.Code = txtItemCode.Text;
                        aMajorCategory.ID = lblID.Text;
                        aMajorCategory.Name = txtItemDesc.Text.Replace("'", "’");
                        aMajorCategory.Description = txtDesc.Text.Replace("'", "’");
                        aMajorCategory.DeptID = ddlDepart.SelectedValue;
                        if (CheckBox1.Checked)
                        {
                            aMajorCategory.Active = "1";
                        }
                        else
                        {
                            aMajorCategory.Active = "0";
                        }

                        try
                        {
                            aMajorCategory.LoginBy = Session["user"].ToString();
                        }
                        catch
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                                "alert('Session Out please login again..!!');", true);
                            return;
                        }

                        MajorCategoryManager.UpdateMajorCat(aMajorCategory);
                        Catagory();
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                            "alert('Category update Successfully....!!');", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                    }
                }
                else
                {

                  
                        if (per.AllowAdd == "Y")
                        {
                            int Count = IdManager.GetShowSingleValueInt("COUNT(*)",
                                "DeleteBy is null and UPPER([Name])", "[Category]", txtItemDesc.Text.ToUpper());
                            if (Count > 0)
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                    "alert('this category already exist..!!');", true);
                                return;
                            }
                            txtItemCode.Text = IdManager.GetTransSl1("Category", "Code");
                            aMajorCategory = new MajorCategory();
                            aMajorCategory.Code = txtItemCode.Text;
                            aMajorCategory.ID = lblID.Text;
                            aMajorCategory.Name = txtItemDesc.Text.Replace("'", "’");
                            aMajorCategory.Description = txtDesc.Text.Replace("'", "’");
                            aMajorCategory.DeptID = ddlDepart.SelectedValue;
                            if (CheckBox1.Checked)
                            {
                                aMajorCategory.Active = "1";
                            }
                            else { aMajorCategory.Active = "0"; }

                            try
                            {
                                aMajorCategory.LoginBy = Session["user"].ToString();
                            }
                            catch
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                                    "alert('Session Out please login again..!!');", true);
                                return;
                            }

                            MajorCategoryManager.CreateMajorCat(aMajorCategory);
                            Catagory();
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                                "alert('Category saved Successfully....!!');", true);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                        }
                    
                }
            }
            else if (Session["TreeNode"].ToString() == "SC")
            {
                SubMajorCategory aSubMajorCategory = SubMajorCategoryManager.GetSubMajorCat(lblID.Text);

                if (aSubMajorCategory != null)
                {

                    int Counts = IdManager.GetShowSingleValueInt1("COUNT(*)", "DeleteBy Is Null and UPPER([Name])",
                        "[SubCategory]", txtItemDesc.Text.ToUpper(), "ID", Convert.ToInt32(lblID.Text));
                    if (Counts > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('you already saved this..!!');", true);
                    }
                    else if (txtItemDesc.Text == "")
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('Enter Sub Category Name..!!');", true);
                    }

                    if (per.AllowEdit == "Y")
                    {
                        aSubMajorCategory.Code = txtItemCode.Text;
                        aSubMajorCategory.ID = lblID.Text;
                        aSubMajorCategory.Name = txtItemDesc.Text;
                        aSubMajorCategory.Catagory = ddlCatagory.SelectedValue;
                        aSubMajorCategory.Description = txtDesc.Text;
                        try
                        {
                            aSubMajorCategory.DeptID = ddlDepart.SelectedValue;
                        }
                        catch
                        {
                            aSubMajorCategory.DeptID = null;
                        }

                        if (CheckBox1.Checked)
                        {
                            aSubMajorCategory.Active = "1";
                        }
                        else
                        {
                            aSubMajorCategory.Active = "0";
                        }

                        try
                        {
                            aSubMajorCategory.LoginBy = Session["user"].ToString();
                        }
                        catch
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                                "alert('Session Out please login again..!!');", true);
                            return;
                        }

                        SubMajorCategoryManager.UpdateSubMajorCat(aSubMajorCategory);
                        RefreshAllSubCatagory();
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                            "alert('Sub category update successfully....!!');", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('You are not Permitted this Step...!!');", true);
                    }
                }
                else
                {

                    if (aSubMajorCategory == null)
                    {
                        if (per.AllowAdd == "Y")
                        {
                            int Count = IdManager.GetShowSingleValueInt("COUNT(*)",
                                "DeleteBy Is Null and UPPER([Name])", "[SubCategory]", txtItemDesc.Text.ToUpper());
                            if (Count > 0)
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                    "alert('this sub Category already exist..!!');", true);
                                return;
                            }
                            txtItemCode.Text = IdManager.GetTransSl1("SubCategory", "Code");
                            aSubMajorCategory = new SubMajorCategory();
                            aSubMajorCategory.Code = txtItemCode.Text;
                            aSubMajorCategory.Code = txtItemCode.Text;
                            aSubMajorCategory.ID = lblID.Text;
                            aSubMajorCategory.Name = txtItemDesc.Text;
                            aSubMajorCategory.Catagory = ddlCatagory.SelectedValue;
                            aSubMajorCategory.Description = txtDesc.Text;
                            aSubMajorCategory.DeptID = ddlDepart.SelectedValue;
                            if (CheckBox1.Checked)
                            {
                                aSubMajorCategory.Active = "1";
                            }
                            else
                            {
                                aSubMajorCategory.Active = "0";
                            }

                            try
                            {
                                aSubMajorCategory.LoginBy = Session["user"].ToString();
                            }
                            catch
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                                    "alert('Session Out please login again..!!');", true);
                                return;
                            }

                            SubMajorCategoryManager.CreateSubMajorCat(aSubMajorCategory);
                            RefreshAllSubCatagory();
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                                "alert('Sub Category Saved Successfully..!!');", true);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                "alert('You are not Permitted this Step..!!');", true);
                        }
                    }
                }
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
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            if (Session["TreeNode"].ToString() == "C")
            {
                if (per.AllowDelete == "Y")
                {
                    MajorCategory aMajorCategory = MajorCategoryManager.GetMajorCat(lblID.Text);
                    if (aMajorCategory != null)
                    {
                        aMajorCategory.ID = lblID.Text;
                        MajorCategoryManager.DeleteMajorCat(aMajorCategory);
                        Catagory();
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Category Delete Successfully....!!');", true);
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                }
            }
            else if (Session["TreeNode"].ToString() == "SC")
            {
                if (per.AllowDelete == "Y")
                {
                    SubMajorCategory aSubMajorCategory = SubMajorCategoryManager.GetSubMajorCat(lblID.Text);
                    if (aSubMajorCategory != null)
                    {
                        aSubMajorCategory.ID = lblID.Text;

                        SubMajorCategoryManager.DeleteSubMajorCat(aSubMajorCategory);
                        SubCatagory();
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Sub Category Delete Successfully....!!');", true);
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                }
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
    protected void btnClear_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
    {
        try
        {
            txtDesc.Enabled = ddlCatagory.Enabled = txtItemDesc.Enabled = CheckBox1.Enabled = true;
            if (TreeView1.SelectedNode.Value == "C")
            {
                Catagory();
            }
            else if (TreeView1.SelectedNode.Value == "SC")
            {
                SubCatagory();
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Please Select Category or Sub Category First ....!!');", true);
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

    private void SubCatagory()
    {
        Session["TreeNode"] = "SC";
        btnSave.Visible = true;        
        txtItemDesc.Text = "";      
        lblddlCat.Visible = true;
        ddlCatagory.Visible = true;
        lblCatagory.Text = "Sub Category";
        dgCatagory.Visible = false;
        dgSubCatagory.Visible = true;
        DataTable dtSubCatagory = SubMajorCategoryManager.getshowSubCatagory("");
        dgSubCatagory.DataSource = dtSubCatagory;
        ViewState["dtSubCatagory"] = dtSubCatagory;
        dgSubCatagory.DataBind();
        ddlCatagory.DataSource = MajorCategoryManager.GetMajorCats("");
        ddlCatagory.DataTextField = "mjr_desc";
        ddlCatagory.DataValueField = "mjr_code";
        ddlCatagory.DataBind();
        ddlCatagory.Items.Insert(0, new ListItem(""));
        ddlDepart.Items.Clear();

        txtDesc.Text = "";
        txtItemCode.Text =lblID.Text= "";
        txtItemDesc.Focus();
        UPCatagory.Update();
        UPDetails.Update();
        UPSubCat.Update();
        UPTree.Update();
    }

    private void Catagory()
    {
        Session["TreeNode"] = "C";
        btnSave.Visible = true;
        txtItemDesc.Text = "";
        lblddlCat.Visible = false;
        ddlCatagory.Visible = false;
        lblCatagory.Text = "Category";
        dgCatagory.Visible = true;
        dgSubCatagory.Visible = false;
        dgCatagory.DataSource = MajorCategoryManager.GetMajorCats("");
        dgCatagory.DataBind();
        txtDesc.Text = "";
        txtItemCode.Text =lblID.Text= "";

        ddlDepart.DataSource = SizeManager.GetShowDeptDetails();
        ddlDepart.DataTextField = "Dept_Name";
        ddlDepart.DataValueField = "ID";
        ddlDepart.DataBind();
        ddlDepart.Items.Insert(0, "");

        txtItemDesc.Focus();
        UPCatagory.Update();
        UPDetails.Update();
        UPSubCat.Update();
        UPTree.Update();
    }

    private void RefreshAllSubCatagory()
    {
        Session["TreeNode"] = "SC";
        btnSave.Visible = true;
        txtItemDesc.Text = "";
        lblddlCat.Visible = true;
        ddlCatagory.Visible = true;
        lblCatagory.Text = "Sub Category";
        dgCatagory.Visible = false;
        dgSubCatagory.Visible = true;
        DataTable dtSubCatagory = SubMajorCategoryManager.getshowSubCatagory("");
        dgSubCatagory.DataSource = dtSubCatagory;
        ViewState["dtSubCatagory"] = dtSubCatagory;
        dgSubCatagory.DataBind();
        //ddlCatagory.DataSource = MajorCategoryManager.GetMajorCats("");
        //ddlCatagory.DataTextField = "mjr_desc";
        //ddlCatagory.DataValueField = "mjr_code";
        //ddlCatagory.DataBind();
        //ddlCatagory.Items.Insert(0, new ListItem(""));
        txtDesc.Text = "";
        txtItemCode.Text = lblID.Text = "";
        txtItemDesc.Focus();
        UPCatagory.Update();
        UPDetails.Update();
        UPSubCat.Update();
        UPTree.Update();
    }

    protected void dgCatagory_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            lblID.Text = dgCatagory.SelectedRow.Cells[1].Text;
            HfdCat.Value = dgCatagory.SelectedRow.Cells[1].Text;
            try
            {
                if (dgCatagory.SelectedRow.Cells[7].Text != "&nbsp;")
                {
                    ddlDepart.Text = dgCatagory.SelectedRow.Cells[7].Text;
                }
            }
            catch
            {
                ddlDepart.Text = null;
            }
            txtItemDesc.Text = dgCatagory.SelectedRow.Cells[3].Text;
            
            if (dgCatagory.SelectedRow.Cells[5].Text == "True")
            {
                CheckBox1.Checked = true;
            }
            else
            {
                CheckBox1.Checked = false;
            }

            txtItemCode.Text = dgCatagory.SelectedRow.Cells[6].Text;
            if (dgCatagory.SelectedRow.Cells[4].Text == "&nbsp;")
            {
                txtDesc.Text = "";
            }
            else
            {
                txtDesc.Text = dgCatagory.SelectedRow.Cells[4].Text;
            }
            UPCatagory.Update();
            UPDetails.Update();
            UPSubCat.Update();
            UPTree.Update();
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

    protected void dgSubCatagory_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            lblID.Text = dgSubCatagory.SelectedRow.Cells[1].Text;
            HfdSubCat.Value = dgSubCatagory.SelectedRow.Cells[1].Text;
            txtItemDesc.Text = dgSubCatagory.SelectedRow.Cells[4].Text;
            ddlCatagory.SelectedValue = dgSubCatagory.SelectedRow.Cells[7].Text;
            try
            {
                ddlDepart.SelectedValue = dgSubCatagory.SelectedRow.Cells[9].Text;
            }
            catch
            {
                ddlDepart.SelectedValue = null;
            }

            if (dgSubCatagory.SelectedRow.Cells[9].Text.Equals("&nbsp;"))
            {
                int DepartmentID = IdManager.GetShowSingleValueInt("[DeptID]",
                    " [Category] where [ID]='" + ddlCatagory.SelectedValue + "' ");
                try
                {
                    ddlDepart.SelectedValue = DepartmentID.ToString();
                }
                catch
                {
                    ddlDepart.SelectedValue = null;
                }
            }
            if (dgSubCatagory.SelectedRow.Cells[5].Text == "&nbsp;")
            {
                txtDesc.Text = "";
            }
            else
            {
                txtDesc.Text = dgSubCatagory.SelectedRow.Cells[5].Text;
            }

            if (dgSubCatagory.SelectedRow.Cells[6].Text == "True")
            {
                CheckBox1.Checked = true;
            }
            else
            {
                CheckBox1.Checked = false;
            }
            txtItemCode.Text = dgSubCatagory.SelectedRow.Cells[10].Text;
            UPCatagory.Update();
            UPDetails.Update();
            UPSubCat.Update();
            UPTree.Update();
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
    protected void dgSubCatagory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[7].Attributes.Add("style", "display:none");
                e.Row.Cells[6].Attributes.Add("style", "display:none");
                e.Row.Cells[8].Attributes.Add("style", "display:none");
                e.Row.Cells[9].Attributes.Add("style", "display:none");
                e.Row.Cells[10].Attributes.Add("style", "display:none");
                e.Row.Cells[1].Attributes.Add("style", "display:none");
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
    protected void dgCatagory_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgCatagory.DataSource = MajorCategoryManager.GetMajorCats("");
        dgCatagory.PageIndex = e.NewPageIndex;
        dgCatagory.DataBind();
    }
    protected void dgSubCatagory_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgSubCatagory.DataSource = SubMajorCategoryManager.getshowSubCatagory("");
        dgSubCatagory.PageIndex = e.NewPageIndex;
        dgSubCatagory.DataBind();
    }
    protected void dgCatagory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[6].Attributes.Add("style", "display:none");
            e.Row.Cells[7].Attributes.Add("style", "display:none");
            e.Row.Cells[1].Attributes.Add("style", "display:none");
        }  
    }
    protected void ddlCatagory_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(ddlCatagory.SelectedValue !=null)
        {
            DataTable dtSubCatagory = SubMajorCategoryManager.getshowSubCatagory(ddlCatagory.SelectedValue);
            dgSubCatagory.DataSource = dtSubCatagory;
            ViewState["dtSubCatagory"] = dtSubCatagory;
            dgSubCatagory.DataBind();

            int DepartmentID = IdManager.GetShowSingleValueInt("[DeptID]",
                " [Category] where [ID]='" + ddlCatagory.SelectedValue + "' ");
            try
            {
                ddlDepart.DataSource = SizeManager.GetShowDeptDetails();
                ddlDepart.DataTextField = "Dept_Name";
                ddlDepart.DataValueField = "ID";
                ddlDepart.DataBind();
                ddlDepart.Items.Insert(0, "");

                ddlDepart.SelectedValue = DepartmentID.ToString();
            }
            catch
            {
                ddlDepart.SelectedValue = null;
            }

            UPSubCat.Update();
        }
    }
    //protected void ddlDepart_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (TreeView1.SelectedNode.Value == "C")
    //    {
    //        DataTable dtSetupInfo = ClsItemDetailsManager.GetSetupItemInfo(ddlDepart.SelectedValue);
    //        dgCatagory.DataSource = dtSetupInfo;
    //        dgCatagory.DataBind();
    //        dgCatagory.Caption = "<h1>Total Product : " + (Convert.ToInt32(dgCatagory.Rows.Count)).ToString() + "</h1>";
    //        UPCatagory.Update();
    //    }
    //    else if (TreeView1.SelectedNode.Value == "SC")
    //    {
    //        DataTable dtSetupInfo = ClsItemDetailsManager.GetSetupItemInfo(ddlDepart.SelectedValue);
    //        dgSubCatagory.DataSource = dtSetupInfo;
    //        dgSubCatagory.DataBind();
    //        dgSubCatagory.Caption = "<h1>Total Product : " + (Convert.ToInt32(dgSubCatagory.Rows.Count)).ToString() + "</h1>";
    //        UPSubCat.Update();
    //    }
    //    else
    //    {
    //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Please Select Catagory or SubCatagory First ....!!');", true);
    //    }
    //    UPDetails.Update();
    //}
    protected void ddlDepart_SelectedIndexChanged(object sender, EventArgs e)
    {

        if (TreeView1.SelectedNode != null)
        {
            if (TreeView1.SelectedNode.Value == "C")
            {
                dgCatagory.DataSource = MajorCategoryManager.GetMajorCats(ddlDepart.SelectedValue);
                dgCatagory.DataBind();
            }
            UPCatagory.Update();
            UPDetails.Update();
            UPSubCat.Update();
            UPTree.Update();
        }
        else
        {
            var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
            Response.Redirect(pageName);
        }
       
        
    }
}