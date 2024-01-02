using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for SaleReturn
/// </summary>
public class SaleReturn
{
	public SaleReturn()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string ID, GRN, ReturnDate, Remarks, Return_No, PaymentMethod, BankName, ChequeNo, ChequeDate, Chk_Status, TotalAmount, Pay_Amount, BranchID;
     public SaleReturn(DataRow dr)
     {
        if (dr["ID"].ToString() != String.Empty) {this.ID = dr["ID"].ToString();}
        if (dr["InvoiceNo"].ToString() != String.Empty) { this.GRN = dr["InvoiceNo"].ToString(); }
        if (dr["ReturnDate"].ToString() != String.Empty) { this.ReturnDate = dr["ReturnDate"].ToString(); }
        if (dr["Remarks"].ToString() != String.Empty) { this.Remarks = dr["Remarks"].ToString(); }
        if (dr["Return_No"].ToString() != String.Empty) { this.Return_No = dr["Return_No"].ToString(); }
        if (dr["TotalAmount"].ToString() != String.Empty) { this.TotalAmount = dr["TotalAmount"].ToString(); }
        if (dr["Pay_Amount"].ToString() != String.Empty) { this.Pay_Amount = dr["Pay_Amount"].ToString(); }

        if (dr["PaymentMethod"].ToString() != String.Empty) { this.PaymentMethod = dr["PaymentMethod"].ToString(); }
        if (dr["BankName"].ToString() != String.Empty) { this.BankName = dr["BankName"].ToString(); }
        if (dr["ChequeNo"].ToString() != String.Empty) { this.ChequeNo = dr["ChequeNo"].ToString(); }
        if (dr["ChequeDate"].ToString() != String.Empty) { this.ChequeDate = dr["ChequeDate"].ToString(); }
        if (dr["Chk_Status"].ToString() != String.Empty) { this.Chk_Status = dr["Chk_Status"].ToString(); }
        if (dr["BranchID"].ToString() != String.Empty) { this.BranchID = dr["BranchID"].ToString(); }
     }

    public string LogonBy { get; set; }

    public string SupplierID { get; set; }

    public string SupplierName { get; set; }

    public string BankCoaCode { get; set; }

    public string bankNameDes { get; set; }

    public string Due { get; set; }

    public string CustomerID { get; set; }
 
}