using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for clsItemTransferStock
/// </summary>
public class clsItemTransferStock
{
	public clsItemTransferStock()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string ID, BranchId, Remark, TransferDate, TransferType, Code;
    public clsItemTransferStock(DataRow dr)
    {
        if (dr["ID"].ToString() != String.Empty)
        {
            this.ID = dr["ID"].ToString();
        }
        if (dr["BranchID"].ToString() != String.Empty)
        {
            this.BranchId = dr["BranchID"].ToString();
        }
        if (dr["Remark"].ToString() != String.Empty)
        {
            this.Remark = dr["Remark"].ToString();
        }
        if (dr["TransferDate"].ToString() != String.Empty)
        {
            this.TransferDate = dr["TransferDate"].ToString();
        }
        
        if (dr["Code"].ToString() != String.Empty)
        {
            this.Code = dr["Code"].ToString();
        }
        if (dr["TransferType"].ToString() != String.Empty)
        {
            this.TransferType = dr["TransferType"].ToString();
        }
       
      
     }

    public string LoginBy { get; set; }

    public string TotalAmount { get; set; }

    public string BranchName { get; set; }

    public string StockType { get; set; }

   
    public string ChallanNo { get; set; }
    public string CartonNo { get; set; }
    public string RemarkNote { get; set; }
}