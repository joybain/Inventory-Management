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

public partial class GetCoa : System.Web.UI.Page
{
    
    private DataTable dtTable;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        LoadGrid();
    }
    private void LoadGrid()
    {
        dtTable = GlCoaManager.GetGlCoaCodes();
        dgGlCoa.DataSource = dtTable;
        dgGlCoa.DataBind();
    }
    protected void dgGlCoa_SelectedIndexChanged(object sender, EventArgs e)
    {
        string glcode;
        string glname;

        glcode = dgGlCoa.SelectedRow.Cells[1].Text.Trim();
        glname = dgGlCoa.SelectedRow.Cells[2].Text.Trim();
        StringBuilder script = new StringBuilder("");
        ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script language=JavaScript>SubmitToParent('" + glcode + "','" + glname + "');</script>");


    }

}
