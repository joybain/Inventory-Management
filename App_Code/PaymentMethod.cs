using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for PaymentMethod
/// </summary>
public class PaymentMethod
{
	public PaymentMethod()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public PaymentMethod(DataRow dr)
    {
        if (dr["ID"].ToString() != string.Empty)
        {
            this.Id = dr["ID"].ToString();
        }
        if (dr["Name"].ToString() != string.Empty)
        {
            this.Name = dr["Name"].ToString();
        }
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public string LoginBy { get; set; }

    public string Status { get; set; }
}