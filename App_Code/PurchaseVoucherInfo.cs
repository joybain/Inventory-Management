using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for PurchaseVoucherInfo
/// </summary>
public class PurchaseVoucherInfo
{
    public string ID,
        GoodsReceiveNo,
        GoodsReceiveDate,
        PurchaseOrderNo,
        PurchaseOrderDate,
        ChallanNo,
        ChallanDate,
        Supplier,
        Remarks,
        TotalAmount,
        TotalPayment,
        DiscountAmt,
        CarriagePerson,
        CarriageCharge,
        LaburePerson,
        LabureCharge,
        OtherCharge,
        PaymentMethord,
        BankId,
        ChequeNo,
        ChequeDate,
        ChequeAmount,
        Due,
        ShiftmentID,
        ShiftmentNO,
        ChkStatus,
        PartyID,
        AdvancePayFlag,
        PvType,
        PvOrder;
	public PurchaseVoucherInfo()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public PurchaseVoucherInfo(DataRow dr)
    {
        if (dr["ID"].ToString() != string.Empty)
        {
            this.ID = dr["ID"].ToString();
        }
        if (dr["GRN"].ToString() != string.Empty)
        {
            this.GoodsReceiveNo = dr["GRN"].ToString();
        }
        if (dr["ReceivedDate"].ToString() != string.Empty)
        {
            this.GoodsReceiveDate = dr["ReceivedDate"].ToString();
        }
        if (dr["PO"].ToString() != string.Empty)
        {
            this.PurchaseOrderNo = dr["PO"].ToString();
        }
        if (dr["PODate"].ToString() != string.Empty)
        {
            this.PurchaseOrderDate = dr["PODate"].ToString();
        }
        if (dr["ChallanNo"].ToString() != string.Empty)
        {
            this.ChallanNo = dr["ChallanNo"].ToString();
        }
        if (dr["ChallanDate"].ToString() != string.Empty)
        {
            this.ChallanDate = dr["ChallanDate"].ToString();
        }
        if (dr["SupplierID"].ToString() != string.Empty)
        {
            this.Supplier = dr["SupplierID"].ToString();
        }
        if (dr["Remarks"].ToString() != string.Empty)
        {
            this.Remarks = dr["Remarks"].ToString();
        }
        if (dr["Total"].ToString() != string.Empty)
        {
            this.TotalAmount = dr["Total"].ToString();
        }
        if (dr["OtherCharge"].ToString() != string.Empty)
        {
            this.OtherCharge = dr["OtherCharge"].ToString();
        }
        if (dr["CarriageCharge"].ToString() != string.Empty)
        {
            this.CarriageCharge = dr["CarriageCharge"].ToString();
        }
        if (dr["LabureCharge"].ToString() != string.Empty)
        {
            this.LabureCharge = dr["LabureCharge"].ToString();
        }
        if (dr["TotalPayment"].ToString() != string.Empty)
        {
            this.TotalPayment = dr["TotalPayment"].ToString();
        }

        if (dr["DiscountAmt"].ToString() != string.Empty)
        {
            this.DiscountAmt = dr["DiscountAmt"].ToString();
        }

        
        if (dr["LaburePerson"].ToString() != string.Empty)
        {
            this.LaburePerson = dr["LaburePerson"].ToString();
        }
        if (dr["CarriagePerson"].ToString() != string.Empty)
        {
            this.CarriagePerson = dr["CarriagePerson"].ToString();
        }
        if (dr["PaymentMethod"].ToString() != string.Empty)
        {
            this.PaymentMethord = dr["PaymentMethod"].ToString();
        }
        if (dr["BankName"].ToString() != string.Empty)
        {
            this.BankId = dr["BankName"].ToString();
        }
        if (dr["ChequeNo"].ToString() != string.Empty)
        {
            this.ChequeNo = dr["ChequeNo"].ToString();
        }
        if (dr["ChequeDate"].ToString() != string.Empty)
        {
            this.ChequeDate = dr["ChequeDate"].ToString();
        }
        if (dr["ChequeAmount"].ToString() != string.Empty)
        {
            this.ChequeAmount = dr["ChequeAmount"].ToString();
        }
        if (dr["Due"].ToString() != string.Empty)
        {
            this.Due = dr["Due"].ToString();
        }
        if (dr["ShiftmentID"].ToString() != string.Empty)
        {
            this.ShiftmentID = dr["ShiftmentID"].ToString();
        }
        if (dr["Chk_Status"].ToString() != string.Empty)
        {
            this.ChkStatus = dr["Chk_Status"].ToString();
        }
        if (dr["PartyID"].ToString() != string.Empty)
        {
            this.PartyID = dr["PartyID"].ToString();
        }
        if (dr["ShiftmentNO"].ToString() != string.Empty)
        {
            this.ShiftmentNO = dr["ShiftmentNO"].ToString();
        }
        if (dr["AdvancePayFlag"].ToString() != string.Empty)
        {
            this.AdvancePayFlag = dr["AdvancePayFlag"].ToString();
        }
        if (dr["PvType"].ToString() != string.Empty)
        {
            this.PvType = dr["PvType"].ToString();
        }
        if (dr["PvOrder"].ToString() != string.Empty)
        {
            this.PvOrder = dr["PvOrder"].ToString();
        }
    }

    public string LoginBy { get; set; }

    public string SupplierName { get; set; }

    public string CashCode { get; set; }

    public string BankName { get; set; }

    public string BankCoaCode { get; set; }
}