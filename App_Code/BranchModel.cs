using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BranchModel
/// </summary>
public class BranchModel
{
	public BranchModel()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string Id { get; set; }

    public string BranchName { get; set; }
    public string ShortName { get; set; }

    public string Address1 { get; set; }


    public string Address2 { get; set; }

    public string Phone { get; set; }
    public string Mobile { get; set; }

    public string EMail { get; set; }
    public string VatRegNo { get; set; }

    public bool  Status { get; set; }
}