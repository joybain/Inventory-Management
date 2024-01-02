using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;

public partial class ItemsInformation : System.Web.UI.Page
{
    private byte[] ItemsPhoto;
    private static Permis per;
    MajorCategoryManager _aMajorCategoryManager=new MajorCategoryManager();
    ClsItemDetailsManager _aClsItemDetailsManager=new ClsItemDetailsManager();
    ItemManager _aItemManager=new ItemManager();
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
                RefreshAll();
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

    private void RefreshAll()
    {
        lblID.Text = "";
        tabVch.ActiveTabIndex = 0;
        txtCode.Text = "";
        imgEmp.ImageUrl = "S00003_guard1.jpg";
        ddlCatagory.DataSource = MajorCategoryManager.GetMajorCats("");
        ddlCatagory.DataTextField = "mjr_desc";
        ddlCatagory.DataValueField = "mjr_code";
        ddlCatagory.DataBind();       
       // ddlCatagory.Items.Insert(0, "");
        CheckBox0.Checked = dgHistory.Visible =  true;
        txtStyleNo.Text = "";
        txtName.Text = "";
        txtBrand.Text = "";
        txtOpeningStock.Text =
            txtOpeningAmount.Text =
                txtClosingStock.Text =
                    txtClosingAmount.Text = "0"; //txtDiscountAmount.Text = txtUnitPrice.Text = txtDiscountAmount.Text = "0";
        ddlCatagory.SelectedIndex = ddlSubCatagory.SelectedIndex = ddlCurrency.SelectedIndex = ddlTextCatagory.SelectedIndex = ddlUmo.SelectedIndex = -1;
        dgHistory.DataSource = ClsItemDetailsManager.getShowItemsHistoryDetails("");
        dgHistory.DataBind();

        ddlDepart.DataSource = SizeManager.GetShowDeptDetails();
        ddlDepart.DataTextField = "Dept_Name";
        ddlDepart.DataValueField = "ID";
        ddlDepart.DataBind();
        //ddlDepart.Items.Insert(0, "");

        ddlTextCatagory.DataSource = ClsItemDetailsManager.ShowTextCatagory();
        ddlTextCatagory.DataValueField = "ID";
        ddlTextCatagory.DataTextField = "Name";
        ddlTextCatagory.DataBind();
        ddlTextCatagory.Items.Insert(0,new ListItem(""));        
        //txtCode.Text = ClsItemDetailsManager.GetShowItemsDetailsInformation().ToString().PadLeft(6, '0');
        //txtDiscountAmount.Enabled=CheckBox2.Checked= false;
        txtDescription.Text = "";
        lblBrandID.Text = "";
        ddlUmo.Items.Clear();
        string query2 = "select '' [ID],'' [Name]  union select [ID] ,[Name] from [UOM] where Active='True' order by Name ASC";
        util.PopulationDropDownList(ddlUmo, "UOM", query2, "Name", "ID");
        ddlUmo.SelectedValue = "1";

        //string query3 = "select '' [ID],'' [BrandName]  union select [ID] ,[BrandName] from [Brand] ORDER BY BrandName ASC";
        //util.PopulationDropDownList(ddlBrand, "Brand", query3, "BrandName", "ID");
        txtStName.Text = "";
        txtCode.Text = IdManager.GetDateTimeWiseSerialGetItems("", "Code", "Item");
      // txtStyleNo.Text = "SDL-" + IdManager.GetDateTimeWiseSerialGetItems("", "Code", "Item");
        ddlTextCatagory.SelectedIndex = 2;
        txtDiscountAmount.Text = "0";
        txtUnitPrice.Text = "0";

        Session["empPhoto"] = null;

        ddlWarrantyYear.Items.Clear();
        for (int i = 0; i <= 20; i++)
        {
            ddlWarrantyYear.Items.Add(i.ToString());
            //ddlWarrantyYear.SelectedIndex = -1;
        }

        //  ddlWarrantyYear.Items.Insert(0, "");
        ddlWarrantyMonth.Items.Clear();
        for (int ij = 0; ij <= 50; ij++)
        {
            ddlWarrantyMonth.Items.Add(ij.ToString());
            //ddlWarrantyMonth.SelectedIndex = -1;
        }

        ddlSize.DataSource = SizeManager.GetShowSizeDetails();
        ddlSize.DataTextField = "SizeName";
        ddlSize.DataValueField = "ID";
        ddlSize.DataBind();
        hfItemSetupID.Value = txtName.Text = txtStName.Text = string.Empty;
        txtName.Focus();
    }
    private void ClearAll()
    {
        tabVch.ActiveTabIndex = 0;
        txtCode.Text = "";
        imgEmp.ImageUrl = "S00003_guard1.jpg";
        //ddlCatagory.DataSource = MajorCategoryManager.GetMajorCats("");
        //ddlCatagory.DataTextField = "mjr_desc";
        //ddlCatagory.DataValueField = "mjr_code";
        //ddlCatagory.DataBind();
        //ddlCatagory.Items.Insert(0, "");
        CheckBox0.Checked = dgHistory.Visible = true;
        txtStyleNo.Text = "";
        txtName.Text = "";
        txtBrand.Text = "";
        txtOpeningStock.Text =
            txtOpeningAmount.Text =
                txtClosingStock.Text =
                    txtClosingAmount.Text = "0"; //txtDiscountAmount.Text = txtUnitPrice.Text = txtDiscountAmount.Text = "0";
        ddlCatagory.SelectedIndex = ddlSubCatagory.SelectedIndex = ddlCurrency.SelectedIndex = ddlTextCatagory.SelectedIndex = ddlUmo.SelectedIndex = -1;
        dgHistory.DataSource = ClsItemDetailsManager.getShowItemsHistoryDetails("");
        dgHistory.DataBind();
        //ddlDepart.DataSource = SizeManager.GetShowDeptDetails();
        //ddlDepart.DataTextField = "Dept_Name";
        //ddlDepart.DataValueField = "ID";
        //ddlDepart.DataBind();
        //ddlDepart.Items.Insert(0, "");
        //ddlTextCatagory.DataSource = ClsItemDetailsManager.ShowTextCatagory();
        ddlTextCatagory.DataValueField = "ID";
        ddlTextCatagory.DataTextField = "Name";
        ddlTextCatagory.DataBind();
        ddlTextCatagory.Items.Insert(0, new ListItem(""));
        txtDescription.Text = "";
        lblBrandID.Text = "";
        ddlUmo.Items.Clear();
        string query2 = "select '' [ID],'' [Name]  union select [ID] ,[Name] from [UOM] where Active='True' order by Name ASC";
        util.PopulationDropDownList(ddlUmo, "UOM", query2, "Name", "ID");
        ddlUmo.SelectedIndex = 8;
        txtStName.Text = "";
        txtCode.Text = IdManager.GetDateTimeWiseSerialGetItems("", "Code", "Item");
        ddlTextCatagory.SelectedIndex = 2;
        txtDiscountAmount.Text = "0";
        txtName.Focus();
    }
    protected void ddlCatagory_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            ddlSubCatagory.DataSource = SubMajorCategoryManager.GetSubMajorCategories(ddlCatagory.SelectedValue);
            ddlSubCatagory.DataTextField = "Name";
            ddlSubCatagory.DataValueField = "ID";
            ddlSubCatagory.DataBind();
            ddlSubCatagory.Items.Insert(0, "");
            UpdatePanel2.Update();
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
    protected void BtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('input Items Name...!!');", true);
                return;
            }
            if (string.IsNullOrEmpty(txtBrand.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('input Brand Name...!!');", true);
                return;
            }
            if (string.IsNullOrEmpty(ddlCatagory.SelectedItem.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Select Items Category.!!');", true);
                return;
            }
            //if (string.IsNullOrEmpty(ddlSubCatagory.SelectedItem.Text))
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Select Sub-Catagory.!!');", true);
            //    return;
            //}
            ClsItemDetailsInfo aClsItemDetailsInfoObj = ClsItemDetailsManager.GetShowDetails(lblID.Text.Trim());
            if (aClsItemDetailsInfoObj == null)
            {
                if (per.AllowAdd == "Y")
                {
                    //int count = IdManager.GetShowSingleValueInt("Count(*)",
                    //    "t1.Code='" + txtCode.Text + "' and replace(Upper(t1.Name),' ','')=Upper('" + txtName.Text +
                    //    "') and t1.Brand", "Item t1", lblBrandID.Text);
                    //if (count > 0)
                    //{
                    //    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    //        "alert('Already this Brand Name Product is Exist...!!');", true);
                    //    return;
                    //}

                    //int count = IdManager.GetShowSingleValueInt("Count(*)",
                    //    "replace(Upper(t1.StyleNo),' ','')", "Item t1", txtStyleNo.Text.ToUpper());
                    //if (count > 0)
                    //{
                    //    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    //        "alert('Already this style No. is Exist...!!');", true);
                    //    return;
                    //}

                    aClsItemDetailsInfoObj = new ClsItemDetailsInfo();

                   
                    //txtCode.Text = ClsItemDetailsManager.GetShowItemsDetailsInformation().ToString().PadLeft(6, '0');
                    // aClsItemDetailsInfoObj.ItemsCode = txtCode.Text;
                    if (CheckBox0.Checked == true)
                    {
                        aClsItemDetailsInfoObj.Active = true;
                    }
                    else
                    {
                        aClsItemDetailsInfoObj.Active = false;
                    }
                    aClsItemDetailsInfoObj.ItemsName = txtName.Text.Replace("'", "’");
                    aClsItemDetailsInfoObj.Umo = ddlUmo.SelectedValue;
                    aClsItemDetailsInfoObj.UnitPrice = txtUnitPrice.Text;
                    aClsItemDetailsInfoObj.Currency = ddlCurrency.SelectedValue;
                    aClsItemDetailsInfoObj.DepID = ddlDepart.SelectedValue;
                    aClsItemDetailsInfoObj.OpeningStock = txtOpeningStock.Text.Replace(",", "");
                    aClsItemDetailsInfoObj.OpeningAmount = txtOpeningAmount.Text.Replace(",", "");
                    aClsItemDetailsInfoObj.ClosingStock = txtOpeningStock.Text.Replace(",", "");
                    aClsItemDetailsInfoObj.ClosingAmount = txtOpeningAmount.Text.Replace(",", "");
                   // aClsItemDetailsInfoObj.StyleNo = txtStyleNo.Text;
                    aClsItemDetailsInfoObj.Catagory = ddlCatagory.SelectedValue;

                    if (!string.IsNullOrEmpty(ddlSubCatagory.SelectedValue))
                    {
                        aClsItemDetailsInfoObj.SubCatagory = ddlSubCatagory.SelectedValue;
                    }
                    else
                    {
                        aClsItemDetailsInfoObj.SubCatagory = "";
                    }
                    if (ddlTextCatagory.SelectedValue == "")
                    {
                        aClsItemDetailsInfoObj.Text = "0";
                    }
                    else
                    {
                        aClsItemDetailsInfoObj.Text = ddlTextCatagory.SelectedValue;
                    }

                    if (CheckBox2.Checked == true)
                    {
                        aClsItemDetailsInfoObj.DiscountCheck = true;
                    }
                    else
                    {
                        aClsItemDetailsInfoObj.DiscountCheck = false;
                    }
                    aClsItemDetailsInfoObj.Discount = txtDiscountAmount.Text;
                    aClsItemDetailsInfoObj.ItemsImage = (byte[]) Session["empPhoto"];
                    aClsItemDetailsInfoObj.LoginBy = Session["user"].ToString();
                    aClsItemDetailsInfoObj.Description = txtDescription.Text.Replace("'", "’");
                    aClsItemDetailsInfoObj.Brand = lblBrandID.Text;
                    aClsItemDetailsInfoObj.ShortName = txtStName.Text;
                    aClsItemDetailsInfoObj.StyleNo = txtStyleNo.Text;
                    txtCode.Text = IdManager.GetDateTimeWiseSerialGetItems("", "Code", "Item");
                    aClsItemDetailsInfoObj.ItemsCode = txtCode.Text;
                    aClsItemDetailsInfoObj.WarrantyMonth = ddlWarrantyMonth.SelectedValue;
                    aClsItemDetailsInfoObj.WarrantyYear = ddlWarrantyYear.SelectedValue;
                    aClsItemDetailsInfoObj.SizeID = ddlSize.SelectedValue;
                    aClsItemDetailsInfoObj.ItemSetupID = hfItemSetupID.Value;

                    DataTable data = clsItemTransferStockManager.IsExist(aClsItemDetailsInfoObj);

                    if (data.Rows.Count>0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('The Same Item Already Exist..!!');", true);
                        return;
                    }

                    ClsItemDetailsManager.SaveItemsInformation(aClsItemDetailsInfoObj);
                    RefreshAll();
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('Record is/are saved saved successfully..!!');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('You are not Permitted this Step..!!');", true);
                }
            }
            else
            {
                if (per.AllowEdit == "Y")
                {
                    //int count1 = IdManager.GetShowSingleValueInt("Count(*)",
                    //    "t1.Code='" + txtCode.Text + "' and replace(Upper(t1.Name),' ','')=Upper('" +
                    //    txtName.Text + "') and t1.Brand", "Item t1", lblBrandID.Text,
                    //    Convert.ToInt32(lblID.Text));
                    //if (count1 > 0)
                    //{
                    //    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    //        "alert('Already this Brand Name Product is Exist...!!');", true);
                    //    return;
                    //}

                    //int count = IdManager.GetShowSingleValueInt("Count(*)",
                    //    "replace(Upper(t1.StyleNo),' ','')", "Item t1", txtStyleNo.Text.ToUpper(),
                    //  Convert.ToInt32(lblID.Text));
                    //if (count > 0)
                    //{
                    //    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    //        "alert('Already this style No. is Exist...!!');", true);
                    //    return;
                    //}
                    aClsItemDetailsInfoObj.Id = lblID.Text.Trim();
                    if (CheckBox0.Checked == true)
                    {
                        aClsItemDetailsInfoObj.Active = true;
                    }
                    else
                    {
                        aClsItemDetailsInfoObj.Active = false;
                    }
                    aClsItemDetailsInfoObj.ItemsName = txtName.Text.Replace("'", "’");
                    aClsItemDetailsInfoObj.Umo = ddlUmo.SelectedValue;
                    aClsItemDetailsInfoObj.UnitPrice = txtUnitPrice.Text;
                    aClsItemDetailsInfoObj.Currency = ddlCurrency.SelectedValue;
                    aClsItemDetailsInfoObj.OpeningStock = txtOpeningStock.Text.Replace(",", "");
                    aClsItemDetailsInfoObj.OpeningAmount = txtOpeningAmount.Text.Replace(",", "");
                    aClsItemDetailsInfoObj.DepID = ddlDepart.SelectedValue;
                    //aClsItemDetailsInfoObj.ClosingStock = txtClosingStock.Text;
                    //aClsItemDetailsInfoObj.ClosingAmount = txtClosingAmount.Text;
                    aClsItemDetailsInfoObj.Catagory = ddlCatagory.SelectedValue;
                    //aClsItemDetailsInfoObj.SubCatagory = ddlSubCatagory.SelectedValue;
                    if (!string.IsNullOrEmpty(ddlSubCatagory.SelectedValue))
                    {
                        aClsItemDetailsInfoObj.SubCatagory = ddlSubCatagory.SelectedValue;
                    }
                    else
                    {
                        aClsItemDetailsInfoObj.SubCatagory = "";
                    }

                    aClsItemDetailsInfoObj.Text = ddlTextCatagory.SelectedValue;
                    if (CheckBox2.Checked == true)
                    {
                        aClsItemDetailsInfoObj.DiscountCheck = true;
                        aClsItemDetailsInfoObj.Discount = txtDiscountAmount.Text;
                    }
                    else
                    {
                        aClsItemDetailsInfoObj.DiscountCheck = true;
                        aClsItemDetailsInfoObj.Discount = txtDiscountAmount.Text;
                    }
                    aClsItemDetailsInfoObj.ItemsImage = (byte[])Session["empPhoto"];
                    aClsItemDetailsInfoObj.LoginBy = Session["user"].ToString();
                    aClsItemDetailsInfoObj.Description = txtDescription.Text.Replace("'", "’");
                    aClsItemDetailsInfoObj.Brand = lblBrandID.Text;
                    aClsItemDetailsInfoObj.ShortName = txtStName.Text;
                    aClsItemDetailsInfoObj.StyleNo = txtStyleNo.Text;
                    aClsItemDetailsInfoObj.ItemsCode = txtCode.Text;
                    aClsItemDetailsInfoObj.WarrantyMonth = ddlWarrantyMonth.SelectedValue;
                    aClsItemDetailsInfoObj.WarrantyYear = ddlWarrantyYear.SelectedValue;
                    aClsItemDetailsInfoObj.SizeID = ddlSize.SelectedValue;
                    aClsItemDetailsInfoObj.ItemSetupID = hfItemSetupID.Value;
                    DataTable data = clsItemTransferStockManager.IsExist(aClsItemDetailsInfoObj);

                    if (data.Rows.Count > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('The Same Item Already Exist..!!');", true);
                        return;
                    }

                    ClsItemDetailsManager.UpdateItemsInformation(aClsItemDetailsInfoObj);
                    RefreshAll();
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is/are update Successfully..!!');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('You are not Permitted this Step...!!');", true);
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
    protected void BtnFind_Click(object sender, EventArgs e)
    {

    }
    protected void BtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            if (per.AllowDelete == "Y")
            {
                int countPurchase = IdManager.GetShowSingleValueInt("COUNT(*)", "ItemPurchaseDtl where ItemID='" + lblID.Text + "' ");
               // int countPurchaseLocal = IdManager.GetShowSingleValueInt("COUNT(*)", "ItemPurchaseLocalDtl where ItemID='" + lblID.Text + "' ");
                if (countPurchase > 0 )
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('This items alrady assign . \\n you can not delete this item.\\n if you want to delete this items please contract you administrator..!!');", true);
                    return;
                }
                ClsItemDetailsInfo aClsItemDetailsInfoObj = new ClsItemDetailsInfo();
                aClsItemDetailsInfoObj.ItemsCode = txtCode.Text;
                aClsItemDetailsInfoObj.ItemsName = txtName.Text;
                aClsItemDetailsInfoObj.Umo = ddlUmo.SelectedValue;
                aClsItemDetailsInfoObj.ItemId = lblID.Text;
                ClsItemDetailsManager.DeleteItemsInformation(aClsItemDetailsInfoObj);
                RefreshAll();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is/are Deleted Sucessfully..!!');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step..!!');", true);
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
    protected void BtnReset_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {

    }
    protected void dgHistory_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            ClsItemDetailsInfo Items = ClsItemDetailsManager.GetShowDetails(dgHistory.SelectedRow.Cells[1].Text.Trim());
            if (Items != null)
            {
                Session["empPhoto"] = "";
                lblID.Text = dgHistory.SelectedRow.Cells[1].Text;
                Session["ItemID"] = lblID.Text;
                txtCode.Text = Items.ItemsCode;
                txtName.Text = Items.ItemsName;
                ddlUmo.SelectedValue = Items.Umo;
                txtUnitPrice.Text = Items.UnitPrice;
                txtStyleNo.Text = Items.StyleNo;
                try
                {
                    ddlCurrency.SelectedValue = Items.Currency;
                }
                catch
                {
                    ddlCurrency.SelectedValue = null;
                }
                txtOpeningStock.Text = Items.OpeningStock.Replace(",","");
                txtOpeningAmount.Text = Items.OpeningAmount.Replace(",", "");
                txtClosingStock.Text = Items.ClosingStock.Replace(",", "");
                txtClosingAmount.Text = Items.ClosingAmount.Replace(",", "");
                try
                {
                    ddlDepart.DataSource = SizeManager.GetShowDeptDetails();
                    ddlDepart.DataTextField = "Dept_Name";
                    ddlDepart.DataValueField = "ID";
                    ddlDepart.DataBind();
                    //ddlDepart.Items.Insert(0, "");
                    ddlDepart.SelectedValue = Items.DepID.Replace(",", "");
                }
                catch
                {
                    ddlDepart.SelectedValue = null;
                }
               
                txtStName.Text = Items.ShortName;
                ddlCatagory.DataSource = MajorCategoryManager.GetMajorCats("");
                ddlCatagory.DataTextField = "mjr_desc";
                ddlCatagory.DataValueField = "mjr_code";
                ddlCatagory.DataBind();
                ddlCatagory.Items.Insert(0, new ListItem(""));
                ddlCatagory.SelectedValue = Items.Catagory;
                //if (Items.Catagory != "0")
                //{
                //    ddlCatagory.SelectedValue = Items.Catagory;
                //}
                if (Items.Brand != "0")
                {
                    lblBrandID.Text = Items.Brand;
                }
                txtBrand.Text = Items.BrandName;
              

                if (Items.SubCatagory != "0")
                {
                    ddlSubCatagory.SelectedValue = Items.SubCatagory;
                }               
                txtDescription.Text = Items.Description;
                if (Items.DiscountCheck == true)
                {
                    CheckBox2.Checked = true;
                }
                txtDiscountAmount.Text = Items.Discount;
                if (Items.Text != "0")
                {
                    ddlTextCatagory.SelectedValue = Items.Text;
                }
                if (Items.Active == true)
                {
                    CheckBox0.Checked = true;
                }
                ItemsPhoto = (byte[])Items.ItemsImage;
                Session["empPhoto"] = ItemsPhoto;
                if (ItemsPhoto != null)
                {
                    string base64String = Convert.ToBase64String(ItemsPhoto, 0, ItemsPhoto.Length);
                    imgEmp.ImageUrl = "data:image/png;base64," + base64String;
                }

                try
                {
                    ddlWarrantyMonth.SelectedValue = Items.WarrantyMonth;
                }
                catch
                {
                    ddlWarrantyMonth.SelectedValue = null;
                }
                try
                {
                    ddlWarrantyYear.SelectedValue = Items.WarrantyYear;
                }
                catch
                {
                    ddlWarrantyYear.SelectedValue = null;
                }

                try
                {
                    ddlSize.SelectedValue = Items.SizeID;
                }
                catch
                {
                    ddlSize.SelectedValue = null;
                }

                hfItemSetupID.Value = Items.ItemSetupID;
                BindGridData();

                dgHistory.Visible = txtOpeningStock.Enabled =
                    txtOpeningAmount.Enabled = txtClosingStock.Enabled = txtClosingAmount.Enabled = false;
                UPOPAmount.Update();
                UPOPStock.Update();
                UPanel1Cat.Update();
                UpdatePanel2.Update();
               
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
    protected void lbImgUpload_Click(object sender, EventArgs e)
    {
        Session["empPhoto"] = null;
        try
        {
            if (imgUpload.HasFile)
            {
                int width = 145;
                int height = 165;
                using (System.Drawing.Bitmap img = EmpManager.ResizeImage(new System.Drawing.Bitmap(imgUpload.PostedFile.InputStream), width, height, EmpManager.ResizeOptions.ExactWidthAndHeight))
                {
                    imgUpload.PostedFile.InputStream.Close();
                    ItemsPhoto = EmpManager.ConvertImageToByteArray(img, System.Drawing.Imaging.ImageFormat.Png);
                    Session["empPhoto"] = ItemsPhoto;
                    img.Dispose();
                }
                string base64String = Convert.ToBase64String(ItemsPhoto, 0, ItemsPhoto.Length);
                imgEmp.ImageUrl = "data:image/png;base64," + base64String;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please input employee first name, birth date, and then browse a photograph image!!');", true);
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
    protected void dgHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgHistory.DataSource = ClsItemDetailsManager.getShowItemsHistoryDetails("");
        dgHistory.PageIndex = e.NewPageIndex;
        dgHistory.DataBind();
    }
    protected void dgHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
           
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[8].Attributes.Add("style", "display:none");
                //e.Row.Cells[9].Attributes.Add("style", "display:none");
                e.Row.Cells[10].Attributes.Add("style", "display:none");

                e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Left;
                e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Left;
                e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Left;
                e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Left;
                e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Left;
                e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Left;
                //e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[8].Attributes.Add("style", "display:none");
               // e.Row.Cells[9].Attributes.Add("style", "display:none");
                e.Row.Cells[10].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[8].Attributes.Add("style", "display:none");
               // e.Row.Cells[9].Attributes.Add("style", "display:none");
                e.Row.Cells[10].Attributes.Add("style", "display:none");
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
    protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
    {
        //if (CheckBox2.Checked == true)
        //{
        //    txtDiscountAmount.Enabled = true;
        //}
        //else
        //{
        //    txtDiscountAmount.Enabled = false;
        //}
    }
    protected void txtOpeningStock_TextChanged(object sender, EventArgs e)
    {
        try
        {
            txtOpeningAmount.Text = (Convert.ToDouble(txtUnitPrice.Text) * Convert.ToDouble(txtOpeningStock.Text)).ToString("N3");
            UPOPAmount.Update();
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
    protected void lbSearch_Click(object sender, EventArgs e)
    {
        DataTable dtFindItems = _aItemManager.GetSearchItemsWithSize(txtSearchItems.Text);
        string Parameter = " where t1.[ID]='" + dtFindItems.Rows[0]["ID"].ToString() + "' and  t1.[Active]=1 ";
        DataTable dtSearchItems = ItemManager.GetItems(Parameter);
        if (dtSearchItems.Rows.Count > 0)
        {
            dgHistory.DataSource = ClsItemDetailsManager.getShowItemsHistoryDetails(" where t1.id='" + dtSearchItems.Rows[0]["ID"].ToString() + "' ");
            dgHistory.DataBind();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('No items Search.!!');", true);
        }
    }
    protected void lbClear_Click(object sender, EventArgs e)
    {
        txtSearchItems.Text = string.Empty;
        dgHistory.DataSource = ClsItemDetailsManager.getShowItemsHistoryDetails("");
        dgHistory.DataBind();
        dgHistory.Visible = true;
        txtSearchItems.Focus();
    }
    protected void txtBrand_TextChanged(object sender, EventArgs e)
    {
        if(!string.IsNullOrEmpty(txtBrand.Text))
        {
            DataTable dt = ClsItemDetailsManager.getBrandInfo(txtBrand.Text);
            if(dt.Rows.Count>0)
            {
                txtBrand.Text = dt.Rows[0]["BrandName"].ToString();
                lblBrandID.Text = dt.Rows[0]["ID"].ToString();
            }
            else
            {
                txtBrand.Text = "";
                lblBrandID.Text ="";
            }
        }
        UPBrand.Update();
    }
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(lblID.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Plesae Select Items then select item!!');", true);
                return;
            }
            if (FileUpload1.HasFile)
            {
                int width = 145;
                int height = 165;
                byte[] Photo;
                string pp = FileUpload1.PostedFile.ContentType;
                if (pp.ToUpper().Contains("JPG") || pp.ToUpper().Contains("PNG") || pp.ToUpper().Contains("JPEG"))
                {
                    using (System.Drawing.Bitmap img = DataManager.ResizeImage(new System.Drawing.Bitmap(FileUpload1.PostedFile.InputStream), width, height, DataManager.ResizeOptions.ExactWidthAndHeight))
                    {
                        DataTable dt = null;
                        if (Session["Img"] == null)
                        {
                            dt = new DataTable();
                            dt.Columns.Add("ImageID", typeof(string));
                            dt.Columns.Add("imagename", typeof(string));
                            dt.Columns.Add("Image", typeof(byte[]));
                            //DataRow dr = dt.NewRow();       
                            //dt.Rows.Add(dr); 
                            Session["Img"] = dt;
                        }
                        int MaxID = IdManager.GetShowSingleValueInt(" CASE WHEN MAX(ImageID) IS NULL THEN 1 else MAX(ImageID) END ",
                            "ItemImage where MstID='" + lblID.Text + "' ");
                        
                        FileUpload1.PostedFile.InputStream.Close();
                        Photo = DataManager.ConvertImageToByteArray(img, System.Drawing.Imaging.ImageFormat.Png);
                        string base64String = Convert.ToBase64String(Photo, 0, Photo.Length);
                        dt = (DataTable)Session["Img"];
                        dt.Rows.Add(FileUpload1.FileName, (MaxID + 1), Photo);
                        Session["Img"] = dt;
                        //img.Dispose();
                        //ShiftmentItemsManager.SaveImageTemporary(dt.Rows.Count, Photo);
                        //dgImage.DataSource = dt;
                        //dgImage.DataBind();                  
                        //imgStd.ImageUrl = "data:image/png;base64," + base64String;
                        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
                        int length = FileUpload1.PostedFile.ContentLength;
                        byte[] imgbyte = new byte[length];
                        HttpPostedFile img1 = FileUpload1.PostedFile;
                        img1.InputStream.Read(imgbyte, 0, length);
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("INSERT INTO ItemImage (MstID,ImageName,ImageID,Image) VALUES (@MstID,@ImageName,@ImageID,@imagedata)", connection);
                        cmd.Parameters.Add("@MstID", SqlDbType.Int, 50).Value =Convert.ToUInt32(lblID.Text);
                        cmd.Parameters.Add("@ImageName", SqlDbType.Text, 50).Value = FileUpload1.FileName;
                        cmd.Parameters.Add("@ImageID", SqlDbType.Int, 50).Value = (MaxID + 1);
                        cmd.Parameters.Add("@imagedata", SqlDbType.Image).Value = Photo;
                        int count = cmd.ExecuteNonQuery();
                        connection.Close();
                        if (count == 1)
                        {
                            BindGridData();
                        }
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please Select Image Type of (*.PNG) Or (*.JPG) ..!!');", true);
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please Select File Then Upload!!');", true);
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
    private void BindGridData()
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlCommand command = new SqlCommand(
            "SELECT imagename,ImageID,Image from [ItemImage] where MstID=" + lblID.Text + " ", connection);
        SqlDataAdapter daimages = new SqlDataAdapter(command);
        DataTable dt = new DataTable();
        daimages.Fill(dt);
        dgImage.DataSource = dt;
        Session["Img"] = dt;
        dgImage.DataBind();
        dgImage.Attributes.Add("bordercolor", "black");
    }
    protected void dgImage_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (Session["Img"] != null)
        {
            DataTable dtDtlGrid = (DataTable)Session["Img"];
            dtDtlGrid.Rows.RemoveAt(dgImage.Rows[e.RowIndex].DataItemIndex);
            string ID = dgImage.Rows[dgImage.Rows[e.RowIndex].DataItemIndex].Cells[2].Text;
            IdManager.getInsertUpdateDelete("DELETE FROM [ItemImage] Where ImageID='" + ID + "' and MstID=" + lblID.Text +
                                            " ");
            BindGridData();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Your session is over. Try it again!!');", true);
        }
    }
    protected void dgImage_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[2].Attributes.Add("style", "display:none");
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
    protected void ddlDepart_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable dtCatagory = _aMajorCategoryManager.GetCaregotyOnDepartment(ddlDepart.SelectedValue);
        ddlCatagory.DataSource = dtCatagory;
        ddlCatagory.DataTextField = "Name";
        ddlCatagory.DataValueField = "ID";
        ddlCatagory.DataBind();
        ddlCatagory.Items.Insert(0,"");
        UPanel1Cat.Update();
    }
    protected void txtName_TextChanged(object sender, EventArgs e)
    {
        string Parameter = "where upper([Search])=upper('" + txtName.Text + "')";
        DataTable dtIteam = _aClsItemDetailsManager.GetItemsSetupDetails(Parameter);
        if (dtIteam.Rows.Count > 0)
        {
            hfItemSetupID.Value = dtIteam.Rows[0]["ID"].ToString();
            txtName.Text = dtIteam.Rows[0]["Name"].ToString();
            txtStName.Text =  dtIteam.Rows[0]["ShortName"].ToString();
        }
        else
        {
            hfItemSetupID.Value = txtName.Text = txtStName.Text = string.Empty;
        }
    }
    protected void lbItemLink_Click(object sender, EventArgs e)
    {
        string strJS = ("<script type='text/javascript'>window.open('IteamCreate.aspx?mno=8.24','_blank');</script>");
        Page.ClientScript.RegisterStartupScript(this.GetType(), "strJSAlert", strJS);
    }
}