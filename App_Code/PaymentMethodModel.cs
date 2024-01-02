using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PaymentMethodModel
/// </summary>
public class PaymentMethodModel
{
	public PaymentMethodModel()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int Id { get; set; }
    public int PaymentTypeId { get; set; }
    public string AccountName { get; set; }
    public string AccountNo { get; set; }
    public string gl_Code { get; set; }
    public string LoginBy { get; set; }
   
}