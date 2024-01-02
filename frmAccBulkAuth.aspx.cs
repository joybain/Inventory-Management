using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Delve;
using System.Drawing;
//using cins;
public partial class frmAccBulkAuth : System.Web.UI.Page
{
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
                        cmd.CommandText =
                            "Select book_desc,company_address1,company_address2,separator_type,ShotName from gl_set_of_books where book_name='" +
                            Session["bookMAN"] + "' ";

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
                ((Label) Page.Master.FindControl("lblLogin")).Text = Session["wnote"].ToString();
                ((Label) Page.Master.FindControl("lblCountryName")).Text = Session["LoginCountry"].ToString();
                ((LinkButton) Page.Master.FindControl("lbLogout")).Visible = true;
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
            txtFromDt.Attributes.Add("onBlur", "formatdate('" + txtFromDt.ClientID + "')");
            txtToDt.Attributes.Add("onBlur", "formatdate('" + txtToDt.ClientID + "')");
            string criteria =
                " AND (select tt.UserType from dbo.UTL_USERINFO tt where tt.USER_NAME=ENTRY_USER)=(select tt.UserType from dbo.UTL_USERINFO tt where tt.USER_NAME='" +
                Session["user"].ToString() + "') order by convert(date,value_date,103) desc, vch_sys_no desc";
            if (Session["userlevel"].ToString().Equals("4"))
            {
                DataTable dtTable = VouchManager.GetVouchMstAuth(criteria);
                dgVoucher.DataSource = dtTable;
                dgVoucher.DataBind();
            }
            else if (Session["userlevel"].ToString().Equals("5"))
            {
                criteria = "";
                criteria = " order by convert(date,value_date,103) desc, vch_sys_no desc";
                DataTable dtTable = VouchManager.GetVouchMstAuth(criteria);
                dgVoucher.DataSource = dtTable;
                dgVoucher.DataBind();
            }
            DataTable dt1 = VouchManager.GetUserInfo(Session["user"].ToString());
            if (dt1.Rows.Count > 0)
            {
                ddlUser.DataSource = dt1;
                ddlUser.DataTextField = "DESCRIPTION";
                ddlUser.DataValueField = "USER_NAME";
                ddlUser.DataBind();
                ddlUser.Items.Insert(0, "");
            }
            if (dgVoucher.Rows.Count <= 0)
            {
                lblMessage.Text = " All Voucher are alradey Authorization..!!!";
                lblMessage.Visible = true;
            }
            else
            {
                lblMessage.Visible = false;
            }

            //UP2.Update();
            UpdatePanel1.Update();
        }
    }
    protected void dgVoucher_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        
    }
    protected void chkSelect_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chk = (CheckBox)sender;
        if (chk.Checked)
        {
            foreach (GridViewRow gvr in dgVoucher.Rows)
            {
                ((CheckBox)gvr.FindControl("chkSelect")).Checked = true;
            }
        }
        else
        {
            foreach (GridViewRow gvr in dgVoucher.Rows)
            {
                ((CheckBox)gvr.FindControl("chkSelect")).Checked = false;
            }
        }
    }
    protected void btnFind_Click(object sender, EventArgs e)
    {
        string criteria = " AND autho_user_type= " + Session["userlevel"].ToString() + " and value_date>= isnull(convert(datetime,'" + txtFromDt.Text + "',103),convert(datetime,'01/01/1960',103)) " +
             "and value_date <= isnull(convert(datetime,'" + txtToDt.Text + "',103),convert(datetime,'01/01/2050',103)) and upper(particulars) like upper('%" + txtParticulars.Text + "%') and upper(isnull(payee,'a')) like upper('%" + txtPayee.Text + "%')  order by value_date desc, vch_sys_no desc";
        DataTable dtTable = VouchManager.GetVouchMstAuth(criteria);
        dgVoucher.DataSource = dtTable;
        dgVoucher.DataBind();
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void btnAuth_Click(object sender, EventArgs e)
    {
        if (per.AllowAdd == "Y")
        {
            if (dgVoucher.Rows.Count > 0)
            {
                foreach (GridViewRow gvr in dgVoucher.Rows)
                {
                    if (((CheckBox) gvr.FindControl("chkSelect")).Checked)
                    {
                        VouchMst vch = VouchManager.GetVouchMst(gvr.Cells[2].Text.Trim());
                        if (vch != null)
                        {
                            vch.Status = "A";
                            vch.AuthoUser = Session["user"].ToString();
                            vch.AuthoDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                            vch.AuthoUserType = Session["userlevel"].ToString();
                            VouchManager.UpdatVoucherAuthorize(vch);
                           // VouchManager.UpdateVouchDtl(vch);
                        }
                    }
                }
                string criteria = " AND autho_user_type = " + Session["userlevel"].ToString() +
                                  " and value_date>= isnull(convert(datetime,'" + txtFromDt.Text +
                                  "',103),convert(datetime,'01/01/1960',103)) " +
                                  "and value_date <= isnull(convert(datetime,'" + txtToDt.Text +
                                  "',103),convert(datetime,'01/01/2050',103)) and upper(particulars) like upper('%" +
                                  txtParticulars.Text + "%') and upper(isnull(payee,'a')) like upper('%" + txtPayee.Text +
                                  "%')  order by value_date desc, vch_sys_no desc";
                DataTable dtTable = VouchManager.GetVouchMstAuth(criteria);
                dgVoucher.DataSource = dtTable;
                dgVoucher.DataBind();
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    "alert('Voucher(s) is/are authorized successfully!!');", true);
                //Response.Redirect("frmAccBulkAuth.aspx?mno=0.3");
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale",
                  "alert('You are not permitted in this step.\\n please contract your authorize person.!!');", true);
            return;
        }
    }
    protected void ddlUser_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlUser != null)
        {
            string criteria = " AND ENTRY_USER='" + ddlUser.SelectedValue + "' order by convert(date,value_date,103) desc, vch_sys_no desc";
            if (Session["userlevel"].ToString().Equals("4"))
            {
                DataTable dtTable = VouchManager.GetVouchMstAuth(criteria);
                dgVoucher.DataSource = dtTable;
                dgVoucher.DataBind();
            }
        }
        else
        {
            string criteria = " AND (select tt.UserType from dbo.UTL_USERINFO tt where tt.USER_NAME=ENTRY_USER)=(select tt.UserType from dbo.UTL_USERINFO tt where tt.USER_NAME='" + Session["user"].ToString() + "') order by value_date desc, vch_sys_no desc";
            if (Session["userlevel"].ToString().Equals("4"))
            {
                DataTable dtTable = VouchManager.GetVouchMstAuth(criteria);
                dgVoucher.DataSource = dtTable;
                dgVoucher.DataBind();
            }
        }
    }
    protected void dgVoucher_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "View")
        {
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            gvr.Cells[2].BackColor = gvr.Cells[3].BackColor = gvr.Cells[0].BackColor = gvr.Cells[1].BackColor = Color.Bisque;
            ModalPopupExtenderLogin.Show();
           
            
            DataTable dt = VouchManager.GetVouchDtl(gvr.Cells[2].Text.ToString().Trim(), "");
            if (dt.Rows.Count > 0)
            {
                lblPartuculars.Text = "Particulars : " + gvr.Cells[5].Text.ToString();
                dgVoucherDtl.Caption = "<b> Voucher No. : " + gvr.Cells[2].Text.ToString().Trim()+" </b>";
                dgVoucherDtl.DataSource = dt;
                ViewState["vouchdtl"] = dt;
                dgVoucherDtl.DataBind();
                GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
                TableCell cell;
                int j;
                if (dgVoucherDtl.Columns[0].Visible == true)
                {
                    j = dgVoucherDtl.Columns.Count - 3;
                }
                else
                {
                    j = dgVoucherDtl.Columns.Count - 4;
                }

                for (int i = 0; i < j; i++)
                {
                    cell = new TableCell();
                    cell.Text = "";
                    row.Cells.Add(cell);
                }
                cell = new TableCell();
                cell.Text = "Total";
                row.Cells.Add(cell);
                cell = new TableCell();
                decimal priceCr = decimal.Zero;
                decimal priceDr = decimal.Zero;

                cell = new TableCell();
                priceCr = GetTotal("amount_cr");
                cell.Text = priceCr.ToString("N3");
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);
                row.Font.Bold = true;

                cell = new TableCell();
                priceDr = GetTotal("amount_dr");
                cell.Text = priceDr.ToString("N3");
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);
                row.Font.Bold = true;
                dgVoucherDtl.Controls[0].Controls.Add(row);

               
            }
           
            //UP2.Update();
           // UpdatePanel1.Update();
        }
    }

    private decimal GetTotal(string ctrl)
    {
        decimal drt = 0;
        DataTable dt = (DataTable)ViewState["vouchdtl"];

        foreach (DataRow rowst in dt.Rows)
        {
            drt += decimal.Parse(string.IsNullOrEmpty(rowst[ctrl].ToString()) ? "0" : rowst[ctrl].ToString());
        }
        return drt;
    }
}