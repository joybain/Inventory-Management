using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for clsCurrency
/// </summary>
public class clsCurrency
{
	public clsCurrency()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string CurrencyId, Date, Rate;

    public clsCurrency(DataRow dr)
    {
        if (dr["ID"].ToString() != String.Empty)
        {
            this.CurrencyId = dr["ID"].ToString();
        }
        if (dr["CurrencyDate"].ToString() != String.Empty)
        {
            this.Date = dr["CurrencyDate"].ToString();
        }
        if (dr["CurrencyRate"].ToString() != String.Empty)
        {
            this.Rate = dr["CurrencyRate"].ToString();
        }
     }

    public string LoginBy { get; set; }
}