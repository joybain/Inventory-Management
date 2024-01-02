using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for clsClientPaymentRec
/// </summary>
public class clsClientPaymentRec
{
    public string ID, Date, Customer_id, PayAmt, PaymentMethord, BankId, ChequeNo, ChequeDate, Chk_Status, INV_ID, InvoiceNo;
	public clsClientPaymentRec()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public clsClientPaymentRec(DataRow dr)
    {
        if (dr["ID"].ToString() != String.Empty) { this.ID = dr["ID"].ToString(); }
        if (dr["Date"].ToString() != String.Empty) { this.Date = dr["Date"].ToString(); }
        if (dr["Customer_id"].ToString() != String.Empty) { this.Customer_id = dr["Customer_id"].ToString(); }
        if (dr["PayAmt"].ToString() != String.Empty) { this.PayAmt = dr["PayAmt"].ToString(); }
        if (dr["PayMethod"].ToString() != String.Empty) { this.PaymentMethord = dr["PayMethod"].ToString(); }
        if (dr["Bank_id"].ToString() != String.Empty) { this.BankId = dr["Bank_id"].ToString(); }
        if (dr["ChequeNo"].ToString() != String.Empty) { this.ChequeNo = dr["ChequeNo"].ToString(); }
        if (dr["ChequeDate"].ToString() != String.Empty) { this.ChequeDate = dr["ChequeDate"].ToString(); }
        if (dr["Chk_Status"].ToString() != String.Empty) { this.Chk_Status = dr["Chk_Status"].ToString(); }
        if (dr["INV_ID"].ToString() != String.Empty) { this.INV_ID = dr["INV_ID"].ToString(); }
        if (dr["InvoiceNo"].ToString() != String.Empty) { this.InvoiceNo = dr["InvoiceNo"].ToString(); }
    }
    public string LoginBy { get; set; }
    public string BranchId { get; set; }

    public string P_Type { get; set; }

    public string Remarks { get; set; }

    public string CustomerCoa { get; set; }
}