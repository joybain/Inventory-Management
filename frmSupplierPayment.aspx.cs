using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using sales;
using System.Data;
using Delve;
using System.Data.SqlClient;

public partial class frmSupplierPayment : System.Web.UI.Page
{
    private static Permis per;
    clsClientInfoManager _aclsClientInfoManager = new clsClientInfoManager();
    clsSupplierPaymentRec _aclsClientPaymentRec = new clsSupplierPaymentRec();
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
                    cmd.CommandText = "Select ID,user_grp,description from utl_userinfo where upper(user_name)=upper('" + Session["user"].ToString().ToUpper() + "') and status='A'";
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
                        cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type from gl_set_of_books where book_name='" + Session["book"].ToString() + "' ";
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
                Response.Redirect("Default.aspx?sid=sam");
            }
        }
        catch
        {
            Response.Redirect("Default.aspx?sid=sam");
        } 
        if (!IsPostBack)
        {
            ClearAll();
        }
    }

    private void ClearAll()
    {

        txtSupplierSearch.Text = txtRemarks.Text = txtDueAmount.Text = txtPayAmount.Text = string.Empty;
        txtSupplierSearch.Focus();
        txtSupplierSearch.Text = "";
        hfSupplier.Value = hfSupplierCoa.Value = "";
        hfID.Value = "";
        DataTable dtPayment = _aclsClientInfoManager.GetPaymentDetailsSupplier("");
        dgSVMst.DataSource = dtPayment;
        ViewState["PaymentHistory"] = dtPayment;
        dgSVMst.DataBind();
        txtPayDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        txtSupplierSearch.Focus();
    }

    protected void txtSearchCustomer_TextChanged(object sender, EventArgs e)
    {
        string Parameter = "";
        Parameter = " where UPPER([SearchName])='" + txtSupplierSearch.Text.ToUpper() + "' ";
        DataTable dtSupplier = PurchaseVoucherManager.GetSupplierInfo(txtSupplierSearch.Text);
        if (dtSupplier != null)
        {
            if (dtSupplier.Rows.Count > 0)
            {

                hfSupplierID.Value = dtSupplier.Rows[0]["ID"].ToString();
               txtSupplierSearch.Text= hfSupplier.Value = dtSupplier.Rows[0]["SupplierSearch"].ToString();
                hfSupplierCoa.Value = dtSupplier.Rows[0]["Gl_CoaCode"].ToString();
                double DueAmount = _aclsClientInfoManager.GetDueAmountSupplier(hfSupplierID.Value);
                txtDueAmount.Text = DueAmount.ToString("N2");
                txtPayAmount.Focus();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Not Found Supplier.!!');", true);
                txtSupplierSearch.Text = "";
                hfSupplier.Value = "";
                hfSupplierCoa.Value = "";
                txtSupplierSearch.Focus();
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Not Found Supplier.!!');", true);
            txtSupplierSearch.Text = "";
            hfSupplier.Value = "";
            hfSupplierCoa.Value = "";
            txtSupplierSearch.Focus();
        }


    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearAll();
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtSupplierSearch.Text))
        {
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                "alert('Search customer.!!');", true);
            txtSupplierSearch.Focus();
            return;
        }
        if (string.IsNullOrEmpty(txtPayAmount.Text.Trim()))
        {
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                "alert('Check Pay amount.!!');", true);
            txtPayAmount.Focus();
            return;
        }
        if (Convert.ToDouble(txtPayAmount.Text.Trim()) <= 0)
        {
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                "alert('Check Pay amount.!!');", true);
            txtPayAmount.Focus();
            return;
        }
        if (string.IsNullOrEmpty(hfID.Value))
        {
            if (per.AllowAdd == "Y")
            {
                _aclsClientPaymentRec.Supplier_id = hfSupplierID.Value;
                _aclsClientPaymentRec.SupplierCoa = hfSupplierCoa.Value;
                _aclsClientPaymentRec.Date = txtPayDate.Text;
                _aclsClientPaymentRec.PayAmt = txtPayAmount.Text;
                _aclsClientPaymentRec.Remarks = txtRemarks.Text;
                _aclsClientPaymentRec.LoginBy = "";
                _aclsClientPaymentRec.SupplierCoa = hfSupplierCoa.Value;

                //********* Credit Voucher *********//
                VouchMst vmstCR = new VouchMst();
                vmstCR.FinMon = FinYearManager.getFinMonthByDate(txtPayDate.Text);
                vmstCR.ValueDate = txtPayDate.Text;
                vmstCR.VchCode = "01";
                vmstCR.RefFileNo = "";
                vmstCR.VolumeNo = "";
                vmstCR.SerialNo = "";
                vmstCR.Particulars = txtRemarks.Text;
                vmstCR.ControlAmt = txtPayAmount.Text.Replace(",", "");
                vmstCR.Payee = "SPR";
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
                vmstCR.VchRefNo = "DV-" + vmstCR.VchSysNo.ToString().PadLeft(10, '0');

                _aclsClientInfoManager.SaveSupplierPayment(_aclsClientPaymentRec, vmstCR);
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                    "alert('Record are saved successfully..!!');", true);
                ClearAll();
            }

            else
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                    "alert('You are Not Permited to Add This Data..!!');", true);
              
            }
        }
        else
        {
            if (per.AllowEdit == "Y")
            {
                _aclsClientPaymentRec.ID = hfID.Value;
                _aclsClientPaymentRec.Supplier_id = hfSupplierID.Value;
                _aclsClientPaymentRec.SupplierCoa = hfSupplierCoa.Value;
                _aclsClientPaymentRec.Date = txtPayDate.Text;
                _aclsClientPaymentRec.PayAmt = txtPayAmount.Text;
                _aclsClientPaymentRec.Remarks = txtRemarks.Text;
                _aclsClientPaymentRec.LoginBy = "";

                string VCH_SYS_NO_PVCV = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                               "t1.PAYEE='SPR' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                               hfID.Value);
                VouchMst vmstDV = VouchManager.GetVouchMst(VCH_SYS_NO_PVCV.Trim());
                if (vmstDV != null)
                {
                    vmstDV.FinMon = FinYearManager.getFinMonthByDate(txtPayDate.Text);
                    vmstDV.ValueDate = txtPayDate.Text;
                    vmstDV.RefFileNo = "";
                    vmstDV.VchCode = "01";
                    //vmst.SerialNo = txtGRNO.Text.Trim();
                    vmstDV.Particulars = txtRemarks.Text;
                    vmstDV.ControlAmt = txtPayAmount.Text.Replace(",", "");
                    //vmst.Payee = "PV";
                    vmstDV.CheqAmnt = "0";
                    vmstDV.UpdateUser = Session["user"].ToString().ToUpper();
                    vmstDV.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                    vmstDV.AuthoUserType = Session["userlevel"].ToString();
                }
                else
                {
                    if (Convert.ToDecimal(txtPayAmount.Text) > 0)
                    {
                        vmstDV = new VouchMst();
                        vmstDV.FinMon = FinYearManager.getFinMonthByDate(txtPayDate.Text);
                        vmstDV.ValueDate = txtPayDate.Text;
                        vmstDV.VchCode = "01";
                        vmstDV.RefFileNo = "New";
                        vmstDV.VolumeNo = "";
                        vmstDV.SerialNo = hfID.Value.Trim();
                        vmstDV.Particulars = txtRemarks.Text;
                        vmstDV.ControlAmt = txtPayAmount.Text.Replace(",", "");
                        vmstDV.Payee = "SPR";
                        vmstDV.CheckNo = "";
                        vmstDV.CheqDate = txtPayDate.Text;
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
                _aclsClientInfoManager.UpdateSupplierPayment(_aclsClientPaymentRec, vmstDV);
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                    "alert('Record are update successfully..!!');", true);
                ClearAll();
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                    "alert('You are Not Permited To Update This Data..!!');", true);
              
            }
        }
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hfID.Value))
        {
            _aclsClientPaymentRec.ID = hfID.Value;
            _aclsClientPaymentRec.Supplier_id = hfSupplierID.Value;
            _aclsClientPaymentRec.PayAmt = txtPayAmount.Text;
            _aclsClientPaymentRec.Remarks = txtRemarks.Text;
            _aclsClientPaymentRec.LoginBy = "";
            _aclsClientInfoManager.DeleteSupplierPayment(_aclsClientPaymentRec);
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                "alert('Record are delete successfully..!!');", true);
            ClearAll();
        }
    }

    protected void dgSVMst_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgSVMst.DataSource = ViewState["PaymentHistory"];
        dgSVMst.PageIndex = e.NewPageIndex;
        dgSVMst.DataBind();
    }
    protected void dgSVMst_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header |
            e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");
        }
    }
    protected void dgSVMst_SelectedIndexChanged(object sender, EventArgs e)
    {
        hfID.Value = dgSVMst.SelectedRow.Cells[1].Text;
        DataTable dtPayment = _aclsClientInfoManager.GetPaymentDetailsSupplier(hfID.Value);
        if (dtPayment.Rows.Count > 0)
        {
            txtPayDate.Text = dtPayment.Rows[0]["Date"].ToString();
            txtSupplierSearch.Text = dtPayment.Rows[0]["SearchName"].ToString();
            txtPayAmount.Text = dtPayment.Rows[0]["PayAmt"].ToString();
            txtRemarks.Text = dtPayment.Rows[0]["Remarks"].ToString();
            hfSupplierCoa.Value = dtPayment.Rows[0]["Gl_CoaCode"].ToString();
            hfSupplierID.Value = dtPayment.Rows[0]["Supplier_id"].ToString();
            double DueAmount = _aclsClientInfoManager.GetDueAmountSupplier(hfSupplierID.Value);
            txtDueAmount.Text = DueAmount.ToString("N2");
        }
    }
    protected void txtPayAmount_TextChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(hfID.Value))
        {
            if (string.IsNullOrEmpty(txtDueAmount.Text))
            {
                txtDueAmount.Text = "0";
            }
            if (Convert.ToDouble(txtDueAmount.Text) < Convert.ToDouble(txtPayAmount.Text))
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                    "alert('pay amount not upper then due amount ..!!');", true);
                txtPayAmount.Text = "";
                txtPayAmount.Focus();
                return;
            }
        }
    }
    protected void txtSupplierSearch_TextChanged(object sender, EventArgs e)
    {
        string Parameter = "";
        Parameter = " where UPPER([SearchName])='" + txtSupplierSearch.Text.ToUpper() + "' ";
        DataTable dtSupplier = PurchaseVoucherManager.GetSupplierInfo(txtSupplierSearch.Text);
        if (dtSupplier != null)
        {
            if (dtSupplier.Rows.Count > 0)
            {

                hfSupplierID.Value = dtSupplier.Rows[0]["ID"].ToString();
                txtSupplierSearch.Text = hfSupplier.Value = dtSupplier.Rows[0]["SupplierSearch"].ToString();
                hfSupplierCoa.Value = dtSupplier.Rows[0]["Gl_CoaCode"].ToString();
                double DueAmount = _aclsClientInfoManager.GetDueAmountSupplier(hfSupplierID.Value);
                txtDueAmount.Text = DueAmount.ToString("N2");
                txtPayAmount.Focus();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Not Found Supplier.!!');", true);
                txtSupplierSearch.Text = "";
                hfSupplier.Value = "";
                hfSupplierCoa.Value = "";
                txtSupplierSearch.Focus();
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Not Found Supplier.!!');", true);
            txtSupplierSearch.Text = "";
            hfSupplier.Value = "";
            hfSupplierCoa.Value = "";
            txtSupplierSearch.Focus();
        }
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hfID.Value))
        {
            SalesManager.UpdatePrintStatus(hfID.Value, 3);
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Please Select Item First..!!');", true);
            return;
          
        }
    }
}