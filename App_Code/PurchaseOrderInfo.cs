using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for PurchaseOrder
/// </summary>
public class PurchaseOrderInfo
{
    public string ID, PO, PODate, SupplierID, TermsOfDelivery, TermsOfPayment, OrderStatus, ExpDelDate, POCode;
	public PurchaseOrderInfo()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public PurchaseOrderInfo(DataRow dr)
    {
        if (dr["ID"].ToString() != String.Empty) { this.ID = dr["ID"].ToString(); }
        if (dr["PO"].ToString() != String.Empty) { this.PO = dr["PO"].ToString(); }
        if (dr["PODate"].ToString() != String.Empty) { this.PODate = dr["PODate"].ToString(); }
        if (dr["SupplierID"].ToString() != String.Empty) { this.SupplierID = dr["SupplierID"].ToString(); }
        if (dr["TermsOfDelivery"].ToString() != String.Empty) { this.TermsOfDelivery = dr["TermsOfDelivery"].ToString(); }
        if (dr["TermsOfPayment"].ToString() != String.Empty) { this.TermsOfPayment = dr["TermsOfPayment"].ToString(); }
        if (dr["ExpDelDate"].ToString() != String.Empty) { this.ExpDelDate = dr["ExpDelDate"].ToString(); }
        if (dr["OrderStatus"].ToString() != String.Empty) { this.OrderStatus = dr["OrderStatus"].ToString(); }
        if (dr["POCode"].ToString() != String.Empty) { this.POCode = dr["POCode"].ToString(); }
    }

    public string LoginBy { get; set; }

    //public string POCode { get; set; }
}