using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Summary description for GlBook
/// </summary>
public class GlBook
{
    public string BookName;
    public string BookDesc;
    public string BookStatus;
    public string SeparatorType;
    public string CompanyAddress1;
    public string CompanyAddress2;
    public string RetdEarnAcc;
    public string TaxNo;
    public string Phone;
    public string Fax;
    public string Url;
    public string BankCode;
    public string CashCode;
    public string Status;
    public byte[] logo;


    public GlBook()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public GlBook(DataRow dr)
    {
        if (dr["book_name"].ToString() != String.Empty)
        {
            this.BookName = dr["book_name"].ToString();
        }
        if (dr["book_desc"].ToString() != String.Empty)
        {
            this.BookDesc = dr["book_desc"].ToString();
        }
        if (dr["book_status"].ToString() != String.Empty)
        {
            this.BookStatus = dr["book_status"].ToString();
        }
        if (dr["separator_type"].ToString() != String.Empty)
        {
            this.SeparatorType = dr["separator_type"].ToString();
        }
        if (dr["company_address1"].ToString() != String.Empty)
        {
            this.CompanyAddress1 = dr["company_address1"].ToString();
        }
        if (dr["company_address2"].ToString() != String.Empty)
        {
            this.CompanyAddress2 = dr["company_address2"].ToString();
        }
        if (dr["retd_earn_acc"].ToString() != String.Empty)
        {
            this.RetdEarnAcc = dr["retd_earn_acc"].ToString();
        }
        if (dr["tax_no"].ToString() != String.Empty)
        {
            this.TaxNo = dr["tax_no"].ToString();
        }
        if (dr["phone"].ToString() != String.Empty)
        {
            this.Phone = dr["phone"].ToString();
        }
        if (dr["fax"].ToString() != String.Empty)
        {
            this.Fax = dr["fax"].ToString();
        }
        if (dr["url"].ToString() != String.Empty)
        {
            this.Url = dr["url"].ToString();
        }
        if (dr["bank_code"].ToString() != String.Empty)
        {
            this.BankCode = dr["bank_code"].ToString();
        }
        if (dr["cash_code"].ToString() != String.Empty)
        {
            this.CashCode = dr["cash_code"].ToString();
        }
        if (dr["status"].ToString() != String.Empty)
        {
            this.Status = dr["status"].ToString();
        }
        if (dr["logo"].ToString() != String.Empty)
        {
            this.logo = (byte[])dr["logo"];
        }
    }

}
