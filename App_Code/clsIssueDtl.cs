using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for clsIssueDtl
/// </summary>
public class clsIssueDtl
{
    public string IssueId, ItemCode, MsrUnitCode, Qnty;
	public clsIssueDtl()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public clsIssueDtl(DataRow dr)
    {
        if (dr["IssueId"].ToString() != string.Empty) { this.IssueId = dr["IssueId"].ToString(); }
        if (dr["item_code"].ToString() != string.Empty) { this.ItemCode = dr["item_code"].ToString(); }
        if (dr["msr_unit_code"].ToString() != string.Empty) { this.MsrUnitCode = dr["msr_unit_code"].ToString(); }
        if (dr["qnty"].ToString() != string.Empty) { this.Qnty = dr["qnty"].ToString(); }
    }
}