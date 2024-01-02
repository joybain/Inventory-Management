using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;

/// <summary>
/// Summary description for Emp
/// </summary>
public class Emp
{
    public string EmpNo;
    public string FName;
    public string MName;
    public string LName;
    public string JoinDate;
    public string JobStatus;
    public string JoinDesigCode;
    public string ConfirmDate;
    public string EmpStatus;
    public string EmpType;
    public string EmpCat;
    public string BranchKey;
    public string PfNo;
    public string EmpBirthDt;
    public string Sex;
    public string PlaceOfBirth;
    public string MaritalStatusCode;
    public string BloodGroup;
    public string SpouseName;
    public string Nationality;
    public string FhName;
    public string MhName;
    public string ReligionCode;
    public string PerLoc;
    public string PerDistCode;
    public string PerThanaCode;
    public string ZipAreaCode;
    public string MailLoc;
    public string MailDistCode;
    public string MailThanaCode;
    public string EMail;
    public string ResPhNo;
    public string Mobile;
    public string BankBranchNo;
    public string BankAccNo;
    public string EmpInsuredDt;
    public string SpouseInsDt;
    public string SenrSlNo;
    public string LastIncrDt;
    public string PrstDesigCode;
    public string PrstPostBr;
    public string PrstPostDt;
    public string ImmePrevBr;
    public string LastPromDate;
    public string LprDate;
    public Byte[] EmpPhoto;
    public Byte[] SpecSigna;
    public string PassNo;
    public string DrivLicNo;
    public string NationalId;
    public string PersoneelFileNo;
    public string TinNo;
    public string GlCoaCode;
    public string County, BasicSalary;

    public Emp()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Emp(DataRow dr)
    {
        if (dr["Emp_No"].ToString() != String.Empty)
        {
            this.EmpCode = dr["Emp_No"].ToString();
        }
        if (dr["Emp_No"].ToString() != String.Empty)
        {
            this.EmpCode = dr["Emp_No"].ToString();
        }

        if (dr["BasicSalary"].ToString() != String.Empty)
        {
            this.BasicSalary = dr["BasicSalary"].ToString();
        }
        if (dr["GlCoaCode"].ToString() != String.Empty)
        {
            this.GlCoaCode = dr["GlCoaCode"].ToString();
        }
        if (dr["ID"].ToString() != String.Empty)
        {
            this.EmpNo = dr["ID"].ToString();
        }
        if (dr["f_name"].ToString() != String.Empty)
        {
            this.FName = dr["f_name"].ToString();
        }
        if (dr["m_name"].ToString() != String.Empty)
        {
            this.MName = dr["m_name"].ToString();
        }
        if (dr["l_name"].ToString() != String.Empty)
        {
            this.LName = dr["l_name"].ToString();
        }
        if (dr["join_date"].ToString() != String.Empty)
        {
            this.JoinDate = dr["join_date"].ToString();
        }
        if (dr["job_status"].ToString() != String.Empty)
        {
            this.JobStatus = dr["job_status"].ToString();
        }
        if (dr["join_desig_code"].ToString() != String.Empty)
        {
            this.JoinDesigCode = dr["join_desig_code"].ToString();
        }
        if (dr["confirm_date"].ToString() != String.Empty)
        {
            this.ConfirmDate = dr["confirm_date"].ToString();
        }
        if (dr["emp_status"].ToString() != String.Empty)
        {
            this.EmpStatus = dr["emp_status"].ToString();
        }
        if (dr["emp_type"].ToString() != String.Empty)
        {
            this.EmpType = dr["emp_type"].ToString();
        }
        if (dr["emp_cat"].ToString() != String.Empty)
        {
            this.EmpCat = dr["emp_cat"].ToString();
        }
        if (dr["BRANCH_CODE"].ToString() != String.Empty)
        {
            this.BranchKey = dr["BRANCH_CODE"].ToString();
        }
        if (dr["pf_no"].ToString() != String.Empty)
        {
            this.PfNo = dr["pf_no"].ToString();
        }
        if (dr["emp_birth_dt"].ToString() != String.Empty)
        {
            this.EmpBirthDt = dr["emp_birth_dt"].ToString();
        }
        if (dr["sex"].ToString() != String.Empty)
        {
            this.Sex = dr["sex"].ToString();
        }
        if (dr["place_of_birth"].ToString() != String.Empty)
        {
            this.PlaceOfBirth = dr["place_of_birth"].ToString();
        }
        if (dr["marital_status_code"].ToString() != String.Empty)
        {
            this.MaritalStatusCode = dr["marital_status_code"].ToString();
        }
        if (dr["blood_group"].ToString() != String.Empty)
        {
            this.BloodGroup = dr["blood_group"].ToString();
        }
        if (dr["spouse_name"].ToString() != String.Empty)
        {
            this.SpouseName = dr["spouse_name"].ToString();
        }
        if (dr["nationality"].ToString() != String.Empty)
        {
            this.Nationality = dr["nationality"].ToString();
        }
        if (dr["fh_name"].ToString() != String.Empty)
        {
            this.FhName = dr["fh_name"].ToString();
        }
        if (dr["mh_name"].ToString() != String.Empty)
        {
            this.MhName = dr["mh_name"].ToString();
        }
        if (dr["religion_code"].ToString() != String.Empty)
        {
            this.ReligionCode = dr["religion_code"].ToString();
        }
        if (dr["per_loc"].ToString() != String.Empty)
        {
            this.PerLoc = dr["per_loc"].ToString();
        }
        if (dr["per_dist_code"].ToString() != String.Empty)
        {
            this.PerDistCode = dr["per_dist_code"].ToString();
        }
        if (dr["per_thana_code"].ToString() != String.Empty)
        {
            this.PerThanaCode = dr["per_thana_code"].ToString();
        }
        if (dr["zip_area_code"].ToString() != String.Empty)
        {
            this.ZipAreaCode = dr["zip_area_code"].ToString();
        }
        if (dr["mail_loc"].ToString() != String.Empty)
        {
            this.MailLoc = dr["mail_loc"].ToString();
        }
        if (dr["mail_loc"].ToString() != String.Empty)
        {
            this.MailDistCode = dr["mail_dist_code"].ToString();
        }
        if (dr["mail_thana_code"].ToString() != String.Empty)
        {
            this.MailThanaCode = dr["mail_thana_code"].ToString();
        }
        if (dr["e_mail"].ToString() != String.Empty)
        {
            this.EMail = dr["e_mail"].ToString();
        }
        if (dr["res_ph_no"].ToString() != String.Empty)
        {
            this.ResPhNo = dr["res_ph_no"].ToString();
        }
        if (dr["mobile"].ToString() != String.Empty)
        {
            this.Mobile = dr["mobile"].ToString();
        }
        //if (dr["BankBranchKey"].ToString() != String.Empty)
        //{
        //    this.BankBranchNo = dr["BankBranchKey"].ToString();
        //}
        if (dr["bank_acc_no"].ToString() != String.Empty)
        {
            this.BankAccNo = dr["bank_acc_no"].ToString();
        }
        if (dr["emp_insured_dt"].ToString() != String.Empty)
        {
            this.EmpInsuredDt = dr["emp_insured_dt"].ToString();
        }
        if (dr["spouse_ins_dt"].ToString() != String.Empty)
        {
            this.SpouseInsDt = dr["spouse_ins_dt"].ToString();
        }
        if (dr["senr_sl_no"].ToString() != String.Empty)
        {
            this.SenrSlNo = dr["senr_sl_no"].ToString();
        }
        if (dr["prst_desig_code"].ToString() != String.Empty)
        {
            this.PrstDesigCode = dr["prst_desig_code"].ToString();
        }
        if (dr["prst_post_br"].ToString() != String.Empty)
        {
            this.PrstPostBr = dr["prst_post_br"].ToString();
        }
        if (dr["lpr_date"].ToString() != String.Empty)
        {
            this.LprDate = dr["lpr_date"].ToString();
        }
        if (dr["emp_photo"].ToString() != String.Empty)
        {
            this.EmpPhoto = (byte[])dr["emp_photo"];
        }
        if (dr["spec_signa"].ToString() != String.Empty)
        {
            this.SpecSigna = (byte[])dr["spec_signa"];
        }
        if (dr["pass_no"].ToString() != String.Empty)
        {
            this.PassNo = dr["pass_no"].ToString();
        }
        if (dr["driv_lic_no"].ToString() != String.Empty)
        {
            this.DrivLicNo = dr["driv_lic_no"].ToString();
        }
        if (dr["national_id"].ToString() != String.Empty)
        {
            this.NationalId = dr["national_id"].ToString();
        }
        if (dr["personeel_file_no"].ToString() != String.Empty)
        {
            this.PersoneelFileNo = dr["personeel_file_no"].ToString();
        }
        if (dr["tin_no"].ToString() != String.Empty)
        {
            this.TinNo = dr["tin_no"].ToString();
        }
        if (dr["CountryID"].ToString() != String.Empty)
        {
            this.County = dr["CountryID"].ToString();
        }
        if (dr["BANK_NO"].ToString() != String.Empty)
        {
            this.BankBranchNo = dr["BANK_NO"].ToString();
        }

        if (dr["MAIL_POST_CODE"].ToString() != String.Empty)
        {
            this.MailPostCode = dr["MAIL_POST_CODE"].ToString();
        }

    }

    //public string GlCoa { get; set; }

    public string descript { get; set; }



    public string EmpCode { get; set; }

    public string MailPostCode { get; set; }

    public string UpdateBy { get; set; }

    public string EntryBy { get; set; }
}
