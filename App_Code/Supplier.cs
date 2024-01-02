using System;
using System.Data;
using System.Configuration;
using System.Linq;






using System.Xml.Linq;

/// <summary>
/// Summary description for Supplier
/// </summary>
public class Supplier
{
    public string SupCode, ComName, SupName, SupAddr1, SupAddr2, Designation, City, SupPhone, State, SupMobile, PostCode, Fax, Country, Email, SupGroup, ID, Active, GlCoa;

    public Supplier()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public Supplier(DataRow dr)
    {
        if (dr["ID"].ToString() != String.Empty) { this.ID = dr["ID"].ToString(); }
        if (dr["Code"].ToString() != String.Empty) { this.SupCode = dr["Code"].ToString(); }
        if (dr["Name"].ToString() != String.Empty) { this.ComName = dr["Name"].ToString(); }
        if (dr["ContactName"].ToString() != String.Empty) { this.SupName = dr["ContactName"].ToString(); }
        if (dr["Designation"].ToString() != String.Empty) { this.Designation = dr["Designation"].ToString(); }
        if (dr["Email"].ToString() != String.Empty) { this.Email = dr["Email"].ToString(); }
        if (dr["Fax"].ToString() != String.Empty) { this.Fax = dr["Fax"].ToString(); }
        if (dr["Mobile"].ToString() != String.Empty) { this.SupMobile = dr["Mobile"].ToString(); }
        if (dr["Phone"].ToString() != String.Empty) { this.SupPhone = dr["Phone"].ToString(); }
        if (dr["Address1"].ToString() != String.Empty) { this.SupAddr1 = dr["Address1"].ToString(); }
        if (dr["Address2"].ToString() != String.Empty) { this.SupAddr2 = dr["Address2"].ToString(); }
        if (dr["City"].ToString() != String.Empty) { this.City = dr["City"].ToString(); }
        if (dr["State"].ToString() != String.Empty) { this.State = dr["State"].ToString(); }
        if (dr["PostalCode"].ToString() != String.Empty) { this.PostCode = dr["PostalCode"].ToString(); }
        if (dr["Country"].ToString() != String.Empty) { this.Country = dr["Country"].ToString(); }
        if (dr["SupplierGroupID"].ToString() != String.Empty) { this.SupGroup = dr["SupplierGroupID"].ToString(); }
        if (dr["Active"].ToString() != String.Empty) { this.Active = dr["Active"].ToString(); }
        if (dr["Gl_CoaCode"].ToString() != String.Empty) { this.GlCoa = dr["Gl_CoaCode"].ToString(); }

        if (dr["NIDImage"].ToString() != string.Empty)
        {
            this.SupNIDImage = (byte[])dr["NIDImage"];
        }
        if (dr["CurrentImage"].ToString() != string.Empty)
        {
            this.SupCurrentImage = (byte[])dr["CurrentImage"];
        }
    }

    public string LoginBy { get; set; }



    public string CoaDesc { get; set; }

    public Byte[] SupNIDImage { get; set; }

    public Byte[] SupCurrentImage { get; set; }

    public string CompanyName { get; set; }
}
