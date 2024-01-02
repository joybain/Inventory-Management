using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
//using autouniv;
using System.Data.SqlClient;
using Delve;
using DocumentFormat.OpenXml.Drawing.Diagrams;

/// <summary>
/// Summary description for ItemStockReceiveManager
/// </summary>
public class ItemStockReceiveManager
{
	public ItemStockReceiveManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable GetItemStockMst(string transferDate)
    {
        string connectionString = DataManager.OraConnString();
        string Parameter = "";
        if (!string.IsNullOrEmpty(transferDate))
        {
            Parameter = "  Convert(date,t1.TransferDate,103)= Convert(date,'" + transferDate + "',103)  ";
        }
        else
        {
            Parameter = "t1.ReceivedBy is null";
        }
        string Query =
            "Select top(100) t1.ID,t1.BranchID,t2.BranchName,t1.LocalServerID,Convert(nvarchar,t1.TransferDate,103) as TransferDate,t1.Remark as Remark,CostingPriceTotal,CostingPriceTotalHeadOffice, CASE WHEN t1.ReceivedBy is null then 'Not Received' ELSE 'Received' end AS ReceivedBy  from dbo.ItemTransferMst t1 inner join BranchInfo t2 on t2.ID=t1.BranchID where " + Parameter + " ORDER BY t1.ID DESC ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemTransferMst");
        return dt;
    }

    public static DataTable GetItemStockDtl(string MstID)
    {
        string connectionString = DataManager.OraConnString();
        string Query = @"Select t1.ID as DtlID,t2.Code as Code, t1.ItemID,t2.Name as ItemName,convert(decimal(18,0),t1.Quantity) as TransferQnty,t1.UnitPrice as TransferPrice,convert(decimal(18,0),t1.Quantity) AS ReceivedQuantity,t3.BranchID,t1.LocalItemID,t1.PvUnitPrice from ItemTransferDtl t1 
inner join dbo.ItemTransferMst t3 on t3.ID=t1.MstID
inner join ItemSalesStock t4 on t4.ID=t1.LocalItemID
inner join Item t2 on t2.ID=t4.ItemsID where t1.MstID=" + MstID;
        DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemTransferDtl");
        return dt;
    }

    public static void SaveTransferItemStock(DataTable dt, string LogineBy, string MstID, string CurrencyRate,
        string UserType, VouchMst vmst, string Date, string BranchId, string CostingPriceHeadOffice, string CostingPrice, string UploadStatus)
    {
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        DataTable dtBranchFixCode = VouchManager.GetAllFixGlCode(BranchId);
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;
            //SqlCommand command1 = new SqlCommand();
            //command1.Connection = connection;
            //command1.Transaction = transaction;
            //LocalItemID
            if (UploadStatus.Equals("Received"))
            {
                command.CommandText = @"UPDATE [ItemTransferMst] SET 
            [UpdateBy] = '" + LogineBy + "' ,[UpdateDate]=GETDATE(),Remark='" + vmst.Particulars + "' WHERE ID='" +
                                      MstID + "' ";
                command.ExecuteNonQuery();

                command.CommandType = CommandType.Text;
                command.CommandText = VouchManager.SaveVoucherMst(vmst, 2);
                command.ExecuteNonQuery();

            }
            else
            {
                command.CommandText = @"UPDATE [ItemTransferMst] SET 
            [ReceivedBy] = '" + LogineBy + "' ,[ReceivedDate]=GETDATE(),Remark='" + vmst.Particulars + "' WHERE ID='" +
                                      MstID + "' ";
                command.ExecuteNonQuery();
                foreach (DataRow dr in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(dr["DtlID"].ToString()))
                    {
                        decimal stock = decimal.Zero;
                        command.CommandText = @"UPDATE [ItemTransferDtl] SET [ReceivedQuantity] = '" +
                                              Convert.ToDecimal(dr["ReceivedQuantity"].ToString()) + "' WHERE [ID] ='" +
                                              dr["DtlID"].ToString() + "' ";
                        command.ExecuteNonQuery();

                        command.CommandText = @" update [ItemSalesStock] set [Quantity]=ISNULL([Quantity],0)+'" +
                                              Convert.ToDecimal(dr["TransferQnty"].ToString()) +
                                              "' ,[SalesClosingQty]=isnull([SalesClosingQty],0)-'" +
                                              dr["TransferQnty"].ToString() + "' ,[Price]=isnull([Price],0)+'" +
                                              Convert.ToDecimal(dr["TransferQnty"]) *
                                              Convert.ToDecimal(dr["PvUnitPrice"]) +
                                              "' where [ID]='" + dr["LocalItemID"].ToString() +
                                              "' ";
                        command.ExecuteNonQuery();

                        command.CommandText = @"UPDATE [ItemSalesStockBranchWise]
                        SET [Quantity] =ISNULL([Quantity],0)-" + Convert.ToDecimal(dr["TransferQnty"].ToString()) +
                                              "  WHERE [ItemsID]='" + dr["LocalItemID"].ToString() + "' ";
                        command.ExecuteNonQuery();

                    }
                }
                //***************************  Jurnal Voucher ********************************// 
                //******* Vucher One ******//
                command.CommandType = CommandType.Text;
                command.CommandText = VouchManager.SaveVoucherMst(vmst, 1);
                command.ExecuteNonQuery();
                VouchDtl vdtl;
                for (int j = 0; j < 4; j++)
                {
                    if (j == 0)
                    {
                        vdtl = new VouchDtl();
                        vdtl.VchSysNo = vmst.VchSysNo;
                        vdtl.ValueDate = Date;
                        vdtl.LineNo = "1";
                        vdtl.GlCoaCode = dtFixCode.Rows[0]["MainOfficeTransferReturn"].ToString();
                        vdtl.Particulars = dtFixCode.Rows[0]["MainOfficeTransferReturnDesc"].ToString();
                        vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["MainOfficeTransferReturn"].ToString());
                        vdtl.AmountDr = Convert.ToDouble(vmst.ControlAmt).ToString().Replace(",", "");
                        vdtl.AmountCr = "0";
                        vdtl.Status = vmst.Status;
                        vdtl.BookName = vmst.BookName;
                        BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                        VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                    }
                    else if (j == 1)
                    {
                        vdtl = new VouchDtl();
                        vdtl.VchSysNo = vmst.VchSysNo;
                        vdtl.ValueDate = Date;
                        vdtl.LineNo = "2";
                        vdtl.GlCoaCode = "1-" + dtBranchFixCode.Rows[0]["Gl_CoaCode"].ToString();
                        //**** Gl_Main_OfficeCode *******//
                        vdtl.Particulars = dtBranchFixCode.Rows[0]["Gl_CoaDesc"].ToString();
                        vdtl.AccType = VouchManager.getAccType("1-" + dtBranchFixCode.Rows[0]["Gl_CoaCode"].ToString());
                        //**** Gl_Main_OfficeCode *******//
                        vdtl.AmountDr = "0";
                        vdtl.AmountCr = Convert.ToDouble(vmst.ControlAmt).ToString().Replace(",", "");
                        vdtl.Status = vmst.Status;
                        vdtl.BookName = vmst.BookName;
                        BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                        VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                    }
                    else if (j == 2)
                    {
                        vdtl = new VouchDtl();
                        vdtl.VchSysNo = vmst.VchSysNo;
                        vdtl.ValueDate = Date;
                        vdtl.LineNo = "3";
                        vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString();
                        vdtl.Particulars = "Closing Stock";
                        vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
                        vdtl.AmountDr = CostingPriceHeadOffice.Replace(",", "");
                        vdtl.AmountCr = "0";
                        vdtl.AUTHO_USER = "CS";
                        vdtl.Status = vmst.Status;
                        vdtl.BookName = vmst.BookName;
                        BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                        VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                    }
                    else if (j == 3)
                    {
                        vdtl = new VouchDtl();
                        vdtl.VchSysNo = vmst.VchSysNo;
                        vdtl.ValueDate = Date;
                        vdtl.LineNo = "4";
                        vdtl.GlCoaCode = dtBranchFixCode.Rows[0]["BR_Closing_Stock"].ToString();
                        vdtl.Particulars = dtBranchFixCode.Rows[0]["BR_Closing_Stock_Des"].ToString();
                        vdtl.AccType = VouchManager.getAccType(dtBranchFixCode.Rows[0]["BR_Closing_Stock"].ToString());
                        vdtl.AmountDr = "0";
                        vdtl.AmountCr = vmst.ControlAmt.Replace(",", "");
                        vdtl.AUTHO_USER = "CS";
                        vdtl.Status = vmst.Status;
                        vdtl.BookName = vmst.BookName;
                        BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                        VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
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
}