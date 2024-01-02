using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Delve;
using DocumentFormat.OpenXml.Office2010.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System.Data.SqlClient;
using Color = System.Drawing.Color;

public partial class PurchaseReturn : System.Web.UI.Page
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
            Response.Redirect("Home.aspx?sid=sam");
        } 
        if (!IsPostBack)
        {
            try
            {
                ViewState["PVID"] = "";
                Session["purdtl"] = null;
                ViewState["OldpurRtndtl"] = null;
                getEmptyDtl();
                txtReturnDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                DataTable dt1 = IdManager.GetShowDataTable("SELECT * FROM [FixGlCoaCode]");
                Session["Cash_Code"] = dt1.Rows[0]["CashInHand_BD"].ToString();
                Session["Cash_Name"] = dt1.Rows[0]["CashName_BD"].ToString();
                txtGoodsReceiveNo.Enabled = txtSupplier.Enabled = txtReturnNO.Enabled = txtRemarks.Enabled = false;
                dgPRNMst.Visible = true;
                btnSave.Enabled = true;
                PVIesms_UP.Visible = false;
                DataTable dt = PVReturnManager.GetShowPurchaseReturnItems();
                dgPRNMst.DataSource = dt;
                ViewState["mst"] = dt;
                dgPRNMst.DataBind();
                btnNew.Visible =SearchID.Visible= true;
                VisiblePayment(false, false, false, false, false, false, false, false);
                txtTotal.Text = txtTotPayment.Text = txtDue.Text = "0";
                string query2 =
                    "select '' [bank_id],'' [bank_name]  union select convert(nvarchar,[bank_id]) ,[bank_name] from [bank_info] order by 1";
                util.PopulationDropDownList(ddlBank, "bank_info", query2, "bank_name", "bank_id");
                if (Session["GRNPV"] != null)
                {
                    btnNew_Click(sender, e);
                    txtGoodsReceiveNo_TextChanged(sender, e);
                  //  Session["GRNPV"] = null;
                }

                if (Session["PurchaseRtn"] != null)
                {
                    lbLId.Text = Session["PurchaseRtn"].ToString();
                    PurchaseRtn();
                    Session["PurchaseRtn"] = null;
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
        else
        {
            DataTable dtDtl = (DataTable) Session["purdtl"];
            ShowFooterTotal(dtDtl);
        }
    }
    private void getEmptyDtl()
    {
        dgPODetailsDtl.Visible = true;
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("ID", typeof(string));
        dtDtlGrid.Columns.Add("item_code", typeof(string));
        dtDtlGrid.Columns.Add("item_desc", typeof(string));
        dtDtlGrid.Columns.Add("msr_unit_code", typeof(string));
        dtDtlGrid.Columns.Add("item_rate", typeof(string));
        dtDtlGrid.Columns.Add("qnty", typeof(string));
        dtDtlGrid.Columns.Add("PvQty", typeof(string));
        dtDtlGrid.Columns.Add("PurchaseQty", typeof(string));
        dtDtlGrid.Columns.Add("ReturnQty", typeof(string));
        dtDtlGrid.Columns.Add("ExpireDate", typeof(string));
        dtDtlGrid.Columns.Add("SalesPrice", typeof(string));
        DataRow dr = dtDtlGrid.NewRow();
        dr["qnty"] = "0";
        dr["ReturnQty"] = "0";
        dtDtlGrid.Rows.Add(dr);
        dgPODetailsDtl.DataSource = dtDtlGrid;
        Session["purdtl"] = dtDtlGrid;
        dgPODetailsDtl.DataBind();
        ShowFooterTotal(dtDtlGrid);
    }
    protected void txtGoodsReceiveNo_TextChanged(object sender, EventArgs e)
    {
        ViewState["PVID"] = "";
        lblGlCoa.Text = "";
        DataTable dt = null;
        if (Session["GRNPV"] != null)
        {
            dt = PVReturnManager.GetShowPVMasterInfo(Session["GRNPV"].ToString());
            txtGoodsReceiveNo.Text = Session["GRNPV"].ToString();
        }
        else
        {
            dt = PVReturnManager.GetShowPVMasterInfo(txtGoodsReceiveNo.Text);
        }
        if (dt.Rows.Count > 0)
        {
            txtSupplier.Text = dt.Rows[0]["ContactName"].ToString();
            ViewState["PVID"] = dt.Rows[0]["ID"].ToString();
            lblPVID.Text = dt.Rows[0]["ID"].ToString();
            lblGlCoa.Text = dt.Rows[0]["Gl_CoaCode"].ToString();
            lblSupID.Text = dt.Rows[0]["SupplierID"].ToString();
            lblType.Text = dt.Rows[0]["PvType"].ToString();
            DataTable dataTable = (DataTable)Session["purdtl"];
            dgPODetailsDtl.DataSource = Session["purdtl"];
            dgPODetailsDtl.DataBind();
            PVIesms_UP.Update();
            Session["GRNPV"] = null;
        }
    }
   
    public DataTable PopulatePayType()
    {
        DataTable dt = PVReturnManager.GetShowPurchaseItems(ViewState["PVID"].ToString());
        DataRow dr = dt.NewRow();
        dt.Rows.InsertAt(dr, 0);        
        return dt;
    }
    protected void btnNew_Click(object sender, EventArgs e)
    {
        dgPRNMst.Visible = SearchID.Visible = false;
        PVIesms_UP.Visible = true;
        txtGoodsReceiveNo.Text =txtSupplier.Text=txtReturnNO.Text=txtRemarks.Text= "";
        txtGoodsReceiveNo.Enabled = txtSupplier.Enabled = txtRemarks.Enabled = true;
        txtReturnNO.Text = IdManager.GetDateTimeWiseSerial("PRN", "Return_No", "[PurReturnMst]");
        txtGoodsReceiveNo.Focus();
        btnNew.Visible = false;
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (txtGoodsReceiveNo.Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Select Goods Receive No...!!');", true);
        }
        else if (Session["purdtl"] == null)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There are no items in list.!!');", true);
        }
        else if (ddlPaymentMethord.SelectedValue == "Q" && Convert.ToDouble(txtTotPayment.Text) > 0 && ddlChequeStatus.SelectedValue == "P")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Incorrect Check Status.!!');", true);
        }
        else
        {
            PVReturn rtn = PVReturnManager.getShowRetirnItems(lbLId.Text);
            if (rtn != null)
            {
                if (per.AllowEdit == "Y")
                {
                    int CountRTNO = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER(Return_No)", "PurReturnMst",
                        txtReturnNO.Text.ToUpper(),Convert.ToInt32(lbLId.Text));
                    if (CountRTNO > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                            "alert('Warning : \\n this return No. already exist.!!');", true);
                    }
                    rtn.ReturnDate = txtReturnDate.Text;
                    rtn.Remarks = txtRemarks.Text;
                    rtn.LogonBy = Session["user"].ToString();
                    rtn.TotalAmount = txtTotal.Text.Replace(",", "");
                    rtn.Pay_Amount = txtTotPayment.Text.Replace(",", "");
                    rtn.PaymentMethod = ddlPaymentMethord.SelectedValue.Trim();
                    rtn.BankName = ddlBank.SelectedValue.Trim();
                    if (!string.IsNullOrEmpty(ddlBank.SelectedValue.Trim()))
                    {
                        rtn.BankCoaCode = IdManager.GetShowSingleValuestring("Gl_Code",
                            " View_Bank_Branch_Info where ID='" + ddlBank.SelectedValue + "' ");
                    }
                    else
                    {
                        rtn.BankCoaCode = "";
                    }
                    rtn.SupplierName = txtSupplier.Text;
                    rtn.ChequeNo = txtChequeNo.Text.Trim();
                    rtn.ChequeDate = txtChequeDate.Text;
                    rtn.Chk_Status = ddlChequeStatus.SelectedValue.Trim();                  
                    DataTable dt = (DataTable)Session["purdtl"];
                    DataTable OldpurRtndtl = (DataTable)ViewState["OldpurRtndtl"];

                    //********************** Journal Voucher Update *************//

                    string VCH_SYS_NO = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                        "t1.PAYEE='PVR' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                        txtReturnNO.Text);
                    VouchMst vmst = VouchManager.GetVouchMst(VCH_SYS_NO.Trim());
                    if (vmst != null)
                    {
                        vmst.FinMon = FinYearManager.getFinMonthByDate(txtReturnDate.Text);
                        vmst.ValueDate = txtReturnDate.Text;
                        vmst.VchCode = "03";
                        //vmst.SerialNo = txtGRNO.Text.Trim();
                        vmst.Particulars = txtRemarks.Text;
                        vmst.ControlAmt = txtTotal.Text.Replace(",", "");
                        vmst.Payee = "PVR";
                        vmst.CheqAmnt = "0";
                        vmst.UpdateUser = Session["user"].ToString().ToUpper();
                        vmst.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                        vmst.AuthoUserType = Session["userlevel"].ToString();
                    }

                    string VCH_SYS_NO_PVCV = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                        "t1.PAYEE='PvRtnCV' and SUBSTRING(t1.VCH_REF_NO,1,2)='CV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                        txtReturnNO.Text);
                    VouchMst vmstCV = VouchManager.GetVouchMst(VCH_SYS_NO_PVCV.Trim());
                    if (vmstCV != null)
                    {
                        vmstCV.FinMon = FinYearManager.getFinMonthByDate(txtReturnDate.Text);
                        vmstCV.ValueDate = txtReturnDate.Text;
                        vmstCV.VchCode = "02";
                        //vmst.SerialNo = txtGRNO.Text.Trim();
                        vmstCV.RefFileNo = "";
                        vmstCV.Particulars = txtRemarks.Text;
                        vmstCV.ControlAmt = txtTotal.Text.Replace(",", "");
                        //vmst.Payee = "PV";
                        vmstCV.CheqAmnt = "0";
                        vmstCV.UpdateUser = Session["user"].ToString().ToUpper();
                        vmstCV.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                        vmstCV.AuthoUserType = Session["userlevel"].ToString();
                    }
                    else
                    {
                        if (Convert.ToDouble(txtTotPayment.Text) > 0)
                        {
                            //********* CV Voucher *********//
                            vmstCV = new VouchMst();
                            vmstCV.FinMon = FinYearManager.getFinMonthByDate(txtReturnDate.Text);
                            vmstCV.ValueDate = txtReturnDate.Text;
                            vmstCV.VchCode = "02";
                            vmstCV.RefFileNo = "New";
                            vmstCV.VolumeNo = "";
                            vmstCV.SerialNo = txtReturnNO.Text.Trim();
                            vmstCV.Particulars = txtRemarks.Text;
                            vmstCV.ControlAmt = txtTotPayment.Text.Replace(",", "");
                            vmstCV.Payee = "PvRtnCV";
                            vmstCV.CheckNo = txtChequeNo.Text;
                            vmstCV.CheqDate = txtChequeDate.Text;
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

                    PVReturnManager.UpdatePurchaseReturn(rtn, dt, OldpurRtndtl, vmst,
                            lblGlCoa.Text, vmstCV);
                    btnSave.Enabled = false;
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record are update successfully...!!');", true);
                    Response.Redirect("PurchaseReturn.aspx");
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
                    int CountRTNO = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER(Return_No)", "PurReturnMst",
                       txtReturnNO.Text.ToUpper());
                    if (CountRTNO > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "Warning",
                            "alert('Warning : \\n this return No. already exist.!!');", true);
                    }
                    rtn = new PVReturn();
                    rtn.GRN = lblPVID.Text;
                    rtn.ReturnDate = txtReturnDate.Text;
                    rtn.Remarks = txtRemarks.Text;
                    rtn.LogonBy = Session["user"].ToString();
                    rtn.TotalAmount = txtTotal.Text.Replace(",", "");
                    rtn.Pay_Amount = txtTotPayment.Text.Replace(",", "");
                    rtn.SupplierID = lblSupID.Text;
                    rtn.SupplierName = txtSupplier.Text;
                    rtn.PaymentMethod = ddlPaymentMethord.SelectedValue.Trim();
                    rtn.BankName = ddlBank.SelectedValue.Trim();
                    if (!string.IsNullOrEmpty(ddlBank.SelectedValue.Trim()))
                    {
                        rtn.BankCoaCode = IdManager.GetShowSingleValuestring("Gl_Code",
                            " View_Bank_Branch_Info where ID='" + ddlBank.SelectedValue + "' ");
                    }
                    else
                    {
                        rtn.BankCoaCode = "";
                    }
                    rtn.ChequeNo = txtChequeNo.Text.Trim();
                    rtn.ChequeDate = txtChequeDate.Text;
                    rtn.Chk_Status = ddlChequeStatus.SelectedValue.Trim();
                    DataTable dt = (DataTable)Session["purdtl"];
                    txtReturnNO.Text = IdManager.GetDateTimeWiseSerial("PRN", "Return_No", "[PurReturnMst]");
                    rtn.Return_No = txtReturnNO.Text;

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
                    vmst.Payee = "PVR";
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

                    //********* CV Voucher *********//
                    VouchMst vmstCR = new VouchMst();
                    vmstCR.FinMon = FinYearManager.getFinMonthByDate(txtReturnDate.Text);
                    vmstCR.ValueDate = txtReturnDate.Text;
                    vmstCR.VchCode = "02";
                    vmstCR.RefFileNo = "";
                    vmstCR.VolumeNo = "";
                    vmstCR.SerialNo = txtReturnNO.Text.Trim();
                    vmstCR.Particulars = txtRemarks.Text;
                    vmstCR.ControlAmt = txtTotPayment.Text.Replace(",", "");
                    vmstCR.Payee = "PvRtnCV";
                    vmstCR.CheckNo = txtChequeNo.Text;
                    vmstCR.CheqDate = txtChequeDate.Text;
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

                    int ID = PVReturnManager.SavePurchaseReturn(rtn, dt, vmst,
                            lblGlCoa.Text, vmstCR);
                    btnSave.Enabled = false;
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Record is/are saved successfully...!!');", true);
                    lbLId.Text = ID.ToString();
                    Response.Redirect("PurchaseReturn.aspx");
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You are not Permitted this Step...!!');", true);
                }
            }
        }
    }

    // - string PurchaseCode = "1-6010001";

    const string BDItemPurchaseReturn = "1-7010103";
    const string BDLocalPurchaseReturn = "1-7010104";
    const string ClosingStock = "1-1030002";


    protected void Delete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            DataTable OldpurRtndtl = (DataTable)ViewState["OldpurRtndtl"];
            PVReturn rtn = PVReturnManager.getShowRetirnItems(lbLId.Text);
            if (rtn != null)
            {
                PVReturnManager.DeleteItemsReturn(rtn, OldpurRtndtl);
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
                e.Row.Cells[7].Attributes.Add("style", "display:none");
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
        Session["PurchaseRtn"] = dgPRNMst.SelectedRow.Cells[7].Text;

        string strJS = ("<script type='text/javascript'>window.open('PurchaseReturn.aspx?mno=5.16','_blank');</script>");
        Page.ClientScript.RegisterStartupScript(this.GetType(), "strJSAlert", strJS);
        //PurchaseRtn();
    }

    private void PurchaseRtn()
    {
        txtRemarks.Enabled = txtReturnDate.Enabled = true;
        if (string.IsNullOrEmpty(lbLId.Text))
        {
            lbLId.Text = dgPRNMst.SelectedRow.Cells[7].Text.Trim();
        }
        PVReturn rtn = PVReturnManager.getShowRetirnItems(lbLId.Text);
        if (rtn != null)
        {
            txtGoodsReceiveNo.Text = IdManager.GetShowSingleValueString("t2.GRN", "t1.ID", "[PurReturnMst] t1 inner join ItemPurchaseMst t2 on t2.ID=t1.GRN", lbLId.Text);
            ViewState["PVID"] = "";
            lblGlCoa.Text = "";
            DataTable dt = PVReturnManager.GetShowPVMasterInfo(txtGoodsReceiveNo.Text);
            if (dt.Rows.Count > 0)
            {
                txtSupplier.Text = dt.Rows[0]["ContactName"].ToString();
                ViewState["PVID"] = dt.Rows[0]["ID"].ToString();
                lblPVID.Text = dt.Rows[0]["ID"].ToString();
                lblGlCoa.Text = dt.Rows[0]["Gl_CoaCode"].ToString();
                lblSupID.Text = dt.Rows[0]["SupplierID"].ToString();
                txtReturnDate.Text = rtn.ReturnDate;
                txtReturnNO.Text = rtn.Return_No;
                txtRemarks.Text = rtn.Remarks;

                DataTable dtItems = PVReturnManager.ItemsDetails(lbLId.Text);
                DataRow drRow = dtItems.NewRow();
                drRow["ReturnQty"] = "0";
                dtItems.Rows.Add(drRow);

                Session["purdtl"] = dtItems;
                ViewState["OldpurRtndtl"] = dtItems;
                dgPODetailsDtl.DataSource = dtItems;
                dgPODetailsDtl.DataBind();
                ShowFooterTotal(dtItems);
                dgPRNMst.Visible = btnNew.Visible = false;
                PVIesms_UP.Visible = true;
                PVIesms_UP.Update();
            }
            ddlPaymentMethord.SelectedValue = rtn.PaymentMethod.Trim();
            txtTotPayment.Text = rtn.Pay_Amount;
            txtDue.Text = (Convert.ToDouble(txtTotal.Text) - Convert.ToDouble(txtTotPayment.Text)).ToString("N3");
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
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            DataTable dt = (DataTable)Session["purdtl"];
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            double HigQtyRtn = 0;

            var a = Convert.ToDouble(dr["PvQty"]);
            var b = Convert.ToDouble(dr["PurchaseQty"]);
            var c = Convert.ToDouble(gvr.Cells[9].Text);
            if ((Convert.ToDouble(dr["PvQty"]) + Convert.ToDouble(gvr.Cells[9].Text)) > Convert.ToDouble(dr["PurchaseQty"]))
            {
                HigQtyRtn = Convert.ToDouble(dr["PurchaseQty"]);

            }
            else
            {
                HigQtyRtn = Convert.ToDouble(dr["PvQty"]) + Convert.ToDouble(gvr.Cells[9].Text);
            }
            // DataTable dt = ItemManager.GetItems(((TextBox)gvr.FindControl("txtItemDesc")).Text);
            if (Convert.ToDouble(((TextBox)gvr.FindControl("txtQnty")).Text) > HigQtyRtn)
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                    "alert('Return quantity not upper then stock quantity.!!!');", true);
                ((TextBox)gvr.FindControl("txtQnty")).Text = "0";
                return;
            }
            if (dt.Rows.Count > 0)
            {

                dr["ID"] = dr["ID"].ToString();
                dr["item_desc"] = dr["item_desc"].ToString();
                dr["item_code"] = dr["item_code"].ToString();
                dr["msr_unit_code"] = dr["msr_unit_code"].ToString();
                dr["item_rate"] = ((TextBox)gvr.FindControl("txtItemRate")).Text;
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
            dgPODetailsDtl.DataSource = dt;
            dgPODetailsDtl.DataBind();
            ShowFooterTotal(dt);
            ((DropDownList)dgPODetailsDtl.Rows[dgPODetailsDtl.Rows.Count - 1].FindControl("DropDownList1")).Focus();
            PVIesms_UP.Update();
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
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((DropDownList)sender).NamingContainer;
        DataTable dtdtl = (DataTable)Session["purdtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        DataTable dt = PVReturnManager.GetPVItems(((DropDownList)gvr.FindControl("DropDownList1")).SelectedValue, ViewState["PVID"].ToString());
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
            dr["msr_unit_code"] = ((DataRow)dt.Rows[0])["msr_unit_code"].ToString();
            dr["item_rate"] = ((DataRow)dt.Rows[0])["item_rate"].ToString();
            dr["ExpireDate"] = ((DataRow)dt.Rows[0])["ExpireDate"].ToString();
            dr["SalesPrice"] = ((DataRow)dt.Rows[0])["SalesPrice"].ToString();
            dr["qnty"] = "0";
            dr["ReturnQty"] = "0";
            dr["PvQty"] = ((DataRow)dt.Rows[0])["PvQty"].ToString();
            dr["PurchaseQty"] = ((DataRow)dt.Rows[0])["PurchaseQty"].ToString();
            dtdtl.Rows.InsertAt(dr, gvr.DataItemIndex);
        }
        dgPODetailsDtl.DataSource = dtdtl;
        dgPODetailsDtl.DataBind();
        ShowFooterTotal(dtdtl);
        ((TextBox)dgPODetailsDtl.Rows[dgPODetailsDtl.Rows.Count - 1].FindControl("txtQnty")).Focus();
        PVIesms_UP.Update();
    }
    private void ShowFooterTotal(DataTable DT1)
    {
        decimal tot = 0;
        if (DT1 != null)
        {
            foreach (DataRow dr in DT1.Rows)
            {
                if (dr["item_desc"].ToString() != "")
                {
                    tot += Convert.ToDecimal(dr["item_rate"]) * Convert.ToDecimal(dr["qnty"]);
                }
            }
            txtTotal.Text = tot.ToString("N2");
            txtDue.Text = tot.ToString("N2");
        }
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
        Document document = new Document(PageSize.A4, 20f, 20f, 40f, 40f);
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
        cell = new PdfPCell(new Phrase("Purchase Return", FontFactory.GetFont(FontFactory.TIMES_BOLD, 11, iTextSharp.text.Font.BOLD)));
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
        cell = new PdfPCell(FormatHeaderPhrase("P.Return No "));
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        pdtclient.AddCell(cell);
        cell = new PdfPCell(FormatPhrase(": " + txtReturnNO.Text));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        cell.Colspan = 3;
        pdtclient.AddCell(cell);
        string Phone = IdManager.GetShowSingleValueString("Mobile", "ID", "Customer", lblSupID.Text);
        cell = new PdfPCell(FormatHeaderPhrase("Supplier Name "));
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
        cell = new PdfPCell(FormatHeaderPhrase("GRN. "));
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
        DataTable DT1 = PVReturnManager.ItemsDetails(lbLId.Text);     
        foreach (DataRow dr in DT1.Rows)
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

        cell = new PdfPCell(FormatPhrase("Total"));
        cell.FixedHeight = 20f;
        cell.HorizontalAlignment = 2;
        cell.VerticalAlignment = 1;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        cell.Colspan = 5;
        pdtdtl.AddCell(cell);

        cell = new PdfPCell(FormatPhrase(tot.ToString("N3")));
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
                e.Row.Cells[8].ForeColor = Color.Red;
                if (((DataRowView)e.Row.DataItem)["qnty"].ToString() != "" && ((DataRowView)e.Row.DataItem)["item_rate"].ToString() != "")
                {
                    decimal total = decimal.Parse(((DataRowView)e.Row.DataItem)["item_rate"].ToString()) *
                                   decimal.Parse(((DataRowView)e.Row.DataItem)["qnty"].ToString());
                    ((Label)e.Row.FindControl("lblTotal")).Text = total.ToString("N3");

                }
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[3].Attributes.Add("style", "display:none");
                e.Row.Cells[7].Attributes.Add("style", "display:none");
                e.Row.Cells[9].Attributes.Add("style", "display:none");
                e.Row.Cells[11].Attributes.Add("style", "display:none");

            }
            else if (e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[3].Attributes.Add("style", "display:none");
                e.Row.Cells[7].Attributes.Add("style", "display:none");
                e.Row.Cells[9].Attributes.Add("style", "display:none");
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
    protected void dgPurDtl_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

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
           // lblAmount.Text = "Cheque Amount ";
            ddlChequeStatus.SelectedIndex = 1;
        }
        else if (ddlPaymentMethord.SelectedValue == "CR")
        {
            VisiblePayment(false, false, true, true, true, true, true, true);
           // lblAmount.Text = "Card Amount ";
        }
        else
        {
            VisiblePayment(false, false, false, false, false, false, false, false);
            //lblAmount.Text = "Cash Amount ";
            ddlChequeStatus.SelectedIndex = -1;
        }
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
            txtDue.Text = (Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(txtTotPayment.Text)).ToString("N3");
        }
        catch
        {
            txtTotPayment.Text = "0";
            
           
        }
        ddlPaymentMethord.Focus();
        UPPaymentMtd.Update();
    }
    
    protected void txtSupplier_TextChanged(object sender, EventArgs e)
    {
        DataTable dtSupplier = PVReturnManager.GetSupplierInfo(txtSupplierSearch.Text);
        if (dtSupplier.Rows.Count > 0)
        {
            txtSupplierSearch.Text = dtSupplier.Rows[0]["ContactName"].ToString();
            lblSupplier.Text = dtSupplier.Rows[0]["ID"].ToString();
        }
        else
        {
            txtSupplier.Text = "";
            lblSupplier.Text = "";
        }
        
    }
    protected void BtnSearch_Click(object sender, EventArgs e)
    {
        DataTable dt = PVReturnManager.GetShowPurchaseReturnMst(txtPurReturn.Text, lblSupplier.Text, txtFromDate.Text, txtToDate.Text);
        dgPRNMst.DataSource = dt;
        dgPRNMst.DataBind();
      
        
    }
    protected void Refresh_Click(object sender, EventArgs e)
    {
        DataTable dt = PVReturnManager.GetShowPurchaseReturnItems();
        dgPRNMst.DataSource = dt;
        ViewState["mst"] = dt;
        dgPRNMst.DataBind();
        txtPurReturn.Text = txtSupplierSearch.Text = txtFromDate.Text = txtToDate.Text = string.Empty;
       
    }
}