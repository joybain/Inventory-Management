using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ShiftmentItems
/// </summary>
public class ShiftmentItems
{
	public ShiftmentItems()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string ID { get; set; }

    public string PartyID { get; set; }

    public string ShiftmentNO { get; set; }

    public string SupplierID { get; set; }

    public string ItemsID { get; set; }

    public string Quantity { get; set; }

    public string PartyRate { get; set; }

    public string Label { get; set; }

    public string Remarks { get; set; }

    public string LoginBy { get; set; }

    public string CustomerID { get; set; }

    public string SenderID { get; set; }
}