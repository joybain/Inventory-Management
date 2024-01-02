using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Summary description for BudgetMst
/// </summary>
/// 

public class BudgetMst
{
    public String BudSysId;
    public String BudDesc;
    public String FinYear;
    public string FinStartDt;
    public string FinEndDt;
    public String BudTypeCode;
    public String BudOpen;
    public String Status;
    public String BookName;
    public String EntryUser;
    public string EntryDate;
    public String AuthoUser;
    public string AuthoDate;

    public BudgetMst()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public BudgetMst(DataRow dr)
    {
        if (dr["bud_sys_id"].ToString() != String.Empty)
        {
            this.BudSysId = dr["bud_sys_id"].ToString();
        }
        if (dr["bud_desc"].ToString() != String.Empty)
        {
            this.BudDesc = dr["bud_desc"].ToString();
        }
        if (dr["fin_year"].ToString() != String.Empty)
        {
            this.FinYear = dr["fin_year"].ToString();
        }
        if (dr["fin_start_dt"].ToString() != String.Empty)
        {
            this.FinStartDt = dr["fin_start_dt"].ToString();
        }
        if (dr["fin_end_dt"].ToString() != String.Empty)
        {
            this.FinEndDt = dr["fin_end_dt"].ToString();
        }
        if (dr["bud_type_code"].ToString() != String.Empty)
        {
            this.BudTypeCode = dr["bud_type_code"].ToString();
        }
        if (dr["bud_open"].ToString() != String.Empty)
        {
            this.BudOpen = dr["bud_open"].ToString();
        }
        if (dr["status"].ToString() != String.Empty)
        {
            this.Status = dr["status"].ToString();
        }
        if (dr["book_name"].ToString() != String.Empty)
        {
            this.BookName = dr["book_name"].ToString();
        }
        if (dr["entry_user"].ToString() != String.Empty)
        {
            this.EntryUser = dr["entry_user"].ToString();
        }
        if (dr["entry_date"].ToString() != String.Empty)
        {
            this.EntryDate = dr["entry_date"].ToString();
        }
        if (dr["autho_user"].ToString() != String.Empty)
        {
            this.AuthoUser = dr["autho_user"].ToString();
        }
        if (dr["autho_date"].ToString() != String.Empty)
        {
            this.AuthoDate = dr["autho_date"].ToString();
        }
    }
}