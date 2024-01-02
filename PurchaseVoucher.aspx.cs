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
using System.Globalization;
using sales;

public partial class PurchaseVoucher : System.Web.UI.Page
{
    List<VouchDtl> _aVouchDtlList = new List<VouchDtl>();
    private static DataTable dtsup = new DataTable();
    private static DataTable dtmsr = new DataTable();
    public static decimal priceDr = 0;
    private static Permis per;
    DateTime Test;
    private readonly PurchaseVoucherManager _aPurchaseVoucherManager=new PurchaseVoucherManager();
    ItemManager _aItemManager=new ItemManager();
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
                            wnot = dReader["description"].ToString();
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
            try
            {
                Session["Cash_Code"] = "";
                DropDownListValue();
                hfSupplierID.Value = "";
                txtGRNODate.Attributes.Add("onBlur", "formatdate('" + txtGRNODate.ClientID + "')");
                txtPODate.Attributes.Add("onBlur", "formatdate('" + txtPODate.ClientID + "')");
                txtChallanDate.Attributes.Add("onBlur", "formatdate('" + txtChallanDate.ClientID + "')");

                txtGRNODate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtPODate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtChallanDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                PanelHistory.Visible = true;
                tabVch.Visible = false;
                DataTable dt1 = IdManager.GetShowDataTable("SELECT * FROM [FixGlCoaCode]");
                Session["Cash_Code"] = dt1.Rows[0]["CashInHand_BD"].ToString();
                Session["Cash_Name"] = dt1.Rows[0]["CashName_BD"].ToString();

                DataTable dt = PurchaseVoucherManager.GetShowPurchaseMst();
                dgPVMst.DataSource = dt;
                Session["PvMst"] = dt;
                dgPVMst.DataBind();

                btnDelete.Enabled = btnSave.Enabled = btnNew.Visible = true;
                txtGRNO.Enabled =
                    txtChallanNo.Enabled =
                        txtPO.Enabled =
                            txtGRNODate.Enabled =
                                txtChallanDate.Enabled =
                                    txtSupplierSearch.Enabled =
                                        txtRemarks.Enabled =
                                                txtShiftmentNo.Enabled = ddlParty.Enabled = chkAdvance.Enabled = false;
                txtAddTot.Text = "0";
                txtDiscountAmt.Text = "0";
                txtID.Text = "";
                ddlPaymentMethord.SelectedIndex = -1;
                btnNew.Visible = true;
                ViewState["Oldpurdtl"] = null;
                PVI_UP.Update();
                PVIesms_UP.Update();
                UP1.Update();
                UP2.Update();
                txtGRNO.Focus();
                if (Session["PurchaseVoucher"] != null)
                {
                    txtID.Text = Session["PurchaseVoucher"].ToString();
                    btnFind_Click(sender, e);
                    Session["PurchaseVoucher"] = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendExcepToDB(ex);
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex.Message + "');", true);
            }
        }
        else
        {
            ShowFooterTotal();
        }
    }

    private void DropDownListValue()
    {
        //string queryLoc = "select '' ID,'' ContactName  union select ID ,ContactName from Supplier where Country='1' order by 1";
        //util.PopulationDropDownList(ddlSupplier, "CostType", queryLoc, "ContactName", "ID");

        string queryCon = @"SELECT [COUNTRY_CODE],[COUNTRY_DESC] FROM [COUNTRY_INFO]";
        util.PopulationDropDownList(ddlPopSupplier, "CostType", queryCon, "COUNTRY_DESC", "COUNTRY_CODE");

        string query = "select '' ID,'' ContactName  union select ID ,ContactName from Labure where SupplierGroupID='CP' order by 1";
        util.PopulationDropDownList(ddlCarriagePerson, "CostType", query, "ContactName", "ID");

        string query1 = "select '' ID,'' ContactName  union select ID ,ContactName from Labure where SupplierGroupID='LP' order by 1";
        util.PopulationDropDownList(ddlLaburePerson, "CostType", query1, "ContactName", "ID");

        string query2 = "select '' [bank_id],'' [bank_name]  union select convert(nvarchar,[ID]) ,BankBranch_AccName AS [bank_name] from [View_Bank_Branch_Info] order by 1";
        util.PopulationDropDownList(ddlBank, "bank_info", query2, "bank_name", "bank_id");

        string query3 = "select '' ID,'' PartyName  union select  ID,PartyName from  PartyInfo order by 1";
        util.PopulationDropDownList(ddlParty, "PartyInfo", query3, "PartyName", "ID");



    }
    protected void btnNew_Click(object sender, EventArgs e)
    {
        ClearFields();
        PanelHistory.Visible = btnNew.Visible = btnNew.Visible = false;       
        getEmptyDtl();      
       // ddlSupplier.SelectedIndex = -1;
        ddlPaymentMethord.SelectedIndex = -1;
        tabVch.Visible =  chkAdvance.Enabled =txtPODate.Enabled= true;
        txtChallanNo.Enabled = txtPO.Enabled = txtGRNODate.Enabled = txtChallanDate.Enabled =
            txtSupplierSearch.Enabled = txtRemarks.Enabled =
                dgPVDetailsDtl.Enabled = txtShiftmentNo.Enabled = ddlParty.Enabled = true;
        txtChallanNo.Focus();
    }
    private void ClearFields()
    {
        ViewState["Supplier_COA"] = null;
        ViewState["purdtl"] = null;
        txtGRNO.Text = "";
        txtPO.Text = "";
        txtChallanNo.Text = "";
        txtRemarks.Text = "";
       // txtSiftment.Text = "";
        txtGRNODate.Text = System.DateTime.Now.Date.ToString("dd/MM/yyyy");
        txtPODate.Text = System.DateTime.Now.Date.ToString("dd/MM/yyyy");
        txtChallanDate.Text = System.DateTime.Now.Date.ToString("dd/MM/yyyy");
        txtTotalAmount.Text = "0";
        txtOtherCharge.Text = "0";
        txtCarriageCharge.Text = "0";
        txtLabureCharge.Text = "0";
        txtTotPayment.Text = "0";
        txtDue.Text = "0";
        txtDiscountAmt.Text = "0";
        //ddlPaymentMethord.SelectedIndex = 1;
        ddlBank.SelectedIndex = -1;
        txtChequeDate.Text = "";
        txtChequeNo.Text = "";
        txtChequeAmount.Text = "0";
        txtID.Text =hfSupplierID.Value=txtSupplierSearch.Text= "";
        txtTotItems.Text = txtAddTot.Text = "0";
        VisiblePayment(false, false, false, false, false, false, false, false);
    }
    private void getEmptyDtl()
    {      
        dgPVDetailsDtl.Visible = true;
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("ID", typeof(string));
        dtDtlGrid.Columns.Add("item_code", typeof(string));
        dtDtlGrid.Columns.Add("item_desc", typeof(string));
        dtDtlGrid.Columns.Add("Barcode", typeof(string));
        dtDtlGrid.Columns.Add("msr_unit_code", typeof(string));
        dtDtlGrid.Columns.Add("item_rate", typeof(string));
        dtDtlGrid.Columns.Add("qnty", typeof(string));
        dtDtlGrid.Columns.Add("Expdate", typeof(string));
        dtDtlGrid.Columns.Add("Additional", typeof(string));
        dtDtlGrid.Columns.Add("UMO", typeof(string));
        dtDtlGrid.Columns.Add("BrandName", typeof(string));
        dtDtlGrid.Columns.Add("item_sales_rate", typeof(string));
        DataRow dr = dtDtlGrid.NewRow();
        dr["Additional"] = "0";
        dtDtlGrid.Rows.Add(dr);
        dgPVDetailsDtl.DataSource = dtDtlGrid;
        ViewState["purdtl"] = dtDtlGrid;
        dgPVDetailsDtl.DataBind();
        ShowFooterTotal();
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            
            if (string.IsNullOrEmpty(txtSupplierSearch.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Enter Supplier..!!');", true);
                return;
            }
            else if (ddlPaymentMethord.SelectedValue == "Q" && ddlChequeStatus.SelectedItem.Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Select Cheque Status..!!');",
                    true);
                return;
            }
            else if (ddlPaymentMethord.SelectedValue == "Q" && Convert.ToDouble(txtTotPayment.Text) > 0 &&
                     ddlChequeStatus.SelectedValue == "P")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Incorrect Check Status.!!');",
                    true);
                return;
            }
            else if (string.IsNullOrEmpty(txtRemarks.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Input Remarks/Particulars.!!');",
                    true);
                return;
            }
            else if (string.IsNullOrEmpty(txtChallanNo.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Input Challan No.!!');", true);
                return;
            }
           

                PurchaseVoucherInfo purmst = PurchaseVoucherManager.GetPurchaseMst(txtID.Text.Trim());
                if (purmst != null)
                {
                    if (per.AllowEdit == "Y")
                    {
                        DataTable dt = (DataTable)ViewState["purdtl"];
                        DataTable dtOldVcDtl = (DataTable)ViewState["Oldpurdtl"];

                        int CountGRN = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER(GRN)", "ItemPurchaseMst",
                            txtGRNO.Text.ToUpper(), Convert.ToInt32(txtID.Text));
                        if (CountGRN > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                                "alert('Warning :\\n This GRN already exist.!!');", true);
                            return;
                        }

                        int IsCheckChalan = IdManager.GetShowSingleValueInt("count(ChallanNo)",
                            "[ItemPurchaseMst] where ID!='" + txtID.Text + "' and ChallanNo='"+txtChallanNo.Text+"' ");

                        if (IsCheckChalan > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                                "alert('Warning :\\n This Chalan  already exist.!!');", true);
                            return;
                        }
                        bool isCheckQty = false;
                        string Message = "";
                        foreach (DataRow drold in dtOldVcDtl.Rows)
                        {
                            if (drold["item_code"].ToString() != "")
                            {
                                double CheckSalesAndPurQty = _aPurchaseVoucherManager.GetShowCheckQty(
                                    drold["ID"].ToString(), drold["item_sales_rate"].ToString(),
                                    drold["item_rate"].ToString(), drold["Expdate"].ToString());
                                double CheckSales = _aPurchaseVoucherManager.GetShowCheckSalesQty(
                                    drold["ID"].ToString(), drold["item_sales_rate"].ToString(),
                                    drold["item_rate"].ToString(), drold["Expdate"].ToString());

                                //int Items_Dtl_ID = 0;
                                ////DataRow drCheckDtonQty = null;
                                //try
                                //{
                                //    Items_Dtl_ID = Convert.ToInt32(drold["Items_Dtl_ID"]);
                                //}
                                //catch
                                //{
                                //    Items_Dtl_ID = 0;
                                //}

                                //DataRow drCheckDtonQty = dt.AsEnumerable()
                                //    .SingleOrDefault(r => r.Field<string>("Items_Dtl_ID") == Items_Dtl_ID.ToString());
                                double NewQty = Convert.ToDouble(drold["qnty"].ToString());
                                if (NewQty>CheckSalesAndPurQty)
                                {
                                    isCheckQty = true;
                                    Message = "this items : " + drold["item_desc"].ToString() +
                                              " purchase quantity upper then stock quantity.you can not update this voucher.contract your administrator..!!";
                                    break;
                                }
                            }
                        }

                        if (isCheckQty.Equals(true))
                        {
                           
                            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                                "alert('" + Message + "');", true);
                            return;
                        }
                        bool isCheck = false;
                        string message = "";
                        foreach (DataRow drItems in dt.Rows)
                        {
                            if (!string.IsNullOrEmpty(drItems["Expdate"].ToString()))
                            {
                                if (
                                    DateTime.TryParseExact(drItems["Expdate"].ToString(), "dd/MM/yyyy", null,
                                        DateTimeStyles.None,
                                        out Test) == false)
                                {
                                    isCheck = true;
                                    message = "this item : " + drItems["item_desc"].ToString() +
                                              " expiration date are wrong.please correct expiration date and saved again..!!";
                                    break;
                                }
                            }
                        }

                        if (isCheck == true)
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                                "alert('" + message + "');", true);
                            return;
                        }
                        purmst.ID = txtID.Text;
                        purmst.GoodsReceiveNo = txtGRNO.Text.Trim();
                        purmst.GoodsReceiveDate = txtGRNODate.Text;
                        purmst.PurchaseOrderNo = lblOrNo.Text;
                        purmst.PurchaseOrderDate = txtPODate.Text;
                        purmst.ChallanNo = txtChallanNo.Text;
                        purmst.ChallanDate = txtChallanDate.Text;
                        purmst.Supplier = hfSupplierID.Value;
                        purmst.SupplierName = txtSupplierSearch.Text;
                        purmst.Remarks = txtRemarks.Text;
                        purmst.TotalAmount = txtTotalAmount.Text.Replace(",", "");
                        purmst.TotalPayment = txtTotPayment.Text.Replace(",", "");
                        purmst.DiscountAmt = txtDiscountAmt.Text.Replace(",", "");
                        purmst.CarriagePerson = ddlCarriagePerson.SelectedValue;
                        purmst.CarriageCharge = txtCarriageCharge.Text.Replace(",", "");
                        purmst.LaburePerson = ddlLaburePerson.SelectedValue;
                        purmst.LabureCharge = txtLabureCharge.Text.Replace(",", "");
                        purmst.OtherCharge = txtOtherCharge.Text.Replace(",", "");
                        purmst.PaymentMethord = ddlPaymentMethord.SelectedValue;
                        if (!ddlPaymentMethord.SelectedValue.Equals("C"))
                        {
                            purmst.BankId = ddlBank.SelectedValue;
                            purmst.BankCoaCode = IdManager.GetShowSingleValuestring("Gl_Code",
                                "View_Bank_Branch_Info where ID='" + ddlBank.SelectedValue + "' ");
                            purmst.ChequeNo = txtChequeNo.Text;
                            purmst.ChequeDate = txtChequeDate.Text;
                            purmst.ChequeAmount = txtChequeAmount.Text.Replace(",", "");
                            purmst.BankName = ddlBank.SelectedItem.Text;
                            purmst.ChkStatus = ddlChequeStatus.SelectedValue;
                        }
                        else
                        {
                            purmst.BankId = ""; purmst.BankCoaCode = "";
                            purmst.ChequeNo = "";
                            purmst.ChequeDate = DateTime.Now.ToString("dd/MM/yyyy");
                            purmst.ChequeAmount = "0";
                            purmst.BankName = "";
                            purmst.ChkStatus = "";
                        }
                       // purmst.PvType = rbType.SelectedValue;
                        purmst.PvOrder = txtPO.Text;
                        //if (ddlParty.SelectedItem.Text == "") { purmst.PartyID = "1"; }
                        //else { purmst.PartyID = ddlParty.SelectedValue; }
                        purmst.ShiftmentID = "";
                        purmst.LoginBy = Session["user"].ToString();
                        
                        
                       

                        //********************** Journal Voucher Update *************//

                        string VCH_SYS_NO = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                            "t1.PAYEE='PV' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                            txtGRNO.Text);
                        VouchMst vmst = VouchManager.GetVouchMst(VCH_SYS_NO.Trim());
                        if (vmst != null)
                        {
                            vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
                            vmst.ValueDate = txtGRNODate.Text;
                            vmst.VchCode = "03";
                            //vmst.SerialNo = txtGRNO.Text.Trim();
                            vmst.Particulars = txtRemarks.Text;
                            vmst.ControlAmt = txtTotalAmount.Text.Replace(",", "");
                            //vmst.Payee = "PV";
                            vmst.CheqAmnt = "0";
                            vmst.UpdateUser = Session["user"].ToString().ToUpper();
                            vmst.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                            vmst.AuthoUserType = Session["userlevel"].ToString();
                        }

                        string VCH_SYS_NO_PVCV = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                            "t1.PAYEE='PVDV' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                            txtGRNO.Text);
                        VouchMst vmstDV = VouchManager.GetVouchMst(VCH_SYS_NO_PVCV.Trim());
                        if (vmstDV != null)
                        {
                            vmstDV.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
                            vmstDV.ValueDate = txtGRNODate.Text;
                            vmstDV.RefFileNo = "";
                            vmstDV.VchCode = "01";
                            //vmst.SerialNo = txtGRNO.Text.Trim();
                            vmstDV.Particulars = txtRemarks.Text;
                            vmstDV.ControlAmt = txtTotPayment.Text.Replace(",", "");
                            //vmst.Payee = "PV";
                            vmstDV.CheqAmnt = "0";
                            vmstDV.UpdateUser = Session["user"].ToString().ToUpper();
                            vmstDV.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                            vmstDV.AuthoUserType = Session["userlevel"].ToString();
                        }
                        else
                        {
                            if (Convert.ToDecimal(txtTotPayment.Text) > 0)
                            {
                                vmstDV = new VouchMst();
                                vmstDV.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
                                vmstDV.ValueDate = txtGRNODate.Text;
                                vmstDV.VchCode = "01";
                                vmstDV.RefFileNo = "New";
                                vmstDV.VolumeNo = "";
                                vmstDV.SerialNo = txtGRNO.Text.Trim();
                                vmstDV.Particulars = txtRemarks.Text;
                                vmstDV.ControlAmt = txtTotPayment.Text.Replace(",", "");
                                vmstDV.Payee = "PVDV";
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

                        PurchaseVoucherManager.UpdatePurchaseVoucher(purmst, dt, dtOldVcDtl, vmst,
                            ViewState["Supplier_COA"].ToString(), vmstDV);
                        ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                            "alert('Record are update successfully..!!');", true);
                        btnSave.Enabled = false;
                        Response.Redirect("PurchaseVoucher.aspx");
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('You are not Permitted this Step...!!');", true);
                    }
                }
                else
                {
                    if (per.AllowAdd == "Y")
                    {

                        int IsCheckChalan = IdManager.GetShowSingleValueInt("count(ChallanNo)",
                            "[ItemPurchaseMst] where  ChallanNo='" + txtChallanNo.Text + "' ");

                        if (IsCheckChalan > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                                "alert('Warning :\\n This ChallanNo  already exist.!!');", true);
                            return;
                        }


                        int CountGRN = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER(GRN)", "ItemPurchaseMst",
                            txtGRNO.Text.ToUpper());
                        if (CountGRN > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                                "alert('Warning :\\n This GRN already exist.!!');", true);
                            return;
                        }
                        DataTable dt = (DataTable)ViewState["purdtl"];
                        bool isCheck = false;
                        string message = "";
                        foreach (DataRow drItems in dt.Rows)
                        {
                            if (!string.IsNullOrEmpty(drItems["Expdate"].ToString()))
                            {
                                if (
                                    DateTime.TryParseExact(drItems["Expdate"].ToString(), "dd/MM/yyyy", null,
                                        DateTimeStyles.None,
                                        out Test) == false)
                                {
                                    isCheck = true;
                                    message = "this item : " + drItems["item_desc"].ToString() +
                                              " expiration date are wrong.please correct expiration date and saved again..!!";
                                    break;
                                }
                            }
                        }

                        if (isCheck == true)
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                                "alert('" + message + "');", true);
                            return;
                        }
                        purmst = new PurchaseVoucherInfo();
                        purmst.GoodsReceiveDate = txtGRNODate.Text;
                        purmst.PurchaseOrderNo = lblOrNo.Text;
                        purmst.PurchaseOrderDate = txtPODate.Text;
                        purmst.ChallanNo = txtChallanNo.Text;
                        purmst.ChallanDate = txtChallanDate.Text;


                        purmst.Supplier = hfSupplierID.Value;
                        purmst.SupplierName = txtSupplierSearch.Text;
                        purmst.Remarks = txtRemarks.Text;
                        purmst.TotalAmount = txtTotalAmount.Text.Replace(",", "");
                        if (string.IsNullOrEmpty(txtTotPayment.Text))
                        {
                            txtTotPayment.Text = "0";
                        }
                        purmst.TotalPayment = txtTotPayment.Text.Replace(",", "");
                        purmst.DiscountAmt = txtDiscountAmt.Text.Replace(",", "");
                        purmst.CarriagePerson = ddlCarriagePerson.SelectedValue;
                        purmst.CarriageCharge = txtCarriageCharge.Text.Replace(",", "");
                        purmst.LaburePerson = ddlLaburePerson.SelectedValue;
                        purmst.LabureCharge = txtLabureCharge.Text.Replace(",", "");
                        purmst.OtherCharge = txtOtherCharge.Text.Replace(",", "");
                        purmst.PaymentMethord = ddlPaymentMethord.SelectedValue;
                        if (!ddlPaymentMethord.SelectedValue.Equals("C"))
                        {
                            purmst.BankId = ddlBank.SelectedValue;
                            purmst.BankCoaCode = IdManager.GetShowSingleValuestring("Gl_Code",
                                " View_Bank_Branch_Info where ID='" + ddlBank.SelectedValue + "' ");
                            purmst.ChequeNo = txtChequeNo.Text;
                            purmst.ChequeDate = txtChequeDate.Text;
                            purmst.ChequeAmount = txtChequeAmount.Text.Replace(",", "");
                            purmst.BankName = ddlBank.SelectedItem.Text;
                            purmst.ChkStatus = ddlChequeStatus.SelectedValue;
                        }
                        else
                        {
                            purmst.BankId = "";purmst.BankCoaCode="";
                            purmst.ChequeNo = "";
                            purmst.ChequeDate = DateTime.Now.ToString("dd/MM/yyyy");
                            purmst.ChequeAmount = "0";
                            purmst.BankName = "";
                            purmst.ChkStatus = "";
                        }
                        purmst.LoginBy = Session["user"].ToString();
                        //purmst.PvType = rbType.SelectedValue;
                        purmst.PvOrder = txtPO.Text;
                        if (chkAdvance.Checked == true)
                        {
                            purmst.AdvancePayFlag = "1";
                        }
                        purmst.ShiftmentID ="";
                        purmst.CashCode = Session["Cash_Code"].ToString();
                       

                        txtGRNO.Text = IdManager.GetDateTimeWiseSerial("GRN", "GRN", "[ItemPurchaseMst]");
                        purmst.GoodsReceiveNo = txtGRNO.Text.Trim();

                        //*************************** Account Entry ******************//
                        //********* Jurnal Voucher *********//
                        VouchMst vmst = new VouchMst();
                        vmst.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
                        vmst.ValueDate = txtGRNODate.Text;
                        vmst.VchCode = "03";
                        vmst.RefFileNo = "";
                        vmst.VolumeNo = "";
                        vmst.SerialNo = txtGRNO.Text.Trim();
                        vmst.Particulars = txtRemarks.Text;
                        vmst.ControlAmt = txtTotalAmount.Text.Replace(",", "");
                        vmst.Payee = "PV";
                        vmst.CheckNo = txtChequeNo.Text;
                        vmst.CheqDate = txtChequeDate.Text;
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
                        //***************** Debid Voucher **************//

                        //********* Debit Voucher *********//
                        VouchMst vmstDV = new VouchMst();
                        vmstDV.FinMon = FinYearManager.getFinMonthByDate(txtGRNODate.Text);
                        vmstDV.ValueDate = txtGRNODate.Text;
                        vmstDV.VchCode = "01";
                        vmstDV.RefFileNo = "";
                        vmstDV.VolumeNo = "";
                        vmstDV.SerialNo = txtGRNO.Text.Trim();
                        vmstDV.Particulars = txtRemarks.Text;
                        vmstDV.ControlAmt = txtTotPayment.Text.Replace(",", "");
                        vmstDV.Payee = "PVDV";
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

                        ViewState["CurrencyRate"] = "0";
                        int ID = PurchaseVoucherManager.SavePurchaseVoucher(purmst, dt, lblOrNo.Text, vmst,
                            ViewState["Supplier_COA"].ToString(), vmstDV);
                        ViewState["purdtl"] = PurchaseVoucherManager.GetPurchaseItemsDetails(ID.ToString());
                        txtID.Text = ID.ToString();
                        /*************** auto voucher generate off korar jonno *************/
                       
                        ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                            "alert('Record is/are saved successfully..!!');", true);
                        btnSave.Enabled = false;
                        Response.Redirect("PurchaseVoucher.aspx");
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('You are not Permitted this Step...!!');", true);
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
          
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                    "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                    "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }

    //const string BDItemPurchas = "1-7010101";
    //const string BDLocalPurchase = "1-7010102";
    //const string ClosingStock = "1-1030002";
   
    protected void Delete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            PurchaseVoucherInfo purmst = PurchaseVoucherManager.GetPurchaseMst(txtID.Text.Trim());
            if (purmst != null)
            {
                purmst.ID = txtID.Text;
                purmst.GoodsReceiveNo = txtGRNO.Text.Trim();
                DataTable dtOldVcDtl = (DataTable)ViewState["Oldpurdtl"];
                PurchaseVoucherManager.DeletePurchaseVoucher(purmst, dtOldVcDtl);
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record are delete successfully..!!');", true);
                btnDelete.Enabled = false;
                btnSave.Enabled = false;
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
        }
    }
  
    protected void Clear_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
        //Response.Redirect("PurchaseVoucher.aspx?mno=5.18");
        //RefreshAll();
    }

    private void RefreshAll()
    {
        ClearFields();
        Session["Cash_Code"] = "";
        DropDownListValue();
        dgPVDetailsDtl.DataSource = null;
        dgPVDetailsDtl.DataBind();
        PanelHistory.Visible = btnNew.Visible = true;
        tabVch.Visible = false;
        DataTable dt1 = IdManager.GetShowDataTable("SELECT * FROM [FixGlCoaCode]");
        Session["Cash_Code"] = dt1.Rows[0]["CashInHand_BD"].ToString();
        Session["Cash_Name"] = dt1.Rows[0]["CashName_BD"].ToString();
        DataTable dt = PurchaseVoucherManager.GetShowPurchaseMst();
        dgPVMst.DataSource = dt;
        Session["PvMst"] = dt;
        lblOrNo.Text = ""; txtID.Text = "";
        dgPVMst.DataBind();      
        btnDelete.Enabled =btnSave.Enabled = true;
        txtGRNO.Enabled =
            txtChallanNo.Enabled =
                txtPO.Enabled =
                    txtGRNODate.Enabled =
                        txtChallanDate.Enabled = txtSupplierSearch.Enabled = txtRemarks.Enabled = false;
        txtGRNO.Focus();
    }
    //************* Pv Items Details ******//
    protected void dgPurDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TextBox txtExpDate = (TextBox)e.Row.FindControl("txtExpireDate");
                txtExpDate.Attributes.Add("onBlur", "formatdate('" + txtExpDate.ClientID + "')");

                if (((DataRowView)e.Row.DataItem)["qnty"].ToString() != "" && ((DataRowView)e.Row.DataItem)["item_rate"].ToString() != "")
                {
                    decimal total = decimal.Parse(((DataRowView)e.Row.DataItem)["item_rate"].ToString()) *
                                   decimal.Parse(((DataRowView)e.Row.DataItem)["qnty"].ToString());
                    ((Label)e.Row.FindControl("lblTotal")).Text = total.ToString("N3");

                    decimal totAdd = decimal.Parse(((Label)e.Row.FindControl("lblTotal")).Text)+((decimal.Parse(((Label)e.Row.FindControl("lblTotal")).Text) * decimal.Parse(((DataRowView)e.Row.DataItem)["Additional"].ToString())) / 100);
                    ((Label)e.Row.FindControl("lblAddTotal")).Text = totAdd.ToString("N3");
                   
                }
                e.Row.Cells[9].Attributes.Add("style", "display:none");
                e.Row.Cells[10].Attributes.Add("style", "display:none");
                e.Row.Cells[12].Attributes.Add("style", "display:none");
            }       
            else if (e.Row.RowType == DataControlRowType.Header)
            {
               
                e.Row.Cells[9].Attributes.Add("style", "display:none");
                e.Row.Cells[10].Attributes.Add("style", "display:none");
                e.Row.Cells[12].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[9].Attributes.Add("style", "display:none");
                e.Row.Cells[10].Attributes.Add("style", "display:none");
                e.Row.Cells[12].Attributes.Add("style", "display:none");
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
    protected void dgPurDtl_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["purdtl"] != null)
        {
            DataTable dtDtlGrid = (DataTable)ViewState["purdtl"];
            dtDtlGrid.Rows.RemoveAt(dgPVDetailsDtl.Rows[e.RowIndex].DataItemIndex);
            if (dtDtlGrid.Rows.Count > 0)
            {
                string found = "";
                foreach (DataRow drf in dtDtlGrid.Rows)
                {
                    if (drf["item_code"].ToString() == "" && drf["item_desc"].ToString() == "")
                    {
                        found = "Y";
                    }
                }
                if (found == "")
                {
                    DataRow dr = dtDtlGrid.NewRow();
                    dtDtlGrid.Rows.Add(dr);
                }
                dgPVDetailsDtl.DataSource = dtDtlGrid;
                dgPVDetailsDtl.DataBind();
            }
            else
            {
                getEmptyDtl();
            }
            ShowFooterTotal();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Your session is over. Try it again!!');", true);
        }
    }

    // *************** PV History **************//

    protected void dgPurMst_PageIndexChanging(object sender, GridViewPageEventArgs e)    
    {        
        dgPVMst.PageIndex = e.NewPageIndex;
        dgPVMst.DataSource = Session["PvMst"];
        dgPVMst.DataBind();
    }
    protected void dgPurMst_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session["PurchaseVoucher"] = dgPVMst.SelectedRow.Cells[8].Text;

        string strJS = ("<script type='text/javascript'>window.open('PurchaseVoucher.aspx?mno=5.15','_blank');</script>");
        Page.ClientScript.RegisterStartupScript(this.GetType(), "strJSAlert", strJS);
        
    }
    protected void dgPVMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[8].Attributes.Add("style", "display:none");
                e.Row.Cells[5].Attributes.Add("style", "display:none");
                e.Row.Cells[3].Attributes.Add("style", "display:none");
            }
        }
        catch (FormatException fex)
        {
            ExceptionLogging.SendExcepToDB(fex);
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
        }
        catch (Exception ex)
        {
            
            if (ex.Message.Contains("Database"))
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
        }
    }
    //*********************** PV Details ********************************//
    public DataTable PopulateMeasure()
    {
        dtmsr = ItemManager.GetMeasure();
        DataRow dr = dtmsr.NewRow();
        dtmsr.Rows.InsertAt(dr, 0);
        return dtmsr;
    }
    protected void txtItemCode_TextChanged(object sender, EventArgs e)
    {

    }

    protected void txtItemDesc_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow) ((TextBox) sender).NamingContainer;
            DataTable dtdtl = (DataTable) ViewState["purdtl"];
            DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
            DataTable dtFindItems = _aItemManager.GetSearchItemsWithSize(((TextBox)gvr.FindControl("txtItemDesc")).Text);
            string Parameter = " where t1.[ID]='" + dtFindItems.Rows[0]["ID"].ToString() + "' and  t1.[Active]=1 ";
            DataTable dt = ItemManager.GetItems(Parameter);
            if (dt.Rows.Count > 0)
            {
                bool IsCheck = false;
                foreach (DataRow ddr in dtdtl.Rows)
                {
                    if (string.IsNullOrEmpty(dr["item_desc"].ToString()))
                    {
                        if (ddr["ID"].ToString().Equals(((DataRow)dt.Rows[0])["ID"].ToString()))
                        {
                            IsCheck = true;
                            break;
                        }
                    }
                }

                if (IsCheck == true)
                {
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                        "alert('This items already added ..!!');", true);
                    ((TextBox)gvr.FindControl("txtItemDesc")).Text = "";
                    ((TextBox)gvr.FindControl("txtItemDesc")).Focus();
                    return;
                }
                dtdtl.Rows.Remove(dr);
                dr = dtdtl.NewRow();
                dr["ID"] = ((DataRow) dt.Rows[0])["ID"].ToString();
               
                //dr["item_desc"] = ((DataRow) dt.Rows[0])["item_desc"].ToString();
                dr["item_desc"] = ((DataRow)dt.Rows[0])["ItemSearchDesc"].ToString();

                dr["item_code"] = ((DataRow) dt.Rows[0])["item_code"].ToString();
                dr["Barcode"] = "";
               
                dr["msr_unit_code"] = ((DataRow) dt.Rows[0])["msr_unit_code"].ToString();
                dr["Expdate"] = "";
                dr["item_rate"] = "0";
                dr["qnty"] = "0";
               
                dr["Additional"] = "0";
                dr["item_sales_rate"] = ((DataRow)dt.Rows[0])["UnitPrice"].ToString();
                dr["UMO"] = ((DataRow) dt.Rows[0])["UMO"].ToString();
                dr["BrandName"] = ((DataRow) dt.Rows[0])["BrandName"].ToString();
                dtdtl.Rows.InsertAt(dr, gvr.DataItemIndex);
            }

            dgPVDetailsDtl.DataSource = dtdtl;
            dgPVDetailsDtl.DataBind();
            ShowFooterTotal();
            ((TextBox) dgPVDetailsDtl.Rows[dgPVDetailsDtl.Rows.Count - 1].FindControl("txtBarcode")).Focus();
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

    private void ShowFooterTotal()
    {
        decimal ctot = decimal.Zero;
        decimal totAddi = 0;
        decimal totRat = 0;
        decimal totQty = 0;
        decimal totItemsP = 0;
        decimal totA = 0;
        decimal Total = 0;
        
        if (ViewState["purdtl"] != null)
        {
            DataTable dt = (DataTable)ViewState["purdtl"];
            foreach (DataRow drp in dt.Rows)
            {
                if (drp["item_code"].ToString() != "" && drp["item_rate"].ToString() != "" && drp["qnty"].ToString() != "")
                {
                    totRat += decimal.Parse(drp["item_rate"].ToString());
                    totQty += decimal.Parse(drp["qnty"].ToString());
                    totItemsP += Convert.ToDecimal(drp["item_rate"].ToString()) * Convert.ToDecimal(drp["qnty"].ToString());
                    // totA += decimal.Parse(drp["Additional"].ToString());
                    //totAddi += (totItemsP * decimal.Parse(drp["Additional"].ToString())) / 100;
                    //Total += totItemsP;
                }
            }
            txtAddTot.Text = totAddi.ToString("N3");
            txtTotItems.Text = totItemsP.ToString("N2");
            
        }

        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
        TableCell cell;         
        cell = new TableCell();
        cell.Text = "Total";
        cell.ColumnSpan = 7;
        cell.HorizontalAlign = HorizontalAlign.Right;  
        row.Cells.Add(cell);
        cell = new TableCell();
        //cell.Text = totRat.ToString("N3");
        cell.Text = "";
        cell.HorizontalAlign = HorizontalAlign.Right;        
        row.Cells.Add(cell);
        cell = new TableCell();
        cell.Text = totQty.ToString("N0");
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);

        //cell = new TableCell();
        //cell.Text = "";
        //cell.HorizontalAlign = HorizontalAlign.Right;
        //row.Cells.Add(cell);

        cell = new TableCell();
        priceDr = Total;
        cell.Text = totItemsP.ToString("N2");
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        row.Font.Bold = true;
        row.BackColor = System.Drawing.Color.LightGray;
        if (dgPVDetailsDtl.Rows.Count > 0)
        {
            dgPVDetailsDtl.Controls[0].Controls.Add(row);
        }
        txtTotalAmount.Text = totItemsP.ToString("N2");
       // txtChequeAmount.Text = totItemsP.ToString("N2");
       txtDue.Text = totItemsP.ToString("N2");
        //
    }



    //*************************  txtItemsRate_TextChanged *******************//

    protected void txtItemsRate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            DataTable dt = (DataTable)ViewState["purdtl"];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];
                dr["ID"] = dr["ID"].ToString();
                dr["item_desc"] = dr["item_desc"].ToString();
                dr["item_code"] = dr["item_code"].ToString();
                dr["msr_unit_code"] = dr["msr_unit_code"].ToString();
                dr["item_rate"] = ((TextBox)gvr.FindControl("txtItemRate")).Text;
                if (((TextBox)gvr.FindControl("txtQnty")).Text == "") { dr["qnty"] = "0"; }
                dr["qnty"] = ((TextBox)gvr.FindControl("txtQnty")).Text;

            }
            dgPVDetailsDtl.DataSource = dt;
            dgPVDetailsDtl.DataBind();
            ShowFooterTotal();
            ((TextBox)dgPVDetailsDtl.Rows[dgPVDetailsDtl.Rows.Count - 1].FindControl("txtSalesPrice")).Focus();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }


    //*************************  txtSalesRate_TextChanged *******************//

    protected void txtSalesRate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            DataTable dt = (DataTable)ViewState["purdtl"];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];
                dr["ID"] = dr["ID"].ToString();
                dr["item_desc"] = dr["item_desc"].ToString();
                dr["item_code"] = dr["item_code"].ToString();
                dr["msr_unit_code"] = dr["msr_unit_code"].ToString();
                dr["item_rate"] = ((TextBox)gvr.FindControl("txtItemRate")).Text;
                dr["item_sales_rate"] = ((TextBox)gvr.FindControl("txtSalesPrice")).Text;
                if (((TextBox)gvr.FindControl("txtQnty")).Text == "") { dr["qnty"] = "0"; }
                dr["qnty"] = ((TextBox)gvr.FindControl("txtQnty")).Text;

            }
            //string found = "";
            //foreach (DataRow drd in dt.Rows)
            //{
            //    if (drd["item_code"].ToString() == "" && drd["item_desc"].ToString() == "")
            //    {
            //        found = "Y";
            //    }
            //}
            //if (found == "")
            //{
            //    DataRow drd = dt.NewRow();
            //    dt.Rows.Add(drd);
            //}
            dgPVDetailsDtl.DataSource = dt;
            dgPVDetailsDtl.DataBind();
            ShowFooterTotal();
            ((TextBox)dgPVDetailsDtl.Rows[dgPVDetailsDtl.Rows.Count - 1].FindControl("txtExpireDate")).Focus();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }
    protected void txtExpireDate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            DataTable dt = (DataTable)ViewState["purdtl"];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];
                dr["Expdate"] = ((TextBox)gvr.FindControl("txtExpireDate")).Text;
            }
            //string found = "";
            //foreach (DataRow drd in dt.Rows)
            //{
            //    if (drd["item_code"].ToString() == "" && drd["item_desc"].ToString() == "")
            //    {
            //        found = "Y";
            //    }
            //}
            //if (found == "")
            //{
            //    DataRow drd = dt.NewRow();
            //    dt.Rows.Add(drd);
            //}
            dgPVDetailsDtl.DataSource = dt;
            dgPVDetailsDtl.DataBind();
            ShowFooterTotal();
            ((TextBox)dgPVDetailsDtl.Rows[dgPVDetailsDtl.Rows.Count - 1].FindControl("txtQnty")).Focus();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }
  //*************************  txtQnty_TextChanged *******************//

    protected void txtQnty_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            DataTable dt = (DataTable)ViewState["purdtl"];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];
                dr["ID"] = dr["ID"].ToString();
                dr["item_desc"] = dr["item_desc"].ToString();
                dr["item_code"] = dr["item_code"].ToString();
                dr["msr_unit_code"] = dr["msr_unit_code"].ToString();
                dr["item_rate"] = ((TextBox)gvr.FindControl("txtItemRate")).Text;
                if (((TextBox)gvr.FindControl("txtQnty")).Text == "") { dr["qnty"] = "0"; }
                dr["qnty"] = ((TextBox)gvr.FindControl("txtQnty")).Text;

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
            dgPVDetailsDtl.DataSource = dt;
            dgPVDetailsDtl.DataBind();
            ShowFooterTotal();
            DueAmount();
            ((TextBox)dgPVDetailsDtl.Rows[dgPVDetailsDtl.Rows.Count - 1].FindControl("txtItemDesc")).Focus();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }
    //*************************  txtAdditional_TextChanged *******************//

    protected void txtAdditional_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            DataTable dt = (DataTable)ViewState["purdtl"];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];
                if (((TextBox)gvr.FindControl("txtAdditional")).Text == "") { dr["Additional"] = "0"; }
                dr["Additional"] = ((TextBox)gvr.FindControl("txtAdditional")).Text;
            }
            dgPVDetailsDtl.DataSource = dt;
            dgPVDetailsDtl.DataBind();
            ShowFooterTotal();
            ((TextBox)dgPVDetailsDtl.Rows[dgPVDetailsDtl.Rows.Count - 1].FindControl("txtItemDesc")).Focus();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }
   
    protected void ddlParty_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["AddressParty"] = IdManager.GetShowSingleValueString("Address", "ID", "PartyInfo", ddlParty.SelectedValue);
        ViewState["PhoneParty"] = IdManager.GetShowSingleValueString("Phone", "ID", "PartyInfo", ddlParty.SelectedValue);
        Session["Party_COA"] = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "PartyInfo", ddlParty.SelectedValue);
    }
    protected void txtPO_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = PurchaseOrderManager.GetShowOrder(txtPO.Text);
        if (dt.Rows.Count > 0)
        {
            lblOrNo.Text = dt.Rows[0]["ID"].ToString();
            txtPO.Text = dt.Rows[0]["PO"].ToString();
            txtPODate.Text =Convert.ToDateTime(dt.Rows[0]["PODate"]).ToString("dd/MM/yyyy");
            hfSupplierID.Value = dt.Rows[0]["SupplierID"].ToString();
            Supplier sup = SupplierManager.GetSupplier(dt.Rows[0]["SupplierID"].ToString());
            if (sup != null)
            {
                txtSupplierSearch.Text = sup.SupName;
                ViewState["Address"] = sup.SupAddr1;
                ViewState["Phone"] = sup.SupPhone;
                ViewState["Supplier_COA"] = sup.GlCoa;
                lblPhoneNo.Text = "Ph : " + sup.SupPhone + " Mob : " + sup.SupMobile;
            }
            //txtSupplierPhone.Text = IdManager.GetShowSingleValueString("Phone", "ID", "Supplier", ddlSupplier.SelectedValue);
            //ViewState["Supplier_COA"] = IdManager.GetShowSingleValueString("Gl_CoaCode", "ID", "Supplier", ddlSupplier.SelectedValue);
           // lblOrNo.Text = dt.Rows[0]["ID"].ToString();
            DataTable dt1 = PurchaseOrderManager.GetPurchaseOrderItemsDetails(dt.Rows[0]["ID"].ToString());
            dgPVDetailsDtl.DataSource = dt1;
            ViewState["purdtl"] = dt1;
            dgPVDetailsDtl.DataBind();
            ShowFooterTotal();
            tabVch.Visible = true;
           // dgPOrderMst.Visible = false;
            PVI_UP.Update();
            PVIesms_UP.Update();
            txtGRNO.Enabled = txtPO.Enabled = txtGRNODate.Enabled = txtSupplierSearch.Enabled =txtPODate.Enabled= false;
        }
    }
    protected void ddlPaymentMethord_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlPaymentMethord.SelectedValue == "C")
        { 
            VisiblePayment(false,false,false,false,false,false,false,false);
            lblAmount.Text = "Cash Amount ";
            ddlChequeStatus.SelectedIndex = -1;
        }
        else if (ddlPaymentMethord.SelectedValue == "Q")
        {
            VisiblePayment(true, true, true, true, true, true, true, true);
            lblAmount.Text = "Cheque Amount ";
            ddlChequeStatus.SelectedIndex = 1;
        }
        else if (ddlPaymentMethord.SelectedValue == "CR")
        {
            VisiblePayment(false, false, true, true, true, true, true, true);
            lblAmount.Text = "Card Amount ";
        }
        else
        {
            VisiblePayment(false, false, false, false, false, false, false, false);
            lblAmount.Text = "Cash Amount ";
            ddlChequeStatus.SelectedIndex = -1;
        }
        UP1.Update();
        UP2.Update();
    }
    public void VisiblePayment(bool lblBank,bool Bank,bool lblChkNo,bool ChkNo,bool lblChkDate,bool chkdate,bool lblChkStatus,bool chkStatus)
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
        txtChequeAmount.Text = "0";
        txtChequeAmount.Focus();
    }
    protected void txtGrnNo_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = PurchaseVoucherManager.GetShowPVMasterInfo(txtGrnNo.Text);
        
        if (dt.Rows.Count > 0)
        {
            txtGrnNo.Text = dt.Rows[0]["GRN"].ToString();
            //txtID.Text = dgPVMst.SelectedRow.Cells[7].Text;
            //btnFind_Click(sender, e);

        }
        //Up1.Update();
    }
    protected void btnFind_Click(object sender, EventArgs e)
    {

        PurchaseVoucherInfo purmst = PurchaseVoucherManager.GetPurchaseMst(txtID.Text);
        if (purmst != null)
        {
            txtChallanNo.Enabled = txtPO.Enabled = txtGRNODate.Enabled = txtChallanDate.Enabled = txtSupplierSearch.Enabled = txtRemarks.Enabled = true;
            //txtID.Text = dgPVMst.SelectedRow.Cells[7].Text;
            txtGRNO.Text = purmst.GoodsReceiveNo;
            txtGRNODate.Text = purmst.GoodsReceiveDate;
            if (!string.IsNullOrEmpty(purmst.PurchaseOrderNo))
            {
                DataTable dtPurOrder =
                    IdManager.GetShowDataTable("select * from View_SearchPurchaseOrder where ID='" +
                                               purmst.PurchaseOrderNo + "' ");
                lblOrNo.Text = purmst.PurchaseOrderNo;
                txtPO.Text = dtPurOrder.Rows[0]["PO"].ToString();
                txtPODate.Text = Convert.ToDateTime(dtPurOrder.Rows[0]["PODate"]).ToString("dd/MM/yyyy");
            }
            else
            {
                txtPO.Text = purmst.PvOrder;
            }
            txtPODate.Text = purmst.PurchaseOrderDate;
            txtChallanNo.Text = purmst.ChallanNo;
            txtChallanDate.Text = purmst.ChallanDate;
            hfSupplierID.Value = purmst.Supplier;
            if (purmst.AdvancePayFlag == "1")
            {
                chkAdvance.Checked = true;
            }
            Supplier sup = SupplierManager.GetSupplier(purmst.Supplier);
            if (sup != null)
            {
                txtSupplierSearch.Text = sup.SupName;
                ViewState["Address"] = sup.SupAddr1;
                ViewState["Phone"] = sup.SupPhone;
                ViewState["Supplier_COA"] = sup.GlCoa;
                lblPhoneNo.Text = "Ph : " + sup.SupPhone + " Mob : " + sup.SupMobile;
            }
            txtRemarks.Text = purmst.Remarks;
            txtTotalAmount.Text = Convert.ToDouble(purmst.TotalAmount).ToString("N2");
            txtTotPayment.Text = Convert.ToDouble(purmst.TotalPayment).ToString("N2");
            txtDiscountAmt.Text = Convert.ToDouble(purmst.DiscountAmt).ToString("N2");
            ddlCarriagePerson.SelectedValue = purmst.CarriagePerson;
            txtCarriageCharge.Text = Convert.ToDouble(purmst.CarriageCharge).ToString("N2");
            ddlLaburePerson.SelectedValue = purmst.LaburePerson;
            txtLabureCharge.Text = Convert.ToDouble(purmst.LabureCharge).ToString("N2");
            txtOtherCharge.Text = Convert.ToDouble(purmst.OtherCharge).ToString("N2");
            ddlPaymentMethord.SelectedValue = purmst.PaymentMethord.Trim();
            
            ddlChequeStatus.SelectedValue = purmst.ChkStatus;
           
            txtShiftmentNo.Text = purmst.ShiftmentNO;
            lblShiftmentID.Text = purmst.ShiftmentID;
            if (purmst.PaymentMethord.Trim() != "C")
            {
                VisiblePayment(true, true, true, true, true, true, true, true);
                try
                {
                    ddlBank.SelectedValue = purmst.BankId;
                }
                catch
                {
                    ddlBank.SelectedValue = null;
                }
                purmst.BankCoaCode = IdManager.GetShowSingleValuestring("Gl_Code",
                    " View_Bank_Branch_Info where ID='" + ddlBank.SelectedValue + "' ");
                txtChequeNo.Text = purmst.ChequeNo;
                txtChequeDate.Text = purmst.ChequeDate;
                txtChequeAmount.Text = Convert.ToDouble(purmst.ChequeAmount).ToString("N0");
            }
            else
            {
                VisiblePayment(false, false, false, false, false, false, false, false);
            }
            DataTable dt = PurchaseVoucherManager.GetPurchaseItemsDetails(txtID.Text);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                dgPVDetailsDtl.DataSource = dt;
                ViewState["purdtl"] = dt;
                ViewState["Oldpurdtl"] = dt;
                dgPVDetailsDtl.DataBind();
                ShowFooterTotal();
            }
            //rbType.SelectedValue = purmst.PvType;
            txtDue.Text = Convert.ToDecimal(purmst.Due).ToString("N2");
            tabVch.Visible =txtShiftmentNo.Enabled= true;
            PanelHistory.Visible = btnNew.Visible = false;
            PVI_UP.Update();
            PVIesms_UP.Update();
            UP1.Update();
            UP2.Update();
        }
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(lblSupplier.Text))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('First entry Supplier.!!');",
                      true);
            return;
        }
        if (ddlPaymentMethord.SelectedValue == "C")
        {
            CashReport();
        }
        else
        {
            BankReport();
        }
    }

    //*************************  Check Popup  *******************//

   
    private void CashReport()
    {




        DataTable dtSupplierDetails = SupplierManager.GetSuppliers(" where id='" + lblSupplier.Text + "' ");
        string filename = "PV_" + txtGRNO.Text;
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
        Document document = new Document(PageSize.LEGAL.Rotate(), 50f, 50f, 40f, 40f);
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
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        
        dth.AddCell(cell);
        string StatusChk = "";
        if (ddlChequeStatus.SelectedValue == "P") { StatusChk = "PENDING CHEQUE STATEMENT"; }
        else if (ddlChequeStatus.SelectedValue == "A") { StatusChk = "HONOURED CHEQUE STATEMENT"; } else { StatusChk = "DISHONOURED CHEQUE STATEMENT"; }
        cell = new PdfPCell(new Phrase("CASH STATEMENT", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);

        cell = new PdfPCell(new Phrase(txtSupplierSearch.Text + "  add: " + dtSupplierDetails.Rows[0]["Address1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(" Mobile Number : " + dtSupplierDetails.Rows[0]["Mobile"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
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
        cell.FixedHeight = 10f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] widthdtl = new float[9] { 45, 20, 45, 30, 20, 20, 23, 20,  20 };
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Particulars Of Goods"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Brand"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Qnty"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Rate"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 2;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);        

        //cell = new PdfPCell(FormatHeaderPhrase("Shipment No"));
        //cell.HorizontalAlignment = 1;
        //cell.VerticalAlignment = 1;
        //cell.BorderColor = BaseColor.LIGHT_GRAY;
        //cell.Rowspan = 2;
        //cell.PaddingTop = 10;
        //pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Status"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Expiration date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

     

       
        cell = new PdfPCell(FormatPhrase("Purchase Rate"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);       

        DataTable dt = (DataTable)ViewState["purdtl"];
        //DataRow dr1 = dt.NewRow();
        //dt.Rows.Add(dr1);
        int Serial = 1;
        decimal totQty = 0;
        decimal tot = 0;
        decimal GrandTot = 0;
        foreach (DataRow dr in dt.Rows)
        {
            if (dr["item_code"].ToString() != "" && dr["item_desc"].ToString() != "")
            {
                cell = new PdfPCell(FormatPhrase(Serial.ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                //string GRN = "";
                //if (Serial == 1)
                //{ GRN = txtGRNODate.Text; }
                cell = new PdfPCell(FormatPhrase(DataManager.DateEncode(txtGRNODate.Text).ToString(IdManager.DateFormat())));
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

                cell = new PdfPCell(FormatPhrase(dr["qnty"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                decimal total = Convert.ToDecimal(dr["item_rate"]) * Convert.ToDecimal(dr["qnty"]);
               // decimal totAdd = (total) + ((total * Convert.ToDecimal(dr["Additional"])) / 100);
                

                cell = new PdfPCell(FormatPhrase(dr["Expdate"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

               

                cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["item_rate"]).ToString("N2")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(total.ToString("N2")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);                
                

               // cell = new PdfPCell(FormatPhrase(txtSiftment.Text));
                //cell.HorizontalAlignment = 2;
                //cell.VerticalAlignment = 1;
                //cell.BorderColor = BaseColor.LIGHT_GRAY;
                //pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(ddlChequeStatus.SelectedItem.Text));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                //tot += Convert.ToDecimal(dr["Total"]);
                //totQty += Convert.ToDecimal(dr["qnty"]);

                GrandTot += total;
                Serial++;
            }
        }

        cell = new PdfPCell(FormatPhrase("Grand Total"));
       // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 8;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(GrandTot.ToString("N2")));
        //cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(""));
        //cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);

        PdfPTable pdt1 = new PdfPTable(1);
        cell.FixedHeight = 10f;
        cell.Border = 0;
        pdt1.AddCell(cell);


        cell = SignatureFormat(document, cell);       

        document.Close();
        Response.Flush();
        Response.End();
    }

    private void BankReport()
    {
        DataTable dtSupplierDetails = SupplierManager.GetSuppliers(" where id='" + hfSupplierID.Value + "' ");
        string filename = "PV_" + txtGRNO.Text;
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".pdf");
        Document document = new Document(PageSize.LEGAL.Rotate(), 50f, 50f, 40f, 40f);
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
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        dth.AddCell(cell);
        string StatusChk = "";
        if (ddlChequeStatus.SelectedValue == "P") { StatusChk = "PENDING CHEQUE STATEMENT"; }
        else if (ddlChequeStatus.SelectedValue == "A") { StatusChk = "HONOURED CHEQUE STATEMENT"; } else { StatusChk = "DISHONOURED CHEQUE STATEMENT"; }
        cell = new PdfPCell(new Phrase(StatusChk, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        // cell.FixedHeight = 20f;
        dth.AddCell(cell);

        cell = new PdfPCell(new Phrase(txtSupplierSearch.Text + "  add: " + dtSupplierDetails.Rows[0]["Address1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Colspan = 7;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(" Mobile Number : " + dtSupplierDetails.Rows[0]["Mobile"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
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
        cell.FixedHeight = 10f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] widthdtl = new float[14] { 15, 20, 45, 30, 20, 20, 23, 22, 20,20,20,20,20,20};
        PdfPTable pdtdtl = new PdfPTable(widthdtl);
        pdtdtl.WidthPercentage = 100;

        cell = new PdfPCell(FormatHeaderPhrase("Serial"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Particulars Of Goods"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Brand"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Quantity"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Rate"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 3;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Amount"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Cheque Status"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Status"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Rowspan = 2;
        cell.PaddingTop = 10;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Expiration date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Sales Rate"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Purchase Rate"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Issue Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Cheque No"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);
        cell = new PdfPCell(FormatPhrase("Payment Date"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase("Bank"));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        DataTable dt = (DataTable)ViewState["purdtl"];
        //DataRow dr1 = dt.NewRow();
        //dt.Rows.Add(dr1);
        int Serial = 1;
        decimal totQty = 0;
        decimal tot = 0;
        decimal GrandTotal = 0;
        foreach (DataRow dr in dt.Rows)
        {
            if (dr["item_code"].ToString() != "" && dr["item_desc"].ToString() != "")
            {
                cell = new PdfPCell(FormatPhrase(Serial.ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                //string GRN = "";
                //if (Serial == 1)
                //{ GRN = txtGRNODate.Text; }
                cell = new PdfPCell(FormatPhrase(DataManager.DateEncode(txtGRNODate.Text).ToString(IdManager.DateFormat())));
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

                cell = new PdfPCell(FormatPhrase(dr["qnty"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                decimal total = Convert.ToDecimal(dr["item_rate"]) * Convert.ToDecimal(dr["qnty"]);
                decimal totAdd = (total) + ((total * Convert.ToDecimal(dr["Additional"])) / 100);
                decimal totaddPer = ((total * Convert.ToDecimal(dr["Additional"])) / 100);

                GrandTotal += total;
                cell = new PdfPCell(FormatPhrase(dr["Expdate"].ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["item_sales_rate"]).ToString()));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(Convert.ToDecimal(dr["item_rate"]).ToString("N2")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(totAdd.ToString("N3")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(txtGRNODate.Text));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(txtChequeNo.Text));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(txtChequeDate.Text));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(ddlBank.SelectedItem.Text));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(ddlChequeStatus.SelectedItem.Text));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                //tot += Convert.ToDecimal(dr["Total"]);
                totQty += Convert.ToDecimal(dr["qnty"]);

                Serial++;
            }
        }

        cell = new PdfPCell(FormatPhrase("Grand Total"));
        //cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 4;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(totQty.ToString("N2")));
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
        // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.Colspan = 3;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(GrandTotal.ToString("N2")));
       // cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(""));
      //  cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 5;
        pdtdtl.AddCell(cell);

        document.Add(pdtdtl);

        PdfPTable pdt1 = new PdfPTable(1);
        cell.FixedHeight = 10f;
        cell.Border = 0;
        pdt1.AddCell(cell);

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

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }
    protected void txtShiftmentNo_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = ShiftmentAssignManager.GetShowShiftmentAssignOnSearch(txtShiftmentNo.Text);
        if (dt.Rows.Count > 0)
        {
            txtShiftmentNo.Text = dt.Rows[0]["ShiftmentNO"].ToString();            
            lblShiftmentID.Text = dt.Rows[0]["ID"].ToString();
            txtRemarks.Focus();
        }
    }
    protected void BtnSearch_Click(object sender, EventArgs e)
    {
     
        DataTable dt = PurchaseVoucherManager.GetShowPurchaseMst(txtGrnNo.Text,lblSupplier.Text,txtFromDate.Text,txtToDate.Text);
        dgPVMst.DataSource = dt;
        dgPVMst.DataBind();
       // ViewState["purdtl"] = dt;

    }
    protected void txtSupplier_TextChanged(object sender, EventArgs e)
    {
        DataTable dtSupplier = PurchaseVoucherManager.GetSupplierInfo(txtSupplier.Text);
        if(dtSupplier.Rows.Count>0)
        {
            txtSupplier.Text = dtSupplier.Rows[0]["SupplierSearch"].ToString();
            lblSupplier.Text = dtSupplier.Rows[0]["ID"].ToString();
        }
        UPSupplier.Update();
    }
    protected void Refresh_Click(object sender, EventArgs e)
    {
        DataTable dt = PurchaseVoucherManager.GetShowPurchaseMst();
        dgPVMst.DataSource = dt;
        Session["PvMst"] = dt;
        dgPVMst.DataBind();
        txtSupplier.Text = txtToDate.Text = txtFromDate.Text = txtGrnNo.Text = lblSupplier.Text = string.Empty;
        //Up1.Update();
    }
    protected void txtSupplierSearch_TextChanged(object sender, EventArgs e)
    {
        DataTable dtSupplier = PurchaseVoucherManager.GetSupplierInfo(txtSupplierSearch.Text);
        if (dtSupplier.Rows.Count > 0)
        {
            ViewState["Supplier_COA"] = dtSupplier.Rows[0]["Gl_CoaCode"].ToString();
            txtSupplierSearch.Text = dtSupplier.Rows[0]["SupplierSearch"].ToString();
            lblPhoneNo.Text = "Ph : " + dtSupplier.Rows[0]["Phone"].ToString() + " Mob : " + dtSupplier.Rows[0]["Mobile"].ToString();
            hfSupplierID.Value = dtSupplier.Rows[0]["ID"].ToString();
            txtRemarks.Text = "Item Purchase : " + txtSupplierSearch.Text + ", " + lblPhoneNo.Text;
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Not Found Supplier.!!');", true);
            txtSupplier.Text = hfSupplierID.Value = "";
            txtSupplier.Focus();
        }
        PVI_UP.Update();
        PVIesms_UP.Update();
    }

    public void DueAmount()
    {
        try
        {
            if (string.IsNullOrEmpty(txtChequeAmount.Text))
            {
                txtChequeAmount.Text = "0";
            }
            txtTotPayment.Text = txtChequeAmount.Text;

            txtDue.Text = (Convert.ToDouble(txtTotalAmount.Text) - Convert.ToDouble(txtChequeAmount.Text) - Convert.ToDouble(txtDiscountAmt.Text)).ToString("N2");

            UP1.Update();
            UP2.Update();
        }
        catch
        {
          
            txtChequeAmount.Text = "0";
            txtTotPayment.Text = "0";
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Check your Pay amount..!!');", true);
        }
    }
    protected void txtChequeAmount_TextChanged(object sender, EventArgs e)
    {
        try
        {
           DueAmount();
        }
        catch
        {
            txtTotPayment.Text = "0";
            txtChequeAmount.Text = "0";
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Check your Pay amount..!!');", true);
        }
    }

    protected void ddlChequeStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        UP1.Update();
        UP2.Update();
    }
    protected void txtDiscountAmt_TextChanged(object sender, EventArgs e)
    {
        DueAmount();

    }
    protected void txtBarcode_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;

            DataTable dt = (DataTable)ViewState["purdtl"];

            DataRow dr1 = dt.Rows[gvr.DataItemIndex];
            var Barcode = ((TextBox)gvr.FindControl("txtBarcode")).Text;
            if (dt.Rows.Count>0 && dt.Rows[0]["Id"].ToString()!="")
            {
                //for (int i = dt.Rows.Count; i> dt.Rows.Count-1; i--)
                //{
                    
                //}



                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Barcode"].ToString()==Barcode)
                    {


                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                            "alert('The Same Barcode Already Exists this GridView ..!!');", true);
                        ((TextBox)gvr.FindControl("txtBarcode")).Text = "";
                        ((TextBox)gvr.FindControl("txtBarcode")).Focus();
                        return;
                    }
                }
                int i = dt.Rows.Count;
                var Id = dt.Rows[i-1]["Id"].ToString();
               bool IsExistBarcode=_aPurchaseVoucherManager.IsExistBarcode(Id,Barcode);
               if (IsExistBarcode)
               {
                   ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                       "alert('The Same Barcode Already Exists Stock  ..!!');", true);
                   ((TextBox)gvr.FindControl("txtBarcode")).Text = "";
                   ((TextBox)gvr.FindControl("txtBarcode")).Focus();
                   return;
               }

            }

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];
                dr["Barcode"] = ((TextBox)gvr.FindControl("txtBarcode")).Text;
            }
            //string found = "";
            //foreach (DataRow drd in dt.Rows)
            //{
            //    if (drd["item_code"].ToString() == "" && drd["item_desc"].ToString() == "")
            //    {
            //        found = "Y";
            //    }
            //}
            //if (found == "")
            //{
            //    DataRow drd = dt.NewRow();
            //    dt.Rows.Add(drd);
            //}
            dgPVDetailsDtl.DataSource = dt;
            dgPVDetailsDtl.DataBind();
            ShowFooterTotal();
            ((TextBox)dgPVDetailsDtl.Rows[dgPVDetailsDtl.Rows.Count - 1].FindControl("txtItemRate")).Focus();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + ex + "');", true);
        }
    }
}