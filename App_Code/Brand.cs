using System;
using System.Data;
using System.Configuration;
using System.Linq;






using System.Xml.Linq;

/// <summary>
/// Summary description for Company
/// </summary>
public class Brand
{
    public String CompanyId;
    public String CompanyDesc;
    public String Active;

    public Brand()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public Brand(DataRow dr)
    {
        if (dr["ID"].ToString() != String.Empty)
        {
            this.CompanyId = dr["ID"].ToString();
        }
        if (dr["BrandName"].ToString() != String.Empty)
        {
            this.CompanyDesc = dr["BrandName"].ToString();
        }
        if (dr["Active"].ToString() != String.Empty)
        {
            this.Active = dr["Active"].ToString();
        }
    }
}
