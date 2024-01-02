using System;
using System.Data;
using System.Configuration;

/// <summary>
/// Summary description for VouchMst
/// </summary>
public class VouchMst
{
    public String VchSysNo;
    public String FinMon;
    public String ValueDate;
    public String VchRefNo;
    public String RefFileNo;
    public String VolumeNo;
    public String SerialNo;
    public String VchCode;
    public String TransType;
    public String Particulars;
    public String ControlAmt;
    public String BookName;
    public String Payee;
    public String CheckNo;
    public String CheqDate;
    public String CheqAmnt;
    public String MoneyRptNo;
    public String MoneyRptDate;
    public String Status;
    public String EntryUser;
    public String EntryDate;
    public String UpdateUser;
    public String UpdateDate;
    public String AuthoUser;
    public String AuthoDate;
    public String AuthoUserType;

    public VouchMst()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public VouchMst(DataRow dr)
    {
        if (dr["vch_sys_no"].ToString() != String.Empty)
        {
            this.VchSysNo = dr["vch_sys_no"].ToString();
        }
        if (dr["fin_mon"].ToString() != String.Empty)
        {
            this.FinMon = dr["fin_mon"].ToString();
        }
        if (dr["value_date"].ToString() != String.Empty)
        {
            this.ValueDate = dr["value_date"].ToString();
        }
        if (dr["vch_ref_no"].ToString() != String.Empty)
        {
            this.VchRefNo = dr["vch_ref_no"].ToString();
        }
        if (dr["ref_file_no"].ToString() != String.Empty)
        {
            this.RefFileNo = dr["ref_file_no"].ToString();
        }
        if (dr["volume_no"].ToString() != String.Empty)
        {
            this.VolumeNo = dr["volume_no"].ToString();
        }
        if (dr["serial_no"].ToString() != String.Empty)
        {
            this.SerialNo = dr["serial_no"].ToString();
        }
        if (dr["vch_code"].ToString() != String.Empty)
        {
            this.VchCode = dr["vch_code"].ToString();
        }
        if (dr["trans_type"].ToString() != String.Empty)
        {
            this.TransType = dr["trans_type"].ToString();
        }
        if (dr["particulars"].ToString() != String.Empty)
        {
            this.Particulars = dr["particulars"].ToString();
        }
        if (dr["control_amt"].ToString() != String.Empty)
        {
            this.ControlAmt = dr["control_amt"].ToString();
        }
        if (dr["book_name"].ToString() != String.Empty)
        {
            this.BookName = dr["book_name"].ToString();
        }
        if (dr["payee"].ToString() != String.Empty)
        {
            this.Payee = dr["payee"].ToString();
        }
        if (dr["check_no"].ToString() != String.Empty)
        {
            this.CheckNo = dr["check_no"].ToString();
        }
        if (dr["cheq_date"].ToString() != String.Empty)
        {
            this.CheqDate = dr["cheq_date"].ToString();
        }
        if (dr["cheq_amnt"].ToString() != String.Empty)
        {
            this.CheqAmnt = dr["cheq_amnt"].ToString();
        }
        if (dr["money_rpt_no"].ToString() != String.Empty)
        {
            this.MoneyRptNo = dr["money_rpt_no"].ToString();
        }
        if (dr["money_rpt_date"].ToString() != String.Empty)
        {
            this.MoneyRptDate = dr["money_rpt_date"].ToString();
        }
        if (dr["status"].ToString() != String.Empty)
        {
            this.Status = dr["status"].ToString();
        }
        if (dr["entry_user"].ToString() != String.Empty)
        {
            this.EntryUser = dr["entry_user"].ToString();
        }
        if (dr["entry_date"].ToString() != String.Empty)
        {
            this.EntryDate = dr["entry_date"].ToString();
        }
        if (dr["update_user"].ToString() != String.Empty)
        {
            this.UpdateUser = dr["update_user"].ToString();
        }
        if (dr["update_date"].ToString() != String.Empty)
        {
            this.UpdateDate = dr["update_date"].ToString();
        }
        if (dr["autho_user"].ToString() != String.Empty)
        {
            this.AuthoUser = dr["autho_user"].ToString();
        }
        if (dr["autho_date"].ToString() != String.Empty)
        {
            this.AuthoDate = dr["autho_date"].ToString();
        }
        if (dr["autho_user_type"].ToString() != String.Empty)
        {
            this.AuthoUserType = dr["autho_user_type"].ToString();
        }
    }


    public string UserType { get; set; }
}