using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Delve;
using sales;

public partial class ClientRef : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DataTable dt = clsClientInfoManager.GetClientInfos();
        dgQuot.DataSource = dt;
        dgQuot.DataBind();
    }
    protected void dgQuot_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {

    }
    protected void dgQuot_SelectedIndexChanged(object sender, EventArgs e)
    {
        string refno;
        string Address;
        refno = dgQuot.SelectedRow.Cells[1].Text.Trim() + '-' + dgQuot.SelectedRow.Cells[2].Text.Trim();
        Address = dgQuot.SelectedRow.Cells[3].Text.Trim();
        ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script language=JavaScript>SubmitToParent('" + refno + "','" + Address + "');</script>");


    }
}