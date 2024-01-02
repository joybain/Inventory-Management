using System;
using System.Data;
using System.Configuration;
using System.Linq;






using System.Xml.Linq;

/// <summary>
/// Summary description for clsMeasure
/// </summary>
public class clsMeasure
{
    public string MsrUnitCode;
    public string MsrUnitDesc;

	public clsMeasure()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public clsMeasure(DataRow dr)
    {
        if (dr["ID"].ToString() != string.Empty)
        {
            this.MsrUnitCode = dr["ID"].ToString();
        }
        if (dr["Name"].ToString() != string.Empty)
        {
            this.MsrUnitDesc = dr["Name"].ToString();
        }
    }
}
