using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using sales;
using Delve;
using System.Data.SqlClient;

public partial class frmCustomerPayment : System.Web.UI.Page
{
    private static Permis per;
    clsClientInfoManager _aclsClientInfoManager=new clsClientInfoManager();
    clsClientPaymentRec _aclsClientPaymentRec=new clsClientPaymentRec();
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
                    cmd.CommandText = "Select BranchId,ID,user_grp,description from utl_userinfo where upper(user_name)=upper('" + Session["user"].ToString().ToUpper() + "') and status='A'";
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
            try
            {
                var BranchId = Session["BranchId"].ToString();
                if (string.IsNullOrEmpty(BranchId))
                {
                    Response.Redirect("Default.aspx?sid=sam");
                }
                else
                {
                    ClearAll();
                }
            }
            catch
            {
                Response.Redirect("Default.aspx?sid=sam");
            }
        }
    }

    private void ClearAll()
    {
        txtSearchCustomer.Text = txtRemarks.Text = txtDueAmount.Text = txtPayAmount.Text = string.Empty;
        txtSearchCustomer.Focus();
        txtSearchCustomer.Text = "";
        hfCustomer.Value = hfCustomerCoa.Value = "";
        hfID.Value = "";
        DataTable dtPayment = _aclsClientInfoManager.GetBranchPaymentDetails("", Session["BranchId"].ToString());
        dgSVMst.DataSource = dtPayment;
        ViewState["PaymentHistory"] = dtPayment;
        dgSVMst.DataBind();
        txtPayDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        txtSearchCustomer.Focus();
    }

    protected void txtSearchCustomer_TextChanged(object sender, EventArgs e)
    {
        string Parameter = "";
        Parameter = " where UPPER([SearchName])='" + txtSearchCustomer.Text.ToUpper() + "' ";
        DataTable dtCustomer = _aclsClientInfoManager.GetCustomerOnSearch(Parameter, 0);
        if (dtCustomer != null)
        {
            if (dtCustomer.Rows.Count > 0)
            {
                hfCustomer.Value = dtCustomer.Rows[0]["ID"].ToString();
                hfCustomerCoa.Value = dtCustomer.Rows[0]["Gl_CoaCode"].ToString();
                double DueAmount = _aclsClientInfoManager.GetDueAmount(hfCustomer.Value);
                txtDueAmount.Text = DueAmount.ToString("N2");
                txtPayAmount.Focus();
            }
            else
            {
                txtSearchCustomer.Text = "";
                hfCustomer.Value = "";
                hfCustomerCoa.Value = "";
                txtSearchCustomer.Focus();
            }
        }
        else
        {
            hfCustomerCoa.Value = "";
            txtSearchCustomer.Text = "";
            hfCustomer.Value = "";
            txtSearchCustomer.Focus();
        }
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearAll();
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
        if (string.IsNullOrEmpty(txtSearchCustomer.Text))
        {
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                "alert('Search customer.!!');", true);
            txtSearchCustomer.Focus();
            return;
        }
        if (string.IsNullOrEmpty(txtPayAmount.Text.Trim()))
        {
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                "alert('Check Pay amount.!!');", true);
            txtPayAmount.Focus();
            return;
        }
        if (Convert.ToDouble(txtPayAmount.Text.Trim())<=0)
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
                _aclsClientPaymentRec.Customer_id = hfCustomer.Value;
                _aclsClientPaymentRec.Date = txtPayDate.Text;
                _aclsClientPaymentRec.PayAmt = txtPayAmount.Text;
                _aclsClientPaymentRec.Remarks = txtRemarks.Text;
                _aclsClientPaymentRec.BranchId = Session["BranchId"].ToString();
                try
                {
                    _aclsClientPaymentRec.LoginBy = Session["userID"].ToString();
                }
                catch

                {
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                   "alert('Session Out Please Login Again..!!');", true);
                    ClearAll();
                }
                _aclsClientPaymentRec.CustomerCoa = hfCustomerCoa.Value;

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
                vmstCR.Payee = "CPR";
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

                _aclsClientInfoManager.SaveBranchCustomerPayment(_aclsClientPaymentRec, vmstCR);
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                    "alert('Record are saved successfully..!!');", true);
                ClearAll();
            }

            else
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                   "alert('You are Not Permited this Scope..!!');", true);
            }
        }
        else
        {
            if(per.AllowEdit=="Y")
            {
            _aclsClientPaymentRec.ID = hfID.Value;
            _aclsClientPaymentRec.BranchId = Session["BranchId"].ToString();
            _aclsClientPaymentRec.Customer_id = hfCustomer.Value;
            _aclsClientPaymentRec.Date = txtPayDate.Text;
            _aclsClientPaymentRec.PayAmt = txtPayAmount.Text;
            _aclsClientPaymentRec.Remarks = txtRemarks.Text;
            try
            {
                _aclsClientPaymentRec.LoginBy = Session["userID"].ToString();
            }
            catch
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
               "alert('Session Out Please Login Again..!!');", true);
                ClearAll();
            }
            _aclsClientPaymentRec.CustomerCoa = hfCustomerCoa.Value;

            string VCH_SYS_NO_PVCV = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                           "t1.PAYEE='CPR' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
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
                    vmstDV.Payee = "CPR";
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

            _aclsClientInfoManager.UpdateBranchCustomerPayment(_aclsClientPaymentRec, vmstDV);
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                "alert('Record are update successfully..!!');", true);
            ClearAll();
            }

            else
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                   "alert('You are Not Permited Update this Date..!!');", true);
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
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
        if(per.AllowDelete=="Y")
        {
        if (!string.IsNullOrEmpty(hfID.Value))
        {
            _aclsClientPaymentRec.ID = hfID.Value;
            _aclsClientPaymentRec.Customer_id = hfCustomer.Value;
            _aclsClientPaymentRec.PayAmt = txtPayAmount.Text;
            _aclsClientPaymentRec.Remarks = txtRemarks.Text;
            _aclsClientPaymentRec.LoginBy = "";
            _aclsClientInfoManager.DeleteCustomerPayment(_aclsClientPaymentRec);
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                "alert('Record are delete successfully..!!');", true);
            ClearAll();
        }
        }

        else
        {
            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
               "alert('You are Not Permited this Scope..!!');", true);
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
    //protected void dgSVMst_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    hfID.Value = dgSVMst.SelectedRow.Cells[1].Text;
    //    DataTable dtPayment = _aclsClientInfoManager.GetPaymentDetails(hfID.Value);
    //    if (dtPayment.Rows.Count > 0)
    //    {
    //        txtPayDate.Text = dtPayment.Rows[0]["Date"].ToString();
    //        txtSearchCustomer.Text = dtPayment.Rows[0]["SearchName"].ToString();
    //        txtPayAmount.Text = dtPayment.Rows[0]["PayAmt"].ToString();
    //        hfCustomerCoa.Value = dtPayment.Rows[0]["Gl_CoaCode"].ToString();
    //        txtRemarks.Text = dtPayment.Rows[0]["Remarks"].ToString();
    //        hfCustomer.Value = dtPayment.Rows[0]["Customer_id"].ToString();
    //        double DueAmount = _aclsClientInfoManager.GetDueAmount(hfCustomer.Value);
    //        txtDueAmount.Text = DueAmount.ToString("N2");
    //    }
    //}
    protected void dgSVMst_SelectedIndexChanged(object sender, EventArgs e)
    {
        hfID.Value = dgSVMst.SelectedRow.Cells[1].Text;
        DataTable dtPayment = _aclsClientInfoManager.GetBranchPaymentDetails(hfID.Value, Session["BranchId"].ToString());
        if (dtPayment.Rows.Count > 0)
        {
            txtPayDate.Text = dtPayment.Rows[0]["Date"].ToString();
            txtSearchCustomer.Text = dtPayment.Rows[0]["SearchName"].ToString();
            txtPayAmount.Text = dtPayment.Rows[0]["PayAmt"].ToString();
            hfCustomerCoa.Value = dtPayment.Rows[0]["Gl_CoaCode"].ToString();
            txtRemarks.Text = dtPayment.Rows[0]["Remarks"].ToString();
            hfCustomer.Value = dtPayment.Rows[0]["Customer_id"].ToString();
            double DueAmount = _aclsClientInfoManager.GetDueAmount(hfCustomer.Value);
            txtDueAmount.Text = DueAmount.ToString("N2");
        }
    }
    protected void txtPayAmount_TextChanged(object sender, EventArgs e)
    {
        try
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
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hfID.Value))
        {
            SalesManager.UpdatePrintStatus(hfID.Value, 2);
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Please Select Item First..!!');", true);
            return;

        }
    }
}