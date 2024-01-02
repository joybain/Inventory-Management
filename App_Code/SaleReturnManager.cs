using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Delve;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for SaleReturnManager
/// </summary>
public class SaleReturnManager
{
	public SaleReturnManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static System.Data.DataTable GetShowSLMasterInfo(string IV)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID] ,t1.InvoiceNo ,t1.CustomerID,t2.ContactName ,t2.Gl_CoaCode
      FROM [Order] t1 Left join Customer t2 on t2.ID=t1.CustomerID WHERE t1.InvoiceNo='" + IV + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }


    public static System.Data.DataTable GetBranchShowSLMasterInfo(string IV,string BranchId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID] ,t1.InvoiceNo ,t1.CustomerID,t2.ContactName ,t2.Gl_CoaCode
      FROM [Order] t1 Left join Customer t2 on t2.ID=t1.CustomerID and t1.BranchId=t2.BranchId WHERE t1.InvoiceNo='" + IV + "' and t1.BranchId='"+BranchId+"'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }



    public static DataTable GetSalesReturnItems(string ID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT  t1.[ItemID],t2.Code+' - '+t2.Name AS Items_Name FROM OrderDetail t1 inner join [dbo].[ItemStock] tt on tt.ID=t1.ItemID inner join Item t2 on t2.ID=tt.ItemID where t1.OrderID='" + ID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }




    public static DataTable GetSalesReturnItems(string ID,string BranchId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT  t1.[ItemID],t2.Code+' - '+t2.Name AS Items_Name FROM OrderDetail t1 inner join dbo.OutLetItemStock tt on tt.ID=t1.ItemID inner join Item t2 on t2.ID=tt.ItemID   where t1.OrderID='" + ID + "' and t1.BranchId='"+BranchId+"' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }





    public static DataTable GetIVItems(string ItemsID, string IV)
    {
        String connectionString = DataManager.OraConnString();

//        string query = @"SELECT t1.[ID]    
//          ,'' AS item_code   
//          ,t1.[ItemID]  AS item_desc   
//          ,CONVERT(DECIMAL(18,2),t1.[UnitPrice])  AS item_rate
//          ,CONVERT(DECIMAL(18,1),ISNULL(t1.Quantity,0)-ISNULL(t1.ReturnQuantity,0)) AS salqnty 
//          ,'' AS qnty     
//          ,''  AS msr_unit_code               
//          ,'' AS [Type]
//          ,t2.ItemID AS ItemsID
//         ,t1.[CostPrice] AS PvUnitPrice
//		 ,(isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100  AS FixdiscountAmt,
//		  		  (isnull(t3.DiscountAmount/(select COUNT(*) from OrderDetail where OrderID='"+IV+"'),0) /  CONVERT(DECIMAL(18,1),ISNULL(t1.Quantity,0)-ISNULL(t1.ReturnQuantity,0)))   +   ((isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100) AS discountAmt" +
//                       ",(( CONVERT(DECIMAL(18,2),t1.[UnitPrice])-(isnull(t3.DiscountAmount/(select COUNT(*) from OrderDetail where OrderID='"+IV+"'),0) /  CONVERT(DECIMAL(18,1),ISNULL(t1.Quantity,0)-ISNULL(t1.ReturnQuantity,0)))   +   ((isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100))* (isnull(t3.TaxAmount,0)))/100 as TaxAmount FROM OrderDetail t1 inner join [Order] t3 on t3.ID=t1.OrderID    inner join dbo.[ItemStock] t2 on t2.ID=t1.[ItemID]  inner join Item t4 on t2.ItemId=t4.Id where t1.OrderID='" +
//                   IV + "' and  t1.[ItemID]='" + ItemsID + "' ";

        string query = @"SELECT t1.[ID]    
          ,'' AS item_code   
          ,t1.[ItemID]  AS item_desc   
          ,CONVERT(DECIMAL(18,2),t1.[UnitPrice])  AS item_rate
          ,CONVERT(DECIMAL(18,1),ISNULL(t1.Quantity,0)-ISNULL(t1.ReturnQuantity,0)) AS salqnty 
          ,'' AS qnty     
          ,''  AS msr_unit_code               
          ,'' AS [Type]
          ,t2.ItemID AS ItemsID
         ,t1.[CostPrice] AS PvUnitPrice
		 ,(isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100  AS FixdiscountAmt,
		  		 ((isnull(t3.DiscountAmount,0)*isnull(t1.UnitPrice,0))/isnull(t3.SubTotal,0)+((isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100)) AS discountAmt" +
                      ",((isnull(t3.DiscountAmount,0)*isnull(t1.UnitPrice,0))/isnull(t3.SubTotal,0)+((isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100)) as discountAmtfix ,(((isnull(t1.UnitPrice,0)-((isnull(t3.DiscountAmount,0)*isnull(t1.UnitPrice,0))/isnull(t3.SubTotal,0)+((isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100)))*isnull(t3.TaxAmount,0))/100) as TaxAmount,(((isnull(t1.UnitPrice,0)-((isnull(t3.DiscountAmount,0)*isnull(t1.UnitPrice,0))/isnull(t3.SubTotal,0)+((isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100)))*isnull(t3.TaxAmount,0))/100) as TaxAmountfix FROM OrderDetail t1 inner join [Order] t3 on t3.ID=t1.OrderID    inner join dbo.[ItemStock] t2 on t2.ID=t1.[ItemID]  inner join Item t4 on t2.ItemId=t4.Id where t1.OrderID='" +
                  IV + "' and  t1.[ItemID]='" + ItemsID + "' ";

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }



    public static DataTable GetBranchIVItems(string ItemsID, string IV,string BranchId)
    {
        String connectionString = DataManager.OraConnString();
        //string query = @"SELECT t2.Barcode,t1.[ID] ,'' AS item_code ,t1.[ItemID]  AS item_desc ,CONVERT(DECIMAL(18,2),t1.[UnitPrice])  AS item_rate ,CONVERT(DECIMAL(18,1),ISNULL(t1.Quantity,0)-ISNULL(t1.ReturnQuantity,0)) AS salqnty ,'' AS qnty ,''  AS msr_unit_code ,'' AS [Type] ,t2.ItemID AS ItemsID ,t1.[CostPrice] AS PvUnitPrice ,isnull(t3.DiscountAmount/(select COUNT(*) from OrderDetail where  OrderID='" + IV + "' and BranchId='" + BranchId + "'),0) AS FixdiscountAmt,isnull(t3.DiscountAmount/(select COUNT(*) from OrderDetail where OrderID='" + IV + "' and BranchId='" + BranchId + "' ),0) AS discountAmt FROM OrderDetail t1 inner join [Order] t3 on t3.ID=t1.OrderID inner join dbo.OutLetItemStock t2 on t2.ID=t1.[ItemID] where  t1.OrderID='" + IV + "' and  t1.[ItemID]='" + ItemsID + "'  and t1.BranchId='" + BranchId + "' ";
//        string query = @"SELECT t1.[ID] ,t2.Barcode  as Barcode  
//          ,'' AS item_code   
//          ,t1.[ItemID]  AS item_desc   
//          ,CONVERT(DECIMAL(18,2),t1.[UnitPrice])  AS item_rate
//          ,CONVERT(DECIMAL(18,1),ISNULL(t1.Quantity,0)-ISNULL(t1.ReturnQuantity,0)) AS salqnty 
//          ,'' AS qnty     
//          ,''  AS msr_unit_code               
//          ,'' AS [Type]
//          ,t2.ItemID AS ItemsID
//         ,t1.[CostPrice] AS PvUnitPrice
//		 ,(isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100  AS FixdiscountAmt,
//		  		  (isnull(t3.DiscountAmount/(select COUNT(*) from OrderDetail where OrderID='" + IV + "'),0) /  CONVERT(DECIMAL(18,1),ISNULL(t1.Quantity,0)-ISNULL(t1.ReturnQuantity,0)))   +   ((isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100) AS discountAmt,(( CONVERT(DECIMAL(18,2),t1.[UnitPrice])-(isnull(t3.DiscountAmount/(select COUNT(*) from OrderDetail where OrderID='"+IV+"'),0) /  CONVERT(DECIMAL(18,1),ISNULL(t1.Quantity,0)-ISNULL(t1.ReturnQuantity,0)))   +   ((isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100))* (isnull(t3.TaxAmount,0)))/100 as TaxAmount FROM OrderDetail t1 inner join [Order] t3 on t3.ID=t1.OrderID    inner join dbo.[OutLetItemStock] t2 on t2.ID=t1.[ItemID]  inner join Item t4 on t2.ItemId=t4.Id where t1.OrderID='" +
//                   IV + "' and  t1.[ItemID]='" + ItemsID + "' and t1.BranchId='" + BranchId + "' ";
        string query = @"SELECT t1.[ID] ,t2.Barcode  as Barcode  
          ,'' AS item_code   
          ,t1.[ItemID]  AS item_desc   
          ,CONVERT(DECIMAL(18,2),t1.[UnitPrice])  AS item_rate
          ,CONVERT(DECIMAL(18,1),ISNULL(t1.Quantity,0)-ISNULL(t1.ReturnQuantity,0)) AS salqnty 
          ,'' AS qnty     
          ,''  AS msr_unit_code               
          ,'' AS [Type]
          ,t2.ItemID AS ItemsID
         ,t1.[CostPrice] AS PvUnitPrice
		 ,(isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100  AS FixdiscountAmt,
		  		  ((isnull(t3.DiscountAmount,0)*isnull(t1.UnitPrice,0))/isnull(t3.SubTotal,0)+((isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100)) AS discountAmt, ((isnull(t3.DiscountAmount,0)*isnull(t1.UnitPrice,0))/isnull(t3.SubTotal,0)+((isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100)) AS discountAmtfix,(((isnull(t1.UnitPrice,0)-((isnull(t3.DiscountAmount,0)*isnull(t1.UnitPrice,0))/isnull(t3.SubTotal,0)+((isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100)))*isnull(t3.TaxAmount,0))/100) as TaxAmount,(((isnull(t1.UnitPrice,0)-((isnull(t3.DiscountAmount,0)*isnull(t1.UnitPrice,0))/isnull(t3.SubTotal,0)+((isnull(t4. DiscountAmount,0)*CONVERT(DECIMAL(18,2),t1.[UnitPrice]))/100)))*isnull(t3.TaxAmount,0))/100) as TaxAmountfix FROM OrderDetail t1 inner join [Order] t3 on t3.ID=t1.OrderID    inner join dbo.[OutLetItemStock] t2 on t2.ID=t1.[ItemID]  inner join Item t4 on t2.ItemId=t4.Id where t1.OrderID='" +
               IV + "' and  t1.[ItemID]='" + ItemsID + "' and t1.BranchId='" + BranchId + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }



    public static SaleReturn getShowRetirnItems(string p)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID],t1.InvoiceNo,CONVERT(nvarchar,t1.[ReturnDate],103) AS ReturnDate,t1.[Remarks],t1.[Return_No],t1.TotalAmount,CASE WHEN t2.PayMethod IS NULL THEN 'C' ELSE t2.PayMethod END AS [PaymentMethod],t2.Bank_id AS[BankName],t2.[ChequeNo],CONVERT(NVARCHAR,t2.[ChequeDate],103) as ChequeDate ,t2.Chk_Status,ISNULL(t1.Pay_Amount,0) AS Pay_Amount,'' AS BranchID FROM [OrderReturn] t1 left join CustomerPaymentReceive t2 on t2.Invoice=t1.ID and t2.Payment_Type='IV' Where t1.[ID]='" + p + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OtMaster");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new SaleReturn(dt.Rows[0]);
    }
   

    public static SaleReturn getBranchShowRetirnItems(string p,string BranchId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID],t1.InvoiceNo,CONVERT(nvarchar,t1.[ReturnDate],103) AS ReturnDate,t1.[Remarks],t1.[Return_No],t1.TotalAmount,CASE WHEN t2.PayMethod IS NULL THEN 'C' ELSE t2.PayMethod END AS [PaymentMethod],t2.Bank_id AS[BankName],t2.[ChequeNo],CONVERT(NVARCHAR,t2.[ChequeDate],103) as ChequeDate ,t2.Chk_Status,ISNULL(t1.Pay_Amount,0) AS Pay_Amount,t1.BranchID AS BranchID FROM [OrderReturn] t1 left join CustomerPaymentReceive t2 on t2.Invoice=t1.ID and t1.BranchId=t2.BranchId and t2.Payment_Type='IV' Where t1.[ID]='" + p + "'and t1.BranchID='" + BranchId + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OtMaster");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new SaleReturn(dt.Rows[0]);
    }

    public static int SaveInvoiceReturn(SaleReturn rtn, DataTable dt, double ReturnBlance, VouchMst _aVouchMst, VouchMst _aVouchMstDV, string CustomerCoa)
    {
        decimal totVat = 0;
        string PurchaseMstID = string.Empty;
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"INSERT INTO [OrderReturn]
           (BranchId,[InvoiceNo],[ReturnDate],[Remarks],[CreatedBy],[CreatedDate],[Return_No],[TotalAmount],[Pay_Amount])
     VALUES
           ('"+rtn.BranchID+"','" + rtn.GRN + "',convert(date,'" + rtn.ReturnDate + "',103),'" + rtn.Remarks + "','" + rtn.LogonBy +
                                  "',GETDATE(),'" + rtn.Return_No + "','" + rtn.TotalAmount.Replace(",", "") + "','" +
                                  rtn.Pay_Amount.Replace(",", "") +
                                  "')";
            command.ExecuteNonQuery();

            command.CommandText = @"SELECT top(1) [ID]  FROM [OrderReturn] where BranchId='"+rtn.BranchID+"' order by ID desc";
            PurchaseMstID = command.ExecuteScalar().ToString();

            //***************************  ********************************// 
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["item_desc"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [OrderReturnDetail]
                    (BranchId,[OrderReturnMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],discountAmt,FixdiscountAmt,VatAmt)  
                    VALUES ('"+rtn.BranchID+"','" + PurchaseMstID + "','" + dr["item_desc"].ToString() + "','" +
                                          dr["item_rate"].ToString().Replace(",", "") +
                                          "','" + dr["qnty"].ToString() + "','" +
                                          Convert.ToDouble(dr["item_rate"]) * Convert.ToDouble(dr["qnty"]) + "','" +
                                          rtn.LogonBy + "',GETDATE(),'" +
                                          dr["discountAmt"].ToString().Replace(",", "") + "','" +
                                          dr["FixdiscountAmt"].ToString().Replace(",", "") + "','" + dr["TaxAmount"].ToString().Replace(",", "") + "')";
                    command.ExecuteNonQuery();


                    command.CommandText = @"UPDATE [ItemStock]
                         SET [SalesClosingQty] =ISNULL([SalesClosingQty],0)-'" + dr["qnty"].ToString() +
                                          "',[ClosingStock] =ISNULL([ClosingStock],0)+'" + dr["qnty"].ToString() +
                                          "' WHERE ID='" +
                                          dr["item_desc"].ToString() + "'";
                   command.ExecuteNonQuery();
                   totVat += Convert.ToDecimal(dr["TaxAmount"].ToString());

                }
            }

            if (Convert.ToDouble(rtn.Due) > 0)
            {
                
                    command.CommandText = @"INSERT INTO [dbo].[CustomerPaymentReceive]
                           (BranchId,[Date],[Customer_id],[ReturnID],[PayAmt],[entry_by],[entry_date],PayType)
                     VALUES
                           ('"+rtn.BranchID+"',convert(datetime, nullif( '" + rtn.ReturnDate + "',''), 103),'" + rtn.CustomerID + "','" +
                                          PurchaseMstID +
                                          "','" + rtn.Due.Replace(",","") + "','" + rtn.LogonBy + "',GETDATE(),'"+rtn.PaymentMethod+"')";
                    command.ExecuteNonQuery();
                
            }

            //***************************  Jurnal Voucher ********************************// 
            command.CommandText = VouchManager.SaveVoucherMst(_aVouchMst, 1);
            command.ExecuteNonQuery();

            VouchDtl vdtl;
            for (int j = 0; j < 4; j++)
            {
                if (j == 0)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "1";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["PHSalesReturn"].ToString(); //**** AdditionalCharge Code *******//
                    vdtl.Particulars = "PH Main office sales return";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["PHSalesReturn"].ToString()); //**** AdditionalCharge Code *******//
                    vdtl.AmountDr = rtn.TotalAmount.Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 1)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "2";
                    vdtl.GlCoaCode = "1-" + CustomerCoa;
                    vdtl.Particulars = rtn.SupplierName;
                    vdtl.AccType = VouchManager.getAccType("1-" + CustomerCoa);
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = rtn.TotalAmount.Replace(",", "");
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 2)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "3";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
                    vdtl.Particulars = "Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
                    vdtl.AmountDr = ReturnBlance.ToString().Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 3)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "4";
                    vdtl.GlCoaCode = "1-" + dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
                    vdtl.AccType =
                            VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
                                .ToString()); //**** Purchase Code *******//
                    vdtl.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
                    vdtl.AmountDr =  totVat.ToString().Replace("'", "").Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    vdtl.AUTHO_USER = _aVouchMst.EntryUser;
                    //*********** Convert Rate ********//
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
            }

            //******** Devied Voucher ********************************// 

            if (!string.IsNullOrEmpty(rtn.Pay_Amount))
            {
                if (Convert.ToDecimal(rtn.Pay_Amount) > 0)
                {
                    _aVouchMstDV.ControlAmt = rtn.Pay_Amount.Replace("'", "").Replace(",", "");
                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstDV, 1);
                    command.ExecuteNonQuery();
                    VouchDtl vdtlCR;
                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                        {
                            //DataRow 
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = rtn.ReturnDate;
                            vdtlCR.LineNo = "1";
                            vdtlCR.GlCoaCode = "1-" + CustomerCoa;
                            vdtlCR.Particulars = rtn.SupplierName;
                            vdtlCR.AccType = VouchManager.getAccType("1-" + CustomerCoa);
                            vdtlCR.AmountDr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.AmountCr = "0";
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName; //*********** Convert Rate ********//

                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            BankAndCashBlanceCheck.GetBanlanceConvertion(vdtlCR, vdtlCR.AmountDr, vdtlCR.AmountCr);
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
                        else if (j == 1)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = rtn.ReturnDate;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(rtn.BankCoaCode))
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {

                                vdtlCR.GlCoaCode = "1-" + rtn.BankCoaCode; //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + rtn.BankCoaCode); //**** SalesCode *******//
                                vdtlCR.Particulars = rtn.bankNameDes;
                            }

                            vdtlCR.AmountDr = "0";
                            vdtlCR.AmountCr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            //*********** Convert Rate ********//
                            BankAndCashBlanceCheck.GetBanlanceConvertion(vdtlCR, vdtlCR.AmountDr, vdtlCR.AmountCr);
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
                    }
                }
            }
            command.CommandText = @"UPDATE [FixGlCoaCode]
                                SET [ReturnFixID] = [ReturnFixID]+1 ";
            command.ExecuteNonQuery();

            transaction.Commit();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        if (string.IsNullOrEmpty(PurchaseMstID))
        {
            return 0;
        }
        return Convert.ToInt32(PurchaseMstID);
    }



















    public static int BranchSaveInvoiceReturn(SaleReturn rtn, DataTable dt, double ReturnBlance, VouchMst _aVouchMst, VouchMst _aVouchMstDV, string CustomerCoa)
    {
        decimal totVat = 0;
        string PurchaseMstID = string.Empty;
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;


      
            command.CommandText = @"INSERT INTO [OrderReturn]
           (BranchId,[InvoiceNo],[ReturnDate],[Remarks],[CreatedBy],[CreatedDate],[Return_No],[TotalAmount],[Pay_Amount])
     VALUES
           ('"+rtn.BranchID+"','" + rtn.GRN + "',convert(date,'" + rtn.ReturnDate + "',103),'" + rtn.Remarks + "','" + rtn.LogonBy +
                                  "',GETDATE(),'" + rtn.Return_No + "','" + rtn.TotalAmount.Replace(",", "") + "','" +
                                  rtn.Pay_Amount.Replace(",", "") +
                                  "')";
            command.ExecuteNonQuery();

            command.CommandText = @"SELECT top(1) [ID]  FROM [OrderReturn] order by ID desc";
            PurchaseMstID = command.ExecuteScalar().ToString();

            //***************************  ********************************// 
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["item_desc"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [OrderReturnDetail]
                    (BranchId,[OrderReturnMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],discountAmt,FixdiscountAmt,VatAmt)  
                    VALUES ('"+rtn.BranchID+"','" + PurchaseMstID + "','" + dr["item_desc"].ToString() + "','" +
                                          dr["item_rate"].ToString().Replace(",", "") +
                                          "','" + dr["qnty"].ToString() + "','" +
                                          Convert.ToDouble(dr["item_rate"]) * Convert.ToDouble(dr["qnty"]) + "','" +
                                          rtn.LogonBy + "',GETDATE(),'" +
                                          dr["discountAmt"].ToString().Replace(",", "") + "','" +
                                          dr["FixdiscountAmt"].ToString().Replace(",", "") + "','" + dr["TaxAmount"].ToString().Replace(",", "") + "')";
                    command.ExecuteNonQuery();


                    command.CommandText = @"UPDATE [OutLetItemStock]
                         SET [SalesClosingQty] =ISNULL([SalesClosingQty],0)-'" + dr["qnty"].ToString() +
                                          "',[ClosingStock] =ISNULL([ClosingStock],0)+'" + dr["qnty"].ToString() +
                                          "' WHERE ID='" +
                                          dr["item_desc"].ToString() + "' and Barcode='" + dr["Barcode"].ToString() + "'";
                    command.ExecuteNonQuery();
                    totVat += Convert.ToDecimal(dr["TaxAmount"].ToString().Replace(",", ""));

                }
            }

            if (Convert.ToDouble(rtn.Due) > 0)
            {

                command.CommandText = @"INSERT INTO [dbo].[CustomerPaymentReceive]
                           ([Date],[Customer_id],[ReturnID],[PayAmt],[entry_by],[entry_date],BranchId,PayType)
                     VALUES
                           (convert(datetime, nullif( '" + rtn.ReturnDate + "',''), 103),'" + rtn.CustomerID + "','" +
                                      PurchaseMstID +
                                      "','" + rtn.Due.Replace(",", "") + "','" + rtn.LogonBy + "',GETDATE(),'"+rtn.BranchID+"','"+rtn.PaymentMethod+"')";
                command.ExecuteNonQuery();

            }

            //***************************  Jurnal Voucher ********************************// 
            command.CommandText = VouchManager.SaveVoucherMst(_aVouchMst, 1);
            command.ExecuteNonQuery();

            VouchDtl vdtl;
            for (int j = 0; j < 4; j++)
            {
                if (j == 0)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "1";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["PHSalesReturn"].ToString(); //**** AdditionalCharge Code *******//
                    vdtl.Particulars = "PH Main office sales return";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["PHSalesReturn"].ToString()); //**** AdditionalCharge Code *******//
                    vdtl.AmountDr = rtn.TotalAmount.Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 1)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "2";
                    vdtl.GlCoaCode = "1-" + CustomerCoa;
                    vdtl.Particulars = rtn.SupplierName;
                    vdtl.AccType = VouchManager.getAccType("1-" + CustomerCoa);
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = rtn.TotalAmount.Replace(",", "");
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 2)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "3";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
                    vdtl.Particulars = "Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
                    vdtl.AmountDr = ReturnBlance.ToString().Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 3)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "4";
                    vdtl.GlCoaCode = "1-" + dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
                    vdtl.AccType =
                            VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
                                .ToString()); //**** Purchase Code *******//
                    vdtl.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
                    vdtl.AmountDr =  totVat.ToString().Replace("'", "").Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    vdtl.AUTHO_USER = _aVouchMst.EntryUser;
                    //*********** Convert Rate ********//
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
            }

            //******** Devied Voucher ********************************// 

            if (!string.IsNullOrEmpty(rtn.Pay_Amount))
            {
                if (Convert.ToDecimal(rtn.Pay_Amount) > 0)
                {
                    _aVouchMstDV.ControlAmt = rtn.Pay_Amount.Replace("'", "").Replace(",", "");
                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstDV, 1);
                    command.ExecuteNonQuery();
                    VouchDtl vdtlCR;
                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                        {
                            //DataRow 
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = rtn.ReturnDate;
                            vdtlCR.LineNo = "1";
                            vdtlCR.GlCoaCode = "1-" + CustomerCoa;
                            vdtlCR.Particulars = rtn.SupplierName;
                            vdtlCR.AccType = VouchManager.getAccType("1-" + CustomerCoa);
                            vdtlCR.AmountDr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.AmountCr = "0";
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName; //*********** Convert Rate ********//

                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            BankAndCashBlanceCheck.GetBanlanceConvertion(vdtlCR, vdtlCR.AmountDr, vdtlCR.AmountCr);
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
                        else if (j == 1)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = rtn.ReturnDate;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(rtn.BankCoaCode))
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {

                                vdtlCR.GlCoaCode = "1-" + rtn.BankCoaCode; //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + rtn.BankCoaCode); //**** SalesCode *******//
                                vdtlCR.Particulars = rtn.bankNameDes;
                            }

                            vdtlCR.AmountDr = "0";
                            vdtlCR.AmountCr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            //*********** Convert Rate ********//
                            BankAndCashBlanceCheck.GetBanlanceConvertion(vdtlCR, vdtlCR.AmountDr, vdtlCR.AmountCr);
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
                    }
                }
            }
            command.CommandText = @"UPDATE [FixGlCoaCode]
                                SET [ReturnFixID] = [ReturnFixID]+1 ";
            command.ExecuteNonQuery();

            transaction.Commit();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        if (string.IsNullOrEmpty(PurchaseMstID))
        {
            return 0;
        }
        return Convert.ToInt32(PurchaseMstID);
    }









    public static void UpdateSalesReturn(SaleReturn rtn, DataTable dt, DataTable dtOldList, double ReturnBlance, VouchMst _aVouchMst,VouchMst _aVouchMstCR, string CustomerCoa)
    {
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"UPDATE [OrderReturn]
            SET [ReturnDate] = convert(date,'" + rtn.ReturnDate + "',103),[Remarks] ='" + rtn.Remarks.Replace("'", "") +
                                  "' ,[ModifiedBy] ='" + rtn.LogonBy + "' ,[ModifiedDate] =GETDATE(),[TotalAmount] ='" +
                                  rtn.TotalAmount.Replace(",", "") + "',[Pay_Amount] ='" +
                                  rtn.Pay_Amount.Replace(",", "") + "' WHERE ID='" + rtn.ID + "' ";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dtOldList.Rows)
            {
                if (!string.IsNullOrEmpty(dr["item_desc"].ToString()))
                {
                    command.CommandText = @"DELETE FROM [OrderReturnDetail] WHERE OrderReturnMstID='" + rtn.ID +
                                          "' and [ItemID]='" + dr["item_desc"].ToString() + "' ";
                    command.ExecuteNonQuery();


                    command.CommandText = @"UPDATE [ItemStock]
                         SET [SalesClosingQty] =ISNULL([SalesClosingQty],0)-'" + dr["qnty"].ToString() +
                                          "',ClosingStock=ISNULL(ClosingStock,0)-'" + dr["qnty"].ToString() +
                                          "' WHERE ID='" +
                                          dr["item_desc"].ToString() + "'";
               var a=    command.ExecuteNonQuery();
                    
                }
            }
            foreach (DataRow dr in dt.Rows)
            {
                if (!string.IsNullOrEmpty(dr["item_desc"].ToString()))
                {
                    command.CommandText = @"INSERT INTO [OrderReturnDetail]
                    ([OrderReturnMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],discountAmt,FixdiscountAmt,VatAmt)  
                    VALUES ('" + rtn.ID + "','" + dr["item_desc"].ToString() + "','" + dr["item_rate"].ToString().Replace(",", "") +
                                          "','" + dr["qnty"].ToString() + "','" +
                                          Convert.ToDouble(dr["item_rate"]) * Convert.ToDouble(dr["qnty"]) + "','" +
                                          rtn.LogonBy + "',GETDATE(),'" + dr["discountAmt"].ToString() + "','" +
                                          dr["FixdiscountAmt"].ToString() + "','" + dr["TaxAmount"].ToString() + "')";
                    command.ExecuteNonQuery();

                    command.CommandText = @"UPDATE [ItemStock]
                         SET [SalesClosingQty] =ISNULL([SalesClosingQty],0)+'" + dr["qnty"].ToString() +
                                          "',ClosingStock=ISNULL(ClosingStock,0)+'" + dr["qnty"].ToString() +
                                          "' WHERE ID='" +
                                          dr["item_desc"].ToString() + "'";
                    command.ExecuteNonQuery();
                }
            }

            if (Convert.ToDouble(rtn.Due) > 0)
            {
                command.CommandText =
                    @"select count(*) from  [CustomerPaymentReceive] where [ReturnID]='" + rtn.ID + "'";
                int CheckDues = Convert.ToInt32(command.ExecuteScalar());
                command.CommandType = CommandType.Text;

                if (CheckDues > 0)
                {
                    command.CommandText = @"UPDATE [dbo].[CustomerPaymentReceive]
                       SET [Date] = convert(date,'" + rtn.ReturnDate + "',103),[Customer_id] ='" +
                                          rtn.CustomerID + "',[PayAmt] ='" + rtn.Due.Replace(",", "") + "' ,[update_by] ='" +
                                          rtn.LogonBy + "' ,[update_date] = GETDATE(),PayType='"+rtn.PaymentMethod+"',BranchId='"+rtn.BranchID+"' WHERE [ReturnID]='" +
                                          rtn.ID + "' ";
                    command.ExecuteNonQuery();
                }
                else
                {
                    command.CommandText = @"INSERT INTO [dbo].[CustomerPaymentReceive]
                           ([Date],[Customer_id],[ReturnID],[PayAmt],[entry_by],[entry_date],PayType,BranchId)
                     VALUES
                           (convert(datetime, nullif( '" + rtn.ReturnDate + "',''), 103),'" + rtn.CustomerID + "','" +
                                          rtn.ID +
                                          "','" + rtn.Due.Replace(",", "") + "','" + rtn.LogonBy + "',GETDATE(),'"+rtn.PaymentMethod+"','"+rtn.BranchID+"')";
                    command.ExecuteNonQuery();
                }
            }

            //***************************  Jurnal Voucher Update ********************************// 
            command.CommandText = VouchManager.SaveVoucherMst(_aVouchMst, 2);
            command.ExecuteNonQuery();

            command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" + _aVouchMst.VchSysNo + "')";
            command.ExecuteNonQuery();

            VouchDtl vdtl;
            for (int j = 0; j < 3; j++)
            {
                if (j == 0)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "1";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["PHSalesReturn"].ToString(); //**** PHSalesReturn *******//
                    vdtl.Particulars = "PH Main office sales return";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["PHSalesReturn"].ToString()); //**** PHSalesReturn *******//
                    vdtl.AmountDr = rtn.TotalAmount.Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 1)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "2";
                    vdtl.GlCoaCode = "1-" + CustomerCoa;
                    vdtl.Particulars = rtn.SupplierName;
                    vdtl.AccType = VouchManager.getAccType("1-" + CustomerCoa);
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = rtn.TotalAmount.Replace(",", "");
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 2)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "3";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
                    vdtl.Particulars = "Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
                    vdtl.AmountDr = ReturnBlance.ToString().Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
            }

            //*********  Debite Voucher *********// 
            if (!string.IsNullOrEmpty(rtn.Pay_Amount))
            {
                if (Convert.ToDecimal(rtn.Pay_Amount) > 0)
                {
                    if (_aVouchMstCR.RefFileNo.Equals("New"))
                    {
                        _aVouchMstCR.ControlAmt = rtn.Pay_Amount.Replace("'", "").Replace(",", "");
                        command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 1);
                        command.ExecuteNonQuery();
                        VouchDtl vdtlCR;
                        for (int j = 0; j < 2; j++)
                        {
                            if (j == 0)
                            {
                                //DataRow 
                                vdtlCR = new VouchDtl();
                                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                                vdtlCR.ValueDate = rtn.ReturnDate;
                                vdtlCR.LineNo = "1";
                                vdtlCR.GlCoaCode = "1-" + CustomerCoa;
                                vdtlCR.Particulars = rtn.SupplierName;
                                vdtlCR.AccType = VouchManager.getAccType("1-" + CustomerCoa);
                                vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
                                vdtlCR.AmountCr = "0";
                                vdtlCR.Status = _aVouchMstCR.Status;
                                vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

                                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                            }
                            else if (j == 1)
                            {
                                vdtlCR = new VouchDtl();
                                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                                vdtlCR.ValueDate = rtn.ReturnDate;
                                vdtlCR.LineNo = "2";
                                if (string.IsNullOrEmpty(rtn.BankCoaCode))
                                {
                                    vdtlCR.GlCoaCode =
                                        "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                    vdtlCR.AccType =
                                        VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                                                    .ToString()); //**** SalesCode *******//
                                    vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                                }
                                else
                                {
                                    vdtlCR.GlCoaCode = "1-" + rtn.BankCoaCode; //**** SalesCode *******//
                                    vdtlCR.AccType =
                                        VouchManager.getAccType("1-" + rtn.BankCoaCode); //**** SalesCode *******//
                                    vdtlCR.Particulars = rtn.bankNameDes;
                                }

                                vdtlCR.AmountDr = "0";
                                vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
                                vdtlCR.Status = _aVouchMstCR.Status;
                                vdtlCR.BookName = _aVouchMstCR.BookName;
                                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                                //*********** Convert Rate ********//
                                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                            }
                        }
                    }
                    else
                    {
                        _aVouchMstCR.ControlAmt = rtn.Pay_Amount.Replace("'", "").Replace(",", "");
                        command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 2);
                        command.ExecuteNonQuery();

                        command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
                                              _aVouchMstCR.VchSysNo + "')";
                        command.ExecuteNonQuery();

                        VouchDtl vdtlCR;
                        for (int j = 0; j < 3; j++)
                        {
                            if (j == 0)
                            {
                                //DataRow 
                                vdtlCR = new VouchDtl();
                                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                                vdtlCR.ValueDate = rtn.ReturnDate;
                                vdtlCR.LineNo = "1";
                                vdtlCR.GlCoaCode = "1-" + CustomerCoa;
                                vdtlCR.Particulars = rtn.SupplierName;
                                vdtlCR.AccType = VouchManager.getAccType("1-" + CustomerCoa);
                                vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
                                vdtlCR.AmountCr = "0";
                                vdtlCR.Status = _aVouchMstCR.Status;
                                vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

                                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                            }
                            else if (j == 1)
                            {
                                vdtlCR = new VouchDtl();
                                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                                vdtlCR.ValueDate = rtn.ReturnDate;
                                vdtlCR.LineNo = "2";
                                if (string.IsNullOrEmpty(rtn.BankCoaCode))
                                {
                                    vdtlCR.GlCoaCode = "1-" +
                                                       dtFixCode.Rows[0]["CashInHand_BD"]
                                                           .ToString(); //**** SalesCode *******//
                                    vdtlCR.AccType =
                                        VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                                                    .ToString()); //**** SalesCode *******//
                                    vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                                }
                                else
                                {
                                    vdtlCR.GlCoaCode = "1-" + rtn.BankCoaCode; //**** SalesCode *******//
                                    vdtlCR.AccType =
                                        VouchManager.getAccType("1-" + rtn.BankCoaCode); //**** SalesCode *******//
                                    vdtlCR.Particulars = rtn.bankNameDes;
                                }

                                vdtlCR.AmountDr = "0";
                                vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
                                vdtlCR.Status = _aVouchMstCR.Status;
                                vdtlCR.BookName = _aVouchMstCR.BookName;
                                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                                //*********** Convert Rate ********//
                                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                            }
                        }
                    }
                }
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }
    }



    public static void UpdateBranchSalesReturn(SaleReturn rtn, DataTable dt, DataTable dtOldList, double ReturnBlance, VouchMst _aVouchMst, VouchMst _aVouchMstCR, string CustomerCoa)
    {
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"UPDATE [OrderReturn]
            SET BranchId='"+rtn.BranchID+"', [ReturnDate] = convert(date,'" + rtn.ReturnDate + "',103),[Remarks] ='" + rtn.Remarks.Replace("'", "") +
                                  "' ,[ModifiedBy] ='" + rtn.LogonBy + "' ,[ModifiedDate] =GETDATE(),[TotalAmount] ='" +
                                  rtn.TotalAmount.Replace(",", "") + "',[Pay_Amount] ='" +
                                  rtn.Pay_Amount.Replace(",", "") + "' WHERE ID='" + rtn.ID + "' ";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dtOldList.Rows)
            {
                if (!string.IsNullOrEmpty(dr["item_desc"].ToString()))
                {
                    command.CommandText = @"DELETE FROM [OrderReturnDetail] WHERE OrderReturnMstID='" + rtn.ID +
                                          "' and [ItemID]='" + dr["item_desc"].ToString() + "' and BranchId='"+rtn.BranchID+"' ";
                    command.ExecuteNonQuery();


                    command.CommandText = @"UPDATE [OutLetItemStock]
                         SET [SalesClosingQty] =ISNULL([SalesClosingQty],0)-'" + dr["qnty"].ToString() +
                                          "',ClosingStock=ISNULL(ClosingStock,0)-'" + dr["qnty"].ToString() +
                                          "' WHERE ID='" +
                                          dr["item_desc"].ToString() + "' and Barcode='" + dr["Barcode"].ToString() + "'";
                    var a = command.ExecuteNonQuery();

                }
            }
            foreach (DataRow dr in dt.Rows)
            {
                if (!string.IsNullOrEmpty(dr["item_desc"].ToString()))
                {
                    command.CommandText = @"INSERT INTO [OrderReturnDetail]
                    (BranchId,[OrderReturnMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],discountAmt,FixdiscountAmt,VatAmt)  
                    VALUES ('"+rtn.BranchID+"','" + rtn.ID + "','" + dr["item_desc"].ToString() + "','" + dr["item_rate"].ToString().Replace(",", "") +
                                          "','" + dr["qnty"].ToString() + "','" +
                                          Convert.ToDouble(dr["item_rate"]) * Convert.ToDouble(dr["qnty"]) + "','" +
                                          rtn.LogonBy + "',GETDATE(),'" + dr["discountAmt"].ToString() + "','" +
                                          dr["FixdiscountAmt"].ToString() + "','" + dr["TaxAmount"].ToString() + "')";
                    command.ExecuteNonQuery();

                    command.CommandText = @"UPDATE [OutLetItemStock]
                         SET [SalesClosingQty] =ISNULL([SalesClosingQty],0)+'" + dr["qnty"].ToString() +
                                          "',ClosingStock=ISNULL(ClosingStock,0)+'" + dr["qnty"].ToString() +
                                          "' WHERE ID='" +
                                          dr["item_desc"].ToString() + "' and Barcode='" + dr["Barcode"].ToString() + "'";
                    command.ExecuteNonQuery();
                }
            }

            if (Convert.ToDouble(rtn.Due) > 0)
            {
                command.CommandText =
                    @"select count(*) from  [CustomerPaymentReceive]  where BranchId='"+rtn.BranchID+"' and  [ReturnID]='" + rtn.ID + "'";
                int CheckDues = Convert.ToInt32(command.ExecuteScalar());
                command.CommandType = CommandType.Text;

                if (CheckDues > 0)
                {
                    command.CommandText = @"UPDATE [dbo].[CustomerPaymentReceive]
                       SET [Date] = convert(date,'" + rtn.ReturnDate + "',103),[Customer_id] ='" +
                                          rtn.CustomerID + "',[PayAmt] ='" + rtn.Due.Replace(",", "") + "' ,[update_by] ='" +
                                          rtn.LogonBy + "' ,[update_date] = GETDATE(),BranchId='"+rtn.BranchID+"',PayType='"+rtn.PaymentMethod+"' WHERE BranchId='"+rtn.BranchID+"' and [ReturnID]='" +
                                          rtn.ID + "' ";
                    command.ExecuteNonQuery();
                }
                else
                {
                    command.CommandText = @"INSERT INTO [dbo].[CustomerPaymentReceive]
                           ([Date],[Customer_id],[ReturnID],[PayAmt],[entry_by],[entry_date],PayType,BranchId)
                     VALUES
                           (convert(datetime, nullif( '" + rtn.ReturnDate + "',''), 103),'" + rtn.CustomerID + "','" +
                                          rtn.ID +
                                          "','" + rtn.Due.Replace(",", "") + "','" + rtn.LogonBy + "',GETDATE(),'"+rtn.PaymentMethod+"','"+rtn.BranchID+"')";
                    command.ExecuteNonQuery();
                }
            }

            //***************************  Jurnal Voucher Update ********************************// 
            command.CommandText = VouchManager.SaveVoucherMst(_aVouchMst, 2);
            command.ExecuteNonQuery();

            command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" + _aVouchMst.VchSysNo + "')";
            command.ExecuteNonQuery();

            VouchDtl vdtl;
            for (int j = 0; j < 3; j++)
            {
                if (j == 0)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "1";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["PHSalesReturn"].ToString(); //**** PHSalesReturn *******//
                    vdtl.Particulars = "PH Main office sales return";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["PHSalesReturn"].ToString()); //**** PHSalesReturn *******//
                    vdtl.AmountDr = rtn.TotalAmount.Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 1)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "2";
                    vdtl.GlCoaCode = "1-" + CustomerCoa;
                    vdtl.Particulars = rtn.SupplierName;
                    vdtl.AccType = VouchManager.getAccType("1-" + CustomerCoa);
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = rtn.TotalAmount.Replace(",", "");
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 2)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = rtn.ReturnDate;
                    vdtl.LineNo = "3";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
                    vdtl.Particulars = "Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
                    vdtl.AmountDr = ReturnBlance.ToString().Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
            }

            //*********  Debite Voucher *********// 
            if (!string.IsNullOrEmpty(rtn.Pay_Amount))
            {
                if (Convert.ToDecimal(rtn.Pay_Amount) > 0)
                {
                    if (_aVouchMstCR.RefFileNo.Equals("New"))
                    {
                        _aVouchMstCR.ControlAmt = rtn.Pay_Amount.Replace("'", "").Replace(",", "");
                        command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 1);
                        command.ExecuteNonQuery();
                        VouchDtl vdtlCR;
                        for (int j = 0; j < 2; j++)
                        {
                            if (j == 0)
                            {
                                //DataRow 
                                vdtlCR = new VouchDtl();
                                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                                vdtlCR.ValueDate = rtn.ReturnDate;
                                vdtlCR.LineNo = "1";
                                vdtlCR.GlCoaCode = "1-" + CustomerCoa;
                                vdtlCR.Particulars = rtn.SupplierName;
                                vdtlCR.AccType = VouchManager.getAccType("1-" + CustomerCoa);
                                vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
                                vdtlCR.AmountCr = "0";
                                vdtlCR.Status = _aVouchMstCR.Status;
                                vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

                                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                            }
                            else if (j == 1)
                            {
                                vdtlCR = new VouchDtl();
                                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                                vdtlCR.ValueDate = rtn.ReturnDate;
                                vdtlCR.LineNo = "2";
                                if (string.IsNullOrEmpty(rtn.BankCoaCode))
                                {
                                    vdtlCR.GlCoaCode =
                                        "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                    vdtlCR.AccType =
                                        VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                                                    .ToString()); //**** SalesCode *******//
                                    vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                                }
                                else
                                {
                                    vdtlCR.GlCoaCode = "1-" + rtn.BankCoaCode; //**** SalesCode *******//
                                    vdtlCR.AccType =
                                        VouchManager.getAccType("1-" + rtn.BankCoaCode); //**** SalesCode *******//
                                    vdtlCR.Particulars = rtn.bankNameDes;
                                }

                                vdtlCR.AmountDr = "0";
                                vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
                                vdtlCR.Status = _aVouchMstCR.Status;
                                vdtlCR.BookName = _aVouchMstCR.BookName;
                                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                                //*********** Convert Rate ********//
                                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                            }
                        }
                    }
                    else
                    {
                        _aVouchMstCR.ControlAmt = rtn.Pay_Amount.Replace("'", "").Replace(",", "");
                        command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 2);
                        command.ExecuteNonQuery();

                        command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
                                              _aVouchMstCR.VchSysNo + "')";
                        command.ExecuteNonQuery();

                        VouchDtl vdtlCR;
                        for (int j = 0; j < 3; j++)
                        {
                            if (j == 0)
                            {
                                //DataRow 
                                vdtlCR = new VouchDtl();
                                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                                vdtlCR.ValueDate = rtn.ReturnDate;
                                vdtlCR.LineNo = "1";
                                vdtlCR.GlCoaCode = "1-" + CustomerCoa;
                                vdtlCR.Particulars = rtn.SupplierName;
                                vdtlCR.AccType = VouchManager.getAccType("1-" + CustomerCoa);
                                vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
                                vdtlCR.AmountCr = "0";
                                vdtlCR.Status = _aVouchMstCR.Status;
                                vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

                                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                            }
                            else if (j == 1)
                            {
                                vdtlCR = new VouchDtl();
                                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                                vdtlCR.ValueDate = rtn.ReturnDate;
                                vdtlCR.LineNo = "2";
                                if (string.IsNullOrEmpty(rtn.BankCoaCode))
                                {
                                    vdtlCR.GlCoaCode = "1-" +
                                                       dtFixCode.Rows[0]["CashInHand_BD"]
                                                           .ToString(); //**** SalesCode *******//
                                    vdtlCR.AccType =
                                        VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                                                    .ToString()); //**** SalesCode *******//
                                    vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                                }
                                else
                                {
                                    vdtlCR.GlCoaCode = "1-" + rtn.BankCoaCode; //**** SalesCode *******//
                                    vdtlCR.AccType =
                                        VouchManager.getAccType("1-" + rtn.BankCoaCode); //**** SalesCode *******//
                                    vdtlCR.Particulars = rtn.bankNameDes;
                                }

                                vdtlCR.AmountDr = "0";
                                vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
                                vdtlCR.Status = _aVouchMstCR.Status;
                                vdtlCR.BookName = _aVouchMstCR.BookName;
                                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                                //*********** Convert Rate ********//
                                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                            }
                        }
                    }
                }
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }
    }





    public static DataTable GetShowSalesReturnItems(string BranchID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]
      ,t2.InvoiceNo AS GRN 
      ,t1.[Return_No]
      ,CONVERT(NVARCHAR, t1.[ReturnDate],103) AS [ReturnDate]
      ,t1.[Remarks]  
      ,t3.ContactName
      ,t1.TotalAmount
  FROM dbo.OrderReturn t1 
  inner join [Order] t2 on t2.ID=t1.InvoiceNo
  inner join dbo.Customer t3 on t3.ID=t2.CustomerID
    ORDER BY t1.[ID] DESC ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OrderReturn");
        return dt;
    }




    public static DataTable GetBranchShowSalesReturnItems(string BranchID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]
      ,t2.InvoiceNo AS GRN 
      ,t1.[Return_No]
      ,CONVERT(NVARCHAR, t1.[ReturnDate],103) AS [ReturnDate]
      ,t1.[Remarks]  
      ,t3.ContactName
      ,t1.TotalAmount
  FROM dbo.OrderReturn t1 
  inner join [Order] t2 on t2.ID=t1.InvoiceNo and ISNULL(t2.BranchId,0)=ISNULL(t1.BranchId,0)
  inner join dbo.Customer t3 on t3.ID=t2.CustomerID where ISNULL(t1.BranchId,0)='"+BranchID+"' ORDER BY t1.[ID] DESC  ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OrderReturn");
        return dt;
    }
    public static DataTable ItemsDetails(string OrderReturnMstID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]  
	  ,'' AS item_code    
      ,t1.[ItemID] AS item_desc
      ,t1.[UnitPrice] AS item_rate
      ,'' AS msr_unit_code
      ,t1.[Quantity] AS qnty
      ,t1.[Total]      
      ,t3.BrandName
      ,t2.Name AS[des_name]
      ,t4.salqnty-isnull(t1.[Quantity],0) AS salqnty
      ,t4.PvUnitPrice
      ,'' AS [Type]
      ,tt.ItemID AS ItemsID
      ,t1.discountAmt
      ,t1.FixdiscountAmt,tt.Barcode
  FROM OrderReturnDetail t1 inner join dbo.OrderReturn tt1 on tt1.ID=t1.OrderReturnMstID inner join [dbo].[ItemStock] tt on tt.ID=t1.ItemID left join  Item t2 on t2.ID=tt.ItemID left join Brand t3 on t3.ID=t2.Brand
  inner join (select t1.ItemID,t2.ID,t1.[CostPrice] AS PvUnitPrice,CONVERT(DECIMAL(18,1),ISNULL(t1.Quantity,0)-ISNULL(t1.ReturnQuantity,0)) AS salqnty from dbo.OrderDetail t1 inner join [Order] t2 on t2.ID=t1.OrderID) t4 on t4.ItemID=t1.ItemID AND t4.ID=tt1.InvoiceNo   where t1.OrderReturnMstID='" + OrderReturnMstID + "' union all select NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL  ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OrderReturn");
        return dt;
    }


    public static DataTable BranchItemsDetails(string OrderReturnMstID,string BranchId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]  
	  ,'' AS item_code    
      ,t1.[ItemID] AS item_desc
      ,t1.[UnitPrice] AS item_rate
      ,'' AS msr_unit_code
      ,t1.[Quantity] AS qnty
      ,t1.[Total]      
      ,t3.BrandName
      ,t2.Name AS[des_name]
      ,t4.salqnty-isnull(t1.[Quantity],0) AS salqnty
      ,t4.PvUnitPrice
      ,'' AS [Type]
      ,tt.ItemID AS ItemsID
      ,t1.discountAmt,convert(decimal(18,2),(t1.discountAmt/t1.[Quantity])) as discountAmtfix ,isnull(t1.VatAmt,0) as TaxAmount,convert(decimal(18,2),(isnull(t1.VatAmt,0)/t1.[Quantity])) as TaxAmountfix
      ,t1.FixdiscountAmt,tt.Barcode
  FROM OrderReturnDetail t1 inner join dbo.OrderReturn tt1 on tt1.ID=t1.OrderReturnMstID and t1.BranchId=tt1.BranchId inner join [dbo].[OutLetItemStock] tt on tt.ID=t1.ItemID left join  Item t2 on t2.ID=tt.ItemID left join Brand t3 on t3.ID=t2.Brand
  inner join (select t1.ItemID,t2.ID,t1.[CostPrice] AS PvUnitPrice,CONVERT(DECIMAL(18,1),ISNULL(t1.Quantity,0)-ISNULL(t1.ReturnQuantity,0)) AS salqnty from dbo.OrderDetail t1 inner join [Order] t2 on t2.ID=t1.OrderID and t2.BranchId='" + BranchId+"') t4 on t4.ItemID=t1.ItemID AND t4.ID=tt1.InvoiceNo   where t1.OrderReturnMstID='" + OrderReturnMstID + "' and tt1.BranchId='"+BranchId+"'  union all select NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL  ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OrderReturn");
        return dt;
    }
    public static DataTable ManiBranchItemsDetails(string OrderReturnMstID, string BranchId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]  
	  ,'' AS item_code    
      ,t1.[ItemID] AS item_desc
      ,t1.[UnitPrice] AS item_rate
      ,'' AS msr_unit_code
      ,t1.[Quantity] AS qnty
      ,t1.[Total]      
      ,t3.BrandName
      ,t2.Name AS[des_name]
      ,t4.salqnty-isnull(t1.[Quantity],0) AS salqnty
      ,t4.PvUnitPrice
      ,'' AS [Type]
      ,tt.ItemID AS ItemsID
      ,t1.discountAmt,convert(decimal(18,2),(t1.discountAmt/t1.[Quantity])) as discountAmtfix ,isnull(t1.VatAmt,0) as TaxAmount,convert(decimal(18,2),(isnull(t1.VatAmt,0)/t1.[Quantity])) as TaxAmountfix
      ,t1.FixdiscountAmt,tt.Barcode
  FROM OrderReturnDetail t1 inner join dbo.OrderReturn tt1 on tt1.ID=t1.OrderReturnMstID and t1.BranchId=tt1.BranchId inner join [dbo].[ItemStock] tt on tt.ID=t1.ItemID left join  Item t2 on t2.ID=tt.ItemID left join Brand t3 on t3.ID=t2.Brand
  inner join (select t1.ItemID,t2.ID,t1.[CostPrice] AS PvUnitPrice,CONVERT(DECIMAL(18,1),ISNULL(t1.Quantity,0)-ISNULL(t1.ReturnQuantity,0)) AS salqnty from dbo.OrderDetail t1 inner join [Order] t2 on t2.ID=t1.OrderID and t2.BranchId='" + BranchId + "') t4 on t4.ItemID=t1.ItemID AND t4.ID=tt1.InvoiceNo   where t1.OrderReturnMstID='" + OrderReturnMstID + "' and tt1.BranchId='" + BranchId + "'  union all select NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL  ";

      
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OrderReturn");
        return dt;
    }
    public static void DeleteItemsReturn(SaleReturn rtn, DataTable dtOldList)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            string Query = @"select t1.VCH_SYS_NO  from [GL_TRANS_MST] t1 where SERIAL_NO='" + rtn.Return_No + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemPurchaseMst");

            command.CommandText = @"DELETE FROM [GL_TRANS_MST]  WHERE SERIAL_NO='" + rtn.Return_No + "'";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                command.CommandText = @"DELETE FROM [GL_TRANS_DTL]  WHERE VCH_SYS_NO='" + dr["VCH_SYS_NO"].ToString() + "'";
                command.ExecuteNonQuery();
            }

            //command.CommandText = @"DELETE FROM [OrderReturnDetail]  WHERE OrderReturnMstID='"+rtn.ID+"'";
            //command.ExecuteNonQuery();


            foreach (DataRow dr in dtOldList.Rows)
            {
                if (!string.IsNullOrEmpty(dr["item_desc"].ToString()))
                {
                    command.CommandText = @"DELETE FROM [OrderReturnDetail] WHERE OrderReturnMstID='" + rtn.ID +
                                          "' and [ItemID]='" + dr["item_desc"].ToString() + "' and BranchId='"+rtn.BranchID+"' ";
                    command.ExecuteNonQuery();

                    command.CommandText = @"UPDATE [ItemStock]
                         SET [SalesClosingQty] =ISNULL([SalesClosingQty],0)-'" + dr["qnty"].ToString() +
                                          "',ClosingStock=ISNULL(ClosingStock,0)-'" + dr["qnty"].ToString() +
                                          "' WHERE ID='" +
                                          dr["item_desc"].ToString() + "'";
                    command.ExecuteNonQuery();
                    
                }
            }
            command.CommandText = @"DELETE FROM [OrderReturn] WHERE ID='" + rtn.ID + "' and BranchId='"+rtn.BranchID+"'";
            command.ExecuteNonQuery();

            transaction.Commit();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }
    }











    public static void BranchDeleteItemsReturn(SaleReturn rtn, DataTable dtOldList)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            string Query = @"select t1.VCH_SYS_NO  from [GL_TRANS_MST] t1 where SERIAL_NO='" + rtn.Return_No + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemPurchaseMst");

            command.CommandText = @"DELETE FROM [GL_TRANS_MST]  WHERE SERIAL_NO='" + rtn.Return_No + "'";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                command.CommandText = @"DELETE FROM [GL_TRANS_DTL]  WHERE VCH_SYS_NO='" + dr["VCH_SYS_NO"].ToString() + "'";
                command.ExecuteNonQuery();
            }

            //command.CommandText = @"DELETE FROM [OrderReturnDetail]  WHERE OrderReturnMstID='"+rtn.ID+"'";
            //command.ExecuteNonQuery();


            foreach (DataRow dr in dtOldList.Rows)
            {
                if (!string.IsNullOrEmpty(dr["item_desc"].ToString()))
                {
                    command.CommandText = @"DELETE FROM [OrderReturnDetail] WHERE OrderReturnMstID='" + rtn.ID +
                                          "' and [ItemID]='" + dr["item_desc"].ToString() + "' and BranchId='"+rtn.BranchID+"' ";
                    command.ExecuteNonQuery();

                    command.CommandText = @"UPDATE [OutLetItemStock]
                         SET [SalesClosingQty] =ISNULL([SalesClosingQty],0)-'" + dr["qnty"].ToString() +
                                          "',ClosingStock=ISNULL(ClosingStock,0)-'" + dr["qnty"].ToString() +
                                          "' WHERE ID='" +
                                          dr["item_desc"].ToString() + "' and Barcode='" + dr["Barcode"].ToString() + "'";
                    command.ExecuteNonQuery();

                }
            }
            command.CommandText = @"DELETE FROM [OrderReturn] WHERE ID='" + rtn.ID + "' and BranchId='"+rtn.BranchID+"'";
            command.ExecuteNonQuery();

            transaction.Commit();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }
    }














}