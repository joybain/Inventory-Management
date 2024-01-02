using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for PartyAdvance
/// </summary>
public class PartyAdvance
{
	public PartyAdvance()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public PartyAdvance(DataRow dr)
    {
        if (dr["PartySupID"].ToString() != String.Empty)
        {
            this.MstID = dr["PartySupID"].ToString();
        }
        if (dr["AdvanceAmount"].ToString() != String.Empty)
        {
            this.Advance = dr["AdvanceAmount"].ToString();
        }
        if (dr["PaymentAmount"].ToString() != String.Empty)
        {
            this.PaymentAmount = dr["PaymentAmount"].ToString();
        }
        if (dr["VoucherNo"].ToString() != String.Empty)
        {
            this.VoucherNo = dr["VoucherNo"].ToString();
        }
        if (dr["PayMethod"].ToString() != String.Empty)
        {
            this.PayMethod = dr["PayMethod"].ToString();
        }
        if (dr["Bank_id"].ToString() != String.Empty)
        {
            this.Bank_id = dr["Bank_id"].ToString();
        }
        if (dr["ChequeNo"].ToString() != String.Empty)
        {
            this.ChequeNo = dr["ChequeNo"].ToString();
        }
        if (dr["ChequeDate"].ToString() != String.Empty)
        {
            this.ChequeDate = dr["ChequeDate"].ToString();
        }
        if (dr["Chk_Status"].ToString() != String.Empty)
        {
            this.Chk_Status = dr["Chk_Status"].ToString();
        }
        if (dr["PayDate"].ToString() != String.Empty)
        {
            this.PayDate = dr["PayDate"].ToString();
        }
        if (dr["GRN"].ToString() != String.Empty)
        {
            this.GRN = dr["GRN"].ToString();
        }
    }
    public string MstID, Advance, PaymentAmount, VoucherNo, LoginBy, ID, PayMethod, Bank_id, ChequeNo, ChequeDate, Chk_Status, PayDate, GRN;

    public string Flag { get; set; }
}