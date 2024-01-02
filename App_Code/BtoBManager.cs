using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Delve;

/// <summary>
/// Summary description for BtoBManager
/// </summary>
public class BtoBManager
{
	public BtoBManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int SaveInformation(clsItemTransferStock aclsItemTransferStock, System.Data.DataTable dt, VouchMst vmst, VouchMst vmstPayment, string branchTo)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        connection.Open();
        transaction = connection.BeginTransaction();
        //try
        //{

        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");


        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        command.CommandText = @"INSERT INTO [dbo].[BtoBTransferMst]
           (from_Branch,to_Branch,AddDate,Remarks,AdBay,UserDate,TransferCode)
     VALUES
           ('" + aclsItemTransferStock.BranchId + "'," + branchTo + ",CONVERT(datetime,'" + aclsItemTransferStock.TransferDate + "'+' '+CONVERT(VARCHAR(8),GETDATE(),108),103),'" +
                              aclsItemTransferStock.Remark.Replace("'", "") + "','" + aclsItemTransferStock.LoginBy +
                              "',GETDATE(),'" + aclsItemTransferStock.Code + "')";
        command.ExecuteNonQuery();
        command.CommandText = @"select top(1)ID from [BtoBTransferMst] ORDER BY ID DESC";
        int MstID = Convert.ToInt32(command.ExecuteScalar());
        decimal TotalAmount = 0, TotalSaleAmount = 0;
        foreach (DataRow dr in dt.Rows)
        {
            if (!string.IsNullOrEmpty(dr["item_code"].ToString()))
            {
                if (!string.IsNullOrEmpty(dr["TransferQty"].ToString()))
                {
                    if (Convert.ToDouble(dr["TransferQty"]) > 0)
                    {
                        var ItemId = dr["ItemsID"].ToString();
                        var TransferQuantity = dr["TransferQty"].ToString().Replace(",", "");
                        var TransferPrice = dr["Price"].ToString().Replace(",", "");
                        var BranchSalesPrice = dr["BranchSalesPrice"].ToString().Replace(",", "");
                        var BranchID = aclsItemTransferStock.BranchId;
                        var Code = dr["item_code"].ToString().Replace(",", "");
                        var StyleNo = dr["StyleNo"].ToString().Replace(",", "");
                        var Discount = dr["Discount"].ToString();
                        var Type = aclsItemTransferStock.TransferType;
                        var Barcode = dr["Barcode"].ToString();
                        //var ClosingAmount = dr["TransferQty"].ToString() * dr["Price"].ToString().Replace(",", "");

                        command.CommandText = @"INSERT INTO BtoBtransferDtl(MstId,[ItemId],transferQty,TransferPrice,SalePrice,from_BranchId,to_BranchId,ItemCode,Barcode) VALUES
('" + MstID + "','" + ItemId + "','" + TransferQuantity + "','" + TransferPrice + "','" + BranchSalesPrice + "','" + BranchID + "','"+branchTo+"','" + Code + "','" + Barcode + "')";
                        command.CommandTimeout = 500;
                        command.ExecuteNonQuery();

                        command.CommandText = @"select top(1)ID from ItemStock where ItemID='" + ItemId + "' and BranceId='" + BranchID + "' and barcode='" + Barcode + "'";
                        int ItemStockId = Convert.ToInt32(command.ExecuteScalar());

                        if (ItemStockId == 0)
                        {
                            command.CommandText = @"Update ItemStock set  ClosingStock=ClosingStock - " + TransferQuantity + ",ClosingAmount=" + TransferPrice + "*(" + TransferQuantity + "+" + TransferQuantity + ") where ID='" + ItemStockId + "' and  BranceId='" + aclsItemTransferStock.BranchId + "'";
                            command.ExecuteNonQuery();

                            command.CommandText = @"INSERT INTO [ItemStock]([ItemId],[ClosingStock],ClosingAmount,CostPrice,ItemsPrice,BranceId,ItemCode,Barcode,[AddBy],[AddDate]) VALUES
('" + ItemId + "','" + TransferQuantity + "'," + TransferPrice + "*" + TransferQuantity + ",'" + TransferPrice + "','" + BranchSalesPrice + "','" + branchTo + "','" + Code + "','" + Barcode + "','" + aclsItemTransferStock.LoginBy + "',GETDATE())";
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            command.CommandText = @"Update ItemStock set  ClosingStock=ClosingStock - " + TransferQuantity + ",ClosingAmount=" + TransferPrice + "*(" + TransferQuantity + "+" + TransferQuantity + ") where ID='" + ItemStockId + "' and  BranceId='"+aclsItemTransferStock.BranchId+"'";
                            command.ExecuteNonQuery();

                            command.CommandText = @"select top(1)ID from ItemStock where ItemID='" + ItemId + "' and BranceId='" + branchTo + "' and barcode='" + Barcode + "'";
                            int ItemkId = Convert.ToInt32(command.ExecuteScalar());
                            if (ItemkId == 0)
                            {
                                command.CommandText = @"INSERT INTO [ItemStock]([ItemId],[ClosingStock],ClosingAmount,CostPrice,ItemsPrice,BranceId,ItemCode,Barcode,[AddBy],[AddDate]) VALUES
('" + ItemId + "','" + TransferQuantity + "'," + TransferPrice + "*" + TransferQuantity + ",'" + TransferPrice + "','" + BranchSalesPrice + "','" + branchTo + "','" + Code + "','" + Barcode + "','" + aclsItemTransferStock.LoginBy + "',GETDATE())";
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                command.CommandText = @"Update ItemStock set  ClosingStock=ClosingStock + " + TransferQuantity + ",ClosingAmount=" + TransferPrice + "*(" + TransferQuantity + "+" + TransferQuantity + ") where ID='" + ItemkId + "' and  BranceId='" + branchTo + "'";
                                command.ExecuteNonQuery();
                            }

                           
                        }


                        TotalAmount += Convert.ToDecimal(dr["Price"]) * Convert.ToDecimal(dr["TransferQty"]);
                        try
                        {

                            if (!string.IsNullOrEmpty(dr["Discount"].ToString()) && dr["Discount"].ToString() != "0")
                            {
                                TotalSaleAmount += (Convert.ToDecimal(dr["Price"]) * (Convert.ToDecimal(dr["Discount"]) / 100)) * Convert.ToDecimal(dr["TransferQty"]);


                            }
                            else
                            {

                                TotalSaleAmount += Convert.ToDecimal(dr["BranchSalesPrice"]) * Convert.ToDecimal(dr["TransferQty"]);
                            }
                        }
                        catch
                        {

                            TotalSaleAmount += Convert.ToDecimal(dr["BranchSalesPrice"]) * Convert.ToDecimal(dr["TransferQty"]);
                        }
                        
                    }
                }
            }
        }

        command.CommandText = @"select PurchaseCoaCode from [BranchInfo] where ID='" + aclsItemTransferStock.BranchId + "' ";
        string CoaCode = command.ExecuteScalar().ToString();
        command.CommandText = @"select BranchCoaCode from [BranchInfo] where ID='" + aclsItemTransferStock.BranchId + "' ";
        string ClosingCoaCode = command.ExecuteScalar().ToString();

        vmst.ControlAmt = TotalAmount.ToString();
        vmst.EntryDate = DateTime.Now.ToString("dd/MM/yyyy");
        command.CommandText = VouchManager.SaveVoucherMst(vmst, 1);
        command.ExecuteNonQuery();

        transaction.Commit();
        connection.Close();

       
        return 1;
      

    }

    public DataTable GetStockTransferInfo(string TransferId)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query =
            @"select Id,TransferCode as Code,from_Branch, to_Branch,convert(nvarchar,AddDate,103)TransferDate,Remarks as Remark from BtoBTransferMst  where ID='" +
            TransferId + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransfer");     
        return  dt;
    }

    public DataTable GetBranchInfo(string p)
    {
        String connectionString = DataManager.OraConnString();
        string query = @" select t1.Id,TransferCode  as Code, t2.BranchName as ToBranch ,convert(nvarchar,AddDate,103)TransferDate,Remarks as Remark,tot.Qty from BtoBTransferMst t1 inner join (select MstID,SUM(ISNULL(transferQty,0))AS Qty from BtoBtransferDtl  GROUP BY MstID) tot on tot.MstId=t1.Id inner join BranchInfo t2 on t1.to_Branch=t2.id
  order by t1.Id desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "BtoBTransferMst");
        return dt;
    }

    public DataTable GetShowItemsDetails(string Parameter)
    {
        String connectionString = DataManager.OraConnString();
        string query =
            @"select t1.MstId,t3.id as ItemsID,t1.ItemCode as [item_code],'' as ReceivedQuantity,
'' as StyleNo,t1.Barcode as Barcode,t3.Name as [ItemsName],
t1.transferQty as [StockQty],t1.transferQty as [TransferQty],
convert(decimal (18,2),isnull(t1.TransferPrice,0)) as Price,convert(decimal (18,2),isnull(t1.SalePrice,0)) as BranchSalesPrice from BtoBtransferDtl t1 
inner join BtoBTransferMst t2 on t1.mstid=t2.Id
inner join Item t3 on t1.ItemId=t3.Id " + Parameter + " Order By t3.Name ASC";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransfer");
        return dt;
    }

    public DataTable GetBranchInfobranch(string BranchId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"select t1.Id,TransferCode  as Code, t2.BranchName as ToBranch ,convert(nvarchar,AddDate,103)TransferDate,Remarks as Remark,tot.Qty from BtoBTransferMst t1 inner join (select MstID,SUM(ISNULL(transferQty,0))AS Qty from BtoBtransferDtl  GROUP BY MstID) tot on tot.MstId=t1.Id inner join BranchInfo t2 on t1.to_Branch=t2.id where t1.to_Branch='" + BranchId + "' order by t1.Id desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "BtoBTransferMst");
        return dt;
    }
}