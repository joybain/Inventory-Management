using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DailyStockPriceChange
/// </summary>
public class DailyStockPriceChange
{
	public DailyStockPriceChange()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string ItemStockId { get; set; }
    public string ItemID { get; set; }
    public decimal ClosingStock { get; set; }
    public decimal ClosingAmount { get; set; }
    public string ExpireDate { get; set; }
    public decimal ItemsPrice { get; set; }
    public decimal SalesClosingQty { get; set; }
    public decimal CostPrice { get; set; }
    public string ItemCode { get; set; }
    public string GRN_ID { get; set; }
    public string Barcode { get; set; }
    public string LoginBy { get; set; }
}