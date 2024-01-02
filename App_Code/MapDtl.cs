using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Summary description for MapDtl
/// </summary>
namespace Delve
{     
    public class MapDtl
    {
        public String BookName;
        public String TypeCode;
        public String VerNo;
        public string SlNo;
        public String GlSegCode;
        public String Description;
        public string BalFrom;
        public String AddLess;
        public string ConsAmt;
        

        public MapDtl()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public MapDtl(DataRow dr)
        {
            if (dr["book_name"].ToString() != String.Empty)
            {
                this.BookName = dr["book_name"].ToString();
            }
            if (dr["type_code"].ToString() != String.Empty)
            {
                this.TypeCode = dr["type_code"].ToString();
            }
            if (dr["ver_no"].ToString() != String.Empty)
            {
                this.VerNo = dr["ver_no"].ToString();
            }
            if (dr["sl_no"].ToString() != String.Empty)
            {
                this.SlNo = dr["sl_no"].ToString();
            }
            if (dr["gl_seg_code"].ToString() != String.Empty)
            {
                this.GlSegCode = dr["gl_seg_code"].ToString();
            }
            if (dr["description"].ToString() != String.Empty)
            {
                this.Description = dr["description"].ToString();
            }
            if (dr["bal_from"].ToString() != String.Empty)
            {
                this.BalFrom = dr["bal_from"].ToString();
            }
            if (dr["add_less"].ToString() != String.Empty)
            {
                this.AddLess = dr["add_less"].ToString();
            }
            if (dr["cons_amt"].ToString() != String.Empty)
            {
                this.ConsAmt = dr["cons_amt"].ToString();
            }
        }
    }
}