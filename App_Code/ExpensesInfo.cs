using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ExpensesInfo
/// </summary>
public class ExpensesInfo
{
	public ExpensesInfo()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string ID { get; set; }

    public string Code { get; set; }

    public string Date { get; set; }

    public string Remarks { get; set; }

    public string LoginBy { get; set; }
}