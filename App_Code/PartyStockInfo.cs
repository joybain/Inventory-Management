using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ItemPartyStockInfo
/// </summary>
public class ItemPartyStockInfo
{
	public ItemPartyStockInfo()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string ID { get; set; }

    public string PartyID { get; set; }

    public string ItemsID { get; set; }

    public string Quantity { get; set; }

    public string LoginBy { get; set; }
}