using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using System.Data.SqlClient;
using Delve;

/// <summary>
/// Summary description for ClsItemDetailsManager
/// </summary>
public class ClsItemDetailsManager
{
	public ClsItemDetailsManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static DataTable GetSetupItemInfo(string Parameter)
    {
        string connectionString = DataManager.OraConnString();
        if (!string.IsNullOrEmpty(Parameter))
        { Parameter = " and t1.DeptID='" + Parameter + "'"; }
        string selectQuery = @"SELECT t1.ID
      ,t1.[Code]
      ,t1.[Name]                 
  FROM [Item] t1  Where DeleteBy is null" + Parameter + "  order by t1.ID DESC ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }
    public static DataTable GetItemsSetupInfo(string Parameter)
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = @"SELECT t1.ID
      ,t1.[Code]
      ,t1.[Name],ID
                 
  FROM [ItemSetup] t1  " + Parameter + "   order by t1.ID DESC ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }
    public static void SaveItemsSetupInformation(ClsItemDetailsInfo aClsItemDetailsInfoObj, DataTable dtSize)
    {

        {
            String connectionString = DataManager.OraConnString();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                string query = "";

                query = @"INSERT INTO [ItemSetup]
 ([Code],[Name],[AddBy],[AddDate],ShortName,Active,ModelNo,description,[Type])
     VALUES
           ('" + aClsItemDetailsInfoObj.ItemsCode + "','" + aClsItemDetailsInfoObj.ItemsName + "','" +
                        aClsItemDetailsInfoObj.LoginBy + "',GETDATE(),'" + aClsItemDetailsInfoObj.ShortName + "','" +
                        aClsItemDetailsInfoObj.Active + "','" + aClsItemDetailsInfoObj.ModelNo + "','" +
                        aClsItemDetailsInfoObj.Description + "','" + aClsItemDetailsInfoObj.Type + "')";
                DataManager.ExecuteNonQuery(connectionString, query);
            }
        }
    }

    public static void UpdateItemsSetupInformation(ClsItemDetailsInfo aClsItemDetailsInfoObj, DataTable dtSize, DataTable OldSize)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;

        connection.Open();
        transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        command.CommandText = @"Update  [ItemSetup] set [Name]='" + aClsItemDetailsInfoObj.ItemsName + "',[Type]='" +
                              aClsItemDetailsInfoObj.Type + "',ShortName='" + aClsItemDetailsInfoObj.ShortName +
                              "',[UpdateBy]='" + aClsItemDetailsInfoObj.LoginBy + "',[UpdateDate]=GETDATE(),Active='" +
                              aClsItemDetailsInfoObj.Active + "' where ID='" + aClsItemDetailsInfoObj.ItemId + "'";
        command.ExecuteNonQuery();

        command.CommandText = @"UPDATE [dbo].[Item]
        SET [Name] ='" + aClsItemDetailsInfoObj.ItemsName + "' ,[ShortName] ='" + aClsItemDetailsInfoObj.ShortName +
                              "' WHERE [ItemSetupID]='" + aClsItemDetailsInfoObj.ItemId + "' ";
        command.ExecuteNonQuery();

        transaction.Commit();
        connection.Close();
    }

    public static void DeleteItemSetupInfo(string ID, string UserID)
    {
        //use as IteamCreate from Delete Btn
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
       
        connection.Open();
        transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        command.CommandText = @"Update ItemSetup set DeleteBy='" + UserID + "',DeleteDate=GETDATE() where ID='" + ID + "' ";
        command.ExecuteNonQuery();

        transaction.Commit();
        connection.Close();
    }

    public static DataTable  GetShowItemSetupDetails(string p)
    {
        String connectionString = DataManager.OraConnString();
        string query =
            @"SELECT ID, Code, Name, ShortName, Active, ModelNo, [description], [Type], AddBy, AddDate, UpdateBy, UpdateDate, DeleteBy, DeleteDate FROM [dbo].[ItemSetup] where DeleteBy IS NULL and  [ID]='" +
            p + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
        return dt;
        //if (dt.Rows.Count == 0)
        //{
        //    return null;
        //}
        //return new ClsItemDetailsInfo(dt.Rows[0]);
    }

    public DataTable GetItemsSetupDetails(string Parameter)
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = @"SELECT [ID],[Code],[Name],[ShortName],[Active],[ModelNo],[description],[Type],[Search]
        FROM [dbo].[View_Search_ItemsSetupInfo] " + Parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }


    //************************** New Code **********************************//
    public static void SaveItemsInformation(ClsItemDetailsInfo aClsItemDetailsInfoObj)
    {
        String connectionString = DataManager.OraConnString();
        string FieldSubCategory = "", ValueSubCategory = "";
        using (SqlConnection sqlCon = new SqlConnection(connectionString))
        {
            string query = "";
            if (aClsItemDetailsInfoObj.ItemsImage != null)
            {
                if (!string.IsNullOrEmpty(aClsItemDetailsInfoObj.SubCatagory))
                {
                    FieldSubCategory = ",[SubCategoryID]"; ValueSubCategory = ",'" + aClsItemDetailsInfoObj.SubCatagory + "'";
                }

                query = @"INSERT INTO [Item]
 ([Code],[Name],[ItemSize],[ItemColor],[UOMID],[UnitPrice],[Currency],[OpeningStock],[OpeningAmount],[ClosingStock],[ClosingAmount],[CategoryID],[TaxCategoryID],[Discounted],[DiscountAmount],[Active],[IsNew],[CreatedBy],[CreatedDate],ItemImage,description,Brand,ShortName,StyleNo,DeptID,SizeID,ItemSetupID " +
                        FieldSubCategory + ") VALUES ('" + aClsItemDetailsInfoObj.ItemsCode + "','" +
                        aClsItemDetailsInfoObj.ItemsName + "','0','0','" + aClsItemDetailsInfoObj.Umo + "','" +
                        aClsItemDetailsInfoObj.UnitPrice + "','" + aClsItemDetailsInfoObj.Currency + "','" +
                        aClsItemDetailsInfoObj.OpeningStock + "','" + aClsItemDetailsInfoObj.OpeningAmount + "','" +
                        aClsItemDetailsInfoObj.ClosingStock + "','" + aClsItemDetailsInfoObj.ClosingAmount + "','" +
                        aClsItemDetailsInfoObj.Catagory + "','" + aClsItemDetailsInfoObj.Text + "','" +
                        aClsItemDetailsInfoObj.DiscountCheck + "','" + aClsItemDetailsInfoObj.Discount + "','" +
                        aClsItemDetailsInfoObj.Active + "','True','" + aClsItemDetailsInfoObj.LoginBy +
                        "',GETDATE(),@img,'" + aClsItemDetailsInfoObj.Description + "','" +
                        aClsItemDetailsInfoObj.Brand + "','" + aClsItemDetailsInfoObj.ShortName + "','" +
                        aClsItemDetailsInfoObj.StyleNo + "','" + aClsItemDetailsInfoObj.DepID + "','" +
                        aClsItemDetailsInfoObj.SizeID + "','" +
                        aClsItemDetailsInfoObj.ItemSetupID + "' " +
                        ValueSubCategory + ")";
            }
            else
            {
                if (!string.IsNullOrEmpty(aClsItemDetailsInfoObj.SubCatagory))
                {
                    FieldSubCategory = ",[SubCategoryID]"; ValueSubCategory = ",'" + aClsItemDetailsInfoObj.SubCatagory + "'";
                }

                query = @"INSERT INTO [Item]
 ([Code],[Name],[ItemSize],[ItemColor],[UOMID],[UnitPrice],[Currency],[OpeningStock],[OpeningAmount],[ClosingStock],[ClosingAmount],[CategoryID],[TaxCategoryID],[Discounted],[DiscountAmount],[Active],[IsNew],[CreatedBy],[CreatedDate],ItemImage,description,Brand,ShortName,StyleNo,DeptID,WarrantyYear,WarrantyMonth,SizeID,ItemSetupID " +
                        FieldSubCategory + ") VALUES ('" + aClsItemDetailsInfoObj.ItemsCode + "','" +
                        aClsItemDetailsInfoObj.ItemsName + "','0','0','" + aClsItemDetailsInfoObj.Umo + "','" +
                        aClsItemDetailsInfoObj.UnitPrice + "','" + aClsItemDetailsInfoObj.Currency + "','" +
                        aClsItemDetailsInfoObj.OpeningStock + "','" + aClsItemDetailsInfoObj.OpeningAmount + "','" +
                        aClsItemDetailsInfoObj.ClosingStock + "','" + aClsItemDetailsInfoObj.ClosingAmount + "','" +
                        aClsItemDetailsInfoObj.Catagory + "','" +
                        aClsItemDetailsInfoObj.Text + "','" + aClsItemDetailsInfoObj.DiscountCheck + "','" +
                        aClsItemDetailsInfoObj.Discount + "','" + aClsItemDetailsInfoObj.Active + "','True','" +
                        aClsItemDetailsInfoObj.LoginBy + "',GETDATE(),null,'" + aClsItemDetailsInfoObj.Description +
                        "','" + aClsItemDetailsInfoObj.Brand + "','" + aClsItemDetailsInfoObj.ShortName + "','" +
                        aClsItemDetailsInfoObj.StyleNo + "','" + aClsItemDetailsInfoObj.DepID + "','" +
                        aClsItemDetailsInfoObj.WarrantyYear + "','" + aClsItemDetailsInfoObj.WarrantyMonth + "','" +
                        aClsItemDetailsInfoObj.SizeID + "','" +
                        aClsItemDetailsInfoObj.ItemSetupID + "'  " +
                        ValueSubCategory + ")";
            }
            SqlParameter img = new SqlParameter();
            img.SqlDbType = SqlDbType.VarBinary;
            img.ParameterName = "img";
            img.Value = aClsItemDetailsInfoObj.ItemsImage;            
            using (SqlCommand cmnd = new SqlCommand(query, sqlCon))
            {
                cmnd.Parameters.Add(img);
                if (aClsItemDetailsInfoObj.ItemsImage == null)
                {
                    cmnd.Parameters.Remove(img);
                }                
                sqlCon.Open();
                cmnd.ExecuteNonQuery();
            }
        }
    }
    public static void UpdateItemsInformation(ClsItemDetailsInfo aClsItemDetailsInfoObj)
    {
        String connectionString = DataManager.OraConnString();
        using (SqlConnection sqlCon = new SqlConnection(connectionString))
        {
            string query = "", ValueSubCategory="";
            if (aClsItemDetailsInfoObj.ItemsImage != null)
            {
                if (!string.IsNullOrEmpty(aClsItemDetailsInfoObj.SubCatagory))
                {
                    ValueSubCategory = ",[SubCategoryID]='" + aClsItemDetailsInfoObj.SubCatagory + "'";
                }

                query = @"UPDATE [Item]
               SET [Name] ='" + aClsItemDetailsInfoObj.ItemsName + "',[UOMID] ='" + aClsItemDetailsInfoObj.Umo +
                        "' ,[UnitPrice] ='" + aClsItemDetailsInfoObj.UnitPrice + "',[Currency] ='" +
                        aClsItemDetailsInfoObj.Currency + "',[OpeningStock] ='" + aClsItemDetailsInfoObj.OpeningStock +
                        "',[OpeningAmount] ='" + aClsItemDetailsInfoObj.OpeningAmount + "',[ClosingStock] ='" +
                        aClsItemDetailsInfoObj.ClosingStock + "',[ClosingAmount] ='" +
                        aClsItemDetailsInfoObj.ClosingAmount + "' ,[CategoryID] ='" + aClsItemDetailsInfoObj.Catagory +
                        "',[TaxCategoryID] ='" +
                        aClsItemDetailsInfoObj.Text + "',[Discounted] ='" + aClsItemDetailsInfoObj.DiscountCheck +
                        "',[DiscountAmount] ='" + aClsItemDetailsInfoObj.Discount + "' ,[Active] ='" +
                        aClsItemDetailsInfoObj.Active + "',[ModifiedBy] ='" + aClsItemDetailsInfoObj.LoginBy +
                        "' ,[ModifiedDate] =GETDATE() ,[description]='" + aClsItemDetailsInfoObj.Description +
                        "',Brand='" + aClsItemDetailsInfoObj.Brand + "',ShortName='" +
                        aClsItemDetailsInfoObj.ShortName + "',WarrantyYear='" + aClsItemDetailsInfoObj.WarrantyYear +
                        "',WarrantyMonth='" + aClsItemDetailsInfoObj.WarrantyMonth + "',StyleNo='" +
                        aClsItemDetailsInfoObj.StyleNo +
                        "',[ItemImage] =@img,DeptID='" + aClsItemDetailsInfoObj.DepID + "',SizeID='" +
                        aClsItemDetailsInfoObj.SizeID + "',ItemSetupID ='" + aClsItemDetailsInfoObj.ItemSetupID + "' " +
                        ValueSubCategory +
                        "  WHERE [Code] ='" +
                        aClsItemDetailsInfoObj.ItemsCode + "' ";
            }
            else
            {
                if (!string.IsNullOrEmpty(aClsItemDetailsInfoObj.SubCatagory))
                {
                    ValueSubCategory = ",[SubCategoryID]='" + aClsItemDetailsInfoObj.SubCatagory + "'";
                }

                query = @"UPDATE [Item]
                SET [Name] ='" + aClsItemDetailsInfoObj.ItemsName + "',[UOMID] ='" + aClsItemDetailsInfoObj.Umo +
                        "' ,[UnitPrice] ='" + aClsItemDetailsInfoObj.UnitPrice + "',[Currency] ='" +
                        aClsItemDetailsInfoObj.Currency + "',[OpeningStock] ='" + aClsItemDetailsInfoObj.OpeningStock +
                        "',[OpeningAmount] ='" + aClsItemDetailsInfoObj.OpeningAmount + "',[ClosingStock] ='" +
                        aClsItemDetailsInfoObj.ClosingStock + "',[ClosingAmount] ='" +
                        aClsItemDetailsInfoObj.ClosingAmount + "' ,[CategoryID] ='" + aClsItemDetailsInfoObj.Catagory +
                        "',[TaxCategoryID] ='" +
                        aClsItemDetailsInfoObj.Text + "',[Discounted] ='" + aClsItemDetailsInfoObj.DiscountCheck +
                        "',[DiscountAmount] ='" + aClsItemDetailsInfoObj.Discount + "' ,[Active] ='" +
                        aClsItemDetailsInfoObj.Active + "',[ModifiedBy] ='" + aClsItemDetailsInfoObj.LoginBy +
                        "' ,[ModifiedDate] =GETDATE() ,[description]='" + aClsItemDetailsInfoObj.Description +
                        "',Brand='" + aClsItemDetailsInfoObj.Brand + "',ShortName='" +
                        aClsItemDetailsInfoObj.ShortName + "',StyleNo='" + aClsItemDetailsInfoObj.StyleNo +
                        "',DeptID='" + aClsItemDetailsInfoObj.DepID + "',SizeID='" + aClsItemDetailsInfoObj.SizeID +
                        "',ItemSetupID='" + aClsItemDetailsInfoObj.ItemSetupID + "'  " +
                        ValueSubCategory + "  WHERE [Code] ='" +
                        aClsItemDetailsInfoObj.ItemsCode + "' ";
            }
            SqlParameter img = new SqlParameter();
            img.SqlDbType = SqlDbType.VarBinary;
            img.ParameterName = "img";
            img.Value = aClsItemDetailsInfoObj.ItemsImage;
            using (SqlCommand cmnd = new SqlCommand(query, sqlCon))
            {
                cmnd.Parameters.Add(img);
                if (aClsItemDetailsInfoObj.ItemsImage == null)
                {
                    cmnd.Parameters.Remove(img);
                }
                sqlCon.Open();
                cmnd.ExecuteNonQuery();
            }
        }
    }
    public static void DeleteItemsInformation(ClsItemDetailsInfo aClsItemDetailsInfoObj)
    {
        string connectionString = DataManager.OraConnString();
        string SelectQuery = @"DELETE FROM [Item]   WHERE [ID] ='" + aClsItemDetailsInfoObj.ItemId + "'";
             DataManager.ExecuteNonQuery(connectionString, SelectQuery);   
    }
    public static object   getShowItemsHistoryDetails(string Parameter)
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = @"SELECT top(100) t1.ID
      ,t1.[Code],t1.StyleNo as [Style No]
      ,t1.[Name]  
      ,t4.BrandName as [Brand Name]     
      ,t6.SizeName AS[Size]   
      ,t2.Name as [Category]
      ,t3.Name as[Sub Category]
      ,t1.UnitPrice as[Unit Price]  
      ,convert(int,t1.ClosingStock) as [Closing Stock]  
     ,t5.Dept_Name AS [Department]        
  FROM [Item] t1
  left join Category t2 on t2.ID=t1.CategoryID left join SubCategory t3 on t3.ID=t1.SubCategoryID
  left join Brand t4 on t4.ID=t1.Brand 
  left join [dbo].[Depertment_Type] t5 on t5.ID=t1.DeptID left join SizeInfo t6 on t6.ID=t1.SizeID " + Parameter + " order by t1.ID desc  ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }

    public static ClsItemDetailsInfo GetShowDetails(string p)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t1.[ID]
      ,t1.[Code],t1.[Name],t1.[ItemImage],t1.[ItemSize],t1.[StyleNo],t1.[ItemColor],t1.[UOMID],t1.[UnitPrice],t1.[Currency],t1.[OpeningStock],t1.[OpeningAmount],t1.[ClosingStock],t1.[ClosingAmount],t1.[CategoryID],t1.[SubCategoryID],t1.[TaxCategoryID],t1.[Discounted],t1.[DiscountAmount],t1.[Active],t2.BrandName,t1.[IsNew]      
      ,t1.[description],t1.Brand,t1.ShortName,t1.DeptID ,WarrantyYear,WarrantyMonth,SizeID,ItemSetupID
  FROM [Item] t1 left join Brand t2 on t2.ID=t1.Brand where t1.[ID]='" + p + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Item");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new ClsItemDetailsInfo(dt.Rows[0]);
    }


    public static DataTable GetShowItemSetUpDetails(string p)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT *
  FROM ItemSetup  where ID='" + p + "'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemSetup");
        return dt;
    }

    public static DataTable ShowTextCatagory()
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = @"SELECT [ID] ,[Name]  FROM [TaxCategory] where Active='True' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }

    public static DataTable GetShowItemsMinibar(string p)
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = @"SELECT [ID],[Name] 
FROM  [Item] where ([Code]+' - '+[Name]) LIKE '%" +p+"%' and [items_type]='1'";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }

    public static int GetShowItemsDetailsInformation()
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        try
        {
            connection.Open();
            string selectQuery = @"select isnull(max(convert(int,t1.Code)),0)+1  from Item t1";
            SqlCommand command = new SqlCommand(selectQuery,connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    public static DataTable GetShowItemsInfo(string SR)
    {
        string connectionString = DataManager.OraConnString();
        string Parameter="";
        if (SR != "") { Parameter = " AND UPPER(t3.Code+' - '+t3.Name+' - '+Isnull(t1.BrandName,'')+' - '+Isnull(t2.Name,'')) LIKE '" + SR + "' "; }
        string selectQuery = @"SELECT t3.ID,t3.Code,t3.Name,t.Price AS UnitPrice,t.Quantity AS ClosingStock,t3.ClosingAmount FROM ItemSalesStock t left join Item t3 on t3.ID=t.ItemsID left join Brand t1 on t3.Brand=t1.ID left join Category t2 on t2.ID=t3.CategoryID where t.Price>0 " + Parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }
   
    public static DataTable GetShowItemsDetaile(string p)
    {
        string connectionString = DataManager.OraConnString();
        string parameter = "";
        if (p != "") { parameter = " where upper(t1.[Code]+ ' - '+t1.[Name])=upper('" + p + "')"; }
        string selectQuery = @"SELECT t1.ID,t1.[Code]+' - '+t1.[Name] AS Items  
      ,t2.Name AS Catagory
      ,t3.Name AS SubCat
      ,t1.[UnitPrice]     
      ,t1.[OpeningStock]
      ,t1.[OpeningAmount]      
      ,t1.[ClosingStock] 
      ,(ISNULL(t1.[UnitPrice],0)*ISNULL(t1.[ClosingStock],0)) AS [ClosingAmount]       
  FROM [Item] t1 left join Category t2 on t2.ID=t1.CategoryID left join SubCategory t3 on t3.ID=t1.SubCategoryID " + parameter + " ORDER BY ISNULL(t1.[ClosingStock],0) DESC ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }
    public static DataTable GetShowItemsDetaile(string p,string Flag)
    {
        string connectionString = DataManager.OraConnString();
        string parameter = "";
        if (p != "") { parameter = " where upper(t1.[Code]+ ' - '+t1.[Name])=upper('" + p + "')"; }
        string selectQuery = @"SELECT t1.ID,t1.[Code]+' - '+t1.[Name] AS Items,t2.Name AS Catagory,t3.Name AS SubCat,t1.[UnitPrice],ISNULL(t4.Quantity,0) AS [ClosingStock]FROM [Item] t1 left join Category t2 on t2.ID=t1.CategoryID left join SubCategory t3 on t3.ID=t1.SubCategoryID LEFT join ItemSalesStock t4 on t4.ItemsID=t1.ID" + parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }
    public static DataTable GetShowItemsInfoSearch(string ItemsName,string ItemID)
    {
        string Parameter = string.Empty;
        string connectionString = DataManager.OraConnString();
        if (!string.IsNullOrEmpty(ItemsName) && string.IsNullOrEmpty(ItemID))
        {
            Parameter = " where upper(SearchItems)= upper('" + ItemsName + "')";
        }
        else if (string.IsNullOrEmpty(ItemsName) && !string.IsNullOrEmpty(ItemID))
        {
            Parameter = " where upper(ID)= upper('" + ItemID + "')";
        }
        string selectQuery =
            @"SELECT [SearchItems],[ID],Name,[Code],[CategoryID],[CatagoryName],[SubCategoryID],[Brand],[BrandName]  ,[ClosingStock] FROM [View_SearchBDItems] " + Parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "View_SearchBDItems");
        return dt;
    }

    public static DataTable GetShowPartyStock(string PartyID, string ItemsID)
    {
        string connectionString = DataManager.OraConnString();
        string parameter = "";
        if (PartyID == "" && ItemsID != "") { parameter = " where upper(t1.[Code]+ ' - '+t1.[Name])=upper('" + ItemsID + "') order by t4.PartyName,t1.ID"; }
        else if (PartyID != "" && ItemsID == "") { parameter = " where upper(t4.[PartyCode]+' - '+t4.[PartyName])=upper('" + PartyID + "') order by t4.PartyName,t1.ID"; }
        else if (PartyID != "" && ItemsID != "") { parameter = " where upper(t4.[PartyCode]+' - '+t4.[PartyName])=upper('" + PartyID + "') and upper(t1.[Code]+ ' - '+t1.[Name])=upper('" + ItemsID + "')  order by t4.PartyName,t1.ID"; }
        else { parameter = " order by t4.PartyName,t1.ID"; }
        string selectQuery = @"select t1.ID,t1.[Code]+' - '+t1.[Name] AS Items  
	  ,t4.PartyName
      ,t2.Name AS Catagory
      ,t3.Name AS SubCat
      ,t1.[UnitPrice]     
      ,t1.[OpeningStock]     
      ,tt.Quantity
       from ItemPartyStock tt inner join [Item] t1 on t1.ID=tt.ItemsID left join Category t2 on t2.ID=t1.CategoryID left join SubCategory t3 on t3.ID=t1.SubCategoryID inner join PartyInfo t4 on t4.ID=tt.PartyID  " + parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        return dt;
    }

    public static DataTable GetBadStockInformation(int i)
    {
        string parameter="",Field="";
        if (i == 0)
        {
            parameter = "where t1.Quantity>0 group by t7.ShiftmentNO,t2.CartoonNo,t4.Code,t3.Quantity,t4.Name,t5.Name,t6.Name,t8.BrandName";
            Field = ",sum(t1.Quantity) as BadQuantity";
        }
        else if (i == 1)
        {
            parameter = "where t1.Lost_Qty>0 group by t7.ShiftmentNO,t2.CartoonNo,t4.Code,t3.Quantity,t4.Name,t5.Name,t6.Name,t8.BrandName";
            Field = ",sum(t1.Lost_Qty) as BadQuantity";
        }
        else if (i == 2)
        {
            parameter = "where t1.Access_Qty>0 group by t7.ShiftmentNO,t2.CartoonNo,t4.Code,t3.Quantity,t4.Name,t5.Name,t6.Name,t8.BrandName";
            Field = ",sum(t1.Access_Qty) as BadQuantity";
        }
        string connectionString = DataManager.OraConnString();
        string selectQuery =
            @"select t4.Code+' - '+t4.Name+' - Brand : '+t8.BrandName ItemsCodeWithName,t5.Name as CategotyName,t6.Name as SubCategotyName,t7.ShiftmentNO,t2.CartoonNo,t3.Quantity as TotalQuantity " +
            Field +
            " from ItemBadStock t1 inner join ShiftmentBoxingMst t2 on t1.ShiftmentBoxingMstID=t2.ID inner join ShiftmentBoxingItemsDtl t3 on t2.ID=t3.MasterID left join dbo.ShiftmentItems tt3 on tt3.ID=t3.ItemsID left join Item t4 on tt3.ItemsID=t4.ID and t1.ItemsID=t4.ID inner join Category t5 on t5.ID=t4.CategoryID inner join SubCategory t6 on t4.SubCategoryID=t6.ID left join Brand t8 on t8.ID=t4.Brand inner join ShiftmentAssigen t7 on t7.ID=t1.ShiftmentID  " +
            parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, selectQuery, "ItemBadStock");
        return dt;
    }

    public static DataTable GetShowSalesItemsInfo(string p)
    {
        string connectionString = DataManager.OraConnString();
        string SelectQuery = @"SELECT t3.ID,t3.Code,t3.Name,t.Price AS UnitPrice,t.Quantity AS ClosingStock,t3.ClosingAmount,tt.ShiftmentNO FROM ItemSalesStock t left join Item t3 on t3.ID=t.ItemsID left join Brand t1 on t3.Brand=t1.ID left join Category t2 on t2.ID=t3.CategoryID inner join ShiftmentAssigen tt on tt.ID=t.Type and t.Flag=1 where t.Quantity>0";

        DataTable dt = DataManager.ExecuteQuery(connectionString, SelectQuery, "T_ItemDetails");
        return dt;
    }

    public static DataTable GetShowItemsSalesStock(string Parameter,string User)
    {
        string connectionString = DataManager.OraConnString();
        string selectQuery = "";
        DataTable dt = null;
        Users usr = UsersManager.getUser(User);
        if (usr != null)
        {
            string FalgMainBranch = IdManager.GetShowSingleValueString("Flag", "ID", "BranchInfo", usr.Dept);
            if (!string.IsNullOrEmpty(FalgMainBranch))
            {
                selectQuery = @" SELECT t.ID ,t3.Code ,t3.Name ,t3.UnitPrice ,t.Quantity AS[ClosingStock],t3.ClosingAmount,CASE WHEN tt.ShiftmentNO IS NULL THEN 'Local Purchase'  ELSE tt.ShiftmentNO END AS ShiftmentNO  FROM ItemSalesStock t left join Item t3 on t3.ID=t.ItemsID left join Brand t1 on t3.Brand=t1.ID left join Category t2 on t2.ID=t3.CategoryID left join ShiftmentAssigen tt on tt.ID=t.[Type] where t.Quantity>0 " + Parameter;
                dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
            }
            else
            {
                selectQuery = @"select t3.ID,t3.Code,t3.Name,t3.UnitPrice AS UnitPrice,tt1.Quantity AS ClosingStock,t3.ClosingAmount,CASE WHEN tt.ShiftmentNO IS NULL THEN 'Local Purchase'  ELSE tt.ShiftmentNO END AS ShiftmentNO from [ItemSalesStockBranchWise] tt1 inner join ItemSalesStock t on t.ID=tt1.ItemsID  left join Item t3 on t3.ID=t.ItemsID left join Brand t1 on t3.Brand=t1.ID left join Category t2 on t2.ID=t3.CategoryID LEFT join ShiftmentAssigen tt on tt.ID=t.[Type] where t.Quantity>0 and tt1.BranchID='" + usr.Dept + "' " + Parameter;
                dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
            }
        }
        else
        {
            selectQuery = @"SELECT t.ID AS[ItemsID],t3.Code AS[item_code],t3.Name AS[item_desc],t.Price AS[Price],t.Quantity AS[StockQty],t3.ClosingAmount,CASE WHEN tt.ShiftmentNO IS NULL THEN 'Local Purchase' Else tt.ShiftmentNO END AS ShiftmentNO ,t.[Flag] AS[Type] FROM ItemSalesStock t left join Item t3 on t3.ID=t.ItemsID left join Brand t1 on t3.Brand=t1.ID left join Category t2 on t2.ID=t3.CategoryID left join ShiftmentAssigen tt on tt.ID=t.[Type]  where t.Quantity>0" + Parameter;
            dt = DataManager.ExecuteQuery(connectionString, selectQuery, "Item");
        }
        return dt;
    }

    public DataTable GetShowAvailableStock(string ReportTypeID, string ShipmentID, string CatagoryID, string SubCatagoryID,string ItemsName,string BranchID,string SupplierID)
    {
        using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
        {
            SqlCommand sqlComm = new SqlCommand("SP_SalesItemsStockDetails", conn);
            if (!string.IsNullOrEmpty(CatagoryID))
            {
                sqlComm.Parameters.AddWithValue("@CatagoryID", CatagoryID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@CatagoryID", null);
            }
            if (!string.IsNullOrEmpty(SubCatagoryID))
            {
                sqlComm.Parameters.AddWithValue("@SubCatagory", SubCatagoryID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@SubCatagory", null);
            }
            if (!ReportTypeID.Equals("0"))
            {
                sqlComm.Parameters.AddWithValue("@TypeID", ReportTypeID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@TypeID", null);
            }
            if (!string.IsNullOrEmpty(ShipmentID))
            {
                sqlComm.Parameters.AddWithValue("@ShipmentID", ShipmentID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@ShipmentID", null);
            }
            if (!string.IsNullOrEmpty(BranchID))
            {
                sqlComm.Parameters.AddWithValue("@BranchID", BranchID);
              
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@BranchID", null);
               //sqlComm.Parameters.AddWithValue("@ReportType", 1);
            }
            sqlComm.Parameters.AddWithValue("@ReportType", ReportTypeID);
            if (!string.IsNullOrEmpty(ItemsName))
            {
                sqlComm.Parameters.AddWithValue("@ItemName", ItemsName);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@ItemName", null);
            }
            if (!string.IsNullOrEmpty(SupplierID))
            {
                sqlComm.Parameters.AddWithValue("@SupplierID", SupplierID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@SupplierID", null);
            }
            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "SP_SalesItemsStockDetails");
            DataTable dtStokDtl = ds.Tables["SP_SalesItemsStockDetails"];
            return dtStokDtl;
        }
    }

    public DataTable GetItemsDetailsOnSearch(string ItemsSearchID)
    {
        string connectionString = DataManager.OraConnString();
        string SelectQuery = @"SELECT [ID],[Items],[Code],[Name],[BrandName],[Catagory],[SubCat],[UnitPrice],[OpeningStock],[OpeningAmount],[ClosingStock],[ClosingAmount],[SearchItems]
  FROM [View_SearchItems] where upper(SearchItems)='" + ItemsSearchID.ToUpper() + "' ";

        DataTable dt = DataManager.ExecuteQuery(connectionString, SelectQuery, "T_ItemDetails");
        return dt;
    }

    public DataTable getShowItemsInfo(string ItemsID,string CatagoryID,string SubCatagoryID,string BrandID,string DepartmentID,string FormDate,string ToDate, string ItemSetupID,string Barcode)
    {
        using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
        {
            SqlCommand sqlComm = new SqlCommand("SP_ItemsStockDetails", conn);
            if (!string.IsNullOrEmpty(ItemsID))
            {
                sqlComm.Parameters.AddWithValue("@ItemsID", ItemsID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@ItemsID", null);
            }
            if (!string.IsNullOrEmpty(BrandID))
            {
                sqlComm.Parameters.AddWithValue("@BrandID", BrandID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@BrandID", null);
            }
            if (!string.IsNullOrEmpty(CatagoryID))
            {
                sqlComm.Parameters.AddWithValue("@CategoryID", CatagoryID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@CategoryID", null);
            }
            if (!string.IsNullOrEmpty(SubCatagoryID))
            {
                sqlComm.Parameters.AddWithValue("@SubCategoryID", SubCatagoryID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@SubCategoryID", null);
                //sqlComm.Parameters.AddWithValue("@ReportType", 1);
            }
            if (!string.IsNullOrEmpty(DepartmentID))
            {
                sqlComm.Parameters.AddWithValue("@DepartmentID", DepartmentID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@DepartmentID", null);

            }
            if (!string.IsNullOrEmpty(FormDate))
            {
                sqlComm.Parameters.AddWithValue("@FormDate", FormDate);
                if (string.IsNullOrEmpty(ToDate))
                {
                    ToDate = FormDate;
                }
                sqlComm.Parameters.AddWithValue("@ToDate", ToDate);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@FormDate", null);
                sqlComm.Parameters.AddWithValue("@ToDate", null);
            }

            if (!string.IsNullOrEmpty(ItemSetupID))
            {
                sqlComm.Parameters.AddWithValue("@ItemSetupID", ItemSetupID);
                
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@ItemSetupID", null);

            }
            if (!string.IsNullOrEmpty(Barcode))
            {
                sqlComm.Parameters.AddWithValue("@Barcode", Barcode);

            }
            else
            {
                sqlComm.Parameters.AddWithValue("@Barcode", null);

            }
            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "SP_ItemsStockDetails");
            DataTable dtStokDtl = ds.Tables["SP_ItemsStockDetails"];
            return dtStokDtl;
        }
    }
    public DataTable getShowBranchItemsInfo(string ItemsID, string CatagoryID, string SubCatagoryID, string BrandID, string DepartmentID, string FormDate, string ToDate, string ItemSetupID, string Barcode,string BranchId)
    {
        using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
        {
            SqlCommand sqlComm = new SqlCommand("SP_ItemsBranchStockDetails", conn);
            if (!string.IsNullOrEmpty(ItemsID))
            {
                sqlComm.Parameters.AddWithValue("@ItemsID", ItemsID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@ItemsID", null);
            }
            if (!string.IsNullOrEmpty(BrandID))
            {
                sqlComm.Parameters.AddWithValue("@BrandID", BrandID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@BrandID", null);
            }
            if (!string.IsNullOrEmpty(CatagoryID))
            {
                sqlComm.Parameters.AddWithValue("@CategoryID", CatagoryID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@CategoryID", null);
            }
            if (!string.IsNullOrEmpty(SubCatagoryID))
            {
                sqlComm.Parameters.AddWithValue("@SubCategoryID", SubCatagoryID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@SubCategoryID", null);
                //sqlComm.Parameters.AddWithValue("@ReportType", 1);
            }
            if (!string.IsNullOrEmpty(DepartmentID))
            {
                sqlComm.Parameters.AddWithValue("@DepartmentID", DepartmentID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@DepartmentID", null);

            }
            if (!string.IsNullOrEmpty(FormDate))
            {
                sqlComm.Parameters.AddWithValue("@FormDate", FormDate);
                if (string.IsNullOrEmpty(ToDate))
                {
                    ToDate = FormDate;
                }
                sqlComm.Parameters.AddWithValue("@ToDate", ToDate);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@FormDate", null);
                sqlComm.Parameters.AddWithValue("@ToDate", null);
            }

            if (!string.IsNullOrEmpty(ItemSetupID))
            {
                sqlComm.Parameters.AddWithValue("@ItemSetupID", ItemSetupID);

            }
            else
            {
                sqlComm.Parameters.AddWithValue("@ItemSetupID", null);

            }
            if (!string.IsNullOrEmpty(Barcode))
            {
                sqlComm.Parameters.AddWithValue("@Barcode", Barcode);

            }
            else
            {
                sqlComm.Parameters.AddWithValue("@Barcode", null);

            }

            if (!string.IsNullOrEmpty(BranchId))
            {
                sqlComm.Parameters.AddWithValue("@BranchId", BranchId);

            }
            else
            {
                sqlComm.Parameters.AddWithValue("@BranchId", null);

            }
            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "SP_ItemsBranchStockDetails");
            DataTable dtStokDtl = ds.Tables["SP_ItemsBranchStockDetails"];
            return dtStokDtl;
        }
    }

    public static DataTable getBrandInfo(string variable)
    {
        string connectionString = DataManager.OraConnString();
        string SelectQuery = @"Select ID,BrandName from Brand where  UPPER (CONVERT(nvarchar,ID)+'-'+BrandName) =Upper ('"+variable+"') ";

        DataTable dt = DataManager.ExecuteQuery(connectionString, SelectQuery, "Brand");
        return dt;
    }

   

    public DataTable getShowItemsInfoDetails(string ItemsID, string CatagoryID, string SubCatagoryID, string BrandID, string DepartmentID, string FormDate, string ToDate)
    {
        using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
        {
            SqlCommand sqlComm = new SqlCommand("SP_Search_Items_Details", conn);
            if (!string.IsNullOrEmpty(ItemsID))
            {
                sqlComm.Parameters.AddWithValue("@ItemsID", ItemsID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@ItemsID", null);
            }
            if (!string.IsNullOrEmpty(BrandID))
            {
                sqlComm.Parameters.AddWithValue("@BrandID", BrandID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@BrandID", null);
            }
            if (!string.IsNullOrEmpty(CatagoryID))
            {
                sqlComm.Parameters.AddWithValue("@CategoryID", CatagoryID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@CategoryID", null);
            }
            if (!string.IsNullOrEmpty(SubCatagoryID))
            {
                sqlComm.Parameters.AddWithValue("@SubCategoryID", SubCatagoryID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@SubCategoryID", null);
                //sqlComm.Parameters.AddWithValue("@ReportType", 1);
            }
            if (!string.IsNullOrEmpty(DepartmentID))
            {
                sqlComm.Parameters.AddWithValue("@DepartmentID", DepartmentID);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@DepartmentID", null);

            }
            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "SP_Search_Items_Details");
            DataTable dtStokDtl = ds.Tables["SP_Search_Items_Details"];
            return dtStokDtl;
        }
    }



    public DataTable GetItemStatus(string ItemID)
    {
        string connectionString = DataManager.OraConnString();
        string Paremeter = "";

        if (!string.IsNullOrEmpty(ItemID))
        {
            Paremeter = " Where ItemID ='" + ItemID + "'";
        }
        string SelectQuery = @"Select top(200)  [ItemCode]
      ,[Name]
      ,[ExpireDate]
      ,[UnitPrice],SizeName
      ,[Quantity] as PurchaseQuantity
      ,[RetQty]
      ,[SalesPrice]
      ,[Total]
      ,[Sales1] as SalesQty
      ,[ClosingStock] from dbo.View_ItemStatus " + Paremeter + " order by ID desc";

        DataTable dt = DataManager.ExecuteQuery(connectionString, SelectQuery, "Brand");
        return dt;
    }

    public DataTable GetBranchInfo(string Parameter)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT top(100) t1.[ID],t1.[BranchID],t2.BranchName,convert(nvarchar,t1.[TransferDate],103)TransferDate,t1.[Remark] ,tot.Qty ,t1.Code  ,CASE WHEN t1.ReceivedBy IS NULL then 'Not Received' else 'Received' end as [Status]
  FROM [ItemStockTransferMst] t1  INNER JOIN BranchInfo t2 on t2.ID=t1.BranchID 
  inner join (select MstID,SUM(ISNULL(TransferQuantity,0))AS Qty from ItemStockTransferDtl WHERE DeleteBy IS NULL GROUP BY MstID) tot on tot.MstID=t1.ID WHERE t1.DeleteBy IS NULL and t1.TransferFromBranchID is null " + Parameter + " ORDER BY convert(date,t1.[TransferDate],103) Desc,t1.ID DESC";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransfer");
        return dt;
    }

    public static DataTable GetBranchInfo()
    {
        string connectionString = DataManager.OraConnString();
        string query = "Select * from View_BranchInfo";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "View_BranchInfo");
        return dt;
    }
}