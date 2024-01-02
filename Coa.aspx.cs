using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Delve;

public partial class Coa : System.Web.UI.Page
{
    private string pageAction;
    private DataTable dtTable;

    protected void Page_Load(object sender, EventArgs e)
    {
        pageAction = "ADD";
        LoadGrid();
    }
    private void LoadGrid()
    {
        dtTable = GlCoaManager.GetGlCoaCodes();
        dgGlCoa.DataSource = dtTable;
        dgGlCoa.DataBind();
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (pageAction == "ADD")
        {
            GlCoa glcoa = new GlCoa();
            glcoa.GlCoaCode = txtGlCoaCode.Text;
            glcoa.CoaEnabled = "Y";
            glcoa.EffectiveFrom = DateTime.Parse("01/01/2009").ToString("dd/MM/yyyy");
            glcoa.EffectiveTo = DateTime.Parse("01/01/2010").ToString("dd/MM/yyyy");
            glcoa.BudAllowed = "Y";
            glcoa.PostAllowed = "Y";
            glcoa.Taxable = "N";
            glcoa.CoaDesc = txtCoaDesc.Text;
            glcoa.CoaCurrBal = "0";
            glcoa.Status = "Y";
            glcoa.BookName = "ACC";
            glcoa.CoaNaturalCode = "100";

            GlCoaManager.CreateGlCoa(glcoa);
            Response.Write("<script languange = javascript>alert ('Saved Successfully !')</script>");

        }
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtGlCoaCode.Text = String.Empty;
        txtCoaEnabled.Text = String.Empty;
        txtEffectiveFrom.Text = String.Empty;
        txtEffectiveTo.Text = String.Empty;
        txtBudAllowed.Text = String.Empty;
        txtPostAllowed.Text = String.Empty;
        txtTaxable.Text = String.Empty;
        txtCoaDesc.Text = String.Empty;
        txtCoaCurrbal.Text = String.Empty;
        txtStatus.Text = "Y";
            

    }
    protected void dgGlCoa_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}
