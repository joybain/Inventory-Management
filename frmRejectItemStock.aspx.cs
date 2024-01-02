using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;

public partial class frmRejectItemStock : System.Web.UI.Page
{
    private static Permis per;
    RejectItemManager _aClsItemDetailsManager = new RejectItemManager();
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
            Response.Redirect("Default.aspx?sid=sam");
        }

        if (!IsPostBack)
        {
            var BranchId = Session["BranchId"].ToString();
            if (string.IsNullOrEmpty(BranchId))
            {
                Response.Redirect("Default.aspx?sid=sam");
            }
            else
            {

                DataTable dtItems = _aClsItemDetailsManager.getShowBranchItemsInfo("", "",
                   "", "", "", "", "", "", "", BranchId);
                if (dtItems.Rows.Count > 0)
                {
                    dgItems.DataSource = dtItems;
                    ViewState["STK"] = dtItems;
                    dgItems.DataBind();
                    ShowFooterTotal();
                }
                

            }


        }
    }
    private void ShowFooterTotal()
    {

        decimal totQty = 0;
        DataTable dtStock = (DataTable)ViewState["STK"];
        totQty = Convert.ToDecimal(dtStock.Compute("Sum(TotalClosingStock)", ""));
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
        TableCell cell;
        cell = new TableCell();
        cell.Text = "<h3>Total Stock</h3>";
        cell.ColumnSpan = 9;
        cell.Font.Bold = true;
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        cell = new TableCell();
        cell.Text = "<h3>" + totQty.ToString("N2") + "</h3>";
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);

        row.Font.Bold = true;
        row.BackColor = System.Drawing.Color.LightGray;
        if (dgItems.Rows.Count > 0)
        {
            dgItems.Controls[0].Controls.Add(row);
        }

    }

  
    protected void dgItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgItems.DataSource = ViewState["STK"];
        dgItems.PageIndex = e.NewPageIndex;
        dgItems.DataBind();
    }
    protected void dgItems_SelectedIndexChanged(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "newWindow",
            "window.open('frmImageView.aspx?ID=" + dgItems.SelectedRow.Cells[7].Text + " &ItemsName=" +
            dgItems.SelectedRow.Cells[1].Text +
            "','_blank','status=1,toolbar=0,menubar=0,location=1,top=250,left=250px,width=500px,height=250px,directories=no,status=no, linemenubar=no,scrollbars=no,resizable=no ,modal=yes');",
            true);
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Attributes.Add("style", "display:none");
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                try
                {
                    if (!string.IsNullOrEmpty(e.Row.Cells[8].Text))
                    {
                        int totdate = (DataManager.DateEncode(e.Row.Cells[8].Text) -
                                       DataManager.DateEncode(DateTime.Now.ToString("dd/MM/yyyy"))).Days;
                        if (totdate < 0)
                        {
                            e.Row.Cells[8].BackColor = Color.OrangeRed;
                            e.Row.Cells[8].ForeColor = Color.White;
                        }
                        else if (totdate > 0 && totdate <= 7)
                        {
                            e.Row.Cells[8].BackColor = Color.Yellow;
                        }
                    }
                }
                catch
                {

                }

            }
            else if (e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].Attributes.Add("style", "display:none");
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
            else ;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
        }
    }
}