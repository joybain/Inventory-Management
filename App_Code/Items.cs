using System;
using System.Data;
using System.Configuration;
using System.Linq;






using System.Xml.Linq;

/// <summary>
/// Summary description for Items
/// </summary>
/// 
namespace Delve
{
    public class Items
    {
        public String ItemCode;
        public String ItemDesc;
        public String MsrCode;
        public string ItemRate;
        public String ItemDescbang;


        public Items()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public Items(DataRow dr)
        {
            if (dr["item_code"].ToString() != String.Empty)
            {
                this.ItemCode = dr["item_code"].ToString();
            }
            if (dr["item_desc"].ToString() != String.Empty)
            {
                this.ItemDesc = dr["item_desc"].ToString();
            }
            if (dr["msr_unit_code"].ToString() != String.Empty)
            {
                this.MsrCode = dr["msr_unit_code"].ToString();
            }
            if (dr["item_rate"].ToString() != String.Empty)
            {
                this.ItemRate = dr["item_rate"].ToString();
            }
            if (dr["ITEM_DESC_Bang"].ToString() != String.Empty)
            {
                this.ItemDescbang = dr["ITEM_DESC_Bang"].ToString();
            }
        }

    }
}