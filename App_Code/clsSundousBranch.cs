using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for clsSundousBranch
/// </summary>
public class clsSundousBranch
{
	public clsSundousBranch()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string BranchId, BranchName, Address, Mobile, Email;

    public clsSundousBranch(DataRow dr)
    {
        if (dr["ID"].ToString() != String.Empty)
        {
            this.BranchId = dr["ID"].ToString();
        }
        if (dr["BranchName"].ToString() != String.Empty)
        {
            this.BranchName = dr["BranchName"].ToString();
        }
        if (dr["Address"].ToString() != String.Empty)
        {
            this.Address = dr["Address"].ToString();
        }
        if (dr["MobileNumber"].ToString() != String.Empty)
        {
            this.Mobile = dr["MobileNumber"].ToString();
        }
        if (dr["Email"].ToString() != String.Empty)
        {
            this.Email = dr["Email"].ToString();
        }
     }

    public string LoginBy { get; set; }
}