using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
//using autouniv;
using Delve;
using System.Drawing;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System.Net.NetworkInformation;

public partial class Home : System.Web.UI.Page
{
    private static Permis per;
    VouchManager _aVouchManager=new VouchManager();
    SalesManager _aSalesManager=new SalesManager();
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
        if (!IsPostBack)
        {
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            pnlChangePass.Visible = pnlDailySalesStatus.Visible = false;
            try
            {
                string pageName = DataManager.GetCurrentPageName();
                string modid = PermisManager.getModuleId(pageName);
                per = PermisManager.getUsrPermis(Session["user"].ToString().Trim().ToUpper(), modid);
                ((Label)Page.Master.FindControl("lblLogin")).Text = Session["wnote"].ToString();
                ((Label)Page.Master.FindControl("lblCountryName")).Text = Session["LoginCountry"].ToString();
                ((LinkButton)Page.Master.FindControl("lbLogout")).Visible = true;

                //var address1 = GetMACAddress();
                //var ad = System.Web.HttpContext.Current.Request.UserHostAddress;
                //var ad1 = System.Web.HttpContext.Current.Request.UrlReferrer;
                //var ad2 = System.Web.HttpContext.Current.Request.UserAgent;
                //var ad3 = System.Web.HttpContext.Current.Request.UserHostName;
                //var ad4 = System.Web.HttpContext.Current.Request.UserLanguages;
                //var ad5 = System.Web.HttpContext.Current.Request.Path;
                //var ad9 = System.Web.HttpContext.Current.Request.PhysicalPath;
                //var ad8 = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
                //var ad7 = System.Web.HttpContext.Current.Request.UserHostAddress;
                //var ad6 = System.Web.HttpContext.Current.Request.UserHostAddress;

            }
            catch
            {
                Session["user"] = "";
                Session["pass"] = "";
                pnlTask.Visible = false;
                Response.Redirect("Default.aspx?sid=sam");
            }
            txtCpUserName.Text = Session["user"].ToString();
            lblTranStatus.Visible = false;
            //pnlChangePass.Visible = false;            
        }
    }
    public static PhysicalAddress GetMacAddress()
    {
        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            // Only consider Ethernet network interfaces
            if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                nic.OperationalStatus == OperationalStatus.Up)
            {
                return nic.GetPhysicalAddress();
            }
        }
        return null;
    }


    public string GetMACAddress()
    {
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        String sMacAddress = string.Empty;
        foreach (NetworkInterface adapter in nics)
        {
            if (sMacAddress == String.Empty)// only return MAC Address from first card
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                sMacAddress = adapter.GetPhysicalAddress().ToString();
            }
        } return sMacAddress;
    }
    protected void lbChangePassword_click(object sender, EventArgs e)
    {
        if (txtCpNewPass.Text != txtCpConfPass.Text)
        {
            lblTranStatus.Text = "New Password & Confirm Password are not same!!";
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
         //   ModalPopupExtenderLogin.Show();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "LoadModalDiv();", true);
        }
        else if (txtCpCurPass.Text == String.Empty)
        {
            lblTranStatus.Text = "Please provide current password!!";
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
        //    ModalPopupExtenderLogin.Show();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "LoadModalDiv();", true);
        }
        else if (txtCpNewPass.Text == String.Empty | txtCpConfPass.Text == String.Empty)
        {
            lblTranStatus.Text = "New password cannot be null!!";
            lblTranStatus.Visible = true;
            lblTranStatus.ForeColor = System.Drawing.Color.Red;
           // ModalPopupExtenderLogin.Show();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "LoadModalDiv();", true);
        }
        else if (txtCpNewPass.Text != String.Empty && txtCpConfPass.Text != String.Empty)
        {
            Users usr = UsersManager.getUser(txtCpUserName.Text.ToString().ToUpper());
            if (usr != null && PasswordEncriptAndDecript.Decrypt(usr.Password) == txtCpCurPass.Text)
            {
                usr.LoginBy = Session["user"].ToString();
                usr.Password = PasswordEncriptAndDecript.Encrypt(txtCpNewPass.Text);
                UsersManager.GetResetPassword(usr);
                lblTranStatus.Text = "Password has changed!!";
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Green;
                txtCpCurPass.Text = "";
                txtCpNewPass.Text = "";
                txtCpConfPass.Text = "";
            }
            else
            {
                lblTranStatus.Text = "Old password is not correct!!";
                lblTranStatus.Visible = true;
                lblTranStatus.ForeColor = System.Drawing.Color.Red;
             //   ModalPopupExtenderLogin.Show();
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "LoadModalDiv();", true);
            }
        }
    }

    protected void lbCancel_Click(object sender, EventArgs e)
    {
        pnlChangePass.Visible = false;
    }
    protected void lbChangePass_Click(object sender, EventArgs e)
    {
        pnlTask.Visible = true;
       
        pnlChangePass.Visible = true;
        txtCpUserName.Text = Session["user"].ToString();
        lblTranStatus.Visible = false;
    }

    //protected void Timer1_Tick(object sender, EventArgs e)
    //{
    //    lblTime.Text = "T : " + DateTime.Now.ToString("hh:mm:ss tt");
    //}


    private static Phrase FormatPhraseOnSingleFont(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7));
    }
    private static Phrase FormatPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9));
    }

    private static Phrase FormatHeaderPhrase(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD));
    }
    private static Phrase FormatHeaderPhraseNew(string value)
    {
        return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, iTextSharp.text.Font.NORMAL));
    }
    protected void ibDailySalesStatus_Click(object sender, ImageClickEventArgs e)
    {
        pnlDailySalesStatus.Visible = true;
        pnlChangePass.Visible = false;
        PaymentDetails();
    }

    private void PaymentDetails()
    {
        if (string.IsNullOrEmpty(txtDate.Text))
        {
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
        double totSaleQty = IdManager.GetShowSingleValueCurrency(
            @"select isnull(sum(isnull(t1.Quantity,0)),0) AS[totQty] from OrderDetail t1
        inner join [Order] t2 on t2.ID=t1.OrderID where (convert(date,t2.OrderDate,103) between convert(date,'" +
            txtDate.Text + "',103) and convert(date,'" + txtDate.Text + "',103))");

        double totReturnQty = IdManager.GetShowSingleValueCurrency(
            @"select isnull(sum(isnull(t1.Quantity,0)),0) AS[totQty] from OrderReturnDetail t1
inner join OrderReturn t2 on t2.ID=t1.OrderReturnMstID
where (convert(date,t2.ReturnDate,103) between convert(date,'" + txtDate.Text + "',103) and convert(date,'" +
            txtDate.Text + "',103))");
        lblTotSaleQty.Text = totSaleQty.ToString("N2");
        lblTotReturnQty.Text = totReturnQty.ToString("N2");
        DataTable dtPayment = _aSalesManager.GetPaymentSummery(txtDate.Text, "1");
        if (dtPayment.Rows.Count > 0)
        {
            dgPayment.DataSource = dtPayment;
            dgPayment.DataBind();
            ShowFooterTotal(dtPayment);
        }
    }

    private void ShowFooterTotal(DataTable DT1)
    {
        decimal tot = 0;
        foreach (DataRow dr in DT1.Rows)
        {
            if (!string.IsNullOrEmpty(dr["Amount"].ToString()))
            {
                tot += Convert.ToDecimal(dr["Amount"]);
            }

        }
        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
        TableCell cell;
        cell = new TableCell();
        cell.Text = "<h4>Total Received Amount</h4>";
        cell.ColumnSpan = 2;
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        cell = new TableCell();
        cell.Text = "<h4>" + tot.ToString("N2") + "</h4>";
        cell.HorizontalAlign = HorizontalAlign.Right;
        row.Cells.Add(cell);
        if (dgPayment.Rows.Count > 0)
        {
            dgPayment.Controls[0].Controls.Add(row);
        }
    }
    protected void ibItemPurchase_Click(object sender, ImageClickEventArgs e)
    {
        pnlDailySalesStatus.Visible = pnlChangePass.Visible = false;
        string strJS = ("<script type='text/javascript'>window.open('PurchaseVoucher.aspx?mno=5.15','_blank');</script>");
        Page.ClientScript.RegisterStartupScript(this.GetType(), "strJSAlert", strJS);
    }
    protected void ibSalesInvoice_Click(object sender, ImageClickEventArgs e)
    {
        pnlDailySalesStatus.Visible = pnlChangePass.Visible = false;
        string strJS = ("<script type='text/javascript'>window.open('SalesVoucher.aspx?mno=6.19','_blank');</script>");
        Page.ClientScript.RegisterStartupScript(this.GetType(), "strJSAlert", strJS);
    }
    protected void ibItemStock_Click(object sender, ImageClickEventArgs e)
    {
        pnlDailySalesStatus.Visible = pnlChangePass.Visible = false;
        string strJS = ("<script type='text/javascript'>window.open('StockItemsDetails.aspx?mno=5.18','_blank');</script>");
        Page.ClientScript.RegisterStartupScript(this.GetType(), "strJSAlert", strJS);
    }
    protected void ibChangePassword_Click(object sender, ImageClickEventArgs e)
    {
        pnlDailySalesStatus.Visible = false;
        pnlChangePass.Visible = true;
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        PaymentDetails();
    }
}