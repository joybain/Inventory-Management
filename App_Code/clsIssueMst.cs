using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for clsIssueMst
/// </summary>
public class clsIssueMst
{
    public string IssueId, IssueDate, BookId, DeedId, ClientId, Remarks;

	public clsIssueMst()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public clsIssueMst(DataRow dr)
    {
        if (dr["IssueId"].ToString() != string.Empty) { this.IssueId = dr["IssueId"].ToString(); }
        if (dr["IssueDate"].ToString() != string.Empty) { this.IssueDate = dr["IssueDate"].ToString(); }
        if (dr["BookId"].ToString() != string.Empty) { this.BookId = dr["BookId"].ToString(); }
        if (dr["DeedId"].ToString() != string.Empty) { this.DeedId = dr["DeedId"].ToString(); }
        if (dr["ClientId"].ToString() != string.Empty) { this.ClientId = dr["ClientId"].ToString(); }
        if (dr["Remarks"].ToString() != string.Empty) { this.Remarks = dr["Remarks"].ToString(); }
    }
}