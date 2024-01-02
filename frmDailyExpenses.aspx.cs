using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Delve;

public partial class frmDailyExpenses : System.Web.UI.Page
{
    private static Permis per;
    ExpenseHeadManager _aeExpenseHeadManager=new ExpenseHeadManager();
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
                    UP1.Update();
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
        getEmptyDtl();
        txtCode.Text = "Exp-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() +
                       DateTime.Now.Day.ToString() + "-00" +
                       IdManager.GetShowSingleValueString("ExpensesID", "FixValue");
        txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        txtRemarks.Text =txtTotal.Text= string.Empty;
        txtRemarks.Text = "Daily Expenses for ";
        DataTable dtExpensesDetais = _aeExpenseHeadManager.GetShowwAllExpenses(Session["BranchId"].ToString());
        dgHistory.DataSource = dtExpensesDetais;
        ViewState["History"] = dtExpensesDetais;
        dgHistory.DataBind();
        hfID.Value = string.Empty;
        tabVch.ActiveTabIndex = 0;
    }

    private void getEmptyDtl()
    {
        dgPVDetailsDtl.Visible = true;
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("ID", typeof(string));
        dtDtlGrid.Columns.Add("GL_COA_CODE", typeof(string));
        dtDtlGrid.Columns.Add("ExpensesHead", typeof(string));
        dtDtlGrid.Columns.Add("Amount", typeof(string));
        dtDtlGrid.Columns.Add("dtl_ID", typeof(string));
        DataRow dr = dtDtlGrid.NewRow();
        dtDtlGrid.Rows.Add(dr);
        dgPVDetailsDtl.DataSource = dtDtlGrid;
        ViewState["purdtl"] = dtDtlGrid;
        dgPVDetailsDtl.DataBind();
    }
    protected void txtItemDesc_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        DataTable dtdtl = (DataTable)ViewState["purdtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        DataTable dtExpHear = _aeExpenseHeadManager.GetShowExpenses(((TextBox)gvr.FindControl("txtItemDesc")).Text,"1");
        if (dtExpHear.Rows.Count > 0)
        {
           // DataTable dt = ItemManager.GetItems(dtSerial.Rows[0]["ItemMstID"].ToString(), 5);
            bool IsCheck = false;
            foreach (DataRow ddr in dtdtl.Rows)
            {
                if (string.IsNullOrEmpty(dr["ExpensesHead"].ToString()))
                {
                    if (ddr["ID"].ToString().Equals(((DataRow)dtExpHear.Rows[0])["ID"].ToString()))
                    {
                        IsCheck = true;
                        break;
                    }
                }
            }

            if (IsCheck == true)
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                    "alert('This Exp Head already added...!!!');", true);
                ((TextBox)gvr.FindControl("txtItemDesc")).Text = "";
                ((TextBox)gvr.FindControl("txtItemDesc")).Focus();
                return;
            }

            if (dtExpHear.Rows.Count > 0)
            {
                if (string.IsNullOrEmpty(((TextBox)gvr.FindControl("txtItemDesc")).Text))
                {
                    dtdtl.Rows.Remove(dr);
                    dr = dtdtl.NewRow();
                    dr["ID"] = ((DataRow)dtExpHear.Rows[0])["ID"].ToString();
                    dr["ExpensesHead"] = ((DataRow)dtExpHear.Rows[0])["Name"].ToString();
                    dr["GL_COA_CODE"] = ((DataRow)dtExpHear.Rows[0])["GL_COA_CODE"].ToString();
                    dr["Amount"] = "0";
                    dtdtl.Rows.InsertAt(dr, gvr.DataItemIndex);
                }
                else
                {
                    DataRow drAdd = dtdtl.Rows[gvr.DataItemIndex];
                    drAdd["ID"] = ((DataRow)dtExpHear.Rows[0])["ID"].ToString();
                    drAdd["ExpensesHead"] = ((DataRow)dtExpHear.Rows[0])["Name"].ToString();
                    drAdd["GL_COA_CODE"] = ((DataRow)dtExpHear.Rows[0])["GL_COA_CODE"].ToString();
                    drAdd["Amount"] = "0";
                    string found = "";
                    foreach (DataRow drd in dtdtl.Rows)
                    {
                        if (drd["ID"].ToString() == "" && drd["ExpensesHead"].ToString() == "")
                        {
                            found = "Y";
                        }
                    }
                    if (found == "")
                    {
                        DataRow drd = dtdtl.NewRow();
                        dtdtl.Rows.Add(drd);
                    }

                }
            }
            dgPVDetailsDtl.DataSource = dtdtl;
            dgPVDetailsDtl.DataBind();
            ShowFooterTotal();
            // ((TextBox)gvr.FindControl("txtItemRate")).Focus();
            ((TextBox)dgPVDetailsDtl.Rows[dgPVDetailsDtl.Rows.Count - 2].FindControl("txtItemRate")).Focus();
        }
    }
    protected void txtItemRate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
            DataTable dt = (DataTable)ViewState["purdtl"];
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[gvr.DataItemIndex];
                dr["ID"] = dr["ID"].ToString();
                dr["ExpensesHead"] = dr["ExpensesHead"].ToString();
                dr["GL_COA_CODE"] = dr["GL_COA_CODE"].ToString();
                dr["Amount"] = ((TextBox)gvr.FindControl("txtItemRate")).Text;
            }
            string found = "";
            foreach (DataRow drd in dt.Rows)
            {
                if (drd["ID"].ToString() == "" && drd["ExpensesHead"].ToString() == "")
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
            ((TextBox)dgPVDetailsDtl.Rows[dgPVDetailsDtl.Rows.Count - 1].FindControl("txtItemDesc")).Focus();
            //PVIesms_UP.Update();
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
    private void ShowFooterTotal()
    {
        decimal Total = 0;

        if (ViewState["purdtl"] != null)
        {
            DataTable dt = (DataTable)ViewState["purdtl"];
            foreach (DataRow drp in dt.Rows)
            {
                if (drp["ExpensesHead"].ToString() != "")
                {
                    Total += decimal.Parse(drp["Amount"].ToString());
                }
            }
        }

        txtTotal.Text = Total.ToString("N2");
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
        TableCell cell;
        cell = new TableCell();
        cell.Text = "Total";
        cell.ColumnSpan = 2;
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        cell = new TableCell();
        cell.Text = Total.ToString("N0");
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        row.Font.Bold = true;
        row.BackColor = System.Drawing.Color.LightGray;
        if (dgPVDetailsDtl.Rows.Count > 0)
        {
            dgPVDetailsDtl.Controls[0].Controls.Add(row);
        }
       UP1.Update();
    }

    protected void dgPurDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
                e.Row.Cells[2].Attributes.Add("style", "display:none");
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
    protected void dgPVDetailsDtl_RowCommand(object sender, GridViewCommandEventArgs e)
    {

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
                    if (drf["ID"].ToString() == "" && drf["ExpensesHead"].ToString() == "")
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
    protected void btnSave_Click(object sender, EventArgs e)
    {
        DataTable dtExp = (DataTable) ViewState["purdtl"];
        if (dtExp.Rows.Count <= 1)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please set expenses list..!!');", true);
            return;
        }
        ExpensesInfo _aExpensesInfo=new ExpensesInfo();
        if (string.IsNullOrEmpty(hfID.Value))
        {
            //_aExpensesInfo.ID = hfID.Value;
            _aExpensesInfo.Code = txtCode.Text;
            _aExpensesInfo.Date = txtDate.Text;
            _aExpensesInfo.Remarks = txtRemarks.Text.Replace("'", "");
            _aExpensesInfo.LoginBy = "";


            VouchMst vmstCR = new VouchMst();
            vmstCR.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
            vmstCR.ValueDate = txtDate.Text;
            vmstCR.VchCode = "01";
            vmstCR.RefFileNo = "";
            vmstCR.VolumeNo = "";
            vmstCR.SerialNo = txtCode.Text.Trim();
            vmstCR.Particulars = txtRemarks.Text.Replace("'", "");
            vmstCR.ControlAmt = txtTotal.Text.Replace(",", "");
            vmstCR.Payee = "Exp_DV";
            vmstCR.CheckNo ="";
            vmstCR.CheqDate = txtDate.Text;
            vmstCR.CheqAmnt = "0";
            vmstCR.MoneyRptNo = "";
            vmstCR.MoneyRptDate = "";
            vmstCR.TransType = "R";
            vmstCR.BookName = "AMB";
            vmstCR.EntryUser = Session["user"].ToString();
            vmstCR.EntryDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
            vmstCR.Status = "A";
            vmstCR.AuthoUserType = Session["userlevel"].ToString();
            vmstCR.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no")
                .ToString();
            vmstCR.VchRefNo = "DV-" + vmstCR.VchSysNo.ToString().PadLeft(10, '0');

            
            _aeExpenseHeadManager.SaveDailyExpenses(_aExpensesInfo, dtExp, vmstCR,Session["BranchId"].ToString());

            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is/are saved successfully!!');", true);
            RefreshAll();
        }
        else
        {
            _aExpensesInfo.ID = hfID.Value;
            _aExpensesInfo.Code = txtCode.Text;
            _aExpensesInfo.Date = txtDate.Text;
            _aExpensesInfo.Remarks = txtRemarks.Text.Replace("'", "");
            _aExpensesInfo.LoginBy = "";

            string VCH_SYS_NO_PVCV = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                "t1.PAYEE='Exp_DV' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                txtCode.Text);
            VouchMst vmstCV = VouchManager.GetVouchMst(VCH_SYS_NO_PVCV.Trim());
            if (vmstCV != null)
            {
                vmstCV.FinMon = FinYearManager.getFinMonthByDate(txtDate.Text);
                vmstCV.ValueDate = txtDate.Text;
                vmstCV.RefFileNo = "";
                vmstCV.VchCode = "01";
                //vmst.SerialNo = txtGRNO.Text.Trim();
                vmstCV.Particulars = txtRemarks.Text.Replace("'","");
                vmstCV.ControlAmt = txtTotal.Text.Replace(",", "");
                //vmst.Payee = "PV";
                vmstCV.CheqAmnt = "0";
                vmstCV.UpdateUser = Session["user"].ToString().ToUpper();
                vmstCV.UpdateDate = System.DateTime.Now.ToString("dd/MM/yyyy");
                vmstCV.AuthoUserType = Session["userlevel"].ToString();
            }

           
            _aeExpenseHeadManager.UpdateDailyExpenses(_aExpensesInfo, dtExp, vmstCV,Session["BranchId"].ToString());

            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record are update successfully!!');", true);
            RefreshAll();
        }
    }
    protected void CloseButton_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void DeleteButton_Click(object sender, EventArgs e)
    {
        DataTable dtExp = _aeExpenseHeadManager.getDailyExpences(hfID.Value, "1", Session["BranchId"].ToString());
        if (dtExp.Rows.Count > 0)
        {
            string VCH_SYS_NO_PVCV = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                "t1.PAYEE='Exp_DV' and SUBSTRING(t1.VCH_REF_NO,1,2)='DV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                txtCode.Text);
            VouchMst vmstCV = VouchManager.GetVouchMst(VCH_SYS_NO_PVCV.Trim());
            _aeExpenseHeadManager.DeleteExpensesDetails(hfID.Value, vmstCV, Session["BranchId"].ToString());
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record are delete successfully!!');", true);
            RefreshAll();
        }

    }



    protected void dgHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
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
    protected void dgHistory_SelectedIndexChanged(object sender, EventArgs e)
    {
        hfID.Value = dgHistory.SelectedRow.Cells[1].Text;
        DataTable dtExp = _aeExpenseHeadManager.getDailyExpences(hfID.Value, "1", Session["BranchId"].ToString());
        if (dtExp.Rows.Count > 0)
        {
            txtCode.Text = dtExp.Rows[0]["Code"].ToString();
            txtDate.Text = dtExp.Rows[0]["ExpDate"].ToString();
            txtRemarks.Text = dtExp.Rows[0]["Remarks"].ToString();
            DataTable dtExpDtl = _aeExpenseHeadManager.getDailyExpences(hfID.Value, "2", Session["BranchId"].ToString());
            if (dtExpDtl.Rows.Count > 0)
            {
                dgPVDetailsDtl.DataSource = dtExpDtl;
                ViewState["purdtl"] = dtExpDtl;
                dgPVDetailsDtl.DataBind();
                ShowFooterTotal();
            }

            tabVch.ActiveTabIndex = 0;
        }
    }
    protected void dgHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgHistory.DataSource = ViewState["History"];
        dgHistory.PageIndex = e.NewPageIndex;
        dgHistory.DataBind();
    }
}