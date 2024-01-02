using System;
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
using System.Collections.Generic;
using System.Data.SqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Delve;
//using cins;

//using CrystalDecisions.CrystalReports.Engine;
//using CrystalDecisions.Shared;

public partial class Reports : System.Web.UI.Page
{
    public static Permis per;
    //public static ReportDocument rpt;    
    private static DataTable dtSegParent = new DataTable();
    private static DataTable dtSegChild = new DataTable();
    private static string book = "";
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
                        "Select user_grp,[description] from utl_userinfo where upper(user_name)=upper('" +
                        Session["user"].ToString().ToUpper() + "') and status='A'";
                    conn.Open();
                    dReader = cmd.ExecuteReader();
                    string wnot = "",userType="";
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            Session["userlevel"] = int.Parse(dReader["user_grp"].ToString());
                            //Session["dept"] = dReader["dept"].ToString();
                            wnot = "Welcome " + dReader["description"].ToString();
                          
                        }
                        Session["wnote"] = wnot;
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        //cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type from gl_set_of_books where book_name='" + Session["book"].ToString() + "' ";
                       
                        {
                            Session["bookMAN"] = Session["book"].ToString();
                        }
                        cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type,ShortName from gl_set_of_books where book_name='" + Session["bookMAN"] + "' ";

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
                //((Label)Page.Master.FindControl("lblCountryName")).Text = Session["LoginCountry"].ToString();
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
        if (!IsPostBack)
        {
            lblCountry.Visible = lblBranch.Visible = ddlUserType.Visible = ddlBranchID.Visible = false;            
            dgSeg.DataSource = LvlManager.GetLevelsGrid();
            dgSeg.DataBind();
            Populate();
            TreeView1.CollapseAll();
            ddlRepType.AutoPostBack = true;
            txtStartDt.Text = System.DateTime.Now.AddDays(-2).ToString("dd/MM/yyyy");
            txtEndDt.Text = System.DateTime.Now.ToString("dd/MM/yyyy");
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtStartDt.Enabled = false;
            txtStartDt.BackColor = System.Drawing.Color.White;
            txtEndDt.Enabled = false;
            txtEndDt.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            lbRunReport.Attributes.Add("onclick", "OpenWindow()");
            ddlSegLvl.Items.Clear();
            string querySegLvl = "select '' lvl_code, '' lvl_desc  union select lvl_code,dbo.initcap(lvl_desc) lvl_desc from gl_level_type order by lvl_code";
            util.PopulationDropDownList(ddlSegLvl, "level", querySegLvl, "lvl_desc", "lvl_code");
            book = Session["book"].ToString();           
            txtStartDt.Attributes.Add("onBlur", "formatdate('" + txtStartDt.ClientID + "')");
            txtEndDt.Attributes.Add("onBlur", "formatdate('" + txtEndDt.ClientID + "')");
            AddDgSeg();
            ddlBranchID.DataSource = clsBranchManager.GetBranchInfosGrid("");
            ddlBranchID.DataTextField = "BranchName";
            ddlBranchID.DataValueField = "ID";
            ddlBranchID.DataBind();
            ddlBranchID.Items.Insert(0, "");
            Session["UserTypeID"] = Session["BranchID"] = null;
            TextBox1.Enabled = false;
            UpdatePanelReport.Update();
            UpdatePanelTree.Update();
        }        
      
    }

    private void AddDgSeg()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("seg_code", typeof(string));
        dt.Columns.Add("seg_desc", typeof(string));
        dt.Columns.Add("lvl_desc", typeof(string));
        DataRow dr = dt.NewRow();
        dr["seg_code"] = "1";
        dr["seg_desc"] = Session["org"].ToString();
        dr["lvl_desc"] = "Balance Segments";
        dt.Rows.Add(dr);
        DataRow dr1 = dt.NewRow();
        dr1["seg_code"] = "0000000";
        dr1["seg_desc"] = "All Natural Accounts";
        dr1["lvl_desc"] = "Natural Accounts";
        dt.Rows.Add(dr1);
        dgSeg.DataSource = dt;
        dgSeg.DataBind();
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
            TreeView1.Nodes.Add(newNode);
            if (row["rootleaf"].ToString() == "R")
            {
                PopChild(row["seg_coa_code"].ToString(), newNode);
            }
        }
    }
    public void PopChild(string segcode, TreeNode node)
    {
        DataTable dt = SegCoaManager.GetSegCoaChild("parent_code='" + segcode + "' order by convert(numeric,nullif(seg_coa_code,''))");
        TreeNode newNode;

        foreach (DataRow dr in dt.Rows)
        {
            newNode = new TreeNode();
            newNode.Text = dr["seg_coa_code"].ToString() + " - " + dr["seg_coa_desc"].ToString();
            newNode.Value = dr["seg_coa_code"].ToString();
            node.ChildNodes.Add(newNode);
            if (dr["rootleaf"].ToString() == "R")
            {
                PopChild(dr["seg_coa_code"].ToString(), newNode);
            }
        }
    }
    protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
    {
        SegCoa segcoa = SegCoaManager.getSegCoa(TreeView1.SelectedNode.Value.ToString());
        if (segcoa != null)
        {
            foreach (GridViewRow gvr in dgSeg.Rows)
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
                cmd.CommandText = "select lvl_code from gl_level_type where lvl_desc='" + gvr.Cells[0].Text.ToString().Trim() + "'";
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
                    txtdes.Text = segcoa.SegCoaDesc;
                                      
                }
            }
            if (ddlRepType.SelectedValue.Equals("IAES"))
            {
                if (string.IsNullOrEmpty(TextBox1.Text))
                {
                    TextBox1.Text = segcoa.GlSegCode;
                    txtdes.Text = segcoa.GlSegCode;
                }
                else
                {
                    if (!TextBox1.Text.Contains(segcoa.GlSegCode))
                    {
                        TextBox1.Text += "," + segcoa.GlSegCode;
                        txtdes.Text += "," + segcoa.GlSegCode;
                    }
                }
            }
        }
        UpdatePanelReport.Update();
       // UpdatePanelTree.Update();
    }

    protected void ddlRepType_SelectedIndexChanged(object sender, EventArgs e)
    {
        TextBox1.Enabled = true;
      
        if (ddlRepType.SelectedValue == "B" | ddlRepType.SelectedValue == "I" | ddlRepType.SelectedValue == "C" | ddlRepType.SelectedValue == "B1")
        {
            //txtStartDt.Enabled = true;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            if (!ddlRepType.SelectedValue.Equals("B"))
            {
                txtStartDt.Enabled = true;
                txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            }
            else
            {
                txtStartDt.Enabled = false;
                txtStartDt.BackColor = System.Drawing.Color.White;
                txtStartDt.Text = "01/01/2017";
            }
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;            
            txtRptSysId.Enabled = true;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            txtRptSysId.Text = DateTime.Now.ToString("yyMMddhhmmss");
           // TreeView1.Enabled = false;
            dgSeg.DataSource = LvlManager.GetLevelsGrid();
            dgSeg.DataBind();
            AddDgSeg();
        }
        else if (ddlRepType.SelectedValue == "B1" | ddlRepType.SelectedValue == "I1" | ddlRepType.SelectedValue == "C1")
        {
            txtRptSysId.Enabled = true;
            txtRptSysId.BackColor = System.Drawing.Color.SkyBlue;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtStartDt.Enabled = false;
            txtStartDt.BackColor = System.Drawing.Color.White;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            Label9.Text = "Notes No";
            txtRptSysId.Text = DateTime.Now.ToString("yyMMddhhmmss");
            TreeView1.Enabled = false;
            AddDgSeg();
        }
        else if (ddlRepType.SelectedValue == "3" || ddlRepType.SelectedValue == "4" || ddlRepType.SelectedValue == "RP")
        {
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;            
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            string field = "";
            string CashBankCode = "";
            if (ddlRepType.SelectedValue == "3")
            {
                field = "cash_code";
            }
            else
            {
                field = "bank_code";
            }
            string connectionString = DataManager.OraConnString();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlDataReader dReader;
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select " + field + " codes from gl_set_of_books where book_name='" + book + "'";
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    CashBankCode = dReader["codes"].ToString();
                }
            }
            cmd.Dispose(); dReader.Close();
            if (conn.State != ConnectionState.Open)
            {
                conn.Close();
                SqlConnection.ClearPool(conn);
            }
            int i = 0;
            foreach (GridViewRow gvr in dgSeg.Rows)
            {
                if (i != 0)
                {
                    gvr.Cells[1].Text = CashBankCode;
                    gvr.Cells[2].Text = "All " + field.Replace('_', ' ') + "s";
                }
                i++;
                dReader.Close();
            }
            TreeView1.Enabled = true;
            UpdatePanelReport.Update();
            UpdatePanelTree.Update();
           // AddDgSeg();
        }
        else if (ddlRepType.SelectedValue == "5" )
        {
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;            
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            TreeView1.Enabled = true;
        }
        else if (ddlRepType.SelectedValue == "DB")
        {
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = true;
            ddlVchType.BackColor = System.Drawing.Color.SkyBlue;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            Session["UserTypeID"] = Session["BranchID"] = null;
            if (Session["userlevel"].ToString().Equals("5"))
            {
                lblCountry.Visible = lblBranch.Visible = ddlUserType.Visible = ddlBranchID.Visible = true;
            }
            else
            {
                lblCountry.Visible = lblBranch.Visible = ddlUserType.Visible = ddlBranchID.Visible = false;
            }
            TreeView1.Enabled = true;
            AddDgSeg();
            UpdatePanelReport.Update();
            UpdatePanelTree.Update();
        }
        else if (ddlRepType.SelectedValue == "DBS")
        {
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            TreeView1.Enabled = true;
            AddDgSeg();
        }
        else if (ddlRepType.SelectedValue == "6" | ddlRepType.SelectedValue == "7" | ddlRepType.SelectedValue == "8")
        {
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlRepLvl.Enabled = true;
            ddlRepLvl.BackColor = System.Drawing.Color.SkyBlue;
            ddlSegLvl.Enabled = true;
            ddlSegLvl.BackColor = System.Drawing.Color.SkyBlue;            
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            TreeView1.Enabled = true;
            dgSeg.DataSource = LvlManager.GetLevelsGrid();
            dgSeg.DataBind();
            AddDgSeg();
        }
        else if (ddlRepType.SelectedValue == "2")
        {
            ddlVchType.Enabled = true;
            ddlVchType.BackColor = System.Drawing.Color.SkyBlue;
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = true;
            txtNotesNo.BackColor = System.Drawing.Color.SkyBlue;
            Label9.Text = "Voucher No";
            AddDgSeg();
        }
        else if (ddlRepType.SelectedValue == "IAES") // Income And expance Statement.
        {
            //txtStartDt.Enabled = true;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;

            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = true;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            txtRptSysId.Text = DateTime.Now.ToString("yyMMddhhmmss");
            TreeView1.Enabled = true;
            dgSeg.DataSource = LvlManager.GetLevelsGrid();
            dgSeg.DataBind();
            AddDgSeg();
        }
        else if (ddlRepType.SelectedValue == "SCB")//************************ Shedile
        {
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            TreeView1.Enabled = true;

            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            string field = "";
            string CashBankCode = "";
            if (ddlRepType.SelectedValue == "3")
            {
                field = "cash_code";
            }
            else
            {
                field = "bank_code";
            }
            string connectionString = DataManager.OraConnString();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlDataReader dReader;
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select " + field + " codes from gl_set_of_books where book_name='" + book + "'";
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    CashBankCode = dReader["codes"].ToString();
                }
            }
            cmd.Dispose(); dReader.Close();
            if (conn.State != ConnectionState.Open)
            {
                conn.Close();
            }
            foreach (GridViewRow gvr in dgSeg.Rows)
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select lvl_seg_type from gl_level_type where lvl_code = " +
                    " (select lvl_code from gl_seg_coa where seg_coa_code = '" + gvr.Cells[1].Text.ToString().Trim().Replace("&nbsp;", "").ToString().Trim() + "')";
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                dReader = cmd.ExecuteReader();
                string mainseg = "";
                if (dReader.HasRows == true)
                {
                    while (dReader.Read())
                    {
                        mainseg = dReader["lvl_seg_type"].ToString();
                    }
                }
                if (mainseg == "N")
                {
                    gvr.Cells[1].Text = CashBankCode;
                    gvr.Cells[2].Text = "All " + field.Replace('_', ' ') + "s";
                }
                dReader.Close();
            }
            TreeView1.Enabled = true;
            txtdes.Text = ddlRepType.SelectedItem.Text;
            AddDgSeg();
        }

        else if (ddlRepType.SelectedValue == "TB")//************************ Shedile
        {
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            txtRptSysId.Text = DateTime.Now.ToString("yyMMddhhmmss");
            TreeView1.Enabled = true;
            dgSeg.DataSource = LvlManager.GetLevelsGrid();
            dgSeg.DataBind();
            AddDgSeg();
        }
        UpdatePanelReport.Update();
        UpdatePanelTree.Update();
    }
    
    public void ResetField()
    {
        ddlRepLvl.Enabled = false;
        ddlRepLvl.BackColor = System.Drawing.Color.White;
        ddlSegLvl.Enabled = false;
        ddlSegLvl.BackColor = System.Drawing.Color.White;
        ddlVchType.Enabled = false;
        ddlVchType.BackColor = System.Drawing.Color.White;
        txtStartDt.Enabled = false;
        txtStartDt.BackColor = System.Drawing.Color.White;
        txtEndDt.Enabled = false;
        txtEndDt.BackColor = System.Drawing.Color.White;
        txtRptSysId.Enabled = false;
        txtRptSysId.BackColor = System.Drawing.Color.White;
        txtNotesNo.Enabled = false;
        txtNotesNo.BackColor = System.Drawing.Color.White;
    }
    public void _LoadScript()
    {
        string jScript = "<script language='javascript'>";
        jScript += "OpenWindow();";
        jScript += "</script>";
        ClientScript.RegisterStartupScript(this.GetType(), "onclick", "jScript");
    }
    protected void lbRunReport_Click(object sender, EventArgs e)
    {
        if (ddlRepType.SelectedValue == "IAES")
        {
           
        }
        else if (ddlRepType.SelectedValue == "B" | ddlRepType.SelectedValue == "I" | ddlRepType.SelectedValue == "C" | ddlRepType.SelectedValue == "B1")
        {
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            if (!ddlRepType.SelectedValue.Equals("B"))
            {
                txtStartDt.Enabled = true;
                txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            }
            else
            {
                txtStartDt.Enabled = false;
                txtStartDt.BackColor = System.Drawing.Color.White;
                txtStartDt.Text = "01/01/2017";
            }
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            //txtStartDt.Enabled = true;
            //txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtRptSysId.Enabled = true;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            txtRptSysId.Text = DateTime.Now.ToString("yyMMddhhmmss");
            TreeView1.Enabled = false;
            dgSeg.DataSource = LvlManager.GetLevelsGrid();
            dgSeg.DataBind();
        }
        else if (ddlRepType.SelectedValue == "B1" | ddlRepType.SelectedValue == "I1" | ddlRepType.SelectedValue == "C1")
        {
            txtRptSysId.Enabled = true;
            txtRptSysId.BackColor = System.Drawing.Color.SkyBlue;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtStartDt.Enabled = false;
            txtStartDt.BackColor = System.Drawing.Color.White;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            Label9.Text = "Notes No";
            txtRptSysId.Text = DateTime.Now.ToString("yyMMddhhmmss");
            TreeView1.Enabled = false;
        }
        else if (ddlRepType.SelectedValue == "3" | ddlRepType.SelectedValue == "4")
        {
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            string field = "";
            string CashBankCode = "";
            if (ddlRepType.SelectedValue == "3")
            {
                field = "cash_code";
            }
            else
            {
                field = "bank_code";
            }
            string connectionString = DataManager.OraConnString();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlDataReader dReader;
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select " + field + " codes from gl_set_of_books where book_name='" + book + "'";
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    CashBankCode = dReader["codes"].ToString();
                }
            }
            cmd.Dispose(); dReader.Close();
            if (conn.State != ConnectionState.Open)
            {
                conn.Close();
                SqlConnection.ClearPool(conn);
            }
            foreach (GridViewRow gvr in dgSeg.Rows)
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select lvl_seg_type from gl_level_type where lvl_code = " +
                    " (select lvl_code from gl_seg_coa where seg_coa_code = '" + gvr.Cells[1].Text.ToString().Trim().Replace("&nbsp;", "").ToString().Trim() + "')";
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                dReader = cmd.ExecuteReader();
                string mainseg = "";
                if (dReader.HasRows == true)
                {
                    while (dReader.Read())
                    {
                        mainseg = dReader["lvl_seg_type"].ToString();
                    }
                }
                if (mainseg == "N")
                {
                    gvr.Cells[1].Text = CashBankCode;
                    gvr.Cells[2].Text = "All " + field.Replace('_', ' ') + "s";
                }
                dReader.Close();
            }
            TreeView1.Enabled = false;
        }
        else if (ddlRepType.SelectedValue == "5")
        {
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
           
            DataTable dt = VouchManager.GetshowGlCoa(TextBox1.Text);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                Index1 = row["GL_COA_CODE"].ToString().IndexOf("-") + 1;
                int l = row["GL_COA_CODE"].ToString().Length;
                int t = (l - (Index1));

                dgSeg.Rows[1].Cells[1].Text = row["GL_COA_CODE"].ToString().Substring(Index1, t);
                int totalLenth = row["COA_DESC"].ToString().Length;
                Index = row["COA_DESC"].ToString().IndexOf(",") + 1;
                int tot = (totalLenth - (Index));
                dgSeg.Rows[1].Cells[2].Text = row["COA_DESC"].ToString().Substring(Index, tot);
                txtdes.Text = row["COA_DESC"].ToString().Substring(Index, tot);

            }
            TreeView1.Enabled = false;
            UpdatePanelReport.Update();
            UpdatePanelTree.Update();
        }
        else if (ddlRepType.SelectedValue == "DB")
        {
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = true;
            ddlVchType.BackColor = System.Drawing.Color.SkyBlue;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            TreeView1.Enabled = true;
        }
        else if (ddlRepType.SelectedValue == "DBS")
        {
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            TreeView1.Enabled = true;
        }
        else if (ddlRepType.SelectedValue == "6" | ddlRepType.SelectedValue == "7" || ddlRepType.SelectedValue == "8")
        {
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlRepLvl.Enabled = true;
            ddlRepLvl.BackColor = System.Drawing.Color.SkyBlue;
            ddlSegLvl.Enabled = true;
            ddlSegLvl.BackColor = System.Drawing.Color.SkyBlue;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            TreeView1.Enabled = true;
            dgSeg.DataSource = LvlManager.GetLevelsGrid();
            dgSeg.DataBind();
        }
        else if (ddlRepType.SelectedValue == "2")
        {
            ddlVchType.Enabled = true;
            ddlVchType.BackColor = System.Drawing.Color.SkyBlue;
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = true;
            txtNotesNo.BackColor = System.Drawing.Color.SkyBlue;
            Label9.Text = "Voucher No";
            TreeView1.Enabled = false;
        }
        else if (ddlRepType.SelectedValue == "SCB")//************************ Shedile
        {
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            TreeView1.Enabled = true;

            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            string field = "";
            string CashBankCode = "";
            if (ddlRepType.SelectedValue == "3")
            {
                field = "cash_code";
            }
            else
            {
                field = "bank_code";
            }
            string connectionString = DataManager.OraConnString();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            SqlDataReader dReader;
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select " + field + " codes from gl_set_of_books where book_name='" + book + "'";
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            dReader = cmd.ExecuteReader();
            if (dReader.HasRows == true)
            {
                while (dReader.Read())
                {
                    CashBankCode = dReader["codes"].ToString();
                }
            }
            cmd.Dispose(); dReader.Close();
            if (conn.State != ConnectionState.Open)
            {
                conn.Close();
            }
            foreach (GridViewRow gvr in dgSeg.Rows)
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select lvl_seg_type from gl_level_type where lvl_code = " +
                    " (select lvl_code from gl_seg_coa where seg_coa_code = '" + gvr.Cells[1].Text.ToString().Trim().Replace("&nbsp;", "").ToString().Trim() + "')";
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                dReader = cmd.ExecuteReader();
                string mainseg = "";
                if (dReader.HasRows == true)
                {
                    while (dReader.Read())
                    {
                        mainseg = dReader["lvl_seg_type"].ToString();
                    }
                }
                if (mainseg == "N")
                {
                    gvr.Cells[1].Text = CashBankCode;
                    gvr.Cells[2].Text = "All " + field.Replace('_', ' ') + "s";
                }
                dReader.Close();
            }
            TreeView1.Enabled = true;
        }

        else if (ddlRepType.SelectedValue == "TB")//************************ Shedile
        {
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Notes No";
            txtRptSysId.Text = DateTime.Now.ToString("yyMMddhhmmss");
            TreeView1.Enabled = true;
            //dgSeg.DataSource = LvlManager.GetLevelsGrid();
            //dgSeg.DataBind();
            //AddDgSeg();
            //Label9.Text = "Voucher No";


            //Label9.Text = "Notes No";
            //string field = "";
            //string CashBankCode = "";
            //if (ddlRepType.SelectedValue == "3")
            //{
            //    field = "cash_code";
            //}
            //else
            //{
            //    field = "bank_code";
            //}
            //string connectionString = DataManager.OraConnString();
            //SqlConnection conn = new SqlConnection();
            //conn.ConnectionString = connectionString;
            //SqlDataReader dReader;
            //SqlCommand cmd = new SqlCommand();
            //cmd = new SqlCommand();
            //cmd.Connection = conn;
            //cmd.CommandType = CommandType.Text;
            //cmd.CommandText = "select " + field + " codes from gl_set_of_books where book_name='" + book + "'";
            //if (conn.State != ConnectionState.Open)
            //{
            //    conn.Open();
            //}
            //dReader = cmd.ExecuteReader();
            //if (dReader.HasRows == true)
            //{
            //    while (dReader.Read())
            //    {
            //        CashBankCode = dReader["codes"].ToString();
            //    }
            //}
            //cmd.Dispose(); dReader.Close();
            //if (conn.State != ConnectionState.Open)
            //{
            //    conn.Close();
            //}
            //foreach (GridViewRow gvr in dgSeg.Rows)
            //{
            //    cmd = new SqlCommand();
            //    cmd.Connection = conn;
            //    cmd.CommandType = CommandType.Text;
            //    cmd.CommandText = "select lvl_seg_type from gl_level_type where lvl_code = " +
            //        " (select lvl_code from gl_seg_coa where seg_coa_code = '" + gvr.Cells[1].Text.ToString().Trim().Replace("&nbsp;", "").ToString().Trim() + "')";
            //    if (conn.State != ConnectionState.Open)
            //    {
            //        conn.Open();
            //    }
            //    dReader = cmd.ExecuteReader();
            //    string mainseg = "";
            //    if (dReader.HasRows == true)
            //    {
            //        while (dReader.Read())
            //        {
            //            mainseg = dReader["lvl_seg_type"].ToString();
            //        }
            //    }
            //    if (mainseg == "N")
            //    {
            //        gvr.Cells[1].Text = CashBankCode;
            //        gvr.Cells[2].Text = "All " + field.Replace('_', ' ') + "s";
            //    }
            //    dReader.Close();
            //}
            //TreeView1.Enabled = true;
            //conn.Close();
            //dReader.Close();


            //SegCoa segcoa = SegCoaManager.getSegCoa("1103000");
            //if (segcoa != null)
            //{
            //    foreach (GridViewRow gvr in dgSeg.Rows)
            //    {
            //        string lvl = "";
            //        string connectionString1 = DataManager.OraConnString();
            //        SqlDataReader dReader1;
            //        SqlConnection conn1 = new SqlConnection();
            //        conn1.ConnectionString = connectionString;
            //        SqlCommand cmd1 = new SqlCommand();
            //        cmd1 = new SqlCommand();
            //        cmd1.Connection = conn1;
            //        cmd1.CommandType = CommandType.Text;
            //        cmd1.CommandText = "select lvl_code from gl_level_type where lvl_desc='" + gvr.Cells[0].Text.ToString().Trim() + "'";
            //        conn1.Open();
            //        dReader1 = cmd1.ExecuteReader();
            //        if (dReader1.HasRows == true)
            //        {
            //            while (dReader1.Read())
            //            {
            //                lvl = dReader1["lvl_code"].ToString();
            //            }
            //        }
            //        if (lvl == segcoa.LvlCode)
            //        {
            //            gvr.Cells[1].Text = segcoa.GlSegCode;
            //            gvr.Cells[2].Text = segcoa.SegCoaDesc;
            //        }

            //        conn1.Close();
            //        dReader1.Close();
            //    }
            //}
            UpdatePanelReport.Update();
            UpdatePanelTree.Update();

        }
        else if (ddlRepType.SelectedValue == "BCS") //************************ Shedile
        {
            ddlVchType.Enabled = false;
            ddlVchType.BackColor = System.Drawing.Color.White;
            txtStartDt.Enabled = true;
            txtStartDt.BackColor = System.Drawing.Color.SkyBlue;
            txtEndDt.Enabled = true;
            txtEndDt.BackColor = System.Drawing.Color.SkyBlue;
            ddlRepLvl.Enabled = false;
            ddlRepLvl.BackColor = System.Drawing.Color.White;
            ddlSegLvl.Enabled = false;
            ddlSegLvl.BackColor = System.Drawing.Color.White;
            txtRptSysId.Enabled = false;
            txtRptSysId.BackColor = System.Drawing.Color.White;
            txtNotesNo.Enabled = false;
            txtNotesNo.BackColor = System.Drawing.Color.White;
            Label9.Text = "Voucher No";          

            
            UpdatePanelReport.Update();
            UpdatePanelTree.Update();

        }
    }

    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8));
    }
    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD));
    }
    protected void lbReset_Click(object sender, EventArgs e)
    {       
        Session.Remove("DESC");
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
        
    }
    int Index1; 
    int Index;
    protected void TextBox1_TextChanged(object sender, EventArgs e)
    {
        
        DataTable dt = VouchManager.GetshowGlCoa(TextBox1.Text);
        if (dt.Rows.Count > 0)
        {
            DataRow row = dt.Rows[0];
            Index1 = row["GL_COA_CODE"].ToString().IndexOf("-") + 1;
            int l = row["GL_COA_CODE"].ToString().Length;
            int t = (l - (Index1));

            dgSeg.Rows[1].Cells[1].Text = row["GL_COA_CODE"].ToString().Substring(Index1, t);
            int totalLenth = row["COA_DESC"].ToString().Length;
            Index = row["COA_DESC"].ToString().IndexOf(",") + 1;
            int tot = (totalLenth - (Index));
            dgSeg.Rows[1].Cells[2].Text = row["COA_DESC"].ToString().Substring(Index, tot);
            txtdes.Text = row["COA_DESC"].ToString().Substring(Index, tot);        

        }
    }

    protected void ddlUserType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlUserType.SelectedItem.Text))        
            Session["UserTypeID"] = null;       
        else
            Session["UserTypeID"] = ddlUserType.SelectedValue;        
    }
    protected void ddlBranchID_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlUserType.SelectedItem.Text))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Select Country First.!!');", true);
            ddlBranchID.SelectedIndex = -1;
            Session["BranchID"] = null;
            return;
        }
        else
            Session["BranchID"] = ddlBranchID.SelectedValue;
    }
}
