using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for SubMajorCategory
/// </summary>
public class SubMajorCategory
{
    
    public string ID,Name,Description,Catagory,Active,Code;
   
	public SubMajorCategory()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public SubMajorCategory(DataRow dr)
    {
        if (dr["ID"].ToString() != String.Empty) { this.ID = dr["ID"].ToString(); }
        if (dr["Code"].ToString() != String.Empty) { this.Code = dr["Code"].ToString(); }
        if (dr["Name"].ToString() != String.Empty) { this.Name = dr["Name"].ToString(); }
        if (dr["Description"].ToString() != String.Empty) { this.Description = dr["Description"].ToString(); }
        if (dr["CategoryID"].ToString() != String.Empty) { this.Catagory = dr["CategoryID"].ToString(); }
        if (dr["Active"].ToString() != String.Empty) { this.Active = dr["Active"].ToString(); }
    }

    public string DeptID { get; set; }

    public string LoginBy { get; set; }
}