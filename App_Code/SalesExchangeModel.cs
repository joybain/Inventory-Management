using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SalesExchangeModel
/// </summary>
public class SalesExchangeModel
{
	public SalesExchangeModel()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int companyId { get; set; }
    public string InvoiceNo { get; set; }
    public string NewInvoiceNo { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }

   
}