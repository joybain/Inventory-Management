using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;


public partial class ItemsPurchaseHistory : System.Web.UI.Page
{
    public readonly PurchaseVoucherManager _aPurchaseVoucherManager = new PurchaseVoucherManager();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            divHitory.Visible = false;
            txtStartDateDate.Text = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            txtEndDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {

        //(hfSupplierID.Value, "GRN",txtStartDate.Text, txtEndDate.Text, "Type", "TypeDtl");
        DataTable dtPurchaseHistory = _aPurchaseVoucherManager.GetShowPurchaseHistoryDtl(hfSupplierID.Value, "",
            txtStartDateDate.Text, txtEndDate.Text, "1");
        dtPurchaseHistory.Columns.Add("dtDeptType", typeof(DataTable));
        if (dtPurchaseHistory.Rows.Count > 0)
        {
            foreach (DataRow drRow in dtPurchaseHistory.Rows)
            {
                DataTable dtPurchaseDtl = _aPurchaseVoucherManager.GetShowPurchaseHistoryDtl(hfSupplierID.Value,
                    drRow["ID"].ToString(),
                    txtStartDateDate.Text, txtEndDate.Text, "2");

                dtPurchaseDtl.TableName = "dtDeptType";
                drRow["dtDeptType"] = dtPurchaseDtl;
            }

            dgDepartment.DataSource = dtPurchaseHistory;
            dgDepartment.DataBind();
            divHitory.Visible = true;
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('No Items Found .!!');", true);
        }

    }

    protected void txtSupplierSearch_TextChanged(object sender, EventArgs e)
    {
        DataTable dtSupplier = PurchaseVoucherManager.GetSupplierInfo(txtSupplierSearch.Text);
        if (dtSupplier.Rows.Count > 0)
        {
            Session["Supplier_COA"] = dtSupplier.Rows[0]["Gl_CoaCode"].ToString();
            txtSupplierSearch.Text = dtSupplier.Rows[0]["ContactName"].ToString();
            //  lblPhoneNo.Text = dtSupplier.Rows[0]["Phone"].ToString();
            hfSupplierID.Value = dtSupplier.Rows[0]["ID"].ToString();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Not Found Supplier.!!');", true);
            // txtSupplier.Text = hfSupplierID.Value = "";
            txtSupplierSearch.Focus();
        }
    }

    protected void txtStartDateDate_TextChanged(object sender, EventArgs e)
    {
        if (txtEndDate.Text != "")
        {
            DateTime dtStart = DateTime.ParseExact(txtStartDateDate.Text.Trim(), "dd/MM/yyyy",
                CultureInfo.InvariantCulture);
            DateTime dtEnd = DateTime.ParseExact(txtEndDate.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (dtStart > dtEnd)
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                    "alert('Start Date can not be longer than End Date ..!!!');", true);
                txtStartDateDate.Text = string.Empty;
            }
        }
    }

    protected void txtEndDate_TextChanged(object sender, EventArgs e)
    {
        if (txtStartDateDate.Text != "")
        {
            DateTime dtStart =
                DateTime.ParseExact(txtStartDateDate.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dtEnd = DateTime.ParseExact(txtEndDate.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (dtStart > dtEnd)
            {
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert",
                    "alert('End Date can not be Lower than Start Date..!!!');", true);
                txtEndDate.Text = string.Empty;
            }
        }
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }

    protected void dgDepartment_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //  e.Row.Cells[5].Attributes.Add("style", "display:none");
            e.Row.Cells[6].Attributes.Add("style", "display:none");
            GridView gv = (GridView) e.Row.FindControl("dgSubMjrCat");
            DataTable dtDeptType = (DataTable) DataBinder.Eval(e.Row.DataItem, "dtDeptType");
            if (dtDeptType.Rows.Count == 0)
            {
                DataRow dr = dtDeptType.NewRow();
                dtDeptType.Rows.Add(dr);
            }

            gv.DataSource = dtDeptType;
            gv.DataBind();
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            // e.Row.Cells[5].Attributes.Add("style", "display:none");
            e.Row.Cells[6].Attributes.Add("style", "display:none");
        }
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            //e.Row.Cells[5].Attributes.Add("style", "display:none");
            e.Row.Cells[6].Attributes.Add("style", "display:none");
        }
    }

    protected void dgDepartment_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "Select")
        {
            GridViewRow gvr = (GridViewRow) (((LinkButton) e.CommandSource).NamingContainer);
            gvr.Cells[2].BackColor =
                gvr.Cells[3].BackColor = gvr.Cells[0].BackColor = gvr.Cells[1].BackColor = Color.Bisque;
            string GRN = gvr.Cells[2].Text.ToString().Trim();
            if (!string.IsNullOrEmpty(GRN))
            {
                Session["GRNPV"] = GRN;
                //Response.Write("<script>");
                //Response.Write("window.open('PurchaseReturn.aspx?mno=5.17','_blank')");
                //Response.Write("</script>");
                string strJS = ("<script type='text/javascript'>window.open('PurchaseReturn.aspx?mno=5.16','_blank');</script>");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "strJSAlert", strJS);
            }
        }

    }
}