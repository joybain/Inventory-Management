using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;

public partial class frmBranchExpenses : System.Web.UI.Page
{
    private static Permis per;
    VouchManager _aVouchManager = new VouchManager();
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
                    RefreshAll();
                    DataTable dtFixCode = _aVouchManager.getFixCode();
                    ViewState["dtFixCode"] = dtFixCode;
                }
            }
            catch 
            {
                Response.Redirect("Default.aspx?sid=sam");
            }
          
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            string IdGlCoa = "";
            DataTable dtFixCode = (DataTable)ViewState["dtFixCode"];
            int count = IdManager.GetShowSingleValueIntTwo("COUNT(*)", "ID","BranchId", "ExpenseHead", txtColorID.Text,Session["BranchId"].ToString());
            if (count > 0)
            {
                if (txtColorName.Text == "")
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input Expense.!!');", true);
                    return;
                }
                int countN = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER(Name)", "ExpenseHead", txtColorName.Text.ToUpper());
                if (countN > 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('this Expense already saved ..!!');", true);
                    return;
                }
                string NM = IdManager.GetAccCompanyName("");
                string CoaDesc = NM + "," + txtColorName.Text + " Expense";

                ExpenseHeadManager.UpdateColorInfo(txtColorID.Text, txtColorName.Text, Session["user"].ToString(), CoaDesc, txtGlCoaCode.Text);
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('update expense information..!!');", true);
                RefreshAll();
            }
            else
            {
                if (txtColorName.Text == "")
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('input Expense..!!');", true);
                    return;
                }
                IdGlCoa = IdManager.getAutoIdWithParameter("702", "GL_SEG_COA", "SEG_COA_CODE",
                    "" + dtFixCode.Rows[0]["IndirectExpenses"].ToString() + "", "0000", "4");

                //Gl_COA(IdGlCoa);
                //Coa Set
                SegCoa sg = SegCoaSet(IdGlCoa);
                //SegCoaManager.CreateSegCoa(sg);
                //string dept = SegCoaManager.GetSegCoaDesc(Session["dept"].ToString());
                GlCoa gl = GLCoaSet(IdGlCoa);
                //GlCoaManager.CreateGlCoa(gl);



              //  ExpenseHeadManager.SaveColorInfo(txtColorID.Text, txtColorName.Text, Session["user"].ToString(), gl, sg);
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Save Expense Information..!!');", true);
                RefreshAll();
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
    private GlCoa GLCoaSet(string IdGlCoa)
    {
        GlCoa gl = new GlCoa();
        //gl.GlCoaCode = "1-" + GlCoa;
        gl.GlCoaCode = "1-" + IdGlCoa;
        gl.CoaEnabled = "Y";
        gl.BudAllowed = "N";
        gl.PostAllowed = "Y";
        gl.Taxable = "N";
        gl.AccType = "E";
        gl.Status = "A";
        gl.BookName = "AMB";
        string NM = IdManager.GetAccCompanyName("");
        gl.CoaDesc = NM + "," + txtColorName.Text + " Expense";
        gl.CoaCurrBal = "0.00";
        //gl.CoaNaturalCode = GlCoa;
        gl.CoaNaturalCode = IdGlCoa;
        return gl;
    }

    private SegCoa SegCoaSet(string IdGlCoa)
    {
        DataTable dtFixCode = (DataTable)ViewState["dtFixCode"];
        SegCoa sg = new SegCoa();
        //sg.GlSegCode = GlCoa;
        sg.GlSegCode = IdGlCoa;
        sg.SegCoaDesc = txtColorName.Text + " Expense";
        sg.LvlCode = "02";
        sg.ParentCode = dtFixCode.Rows[0]["IndirectExpenses"].ToString();
        sg.BudAllowed = "Y";
        sg.PostAllowed = "N";
        sg.AccType = "E";
        sg.OpenDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
        sg.RootLeaf = "L";
        sg.Status = "A";
        sg.Taxable = "N";
        sg.BookName = "AMB";
        sg.EntryUser = Session["user"].ToString();
        sg.EntryDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
        sg.AuthoDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
        sg.AuthoUser = "ACC";
        return sg;
    }
    protected void DeleteButton_Click(object sender, EventArgs e)
    {
        if (txtColorID.Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Enter Expense Head Id..!!');", true);
            return;
        }
        if (txtColorName.Text == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('input Expense..!!');", true);
            return;
        }
        ExpenseHeadManager.DeleteColorInfo(txtColorID.Text, txtColorName.Text, txtGlCoaCode.Text);
        ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Delete Expense Information..!!');", true);
        RefreshAll();
    }
    protected void dgColor_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtColorID.Text = dgColor.SelectedRow.Cells[1].Text;
        txtColorName.Text = dgColor.SelectedRow.Cells[2].Text;
        txtGlCoaCode.Text = dgColor.SelectedRow.Cells[3].Text;
    }
    protected void CloseButton_Click(object sender, EventArgs e)
    {
        RefreshAll();
    }

    private void RefreshAll()
    {
        txtColorID.Text = txtColorName.Text = txtGlCoaCode.Text = string.Empty;
        dgColor.DataSource = ExpenseHeadManager.GetBranchExpenseHead(Session["BranchId"].ToString());
        dgColor.DataBind();
        txtColorName.Focus();
    }
    protected void dgColor_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Attributes.Add("style", "display:none");
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
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
}