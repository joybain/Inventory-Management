using System;
using System.Data;
using System.Configuration;

/// <summary>
/// Summary description for VouchDtl
/// </summary>

namespace Delve
{
    public class VouchDtl
    {
        public String VchSysNo;
        public String LineNo;
        public String GlCoaCode;
        public String ValueDate;
        public String Particulars;
        public String AccType;
        public String AmountDr;
        public String AmountCr;
        public String Status;
        public string BookName, Amount_DR_PH, Amount_CR_PH, Amount_DR_BD, Amount_CR_DB, AUTHO_USER;

        public VouchDtl()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public VouchDtl(DataRow dr)
        {
            if (dr["vch_sys_no"].ToString() != String.Empty)
            {
                this.VchSysNo = dr["vch_sys_no"].ToString();
            }
            if (dr["line_no"].ToString() != String.Empty)
            {
                this.LineNo = dr["line_no"].ToString();
            }
            if (dr["gl_coa_code"].ToString() != String.Empty)
            {
                this.GlCoaCode = dr["gl_coa_code"].ToString();
            }
            if (dr["value_date"].ToString() != String.Empty)
            {
                this.ValueDate = dr["value_date"].ToString();
            }
            if (dr["particulars"].ToString() != String.Empty)
            {
                this.Particulars = dr["particulars"].ToString();
            }
            if (dr["acc_type"].ToString() != String.Empty)
            {
                this.AccType = dr["acc_type"].ToString();
            }
            if (dr["amount_dr"].ToString() != String.Empty)
            {
                this.AmountDr = dr["amount_dr"].ToString();
            }
            if (dr["amount_cr"].ToString() != String.Empty)
            {
                this.AmountCr = dr["amount_cr"].ToString();
            }
            if (dr["status"].ToString() != String.Empty)
            {
                this.Status = dr["status"].ToString();
            }
            if (dr["book_name"].ToString() != String.Empty)
            {
                this.BookName = dr["book_name"].ToString();
            }
            if (dr["Amount_DR_PH"].ToString() != String.Empty)
            {
                this.Amount_DR_PH = dr["Amount_DR_PH"].ToString();
            }
            if (dr["Amount_CR_PH"].ToString() != String.Empty)
            {
                this.Amount_CR_PH = dr["Amount_CR_PH"].ToString();
            }
            if (dr["Amount_DR_BD"].ToString() != String.Empty)
            {
                this.Amount_DR_BD = dr["Amount_DR_BD"].ToString();
            }
            if (dr["Amount_CR_DB"].ToString() != String.Empty)
            {
                this.Amount_CR_DB = dr["Amount_CR_DB"].ToString();
            }
            if (dr["AUTHO_USER"].ToString() != String.Empty)
            {
                this.AUTHO_USER = dr["AUTHO_USER"].ToString();
            }
        }
    }
}