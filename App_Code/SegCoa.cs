using System;
using System.Collections.Generic;
using System.Linq;

using System.Data;


/// <summary>
/// Summary description for SegCoa
/// </summary>
/// 
namespace Delve
{
    public class SegCoa
    {
        public string GlSegCode;
        public string LvlCode;
        public string SegCoaDesc;
        public string ParentCode;
        public string BudAllowed;
        public string PostAllowed;
        public string AccType;
        public string OpenDate;
        public string RootLeaf;
        public string Status;
        public string Taxable;
        public string BookName;
        public string EntryUser;
        public string EntryDate;
        public string AuthoUser;
        public string AuthoDate;
        public string ClientName;
        public string BankName;
        public SegCoa()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public SegCoa(DataRow dr)
        {
            if (dr["seg_coa_code"].ToString() != String.Empty)
            {
                this.GlSegCode = dr["seg_coa_code"].ToString();
            }
            if (dr["lvl_code"].ToString() != String.Empty)
            {
                this.LvlCode = dr["lvl_code"].ToString();
            }
            if (dr["seg_coa_desc"].ToString() != String.Empty)
            {
                this.SegCoaDesc = dr["seg_coa_desc"].ToString();
            }
            if (dr["parent_code"].ToString() != String.Empty)
            {
                this.ParentCode = dr["parent_code"].ToString();
            }
            if (dr["bud_allowed"].ToString() != String.Empty)
            {
                this.BudAllowed = dr["bud_allowed"].ToString();
            }
            if (dr["post_allowed"].ToString() != String.Empty)
            {
                this.PostAllowed = dr["post_allowed"].ToString();
            }
            if (dr["acc_type"].ToString() != String.Empty)
            {
                this.AccType = dr["acc_type"].ToString();
            }
            if (dr["open_date"].ToString() != String.Empty)
            {
                this.OpenDate = dr["open_date"].ToString();
            }
            if (dr["rootleaf"].ToString() != String.Empty)
            {
                this.RootLeaf = dr["rootleaf"].ToString();
            }
            if (dr["status"].ToString() != String.Empty)
            {
                this.Status = dr["status"].ToString();
            }
            if (dr["taxable"].ToString() != String.Empty)
            {
                this.Taxable = dr["taxable"].ToString();
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

        public string Cash_Bank_Status { get; set; }
    }
}