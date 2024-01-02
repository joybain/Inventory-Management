using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
//using cins;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.pdf.draw;
using System.Reflection;
using System.IO;
using Delve;
//using Delve;

public partial class JournalVoucherUI : System.Web.UI.Page
{
    VouchManager aVouchManager = new VouchManager();
    public static Permis per;
    private int dtlRecordNum = 0;
    List<VouchDtl> _aVouchDtlList = new List<VouchDtl>();
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
        if (!Page.IsPostBack)
        {
            try
            {
                 txtCheckNo.Visible =
                    txtCheqDate.Visible =
                        txtCheqAmnt.Visible =
                            lblCheckNo.Visible =
                                lblCheqAmnt.Visible =
                                    lblCheqDate.Visible =
                                                btnCheqPrint.Visible=false;
                 //string UserType = IdManager.GetShowSingleValueString(" t.UserType", "t.USER_NAME", "UTL_USERINFO t",
                 //       Session["user"].ToString());
                 //ViewState["UserType"] = UserType;
                if (Session["JV_ID"]!=null)
                {
                    txtVchSysNo.Text = txtVchSysNo0.Text = Session["JV_ID"].ToString();
                    Find_Click(e, null);
                    Session["JV_ID"] = null;
                    //txtSerialNo.Enabled = true;
                }
                else
                {
                    btnNew_Click(sender, e);
                    txtParticulars.Text = "Journal :";
                    /************ for Currency Set Check Save Button Disable to check currency ************/
                    //double CurrencyRate = BankAndCashBlanceCheck.GetCurrency(btnSave, txtValueDate, 0);
                    //ViewState["CurrencyRate"] = CurrencyRate;
                    //string UserType = IdManager.GetShowSingleValueString(" t.UserType", "t.USER_NAME", "UTL_USERINFO t",
                    //    Session["user"].ToString());
                    string Parameter = "";
                    //if (!UserType.Equals("3"))
                    //{
                    //    Parameter = " UPPER(ENTRY_USER)='" + Session["user"].ToString().Trim().ToUpper() + "' and PAYEE !='CGORL' ";
                    //}
                    DataTable dt = VouchManager.GetShowAllVoucherWithParameter("JV", Parameter);
                    dgVoucher.DataSource = dt;
                    ViewState["History"] = dt;
                    dgVoucher.DataBind();
                    txtValueDate.Text = System.DateTime.Now.ToString("dd/MM/yyyy");
                    //string Code = DateTime.Now.ToString("MMM").ToUpper() + '-' + (DateTime.Now.Year - 1).ToString().Substring(2, 2) + '-' + DateTime.Now.Year.ToString().Substring(2, 2);
                    //ddlFinMon.SelectedValue = Code;
                    txtValueDate.Attributes.Add("onBlur", "formatdate('" + txtValueDate.ClientID + "')");
                    txtCheqDate.Attributes.Add("onBlur", "formatdate('" + txtCheqDate.ClientID + "')");
                    txtMoneyRptDate.Attributes.Add("onBlur", "formatdate('" + txtMoneyRptDate.ClientID + "')");
                    txtControlAmt.Attributes.Add("onBlur", "setDecimal('" + txtControlAmt.ClientID + "')");
                    txtCheqAmnt.Attributes.Add("onBlur", "setDecimal('" + txtCheqAmnt.ClientID + "')");
                    if (Request.QueryString["vchno"] != null)
                    {
                        if (Request.QueryString["vchno"].ToString() != "")
                        {
                            txtVchSysNo.Text = Request.QueryString["vchno"].ToString();
                            dgVoucherDtl.Columns[0].Visible = true;
                            FindForAuth();
                            VEUnauth();
                            //dgVoucher.Visible = false;
                            ShowFooterTotal();
                        }
                    }
                    txtVchSysNo0.Text = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
                    lbAuth.Visible = btnSave.Enabled = true;
                    tabVch.ActiveTabIndex = 0;
                    txtSerialNo.Focus();
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
    }
    private void RefreshDropDown()
    {
        //txtVchCode.Items.Clear();
        //util.PopulateCombo(txtVchCode, "gl_voucher_type", "vch_desc", "vch_code");
        ddlFinMon.Items.Clear();
        util.PopulateCombo1(ddlFinMon, "gl_fin_month", "fin_mon", "fin_mon");
    }
    private void ClearField()
    {
        txtVchSysNo.Text = String.Empty;
        RefreshDropDown();
        txtValueDate.Text = String.Empty;
        txtVchRefNo.Text = String.Empty;
        txtRefFileNo.Text = String.Empty;
        txtVolumeNo.Text = String.Empty;
        txtSerialNo.Text = String.Empty;
        ddlTransType.SelectedIndex = -1;
        txtVchCode.SelectedIndex = -1;
        txtParticulars.Text = String.Empty;
        txtPayee.Text = String.Empty;
        txtCheckNo.Text = String.Empty;
        txtCheqDate.Text = String.Empty;
        txtCheqAmnt.Text = String.Empty;
        txtMoneyRptDate.Text = String.Empty;
        txtMoneyRptNo.Text = String.Empty;
        txtControlAmt.Text = String.Empty;
        txtStatus.Text = "U";
        loginId.Text = "";
        pwd.Text = "";
        ddlFinMon.SelectedIndex = -1;
        ddlTransType.SelectedIndex = -1;
        txtVchCode.SelectedIndex = -1;
        lbSetNew.Visible = false;
    }
    private void LoadGrid()
    {
        DataTable vchM = VouchManager.GetVouchMstFind("", "", "", "JV",1);
        dgVoucher.DataSource = vchM;
        //dgVoucher.DataBind();
    }
    private void BindGrid()
    {
        dgVoucher.DataBind();
    }
    protected void Clear_Click(object sender, EventArgs e)
    {
        ViewState.Remove("vouchdtl");
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void dgVoucher_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            RefreshDropDown();
            ClearField();
            txtVchSysNo.Text = dgVoucher.SelectedRow.Cells[1].Text.Trim();
            VouchMst vchmst = VouchManager.GetVouchMaster(txtVchSysNo.Text, (Session["userlevel"].ToString()).ToString());
            ddlFinMon.Items.Add(vchmst.FinMon);
            ddlFinMon.SelectedValue = vchmst.FinMon;
            txtValueDate.Text = vchmst.ValueDate;
            txtVchRefNo.Text = vchmst.VchRefNo;
            txtRefFileNo.Text = vchmst.RefFileNo;
            txtVolumeNo.Text = vchmst.VolumeNo;
            txtSerialNo.Text = vchmst.SerialNo;
            //txtVchCode.SelectedValue = vchmst.VchCode;
            //ddlTransType.SelectedValue = vchmst.TransType;
            txtParticulars.Text = vchmst.Particulars;
            txtPayee.Text = vchmst.Payee;
            txtCheckNo.Text = vchmst.CheckNo;
            txtCheqDate.Text = vchmst.CheqDate;
            txtCheqAmnt.Text = vchmst.CheqAmnt;
            txtMoneyRptNo.Text = vchmst.MoneyRptNo;
            txtMoneyRptDate.Text = vchmst.MoneyRptDate;
            txtControlAmt.Text = decimal.Parse(vchmst.ControlAmt).ToString("N3");
            txtStatus.Text = vchmst.Status;
            txtVchSysNo0.Text = txtVchSysNo.Text;
            //dgVoucher.Visible = false;
            //dgVoucherDtl.Visible = true;
            ShowChild_Click(sender, e);
            if (!string.IsNullOrEmpty(vchmst.AuthoUserType))
            {
                if ((vchmst.Status == "A") && (int.Parse(vchmst.AuthoUserType) >= int.Parse(Session["userlevel"].ToString())))
                {
                    lbAuth.Visible = false;
                    VEAuth();
                }
                else
                {
                    lbAuth.Visible = true;
                    VEUnauth();
                    lbSetNew.Visible = true;
                }
            }
            else
            {
                lbAuth.Visible = true;
                VEUnauth();
                lbSetNew.Visible = true;
            }

            //if (vchmst.Status == "U")
            //{
            //    lbAuth.Visible = true;
            //    VEUnauth();
            //    lbSetNew.Visible = true;
            //}
            //else
            //{
            //    lbAuth.Visible = false;
            //    VEAuth();
            //}
            ShowFooterTotal();
            btnPrint.Visible = true;
            txtParticulars.Enabled = true;
            detailsupdatepanal.Update();
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
    protected void lbSetNew_Click(object sender, EventArgs e)
    {
        if (txtVchSysNo.Text != String.Empty)
        {
            VouchMst vchmst = VouchManager.GetVouchMst(txtVchSysNo.Text);
            if (vchmst != null)
            {
                ClearField();
                ddlFinMon.SelectedIndex = -1;
                txtRefFileNo.Text = vchmst.RefFileNo;
                txtVolumeNo.Text = vchmst.VolumeNo;
                txtSerialNo.Text = vchmst.SerialNo;
                txtVchCode.SelectedValue = vchmst.VchCode.ToString();
                ddlTransType.SelectedValue = vchmst.TransType;
                txtParticulars.Text = vchmst.Particulars;
                txtPayee.Text = vchmst.Payee;
                txtCheqAmnt.Text = "";
                txtMoneyRptNo.Text = vchmst.MoneyRptNo;
                txtControlAmt.Text = "0";
                txtStatus.Text = "U";
                DataTable dt = new DataTable();
                if (dt.Columns.Count == 0)
                {
                    dt.Columns.Add("line_no", typeof(string));
                    dt.Columns.Add("gl_coa_code", typeof(string));
                    dt.Columns.Add("particulars", typeof(string));
                    dt.Columns.Add("amount_dr", typeof(string));
                    dt.Columns.Add("amount_cr", typeof(string));
                }
                DataRow dr;
                foreach (GridViewRow gvr in dgVoucherDtl.Rows)
                {
                   
                    dr = dt.NewRow();
                    dr["line_no"] = ((TextBox)gvr.FindControl("txtLineNo")).Text.ToString();
                    dr["gl_coa_code"] = ((TextBox)gvr.FindControl("txtGlCoaCode")).Text.ToString();
                    dr["particulars"] = ((TextBox)gvr.FindControl("txtCoaDesc")).Text.ToString();
                    dr["amount_dr"] = "0";
                    dr["amount_cr"] = "0";
                    dt.Rows.Add(dr);
                }
                dgVoucherDtl.DataSource = dt;
                dgVoucherDtl.DataBind();
                ShowFooterTotal();
                txtValueDate.Focus();
                dgVoucherDtl.Columns[0].Visible = true;
                ViewState["vouchdtl"] = dt;
            }
        }
    }
    protected void Find_Click(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(txtVoucher.Text))
            {
                txtVchSysNo.Text = txtVoucher.Text;
            }
            DataTable vchM = VouchManager.GetVouchMstFind(txtVchSysNo.Text, txtValueDate.Text, txtSerialNo.Text, "JV",1);
            if (vchM.Rows.Count == 1)
            {
                RefreshDropDown();
                VouchMst vchmst = VouchManager.GetVouchMaster(vchM.Rows[0]["vch_sys_no"].ToString(), Session["userlevel"].ToString());
                if (vchmst != null)
                {
                    txtVchSysNo.Text = vchmst.VchSysNo.ToString();
                    txtVchSysNo0.Text = txtVchSysNo.Text;
                    ddlFinMon.Items.Add(vchmst.FinMon);
                    ddlFinMon.SelectedValue = vchmst.FinMon;
                    txtValueDate.Text = vchmst.ValueDate;
                    txtVchRefNo.Text = vchmst.VchRefNo;
                    txtRefFileNo.Text = vchmst.RefFileNo;
                    txtVolumeNo.Text = vchmst.VolumeNo;
                    txtSerialNo.Text = vchmst.SerialNo;
                    txtVchCode.SelectedValue = vchmst.VchCode;
                    ddlTransType.SelectedValue = vchmst.TransType;
                    txtParticulars.Text = vchmst.Particulars;
                    txtPayee.Text = vchmst.Payee;
                    txtCheckNo.Text = vchmst.CheckNo;
                    txtCheqDate.Text = vchmst.CheqDate;
                    txtCheqAmnt.Text = vchmst.CheqAmnt;
                    txtMoneyRptNo.Text = vchmst.MoneyRptNo;
                    txtMoneyRptDate.Text = vchmst.MoneyRptDate;
                    txtControlAmt.Text = decimal.Parse(vchmst.ControlAmt).ToString("N3");
                    txtStatus.Text = vchmst.Status;
                    dgVoucherDtl.Visible = true;
                    dgVoucher.Visible = false;
                    ShowChild_Click(sender, e);

                    /* if the current user's rating is equal or higher than actual rating of voucher authorization, then the voucher will be disabled for edit */
                    if (vchmst.AuthoUserType != "")
                    {
                        if ((vchmst.Status == "A") && (int.Parse(vchmst.AuthoUserType) >= int.Parse(Session["userlevel"].ToString())))
                        {
                            lbAuth.Visible = false;
                            VEAuth();
                        }
                    }
                    else
                    {
                        lbAuth.Visible = true;
                        VEUnauth();
                        lbSetNew.Visible = true;
                    }
                 
                    ShowFooterTotal();
                    lblTranStatus.Visible = false;
                }
            }
            else if (vchM.Rows.Count > 1)
            {
                dgVoucher.Visible = true;
                //dgVoucherDtl.Visible = false;
                dgVoucher.DataBind();
                dgVoucher.DataSource = vchM;
                ViewState["History"] = vchM;
                dgVoucher.DataBind();
            }
            else if (vchM.Rows.Count == 0)
            {
                lbSetNew.Visible = false;
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('No Such Voucher Exist!!');", true);
            }
            txtParticulars.Enabled = txtValueDate.Enabled = true;
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

    public void FindForAuth()
    {
        RefreshDropDown();
        VouchMst vchmst = VouchManager.GetVouchMst(txtVchSysNo.Text.Trim());
        if (vchmst != null)
        {
            ddlFinMon.Items.Add(vchmst.FinMon);
            ddlFinMon.SelectedValue = vchmst.FinMon;
            txtValueDate.Text = vchmst.ValueDate;
            txtVchRefNo.Text = vchmst.VchRefNo;
            txtRefFileNo.Text = vchmst.RefFileNo;
            txtVolumeNo.Text = vchmst.VolumeNo;
            txtSerialNo.Text = vchmst.SerialNo;
            txtVchCode.SelectedValue = vchmst.VchCode.Trim();
            ddlTransType.SelectedValue = vchmst.TransType.ToString();
            txtParticulars.Text = vchmst.Particulars;
            txtPayee.Text = vchmst.Payee;
            txtCheckNo.Text = vchmst.CheckNo;
            txtCheqDate.Text = vchmst.CheqDate;
            txtCheqAmnt.Text = vchmst.CheqAmnt;
            txtMoneyRptNo.Text = vchmst.MoneyRptNo;
            txtMoneyRptDate.Text = vchmst.MoneyRptDate;
            txtControlAmt.Text = vchmst.ControlAmt;
            txtStatus.Text = "U";

            DataTable dt = VouchManager.GetVouchDtl(txtVchSysNo.Text, "");
            dgVoucherDtl.DataSource = dt;
            ViewState["vouchdtl"] = dt;
            dgVoucherDtl.DataBind();
        }
    }
    protected void Delete_Click(object sender, EventArgs e)
    {
        try
        {
            if (per.AllowDelete == "Y")
            {
                if (txtVchSysNo.Text.Trim() == String.Empty)
                {
                    lbAuth.Visible = false;
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Select a voucher first!!');", true);
                    txtVchSysNo.Focus();
                    return;
                }
                if (txtValueDate.Text == "" && txtControlAmt.Text == "" && dgVoucherDtl.Visible == false)
                {
                    lbAuth.Visible = false;
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Select a voucher first!!');", true);
                    return;
                }
                if (txtStatus.Text == "A")
                {
                    if (per.AllowAutho == "Y")
                    {
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('You cannot delete an authorized voucher!!');", true);
                        lbAuth.Visible = false;
                        ShowFooterTotal();
                        return;
                        // , Session["user"].ToString().ToUpper()
                    }
                }
                VouchMst vchmst = VouchManager.GetVouchMst(txtVchSysNo.Text.Trim());
                if (vchmst != null)
                {
                    if (vchmst.Payee != null)
                    {
                        if (vchmst.Payee.Equals("SV") || vchmst.Payee.Equals("PV") ||
                            vchmst.Payee.Equals("PVR") || vchmst.Payee.Equals("IT") ||
                            vchmst.Payee.Equals("ITSB"))
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                "alert('Caution:\\n Go to inventory for solved this problem.!!!!');", true);
                            return;
                        }
                    }
                   // VouchMst vchmst = VouchManager.GetVouchMst(txtVchSysNo.Text.Trim());
                    VouchManager.DeleteVouchMst(vchmst, Session["user"].ToString().ToUpper());
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('Record is Successfully Delete........!!!!');", true);
                    txtVchSysNo0.Text = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();

                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('You have no permission to delete this voucher!!');", true);
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

    private void LoadDetailGrid()
    {
        DataTable dtlTable = VouchManager.GetVouchDtl(txtVchSysNo.Text, "");
        dgVoucherDtl.DataSource = dtlTable;
    }
    private void DetailGridBind()
    {
        dgVoucherDtl.DataBind();
        tabVch.ActiveTabIndex = 0;
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        try
        {
            if (per.AllowPrint == "Y")
            {
                if (txtVchSysNo.Text != "")
                {
                    VouchMst vmst = VouchManager.GetVouchMst(txtVchSysNo.Text);
                    //if (!vmst.Status.ToString().Equals("A"))
                    //{
                    //    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    //        "alert('This voucher is not authorized.\\n you can not print this voucher..!!');", true);
                    //    return;
                    //}
                    Session.Remove("rptbyte");
                    getVoucherPdf(vmst);
                    string strJS = ("<script type='text/javascript'>window.open('Default4.aspx','_blank');</script>");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "strJSAlert", strJS);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please select voucher first!!');", true);
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You have no permission to print this voucher!!');", true);
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

    private void getVoucherPdf(VouchMst vmst)
    {
        //VouchMst vmst = VouchManager.GetVouchMst(txtVchSysNo.Text);
        Response.Clear();
        //Response.ContentType = "application/pdf";
        //Response.AddHeader("content-disposition", "attachment; filename='Voucher-Information'.pdf");
        Document document = new Document();
        //float pwidth = (float)(14 / 2.54) * 72;
        //float pheight = (float)(20 / 2.54) * 72;
        //document = new Document(new Rectangle(pwidth, pheight));
        document = new Document(PageSize.A4, 90f, 40f, 40f, 40f);

        //document = new Document(PageSize.A4);
        //Response.OutputStream
        MemoryStream ms = new MemoryStream();
        PdfWriter writer = PdfWriter.GetInstance(document, ms);
        pdfPage page = new pdfPage();
        writer.PageEvent = page;
        document.Open();
        //Rectangle page = document.PageSize;
        //PdfPTable head = new PdfPTable(1);
        //head.TotalWidth = page.Width - 20;
        //Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        //PdfPCell c = new PdfPCell(phrase);
        //c.Border = Rectangle.NO_BORDER;
        //c.VerticalAlignment = Element.ALIGN_BOTTOM;
        //c.HorizontalAlignment = Element.ALIGN_RIGHT;
        //head.AddCell(c);
        //head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPCell cell;
        if (vmst != null)
        {
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
            //cell.FixedHeight = 80f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);
            cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);
            //string vchtype = "";
            //if (vmst.VchCode == "02") { vchtype = "Credit Voucher"; }
            //else if (vmst.VchCode == "01") { vchtype = "Debit Voucher"; }
            //else if (vmst.VchCode == "03") { vchtype = "Journal Voucher"; }

            cell = new PdfPCell(new Phrase("Journal Voucher", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            dth.AddCell(cell);
            document.Add(dth);

            LineSeparator line = new LineSeparator(1f, 100, null, Element.ALIGN_CENTER, -2);
            document.Add(line);
            PdfPTable dtempty = new PdfPTable(1);
            cell = new PdfPCell(FormatHeaderPhrase(""));
            cell.BorderWidth = 0f;
            cell.FixedHeight = 10f;
            dtempty.AddCell(cell);
            document.Add(dtempty);



            float[] widthmst = new float[5] { 20, 25, 15, 20, 20 };
            PdfPTable pdtMst = new PdfPTable(widthmst);
            pdtMst.WidthPercentage = 100;
            cell = new PdfPCell(FormatPhrase("Voucher No"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            //cell.BorderWidth = 0;
            //cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(vmst.VchSysNo));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            // cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatPhrase("Voucher Date"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            // cell.BorderWidth = 0;
            //cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(DataManager.DateEncode(vmst.ValueDate).ToString(IdManager.DateFormat())));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            // cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatPhrase("Reference No"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            //cell.BorderWidth = 0;
            // cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(vmst.VchRefNo));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            //cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            //cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatPhrase("Serial No"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            //cell.BorderWidth = 0;
            // cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(txtSerialNo.Text));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            // cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);

            if (decimal.Parse(string.IsNullOrEmpty(vmst.CheckNo) ? "0" : vmst.CheckNo) > decimal.Zero)
            {
                cell = new PdfPCell(FormatPhrase("Cheque No"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                // cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(vmst.CheckNo));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                //cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                //cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase("Cheque Date"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                //cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(vmst.CheqDate));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                // cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);

                cell = new PdfPCell(FormatPhrase("Cheque Amount"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                // cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(vmst.CheqAmnt));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                //cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                // cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0;
                // cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                // cell.FixedHeight = 18f;
                cell.BorderWidth = 0;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
            }

            cell = new PdfPCell(FormatPhrase("Particulars"));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            // cell.BorderWidth = 0;
            // cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatPhraseOnSingleFont(vmst.Particulars));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.Colspan = 4;
            cell.FixedHeight = 35f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            document.Add(pdtMst);

            DataTable dtdtl = VouchManager.GetVouchDtlRpt(vmst.VchSysNo, "");

            float[] widthdtl = new float[3] { 50, 20, 20 };
            PdfPTable pdtDtl = new PdfPTable(widthdtl);
            pdtDtl.WidthPercentage = 100;
            decimal ctot = decimal.Zero;
            decimal dtot = decimal.Zero;

            cell = new PdfPCell(FormatHeaderPhrase("COA Description"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            // cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtDtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Credit "));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            // cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtDtl.AddCell(cell);

            cell = new PdfPCell(FormatHeaderPhrase("Debit "));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            // cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtDtl.AddCell(cell);

            foreach (DataRow row in dtdtl.Rows)
            {
                if (row["particulars"].ToString() != "")
                {
                    cell = new PdfPCell(FormatPhrase(row["particulars"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    // cell.FixedHeight = 20f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtDtl.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(row["amount_cr"].ToString()));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    // cell.FixedHeight = 20f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtDtl.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(row["amount_dr"].ToString()));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    // cell.FixedHeight = 20f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtDtl.AddCell(cell);

                    if (row["amount_cr"].ToString() != "")
                    {
                        ctot += decimal.Parse(row["amount_cr"].ToString());
                    }
                    if (row["amount_dr"].ToString() != "")
                    {
                        dtot += decimal.Parse(row["amount_dr"].ToString());
                    }
                }
            }
            decimal Total = (ctot + dtot);
            cell = new PdfPCell(FormatHeaderPhrase("Total"));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            // cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtDtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(dtot.ToString("N3")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            // cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtDtl.AddCell(cell);
            cell = new PdfPCell(FormatPhrase(ctot.ToString("N3")));
            cell.HorizontalAlignment = 2;
            cell.VerticalAlignment = 1;
            // cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtDtl.AddCell(cell);
            document.Add(pdtDtl);

            PdfPTable dtempty1 = new PdfPTable(1);
            dtempty1.WidthPercentage = 100;
            cell = new PdfPCell(FormatPhrase("In word: " + DataManager.GetLiteralAmt(dtot.ToString()).Replace("  ", " ").Replace("  ", " ")));
            cell.VerticalAlignment = 0;
            cell.HorizontalAlignment = 0;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 60f;
            dtempty1.AddCell(cell);
            document.Add(dtempty1);

            float[] widthsig = new float[] { 5, 20, 5, 20, 5, 20, 5 };
            PdfPTable pdtsig = new PdfPTable(widthsig);
            pdtsig.WidthPercentage = 100;

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            pdtsig.AddCell(cell);
            cell = new PdfPCell(FormatPhrase("Prepared by/Cashier"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            pdtsig.AddCell(cell);
            cell = new PdfPCell(FormatPhrase("Manager - A & F"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            pdtsig.AddCell(cell);
            cell = new PdfPCell(FormatPhrase("Managing Director"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 1;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtsig.AddCell(cell);

            cell = new PdfPCell(FormatPhrase(""));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Border = 0;
            pdtsig.AddCell(cell);
            document.Add(pdtsig);
        }

        document.Close();
        //Response.Flush();
        //Response.End();
        byte[] byt = ms.GetBuffer();
        if (Session["rptbyte"] != null) { byte[] rptbyt = (byte[])Session["rptbyte"]; rptbyt = byt; } else { Session["rptbyte"] = byt; }
    }
    private static Phrase FormatPhraseOnSingleFont(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7));
    }
    protected void ShowChild_Click(object sender, EventArgs e)
    {
        if (txtVchSysNo.Text.Trim() == String.Empty)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input voucher No First!!');", true);
            txtVchSysNo.Focus();
            return;
        }
        DataTable dt = VouchManager.GetVouchDtl(txtVchSysNo.Text, "");
        if (dt.Rows.Count > 0)
        {
            dgVoucherDtl.DataSource = dt;
            DetailGridBind();
            ViewState["vouchdtl"] = dt;
            //ShowFooterTotal();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('No detail record!!');", true);
            getVoucherDtlGrid();
        }
    }

    protected void DeleteChild_Click(object sender, EventArgs e)
    {
        if (txtVchSysNo.Text.Trim() == String.Empty)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input voucher no first!!');", true);
            txtVchSysNo.Focus();
            return;
        }
        VouchManager.DeleteVouchDtl(txtVchSysNo.Text);
        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Detail record deleted successfully!!');", true);
        ShowChild_Click(sender, e);
        ShowFooterTotal();
    }

    protected void dgVoucherDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string dr = "0";
                string cr = "0";
                if (((DataRowView)e.Row.DataItem)["amount_dr"].ToString() != "")
                {
                    dr = ((DataRowView)e.Row.DataItem)["amount_dr"].ToString();
                }
                if (((DataRowView)e.Row.DataItem)["amount_cr"].ToString() != "")
                {
                    cr = ((DataRowView)e.Row.DataItem)["amount_cr"].ToString();
                }
                ((TextBox)e.Row.FindControl("txtDebit")).Text = decimal.Parse(dr).ToString("N3");
                ((TextBox)e.Row.FindControl("txtCredit")).Text = decimal.Parse(cr).ToString("N3");
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
    protected void dgVoucher_PageIndexChanging1(object sender, GridViewPageEventArgs e)
    {
        dgVoucher.PageIndex = e.NewPageIndex;
        dgVoucher.DataSource = ViewState["History"];
        dgVoucher.DataBind();
    }
    protected void dgVoucherDtl_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["vouchdtl"] != null)
        {
            DataTable dt = (DataTable)ViewState["vouchdtl"];
            DataRow drr = dt.Rows[dgVoucherDtl.Rows[e.RowIndex].DataItemIndex];
            //txtAmt.Text = drr["amount_dr"].ToString();
            dt.Rows.RemoveAt(dgVoucherDtl.Rows[e.RowIndex].DataItemIndex);
            string found = "";
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["gl_coa_code"].ToString() == "" && dr["particulars"].ToString() == "")
                {
                    found = "Y";
                }
            }
            if (found == "")
            {
                DataRow dr = dt.NewRow();
                dr["line_no"] = (dt.Rows.Count + 1).ToString();
                dt.Rows.Add(dr);
            }
            if (dt.Rows.Count > 0)
            {
                int x = 1;
                foreach (DataRow drdt in dt.Rows)
                {
                    drdt.BeginEdit();
                    drdt["line_no"] = x;
                    drdt.AcceptChanges();
                    x = x + 1;
                }
                dgVoucherDtl.DataSource = dt;
                DetailGridBind();
            }
            else
            {
                getVoucherDtlGrid();
            }
            ShowFooterTotal();
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record/s is/are in Delete queue. To Permanently delete click on Update Voucher!!');", true);
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Your session is over. Please do these again!!');", true);
        }
    }

    protected void txtDebit_TextChanged(object sender, EventArgs e)
    {
        try
        {
            TextBox tb = (TextBox)sender;
            GridViewRow row = (GridViewRow)tb.NamingContainer;
            addVouchDtl(row, "Y", "amount_dr");
            ShowFooterTotal();
            ((TextBox)dgVoucherDtl.Rows[dgVoucherDtl.Rows.Count - 1].FindControl("txtCoaDesc")).Focus();
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
    protected void txtCredit_TextChanged(object sender, EventArgs e)
    {
        try
        {
            TextBox tb = (TextBox)sender;
            GridViewRow row = (GridViewRow)tb.NamingContainer;
            addVouchDtl(row, "Y", "amount_cr");
            ShowFooterTotal();
            ((TextBox)dgVoucherDtl.Rows[dgVoucherDtl.Rows.Count - 1].FindControl("txtCoaDesc")).Focus();
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
    protected void txtGlCode_TextChanged(object sender, EventArgs e)
    {
        try
        {
            TextBox tb = (TextBox)sender;
            GridViewRow row = (GridViewRow)tb.NamingContainer;
            addVouchDtl(row, "N", "gl_coa_code");
            ShowFooterTotal();
            ((TextBox)dgVoucherDtl.Rows[dgVoucherDtl.Rows.Count - 1].FindControl("txtCoaDesc")).Focus();
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

    protected void txtCoaDesc_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow row = (GridViewRow)((TextBox)sender).NamingContainer;
            addVouchDtl(row, "N", "coa_desc");
            ShowFooterTotal();
            

            ((TextBox)dgVoucherDtl.Rows[dgVoucherDtl.Rows.Count - 2].FindControl("txtDebit")).Focus();
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
    private void addVouchDtl(GridViewRow row, string ownvalue, string colname)
    {
        DataTable dt = new DataTable();
        string param1 = "";
        string param2 = "";
        //param1 = ((TextBox)row.FindControl("txtGlCoaCode")).Text;
        param2 = ((TextBox)row.FindControl("txtCoaDesc")).Text;
        if (colname != "coa_desc")
        {
            dt = VouchManager.getCoaCode(((TextBox)row.FindControl("txtGlCoaCode")).Text, "");
        }
        else
        {
            dt = VouchManager.getCoaCode("", ((TextBox)row.FindControl("txtCoaDesc")).Text);
        }
        decimal ctlamt = Decimal.Zero;
        string glcode = "";
        string gldesc = "";
        if (txtControlAmt.Text != "")
        {
            ctlamt = decimal.Parse(txtControlAmt.Text);
        }
        if (dt.Rows.Count > 0)
        {
            glcode = ((DataRow)dt.Rows[0])[0].ToString();
            gldesc = ((DataRow)dt.Rows[0])[1].ToString();
        }
        dt.Dispose();

        string found = "";
        int fndline = 0;
        if (glcode != "")
        {
            DataTable dtv = (DataTable)ViewState["vouchdtl"];
            for (int i = 0; i < dtv.Rows.Count; i++)
            {
                if (i != row.DataItemIndex)
                {
                    if (glcode == ((DataRow)dtv.Rows[i])["gl_coa_code"].ToString())
                    {
                        found = "Y";
                        fndline = i + 1;

                    }
                }
            }

            if (found == "Z") // Not Used
            {
                ((TextBox)row.FindControl("txtGlCoaCode")).Text = "";
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You have already use this COA at line " + fndline.ToString() + " !!');", true);
                return;
            }
            else
            {
                dtv.Rows.RemoveAt(row.DataItemIndex);
                DataRow dr = dtv.NewRow();
                dr["line_no"] = ((TextBox)row.FindControl("txtLineNo")).Text;
                dr["gl_coa_code"] = glcode;
                dr["particulars"] = gldesc;

                if (ownvalue == "N")
                {

                    if (decimal.Parse(((TextBox)row.FindControl("txtDebit")).Text) > 0 | decimal.Parse(((TextBox)row.FindControl("txtCredit")).Text) > 0)
                    {
                        dr["amount_dr"] = ((TextBox)row.FindControl("txtDebit")).Text;
                        dr["amount_cr"] = ((TextBox)row.FindControl("txtCredit")).Text;
                    }
                    else
                    {
                        decimal pdr = GetTotal("amount_dr");
                        decimal pcr = GetTotal("amount_cr");
                        if (pdr < decimal.Parse(txtControlAmt.Text))
                        {
                            dr["amount_dr"] = (ctlamt - pdr).ToString("N3");
                            dr["amount_cr"] = "0";
                        }
                        else
                        {
                            if (pcr < decimal.Parse(txtControlAmt.Text))
                            {
                                dr["amount_cr"] = (ctlamt - pcr).ToString("N3");
                                dr["amount_dr"] = "0";
                            }
                        }
                    }
                }
                else if (ownvalue == "Y")
                {
                    dr["amount_dr"] = ((TextBox)row.FindControl("txtDebit")).Text;
                    dr["amount_cr"] = ((TextBox)row.FindControl("txtCredit")).Text;
                }
                dtv.Rows.InsertAt(dr, row.DataItemIndex);
                string nrow = "";
                for (int x = 0; x < dtv.Rows.Count; x++)
                {
                    if (((DataRow)dtv.Rows[x])["gl_coa_code"].ToString() == "" && ((DataRow)dtv.Rows[x])["particulars"].ToString() == "")
                    {
                        nrow = "N";
                    }
                }
                if (nrow == "")
                {
                    //if (dgVoucherDtl.Rows.Count < 2)
                    //{
                        dr = dtv.NewRow();
                        dr["line_no"] = (dtv.Rows.Count + 1).ToString();
                        dtv.Rows.Add(dr);
                   // }
                }

                ViewState["vouchdtl"] = dtv;
                dgVoucherDtl.DataSource = dtv;
                dgVoucherDtl.DataBind();
            }
        }
    }
    private void ShowFooterTotal()
    {
        if (dgVoucherDtl.Rows.Count > 0)
        {
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
            if (Convert.ToDouble(GetTotal("amount_dr")) > 0)
            {
                priceDr = GetTotal("amount_dr");
                txtControlAmt.Text = GetTotal("amount_dr").ToString("N3");
            }
            else
            {
                priceDr = GetTotal("amount_dr");
            }
            cell.Text = priceDr.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);
            cell = new TableCell();
            if (Convert.ToDouble(GetTotal("amount_cr")) > 0)
            {
                priceCr = GetTotal("amount_cr");
                txtControlAmt.Text = GetTotal("amount_cr").ToString("N3");
            }
            else
            {
                priceCr = GetTotal("amount_cr");
            }
            cell.Text = priceCr.ToString("N3");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);
            row.Font.Bold = true;
            //dgVoucherDtl.Visible = true;
            dgVoucherDtl.Controls[0].Controls.Add(row);
            ViewState["pricecr"] = priceCr.ToString();
            ViewState["pricedr"] = priceDr.ToString();
            detailsupdatepanal.Update();
        }
    }
    
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            decimal priceCr = decimal.Zero;
            decimal priceDr = decimal.Zero;
            
            if (ViewState["pricecr"] != null && ViewState["pricedr"] != null)
            {
                priceCr = decimal.Parse(ViewState["pricecr"].ToString());
                priceDr = decimal.Parse(ViewState["pricedr"].ToString());
            }
            if (txtValueDate.Text == "" && txtControlAmt.Text == "" && dgVoucherDtl.Visible == false)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    "alert('Please click on New button for new voucher entry!!');", true);
                return;
            }
            if (txtValueDate.Text == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Voucher date cannot be null!!');",
                    true);
                return;
            }
            if (FinYearManager.getFinMonthByDate(txtValueDate.Text) == string.Empty)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    "alert('Please setup your financial month!!');", true);
                return;
            }
            if (FinYearManager.getFinMonthByDateCheckClose(txtValueDate.Text) == "C")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('financial month are Close!!');", true);
                return;
            }
            if (FinYearManager.getFinMonthByDateNeverOpen(txtValueDate.Text) == "N")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    "alert('financial month are Never Open.please check your financial Year...!!');", true);
                return;
            }
            if (Convert.ToDecimal(txtControlAmt.Text) == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please Input Amount..!!');", true);
                return;
            }
            if (priceCr != priceDr | decimal.Parse(txtControlAmt.Text) != priceCr)
            {
                txtCheckNo.Text = priceCr.ToString() + "-" + priceDr.ToString() + "-" + txtControlAmt.Text;
                ClientScript.RegisterStartupScript(this.GetType(), "ale",
                    "alert('Debit And Credit or Control Amount are Not Equal or No Details!!');", true);
                return;
            }
            if (txtStatus.Text == "A")
            {
                if (!per.AllowAutho.Equals("Y"))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('You cannot delete an authorized voucher!!');", true);
                    lbAuth.Visible = false;
                    ShowFooterTotal();
                    return;
                    // , Session["user"].ToString().ToUpper()
                }
            }


            double CurrencyRate = 0;
            ViewState["CurrencyRate"] = CurrencyRate;
            VouchMst vchmst;
            if (txtVchSysNo.Text == "")
            {
                if (per.AllowAdd == "Y")
                {
                    vchmst = new VouchMst();
                    vchmst.FinMon = ddlFinMon.SelectedItem.Text;
                    vchmst.ValueDate = txtValueDate.Text;
                    vchmst.RefFileNo = txtRefFileNo.Text.ToString();
                    vchmst.VolumeNo = txtVolumeNo.Text.ToString();
                    vchmst.SerialNo = txtSerialNo.Text.ToString();
                    vchmst.VchCode = txtVchCode.SelectedItem.Value;
                    vchmst.TransType = ddlTransType.SelectedValue;
                    vchmst.Particulars = txtParticulars.Text.Replace("'", "''");
                    vchmst.Payee = txtPayee.Text.Replace("'", "''");
                    vchmst.CheckNo = txtCheckNo.Text;
                    vchmst.CheqDate = txtCheqDate.Text;
                    vchmst.CheqAmnt = txtCheqAmnt.Text;
                    vchmst.MoneyRptNo = txtMoneyRptNo.Text;
                    vchmst.MoneyRptDate = txtMoneyRptDate.Text;
                    vchmst.ControlAmt = txtControlAmt.Text;
                    vchmst.Status = txtStatus.Text;
                    vchmst.BookName = Session["book"].ToString();
                    vchmst.EntryUser = Session["user"].ToString().ToUpper();
                    vchmst.AuthoUserType = Session["userlevel"].ToString();
                    vchmst.EntryDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                    vchmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
                    txtVchSysNo.Text = vchmst.VchSysNo;
                    vchmst.VchRefNo = "JV-" + vchmst.VchSysNo.ToString().PadLeft(10, '0');
                    txtVchRefNo.Text = "JV-" + vchmst.VchSysNo.ToString().PadLeft(10, '0');

                    //************* dtVoucherDtl History *************//
                    DataTable dtVoucherDtl = (DataTable) ViewState["vouchdtl"];
                    _aVouchDtlList = VouchManager.getVoucherDtl(dtVoucherDtl, CurrencyRate,
                        "", vchmst);

                    VouchManager.CreateVouchMst(vchmst, _aVouchDtlList);
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('Record is Successfully Saved..!!!');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('You have no permission to add any datam in this form!!');", true);
                }
            }
            else
            {
                if (per.AllowEdit == "Y")
                {
                    vchmst = VouchManager.GetVouchMst(txtVchSysNo.Text.ToString());
                    if (vchmst != null)
                    {
                        if (vchmst.FinMon != ddlFinMon.SelectedItem.Text | vchmst.ValueDate != txtValueDate.Text |
                            vchmst.VchRefNo != txtVchRefNo.Text | vchmst.RefFileNo != txtRefFileNo.Text |
                            vchmst.VolumeNo != txtVolumeNo.Text | vchmst.SerialNo != txtSerialNo.Text |
                            vchmst.VchCode != txtVchCode.SelectedValue | vchmst.Particulars != txtParticulars.Text |
                            vchmst.Payee != txtPayee.Text | vchmst.CheckNo != txtCheckNo.Text |
                            vchmst.CheqDate != txtCheqDate.Text | vchmst.ControlAmt != txtCheqAmnt.Text |
                            vchmst.MoneyRptNo != txtMoneyRptNo.Text | vchmst.MoneyRptDate != txtMoneyRptDate.Text |
                            vchmst.ControlAmt != txtControlAmt.Text | vchmst.Status != txtStatus.Text)
                        {
                            if (vchmst.Payee != null)
                            {
                                if (vchmst.Payee.Equals("SV") || vchmst.Payee.Equals("PV") ||
                                    vchmst.Payee.Equals("PVR") || vchmst.Payee.Equals("IT") ||
                                    vchmst.Payee.Equals("ITSB") || vchmst.Payee.Equals("SVBD"))
                                {
                                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                                        "alert('Caution:\\n Go to inventory for solved this problem.!!!!');", true);
                                    return;
                                }
                            }
                            vchmst.FinMon = ddlFinMon.SelectedItem.Text;
                            vchmst.ValueDate = txtValueDate.Text;
                            vchmst.VchRefNo = txtVchRefNo.Text;
                            vchmst.RefFileNo = txtRefFileNo.Text;
                            vchmst.VolumeNo = txtVolumeNo.Text;
                            vchmst.SerialNo = txtSerialNo.Text;
                            vchmst.VchCode = txtVchCode.SelectedItem.Value;
                            vchmst.TransType = ddlTransType.SelectedValue;
                            vchmst.Particulars = txtParticulars.Text.Replace("'", "''");
                            vchmst.Payee = txtPayee.Text.Replace("'", "''");
                            vchmst.CheckNo = txtCheckNo.Text;
                            vchmst.CheqDate = txtCheqDate.Text;
                            vchmst.CheqAmnt = txtCheqAmnt.Text;
                            vchmst.MoneyRptNo = txtMoneyRptNo.Text;
                            vchmst.MoneyRptDate = txtMoneyRptDate.Text;
                            vchmst.ControlAmt = txtControlAmt.Text;
                            vchmst.Status = txtStatus.Text;
                            vchmst.UpdateUser = Session["user"].ToString().ToUpper();
                            vchmst.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                            //************* dtVoucherDtl History *************//
                            DataTable dtVoucherDtl = (DataTable)ViewState["vouchdtl"];
                            _aVouchDtlList = VouchManager.getVoucherDtl(dtVoucherDtl, CurrencyRate,
                               "", vchmst);
                            VouchManager.UpdateVouchMst(vchmst, _aVouchDtlList);
                        }
                        ClientScript.RegisterStartupScript(this.GetType(), "ale",
                            "alert('Record are update successfully.!!!!');", true);
                        dgVoucherDtl.ShowFooter = false;
                        LoadDetailGrid();
                        DetailGridBind();
                        if (int.Parse(Session["userlevel"].ToString()) > 1)
                        {
                            lbAuth.Visible = true;
                        }
                        else
                        {
                            lbAuth.Visible = false;
                        }
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
                        "alert('You have no permission to edit/update these data!!');", true);
                }
            }

            detailsupdatepanal.Update();
            ShowFooterTotal();
            btnSave.Enabled = false;
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
    private void getVoucherDtlGrid()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("line_no", typeof(string));
        dt.Columns.Add("gl_coa_code", typeof(string));
        dt.Columns.Add("particulars", typeof(string));
        dt.Columns.Add("amount_dr", typeof(string));
        dt.Columns.Add("amount_cr", typeof(string));
        DataRow dr = dt.NewRow();
        dr["line_no"] = "1";
        dt.Rows.Add(dr);
        dgVoucherDtl.DataSource = dt;
        dgVoucherDtl.DataBind();
        ViewState["vouchdtl"] = dt;
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

    protected void btnNew_Click(object sender, EventArgs e)
    {
        try
        {
            ViewState.Remove("vouchdtl");
            ClearField();
            VEUnauth();
            lbAuth.Visible = false;
            btnCheqPrint.Visible = false;
            RefreshDropDown();
            btnCheqPrint.Visible = true;
            txtStatus.Text = "U";
            txtVchSysNo.Enabled = false;
            txtVchRefNo.Enabled = false;
            ddlTransType.SelectedValue = "R";
            txtVchCode.SelectedValue = "01";
            DataTable dt = new DataTable();
            dt.Columns.Add("line_no", typeof(string));
            dt.Columns.Add("gl_coa_code", typeof(string));
            dt.Columns.Add("particulars", typeof(string));
            dt.Columns.Add("amount_dr", typeof(string));
            dt.Columns.Add("amount_cr", typeof(string));
            DataRow dr = dt.NewRow();
            dr["line_no"] = "1";
            dt.Rows.Add(dr);
            dgVoucherDtl.DataSource = dt;
            dgVoucherDtl.DataBind();
            txtControlAmt.Text = "0";
            //dgVoucher.Visible = false;
            //dgVoucherDtl.Visible = true;
            ViewState["vouchdtl"] = dt;
            ShowFooterTotal();
            txtVchCode.Enabled = false;
            txtValueDate.Focus();
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

    protected void LoginBtn_Click(object sender, EventArgs e)
    {
        if (per.AllowAutho == "Y")
        {
            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select password,user_grp from utl_userinfo where upper(user_name)=upper('" + loginId.Text + "')";
            conn.Open();
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read()) 
                    if (pwd.Text.Trim() == dReader["password"].ToString())
                    {
                        if (int.Parse(dReader["user_grp"].ToString()) > 1)
                        {
                            VouchMst vchmst = VouchManager.GetVouchMst(txtVchSysNo.Text.ToString());
                            if (vchmst != null)
                            {
                                vchmst.Status = "A";
                                vchmst.AuthoUser = loginId.Text.ToString().ToUpper();
                                vchmst.AuthoDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                                vchmst.AuthoUserType = dReader["user_grp"].ToString();
                                VouchManager.UpdatVoucherAuthorize(vchmst);
                                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Authorized successfully.......!!');", true);
                                txtStatus.Text = "A";
                                lbAuth.Visible = false;
                                pwd.Text = "";
                                VEAuth();
                            }
                            else
                            {

                                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('No Voucher Show.....!!');", true);
                                return;
                            }
                        }
                        else
                        {

                            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('You have no enough permission to authorize!!..!!');", true);
                            pwd.Text = "";
                            return;
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Password Incorrect, Authentication Failed...!!');", true);
                        pwd.Text = "";
                        return;
                    }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Incorrect user id or password....!!');", true);
            }
            dReader.Close();
            conn.Close();
            ShowFooterTotal();
        }
    }

    protected void CancelBtn_Click(object sender, EventArgs e)
    {
        ShowFooterTotal();
    }
    private void VEAuth()
    {
        txtVchSysNo.Enabled = false;
        txtValueDate.Enabled = false;
        txtVchCode.Enabled = false;
        txtRefFileNo.Enabled = false;
        txtVchRefNo.Enabled = false;
        ddlFinMon.Enabled = false;
        txtPayee.Enabled = false;
        txtCheckNo.Enabled = false;
        txtCheqAmnt.Enabled = false;
        txtCheqDate.Enabled = false;
        txtControlAmt.Enabled = false;
        txtMoneyRptDate.Enabled = false;
        txtMoneyRptNo.Enabled = false;
        txtParticulars.Enabled = false;
        //dgVoucherDtl.Columns[0].Visible = false;
        lbAuth.Visible = false;
        btnCheqPrint.Visible = false;
        txtVolumeNo.Enabled = false;
        //txtSerialNo.Enabled = false;
        ddlTransType.Enabled = false;
        lbSetNew.Visible = true;
        /*
        foreach (GridViewRow gvr in dgVoucherDtl.Rows)
        {

        }
        */
    }
    private void VEUnauth()
    {
        txtVchSysNo.Enabled = true;
        txtValueDate.Enabled = true;
        txtVchCode.Enabled = true;
        txtRefFileNo.Enabled = true;
        txtVchRefNo.Enabled = true;
        ddlFinMon.Enabled = true;
        txtPayee.Enabled = true;
        txtCheckNo.Enabled = true;
        txtCheqAmnt.Enabled = true;
        txtCheqDate.Enabled = true;
        txtControlAmt.Enabled = true;
        txtMoneyRptDate.Enabled = true;
        txtMoneyRptNo.Enabled = true;
        txtParticulars.Enabled = true;
        dgVoucherDtl.Columns[0].Visible = true;
        if (int.Parse(Session["userlevel"].ToString()) > 1)
        {
            lbAuth.Visible = true;
        }
        else
        {
            lbAuth.Visible = false;
        }
        btnCheqPrint.Visible = true;
        txtVolumeNo.Enabled = true;
        //txtSerialNo.Enabled = true;
        ddlTransType.Enabled = true;
    }
    protected void Autho_Click(object sender, EventArgs e)
    {
        if (txtVchSysNo.Text.Trim() == String.Empty)
        {
            //lblTranStatus.Visible = true;
            //lblTranStatus.ForeColor = System.Drawing.Color.Red;
            //lblTranStatus.Text = "Enter voucher no first!";
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input Voucher No First!!');", true);
            txtVchSysNo.Focus();
            return;
        }
    }
    protected void btnCheqPrint_Click(object sender, EventArgs e)
    {
        ShowFooterTotal();
    }
    public override void VerifyRenderingInServerForm(Control control)
    {

    }
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }
    protected void txtVoucher_TextChanged(object sender, EventArgs e)
    {
        try
        {
            VouchMst vchmst = VouchManager.GetVouchMaster(txtVoucher.Text, Session["userlevel"].ToString());
            if (vchmst != null)
            {
                txtVchSysNo.Text = vchmst.VchSysNo.ToString();
                txtVchSysNo0.Text = txtVchSysNo.Text;
                ddlFinMon.Items.Add(vchmst.FinMon);
                ddlFinMon.SelectedValue = vchmst.FinMon;
                txtValueDate.Text = vchmst.ValueDate;
                txtVchRefNo.Text = vchmst.VchRefNo;
                txtRefFileNo.Text = vchmst.RefFileNo;
                txtVolumeNo.Text = vchmst.VolumeNo;
                txtSerialNo.Text = vchmst.SerialNo;
                ddlTransType.SelectedValue = vchmst.TransType;
                txtParticulars.Text = vchmst.Particulars;
                txtPayee.Text = vchmst.Payee;
                txtCheckNo.Text = vchmst.CheckNo;
                txtCheqDate.Text = vchmst.CheqDate;
                txtCheqAmnt.Text = vchmst.CheqAmnt;
                txtMoneyRptNo.Text = vchmst.MoneyRptNo;
                txtMoneyRptDate.Text = vchmst.MoneyRptDate;
                txtControlAmt.Text = decimal.Parse(vchmst.ControlAmt).ToString("N3");
                txtStatus.Text = vchmst.Status;
                dgVoucherDtl.Visible = true;
                dgVoucher.Visible = false;
                ShowChild_Click(sender, e);
                /* if the current user's rating is equal or higher than actual rating of voucher authorization, then the voucher will be disabled for edit */
                if (vchmst.AuthoUserType != "")
                {
                    if ((vchmst.Status == "A") && (int.Parse(vchmst.AuthoUserType) >= int.Parse(Session["userlevel"].ToString())))
                    {
                        lbAuth.Visible = false;
                        VEAuth();
                    }
                }
                else
                {
                    lbAuth.Visible = true;
                    VEUnauth();
                    lbSetNew.Visible = true;
                }
                ShowFooterTotal();
                lblTranStatus.Visible = false;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('No Search Vouvher. Try Again');", true);
            }
            txtParticulars.Enabled = true;
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
    
}