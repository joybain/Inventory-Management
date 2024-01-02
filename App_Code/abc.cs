using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Delve;

/// <summary>
/// Summary description for abc
/// </summary>
public class abc
{
	public abc()
	{
		//
		// TODO: Add constructor logic here
		//
	}

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
            command.CommandText = @"DELETE FROM [Order_Payment] WHERE  MstId='" + aSales.ID + "' and BranchId='"+aSales.BranchId+"'";
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
                    PurchasePrice = +Convert.ToDecimal(dr["CostPrice"].ToString().Replace(",", ""));
                }

            }

            foreach (DataRow drPay in dtPayment.Rows)
            {
                if (!string.IsNullOrEmpty(drPay["PaymentypeID"].ToString()))
                {
                    command.CommandText =
                        "insert into Order_Payment(BranchId,MstId, PaymentypeId, PaymentypeIdFrom, Paymentype, PaymentypeFrom, Amount, AddBy, AddDate,[BankID],[BankName] , [AccountNo],[AccountID],[CardOrCheeckNo],[BankIDFrom],[BankNameFrom],[AccountNoFrom],[Status],[ApprovedDate]) values('"+aSales.BranchId+"','" +
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
                    command.CommandText = @"INSERT INTO [dbo].[CustomerPaymentReceive]
           ([Date],[Customer_id],[Invoice],[PayAmt],[entry_by],[entry_date],BranchId)
     VALUES
           ( convert(date,'" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103) ,'" + aSales.Customer + "','" + aSales.ID +
                                          "','" + (Convert.ToDecimal(aSales.CReceive) - Convert.ToDecimal(aSales.CRefund)) + "','" + aSales.LoginBy + "',GETDATE(),'" + aSales.BranchId + "')";
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
            totVat = (Convert.ToDouble(aSales.Total) * Convert.ToDouble(aSales.Tax)) / 100;
            Discount = Convert.ToDouble(aSales.Disount);
            string tot = (Convert.ToDouble(aSales.Total) - Discount).ToString();
            _aVouchMst.ControlAmt = (Convert.ToDouble(aSales.Total) - Discount).ToString();

            command.CommandType = CommandType.Text;
            command.CommandText = VouchManager.SaveVoucherMst(_aVouchMst, 2);
            command.ExecuteNonQuery();

            command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" + _aVouchMst.VchSysNo +
                                  "')";
            command.ExecuteNonQuery();

            VouchDtl vdtl;
            for (int j = 0; j < 3; j++)
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
                        VouchManager.getAccType(dtFixCode.Rows[0]["PHSalesInvoice"]
                            .ToString()); //**** Sales Code *******//
                    vdtl.Particulars = dtFixCode.Rows[0]["PHSalesInvoiceDesc"].ToString();
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = _aVouchMst.ControlAmt.Replace(",", "");
                    ;
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
            }

            //***************************  Credit Voucher ********************************// 
            if (Convert.ToDecimal(aSales.CReceive) > 0)
            {
                if (_aVouchMstCR.RefFileNo == "New")
                {
                    _aVouchMstCR.ControlAmt = aSales.CReceive.Replace("'", "").Replace(",", "");
                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 1);
                    command.ExecuteNonQuery();

                    VouchDtl vdtlCR;
                    for (int j = 0; j < 3; j++)
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
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                            vdtlCR.ValueDate = aSales.Date;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(aSales.BankCoaCode))
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
                                vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
                                vdtlCR.Particulars = aSales.BankName;
                            }

                            vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
                            vdtlCR.AmountCr = "0";
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

                    _aVouchMstCR.ControlAmt = aSales.CReceive.Replace("'", "").Replace(",", "");
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
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                            vdtlCR.ValueDate = aSales.Date;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(aSales.BankCoaCode))
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
                                vdtlCR.GlCoaCode = "1-" + aSales.BankId; //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + aSales.BankId); //**** SalesCode *******//
                                vdtlCR.Particulars = aSales.BankName;
                            }

                            vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
                            vdtlCR.AmountCr = "0";
                            vdtlCR.Status = _aVouchMstCR.Status;
                            vdtlCR.BookName = _aVouchMstCR.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                        }
                    }
                }
            }

            //***************************  Tax ********************************// 
            if (Convert.ToDecimal(aSales.Tax) > 0)
            {
                if (_aVouchMstTax.RefFileNo == "New")
                {
                    _aVouchMstTax.ControlAmt = totVat.ToString().Replace("'", "").Replace(",", "");
                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstTax, 1);
                    command.ExecuteNonQuery();

                    VouchDtl vdtlTax;
                    for (int j = 0; j < 3; j++)
                    {
                        if (j == 0)
                        {
                            //DataRow 
                            vdtlTax = new VouchDtl();
                            vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
                            vdtlTax.ValueDate = aSales.Date;
                            vdtlTax.LineNo = "1";
                            vdtlTax.GlCoaCode = "1-" + aSales.CustomerCoa;
                            vdtlTax.Particulars = aSales.CustomerName;
                            vdtlTax.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
                            vdtlTax.AmountDr = "0";
                            vdtlTax.AmountCr = totVat.ToString().Replace("'", "").Replace(",", "");
                            vdtlTax.Status = _aVouchMstTax.Status;
                            vdtlTax.BookName = _aVouchMstTax.BookName; //*********** Convert Rate ********//

                            vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
                        }
                        else if (j == 1)
                        {
                            vdtlTax = new VouchDtl();
                            vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
                            vdtlTax.ValueDate = aSales.Date;
                            vdtlTax.LineNo = "2";
                            vdtlTax.GlCoaCode =
                                dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
                            vdtlTax.AccType =
                                VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
                                    .ToString()); //**** Purchase Code *******//
                            vdtlTax.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
                            vdtlTax.AmountDr = totVat.ToString().Replace("'", "").Replace(",", "");
                            vdtlTax.AmountCr = "0";
                            vdtlTax.Status = _aVouchMstTax.Status;
                            vdtlTax.BookName = _aVouchMstTax.BookName;
                            vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
                        }
                    }
                }
                else
                {
                    _aVouchMstTax.ControlAmt = totVat.ToString().Replace("'", "").Replace(",", "");
                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstTax, 2);
                    command.ExecuteNonQuery();
                    command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
                                          _aVouchMstTax.VchSysNo + "')";
                    command.ExecuteNonQuery();
                    VouchDtl vdtlTax;
                    for (int j = 0; j < 3; j++)
                    {
                        if (j == 0)
                        {
                            //DataRow 
                            vdtlTax = new VouchDtl();
                            vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
                            vdtlTax.ValueDate = aSales.Date;
                            vdtlTax.LineNo = "1";
                            vdtlTax.GlCoaCode = "1-" + aSales.CustomerCoa;
                            vdtlTax.Particulars = aSales.CustomerName;
                            vdtlTax.AccType = VouchManager.getAccType("1-" + aSales.CustomerCoa);
                            vdtlTax.AmountDr = "0";
                            vdtlTax.AmountCr = totVat.ToString().Replace("'", "").Replace(",", "");
                            vdtlTax.Status = _aVouchMstTax.Status;
                            vdtlTax.BookName = _aVouchMstTax.BookName; //*********** Convert Rate ********//

                            vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
                        }
                        else if (j == 1)
                        {
                            vdtlTax = new VouchDtl();
                            vdtlTax.VchSysNo = _aVouchMstTax.VchSysNo;
                            vdtlTax.ValueDate = aSales.Date;
                            vdtlTax.LineNo = "2";
                            vdtlTax.GlCoaCode =
                                dtFixCode.Rows[0]["Vat_Sales"].ToString(); //**** Purchase Code *******//
                            vdtlTax.AccType =
                                VouchManager.getAccType(dtFixCode.Rows[0]["Vat_Sales"]
                                    .ToString()); //**** Purchase Code *******//
                            vdtlTax.Particulars = dtFixCode.Rows[0]["Vat_Sales_Description"].ToString();
                            vdtlTax.AmountDr = totVat.ToString().Replace("'", "").Replace(",", "");
                            vdtlTax.AmountCr = "0";
                            vdtlTax.Status = _aVouchMstTax.Status;
                            vdtlTax.BookName = _aVouchMstTax.BookName;
                            vdtlTax.AUTHO_USER = _aVouchMstTax.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstTax, vdtlTax, command);
                        }
                    }
                }
            }

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

}