using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Delve;
using System.Data.SqlClient;
using sales;

public partial class BranchItemReceivedStock : System.Web.UI.Page
{
    private static Permis per;
    protected void Page_Load(object sender, EventArgs e)
    
    {  if (Session["user"] == null)
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
                            "Select ID,user_grp,BranchId,description from utl_userinfo where upper(user_name)=upper('" +
                            Session["user"].ToString().ToUpper() + "') and status='A'";
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
                            cmd.CommandText =
                                "Select book_desc,company_address1,company_address2,separator_type from gl_set_of_books where book_name='" +
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
                    ((LinkButton)Page.Master.FindControl("lbLogout")).Visible = true;
                }
                else
                {
                    Response.Redirect("BranchHome.aspx?sid=sam");
                }
            }
            catch
            {
                Response.Redirect("BranchHome.aspx?sid=sam");

            }

            if (!IsPostBack)
            {
                try
                {
                    var BranchId = Session["BranchId"].ToString();
                    if (string.IsNullOrEmpty(BranchId))

                    {
                        Response.Redirect("BranchHome.aspx?sid=sam");

                    }
                    else
                    {
                    
                        txtByDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                        txtToDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                        int branchId = Convert.ToInt32(BranchId);
                        string TransferMstIds = "";
                        DataTable data=new DataTable();
                        try
                        {
                            data = clsItemTransferStockManager.getShowTransferMst(branchId, "", "");
                        }
                        catch 
                        {
                           
                        }
                         
                       
                        if (data.Rows.Count > 0)
                        {
                            for (int i = 0; i < data.Rows.Count; i++)
                            {
                                TransferMstIds = TransferMstIds + data.Rows[i]["Id"].ToString();
                                if (i < data.Rows.Count - 1)
                                {
                                    TransferMstIds = TransferMstIds + ",";
                                }
                            }
                           

                            var dt = clsItemTransferStockManager.getShowTransferDtl(TransferMstIds);

                            int IsExist = clsItemTransferStockManager.getSupplier();
                           
                            var  data1 = clsItemTransferStockManager.GetMainBrancSupplier();


                          clsItemTransferStockManager.SaveTranseferStock(data, dt, data1, IsExist, Session["userID"].ToString(), BranchId);
                        }

                       

                        DataTable dtTransferMst2 = clsItemTransferStockManager.getShowTransferMstOnPos("", "",BranchId);
                        // dgTransMst.DataSource = dtTransferMst2;
                        //dgTransMst.Visible = true;

                        dgBranchItemReceivedMst.DataSource = dtTransferMst2;
                        dgBranchItemReceivedMst.DataBind();
                        divItemDtls.Visible = false;


                    }
                }
                catch 
                {
                    Response.Redirect("BranchHome.aspx?sid=sam");

                }
               
            }
    }

    protected void BtnSave_Click(object sender, System.EventArgs e)
    {
        try
        {
            var data = (DataTable)ViewState["TransferDtl"];
            if (data.Rows.Count > 0)
            {
                if (hfStatus.Value != "Recived")
                {

                    int MstTransferUpdate =
                        clsItemTransferStockManager.UpdateTranseferMst(hfId.Value, Session["userID"].ToString());
                    if (MstTransferUpdate>0)
                    {
                        int DtlTransferUpdate = clsItemTransferStockManager.UpdateTranseferDtl(hfId.Value, Session["userID"].ToString());
                        if (DtlTransferUpdate>0)
                        {
                            int save = clsItemTransferStockManager.SaveRecivedStockDtl(data, hfId.Value, Session["userID"].ToString());
                            if (save > 0)
                            {
                                Clear();
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Record(s) is/are Save suceessfullly!!','green',1);", true);


                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);

                            }
                        }
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
    protected void lbSearch_Click(object sender, EventArgs e)
    {
        try
        {

            DataTable data = clsItemTransferStockManager.getShowTransferMstOnPos(txtByDate.Text, txtToDate.Text, Session["BranchId"].ToString());
            //int branchId = IdManager.GetShowSingleValueInt("Id", "MainBranch", "BranchInfo", "1");
            //string TransferMstIds = "";
           // var data = clsItemTransferStockManager.getShowTransferMst(branchId,txtByDate.Text,txtToDate.Text);
            dgBranchItemReceivedMst.DataSource = data;
            dgBranchItemReceivedMst.DataBind();
            divItemMst.Visible = true;
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
    protected void dgBranchItemReceivedMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header |
                e.Row.RowType == DataControlRowType.Footer)
            {
               
                e.Row.Cells[1].Attributes.Add("style", "display:none");
              
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
    protected void dgBranchItemReceivedMst_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        try
        {

            DataTable data = clsItemTransferStockManager.getShowTransferDtlPos(Convert.ToInt16(dgBranchItemReceivedMst.SelectedRow.Cells[1].Text.Trim()));
          

          //  var data = clsItemTransferStockManager.getShowTransferDtl(dgBranchItemReceivedMst.SelectedRow.Cells[1].Text.Trim());
            if (data.Rows.Count>0)
            {
                dgBranchItemReceivedDtl.DataSource = data;
                dgBranchItemReceivedDtl.DataBind();
                dgBranchItemReceivedDtl.Visible = true;
                BtnSave.Visible = true;
                ViewState["TransferDtl"] = data;
                hfId.Value = dgBranchItemReceivedMst.SelectedRow.Cells[1].Text.Trim();
                hfStatus.Value = dgBranchItemReceivedMst.SelectedRow.Cells[6].Text.Trim();
                if (hfStatus.Value == "Recived")
                {
                    BtnSave.Enabled = false;
                }
                else
                {
                    BtnSave.Enabled = true;
                }
            }
            else
            {
                dgBranchItemReceivedDtl.DataSource = null;
                dgBranchItemReceivedDtl.DataBind();
                dgBranchItemReceivedDtl.Visible = false;
                BtnSave.Visible = false;
                ViewState["TransferDtl"] = null;
                hfId.Value = "";
                hfStatus.Value = "";
            }

            divItemMst.Visible = false;
            divItemDtls.Visible = true;
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
    protected void dgBranchItemReceivedDtl_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header |
                e.Row.RowType == DataControlRowType.Footer)
            {

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
  
    protected void lbClear_Click(object sender, System.EventArgs e)
    {
        Clear();
    }

    public void Clear()
    {
        //int branchId = IdManager.GetShowSingleValueInt("Id", "MainBranch", "BranchInfo", "1");



        DataTable dtTransferMst2 = clsItemTransferStockManager.getShowTransferMstOnPos("", "", Session["BranchId"].ToString());
        // dgTransMst.DataSource = dtTransferMst2;
        //dgTransMst.Visible = true;

        dgBranchItemReceivedMst.DataSource = dtTransferMst2;
        dgBranchItemReceivedMst.DataBind();



      


      
        dgBranchItemReceivedDtl.DataSource = null;
        dgBranchItemReceivedDtl.DataBind();
        dgBranchItemReceivedDtl.Visible = false;
        BtnSave.Visible = false;
        ViewState["TransferDtl"] = null;
        hfId.Value = "";
        hfStatus.Value = "";
        txtByDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
        txtToDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
        BtnSave.Enabled = true;
        divItemMst.Visible = true;
        divItemDtls.Visible = false;

    }
    protected void btnBack_Click(object sender, System.EventArgs e)
    {
        Clear();
    }
   
}