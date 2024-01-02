using System;
using System.Collections.Generic;
using System.Linq;

using System.Data;

/// <summary>
/// Summary description for FinMonth
/// </summary>
/// 


public class FinMonth
{
    public String BookName;
    public String FinMon;
    public String FinYear;
    public String MonthSl;
    public String Quarter;
    public String MonStartDt;
    public String MonEndDt;
    public String YearFlag;

    public FinMonth()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public FinMonth(DataRow dr)
    {
        if (dr["book_name"].ToString() != String.Empty)
        {
            this.BookName = dr["book_name"].ToString();
        }
        if (dr["fin_mon"].ToString() != String.Empty)
        {
            this.FinMon = dr["fin_mon"].ToString();
        }
        if (dr["fin_year"].ToString() != String.Empty)
        {
            this.FinYear = dr["fin_year"].ToString();
        }
        if (dr["month_sl"].ToString() != String.Empty)
        {
            this.MonthSl = dr["month_sl"].ToString();
        }
        if (dr["quarter"].ToString() != String.Empty)
        {
            this.Quarter = dr["quarter"].ToString();
        }
        if (dr["mon_start_dt"].ToString() != String.Empty)
        {
            this.MonStartDt = dr["mon_start_dt"].ToString();
        }
        if (dr["mon_end_dt"].ToString() != String.Empty)
        {
            this.MonEndDt = dr["mon_end_dt"].ToString();
        }
        if (dr["year_flag"].ToString() != String.Empty)
        {
            this.YearFlag = dr["year_flag"].ToString();
        }
    }
}