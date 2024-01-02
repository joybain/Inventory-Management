using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using sales;
using Delve;

public partial class PartyStock : System.Web.UI.Page
{
    ItemPartyStockManager aItemPartyStockManager = new ItemPartyStockManager();
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
                    string wnot = "";
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            Session["userlevel"] = int.Parse(dReader["user_grp"].ToString());
                            //Session["dept"] = dReader["dept"].ToString();
                            wnot = "Welcome " + dReader["description"].ToString();
                        }
                        Session["wnote"] = wnot;
                        Session["LoginCountry"] = dReader["LoginCountry"].ToString();
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        //cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type from gl_set_of_books where book_name='" + Session["book"].ToString() + "' ";
                        if (Convert.ToInt32(dReader["UserType"].ToString()) == 2)
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
            RefreshAll();              
        }
    }
    private void RefreshAll()
    {
        txtItemsName.Text = txtPartyName.Text = txtQuantity.Text =lblItemsID.Text=lblPartyID.Text= "";
        txtPartyName.Focus();
        DataTable dt = ItemPartyStockManager.GetItemPartyStockDetails("");
        dgStock.DataSource = dt;
        dgStock.DataBind();
    }
    protected void txtPartyName_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = clsClientInfoManager.GetShowPartyInfo(txtPartyName.Text);
        if (dt.Rows.Count > 0)
        {
            lblPartyID.Text = dt.Rows[0]["ID"].ToString();
            txtPartyName.Text = dt.Rows[0]["ContactName"].ToString();
            txtItemsName.Focus();
            UP1.Update();
        }
    }
    protected void txtItemsName_TextChanged(object sender, EventArgs e)
    {
        //DataTable dt = ClsItemDetailsManager.GetShowItemsInfoSearch(txtItemsName.Text);
        //if (dt.Rows.Count > 0)
        //{
        //    lblItemsID.Text = dt.Rows[0]["ID"].ToString();
        //    txtItemsName.Text = dt.Rows[0]["Name"].ToString();
        //    txtQuantity.Focus();
        //    UP1.Update();
        //}
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {     
         ItemPartyStockInfo aItemPartyStockInfo = new ItemPartyStockInfo();
         if (txtID.Text == "")
         {
             aItemPartyStockInfo.ID = txtID.Text;
             aItemPartyStockInfo.PartyID = lblPartyID.Text;
             aItemPartyStockInfo.ItemsID = lblItemsID.Text;
             aItemPartyStockInfo.Quantity = txtQuantity.Text;
             aItemPartyStockInfo.LoginBy = "";
             ItemPartyStockManager.SaveItemPartyStock(aItemPartyStockInfo);
         }
         else
         {
             aItemPartyStockInfo.ID = txtID.Text;
             aItemPartyStockInfo.PartyID = lblPartyID.Text;
             aItemPartyStockInfo.ItemsID = lblItemsID.Text;
             aItemPartyStockInfo.Quantity = txtQuantity.Text;
             aItemPartyStockInfo.LoginBy = "";
             ItemPartyStockManager.UpdateItemPartyStock(aItemPartyStockInfo);
         }
        
    }
    protected void DeleteButton_Click(object sender, EventArgs e)
    {
        ItemPartyStockInfo aItemPartyStockInfo = new ItemPartyStockInfo();
        aItemPartyStockInfo.ID = txtID.Text;       
        ItemPartyStockManager.DeleteItemPartyStock(aItemPartyStockInfo);
    }
    protected void CloseButton_Click(object sender, EventArgs e)
    {
        RefreshAll();
    }
}