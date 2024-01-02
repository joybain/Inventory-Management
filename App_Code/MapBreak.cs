using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Summary description for MapBreak
/// </summary>
/// 
namespace Delve
{
    public class MapBreak
    {
        public String BookName;
        public String TypeCode;
        public String VerNo;
        public string RefSlNo;
        public string SlNo, ADD_LESS;
        public String GlSegCode;
        

        public MapBreak()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public MapBreak(DataRow dr)
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
            if (dr["ref_sl_no"].ToString() != String.Empty)
            {
                this.RefSlNo = dr["ref_sl_no"].ToString();
            }
            if (dr["sl_no"].ToString() != String.Empty)
            {
                this.SlNo = dr["sl_no"].ToString();
            }
            if (dr["gl_seg_code"].ToString() != String.Empty)
            {
                this.GlSegCode = dr["gl_seg_code"].ToString();
            }
            if (dr["ADD_LESS"].ToString() != String.Empty)
            {
                this.ADD_LESS = dr["ADD_LESS"].ToString();
            }
        }
    }
}