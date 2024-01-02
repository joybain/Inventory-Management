using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Delve;

public partial class GlFinYearPopup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            LoadGrid();
            LoadGridBind();
        }
    }
    public void LoadGrid()
    {
        DataTable dt=FinYearManager.GetOpenFinMonths();
        dgFinMonth.DataSource = dt;
    }
    public void LoadGridBind()
    {
        dgFinMonth.DataBind();
    }
    protected void dgFinMonth_SelectedIndexChanged(object sender, EventArgs e)
    {
        string finmon = dgFinMonth.SelectedRow.Cells[2].Text.Trim();
        
        StringBuilder script = new StringBuilder("");
        ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script language=JavaScript>SubmitToParent('" + finmon + "');</script>");
    }
}
