using System;
using System.Data;
using System.Configuration;
//using System.Linq;






//using System.Xml.Linq;

/// <summary>
/// Summary description for clsCountry
/// </summary>
public class clsCountry
{
    public string CountryCode;
    public string CountryAbvr;
    public string CountryDesc;
	public clsCountry()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public clsCountry(DataRow dr)
    {
        if (dr["country_code"].ToString() != String.Empty)
        {
            this.CountryCode = dr["country_code"].ToString();
        }
        if (dr["country_abvr"].ToString() != String.Empty)
        {
            this.CountryAbvr = dr["country_anvr"].ToString();
        }
        if (dr["country_desc"].ToString() != String.Empty)
        {
            this.CountryDesc = dr["country_desc"].ToString();
        }
    }
}
