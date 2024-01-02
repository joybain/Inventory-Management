using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using System.Data;
using iTextSharp.text.pdf;
using sales;
using ClosedXML.Excel;
using System.Data.SqlClient;
using System.IO;
using iTextSharp.text.pdf.draw;
using Delve;

public partial class frmBranchInvReport : System.Web.UI.Page
{
    private static Permis per;
    public readonly clsClientInfoManager _aclsClientInfoManager = new clsClientInfoManager();
    public readonly clsItemTransferStockManager _aclsItemTransferStockManager = new clsItemTransferStockManager();
    public readonly ShiftmentItemsManager _aShiftmentItemsManager = new ShiftmentItemsManager();
    public readonly SalesManager _aSalesManager = new SalesManager();
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
                            userType = dReader["UserType"].ToString();
                            Session["BranchId"] = dReader["BranchId"].ToString();
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
                var BranchId = Session["BranchId"].ToString();
                if (string.IsNullOrEmpty(BranchId))

                {
                    Response.Redirect("Default.aspx?sid=sam");
                }
                else
                {

                    RefreshAll();
                }
            }
            catch 
            {
                Response.Redirect("Default.aspx?sid=sam");
            }
        }
    }

    private void RefreshAll()
    {
        rbCurDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        PnlSales.Visible = PnlItemsStock.Visible = pnlItemsTrans.Visible = PnlExpanse.Visible = pnlItemsStatus.Visible = PnlPayment.Visible = false;
        txtStartDate.Visible = txtEndDate.Visible = lblTo.Visible = false;
        //lblSupOrCus.Visible = 
        ddlSupplier.Visible = false;
        lblTo.Text = "TO";
        txtStartDate.Text = txtEndDate.Text = txtDate.Text = "";
        rbCurDate.Checked = rbByDate.Checked = rdbAll.Checked = rdbAvailable.Checked = rdbNotAvailable.Checked = false;
        lblCustomeSupplier.Text = "Search Customer";
        txtCustomer.Visible = rbByDateExpanse.Visible = true;
        txtSupplierSearch.Visible = false;
        txtCustomer.Text = txtSupplierSearch.Text = hfCustomerIDPayment.Value = string.Empty;
        rbCurDateExpanse.Text = "Today (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";
        rbReportType.Visible = lblCustomeSupplier.Visible = txtCustomer.Visible = txtSupplierSearch.Visible = false;
        txtStartDate.Attributes.Add("onBlur", "formatdate('" + txtStartDate.ClientID + "')");
        txtEndDate.Attributes.Add("onBlur", "formatdate('" + txtEndDate.ClientID + "')");
        txtDate.Attributes.Add("onBlur", "formatdate('" + txtDate.ClientID + "')");

        txtStartDate.Text = txtStartDatePayment.Text = txtStartDateExpanse.Text = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
        txtEndDate.Text = txtEndDatePayment.Text = txtEndDateExpanse.Text = rbCurDatePayment.Text = DateTime.Now.ToString("dd/MM/yyyy");
        //  RadioButtonList1_SelectedIndexChanged(null, null);
    }
    protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        RefreshAll();
        if (RadioButtonList1.SelectedValue == "Sales")
        {

            PnlExpanse.Visible = false;
            rbCurDateExpanse.Checked = false;
            rbByDateExpanse.Checked = true;
            //rbItemWise.Visible = true;
            lblCustomeSupplier.Visible = txtCustomer.Visible = txtSupplierSearch.Visible = true;
            PnlSales.Visible = true;
            lblCustomeSupplier.Text = "Search Customer";
            txtCustomer.Visible = true;
            txtSupplierSearch.Visible = false;
            txtCustomer.Text = txtSupplierSearch.Text = string.Empty;
            UPInvReport.Update();
        }

        if (RadioButtonList1.SelectedValue == "CD" || RadioButtonList1.SelectedValue == "CPI")
        {
            txtCustomerPayment.Visible = true;
            PnlPayment.Visible = true;
            rbCurDatePayment.Checked = false;
            rbByDatePayment.Checked = true;
            txtSupplierSearch.Visible = false;
            txtSupplierSearchDue.Visible = false;
            txtCustomerPayment.Text = string.Empty;
            lblToPayment.Visible = true;
            rbItemWise.Visible = false;

            UPInvReport.Update();
        }
        if (RadioButtonList1.SelectedValue == "SD")
        {
            txtCustomerPayment.Visible = false;
            txtSupplierSearchDue.Visible = true;
            PnlPayment.Visible = true;
            rbItemWise.Visible = false;

            rbCurDatePayment.Checked = false;
            rbByDatePayment.Checked = true;
            txtSupplierSearch.Visible = false;
            txtCustomerPayment.Text = txtSupplierSearchDue.Text = hfSupplierIDPayment.Value = string.Empty;
            lblToPayment.Visible = true;
            UPInvReport.Update();
        }

        if (RadioButtonList1.SelectedValue == "DE" || RadioButtonList1.SelectedValue == "DSSR")
        {
            PnlExpanse.Visible = true;
            rbItemWise.Visible = false;

            rbCurDateExpanse.Checked = false;
            rbByDateExpanse.Checked = true;
            txtCustomerPayment.Visible = false;
            txtSupplierSearchDue.Visible = false;
            PnlPayment.Visible = false;
            rbCurDatePayment.Checked = false;
            rbByDatePayment.Checked = false;
            txtSupplierSearch.Visible = false;
            txtCustomerPayment.Text = txtSupplierSearchDue.Text = hfSupplierIDPayment.Value = string.Empty;
            lblToPayment.Visible = false;
            UPInvReport.Update();
        }
        if (RadioButtonList1.SelectedValue == "DSS")
        {
            lblCustomeSupplier.Visible = txtCustomer.Visible = txtSupplierSearch.Visible = false;
            PnlSales.Visible = true;
            lblCustomeSupplier.Text = "Search Customer";
            txtCustomer.Visible = false;
            txtSupplierSearch.Visible = false;
            rbItemWise.Visible = false;

            txtCustomer.Text = txtSupplierSearch.Text = string.Empty;
            UPInvReport.Update();
        }
        else if (RadioButtonList1.SelectedValue == "POL")
        {
            lblCustomeSupplier.Visible = txtCustomer.Visible = txtSupplierSearch.Visible = true;
            PnlSales.Visible = true;
            lblCustomeSupplier.Text = "Search Customer";
            txtCustomer.Visible = true;
            txtSupplierSearch.Visible = false;
            rbItemWise.Visible = false;

            txtCustomer.Text = txtSupplierSearch.Text = string.Empty;
            UPInvReport.Update();
        }
        else if (RadioButtonList1.SelectedValue == "Purchase")
        {
            lblCustomeSupplier.Visible = txtCustomer.Visible = txtSupplierSearch.Visible = true;
            PnlSales.Visible = true;
            
            lblCustomeSupplier.Text = "";
            txtCustomer.Visible = false;
            txtSupplierSearch.Visible = false;
            txtCustomer.Text = txtSupplierSearch.Text = string.Empty;
            rbReportType.Visible = false;
            rbItemWise.Visible = false;

            UPInvReport.Update();
        }
        else if (RadioButtonList1.SelectedValue == "TI")
        {
            PnlSales.Visible = true;
            //  //lblCustomeSupplier.Text = "Search Supplier";
            // txtCustomer.Visible = false;
            // txtSupplierSearch.Visible = true;
            rbItemWise.Visible = false;

            txtCustomer.Text = txtSupplierSearch.Text = string.Empty;
            UPInvReport.Update();
        }
        else if (RadioButtonList1.SelectedValue.Equals("SWIS"))
        {
            pnlItemsStatus.Visible = true;
            rbItemWise.Visible = false;

            txtShiftmentNo.Text = txtItemsName.Text = string.Empty;
        }
    }
    //**************** Sales And Purchase Panel **********************//
    protected void rbByDate_CheckedChanged(object sender, EventArgs e)
    {
        txtStartDate.Visible = txtEndDate.Visible = lblTo.Visible = true;
        rbCurDate.Checked = false;
        UPInvReport.Update();
    }
    protected void rbCurDate_CheckedChanged(object sender, EventArgs e)
    {
        //lblSupOrCus.Visible = ddlSupplier.Visible = false;
        rbByDate.Checked = false;
        txtStartDate.Visible = txtEndDate.Visible = lblTo.Visible = false;
        txtStartDate.Text = txtEndDate.Text = "";
        UPInvReport.Update();
    }
    //*** Items Trans *****//
    protected void rdbAll_CheckedChanged(object sender, EventArgs e)
    {
        rdbAvailable.Checked = rdbNotAvailable.Checked = false;
        UPInvReport.Update();
    }
    protected void rdbAvailable_CheckedChanged(object sender, EventArgs e)
    {
        rdbAll.Checked = rdbNotAvailable.Checked = false;
        UPInvReport.Update();
    }
    protected void rdbNotAvailable_CheckedChanged(object sender, EventArgs e)
    {
        rdbAll.Checked = rdbAvailable.Checked = false;
        UPInvReport.Update();
    }
    protected void ddlCatagory_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlSubCatagory.DataSource = SubMajorCategoryManager.GetSubMajorCategories(ddlCatagory.SelectedValue);
        ddlSubCatagory.DataTextField = "Name";
        ddlSubCatagory.DataValueField = "ID";
        ddlSubCatagory.DataBind();
        ddlSubCatagory.Items.Insert(0, "");
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {

        try
        {
            // rbCurDate.Checked = rbByDate.Checked = rdbAll.Checked = rdbAvailable.Checked = rdbNotAvailable.Checked = false;
            if (RadioButtonList1.SelectedValue == "Purchase")
            {
                if (rbCurDate.Checked)
                {
                    string DDT = "";
                    if (!string.IsNullOrEmpty(txtSupplierSearch.Text))
                    {
                        DDT = "Report of (" + txtSupplierSearch.Text + ") From  (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";
                    }
                    else
                    {
                        DDT = "Report of (" + DateTime.Now.ToString("dd/MM/yyyy") + ") ";
                    }

                    PurchaseDetailsCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "C");
                }
                else if (rbByDate.Checked)
                {
                    string DDT = "";
                    if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && !string.IsNullOrEmpty(txtSupplierSearch.Text) && !string.IsNullOrEmpty(txtEndDate.Text))
                    {
                        DDT = "Report of (" + txtSupplierSearch.Text + ") From (" + txtStartDate.Text + " To " +
                              txtEndDate.Text + ")";
                    }
                    else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && string.IsNullOrEmpty(txtSupplierSearch.Text) && string.IsNullOrEmpty(txtEndDate.Text))
                    {
                        DDT = "Report of (" + txtSupplierSearch.Text + ") From (" + txtStartDate.Text + " To " +
                              txtEndDate.Text + ")";
                    }
                    else
                    {
                        DDT = "Report of (" + txtStartDate.Text + " To " +
                              txtEndDate.Text + ")";
                    }
                    PurchaseDetailsCurrentDate(DDT, "", "D");
                }

            }

            if (RadioButtonList1.SelectedValue == "DE")
            {
                if (rbCurDateExpanse.Checked)
                {
                    string DDT = "";

                    DataTable dt = ExperManager.GetBranchShowExpensesReport(DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"),Session["BranchId"].ToString());

                    {
                        DDT = "Report of (" + DateTime.Now.ToString("dd/MM/yyyy") + ") ";
                    }

                    if (rbPrintTypeExpanse.SelectedValue.Equals("P"))
                    {
                        DailyExpenseReport(DDT, dt);
                    }
                    else
                    {

                        if (dt.Rows.Count > 0)
                        {

                            dt.Columns.Remove("ID");
                            //ddt.Columns.Remove("InvoiceNo");
                            dt.Columns.Remove("MstID");
                            dt.Columns.Remove("ExpensesHeadID");

                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                wb.Worksheets.Add(dt, "PurchaseSummery");
                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition",
                                    "attachment;filename=Daily_Expanse" + "-Purchasetatement-(" +
                                    DateTime.Now.ToString("dd/MM/yyyy") + ").xlsx");

                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {
                                    wb.SaveAs(MyMemoryStream);
                                    MyMemoryStream.WriteTo(Response.OutputStream);
                                    Response.Flush();
                                    Response.End();
                                }
                            }
                        }
                    }
                }
                else if (rbByDateExpanse.Checked)
                {
                    string DDT = "";

                    DataTable dt = ExperManager.GetBranchShowExpensesReport(txtStartDateExpanse.Text, txtEndDateExpanse.Text, Session["BranchId"].ToString());

                    {
                        DDT = "Report on " + txtStartDateExpanse.Text + " to " + txtEndDateExpanse.Text;
                    }

                    if (rbPrintTypeExpanse.SelectedValue.Equals("P"))
                    {
                        DailyExpenseReport(DDT, dt);
                    }
                    else
                    {

                        if (dt.Rows.Count > 0)
                        {

                            dt.Columns.Remove("ID");
                            //ddt.Columns.Remove("InvoiceNo");
                            dt.Columns.Remove("MstID");
                            dt.Columns.Remove("ExpensesHeadID");

                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                wb.Worksheets.Add(dt, "PurchaseSummery");
                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition",
                                    "attachment;filename=Daily_Expanse" + "-Purchasetatement-(" +
                                    DateTime.Now.ToString("dd/MM/yyyy") + ").xlsx");

                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {
                                    wb.SaveAs(MyMemoryStream);
                                    MyMemoryStream.WriteTo(Response.OutputStream);
                                    Response.Flush();
                                    Response.End();
                                }
                            }
                        }
                    }
                }


                if (RadioButtonList1.SelectedValue == "DSS")
                {
                    if (rbCurDateExpanse.Checked)
                    {
                        string DDT = "";

                        DataTable dt = ExperManager.GetBranchShowExpensesReport(DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), Session["BranchId"].ToString());

                        {
                            DDT = "Report of (" + DateTime.Now.ToString("dd/MM/yyyy") + ") ";
                        }

                        if (rbPrintTypeExpanse.SelectedValue.Equals("P"))
                        {
                            DailyExpenseReport(DDT, dt);
                        }
                        else
                        {

                            if (dt.Rows.Count > 0)
                            {

                                dt.Columns.Remove("ID");
                                //ddt.Columns.Remove("InvoiceNo");
                                dt.Columns.Remove("MstID");
                                dt.Columns.Remove("ExpensesHeadID");

                                using (XLWorkbook wb = new XLWorkbook())
                                {
                                    wb.Worksheets.Add(dt, "PurchaseSummery");
                                    Response.Clear();
                                    Response.Buffer = true;
                                    Response.Charset = "";
                                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                    Response.AddHeader("content-disposition",
                                        "attachment;filename=Daily_Expanse" + "-Purchasetatement-(" +
                                        DateTime.Now.ToString("dd/MM/yyyy") + ").xlsx");

                                    using (MemoryStream MyMemoryStream = new MemoryStream())
                                    {
                                        wb.SaveAs(MyMemoryStream);
                                        MyMemoryStream.WriteTo(Response.OutputStream);
                                        Response.Flush();
                                        Response.End();
                                    }
                                }
                            }
                        }
                    }
                    else if (rbByDateExpanse.Checked)
                    {
                        string DDT = "";

                        DataTable dt = ExperManager.GetBranchShowExpensesReport(txtStartDateExpanse.Text, txtEndDateExpanse.Text,Session["BranchId"].ToString());

                        {
                            DDT = "Report on " + txtStartDateExpanse.Text + " to " + txtEndDateExpanse.Text;
                        }

                        if (rbPrintTypeExpanse.SelectedValue.Equals("P"))
                        {
                            DailyExpenseReport(DDT, dt);
                        }
                        else
                        {

                            if (dt.Rows.Count > 0)
                            {

                                dt.Columns.Remove("ID");
                                //ddt.Columns.Remove("InvoiceNo");
                                dt.Columns.Remove("MstID");
                                dt.Columns.Remove("ExpensesHeadID");

                                using (XLWorkbook wb = new XLWorkbook())
                                {
                                    wb.Worksheets.Add(dt, "PurchaseSummery");
                                    Response.Clear();
                                    Response.Buffer = true;
                                    Response.Charset = "";
                                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                    Response.AddHeader("content-disposition",
                                        "attachment;filename=Daily Expanse" + "-Purchasetatement-(" +
                                        DateTime.Now.ToString("dd/MM/yyyy") + ").xlsx");

                                    using (MemoryStream MyMemoryStream = new MemoryStream())
                                    {
                                        wb.SaveAs(MyMemoryStream);
                                        MyMemoryStream.WriteTo(Response.OutputStream);
                                        Response.Flush();
                                        Response.End();
                                    }
                                }
                            }
                        }
                    }

                }
            }
            else if (RadioButtonList1.SelectedValue == "Sales")
            {
                string DDT = "";
                if (rbCurDate.Checked)
                {
                    DDT = "From (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";
                    SalesDetailsCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "C");
                }
                else if (rbByDate.Checked)
                {
                    if (string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text != "" && txtEndDate.Text != "")
                    {
                        DDT = " From (" + txtStartDate.Text + " TO " + txtEndDate.Text + ")";
                    }
                    else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text == "" && txtEndDate.Text == "")
                    {
                        DDT = "Report of  (" + txtSupplierSearch.Text + ") ";
                    }
                    else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text == "" && txtEndDate.Text == "")
                    {
                        DDT = "Report of  (" + txtSupplierSearch.Text + ") From (" + txtStartDate.Text + " To " + txtEndDate.Text + ")";
                    }
                    SalesDetailsCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "D");
                }
            }

            else if (RadioButtonList1.SelectedValue == "DSSR")
            {
                string DDT = "";
                if (rbCurDateExpanse.Checked)
                {

                    DDT = "From (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";
                    SalesSummeryCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "C");
                }

                else if (rbByDateExpanse.Checked)
                {

                    if (txtStartDateExpanse.Text != "" && txtEndDateExpanse.Text != "")
                    {
                        DDT = " From (" + txtStartDateExpanse.Text + " TO " + txtEndDateExpanse.Text + ")";
                    }
                    else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDateExpanse.Text == "" && txtEndDateExpanse.Text == "")
                    {
                        DDT = "Report of  (" + txtSupplierSearch.Text + ") ";
                    }
                    else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDateExpanse.Text == "" && txtEndDateExpanse.Text == "")
                    {
                        DDT = "Report of  (" + txtSupplierSearch.Text + ") From (" + txtStartDateExpanse.Text + " To " + txtEndDateExpanse.Text + ")";
                    }
                    SalesSummeryCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "D");
                }
            }
            else if (RadioButtonList1.SelectedValue == "CPI")
            {
                if (ddlReportType.SelectedValue == "DR")
                {
                    if (string.IsNullOrEmpty(hfCustomerIDPayment.Value))
                    {
                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                            "alert('Please Select Customer..!!');", true);
                        return;
                    }
                    string DDT = "";
                    if (rbCurDatePayment.Checked)
                    {
                        DDT = "  On " + DateTime.Now.ToString("dd/MM/yyyy") + "";
                        DataTable dt = _aSalesManager.GetBranchCutomerPaymet(DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), hfCustomerIDPayment.Value,Session["BranchId"].ToString());
                        if (dt.Rows.Count > 0)
                        {
                            CustomerPaymentReport(DDT, dt);
                        }
                    }
                    else if (rbByDatePayment.Checked)
                    {
                        DDT = "From " + txtStartDatePayment.Text + " to " + txtEndDatePayment.Text + "";
                        DataTable dt = _aSalesManager.GetBranchCutomerPaymet(txtStartDatePayment.Text, txtEndDatePayment.Text, hfCustomerIDPayment.Value,Session["BranchId"].ToString());
                        if (dt.Rows.Count > 0)
                        {
                            CustomerPaymentReport(DDT, dt);
                        }
                    }
                }
                else
                {
                    string DDT = "";
                    if (rbCurDatePayment.Checked)
                    {
                        DDT = "  On " + DateTime.Now.ToString("dd/MM/yyyy") + "";
                        DataTable dt = _aSalesManager.GetBranchCutomerPaymentSummery(DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), hfCustomerIDPayment.Value,Session["BranchId"].ToString());
                        if (dt.Rows.Count > 0)
                        {
                            CustomerPaymentSummeryReport(DDT, dt);
                        }
                    }
                    else if (rbByDatePayment.Checked)
                    {
                        DDT = "From " + txtStartDatePayment.Text + " to " + txtEndDatePayment.Text + "";
                        DataTable dt = _aSalesManager.GetBranchCutomerPaymentSummery(txtStartDatePayment.Text, txtEndDatePayment.Text, hfCustomerIDPayment.Value, Session["BranchId"].ToString());
                        if (dt.Rows.Count > 0)
                        {
                            CustomerPaymentSummeryReport(DDT, dt);
                        }
                    }
                }
            }
            // Customer DUe Report
            else if (RadioButtonList1.SelectedValue == "CD")
            {
                if (ddlReportType.SelectedValue == "DR")
                {
                    if (string.IsNullOrEmpty(hfCustomerIDPayment.Value))
                    {
                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                            "alert('Please Select Customer..!!');", true);
                        return;
                    }
                    string DDT = "";
                    if (rbCurDatePayment.Checked)
                    {
                        DDT = "  On " + DateTime.Now.ToString("dd/MM/yyyy") + "";
                        DataTable dt1 = _aSalesManager.GetBranchSaleInfo(DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), hfCustomerIDPayment.Value,Session["BranchId"].ToString());
                        DataTable dt = _aSalesManager.GetBranchCutomerPaymet(DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), hfCustomerIDPayment.Value,Session["BranchId"].ToString());
                        if (dt1.Rows.Count > 0)
                        {
                            CustomerPaymentANdDueReport(DDT, dt, dt1);

                        }
                    }
                    else if (rbByDatePayment.Checked)
                    {
                        DDT = "From " + txtStartDate.Text + " to " + txtEndDate.Text + "";
                        DataTable dt1 = _aSalesManager.GetBranchSaleInfo(txtStartDatePayment.Text, txtEndDatePayment.Text, hfCustomerIDPayment.Value,Session["BranchId"].ToString());
                        DataTable dt = _aSalesManager.GetBranchCutomerPaymet(txtStartDatePayment.Text, txtEndDatePayment.Text, hfCustomerIDPayment.Value, Session["BranchId"].ToString());
                        if (dt1.Rows.Count > 0)
                        {
                            CustomerPaymentANdDueReport(DDT, dt, dt1);

                        }
                    }
                }

                else
                {
                    string DDT = "";
                    if (rbCurDatePayment.Checked)
                    {
                        DDT = "  On " + DateTime.Now.ToString("dd/MM/yyyy") + "";
                        DataTable dt = _aSalesManager.GetBranchCutomerPaymetSummery(DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), hfCustomerIDPayment.Value,Session["BranchId"].ToString());
                        if (dt.Rows.Count > 0)
                        {
                            CustomerPaymentANdDueReportSummery(DDT, dt);

                        }
                    }
                    else if (rbByDatePayment.Checked)
                    {
                        DDT = "From " + txtStartDate.Text + " to " + txtEndDate.Text + "";
                        DataTable dt = _aSalesManager.GetBranchCutomerPaymetSummery(txtStartDatePayment.Text, txtEndDatePayment.Text, hfCustomerIDPayment.Value,Session["BranchId"].ToString());
                        if (dt.Rows.Count > 0)
                        {
                            CustomerPaymentANdDueReportSummery(DDT, dt);

                        }
                    }
                }
            }
            else if (RadioButtonList1.SelectedValue == "SD")
            {
                if (ddlReportType.SelectedValue == "DR")
                {
                    if (string.IsNullOrEmpty(hfSupplierIDPayment.Value))
                    {
                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                            "alert('Please Select Supplier..!!');", true);
                        return;
                    }
                    string DDT = "";
                    if (rbCurDatePayment.Checked)
                    {
                        DDT = "  On " + DateTime.Now.ToString("dd/MM/yyyy") + "";
                        DataTable dt1 = _aSalesManager.GetPurchaseInfo(DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), hfSupplierIDPayment.Value);
                        DataTable dt = _aSalesManager.GetSupplierPaymet(DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), hfSupplierIDPayment.Value);
                        if (dt1.Rows.Count > 0)
                        {
                            SupplierPaymentANdDueReport(DDT, dt, dt1);

                        }
                    }
                    else if (rbByDatePayment.Checked)
                    {
                        DDT = "From " + txtStartDate.Text + " to " + txtEndDate.Text + "";
                        DataTable dt1 = _aSalesManager.GetPurchaseInfo(txtStartDatePayment.Text, txtEndDatePayment.Text, hfSupplierIDPayment.Value);
                        DataTable dt = _aSalesManager.GetSupplierPaymet(txtStartDatePayment.Text, txtEndDatePayment.Text, hfSupplierIDPayment.Value);
                        if (dt1.Rows.Count > 0)
                        {
                            SupplierPaymentANdDueReport(DDT, dt, dt1);

                        }
                    }
                }

                else
                {
                    string DDT = "";
                    if (rbCurDatePayment.Checked)
                    {
                        DDT = "  On " + DateTime.Now.ToString("dd/MM/yyyy") + "";
                        DataTable dt = _aSalesManager.GetSupplierPaymetSummery(DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), hfSupplierIDPayment.Value);
                        if (dt.Rows.Count > 0)
                        {
                            SupplierPaymentANdDueReportSummery(DDT, dt);

                        }
                    }
                    else if (rbByDatePayment.Checked)
                    {
                        DDT = "From " + txtStartDate.Text + " to " + txtEndDate.Text + "";
                        DataTable dt = _aSalesManager.GetSupplierPaymetSummery(txtStartDatePayment.Text, txtEndDatePayment.Text, hfSupplierIDPayment.Value);
                        if (dt.Rows.Count > 0)
                        {
                            SupplierPaymentANdDueReportSummery(DDT, dt);

                        }
                    }
                }
            }

            else if (RadioButtonList1.SelectedValue == "POL")
            {
                if (rbPrintType.SelectedValue == "P")
                {
                    string DDT = "";
                    if (rbCurDate.Checked)
                    {
                        DDT = "From (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";
                        PoLSalesDetailsCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "C");
                    }
                    else if (rbByDate.Checked)
                    {
                        if (string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text != "" && txtEndDate.Text != "")
                        {
                            DDT = " From (" + txtStartDate.Text + " TO " + txtEndDate.Text + ")";

                        }
                        else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text == "" && txtEndDate.Text == "")
                        {
                            DDT = "Report of  (" + txtSupplierSearch.Text + ") ";

                        }
                        else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text == "" && txtEndDate.Text == "")
                        {
                            DDT = "Report of  (" + txtSupplierSearch.Text + ") From (" + txtStartDate.Text + " To " + txtEndDate.Text + ")";

                        }
                        PoLSalesDetailsCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "D");
                    }
                }
                else
                {
                    DataTable dt = null;
                    if (rbCurDate.Checked)
                    {
                        dt = PurchaseVoucherManager.GetShowSalesReportReport("2",
                            DateTime.Now.ToString("dd/MMM/yyyy"), DateTime.Now.ToString("dd/MMM/yyyy"),
                            hfCustomerID.Value,Session["BranchId"].ToString());
                    }
                    else
                    {
                        dt = PurchaseVoucherManager.GetShowSalesReportReport("2", txtStartDate.Text,
                            txtEndDate.Text,
                            hfCustomerID.Value,Session["BranchId"].ToString());
                    }
                    if (dt.Rows.Count > 0)
                    {

                        dt.Columns.Remove("ID");
                        dt.Columns.Remove("CustomerID");
                        dt.Columns.Remove("CusID");

                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            wb.Worksheets.Add(dt, "ProfitAndLoss");
                            Response.Clear();
                            Response.Buffer = true;
                            Response.Charset = "";
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition",
                                "attachment;filename=ProfitLossStatement-(" +
                                DateTime.Now.ToString("dd/MM/yyyy") + ").xlsx");

                            using (MemoryStream MyMemoryStream = new MemoryStream())
                            {
                                wb.SaveAs(MyMemoryStream);
                                MyMemoryStream.WriteTo(Response.OutputStream);
                                Response.Flush();
                                Response.End();
                            }
                        }
                    }
                }
            }




            // **Dmage** //
            else if (RadioButtonList1.SelectedValue == "DSS")
            {
                string DDT = "";
                if (rbCurDate.Checked)
                {
                    DDT = "Damage/Short Items on Date : (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";
                    DmageDetailsCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "C");
                }
                else if (rbByDate.Checked)
                {
                    if (string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text != "" && txtEndDate.Text != "")
                    {
                        DDT = "Damage/Short Items on Date :  From (" + txtStartDate.Text + " TO " + txtEndDate.Text + ")";

                    }
                    DmageDetailsCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "D");
                }
            }
            else if (RadioButtonList1.SelectedValue == "TI")
            {
                string DDT = "";
                if (rbCurDate.Checked)
                {
                    DDT = "Items Transfer From (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";
                    TransferItemCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "C");
                }
                else if (rbByDate.Checked)
                {
                    if (string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text != "" && txtEndDate.Text != "")
                    {
                        DDT = "Items Transfer From (" + txtStartDate.Text + " TO " + txtEndDate.Text + ")";

                    }
                    else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text == "" && txtEndDate.Text == "")
                    {
                        DDT = "Items Transfer Report of  (" + txtSupplierSearch.Text + ") ";

                    }
                    else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text == "" && txtEndDate.Text == "")
                    {
                        DDT = "Items Transfer Report of  (" + txtSupplierSearch.Text + ") From (" + txtStartDate.Text + " To " + txtEndDate.Text + ")";

                    }
                    TransferItemCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "D");
                }
            }
            else if (RadioButtonList1.SelectedValue.Equals("SWIS"))
            {
                ShipmentWiseItemsStatus();
            }
            //else if (RadioButtonList1.SelectedValue == "IS")
            //{
            //    string DDT = "";
            //    if (rdbAll.Checked)
            //    {
            //        DDT = "All Items Stock";
            //        if (ddlCatagory.SelectedItem.Text != "" && ddlCatagory.SelectedItem.Text == "") { DDT = DDT + " of Catagory (" + ddlCatagory.SelectedItem.Text + ")"; }
            //        if (ddlCatagory.SelectedItem.Text != "" && ddlCatagory.SelectedItem.Text != "") {
            //            DDT = DDT + " of (" + ddlCatagory.SelectedItem.Text + " And Sub Catagory" + ddlSubCatagory.SelectedItem.Text +")"; }
            //        Total_Stock("all", ddlCatagory.SelectedValue, ddlSubCatagory.SelectedValue,DDT);
            //    }
            //    if (rdbAvailable.Checked)
            //    {
            //        DDT = "Available Items Stock";
            //        if (ddlCatagory.SelectedItem.Text != "" && ddlCatagory.SelectedItem.Text == "") { DDT = DDT + " of Catagory (" + ddlCatagory.SelectedItem.Text + ")"; }
            //        if (ddlCatagory.SelectedItem.Text != "" && ddlCatagory.SelectedItem.Text != "")
            //        {
            //            DDT = DDT + " of Catagory (" + ddlCatagory.SelectedItem.Text + " And Sub Catagory" + ddlSubCatagory.SelectedItem.Text + ")";
            //        }
            //        Total_Stock("Available", ddlCatagory.SelectedValue, ddlSubCatagory.SelectedValue, DDT);
            //    }
            //    if (rdbNotAvailable.Checked)
            //    {
            //        DDT = "Unavailable Items Stock";
            //        if (ddlCatagory.SelectedItem.Text != "" && ddlCatagory.SelectedItem.Text == "") { DDT = DDT + " of Catagory (" + ddlCatagory.SelectedItem.Text + ")"; }
            //        if (ddlCatagory.SelectedItem.Text != "" && ddlCatagory.SelectedItem.Text != "")
            //        {
            //            DDT = DDT + " of Catagory (" + ddlCatagory.SelectedItem.Text + " And Sub Catagory" + ddlSubCatagory.SelectedItem.Text + ")";
            //        }
            //        Total_Stock("Unavailable", ddlCatagory.SelectedValue, ddlSubCatagory.SelectedValue, DDT);
            //    }
            //}
            //else if (RadioButtonList1.SelectedValue == "TIQ")
            //{          

            //    if (rbCurDate.Checked)
            //    {
            //        string DDT = DateTime.Now.ToString("dd/MM/yyyy");
            //        if (txtStartDate.Text == "" && txtEndDate.Text == "") { DDT = "Total Quantity of Items Details From (" + txtStartDate.Text + ")"; } else { DDT = "Total Quantity of Items Details From  (" + txtStartDate.Text + " To " + txtEndDate.Text + ")"; }
            //        TotalItemsStockDateWise(DDT, DateTime.Now.ToString("dd/MM/yyyy"));
            //    }
            //    else if (rbByDate.Checked)
            //    {
            //        string DDT = DateTime.Now.ToString("dd/MM/yyyy");
            //        if (txtStartDate.Text == "" && txtEndDate.Text == "") { DDT = "Total Quantity of Items Details From (" + txtStartDate.Text + ")"; } else { DDT = "Total Quantity of Items Details From  (" + txtStartDate.Text + " To " + txtEndDate.Text + ")"; }
            //        TotalItemsStockDateWise(DDT, "");
            //    }
            //}
            //else
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('You select Any one option..!!');", true);
            //}
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

    private void CustomerPaymentANdDueReportSummery(string DDT, DataTable dt)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename='SalesOrInvoice-(" + DateTime.Now.ToString("dd-MMM-yyyy") + ")'.pdf");
        Document document = new Document(PageSize.A4, 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(5f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        string Head = "";

        cell =
           new PdfPCell(new Phrase(" CUSTOMER DUE INFORMATION ",
               FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(" Date " + DDT,
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);


        float[] widthdtl = null;

        widthdtl = new float[7] { 11, 35, 20, 25, 20, 20, 20 };

        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 40f;
        cell.Border = 0;
        cell.Colspan = 7;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Customer Name"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Mobile No"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Email"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase("Sales"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Payment"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase("Due"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        double totsales = 0;
        double tot = 0;
        double Vat = 0;
        double Discount = 0;
        double due = 0;

        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell =
                new PdfPCell(
                    FormatPhrase((dr["ContactName"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell =
                new PdfPCell(
                    FormatPhrase((dr["Mobile"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell =
                new PdfPCell(
                    FormatPhrase((dr["Email"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);



            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Sales"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Payment"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);



            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Due"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            totsales += Convert.ToDouble(dr["Sales"]);
            tot += Convert.ToDouble(dr["Payment"]);
            due += Convert.ToDouble(dr["Due"]);

        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;

        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totsales.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(due.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }
    private void SupplierPaymentANdDueReportSummery(string DDT, DataTable dt)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename='SalesOrInvoice-(" + DateTime.Now.ToString("dd-MMM-yyyy") + ")'.pdf");
        Document document = new Document(PageSize.A4, 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(5f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        string Head = "";

        cell =
           new PdfPCell(new Phrase(" SUPPLIER DUE INFORMATION ",
               FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(" Date " + DDT,
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);


        float[] widthdtl = null;

        widthdtl = new float[7] { 11, 35, 20, 25, 20, 20, 20 };

        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 40f;
        cell.Border = 0;
        cell.Colspan = 7;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Supplier Name"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Mobile No"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Email"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase("Purchase"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Payment"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase("Due"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        double totsales = 0;
        double tot = 0;
        double Vat = 0;
        double Discount = 0;
        double due = 0;

        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell =
                new PdfPCell(
                    FormatPhrase((dr["ContactName"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell =
                new PdfPCell(
                    FormatPhrase((dr["Mobile"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell =
                new PdfPCell(
                    FormatPhrase((dr["Email"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);



            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Purchase"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Payment"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);



            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Due"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            totsales += Convert.ToDouble(dr["Purchase"]);
            tot += Convert.ToDouble(dr["Payment"]);
            due += Convert.ToDouble(dr["Due"]);

        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;

        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totsales.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(due.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }

    private void CustomerPaymentSummeryReport(string DDT, DataTable dt)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename='SalesOrInvoice-(" + DateTime.Now.ToString("dd-MMM-yyyy") + ")'.pdf");
        Document document = new Document(PageSize.A4, 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(5f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        string Head = "";

        cell =
           new PdfPCell(new Phrase(" CUSTOMER PAYMENT INFORMATION ",
               FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(" Date " + DDT,
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);


        float[] widthdtl = null;

        widthdtl = new float[5] { 11, 60, 30, 30, 35 };

        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 40f;
        cell.Border = 0;
        cell.Colspan = 5;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Customer Name"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Mobile No"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Email"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase("Total Pay Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        double totQty = 0;
        double tot = 0;
        double Vat = 0;
        double Discount = 0;
        double ReceivedAmt = 0;

        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell =
                new PdfPCell(
                    FormatPhrase((dr["ContactName"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell =
                new PdfPCell(
                    FormatPhrase((dr["Mobile"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell =
                new PdfPCell(
                    FormatPhrase((dr["Email"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);



            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Payment"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            tot += Convert.ToDouble(dr["Payment"]);

        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;

        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }

    private void ShipmentWiseItemsStatus()
    {
        DataTable dtItemsList = _aShiftmentItemsManager.GetShowSipmentItemsStatus(lblShiftmentID.Text,
            lblItemsId.Text);
        if (dtItemsList.Rows.Count > 0)
        {
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition",
                "attachment; filename=ShipmentItemsPurchase(" + DateTime.Now.ToString("dd-MMM-yyyy") + ").pdf");
            Document document = new Document(PageSize.LEGAL.Rotate(), 20f, 20f, 30f, 20f);
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();
            Rectangle page = document.PageSize;
            PdfPTable head = new PdfPTable(1);
            head.TotalWidth = page.Width - 50;
            Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
            PdfPCell c = new PdfPCell(phrase);
            c.Border = Rectangle.NO_BORDER;
            c.VerticalAlignment = Element.ALIGN_BOTTOM;
            c.HorizontalAlignment = Element.ALIGN_RIGHT;
            head.AddCell(c);
            head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

            PdfPCell cell;
            byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
            iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
            gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
            gif.ScalePercent(8f);

            float[] titwidth = new float[2] { 10, 200 };
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;


            cell = new PdfPCell(gif);
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Rowspan = 4;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase(Session["org"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;

            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase(Session["add1"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;

            dth.AddCell(cell);
            cell =
                new PdfPCell(new Phrase(Session["add2"].ToString(),
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);

            cell =
                new PdfPCell(new Phrase("Shipment Wise Item's Status.",
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 7;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 30f;
            dth.AddCell(cell);
            document.Add(dth);
            LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
            document.Add(line);

            PdfPTable dtempty = new PdfPTable(1);
            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.BorderWidth = 0f;
            cell.FixedHeight = 20f;
            dtempty.AddCell(cell);
            document.Add(dtempty);

            float[] widthdtl = new float[13] { 8, 15, 15, 25, 15, 15, 15, 15, 15, 15, 15, 15, 15 };
            PdfPTable pdtdtl = new PdfPTable(widthdtl);
            pdtdtl.WidthPercentage = 100;
            pdtdtl.HeaderRows = 1;
            int Serial = 1;
            cell = new PdfPCell(FormatHeaderPhrase("SL."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Shipment No."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Items Code"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Name"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Label"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Supplier"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Ship. Rate"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Ship. Qty."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Sales Qty."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Sales Rtn."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Transfer Qty."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Transfer Rtn."));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Closing Stock"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            decimal ShipQty = 0;
            decimal SalesQty = 0;
            decimal RtnQty = 0;
            decimal transferQty = 0;
            decimal TransRtnQty = 0;
            decimal ClosingStock = 0;

            string itemsID = string.Empty;
            string ItemsIdShip = string.Empty;
            int chk = 0, borderChk = 0, subChk = 0;
            foreach (DataRow dr in dtItemsList.Rows)
            {
                if (chk.Equals(0))
                {
                    itemsID = dr["ShipItemID"].ToString();
                    ItemsIdShip = dr["StkItemsID"].ToString();
                }
                cell = new PdfPCell(FormatPhrase(Serial.ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                Serial++;

                cell = new PdfPCell(FormatPhrase(dr["ShiftmentNO"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["ItemName"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Label"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Supplier"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["PartyRate"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["ShipQty"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);


                if (subChk == 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["SalesQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;

                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else if (!ItemsIdShip.Equals(dr["StkItemsID"].ToString()) && dtItemsList.Rows.Count > 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["SalesQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;

                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthBottom = .5f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;


                    //cell.BorderWidthBottom = .5f;
                }
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0;
                pdtdtl.AddCell(cell);


                if (subChk == 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["RtnQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;

                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else if (!ItemsIdShip.Equals(dr["StkItemsID"].ToString()) && dtItemsList.Rows.Count > 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["RtnQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthBottom = .5f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                }
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                //cell.BorderWidth = 0;
                pdtdtl.AddCell(cell);

                if (subChk == 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["transferQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;

                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else if (!ItemsIdShip.Equals(dr["StkItemsID"].ToString()) && dtItemsList.Rows.Count > 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["transferQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthBottom = .5f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                }
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                // cell.BorderWidth = 0;
                pdtdtl.AddCell(cell);

                if (subChk == 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["TransRtnQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;

                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else if (!ItemsIdShip.Equals(dr["StkItemsID"].ToString()) && dtItemsList.Rows.Count > 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["TransRtnQty"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthBottom = .5f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                }
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                pdtdtl.AddCell(cell);

                if (subChk == 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["ClosingStock"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;

                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else if (!ItemsIdShip.Equals(dr["StkItemsID"].ToString()) && dtItemsList.Rows.Count > 0)
                {
                    cell = new PdfPCell(FormatPhrase(dr["ClosingStock"].ToString()));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                    cell.BorderWidthTop = .5f;
                    cell.BorderWidthBottom = 0f;
                    if (borderChk.Equals(23))
                    {
                        cell.BorderWidthBottom = .5f;
                    }
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidthBottom = .5f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthRight = .5f;
                    cell.BorderWidthLeft = .5f;
                }
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                pdtdtl.AddCell(cell);

                if (chk > 0 && !ItemsIdShip.Equals(dr["StkItemsID"].ToString()))
                {
                    itemsID = dr["ShipItemID"].ToString();
                    ItemsIdShip = dr["StkItemsID"].ToString();
                }
                chk++;
                borderChk++;
                subChk++;
                //if (subChk == 0)
                //{
                //    if (borderChk > 19)
                //    {

                //        borderChk = 0;  document.NewPage();
                //        subChk++;
                //    }

                //}
                //else
                //{
                //    if (borderChk > 23)
                //    {

                //        borderChk = 0;  document.NewPage();
                //    }
                //}
                if (!string.IsNullOrEmpty(dr["ShipQty"].ToString()))
                    ShipQty += Convert.ToDecimal(dr["ShipQty"].ToString());
                if (!string.IsNullOrEmpty(dr["SalesQty"].ToString()))
                    SalesQty += Convert.ToDecimal(dr["SalesQty"].ToString());
                if (!string.IsNullOrEmpty(dr["RtnQty"].ToString()))
                    RtnQty += Convert.ToDecimal(dr["RtnQty"].ToString());
                if (!string.IsNullOrEmpty(dr["transferQty"].ToString()))
                    transferQty += Convert.ToDecimal(dr["transferQty"].ToString());
                if (!string.IsNullOrEmpty(dr["TransRtnQty"].ToString()))
                    TransRtnQty += Convert.ToDecimal(dr["TransRtnQty"].ToString());
                if (!string.IsNullOrEmpty(dr["ClosingStock"].ToString()))
                    ClosingStock += Convert.ToDecimal(dr["ClosingStock"].ToString());
            }

            cell = new PdfPCell(FormatHeaderPhrase("Total : "));
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 7;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(ShipQty.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(SalesQty.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(RtnQty.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(transferQty.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(TransRtnQty.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase(ClosingStock.ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            document.Add(pdtdtl);
            cell = SignatureFormat(document, cell);
            document.Close();
            Response.Flush();
            Response.End();
        }
    }

    private void TransferItemCurrentDate(string DDT, string CurrentDate, string ReportType)
    {
        DataTable dt = null;
        if (ReportType.Equals("C"))
        {
            dt = _aclsItemTransferStockManager.GetShowTransferItemReport(CurrentDate, DateTime.Now.ToString("dd/MMM/yyyy"), DateTime.Now.ToString("dd/MMM/yyyy"),
                hfCustomerID.Value, rbReportType.SelectedValue);
        }
        else
        {
            dt = _aclsItemTransferStockManager.GetShowTransferItemReport(CurrentDate, txtStartDate.Text, txtEndDate.Text,
               hfCustomerID.Value, rbReportType.SelectedValue);
        }

        if (dt.Rows.Count <= 0) { return; }
        if (rbPrintType.SelectedValue.Equals("P"))
        {
            TransferReport(DDT, dt);
        }
        else
        {

            if (dt.Rows.Count > 0)
            {

                dt.Columns.Remove("ID");
                //ddt.Columns.Remove("InvoiceNo");
                dt.Columns.Remove("BranchID");
                dt.Columns.Remove("Remark");
                dt.Columns.Remove("Catagory");
                dt.Columns.Remove("SubCat");
                //  dt.Columns.Remove("Catagory");
                dt.Columns["Ship_Local"].ColumnName = "Ship/Local";
                dt.Columns["Items"].ColumnName = "Items Name";
                dt.Columns["TransferDate"].ColumnName = "Transfer Date";
                //dt.Columns["Name"].ColumnName = "Items Name";
                dt.Columns["BrandName"].ColumnName = "Brand";
                dt.Columns["UnitPrice"].ColumnName = "Price";
                dt.Columns["Qty"].ColumnName = "Quantity";
                dt.Columns["Total"].ColumnName = "Total";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "TransferSummery");
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition",
                        "attachment;filename=" + DDT.Replace(" ", "").Replace("  ", "") + "-TransferStatemrnt-(" +
                        DateTime.Now.ToString("dd/MM/yyyy") + ").xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
        }
    }

    private void TransferReport(string DDT, DataTable dt)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename='ItemsPurchase(" + DateTime.Now.ToString("dd-MMM-yyyy") + ")'.pdf");
        Document document = new Document(PageSize.LEGAL.Rotate(), 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(8f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;


        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);

        cell = new PdfPCell(new Phrase(DDT, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] widthdtl = new float[9] { 8, 15, 15, 15, 25, 15, 15, 15, 15 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;
        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Ship/Local"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Transfer Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("GRN"));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Items Code"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Items Name"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Brand"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("UOM"));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        decimal totQty = 0;
        decimal tot = 0;
        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell = new PdfPCell(FormatPhrase(dr["Ship_Local"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell =
                new PdfPCell(
                    FormatPhrase((DataManager.DateEncode(dr["TransferDate"].ToString())).ToString(IdManager.DateFormat())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Items"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["UnitPrice"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Qty"]).ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Total"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            totQty += Convert.ToDecimal(dr["Qty"]);
            tot += Convert.ToDecimal(dr["Total"]);
        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 7;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totQty.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N2")));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        //cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 9;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }

    private void TotalItemsStockDateWise(string p, string Date)
    {
        DataTable dt = PurchaseVoucherManager.getShowTotalItemsStockByDate(txtStartDate.Text, txtEndDate.Text, Date);
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename='Stock-Items'.pdf");
        Document document = new Document(PageSize.LEGAL.Rotate(), 50f, 50f, 40f, 40f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(8f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);

        cell = new PdfPCell(new Phrase(p, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        float[] widthdtl = new float[8] { 10, 30, 20, 20, 20, 20, 20, 20 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 15f;
        cell.Border = 0;
        cell.Colspan = 8;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Items Name & Code"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Opening Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Purchase Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Pur. Return Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Sales Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Sales Return"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Closing Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        int Serial = 1;
        double OpeningQty = 0; double PurchaseQty = 0; double PurReturnQty = 0; double SalesQty = 0; double SalesReturnQty = 0;
        decimal ClosingAmount = 0;
        //DataTable dtdtl = (DataTable)ViewState["STK"];
        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;
            cell = new PdfPCell(FormatPhrase(dr["Code"].ToString() + " - " + dr["Name"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["opening"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Purchase_qty"]).ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Return_qty"]).ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Order_qty"]).ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["order_Return"]).ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["Closing_Stock"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            OpeningQty += Convert.ToDouble(dr["opening"]);
            ClosingAmount += Convert.ToDecimal(dr["Closing_Stock"]);
            PurchaseQty += Convert.ToDouble(dr["Purchase_qty"]);
            PurReturnQty += Convert.ToDouble(dr["Return_qty"]);
            SalesQty += Convert.ToDouble(dr["Order_qty"]);
            SalesReturnQty += Convert.ToDouble(dr["order_Return"]);
        }
        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 2;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(OpeningQty.ToString("N0")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(PurchaseQty.ToString("N0")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(PurReturnQty.ToString("N0")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(SalesQty.ToString("N0")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(SalesReturnQty.ToString("N0")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(ClosingAmount.ToString("N2")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);
        document.Close();
        Response.Flush();
        Response.End();
    }

    private void Total_Stock(string Type, string Cat, string SubCat, string p)
    {
        DataTable dtdtl = PurchaseVoucherManager.GetShowStock(Type, Cat, SubCat);
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename='Stock-Items'.pdf");
        Document document = new Document(PageSize.LEGAL.Rotate(), 50f, 50f, 40f, 40f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(8f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);

        cell = new PdfPCell(new Phrase(p, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        float[] widthdtl = new float[7] { 10, 30, 15, 20, 20, 10, 15 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.HeaderRows = 2;
        pdtdtl.WidthPercentage = 100;

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 15f;
        cell.Border = 0;
        cell.Colspan = 7;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Items Name & Code"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Brand"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Category"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Sub Category"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("UOM"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("Unit Price"));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtdtl.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("OP. Stock"));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtdtl.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("OP. Amount"));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Closing Stock (Pch)"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase("Closing Amount"));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtdtl.AddCell(cell);

        int Serial = 1;
        double totOPStk = 0; decimal totOpAmt = 0; double totCloseStk = 0; decimal totCloseAmt = 0;
        //DataTable dtdtl = (DataTable)ViewState["STK"];
        foreach (DataRow dr in dtdtl.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;
            cell = new PdfPCell(FormatPhrase(dr["Items"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Catagory"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["SubCat"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["UMO"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            //cell = new PdfPCell(FormatPhrase(dr["UnitPrice"].ToString()));
            //cell.HorizontalAlignment = 2;
            //cell.VerticalAlignment = 1;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            //pdtdtl.AddCell(cell);

            //cell = new PdfPCell(FormatPhrase(dr["OpeningStock"].ToString()));
            //cell.HorizontalAlignment = 2;
            //cell.VerticalAlignment = 1;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            //pdtdtl.AddCell(cell);

            //cell = new PdfPCell(FormatPhrase(dr["OpeningAmount"].ToString()));
            //cell.HorizontalAlignment = 2;
            //cell.VerticalAlignment = 1;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            //pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["ClosingStock"]).ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            //cell =
            //    new PdfPCell(
            //        FormatPhrase(
            //            (Convert.ToDouble(dr["UnitPrice"].ToString())*Convert.ToDouble(dr["ClosingStock"].ToString()))
            //                .ToString("N2")));
            //cell.HorizontalAlignment = 2;
            //cell.VerticalAlignment = 1;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            //pdtdtl.AddCell(cell);

            totOPStk += Convert.ToDouble(dr["OpeningStock"]);
            totOpAmt += Convert.ToDecimal(dr["OpeningAmount"]);
            totCloseStk += Convert.ToDouble(dr["ClosingStock"]);
            totCloseAmt += Convert.ToDecimal(dr["ClosingAmount"]);
        }
        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 6;
        pdtdtl.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase(totOPStk.ToString("N3")));
        //cell.HorizontalAlignment = 2;
        //cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtdtl.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase(totOpAmt.ToString("N3")));
        //cell.HorizontalAlignment = 2;
        //cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(totCloseStk.ToString("N3")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        //cell = new PdfPCell(FormatHeaderPhrase(totCloseAmt.ToString("N3")));
        //cell.HorizontalAlignment = 2;
        //cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtdtl.AddCell(cell);

        document.Add(pdtdtl);
        document.Close();
        Response.Flush();
        Response.End();
    }

    //private void PurchaseDetailsSupplier(string p,string Date)
    //{
    //    DataTable dt = PurchaseVoucherManager.GetShowPurchaeReport(Date, txtStartDate.Text, txtEndDate.Text,
    //        ddlSupplier.SelectedValue, rbReportType.SelectedValue);
    //    if (dt.Rows.Count <= 0) { return; }
    //    Response.Clear();
    //    Response.ContentType = "application/pdf";
    //    Response.AddHeader("content-disposition", "attachment; filename='ItemsPurchase'.pdf");
    //    Document document = new Document(PageSize.LEGAL.Rotate(), 50f, 50f, 40f, 40f);
    //    PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
    //    document.Open();
    //    Rectangle page = document.PageSize;
    //    PdfPTable head = new PdfPTable(1);
    //    head.TotalWidth = page.Width - 50;
    //    Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
    //    PdfPCell c = new PdfPCell(phrase);
    //    c.Border = Rectangle.NO_BORDER;
    //    c.VerticalAlignment = Element.ALIGN_BOTTOM;
    //    c.HorizontalAlignment = Element.ALIGN_RIGHT;
    //    head.AddCell(c);
    //    head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

    //    PdfPCell cell;
    //    byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
    //    iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
    //    gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
    //    gif.ScalePercent(8f);

    //    float[] titwidth = new float[2] { 10, 200 };
    //    PdfPTable dth = new PdfPTable(titwidth);
    //    dth.WidthPercentage = 100;

    //    cell = new PdfPCell(gif);
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.Rowspan = 4;
    //    cell.BorderWidth = 0f;
    //    dth.AddCell(cell);
    //    cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.Colspan = 7;
    //    cell.BorderWidth = 0f;
    //    cell.FixedHeight = 20f;
    //    dth.AddCell(cell);
    //    cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.Colspan = 7;
    //    cell.BorderWidth = 0f;
    //    cell.FixedHeight = 20f;
    //    dth.AddCell(cell);
    //    cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.Colspan = 7;
    //    cell.BorderWidth = 0f;
    //    cell.FixedHeight = 20f;
    //    dth.AddCell(cell);
    //    if (rbReportType.Equals("1"))
    //    {
    //        cell =
    //            new PdfPCell(new Phrase("Items Purchase " + p,
    //                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
    //    }
    //    else
    //    {
    //        cell =
    //           new PdfPCell(new Phrase("Local Purchase " + p,
    //               FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
    //    }
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.Colspan = 7;
    //    cell.BorderWidth = 0f;
    //    cell.FixedHeight = 30f;
    //    dth.AddCell(cell);
    //    document.Add(dth);
    //    LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
    //    document.Add(line);

    //    PdfPTable dtempty = new PdfPTable(1);
    //    cell = new PdfPCell(FormatHeaderPhrase(""));
    //    cell.BorderWidth = 0f;
    //    cell.FixedHeight = 20f;
    //    dtempty.AddCell(cell);
    //    document.Add(dtempty);

    //    float[] widthdtl = new float[10] { 8,  15, 20, 10, 25, 15, 10, 15, 15, 15 };
    //    PdfPTable pdtdtl = new PdfPTable(widthdtl);
    //    pdtdtl.WidthPercentage = 100;
    //    int Serial = 1;
    //    cell = new PdfPCell(FormatHeaderPhrase("Serial"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);       
    //    cell = new PdfPCell(FormatHeaderPhrase("Received Date"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);
    //    cell = new PdfPCell(FormatHeaderPhrase("GRN"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);
    //    cell = new PdfPCell(FormatHeaderPhrase("Items Code"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);
    //    cell = new PdfPCell(FormatHeaderPhrase("Items Name"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);
    //    cell = new PdfPCell(FormatHeaderPhrase("Brand"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);
    //    cell = new PdfPCell(FormatHeaderPhrase("UOM"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);
    //    cell = new PdfPCell(FormatHeaderPhrase("Price"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);
    //    cell = new PdfPCell(FormatHeaderPhrase("Quantity"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);
    //    cell = new PdfPCell(FormatHeaderPhrase("Total"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);

    //    decimal totQty = 0;
    //    decimal tot = 0;
    //    foreach (DataRow dr in dt.Rows)
    //    {
    //        cell = new PdfPCell(FormatPhrase(Serial.ToString()));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);
    //        Serial++;            

    //        cell = new PdfPCell(FormatPhrase((DataManager.DateEncode(dr["ReceivedDate"].ToString())).ToString(IdManager.DateFormat())));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);

    //        cell = new PdfPCell(FormatPhrase(dr["GRN"].ToString()));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);

    //        cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);

    //        cell = new PdfPCell(FormatPhrase(dr["Name"].ToString()));
    //        cell.HorizontalAlignment = 0;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);

    //        cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);

    //        cell = new PdfPCell(FormatPhrase(dr["UOM"].ToString()));
    //        cell.HorizontalAlignment = 2;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);

    //        cell = new PdfPCell(FormatPhrase(dr["PurchasePrice"].ToString()));
    //        cell.HorizontalAlignment = 2;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);

    //        cell = new PdfPCell(FormatPhrase(dr["Quantity"].ToString()));
    //        cell.HorizontalAlignment = 2;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);

    //        cell = new PdfPCell(FormatPhrase(dr["Total"].ToString()));
    //        cell.HorizontalAlignment = 2;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);

    //        totQty += Convert.ToDecimal(dr["Quantity"]);
    //        tot += Convert.ToDecimal(dr["Total"]);
    //    }

    //    cell = new PdfPCell(FormatHeaderPhrase("Total"));
    //    // cell.FixedHeight = 20f;
    //    cell.HorizontalAlignment = 2;
    //    cell.VerticalAlignment = 1;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    cell.Colspan = 9;
    //    pdtdtl.AddCell(cell);

    //    cell = new PdfPCell(FormatHeaderPhrase(totQty.ToString("N0")));
    //    // cell.BorderWidth = 0f;
    //    // cell.FixedHeight = 20f;
    //    cell.HorizontalAlignment = 2;
    //    cell.VerticalAlignment = 1;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);

    //    cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N2")));
    //    //cell.BorderWidth = 0f;
    //    //cell.FixedHeight = 20f;
    //    cell.HorizontalAlignment = 2;
    //    cell.VerticalAlignment = 1;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);

    //    cell = new PdfPCell(FormatPhrase(""));
    //    //cell.BorderWidth = 0f;
    //    cell.FixedHeight = 10f;
    //    cell.HorizontalAlignment = 0;
    //    cell.VerticalAlignment = 1;
    //    cell.Border = 0;
    //    cell.Colspan = 11;
    //    pdtdtl.AddCell(cell);

    //    document.Add(pdtdtl);

    //    cell = SignatureFormat(document, cell);

    //    document.Close();
    //    Response.Flush();
    //    Response.End();
    //}
    private void PurchaseDetailsCurrentDate(string p, string CurrentDate, string ReportType)
    {
        DataTable dt = null;
        if (ReportType.Equals("C"))
        {
            dt = PurchaseVoucherManager.GetShowBranchPurchaeReport(CurrentDate, DateTime.Now.ToString("dd/MMM/yyyy"), DateTime.Now.ToString("dd/MMM/yyyy"),
                ddlSupplier.SelectedValue, rbReportType.SelectedValue,Session["BranchId"].ToString());
        }
        else
        {
            dt = PurchaseVoucherManager.GetShowBranchPurchaeReport(CurrentDate, txtStartDate.Text, txtEndDate.Text,
               ddlSupplier.SelectedValue, rbReportType.SelectedValue, Session["BranchId"].ToString());
        }

        if (dt.Rows.Count <= 0) { return; }
        if (rbPrintType.SelectedValue.Equals("P"))
        {
            PurchaseReport(p, dt);
        }
        else
        {

            if (dt.Rows.Count > 0)
            {

                dt.Columns.Remove("ID");
                //ddt.Columns.Remove("InvoiceNo");
                dt.Columns.Remove("ItemSize");
                dt.Columns.Remove("ItemColor");
                dt.Columns.Remove("Sup_ID");
                dt.Columns["ContactName"].ColumnName = "Supplier";
                dt.Columns["ReceivedDate"].ColumnName = "Received Date";
                dt.Columns["Code"].ColumnName = "Items Code";
                dt.Columns["Name"].ColumnName = "Items Name";
                dt.Columns["BrandName"].ColumnName = "Brand";
                dt.Columns["PurchasePrice"].ColumnName = "Price";
                dt.Columns["Quantity"].ColumnName = "Quantity";
                dt.Columns["Total"].ColumnName = "Total";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "PurchaseSummery");
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition",
                        "attachment;filename=" + p.Replace(" ", "").Replace("  ", "") + "-Purchasetatement-(" +
                        DateTime.Now.ToString("dd/MM/yyyy") + ").xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
        }
    }

    private void PurchaseReport(string p, DataTable dt)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename='ItemsPurchase(" + DateTime.Now.ToString("dd-MMM-yyyy") + ")'.pdf");
        Document document = new Document(PageSize.LEGAL.Rotate(), 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(5f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;


        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        if (rbReportType.SelectedValue.Equals("1"))
        {
            cell =
                new PdfPCell(new Phrase("BD Purchase " + p,
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        }
        else
        {
            cell =
                new PdfPCell(new Phrase("Local Purchase " + p,
                    FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        }
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] widthdtl = new float[11] { 8, 15, 15, 20, 10, 25, 15, 10, 15, 15, 15 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;
        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Supplier"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Received Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("GRN"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Items Code"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Items Name"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Brand"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("UOM"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        decimal totQty = 0;
        decimal tot = 0;
        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell = new PdfPCell(FormatPhrase(dr["ContactName"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell =
                new PdfPCell(
                    FormatPhrase((DataManager.DateEncode(dr["ReceivedDate"].ToString())).ToString(IdManager.DateFormat())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["GRN"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Name"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["UOM"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["PurchasePrice"]).ToString("N2")));
               
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Quantity"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Total"]).ToString("N2")));
               
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            totQty += Convert.ToDecimal(dr["Quantity"]);
            tot += Convert.ToDecimal(dr["Total"]);
        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 9;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totQty.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N2")));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        //cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 11;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }

    private void DailyExpenseReport(string p, DataTable dt)
    {
        if (dt.Rows.Count <= 0) { return; }
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename='ItemsPurchase'.pdf");
        Document document = new Document(PageSize.A4.Rotate(), 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(11f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase("Daily Expenses Report ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell); cell = new PdfPCell(new Phrase(p, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] widthdtl = new float[6] { 5, 14, 10, 25, 10, 25 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        int Serial = 1;

        cell = new PdfPCell(FormatHeaderPhrase("SL."));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Code"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Expenses Head"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Remarks"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        decimal totQty = 0;
        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["ExpDate"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["ExpName"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Amount"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Remarks"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            totQty += Convert.ToDecimal(dr["Amount"]);
        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totQty.ToString("N2")));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        //cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        //cell.Colspan = 8;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }

    private void DmageDetailsCurrentDate(string Heading, string Date, string Type)
    {
        DataTable dt = null;
        if (Type.Equals("C"))
        {
            dt = PurchaseVoucherManager.GetShowDamageReport("", "", "",
                DateTime.Now.ToString("dd/MMM/yyyy"), DateTime.Now.ToString("dd/MMM/yyyy"),
                hfCustomerID.Value);
        }
        else
        {
            dt = PurchaseVoucherManager.GetShowDamageReport("", "", "", txtStartDate.Text,
                txtEndDate.Text,
                hfCustomerID.Value);
        }


        if (dt.Rows.Count <= 0) { return; }
        if (rbPrintType.SelectedValue.Equals("P"))
        {
            Dmage_ShortItems(Heading, dt);
        }
        else
        {

            //if (dt.Rows.Count > 0)
            //{

            //    dt.Columns.Remove("ID");
            //    //ddt.Columns.Remove("InvoiceNo");
            //    dt.Columns.Remove("DiscountAmount");
            //    dt.Columns.Remove("TaxAmount");
            //    dt.Columns.Remove("SubTotal");
            //    dt.Columns.Remove("CustomerID");
            //    dt.Columns.Remove("ItemSize");
            //    dt.Columns.Remove("ItemColor");
            //    dt.Columns.Remove("UOM");
            //    dt.Columns.Remove("Cus_ID");
            //    dt.Columns.Remove("GrandTotal");
            //    dt.Columns["ShiftmentNO"].ColumnName = "Ship/Local";
            //    dt.Columns["OrderDate"].ColumnName = "Sales Date";
            //    dt.Columns["ContactName"].ColumnName = "Customer Name";
            //    dt.Columns["BrandName"].ColumnName = "Brand";
            //    dt.Columns["Name"].ColumnName = "Items Name";
            //    dt.Columns["Code"].ColumnName = "Items Code";
            //    dt.Columns["SalesPrice"].ColumnName = "Price";
            //    dt.Columns["TotalPrice"].ColumnName = "Total";
            //    using (XLWorkbook wb = new XLWorkbook())
            //    {
            //        wb.Worksheets.Add(dt, "SalesSummery");
            //        Response.Clear();
            //        Response.Buffer = true;
            //        Response.Charset = "";
            //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //        Response.AddHeader("content-disposition",
            //            "attachment;filename=" + Heading.Replace(" ", "").Replace("  ", "") + "-SalesStatement-(" +
            //            DateTime.Now.ToString("dd/MM/yyyy") + ").xlsx");

            //        using (MemoryStream MyMemoryStream = new MemoryStream())
            //        {
            //            wb.SaveAs(MyMemoryStream);
            //            MyMemoryStream.WriteTo(Response.OutputStream);
            //            Response.Flush();
            //            Response.End();
            //        }
            //    }
            //}
        }
    }

    private void Dmage_ShortItems(string Heading, DataTable dt)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename='SalesOrInvoice-(" + DateTime.Now.ToString("dd-MMM-yyyy") + ")'.pdf");
        Document document = new Document(PageSize.A4.Rotate(), 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(8f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        string Head = "";
        cell =
            new PdfPCell(new Phrase(Heading,
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);
        float[] widthdtl = null;
        widthdtl = new float[10] { 8, 10, 13, 13, 10, 10, 20, 10, 10, 10 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        //if (rbReportType.SelectedValue.Equals("2"))
        //{
        cell = new PdfPCell(FormatHeaderPhrase("Type"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);//}
        cell = new PdfPCell(FormatHeaderPhrase("Ship/Local"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);//}

        cell = new PdfPCell(FormatHeaderPhrase("Code"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);//}


        cell = new PdfPCell(FormatHeaderPhrase("Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase("Items Code"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Items Name"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Brand"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("UOM"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        double totQty = 0;
        double tot = 0;
        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell = new PdfPCell(FormatPhrase(dr["Type1"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["ShiftmentNO"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell =
                new PdfPCell(
                    FormatPhrase((DataManager.DateEncode(dr["OutDate"].ToString())).ToString(IdManager.DateFormat())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["item_code"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["item_desc"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["UMO"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["OutQty"]).ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            totQty += Convert.ToDouble(dr["OutQty"]);

        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 9;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totQty.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);



        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }

    private void SalesDetailsCurrentDate(string Heading, string Date, string Type)
    {
        DataTable dt = null;
        decimal ReturnAmount = 0;
        if (Type.Equals("C"))
        {
            //if clint See Item Name Wise Group By  Total Sales Report
            if (rbItemWise.SelectedValue == "Item")
            {
                dt = PurchaseVoucherManager.GetShowBranchSalesReportReport("3",
                    DateTime.Now.ToString("dd/MMM/yyyy"), DateTime.Now.ToString("dd/MMM/yyyy"),
                    hfCustomerID.Value, Session["BranchId"].ToString());
                ReturnAmount = PurchaseVoucherManager.GetBranchShowreturnAmount(DateTime.Now.ToString("dd/MMM/yyyy"), DateTime.Now.ToString("dd/MMM/yyyy"),
                    hfCustomerID.Value,Session["BranchId"].ToString());
            }
            //else clint See Invoice No  Total Sales Report
            else
            {
                dt = PurchaseVoucherManager.GetShowBranchSalesReportReport(rbReportType.SelectedValue,
                    DateTime.Now.ToString("dd/MMM/yyyy"), DateTime.Now.ToString("dd/MMM/yyyy"),
                    hfCustomerID.Value, Session["BranchId"].ToString());
                ReturnAmount = PurchaseVoucherManager.GetBranchShowreturnAmount(DateTime.Now.ToString("dd/MMM/yyyy"), DateTime.Now.ToString("dd/MMM/yyyy"),
                    hfCustomerID.Value,Session["BranchId"].ToString());
            }

        }
        else
        {





            //if clint See Item Name Wise Group By  Total Sales Report
            if (rbItemWise.SelectedValue == "Item")
            {
                dt = PurchaseVoucherManager.GetShowBranchSalesReportReport("3", txtStartDate.Text,
                    txtEndDate.Text, hfCustomerID.Value, Session["BranchId"].ToString());
                ReturnAmount = PurchaseVoucherManager.GetBranchShowreturnAmount(txtStartDate.Text,
                    txtEndDate.Text, hfCustomerID.Value, Session["BranchId"].ToString());
            }
            //else clint See Invoice No  Total Sales Report
            else
            {
                dt = PurchaseVoucherManager.GetShowBranchSalesReportReport(rbReportType.SelectedValue, txtStartDate.Text,
                    txtEndDate.Text, hfCustomerID.Value, Session["BranchId"].ToString());
                ReturnAmount = PurchaseVoucherManager.GetBranchShowreturnAmount(txtStartDate.Text,
                    txtEndDate.Text, hfCustomerID.Value, Session["BranchId"].ToString());
            }





        }


        if (dt.Rows.Count <= 0) { return; }
        if (rbPrintType.SelectedValue.Equals("P"))
        {
            SalesReport(Heading, dt, ReturnAmount);
        }
        else
        {

            if (dt.Rows.Count > 0)
            {

                dt.Columns.Remove("ID");
                //ddt.Columns.Remove("InvoiceNo");
                dt.Columns.Remove("DiscountAmount");
                dt.Columns.Remove("TaxAmount");
                dt.Columns.Remove("SubTotal");
                dt.Columns.Remove("CustomerID");
                dt.Columns.Remove("ItemSize");
                dt.Columns.Remove("ItemColor");
                dt.Columns.Remove("UOM");
                dt.Columns.Remove("Cus_ID");
                dt.Columns.Remove("GrandTotal");
                // dt.Columns["ShiftmentNO"].ColumnName = "Ship/Local";
                dt.Columns["OrderDate"].ColumnName = "Sales Date";
                dt.Columns["ContactName"].ColumnName = "Customer Name";
                dt.Columns["BrandName"].ColumnName = "Brand";
                dt.Columns["Name"].ColumnName = "Items Name";
                dt.Columns["Code"].ColumnName = "Items Code";
                dt.Columns["SalesPrice"].ColumnName = "Price";
                dt.Columns["TotalPrice"].ColumnName = "Total";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "SalesSummery");
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition",
                        "attachment;filename=" + Heading.Replace(" ", "").Replace("  ", "") + "-SalesStatement-(" +
                        DateTime.Now.ToString("dd/MM/yyyy") + ").xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
        }
    }

    private void SalesSummeryCurrentDate(string Heading, string Date, string Type)
    {
        DataTable dt = null;
        if (Type.Equals("C"))
        {
            dt = PurchaseVoucherManager.GetBranchIdSalesSummeryInfo(DateTime.Now.ToString("dd/MMM/yyyy"), DateTime.Now.ToString("dd/MMM/yyyy"),Session["BranchId"].ToString());
        }
        else
        {
            dt = PurchaseVoucherManager.GetBranchIdSalesSummeryInfo(txtStartDateExpanse.Text, txtEndDateExpanse.Text,Session["BranchId"].ToString());
        }


        if (dt.Rows.Count <= 0) { return; }
        if (rbPrintTypeExpanse.SelectedValue.Equals("P"))
        {
            SalesSummeryReport(Heading, dt);
        }
        else
        {

            if (dt.Rows.Count > 0)
            {


                dt.Columns["OrderDate"].ColumnName = "Sales Date";
                dt.Columns["returnamn"].ColumnName = "Return Amount";
                dt.Columns["Quantity"].ColumnName = "Sales Quantity";
                dt.Columns["Return"].ColumnName = "Return Quantity";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "SalesSummery");
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition",
                        "attachment;filename=" + Heading.Replace(" ", "").Replace("  ", "") + "-SalesStatement-(" +
                        DateTime.Now.ToString("dd/MM/yyyy") + ").xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
        }
    }



    private void PoLSalesDetailsCurrentDate(string Heading, string Date, string Type)
    {
        DataTable dt = null;
        if (Type.Equals("C"))
        {
            dt = PurchaseVoucherManager.GetShowBranchSalesReportReport("2",
                DateTime.Now.ToString("dd/MMM/yyyy"), DateTime.Now.ToString("dd/MMM/yyyy"),
                hfCustomerID.Value, Session["BranchId"].ToString());
        }
        else
        {
            dt = PurchaseVoucherManager.GetShowBranchSalesReportReport("2", txtStartDate.Text,
                txtEndDate.Text,
                hfCustomerID.Value, Session["BranchId"].ToString());
        }


        if (dt.Rows.Count <= 0) { return; }

        PolSalesReport(Heading, dt);


    }




   private void SalesReport(string Heading, DataTable dt, decimal ReturnAmount)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename='SalesOrInvoice-(" + DateTime.Now.ToString("dd-MMM-yyyy") + ")'.pdf");
        Document document = new Document(PageSize.A4.Rotate(), 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(5f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        string Head = "";
        if (rbReportType.SelectedValue.Equals("2"))
        {
            Head = "PH";
        }
        else
        {
            Head = "BD";
        }
        cell =
            new PdfPCell(new Phrase(" Sales " + Heading,
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);
        float[] widthdtl = null;
        if (rbReportType.SelectedValue.Equals("2"))
        {
            widthdtl = new float[12] { 8, 13, 20, 14, 16, 14, 20, 10, 10, 10, 10, 10 };
        }

        else
        {
            widthdtl = new float[11] { 8, 20, 10, 16, 14, 20, 10, 10, 10, 10, 10 };
            if (rbItemWise.SelectedValue == "Item")
            {
                widthdtl = new float[9] { 8, 20, 14, 20, 10, 10, 10, 10, 10 };

            }
        }
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        if (rbReportType.SelectedValue.Equals("2"))
        {
            cell = new PdfPCell(FormatHeaderPhrase("Ship/Local"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
        }
       
        if (rbItemWise.SelectedValue != "Item")
        {
            cell = new PdfPCell(FormatHeaderPhrase("Customer Name"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            cell = new PdfPCell(FormatHeaderPhrase("Sales Date"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Invoice No"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


        }

        cell = new PdfPCell(FormatHeaderPhrase("Items Code"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Items Name"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Brand"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("UOM"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        double totQty = 0;
        double tot = 0;
        double Vat = 0;
        double Discount = 0;
        double ReceivedAmt = 0;
        string CustomerID = "0";
        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;
            if (rbReportType.SelectedValue.Equals("2"))
            {
                cell = new PdfPCell(FormatPhrase(dr["ShiftmentNO"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
            }
          

            if (rbItemWise.SelectedValue != "Item")
            {
                cell = new PdfPCell(FormatPhrase(dr["ContactName"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase((DataManager.DateEncode(dr["OrderDate"].ToString())).ToString(IdManager.DateFormat())));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["InvoiceNo"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
            }


            cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["Name"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["UOM"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["SalesPrice"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Quantity"]).ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["TotalPrice"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            totQty += Convert.ToDouble(dr["Quantity"]);
            tot += Convert.ToDouble(dr["TotalPrice"]);

            if (rbItemWise.SelectedValue == "Item")
            {
                CustomerID = CustomerID + "," + dr["Cus_ID"].ToString();
            }
            else
            {
                CustomerID = CustomerID + "," + dr["CustomerID"].ToString();
            }


        }

        //var cou = CustomerID;
        DataTable PayAmount = null;
        if (rbCurDate.Checked == true)
        {

            PayAmount = _aSalesManager.GetBranchSalesPaymentDetails(DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), CustomerID, Session["BranchId"].ToString());
        }
        else
        {
            PayAmount = _aSalesManager.GetBranchSalesPaymentDetails(txtStartDate.Text, txtEndDate.Text, CustomerID, Session["BranchId"].ToString());
        }

        if (rbItemWise.SelectedValue != "Item")
        {
            cell = new PdfPCell(FormatHeaderPhrase("Total"));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 9;
            if (rbReportType.SelectedValue.Equals("2"))
            {
                cell.Colspan = 10;
            }
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(totQty.ToString("N0")));
            // cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N2")));
            //cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            //************** Vat **************//
            cell = new PdfPCell(FormatHeaderPhrase("VAT (%)"));
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 9;
            if (rbReportType.SelectedValue.Equals("2"))
            {
                cell.Colspan = 10;
            }
            pdtdtl.AddCell(cell);

            Double Due = (tot + Convert.ToDouble(PayAmount.Rows[0]["TaxAmount"])) - Convert.ToDouble(ReturnAmount) - (Convert.ToDouble(PayAmount.Rows[0]["DiscountAmount"]) + Convert.ToDouble(PayAmount.Rows[0]["CashReceived"]) + Convert.ToDouble(PayAmount.Rows[0]["PayAmt"]));
            cell = new PdfPCell(FormatHeaderPhrase(Convert.ToDouble(PayAmount.Rows[0]["TaxAmount"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 2;
            pdtdtl.AddCell(cell);
            //************** Discount **************//
            cell = new PdfPCell(FormatHeaderPhrase("Discount(TK)"));
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 9;
            if (rbReportType.SelectedValue.Equals("2"))
            {
                cell.Colspan = 10;
            }
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(Convert.ToDouble(PayAmount.Rows[0]["DiscountAmount"]).ToString("N2")));
            cell.Colspan = 2;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            //************** Cash Received **************//
            cell = new PdfPCell(FormatHeaderPhrase("Total Received Amount"));
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 9;
            if (rbReportType.SelectedValue.Equals("2"))
            {
                cell.Colspan = 10;
            }
            pdtdtl.AddCell(cell);


            cell = new PdfPCell(FormatHeaderPhrase(Convert.ToDouble(PayAmount.Rows[0]["CashReceived"]).ToString("N2")));
            cell.Colspan = 2;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            ////CustomerPayment


            //cell = new PdfPCell(FormatHeaderPhrase("CustomerPayment Received Amount"));
            //// cell.FixedHeight = 20f;
            //cell.HorizontalAlignment = 2;
            //cell.VerticalAlignment = 1;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            //cell.Colspan = 9;
            //if (rbReportType.SelectedValue.Equals("2"))
            //{
            //    cell.Colspan = 10;
            //}
            //pdtdtl.AddCell(cell);


            //cell = new PdfPCell(FormatHeaderPhrase(Convert.ToDouble(PayAmount.Rows[0]["PayAmt"]).ToString("N2")));
            //cell.Colspan = 2;
            //cell.HorizontalAlignment = 2;
            //cell.VerticalAlignment = 1;
            //cell.BorderColor = BaseColor.LIGHT_GRAY;
            //pdtdtl.AddCell(cell);



            ////

            cell = new PdfPCell(FormatHeaderPhrase("Total Return Amount(-)"));
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 9;
            if (rbReportType.SelectedValue.Equals("2"))
            {
                cell.Colspan = 10;
            }
            pdtdtl.AddCell(cell);


            cell = new PdfPCell(FormatHeaderPhrase(ReturnAmount.ToString("N2")));
            cell.Colspan = 2;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            //************** Due **************//
            cell = new PdfPCell(FormatHeaderPhrase("Due(TK)"));
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 9;
            if (rbReportType.SelectedValue.Equals("2"))
            {
                cell.Colspan = 10;
            }
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(Due.ToString("N2")));
            cell.Colspan = 2;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            //cell.BorderWidth = 0f;
            cell.FixedHeight = 10f;
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            cell.Colspan = 11;
            pdtdtl.AddCell(cell);

        }

        else
        {


            cell = new PdfPCell(FormatHeaderPhrase("Total"));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 7;
            if (rbReportType.SelectedValue.Equals("2"))
            {
                cell.Colspan = 10;
            }
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(totQty.ToString("N0")));
            // cell.BorderWidth = 0f;
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N2")));
            //cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);








            //************** Vat **************//
            cell = new PdfPCell(FormatHeaderPhrase("VAT (%)"));
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 8;
            if (rbReportType.SelectedValue.Equals("2"))
            {
                cell.Colspan = 10;
            }
            pdtdtl.AddCell(cell);

            Double Due = (tot + Convert.ToDouble(PayAmount.Rows[0]["TaxAmount"])) - Convert.ToDouble(ReturnAmount) - (Convert.ToDouble(PayAmount.Rows[0]["DiscountAmount"]) + Convert.ToDouble(PayAmount.Rows[0]["CashReceived"]) + Convert.ToDouble(PayAmount.Rows[0]["PayAmt"]));
            cell = new PdfPCell(FormatHeaderPhrase(Convert.ToDouble(PayAmount.Rows[0]["TaxAmount"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 2;
            pdtdtl.AddCell(cell);






            //************** Discount **************//
            cell = new PdfPCell(FormatHeaderPhrase("Discount(TK)"));
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 8;
            if (rbReportType.SelectedValue.Equals("2"))
            {
                cell.Colspan = 10;
            }
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(Convert.ToDouble(PayAmount.Rows[0]["DiscountAmount"]).ToString("N2")));
            cell.Colspan = 2;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            //************** Cash Received **************//
            cell = new PdfPCell(FormatHeaderPhrase("Total Received Amount"));
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 8;
            if (rbReportType.SelectedValue.Equals("2"))
            {
                cell.Colspan = 10;
            }
            pdtdtl.AddCell(cell);


            cell = new PdfPCell(FormatHeaderPhrase(Convert.ToDouble(PayAmount.Rows[0]["CashReceived"]).ToString("N2")));
            cell.Colspan = 2;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Total Return Amount(-)"));
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 8;
            if (rbReportType.SelectedValue.Equals("2"))
            {
                cell.Colspan = 10;
            }
            pdtdtl.AddCell(cell);


            cell = new PdfPCell(FormatHeaderPhrase(ReturnAmount.ToString("N2")));
            cell.Colspan = 2;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            //************** Due **************//
            cell = new PdfPCell(FormatHeaderPhrase("Due(TK)"));
            // cell.FixedHeight = 20f;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.Colspan = 8;
            if (rbReportType.SelectedValue.Equals("2"))
            {
                cell.Colspan = 10;
            }
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase(Due.ToString("N2")));
            cell.Colspan = 2;
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            //cell.BorderWidth = 0f;
            cell.FixedHeight = 10f;
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            cell.Colspan = 11;
            pdtdtl.AddCell(cell);






        }

        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }

    

    private void SalesSummeryReport(string Heading, DataTable dt)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename='SalesOrInvoice-(" + DateTime.Now.ToString("dd-MMM-yyyy") + ")'.pdf");
        Document document = new Document(PageSize.A4.Rotate(), 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(5f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        string Head = "";

        cell =
            new PdfPCell(new Phrase(" Sales Summery",
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);

        cell =
          new PdfPCell(new Phrase(Heading,
              FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);
        float[] widthdtl = null;

        widthdtl = new float[6] { 8, 25, 22, 22, 22, 23 };

        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Sales Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Sales Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Ret. Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Sales Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Ret. Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        double totQty = 0;
        double tot = 0;

        double totQtyRet = 0;
        double totRet = 0;

        double Vat = 0;
        double Discount = 0;
        double ReceivedAmt = 0;

        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell =
                new PdfPCell(
                    FormatPhrase((dr["OrderDate"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["grandTotal"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["returnamn"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["Quantity"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["Return"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            totQty += Convert.ToDouble(dr["Quantity"]);
            tot += Convert.ToDouble(dr["grandTotal"]);
            totQtyRet += Convert.ToDouble(dr["Return"]);
            totRet += Convert.ToDouble(dr["returnamn"]);
        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 1;

        pdtdtl.AddCell(cell);



        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(totRet.ToString("N2")));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase(totQty.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);



        pdtdtl.AddCell(cell); cell = new PdfPCell(FormatHeaderPhrase(totQtyRet.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }

    private void CustomerPaymentReport(string Heading, DataTable dt)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename='SalesOrInvoice-(" + DateTime.Now.ToString("dd-MMM-yyyy") + ")'.pdf");
        Document document = new Document(PageSize.A4, 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(5f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        string Head = "";

        cell =
           new PdfPCell(new Phrase(" CUSTOMER PAYMENT DETAILS INFORMATION ",
               FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(" Date " + Heading,
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);
        float[] dtlw = new float[3] { 15, 70, 15 };

        PdfPTable pdt = new PdfPTable(dtlw);
        pdt.WidthPercentage = 100;
        pdt.HeaderRows = 1;


        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Customer Name :"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(dt.Rows[0]["CustomerName"].ToString()));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);


        document.Add(pdt);


        float[] widthdtl = null;

        widthdtl = new float[4] { 10, 45, 30, 30 };

        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 40f;
        cell.Border = 0;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);



        cell = new PdfPCell(FormatHeaderPhrase("Payment Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Ref."));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        double totQty = 0;
        double tot = 0;
        double Vat = 0;
        double Discount = 0;
        double ReceivedAmt = 0;

        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell =
                new PdfPCell(
                    FormatPhrase((dr["PayDate"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell =
               new PdfPCell(
                   FormatPhrase((dr["Ref"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);



            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["PayAmount"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            tot += Convert.ToDouble(dr["PayAmount"]);

        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 3;

        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }

    private void CustomerPaymentANdDueReport(string Heading, DataTable dt, DataTable dt2)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename='SalesOrInvoice-(" + DateTime.Now.ToString("dd-MMM-yyyy") + ")'.pdf");
        Document document = new Document(PageSize.A4, 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(5f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        string Head = "";

        cell =
          new PdfPCell(new Phrase(" CUSTOMER DUE AND PAYMENT DETAILS ",
              FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);

        cell =
            new PdfPCell(new Phrase(" Date " + Heading,
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);
        float[] dtlw = new float[3] { 15, 70, 15 };

        PdfPTable pdt = new PdfPTable(dtlw);
        pdt.WidthPercentage = 100;
        pdt.HeaderRows = 1;


        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Customer Name :"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(dt2.Rows[0]["CustomerName"].ToString()));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);


        document.Add(pdt);

        float[] widthdtl2 = null;

        widthdtl2 = new float[4] { 8, 30, 30, 30 };

        PdfPTable pdtdtl2 = new PdfPTable(widthdtl2);
        pdtdtl2.WidthPercentage = 100;
        pdtdtl2.HeaderRows = 1;
        int Serial1 = 1;

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 40f;
        cell.Border = 0;
        cell.Colspan = 4;
        pdtdtl2.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl2.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Invoce No"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl2.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Sales Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl2.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl2.AddCell(cell);


        double tot2 = 0;


        foreach (DataRow dr in dt2.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial1.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl2.AddCell(cell);
            Serial1++;
            cell =
                   new PdfPCell(
                       FormatPhrase((dr["InvoiceNo"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl2.AddCell(cell);
            cell =
                new PdfPCell(
                    FormatPhrase((dr["OrderDate"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl2.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["GrandTotal"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl2.AddCell(cell);


            tot2 += Convert.ToDouble(dr["GrandTotal"]);

        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 3;

        pdtdtl2.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot2.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl2.AddCell(cell);
        document.Add(pdtdtl2);

        float[] widthdtl = null;

        widthdtl = new float[4] { 8, 40, 30, 30 };

        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.Border = 0;
        cell.Colspan = 3;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Payment Details"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 3;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Payment Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Ref."));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        double totQty = 0;
        double tot = 0;
        double Vat = 0;
        double Discount = 0;
        double ReceivedAmt = 0;

        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell =
                new PdfPCell(
                    FormatPhrase((dr["PayDate"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Ref"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["PayAmount"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            tot += Convert.ToDouble(dr["PayAmount"]);

        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 3;

        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);



        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 50f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.Colspan = 3;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);


        float[] dtlw2 = new float[3] { 30, 30, 30 };

        PdfPTable pdt2 = new PdfPTable(dtlw2);
        pdt2.WidthPercentage = 100;


        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 15f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.Colspan = 3;
        pdt2.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Total Sales: " + tot2.ToString()));
        cell.BorderWidth = 0f;

        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;

        pdt2.AddCell(cell);




        cell = new PdfPCell(FormatHeaderPhrase("Payment: " + tot.ToString()));
        cell.BorderWidth = 0f;
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        pdt2.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase("Due : " + (tot2 - tot).ToString("N0")));
        cell.BorderWidth = 0f;
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        pdt2.AddCell(cell);

        document.Add(pdt2);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }

    private void SupplierPaymentANdDueReport(string Heading, DataTable dt, DataTable dt2)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename='SalesOrInvoice-(" + DateTime.Now.ToString("dd-MMM-yyyy") + ")'.pdf");
        Document document = new Document(PageSize.A4, 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(5f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        string Head = "";

        cell =
          new PdfPCell(new Phrase(" SUPPLIER DUE AND PAYMENT DETAILS ",
              FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);

        cell =
            new PdfPCell(new Phrase(" Date " + Heading,
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);
        float[] dtlw = new float[3] { 15, 70, 15 };

        PdfPTable pdt = new PdfPTable(dtlw);
        pdt.WidthPercentage = 100;
        pdt.HeaderRows = 1;


        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Supplier Name :"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(dt2.Rows[0]["SupplierName"].ToString()));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        cell.BorderWidth = 0f;
        pdt.AddCell(cell);


        document.Add(pdt);

        float[] widthdtl2 = null;

        widthdtl2 = new float[4] { 8, 30, 30, 30 };

        PdfPTable pdtdtl2 = new PdfPTable(widthdtl2);
        pdtdtl2.WidthPercentage = 100;
        pdtdtl2.HeaderRows = 1;
        int Serial1 = 1;

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 40f;
        cell.Border = 0;
        cell.Colspan = 4;
        pdtdtl2.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl2.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("GRN"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl2.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Challan Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl2.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl2.AddCell(cell);


        double tot2 = 0;


        foreach (DataRow dr in dt2.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial1.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl2.AddCell(cell);
            Serial1++;
            cell =
                   new PdfPCell(
                       FormatPhrase((dr["GRN"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl2.AddCell(cell);
            cell =
                new PdfPCell(
                    FormatPhrase((dr["ChallanDate"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl2.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Total"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl2.AddCell(cell);


            tot2 += Convert.ToDouble(dr["Total"]);

        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 3;

        pdtdtl2.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot2.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl2.AddCell(cell);
        document.Add(pdtdtl2);

        float[] widthdtl = null;

        widthdtl = new float[4] { 8, 40, 30, 30 };

        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.Border = 0;
        cell.Colspan = 3;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Payment Details"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 3;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Payment Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Ref."));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        double totQty = 0;
        double tot = 0;
        double Vat = 0;
        double Discount = 0;
        double ReceivedAmt = 0;

        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell =
                new PdfPCell(
                    FormatPhrase((dr["PayDate"].ToString())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Ref"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["PayAmount"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            tot += Convert.ToDouble(dr["PayAmount"]);

        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 3;

        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);



        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 50f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.Colspan = 3;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);


        float[] dtlw2 = new float[3] { 30, 30, 30 };

        PdfPTable pdt2 = new PdfPTable(dtlw2);
        pdt2.WidthPercentage = 100;


        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 15f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.Colspan = 3;
        pdt2.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Total Purchase: " + tot2.ToString()));
        cell.BorderWidth = 0f;

        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;

        pdt2.AddCell(cell);




        cell = new PdfPCell(FormatHeaderPhrase("Payment: " + tot.ToString()));
        cell.BorderWidth = 0f;
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        pdt2.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase("Due : " + (tot2 - tot).ToString("N0")));
        cell.BorderWidth = 0f;
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        pdt2.AddCell(cell);

        document.Add(pdt2);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }
    private void PolSalesReport(string Heading, DataTable dt)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition",
            "attachment; filename='SalesOrInvoice-(" + DateTime.Now.ToString("dd-MMM-yyyy") + ")'.pdf");
        Document document = new Document(PageSize.A4.Rotate(), 20f, 20f, 30f, 20f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(5f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["org"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add1"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

        dth.AddCell(cell);
        cell =
            new PdfPCell(new Phrase(Session["add2"].ToString(),
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        string Head = "";

        Head = "BD";

        cell =
            new PdfPCell(new Phrase(" Profit Or Loss " + Heading,
                FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dtempty.AddCell(cell);
        document.Add(dtempty);
        float[] widthdtl = null;
        widthdtl = new float[15] { 5, 15, 15, 15, 5, 5, 5, 5, 10, 10, 5, 15, 15, 12, 10 };



        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;
        pdtdtl.HeaderRows = 1;
        int Serial = 1;

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Customer Name"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Sales Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Invoice No"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Items Code"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Items Name"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Brand"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("UOM"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Sales Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Cost Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Qty"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Total Sales Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Total Cost Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);



        cell = new PdfPCell(FormatHeaderPhrase("Discount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);



        cell = new PdfPCell(FormatHeaderPhrase("Total Profit Or Loss"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        double totQty = 0;
        double tot = 0;
        double totCost = 0;
        double TotProfitOrLoss = 0;
        double totDisCount = 0;
        foreach (DataRow dr in dt.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            cell = new PdfPCell(FormatPhrase(dr["ContactName"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell =
                new PdfPCell(FormatPhrase((DataManager.DateEncode(dr["OrderDate"].ToString())).ToString(IdManager.DateFormat())));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["InvoiceNo"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["Code"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["Name"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dr["UOM"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["SalesPrice"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["PvUnitPrice"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["Quantity"]).ToString("N0")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["TotalPrice"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["TotalCostPrice"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);



            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["totDiscountAmount"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);



            cell = new PdfPCell(FormatPhrase(Convert.ToDouble(dr["TotalProfitAndLoss"]).ToString("N2")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);


            totQty += Convert.ToDouble(dr["Quantity"]);
            tot += Convert.ToDouble(dr["TotalPrice"]);
            totCost += Convert.ToDouble(dr["TotalCostPrice"]);
            TotProfitOrLoss += Convert.ToDouble(dr["TotalProfitAndLoss"]);
            totDisCount += Convert.ToDouble(dr["totDiscountAmount"]);
        }

        cell = new PdfPCell(FormatHeaderPhrase("Total"));
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 10;

        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(totQty.ToString("N0")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N2")));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);








        cell = new PdfPCell(FormatHeaderPhrase(totCost.ToString("N2")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase(totDisCount.ToString("N2")));
        // cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase(TotProfitOrLoss.ToString("N2")));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);











        cell = new PdfPCell(FormatPhrase(""));
        //cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 11;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }

    //private void SalesDetailsCustomer(string p, string Date)
    //{
    //    DataTable dt = PurchaseVoucherManager.GetShowSalesReportReport(Date, txtStartDate.Text, txtEndDate.Text, ddlSupplier.SelectedValue);
    //    if (dt.Rows.Count <= 0) { return; }
    //    Response.Clear();
    //    Response.ContentType = "application/pdf";
    //    Response.AddHeader("content-disposition", "attachment; filename='ItemsPurchase'.pdf");
    //    Document document = new Document(PageSize.A4.Rotate(), 50f, 50f, 40f, 40f);
    //    PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
    //    document.Open();
    //    Rectangle page = document.PageSize;
    //    PdfPTable head = new PdfPTable(1);
    //    head.TotalWidth = page.Width - 50;
    //    Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
    //    PdfPCell c = new PdfPCell(phrase);
    //    c.Border = Rectangle.NO_BORDER;
    //    c.VerticalAlignment = Element.ALIGN_BOTTOM;
    //    c.HorizontalAlignment = Element.ALIGN_RIGHT;
    //    head.AddCell(c);
    //    head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

    //    PdfPCell cell;
    //    byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
    //    iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
    //    gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
    //    gif.ScalePercent(8f);

    //    float[] titwidth = new float[2] { 10, 200 };
    //    PdfPTable dth = new PdfPTable(titwidth);
    //    dth.WidthPercentage = 100;

    //    cell = new PdfPCell(gif);
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.Rowspan = 4;
    //    cell.BorderWidth = 0f;
    //    dth.AddCell(cell);

    //    cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.Colspan = 7;
    //    cell.BorderWidth = 0f;

    //    dth.AddCell(cell);
    //    cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.Colspan = 7;
    //    cell.BorderWidth = 0f;

    //    dth.AddCell(cell);
    //    cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.Colspan = 7;
    //    cell.BorderWidth = 0f;

    //    dth.AddCell(cell);
    //    cell = new PdfPCell(new Phrase("Sales " + p, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.Colspan = 7;
    //    cell.BorderWidth = 0f;
    //    cell.FixedHeight = 30f;
    //    dth.AddCell(cell);
    //    document.Add(dth);
    //    LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
    //    document.Add(line);

    //    PdfPTable dtempty = new PdfPTable(1);
    //    cell = new PdfPCell(FormatHeaderPhrase(""));
    //    cell.BorderWidth = 0f;
    //    cell.FixedHeight = 20f;
    //    dtempty.AddCell(cell);
    //    document.Add(dtempty);

    //    float[] widthdtl = new float[7] { 8, 15, 20, 10, 15, 10, 15 };
    //    PdfPTable pdtdtl = new PdfPTable(widthdtl);
    //    pdtdtl.WidthPercentage = 100;
    //    int Serial = 1;

    //    cell = new PdfPCell(FormatHeaderPhrase("Serial"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);

    //    cell = new PdfPCell(FormatHeaderPhrase("Order Date"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);
    //    cell = new PdfPCell(FormatHeaderPhrase("Invoice No"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);        
    //    cell = new PdfPCell(FormatHeaderPhrase("Sub Total"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);
    //    cell = new PdfPCell(FormatHeaderPhrase("VAT"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);
    //    cell = new PdfPCell(FormatHeaderPhrase("Discount"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);
    //    cell = new PdfPCell(FormatHeaderPhrase("Total"));
    //    cell.HorizontalAlignment = 1;
    //    cell.VerticalAlignment = 1;
    //    cell.FixedHeight = 20f;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);

    //    decimal totQty = 0;
    //    decimal tot = 0;
    //    foreach (DataRow dr in dt.Rows)
    //    {
    //        cell = new PdfPCell(FormatPhrase(Serial.ToString()));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);
    //        Serial++;

    //        cell = new PdfPCell(FormatPhrase((DataManager.DateEncode(dr["OrderDate"].ToString())).ToString(IdManager.DateFormat())));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);

    //        cell = new PdfPCell(FormatPhrase(dr["InvoiceNo"].ToString()));
    //        cell.HorizontalAlignment = 1;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);           

    //        cell = new PdfPCell(FormatPhrase(dr["SubTotal"].ToString()));
    //        cell.HorizontalAlignment = 2;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);

    //        cell = new PdfPCell(FormatPhrase(dr["TaxAmount"].ToString()));
    //        cell.HorizontalAlignment = 2;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);

    //        cell = new PdfPCell(FormatPhrase(dr["DiscountAmount"].ToString()));
    //        cell.HorizontalAlignment = 2;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);

    //        cell = new PdfPCell(FormatPhrase(dr["GrandTotal"].ToString()));
    //        cell.HorizontalAlignment = 2;
    //        cell.VerticalAlignment = 1;
    //        cell.BorderColor = BaseColor.LIGHT_GRAY;
    //        pdtdtl.AddCell(cell);


    //        tot += Convert.ToDecimal(dr["GrandTotal"]);
    //    }

    //    cell = new PdfPCell(FormatHeaderPhrase("Total"));
    //    // cell.FixedHeight = 20f;
    //    cell.HorizontalAlignment = 2;
    //    cell.VerticalAlignment = 1;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    cell.Colspan = 6;
    //    pdtdtl.AddCell(cell);

    //    cell = new PdfPCell(FormatHeaderPhrase(tot.ToString("N3")));
    //    cell.HorizontalAlignment = 2;
    //    cell.VerticalAlignment = 1;
    //    cell.BorderColor = BaseColor.LIGHT_GRAY;
    //    pdtdtl.AddCell(cell);

    //    cell = new PdfPCell(FormatPhrase(""));
    //    //cell.BorderWidth = 0f;
    //    cell.FixedHeight = 10f;
    //    cell.HorizontalAlignment = 0;
    //    cell.VerticalAlignment = 1;
    //    cell.Border = 0;
    //    cell.Colspan = 6;
    //    pdtdtl.AddCell(cell);

    //    document.Add(pdtdtl);

    //    cell = SignatureFormat(document, cell);

    //    document.Close();
    //    Response.Flush();
    //    Response.End();
    //}

    private static PdfPCell SignatureFormat(Document document, PdfPCell cell)
    {
        float[] widtl = new float[5] { 20, 20, 20, 20, 20 };
        PdfPTable pdtsig = new PdfPTable(widtl);
        pdtsig.WidthPercentage = 100;
        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 5;
        cell.FixedHeight = 40f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Received by"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Checked by"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Authorised by"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);
        document.Add(pdtsig);
        return cell;
    }

    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.BOLD));
    }

    protected void txtCustomer_TextChanged(object sender, EventArgs e)
    {
        DataTable dtCustomer = _aclsClientInfoManager.GetCustomerOnSearch("where UPPER(SearchName) = UPPER('" + txtCustomer.Text + "')", 0);
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
            txtCustomer.Focus();
            return;
        }
    }
    protected void txtSupplierSearch_TextChanged(object sender, EventArgs e)
    {
        DataTable dtSupplier = PurchaseVoucherManager.GetSupplierInfo(txtSupplierSearch.Text);
        if (dtSupplier.Rows.Count > 0)
        {
            hfCustomerID.Value = dtSupplier.Rows[0]["ID"].ToString();
            txtCustomer.Text = dtSupplier.Rows[0]["ContactName"].ToString();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Not Found Supplier.!!');", true);
            hfCustomerID.Value = "";
            txtCustomer.Text = "";
            txtSupplierSearch.Focus();
        }
    }

    protected void btnPreview_Click(object sender, EventArgs e)
    {
        string embed = "<object data=\"{0}{1}\" type=\"application/pdf\" width=\"1000px\" height=\"550px\">";
        embed += "If you are unable to view file, you can download from <a href = \"{0}{1}&download=1\">here</a>";
        embed += " or download <a target = \"_blank\" href = \"http://get.adobe.com/reader/\">Adobe PDF Reader</a> to view the file.";
        embed += "</object>";


        Session["SupplierID"] = hfCustomerID.Value;
        Session["rbReportType"] = rbReportType.SelectedValue;
        Session["ReportType"] = RadioButtonList1.SelectedValue;
        if (RadioButtonList1.SelectedValue == "Purchase")
        {
            if (rbCurDate.Checked)
            {
                string DDT = "";
                if (!string.IsNullOrEmpty(txtSupplierSearch.Text))
                {
                    DDT = "Report of (" + txtSupplierSearch.Text + ") From  (" + DateTime.Now.ToString("dd/MM/yyyy") +
                          ")";
                }
                else
                {
                    DDT = "Report of (" + DateTime.Now.ToString("dd/MM/yyyy") + ") ";
                }

                Session["SupplierName"] = txtSupplierSearch.Text;
                Session["Heading"] = DDT;
                Session["StartDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                Session["txtEndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                Session["SelectReport"] = "C";
                ltEmbed.Text = string.Format(embed, ResolveUrl("~/PrintPreview/InventoryReport.ashx?Id="), "");
                ModalPopupExtenderLogin.Show();
                // PurchaseDetailsCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "C");
            }
            else if (rbByDate.Checked)
            {
                string DDT = "";
                if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && !string.IsNullOrEmpty(txtSupplierSearch.Text) &&
                    !string.IsNullOrEmpty(txtEndDate.Text))
                {
                    DDT = "Report of (" + txtSupplierSearch.Text + ") From (" + txtStartDate.Text + " To " +
                          txtEndDate.Text + ")";
                }
                else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && string.IsNullOrEmpty(txtSupplierSearch.Text) &&
                         string.IsNullOrEmpty(txtEndDate.Text))
                {
                    DDT = "Report of (" + txtSupplierSearch.Text + ") From (" + txtStartDate.Text + " To " +
                          txtEndDate.Text + ")";
                }
                else
                {
                    DDT = "Report of (" + txtStartDate.Text + " To " +
                          txtEndDate.Text + ")";
                }
                Session["SupplierName"] = txtSupplierSearch.Text;
                Session["Heading"] = DDT;
                Session["StartDate"] = txtStartDate.Text;
                Session["txtEndDate"] = txtEndDate.Text;
                Session["SelectReport"] = "D";
                ltEmbed.Text = string.Format(embed, ResolveUrl("~/PrintPreview/InventoryReport.ashx?Id="), "");
                ModalPopupExtenderLogin.Show();
                //PurchaseDetailsCurrentDate(DDT, "", "D");
            }

        }
        else if (RadioButtonList1.SelectedValue == "Sales")
        {
            string DDT = "";
            if (rbCurDate.Checked)
            {
                DDT = "From (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";
                // SalesDetailsCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "C");
                Session["SupplierName"] = txtSupplierSearch.Text;
                Session["Heading"] = DDT;
                Session["StartDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                Session["txtEndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                Session["SelectReport"] = "C";
                ltEmbed.Text = string.Format(embed, ResolveUrl("~/PrintPreview/InventoryReport.ashx?Id="), "");
                ModalPopupExtenderLogin.Show();
            }
            else if (rbByDate.Checked)
            {
                if (string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text != "" && txtEndDate.Text != "")
                {
                    DDT = " From (" + txtStartDate.Text + " TO " + txtEndDate.Text + ")";

                }
                else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text == "" &&
                         txtEndDate.Text == "")
                {
                    DDT = "Report of  (" + txtSupplierSearch.Text + ") ";

                }
                else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text == "" &&
                         txtEndDate.Text == "")
                {
                    DDT = "Report of  (" + txtSupplierSearch.Text + ") From (" + txtStartDate.Text + " To " +
                          txtEndDate.Text + ")";

                }

                Session["SupplierName"] = txtCustomer.Text;
                Session["Heading"] = DDT;
                Session["StartDate"] = txtStartDate.Text;
                Session["txtEndDate"] = txtEndDate.Text;
                Session["SelectReport"] = "D";
                ltEmbed.Text = string.Format(embed, ResolveUrl("~/PrintPreview/InventoryReport.ashx?Id="), "");
                ModalPopupExtenderLogin.Show();

                //SalesDetailsCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "D");
            }
        }
        else if (RadioButtonList1.SelectedValue == "TI")
        {
            string DDT = "";
            if (rbCurDate.Checked)
            {
                DDT = "Items Transfer From (" + DateTime.Now.ToString("dd/MM/yyyy") + ")";
                //TransferItemCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "C");

                Session["SupplierName"] = txtCustomer.Text;
                Session["Heading"] = DDT;
                Session["StartDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                Session["txtEndDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                Session["SelectReport"] = "C";
                ltEmbed.Text = string.Format(embed, ResolveUrl("~/PrintPreview/InventoryReport.ashx?Id="), "");
                ModalPopupExtenderLogin.Show();
            }
            else if (rbByDate.Checked)
            {
                if (string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text != "" && txtEndDate.Text != "")
                {
                    DDT = "Items Transfer From (" + txtStartDate.Text + " TO " + txtEndDate.Text + ")";

                }
                else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text == "" && txtEndDate.Text == "")
                {
                    DDT = "Items Transfer Report of  (" + txtSupplierSearch.Text + ") ";

                }
                else if (!string.IsNullOrEmpty(txtSupplierSearch.Text) && txtStartDate.Text == "" && txtEndDate.Text == "")
                {
                    DDT = "Items Transfer Report of  (" + txtSupplierSearch.Text + ") From (" + txtStartDate.Text + " To " + txtEndDate.Text + ")";

                }

                Session["SupplierName"] = txtCustomer.Text;
                Session["Heading"] = DDT;
                Session["StartDate"] = txtStartDate.Text;
                Session["txtEndDate"] = txtEndDate.Text;
                Session["SelectReport"] = "D";
                ltEmbed.Text = string.Format(embed, ResolveUrl("~/PrintPreview/InventoryReport.ashx?Id="), "");
                ModalPopupExtenderLogin.Show();

                // TransferItemCurrentDate(DDT, DateTime.Now.ToString("dd/MM/yyyy"), "D");
            }
        }
        else if (RadioButtonList1.SelectedValue.Equals("SWIS"))
        {
            Session["SupplierID"] = lblShiftmentID.Text;
            Session["SupplierName"] = lblItemsId.Text;
            Session["Heading"] = "";
            Session["StartDate"] = "";
            Session["txtEndDate"] = "";
            Session["SelectReport"] = "D";
            ltEmbed.Text = string.Format(embed, ResolveUrl("~/PrintPreview/InventoryReport.ashx?Id="), "");
            ModalPopupExtenderLogin.Show();
        }
    }

    protected void ibHelp_Click(object sender, ImageClickEventArgs e)
    {
        string filePath = "~/Help/" + "HelpPopup.pdf";
        string Name = "HelpPopup.pdf";
        Response.ContentType = "doc/docx";
        Response.AddHeader("Content-Disposition", "attachment;filename=\"" + Name + "\"");
        Response.TransmitFile(Server.MapPath(filePath));
        Response.End();
    }
    protected void ibCancel_Click(object sender, ImageClickEventArgs e)
    {
        ModalPopupExtenderLogin.Hide();
    }
    protected void txtShiftmentNo_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = ShiftmentAssignManager.GetShowShiftmentAssignOnSearch(txtShiftmentNo.Text);
        if (dt.Rows.Count > 0)
        {
            txtShiftmentNo.Text = dt.Rows[0]["ShiftmentNO"].ToString();
            //txtShiftmentDate.Text = dt.Rows[0]["ShiftmentDate"].ToString();
            lblShiftmentID.Text = dt.Rows[0]["ID"].ToString();
            txtItemsName.Focus();
            // UP1.Update();
        }
        else
        {
            txtShiftmentNo.Text = lblShiftmentID.Text = "";
            txtShiftmentNo.Focus();
        }
    }
    protected void txtItemsName_TextChanged(object sender, EventArgs e)
    {
        DataTable dtItems = ClsItemDetailsManager.GetShowItemsInfoSearch(txtItemsName.Text, "");
        if (dtItems.Rows.Count > 0)
        {
            lblItemsId.Text = dtItems.Rows[0]["ID"].ToString();
            txtItemsName.Text = dtItems.Rows[0]["Name"].ToString();
        }
        else
        {
            txtItemsName.Text = lblItemsId.Text = "";
            txtItemsName.Focus();
        }
    }
    protected void txtCustomerPayment_TextChanged(object sender, EventArgs e)
    {
        DataTable dtCustomer = _aclsClientInfoManager.GetCustomerOnSearch("where UPPER(SearchName) = UPPER('" + txtCustomerPayment.Text + "')", 0);
        if (dtCustomer.Rows.Count > 0)
        {
            hfCustomerIDPayment.Value = dtCustomer.Rows[0]["ID"].ToString();
            txtCustomerPayment.Text = dtCustomer.Rows[0]["ContactName"].ToString();
            Session["Customer_COA"] = dtCustomer.Rows[0]["Gl_CoaCode"].ToString();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step.!!');", true);
            Session["Customer_COA"] = null;
            hfCustomerIDPayment.Value = "";
            txtCustomerPayment.Focus();
            return;
        }
    }
    protected void rbCurDatePayment_CheckedChanged(object sender, EventArgs e)
    {
        txtStartDatePayment.Visible = txtEndDatePayment.Visible = lblToPayment.Visible = false;
        rbCurDatePayment.Checked = true;
        rbByDatePayment.Checked = false;
        UPInvReport.Update();
    }
    protected void rbByDatePayment_CheckedChanged(object sender, EventArgs e)
    {
        txtStartDatePayment.Visible = txtEndDatePayment.Visible = lblToPayment.Visible = true;
        rbCurDatePayment.Checked = false;
        rbByDatePayment.Checked = true;
        UPInvReport.Update();
    }
    protected void txtSupplierSearchDue_TextChanged(object sender, EventArgs e)
    {
        string Parameter = "";
        Parameter = " where UPPER([SearchName])='" + txtSupplierSearchDue.Text.ToUpper() + "' ";
        DataTable dtSupplier = PurchaseVoucherManager.GetSupplierInfo(txtSupplierSearchDue.Text);
        if (dtSupplier != null)
        {
            if (dtSupplier.Rows.Count > 0)
            {

                hfSupplierIDPayment.Value = dtSupplier.Rows[0]["ID"].ToString();
                txtSupplierSearch.Text = dtSupplier.Rows[0]["SupplierSearch"].ToString();

                txtSupplierSearch.Focus();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Not Found Supplier.!!');", true);
                txtSupplierSearch.Text = "";
                hfSupplierIDPayment.Value = "";
                txtSupplierSearch.Focus();
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Not Found Supplier.!!');", true);
            txtSupplierSearch.Text = "";
            hfSupplierIDPayment.Value = "";
            txtSupplierSearch.Focus();
        }
        UPInvReport.Update();
    }
    protected void rbByDateExpanse_CheckedChanged(object sender, EventArgs e)
    {
        txtStartDateExpanse.Visible = txtEndDateExpanse.Visible = lblToExpanse.Visible = true;
        rbCurDateExpanse.Checked = false;
        rbByDateExpanse.Checked = true;
    }
    protected void rbCurDateExpanse_CheckedChanged(object sender, EventArgs e)
    {
        txtStartDateExpanse.Visible = txtEndDateExpanse.Visible = lblToExpanse.Visible = false;
        rbCurDateExpanse.Checked = true;
        rbByDateExpanse.Checked = false;
    }
}