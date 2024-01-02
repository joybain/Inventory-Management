using System;
using System.Data;

using System.Data.SqlClient;

using Delve;

/// <summary>
/// Summary description for SalesManager
/// </summary>
public class SalesManager
{
	public SalesManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable GetShowItemsInformation(string ID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]
      ,t1.[Code]
      ,t1.[Name]+' - '+isnull(t1.ModelNo,0) as txtItems
      ,isnull(t2.Rate,0) AS Tax
      ,t1.[DiscountAmount] 
      ,t1.[UnitPrice] AS SPrice
      ,'1' AS Qty   
      ,t1.[UOMID] AS [msr_unit_code] 
      ,CASE WHEN t4.SerialNo IS NULL then convert(decimal(18,0),t1.ClosingStock,0) else t4.StockQty end as ClosingStock
      ,convert(decimal(18,2),((ISNULL(t1.UnitPrice,0)*1)-(ISNULL(t1.UnitPrice,0)*(t1.DiscountAmount/100)))) AS Total 
	  ,t4.SerialNo AS item_Serial
	  ,convert(nvarchar,t4.ID) AS item_Serial_ID
      ,'' AS Remarks
      FROM [Item] t1 left join TaxCategory t2 on t2.ID=t1.TaxCategoryID and t2.Active='1'
      left join UOM t3 on t3.ID=t1.UOMID
	  left join ItemsDetailsInfo t4 on t4.ItemMstID=t1.ID
	   where t1.[Active]='1' and t1.Code='" + ID + "' and t1.ClosingStock>0 ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
        return dt;
    }
    public static DataTable GetShowItemsInformation(string SeialID,int flag)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]
      ,t1.[Code]
      ,t1.[Name]+' - '+isnull(t1.ModelNo,0) as txtItems
      ,isnull(t2.Rate,0) AS Tax
      ,t1.[DiscountAmount] 
      ,t1.[UnitPrice] AS SPrice
      ,'1' AS Qty   
      ,t1.[UOMID] AS [msr_unit_code] 
      ,CASE WHEN t4.SerialNo IS NULL then convert(decimal(18,0),t1.ClosingStock,0) else t4.StockQty end as ClosingStock
      ,convert(decimal(18,2),((ISNULL(t1.UnitPrice,0)*1)-(ISNULL(t1.UnitPrice,0)*(t1.DiscountAmount/100)))) AS Total 
	  ,t4.SerialNo AS item_Serial
	  ,convert(nvarchar,t4.ID) AS item_Serial_ID
      ,'' AS Remarks
      FROM [Item] t1 left join TaxCategory t2 on t2.ID=t1.TaxCategoryID and t2.Active='1'
      left join UOM t3 on t3.ID=t1.UOMID
	  left join ItemsDetailsInfo t4 on t4.ItemMstID=t1.ID
	   where t1.[Active]='1' and t4.ID='" + SeialID + "' and t4.StockQty>0 ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
        return dt;
    }
    public static Sales GetShowSalesInfo(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT * FROM [View_Order_Mst] where ID='" + ID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new Sales(dt.Rows[0]);
    }


 


    public static Sales GetBranchShowSalesInfo(string ID,string BranchId)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT * FROM [View_Order_Mst] where ID='" + ID + "'and BranchId='"+BranchId+"'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new Sales(dt.Rows[0]);
    }







    public static int SaveSalesInfo(Sales aSales, DataTable dt, DataTable dtPayment, VouchMst _aVouchMst, VouchMst _aVouchMstCR, VouchMst _aVouchMstTax)
    {
        decimal PurchasePrice = 0;
        int SalesID = 0;
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        DataTable dtFixCode;
        if (string.IsNullOrEmpty(aSales.BranchId) || aSales.BranchId == "0")
        {
            dtFixCode = VouchManager.GetAllFixGlCode("");
            
        }
        else
        {
            dtFixCode = VouchManager.GetAllFixGlCode(aSales.BranchId);
        }
        connection.Open();
        transaction = connection.BeginTransaction();
        try
        {
           
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            string FldPaymentMethodID = "",
                FldPaymentMethodNumber = "",
                FldBankId = "",
                FldChequeDate = "",
                FldOrderStatusID = "",
                ValPaymentMethodID = "",
                ValPaymentMethodNumber = "",
                ValBankId = "",
                ValChequeDate = "",
                ValOrderStatusID = "",
                FldExtraAmount="",
                ValExtraAmount = "";
          
            //if (!string.IsNullOrEmpty(aSales.BankId))
            //{
            //    FldPaymentMethodNumber = ",PaymentMethodNumber";
            //    FldBankId = ",BankId";
            //    FldChequeDate = ",ChequeDate";
            //    FldOrderStatusID = ",OrderStatusID";
               
            //    ValPaymentMethodNumber = ",'" + aSales.PMNumber + "' ";
            //    ValBankId = ",'" + aSales.BankId + "'";
            //    ValChequeDate = ",convert(date,'" + aSales.ChequeDate + "',103)";
            //    ValOrderStatusID = ",'" + aSales.Chk_Status + "'";
                
            //}
            FldExtraAmount = ",ExtraAmount";
            ValExtraAmount = ",'" + Convert.ToDecimal(aSales.ExtraAmount) + "'";

            command.CommandText = @"INSERT INTO [Order]
           (BranchId,[InvoiceNo],[SubTotal],[TaxAmount],[DiscountAmount],OrderType,[GrandTotal],[CashReceived],[CashRefund],[OrderDate],[CustomerID],[Due],[DeliveryStatus], [DeliveryDate],[Remark],[CreatedBy],[CreatedDate],PaymentMethodID,LocalCustomer,LocalCusAddress,LocalCusPhone,Note " +
                                  FldPaymentMethodNumber + " " + FldBankId + " " + FldChequeDate + " " +
                                  FldOrderStatusID + " " + FldExtraAmount + " ) VALUES ('" + aSales.BranchId + "','" + aSales.Invoice + "','" + aSales.Total + "','" +
                                  aSales.Tax + "','" + aSales.Disount + "','" +
                                  aSales.OrderType + "','" + aSales.GTotal + "','" + aSales.CReceive +
                                  "','"+aSales.CRefund+"',convert(datetime, nullif( '" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),''), 103),'" +
                                  aSales.Customer + "','" + Convert.ToDecimal(aSales.Due) + "','" + aSales.DvStatus +
                                  "',convert(datetime, nullif( '" + aSales.DvDate + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),''), 103),'" + aSales.Remarks +
                                  "','" + aSales.LoginBy + "',GETDATE(),'" + aSales.PMethod + "','" +
                                  aSales.LocalCustomer + "','" + aSales.LocalCusAddress + "','" + aSales.LocalCusPhone +
                                  "','" + aSales.Note + "' " +
                                  ValPaymentMethodNumber + ValBankId + ValChequeDate + ValOrderStatusID +ValExtraAmount+ ")";
             command.ExecuteNonQuery();

            command.CommandText = @"SELECT top(1) [ID]  FROM [Order] order by ID desc";
            string OrderMstID = command.ExecuteScalar().ToString();

            SalesID = Convert.ToInt32(OrderMstID);
          
            //*************************** Order Details ********************************// 
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Code"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [OrderDetail]
           (BranchId,[OrderID],[ItemID],[UnitPrice]  ,[TaxRate] ,[DiscountAmount] ,[SalePrice] ,[Quantity] ,[TotalPrice] ,[CreatedBy],[CreatedDate],Remarks,CostPrice,Barcode)
     VALUES
           ('"+aSales.BranchId+"','" + OrderMstID + "','" + dr["ID"].ToString() + "','" + dr["SPrice"].ToString().Replace(",", "") + "','" +
                                          dr["Tax"].ToString().Replace(",", "") + "','" +
                                          dr["DiscountAmount"].ToString().Replace(",", "") + "','" +
                                          dr["SPrice"].ToString().Replace(",", "") + "','" +
                                          dr["Qty"].ToString().Replace(",", "") + "','" +
                                          dr["Total"].ToString().Replace(",", "") + "','" + aSales.LoginBy +
                                          "',GETDATE(),'" +
                                          dr["Remarks"].ToString().Replace("'", "") + "','" +
                                          dr["CostPrice"].ToString().Replace(",", "") + "','" + dr["Barcode"].ToString() + "')";
                    command.ExecuteNonQuery();
                    PurchasePrice += (Convert.ToDecimal(dr["CostPrice"].ToString().Replace(",", "")) * Convert.ToDecimal(dr["Qty"].ToString().Replace(",", "")));
                }

            }
          


            foreach (DataRow drPay in dtPayment.Rows)
            {
                if (!string.IsNullOrEmpty(drPay["PaymentypeID"].ToString()))
                {
                    //                    command.CommandText = @"INSERT INTO [dbo].[Order_Payment]
                    //                   ([OrderID],[PaymentypeID],[Paymentype],[BankID],[AccountID],[Amount])
                    //                VALUES
                    //                   ('" + OrderMstID + "','" + drPay["PaymentypeID"].ToString() + "','" +
                    //                                          drPay["Paymentype"].ToString() +
                    //                                          "','" + drPay["BankID"].ToString() + "','" + drPay["AccountID"].ToString() +
                    //                                          "','" + drPay["Amount"].ToString() + "')";
                    //                    command.ExecuteNonQuery();

                    command.CommandText =
                        "insert into Order_Payment(BranchId,MstId, PaymentypeId, PaymentypeIdFrom, Paymentype, PaymentypeFrom, Amount, AddBy, AddDate,[BankID],[BankName] ,                                  [AccountNo],[AccountID],[CardOrCheeckNo],[BankIDFrom],[BankNameFrom],[AccountNoFrom],[Status],[ApprovedDate]) values('" + aSales.BranchId + "','" +
                        OrderMstID + "','" + drPay["PaymentypeId"].ToString() + "','" +
                        drPay["PaymentypeIdFrom"].ToString() + "','" + drPay["Paymentype"].ToString() + "','" +
                        drPay["PaymentypeFrom"].ToString() + "','" + drPay["Amount"].ToString() + "','" +
                        aSales.LoginBy + "',GETDATE(),'" + drPay["BankID"].ToString() + "','" +
                        drPay["BankName"].ToString() + "','" + drPay["AccountNo"].ToString() + "','" +
                        drPay["AccountID"].ToString() + "'," +
                        "'" + drPay["CardOrCheeckNo"].ToString() + "','" + drPay["BankIDFrom"].ToString() + "','" +
                        drPay["BankNameFrom"].ToString() + "','" + drPay["AccountNoFrom"].ToString() + "','" +
                        drPay["Status"].ToString() + "',convert(date,'" + drPay["ApprovedDate"].ToString() + "',103))";
                    command.ExecuteNonQuery();

                }
            }

          

            if (Convert.ToDouble(aSales.CReceive) > 0)
            {
                command.CommandText = @"INSERT INTO [dbo].[CustomerPaymentReceive]
           ([Date],BranchId,[Customer_id],[Invoice],[PayAmt],[entry_by],[entry_date])
     VALUES
           (convert(datetime, nullif( '" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),''), 103),'"+aSales.BranchId+"','" + aSales.Customer + "','" + OrderMstID +
                                      "','" + (Convert.ToDecimal(aSales.CReceive) - Convert.ToDecimal(aSales.CRefund)) + "','" + aSales.LoginBy + "',GETDATE())";
                command.ExecuteNonQuery();


            }
            //********************* Sales Total (Show Purchase Price) *********//

            //command.CommandText = "SP_PV_UnitPrice_All";
            //command.CommandType = CommandType.StoredProcedure;
            //command.Parameters.AddWithValue("@MstID", Convert.ToInt32(OrderMstID));
            //command.Parameters.AddWithValue("@Rate", Convert.ToDecimal(CurrencyRate));
          //  PurchasePrice = Convert.ToDecimal(command.ExecuteScalar());

            //***************************  Jurnal Voucher ********************************// 

            //double totVat = 0, Discount = 0;
            //totVat = ((Convert.ToDouble(aSales.Total) - Convert.ToDouble(aSales.Disount)) * Convert.ToDouble(aSales.Tax)) / 100;
            //Discount = Convert.ToDouble(aSales.Disount);
            //_aVouchMst.ControlAmt = aSales.GTotal;
            //command.CommandType = CommandType.Text;
            //command.CommandText = VouchManager.SaveVoucherMst(_aVouchMst, 1);
            //command.ExecuteNonQuery();
            //VouchDtl vdtl;
            //for (int j = 0; j < 4; j++)
            //{
            //    if (j == 0)
            //    {
            //        //DataRow 
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "1";

            //        if (string.IsNullOrEmpty(aSales.BranchId) || aSales.BranchId == "0")
            //        {

            //            vdtl.GlCoaCode = dtFixCode.Rows[0]["PHSalesInvoice"].ToString(); //**** Sales Code *******//
            //            vdtl.AccType =
            //            VouchManager.getAccType(dtFixCode.Rows[0]["PHSalesInvoice"].ToString()); //**** Sales Code *******//
            //            vdtl.Particulars = dtFixCode.Rows[0]["PHSalesInvoiceDesc"].ToString();
            //        }
            //        else
            //        {
            //            vdtl.GlCoaCode = dtFixCode.Rows[0]["SalesReveneuCode"].ToString(); //**** Sales Code *******//
            //            vdtl.AccType =
            //            VouchManager.getAccType(dtFixCode.Rows[0]["SalesReveneuCode"].ToString()); //**** Sales Code *******//
            //            vdtl.Particulars = "Branch  Sales";
            //        }

                   



            //        vdtl.AmountDr = "0";
            //        vdtl.AmountCr = aSales.GTotal;
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        vdtl.AUTHO_USER = _aVouchMst.EntryUser;
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
            //    }
            //    else if (j == 1)
            //    {
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "2";
            //        vdtl.GlCoaCode = "1-" + aSales.CustomerCoa;
            //        vdtl.Particulars = aSales.CustomerName;
            //        vdtl.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //        vdtl.AmountDr = _aVouchMst.ControlAmt.Replace(",", "");
            //        vdtl.AmountCr = "0";
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        vdtl.AUTHO_USER = _aVouchMst.EntryUser;
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
            //    }
            //    else if (j == 2)
            //    {
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "3";


            //        if (string.IsNullOrEmpty(aSales.BranchId) || aSales.BranchId == "0")
            //        {

            //            vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
            //            vdtl.Particulars = "Closing Stock";
            //            vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
            //        }
            //        else
            //        {
            //            vdtl.GlCoaCode = dtFixCode.Rows[0]["Gl_CoaCode"].ToString();
            //            vdtl.Particulars = "Branch Closing Stock";
            //            vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Gl_CoaCode"].ToString());
            //        }



                   
            //        vdtl.AmountDr = "0";
            //        vdtl.AmountCr = PurchasePrice.ToString().Replace(",", "");
            //        vdtl.AUTHO_USER = "CS";
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);

            //    }

            //    else if (j == 3)
            //    {
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "4";
            //        vdtl.GlCoaCode = "1-"+dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
            //        vdtl.AccType =
            //                VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
            //                    .ToString()); //**** Purchase Code *******//
            //        vdtl.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
            //        vdtl.AmountDr = "0";
            //        vdtl.AmountCr = totVat.ToString().Replace("'", "").Replace(",", ""); ;
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        vdtl.AUTHO_USER = _aVouchMst.EntryUser;
            //            //*********** Convert Rate ********//
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
            //    }
            //}

            //***************************  Credit Voucher ********************************// 
            if (Convert.ToDecimal(aSales.CReceive) > 0)
            {
                _aVouchMstCR.ControlAmt = aSales.CReceive.Replace("'", "").Replace(",", "");
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
                        vdtlCR.ValueDate = aSales.Date;
                        vdtlCR.LineNo = "1";
                        vdtlCR.GlCoaCode = "1-" + aSales.CustomerCoa;
                        vdtlCR.Particulars = aSales.CustomerName;
                        vdtlCR.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
                        vdtlCR.AmountDr = "0";
                        vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
                        vdtlCR.Status = _aVouchMstCR.Status;
                        vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

                        vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                    }
                    else if (j == 1)
                    {
                        int LineNumber = 2;
                        foreach (DataRow dr1  in dtPayment.Rows)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                            vdtlCR.ValueDate = aSales.Date;
                            vdtlCR.LineNo =LineNumber.ToString();
                           


                            if (string.IsNullOrEmpty(aSales.BranchId) || aSales.BranchId == "0")
                            {

                                if (string.IsNullOrEmpty(dr1["AccountId"].ToString()))
                                {
                                    vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                    vdtlCR.AccType =
                                        VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                            .ToString()); //**** SalesCode *******//
                                    vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                                }
                                else
                                {
                                    var GlCoaCode = IdManager.GetShowSingleValueString("Gl_Code", "Id", "bank_branch",
                                        dr1["AccountId"].ToString());


                                    vdtlCR.GlCoaCode = "1-" + GlCoaCode; //**** SalesCode *******//
                                    vdtlCR.AccType =
                                        VouchManager.getAccType("1-" + GlCoaCode); //**** SalesCode *******//
                                    vdtlCR.Particulars = aSales.BankName;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(dr1["AccountId"].ToString()))
                                {
                                    vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashCoaCode"].ToString(); //**** SalesCode *******//
                                    vdtlCR.AccType =
                                        VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashCoaCode"]
                                            .ToString()); //**** SalesCode *******//
                                    vdtlCR.Particulars = "Branch Cash in Hand-Bangladesh";
                                }
                                else
                                {
                                    var GlCoaCode = IdManager.GetShowSingleValueString("Gl_Code", "Id", "bank_branch",
                                        dr1["AccountId"].ToString());


                                    vdtlCR.GlCoaCode = "1-" + GlCoaCode; //**** SalesCode *******//
                                    vdtlCR.AccType =
                                        VouchManager.getAccType("1-" + GlCoaCode); //**** SalesCode *******//
                                    vdtlCR.Particulars = aSales.BankName;
                                }
                            }



                            //vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
                            vdtlCR.AmountDr = dr1["Amount"].ToString();
                            vdtlCR.AmountCr = "0";
                            vdtlCR.Status = _aVouchMstCR.Status;
                            vdtlCR.BookName = _aVouchMstCR.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                            LineNumber++;
                        }
                    }
                }
            }

            //***************************  Tax ********************************// 
            //if (Convert.ToDecimal(aSales.Tax) > 0)
            //{
            //    _aVouchMstTax.ControlAmt = totVat.ToString().Replace("'", "").Replace(",", "");
            //    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstTax, 1);
            //    command.ExecuteNonQuery();
            //    VouchDtl vdtlTax;
            //    for (int j = 0; j < 2; j++)
            //    {
            //        if (j == 0)
            //        {
            //            //DataRow 
            //            vdtlTax = new VouchDtl();
            //            vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
            //            vdtlTax.ValueDate = aSales.Date;
            //            vdtlTax.LineNo = "1";
            //            vdtlTax.GlCoaCode = "1-" + aSales.CustomerCoa;
            //            vdtlTax.Particulars = aSales.CustomerName;
            //            vdtlTax.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //            vdtlTax.AmountDr = totVat.ToString().Replace("'", "").Replace(",", "");
            //            vdtlTax.AmountCr = "0";
            //            vdtlTax.Status = _aVouchMstTax.Status;
            //            vdtlTax.BookName = _aVouchMstTax.BookName; //*********** Convert Rate ********//

            //            vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
            //            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
            //        }
            //        else if (j == 1)
            //        {
            //            vdtlTax = new VouchDtl();
            //            vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
            //            vdtlTax.ValueDate = aSales.Date;
            //            vdtlTax.LineNo = "2";
            //            vdtlTax.GlCoaCode = dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
            //            vdtlTax.AccType =
            //                VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
            //                    .ToString()); //**** Purchase Code *******//
            //            vdtlTax.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
            //            vdtlTax.AmountDr = "0";
            //            vdtlTax.AmountCr = totVat.ToString().Replace("'", "").Replace(",", ""); ;
            //            vdtlTax.Status = _aVouchMstTax.Status;
            //            vdtlTax.BookName = _aVouchMstTax.BookName;
            //            vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
            //            //*********** Convert Rate ********//
            //            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
            //        }
            //    }
            //}


            command.CommandText = "update FixGlCoaCode set InvoiceID=(select InvoiceID+1 from FixGlCoaCode)";
            command.ExecuteNonQuery();

            command.CommandText = "update FixGlCoaCode set PrintOrderID='" + OrderMstID + "' ";
            command.ExecuteNonQuery();

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new Exception(ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        return SalesID;
    }

  





    public static int SaveSalesExchangeInfo(Sales aSales, DataTable dt, DataTable dtPayment, VouchMst _aVouchMst, VouchMst _aVouchMstCR, VouchMst _aVouchMstTax, DataTable dt1, string OrderId, SalesExchangeModel _salesExchange)
    {
        decimal PurchasePrice = 0;
        int SalesID = 0;
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        connection.Open();
        transaction = connection.BeginTransaction();
        try
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            string FldPaymentMethodID = "",
                FldPaymentMethodNumber = "",
                FldBankId = "",
                FldChequeDate = "",
                FldOrderStatusID = "",
                ValPaymentMethodID = "",
                ValPaymentMethodNumber = "",
                ValBankId = "",
                ValChequeDate = "",
                ValOrderStatusID = "",
                FldExtraAmount = "",
                ValExtraAmount = "";

            //if (!string.IsNullOrEmpty(aSales.BankId))
            //{
            //    FldPaymentMethodNumber = ",PaymentMethodNumber";
            //    FldBankId = ",BankId";
            //    FldChequeDate = ",ChequeDate";
            //    FldOrderStatusID = ",OrderStatusID";

            //    ValPaymentMethodNumber = ",'" + aSales.PMNumber + "' ";
            //    ValBankId = ",'" + aSales.BankId + "'";
            //    ValChequeDate = ",convert(date,'" + aSales.ChequeDate + "',103)";
            //    ValOrderStatusID = ",'" + aSales.Chk_Status + "'";

            //}
            FldExtraAmount = ",ExtraAmount";
            ValExtraAmount = ",'0'";

            command.CommandText = @"INSERT INTO [Order]
           (BranchId,[InvoiceNo],[SubTotal],[TaxAmount],[DiscountAmount],OrderType,[GrandTotal],[CashReceived],ExchangeAmount,[CashRefund],[OrderDate],[CustomerID],[Due],[DeliveryStatus], [DeliveryDate],[Remark],[CreatedBy],[CreatedDate],PaymentMethodID,LocalCustomer,LocalCusAddress,LocalCusPhone,Note " +
                                  FldPaymentMethodNumber + " " + FldBankId + " " + FldChequeDate + " " +
                                  FldOrderStatusID + " " + FldExtraAmount + " ) VALUES ('" + aSales.BranchId + "','" + aSales.Invoice + "','" + aSales.Total + "','" +
                                  aSales.Tax + "','" + aSales.Disount + "','" +
                                  aSales.OrderType + "','" + aSales.GTotal + "','" + aSales.CReceive +
                                  "','"+aSales.ExchangeAmount+"','" + aSales.CRefund + "',convert(datetime, nullif( '" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),''), 103),'" +
                                  aSales.Customer + "','" + Convert.ToDecimal(aSales.Due) + "','" + aSales.DvStatus +
                                  "',convert(datetime, nullif( '" + aSales.DvDate + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),''), 103),'" + aSales.Remarks +
                                  "','" + aSales.LoginBy + "',GETDATE(),'" + aSales.PMethod + "','" +
                                  aSales.LocalCustomer + "','" + aSales.LocalCusAddress + "','" + aSales.LocalCusPhone +
                                  "','" + aSales.Note + "' " +
                                  ValPaymentMethodNumber + ValBankId + ValChequeDate + ValOrderStatusID + ValExtraAmount + ")";
            command.ExecuteNonQuery();

            command.CommandText = @"SELECT top(1) [ID]  FROM [Order]  where BranchId='"+aSales.BranchId+"' order by ID desc";
            string OrderMstID = command.ExecuteScalar().ToString();

            SalesID = Convert.ToInt32(OrderMstID);

//            //*************************** Order Details ********************************// 
//            foreach (DataRow dr in dt.Rows)
//            {
//                if (dr["Code"].ToString() != "")
//                {
//                    command.CommandText = @"INSERT INTO [OrderDetail]
//           ([OrderID],[ItemID],[UnitPrice]  ,[TaxRate] ,[DiscountAmount] ,[SalePrice] ,[Quantity] ,[TotalPrice] ,[CreatedBy],[CreatedDate],Remarks,CostPrice,Barcode)
//     VALUES
//           ('" + SalesID + "','" + dr["ID"].ToString() + "','" + dr["SPrice"].ToString().Replace(",", "") + "','" +
//                                          dr["Tax"].ToString().Replace(",", "") + "','" +
//                                          dr["DiscountAmount"].ToString().Replace(",", "") + "','" +
//                                          dr["SPrice"].ToString().Replace(",", "") + "','" +
//                                          dr["Qty"].ToString().Replace(",", "") + "','" +
//                                          dr["Total"].ToString().Replace(",", "") + "','" + aSales.LoginBy +
//                                          "',GETDATE(),'" +
//                                          dr["Remarks"].ToString().Replace("'", "") + "','" +
//                                          dr["CostPrice"].ToString().Replace(",", "") + "','" + dr["Barcode"].ToString() + "')";
//                    command.ExecuteNonQuery();
//                    PurchasePrice = +Convert.ToDecimal(dr["CostPrice"].ToString().Replace(",", ""));
//                }

//            }


            //*************************** Order Details ********************************// 
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Code"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [OrderDetail]
           (BranchId,[OrderID],[ItemID],[UnitPrice]  ,[TaxRate] ,[DiscountAmount] ,[SalePrice] ,[Quantity] ,[TotalPrice] ,[CreatedBy],[CreatedDate],Remarks,CostPrice,Barcode)
     VALUES
           ('" + aSales.BranchId + "','" + OrderMstID + "','" + dr["ID"].ToString() + "','" + dr["SPrice"].ToString().Replace(",", "") + "','" +
                                          dr["Tax"].ToString().Replace(",", "") + "','" +
                                          dr["DiscountAmount"].ToString().Replace(",", "") + "','" +
                                          dr["SPrice"].ToString().Replace(",", "") + "','" +
                                          dr["Qty"].ToString().Replace(",", "") + "','" +
                                          dr["Total"].ToString().Replace(",", "") + "','" + aSales.LoginBy +
                                          "',GETDATE(),'" +
                                          dr["Remarks"].ToString().Replace("'", "") + "','" +
                                          dr["CostPrice"].ToString().Replace(",", "") + "','" + dr["Barcode"].ToString() + "')";
                    command.ExecuteNonQuery();
                    PurchasePrice += (Convert.ToDecimal(dr["CostPrice"].ToString().Replace(",", "")) * Convert.ToDecimal(dr["Qty"].ToString().Replace(",", "")));
                }

            }


            foreach (DataRow drPay in dtPayment.Rows)
            {
                if (!string.IsNullOrEmpty(drPay["PaymentypeID"].ToString()))
                {
                    //                    command.CommandText = @"INSERT INTO [dbo].[Order_Payment]
                    //                   ([OrderID],[PaymentypeID],[Paymentype],[BankID],[AccountID],[Amount])
                    //                VALUES
                    //                   ('" + OrderMstID + "','" + drPay["PaymentypeID"].ToString() + "','" +
                    //                                          drPay["Paymentype"].ToString() +
                    //                                          "','" + drPay["BankID"].ToString() + "','" + drPay["AccountID"].ToString() +
                    //                                          "','" + drPay["Amount"].ToString() + "')";
                    //                    command.ExecuteNonQuery();

                    command.CommandText =
                        "insert into Order_Payment(BranchId,MstId, PaymentypeId, PaymentypeIdFrom, Paymentype, PaymentypeFrom, Amount, AddBy, AddDate,[BankID],[BankName] ,                                  [AccountNo],[AccountID],[CardOrCheeckNo],[BankIDFrom],[BankNameFrom],[AccountNoFrom],[Status],[ApprovedDate]) values('"+aSales.BranchId+"','" +
                        OrderMstID + "','" + drPay["PaymentypeId"].ToString() + "','" +
                        drPay["PaymentypeIdFrom"].ToString() + "','" + drPay["Paymentype"].ToString() + "','" +
                        drPay["PaymentypeFrom"].ToString() + "','" + drPay["Amount"].ToString() + "','" +
                        aSales.LoginBy + "',GETDATE(),'" + drPay["BankID"].ToString() + "','" +
                        drPay["BankName"].ToString() + "','" + drPay["AccountNo"].ToString() + "','" +
                        drPay["AccountID"].ToString() + "'," +
                        "'" + drPay["CardOrCheeckNo"].ToString() + "','" + drPay["BankIDFrom"].ToString() + "','" +
                        drPay["BankNameFrom"].ToString() + "','" + drPay["AccountNoFrom"].ToString() + "','" +
                        drPay["Status"].ToString() + "',convert(date,'" + drPay["ApprovedDate"].ToString() + "',103))";
                    command.ExecuteNonQuery();

                }
            }

            if (Convert.ToDouble(aSales.CReceive) > 0)
            {
                command.CommandText = @"INSERT INTO [dbo].[CustomerPaymentReceive]
           (BranchId,[Date],[Customer_id],[Invoice],[PayAmt],[entry_by],[entry_date])
     VALUES
           ('" + aSales.BranchId + "',convert(datetime, nullif( '" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),''), 103),'" + aSales.Customer + "','" + OrderMstID +
                                      "','" + (Convert.ToDecimal(aSales.CReceive) - Convert.ToDecimal(aSales.CRefund)) + "','" + aSales.LoginBy + "',GETDATE())";
                command.ExecuteNonQuery();


            }
            //********************* Sales Total (Show Purchase Price) *********//

            //command.CommandText = "SP_PV_UnitPrice_All";
            //command.CommandType = CommandType.StoredProcedure;
            //command.Parameters.AddWithValue("@MstID", Convert.ToInt32(OrderMstID));
            //command.Parameters.AddWithValue("@Rate", Convert.ToDecimal(CurrencyRate));
            //  PurchasePrice = Convert.ToDecimal(command.ExecuteScalar());

            //***************************  Jurnal Voucher ********************************// 

            double totVat = 0, Discount = 0;
            totVat = ((Convert.ToDouble(aSales.Total)-Convert.ToDouble(aSales.ExchangeAmount) - Convert.ToDouble(aSales.Disount)) * Convert.ToDouble(aSales.Tax)) / 100;
            Discount = Convert.ToDouble(aSales.Disount);
            _aVouchMst.ControlAmt =aSales.GTotal;
            command.CommandType = CommandType.Text;
            command.CommandText = VouchManager.SaveVoucherMst(_aVouchMst, 1);
            command.ExecuteNonQuery();
            VouchDtl vdtl;
            for (int j = 0; j < 4; j++)
            {
                if (j == 0)
                {
                    //DataRow 
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = aSales.Date;
                    vdtl.LineNo = "1";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["PHSalesInvoice"].ToString(); //**** Sales Code *******//
                    vdtl.AccType =
                        VouchManager.getAccType(dtFixCode.Rows[0]["PHSalesInvoice"].ToString()); //**** Sales Code *******//
                    vdtl.Particulars = dtFixCode.Rows[0]["PHSalesInvoiceDesc"].ToString();
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = _aVouchMst.ControlAmt.Replace(",", "");
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    vdtl.AUTHO_USER = _aVouchMst.EntryUser;
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 1)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = aSales.Date;
                    vdtl.LineNo = "2";
                    vdtl.GlCoaCode = "1-" + aSales.CustomerCoa;
                    vdtl.Particulars = aSales.CustomerName;
                    vdtl.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
                    vdtl.AmountDr = _aVouchMst.ControlAmt.Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    vdtl.AUTHO_USER = _aVouchMst.EntryUser;
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 2)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = aSales.Date;
                    vdtl.LineNo = "3";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
                    vdtl.Particulars = "Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = PurchasePrice.ToString().Replace(",", "");
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);

                }
                else if (j == 3)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = aSales.Date;
                    vdtl.LineNo = "4";
                    vdtl.GlCoaCode ="1-"+ dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
                    vdtl.AccType =
                        VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
                            .ToString()); //**** Purchase Code *******//
                    vdtl.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = totVat.ToString().Replace("'", "").Replace(",", ""); ;
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    vdtl.AUTHO_USER = _aVouchMst.EntryUser;
                    //*********** Convert Rate ********//
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
            }

            //***************************  Credit Voucher ********************************// 
            if (Convert.ToDecimal(aSales.CReceive) > 0)
            {
                _aVouchMstCR.ControlAmt = aSales.CReceive.Replace("'", "").Replace(",", "");
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
                        vdtlCR.ValueDate = aSales.Date;
                        vdtlCR.LineNo = "1";
                        vdtlCR.GlCoaCode = "1-" + aSales.CustomerCoa;
                        vdtlCR.Particulars = aSales.CustomerName;
                        vdtlCR.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
                        vdtlCR.AmountDr = "0";
                        vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
                        vdtlCR.Status = _aVouchMstCR.Status;
                        vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

                        vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                    }
                    else if (j == 1)
                    {
                        int LineNumber = 2;
                        foreach (DataRow dr1 in dtPayment.Rows)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                            vdtlCR.ValueDate = aSales.Date;
                            vdtlCR.LineNo = LineNumber.ToString();
                            if (string.IsNullOrEmpty(dr1["AccountId"].ToString()))
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                var GlCoaCode = IdManager.GetShowSingleValueString("Gl_Code", "Id", "bank_branch",
                                    dr1["AccountId"].ToString());


                                vdtlCR.GlCoaCode = "1-" + GlCoaCode; //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + GlCoaCode); //**** SalesCode *******//
                                vdtlCR.Particulars = aSales.BankName;
                            }

                            //vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
                            vdtlCR.AmountDr = dr1["Amount"].ToString();
                            vdtlCR.AmountCr = "0";
                            vdtlCR.Status = _aVouchMstCR.Status;
                            vdtlCR.BookName = _aVouchMstCR.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                            LineNumber++;
                        }
                    }
                }
            }

            ////***************************  Tax ********************************// 
            //if (Convert.ToDecimal(aSales.Tax) > 0)
            //{
            //    _aVouchMstTax.ControlAmt = totVat.ToString().Replace("'", "").Replace(",", "");
            //    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstTax, 1);
            //    command.ExecuteNonQuery();
            //    VouchDtl vdtlTax;
            //    for (int j = 0; j < 2; j++)
            //    {
            //        if (j == 0)
            //        {
            //            //DataRow 
            //            vdtlTax = new VouchDtl();
            //            vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
            //            vdtlTax.ValueDate = aSales.Date;
            //            vdtlTax.LineNo = "1";
            //            vdtlTax.GlCoaCode = "1-" + aSales.CustomerCoa;
            //            vdtlTax.Particulars = aSales.CustomerName;
            //            vdtlTax.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //            vdtlTax.AmountDr = totVat.ToString().Replace("'", "").Replace(",", "");
            //            vdtlTax.AmountCr = "0";
            //            vdtlTax.Status = _aVouchMstTax.Status;
            //            vdtlTax.BookName = _aVouchMstTax.BookName; //*********** Convert Rate ********//

            //            vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
            //            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
            //        }
            //        else if (j == 1)
            //        {
            //            vdtlTax = new VouchDtl();
            //            vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
            //            vdtlTax.ValueDate = aSales.Date;
            //            vdtlTax.LineNo = "2";
            //            vdtlTax.GlCoaCode = dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
            //            vdtlTax.AccType =
            //                VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
            //                    .ToString()); //**** Purchase Code *******//
            //            vdtlTax.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
            //            vdtlTax.AmountDr = "0";
            //            vdtlTax.AmountCr = totVat.ToString().Replace("'", "").Replace(",", ""); ;
            //            vdtlTax.Status = _aVouchMstTax.Status;
            //            vdtlTax.BookName = _aVouchMstTax.BookName;
            //            vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
            //            //*********** Convert Rate ********//
            //            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
            //        }
            //    }
            //}


            command.CommandText = "update FixGlCoaCode set InvoiceID=(select InvoiceID+1 from FixGlCoaCode)";
            command.ExecuteNonQuery();

            command.CommandText = "update FixGlCoaCode set PrintOrderID='" + OrderMstID + "' ";
            command.ExecuteNonQuery();








            command.CommandText = "insert into OrderExchange (BranchId,companyId,InvoiceNo,NewInvoiceID,SubTotal,TaxAmount,DiscountAmount,GrandTotal,OrderDate,CustomerId,CreatedBy,CreatedDate) " +
                                  "values ('" + aSales.BranchId + "','','" + _salesExchange.InvoiceNo + "','" + OrderMstID + "','" + _salesExchange.SubTotal + "','" + _salesExchange.TaxAmount + "','" + _salesExchange.DiscountAmount.ToString("#,0.00") + "','" + _salesExchange.GrandTotal + "',GetDate(),'" + aSales.Customer + "','" + aSales.LoginBy + "',GetDate())";
            command.ExecuteNonQuery();


            command.CommandText = @"SELECT top(1) [ID]  FROM [OrderExchange] where BranchId='"+aSales.BranchId+"' order by ID desc";
            string OrderExchangeMstID = command.ExecuteScalar().ToString();



            foreach (DataRow dr1 in dt1.Rows)
            {


                var ChangeQty = Convert.ToInt32(dr1["ChangeQty"].ToString());
                var ItemId = Convert.ToInt32(dr1["ItemId"].ToString());
                var ItemCode = dr1["ItemCode"].ToString();
                var Barcode = dr1["Barcode"].ToString();
                if (ChangeQty > 0)
                {

                    if (string.IsNullOrEmpty(aSales.BranchId)|| aSales.BranchId=="0")
                    {
                        command.CommandText = "Update OrderDetail set ChangeQty='" + ChangeQty + "' where BranchId='"+aSales.BranchId+"' and OrderID='" + OrderId +
                                              "' and ItemID='" + ItemId + "'";
                        command.ExecuteNonQuery();

                        command.CommandText = "update ItemStock set  ClosingStock=ClosingStock+" + ChangeQty + "  Where ItemCode='" + ItemCode + "' and Barcode='" + Barcode + "' ";
                        var a = command.ExecuteNonQuery(); 
                    }

                    else
                    {
                        command.CommandText = "Update OrderDetail set ChangeQty='" + ChangeQty + "' where OrderID='" + OrderId +
                                              "' and ItemID='" + ItemId + "' and BranchId='"+aSales.BranchId+"'";
                        command.ExecuteNonQuery();

                        command.CommandText = "update OutLetItemStock set  ClosingStock=ClosingStock+" + ChangeQty + "  Where ItemCode='" + ItemCode + "' and Barcode='" + Barcode + "' ";
                        var a = command.ExecuteNonQuery();
                    }
                    


                    command.CommandText = "Insert into  OrderExchangeDetail (BranchId,ExchangeID,ItemID,UnitPrice,SalePrice,Quantity,TotalPrice,ItemCode,CreatedBy,CreatedDate)" +
                                          " values('"+aSales.BranchId+"','" + OrderExchangeMstID + "','" + ItemId + "','" + Convert.ToDecimal(dr1["UnitPrice"].ToString()) + "','" + Convert.ToDecimal(dr1["SalePrice"].ToString()) + "','" + ChangeQty + "'," +
                                          "'" + Convert.ToDecimal(dr1["TotalPrice"].ToString()) + "','" + dr1["ItemCode"].ToString() + "','" + aSales.LoginBy + "',GetDate())";
                    command.ExecuteNonQuery();
                }


            }













            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new Exception(ex.Message);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        return SalesID;
    }






















//    public static void UpdateSalesInfo(Sales aSales, DataTable dt, DataTable dtOldSalesDetails, DataTable dtPayment,VouchMst _aVouchMst,VouchMst _aVouchMstCR, VouchMst _aVouchMstTax)
//    {
//        decimal PurchasePrice = 0;
//        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
//        SqlTransaction transaction;
//        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
//        try
//        {
//            connection.Open();

//            transaction = connection.BeginTransaction();
//            SqlCommand command = new SqlCommand();
//            command.Connection = connection;
//            command.Transaction = transaction;

//            string FldPaymentMethodID = "",
//                FldPaymentMethodNumber = "",
//                FldBankId = "",
//                FldChequeDate = "",
//                FldOrderStatusID = "",
//            FldExtraAmount = ",ExtraAmount='0'";


//            if (!string.IsNullOrEmpty(aSales.BankId))
//            {
//                FldPaymentMethodNumber = ",PaymentMethodNumber='" + aSales.PMNumber + "' ";
//                FldBankId = ",BankId='" + aSales.BankId + "' ";
//                FldChequeDate = ",ChequeDate=convert(date,'" + aSales.ChequeDate + "',103) ";
//                FldOrderStatusID = ",OrderStatusID='" + aSales.Chk_Status + "'";
//            }


//            command.CommandText = @"update [Order] set
//           [SubTotal]='" + aSales.Total + "',BranchId='" + aSales.BranchId + "',[TaxAmount]='" + aSales.Tax + "',ExchangeAmount='"+aSales.ExchangeAmount+"',[DiscountAmount]='" + aSales.Disount +
//                                  "',[GrandTotal]='" + aSales.GTotal + "',[CashReceived]='" + aSales.CReceive +
//                                  "',[CashRefund]='" + aSales.CRefund + "',[OrderDate]=convert(date,'" + aSales.Date +
//                                  "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103),[CustomerID]='" + aSales.Customer + "',[Due]='" + Convert.ToDecimal(aSales.Due) +
//                                  "',[DeliveryStatus]='" + aSales.DvStatus + "',[DeliveryDate]=convert(date,'" +
//                                  aSales.DvDate + "',103),[Remark]='" + aSales.Remarks + "',[ModifiedBy]='" +
//                                  aSales.LoginBy + "',[ModifiedDate]=GETDATE(),PaymentMethodID='" + aSales.PMethod +
//                                  "',LocalCustomer='" + aSales.LocalCustomer + "',LocalCusAddress='" +
//                                  aSales.LocalCusAddress + "',LocalCusPhone='" + aSales.LocalCusPhone + "',Note='" +
//                                  aSales.Note + "' " +
//                                  FldPaymentMethodNumber + FldBankId + FldOrderStatusID + FldChequeDate + FldExtraAmount +
//                                  " where ID='" + aSales.ID + "' and BranchId='"+aSales.BranchId+"'";
//            command.ExecuteNonQuery();

//            foreach (DataRow drOld in dtOldSalesDetails.Rows)
//            {
//                command.CommandText = @"delete from [OrderDetail] where ID='" + drOld["Dtl_ID"].ToString() +
//                                      "' and OrderID='" + aSales.ID + "' and BranchId='"+aSales.BranchId+"'";
//                command.ExecuteNonQuery();
//            }
//            command.CommandText = @"DELETE FROM [Order_Payment] WHERE  MstId='" + aSales.ID + "'";
//            command.ExecuteNonQuery();

//            foreach (DataRow dr in dt.Rows)
//            {
//                if (dr["Code"].ToString() != "")
//                {
//                    command.CommandText = @"INSERT INTO [OrderDetail]
//           (BranchId,[OrderID],[ItemID],[UnitPrice]  ,[TaxRate] ,[DiscountAmount] ,[SalePrice] ,[Quantity] ,[TotalPrice] ,[CreatedBy],[CreatedDate],Remarks,CostPrice,Barcode)
//     VALUES
//           ('"+aSales.BranchId+"','" + aSales.ID + "','" + dr["ID"].ToString() + "','" + dr["SPrice"].ToString().Replace(",", "") + "','" +
//                                          dr["Tax"].ToString().Replace(",", "") + "','" +
//                                          dr["DiscountAmount"].ToString().Replace(",", "") + "','" +
//                                          dr["SPrice"].ToString().Replace(",", "") + "','" +
//                                          dr["Qty"].ToString().Replace(",", "") + "','" +
//                                          dr["Total"].ToString().Replace(",", "") + "','" + aSales.LoginBy +
//                                          "',GETDATE(),'" +
//                                          dr["Remarks"].ToString().Replace("'", "") + "','" +
//                                          dr["CostPrice"].ToString().Replace(",", "") + "','" + dr["Barcode"].ToString() + "')";
//                    command.ExecuteNonQuery();
//                    PurchasePrice = +Convert.ToDecimal(dr["CostPrice"].ToString().Replace(",", ""));
//                }

//            }

//            //foreach (DataRow drPay in dtPayment.Rows)
//            //{
//            //    if (!string.IsNullOrEmpty(drPay["PaymentypeID"].ToString()))
//            //    {
//            //        command.CommandText =
//            //            "insert into Order_Payment(MstId, PaymentypeId, PaymentypeIdFrom, Paymentype, PaymentypeFrom, Amount, AddBy, AddDate,[BankID],[BankName] ,                                  [AccountNo],[AccountID],[CardOrCheeckNo],[BankIDFrom],[BankNameFrom],[AccountNoFrom],[Status],[ApprovedDate]) values('" +
//            //            aSales.ID + "','" + drPay["PaymentypeId"].ToString() + "','" +
//            //            drPay["PaymentypeIdFrom"].ToString() + "','" + drPay["Paymentype"].ToString() + "','" +
//            //            drPay["PaymentypeFrom"].ToString() + "','" + drPay["Amount"].ToString() + "','" +
//            //            aSales.LoginBy + "',GETDATE(),'" + drPay["BankID"].ToString() + "','" +
//            //            drPay["BankName"].ToString() + "','" + drPay["AccountNo"].ToString() + "','" +
//            //            drPay["AccountID"].ToString() + "'," +
//            //            "'" + drPay["CardOrCheeckNo"].ToString() + "','" + drPay["BankIDFrom"].ToString() + "','" +
//            //            drPay["BankNameFrom"].ToString() + "','" + drPay["AccountNoFrom"].ToString() + "','" +
//            //            drPay["Status"].ToString() + "',convert(date,'" + drPay["ApprovedDate"].ToString() + "',103))";
//            //        command.ExecuteNonQuery();
//            //    }
//            //}

//            if (Convert.ToDouble(aSales.CReceive) > 0)
//            {
//                command.CommandText = @"select count(*) from  [CustomerPaymentReceive] where [Invoice]='" + aSales.ID + "' and BranchId='"+aSales.BranchId+"'";
//                int CheckDues = Convert.ToInt32(command.ExecuteScalar());
//                command.CommandType = CommandType.Text;

//                if (CheckDues > 0)
//                {
//                    command.CommandText = @"UPDATE [dbo].[CustomerPaymentReceive]
//                       SET [Date] = convert(date,'" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103),[Customer_id] ='" +
//                                          aSales.Customer + "',[PayAmt] ='" + (Convert.ToDecimal(aSales.CReceive) - Convert.ToDecimal(aSales.CRefund)) + "' ,[update_by] ='" +
//                                          aSales.LoginBy + "' ,[update_date] = GETDATE(),PayType='" + aSales.paymentID + "',Bank_id='"+aSales.BankId+"',BankBranchId ='"+aSales.PMNumber+"' WHERE [Invoice]='" +
//                                          aSales.ID + "' and BranchId='"+aSales.BranchId+"'";
//                    command.ExecuteNonQuery();
//                }
//                else
//                {
//                    command.CommandText = @"INSERT INTO [dbo].[CustomerPaymentReceive]
//           (BranchId,[Date],[Customer_id],[Invoice],[PayAmt],[entry_by],[entry_date],PayType,Bank_id,BankBranchId )
//     VALUES
//           (convert(datetime, nullif( '" + aSales.BranchId+"','" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),''),''), 103),'" + aSales.Customer + "','" + aSales.ID +
//                                          "','" + (Convert.ToDecimal(aSales.CReceive) - Convert.ToDecimal(aSales.CRefund)) + "','" + aSales.LoginBy + "',GETDATE(),'"+aSales.paymentID+"','"+aSales.BankId+"','"+aSales.PMNumber+"')";
//                    command.ExecuteNonQuery();
//                }
//            }

//            //********************* Sales Total (Show Purchase Price) *********//

//            //command.CommandText = "SP_PV_UnitPrice_All";
//            //command.CommandType = CommandType.StoredProcedure;
//            //command.Parameters.AddWithValue("@MstID", Convert.ToInt32(aSales.ID));
//            //command.Parameters.AddWithValue("@Rate", Convert.ToDecimal(CurrencyRate));
//            //PurchasePrice = Convert.ToDecimal(command.ExecuteScalar());

//            //************************ Account Code ************************//

//            double totVat = 0, Discount = 0;
//            totVat = (Convert.ToDouble(aSales.Total) * Convert.ToDouble(aSales.Tax)) / 100;
//            Discount = Convert.ToDouble(aSales.Disount);
//            string tot = (Convert.ToDouble(aSales.Total) - Discount).ToString();
//            _aVouchMst.ControlAmt = (Convert.ToDouble(aSales.Total) - Discount).ToString();

//            command.CommandType = CommandType.Text;
//            command.CommandText = VouchManager.SaveVoucherMst(_aVouchMst, 2);
//            command.ExecuteNonQuery();

//            command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" + _aVouchMst.VchSysNo +
//                                  "')";
//            command.ExecuteNonQuery();

//            VouchDtl vdtl;
//            for (int j = 0; j < 3; j++)
//            {
//                if (j == 0)
//                {
//                    //DataRow 
//                    vdtl = new VouchDtl();
//                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
//                    vdtl.ValueDate = aSales.Date;
//                    vdtl.LineNo = "1";
//                    vdtl.GlCoaCode = dtFixCode.Rows[0]["PHSalesInvoice"].ToString(); //**** Sales Code *******//
//                    vdtl.AccType =
//                        VouchManager.getAccType(dtFixCode.Rows[0]["PHSalesInvoice"]
//                            .ToString()); //**** Sales Code *******//
//                    vdtl.Particulars = dtFixCode.Rows[0]["PHSalesInvoiceDesc"].ToString();
//                    vdtl.AmountDr = "0";
//                    vdtl.AmountCr = _aVouchMst.ControlAmt.Replace(",", "");
//                    ;
//                    vdtl.Status = _aVouchMst.Status;
//                    vdtl.BookName = _aVouchMst.BookName;
//                    vdtl.AUTHO_USER = _aVouchMst.EntryUser;
//                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
//                }
//                else if (j == 1)
//                {
//                    vdtl = new VouchDtl();
//                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
//                    vdtl.ValueDate = aSales.Date;
//                    vdtl.LineNo = "2";
//                    vdtl.GlCoaCode = "1-" + aSales.CustomerCoa;
//                    vdtl.Particulars = aSales.CustomerName;
//                    vdtl.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
//                    vdtl.AmountDr = _aVouchMst.ControlAmt.Replace(",", "");
//                    vdtl.AmountCr = "0";
//                    vdtl.Status = _aVouchMst.Status;
//                    vdtl.BookName = _aVouchMst.BookName;
//                    vdtl.AUTHO_USER = _aVouchMst.EntryUser;
//                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
//                }
//                else if (j == 2)
//                {
//                    vdtl = new VouchDtl();
//                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
//                    vdtl.ValueDate = aSales.Date;
//                    vdtl.LineNo = "3";
//                    vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
//                    vdtl.Particulars = "Closing Stock";
//                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
//                    vdtl.AmountDr = "0";
//                    vdtl.AmountCr = PurchasePrice.ToString().Replace(",", "");
//                    vdtl.AUTHO_USER = "CS";
//                    vdtl.Status = _aVouchMst.Status;
//                    vdtl.BookName = _aVouchMst.BookName;
//                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
//                }
//            }

//            //***************************  Credit Voucher ********************************// 
//            if (Convert.ToDecimal(aSales.CReceive) > 0)
//            {
//                if (_aVouchMstCR.RefFileNo == "New")
//                {
//                    _aVouchMstCR.ControlAmt = aSales.CReceive.Replace("'", "").Replace(",", "");
//                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 1);
//                    command.ExecuteNonQuery();

//                    VouchDtl vdtlCR;
//                    for (int j = 0; j < 2; j++)
//                    {
//                        if (j == 0)
//                        {
//                            //DataRow 
//                            vdtlCR = new VouchDtl();
//                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
//                            vdtlCR.ValueDate = aSales.Date;
//                            vdtlCR.LineNo = "1";
//                            vdtlCR.GlCoaCode = "1-" + aSales.CustomerCoa;
//                            vdtlCR.Particulars = aSales.CustomerName;
//                            vdtlCR.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
//                            vdtlCR.AmountDr = "0";
//                            vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
//                            vdtlCR.Status = _aVouchMstCR.Status;
//                            vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

//                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
//                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
//                        }
//                        else if (j == 1)
//                        {
//                            vdtlCR = new VouchDtl();
//                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
//                            vdtlCR.ValueDate = aSales.Date;
//                            vdtlCR.LineNo = "2";
//                            if (string.IsNullOrEmpty(aSales.BankCoaCode))
//                            {
//                                vdtlCR.GlCoaCode =
//                                    "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
//                                vdtlCR.AccType =
//                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
//                                                                .ToString()); //**** SalesCode *******//
//                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
//                            }
//                            else
//                            {
//                                vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
//                                vdtlCR.AccType =
//                                    VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
//                                vdtlCR.Particulars = aSales.BankName;
//                            }

//                            vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
//                            vdtlCR.AmountCr = "0";
//                            vdtlCR.Status = _aVouchMstCR.Status;
//                            vdtlCR.BookName = _aVouchMstCR.BookName;
//                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
//                            //*********** Convert Rate ********//
//                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
//                        }
//                    }
//                }
//                else
//                {

//                    _aVouchMstCR.ControlAmt = aSales.CReceive.Replace("'", "").Replace(",", "");
//                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 2);
//                    command.ExecuteNonQuery();
//                    command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
//                                          _aVouchMstCR.VchSysNo + "')";
//                    command.ExecuteNonQuery();

//                    VouchDtl vdtlCR;
//                    for (int j = 0; j < 3; j++)
//                    {
//                        if (j == 0)
//                        {
//                            //DataRow 
//                            vdtlCR = new VouchDtl();
//                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
//                            vdtlCR.ValueDate = aSales.Date;
//                            vdtlCR.LineNo = "1";
//                            vdtlCR.GlCoaCode = "1-" + aSales.CustomerCoa;
//                            vdtlCR.Particulars = aSales.CustomerName;
//                            vdtlCR.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
//                            vdtlCR.AmountDr = "0";
//                            vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
//                            vdtlCR.Status = _aVouchMstCR.Status;
//                            vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

//                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
//                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
//                        }
//                        else if (j == 1)
//                        {
//                            vdtlCR = new VouchDtl();
//                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
//                            vdtlCR.ValueDate = aSales.Date;
//                            vdtlCR.LineNo = "2";
//                            if (string.IsNullOrEmpty(aSales.BankCoaCode))
//                            {
//                                vdtlCR.GlCoaCode =
//                                    "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
//                                vdtlCR.AccType =
//                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
//                                                                .ToString()); //**** SalesCode *******//
//                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
//                            }
//                            else
//                            {
//                                vdtlCR.GlCoaCode = "1-" + aSales.BankId; //**** SalesCode *******//
//                                vdtlCR.AccType =
//                                    VouchManager.getAccType("1-" + aSales.BankId); //**** SalesCode *******//
//                                vdtlCR.Particulars = aSales.BankName;
//                            }

//                            vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
//                            vdtlCR.AmountCr = "0";
//                            vdtlCR.Status = _aVouchMstCR.Status;
//                            vdtlCR.BookName = _aVouchMstCR.BookName;
//                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
//                            //*********** Convert Rate ********//
//                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
//                        }
//                    }
//                }
//            }

//            //***************************  Tax ********************************// 
//            if (Convert.ToDecimal(aSales.Tax) > 0)
//            {
//                if (_aVouchMstTax.RefFileNo == "New")
//                {
//                    _aVouchMstTax.ControlAmt = totVat.ToString().Replace("'", "").Replace(",", "");
//                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstTax, 1);
//                    command.ExecuteNonQuery();

//                    VouchDtl vdtlTax;
//                    for (int j = 0; j < 3; j++)
//                    {
//                        if (j == 0)
//                        {
//                            //DataRow 
//                            vdtlTax = new VouchDtl();
//                            vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
//                            vdtlTax.ValueDate = aSales.Date;
//                            vdtlTax.LineNo = "1";
//                            vdtlTax.GlCoaCode = "1-" + aSales.CustomerCoa;
//                            vdtlTax.Particulars = aSales.CustomerName;
//                            vdtlTax.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
//                            vdtlTax.AmountDr = "0";
//                            vdtlTax.AmountCr = totVat.ToString().Replace("'", "").Replace(",", "");
//                            vdtlTax.Status = _aVouchMstTax.Status;
//                            vdtlTax.BookName = _aVouchMstTax.BookName; //*********** Convert Rate ********//

//                            vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
//                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
//                        }
//                        else if (j == 1)
//                        {
//                            vdtlTax = new VouchDtl();
//                            vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
//                            vdtlTax.ValueDate = aSales.Date;
//                            vdtlTax.LineNo = "2";
//                            vdtlTax.GlCoaCode =
//                                dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
//                            vdtlTax.AccType =
//                                VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
//                                    .ToString()); //**** Purchase Code *******//
//                            vdtlTax.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
//                            vdtlTax.AmountDr = totVat.ToString().Replace("'", "").Replace(",", "");
//                            vdtlTax.AmountCr = "0";
//                            vdtlTax.Status = _aVouchMstTax.Status;
//                            vdtlTax.BookName = _aVouchMstTax.BookName;
//                            vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
//                            //*********** Convert Rate ********//
//                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
//                        }
//                    }
//                }
//                else
//                {
//                    _aVouchMstTax.ControlAmt = totVat.ToString().Replace("'", "").Replace(",", "");
//                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstTax, 2);
//                    command.ExecuteNonQuery();
//                    command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
//                                          _aVouchMstTax.VchSysNo + "')";
//                    command.ExecuteNonQuery();
//                    VouchDtl vdtlTax;
//                    for (int j = 0; j < 3; j++)
//                    {
//                        if (j == 0)
//                        {
//                            //DataRow 
//                            vdtlTax = new VouchDtl();
//                            vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
//                            vdtlTax.ValueDate = aSales.Date;
//                            vdtlTax.LineNo = "1";
//                            vdtlTax.GlCoaCode = "1-" + aSales.CustomerCoa;
//                            vdtlTax.Particulars = aSales.CustomerName;
//                            vdtlTax.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
//                            vdtlTax.AmountDr = "0";
//                            vdtlTax.AmountCr = totVat.ToString().Replace("'", "").Replace(",", "");
//                            vdtlTax.Status = _aVouchMstTax.Status;
//                            vdtlTax.BookName = _aVouchMstTax.BookName; //*********** Convert Rate ********//

//                            vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
//                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
//                        }
//                        else if (j == 1)
//                        {
//                            vdtlTax = new VouchDtl();
//                            vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
//                            vdtlTax.ValueDate = aSales.Date;
//                            vdtlTax.LineNo = "2";
//                            vdtlTax.GlCoaCode =
//                                dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
//                            vdtlTax.AccType =
//                                VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
//                                    .ToString()); //**** Purchase Code *******//
//                            vdtlTax.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
//                            vdtlTax.AmountDr = totVat.ToString().Replace("'", "").Replace(",", "");
//                            vdtlTax.AmountCr = "0";
//                            vdtlTax.Status = _aVouchMstTax.Status;
//                            vdtlTax.BookName = _aVouchMstTax.BookName;
//                            vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
//                            //*********** Convert Rate ********//
//                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
//                        }
//                    }
//                }
//            }

//            command.CommandText = "update FixGlCoaCode set PrintOrderID='" + aSales.ID + "' ";
//            command.ExecuteNonQuery();

//            transaction.Commit();
//        }
//        catch (Exception ex)
//        {
//            throw new Exception(ex.Message);
//        }
//        finally
//        {
//            if (connection.State == ConnectionState.Open)
//                connection.Close();
//        }
//    }
    public static void UpdateSalesInfo(Sales aSales, DataTable dt, DataTable dtOldSalesDetails, DataTable dtPayment, VouchMst _aVouchMst, VouchMst _aVouchMstCR, VouchMst _aVouchMstTax)
    {
        decimal PurchasePrice = 0;
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            string FldPaymentMethodID = "",
                FldPaymentMethodNumber = "",
                FldBankId = "",
                FldChequeDate = "",
                FldOrderStatusID = "",
              FldExtraAmount = ",ExtraAmount='" + Convert.ToDecimal(aSales.ExtraAmount) + "'";


            if (!string.IsNullOrEmpty(aSales.BankId))
            {
                FldPaymentMethodNumber = ",PaymentMethodNumber='" + aSales.PMNumber + "' ";
                FldBankId = ",BankId='" + aSales.BankId + "' ";
                FldChequeDate = ",ChequeDate=convert(date,'" + aSales.ChequeDate + "',103) ";
                FldOrderStatusID = ",OrderStatusID='" + aSales.Chk_Status + "'";
            }


            command.CommandText = @"update [Order] set
           [SubTotal]='" + aSales.Total + "',BranchId='" + aSales.BranchId + "',[TaxAmount]='" + aSales.Tax + "',ExchangeAmount='" + aSales.ExchangeAmount + "',[DiscountAmount]='" + aSales.Disount +
                                  "',[GrandTotal]='" + aSales.GTotal + "',[CashReceived]='" + aSales.CReceive +
                                  "',[CashRefund]='" + aSales.CRefund + "',[OrderDate]=convert(date,'" + aSales.Date +
                                  "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103),[CustomerID]='" + aSales.Customer + "',[Due]='" + Convert.ToDecimal(aSales.Due) +
                                  "',[DeliveryStatus]='" + aSales.DvStatus + "',[DeliveryDate]=convert(date,'" +
                                  aSales.DvDate + "',103),[Remark]='" + aSales.Remarks + "',[ModifiedBy]='" +
                                  aSales.LoginBy + "',[ModifiedDate]=GETDATE(),PaymentMethodID='" + aSales.PMethod +
                                  "',LocalCustomer='" + aSales.LocalCustomer + "',LocalCusAddress='" +
                                  aSales.LocalCusAddress + "',LocalCusPhone='" + aSales.LocalCusPhone + "',Note='" +
                                  aSales.Note + "' " +
                                  FldPaymentMethodNumber + FldBankId + FldOrderStatusID + FldChequeDate + FldExtraAmount +
                                  " where ID='" + aSales.ID + "' and BranchId='" + aSales.BranchId + "'";
            command.ExecuteNonQuery();

            foreach (DataRow drOld in dtOldSalesDetails.Rows)
            {
                command.CommandText = @"delete from [OrderDetail] where ID='" + drOld["Dtl_ID"].ToString() +
                                      "' and OrderID='" + aSales.ID + "' and BranchId='" + aSales.BranchId + "'";
                command.ExecuteNonQuery();
            }
            command.CommandText = @"DELETE FROM [Order_Payment] WHERE  MstId='" + aSales.ID + "'and BranchId='" + aSales.BranchId + "' ";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Code"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [OrderDetail]
           (BranchId,[OrderID],[ItemID],[UnitPrice]  ,[TaxRate] ,[DiscountAmount] ,[SalePrice] ,[Quantity] ,[TotalPrice] ,[CreatedBy],[CreatedDate],Remarks,CostPrice,Barcode)
     VALUES
           ('" + aSales.BranchId + "','" + aSales.ID + "','" + dr["ID"].ToString() + "','" + dr["SPrice"].ToString().Replace(",", "") + "','" +
                                          dr["Tax"].ToString().Replace(",", "") + "','" +
                                          dr["DiscountAmount"].ToString().Replace(",", "") + "','" +
                                          dr["SPrice"].ToString().Replace(",", "") + "','" +
                                          dr["Qty"].ToString().Replace(",", "") + "','" +
                                          dr["Total"].ToString().Replace(",", "") + "','" + aSales.LoginBy +
                                          "',GETDATE(),'" +
                                          dr["Remarks"].ToString().Replace("'", "") + "','" +
                                          dr["CostPrice"].ToString().Replace(",", "") + "','" + dr["Barcode"].ToString() + "')";
                    command.ExecuteNonQuery();
                    PurchasePrice += (Convert.ToDecimal(dr["CostPrice"].ToString().Replace(",", "")) * Convert.ToDecimal(dr["Qty"].ToString().Replace(",", "")));
                }

            }

            foreach (DataRow drPay in dtPayment.Rows)
            {
                if (!string.IsNullOrEmpty(drPay["PaymentypeID"].ToString()))
                {
                    command.CommandText =
                        "insert into Order_Payment(BranchId,MstId, PaymentypeId, PaymentypeIdFrom, Paymentype, PaymentypeFrom, Amount, AddBy, AddDate,[BankID],[BankName],[AccountNo],[AccountID],[CardOrCheeckNo],[BankIDFrom],[BankNameFrom],[AccountNoFrom],[Status],[ApprovedDate]) values('" + aSales.BranchId + "','" +
                        aSales.ID + "','" + drPay["PaymentypeId"].ToString() + "','" +
                        drPay["PaymentypeIdFrom"].ToString() + "','" + drPay["Paymentype"].ToString() + "','" +
                        drPay["PaymentypeFrom"].ToString() + "','" + drPay["Amount"].ToString() + "','" +
                        aSales.LoginBy + "',GETDATE(),'" + drPay["BankID"].ToString() + "','" +
                        drPay["BankName"].ToString() + "','" + drPay["AccountNo"].ToString() + "','" +
                        drPay["AccountID"].ToString() + "'," +
                        "'" + drPay["CardOrCheeckNo"].ToString() + "','" + drPay["BankIDFrom"].ToString() + "','" +
                        drPay["BankNameFrom"].ToString() + "','" + drPay["AccountNoFrom"].ToString() + "','" +
                        drPay["Status"].ToString() + "',convert(date,'" + drPay["ApprovedDate"].ToString() + "',103))";
                    command.ExecuteNonQuery();
                }
            }

            if (Convert.ToDouble(aSales.CReceive) > 0)
            {
                command.CommandText = @"select count(*) from  [CustomerPaymentReceive] where [Invoice]='" + aSales.ID + "' and BranchId='" + aSales.BranchId + "'";
                int CheckDues = Convert.ToInt32(command.ExecuteScalar());
                command.CommandType = CommandType.Text;

                if (CheckDues > 0)
                {
                    command.CommandText = @"UPDATE [dbo].[CustomerPaymentReceive]
                       SET [Date] = convert(date,'" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103),[Customer_id] ='" +
                                          aSales.Customer + "',[PayAmt] ='" + (Convert.ToDecimal(aSales.CReceive) - Convert.ToDecimal(aSales.CRefund)) + "' ,[update_by] ='" +
                                          aSales.LoginBy + "' ,[update_date] = GETDATE() WHERE [Invoice]='" +
                                          aSales.ID + "' and BranchId='" + aSales.BranchId + "'";
                    command.ExecuteNonQuery();
                }
                else
                {
//                    command.CommandText = @"INSERT INTO [dbo].[CustomerPaymentReceive]
//           (BranchId,[Date],[Customer_id],[Invoice],[PayAmt],[entry_by],[entry_date])
//     VALUES
//           (convert(datetime, nullif( '" + aSales.BranchId + "','" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),''),''), 103),'" + aSales.Customer + "','" + aSales.ID +
//                                          "','" + (Convert.ToDecimal(aSales.CReceive) - Convert.ToDecimal(aSales.CRefund)) + "','" + aSales.LoginBy + "',GETDATE())";
//                    command.ExecuteNonQuery();



                    command.CommandText = @"INSERT INTO [dbo].[CustomerPaymentReceive]
           ([Date],BranchId,[Customer_id],[Invoice],[PayAmt],[entry_by],[entry_date])
     VALUES
           (convert(datetime, nullif( '" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),''), 103),'" + aSales.BranchId + "','" + aSales.Customer + "','" + aSales.ID +
                                     "','" + (Convert.ToDecimal(aSales.CReceive) - Convert.ToDecimal(aSales.CRefund)) + "','" + aSales.LoginBy + "',GETDATE())";
                    command.ExecuteNonQuery();
                }
            }

            //********************* Sales Total (Show Purchase Price) *********//

            //command.CommandText = "SP_PV_UnitPrice_All";
            //command.CommandType = CommandType.StoredProcedure;
            //command.Parameters.AddWithValue("@MstID", Convert.ToInt32(aSales.ID));
            //command.Parameters.AddWithValue("@Rate", Convert.ToDecimal(CurrencyRate));
            //PurchasePrice = Convert.ToDecimal(command.ExecuteScalar());

            //************************ Account Code ************************//

            //double totVat = 0, Discount = 0;
            //totVat = ((Convert.ToDouble(aSales.Total) - Convert.ToDouble(aSales.Disount)) * Convert.ToDouble(aSales.Tax)) / 100;
            //Discount = Convert.ToDouble(aSales.Disount);
            //string tot = (Convert.ToDouble(aSales.Total) - Discount).ToString();
            //_aVouchMst.ControlAmt = aSales.GTotal;

            //command.CommandType = CommandType.Text;
            //command.CommandText = VouchManager.SaveVoucherMst(_aVouchMst, 2);
            //command.ExecuteNonQuery();

            //command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" + _aVouchMst.VchSysNo +
            //                      "')";
            //command.ExecuteNonQuery();

            //VouchDtl vdtl;
            //for (int j = 0; j < 4; j++)
            //{
            //    if (j == 0)
            //    {
            //        //DataRow 
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "1";
            //        vdtl.GlCoaCode = dtFixCode.Rows[0]["PHSalesInvoice"].ToString(); //**** Sales Code *******//
            //        vdtl.AccType =
            //        VouchManager.getAccType(dtFixCode.Rows[0]["PHSalesInvoice"].ToString()); //**** Sales Code *******//
            //        vdtl.Particulars = dtFixCode.Rows[0]["PHSalesInvoiceDesc"].ToString();
            //        vdtl.AmountDr = "0";
            //        vdtl.AmountCr = aSales.GTotal;
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        vdtl.AUTHO_USER = _aVouchMst.EntryUser;
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
            //    }
            //    else if (j == 1)
            //    {
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "2";
            //        vdtl.GlCoaCode = "1-" + aSales.CustomerCoa;
            //        vdtl.Particulars = aSales.CustomerName;
            //        vdtl.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //        vdtl.AmountDr = _aVouchMst.ControlAmt.Replace(",", "");
            //        vdtl.AmountCr = "0";
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        vdtl.AUTHO_USER = _aVouchMst.EntryUser;
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
            //    }
            //    else if (j == 2)
            //    {
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "3";
            //        vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
            //        vdtl.Particulars = "Closing Stock";
            //        vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
            //        vdtl.AmountDr = "0";
            //        vdtl.AmountCr = PurchasePrice.ToString().Replace(",", "");
            //        vdtl.AUTHO_USER = "CS";
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);

            //    }

            //    else if (j == 3)
            //    {
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "4";
            //        vdtl.GlCoaCode = "1-" + dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
            //        vdtl.AccType =
            //                VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
            //                    .ToString()); //**** Purchase Code *******//
            //        vdtl.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
            //        vdtl.AmountDr = "0";
            //        vdtl.AmountCr = totVat.ToString().Replace("'", "").Replace(",", ""); ;
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        vdtl.AUTHO_USER = _aVouchMst.EntryUser;
            //        //*********** Convert Rate ********//
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
            //    }
            //}

            //***************************  Credit Voucher ********************************// 
            if (Convert.ToDecimal(aSales.CReceive) > 0)
            {
                _aVouchMstCR.ControlAmt = aSales.CReceive.Replace("'", "").Replace(",", "");
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
                        vdtlCR.ValueDate = aSales.Date;
                        vdtlCR.LineNo = "1";
                        vdtlCR.GlCoaCode = "1-" + aSales.CustomerCoa;
                        vdtlCR.Particulars = aSales.CustomerName;
                        vdtlCR.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
                        vdtlCR.AmountDr = "0";
                        vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
                        vdtlCR.Status = _aVouchMstCR.Status;
                        vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

                        vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                    }
                    else if (j == 1)
                    {
                        int LineNumber = 2;
                        foreach (DataRow dr1 in dtPayment.Rows)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                            vdtlCR.ValueDate = aSales.Date;
                            vdtlCR.LineNo = LineNumber.ToString();
                            if (string.IsNullOrEmpty(dr1["AccountId"].ToString()) || dr1["AccountId"].ToString()=="0")
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                var GlCoaCode = IdManager.GetShowSingleValueString("Gl_Code", "Id", "bank_branch",
                                    dr1["AccountId"].ToString());


                                vdtlCR.GlCoaCode = "1-" + GlCoaCode; //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + GlCoaCode); //**** SalesCode *******//
                                vdtlCR.Particulars = aSales.BankName;
                            }

                            //vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
                            vdtlCR.AmountDr = dr1["Amount"].ToString();
                            vdtlCR.AmountCr = "0";
                            vdtlCR.Status = _aVouchMstCR.Status;
                            vdtlCR.BookName = _aVouchMstCR.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                            LineNumber++;
                        }
                    }
                }
            }







            //VouchDtl vdtl;
            //for (int j = 0; j < 3; j++)
            //{
            //    if (j == 0)
            //    {
            //        //DataRow 
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "1";
            //        vdtl.GlCoaCode = dtFixCode.Rows[0]["PHSalesInvoice"].ToString(); //**** Sales Code *******//
            //        vdtl.AccType =
            //            VouchManager.getAccType(dtFixCode.Rows[0]["PHSalesInvoice"]
            //                .ToString()); //**** Sales Code *******//
            //        vdtl.Particulars = dtFixCode.Rows[0]["PHSalesInvoiceDesc"].ToString();
            //        vdtl.AmountDr = "0";
            //        vdtl.AmountCr = _aVouchMst.ControlAmt.Replace(",", "");
            //        ;
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        vdtl.AUTHO_USER = _aVouchMst.EntryUser;
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
            //    }
            //    else if (j == 1)
            //    {
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "2";
            //        vdtl.GlCoaCode = "1-" + aSales.CustomerCoa;
            //        vdtl.Particulars = aSales.CustomerName;
            //        vdtl.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //        vdtl.AmountDr = _aVouchMst.ControlAmt.Replace(",", "");
            //        vdtl.AmountCr = "0";
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        vdtl.AUTHO_USER = _aVouchMst.EntryUser;
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
            //    }
            //    else if (j == 2)
            //    {
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "3";
            //        vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
            //        vdtl.Particulars = "Closing Stock";
            //        vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
            //        vdtl.AmountDr = "0";
            //        vdtl.AmountCr = PurchasePrice.ToString().Replace(",", "");
            //        vdtl.AUTHO_USER = "CS";
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
            //    }
            //}

            ////***************************  Credit Voucher ********************************// 
            //if (Convert.ToDecimal(aSales.CReceive) > 0)
            //{
            //    if (_aVouchMstCR.RefFileNo == "New")
            //    {
            //        _aVouchMstCR.ControlAmt = aSales.CReceive.Replace("'", "").Replace(",", "");
            //        command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 1);
            //        command.ExecuteNonQuery();

            //        VouchDtl vdtlCR;
            //        for (int j = 0; j < 2; j++)
            //        {
            //            if (j == 0)
            //            {
            //                //DataRow 
            //                vdtlCR = new VouchDtl();
            //                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
            //                vdtlCR.ValueDate = aSales.Date;
            //                vdtlCR.LineNo = "1";
            //                vdtlCR.GlCoaCode = "1-" + aSales.CustomerCoa;
            //                vdtlCR.Particulars = aSales.CustomerName;
            //                vdtlCR.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //                vdtlCR.AmountDr = "0";
            //                vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
            //                vdtlCR.Status = _aVouchMstCR.Status;
            //                vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

            //                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
            //            }
            //            else if (j == 1)
            //            {
            //                int LineNumber = 2;
            //                foreach (DataRow dr1 in dtPayment.Rows)
            //                {
            //                    vdtlCR = new VouchDtl();
            //                    vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
            //                    vdtlCR.ValueDate = aSales.Date;
            //                    vdtlCR.LineNo = LineNumber.ToString();
            //                    if (string.IsNullOrEmpty(dr1["AccountId"].ToString()))
            //                    {
            //                        vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
            //                        vdtlCR.AccType =
            //                            VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
            //                                .ToString()); //**** SalesCode *******//
            //                        vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
            //                    }
            //                    else
            //                    {
            //                        var GlCoaCode = IdManager.GetShowSingleValueString("Gl_Code", "Id", "bank_branch",
            //                            dr1["AccountId"].ToString());


            //                        vdtlCR.GlCoaCode = "1-" + GlCoaCode; //**** SalesCode *******//
            //                        vdtlCR.AccType =
            //                            VouchManager.getAccType("1-" + GlCoaCode); //**** SalesCode *******//
            //                        vdtlCR.Particulars = aSales.BankName;
            //                    }

            //                    //vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
            //                    vdtlCR.AmountDr = dr1["Amount"].ToString();
            //                    vdtlCR.AmountCr = "0";
            //                    vdtlCR.Status = _aVouchMstCR.Status;
            //                    vdtlCR.BookName = _aVouchMstCR.BookName;
            //                    vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
            //                    //*********** Convert Rate ********//
            //                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
            //                    LineNumber++;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {

            //        _aVouchMstCR.ControlAmt = aSales.CReceive.Replace("'", "").Replace(",", "");
            //        command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 2);
            //        command.ExecuteNonQuery();
            //        command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
            //                              _aVouchMstCR.VchSysNo + "')";
            //        command.ExecuteNonQuery();

            //        VouchDtl vdtlCR;
            //        for (int j = 0; j < 3; j++)
            //        {
            //            if (j == 0)
            //            {
            //                //DataRow 
            //                vdtlCR = new VouchDtl();
            //                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
            //                vdtlCR.ValueDate = aSales.Date;
            //                vdtlCR.LineNo = "1";
            //                vdtlCR.GlCoaCode = "1-" + aSales.CustomerCoa;
            //                vdtlCR.Particulars = aSales.CustomerName;
            //                vdtlCR.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //                vdtlCR.AmountDr = "0";
            //                vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
            //                vdtlCR.Status = _aVouchMstCR.Status;
            //                vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

            //                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
            //            }
            //            else if (j == 1)
            //            {
            //                vdtlCR = new VouchDtl();
            //                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
            //                vdtlCR.ValueDate = aSales.Date;
            //                vdtlCR.LineNo = "2";
            //                if (string.IsNullOrEmpty(aSales.BankCoaCode))
            //                {
            //                    vdtlCR.GlCoaCode =
            //                        "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
            //                    vdtlCR.AccType =
            //                        VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
            //                                                    .ToString()); //**** SalesCode *******//
            //                    vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
            //                }
            //                else
            //                {
            //                    vdtlCR.GlCoaCode = "1-" + aSales.BankId; //**** SalesCode *******//
            //                    vdtlCR.AccType =
            //                        VouchManager.getAccType("1-" + aSales.BankId); //**** SalesCode *******//
            //                    vdtlCR.Particulars = aSales.BankName;
            //                }

            //                vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
            //                vdtlCR.AmountCr = "0";
            //                vdtlCR.Status = _aVouchMstCR.Status;
            //                vdtlCR.BookName = _aVouchMstCR.BookName;
            //                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
            //                //*********** Convert Rate ********//
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
            //            }
            //        }
            //    }
            //}

            ////***************************  Tax ********************************// 
            //if (Convert.ToDecimal(aSales.Tax) > 0)
            //{
            //    if (_aVouchMstTax.RefFileNo == "New")
            //    {
            //        _aVouchMstTax.ControlAmt = totVat.ToString().Replace("'", "").Replace(",", "");
            //        command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstTax, 1);
            //        command.ExecuteNonQuery();

            //        VouchDtl vdtlTax;
            //        for (int j = 0; j < 3; j++)
            //        {
            //            if (j == 0)
            //            {
            //                //DataRow 
            //                vdtlTax = new VouchDtl();
            //                vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
            //                vdtlTax.ValueDate = aSales.Date;
            //                vdtlTax.LineNo = "1";
            //                vdtlTax.GlCoaCode = "1-" + aSales.CustomerCoa;
            //                vdtlTax.Particulars = aSales.CustomerName;
            //                vdtlTax.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //                vdtlTax.AmountDr = "0";
            //                vdtlTax.AmountCr = totVat.ToString().Replace("'", "").Replace(",", "");
            //                vdtlTax.Status = _aVouchMstTax.Status;
            //                vdtlTax.BookName = _aVouchMstTax.BookName; //*********** Convert Rate ********//

            //                vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
            //            }
            //            else if (j == 1)
            //            {
            //                vdtlTax = new VouchDtl();
            //                vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
            //                vdtlTax.ValueDate = aSales.Date;
            //                vdtlTax.LineNo = "2";
            //                vdtlTax.GlCoaCode =
            //                    dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
            //                vdtlTax.AccType =
            //                    VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
            //                        .ToString()); //**** Purchase Code *******//
            //                vdtlTax.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
            //                vdtlTax.AmountDr = totVat.ToString().Replace("'", "").Replace(",", "");
            //                vdtlTax.AmountCr = "0";
            //                vdtlTax.Status = _aVouchMstTax.Status;
            //                vdtlTax.BookName = _aVouchMstTax.BookName;
            //                vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
            //                //*********** Convert Rate ********//
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        _aVouchMstTax.ControlAmt = totVat.ToString().Replace("'", "").Replace(",", "");
            //        command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstTax, 2);
            //        command.ExecuteNonQuery();
            //        command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
            //                              _aVouchMstTax.VchSysNo + "')";
            //        command.ExecuteNonQuery();
            //        VouchDtl vdtlTax;
            //        for (int j = 0; j < 3; j++)
            //        {
            //            if (j == 0)
            //            {
            //                //DataRow 
            //                vdtlTax = new VouchDtl();
            //                vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
            //                vdtlTax.ValueDate = aSales.Date;
            //                vdtlTax.LineNo = "1";
            //                vdtlTax.GlCoaCode = "1-" + aSales.CustomerCoa;
            //                vdtlTax.Particulars = aSales.CustomerName;
            //                vdtlTax.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //                vdtlTax.AmountDr = "0";
            //                vdtlTax.AmountCr = totVat.ToString().Replace("'", "").Replace(",", "");
            //                vdtlTax.Status = _aVouchMstTax.Status;
            //                vdtlTax.BookName = _aVouchMstTax.BookName; //*********** Convert Rate ********//

            //                vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
            //            }
            //            else if (j == 1)
            //            {
            //                vdtlTax = new VouchDtl();
            //                vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
            //                vdtlTax.ValueDate = aSales.Date;
            //                vdtlTax.LineNo = "2";
            //                vdtlTax.GlCoaCode =
            //                    dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
            //                vdtlTax.AccType =
            //                    VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
            //                        .ToString()); //**** Purchase Code *******//
            //                vdtlTax.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
            //                vdtlTax.AmountDr = totVat.ToString().Replace("'", "").Replace(",", "");
            //                vdtlTax.AmountCr = "0";
            //                vdtlTax.Status = _aVouchMstTax.Status;
            //                vdtlTax.BookName = _aVouchMstTax.BookName;
            //                vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
            //                //*********** Convert Rate ********//
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
            //            }
            //        }
            //    }
            //}

            command.CommandText = "update FixGlCoaCode set PrintOrderID='" + aSales.ID + "' ";
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


    //******Branch Sales Update/////

    public static void UpdateBranchSalesInfo(Sales aSales, DataTable dt, DataTable dtOldSalesDetails, DataTable dtPayment,
      VouchMst _aVouchMst, VouchMst _aVouchMstCR, VouchMst _aVouchMstTax)
    {
        decimal PurchasePrice = 0;
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            var a = aSales.ExtraAmount;
            string FldPaymentMethodID = "",
                FldPaymentMethodNumber = "",
                FldBankId = "",
                FldChequeDate = "",
                FldOrderStatusID = "",
            FldExtraAmount = ",ExtraAmount='" + Convert.ToDecimal(aSales.ExtraAmount) + "'";


            if (!string.IsNullOrEmpty(aSales.BankId))
            {
                FldPaymentMethodNumber = ",PaymentMethodNumber='" + aSales.PMNumber + "' ";
                FldBankId = ",BankId='" + aSales.BankId + "' ";
                FldChequeDate = ",ChequeDate=convert(date,'" + aSales.ChequeDate + "',103) ";
                FldOrderStatusID = ",OrderStatusID='" + aSales.Chk_Status + "'";
            }


            command.CommandText = @"update [Order] set
           [SubTotal]='" + aSales.Total + "',[TaxAmount]='" + aSales.Tax + "',[DiscountAmount]='" + aSales.Disount +
                                  "',[GrandTotal]='" + aSales.GTotal + "',[CashReceived]='" + aSales.CReceive +
                                  "',[CashRefund]='" + aSales.CRefund + "',[OrderDate]=convert(date,'" + aSales.Date +
                                  "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103),[CustomerID]='" + aSales.Customer + "',[Due]='" + Convert.ToDecimal(aSales.Due) +
                                  "',[DeliveryStatus]='" + aSales.DvStatus + "',[DeliveryDate]=convert(date,'" +
                                  aSales.DvDate + "',103),[Remark]='" + aSales.Remarks + "',[ModifiedBy]='" +
                                  aSales.LoginBy + "',[ModifiedDate]=GETDATE(),PaymentMethodID='" + aSales.PMethod +
                                  "',LocalCustomer='" + aSales.LocalCustomer + "',LocalCusAddress='" +
                                  aSales.LocalCusAddress + "',LocalCusPhone='" + aSales.LocalCusPhone + "',Note='" +
                                  aSales.Note + "' " +
                                  FldPaymentMethodNumber + FldBankId + FldOrderStatusID + FldChequeDate + FldExtraAmount +
                                  " where ID='" + aSales.ID + "' and BranchId='" + aSales.BranchId + "'";
            command.ExecuteNonQuery();

            foreach (DataRow drOld in dtOldSalesDetails.Rows)
            {
                command.CommandText = @"delete from [OrderDetail] where ID='" + drOld["Dtl_ID"].ToString() +
                                      "' and OrderID='" + aSales.ID + "' and BranchId='" + aSales.BranchId + "' ";
                command.ExecuteNonQuery();
            }
            command.CommandText = @"DELETE FROM [Order_Payment] WHERE  MstId='" + aSales.ID + "' and BranchId='" + aSales.BranchId + "'";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Code"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [OrderDetail]
           (BranchId,[OrderID],[ItemID],[UnitPrice]  ,[TaxRate] ,[DiscountAmount] ,[SalePrice] ,[Quantity] ,[TotalPrice] ,[CreatedBy],[CreatedDate],Remarks,CostPrice,Barcode)
     VALUES
           ('" + aSales.BranchId + "','" + aSales.ID + "','" + dr["ID"].ToString() + "','" + dr["SPrice"].ToString().Replace(",", "") + "','" +
                                          dr["Tax"].ToString().Replace(",", "") + "','" +
                                          dr["DiscountAmount"].ToString().Replace(",", "") + "','" +
                                          dr["SPrice"].ToString().Replace(",", "") + "','" +
                                          dr["Qty"].ToString().Replace(",", "") + "','" +
                                          dr["Total"].ToString().Replace(",", "") + "','" + aSales.LoginBy +
                                          "',GETDATE(),'" +
                                          dr["Remarks"].ToString().Replace("'", "") + "','" +
                                          dr["CostPrice"].ToString().Replace(",", "") + "','" + dr["Barcode"].ToString() + "')";
                    command.ExecuteNonQuery();
                    PurchasePrice += (Convert.ToDecimal(dr["CostPrice"].ToString().Replace(",", "")) * Convert.ToDecimal(dr["Qty"].ToString().Replace(",", "")));
                }

            }

            foreach (DataRow drPay in dtPayment.Rows)
            {
                if (!string.IsNullOrEmpty(drPay["PaymentypeID"].ToString()))
                {
                    command.CommandText =
                        "insert into Order_Payment(BranchId,MstId, PaymentypeId, PaymentypeIdFrom, Paymentype, PaymentypeFrom, Amount, AddBy, AddDate,[BankID],[BankName] , [AccountNo],[AccountID],[CardOrCheeckNo],[BankIDFrom],[BankNameFrom],[AccountNoFrom],[Status],[ApprovedDate]) values('" + aSales.BranchId + "','" +
                        aSales.ID + "','" + drPay["PaymentypeId"].ToString() + "','" +
                        drPay["PaymentypeIdFrom"].ToString() + "','" + drPay["Paymentype"].ToString() + "','" +
                        drPay["PaymentypeFrom"].ToString() + "','" + drPay["Amount"].ToString() + "','" +
                        aSales.LoginBy + "',GETDATE(),'" + drPay["BankID"].ToString() + "','" +
                        drPay["BankName"].ToString() + "','" + drPay["AccountNo"].ToString() + "','" +
                        drPay["AccountID"].ToString() + "'," +
                        "'" + drPay["CardOrCheeckNo"].ToString() + "','" + drPay["BankIDFrom"].ToString() + "','" +
                        drPay["BankNameFrom"].ToString() + "','" + drPay["AccountNoFrom"].ToString() + "','" +
                        drPay["Status"].ToString() + "',convert(date,'" + drPay["ApprovedDate"].ToString() + "',103))";
                    command.ExecuteNonQuery();
                }
            }

            if (Convert.ToDouble(aSales.CReceive) > 0)
            {
                command.CommandText = @"select count(*) from  [CustomerPaymentReceive] where [Invoice]='" + aSales.ID + "' and BranchId='" + aSales.BranchId + "'";
                int CheckDues = Convert.ToInt32(command.ExecuteScalar());
                command.CommandType = CommandType.Text;

                if (CheckDues > 0)
                {
                    command.CommandText = @"UPDATE [dbo].[CustomerPaymentReceive]
                       SET [Date] = convert(date,'" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103),[Customer_id] ='" +
                                          aSales.Customer + "',[PayAmt] ='" + (Convert.ToDecimal(aSales.CReceive) - Convert.ToDecimal(aSales.CRefund)) + "' ,[update_by] ='" +
                                          aSales.LoginBy + "' ,[update_date] = GETDATE()   WHERE [Invoice]='" +
                                          aSales.ID + "' and BranchId='" + aSales.BranchId + "' ";
                    command.ExecuteNonQuery();
                }
                else
                {
//                    command.CommandText = @"INSERT INTO [dbo].[CustomerPaymentReceive]
//           ([Date],[Customer_id],[Invoice],[PayAmt],[entry_by],[entry_date],BranchId)
//     VALUES
//           ( convert(date,'" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103) ,'" + aSales.Customer + "','" + aSales.ID +
//                                          "','" + (Convert.ToDecimal(aSales.CReceive) - Convert.ToDecimal(aSales.CRefund)) + "','" + aSales.LoginBy + "',GETDATE(),'" + aSales.BranchId + "')";
//                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO [dbo].[CustomerPaymentReceive]
           ([Date],BranchId,[Customer_id],[Invoice],[PayAmt],[entry_by],[entry_date])
     VALUES
           (convert(datetime, nullif( '" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),''), 103),'" + aSales.BranchId + "','" + aSales.Customer + "','" + aSales.ID +
                             "','" + (Convert.ToDecimal(aSales.CReceive) - Convert.ToDecimal(aSales.CRefund)) + "','" + aSales.LoginBy + "',GETDATE())";
                    command.ExecuteNonQuery();
                }
            }

            //********************* Sales Total (Show Purchase Price) *********//

            //command.CommandText = "SP_PV_UnitPrice_All";
            //command.CommandType = CommandType.StoredProcedure;
            //command.Parameters.AddWithValue("@MstID", Convert.ToInt32(aSales.ID));
            //command.Parameters.AddWithValue("@Rate", Convert.ToDecimal(CurrencyRate));
            //PurchasePrice = Convert.ToDecimal(command.ExecuteScalar());

            //************************ Account Code ************************//








            double totVat = 0, Discount = 0;
            totVat = ((Convert.ToDouble(aSales.Total) - Convert.ToDouble(aSales.Disount)) * Convert.ToDouble(aSales.Tax)) / 100;
            Discount = Convert.ToDouble(aSales.Disount);
            string tot = (Convert.ToDouble(aSales.Total) - Discount).ToString();
            _aVouchMst.ControlAmt = aSales.GTotal;

            command.CommandType = CommandType.Text;
            command.CommandText = VouchManager.SaveVoucherMst(_aVouchMst, 2);
            command.ExecuteNonQuery();

            command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" + _aVouchMst.VchSysNo +
                                  "')";
            command.ExecuteNonQuery();

            VouchDtl vdtl;
            for (int j = 0; j < 4; j++)
            {
                if (j == 0)
                {
                    //DataRow 
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = aSales.Date;
                    vdtl.LineNo = "1";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["PHSalesInvoice"].ToString(); //**** Sales Code *******//
                    vdtl.AccType =
                    VouchManager.getAccType(dtFixCode.Rows[0]["PHSalesInvoice"].ToString()); //**** Sales Code *******//
                    vdtl.Particulars = dtFixCode.Rows[0]["PHSalesInvoiceDesc"].ToString();
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = aSales.GTotal;
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    vdtl.AUTHO_USER = _aVouchMst.EntryUser;
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 1)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = aSales.Date;
                    vdtl.LineNo = "2";
                    vdtl.GlCoaCode = "1-" + aSales.CustomerCoa;
                    vdtl.Particulars = aSales.CustomerName;
                    vdtl.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
                    vdtl.AmountDr = _aVouchMst.ControlAmt.Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    vdtl.AUTHO_USER = _aVouchMst.EntryUser;
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 2)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = aSales.Date;
                    vdtl.LineNo = "3";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
                    vdtl.Particulars = "Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = PurchasePrice.ToString().Replace(",", "");
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);

                }

                else if (j == 3)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = aSales.Date;
                    vdtl.LineNo = "4";
                    vdtl.GlCoaCode = "1-" + dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
                    vdtl.AccType =
                            VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
                                .ToString()); //**** Purchase Code *******//
                    vdtl.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = totVat.ToString().Replace("'", "").Replace(",", ""); ;
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    vdtl.AUTHO_USER = _aVouchMst.EntryUser;
                    //*********** Convert Rate ********//
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
            }

            //***************************  Credit Voucher ********************************// 
            if (Convert.ToDecimal(aSales.CReceive) > 0)
            {
                _aVouchMstCR.ControlAmt = aSales.CReceive.Replace("'", "").Replace(",", "");
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
                        vdtlCR.ValueDate = aSales.Date;
                        vdtlCR.LineNo = "1";
                        vdtlCR.GlCoaCode = "1-" + aSales.CustomerCoa;
                        vdtlCR.Particulars = aSales.CustomerName;
                        vdtlCR.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
                        vdtlCR.AmountDr = "0";
                        vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
                        vdtlCR.Status = _aVouchMstCR.Status;
                        vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

                        vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                    }
                    else if (j == 1)
                    {
                        int LineNumber = 2;
                        foreach (DataRow dr1 in dtPayment.Rows)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                            vdtlCR.ValueDate = aSales.Date;
                            vdtlCR.LineNo = LineNumber.ToString();
                            if (string.IsNullOrEmpty(dr1["AccountId"].ToString()) || dr1["AccountId"].ToString() == "0")
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                var GlCoaCode = IdManager.GetShowSingleValueString("Gl_Code", "Id", "bank_branch",
                                    dr1["AccountId"].ToString());


                                vdtlCR.GlCoaCode = "1-" + GlCoaCode; //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + GlCoaCode); //**** SalesCode *******//
                                vdtlCR.Particulars = aSales.BankName;
                            }

                            //vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
                            vdtlCR.AmountDr = dr1["Amount"].ToString();
                            vdtlCR.AmountCr = "0";
                            vdtlCR.Status = _aVouchMstCR.Status;
                            vdtlCR.BookName = _aVouchMstCR.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                            LineNumber++;
                        }
                    }
                }
            }



            //double totVat = 0, Discount = 0;
            //totVat = (Convert.ToDouble(aSales.Total) * Convert.ToDouble(aSales.Tax)) / 100;
            //Discount = Convert.ToDouble(aSales.Disount);
            //string tot = (Convert.ToDouble(aSales.Total) - Discount).ToString();
            //_aVouchMst.ControlAmt = (Convert.ToDouble(aSales.Total) - Discount).ToString();

            //command.CommandType = CommandType.Text;
            //command.CommandText = VouchManager.SaveVoucherMst(_aVouchMst, 2);
            //command.ExecuteNonQuery();

            //command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" + _aVouchMst.VchSysNo +
            //                      "')";
            //command.ExecuteNonQuery();

            //VouchDtl vdtl;
            //for (int j = 0; j < 3; j++)
            //{
            //    if (j == 0)
            //    {
            //        //DataRow 
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "1";
            //        vdtl.GlCoaCode = dtFixCode.Rows[0]["PHSalesInvoice"].ToString(); //**** Sales Code *******//
            //        vdtl.AccType =
            //            VouchManager.getAccType(dtFixCode.Rows[0]["PHSalesInvoice"]
            //                .ToString()); //**** Sales Code *******//
            //        vdtl.Particulars = dtFixCode.Rows[0]["PHSalesInvoiceDesc"].ToString();
            //        vdtl.AmountDr = "0";
            //        vdtl.AmountCr = _aVouchMst.ControlAmt.Replace(",", "");
            //        ;
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        vdtl.AUTHO_USER = _aVouchMst.EntryUser;
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
            //    }
            //    else if (j == 1)
            //    {
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "2";
            //        vdtl.GlCoaCode = "1-" + aSales.CustomerCoa;
            //        vdtl.Particulars = aSales.CustomerName;
            //        vdtl.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //        vdtl.AmountDr = _aVouchMst.ControlAmt.Replace(",", "");
            //        vdtl.AmountCr = "0";
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        vdtl.AUTHO_USER = _aVouchMst.EntryUser;
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
            //    }
            //    else if (j == 2)
            //    {
            //        vdtl = new VouchDtl();
            //        vdtl.VchSysNo = _aVouchMst.VchSysNo;
            //        vdtl.ValueDate = aSales.Date;
            //        vdtl.LineNo = "3";
            //        vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
            //        vdtl.Particulars = "Closing Stock";
            //        vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
            //        vdtl.AmountDr = "0";
            //        vdtl.AmountCr = PurchasePrice.ToString().Replace(",", "");
            //        vdtl.AUTHO_USER = "CS";
            //        vdtl.Status = _aVouchMst.Status;
            //        vdtl.BookName = _aVouchMst.BookName;
            //        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
            //    }
            //}

            ////***************************  Credit Voucher ********************************// 
            //if (Convert.ToDecimal(aSales.CReceive) > 0)
            //{
            //    if (_aVouchMstCR.RefFileNo == "New")
            //    {
            //        _aVouchMstCR.ControlAmt = aSales.CReceive.Replace("'", "").Replace(",", "");
            //        command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 1);
            //        command.ExecuteNonQuery();

            //        VouchDtl vdtlCR;
            //        for (int j = 0; j < 3; j++)
            //        {
            //            if (j == 0)
            //            {
            //                //DataRow 
            //                vdtlCR = new VouchDtl();
            //                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
            //                vdtlCR.ValueDate = aSales.Date;
            //                vdtlCR.LineNo = "1";
            //                vdtlCR.GlCoaCode = "1-" + aSales.CustomerCoa;
            //                vdtlCR.Particulars = aSales.CustomerName;
            //                vdtlCR.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //                vdtlCR.AmountDr = "0";
            //                vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
            //                vdtlCR.Status = _aVouchMstCR.Status;
            //                vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

            //                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
            //            }
            //            else if (j == 1)
            //            {
            //                vdtlCR = new VouchDtl();
            //                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
            //                vdtlCR.ValueDate = aSales.Date;
            //                vdtlCR.LineNo = "2";
            //                if (string.IsNullOrEmpty(aSales.BankCoaCode))
            //                {
            //                    vdtlCR.GlCoaCode =
            //                        "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
            //                    vdtlCR.AccType =
            //                        VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
            //                                                    .ToString()); //**** SalesCode *******//
            //                    vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
            //                }
            //                else
            //                {
            //                    vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
            //                    vdtlCR.AccType =
            //                        VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
            //                    vdtlCR.Particulars = aSales.BankName;
            //                }

            //                vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
            //                vdtlCR.AmountCr = "0";
            //                vdtlCR.Status = _aVouchMstCR.Status;
            //                vdtlCR.BookName = _aVouchMstCR.BookName;
            //                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
            //                //*********** Convert Rate ********//
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
            //            }
            //        }
            //    }
            //    else
            //    {

            //        _aVouchMstCR.ControlAmt = aSales.CReceive.Replace("'", "").Replace(",", "");
            //        command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 2);
            //        command.ExecuteNonQuery();
            //        command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
            //                              _aVouchMstCR.VchSysNo + "')";
            //        command.ExecuteNonQuery();

            //        VouchDtl vdtlCR;
            //        for (int j = 0; j < 3; j++)
            //        {
            //            if (j == 0)
            //            {
            //                //DataRow 
            //                vdtlCR = new VouchDtl();
            //                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
            //                vdtlCR.ValueDate = aSales.Date;
            //                vdtlCR.LineNo = "1";
            //                vdtlCR.GlCoaCode = "1-" + aSales.CustomerCoa;
            //                vdtlCR.Particulars = aSales.CustomerName;
            //                vdtlCR.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //                vdtlCR.AmountDr = "0";
            //                vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
            //                vdtlCR.Status = _aVouchMstCR.Status;
            //                vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

            //                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
            //            }
            //            else if (j == 1)
            //            {
            //                vdtlCR = new VouchDtl();
            //                vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
            //                vdtlCR.ValueDate = aSales.Date;
            //                vdtlCR.LineNo = "2";
            //                if (string.IsNullOrEmpty(aSales.BankCoaCode))
            //                {
            //                    vdtlCR.GlCoaCode =
            //                        "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
            //                    vdtlCR.AccType =
            //                        VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
            //                                                    .ToString()); //**** SalesCode *******//
            //                    vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
            //                }
            //                else
            //                {
            //                    vdtlCR.GlCoaCode = "1-" + aSales.BankId; //**** SalesCode *******//
            //                    vdtlCR.AccType =
            //                        VouchManager.getAccType("1-" + aSales.BankId); //**** SalesCode *******//
            //                    vdtlCR.Particulars = aSales.BankName;
            //                }

            //                vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
            //                vdtlCR.AmountCr = "0";
            //                vdtlCR.Status = _aVouchMstCR.Status;
            //                vdtlCR.BookName = _aVouchMstCR.BookName;
            //                vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
            //                //*********** Convert Rate ********//
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
            //            }
            //        }
            //    }
            //}

            ////***************************  Tax ********************************// 
            //if (Convert.ToDecimal(aSales.Tax) > 0)
            //{
            //    if (_aVouchMstTax.RefFileNo == "New")
            //    {
            //        _aVouchMstTax.ControlAmt = totVat.ToString().Replace("'", "").Replace(",", "");
            //        command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstTax, 1);
            //        command.ExecuteNonQuery();

            //        VouchDtl vdtlTax;
            //        for (int j = 0; j < 3; j++)
            //        {
            //            if (j == 0)
            //            {
            //                //DataRow 
            //                vdtlTax = new VouchDtl();
            //                vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
            //                vdtlTax.ValueDate = aSales.Date;
            //                vdtlTax.LineNo = "1";
            //                vdtlTax.GlCoaCode = "1-" + aSales.CustomerCoa;
            //                vdtlTax.Particulars = aSales.CustomerName;
            //                vdtlTax.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //                vdtlTax.AmountDr = "0";
            //                vdtlTax.AmountCr = totVat.ToString().Replace("'", "").Replace(",", "");
            //                vdtlTax.Status = _aVouchMstTax.Status;
            //                vdtlTax.BookName = _aVouchMstTax.BookName; //*********** Convert Rate ********//

            //                vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
            //            }
            //            else if (j == 1)
            //            {
            //                vdtlTax = new VouchDtl();
            //                vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
            //                vdtlTax.ValueDate = aSales.Date;
            //                vdtlTax.LineNo = "2";
            //                vdtlTax.GlCoaCode =
            //                    dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
            //                vdtlTax.AccType =
            //                    VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
            //                        .ToString()); //**** Purchase Code *******//
            //                vdtlTax.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
            //                vdtlTax.AmountDr = totVat.ToString().Replace("'", "").Replace(",", "");
            //                vdtlTax.AmountCr = "0";
            //                vdtlTax.Status = _aVouchMstTax.Status;
            //                vdtlTax.BookName = _aVouchMstTax.BookName;
            //                vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
            //                //*********** Convert Rate ********//
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        _aVouchMstTax.ControlAmt = totVat.ToString().Replace("'", "").Replace(",", "");
            //        command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstTax, 2);
            //        command.ExecuteNonQuery();
            //        command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
            //                              _aVouchMstTax.VchSysNo + "')";
            //        command.ExecuteNonQuery();
            //        VouchDtl vdtlTax;
            //        for (int j = 0; j < 3; j++)
            //        {
            //            if (j == 0)
            //            {
            //                //DataRow 
            //                vdtlTax = new VouchDtl();
            //                vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
            //                vdtlTax.ValueDate = aSales.Date;
            //                vdtlTax.LineNo = "1";
            //                vdtlTax.GlCoaCode = "1-" + aSales.CustomerCoa;
            //                vdtlTax.Particulars = aSales.CustomerName;
            //                vdtlTax.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
            //                vdtlTax.AmountDr = "0";
            //                vdtlTax.AmountCr = totVat.ToString().Replace("'", "").Replace(",", "");
            //                vdtlTax.Status = _aVouchMstTax.Status;
            //                vdtlTax.BookName = _aVouchMstTax.BookName; //*********** Convert Rate ********//

            //                vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
            //            }
            //            else if (j == 1)
            //            {
            //                vdtlTax = new VouchDtl();
            //                vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
            //                vdtlTax.ValueDate = aSales.Date;
            //                vdtlTax.LineNo = "2";
            //                vdtlTax.GlCoaCode =
            //                    dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
            //                vdtlTax.AccType =
            //                    VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
            //                        .ToString()); //**** Purchase Code *******//
            //                vdtlTax.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
            //                vdtlTax.AmountDr = totVat.ToString().Replace("'", "").Replace(",", "");
            //                vdtlTax.AmountCr = "0";
            //                vdtlTax.Status = _aVouchMstTax.Status;
            //                vdtlTax.BookName = _aVouchMstTax.BookName;
            //                vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
            //                //*********** Convert Rate ********//
            //                VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
            //            }
            //        }
            //    }
            //}

            command.CommandText = "update FixGlCoaCode set PrintOrderID='" + aSales.ID + "' ";
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


    //Branch Sales Update End////

    public static void DeleteSalesVoucher(Sales aSales, DataTable dtItemsDetails)
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



            string Query = @"select t1.VCH_SYS_NO  from [GL_TRANS_MST] t1 where (t1.PAYEE='SVSLJV' or t1.PAYEE='SVSLCV') and SERIAL_NO='" + aSales.Invoice + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemPurchaseMst");

            command.CommandText = @"DELETE FROM [GL_TRANS_MST]  WHERE (PAYEE='SVSLJV' or PAYEE='SVSLCV') and SERIAL_NO='" + aSales.Invoice + "'";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                command.CommandText = @"DELETE FROM [GL_TRANS_DTL]  WHERE VCH_SYS_NO='" + dr["VCH_SYS_NO"].ToString() + "'";
                command.ExecuteNonQuery();
            }

            foreach (DataRow drDetails in dtItemsDetails.Rows)
            {
                command.CommandText = @"DELETE FROM [OrderDetail] WHERE OrderID='" + aSales.ID + "' and ID='" + drDetails["Dtl_ID"].ToString() + "' and BranchId='" + aSales.BranchId + "' ";
                command.ExecuteNonQuery();
            }

            command.CommandText = @"DELETE FROM [Order] WHERE  ID='" + aSales.ID + "' and BranchId='" + aSales.BranchId + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"DELETE FROM [Order_Payment] WHERE  MstId='" + aSales.ID + "'and BranchId='"+aSales.BranchId+"'";
            command.ExecuteNonQuery();

            command.CommandText = @"DELETE FROM [CustomerPaymentReceive] WHERE  Invoice='" + aSales.ID + "' and BranchId='" + aSales.BranchId + "'";
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
   

   

    public static DataTable GetShowSalesDetails()
    {
        String connectionString = DataManager.OraConnString();
        string query =
            @"select t1.ID,t1.InvoiceNo,t2.ContactName as [CustomerName],CONVERT(nvarchar,t1.OrderDate,103)OrderDate,DeliveryStatus AS [Status],convert(decimal(18,2),t1.CashReceived) AS [CashReceived],t1.GrandTotal,t3.BranchName as BranchName,t1.BranchId   from [Order] t1 left join Customer t2 on t2.ID=t1.CustomerID inner join [dbo].[BranchInfo] as t3 on t1.BranchId=t3.ID where t1.OrderType='C' or t1.OrderType is null order by t1.ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
        return dt;
    }

   // public static DataTable GetShowSalesDetails(string BranchId)
    //{
    //    String connectionString = DataManager.OraConnString();
    //    string query =
    //        @"select t1.ID,t1.InvoiceNo,t2.ContactName as [CustomerName],CONVERT(nvarchar,t1.OrderDate,103)OrderDate,DeliveryStatus AS [Status],convert(decimal(18,2),t1.CashReceived) AS [CashReceived],t1.GrandTotal  from [Order] t1 left join Customer t2 on t2.ID=t1.CustomerID and t1.BranchId=t2.BranchId where t1.Id not in (select NewInvoiceId from OrderExchange  where  BranchId='"+BranchId+"') and  t1.BranchId='" + BranchId + "' and t1.OrderType='C' or t1.OrderType is null   order by t1.ID desc ";
    //    DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
    //    return dt;
    //}
 

    public static DataTable GetSalesDetails(string OrderMstId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"select t1.ID AS[Dtl_ID],t3.ItemCode,t3.ID,t1.ItemID,t1.Barcode,t2.Code,t2.Name as ItemsName, 0 as ChangeQty,isnull(t3.[ClosingStock],0) as TotalClosingStock
, CASE WHEN t3.[ExpireDate] IS NULL THEN t3.ItemCode+' - '+t2.Name + ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) ELSE t2.Name + ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) + ' - ' + CONVERT(nvarchar, t3.[ExpireDate], 103) END AS txtItems
						 ,convert(decimal(18,2),t1.TaxRate) AS Tax,convert(decimal(18,2),t1.DiscountAmount) AS DiscountAmount,convert(decimal(18,2),t1.UnitPrice) AS SPrice,convert(decimal(18,2),t1.Quantity)  AS Qty,t2.ClosingStock as ClosingStock,convert(decimal(18,2),TotalPrice) AS Total,t4.BrandName,t2.UOMID as msr_unit_code,t2.StyleNo AS item_Serial,t2.StyleNo AS  ModelNo,isnull(t1.Remarks,'') AS Remarks,convert(decimal(18,2),t1.Quantity)  AS SaleQty,t2.WarrantyMonth,t2.WarrantyYear,t1.CostPrice
, CASE WHEN t3.[ExpireDate] IS NULL THEN CONVERT(nvarchar, t3.[ID]) + ' - ' + t2.Name+' - '+isnull(t5.SizeName,'None')+ ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) 
                         ELSE CONVERT(nvarchar, t3.[ID]) + ' - ' + t2.Name+' - '+isnull(t5.SizeName,'None')+ ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) + ' - ' + CONVERT(nvarchar, t3.[ExpireDate], 103) END AS SearchItems, CASE WHEN t3.[ExpireDate] IS NULL 
                         THEN t3.ItemCode + ' - ' + t2.Name +' - '+isnull(t5.SizeName,'None')+ ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) ELSE CONVERT(nvarchar, t3.[ID]) + ' - ' + t2.Name +' - '+isnull(t5.SizeName,'None')+ ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) + ' - ' + CONVERT(nvarchar, t3.[ExpireDate], 103) 
                         END AS CodeWiseSearchItems						 
 from OrderDetail t1 
left join dbo.ItemStock t3 on t3.ID=t1.ItemID
left join Item t2 on t2.ID=t3.ItemID
left join Brand t4 on t4.ID=t2.Brand
left join SizeInfo t5 on t5.ID=t2.SizeID where t1.OrderID='" + OrderMstId + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OrderDetail");
        return dt;
    }



    public static DataTable GetMainBranchSalesDetails(string OrderMstId, string BranchId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"select t1.ID AS[Dtl_ID],t3.ItemCode,t3.ID,t2.ID as ItemID,t1.Barcode,t2.Code,t2.Name as ItemsName, 0 as ChangeQty,isnull(t3.[ClosingStock],0) as TotalClosingStock
, CASE WHEN t3.[ExpireDate] IS NULL THEN t3.ItemCode+' - '+t2.Name + ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) ELSE t2.Name + ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) + ' - ' + CONVERT(nvarchar, t3.[ExpireDate], 103) END AS txtItems
						 ,convert(decimal(18,2),t1.TaxRate) AS Tax,convert(decimal(18,2),t1.DiscountAmount) AS DiscountAmount,convert(decimal(18,2),t1.UnitPrice) AS SPrice,convert(decimal(18,2),t1.Quantity)  AS Qty,t2.ClosingStock as ClosingStock,convert(decimal(18,2),TotalPrice) AS Total,t4.BrandName,t2.UOMID as msr_unit_code,t2.StyleNo AS item_Serial,t2.StyleNo AS  ModelNo,isnull(t1.Remarks,'') AS Remarks,convert(decimal(18,2),t1.Quantity)  AS SaleQty,t2.WarrantyMonth,t2.WarrantyYear,t1.CostPrice
, CASE WHEN t3.[ExpireDate] IS NULL THEN CONVERT(nvarchar, t3.[ID]) + ' - ' + t2.Name+' - '+isnull(t5.SizeName,'None')+ ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) 
                         ELSE CONVERT(nvarchar, t3.[ID]) + ' - ' + t2.Name+' - '+isnull(t5.SizeName,'None')+ ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) + ' - ' + CONVERT(nvarchar, t3.[ExpireDate], 103) END AS SearchItems, CASE WHEN t3.[ExpireDate] IS NULL 
                         THEN t3.ItemCode + ' - ' + t2.Name +' - '+isnull(t5.SizeName,'None')+ ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) ELSE CONVERT(nvarchar, t3.[ID]) + ' - ' + t2.Name +' - '+isnull(t5.SizeName,'None')+ ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) + ' - ' + CONVERT(nvarchar, t3.[ExpireDate], 103) 
                         END AS CodeWiseSearchItems						 
 from OrderDetail t1 
left join dbo.ItemStock t3 on t3.ID=t1.ItemID  
left join Item t2 on t2.ID=t3.ItemID
left join Brand t4 on t4.ID=t2.Brand
left join SizeInfo t5 on t5.ID=t2.SizeID where t1.OrderID='" + OrderMstId + "'and t1.BranchId='" + BranchId + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OrderDetail");
        return dt;
    }
    

    public static DataTable GetBranchSalesDetails(string OrderMstId,string BranchId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"select t1.ID AS[Dtl_ID],t3.ItemCode,t3.ID,t2.ID as ItemID,t1.Barcode,t2.Code,t2.Name as ItemsName, 0 as ChangeQty,isnull(t3.[ClosingStock],0) as TotalClosingStock
, CASE WHEN t3.[ExpireDate] IS NULL THEN t3.ItemCode+' - '+t2.Name + ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) ELSE t2.Name + ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) + ' - ' + CONVERT(nvarchar, t3.[ExpireDate], 103) END AS txtItems
						 ,convert(decimal(18,2),t1.TaxRate) AS Tax,convert(decimal(18,2),t1.DiscountAmount) AS DiscountAmount,convert(decimal(18,2),t1.UnitPrice) AS SPrice,convert(decimal(18,2),t1.Quantity)  AS Qty,t2.ClosingStock as ClosingStock,convert(decimal(18,2),TotalPrice) AS Total,t4.BrandName,t2.UOMID as msr_unit_code,t2.StyleNo AS item_Serial,t2.StyleNo AS  ModelNo,isnull(t1.Remarks,'') AS Remarks,convert(decimal(18,2),t1.Quantity)  AS SaleQty,t2.WarrantyMonth,t2.WarrantyYear,t1.CostPrice
, CASE WHEN t3.[ExpireDate] IS NULL THEN CONVERT(nvarchar, t3.[ID]) + ' - ' + t2.Name+' - '+isnull(t5.SizeName,'None')+ ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) 
                         ELSE CONVERT(nvarchar, t3.[ID]) + ' - ' + t2.Name+' - '+isnull(t5.SizeName,'None')+ ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) + ' - ' + CONVERT(nvarchar, t3.[ExpireDate], 103) END AS SearchItems, CASE WHEN t3.[ExpireDate] IS NULL 
                         THEN t3.ItemCode + ' - ' + t2.Name +' - '+isnull(t5.SizeName,'None')+ ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) ELSE CONVERT(nvarchar, t3.[ID]) + ' - ' + t2.Name +' - '+isnull(t5.SizeName,'None')+ ' - ' + CONVERT(nvarchar, t3.[ItemsPrice]) + ' - ' + CONVERT(nvarchar, t3.[ExpireDate], 103) 
                         END AS CodeWiseSearchItems						 
 from OrderDetail t1 
left join dbo.OutLetItemStock t3 on t3.ID=t1.ItemID  and t1.BranchId=t3.BranchID
left join Item t2 on t2.ID=t3.ItemID
left join Brand t4 on t4.ID=t2.Brand
left join SizeInfo t5 on t5.ID=t2.SizeID where t1.OrderID='" + OrderMstId + "'and t1.BranchId='"+BranchId+"'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OrderDetail");
        return dt;
    }
    

    public DataTable GetOrderPayment(string OrderID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT [Id] AS DtlID,[MstId] ,[PaymentypeId],[PaymentypeIdFrom],[Paymentype],[PaymentypeFrom],[Amount],[BankID],[BankName],[AccountNo],[CardOrCheeckNo],[MstID1],[AccountID],[BankIDFrom],[BankNameFrom],[AccountNoFrom],[Status],convert(nvarchar,[ApprovedDate],103) AS [ApprovedDate]
  FROM [dbo].[Order_Payment] where [DeleteBy] IS NULL and [MstId]='" + OrderID + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;
    }

    public static void getSavePrintID(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = "update FixGlCoaCode set PrintOrderID='" + ID + "',PrintType=1 ";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public DataTable GetSalesPaymentDetails(string StartDate, string EndDate,string CustomerId)
    {
        String connectionString = DataManager.OraConnString();
        string query =
            @"SELECT sum(isnull([SubTotal],0)) AS [SubTotal],sum(isnull([TaxAmount],0)) AS [TaxAmount],sum(isnull([DiscountAmount],0)) AS [DiscountAmount] ,sum(isnull([GrandTotal],0)) AS [GrandTotal] ,sum(isnull([CashReceived],0)) AS [CashReceived]      
       ,(select  isnull( SUM(PayAmt),0) as PayAmt from CustomerPaymentReceive where Customer_id in(" + CustomerId + ") and Invoice IS NULL  and DeleteBy IS NULL and convert(date,Date,103) between convert(date,'" + StartDate + "',103) and convert(date,'" + EndDate + "',103))  as PayAmt   FROM [dbo].[Order] where [DeleteBy] IS NULL and convert(date,[OrderDate],103) between convert(date,'" + StartDate +
            "',103) and convert(date,'" + EndDate + "',103)";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;
    }
    //Branch******
    public DataTable GetBranchSalesPaymentDetails(string StartDate, string EndDate, string CustomerId,string BranchId)
    {
        String connectionString = DataManager.OraConnString();
        string query =
            @"SELECT sum(isnull([SubTotal],0)) AS [SubTotal],sum(isnull([TaxAmount],0)) AS [TaxAmount],sum(isnull([DiscountAmount],0)) AS [DiscountAmount] ,sum(isnull([GrandTotal],0)) AS [GrandTotal] ,sum(isnull([CashReceived],0)) AS [CashReceived]      
       ,(select  isnull( SUM(PayAmt),0) as PayAmt from CustomerPaymentReceive where BranchId='"+BranchId+"' and  Customer_id in(" + CustomerId + ") and Invoice IS NULL  and DeleteBy IS NULL and convert(date,Date,103) between convert(date,'" + StartDate + "',103) and convert(date,'" + EndDate + "',103))  as PayAmt   FROM [dbo].[Order] where BranchId='"+BranchId+"' and  [DeleteBy] IS NULL and convert(date,[OrderDate],103) between convert(date,'" + StartDate +
            "',103) and convert(date,'" + EndDate + "',103)";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;
    }
    //****Branch
    public DataTable GetPaymentSummery(string Date,string TypeID)
    {
        using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
        {
            SqlCommand sqlComm = new SqlCommand("SP_DailyPaymentStatus", conn);
            if (!string.IsNullOrEmpty(Date))
            {
                sqlComm.Parameters.AddWithValue("@StartDate", Date);
                sqlComm.Parameters.AddWithValue("@EndDate", Date);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@StartDate", null);
                sqlComm.Parameters.AddWithValue("@EndDate", null);
            }
            sqlComm.Parameters.AddWithValue("@TypeID", TypeID);
            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "SP_GetTotalItemStock");
            DataTable dtCartonReceived = ds.Tables["SP_GetTotalItemStock"];
            return dtCartonReceived;
        }
    }
    public DataTable GetPaymentSummery(string StartDate,string EndDate, string TypeID)
    {
        using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
        {
            SqlCommand sqlComm = new SqlCommand("SP_DailyPaymentStatus", conn);
            if (!string.IsNullOrEmpty(StartDate))
            {
                sqlComm.Parameters.AddWithValue("@StartDate", StartDate);
                sqlComm.Parameters.AddWithValue("@EndDate", EndDate);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@StartDate", null);
                sqlComm.Parameters.AddWithValue("@EndDate", null);
            }
            sqlComm.Parameters.AddWithValue("@TypeID", TypeID);
            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "SP_GetTotalItemStock");
            DataTable dtCartonReceived = ds.Tables["SP_GetTotalItemStock"];
            return dtCartonReceived;
        }
    }
    public DataTable GetCutomerPaymet(string DateFrom, string toDate, string CustomerID)
    {

        string parameter = "";
        if (!string.IsNullOrEmpty(CustomerID))
        {
            parameter = " and t2.ID='" + CustomerID + "'";
        }
        String connectionString = DataManager.OraConnString();
        string query =
            @"Select Convert(nvarchar,t1.Date,103) as PayDate,case when t3.InvoiceNo is null  then 'Payment' else t3.InvoiceNo end  as Ref ,t1.PayAmt as PayAmount,t2.ContactName as CustomerName from CustomerPaymentReceive as T1 
inner join Customer t2 on t2.ID=t1.Customer_ID and t2.BranchId=t1.BranchId  left join [order] t3 on t1.Invoice=t3.ID where  t1.[DeleteBy] IS NULL " + parameter + " and convert(date,[Date],103) between convert(date,'" + DateFrom +
            "',103) and convert(date,'" + toDate + "',103)";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;
    }

   
    public DataTable GetBranchCutomerPaymet(string DateFrom, string toDate, string CustomerID,string BranchId)
    {

        string parameter = "";
        if (!string.IsNullOrEmpty(CustomerID))
        {
            parameter = " and t2.ID='" + CustomerID + "'";
        }
        String connectionString = DataManager.OraConnString();
        string query =
            @"Select Convert(nvarchar,t1.Date,103) as PayDate,case when t3.InvoiceNo is null  then 'Payment' else t3.InvoiceNo end  as Ref ,t1.PayAmt as PayAmount,t2.ContactName as CustomerName from CustomerPaymentReceive as T1 
inner join Customer t2 on t2.ID=t1.Customer_ID and t1.BranchId=t2.BranchId left join [order] t3 on t1.Invoice=t3.ID and t1.BranchId=t3.BranchId where  t1.[DeleteBy] IS NULL and t1.BranchId='"+BranchId+"' " + parameter + " and convert(date,[Date],103) between convert(date,'" + DateFrom +
            "',103) and convert(date,'" + toDate + "',103)";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;
    }

    public DataTable GetSupplierPaymet(string DateFrom, string toDate, string CustomerID)
    {

        string parameter = "";
        if (!string.IsNullOrEmpty(CustomerID))
        {
            parameter = " and t2.ID='" + CustomerID + "'";
        }
        String connectionString = DataManager.OraConnString();
        string query =
            @"Select Convert(nvarchar,t1.Date,103) as PayDate,case when t3.GRN is null  then 'Payment' else t3.GRN end  as Ref ,t1.PayAmt as PayAmount,t2.ContactName as CustomerName from SupplierPaymentReceive as T1 
inner join Supplier t2 on t2.ID=t1.Supplier_id left join ItemPurchaseMst t3 on t1.PurchaseVoucherID=t3.ID where  t1.[DeleteBy] IS NULL  " + parameter + " and convert(date,[Date],103) between convert(date,'" + DateFrom +
            "',103) and convert(date,'" + toDate + "',103)";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;
    }


    public DataTable GetSaleInfo(string DateFrom, string toDate, string CustomerID)
    {
        string parameter = "";
        if (!string.IsNullOrEmpty(CustomerID))
        {
            parameter = " and t2.ID='" + CustomerID + "'";
        }
        String connectionString = DataManager.OraConnString();
        string query =
            @"Select t1.InvoiceNo,Convert(Nvarchar,t1.OrderDate,103) as OrderDate,t1.GrandTotal,t2.ContactName as CustomerName from [Order] t1
inner join Customer t2 on t2.ID=t1.CustomerID  where  t1.[DeleteBy] IS NULL "+parameter+" and convert(date,[OrderDate],103) between convert(date,'" + DateFrom +
            "',103) and convert(date,'" + toDate + "',103)";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;
    }


    public DataTable GetBranchSaleInfo(string DateFrom, string toDate, string CustomerID,string BranchId)
    {
        string parameter = "";
        if (!string.IsNullOrEmpty(CustomerID))
        {
            parameter = " and t2.ID='" + CustomerID + "'";
        }
        String connectionString = DataManager.OraConnString();
        string query =
            @"Select t1.InvoiceNo,Convert(Nvarchar,t1.OrderDate,103) as OrderDate,t1.GrandTotal,t2.ContactName as CustomerName from [Order] t1
inner join Customer t2 on t2.ID=t1.CustomerID and t1.BranchId=t2.BranchId  where t1.BranchId='"+BranchId+"' and  t1.[DeleteBy] IS NULL " + parameter + " and convert(date,[OrderDate],103) between convert(date,'" + DateFrom +
            "',103) and convert(date,'" + toDate + "',103)";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;
    }
    public DataTable GetPurchaseInfo(string DateFrom, string toDate, string CustomerID)
    {
        string parameter = "";
        if (!string.IsNullOrEmpty(CustomerID))
        {
            parameter = " where t2.ID='" + CustomerID + "'";
        }
        String connectionString = DataManager.OraConnString();
        string query =
            @"Select t1.GRN,Convert(Nvarchar,t1.ChallanDate,103) as ChallanDate,t1.Total,t2.ContactName as SupplierName from ItemPurchaseMst t1
inner join Supplier t2 on t2.ID=t1.SupplierID  " + parameter + " and convert(date,[ChallanDate],103) between convert(date,'" + DateFrom +
            "',103) and convert(date,'" + toDate + "',103)";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;
    }

   

    public DataTable GetCutomerPaymentSummery(string DateFrom, string toDate, string CustomerID)
    {
        string parameter="";
        if(!string.IsNullOrEmpty(CustomerID))
        {
            parameter=" and t2.ID='"+CustomerID+"'";
        }
        String connectionString = DataManager.OraConnString();
        string query =
            @"Select Sum(t1.PayAmt ) as Payment,t2.ContactName,t2.Email,t2.Mobile from CustomerPaymentReceive t1 inner join Customer t2 on t2.ID=t1.Customer_ID where Convert(date,date,13) between Convert(Date,'" + DateFrom + "',103) and Convert(Date,'" + toDate + "',103) "+parameter+" group by t2.ContactName,t2.Email,t2.Mobile ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;

    }

    public DataTable GetBranchCutomerPaymentSummery(string DateFrom, string toDate, string CustomerID,string BranchId)
    {
        string parameter = "";
        if (!string.IsNullOrEmpty(CustomerID))
        {
            parameter = " and t2.ID='" + CustomerID + "'";
        }
        String connectionString = DataManager.OraConnString();
        string query =
            @"Select Sum(t1.PayAmt ) as Payment,t2.ContactName,t2.Email,t2.Mobile from CustomerPaymentReceive t1 inner join Customer t2 on t2.ID=t1.Customer_ID  and t1.BranchId=t2.BranchId where t1.BranchId='" + BranchId + "' and Convert(date,date,13) between Convert(Date,'" + DateFrom + "',103) and Convert(Date,'" + toDate + "',103) " + parameter + " group by t2.ContactName,t2.Email,t2.Mobile ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;

    }


    public DataTable GetCutomerPaymetSummery(string DateFrom, string toDate, string CustomerID)
    {
        string parameter = "";
        if (!string.IsNullOrEmpty(CustomerID))
        {
            parameter = " and t1.ID='" + CustomerID + "'";
        }
        String connectionString = DataManager.OraConnString();
        string query =
            @"Select Sum(isnull(Payment,0)) as Payment,Sum(isnull(Sales,0)) as Sales,sum(isnull(Sales,0)-isnull(Payment,0)) as Due ,t1.ContactName,t1.Email,t1.Mobile from (Select t1.PayAmt as Payment,0 as Sales,[date],t2.ContactName,t2.Email,t2.Mobile,t2.ID from CustomerPaymentReceive t1 
inner join Customer t2 on t2.ID=t1.Customer_ID 

  union all
 Select 0,t1.GrandTotal,OrderDate ,t2.ContactName,t2.Email,t2.Mobile,t2.ID from [order] t1 
inner join Customer t2 on t2.ID=t1.CustomerID )  t1 
where Convert(date,t1.[Date],13) between Convert(Date,'"+DateFrom+"',103) and Convert(Date,'"+toDate+"',103) "+parameter+" group by t1.ContactName,t1.Email,t1.Mobile ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;
    }


    public DataTable GetBranchCutomerPaymetSummery(string DateFrom, string toDate, string CustomerID, string BranchId)
    {
        string parameter = "";
        if (!string.IsNullOrEmpty(CustomerID))
        {
            parameter = " and t1.ID='" + CustomerID + "'";
        }
        String connectionString = DataManager.OraConnString();
        string query =
            @"select * from (Select Sum(isnull(Payment,0)) as Payment,Sum(isnull(Sales,0)) as Sales,sum(isnull(Sales,0)-isnull(Payment,0)) as Due ,t1.ContactName,t1.Email,t1.Mobile from (Select t1.PayAmt as Payment,0 as Sales,[date],t2.ContactName,t2.Email,t2.Mobile,t2.ID from CustomerPaymentReceive t1 inner join Customer t2 on t2.ID=t1.Customer_ID  and t1.BranchId=t2.BranchId where t1.BranchId='" + BranchId + "' union all Select 0,t1.GrandTotal,OrderDate ,t2.ContactName,t2.Email,t2.Mobile,t2.ID from [order] t1 inner join Customer t2 on t2.ID=t1.CustomerID and t1.BranchId=t2.BranchId where t1.BranchId='" + BranchId + "')  t1 where Convert(date,t1.[Date],13) between Convert(Date,'" + DateFrom + "',103) and Convert(Date,'" + toDate + "',103) " + parameter + " group by t1.ContactName,t1.Email,t1.Mobile ) t1 where t1.Due>0";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;
    }

    public DataTable GetSupplierPaymetSummery(string DateFrom, string toDate, string SupplierID)
    {
        string parameter = "";
        if (!string.IsNullOrEmpty(SupplierID))
        {
            parameter = " and t1.Supplier_id='" + SupplierID + "'";
        }
        String connectionString = DataManager.OraConnString();
        string query =
            @"Select Sum(isnull(Payment,0)) as Payment,t1.Supplier_id,Sum(isnull(Sales,0)) as Purchase,sum(isnull(Sales,0)-isnull(Payment,0)) as Due ,t1.ContactName,t1.Email,t1.Mobile from (Select t1.PayAmt as Payment,0 as Sales,[date],t1.Supplier_id,t2.ContactName,t2.Email,t2.Mobile from SupplierPaymentReceive t1 
inner join Supplier t2 on t2.ID=t1.Supplier_id 

  union all
 Select 0,t1.Total,ChallanDate ,t1.SupplierID,t2.ContactName,t2.Email,t2.Mobile from ItemPurchaseMst t1 
inner join Supplier t2 on t2.ID=t1.SupplierID )  t1 
where Convert(date,t1.[Date],13) between Convert(Date,'" + DateFrom + "',103) and Convert(Date,'" + toDate + "',103) " + parameter + " group by t1.ContactName,t1.Email,t1.Mobile,t1.Supplier_id";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Order_Payment");
        return dt;
    }

    public static void UpdatePrintStatus(string ID, int Type)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"update [FixGlCoaCode] set [PrintOrderID]="+ID+",PrintType="+Type+"";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static int IsExistExchange(string OrderId)
    {
        var connectionsting = DataManager.OraConnString();
        var connection=new SqlConnection(connectionsting);
        connection.Open();
        string Query = "select count(*) as IdCount from dbo.OrderDetail where OrderID='"+OrderId+"' and  ChangeQty IS NOT NULL";
        SqlCommand command = new SqlCommand(Query, connection);
        return Convert.ToInt32(command.ExecuteScalar());
    }



    public static int IsBranchExistExchange(string OrderId,string BranchId)
    {
        var connectionsting = DataManager.OraConnString();
        var connection = new SqlConnection(connectionsting);
        connection.Open();
        string Query = "select count(*) as IdCount from dbo.OrderDetail where BranchId='"+BranchId+"' and OrderID='" + OrderId + "' and  ChangeQty IS NOT NULL";
        SqlCommand command = new SqlCommand(Query, connection);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public static int IsExistExchangeInvoice(string InvoiceNo, string BranchId)
    {
        string query = "";
        var connectionstring = DataManager.OraConnString();
        SqlConnection connection=new SqlConnection(connectionstring);
        connection.Open();
        //string query = "select count(t1.Id) from [Order] t1   Inner join  OrderExchange t2 on t1.InvoiceNo=t2.InvoiceNo and t2.NewInvoiceID=t1.Id where t1.DeleteBy is null and t1.BranchId='" + BranchId + "' and t1.InvoiceNo='" + InvoiceNo + "' ";
         query = "select count(t1.Id) from [Order] t1   Inner join  OrderExchange t2 on t1.InvoiceNo=t2.InvoiceNo   where t1.DeleteBy is null and t1.BranchId='" + BranchId + "' and t1.InvoiceNo='" + InvoiceNo + "' ";

        var command=new SqlCommand(query,connection);
        int Count = Convert.ToInt32(command.ExecuteScalar());
        connection.Close();
        if (Count<=0)
        {
            connection.Open();
            query = "select COUNT(*) from (select * from [Order] where InvoiceNo='"+InvoiceNo+"' and BranchId='"+BranchId+"' and DeleteDate is null) t1 inner join OrderExchange t2 on t1.ID=t2.NewInvoiceID where t2.DeleteDate is null and t2.BranchId='"+BranchId+"'";
            var command1=new SqlCommand(query,connection);
            Count = Convert.ToInt32(command1.ExecuteScalar());
        }
        return Count;
    }

    public static int IsExistExchangeReturn(string OrderId, string BranchId)
    {
        var connectionstring = DataManager.OraConnString();
        SqlConnection connection = new SqlConnection(connectionstring);
        connection.Open();
        string query = "select count(t1.Id) from [Order] t1   Inner join  OrderReturn t2 on t1.Id=t2.InvoiceNo  where t1.DeleteBy is null and t1.BranchId='"+BranchId+"' and t1.ID='"+OrderId+"'";
        var command = new SqlCommand(query, connection);
        int Count = Convert.ToInt32(command.ExecuteScalar());
        connection.Close();
        return Count;
    }
}