using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Delve;
using System.Data.SqlClient;

/// <summary>
/// Summary description for DayClosingSaleSummaryManager
/// </summary>
public class DayClosingSaleSummaryManager
{
	public DayClosingSaleSummaryManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public DataTable GetDayClosingItemsSummery(string Parameter)
    {
        if (string.IsNullOrEmpty(Parameter))
        {
            Parameter = " Where ReceivedBy is Null ";
        }
        else
        {
            Parameter = "  where convert(date,t1.[ClosingDate],103)=convert(date,'" + Parameter + "',103)";
        }
        string connectionString = DataManager.OraConnString();
        string Query = @"SELECT t1.[ID],convert(nvarchar,t1.[ClosingDate],103) AS ClosingDate,t1.[CashAmounTotal],t1.[BankAmountTotal],t1.[ServerUpload],t1.[CostingPriceTotal],t1.BranchID,t2.BranchName,case when ReceivedBy is Null then 'Not Received' else 'Received' end  AS ReceivedBy,Remark
  FROM [DalyItemSaleSummeryMst]  t1
  inner join dbo.BranchInfo t2 on t2.ID=t1.BranchID " + Parameter + " ORDER BY t1.[ClosingDate] ASC ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemTransferDtl");
        return dt;
    }

    public DataTable GetItemDayClosingDtl(string MstID)
    {
        string connectionString = DataManager.OraConnString();
        string Query = @"select * from (Select t1.ID as DtlID,t2.Code as Code, t1.ItemID,t2.Name as ItemName,convert(decimal(18,0),t1.Quentity) as SaleQty,t1.SaleAmount,t3.BranchID,t1.LocalItemID,'Sales' AS[Type],t1.MstID,t1.RecordType from dbo.DalyItemSaleSummeryDtl t1 
inner join dbo.DalyItemSaleSummeryMst t3 on t3.ID=t1.MstID
inner join ItemSalesStock t4 on t4.ID=t1.LocalItemID
inner join Item t2 on t2.ID=t4.ItemsID where RecordType='S' 
union all
Select t1.ID as DtlID,t2.Code as Code, t1.ItemID,t2.Name as ItemName,convert(decimal(18,0),t1.Quentity) as SaleQty,t1.SaleAmount,t3.BranchID,t1.LocalItemID,CASE WHEN RecordType='E' then 'Exchange' WHEN RecordType='R' then 'Return' else 'S' end  AS[Type] ,t1.MstID,t1.RecordType   from dbo.DalyItemSaleSummeryDtl t1 
inner join dbo.DalyItemSaleSummeryMst t3 on t3.ID=t1.MstID
inner join ItemSalesStock t4 on t4.ID=t1.LocalItemID
inner join Item t2 on t2.ID=t4.ItemsID where RecordType<>'S') tot where tot.MstID='" + MstID + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemTransferDtl");
        return dt;
    }

    public void SaveDayClosingItemsSummery(DataTable dtStockRecived, string LogineBy, string MstID,
                   string CurrencyRate, string UserType, VouchMst vmst, string ClosingDate,
                    string BranchId,string CostingPrice,string UploadStatus)
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
                command.CommandText = @"UPDATE [DalyItemSaleSummeryMst] SET 
            [UpdateBy] = '" + LogineBy + "' ,[UpdateDate]=GETDATE(),Remark='" + vmst.Particulars + "' WHERE ID='" +
                                      MstID + "' ";
                command.ExecuteNonQuery();

                command.CommandType = CommandType.Text;
                command.CommandText = VouchManager.SaveVoucherMst(vmst, 2);
                command.ExecuteNonQuery();

            }
            else
            {

                command.CommandText = @"UPDATE [DalyItemSaleSummeryMst] SET 
            [ReceivedBy] = '" + LogineBy + "' ,[ReceivedDate]=GETDATE(),Remark='" + vmst.Particulars + "' WHERE ID='" +
                                      MstID + "' ";
                command.ExecuteNonQuery();
                if (dtStockRecived != null)
                {
                    foreach (DataRow dr in dtStockRecived.Rows)
                    {
                        if (!string.IsNullOrEmpty(dr["DtlID"].ToString()))
                        {

                            if (dr["RecordType"].ToString().Equals("S"))
                            {
                                command.CommandText = @"UPDATE [ItemSalesStockBranchWise]
                            SET [Quantity] =ISNULL([Quantity],0)-" + Convert.ToDecimal(dr["SaleQty"].ToString()) +
                                                      "  WHERE [ItemsID]='" + dr["LocalItemID"].ToString() + "' ";
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                command.CommandText = @"UPDATE [ItemSalesStockBranchWise]
                            SET [Quantity] =ISNULL([Quantity],0)+" + Convert.ToDecimal(dr["SaleQty"].ToString()) +
                                                      "  WHERE [ItemsID]='" + dr["LocalItemID"].ToString() + "' ";
                                command.ExecuteNonQuery();
                            }

                        }
                    }
                }
                //***************************  Jurnal Voucher ********************************// 
                //******* Vucher One ******//
                command.CommandType = CommandType.Text;
                command.CommandText = VouchManager.SaveVoucherMst(vmst, 1);
                command.ExecuteNonQuery();
                VouchDtl vdtl;
                for (int j = 0; j < 3; j++)
                {
                    if (j == 0)
                    {
                        vdtl = new VouchDtl();
                        vdtl.VchSysNo = vmst.VchSysNo;
                        vdtl.ValueDate = ClosingDate;
                        vdtl.LineNo = "1";
                        vdtl.GlCoaCode = dtBranchFixCode.Rows[0]["BR_CashInHand"].ToString();
                        vdtl.Particulars = dtBranchFixCode.Rows[0]["BR_CashInHand_Des"].ToString();
                        vdtl.AccType = VouchManager.getAccType(dtBranchFixCode.Rows[0]["BR_CashInHand"].ToString());
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
                        vdtl.ValueDate = ClosingDate;
                        vdtl.LineNo = "2";
                        vdtl.GlCoaCode = dtBranchFixCode.Rows[0]["BR_Sales_Code"].ToString();
                        //**** Gl_Main_OfficeCode *******//
                        vdtl.Particulars = dtBranchFixCode.Rows[0]["BR_Sales_Code_Des"].ToString();
                        vdtl.AccType = VouchManager.getAccType(dtBranchFixCode.Rows[0]["BR_Sales_Code"].ToString());
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
                        vdtl.ValueDate = ClosingDate;
                        vdtl.LineNo = "3";
                        vdtl.GlCoaCode = dtBranchFixCode.Rows[0]["BR_Closing_Stock"].ToString();
                        vdtl.Particulars = dtBranchFixCode.Rows[0]["BR_Closing_Stock_Des"].ToString();
                        vdtl.AccType = VouchManager.getAccType(dtBranchFixCode.Rows[0]["BR_Closing_Stock"].ToString());
                        vdtl.AmountDr = "0";
                        vdtl.AmountCr = CostingPrice.Replace("'", "").Replace(",", "");
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

    public void DeleteDayClosingItemsSummery(DataTable dtStockRecived, string LogineBy, string MstID, string Currency, string UserType, VouchMst vmst, string Date, string BranchID, string CostingPrice, string UpdateStatus)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" + vmst.VchSysNo +
                                 "')";
            command.ExecuteNonQuery();

            command.CommandText = @"delete from gl_trans_mst where vch_sys_no=" + vmst.VchSysNo + " ";
            command.ExecuteNonQuery();

            foreach (DataRow dr in dtStockRecived.Rows)
            {
                if (!string.IsNullOrEmpty(dr["DtlID"].ToString()))
                {
                    if (dr["RecordType"].ToString().Equals("S"))
                    {
                        command.CommandText = @"UPDATE [ItemSalesStockBranchWise]
                            SET [Quantity] =ISNULL([Quantity],0)+" + Convert.ToDecimal(dr["SaleQty"].ToString()) +
                                              "  WHERE [ItemsID]='" + dr["LocalItemID"].ToString() + "' ";
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        command.CommandText = @"UPDATE [ItemSalesStockBranchWise]
                            SET [Quantity] =ISNULL([Quantity],0)-" + Convert.ToDecimal(dr["SaleQty"].ToString()) +
                                              "  WHERE [ItemsID]='" + dr["LocalItemID"].ToString() + "' ";
                        command.ExecuteNonQuery();
                    }

                }
            }

            command.CommandText = @"delete from DalyItemSaleSummeryDtl where MstID='" + MstID + "' ";
            command.ExecuteNonQuery();

            command.CommandText = @"delete from DalyItemSaleSummeryMst WHERE ID='" + MstID + "' ";
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