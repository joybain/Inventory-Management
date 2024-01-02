using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Data.SqlClient;

using Delve;


/// <summary>
/// Summary description for ItemManager
/// </summary>
/// 
namespace Delve
{
    public class ItemManager
    {
        public static void DeleteItems(string item)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "delete from item_mst where item_code='" + item + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void CreateItems(Items item)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " insert into item_mst(item_code,item_desc,msr_unit_code,item_rate,ITEM_DESC_Bang) values ('" + item.ItemCode + "', " +
            " '" + item.ItemDesc + "','" + item.MsrCode + "', convert(decimal(13,2), nullif( '" + item.ItemRate + "' ,'')), " +
            " N'" + item.ItemDescbang + "' )";

            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void UpdateItems(Items item)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " update item_mst set item_code='" + item.ItemCode + "' , " +
            " item_desc= '" + item.ItemDesc + "',msr_unit_code='" + item.MsrCode + "', item_rate=convert(decimal(13,2), nullif( '" + item.ItemRate + "' ,'')), " +
            " ITEM_DESC_Bang =N'" + item.ItemDescbang + "' where item_code= '" + item.ItemCode + "'";

            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static Items GetItem(System.String item)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select item_code,item_desc,msr_unit_code,convert(varchar,item_rate)item_rate,ITEM_DESC_Bang from item_mst where item_code='" + item + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemMst");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Items(dt.Rows[0]);
        }
        public static DataTable GetItems(string criteria)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            //string query = @" SELECT t1.[ID],t1.[Code] AS [item_code],t1.[Name] AS [item_desc],t1.[UOMID] AS [msr_unit_code],t1.[UnitPrice],t1.[Currency],t2.Name AS[UMO] ,t3.BrandName FROM [Item] t1 left join UOM t2 on t2.ID=t1.UOMID left join Brand t3 on t3.ID=t1.Brand  where  upper (t1.Code+ ' - '+t1.Name) = upper('" + criteria + "') and  t1.[Active]=1";
            string query =
                @"SELECT t1.[ID],t1.[Code] AS [item_code],t1.[Name] AS [item_desc],t1.[UOMID] AS [msr_unit_code],t1.[UnitPrice],t1.[Currency],t2.Name AS[UMO] ,t3.BrandName,convert(nvarchar,t4.ExpireDate) as ExpireDate,t1.Code+'--'+t1.Name+'--'+t3.BrandName+'--'+convert(nvarchar,t1.UnitPrice) as ItemSearchDesc FROM [Item] t1 left join UOM t2 on t2.ID=t1.UOMID left join Brand t3 on t3.ID=t1.Brand left join itemStock t4 on t4.ItemID=t1.ID " +
                criteria;

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
            return dt;
        }
        public static DataTable GetItemGrid()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select item_code,item_desc,(select msr_unit_desc from measure_unit where msr_unit_code=a.msr_unit_code)msr_unit_code,convert(varchar,item_rate)item_rate " +
            " from item_mst a order by item_code";            
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemMst");
            return dt;
        }
        public static DataTable GetAllItems()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select item_code,item_desc,msr_unit_code,(select msr_unit_desc from measure_unit where msr_unit_code=a.msr_unit_code)msr_unit_desc,convert(varchar,item_rate)item_rate from item_mst a order by item_code";

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemMst");
            return dt;
        }
        public static DataTable GetMeasure()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select ID,Name from UOM";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Measure");
            return dt;
        }

        public static DataTable getItemBalance()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select a.item_code,c.item_desc,d.msr_unit_desc,a.qnty-coalesce(b.qnty,0) qnty  from "+
            " (select item_code,msr_unit_code,sum(qnty) qnty from DeedDtl group by item_code,msr_unit_code) a inner join "+
            " (select item_code,msr_unit_code,sum(qnty) qnty from IssueDtl group by item_code,msr_unit_code) b on (a.item_code=b.item_code)"+
            " inner join item_mst c on (a.item_code=c.item_code) inner join measure_unit d on (a.msr_unit_code=d.msr_unit_code)"+
            " where a.qnty-coalesce(b.qnty,0)>0";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Measure");
            return dt;
        }

        public static string GetItemDesc(string itm)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select item_desc from item_mst where  item_code='" + itm + "'";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            if (maxValue != null)
            {
                return maxValue.ToString();
            }
            return "";
        }

        public static DataTable GetItemsBercode(string criteria,int Flag)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = "";
            DataTable dt = null;
            if (Flag>1)
            {
                query =
                    @"SELECT [ID],[ItemID],[ClosingStock],[ClosingAmount],[ExpireDate],[ItemsPrice],[Code],[Name],[Category],[SubCategory],[BrandName],[Umo],[Dept_Name],[Items_Code_Name],[Items_Code_Name_Price_ExpDate],BarcodeID,Barcode
  FROM [dbo].[View_Search_Stock_Items] where upper(Items_Code_Name_Price_ExpDate)='" + criteria + "' ";

            }
            else
            {
                query =
                    @"SELECT [ID],[ItemID],[ClosingStock],[ClosingAmount],[ExpireDate],[ItemsPrice],[Code],[Name],[Category],[SubCategory],[BrandName],[Umo],[Dept_Name],[Items_Code_Name],[Items_Code_Name_Price_ExpDate],BarcodeID,Barcode
  FROM [dbo].[View_Search_Stock_Items] where upper(Barcode)='" + criteria + "' "; 
            }
         
            dt = DataManager.ExecuteQuery(connectionString, query, "autoname");
               
            return dt;
        }



        public DataTable GetSearchItemsOnStock1(string SearchItems, int flag,string BranchId)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);


            string query = "";
            DataTable dt = null;
            if (flag>1)
            {
                if (BranchId=="")
                {
                    query =
                    @"SELECT top(1) [ID],StyleNo,[Code],Barcode,[txtItems],CostPrice,[Tax],[DiscountAmount] as DiscountAmountInt,convert(nvarchar,[DiscountAmount]) as DiscountAmount,[SPrice],[Qty],[msr_unit_code],[ClosingStock],TotalClosingStock,[Total] ,[item_Serial],[Remarks],[UnitPrice],[Currency],[UMO],[ClosingAmount],[ExpireDate],[CategoryName],[SubCategoryName],[ItemID],[ItemSize],[ItemColor],[OpeningStock],[OpeningAmount],[CategoryID],[SubCategoryID],[Discounted]
      ,[Active],[SearchItems],[CodeWiseSearchItems]
  FROM [dbo].[View_Search_Items_On_Stock] where UPPER(CodeWiseSearchItems)='" + SearchItems + "' and BranceId is Null";
                    dt = DataManager.ExecuteQuery(connectionString, query, "autoname");
                }
                else
                {
                    query =
                   @"SELECT top(1) [ID],StyleNo,[Code],Barcode,[txtItems],CostPrice,[Tax],[DiscountAmount] as DiscountAmountInt,convert(nvarchar,[DiscountAmount]) as DiscountAmount,[SPrice],[Qty],[msr_unit_code],[ClosingStock],TotalClosingStock,[Total] ,[item_Serial],[Remarks],[UnitPrice],[Currency],[UMO],[ClosingAmount],[ExpireDate],[CategoryName],[SubCategoryName],[ItemID],[ItemSize],[ItemColor],[OpeningStock],[OpeningAmount],[CategoryID],[SubCategoryID],[Discounted]
      ,[Active],[SearchItems],[CodeWiseSearchItems]
  FROM [dbo].[View_Search_Items_On_Stock] where UPPER(CodeWiseSearchItems)='" + SearchItems + "'and BranceId='" + BranchId + "' ";
                    dt = DataManager.ExecuteQuery(connectionString, query, "autoname");
                }
               
            }
            else
            {
                query =
                    @"SELECT top(1) [ID],StyleNo,[Code],Barcode,[txtItems],CostPrice,[Tax],[DiscountAmount] as DiscountAmountInt,convert(nvarchar,[DiscountAmount]) as DiscountAmount,[SPrice],[Qty],[msr_unit_code],[ClosingStock],TotalClosingStock,[Total] ,[item_Serial],[Remarks],[UnitPrice],[Currency],[UMO],[ClosingAmount],[ExpireDate],[CategoryName],[SubCategoryName],[ItemID],[ItemSize],[ItemColor],[OpeningStock],[OpeningAmount],[CategoryID],[SubCategoryID],[Discounted]
      ,[Active],[SearchItems],[CodeWiseSearchItems]
  FROM [dbo].[View_Search_Items_On_Stock] where UPPER(Barcode)='" + SearchItems + "' ";
                dt = DataManager.ExecuteQuery(connectionString, query, "autoname");
            }
            return dt;
        }
        public DataTable GetSearchItemsOnStock(string SearchItems, int flag)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = "";
            DataTable dt = null;
            if (flag > 1)
            {
                //                query =
                //                    @"SELECT top(1) [ID],StyleNo,[Code],Barcode,[txtItems],CostPrice,[Tax],[DiscountAmount] as DiscountAmountInt,convert(nvarchar,[DiscountAmount]) as DiscountAmount,[SPrice],[Qty],[msr_unit_code],[ClosingStock],TotalClosingStock,[Total] ,[item_Serial],[Remarks],[UnitPrice],[Currency],[UMO],[ClosingAmount],[ExpireDate],[CategoryName],[SubCategoryName],[ItemID],[ItemSize],[ItemColor],[OpeningStock],[OpeningAmount],[CategoryID],[SubCategoryID],[Discounted]
                //      ,[Active],[SearchItems],[CodeWiseSearchItems]
                //  FROM [dbo].[View_Search_Items_On_Stock] where UPPER(CodeWiseSearchItems)='" + SearchItems + "' ";
                query =
                    @"SELECT top(1) [ID],StyleNo,[Code],Barcode,[txtItems],CostPrice,[Tax],[DiscountAmount] as DiscountAmountInt,convert(nvarchar,[DiscountAmount]) as DiscountAmount,[SPrice],[Qty],[msr_unit_code],[ClosingStock],TotalClosingStock,[Total] ,[item_Serial],[Remarks],[UnitPrice],[Currency],[UMO],[ClosingAmount],[ExpireDate],[CategoryName],[SubCategoryName],[ItemID],[ItemSize],[ItemColor],[OpeningStock],[OpeningAmount],[CategoryID],[SubCategoryID],[Discounted]
      ,[Active],[SearchItems],[CodeWiseSearchItems]
  FROM [dbo].[View_Search_Items_On_Stock] where UPPER(CodeWiseSearchItems)='" + SearchItems + "'";
                dt = DataManager.ExecuteQuery(connectionString, query, "autoname");
            }
            else
            {
                query =
                    @"SELECT top(1) [ID],StyleNo,[Code],Barcode,[txtItems],CostPrice,[Tax],[DiscountAmount] as DiscountAmountInt,convert(nvarchar,[DiscountAmount]) as DiscountAmount,[SPrice],[Qty],[msr_unit_code],[ClosingStock],TotalClosingStock,[Total] ,[item_Serial],[Remarks],[UnitPrice],[Currency],[UMO],[ClosingAmount],[ExpireDate],[CategoryName],[SubCategoryName],[ItemID],[ItemSize],[ItemColor],[OpeningStock],[OpeningAmount],[CategoryID],[SubCategoryID],[Discounted]
      ,[Active],[SearchItems],[CodeWiseSearchItems]
  FROM [dbo].[View_Search_Items_On_Stock] where UPPER(Barcode)='" + SearchItems + "' ";
                dt = DataManager.ExecuteQuery(connectionString, query, "autoname");
            }
            return dt;
        }
        public DataTable GetSearchItemsOnStockBranchWish(string SearchItems, int flag,string BranchId)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = "";
            DataTable dt = null;
            if (flag > 1)
            {
                //                query =
                //                    @"SELECT top(1) [ID],StyleNo,[Code],Barcode,[txtItems],CostPrice,[Tax],[DiscountAmount] as DiscountAmountInt,convert(nvarchar,[DiscountAmount]) as DiscountAmount,[SPrice],[Qty],[msr_unit_code],[ClosingStock],TotalClosingStock,[Total] ,[item_Serial],[Remarks],[UnitPrice],[Currency],[UMO],[ClosingAmount],[ExpireDate],[CategoryName],[SubCategoryName],[ItemID],[ItemSize],[ItemColor],[OpeningStock],[OpeningAmount],[CategoryID],[SubCategoryID],[Discounted]
                //      ,[Active],[SearchItems],[CodeWiseSearchItems]
                //  FROM [dbo].[View_Search_Items_On_Stock] where UPPER(CodeWiseSearchItems)='" + SearchItems + "' ";
                query =
                    @"SELECT top(1) [ID],StyleNo,[Code],Barcode,[txtItems],CostPrice,[Tax],[DiscountAmount] as DiscountAmountInt,convert(nvarchar,[DiscountAmount]) as DiscountAmount,[SPrice],[Qty],[msr_unit_code],[ClosingStock],TotalClosingStock,[Total] ,[item_Serial],[Remarks],[UnitPrice],[Currency],[UMO],[ClosingAmount],[ExpireDate],[CategoryName],[SubCategoryName],[ItemID],[ItemSize],[ItemColor],[OpeningStock],[OpeningAmount],[CategoryID],[SubCategoryID],[Discounted]
      ,[Active],[SearchItems],[CodeWiseSearchItems]
  FROM [dbo].[View_Search_Items_On_Stock] where UPPER(CodeWiseSearchItems)='" + SearchItems + "'and BranceId='"+BranchId+"'";
                dt = DataManager.ExecuteQuery(connectionString, query, "autoname");
            }
            else
            {
                query =
                    @"SELECT top(1) [ID],StyleNo,[Code],Barcode,[txtItems],CostPrice,[Tax],[DiscountAmount] as DiscountAmountInt,convert(nvarchar,[DiscountAmount]) as DiscountAmount,[SPrice],[Qty],[msr_unit_code],[ClosingStock],TotalClosingStock,[Total] ,[item_Serial],[Remarks],[UnitPrice],[Currency],[UMO],[ClosingAmount],[ExpireDate],[CategoryName],[SubCategoryName],[ItemID],[ItemSize],[ItemColor],[OpeningStock],[OpeningAmount],[CategoryID],[SubCategoryID],[Discounted]
      ,[Active],[SearchItems],[CodeWiseSearchItems]
  FROM [dbo].[View_Search_Items_On_Stock] where UPPER(Barcode)='" + SearchItems + "' ";
                dt = DataManager.ExecuteQuery(connectionString, query, "autoname");
            }
            return dt;
        }
        public DataTable GetSearchItemsOnBranchStock(string SearchItems, int flag,string BranchId)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = "";
            DataTable dt = null;
            if (flag > 1)
            {
                query =
                    @"SELECT top(1) [ID],StyleNo,[Code],Barcode,[txtItems],CostPrice,[Tax],[DiscountAmount] as DiscountAmountInt,convert(nvarchar,[DiscountAmount]) as DiscountAmount,[SPrice],[Qty],[msr_unit_code],[ClosingStock],TotalClosingStock,[Total] ,[item_Serial],[Remarks],[UnitPrice],[Currency],[UMO],[ClosingAmount],[ExpireDate],[CategoryName],[SubCategoryName],[ItemID],[ItemSize],[ItemColor],[OpeningStock],[OpeningAmount],[CategoryID],[SubCategoryID],[Discounted]
      ,[Active],[SearchItems],[CodeWiseSearchItems]
  FROM [dbo].[View_Search_Items_On_OutLetStock] where BranchId='"+BranchId+"' and UPPER(CodeWiseSearchItems)='" + SearchItems + "' ";
                dt = DataManager.ExecuteQuery(connectionString, query, "autoname");
            }
            else
            {
                query =
                    @"SELECT top(1) [ID],StyleNo,[Code],Barcode,[txtItems],CostPrice,[Tax],[DiscountAmount] as DiscountAmountInt,convert(nvarchar,[DiscountAmount]) as DiscountAmount,[SPrice],[Qty],[msr_unit_code],[ClosingStock],TotalClosingStock,[Total] ,[item_Serial],[Remarks],[UnitPrice],[Currency],[UMO],[ClosingAmount],[ExpireDate],[CategoryName],[SubCategoryName],[ItemID],[ItemSize],[ItemColor],[OpeningStock],[OpeningAmount],[CategoryID],[SubCategoryID],[Discounted]
      ,[Active],[SearchItems],[CodeWiseSearchItems]
  FROM [dbo].[View_Search_Items_On_OutLetStock] where BranchId='" + BranchId + "' and UPPER(Barcode)='" + SearchItems + "' ";
                dt = DataManager.ExecuteQuery(connectionString, query, "autoname");
            }
            return dt;
        }
        public DataTable GetSearchItemsWithSize(string SearchItems)
        {
            DataTable dt = IdManager.GetShowDataTable(@"SELECT [ID],[Name],[ShortName]
        FROM [dbo].[View_SearchItemOnPurchase] where upper([SearchItem])=upper('" + SearchItems + "')");
            return dt;
        }

        public DataTable GetItemsInformationDetailsCode(string itemsID, string BrandID, string CatagpryID, string SubCatagoryID, string searchItems, int Check, string ItemCode)
        {
            using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
            {
                SqlCommand sqlComm = new SqlCommand("SP_TransferSearchItems", conn);
                if (!string.IsNullOrEmpty(itemsID))
                {
                    sqlComm.Parameters.AddWithValue("@itemsID", itemsID);
                }
                else
                {
                    sqlComm.Parameters.AddWithValue("@itemsID", null);
                }


                if (!string.IsNullOrEmpty(BrandID))
                {
                    sqlComm.Parameters.AddWithValue("@BrandID", BrandID);
                }
                else
                {
                    sqlComm.Parameters.AddWithValue("@BrandID", null);
                }
                if (!string.IsNullOrEmpty(CatagpryID))
                {
                    sqlComm.Parameters.AddWithValue("@CatagpryID", CatagpryID);
                }
                else
                {
                    sqlComm.Parameters.AddWithValue("@CatagpryID", null);
                }
                if (!string.IsNullOrEmpty(SubCatagoryID))
                {
                    sqlComm.Parameters.AddWithValue("@SubCatagory", SubCatagoryID);
                }
                else
                {
                    sqlComm.Parameters.AddWithValue("@SubCatagory", null);
                }
                if (!string.IsNullOrEmpty(searchItems))
                {
                    sqlComm.Parameters.AddWithValue("@searchItems", searchItems);
                }
                else
                {
                    sqlComm.Parameters.AddWithValue("@searchItems", null);
                }
                if (!string.IsNullOrEmpty(ItemCode))
                {
                    sqlComm.Parameters.AddWithValue("@ItemCode", ItemCode);
                }
                else
                {
                    sqlComm.Parameters.AddWithValue("@ItemCode", null);
                }


                sqlComm.Parameters.AddWithValue("@Check", Check);
                sqlComm.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = sqlComm;
                da.Fill(ds, "SP_TransferSearchItems");
                DataTable dtStdSummery = ds.Tables["SP_TransferSearchItems"];
                return dtStdSummery;
            }
        }



     
    
   public DataTable GetItemsInformationDetails(string itemsID, string BrandID, string CatagpryID, string SubCatagoryID, string searchItems, int Check)
    {
        using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
        {
            SqlCommand sqlComm = new SqlCommand("SP_TransferSearchItems", conn);
            if (!string.IsNullOrEmpty(itemsID))
            {
                sqlComm.Parameters.AddWithValue("@itemsID", itemsID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@itemsID", null);
            }
            if (!string.IsNullOrEmpty(BrandID))
            {
                sqlComm.Parameters.AddWithValue("@BrandID", BrandID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@BrandID", null);
            }
            if (!string.IsNullOrEmpty(CatagpryID))
            {
                sqlComm.Parameters.AddWithValue("@CatagpryID", CatagpryID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@CatagpryID", null);
            }
            if (!string.IsNullOrEmpty(SubCatagoryID))
            {
                sqlComm.Parameters.AddWithValue("@SubCatagory", SubCatagoryID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@SubCatagory", null);
            }
            if (!string.IsNullOrEmpty(searchItems))
            {
                sqlComm.Parameters.AddWithValue("@searchItems", searchItems);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@searchItems", null);
            }

            sqlComm.Parameters.AddWithValue("@ItemCode", null);

            sqlComm.Parameters.AddWithValue("@Check", Check);
            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "SP_TransferSearchItems");
            DataTable dtStdSummery = ds.Tables["SP_TransferSearchItems"];
            return dtStdSummery;
        }
    }



   public DataTable GetCartonWiseItemInfo(string CartonNo)
   {
       string connectionString = DataManager.OraConnString();
       SqlConnection sqlCon = new SqlConnection(connectionString);

       string query = @"select t1.CartonNo,t2.ID as CartonID,t1.ID as CartonMstID,t3.SalePrice as BranchSalesPrice ,0 as Discount,t3.ID as ItemsID,t3.StyleNo as StyleNo,t1.CartonNo,t4.Name as ItemsName,t3.ItemCode as item_code,t4.Name as item_desc
,isnull(t2.ReceivedQuantity,0) as StockQty ,isnull(Convert(int,t2.ReceivedQuantity),0) as TransferQty,t3.CostingPrice as Price,

'0' as ReceivedQuantity
 from CartonReceivedMST t1
 inner join CartonReceived t2 on t2.MSTID=t1.ID and t1.TransferStatus is null
 inner join ItemPurOrderDtl t3 on t3.ID=t2.ItemID 
 inner join Item t4 on t4.ID=t3.ItemID
 
 where t1.CartonNo='" + CartonNo + "' and t2.DeleteBy is null and t2.DeleteBy is null and t1.CartonNo not in (Select isnull(CartonNo,0) from [dbo].[ItemStockTransferMst] where [DeleteBy] is null) ";
       DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
       return dt;
   }

    }
}