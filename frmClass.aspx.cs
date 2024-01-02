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
using System.Data.SqlClient;
using Delve;
using Delve;
using Delve;

public partial class frmClass : System.Web.UI.Page
{
    private static Permis per;
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
            DataTable dt = clsClassManager.getClasss();
            if (dt.Rows.Count > 0)
            {
                dgClass.DataSource = dt;
                dgClass.DataBind();
            }
            else
            {
                getEmptyGrid();
            }
        }
    }
    private void getEmptyGrid()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("class_id", typeof(string));
        dt.Columns.Add("class_name", typeof(string));
        DataRow dr = dt.NewRow();
        dt.Rows.Add(dr);
        dgClass.DataSource = dt;
        dgClass.DataBind();
        dgClass.ShowFooter = true;
        dgClass.FooterRow.Visible = true;
        ((TextBox)dgClass.FooterRow.FindControl("txtClassId")).Text = "1";
    }
    protected void dgClass_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        DataTable dt = clsClassManager.getClasss();
        if (dt.Rows.Count > 0)
        {
            dgClass.DataSource = dt;
            dgClass.EditIndex = -1;
            dgClass.DataBind();
            dgClass.ShowFooter = false;
            dgClass.FooterRow.Visible = false;
        }
        else
        {
            getEmptyGrid();
        }
    }
    protected void dgClass_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "AddNew")
        {
            
            DataTable dt = clsClassManager.getClasss();
            dgClass.DataSource = dt;
            dgClass.DataBind();
            dgClass.ShowFooter = true;
            dgClass.FooterRow.Visible = true;
            ((TextBox)dgClass.FooterRow.FindControl("txtClassId")).Text = (Convert.ToInt32(((Label)dgClass.Rows[dgClass.Rows.Count - 1].Cells[1].FindControl("lblClassId")).Text) + 1).ToString();
            ((TextBox)dgClass.FooterRow.FindControl("txtClassName")).Focus();
        }
        else if (e.CommandName == "Insert")
        {
            if (per.AllowAdd == "Y")
            {
                clsClass cls = new clsClass();
                cls.ClassId = ((TextBox)dgClass.FooterRow.FindControl("txtClassId")).Text;
                cls.ClassName = ((TextBox)dgClass.FooterRow.FindControl("txtClassName")).Text;
                clsClassManager.CreateClass(cls);
                string conn = DataManager.OraConnString();
                //string col = clsClassManager.getColumn(cls.ClassName.Replace(" ", ""));
                //if (col != "Y")
                //{
                //    string queryd = "alter table fund_info add " + cls.ClassName.Replace(" ", "") + " varchar2(6)";
                //    DataManager.ExecuteNonQuery(conn, queryd);
                //    string querya = "alter table pay_types add " + cls.ClassName.Replace(" ", "") + " varchar2(6)";
                //    DataManager.ExecuteNonQuery(conn, querya);
                //}  
                DataTable dt = clsClassManager.getClasss();
                if (dt.Rows.Count > 0)
                {
                    dgClass.DataSource = dt;
                    dgClass.EditIndex = -1;
                    dgClass.DataBind();
                    dgClass.ShowFooter = false;
                    dgClass.FooterRow.Visible = false;
                }
                else
                {
                    getEmptyGrid();
                }
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Green;
                lblTranStatus.Text = "Records Created Successfully";
            }
            else
            {
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
                lblTranStatus.Text = "You have no enough permission to create record here.";
            }
        }
    }
    protected void dgClass_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (per.AllowDelete == "Y")
        {
            string clsname = ((Label)dgClass.Rows[e.RowIndex].FindControl("lblClassName")).Text.ToString();
            clsClassManager.DeleteClass(((Label)dgClass.Rows[e.RowIndex].FindControl("lblClassId")).Text);
            string conn = DataManager.OraConnString();
            //string col = clsClassManager.getColumn(((Label)dgClass.Rows[e.RowIndex].FindControl("lblClassId")).Text.Trim().Replace(" ", ""));
            //if (col != "Y")
            //{
            //    string queryd = "alter table fund_info drop column " + ((Label)dgClass.Rows[e.RowIndex].FindControl("lblClassName")).Text.Trim().Replace(" ", "") + " ";
            //    DataManager.ExecuteNonQuery(conn, queryd);
            //    string querya = "alter table pay_types drop column " + ((Label)dgClass.Rows[e.RowIndex].FindControl("lblClassName")).Text.Trim().Replace(" ", "") + " ";
            //    DataManager.ExecuteNonQuery(conn, querya);
            //}  
            DataTable dt = clsClassManager.getClasss();
            if (dt.Rows.Count > 0)
            {
                dgClass.DataSource = dt;
                dgClass.EditIndex = -1;
                dgClass.DataBind();
                dgClass.ShowFooter = false;
                dgClass.FooterRow.Visible = false;                              
            }
            else
            {
                getEmptyGrid();
            }
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Green;
            lblTranStatus.Text = "Records Deleted Successfully";
        }
        else
        {
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
            lblTranStatus.Text = "You have no enough permission to delete record here.";
        }
    }
    protected void dgClass_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DataTable dt = clsClassManager.getClasss();
        dgClass.DataSource = dt;
        dgClass.EditIndex = e.NewEditIndex;
        dgClass.DataBind();
    }
    protected void dgClass_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (per.AllowEdit == "Y")
        {
            clsClass cls = clsClassManager.getClass(((TextBox)dgClass.Rows[e.RowIndex].FindControl("txtClassId")).Text);
            cls.ClassName = ((TextBox)dgClass.Rows[e.RowIndex].FindControl("txtClassName")).Text;
            clsClassManager.UpdateClass(cls);            
            DataTable dt = clsClassManager.getClasss();
            if (dt.Rows.Count > 0)
            {
                dgClass.DataSource = dt;
                dgClass.EditIndex = -1;
                dgClass.DataBind();
            }
            else
            {
                getEmptyGrid();
            }
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Green;
            lblTranStatus.Text = "Records Update Successfully";
            string conn = DataManager.OraConnString();
            string col = clsClassManager.getColumn(((Label)dgClass.Rows[e.RowIndex].FindControl("lblClassId")).Text.Trim().Replace(" ", ""));
            if (col != "Y")
            {
                string queryd = "alter table fund_info modify " + ((Label)dgClass.Rows[e.RowIndex].FindControl("lblClassId")).Text.Trim().Replace(" ", "") + " varchar2(6) ";
                DataManager.ExecuteNonQuery(conn, queryd);
                string querya = "alter table pay_types modify " + ((Label)dgClass.Rows[e.RowIndex].FindControl("lblClassId")).Text.Trim().Replace(" ", "") + " varchar2(6) ";
                DataManager.ExecuteNonQuery(conn, querya);
            }
        }
        else
        {
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
            lblTranStatus.Text = "You have no enough permission to change record here.";
        }
    }
    protected void dgClass_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((DataRowView)e.Row.DataItem)[1].ToString() == String.Empty)
            {
                e.Row.Visible = false;
            }
        }
    }
}
