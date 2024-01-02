using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Delve;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Delve;
using Delve;

public partial class frmEmployeeReport : System.Web.UI.Page
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
        if(!IsPostBack)
        {
            util.PopulateCombo(ddlBranchKey,
                "select ' ' BranchName,'000' BranchKey union select branchName + ' ('+shortname+')',BranchKey from branchinfo",
                "BranchName", "BranchKey");
            //DesignationDrropDownList
            util.PopulateCombo(DesignationDrropDownList, "select ' ' DESIG_NAME,'000' DESIG_CODE union select DESIG_NAME + ' ('+DESIG_ABB+')',DESIG_CODE from PMIS_DESIG_CODE", "DESIG_NAME", "DESIG_CODE");
            StartDtateTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy");
            EndDateTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy");
            
        }
    }
    protected void ddlRepType_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void lbRunReport_Click(object sender, EventArgs e)
    {
         Session.Remove("rptbyte");
        if (ddlRepType.SelectedValue == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please select a report type!!');", true);
            return;
        }
        else if (ddlRepType.SelectedValue == "AEL")
        {
            DataTable dt = EmpManager.GetAllEmployeeInformationForReport();
            Response.Clear();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment; filename=IDRASBW.pdf");
            Document document = new Document(PageSize.A4.Rotate(), 30f, 30f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            pdfPage page1 = new pdfPage();
            writer.PageEvent = page1;
            document.Open();
            PdfPCell cell;

            float[] titwidth = new float[2] { 10, 200 };
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

            cell = new PdfPCell(FormatHeaderTopPhrase("Continental Insurance Limited"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 0;
            cell.BorderWidth = 0f;
            cell.Colspan = 2;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            dth.AddCell(cell);


            string repname = "";
            repname = "Detail Reports of Employee ";

            cell = new PdfPCell(new Phrase(repname, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 14, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 20f;
            cell.Colspan = 2;
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

            float[] widthmst = new float[11] { 10, 20, 20, 25, 15, 15, 15, 15, 15, 15, 15 };

            PdfPTable pdtMst = new PdfPTable(widthmst);
            pdtMst.WidthPercentage = 100;
            pdtMst.HeaderRows = 1;
            cell = new PdfPCell(FormatHeaderPhrase("SL No"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("ID"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Name of Employee"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Designation Name"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Joining Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Confirmation Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Last Promotion Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Last Increment Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Date of Birth"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Service Length"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            //Edit
            cell = new PdfPCell(FormatHeaderPhrase("Emp Type"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                cell = new PdfPCell(FormatPhrase((i + 1).ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["ID"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Name of Employee"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Designation Name"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 0.5f;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Joining Date"].ToString()));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Confirmation Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                //cell = new PdfPCell(FormatPhrase(decimal.Parse(string.IsNullOrEmpty(dr["Amount"].ToString()) ? "0" : dr["Amount"].ToString()).ToString("N3")));
                //cell.VerticalAlignment = 2;
                //cell.HorizontalAlignment = 2;
                //cell.BorderWidth = 0.5f;
                //cell.BorderColor = BaseColor.LIGHT_GRAY;
                //pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Last Promotion Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Last Increment Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Date of Birth"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);

                //Edit
                cell = new PdfPCell(FormatPhrase(dr["Service Length"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Emp Type"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                //edit
                i++;

            }
            document.Add(pdtMst);
            document.Close();
            byte[] byt = ms.GetBuffer();
            if (Session["rptbyte"] != null) { byte[] rptbyt = (byte[])Session["rptbyte"]; rptbyt = byt; } else { Session["rptbyte"] = byt; }
        }
        else if(ddlRepType.SelectedValue.ToString()=="BR")
        {
            if (ddlBranchKey.SelectedValue != "000")
            {
                DataTable dt = EmpManager.GetAllEmployeeInformationForSpecificBrachReport(ddlBranchKey.SelectedValue);
                Response.Clear();
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-disposition", "attachment; filename=IDRASBW.pdf");
                Document document = new Document(PageSize.A4.Rotate(), 30f, 30f, 40f, 40f);
                MemoryStream ms = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                pdfPage page1 = new pdfPage();
                writer.PageEvent = page1;
                document.Open();
                PdfPCell cell;

                float[] titwidth = new float[2] { 10, 200 };
                PdfPTable dth = new PdfPTable(titwidth);
                dth.WidthPercentage = 100;

                cell = new PdfPCell(FormatHeaderTopPhrase("Continental Insurance Limited"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 0;
                cell.BorderWidth = 0f;
                cell.Colspan = 2;
                cell.FixedHeight = 20f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                dth.AddCell(cell);


                string repname = "";
                repname = ddlBranchKey.SelectedItem.ToString() + " All Employee List";

                cell = new PdfPCell(new Phrase(repname, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 14, iTextSharp.text.Font.BOLD)));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                cell.FixedHeight = 20f;
                cell.Colspan = 2;
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

                float[] widthmst = new float[11] { 10, 20, 20, 25, 15, 15, 15, 15, 15, 15, 15 };

                PdfPTable pdtMst = new PdfPTable(widthmst);
                pdtMst.WidthPercentage = 100;
                pdtMst.HeaderRows = 1;
                cell = new PdfPCell(FormatHeaderPhrase("SL No"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("ID"));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Name of Employee"));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Designation Name"));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Joining Date"));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Confirmation Date"));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Last Promotion Date"));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Last Increment Date"));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Date of Birth"));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Service Length"));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                //Edit
                cell = new PdfPCell(FormatHeaderPhrase("Emp Type"));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);

                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    cell = new PdfPCell(FormatPhrase((i + 1).ToString()));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0.5f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtMst.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(dr["ID"].ToString()));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0.5f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtMst.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(dr["Name of Employee"].ToString()));
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0.5f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtMst.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(dr["Designation Name"].ToString()));
                    cell.VerticalAlignment = 2;
                    cell.HorizontalAlignment = 0;
                    cell.BorderWidth = 0.5f;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtMst.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(dr["Joining Date"].ToString()));
                    cell.VerticalAlignment = 1;
                    cell.HorizontalAlignment = 1;
                    cell.BorderWidth = 0.5f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtMst.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(dr["Confirmation Date"].ToString()));
                    cell.VerticalAlignment = 2;
                    cell.HorizontalAlignment = 1;
                    cell.BorderWidth = 0.5f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtMst.AddCell(cell);
                    //cell = new PdfPCell(FormatPhrase(decimal.Parse(string.IsNullOrEmpty(dr["Amount"].ToString()) ? "0" : dr["Amount"].ToString()).ToString("N3")));
                    //cell.VerticalAlignment = 2;
                    //cell.HorizontalAlignment = 2;
                    //cell.BorderWidth = 0.5f;
                    //cell.BorderColor = BaseColor.LIGHT_GRAY;
                    //pdtMst.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(dr["Last Promotion Date"].ToString()));
                    cell.VerticalAlignment = 2;
                    cell.HorizontalAlignment = 1;
                    cell.BorderWidth = 0.5f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtMst.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(dr["Last Increment Date"].ToString()));
                    cell.VerticalAlignment = 2;
                    cell.HorizontalAlignment = 1;
                    cell.BorderWidth = 0.5f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtMst.AddCell(cell);
                    cell = new PdfPCell(FormatPhrase(dr["Date of Birth"].ToString()));
                    cell.VerticalAlignment = 2;
                    cell.HorizontalAlignment = 1;
                    cell.BorderWidth = 0.5f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtMst.AddCell(cell);

                    //Edit
                    cell = new PdfPCell(FormatPhrase(dr["Service Length"].ToString()));
                    cell.VerticalAlignment = 2;
                    cell.HorizontalAlignment = 1;
                    cell.BorderWidth = 0.5f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtMst.AddCell(cell);

                    cell = new PdfPCell(FormatPhrase(dr["Emp Type"].ToString()));
                    cell.VerticalAlignment = 2;
                    cell.HorizontalAlignment = 1;
                    cell.BorderWidth = 0.5f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtMst.AddCell(cell);
                    //edit
                    i++;

                }
                document.Add(pdtMst);
                document.Close();
                byte[] byt = ms.GetBuffer();
                if (Session["rptbyte"] != null) { byte[] rptbyt = (byte[])Session["rptbyte"]; rptbyt = byt; } else { Session["rptbyte"] = byt; 
                }
            }
        }
        else if (ddlRepType.SelectedValue.ToString() == "Des")
        {
            DataTable dt = EmpManager.GetAllEmployeeInformationForSpecificDesignationReport(DesignationDrropDownList.SelectedValue);
            Response.Clear();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment; filename=IDRASBW.pdf");
            Document document = new Document(PageSize.A4.Rotate(), 30f, 30f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            pdfPage page1 = new pdfPage();
            writer.PageEvent = page1;
            document.Open();
            PdfPCell cell;

            float[] titwidth = new float[2] { 10, 200 };
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

            cell = new PdfPCell(FormatHeaderTopPhrase("Continental Insurance Limited"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 0;
            cell.BorderWidth = 0f;
            cell.Colspan = 2;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            dth.AddCell(cell);


            string repname = "";
            repname =DesignationDrropDownList.SelectedItem.ToString() + " All Employee List ";

            cell = new PdfPCell(new Phrase(repname, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 14, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 20f;
            cell.Colspan = 2;
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

            float[] widthmst = new float[11] { 10, 20, 20, 25, 15, 15, 15, 15, 15, 15, 15 };

            PdfPTable pdtMst = new PdfPTable(widthmst);
            pdtMst.WidthPercentage = 100;
            pdtMst.HeaderRows = 1;
            cell = new PdfPCell(FormatHeaderPhrase("SL No"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("ID"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Name of Employee"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Designation Name"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Joining Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Confirmation Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Last Promotion Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Last Increment Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Date of Birth"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Service Length"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            //Edit
            cell = new PdfPCell(FormatHeaderPhrase("Emp Type"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                cell = new PdfPCell(FormatPhrase((i + 1).ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["ID"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Name of Employee"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Designation Name"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 0.5f;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Joining Date"].ToString()));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Confirmation Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                //cell = new PdfPCell(FormatPhrase(decimal.Parse(string.IsNullOrEmpty(dr["Amount"].ToString()) ? "0" : dr["Amount"].ToString()).ToString("N3")));
                //cell.VerticalAlignment = 2;
                //cell.HorizontalAlignment = 2;
                //cell.BorderWidth = 0.5f;
                //cell.BorderColor = BaseColor.LIGHT_GRAY;
                //pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Last Promotion Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Last Increment Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Date of Birth"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);

                //Edit
                cell = new PdfPCell(FormatPhrase(dr["Service Length"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Emp Type"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                //edit
                i++;

            }
            document.Add(pdtMst);
            document.Close();
            byte[] byt = ms.GetBuffer();
            if (Session["rptbyte"] != null) { byte[] rptbyt = (byte[])Session["rptbyte"]; rptbyt = byt; }
            else
            {
                Session["rptbyte"] = byt;
            }
        }
        else if (ddlRepType.SelectedValue.ToString() == "ET")
        {
            DataTable dt = EmpManager.GetAllEmployeeInformationForSpecificEmployeeTypeReport(EmployeeTypeDrropDownList.SelectedValue.ToString());
            Response.Clear();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment; filename=IDRASBW.pdf");
            Document document = new Document(PageSize.A4.Rotate(), 30f, 30f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            pdfPage page1 = new pdfPage();
            writer.PageEvent = page1;
            document.Open();
            PdfPCell cell;

            float[] titwidth = new float[2] { 10, 200 };
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

            cell = new PdfPCell(FormatHeaderTopPhrase("Continental Insurance Limited"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 0;
            cell.BorderWidth = 0f;
            cell.Colspan = 2;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            dth.AddCell(cell);

             
            string repname = "";
            repname =EmployeeTypeDrropDownList.SelectedItem.ToString() + " All Employee List ";

            cell = new PdfPCell(new Phrase(repname, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 14, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 20f;
            cell.Colspan = 2;
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

            float[] widthmst = new float[11] { 10, 20, 20, 25, 15, 15, 15, 15, 15, 15, 15 };

            PdfPTable pdtMst = new PdfPTable(widthmst);
            pdtMst.WidthPercentage = 100;
            pdtMst.HeaderRows = 1;
            cell = new PdfPCell(FormatHeaderPhrase("SL No"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("ID"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Name of Employee"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Designation Name"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Joining Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Confirmation Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Last Promotion Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Last Increment Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Date of Birth"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Service Length"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            //Edit
            cell = new PdfPCell(FormatHeaderPhrase("Emp Type"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                cell = new PdfPCell(FormatPhrase((i + 1).ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["ID"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Name of Employee"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Designation Name"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 0.5f;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Joining Date"].ToString()));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Confirmation Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                //cell = new PdfPCell(FormatPhrase(decimal.Parse(string.IsNullOrEmpty(dr["Amount"].ToString()) ? "0" : dr["Amount"].ToString()).ToString("N3")));
                //cell.VerticalAlignment = 2;
                //cell.HorizontalAlignment = 2;
                //cell.BorderWidth = 0.5f;
                //cell.BorderColor = BaseColor.LIGHT_GRAY;
                //pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Last Promotion Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Last Increment Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Date of Birth"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);

                //Edit
                cell = new PdfPCell(FormatPhrase(dr["Service Length"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Emp Type"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                //edit
                i++;

            }
            document.Add(pdtMst);
            document.Close();
            byte[] byt = ms.GetBuffer();
            if (Session["rptbyte"] != null) { byte[] rptbyt = (byte[])Session["rptbyte"]; rptbyt = byt; }
            else
            {
                Session["rptbyte"] = byt;
            }
        }
        else if (ddlRepType.SelectedValue.ToString() == "JD")
        {
            DataTable dt = EmpManager.GetAllEmployeeInformationForSpecificJoiningDateReport(StartDtateTextBox.Text,EndDateTextBox.Text);
            Response.Clear();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment; filename=IDRASBW.pdf");
            Document document = new Document(PageSize.A4.Rotate(), 30f, 30f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            pdfPage page1 = new pdfPage();
            writer.PageEvent = page1;
            document.Open();
            PdfPCell cell;

            float[] titwidth = new float[2] { 10, 200 };
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

            cell = new PdfPCell(FormatHeaderTopPhrase("Continental Insurance Limited"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 0;
            cell.BorderWidth = 0f;
            cell.Colspan = 2;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            dth.AddCell(cell);


            string repname = "";
            repname = "Detail Reports of Employee Joining Date Wise ";

            cell = new PdfPCell(new Phrase(repname, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 14, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 20f;
            cell.Colspan = 2;
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

            float[] widthmst = new float[11] { 10, 20, 20, 25, 15, 15, 15, 15, 15, 15, 15 };

            PdfPTable pdtMst = new PdfPTable(widthmst);
            pdtMst.WidthPercentage = 100;
            pdtMst.HeaderRows = 1;
            cell = new PdfPCell(FormatHeaderPhrase("SL No"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("ID"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Name of Employee"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Designation Name"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Joining Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Confirmation Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Last Promotion Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Last Increment Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Date of Birth"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Service Length"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            //Edit
            cell = new PdfPCell(FormatHeaderPhrase("Emp Type"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                cell = new PdfPCell(FormatPhrase((i + 1).ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["ID"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Name of Employee"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Designation Name"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 0.5f;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Joining Date"].ToString()));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Confirmation Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                //cell = new PdfPCell(FormatPhrase(decimal.Parse(string.IsNullOrEmpty(dr["Amount"].ToString()) ? "0" : dr["Amount"].ToString()).ToString("N3")));
                //cell.VerticalAlignment = 2;
                //cell.HorizontalAlignment = 2;
                //cell.BorderWidth = 0.5f;
                //cell.BorderColor = BaseColor.LIGHT_GRAY;
                //pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Last Promotion Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Last Increment Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Date of Birth"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);

                //Edit
                cell = new PdfPCell(FormatPhrase(dr["Service Length"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Emp Type"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                //edit
                i++;

            }
            document.Add(pdtMst);
            document.Close();
            byte[] byt = ms.GetBuffer();
            if (Session["rptbyte"] != null) { byte[] rptbyt = (byte[])Session["rptbyte"]; rptbyt = byt; }
            else
            {
                Session["rptbyte"] = byt;
            }
        }
        else if (ddlRepType.SelectedValue == "OT")
        {
            DataTable dt = EmpManager.GetAllEmployeeInformationForOthers(ddlBranchKey.SelectedValue,DesignationDrropDownList.SelectedValue,EmployeeTypeDrropDownList.SelectedValue,StartDtateTextBox.Text, EndDateTextBox.Text);
            Response.Clear();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment; filename=IDRASBW.pdf");
            Document document = new Document(PageSize.A4.Rotate(), 30f, 30f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            pdfPage page1 = new pdfPage();
            writer.PageEvent = page1;
            document.Open();
            PdfPCell cell;

            float[] titwidth = new float[2] { 10, 200 };
            PdfPTable dth = new PdfPTable(titwidth);
            dth.WidthPercentage = 100;

            cell = new PdfPCell(FormatHeaderTopPhrase("Continental Insurance Limited"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 0;
            cell.BorderWidth = 0f;
            cell.Colspan = 2;
            cell.FixedHeight = 20f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            dth.AddCell(cell);


            string repname = "";
            repname = "Detail Reports of Employee Branch, Designation & Employee Type Wise";

            cell = new PdfPCell(new Phrase(repname, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 14, iTextSharp.text.Font.BOLD)));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0f;
            cell.FixedHeight = 20f;
            cell.Colspan = 2;
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

            float[] widthmst = new float[11] { 10, 20, 20, 25, 15, 15, 15, 15, 15, 15, 15 };

            PdfPTable pdtMst = new PdfPTable(widthmst);
            pdtMst.WidthPercentage = 100;
            pdtMst.HeaderRows = 1;
            cell = new PdfPCell(FormatHeaderPhrase("SL No"));
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("ID"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Name of Employee"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Designation Name"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Joining Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Confirmation Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Last Promotion Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Last Increment Date"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Date of Birth"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            cell = new PdfPCell(FormatHeaderPhrase("Service Length"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);
            //Edit
            cell = new PdfPCell(FormatHeaderPhrase("Emp Type"));
            cell.VerticalAlignment = 1;
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtMst.AddCell(cell);

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                cell = new PdfPCell(FormatPhrase((i + 1).ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["ID"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Name of Employee"].ToString()));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Designation Name"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 0.5f;
                cell.FixedHeight = 18f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Joining Date"].ToString()));
                cell.VerticalAlignment = 1;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Confirmation Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                //cell = new PdfPCell(FormatPhrase(decimal.Parse(string.IsNullOrEmpty(dr["Amount"].ToString()) ? "0" : dr["Amount"].ToString()).ToString("N3")));
                //cell.VerticalAlignment = 2;
                //cell.HorizontalAlignment = 2;
                //cell.BorderWidth = 0.5f;
                //cell.BorderColor = BaseColor.LIGHT_GRAY;
                //pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Last Promotion Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Last Increment Date"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                cell = new PdfPCell(FormatPhrase(dr["Date of Birth"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);

                //Edit
                cell = new PdfPCell(FormatPhrase(dr["Service Length"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);

                cell = new PdfPCell(FormatPhrase(dr["Emp Type"].ToString()));
                cell.VerticalAlignment = 2;
                cell.HorizontalAlignment = 1;
                cell.BorderWidth = 0.5f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtMst.AddCell(cell);
                //edit
                i++;

            }
            document.Add(pdtMst);
            document.Close();
            byte[] byt = ms.GetBuffer();
            if (Session["rptbyte"] != null) { byte[] rptbyt = (byte[])Session["rptbyte"]; rptbyt = byt; }
            else
            {
                Session["rptbyte"] = byt;
            }
        }
        string strJS = ("<script type='text/javascript'>window.open('Default4.aspx','_blank');</script>");
        Page.ClientScript.RegisterStartupScript(this.GetType(), "strJSAlert", strJS);
    }

    public override void VerifyRenderingInServerForm(Control control)
    {

    }
    protected void ddlBranchKey_SelectedIndexChanged(object sender, EventArgs e)
    {
        
    }

    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8));
    }
    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }
    private static Phrase FormatHeaderTopPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16, iTextSharp.text.Font.BOLD));
    }
    protected void lbReset_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
}