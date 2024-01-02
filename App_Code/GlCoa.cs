using System;
using System.Collections.Generic;
using System.Linq;

using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for GlCoa
/// </summary>
/// 
namespace Delve
{
    public class GlCoa
    {
        public String BookName;
        public String GlCoaCode;
        public String CoaEnabled;
        public string EffectiveFrom;
        public string EffectiveTo;
        public String BudAllowed;
        public String PostAllowed;
        public String Taxable;
        public String AccType;
        public String Status;
        public String CoaDesc;
        public String CoaCurrBal;
        public String CoaNaturalCode;

        public GlCoa()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public GlCoa(DataRow dr)
        {
            if (dr["book_name"].ToString() != String.Empty)
            {
                this.BookName = dr["book_name"].ToString();
            }
            if (dr["gl_coa_code"].ToString() != String.Empty)
            {
                this.GlCoaCode = dr["gl_coa_code"].ToString();
            }
            if (dr["coa_enabled"].ToString() != String.Empty)
            {
                this.CoaEnabled = dr["coa_enabled"].ToString();
            }
            if (dr["effective_from"].ToString() != String.Empty)
            {
                this.EffectiveFrom = dr["effective_from"].ToString();
            }
            if (dr["effective_to"].ToString() != String.Empty)
            {
                this.EffectiveTo = dr["effective_To"].ToString();
            }
            if (dr["bud_allowed"].ToString() != String.Empty)
            {
                this.BudAllowed = dr["bud_allowed"].ToString();
            }
            if (dr["post_allowed"].ToString() != String.Empty)
            {
                this.PostAllowed = dr["post_allowed"].ToString();
            }
            if (dr["taxable"].ToString() != String.Empty)
            {
                this.Taxable = dr["taxable"].ToString();
            }
            if (dr["acc_type"].ToString() !=String.Empty)
            {
                this.AccType = dr["acc_type"].ToString();
            }
            if (dr["status"].ToString() != String.Empty)
            {
                this.Status = dr["status"].ToString();
            }
            if (dr["coa_desc"].ToString() != String.Empty)
            {
                this.CoaDesc = dr["coa_desc"].ToString();
            }
            if (dr["coa_curr_bal"].ToString() != String.Empty)
            {
                this.CoaCurrBal = dr["coa_curr_bal"].ToString();
            }
            if (dr["coa_natural_code"].ToString() != String.Empty)
            {
                this.CoaNaturalCode = dr["coa_natural_code"].ToString();
            }

        }

        public string LoginBy { get; set; }
    }
}