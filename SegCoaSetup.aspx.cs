using System;
//using System.Activities.Statements;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
//using  Dorjibari;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.draw;
using System.IO;
using Delve;
using sales;
//using cins;

public partial class SegCoaSetup : System.Web.UI.Page
{
    public static Permis per;
    private static DataTable dtSegParent = new DataTable();
    private static DataTable dtSegChild = new DataTable();

    public string imgFolder;
    public string imagesFolder;
    public string appHome;
    public string Accounce;

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
                    cmd.CommandText = "Select user_grp,description from utl_userinfo where upper(user_name)=upper('" + Session["user"].ToString().ToUpper() + "') and status='A'";
                    conn.Open();
                    dReader = cmd.ExecuteReader();
                    string wnot = "";
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            Session["userlevel"] = int.Parse(dReader["user_grp"].ToString()); 
                            //Session["dept"] = dReader["dept"].ToString();
                            wnot = "Welcome Mr. " + dReader["description"].ToString();
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
            Response.Redirect("Default.aspx?sid=sam");
        }       
        txtOpenDate.Attributes.Add("onBlur", "formatdate('" + txtOpenDate.ClientID + "')");
        txtOpenDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        if (!IsPostBack)
        {
            DataTable dtLvl = LvlManager.GetLevels();
            if (dtLvl.Rows.Count > 0)
            {
                dgLevel.DataSource = dtLvl;
                dgLevel.DataBind();
                dgLevel.Caption = "<h3> Step:1 - Level Setup <h3>";
            }
            else
            {
                getMapDtlGrid();
            } 

            dgGlCoaGen.DataSource = LvlManager.GetLevelsGrid();
            dgGlCoaGen.DataBind();
            ddlLvlcode.DataSource = LvlManager.GetLevelCode_Dropdown();
            ddlLvlcode.DataBind();
            Populate();
            TreeView1.CollapseAll();
            lblClient.Visible = false;
            ddlClientName.Visible = false;          
        }
        string path = Request.ApplicationPath; // this gets the application root -- for localhost it's "/ApplicationName", for remote it's likely "/"
        imgFolder = path + "/img/";
        imagesFolder = path + "/images/";
        appHome = path + "/";
        Accounce = path + "/Accounce/";
    }
    protected void PopulateNode(Object sender, TreeNodeEventArgs e)
    {
        
    }
    public void Populate()
    {
        dtSegParent = SegCoaManager.GetSegCoaAll();
        TreeNode newNode;
        foreach (DataRow row in dtSegParent.Rows)
        {
            newNode = new TreeNode();
            newNode.Text = row["seg_coa_code"].ToString() + " - " + row["seg_coa_desc"].ToString();
            newNode.Value = row["seg_coa_code"].ToString();
            //newNode.SelectAction = TreeNodeSelectAction.Expand;
            //node.ChildNodes.Add(newNode);
            TreeView1.Nodes.Add(newNode);

            if (row["rootleaf"].ToString() == "R")
            {
                PopChild(row["seg_coa_code"].ToString(), newNode);
            }
        }
    }
    public void PopChild(string segcode, TreeNode node)
    {
        DataTable dt = SegCoaManager.GetSegCoaChild("parent_code='" + segcode + "' order by convert(numeric,seg_coa_code)");
        TreeNode newNode;
        
        foreach (DataRow dr in dt.Rows)
        {
            newNode = new TreeNode();
            newNode.Text = dr["seg_coa_code"].ToString() + " - " + dr["seg_coa_desc"].ToString();
            newNode.Value = dr["seg_coa_code"].ToString();
            //newNode.SelectAction = TreeNodeSelectAction.Expand;
            node.ChildNodes.Add(newNode);            
            if (dr["rootleaf"].ToString() == "R")
            {
                PopChild(dr["seg_coa_code"].ToString(), newNode);
            }            
        }        
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
       // System.Threading.Thread.Sleep(3000);
        int CheckFlag = IdManager.GetShowSingleValueInt("COUNT(*)", "SEG_COA_CODE", "GL_SEG_COA", txtSegCode.Text);
        if (CheckFlag > 0)
        {
            ModalPopupExtenderLogin.Show();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "LoadModalDiv();", true);
            return;
        }
        if (string.IsNullOrEmpty(txtSegDesc.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                "alert('Input Description..!!');", true);
            return;
        }
        if (string.IsNullOrEmpty(ddlRootLeaf.SelectedItem.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                "alert('Select Root/Leaf..!!');", true);
            return;
        }
        if (string.IsNullOrEmpty(ddlAccType.SelectedItem.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                "alert('Select Account Type!!');", true);
            
        }
        else
        {

            if (txtSegCode.Text != "")
            {
                SegCoa sgcoa = SegCoaManager.getSegCoa(txtSegCode.Text);
                if (sgcoa != null)
                {
                    int count = IdManager.GetShowSingleValueInt1("COUNT(*)", "UPPER([SEG_COA_DESC])", "GL_SEG_COA",
                        txtSegDesc.Text.ToUpper(),"SEG_COA_CODE",Convert.ToInt32(txtSegCode.Text));
                    if (count > 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                            "alert('This Chart of account alrady exist....!!!!');", true);
                        return;
                    }

                    sgcoa.SegCoaDesc = txtSegDesc.Text.Replace(" ", "").Replace("'", "");
                    sgcoa.RootLeaf = ddlRootLeaf.SelectedValue;
                    sgcoa.Taxable = ddlTaxable.SelectedValue;
                    sgcoa.Status = ddlStatus.SelectedValue;
                    sgcoa.PostAllowed = ddlPostAllowed.SelectedValue;
                    sgcoa.ParentCode = txtParentCode.Text.Replace(" ", "").Replace("'", "");
                    sgcoa.OpenDate = txtOpenDate.Text;
                    sgcoa.LvlCode = ddlLvlcode.SelectedValue;
                    sgcoa.BudAllowed = ddlBudAllowed.SelectedValue;
                    sgcoa.AccType = ddlAccType.SelectedValue;
                    sgcoa.ClientName = ddlClientName.SelectedValue;
                    sgcoa.EntryUser = Session["user"].ToString();
                    SegCoaManager.UpdateSegCoa(sgcoa);
                    sgcoa = SegCoaManager.getSegCoa(txtParentCode.Text);
                    if (sgcoa != null)
                    {
                        if (sgcoa.RootLeaf.Equals("L"))
                        {
                            sgcoa.RootLeaf = "R";
                            SegCoaManager.UpdateSegCoa(sgcoa);
                        }
                    }
                    TreeView1.Nodes.Clear();
                    Populate();
                    TreeNode node = TreeView1.FindNode(Server.HtmlEncode(txtParentCode.Text));
                    if (node != null)
                    {
                        node.Expand();
                    }
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                  "alert('Segment Codes are update in Database Successfully..!!');", true);
                }
                else
                {
                    int count = IdManager.GetShowSingleValueInt("COUNT(*)", "UPPER([SEG_COA_DESC])", "GL_SEG_COA",
                        txtSegDesc.Text.ToUpper());
                    if (count > 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                            "alert('This Chart of account alrady exist....!!!!');", true);
                        return;
                    }
                    sgcoa = new SegCoa();
                    sgcoa.GlSegCode = txtSegCode.Text.Replace(" ","").Replace("'","");
                    sgcoa.BookName = Session["book"].ToString();
                    sgcoa.SegCoaDesc = txtSegDesc.Text.Replace("'","");
                    sgcoa.RootLeaf = ddlRootLeaf.SelectedValue;
                    sgcoa.Taxable = ddlTaxable.SelectedValue;
                    sgcoa.Status = ddlStatus.SelectedValue;
                    sgcoa.PostAllowed = ddlPostAllowed.SelectedValue;
                    sgcoa.ParentCode = txtParentCode.Text.Replace(" ", "").Replace("'", "");
                    sgcoa.OpenDate = txtOpenDate.Text;
                    sgcoa.LvlCode = ddlLvlcode.SelectedValue;
                    sgcoa.BudAllowed = ddlBudAllowed.SelectedValue;
                    sgcoa.AccType = ddlAccType.SelectedValue;
                    sgcoa.EntryUser = Session["user"].ToString();
                    if (CheckBox1.Checked == true)
                    {
                        sgcoa.ClientName = ddlClientName.SelectedValue;
                    }
                    else
                    {
                        sgcoa.BankName = ddlClientName.SelectedValue;
                    }
                    SegCoaManager.CreateSegCoa(sgcoa,"");

                    sgcoa = SegCoaManager.getSegCoa(txtParentCode.Text);
                    if (sgcoa != null)
                    {
                        if (sgcoa.RootLeaf.Equals("L"))
                        {
                            sgcoa.RootLeaf = "R";
                            SegCoaManager.UpdateSegCoa(sgcoa);
                        }
                    }
                    TreeView1.Nodes.Clear();
                    Populate();
                    TreeNode node = TreeView1.FindNode(Server.HtmlEncode(txtParentCode.Text));
                    if (node != null)
                    {
                        node.Expand();
                    }
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                  "alert('Segment Codes Saved in Database Successfully..!!');", true);
                }
            }
        }
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        //System.Threading.Thread.Sleep(3000);
        if (txtSegCode.Text != "")
        {
            int coaF = SegCoaManager.getChild(txtSegCode.Text.ToString());
            if (coaF < 1)
            {
                SegCoa sgcoa = SegCoaManager.getSegCoa(txtSegCode.Text.ToString());
                if (sgcoa != null)
                {
                    int count = SegCoaManager.GetShowCount(txtSegCode.Text.ToString());
                    if (count > 0)
                    {
                        
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('you can not delete this account.because this account alrady transaction..!!!');", true);
                    }
                    else
                    {
                        SegCoaManager.DeleteSegCoa(sgcoa);
                        TreeView1.Nodes.Clear();
                        Populate();
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('***Segment Codes deleted from Database Successfully!!');", true);
                        btnClear_Click(sender, e);
                    }
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock( this, this.GetType(), "ale", "alert('*** You cannnot delete this segment code while child segment is exist!!');", true);
            }
        }        
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        Clear();
    }

    private void Clear()
    {
        txtSegCode.Text =txtSearchSegCoa.Text= "";
        txtSegDesc.Text = "";
        ddlAccType.SelectedValue = "N";
        ddlBudAllowed.SelectedValue = "N";
        ddlLvlcode.SelectedValue = "0";
        txtOpenDate.Text = "";
        txtParentCode.Text = "";
        ddlPostAllowed.SelectedValue = "N";
        ddlRootLeaf.SelectedValue = "L";
        ddlTaxable.SelectedValue = "N";
        ddlStatus.SelectedValue = "A";
    }
    
    protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
    {
        SegCoaFind(TreeView1.SelectedNode.Value.ToString());
    }

    private void SegCoaFind(string CoaCode)
    {
        SegCoa segcoa = SegCoaManager.getSegCoa(CoaCode);
        // DataTable dt = SegCoaManager.GetShowBankOrClientInformation(TreeView1.SelectedNode.Value.ToString());
        if (segcoa != null)
        {
            txtSegCode.Text = segcoa.GlSegCode;
            txtSegDesc.Text = segcoa.SegCoaDesc;
            ddlAccType.SelectedValue = segcoa.AccType;
            ddlBudAllowed.SelectedValue = segcoa.BudAllowed;
            ddlLvlcode.SelectedValue = segcoa.LvlCode;
            txtOpenDate.Text = segcoa.OpenDate;
            txtParentCode.Text = segcoa.ParentCode;
            ddlPostAllowed.SelectedValue = segcoa.PostAllowed;
            ddlRootLeaf.SelectedValue = segcoa.RootLeaf;
            ddlTaxable.SelectedValue = segcoa.Taxable;
            ddlStatus.SelectedValue = segcoa.Status;
            //if (dt.Rows.Count > 0)
            //{
            //    DataRow row = dt.Rows[0];
            //    if (row["Check"].ToString() == "Bank")
            //    {
            //        string queryBank = " select BANK_ID,BANK_NAME from BANK_INFO order by 1";
            //        util.PopulationDropDownList(ddlClientName, "Bank", queryBank, "BANK_NAME", "BANK_ID");
            //        ddlClientName.Items.Insert(0, new ListItem("Select Bank"));
            //        CheckBox2.Checked = true;
            //        lblClient.Visible = true;
            //        ddlClientName.SelectedValue = row["bank_id"].ToString();
            //        ddlClientName.Visible = true;
            //    }
            //    //else if (row["Check"].ToString() == "Client")
            //    //{
            //    //    ddlClientName.DataSource = clsClientInfoManager.GetClientInfosGrid();
            //    //    ddlClientName.DataTextField = "client_name";
            //    //    ddlClientName.DataValueField = "client_id";
            //    //    ddlClientName.DataBind();
            //    //    //ddlClientName.Items.Insert(0, new ListItem("Select Client"));
            //    //    CheckBox1.Checked = true;
            //    //    lblClient.Visible = true;
            //    //    ddlClientName.SelectedValue = row["client_id"].ToString();
            //    //    ddlClientName.Visible = true;
            //    //}
            //}
            foreach (GridViewRow gvr in dgGlCoaGen.Rows)
            {
                string lvl = "";
                string connectionString = DataManager.OraConnString();
                SqlDataReader dReader;
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connectionString;
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select lvl_code from gl_level_type where lvl_desc='" +
                                  gvr.Cells[0].Text.ToString().Trim() + "'";
                conn.Open();
                dReader = cmd.ExecuteReader();
                if (dReader.HasRows == true)
                {
                    while (dReader.Read())
                    {
                        lvl = dReader["lvl_code"].ToString();
                    }
                }
                if (lvl == segcoa.LvlCode)
                {
                    gvr.Cells[1].Text = segcoa.GlSegCode;
                    gvr.Cells[2].Text = segcoa.SegCoaDesc;
                }
            }
            //TreeView1.Nodes.Clear();
            //Populate();
            //TreeNode node = TreeView1.FindNode(Server.HtmlEncode(txtParentCode.Text));
            //if (node != null)
            //{
            //    node.Expand();
            //}

            //TreeView1.CollapseAll();
            TreeNode searchNode = TreeView1.FindNode(txtSegCode.Text);
            if (searchNode != null)
                searchNode.Expand();
        }
    }

    protected void dgLevel_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        DataTable dtLvl = LvlManager.GetLevels();
        dgLevel.EditIndex = -1;
        dgLevel.DataSource = dtLvl;
        dgLevel.DataBind();
    }
    protected void dgLevel_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GlLevel lvl = LvlManager.getLevel(((TextBox)dgLevel.Rows[e.RowIndex].FindControl("txtLevelCode")).Text);
        lvl.LvlDesc = ((TextBox)dgLevel.Rows[e.RowIndex].FindControl("txtLevelDesc")).Text;
        lvl.LvlMaxSize = ((TextBox)dgLevel.Rows[e.RowIndex].FindControl("txtLevelMaxSize")).Text;
        lvl.LvlEnabled = ((DropDownList)dgLevel.Rows[e.RowIndex].FindControl("ddlLevelEnabled")).SelectedValue.ToString();
        lvl.LvlSegType = ((DropDownList)dgLevel.Rows[e.RowIndex].FindControl("ddlLevelSegType")).SelectedValue.ToString();
        lvl.LvlOrder = ((TextBox)dgLevel.Rows[e.RowIndex].FindControl("txtLevelOrder")).Text;
        LvlManager.UpdateLevel(lvl);
        DataTable dtLvl = LvlManager.GetLevels();
        dgLevel.EditIndex = -1;
        dgLevel.DataSource = dtLvl;
        dgLevel.DataBind();
        dgGlCoaGen.DataSource = LvlManager.GetLevelsGrid();
        dgGlCoaGen.DataBind();
        //ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script language=JavaScript>alert('Updated Successfully! ');</script>");
    }
    protected void dgLevel_RowEditing(object sender, GridViewEditEventArgs e)
    {
        string le = ((Label)dgLevel.Rows[e.NewEditIndex].FindControl("lblLevelEnabled")).Text.ToString();
        string ls = ((Label)dgLevel.Rows[e.NewEditIndex].FindControl("lblLevelSegType")).Text.ToString();
        DataTable dtLvl = LvlManager.GetLevels();
        dgLevel.EditIndex = e.NewEditIndex;
        dgLevel.DataSource = dtLvl;
        dgLevel.DataBind();
        ((DropDownList)dgLevel.Rows[e.NewEditIndex].FindControl("ddlLevelEnabled")).SelectedIndex = ((DropDownList)dgLevel.Rows[e.NewEditIndex].FindControl("ddlLevelEnabled")).Items.IndexOf(((DropDownList)dgLevel.Rows[e.NewEditIndex].FindControl("ddlLevelEnabled")).Items.FindByValue(le));
        ((DropDownList)dgLevel.Rows[e.NewEditIndex].FindControl("ddlLevelSegType")).SelectedIndex = ((DropDownList)dgLevel.Rows[e.NewEditIndex].FindControl("ddlLevelSegType")).Items.IndexOf(((DropDownList)dgLevel.Rows[e.NewEditIndex].FindControl("ddlLevelSegType")).Items.FindByValue(ls));
    }
    protected void btnGenCoa_Click(object sender, EventArgs e)
    {
        //System.Threading.Thread.Sleep(3000);
        foreach (GridViewRow gvr in dgGlCoaGen.Rows)
        {
            if (gvr.Cells[1].Text.ToString().Replace("&nbsp;", "").ToString().Trim() == "")
            {
                //lblTransStatus.Visible = true;
                //lblTransStatus.ForeColor = System.Drawing.Color.Red;
                //lblTransStatus.Text = "Please input all segment code in Chart-of-Account Generation table";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Please input all segment code in Chart-of-Account Generation table!!');", true);
                return;
            }
        }
        DataTable[] dtm = new DataTable[dgGlCoaGen.Rows.Count];
        DataTable dtCoa = new DataTable();
        string sepTyp = Session["septype"].ToString();
        int[] lvlSize = new int[dgGlCoaGen.Rows.Count];
        string[] lvlcode = new string[dgGlCoaGen.Rows.Count];

        string criteria = "";
        string[] criteriaN = new string[dgGlCoaGen.Rows.Count];
        string query = "";
        string query1 = "";
        int x = 0;
        int y = 0;

        foreach (GridViewRow gvr in dgGlCoaGen.Rows)
        {
            string connectionString = DataManager.OraConnString();
            SqlDataReader dReader;
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select lvl_code,lvl_max_size from gl_level_type where lvl_desc='" + gvr.Cells[0].Text.ToString().Trim() + "'";
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    lvlSize[x] = int.Parse(dReader["lvl_max_size"].ToString());
                    lvlcode[x] = dReader["lvl_code"].ToString();
                }
            }
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            if (x == 0 && gvr.Cells[1].Text.ToString().Replace("&nbsp;", "").ToString().Trim() != String.Empty)
            {
                query = "with t" + x.ToString() + "(seg_coa_code,seg_coa_desc,parent_code,rootleaf) as " +
                " (select seg_coa_code,seg_coa_desc,PARENT_CODE,rootleaf from GL_SEG_COA where SEG_COA_CODE='" + gvr.Cells[1].Text.ToString().Trim() + "' and LVL_CODE='" + lvlcode[x] + "' " +
                " union all select tt" + x.ToString() + ".seg_coa_code,tt" + x.ToString() + ".seg_coa_desc,tt" + x.ToString() + ".parent_code,tt" + x.ToString() + ".ROOTLEAF from GL_SEG_COA as tt" + x.ToString() + " ,t" + x.ToString() + " " +
                " where t" + x.ToString() + ".seg_coa_code=tt" + x.ToString() + ".PARENT_CODE and tt" + x.ToString() + ".LVL_CODE='" + lvlcode[x] + "') ";
                criteria += " substring(gl_coa_code,1," + lvlSize[x].ToString() + ") in (SELECT SEG_COA_CODE  FROM t" + x.ToString() + " where rootleaf='L' )  ";
            }
            else if (x > 0 && gvr.Cells[1].Text.ToString().Replace("&nbsp;", "").ToString().Trim() != String.Empty)
            {
                query += ", t" + x.ToString() + "(seg_coa_code,seg_coa_desc,parent_code,rootleaf) as " +
                " (select seg_coa_code,seg_coa_desc,PARENT_CODE,rootleaf from GL_SEG_COA where SEG_COA_CODE='" + gvr.Cells[1].Text.ToString().Trim() + "' and LVL_CODE='" + lvlcode[x] + "' " +
                " union all select tt" + x.ToString() + ".seg_coa_code,tt" + x.ToString() + ".seg_coa_desc,tt" + x.ToString() + ".parent_code,tt" + x.ToString() + ".ROOTLEAF from GL_SEG_COA as tt" + x.ToString() + " ,t" + x.ToString() + " " +
                " where t" + x.ToString() + ".seg_coa_code=tt" + x.ToString() + ".PARENT_CODE and tt" + x.ToString() + ".LVL_CODE='" + lvlcode[x] + "') ";
                criteria += " and substring(gl_coa_code,charindex('" + sepTyp + "',gl_coa_code," + y + ")+1," + lvlSize[x].ToString() + ") in (SELECT SEG_COA_CODE  FROM t" + x.ToString() + " where rootleaf='L' )  ";
            }
            query1 = "with t" + x.ToString() + "(seg_coa_code,seg_coa_desc,parent_code,rootleaf,acc_type) as " +
                " (select seg_coa_code,seg_coa_desc,PARENT_CODE,rootleaf,acc_type from GL_SEG_COA where SEG_COA_CODE='" + gvr.Cells[1].Text.ToString().Trim() + "' and LVL_CODE='" + lvlcode[x] + "' " +
                " union all select tt" + x.ToString() + ".seg_coa_code,tt" + x.ToString() + ".seg_coa_desc,tt" + x.ToString() + ".parent_code,tt" + x.ToString() + ".ROOTLEAF,tt" + x.ToString() + ".acc_type from GL_SEG_COA as tt" + x.ToString() + " ,t" + x.ToString() + " " +
                " where t" + x.ToString() + ".seg_coa_code=tt" + x.ToString() + ".PARENT_CODE and tt" + x.ToString() + ".LVL_CODE='" + lvlcode[x] + "') ";
            criteriaN[x] = " SELECT SEG_COA_CODE,seg_coa_desc,acc_type FROM GL_SEG_COA where status='A' and seg_coa_code in (SELECT SEG_COA_CODE  FROM t" + x.ToString() + " where rootleaf='L') ";

            dtm[x] = SegCoaManager.GetSegCoass(query1 + criteriaN[x]);

            y = y + lvlSize[x];
            x = x + 1;
        }
        if (criteria != "")
        {
            query = query + "select GL_COA_CODE, COA_ENABLED, convert(varchar,EFFECTIVE_FROM,103) EFFECTIVE_FROM, convert(varchar,EFFECTIVE_TO,103) EFFECTIVE_TO, BUD_ALLOWED, POST_ALLOWED, TAXABLE, ACC_TYPE, STATUS, BOOK_NAME, ENTRY_USER, convert(varchar,ENTRY_DATE,103) ENTRY_DATE, " +
                " UPDATE_USER, convert(varchar,UPDATE_DATE,103) UPDATE_DATE, AUTHO_USER, convert(varchar,AUTHO_DATE,103) AUTHO_DATE, COA_DESC, COA_CURR_BAL, COA_NATURAL_CODE,'' INC,opening_balance from gl_coa where " + criteria + " order by 1";
        }
        else
        {
            query = "select GL_COA_CODE, COA_ENABLED, convert(varchar,EFFECTIVE_FROM,103) EFFECTIVE_FROM, convert(varchar,EFFECTIVE_TO,103) EFFECTIVE_TO, BUD_ALLOWED, POST_ALLOWED, TAXABLE, ACC_TYPE, STATUS, BOOK_NAME, ENTRY_USER, convert(varchar,ENTRY_DATE,103) ENTRY_DATE, " +
                "UPDATE_USER, convert(varchar,UPDATE_DATE,103) UPDATE_DATE, AUTHO_USER, convert(varchar,AUTHO_DATE,103) AUTHO_DATE, COA_DESC, COA_CURR_BAL, COA_NATURAL_CODE,'' INC,opening_balance from gl_coa order by 1";
        }
        DataTable dtAlready = GlCoaManager.GetGlCoaCodes(query);
        dtCoa = dtAlready.Clone();

        DataTable dt = new DataTable();
        dt.Columns.Add("seg_coa_code", typeof(string));
        dt.Columns.Add("seg_coa_desc", typeof(string));
        dt.Columns.Add("acc_type", typeof(string));
        dt.Columns.Add("opening_balance", typeof(string));
        DataRow dr;
        for (int i = 0; i < dgGlCoaGen.Rows.Count - 1; i++)
        {
            dt.Clear();
            for (int m = 0; m <= dtm[i].Rows.Count - 1; m++)
            {
                for (int n = 0; n <= dtm[i + 1].Rows.Count - 1; n++)
                {
                    dr = dt.NewRow();
                    dr["seg_coa_code"] = ((DataRow)dtm[i].Rows[m])["seg_coa_code"].ToString().Trim() + sepTyp + ((DataRow)dtm[i + 1].Rows[n])["seg_coa_code"].ToString().Trim();
                    dr["seg_coa_desc"] = ((DataRow)dtm[i].Rows[m])["seg_coa_desc"].ToString().Trim() + ", " + ((DataRow)dtm[i + 1].Rows[n])["seg_coa_desc"].ToString().Trim();
                    //dr["opening_balance"] = ((DataRow)dtm[i].Rows[m])["opening_balance"].ToString().Trim() + ", " + ((DataRow)dtm[i + 1].Rows[n])["opening_balance"].ToString().Trim();
                    if (((DataRow)dtm[i].Rows[m])["acc_type"].ToString().Trim() != "N")
                    {
                        dr["acc_type"] = ((DataRow)dtm[i].Rows[m])["acc_type"].ToString().Trim();
                    }
                    else if (((DataRow)dtm[i + 1].Rows[n])["acc_type"].ToString().Trim() != "N")
                    {
                        dr["acc_type"] = ((DataRow)dtm[i + 1].Rows[n])["acc_type"].ToString().Trim();
                    }
                    dt.Rows.Add(dr);
                }
            }
            dtm[i + 1] = dt.Copy();
        }
        DataRow drCoa;
        foreach (DataRow drC in dtm[dgGlCoaGen.Rows.Count - 1].Rows)
        {
            drCoa = dtCoa.NewRow();
            drCoa["gl_coa_code"] = drC["seg_coa_code"].ToString().Trim();
            drCoa["coa_desc"] = drC["seg_coa_desc"].ToString().Trim();
            drCoa["acc_type"] = drC["acc_type"].ToString().Trim();
            //drCoa["opening_balance"] = drC["opening_balance"].ToString().Trim();
            drCoa["status"] = "U";
            dtCoa.Rows.Add(drCoa);
        }
        for (int i = 0; i < dtAlready.Rows.Count; i++)
        {
            for (int j = 0; j < dtCoa.Rows.Count; j++)
            {
                if (((DataRow)dtCoa.Rows[j])["gl_coa_code"].ToString().Trim() == ((DataRow)dtAlready.Rows[i])["gl_coa_code"].ToString().Trim())
                {
                    dtCoa.Rows.RemoveAt(j);
                }
            }
        }
        char sep = Convert.ToChar(Session["septype"].ToString());
        foreach (DataRow drc in dtCoa.Rows)
        {
            string mainseg = "";
            string[] segcode = drc["gl_coa_code"].ToString().Split(sep);
            for (int i = 0; i < segcode.Length; i++)
            {
                string a = SegCoaManager.getMainSeg(segcode[i].ToString());
                if (a == "N")
                {
                    mainseg = segcode[i].ToString();
                }
            }
            drc.BeginEdit();
            drc["coa_natural_code"] = mainseg;
            drc.AcceptChanges();
        }

        dgGlCoa.EditIndex = -1;
        if (dtCoa.Rows.Count > 0)
        {
            btnSaveCoa.Visible = true;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('To Save Gl COA Codes in Database Click on SaveCoa Link!!');", true);
            btnSaveCoa.Visible = true;
        }
        else
        {
            btnSaveCoa.Visible = false;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Possible Gl COA Codes Are Already in Database. Or \\n Check your COA Root/Leaf?. !!');", true);
            btnSaveCoa.Visible = false;
        }
        dgGlCoa.DataSource = dtCoa;
        Session["coa"] = dtCoa;
        dgGlCoa.DataBind();
        foreach (GridViewRow gvr in dgGlCoa.Rows)
        {
            ((LinkButton)gvr.FindControl("lbEdit")).Visible = false;
            ((LinkButton)gvr.FindControl("lbDelete")).Visible = false;
            ((CheckBox)gvr.FindControl("chkInc")).Visible = true;
        }
    }   
   
    protected void btnShowCoa_Click(object sender, EventArgs e)
    {
        //System.Threading.Thread.Sleep(3000);
        string sepTyp = Session["septype"].ToString();
        int[] lvlSize = new int[dgGlCoaGen.Rows.Count];
        string[] lvlcode = new string[dgGlCoaGen.Rows.Count];
        string connectionString = DataManager.OraConnString();
        SqlDataReader dReader;
        SqlConnection conn = new SqlConnection();
        conn.ConnectionString = connectionString;
        SqlCommand cmd = new SqlCommand(); 
        string query = "";
        string criteria = "";
        int x = 0;
        int y = 0;
        foreach (GridViewRow gvr in dgGlCoaGen.Rows)
        {
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select lvl_code,lvl_max_size from gl_level_type where lvl_desc='" + gvr.Cells[0].Text.ToString().Trim() + "'";
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    lvlSize[x] = int.Parse(dReader["lvl_max_size"].ToString());
                    lvlcode[x] = dReader["lvl_code"].ToString();
                }
            }
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            if (x == 0 && gvr.Cells[1].Text.ToString().Replace("&nbsp;", "").ToString().Trim() != "")
            {
                query = "with t" + x.ToString() + "(seg_coa_code,seg_coa_desc,parent_code,rootleaf) as " +
                " (select seg_coa_code,seg_coa_desc,PARENT_CODE,rootleaf from GL_SEG_COA where SEG_COA_CODE='" + gvr.Cells[1].Text.ToString().Trim() + "' and LVL_CODE='" + lvlcode[x] + "' " +
                " union all select tt" + x.ToString() + ".seg_coa_code,tt" + x.ToString() + ".seg_coa_desc,tt" + x.ToString() + ".parent_code,tt" + x.ToString() + ".ROOTLEAF from GL_SEG_COA as tt" + x.ToString() + " ,t" + x.ToString() + " " +
                " where t" + x.ToString() + ".seg_coa_code=tt" + x.ToString() + ".PARENT_CODE and tt" + x.ToString() + ".LVL_CODE='" + lvlcode[x] + "') ";
                criteria += " substring(gl_coa_code,1," + lvlSize[x].ToString() + ") in (SELECT SEG_COA_CODE  FROM t" + x.ToString() + " where rootleaf='L' )  ";
            }
            else if (x > 0 && gvr.Cells[1].Text.ToString().Replace("&nbsp;", "").ToString().Trim() != "")
            {
                query += ", t" + x.ToString() + "(seg_coa_code,seg_coa_desc,parent_code,rootleaf) as " +
                " (select seg_coa_code,seg_coa_desc,PARENT_CODE,rootleaf from GL_SEG_COA where SEG_COA_CODE='" + gvr.Cells[1].Text.ToString().Trim() + "' and LVL_CODE='" + lvlcode[x] + "' " +
                " union all select tt" + x.ToString() + ".seg_coa_code,tt" + x.ToString() + ".seg_coa_desc,tt" + x.ToString() + ".parent_code,tt" + x.ToString() + ".ROOTLEAF from GL_SEG_COA as tt" + x.ToString() + " ,t" + x.ToString() + " " +
                " where t" + x.ToString() + ".seg_coa_code=tt" + x.ToString() + ".PARENT_CODE and tt" + x.ToString() + ".LVL_CODE='" + lvlcode[x] + "') ";
                criteria += " and substring(gl_coa_code,charindex('" + sepTyp + "',gl_coa_code," + y + ")+1," + lvlSize[x].ToString() + ") in (SELECT SEG_COA_CODE  FROM t" + x.ToString() + " where rootleaf='L' )  ";
            }
            y = y + lvlSize[x];
            x = x + 1;
        }
        if (criteria != "")
        {
            query = query + "select GL_COA_CODE, COA_ENABLED, convert(varchar,EFFECTIVE_FROM,103) EFFECTIVE_FROM, convert(varchar,EFFECTIVE_TO,103) EFFECTIVE_TO, BUD_ALLOWED, POST_ALLOWED, TAXABLE, ACC_TYPE, STATUS, BOOK_NAME, ENTRY_USER, convert(varchar,ENTRY_DATE,103) ENTRY_DATE, " +
                " UPDATE_USER, convert(varchar,UPDATE_DATE,103) UPDATE_DATE, AUTHO_USER, convert(varchar,AUTHO_DATE,103) AUTHO_DATE, COA_DESC, COA_CURR_BAL, COA_NATURAL_CODE,'' INC,opening_balance from gl_coa where " + criteria + " order by 1";
        }
        else
        {
            query = "select GL_COA_CODE, COA_ENABLED, convert(varchar,EFFECTIVE_FROM,103) EFFECTIVE_FROM, convert(varchar,EFFECTIVE_TO,103) EFFECTIVE_TO, BUD_ALLOWED, POST_ALLOWED, TAXABLE, ACC_TYPE, STATUS, BOOK_NAME, ENTRY_USER, convert(varchar,ENTRY_DATE,103) ENTRY_DATE, " +
                " UPDATE_USER, convert(varchar,UPDATE_DATE,103) UPDATE_DATE, AUTHO_USER, convert(varchar,AUTHO_DATE,103) AUTHO_DATE, COA_DESC, COA_CURR_BAL, COA_NATURAL_CODE,'' INC,opening_balance from gl_coa order by 1";
        }
        DataTable dt = GlCoaManager.GetGlCoaCodes(query);
        dgGlCoa.DataSource = dt;
        Session.Remove("coa");
        Session["coa"] = dt;
        foreach (GridViewRow gvr in dgGlCoa.Rows)
        {
            ((LinkButton)gvr.FindControl("lbEdit")).Visible = true;
            ((LinkButton)gvr.FindControl("lbDelete")).Visible = true;
            ((CheckBox)gvr.FindControl("chkInc")).Visible = false;
        }
        if (dt.Rows.Count > 0)
        {
            btnSaveCoa.Visible = false;
            dgGlCoa.DataBind();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock( this, this.GetType(), "ale", "alert('No Chart-of-Account Code was previously created!!');", true);
        }
        lblTransStatus.Text = "";
    }

    protected void dgGlCoa_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        if (Session["coa"] != null)
        {
            DataTable dt = (DataTable)Session["coa"];
            dgGlCoa.DataSource = dt;
            dgGlCoa.PageIndex = e.NewPageIndex;
            dgGlCoa.DataBind();
            if (btnSaveCoa.Visible == false)
            {               
                foreach (GridViewRow gvr in dgGlCoa.Rows)
                {
                    ((LinkButton)gvr.FindControl("lbEdit")).Visible = true;
                    ((LinkButton)gvr.FindControl("lbDelete")).Visible = true;
                    ((CheckBox)gvr.FindControl("chkInc")).Visible = false;
                }
            }
            else
            {
                foreach (GridViewRow gvr in dgGlCoa.Rows)
                {
                    ((LinkButton)gvr.FindControl("lbEdit")).Visible = false;
                    ((LinkButton)gvr.FindControl("lbDelete")).Visible = false;
                    ((CheckBox)gvr.FindControl("chkInc")).Visible = true;
                    if (((DataRow)dt.Rows[gvr.DataItemIndex])["inc"].ToString() == "N")
                    {
                        ((CheckBox)gvr.FindControl("chkInc")).Checked = false;
                    }
                }                
            }  
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock( this, this.GetType(), "ale", "alert('Your session is over. Please try these again!!');", true);
        }
    }
    protected void chkIncCheck_Changed(object sender, EventArgs e)
    {
        if (Session["coa"] != null)
        {
            DataTable dt = (DataTable)Session["coa"];
            CheckBox chk = (CheckBox)sender;
            GridViewRow gvr = (GridViewRow)chk.NamingContainer;
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            if (chk.Checked == true)
            {
                dr["inc"] = "Y";
            }
            else
            {
                dr["inc"] = "N";
            }
        }
    }
    
    protected void btnSaveCoa_Click(object sender, EventArgs e)
    {
        //System.Threading.Thread.Sleep(3000);
        if (Session["coa"] != null)
        {
            DataTable dt = (DataTable)Session["coa"];
            GlCoa gcoa;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["inc"].ToString() != "N")
                {
                    gcoa = new GlCoa();
                    gcoa.GlCoaCode = dr["gl_coa_code"].ToString();
                    gcoa.CoaDesc = dr["coa_desc"].ToString();
                    gcoa.CoaEnabled = "Y";
                    gcoa.CoaNaturalCode = dr["coa_natural_code"].ToString();
                    gcoa.PostAllowed = "Y";
                    gcoa.BudAllowed = "N";
                    gcoa.CoaCurrBal = "0";
                    gcoa.EffectiveFrom = null;
                    gcoa.EffectiveTo = null;
                    gcoa.Status = "A";
                    gcoa.Taxable = "N";
                    gcoa.AccType = dr["acc_type"].ToString();
                    gcoa.BookName = Session["book"].ToString();
                    gcoa.LoginBy = Session["user"].ToString();
                    int count = GlCoaManager.GetDuplicatId(dr["gl_coa_code"].ToString());
                    if (count == 0)
                    {
                        GlCoaManager.CreateGlCoa(gcoa);
                    }
                }
            }
            btnSaveCoa.Visible = false;
            for (int i = 0; i < dt.Rows.Count; i++ )
            {
                if (((DataRow)dt.Rows[i])["inc"].ToString() == "N")
                {
                    dt.Rows.RemoveAt(i);
                }
            }
            dgGlCoa.DataSource = dt;
            dgGlCoa.DataBind();
            foreach (GridViewRow gvr in dgGlCoa.Rows)
            {
                ((LinkButton)gvr.FindControl("lbEdit")).Visible = true;
                ((LinkButton)gvr.FindControl("lbDelete")).Visible = true;
                ((CheckBox)gvr.FindControl("chkInc")).Visible = false;
            }
            ScriptManager.RegisterClientScriptBlock( this, this.GetType(), "ale", "alert('Gl COA codes are saved successfully!!');", true);            
        }
    }
    protected void dgGlCoa_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        if (Session["coa"] != null)
        {
            DataTable dt = (DataTable)Session["coa"];
            dgGlCoa.DataSource = dt;
            btnSaveCoa.Visible = false;
            dgGlCoa.EditIndex = -1;
            dgGlCoa.DataBind();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock( this, this.GetType(), "ale", "alert('Your session is over. Please try these again!!');", true);
        }
    }
    protected void dgGlCoa_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (Session["coa"] != null)
        {
            DataTable dt = (DataTable)Session["coa"];
            string accType = ((Label)dgGlCoa.Rows[e.NewEditIndex].FindControl("lblGlAccType")).Text.ToString();
            string status = ((Label)dgGlCoa.Rows[e.NewEditIndex].FindControl("lblGlStatus")).Text.ToString();
            
            dgGlCoa.DataSource = dt;
            btnSaveCoa.Visible = false;
            dgGlCoa.EditIndex = e.NewEditIndex;
            dgGlCoa.DataBind();
            ((DropDownList)dgGlCoa.Rows[e.NewEditIndex].FindControl("ddlGlAccType")).SelectedIndex = ((DropDownList)dgGlCoa.Rows[e.NewEditIndex].FindControl("ddlGlAccType")).Items.IndexOf(((DropDownList)dgGlCoa.Rows[e.NewEditIndex].FindControl("ddlGlAccType")).Items.FindByValue(accType));
            ((DropDownList)dgGlCoa.Rows[e.NewEditIndex].FindControl("ddlGlStatus")).SelectedIndex = ((DropDownList)dgGlCoa.Rows[e.NewEditIndex].FindControl("ddlGlStatus")).Items.IndexOf(((DropDownList)dgGlCoa.Rows[e.NewEditIndex].FindControl("ddlGlStatus")).Items.FindByValue(status));
            ((TextBox)dgGlCoa.Rows[e.NewEditIndex].FindControl("txtOpBalance")).Text = "0";
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock( this, this.GetType(), "ale", "alert('Your session is over. Please try these again!!');", true);
        }

    }
    protected void dgGlCoa_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (Session["coa"] != null)
        {
            DataTable dt = (DataTable)Session["coa"];
            DataRow dr = dt.Rows[dgGlCoa.Rows[e.RowIndex].DataItemIndex];
            dt.Rows.Remove(dr);
            dgGlCoa.DataSource = dt;
            btnSaveCoa.Visible = false;
            dgGlCoa.EditIndex = -1;
            dgGlCoa.DataBind();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock( this, this.GetType(), "ale", "alert('Your session is over. Please try these again!!');", true);
        }
    }
    protected void dgGlCoa_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (Session["coa"] != null)
        {
            DataTable dt = (DataTable)Session["coa"];
            DataRow dr = dt.Rows[dgGlCoa.Rows[e.RowIndex].DataItemIndex];
            dr["coa_desc"] = ((TextBox)dgGlCoa.Rows[e.RowIndex].FindControl("txtGlCoaDesc")).Text;
            dr["acc_type"] = ((DropDownList)dgGlCoa.Rows[e.RowIndex].FindControl("ddlGlAccType")).SelectedValue;
            dr["status"] = ((DropDownList)dgGlCoa.Rows[e.RowIndex].FindControl("ddlGlStatus")).SelectedValue;
            dr["opening_balance"] = ((TextBox)dgGlCoa.Rows[e.RowIndex].FindControl("txtOpBalance")).Text;
            dr["gl_coa_code"] = ((TextBox)dgGlCoa.Rows[e.RowIndex].FindControl("txtGlCoaCode")).Text;
            dgGlCoa.DataSource = dt;
            btnSaveCoa.Visible = false;
            dgGlCoa.EditIndex = -1;
            dgGlCoa.DataBind();

            GlCoaManager.UpdateOpeningBalance(dr["gl_coa_code"].ToString(), dr["opening_balance"].ToString());
        }
        else
        {

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Your session is over. Please try these again!!');", true);
        }
    }
    private void getMapDtlGrid()
    {
        //DataTable dt = new DataTable();
        //dt.Columns.Add("lvl_code", typeof(string));
        //dt.Columns.Add("lvl_desc", typeof(string));
        //dt.Columns.Add("lvl_max_size", typeof(string));
        //dt.Columns.Add("lvl_enabled", typeof(string));
        //dt.Columns.Add("lvl_seg_type", typeof(string));
        //dt.Columns.Add("lvl_order", typeof(string));
        ////dt.Columns.Add("cons_amt", typeof(string));
        ////dt.Columns.Add("dtMapBreak", typeof(DataTable));
        ////DataTable dtMapBreak = new DataTable();
        ////dtMapBreak.Columns.Add("ref_sl_no", typeof(string));
        ////dtMapBreak.Columns.Add("sl_no", typeof(string));
        ////dtMapBreak.Columns.Add("gl_seg_code", typeof(string));
        //DataRow dr = dt.NewRow();
        ////dr["lvl_code"] = "";
        ////dr["lvl_desc"] = "";
        ////dr["lvl_max_size"] = "";
        ////dr["lvl_enabled"] = "";
        ////dr["lvl_seg_type"] = "";
        ////dr["lvl_order"] = "";
        //dt.Rows.Add(dr);
        //dgLevel.DataSource = dt;
        //dgLevel.EditIndex = -1;
        //dgLevel.ShowFooter = true;
        //dgLevel.DataBind();
        ////((TextBox)dgLevel.FooterRow.FindControl("txtSlNo")).Text = "1";
        //Session["mapdtl"] = dt;
        
        DataTable dt = new DataTable();
        dt.Columns.Add("lvl_code", typeof(string));
        dt.Columns.Add("lvl_desc", typeof(string));
        dt.Columns.Add("lvl_max_size", typeof(string));
        dt.Columns.Add("lvl_enabled", typeof(string));
        dt.Columns.Add("lvl_seg_type", typeof(string));
        dt.Columns.Add("lvl_order", typeof(string));
        DataRow dr = dt.NewRow();
        dt.Rows.Add(dr);
        ViewState["paydtl"] = dt;
        dgLevel.DataSource = dt;
        dgLevel.DataBind();
    }
    protected void dgLevel_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "AddNew")
        {
            dgLevel.FooterRow.Visible = true;
        }
        else if (e.CommandName == "Insert")
        {
            GlLevel glevel = new GlLevel();
            glevel.LvlCode=((TextBox)dgLevel.FooterRow.FindControl("txtLevelCode")).Text;
            glevel.LvlDesc = ((TextBox)dgLevel.FooterRow.FindControl("txtLevelDesc")).Text;
            glevel.LvlMaxSize = ((TextBox)dgLevel.FooterRow.FindControl("txtLevelMaxSize")).Text;
            glevel.LvlEnabled = ((DropDownList)dgLevel.FooterRow.FindControl("ddlLevelEnabled")).SelectedValue;
            glevel.LvlSegType = ((DropDownList)dgLevel.FooterRow.FindControl("ddlLevelSegType")).SelectedValue;
            glevel.LvlOrder = ((TextBox)dgLevel.FooterRow.FindControl("txtLevelOrder")).Text;
            LvlManager.InsertLevel(glevel);
            dgLevel.FooterRow.Visible = false;
            dgLevel.DataSource = LvlManager.GetLevels();
            dgLevel.DataBind();
            dgGlCoaGen.DataSource = LvlManager.GetLevelsGrid();
            dgGlCoaGen.DataBind();
            //ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script language=JavaScript>alert('Created Successfully! ');</script>");
        }        
        else
        {
            dgLevel.FooterRow.Visible = false;
        }
    }
    protected void dgLevel_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        LvlManager.DeleteLevel(((Label)dgLevel.Rows[e.RowIndex].FindControl("lblLevelCode")).Text);
        DataTable dt = LvlManager.GetLevels();
        if (dt.Rows.Count > 0)
        {
            dgLevel.DataSource = dt;
            dgLevel.DataBind();
        }
        dgGlCoaGen.DataSource = LvlManager.GetLevelsGrid();
        dgGlCoaGen.DataBind();
    }


    protected void dgGlCoa_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[5].Attributes.Add("style", "display:none");
        }
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[5].Attributes.Add("style", "display:none");
        }
    }
    protected void dgLevel_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    e.Row.Cells[1].Attributes.Add("style", "display:none");
        //}
        //if (e.Row.RowType == DataControlRowType.Header)
        //{
        //    e.Row.Cells[1].Attributes.Add("style", "display:none");
        //}
        //if (e.Row.RowType == DataControlRowType.Footer)
        //{
        //    e.Row.Cells[1].Attributes.Add("style", "display:none");
        //}
    }
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        if (CheckBox1.Checked == true)
        {
            CheckBox2.Checked = false;
            ddlClientName.Items.Clear();
            ddlClientName.DataSource = clsClientInfoManager.GetClientInfosGrid();
            ddlClientName.DataTextField = "client_name";
            ddlClientName.DataValueField = "client_id";
            ddlClientName.DataBind();
            ddlClientName.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Client"));

            lblClient.Text = "Client Name";
            lblClient.Visible = true;
            ddlClientName.Visible = true;
            UpdatePanel3.Update();
        }
        else
        {
            lblClient.Visible = false;
            ddlClientName.Visible = false;
            UpdatePanel3.Update();
        }
    }
    protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
    {
        if (CheckBox2.Checked == true)
        {
            CheckBox1.Checked = false;
            ddlClientName.Items.Clear();
            string queryBank = " select BANK_ID,BANK_NAME from BANK_INFO order by 1";
            util.PopulationDropDownList(ddlClientName, "Bank", queryBank, "BANK_NAME", "BANK_ID");
            ddlClientName.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Bank"));
            lblClient.Text = "Bank Name";
            lblClient.Visible = true;
            ddlClientName.Visible = true;

            UpdatePanel3.Update();
            
        }
        else
        {
            lblClient.Visible = false;
            ddlClientName.Visible = false;
            UpdatePanel3.Update();
        }
    }
    protected void ddlClientName_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtSegDesc.Text = ddlClientName.SelectedItem.Text;
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(btnPrint);
        //Response.Clear();
        //Response.ContentType = "application/pdf";
        //Response.AddHeader("content-disposition", "attachment; filename='StudentInformation'.pdf");
        //Document document = new Document();

        //document = new Document(PageSize.A4);
        //MemoryStream ms = new MemoryStream();
        //PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        //pdfPage page = new pdfPage();
        //writer.PageEvent = page;
        //document.Open();
        Session.Remove("rptbyte");
        Response.Clear();
        Document document = new Document();
        document = new Document(PageSize.A4);
        MemoryStream ms = new MemoryStream();
        PdfWriter writer = PdfWriter.GetInstance(document, ms);
        pdfPage page = new pdfPage();
        writer.PageEvent = page;
        document.Open();

        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(35f);
        float[] titwidth = new float[2] { 5, 200 };

        PdfPCell cell;
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;
        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        //cell.FixedHeight = 20f;
        dth.AddCell(cell);    

        cell = new PdfPCell(new Phrase("CHART OF ACCOUNTS (COA)", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        dth.AddCell(cell);       
        document.Add(dth);

        //LineSeparator line = new LineSeparator(0f, 100, null, Element.ALIGN_CENTER, -2);
        //document.Add(line);
        //PdfPTable dtempty = new PdfPTable(1);
        //cell.BorderWidth = 0f;
        //cell.FixedHeight = 5f;
        //dtempty.AddCell(cell);
        //document.Add(dtempty);

        float[] width = new float[2] { 30,70};
        PdfPTable pdtc = new PdfPTable(width);
        pdtc.WidthPercentage = 100;
        pdtc.HeaderRows = 2;

        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        cell.Colspan = 2;
        cell.Border = 0;
        pdtc.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Code"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 0;
        cell.BorderWidthBottom = 1f;
        cell.BorderWidthLeft = 0f;
        cell.BorderWidthTop = 0f;
        cell.BorderWidthRight = 0f;
        cell.FixedHeight = 15f;
        pdtc.AddCell(cell);

        cell = new PdfPCell(FormatHeaderPhrase("Accounts"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 0;
        cell.BorderWidthBottom = 1f;
        cell.BorderWidthLeft = 0f;
        cell.BorderWidthTop = 0f;
        cell.BorderWidthRight = 0f;
        cell.FixedHeight = 15f;
        pdtc.AddCell(cell);

        dtSegParent = SegCoaManager.GetSegCoaAll();       
        foreach (DataRow row in dtSegParent.Rows)
        {
            if (row["seg_coa_code"].ToString() != "0")
            {
                cell = new PdfPCell(FormatPhrase(row["seg_coa_code"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdtc.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(row["seg_coa_desc"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdtc.AddCell(cell);


                if (row["rootleaf"].ToString() == "R")
                {
                    PopLeft(row["seg_coa_code"].ToString(), cell, pdtc, document);
                }
            }
        }
        //UpdatePanel3.Update();
       

        PdfPTable dtempty1 = new PdfPTable(1);
        dtempty1.WidthPercentage = 100;
        cell = new PdfPCell(FormatPhrase(""));
        cell.VerticalAlignment = 0;
        cell.HorizontalAlignment = 0;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 30f;
        dtempty1.AddCell(cell);
        document.Add(dtempty1);
               
        document.Add(pdtc);       

        document.Close();       
        byte[] byt = ms.GetBuffer();
        if (Session["rptbyte"] != null) { byte[] rptbyt = (byte[])Session["rptbyte"]; rptbyt = byt; } else { Session["rptbyte"] = byt; }

        string strJS = ("<script type='text/javascript'>window.open('Default2.aspx','_blank');</script>");
        Page.ClientScript.RegisterStartupScript(this.GetType(), "strJSAlert", strJS);
      
    }

    public void PopLeft(string segcode, PdfPCell cell, PdfPTable pdtc, Document document)
    {
        DataTable dt = SegCoaManager.GetSegCoaChild("parent_code='" + segcode + "' order by convert(numeric,seg_coa_code)");     

        foreach (DataRow dr in dt.Rows)
        {
            if (dr["rootleaf"].ToString() == "R")
            {
                cell = new PdfPCell(FormatHeaderServiceColor(dr["seg_coa_code"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdtc.AddCell(cell);

                cell = new PdfPCell(FormatHeaderServiceColor(dr["seg_coa_desc"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdtc.AddCell(cell);
            }
            else
            {              

                cell = new PdfPCell(FormatPhrase(dr["seg_coa_code"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdtc.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["seg_coa_desc"].ToString()));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Border = 0;
                pdtc.AddCell(cell);
            }

            // document.Add(pdtc);
            //newNode.SelectAction = TreeNodeSelectAction.Expand;
         
            if (dr["rootleaf"].ToString() == "R")
            {
                PopLeft(dr["seg_coa_code"].ToString(), cell, pdtc, document);
                
            }
            
        }
    }
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }
    private static Phrase FormatHeaderServiceColor(string value)
    {
        var redListTextFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLUE);
       // var blackListTextFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

        //if (value1 == "")
        //{

        //    redListTextFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.GREEN);
        //}
        //else
        //{
        redListTextFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLUE);
        //}

       // var titleChunk = new Chunk(value, blackListTextFont);
        var descriptionChunk = new Chunk(value, redListTextFont);

        var phrase = new Phrase(descriptionChunk);
        //phrase.Add(descriptionChunk);
        return phrase;
    }
    protected void lbYes_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtSegDesc.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Input Description..!!');", true);
            return;
        }
        if (string.IsNullOrEmpty(ddlRootLeaf.SelectedItem.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Select Root/Leaf..!!');", true);
            return;
        }
        if (string.IsNullOrEmpty(ddlAccType.SelectedItem.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Select Account Type!!');", true);
            return;
        }

        SegCoa sgcoa = SegCoaManager.getSegCoa(txtSegCode.Text);
        if (sgcoa != null)
        {
            int count = IdManager.GetShowSingleValueInt1("COUNT(*)", "UPPER([SEG_COA_DESC])", "GL_SEG_COA",
                        txtSegDesc.Text.ToUpper(), "SEG_COA_CODE", Convert.ToInt32(txtSegCode.Text));
            if (count > 0)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale",
                    "alert('This Chart of account alrady exist....!!!!');", true);
                return;
            }

            sgcoa.SegCoaDesc = txtSegDesc.Text;
            sgcoa.RootLeaf = ddlRootLeaf.SelectedValue;
            sgcoa.Taxable = ddlTaxable.SelectedValue;
            sgcoa.Status = ddlStatus.SelectedValue;
            sgcoa.PostAllowed = ddlPostAllowed.SelectedValue;
            sgcoa.ParentCode = txtParentCode.Text;
            sgcoa.OpenDate = txtOpenDate.Text;
            sgcoa.LvlCode = ddlLvlcode.SelectedValue;
            sgcoa.BudAllowed = ddlBudAllowed.SelectedValue;
            sgcoa.AccType = ddlAccType.SelectedValue;
            sgcoa.ClientName = ddlClientName.SelectedValue;
            sgcoa.EntryUser = Session["user"].ToString();
            SegCoaManager.UpdateSegCoa(sgcoa);

            sgcoa = SegCoaManager.getSegCoa(txtParentCode.Text);
            if (sgcoa != null)
            {
                if (sgcoa.RootLeaf.Equals("L"))
                {
                    sgcoa.RootLeaf = "R";
                    SegCoaManager.UpdateSegCoa(sgcoa);
                }
            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('***Segment Codes are update from Database Successfully!!');", true);
        }
    }
    protected void lbNo_Click(object sender, EventArgs e)
    {

    }

    protected void txtSearchSegCoa_TextChanged(object sender, EventArgs e)
    {
        int CoaCode = IdManager.GetShowSingleValueInt("SEG_COA_CODE", "UPPER([SEG_COA_CODE]+' - '+[SEG_COA_DESC])",
            "[GL_SEG_COA]", txtSearchSegCoa.Text);
        SegCoaFind(CoaCode.ToString());
    }
}
