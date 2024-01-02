using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;
using sales;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

using System.Drawing;
using Delve;
using System.IO;
using System.Drawing.Printing;
using System.Configuration;
using System.Diagnostics;

public partial class BranchSalesVoucher : System.Web.UI.Page
{
    private static Permis per;
    private static DataTable dtmsr = new DataTable();
    private static VouchManager _aVouchManager = new VouchManager();
    SalesManager _aSalesManager = new SalesManager();
    clsClientInfoManager _aclsClientInfoManager = new clsClientInfoManager();

    // ClsItemDtlManager _aClsItemDtlManager=new ClsItemDtlManager();
    ItemManager _aItemManager = new ItemManager();

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
                    // UpItemsDetails.Update();
                    //UPPaymentMtd.Update();
                    //UpSearch.Update();
                    txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    Panel1.Visible = txtItemsCode.Enabled = btnSave.Visible = false;
                    dgSVMst.Visible = true;
                    btnNew.Visible = true;

                    DataTable dtpaymentType = IdManager.GetShowDataTable("Select Name,ID from PaymentMethod where Id!=2  order by id asc");
                    //DataTable dtpaymentTypeReceived = IdManager.GetShowDataTable("Select Name,ID from PaymentMethod   order by id asc");
                    ddlPaymentTypeFrom.DataSource = dtpaymentType;
                    ddlPaymentTypeFrom.DataTextField = "Name";
                    ddlPaymentTypeFrom.DataValueField = "ID";
                    ddlPaymentTypeFrom.DataBind();

                    ddlPaymentTypeTo.DataSource = dtpaymentType;
                    ddlPaymentTypeTo.DataTextField = "Name";
                    ddlPaymentTypeTo.DataValueField = "ID";
                    ddlPaymentTypeTo.DataBind();
                    // util.PopulationDropDownList(ddlPaymentTypeFrom, "Name", "ID", dtpaymentType);
                    // util.PopulationDropDownList(ddlPaymentTypeTo, "Name", "ID", dtpaymentTypeReceived);
                    // ddlPaymentTypeTo.SelectedIndex =ddlPaymentTypeFrom.SelectedIndex= 1;

                    DataTable dtBankList =
                        IdManager.GetShowDataTable(
                            "Select bank_name,bank_id from bank_info where Active=1  order by bank_name asc");
                    util.PopulationDropDownList(ddlBankNameFrom, "bank_name", "bank_id", dtBankList);

                    Session["Cash_Code"] = IdManager.GetShowSingleValueString("CASH_CODE", "BOOK_NAME", "GL_SET_OF_BOOKS", "AMB");
                    DataTable dt = SalesManager.GetShowSalesDetails();
                    dgSVMst.DataSource = dt;
                    Session["SvMst"] = dt;
                    dgSVMst.DataBind();
                    VisiblePayment(false, false, false, false, false, false, false, false);
                    btnDelete.Enabled = btnSave.Enabled = true;
                    btnNew.Visible = true;
                    hfCustomerCoa.Value = txtInvoiceNo.Text = "";
                    DataTable dtFixCode = _aVouchManager.getFixCode();
                    ViewState["dtFixCode"] = dtFixCode;
                    lblInvNo.Text = "";
                    ViewState["SalesID"] = ViewState["OldSV"] = "";

                    lblAcountNo.Visible = ddlBankName.Visible =
                        lblBankNameTo.Visible = lblAcountNo.Visible = ddlAccountNo.Visible = lblAccNoPint.Visible = lblRcbBankPoint.Visible = false;
                    txtCustomerName.Focus();
                    lblAcountNoFrom.Visible = ddlBankNameFrom.Visible = lblBankNameFrom.Visible = lblStatus.Visible =
                        lblApprovedDate.Visible = txtApprovedDate.Visible = ddlPaymentStatus.Visible =
                            lblChekNo.Visible = txtcheeckNo.Visible = txtAccountNo.Visible = false;
                }
            }
            catch 
            {
                Response.Redirect("Default.aspx?sid=sam");
              
            }
        }
    }

    private void getEmptyDtl()
    {

        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("ID", typeof(string));
        dtDtlGrid.Columns.Add("Code", typeof(string));
        //dtDtlGrid.Columns.Add("Name", typeof(string));

        dtDtlGrid.Columns.Add("CodeWiseSearchItems", typeof(string));
        dtDtlGrid.Columns.Add("msr_unit_code", typeof(string));
        dtDtlGrid.Columns.Add("Tax", typeof(string));
        dtDtlGrid.Columns.Add("DiscountAmount", typeof(string));
        dtDtlGrid.Columns.Add("SPrice", typeof(string));
        dtDtlGrid.Columns.Add("Qty", typeof(string));
        dtDtlGrid.Columns.Add("Total", typeof(string));
        dtDtlGrid.Columns.Add("TotalClosingStock", typeof(string));
        dtDtlGrid.Columns.Add("Barcode", typeof(string));
        dtDtlGrid.Columns.Add("Remarks", typeof(string));
        DataRow dr = dtDtlGrid.NewRow();
        dr["CodeWiseSearchItems"] = "";
        dr["Tax"] = 0;
        dr["DiscountAmount"] = 0;
        dr["SPrice"] = 0;
        dr["Qty"] = 0;
        dr["Total"] = 0;
        dr["TotalClosingStock"] = 0;
        dtDtlGrid.Rows.Add(dr);
        dgSV.DataSource = dtDtlGrid;
        dgSV.DataBind();
        //ViewState["SV"] = dtDtlGrid;
        
    }

    protected void txtCustomerName_TextChanged(object sender, EventArgs e)
    {
        string Parameter = "";
        Parameter = " where BranchId='" + Session["BranchId"].ToString()+ "' and  UPPER([SearchName])='" + txtCustomerName.Text.ToUpper() + "' ";
        DataTable dtCustomer = _aclsClientInfoManager.GetCustomerOnSearch(Parameter, 0);
        if (dtCustomer != null)
        {
            if (dtCustomer.Rows.Count > 0)
            {
                txtCustomerName.Text = dtCustomer.Rows[0]["ContactName"].ToString();
                txtRemarks.Text = "Items Sales By Customer : " + txtCustomerName.Text;
                hfCustomerCoa.Value = dtCustomer.Rows[0]["Gl_CoaCode"].ToString();
                hfCustomerID.Value = dtCustomer.Rows[0]["ID"].ToString();
            }
            else
            {
                txtCustomerName.Text = "";
                hfCustomerCoa.Value = "";
                hfCustomerID.Value = "";
                txtRemarks.Text = "Items Sales";
                txtCustomerName.Focus();
            }
        }
        else
        {
            txtCustomerName.Text = "";
            hfCustomerCoa.Value = "";
            hfCustomerID.Value = "";
            txtRemarks.Text = "Items Sales";
            txtCustomerName.Focus();
        }

        txtCustomerName.Focus();
    }

    protected void txtCode_TextChanged(object sender, EventArgs e)
    {
        bool IsChk = false;
        string[] splitCode = txtItemsCode.Text.Trim().Split('-');

        //string Code = IdManager.GetShowSingleValueString("Code",
        //    "ItemInformationForReport where upper(Name_Brand_Model)=upper('" + txtItemsCode.Text + "')");

        DataTable dtItemsSearch = _aItemManager.GetSearchItemsOnBranchStock(txtItemsCode.Text.ToUpper(), splitCode.Length,Session["BranchId"].ToString());
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

        txtSubTotal.Text = tot.ToString();
        txtVat.Text = totVat.ToString();

        VetAndDiscount();
        //txtGrandTotal.Text = (tot + ((tot * totVat) / 100)).ToString("N2");
        //txtDue.Text = ((tot + ((tot * totVat) / 100)) - Convert.ToDecimal(txtPayment.Text)).ToString("N2");
    }

    protected void txtVat_TextChanged(object sender, EventArgs e)
    {
        VetAndDiscount();
    }


    protected void txtInvoiceNo_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = SaleReturnManager.GetShowSLMasterInfo(txtInvoiceNo.Text);
        if (dt.Rows.Count > 0)
        {
            lblInvNo.Text = dt.Rows[0]["ID"].ToString();
            btnFind_Click(sender, e);

            btnNew.Visible = false;
            btnSave.Visible = true;
            // UpItemsDetails.Update();
            // UPPaymentMtd.Update();
            //UpSearch.Update();
        }
    }

    protected void btnFind_Click1(object sender, EventArgs e)
    {
        txtInvoiceNo_TextChanged(sender, e);
    }

    protected void TotalDiscount_TextChange(object sender, EventArgs e)
    {
        VetAndDiscount();
    }

    private void VetAndDiscount()
    {
        decimal subtotal, vat, discount, totalPayment;
        try
        {
            subtotal = Convert.ToDecimal(txtSubTotal.Text);
        }
        catch (Exception ex)
        {
            subtotal = 0;
        }

        try
        {
            vat = Convert.ToDecimal(txtVat.Text);
        }
        catch (Exception ex)
        {
            vat = 0;
        }

        try
        {
            discount = Convert.ToDecimal(txtDiscount.Text);
        }
        catch (Exception ex)
        {
            discount = 0;
        }

        try
        {
            totalPayment = Convert.ToDecimal(txtPayment.Text);
        }
        catch (Exception ex)
        {
            totalPayment = 0;
        }

        txtGrandTotal.Text = (((subtotal - discount) + ((subtotal - discount) * vat / 100))).ToString("N2");
        txtDue.Text = (((subtotal - discount) + ((subtotal - discount) * vat / 100)) - totalPayment).ToString("N2");
        //  UpItemsDetails.Update();
    }


    private void PointCalculation()
    {
        if (Convert.ToDecimal(txtPayment.Text) > 0)
        {
            string FirstDigit = "", LastDigit = "", return_value = "", Number = "";

            int LastFirst = 0, LastLast = 0;
            Number = Convert.ToDecimal(txtPayment.Text).ToString("N2");

            string[] words = Number.Split('.');
            FirstDigit = words[0].ToString();
            LastDigit = words[1].ToString();
            int length = LastDigit.Length;
            LastFirst = Convert.ToInt32(LastDigit.Substring(0, 1));
            LastLast = Convert.ToInt32(LastDigit.Substring(length - 1, 1));


            if (LastFirst == 9 && LastLast > 7)
                return_value = (Convert.ToDecimal(FirstDigit) + 1).ToString("N2");

            else if (LastLast > -1 && LastLast < 3)
                return_value = FirstDigit + '.' + LastFirst.ToString() + '0';


            else if (LastLast > 2 && LastLast < 8)
                return_value = FirstDigit + '.' + LastFirst.ToString() + '5';

            else if (LastLast > 7 && LastLast < 10)
                return_value = FirstDigit + '.' + (LastFirst + 1).ToString() + '0';

            else
                return_value = Number;

            txtLastFigarAmount.Text = return_value;
            //txtAmount.Text = return_value;
        }

        //else
        //{
        //    txtPayment.Text = txtAmount.Text;
        //    txtDue.Text = (Convert.ToDecimal(txtGrandTotal.Text) - Convert.ToDecimal(txtPayment.Text)).ToString();
        //}
    }

    private void amountPointCalculation()
    {
        if (Convert.ToDecimal(txtAmount.Text) > 0)
        {
            string FirstDigit = "", LastDigit = "", return_value = "", Number = "";

            int LastFirst = 0, LastLast = 0;
            Number = Convert.ToDecimal(txtAmount.Text).ToString("N2");

            string[] words = Number.Split('.');
            FirstDigit = words[0].ToString();
            LastDigit = words[1].ToString();
            int length = LastDigit.Length;
            LastFirst = Convert.ToInt32(LastDigit.Substring(0, 1));
            LastLast = Convert.ToInt32(LastDigit.Substring(length - 1, 1));


            if (LastFirst == 9 && LastLast > 7)
                return_value = (Convert.ToDecimal(FirstDigit) + 1).ToString("N2");

            else if (LastLast > -1 && LastLast < 3)
                return_value = FirstDigit + '.' + LastFirst.ToString() + '0';


            else if (LastLast > 2 && LastLast < 8)
                return_value = FirstDigit + '.' + LastFirst.ToString() + '5';

            else if (LastLast > 7 && LastLast < 10)
                return_value = FirstDigit + '.' + (LastFirst + 1).ToString() + '0';

            else
                return_value = Number;


            if (Convert.ToDecimal(txtAmount.Text).ToString("N2") != Convert.ToDecimal(return_value).ToString("N2"))
            {
              
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('This Amount Is Not Right!!','red',0);", true);

            }


            txtAmount.Text = return_value;
        }

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
            else;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
        }  
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
            else;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
        }  
    }

    protected void txtSalesPrice_TextChanged(object sender, EventArgs e)
    {
        try
        {
            Quantity(sender);
            ((TextBox)dgSV.Rows[dgSV.Rows.Count - 2].FindControl("txtDiscount")).Focus();
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

    protected void txtRemarks_TextChanged(object sender, EventArgs e)
    {
        try
        {
            Quantity(sender);
            ((TextBox)dgSV.Rows[dgSV.Rows.Count - 2].FindControl("txtQty")).Focus();
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
                var qtyCk = Convert.ToDecimal(((TextBox) gvr.FindControl("txtQty")).Text);
            }
            catch
            {

                ((TextBox) gvr.FindControl("txtQty")).Text = "1";
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                       "alert('This Qty is not valid So 1  Qty is fixed');", true);
            }
           



            if (string.IsNullOrEmpty(lblInvNo.Text))
            {
                if (((Convert.ToDouble(dr["TotalClosingStock"])) <
                     Convert.ToDouble(((TextBox)gvr.FindControl("txtQty")).Text)))
                {
                    string Mgs = "Items Quantity Over This Closing Quantity.\\n Tolat Closing Qiantity : (" +
                                  dr["TotalClosingStock"].ToString() + ")..!!";
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                        "alert('" + Mgs + "');", true);
                    ((TextBox)gvr.FindControl("txtQty")).Text = "0";


                    return;
                }
            }
            else
            {
                foreach (DataRow drRow in dt.Rows)
                {
                    if (dr["ID"].ToString().Equals(drRow["ID"].ToString()))
                    {
                        Qty += Convert.ToDouble(drRow["Qty"]);
                        SalesQty += Convert.ToDouble(drRow["SaleQty"]);
                    }
                }

                if (((Convert.ToDouble(dr["TotalClosingStock"]) + SalesQty) <
                     Convert.ToDouble(((TextBox)gvr.FindControl("txtQty")).Text)))
                {
                    string Mgs = "Items Quantity Over This Closing Quantity.\\n Tolat Closing Qiantity : (" +
                                  dr["TotalClosingStock"].ToString() + ")..!!";
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('" + Mgs + "','INDIANRED',2);", true);

                    ((TextBox)gvr.FindControl("txtQty")).Text = "0";

                    return;

                }
            }

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

    protected void txtDiscount_TextChanged(object sender, EventArgs e)
    {
        try
        {
            Quantity(sender);
            ((TextBox)dgSV.Rows[dgSV.Rows.Count - 1].FindControl("txtQty")).Focus();

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

    protected void btnNew_Click(object sender, EventArgs e)
    {
        RefreshAll();
        lblPreviousDue.Text = "";
        txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        txtInvoiceNo.Text = "INV-RD-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() +
                            DateTime.Now.Day.ToString() + "-00" +
                            IdManager.GetShowSingleValueString("InvoiceID", "FixGlCoaCode") + Session["BranchId"].ToString();

        Panel1.Visible = txtItemsCode.Enabled = btnNew.Visible = false;
        dgSVMst.Visible = true;
        //dgSVMst.DataSource = SalesManager.GetShowSalesDetails();
        //dgSVMst.DataBind();
        Clear();
        txtRemarks.Text = "Item Sales.";
        btnSave.Visible = true;
        lblAcountNo.Visible = ddlBankName.Visible =
            lblBankNameTo.Visible = lblAcountNo.Visible = ddlAccountNo.Visible = lblAccNoPint.Visible = lblRcbBankPoint.Visible = false;
        DataTable dtpaymentTypeReceived = IdManager.GetShowDataTable("Select Name,ID from PaymentMethod   order by id asc");
        //util.PopulationDropDownList(ddlPaymentTypeFrom, "Name", "ID", dtpaymentType);




        DataTable dtpaymentType = IdManager.GetShowDataTable("Select Name,ID from PaymentMethod where Id!=2  order by id asc");


        ddlPaymentTypeTo.DataSource = dtpaymentType;
        ddlPaymentTypeTo.DataTextField = "Name";
        ddlPaymentTypeTo.DataValueField = "ID";
        ddlPaymentTypeTo.DataBind();

        ddlPaymentTypeTo.SelectedIndex = 0;



        Session["Cash_Code"] =
            IdManager.GetShowSingleValueString("CASH_CODE", "BOOK_NAME", "GL_SET_OF_BOOKS", "AMB");
        ddlAccountNo.SelectedIndex = ddlBankName.SelectedIndex = -1;
        dgPaymentInfo.DataSource = null;
        dgPaymentInfo.DataBind();
        getEmptyDtl();
        GetPaymentEmptyTable();
        // Panel1.Width = 100;
        lblInvNo.Text = hfCustomerCoa.Value = hfCustomerID.Value = string.Empty;

        //txtCustomerName.Focus();   

    }

    protected void btnFind_Click(object sender, EventArgs e)
    {
        //lblInvNo.Text = dgSVMst.SelectedRow.Cells[6].Text.Trim();
        try
        {
            Sales aSales = SalesManager.GetBranchShowSalesInfo(lblInvNo.Text, Session["BranchId"].ToString());
            if (aSales != null)
            {
                Clear();
                txtInvoiceNo.Text = aSales.Invoice;
                txtDate.Text = aSales.Date;
                txtSubTotal.Text = aSales.Total;
                txtGrandTotal.Text = aSales.GTotal;
                txtPayment.Text = aSales.CReceive;
                hfCustomerID.Value = aSales.Customer;
                string Parameter = " where ID=" + hfCustomerID.Value + " and BranchId='" + Session["BranchId"].ToString() + "' or CommonCus=1";
                DataTable dtCustomer = _aclsClientInfoManager.GetCustomerOnSearch(Parameter, 2);
                if (dtCustomer != null)
                {
                    txtCustomerName.Text = dtCustomer.Rows[0]["ContactName"].ToString();
                    // txtRemarks.Text = "Items Sales By Customer : " + txtCustomerName.Text;
                    hfCustomerCoa.Value = dtCustomer.Rows[0]["Gl_CoaCode"].ToString();
                    //hfCustomerID.Value = dtCustomer.Rows[0]["ID"].ToString();
                }

                ddlDelevery.SelectedValue = aSales.DvStatus;
                txtDeleveryDate.Text = aSales.DvDate;
                txtRemarks.Text = aSales.Remarks;
                // ddlPaymentTypeFrom.SelectedValue = aSales.PMethod;
                if (aSales.PMethod == "3")
                {
                    DataTable dtpaymentType = IdManager.GetShowDataTable("Select Name,ID from PaymentMethod where Id!=2  order by id asc");
                    //DataTable dtpaymentTypeReceived = IdManager.GetShowDataTable("Select Name,ID from PaymentMethod   order by id asc");
                    ddlPaymentTypeFrom.DataSource = dtpaymentType;
                    ddlPaymentTypeFrom.DataTextField = "Name";
                    ddlPaymentTypeFrom.DataValueField = "ID";
                    ddlPaymentTypeFrom.DataBind();

                    ddlPaymentTypeTo.DataSource = dtpaymentType;
                    ddlPaymentTypeTo.DataTextField = "Name";
                    ddlPaymentTypeTo.DataValueField = "ID";
                    ddlPaymentTypeTo.DataBind();
                    ddlPaymentTypeTo.SelectedValue = aSales.PMethod;
                    if (!string.IsNullOrEmpty(aSales.BankId))
                    {
                        DataTable dtBankList =
                   IdManager.GetShowDataTable(
                      "Select t2.bank_name as bank_name,t2.bank_id from dbo.bank_info t2 where t2.Active=1 order by bank_name asc");
                        ddlBankName.DataSource = dtBankList;
                        ddlBankName.DataValueField = "bank_id";
                        ddlBankName.DataTextField = "bank_name";
                        ddlBankName.DataBind();
                        try
                        {
                            ddlBankName.Items.Insert(0, "");
                        }
                        catch
                        {
                        }
                        ddlBankName.SelectedValue = aSales.BankId;
                        ddlBankName.Visible = true;
                        if (!string.IsNullOrEmpty(aSales.PMNumber))
                        {
                            DataTable dtBankList1 =
                    IdManager.GetShowDataTable(
                        "Select t2.bank_name+' - '+AccountNo as bank_name,t1.ID from dbo.bank_branch t1 inner join dbo.bank_info t2 on t2.Bank_Id=t1.bank_id where t1.DeleteBy is null and t1.bank_id='" + ddlBankName.SelectedValue + "' and t1.BankType ='C' order by bank_name asc");
                            util.PopulationDropDownList(ddlAccountNo, "bank_name", "ID", dtBankList1);
                            ddlAccountNo.SelectedValue = aSales.PMNumber;
                            ddlAccountNo.Visible = true;
                        }
                    }
                }


                txtLocalCusPhone.Text = aSales.LocalCusPhone;
                txtLocalCusAddress.Text = aSales.LocalCusAddress;
                txtLocalCustomer.Text = aSales.LocalCustomer;
                txtNote.Text = aSales.Note;

                DataTable DT1 = SalesManager.GetBranchSalesDetails(lblInvNo.Text,Session["BranchId"].ToString());
                if (DT1.Rows.Count > 0)
                {
                    dgSV.DataSource = DT1;
                    ViewState["SV"] = DT1;
                    ViewState["OldSV"] = DT1;
                    dgSV.DataBind();
                    ShowFooterTotal(DT1);
                }



                txtVat.Text = Convert.ToDouble(aSales.Tax).ToString("N2");
                txtDiscount.Text = Convert.ToDouble(aSales.Disount).ToString("N2");

                DataTable dtOrderPay = _aSalesManager.GetOrderPayment(lblInvNo.Text);
                if (dtOrderPay.Rows.Count > 0)
                {
                    dgPaymentInfo.DataSource = dtOrderPay;
                    ViewState["paymentInfo"] = dtOrderPay;
                    dgPaymentInfo.DataBind();
                }
                VetAndDiscount();
                txtAmount.Text = "0";
                var Refund = Convert.ToDouble(aSales.CRefund);
                if (Refund > 0)
                {
                    txtDue.Text = ((-1) * Refund).ToString("N2");
                }
                else
                {
                    txtDue.Text = Convert.ToDouble(aSales.Due).ToString("N2");
                }

                //UpItemsDetails.Update();
                //UPPaymentMtd.Update();
                //UpSearch.Update();
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

    private void Clear()
    {
        Panel1.Visible = txtItemsCode.Enabled = true;
        txtDeleveryDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        dgSVMst.Visible = false;
        txtSubTotal.Text = txtVat.Text = txtDiscount.Text = txtPayment.Text = txtDue.Text = txtChequeAmount.Text = txtGrandTotal.Text = txtAmount.Text = txtLastFigarAmount.Text = "0";
        ddlDelevery.SelectedIndex = ddlBankName.SelectedIndex = -1;
        txtRemarks.Text = txtCustomerName.Text = hfCustomerID.Value = hfCustomerCoa.Value = "";
        getEmptyDtl();
        ViewState["SV"] = null;
        txtItemsCode.Focus();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            //UPCustomer.Update();
            DataTable dtSalesDetails = (DataTable)ViewState["SV"];
            DataTable dtPayment = (DataTable)ViewState["paymentInfo"];
            if (dtSalesDetails == null)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('No Items added in the list..!!','INDIANRED',3);", true);

                return;
            }
            if (string.IsNullOrEmpty(txtInvoiceNo.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('input invoice No.!!','INDIANRED',3);", true);

                return;
            }
            if (dtSalesDetails.Rows.Count <= 0)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('No Items added in the list..!!','INDIANRED',3);", true);
                return;
            }
            if (Convert.ToDouble(txtDue.Text) > 0 && string.IsNullOrEmpty(txtCustomerName.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Local customer must be full payment..!!','INDIANRED',3);", true);


                return;
            }
            if (Convert.ToDouble(txtDue.Text) > 0 && hfCustomerID.Value == "1")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Local customer must be full payment..!!','INDIANRED',3);", true);

                return;
            }
            //if (Convert.ToDouble(txtDue.Text) < 0)
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Please check your payment status.clear and set again.!!');",
            //        true);
            //    return;
            //}
            //if (ddlPaymentTypeTo.SelectedValue == "3" && string.IsNullOrEmpty(ddlBankName.SelectedValue))
            //{
            //    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Places Select Bank Name..!!','INDIANRED',3);", true);

            //    return;
            //}
            //if (!string.IsNullOrEmpty(ddlBankName.SelectedValue) && string.IsNullOrEmpty(ddlAccountNo.SelectedValue))
            //{
            //    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Places Select Bank Account No..!!','INDIANRED',3);", true);

            //    return;
            //}
            double Dis = 0;
            if (Panel1.Visible == false)
            {
            }
            else
            {
                int CountInvUpdate = IdManager.GetShowSingleValueInt("COUNT(*)",
                    "[Order] where UPPER([InvoiceNo])='" + txtInvoiceNo.Text.ToUpper() + "' and ID!='" + lblInvNo.Text +
                    "' ");
                if (CountInvUpdate > 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('This invoice already exist in database.!!','INDIANRED',3);", true);

                    return;
                }

                Sales aSales = SalesManager.GetBranchShowSalesInfo(lblInvNo.Text,Session["BranchId"].ToString());
                Dis = Convert.ToDouble(txtDiscount.Text);
                UPPaymentMtd.Update();
                if (aSales != null)
                {
                    if (per.AllowEdit == "Y")
                    {
                        aSales.ID = lblInvNo.Text;
                        aSales.Invoice = txtInvoiceNo.Text.ToUpper();
                        aSales.Date = txtDate.Text;
                        aSales.Total = txtSubTotal.Text.Replace(",", "");
                        aSales.Tax = txtVat.Text.Replace(",", "");
                        aSales.Disount = txtDiscount.Text.Replace(",", "");
                        aSales.GTotal = txtGrandTotal.Text.Replace(",", "");

                        aSales.paymentID = ddlPaymentTypeTo.SelectedValue;
                        var RefundOrDue = Convert.ToDecimal(txtDue.Text);


                        decimal due, refund;
                        if (RefundOrDue < 0)
                        {
                            due = 0;
                            refund = (-1) * RefundOrDue;

                        }
                        else
                        {
                            due = RefundOrDue;
                            refund = 0;
                        }


                        aSales.CReceive = txtPayment.Text.Replace(",", "");


                        aSales.Due = due.ToString("N2");

                        aSales.CRefund = refund.ToString("N2");
                        //aSales.CRefund = "0";

                        aSales.BranchId = Session["BranchId"].ToString();






                        if (string.IsNullOrEmpty(txtCustomerName.Text))
                        {
                            DataTable dtCustomerDtl = _aclsClientInfoManager.GetCustomerOnSearch(" and BranchId='"+Session["BranchId"].ToString()+"'", 5);
                            if (dtCustomerDtl != null)
                            {
                                txtCustomerName.Text = dtCustomerDtl.Rows[0]["ContactName"].ToString();
                                txtRemarks.Text = "Items Sales By Customer : " + txtCustomerName.Text +
                                                  " for Local Customer (" + txtLocalCustomer.Text + ")";
                                hfCustomerCoa.Value = dtCustomerDtl.Rows[0]["Gl_CoaCode"].ToString();
                                hfCustomerID.Value = dtCustomerDtl.Rows[0]["ID"].ToString();
                            }
                        }


                        aSales.Customer = hfCustomerID.Value;
                        aSales.CustomerCoa = hfCustomerCoa.Value;

                        aSales.LocalCustomer = txtLocalCustomer.Text.Replace("'", "");
                        aSales.LocalCusPhone = txtLocalCusPhone.Text.Replace("'", "");
                        aSales.LocalCusAddress = txtLocalCusAddress.Text.Replace("'", "");
                        aSales.Note = txtNote.Text;
                        aSales.OrderType = "C";
                        //  aSales.GuarantorID = lblguarantorID.Text;
                        aSales.DvStatus = ddlDelevery.SelectedValue;
                        aSales.DvDate = txtDate.Text;
                        var ExtraAmount = Convert.ToDecimal(txtLastFigarAmount.Text) -
                                          Convert.ToDecimal(txtPayment.Text);
                        //aSales.ExtraAmount = ExtraAmount.ToString("N2");
                        aSales.ExtraAmount = "0";
                        aSales.Remarks = txtRemarks.Text;

                        aSales.BankCoaCode = "";
                       //if (ddlBankName.SelectedValue != null && ddlBankName.SelectedValue != "" &&
                       //         ddlBankName.SelectedValue != "0")
                       //     {
                       //         aSales.BankCoaCode = IdManager.GetShowSingleValueString("Gl_Code", "ID",
                       //             "dbo.bank_branch", ddlBankName.SelectedValue);
                       //     }
                       //     else
                       //     {
                       //         aSales.BankCoaCode = "";

                       //     }
                     

                        //if (!string.IsNullOrEmpty(ddlBankNameFrom.SelectedValue))
                        //{

                        //    aSales.BankName = ddlBankNameFrom.SelectedItem.Text;
                        //    aSales.PMNumber = txtcheeckNo.Text;
                        //    aSales.ChequeDate = txtApprovedDate.Text;
                        //    aSales.ChequeAmount = txtChequeAmount.Text.Replace(",", "");
                        //    aSales.Chk_Status = ddlPaymentStatus.SelectedValue;
                        //    aSales.BankCoaCode = IdManager.GetShowSingleValuestring("Gl_Code",
                        //        " View_Bank_Branch_Info where ID='" + ddlBankNameFrom.SelectedValue + "' ");
                        //}
                       // aSales.PMethod = ddlPaymentTypeTo.SelectedValue;
                       //if (ddlPaymentTypeTo.SelectedValue != "1")
                       //{
                       //    aSales.BankId = ddlBankName.SelectedValue;
                       //    aSales.PMNumber = ddlAccountNo.SelectedValue;
                       //}
                        aSales.CustomerName = txtCustomerName.Text;
                        aSales.LoginBy = Session["user"].ToString();

                        // DataTable dtinstallment = (DataTable)ViewState["installment"];
                        string VCH_SYS_NO_SVJV = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                            "t1.PAYEE='SVSLJV' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                            txtInvoiceNo.Text);
                        VouchMst vmst = VouchManager.GetVouchMst(VCH_SYS_NO_SVJV.Trim());
                        if (vmst != null)
                        {
                            vmst.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
                            vmst.ValueDate = txtDate.Text;
                            vmst.VchCode = "03";
                            //vmst.SerialNo = txtGRNO.Text.Trim();
                            vmst.Particulars = txtRemarks.Text;
                            vmst.ControlAmt = txtSubTotal.Text.Replace(",", "");
                            //vmst.Payee = "PV";
                            vmst.CheqAmnt = "0";
                            vmst.UpdateUser = Session["user"].ToString().ToUpper();
                            vmst.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                            vmst.AuthoUserType = Session["userlevel"].ToString();
                        }

                        string VCH_SYS_NO_SVCV = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                            "t1.PAYEE='SVSLCV' and SUBSTRING(t1.VCH_REF_NO,1,2)='CV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                            txtInvoiceNo.Text);
                        VouchMst vmstCV = VouchManager.GetVouchMst(VCH_SYS_NO_SVCV.Trim());
                        if (vmstCV != null)
                        {
                            vmstCV.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
                            vmstCV.ValueDate = txtDate.Text;
                            vmstCV.VchCode = "02";
                            vmstCV.RefFileNo = "";
                            //vmst.SerialNo = txtGRNO.Text.Trim();
                            vmstCV.Particulars = txtRemarks.Text;
                            vmstCV.ControlAmt = aSales.GTotal.Replace(",", "");
                            //vmst.Payee = "PV";
                            vmstCV.CheqAmnt = "0";
                            vmstCV.UpdateUser = Session["user"].ToString().ToUpper();
                            vmstCV.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                            vmstCV.AuthoUserType = Session["userlevel"].ToString();
                        }
                        else
                        {
                            //********* Credit Voucher *********//
                            if (Convert.ToDecimal(txtPayment.Text) > 0)
                            {
                                vmstCV = new VouchMst();
                                vmstCV.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
                                vmstCV.ValueDate = txtDate.Text;
                                vmstCV.VchCode = "02";
                                vmstCV.RefFileNo = "New";
                                vmstCV.VolumeNo = "";
                                vmstCV.SerialNo = txtInvoiceNo.Text.Trim();
                                vmstCV.Particulars = txtRemarks.Text;
                                vmstCV.ControlAmt = aSales.GTotal.Replace(",", "");
                                vmstCV.Payee = "SVSLCV";
                                vmstCV.CheckNo = "";
                                vmstCV.CheqDate = DateTime.Now.ToString("dd/MM/yyyy");
                                vmstCV.CheqAmnt = "0";
                                vmstCV.MoneyRptNo = "";
                                vmstCV.MoneyRptDate = "";
                                vmstCV.TransType = "R";
                                vmstCV.BookName = "AMB";
                                vmstCV.EntryUser = Session["user"].ToString();
                                vmstCV.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
                                vmstCV.Status = "A";
                                vmstCV.AuthoUserType = Session["userlevel"].ToString();
                                vmstCV.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
                                vmstCV.VchRefNo = "CV-" + vmstCV.VchSysNo.ToString().PadLeft(10, '0');
                            }
                        }

                        string VCH_SYS_NO_SVTXT = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                            "t1.PAYEE='SVSLTXT' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                            txtInvoiceNo.Text);
                        VouchMst vmstTx = VouchManager.GetVouchMst(VCH_SYS_NO_SVTXT.Trim());
                        if (vmstTx != null)
                        {
                            vmstTx.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
                            vmstTx.ValueDate = txtDate.Text;
                            vmstTx.VchCode = "03";
                            vmstTx.RefFileNo = "";
                            //vmst.SerialNo = txtGRNO.Text.Trim();
                            vmstTx.Particulars = "Total Vat on " + txtRemarks.Text;
                            vmstTx.ControlAmt = txtVat.Text.Replace(",", "");
                            //vmst.Payee = "SVTXT";
                            vmstTx.CheqAmnt = "0";
                            vmstTx.UpdateUser = Session["user"].ToString().ToUpper();
                            vmstTx.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                            vmstTx.AuthoUserType = Session["userlevel"].ToString();
                        }
                        else
                        {
                            vmstTx = new VouchMst();
                            vmstTx.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
                            vmstTx.ValueDate = txtDate.Text;
                            vmstTx.VchCode = "03";
                            vmstTx.RefFileNo = "New";
                            vmstTx.VolumeNo = "";
                            vmstTx.SerialNo = txtInvoiceNo.Text.Trim();
                            vmstTx.Particulars = "Total Vat on " + txtRemarks.Text;
                            vmstTx.ControlAmt = txtVat.Text.Replace(",", "");
                            vmstTx.Payee = "SVSLTXT";
                            vmstTx.CheckNo = "";
                            vmstTx.CheqDate = DateTime.Now.ToString("dd/MM/yyyy");
                            vmstTx.CheqAmnt = "0";
                            vmstTx.MoneyRptNo = "";
                            vmstTx.MoneyRptDate = "";
                            vmstTx.TransType = "R";
                            vmstTx.BookName = "AMB";
                            vmstTx.EntryUser = Session["user"].ToString();
                            vmstTx.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
                            vmstTx.Status = "A";
                            vmstTx.AuthoUserType = Session["userlevel"].ToString();
                            vmstTx.VchSysNo = (Convert.ToInt64(IdManager.GetNextID("gl_trans_mst", "vch_sys_no")) + 2)
                                .ToString();
                            vmstTx.VchRefNo = "JV-" + vmstTx.VchSysNo.ToString().PadLeft(10, '0');
                        }

                        DataTable dtOldSalesDetails = (DataTable)ViewState["OldSV"];
                        SalesManager.UpdateBranchSalesInfo(aSales, dtSalesDetails, dtOldSalesDetails, dtPayment, vmst, vmstCV, vmstTx);
                        // PosPrint();
                        btnNew_Click(sender, e);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('You are not Permitted this Step...!!','red',0);", true);
                        return;
                    }
                }
                else
                {
                    if (ViewState["SV"] == null)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('No Items In this list...!!','INDIANRED',3);", true);
                        return;
                    }

                    else
                    {
                        if (per.AllowAdd == "Y")
                        {
                            int CountInv = IdManager.GetShowSingleValueInt("[InvoiceNo]",
                                "[Order] where UPPER([InvoiceNo])='" + txtInvoiceNo.Text.ToUpper() + "' ");
                            if (CountInv > 0)
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('This invoice already exist in database.!!','INDIANRED',3);", true);

                                return;
                            }

                            aSales = new Sales();
                            txtInvoiceNo.Text = "INV-RD-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() +
                                                DateTime.Now.Day.ToString() + "-00" +
                                                IdManager.GetShowSingleValueString("InvoiceID", "FixGlCoaCode") + Session["BranchId"].ToString();
                            aSales.BranchId = Session["BranchId"].ToString();
                            aSales.Invoice = txtInvoiceNo.Text.ToUpper();
                            aSales.Date = txtDate.Text;
                            aSales.Total = txtSubTotal.Text.Replace(",", "");
                            aSales.Tax = txtVat.Text.Replace(",", "");
                            aSales.Disount = txtDiscount.Text.Replace(",", "");
                            aSales.GTotal = txtGrandTotal.Text.Replace(",", "");



                            var RefundOrDue = Convert.ToDecimal(txtDue.Text);


                            decimal due, refund;
                            if (RefundOrDue < 0)
                            {
                                due = 0;
                                refund = (-1) * RefundOrDue;

                            }
                            else
                            {
                                due = RefundOrDue;
                                refund = 0;
                            }

                            var ExtraAmount = Convert.ToDecimal(txtLastFigarAmount.Text) -
                                              Convert.ToDecimal(txtPayment.Text);
                            aSales.ExtraAmount = "0";

                            aSales.CReceive = txtPayment.Text.Replace(",", "");


                            aSales.Due = due.ToString("N2");

                            aSales.CRefund = refund.ToString("N2");





                            aSales.paymentID = ddlPaymentTypeTo.SelectedValue;

                            aSales.Customer = hfCustomerID.Value;
                            aSales.OrderType = "C";
                            //  aSales.GuarantorID = lblguarantorID.Text;
                            aSales.DvStatus = ddlDelevery.SelectedValue;
                            aSales.DvDate = txtDate.Text;
                            // aSales.TotalInstallment = txtInstallmentNumber.Text;
                            // aSales.installmentDate = txtpaydate.Text;

                            //aSales.PMethod = ddlPaymentTypeFrom.SelectedValue;
                            //aSales.BankId = ddlBankNameFrom.SelectedValue;
                            if (string.IsNullOrEmpty(txtCustomerName.Text))
                            {
                                DataTable dtCustomerDtl = _aclsClientInfoManager.GetCustomerOnSearch("", 5);
                                if (dtCustomerDtl != null)
                                {
                                    txtCustomerName.Text = dtCustomerDtl.Rows[0]["ContactName"].ToString();
                                    txtRemarks.Text = "Items Sales By Customer : " + txtCustomerName.Text +
                                                      " for Local Customer (" + txtLocalCustomer.Text + ")";
                                    hfCustomerCoa.Value = dtCustomerDtl.Rows[0]["Gl_CoaCode"].ToString();
                                    hfCustomerID.Value = dtCustomerDtl.Rows[0]["ID"].ToString();
                                }
                            }
                            aSales.Remarks = txtRemarks.Text;
                            aSales.Customer = hfCustomerID.Value;
                            aSales.CustomerCoa = hfCustomerCoa.Value;
                            aSales.LocalCustomer = txtLocalCustomer.Text.Replace("'", "");
                            aSales.LocalCusPhone = txtLocalCusPhone.Text.Replace("'", "");
                            aSales.LocalCusAddress = txtLocalCusAddress.Text.Replace("'", "");
                            aSales.Note = txtNote.Text;
                            aSales.BankCoaCode = "";
                            //if (ddlBankName.SelectedValue != null && ddlBankName.SelectedValue != "" &&
                            //    ddlBankName.SelectedValue != "0")
                            //{
                            //    aSales.BankCoaCode = IdManager.GetShowSingleValueString("Gl_Code", "ID",
                            //        "dbo.bank_branch", ddlBankName.SelectedValue);
                            //}
                            //else
                            //{
                            //    aSales.BankCoaCode = "";

                            //}
                            aSales.CustomerName = txtCustomerName.Text;

                            //aSales.PMethod = ddlPaymentTypeTo.SelectedValue;
                            //if (ddlPaymentTypeTo.SelectedValue != "1")
                            //{
                            //    aSales.BankId = ddlBankName.SelectedValue;
                            //    aSales.PMNumber = ddlAccountNo.SelectedValue;
                            //}
                            aSales.LoginBy = Session["userID"].ToString();

                            //*************************** Account Entry ******************//
                            //********* Jurnal Voucher *********//
                            VouchMst vmst = new VouchMst();
                            vmst.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
                            vmst.ValueDate = txtDate.Text;
                            vmst.VchCode = "03";
                            vmst.RefFileNo = "";
                            vmst.VolumeNo = "";
                            vmst.SerialNo = txtInvoiceNo.Text.Trim();
                            vmst.Particulars = "Account payable To " + txtCustomerName.Text + " , " + txtRemarks.Text;
                            vmst.ControlAmt = txtSubTotal.Text.Replace(",", "");
                            vmst.Payee = "SVSLJV";
                            vmst.CheckNo = "";
                            vmst.CheqDate = "";
                            vmst.CheqAmnt = "0";
                            vmst.MoneyRptNo = "";
                            vmst.MoneyRptDate = "";
                            vmst.TransType = "R";
                            vmst.BookName = "AMB";
                            vmst.EntryUser = Session["user"].ToString();
                            vmst.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
                            vmst.Status = "A";
                            vmst.AuthoUserType = Session["userlevel"].ToString();
                            vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
                            vmst.VchRefNo = "JV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
                            //********* Credit Voucher *********//
                            VouchMst vmstCR = new VouchMst();
                            vmstCR.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
                            vmstCR.ValueDate = txtDate.Text;
                            vmstCR.VchCode = "02";
                            vmstCR.RefFileNo = "";
                            vmstCR.VolumeNo = "";
                            vmstCR.SerialNo = txtInvoiceNo.Text.Trim();
                            vmstCR.Particulars = txtRemarks.Text;
                            vmstCR.ControlAmt = aSales.GTotal.Replace(",", "");
                            vmstCR.Payee = "SVSLCV";
                            vmstCR.CheckNo = "";
                            vmstCR.CheqDate = "";
                            vmstCR.CheqAmnt = "0";
                            vmstCR.MoneyRptNo = "";
                            vmstCR.MoneyRptDate = "";
                            vmstCR.TransType = "R";
                            vmstCR.BookName = "AMB";
                            vmstCR.EntryUser = Session["user"].ToString();
                            vmstCR.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
                            vmstCR.Status = "A";
                            vmstCR.AuthoUserType = Session["userlevel"].ToString();
                            vmstCR.VchSysNo = (Convert.ToInt64(IdManager.GetNextID("gl_trans_mst", "vch_sys_no")) + 1)
                                .ToString();
                            vmstCR.VchRefNo = "CV-" + vmstCR.VchSysNo.ToString().PadLeft(10, '0');

                            //********* Tax *********//
                            VouchMst vmstTax = new VouchMst();
                            vmstTax.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
                            vmstTax.ValueDate = txtDate.Text;
                            vmstTax.VchCode = "03";
                            vmstTax.RefFileNo = "";
                            vmstTax.VolumeNo = "";
                            vmstTax.SerialNo = txtInvoiceNo.Text.Trim();
                            vmstTax.Particulars = "Total Vat on " + txtRemarks.Text;
                            vmstTax.ControlAmt = txtVat.Text.Replace(",", "");
                            vmstTax.Payee = "SVSLTXT";
                            vmstTax.CheckNo = "";
                            vmstTax.CheqDate = "";
                            vmstTax.CheqAmnt = "0";
                            vmstTax.MoneyRptNo = "";
                            vmstTax.MoneyRptDate = "";
                            vmstTax.TransType = "R";
                            vmstTax.BookName = "AMB";
                            vmstTax.EntryUser = Session["user"].ToString();
                            vmstTax.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
                            vmstTax.Status = "A";
                            vmstTax.AuthoUserType = Session["userlevel"].ToString();
                            vmstTax.VchSysNo = (Convert.ToInt64(IdManager.GetNextID("gl_trans_mst", "vch_sys_no")) + 2)
                                .ToString();
                            vmstTax.VchRefNo = "JV-" + vmstTax.VchSysNo.ToString().PadLeft(10, '0');

                            int SalesID = SalesManager.SaveSalesInfo(aSales, dtSalesDetails, dtPayment, vmst, vmstCR, vmstTax);
                            lblInvNo.Text = SalesID.ToString();

                            //PosPrint();
                            btnNew_Click(sender, e);
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('You are not Permitted this Step...!!','red',2);", true);

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
    public void Refresh()
    {
        //rdbPrintOption.SelectedValue = "Pr";
        hfCustomerCoa.Value = "";
        hfCustomerID.Value = "";
        txtCustomerName.Text = "";
        txtNote.Text = "";
        txtDate.Text = "";
        txtDeleveryDate.Text = "";
        txtNote.Text = "";
        txtRemarks.Text = "";
        txtItemsCode.Text = "";
        txtInvoiceNo.Text = "";

        Clear();
        txtSubTotal.Text = "";
        txtVat.Text = "";
        txtDiscount.Text = "";
        txtGrandTotal.Text = "";
        txtPayment.Text = "";
        txtDue.Text = "";
        lblAmount.Text = "";
        lblInvNo.Text = "";
        lblPreviousDue.Text = "";
        //lblChekNo.Text = "";
        lblPreviousDue.Text = "";
        txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        txtInvoiceNo.Text = "INV-RD-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() +
                            DateTime.Now.Day.ToString() + "-00" +
                            IdManager.GetShowSingleValueString("InvoiceID", "FixGlCoaCode") + Session["BranchId"].ToString();

        Panel1.Visible = txtItemsCode.Enabled = btnNew.Visible = false;
        dgSVMst.Visible = true;

        string query2 =
            "select '' [bank_id],'' [bank_name]  union select convert(nvarchar,[ID]) ,BankBranch_AccName AS [bank_name] from [View_Bank_Branch_Info] order by 1";
        //util.PopulationDropDownList(ddlBankNameFrom, "bank_info", query2, "bank_name", "bank_id");

        dgSVMst.DataSource = SalesManager.GetShowSalesDetails();
        dgSVMst.DataBind();
        Clear();
        txtRemarks.Text = "Item Sales.";
        btnSave.Visible = true;

    }

    public DataTable PopulateMeasure()
    {
        dtmsr = ItemManager.GetMeasure();
        DataRow dr = dtmsr.NewRow();
        dtmsr.Rows.InsertAt(dr, 0);
        return dtmsr;
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        //Response.Redirect("SalesVoucher.aspx?mno=5.20");
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);

    }

    protected void dgSVMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header |
                e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[6].Attributes.Add("style", "display:none");
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

    protected void dgSVMst_SelectedIndexChanged(object sender, EventArgs e)
    {

        try
        {
            lblInvNo.Text = dgSVMst.SelectedRow.Cells[6].Text.Trim();
            btnFind_Click(sender, e);
            btnNew.Visible = false;
            btnSave.Visible = true;
            // UpItemsDetails.Update();
            // UPPaymentMtd.Update();
            //UpSearch.Update();
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

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            if (per.AllowDelete == "Y")
            {
                // DataTable dtItemsDetails = (DataTable) ViewState["SV"];
                DataTable dtOldSalesDetails = (DataTable)ViewState["OldSV"];

                // DataTable dtinstallment = (DataTable)ViewState["installment"];
                string VCH_SYS_NO_SVJV = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                    "t1.PAYEE='SVSLJV' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                    txtInvoiceNo.Text);
                
                string VCH_SYS_NO_SVCV = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                    "t1.PAYEE='SVSLCV' and SUBSTRING(t1.VCH_REF_NO,1,2)='CV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                    txtInvoiceNo.Text);

                Sales aSales = SalesManager.GetBranchShowSalesInfo(lblInvNo.Text,Session["BranchId"].ToString());
                if (aSales != null)
                {
                    aSales.ID = lblInvNo.Text;
                    aSales.Invoice = txtInvoiceNo.Text;
                    SalesManager.DeleteSalesVoucher(aSales, dtOldSalesDetails);
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Record(s) is/are Delete suceessfullly!!','green',1);", true);

                    btnDelete.Enabled = false;
                    btnSave.Enabled = false;
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('You are not Permitted this Step...!!','red',0);", true);

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

    protected void dgSVMst_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgSVMst.DataSource = Session["SvMst"];
        dgSVMst.PageIndex = e.NewPageIndex;
        dgSVMst.DataBind();
    }


    public void VisiblePayment(bool lblBank, bool Bank, bool lblChkNo, bool ChkNo, bool lblChkDate, bool chkdate, bool lblChkStatus, bool chkStatus)
    {

        // lblBankNameFrom.Visible = lblBank;
        //ddlBankNameFrom.Visible = Bank;
        // lblChekNo.Visible = lblChkNo;
        // txtcheeckNo.Visible = ChkNo;
        // lblApprovedDate.Visible = lblChkDate;
        // txtApprovedDate.Visible = chkdate;
        // lblStatus.Visible = lblChkStatus;
        // ddlPaymentStatus.Visible = chkStatus;
        // ddlBankNameFrom.SelectedIndex = -1;
        // txtApprovedDate.Text = txtcheeckNo.Text = "";
        txtChequeAmount.Text = "0";
        txtChequeAmount.Focus();
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {

        // string embed = "<object data=\"{0}{1}\" type=\"application/pdf\" width=\"800px\" height=\"500px\">";
        // embed += "If you are unable to view file, you can download from <a href = \"{0}{1}&download=1\">here</a>";
        // embed += " or download <a target = \"_blank\" href = \"http://get.adobe.com/reader/\">Adobe PDF Reader</a> to view the file.";
        // embed += "</object>";

        //Session["InvoiceNo"]=txtInvoiceNo.Text;
        //Session["ID"]=lblInvNo.Text;
        // Session["CustomerName"]=ddlCustomer.SelectedItem.Text;
        // Session["CustomerID"]=ddlCustomer.SelectedValue;
        // Session["totPayment"]=txtPayment.Text;
        // Session["totDue"]=txtDue.Text;
        // Session["InvoiceDate"]=txtDate.Text;



        //ltEmbed.Text = string.Format(embed, ResolveUrl("~/PrintPreview/Handler.ashx?Id="), "");
        //     ModalPopupExtenderLogin.Show();


        //SalesPrintReport();
        SalesPrintReportnew();
    }

    private BarcodeLib.Barcode barcode = new BarcodeLib.Barcode();

    private void SalesPrintReport()
    {
        DataTable dtCustomerDtl = null;
        if (string.IsNullOrEmpty(txtCustomerName.Text))
        {
            dtCustomerDtl = _aclsClientInfoManager.GetCustomerOnSearch("", 5);
        }
        else
        {
            dtCustomerDtl = _aclsClientInfoManager.GetCustomerOnSearch(" where ID=" + hfCustomerID.Value, 1);
        }

        string filename = txtInvoiceNo.Text;
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
        Document document = new Document(PageSize.A4, 30f, 30f, 30f, 30f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        iTextSharp.text.Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20,
            writer.DirectContent);
        PdfPCell cell;

        //********** Barcode ***********//
        barcode.Alignment = BarcodeLib.AlignmentPositions.CENTER;
        int W = 800;
        int H = 130;

        BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128;
        barcode.IncludeLabel = false;
        barcode.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
        barcode.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
        System.Drawing.Image generatedBarcode = barcode.Encode(type, txtInvoiceNo.Text, Color.Black, Color.White, W, H);
        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        generatedBarcode.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

        byte[] logo2 = stream.ToArray();
        iTextSharp.text.Image gif2 = iTextSharp.text.Image.GetInstance(logo2);
        gif2.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif2.ScalePercent(25f);
        //******************************//
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(35f);

        float[] titwidth = new float[3] { 10, 200, 10 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        // cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        // cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.BOLD)));
        cell = new PdfPCell(gif2);
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        // cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        //cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 15f;
        dth.AddCell(cell);

        cell = new PdfPCell(new Phrase("INVOICE / BILL",
            FontFactory.GetFont(FontFactory.TIMES_BOLD, 11, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        document.Add(dth);

        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] titW = new float[2] { 80, 60 };
        PdfPTable pdtm = new PdfPTable(titW);
        pdtm.WidthPercentage = 100;

        PdfPTable pdtclient = new PdfPTable(4);
        pdtclient.WidthPercentage = 100;
        PdfPTable pdtpur = new PdfPTable(2);
        pdtpur.WidthPercentage = 100;
        if (string.IsNullOrEmpty(txtLocalCustomer.Text))
        {
            cell = new PdfPCell(FormatHeaderPhrase(dtCustomerDtl.Rows[0]["ContactName"].ToString()));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dtCustomerDtl.Rows[0]["Address1"].ToString()));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dtCustomerDtl.Rows[0]["Address2"].ToString()));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
        }
        else
        {
            cell = new PdfPCell(FormatHeaderPhrase(txtLocalCustomer.Text));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(txtLocalCusPhone.Text));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(txtLocalCusAddress.Text));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);

        }

        cell = new PdfPCell(FormatPhrase("Invoice No : " + txtInvoiceNo.Text));
        cell.BorderWidth = 0f;
        cell.HorizontalAlignment = 2;
        cell.Colspan = 2;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Invoice Date : " + txtDate.Text));
        cell.BorderWidth = 0f;
        cell.HorizontalAlignment = 2;
        cell.Colspan = 2;
        pdtpur.AddCell(cell);
        string BillingBy = IdManager.GetShowSingleValueString("t2.[DESCRIPTION]",
            "[dbo].[Order] t1 inner join UTL_USERINFO t2 on t2.[USER_NAME]=t1.CreatedBy where t1.ID='" + lblInvNo.Text +
            "' ");
        cell = new PdfPCell(FormatPhrase("Billing By : " + BillingBy));
        cell.BorderWidth = 0f;
        cell.HorizontalAlignment = 2;
        cell.Colspan = 2;
        pdtpur.AddCell(cell);

        cell = new PdfPCell(pdtclient);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);

        cell = new PdfPCell(pdtpur);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);
        document.Add(pdtm);

        PdfPTable dtempty1 = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 15f;
        dtempty1.AddCell(cell);
        document.Add(dtempty1);


        float[] widthdtl = new float[5] { 15, 60, 20, 20, 20 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;

        if (!string.IsNullOrEmpty(txtNote.Text))
        {
            cell = new PdfPCell(FormatHeaderPhrase(txtNote.Text));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 35f;
            cell.Border = 0;
            cell.Colspan = 5;
            pdtdtl.AddCell(cell);
        }

        cell = new PdfPCell(FormatHeaderPhrase("SN"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Product Description"));
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
        cell = new PdfPCell(FormatHeaderPhrase("Unit Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Total Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        DataTable DT1 = SalesManager.GetSalesDetails(lblInvNo.Text);
        int Serial = 1;
        decimal totQty = 0;
        decimal tot = 0;
        foreach (DataRow dr in DT1.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;


            if (Convert.ToInt32(dr["WarrantyYear"]) > 0 & Convert.ToInt32(dr["WarrantyMonth"]) == 0)
            {
                cell = new PdfPCell(FormatPhrase(" " + dr["ItemsName"].ToString() + " - " +
                                                 dr["BrandName"].ToString() +
                                                 "\n Style/Model/Serial : " + dr["ModelNo"].ToString() +
                                                 "\n Warranty : " +
                                                 dr["WarrantyYear"].ToString() +
                                                 " years.\n " + dr["Remarks"].ToString() + "\n"));
            }
            else if (Convert.ToInt32(dr["WarrantyYear"]) == 0 & Convert.ToInt32(dr["WarrantyMonth"]) > 0)
            {
                cell = new PdfPCell(FormatPhrase(" " + dr["ItemsName"].ToString() + " - " +
                                                 dr["BrandName"].ToString() +
                                                 "\n Style/Model/Serial : " + dr["ModelNo"].ToString() +
                                                 "\n Warranty : " +
                                                 dr["WarrantyMonth"].ToString() +
                                                 " month.\n " + dr["Remarks"].ToString() + "\n"));
            }
            else if (Convert.ToInt32(dr["WarrantyYear"]) == 0 & Convert.ToInt32(dr["WarrantyMonth"]) == 0)
            {
                cell = new PdfPCell(FormatPhrase(" " + dr["ItemsName"].ToString() + " - " +
                                                 dr["BrandName"].ToString() +
                                                 "\n Style/Model/Serial : " + dr["ModelNo"].ToString() + "\n " +
                                                 dr["Remarks"].ToString() + "\n"));
            }
            else
            {

                cell = new PdfPCell(FormatPhrase(" " + dr["ItemsName"].ToString() + " - " +
                                                 dr["BrandName"].ToString() +
                                                 "\n Style/Model/Serial : " + dr["ModelNo"].ToString() +
                                                 "\n Warranty : " +
                                                 dr["WarrantyYear"].ToString() +
                                                 " years & " +
                                                 dr["WarrantyMonth"].ToString() +
                                                 " month.\n " + dr["Remarks"].ToString() + "\n"));
            }

            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);



            cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["Qty"]).ToString("N0")));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["SPrice"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Total"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            tot += Convert.ToDecimal(dr["Total"]);

        }

        cell = new PdfPCell(FormatPhrase("Total"));
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(tot.ToString("N2")));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Vat(%)"));
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(txtVat.Text));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Discount"));
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(txtDiscount.Text));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Grand Total"));
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(txtGrandTotal.Text));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Paid Amount"));
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(txtPayment.Text));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Due Amount"));
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(txtDue.Text));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase("In Words : " +
                                               DataManager.GetLiteralAmt(txtGrandTotal.Text.Replace(",", ""))
                                                   .Replace("  ", "")
                                                   .Replace(",", "")));

        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Colspan = 5;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell, txtLocalCustomer);

        iTextSharp.text.Rectangle page1 = document.PageSize;
        float[] FootWth = new float[] { 100 };
        PdfPTable Fdth = new PdfPTable(FootWth);
        Fdth.TotalWidth = page1.Width - 10;
        Fdth.HorizontalAlignment = Element.ALIGN_CENTER;

        cell = new PdfPCell(new Phrase(
            "Goods sold once received or accepted by the customer are not returnable.Warranty will void of all products if sticker is removed.\n\n",
            FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.NORMAL)));
        cell.HorizontalAlignment = 1;
        cell.Border = 0;
        cell.VerticalAlignment = 1;
        Fdth.AddCell(cell);

        cell = new PdfPCell(new Phrase(
            "675,WEST KAZI PARA,(5th Floor),Mirpur, Dhaka-1216, Cell : +88 01989-060151, +88 01774-625474\nE-mail : dentalinebusiness@gmail.com,Facebook ID:www.facebook.com/dentalinedhaka",
            FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.NORMAL)));
        cell.BackgroundColor = new iTextSharp.text.BaseColor(Color.Gainsboro);
        cell.HorizontalAlignment = 1;
        cell.Border = 0;
        cell.FixedHeight = 35f;
        cell.VerticalAlignment = 1;
        Fdth.AddCell(cell);

        Fdth.WriteSelectedRows(0, 5, 5, 48, writer.DirectContent);
        document.Close();
        Response.Flush();
        Response.End();

    }
    private void SalesPrintReportnew()
    {
        DataTable dtCustomerDtl = null;
        if (string.IsNullOrEmpty(txtCustomerName.Text))
        {
            dtCustomerDtl = _aclsClientInfoManager.GetCustomerOnSearch("", 5);
        }
        else
        {
            dtCustomerDtl = _aclsClientInfoManager.GetCustomerOnSearch(" where ID=" + hfCustomerID.Value, 1);
        }

        string filename = txtInvoiceNo.Text;
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
        //Document document = new Document(PageSize.A4, 30f, 30f, 30f, 30f);
        //PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        Document document = new Document();
        //float pwidth = (float)(25.7 / 2.54) * 72;
        //float pheight = (float)(22.7 / 2.54) * 72; 
        float pwidth = (float)(14 / 2.54) * 72;
        float pheight = (float)(20 / 2.54) * 72;
        document = new Document(new iTextSharp.text.Rectangle(pwidth, pheight));
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        //iTextSharp.text.Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        //head.TotalWidth = page.Width - 50;
        //head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20,
        //    writer.DirectContent);
        PdfPCell cell;

        //********** Barcode ***********//
        barcode.Alignment = BarcodeLib.AlignmentPositions.CENTER;
        int W = 800;
        int H = 130;

        BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128;
        barcode.IncludeLabel = false;
        barcode.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
        barcode.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
        System.Drawing.Image generatedBarcode = barcode.Encode(type, txtInvoiceNo.Text, Color.Black, Color.White, W, H);
        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        generatedBarcode.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

        byte[] logo2 = stream.ToArray();
        iTextSharp.text.Image gif2 = iTextSharp.text.Image.GetInstance(logo2);
        gif2.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif2.ScalePercent(20f);
        //******************************//
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(35f);

        float[] titwidth = new float[3] { 20, 200, 10 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        // cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        var a = Session["org"].ToString();
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.BOLD)));
        //cell = new PdfPCell(gif2);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        // cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        //cell = new PdfPCell(FormatPhrase(""));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.BorderWidth = 0f;
        //dth.AddCell(cell);
        ////cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        //cell = new PdfPCell(FormatPhrase(""));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 15f;
        //dth.AddCell(cell);

        cell = new PdfPCell(new Phrase("INVOICE / BILL",
            FontFactory.GetFont(FontFactory.TIMES_BOLD, 9, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        document.Add(dth);

        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] titW = new float[2] { 80, 60 };
        PdfPTable pdtm = new PdfPTable(titW);
        pdtm.WidthPercentage = 100;

        PdfPTable pdtclient = new PdfPTable(4);
        pdtclient.WidthPercentage = 100;
        PdfPTable pdtpur = new PdfPTable(2);
        pdtpur.WidthPercentage = 100;
        if (string.IsNullOrEmpty(txtLocalCustomer.Text))
        {
            cell = new PdfPCell(FormatHeaderPhrase9(dtCustomerDtl.Rows[0]["ContactName"].ToString()));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase9(dtCustomerDtl.Rows[0]["Address1"].ToString()));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase9(dtCustomerDtl.Rows[0]["Address2"].ToString()));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
        }
        else
        {
            cell = new PdfPCell(FormatHeaderPhrase9(txtLocalCustomer.Text));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase9(txtLocalCusPhone.Text));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase9(txtLocalCusAddress.Text));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);

        }

        cell = new PdfPCell(FormatPhrase("Invoice No : " + txtInvoiceNo.Text));
        cell.BorderWidth = 0f;
        cell.HorizontalAlignment = 2;
        cell.Colspan = 2;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Invoice Date : " + txtDate.Text));
        cell.BorderWidth = 0f;
        cell.HorizontalAlignment = 2;
        cell.Colspan = 2;
        pdtpur.AddCell(cell);
        string BillingBy = IdManager.GetShowSingleValueString("t2.[DESCRIPTION]",
            "[dbo].[Order] t1 inner join UTL_USERINFO t2 on t2.[USER_NAME]=t1.CreatedBy where t1.ID='" + lblInvNo.Text +
            "' ");
        cell = new PdfPCell(FormatPhrase("Billing By : " + BillingBy));
        cell.BorderWidth = 0f;
        cell.HorizontalAlignment = 2;
        cell.Colspan = 2;
        pdtpur.AddCell(cell);

        cell = new PdfPCell(pdtclient);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);

        cell = new PdfPCell(pdtpur);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);
        document.Add(pdtm);

        PdfPTable dtempty1 = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 15f;
        dtempty1.AddCell(cell);
        document.Add(dtempty1);


        float[] widthdtl = new float[5] { 15, 60, 20, 20, 20 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;

        if (!string.IsNullOrEmpty(txtNote.Text))
        {
            cell = new PdfPCell(FormatHeaderPhrase(txtNote.Text));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 35f;
            cell.Border = 0;
            cell.Colspan = 5;
            pdtdtl.AddCell(cell);
        }

        cell = new PdfPCell(FormatPhrase("SN"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Product Description"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Unit Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Total Price"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        DataTable DT1 = SalesManager.GetSalesDetails(lblInvNo.Text);
        int Serial = 1;
        decimal totQty = 0;
        decimal tot = 0;
        foreach (DataRow dr in DT1.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;


            if (Convert.ToInt32(dr["WarrantyYear"]) > 0 & Convert.ToInt32(dr["WarrantyMonth"]) == 0)
            {
                cell = new PdfPCell(FormatPhrase(" " + dr["ItemsName"].ToString() + " - " +
                                                 dr["BrandName"].ToString() +
                                                 "\n Style/Model/Serial : " + dr["ModelNo"].ToString() +
                                                 "\n Warranty : " +
                                                 dr["WarrantyYear"].ToString() +
                                                 " years.\n " + dr["Remarks"].ToString() + "\n"));
            }
            else if (Convert.ToInt32(dr["WarrantyYear"]) == 0 & Convert.ToInt32(dr["WarrantyMonth"]) > 0)
            {
                cell = new PdfPCell(FormatPhrase(" " + dr["ItemsName"].ToString() + " - " +
                                                 dr["BrandName"].ToString() +
                                                 "\n Style/Model/Serial : " + dr["ModelNo"].ToString() +
                                                 "\n Warranty : " +
                                                 dr["WarrantyMonth"].ToString() +
                                                 " month.\n " + dr["Remarks"].ToString() + "\n"));
            }
            else if (Convert.ToInt32(dr["WarrantyYear"]) == 0 & Convert.ToInt32(dr["WarrantyMonth"]) == 0)
            {
                cell = new PdfPCell(FormatPhrase(" " + dr["ItemsName"].ToString() + " - " +
                                                 dr["BrandName"].ToString() +
                                                 "\n Style/Model/Serial : " + dr["ModelNo"].ToString() + "\n " +
                                                 dr["Remarks"].ToString() + "\n"));
            }
            else
            {

                cell = new PdfPCell(FormatPhrase(" " + dr["ItemsName"].ToString() + " - " +
                                                 dr["BrandName"].ToString() +
                                                 "\n Style/Model/Serial : " + dr["ModelNo"].ToString() +
                                                 "\n Warranty : " +
                                                 dr["WarrantyYear"].ToString() +
                                                 " years & " +
                                                 dr["WarrantyMonth"].ToString() +
                                                 " month.\n " + dr["Remarks"].ToString() + "\n"));
            }

            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);



            cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["Qty"]).ToString("N0")));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["SPrice"].ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(dr["Total"].ToString()));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            tot += Convert.ToDecimal(dr["Total"]);

        }

        cell = new PdfPCell(FormatPhrase("Total"));
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(tot.ToString("N2")));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Vat(%)"));
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(txtVat.Text));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Discount"));
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(txtDiscount.Text));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Grand Total"));
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(txtGrandTotal.Text));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Paid Amount"));
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(txtPayment.Text));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Due Amount"));
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(txtDue.Text));
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 18f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);


        cell = new PdfPCell(FormatHeaderPhrase("In Words : " +
                                               DataManager.GetLiteralAmt(txtGrandTotal.Text.Replace(",", ""))
                                                   .Replace("  ", "")
                                                   .Replace(",", "")));

        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Colspan = 5;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell, txtLocalCustomer);

        iTextSharp.text.Rectangle page1 = document.PageSize;
        float[] FootWth = new float[] { 100 };
        PdfPTable Fdth = new PdfPTable(FootWth);
        Fdth.TotalWidth = page1.Width - 10;
        Fdth.HorizontalAlignment = Element.ALIGN_CENTER;

        cell = new PdfPCell(new Phrase(
            "Goods sold once received or accepted by the customer are not returnable.Warranty will void of all products if sticker is removed.\n\n",
            FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL)));
        cell.HorizontalAlignment = 1;
        cell.Border = 0;
        cell.VerticalAlignment = 1;
        Fdth.AddCell(cell);

        cell = new PdfPCell(new Phrase(
            "675,WEST KAZI PARA,(5th Floor),Mirpur, Dhaka-1216, Cell : +88 01989-060151, +88 01774-625474\nE-mail : dentalinebusiness@gmail.com,Facebook ID:www.facebook.com/dentalinedhaka",
            FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.NORMAL)));
        cell.BackgroundColor = new iTextSharp.text.BaseColor(Color.Gainsboro);
        cell.HorizontalAlignment = 1;
        cell.Border = 0;
        cell.FixedHeight = 35f;
        cell.VerticalAlignment = 1;
        Fdth.AddCell(cell);

        Fdth.WriteSelectedRows(0, 5, 5, 70, writer.DirectContent);
        document.Close();
        Response.Flush();
        Response.End();

    }

    private static PdfPCell SignatureFormat(Document document, PdfPCell cell, TextBox txtLocalCustomer)
    {
        float[] widtl = new float[3] { 20, 5, 20 };
        PdfPTable pdtsig = new PdfPTable(widtl);
        pdtsig.WidthPercentage = 100;
        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.Colspan = 3;
        cell.FixedHeight = 80f;
        pdtsig.AddCell(cell);

        if (string.IsNullOrEmpty(txtLocalCustomer.Text))
        {
            cell = new PdfPCell(FormatPhrase("Received with good condition by"));
        }
        else
        {
            cell = new PdfPCell(FormatPhrase("Customer signature"));
        }

        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);



        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Authorized signature and company stamp"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Border = 0;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtsig.AddCell(cell);
        document.Add(pdtsig);
        return cell;
    }

    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9));
    }

    private static Phrase FormatHeaderTopPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }
    private static Phrase FormatHeaderPhrase9(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD));
    }
    protected void btnChallan_Click(object sender, EventArgs e)
    {
        DataTable dtCustomerDtl = null;
        if (string.IsNullOrEmpty(txtCustomerName.Text))
        {
            dtCustomerDtl = _aclsClientInfoManager.GetCustomerOnSearch("", 5);
        }
        else
        {
            dtCustomerDtl = _aclsClientInfoManager.GetCustomerOnSearch(" where ID=" + hfCustomerID.Value, 1);
        }

        string filename = txtInvoiceNo.Text;
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
        Document document = new Document(PageSize.A4, 50f, 50f, 40f, 40f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        iTextSharp.text.Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width - 50;
        //  Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        // PdfPCell c = new PdfPCell("");
        //c.Border = Rectangle.NO_BORDER;
        //c.VerticalAlignment = Element.ALIGN_BOTTOM;
        //c.HorizontalAlignment = Element.ALIGN_RIGHT;
        //head.AddCell(c);
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20,
            writer.DirectContent);

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(5f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;

        cell = new PdfPCell(gif);
        //cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        // cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.BOLD)));
        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        // cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        //cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell = new PdfPCell(FormatPhrase(""));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 15f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase("CHALLAN",
            FontFactory.GetFont(FontFactory.TIMES_BOLD, 11, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        //cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        document.Add(dth);

        LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);

        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 15f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] titW = new float[2] { 80, 60 };
        PdfPTable pdtm = new PdfPTable(titW);
        pdtm.WidthPercentage = 100;

        PdfPTable pdtclient = new PdfPTable(4);
        pdtclient.WidthPercentage = 100;
        PdfPTable pdtpur = new PdfPTable(2);
        pdtpur.WidthPercentage = 100;
        if (string.IsNullOrEmpty(txtLocalCustomer.Text))
        {
            cell = new PdfPCell(FormatHeaderPhrase(dtCustomerDtl.Rows[0]["ContactName"].ToString()));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dtCustomerDtl.Rows[0]["Address1"].ToString()));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dtCustomerDtl.Rows[0]["Address2"].ToString()));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
        }
        else
        {
            cell = new PdfPCell(FormatHeaderPhrase(txtLocalCustomer.Text));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(txtLocalCusPhone.Text));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(txtLocalCusAddress.Text));
            cell.BorderWidth = 0f;
            cell.Colspan = 4;
            pdtclient.AddCell(cell);

        }

        cell = new PdfPCell(FormatPhrase("Challan No : " + txtInvoiceNo.Text.Replace("INV", "CLN")));
        cell.BorderWidth = 0f;
        cell.HorizontalAlignment = 2;
        cell.Colspan = 2;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Challan Date : " + txtDate.Text));
        cell.BorderWidth = 0f;
        cell.HorizontalAlignment = 2;
        cell.Colspan = 2;
        pdtpur.AddCell(cell);
        string BillingBy = IdManager.GetShowSingleValueString("t2.[DESCRIPTION]",
            "[dbo].[Order] t1 inner join UTL_USERINFO t2 on t2.[USER_NAME]=t1.CreatedBy where t1.ID='" + lblInvNo.Text +
            "' ");
        cell = new PdfPCell(FormatPhrase("Billing By : " + BillingBy));
        cell.BorderWidth = 0f;
        cell.HorizontalAlignment = 2;
        cell.Colspan = 2;
        pdtpur.AddCell(cell);

        cell = new PdfPCell(pdtclient);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);

        cell = new PdfPCell(pdtpur);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);
        document.Add(pdtm);

        PdfPTable dtempty1 = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 15f;
        dtempty1.AddCell(cell);
        document.Add(dtempty1);


        float[] widthdtl = new float[3] { 15, 60, 20 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;

        if (!string.IsNullOrEmpty(txtNote.Text))
        {
            cell = new PdfPCell(FormatHeaderPhrase(txtNote.Text));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 35f;
            cell.Border = 0;
            cell.Colspan = 5;
            pdtdtl.AddCell(cell);
        }

        cell = new PdfPCell(FormatHeaderPhrase("SN"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Product Description"));
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

        DataTable DT1 = SalesManager.GetSalesDetails(lblInvNo.Text);
        int Serial = 1;
        decimal totQty = 0;
        decimal tot = 0;
        foreach (DataRow dr in DT1.Rows)
        {
            cell = new PdfPCell(FormatPhrase(Serial.ToString()));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);
            Serial++;

            if (Convert.ToInt32(dr["WarrantyYear"]) > 0 & Convert.ToInt32(dr["WarrantyMonth"]) == 0)
            {
                cell = new PdfPCell(FormatPhrase(" " + dr["txtItems"].ToString() + " - " +
                                                 dr["BrandName"].ToString() +
                                                 "\n Model : " + dr["ModelNo"].ToString() +
                                                 "\n Warranty : " +
                                                 dr["WarrantyYear"].ToString() +
                                                 " years.\n " + dr["Remarks"].ToString() + "\n"));
            }
            else if (Convert.ToInt32(dr["WarrantyYear"]) == 0 & Convert.ToInt32(dr["WarrantyMonth"]) > 0)
            {
                cell = new PdfPCell(FormatPhrase(" " + dr["txtItems"].ToString() + " - " +
                                                 dr["BrandName"].ToString() +
                                                 "\n Model : " + dr["ModelNo"].ToString() +
                                                 "\n Warranty : " +
                                                 dr["WarrantyMonth"].ToString() +
                                                 " month.\n " + dr["Remarks"].ToString() + "\n"));
            }
            else if (Convert.ToInt32(dr["WarrantyYear"]) == 0 & Convert.ToInt32(dr["WarrantyMonth"]) == 0)
            {
                cell = new PdfPCell(FormatPhrase(" " + dr["txtItems"].ToString() + " - " +
                                                 dr["BrandName"].ToString() +
                                                 "\n Model : " + dr["ModelNo"].ToString() +
                                                 "\n " + dr["Remarks"].ToString() + "\n"));
            }
            else
            {

                cell = new PdfPCell(FormatPhrase(" " + dr["txtItems"].ToString() + " - " +
                                                 dr["BrandName"].ToString() +
                                                 "\n Model : " + dr["ModelNo"].ToString() +
                                                 "\n Warranty : " +
                                                 dr["WarrantyYear"].ToString() +
                                                 " years & " +
                                                 dr["WarrantyMonth"].ToString() +
                                                 " month.\n " + dr["Remarks"].ToString() + "\n"));
            }

            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["Qty"]).ToString("N0")));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtdtl.AddCell(cell);

        }




        //cell = new PdfPCell(FormatPhrase(dr["SPrice"].ToString()));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtdtl.AddCell(cell);

        //cell = new PdfPCell(FormatPhrase(dr["Total"].ToString()));
        //cell.HorizontalAlignment = 2;
        //cell.VerticalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //pdtdtl.AddCell(cell);
        //tot += Convert.ToDecimal(dr["Total"]);



        document.Add(pdtdtl);
        cell = SignatureFormat(document, cell, txtLocalCustomer);

        iTextSharp.text.Rectangle page1 = document.PageSize;
        float[] FootWth = new float[] { 100 };
        PdfPTable Fdth = new PdfPTable(FootWth);
        Fdth.TotalWidth = page1.Width - 10;
        Fdth.HorizontalAlignment = Element.ALIGN_CENTER;

        cell = new PdfPCell(new Phrase(
            "Goods sold once received or accepted by the customer are not returnable.Warranty will void of all products if sticker is removed.\n\n",
            FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.NORMAL)));
        cell.HorizontalAlignment = 1;
        cell.Border = 0;
        cell.VerticalAlignment = 1;
        Fdth.AddCell(cell);

        cell = new PdfPCell(new Phrase(
            "",
            FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.NORMAL)));
        cell.BackgroundColor = new iTextSharp.text.BaseColor(Color.Gainsboro);
        cell.HorizontalAlignment = 1;
        cell.Border = 0;
        cell.FixedHeight = 35f;
        cell.VerticalAlignment = 1;
        Fdth.AddCell(cell);

        Fdth.WriteSelectedRows(0, 5, 5, 48, writer.DirectContent);


        document.Close();
        Response.Flush();
        Response.End();
    }
    protected void dgSV_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void btnPosPrint_Click(object sender, EventArgs e)
    {
        try
        {
            // PosPrint();
            SalesManager.getSavePrintID(lblInvNo.Text);
            //ProcessStartInfo info = new ProcessStartInfo();
            //info.FileName = ConfigurationManager.AppSettings["OpenLocation"]; //@"C:\Program Files\Default Company Name\Setup1\WindowsFormsApplication1.exe";
            //info.Arguments = "";
            //info.WindowStyle = ProcessWindowStyle.Normal;
            //Process pro = Process.Start(info);
            //pro.WaitForExit();
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
        //SalesPrintReport();

        //var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        //Response.Redirect(pageName);


    }

    public void PosPrint()
    {
        //  80mm Series Printer
        System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
        string pp = ConfigurationManager.AppSettings["PrinterDriver"];
        printerSettings.PrinterName = ConfigurationManager.AppSettings["PrinterDriver"]; // "RONGTA RP80 Printer"; RONGTA RP80 Printer
        // printerSettings.PrinterName = "80mm Series Printer";
        System.Drawing.Printing.PageSettings pSettings = new System.Drawing.Printing.PageSettings(printerSettings);

        PrintDocument printDocument = new PrintDocument();
        printDocument.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);
        printDocument.Print();



    }




    private void pdoc_PrintPage(object sender, PrintPageEventArgs e)
    {



        //flag = "Y";

        Graphics graphics = e.Graphics;

        System.Drawing.Font fontSize4 = new System.Drawing.Font("Courier New", 4);
        System.Drawing.Font fontSize5 = new System.Drawing.Font("Courier New", 5);
        System.Drawing.Font fontSize5Bold = new System.Drawing.Font("Courier New", 5, FontStyle.Bold);
        System.Drawing.Font fontSize6 = new System.Drawing.Font("Courier New", 6);
        System.Drawing.Font fontSize7 = new System.Drawing.Font("Courier New", 7);
        System.Drawing.Font fontSize8 = new System.Drawing.Font("Courier New", 8, FontStyle.Regular);
        System.Drawing.Font fontSize99 = new System.Drawing.Font("Courier New", 9, FontStyle.Regular);
        System.Drawing.Font fontSize9 = new System.Drawing.Font("Courier New", 9, FontStyle.Bold);
        System.Drawing.Font fontSize7Bold = new System.Drawing.Font("Courier New", 7, FontStyle.Bold);
        System.Drawing.Font fontSize8Bold = new System.Drawing.Font("Courier New", 8, FontStyle.Bold);

        float fontHeight = fontSize7.GetHeight();
        int startX = 5, startY = 5, offset = 20;

        //Company company = new Company();



        //DataTable dtorderForMoneyReceipt = IdManager.GetShowDataTable("select TOP(1) '16.03.2020' as OrderDate,'03.30' as Time,'131' as InvoiceNo,'ABCD' AS ContactName,'ABD' AS ContactName,'154' AS SubTotal ,'800' AS DiscountAmount,'40' AS vatRate,'40' AS TaxAmount,'40' AS GrandTotal,'40' AS PaymentMode,'7000' AS CashReceived,'140' AS CashRefund,'41' AS GrandTotal, 'SAM' AS ServiceBy,'100' AS DiscountAmount  from dbo.CLIENT_INFO");

        //graphics.DrawImage(;company.Name, new Font("Courier New", 14, FontStyle.Bold),
        //                    new SolidBrush(Color.Black), startX + 50, startY + offset);






        byte[] image = GlBookManager.GetGlLogo(Session["book"].ToString());
        MemoryStream ms = new MemoryStream(image);


        //Image returnImage = Image.FromStream(ms);//Exception occurs here
        //e.Graphics.DrawImage(returnImage, 90, 5, 98, 38);
        DataTable companyInfo = IdManager.GetShowDataTable("select * from dbo.GL_SET_OF_BOOKS");
        // company.VatGroupNo = "41";
        //graphics.DrawString("[Vat:" + companyInfo.Rows[0]["Tax_no"].ToString() + "]", fontSize6,
        //                  new SolidBrush(Color.Black), startX + 200, startY + offset);





        offset = offset + 10;



        graphics.DrawString(companyInfo.Rows[0]["BOOK_DESC"].ToString(),
                 fontSize9,
                 new SolidBrush(Color.Black), startX + 40, startY + offset);




        offset = offset + 18;
        //offset = BreakAmountInWorkToNewLine(graphics, fontSize7, startX, startY, offset, company.Address1, 42);
        graphics.DrawString(companyInfo.Rows[0]["COMPANY_ADDRESS1"].ToString(), fontSize8,
                            new SolidBrush(Color.Black), startX, startY + offset);
        offset = offset + 10;

        graphics.DrawString(companyInfo.Rows[0]["COMPANY_ADDRESS2"].ToString(), fontSize8,
                          new SolidBrush(Color.Black), startX + 60, startY + offset);
        offset = offset + 10;
        //graphics.DrawString(company.Address2 , fontSize8,
        //                    new SolidBrush(Color.Black), startX + 40, startY + offset);
        //offset = offset + 15;
        // company.Phone = "01871741441";
        graphics.DrawString("Phone : " + companyInfo.Rows[0]["PHONE"].ToString(), fontSize8,
                            new SolidBrush(Color.Black), startX, startY + offset);
        offset = offset + 14;
        graphics.DrawString("E-mail : ridersdna.bd@gmail.com", fontSize8,
            new SolidBrush(Color.Black), startX, startY + offset);
        //offset = offset + 15;

        // offset = offset + 10;
        //// company.Mobile = "1445214";
        // graphics.DrawString("Phone-2: " + "", fontSize8,
        //                     new SolidBrush(Color.Black), startX , startY + offset);
        // offset = offset + 15;

        //graphics.DrawString("" + company.VATRegistration, fontSize7,
        //                    new SolidBrush(Color.Black), startX + 40, startY + offset);
        //offset = offset + 15;
        //  company.VATRegistration = "45142143";

        //graphics.DrawString("VAT Registration # " + companyInfo.Rows[0]["Tax_no"].ToString(), fontSize7,
        //                    new SolidBrush(Color.Black), startX, startY + offset);
        offset = offset + 15;

        graphics.DrawString("User # " + Session["user"].ToString(), fontSize7,
                            new SolidBrush(Color.Black), startX, startY + offset);


        offset = offset + 10;
        graphics.DrawLine(new Pen(Color.Black, Convert.ToSingle(0.5)), startX, startY + offset, startX + 262, startY + offset);
        offset = offset + 12;
        //DateTime orderDateTime = Convert.ToDateTime(companyInfo.Rows[0]["OrderDate"].ToString());
        //graphics.DrawString("Sales Date : " + orderDateTime.ToString("dd-MMM-yyyy") ,
        //         fontSize8,
        //         new SolidBrush(Color.Black), startX, startY + offset);
        //offset = offset + 10;




        //   var orderDateTime = dtorderForMoneyReceipt.Rows[0]["OrderDate"].ToString();

        graphics.DrawString("Sales Date : " + txtDate.Text,
                 fontSize8,
                 new SolidBrush(Color.Black), startX, startY + offset);
        offset = offset + 10;



        graphics.DrawString("Invoice No : " + txtInvoiceNo.Text,
                 fontSize9,
                 new SolidBrush(Color.Black), startX, startY + offset);

        //if (!dtorderForMoneyReceipt.Rows[0]["ContactName"].ToString().Equals(string.Empty))
        //{
        offset = offset + 15;
        graphics.DrawString("Customer : " + txtCustomerName.Text,
                 fontSize8,
                 new SolidBrush(Color.Black), startX, startY + offset);
        //}

        Pen pen = new Pen(Color.Black, Convert.ToSingle(0.5));
        pen.DashPattern = new float[] { 2f, 4f };

        offset = offset + 15;
        graphics.DrawLine(pen, startX, startY + offset, startX + 263, startY + offset);

        offset = offset + 5;
        //graphics.DrawString("Items,Code,Fabr.T   Prd.F,Design Size,Color Qty,Rate Amount ", fontSize5Bold,
        //         new SolidBrush(Color.Black), startX, startY + offset);

        DataTable DT1 = SalesManager.GetSalesDetails(lblInvNo.Text);

        graphics.DrawString("Name", fontSize6,
             new SolidBrush(Color.Black), startX, startY + offset);



        //graphics.DrawString(item.ItemSize.PadLeft(5), fontSize7,
        //  new SolidBrush(Color.Black), startX + 75, startY + offset + 10);
        graphics.DrawString("", fontSize6,
         new SolidBrush(Color.Black), startX + 80, startY + offset + 10);






        graphics.DrawString("Qty".PadLeft(4), fontSize6,
                new SolidBrush(Color.Black), startX + 100, startY + offset);



        graphics.DrawString("Seals Price".PadLeft(4), fontSize6,
               new SolidBrush(Color.Black), startX + 130, startY + offset);



        graphics.DrawString("", fontSize6,
        new SolidBrush(Color.Black), startX + 150, startY + offset + 10);


        graphics.DrawString("Total Price".PadLeft(4), fontSize6,
             new SolidBrush(Color.Black), startX + 210, startY + offset);
        //totalQuantity = totalQuantity + Convert.ToInt16(item.Quantity);
        //if (flag != "Y")
        //{
        //    graphics.DrawString("Rate,Amnt".PadLeft(6), fontSize6,
        //             new SolidBrush(Color.Black), startX + 226, startY + offset);


        //}


        offset = offset + 15;
        graphics.DrawLine(pen, startX, startY + offset, startX + 263, startY + offset);

        double totalQuantity = 0;
        decimal SubTotal = 0;
        decimal Total = 0;
        decimal discount = 0;
        decimal vat = 0;
        decimal TotalPayment = 0;

        offset = offset + 5;

        //DataTable orderForMoneyReceipt = IdManager.GetShowDataTable("select TOP(4) 'Shirt' as ItemName,'0424' as ItemCode,'good' as FabricsType,'ABCD' AS Design,'f54ds5f' AS GoodsTypeID,'XL' AS SizeName ,'RED' AS ColorName,'748' AS Quantity,'40' AS SalePrice,'40' AS TotalPrice,'40' AS PaymentMode,'7000' AS CashReceived,'140' AS CashRefund,'41' AS GrandTotal, 'SAM' AS ServiceBy,'100' AS DiscountAmount  from dbo.CLIENT_INFO");
        foreach (DataRow item in DT1.Rows)
        {
            graphics.DrawString(item["ItemsName"].ToString(), fontSize6,
            new SolidBrush(Color.Black), startX, startY + offset);

            //graphics.DrawString(item.ItemSize.PadLeft(5), fontSize7,
            //  new SolidBrush(Color.Black), startX + 75, startY + offset + 10);





            graphics.DrawString("", fontSize6,
       new SolidBrush(Color.Black), startX + 80, startY + offset + 10);







            if (!string.IsNullOrEmpty(item["Qty"].ToString()))
            {


                graphics.DrawString(item["Qty"].ToString().ToString().PadLeft(4), fontSize6,
               new SolidBrush(Color.Black), startX + 100, startY + offset);

            }
            else
            {
                graphics.DrawString("".PadLeft(4), fontSize6,
                new SolidBrush(Color.Black), startX + 100, startY + offset);
            }

            if (!string.IsNullOrEmpty(item["SPrice"].ToString()))
            {

                graphics.DrawString(item["SPrice"].ToString().PadLeft(4), fontSize6,
              new SolidBrush(Color.Black), startX + 130, startY + offset);
            }
            else
            {
                graphics.DrawString("".PadLeft(4), fontSize6,
               new SolidBrush(Color.Black), startX + 130, startY + offset);
            }


            graphics.DrawString("", fontSize6,
            new SolidBrush(Color.Black), startX + 150, startY + offset + 10);


            if (!string.IsNullOrEmpty(item["Total"].ToString()))
            {

                graphics.DrawString(item["Total"].ToString().PadLeft(4), fontSize6,
            new SolidBrush(Color.Black), startX + 210, startY + offset);
            }
            else
            {
                graphics.DrawString("".PadLeft(4), fontSize6,
             new SolidBrush(Color.Black), startX + 210, startY + offset);
            }

            offset = offset + 15;
        }

        graphics.DrawLine(new Pen(Color.Black, Convert.ToSingle(0.5)), startX, startY + offset, startX + 262, startY + offset);

        offset = offset + 5;
        graphics.DrawString("Total : ", fontSize7Bold,
                 new SolidBrush(Color.Black), startX, startY + offset);
        graphics.DrawString(txtSubTotal.Text.PadLeft(13), fontSize7,
                 new SolidBrush(Color.Black), startX + 175, startY + offset);
        offset = offset + 15;
        graphics.DrawString("VAT (%) : ", fontSize7Bold,
            new SolidBrush(Color.Black), startX, startY + offset);
        graphics.DrawString(txtVat.Text.PadLeft(13), fontSize7,
            new SolidBrush(Color.Black), startX + 175, startY + offset);
        offset = offset + 15;
        graphics.DrawString("Discount(TK) : ", fontSize7Bold,
            new SolidBrush(Color.Black), startX, startY + offset);
        graphics.DrawString(txtDiscount.Text.PadLeft(13), fontSize7,
            new SolidBrush(Color.Black), startX + 175, startY + offset);
        offset = offset + 15;
        graphics.DrawString("Grand Total : ", fontSize7Bold,
            new SolidBrush(Color.Black), startX, startY + offset);
        graphics.DrawString(txtGrandTotal.Text.PadLeft(13), fontSize7,
            new SolidBrush(Color.Black), startX + 175, startY + offset);
        offset = offset + 15;
        graphics.DrawString("Total Payment : ", fontSize7Bold,
            new SolidBrush(Color.Black), startX, startY + offset);
        graphics.DrawString(txtPayment.Text.PadLeft(13), fontSize7,
            new SolidBrush(Color.Black), startX + 175, startY + offset);
        offset = offset + 15;
        graphics.DrawString("Due Amount : ", fontSize7Bold,
            new SolidBrush(Color.Black), startX, startY + offset);
        graphics.DrawString(txtDue.Text.PadLeft(13), fontSize7,
            new SolidBrush(Color.Black), startX + 175, startY + offset);
        offset = offset + 15;

        offset = offset + 15;
        graphics.DrawString("Served By : " + Session["user"].ToString(),
                    fontSize7,
                    new SolidBrush(Color.Black), startX, startY + offset);

        offset = offset + 15;
        //graphics.DrawString("We hepply Exchange unused & unaltered  ", fontSize7,
        //         new SolidBrush(Color.Black), startX + 5, startY + offset);

        //offset = offset + 10;
        //graphics.DrawString("Product Within 7 Days along with the ", fontSize7,
        //         new SolidBrush(Color.Black), startX + 5, startY + offset);

        //offset = offset + 10;
        //graphics.DrawString("original receipt & tag or SMS. ", fontSize7,
        //         new SolidBrush(Color.Black), startX + 5, startY + offset);
        //offset = offset + 10;

        //graphics.DrawString("excapt undergarments,belt,tai or any ", fontSize7,
        //         new SolidBrush(Color.Black), startX + 5, startY + offset);

        //offset = offset + 10;
        //graphics.DrawString("others accessories. our product are also ", fontSize7,
        //         new SolidBrush(Color.Black), startX + 5, startY + offset);

        //offset = offset + 15;

        //graphics.DrawString("available at:"+companyInfo.Rows[0]["URL"].ToString(), fontSize7Bold,
        //         new SolidBrush(Color.Black), startX + 5, startY + offset);
        //try
        //{
        //    if (Convert.ToDouble(dtorderForMoneyReceipt.Rows[0]["DiscountAmount"]) > 0)
        //    {
        //        offset = offset + 15;
        //        graphics.DrawString("Discount Product Not Changeable!", fontSize7Bold,
        //                new SolidBrush(Color.Blue), startX + 5, startY + offset);
        //    }
        //}
        //catch
        //{
        //}
        // offset = offset + 15;


        //offset = offset + 15;
        //graphics.DrawString("www.facebook.com/dorjibaribd", fontSize7Bold,
        //         new SolidBrush(Color.Black), startX, startY + offset);

        // offset = offset + 15;
        // company.Email = "541454";
        //  graphics.DrawString(companyInfo.Rows[0]["EMAIL"].ToString(), fontSize7Bold,
        //          new SolidBrush(Color.Black), startX + 5, startY + offset);

        //   offset = offset + 15;
        //graphics.DrawString(Global.ApplicationNameWithVersion + "/Powered By " + Application.CompanyName, fontSize6,
        //         new SolidBrush(Color.Black), startX, startY + offset);

        graphics.DrawString("Powered By Netsoft Solution Ltd", fontSize6,
                 new SolidBrush(Color.Black), startX, startY + offset);

        offset = offset + 15;
        graphics.DrawString(" ", fontSize6,
                 new SolidBrush(Color.Black), startX, startY + offset);

        offset = offset + 30;
        graphics.DrawString(".", fontSize6,
               new SolidBrush(Color.Black), startX, startY + offset);
        offset = offset + 30;
        graphics.DrawString(". ", fontSize6,
                new SolidBrush(Color.Black), startX, startY + offset);
        offset = offset + 20;


    }

    protected void ddlPaymentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlBankName.DataSource = "";
        ddlBankName.DataBind();
        ddlBankName.SelectedIndex = -1;
        ddlAccountNo.DataSource = "";
        ddlAccountNo.DataBind();
        ddlAccountNo.SelectedIndex = -1;


        lblAcountNo.Visible = ddlBankName.Visible =
            lblBankNameTo.Visible = lblAcountNo.Visible = ddlAccountNo.Visible = lblAccNoPint.Visible = lblRcbBankPoint.Visible = false;
        if (ddlPaymentTypeTo.SelectedValue != "1")
        {
            ddlBankName.Visible = lblBankNameTo.Visible = lblAcountNo.Visible = ddlAccountNo.Visible = lblAccNoPint.Visible = lblRcbBankPoint.Visible = true;
            if (ddlPaymentTypeTo.SelectedValue == "2")
            {
                DataTable dtBankList =
                    IdManager.GetShowDataTable(
                        "Select t2.bank_name as bank_name,t2.bank_id from dbo.bank_info t2 where t2.Active=1 order by bank_name asc");
                util.PopulationDropDownList(ddlBankName, "bank_name", "bank_id", dtBankList);
            }

            else if (ddlPaymentTypeTo.SelectedValue == "5")
            {
                DataTable dtBankList =
                    IdManager.GetShowDataTable(
                        "Select t2.bank_name as bank_name,t2.bank_id from dbo.bank_info t2 where t2.Active=1 order by bank_name asc");
                util.PopulationDropDownList(ddlBankName, "bank_name", "bank_id", dtBankList);
            }

            else if (ddlPaymentTypeTo.SelectedValue == "3")
            {
                DataTable dtBankList =
                    IdManager.GetShowDataTable(
                       "Select t2.bank_name as bank_name,t2.bank_id from dbo.bank_info t2 where t2.Active=1 order by bank_name asc");
                ddlBankName.DataSource = dtBankList;
                ddlBankName.DataValueField = "bank_id";
                ddlBankName.DataTextField = "bank_name";
                ddlBankName.DataBind();
                try
                {
                    ddlBankName.Items.Insert(0, "");
                }
                catch
                {
                }

                // util.PopulationDropDownList(ddlBankName, "bank_name", "bank_id", dtBankList);
                //var a = ddlBankName.SelectedValue;
                //var b = ddlBankName.SelectedItem;

                if (ddlBankName.SelectedValue == "2")
                {
                    DataTable dtBankListSub = IdManager.GetShowDataTable(
                            "Select t2.bank_name+' - '+AccountNo as bank_name,t1.ID from dbo.bank_branch t1 inner join dbo.bank_info t2 on t2.Bank_Id=t1.bank_id where t1.DeleteBy is null and t1.bank_id='" + ddlBankName.SelectedValue + "' and t1.BankType ='C' order by bank_name asc");
                    ddlAccountNo.DataSource = dtBankListSub;
                    ddlAccountNo.DataValueField = "ID";
                    ddlAccountNo.DataTextField = "bank_name";
                    ddlAccountNo.DataBind();
                    ddlAccountNo.SelectedIndex = 0;
                    // util.PopulationDropDownList(ddlAccountNo, "bank_name", "ID", dtBankListSub);
                }

            }

            else if (ddlPaymentTypeTo.SelectedValue == "4")
            {
                DataTable dtBankList =
                    IdManager.GetShowDataTable(
                    "Select t2.bank_name as bank_name,t2.bank_id from dbo.bank_info t2 where t2.Active=1 order by bank_name asc");

                util.PopulationDropDownList(ddlBankName, "bank_name", "bank_id", dtBankList);
            }


        }

    }

    private void GetPaymentEmptyTable()
    {
        DataTable dtPackageList = new DataTable();
        dtPackageList.Columns.Add("DtlID", typeof(string));
        dtPackageList.Columns.Add("PaymentypeID", typeof(string));
        dtPackageList.Columns.Add("PaymentypeIDFrom", typeof(string));
        dtPackageList.Columns.Add("Amount", typeof(string));
        dtPackageList.Columns.Add("Paymentype", typeof(string));
        dtPackageList.Columns.Add("PaymentypeFrom", typeof(string));
        dtPackageList.Columns.Add("BankID", typeof(string));
        dtPackageList.Columns.Add("BankName", typeof(string));
        dtPackageList.Columns.Add("BankIDFrom", typeof(string));
        dtPackageList.Columns.Add("BankNameFrom", typeof(string));
        dtPackageList.Columns.Add("AccountNoFrom", typeof(string));
        dtPackageList.Columns.Add("AccountNo", typeof(string));
        dtPackageList.Columns.Add("AccountID", typeof(string));
        dtPackageList.Columns.Add("CardOrCheeckNo", typeof(string));
        dtPackageList.Columns.Add("Status", typeof(string));
        dtPackageList.Columns.Add("ApprovedDate", typeof(string));


        ViewState["paymentInfo"] = dtPackageList;
    }
    protected void btnPaymentAdd_Click(object sender, EventArgs e)
    {
        try
        {

            if (ddlPaymentTypeTo.SelectedValue == "" || ddlPaymentTypeTo.SelectedValue == "0")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please Select  payment Type!','teal',3);", true);
                return;
            }
            if (string.IsNullOrEmpty(txtAmount.Text) || txtAmount.Text == "0")
            {

                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please Input Amount..!!','teal',3);", true);

                txtAmount.Focus();
                return;
            }

            if (string.IsNullOrEmpty(ddlPaymentTypeTo.SelectedItem.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please Select  payment Type!!','teal',3);", true);
                return;
            }
            if (string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please Input Amount..!!','teal',3);", true);
                
                txtAmount.Focus();
                return;
            }
            if (Convert.ToDecimal(txtAmount.Text) <= 0)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please Input Amount..!!','teal',3);", true);

                txtAmount.Focus();
                return;
            }

            if (!ddlPaymentTypeTo.SelectedValue.ToString().Equals("1"))
            {
                if (string.IsNullOrEmpty(ddlBankName.SelectedItem.Text))
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please select bank name.!!','teal',3);", true);
                     return;
                }

                if (string.IsNullOrEmpty(ddlAccountNo.SelectedItem.Text))
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Please account number.!!','teal',3);", true);
                    return;
                }
            }

            DataTable dt = new DataTable();
            if (ViewState["paymentInfo"] == null)
            {
                GetPaymentEmptyTable();
            }

            double Amount = 0;
            dt = (DataTable)ViewState["paymentInfo"];
            foreach (DataRow drchk in dt.Rows)
            {
                if (drchk["PaymentypeID"].ToString() != ddlPaymentTypeTo.SelectedValue)
                {
                    Amount += Convert.ToDouble(drchk["Amount"]);
                }
            }
            if (Convert.ToDouble(txtGrandTotal.Text) < (Amount + Convert.ToDouble(txtAmount.Text)))
            {


                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Payment amount not upper then sale amount..!!','teal',3);", true);
                return;
            }
            string flag = "";
            foreach (DataRow data in dt.Rows)
            {

                if (ddlPaymentTypeFrom.SelectedValue != "2")
                {
                    //  DataRow data = dt.Rows(dr2);
                    if (data["PaymentypeIDFrom"].ToString() == ddlPaymentTypeFrom.SelectedValue && data["PaymentypeID"].ToString() == ddlPaymentTypeTo.SelectedValue)
                    {
                        if (!string.IsNullOrEmpty(ddlPaymentStatus.SelectedValue))
                        {
                            data["Status"] = ddlPaymentStatus.SelectedValue;
                        }
                        // data["AccountNoFrom"] = txtAccountNo.Text;
                        //data["BankIDFrom"] = ddlBankNameFrom.SelectedValue;
                        // data["BankNameFrom"] = ddlBankNameFrom.SelectedItem.Text;

                        //data["PaymentypeFrom"] = ddlPaymentTypeFrom.SelectedItem.Text;
                        //if (!string.IsNullOrEmpty(ddlPaymentTypeFrom.SelectedValue))
                        //{
                        //    data["PaymentypeIDFrom"] = ddlPaymentTypeFrom.SelectedValue;
                        //}

                        //data["ApprovedDate"] = txtApprovedDate.Text;
                        if (!string.IsNullOrEmpty(ddlPaymentTypeTo.SelectedValue))
                        {
                            data["PaymentypeID"] = ddlPaymentTypeTo.SelectedValue;
                        }

                        if (!string.IsNullOrEmpty(txtAmount.Text))
                        {
                            try
                            {
                                //decimal totalAmount = 0, totalservice = 0;
                                //if (txtPaidAmount.Text == "")
                                //{
                                //    txtPaidAmount.Text = "0";
                                //}
                                //totalAmount = Convert.ToDecimal(txtPaidAmount.Text);
                                //totalservice = Convert.ToDecimal(txtServicCharge.Text);
                                //if (totalAmount + Convert.ToDecimal(txtAmount.Text) > totalservice)
                                //{
                                //    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                //        "alert('Received Amount is Over From service Charge...!!');", true);
                                //    return;
                                //}

                                data["Amount"] = txtAmount.Text;
                            }
                            catch
                            {

                            }

                        }

                        data["Paymentype"] = ddlPaymentTypeTo.SelectedItem.Text;
                        if (!string.IsNullOrEmpty(ddlBankName.SelectedValue))
                        {
                            data["BankID"] = ddlBankName.SelectedValue;
                        }

                        try
                        {
                            data["BankName"] = ddlBankName.SelectedItem.Text;
                            data["AccountNo"] = ddlAccountNo.SelectedItem.Text;
                            data["AccountID"] = ddlAccountNo.SelectedValue;
                        }
                        catch
                        {

                        }
                        data["CardOrCheeckNo"] = txtcheeckNo.Text;
                        flag = "Y";
                    }
                }
                //else if (data["PaymentypeIDFrom"].ToString() == ddlPaymentTypeFrom.SelectedValue)
                //{


                //    if (!string.IsNullOrEmpty(ddlPaymentStatus.SelectedValue))
                //    {
                //        data["Status"] = ddlPaymentStatus.SelectedValue;
                //    }
                //    data["AccountNoFrom"] = txtAccountNo.Text;
                //    data["BankIDFrom"] = ddlBankNameFrom.SelectedValue;
                //    data["BankNameFrom"] = ddlBankNameFrom.SelectedItem.Text;

                //    data["PaymentypeFrom"] = ddlPaymentTypeFrom.SelectedItem.Text;
                //    if (!string.IsNullOrEmpty(ddlPaymentTypeFrom.SelectedValue))
                //    {
                //        data["PaymentypeIDFrom"] = ddlPaymentTypeFrom.SelectedValue;
                //    }

                //    data["ApprovedDate"] = txtApprovedDate.Text;
                //    if (!string.IsNullOrEmpty(ddlPaymentTypeTo.SelectedValue))
                //    {
                //        data["PaymentypeID"] = ddlPaymentTypeTo.SelectedValue;
                //    }
                //    try
                //    {
                //        decimal totalAmount = 0, totalservice = 0;
                //        //if (txtPaidAmount.Text == "")
                //        //{
                //        //    txtPaidAmount.Text = "0";
                //        //}
                //        //totalAmount = Convert.ToDecimal(txtPaidAmount.Text);
                //        //totalservice = Convert.ToDecimal(txtServicCharge.Text);
                //        //if (totalAmount + Convert.ToDecimal(txtAmount.Text) > totalservice)
                //        //{
                //        //    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                //        //        "alert('Received Amount is Over From service Charge...!!');", true);
                //        //    return;
                //        //}

                //        data["Amount"] = txtAmount.Text;
                //    }
                //    catch
                //    {

                //    }
                //    data["Paymentype"] = ddlPaymentTypeTo.SelectedItem.Text;
                //    if (!string.IsNullOrEmpty(ddlBankName.SelectedValue))
                //    {
                //        data["BankID"] = ddlBankName.SelectedValue;
                //    }

                //    try
                //    {
                //        data["BankName"] = ddlBankName.SelectedItem.Text;
                //        data["AccountNo"] = ddlAccountNo.SelectedItem.Text;
                //        data["AccountID"] = ddlAccountNo.SelectedValue;
                //    }
                //    catch
                //    {

                //    }
                //    data["CardOrCheeckNo"] = txtcheeckNo.Text;
                //    flag = "Y";
                //}
            }


            if (flag != "Y")
            {

                DataRow dr = dt.NewRow();
                dr["DtlID"] = "0";

                if (!string.IsNullOrEmpty(ddlPaymentStatus.SelectedValue))
                {
                    dr["Status"] = ddlPaymentStatus.SelectedValue;
                }
                dr["AccountNoFrom"] = txtAccountNo.Text;
                if (!string.IsNullOrEmpty(ddlBankNameFrom.SelectedValue))
                {
                    dr["BankIDFrom"] = ddlBankNameFrom.SelectedValue;
                }
                dr["BankNameFrom"] = ddlBankNameFrom.SelectedItem.Text;

                dr["PaymentypeFrom"] = ddlPaymentTypeFrom.SelectedItem.Text;
                if (!string.IsNullOrEmpty(ddlPaymentTypeFrom.SelectedValue))
                {
                    dr["PaymentypeIDFrom"] = ddlPaymentTypeFrom.SelectedValue;
                }

                dr["ApprovedDate"] = txtApprovedDate.Text;
                if (!string.IsNullOrEmpty(ddlPaymentTypeTo.SelectedValue))
                {
                    dr["PaymentypeID"] = ddlPaymentTypeTo.SelectedValue;
                }
                try
                {
                    decimal totalAmount = 0, totalservice = 0;

                    //if (txtPaidAmount.Text == "")
                    //{
                    //    txtPaidAmount.Text = "0";
                    //}
                    //totalAmount = Convert.ToDecimal(txtPaidAmount.Text);
                    //totalservice = Convert.ToDecimal(txtServicCharge.Text);
                    //if (totalAmount + Convert.ToDecimal(txtAmount.Text) > totalservice
                    //)
                    //{
                    //    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    //        "alert('Received Amount is Over From service Charge...!!');", true);
                    //    return;
                    //}

                    dr["Amount"] = txtAmount.Text;
                }
                catch
                {

                }
                dr["Paymentype"] = ddlPaymentTypeTo.SelectedItem.Text;
                if (!string.IsNullOrEmpty(ddlBankName.SelectedValue))
                {
                    dr["BankID"] = ddlBankName.SelectedValue;
                }

                try
                {
                    dr["BankName"] = ddlBankName.SelectedItem.Text;
                    dr["AccountNo"] = ddlAccountNo.SelectedItem.Text;
                    dr["AccountID"] = ddlAccountNo.SelectedValue;

                }
                catch
                {

                }

                dr["CardOrCheeckNo"] = txtcheeckNo.Text;
                dt.Rows.Add(dr);


                // txtPaidAmount.Text = (Convert.ToDecimal(txtPaidAmount.Text) + Convert.ToDecimal(txtAmount.Text)).ToString();
            }

            Calculation();
            dgPaymentInfo.DataSource = dt;
            dgPaymentInfo.DataBind();
            ViewState["paymentInfo"] = dt;
            txtAmount.Text = txtcheeckNo.Text = txtAccountNo.Text = txtApprovedDate.Text = "";
            //ddlPaymentTypeTo.SelectedIndex =
            ddlBankName.SelectedIndex = ddlBankNameFrom.SelectedIndex = ddlAccountNo.SelectedIndex = -1;

            // UP1.Update();
        }
        catch
        {

        }
    }

    private void Calculation()
    {
        DataTable dt = (DataTable)ViewState["paymentInfo"];
        txtAmount.Text = "0";
        txtPayment.Text = "0";
        if (dt.Rows.Count != null)
        {
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Amount"].ToString() == "")
                    {

                        dr["Amount"] = "0";
                    }

                    txtAmount.Text = (Convert.ToDecimal(txtAmount.Text) + Convert.ToDecimal(dr["Amount"].ToString()))
                        .ToString();
                    txtPayment.Text = (Convert.ToDecimal(txtPayment.Text) + Convert.ToDecimal(dr["Amount"].ToString()))
                        .ToString();
                }

                VetAndDiscount();
            }
        }

    }


    protected void btnClearAll_Click(object sender, EventArgs e)
    {
        RefreshAll();
    }
    protected void dgPaymentInfo_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[0].Attributes.Add("style", "display:none");
            e.Row.Cells[1].Attributes.Add("style", "display:none");

        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[0].Attributes.Add("style", "display:none");
            e.Row.Cells[1].Attributes.Add("style", "display:none");
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Attributes.Add("style", "display:none");
            e.Row.Cells[1].Attributes.Add("style", "display:none");

        }
    }

    private void RefreshAll()
    {
        DataTable dtpaymentType = IdManager.GetShowDataTable("Select Name,ID from PaymentMethod where Id!=2  order by id asc");
        //DataTable dtpaymentTypeReceived = IdManager.GetShowDataTable("Select Name,ID from PaymentMethod   order by id asc");
        ddlPaymentTypeFrom.DataSource = dtpaymentType;
        ddlPaymentTypeFrom.DataTextField = "Name";
        ddlPaymentTypeFrom.DataValueField = "ID";
        ddlPaymentTypeFrom.DataBind();

        ddlPaymentTypeTo.DataSource = dtpaymentType;
        ddlPaymentTypeTo.DataTextField = "Name";
        ddlPaymentTypeTo.DataValueField = "ID";
        ddlPaymentTypeTo.DataBind();

        txtAmount.Text = txtcheeckNo.Text = txtAccountNo.Text = txtApprovedDate.Text = "";
        ddlBankName.SelectedIndex = ddlBankNameFrom.SelectedIndex = ddlAccountNo.SelectedIndex = -1;
        DataTable dtBankList =
            IdManager.GetShowDataTable(
                "Select bank_name,bank_id from bank_info  order by bank_name asc");
        util.PopulationDropDownList(ddlBankNameFrom, "bank_name", "bank_id", dtBankList);
        dgPaymentInfo.DataSource = null;
        dgPaymentInfo.DataBind();
        ViewState["paymentInfo"] = null;
        txtCustomerName.Text = "";
        lblAcountNo.Visible = ddlBankName.Visible =
            lblBankNameTo.Visible = lblAcountNo.Visible =
                ddlAccountNo.Visible = lblAccNoPint.Visible = lblAccNoPint.Visible = false;

        lblAcountNoFrom.Visible = ddlBankNameFrom.Visible = lblBankNameFrom.Visible = lblStatus.Visible =
            lblApprovedDate.Visible = txtApprovedDate.Visible = ddlPaymentStatus.Visible =
                lblChekNo.Visible = txtcheeckNo.Visible = txtAccountNo.Visible = false;
        txtPayment.Text = "0";
        txtLastFigarAmount.Text = "0";
        VetAndDiscount();
        //txtLastFigarAmount.Text = "0";


    }

    protected void ddlBankName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlBankName.SelectedItem.Text != "")
        {
            if (ddlPaymentTypeTo.SelectedValue == "2")
            {
                DataTable dtBankList =
                    IdManager.GetShowDataTable(
                        "Select t2.bank_name+' - '+AccountNo as bank_name,t1.ID from dbo.bank_branch t1 inner join dbo.bank_info t2 on t2.Bank_Id=t1.bank_id where t1.DeleteBy is null and t1.bank_id='" + ddlBankName.SelectedValue + "' and t1.BankType ='B' order by bank_name asc");
                util.PopulationDropDownList(ddlAccountNo, "bank_name", "ID", dtBankList);
            }

            else if (ddlPaymentTypeTo.SelectedValue == "3")
            {
                DataTable dtBankList =
                    IdManager.GetShowDataTable(
                        "Select t2.bank_name+' - '+AccountNo as bank_name,t1.ID from dbo.bank_branch t1 inner join dbo.bank_info t2 on t2.Bank_Id=t1.bank_id where t1.DeleteBy is null and t1.bank_id='" + ddlBankName.SelectedValue + "' and t1.BankType ='C' order by bank_name asc");
                util.PopulationDropDownList(ddlAccountNo, "bank_name", "ID", dtBankList);
            }

            else if (ddlPaymentTypeTo.SelectedValue == "4")
            {
                DataTable dtBankList =
                    IdManager.GetShowDataTable(
                       "Select t2.bank_name+' - '+AccountNo as bank_name,t1.ID from dbo.bank_branch t1 inner join dbo.bank_info t2 on t2.Bank_Id=t1.bank_id where t1.DeleteBy is null and t1.bank_id='" + ddlBankName.SelectedValue + "' and t1.BankType ='M' order by bank_name asc");
                util.PopulationDropDownList(ddlAccountNo, "bank_name", "ID", dtBankList);
            }

        }
        else
        {
            DataTable dtBankList = IdManager.GetShowDataTable("Select t2.bank_name+' - '+AccountNo as bank_name,t1.ID from dbo.bank_branch t1 inner join dbo.bank_info t2 on t2.Bank_Id=t1.bank_id where t1.DeleteBy is null and t1.bank_id='" + ddlBankName.SelectedValue + "' and t1.BankType ='M' order by bank_name asc");
            util.PopulationDropDownList(ddlAccountNo, "bank_name", "ID", dtBankList);
        }
    }
    protected void lnkbCustomerLink_Click(object sender, EventArgs e)
    {
        string strJS = ("<script type='text/javascript'>window.open('frmClient.aspx?mno=8.28','_blank');</script>");
        Page.ClientScript.RegisterStartupScript(this.GetType(), "strJSAlert", strJS);
    }
    protected void ddlPaymentTypeFrom_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblAcountNoFrom.Visible = ddlBankNameFrom.Visible = lblBankNameFrom.Visible = lblStatus.Visible =
            lblApprovedDate.Visible = txtApprovedDate.Visible = ddlPaymentStatus.Visible =
                lblChekNo.Visible = txtcheeckNo.Visible = txtAccountNo.Visible = false;
        if (ddlPaymentTypeFrom.SelectedValue != "1")
        {
            if (ddlPaymentTypeFrom.SelectedValue == "2" || ddlPaymentTypeFrom.SelectedItem.Text == "2" || ddlPaymentTypeFrom.SelectedItem.Text == "2")
            {
                lblStatus.Visible = lblApprovedDate.Visible = txtApprovedDate.Visible =
                    ddlPaymentStatus.Visible = lblChekNo.Visible = txtcheeckNo.Visible = true;
            }

            lblAcountNoFrom.Visible = ddlBankNameFrom.Visible = lblBankNameFrom.Visible = txtAccountNo.Visible = true;
        }
    }

    protected void txtAmount_TextChanged(object sender, EventArgs e)
    {
        //try
        //{
        //    try
        //    {
        //        var amt = Convert.ToDecimal(txtAmount.Text);
        //    }
        //    catch
        //    {
        //        txtAmount.Text = "0";
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('This Amount Is Not Right So 0 value is Fixed..!!!!');", true);

        //    }
        //    amountPointCalculation();
        //    var grandTotal = Convert.ToDecimal(txtGrandTotal.Text);
        //    if (grandTotal > 0)
        //    {
        //        if (string.IsNullOrEmpty(txtAmount.Text))
        //        {
        //            txtAmount.Text = txtGrandTotal.Text;
        //        }
        //        var CusPayAmt = Convert.ToDecimal(txtAmount.Text);
        //        //if (CusPayAmt < grandTotal)
        //        //{
        //        //    txtAmount.Text = grandTotal.ToString("N2");

        //        //    txtDue.Text = (grandTotal - Convert.ToDecimal(txtAmount.Text)).ToString("N2");

        //        // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('This Amount Is Low So I Do Lavel This Amount..!!!!');", true);
        //        //}

        //        //else
        //        //{
        //        //    txtDue.Text = (grandTotal - CusPayAmt).ToString("N2");

        //        //}
        //        txtPayment.Text = CusPayAmt.ToString();
            
        //            txtDue.Text = (grandTotal - CusPayAmt).ToString("N2");

                


        //    }
        //    else
        //    {
        //        txtAmount.Text = "0.00";
        //    }

        //}
        //catch (FormatException fex)
        //{
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('" + fex.Message + "','red',0);", true);

        //}
        //catch (Exception ex)
        //{
        //    if (ex.Message.Contains("Database"))
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Database Maintain Error. Contact to the Software Provider..!!','red',0);", true);
        //    else;
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
        //} 
        //try
        //{
            if (ddlPaymentTypeTo.SelectedValue == "" || ddlPaymentTypeTo.SelectedValue == "0")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Please Select  payment Type!!');", true);
                 return;
            }
            if (string.IsNullOrEmpty(txtAmount.Text) || txtAmount.Text == "0")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Please Input Amount..!!');", true);
                txtAmount.Focus();
                return;
            }

            if (string.IsNullOrEmpty(ddlPaymentTypeTo.SelectedItem.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Please Select  payment Type!!');", true);

                return;
            }
            if (string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
             
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Please Input Amount..!!');", true);

                txtAmount.Focus();
                return;
            }
            if (Convert.ToDecimal(txtAmount.Text) <= 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Please Input Amount..!!');", true);
                 txtAmount.Focus();
                return;
            }

            if (!ddlPaymentTypeTo.SelectedValue.ToString().Equals("1"))
            {
                if (string.IsNullOrEmpty(ddlBankName.SelectedItem.Text))
                {
                    txtAmount.Text = "0";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Please select bank name.!!');", true);
                     return;
                }

                if (string.IsNullOrEmpty(ddlAccountNo.SelectedItem.Text))
                {
                    txtAmount.Text = "0";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Please account number.!!');", true);

                    return;
                }
            }

            DataTable dt = new DataTable();
            if (ViewState["paymentInfo"] == null)
            {
                GetPaymentEmptyTable();
            }

            double Amount = 0;
            dt = (DataTable)ViewState["paymentInfo"];
            foreach (DataRow drchk in dt.Rows)
            {
                if (drchk["PaymentypeID"].ToString() != ddlPaymentTypeTo.SelectedValue)
                {
                    Amount += Convert.ToDouble(drchk["Amount"]);
                }
            }
            //if (Convert.ToDouble(txtGrandTotal.Text) < (Amount + Convert.ToDouble(txtAmount.Text)))
            //{
            //    txtAmount.Text = "0";
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Payment amount not upper then sale amount..!!');", true);
            //     return;
            //}
            string flag = "";
            foreach (DataRow data in dt.Rows)
            {

                if (ddlPaymentTypeFrom.SelectedValue != "2")
                {
                    //  DataRow data = dt.Rows(dr2);
                    if (data["PaymentypeIDFrom"].ToString() == ddlPaymentTypeFrom.SelectedValue && data["PaymentypeID"].ToString() == ddlPaymentTypeTo.SelectedValue)
                    {
                        if (!string.IsNullOrEmpty(ddlPaymentStatus.SelectedValue))
                        {
                            data["Status"] = ddlPaymentStatus.SelectedValue;
                        }
                        data["AccountNoFrom"] = txtAccountNo.Text;

                       
                        if (string.IsNullOrEmpty(ddlBankNameFrom.SelectedValue))
                        {
                            data["BankIDFrom"] = "0";
                            data["BankNameFrom"] = ddlBankNameFrom.SelectedItem.Text;
                        }
                        else
                        {
                            data["BankIDFrom"] = ddlBankNameFrom.SelectedValue;
                            data["BankNameFrom"] = ddlBankNameFrom.SelectedItem.Text;
                        }
                        

                        data["PaymentypeFrom"] = ddlPaymentTypeFrom.SelectedItem.Text;
                        if (!string.IsNullOrEmpty(ddlPaymentTypeFrom.SelectedValue))
                        {
                            data["PaymentypeIDFrom"] = ddlPaymentTypeFrom.SelectedValue;
                        }

                        data["ApprovedDate"] = txtApprovedDate.Text;
                        if (!string.IsNullOrEmpty(ddlPaymentTypeTo.SelectedValue))
                        {
                            data["PaymentypeID"] = ddlPaymentTypeTo.SelectedValue;
                        }

                        if (!string.IsNullOrEmpty(txtAmount.Text))
                        {
                            try
                            {
                                //decimal totalAmount = 0, totalservice = 0;
                                //if (txtPaidAmount.Text == "")
                                //{
                                //    txtPaidAmount.Text = "0";
                                //}
                                //totalAmount = Convert.ToDecimal(txtPaidAmount.Text);
                                //totalservice = Convert.ToDecimal(txtServicCharge.Text);
                                //if (totalAmount + Convert.ToDecimal(txtAmount.Text) > totalservice)
                                //{
                                //    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                //        "alert('Received Amount is Over From service Charge...!!');", true);
                                //    return;
                                //}

                                data["Amount"] = txtAmount.Text;
                            }
                            catch
                            {

                            }

                        }

                        data["Paymentype"] = ddlPaymentTypeTo.SelectedItem.Text;
                        if (!string.IsNullOrEmpty(ddlBankName.SelectedValue))
                        {
                            data["BankID"] = ddlBankName.SelectedValue;
                        }

                        try
                        {
                            data["BankName"] = ddlBankName.SelectedItem.Text;
                            data["AccountNo"] = ddlAccountNo.SelectedItem.Text;
                            data["AccountID"] = ddlAccountNo.SelectedValue;
                        }
                        catch
                        {

                        }
                        data["CardOrCheeckNo"] = txtcheeckNo.Text;
                        flag = "Y";
                    }
                }
                else if (data["PaymentypeIDFrom"].ToString() == ddlPaymentTypeFrom.SelectedValue)
                {


                    if (!string.IsNullOrEmpty(ddlPaymentStatus.SelectedValue))
                    {
                        data["Status"] = ddlPaymentStatus.SelectedValue;
                    }
                    data["AccountNoFrom"] = txtAccountNo.Text;
                    if (string.IsNullOrEmpty(ddlBankNameFrom.SelectedValue))
                    {
                        data["BankIDFrom"] = "0";
                        data["BankNameFrom"] = ddlBankNameFrom.SelectedItem.Text;
                    }
                    else
                    {
                        data["BankIDFrom"] = ddlBankNameFrom.SelectedValue;
                        data["BankNameFrom"] = ddlBankNameFrom.SelectedItem.Text;
                    }

                    data["PaymentypeFrom"] = ddlPaymentTypeFrom.SelectedItem.Text;
                    if (!string.IsNullOrEmpty(ddlPaymentTypeFrom.SelectedValue))
                    {
                        data["PaymentypeIDFrom"] = ddlPaymentTypeFrom.SelectedValue;
                    }

                    data["ApprovedDate"] = txtApprovedDate.Text;
                    if (!string.IsNullOrEmpty(ddlPaymentTypeTo.SelectedValue))
                    {
                        data["PaymentypeID"] = ddlPaymentTypeTo.SelectedValue;
                    }
                    try
                    {
                        decimal totalAmount = 0, totalservice = 0;
                        //if (txtPaidAmount.Text == "")
                        //{
                        //    txtPaidAmount.Text = "0";
                        //}
                        //totalAmount = Convert.ToDecimal(txtPaidAmount.Text);
                        //totalservice = Convert.ToDecimal(txtServicCharge.Text);
                        //if (totalAmount + Convert.ToDecimal(txtAmount.Text) > totalservice)
                        //{
                        //    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        //        "alert('Received Amount is Over From service Charge...!!');", true);
                        //    return;
                        //}

                        data["Amount"] = txtAmount.Text;
                    }
                    catch
                    {

                    }
                    data["Paymentype"] = ddlPaymentTypeTo.SelectedItem.Text;
                    if (!string.IsNullOrEmpty(ddlBankName.SelectedValue))
                    {
                        data["BankID"] = ddlBankName.SelectedValue;
                    }

                    try
                    {
                        data["BankName"] = ddlBankName.SelectedItem.Text;
                        data["AccountNo"] = ddlAccountNo.SelectedItem.Text;
                        data["AccountID"] = ddlAccountNo.SelectedValue;
                    }
                    catch
                    {

                    }
                    data["CardOrCheeckNo"] = txtcheeckNo.Text;
                    flag = "Y";
                }
            }


            if (flag != "Y")
            {

                DataRow dr = dt.NewRow();
                dr["DtlID"] = "0";

                if (!string.IsNullOrEmpty(ddlPaymentStatus.SelectedValue))
                {
                    dr["Status"] = ddlPaymentStatus.SelectedValue;
                }
                dr["AccountNoFrom"] = txtAccountNo.Text;
                if (!string.IsNullOrEmpty(ddlBankNameFrom.SelectedValue))
                {
                    dr["BankIDFrom"] = ddlBankNameFrom.SelectedValue;
                }
                dr["BankNameFrom"] = ddlBankNameFrom.SelectedItem.Text;

                dr["PaymentypeFrom"] = ddlPaymentTypeFrom.SelectedItem.Text;
                if (!string.IsNullOrEmpty(ddlPaymentTypeFrom.SelectedValue))
                {
                    dr["PaymentypeIDFrom"] = ddlPaymentTypeFrom.SelectedValue;
                }

                dr["ApprovedDate"] = txtApprovedDate.Text;
                if (!string.IsNullOrEmpty(ddlPaymentTypeTo.SelectedValue))
                {
                    dr["PaymentypeID"] = ddlPaymentTypeTo.SelectedValue;
                }
                try
                {
                    decimal totalAmount = 0, totalservice = 0;

                    //if (txtPaidAmount.Text == "")
                    //{
                    //    txtPaidAmount.Text = "0";
                    //}
                    //totalAmount = Convert.ToDecimal(txtPaidAmount.Text);
                    //totalservice = Convert.ToDecimal(txtServicCharge.Text);
                    //if (totalAmount + Convert.ToDecimal(txtAmount.Text) > totalservice
                    //)
                    //{
                    //    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    //        "alert('Received Amount is Over From service Charge...!!');", true);
                    //    return;
                    //}

                    dr["Amount"] = txtAmount.Text;
                }
                catch
                {

                }
                dr["Paymentype"] = ddlPaymentTypeTo.SelectedItem.Text;
                if (!string.IsNullOrEmpty(ddlBankName.SelectedValue))
                {
                    dr["BankID"] = ddlBankName.SelectedValue;
                }

                try
                {
                    dr["BankName"] = ddlBankName.SelectedItem.Text;
                    dr["AccountNo"] = ddlAccountNo.SelectedItem.Text;
                    dr["AccountID"] = ddlAccountNo.SelectedValue;

                }
                catch
                {

                }

                dr["CardOrCheeckNo"] = txtcheeckNo.Text;
                dt.Rows.Add(dr);


                // txtPaidAmount.Text = (Convert.ToDecimal(txtPaidAmount.Text) + Convert.ToDecimal(txtAmount.Text)).ToString();
            }

            Calculation();
            dgPaymentInfo.DataSource = dt;
            dgPaymentInfo.DataBind();
            ViewState["paymentInfo"] = dt;
            txtAmount.Text = txtcheeckNo.Text = txtAccountNo.Text = txtApprovedDate.Text = "";
            //ddlPaymentTypeTo.SelectedIndex =
            ddlBankName.SelectedIndex = ddlBankNameFrom.SelectedIndex = ddlAccountNo.SelectedIndex = -1;

            // UP1.Update();
        //}
      
        //catch (FormatException fex)
        //{
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('" + fex.Message + "','red',0);", true);

        //}
        //catch (Exception ex)
        //{
        //    if (ex.Message.Contains("Database"))
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Database Maintain Error. Contact to the Software Provider..!!','red',0);", true);
        //    else;
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
        //}  
    }

    protected void ddlAccountNo_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}