using System;
using System.Data;
using System.Configuration;


/// <summary>
/// Summary description for BudgetDtl
/// </summary>
public class BudgetDtl
{
    public String BudSysId;
    public String GlCoaCode;
    public string BudIncPct;
    public string BudTolPct;
    public string BudTolAmnt;
    public string BudOverrideAmnt;
    public string BudAmnt;
    public String Status;
    public String BookName;

    public BudgetDtl()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public BudgetDtl(DataRow dr)
    {
        if (dr["bud_sys_id"].ToString() != String.Empty)
        {
            this.BudSysId = dr["bud_sys_id"].ToString();
        }
        if (dr["gl_coa_code"].ToString() != String.Empty)
        {
            this.GlCoaCode = dr["gl_coa_code"].ToString();
        }
        if (dr["bud_inc_pct"].ToString() != String.Empty)
        {
            this.BudIncPct = dr["bud_inc_pct"].ToString();
        }
        if (dr["bud_tol_pct"].ToString() != String.Empty)
        {
            this.BudTolPct = dr["bud_tol_pct"].ToString();
        }
        if (dr["bud_tol_amnt"].ToString() != String.Empty)
        {
            this.BudTolAmnt = dr["bud_tol_amnt"].ToString();
        }
        if (dr["bud_override_amnt"].ToString() != String.Empty)
        {
            this.BudOverrideAmnt = dr["bud_override_amnt"].ToString();
        }
        if (dr["bud_amnt"].ToString() != String.Empty)
        {
            this.BudAmnt = dr["bud_amnt"].ToString();
        }
        if (dr["status"].ToString() != String.Empty)
        {
            this.Status = dr["status"].ToString();
        }
        if (dr["book_name"].ToString() != String.Empty)
        {
            this.BookName = dr["book_name"].ToString();
        }
    }

}
