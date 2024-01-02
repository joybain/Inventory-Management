using System;
using System.Data;
using System.Configuration;
using System.Linq;
using Delve;

/// <summary>
/// Summary description for GlLevel
/// </summary>
/// 
namespace Delve
{
    public class GlLevel
    {
        public String BookName;
        public String LvlCode;
        public String LvlDesc;
        public String LvlMaxSize;
        public String LvlEnabled;
        public String LvlSegType;
        public String LvlOrder;
        public string Status;
        public string EntryUser;
        public string EntryDate;
        public string AuthoUser;
        public string AuthoDate;
       
        public GlLevel()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public GlLevel(DataRow dr)
        {
            if (dr["book_name"].ToString() != String.Empty)
            {
                this.BookName = dr["book_name"].ToString();
            }
            if (dr["lvl_code"].ToString() != String.Empty)
            {
                this.LvlCode = dr["lvl_code"].ToString();
            }
            if (dr["lvl_desc"].ToString() != String.Empty)
            {
                this.LvlDesc = dr["lvl_desc"].ToString();
            }
            if (dr["lvl_max_size"].ToString() != String.Empty)
            {
                this.LvlMaxSize = dr["lvl_max_size"].ToString();
            }
            if (dr["lvl_enabled"].ToString() != String.Empty)
            {
                this.LvlEnabled = dr["lvl_enabled"].ToString();
            }
            if (dr["lvl_seg_type"].ToString() != String.Empty)
            {
                this.LvlSegType = dr["lvl_seg_type"].ToString();
            }
            if (dr["lvl_order"].ToString() != String.Empty)
            {
                this.LvlOrder = dr["lvl_order"].ToString();
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
}