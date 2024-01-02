using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for MajorCategory
/// </summary>
public class MajorCategory
{
    public string ID;
    public string Name,Description,Active,Code;

	public MajorCategory()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public MajorCategory(DataRow dr)
    {
        if (dr["ID"].ToString() != String.Empty) {this.ID = dr["ID"].ToString(); }
        if (dr["Code"].ToString() != String.Empty) { this.Code = dr["Code"].ToString(); }
        if (dr["Name"].ToString() != String.Empty) { this.Name = dr["Name"].ToString(); }
        if (dr["Active"].ToString() != String.Empty) { this.Active = dr["Active"].ToString(); }
        if (dr["Description"].ToString() != String.Empty) { this.Description = dr["Description"].ToString(); }
    }

    public string DeptID { get; set; }

    public string LoginBy { get; set; }
}