using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Office2010.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.draw;
using ClosedXML.Excel;
using System.IO;

using System.Net;
using System.Globalization;
using Microsoft.Reporting.WebForms;
using Delve;

public partial class ItemTransfferStock : System.Web.UI.Page
{
    clsItemTransferStockManager aclsItemTransferStockManager = new clsItemTransferStockManager();
    ItemManager _aItemManager=new ItemManager();
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
                    cmd.CommandText = "Select ID,user_grp,description from utl_userinfo where upper(user_name)=upper('" + Session["user"].ToString().ToUpper() + "') and status='A'";
                    conn.Open();
                    dReader = cmd.ExecuteReader();
                    string wnot = "";
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            Session["userlevel"] = int.Parse(dReader["user_grp"].ToString());
                            //Session["dept"] = dReader["dept"].ToString();
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
                                //Session["ShotName"] = dReader["ShortName"].ToString();
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
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('" + Session.SessionID + "');", true);
            string pageName = DataManager.GetCurrentPageName();
            string modid = PermisManager.getModuleId(pageName);
            per = PermisManager.getUsrPermis(Session["user"].ToString().Trim().ToUpper(), modid);
            if (per != null & per.AllowView == "Y")
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
            Response.Redirect("Home.aspx?sid=sam");

        }
        if (!IsPostBack)
        {

          

            Session["purdtl"] = null;
            BtnSave.Enabled = true;
            DataTable dt = aclsItemTransferStockManager.GetBranchInfo("");
            if (dt.Rows.Count > 0)
            {
                dgHistory.DataSource = dt;
                ViewState["History"] = dt;
                dgHistory.DataBind();
                
            }

            DataTable dtBranch = ClsItemDetailsManager.GetBranchInfo();
            util.PopulationDropDownList(ddlBranch, "Name", "ID", dtBranch);
           
            //TabContainer1.ActiveTabIndex = 2;
            txtRemark.Text = lblID.Text = "";
            BtnSave.Enabled = true;
            Refresh();
            DropDownListValue();
            //RefreshReject();
            //getEmptyDtlReject();
            txtTfDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            Up1.Update();
        }
    }
    private void Refresh()
    {

        txtRemark.Text = lblID.Text = "";
        lblPrintFlag.Text = "Top(1)";
        txtSearchCarton.Text = "";
        txtTfDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        ItemsDetails.Visible = pnl.Enabled = true;
        ddlBranchSearch.SelectedIndex = -1;
        txtRemark.Text = lblID.Text = "";
        dgPODetailsDtl.DataSource = null;
        dgPODetailsDtl.DataBind();
        dgPODetailsDtl.Enabled = txtSearchCarton.Enabled=true;
        //dgTransferHistory.Visible = false;
        //pnlItemDtl.Visible = true;
        Session["purdtl"] = null;
        getEmptyDtl();
        BtnSave.Enabled = true;
        txtChallanNo.Text = "";
        // btnNew.Visible = false;
        txtChallanNo.Text = IdManager.GetDateTimeWiseSerial("CF", "ID", "dbo.ItemStockTransferMst");
        txtTransferCode.Text = "TF-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +
                                   DateTime.Now.ToString("dd") + "00" +
                                   IdManager.GetShowSingleValueInt("TransferID", "ID", "FixValue", "1").ToString();
        DataTable dtBranch = ClsItemDetailsManager.GetBranchInfo();
        util.PopulationDropDownList(ddlBranch, "Name", "ID", dtBranch);


    }

    //private void RefreshReject()
    //{

    //    txtRemarkReject.Text = lblIDReject.Text = "";
    //    lblPrintFlagReject.Text = "Top(1)";
    //    txtTfDateReject.Text = DateTime.Now.ToString("dd/MM/yyyy");
    //    //ItemsDetailsReject.Visible = pnl.Enabled = true;
    //    ddlBranchReject.SelectedIndex = -1;
    //    txtRemarkReject.Text = lblIDReject.Text = "";
    //    dgPODetailsDtlReject.DataSource = null;
    //    dgPODetailsDtlReject.DataBind();
    //    dgPODetailsDtlReject.Enabled = true;
    //    btnSaveReject.Enabled = true;
    //    //dgTransferHistory.Visible = false;
    //    //pnlItemDtlReject.Visible = true;
    //    Session["purdtlReject"] = null;
    //    getEmptyDtlReject();
    //    // btnNew.Visible = false;
    //    txtChallanNoReject.Text = IdManager.GetDateTimeWiseSerial("CR", "ID", "dbo.ItemStockTransferMst");
    //    txtTransferCodeReject.Text = "TR-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +
    //                               DateTime.Now.ToString("dd") + "00" +
    //                               IdManager.GetShowSingleValueInt("TransferID", "ID", "FixValue", "1").ToString();


    //}
    
    protected void btnNew_Click(object sender, EventArgs e)
    {
       

    }
    private void DropDownListValue()
    {
        DataTable dt = IdManager.GetShowDataTable("select ID,BranchName from BranchInfo where Flag IS NULL and MainBranch!=1 or MainBranch is null and DeleteBy is null and [Status]=1");
        ddlBranchSearch.DataSource = dt;
        ddlBranchSearch.DataTextField = "BranchName";
        ddlBranchSearch.DataValueField = "ID";
        ddlBranchSearch.DataBind();
        ddlBranchSearch.Items.Insert(0, "");


        //ddlBranchReject.DataSource = dt;
        //ddlBranchReject.DataTextField = "BranchName";
        //ddlBranchReject.DataValueField = "ID";
        //ddlBranchReject.DataBind();
        //ddlBranchReject.Items.Insert(0, "");

    }
    private void getEmptyDtl()
    {
        dgPODetailsDtl.Visible = true;
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("ItemsID", typeof(string));
        dtDtlGrid.Columns.Add("ItemsName", typeof(string));
        dtDtlGrid.Columns.Add("item_code", typeof(string));
        dtDtlGrid.Columns.Add("Code", typeof(string));
        dtDtlGrid.Columns.Add("Barcode", typeof(string));
        dtDtlGrid.Columns.Add("item_desc", typeof(string));
        dtDtlGrid.Columns.Add("StockQty", typeof(string));
        dtDtlGrid.Columns.Add("TransferQty", typeof(string));
        dtDtlGrid.Columns.Add("Price", typeof(string));
        dtDtlGrid.Columns.Add("Catagory", typeof(string));
        dtDtlGrid.Columns.Add("SubCatagory", typeof(string));
        dtDtlGrid.Columns.Add("UMO", typeof(string));
        dtDtlGrid.Columns.Add("StyleNo", typeof(string));
        dtDtlGrid.Columns.Add("Discount", typeof(string));
        dtDtlGrid.Columns.Add("CartonNo", typeof(string));
        dtDtlGrid.Columns.Add("ReceivedQuantity", typeof(string));
        dtDtlGrid.Columns.Add("BranchSalesPrice", typeof(Decimal));
        DataRow dr = dtDtlGrid.NewRow();
        dtDtlGrid.Rows.Add(dr);
        dgPODetailsDtl.DataSource = dtDtlGrid;
        Session["purdtl"] = dtDtlGrid;
       
        dgPODetailsDtl.DataBind();
    }
    //private void getEmptyDtlReject()
    //{
    //    dgPODetailsDtlReject.Visible = true;
    //    DataTable dtDtlGrid = new DataTable();
    //    dtDtlGrid.Columns.Add("ItemsID", typeof(string));
    //    dtDtlGrid.Columns.Add("ItemsName", typeof(string));
    //    dtDtlGrid.Columns.Add("item_code", typeof(string));
    //    dtDtlGrid.Columns.Add("Code", typeof(string));
    //    dtDtlGrid.Columns.Add("item_desc", typeof(string));
    //    dtDtlGrid.Columns.Add("StockQty", typeof(string));
    //    dtDtlGrid.Columns.Add("TransferQty", typeof(string));
    //    dtDtlGrid.Columns.Add("Price", typeof(string));
    //    dtDtlGrid.Columns.Add("Discount", typeof(string));
    //    dtDtlGrid.Columns.Add("Catagory", typeof(string));
    //    dtDtlGrid.Columns.Add("SubCatagory", typeof(string));
    //    dtDtlGrid.Columns.Add("UMO", typeof(string));
    //    dtDtlGrid.Columns.Add("StyleNo", typeof(string));
    //    dtDtlGrid.Columns.Add("CartonNo", typeof(string));
    //    dtDtlGrid.Columns.Add("ReceivedQuantity", typeof(string));
    //    dtDtlGrid.Columns.Add("NewItemCode", typeof(string));
      
    //    dtDtlGrid.Columns.Add("BranchSalesPrice", typeof(Decimal));
    //    DataRow dr = dtDtlGrid.NewRow();
    //    dtDtlGrid.Rows.Add(dr);
    //    dgPODetailsDtlReject.DataSource = dtDtlGrid;
        
    //    Session["purdtlReject"] = dtDtlGrid;
    //    dgPODetailsDtlReject.DataBind();
    //}

    public DataTable PopulateMeasure()
    {
        DataTable dtmsr = IdManager.GetShowDataTable("select '' ID,'' Name  union select CONVERT(NVARCHAR,ID) ,Name from ItemReceivedType order by 1");        
        return dtmsr;
    } 

    protected void BtnSave_Click(object sender, EventArgs e)
    {

        
        try
        {
            if (string.IsNullOrEmpty(ddlBranch.SelectedItem.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Select Branch.!!','teal',3);", true);


                ddlBranchSearch.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtTfDate.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Input Date.!!','teal',3);", true);
                txtTfDate.Focus();
                return;
            }          
            DataTable dt = null;
            if (Session["purdtl"] != null)
            {
                dt = (DataTable)Session["purdtl"];
                if (string.IsNullOrEmpty(dt.Rows[0]["ItemsID"].ToString()) || dt.Rows[0]["ItemsID"].ToString() == "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Add this transfer items in list.!!','teal',3);", true);
                    return;
                }
            }
   
           
            else
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Add this transfer items in list.!!','teal',3);", true);
                return;
            }
            
            clsItemTransferStock aclsItemTransferStock = aclsItemTransferStockManager.GetStockTransferInfo(lblID.Text);
            if (aclsItemTransferStock != null)
            {
                if (per.AllowEdit == "Y")
                {
                    int CheckReceived = IdManager.GetShowSingleValueInt("COUNT(*)", "ReceivedBy IS NOT NULL AND ID", "ItemStockTransferMst", lblID.Text);
                    string CheckExcelFlag = IdManager.GetShowSingleValueString("ExcelUser", "ID", "ItemStockTransferMst", lblID.Text);
                    if (!string.IsNullOrEmpty(CheckExcelFlag))
                    {
                        
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Alrady Generate Excel File.you are not update this.!!','red',2);", true);
                        
                        return;
                    }
                    if (CheckReceived > 0)
                    {
                      
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('this transfer items alrady received .\\n you never edit this transfer items.\\n are  you want please contract to administratio.!!','red',2);", true);
                        return;
                    }
                    aclsItemTransferStock.Code = txtTransferCode.Text.Replace("'", "").Replace(",", "").Trim();
                    aclsItemTransferStock.TransferDate = txtTfDate.Text.Replace("'", "").Replace(",", "").Trim();
                    aclsItemTransferStock.BranchId = ddlBranch.SelectedValue.Replace("'", "").Replace(",", "").Trim();
                    aclsItemTransferStock.LoginBy = Session["userID"].ToString().Replace("'", "").Replace(",", "").Trim();
                    aclsItemTransferStock.CartonNo = txtSearchCarton.Text.Replace("'", "").Replace(",", "").Trim();
                    aclsItemTransferStock.ChallanNo = txtChallanNo.Text.Replace("'", "").Replace(",", "").Trim();
                    aclsItemTransferStock.TransferType = "F";
                    aclsItemTransferStock.Remark = txtRemark.Text.Replace("'", "").Replace(",", "").Trim();
                    string Parameter = "where [MstID]='" + lblID.Text + "' AND t1.DeleteBy IS NULL";
                    DataTable dtOld = aclsItemTransferStockManager.GetShowItemsDetails(Parameter,"F");
                    string ITSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                       "t1.PAYEE='IT' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                       txtTransferCode.Text);
                    //*************************** Account Entry (Update) ******************//
                    //********* Jurnal Voucher - 1 *********//
                    VouchMst vmst = VouchManager.GetVouchMst(ITSerial.Trim());                    
                    if (vmst != null)
                    {
                        vmst.FinMon = FinYearManager.getFinMonthByDate(txtTfDate.Text);
                        vmst.ValueDate = txtTfDate.Text;
                        vmst.VchCode = "03";
                        // vmst.RefFileNo = "";
                        // vmst.VolumeNo = "";
                        //vmst.SerialNo = lblID.Text;
                        vmst.Particulars = txtRemark.Text.Replace("'", "").Replace(",", "").Trim();
                        //vmst.ControlAmt = Convert.ToDouble(txtTotal.Text).ToString().Replace(",", "");
                        vmst.UpdateUser = Session["user"].ToString();
                        vmst.UpdateDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
                        vmst.AuthoUserType = Session["userlevel"].ToString();
                    }

                    aclsItemTransferStockManager.UpdateBranchInfo(aclsItemTransferStock, dt, dtOld, vmst);
                    Refresh();
                    
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Record(s) is/are updated suceessfullly!!','green',1);", true);
                        
                    BtnSave.Enabled = false;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('You have not enough permissoin to update this record!!','red',0);", true);
                    
                }
            }
            else
            {
                if (per.AllowAdd == "Y")
                {
                    aclsItemTransferStock = new clsItemTransferStock();
                    aclsItemTransferStock.TransferDate = txtTfDate.Text.Replace("'", "").Replace(",", "").Trim();
                    aclsItemTransferStock.BranchId = ddlBranch.SelectedValue.Replace("'", "").Replace(",", "").Trim();
                    try
                    {
                        var a = Session["userID"].ToString().Replace("'", "").Replace(",", "").Trim();

                    }
                    catch 
                    {

                        Response.Redirect("Home.aspx?sid=sam");
                    }
               
                    aclsItemTransferStock.LoginBy = Session["userID"].ToString().Replace("'", "").Replace(",", "").Trim();
                    if (string.IsNullOrEmpty(txtRemark.Text))
                    {
                        txtRemark.Text = "Transfer To " + ddlBranchSearch.SelectedItem.Text.Replace("'", "").Replace(",", "").Trim() +
                                         " From Head Office. Transfer By : " + Session["user"].ToString() +
                                         " . Transfer Date :" + DateTime.Now.ToString("dd-MMM-yyyy") + ".";
                    }
                    aclsItemTransferStock.Remark = txtRemark.Text.Replace("'", "").Replace(",", "").Trim();
                    aclsItemTransferStock.ChallanNo = txtChallanNo.Text.Replace("'", "").Replace(",", "").Trim();
                    aclsItemTransferStock.TransferType = "F";
                    txtTransferCode.Text = "TF-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +
                                   DateTime.Now.ToString("dd") + "00" +
                                   IdManager.GetShowSingleValueInt("TransferID", "ID", "FixValue", "1").ToString();
                    aclsItemTransferStock.Code = txtTransferCode.Text.Replace("'", "").Replace(",", "").Trim();
                    aclsItemTransferStock.CartonNo = txtSearchCarton.Text.Replace("'", "").Replace(",", "").Trim();

                    VouchMst vmst = new VouchMst();
                    vmst.FinMon = FinYearManager.getFinMonthByDate(txtTfDate.Text.Replace("'", "").Replace(",", "").Trim());
                    vmst.ValueDate = txtTfDate.Text.Replace("'", "").Replace(",", "").Trim();
                    vmst.VchCode = "03";
                    vmst.RefFileNo = "";
                    vmst.VolumeNo = "";
                    vmst.SerialNo = txtTransferCode.Text.Replace("'", "").Replace(",", "").Trim();
                    vmst.Particulars = "Transfer To Branch " + ddlBranchSearch.SelectedItem.Text.Replace("'", "").Replace(",", "").Trim();
                    vmst.Payee = "IT";
                    //vmst.CheckNo = txtChequeNo.Text;
                    //vmst.CheqDate = txtChequeDate.Text;
                    vmst.CheqAmnt = "0";
                    vmst.MoneyRptNo = "";
                    vmst.MoneyRptDate = "";
                    vmst.TransType = "T";
                    vmst.BookName = "AMB";
                    vmst.EntryUser = Session["user"].ToString();
                    vmst.EntryDate = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
                    vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
                    vmst.VchRefNo = "JV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
                    vmst.Status = "A";
                    vmst.AuthoUserType = Session["userlevel"].ToString();

                    VouchMst vmstPayment = new VouchMst();
                    int count =aclsItemTransferStockManager.SaveInformation(aclsItemTransferStock, dt, vmst, vmstPayment);
                    if (count ==1)
                    {

                       // ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Record(s) is/are Save suceessfullly!!','green',1);", true);
                        BtnSave.Enabled = false;
                        Refresh();
                       //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are Save suceessfullly!!','green',1);", true);

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Record(s) is/are Save suceessfullly!!','green',1);", true);
                    }

                    else
                    {
                         
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
                        
                return;
                    }

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('You have not enough permissoin to update this record!!','orange',2);", true);
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
    
    protected void BtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            clsItemTransferStock aclsItemTransferStock = aclsItemTransferStockManager.GetStockTransferInfo(lblID.Text);
            if (aclsItemTransferStock != null)
            {
                if (per.AllowDelete == "Y")
                {
                    aclsItemTransferStock.LoginBy = Session["userID"].ToString();
                    aclsItemTransferStockManager.DeleteInfo(aclsItemTransferStock);
                    Refresh();
                    
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Record(s) is/are Deleted suceessfullly!!'','green',1);", true);

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('You have not enough permissoin to update this record!!','orange',2);", true);

                   
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
    protected void BtnReset_Click(object sender, EventArgs e)
    {
        Refresh();
        TabContainer1.ActiveTabIndex = 0;
    }
  
    protected void dgSundousBranch_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");
            e.Row.Cells[7].Attributes.Add("style", "display:none");
           // e.Row.Cells[8].Attributes.Add("style", "display:none");
        }
        else if (e.Row.RowType == DataControlRowType.DataRow )
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");
            e.Row.Cells[7].Attributes.Add("style", "display:none");
            //if (e.Row.Cells[7].Text.Equals("Not Received"))
            //{
            //    e.Row.Cells[7].ForeColor = System.Drawing.Color.Red;
                
            //}
            //else
            //{
            //    e.Row.Cells[7].ForeColor = System.Drawing.Color.Green;
            //}
        }
    }
    protected void dgSundousBranch_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            clsItemTransferStock aclsItemTransferStock = aclsItemTransferStockManager.GetStockTransferInfo(dgHistory.SelectedRow.Cells[1].Text.Trim());
            if (aclsItemTransferStock != null)
            {
                if (aclsItemTransferStock.TransferType == "F")
                {
                    dgPODetailsDtl.Enabled = true;
                    txtTransferCode.Text = aclsItemTransferStock.Code;
                    lblID.Text = dgHistory.SelectedRow.Cells[1].Text.Trim();
                    txtTfDate.Text = aclsItemTransferStock.TransferDate;
                    ddlBranch.SelectedValue = aclsItemTransferStock.BranchId;
                    if (!string.IsNullOrEmpty(aclsItemTransferStock.CartonNo))
                    {
                        txtSearchCarton.Text = aclsItemTransferStock.CartonNo;
                        dgPODetailsDtl.Enabled = txtSearchCarton .Enabled= false;
                    }
                    txtChallanNo.Text = aclsItemTransferStock.ChallanNo;
                    txtRemark.Text = aclsItemTransferStock.Remark;
                    string Parameter = "where t1.[MstID]='" + lblID.Text + "' AND t1.DeleteBy IS NULL";
                    DataTable dtOld = aclsItemTransferStockManager.GetShowItemsDetails(Parameter,"F");
                    DataRow drd = dtOld.NewRow();
                    dtOld.Rows.Add(drd);
                    dgPODetailsDtl.DataSource = dtOld;
                    Session["purdtl"] = dtOld;
                    dgPODetailsDtl.DataBind();
                    ShowFooterTotal();

                    pnl.Enabled = ItemsDetails.Visible = true;
                    pnlItemDtl.Visible = true;
                    //dgPODetailsDtl.Enabled = true;
                    Up1.Update();
                    TabContainer1.ActiveTabIndex = 0;
                }

                else if (aclsItemTransferStock.TransferType == "R")
                {
                   // dgPODetailsDtlReject.Enabled = true;
                   // txtTransferCodeReject.Text = aclsItemTransferStock.Code;
                   // lblIDReject.Text = dgHistory.SelectedRow.Cells[1].Text.Trim();
                   // txtTfDateReject.Text = aclsItemTransferStock.TransferDate;
                   // ddlBranchReject.SelectedValue = aclsItemTransferStock.BranchId;
                   // if (!string.IsNullOrEmpty(aclsItemTransferStock.CartonNo))
                   // {
                   //     //txtSearchCartonReject.Text = aclsItemTransferStock.CartonNo;
                   //     dgPODetailsDtlReject.Enabled = false;
                   // }
                   // txtChallanNoReject.Text = aclsItemTransferStock.ChallanNo;
                   // txtRemarkReject.Text = aclsItemTransferStock.Remark;
                   // string Parameter = "where t1.[MstID]='" + lblIDReject.Text + "' AND t1.DeleteBy IS NULL";
                   // DataTable dtOld = aclsItemTransferStockManager.GetShowItemsDetails(Parameter,"R");
                   // DataRow drd = dtOld.NewRow();
                   // dtOld.Rows.Add(drd);
                   // dgPODetailsDtlReject.DataSource = dtOld;
                   // Session["purdtlReject"] = dtOld;
                   // dgPODetailsDtlReject.DataBind();
                   //// ShowFooterTotal();

                    
                   // //dgPODetailsDtl.Enabled = true;
                  
                   // TabContainer1.ActiveTabIndex = 1;
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
    protected void dgPurDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[2].Attributes.Add("style", "display:none");
            //e.Row.Cells[6].Attributes.Add("style", "display:none");
            //e.Row.Cells[7].Attributes.Add("style", "display:none");
            //e.Row.Cells[8].Attributes.Add("style", "display:none");
            //e.Row.Cells[9].Attributes.Add("style", "display:none");
            e.Row.Cells[9].Attributes.Add("style", "display:none");
        }
    }
    protected void dgPurDtl_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (Session["purdtl"] != null)
        {
            DataTable dtDtlGrid = (DataTable)Session["purdtl"];
            dtDtlGrid.Rows.RemoveAt(dgPODetailsDtl.Rows[e.RowIndex].DataItemIndex);
           
            if (dtDtlGrid == null)
            {
                getEmptyDtl();
            }
            dgPODetailsDtl.DataSource = dtDtlGrid;
            Session["purdtl"] = dtDtlGrid;
            dgPODetailsDtl.DataBind();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Your session is over. Try it again!!','teal',3);", true);
          
        }
    }



    protected void txtItemName_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        DataTable dtdtl = (DataTable)Session["purdtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        string CartonNo = "";
        string[] words = ((TextBox)gvr.FindControl("txtItemName")).Text.Split('-');
        DataTable dt = _aItemManager.GetSearchItemsOnStock(((TextBox)gvr.FindControl("txtItemName")).Text.ToUpper(), words.Length);


        if (dt.Rows.Count > 0)
        {
            bool IsCheck = false;
            foreach (DataRow ddr in dtdtl.Rows)
            {
                var a = dr["ItemsName"].ToString();
                if (string.IsNullOrEmpty(dr["ItemsName"].ToString()))
                {
                    //if (ddr["ItemsID"].ToString().Equals(((DataRow)dt.Rows[0])["ItemID"].ToString()))
                    if (ddr["Barcode"].ToString().Equals(((DataRow)dt.Rows[0])["Barcode"].ToString()))
                    {
                        IsCheck = true;
                        if (Convert.ToDouble(dt.Rows[0]["TotalClosingStock"].ToString()) > Convert.ToDouble(ddr["TransferQty"].ToString()))
                        {
                            ddr["TransferQty"] = Convert.ToDouble(ddr["TransferQty"].ToString()) + 1;
                            dgPODetailsDtl.DataSource = dtdtl;
                            //Session["purdtl"] = dtdtl;
                            dgPODetailsDtl.DataBind();
                            ShowFooterTotal();
                            ((TextBox)dgPODetailsDtl.Rows[gvr.DataItemIndex].FindControl("txtItemName")).Text = "";
                            ((TextBox)dgPODetailsDtl.Rows[gvr.DataItemIndex].FindControl("txtItemName")).Focus();
                            return;

                        }

                        else
                        {
                            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Transfer Stock Upper then Stock Quantity.!!');", true);
                            ((TextBox)dgPODetailsDtl.Rows[gvr.DataItemIndex].FindControl("txtItemName")).Text = "";
                            ((TextBox)dgPODetailsDtl.Rows[gvr.DataItemIndex].FindControl("txtItemName")).Focus();
                            return;

                        }
                    }
                }
            }


            dr["ItemsID"] = ((DataRow)dt.Rows[0])["ItemID"].ToString();
            dr["item_code"] = ((DataRow)dt.Rows[0])["Code"].ToString();
            dr["Barcode"] = ((DataRow)dt.Rows[0])["Barcode"].ToString();
            dr["ItemsName"] = ((DataRow)dt.Rows[0])["txtItems"].ToString();
            //dr["Type"] = ((DataRow)dt.Rows[0])["Type"].ToString();
            dr["StockQty"] = ((DataRow)dt.Rows[0])["TotalClosingStock"].ToString();
            dr["Price"] = ((DataRow)dt.Rows[0])["CostPrice"].ToString();
            //dr["TransferQty"] = "1";
            dr["TransferQty"] = Convert.ToInt32(((DataRow)dt.Rows[0])["TotalClosingStock"]).ToString();
            dr["Discount"] = "0";
            //dr["Catagory"] = ((DataRow)dt.Rows[0])["Category"].ToString();
            //dr["SubCatagory"] = ((DataRow)dt.Rows[0])["SubCategory"].ToString();
            dr["UMO"] = ((DataRow)dt.Rows[0])["UMO"].ToString();
            dr["StyleNo"] = ((DataRow)dt.Rows[0])["StyleNo"].ToString();
            dr["BranchSalesPrice"] = ((DataRow)dt.Rows[0])["SPrice"].ToString();
            dr["ReceivedQuantity"] = "0";
            //dtdtl.Rows.Add(dr);
            string found = "";
            foreach (DataRow drd in dtdtl.Rows)
            {
                if (drd["ItemsID"].ToString() == "" && drd["ItemsName"].ToString() == "")
                {
                    found = "Y";
                }
            }
            if (found == "")
            {
                DataRow drd = dtdtl.NewRow();
                dtdtl.Rows.Add(drd);
            }
            dgPODetailsDtl.DataSource = dtdtl;
            Session["purdtl"] = dtdtl;
            //Session["purdtl"] = dtdtl;
            dgPODetailsDtl.DataBind();
            ShowFooterTotal();


            ((TextBox)dgPODetailsDtl.Rows[gvr.DataItemIndex + 1].FindControl("txtItemName")).Focus();

             Up1.Update();
        }
    }




    protected void txtTransferQuantity_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        DataTable dtdtl = (DataTable)Session["purdtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        clsItemTransferStock aclsItemTransferStock = aclsItemTransferStockManager.GetStockTransferInfo(lblID.Text);
        if (!string.IsNullOrEmpty(((TextBox)gvr.FindControl("txtTransferQuantity")).Text))
        {
            if (aclsItemTransferStock != null)
            {
                if ((Convert.ToDouble(dr["StockQty"]) + Convert.ToDouble(dr["TransferQty"])) >= Convert.ToDouble(((TextBox)gvr.FindControl("txtTransferQuantity")).Text))
                {
                    dr["TransferQty"] = ((TextBox)gvr.FindControl("txtTransferQuantity")).Text;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Transfer Stock Upper then Stock Quantity.!!','orange',2);", true);
                    ((TextBox)gvr.FindControl("txtTransferQuantity")).Text = dr["TransferQty"].ToString();
                    ((TextBox)dgPODetailsDtl.Rows[gvr.DataItemIndex].FindControl("txtTransferQuantity")).Focus();
                    return;
                }
            }
            else
            {
                if (Convert.ToDouble(dr["StockQty"]) >= Convert.ToDouble(((TextBox)gvr.FindControl("txtTransferQuantity")).Text))
                {
                    dr["TransferQty"] = ((TextBox)gvr.FindControl("txtTransferQuantity")).Text;
                }
                else
                {
                   
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Transfer Stock Upper then Stock Quantity.!!','orange',2);", true);
                    ((TextBox)gvr.FindControl("txtTransferQuantity")).Text = dr["TransferQty"].ToString();
                    //((TextBox)gvr.FindControl("txtTransferQuantity")).Focus();
                    ((TextBox)dgPODetailsDtl.Rows[gvr.DataItemIndex].FindControl("txtTransferQuantity")).Focus();
                    return;
                }
            }
        }
        else
        {
            dr["TransferQty"] = '1';

          ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('This Qty  Is Not valid So 1 Qty is Fixed!!','teal',3);", true);

            //return;
        }

        dgPODetailsDtl.DataSource = dtdtl;
        Session["purdtl"] = dtdtl;
        dgPODetailsDtl.DataBind();
        ShowFooterTotal();
        if (dgPODetailsDtl.Rows.Count!=gvr.DataItemIndex+1)
        {
           //((TextBox)dgPODetailsDtl.Rows[gvr.DataItemIndex+1].FindControl("txtTransferQuantity")).Focus();
           ((TextBox)dgPODetailsDtl.Rows[gvr.DataItemIndex + 1].FindControl("txtItemName")).Focus();


        }
        Up1.Update();
    }





    protected void txtPrice_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        DataTable dtdtl = (DataTable)Session["purdtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        if (!string.IsNullOrEmpty(((TextBox)gvr.FindControl("txtTransferQuantity")).Text))
        {
            dr["BranchSalesPrice"] = ((TextBox)gvr.FindControl("txtBranchSalePrice")).Text;            
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('please input Transfer Quantity!!','teal',3);", true);
            return;
        }
        dgPODetailsDtl.DataSource = dtdtl;
        Session["purdtl"] = dtdtl;
        dgPODetailsDtl.DataBind();
        ShowFooterTotal();
        //((TextBox)dgPODetailsDtl.Rows[gvr.DataItemIndex].FindControl("txtBranchSalePrice")).Focus();
        Up1.Update();
    }
    private void ShowFooterTotal()
    {
        try
        {
            decimal ctot = decimal.Zero;
            decimal totAddi = 0;
            decimal totSalPrice = 0;
            decimal totQty = 0;
            decimal totItemsP = 0;
            decimal totA = 0;
            decimal Total = 0;

            if (Session["purdtl"] != null)
            {
                DataTable dt = (DataTable)Session["purdtl"];
                foreach (DataRow drp in dt.Rows)
                {
                    if (drp["ItemsID"].ToString() != "" && drp["Price"].ToString() != "")
                    {
                        totSalPrice += decimal.Parse(drp["BranchSalesPrice"].ToString());
                        totQty += decimal.Parse(drp["TransferQty"].ToString());
                        
                       // totItemsP += decimal.Parse(drp["item_rate"].ToString()) * decimal.Parse(drp["qnty"].ToString());
                        // totA += decimal.Parse(drp["ExpireDate"].ToString());

                        //totAddi += (totItemsP * decimal.Parse(drp["Additional"].ToString())) / 100;
                       // Total += decimal.Parse(drp["item_rate"].ToString()) * decimal.Parse(drp["qnty"].ToString()); ;
                    }
                }
            }
                GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
                TableCell cell;
                cell = new TableCell();
                cell.Text = "Total";
                cell.ColumnSpan = 3;
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);
                //cell = new TableCell();
                //cell.Text = "";
                //cell.HorizontalAlign = HorizontalAlign.Right;
                //row.Cells.Add(cell);
                cell = new TableCell();

                cell.Text = totQty.ToString("N2");
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);
                //cell = new TableCell();
                //cell.Text = totItemsP.ToString("N0");
                //cell.HorizontalAlign = HorizontalAlign.Right;
                //row.Cells.Add(cell);
                //cell = new TableCell();
                //cell.Text = totA.ToString("N2");
                //cell.HorizontalAlign = HorizontalAlign.Right;
                //row.Cells.Add(cell);
                cell = new TableCell();
                // cell.ColumnSpan = 2;
                cell.Text = totSalPrice.ToString("N0");
                cell.ColumnSpan = 1;
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);

                cell = new TableCell();
                // cell.ColumnSpan = 2;
                cell.Text = "";
                cell.HorizontalAlign = HorizontalAlign.Right;
                row.Cells.Add(cell);
                row.Font.Bold = true;
                row.BackColor = System.Drawing.Color.LightGray;
                if (dgPODetailsDtl.Rows.Count > 0)
                {
                    dgPODetailsDtl.Controls[0].Controls.Add(row);
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

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        String connectionString = DataManager.OraConnString();
        
        string Parameter = "";
        if (!string.IsNullOrEmpty(lblID.Text))
        {
            Parameter = " where ID='" + lblID.Text + "' AND DeleteBy IS NULL  ";
        }

        else
        {
            if (!string.IsNullOrEmpty(ddlBranch.SelectedItem.Text) && string.IsNullOrEmpty(txtTfDate.Text))
            {
                Parameter = " where BranchID='" + ddlBranch.SelectedValue + "' AND DeleteBy IS NULL  ";
            }
            else if (string.IsNullOrEmpty(ddlBranch.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDate.Text))
            {
                Parameter = " where convert(date,TransferDate,103)=convert(date,'" + txtTfDate.Text + "',103) AND DeleteBy IS NULL  ";
            }
            else if (!string.IsNullOrEmpty(ddlBranch.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDate.Text))
            {
                Parameter = " where BranchID='" + ddlBranch.SelectedValue + "' and convert(date,TransferDate,103)=convert(date,'" + txtTfDate.Text + "',103) AND DeleteBy IS NULL  ";
            }
        }
        DataTable dtt = IdManager.GetShowDataTable("select " + lblPrintFlag.Text + " ID,BranchID,TransferDate,ChallanNo,Code from ItemStockTransferMst " + Parameter + " order by ID desc");
        if (dtt != null)
        {
            if (dtt.Rows.Count > 0)
            {
                DataTable dt = aclsItemTransferStockManager.getShowTransferDetailsOnReport(dtt.Rows[0]["ID"].ToString(), connectionString);

                if (dtt != null)
                {
                    if (dtt.Rows.Count > 0)
                    {
                        decimal TotalQuantity = 0;
                        decimal TotalAmount = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            TotalQuantity += Convert.ToDecimal(dr["Qty"].ToString());
                            TotalAmount += Convert.ToDecimal(dr["UnitPrice"].ToString()) * Convert.ToDecimal(dr["Qty"].ToString());

                        }
                        DataTable dt2 = IdManager.GetShowDataTable("Select   * from dbo.GL_SET_OF_BOOKS where BOOK_NAME='AMB'");

                        ReportViewer1.ProcessingMode = ProcessingMode.Local;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("Report/rptStockTransferBill_Challan.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter("ReportType", "Item Transfer Information"));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter("ReportDate", DateTime.Now.ToString("dd/MM/yyyy")));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter("TotalQuantity", TotalQuantity.ToString("N2")));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter("TotalAmount", TotalAmount.ToString("N2")));

                        ReportViewer1.LocalReport.SetParameters(new ReportParameter("VatRate", "Vat-" + Convert.ToDouble(dt.Rows[0]["VatRate"]).ToString("N1") + "%"));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter("BranchName", dt.Rows[0]["BranchName"].ToString()));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter("Address", dt.Rows[0]["Address1"].ToString()));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter("PhoneNo", dt.Rows[0]["Phone"].ToString()));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter("Remarks", dt.Rows[0]["Remark"].ToString()));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter("ChallanNo", dtt.Rows[0]["ChallanNo"].ToString()));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter("TransferCode", dtt.Rows[0]["Code"].ToString()));
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter("TransferDate", dt.Rows[0]["TransferDate"].ToString()));

                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("itemstockTransferReport", dt));
                        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Company", dt2));
                        ReportViewer1.LocalReport.Refresh();

                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Empty Data..!!','teal',3);", true);

                       

                        ReportViewer1.Reset();
                        return;
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Empty Data..!!','teal',3);", true);

                    ReportViewer1.Reset();
                    return;
                }

            }
            ModalPopupExtenderLogin3.Show();
            //PrintSingleBranchStockReport(Parameter);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Empty Data..!!','teal',3);", true);

            ReportViewer1.Reset();
            return;
        }
        ModalPopupExtenderLogin3.Show();
  }
   
    
    
    private void PrintSingleBranchStockReport(string Parameter)
    {

        String connectionString = DataManager.OraConnString();
        DataTable dtt = IdManager.GetShowDataTable("select " + lblPrintFlag.Text + " ID,BranchID,TransferDate,ChallanNo,Code from ItemStockTransferMst " + Parameter + " order by ID desc");  

        if (dtt.Rows.Count > 0)
        {
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename='Transfer-Stock-Bill-"+Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy")+"'.pdf");
            Document document = new Document(PageSize.A4, 10f, 20f, 20f, 20f);
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();

            PdfPCell cell;
            byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
            iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
            gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
            gif.ScalePercent(15f);

            float[] titwidth = new float[2] { 10, 200 };
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

           // cell = new PdfPCell(gif);
          cell = new PdfPCell(new Phrase(""));
          
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Rowspan = 1;
            cell.Colspan = 2;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 150f;
            dth.AddCell(cell);


            cell = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 2;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);
            cell.BorderWidth = 0f;     
            //cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.BOLD)));
            //cell.HorizontalAlignment = 1;
            //cell.VerticalAlignment = 1;
            //cell.Colspan = 2;
            
            //cell.BorderWidth = 0f;           
            //dth.AddCell(cell);
            //cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL)));
            //cell.HorizontalAlignment = 1;
            //cell.VerticalAlignment = 1;
            //cell.Colspan = 7;
            //cell.BorderWidth = 0f;           
            //dth.AddCell(cell);
            //cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL)));
            //cell.HorizontalAlignment = 1;
            //cell.VerticalAlignment = 1;
            //cell.Colspan = 7;
            //cell.BorderWidth = 0f;            
            //dth.AddCell(cell);
            //string Heading = "";
            ////if (string.IsNullOrEmpty(ddlBranch.SelectedItem.Text))
            ////{
            ////    Heading = "Total Items Stock On " + lblBranchName.Text;
            ////}
            ////else { Heading = "Transfer Stock" + ddlBranch.SelectedItem.Text; }
            //cell = new PdfPCell(new Phrase("Transfer Items Details", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            //cell.HorizontalAlignment = 1;
            //cell.VerticalAlignment = 1;
            //cell.Colspan = 7;
            //cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            //dth.AddCell(cell);
            document.Add(dth);
           // LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
           // document.Add(line);         

            foreach (DataRow row in dtt.Rows)
            {
                
                DataTable dt = aclsItemTransferStockManager.getShowTransferDetailsOnReport(row["ID"].ToString(), connectionString);
                float[] wdth = new float[7] { 15, 2, 40,25, 15, 2, 30 };
                PdfPTable pdt = new PdfPTable(wdth);
                pdt.WidthPercentage = 100;

                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 7;
                cell.FixedHeight = 18f;
                pdt.AddCell(cell);
                ////////////////////////*********************************************
                cell = new PdfPCell(FormatHeaderPhrase("Branch Name"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase9(dt.Rows[0]["BranchName"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 1;
                pdt.AddCell(cell);

                
                cell = new PdfPCell(FormatHeaderPhrase("Bill No"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(row["ChallanNo"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 1;
                pdt.AddCell(cell);

                
                //**************************************************************

                cell = new PdfPCell(FormatHeaderPhrase("Address"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(dt.Rows[0]["Address1"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 1;
                pdt.AddCell(cell);


                cell = new PdfPCell(FormatHeaderPhrase("Transfer Code"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(row["Code"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 1;
                pdt.AddCell(cell);

               

                //*********************************
                cell = new PdfPCell(FormatHeaderPhrase("Phone No"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(dt.Rows[0]["Phone"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);


                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 1;
                pdt.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Transfer Date"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(dt.Rows[0]["TransferDate"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);

               
                //*************************************************

                cell = new PdfPCell(FormatHeaderPhrase("Remark "));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(dt.Rows[0]["Remark"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 2;
                pdt.AddCell(cell);


                cell = new PdfPCell(FormatHeaderPhrase("Vat RegNo"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 1;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);


                cell = new PdfPCell(FormatHeaderPhrase( dt.Rows[0]["VatRegNo"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 2;
                pdt.AddCell(cell);


                document.Add(pdt);

                float[] widthdtl = new float[13] { 4, 22, 10, 7, 8, 10,8,5,5,8,7,8 ,9};
                PdfPTable pdtdtl = new PdfPTable(widthdtl);
                pdtdtl.WidthPercentage = 100;

                //********************** Details ******//
                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 15f;
                cell.Border = 0;
                cell.Colspan = 13;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("SL."));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.FixedHeight = 20f;
                //cell.PaddingTop = 12;        
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Items Name & Code"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;        
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Product Fit"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;       
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Fabrics Type"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;       
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);


                cell = new PdfPCell(FormatHeaderPhrase("Catagory"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;       
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Color"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;       
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Design"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;       
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Size"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;       
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("QTY"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.FixedHeight = 20f;        
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);


                cell = new PdfPCell(FormatHeaderPhrase("Price"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;        
                // cell.FixedHeight = 20f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Total"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;        
                // cell.FixedHeight = 20f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Vat" + Convert.ToDouble(dt.Rows[0]["VatRate"]).ToString("N0")+"%"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;        
                // cell.FixedHeight = 20f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);


                cell = new PdfPCell(FormatHeaderPhrase("Total+Vat"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;        
                // cell.FixedHeight = 20f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                int Serial = 1;
                decimal totCloseAmt = 0;
                double totalAmount = 0;
                double TotalvatAmount = 0;
                double GrandTotal = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    cell = new PdfPCell(FormatPhrase(Serial.ToString()));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    // cell.FixedHeight = 20f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);
                    Serial++;
                    
                    cell = new PdfPCell(FormatPhrase(dr["Items"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatPhrase(dr["ProductType"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);


                    cell = new PdfPCell(FormatPhrase(dr["FabricsType"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                   




                    cell = new PdfPCell(FormatPhrase(dr["Catagory"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatPhrase(dr["ColorName"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatPhrase(dr["DesigNo"].ToString()));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);


                    cell = new PdfPCell(FormatPhrase(dr["SizeName"].ToString()));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatPhrase(dr["Qty"].ToString()));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;

                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatPhrase(dr["UnitPrice"].ToString()));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatPhrase((Convert.ToDouble(dr["UnitPrice"]) * Convert.ToDouble(dr["Qty"])).ToString("N2")));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                    double totalvat = (Convert.ToDouble(dr["UnitPrice"]) * Convert.ToDouble(dr["Qty"])) * (Convert.ToDouble(dr["VatRate"]) / 100);
                    cell = new PdfPCell(FormatPhrase((totalvat).ToString("N2")));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);


                    cell = new PdfPCell(FormatPhrase((+(Convert.ToDouble(dr["UnitPrice"]) * Convert.ToDouble(dr["Qty"]))+totalvat).ToString("N2")));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                    GrandTotal += totalvat+(Convert.ToDouble(dr["UnitPrice"]) * Convert.ToDouble(dr["Qty"]));
                    totalAmount += Convert.ToDouble(dr["UnitPrice"]) * Convert.ToDouble(dr["Qty"]);
                    TotalvatAmount += totalvat;
                    totCloseAmt += Convert.ToDecimal(dr["Qty"].ToString());
                }
                cell = new PdfPCell(FormatHeaderPhrase("Total"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                // cell.FixedHeight = 20f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 8;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(totCloseAmt.ToString("N0")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;                
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(totalAmount.ToString("N2")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(TotalvatAmount.ToString("N2")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(GrandTotal.ToString("N2")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;                
                cell.FixedHeight = 20f;
                cell.Border = 0;
                cell.Colspan = 13;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Note:*VAT amount is payable from sales point which is paid by customer"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 20f;
                cell.Border = 0;
                cell.Colspan = 13;
                pdtdtl.AddCell(cell);


                document.Add(pdtdtl);

                LineSeparator line1 = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);                
                document.Add(line1); 
            }
            PdfPTable pdt1 = new PdfPTable(1);
            cell.FixedHeight = 10f;
            cell.Border = 0;
            pdt1.AddCell(cell);

            cell = SignatureFormat(document, cell);
            document.Close();
            Response.Flush();
            Response.End();
        }
    }
    private void PrintSingleBranchStockReportChallan(string Parameter)
    {

        String connectionString = DataManager.OraConnString();
        DataTable dtt = IdManager.GetShowDataTable("select " + lblPrintFlag.Text + " ID,BranchID,TransferDate,ChallanNo,Code from ItemStockTransferMst " + Parameter + " order by ID desc");

        if (dtt.Rows.Count > 0)
        {
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename='Transfer-Stock-Challan-" + Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy") + "'.pdf");
            Document document = new Document(PageSize.A4, 10f, 20f, 20f, 20f);
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();

            PdfPCell cell;
            byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
            iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
            gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
            gif.ScalePercent(15f);

            float[] titwidth = new float[2] { 10, 200 };
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;
            cell = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Rowspan = 1;
            cell.Colspan = 2;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 150f;
            dth.AddCell(cell);


            cell = new PdfPCell(new Phrase("", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.Colspan = 2;
            cell.BorderWidth = 0f;
            dth.AddCell(cell);
            cell.BorderWidth = 0f;    

            //cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 15, iTextSharp.text.Font.BOLD)));
            //cell.HorizontalAlignment = 1;
            //cell.VerticalAlignment = 1;
            //cell.Colspan = 7;
            //cell.BorderWidth = 0f;
            //dth.AddCell(cell);
            //cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL)));
            //cell.HorizontalAlignment = 1;
            //cell.VerticalAlignment = 1;
            //cell.Colspan = 7;
            //cell.BorderWidth = 0f;
            //dth.AddCell(cell);
            //cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL)));
            //cell.HorizontalAlignment = 1;
            //cell.VerticalAlignment = 1;
            //cell.Colspan = 7;
            //cell.BorderWidth = 0f;
            //dth.AddCell(cell);
            //string Heading = "";
            ////if (string.IsNullOrEmpty(ddlBranch.SelectedItem.Text))
            ////{
            ////    Heading = "Total Items Stock On " + lblBranchName.Text;
            ////}
            ////else { Heading = "Transfer Stock" + ddlBranch.SelectedItem.Text; }
            //cell = new PdfPCell(new Phrase("Transfer Items Details", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
            //cell.HorizontalAlignment = 1;
            //cell.VerticalAlignment = 1;
            //cell.Colspan = 7;
            //cell.BorderWidth = 0f;
            //cell.FixedHeight = 20f;
            //dth.AddCell(cell);
            document.Add(dth);
            //LineSeparator line = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
            //document.Add(line);

            foreach (DataRow row in dtt.Rows)
            {

                DataTable dt = aclsItemTransferStockManager.getShowTransferDetailsOnReport(row["ID"].ToString(), connectionString);
                float[] wdth = new float[7] { 15, 2, 40, 25, 30, 2, 30 };
                PdfPTable pdt = new PdfPTable(wdth);
                pdt.WidthPercentage = 100;

                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 7;
                cell.FixedHeight = 18f;
                pdt.AddCell(cell);
                ////////////////////////*********************************************
                cell = new PdfPCell(FormatHeaderPhrase("Branch Name"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase9(dt.Rows[0]["BranchName"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 1;
                pdt.AddCell(cell);


                cell = new PdfPCell(FormatHeaderPhrase("Challan No"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(row["ChallanNo"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 1;
                pdt.AddCell(cell);


                //**************************************************************

                cell = new PdfPCell(FormatHeaderPhrase("Address"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(dt.Rows[0]["Address1"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 1;
                pdt.AddCell(cell);


                cell = new PdfPCell(FormatHeaderPhrase("Transfer Code"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(row["Code"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 1;
                pdt.AddCell(cell);

                //*********************************
                cell = new PdfPCell(FormatHeaderPhrase("Phone No"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(dt.Rows[0]["Phone"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);


                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 1;
                pdt.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Transfer Date"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(dt.Rows[0]["TransferDate"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);


                //*************************************************

                cell = new PdfPCell(FormatHeaderPhrase("Remark "));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(":"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdt.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(dt.Rows[0]["Remark"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                cell.Colspan = 5;
                pdt.AddCell(cell);


                document.Add(pdt);

                float[] widthdtl = new float[9] { 4, 22, 10, 7, 8, 10, 8, 5, 5 };
                PdfPTable pdtdtl = new PdfPTable(widthdtl);
                pdtdtl.WidthPercentage = 100;

                //********************** Details ******//
                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 15f;
                cell.Border = 0;
                cell.Colspan = 12;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("SL."));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.FixedHeight = 20f;
                //cell.PaddingTop = 12;        
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Items Name & Code"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;        
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Product Fit"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;       
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Fabrics Type"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;       
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);


                cell = new PdfPCell(FormatHeaderPhrase("Catagory"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;       
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Color"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;       
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Design"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;       
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("Size"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                //cell.PaddingTop = 12;
                // cell.FixedHeight = 20f;       
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                cell = new PdfPCell(FormatHeaderPhrase("QTY"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                // cell.FixedHeight = 20f;        
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);


               



                int Serial = 1;
                decimal totCloseAmt = 0;
                double totalAmount = 0;
                double TotalvatAmount = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    cell = new PdfPCell(FormatPhrase(Serial.ToString()));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    // cell.FixedHeight = 20f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);
                    Serial++;

                    cell = new PdfPCell(FormatPhrase(dr["Items"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatPhrase(dr["ProductType"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);


                    cell = new PdfPCell(FormatPhrase(dr["FabricsType"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);






                    cell = new PdfPCell(FormatPhrase(dr["Catagory"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatPhrase(dr["ColorName"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatPhrase(dr["DesigNo"].ToString()));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);


                    cell = new PdfPCell(FormatPhrase(dr["SizeName"].ToString()));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                    cell = new PdfPCell(FormatPhrase(dr["Qty"].ToString()));
                    cell.HorizontalAlignment = 2;
                    cell.VerticalAlignment = 1;

                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtdtl.AddCell(cell);

                    
                    double totalvat = (Convert.ToDouble(dr["UnitPrice"]) * Convert.ToDouble(dr["Qty"])) * (Convert.ToDouble(dr["VatRate"]) / 100);
                   

                    totalAmount += Convert.ToDouble(dr["UnitPrice"]) * Convert.ToDouble(dr["Qty"]);
                    TotalvatAmount += totalvat;
                    totCloseAmt += Convert.ToDecimal(dr["Qty"].ToString());
                }
                cell = new PdfPCell(FormatHeaderPhrase("Total"));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                // cell.FixedHeight = 20f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                cell.Colspan = 8;
                pdtdtl.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase(totCloseAmt.ToString("N0")));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtdtl.AddCell(cell);

                

                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.FixedHeight = 10f;
                cell.Border = 0;
                cell.Colspan = 9;
                pdtdtl.AddCell(cell);

                document.Add(pdtdtl);

                LineSeparator line1 = new LineSeparator(1, 100, null, Element.ALIGN_CENTER, -2);
                document.Add(line1);
            }
            PdfPTable pdt1 = new PdfPTable(1);
            cell.FixedHeight = 10f;
            cell.Border = 0;
            pdt1.AddCell(cell);

            cell = SignatureFormat(document, cell);
            document.Close();
            Response.Flush();
            Response.End();
        }
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
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 6));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD));
    }

    private static Phrase FormatHeaderPhrase9(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD));
    }
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlBranchSearch.SelectedItem.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Select Branch..!!','teal',3);", true);

            return;
        }

        if (string.IsNullOrEmpty(txtTfDate.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Input Transerfer Date..!!','teal',3);", true);

            return;
        }

        if (string.IsNullOrEmpty(lblID.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Select first Then Create Excel File.!!','teal',3);", true);

            return;
           
        } 
        string filename = "T_ID-" + lblID.Text + "-" + ddlBranchSearch.SelectedItem.Text.Replace(" ", "") + "-T_Date-" + txtTfDate.Text;

        DataTable dtlist = aclsItemTransferStockManager.getShowTransfrerDetails(lblID.Text);
        if (dtlist.Rows.Count > 0)
        {
            aclsItemTransferStockManager.UpdateBranchInfoExcelRecord(lblID.Text, Session["user"].ToString());
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dtlist, "Stock");
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xlsx");

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
    protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
    {
        int ckeck = 0;
        if (chkAllItems.Checked)
        {
            ckeck = 1;
            
            DataTable dtItemsAll = _aItemManager.GetItemsInformationDetails("", "", "", "", "", ckeck);
            Session["purdtl"] = dtItemsAll;
            dgPODetailsDtl.DataSource = dtItemsAll;
            dgPODetailsDtl.DataBind();
            ShowFooterTotal();
        }
    }


    protected void dgHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgHistory.DataSource = ViewState["History"];
        dgHistory.PageIndex = e.NewPageIndex;
        dgHistory.DataBind();
    }
    protected void txtCode_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
        DataTable dtdtl = (DataTable)Session["purdtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        DataTable dt = _aItemManager.GetItemsInformationDetailsCode("", "", "", "", "", 0, ((TextBox)gvr.FindControl("txtCode")).Text.ToUpper());
        if (dt.Rows.Count > 0)
        {
            bool IsCheck = false;
            foreach (DataRow ddr in dtdtl.Rows)
            {
                if (string.IsNullOrEmpty(dr["ItemsName"].ToString()))
                {
                    if (ddr["ItemsID"].ToString().Equals(((DataRow)dt.Rows[0])["ItemsID"].ToString()))
                    {
                        IsCheck = true;
                       //ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('This items already added...!!!');", true);
                        if (Convert.ToDouble(dt.Rows[0]["StockQty"].ToString()) > Convert.ToDouble(ddr["TransferQty"].ToString()))
                        {
                            ddr["TransferQty"] = Convert.ToDouble(ddr["TransferQty"].ToString()) + 1;
                            dgPODetailsDtl.DataSource = dtdtl;
                            Session["purdtl"] = dtdtl;
                            //Session["purdtl"] = dtdtl;
                            dgPODetailsDtl.DataBind();
                            ShowFooterTotal();
                            ((TextBox)dgPODetailsDtl.Rows[gvr.RowIndex].FindControl("txtCode")).Text = "";
                            ((TextBox)dgPODetailsDtl.Rows[gvr.RowIndex].FindControl("txtCode")).Focus();
                            return;

                        }

                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Quantity Is Over...!!','orange',2);", true);

                          
                            ((TextBox)dgPODetailsDtl.Rows[gvr.RowIndex].FindControl("txtCode")).Text = "";
                            ((TextBox)dgPODetailsDtl.Rows[gvr.RowIndex].FindControl("txtCode")).Focus();
                            return;
                       
                        }

                    }
                }
            }
           

            dr["ItemsID"] = ((DataRow)dt.Rows[0])["ItemsID"].ToString();
            dr["item_code"] = ((DataRow)dt.Rows[0])["item_code"].ToString();
            dr["ItemsName"] = ((DataRow)dt.Rows[0])["item_desc"].ToString();
            //dr["Type"] = ((DataRow)dt.Rows[0])["Type"].ToString();
            dr["StockQty"] = ((DataRow)dt.Rows[0])["StockQty"].ToString();
            dr["Price"] = ((DataRow)dt.Rows[0])["Price"].ToString();
            dr["TransferQty"] = "1";
            //dr["Catagory"] = ((DataRow)dt.Rows[0])["Category"].ToString();
            //dr["SubCatagory"] = ((DataRow)dt.Rows[0])["SubCategory"].ToString();
            dr["UMO"] = ((DataRow)dt.Rows[0])["UMO"].ToString();
            dr["StyleNo"] = ((DataRow)dt.Rows[0])["StyleNo"].ToString();
            dr["BranchSalesPrice"] = ((DataRow)dt.Rows[0])["BranchSalesPrice"].ToString();
            dr["ReceivedQuantity"] = "0";
            //dtdtl.Rows.Add(dr);
            string found = "";
            foreach (DataRow drd in dtdtl.Rows)
            {
                if (drd["ItemsID"].ToString() == "" && drd["ItemsName"].ToString() == "")
                {
                    found = "Y";
                }
            }
            if (found == "")
            {
                DataRow drd = dtdtl.NewRow();
                dtdtl.Rows.Add(drd);
            }
            dgPODetailsDtl.DataSource = dtdtl;
            Session["purdtl"] = dtdtl;
            //Session["purdtl"] = dtdtl;
            dgPODetailsDtl.DataBind();
            ShowFooterTotal();

            ((TextBox)dgPODetailsDtl.Rows[gvr.DataItemIndex + 1].FindControl("txtCode")).Focus();
            //Up1.Update();

        }
        else
        {
            //((TextBox)dgPODetailsDtlReject.Rows[dgPODetailsDtl.Rows.Count].FindControl("txtCode")).Text = "";
            //((TextBox)dgPODetailsDtlReject.Rows[gvr.DataItemIndex].FindControl("txtCode")).Focus();
        }
    }
    protected void txtSearchCarton_TextChanged(object sender, EventArgs e)
    {
        string CartonNo = "";
        string[] words = txtSearchCarton.Text.Split(':');
        if (words.Length == 1)
        {

            CartonNo = txtSearchCarton.Text;
            //P09602600026
        }
        else
        {

            DataTable dtl = IdManager.GetShowDataTable("Select CartonNo from CartonReceivedMST where CartonNo!='' and CartonNo is not null and TransferStatus is null and UPPER('CartonNo:'+CartonNo) = UPPER('" + txtSearchCarton.Text + "')");
            if (dtl.Rows.Count > 0)
            {
                CartonNo = dtl.Rows[0]["CartonNo"].ToString();
            }
            else
            {
                txtSearchCarton.Text = "";
                txtSearchCarton.Focus();
                return;
            }
        }

        txtSearchCarton.Text = CartonNo;
        DataTable dt = _aItemManager.GetCartonWiseItemInfo(CartonNo);

        if (dt.Rows.Count > 0)
        {
            dgPODetailsDtl.DataSource = dt;
            Session["purdtl"] = dt;
            //Session["purdtl"] = dtdtl;
            dgPODetailsDtl.DataBind();
            ShowFooterTotal();

        }
        else
        {
            txtSearchCarton.Text = "";
        }

        dgPODetailsDtl.Enabled = false;
        Up1.Update();

    }
    //protected void txtItemNameReject_TextChanged(object sender, EventArgs e)
    //{
    //    GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
    //    DataTable dtdtl = (DataTable)Session["purdtlReject"];
    //    DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
    //    string CartonNo = "";
    //    string[] words = ((TextBox)gvr.FindControl("txtItemNameReject")).Text.Split('-');
    //    DataTable dt = new DataTable();
    //    if (words.Length == 1)
    //    {
    //        dt = IdManager.GetShowDataTable("select * from View_SearchItemsInformation where upper(NewItemCode) = upper('" +
    //            ((TextBox)gvr.FindControl("txtItemNameReject")).Text.ToUpper() + "') and [StockQty]>0 And DeleteBy IS NULL and ItemType='R'");
    //    }
    //    else
    //    {
    //        dt = IdManager.GetShowDataTable("select * from View_SearchItemsInformation where upper(SearchItem) = upper('" +
    //             ((TextBox)gvr.FindControl("txtItemNameReject")).Text.ToUpper() + "') and [StockQty]>0 And DeleteBy IS NULL and ItemType='R'");
    //    }
    //    if (dt.Rows.Count > 0)
    //    {
    //        bool IsCheck = false;
    //        foreach (DataRow ddr in dtdtl.Rows)
    //        {
    //            if (string.IsNullOrEmpty(dr["ItemsName"].ToString()))
    //            {
    //                if (ddr["ItemsID"].ToString().Equals(((DataRow)dt.Rows[0])["ItemsID"].ToString()))
    //                {
    //                    IsCheck = true;

    //                    //ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('This items already added...!!!');", true);
    //                    if (Convert.ToDouble(dt.Rows[0]["StockQty"].ToString()) > Convert.ToDouble(ddr["TransferQty"].ToString()))
    //                    {
    //                        ddr["TransferQty"] = Convert.ToDouble(ddr["TransferQty"].ToString()) + 1;
    //                        dgPODetailsDtlReject.DataSource = dtdtl;
    //                        Session["purdtlReject"] = dtdtl;
    //                        //Session["purdtl"] = dtdtl;
    //                        dgPODetailsDtlReject.DataBind();
    //                        // ShowFooterTotal();
    //                        ((TextBox)dgPODetailsDtlReject.Rows[gvr.RowIndex].FindControl("txtItemNameReject")).Text = "";
    //                        ((TextBox)dgPODetailsDtlReject.Rows[gvr.RowIndex].FindControl("txtItemNameReject")).Focus();
                          
    //                        return;

    //                    }

    //                    else
    //                    {
    //                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Transfer Stock Upper then Stock Quantity.!!');", true);
    //                        ((TextBox)dgPODetailsDtlReject.Rows[gvr.RowIndex].FindControl("txtItemNameReject")).Text = "";
    //                        ((TextBox)dgPODetailsDtlReject.Rows[gvr.RowIndex].FindControl("txtItemNameReject")).Focus();
    //                        Up2.Update();
    //                        return;

    //                    }

    //                }
    //            }
    //        }
            

    //        dr["ItemsID"] = ((DataRow)dt.Rows[0])["ItemsID"].ToString();
    //        dr["NewItemCode"] = ((DataRow)dt.Rows[0])["NewItemCode"].ToString();
    //        dr["item_code"] = ((DataRow)dt.Rows[0])["NewItemCode"].ToString();
            
    //        dr["ItemsName"] = ((DataRow)dt.Rows[0])["item_desc"].ToString();
    //        //dr["Type"] = ((DataRow)dt.Rows[0])["Type"].ToString();
    //        dr["StockQty"] = ((DataRow)dt.Rows[0])["StockQty"].ToString();
    //        dr["Price"] = ((DataRow)dt.Rows[0])["Price"].ToString();
    //        dr["TransferQty"] = "1";
    //        dr["Discount"] = "0";
    //        //dr["SubCatagory"] = ((DataRow)dt.Rows[0])["SubCategory"].ToString();
    //        dr["UMO"] = ((DataRow)dt.Rows[0])["UMO"].ToString();
    //        dr["StyleNo"] = ((DataRow)dt.Rows[0])["StyleNo"].ToString();
    //        dr["BranchSalesPrice"] = ((DataRow)dt.Rows[0])["BranchSalesPrice"].ToString();
    //        dr["ReceivedQuantity"] = "0";
    //        //dtdtl.Rows.Add(dr);
    //        string found = "";
    //        foreach (DataRow drd in dtdtl.Rows)
    //        {
    //            if (drd["ItemsID"].ToString() == "" && drd["ItemsName"].ToString() == "")
    //            {
    //                found = "Y";
    //            }
    //        }
    //        if (found == "")
    //        {
    //            DataRow drd = dtdtl.NewRow();
    //            dtdtl.Rows.Add(drd);
    //        }
    //        dgPODetailsDtlReject.DataSource = dtdtl;
    //        Session["purdtlReject"] = dtdtl;
    //        //Session["purdtl"] = dtdtl;
    //        dgPODetailsDtlReject.DataBind();
    //        ShowFooterTotal();

    //        ((TextBox)dgPODetailsDtlReject.Rows[gvr.DataItemIndex + 1].FindControl("txtItemNameReject")).Focus();
    //        Up2.Update();
    //    }
    //}
    //protected void txtDiscountReject_TextChanged(object sender, EventArgs e)
    //{
    //    GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
    //    DataTable dtdtl = (DataTable)Session["purdtlReject"];
    //    DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
    //    clsItemTransferStock aclsItemTransferStock = aclsItemTransferStockManager.GetStockTransferInfo(lblID.Text);

    //    if (((TextBox)gvr.FindControl("txtDiscountReject")).Text!="")
    //            {
    //                dr["Discount"] = ((TextBox)gvr.FindControl("txtDiscountReject")).Text;
    //            }
    //            else
    //            {
    //                dr["Discount"] = "0";
    //                return;
    //            }
    
      

    //    dgPODetailsDtlReject.DataSource = dtdtl;
    //    Session["purdtlReject"] = dtdtl;
    //    dgPODetailsDtlReject.DataBind();
    //    //ShowFooterTotal();
    //    if (dgPODetailsDtlReject.Rows.Count != gvr.DataItemIndex + 1)
    //    {
    //        ((TextBox)dgPODetailsDtlReject.Rows[gvr.DataItemIndex + 1].FindControl("txtDiscountReject")).Focus();
    //    }
    //    Up2.Update();
    //}
    //protected void txtTransferQuantityReject_TextChanged(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
    //        DataTable dtdtl = (DataTable)Session["purdtlReject"];
    //        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
    //        clsItemTransferStock aclsItemTransferStock = aclsItemTransferStockManager.GetStockTransferInfo(lblID.Text);
    //        if (!string.IsNullOrEmpty(((TextBox)gvr.FindControl("txtTransferQuantityReject")).Text))
    //        {
    //            if (aclsItemTransferStock != null)
    //            {
    //                if ((Convert.ToDouble(dr["StockQty"]) + Convert.ToDouble(dr["TransferQty"])) >= Convert.ToDouble(((TextBox)gvr.FindControl("txtTransferQuantityReject")).Text))
    //                {
    //                    dr["TransferQty"] = ((TextBox)gvr.FindControl("txtTransferQuantityReject")).Text;
    //                }
    //                else
    //                {
    //                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Transfer Stock Upper then Stock Quantity.!!');", true);
    //                    ((TextBox)gvr.FindControl("txtTransferQuantity")).Text = dr["txtTransferQuantityReject"].ToString();
    //                    ((TextBox)dgPODetailsDtlReject.Rows[gvr.DataItemIndex].FindControl("txtTransferQuantityReject")).Focus();
    //                    return;
    //                }
    //            }
    //            else
    //            {
    //                if (Convert.ToDouble(dr["StockQty"]) >= Convert.ToDouble(((TextBox)gvr.FindControl("txtTransferQuantityReject")).Text))
    //                {
    //                    dr["TransferQty"] = ((TextBox)gvr.FindControl("txtTransferQuantityReject")).Text;
    //                }
    //                else
    //                {

    //                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Transfer Stock Upper then Stock Quantity.!!');", true);
    //                    ((TextBox)gvr.FindControl("txtTransferQuantityReject")).Text = dr["TransferQty"].ToString();
    //                    //((TextBox)gvr.FindControl("txtTransferQuantity")).Focus();
    //                    ((TextBox)dgPODetailsDtlReject.Rows[gvr.DataItemIndex].FindControl("txtTransferQuantityReject")).Focus();
    //                    return;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('please input Transfer Quantity!');", true);
    //            return;
    //        }

    //        dgPODetailsDtlReject.DataSource = dtdtl;
    //        Session["purdtlReject"] = dtdtl;
    //        dgPODetailsDtlReject.DataBind();
    //        //ShowFooterTotal();
    //        if (dgPODetailsDtlReject.Rows.Count != gvr.DataItemIndex + 1)
    //        {
    //            ((TextBox)dgPODetailsDtlReject.Rows[gvr.DataItemIndex + 1].FindControl("txtTransferQuantityReject")).Focus();
    //        }
    //    }
    //    catch
    //    {
    //    }
        
    //}
    //protected void btnSaveReject_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
           
    //        if (string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text))
    //        {
    //            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Select Branch.!!');", true);
    //            ddlBranchReject.Focus();
    //            return;
    //        }
    //        if (string.IsNullOrEmpty(txtTfDateReject.Text))
    //        {
    //            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input Date.!!');", true);
    //            txtTfDateReject.Focus();
    //            return;
    //        }
    //        DataTable dt = null;
    //        if (Session["purdtlReject"] != null)
    //        {
    //            dt = (DataTable)Session["purdtlReject"];
    //        }
    //        else
    //        {
    //            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Add this transfer items in list.!!');", true);
    //            return;
    //        }

    //        clsItemTransferStock aclsItemTransferStock = aclsItemTransferStockManager.GetStockTransferInfo(lblIDReject.Text);
    //        if (aclsItemTransferStock != null)
    //        {
    //            if (per.AllowEdit == "Y")
    //            {
    //                int CheckReceived = IdManager.GetShowSingleValueInt("COUNT(*)", "ReceivedBy IS NOT NULL AND ID", "ItemStockTransferMst", lblIDReject.Text);
    //                string CheckExcelFlag = IdManager.GetShowSingleValueString("ExcelUser", "ID", "ItemStockTransferMst", lblIDReject.Text);
    //                if (!string.IsNullOrEmpty(CheckExcelFlag))
    //                {
    //                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Alrady Generate Excel File.you are not update this.!!');", true);
    //                    return;
    //                }
    //                if (CheckReceived > 0)
    //                {
    //                    ClientScript.RegisterStartupScript(this.GetType(), "ale",
    //                        "alert('this transfer items alrady received .\\n you never edit this transfer items.\\n are  you want please contract to administratio.!!');",
    //                        true);
    //                    return;
    //                }
    //                aclsItemTransferStock.Code = txtTransferCodeReject.Text.Replace("'", "").Replace(",", "").Trim();
    //                aclsItemTransferStock.TransferDate = txtTfDateReject.Text.Replace("'", "").Replace(",", "").Trim();
    //                aclsItemTransferStock.BranchId = ddlBranchReject.SelectedValue.Replace("'", "").Replace(",", "").Trim();
    //                aclsItemTransferStock.LoginBy = Session["userID"].ToString().Replace("'", "").Replace(",", "").Trim();
    //                //aclsItemTransferStock.CartonNo = txtSearchCartonReject.Text;
    //                aclsItemTransferStock.ChallanNo = txtChallanNoReject.Text.Replace("'", "").Replace(",", "").Trim();
    //                aclsItemTransferStock.TransferType = "R";
    //                aclsItemTransferStock.Remark = txtRemarkReject.Text.Replace("'", "").Replace("'", "").Replace(",", "").Trim();
    //                string Parameter = "where [MstID]='" + lblIDReject.Text.Replace("'", "").Replace(",", "").Trim() + "' AND t1.DeleteBy IS NULL";
    //                DataTable dtOld = aclsItemTransferStockManager.GetShowItemsDetails(Parameter,"R");

    //                string ITSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO",
    //                    "t1.PAYEE='IT' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
    //                    txtTransferCodeReject.Text);
    //                //*************************** Account Entry (Update) ******************//
    //                //********* Jurnal Voucher - 1 *********//
    //                VouchMst vmst = VouchManager.GetVouchMst(ITSerial.Trim());

    //                if (vmst != null)
    //                {
    //                    vmst.FinMon = FinYearManager.getFinMonthByDate(txtTfDateReject.Text);
    //                    vmst.ValueDate = txtTfDateReject.Text;
    //                    vmst.VchCode = "03";
    //                    // vmst.RefFileNo = "";
    //                    // vmst.VolumeNo = "";
    //                    //vmst.SerialNo = lblID.Text;
    //                    vmst.Particulars = txtRemarkReject.Text.Replace("'", "").Replace(",", "").Trim();
    //                    //vmst.ControlAmt = Convert.ToDouble(txtTotal.Text).ToString().Replace(",", "");
    //                    vmst.UpdateUser = Session["user"].ToString().Replace("'", "").Replace(",", "").Trim();
    //                    vmst.UpdateDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
    //                    vmst.AuthoUserType = Session["userlevel"].ToString();
    //                }

    //                aclsItemTransferStockManager.UpdateBranchInfo(aclsItemTransferStock, dt, dtOld,vmst);
    //                //Refresh();
    //                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are updated suceessfullly!!');", true);
    //                btnSaveReject.Enabled = false;
    //            }
    //            else
    //            {
    //                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You have not enough permissoin to update this record!!');", true);
    //            }
    //        }
    //        else
    //        {
    //            if (per.AllowAdd == "Y")
    //            {
    //                aclsItemTransferStock = new clsItemTransferStock();
    //                aclsItemTransferStock.TransferDate = txtTfDateReject.Text.Replace("'", "").Replace(",", "").Trim();
    //                aclsItemTransferStock.BranchId = ddlBranchReject.SelectedValue.Replace("'", "").Replace(",", "").Trim();
    //                aclsItemTransferStock.LoginBy = Session["userID"].ToString().Replace("'", "").Replace(",", "").Trim();
    //                if (string.IsNullOrEmpty(txtRemarkReject.Text))
    //                {
    //                    txtRemarkReject.Text = "Transfer To " + ddlBranchReject.SelectedItem.Text +
    //                                     " From Head Office(Dorjibari). Transfer By : " + Session["user"].ToString() +
    //                                     " . Transfer Date :" + DateTime.Now.ToString("dd-MMM-yyyy") + ".";
    //                }
    //                aclsItemTransferStock.Remark = txtRemarkReject.Text.Replace("'", "").Replace(",", "").Trim();
    //                aclsItemTransferStock.ChallanNo = txtChallanNoReject.Text.Replace("'", "").Replace(",", "").Trim();
    //                aclsItemTransferStock.TransferType = "R";
    //                txtTransferCodeReject.Text = "TR-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +
    //                               DateTime.Now.ToString("dd") + "00" +
    //                               IdManager.GetShowSingleValueInt("TransferID", "ID", "FixValue", "1").ToString();
    //                aclsItemTransferStock.Code = txtTransferCodeReject.Text.Replace("'", "");
    //                //aclsItemTransferStock.CartonNo = txtSearchCarton.Text;

    //                VouchMst vmst = new VouchMst();
    //                vmst.FinMon = FinYearManager.getFinMonthByDate(txtTfDateReject.Text);
    //                vmst.ValueDate = txtTfDateReject.Text.Replace("'", "").Replace(",", "").Trim();
    //                vmst.VchCode = "03";
    //                vmst.RefFileNo = "";
    //                vmst.VolumeNo = "";
    //                vmst.SerialNo = txtTransferCodeReject.Text.Replace("'", "").Replace(",", "").Trim();
    //                vmst.Particulars = "Transfer To Branch " + ddlBranchReject.SelectedItem.Text.Replace("'", "").Replace(",", "").Trim();
    //                vmst.Payee = "IT";
    //                //vmst.CheckNo = txtChequeNo.Text;
    //                //vmst.CheqDate = txtChequeDate.Text;
    //                vmst.CheqAmnt = "0";
    //                vmst.MoneyRptNo = "";
    //                vmst.MoneyRptDate = "";
    //                vmst.TransType = "T";
    //                vmst.BookName = "AMB";
    //                vmst.EntryUser = Session["user"].ToString();
    //                vmst.EntryDate = Convert.ToDateTime(Session["date"]).ToString("dd/MM/yyyy");
    //                vmst.VchSysNo = IdManager.GetNextID("gl_trans_mst", "vch_sys_no").ToString();
    //                vmst.VchRefNo = "JV-" + vmst.VchSysNo.ToString().PadLeft(10, '0');
    //                vmst.Status = "A";
    //                vmst.AuthoUserType = Session["userlevel"].ToString();


                   
                   
    //                VouchMst vmstPayment = new VouchMst();

    //                aclsItemTransferStockManager.SaveInformation(aclsItemTransferStock, dt,vmst,vmstPayment);
    //                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are created suceessfullly!!');", true);
    //                btnSaveReject.Enabled = false;
    //            }
    //            else
    //            {
    //                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You have not enough permissoin to update this record!!');", true);
    //            }
    //        }
    //    }
    //    catch (FormatException fex)
    //    {
    //        ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('" + fex.Message + "');", true);
    //    }
    //    catch (Exception ex)
    //    {
    //        if (ex.Message.Contains("Database"))
    //            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Database Maintain Error. Contact to the Software Provider..!!');", true);
    //        else
    //            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('There is some problem to do the task. Try again properly.!!');", true);
    //    }  
    //}
    //protected void btnClearReject_Click(object sender, EventArgs e)
    //{
    //    RefreshReject();
    //    TabContainer1.ActiveTabIndex = 1;
    //}
    //protected void txtCodeReject_TextChanged(object sender, EventArgs e)
    //{
    //    GridViewRow gvr = (GridViewRow)((TextBox)sender).NamingContainer;
    //    DataTable dtdtl = (DataTable)Session["purdtlReject"];
    //    DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
    //    DataTable dt = _aItemManager.GetItemsInformationDetailsCode("", "", "", "", "", 0, ((TextBox)gvr.FindControl("txtCodeReject")).Text.ToUpper());
    //    if (dt.Rows.Count > 0)
    //    {
    //        bool IsCheck = false;
    //        foreach (DataRow ddr in dtdtl.Rows)
    //        {
    //            if (string.IsNullOrEmpty(dr["ItemsName"].ToString()))
    //            {
    //                if (ddr["ItemsID"].ToString().Equals(((DataRow)dt.Rows[0])["ItemsID"].ToString()))
    //                {
    //                    IsCheck = true;



    //                    //ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('This items already added...!!!');", true);
    //                    if (Convert.ToDouble(dt.Rows[0]["StockQty"].ToString()) > Convert.ToDouble(ddr["TransferQty"].ToString()))
    //                    {
    //                        ddr["TransferQty"] = Convert.ToDouble(ddr["TransferQty"].ToString()) + 1;
    //                        dgPODetailsDtlReject.DataSource = dtdtl;
    //                        Session["purdtlReject"] = dtdtl;
    //                        //Session["purdtl"] = dtdtl;
    //                        dgPODetailsDtlReject.DataBind();
    //                        // ShowFooterTotal();
    //                        ((TextBox)dgPODetailsDtlReject.Rows[gvr.RowIndex].FindControl("txtCodeReject")).Text = "";
    //                        ((TextBox)dgPODetailsDtlReject.Rows[gvr.RowIndex].FindControl("txtCodeReject")).Focus();
    //                        Up2.Update();
    //                        return;

    //                    }

    //                    else
    //                    {
    //                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", "alert('Transfer Stock Upper then Stock Quantity.!!');", true);
    //                        ((TextBox)dgPODetailsDtlReject.Rows[gvr.RowIndex].FindControl("txtCodeReject")).Text = "";
    //                        ((TextBox)dgPODetailsDtlReject.Rows[gvr.RowIndex].FindControl("txtCodeReject")).Focus();
    //                        Up2.Update();
    //                        return;

    //                    }

    //                }
    //            }
    //        }

    //        dr["ItemsID"] = ((DataRow)dt.Rows[0])["ItemsID"].ToString();
    //        dr["item_code"] = ((DataRow)dt.Rows[0])["item_code"].ToString();
    //        dr["ItemsName"] = ((DataRow)dt.Rows[0])["item_desc"].ToString();
    //        //dr["Type"] = ((DataRow)dt.Rows[0])["Type"].ToString();
    //        dr["StockQty"] = ((DataRow)dt.Rows[0])["StockQty"].ToString();
    //        dr["Price"] = ((DataRow)dt.Rows[0])["Price"].ToString();
    //        dr["TransferQty"] = "1";
    //        //dr["Catagory"] = ((DataRow)dt.Rows[0])["Category"].ToString();
    //        //dr["SubCatagory"] = ((DataRow)dt.Rows[0])["SubCategory"].ToString();
    //        dr["UMO"] = ((DataRow)dt.Rows[0])["UMO"].ToString();
    //        dr["StyleNo"] = ((DataRow)dt.Rows[0])["StyleNo"].ToString();
    //        dr["BranchSalesPrice"] = ((DataRow)dt.Rows[0])["BranchSalesPrice"].ToString();
    //        dr["ReceivedQuantity"] = "0";
    //        //dtdtl.Rows.Add(dr);
    //        string found = "";
    //        foreach (DataRow drd in dtdtl.Rows)
    //        {
    //            if (drd["ItemsID"].ToString() == "" && drd["ItemsName"].ToString() == "")
    //            {
    //                found = "Y";
    //            }
    //        }
    //        if (found == "")
    //        {
    //            DataRow drd = dtdtl.NewRow();
    //            dtdtl.Rows.Add(drd);
    //        }
    //        dgPODetailsDtlReject.DataSource = dtdtl;
    //        Session["purdtlReject"] = dtdtl;
    //        //Session["purdtl"] = dtdtl;
    //        dgPODetailsDtlReject.DataBind();
    //        //ShowFooterTotal();
           
    //        ((TextBox)dgPODetailsDtlReject.Rows[gvr.DataItemIndex + 1].FindControl("txtCodeReject")).Focus();

    //        Up2.Update();
    //    }

    //    else
    //    {
    //        ((TextBox)dgPODetailsDtlReject.Rows[dgPODetailsDtl.Rows.Count ].FindControl("txtCodeReject")).Text = "";
    //        ((TextBox)dgPODetailsDtlReject.Rows[gvr.DataItemIndex ].FindControl("txtCodeReject")).Focus();
    //    }
        
        
    //}
    //protected void btnPrintReject_Click(object sender, EventArgs e)
    //{
    //    string Parameter = "";
    //    if (!string.IsNullOrEmpty(lblIDReject.Text))
    //    {
    //        Parameter = " where ID='" + lblIDReject.Text + "' AND DeleteBy IS NULL  ";
    //    }

    //    else
    //    {
    //        if (!string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && string.IsNullOrEmpty(txtTfDate.Text))
    //        {
    //            Parameter = " where BranchID='" + ddlBranchReject.SelectedValue + "' AND DeleteBy IS NULL  ";
    //        }
    //        else if (string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDateReject.Text))
    //        {
    //            Parameter = " where convert(date,TransferDate,103)=convert(date,'" + txtTfDateReject.Text + "',103) AND DeleteBy IS NULL  ";
    //        }
    //        else if (!string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDateReject.Text))
    //        {
    //            Parameter = " where BranchID='" + ddlBranchReject.SelectedValue + "' and convert(date,TransferDate,103)=convert(date,'" + txtTfDateReject.Text + "',103) AND DeleteBy IS NULL  ";
    //        }
    //    }
    //    PrintSingleBranchStockReport(Parameter);
    //}
    //protected void dgPODetailsDtlReject_RowDeleting(object sender, GridViewDeleteEventArgs e)
    //{
    //    if (Session["purdtlReject"] != null)
    //    {
    //        DataTable dtDtlGrid = (DataTable)Session["purdtlReject"];
    //        dtDtlGrid.Rows.RemoveAt(dgPODetailsDtlReject.Rows[e.RowIndex].DataItemIndex);
            
              

    //            if (dtDtlGrid==null)
    //        {
    //            getEmptyDtlReject();
    //        }

    //            dgPODetailsDtlReject.DataSource = dtDtlGrid;
    //            Session["purdtlReject"] = dtDtlGrid;
    //            dgPODetailsDtlReject.DataBind();
    //    }
    //    else
    //    {
    //        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Your session is over. Try it again!!');", true);
    //    }
    //}
    protected void btnChallan_Click(object sender, EventArgs e)
    {
        if (TabContainer1.TabIndex == 0)
        {
            string Parameter = "";
            if (!string.IsNullOrEmpty(lblID.Text))
            {
                Parameter = " where ID='" + lblID.Text + "' AND DeleteBy IS NULL  ";
            }

            else
            {
                if (!string.IsNullOrEmpty(ddlBranchSearch.SelectedItem.Text) && string.IsNullOrEmpty(txtTfDate.Text))
                {
                    Parameter = " where BranchID='" + ddlBranchSearch.SelectedValue + "' AND DeleteBy IS NULL  ";
                }
                else if (string.IsNullOrEmpty(ddlBranchSearch.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDate.Text))
                {
                    Parameter = " where convert(date,TransferDate,103)=convert(date,'" + txtTfDate.Text + "',103) AND DeleteBy IS NULL  ";
                }
                else if (!string.IsNullOrEmpty(ddlBranchSearch.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDate.Text))
                {
                    Parameter = " where BranchID='" + ddlBranchSearch.SelectedValue + "' and convert(date,TransferDate,103)=convert(date,'" + txtTfDate.Text + "',103) AND DeleteBy IS NULL  ";
                }
            }
            PrintSingleBranchStockReportChallan(Parameter);
        }

        else if (TabContainer1.TabIndex == 1)
        {
        //    string Parameter = "";
        //    if (!string.IsNullOrEmpty(lblIDReject.Text))
        //    {
        //        Parameter = " where ID='" + lblIDReject.Text + "' AND DeleteBy IS NULL  ";
        //    }

        //else
        //{
        //    //if (!string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && string.IsNullOrEmpty(txtTfDate.Text))
        //    //{
        //    //    Parameter = " where BranchID='" + ddlBranchReject.SelectedValue + "' AND DeleteBy IS NULL  ";
        //    //}
        //    //else if (string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) )
        //    //{
        //    //    Parameter = " where convert(date,TransferDate,103)=convert(date,'" + txtTfDateReject.Text + "',103) AND DeleteBy IS NULL  ";
        //    //}
        //    //else if (!string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDateReject.Text))
        //    //{
        //    //    Parameter = " where BranchID='" + ddlBranchReject.SelectedValue + "' and convert(date,TransferDate,103)=convert(date,'" + txtTfDateReject.Text + "',103) AND DeleteBy IS NULL  ";
        //    //}
        //}
        //PrintSingleBranchStockReportChallan(Parameter);
        }
    }
    //protected void btnChallanReject_Click(object sender, EventArgs e)
    //{
    //    string Parameter = "";
    //    if (!string.IsNullOrEmpty(lblIDReject.Text))
    //    {
    //        Parameter = " where ID='" + lblIDReject.Text + "' AND DeleteBy IS NULL  ";
    //    }

    //    else
    //    {
    //        if (!string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && string.IsNullOrEmpty(txtTfDate.Text))
    //        {
    //            Parameter = " where BranchID='" + ddlBranchReject.SelectedValue + "' AND DeleteBy IS NULL  ";
    //        }
    //        else if (string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDateReject.Text))
    //        {
    //            Parameter = " where convert(date,TransferDate,103)=convert(date,'" + txtTfDateReject.Text + "',103) AND DeleteBy IS NULL  ";
    //        }
    //        else if (!string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDateReject.Text))
    //        {
    //            Parameter = " where BranchID='" + ddlBranchReject.SelectedValue + "' and convert(date,TransferDate,103)=convert(date,'" + txtTfDateReject.Text + "',103) AND DeleteBy IS NULL  ";
    //        }
    //    }
    //    PrintSingleBranchStockReportChallan(Parameter);
    //}
    protected void txtSearchBydateCode_TextChanged(object sender, EventArgs e)
    {

    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string parameter="";
        if (!string.IsNullOrEmpty(ddlBranchSearch.SelectedItem.Text) && ddlBranchSearch.SelectedValue != "0")
        {
           
            parameter += "and t1.[BranchID]='" + ddlBranchSearch.SelectedValue + "'";
        }
         if (!string.IsNullOrEmpty(txtSearchBydateCode.Text))
        {
            parameter += " and upper('Date:'+Convert(nvarchar,t1.TransferDate,103)+'-Code:'+t1.Code)=upper('" + txtSearchBydateCode.Text + "')";
        }
         DataTable dt = aclsItemTransferStockManager.GetBranchInfo(parameter);
        if (dt.Rows.Count > 0)
        {
            dgHistory.DataSource = dt;
            ViewState["History"] = dt;
            dgHistory.DataBind();

        }
    }
    protected void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearchBydateCode.Text = "";
        ddlBranchSearch.SelectedIndex = -1;
        DataTable dt = aclsItemTransferStockManager.GetBranchInfo("");
        if (dt.Rows.Count > 0)
        {
            dgHistory.DataSource = dt;
            ViewState["History"] = dt;
            dgHistory.DataBind();

        }
    }
    protected void CancelBtn_Click(object sender, EventArgs e)
    {

    }
    protected void btnBillPrintr_Click(object sender, EventArgs e)
    {
        if (TabContainer1.ActiveTabIndex == 0)
        {
            string Parameter = "";
            if (!string.IsNullOrEmpty(lblID.Text))
            {
                Parameter = " where ID='" + lblID.Text + "' AND DeleteBy IS NULL  ";
            }

            else
            {
                if (!string.IsNullOrEmpty(ddlBranchSearch.SelectedItem.Text) && string.IsNullOrEmpty(txtTfDate.Text))
                {
                    Parameter = " where BranchID='" + ddlBranchSearch.SelectedValue + "' AND DeleteBy IS NULL  ";
                }
                else if (string.IsNullOrEmpty(ddlBranchSearch.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDate.Text))
                {
                    Parameter = " where convert(date,TransferDate,103)=convert(date,'" + txtTfDate.Text + "',103) AND DeleteBy IS NULL  ";
                }
                else if (!string.IsNullOrEmpty(ddlBranchSearch.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDate.Text))
                {
                    Parameter = " where BranchID='" + ddlBranchSearch.SelectedValue + "' and convert(date,TransferDate,103)=convert(date,'" + txtTfDate.Text + "',103) AND DeleteBy IS NULL  ";
                }
            }

            PrintSingleBranchStockReport(Parameter);
        }

        else if(TabContainer1.ActiveTabIndex==1)
        {


            //string Parameter = "";
            //if (!string.IsNullOrEmpty(lblIDReject.Text))
            //{
            //    Parameter = " where ID='" + lblIDReject.Text + "' AND DeleteBy IS NULL  ";
            //}

            //else
            //{
            //    //if (!string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && string.IsNullOrEmpty(txtTfDate.Text))
            //    //{
            //    //    Parameter = " where BranchID='" + ddlBranchReject.SelectedValue + "' AND DeleteBy IS NULL  ";
            //    //}
            //    //else if (string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDateReject.Text))
            //    //{
            //    //    Parameter = " where convert(date,TransferDate,103)=convert(date,'" + txtTfDateReject.Text + "',103) AND DeleteBy IS NULL  ";
            //    //}
            //    //else if (!string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDateReject.Text))
            //    //{
            //    //    Parameter = " where BranchID='" + ddlBranchReject.SelectedValue + "' and convert(date,TransferDate,103)=convert(date,'" + txtTfDateReject.Text + "',103) AND DeleteBy IS NULL  ";
            //    //}
            //}
            //PrintSingleBranchStockReport(Parameter);

            
        }
    }
    //protected void btnRejkectPrint_Click(object sender, EventArgs e)
    //{
    //    String connectionString = DataManager.OraConnString();
        
    //    string Parameter = "";
    //    if (!string.IsNullOrEmpty(lblID.Text))
    //    {
    //        Parameter = " where ID='" + lblIDReject.Text + "' AND DeleteBy IS NULL  ";
    //    }

    //    else
    //    {
    //        //if (!string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && string.IsNullOrEmpty(txtTfDateReject.Text))
    //        //{
    //        //    Parameter = " where BranchID='" + ddlBranchReject.SelectedValue + "' AND DeleteBy IS NULL  ";
    //        //}
    //        //else if (string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDateReject.Text))
    //        //{
    //        //    Parameter = " where convert(date,TransferDate,103)=convert(date,'" + txtTfDateReject.Text + "',103) AND DeleteBy IS NULL  ";
    //        //}
    //        //else if (!string.IsNullOrEmpty(ddlBranchReject.SelectedItem.Text) && !string.IsNullOrEmpty(txtTfDateReject.Text))
    //        //{
    //        //    Parameter = " where BranchID='" + ddlBranchReject.SelectedValue + "' and convert(date,TransferDate,103)=convert(date,'" + txtTfDateReject.Text + "',103) AND DeleteBy IS NULL  ";
    //        //}
    //    }
    //    DataTable dtt = IdManager.GetShowDataTable("select " + lblPrintFlagReject.Text + " ID,BranchID,TransferDate,ChallanNo,Code from ItemStockTransferMst " + Parameter + " order by ID desc");
    //    if (dtt != null)
    //    {
    //        if (dtt.Rows.Count > 0)
    //        {
    //            DataTable dt = aclsItemTransferStockManager.getShowTransferDetailsOnReport(dtt.Rows[0]["ID"].ToString(), connectionString);

    //            if (dtt != null)
    //            {
    //                if (dtt.Rows.Count > 0)
    //                {
    //                    decimal TotalQuantity = 0;
    //                    decimal TotalAmount = 0;
    //                    foreach (DataRow dr in dt.Rows)
    //                    {
    //                        TotalQuantity += Convert.ToDecimal(dr["Qty"].ToString());
    //                        TotalAmount += Convert.ToDecimal(dr["UnitPrice"].ToString()) * Convert.ToDecimal(dr["Qty"].ToString());

    //                    }
    //                    DataTable dt2 = IdManager.GetShowDataTable("Select   * from dbo.GL_SET_OF_BOOKS where BOOK_NAME='AMB'");

    //                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
    //                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("Report/rptStockTransferBill_Challan.rdlc");
    //                    ReportViewer1.LocalReport.DataSources.Clear();
    //                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("ReportType", "Purchase"));
    //                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("ReportDate", DateTime.Now.ToString("dd/MM/yyyy")));
    //                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("TotalQuantity", TotalQuantity.ToString("N0")));
    //                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("TotalAmount", TotalAmount.ToString("N2")));

    //                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("VatRate", "Vat-" + Convert.ToDecimal(dt.Rows[0]["VatRate"]).ToString("N1") + "%"));
    //                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("BranchName", dt.Rows[0]["BranchName"].ToString()));
    //                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("Address", dt.Rows[0]["Address1"].ToString()));
    //                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("PhoneNo", dt.Rows[0]["Phone"].ToString()));
    //                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("Remarks", dt.Rows[0]["Remark"].ToString()));
    //                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("ChallanNo", dtt.Rows[0]["ChallanNo"].ToString()));
    //                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("TransferCode", dtt.Rows[0]["Code"].ToString()));
    //                    ReportViewer1.LocalReport.SetParameters(new ReportParameter("TransferDate", dt.Rows[0]["TransferDate"].ToString()));

    //                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("itemstockTransferReport", dt));
    //                    ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Company", dt2));
    //                    ReportViewer1.LocalReport.Refresh();

    //                }
    //                else
    //                {
    //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "alert('Empty Data..!!');", true);

    //                    ReportViewer1.Reset();
    //                    return;
    //                }
    //            }
    //            else
    //            {
    //                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "alert('Empty Data..!!');", true);

    //                ReportViewer1.Reset();
    //                return;
    //            }

    //        }
    //        else
    //        {
    //            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "alert('Empty Data..!!');", true);

    //            ReportViewer1.Reset();
    //            return;
    //        }
                

           
    //    }

    //    ModalPopupExtenderLogin3.Show();
    //}


 
}