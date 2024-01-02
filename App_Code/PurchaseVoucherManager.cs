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
public class PurchaseVoucherManager
{
    SqlTransaction transaction;
    SqlConnection Connection = new SqlConnection(DataManager.OraConnString());
	public PurchaseVoucherManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static PurchaseVoucherInfo GetPurchaseMst(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.[ID],t1.[GRN],CONVERT(NVARCHAR,t1.[ReceivedDate],103) as ReceivedDate,t1.[PO],CONVERT(NVARCHAR,t1.[PODate],103) as PODate,t1.[ChallanNo],CONVERT(NVARCHAR,t1.[ChallanDate],103) as ChallanDate,t1.[SupplierID],t1.[Remarks],ISNULL(t1.[Total],0)Total,t1.[CarriagePerson],t1.[LaburePerson],ISNULL(t1.[OtherCharge],0) as OtherCharge,ISNULL(t1.[CarriageCharge],0) as CarriageCharge,ISNULL(t1.[LabureCharge],0) as LabureCharge,ISNULL(t1.[TotalPayment],0) as TotalPayment,ISNULL(t1.[DiscountAmt],0) as DiscountAmt,t1.ShiftmentID,t3.ShiftmentNO,t1.PartyID
,CASE WHEN t2.PayMethod='Q' and (t2.Chk_Status='P' OR t2.Chk_Status='B') THEN isnull(t1.[Total],0) else (isnull(t1.[Total],0)-isnull(t1.TotalPayment,0)-isnull(t1.DiscountAmt,0)) end AS Due
,t1.PaymentMethod AS [PaymentMethod],t1.BankName AS[BankName],t1.[ChequeNo],CONVERT(NVARCHAR,t1.[ChequeDate],103) as ChequeDate ,t1.[ChequeAmount],t2.Chk_Status,t1.AdvancePayFlag,t1.PvType,t1.PvOrder FROM [ItemPurchaseMst] t1 left join SupplierPayment t2 on t2.purchase_id=t1.ID and t2.Payment_Type='PV' left join ShiftmentAssigen t3 on t3.ID=t1.ShiftmentID Where t1.ID='" + ID + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseMst");
        sqlCon.Close();
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new PurchaseVoucherInfo(dt.Rows[0]);
    }

    public static int SavePurchaseVoucher(PurchaseVoucherInfo purmst, DataTable dt, string ORID, VouchMst _aVouchMst, string SupplierCoaCode, VouchMst _aVouchMstDV)
    {
        string PurchaseMstID = "";
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

            command.CommandText = @"INSERT INTO [ItemPurchaseMst]
           ([GRN],[ReceivedDate],[PO],[PODate],[ChallanNo],[ChallanDate],[SupplierID],[Remarks],[Total],[CreatedBy],[CreatedDate],[CarriagePerson],[LaburePerson],[OtherCharge],[CarriageCharge],[LabureCharge],[TotalPayment],DiscountAmt,PartyID,ShiftmentID,AdvancePayFlag,PvType,PvOrder,[PaymentMethod],[BankName],[ChequeNo],[ChequeDate],[ChequeAmount])
     VALUES
          ('" + purmst.GoodsReceiveNo + "',convert(date,'" + purmst.GoodsReceiveDate + "',103),'" +
                                  purmst.PurchaseOrderNo + "',convert(date,'" + purmst.PurchaseOrderDate + "',103),'" +
                                  purmst.ChallanNo + "',convert(date,'" + purmst.ChallanDate + "',103),'" +
                                  purmst.Supplier + "','" + purmst.Remarks + "','" + purmst.TotalAmount + "','" +
                                  purmst.LoginBy + "',GETDATE(),'" + purmst.CarriagePerson + "','" +
                                  purmst.LaburePerson +
                                  "','" + purmst.OtherCharge + "','" + purmst.CarriageCharge + "','" +
                                  purmst.LabureCharge + "','" + purmst.TotalPayment + "','"+purmst.DiscountAmt+"','" + purmst.PartyID + "','" +
                                  purmst.ShiftmentID + "','" + purmst.AdvancePayFlag + "','" + purmst.PvType + "','" +
                                  purmst.PvOrder + "','" + purmst.PaymentMethord + "','" + purmst.BankId + "','" +
                                  purmst.ChequeNo + "', convert(date,'" + purmst.ChequeDate + "',103),'" + purmst.TotalPayment + "')";
            command.ExecuteNonQuery();
            command.CommandText = @"SELECT top(1) [ID]  FROM [ItemPurchaseMst] order by ID desc";
            PurchaseMstID = command.ExecuteScalar().ToString();
            command.CommandText = "select PvCode from FixGlCoaCode";
            int PvCode = Convert.ToInt32(command.ExecuteScalar().ToString());
            //*************************** Item Purchase Dtl ********************************// 
            foreach (DataRow dr in dt.Rows)
            {
               

                if (dr["item_code"].ToString() != "")
                {
                   
                    string ExpireValue = "", ExpireData = "";

                    if (!string.IsNullOrEmpty(dr["Expdate"].ToString()))
                    {
                        ExpireData = ",ExpireDate";
                        ExpireValue = ",convert(date,'" + dr["Expdate"].ToString() + "',103)";
                    }
                    command.CommandText = "update FixGlCoaCode set PvCode=(select PvCode+1 from FixGlCoaCode)";
                    command.ExecuteNonQuery();
                    var ItemCode = Convert.ToInt64(dr["item_code"].ToString());
                    var Code = ItemCode + PvCode;
                    command.CommandText =
                        @"INSERT INTO [ItemPurchaseDtl]
           ([ItemPurchaseMstID],[ItemID],[Barcode],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],[MsrUnitCode],SalesPrice,ItemCode,Additional " +
                        ExpireData + ") VALUES ('" + PurchaseMstID + "','" + dr["ID"].ToString() + "','" + dr["Barcode"].ToString() + "','" +
                        dr["item_rate"].ToString() + "','" +
                        dr["qnty"].ToString() + "','" +
                        Convert.ToDouble(dr["item_rate"].ToString()) *
                        Convert.ToDouble(dr["qnty"].ToString()) + "','" + purmst.LoginBy +
                        "',GETDATE(),'" + dr["msr_unit_code"].ToString() + "','" + dr["item_sales_rate"].ToString() +
                        "','" +Code+ "','" + dr["Additional"].ToString() + "'" + ExpireValue + ")";
                    command.ExecuteNonQuery();

                    
                }
            }

            if (Convert.ToDouble(purmst.TotalPayment) > 0)
            {
                command.CommandText = @"INSERT INTO [dbo].[SupplierPaymentReceive]
           ([Date],[Supplier_id],[GRN],[PurchaseVoucherID],[PayAmt],[entry_by],[entry_date])
     VALUES
           (convert(datetime, nullif( '" + purmst.ChallanDate + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),''), 103),'" + purmst.Supplier + "','" + purmst.GoodsReceiveNo +
                                      "','" + PurchaseMstID +"','" + (Convert.ToDecimal(purmst.TotalPayment)) + "','" + purmst.LoginBy + "',GETDATE())";
                command.ExecuteNonQuery();


            }

            //***************************  Jurnal Voucher ********************************// 
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
                  
                        vdtl.GlCoaCode = dtFixCode.Rows[0]["BDItemPurchas"].ToString(); //**** Purchase Code *******//
                        vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["BDItemPurchas"].ToString()); //**** Purchase Code *******//
                        vdtl.Particulars = "Item Purchase";
                   
                    
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

            //***************************  Debid Voucher ********************************// 

            if (Convert.ToDecimal(purmst.TotalPayment) > 0)
            {
                _aVouchMstDV.ControlAmt = purmst.TotalPayment.Replace("'", "").Replace(",", "");
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
                        vdtlCR.ValueDate = purmst.GoodsReceiveDate;
                        vdtlCR.LineNo = "1";
                        vdtlCR.GlCoaCode = "1-" + SupplierCoaCode;
                        vdtlCR.Particulars = purmst.SupplierName;
                        vdtlCR.AccType = VouchManager.getAccType("1-" + SupplierCoaCode);
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
                        vdtlCR.ValueDate = purmst.GoodsReceiveDate;
                        vdtlCR.LineNo = "2";
                        if (string.IsNullOrEmpty(purmst.BankId) || purmst.BankId.Equals("0"))
                        {
                            vdtlCR.GlCoaCode ="1-"+ dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                            vdtlCR.AccType =
                                VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                    .ToString()); //**** SalesCode *******//
                            vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                        }
                        else
                        {

                            vdtlCR.GlCoaCode = "1-" + purmst.BankCoaCode; //**** SalesCode *******//
                            vdtlCR.AccType =
                                VouchManager.getAccType("1-" + purmst.BankCoaCode); //**** SalesCode *******//
                            vdtlCR.Particulars = purmst.BankName;
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

            command.CommandText = @"UPDATE [ItemPurOrderMst]  SET [OrderStatus] ='C'  WHERE ID='" + ORID + "'";
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

    public static DataTable GetShowPurchaseMst()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT top(100) t1.ID,t1.[GRN],t4.POCode
      ,CONVERT(NVARCHAR,t1.[ReceivedDate],103) AS ReceivedDate   
      ,t1.[ChallanNo]
      ,CONVERT(NVARCHAR,t1.[ChallanDate],103) AS ChallanDate
      ,isnull(t2.Name,'') +' - '+ t2.ContactName as Name
      ,REPLACE(CONVERT(varchar(20), (CAST (t1.[Total] as money)), 1), '.00', '') as Total     
      ,t3.PartyName AS Party    
  FROM [ItemPurchaseMst] t1 inner join Supplier t2 on t2.ID=t1.SupplierID left join PartyInfo t3 on t3.ID=t1.PartyID
  left join dbo.ItemPurOrderMst t4 on t4.id=t1.PO order By t1.ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseMst");
        return dt;
    }

    public static DataTable GetPurchaseItemsDetails(string ItemsID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.[ItemID] AS ID
      ,t2.Code AS item_code,t1.BarCode
      ,t2.Name AS item_desc
      ,ISNULL(t1.[UnitPrice],0) AS item_rate
      ,convert(decimal(18,0),ISNULL(t1.[Quantity],0)) AS qnty
      ,  REPLACE(CONVERT(varchar(20), (CAST (ISNULL(t1.[Total],0) as money)), 1), '.00', '')+'.00' as Total    
      ,t1.[MsrUnitCode] AS msr_unit_code
      ,t3.Name AS UMO
      ,ISNULL(Additional,0) AS [Additional]
      ,t4.BrandName
      ,convert(nvarchar,t1.[ExpireDate],103) as [ExpireDate]
      ,convert(nvarchar,t1.[ExpireDate],103) Expdate
      ,t1.SalesPrice AS item_sales_rate
      ,convert(nvarchar,t1.ID) AS[Items_Dtl_ID]
    FROM [ItemPurchaseDtl] t1 inner join Item t2 on t2.ID=t1.ItemID inner join UOM t3 on t3.ID=t1.MsrUnitCode left join Brand t4 on t4.ID=t2.Brand where t1.[ItemPurchaseMstID]='" + ItemsID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    public static void DeletePurchaseVoucher(PurchaseVoucherInfo purmst, DataTable dtOldVchDtl)
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
            DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemPurchaseMst");
            command.CommandText = @"DELETE FROM [GL_TRANS_MST]  WHERE SERIAL_NO='" + purmst.GoodsReceiveNo + "' and PAYEE='PV' ";
            command.ExecuteNonQuery();
            foreach (DataRow dr in dt.Rows)
            {
                command.CommandText = @"DELETE FROM [GL_TRANS_DTL]  WHERE VCH_SYS_NO='" + dr["VCH_SYS_NO"].ToString() + "'";
                command.ExecuteNonQuery();
            }

           
            if (dtOldVchDtl != null)
            {
                foreach (DataRow drold in dtOldVchDtl.Rows)
                {
                    command.CommandText = @"delete from [ItemPurchaseDtl] where ItemID='" + drold["ID"].ToString() +
                                          "' and ItemPurchaseMstID='" + purmst.ID + "'";
                    command.ExecuteNonQuery();
                }
            }

            command.CommandText = @"DELETE FROM [ItemPurchaseMst] WHERE  ID='" + purmst.ID + "'";
            command.ExecuteNonQuery();

            //command.CommandText = @"DELETE FROM [SupplierPayment] WHERE  purchase_id='" + purmst.ID + "' AND Payment_Type='PV' ";
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

    public static void UpdatePurchaseVoucher(PurchaseVoucherInfo purmst, DataTable dt, DataTable dtOldVchDtl, VouchMst _aVouchMst, string SupplierCoaCode, VouchMst _aVouchMstCR)
    {
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

            command.CommandText = @"UPDATE [ItemPurchaseMst]
            SET [ReceivedDate] =convert(date,'" + purmst.GoodsReceiveDate + "',103),[PO] ='" + purmst.PurchaseOrderNo +
                                  "',[PODate] =convert(date,'" + purmst.PurchaseOrderDate + "',103),[ChallanNo] ='" +
                                  purmst.ChallanNo + "',[ChallanDate] =convert(date,'" + purmst.ChallanDate +
                                  "',103),[SupplierID] ='" + purmst.Supplier + "',[Remarks] ='" + purmst.Remarks +
                                  "',[Total] ='" + purmst.TotalAmount + "',[ModifiedBy] ='" + purmst.LoginBy +
                                  "' ,[ModifiedDate] =GETDATE(),[CarriagePerson] ='" + purmst.CarriagePerson +
                                  "',[LaburePerson] ='" + purmst.LaburePerson + "',[OtherCharge] ='" +
                                  purmst.OtherCharge + "',[CarriageCharge] ='" + purmst.CarriageCharge +
                                  "',[LabureCharge] ='" + purmst.LabureCharge + "',[TotalPayment] ='" +
                                  purmst.TotalPayment + "',DiscountAmt='"+purmst.DiscountAmt+"',ShiftmentID='" + purmst.ShiftmentID + "',PvType='" +
                                  purmst.PvType + "',PvOrder='" + purmst.PvOrder + "',[PaymentMethod]='" +
                                  purmst.PaymentMethord + "',[BankName]='" + purmst.BankId + "',[ChequeNo]='" +
                                  purmst.ChequeNo + "',[ChequeDate]= convert(date,'" + purmst.ChequeDate + "',103),[ChequeAmount]='" +
                                  purmst.TotalPayment + "'  WHERE ID='" + purmst.ID + "' ";
            command.ExecuteNonQuery();

            foreach (DataRow drold in dtOldVchDtl.Rows)
            {
                if (drold["item_code"].ToString() != "")
                {
                    command.CommandText = @"delete from [ItemPurchaseDtl] where ItemID='" + drold["ID"].ToString() +
                                          "' and ItemPurchaseMstID='" + purmst.ID + "'";
                    command.ExecuteNonQuery();
                }
            }

            //*************************** Item Purchase Dtl ********************************// 
            command.CommandText = "select PvCode from FixGlCoaCode";
            int PvCode = Convert.ToInt32(command.ExecuteScalar().ToString());

            foreach (DataRow dr in dt.Rows)
            {
                //>>>>>

                if (dr["item_code"].ToString() != "")
                {
                    var ItemCode = Convert.ToInt64(dr["item_code"].ToString());
                    var Code = ItemCode + PvCode;
                    string ExpireValue = "", ExpireData = "";

                    if (!string.IsNullOrEmpty(dr["Expdate"].ToString()))
                    {
                        ExpireData = ",ExpireDate";
                        ExpireValue = ",convert(date,'" + dr["Expdate"].ToString() + "',103)";
                    }

                    var a = dr["qnty"].ToString();
                    command.CommandText =
                        @"INSERT INTO [ItemPurchaseDtl]
           ([ItemPurchaseMstID],[ItemID],[Barcode],[UnitPrice],[Quantity],[Total],[CreatedBy],[CreatedDate],[MsrUnitCode],SalesPrice,ItemCode,Additional " +
                        ExpireData + ") VALUES ('" + purmst.ID + "','" + dr["ID"].ToString() + "','" + dr["Barcode"].ToString() + "','" +
                        dr["item_rate"].ToString() + "','" +
                        dr["qnty"].ToString() + "','" +
                        Convert.ToDouble(dr["item_rate"].ToString()) *
                        Convert.ToDouble(dr["qnty"].ToString()) + "','" + purmst.LoginBy +
                        "',GETDATE(),'" + dr["msr_unit_code"].ToString() + "','" + dr["item_sales_rate"].ToString() +
                        "','" + Code + "','" + dr["Additional"].ToString() + "'" + ExpireValue + ")";
                    command.ExecuteNonQuery();

                    command.CommandText = "update FixGlCoaCode set PvCode=(select PvCode+1 from FixGlCoaCode)";
                    command.ExecuteNonQuery();
                }
            }
            if (Convert.ToDouble(purmst.TotalPayment) > 0)
            {
                command.CommandText = @"select count(*) from  [dbo].[SupplierPaymentReceive] where [PurchaseVoucherID]='" + purmst.ID + "'";
                int CheckDues = Convert.ToInt32(command.ExecuteScalar());
                command.CommandType = CommandType.Text;

                if (CheckDues > 0)
                {
                    command.CommandText = @"UPDATE [dbo].[SupplierPaymentReceive]
                       SET [Date] = convert(date,'" + purmst.ChallanDate + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103),[Supplier_id] ='" +
                                          purmst.Supplier + "',[PayAmt] ='" + (Convert.ToDecimal(purmst.TotalPayment)) + "' ,[update_by] ='" +
                                          purmst.LoginBy + "' ,[update_date] = GETDATE() WHERE [PurchaseVoucherID]='" +
                                          purmst.ID + "' ";
                    command.ExecuteNonQuery();
                }
                else
                {
                    command.CommandText = @"INSERT INTO [dbo].[SupplierPaymentReceive]
           ([Date],[Supplier_id],[PurchaseVoucherID],GRN,[PayAmt],[entry_by],[entry_date])
     VALUES
           (convert(datetime, nullif( '" + purmst.ChallanDate + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),''), 103),'" + purmst.Supplier + "','" + purmst.ID +
                                          "','"+purmst.GoodsReceiveNo+"','" + (Convert.ToDecimal(purmst.TotalPayment)) + "','" + purmst.LoginBy + "',GETDATE())";
                    command.ExecuteNonQuery();


                    //                    command.CommandText = @"INSERT INTO [dbo].[SupplierPaymentReceive]
                    //           ([Date],BranchId,[Customer_id],[Invoice],[PayAmt],[entry_by],[entry_date])
                    //     VALUES
                    //           (convert(datetime, nullif( '" + aSales.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),''), 103),'" + aSales.BranchId + "','" + aSales.Customer + "','" + aSales.ID +
                    //                                     "','" + (Convert.ToDecimal(aSales.CReceive) - Convert.ToDecimal(aSales.CRefund)) + "','" + aSales.LoginBy + "',GETDATE())";
                    //                    command.ExecuteNonQuery();


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
                    
                        vdtl.GlCoaCode = dtFixCode.Rows[0]["BDItemPurchas"].ToString(); //**** Purchase Code *******//
                        vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["BDItemPurchas"].ToString()); //**** Purchase Code *******//
                        vdtl.Particulars = "Item Purchas";
                   
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

            //*********  Debite Voucher *********// 

            if (Convert.ToDecimal(purmst.TotalPayment) > 0)
            {
                if (_aVouchMstCR.RefFileNo.Equals("New"))
                {
                    _aVouchMstCR.ControlAmt = purmst.TotalPayment.Replace("'", "").Replace(",", "");
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
                            vdtlCR.ValueDate = purmst.GoodsReceiveDate;
                            vdtlCR.LineNo = "1";
                            vdtlCR.GlCoaCode = "1-" + SupplierCoaCode;
                            vdtlCR.Particulars = purmst.SupplierName;
                            vdtlCR.AccType = VouchManager.getAccType("1-" + SupplierCoaCode);
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
                            vdtlCR.ValueDate = purmst.GoodsReceiveDate;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(purmst.BankId) || purmst.BankId.Equals("0"))
                            {
                                vdtlCR.GlCoaCode ="1-"+ dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                vdtlCR.GlCoaCode = "1-" + purmst.BankCoaCode; //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + purmst.BankCoaCode); //**** SalesCode *******//
                                vdtlCR.Particulars = purmst.BankName;
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
                    _aVouchMstCR.ControlAmt = purmst.TotalPayment.Replace("'", "").Replace(",", "");
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
                            vdtlCR.ValueDate = purmst.GoodsReceiveDate;
                            vdtlCR.LineNo = "1";
                            vdtlCR.GlCoaCode = "1-" + SupplierCoaCode;
                            vdtlCR.Particulars = purmst.SupplierName;
                            vdtlCR.AccType = VouchManager.getAccType("1-" + SupplierCoaCode);
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
                            vdtlCR.ValueDate = purmst.GoodsReceiveDate;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(purmst.BankId))
                            {
                                vdtlCR.GlCoaCode ="1-"+
                                    dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                vdtlCR.GlCoaCode = "1-" + purmst.BankId; //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + purmst.BankId); //**** SalesCode *******//
                                vdtlCR.Particulars = purmst.BankName;
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

    //*********************************** Report Query ********************//
    public static DataTable GetShowPurchaeReport(string CurTime, string StrDate, string EndDate, string SupploerID,string typeID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        sqlCon.Open();
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = new SqlCommand("SP_ItemPurchaseForReport", sqlCon);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        if (!string.IsNullOrEmpty(StrDate))
        {
            da.SelectCommand.Parameters.AddWithValue("@StartDate", StrDate);
            da.SelectCommand.Parameters.AddWithValue("@EndDate", EndDate);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@StartDate", null);
            da.SelectCommand.Parameters.AddWithValue("@EndDate", null);
        }
        if (!string.IsNullOrEmpty(SupploerID))
        {
            da.SelectCommand.Parameters.AddWithValue("@SupplierID", SupploerID);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@SupplierID", null);
        }
        da.SelectCommand.Parameters.AddWithValue("@ReportType", typeID);
        da.SelectCommand.CommandTimeout = 500;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_ItemPurchaseForReport");
        DataTable dt = ds.Tables["SP_ItemPurchaseForReport"];
        return dt;
        
    }



    //*********************************** Report Query ********************//
    public static DataTable GetShowBranchPurchaeReport(string CurTime, string StrDate, string EndDate, string SupploerID, string typeID,string BranchId)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        sqlCon.Open();
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = new SqlCommand("SP_BranchItemPurchaseForReport", sqlCon);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        if (!string.IsNullOrEmpty(StrDate))
        {
            da.SelectCommand.Parameters.AddWithValue("@StartDate", StrDate);
            da.SelectCommand.Parameters.AddWithValue("@EndDate", EndDate);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@StartDate", null);
            da.SelectCommand.Parameters.AddWithValue("@EndDate", null);
        }
        if (!string.IsNullOrEmpty(SupploerID))
        {
            da.SelectCommand.Parameters.AddWithValue("@SupplierID", SupploerID);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@SupplierID", null);
        }

        if (!string.IsNullOrEmpty(BranchId))
        {
            da.SelectCommand.Parameters.AddWithValue("@BranchId", BranchId);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@BranchId", null);
        }
        da.SelectCommand.Parameters.AddWithValue("@ReportType", typeID);
        da.SelectCommand.CommandTimeout = 500;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_BranchItemPurchaseForReport");
        DataTable dt = ds.Tables["SP_BranchItemPurchaseForReport"];
        return dt;

    }
    public static DataTable GetShowSalesReportReport1(string ReportType, string StrDate, string EndDate, string CustomerID)
    {

        // Report Type (3) us when Client Search   Item Name Wise Group By  Total Sales Report

        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        sqlCon.Open();
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = new SqlCommand("SP_OrderInformationForReport", sqlCon);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        if (!string.IsNullOrEmpty(StrDate))
        {
            da.SelectCommand.Parameters.AddWithValue("@StartDate", StrDate);
            da.SelectCommand.Parameters.AddWithValue("@EndDate", EndDate);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@StartDate", null);
            da.SelectCommand.Parameters.AddWithValue("@EndDate", null);
        }
        if (!string.IsNullOrEmpty(CustomerID))
        {
            da.SelectCommand.Parameters.AddWithValue("@SupplierID", CustomerID);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@SupplierID", null);
        }



        da.SelectCommand.Parameters.AddWithValue("@ReportType", ReportType);
        da.SelectCommand.CommandTimeout = 500;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_OrderInformationForReport");
        DataTable dt = ds.Tables["SP_OrderInformationForReport"];
        return dt;
    }

   
    //hh
    public static DataTable GetShowSalesReportReport(string ReportType, string StrDate, string EndDate, string CustomerID,string BranchId)
    {

        // Report Type (3) us when Client Search   Item Name Wise Group By  Total Sales Report

        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        sqlCon.Open();
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = new SqlCommand("SP_OrderInformationForReport", sqlCon);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        if (!string.IsNullOrEmpty(StrDate))
        {
            da.SelectCommand.Parameters.AddWithValue("@StartDate", StrDate);
            da.SelectCommand.Parameters.AddWithValue("@EndDate", EndDate);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@StartDate", null);
            da.SelectCommand.Parameters.AddWithValue("@EndDate", null);
        }
        if (!string.IsNullOrEmpty(CustomerID))
        {
            da.SelectCommand.Parameters.AddWithValue("@SupplierID", CustomerID);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@SupplierID", null);
        }

        if (!string.IsNullOrEmpty(BranchId))
        {
            da.SelectCommand.Parameters.AddWithValue("@BranchId", BranchId);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@BranchId", null);
        }

        da.SelectCommand.Parameters.AddWithValue("@ReportType", ReportType);
        da.SelectCommand.CommandTimeout = 500;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_OrderInformationForReport");
        DataTable dt = ds.Tables["SP_OrderInformationForReport"];
        return dt;
    }
  
    //---------Branch----//

  public static DataTable GetShowBranchSalesReportReport(string ReportType, string StrDate, string EndDate, string CustomerID,string BranchId)
  {

      // Report Type (3) us when Client Search   Item Name Wise Group By  Total Sales Report

      string connectionString = DataManager.OraConnString();
      SqlConnection sqlCon = new SqlConnection(connectionString);
      sqlCon.Open();
      SqlDataAdapter da = new SqlDataAdapter();
      da.SelectCommand = new SqlCommand("SP_OrderInformationForReportBranch", sqlCon);
      da.SelectCommand.CommandType = CommandType.StoredProcedure;
      if (!string.IsNullOrEmpty(StrDate))
      {
          da.SelectCommand.Parameters.AddWithValue("@StartDate", StrDate);
          da.SelectCommand.Parameters.AddWithValue("@EndDate", EndDate);
      }
      else
      {
          da.SelectCommand.Parameters.AddWithValue("@StartDate", null);
          da.SelectCommand.Parameters.AddWithValue("@EndDate", null);
      }
      if (!string.IsNullOrEmpty(CustomerID))
      {
          da.SelectCommand.Parameters.AddWithValue("@SupplierID", CustomerID);
      }
      else
      {
          da.SelectCommand.Parameters.AddWithValue("@SupplierID", null);
      }

      if (!string.IsNullOrEmpty(BranchId))
      {
          da.SelectCommand.Parameters.AddWithValue("@BranchID", BranchId);
      }
      else
      {
          da.SelectCommand.Parameters.AddWithValue("@BranchID", null);
      }

      da.SelectCommand.Parameters.AddWithValue("@ReportType", ReportType);
      da.SelectCommand.CommandTimeout = 500;
      DataSet ds = new DataSet();
      da.Fill(ds, "SP_OrderInformationForReportBranch");
      DataTable dt = ds.Tables["SP_OrderInformationForReportBranch"];
      return dt;
  }
  //-----Branch------//

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


    public static DataTable GetShowPurchaseMst(string GrNo, string SupplierCode, string ReceiveFromDate, string ReceiveToDate)
    {
        string per="";

        if (!string.IsNullOrEmpty(GrNo))
        {
            per = "where  t1.GRN='" + GrNo + "' ";
        }

        if (!string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) && !string.IsNullOrEmpty(ReceiveFromDate) && !string.IsNullOrEmpty(ReceiveToDate))
        {

            per = "where t2.ID='" + SupplierCode + "' and  (CONVERT(date,t1.ReceivedDate,103) between CONVERT(date,'"+ReceiveFromDate+"',103) and CONVERT(date,'"+ReceiveToDate+"',103))";
        }      
        //&& (string.IsNullOrEmpty(GrNo) | string.IsNullOrEmpty(ReceiveFromDate) | string.IsNullOrEmpty(ReceiveFromDate)))
        if (!string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) && ( string.IsNullOrEmpty(ReceiveFromDate) | string.IsNullOrEmpty(ReceiveToDate)))            
        {            
            per = "where t2.ID='" + SupplierCode + "'";
        }

        if (string.IsNullOrEmpty(SupplierCode) && string.IsNullOrEmpty(GrNo) && !string.IsNullOrEmpty(ReceiveFromDate) && !string.IsNullOrEmpty(ReceiveToDate))
        {

            per = "where  (CONVERT(date,t1.ReceivedDate,103) between CONVERT(date,'" + ReceiveFromDate + "',103) and CONVERT(date,'" + ReceiveToDate + "',103))";
        }  
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT top(50) t1.ID,t1.[GRN]
      ,CONVERT(NVARCHAR,t1.[ReceivedDate],103) AS ReceivedDate   
      ,t1.[ChallanNo]
      ,CONVERT(NVARCHAR,t1.[ChallanDate],103) AS ChallanDate
       ,t2.Name+' - '+ t2.ContactName as Name
      ,t1.[Total]  
      ,t3.PartyName AS Party    
 ,t4.POCode  
  FROM [ItemPurchaseMst] t1 inner join Supplier t2 on t2.ID=t1.SupplierID
left join PartyInfo t3 on t3.ID=t1.PartyID  
left join dbo.ItemPurOrderMst t4 on convert(nvarchar,t4.id)=convert(nvarchar,t1.PO)   " + per + " order By t1.ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseMst");
        return dt;
    }

    public static DataTable GetSupplierInfo(string Supplieer)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT [ID],[Code],[Name],[ContactTitle],[ContactName],[Designation],[Email],[Phone],[Fax],[Website],[Mobile],[Address1],[Address2],[City],[State],[PostalCode],[Country],[SupplierGroupID],[Gl_CoaCode],[SupplierSearch_Mobile],[SupplierSearch]
  FROM [dbo].[View_Search_Supplier] where Upper(SupplierSearch_Mobile)=UPPER('" + Supplieer + "')";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
        return dt;
    }

    public static DataTable GetShowPVMasterInfo(string GRN)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID] ,t1.[GRN] ,t1.[SupplierID],t2.ContactName ,t2.Gl_CoaCode,PvType
      FROM [ItemPurchaseMst] t1 inner join Supplier t2 on t2.ID=t1.SupplierID WHERE t1.GRN='" + GRN + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    public static decimal GetShowreturnAmount(string StartDate,string EndDate,string CustomerID)
    { 
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            
        try
        {
            connection.Open();
           string parameter = "";
            if (CustomerID != "" && !string.IsNullOrEmpty(CustomerID))
            {
                parameter = " and t2.CustomerID='" + CustomerID + "'";
            }
            string query = @"Select isnull(sum(isnull(TotalAmount,0)),0) as TotalAmount from dbo.OrderReturn t1 inner join [order] t2 on t2.ID=t1.InvoiceNo where convert(date,ReturnDate,103) between Convert(date,'" + StartDate + "',103) and Convert(date,'" + EndDate + "',103)  ";

            SqlCommand command = new SqlCommand(query, connection);
            var dd = command.ExecuteScalar();
            if (dd == null)
            {
                dd=0;
            }
            return Convert.ToDecimal(dd);
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

    //------Branch***

    public static decimal GetBranchShowreturnAmount(string StartDate, string EndDate, string CustomerID,string BranchId)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());

        try
        {
            connection.Open();
            string parameter = "";
            if (CustomerID != "" && !string.IsNullOrEmpty(CustomerID))
            {
                parameter = " and t2.CustomerID='" + CustomerID + "'";
            }
            string query = @"Select isnull(sum(isnull(TotalAmount,0)),0) as TotalAmount from dbo.OrderReturn t1 inner join [order] t2 on t2.ID=t1.InvoiceNo where t1.BranchId='"+BranchId+"' and convert(date,ReturnDate,103) between Convert(date,'" + StartDate + "',103) and Convert(date,'" + EndDate + "',103)  ";

            SqlCommand command = new SqlCommand(query, connection);
            var dd = command.ExecuteScalar();
            if (dd == null)
            {
                dd = 0;
            }
            return Convert.ToDecimal(dd);
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
    //-----Branch*****

    public static DataTable GetSalesSummeryInfo(string StarDate,string EndDate)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"Select OrderDate,sum(grandTotal) as grandTotal,sum(returnamn) as returnamn ,sum(Quantity) as Quantity ,Sum([Return]) as [Return]  from (Select Convert(nvarchar,OrderDate,103)as OrderDate,sum(grandTotal)  as grandTotal,0 as returnamn,sum(Quantity) as Quantity,0 as [Return] from [Order] t1 inner join dbo.OrderDetail t2 on t2.OrderID=t1.ID
group by Convert(nvarchar,OrderDate,103)
union all
Select Convert(nvarchar,ReturnDate,103),0,sum(TotalAmount),0,sum(t1.Quantity) as Quantity from OrderReturnDetail t1 inner join OrderReturn t2 on t2.ID=t1.OrderReturnMstID inner join [Order] t3 on t3.ID=t2.InvoiceNo
group by Convert(nvarchar,ReturnDate,103)) a1  where (convert(date,OrderDate,103) between convert(date,'" + StarDate + "',103) and convert(date,'" + EndDate + "',103)) group by a1.OrderDate";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }


    public static DataTable GetBranchIdSalesSummeryInfo(string StarDate, string EndDate,string BranchId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"Select OrderDate,sum(grandTotal) as grandTotal,sum(returnamn) as returnamn , sum(Quantity) as Quantity ,Sum([Return]) as [Return] from (Select Convert(nvarchar,OrderDate,103)as OrderDate,sum(grandTotal)  as grandTotal,0 as returnamn,sum(Quantity) as Quantity,0 as [Return] from [Order] t1 inner join dbo.OrderDetail t2 on t2.OrderID=t1.ID and t1.BranchId=t2.BranchId where t1.BranchId='"+BranchId+"' group by Convert(nvarchar,OrderDate,103) union all Select Convert(nvarchar,ReturnDate,103),0,sum(TotalAmount),0,sum(t1.Quantity) as Quantity from OrderReturnDetail t1 inner join OrderReturn t2 on t2.ID=t1.OrderReturnMstID and t1.BranchId=t2.BranchId inner join [Order] t3 on t3.ID=t2.InvoiceNo and t3.BranchId=t1.BranchId where t1.BranchId='"+BranchId+"' group by Convert(nvarchar,ReturnDate,103)) a1 where (convert(date,OrderDate,103) between convert(date,'" + StarDate + "',103) and convert(date,'" + EndDate + "',103)) group by a1.OrderDate";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemPurchaseDtl");
        return dt;
    }

    public DataTable GetShowPurchaseHistoryDtl(string SupplierID,string GRN,
               string StartDate, string EndDate, string typeDtl)
    {
        DataSet ds = new DataSet();
        string connectionString = DataManager.OraConnString();
        using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
        {
            SqlCommand sqlComm = new SqlCommand("SP_ItemsPurchaseHistory", conn);
            if (string.IsNullOrEmpty(SupplierID))
            {
                sqlComm.Parameters.AddWithValue("@SupplierID", null);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@SupplierID", SupplierID);
            }
            if (string.IsNullOrEmpty(GRN))
            {
                sqlComm.Parameters.AddWithValue("@GRN", null);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@GRN", GRN);
            }
            if (string.IsNullOrEmpty(StartDate))
            {
                sqlComm.Parameters.AddWithValue("@StartDate", null);
                sqlComm.Parameters.AddWithValue("@EndDate", null);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@StartDate", StartDate);
                sqlComm.Parameters.AddWithValue("@EndDate", EndDate);
            }
            sqlComm.Parameters.AddWithValue("@typeDtl", typeDtl);
            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "SP_ItemsPurchaseHistory");
            ds.Tables[0].TableName = "SP_ItemsPurchaseHistory";
            return ds.Tables[0];
        }
    }

    public static DataTable GetShowDamageReport(string ItemsID, string ShipmentID, string Code, string StartDate, string EndDate, string CustomerID)
    {
        DataSet ds = new DataSet();
        string connectionString = DataManager.OraConnString();
        using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
        {
            SqlCommand sqlComm = new SqlCommand("SP_StockOutReport", conn);
            if (string.IsNullOrEmpty(ItemsID))
            {
                sqlComm.Parameters.AddWithValue("@ItemsID", null);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@ItemsID", ItemsID);
            }

            if (string.IsNullOrEmpty(ShipmentID))
            {
                sqlComm.Parameters.AddWithValue("@ShipmentID", null);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@ShipmentID", ShipmentID);
            }

            if (string.IsNullOrEmpty(Code))
            {
                sqlComm.Parameters.AddWithValue("@Code", null);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@Code", Code);
            }

            if (string.IsNullOrEmpty(StartDate))
            {
                sqlComm.Parameters.AddWithValue("@StartDate", null);
                sqlComm.Parameters.AddWithValue("@EndDate", null);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@StartDate", StartDate);
                sqlComm.Parameters.AddWithValue("@EndDate", EndDate);
            }
            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "SP_StockOutReport");
            ds.Tables[0].TableName = "SP_StockOutReport";
            return ds.Tables[0];
        }
    }

    public DataTable GetShowPurchaseDetails(string MstID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query =
            @"SELECT *  FROM [dbo].[View_Search_Purchase_Details_with_barcode] where ItemPurchaseMstID='" + MstID + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
        return dt;
    }

    public double GetShowCheckQty(string ID, string item_sales_rate, string item_rate, string Expdate)
    {
        double Qty = 0;
        if (string.IsNullOrEmpty(Expdate))
        {
            Qty = IdManager.GetShowSingleValueCurrency(
                @"SELECT isnull(t1.[ClosingStock],0) AS[ClosingQty]    
                      FROM [dbo].[ItemStock] t1 
                      left join ( select t1.ItemID,sum(t1.Quantity) AS Quantity from [OrderDetail] t1 
                      group by t1.ItemID) t2 on t2.ItemID=t1.ID
                      WHERE t1.ItemID ='" + ID + "' and t1.ItemsPrice='" + item_sales_rate +
                "'  and t1.CostPrice='" + item_rate +
                "' and t1.[ExpireDate] IS NULL");
        }
        else
        {

            Qty = IdManager.GetShowSingleValueCurrency(
                @"SELECT isnull(t1.[ClosingStock] -isnull(t2.Quantity,0),0) AS[ClosingQty]    
                      FROM [dbo].[ItemStock] t1 
                      left join ( select t1.ItemID,sum(t1.Quantity) AS Quantity from [OrderDetail] t1 
                      group by t1.ItemID) t2 on t2.ItemID=t1.ID
                      WHERE t1.ItemID ='" + ID + "' and t1.ItemsPrice='" + item_sales_rate +
                "'  and t1.CostPrice='" + item_rate +
                "' and (convert(date,t1.[ExpireDate])=convert(date,'" +
                Expdate + "',103))");
        }

        return Qty;
    }

    public double GetShowCheckSalesQty(string ID, string item_sales_rate, string item_rate, string Expdate)
    {
        double Qty = 0;
        if (string.IsNullOrEmpty(Expdate))
        {
            Qty = IdManager.GetShowSingleValueCurrency(
                @"SELECT isnull(t1.[ClosingStock],0) AS[ClosingQty]    
                      FROM [dbo].[ItemStock] t1 
                      left join ( select t1.ItemID,sum(t1.Quantity) AS Quantity from [OrderDetail] t1 
                      group by t1.ItemID) t2 on t2.ItemID=t1.ID
                      WHERE t1.ItemID ='" + ID + "' and t1.ItemsPrice='" + item_sales_rate +
                "'  and t1.CostPrice='" + item_rate +
                "' and t1.[ExpireDate] IS NULL");
        }
        else
        {

            Qty = IdManager.GetShowSingleValueCurrency(
                @"SELECT isnull(t1.[ClosingStock] -isnull(t2.Quantity,0),0) AS[ClosingQty]    
                      FROM [dbo].[ItemStock] t1 
                      left join ( select t1.ItemID,sum(t1.Quantity) AS Quantity from [OrderDetail] t1 
                      group by t1.ItemID) t2 on t2.ItemID=t1.ID
                      WHERE t1.ItemID ='" + ID + "' and t1.ItemsPrice='" + item_sales_rate +
                "'  and t1.CostPrice='" + item_rate +
                "' and (convert(date,t1.[ExpireDate])=convert(date,'" +
                Expdate + "',103))");
        }

        return Qty;

       
    }



    public bool IsExistBarcode(string Id, string Barcode)
    {
        string query = "select * from ItemStock where ItemID!='" + Id + "' and Barcode='"+Barcode+"' ";
        var data = DataManager.ExecuteQuery(DataManager.OraConnString(), query, "ItemStock");
        if (data.Rows.Count>0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}