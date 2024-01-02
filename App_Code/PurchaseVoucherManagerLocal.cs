using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using Delve;

/// <summary>
/// Summary description for PurchaseVoucherManager
/// </summary>
public class PurchaseVoucherManagerLocal
{
    SqlTransaction transaction;
    SqlConnection Connection = new SqlConnection(DataManager.OraConnString());
    public PurchaseVoucherManagerLocal()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static PurchaseVoucherInfoLocal GetPurchaseMst(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.[ID],t1.[GRN],CONVERT(NVARCHAR,t1.[ReceivedDate],103) as ReceivedDate,t1.[PO],CONVERT(NVARCHAR,t1.[PODate],103) as PODate,t1.[ChallanNo],CONVERT(NVARCHAR,t1.[ChallanDate],103) as ChallanDate,t1.[SupplierID],t1.[Remarks],ISNULL(t1.[Total],0)Total,t1.[CarriagePerson],t1.[LaburePerson],ISNULL(t1.[OtherCharge],0) as OtherCharge,ISNULL(t1.[CarriageCharge],0) as CarriageCharge,ISNULL(t1.[LabureCharge],0) as LabureCharge,ISNULL(t1.[TotalPayment],0) as TotalPayment,t1.ShiftmentID,t3.ShiftmentNO,t1.PartyID
,CASE WHEN t2.PayMethod='Q' and (t2.Chk_Status='P' OR t2.Chk_Status='B') THEN isnull(t1.[Total],0) else (isnull(t1.[Total],0)-isnull(t2.PayAmt,0)) end AS Due
,CASE WHEN t2.PayMethod IS NULL THEN 'C' ELSE t2.PayMethod END AS [PaymentMethod],t2.Bank_id AS[BankName],t2.[ChequeNo],CONVERT(NVARCHAR,t2.[ChequeDate],103) as ChequeDate ,t1.[ChequeAmount],t2.Chk_Status,t1.AdvancePayFlag,t1.BranchID FROM [ItemPurchaseLocalMst] t1 left join SupplierPayment t2 on t2.purchase_id=t1.ID and t2.Payment_Type='PV' left join ShiftmentAssigen t3 on t3.ID=t1.ShiftmentID Where t1.ID='" + ID + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseLocalMst");
        sqlCon.Close();
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new PurchaseVoucherInfoLocal(dt.Rows[0]);
    }

    public static int SavePurchaseVoucher(PurchaseVoucherInfoLocal purmst, DataTable dt,string ORID,VouchMst _aVouchMst, string SupplierCoaCode, string UserType,string CurrencyRate)
    {
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        string PurchaseMstID = "";
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"INSERT INTO [ItemPurchaseLocalMst]
           ([GRN],[ReceivedDate],[PO],[PODate],[ChallanNo],[ChallanDate],[SupplierID],[Remarks],[Total],[CreatedBy],[CreatedDate],[CarriagePerson],[LaburePerson],[OtherCharge],[CarriageCharge],[LabureCharge],[TotalPayment],PartyID,ShiftmentID,AdvancePayFlag,BranchID)
     VALUES
          ('" + purmst.GoodsReceiveNo + "',convert(date,'" + purmst.GoodsReceiveDate + "',103),'" + purmst.PurchaseOrderNo + "',convert(date,'" + purmst.PurchaseOrderDate + "',103),'" + purmst.ChallanNo + "',convert(date,'" + purmst.ChallanDate + "',103),'" + purmst.Supplier + "','" + purmst.Remarks + "','" + purmst.TotalAmount + "','" + purmst.LoginBy + "',GETDATE(),'" + purmst.CarriagePerson + "','" + purmst.LaburePerson + "','" + purmst.OtherCharge + "','" + purmst.CarriageCharge + "','" + purmst.LabureCharge + "','" + purmst.TotalPayment + "','" + purmst.PartyID + "','" + purmst.ShiftmentID + "','" + purmst.AdvancePayFlag + "','" + purmst.BranchID + "')";
            command.ExecuteNonQuery();
            command.CommandText = @"SELECT top(1) [ID]  FROM [ItemPurchaseLocalMst] order by ID desc";
            PurchaseMstID = command.ExecuteScalar().ToString();
            //***********  Items Details  *********//  
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["item_code"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [ItemPurchaseLocalDtl]
           ([ItemPurchaseMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],[MsrUnitCode],Additional)
     VALUES
           ('" + PurchaseMstID + "','" + dr["ID"].ToString() + "','" + dr["item_rate"].ToString() + "','" +
                                          dr["qnty"].ToString() + "','" +
                                          Convert.ToDouble(dr["item_rate"].ToString())*
                                          Convert.ToDouble(dr["qnty"].ToString()) + "','" + purmst.LoginBy +
                                          "',GETDATE(),'" + dr["msr_unit_code"].ToString() + "','" +
                                          dr["Additional"].ToString() + "')";
                    command.ExecuteNonQuery();
                }
            }

            //******************** Jurnal Voucher *********************// 
            command.CommandText = VouchManager.SaveVoucherMst(_aVouchMst, 1);
            command.ExecuteNonQuery();
            VouchDtl vdtl;
            for (int j = 0; j < 3; j++)
            {
                if (j == 0)
                {
                    //DataRow 
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = purmst.GoodsReceiveDate;
                    vdtl.LineNo = "1";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["PHLocalPurchase"].ToString(); //**** Purchase Code *******//
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["PHLocalPurchase"].ToString()); //**** Purchase Code *******//
                    vdtl.Particulars = "PH Local Purchase";
                    vdtl.AmountDr = purmst.TotalAmount.Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    //*********** Convert Rate ********//
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 1)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = purmst.GoodsReceiveDate;
                    vdtl.LineNo = "2";
                    vdtl.GlCoaCode = "1-" + SupplierCoaCode;
                    vdtl.Particulars = purmst.SupplierName;
                    vdtl.AccType = VouchManager.getAccType("1-" + SupplierCoaCode);
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = purmst.TotalAmount.Replace(",", "");
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    //*********** Convert Rate ********//
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 2)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = purmst.GoodsReceiveDate;
                    vdtl.LineNo = "3";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
                    vdtl.Particulars = "Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
                    vdtl.AmountDr = purmst.TotalAmount.Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    //*********** Convert Rate ********//
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

    public static DataTable GetShowPurchaseMst(string BranchID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.ID,t1.[GRN]
      ,CONVERT(NVARCHAR,t1.[ReceivedDate],103) AS ReceivedDate   
      ,t1.[ChallanNo]
      ,CONVERT(NVARCHAR,t1.[ChallanDate],103) AS ChallanDate
      ,t2.ContactName as Name
      ,REPLACE(CONVERT(varchar(20), (CAST (t1.Total as money)), 1), '.00', '')+'.00' as   Total
      ,t3.PartyName AS Party    
  FROM [ItemPurchaseLocalMst] t1 inner join Supplier t2 on t2.ID=t1.SupplierID left join PartyInfo t3 on t3.ID=t1.PartyID WHERE t1.BranchID='" + BranchID + "' order By t1.ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseLocalMst");
        return dt;
    }



    public static DataTable GetPurchaseItemsDetails(string ItemsID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.[ItemID] AS ID
      ,t2.Code AS item_code
      ,t2.Name AS item_desc
      ,ISNULL(t1.[UnitPrice],0) AS item_rate
      ,convert(decimal(18,0),ISNULL(t1.[Quantity],0)) AS qnty
      ,ISNULL(t1.[Total],0) AS Total    
      ,t1.[MsrUnitCode] AS msr_unit_code
      ,t3.Name AS UMO
      ,ISNULL(Additional,0) AS [Additional]
      ,t4.BrandName
  FROM [ItemPurchaseLocalDtl] t1 inner join Item t2 on t2.ID=t1.ItemID inner join UOM t3 on t3.ID=t1.MsrUnitCode left join Brand t4 on t4.ID=t2.Brand where t1.[ItemPurchaseMstID]='" + ItemsID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseLocalDtl");
        return dt;
    }

    public static void DeletePurchaseVoucher(PurchaseVoucherInfoLocal purmst, DataTable dtOldPvLocal)
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

            string Query = @"select t1.VCH_SYS_NO  from [GL_TRANS_MST] t1 where t1.SERIAL_NO='" + purmst.GoodsReceiveNo + "' and t1.PAYEE='PV' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemPurchaseLocalMst");

            command.CommandText = @"DELETE FROM [GL_TRANS_MST]  WHERE SERIAL_NO='" + purmst.GoodsReceiveNo + "' and PAYEE='PV' ";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dt.Rows)
            {
                command.CommandText = @"DELETE FROM [GL_TRANS_DTL]  WHERE VCH_SYS_NO='" + dr["VCH_SYS_NO"].ToString() + "'";
                command.ExecuteNonQuery();
            }

            if (dtOldPvLocal != null)
            {
                foreach (DataRow drOld in dtOldPvLocal.Rows)
                {
                    if (drOld["item_code"].ToString() != "")
                    {
                        command.CommandText = @"delete from [ItemPurchaseLocalDtl] where [ItemID]='" +
                                              drOld["ID"].ToString() + "' and ItemPurchaseMstID='" + purmst.ID +
                                              "'";
                        command.ExecuteNonQuery();
                    }
                }
            }

            command.CommandText = @"DELETE FROM [ItemPurchaseLocalMst] WHERE  ID='" + purmst.ID + "'";
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

    public static void UpdatePurchaseVoucher(PurchaseVoucherInfoLocal purmst, DataTable dt, DataTable dtOldPvLocal, VouchMst _aVouchMst, string SupplierCoaCode, string UserType, string CurrencyRate)
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
            command.CommandText = @"UPDATE [ItemPurchaseLocalMst]
            SET [ReceivedDate] =convert(date,'" + purmst.GoodsReceiveDate + "',103),[PO] ='" + purmst.PurchaseOrderNo +
                                  "',[PODate] =convert(date,'" + purmst.PurchaseOrderDate + "',103),[ChallanNo] ='" +
                                  purmst.ChallanNo + "',[ChallanDate] =convert(date,'" + purmst.ChallanDate +
                                  "',103),[SupplierID] ='" + purmst.Supplier + "',[Remarks] ='" + purmst.Remarks +
                                  "',[Total] ='" + purmst.TotalAmount + "',[ModifiedBy] ='" + purmst.LoginBy +
                                  "' ,[ModifiedDate] =GETDATE(),[CarriagePerson] ='" + purmst.CarriagePerson +
                                  "',[LaburePerson] ='" + purmst.LaburePerson + "',[OtherCharge] ='" +
                                  purmst.OtherCharge + "',[CarriageCharge] ='" + purmst.CarriageCharge +
                                  "',[LabureCharge] ='" + purmst.LabureCharge + "',[TotalPayment] ='" +
                                  purmst.TotalPayment + "',ShiftmentID='" + purmst.ShiftmentID + "',BranchID='" +
                                  purmst.BranchID + "'  WHERE ID='" + purmst.ID + "' ";
            command.ExecuteNonQuery();
            foreach (DataRow drOld in dtOldPvLocal.Rows)
            {
                if (drOld["item_code"].ToString() != "")
                {
                    command.CommandText = @"delete from [ItemPurchaseLocalDtl] where [ItemID]='" +
                                          drOld["ID"].ToString() + "' and ItemPurchaseMstID='" + purmst.ID +
                                          "'";
                    command.ExecuteNonQuery();
                }
            }
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["item_code"].ToString() != "")
                {
                    command.CommandText = @"INSERT INTO [ItemPurchaseLocalDtl]
                    ([ItemPurchaseMstID],[ItemID],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],[MsrUnitCode],Additional)
                    VALUES
                     ('" + purmst.ID + "','" + dr["ID"].ToString() + "','" + dr["item_rate"].ToString() + "','" +
                                          dr["qnty"].ToString() + "','" +
                                          Convert.ToDouble(dr["item_rate"].ToString())*
                                          Convert.ToDouble(dr["qnty"].ToString()) + "','" + purmst.LoginBy +
                                          "',GETDATE(),'" + dr["msr_unit_code"].ToString() + "','" +
                                          dr["Additional"].ToString() + "')";
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
                    //DataRow 
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = purmst.GoodsReceiveDate;
                    vdtl.LineNo = "1";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["PHLocalPurchase"].ToString(); //**** Purchase Code *******//
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["PHLocalPurchase"].ToString()); //**** Purchase Code *******//
                    vdtl.Particulars = "PH Local Purchase";
                    vdtl.AmountDr = purmst.TotalAmount.Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    //*********** Convert Rate ********//
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 1)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = purmst.GoodsReceiveDate;
                    vdtl.LineNo = "2";
                    vdtl.GlCoaCode = "1-" + SupplierCoaCode;
                    vdtl.Particulars = purmst.SupplierName;
                    vdtl.AccType = VouchManager.getAccType("1-" + SupplierCoaCode);
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = purmst.TotalAmount.Replace(",", "");
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    //*********** Convert Rate ********//
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMst, vdtl, command);
                }
                else if (j == 2)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = _aVouchMst.VchSysNo;
                    vdtl.ValueDate = purmst.GoodsReceiveDate;
                    vdtl.LineNo = "3";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
                    vdtl.Particulars = "Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
                    vdtl.AmountDr = purmst.TotalAmount.Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = _aVouchMst.Status;
                    vdtl.BookName = _aVouchMst.BookName;
                    //*********** Convert Rate ********//
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

    //************** Report Query **************//

    public static DataTable GetShowPurchaeReport(string CurTime, string StrDate, string EndDate, string SupploerID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string Paramater = "";
        if (CurTime != "")
        {
            Paramater = "where CONVERT(NVARCHAR,[ReceivedDate],103)='" + CurTime + "'";
        }
        if (StrDate != "" && EndDate != "" && SupploerID == "0")
        {
            Paramater = "where CONVERT(DATE,[ReceivedDate],103) between CONVERT(DATE,'" + StrDate +
                        "',103) and CONVERT(DATE,'" + EndDate + "',103)";
        }
        if (StrDate != "" && EndDate != "" && SupploerID != "0")
        {
            Paramater = "where [Sup_ID]='" + SupploerID +
                        "' and CONVERT(DATE,[ReceivedDate],103) between CONVERT(DATE,'" + StrDate +
                        "',103) and CONVERT(DATE,'" + EndDate + "',103)";
        }
        if (SupploerID != "0" && StrDate == "" && EndDate == "")
        {
            Paramater = "where [Sup_ID]='" + SupploerID + "'";
        }
        string query = @"SELECT * FROM [ItemPurchaseForReport]  " + Paramater;
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseLocalDtl");
        return dt;
    }

    public static DataTable GetShowSalesReportReport(string CurTime, string StrDate, string EndDate, string CustomerID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string Paramater = "";
        if (CurTime != "") { Paramater = "where CONVERT(NVARCHAR,[OrderDate],103)='" + CurTime + "'"; }
        if (StrDate != "" && EndDate != "" && CustomerID == "0") { Paramater = "where CONVERT(DATE,[OrderDate],103) between CONVERT(DATE,'" + StrDate + "',103) and CONVERT(DATE,'" + EndDate + "',103)"; }
        if (StrDate != "" && EndDate != "" && CustomerID != "0") { Paramater = "where [CustomerID]='" + CustomerID + "' and CONVERT(DATE,[OrderDate],103) between CONVERT(DATE,'" + StrDate + "',103) and CONVERT(DATE,'" + EndDate + "',103)"; }        
        if (CustomerID != "0" && StrDate == "" && EndDate == "") { Paramater = "where [CustomerID]='" + CustomerID + "'"; }
        string query = @"SELECT * FROM [OrderInformationForReport]  " + Paramater;
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseLocalDtl");
        return dt;
    }

    public static DataTable GetShowStock(string Type, string Catagory, string SubCat)
    {
        string connectionString = DataManager.OraConnString();
        string parameter = "";
        if (Type == "all")
        {
            if (Catagory != "" && SubCat == "")
            {
                parameter = "where t1.CategoryID='" + Catagory + "'";
            }
            else if (Catagory != "" && SubCat != "")
            {
                parameter = "where t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "'";
            }
            else
            {
                parameter = "order by t1.ID asc";
            }
        }
        else if (Type == "Available")
        {
            if (Catagory != "" && SubCat == "")
            {
                parameter = "where t1.ClosingStock>0 and t1.CategoryID='" + Catagory + "'";
            }
            else if (Catagory != "" && SubCat != "")
            {
                parameter = "where t1.ClosingStock>0 and t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "' ";
            }
            else
            {
                parameter = "where t1.ClosingStock>0 order by t1.ID asc";
            }
        }
        else if (Type == "Unavailable")
        {
            if (Catagory != "" && SubCat == "")
            {
                parameter = "where t1.ClosingStock<=0 and t1.CategoryID='" + Catagory + "'";
            }
            else if (Catagory != "" && SubCat != "")
            {
                parameter = "where t1.ClosingStock<=0 and t1.CategoryID='" + Catagory + "' and t1.SubCategoryID='" + SubCat + "' ";
            }
            else
            {
                parameter = "where t1.ClosingStock<=0 order by t1.ID asc";
            }
        }
        string selectQuery = @"SELECT t1.[Code]+' - '+t1.[Name] AS Items  
	  ,t4.BrandName
      ,t2.Name AS Catagory
      ,t3.Name AS SubCat
      ,t5.Name AS UMO
      ,t1.[UnitPrice]     
      ,t1.[OpeningStock]
      ,t1.[OpeningAmount]
      ,t1.[ClosingStock]
      ,t1.[ClosingAmount]          
  FROM [Item] t1 left join Category t2 on t2.ID=t1.CategoryID left join SubCategory t3 on t3.ID=t1.SubCategoryID left join Brand t4 on t4.ID=t1.Brand left join UOM t5 on t5.ID=t1.UOMID " + parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }

    public static DataTable getShowTotalItemsStockByDate(string StrDt, string EndDt,string Date)
    {
        DataSet ds = new DataSet();
        string connectionString = DataManager.OraConnString();
        using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
        {

            if (EndDt == "" && StrDt == "") 
            { 
                StrDt = DateTime.Now.ToString("dd/MM/yyyy"); 
            }
            SqlCommand sqlComm = new SqlCommand("Total_Product_Details", conn);
            if (Date != "")
            {
                sqlComm.Parameters.AddWithValue("@StartDate", DataManager.DateEncode(Date));
                sqlComm.Parameters.AddWithValue("@EndDate", DataManager.DateEncode(Date));
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@StartDate", DataManager.DateEncode(StrDt));
                sqlComm.Parameters.AddWithValue("@EndDate", DataManager.DateEncode(EndDt));
            }
            sqlComm.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "Total_Product_Details");
            ds.Tables[0].TableName = "Total_Product_Details";
            return ds.Tables[0];
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

    public static DataTable GetShowPurchaseMst(string GrNo, string SupplierCode, string ReceiveFromDate, string ReceiveToDate)
    {
        string per = "";

        if (!string.IsNullOrEmpty(GrNo))
        {
            per = "where  t1.GRN='" + GrNo + "' ";
        }

        if (!string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) && !string.IsNullOrEmpty(ReceiveFromDate) && !string.IsNullOrEmpty(ReceiveToDate))
        {

            per = "where t2.ID='" + SupplierCode + "' and  (CONVERT(date,t1.ReceivedDate,103) between CONVERT(date,'" + ReceiveFromDate + "',103) and CONVERT(date,'" + ReceiveToDate + "',103))";
        }
        //&& (string.IsNullOrEmpty(GrNo) | string.IsNullOrEmpty(ReceiveFromDate) | string.IsNullOrEmpty(ReceiveFromDate)))
        if (!string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) && (string.IsNullOrEmpty(ReceiveFromDate) | string.IsNullOrEmpty(ReceiveToDate)))
        {
            per = "where t2.ID='" + SupplierCode + "'";
        }

        if (string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) && !string.IsNullOrEmpty(ReceiveFromDate) && !string.IsNullOrEmpty(ReceiveToDate))
        {

            per = "where  (CONVERT(date,t1.ReceivedDate,103) between CONVERT(date,'" + ReceiveFromDate + "',103) and CONVERT(date,'" + ReceiveToDate + "',103))";
        }
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.ID,t1.[GRN]
      ,CONVERT(NVARCHAR,t1.[ReceivedDate],103) AS ReceivedDate   
      ,t1.[ChallanNo]
      ,CONVERT(NVARCHAR,t1.[ChallanDate],103) AS ChallanDate
      ,t2.ContactName as Name
      ,t1.[Total]  
      ,t3.PartyName AS Party    
  FROM [ItemPurchaseLocalMst] t1 inner join Supplier t2 on t2.ID=t1.SupplierID left join PartyInfo t3 on t3.ID=t1.PartyID " + per + "  order By t1.ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseMst");
        return dt;
    }
}