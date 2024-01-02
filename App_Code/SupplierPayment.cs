using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for SupplierPayment
/// </summary>
public class SupplierPayment
{
    public string ID, PmDate, PvID, SupplierID, PayAmt, PaymentMethord, BankId, ChequeNo, ChequeDate, ChequeStatus, Payment_Type, GRN;
	public SupplierPayment()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public SupplierPayment(DataRow dr)
    {
        if (dr["ID"].ToString() != String.Empty) { this.ID = dr["ID"].ToString(); }
        if (dr["Payment_Type"].ToString() != String.Empty) { this.Payment_Type = dr["Payment_Type"].ToString(); }
        if (dr["PmDate"].ToString() != String.Empty) { this.PmDate = dr["PmDate"].ToString(); }        
        if (dr["supplier_id"].ToString() != String.Empty) { this.SupplierID = dr["supplier_id"].ToString(); }
        if (dr["PayAmt"].ToString() != String.Empty) { this.PayAmt = dr["PayAmt"].ToString(); }
        if (dr["PayMethod"].ToString() != String.Empty) { this.PaymentMethord = dr["PayMethod"].ToString(); }
        if (dr["ChequeNo"].ToString() != String.Empty) { this.ChequeNo = dr["ChequeNo"].ToString(); }
        if (dr["ChequeDate"].ToString() != String.Empty) { this.ChequeDate = dr["ChequeDate"].ToString(); }
        if (dr["Bank_id"].ToString() != String.Empty) { this.BankId = dr["Bank_id"].ToString(); }
        if (dr["Chk_Status"].ToString() != String.Empty) { this.ChequeStatus = dr["Chk_Status"].ToString(); }
        if (dr["purchase_id"].ToString() != String.Empty) { this.PvID = dr["purchase_id"].ToString(); }
        if (dr["GRN"].ToString() != String.Empty) { this.GRN = dr["GRN"].ToString(); }
    }
    public string LoginBy { get; set; }

    public string PartyOrSupplier { get; set; }
}