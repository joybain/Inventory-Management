using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;

public partial class frmBtoBTransfer : System.Web.UI.Page
{
    private BtoBManager _btobManager = new BtoBManager();
    private clsItemTransferStockManager aclsItemTransferStockManager = new clsItemTransferStockManager();
    private ItemManager _aItemManager = new ItemManager();
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
                            //Session["dept"] = dReader["dept"].ToString();
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
            Response.Redirect("Home.aspx?sid=sam");

        }
        if (!IsPostBack)
        {



            Session["purdtl"] = null;
            BtnSave.Enabled = true;
            //DataTable dt = aclsItemTransferStockManager.GetBranchInfo("");
            //if (dt.Rows.Count > 0)
            //{
            //    dgHistory.DataSource = dt;
            //    ViewState["History"] = dt;
            //    dgHistory.DataBind();

            //}

            DataTable dtBranch = ClsItemDetailsManager.GetBranchInfo();
            util.PopulationDropDownList(ddlFromBranch, "Name", "ID", dtBranch);

            util.PopulationDropDownList(ddlToBranch, "Name", "ID", dtBranch);

            //TabContainer1.ActiveTabIndex = 2;
            txtRemark.Text = lblID.Text = "";
            BtnSave.Enabled = true;
            Refresh();
            //DropDownListValue();
            //RefreshReject();
            //getEmptyDtlReject();
            txtTfDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            Up1.Update();
        }
    }


    private void DropDownListValue()
    {
        DataTable dt =
            IdManager.GetShowDataTable(
                "select ID,BranchName from BranchInfo where Flag IS NULL and MainBranch!=1 or MainBranch is null and DeleteBy is null and [Status]=1");
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

    private void Refresh()
    {
        DataTable dt = _btobManager.GetBranchInfo("");
        if (dt.Rows.Count > 0)
        {
            dgHistory.DataSource = dt;
            ViewState["History"] = dt;
            dgHistory.DataBind();

        }
        txtSearchBydateCode.Visible = false;
        txtRemark.Text = lblID.Text = "";
        lblPrintFlag.Text = "Top(1)";
        //txtSearchCarton.Text = "";
        txtTfDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        ItemsDetails.Visible = pnl.Enabled = true;
        ddlBranchSearch.SelectedIndex = -1;
        txtRemark.Text = lblID.Text = "";
        dgPODetailsDtl.DataSource = null;
        dgPODetailsDtl.DataBind();
        dgPODetailsDtl.Enabled = true;
        //dgTransferHistory.Visible = false;
        //pnlItemDtl.Visible = true;
        Session["purdtl"] = null;
        //getEmptyDtl();
        BtnSave.Enabled = true;
        // txtChallanNo.Text = "";
        // btnNew.Visible = false;
        //txtChallanNo.Text = IdManager.GetDateTimeWiseSerial("CF", "ID", "dbo.ItemStockTransferMst");
        txtTransferCode.Text = "BTB-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +
                               DateTime.Now.ToString("dd") + "00" +
                               IdManager.GetShowSingleValueInt("TransferID", "ID", "FixValue", "1").ToString();
        DataTable dtBranch = ClsItemDetailsManager.GetBranchInfo();
        util.PopulationDropDownList(ddlFromBranch, "Name", "ID", dtBranch);
        util.PopulationDropDownList(ddlToBranch, "Name", "ID", dtBranch);
        util.PopulationDropDownList(ddlBranchSearch, "Name", "ID", dtBranch);


    }

    private void getEmptyDtl()
    {
        dgPODetailsDtl.Visible = true;
        DataTable dtDtlGrid = new DataTable();
        dtDtlGrid.Columns.Add("ItemsID", typeof (string));
        dtDtlGrid.Columns.Add("ItemsName", typeof (string));
        dtDtlGrid.Columns.Add("item_code", typeof (string));
        dtDtlGrid.Columns.Add("Code", typeof (string));
        dtDtlGrid.Columns.Add("Barcode", typeof (string));
        dtDtlGrid.Columns.Add("item_desc", typeof (string));
        dtDtlGrid.Columns.Add("StockQty", typeof (string));
        dtDtlGrid.Columns.Add("TransferQty", typeof (string));
        dtDtlGrid.Columns.Add("Price", typeof (string));
        dtDtlGrid.Columns.Add("Catagory", typeof (string));
        dtDtlGrid.Columns.Add("SubCatagory", typeof (string));
        dtDtlGrid.Columns.Add("UMO", typeof (string));
        dtDtlGrid.Columns.Add("StyleNo", typeof (string));
        dtDtlGrid.Columns.Add("Discount", typeof (string));
        dtDtlGrid.Columns.Add("CartonNo", typeof (string));
        dtDtlGrid.Columns.Add("ReceivedQuantity", typeof (string));
        dtDtlGrid.Columns.Add("BranchSalesPrice", typeof (Decimal));
        DataRow dr = dtDtlGrid.NewRow();
        dtDtlGrid.Rows.Add(dr);
        dgPODetailsDtl.DataSource = dtDtlGrid;
        Session["purdtl"] = dtDtlGrid;

        dgPODetailsDtl.DataBind();
    }

    protected void ddlFromBranch_SelectedIndexChanged(object sender, EventArgs e)
    {
        var BranchId = 0;
        if (string.IsNullOrEmpty(ddlFromBranch.SelectedValue))
        {
            Session["branchId"] = BranchId;
        }
        else
        {
            Session["branchId"] = ddlFromBranch.SelectedValue;
        }
        getEmptyDtl();


    }

    private void ShowFooterTotal()
    {
        try
        {
            decimal ctot = decimal.Zero;
            decimal TransferPrice = 0;
            decimal totSalPrice = 0;
            decimal TotalFromBranchQty = 0;
            decimal TotalToBranchQty = 0;
            decimal totA = 0;
            decimal Total = 0;

            if (Session["purdtl"] != null)
            {
                DataTable dt = (DataTable) Session["purdtl"];
                foreach (DataRow drp in dt.Rows)
                {
                    if (drp["ItemsID"].ToString() != "" && drp["Price"].ToString() != "")
                    {
                        totSalPrice += decimal.Parse(drp["BranchSalesPrice"].ToString());
                        TransferPrice += decimal.Parse(drp["Price"].ToString());
                        TotalFromBranchQty += decimal.Parse(drp["StockQty"].ToString());
                        TotalToBranchQty += decimal.Parse(drp["TransferQty"].ToString());
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

            cell.Text = TotalFromBranchQty.ToString("N2");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);

            cell = new TableCell();
            // cell.ColumnSpan = 2;
            cell.Text = TransferPrice.ToString("N0");
            //cell.ColumnSpan = 1;
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);

            cell = new TableCell();

            cell.Text = TotalToBranchQty.ToString("N2");
            cell.HorizontalAlign = HorizontalAlign.Right;
            row.Cells.Add(cell);

            cell = new TableCell();
            // cell.ColumnSpan = 2;
            cell.Text = totSalPrice.ToString("N0");
            //cell.ColumnSpan = 1;
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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                "jsAlert('" + fex.Message + "','red',0);", true);

        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                    "jsAlert('Database Maintain Error. Contact to the Software Provider..!!','red',0);", true);
            else ;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
        }

    }

    protected void txtItemName_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow) ((TextBox) sender).NamingContainer;
        DataTable dtdtl = (DataTable) Session["purdtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        string CartonNo = "";
        string[] words = ((TextBox) gvr.FindControl("txtItemName")).Text.Split('-');
        DataTable dt =
            _aItemManager.GetSearchItemsOnStockBranchWish(((TextBox) gvr.FindControl("txtItemName")).Text.ToUpper(),
                words.Length, ddlFromBranch.SelectedValue);


        if (dt.Rows.Count > 0)
        {
            bool IsCheck = false;
            foreach (DataRow ddr in dtdtl.Rows)
            {
                var a = dr["ItemsName"].ToString();
                if (string.IsNullOrEmpty(dr["ItemsName"].ToString()))
                {
                    //if (ddr["ItemsID"].ToString().Equals(((DataRow)dt.Rows[0])["ItemID"].ToString()))
                    if (ddr["Barcode"].ToString().Equals(((DataRow) dt.Rows[0])["Barcode"].ToString()))
                    {
                        IsCheck = true;
                        if (Convert.ToDouble(dt.Rows[0]["TotalClosingStock"].ToString()) >
                            Convert.ToDouble(ddr["TransferQty"].ToString()))
                        {
                            ddr["TransferQty"] = Convert.ToDouble(ddr["TransferQty"].ToString()) + 1;
                            dgPODetailsDtl.DataSource = dtdtl;
                            //Session["purdtl"] = dtdtl;
                            dgPODetailsDtl.DataBind();
                            ShowFooterTotal();
                            ((TextBox) dgPODetailsDtl.Rows[gvr.DataItemIndex].FindControl("txtItemName")).Text = "";
                            ((TextBox) dgPODetailsDtl.Rows[gvr.DataItemIndex].FindControl("txtItemName")).Focus();
                            return;

                        }

                        else
                        {
                            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                                "alert('Transfer Stock Upper then Stock Quantity.!!');", true);
                            ((TextBox) dgPODetailsDtl.Rows[gvr.DataItemIndex].FindControl("txtItemName")).Text = "";
                            ((TextBox) dgPODetailsDtl.Rows[gvr.DataItemIndex].FindControl("txtItemName")).Focus();
                            return;

                        }
                    }
                }
            }


            dr["ItemsID"] = ((DataRow) dt.Rows[0])["ItemID"].ToString();
            dr["item_code"] = ((DataRow) dt.Rows[0])["Code"].ToString();
            dr["Barcode"] = ((DataRow) dt.Rows[0])["Barcode"].ToString();
            dr["ItemsName"] = ((DataRow) dt.Rows[0])["txtItems"].ToString();
            //dr["Type"] = ((DataRow)dt.Rows[0])["Type"].ToString();
            dr["StockQty"] = ((DataRow) dt.Rows[0])["TotalClosingStock"].ToString();
            dr["Price"] = ((DataRow) dt.Rows[0])["CostPrice"].ToString();
            //dr["TransferQty"] = "1";
            dr["TransferQty"] = Convert.ToInt32(((DataRow) dt.Rows[0])["TotalClosingStock"]).ToString();
            dr["Discount"] = "0";
            //dr["Catagory"] = ((DataRow)dt.Rows[0])["Category"].ToString();
            //dr["SubCatagory"] = ((DataRow)dt.Rows[0])["SubCategory"].ToString();
            dr["UMO"] = ((DataRow) dt.Rows[0])["UMO"].ToString();
            dr["StyleNo"] = ((DataRow) dt.Rows[0])["StyleNo"].ToString();
            dr["BranchSalesPrice"] = ((DataRow) dt.Rows[0])["SPrice"].ToString();
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


            ((TextBox) dgPODetailsDtl.Rows[gvr.DataItemIndex + 1].FindControl("txtItemName")).Focus();

            Up1.Update();
        }
    }

    protected void dgPODetailsDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Header |
            e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[2].Attributes.Add("style", "display:none");
            //e.Row.Cells[6].Attributes.Add("style", "display:none");
            //e.Row.Cells[7].Attributes.Add("style", "display:none");
            //e.Row.Cells[8].Attributes.Add("style", "display:none");
            //e.Row.Cells[9].Attributes.Add("style", "display:none");
            e.Row.Cells[8].Attributes.Add("style", "display:none");
        }
    }

    protected void txtTransferQuantity_TextChanged(object sender, EventArgs e)
    {
        GridViewRow gvr = (GridViewRow) ((TextBox) sender).NamingContainer;
        DataTable dtdtl = (DataTable) Session["purdtl"];
        DataRow dr = dtdtl.Rows[gvr.DataItemIndex];
        clsItemTransferStock aclsItemTransferStock = aclsItemTransferStockManager.GetStockTransferInfo(lblID.Text);
        if (!string.IsNullOrEmpty(((TextBox) gvr.FindControl("txtTransferQuantity")).Text))
        {
            if (aclsItemTransferStock != null)
            {
                if ((Convert.ToDouble(dr["StockQty"]) + Convert.ToDouble(dr["TransferQty"])) >=
                    Convert.ToDouble(((TextBox) gvr.FindControl("txtTransferQuantity")).Text))
                {
                    dr["TransferQty"] = ((TextBox) gvr.FindControl("txtTransferQuantity")).Text;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                        "jsAlert('Transfer Stock Upper then Stock Quantity.!!','orange',2);", true);
                    ((TextBox) gvr.FindControl("txtTransferQuantity")).Text = dr["TransferQty"].ToString();
                    ((TextBox) dgPODetailsDtl.Rows[gvr.DataItemIndex].FindControl("txtTransferQuantity")).Focus();
                    return;
                }
            }
            else
            {
                if (Convert.ToDouble(dr["StockQty"]) >=
                    Convert.ToDouble(((TextBox) gvr.FindControl("txtTransferQuantity")).Text))
                {
                    dr["TransferQty"] = ((TextBox) gvr.FindControl("txtTransferQuantity")).Text;
                }
                else
                {

                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                        "jsAlert('Transfer Stock Upper then Stock Quantity.!!','orange',2);", true);
                    ((TextBox) gvr.FindControl("txtTransferQuantity")).Text = dr["TransferQty"].ToString();
                    //((TextBox)gvr.FindControl("txtTransferQuantity")).Focus();
                    ((TextBox) dgPODetailsDtl.Rows[gvr.DataItemIndex].FindControl("txtTransferQuantity")).Focus();
                    return;
                }
            }
        }
        else
        {
            dr["TransferQty"] = '1';

            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                "jsAlert('This Qty  Is Not valid So 1 Qty is Fixed!!','teal',3);", true);

            //return;
        }

        dgPODetailsDtl.DataSource = dtdtl;
        Session["purdtl"] = dtdtl;
        dgPODetailsDtl.DataBind();
        ShowFooterTotal();
        if (dgPODetailsDtl.Rows.Count != gvr.DataItemIndex + 1)
        {
            //((TextBox)dgPODetailsDtl.Rows[gvr.DataItemIndex+1].FindControl("txtTransferQuantity")).Focus();
            ((TextBox) dgPODetailsDtl.Rows[gvr.DataItemIndex + 1].FindControl("txtItemName")).Focus();


        }
        Up1.Update();
    }

    protected void BtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(ddlFromBranch.SelectedItem.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                    "jsAlert('Select From Branch.!!','teal',3);", true);


                ddlFromBranch.Focus();
                return;
            }
            if (string.IsNullOrEmpty(ddlToBranch.SelectedItem.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                    "jsAlert('Select Transfer Branch.!!','teal',3);", true);


                ddlToBranch.Focus();
                return;
            }
            if ((ddlFromBranch.SelectedItem.Text) == (ddlToBranch.SelectedItem.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                    "jsAlert('Same  Branch To Branch Not Transfer.!!','teal',3);", true);


                ddlToBranch.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtTfDate.Text))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                    "jsAlert('Input Date.!!','teal',3);", true);
                txtTfDate.Focus();
                return;
            }
            DataTable dt = null;
            if (Session["purdtl"] != null)
            {
                dt = (DataTable) Session["purdtl"];
                if (string.IsNullOrEmpty(dt.Rows[0]["ItemsID"].ToString()) || dt.Rows[0]["ItemsID"].ToString() == "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                        "jsAlert('Add this transfer items in list.!!','teal',3);", true);
                    return;
                }
            }


            else
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                    "jsAlert('Add this transfer items in list.!!','teal',3);", true);
                return;
            }

            clsItemTransferStock aclsItemTransferStock = aclsItemTransferStockManager.GetStockTransferInfo(lblID.Text);
            if (lblID.Text != null)
            {
                if (per.AllowEdit == "Y")
                {
                    //int CheckReceived = IdManager.GetShowSingleValueInt("COUNT(*)", "ReceivedBy IS NOT NULL AND ID", "ItemStockTransferMst", lblID.Text);
                    //string CheckExcelFlag = IdManager.GetShowSingleValueString("ExcelUser", "ID", "ItemStockTransferMst", lblID.Text);
                    //if (!string.IsNullOrEmpty(CheckExcelFlag))
                    //{

                    //    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Alrady Generate Excel File.you are not update this.!!','red',2);", true);

                    //    return;
                    //}

                    //aclsItemTransferStock.Code = txtTransferCode.Text.Replace("'", "").Replace(",", "").Trim();
                    //aclsItemTransferStock.TransferDate = txtTfDate.Text.Replace("'", "").Replace(",", "").Trim();
                    //aclsItemTransferStock.BranchId = ddlFromBranch.SelectedValue.Replace("'", "").Replace(",", "").Trim();
                    //aclsItemTransferStock.LoginBy = Session["userID"].ToString().Replace("'", "").Replace(",", "").Trim();
                    ////aclsItemTransferStock.CartonNo = txtSearchCarton.Text.Replace("'", "").Replace(",", "").Trim();
                    ////aclsItemTransferStock.ChallanNo = txtChallanNo.Text.Replace("'", "").Replace(",", "").Trim();
                    //aclsItemTransferStock.TransferType = "F";
                    //aclsItemTransferStock.Remark = txtRemark.Text.Replace("'", "").Replace(",", "").Trim();
                    //string Parameter = "where [MstID]='" + lblID.Text + "' AND t1.DeleteBy IS NULL";
                    //DataTable dtOld = aclsItemTransferStockManager.GetShowItemsDetails(Parameter,"F");
                    //string ITSerial = IdManager.GetShowSingleValueString("VCH_SYS_NO",
                    //   "t1.PAYEE='IT' and SUBSTRING(t1.VCH_REF_NO,1,2)='JV' and t1.SERIAL_NO", "GL_TRANS_MST t1",
                    //   txtTransferCode.Text);
                    ////*************************** Account Entry (Update) ******************//
                    ////********* Jurnal Voucher - 1 *********//
                    //VouchMst vmst = VouchManager.GetVouchMst(ITSerial.Trim());                    
                    //if (vmst != null)
                    //{
                    //    vmst.FinMon = FinYearManager.getFinMonthByDate(txtTfDate.Text);
                    //    vmst.ValueDate = txtTfDate.Text;
                    //    vmst.VchCode = "03";
                    //    // vmst.RefFileNo = "";
                    //    // vmst.VolumeNo = "";
                    //    //vmst.SerialNo = lblID.Text;
                    //    vmst.Particulars = txtRemark.Text.Replace("'", "").Replace(",", "").Trim();
                    //    //vmst.ControlAmt = Convert.ToDouble(txtTotal.Text).ToString().Replace(",", "");
                    //    vmst.UpdateUser = Session["user"].ToString();
                    //    vmst.UpdateDate = DateTime.Parse(DateTime.Now.ToString()).ToString("dd/MM/yyyy");
                    //    vmst.AuthoUserType = Session["userlevel"].ToString();
                    //}

                    // aclsItemTransferStockManager.UpdateBranchInfo(aclsItemTransferStock, dt, dtOld, vmst);
                    Refresh();

                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                        "jsAlert('Record(s) is/are not updated!!','red',0);", true);

                    // BtnSave.Enabled = false;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                        "jsAlert('You have not enough permissoin to update this record!!','red',0);", true);

                }
            }
            else
            {
                if (per.AllowAdd == "Y")
                {
                    aclsItemTransferStock = new clsItemTransferStock();
                    if (string.IsNullOrEmpty(ddlToBranch.SelectedValue))
                    {
                        BranchToId.Value = "0";
                    }
                    else
                    {
                        BranchToId.Value = ddlToBranch.SelectedValue;
                    }

                    aclsItemTransferStock.TransferDate = txtTfDate.Text.Replace("'", "").Replace(",", "").Trim();
                    aclsItemTransferStock.BranchId =
                        ddlFromBranch.SelectedValue.Replace("'", "").Replace(",", "").Trim();
                    try
                    {
                        var a = Session["userID"].ToString().Replace("'", "").Replace(",", "").Trim();

                    }
                    catch
                    {

                        Response.Redirect("Home.aspx?sid=sam");
                    }

                    aclsItemTransferStock.LoginBy =
                        Session["userID"].ToString().Replace("'", "").Replace(",", "").Trim();
                    if (string.IsNullOrEmpty(txtRemark.Text))
                    {
                        txtRemark.Text = "Transfer To " +
                                         ddlToBranch.SelectedItem.Text.Replace("'", "").Replace(",", "").Trim() +
                                         " From " +
                                         ddlFromBranch.SelectedItem.Text.Replace("'", "").Replace(",", "").Trim() +
                                         ". Transfer By : " + Session["user"].ToString() +
                                         " . Transfer Date :" + DateTime.Now.ToString("dd-MMM-yyyy") + ".";
                    }

                    aclsItemTransferStock.Remark = txtRemark.Text.Replace("'", "").Replace(",", "").Trim();
                    // aclsItemTransferStock.ChallanNo = txtChallanNo.Text.Replace("'", "").Replace(",", "").Trim();
                    aclsItemTransferStock.TransferType = "F";
                    txtTransferCode.Text = "BTOB-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +
                                           DateTime.Now.ToString("dd") + "00" +
                                           IdManager.GetShowSingleValueInt("TransferID", "ID", "FixValue", "1")
                                               .ToString();
                    aclsItemTransferStock.Code = txtTransferCode.Text.Replace("'", "").Replace(",", "").Trim();
                    //aclsItemTransferStock.CartonNo = txtSearchCarton.Text.Replace("'", "").Replace(",", "").Trim();

                    VouchMst vmst = new VouchMst();
                    vmst.FinMon =
                        FinYearManager.getFinMonthByDate(txtTfDate.Text.Replace("'", "").Replace(",", "").Trim());
                    vmst.ValueDate = txtTfDate.Text.Replace("'", "").Replace(",", "").Trim();
                    vmst.VchCode = "03";
                    vmst.RefFileNo = "";
                    vmst.VolumeNo = "";
                    vmst.SerialNo = txtTransferCode.Text.Replace("'", "").Replace(",", "").Trim();
                    vmst.Particulars = "Transfer To Branch " +
                                       ddlToBranch.SelectedItem.Text.Replace("'", "").Replace(",", "").Trim();
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
                    int count = _btobManager.SaveInformation(aclsItemTransferStock, dt, vmst, vmstPayment,
                        BranchToId.Value);
                    if (count == 1)
                    {

                        // ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "jsAlert('Record(s) is/are Save suceessfullly!!','green',1);", true);
                        BtnSave.Enabled = false;
                        Refresh();
                        //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are Save suceessfullly!!','green',1);", true);

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                            "jsAlert('Record(s) is/are Save suceessfullly!!','green',1);", true);
                    }

                    else
                    {

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                            "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);

                        return;
                    }

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                        "jsAlert('You have not enough permissoin to update this record!!','orange',2);", true);
                }
            }
        }
        catch (FormatException fex)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                "jsAlert('" + fex.Message + "','red',0);", true);

        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                    "jsAlert('Database Maintain Error. Contact to the Software Provider..!!','red',0);", true);
            else ;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
        }
    }

    protected void dgHistory_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            lblID.Text = dgHistory.SelectedRow.Cells[1].Text.Trim();
            DataTable dt = _btobManager.GetStockTransferInfo(lblID.Text);


            dgPODetailsDtl.Enabled = true;

            txtTransferCode.Text = dt.Rows[0]["Code"].ToString();
            // lblID.Text = dgHistory.SelectedRow.Cells[1].Text.Trim();
            txtTfDate.Text = dt.Rows[0]["TransferDate"].ToString();
            ddlFromBranch.SelectedValue = dt.Rows[0]["from_Branch"].ToString();
            ddlToBranch.SelectedValue = dt.Rows[0]["to_Branch"].ToString();
            //txtChallanNo.Text = aclsItemTransferStock.ChallanNo;
            txtRemark.Text = dt.Rows[0]["Remark"].ToString();
            string Parameter = "where t1.[MstID]='" + lblID.Text + "'";
            DataTable dtOld = _btobManager.GetShowItemsDetails(Parameter);
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
            TabContainer2.ActiveTabIndex = 0;





        }
        catch (FormatException fex)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                "jsAlert('" + fex.Message + "','red',0);", true);

        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Database"))
                ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                    "jsAlert('Database Maintain Error. Contact to the Software Provider..!!','red',0);", true);
            else ;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert",
                "jsAlert('There is some problem to do the task. Try again properly.!!','red',0);", true);
        }
    }

    protected void dgHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgHistory.DataSource = ViewState["History"];
        dgHistory.PageIndex = e.NewPageIndex;
        dgHistory.DataBind();
    }

    protected void dgHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header | e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");

            // e.Row.Cells[8].Attributes.Add("style", "display:none");
        }
        else if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");

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

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DataTable dt = _btobManager.GetBranchInfobranch(ddlBranchSearch.SelectedValue);
        if (dt.Rows.Count > 0)
        {
            dgHistory.DataSource = dt;
            // ViewState["History"] = dt;
            dgHistory.DataBind();

        }
    }

   
    protected void btnClearSearch_Click(object sender, EventArgs e)
    {
        txtSearchBydateCode.Text = "";
        ddlBranchSearch.SelectedIndex = -1;
        DataTable dt = _btobManager.GetBranchInfo("");
        if (dt.Rows.Count > 0)
        {
            dgHistory.DataSource = dt;
            ViewState["History"] = dt;
            dgHistory.DataBind();
        }
    }
}