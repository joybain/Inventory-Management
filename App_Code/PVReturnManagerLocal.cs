using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Delve;
using System.Data.SqlClient;

/// <summary>
/// Summary description for PVReturnManager
/// </summary>
public class PVReturnManagerLocal
{
    public PVReturnManagerLocal()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable GetShowPurchaseItems(string PVMst)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT  t1.[ItemID],t2.Code+' - '+t2.Name+' - '+t3.BrandName AS Items_Name FROM [ItemPurchaseLocalDtl] t1 inner join Item t2 on t2.ID=t1.ItemID inner join dbo.Brand t3 on t3.ID=t2.Brand WHERE [ItemPurchaseMstID]='" + PVMst + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseLocalDtl");
        return dt;
    }

    public static DataTable GetShowPVMasterInfo(string GRN)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID] ,t1.[GRN] ,t1.[SupplierID],t2.ContactName ,t2.Gl_CoaCode
      FROM [ItemPurchaseLocalMst] t1 inner join Supplier t2 on t2.ID=t1.SupplierID WHERE t1.GRN='" + GRN + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseLocalDtl");
        return dt;
    }

    public static DataTable GetPVItems(string ItemsID,string PVID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]    
      ,'' AS item_code   
      ,t1.[ItemID]  AS item_desc   
      ,t1.[UnitPrice] AS item_rate
      ,'' AS qnty   
      ,'0' as ReturnQty
      ,t1.[MsrUnitCode]  AS msr_unit_code    
      ,isnull(t1.Quantity,0)-isnull(t1.ReturnQuantity,0) AS[PvQty]  
  FROM [ItemPurchaseLocalDtl] t1 where t1.[ItemPurchaseMstID]='" + PVID + "' and  t1.[ItemID]='" + ItemsID + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseLocalDtl");
        return dt;
    }

    public static PVReturn getShowRetirnItems(string RId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID],t1.[GRN],CONVERT(nvarchar,t1.[ReturnDate],103) AS ReturnDate,t1.[Remarks],t1.[Return_No],t1.TotalAmount,CASE WHEN t2.PayMethod IS NULL THEN 'C' ELSE t2.PayMethod END AS [PaymentMethod],t2.Bank_id AS[BankName],t2.[ChequeNo],CONVERT(NVARCHAR,t2.[ChequeDate],103) as ChequeDate ,t2.Chk_Status,ISNULL(t2.PayAmt,0) AS Pay_Amount FROM [PurReturnMstLocal] t1 left join SupplierPayment t2 on t2.purchase_id=t1.ID and t2.Payment_Type='PR' Where t1.[ID]='" + RId + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "OtMaster");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new PVReturn(dt.Rows[0]);
    }

    public static int SavePurchaseReturn(PVReturn rtn, DataTable dt,VouchMst _aVouchMst, string SupplierCoaCode, string UserType, string CurrencyRate)
    {
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

            command.CommandText = @"INSERT INTO [PurReturnMstLocal]
           ([GRN],[ReturnDate],[Remarks],[CreatedBy],[CreatedDate],Return_No,[TotalAmount],[Pay_Amount])
     VALUES
           ('" + rtn.GRN + "',convert(date,'" + rtn.ReturnDate + "',103),'" + rtn.Remarks + "','" + rtn.LogonBy +
                                  "',GETDATE(),'" + rtn.Return_No + "','" + rtn.TotalAmount + "','" + rtn.Pay_Amount +
                                  "')";
            command.ExecuteNonQuery();
            command.CommandText = @"SELECT top(1) [ID]  FROM [PurReturnMstLocal] order by ID desc";
            PurchaseMstID = command.ExecuteScalar().ToString();
            //*************************** Return Items ******************//
            decimal totPay = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["item_desc"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [PurReturnDlLocal]
                    ([PurReturnMstLocalID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate])  
                    VALUES ('" + PurchaseMstID + "','" + dr["item_desc"].ToString() + "','" + dr["item_rate"].ToString() +
                                          "','" + dr["qnty"].ToString() + "','" +
                                          Convert.ToDouble(dr["item_rate"])*Convert.ToDouble(dr["qnty"]) + "','" +
                                          rtn.LogonBy + "',GETDATE())";
                    command.ExecuteNonQuery();
                }
            }

            //***************************  Jurnal Voucher ********************************// 
            command.CommandText = VouchManager.SaveVoucherMst(_aVouchMst, 1);
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
                    vdtl.GlCoaCode = "1-" + SupplierCoaCode;
                    vdtl.Particulars = rtn.SupplierName;
                    vdtl.AccType = VouchManager.getAccType("1-" + SupplierCoaCode);
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
                    vdtl.GlCoaCode =dtFixCode.Rows[0]["PHLocalPurchaseReturn"].ToString(); //**** Purchase Code *******//
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["PHLocalPurchaseReturn"].ToString()); //**** Purchase Code *******//
                    vdtl.Particulars = "PH Local Purchase Return";
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
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = rtn.TotalAmount.Replace(",", "");
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
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
        if (string.IsNullOrEmpty(PurchaseMstID))
        {
            return 0;
        }
        return Convert.ToInt32(PurchaseMstID);
    }
    public static void UpdatePurchaseReturn(PVReturn rtn, DataTable dt, DataTable dtOldPvLocal, VouchMst _aVouchMst, string SupplierCoaCode, string UserType, string CurrencyRate)
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

            command.CommandText = @"UPDATE [PurReturnMstLocal]
             SET [ReturnDate] =convert(date, '" + rtn.ReturnDate + "',103),[Remarks] ='" + rtn.Remarks.Replace(",","") + "' ,[ModifiedBy] ='" + rtn.LogonBy +
                                  "' ,[ModifiedDate] =GETDATE(),[TotalAmount] ='" + rtn.TotalAmount +
                                  "',[Pay_Amount] ='" + rtn.Pay_Amount + "' WHERE ID='" + rtn.ID + "' ";
            command.ExecuteNonQuery();
            foreach (DataRow drOld in dtOldPvLocal.Rows)
            {
                if (drOld["ID"].ToString() != "" && drOld["item_desc"].ToString() != "")
                {
                    command.CommandText = @"DELETE FROM [PurReturnDlLocal] WHERE ItemID='" +
                                          drOld["item_desc"].ToString() + "' and PurReturnMstLocalID='" + rtn.ID + "'";
                    command.ExecuteNonQuery();
                }
            }

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["ID"].ToString() != "" && dr["item_desc"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [PurReturnDlLocal]
                    ([PurReturnMstLocalID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate])  
                    VALUES ('" + rtn.ID + "','" + dr["item_desc"].ToString() + "','" + dr["item_rate"].ToString() +
                                          "','" + dr["qnty"].ToString() + "','" +
                                          Convert.ToDouble(dr["item_rate"])*Convert.ToDouble(dr["qnty"]) + "','" +
                                          rtn.LogonBy + "',GETDATE())";
                    command.ExecuteNonQuery();
                }
            }

            //******************* Update Journal Voucher **********//

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
                    vdtl.GlCoaCode = "1-" + SupplierCoaCode;
                    vdtl.Particulars = rtn.SupplierName;
                    vdtl.AccType = VouchManager.getAccType("1-" + SupplierCoaCode);
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
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["PHLocalPurchaseReturn"].ToString(); //**** Purchase Code *******//
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["PHLocalPurchaseReturn"].ToString()); //**** Purchase Code *******//
                    vdtl.Particulars = "PH Local Purchase Return";
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
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = rtn.TotalAmount.Replace(",", "");
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
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

    public static DataTable GetShowPurchaseReturnItems()
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT TOP(50) t1.[ID]
      ,t2.GRN 
      ,t1.[Return_No],t3.ContactName
      ,CONVERT(NVARCHAR, t1.[ReturnDate],103) AS [ReturnDate]
      ,t1.[Remarks]   
      ,REPLACE(CONVERT(varchar(20), (CAST (t1.TotalAmount as money)), 1), '.00', '')+'.00' as TotalAmount
  FROM [PurReturnMstLocal] t1 inner join ItemPurchaseLocalMst t2 on t2.ID=t1.GRN inner join supplier t3 on t3.id=t2.SupplierID ORDER BY t1.[ID] DESC";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseLocalDtl");
        return dt;
    }

    public static DataTable ItemsDetails(string MstID)
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
      ,t1.[Quantity] AS ReturnQty
      ,isnull(totPvQty.Quantity,0)-isnull(t1.Quantity,0) AS[PvQty]
  FROM [PurReturnDlLocal] t1 inner join PurReturnMstLocal tt on tt.ID=t1.PurReturnMstLocalID left join Item t2 on t2.ID=t1.ItemID left join Brand t3 on t3.ID=t2.Brand
  left JOIN (select t2.ID,t1.ItemID,t1.Quantity from ItemPurchaseLocalDtl t1 inner join ItemPurchaseLocalMst t2 on t2.ID=t1.ItemPurchaseMstID)totPvQty on totPvQty.ID=tt.GRN and totPvQty.ItemID=t1.ItemID where t1.[PurReturnMstLocalID]='" + MstID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "PurReturnDlLocal");
        return dt;
    }

    public static void DeleteItemsReturn(PVReturn rtn, DataTable dtOldPvLocal)
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

            string Query = @"select t1.VCH_SYS_NO  from [GL_TRANS_MST] t1 where t1.SERIAL_NO='" + rtn.Return_No + "' and t1.PAYEE='PVR' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemPurchaseLocalMst");

            command.CommandText = @"DELETE FROM [GL_TRANS_MST]  WHERE SERIAL_NO='" + rtn.Return_No + "' and PAYEE='PVR' ";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                command.CommandText = @"DELETE FROM [GL_TRANS_DTL]  WHERE VCH_SYS_NO='" + dr["VCH_SYS_NO"].ToString() + "'";
                command.ExecuteNonQuery();
            }

            foreach (DataRow drOld in dtOldPvLocal.Rows)
            {
                if (drOld["ID"].ToString() != "" && drOld["item_desc"].ToString() != "")
                {
                    command.CommandText = @"DELETE FROM [PurReturnDlLocal] WHERE ItemID='" +
                                          drOld["item_desc"].ToString() + "' and PurReturnMstLocalID='" + rtn.ID + "'";
                    command.ExecuteNonQuery();
                }
            }

            command.CommandText = @"DELETE FROM [PurReturnMstLocal] WHERE ID='" + rtn.ID + "'";
            command.ExecuteNonQuery();

            //command.CommandText = @"DELETE FROM [PurReturnMstLocal] WHERE  ID='" + rtn.ID + "'";
            //command.ExecuteNonQuery();
            //command.CommandText = @"DELETE FROM [SupplierPayment] WHERE  purchase_id='" + rtn.ID + "' AND Payment_Type='PR' ";
            //command.ExecuteNonQuery();

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

    public static DataTable GetSupplierInfo(string Supplieer)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @" select ID,Code,Name,ContactName from Supplier where Upper(isnull(Code,'')+' - '+ContactName)=UPPER('" + Supplieer + "')";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
        return dt;
    }

    public static DataTable GetShowPurchaseReturnMst(string PRNo, string SupplierID, string ReceiveFromDate, string ReceiveToDate)
    {
        string per = "";

        if (!string.IsNullOrEmpty(PRNo))
        {
            per = "where  t1.[Return_No]='" + PRNo + "' ";
        }

        if (!string.IsNullOrEmpty(SupplierID) && string.IsNullOrEmpty(PRNo) && !string.IsNullOrEmpty(ReceiveFromDate) && !string.IsNullOrEmpty(ReceiveToDate))
        {

            per = "where t3.ID='" + SupplierID + "' and  (CONVERT(date,t1.[ReturnDate],103) between CONVERT(date,'" + ReceiveFromDate + "',103) and CONVERT(date,'" + ReceiveToDate + "',103))";
        }
        //&& (string.IsNullOrEmpty(GrNo) | string.IsNullOrEmpty(ReceiveFromDate) | string.IsNullOrEmpty(ReceiveFromDate)))
        if (!string.IsNullOrEmpty(SupplierID) && string.IsNullOrEmpty(PRNo) && (string.IsNullOrEmpty(ReceiveFromDate) | string.IsNullOrEmpty(ReceiveToDate)))
        {
            per = "where t3.ID='" + SupplierID + "'";
        }

        if (string.IsNullOrEmpty(SupplierID) && string.IsNullOrEmpty(PRNo) && !string.IsNullOrEmpty(ReceiveFromDate) && !string.IsNullOrEmpty(ReceiveToDate))
        {

            per = "where  (CONVERT(date,t1.[ReturnDate],103) between CONVERT(date,'" + ReceiveFromDate + "',103) and CONVERT(date,'" + ReceiveToDate + "',103))";
        }
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT TOP(50) t1.[ID]
      ,t2.GRN 
      ,t1.[Return_No],t3.ContactName
      ,CONVERT(NVARCHAR, t1.[ReturnDate],103) AS [ReturnDate]
      ,t1.[Remarks] 
      ,t1.TotalAmount  
  FROM [PurReturnMstLocal] t1 inner join ItemPurchaseLocalMst t2 on t2.ID=t1.GRN inner join supplier t3 on t3.id=t2.SupplierID  " + per + " order By t1.ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "PurReturnMst");
        return dt;
    }
}