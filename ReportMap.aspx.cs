using System;
using System.Collections;
using System.Configuration;
using System.Data;
//using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
//using System.Xml.Linq;
using System.Data.SqlClient;
using Delve;

//using mems;


public partial class ReportMap : System.Web.UI.Page
{
    public static Permis per;
    
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
                        "Select user_grp,[description],UserType,case when UserType=1 then 'Bangladesh' else 'Philippine' end AS[LoginCountry] from utl_userinfo where upper(user_name)=upper('" +
                        Session["user"].ToString().ToUpper() + "') and status='A'";
                    conn.Open();
                    dReader = cmd.ExecuteReader();
                    string wnot = "", userType = "";
                    if (dReader.HasRows == true)
                    {
                        while (dReader.Read())
                        {
                            Session["userlevel"] = int.Parse(dReader["user_grp"].ToString());
                            //Session["dept"] = dReader["dept"].ToString();
                            wnot = "Welcome " + dReader["description"].ToString();
                            Session["LoginCountry"] = dReader["LoginCountry"].ToString();
                            userType = dReader["UserType"].ToString();
                        }
                        Session["wnote"] = wnot;
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        //cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type from gl_set_of_books where book_name='" + Session["book"].ToString() + "' ";
                        if (Convert.ToInt32(userType) == 2)
                        {

                            Session["bookMAN"] = "MAN";
                        }
                        else
                        {
                            Session["bookMAN"] = Session["book"].ToString();
                        }
                        cmd.CommandText = "Select book_desc,company_address1,company_address2,separator_type,ShotName from gl_set_of_books where book_name='" + Session["bookMAN"] + "' ";

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
                ((Label)Page.Master.FindControl("lblCountryName")).Text = Session["LoginCountry"].ToString();
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
        if (!Page.IsPostBack)
        {
            DataTable dt = MapManager.GetMapMasters();
            if (dt.Rows.Count > 0)
            {
                dgMapMstLoad();
                dgMapMstBind();
            }
            else
            {
                getMapDtlGrid();
            }
        }
    }

     
    private void dgMapMstLoad()
    {
        DataTable dt = MapManager.GetMapMasters();
        dgMapMst.DataSource = dt;
    }
    private void dgMapMstBind()
    {
        dgMapMst.DataBind();
    }
    private void ClearField()
    {       
        txtReportType.Text = String.Empty;
        txtVerNo.Text = String.Empty;
        txtRefType.Text = String.Empty;
        txtRefVer.Text = String.Empty;
        txtMstDesc.Text = String.Empty;
        lblTranStatus.Visible = false;
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/ReportMap.aspx?mno=0.1");
    }
    protected void btnNew_Click(object sender, EventArgs e)
    {        
        ClearField();
        DetailShow();
        getMapDtlGrid();

    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (txtReportType.Text == String.Empty)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script language=JavaScript>alert('Please select a report type! ');</script>");
        }
        else
        {
            if (per.AllowEdit == "Y" & per.AllowAdd=="Y")
            {
                foreach (GridViewRow gvrd in tmpMapDtl.Rows)
                {
                    MapDtl mapd = MapManager.GetMapDetail(txtReportType.Text, gvrd.Cells[0].Text.Trim());
                    if (mapd != null)
                    {
                        MapManager.DeleteMapDetail(mapd);
                    }
                }
                MapMst mapmst = MapManager.GetMapMaster(txtReportType.Text);
                if (mapmst != null)
                {
                    if ((mapmst.RefVerNo != txtVerNo.Text) | (mapmst.RefTypeCode != txtRefType.Text) |
                        (mapmst.RefVerNo != txtRefVer.Text) | (mapmst.Description != txtMstDesc.Text))
                    {
                        mapmst.VerNo = txtVerNo.Text;
                        mapmst.RefTypeCode = txtRefType.Text;
                        mapmst.RefVerNo = txtRefVer.Text;
                        mapmst.Description = txtMstDesc.Text;
                        MapManager.UpdateMapMaster(mapmst);
                        dgMapDtl.EditIndex = -1;
                        dgMapDtl.FooterRow.Visible = false;
                        MapManager.DeleteMapBreaks(mapmst.TypeCode);
                        foreach (GridViewRow gvr in dgMapDtl.Rows)
                        {
                            if (((Label)gvr.FindControl("lblSlNo")).Text.Trim() != "")
                            {
                                MapDtl mapdtl = MapManager.GetMapDetail(mapmst.TypeCode, ((Label)gvr.FindControl("lblSlNo")).Text.Trim());
                                if (mapdtl != null)
                                {
                                    if ((((Label)gvr.FindControl("lblGlSegCode")).Text.Trim() != mapdtl.GlSegCode) |
                                (((Label)gvr.FindControl("lblGlDesc")).Text.Trim() != mapdtl.Description) |
                                (((Label)gvr.FindControl("lblBalFrom")).Text.Trim() != mapdtl.BalFrom) |
                                (((Label)gvr.FindControl("lblAddLess")).Text.Trim() != mapdtl.AddLess) |
                                (((Label)gvr.FindControl("lblConsAmt")).Text.Trim() != mapdtl.ConsAmt))
                                    {
                                        mapdtl.GlSegCode = ((Label)gvr.FindControl("lblGlSegCode")).Text.Trim();
                                        mapdtl.Description = ((Label)gvr.FindControl("lblGlDesc")).Text.Trim();
                                        mapdtl.BalFrom = ((Label)gvr.FindControl("lblBalFrom")).Text.Trim();
                                        mapdtl.AddLess = ((Label)gvr.FindControl("lblAddLess")).Text.Trim();
                                        mapdtl.ConsAmt = ((Label)gvr.FindControl("lblConsAmt")).Text.Trim();
                                        MapManager.UpdateMapDetail(mapdtl);
                                        GridView gv = (GridView)gvr.FindControl("dgMapBreak");
                                        gv.EditIndex = -1;
                                        gv.FooterRow.Visible = false;
                                        foreach (GridViewRow gvrb in gv.Rows)
                                        {
                                            if (((Label)gvrb.FindControl("lblBrkSlNo")).Text.Trim() != "")
                                            {
                                                MapBreak mapbrk = new MapBreak();
                                                mapbrk.TypeCode = mapmst.TypeCode;
                                                mapbrk.BookName = mapmst.BookName;
                                                mapbrk.VerNo = mapmst.VerNo;
                                                mapbrk.RefSlNo = mapdtl.SlNo;
                                                mapbrk.SlNo = ((Label)gvrb.FindControl("lblBrkSlNo")).Text.Trim();
                                                mapbrk.GlSegCode = ((Label)gvrb.FindControl("lblGl_Coa_Code")).Text.Trim();
                                                mapbrk.ADD_LESS = "";
                                                //mapbrk.ADD_LESS = ((Label)gvrb.FindControl("lblAddLess")).Text.Trim();
                                                MapManager.CreateMapBreak(mapbrk);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    mapdtl = new MapDtl();
                                    mapdtl.BookName = mapmst.BookName;
                                    mapdtl.TypeCode = mapmst.TypeCode;
                                    mapdtl.VerNo = mapmst.VerNo;
                                    mapdtl.SlNo = ((Label)gvr.FindControl("lblSlNo")).Text.Trim();
                                    mapdtl.GlSegCode = ((Label)gvr.FindControl("lblGlSegCode")).Text.Trim();
                                    mapdtl.Description = ((Label)gvr.FindControl("lblGlDesc")).Text.Trim();
                                    mapdtl.BalFrom = ((Label)gvr.FindControl("lblBalFrom")).Text.Trim();
                                    mapdtl.AddLess = ((Label)gvr.FindControl("lblAddLess")).Text.Trim();
                                    mapdtl.ConsAmt = ((Label)gvr.FindControl("lblConsAmt")).Text.Trim();
                                    MapManager.CreateMapDetail(mapdtl);
                                    GridView gv = (GridView)gvr.FindControl("dgMapBreak");
                                    gv.EditIndex = -1;
                                    gv.FooterRow.Visible = false;
                                    foreach (GridViewRow gvrb in gv.Rows)
                                    {
                                        if (((Label)gvrb.FindControl("lblBrkSlNo")).Text.Trim() != "")
                                        {
                                            MapBreak mapbrk = new MapBreak();
                                            mapbrk.TypeCode = mapmst.TypeCode;
                                            mapbrk.BookName = mapmst.BookName;
                                            mapbrk.VerNo = mapmst.VerNo;
                                            mapbrk.RefSlNo = mapdtl.SlNo;
                                            mapbrk.SlNo = ((Label)gvrb.FindControl("lblBrkSlNo")).Text.Trim();
                                            mapbrk.GlSegCode = "";
                                            mapbrk.ADD_LESS = "";
                                            MapManager.CreateMapBreak(mapbrk);
                                        }
                                    }
                                }
                                
                            }
                        }
                    }
                }
                else
                {
                    mapmst = new MapMst();
                    mapmst.TypeCode = txtReportType.Text;
                    mapmst.VerNo = txtVerNo.Text;
                    mapmst.RefTypeCode = txtRefType.Text;
                    mapmst.RefVerNo = txtRefVer.Text;
                    mapmst.Description = txtMstDesc.Text;
                    mapmst.BookName = Session["book"].ToString();
                    MapManager.CreateMapMaster(mapmst);
                    foreach (GridViewRow gvr in dgMapDtl.Rows)
                    {
                        if (((Label)gvr.FindControl("lblSlNo")).Text.Trim() != "")
                        {
                            MapDtl mapdtl = new MapDtl();
                            mapdtl.BookName = mapmst.BookName;
                            mapdtl.TypeCode = mapmst.TypeCode;
                            mapdtl.VerNo = mapmst.VerNo;
                            mapdtl.SlNo = ((Label)gvr.FindControl("lblSlNo")).Text.Trim();
                            mapdtl.GlSegCode = ((Label)gvr.FindControl("lblGlSegCode")).Text.Trim();
                            mapdtl.Description = ((Label)gvr.FindControl("lblGlDesc")).Text.Trim();
                            mapdtl.BalFrom = ((Label)gvr.FindControl("lblBalFrom")).Text.Trim();
                            mapdtl.AddLess = ((Label)gvr.FindControl("lblAddLess")).Text.Trim();
                            mapdtl.ConsAmt = ((Label)gvr.FindControl("lblConsAmt")).Text.Trim();
                            MapManager.CreateMapDetail(mapdtl);
                            GridView gv = (GridView)gvr.FindControl("dgMapBreak");
                            gv.EditIndex = -1;
                            gv.FooterRow.Visible = false;
                            foreach (GridViewRow gvrb in gv.Rows)
                            {
                                if (((Label)gvrb.FindControl("lblBrkSlNo")).Text.Trim() != "")
                                {
                                    MapBreak mapbrk = new MapBreak();
                                    mapbrk.TypeCode = mapmst.TypeCode;
                                    mapbrk.BookName = mapmst.BookName;
                                    mapbrk.VerNo = mapmst.VerNo;
                                    mapbrk.RefSlNo = mapdtl.SlNo;
                                    mapbrk.SlNo = ((Label)gvrb.FindControl("lblBrkSlNo")).Text.Trim();
                                    mapbrk.GlSegCode = "";
                                    MapManager.CreateMapBreak(mapbrk);
                                }
                            }
                        }
                    }
                }                
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script language=JavaScript>alert('Records are saved successfully!! ');</script>");
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script language=JavaScript>alert('You have no enough permission to edit these data!! ');</script>");
            }
        }
    }
        
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            if (txtReportType.Text == String.Empty)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script language=JavaScript>alert('Please select a report type! ');</script>");
            }
            else
            {
                MapManager.DeleteMapBreaks(txtReportType.Text);
                MapManager.DeleteMapDetails(txtReportType.Text);
                MapMst mapmst = MapManager.GetMapMaster(txtReportType.Text);
                MapManager.DeleteMapMaster(mapmst);
                btnClear_Click(sender, e);
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script language=JavaScript>alert('Deleted Successfully! ');</script>");
            }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script language=JavaScript>alert('You have no enough permission to delete these data! ');</script>");
        }        
    }
    protected void dgMapMst_SelectedIndexChanged(object sender, EventArgs e)
    {
       
        DetailShow();
        txtReportType.Text = dgMapMst.SelectedRow.Cells[1].Text.ToString();
        MapMst mapmst = MapManager.GetMapMaster(txtReportType.Text);
        txtVerNo.Text = mapmst.VerNo;
        txtMstDesc.Text = mapmst.Description;
        txtRefType.Text = mapmst.RefTypeCode;
        txtRefVer.Text = mapmst.RefVerNo;
        DataTable dtmapdtl = MapManager.GetMapDetails(txtReportType.Text);
        if (dtmapdtl.Rows.Count == 0)
        {
            getMapDtlGrid();
        }
        else
        {
           
            //DataTable dtmapdtl = MapManager.GetMapDetails(txtReportType.Text);
            DataTable dtMapDtl = new DataTable();
            dtMapDtl.Columns.Add("sl_no", typeof(string));
            dtMapDtl.Columns.Add("gl_seg_code", typeof(string));
            dtMapDtl.Columns.Add("description", typeof(string));
            dtMapDtl.Columns.Add("bal_from", typeof(string));
            dtMapDtl.Columns.Add("add_less", typeof(string));
            dtMapDtl.Columns.Add("cons_amt", typeof(string));
            dtMapDtl.Columns.Add("dtMapBreak", typeof(DataTable));
            DataRow drm;
            DataTable dtMapBreak;
            foreach (DataRow dr in dtmapdtl.Rows)
            {
                drm = dtMapDtl.NewRow();
                drm["sl_no"] = dr["sl_no"].ToString();
                drm["gl_seg_code"] = dr["gl_seg_code"].ToString();
                drm["description"] = dr["description"].ToString();
                drm["bal_from"] = dr["bal_from"].ToString();
                drm["add_less"] = dr["add_less"].ToString();
                drm["cons_amt"] = dr["cons_amt"].ToString();
                dtMapBreak = MapManager.GetMapBreaks(txtReportType.Text, dr["sl_no"].ToString());
                drm["dtMapBreak"] = dtMapBreak;
                dtMapDtl.Rows.Add(drm);
            }
            if (dtMapDtl.Rows.Count == 0)
            {
                DataRow dr = dtMapDtl.NewRow();
                dtMapDtl.Rows.Add(dr);
            }
            dgMapDtl.DataSource = dtMapDtl;
            dgMapDtl.DataBind();

            if (dtMapDtl.Rows.Count == 0)
            {
                
                dgMapDtl.FooterRow.Visible = true;
                dgMapDtl.ShowFooter = true;
            }
            else
            {
                dgMapDtl.FooterRow.Visible = false;
                dgMapDtl.ShowFooter = false;
            }
            Session["mapdtl"] = dtMapDtl;

        }
       
    }
    private void MasterShow()
    {
        dgMapDtl.Visible = false;
        dgMapMst.Visible = true;
       
    }
    private void DetailShow()
    {
        dgMapMst.Visible = false;
        dgMapDtl.Visible = true;
    }
    private void getMapDtlGrid()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("expCol", typeof(string));
        dt.Columns.Add("sl_no", typeof(string));
        dt.Columns.Add("gl_seg_code", typeof(string));
        dt.Columns.Add("description", typeof(string));
        dt.Columns.Add("add_less", typeof(string));
        dt.Columns.Add("bal_from", typeof(string));
        dt.Columns.Add("cons_amt", typeof(string));
        dt.Columns.Add("dtMapBreak", typeof(DataTable));
        DataTable dtMapBreak = new DataTable();
        dtMapBreak.Columns.Add("ref_sl_no", typeof(string));
        dtMapBreak.Columns.Add("sl_no", typeof(string));
        dtMapBreak.Columns.Add("gl_seg_code", typeof(string));
        DataRow dr = dt.NewRow();
        dr["sl_no"] = "";
        dr["gl_seg_code"] = "";
        dr["description"] = "";
        dr["add_less"] = "";
        dr["bal_from"] = "";
        dr["cons_amt"] = "";
        dr["dtMapBreak"] = dtMapBreak;
        dt.Rows.Add(dr);
        dgMapDtl.DataSource = dt;
        dgMapDtl.EditIndex = -1;
        dgMapDtl.ShowFooter = true;
        dgMapDtl.DataBind();
        ((TextBox)dgMapDtl.FooterRow.FindControl("txtSlNO")).Text = "1";
        Session["mapdtl"] = dt;
    }
    protected void dgMapDtl_CancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        if (Session["mapdtl"] != null)
        {
            DataTable dtMapDtl = (DataTable)Session["mapdtl"];
            DataRow dr;
            if (dtMapDtl.Rows.Count == 0)
            {
                dr = dtMapDtl.NewRow();
                dtMapDtl.Rows.Add(dr);
            }
            dgMapDtl.DataSource = dtMapDtl;
            if (dtMapDtl.Rows.Count > 0)
            {
                dgMapDtl.EditIndex = -1;
            }
            dgMapDtl.DataBind();
            if (dtMapDtl.Rows.Count > 1)
            {
                dgMapDtl.FooterRow.Visible = false;
                dgMapDtl.ShowFooter = false;
            }
            else if (dtMapDtl.Rows.Count == 1 && ((DataRow)dtMapDtl.Rows[0])["sl_no"].ToString() != "")
            {
                dgMapDtl.FooterRow.Visible = false;
                dgMapDtl.ShowFooter = false;
            }
            else
            {
                dgMapDtl.FooterRow.Visible = true;
                dgMapDtl.ShowFooter = true;
                ((TextBox)dgMapDtl.FooterRow.FindControl("txtSlNo")).Text = "1";
            }
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Your session is over. Please do these again!!');", true);
        }
    }
    protected void dgMapDtl_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (Session["mapdtl"] != null)
        {
            string msr = ((Label)dgMapDtl.Rows[e.NewEditIndex].FindControl("lblAddLess")).Text.ToString();
            string balf = ((Label)dgMapDtl.Rows[e.NewEditIndex].FindControl("lblBalFrom")).Text.ToString();
            DataTable dtMapDtl = (DataTable)Session["mapdtl"];
            dgMapDtl.DataSource = dtMapDtl;
            dgMapDtl.EditIndex = e.NewEditIndex;
            dgMapDtl.DataBind();
            ((DropDownList)dgMapDtl.Rows[e.NewEditIndex].FindControl("ddlAddLess")).SelectedIndex = ((DropDownList)dgMapDtl.Rows[e.NewEditIndex].FindControl("ddlAddLess")).Items.IndexOf(((DropDownList)dgMapDtl.Rows[e.NewEditIndex].FindControl("ddlAddLess")).Items.FindByValue(msr));
            ((DropDownList)dgMapDtl.Rows[e.NewEditIndex].FindControl("ddlBalFrom")).SelectedIndex = ((DropDownList)dgMapDtl.Rows[e.NewEditIndex].FindControl("ddlBalFrom")).Items.IndexOf(((DropDownList)dgMapDtl.Rows[e.NewEditIndex].FindControl("ddlBalFrom")).Items.FindByValue(balf));
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Your session is over. Please do these again!!');", true);
        }
    }
    protected void dgMapDtl_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {        
        if (Session["mapdtl"] != null)
        {
            GridViewRow gvr = dgMapDtl.Rows[e.RowIndex];
            DataTable dtMapDtl = (DataTable)Session["mapdtl"];
            DataRow dr = dtMapDtl.Rows[dgMapDtl.Rows[e.RowIndex].DataItemIndex];
            dr["sl_no"] = ((TextBox)gvr.FindControl("txtSlNo")).Text;
            dr["gl_seg_code"] = ((TextBox)gvr.FindControl("txtGlSegCode")).Text;
            dr["description"] = ((TextBox)gvr.FindControl("txtGlDesc")).Text;
            dr["add_less"] = ((DropDownList)gvr.FindControl("ddlAddLess")).SelectedValue;
            dr["bal_from"] = ((DropDownList)gvr.FindControl("ddlBalFrom")).SelectedValue;
            dr["cons_amt"] = ((TextBox)gvr.FindControl("txtConsAmt")).Text;
            dgMapDtl.DataSource = dtMapDtl;
            dgMapDtl.EditIndex = -1;
            dgMapDtl.DataBind();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Your session is over. Please do these again!!');", true);
        }
    }

    protected void dgMapDtl_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {

        //dtMapBreak.Clear();

        //int rowno = e.NewSelectedIndex + 2;
        //GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
        //TableCell cell = new TableCell();
        //cell.ColumnSpan = 4;                      
        //dtMapBreak = MapManager.GetMapBreaks(txtReportType.Text, ((Label)dgMapDtl.Rows[e.NewSelectedIndex].Cells[0].FindControl("lblSlNo")).Text);                           

        //dgMapBreak.DataSource = dtMapBreak;

        //dgMapBreak.DataBind();

        //cell.Controls.Add(dgMapBreak);
        //row.Cells.Add(cell);

        //dgMapDtl.Controls[0].Controls.AddAt(rowno, row);
        //dtMapBreak = MapManager.GetMapBreaks(txtReportType.Text, ((Label)dgMapBreak.SelectedRow.FindControl("lblSlNo")).Text);        

    }

    protected void dgMapDtl_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (Session["mapdtl"] != null)
        {
            DataTable dtMapDtl = (DataTable)Session["mapdtl"];
            DataRow drm = dtMapDtl.Rows[dgMapDtl.Rows[e.RowIndex].DataItemIndex];
            DataTable dt = (DataTable)drm["dtMapBreak"];
            if (dt.Rows.Count > 1)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('You cannot delete this map while detail mapping is exist!!');", true);
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["sl_no"].ToString()!="")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('You cannot delete this map while detail mapping is exist!!');", true);
            }
            else
            {
                DataRow dr;
                DataTable dtmap = new DataTable();
                dtmap.Columns.Add("sl_no", typeof(string));
                dtmap.Columns.Add("gl_seg_code", typeof(string));
                foreach (GridViewRow gvt in tmpMapDtl.Rows)
                {
                    dr = dtmap.NewRow();
                    dr["sl_no"] = gvt.Cells[0].Text.Trim();
                    dr["gl_seg_code"] = gvt.Cells[1].Text.Trim();
                    dtmap.Rows.Add(dr);
                }
                dr = dtmap.NewRow();
                dr["sl_no"] = drm["sl_no"].ToString();
                dr["gl_seg_code"] = drm["gl_seg_code"].ToString();
                dtmap.Rows.Add(dr);
                tmpMapDtl.DataSource = dtmap;
                tmpMapDtl.DataBind();
                dtMapDtl.Rows.Remove(drm);
            }
            if (dtMapDtl.Rows.Count > 0)
            {
                dgMapDtl.DataSource = dtMapDtl;
                dgMapDtl.EditIndex = -1;
                dgMapDtl.DataBind();
            }
            else
            {
                getMapDtlGrid();
            }
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Your session is over. Please do these again!!');", true);
        }
    }

    protected void dgMapBreak_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {        
        if (Session["mapdtl"] != null)
        {
            GridView gv1 = (GridView)sender;
            GridViewRow gvrow = (GridViewRow)gv1.NamingContainer;
            DataTable dtMapDtl = (DataTable)Session["mapdtl"];
            DataRow drm = dtMapDtl.Rows[gvrow.DataItemIndex];
            DataTable dtMapBreak = (DataTable)drm["dtMapBreak"];
            dtMapBreak.Rows.RemoveAt(gv1.Rows[e.RowIndex].DataItemIndex);
            DataTable dtmapbrk = new DataTable();
            dtmapbrk.Columns.Add("ref_sl_no", typeof(string));
            dtmapbrk.Columns.Add("sl_no", typeof(string));
            dtmapbrk.Columns.Add("gl_seg_code", typeof(string));
            dtmapbrk.Columns.Add("ADD_LESS", typeof(string));
            DataRow dr2;
            foreach (GridViewRow gvt in tmpMapBreak.Rows)
            {
                if (gvt.Cells[0].Text.Trim() != "")
                {
                    dr2 = dtmapbrk.NewRow();
                    dr2["ref_sl_no"] = gvt.Cells[0].Text.Trim();
                    dr2["sl_no"] = gvt.Cells[1].Text.Trim();
                    dr2["gl_seg_code"] = gvt.Cells[2].Text.Trim();
                    dr2["ADD_LESS"] = gvt.Cells[3].Text.Trim();
                    dtmapbrk.Rows.Add(dr2);
                }
            }
            dr2 = dtmapbrk.NewRow();
            dr2["ref_sl_no"] = ((Label)gv1.Rows[e.RowIndex].FindControl("lblBrkRefSlNo")).Text;
            dr2["sl_no"] = ((Label)gv1.Rows[e.RowIndex].FindControl("lblBrkSlNo")).Text;
            dr2["gl_seg_code"] = ((Label)gv1.Rows[e.RowIndex].FindControl("lblGl_Coa_Code")).Text;
           // dr2["ADD_LESS"] = ((Label)gv1.Rows[e.RowIndex].FindControl("lblAddLess")).Text;
            dtmapbrk.Rows.Add(dr2);
            tmpMapBreak.DataSource = dtmapbrk;
            tmpMapBreak.DataBind();
            dgMapDtl.DataSource = dtMapDtl;
            dgMapDtl.DataBind();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Your session is over. Please do these again!!');", true);
        }
    }
    protected void dgMapBreak_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (Session["mapdtl"] != null)
        {
            if (e.CommandName == "Add" | e.CommandName == "Insert")
            {
                GridView gv = (GridView)sender;
                GridViewRow gvrow = (GridViewRow)gv.NamingContainer;
                if (e.CommandName == "Add")
                {
                    DataTable dtMapDtl = (DataTable)Session["mapdtl"]; 
                    gv.FooterRow.Visible = true;
                    gv.ShowFooter = true;
                    gv.FooterRow.Cells[1].Text = ((Label)gvrow.FindControl("lblSlNo")).Text.ToString();
                    Session["MapBreakFooter"] = "Y";
                    Session["dgMapDtlEditIndexRow"] = ((Label)gvrow.FindControl("lblSlNo")).Text;
                    dgMapDtl.DataSource = dtMapDtl;
                    dgMapDtl.DataBind();
                }
                else if (e.CommandName == "Insert")
                {
                    try
                    {

                        DataTable dtMapDtl = (DataTable) Session["mapdtl"];
                        DataRow dr = dtMapDtl.Rows[gvrow.DataItemIndex];
                        DataTable dtMapBreak = (DataTable) dr["dtMapBreak"];
                        DataRow dr1 = dtMapBreak.NewRow();
                        dr1["ref_sl_no"] = ((Label) gvrow.FindControl("lblSlNo")).Text;
                        dr1["sl_no"] = ((TextBox) gv.FooterRow.FindControl("txtBrkSlNo")).Text;
                        dr1["gl_seg_code"] = ((TextBox) gv.FooterRow.FindControl("txtGl_Coa_Code")).Text;
                        dr1["ADD_LESS"] = ((DropDownList) gv.FooterRow.FindControl("ddlAddLess")).SelectedValue;
                        dtMapBreak.Rows.Add(dr1);
                        dgMapDtl.DataSource = dtMapDtl;
                        dgMapDtl.DataBind();
                    }
                    catch (Exception)
                    {

                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Sl No. Not Empty.\\n when you set empty please input 0 .!!');", true);
                        return;
                    }
                }
            }
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Your session is over. Please do these again!!');", true);
        }
    }
    protected void dgMapBreak_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        if (Session["mapdtl"] != null)
        {
            GridView gv = (GridView)sender;
            GridViewRow gvrow = (GridViewRow)gv.NamingContainer;
            DataTable dtMapDtl = (DataTable)Session["mapdtl"];
            gv.EditIndex = -1;
            dgMapDtl.DataSource = dtMapDtl;
            dgMapDtl.DataBind();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Your session is over. Please do these again!!');", true);
        }
    }
    protected void dgMapBreak_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (Session["mapdtl"] != null)
        {
            GridView gv = (GridView)sender;
            GridViewRow gvrow = (GridViewRow)gv.NamingContainer;
            DataTable dtMapDtl = (DataTable)Session["mapdtl"];
            Session["dgMapDtlEditIndexRow"] = ((Label)gvrow.FindControl("lblSlNo")).Text;
            Session["dgMapBreakEditIndex"] = e.NewEditIndex.ToString();
            dgMapDtl.DataSource = dtMapDtl;
            dgMapDtl.DataBind();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Your session is over. Please do these again!!');", true);
        }
    }
    
    protected void dgMapBreak_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (Session["mapdtl"] != null)
        {
            GridView gv = (GridView)sender;
            GridViewRow gvrow = (GridViewRow)gv.NamingContainer;
            DataTable dtMapDtl = (DataTable)Session["mapdtl"];
            DataRow drdtl = dtMapDtl.Rows[gvrow.DataItemIndex];
            DataTable dtMapBreak = (DataTable)drdtl["dtMapBreak"];
            DataRow dr = dtMapBreak.Rows[gv.Rows[e.RowIndex].DataItemIndex];
            dr["ref_sl_no"] = ((Label)gvrow.FindControl("lblSlNo")).Text;
            dr["sl_no"] = ((TextBox)gv.Rows[e.RowIndex].FindControl("txtBrkSlNo")).Text;
            dr["gl_seg_code"] = ((TextBox)gv.Rows[e.RowIndex].FindControl("txtGl_Coa_Code")).Text;
            dr["ADD_LESS"] = ((DropDownList)gv.Rows[e.RowIndex].FindControl("ddlAddLess")).SelectedValue;
            dgMapDtl.DataSource = dtMapDtl;
            dgMapDtl.DataBind();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Your session is over. Please do these again!!');", true);
        }
    }
    
    protected void dgMapDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {        
                if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((DataRowView)e.Row.DataItem)["sl_no"].ToString() == String.Empty)
            {
                e.Row.Visible = false;
            }
            else
            {
                GridView gv = (GridView)e.Row.FindControl("dgMapBreak");
                DataTable dtMapBreak = (DataTable)DataBinder.Eval(e.Row.DataItem, "dtMapBreak");

                if (dtMapBreak.Rows.Count == 0)
                {
                    
                    DataRow dr = dtMapBreak.NewRow();
                    dtMapBreak.Rows.Add(dr);
                }
                gv.DataSource = dtMapBreak;
                if (Session["dgMapDtlEditIndexRow"] != null && Session["dgMapBreakEditIndex"] != null)
                {
                    string rowMjr = Session["dgMapDtlEditIndexRow"].ToString();
                    if (rowMjr == ((DataRowView)e.Row.DataItem)["sl_no"].ToString())
                    {
                        int indxSubMjr = int.Parse(Session["dgMapBreakEditIndex"].ToString());
                        gv.EditIndex = indxSubMjr;
                        Session.Remove("dgMapDtlEditIndexRow");
                        Session.Remove("dgMapBreakEditIndex");
                    }
                }
                else
                {
                    gv.EditIndex = -1;
                }
                gv.DataBind();
                gv.BorderColor = System.Drawing.Color.Blue;
                gv.Caption = "Detail Mapping Operations";

                if (dtMapBreak.Rows.Count > 1)
                {
                    if (Session["dgMapDtlEditIndexRow"] != null && Session["MapBreakFooter"] != null)
                    {
                        string rowMjr = Session["dgMapDtlEditIndexRow"].ToString();
                        if (rowMjr == ((DataRowView)e.Row.DataItem)["sl_no"].ToString())
                        {
                            string mapbreakfooter = Session["MapBreakFooter"].ToString();
                            Session.Remove("dgMapDtlEditIndexRow");
                            Session.Remove("MapBreakFooter");
                            if (mapbreakfooter == "Y")
                            {
                                gv.FooterRow.Visible = true;
                                gv.ShowFooter = true;
                                ((Label)gv.FooterRow.FindControl("lblBrkRefSlNo")).Text = ((DataRowView)e.Row.DataItem)["sl_no"].ToString();
                            }
                            else
                            {
                                gv.FooterRow.Visible = false;
                                gv.ShowFooter = false;
                            }
                        }
                    }
                    else
                    {
                        gv.FooterRow.Visible = false;
                        gv.ShowFooter = false;
                    }
                }
                else if (dtMapBreak.Rows.Count == 1 && ((DataRow)dtMapBreak.Rows[0])[1].ToString() != "")
                {
                    if (Session["dgMapDtlEditIndexRow"] != null && Session["MapBreakFooter"] != null)
                    {
                        string rowMjr = Session["dgMapDtlEditIndexRow"].ToString();
                        if (rowMjr == ((DataRowView)e.Row.DataItem)["sl_no"].ToString())
                        {
                            string mapbreakfooter = Session["MapBreakFooter"].ToString();
                            Session.Remove("dgMapDtlEditIndexRow");
                            Session.Remove("MapBreakFooter");
                            if (mapbreakfooter == "Y")
                            {
                                gv.FooterRow.Visible = true;
                                gv.ShowFooter = true;
                                ((Label)gv.FooterRow.FindControl("lblBrkRefSlNo")).Text = ((DataRowView)e.Row.DataItem)["sl_no"].ToString();
                            }
                            else
                            {
                                gv.FooterRow.Visible = false;
                                gv.ShowFooter = false;
                            }
                        }
                    }
                    else
                    {
                        gv.FooterRow.Visible = false;
                        gv.ShowFooter = false;
                    }
                }
                else
                {
                    gv.FooterRow.Visible = true;
                    gv.ShowFooter = true;
                    ((Label)gv.FooterRow.FindControl("lblBrkRefSlNo")).Text = ((DataRowView)e.Row.DataItem)["sl_no"].ToString();
                }
            }
        }       
    }
    protected void dgMapBreak_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((DataRowView)e.Row.DataItem)["ref_sl_no"].ToString() == String.Empty)
            {
                e.Row.Visible = false;
            }
        }
    }
    protected void dgMapDtl_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (Session["mapdtl"] != null)
        {
            DataTable dtMapDtl = (DataTable)Session["mapdtl"];
            if (e.CommandName == "Add")
            {
                dgMapDtl.DataSource = dtMapDtl;
                dgMapDtl.DataBind();
                dgMapDtl.FooterRow.Visible = true;
                dgMapDtl.ShowFooter = true;
            }
            else if (e.CommandName == "Insert")
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ref_sl_no", typeof(string));
                dt.Columns.Add("sl_no", typeof(string));
                dt.Columns.Add("gl_seg_code", typeof(string));
                DataRow dr = dtMapDtl.NewRow();
                dr["sl_no"] = ((TextBox)dgMapDtl.FooterRow.FindControl("txtSlNo")).Text;
                dr["gl_seg_code"] = ((TextBox)dgMapDtl.FooterRow.FindControl("txtGlSegCode")).Text;
                dr["description"] = ((TextBox)dgMapDtl.FooterRow.FindControl("txtGlDesc")).Text;
                dr["add_less"] = ((DropDownList)dgMapDtl.FooterRow.FindControl("ddlAddLess")).SelectedValue;
                dr["bal_from"] = ((DropDownList)dgMapDtl.FooterRow.FindControl("ddlBalFrom")).SelectedValue;
                dr["cons_amt"] = ((TextBox)dgMapDtl.FooterRow.FindControl("txtConsAmt")).Text;
                dr["dtMapBreak"] = dt;
                dtMapDtl.Rows.Add(dr);
                dgMapDtl.DataSource = dtMapDtl;
                dgMapDtl.DataBind();
                dgMapDtl.FooterRow.Visible = false;
                dgMapDtl.ShowFooter = false;
            }
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('Your session is over. Please do these again!!');", true);
        }
    }
}