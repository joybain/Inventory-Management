using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Summary description for clsBankDtl
/// </summary>
public class clsBankDtl
{
    public string BankId;
    public string BranchId;
    public string BranchName;
    public string Addr1;
    public string Addr2;
    public string Phone;

    public clsBankDtl()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public clsBankDtl(DataRow dr)
    {
        if (dr["bank_id"].ToString() != string.Empty)
        {
            this.BankId = dr["bank_id"].ToString();
        }
        if (dr["ID"].ToString() != string.Empty)
        {
            this.ID = dr["ID"].ToString();
        }
        if (dr["addr1"].ToString() != string.Empty)
        {
            this.Addr1 = dr["addr1"].ToString();
        }
        if (dr["addr2"].ToString() != string.Empty)
        {
            this.Addr2 = dr["addr2"].ToString();
        }
        if (dr["PHONE1"].ToString() != string.Empty)
        {
            this.Phone = dr["PHONE1"].ToString();
        }
        if (dr["PHONE2"].ToString() != string.Empty)
        {
            this.Phone1 = dr["PHONE2"].ToString();
        }
        if (dr["AccountNo"].ToString() != string.Empty)
        {
            this.AccountNo = dr["AccountNo"].ToString();
        }
        if (dr["Gl_Code"].ToString() != string.Empty)
        {
            this.Gl_Code = dr["Gl_Code"].ToString();
        }

        if (dr["BRANCH_NAME"].ToString() != string.Empty)
        {
            this.BranchName = dr["BRANCH_NAME"].ToString();
        }

        if (dr["Active"].ToString() != string.Empty)
        {
            this.Status = dr["Active"].ToString();
        }

        if (dr["AccountType"].ToString() != string.Empty)
        {
            this.AccountType = dr["AccountType"].ToString();
        }

        if (dr["AccountName"].ToString() != string.Empty)
        {
            this.AccountName = dr["AccountName"].ToString();
        }
        if (dr["BankType"].ToString() != string.Empty)
        {
            this.BankType = dr["BankType"].ToString();
        }
       
    }

    public string AccountNo { get; set; }

    public string Phone1 { get; set; }

    public string Gl_Code { get; set; }

    public string LoginBy { get; set; }

    public string ID { get; set; }

    public string ShoprtName { get; set; }

    public string AccountName { get; set; }

    public string AccountType { get; set; }

    public string Status { get; set; }

    public string BankCharge { get; set; }

    public string BankType { get; set; }
}
