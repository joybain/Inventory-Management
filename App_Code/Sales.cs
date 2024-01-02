using System.Data;

/// <summary>
/// Summary description for Sales
/// </summary>
public class Sales
{
    public string ID,
        Invoice,
        Company,
        Total,
        Tax,
        Disount,
        GTotal,
        CReceive,
        CRefund,
        Date,
        PMethod,
        PMNumber,
        Customer,
        OrderStatus,
        Due,
        DvStatus,
        DvDate,
        Remarks,
        BankId,
        ChequeDate,
        ChequeAmount,
        Chk_Status,
        LocalCustomer,
        LocalCusAddress,
        LocalCusPhone,
        Note, Gl_CoaCode, ExtraAmount, CustomerName, CommonCus, BranchId;
    //error hote pare New Property
   // public string CustomerName { get; set; }
    //public string CommonCus { get; set; }
	public Sales()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public Sales(DataRow dr)
    {
        if (dr["ID"].ToString() != string.Empty)
        {
            this.ID = dr["ID"].ToString();
        }
        if (dr["CustomerName"].ToString() != string.Empty)
        {
            this.CustomerName = dr["CustomerName"].ToString();
        }
        if (dr["CommonCus"].ToString() != string.Empty)
        {
            this.CommonCus = dr["CommonCus"].ToString();
        }
        if (dr["Gl_CoaCode"].ToString() != string.Empty)
        {
            this.Gl_CoaCode = dr["Gl_CoaCode"].ToString();
        }
      
        if (dr["Note"].ToString() != string.Empty)
        {
            this.Note = dr["Note"].ToString();
        }
        if (dr["LocalCustomer"].ToString() != string.Empty)
        {
            this.LocalCustomer = dr["LocalCustomer"].ToString();
        }
        if (dr["LocalCusAddress"].ToString() != string.Empty)
        {
            this.LocalCusAddress = dr["LocalCusAddress"].ToString();
        }
        if (dr["LocalCusPhone"].ToString() != string.Empty)
        {
            this.LocalCusPhone = dr["LocalCusPhone"].ToString();
        }

        if (dr["Chk_Status"].ToString() != string.Empty)
        {
            this.Chk_Status = dr["Chk_Status"].ToString();
        }

        if (dr["CompanyId"].ToString() != string.Empty)
        {
            this.Company = dr["CompanyId"].ToString();
        }

        if (dr["InvoiceNo"].ToString() != string.Empty)
        {
            this.Invoice = dr["InvoiceNo"].ToString();
        }

        if (dr["SubTotal"].ToString() != string.Empty)
        {
            this.Total = dr["SubTotal"].ToString();
        }

        if (dr["TaxAmount"].ToString() != string.Empty)
        {
            this.Tax = dr["TaxAmount"].ToString();
        }

        if (dr["DiscountAmount"].ToString() != string.Empty)
        {
            this.Disount = dr["DiscountAmount"].ToString();
        }

        if (dr["GrandTotal"].ToString() != string.Empty)
        {
            this.GTotal = dr["GrandTotal"].ToString();
        }

        if (dr["CashReceived"].ToString() != string.Empty)
        {
            this.CReceive = dr["CashReceived"].ToString();
        }
        if (dr["ExtraAmount"].ToString() != string.Empty)
        {
            this.ExtraAmount = dr["ExtraAmount"].ToString();
        }


        if (dr["CashRefund"].ToString() != string.Empty)
        {
            this.CRefund = dr["CashRefund"].ToString();
        }

        if (dr["OrderDate"].ToString() != string.Empty)
        {
            this.Date = dr["OrderDate"].ToString();
        }

        if (dr["PaymentMethodID"].ToString() != string.Empty)
        {
            this.PMethod = dr["PaymentMethodID"].ToString();
        }

        if (dr["PaymentMethodNumber"].ToString() != string.Empty)
        {
            this.PMNumber = dr["PaymentMethodNumber"].ToString();
        }

        if (dr["BankId"].ToString() != string.Empty)
        {
            this.BankId = dr["BankId"].ToString();
        }

        if (dr["ChequeDate"].ToString() != string.Empty)
        {
            this.ChequeDate = dr["ChequeDate"].ToString();
        }

        if (dr["ChequeAmount"].ToString() != string.Empty)
        {
            this.ChequeAmount = dr["ChequeAmount"].ToString();
        }

        if (dr["CustomerID"].ToString() != string.Empty)
        {
            this.Customer = dr["CustomerID"].ToString();
        }

        if (dr["OrderStatusID"].ToString() != string.Empty)
        {
            this.OrderStatus = dr["OrderStatusID"].ToString();
        }

        if (dr["Due"].ToString() != string.Empty)
        {
            this.Due = dr["Due"].ToString();
        }

        if (dr["TotalInstallment"].ToString() != string.Empty)
        {
            this.TotalInstallment = dr["TotalInstallment"].ToString();
        }

        if (dr["Installmentdate"].ToString() != string.Empty)
        {
            this.installmentDate = dr["Installmentdate"].ToString();
        }

        if (dr["guarantorID"].ToString() != string.Empty)
        {
            this.GuarantorID = dr["guarantorID"].ToString();
        }

        if (dr["OrderType"].ToString() != string.Empty)
        {
            this.OrderType = dr["OrderType"].ToString();
        }


        if (dr["DeliveryStatus"].ToString() != string.Empty)
        {
            this.DvStatus = dr["DeliveryStatus"].ToString();
        }
        if (dr["BranchId"].ToString() != string.Empty)
        {
            this.DvStatus = dr["BranchId"].ToString();
        }

        if (dr["DeliveryDate"].ToString() != string.Empty)
        {
            this.DvDate = dr["DeliveryDate"].ToString();
        }

        if (dr["Remark"].ToString() != string.Empty)
        {
            this.Remarks = dr["Remark"].ToString();
        }
    }

    public string LoginBy { get; set; }

    public string GuarantorID { get; set; }

    public string installmentDate { get; set; }

    public string TotalInstallment { get; set; }

    public string OrderType { get; set; }

    public string BankName { get; set; }    

   

    public string CustomerCoa { get; set; }

    public string BankCoaCode { get; set; }
    

    public decimal ExchangeAmount { get; set; }

    public string paymentID { get; set; }
}