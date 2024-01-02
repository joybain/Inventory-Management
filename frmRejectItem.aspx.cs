using System;

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;

public partial class frmRejectItem : System.Web.UI.Page
{
    RejectItemManager reject_mg=new RejectItemManager();
    ItemManager _aItemManager = new ItemManager();
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
                txtDate.Text = DateTime.Now.ToString("dd-MM-yyyy");

                var BranchId = Session["BranchId"].ToString();
                if (string.IsNullOrEmpty(BranchId))
                {
                    Response.Redirect("BranchHome.aspx?sid=sam");

                }
                else
                {

                }
            }
            catch
            {
                Response.Redirect("BranchHome.aspx?sid=sam");

            }

        }
    }
    protected void txtCode_TextChanged(object sender, EventArgs e)
    {
        bool IsChk = false;
        string[] splitCode = txtItemsCode.Text.Trim().Split('-');

        //string Code = IdManager.GetShowSingleValueString("Code",
        //    "ItemInformationForReport where upper(Name_Brand_Model)=upper('" + txtItemsCode.Text + "')");

        DataTable dtItemsSearch = _aItemManager.GetSearchItemsOnBranchStock(txtItemsCode.Text.ToUpper(), splitCode.Length, Session["BranchId"].ToString());
        DataTable DT1 = new DataTable();
        DT1 = (DataTable)ViewState["SV"];
        if (DT1 == null)
        {
            DT1 = dtItemsSearch;
        }
        else
        {
            int i = 0;
            decimal Qty = 0;
            decimal SPrice = 0;
            decimal stockQty = 0;
            if (dtItemsSearch.Rows.Count > 0)
            {
                foreach (DataRow dr in DT1.Rows)
                {
                    if (dr["Barcode"].ToString() == dtItemsSearch.Rows[0]["Barcode"].ToString())
                    {
                        IsChk = true;
                        Qty = Convert.ToDecimal(dr["Qty"].ToString());
                        stockQty = Convert.ToDecimal(dr["ClosingStock"].ToString());
                        SPrice = Convert.ToDecimal(dr["SPrice"].ToString());

                        break;
                    }

                    i++;
                }

                if (IsChk == false)
                {
                    DT1.ImportRow(dtItemsSearch.Rows[0]);
                }
                else
                {
                    if (stockQty >= Qty + 1)
                    {
                        DT1.Rows[i].SetField("Qty" /*columnName*/, Qty + 1);
                        DT1.Rows[i].SetField("Total" /*columnName*/, (Qty + 1) * SPrice);
                        // Quantity(sender);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Already this Item Qty is Over!!','red',0);", true);

                    }

                }
            }
        }

        dgSV.DataSource = DT1;
        ViewState["SV"] = DT1;
        dgSV.DataBind();
        ShowFooterTotal(DT1);
        //txtItemsCode.Text = "";
        //UPCustomer.Update();
        // UpItemsDetails.Update();
        //UpSearch.Update();
        txtItemsCode.Text = string.Empty;
        txtItemsCode.Focus();

    }
    private void ShowFooterTotal(DataTable DT1)
    {
        decimal totVat = 0;
        decimal totDis = 0;
        decimal Qty = 0;
        decimal tot = 0;
        foreach (DataRow dr in DT1.Rows)
        {
            totVat += Convert.ToDecimal(dr["Tax"]);
            totDis += Convert.ToDecimal(dr["DiscountAmount"]);
            Qty += Convert.ToDecimal(dr["Qty"]);
            tot += Convert.ToDecimal(dr["Total"]);
        }

      
       
        //txtGrandTotal.Text = (tot + ((tot * totVat) / 100)).ToString("N2");
        //txtDue.Text = ((tot + ((tot * totVat) / 100)) - Convert.ToDecimal(txtPayment.Text)).ToString("N2");
    }
    protected void txtQty_TextChanged(object sender, EventArgs e)
    {
        //lblMessasge.Text = "";
        try
        {
            Quantity(sender);
            //((TextBox)dgSV.Rows[dgSV.Rows.Count - 2].FindControl("txtSalesPrice")).Focus();
        }
        catch (FormatException fex)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('" + fex.Message + "','red',0);", true);

        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Database Maintain Error. Contact to the Software Provider..!!','red',0);", true);
            else ;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
        }
    }
    private void Quantity(object sender)
 {
        double tot = 0;
        double dis = 0;

        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        DataTable dt = (DataTable)ViewState["SV"];
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[gvr.DataItemIndex];

            double Qty = 0;
            double ClosingStock = 0;
            double SalesQty = 0;


            try
            {
                var qtyCk = Convert.ToDecimal(((TextBox)gvr.FindControl("txtQty")).Text);
            }
            catch
            {

                ((TextBox)gvr.FindControl("txtQty")).Text = "1";
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                       "alert('This Qty is not valid So 1  Qty is fixed');", true);
            }




            //if (string.IsNullOrEmpty(lblInvNo.Text))
            //{
            //    if (((Convert.ToDouble(dr["TotalClosingStock"])) <
            //         Convert.ToDouble(((TextBox)gvr.FindControl("txtQty")).Text)))
            //    {
            //        string Mgs = "Items Quantity Over This Closing Quantity.\\n Tolat Closing Qiantity : (" +
            //                      dr["TotalClosingStock"].ToString() + ")..!!";
            //        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
            //            "alert('" + Mgs + "');", true);
            //        ((TextBox)gvr.FindControl("txtQty")).Text = "0";


            //        return;
            //    }
            //}
            //else
            //{
                foreach (DataRow drRow in dt.Rows)
                {
                    if (dr["ID"].ToString().Equals(drRow["ID"].ToString()))
                    {
                        Qty += Convert.ToDouble(drRow["Qty"]);
                      //  SalesQty += Convert.ToDouble(drRow["SaleQty"]);
                    }
                }

                if (((Convert.ToDouble(dr["TotalClosingStock"]) + SalesQty) <
                     Convert.ToDouble(((TextBox)gvr.FindControl("txtQty")).Text)))
                {
                    string Mgs = "Items Quantity Over This Closing Quantity.\\n Tolat Closing Qiantity : (" +
                                  dr["TotalClosingStock"].ToString() + ")..!!";
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('" + Mgs + "','INDIANRED',2);", true);

                    ((TextBox)gvr.FindControl("txtQty")).Text = "1";

                    return;

                }
           // }

            dr["SPrice"] = ((TextBox)gvr.FindControl("txtSalesPrice")).Text;
            dr["Qty"] = ((TextBox)gvr.FindControl("txtQty")).Text;
            dr["DiscountAmount"] = ((TextBox)gvr.FindControl("txtDiscount")).Text;
            dr["Remarks"] = ((TextBox)gvr.FindControl("txtRemarks")).Text;
            tot = Convert.ToDouble(((TextBox)gvr.FindControl("txtSalesPrice")).Text) *
                  Convert.ToDouble(((TextBox)gvr.FindControl("txtQty")).Text);
            dis = (tot / 100) * Convert.ToDouble(((TextBox)gvr.FindControl("txtDiscount")).Text);
            dr["Total"] = (tot - dis).ToString("N2");

        }

        dgSV.DataSource = dt;
        dgSV.DataBind();
        ShowFooterTotal(dt);

    }

    protected void txtItems_TextChanged(object sender, EventArgs e)
    {
        bool IsChk = false;
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        DataTable DT1 = new DataTable();
        DT1 = (DataTable)ViewState["SV"];
        DataRow drindex = null;
        if (DT1 != null)
        {
            drindex = DT1.Rows[gvr.DataItemIndex];
        }

        string splitCode = IdManager.GetShowSingleValueString("Code", "Code+ ' - '+Name", "Item",
            ((TextBox)gvr.FindControl("txtItems")).Text);
        DataTable dt = SalesManager.GetShowItemsInformation(splitCode);
        //var splitCode = txtItemsCode.Text.Trim().Split(' ');
        //DataTable dt = SalesManager.GetShowItemsInformation(splitCode[0]);

        if (DT1 == null)
        {
            DT1 = dt;
        }
        else
        {
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in DT1.Rows)
                {
                    if (dr["Code"].ToString() == dt.Rows[0]["Code"].ToString() &&
                        !string.IsNullOrEmpty(dr["Code"].ToString()))
                    {
                        IsChk = true;
                        //((TextBox)gvr.FindControl("txtQty")).Text = (Convert.ToInt32(((TextBox)gvr.FindControl("txtQty")).Text) + 1).ToString();

                        break;
                    }
                }

                if (IsChk == false)
                {
                    if (drindex != null)
                    {
                        DT1.Rows.Remove(drindex);
                    }

                    DT1.ImportRow(dt.Rows[0]);

                }
            }
        }

        if (IsChk == false)
        {
            DataRow drs = DT1.NewRow();
            drs["Qty"] = "0";
            drs["Tax"] = "0";
            drs["DiscountAmount"] = "0";
            drs["Total"] = "0";
            DT1.Rows.Add(drs);

            dgSV.DataSource = DT1;
            ViewState["SV"] = DT1;
            dgSV.DataBind();
            ShowFooterTotal(DT1);
        }
        else
        {
            //ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('This Item already selected.!!');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('This Item already selected.!!','red',0);", true);

            ((TextBox)gvr.FindControl("txtItems")).Text = "";
        }

        txtItemsCode.Text = "";
        //UPCustomer.Update();
        //UpItemsDetails.Update();
        //pSearch.Update();
    }
    protected void dgSV_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["SV"] != null)
        {
            DataTable dtDtlGrid = (DataTable)ViewState["SV"];
            dtDtlGrid.Rows.RemoveAt(dgSV.Rows[e.RowIndex].DataItemIndex);
            if (dtDtlGrid.Rows.Count == 0)
            {
                DataRow dr = dtDtlGrid.NewRow();
                dr["txtItems"] = "";
                dr["Tax"] = 0;
                dr["DiscountAmount"] = 0;
                dr["Qty"] = 0;
                dr["Total"] = 0;
                dtDtlGrid.Rows.Add(dr);
            }

            dgSV.DataSource = dtDtlGrid;
            dgSV.DataBind();
            ShowFooterTotal(dtDtlGrid);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Your session is over. Try it again!!','red',0);", true);

        }
    }

    protected void dgPVMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header |
                e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].Attributes.Add("style", "display:none");
                e.Row.Cells[8].Attributes.Add("style", "display:none");
                e.Row.Cells[4].Attributes.Add("style", "display:none");
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
            else ;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (per.AllowAdd=="Y")
            {
                DataTable dtDtlGrid = (DataTable)ViewState["SV"];
                string loginby = Session["user"].ToString();
                var BranchId = Session["BranchId"].ToString();
                if (dtDtlGrid.Rows.Count > 0)
                {
                    int success = reject_mg.Save(dtDtlGrid, loginby, BranchId);
                    if (success > 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Record(s) is/are Save suceessfullly!!','green',1);", true);
                        Response.Redirect("frmRejectItem.aspx");
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Save Fail!!','red',0);", true);

                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('First Entry Item!!','red',0);", true);
                }  
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('You Are Not Granted User!!','red',0);", true); 
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
            else ;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
        }
    }
    protected void btnNew_Click(object sender, EventArgs e)
    {

    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        Response.Redirect("frmRejectItem.aspx");

    }
}