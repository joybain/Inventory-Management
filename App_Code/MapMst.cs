using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Summary description for MapMst
/// </summary>
namespace Delve
{
    public class MapMst
    {
        public String BookName;
        public String TypeCode;
        public String VerNo;
        public String Description;
        public String RefTypeCode;
        public String RefVerNo;
        public string Active;
        public string EntryUser;
        public string EntryDate;
        public string AuhtoUser;
        public string AuthoDate;


        public MapMst()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public MapMst(DataRow dr)
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
            if (dr["description"].ToString() != String.Empty)
            {
                this.Description = dr["description"].ToString();
            }
            if (dr["ref_type_code"].ToString() != String.Empty)
            {
                this.RefTypeCode = dr["ref_type_code"].ToString();
            }
            if (dr["ref_ver_no"].ToString() != String.Empty)
            {
                this.RefVerNo = dr["ref_ver_no"].ToString();
            }
            if (dr["active"].ToString() != String.Empty)
            {
                this.Active = dr["active"].ToString();
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
                this.AuhtoUser = dr["autho_user"].ToString();
            }
            if (dr["autho_date"].ToString() != String.Empty)
            {
                this.AuthoDate = dr["autho_date"].ToString();
            }
        }
    }
}