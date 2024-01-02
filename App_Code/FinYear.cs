using System;
using System.Collections.Generic;
using System.Linq;

using System.Data;


/// <summary>
/// Summary description for FinYear
/// </summary>
/// 

public class FinYear
{
    public String BookName;
    public String FinYr;
    public String StartDate;
    public String EndDate;
    public String Description;
    public String WeeklyFin;
    public String YearFlag;
    public string Status;
    public String EntryUser;
    public String EntryDate;
    public String AuthoUser;
    public String AuthoDate;

    public FinYear()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public FinYear(DataRow dr)
    {
        if (dr["book_name"].ToString() != String.Empty)
        {
            this.BookName = dr["book_name"].ToString();
        }
        if (dr["fin_year"].ToString() != String.Empty)
        {
            this.FinYr = dr["fin_year"].ToString();
        }
        if (dr["start_date"].ToString() != String.Empty)
        {
            this.StartDate = dr["start_date"].ToString();
        }
        if (dr["end_date"].ToString() != String.Empty)
        {
            this.EndDate = dr["end_date"].ToString();
        }
        if (dr["description"].ToString() != String.Empty)
        {
            this.Description = dr["description"].ToString();
        }
        if (dr["weekly_fin"].ToString() != String.Empty)
        {
            this.WeeklyFin = dr["weekly_fin"].ToString();
        }
        if (dr["year_flag"].ToString() != String.Empty)
        {
            this.YearFlag = dr["year_flag"].ToString();
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