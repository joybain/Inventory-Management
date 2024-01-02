using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for clsMoneyReceipt
/// </summary>
public class clsMoneyReceipt
{
    public string MrCode, MrDate, IssueId, DeedId, ClientId, PayAmt, PayMethod, ChequeNo, ChequeDate, BankName, AdvDeedId, UsedAdvance,aDeedId;

	public clsMoneyReceipt()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public clsMoneyReceipt(DataRow dr)
    {
        if (dr["MrCode"].ToString() != string.Empty) { this.MrCode = dr["MrCode"].ToString(); }
        if (dr["MrDate"].ToString() != string.Empty) { this.MrDate = dr["MrDate"].ToString(); }
        if (dr["IssueId"].ToString() != string.Empty) { this.IssueId = dr["IssueId"].ToString(); }
        if (dr["DeedId"].ToString() != string.Empty) { this.DeedId = dr["DeedId"].ToString(); }
        if (dr["ClientId"].ToString() != string.Empty) { this.ClientId = dr["ClientId"].ToString(); }
        if (dr["PayAmt"].ToString() != string.Empty) { this.PayAmt = dr["PayAmt"].ToString(); }
        if (dr["PayMethod"].ToString() != string.Empty) { this.PayMethod = dr["PayMethod"].ToString(); }
        if (dr["ChequeNo"].ToString() != string.Empty) { this.ChequeNo = dr["ChequeNo"].ToString(); }
        if (dr["ChequeDate"].ToString() != string.Empty) { this.ChequeDate = dr["ChequeDate"].ToString(); }
        if (dr["BankName"].ToString() != string.Empty) { this.BankName = dr["BankName"].ToString(); }
        if (dr["advDeedId"].ToString() != string.Empty) { this.AdvDeedId = dr["advDeedId"].ToString(); }
        if (dr["used_advance_payment"].ToString() != string.Empty) { this.UsedAdvance = dr["used_advance_payment"].ToString(); }
        if (dr["aDeedId"].ToString() != string.Empty) { this.aDeedId = dr["aDeedId"].ToString(); }
    }



   
}