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
//using ICU;
//using CrystalDecisions.CrystalReports.Engine;
//using CrystalDecisions.Shared;
using System.Data.SqlClient;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.pdf.draw;
using System.Reflection;
using Delve;
using Delve;

public partial class EmpReport : System.Web.UI.Page
{
    //public static ReportDocument rpt;
    private static Permis per;
    EmpManager _aEmpManager=new EmpManager();
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
        //try
        //{
        //    string pageName = DataManager.GetCurrentPageName();
        //    string modid = PermisManager.getModuleId(pageName);
        //    per = PermisManager.getUsrPermis(Session["user"].ToString().Trim().ToUpper(), modid);
        //    if (per != null && per.AllowView == "Y")
        //    {
        //        ((Label)Page.Master.FindControl("lblLogin")).Text = Session["wnote"].ToString();
        //        ((Label)Page.Master.FindControl("lblCountryName")).Text = Session["LoginCountry"].ToString();
        //        ((LinkButton)Page.Master.FindControl("lbLogout")).Visible = true;
        //    }
        //    else
        //    {
        //        Response.Redirect("Home.aspx?sid=sam");
        //    }
        //}
        //catch
        //{
        //    Response.Redirect("Default.aspx?sid=sam");
        //}
        if (!IsPostBack)
        {
            
        }
    }

    protected void btnShow_Click(object sender, EventArgs e)
    {
        //System.Threading.Thread.Sleep(3000);
        //string cEmp="";
        string finCriteria = "";
        //lbBranchIncl_Click(sender, e);
        //lbDesigIncl_Click(sender, e);
        //lbEmpIncl_Click(sender, e);

        if (rdoSelectCriteria.SelectedValue == "Blank")
        {
            string filePath = "~/Help/" + "EmployeeInfo.pdf";
            string Name = "EmployeeInfo.pdf";
            Response.ContentType = "doc/docx";
            Response.AddHeader("Content-Disposition", "attachment;filename=\"" + Name + "\"");
            Response.TransmitFile(Server.MapPath(filePath));
            Response.End();
        }
        else
        {
        DataTable dtabEmp = new DataTable();
        if (rdoSelectCriteria.SelectedValue == "EMP")
        {
            if (Session["dtabemp"] != null)
            {
                dtabEmp = (DataTable)Session["dtabemp"];
            }
        }
        DataTable dtabThana = new DataTable();
        if (Session["dtabthana"] != null)
        {
            dtabThana = (DataTable)Session["dtabthana"];
        }
        DataTable dtabDis = new DataTable();
        if (Session["dtabDis"] != null)
        {
            dtabDis = (DataTable)Session["dtabDis"];
        }
        DataTable dtabBranch = new DataTable();
        if (Session["dtabbranch"] != null)
        {
             dtabBranch = (DataTable)Session["dtabbranch"];
        }
        DataTable dtabDesig = new DataTable();
        if (rdoSelectCriteria.SelectedValue == "DES")
        {
            if (Session["dtabdesig"] != null)
            {
                dtabDesig = (DataTable)Session["dtabdesig"];
            }
        }
        if (dtabEmp.Rows.Count == 0)
        {
            string cThana = "";
            string cDist = "";
            string cBranch = "";
            string cDesig = "";
            if (dtabBranch.Rows.Count > 0)
            {
                //foreach (DataRow dr in dtabBranch.Rows)
                //{
                //    cBranch += "'" + dr[0].ToString() + "',";
                //}
                //if (cBranch.Length > 0)
                //{
                //    cBranch = cBranch.Remove(cBranch.Length - 1, 1);
                //}
                foreach (GridViewRow gvr in dgBranch.Rows)
                {
                    if (((CheckBox)gvr.FindControl("chkInc")).Checked)
                    {
                        if (cBranch == "")
                        {
                            cBranch = gvr.Cells[1].Text.Trim();

                        }
                        else
                        {

                            cBranch = cBranch + "," + gvr.Cells[1].Text.Trim();
                        }
                    }
                }
                cBranch = "(" + cBranch + ")";
                finCriteria = finCriteria + " and t2.ID in " + cBranch;
            }
            if (dtabDesig.Rows.Count > 0)
            {
                foreach (DataRow dr in dtabDesig.Rows)
                {
                    cDesig += "'" + dr[0].ToString() + "',";
                }
                if (cDesig.Length > 0)
                {
                    cDesig = cDesig.Remove(cDesig.Length - 1, 1);
                }
                cDesig = "(" + cDesig + ")";
                finCriteria = finCriteria + " and a.JOIN_DESIG_CODE in " + cDesig;
            }
            if (finCriteria.Length > 5)
            {
                dtabEmp = EmpManager.getEmployeeRpt(finCriteria);
            }
            else
            {
                dtabEmp = EmpManager.getEmployeeRpt("");
            }
        }
        else if (dtabEmp.Rows.Count>0)
        {
            string EmpNo="";
            foreach (GridViewRow gvr in dgEmp.Rows)
            {
                if (((CheckBox)gvr.FindControl("chkInc")).Checked)
                {
                    if (EmpNo == "")
                    {
                        EmpNo = "'"+gvr.Cells[3].Text.Trim()+"'";
                    }
                    else
                    {

                        EmpNo = EmpNo + "," + "'" + gvr.Cells[3].Text.Trim() + "'";
                    }
                    // dr["desig_name"] = gvr.Cells[2].Text.Trim();
                    //  dtabDesig.Rows.Add(dr);
                }
                if (dtabDesig.Rows.Count > 0)
                {
                    //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Selected Designations are added in the list!!');", true);
                }
            }
            if (EmpNo != "")
            {
                dtabEmp = EmpManager.getEmployeeRpt(" and emp_no in (" + EmpNo + ")");
            }
            else
            {
                dtabEmp = EmpManager.getEmployeeRpt("");
            }
        }
        if (rdoReportType.SelectedValue == "R")
        {
            getEmpRptPdf(dtabEmp,"R");
        }
        else if (rdoReportType.SelectedValue == "D")
        {
            getEmpRptPdf(dtabEmp, "D");
        }
        else if (rdoReportType.SelectedValue == "SL")
        {
            string EmpID = "",BranchID="";
            string DesigID = "";
            if (rdoSelectCriteria.SelectedValue.Equals("EMP"))
            {

                foreach (GridViewRow gvr in dgEmp.Rows)
                {
                    if (((CheckBox)gvr.FindControl("chkInc")).Checked)
                    {
                        if (string.IsNullOrEmpty(EmpID))
                        {
                            EmpID = gvr.Cells[1].Text.Trim();
                        }
                        else
                        {
                            EmpID += "," + gvr.Cells[1].Text.Trim(); ;
                        }
                    }
                }
            }
            else if (rdoSelectCriteria.SelectedValue.Equals("DES"))
            {
                foreach (GridViewRow gvr in dgDesig.Rows)
                {
                    if (((CheckBox)gvr.FindControl("chkInc")).Checked)
                    {
                        if (DesigID == "")
                        {
                            DesigID = gvr.Cells[1].Text.Trim();

                        }
                        else
                        {

                            DesigID = DesigID + "," + gvr.Cells[1].Text.Trim();
                        }
                        // dr["desig_name"] = gvr.Cells[2].Text.Trim();
                        //  dtabDesig.Rows.Add(dr);
                    }
                    if (dtabDesig.Rows.Count > 0)
                    {
                        //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Selected Designations are added in the list!!');", true);
                    }
                }
            }

            if (rdoSelectCriteria.SelectedValue.Equals("BR"))
            {

                foreach (GridViewRow gvr in dgBranch.Rows)
                {
                    if (((CheckBox)gvr.FindControl("chkInc")).Checked)
                    {
                        if (BranchID == "")
                        {
                            BranchID = gvr.Cells[1].Text.Trim();

                        }
                        else
                        {

                            BranchID = BranchID + "," + gvr.Cells[1].Text.Trim();
                        }
                    }
                }

            }


            DataTable dtEmployee = _aEmpManager.GetShowEmployeeInformation(DesigID, EmpID, BranchID);
            getEmpSimpleRptPdf(dtEmployee);
        }
        } 
    }
    protected void rdoSelectCriteria_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblTranStatus.Visible = false;
        rdoReportType.Enabled = true;
        Session["dtabdesig"] = null;
        Session["dtabemp"] = null;
       
        dgDesig.DataSource = null;
        dgDesig.DataBind();

        dgSelectDesignation.DataSource = null;
        dgSelectDesignation.DataBind();
        dgEmp.DataSource = null;
        dgEmp.DataBind();
        if (rdoSelectCriteria.SelectedValue == "EMP")
        {
            pnlEmp.Visible = true;
            pnlDiv.Visible = false;
            pnlDis.Visible = false;
            pnlThana.Visible = false;
            pnlBranch.Visible = false;
            pnlDesig.Visible = false;
            getEmp();
            dgEmp.Caption = "Emplyee List";
            dgEmp.CaptionAlign = TableCaptionAlign.Top;
        }
        else if (rdoSelectCriteria.SelectedValue == "DIV")
        {
            pnlEmp.Visible = false;
            pnlDiv.Visible = true;
            pnlDis.Visible = false;
            pnlThana.Visible = false;
            pnlBranch.Visible = false;
            pnlDesig.Visible = false;
           
            getDiv();
            dgDiv.Caption = "Division List";
            dgDiv.CaptionAlign = TableCaptionAlign.Top;
        }
        else if (rdoSelectCriteria.SelectedValue == "DIS")
        {
            pnlEmp.Visible = false;
            pnlDiv.Visible = false;
            pnlDis.Visible = true;
            pnlThana.Visible = false;
            pnlBranch.Visible = false;
            Session["dtabemp"] = null;
            getDist();
            dgDis.Caption = "District List";
            dgDis.CaptionAlign = TableCaptionAlign.Top;
        }
        else if (rdoSelectCriteria.SelectedValue == "THN")
        {
            pnlEmp.Visible = false;
            pnlDiv.Visible = false;
            pnlDis.Visible = false;
            pnlThana.Visible = true;
            pnlBranch.Visible = false;
            pnlDesig.Visible = false;
            getThana();
            Session["dtabemp"] = null;
            dgDis.Caption = "Thana List";
            dgDis.CaptionAlign = TableCaptionAlign.Top;
        }
        else if (rdoSelectCriteria.SelectedValue == "BR")
        {
            pnlEmp.Visible = false;
            pnlDiv.Visible = false;
            pnlDis.Visible = false;
            pnlThana.Visible = false;
            pnlBranch.Visible = true;
            pnlDesig.Visible = false;
            pnlBranch.Visible = true;
            getBranch();
            dgBranch.Caption = "Branch/Unit Office List";
            dgBranch.CaptionAlign = TableCaptionAlign.Top;
        }
        else if (rdoSelectCriteria.SelectedValue == "DES")
        {
            pnlEmp.Visible = false;
            pnlDiv.Visible = false;
            pnlDis.Visible = false;
            pnlThana.Visible = false;
            pnlBranch.Visible = false;
            pnlDesig.Visible = true;
            getDesig();
            dgBranch.Caption = "Designation List";
            dgBranch.CaptionAlign = TableCaptionAlign.Top;
        }
        else if (rdoSelectCriteria.SelectedValue == "Blank")
        {
            pnlEmp.Visible = false;
            pnlDiv.Visible = false;
            pnlDis.Visible = false;
            pnlThana.Visible = false;
            pnlBranch.Visible = false;
            pnlDesig.Visible = false;
            rdoReportType.Enabled = false;
        }
        else
        {
            pnlEmp.Visible = false;
            pnlDiv.Visible = false;
            pnlDis.Visible = false;
            pnlThana.Visible = false;
            pnlBranch.Visible = false; 
            pnlDesig.Visible = false;
            rdoReportType.Enabled = true;

        }
    }
    private void getEmp()
    {
        
        string cThana="";
        string cDist="";
        string cBranch="";
        string cDesig="";

        if (rdoSelectCriteria.SelectedValue == "EMP")
        {
            DataTable dtabEmp = EmpManager.getEmployeeRpt("");
            DataTable dtEmp = new DataTable();
            dtEmp.TableName = "dtEmp";
            dtEmp.Columns.Add("ID", typeof(string));
            dtEmp.Columns.Add("emp_no", typeof(string));
            dtEmp.Columns.Add("name", typeof(string));
            dtEmp.Columns.Add("dob", typeof(string));
            dtEmp.Columns.Add("branch", typeof(string));
            dtEmp.Columns.Add("desig", typeof(string));
            dtEmp.Columns.Add("dtDocument", typeof(DataTable));

            DataRow drPro;
            DataTable dtDocument;
            foreach (DataRow dr in dtabEmp.Rows)
            {
                drPro = dtEmp.NewRow();
                drPro["ID"] = dr["ID"].ToString();
                drPro["emp_no"] = dr["emp_no"].ToString();
                drPro["name"] = dr["name"].ToString();
                drPro["dob"] = dr["dob"].ToString();
                drPro["branch"] = dr["branch"].ToString();
                drPro["desig"] = dr["desig"].ToString();
                dtDocument = EmpManager.getDocument(dr["ID"].ToString());
                dtDocument.TableName = "dtDocument";
                drPro["dtDocument"] = dtDocument; // ADD This
                dtEmp.Rows.Add(drPro);
            }
            dgEmp.DataSource = dtEmp;
            Session["dtabemp"] = dtEmp;
            dgEmp.DataBind();
        }
        else
        {
            Session["dtabemp"] = null;
        }

    }
    private void getDiv()
    {
        DataTable dtDiv = DivDisThanaManager.getDivision();
        dgDiv.DataSource = dtDiv;
        Session["dtdiv"] = dtDiv;
        dgDiv.DataBind();
    }
    private void getDist()
    {
        string criteria = "";
        DataTable dtabDiv = new DataTable();
        if (Session["dtabDiv"] != null)
        {
            dtabDiv = (DataTable)Session["dtabDiv"];
        }
        DataTable dtDis = new DataTable();
        if (dtabDiv.Rows.Count == 0)
        {
            dtDis = DivDisThanaManager.getDistrict(criteria);
            dgDis.DataSource = dtDis;
            Session["dtdis"] = dtDis;
            dgDis.DataBind();
        }
        else
        {
            string div = "";
            foreach (DataRow dr in dtabDiv.Rows)
            {
                div += "'" + dr[0].ToString() + "',";
            }
            if (div.Length > 0)
            {
                div = div.Remove(div.Length - 1, 1);
            }
            div = "(" + div + ")";
            criteria = " a.division_code in " + div;
            dtDis = DivDisThanaManager.getDistrict(criteria);
            dgDis.DataSource = dtDis;
            Session["dtdis"] = dtDis;
            dgDis.DataBind();
        }
    }
    private void getThana()
    {
        string criteria = "";
        DataTable dtabDis = new DataTable();
        if (Session["dtabDis"] != null)
        {
            dtabDis = (DataTable)Session["dtabDis"];
        }
        DataTable dtThana = new DataTable();
        if (dtabDis.Rows.Count == 0)
        {
            dtThana = DivDisThanaManager.getThana(criteria);
            dgThana.DataSource = dtThana;
            Session["dtthana"] = dtThana;
            dgThana.DataBind();
        }
        else
        {
            string dist = "";
            foreach (DataRow dr in dtabDis.Rows)
            {
                dist += "'" + dr[1].ToString() + "',";
            }
            if (dist.Length > 0)
            {
                dist = dist.Remove(dist.Length - 1, 1);
            }
            dist = "(" + dist + ")";
            criteria = " a.district_code in " + dist;
            dtThana = DivDisThanaManager.getThana(criteria);
            dgThana.DataSource = dtThana;
            Session["dtthana"] = dtThana;
            dgThana.DataBind();
        }
    }
    private void getBranch()
    {
        DataTable dtBranch = DivDisThanaManager.getBranch();
        dgBranch.DataSource = dtBranch;
        Session["dtabbranch"] = dtBranch;
        dgBranch.DataBind();
    }
    private void getDesig()
    {
        DataTable dtDesig = DivDisThanaManager.getDesignation();
        dgDesig.DataSource = dtDesig;
        Session["dtdesig"] = dtDesig;
        dgDesig.DataBind();
    }
    protected void dgEmp_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        DataTable dtTestEmp = new DataTable();
        if (Session["dttestemp"] == null)
        {
            if (dtTestEmp.Columns.Count == 0)
            {
                dtTestEmp.Columns.Add("chkIndex", typeof(string));
                dtTestEmp.Columns.Add("pgeIndex", typeof(string));
            }
        }
        else
        {
            dtTestEmp = (DataTable)Session["dttestemp"];
        }
        DataTable dtEmp = (DataTable)Session["dtemp"];
        DataRow drTest;
        foreach (GridViewRow gvr in dgEmp.Rows)
        {
            if (((CheckBox)gvr.FindControl("chkInc")).Checked)
            {
                drTest = dtTestEmp.NewRow();
                drTest["chkIndex"] = gvr.RowIndex.ToString();
                drTest["pgeIndex"] = dgEmp.PageIndex;
                dtTestEmp.Rows.Add(drTest);
            }
        }
        if (Session["dttestemp"] == null)
        {
            Session["dttestemp"] = dtTestEmp;
        }
        dgEmp.DataSource = dtEmp;
        dgEmp.PageIndex = e.NewPageIndex;
        dgEmp.DataBind();

        foreach (DataRow drr in dtTestEmp.Rows)
        {
            if (dgEmp.PageIndex == int.Parse(drr["pgeIndex"].ToString()))
            {
                ((CheckBox)dgEmp.Rows[int.Parse(drr["chkIndex"].ToString())].FindControl("chkInc")).Checked = true;
            }
        } 
        pnlEmp.Visible = true;
        pnlDiv.Visible = false;
        pnlDis.Visible = false;
        pnlThana.Visible = false;
        pnlBranch.Visible = false;
        pnlDesig.Visible = false;
        dgEmp.Caption = "Employee List";
        dgEmp.CaptionAlign = TableCaptionAlign.Top;
    }
    
    protected void lbEmpIncl_Click(object sender, EventArgs e)
    {
        DataTable dtabEmp = new DataTable();
        if (dtabEmp.Columns.Count == 0)
        {
            dtabEmp.Columns.Add("emp_no", typeof(string));
            dtabEmp.Columns.Add("name", typeof(string));
            dtabEmp.Columns.Add("dob", typeof(string));
            dtabEmp.Columns.Add("branch", typeof(string));
            dtabEmp.Columns.Add("desig", typeof(string));
        }
        DataRow dr;
        foreach (GridViewRow gvr in dgEmp.Rows)
        {            
            if (((CheckBox)gvr.FindControl("chkInc")).Checked)
            {
                dr = dtabEmp.NewRow();
                dr["emp_no"] = gvr.Cells[1].Text.Trim();
                dr["name"] = gvr.Cells[2].Text.Trim();
                dtabEmp.Rows.Add(dr);
            }
        }
        Session["dtabemp"] = dtabEmp;
    }
    protected void lbSearch_Click(object sender, EventArgs e)
    {
        string finCriteria = " and lower(rtrim(ltrim(rtrim(f_name))+' '+ " +
                " ltrim(rtrim(m_name))+' '+ltrim(rtrim(l_name)))) like '%" + txtName.Text.ToString() + "%'";
        DataTable dtabEmp = (DataTable)Session["dtemp"];
        dtabEmp = EmpManager.getEmployeeRpt(finCriteria);
        

        dgEmp.DataSource = dtabEmp;        
        dgEmp.DataBind();
    }
    protected void lbDivIncl_Click(object sender, EventArgs e)
    {
        DataTable dtabDiv = new DataTable();
        if (dtabDiv.Columns.Count == 0)
        {
            dtabDiv.Columns.Add("division_code", typeof(string));
            dtabDiv.Columns.Add("division_name", typeof(string));
        }
        DataRow dr;
        foreach (GridViewRow gvr in dgDiv.Rows)
        {
            if (((CheckBox)gvr.FindControl("chkInc")).Checked)
            {
                dr = dtabDiv.NewRow();
                dr["division_code"] = gvr.Cells[1].Text.Trim();
                dr["division_name"] = gvr.Cells[2].Text.Trim();
                dtabDiv.Rows.Add(dr);
            }
            if (dtabDiv.Rows.Count > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Selected Divisions are added in the list!!');", true);
            }
        }
        Session["dtabdiv"] = dtabDiv;
    }
    protected void lbDisIncl_Click(object sender, EventArgs e)
    {
        DataTable dtabDis = new DataTable();
        if (dtabDis.Columns.Count == 0)
        {
            dtabDis.Columns.Add("division_code", typeof(string));
            dtabDis.Columns.Add("district_code", typeof(string));
            dtabDis.Columns.Add("district_name", typeof(string));
        }
        DataRow dr;
        foreach (GridViewRow gvr in dgDis.Rows)
        {            
            if (((CheckBox)gvr.FindControl("chkInc")).Checked)
            {
                dr = dtabDis.NewRow();
                dr["division_code"] = gvr.Cells[1].Text.Trim();
                dr["district_code"] = gvr.Cells[3].Text.Trim();
                dr["district_name"] = gvr.Cells[4].Text.Trim();
                dtabDis.Rows.Add(dr);
            }
            if (dtabDis.Rows.Count > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Selected Districts are added in the list!!');", true);
            }
        }
        Session["dtabdis"] = dtabDis;
    }    
    protected void btnReset_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }    
    protected void lbDesigIncl_Click(object sender, EventArgs e)
    {

        DataTable dtabDesig = new DataTable();

        if (dtabDesig.Columns.Count == 0)
        {
            dtabDesig.Columns.Add("desig_code", typeof(string));
            dtabDesig.Columns.Add("desig_name", typeof(string));
        }
        DataRow dr;
        foreach (GridViewRow gvr in dgDesig.Rows)
        {
            if (((CheckBox)gvr.FindControl("chkInc")).Checked)
            {
                dr = dtabDesig.NewRow();
                dr["desig_code"] = gvr.Cells[1].Text.Trim();
                dr["desig_name"] = gvr.Cells[2].Text.Trim();
                dtabDesig.Rows.Add(dr);
            }
            if (dtabDesig.Rows.Count > 0)
            {
              //  ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Selected Designations are added in the list!!');", true);
            }
        }

        dgSelectDesignation.DataSource = dtabDesig;
        dgSelectDesignation.DataBind();
        Session["dtabdesig"] = dtabDesig;
    }
    protected void lbBranchIncl_Click(object sender, EventArgs e)
    {
        DataTable dtabBranch = new DataTable();
        if (dtabBranch.Columns.Count == 0)
        {
            dtabBranch.Columns.Add("BranchKey", typeof(string));
            dtabBranch.Columns.Add("BranchName", typeof(string));
        }
        DataRow dr;
        foreach (GridViewRow gvr in dgBranch.Rows)
        {            
            if (((CheckBox)gvr.FindControl("chkInc")).Checked)
            {
                dr = dtabBranch.NewRow();
                dr["BranchKey"] = gvr.Cells[1].Text.Trim();
                dr["BranchName"] = gvr.Cells[2].Text.Trim();
                dtabBranch.Rows.Add(dr);
            }
            //if (dtabBranch.Rows.Count > 0)
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Selected Brances are added in the list!!');", true);
            //}
        }
        Session["dtabbranch"] = dtabBranch;
    }
    protected void lblThanaIncl_Click(object sender, EventArgs e)
    {
        DataTable dtabThana = new DataTable();
        if (dtabThana.Columns.Count == 0)
        {
            dtabThana.Columns.Add("division_code", typeof(string));
            dtabThana.Columns.Add("district_code", typeof(string));
            dtabThana.Columns.Add("thana_code", typeof(string));
            dtabThana.Columns.Add("thana_name", typeof(string));
        }
        DataRow dr;
        foreach (GridViewRow gvr in dgThana.Rows)
        {
            if (((CheckBox)gvr.FindControl("chkInc")).Checked)
            {
                dr = dtabThana.NewRow();
                dr["division_code"] = gvr.Cells[1].Text.Trim();
                dr["district_code"] = gvr.Cells[3].Text.Trim();
                dr["thana_code"] = gvr.Cells[5].Text.Trim();
                dr["thana_name"] = gvr.Cells[6].Text.Trim();
                dtabThana.Rows.Add(dr);
            }
            if (dtabThana.Rows.Count > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Selected Thanas are added in the list!!');", true);
            }
        }
        Session["dtabthana"] = dtabThana;
    }
    protected void dgDiv_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        DataTable dtDiv = (DataTable)Session["dtdiv"];
        dgDiv.PageIndex = e.NewPageIndex;
        dgDiv.DataBind();        
        pnlEmp.Visible = false;
        pnlDiv.Visible = true;
        pnlDis.Visible = false;
        pnlThana.Visible = false;
        pnlBranch.Visible = false;
        pnlDesig.Visible = false;
        dgDiv.Caption = "Division List";        
        dgDiv.CaptionAlign = TableCaptionAlign.Top;
    }
    protected void dgDis_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        DataTable dtDis = (DataTable)Session["dtdis"];
        DataTable dtTestDis = new DataTable();
        if (Session["dttestdis"] == null)
        {
            if (dtTestDis.Columns.Count == 0)
            {
                dtTestDis.Columns.Add("chkIndex", typeof(string));
                dtTestDis.Columns.Add("pgeIndex", typeof(string));
            }
        }
        else
        {
            dtTestDis = (DataTable)Session["dttestdis"];
        }
        DataRow drTest;
        foreach (GridViewRow gvr in dgDis.Rows)
        {
            if (((CheckBox)gvr.FindControl("chkInc")).Checked)
            {
                drTest = dtTestDis.NewRow();
                drTest["chkIndex"] = gvr.RowIndex.ToString();
                drTest["pgeIndex"] = dgDis.PageIndex;
                dtTestDis.Rows.Add(drTest);
            }
        }
        if (Session["dttestdis"] == null)
        {
            Session["dttestdis"] = dtTestDis;
        }
        dgDis.DataSource = dtDis;
        dgDis.PageIndex = e.NewPageIndex;
        dgDis.DataBind();
        
        foreach (DataRow drr in dtTestDis.Rows)
        {
            if (dgDis.PageIndex == int.Parse(drr["pgeIndex"].ToString()))
            {
                ((CheckBox)dgDis.Rows[int.Parse(drr["chkIndex"].ToString())].FindControl("chkInc")).Checked = true;
            }
        }
        pnlEmp.Visible = false;
        pnlDiv.Visible = false;
        pnlDis.Visible = true;
        pnlThana.Visible = false;
        pnlBranch.Visible = false;
        dgDis.Caption = "District List";
        dgDis.CaptionAlign = TableCaptionAlign.Top;
    }
    protected void dgThana_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        DataTable dtThana = (DataTable)Session["dtthana"];
        DataTable dtTestThana = new DataTable();
        if (Session["dttestthana"] == null)
        {
            if (dtTestThana.Columns.Count == 0)
            {
                dtTestThana.Columns.Add("chkIndex", typeof(string));
                dtTestThana.Columns.Add("pgeIndex", typeof(string));
            }
        }
        else
        {
            dtTestThana = (DataTable)Session["dttestthana"];
        }
        DataRow drTest;
        foreach (GridViewRow gvr in dgThana.Rows)
        {
            if (((CheckBox)gvr.FindControl("chkInc")).Checked)
            {
                drTest = dtTestThana.NewRow();
                drTest["chkIndex"] = gvr.RowIndex.ToString();
                drTest["pgeIndex"] = dgThana.PageIndex;
                dtTestThana.Rows.Add(drTest);
            }
        }
        if (Session["dttestthana"] == null)
        {
            Session["dttestthana"] = dtTestThana;
        }
        dgThana.DataSource = dtThana;
        dgThana.PageIndex = e.NewPageIndex;
        dgThana.DataBind();

        foreach (DataRow drr in dtTestThana.Rows)
        {
            if (dgThana.PageIndex == int.Parse(drr["pgeIndex"].ToString()))
            {
                ((CheckBox)dgThana.Rows[int.Parse(drr["chkIndex"].ToString())].FindControl("chkInc")).Checked = true;
            }
        }
        pnlEmp.Visible = false;
        pnlDiv.Visible = false;
        pnlDis.Visible = false;
        pnlThana.Visible = true;
        pnlBranch.Visible = false;
        pnlDesig.Visible = false;
        dgThana.Caption = "Thana List";
        dgThana.CaptionAlign = TableCaptionAlign.Top;
    }
    protected void dgBranch_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        DataTable dtBranch = (DataTable)Session["dtbranch"];
        DataTable dtTestBranch = new DataTable();
        if (Session["dttestbranch"] == null)
        {
            if (dtTestBranch.Columns.Count == 0)
            {
                dtTestBranch.Columns.Add("chkIndex", typeof(string));
                dtTestBranch.Columns.Add("pgeIndex", typeof(string));
            }
        }
        else
        {
            dtTestBranch = (DataTable)Session["dttestbranch"];
        }
        DataRow drTest;
        foreach (GridViewRow gvr in dgBranch.Rows)
        {
            if (((CheckBox)gvr.FindControl("chkInc")).Checked)
            {
                drTest = dtTestBranch.NewRow();
                drTest["chkIndex"] = gvr.RowIndex.ToString();
                drTest["pgeIndex"] = dgBranch.PageIndex;
                dtTestBranch.Rows.Add(drTest);
            }
        }
        if (Session["dttestbranch"] == null)
        {
            Session["dttestbranch"] = dtTestBranch;
        }
        dgBranch.DataSource = dtBranch;
        dgBranch.PageIndex = e.NewPageIndex;
        dgBranch.DataBind();

        foreach (DataRow drr in dtTestBranch.Rows)
        {
            if (dgBranch.PageIndex == int.Parse(drr["pgeIndex"].ToString()))
            {
                ((CheckBox)dgBranch.Rows[int.Parse(drr["chkIndex"].ToString())].FindControl("chkInc")).Checked = true;
            }
        }
        pnlEmp.Visible = false;
        pnlDiv.Visible = false;
        pnlDis.Visible = false;
        pnlThana.Visible = false;
        pnlBranch.Visible = true;
        pnlDesig.Visible = false;
        dgBranch.Caption = "Branch/Unit Office List";
        dgBranch.CaptionAlign = TableCaptionAlign.Top;
    }
    protected void dgDesig_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        DataTable dtDesig = (DataTable)Session["dtdesig"];
        DataTable dtTestDesig = new DataTable();
        if (Session["dttestdesig"] == null)
        {
            if (dtTestDesig.Columns.Count == 0)
            {
                dtTestDesig.Columns.Add("chkIndex", typeof(string));
                dtTestDesig.Columns.Add("pgeIndex", typeof(string));
            }
        }
        else
        {
            dtTestDesig = (DataTable)Session["dttestdesig"];
        }
        DataRow drTest;
        foreach (GridViewRow gvr in dgDesig.Rows)
        {
            if (((CheckBox)gvr.FindControl("chkInc")).Checked)
            {
                drTest = dtTestDesig.NewRow();
                drTest["chkIndex"] = gvr.RowIndex.ToString();
                drTest["pgeIndex"] = dgDesig.PageIndex;
                dtTestDesig.Rows.Add(drTest);
            }
        }
        if (Session["dttestdesig"] == null)
        {
            Session["dttestdesig"] = dtTestDesig;
        }
        dgDesig.DataSource = dtDesig;
        dgDesig.PageIndex = e.NewPageIndex;
        dgDesig.DataBind();

        foreach (DataRow drr in dtTestDesig.Rows)
        {
            if (dgDesig.PageIndex == int.Parse(drr["pgeIndex"].ToString()))
            {
                ((CheckBox)dgDesig.Rows[int.Parse(drr["chkIndex"].ToString())].FindControl("chkInc")).Checked = true;
            }
        }
        pnlEmp.Visible = false;
        pnlDiv.Visible = false;
        pnlDis.Visible = false;
        pnlThana.Visible = false;
        pnlBranch.Visible = false;
        pnlDesig.Visible = true;
        dgDesig.Caption = "Designation List";
        dgDesig.CaptionAlign = TableCaptionAlign.Top;
    }
    private void getEmpSimpleRptPdf(DataTable dt)
    {
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=MypdfFile.pdf");
        Document document = new Document();
        document = new Document(PageSize.A3, 40f, 30f, 30f, 30f);        
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        pdfPage page = new pdfPage();
        writer.PageEvent = page;
        document.Open();

        PdfPCell cell;
        byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
        iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
        gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
        gif.ScalePercent(8f);

        float[] titwidth = new float[2] { 10, 200 };
        PdfPTable dth = new PdfPTable(titwidth);
        dth.WidthPercentage = 100;
        cell = new PdfPCell(gif);
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.Rowspan = 4;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 80f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dth.AddCell(cell);
        cell = new PdfPCell(new Phrase("Employee List", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
        cell.HorizontalAlignment = 1;
        cell.VerticalAlignment = 1;
        cell.BorderWidth = 0f;
        cell.FixedHeight = 20f;
        dth.AddCell(cell);
        document.Add(dth);
        LineSeparator line = new LineSeparator(1f, 100, null, Element.ALIGN_CENTER, -2);
        document.Add(line);
        PdfPTable dtempty = new PdfPTable(1);
        cell = new PdfPCell(FormatHeaderPhrase(""));
        cell.BorderWidth = 0f;
        cell.FixedHeight = 10f;
        dtempty.AddCell(cell);
        document.Add(dtempty);

        float[] width = new float[9] { 8, 15, 25, 15,10,10,10,10,15};
        PdfPTable pdtemp = new PdfPTable(width);
        pdtemp.WidthPercentage = 100;
        cell = new PdfPCell(FormatHeaderPhrase("SL No."));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtemp.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Employee ID"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtemp.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Employee Name"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtemp.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Designation"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtemp.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Join Date"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtemp.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Nationality"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtemp.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Religion"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtemp.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Education"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtemp.AddCell(cell);
        cell = new PdfPCell(FormatHeaderPhrase("Branch Name"));
        cell.HorizontalAlignment = 0;
        cell.VerticalAlignment = 1;
        //cell.FixedHeight = 18f;
        cell.BorderColor = BaseColor.LIGHT_GRAY;
        pdtemp.AddCell(cell);
        for (int loop = 0; loop < dt.Rows.Count; loop++)
        {
            cell = new PdfPCell(FormatHeaderPhraseNormal(loop+1.ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.FixedHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                cell = new PdfPCell(FormatHeaderPhraseNormal(((DataRow)dt.Rows[loop])[j].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
            }               
           
        }
        document.Add(pdtemp);
        document.Close();
        Response.Flush();
        Response.End();
    }
    private void getEmpRptPdf(DataTable dt,string type)
    {        
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=MypdfFile.pdf");
        Document document = new Document();
        string reptitle = "";
        if (type == "R")
        {
            document = new Document(PageSize.A4, 40f, 30f, 20f, 20f);
            reptitle = "Employee Resume";
        }
        else if (type == "D")
        {
            document = new Document(PageSize.A4.Rotate(), 40f, 30f, 20f, 20f);
            reptitle = "Employee Detail Report";
        }
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
        for (int loop = 0; loop < dt.Rows.Count; loop++)
        {
            if (loop > 0)
            {
                document.NewPage();
                document.ResetPageCount();
            }
            PdfPCell cell;
            Rectangle page = document.PageSize;
            PdfPTable head = new PdfPTable(1);
            head.TotalWidth = page.Width - 20;
            Phrase phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.TIMES_ROMAN, 8));
            PdfPCell c = new PdfPCell(phrase);
            c.Border = Rectangle.NO_BORDER;
            c.VerticalAlignment = Element.ALIGN_BOTTOM;
            c.HorizontalAlignment = Element.ALIGN_RIGHT;
            head.AddCell(c);
            head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 5, writer.DirectContent);

            PdfPTable foot = new PdfPTable(1);
            foot.TotalWidth = page.Width - 20;
            phrase = new Phrase((document.PageNumber + 1).ToString(), new Font(Font.FontFamily.TIMES_ROMAN, 8));
            c = new PdfPCell(phrase);
            c.Border = Rectangle.NO_BORDER;
            c.VerticalAlignment = Element.ALIGN_BOTTOM;
            c.HorizontalAlignment = Element.ALIGN_RIGHT;
            foot.AddCell(c);
            foot.WriteSelectedRows(0, -1, 0, 20, writer.DirectContent);

            DataTable dtemp = new DataTable();
            if (type == "R")
            {
                dtemp = EmpManager.getEmpRpt(((DataRow)dt.Rows[loop])["emp_no"].ToString());
            }
            else if (type == "D")
            {
                dtemp = EmpManager.getEmpDetailRpt(((DataRow)dt.Rows[loop])["emp_no"].ToString());
            }
            if (dtemp.Rows.Count > 0)
            {
                byte[] logo = GlBookManager.GetGlLogo(Session["book"].ToString());
                iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(logo);
                gif.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                gif.ScalePercent(8f);

                float[] titwidth = new float[2] { 10, 200 };
                PdfPTable dth = new PdfPTable(titwidth);
                dth.WidthPercentage = 100;
                cell = new PdfPCell(gif);
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.Rowspan = 4;
                cell.BorderWidth = 0f;
                //cell.FixedHeight = 80f;
                dth.AddCell(cell);
                cell = new PdfPCell(new Phrase(Session["org"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                //cell.FixedHeight = 20f;
                dth.AddCell(cell);
                cell = new PdfPCell(new Phrase(Session["add1"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                //cell.FixedHeight = 20f;
                dth.AddCell(cell);
                cell = new PdfPCell(new Phrase(Session["add2"].ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                //cell.FixedHeight = 20f;
                dth.AddCell(cell);
                cell = new PdfPCell(new Phrase(reptitle, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                //cell.FixedHeight = 20f;
                dth.AddCell(cell);
                document.Add(dth);
                LineSeparator line = new LineSeparator(1f, 100, null, Element.ALIGN_CENTER, -2);
                document.Add(line);
                PdfPTable dtempty = new PdfPTable(1);
                cell = new PdfPCell(FormatHeaderPhrase(""));
                cell.BorderWidth = 0f;
                cell.FixedHeight = 10f;
                dtempty.AddCell(cell);
                document.Add(dtempty);

                Emp emp = EmpManager.getEmp(((DataRow)dt.Rows[loop])["emp_no"].ToString());
                byte[] pht = (byte[])emp.EmpPhoto;
                byte[] sig = (byte[])emp.SpecSigna;
                PdfPTable pdtphoto = new PdfPTable(2);
                pdtphoto.WidthPercentage = 100;
                if (pht != null)
                {
                    try
                    {
                        iTextSharp.text.Image gifpht = iTextSharp.text.Image.GetInstance(pht);
                        gifpht.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                        if (type == "R")
                        {
                            gifpht.ScalePercent(50f);
                        }
                        else if (type == "D")
                        {
                            gifpht.ScalePercent(40f);
                        }
                        if (gifpht != null)
                        {
                            cell = new PdfPCell(gifpht);
                            cell.HorizontalAlignment = 2;
                            cell.VerticalAlignment = 1;
                            cell.BorderWidth = 0f;
                            pdtphoto.AddCell(cell);
                        }
                        else
                        {
                            cell = new PdfPCell(FormatPhrase(""));
                            cell.HorizontalAlignment = 2;
                            cell.VerticalAlignment = 1;
                            cell.BorderWidth = 0f;
                            pdtphoto.AddCell(cell);
                        }
                        if (sig != null)
                        {
                            iTextSharp.text.Image gifsig = iTextSharp.text.Image.GetInstance(sig);
                            gifsig.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                            gifsig.ScalePercent(40f);
                            cell = new PdfPCell(gifsig);
                            cell.HorizontalAlignment = 1;
                            cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                            cell.BorderWidth = 0f;
                            pdtphoto.AddCell(cell);
                        }
                        else
                        {
                            //cell = new PdfPCell(null);
                            cell = new PdfPCell(FormatPhrase(""));
                            cell.HorizontalAlignment = 1;
                            cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                            cell.BorderWidth = 0f;
                            pdtphoto.AddCell(cell);
                        }
                    }
                    catch
                    {
                    }
                }
                if (type == "R")
                {
                    float[] widthemp = new float[7] { 30, 5, 50, 5, 30, 5, 50 };
                    PdfPTable pdtemp = new PdfPTable(widthemp);
                    pdtemp.WidthPercentage = 100;
                    cell = new PdfPCell(FormatPhrase("Employee No"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dt.Rows[loop])["emp_no"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(pdtphoto);
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.Rowspan = 6;
                    cell.Colspan = 4;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Name"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["name"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Sex"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["sex"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Date of Birth"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["dob"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Father's Name"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["fh_name"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Mother's Name"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["mh_name"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Blood Group"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["blood_group"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Marital Status"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["marital_status"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Permanent Address"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["per_address"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Mailing address"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["mail_address"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Branch"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["branch"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Designation"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["desig"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.MinimumHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    document.Add(pdtemp);
                }
                else if (type == "D")
                {
                    float[] widthemp = new float[11] { 20, 5, 20, 10, 20, 5, 20, 10, 20, 5, 20 };
                    PdfPTable pdtemp = new PdfPTable(widthemp);
                    pdtemp.WidthPercentage = 100;
                    cell = new PdfPCell(FormatHeaderPhrase(((DataRow)dtemp.Rows[0])["emp_no"].ToString() + "  " + ((DataRow)dtemp.Rows[0])["name"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = 3;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("National ID"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["national_id"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);

                    cell = new PdfPCell(pdtphoto);
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = 3;
                    cell.Rowspan = 4;
                    cell.BorderWidth = 0;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatHeaderPhrase(((DataRow)dtemp.Rows[0])["BranchKey"].ToString() ));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = 3;
                    cell.Border = 4;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Blood Group"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["blood_group"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatHeaderPhrase(((DataRow)dtemp.Rows[0])["desig_code"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = 3;
                    cell.Border = 4;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Personnel File No"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["personeel_file_no"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatHeaderPhrase("Personal Information"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.Colspan = 3;
                    cell.Border = 4;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Driving License No"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["driv_lic_no"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Birth Date (DD/MM/YY)"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["dob"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("TIN"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["tin_no"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Father Name"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["fh_name"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Birth Place"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["place_of_birth"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Passport No"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["pass_no"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Mother Name"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["mh_name"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Religion"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["religion_code"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Provident Fund No"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["pf_no"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Bank Account No"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["bank_acc_no"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Marital Status"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["marital_status_code"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatHeaderPhrase("Present Address"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Colspan = 3;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatHeaderPhrase("Permenant Address"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Colspan = 3;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Sex"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["sex"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Road/House No"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["mail_loc"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Road/House No"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["per_loc"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Spouse Name"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["spouse_name"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("District"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["mail_dist_code"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("District"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["per_dist_code"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Spouse Education"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Thana"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["mail_thana_code"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Thana"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["per_thana_code"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Spouse Profession"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Contact No"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["contact_no"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase("Zip"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(":"));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["zip_area_code"].ToString()));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtemp.AddCell(cell);
                    document.Add(pdtemp);
                }
                DataTable dtedu = EduManager.getEduRpt(((DataRow)dt.Rows[loop])["ID"].ToString());                
                float[] widthedu = new float[6] { 30, 20, 50, 15, 30, 15 };
                PdfPTable pdtedu = new PdfPTable(widthedu);
                pdtedu.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Education:"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = Rectangle.ALIGN_BOTTOM;
                cell.Border = 0;
                cell.Colspan = 6;
                cell.FixedHeight = 25f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Exam Name"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Group"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Institution"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Pass year"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Main Subjects"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Div/Class"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                for (int i = 0; i < dtedu.Rows.Count; i++)
                {
                    for (int j = 0; j < dtedu.Columns.Count; j++)
                    {
                        cell = new PdfPCell(FormatPhrase(((DataRow)dtedu.Rows[i])[j].ToString()));
                        cell.HorizontalAlignment = 0;
                        cell.VerticalAlignment = 1;
                        cell.MinimumHeight = 16f;
                        cell.BorderColor = BaseColor.LIGHT_GRAY;
                        pdtedu.AddCell(cell);
                    }
                }

                DataTable dtexp = ExperManager.getExpRpt(((DataRow)dt.Rows[loop])["ID"].ToString());
                
                float[] widthexp = new float[4] { 50, 50, 15, 15 };
                PdfPTable pdtexp = new PdfPTable(widthexp);
                pdtexp.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Experience : "));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = Rectangle.ALIGN_BOTTOM;
                cell.Border = 0;
                cell.Colspan = 5;
                cell.FixedHeight = 25f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtexp.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Organization"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtexp.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Position Held"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtexp.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("From"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtexp.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("To"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtexp.AddCell(cell);
                //cell = new PdfPCell(FormatHeaderPhrase("Pay Scale"));
                //cell.HorizontalAlignment = 0;
                //cell.VerticalAlignment = 1;
                //cell.MinimumHeight = 16f;
                //cell.BorderColor = BaseColor.LIGHT_GRAY;
                //pdtexp.AddCell(cell);
                for (int i = 0; i < dtexp.Rows.Count; i++)
                {
                    
                    for (int j = 0; j < dtexp.Columns.Count; j++)
                    {
                        if (j <= 3)
                        {
                            cell = new PdfPCell(FormatPhrase(((DataRow) dtexp.Rows[i])[j].ToString()));
                            cell.HorizontalAlignment = 0;
                            cell.VerticalAlignment = 1;
                            cell.MinimumHeight = 16f;
                            cell.BorderColor = BaseColor.LIGHT_GRAY;
                            pdtexp.AddCell(cell);
                        }
                    }
                }


                DataTable dtprom = PromManager.getPromRpt(((DataRow)dt.Rows[loop])["ID"].ToString());
                
                float[] widthprom = new float[5] { 50, 20, 50, 50, 15 };
                PdfPTable pdtprom = new PdfPTable(widthprom);
                pdtprom.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Promotion:"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = Rectangle.ALIGN_BOTTOM;
                cell.Border = 0;
                cell.Colspan = 5;
                cell.FixedHeight = 25f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtprom.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Order No"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtprom.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Join Date"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtprom.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Branch"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtprom.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Designation"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtprom.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Basic"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtprom.AddCell(cell);
                for (int i = 0; i < dtprom.Rows.Count; i++)
                {
                    for (int j = 0; j < dtprom.Columns.Count; j++)
                    {
                        cell = new PdfPCell(FormatPhrase(((DataRow)dtprom.Rows[i])[j].ToString()));
                        cell.HorizontalAlignment = 0;
                        cell.VerticalAlignment = 1;
                        cell.MinimumHeight = 16f;
                        cell.BorderColor = BaseColor.LIGHT_GRAY;
                        pdtprom.AddCell(cell);
                    }
                }

                DataTable dttrans = TransfManager.getTransRpt(((DataRow)dt.Rows[loop])["ID"].ToString());
                
                float[] widthtrans = new float[5] { 50, 20, 20, 50, 50 };
                PdfPTable pdttrans = new PdfPTable(widthtrans);
                pdttrans.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Transfer:"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = Rectangle.ALIGN_BOTTOM;
                cell.Border = 0;
                cell.Colspan = 5;
                cell.FixedHeight = 25f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrans.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Order No"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrans.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Date"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrans.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Promotion?"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrans.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Branch"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrans.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Designation"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrans.AddCell(cell);
                for (int i = 0; i < dttrans.Rows.Count; i++)
                {
                    for (int j = 0; j < dttrans.Columns.Count; j++)
                    {
                        cell = new PdfPCell(FormatPhrase(((DataRow)dttrans.Rows[i])[j].ToString()));
                        cell.HorizontalAlignment = 0;
                        cell.VerticalAlignment = 1;
                        cell.MinimumHeight = 16f;
                        cell.BorderColor = BaseColor.LIGHT_GRAY;
                        pdttrans.AddCell(cell);
                    }
                }

                DataTable dttrain = TrainManager.getTrainRpt(((DataRow)dt.Rows[loop])["ID"].ToString());
                
                float[] widthtrain = new float[9] { 50, 10, 30, 15, 15, 15, 8, 8, 8 };
                PdfPTable pdttrain = new PdfPTable(widthtrain);
                pdttrain.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Training:"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = Rectangle.ALIGN_BOTTOM;
                cell.Border = 0;
                cell.Colspan = 9;
                cell.FixedHeight = 25f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrain.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Training Title"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrain.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Year"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrain.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Place/Institution"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrain.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Country"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrain.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Financed By"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrain.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Amount"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrain.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Duration(Y M D)"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.Colspan = 3;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdttrain.AddCell(cell);
                for (int i = 0; i < dttrain.Rows.Count; i++)
                {
                    for (int j = 0; j < dttrain.Columns.Count; j++)
                    {
                        cell = new PdfPCell(FormatPhrase(((DataRow)dttrain.Rows[i])[j].ToString()));
                        cell.HorizontalAlignment = 0;
                        cell.VerticalAlignment = 1;
                        cell.MinimumHeight = 16f;
                        cell.BorderColor = BaseColor.LIGHT_GRAY;
                        pdttrain.AddCell(cell);
                    }
                }

                DataTable dtqtr = QtrManager.getQtrRpt(((DataRow)dt.Rows[loop])["ID"].ToString());
                
                float[] widthqtr = new float[9] { 50, 12, 18, 30, 20, 10, 10, 10, 10 };
                PdfPTable pdtqtr = new PdfPTable(widthqtr);
                pdtqtr.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Quarter:"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = Rectangle.ALIGN_BOTTOM;
                cell.Border = 0;
                cell.Colspan = 9;
                cell.FixedHeight = 25f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtqtr.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Allotment Reference"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtqtr.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Ref Date"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtqtr.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Positioning Date"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtqtr.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Location"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtqtr.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Road"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtqtr.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("House"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtqtr.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Flat"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtqtr.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Flat Type"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtqtr.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Size"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtqtr.AddCell(cell);
                for (int i = 0; i < dtqtr.Rows.Count; i++)
                {
                    for (int j = 0; j < dtqtr.Columns.Count; j++)
                    {
                        cell = new PdfPCell(FormatPhrase(((DataRow)dtqtr.Rows[i])[j].ToString()));
                        cell.HorizontalAlignment = 0;
                        cell.VerticalAlignment = 1;
                        cell.MinimumHeight = 16f;
                        cell.BorderColor = BaseColor.LIGHT_GRAY;
                        pdtqtr.AddCell(cell);
                    }
                }

                DataTable dtsusp = SuspManager.getSuspRpt(((DataRow)dt.Rows[loop])["ID"].ToString());
                
                float[] widthsusp = new float[6] { 50, 15, 50, 40, 15, 40 };
                PdfPTable pdtsusp = new PdfPTable(widthsusp);
                pdtsusp.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Reward/Punishment:"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = Rectangle.ALIGN_BOTTOM;
                cell.Border = 0;
                cell.Colspan = 6;
                cell.FixedHeight = 25f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtsusp.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Office Order No"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtsusp.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Ord Date"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtsusp.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Clause"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtsusp.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Withdrawn Order No"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtsusp.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Date"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtsusp.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Retribution"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtsusp.AddCell(cell);

                for (int i = 0; i < dtsusp.Rows.Count; i++)
                {
                    for (int j = 0; j < dtsusp.Columns.Count; j++)
                    {
                        cell = new PdfPCell(FormatPhrase(((DataRow)dtsusp.Rows[i])[j].ToString()));
                        cell.HorizontalAlignment = 0;
                        cell.VerticalAlignment = 1;
                        cell.MinimumHeight = 16f;
                        cell.BorderColor = BaseColor.LIGHT_GRAY;
                        pdtsusp.AddCell(cell);
                    }
                }

                DataTable dtmemb = MembManager.getMembRpt(((DataRow)dt.Rows[loop])["ID"].ToString());
                
                float[] widthmemb = new float[2] { 100, 30 };
                PdfPTable pdtmemb = new PdfPTable(widthmemb);
                pdtmemb.WidthPercentage = 70;
                cell = new PdfPCell(FormatHeaderPhrase("Member of:"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = Rectangle.ALIGN_BOTTOM;
                cell.Border = 0;
                cell.Colspan = 2;
                cell.FixedHeight = 25f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtmemb.AddCell(cell);
                pdtmemb.HorizontalAlignment = iTextSharp.text.Rectangle.ALIGN_LEFT;
                cell = new PdfPCell(FormatHeaderPhrase("Asscociation"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtmemb.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Membership No"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtmemb.AddCell(cell);

                for (int i = 0; i < dtmemb.Rows.Count; i++)
                {
                    for (int j = 0; j < dtmemb.Columns.Count; j++)
                    {
                        cell = new PdfPCell(FormatPhrase(((DataRow)dtmemb.Rows[i])[j].ToString()));
                        cell.HorizontalAlignment = 0;
                        cell.VerticalAlignment = 1;
                        cell.MinimumHeight = 16f;
                        cell.BorderColor = BaseColor.LIGHT_GRAY;
                        pdtmemb.AddCell(cell);
                    }
                }

                DataTable dtfam = FamManager.getFamRpt(((DataRow)dt.Rows[loop])["ID"].ToString());
                
                float[] widthfam = new float[5] { 50, 20, 20, 15, 40 };
                PdfPTable pdtfam = new PdfPTable(widthfam);
                pdtfam.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Family Information:"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = Rectangle.ALIGN_BOTTOM;
                cell.Border = 0;
                cell.Colspan = 5;
                cell.FixedHeight = 25f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtfam.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Relative Name"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtfam.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Relation"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtfam.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Birth Date"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtfam.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Phone Number"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtfam.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Occupation"));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtfam.AddCell(cell);

                for (int i = 0; i < dtfam.Rows.Count; i++)
                {
                    for (int j = 0; j < dtfam.Columns.Count; j++)
                    {
                        cell = new PdfPCell(FormatPhrase(((DataRow)dtfam.Rows[i])[j].ToString()));
                        cell.HorizontalAlignment = 0;
                        cell.VerticalAlignment = 1;
                        cell.MinimumHeight = 16f;
                        cell.BorderColor = BaseColor.LIGHT_GRAY;
                        pdtfam.AddCell(cell);
                    }
                }
                if (type == "R")
                {
                    if (dtedu.Rows.Count > 0)
                    {
                        document.Add(pdtedu);
                    }
                    if (dtexp.Rows.Count > 0)
                    {
                        document.Add(pdtexp);
                    }
                    if (dtprom.Rows.Count > 0)
                    {
                        document.Add(pdtprom);
                    }
                    if (dttrans.Rows.Count > 0)
                    {
                        document.Add(pdttrans);
                    }
                    if (dttrain.Rows.Count > 0)
                    {
                        document.Add(pdttrain);
                    }
                    if (dtqtr.Rows.Count > 0)
                    {
                        document.Add(pdtqtr);
                    }
                    if (dtsusp.Rows.Count > 0)
                    {
                        document.Add(pdtsusp);
                    }
                    if (dtmemb.Rows.Count > 0)
                    {
                        document.Add(pdtmemb);
                    }
                    if (dtfam.Rows.Count > 0)
                    {
                        document.Add(pdtfam);
                    }
                }
                else if (type == "D")
                {
                    List<PdfPTable> ld = new List<PdfPTable>();
                    if (dtedu.Rows.Count > 0)
                    {
                        ld.Add(pdtedu);
                    }
                    if (dtexp.Rows.Count > 0)
                    {
                        ld.Add(pdtexp);
                    }
                    if (dtprom.Rows.Count > 0)
                    {
                        ld.Add(pdtprom);
                    }
                    if (dttrans.Rows.Count > 0)
                    {
                        ld.Add(pdttrans);
                    }
                    if (dttrain.Rows.Count > 0)
                    {
                        ld.Add(pdttrain);
                    }
                    if (dtqtr.Rows.Count > 0)
                    {
                        ld.Add(pdtqtr);
                    }
                    if (dtsusp.Rows.Count > 0)
                    {
                        ld.Add(pdtsusp);
                    }
                    if (dtmemb.Rows.Count > 0)
                    {
                        ld.Add(pdtmemb);
                    }
                    if (dtfam.Rows.Count > 0)
                    {
                        ld.Add(pdtfam);
                    }
                    float[] width = new float[3] { 200, 5, 200 };
                    PdfPTable pdtlrpt = new PdfPTable(width);
                    pdtlrpt.WidthPercentage = 100;
                    int dividant = (ld.Count / 2) ;
                    int remainder = int.Parse(decimal.Remainder(ld.Count, 2).ToString());
                    int limit = dividant + remainder;
                    for (int i = 0; i <limit; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (i == limit - 1)
                            {
                                if (ld.Count > 0)
                                {
                                    if (j == 1)
                                    {
                                        cell = new PdfPCell(new Phrase(""));
                                        cell.HorizontalAlignment = 0;
                                        cell.VerticalAlignment = 1;
                                        cell.Border = 0;
                                        pdtlrpt.AddCell(cell);
                                    }
                                    else
                                    {
                                        if (ld.Count > 0)
                                        {
                                            cell = new PdfPCell((PdfPTable)ld[0]);
                                            cell.HorizontalAlignment = 0;
                                            cell.VerticalAlignment = 1;
                                            cell.Border = 0;
                                            pdtlrpt.AddCell(cell);
                                            ld.RemoveAt(0);
                                        }
                                    }
                                }
                                else
                                {
                                    cell = new PdfPCell(new Phrase(""));
                                    cell.HorizontalAlignment = 0;
                                    cell.VerticalAlignment = 1;
                                    cell.Border = 0;
                                    pdtlrpt.AddCell(cell);
                                }
                            }
                            else
                            {
                                if (ld.Count > 0)
                                {
                                    if (j == 1)
                                    {
                                        cell = new PdfPCell(new Phrase(""));
                                        cell.HorizontalAlignment = 0;
                                        cell.VerticalAlignment = 1;
                                        cell.Border = 0;
                                        pdtlrpt.AddCell(cell);
                                    }
                                    else
                                    {
                                        if (ld.Count > 0)
                                        {
                                            cell = new PdfPCell((PdfPTable)ld[0]);
                                            cell.HorizontalAlignment = 0;
                                            cell.VerticalAlignment = 1;
                                            cell.Border = 0;
                                            pdtlrpt.AddCell(cell);
                                            ld.RemoveAt(0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    document.Add(pdtlrpt);
                }
            }
        }
        document.Close();
        Response.Flush();
        Response.End();
    }
    private void getEmpDetailRpt(DataTable dt)
    {

    }
    public override void VerifyRenderingInServerForm(Control control)
    {

    }
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8));
    }
    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.BOLD));
    }
    private static Phrase FormatHeaderPhraseNormal(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, iTextSharp.text.Font.NORMAL));
    }
    protected void dgDesig_RowDataBound(object sender, GridViewRowEventArgs e)
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
    protected void dgEmp_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");
            e.Row.Cells[7].Attributes.Add("style", "display:none");
            GridView gv = (GridView)e.Row.FindControl("dgQuestion");
            DataTable dtDeptType = (DataTable)DataBinder.Eval(e.Row.DataItem, "dtDocument");
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
            e.Row.Cells[1].Attributes.Add("style", "display:none");
            e.Row.Cells[7].Attributes.Add("style", "display:none");
        }
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Attributes.Add("style", "display:none");
            e.Row.Cells[7].Attributes.Add("style", "display:none");
        }
    }

    protected void txtName_TextChanged(object sender, EventArgs e)
    {
        lblempID.Text = IdManager.GetShowSingleValuestring("t1.EMP_NO", " PMIS_PERSONNEL t1 left join PMIS_DESIG_CODE t2 on t2.DESIG_CODE=t1.JOIN_DESIG_CODE where upper(t1.EMP_NO+'-'+t1.F_NAME+'-'+t2.DESIG_NAME )=upper(" + txtName.Text + ")");
        if (string.IsNullOrEmpty(lblempID.Text))
        {
           
        }
    }
    protected void dgQuestion_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Download"))
        {
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            //DataTable dtDecumentDtl = (DataTable)ViewState["Document"];
            DataTable dtDecumentDtl = EmpManager.getSingleDocument(gvr.Cells[0].Text);
            if (dtDecumentDtl.Rows.Count > 0)
            {
                DataRow dr = dtDecumentDtl.AsEnumerable().SingleOrDefault(r => r.Field<String>("ID") == gvr.Cells[0].Text);

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition",
                    "attachment;filename=" +
                    dr["FileDescription"].ToString().Replace("'", "").Replace(" ", "_"));
                // to open file prompt Box open or Save file         
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite((byte[])dr["FileImage"]);
                Response.End();
            }
        }
    }
    protected void dgSubject_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Attributes.Add("style", "display:none");           
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[0].Attributes.Add("style", "display:none");           
        }
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[0].Attributes.Add("style", "display:none");            
        }
    }
    protected void chkInc_CheckedChanged(object sender, EventArgs e)
    {
        DataTable dtabBranch = new DataTable();
        if (dtabBranch.Columns.Count == 0)
        {
            dtabBranch.Columns.Add("BranchKey", typeof(string));
            dtabBranch.Columns.Add("BranchName", typeof(string));
        }
        DataRow dr;
        foreach (GridViewRow gvr in dgBranch.Rows)
        {
            if (((CheckBox)gvr.FindControl("chkInc")).Checked)
            {
                dr = dtabBranch.NewRow();
                dr["BranchKey"] = gvr.Cells[1].Text.Trim();
                dr["BranchName"] = gvr.Cells[2].Text.Trim();
                dtabBranch.Rows.Add(dr);
            }
            if (dtabBranch.Rows.Count > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Selected Brances are added in the list!!');", true);
            }
        }
        Session["dtabbranch"] = dtabBranch;
    }
    protected void dgBranch_RowDataBound(object sender, GridViewRowEventArgs e)
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
}
