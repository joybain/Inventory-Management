using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Summary description for clsBankMst
/// </summary>
public class clsBankMst
{
	public string BankId;
    public string BankName;
    
    public clsBankMst()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public clsBankMst(DataRow dr)
    {
        if (dr["bank_id"].ToString() != string.Empty)
        {
            this.BankId = dr["bank_id"].ToString();
        }
        if (dr["bank_name"].ToString() != string.Empty)
        {
            this.BankName = dr["bank_name"].ToString();
        }
    }

    public string LoginBy { get; set; }

    public string SegmentCoaCode { get; set; }

    public string ParentCode { get; set; }

    public string bankType { get; set; }
}
