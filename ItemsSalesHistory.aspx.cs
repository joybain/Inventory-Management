using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Delve;
using System.Data.SqlClient;
using System.Globalization;
using sales;
using System.Drawing;

public partial class ItemsSalesHistory : System.Web.UI.Page
{
    private static Permis per;
    clsClientInfoManager _aclsClientInfoManager=new clsClientInfoManager();
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
            hfCustomerID.Value = ""; 
            OrderHistory(hfCustomerID.Value, DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), "");
            divHitory.Visible = false;
        }
    }

    private void OrderHistory(string CustomerID,string StartDate,string EndDate,string Flag)
    {
        string Parameter = "";
        if (string.IsNullOrEmpty(CustomerID) && !string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
        {
            Parameter = "WHERE convert(date,t1.OrderDate,103) between convert(date,'" + StartDate + "',103) AND convert(date,'" + EndDate + "',103)  ORDER BY t1.InvoiceNo DESC ";
        }
        else if (!string.IsNullOrEmpty(CustomerID) && !string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
        {
            Parameter = "WHERE t1.CustomerID='" + CustomerID + "' AND convert(date,t1.OrderDate,103) between convert(date,'" + StartDate + "',103) AND convert(date,'" + EndDate + "',103)  ORDER BY t1.InvoiceNo DESC ";
        }
        else if (!string.IsNullOrEmpty(CustomerID) && string.IsNullOrEmpty(StartDate) && string.IsNullOrEmpty(EndDate))
        {
            Parameter = "WHERE t1.CustomerID='" + CustomerID + "' ORDER BY t1.InvoiceNo DESC ";
        }
        DataTable dtDept = IdManager.GetShowDataTable("select t1.ID,t1.InvoiceNo,CONVERT(NVARCHAR,t1.OrderDate,103)OrderDate,t3.ContactName AS CustomerName,t2.BranchName,t1.SubTotal from [Order] t1 inner join BranchInfo t2 on t2.ID=t1.BranchID LEFT JOIN Customer t3 on t3.ID=t1.CustomerID " + Parameter);
        DataTable Program = new DataTable();
        Program.TableName = "dtProgram";
        Program.Columns.Add("ID", typeof(string));
        Program.Columns.Add("InvoiceNo", typeof(string));
        Program.Columns.Add("OrderDate", typeof(string));
        Program.Columns.Add("BranchName", typeof(string));
        Program.Columns.Add("CustomerName", typeof(string));
        Program.Columns.Add("SubTotal", typeof(string));
        Program.Columns.Add("dtDeptType", typeof(DataTable)); 
        
        // Call Depertment Type In dataTable

        DataRow drPro;
        DataTable dtProgramType;
        foreach (DataRow dr in dtDept.Rows)
        {
            drPro = Program.NewRow();
            drPro["ID"] = dr["ID"].ToString();
            drPro["InvoiceNo"] = dr["InvoiceNo"].ToString();
            drPro["OrderDate"] = dr["OrderDate"].ToString();
            drPro["CustomerName"] = dr["CustomerName"].ToString();
            drPro["BranchName"] = dr["BranchName"].ToString();
            drPro["SubTotal"] = dr["SubTotal"].ToString();
            dtProgramType = IdManager.GetShowDataTable("select t1.OrderID,t3.Code,t3.Name AS[ItemsName],convert(decimal(18,0),t1.Quantity) AS Quantity,convert(decimal(18,2),t1.SalePrice) AS SalePrice,convert(decimal(18,2),t1.TotalPrice)TotalPrice from OrderDetail t1 inner join ItemSalesStock t2 on t2.ID=t1.ItemID inner join Item t3 on t3.ID=t2.ItemsID WHERE t1.OrderID='" + dr["ID"].ToString() + "'");
            dtProgramType.TableName = "dtDeptType";
            drPro["dtDeptType"] = dtProgramType; // ADD This
            Program.Rows.Add(drPro);
        }
        if (Program.Rows.Count > 0)
        {
            dgDepartment.DataSource = Program;
            dgDepartment.DataBind();
            divHitory.Visible = true;
        }
        //else
        //{
        //   throw new Exception("No Items Foud..!!");
        //}
        // Depart ID          
    }
    protected void dgDepartment_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[5].Attributes.Add("style", "display:none");
            e.Row.Cells[7].Attributes.Add("style", "display:none");
            GridView gv = (GridView)e.Row.FindControl("dgSubMjrCat");
            DataTable dtDeptType = (DataTable)DataBinder.Eval(e.Row.DataItem, "dtDeptType");
            if (dtDeptType.Rows.Count == 0)
            {
                DataRow dr = dtDeptType.NewRow();
                dtDeptType.Rows.Add(dr);
            }
            gv.DataSource = dtDeptType;
            gv.DataBind();
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[5].Attributes.Add("style", "display:none");
            e.Row.Cells[7].Attributes.Add("style", "display:none");
        }
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[5].Attributes.Add("style", "display:none");
            e.Row.Cells[7].Attributes.Add("style", "display:none");
        }
    }
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(txtStartDateDate.Text) && string.IsNullOrEmpty(txtEndDate.Text))
            {
                txtEndDate.Text = txtStartDateDate.Text;
            }
            OrderHistory(hfCustomerID.Value, txtStartDateDate.Text, txtEndDate.Text, "1");
        }
        catch (Exception exception)
        {

            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                     "alert('" + exception.Message + "');", true);
        }
    }
    protected void txtEndDate_TextChanged(object sender, EventArgs e)
    {
        if (txtStartDateDate.Text != "")
        {
            DateTime dtStart = DateTime.ParseExact(txtStartDateDate.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dtEnd = DateTime.ParseExact(txtEndDate.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (dtStart > dtEnd)
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('End Date can not be Lower than Start Date..!!!');", true);
                txtEndDate.Text = string.Empty;
            }
        }
    }

    protected void txtStartDateDate_TextChanged(object sender, EventArgs e)
    {
        if (txtEndDate.Text != "")
        {
            DateTime dtStart = DateTime.ParseExact(txtStartDateDate.Text.Trim(), "dd/MM/yyyy",
                CultureInfo.InvariantCulture);
            DateTime dtEnd = DateTime.ParseExact(txtEndDate.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (dtStart > dtEnd)
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                    "alert('Start Date can not be longer than End Date ..!!!');", true);
                txtStartDateDate.Text = string.Empty;
            }
        }
    }
    protected void txtCustomer_TextChanged(object sender, EventArgs e)
    {
        DataTable dtCustomer = _aclsClientInfoManager.GetCustomerOnSearch("where UPPER(SearchName) = UPPER('" + txtCustomer.Text + "')",0);
        if (dtCustomer.Rows.Count > 0)
        {
            hfCustomerID.Value = dtCustomer.Rows[0]["ID"].ToString();
            txtCustomer.Text = dtCustomer.Rows[0]["ContactName"].ToString();
            Session["Customer_COA"] = dtCustomer.Rows[0]["Gl_CoaCode"].ToString();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step.!!');", true);
            Session["Customer_COA"] = null;
            hfCustomerID.Value = "";
            return;
        }
    }
    protected void dgDepartment_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Select")
        {
            GridViewRow gvr = (GridViewRow) (((LinkButton) e.CommandSource).NamingContainer);
            gvr.Cells[2].BackColor =
                gvr.Cells[3].BackColor = gvr.Cells[0].BackColor = gvr.Cells[1].BackColor = Color.Bisque;
            string InvoiceNo = gvr.Cells[2].Text.ToString().Trim();
            if (!string.IsNullOrEmpty(InvoiceNo))
            {
                Session["InvoiceNo"] = InvoiceNo;
                Response.Write("<script>");
                Response.Write("window.open('SalesReturn.aspx?mno=6.26','_blank')");
                Response.Write("</script>");
            }
        }
    }
}