using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for clsBranchSetup
/// </summary>
public class clsBranchSetup
{
    public string ComKey, BranchKey, BranchName, ShortName, Address1, Phone, Mobile, Fax, EMail, IssuingPlace, Computerized, Status, Flag;
	public clsBranchSetup()
	{
       
	}
    public clsBranchSetup(DataRow dr)
    {
        if (dr["ComKey"].ToString() != string.Empty) { this.ComKey = dr["ComKey"].ToString(); }
        if (dr["BranchKey"].ToString() != string.Empty) { this.BranchKey = dr["BranchKey"].ToString(); }
        if (dr["BranchName"].ToString() != string.Empty) { this.BranchName = dr["BranchName"].ToString(); }
        if (dr["ShortName"].ToString() != string.Empty) { this.ShortName = dr["ShortName"].ToString(); }
        if (dr["Address1"].ToString() != string.Empty) { this.Address1 = dr["Address1"].ToString(); }
        if (dr["Phone"].ToString() != string.Empty) { this.Phone = dr["Phone"].ToString(); }
        if (dr["Mobile"].ToString() != string.Empty) { this.Mobile = dr["Mobile"].ToString(); }
        if (dr["Fax"].ToString() != string.Empty) { this.Fax = dr["Fax"].ToString(); }
        if (dr["EMail"].ToString() != string.Empty) { this.EMail = dr["EMail"].ToString(); }
        if (dr["IssuingPlace"].ToString() != string.Empty) { this.IssuingPlace = dr["IssuingPlace"].ToString(); }
        if (dr["Computerized"].ToString() != string.Empty) { this.Computerized = dr["Computerized"].ToString(); }
        if (dr["Status"].ToString() != string.Empty) { this.Status = dr["Status"].ToString(); }
        if (dr["Flag"].ToString() != string.Empty) { this.Flag = dr["Flag"].ToString(); }
    }

    public string ID { get; set; }
}