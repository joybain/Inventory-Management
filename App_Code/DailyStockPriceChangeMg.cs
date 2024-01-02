using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Delve;
using DocumentFormat.OpenXml.Vml.Spreadsheet;

/// <summary>
/// Summary description for DailyStockPriceChangeMg
/// </summary>
public class DailyStockPriceChangeMg
{
	public DailyStockPriceChangeMg()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int Save(List<DailyStockPriceChange> PriceList)
    {
        int success = 0;

        foreach (var Price in PriceList)
        {


            var connectionstring = DataManager.OraConnString();
            var connection = new SqlConnection(connectionstring);
            connection.Open();
            string qurey = "Insert Into dbo.DailyStockPriceChange(ItemStockId,ItemID,ClosingStock,ClosingAmount,ExpireDate,ItemsPrice,SalesClosingQty,CostPrice,ItemCode,GRN_ID,Barcode,AddBy,AddDate)values('" + Price.ItemStockId + "','" + Price.ItemID + "','" + Price.ClosingStock + "','" + Price.ClosingAmount + "','" + Price.ExpireDate + "','" + Price.ItemsPrice + "','" + Convert.ToInt32(Price.SalesClosingQty) + "','" + Price.CostPrice + "','" + Price.ItemCode + "','" + Price.GRN_ID + "','" + Price.Barcode + "','" + Price.LoginBy + "',GetDate())";
            var command=new SqlCommand(qurey,connection);
             success = command.ExecuteNonQuery();
             connection.Close();
        }

        return success;
    }
}