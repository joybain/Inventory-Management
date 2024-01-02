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

public partial class frmSalesExchange : System.Web.UI.Page
{// Ridoy
    SalesManager _aSalesManager = new SalesManager();
    ItemManager _aItemManager = new ItemManager();
    clsClientInfoManager _aclsClientInfoManager = new clsClientInfoManager();
    private static Permis per;
    private static VouchManager _aVouchManager = new VouchManager();
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
                        "Select ID,user_grp,description from utl_userinfo where upper(user_name)=upper('" +
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
                ((Label) Page.Master.FindControl("lblLogin")).Text = Session["wnote"].ToString();
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

            RefreshAll();
        
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtInvoiceNo.Text = "INV-RD-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() +
                                DateTime.Now.Day.ToString() + "-00" +
                                IdManager.GetShowSingleValueString("InvoiceID", "FixGlCoaCode");

            Panel1.Visible = txtItemsCode.Enabled =  false;
          
            //dgSVMst.DataSource = SalesManager.GetShowSalesDetails();
            //dgSVMst.DataBind();
            Clear();
         
          
            lblAcountNo.Visible = ddlBankName.Visible =
                lblBankNameTo.Visible = lblAcountNo.Visible = ddlAccountNo.Visible = lblAccNoPint.Visible = lblRcbBankPoint.Visible = false;
            DataTable dtpaymentTypeReceived = IdManager.GetShowDataTable("Select Name,ID from PaymentMethod where Id!=2  order by id asc");
            //util.PopulationDropDownList(ddlPaymentTypeFrom, "Name", "ID", dtpaymentType);
            util.PopulationDropDownList(ddlPaymentTypeTo, "Name", "ID", dtpaymentTypeReceived);
            ddlPaymentTypeTo.SelectedIndex = 1;
            Session["Cash_Code"] =
                IdManager.GetShowSingleValueString("CASH_CODE", "BOOK_NAME", "GL_SET_OF_BOOKS", "AMB");
            ddlAccountNo.SelectedIndex = ddlBankName.SelectedIndex = -1;
            dgPaymentInfo.DataSource = null;
            dgPaymentInfo.DataBind();
            getEmptyDtl();
            GetPaymentEmptyTable();
            // Panel1.Width = 100;
         
        }






       
    }
    protected void dgPVMst1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header |
                e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].Attributes.Add("style", "display:none");
               e.Row.Cells[1].Attributes.Add("style", "display:none");
               e.Row.Cells[2].Attributes.Add("style", "display:none");
               e.Row.Cells[3].Attributes.Add("style", "display:none");
               e.Row.Cells[12].Attributes.Add("style", "display:none");
            }
        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                    "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                    "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }

    private void ShowFooterTotal1(DataTable DT1)
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

       // txtSubTotal.Text = tot.ToString();
       // txtVat.Text = totVat.ToString();

        VetAndDiscount1();
        //txtGrandTotal.Text = (tot + ((tot * totVat) / 100)).ToString("N2");
        //txtDue.Text = ((tot + ((tot * totVat) / 100)) - Convert.ToDecimal(txtPayment.Text)).ToString("N2");
    }
    private void VetAndDiscount1()
    {
        //decimal subtotal, vat, discount, totalPayment;
        //try
        //{
        //    subtotal = Convert.ToDecimal(txtSubTotal.Text);
        //}
        //catch (Exception ex)
        //{
        //    subtotal = 0;
        //}

        //try
        //{
        //    vat = Convert.ToDecimal(txtVat.Text);
        //}
        //catch (Exception ex)
        //{
        //    vat = 0;
        //}

        //try
        //{
        //    discount = Convert.ToDecimal(txtDiscount.Text);
        //}
        //catch (Exception ex)
        //{
        //    discount = 0;
        //}

        //try
        //{
        //    totalPayment = Convert.ToDecimal(txtPayment.Text);
        //}
        //catch (Exception ex)
        //{
        //    totalPayment = 0;
        //}

        //txtGrandTotal.Text = (((subtotal + (subtotal * vat / 100)) - discount)).ToString("N2");
        //txtDue.Text = (((subtotal + (subtotal * vat / 100)) - discount) - totalPayment).ToString("N2");
        //UpItemsDetails.Update();
    }
    protected void btnInvoiceSearch_Click(object sender, EventArgs e)
    {
        try
        {

            int OrderID = IdManager.GetShowSingleValueInt("ID", "[Order] where Upper(Convert(nvarchar,OrderDate,103)+'-'+InvoiceNo)='" + txtSearchInvoice.Text + "'");
            Sales aSales = SalesManager.GetBranchShowSalesInfo(OrderID.ToString(),Session["BranchId"].ToString());
            hfInvoiceNo.Value = aSales.Invoice;
            hfOrderId.Value = OrderID.ToString();

            int IsExistExch=SalesManager.IsExistExchangeInvoice(hfInvoiceNo.Value,Session["BranchId"].ToString());
            int IsExistReturn = SalesManager.IsExistExchangeReturn(hfOrderId.Value, Session["BranchId"].ToString());
            if (IsExistExch>0||IsExistReturn>0)
            {
                hfInvoiceNo.Value = "";
                hfOrderId.Value = "";
                this.txtSearchInvoice.Text = "";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                    "alert('This Invoice Is  already Exchenge.!!');", true);
                return;
            }
            else
            {


                //hfDiscount.Value = aSales.Disount;
                //percentage amount wise discount;
                hfDiscount.Value = ((Convert.ToDecimal(aSales.Disount)*100)/Convert.ToDecimal(aSales.Total)).ToString();
                hfsubTotal.Value = aSales.Total;
                hfVat.Value = aSales.Tax;
                hfCustomerID.Value = aSales.Customer;
                hfCustomerName.Value = aSales.CustomerName;
                lblCustomerName.Text = aSales.CustomerName;
                hfCommonCus.Value = aSales.CommonCus;
                hfGl_CoaCode.Value = aSales.Gl_CoaCode;
                hfLocalCustomer.Value = aSales.LocalCustomer;
                hfLocalCusAddress.Value = aSales.LocalCusAddress;
                hfLocalCusPhone.Value = aSales.LocalCusPhone;
                hfNote.Value = aSales.Note;
                hfRemark.Value = aSales.Remarks;

               

                int TotalOrderquantity = 0;
                DataTable DT1 = SalesManager.GetMainBranchSalesDetails(OrderID.ToString(), Session["BranchId"].ToString());
                if (DT1.Rows.Count > 0)
                {
                    dgSV1.DataSource = DT1;
                    ViewState["SV1"] = DT1;
                    ViewState["OldSV1"] = DT1;
                    dgSV1.DataBind();
                    // ShowFooterTotal(DT1);
                }

                foreach (DataRow dr in DT1.Rows)
                {
                    TotalOrderquantity += Convert.ToInt32(dr["Qty"]);
                }

                hfTotalOrderquantity.Value = TotalOrderquantity.ToString();
            }
            // VetAndDiscount1();

        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex.Message + "');", true);
        }//Sales
        
    }
    protected void txtChangeQty_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            var qty = Convert.ToInt32(((TextBox)gvr.FindControl("txtQty")).Text);
            decimal chngQty = 0;
            try
            {
                 chngQty = Convert.ToInt32(((TextBox)gvr.FindControl("txtChangeQty")).Text);
            }
            catch
            {
                ((TextBox)gvr.FindControl("txtChangeQty")).Text = "0";
                return;
                
            }

            if (chngQty > qty)
            {
                ((TextBox)gvr.FindControl("txtChangeQty")).Text = "0";
                chngQty = 0;
            }

            decimal grantTotal,discount,vat;
            decimal  TotalVat = 0;
            decimal vhfSubTotal=0 , vhfTaxAmount=0 , vhfDiscountAmount = 0;
            vat = Convert.ToDecimal(hfVat.Value);
           
                grantTotal = 00;
                decimal total = 00;
            
            
            var singleQtyWiseDiscount = Convert.ToDecimal(hfDiscount.Value) / Convert.ToDecimal(hfTotalOrderquantity.Value);




            decimal SetupDiscountFlag = 0;
            foreach (GridViewRow tt in dgSV1.Rows)
            {


                TextBox txtChangeQty = (TextBox)tt.FindControl("txtChangeQty");
                decimal changeQty = Convert.ToDecimal(txtChangeQty.Text);
                TextBox txtSalesPrice = (TextBox)tt.FindControl("txtSalesPrice");
                decimal SingleSalesPrice = Convert.ToDecimal(txtSalesPrice.Text);

                 TextBox txtDiscount = (TextBox)tt.FindControl("txtDiscount");
                decimal SetupDiscount = ((SingleSalesPrice*changeQty) / 100) * Convert.ToDecimal(txtDiscount.Text);
                SetupDiscountFlag += SetupDiscount;
                vhfSubTotal += SingleSalesPrice * changeQty;

                //TotalVat += (((SingleSalesPrice - (singleQtyWiseDiscount + SetupDiscount))*vat)/100)*changeQty;
               // vhfDiscountAmount += (changeQty * singleQtyWiseDiscount)+SetupDiscount;

                TotalVat += (((SingleSalesPrice * changeQty) -
                              (((changeQty * SingleSalesPrice) * Convert.ToDecimal(hfDiscount.Value)) / 100)) * vat) / 100;
                    //(((SingleSalesPrice - (singleQtyWiseDiscount + SetupDiscount)) * vat) / 100) * changeQty;

                vhfDiscountAmount += (((changeQty * SingleSalesPrice) * Convert.ToDecimal(hfDiscount.Value))/100) + SetupDiscount;
              
                grantTotal += (SingleSalesPrice * changeQty) - ((changeQty * singleQtyWiseDiscount) + SetupDiscount);
                total += SingleSalesPrice * changeQty;
            }

            GrandTotal.Value = grantTotal.ToString();

            lblVat.Text = TotalVat.ToString("N2");
            lblSubTotal.Text = total.ToString("N2");
            lblTotal.Text = ((total-vhfDiscountAmount)+TotalVat).ToString("N2");
            //lblVat.Text = ((vat * total) / 100).ToString("#,0.00");
            //lblSubTotal.Text = grantTotal.ToString("#,0.00");
            //lblTotal.Text = (((vat * total) / 100) + grantTotal).ToString("#,0.00");



            hfsubTotal.Value = vhfSubTotal.ToString();
            //hfSetupTotalDiscount.Value = SetupDiscountFlag.ToString("N2");
            //hfDiscount.Value = (vhfDiscountAmount-SetupDiscountFlag).ToString();





            VetAndDiscount();


        }

        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex.Message + "');", true);
        }//Sales

    }

  
    // ridoy





















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
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                "alert('This Item already selected.!!');", true);

            ((TextBox)gvr.FindControl("txtItems")).Text = "";
        }

        txtItemsCode.Text = "";
        //UPCustomer.Update();
        //UpItemsDetails.Update();
        //UpSearch.Update();
    }


    protected void txtQty_TextChanged(object sender, EventArgs e)
    {
        //lblMessasge.Text = "";
        try
        {
            Quantity(sender);
            ((TextBox)dgSV.Rows[dgSV.Rows.Count - 2].FindControl("txtSalesPrice")).Focus();
        }
        catch (Exception ex)
        {
            //lblMessasge.Text = ex.Message;
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex.Message + "');", true);
        }
    }
    protected void txtRemarks_TextChanged(object sender, EventArgs e)
    {
        try
        {
            Quantity(sender);
            ((TextBox)dgSV.Rows[dgSV.Rows.Count - 2].FindControl("txtQty")).Focus();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
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
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                        "alert('" + Mgs + "');", true);
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
            dis = Convert.ToDouble(((TextBox)gvr.FindControl("txtDiscount")).Text);
            dr["Total"] = (tot - dis).ToString("N2");

        }

        dgSV.DataSource = dt;
        dgSV.DataBind();
        ShowFooterTotal(dt);
    }

    protected void txtSalesPrice_TextChanged(object sender, EventArgs e)
    {
        try
        {
            Quantity(sender);
            ((TextBox)dgSV.Rows[dgSV.Rows.Count - 2].FindControl("txtDiscount")).Focus();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }


    protected void txtDiscount_TextChanged(object sender, EventArgs e)
    {
        try
        {
            Quantity(sender);
            ((TextBox)dgSV.Rows[dgSV.Rows.Count - 1].FindControl("txtQty")).Focus();

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }

    protected void TotalDiscount_TextChange(object sender, EventArgs e)
    {
       VetAndDiscount();
    }
    protected void dgSV_SelectedIndexChanged1(object sender, EventArgs e)
    {

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
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Your session is over. Try it again!!');",
                true);
        }
    }

    protected void dgPaymentInfo_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[0].Attributes.Add("style", "display:none");

        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[0].Attributes.Add("style", "display:none");

        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Attributes.Add("style", "display:none");

        }
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

    protected void ddlPaymentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        //lblAcountNo.Visible = ddlBankName.Visible =
        //    lblBankNameTo.Visible = lblAcountNo.Visible = ddlAccountNo.Visible = lblAccNoPint.Visible = lblRcbBankPoint.Visible = false;
        //if (ddlPaymentTypeTo.SelectedValue != "1")
        //{
        //    ddlBankName.Visible = lblBankNameTo.Visible = lblAcountNo.Visible = ddlAccountNo.Visible = lblAccNoPint.Visible = lblRcbBankPoint.Visible = true;
        //    if (ddlPaymentTypeTo.SelectedValue == "2")
        //    {
        //        DataTable dtBankList =
        //            IdManager.GetShowDataTable(
        //                "Select t2.bank_name as bank_name,t2.bank_id from dbo.bank_info t2 where t2.Active=1 order by bank_name asc");
        //        util.PopulationDropDownList(ddlBankName, "bank_name", "bank_id", dtBankList);
        //    }

        //    else if (ddlPaymentTypeTo.SelectedValue == "5")
        //    {
        //        DataTable dtBankList =
        //            IdManager.GetShowDataTable(
        //                "Select t2.bank_name as bank_name,t2.bank_id from dbo.bank_info t2 where t2.Active=1 order by bank_name asc");
        //        util.PopulationDropDownList(ddlBankName, "bank_name", "bank_id", dtBankList);
        //    }

        //    else if (ddlPaymentTypeTo.SelectedValue == "3")
        //    {
        //        DataTable dtBankList =
        //            IdManager.GetShowDataTable(
        //               "Select t2.bank_name as bank_name,t2.bank_id from dbo.bank_info t2 where t2.Active=1 order by bank_name asc");
        //        util.PopulationDropDownList(ddlBankName, "bank_name", "bank_id", dtBankList);
        //    }

        //    else if (ddlPaymentTypeTo.SelectedValue == "4")
        //    {
        //        DataTable dtBankList =
        //            IdManager.GetShowDataTable(
        //            "Select t2.bank_name as bank_name,t2.bank_id from dbo.bank_info t2 where t2.Active=1 order by bank_name asc");

        //        util.PopulationDropDownList(ddlBankName, "bank_name", "bank_id", dtBankList);
        //    }


        //}
        //ddlBankName.SelectedIndex = -1;

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


    protected void btnPaymentAdd_Click(object sender, EventArgs e)
    {
        try
        {
            if (ddlPaymentTypeTo.SelectedValue == "" || ddlPaymentTypeTo.SelectedValue == "0")
            {

                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please Select  payment Type!!');", true);
                return;
            }
            if (string.IsNullOrEmpty(txtAmount.Text) || txtAmount.Text == "0")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                  "alert('Please Input Amount..!!');", true);
                txtAmount.Focus();
                return;
            }

            if (string.IsNullOrEmpty(ddlPaymentTypeTo.SelectedItem.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please Select  payment Type!!');", true);
                return;
            }
            if (string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    "alert('Please Input Amount..!!');", true);
                txtAmount.Focus();
                return;
            }
            if (Convert.ToDecimal(txtAmount.Text) <= 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    "alert('Please Input Amount..!!');", true);
                txtAmount.Focus();
                return;
            }

            if (!ddlPaymentTypeTo.SelectedValue.ToString().Equals("1"))
            {
                if (string.IsNullOrEmpty(ddlBankName.SelectedItem.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('Please select bank name.!!');", true);
                    return;
                }

                if (string.IsNullOrEmpty(ddlAccountNo.SelectedItem.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('Please account number.!!');", true);
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
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    "alert('Payment amount not upper then sale amount..!!');", true);
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
                        data["AccountNoFrom"] = txtAccountNo.Text;
                        data["BankIDFrom"] = ddlBankNameFrom.SelectedValue;
                        data["BankNameFrom"] = ddlBankNameFrom.SelectedItem.Text;

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
                    data["BankIDFrom"] = ddlBankNameFrom.SelectedValue;
                    data["BankNameFrom"] = ddlBankNameFrom.SelectedItem.Text;

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
        }
        catch
        {

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

        lblAcountNo.Visible = ddlBankName.Visible =
            lblBankNameTo.Visible = lblAcountNo.Visible =
                ddlAccountNo.Visible = lblAccNoPint.Visible = lblAccNoPint.Visible = false;

        lblAcountNoFrom.Visible = ddlBankNameFrom.Visible = lblBankNameFrom.Visible = lblStatus.Visible =
            lblApprovedDate.Visible = txtApprovedDate.Visible = ddlPaymentStatus.Visible =
                lblChekNo.Visible = txtcheeckNo.Visible = txtAccountNo.Visible = false;
        txtPayment.Text = "0";
        VetAndDiscount();

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
        // txtChequeAmount.Text = "0";
        // txtChequeAmount.Focus();
    }
    private void Clear()
    {
        Panel1.Visible = txtItemsCode.Enabled = true;

        lblCustomerName.Text = "";
        txtSubTotal.Text = txtVat.Text = txtDiscount.Text = txtPayment.Text = txtDue.Text = txtGrandTotal.Text = "0";
        ddlBankName.SelectedIndex = -1;

        getEmptyDtl();
        ViewState["SV"] = null;
        txtItemsCode.Focus();
    }
    protected void txtVat_TextChanged(object sender, EventArgs e)
    {
        VetAndDiscount();
    }
    protected void txtCode_TextChanged(object sender, EventArgs e)
    {
        bool IsChk = false;
        string[] splitCode = txtItemsCode.Text.Trim().Split('-');

        //string Code = IdManager.GetShowSingleValueString("Code",
        //    "ItemInformationForReport where upper(Name_Brand_Model)=upper('" + txtItemsCode.Text + "')");

        DataTable dtItemsSearch = _aItemManager.GetSearchItemsOnStock(txtItemsCode.Text.ToUpper(), splitCode.Length);
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
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Already this Item Qty is Over!!');", true);
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


            totalPayment = Convert.ToDecimal(txtPayment.Text) + Convert.ToDecimal(lblTotal.Text);
        }
        catch (Exception ex)
        {
            totalPayment = Convert.ToDecimal(lblTotal.Text);
        }   
        this.txtExAmount.Text = this.lblTotal.Text;
        txtGrandTotal.Text = (((subtotal - discount) + ((subtotal - discount - Convert.ToDecimal(txtExAmount.Text)) * vat / 100)) - Convert.ToDecimal(lblTotal.Text)).ToString("N2");
        
        //  txtAmount.Text = (((subtotal - discount) + (subtotal * vat / 100)) - Convert.ToDecimal(lblTotal.Text)).ToString("N2");
        //txtPayment.Text = (((subtotal - discount) + (subtotal * vat / 100))).ToString("N2");

        txtDue.Text = (((subtotal - discount) + ((subtotal - discount - Convert.ToDecimal(txtExAmount.Text)) * vat / 100)) - totalPayment).ToString("N1");

       // txtLastFigarAmount.Text = txtPayment.Text;
        // PointCalculation();
        // UpItemsDetails.Update();
    }
    //private void VetAndDiscount()
    //{
    //    decimal subtotal, vat, discount, totalPayment;
    //    try
    //    {
    //        subtotal = Convert.ToDecimal(txtSubTotal.Text);
    //    }
    //    catch (Exception ex)
    //    {
    //        subtotal = 0;
    //    }

    //    try
    //    {
    //        vat = Convert.ToDecimal(txtVat.Text);
    //    }
    //    catch (Exception ex)
    //    {
    //        vat = 0;
    //    }

    //    try
    //    {
    //        discount = Convert.ToDecimal(txtDiscount.Text);
    //    }
    //    catch (Exception ex)
    //    {
    //        discount = 0;
    //    }

    //    try
    //    {
    //        totalPayment = Convert.ToDecimal(txtPayment.Text) + Convert.ToDecimal(lblTotal.Text);
    //    }
    //    catch (Exception ex)
    //    {
    //        totalPayment = Convert.ToDecimal(lblTotal.Text);
    //    }

    //    txtGrandTotal.Text = (((subtotal - discount) + (subtotal * vat / 100)) - totalPayment).ToString("N2");
    //    txtExAmount.Text = lblTotal.Text;
    //   // txtDue.Text = (((subtotal + (subtotal * vat / 100)) - discount) - totalPayment).ToString("N2");
    //    txtDue.Text = (((subtotal - discount) + (subtotal * vat / 100)) - totalPayment).ToString("N2");
    //    // UpItemsDetails.Update();
    //}
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
            txtAmount.Text = return_value;
        }
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
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('This Amount Is Not Right..!!!!');", true);

            }


            txtAmount.Text = return_value;
        }
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
    protected void dgSV_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void txtAmount_TextChanged(object sender, EventArgs e)
    {
        try
        {
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
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                    "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                    "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }
   

       protected void btnSave_Click(object sender, EventArgs e)
       {
        try
        {
            //UPCustomer.Update();
            DataTable dtSalesDetails = (DataTable)ViewState["SV"];
            DataTable dtPayment = (DataTable) ViewState["paymentInfo"];

            var GrandTotal = Convert.ToDecimal(txtGrandTotal.Text) + Convert.ToDecimal(lblTotal.Text);

           
            var ReturnGrandTotal = Convert.ToDecimal(lblTotal.Text);

            int IsExistCount =SalesManager.IsExistExchange(hfOrderId.Value);
            if (IsExistCount>0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('The Same Invoice Already Exchange..!!');",
                    true);
                return; 
            }

            if (Convert.ToDecimal(lblTotal.Text)<=0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('First Select Exchange Item..!!');",
                    true);
                return;
            }
            if (ReturnGrandTotal>GrandTotal)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Exchange Amount Is Not more then Total Amount..!!');",
                    true);
                return;
            }


            if (dtSalesDetails == null)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('No Items added in the list..!!');",
                    true);
                return;
            }
            if (string.IsNullOrEmpty(txtInvoiceNo.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('input invoice No.!!');",
                    true);
                return;
            }
            if (dtSalesDetails.Rows.Count <= 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('No Items added in the list..!!');",
                    true);
                return;
            }

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
            //if (Convert.ToDouble(txtDue.Text) > 0 )
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Local customer must be full payment..!!');",
            //        true);
            //    return;
            //}
            //if (Convert.ToDouble(txtDue.Text) > 0 )
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Local customer must be full payment..!!');",
            //        true);
            //    return;
            //}

            if (Convert.ToDouble(txtDue.Text) > 0 && hfCommonCus.Value == "1")
            {
                 ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Local customer must be full payment..!!');",
                    true);
                 return;
            }

            if (Convert.ToDouble(txtDue.Text) < 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Please check your payment status.clear and set again.!!');",
                    true);
                return;
            }

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
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                        "alert('This invoice already exist in database.!!');",
                        true);
                    return;
                }

                Sales aSales = SalesManager.GetBranchShowSalesInfo(lblInvNo.Text,Session["BranchId"].ToString());
                Dis = Convert.ToDouble(txtDiscount.Text);
             
                if (aSales != null)
                {
                    if (per.AllowEdit == "Y")
                    {
                        aSales.ID = lblInvNo.Text;
                        aSales.BranchId = Session["BranchId"].ToString();
                        aSales.Invoice = txtInvoiceNo.Text.ToUpper();
                        aSales.Date = txtDate.Text;
                        aSales.Total = txtSubTotal.Text.Replace(",", "");
                        aSales.Tax = txtVat.Text.Replace(",", "");
                        aSales.Disount = txtDiscount.Text.Replace(",", "");
                        aSales.GTotal = txtGrandTotal.Text.Replace(",", "");
                        aSales.CReceive = txtPayment.Text.Replace(",", "");
                        aSales.Due = txtDue.Text.Replace(",", "");
                        try
                        {
                            aSales.ExchangeAmount = Convert.ToDecimal(this.txtExAmount.Text);

                        }
                        catch 
                        {
                            aSales.ExchangeAmount = 0;

                        }
                        aSales.paymentID = ddlPaymentTypeTo.SelectedValue;
                        //if (string.IsNullOrEmpty(txtCustomerName.Text))
                        //{
                        //    DataTable dtCustomerDtl = _aclsClientInfoManager.GetCustomerOnSearch("", 5);
                        //    if (dtCustomerDtl != null)
                        //    {
                        //        txtCustomerName.Text = dtCustomerDtl.Rows[0]["ContactName"].ToString();
                        //        txtRemarks.Text = "Items Sales By Customer : " + txtCustomerName.Text +
                        //                          " for Local Customer (" + txtLocalCustomer.Text + ")";
                        //        hfCustomerCoa.Value = dtCustomerDtl.Rows[0]["Gl_CoaCode"].ToString();
                        //        hfCustomerID.Value = dtCustomerDtl.Rows[0]["ID"].ToString();
                        //    }
                        //}


                        //aSales.Customer = hfCustomerID.Value;
                        //aSales.CustomerCoa = hfCustomerCoa.Value;

                        //aSales.LocalCustomer = txtLocalCustomer.Text.Replace("'", "");
                        //aSales.LocalCusPhone = txtLocalCusPhone.Text.Replace("'", "");
                        //aSales.LocalCusAddress = txtLocalCusAddress.Text.Replace("'", "");
                        aSales.Note = "";
                        aSales.OrderType = "C";
                        //  aSales.GuarantorID = lblguarantorID.Text;
                       // aSales.DvStatus = ddlDelevery.SelectedValue;
                        aSales.DvDate = txtDate.Text;
                        // aSales.TotalInstallment = txtInstallmentNumber.Text;
                        // aSales.installmentDate = txtpaydate.Text;
                        aSales.Remarks = "";
                        aSales.BankCoaCode = "";
                        //if (ddlBankName.SelectedValue != null && ddlBankName.SelectedValue != "" &&
                        //         ddlBankName.SelectedValue != "0")
                        //{
                        //    aSales.BankCoaCode = IdManager.GetShowSingleValueString("Gl_Code", "ID",
                        //        "dbo.bank_branch", ddlBankName.SelectedValue);
                        //}
                        //else
                        //{
                        //    aSales.BankCoaCode = "";

                        //}
                        //aSales.PMethod = ddlPaymentTypeTo.SelectedValue;
                        //if (ddlPaymentTypeTo.SelectedValue == "3")
                        //{
                        //    aSales.BankId = ddlBankName.SelectedValue;
                        //    aSales.PMNumber = ddlAccountNo.SelectedValue;
                        //}
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

                        aSales.CustomerName = "";
                        aSales.LoginBy = Session["userID"].ToString();
                        

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
                            vmst.Particulars = "";
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
                            vmstCV.Particulars = "";
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
                                vmstCV.Particulars ="";
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
                            vmstTx.Particulars = "Total Vat on " +"";
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
                            vmstTx.Particulars = "Total Vat on " +"";
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
                        SalesManager.UpdateSalesInfo(aSales, dtSalesDetails, dtOldSalesDetails, dtPayment, vmst, vmstCV, vmstTx);
                       // PosPrint();
                       
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('You are not Permitted this Step...!!');", true);
                    }
                }
                else
                {
                    if (ViewState["SV"] == null)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                            "alert('No Items In this list...!!');", true);
                    }
                   
                    else
                    {
                        if (per.AllowAdd == "Y")
                        {
                            int CountInv = IdManager.GetShowSingleValueInt("[InvoiceNo]",
                                "[Order] where UPPER([InvoiceNo])='" + txtInvoiceNo.Text.ToUpper() + "' ");
                            if (CountInv > 0)
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                                    "alert('This invoice already exist in database.!!');",
                                    true);
                                return;
                            }

                            aSales = new Sales();
                            txtInvoiceNo.Text = "INV-RD-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() +
                                                DateTime.Now.Day.ToString() + "-00" +
                                                IdManager.GetShowSingleValueString("InvoiceID", "FixGlCoaCode");
                            aSales.Invoice = txtInvoiceNo.Text.ToUpper();
                            aSales.Date = txtDate.Text;
                            aSales.paymentID = ddlPaymentTypeTo.SelectedValue;
                            aSales.Total = txtSubTotal.Text.Replace(",", "");
                            aSales.Tax = txtVat.Text.Replace(",", "");
                            aSales.Disount = txtDiscount.Text.Replace(",", "");
                            aSales.GTotal = txtGrandTotal.Text.Replace(",", "");
                            aSales.CReceive = txtPayment.Text.Replace(",", "");
                           
                           // aSales.Customer = dtCustomerDtl.Rows[0]["ID"].ToString();
                            aSales.OrderType = "C";
                            //  aSales.GuarantorID = lblguarantorID.Text;
                           // aSales.DvStatus = ddlDelevery.SelectedValue;
                            aSales.DvDate = txtDate.Text;
                            
                            aSales.ExtraAmount ="0";


                            try
                            {
                                aSales.ExchangeAmount = Convert.ToDecimal(this.txtExAmount.Text);

                            }
                            catch
                            {
                                aSales.ExchangeAmount = 0;

                            }

                            //aSales.PMethod = ddlPaymentTypeTo.SelectedValue;
                            //if (ddlPaymentTypeTo.SelectedValue == "3")
                            //{
                            //    aSales.BankId = ddlBankName.SelectedValue;
                            //    aSales.PMNumber = ddlAccountNo.SelectedValue;
                            //}
                            // aSales.TotalInstallment = txtInstallmentNumber.Text;
                            // aSales.installmentDate = txtpaydate.Text;
                            
                            //aSales.PMethod = ddlPaymentTypeFrom.SelectedValue;
                            //aSales.BankId = ddlBankNameFrom.SelectedValue;
                            //if (string.IsNullOrEmpty(txtCustomerName.Text))
                            //{
                                //DataTable dtCustomerDtl = _aclsClientInfoManager.GetCustomerOnSearch("", 5);
                                //if (dtCustomerDtl != null)
                                //{
                                ////    txtCustomerName.Text = dtCustomerDtl.Rows[0]["ContactName"].ToString();
                                ////    txtRemarks.Text = "Items Sales By Customer : " + ""+
                                ////                      " for Local Customer (" +"" + ")";
                                ////    hfCustomerCoa.Value = dtCustomerDtl.Rows[0]["Gl_CoaCode"].ToString();
                                ////    hfCustomerID.Value = dtCustomerDtl.Rows[0]["ID"].ToString();
                                //}
                           // }







                           //Ridoy
                            aSales.Remarks = hfRemark.Value;
                            hfRemark.Value = "";
                            aSales.Customer = hfCustomerID.Value;
                            aSales.CustomerCoa = hfGl_CoaCode.Value;
                            hfGl_CoaCode.Value = "";
                            aSales.LocalCustomer = hfLocalCustomer.Value;
                            hfLocalCustomer.Value = "";
                            aSales.LocalCusPhone = hfLocalCusPhone.Value;
                            hfLocalCusPhone.Value = "";
                            aSales.LocalCusAddress = hfLocalCusAddress.Value;
                            hfLocalCusAddress.Value = "";
                            aSales.Note = hfNote.Value;
                            hfNote.Value = "";
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
                            aSales.CustomerName = hfCustomerName.Value;
                            hfCustomerName.Value = "";
                            aSales.LoginBy = Session["userID"].ToString();
                            aSales.BranchId = Session["BranchId"].ToString();
                            //Ridoy


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

                            aSales.CRefund = refund.ToString("N2");

                            aSales.Due = due.ToString("N2");

                            //Database Table(OrderExchange) Head Direct Tk no (%) or avrage  
                            SalesExchangeModel _salesExchange=new SalesExchangeModel();
                            _salesExchange.GrandTotal = Convert.ToDecimal(lblTotal.Text);
                            _salesExchange.DiscountAmount = (Convert.ToDecimal(lblSubTotal.Text) -
                                                             Convert.ToDecimal(lblTotal.Text)) +
                                                            Convert.ToDecimal(lblVat.Text);
                              
                            _salesExchange.InvoiceNo = hfInvoiceNo.Value;
                            _salesExchange.NewInvoiceNo = txtInvoiceNo.Text;
                            _salesExchange.SubTotal = Convert.ToDecimal(hfsubTotal.Value);
                            _salesExchange.TaxAmount = Convert.ToDecimal(lblVat.Text);
                            







                            //*************************** Account Entry ******************//
                            //********* Jurnal Voucher *********//
                            VouchMst vmst = new VouchMst();
                            vmst.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
                            vmst.ValueDate = txtDate.Text;
                            vmst.VchCode = "03";
                            vmst.RefFileNo = "";
                            vmst.VolumeNo = "";
                            vmst.SerialNo = txtInvoiceNo.Text.Trim();
                            vmst.Particulars = "Account payable To " +"" + " , " + "";
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
                            vmstCR.Particulars ="";
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
                            vmstTax.Particulars = "Total Vat on " + "";
                            vmstTax.ControlAmt = txtVat.Text.Replace(",", "");
                            vmstTax.Payee = "SVSLTXT";
                            vmstTax.CheckNo ="";
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













                            //Order Change qty

                            DataTable dt1 = new DataTable();
                            DataRow dr1;
                            dt1.Columns.Add(new System.Data.DataColumn("ChangeQty", typeof(int)));
                            dt1.Columns.Add(new System.Data.DataColumn("ItemId", typeof(int)));
                            dt1.Columns.Add(new System.Data.DataColumn("ItemCode", typeof(string)));
                            dt1.Columns.Add(new System.Data.DataColumn("UnitPrice", typeof(decimal)));
                            dt1.Columns.Add(new System.Data.DataColumn("SalePrice", typeof(decimal)));
                            dt1.Columns.Add(new System.Data.DataColumn("TotalPrice", typeof(decimal)));
                            dt1.Columns.Add(new System.Data.DataColumn("Barcode", typeof(string)));
                           
                            foreach (GridViewRow tt in dgSV1.Rows)
                            {
                                TextBox txtChangeQty = (TextBox)tt.FindControl("txtChangeQty");
                                int changeQty = Convert.ToInt32(txtChangeQty.Text);
                                TextBox txtItemId = (TextBox)tt.FindControl("txtItemId");
                                int itmeId = Convert.ToInt32(txtItemId.Text);
                                TextBox txtItemCode = (TextBox)tt.FindControl("txtItemCode");
                                TextBox txtCostPrice = (TextBox)tt.FindControl("txtCostPrice");
                                TextBox txtSalesPrice = (TextBox)tt.FindControl("txtSalesPrice");
                                TextBox txtBarcode = (TextBox)tt.FindControl("txtBarcode");

                                
                                dr1 = dt1.NewRow();
                                dr1[0] = changeQty;
                                dr1[1] = itmeId;
                                dr1[2] = txtItemCode.Text;
                                dr1[3] = txtCostPrice.Text;
                                dr1[4] = txtSalesPrice.Text;
                                dr1[5] = Convert.ToDecimal(txtSalesPrice.Text)*changeQty;
                                dr1[6] =txtBarcode.Text;
                                dt1.Rows.Add(dr1);
                            }

                            var ab = dt1;










                            int SalesID = SalesManager.SaveSalesExchangeInfo(aSales, dtSalesDetails, dtPayment, vmst, vmstCR, vmstTax, dt1, hfOrderId.Value, _salesExchange);

                            if (SalesID>0)
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                    "alert('Save Success...!!');", true);
                                ClearPage();
                            }
                            else
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                    "alert('Save Fail...!!');", true);
                            }
                          // lblInvNo.Text = SalesID.ToString();

                            //PosPrint();
                            
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                "alert('You are not Permitted this Step...!!');", true);
                        }
                    }
                }
            }
        }
        catch (FormatException fex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex.Message + "');", true);
        }
    }

        public void ClearPage()
        {
            Response.Redirect("frmSalesExchange.aspx");
            ViewState["SV1"] = "";
            ViewState["OldSV1"] ="";
            getEmptyDtl();
        }

        protected void CloseButton_Click(object sender, EventArgs e)
        {
            ClearPage();
        }



        protected void txtSearchInvoice_TextChanged(object sender, EventArgs e)
        {

        }
}