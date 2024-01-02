using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Delve;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System.Data.SqlClient;
using System.Runtime.InteropServices.ComTypes;

public partial class BranchSalesReturn : System.Web.UI.Page
{
    private static DataTable dtmsr = new DataTable();
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
                            Session["BranchId"] = dReader["BranchId"].ToString();
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
                Response.Redirect("BranchHome.aspx?sid=sam");
            }
        }
        catch
        {
            Response.Redirect("Home.aspx?sid=sam");

        }
        if (!IsPostBack)
        {
            try
            {
                var BranchId = Session["BranchId"].ToString();
                if (string.IsNullOrEmpty(BranchId))
                {
                    Response.Redirect("Home.aspx?sid=sam");

                }
                else
                {



                ViewState["PVID"] = "";
                Session["purdtl"] = null;
                ViewState["OldRtnList"] = null;
                getEmptyDtl();
                txtReturnDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                DataTable dt1 = IdManager.GetShowDataTable("SELECT * FROM [FixGlCoaCode]");
                Session["Cash_Code"] = dt1.Rows[0]["CashInHand_BD"].ToString();
                Session["Cash_Name"] = dt1.Rows[0]["CashName_BD"].ToString();
                txtGoodsReceiveNo.Enabled = txtSupplier.Enabled = txtReturnNO.Enabled = txtRemarks.Enabled = false;
                dgPRNMst.Visible = true;
                btnSave.Enabled = true;
                PVIesms_UP.Visible = false;
                btnNew.Visible = true;
                VisiblePayment(false, false, false, false, false, false, false, false);
                txtTotal.Text = txtTotPayment.Text = txtDue.Text = "0";

                string query2 =
                    "select '' [bank_id],'' [bank_name]  union select convert(nvarchar,[ID]) ,BankBranch_AccName AS [bank_name] from [View_Bank_Branch_Info] order by 1";
                util.PopulationDropDownList(ddlBank, "bank_info", query2, "bank_name", "bank_id");

                Users usr = Delve.UsersManager.getUser(Session["user"].ToString());
                if (usr != null)
                {
                    lblBranchID.Text = BranchId;
                    lblBranchName.Text = "Branch Name : " +
                                         IdManager.GetShowSingleValueString("BranchName", "ID", "BranchInfo", BranchId);
                    DataTable dt = SaleReturnManager.GetBranchShowSalesReturnItems(BranchId);
                    ViewState["mst"] = dt;
                  
                    dgPRNMst.DataSource = dt;
                    dgPRNMst.DataBind();
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                        "alert('Set branch this user.!!!');", true);
                    return;
                }

                // double CurrencyRate = BankAndCashBlanceCheck.GetCurrency(btnSave, txtReturnDate, 0);
                //  ViewState["CurrencyRate"] = CurrencyRate;
                //Session["UserType"] = IdManager.GetShowSingleValueString(" t.UserType", "t.USER_NAME", "UTL_USERINFO t",
                //    Session["user"].ToString());
                if (Session["InvoiceNo"] != null)
                {
                    btnNew_Click(sender, e);
                    txtGoodsReceiveNo_TextChanged(sender, e);
                    //Session["InvoiceNo"] = null;
                }
                //************** Find SalesReturn ************//
             
                if (Session["SalesReturn"] != null)
                {
                    
                    lbLId.Text = Session["SalesReturn"].ToString();
                
                    FindReturn();
                    Session["SalesReturn"] = null;
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
                Response.Redirect("Home.aspx?sid=sam");


            }

        }
    }
    private void getEmptyDtl()
    {
        dgPODetailsDtl.Visible = true;
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("ID", typeof(string));
        dtDtlGrid.Columns.Add("item_code", typeof(string));
        dtDtlGrid.Columns.Add("Barcode", typeof(string));
        dtDtlGrid.Columns.Add("item_desc", typeof(string));
        dtDtlGrid.Columns.Add("msr_unit_code", typeof(string));
        dtDtlGrid.Columns.Add("item_rate", typeof(string));
        dtDtlGrid.Columns.Add("qnty", typeof(string));
        dtDtlGrid.Columns.Add("salqnty", typeof(string));
        dtDtlGrid.Columns.Add("PvUnitPrice", typeof(string));
        dtDtlGrid.Columns.Add("Type", typeof(string));
        dtDtlGrid.Columns.Add("ItemsID", typeof(string));
        dtDtlGrid.Columns.Add("FixdiscountAmt", typeof(string));
        dtDtlGrid.Columns.Add("discountAmt", typeof(string));
        dtDtlGrid.Columns.Add("TaxAmount", typeof(string));
        dtDtlGrid.Columns.Add("discountAmtfix", typeof(string));
        dtDtlGrid.Columns.Add("TaxAmountfix", typeof(string));
        
        DataRow dr = dtDtlGrid.NewRow();
        dr["qnty"] = "0";
        dtDtlGrid.Rows.Add(dr);
        dgPODetailsDtl.DataSource = dtDtlGrid;
        Session["purdtl"] = dtDtlGrid;
        dgPODetailsDtl.DataBind();
    }

    protected void txtGoodsReceiveNo_TextChanged(object sender, EventArgs e)
    {

        var BranchId = Session["BranchId"].ToString();
        ViewState["PVID"] = "";
        lblGlCoa.Text = "";
        DataTable dt = null;
        if (Session["InvoiceNo"] != null)
        {
            dt = SaleReturnManager.GetBranchShowSLMasterInfo(Session["InvoiceNo"].ToString(),BranchId);
            txtGoodsReceiveNo.Text = Session["InvoiceNo"].ToString();
        }
        else
        {
            dt = SaleReturnManager.GetBranchShowSLMasterInfo(txtGoodsReceiveNo.Text,BranchId);
        }
        if (dt.Rows.Count <= 0)
        {
            txtGoodsReceiveNo.Text = "";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
               "alert('Not Find This Invoice Data .!!');", true);
            return;
        }

        int IsExistExch = SalesManager.IsExistExchangeInvoice(dt.Rows[0]["InvoiceNo"].ToString(), Session["BranchId"].ToString());
        int IsExistReturn = SalesManager.IsExistExchangeReturn(dt.Rows[0]["ID"].ToString(), Session["BranchId"].ToString());
        if (IsExistExch > 0 || IsExistReturn > 0)
        {
            this.txtGoodsReceiveNo.Text = "";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                "alert('This Invoice Is  already Exchenge.!!');", true);
            return;
        }

        if (dt.Rows.Count > 0)
        {
            txtSupplier.Text = dt.Rows[0]["ContactName"].ToString();
            ViewState["PVID"] = dt.Rows[0]["ID"].ToString();
            lblPVID.Text = dt.Rows[0]["ID"].ToString();
            lblGlCoa.Text = dt.Rows[0]["Gl_CoaCode"].ToString();
            lblSupID.Text = dt.Rows[0]["CustomerID"].ToString();
            var data1 = (DataTable) Session["purdtl"];
            dgPODetailsDtl.DataSource = Session["purdtl"];
            dgPODetailsDtl.DataBind();
            PVIesms_UP.Update();
            txtRemarks.Focus();
            Session["InvoiceNo"] = null;
            txtRemarks.Text = "Items Return , Customer Name : " + txtSupplier.Text;
        }
        else
        {
            txtGoodsReceiveNo.Text = string.Empty;
            txtGoodsReceiveNo.Focus();
        }
    }
    public DataTable PopulatePayType()
    {

       
        DataTable dt = SaleReturnManager.GetSalesReturnItems(ViewState["PVID"].ToString(), Session["BranchId"].ToString());
        DataRow dr = dt.NewRow();
        dt.Rows.InsertAt(dr, 0);
        return dt;
    }
    protected void btnNew_Click(object sender, EventArgs e)
    {
        dgPRNMst.Visible = false;
        PVIesms_UP.Visible = true;
        txtGoodsReceiveNo.Text = txtSupplier.Text = txtReturnNO.Text = txtRemarks.Text = "";
        txtGoodsReceiveNo.Enabled = txtSupplier.Enabled = txtRemarks.Enabled = true;
        //txtReturnNO.Text = IdManager.GetDateTimeWiseSerial("IVRN", "Return_No", "[OrderReturn]");
        txtGoodsReceiveNo.Focus();
        btnNew.Visible = false;
        txtReturnNO.Text = GetAutoId();
    }
    public string GetAutoId()
    {
        Users usr = Delve.UsersManager.getUser(Session["user"].ToString());
        string AutoID = "";
        if (usr != null)
        {
            lblBranchID.Text = usr.Dept;
            int ID = IdManager.GetShowSingleValueInt("ReturnFixID", "FixGlCoaCode");
            AutoID = "IVRN-" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "-0" + ID + Session["BranchId"].ToString();
        }
        return AutoID;
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
         try
        {
            if (txtGoodsReceiveNo.Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Select Goods Receive No...!!');", true);
                return;
            }
            else if (string.IsNullOrEmpty(txtRemarks.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Input Remarks/Particulars.!!');", true);
                return;
            }
            else if (Session["purdtl"] == null)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There are no items in list.!!');", true);
                return;
            }
            else
            {
                SaleReturn rtn = SaleReturnManager.getBranchShowRetirnItems(lbLId.Text, Session["BranchId"].ToString());
              
               

                if (rtn != null)
                {
                    if (per.AllowEdit == "Y")
                    {
                        rtn.ReturnDate = txtReturnDate.Text;
                        rtn.Remarks = txtRemarks.Text.Replace("'", "");
                        rtn.SupplierName = txtSupplier.Text;
                        rtn.CustomerID = lblSupID.Text;
                        rtn.LogonBy = Session["user"].ToString();
                        rtn.TotalAmount = txtTotal.Text.Replace(",", "");
                        rtn.Pay_Amount = txtTotPayment.Text.Replace(",", "");
                        rtn.PaymentMethod = ddlPaymentMethord.SelectedValue.Trim();
                        rtn.BankName = ddlBank.SelectedValue.Trim();
                        rtn.ChequeNo = txtChequeNo.Text.Trim();
                        rtn.ChequeDate = txtChequeDate.Text;
                        rtn.Chk_Status = ddlChequeStatus.SelectedValue.Trim();
                        rtn.BranchID = lblBranchID.Text;
                        rtn.Due = txtDue.Text;
                        rtn.BranchID = Session["BranchId"].ToString();
                        double ReturnBlance = 0;
                        DataTable dtOldList = (DataTable)ViewState["OldRtnList"];
                        DataTable dt = (DataTable)Session["purdtl"];
                        foreach (DataRow drRow in dt.Rows)
                        {
                            if (!string.IsNullOrEmpty(drRow["item_desc"].ToString()))
                            {
                                ReturnBlance += Convert.ToDouble(drRow["PvUnitPrice"].ToString()) *
                                                Convert.ToDouble(drRow["qnty"].ToString());
                            }
                        }
                        //********************** Journal Voucher Update *************//

                        string VCH_SYS_NO = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                            "t1.PAYEE='IR' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                            txtReturnNO.Text);
                        VouchMst vmst = VouchManager.GetVouchMst(VCH_SYS_NO.Trim());
                        if (vmst != null)
                        {
                            vmst.FinMon = FinYearManager.getFinMonthByDate(rtn.ReturnDate);
                            vmst.ValueDate = rtn.ReturnDate;
                            vmst.VchCode = "03";
                            //vmst.SerialNo = txtGRNO.Text.Trim();
                            vmst.Particulars = txtRemarks.Text;
                            vmst.ControlAmt = rtn.TotalAmount.Replace(",", "");
                            //vmst.Payee = "IR";
                            vmst.CheqAmnt = "0";
                            vmst.UpdateUser = Session["user"].ToString().ToUpper();
                            vmst.UpdateDate = DateTime.Now.ToString("dd/MM/yyyy");
                            vmst.AuthoUserType = Session["userlevel"].ToString();
                        }
                        string VCH_SYS_NO_PVCV = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                            "t1.PAYEE='IRDV' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                            txtReturnNO.Text);
                        VouchMst vmstDV = VouchManager.GetVouchMst(VCH_SYS_NO_PVCV.Trim());
                        if (vmstDV != null)
                        {
                            vmstDV.FinMon = FinYearManager.getFinMonthByDate(txtReturnDate.Text);
                            vmstDV.ValueDate = txtReturnDate.Text;
                            vmstDV.RefFileNo = "";
                            vmstDV.VchCode = "01";
                            //vmst.SerialNo = txtGRNO.Text.Trim();
                            vmstDV.Particulars = txtRemarks.Text;
                            vmstDV.ControlAmt = txtTotPayment.Text.Replace(",", "");
                            //vmst.Payee = "PV";
                            vmstDV.CheqAmnt = "0";
                            vmstDV.UpdateUser = Session["user"].ToString().ToUpper();
                            vmstDV.UpdateDate = DateTime.Now.ToString("dd/MM/yyyy");
                            vmstDV.AuthoUserType = Session["userlevel"].ToString();
                        }
                        else
                        {
                            if (Convert.ToDecimal(txtTotPayment.Text) > 0)
                            {
                                vmstDV = new VouchMst();
                                vmstDV.FinMon = FinYearManager.getFinMonthByDate(txtReturnDate.Text);
                                vmstDV.ValueDate = txtReturnDate.Text;
                                vmstDV.VchCode = "01";
                                vmstDV.RefFileNo = "New";
                                vmstDV.VolumeNo = "";
                                vmstDV.SerialNo = txtReturnNO.Text.Trim();
                                vmstDV.Particulars = txtRemarks.Text;
                                vmstDV.ControlAmt = txtTotPayment.Text.Replace(",", "");
                                vmstDV.Payee = "IRDV";
                                vmstDV.CheckNo = txtChequeNo.Text;
                                vmstDV.CheqDate = txtChequeDate.Text;
                                vmstDV.CheqAmnt = "0";
                                vmstDV.MoneyRptNo = "";
                                vmstDV.MoneyRptDate = "";
                                vmstDV.TransType = "R";
                                vmstDV.BookName = "AMB";
                                vmstDV.EntryUser = Session["user"].ToString();
                                vmstDV.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
                                vmstDV.Status = "A";
                                vmstDV.AuthoUserType = Session["userlevel"].ToString();
                                vmstDV.VchSysNo =
                                    IdManager.GetNextID("gl_trans_mst", "vch_sys_no")
                                        .ToString();
                                vmstDV.VchRefNo = "DV-" + vmstDV.VchSysNo.ToString().PadLeft(10, '0');
                            }
                        }
                        SaleReturnManager.UpdateBranchSalesReturn(rtn, dt, dtOldList, ReturnBlance, vmst, vmstDV, lblGlCoa.Text);
                        btnSave.Enabled = false;
                        btnDelete.Enabled = false;
                        ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record are update successfully...!!');", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                    }
                }
                else
                {
                    if (per.AllowAdd == "Y")
                    {
                        rtn = new SaleReturn();
                        rtn.BranchID = Session["BranchId"].ToString();
                        rtn.GRN = lblPVID.Text;
                        rtn.CustomerID = lblSupID.Text;
                        rtn.ReturnDate = txtReturnDate.Text;
                        rtn.Remarks = txtRemarks.Text.Replace("'", "");
                        rtn.LogonBy = Session["user"].ToString();
                        rtn.TotalAmount = txtTotal.Text.Replace(",", "");
                        rtn.Pay_Amount = txtTotPayment.Text.Replace(",", "");
                        rtn.SupplierID = lblSupID.Text;
                        rtn.SupplierName = txtSupplier.Text;
                        rtn.PaymentMethod = ddlPaymentMethord.SelectedValue.Trim();
                        rtn.Due = txtDue.Text;
                        rtn.BranchID = Session["BranchId"].ToString();
                        if (!ddlPaymentMethord.SelectedValue.Equals("C"))
                        {
                            rtn.BankName = ddlBank.SelectedValue.Trim();
                            rtn.BankCoaCode = IdManager.GetShowSingleValuestring("Gl_Code",
                                " View_Bank_Branch_Info where ID='" + ddlBank.SelectedValue + "' ");
                            rtn.bankNameDes = ddlBank.SelectedItem.Text;
                            rtn.ChequeNo = txtChequeNo.Text.Trim();
                            rtn.ChequeDate = txtChequeDate.Text;
                            rtn.Chk_Status = ddlChequeStatus.SelectedValue.Trim();
                        }
                        else
                        {
                            rtn.BankName = "";
                            rtn.BankCoaCode = "";
                            rtn.ChequeNo = "";
                            rtn.Chk_Status = "";
                        }

                        txtReturnNO.Text = GetAutoId();
                        
                        rtn.Return_No = GetAutoId();
                        double ReturnBlance = 0;
                        DataTable dt = (DataTable)Session["purdtl"];
                        foreach (DataRow drRow in dt.Rows)
                        {
                            if (!string.IsNullOrEmpty(drRow["item_desc"].ToString()))
                            {
                                ReturnBlance += Convert.ToDouble(drRow["PvUnitPrice"].ToString()) *
                                                Convert.ToDouble(drRow["qnty"].ToString());
                            }
                        }
                        if (ReturnBlance <= 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('First Entry Return Qty Item...!!');", true);
                            return;
                        }
                        //*************************** Account Entry ******************//
                        //********* Jurnal Voucher *********//
                        VouchMst vmst = new VouchMst();
                        vmst.FinMon = FinYearManager.getFinMonthByDate(txtReturnDate.Text);
                        vmst.ValueDate = txtReturnDate.Text;
                        vmst.VchCode = "03";
                        vmst.RefFileNo = "";
                        vmst.VolumeNo = "";
                        vmst.SerialNo = txtReturnNO.Text.Trim();
                        vmst.Particulars = txtRemarks.Text;
                        vmst.ControlAmt = txtTotal.Text.Replace(",", "");
                        vmst.Payee = "IR";
                        vmst.CheckNo = txtChequeNo.Text;
                        vmst.CheqDate = txtChequeDate.Text;
                        vmst.CheqAmnt = "0";
                        vmst.MoneyRptNo = "";
                        vmst.MoneyRptDate = "";
                        vmst.TransType = "R";
                        vmst.BookName = "AMB";
                        vmst.EntryUser = Session["user"].ToString();
                        vmst.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
                        vmst.Status = "U";
                        vmst.AuthoUserType = Session["userlevel"].ToString();
                        vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
                        vmst.VchRefNo = "JV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');

                        //********* Devit Voucher *********//

                        VouchMst vmstDV = new VouchMst();
                        vmstDV.FinMon = FinYearManager.getFinMonthByDate(txtReturnDate.Text);
                        vmstDV.ValueDate = txtReturnDate.Text;
                        vmstDV.VchCode = "01";
                        vmstDV.RefFileNo = "";
                        vmstDV.VolumeNo = "";
                        vmstDV.SerialNo = txtReturnNO.Text.Trim();
                        vmstDV.Particulars = txtRemarks.Text;
                        vmstDV.ControlAmt = txtTotal.Text.Replace(",", "");
                        vmstDV.Payee = "IRDV";
                        vmstDV.CheckNo = txtChequeNo.Text;
                        vmstDV.CheqDate = txtChequeDate.Text;
                        vmstDV.CheqAmnt = "0";
                        vmstDV.MoneyRptNo = "";
                        vmstDV.MoneyRptDate = "";
                        vmstDV.TransType = "R";
                        vmstDV.BookName = "AMB";
                        vmstDV.EntryUser = Session["user"].ToString();
                        vmstDV.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
                        vmstDV.Status = "A";
                        vmstDV.AuthoUserType = Session["userlevel"].ToString();
                        vmstDV.VchSysNo = (Convert.ToInt64(IdManager.GetNextID("gl_trans_mst", "vch_sys_no")) + 1)
                            .ToString();
                        vmstDV.VchRefNo = "DV-" + vmstDV.VchSysNo.ToString().PadLeft(10, '0');

                        int ID = SaleReturnManager.BranchSaveInvoiceReturn(rtn, dt, ReturnBlance, vmst, vmstDV, lblGlCoa.Text);
                        btnSave.Enabled = false;
                        btnDelete.Enabled = false;
                        lbLId.Text = ID.ToString();
                        ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record is/are save successfully...!!');", true);

                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                    }
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
    const string SalesCode = "1-8020104";
    const string ClosingStock = "1-1030002";

    protected void Delete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            SaleReturn rtn = SaleReturnManager.getBranchShowRetirnItems(lbLId.Text, Session["BranchId"].ToString());
            if (rtn != null)
            {
                rtn.BranchID = Session["BranchId"].ToString();
                DataTable dtOldList = (DataTable)ViewState["OldRtnList"];
                SaleReturnManager.BranchDeleteItemsReturn(rtn, dtOldList);
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record has been delete successfully...!!');", true);
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
        }
    }

    protected void dgPRNMst_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgPRNMst.DataSource = ViewState["mst"];
        dgPRNMst.PageIndex = e.NewPageIndex;
        dgPRNMst.DataBind();
    }
    protected void dgPRNMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[6].Attributes.Add("style", "display:none");
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

    protected void dgPRNMst_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session["SalesReturn"] = dgPRNMst.SelectedRow.Cells[6].Text;
        string strJS = ("<script type='text/javascript'>window.open('BranchSalesReturn.aspx?mno=6.20','_blank');</script>");
        Page.ClientScript.RegisterStartupScript(this.GetType(), "strJSAlert", strJS);
    }

    private void FindReturn()
    {
        txtRemarks.Enabled = txtReturnDate.Enabled = true;
        if (string.IsNullOrEmpty(lbLId.Text))
        {
            lbLId.Text = dgPRNMst.SelectedRow.Cells[6].Text.Trim();
        }
     
        SaleReturn rtn = SaleReturnManager.getBranchShowRetirnItems(lbLId.Text, Session["BranchId"].ToString());
        if (rtn != null)
        {
            txtGoodsReceiveNo.Text = IdManager.GetShowSingleValueString("t2.InvoiceNo",
                "OrderReturn t1 inner join [Order] t2 on t2.ID=t1.InvoiceNo where t1.ID='" + lbLId.Text +
                "' and t1.BranchId='" + Session["BranchId"].ToString() + "' and t2.BranchId='" +
                Session["BranchId"].ToString() + "'");
                
               
            ViewState["PVID"] = "";
            lblGlCoa.Text = "";
            DataTable dt = SaleReturnManager.GetBranchShowSLMasterInfo(txtGoodsReceiveNo.Text, Session["BranchId"].ToString());
            if (dt.Rows.Count > 0)
            {
                txtSupplier.Text = dt.Rows[0]["ContactName"].ToString();
                ViewState["PVID"] = dt.Rows[0]["ID"].ToString();
                lblPVID.Text = dt.Rows[0]["ID"].ToString();
                lblGlCoa.Text = dt.Rows[0]["Gl_CoaCode"].ToString();
                lblSupID.Text = dt.Rows[0]["CustomerID"].ToString();
                txtReturnDate.Text = rtn.ReturnDate;
                txtReturnNO.Text = rtn.Return_No;
                txtRemarks.Text = rtn.Remarks;
                DataTable dtItems = SaleReturnManager.BranchItemsDetails(lbLId.Text, Session["BranchId"].ToString());
                Session["purdtl"] = dtItems;
                ViewState["OldRtnList"] = dtItems;
                dgPODetailsDtl.DataSource = dtItems;
                dgPODetailsDtl.DataBind();
                ShowFooterTotal(dtItems);
                dgPRNMst.Visible = btnNew.Visible = false;
                PVIesms_UP.Visible = true;
                PVIesms_UP.Update();
            }
            ddlPaymentMethord.SelectedValue = rtn.PaymentMethod.Trim();
            txtTotPayment.Text = rtn.Pay_Amount;
            txtDue.Text = (Convert.ToDouble(txtTotal.Text) - Convert.ToDouble(txtTotPayment.Text)).ToString("N2");
            if (ddlPaymentMethord.SelectedValue != "C")
            {
                txtChequeDate.Text = rtn.ChequeDate;
                txtChequeNo.Text = rtn.ChequeNo;
                ddlBank.SelectedValue = rtn.BankName;
                ddlChequeStatus.SelectedValue = rtn.Chk_Status;
            }
        }
    }

    protected void txtQnty_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        DataTable dt = (DataTable)Session["purdtl"];
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            try
            {
                var qtyCk = Convert.ToDecimal(((TextBox)gvr.FindControl("txtQnty")).Text);
            }
            catch
            {

                ((TextBox)gvr.FindControl("txtQnty")).Text = "1";
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                       "alert('This Qty is not valid So 1  Qty is fixed');", true);
            }
            if (string.IsNullOrEmpty(lbLId.Text))
            {
                if (Convert.ToDouble(dr["salqnty"]) < Convert.ToDouble(((TextBox)gvr.FindControl("txtQnty")).Text))
                {
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                        "alert('Return Quantity Upper Then sales Quantity.!!!');", true);
                    ((TextBox)gvr.FindControl("txtQnty")).Text = "0";
                    return;
                }
            }
            else
            {
                if ((Convert.ToDouble(dr["salqnty"]) + Convert.ToDouble(dr["qnty"])) < Convert.ToDouble(((TextBox)gvr.FindControl("txtQnty")).Text))
                {
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                        "alert('Return Quantity Upper Then sales Quantity.!!!');", true);
                    ((TextBox)gvr.FindControl("txtQnty")).Text = "0";
                    return;
                }
            }
            dr["qnty"] = ((TextBox)gvr.FindControl("txtQnty")).Text;
            dr["discountAmt"] = (Convert.ToDecimal(((TextBox)gvr.FindControl("txtdiscountAmtHf")).Text) *
                                Convert.ToDecimal(((TextBox)gvr.FindControl("txtQnty")).Text)).ToString("N0");

            dr["TaxAmount"] = (Convert.ToDecimal(((TextBox)gvr.FindControl("txtVatHf")).Text) *
                                Convert.ToDecimal(((TextBox)gvr.FindControl("txtQnty")).Text)).ToString("N0");
        }
        string found = "";
        foreach (DataRow drd in dt.Rows)
        {
            if (drd["item_code"].ToString() == "" && drd["item_desc"].ToString() == "")
            {
                found = "Y";
            }
        }
        if (found == "")
        {
            DataRow drd = dt.NewRow();
            dt.Rows.Add(drd);
        }
        dgPODetailsDtl.DataSource = dt;
        dgPODetailsDtl.DataBind();
        ShowFooterTotal(dt);
        ((DropDownList)dgPODetailsDtl.Rows[dgPODetailsDtl.Rows.Count - 1].FindControl("DropDownList1")).Focus();
        PVIesms_UP.Update();
    }
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((DropDownList)sender).NamingContainer;
        DataTable dtdtl = (DataTable)Session["purdtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];

        DataTable dt = SaleReturnManager.GetBranchIVItems(((DropDownList)gvr.FindControl("DropDownList1")).SelectedValue, ViewState["PVID"].ToString(), Session["BranchId"].ToString());
        if (dt.Rows.Count > 0)
        {
            DataRow[] rows = dtdtl.Select("ID = " + ((DataRow)dt.Rows[0])["ID"].ToString() + " ");
            // DataRow drr = dtdtl.AsEnumerable().SingleOrDefault(r => r.Field<int?>("ItemsID") ==Convert.ToInt32(((DataRow)dt.Rows[0])["ItemsID"].ToString()));
            if (rows != null)
            {
                if (rows.Length > 0)
                {
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('This items already added...!!!');", true);
                    ((DropDownList)gvr.FindControl("DropDownList1")).SelectedIndex = -1;
                    return;
                }
            }
            dtdtl.Rows.Remove(dr);
            dr = dtdtl.NewRow();
            dr["ID"] = ((DataRow)dt.Rows[0])["ID"].ToString();
            dr["item_desc"] = ((DataRow)dt.Rows[0])["item_desc"].ToString();
            dr["item_code"] = ((DataRow)dt.Rows[0])["item_code"].ToString();
            var a = ((DataRow)dt.Rows[0])["Barcode"].ToString();
            dr["Barcode"] = ((DataRow)dt.Rows[0])["Barcode"].ToString();
            dr["msr_unit_code"] = ((DataRow)dt.Rows[0])["msr_unit_code"].ToString();
            dr["item_rate"] = ((DataRow)dt.Rows[0])["item_rate"].ToString();
            dr["salqnty"] = ((DataRow)dt.Rows[0])["salqnty"].ToString();
            dr["qnty"] = "0";
            dr["PvUnitPrice"] = ((DataRow)dt.Rows[0])["PvUnitPrice"].ToString();
            dr["Type"] = ((DataRow)dt.Rows[0])["Type"].ToString();
            dr["ItemsID"] = ((DataRow)dt.Rows[0])["ItemsID"].ToString();
            // dr["discountAmt"] =Convert.ToDecimal(((DataRow)dt.Rows[0])["discountAmt"]).ToString("N0");
            dr["discountAmt"] = "0";

            dr["discountAmtfix"] = Convert.ToDecimal(((DataRow)dt.Rows[0])["discountAmtfix"]).ToString("N0");

            //dr["TaxAmount"] = Convert.ToDecimal(((DataRow)dt.Rows[0])["TaxAmount"]).ToString("N0");
            dr["TaxAmount"] = "0";
            dr["TaxAmountfix"] = Convert.ToDecimal(((DataRow)dt.Rows[0])["TaxAmountfix"]).ToString("N0");
            dr["FixdiscountAmt"] = ((DataRow)dt.Rows[0])["FixdiscountAmt"].ToString();
            dr["PvUnitPrice"] = ((DataRow)dt.Rows[0])["PvUnitPrice"].ToString();
            dtdtl.Rows.InsertAt(dr, gvr.DataItemIndex);
        }
        dgPODetailsDtl.DataSource = dtdtl;
        dgPODetailsDtl.DataBind();
        ShowFooterTotal(dtdtl);
        ((TextBox)dgPODetailsDtl.Rows[dgPODetailsDtl.Rows.Count - 1].FindControl("txtQnty")).Focus();
        PVIesms_UP.Update();
    }
    //private void ShowFooterTotal(DataTable DT1)
    //{
    //    decimal tot = 0;
    //    decimal totDiscount = 0;
    //    foreach (DataRow dr in DT1.Rows)
    //    {
    //        if (dr["item_desc"].ToString() != "")
    //        {
    //            tot += Convert.ToDecimal(dr["item_rate"].ToString().Replace(",", "")) * Convert.ToDecimal(dr["qnty"]);
    //            if (!string.IsNullOrEmpty(dr["discountAmt"].ToString()))
    //            {
    //                totDiscount += Convert.ToDecimal(dr["discountAmt"].ToString().Replace(",", ""));
    //            }
    //        }
    //    }
    //    txtTotal.Text = (tot - totDiscount).ToString("N2");
    //    if (string.IsNullOrEmpty(txtTotPayment.Text))
    //    {
    //        txtTotPayment.Text = "0";
    //    }

    //    txtDue.Text = (Convert.ToDouble(txtTotal.Text) - Convert.ToDouble(txtTotPayment.Text)).ToString("N2");
    //}
    private void ShowFooterTotal(DataTable DT1)
    {
        decimal tot = 0;
        decimal totDiscount = 0;
        decimal totVat = 0;
        foreach (DataRow dr in DT1.Rows)
        {
            if (dr["item_desc"].ToString() != "")
            {
                tot += Convert.ToDecimal(dr["item_rate"].ToString().Replace(",", "")) * Convert.ToDecimal(dr["qnty"]);
                if (!string.IsNullOrEmpty(dr["discountAmt"].ToString()))
                {
                    totDiscount += Convert.ToDecimal(dr["discountAmt"].ToString().Replace(",", ""));
                    totVat += Convert.ToDecimal(dr["TaxAmount"].ToString().Replace(",", ""));
                }
            }
        }
        txtTotal.Text = ((tot - totDiscount) + totVat).ToString("N2");
        if (string.IsNullOrEmpty(txtTotPayment.Text))
        {
            txtTotPayment.Text = "0";
        }

        txtDue.Text = (Convert.ToDouble(txtTotal.Text) - Convert.ToDouble(txtTotPayment.Text)).ToString("N2");
    }
    protected void Clear_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {

        string filename = txtReturnNO.Text;
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
        Document document = new Document(PageSize.A4, 50f, 50f, 40f, 40f);
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
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.BOLD)));
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
        cell.FixedHeight = 23f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase("Sales/Invoice Return", FontFactory.GetFont(FontFactory.TIMES_BOLD, 11, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;

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

        float[] titW = new float[2] { 80, 60 };
        PdfPTable pdtm = new PdfPTable(titW);
        pdtm.WidthPercentage = 100;

        PdfPTable pdtclient = new PdfPTable(4);
        pdtclient.WidthPercentage = 100;
        cell = new PdfPCell(FormatHeaderPhrase("I.Return No "));
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        pdtclient.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtReturnNO.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        pdtclient.AddCell(cell);
        string Phone = IdManager.GetShowSingleValueString("Mobile", "ID", "Customer", lblSupID.Text);
        cell = new PdfPCell(FormatHeaderPhrase("Customer Name "));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtclient.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtSupplier.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        pdtclient.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Remarks "));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtclient.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtRemarks.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        pdtclient.AddCell(cell);


        PdfPTable pdtpur = new PdfPTable(2);
        pdtpur.WidthPercentage = 100;
        cell = new PdfPCell(FormatHeaderPhrase("Invoice No. "));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtGoodsReceiveNo.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Return Date"));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + DataManager.DateEncode(txtReturnDate.Text).ToString(IdManager.DateFormat())));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        pdtpur.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.Colspan = 2;
        pdtpur.AddCell(cell);

        cell = new PdfPCell(pdtclient);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);

        cell = new PdfPCell(pdtpur);
        cell.BorderWidth = 0f;
        pdtm.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 8f;
        cell.Colspan = 2;
        pdtm.AddCell(cell);

        document.Add(pdtm);

        //document.Add(dtempty);     

        float[] widthdtl = new float[6] { 15, 60, 20, 20, 20, 25 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Particulars"));
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
        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.FixedHeight = 20f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        //DataTable DT1 = SalesManager.GetSalesDetails(lblInvNo.Text);
        int Serial = 1;
        decimal totQty = 0;
        decimal tot = 0;
        DataTable DT1 = SaleReturnManager.ItemsDetails(lbLId.Text);
        foreach (DataRow dr in DT1.Rows)
        {
            if (!string.IsNullOrEmpty(dr["item_desc"].ToString()))
            {
                cell = new PdfPCell(FormatPhrase(Serial.ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                Serial++;

                cell = new PdfPCell(FormatPhrase(dr["des_name"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["BrandName"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["qnty"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["item_rate"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Total"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                tot += Convert.ToDecimal(dr["Total"]);
            }
        }

        cell = new PdfPCell(FormatPhrase("Total"));
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 5;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(tot.ToString("N2")));
        //cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
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
        cell.Colspan = 6;
        pdtdtl.AddCell(cell);


        //PdfPTable dtempty1 = new PdfPTable(1);
        //dtempty1.WidthPercentage = 100;
        cell = new PdfPCell(FormatPhrase("In word: " + DataManager.GetLiteralAmt(tot.ToString()).Replace("  ", " ").Replace("  ", " ")));
        cell.VerticalAlignment = 1;
        cell.HorizontalAlignment = 0;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        cell.Colspan = 6;
        pdtdtl.AddCell(cell);

        //PdfPTable dtempty1 = new PdfPTable(1);
        //dtempty1.WidthPercentage = 100;
        //cell = new PdfPCell(FormatHeaderPhrase("Comments :"));         
        //cell.FixedHeight = 20f;
        //cell.HorizontalAlignment = 0;
        //cell.VerticalAlignment = 1;
        //cell.Border = 0;

        //dtempty1.AddCell(cell);
        //document.Add(dtempty1);
        document.Add(pdtdtl);

        cell = SignatureFormat(document, cell);

        document.Close();
        Response.Flush();
        Response.End();
    }
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
        cell = new PdfPCell(FormatPhrase("Prepared by"));
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
    public DataTable PopulateMeasure()
    {
        dtmsr = ItemManager.GetMeasure();
        DataRow dr = dtmsr.NewRow();
        dtmsr.Rows.InsertAt(dr, 0);
        return dtmsr;
    }
    protected void dgPurDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((DataRowView)e.Row.DataItem)["qnty"].ToString() != "" && ((DataRowView)e.Row.DataItem)["item_rate"].ToString() != "")
                {

                    decimal total = decimal.Parse(((DataRowView)e.Row.DataItem)["item_rate"].ToString().Replace(",", "")) *
                                                  decimal.Parse(((DataRowView)e.Row.DataItem)["qnty"].ToString());
                    var a = Convert.ToDecimal(((DataRowView)e.Row.DataItem)["discountAmt"].ToString()
                             .Replace(",", ""));

                    ((Label)e.Row.FindControl("lblTotal")).Text =
                        ((total - Convert.ToDecimal(((DataRowView)e.Row.DataItem)["discountAmt"].ToString()
                             .Replace(",", "")) + Convert.ToDecimal(((DataRowView)e.Row.DataItem)["TaxAmount"].ToString()
                            .Replace(",", "")))).ToString("N2");

                }
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[3].Attributes.Add("style", "display:none");

                e.Row.Cells[10].Attributes.Add("style", "display:none");
                e.Row.Cells[11].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[3].Attributes.Add("style", "display:none");
                e.Row.Cells[10].Attributes.Add("style", "display:none");
                e.Row.Cells[11].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[3].Attributes.Add("style", "display:none");
                e.Row.Cells[10].Attributes.Add("style", "display:none");
                e.Row.Cells[11].Attributes.Add("style", "display:none");
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

    //protected void dgPurDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    try
    //    {
    //        if (e.Row.RowType == DataControlRowType.DataRow)
    //        {
    //            if (((DataRowView)e.Row.DataItem)["qnty"].ToString() != "" && ((DataRowView)e.Row.DataItem)["item_rate"].ToString() != "")
    //            {
    //                decimal total = decimal.Parse(((DataRowView)e.Row.DataItem)["item_rate"].ToString().Replace(",", "")) *
    //                               decimal.Parse(((DataRowView)e.Row.DataItem)["qnty"].ToString());
    //                var a = Convert.ToDecimal(((DataRowView)e.Row.DataItem)["discountAmt"].ToString()
    //                         .Replace(",", ""));

    //                ((Label)e.Row.FindControl("lblTotal")).Text =
    //                    ((total - Convert.ToDecimal(((DataRowView)e.Row.DataItem)["discountAmt"].ToString()
    //                         .Replace(",", "")) + Convert.ToDecimal(((DataRowView)e.Row.DataItem)["TaxAmount"].ToString()
    //                        .Replace(",", "")))).ToString("N2");

    //            }
    //            e.Row.Cells[1].Attributes.Add("style", "display:none");
    //            e.Row.Cells[3].Attributes.Add("style", "display:none");
    //            e.Row.Cells[10].Attributes.Add("style", "display:none");
    //        }
    //        else if (e.Row.RowType == DataControlRowType.Header)
    //        {
    //            e.Row.Cells[1].Attributes.Add("style", "display:none");
    //            e.Row.Cells[3].Attributes.Add("style", "display:none");
    //            e.Row.Cells[10].Attributes.Add("style", "display:none");
    //        }
    //        else if (e.Row.RowType == DataControlRowType.Footer)
    //        {
    //            e.Row.Cells[1].Attributes.Add("style", "display:none");
    //            e.Row.Cells[3].Attributes.Add("style", "display:none");
    //            e.Row.Cells[10].Attributes.Add("style", "display:none");
    //        }
    //    }
    //    catch (FormatException fex)
    //    {
    //        ExceptionLogging.SendExcepToDB(fex);
    //        ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
    //    }
    //    catch (Exception ex)
    //    {
    //        ExceptionLogging.SendExcepToDB(ex);
    //        if (ex.Message.Contains("Database"))
    //            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex.Message + "');", true);
    //        else
    //            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex.Message + "');", true);

    //    }
    //}
    protected void dgPurDtl_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (Session["purdtl"] != null)
        {
            DataTable dtDtlGrid = (DataTable)Session["purdtl"];
            dtDtlGrid.Rows.RemoveAt(dgPODetailsDtl.Rows[e.RowIndex].DataItemIndex);
            if (dtDtlGrid.Rows.Count == 0)
            {
                DataRow dr = dtDtlGrid.NewRow();
                dr["salqnty"] = "0";
                dr["item_desc"] = "";
                dtDtlGrid.Rows.Add(dr);
            }
            dgPODetailsDtl.DataSource = dtDtlGrid;
            dgPODetailsDtl.DataBind();
            ShowFooterTotal(dtDtlGrid);
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Your session is over. Try it again!!');", true);
        }
    }
    protected void ddlPaymentMethord_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlPaymentMethord.SelectedValue == "C")
        {
            VisiblePayment(false, false, false, false, false, false, false, false);
            // lblAmount.Text = "Cash Amount ";
            ddlChequeStatus.SelectedIndex = -1;
        }
        else if (ddlPaymentMethord.SelectedValue == "Q")
        {
            VisiblePayment(true, true, true, true, true, true, true, true);
            string query2 =
                "select '' [bank_id],'' [bank_name]  union select convert(nvarchar,[ID]) ,BankBranch_AccName AS [bank_name] from [View_Bank_Branch_Info] order by 1";
            util.PopulationDropDownList(ddlBank, "bank_info", query2, "bank_name", "bank_id");
            ddlChequeStatus.SelectedIndex = 1;
        }
        //else if (ddlPaymentMethord.SelectedValue == "CR")
        //{
        //    VisiblePayment(false, false, true, true, true, true, true, true);
        //   // lblAmount.Text = "Card Amount ";
        //}
        //else
        //{
        //    VisiblePayment(false, false, false, false, false, false, false, false);
        //    //lblAmount.Text = "Cash Amount ";
        //    ddlChequeStatus.SelectedIndex = -1;
        //}
    }
    public void VisiblePayment(bool lblBank, bool Bank, bool lblChkNo, bool ChkNo, bool lblChkDate, bool chkdate, bool lblChkStatus, bool chkStatus)
    {
        lblBankName.Visible = lblBank;
        ddlBank.Visible = Bank;
        lblChequeNo.Visible = lblChkNo;
        txtChequeNo.Visible = ChkNo;
        lblChequeDate.Visible = lblChkDate;
        txtChequeDate.Visible = chkdate;
        lblChequeStatus.Visible = lblChkStatus;
        ddlChequeStatus.Visible = chkStatus;
        ddlBank.SelectedIndex = -1;
        txtChequeDate.Text = txtChequeNo.Text = "";
    }
    protected void txtTotPayment_TextChanged(object sender, EventArgs e)
    {
        try
        {
            txtDue.Text = (Convert.ToDouble(txtTotal.Text) - Convert.ToDouble(txtTotPayment.Text)).ToString("N2");
            ddlPaymentMethord.Focus();
            UPPaymentMtd.Update();
        }
        catch
        {
            txtTotPayment.Text = "0";
            txtDue.Text = (Convert.ToDouble(txtTotal.Text) - Convert.ToDouble(txtTotPayment.Text)).ToString("N2");
            ddlPaymentMethord.Focus();
            UPPaymentMtd.Update();
        }
    }
}