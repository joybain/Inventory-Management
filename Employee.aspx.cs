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
using System.IO;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
//using cins;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.pdf.draw;
using System.Reflection;
using Delve;
using System.Diagnostics;
using OldColor;


public partial class Employee : System.Web.UI.Page
{
    private byte[] empPhoto;
    private byte[] empSigna;
    private static string EduSt ="N";
    private static string FamSt = "N";
    private static string ExpSt = "N";
    private static string TrainSt = "N";
    private static string TransSt = "N";
    private static string PromSt = "N";
    private static string QtrSt = "N";
    private static string SuspSt = "N";
    private static string MembSt = "N";
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
        txtEmpBirthDt.Attributes.Add("onBlur", "formatdate('" + txtEmpBirthDt.ClientID + "')");
        if (!Page.IsPostBack)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "ipconfig";
            info.Arguments = "/renew"; // or /release if you want to disconnect
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(info);
            p.WaitForExit();

            RefreshDropDown();
            getEmptyEdu();
            getEmptyExp();
            getEmptyFam();
            getEmptyTrain();
            getEmptyTrans();
            getEmptyProm();
            getEmptyQtr();
            //getEmptySusp();
            //getEmptyMemb();
            imgEmp.ImageUrl = "~/tmpHandler.ashx?filename=img/noimage.jpg";
            imgSig.ImageUrl = "~/tmpHandler.ashx?filename=img/sign.jpg";
            //FileStream fs = new FileStream(HttpContext.Current.Server.MapPath("~/img/noimage.jpg"), FileMode.Open, FileAccess.Read);
            //BinaryReader br = new BinaryReader(fs);
            //byte[] bt = br.ReadBytes((int)fs.Length);
            //empPhoto = bt;
            //empSigna = bt;
            dgEmp.DataSource = null;
            dgEmp.DataBind();
            dgEmp.DataSource = EmpManager.GetAllEmployeeInformation();
            dgEmp.DataBind();
            //TabPanel4.Visible =TabPanel8.Visible=TabPanel9.Visible=TabPanel10.Visible= false;
            BtnSave.Enabled = true;
            ddlBankNo.DataSource = clsBankManager.getBankMsts();
            ddlBankNo.DataTextField = "bank_name";
            ddlBankNo.DataValueField = "bank_id";
            ddlBankNo.DataBind();
            ddlBankNo.Items.Insert(0,"");
            EmpTabContainer.ActiveTabIndex = 6;
            Session["empPhoto"] = null;
            Session["empSigna"] = null;
            lblID.Text = "";
            ViewState["Emp_No"] = null;
            EmpTabContainer.ActiveTabIndex = 0;

            getEmptyDocumentTable();

            txtEmpNo.Focus();
        }
    }
    private void RefreshDropDown()
    {
        //ddlBankNo.Items.Clear();
        //string queryBank = "select '' BankKey,'' BankName union select b.bank_id BankKey ,dbo.initcap(b.bank_name+', '+BranchName) BankName  from bank_branch a,bank_info b where a.Bank_Id=b.bank_id order by 2";
        //util.PopulationDropDownList(ddlBankNo, "BankInfo", queryBank, "BankName", "BankKey");



        ddlReligionCode.Items.Clear();
        string queryReligion= "select '' ID, '' Name union select ID,dbo.initcap(Name) Name from Religion order by 2 ";
        util.PopulationDropDownList(ddlReligionCode, "Religion", queryReligion, "Name", "ID");

        ddlJoinDesigCode.Items.Clear();
        string queryDesig = "select '' desig_code, '' desig_name union select desig_code,dbo.initcap(desig_name) desig_name from pmis_desig_code order by 2 ";
        util.PopulationDropDownList(ddlJoinDesigCode, "Designation", queryDesig, "desig_name", "desig_code");

        ddlPrstDesigCode.Items.Clear();
        util.PopulationDropDownList(ddlPrstDesigCode, "Designation", queryDesig, "desig_name", "desig_code");

        ddlBranchKey.Items.Clear();
        string queryBranch = "select '' id, '' BranchName union select id,dbo.initcap(BranchName) BranchName from BranchInfo order by 2 ";
        util.PopulationDropDownList(ddlBranchKey, "BranchInfo", queryBranch, "BranchName", "id");

        ddlPerDistCode.Items.Clear();
        string queryDist = "select '' dist_code, '' dist_name union select district_code,district_name from DISTRICT_CODE order by 2 ";
        util.PopulationDropDownList(ddlPerDistCode, "District", queryDist, "dist_name", "dist_code");

        ddlMailDistCode.Items.Clear();
        util.PopulationDropDownList(ddlMailDistCode, "District", queryDist, "dist_name", "dist_code");

        ddlCountry.Items.Clear();
        string query3 = "select '' [COUNTRY_CODE],'' [COUNTRY_DESC]  union select [COUNTRY_CODE] ,[COUNTRY_DESC] from [COUNTRY_INFO] order by 1";
        util.PopulationDropDownList(ddlCountry, "COUNTRY_INFO", query3, "COUNTRY_DESC", "COUNTRY_CODE");
        //ddlCountry.SelectedValue = "1";

    }
    protected void BtnSave_Click(object sender, EventArgs e)
    {
        string IdGlCoa = "";
        string Desc = "";
        string User = "";
        string Login;
        string glCode = "";
        if (ddlCountry.SelectedValue == "1")
        {
            Desc = ",Accounts Payable To - " + txtFName.Text;
            User = "Ban";
            glCode = "6010502";
        }
        else if (ddlCountry.SelectedValue == "2")
        {
            Desc = ",Accounts Payable To - " + txtFName.Text;
            User = "Man";
            glCode = "6030402";
        }
        else
        {
            Desc = ",Accounts Payable To - " + txtFName.Text;
            User = "All";
        }

        if (string.IsNullOrEmpty(ddlBranchKey.SelectedItem.Text))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Select Branch ...!!');", true);
            return;
        }

        if (string.IsNullOrEmpty(txtFName.Text))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('First Name, Birth Date, Permanent District Code cannot be null!!');", true);
            return;
        }
        if (string.IsNullOrEmpty(ddlCountry.SelectedItem.Text))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('select Country..!!');", true);
            return;
        }
        if (string.IsNullOrEmpty(ddlJoinDesigCode.SelectedItem.Text))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('select Designation..!!');", true);
            return;
        }
        if (string.IsNullOrEmpty(txtEmpNo.Text))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input Employee ID..!!');", true);
            return;
        }
        Emp emp = EmpManager.getEmpUpdate(lblID.Text);
        if (emp != null)
        {
            if (per.AllowEdit == "Y")
            {
               // int CheckCode = IdManager.GetShowSingleValueInt("COUNT(*)", "SEG_COA_CODE", "GL_SEG_COA", txtGlCoa.Text);
                //if (CheckCode <= 0)
                //{
                //    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Rong Segment code check this code.\\n Try again ...!!');", true);
                //    return;
                //}
                byte[] oim = (byte[])emp.EmpPhoto;
                byte[] osi = (byte[])emp.SpecSigna;
                if (txtFName.Text != emp.FName |ddlSex.SelectedValue != emp.Sex | emp.EmpBirthDt != txtEmpBirthDt.Text |
                    txtPlaceOfBirth.Text != emp.PlaceOfBirth | ddlReligionCode.SelectedValue != emp.ReligionCode |
                    ddlBloodGroup.SelectedValue != emp.BloodGroup | ddlMaritalStatusCode.SelectedValue != emp.MaritalStatusCode |
                    txtSpouseName.Text != emp.SpouseName | txtFhName.Text != emp.FhName | txtMhName.Text != emp.MhName |
                    txtJoinDate.Text != emp.JoinDate | ddlJoinDesigCode.SelectedValue != emp.JoinDesigCode |
                    ddlJobStatus.SelectedValue != emp.JobStatus | txtConfirmDate.Text != emp.ConfirmDate | 
                    ddlEmpCat.SelectedValue != emp.EmpCat | txtPfNo.Text != emp.PfNo |
                    txtPassNo.Text != emp.PassNo | txtDrivLicNo.Text != emp.DrivLicNo | txtNationalId.Text != emp.NationalId | txtNationality.Text!=emp.Nationality |
                    txtPerLoc.Text != emp.PerLoc | ddlPerDistCode.SelectedValue != emp.PerDistCode | ddlPerThanaCode.SelectedValue != emp.PerThanaCode |
                    txtZipAreaCode.Text != emp.ZipAreaCode | txtMailLoc.Text != emp.MailLoc | ddlMailDistCode.SelectedValue != emp.MailDistCode |
                    ddlMailThanaCode.SelectedValue != emp.MailThanaCode | txtResPhNo.Text != emp.ResPhNo | txtMobile.Text != emp.Mobile |
                    txtEMail.Text != emp.EMail | ddlBankNo.SelectedValue != emp.BankBranchNo | txtBankAccNo.Text != emp.BankAccNo |
                    txtEmpInsureDt.Text != emp.EmpInsuredDt | txtSpouseInsDt.Text != emp.SpouseInsDt | txtSenrSlNo.Text != emp.SenrSlNo |
                    ddlPrstDesigCode.SelectedValue != emp.PrstDesigCode | ddlBranchKey.SelectedValue != emp.BranchKey |
                    emp.PersoneelFileNo != txtPersoneelFileNo.Text | emp.TinNo != txtTinNo.Text |
                    txtLprDate.Text != emp.LprDate | oim != empPhoto | osi != empSigna | ddlCountry.SelectedValue != emp.County)
                {
                    
                 
                    emp.FName = txtFName.Text;
                    emp.Sex = ddlSex.SelectedValue; 
                    emp.EmpBirthDt = txtEmpBirthDt.Text;
                    emp.PlaceOfBirth = txtPlaceOfBirth.Text; 
                    emp.ReligionCode = ddlReligionCode.SelectedValue;
                    emp.BloodGroup = ddlBloodGroup.SelectedValue;
                    emp.MaritalStatusCode = ddlMaritalStatusCode.SelectedValue;
                    emp.SpouseName = txtSpouseName.Text;
                    emp.FhName = txtFhName.Text; 
                    emp.MhName = txtMhName.Text;
                    emp.JoinDate = txtJoinDate.Text; 
                    emp.JoinDesigCode = ddlJoinDesigCode.SelectedValue;
                    emp.JobStatus = ddlJobStatus.SelectedValue; 
                    emp.ConfirmDate = txtConfirmDate.Text;
                    emp.EmpCat = ddlEmpCat.SelectedValue; 
                    emp.PfNo = txtPfNo.Text;
                    emp.PassNo = txtPassNo.Text; 
                    emp.DrivLicNo = txtDrivLicNo.Text; 
                    emp.NationalId = txtNationalId.Text; 
                    emp.Nationality=txtNationality.Text;
                    emp.PerLoc = txtPerLoc.Text; 
                    emp.PerDistCode = ddlPerDistCode.SelectedValue; 
                    emp.PerThanaCode = ddlPerThanaCode.SelectedValue;
                    emp.ZipAreaCode = txtZipAreaCode.Text; 
                    emp.MailLoc = txtMailLoc.Text; 
                    emp.MailDistCode = ddlMailDistCode.SelectedValue;
                    emp.MailThanaCode = ddlMailThanaCode.SelectedValue; 
                    emp.ResPhNo = txtResPhNo.Text; 
                    emp.Mobile = txtMobile.Text;
                    emp.EMail = txtEMail.Text; 
                    emp.BankBranchNo = ddlBankNo.SelectedValue;
                    emp.BankAccNo = txtBankAccNo.Text;
                    emp.EmpInsuredDt = txtEmpInsureDt.Text;
                    emp.SpouseInsDt = txtSpouseInsDt.Text; 
                    emp.SenrSlNo = txtSenrSlNo.Text;
                    emp.PrstDesigCode = ddlPrstDesigCode.SelectedValue; 
                    emp.BranchKey = ddlBranchKey.SelectedValue;
                    emp.LprDate = txtLprDate.Text; 
                    emp.PersoneelFileNo = txtPersoneelFileNo.Text; 
                    emp.TinNo = txtTinNo.Text;
                    emp.County = ddlCountry.SelectedValue;
                    emp.MailPostCode = txtMailPostCode.Text;
                    emp.GlCoaCode = txtGlCoa.Text;
                    emp.EmpCode = txtEmpNo.Text;
                    if (string.IsNullOrEmpty(txtBasicSalary.Text))
                    {
                        emp.BasicSalary = "0";
                    }
                    else
                    {
                        emp.BasicSalary = txtBasicSalary.Text;
                    }
                    byte[] image = null;
                    if (Session["empPhoto"] != " ")
                    {
                        emp.EmpPhoto = (byte[])Session["empPhoto"];
                        image = (byte[])Session["empPhoto"];
                    }
                    if (Session["empSigna"] != " ")
                    {
                        emp.SpecSigna = (byte[])Session["empSigna"];
                        image = (byte[])Session["empSigna"];
                    }
                    emp.UpdateBy = Session["user"].ToString();
                    EmpManager.UpdateEmp(emp);
                    
                }

                if (ViewState["Document"] != null)
                {
                    DataTable dtDocument = (DataTable)ViewState["Document"];
                    EmpManager.GetSaveEmployeeDocument(dtDocument, emp.EmpNo);
                }

                if (EduSt == "O")
                {
                    EduManager.DeleteEdu(emp.EmpNo);
                    Edu edu;
                    DataTable dt = (DataTable)ViewState["dtedu"];
                    foreach (DataRow dredu in dt.Rows)
                    {
                        if (dredu["exam_code"].ToString() != "")
                        {
                            edu = new Edu();
                            edu.EmpNo = emp.EmpNo;
                            edu.ExamCode = dredu["exam_code"].ToString();
                            edu.GroupName = dredu["group_name"].ToString();
                            edu.Institute = dredu["institute"].ToString();
                            edu.PassYear = dredu["pass_year"].ToString();
                            edu.MainSub = dredu["main_sub"].ToString();
                            edu.DivClass = dredu["div_class"].ToString();
                            EduManager.CreateEdu(edu);
                        }
                    }
                }
                if (FamSt == "O")
                {
                    dgEmpFam.ShowFooter = false;
                    dgEmpFam.EditIndex = -1;
                    FamManager.DeleteFam(emp.EmpNo);
                    Fam fam;
                    DataTable dt = (DataTable)ViewState["dtfam"];
                    foreach (DataRow drfam in dt.Rows)
                    {
                        if (drfam["rel_name"].ToString() != "")
                        {
                            fam = new Fam();
                            fam.EmpNo = emp.EmpNo;
                            fam.RelName = drfam["rel_name"].ToString();
                            fam.Relation = drfam["relation"].ToString();
                            fam.BirthDt = drfam["birth_dt"].ToString();
                            fam.Age = drfam["age"].ToString();
                            fam.Occupation = drfam["occupation"].ToString();
                            FamManager.CreateFam(fam);
                        }
                    }
                }
                if (ExpSt == "O")
                {
                    ExperManager.DeleteExper(emp.EmpNo);
                    Exper exp;
                    dgEmpExp.EditIndex = -1;
                    dgEmpExp.ShowFooter = false;
                    DataTable dt = (DataTable)ViewState["dtexp"];
                    foreach (DataRow drexp in dt.Rows)
                    {
                        if (drexp["orga_name"].ToString() != "")
                        {
                            exp = new Exper();
                            exp.EmpNo = emp.EmpNo;
                            exp.OrgaName = drexp["orga_name"].ToString();
                            exp.PositionHeld = drexp["position_held"].ToString();
                            exp.FromDt = drexp["from_dt"].ToString();
                            exp.ToDt = drexp["to_dt"].ToString();
                            exp.PayScale = drexp["pay_scale"].ToString();
                            ExperManager.CreateExper(exp);
                        }
                    }
                }
                if (TrainSt == "O")
                {
                    dgEmpTrain.EditIndex = -1;
                    dgEmpTrain.ShowFooter = false;
                    TrainManager.DeleteTrain(emp.EmpNo);
                    Train trn;
                    DataTable dt = (DataTable)ViewState["dttrain"];
                    foreach (DataRow drtrn in dt.Rows)
                    {
                        if (drtrn["train_title"].ToString() != "")
                        {
                            trn = new Train();
                            trn.EmpNo = emp.EmpNo;
                            trn.TrainTitle = drtrn["train_title"].ToString();
                            trn.Year = drtrn["year"].ToString();
                            trn.Place = drtrn["place"].ToString();
                            trn.Country = drtrn["country"].ToString();
                            trn.Finan = drtrn["finan"].ToString();
                            trn.Amount = drtrn["amount"].ToString();
                            trn.DuYear = drtrn["du_year"].ToString();
                            trn.DuMonth = drtrn["du_month"].ToString();
                            trn.DuDay = drtrn["du_day"].ToString();
                            TrainManager.CreateTrain(trn);
                        }
                    }
                }
                if (TransSt == "O")
                {
                    dgEmpTrans.EditIndex = -1;
                    dgEmpTrans.ShowFooter = false;
                    TransfManager.DeleteTransf(emp.EmpNo);
                    Transf trans;
                    DataTable dt = (DataTable)ViewState["dttrans"];
                    foreach (DataRow drtrn in dt.Rows)
                    {
                        if (drtrn["order_no"].ToString() != "")
                        {
                            trans = new Transf();
                            trans.EmpNo = emp.EmpNo;
                            trans.OrderNo = drtrn["order_no"].ToString();
                            trans.TransDate = drtrn["trans_date"].ToString();
                            trans.TransProm = drtrn["trans_prom"].ToString();
                            trans.BranchCode = drtrn["BRANCH_CODE"].ToString();
                            trans.DesigCode = drtrn["desig_code"].ToString();
                            TransfManager.CreateTransf(trans);
                        }
                    }
                }
                if (PromSt == "O")
                {
                    dgEmpProm.EditIndex = -1;
                    dgEmpProm.ShowFooter = false;
                    PromManager.DeleteProm(emp.EmpNo);
                    Prom prm;
                    DataTable dt = (DataTable)ViewState["dtprom"];
                    foreach (DataRow drprm in dt.Rows)
                    {
                        if (drprm["off_ord_no"].ToString() != "")
                        {
                            prm = new Prom();
                            prm.EmpNo = emp.EmpNo;
                            prm.OffOrdNo = drprm["off_ord_no"].ToString();
                            prm.JoiningDate = drprm["joining_date"].ToString();
                            prm.JoiningBranch = drprm["joining_branch"].ToString();
                            prm.JoiningDesig = drprm["joining_desig"].ToString();
                            prm.PayScale = drprm["pay_scale"].ToString();
                            PromManager.CreateProm(prm);
                        }
                    }
                }
                if (QtrSt == "O")
                {
                    dgEmpQtr.EditIndex = -1;
                    dgEmpQtr.ShowFooter = false;
                    QtrManager.DeleteQtr(emp.EmpNo);
                    Qtr qtr;
                    DataTable dt = (DataTable)ViewState["dtqtr"];
                    foreach (DataRow drqtr in dt.Rows)
                    {
                        if (drqtr["allot_ref"].ToString() != "")
                        {
                            qtr = new Qtr();
                            qtr.EmpNo = emp.EmpNo;
                            qtr.AllotRef = drqtr["allot_ref"].ToString();
                            qtr.RefDate = drqtr["ref_date"].ToString();
                            qtr.PostDate = drqtr["post_date"].ToString();
                            qtr.Locat = drqtr["locat"].ToString();
                            qtr.Road = drqtr["road"].ToString();
                            qtr.Build = drqtr["build"].ToString();
                            qtr.Flat = drqtr["flat"].ToString();
                            qtr.FlatTyp = drqtr["flat_typ"].ToString();
                            qtr.Sizee = drqtr["sizee"].ToString();
                            QtrManager.CreateQtr(qtr);
                        }
                    }
                }
                if (SuspSt == "O")
                {
                    dgEmpSusp.EditIndex = -1;
                    dgEmpSusp.ShowFooter = false;
                    SuspManager.DeleteSusp(emp.EmpNo);
                    Susp susp;
                    DataTable dt = (DataTable)ViewState["dtsusp"];
                    foreach (DataRow drsusp in dt.Rows)
                    {
                        if (drsusp["off_order_no"].ToString() != "")
                        {
                            susp = new Susp();
                            susp.EmpNo = emp.EmpNo;
                            susp.OffOrderNo = drsusp["off_order_no"].ToString();
                            susp.SuspenDate = drsusp["suspen_date"].ToString();
                            susp.SuspenClause = drsusp["suspen_clause"].ToString();
                            susp.WithdrawOrderNo = drsusp["withdraw_order_no"].ToString();
                            susp.WithDate = drsusp["with_date"].ToString();
                            susp.Punishment = drsusp["punishment"].ToString();
                            SuspManager.CreateSusp(susp);
                        }
                    }
                }
                if (MembSt == "O")
                {
                    dgMember.EditIndex = -1;
                    dgMember.ShowFooter = false;
                    empAssoManager.DeleteEmpAssos(emp.EmpNo);
                    empAsso asso;
                    DataTable dt = (DataTable)ViewState["dtmemb"];
                    foreach (DataRow drmem in dt.Rows)
                    {
                        if (drmem["asso_id"].ToString() != "")
                        {
                            asso = new empAsso();
                            asso.EmpNo = emp.EmpNo;
                            asso.AssoId = drmem["asso_id"].ToString();
                            asso.MemberNo = drmem["member_no"].ToString();
                            empAssoManager.CreateEmpAsso(asso);
                        }
                    }
                }
                BtnReset_Click(sender, e);
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are updated suceessfullly!!');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You Have No Enough Permission to Edit the Record!!');", true);
            }
        }
        else
        {
            if (per.AllowAdd == "Y")
            {
                //string IdGlCoa;
                string a = txtEmpBirthDt.Text;
                if (txtFName.Text != "")
                {
                    emp = new Emp();
                    emp.FName = txtFName.Text;
                    emp.Sex = ddlSex.SelectedValue;
                    emp.EmpBirthDt = a;
                    emp.PlaceOfBirth = txtPlaceOfBirth.Text; 
                    emp.ReligionCode = ddlReligionCode.SelectedValue;
                    emp.BloodGroup = ddlBloodGroup.SelectedValue; 
                    emp.MaritalStatusCode = ddlMaritalStatusCode.SelectedValue;
                    emp.SpouseName = txtSpouseName.Text; 
                    emp.FhName = txtFhName.Text; 
                    emp.MhName = txtMhName.Text;
                    emp.JoinDate = txtJoinDate.Text; 
                    emp.JoinDesigCode = ddlJoinDesigCode.SelectedValue;
                    emp.JobStatus = ddlJobStatus.SelectedValue; 
                    emp.ConfirmDate = txtConfirmDate.Text;
                    emp.EmpCat = ddlEmpCat.SelectedValue; 
                    emp.PfNo = txtPfNo.Text;
                    emp.PassNo = txtPassNo.Text; 
                    emp.DrivLicNo = txtDrivLicNo.Text; 
                    emp.NationalId = txtNationalId.Text; 
                    emp.Nationality = txtNationality.Text;
                    emp.PerLoc = txtPerLoc.Text; 
                    emp.PerDistCode = ddlPerDistCode.SelectedValue; 
                    emp.PerThanaCode = ddlPerThanaCode.SelectedValue;
                    emp.ZipAreaCode = txtZipAreaCode.Text; 
                    emp.MailLoc = txtMailLoc.Text; 
                    emp.MailDistCode = ddlMailDistCode.SelectedValue;
                    emp.MailThanaCode = ddlMailThanaCode.SelectedValue; 
                    emp.ResPhNo = txtResPhNo.Text;
                    emp.Mobile = txtMobile.Text;
                    emp.EMail = txtEMail.Text; 
                    emp.BankBranchNo = ddlBankNo.SelectedValue; 
                    emp.BankAccNo = txtBankAccNo.Text;
                    emp.EmpInsuredDt = txtEmpInsureDt.Text; 
                    emp.SpouseInsDt = txtSpouseInsDt.Text;
                    emp.SenrSlNo = txtSenrSlNo.Text;
                    emp.PrstDesigCode = ddlPrstDesigCode.SelectedValue;
                    emp.BranchKey = ddlBranchKey.SelectedValue;
                    emp.LprDate = txtLprDate.Text; 
                    emp.PersoneelFileNo = txtPersoneelFileNo.Text; 
                    emp.TinNo = txtTinNo.Text;
                    emp.County = ddlCountry.SelectedValue;
                    if (string.IsNullOrEmpty(txtBasicSalary.Text))
                    {
                        emp.BasicSalary = "0";
                    }
                    else
                    {
                        emp.BasicSalary = txtBasicSalary.Text;
                    }
                    byte[] image = null;
                    if (Session["empPhoto"] != " ")
                    {
                        emp.EmpPhoto = (byte[])Session["empPhoto"];
                        image = (byte[])Session["empPhoto"];
                    }
                    if (Session["empSigna"] != " ")
                    {
                        emp.SpecSigna = (byte[])Session["empSigna"];
                        image = (byte[])Session["empSigna"];
                    }

                    //emp.EmpPhoto = (byte[])Session["empPhoto"]; 
                    //emp.SpecSigna = (byte[])Session["empSigna"];
                    txtEmpBirthDt.Text = a;
                    emp.EmpCode = txtEmpNo.Text;
                    emp.MailPostCode = txtMailPostCode.Text;
                    //*********** Auto Coa generate off **********//
                    //IdGlCoa = IdManager.getAutoIdWithParameter("60", "GL_SEG_COA", "SEG_COA_CODE", glCode, "0000", "5");                   
                    emp.GlCoaCode = txtGlCoa.Text;
                    emp.EntryBy = Session["user"].ToString();

                    //**************************** Save Employee *****************//
                    emp.EmpNo = EmpManager.CreateEmp(emp);

                    if (ViewState["Document"] != null)
                    {
                        DataTable dtDocument = (DataTable)ViewState["Document"];
                        EmpManager.GetSaveEmployeeDocument(dtDocument, emp.EmpNo);
                    }

                    EduManager.DeleteEdu(emp.EmpNo);
                    Edu edu;
                    DataTable dtedu = (DataTable)ViewState["dtedu"];
                    foreach (DataRow dr in dtedu.Rows)
                    {
                        if (dr["exam_code"].ToString() != "")
                        {
                            edu = new Edu();
                            edu.EmpNo = emp.EmpNo;
                            edu.ExamCode = dr["exam_code"].ToString();
                            edu.GroupName = dr["group_name"].ToString();
                            edu.Institute = dr["institute"].ToString();
                            edu.PassYear = dr["pass_year"].ToString();
                            edu.MainSub = dr["main_sub"].ToString();
                            edu.DivClass = dr["div_class"].ToString();
                            EduManager.CreateEdu(edu);
                        }
                    }
                    dgEmpFam.ShowFooter = false;
                    dgEmpFam.EditIndex = -1;
                    FamManager.DeleteFam(emp.EmpNo);
                    Fam fam;
                    DataTable dtfam = (DataTable)ViewState["dtfam"];
                    foreach (DataRow dr in dtfam.Rows)
                    {
                        if (dr["rel_name"].ToString() != "")
                        {
                            fam = new Fam();
                            fam.EmpNo = emp.EmpNo;
                            fam.RelName = dr["rel_name"].ToString();
                            fam.Relation = dr["relation"].ToString();
                            fam.BirthDt = dr["birth_dt"].ToString();
                            fam.Age = dr["age"].ToString();
                            fam.Occupation = dr["occupation"].ToString();
                            FamManager.CreateFam(fam);
                        }
                    }

                    ExperManager.DeleteExper(emp.EmpNo);
                    Exper exp;
                    dgEmpExp.EditIndex = -1;
                    dgEmpExp.ShowFooter = false;
                    DataTable dtexp = (DataTable)ViewState["dtexp"];
                    foreach (DataRow dr in dtexp.Rows)
                    {
                        if (dr["orga_name"].ToString() != "")
                        {
                            exp = new Exper();
                            exp.EmpNo = emp.EmpNo;
                            exp.OrgaName = dr["orga_name"].ToString();
                            exp.PositionHeld = dr["position_held"].ToString();
                            exp.FromDt = dr["from_dt"].ToString();
                            exp.ToDt = dr["to_dt"].ToString();
                            exp.PayScale = dr["pay_scale"].ToString();
                            ExperManager.CreateExper(exp);
                        }
                    }
                    dgEmpTrain.EditIndex = -1;
                    dgEmpTrain.ShowFooter = false;
                    TrainManager.DeleteTrain(emp.EmpNo);
                    Train trn;
                    DataTable dtrain = (DataTable)ViewState["dttrain"];
                    foreach (DataRow dr in dtrain.Rows)
                    {
                        if (dr["train_title"].ToString() != "")
                        {
                            trn = new Train();
                            trn.EmpNo = emp.EmpNo;
                            trn.TrainTitle = dr["train_title"].ToString();
                            trn.Year = dr["year"].ToString();
                            trn.Place = dr["place"].ToString();
                            trn.Country = dr["country"].ToString();
                            trn.Finan = dr["finan"].ToString();
                            trn.Amount = dr["amount"].ToString();
                            trn.DuYear = dr["du_year"].ToString();
                            trn.DuMonth = dr["du_month"].ToString();
                            trn.DuDay = dr["du_day"].ToString();
                            TrainManager.CreateTrain(trn);
                        }
                    }
                    dgEmpTrans.EditIndex = -1;
                    dgEmpTrans.ShowFooter = false;
                    TransfManager.DeleteTransf(emp.EmpNo);
                    Transf trans;
                    DataTable dtrans = (DataTable)ViewState["dttrans"];
                    foreach (DataRow dr in dtrans.Rows)
                    {
                        if (dr["order_no"].ToString() != "")
                        {
                            trans = new Transf();
                            trans.EmpNo = emp.EmpNo;
                            trans.OrderNo = dr["order_no"].ToString();
                            trans.TransDate = dr["trans_date"].ToString();
                            trans.TransProm = dr["trans_prom"].ToString();
                            trans.BranchCode = dr["BRANCH_CODE"].ToString();
                            trans.DesigCode = dr["desig_code"].ToString();
                            TransfManager.CreateTransf(trans);
                        }
                    }
                    dgEmpProm.EditIndex = -1;
                    dgEmpProm.ShowFooter = false;
                    PromManager.DeleteProm(emp.EmpNo);
                    Prom prm;
                    DataTable dtprom = (DataTable)ViewState["dtprom"];
                    foreach (DataRow dr in dtprom.Rows)
                    {
                        if (dr["off_ord_no"].ToString() != "")
                        {
                            prm = new Prom();
                            prm.EmpNo = emp.EmpNo;
                            prm.OffOrdNo = dr["off_ord_no"].ToString();
                            prm.JoiningDate = dr["joining_date"].ToString();
                            prm.JoiningBranch = dr["joining_branch"].ToString();
                            prm.JoiningDesig = dr["joining_desig"].ToString();
                            prm.PayScale = dr["pay_scale"].ToString();
                            PromManager.CreateProm(prm);
                        }
                    }
                    dgEmpQtr.EditIndex = -1;
                    dgEmpQtr.ShowFooter = false;
                    QtrManager.DeleteQtr(emp.EmpNo);
                    Qtr qtr;
                    DataTable dtqtr = (DataTable)ViewState["dtqtr"];
                    foreach (DataRow dr in dtqtr.Rows)
                    {
                        if (dr["allot_ref"].ToString() != "")
                        {
                            qtr = new Qtr();
                            qtr.EmpNo = emp.EmpNo;
                            qtr.AllotRef = dr["allot_ref"].ToString();
                            qtr.RefDate = dr["ref_date"].ToString();
                            qtr.PostDate = dr["post_date"].ToString();
                            qtr.Locat = dr["locat"].ToString();
                            qtr.Road = dr["road"].ToString();
                            qtr.Build = dr["build"].ToString();
                            qtr.Flat = dr["flat"].ToString();
                            qtr.FlatTyp = dr["flat_typ"].ToString();
                            qtr.Sizee = dr["sizee"].ToString();
                            QtrManager.CreateQtr(qtr);
                        }
                    }
                    dgEmpSusp.EditIndex = -1;
                    dgEmpSusp.ShowFooter = false;
                    SuspManager.DeleteSusp(emp.EmpNo);
                    Susp susp;
                    //DataTable dtsusp = (DataTable)ViewState["dtsusp"];
                    //foreach (DataRow dr in dtsusp.Rows)
                    //{
                    //    if (dr["off_order_no"].ToString() != "")
                    //    {
                    //        susp = new Susp();
                    //        susp.EmpNo = emp.EmpNo;
                    //        susp.OffOrderNo = dr["off_order_no"].ToString();
                    //        susp.SuspenDate = dr["suspen_date"].ToString();
                    //        susp.SuspenClause = dr["suspen_clause"].ToString();
                    //        susp.WithdrawOrderNo = dr["withdraw_order_no"].ToString();
                    //        susp.WithDate = dr["with_date"].ToString();
                    //        susp.Punishment = dr["punishment"].ToString();
                    //        SuspManager.CreateSusp(susp);
                    //    }
                    //}
                    dgMember.EditIndex = -1;
                    dgMember.ShowFooter = false;
                    empAssoManager.DeleteEmpAssos(emp.EmpNo);
                    empAsso asso;
                    //DataTable dtmem = (DataTable)ViewState["dtmemb"];
                    //foreach (DataRow dr in dtmem.Rows)
                    //{
                    //    if (dr["asso_id"].ToString() != "")
                    //    {
                    //        asso = new empAsso();
                    //        asso.EmpNo = emp.EmpNo;
                    //        asso.AssoId = dr["asso_id"].ToString();
                    //        asso.MemberNo = dr["member_no"].ToString();
                    //        empAssoManager.CreateEmpAsso(asso);
                    //    }
                    //}
                    BtnReset_Click(sender, e);
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record(s) is/are saved suceessfullly!!');", true);
                    BtnSave.Enabled = false;
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please input employee first name and date of birth!!');", true);
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You Have No Enough Permission to Insert Record Here!!');", true);
            }
        }       
    }
    private string getId(string f, string m, string l, string d)
    {
        string n = (f.Trim() + " " + m.Trim() + " " + l.Trim()).ToString().Trim().ToUpper().Replace("MD.", "").Replace("MD", "").Replace("MOHAMMED", "").Replace("MOHAMMAD", "").Replace("MRS.", "").Replace("MRS", "").Replace("MISS.", "").Replace("MISS", "").Replace("SREE", "").Replace("SRE", "").Replace("SRI.", "").Trim();
        string[] nam = n.Split(' ');
        string name = "";
        if (nam.Length > 1)
        {
            name = nam[0].Substring(0, 1) + nam[1].Substring(0, 1) + d;
        }
        else
        {
            name = nam[0].Substring(0, 2) + d;
        }
        string sl = IdManager.GetNextEmpSl(name).ToString().PadLeft(3, '0');
        return name + sl;
    }
    protected void BtnFind_Click(object sender, EventArgs e)
     {
        dgEmp.DataBind();
        lblTranStatus.Visible = false;
        //if (txtEmpNo.Text != string.Empty | txtFName.Text != String.Empty |txtEmpBirthDt.Text != String.Empty)
        //{
            //DataTable dt = EmpManager.getEmps(txtEmpNo.Text, (txtFName.Text + " " + txtMName.Text).ToString().Trim(),txtEmpBirthDt.Text);
            DataTable dt = EmpManager.GetEmployeeInformationForSpecificEmployee(lblID.Text);
            if (dt.Rows.Count == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('No Employee found in these criteria!!');", true);
                EmpTabContainer.Visible = true;
            }
            else if (dt.Rows.Count > 1)
            {
                
                EmpTabContainer.ActiveTabIndex = 10;                
                dgEmp.DataSource = dt;
                dgEmp.DataBind();
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('" + dt.Rows.Count.ToString() + " records found!!');", true);
            }
            else if (dt.Rows.Count == 1)
            {
                EmpTabContainer.ActiveTabIndex = 0;
                EmpTabContainer.Visible = true;
                Emp emp = EmpManager.getEmp(dt.Rows[0]["EMP_NO"].ToString());
                if (emp != null)
                {
                    hfEmpNo.Value = dt.Rows[0]["EMP_NO"].ToString();
                    txtEmpNo.Text = emp.EmpCode;
                    txtFName.Text = emp.FName;
                    txtMailPostCode.Text = emp.MailPostCode;
                    ddlSex.SelectedValue = emp.Sex;
                    txtEmpBirthDt.Text = emp.EmpBirthDt;
                    txtPlaceOfBirth.Text = emp.PlaceOfBirth;
                    if (emp.ReligionCode != null)
                    {
                        if (!emp.ReligionCode.Equals("0"))
                        {
                            ddlReligionCode.SelectedValue = emp.ReligionCode;
                        }
                    }
                    ddlBloodGroup.SelectedValue = emp.BloodGroup;
                    ddlMaritalStatusCode.SelectedValue = emp.MaritalStatusCode;
                    txtSpouseName.Text = emp.SpouseName;
                    txtFhName.Text = emp.FhName;
                    txtMhName.Text = emp.MhName;
                    txtJoinDate.Text = emp.JoinDate;
                    ddlJoinDesigCode.SelectedValue = emp.JoinDesigCode;
                    ddlJobStatus.SelectedValue = emp.JobStatus;
                    txtConfirmDate.Text = emp.ConfirmDate;
                    ddlEmpCat.SelectedValue = emp.EmpCat;
                    txtPfNo.Text = emp.PfNo;
                    ddlCountry.SelectedValue = emp.County;
                    txtGlCoa.Text = emp.GlCoaCode;
                    empPhoto = (byte[])emp.EmpPhoto;
                    empSigna = (byte[])emp.SpecSigna;
                    Session["empPhoto"] = empPhoto;
                    Session["empSigna"] = empSigna;
                    if (empPhoto != null)
                    {
                        string base64String = Convert.ToBase64String(empPhoto, 0, empPhoto.Length);
                        imgEmp.ImageUrl = "data:image/png;base64," + base64String;
                    }
                    if (empSigna != null)
                    {
                        string base64String = Convert.ToBase64String(empSigna, 0, empSigna.Length);
                        imgSig.ImageUrl = "data:image/png;base64," + base64String;
                    }
                    txtPerLoc.Text = emp.PerLoc;
                    ddlPerDistCode.SelectedValue = emp.PerDistCode;

                    //ddlPerThanaCode.Items.Clear();
                    //string queryPThana = "select '' th_code, '' th_name union select thana_code, thana_name from pmis_thana_code where district_code like '%" + ddlPerDistCode.SelectedValue + "%' order by 2 desc";
                    ddlPerThanaCode.Items.Clear();
                    string queryThana = "select '' THANA_CODE,'' THANA_NAME union select THANA_CODE, THANA_NAME from dbo.THANA_CODE WHERE DISTRICT_CODE ='" + emp.PerDistCode + "' order by 2 ";
                    util.PopulationDropDownList(ddlPerThanaCode, "Thana", queryThana, "THANA_NAME", "THANA_CODE");                    
                    ddlPerThanaCode.SelectedValue = emp.PerThanaCode;
                    txtZipAreaCode.Text = emp.ZipAreaCode;
                    txtMailLoc.Text = emp.MailLoc;
                    try
                    {
                        if (!emp.MailDistCode.Equals("0"))
                        {
                            ddlMailDistCode.SelectedValue = emp.MailDistCode;
                        }
                    }
                    catch (Exception)
                    {

                        ddlMailDistCode.SelectedValue = null;
                    }
                   
                    ddlMailThanaCode.Items.Clear();
                    //string queryMThana = "select '' th_code, '' th_name union select thana_code, thana_name from pmis_thana_code where district_code like '%" + ddlMailDistCode.SelectedValue + "%' order by 2 desc";
                    string queryMThana = "select '' THANA_CODE,'' THANA_NAME union select THANA_CODE, THANA_NAME from dbo.THANA_CODE WHERE DISTRICT_CODE= '" + emp.MailDistCode + "' order by 2 desc";
                    util.PopulationDropDownList(ddlMailThanaCode, "Thana", queryMThana, "THANA_NAME", "THANA_CODE");
                    
                    try
                    {
                        if (!emp.MailThanaCode.Equals("0"))
                        {
                            ddlMailThanaCode.SelectedValue = emp.MailThanaCode;
                        }
                    }
                    catch (Exception)
                    {

                        ddlMailThanaCode.SelectedValue = null;
                    }

                    txtResPhNo.Text = emp.ResPhNo;
                    txtMobile.Text = emp.Mobile;
                    txtEMail.Text = emp.EMail;
                    ddlBankNo.SelectedValue = emp.BankBranchNo;
                    txtBankAccNo.Text = emp.BankAccNo;
                    txtEmpInsureDt.Text = emp.EmpInsuredDt;
                    txtSpouseInsDt.Text = emp.SpouseInsDt;
                    txtSenrSlNo.Text = emp.SenrSlNo;
                    ddlPrstDesigCode.SelectedValue = emp.PrstDesigCode;
                    ddlBranchKey.SelectedValue = emp.BranchKey;
                    txtLprDate.Text = emp.LprDate;
                    txtPassNo.Text = emp.PassNo;
                    txtDrivLicNo.Text = emp.DrivLicNo;
                    txtNationalId.Text = emp.NationalId;
                    txtNationality.Text = emp.Nationality;
                    txtTinNo.Text = emp.TinNo;
                    txtPersoneelFileNo.Text = emp.PersoneelFileNo;
                    txtBasicSalary.Text = emp.BasicSalary;

                    DataTable dtEdu = EduManager.getEdus(emp.EmpNo);
                    if (dtEdu.Rows.Count > 0)
                    {
                        dgEmpEdu.DataSource = dtEdu;
                        ViewState["dtedu"] = dtEdu;
                        dgEmpEdu.ShowFooter = false;
                        dgEmpEdu.EditIndex = -1;
                        dgEmpEdu.DataBind();
                    }
                    else
                    {
                        getEmptyEdu();
                    }
                    DataTable dtFam = FamManager.getFams(emp.EmpNo);
                    if (dtFam.Rows.Count > 0)
                    {
                        dgEmpFam.DataSource = dtFam;
                        ViewState["dtfam"] = dtFam;
                        dgEmpFam.ShowFooter = false;
                        dgEmpFam.EditIndex = -1;
                        dgEmpFam.DataBind();
                    }
                    else
                    {
                        getEmptyFam();
                    }
                    DataTable dtExp = ExperManager.getExpers(emp.EmpNo);
                    if (dtExp.Rows.Count > 0)
                    {
                        dgEmpExp.DataSource = dtExp;
                        ViewState["dtexp"] = dtExp;
                        dgEmpExp.ShowFooter = false;
                        dgEmpExp.EditIndex = -1;
                        dgEmpExp.DataBind();
                    }
                    else
                    {
                        getEmptyExp();
                    }
                    DataTable dtTrain = TrainManager.getTrains(emp.EmpNo);
                    if (dtTrain.Rows.Count > 0)
                    {
                        dgEmpTrain.DataSource = dtTrain;
                        ViewState["dttrain"] = dtTrain;
                        dgEmpTrain.ShowFooter = false;
                        dgEmpTrain.EditIndex = -1;
                        dgEmpTrain.DataBind();
                    }
                    else
                    {
                        getEmptyTrain();
                    }
                    DataTable dtTrans = TransfManager.getTransfs(emp.EmpNo);
                    if (dtTrans.Rows.Count > 0)
                    {
                        dgEmpTrans.DataSource = dtTrans;
                        ViewState["dttrans"] = dtTrans;
                        dgEmpTrans.ShowFooter = false;
                        dgEmpTrans.EditIndex = -1;
                        dgEmpTrans.DataBind();
                    }
                    else
                    {
                        getEmptyTrans();
                    }
                    DataTable dtProm = PromManager.getProms(emp.EmpNo);
                    if (dtProm.Rows.Count > 0)
                    {
                        dgEmpProm.DataSource = dtProm;
                        ViewState["dtprom"] = dtProm;
                        dgEmpProm.ShowFooter = false;
                        dgEmpProm.EditIndex = -1;
                        dgEmpProm.DataBind();
                    }
                    else
                    {
                        getEmptyProm();
                    }
                    DataTable dtQtr = QtrManager.getQtrs(emp.EmpNo);
                    if (dtQtr.Rows.Count > 0)
                    {
                        dgEmpQtr.DataSource = dtQtr;
                        ViewState["dtqtr"] = dtQtr;
                        dgEmpQtr.ShowFooter = false;
                        dgEmpQtr.EditIndex = -1;
                        dgEmpQtr.DataBind();
                    }
                    else
                    {
                        getEmptyQtr();
                    }
                    DataTable dtSusp = SuspManager.getSusps(emp.EmpNo);
                    if (dtSusp.Rows.Count > 0)
                    {
                        dgEmpSusp.DataSource = dtSusp;
                        ViewState["dtsusp"] = dtSusp;
                        dgEmpSusp.ShowFooter = false;
                        dgEmpSusp.EditIndex = -1;
                        dgEmpSusp.DataBind();
                    }
                    else
                    {
                        //getEmptySusp();
                    }

                    DataTable dtDocument = EmpManager.getDocument(emp.EmpNo);
                    if (dtDocument.Rows.Count > 0)
                    {
                        dgQuestion.DataSource = dtDocument;
                        ViewState["Document"] = dtDocument;
                        dgQuestion.DataBind();
                    }
                    else
                    {
                        getEmptyDocumentTable();
                    }
                }
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('At least input any clue in Employee No, First Name, Middle Name, or Date of Birth!!');", true);
        }
    }
    protected void BtnDelete_Click(object sender, EventArgs e)
    {
        if (per.AllowDelete == "Y")
        {

            Emp emp = EmpManager.getEmp(ViewState["Emp_No"].ToString());
                if (emp != null)
                {
                    EduManager.DeleteEdu(emp.EmpNo);
                    FamManager.DeleteFam(emp.EmpNo);
                    ExperManager.DeleteExper(emp.EmpNo);
                    TrainManager.DeleteTrain(emp.EmpNo);
                    TransfManager.DeleteTransf(emp.EmpNo);
                    PromManager.DeleteProm(emp.EmpNo);
                    QtrManager.DeleteQtr(emp.EmpNo);
                    SuspManager.DeleteSusp(emp.EmpNo);
                    empAssoManager.DeleteEmpAssos(emp.EmpNo);
                    EmpManager.DeleteEmp(emp);
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Records are deleted suceessfullly!!');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('No Employee found with this ID!!');", true);
                }
            
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('You Have No Enough Permission to Delete!!');", true);
        }        
    }
    protected void BtnReset_Click(object sender, EventArgs e)
    {
        var pageName = System.IO.Path.GetFileName(Request.Url.ToString());
        Response.Redirect(pageName);
    }
    protected void dgEmpEdu_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (ViewState["dtedu"] != null)
        {
            EduSt = "O";            
            string examcode = ((Label)dgEmpEdu.Rows[e.NewEditIndex].FindControl("lblExamCode")).Text;
            string div = ((Label)dgEmpEdu.Rows[e.NewEditIndex].FindControl("lblDivClass")).Text;
            string group = ((Label)dgEmpEdu.Rows[e.NewEditIndex].FindControl("lblGroupName")).Text;
            string ins = ((Label)dgEmpEdu.Rows[e.NewEditIndex].FindControl("lblInstitute")).Text;
            string pass = ((Label)dgEmpEdu.Rows[e.NewEditIndex].FindControl("lblPassYear")).Text;
            string sub = ((Label)dgEmpEdu.Rows[e.NewEditIndex].FindControl("lblMainSub")).Text;
            DataTable dt = (DataTable)ViewState["dtedu"];
            dgEmpEdu.EditIndex = e.NewEditIndex;
            dgEmpEdu.DataSource = dt;
            dgEmpEdu.ShowFooter = false;
            dgEmpEdu.DataBind();
            ((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlExamCode")).Items.Clear();
            string queryExam11 = "select '' exam_code, '' exam_name union select exam_code,exam_name from pmis_exam_code order by 1 desc";
            util.PopulationDropDownList((DropDownList)dgEmpEdu.Rows[e.NewEditIndex].FindControl("ddlExamCode"), "Exam", queryExam11, "exam_name", "exam_code");
            ((DropDownList)dgEmpEdu.Rows[e.NewEditIndex].FindControl("ddlExamCode")).SelectedValue = examcode;

            ((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlDivClass")).Items.Clear();
            string queryDiv12 = "select '' DIVISION_CODE, '' DIVISION_NAME union select DIVISION_CODE,DIVISION_NAME from PMIS_DIVISION_CODE order by 2 asc";
            util.PopulationDropDownList((DropDownList)dgEmpEdu.Rows[e.NewEditIndex].FindControl("ddlDivClass"), "PMIS_DIVISION_CODE", queryDiv12, "DIVISION_NAME", "DIVISION_CODE");
            ((DropDownList)dgEmpEdu.Rows[e.NewEditIndex].FindControl("ddlDivClass")).SelectedValue = div;
            //((DropDownList)dgEmpEdu.Rows[e.NewEditIndex].FindControl("ddlDivClass")).SelectedItem.Text = div;
            ((TextBox)dgEmpEdu.Rows[e.NewEditIndex].FindControl("txtGroupName")).Text = group;
            ((TextBox)dgEmpEdu.Rows[e.NewEditIndex].FindControl("txtInstitute")).Text = ins;
            ((TextBox)dgEmpEdu.Rows[e.NewEditIndex].FindControl("txtPassYear")).Text = pass;
            ((TextBox)dgEmpEdu.Rows[e.NewEditIndex].FindControl("txtMainSub")).Text = sub;
        }
    }
    protected void dgEmpEdu_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        if (ViewState["dtedu"] != null)
        {
            EduSt = "O";
            DataTable dt = (DataTable)ViewState["dtedu"];
            if (dt.Rows.Count > 1)
            {
                dgEmpEdu.DataSource = dt;
                dgEmpEdu.EditIndex = -1;
                dgEmpEdu.ShowFooter = false;
                dgEmpEdu.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["exam_code"].ToString() != "")
            {
                dgEmpEdu.DataSource = dt;
                dgEmpEdu.EditIndex = -1;
                dgEmpEdu.ShowFooter = false;
                dgEmpEdu.DataBind();
            } 
            else
            {
                getEmptyEdu();
                ((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlExamCode")).Items.Clear();
                string queryExam = "select '' exam_code, '' exam_name union select exam_code,exam_name from pmis_exam_code order by 1 asc";
                util.PopulationDropDownList((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlExamCode"), "Exam", queryExam, "exam_name", "exam_code");

                ((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlDivClass")).Items.Clear();
                string queryDiv = "select '' DIVISION_CODE, '' DIVISION_NAME union select DIVISION_CODE,DIVISION_NAME from PMIS_DIVISION_CODE order by 2 asc";
                util.PopulationDropDownList((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlDivClass"), "PMIS_DIVISION_CODE", queryDiv, "DIVISION_NAME", "DIVISION_CODE");
            }
        }
    }
    protected void dgEmpEdu_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["dtedu"] != null)
        {
            EduSt = "O";
            DataTable dt = (DataTable)ViewState["dtedu"];
            DataRow dr = dt.Rows[dgEmpEdu.Rows[e.RowIndex].DataItemIndex];
            dt.Rows.Remove(dr);
            if (dt.Rows.Count > 1) 
            {
                dgEmpEdu.DataSource = dt;
                dgEmpEdu.EditIndex = -1;
                dgEmpEdu.ShowFooter = false;
                dgEmpEdu.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["exam_code"].ToString() != "")
            {
                dgEmpEdu.DataSource = dt;
                dgEmpEdu.EditIndex = -1;
                dgEmpEdu.ShowFooter = false;
                dgEmpEdu.DataBind();
            } 
            else
            {
                getEmptyEdu();
                ((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlExamCode")).Items.Clear();
                string queryExam = "select '' exam_code, '' exam_name union select exam_code,exam_name from pmis_exam_code order by 1 asc";
                util.PopulationDropDownList((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlExamCode"), "Exam", queryExam, "exam_name", "exam_code");

                ((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlDivClass")).Items.Clear();
                string queryDiv = "select '' DIVISION_CODE, '' DIVISION_NAME union select DIVISION_CODE,DIVISION_NAME from PMIS_DIVISION_CODE order by 2 asc";
                util.PopulationDropDownList((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlDivClass"), "PMIS_DIVISION_CODE", queryDiv, "DIVISION_NAME", "DIVISION_CODE");
            }
        }
        //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is deleted provisionally, to permanently delete please click on Save Link!!');", true);
    }
    protected void dgEmpEdu_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (ViewState["dtedu"] != null)
        {
            EduSt = "O";
            GridViewRow gvr = dgEmpEdu.Rows[e.RowIndex];
            DataTable dt = (DataTable)ViewState["dtedu"];
            DataRow dr = dt.Rows[dgEmpEdu.Rows[e.RowIndex].DataItemIndex];

            dr["exam_name"] = ((DropDownList)gvr.FindControl("ddlExamCode")).SelectedItem.Text;
            dr["exam_code"] = ((DropDownList)gvr.FindControl("ddlExamCode")).SelectedValue;
            dr["division_name"] = ((DropDownList)gvr.FindControl("ddlDivClass")).SelectedItem.Text;
            dr["div_class"] = ((DropDownList)gvr.FindControl("ddlDivClass")).SelectedValue;
            //dr["div_class"] = ((DropDownList)gvr.FindControl("ddlDivClass")).SelectedItem.Text;
            dr["group_name"] = ((TextBox)gvr.FindControl("txtGroupName")).Text;
            dr["institute"] = ((TextBox)gvr.FindControl("txtInstitute")).Text;
            dr["pass_year"] = ((TextBox)gvr.FindControl("txtPassYear")).Text;
            dr["main_sub"] = ((TextBox)gvr.FindControl("txtMainSub")).Text;
            dgEmpEdu.DataSource = dt;
            dgEmpEdu.EditIndex = -1;
            dgEmpEdu.ShowFooter = false;
            dgEmpEdu.DataBind();
        } 
        //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is updated provisionally, to save in database please click on Save Link!!');", true);
    }
    protected void dgEmpEdu_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((DataRowView)e.Row.DataItem)["exam_code"].ToString() == String.Empty)
            {
                e.Row.Visible = false;
            }
        }
    }
    protected void dgEmpEdu_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (ViewState["dtedu"] != null)
        {            
            DataTable dt = (DataTable)ViewState["dtedu"];
            if (e.CommandName == "AddNew")
            {
                EduSt = "O";
                dgEmpEdu.DataSource = dt;
                dgEmpEdu.DataBind();
                dgEmpEdu.ShowFooter = true;
                dgEmpEdu.FooterRow.Visible = true;
                ((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlExamCode")).Items.Clear();
                string queryExam = "select '' exam_code, '' exam_name union select exam_code,exam_name from pmis_exam_code order by 2 asc";
                util.PopulationDropDownList((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlExamCode"), "Exam", queryExam, "exam_name", "exam_code");

                ((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlDivClass")).Items.Clear();
                string queryDiv = "select '' DIVISION_CODE, '' DIVISION_NAME union select DIVISION_CODE,DIVISION_NAME from PMIS_DIVISION_CODE order by 2 asc";
                util.PopulationDropDownList((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlDivClass"), "PMIS_DIVISION_CODE", queryDiv, "DIVISION_NAME", "DIVISION_CODE");
            }
            else if (e.CommandName.Equals("Insert"))
            {
                EduSt = "O";
                GridViewRow gvr = dgEmpEdu.FooterRow;
                DataRow dr = dt.NewRow();

                dr["exam_name"] = ((DropDownList)gvr.FindControl("ddlExamCode")).SelectedItem.Text;
                dr["exam_code"] = ((DropDownList)gvr.FindControl("ddlExamCode")).SelectedValue;
                
                dr["division_name"] = ((DropDownList)gvr.FindControl("ddlDivClass")).SelectedItem.Text;
                dr["div_class"] = ((DropDownList)gvr.FindControl("ddlDivClass")).SelectedValue;

                //dr["div_class"] = ((DropDownList)gvr.FindControl("ddlDivClass")).SelectedItem.Text;
                dr["group_name"] = ((TextBox)gvr.FindControl("txtGroupName")).Text;
                dr["institute"] = ((TextBox)gvr.FindControl("txtInstitute")).Text;
                dr["pass_year"] = ((TextBox)gvr.FindControl("txtPassYear")).Text;
                dr["main_sub"] = ((TextBox)gvr.FindControl("txtMainSub")).Text;
                dt.Rows.Add(dr);
                dgEmpEdu.DataSource = dt;
                dgEmpEdu.ShowFooter = false;
                dgEmpEdu.DataBind();
                //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is inserted provisionally, to save in database please click on Save Link!!');", true);
            }
        }
    }
    private void getEmptyEdu()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("exam_code", typeof(string));
        dt.Columns.Add("exam_name", typeof(string));
        dt.Columns.Add("group_name", typeof(string));
        dt.Columns.Add("institute", typeof(string));
        dt.Columns.Add("pass_year", typeof(string));
        dt.Columns.Add("main_sub", typeof(string));
        dt.Columns.Add("div_class", typeof(string));
        dt.Columns.Add("division_name", typeof(string));
        DataRow dr = dt.NewRow();
        dt.Rows.Add(dr);
        dgEmpEdu.DataSource = dt;
        ViewState["dtedu"] = dt;
        dgEmpEdu.EditIndex = -1;
        dgEmpEdu.ShowFooter = true;
        dgEmpEdu.DataBind();
        ((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlExamCode")).Items.Clear();
        string queryExam = "select '' exam_code, '' exam_name union select exam_code,exam_name from pmis_exam_code order by 1 asc";
        util.PopulationDropDownList((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlExamCode"), "Exam", queryExam, "exam_name", "exam_code");

        ((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlDivClass")).Items.Clear();
        string queryDiv = "select '' DIVISION_CODE, '' DIVISION_NAME union select DIVISION_CODE,DIVISION_NAME from PMIS_DIVISION_CODE order by 2 asc";
        util.PopulationDropDownList((DropDownList)dgEmpEdu.FooterRow.FindControl("ddlDivClass"), "PMIS_DIVISION_CODE", queryDiv, "DIVISION_NAME", "DIVISION_CODE");
    }
    private void getEmptyFam()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("rel_name", typeof(string));
        dt.Columns.Add("relation", typeof(string));
        dt.Columns.Add("birth_dt", typeof(string));
        dt.Columns.Add("age", typeof(string));
        dt.Columns.Add("occupation", typeof(string));
        DataRow dr = dt.NewRow();
        dt.Rows.Add(dr);
        dgEmpFam.DataSource = dt;
        ViewState["dtfam"] = dt;
        dgEmpFam.EditIndex = -1;
        dgEmpFam.ShowFooter = true;
        dgEmpFam.DataBind();
        ((TextBox)dgEmpFam.FooterRow.FindControl("txtBirthDt")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpFam.FooterRow.FindControl("txtBirthDt")).ClientID + "')");
    }
    private void getEmptyExp()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("orga_name", typeof(string));
        dt.Columns.Add("position_held", typeof(string));
        dt.Columns.Add("from_dt", typeof(string));
        dt.Columns.Add("to_dt", typeof(string));
        dt.Columns.Add("pay_scale", typeof(string));
        DataRow dr = dt.NewRow();
        dt.Rows.Add(dr);
        dgEmpExp.DataSource = dt;
        ViewState["dtexp"] = dt;
        dgEmpExp.EditIndex = -1;
        dgEmpExp.ShowFooter = true;
        dgEmpExp.DataBind();
        ((TextBox)dgEmpExp.FooterRow.FindControl("txtFromDt")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpExp.FooterRow.FindControl("txtFromDt")).ClientID + "')");
        ((TextBox)dgEmpExp.FooterRow.FindControl("txtToDt")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpExp.FooterRow.FindControl("txtToDt")).ClientID + "')");
    }
    private void getEmptyTrain()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("train_title", typeof(string));
        dt.Columns.Add("year", typeof(string));
        dt.Columns.Add("place", typeof(string));
        dt.Columns.Add("country", typeof(string));
        dt.Columns.Add("finan", typeof(string));
        dt.Columns.Add("amount", typeof(string));
        dt.Columns.Add("du_year", typeof(string));
        dt.Columns.Add("du_month", typeof(string));
        dt.Columns.Add("du_day", typeof(string));
        DataRow dr = dt.NewRow();
        dt.Rows.Add(dr);
        dgEmpTrain.DataSource = dt;
        ViewState["dttrain"] = dt;
        dgEmpTrain.EditIndex = -1;
        dgEmpTrain.ShowFooter = true;
        dgEmpTrain.DataBind();
    }
    private void getEmptyTrans()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("order_no", typeof(string));
        dt.Columns.Add("trans_date", typeof(string));
        dt.Columns.Add("trans_prom", typeof(string));
        dt.Columns.Add("BRANCH_CODE", typeof(string));
        dt.Columns.Add("desig_code", typeof(string));
        DataRow dr = dt.NewRow();
        dt.Rows.Add(dr);
        dgEmpTrans.DataSource = dt;
        ViewState["dttrans"] = dt;
        dgEmpTrans.EditIndex = -1;
        dgEmpTrans.ShowFooter = true;
        dgEmpTrans.DataBind();
        ((TextBox)dgEmpTrans.FooterRow.FindControl("txtTransDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpTrans.FooterRow.FindControl("txtTransDate")).ClientID + "')");
        ((DropDownList)dgEmpTrans.FooterRow.FindControl("ddlBranchKey")).Items.Clear();
        string queryBranch = "select '' BranchKey, '' BranchName union select BranchKey,dbo.initcap(BranchName) BranchName from BranchInfo order by 2 asc";
        util.PopulationDropDownList((DropDownList)dgEmpTrans.FooterRow.FindControl("ddlBranchKey"), "Branch", queryBranch, "BranchName", "BranchKey");

        ((DropDownList)dgEmpTrans.FooterRow.FindControl("ddlDesigCode")).Items.Clear();
        string queryDesig = "select '' desig_code, '' desig_name union select desig_code,dbo.initcap(desig_name) desig_name from pmis_desig_code order by 2 asc";
        util.PopulationDropDownList((DropDownList)dgEmpTrans.FooterRow.FindControl("ddlDesigCode"), "Designation", queryDesig, "desig_name", "desig_code");
    }
    private void getEmptyProm()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("off_ord_no", typeof(string));
        dt.Columns.Add("joining_date", typeof(string));
        dt.Columns.Add("joining_branch", typeof(string));
        dt.Columns.Add("joining_desig", typeof(string));
        dt.Columns.Add("basic_pay", typeof(string));
        dt.Columns.Add("pay_scale", typeof(string));
        DataRow dr = dt.NewRow();
        dt.Rows.Add(dr);
        dgEmpProm.DataSource = dt;
        ViewState["dtprom"] = dt;
        dgEmpProm.EditIndex = -1;
        dgEmpProm.ShowFooter = true;
        dgEmpProm.DataBind();

        ((DropDownList)dgEmpProm.FooterRow.FindControl("ddlJoiningBranch")).Items.Clear();
        string queryBranch = "select '' BranchKey, '' BranchName union select BranchKey,dbo.initcap(BranchName) BranchName from BranchInfo order by 2 asc";
        util.PopulationDropDownList((DropDownList)dgEmpProm.FooterRow.FindControl("ddlJoiningBranch"), "Branch", queryBranch, "BranchName", "BranchKey");


        ((DropDownList)dgEmpProm.FooterRow.FindControl("ddlJoiningDesig")).Items.Clear();
        string queryDesig = "select '' desig_code, '' desig_name union select desig_code,dbo.initcap(desig_name) desig_name from pmis_desig_code order by 2 asc";
        util.PopulationDropDownList((DropDownList)dgEmpProm.FooterRow.FindControl("ddlJoiningDesig"), "Designation", queryDesig, "desig_name", "desig_code");

        ((TextBox)dgEmpProm.FooterRow.FindControl("txtJoiningDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpProm.FooterRow.FindControl("txtJoiningDate")).ClientID + "')");
    }
    private void getEmptyQtr()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("allot_ref", typeof(string));
        dt.Columns.Add("ref_date", typeof(string));
        dt.Columns.Add("post_date", typeof(string));
        dt.Columns.Add("locat", typeof(string));
        dt.Columns.Add("road", typeof(string));
        dt.Columns.Add("build", typeof(string));
        dt.Columns.Add("flat", typeof(string));
        dt.Columns.Add("flat_typ", typeof(string));
        dt.Columns.Add("sizee", typeof(string));
        DataRow dr = dt.NewRow();
        dt.Rows.Add(dr);
        dgEmpQtr.DataSource = dt;
        ViewState["dtqtr"] = dt;
        dgEmpQtr.EditIndex = -1;
        dgEmpQtr.ShowFooter = true;
        dgEmpQtr.DataBind();
        //((TextBox)dgEmpQtr.FooterRow.FindControl("txtRefDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpQtr.FooterRow.FindControl("txtRefDate")).ClientID + "')");
        //((TextBox)dgEmpQtr.FooterRow.FindControl("txtPostDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpQtr.FooterRow.FindControl("txtPostDate")).ClientID + "')");
    }
    //private void getEmptySusp()
    //{
    //    DataTable dt = new DataTable();
    //    dt.Columns.Add("off_order_no", typeof(string));
    //    dt.Columns.Add("suspen_date", typeof(string));
    //    dt.Columns.Add("suspen_clause", typeof(string));
    //    dt.Columns.Add("withdraw_order_no", typeof(string));
    //    dt.Columns.Add("with_date", typeof(string));
    //    dt.Columns.Add("punishment", typeof(string));
    //    DataRow dr = dt.NewRow();
    //    dt.Rows.Add(dr);
    //    dgEmpSusp.DataSource = dt;
    //    ViewState["dtsusp"] = dt;
    //    dgEmpSusp.EditIndex = -1;
    //    dgEmpSusp.ShowFooter = true;
    //    dgEmpSusp.DataBind();
    //    ((TextBox)dgEmpSusp.FooterRow.FindControl("txtSuspenDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpSusp.FooterRow.FindControl("txtSuspenDate")).ClientID + "')");
    //    ((TextBox)dgEmpSusp.FooterRow.FindControl("txtWithDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpSusp.FooterRow.FindControl("txtWithDate")).ClientID + "')");
    //}
    //private void getEmptyMemb()
    //{
    //    DataTable dt = new DataTable();
    //    dt.Columns.Add("asso_id", typeof(string));
    //    dt.Columns.Add("member_no", typeof(string));
    //    DataRow dr = dt.NewRow();
    //    dt.Rows.Add(dr);
    //    dgMember.DataSource = dt;
    //    ViewState["dtmemb"] = dt;
    //    dgMember.EditIndex = -1;
    //    dgMember.ShowFooter = true;
    //    dgMember.DataBind();
    //    ((DropDownList)dgMember.FooterRow.FindControl("ddlAssoId")).Items.Clear();
    //    string queryAsso = "select '' asso_id, '' asso_name union select convert(varchar,asso_id),dbo.initcap(asso_name)+' ('+asso_abvr+')' asso_name from association_info order by 2 asc";
    //    util.PopulationDropDownList((DropDownList)dgMember.FooterRow.FindControl("ddlAssoId"), "Association", queryAsso, "asso_name", "asso_id");
    //}

    protected void dgEmpFam_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        if (ViewState["dtfam"] != null)
        {
            FamSt = "O";
            DataTable dt = (DataTable)ViewState["dtfam"];
            if (dt.Rows.Count > 1)
            {
                dgEmpFam.DataSource = dt;
                dgEmpFam.EditIndex = -1;
                dgEmpFam.ShowFooter = false;
                dgEmpFam.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["rel_name"].ToString() != "")
            {
                dgEmpFam.DataSource = dt;
                dgEmpFam.EditIndex = -1;
                dgEmpFam.ShowFooter = false;
                dgEmpFam.DataBind();
            }
            else
            {
                getEmptyFam();
            }
        }
    }
    protected void dgEmpFam_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["dtfam"] != null)
        {
            FamSt = "O";
            DataTable dt = (DataTable)ViewState["dtfam"];
            DataRow dr = dt.Rows[dgEmpFam.Rows[e.RowIndex].DataItemIndex];
            dt.Rows.Remove(dr);
            if (dt.Rows.Count > 1)
            {
                dgEmpFam.DataSource = dt;
                dgEmpFam.EditIndex = -1;
                dgEmpFam.ShowFooter = false;
                dgEmpFam.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["rel_name"].ToString() != "")
            {
                dgEmpFam.DataSource = dt;
                dgEmpFam.EditIndex = -1;
                dgEmpFam.ShowFooter = false;
                dgEmpFam.DataBind();
            }
            else
            {
                getEmptyFam();
            }
        }
        //updatepanelEmpNo.Update();
        //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is deleted provisionally, to delete from database permanently please click on Save Link!!');", true);
    }
    protected void dgEmpFam_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (ViewState["dtfam"] != null)
        {
            FamSt = "O";
            DataTable dt = (DataTable)ViewState["dtfam"];
            string name = ((Label)dgEmpFam.Rows[e.NewEditIndex].FindControl("lblRelName")).Text;
            string rel = ((Label)dgEmpFam.Rows[e.NewEditIndex].FindControl("lblRelation")).Text;
            string dob = ((Label)dgEmpFam.Rows[e.NewEditIndex].FindControl("lblBirthDt")).Text;
            string age = ((Label)dgEmpFam.Rows[e.NewEditIndex].FindControl("lblAge")).Text;
            string occu = ((Label)dgEmpFam.Rows[e.NewEditIndex].FindControl("lblOccupation")).Text;
            dgEmpFam.DataSource = dt;
            dgEmpFam.EditIndex = e.NewEditIndex;
            dgEmpFam.DataBind();
            ((TextBox)dgEmpFam.Rows[e.NewEditIndex].FindControl("txtRelName")).Text = name;
            ((DropDownList)dgEmpFam.Rows[e.NewEditIndex].FindControl("ddlRelation")).SelectedItem.Text = rel;
            ((TextBox)dgEmpFam.Rows[e.NewEditIndex].FindControl("txtBirthDt")).Text = dob;
            ((TextBox)dgEmpFam.Rows[e.NewEditIndex].FindControl("txtAge")).Text = age;
            ((TextBox)dgEmpFam.Rows[e.NewEditIndex].FindControl("txtOccupation")).Text = occu;
            ((TextBox)dgEmpFam.Rows[e.NewEditIndex].FindControl("txtBirthDt")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpFam.Rows[e.NewEditIndex].FindControl("txtBirthDt")).ClientID + "')");
        }
    }
    protected void dgEmpFam_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (ViewState["dtfam"] != null)
        {
            FamSt = "O";
            GridViewRow gvr = dgEmpFam.Rows[e.RowIndex];
            DataTable dt = (DataTable)ViewState["dtfam"];
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            dr["rel_name"] = ((TextBox)gvr.FindControl("txtRelName")).Text;
            dr["relation"] = ((DropDownList)gvr.FindControl("ddlRelation")).SelectedItem.Text;
            dr["birth_dt"] = ((TextBox)gvr.FindControl("txtBirthDt")).Text;
            dr["age"] = ((TextBox)gvr.FindControl("txtAge")).Text;
            dr["occupation"] = ((TextBox)gvr.FindControl("txtOccupation")).Text;
            dgEmpFam.DataSource = dt;
            dgEmpFam.EditIndex = -1;
            dgEmpFam.DataBind();
            //updatepanelEmpNo.Update();
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is updated provisionally, to update in database permanently please click on Save Link!!');", true);
        }
    }

    protected void dgEmpFam_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((DataRowView) e.Row.DataItem)["rel_name"].ToString() == "")
            {
                e.Row.Visible = false;
            }
            e.Row.Cells[3].Attributes.Add("style", "display:none");
        }
        if (e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[3].Attributes.Add("style", "display:none");
        }

    }

    protected void dgEmpFam_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (ViewState["dtfam"] != null)
        {
            FamSt = "O";
            DataTable dt = (DataTable)ViewState["dtfam"];
            if (e.CommandName.Equals("New"))
            {
                dgEmpFam.DataSource = dt;
                dgEmpFam.ShowFooter = true;
                dgEmpFam.FooterRow.Visible = true;
                dgEmpFam.DataBind();
                ((TextBox)dgEmpFam.FooterRow.FindControl("txtBirthDt")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpFam.FooterRow.FindControl("txtBirthDt")).ClientID + "')");
            }
            else if (e.CommandName.Equals("Insert"))
            {
                DataRow dr = dt.NewRow();
                dr["rel_name"] = ((TextBox)dgEmpFam.FooterRow.FindControl("txtRelName")).Text;
                dr["relation"] = ((DropDownList)dgEmpFam.FooterRow.FindControl("ddlRelation")).SelectedItem.Text;
                dr["birth_dt"] = ((TextBox)dgEmpFam.FooterRow.FindControl("txtBirthDt")).Text;
                dr["age"] = ((TextBox)dgEmpFam.FooterRow.FindControl("txtAge")).Text;
                dr["occupation"] = ((TextBox)dgEmpFam.FooterRow.FindControl("txtOccupation")).Text;
                dt.Rows.Add(dr);
                dgEmpFam.DataSource = dt;
                dgEmpFam.ShowFooter = false;
                dgEmpFam.DataBind();
                //updatepanelEmpNo.Update();
                //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is inserted provisionally, to save in database permanently please click on Save Link!!');", true);
            }
        }
    }

    protected void dgEmpExp_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        if (ViewState["dtexp"] != null)
        {
            ExpSt = "O";
            DataTable dt = (DataTable)ViewState["dtexp"];
            if (dt.Rows.Count > 1)
            {
                dgEmpExp.DataSource = dt;
                dgEmpExp.EditIndex = -1;
                dgEmpExp.ShowFooter = false;
                dgEmpExp.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["orga_name"].ToString() != "")
            {
                dgEmpExp.DataSource = dt;
                dgEmpExp.EditIndex = -1;
                dgEmpExp.ShowFooter = false;
                dgEmpExp.DataBind();
            }
            else
            {
                getEmptyExp();
            }
        }
    }
    protected void dgEmpExp_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["dtexp"] != null)
        {
            ExpSt = "O";
            DataTable dt = (DataTable)ViewState["dtexp"];
            DataRow dr = dt.Rows[dgEmpExp.Rows[e.RowIndex].DataItemIndex];
            dt.Rows.Remove(dr);
            if (dt.Rows.Count > 1)
            {
                dgEmpExp.DataSource = dt;
                dgEmpExp.EditIndex = -1;
                dgEmpExp.ShowFooter = false;
                dgEmpExp.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["orga_name"].ToString() != "")
            {
                dgEmpExp.DataSource = dt;
                dgEmpExp.EditIndex = -1;
                dgEmpExp.ShowFooter = false;
                dgEmpExp.DataBind();
            }
            else
            {
                getEmptyExp();
            }
        }
        //updatepanelEmpNo.Update();
        //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is deleted provisionally, to delete from database permanently please click on Save Link!!');", true);
    }
    protected void dgEmpExp_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (ViewState["dtexp"] != null)
        {
            ExpSt = "O";
            DataTable dt = (DataTable)ViewState["dtexp"];
            string name = ((Label)dgEmpExp.Rows[e.NewEditIndex].FindControl("lblOrgaName")).Text;
            string posi = ((Label)dgEmpExp.Rows[e.NewEditIndex].FindControl("lblPositionHeld")).Text;
            string from = ((Label)dgEmpExp.Rows[e.NewEditIndex].FindControl("lblFromDt")).Text;
            string to = ((Label)dgEmpExp.Rows[e.NewEditIndex].FindControl("lblToDt")).Text;
            string pay = ((Label)dgEmpExp.Rows[e.NewEditIndex].FindControl("lblPayScale")).Text;
            dgEmpExp.ShowFooter = false;
            dgEmpExp.DataSource = dt;
            dgEmpExp.EditIndex = e.NewEditIndex;
            dgEmpExp.DataBind();
            ((TextBox)dgEmpExp.Rows[e.NewEditIndex].FindControl("txtOrgaName")).Text = name;
            ((TextBox)dgEmpExp.Rows[e.NewEditIndex].FindControl("txtPositionHeld")).Text = posi;
            ((TextBox)dgEmpExp.Rows[e.NewEditIndex].FindControl("txtFromDt")).Text = from;
            ((TextBox)dgEmpExp.Rows[e.NewEditIndex].FindControl("txtToDt")).Text = to;
            ((TextBox)dgEmpExp.Rows[e.NewEditIndex].FindControl("txtPayScale")).Text = pay;
            ((TextBox)dgEmpExp.Rows[e.NewEditIndex].FindControl("txtFromDt")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpExp.Rows[e.NewEditIndex].FindControl("txtFromDt")).ClientID + "')");
            ((TextBox)dgEmpExp.Rows[e.NewEditIndex].FindControl("txtToDt")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpExp.Rows[e.NewEditIndex].FindControl("txtToDt")).ClientID + "')");
        }
    }
    protected void dgEmpExp_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (ViewState["dtexp"] != null)
        {
            ExpSt = "O";
            DataTable dt = (DataTable)ViewState["dtexp"];
            GridViewRow gvr = dgEmpExp.Rows[e.RowIndex];
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            dr["orga_name"] = ((TextBox)gvr.FindControl("txtOrgaName")).Text;
            dr["position_held"] = ((TextBox)gvr.FindControl("txtPositionHeld")).Text;
            dr["from_dt"] = ((TextBox)gvr.FindControl("txtFromDt")).Text;
            dr["to_dt"] = ((TextBox)gvr.FindControl("txtToDt")).Text;
            dr["pay_scale"] = ((TextBox)gvr.FindControl("txtPayScale")).Text;
            dgEmpExp.DataSource = dt;
            dgEmpExp.EditIndex = -1;
            dgEmpExp.ShowFooter = false;
            dgEmpExp.DataBind();
            //updatepanelEmpNo.Update();
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is updated provisionally, to update in database permanently please click on Save Link!!');", true);
        }
    }
    protected void dgEmpExp_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((DataRowView)e.Row.DataItem)["orga_name"].ToString() == "")
            {
                e.Row.Visible = false;
            }
        }
        if (e.Row.RowType == DataControlRowType.Footer | e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[5].Attributes.Add("style", "display:none");
        }
    }
    protected void dgEmpExp_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (ViewState["dtexp"] != null)
        {
            ExpSt = "O";
            DataTable dt = (DataTable)ViewState["dtexp"];
            if (e.CommandName.Equals("AddNew"))
            {
                dgEmpExp.DataSource = dt;
                dgEmpExp.DataBind();
                dgEmpExp.ShowFooter = true;
                dgEmpExp.FooterRow.Visible = true;
                ((TextBox)dgEmpExp.FooterRow.FindControl("txtFromDt")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpExp.FooterRow.FindControl("txtFromDt")).ClientID + "')");
                ((TextBox)dgEmpExp.FooterRow.FindControl("txtToDt")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpExp.FooterRow.FindControl("txtToDt")).ClientID + "')");
            }
            else if (e.CommandName.Equals("Insert"))
            {
                DataRow dr = dt.NewRow();
                dr["orga_name"] = ((TextBox)dgEmpExp.FooterRow.FindControl("txtOrgaName")).Text;
                dr["position_held"] = ((TextBox)dgEmpExp.FooterRow.FindControl("txtPositionHeld")).Text;
                dr["from_dt"] = ((TextBox)dgEmpExp.FooterRow.FindControl("txtFromDt")).Text;
                dr["to_dt"] = ((TextBox)dgEmpExp.FooterRow.FindControl("txtToDt")).Text;
                dr["pay_scale"] = ((TextBox)dgEmpExp.FooterRow.FindControl("txtPayScale")).Text;
                dt.Rows.Add(dr);
                dgEmpExp.DataSource = dt;
                dgEmpExp.ShowFooter = false;
                dgEmpExp.DataBind();
                //updatepanelEmpNo.Update();
                //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is inserted provisionally, to insert in database permanently please click on Save Link!!');", true);
            }
        }
    }

    protected void dgEmpTrain_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        if (ViewState["dttrain"] != null)
        {
            TrainSt = "O";
            DataTable dt = (DataTable)ViewState["dttrain"];
            if (dt.Rows.Count > 1)
            {
                dgEmpTrain.DataSource = dt;
                dgEmpTrain.EditIndex = -1;
                dgEmpTrain.ShowFooter = false;
                dgEmpTrain.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["train_title"].ToString() != "")
            {
                dgEmpTrain.DataSource = dt;
                dgEmpTrain.EditIndex = -1;
                dgEmpTrain.ShowFooter = false;
                dgEmpTrain.DataBind();
            }
            else
            {
                getEmptyTrain();
            }
        }
    }
    protected void dgEmpTrain_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["dttrain"] != null)
        {
            TrainSt = "O";
            DataTable dt = (DataTable)ViewState["dttrain"];
            DataRow dr = dt.Rows[dgEmpTrain.Rows[e.RowIndex].DataItemIndex];
            dt.Rows.Remove(dr);
            if (dt.Rows.Count > 1)
            {
                dgEmpTrain.DataSource = dt;
                dgEmpTrain.EditIndex = -1;
                dgEmpTrain.ShowFooter = false;
                dgEmpTrain.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["train_title"].ToString() != "")
            {
                dgEmpTrain.DataSource = dt;
                dgEmpTrain.EditIndex = -1;
                dgEmpTrain.ShowFooter = false;
                dgEmpTrain.DataBind();
            }
            else
            {
                getEmptyTrain();
            }
        }
        //updatepanelEmpNo.Update();
        //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is deleted provisionally, to delete from database permanently please click on Save Link!!');", true);
    }
    protected void dgEmpTrain_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (ViewState["dttrain"] != null)
        {
            TrainSt = "O";
            DataTable dt = (DataTable)ViewState["dttrain"];
            string name = ((Label)dgEmpTrain.Rows[e.NewEditIndex].FindControl("lblTrainTitle")).Text;
            string yr = ((Label)dgEmpTrain.Rows[e.NewEditIndex].FindControl("lblYear")).Text;
            string plac = ((Label)dgEmpTrain.Rows[e.NewEditIndex].FindControl("lblPlace")).Text;
            string cont = ((Label)dgEmpTrain.Rows[e.NewEditIndex].FindControl("lblCountry")).Text;
            string fin = ((Label)dgEmpTrain.Rows[e.NewEditIndex].FindControl("lblFinan")).Text;
            string amnt = ((Label)dgEmpTrain.Rows[e.NewEditIndex].FindControl("lblAmount")).Text;
            string dyr = ((Label)dgEmpTrain.Rows[e.NewEditIndex].FindControl("lblDuYear")).Text;
            string dmon = ((Label)dgEmpTrain.Rows[e.NewEditIndex].FindControl("lblDuMonth")).Text;
            string dday = ((Label)dgEmpTrain.Rows[e.NewEditIndex].FindControl("lblDuDay")).Text;
            dgEmpTrain.ShowFooter = false;
            dgEmpTrain.DataSource = dt;
            dgEmpTrain.EditIndex = e.NewEditIndex;
            dgEmpTrain.DataBind();
            ((TextBox)dgEmpTrain.Rows[e.NewEditIndex].FindControl("txtTrainTitle")).Text = name;
            ((TextBox)dgEmpTrain.Rows[e.NewEditIndex].FindControl("txtYear")).Text = yr;
            ((TextBox)dgEmpTrain.Rows[e.NewEditIndex].FindControl("txtPlace")).Text = plac;
            ((TextBox)dgEmpTrain.Rows[e.NewEditIndex].FindControl("txtCountry")).Text = cont;
            ((TextBox)dgEmpTrain.Rows[e.NewEditIndex].FindControl("txtFinan")).Text = fin;
            ((TextBox)dgEmpTrain.Rows[e.NewEditIndex].FindControl("txtAmount")).Text = amnt;
            ((TextBox)dgEmpTrain.Rows[e.NewEditIndex].FindControl("txtDuYear")).Text = dyr;
            ((TextBox)dgEmpTrain.Rows[e.NewEditIndex].FindControl("txtDuMonth")).Text = dmon;
            ((TextBox)dgEmpTrain.Rows[e.NewEditIndex].FindControl("txtDuDay")).Text = dday;
        }
    }
    protected void dgEmpTrain_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (ViewState["dttrain"] != null)
        {
            TrainSt = "O";
            GridViewRow gvr = dgEmpTrain.Rows[e.RowIndex];
            DataTable dt = (DataTable)ViewState["dttrain"];
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            dr["train_title"] = ((TextBox)gvr.FindControl("txtTrainTitle")).Text;
            dr["year"] = ((TextBox)gvr.FindControl("txtYear")).Text;
            dr["place"] = ((TextBox)gvr.FindControl("txtPlace")).Text;
            dr["country"] = ((TextBox)gvr.FindControl("txtCountry")).Text;
            dr["finan"] = ((TextBox)gvr.FindControl("txtFinan")).Text;
            dr["amount"] = ((TextBox)gvr.FindControl("txtAmount")).Text;
            dr["du_year"] = ((TextBox)gvr.FindControl("txtDuYear")).Text;
            dr["du_month"] = ((TextBox)gvr.FindControl("txtDuMonth")).Text;
            dr["du_day"] = ((TextBox)gvr.FindControl("txtDuDay")).Text;
            dgEmpTrain.DataSource = dt;
            dgEmpTrain.EditIndex = -1;
            dgEmpTrain.DataBind();
            //updatepanelEmpNo.Update();
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is updated provisionally, to update in database permanently please click on Save Link!!');", true);
        }
    }
    protected void dgEmpTrain_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((DataRowView)e.Row.DataItem)["train_title"].ToString() == String.Empty)
            {
                e.Row.Visible = false;
            }
        }
    }
    protected void dgEmpTrain_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (ViewState["dttrain"] != null)
        {
            TrainSt = "O";
            DataTable dt = (DataTable)ViewState["dttrain"];
            if (e.CommandName.Equals("AddNew"))
            {
                dgEmpTrain.ShowFooter = true;
                dgEmpTrain.DataSource = dt;
                dgEmpTrain.DataBind();
            }
            else if (e.CommandName.Equals("Insert"))
            {
                DataRow dr = dt.NewRow();
                dr["train_title"] = ((TextBox)dgEmpTrain.FooterRow.FindControl("txtTrainTitle")).Text;
                dr["year"] = ((TextBox)dgEmpTrain.FooterRow.FindControl("txtYear")).Text;
                dr["place"] = ((TextBox)dgEmpTrain.FooterRow.FindControl("txtPlace")).Text;
                dr["country"] = ((TextBox)dgEmpTrain.FooterRow.FindControl("txtCountry")).Text;
                dr["finan"] = ((TextBox)dgEmpTrain.FooterRow.FindControl("txtFinan")).Text;
                dr["amount"] = ((TextBox)dgEmpTrain.FooterRow.FindControl("txtAmount")).Text;
                dr["du_year"] = ((TextBox)dgEmpTrain.FooterRow.FindControl("txtDuYear")).Text;
                dr["du_month"] = ((TextBox)dgEmpTrain.FooterRow.FindControl("txtDuMonth")).Text;
                dr["du_day"] = ((TextBox)dgEmpTrain.FooterRow.FindControl("txtDuDay")).Text;
                dt.Rows.Add(dr);
                dgEmpTrain.DataSource = dt;
                dgEmpTrain.ShowFooter = false;
                dgEmpTrain.DataBind();
                //updatepanelEmpNo.Update();
                //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is inserted provisionally, to insert in database permanently please click on Save Link!!');", true);
            }
        }
    }

    protected void dgEmpTrans_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        if (ViewState["dttrans"] != null)
        {
            TransSt = "O";
            DataTable dt = (DataTable)ViewState["dttrans"];
            if (dt.Rows.Count > 1)
            {
                dgEmpTrans.DataSource = dt;
                dgEmpTrans.EditIndex = -1;
                dgEmpTrans.ShowFooter = false;
                dgEmpTrans.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["order_no"].ToString() != "")
            {
                dgEmpTrans.DataSource = dt;
                dgEmpTrans.EditIndex = -1;
                dgEmpTrans.ShowFooter = false;
                dgEmpTrans.DataBind();
            }
            else
            {
                getEmptyTrans();
            }
        }
    }
    protected void dgEmpTrans_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["dttrans"] != null)
        {
            TransSt = "O";
            DataTable dt = (DataTable)ViewState["dttrans"];
            DataRow dr = dt.Rows[dgEmpTrans.Rows[e.RowIndex].DataItemIndex];
            dt.Rows.Remove(dr);
            if (dt.Rows.Count > 1)
            {
                dgEmpTrans.DataSource = dt;
                dgEmpTrans.EditIndex = -1;
                dgEmpTrans.ShowFooter = false;
                dgEmpTrans.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["order_no"].ToString() != "")
            {
                dgEmpTrans.DataSource = dt;
                dgEmpTrans.EditIndex = -1;
                dgEmpTrans.ShowFooter = false;
                dgEmpTrans.DataBind();
            }
            else
            {
                getEmptyTrans();
            }
        }
        //updatepanelEmpNo.Update();
        //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is deleted provisionally, to delete from database permanently please click on Save Link!!');", true);    
    }
    protected void dgEmpTrans_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (ViewState["dttrans"] != null)
        {
            DataTable dt = (DataTable)ViewState["dttrans"];
            TransSt = "O";
            string ord = ((Label)dgEmpTrans.Rows[e.NewEditIndex].FindControl("lblOrderNo")).Text;
            string dat = ((Label)dgEmpTrans.Rows[e.NewEditIndex].FindControl("lblTransDate")).Text;
            string prom = ((Label)dgEmpTrans.Rows[e.NewEditIndex].FindControl("lblTransProm")).Text;
            string branch = ((Label)dgEmpTrans.Rows[e.NewEditIndex].FindControl("lblBranchKey")).Text;
            string desig = ((Label)dgEmpTrans.Rows[e.NewEditIndex].FindControl("lblDesigCode")).Text;
            dgEmpTrans.DataSource = dt;
            dgEmpTrans.EditIndex = e.NewEditIndex;
            dgEmpTrans.DataBind();
            ((DropDownList)dgEmpTrans.Rows[e.NewEditIndex].FindControl("ddlBranchKey")).Items.Clear();
            string queryBranch = "select '' BranchKey, '' BranchName union select BranchKey,dbo.initcap(BranchName) BranchName from BranchInfo order by 2 desc";
            util.PopulationDropDownList((DropDownList)dgEmpTrans.Rows[e.NewEditIndex].FindControl("ddlBranchKey"), "Branch", queryBranch, "BranchName", "BranchKey");
            ((DropDownList)dgEmpTrans.Rows[e.NewEditIndex].FindControl("ddlDesigCode")).Items.Clear();
            string queryDesig = "select '' desig_code, '' desig_name union select desig_code,dbo.initcap(desig_name) desig_name from pmis_desig_code order by 2 desc";
            util.PopulationDropDownList((DropDownList)dgEmpTrans.Rows[e.NewEditIndex].FindControl("ddlDesigCode"), "Designation", queryDesig, "desig_name", "desig_code");
            ((TextBox)dgEmpTrans.Rows[e.NewEditIndex].FindControl("txtOrderNo")).Text = ord;
            ((TextBox)dgEmpTrans.Rows[e.NewEditIndex].FindControl("txtTransDate")).Text = dat;
            ((DropDownList)dgEmpTrans.Rows[e.NewEditIndex].FindControl("ddlTransProm")).SelectedValue = prom;
            ((DropDownList)dgEmpTrans.Rows[e.NewEditIndex].FindControl("ddlBranchKey")).SelectedValue = branch;
            ((DropDownList)dgEmpTrans.Rows[e.NewEditIndex].FindControl("ddlDesigCode")).SelectedValue = desig;
            ((TextBox)dgEmpTrans.Rows[e.NewEditIndex].FindControl("txtTransDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpTrans.Rows[e.NewEditIndex].FindControl("txtTransDate")).ClientID + "')");
        }
    }
    protected void dgEmpTrans_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (ViewState["dttrans"] != null)
        {
            TransSt = "O";
            GridViewRow gvr = dgEmpTrans.Rows[e.RowIndex];
            DataTable dt = (DataTable)ViewState["dttrans"];
            DataRow dr = dt.Rows[gvr.RowIndex];
            dr["order_no"] = ((TextBox)gvr.FindControl("txtOrderNo")).Text;
            dr["trans_date"] = ((TextBox)gvr.FindControl("txtTransDate")).Text;
            dr["trans_prom"] = ((DropDownList)gvr.FindControl("ddlTransProm")).SelectedValue;
            dr["BRANCH_CODE"] = ((DropDownList)gvr.FindControl("ddlBranchKey")).SelectedValue;
            dr["desig_code"] = ((DropDownList)gvr.FindControl("ddlDesigCode")).SelectedValue;
            dgEmpTrans.DataSource = dt;
            dgEmpTrans.EditIndex = -1;
            dgEmpTrans.DataBind();
            //updatepanelEmpNo.Update();
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is updated provisionally, to update in database permanently please click on Save Link!!');", true);
        }
    }
    protected void dgEmpTrans_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((DataRowView)e.Row.DataItem)["order_no"].ToString() == "")
            {
                e.Row.Visible = false;
            }
        }
    }
    protected void dgEmpTrans_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (ViewState["dttrans"] != null)
        {
            TransSt = "O";
            DataTable dt = (DataTable)ViewState["dttrans"];
            if (e.CommandName.Equals("AddNew"))
            {
                dgEmpTrans.ShowFooter = true;
                dgEmpTrans.DataSource = dt;
                dgEmpTrans.DataBind();
                ((DropDownList)dgEmpTrans.FooterRow.FindControl("ddlBranchKey")).Items.Clear();
                string queryBranch = "select '' BranchKey, '' BranchName union select BranchKey,dbo.initcap(BranchName) BranchName from BranchInfo order by 2 desc";
                util.PopulationDropDownList((DropDownList)dgEmpTrans.FooterRow.FindControl("ddlBranchKey"), "Branch", queryBranch, "BranchName", "BranchKey");

                ((DropDownList)dgEmpTrans.FooterRow.FindControl("ddlDesigCode")).Items.Clear();
                string queryDesig = "select '' desig_code, '' desig_name union select desig_code,dbo.initcap(desig_name) desig_name from pmis_desig_code order by 2 desc";
                util.PopulationDropDownList((DropDownList)dgEmpTrans.FooterRow.FindControl("ddlDesigCode"), "Designation", queryDesig, "desig_name", "desig_code");
                ((TextBox)dgEmpTrans.FooterRow.FindControl("txtTransDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpTrans.FooterRow.FindControl("txtTransDate")).ClientID + "')");
            }
            else if (e.CommandName.Equals("Insert"))
            {
                DataRow dr = dt.NewRow();
                dr["order_no"] = ((TextBox)dgEmpTrans.FooterRow.FindControl("txtOrderNo")).Text;
                dr["trans_date"] = ((TextBox)dgEmpTrans.FooterRow.FindControl("txtTransDate")).Text;
                dr["trans_prom"] = ((DropDownList)dgEmpTrans.FooterRow.FindControl("ddlTransProm")).SelectedValue;
                dr["BRANCH_CODE"] = ((DropDownList)dgEmpTrans.FooterRow.FindControl("ddlBranchKey")).SelectedValue;
                dr["desig_code"] = ((DropDownList)dgEmpTrans.FooterRow.FindControl("ddlDesigCode")).SelectedValue;
                dt.Rows.Add(dr);
                dgEmpTrans.DataSource = dt;
                dgEmpTrans.ShowFooter = false;
                dgEmpTrans.DataBind();
                //updatepanelEmpNo.Update();
                //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is updated provisionally, to update in database permanently please click on Save Link!!');", true);
            }
        }
    }

    protected void dgEmpProm_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        if (ViewState["dtprom"] != null)
        {
            PromSt = "O";
            DataTable dt = (DataTable)ViewState["dtprom"];
            if (dt.Rows.Count > 1)
            {
                dgEmpProm.DataSource = dt;
                dgEmpProm.EditIndex = -1;
                dgEmpProm.ShowFooter = false;
                dgEmpProm.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["off_ord_no"].ToString() != "")
            {
                dgEmpProm.DataSource = dt;
                dgEmpProm.EditIndex = -1;
                dgEmpProm.ShowFooter = false;
                dgEmpProm.DataBind();
            }
            else
            {
                getEmptyProm();
            }
        }
    }
    protected void dgEmpProm_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["dtprom"] != null)
        {
            PromSt = "O";
            DataTable dt = (DataTable)ViewState["dtprom"];
            DataRow dr = dt.Rows[dgEmpProm.Rows[e.RowIndex].DataItemIndex];
            dt.Rows.Remove(dr);
            if (dt.Rows.Count > 1)
            {
                dgEmpProm.DataSource = dt;
                dgEmpProm.EditIndex = -1;
                dgEmpProm.ShowFooter = false;
                dgEmpProm.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["off_ord_no"].ToString() != "")
            {
                dgEmpProm.DataSource = dt;
                dgEmpProm.EditIndex = -1;
                dgEmpProm.ShowFooter = false;
                dgEmpProm.DataBind();
            }
            else
            {
                getEmptyProm();
            }
        }
        //updatepanelEmpNo.Update();
        //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is deleted provisionally, to delete from database permanently please click on Save Link!!');", true);
    }
    protected void dgEmpProm_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (ViewState["dtprom"] != null)
        {
            PromSt = "O";
            DataTable dt = (DataTable)ViewState["dtprom"];
            string ord = ((Label)dgEmpProm.Rows[e.NewEditIndex].FindControl("lblOffOrdNo")).Text;
            string dat = ((Label)dgEmpProm.Rows[e.NewEditIndex].FindControl("lblJoiningDate")).Text;
            string branch = ((Label)dgEmpProm.Rows[e.NewEditIndex].FindControl("lblJoiningBranch")).Text;
            string desig = ((Label)dgEmpProm.Rows[e.NewEditIndex].FindControl("lblJoiningDesig")).Text;
            string basic = ((Label)dgEmpProm.Rows[e.NewEditIndex].FindControl("lblBasicPay")).Text;
            string scal = ((Label)dgEmpProm.Rows[e.NewEditIndex].FindControl("lblPayScale")).Text;
            dgEmpProm.DataSource = dt;
            dgEmpProm.EditIndex = e.NewEditIndex;
            dgEmpProm.DataBind();
            ((DropDownList)dgEmpProm.Rows[e.NewEditIndex].FindControl("ddlJoiningBranch")).Items.Clear();
            string queryBranch = "select '' BranchKey, '' BranchName union select BranchKey,dbo.initcap(BranchName) BranchName from BranchInfo order by 2 desc";
            util.PopulationDropDownList((DropDownList)dgEmpProm.Rows[e.NewEditIndex].FindControl("ddlJoiningBranch"), "Branch", queryBranch, "BranchName", "BranchKey");
            ((DropDownList)dgEmpProm.Rows[e.NewEditIndex].FindControl("ddlJoiningDesig")).Items.Clear();
            string queryDesig = "select '' desig_code, '' desig_name union select desig_code,dbo.initcap(desig_name) desig_name from pmis_desig_code order by 2 desc";
            util.PopulationDropDownList((DropDownList)dgEmpProm.Rows[e.NewEditIndex].FindControl("ddlJoiningDesig"), "Designation", queryDesig, "desig_name", "desig_code");
            ((TextBox)dgEmpProm.Rows[e.NewEditIndex].FindControl("txtOffOrdNo")).Text = ord;
            ((TextBox)dgEmpProm.Rows[e.NewEditIndex].FindControl("txtJoiningDate")).Text = dat;
            ((DropDownList)dgEmpProm.Rows[e.NewEditIndex].FindControl("ddlJoiningBranch")).SelectedValue = branch;
            ((DropDownList)dgEmpProm.Rows[e.NewEditIndex].FindControl("ddlJoiningDesig")).SelectedValue = desig;
            ((TextBox)dgEmpProm.Rows[e.NewEditIndex].FindControl("txtBasicPay")).Text = basic;
            ((TextBox)dgEmpProm.Rows[e.NewEditIndex].FindControl("txtPayScale")).Text = scal;
            ((TextBox)dgEmpProm.Rows[e.NewEditIndex].FindControl("txtJoiningDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpProm.Rows[e.NewEditIndex].FindControl("txtJoiningDate")).ClientID + "')");
        }
    }
    protected void dgEmpProm_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (ViewState["dtprom"] != null)
        {
            PromSt = "O";
            GridViewRow gvr = dgEmpProm.Rows[e.RowIndex];
            DataTable dt = (DataTable)ViewState["dtprom"];
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            dr["off_ord_no"] = ((TextBox)gvr.FindControl("txtOffOrdNo")).Text;
            dr["joining_date"] = ((TextBox)gvr.FindControl("txtJoiningDate")).Text;
            dr["joining_branch"] = ((DropDownList)gvr.FindControl("ddlJoiningBranch")).SelectedValue;
            dr["joining_desig"] = ((DropDownList)gvr.FindControl("ddlJoiningDesig")).SelectedValue;
            dr["basic_pay"] = ((TextBox)gvr.FindControl("txtBasicPay")).Text;
            dr["pay_scale"] = ((TextBox)gvr.FindControl("txtPayScale")).Text;
            dgEmpProm.DataSource = dt;
            dgEmpProm.EditIndex = -1;
            dgEmpProm.DataBind();
            //updatepanelEmpNo.Update();
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is updated provisionally, to update in database permanently please click on Save Link!!');", true);
        }
    }
    protected void dgEmpProm_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((DataRowView)e.Row.DataItem)["off_ord_no"].ToString() == String.Empty)
            {
                e.Row.Visible = false;
            }
            e.Row.Cells[6].Attributes.Add("style", "display:none");
            e.Row.Cells[5].Attributes.Add("style", "display:none");
        }
        else if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[6].Attributes.Add("style", "display:none");
            e.Row.Cells[5].Attributes.Add("style", "display:none");
        }
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[6].Attributes.Add("style", "display:none");
            e.Row.Cells[5].Attributes.Add("style", "display:none");
        }
    }
    protected void dgEmpProm_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (ViewState["dtprom"] != null)
        {
            PromSt = "O";
            DataTable dt = (DataTable)ViewState["dtprom"];
            if (e.CommandName.Equals("AddNew"))
            {
                dgEmpProm.ShowFooter = true;
                dgEmpProm.FooterRow.Visible = true;
                dgEmpProm.DataSource = dt;
                dgEmpProm.DataBind();
                ((DropDownList)dgEmpProm.FooterRow.FindControl("ddlJoiningBranch")).Items.Clear();
                string queryBranch = "select '' BranchKey, '' BranchName union select BranchKey,dbo.initcap(BranchName) BranchName from BranchInfo order by 2 desc";
                util.PopulationDropDownList((DropDownList)dgEmpProm.FooterRow.FindControl("ddlJoiningBranch"), "Branch", queryBranch, "BranchName", "BranchKey");
                ((DropDownList)dgEmpProm.FooterRow.FindControl("ddlJoiningDesig")).Items.Clear();
                string queryDesig = "select '' desig_code, '' desig_name union select desig_code,dbo.initcap(desig_name) desig_name from pmis_desig_code order by 2 desc";
                util.PopulationDropDownList((DropDownList)dgEmpProm.FooterRow.FindControl("ddlJoiningDesig"), "Designation", queryDesig, "desig_name", "desig_code");
                ((TextBox)dgEmpProm.FooterRow.FindControl("txtJoiningDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpProm.FooterRow.FindControl("txtJoiningDate")).ClientID + "')");
            }
            else if (e.CommandName.Equals("Insert"))
            {
                DataRow dr = dt.NewRow();
                dr["off_ord_no"] = ((TextBox)dgEmpProm.FooterRow.FindControl("txtOffOrdNo")).Text;
                dr["joining_date"] = ((TextBox)dgEmpProm.FooterRow.FindControl("txtJoiningDate")).Text;
                dr["joining_branch"] = ((DropDownList)dgEmpProm.FooterRow.FindControl("ddlJoiningBranch")).SelectedValue;
                dr["joining_desig"] = ((DropDownList)dgEmpProm.FooterRow.FindControl("ddlJoiningDesig")).SelectedValue;
                dr["basic_pay"] = ((TextBox)dgEmpProm.FooterRow.FindControl("txtBasicPay")).Text;
                dr["pay_scale"] = ((TextBox)dgEmpProm.FooterRow.FindControl("txtPayScale")).Text;
                dt.Rows.Add(dr);
                dgEmpProm.DataSource = dt;
                dgEmpProm.ShowFooter = false;
                dgEmpProm.DataBind();
                //updatepanelEmpNo.Update();
                //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is inserted provisionally, to insert in database permanently please click on Save Link!!');", true);
            }
        }
    }

    protected void dgEmpQtr_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        if (ViewState["dtqtr"] != null)
        {
            QtrSt = "O";
            DataTable dt = (DataTable)ViewState["dtqtr"];
            if (dt.Rows.Count > 1)
            {
                dgEmpQtr.DataSource = dt;
                dgEmpQtr.EditIndex = -1;
                dgEmpQtr.ShowFooter = false;
                dgEmpQtr.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["allot_ref"].ToString() != "")
            {
                dgEmpQtr.DataSource = dt;
                dgEmpQtr.EditIndex = -1;
                dgEmpQtr.ShowFooter = false;
                dgEmpQtr.DataBind();
            }
            else
            {
                getEmptyQtr();
            }
        }
    }
    protected void dgEmpQtr_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["dtqtr"] != null)
        {
            QtrSt = "O";
            DataTable dt = (DataTable)ViewState["dtqtr"];
            DataRow dr = dt.Rows[dgEmpQtr.Rows[e.RowIndex].DataItemIndex];
            dt.Rows.Remove(dr);
            if (dt.Rows.Count > 1)
            {
                dgEmpQtr.DataSource = dt;
                dgEmpQtr.EditIndex = -1;
                dgEmpQtr.ShowFooter = false;
                dgEmpQtr.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["allot_ref"].ToString() != "")
            {
                dgEmpQtr.DataSource = dt;
                dgEmpQtr.EditIndex = -1;
                dgEmpQtr.ShowFooter = false;
                dgEmpQtr.DataBind();
            }
            else
            {
                getEmptyQtr();
            }
        }
        //updatepanelEmpNo.Update();
        //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is deleted provisionally, to delete from database permanently please click on Save Link!!');", true);
    }
    protected void dgEmpQtr_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (ViewState["dtqtr"] != null)
        {
            QtrSt = "O";
            DataTable dt = (DataTable)ViewState["dtqtr"];
            string ord = ((Label)dgEmpQtr.Rows[e.NewEditIndex].FindControl("lblAllotRef")).Text;
            string adat = ((Label)dgEmpQtr.Rows[e.NewEditIndex].FindControl("lblRefDate")).Text;
            string pdat = ((Label)dgEmpQtr.Rows[e.NewEditIndex].FindControl("lblPostDate")).Text;
            string locat = ((Label)dgEmpQtr.Rows[e.NewEditIndex].FindControl("lblLocat")).Text;
            string road = ((Label)dgEmpQtr.Rows[e.NewEditIndex].FindControl("lblRoad")).Text;
            string build = ((Label)dgEmpQtr.Rows[e.NewEditIndex].FindControl("lblBuild")).Text;
            string flat = ((Label)dgEmpQtr.Rows[e.NewEditIndex].FindControl("lblFlat")).Text;
            string flattyp = ((Label)dgEmpQtr.Rows[e.NewEditIndex].FindControl("lblFlatTyp")).Text;
            string siz = ((Label)dgEmpQtr.Rows[e.NewEditIndex].FindControl("lblSizee")).Text;
            dgEmpQtr.DataSource = dt;
            dgEmpQtr.EditIndex = e.NewEditIndex;
            dgEmpQtr.DataBind();
            ((TextBox)dgEmpQtr.Rows[e.NewEditIndex].FindControl("txtAllotRef")).Text = ord;
            ((TextBox)dgEmpQtr.Rows[e.NewEditIndex].FindControl("txtRefDate")).Text = adat;
            ((TextBox)dgEmpQtr.Rows[e.NewEditIndex].FindControl("txtPostDate")).Text = pdat;
            ((TextBox)dgEmpQtr.Rows[e.NewEditIndex].FindControl("txtLocat")).Text = locat;
            ((TextBox)dgEmpQtr.Rows[e.NewEditIndex].FindControl("txtRoad")).Text = road;
            ((TextBox)dgEmpQtr.Rows[e.NewEditIndex].FindControl("txtBuild")).Text = build;
            ((TextBox)dgEmpQtr.Rows[e.NewEditIndex].FindControl("txtFlat")).Text = flat;
            ((TextBox)dgEmpQtr.Rows[e.NewEditIndex].FindControl("txtFlatTyp")).Text = flattyp;
            ((TextBox)dgEmpQtr.Rows[e.NewEditIndex].FindControl("txtSizee")).Text = siz;
            ((TextBox)dgEmpQtr.Rows[e.NewEditIndex].FindControl("txtRefDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpQtr.Rows[e.NewEditIndex].FindControl("txtRefDate")).ClientID + "')");
            ((TextBox)dgEmpQtr.Rows[e.NewEditIndex].FindControl("txtPostDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpQtr.Rows[e.NewEditIndex].FindControl("txtPostDate")).ClientID + "')");
        }
    }
    protected void dgEmpQtr_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (ViewState["dtqtr"] != null)
        {
            QtrSt = "O";
            DataTable dt = (DataTable)ViewState["dtqtr"];
            GridViewRow gvr = dgEmpQtr.Rows[e.RowIndex];
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            dr["allot_ref"] = ((TextBox)gvr.FindControl("txtAllotRef")).Text;
            dr["ref_date"] = ((TextBox)gvr.FindControl("txtRefDate")).Text;
            dr["post_date"] = ((TextBox)gvr.FindControl("txtPostDate")).Text;
            dr["locat"] = ((TextBox)gvr.FindControl("txtLocat")).Text;
            dr["road"] = ((TextBox)gvr.FindControl("txtRoad")).Text;
            dr["build"] = ((TextBox)gvr.FindControl("txtBuid")).Text;
            dr["flat"] = ((TextBox)gvr.FindControl("txtFlat")).Text;
            dr["flat_typ"] = ((TextBox)gvr.FindControl("txtFlatTyp")).Text;
            dr["sizee"] = ((TextBox)gvr.FindControl("txtSizee")).Text;
            dgEmpQtr.DataSource = dt;
            dgEmpQtr.EditIndex = -1;
            dgEmpQtr.DataBind();
            //updatepanelEmpNo.Update();
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is updated provisionally, to update in database permanently please click on Save Link!!');", true);
        }
    }
    protected void dgEmpQtr_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((DataRowView)e.Row.DataItem)["allot_ref"].ToString() == String.Empty)
            {
                e.Row.Visible = false;
            }
        }
    }
    protected void dgEmpQtr_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (ViewState["dtqtr"] != null)
        {
            QtrSt = "O";
            DataTable dt = (DataTable)ViewState["dtqtr"];
            if (e.CommandName.Equals("AddNew"))
            {
                dgEmpQtr.ShowFooter = true;
                dgEmpQtr.DataSource = dt;
                dgEmpQtr.DataBind();
                ((TextBox)dgEmpQtr.FooterRow.FindControl("txtRefDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpQtr.FooterRow.FindControl("txtRefDate")).ClientID + "')");
                ((TextBox)dgEmpQtr.FooterRow.FindControl("txtPostDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpQtr.FooterRow.FindControl("txtPostDate")).ClientID + "')");
            }
            else if (e.CommandName.Equals("Insert"))
            {
                DataRow dr = dt.NewRow();
                dr["allot_ref"] = ((TextBox)dgEmpQtr.FooterRow.FindControl("txtAllotRef")).Text;
                dr["ref_date"] = ((TextBox)dgEmpQtr.FooterRow.FindControl("txtRefDate")).Text;
                dr["post_date"] = ((TextBox)dgEmpQtr.FooterRow.FindControl("txtPostDate")).Text;
                dr["locat"] = ((TextBox)dgEmpQtr.FooterRow.FindControl("txtLocat")).Text;
                dr["road"] = ((TextBox)dgEmpQtr.FooterRow.FindControl("txtRoad")).Text;
                dr["build"] = ((TextBox)dgEmpQtr.FooterRow.FindControl("txtBuild")).Text;
                dr["flat"] = ((TextBox)dgEmpQtr.FooterRow.FindControl("txtFlat")).Text;
                dr["flat_typ"] = ((TextBox)dgEmpQtr.FooterRow.FindControl("txtFlatTyp")).Text;
                dr["sizee"] = ((TextBox)dgEmpQtr.FooterRow.FindControl("txtSizee")).Text;
                dt.Rows.Add(dr);
                dgEmpQtr.DataSource = dt;
                dgEmpQtr.ShowFooter = false;
                dgEmpQtr.DataBind();
                //updatepanelEmpNo.Update();
                //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is inserted provisionally, to insert in database permanently please click on Save Link!!');", true);
            }
        }
    }

    protected void dgMember_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        if (ViewState["dtmemb"] != null)
        {
            QtrSt = "O";
            DataTable dt = (DataTable)ViewState["dtmemb"];
            if (dt.Rows.Count > 1)
            {
                dgMember.DataSource = dt;
                dgMember.EditIndex = -1;
                dgMember.ShowFooter = false;
                dgMember.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["asso_id"].ToString() != "")
            {
                dgMember.DataSource = dt;
                dgMember.EditIndex = -1;
                dgMember.ShowFooter = false;
                dgMember.DataBind();
            }
            else
            {
                //getEmptyMemb();
            }
        }
    }
    protected void dgMember_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["dtmemb"] != null)
        {
            MembSt = "O";
            DataTable dt = (DataTable)ViewState["dtmemb"];
            GridViewRow gvr = dgMember.Rows[e.RowIndex];
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            dt.Rows.Remove(dr);
            if (dt.Rows.Count > 1)
            {
                dgMember.DataSource = dt;
                dgMember.EditIndex = -1;
                dgMember.ShowFooter = false;
                dgMember.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["asso_id"].ToString() != "")
            {
                dgMember.DataSource = dt;
                dgMember.EditIndex = -1;
                dgMember.ShowFooter = false;
                dgMember.DataBind();
            }
            else
            {
                //getEmptyMemb();
            }
        }
        //updatepanelEmpNo.Update();
        //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is deleted provisionally, to delete from database permanently please click on Save Link!!');", true);
    }
    protected void dgMember_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (ViewState["dtmemb"] != null)
        {
            MembSt = "O";
            DataTable dt = (DataTable)ViewState["dtmemb"];
            string assoid = ((Label)dgMember.Rows[e.NewEditIndex].FindControl("lblAssoId")).Text;
            string membno = ((Label)dgMember.Rows[e.NewEditIndex].FindControl("lblMemberNo")).Text;
            dgMember.DataSource = dt;
            dgMember.EditIndex = e.NewEditIndex;
            dgMember.DataBind();
            ((DropDownList)dgMember.Rows[e.NewEditIndex].FindControl("ddlAssoId")).Items.Clear();
            string queryAsso = "select '' asso_id, '' asso_name union select convert(varchar,asso_id),dbo.initcap(asso_name)+' ('+asso_abvr+')' asso_name from association_info order by 2 desc";
            util.PopulationDropDownList((DropDownList)dgMember.Rows[e.NewEditIndex].FindControl("ddlAssoId"), "Association", queryAsso, "asso_name", "asso_id");
            ((DropDownList)dgMember.Rows[e.NewEditIndex].FindControl("ddlAssoId")).SelectedValue = assoid;
            ((TextBox)dgMember.Rows[e.NewEditIndex].FindControl("txtMemberNo")).Text = membno;
        }
    }
    protected void dgMember_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (ViewState["dtmemb"] != null)
        {
            MembSt = "O";
            DataTable dt = (DataTable)ViewState["dtmemb"];
            GridViewRow gvr = dgMember.Rows[e.RowIndex];
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            dr["asso_id"] = ((DropDownList)gvr.FindControl("ddlAssoId")).Text;
            dr["member_no"] = ((TextBox)gvr.FindControl("txtMemberNo")).Text;
            dgMember.DataSource = dt;
            dgMember.EditIndex = -1;
            dgMember.DataBind();
            //updatepanelEmpNo.Update();
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is updated provisionally, to update in database permanently please click on Save Link!!');", true);
        }
    }
    protected void dgMember_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((DataRowView)e.Row.DataItem)["asso_id"].ToString() == String.Empty)
            {
                e.Row.Visible = false;
            }
        }
    }
    protected void dgMember_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (ViewState["dtmemb"] != null)
        {
            MembSt = "O";
            DataTable dt = (DataTable)ViewState["dtmemb"];
            if (e.CommandName.Equals("AddNew"))
            {
                dgMember.ShowFooter = true;
                dgMember.DataSource = dt;
                dgMember.DataBind();
                ((DropDownList)dgMember.FooterRow.FindControl("ddlAssoId")).Items.Clear();
                string queryAsso = "select '' asso_id, '' asso_name union select asso_id,dbo.initcap(asso_name) asso_name from association_info order by 2 asc";
                util.PopulationDropDownList((DropDownList)dgMember.FooterRow.FindControl("ddlAssoId"), "Association", queryAsso, "asso_name", "asso_code");
            }
            else if (e.CommandName.Equals("Insert"))
            {
                DataRow dr = dt.NewRow();
                dr["asso_id"] = ((DropDownList)dgMember.FooterRow.FindControl("ddlAssoId")).Text;
                dr["member_no"] = ((TextBox)dgMember.FooterRow.FindControl("txtMemberNo")).Text;
                dt.Rows.Add(dr);
                dgMember.DataSource = dt;
                dgMember.ShowFooter = false;
                dgMember.DataBind();
                //updatepanelEmpNo.Update();
                //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is inserted provisionally, to insert in database permanently please click on Save Link!!');", true);
            }
        }
    }

    protected void dgEmpSusp_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        if (ViewState["dtsusp"] != null)
        {
            SuspSt = "O";
            DataTable dt = (DataTable)ViewState["dtsusp"];
            if (dt.Rows.Count > 1)
            {
                dgEmpSusp.DataSource = dt;
                dgEmpSusp.EditIndex = -1;
                dgEmpSusp.ShowFooter = false;
                dgEmpSusp.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["off_order_no"].ToString() != "")
            {
                dgEmpSusp.DataSource = dt;
                dgEmpSusp.EditIndex = -1;
                dgEmpSusp.ShowFooter = false;
                dgEmpSusp.DataBind();
            }
            else
            {
               // getEmptySusp();
            }
        }
    }
    protected void dgEmpSusp_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        if (ViewState["dtsusp"] != null)
        {
            SuspSt = "O";
            DataTable dt = (DataTable)ViewState["dtsusp"];
            GridViewRow gvr = dgEmpSusp.Rows[e.RowIndex];
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            dt.Rows.Remove(dr);
            if (dt.Rows.Count > 1)
            {
                dgEmpSusp.DataSource = dt;
                dgEmpSusp.EditIndex = -1;
                dgEmpSusp.ShowFooter = false;
                dgEmpSusp.DataBind();
            }
            else if (dt.Rows.Count == 1 && ((DataRow)dt.Rows[0])["off_order_no"].ToString() != "")
            {
                dgEmpSusp.DataSource = dt;
                dgEmpSusp.EditIndex = -1;
                dgEmpSusp.ShowFooter = false;
                dgEmpSusp.DataBind();
            }
            else
            {
               // getEmptySusp();
            }
        }
        //updatepanelEmpNo.Update();
        //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is deleted provisionally, to delete from database permanently please click on Save Link!!');", true);
    }
    protected void dgEmpSusp_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (ViewState["dtsusp"] != null)
        {
            SuspSt = "O";
            DataTable dt = (DataTable)ViewState["dtsusp"];
            string ord = ((Label)dgEmpSusp.Rows[e.NewEditIndex].FindControl("lblOffOrderNo")).Text;
            string sdat = ((Label)dgEmpSusp.Rows[e.NewEditIndex].FindControl("lblSuspenDate")).Text;
            string sclos = ((Label)dgEmpSusp.Rows[e.NewEditIndex].FindControl("lblSuspenClause")).Text;
            string word = ((Label)dgEmpSusp.Rows[e.NewEditIndex].FindControl("lblWithdrawOrderNo")).Text;
            string wdat = ((Label)dgEmpSusp.Rows[e.NewEditIndex].FindControl("lblWithDate")).Text;
            string punis = ((Label)dgEmpSusp.Rows[e.NewEditIndex].FindControl("lblPunishment")).Text;
            dgEmpSusp.DataSource = dt;
            dgEmpSusp.EditIndex = e.NewEditIndex;
            dgEmpSusp.DataBind();
            ((TextBox)dgEmpSusp.Rows[e.NewEditIndex].FindControl("txtOffOrderNo")).Text = ord;
            ((TextBox)dgEmpSusp.Rows[e.NewEditIndex].FindControl("txtSuspenDate")).Text = sdat;
            ((TextBox)dgEmpSusp.Rows[e.NewEditIndex].FindControl("txtSuspenClause")).Text = sclos;
            ((TextBox)dgEmpSusp.Rows[e.NewEditIndex].FindControl("txtWithdrawOrderNo")).Text = word;
            ((TextBox)dgEmpSusp.Rows[e.NewEditIndex].FindControl("txtWithDate")).Text = wdat;
            ((TextBox)dgEmpSusp.Rows[e.NewEditIndex].FindControl("txtPunishment")).Text = punis;
            ((TextBox)dgEmpSusp.Rows[e.NewEditIndex].FindControl("txtSuspenDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpSusp.Rows[e.NewEditIndex].FindControl("txtSuspenDate")).ClientID + "')");
            ((TextBox)dgEmpSusp.Rows[e.NewEditIndex].FindControl("txtWithDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpSusp.Rows[e.NewEditIndex].FindControl("txtWithDate")).ClientID + "')");
        }
    }
    protected void dgEmpSusp_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (ViewState["dtsusp"] != null)
        {
            SuspSt = "O";
            DataTable dt = (DataTable)ViewState["dtsusp"];
            GridViewRow gvr = dgEmpSusp.Rows[e.RowIndex];
            DataRow dr = dt.Rows[gvr.DataItemIndex];
            dr["off_order_no"] = ((TextBox)gvr.FindControl("txtOffOrderNo")).Text;
            dr["suspen_date"] = ((TextBox)gvr.FindControl("txtSuspenDate")).Text;
            dr["suspen_clause"] = ((TextBox)gvr.FindControl("txtSuspenClause")).Text;
            dr["withdraw_order_no"] = ((TextBox)gvr.FindControl("txtWithdrawOrderNo")).Text;
            dr["with_date"] = ((TextBox)gvr.FindControl("txtWithDate")).Text;
            dr["punishment"] = ((TextBox)gvr.FindControl("txtPunishment")).Text;
            dgEmpSusp.DataSource = dt;
            dgEmpSusp.EditIndex = -1;
            dgEmpSusp.DataBind();
            //updatepanelEmpNo.Update();
            //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is updated provisionally, to update in database permanently please click on Save Link!!');", true);
        }
    }
    protected void dgEmpSusp_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (((DataRowView)e.Row.DataItem)["off_order_no"].ToString() == String.Empty)
            {
                e.Row.Visible = false;
            }
        }
    }
    protected void dgEmpSusp_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (ViewState["dtsusp"] != null)
        {
            SuspSt = "O";
            DataTable dt = (DataTable)ViewState["dtsusp"];
            if (e.CommandName.Equals("AddNew"))
            {
                dgEmpSusp.ShowFooter = true;
                dgEmpSusp.DataSource = dt;
                dgEmpSusp.DataBind();
                ((TextBox)dgEmpSusp.FooterRow.FindControl("txtSuspenDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpSusp.FooterRow.FindControl("txtSuspenDate")).ClientID + "')");
                ((TextBox)dgEmpSusp.FooterRow.FindControl("txtWithDate")).Attributes.Add("onBlur", "formatdate('" + ((TextBox)dgEmpSusp.FooterRow.FindControl("txtWithDate")).ClientID + "')");
            }
            else if (e.CommandName.Equals("Insert"))
            {
                DataRow dr = dt.NewRow();
                dr["off_order_no"] = ((TextBox)dgEmpSusp.FooterRow.FindControl("txtOffOrderNo")).Text;
                dr["suspen_date"] = ((TextBox)dgEmpSusp.FooterRow.FindControl("txtSuspenDate")).Text;
                dr["suspen_clause"] = ((TextBox)dgEmpSusp.FooterRow.FindControl("txtSuspenClause")).Text;
                dr["withdraw_order_no"] = ((TextBox)dgEmpSusp.FooterRow.FindControl("txtWithdrawOrderNo")).Text;
                dr["with_date"] = ((TextBox)dgEmpSusp.FooterRow.FindControl("txtWithDate")).Text;
                dr["punishment"] = ((TextBox)dgEmpSusp.FooterRow.FindControl("txtPunishment")).Text;
                dt.Rows.Add(dr);
                dgEmpSusp.DataSource = dt;
                dgEmpSusp.ShowFooter = false;
                dgEmpSusp.DataBind();
                //updatepanelEmpNo.Update();
                //ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Record is inserted provisionally, to insert in database permanently please click on Save Link!!');", true);
            }
        }
    }

    private void WriteToFile(string strPath, ref byte[] Buffer)
    {
        FileStream newFile = new FileStream(strPath, FileMode.Create);
        newFile.Write(Buffer, 0, Buffer.Length);
        newFile.Close();
    }
    protected void lbSigUpload_Click(object sender, EventArgs e)
    {
        if (txtFName.Text != "" && txtEmpBirthDt.Text != "" && sigUpload.HasFile)
        {
            int width = 145;
            int height = 60;
            using (System.Drawing.Bitmap img = EmpManager.ResizeImage(new System.Drawing.Bitmap(sigUpload.PostedFile.InputStream), width, height, EmpManager.ResizeOptions.ExactWidthAndHeight))
            {
                
                empSigna = EmpManager.ConvertImageToByteArray(img, System.Drawing.Imaging.ImageFormat.Png);
                Session["empSigna"] = empSigna;
                imgUpload.PostedFile.InputStream.Close();
                img.Dispose();
            }
            string base64String = Convert.ToBase64String(empSigna, 0, empSigna.Length);
            imgSig.ImageUrl = "data:image/png;base64," + base64String;
        }
        else
        {
            //lblTranStatus.Visible = true;
            //lblTranStatus.ForeColor = System.Drawing.Color.Red;
            //lblTranStatus.Text = "Please input employee first name, birth date, and then browse a photograph image.";
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please input employee first name, birth date, and then browse a photograph image!!');", true);
        }
    }
    protected void lbImgUpload_Click(object sender, EventArgs e)
    {
        if (imgUpload.HasFile)
        {
            int width = 145;
            int height = 165;
            using (System.Drawing.Bitmap img = EmpManager.ResizeImage(new System.Drawing.Bitmap(imgUpload.PostedFile.InputStream), width, height, EmpManager.ResizeOptions.ExactWidthAndHeight))
            {
                imgUpload.PostedFile.InputStream.Close();                
                empPhoto = EmpManager.ConvertImageToByteArray(img, System.Drawing.Imaging.ImageFormat.Png);
                Session["empPhoto"] = empPhoto;
                img.Dispose();
            }
            string base64String = Convert.ToBase64String(empPhoto, 0, empPhoto.Length);
            imgEmp.ImageUrl = "data:image/png;base64," + base64String;
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please input employee first name, birth date, and then browse a photograph image!!');", true);
        }
    }
    protected void dgEmp_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblID.Text = dgEmp.SelectedRow.Cells[7].Text.ToString().Trim();
        ViewState["Emp_No"] = dgEmp.SelectedRow.Cells[1].Text.ToString().Trim();
        //txtPersoneelFileNo.Text = dgEmp.SelectedRow.Cells[1].Text.ToString().Trim(); 
        BtnFind_Click(sender, e);
    }
    protected void ddlPerDistCode_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlPerThanaCode.Items.Clear();
        string queryThana = "select '' THANA_CODE,'' THANA_NAME union select THANA_CODE, THANA_NAME from dbo.THANA_CODE WHERE DISTRICT_CODE ='" + ddlPerDistCode.SelectedValue + "' order by 2 ";
        util.PopulationDropDownList(ddlPerThanaCode, "Thana", queryThana, "THANA_NAME", "THANA_CODE");
    }
    protected void ddlMailDistCode_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlMailThanaCode.Items.Clear();
        string queryThana = "select '' THANA_CODE,'' THANA_NAME union select THANA_CODE, THANA_NAME from dbo.THANA_CODE WHERE DISTRICT_CODE ='" + ddlMailDistCode.SelectedValue + "' order by 2 ";
        util.PopulationDropDownList(ddlMailThanaCode, "Thana", queryThana, "THANA_NAME", "THANA_CODE");
    }
    protected void dgEmp_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        DataTable dt = EmpManager.GetAllEmployeeInformation();
        dgEmp.DataSource = dt;
        dgEmp.PageIndex = e.NewPageIndex;
        dgEmp.DataBind();
        
    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (txtEmpNo.Text != String.Empty)
        {
            getEmpRptPdf(hfEmpNo.Value);
        }
        else
        {
            //string filePath = "~/Help/" + "EmployeeInfo.pdf";
            //string Name = "EmployeeInfo.pdf";
            //Response.ContentType = "doc/docx";
            //Response.AddHeader("Content-Disposition", "attachment;filename=\"" + Name + "\"");
            //Response.TransmitFile(Server.MapPath(filePath));
            //Response.End();

        }
    }
    private void getEmpRptPdf(string empno)
    
    {
        DataTable dtemp = EmpManager.getEmpRpt(empno);
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment; filename=MypdfFile.pdf");
        Document document = new Document(PageSize.A4, 40f, 30f, 40f, 40f);
        PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        document.Open();
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
        head.WriteSelectedRows(0, -1, 0, page.Height - document.TopMargin + head.TotalHeight + 20, writer.DirectContent);

        PdfPTable foot = new PdfPTable(1);
        foot.TotalWidth = page.Width - 20;
        phrase = new Phrase((document.PageNumber + 1).ToString(), new Font(Font.FontFamily.TIMES_ROMAN, 8));
        c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_BOTTOM;
        c.HorizontalAlignment = Element.ALIGN_RIGHT;
        foot.AddCell(c);
        foot.WriteSelectedRows(0, -1, 0, 20, writer.DirectContent);

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
            cell = new PdfPCell(new Phrase("Employee Resume", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, iTextSharp.text.Font.BOLD)));
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

            Emp emp = EmpManager.getEmp(hfEmpNo.Value.ToString());
            byte[] pht = (byte[])emp.EmpPhoto;
            byte[] sig = (byte[])emp.SpecSigna;            
            PdfPTable pdtphoto = new PdfPTable(2);
            pdtphoto.WidthPercentage = 100;
            if (pht != null)
            {
                iTextSharp.text.Image gifpht = iTextSharp.text.Image.GetInstance(pht);
                gifpht.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                gifpht.ScalePercent(50f);
                cell = new PdfPCell(gifpht);
                cell.HorizontalAlignment = 2;
                cell.VerticalAlignment = 1;
                cell.BorderWidth = 0f;
                pdtphoto.AddCell(cell);
                if (sig != null)
                {
                    iTextSharp.text.Image gifsig = iTextSharp.text.Image.GetInstance(sig);
                    gifsig.Alignment = iTextSharp.text.Image.MIDDLE_ALIGN;
                    gifsig.ScalePercent(50f);
                    cell = new PdfPCell(gifsig);
                    cell.HorizontalAlignment = 1;
                    cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                    cell.BorderWidth = 0f;
                    pdtphoto.AddCell(cell);
                }
                else
                {
                    cell = new PdfPCell(FormatPhrase(""));
                    cell.HorizontalAlignment = 0;
                    cell.VerticalAlignment = 1;
                    cell.BorderWidth = 0;
                    cell.FixedHeight = 18f;
                    cell.BorderColor = BaseColor.LIGHT_GRAY;
                    pdtphoto.AddCell(cell);
                }
            }

            float[] widthemp = new float[7] { 30, 5, 50, 5, 30, 5, 50 };
            PdfPTable pdtemp = new PdfPTable(widthemp);
            pdtemp.WidthPercentage = 100;
            cell = new PdfPCell(FormatPhrase("Employee Id"));
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
            cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["Emp_No"].ToString()));
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
            cell = new PdfPCell(FormatPhrase("Mail Address"));
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

            cell = new PdfPCell(FormatPhrase("Religion"));
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
            cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["religion"].ToString()));
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

            cell = new PdfPCell(FormatPhrase("Email"));
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
            cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["E_MAIL"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(FormatPhrase("Mobile"));
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
            cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["MOBILE"].ToString()));
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

            cell = new PdfPCell(FormatPhrase("Phone"));
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
            cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["RES_PH_NO"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);

            cell = new PdfPCell(FormatPhrase("Nationality"));
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
            cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["NATIONALITY"].ToString()));
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

            cell = new PdfPCell(FormatPhrase("National ID No"));
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
            cell = new PdfPCell(FormatPhrase(((DataRow)dtemp.Rows[0])["NATIONAL_ID"].ToString()));
            cell.HorizontalAlignment = 0;
            cell.VerticalAlignment = 1;
            cell.BorderWidth = 0;
            cell.MinimumHeight = 18f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            pdtemp.AddCell(cell);
            document.Add(pdtemp);

            empno = lblID.Text;
            DataTable dtedu = EduManager.getEduRpt(empno);
            if (dtedu.Rows.Count > 0)
            {
                PdfPTable dtempty1 = new PdfPTable(1);
                dtempty1.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Education : "));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                cell.BorderWidth = 0f;
                cell.FixedHeight = 25f;
                dtempty1.AddCell(cell);
                document.Add(dtempty1);

                float[] widthedu = new float[6] { 30, 20, 50, 15, 30, 15 };
                PdfPTable pdtedu = new PdfPTable(widthedu);
                pdtedu.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Exam Name"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Group"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Institution"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Pass year"));
                cell.HorizontalAlignment = 1;
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
                cell.HorizontalAlignment = 1;
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
                document.Add(pdtedu);
            }
            //************* Family *****************//
            DataTable dtFam = FamManager.getFams(empno);
            dtFam.Columns.Remove("EMP_NO");
            dtFam.Columns.Remove("birth_dt");
            if (dtFam.Rows.Count > 0)
            {
                PdfPTable dtempty1 = new PdfPTable(1);
                dtempty1.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Family : "));
                cell.HorizontalAlignment = 0;
                cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                cell.BorderWidth = 0f;
                cell.FixedHeight = 25f;
                dtempty1.AddCell(cell);
                document.Add(dtempty1);

                float[] widthedu = new float[4] { 30, 20, 15, 15 };
                PdfPTable pdtedu = new PdfPTable(widthedu);
                pdtedu.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Relative Name"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Relation"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Mob.Number"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                cell = new PdfPCell(FormatHeaderPhrase("Occupation"));
                cell.HorizontalAlignment = 1;
                cell.VerticalAlignment = 1;
                cell.MinimumHeight = 16f;
                cell.BorderColor = BaseColor.LIGHT_GRAY;
                pdtedu.AddCell(cell);
                for (int i = 0; i < dtFam.Rows.Count; i++)
                {
                    for (int j = 0; j < dtFam.Columns.Count; j++)
                    {
                        cell = new PdfPCell(FormatPhrase(((DataRow)dtFam.Rows[i])[j].ToString()));
                        cell.HorizontalAlignment = 0;
                        cell.VerticalAlignment = 1;
                        cell.MinimumHeight = 16f;
                        cell.BorderColor = BaseColor.LIGHT_GRAY;
                        pdtedu.AddCell(cell);
                    }
                }
                document.Add(pdtedu);
            }

            DataTable dtexp = ExperManager.getExpRpt(empno);
            if (dtexp.Rows.Count > 0)
            {
                PdfPTable dtempty2 = new PdfPTable(1);
                dtempty2.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Past Public/Private Sector Experience : " ));
                cell.HorizontalAlignment = iTextSharp.text.Rectangle.ALIGN_LEFT;
                cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                cell.BorderWidth = 0f;
                cell.FixedHeight = 25f;
                dtempty2.AddCell(cell);
                document.Add(dtempty2);

                float[] widthexp = new float[4] { 50, 50, 15, 15 };
                PdfPTable pdtexp = new PdfPTable(widthexp);
                pdtexp.WidthPercentage = 100;
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
                document.Add(pdtexp);
            }

            DataTable dtprom = PromManager.getPromRpt(empno);
            if (dtprom.Rows.Count > 0)
            {
                PdfPTable dtempty3 = new PdfPTable(1);
                dtempty3.WidthPercentage = 100;
                cell = new PdfPCell(FormatHeaderPhrase("Promtions:"));
                cell.HorizontalAlignment = iTextSharp.text.Rectangle.ALIGN_LEFT;
                cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                cell.BorderWidth = 0f;
                cell.FixedHeight = 25f;
                dtempty3.AddCell(cell);
                document.Add(dtempty3);

                float[] widthprom = new float[5] { 50, 20, 50, 50, 15 };
                PdfPTable pdtprom = new PdfPTable(widthprom);
                pdtprom.WidthPercentage = 100;
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
                document.Add(pdtprom);

                DataTable dttrans = TransfManager.getTransRpt(empno);
                if (dttrans.Rows.Count > 0)
                {
                    PdfPTable dtempty4 = new PdfPTable(1);
                    dtempty4.WidthPercentage = 100;
                    cell = new PdfPCell(FormatHeaderPhrase("Transfer : "));
                    cell.HorizontalAlignment = iTextSharp.text.Rectangle.ALIGN_LEFT;
                    cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                    cell.BorderWidth = 0f;
                    cell.FixedHeight = 25f;
                    dtempty4.AddCell(cell);
                    document.Add(dtempty4);

                    float[] widthtrans = new float[5] { 50, 20, 20, 50, 50 };
                    PdfPTable pdttrans = new PdfPTable(widthtrans);
                    pdttrans.WidthPercentage = 100;
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
                    document.Add(pdttrans);
                }
                DataTable dttrain = TrainManager.getTrainRpt(empno);
                if (dttrain.Rows.Count > 0)
                {
                    PdfPTable dtempty5 = new PdfPTable(1);
                    dtempty5.WidthPercentage = 100;
                    cell = new PdfPCell(FormatHeaderPhrase("Training Information [Local & Foreign Training Including Study Tour, Seminar and Workshops]:"));
                    cell.HorizontalAlignment = iTextSharp.text.Rectangle.ALIGN_LEFT;
                    cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                    cell.BorderWidth = 0f;
                    cell.FixedHeight = 25f;
                    dtempty5.AddCell(cell);
                    document.Add(dtempty5);

                    float[] widthtrain = new float[9] { 50, 10, 30, 15, 15, 15, 8, 8, 8 };
                    PdfPTable pdttrain = new PdfPTable(widthtrain);
                    pdttrain.WidthPercentage = 100;
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
                    document.Add(pdttrain);
                }

                DataTable dtqtr = QtrManager.getQtrRpt(empno);
                if (dtqtr.Rows.Count > 0)
                {
                    PdfPTable dtempty6 = new PdfPTable(1);
                    dtempty6.WidthPercentage = 100;
                    cell = new PdfPCell(FormatHeaderPhrase("Quarter:"));
                    cell.HorizontalAlignment = iTextSharp.text.Rectangle.ALIGN_LEFT;
                    cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                    cell.BorderWidth = 0f;
                    cell.FixedHeight = 25f;
                    dtempty6.AddCell(cell);
                    document.Add(dtempty6);

                    float[] widthqtr = new float[9] { 50, 12, 18, 30, 20, 10, 10, 10, 10 };
                    PdfPTable pdtqtr = new PdfPTable(widthqtr);
                    pdtqtr.WidthPercentage = 100;
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
                    document.Add(pdtqtr);
                }

                DataTable dtsusp = SuspManager.getSuspRpt(empno);
                if (dtsusp.Rows.Count > 0)
                {
                    PdfPTable dtempty7 = new PdfPTable(1);
                    dtempty7.WidthPercentage = 100;
                    cell = new PdfPCell(FormatHeaderPhrase("Reward/Punishment:"));
                    cell.HorizontalAlignment = iTextSharp.text.Rectangle.ALIGN_LEFT;
                    cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                    cell.BorderWidth = 0f;
                    cell.FixedHeight = 25f;
                    dtempty7.AddCell(cell);
                    document.Add(dtempty7);

                    float[] widthsusp = new float[6] { 50, 15, 50, 40, 15, 40 };
                    PdfPTable pdtsusp = new PdfPTable(widthsusp);
                    pdtsusp.WidthPercentage = 100;
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
                    document.Add(pdtsusp);
                }

                DataTable dtmemb = MembManager.getMembRpt(empno);
                if (dtmemb.Rows.Count > 0)
                {
                    PdfPTable dtempty8 = new PdfPTable(1);
                    dtempty8.WidthPercentage = 100;
                    cell = new PdfPCell(FormatHeaderPhrase("Member of:"));
                    cell.HorizontalAlignment = iTextSharp.text.Rectangle.ALIGN_LEFT;
                    cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                    cell.BorderWidth = 0f;
                    cell.FixedHeight = 25f;
                    dtempty8.AddCell(cell);
                    document.Add(dtempty8);

                    float[] widthmemb = new float[2] { 100,30 };
                    PdfPTable pdtmemb = new PdfPTable(widthmemb);
                    pdtmemb.WidthPercentage = 70;
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
                    document.Add(pdtmemb);
                }

                DataTable dtfam = FamManager.getFamRpt(empno);
                if (dtfam.Rows.Count > 0)
                {
                    PdfPTable dtempty9 = new PdfPTable(1);
                    dtempty9.WidthPercentage = 100;
                    cell = new PdfPCell(FormatHeaderPhrase("Family Information:"));
                    cell.HorizontalAlignment = iTextSharp.text.Rectangle.ALIGN_LEFT;
                    cell.VerticalAlignment = iTextSharp.text.Rectangle.ALIGN_BOTTOM;
                    cell.BorderWidth = 0f;
                    cell.FixedHeight = 25f;
                    dtempty9.AddCell(cell);
                    document.Add(dtempty9);

                    float[] widthfam = new float[5] { 50, 20, 20, 15, 40 };
                    PdfPTable pdtfam = new PdfPTable(widthfam);
                    pdtfam.WidthPercentage = 100;
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
                    cell = new PdfPCell(FormatHeaderPhrase("Age"));
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
                    document.Add(pdtfam);
                }
            }
            document.Close();
            Response.Flush();
            Response.End();
        }
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
    protected void dgEmp_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Footer | e.Row.RowType == DataControlRowType.Header)
        {
           
            e.Row.Cells[7].Attributes.Add("style", "display:none");
        }
    }

    //*************** Upload File ******************//
    //****************** Document Upload ************//
    public void getEmptyDocumentTable()
    {
        DataTable dtDocument=new DataTable();
        dtDocument.Columns.Add("ID", typeof(string));
        dtDocument.Columns.Add("Mst_ID", typeof(string));
        dtDocument.Columns.Add("FileDescription", typeof(string));
        dtDocument.Columns.Add("FileImage", typeof(byte[]));
        ViewState["Document"] = dtDocument;
    }
    protected void btnUploadFile_Click(object sender, EventArgs e)
    {
        //if (string.IsNullOrEmpty(lblID.Text))
        //{
        //    ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please save branch first then upload file.!!');", true);
        //    return;
        //}
        if (string.IsNullOrEmpty(txtFileDescription.Text))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Input File Description..!!');", true);
            return;
        }
        if (fileUpload1.HasFile)
        {
            string extension = Path.GetExtension(fileUpload1.FileName);
            if (!extension.ToLower().Equals(".pdf"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please Select PDF file only.!!');", true);
                return;
            }
            string filePath = fileUpload1.PostedFile.FileName;
            string filename1 = Path.GetFileName(filePath);
            Stream fs = fileUpload1.PostedFile.InputStream;
            BinaryReader br = new BinaryReader(fs); //reads the binary files
            Byte[] PasportFilbytes = br.ReadBytes((Int32)fs.Length);
            DataRow drDoc;
            DataTable dtDoc = (DataTable) ViewState["Document"];
            if (ViewState["Document"] != null)
            {
                drDoc = dtDoc.NewRow();
                if (dtDoc != null)
                {
                    drDoc["ID"] = (dtDoc.Rows.Count + 99999999).ToString();
                }
                else
                {
                    drDoc["ID"] = "99999999";
                }
                drDoc["FileDescription"] = txtFileDescription.Text;
                drDoc["FileImage"] = PasportFilbytes;
                dtDoc.Rows.Add(drDoc);
                ViewState["Document"] = dtDoc;
            }
            else
            {
              
                drDoc = dtDoc.NewRow();
                if (dtDoc != null)
                {
                    drDoc["ID"] = (dtDoc.Rows.Count + 99999999).ToString();
                }
                else
                {
                    drDoc["ID"] = "99999999";
                }
                drDoc["FileDescription"] = txtFileDescription.Text;
                drDoc["FileImage"] = PasportFilbytes;
                dtDoc.Rows.Add(drDoc);
                ViewState["Document"] = dtDoc;
                
            }
            dgQuestion.DataSource = ViewState["Document"];
            dgQuestion.DataBind();
            txtFileDescription.Text = string.Empty;
            txtFileDescription.Focus();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ale", "alert('Please Select File...!!');", true);
            return;
        }
    }
    protected void dgQuestion_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Download"))
        {
            GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
            DataTable dtDecumentDtl =(DataTable) ViewState["Document"];
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
        if (e.Row.RowType == DataControlRowType.DataRow | e.Row.RowType == DataControlRowType.Footer | e.Row.RowType == DataControlRowType.Header)
        {

            e.Row.Cells[0].Attributes.Add("style", "display:none");
        }
    }
    protected void dgQuestion_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable dtDoc = (DataTable)ViewState["Document"];
        dtDoc.AcceptChanges();
        foreach (DataRow row in dtDoc.Rows)
        {
            if (row["ID"].ToString().Equals(dgQuestion.SelectedRow.Cells[0].Text))
            {
                row.Delete();
            }
        }
        dtDoc.AcceptChanges();
        ViewState["Document"] = dtDoc;
        dgQuestion.DataSource = dtDoc;
        dgQuestion.DataBind();
    }
}