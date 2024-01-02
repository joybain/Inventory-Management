using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Delve;
using OldColor;


public partial class frmBankSetupInfo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            pnlBank.Visible = false;
            pnlBranch.Visible = false;
            dgBank.Visible = false;
            dgBranch.Visible = false;
            dgBank.DataSource = clsBankManager.getBankMsts();
            dgBank.DataBind();
        }
    }
    private void Refresh()
    {
        txtBankId.Text = txtBankName.Text = txtBranchName.Text = txtAccountNo.Text = txtAddress1.Text = txtPhone1.Text = txtPhone2.Text = string.Empty;
        ddlBankName.SelectedIndex = -1;
        if (TreeView1.SelectedNode != null)
        {
            if (TreeView1.SelectedNode.Value == "BankSetup")
            {
                pnlBank.Visible = true;
                pnlBranch.Visible = false;
                dgBank.Visible = true;
                dgBranch.Visible = false;
                dgBank.DataSource = clsBankManager.getBankMsts();
                dgBank.DataBind();
            }
            else if (TreeView1.SelectedNode.Value == "BranchSetup")
            {
                DataTable dtMst = clsBankManager.getBankDtls("");
                dgBranch.DataSource = dtMst;
                dgBranch.DataBind();

            }
        }
        pnlBank.Visible = false;
        pnlBranch.Visible = false;
        //TreeView1.SelectedNode = -1;
    }
    protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
    {
        try
        {

            if (TreeView1.SelectedNode.Value == "BankSetup")
            {
                Refresh();
                pnlBank.Visible = true;
                pnlBranch.Visible = false;
                dgBank.Visible = true;
                dgBranch.Visible = false;
                DataTable dtFixCode = IdManager.GetShowDataTable("SELECT CashInHand_BD AS [CashParentCode],CashAtBank_BD AS [BankParentCode] FROM [FixGlCoaCode]");
                txtParentCode.Text = dtFixCode.Rows[0]["BankParentCode"].ToString();
            }
            else if (TreeView1.SelectedNode.Value == "BranchSetup")
            {
                Refresh();
                pnlBank.Visible = false;
                pnlBranch.Visible = true;
                dgBank.Visible = false;
                dgBranch.Visible = true;
                DataTable dt = clsBankManager.getBankMsts();
                ddlBankName.DataSource = dt;
                ddlBankName.DataTextField = "bank_name";
                ddlBankName.DataValueField = "bank_id";
                ddlBankName.DataBind();
                ddlBankName.Items.Insert(0, "");
                DataTable dtMst = clsBankManager.getBankDtls("");
                dgBranch.DataSource = dtMst;
                dgBranch.DataBind();
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Please Select Bank OR Brtanch First ....!!');", true);
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

    public void BranchClaesr()
    {

        txtAccountNo.Text =
            txtBranchName.Text =
                txtAddress1.Text =
                    txtAddress2.Text = txtPhone1.Text = txtPhone2.Text = txtBrSegCode.Text = txtBrParentCode.Text = hfBranchID.Value =
                        string.Empty;
        DataTable dtMst = clsBankManager.getBankDtls("");
        dgBranch.DataSource = dtMst;
        dgBranch.DataBind();
    }

    protected void BtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            txtBankCharge.Text = "0.00";
            //*********************** Bank Setup ********//
            if (TreeView1.SelectedNode.Value == "BankSetup")
            {
                if (string.IsNullOrEmpty(txtBankName.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Bank Name!!');", true);
                    txtSegCode.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtSegCode.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Segment Code. !!');", true);
                    txtSegCode.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtParentCode.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Parent Code. !!');", true);
                    txtParentCode.Focus();
                    return;
                }
               
                clsBankMst mst = clsBankManager.getBankMst(txtBankId.Text);
                if (mst != null)
                {
                    mst.SegmentCoaCode = txtSegCode.Text;
                    mst.ParentCode = txtParentCode.Text;
                    mst.BankName = txtBankName.Text;

                    mst.LoginBy = Session["user"].ToString();
                     clsBankManager.UpdateBankMst(mst);

                     Create_Gl_Bank_Coa_Code(txtSegCode.Text, txtParentCode.Text, txtBankName.Text, 2,"R");
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are update successfully..!!');", true);
                    dgBank.DataSource = clsBankManager.getBankMsts();
                    dgBank.DataBind();
                    dgBank.Visible = true;
                    dgBranch.Visible = false;
                    txtBankId.Text = txtBankName.Text = txtSegCode.Text=txtParentCode.Text = "";
                    
                    txtBankName.Focus();
                }
                else
                {

                    int COUNT = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER([bank_name])", "bank_info", txtBankName.Text.ToUpper());
                    if (COUNT > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('this Name already exist on database....!!');", true);
                        txtBankName.Focus();
                        return;
                    }

                    int CheckFlag = IdManager.GetShowSingleValueInt("COUNT(*)", "SEG_COA_CODE", "GL_SEG_COA", txtSegCode.Text);
                    if (CheckFlag > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('This  Sedgment Code Already Exist on database..!!');", true);
                        return;
                    }
                    mst = new clsBankMst();
                    mst.BankId = txtBankId.Text;
                    mst.BankName = txtBankName.Text;
                 
                    mst.SegmentCoaCode = txtSegCode.Text;
                    mst.ParentCode = txtParentCode.Text;
                    mst.LoginBy = Session["user"].ToString();
                    clsBankManager.CreateBankMst(mst);
                    Create_Gl_Bank_Coa_Code(txtSegCode.Text, txtParentCode.Text, txtBankName.Text, 1, "R");
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are saved successfully..!!');", true);
                    dgBank.DataSource = clsBankManager.getBankMsts();
                    dgBank.DataBind();
                    dgBank.Visible = true;
                    dgBranch.Visible = false;
                    txtBankId.Text = txtBankName.Text = txtBrSegCode.Text = "";

                    txtBankName.Focus();
                }
            }
            //*********************** Brunch Setup ********//

            else if (TreeView1.SelectedNode.Value == "BranchSetup")
            {
                if (string.IsNullOrEmpty(txtBranchName.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Branch Name!!');", true);
                    txtBranchName.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtAccountNo.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Account No. !!');", true);
                    txtAccountNo.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtAccountName.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Account Name. !!');", true);
                    txtAccountName.Focus();
                    return;
                }
                //if (string.IsNullOrEmpty(txtBankCharge.Text))
                //{
                //    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Bank Charge.. !!');", true);
                //    txtBankCharge.Focus();
                //    return;
                //}
                if (string.IsNullOrEmpty(txtBrSegCode.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Segment Code. !!');", true);
                    txtBrSegCode.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtBrParentCode.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Parent Code. !!');", true);
                    txtBrParentCode.Focus();
                    return;
                }
               
                clsBankDtl aClsBankDtlObj = clsBankManager.getBranchDtl(hfBranchID.Value);
                if (aClsBankDtlObj != null)
                {

                   

                   int  COUNT = IdManager.GetShowSingleValueInt1("COUNT(*)", "UPPER([Gl_Code])", "BANK_BRANCH", txtBrSegCode.Text.ToUpper(), "ID", Convert.ToInt32(aClsBankDtlObj.ID));
                    if (COUNT > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('this Segment Code already exist on database....!!');", true);
                        txtBrSegCode.Focus();
                        return;
                    }
                    aClsBankDtlObj.BankType = rdbBankType.SelectedValue;
                    aClsBankDtlObj.BankId = ddlBankName.SelectedValue;
                    aClsBankDtlObj.BranchName = txtBranchName.Text;
                    aClsBankDtlObj.AccountName = txtAccountName.Text;
                    aClsBankDtlObj.AccountNo = txtAccountNo.Text;
                    aClsBankDtlObj.BankCharge = txtBankCharge.Text;
                    aClsBankDtlObj.Addr1 = txtAddress1.Text;
                    aClsBankDtlObj.Addr2 = txtAddress2.Text;
                    aClsBankDtlObj.Phone = txtPhone1.Text;
                    aClsBankDtlObj.Phone1 = txtPhone2.Text;
                    aClsBankDtlObj.AccountType = ddlBankAccountType.SelectedItem.Text;
                    aClsBankDtlObj.Status = ddlStatus.SelectedValue;
                    aClsBankDtlObj.Gl_Code = txtBrSegCode.Text;
                    // aClsBankDtlObj.ShoprtName = Session["ShotName"].ToString();
                    aClsBankDtlObj.LoginBy = Session["user"].ToString();

                    string Gl_Seg_Coa = ddlBankName.SelectedItem.Text + " - " +
                                        txtAccountNo.Text;
                    if (!string.IsNullOrEmpty(txtAccountName.Text))
                    {
                        Gl_Seg_Coa = ddlBankName.SelectedItem.Text + " - " + txtAccountName.Text + " - " +
                                     txtAccountNo.Text;
                    }

                    Create_Gl_Bank_Coa_Code(txtBrSegCode.Text, txtBrParentCode.Text,
                        Gl_Seg_Coa, 2, "L");
                    clsBankManager.UpdateBankDtl(aClsBankDtlObj);
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are update successfully..!!');", true);
                    BranchClaesr();
                    ddlBankName.SelectedIndex = -1;
                    ddlBankName.Focus();
                }
                else
                {
                    
                    //int COUNT = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER([AccountNo])", "BANK_BRANCH", txtAccountNo.Text.ToUpper());
                    // if (COUNT > 0)
                    //{
                    //    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('this AccountNo alrady exist on database....!!');", true);
                    //    txtAccountNo.Focus();
                    //    return;
                    //}

                    int CheckCode = IdManager.GetShowSingleValueInt("COUNT(*)", "SEG_COA_CODE", "GL_SEG_COA", txtBrSegCode.Text);
                    if (CheckCode > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('this Segment already exist on database....!!');", true);
                        txtBrSegCode.Focus();
                        return;
                    }
                    
                    aClsBankDtlObj = new clsBankDtl();
                    aClsBankDtlObj.BankType = rdbBankType.SelectedValue;
                    aClsBankDtlObj.BankId = ddlBankName.SelectedValue;
                    aClsBankDtlObj.BranchName = txtBranchName.Text;
                    aClsBankDtlObj.BankCharge = txtBankCharge.Text;
                    aClsBankDtlObj.AccountNo = txtAccountNo.Text;
                    aClsBankDtlObj.Addr1 = txtAddress1.Text;
                    aClsBankDtlObj.AccountName = txtAccountName.Text;
                    aClsBankDtlObj.Addr2 = txtAddress2.Text;
                    aClsBankDtlObj.Phone = txtPhone1.Text;
                    aClsBankDtlObj.AccountType = ddlBankAccountType.SelectedItem.Text;
                    aClsBankDtlObj.Status = ddlStatus.SelectedValue;
                    aClsBankDtlObj.Phone1 = txtPhone2.Text;
                    aClsBankDtlObj.Gl_Code = txtBrSegCode.Text;
                    aClsBankDtlObj.LoginBy = Session["user"].ToString();
                    clsBankManager.CreateBankDtl(aClsBankDtlObj);
                    string Gl_Seg_Coa = ddlBankName.SelectedItem.Text + " - " +
                                        txtAccountNo.Text;
                    if (!string.IsNullOrEmpty(txtAccountName.Text))
                    {
                        Gl_Seg_Coa = ddlBankName.SelectedItem.Text + " - " + txtAccountName.Text + " - " +
                                     txtAccountNo.Text;
                    }

                    Create_Gl_Bank_Coa_Code(txtBrSegCode.Text, txtBrParentCode.Text, Gl_Seg_Coa
                        , 1,
                        "L");
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are saved successfully..!!');", true);
                    BranchClaesr();
                    ddlBankName.SelectedIndex = -1;
                    ddlBankName.Focus();
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
    private void Create_Gl_Bank_Coa_Code(string Gl_Seg_Code, string Gl_Parent_Code, string SegCoaDesc, int flag, string RootLeaf)
    {

        //string dept = SegCoaManager.GetSegCoaDesc(Session["dept"].ToString());
        if (flag == 1)
        {
            SegCoa sg = new SegCoa();
            sg.GlSegCode = Gl_Seg_Code;
            sg.SegCoaDesc = SegCoaDesc;
            sg.LvlCode = "02";
            sg.ParentCode = Gl_Parent_Code;
            sg.BudAllowed = "Y";
            sg.PostAllowed = "N";
            sg.AccType = "A";
            sg.OpenDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
            sg.RootLeaf = RootLeaf;

            sg.Status = "A";
            sg.Taxable = "N";
            sg.BookName = "AMB";
            sg.EntryUser = Session["user"].ToString();
            sg.EntryDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
            sg.AuthoDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
            sg.AuthoUser = "ACC";
            sg.Cash_Bank_Status = "1";
            SegCoaManager.CreateSegCoa(sg, "A");
            if (!RootLeaf.Trim().Equals("R"))
            {
                GlCoa gl = new GlCoa();
                gl.GlCoaCode = "1-" + Gl_Seg_Code;
                gl.CoaEnabled = "Y";
                gl.BudAllowed = "N";
                gl.PostAllowed = "Y";
                gl.Taxable = "N";
                gl.AccType = "A";
                gl.Status = "A";
                gl.BookName = "AMB";
                string NM = IdManager.GetAccCompanyName("");
                gl.CoaDesc = NM + "," + SegCoaDesc;
                gl.CoaCurrBal = "0.00";
                gl.CoaNaturalCode = Gl_Seg_Code;
                gl.LoginBy = Session["user"].ToString();
                GlCoaManager.CreateGlCoa(gl);
            }
        }
        if (flag == 2)
        {

            SegCoa sg = new SegCoa();
            sg.GlSegCode = Gl_Seg_Code;
            sg.SegCoaDesc = SegCoaDesc;
            sg.LvlCode = "02";
            sg.ParentCode = Gl_Parent_Code;
            sg.BudAllowed = "Y";
            sg.PostAllowed = "N";
            sg.AccType = "A";
            sg.OpenDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
            sg.RootLeaf = RootLeaf;

            sg.Status = "A";
            sg.Taxable = "N";
            sg.BookName = "AMB";
            sg.EntryUser = Session["user"].ToString();
            sg.EntryDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
            sg.AuthoDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
            sg.AuthoUser = "ACC";
            SegCoaManager.UpdateSegCoa(sg);

            if (!RootLeaf.Trim().Equals("R"))
            {
                GlCoa gl = new GlCoa();
                gl.GlCoaCode = "1-" + Gl_Seg_Code;
                gl.CoaEnabled = "Y";
                gl.BudAllowed = "N";
                gl.PostAllowed = "Y";
                gl.Taxable = "N";
                gl.AccType = "A";
                gl.Status = "A";
                gl.BookName = "AMB";
                string NM = IdManager.GetAccCompanyName("");
                gl.CoaDesc = NM + "," + SegCoaDesc;
                gl.CoaCurrBal = "0.00";
                gl.CoaNaturalCode = Gl_Seg_Code;
                gl.LoginBy = Session["user"].ToString();
                GlCoaManager.UpdateGlCoa(gl);
            }
        }
    }
    protected void BtnDelete_Click(object sender, EventArgs e)
    {
        if (TreeView1.SelectedNode.Value == "BankSetup")
        {
            clsBankMst mst = clsBankManager.getBankMst(txtBankId.Text);
            int count =IdManager.GetShowSingleValueInt("Count(*) "," [dbo].[bank_branch] where  [Bank_Id] ='"+mst.BankId+"' and [DeleteBy] is null");
            if (count > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('This Bank has a Already Branchs on DataBase..!!');", true);
                return;
            }
            if (mst != null)
            {
                clsBankManager.DeleteBankMst(mst);
            }
        }
        else if (TreeView1.SelectedNode.Value == "BranchSetup")
        {
            clsBankDtl aClsBankDtlObj = clsBankManager.getBranchDtl(hfBranchID.Value);
            if (aClsBankDtlObj != null)
            {
                aClsBankDtlObj.LoginBy = Session["user"].ToString();
                clsBankManager.DeleteBankDtl(aClsBankDtlObj);
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Delete Succesfully..!!');", true);
                
            }
        }
    }
    protected void BtnReset_Click(object sender, EventArgs e)
    {
        
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void ddlBankName_SelectedIndexChanged(object sender, EventArgs e)
    {
        BranchClaesr();
        txtBrParentCode.Text =
            IdManager.GetShowSingleValueInt("isnull(Gl_Code,'0')", "BANK_ID", "BANK_INFO", ddlBankName.SelectedValue).ToString();
        DataTable dtMst = clsBankManager.getBankDtls(ddlBankName.SelectedValue);
        dgBranch.DataSource = dtMst;
        dgBranch.DataBind();


    }

    //***************** Dg Bank Information **********//

    protected void dgBank_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtBankId.Text = dgBank.SelectedRow.Cells[1].Text;
        txtBankName.Text = dgBank.SelectedRow.Cells[2].Text;
        txtSegCode.Text = dgBank.SelectedRow.Cells[3].Text;
        txtParentCode.Text = dgBank.SelectedRow.Cells[4].Text;
       
    }
    protected void dgBank_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");
            e.Row.Cells[5].Attributes.Add("style", "display:none");
        }
    }

    //***************** Dg Branch Information **********//

    protected void dgBranch_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");
           
        }
    }
    protected void dgBranch_SelectedIndexChanged(object sender, EventArgs e)
    {
        hfBranchID.Value = dgBranch.SelectedRow.Cells[1].Text;
        clsBankDtl aClsBankDtlObj = clsBankManager.getBranchDtl(hfBranchID.Value);
        if (aClsBankDtlObj != null)
        {
            ddlBankName.SelectedValue = aClsBankDtlObj.BankId;
            txtBrParentCode.Text = IdManager.GetShowSingleValueInt("Gl_Code", "BANK_ID", "BANK_INFO", ddlBankName.SelectedValue).ToString();
            txtBranchName.Text = aClsBankDtlObj.BranchName;
            rdbBankType.SelectedValue = aClsBankDtlObj.BankType;
            txtAccountNo.Text = aClsBankDtlObj.AccountNo;
            txtAddress1.Text = aClsBankDtlObj.Addr1;
            txtAddress2.Text = aClsBankDtlObj.Addr2;
            txtPhone1.Text = aClsBankDtlObj.Phone;
            txtPhone2.Text = aClsBankDtlObj.Phone1;
            txtBankCharge.Text = aClsBankDtlObj.BankCharge;
            txtBrSegCode.Text = aClsBankDtlObj.Gl_Code;
            txtBrSegCode.Text = aClsBankDtlObj.Gl_Code;
            txtAccountName.Text = aClsBankDtlObj.AccountName;
            ddlBankAccountType.SelectedItem.Text = aClsBankDtlObj.AccountType;
            if (aClsBankDtlObj.Status == "True")
            {
                ddlStatus.SelectedValue = "1";
            }

            else
            {
                ddlStatus.SelectedValue = "0";
            }

            rdbBankType.SelectedValue = aClsBankDtlObj.BankType;
        }
    }
    protected void txtBrParentCode_TextChanged(object sender, EventArgs e)
    {

    }


    protected void dgBank_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        dgBank.DataSource = clsBankManager.getBankMsts();
        dgBank.PageIndex = e.NewPageIndex;
        dgBank.DataBind();
    }
}