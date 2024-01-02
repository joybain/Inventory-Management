using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for clsSupplierPaymentRec
/// </summary>
public class clsSupplierPaymentRec
{
    public string ID, Date, Supplier_id, PayAmt, PaymentMethord, BankId, ChequeNo, ChequeDate, Chk_Status, PurchaseVoucherID, GRN;

	public clsSupplierPaymentRec()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public clsSupplierPaymentRec(DataRow dr)
    {
        if (dr["ID"].ToString() != String.Empty) { this.ID = dr["ID"].ToString(); }
        if (dr["Date"].ToString() != String.Empty) { this.Date = dr["Date"].ToString(); }
        if (dr["Supplier_id"].ToString() != String.Empty) { this.Supplier_id = dr["Supplier_id"].ToString(); }
        if (dr["PayAmt"].ToString() != String.Empty) { this.PayAmt = dr["PayAmt"].ToString(); }
        if (dr["PayMethod"].ToString() != String.Empty) { this.PaymentMethord = dr["PayMethod"].ToString(); }
        if (dr["Bank_id"].ToString() != String.Empty) { this.BankId = dr["Bank_id"].ToString(); }
        if (dr["ChequeNo"].ToString() != String.Empty) { this.ChequeNo = dr["ChequeNo"].ToString(); }
        if (dr["ChequeDate"].ToString() != String.Empty) { this.ChequeDate = dr["ChequeDate"].ToString(); }
        if (dr["Chk_Status"].ToString() != String.Empty) { this.Chk_Status = dr["Chk_Status"].ToString(); }
        if (dr["PurchaseVoucherID"].ToString() != String.Empty) { this.PurchaseVoucherID = dr["PurchaseVoucherID"].ToString(); }
        if (dr["GRN"].ToString() != String.Empty) { this.GRN = dr["GRN"].ToString(); }
    }
    public string LoginBy { get; set; }

    public string P_Type { get; set; }

    public string Remarks { get; set; }

    public string SupplierCoa { get; set; }
}