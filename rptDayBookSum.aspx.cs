using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.pdf.draw;
using System.Reflection;
using Delve;
//using cins;

public partial class rptDayBookSum : System.Web.UI.Page
{
    private static string book = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            book = Session["book"].ToString();
            lblOrg.Text = Session["org"].ToString();
            lblAddress1.Text = Session["add1"].ToString();
            lblAddress2.Text = Session["add2"].ToString();
            lblTitle.Text = "DAY BOOK SUMMARY";
            string RepType = Request.QueryString["reptype"].ToString();
            string StartDt = Request.QueryString["startdt"].ToString();
            string EndDt = Request.QueryString["enddt"].ToString();
            string glCode = Request.QueryString["glcoa"].ToString();
            string RepLvl = Request.QueryString["replvl"].ToString();
            string SegLvl = Request.QueryString["seglvl"].ToString();
            string VchType = Request.QueryString["vchtyp"].ToString();
            string RptSysId = Request.QueryString["rptsysid"].ToString();
            string NotesNo = Request.QueryString["notes"].ToString();
            RepType = RepType.Substring(0, 1).Trim();
            lblTranStatus.Text = "";
            lblDate.Text = "FROM "+StartDt+" TO "+EndDt;
            if (StartDt == "" | EndDt == "" | DataManager.DateEncode(EndDt).CompareTo(System.DateTime.Now) > 0)
            {
                lblTranStatus.Text = "Please select the Start Date and Upto Date, or Upto Date cannot be greater than System Date.";
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                DataTable dtVch = VouchManager.getDayBook(StartDt, EndDt, VchType, book, Session["UserType"].ToString());
                string connectionString = DataManager.OraConnString();
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connectionString;
                dgDb.DataSource = dtVch;
                dgDb.DataBind();
                ShowFooterTotal(dgDb, dtVch, "amount_dr");
                DataTable dtInc = VouchManager.getDayIncomeBook(StartDt, EndDt, book, Session["UserType"].ToString());
                dgIncome.DataSource = dtInc;
                dgIncome.DataBind();
                ShowFooterTotal(dgIncome,dtInc,"amount_cr");
            }
        }
    }
    protected void dgDb_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[2].Text = decimal.Parse(((DataRowView)e.Row.DataItem)["amount_dr"].ToString()).ToString("N3");
        }
    }
    protected void dgDb_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {

    }
    protected void lbExp_Click(object sender, EventArgs e)
    {

    }
    protected void dgIncome_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[1].Text = decimal.Parse(((DataRowView)e.Row.DataItem)["amount_cr"].ToString()).ToString("N3");
        }
    }
    private void ShowFooterTotal(GridView gv, DataTable dt,string drcr)
    {
        decimal amt = decimal.Zero;
        foreach (DataRow drs in dt.Rows)
        {
            amt += decimal.Parse(drs[drcr].ToString());
        }
        if (dt.Rows.Count > 0)
        {
            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);

            TableCell cell;
            if (drcr == "amount_dr")
            {
                cell = new TableCell();
                cell.Text = "";
                row.Cells.Add(cell);
            }
            cell = new TableCell();
            cell.Text = "Total";
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = amt.ToString("N3");
            row.Cells.Add(cell);
            row.Font.Bold = true;
            row.HorizontalAlign = HorizontalAlign.Right;
            gv.Controls[0].Controls.Add(row);
        }
    }
}