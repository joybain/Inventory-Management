using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Delve;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.SqlTypes;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;

/// <summary>
/// Summary description for clsItemTransferStockManager
/// </summary>
public class clsItemTransferStockManager
{
	public clsItemTransferStockManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    internal void SaveReceicedQuentity(DataTable table, int UserID)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        //  SqlTransaction transaction;
        connection.Open();
        // transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;

        foreach (DataRow drRow in table.Rows)
        {


            if (!string.IsNullOrEmpty(drRow["ChangeQty"].ToString()))
            {
                if (!string.IsNullOrEmpty(drRow["OldReceivedQty"].ToString()))
                {
                    command.CommandText = @"UPDATE [Item] SET [ClosingStock] =ISNULL([ClosingStock],0)-Isnull('" +
                                       drRow["OldReceivedQty"].ToString().Replace(",", "") + "',0)  WHERE Code='" +
                                       drRow["Code"].ToString() + "' ";
                    command.ExecuteNonQuery();
                    //command.CommandText = @"UPDATE " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[Item] SET [ClosingStock] =ISNULL([ClosingStock],0)+Isnull('" +
                    //                   drRow["OldReceivedQty"].ToString().Replace(",", "") + "',0)  WHERE Code='" +
                    //                   drRow["Code"].ToString() + "' ";
                    //command.ExecuteNonQuery();
                }


                command.CommandText = @"UPDATE [ItemStockTransferDtl] SET [ReceivedQuantity] ='" +
                                       drRow["ChangeQty"].ToString() +
                                       "'  WHERE ID='" +
                                       drRow["ID"].ToString() +
                                       "'";
                command.ExecuteNonQuery();

                command.CommandText = @"UPDATE Item set [ClosingStock]=isnull([ClosingStock],0)+(" +
                                       drRow["ChangeQty"].ToString() +
                                       "),ClosingAmount=isnull(ClosingAmount,0)+(" +
                                       drRow["ChangeQty"].ToString() + "*" + drRow["BranchSalesPrice"].ToString() + ") where [Code]='" + drRow["Code"].ToString() + "'";
                command.ExecuteNonQuery();



                command.CommandText = @"UPDATE " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[ItemStockTransferDtl] SET LocalUpload=1, [ReceivedQuantity]='" +
                                       drRow["ChangeQty"].ToString() +
                                       "'  WHERE id='" +
                                       drRow["DtlID"].ToString() +
                                       "'";
                command.ExecuteNonQuery();

                //string query2 = @" update " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[ItemStockTransferDtl] set LocalUpload=1,ReceivedQuantity='" + Convert.ToInt32(dr["TransferQuantity"]) + "' WHERE MstID= '" + dr["MstID"].ToString() + "' and ID='" + dr["ID"].ToString() + "'";
                // DataManager.ExecuteNonQuery(DataManager.OraConnString(), query2);

                //command.CommandText = @"UPDATE " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[Item] set [ClosingStock]=isnull([ClosingStock],0)-(" +
                //                      drRow["ChangeQty"].ToString() +
                //                      "),ClosingAmount=isnull(ClosingAmount,0)+(" +
                //                      drRow["ChangeQty"].ToString() + "*" + drRow["BranchSalesPrice"].ToString() + ") where [Code]=" + drRow["Code"].ToString();
                //command.ExecuteNonQuery();


            }
        }
        command.CommandText = @"UPDATE [ItemStockTransferMst]
            SET  [ReceivedDate] =GETDATE(),[ReceivedBy] =" + UserID + "  WHERE [Mst_ID]='" +
                              table.Rows[0]["MstID"].ToString() +
                              "'";
        command.ExecuteNonQuery();

        //******************* Update Server *********************//

        command.CommandText = @"UPDATE " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[ItemStockTransferMst] SET   [ReceivedDate] =GETDATE(),[ReceivedBy] =" + UserID + "  WHERE [ID]='" +
                              table.Rows[0]["MstID"].ToString() + "'";
        command.ExecuteNonQuery();
        connection.Close();
        //  transaction.Commit();
    }


   public static DataTable   getShowTransferMst(int MainBranchID,string ByDate,string ToDate)
   {
       string query = "";
      
        String connectionString = DataManager.OraConnString();
//        string query = @"SELECT [ID],[BranchID],TransferFromBranchID,convert(nvarchar,[TransferDate],103)TransferDate,convert(datetime,[TransferDate],103)TransferDate2,[Remark],TransferType,Code ,ChallanNo    
//            FROM " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[ItemStockTransferMst] WHERE [ReceivedBy] IS NULL AND [DeleteBy] IS NULL and BranchID=" + MainBranchID + " and (LocalUpload is null or LocalUpload!=1) ";

       //if (string.IsNullOrEmpty(ByDate) || string.IsNullOrEmpty(ToDate))
       //{
           query = @"SELECT [ID],[BranchID],TransferFromBranchID,convert(nvarchar,[TransferDate],103)TransferDate,convert(datetime,[TransferDate],103)TransferDate2,[Remark],TransferType,Code ,ChallanNo ,'Head Office' as TransferFrom , 'Not Received' as Status
 FROM " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[ItemStockTransferMst] WHERE [ReceivedBy] IS NULL AND [DeleteBy] IS NULL and BranchID=" + MainBranchID + " and (LocalUpload is null or LocalUpload!=1) ";
       
      
        
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransferMst");
        return dt;
    }

   public static DataTable getShowTransferDtl(string MstID)
    {
        String connectionString = DataManager.OraConnString();
//        string query = @"SELECT t1.[ID],t1.[MstID],st.NewItemCode AS item_code,itm.ID as ItemId,t1.[TransferQuantity],t1.[Type],t2.ID as ServerItemID,isnull(t2.[Type],0) as _ItemType ,
//itm.StyleNo,t3.Name AS[Catagory],t4.Name AS[SubCatagory],t2.Code as _Code,t5.Name AS[UMO],fb.Name as FabricsType,itm.FabricsType as FabricsTypeID,gd.Name as GoodsType,itm.GoodsTypeID
//,t2.Name AS item_desc,isnull(t1.TransferPrice,0) as CostingPrice,Isnull(t1.Discount,0) as Discount,t3.Code as CategoryCode,t4.Code as SubCategoryCode,itm.DesigNo,itm.[Session],t3.ID as CategoryID,t4.ID as subCategoryID,itm.FabricPattan,
//isnull(t1.BranchSalesPrice,0) as BranchSalesPrice,sz.SizeName as ItemSize,cl.ColorName as ItemColor,cl.Code as ColorCode,sz.Code as SizeCode " +

//" FROM " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[ItemStockTransferDtl] t1  inner join " + ConfigurationManager.AppSettings["DataBase"] + ".dbo.ItemStock st  on st.NewItemCode=t1.Code and ItemType=t1.[Type]  inner join  " + ConfigurationManager.AppSettings["DataBase"] + ".dbo.ItemPurOrderDtl itm on itm.ItemCode=st.ItemCode " +
//        " left  join " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].ColorInfo cl on cl.Code=itm.ColorCode " +
//         " left  join " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].SizeInfo sz on sz.Code=itm.SizeCode " +
//         " inner join " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[item] t2 on t2.ID=itm.ItemId  " +
//          "LEFT join " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].Category t3 on t3.ID=itm.CategoryID " +
//         " LEFT join " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].SubCategory t4 on t4.ID=itm.SubCategeoryID  " +
//         " LEFT join " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].UOM t5 on t5.ID=t2.UOMID   " +
//        "where t1.MstID in (" + MstID + ") AND t1.[DeleteBy] IS NULL and (t1.LocalUpload is null or t1.LocalUpload!=1)";


        string query = @"select t1.ID,t1.MstId,t1.TransferQuantity,t1.TransferPrice,t1.BranchSalesPrice,t1.receivedQuantity,t1.Code,t1.Discount,t1.Barcode,t1.BranchID
,t5.ID AS UOMID,t5.Name AS UomName,t5.Active AS Active,--UOM
t8.Id as BrandId,t8.BrandName,t8.Active AS BrandActive --Brand
,t7.Id as DeptId,t7.Dept_Name,--Depertment
t6.Id as taxId,t6.Name as TaxName,t6.Rate,t6.Active as TaxActive,--Tax
t2.CategoryID,t3.Code as CategoryCode,t3.Name AS[CatagoryName],t3.Description as CatagoryDescription,t3.Active as CatagoryActive,--Catagory
t2.SubCategoryID,t4.Code as SubCategoryCode,t4.Description as subCatagoryDescription,t4.Name AS[SubCatagory],t4.Active as subCatagoryActive,--SubCatagory
sz.ID as SizeId,sz.SizeName as ItemSize --SizeInfo
,cl.ID as ColorId,cl.ColorName as ItemColor,--ColorInfo
Itms.Id as ItmsId,Itms.Code as ItmsCode,Itms.Name as ItmsName,Itms.ShortName as ItmsShortName, Itms.Active as ItmsActive ,--Itme Setup
t2.ID AS ItemId,t2.Code as ItemCode,t2.Name as ItemName,t2.UnitPrice,t2.Currency,t2.OpeningStock,t2.OpeningAmount,t2.ClosingStock,t2.ClosingAmount,t2.Discounted,t2.DiscountAmount,t2.Active as ItemActive ,t2.IsNew,t2.description as Itemdescription,t2.ShortName,t2.StyleNo,--Item
st.Id as StockId,st.ClosingStock as stClosingStock,st.ExpireDate,st.ItemsPrice,st.CostPrice,st.ItemCode as stItemCode,st.GRN_ID,st.Barcode --ItemStock
FROM  " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[ItemStockTransferDtl] t1 "+
" inner join  " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].ItemStock st  on st.ItemId=t1.ItemId and st.Barcode=t1.Barcode   and st.DeleteDate is null "+
 " inner join  " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[item] t2 on t2.ID=t1.ItemId and t2.DeleteBy is null "+
" inner join  " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].ItemSetup ItmS on ItmS.Id=t2.ItemSetupID and Itms.DeleteDate is null "+
" left  join  " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].ColorInfo cl on cl.ID=t2.ItemColor "+
" left  join  " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].SizeInfo sz on sz.ID=t2.SizeID "+
" LEFT join  " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].Category t3 on t3.ID=t2.CategoryID and t3.DeleteDate is null "+
" LEFT join  " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].SubCategory t4 on t4.ID=t2.SubCategoryID and t4.DeleteDate is null"+
" LEFT join " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].UOM t5 on t5.ID=t2.UOMID   "+
" left join " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].TaxCategory  t6 on t2.TaxCategoryId =t6.ID "+
" left join  " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].Depertment_Type t7 on t2.DeptId=t7.Id and t7.Delete_Date is null "+
"left join   " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].Brand t8 on t2.Brand=t8.ID "+
" where t1.MstID in (" + MstID + ") AND t1.[DeleteBy] IS NULL and (t1.LocalUpload is null or t1.LocalUpload!=1)";


        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransferMst");
        return dt;
    }

   public static void SaveTranseferStock(DataTable dtTransferMst, DataTable dtTransferDtl,DataTable data1,int IsExist, string LoginBy,string BranchId)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        connection.Open();
        transaction = connection.BeginTransaction();
        try
        {

            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            SqlCommand command1 = new SqlCommand();
            command1.Connection = connection;
            command1.Transaction = transaction;

            int BarcodeExistId =0, ItemsCode =0, UMO_ID =0, BrandId =0, Dept_ID =0, Tax_ID =0,
                catagoryID =0, SubCatagoryID =0, SizeID =0, ColorID =0, ItemSetUpId =0, ItemId =0, ItemStockId = 0;
            decimal ClosingStock = 0;
            decimal ClosingAmount = 0;
                decimal SalesPrice= 0;

                if (IsExist>0)
                {
                    command1.CommandText = "Update Supplier set Gl_CoaCode='111',BranchId='" + BranchId + "', Code='" + data1.Rows[0]["CASH_CODE"] + "',Email='" + data1.Rows[0]["Email"] + "',Phone='" + data1.Rows[0]["PHONE"] + "',Address1='" + data1.Rows[0]["COMPANY_ADDRESS1"] + "',Address2='" + data1.Rows[0]["COMPANY_ADDRESS2"] + "' where BranchId='" + BranchId + "'";

                    

                    command1.ExecuteNonQuery();
                }
                else
                {
                    command1.CommandText = "insert into Supplier (Gl_CoaCode,Code,Name,ContactName,Email,Phone,Active,Address1,Address2,CreatedBy,CreatedDate,BranchId) values('111','" + data1.Rows[0]["CASH_CODE"] + "' ,'Main Branch','Mr.A','" + data1.Rows[0]["Email"] + "','" + data1.Rows[0]["PHONE"] + "','1','" + data1.Rows[0]["COMPANY_ADDRESS1"] + "','" + data1.Rows[0]["COMPANY_ADDRESS2"] + "','" + LoginBy + "',Getdate(),'"+BranchId+"')";
                    command1.ExecuteNonQuery();
                }


            foreach (DataRow dr in dtTransferDtl.Rows)
            {

                command1.CommandText = "select Count(*) from ItemStockreceivedDtl Where DtlID ='" + dr["ID"].ToString() + "'";
                int Flag2 = Convert.ToInt32(command1.ExecuteScalar());
                if (Flag2 <= 0)
                {
                    command1.CommandText = @"select ID from [OutLetItemStock] WHERE UPPER(Barcode)=UPPER('" +
                                           dr["Barcode"].ToString() + "') and BranchID='" + dr["BranchId"].ToString() + "'";
                    BarcodeExistId = Convert.ToInt32(command1.ExecuteScalar());
                    command1.CommandText = @"SELECT Id FROM [Item] where UPPER([Code])=UPPER('" +
                                           dr["ItemCode"].ToString() + "')";
                    ItemId = Convert.ToInt32(command1.ExecuteScalar());
                    if (BarcodeExistId == 0)
                    {
                       
                        if (ItemId == 0)
                        {
                            //************** Check UMO ******************//

                            UMO_ID = 0;
                            if (!string.IsNullOrEmpty(dr["UomName"].ToString()))
                            {
                                command1.CommandText = @"select ID from UOM WHERE UPPER(Name)=UPPER('" +
                                                       dr["UomName"].ToString() + "')";
                                UMO_ID = Convert.ToInt32(command1.ExecuteScalar());
                                if (UMO_ID == 0)
                                {
                                    command.CommandText = @"INSERT INTO [UOM] ([Name],Active)
                               VALUES ('" + dr["UomName"].ToString() + "','True')";
                                    command.ExecuteNonQuery();

                                    command1.CommandText = "select top(1)ID from UOM order by ID desc";
                                    UMO_ID = Convert.ToInt32(command1.ExecuteScalar());
                                }
                            }

                            //************** Check Brand Id ******************//

                            BrandId = 0;
                            if (!string.IsNullOrEmpty(dr["BrandName"].ToString()))
                            {
                                command1.CommandText = @"select ID from Brand WHERE UPPER(BrandName)=UPPER('" +
                                                       dr["BrandName"].ToString() + "')";
                                BrandId = Convert.ToInt32(command1.ExecuteScalar());
                                if (BrandId == 0)
                                {
                                    command.CommandText = @"INSERT INTO [Brand] (BrandName,Active)
                               VALUES ('" + dr["BrandName"].ToString() + "','True')";
                                    command.ExecuteNonQuery();

                                    command1.CommandText = "select top(1)ID from Brand order by ID desc";
                                    BrandId = Convert.ToInt32(command1.ExecuteScalar());
                                }
                            }


                            //************** Check Depertment ******************//

                            Dept_ID = 0;
                            if (!string.IsNullOrEmpty(dr["Dept_Name"].ToString()))
                            {
                                command1.CommandText =
                                    @"select ID from Depertment_Type WHERE UPPER(Dept_Name)=UPPER('" +
                                    dr["Dept_Name"].ToString() + "')";
                                Dept_ID = Convert.ToInt32(command1.ExecuteScalar());
                                if (Dept_ID == 0)
                                {
                                    command.CommandText =
                                        @"INSERT INTO [Depertment_Type] (Dept_Name,Entry_By,Entry_Date)
                               VALUES ('" + dr["Dept_Name"].ToString() + "','" + LoginBy + "',GetDate())";
                                    command.ExecuteNonQuery();

                                    command1.CommandText = "select top(1)ID from Depertment_Type order by ID desc";
                                    Dept_ID = Convert.ToInt32(command1.ExecuteScalar());
                                }
                            }


                            //************** Check TaxCategory ******************//

                            Tax_ID = 0;
                            if (!string.IsNullOrEmpty(dr["TaxName"].ToString()))
                            {
                                command1.CommandText = @"select ID from TaxCategory WHERE UPPER(Name)=UPPER('" +
                                                       dr["TaxName"].ToString() + "')";
                                Tax_ID = Convert.ToInt32(command1.ExecuteScalar());
                                if (Tax_ID == 0)
                                {
                                    command.CommandText =
                                        @"INSERT INTO [TaxCategory] ([Name],Rate,Active,CreatedBy,CreatedDate)
                               VALUES ('" + dr["TaxName"].ToString() + "','" + dr["Rate"].ToString() + "','True','" +
                                        LoginBy + "',GetDate())";
                                    command.ExecuteNonQuery();

                                    command1.CommandText = "select top(1)ID from TaxCategory order by ID desc";
                                    Tax_ID = Convert.ToInt32(command1.ExecuteScalar());
                                }
                            }



                            //************** Check Catagory ******************//
                            catagoryID = 0;
                            SubCatagoryID = 0;
                            if (!string.IsNullOrEmpty(dr["CatagoryName"].ToString()))
                            {
                                if (dr["CatagoryName"].ToString().ToUpper().Equals("NONE"))
                                {
                                    catagoryID = 122;
                                }
                                else
                                {
                                    command1.CommandText = @"select ID from Category WHERE UPPER(Name)=UPPER('" +
                                                           dr["CatagoryName"].ToString() + "')";
                                    catagoryID = Convert.ToInt32(command1.ExecuteScalar());
                                    if (catagoryID == 0)
                                    {
                                        command.CommandText =
                                            @"INSERT INTO [Category] (Code,Name,Active,CreatedBy,CreatedDate)
                               VALUES ('" + dr["CategoryCode"].ToString() + "','" + dr["CatagoryName"].ToString() +
                                            "','True','" + LoginBy + "',GetDate())";
                                        command.ExecuteNonQuery();

                                        command1.CommandText = "select top(1)ID from Category order by ID desc";
                                        catagoryID = Convert.ToInt32(command1.ExecuteScalar());
                                    }
                                }
                            }

                            //************** Check Sub_Catagory ******************//
                            if (!string.IsNullOrEmpty(dr["SubCatagory"].ToString()))
                            {

                                if (dr["SubCatagory"].ToString().ToUpper().Equals("NONE"))
                                {
                                    SubCatagoryID = 465;
                                }
                                else
                                {
                                    command1.CommandText = @"select ID from SubCategory WHERE UPPER(Name)=UPPER('" +
                                                           dr["SubCatagory"].ToString() + "')";
                                    SubCatagoryID = Convert.ToInt32(command1.ExecuteScalar());
                                    if (SubCatagoryID == 0)
                                    {
                                        command.CommandText =
                                            @"INSERT INTO [SubCategory] (Code,Name,Description,CategoryID,Active,CreatedBy,CreatedDate)
                               VALUES ('" + dr["SubCategoryCode"].ToString() + "','" + dr["SubCatagory"].ToString() +
                                            "','" + dr["subCatagoryDescription"].ToString() + "','" + catagoryID +
                                            "','True','" + LoginBy + "',GetDate())";
                                        command.ExecuteNonQuery();

                                        command1.CommandText = "select top(1)ID from SubCategory order by ID desc";
                                        SubCatagoryID = Convert.ToInt32(command1.ExecuteScalar());
                                    }
                                }
                            }




                            SizeID = 0;
                            //************** Check Size ******************//
                            if (!dr["ItemSize"].ToString().ToUpper().Equals(""))
                            {

                                command1.CommandText = @"select ID from SizeInfo  WHERE UPPER(SizeName)=UPPER('" +
                                                       dr["ItemSize"].ToString() + "')";
                                SizeID = Convert.ToInt32(command1.ExecuteScalar());
                                if (SizeID == 0)
                                {
                                    command.CommandText = @"INSERT INTO [SizeInfo ] (SizeName)
                               VALUES ('" + dr["ItemSize"].ToString() + "')";
                                    command.ExecuteNonQuery();

                                    command1.CommandText = "select top(1)ID from SizeInfo order by ID desc";
                                    SizeID = Convert.ToInt32(command1.ExecuteScalar());
                                }
                            }


                            //************** Check Color ******************//
                            ColorID = 0;
                            if (!dr["ItemColor"].ToString().ToUpper().Equals(""))
                            {

                                command1.CommandText = @"select ID from ColorInfo WHERE UPPER(ColorName)=UPPER('" +
                                                       dr["ItemColor"].ToString() + "')";
                                ColorID = Convert.ToInt32(command1.ExecuteScalar());
                                if (ColorID == 0)
                                {
                                    command.CommandText = @"INSERT INTO [ColorInfo] (ColorName)
                               VALUES ('" + dr["ItemColor"].ToString() + "')";
                                    command.ExecuteNonQuery();

                                    command1.CommandText = "select top(1)ID from ColorInfo order by ID desc";
                                    ColorID = Convert.ToInt32(command1.ExecuteScalar());
                                }
                            }

                            //************** Check Item SetUp ******************//
                            ItemSetUpId = 0;
                            if (!string.IsNullOrEmpty(dr["ItmsName"].ToString()))
                            {
                                command1.CommandText = @"select ID from ItemSetup WHERE UPPER(Name)=UPPER('" +
                                                       dr["ItmsName"].ToString() + "')";
                                ItemSetUpId = Convert.ToInt32(command1.ExecuteScalar());
                                if (ItemSetUpId == 0)
                                {
                                    command.CommandText =
                                        @"INSERT INTO [ItemSetup] (Code,Name,ShortName,Active,AddBy,AddDate)
                               VALUES ('" + dr["ItmsCode"].ToString() + "','" + dr["ItmsName"].ToString() + "','" +
                                        dr["ItmsShortName"].ToString() + "','True','" + LoginBy + "',GetDate())";
                                    command.ExecuteNonQuery();

                                    command1.CommandText = "select top(1)ID from ItemSetup order by ID desc";
                                    ItemSetUpId = Convert.ToInt32(command1.ExecuteScalar());
                                }
                            }




                            //************** Check Item  ******************//
                            ItemId = 0;
                            if (!string.IsNullOrEmpty(dr["ItemCode"].ToString()))
                            {

                                command.CommandText =
                                    @"INSERT INTO [Item] (Code,Name,ItemSize,ItemColor,UOMID,UnitPrice,Currency,OpeningStock,OpeningAmount,ClosingStock,ClosingAmount
,CategoryID,SubCategoryID,TaxCategoryID,Discounted,DiscountAmount,Active,IsNew,CreatedBy,CreatedDate,description,Brand,ShortName,StyleNo,DeptID,SizeID,ItemSetupID )
                               VALUES ('" + dr["ItemCode"].ToString() + "','" + dr["ItemName"].ToString() + "','0','" +
                                    ColorID + "','" + UMO_ID + "','" + dr["UnitPrice"].ToString() + "','" +
                                    dr["Currency"].ToString() + "','" + dr["OpeningStock"].ToString() + "','" +
                                    dr["OpeningAmount"].ToString() + "','" + dr["ClosingStock"].ToString() + "','" +
                                    dr["ClosingAmount"].ToString() + "','" + catagoryID + "','" + SubCatagoryID +
                                    "','" + Tax_ID + "','" + dr["Discounted"].ToString() + "','" +
                                    dr["DiscountAmount"].ToString() + "','" + dr["ItemActive"].ToString() + "','" +
                                    dr["IsNew"].ToString() + "','" + LoginBy + "',GetDate(),'" +
                                    dr["Itemdescription"].ToString() + "','" + BrandId + "','" +
                                    dr["ShortName"].ToString() + "','" + dr["StyleNo"].ToString() + "','" +
                                    Dept_ID + "','" + SizeID + "','" + ItemSetUpId + "')";
                                command.ExecuteNonQuery();

                                command1.CommandText = "select top(1)ID from Item order by ID desc";
                                ItemId = Convert.ToInt32(command1.ExecuteScalar());


                            }



                            //Item  Close

                            command1.CommandText = @"INSERT INTO [ItemStockreceivedDtl]
                                   (MstId,DtlID,ItemId,TransferQuantity,TransferPrice,BranchSalesPrice,receivedQuantity,Code,StyleNo,BranchId,Discount,Barcode )
                                    VALUES
                                   ('" + dr["MstID"].ToString() + "','" + dr["ID"].ToString() + "','" + ItemId +
                                                   "','" + Convert.ToDecimal(dr["TransferQuantity"].ToString()) + "','" +
                                                   dr["TransferPrice"].ToString() + "','" +
                                                   dr["BranchSalesPrice"].ToString() + "','0','" +
                                                   dr["Code"].ToString() + "','" + dr["StyleNo"].ToString() + "','" +
                                                   dr["BranchId"].ToString() + "','" + dr["Discount"].ToString() +
                                                   "','" + dr["Barcode"].ToString() + "')";
                            command1.ExecuteNonQuery();


                            ClosingStock = Convert.ToDecimal(dr["TransferQuantity"].ToString());
                            SalesPrice = Convert.ToDecimal(dr["BranchSalesPrice"].ToString());
                            ClosingAmount = ClosingStock * SalesPrice;



                            if (!string.IsNullOrEmpty(dr["Barcode"].ToString()))
                            {
                                string ExpireDateFiled = "", ExpireDateValue = "";
                                string ExDate = dr["ExpireDate"].ToString();
                                var a = dr["ExpireDate"].ToString();
                                if (!string.IsNullOrEmpty(ExDate))
                                {
                                    ExpireDateFiled = ",ExpireDate";
                               //ExpireDateValue = ",convert(Datetime,'" + ExDate + "',103)";
                                  ExpireDateValue = ",'" + ExDate + "'";

                                }

                                command.CommandText =
                                  @"INSERT INTO [OutLetItemStock] (BranchID,ItemID,ClosingStock,ClosingAmount,ItemsPrice,AddBy,AddDate,CostPrice,Code,Barcode,ItemCode " + ExpireDateFiled + " ) " +
                                  "VALUES ('" + dr["BranchId"].ToString() + "','" + ItemId + "','0','0','" + dr["BranchSalesPrice"].ToString() + "','" +
                                  LoginBy + "',GetDate(),'" + dr["CostPrice"].ToString() + "','" +
                                  dr["Code"].ToString() + "','" +
                                  dr["Barcode"].ToString() + "','" +
                                  dr["stItemCode"].ToString() + "' " + ExpireDateValue + ")";
                                command.ExecuteNonQuery();




                            }



                        }
                        //Item Code Else
                        else
                        {

                            command1.CommandText = @"INSERT INTO [ItemStockreceivedDtl]
                                   (MstId,DtlID,ItemId,TransferQuantity,TransferPrice,BranchSalesPrice,receivedQuantity,Code,StyleNo,BranchId,Discount,Barcode )
                                    VALUES
                                   ('" + dr["MstID"].ToString() + "','" + dr["ID"].ToString() + "','" + ItemId +
                                                   "','" + Convert.ToInt32(dr["TransferQuantity"].ToString()) + "','" +
                                                   dr["TransferPrice"].ToString() + "','" +
                                                   dr["BranchSalesPrice"].ToString() + "','0','" +
                                                   dr["Code"].ToString() + "','" + dr["StyleNo"].ToString() + "','" +
                                                   dr["BranchId"].ToString() + "','" + dr["Discount"].ToString() +
                                                   "','" + dr["Barcode"].ToString() + "')";
                            command1.ExecuteNonQuery();



                            ClosingStock = Convert.ToDecimal(dr["TransferQuantity"].ToString());
                            SalesPrice = Convert.ToDecimal(dr["BranchSalesPrice"].ToString());
                            ClosingAmount = ClosingStock * SalesPrice;



                            if (!string.IsNullOrEmpty(dr["Barcode"].ToString()))
                            {
                                string ExpireDateFiled = "", ExpireDateValue="";
                                string ExDate = dr["ExpireDate"].ToString();
                                var a = dr["ExpireDate"].ToString();
                                if (!string.IsNullOrEmpty(ExDate))
                                {
                                    ExpireDateFiled = ",ExpireDate";
                             // ExpireDateValue = ",convert(Datetime,'" + ExDate + "',103)";
                              ExpireDateValue = ",'" + ExDate + "'";

                                }
                               
                            
                                command.CommandText =
                                    @"INSERT INTO [OutLetItemStock] (BranchID,ItemID,ClosingStock,ClosingAmount,ItemsPrice,AddBy,AddDate,CostPrice,Code,Barcode,ItemCode "+ExpireDateFiled+" ) " +
                                    "VALUES ('" +dr["BranchId"].ToString()+"','" + ItemId + "','0','0','" + dr["BranchSalesPrice"].ToString() + "','" +
                                    LoginBy + "',GetDate(),'" + dr["CostPrice"].ToString() + "','" +
                                    dr["Code"].ToString() + "','" +
                                    dr["Barcode"].ToString() + "','" +
                                    dr["stItemCode"].ToString() + "' " + ExpireDateValue + ")";
                                command.ExecuteNonQuery();



                            }

                           


                        }
                    }

                    //Barcode Exist Else
                    else
                        {
                            command1.CommandText = @"INSERT INTO [ItemStockreceivedDtl]
                                   (MstId,DtlID,ItemId,TransferQuantity,TransferPrice,BranchSalesPrice,receivedQuantity,Code,StyleNo,BranchId,Discount,Barcode )
                                    VALUES
                                   ('" + dr["MstID"].ToString() + "','" + dr["ID"].ToString() + "','" + ItemId +
                                                   "','" + Convert.ToDecimal(dr["TransferQuantity"].ToString()) + "','" +
                                                   dr["TransferPrice"].ToString() + "','" +
                                                   dr["BranchSalesPrice"].ToString() + "','0','" +
                                                   dr["Code"].ToString() + "','" + dr["StyleNo"].ToString() + "','" +
                                                   dr["BranchId"].ToString() + "','" + dr["Discount"].ToString() +
                                                   "','" + dr["Barcode"].ToString() + "')";
                            command1.ExecuteNonQuery();

                            //var TransferQuantity = Convert.ToDecimal(dr["TransferQuantity"].ToString());
                            //var Code = dr["ItemCode"].ToString();
                            //var barcode = dr["Barcode"].ToString();
                            // SalesPrice = Convert.ToDecimal(dr["BranchSalesPrice"].ToString());
                            //command.CommandText = @"select ISNULL(ClosingStock,0) as ClosingStockfrom from  ItemStock  WHERE  Id='" + BarcodeExistId + "' and ItemCode='" + Code + "' AND Barcode='" + barcode + "'";
                            // ClosingStock = Convert.ToDecimal(command.ExecuteScalar());

                            // command.CommandText = @" update BranchItemStock set ClosingStock='" + TransferQuantity + ClosingStock + "',ClosingAmount='" + (TransferQuantity + ClosingStock) * SalesPrice + "'WHERE  Id='" + BarcodeExistId + "' and ItemCode='" + Code + "' AND Barcode='" + barcode + "'";
                            //command.ExecuteNonQuery();

                        }

                        //barcode Close

                        


                    }


                    //flag closing--



                  
                }
              foreach (DataRow drTransfer in dtTransferMst.Rows)
                    {
                        command1.CommandText = "select Count(*) from ItemStockReceivedMst Where MStID ='" +
                                               drTransfer["ID"].ToString() + "'";
                        int Flag = Convert.ToInt32(command1.ExecuteScalar());

                        if (Flag <= 0)
                        {
                            DateTime receivedDatetime = DataManager.DateEncode(drTransfer["TransferDate"].ToString());
                            string Date = receivedDatetime.Year + "-" + receivedDatetime.Month + "-" +
                                          receivedDatetime.Day + " " + DateTime.Now.ToString("HH:mm:ss");
                            command.CommandText = @"INSERT INTO [ItemStockReceivedMst]
                (BranchId,TransferDate,Remark,AddBy,AddDate,Code,ChallanNo,MStID)
                  VALUES
                ('" + drTransfer["BranchID"].ToString() + "',CONVERT(datetime,'" +
                                                  drTransfer["TransferDate"].ToString() + "',103),'" +
                                                  drTransfer["Remark"].ToString() + "','" + LoginBy +
                                                  "',GETDATE(),'" + drTransfer["Code"].ToString() +
                                                  "','" + drTransfer["ChallanNo"].ToString() + "','" + drTransfer["ID"].ToString() + "')";
                            command.ExecuteNonQuery();
                            //command1.CommandText = @" update " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[ItemStockTransferMst] set LocalUpload=1,[ReceivedBy]=0   WHERE ID= '" + drTransfer["ID"].ToString() + "'";
                            //command1.ExecuteNonQuery();



                            //string query = @"update " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[ItemStockTransferMst] set LocalUpload=1   WHERE ID= '" + drTransfer["ID"].ToString() + "' ";
                            //DataManager.ExecuteNonQuery(DataManager.OraConnString(), query);




                        }
                    }

                transaction.Commit();
            connection.Close();
        }
        catch
        {
            transaction.Rollback();
            connection.Close();
            throw new Exception("System Error ..!!");
        }
    }


   public static int SaveRecivedStockDtl(DataTable data,string Id,string loginBy)
   {
       int value = 0;
       SqlConnection connection = new SqlConnection(DataManager.OraConnString());
       SqlTransaction transaction;
       connection.Open();
       transaction = connection.BeginTransaction();
       try
       {

           SqlCommand command = new SqlCommand();
           command.Connection = connection;
           command.Transaction = transaction;
        

           command.CommandText = "Update  ItemStockreceivedMst set LocalUpload='1',ReceivedBy='" + loginBy + "',ReceivedDate=GetDate() where MstID='" + Id + "'";
           command.ExecuteNonQuery();

           command.CommandText = "Update ItemStockreceivedDtl set ReceivedQuantity=TransferQuantity where MstID='" + Id + "'";
         value=  command.ExecuteNonQuery();

           transaction.Commit();
           connection.Close();
       }
       catch 
       {

           transaction.Rollback();
           connection.Close();
           throw new Exception("System Error ..!!");
       }
       return value;
   }





















    public static DataTable GetItemQuantity(string ItemId)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"select ID,ItemsID,Quantity from(select t2.ID,ItemsID,Max(Quantity) Quantity,Name from ItemSalesStock t1 
inner join Item t2 on t1.ItemsID=t2.ID
 where Upper(convert(nvarchar,ItemsID)+ ' - '+t2.Code+' - '+t2.Name)=Upper('" + ItemId + "') group by t2.ID,ItemsID,Name) x";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemSalesStock");
        return dt;
    }

    public DataTable GetBranchInfo()
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT  t1.Code,t1.[ID],t1.[BranchID],t2.BranchName,convert(nvarchar,t1.[TransferDate],103)TransferDate,t1.[Remark] ,tot.Qty  ,tot.Total  AS Total ,case when t1.ReceivedBy IS NULL then 'Not Received' else 'Received' end AS Received
  FROM [ItemStockTransferMst] t1  INNER JOIN BranchInfo t2 on t2.ID=t1.BranchID 
  inner join (select MstID,SUM(ISNULL(TransferQuantity,0))AS Qty, convert(decimal(18,2),SUM(ISNULL(TransferPrice,0)*ISNULL(TransferQuantity,0))) AS[Total] from ItemStockTransferDtl WHERE DeleteBy IS NULL GROUP BY MstID) tot on tot.MstID=t1.ID WHERE DeleteBy IS NULL ORDER BY t1.ID DESC";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransfer");
        return dt;
    }

    public clsItemTransferStock GetStockTransferInfo(string TransferId)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query =
            @"SELECT [ID],[BranchID],convert(nvarchar,[TransferDate],103)TransferDate,[Remark],TransferType,Code FROM [ItemStockTransferMst] where ID='" +
            TransferId + "' and DeleteBy IS NULL";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransfer");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new clsItemTransferStock(dt.Rows[0]);
    }

   
        

    public int SaveItemsTransferInformation(clsItemTransferStock aclsItemTransferStock, DataTable dt, string CurrencyRate, string UserType, VouchMst vmst,string BranchCoa)
    {
        int MstID = 0;
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        DataTable dtBranchFixCode = VouchManager.GetAllFixGlCode(aclsItemTransferStock.BranchId);
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"INSERT INTO [ItemStockTransferMst]
           ([BranchID],[TransferDate],[Remark],[AddBy],[AddDate],RemarkNote,Code)
     VALUES
           ('" + aclsItemTransferStock.BranchId + "',CONVERT(DATE,'" + aclsItemTransferStock.TransferDate + "',103),'" +
                                  aclsItemTransferStock.Remark + "','" + aclsItemTransferStock.LoginBy + "',GETDATE(),'" +
                                  aclsItemTransferStock.RemarkNote + "','" + aclsItemTransferStock.Code + "')";
            command.ExecuteNonQuery();
            command.CommandText = @"select top(1)ID from [ItemStockTransferMst] ORDER BY ID DESC";
            MstID = Convert.ToInt32(command.ExecuteScalar());
            foreach (DataRow dr in dt.Rows)
            {
                if (!string.IsNullOrEmpty(dr["item_code"].ToString()))
                {
                    command.CommandText = @"INSERT INTO [ItemStockTransferDtl]
                       ([MstID],[Type],[ItemId],[TransferQuantity],TransferPrice,BranchSalesPrice)
                            VALUES
                       ("   + MstID + ",'" + dr["Type"].ToString() + "','" + dr["ItemsID"].ToString() + "','" +
                                          dr["TransferQty"].ToString() + "','" + dr["Price"].ToString() + "','" +
                                          dr["BranchSalesPrice"].ToString() + "')";
                    command.ExecuteNonQuery();

                    command.CommandText = @"Select COUNT(*) from ItemSalesStockBranchWise WHERE ItemsID='" +
                                          dr["ItemsID"].ToString() + "' AND BranchID='" + aclsItemTransferStock.BranchId +
                                          "' ";
                    int Chk = Convert.ToInt32(command.ExecuteScalar());
                    if (Chk > 0)
                    {
                        command.CommandText = @"UPDATE [ItemSalesStockBranchWise]
                        SET [Quantity] =ISNULL([Quantity],0)+" + Convert.ToDouble(dr["TransferQty"].ToString()) + " ,[UpdateBy] ='" + aclsItemTransferStock.LoginBy +
                                              "' ,[UpdateDate] =GETDATE()  WHERE [BranchID]='" +
                                              aclsItemTransferStock.BranchId + "' AND [ItemsID]='" +
                                              dr["ItemsID"].ToString() + "'";
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        command.CommandText = @"INSERT INTO [ItemSalesStockBranchWise]
                           ([BranchID],[ItemsID] ,[Quantity] ,[UpdateBy] ,[UpdateDate])
                             VALUES
                           ('" + aclsItemTransferStock.BranchId + "','" + dr["ItemsID"].ToString() + "','" +
                                              dr["TransferQty"].ToString() + "','" + aclsItemTransferStock.LoginBy +
                                              "',GETDATE())";
                        command.ExecuteNonQuery();
                    }

                    command.CommandText = @"UPDATE [ItemSalesStock]
                         SET [Quantity] =ISNULL([Quantity],0)-" + Convert.ToDouble(dr["TransferQty"].ToString()) + " WHERE [Flag]='" +
                                          dr["Type"].ToString() + "' AND ID='" + dr["ItemsID"].ToString() + "' ";
                    command.ExecuteNonQuery();
                }
            }
            //********************* Transfer Total (Show Purchase Price) *********//

            command.CommandText = "SP_PV_UnitPrice_All_Transfer";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@MstID", Convert.ToInt32(MstID));
            command.Parameters.AddWithValue("@Rate", Convert.ToDecimal(CurrencyRate));
            double PurchasePrice = Convert.ToDouble(command.ExecuteScalar());

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
                    vdtl.ValueDate = aclsItemTransferStock.TransferDate;
                    vdtl.LineNo = "1";
                    vdtl.GlCoaCode = "1-" + dtBranchFixCode.Rows[0]["Gl_CoaCode"].ToString();
                    vdtl.Particulars = dtBranchFixCode.Rows[0]["Gl_CoaDesc"].ToString();
                    vdtl.AccType = VouchManager.getAccType("1-" + dtBranchFixCode.Rows[0]["Gl_CoaCode"].ToString());
                    vdtl.AmountDr = Convert.ToDouble(aclsItemTransferStock.TotalAmount).ToString().Replace(",", "");
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
                    vdtl.ValueDate = aclsItemTransferStock.TransferDate;
                    vdtl.LineNo = "2";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["MainOfficeTransfer"].ToString(); //**** Gl_Main_OfficeCode *******//
                    vdtl.Particulars = dtFixCode.Rows[0]["MainOfficeTransferDesc"].ToString();
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["MainOfficeTransfer"].ToString()); //**** Gl_Main_OfficeCode *******//
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = Convert.ToDouble(aclsItemTransferStock.TotalAmount).ToString().Replace(",", "");
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                }
                else if (j == 2)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = vmst.VchSysNo;
                    vdtl.ValueDate = aclsItemTransferStock.TransferDate;
                    vdtl.LineNo = "3";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString(); ;
                    vdtl.Particulars = "Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = PurchasePrice.ToString().Replace(",", "");
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
                    vdtl.ValueDate = aclsItemTransferStock.TransferDate;
                    vdtl.LineNo = "4";
                    vdtl.GlCoaCode = dtBranchFixCode.Rows[0]["BR_Closing_Stock"].ToString();
                    vdtl.Particulars = dtBranchFixCode.Rows[0]["BR_Closing_Stock_Des"].ToString();
                    vdtl.AccType = VouchManager.getAccType(dtBranchFixCode.Rows[0]["BR_Closing_Stock"].ToString());
                    vdtl.AmountDr = Convert.ToDouble(aclsItemTransferStock.TotalAmount).ToString().Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
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
        return MstID;
    }

    public void UpdateItemsTransferInformation(clsItemTransferStock aclsItemTransferStock, DataTable dt, DataTable OldDt, string CurrencyRate, string UserType, VouchMst vmst, string BranchCoa)
    {
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        DataTable dtBranchFixCode = VouchManager.GetAllFixGlCode(aclsItemTransferStock.BranchId);
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"UPDATE [ItemStockTransferMst]
             SET TransferDate=CONVERT(DATE,'" + aclsItemTransferStock.TransferDate + "',103),[UpdateBy] = '" +
                                  aclsItemTransferStock.LoginBy + "',[UpdateDate] = GetDate(),Remark='" +
                                  aclsItemTransferStock.Remark + "',RemarkNote='" + aclsItemTransferStock.RemarkNote +
                                  "' WHERE ID='" + aclsItemTransferStock.ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"delete from [ItemStockTransferDtl]  WHERE [MstID]='" +
                                  aclsItemTransferStock.ID + "'";
            command.ExecuteNonQuery();

            //********************** Old List *******************//

            foreach (DataRow row in OldDt.Rows)
            {
                if (!string.IsNullOrEmpty(row["item_code"].ToString()))
                {
                    command.CommandText = @"UPDATE [ItemSalesStockBranchWise]
                    SET [Quantity] =ISNULL([Quantity],0)-" +Convert.ToDouble(row["TransferQty"].ToString()) + " ,[UpdateBy] ='" + aclsItemTransferStock.LoginBy +
                                          "' ,[UpdateDate] =GETDATE()  WHERE [BranchID]='" +
                                          aclsItemTransferStock.BranchId + "' AND [ItemsID]='" +
                                          row["ItemsID"].ToString() + "'";
                        command.ExecuteNonQuery();


                    command.CommandText = @"UPDATE [ItemSalesStock]
                         SET [Quantity] =ISNULL([Quantity],0)+" + Convert.ToDouble(row["TransferQty"].ToString()) + "" +
                                          ",Price=isnull(Price,0)+" +
                                          Convert.ToDouble(row["PurchasePrice"].ToString().Replace("'", "")) +
                                          ",UpdateBy='" +
                                          aclsItemTransferStock.LoginBy + "',UpdateDate=GETDATE() WHERE [Flag]='" +
                                          row["Type"].ToString() + "' AND ID='" + row["ItemsID"].ToString() + "'";
                    command.ExecuteNonQuery();

                    if (row["Type"].ToString().Equals("1"))
                    {
                        command.CommandText = @"UPDATE Item
				     	 SET SalesClosingQty= (ISNULL(SalesClosingQty,0)-convert(decimal," +
                                              row["TransferQty"].ToString().Replace(",", "") +
                                              "))  WHERE ID = " + row["ItemsID"].ToString();
                        command.ExecuteNonQuery();
                    }
                    else if (row["Type"].ToString().Equals("2"))
                    {
                        command.CommandText = @"UPDATE ItemSalesStock
					    SET SalesClosingQty= (ISNULL(SalesClosingQty,0)-convert(decimal," +
                                              row["TransferQty"].ToString().Replace(",", "") + "))	WHERE ID = " +
                                              row["ID"].ToString();
                        command.ExecuteNonQuery();
                    }

                }
            }

            //********************** New List *******************//
            foreach (DataRow dr in dt.Rows)
            {
                if (!string.IsNullOrEmpty(dr["item_code"].ToString()))
                {
                    command.CommandText = @"INSERT INTO [ItemStockTransferDtl]
           ([MstID],[Type],[ItemId],[TransferQuantity],TransferPrice,BranchSalesPrice)
     VALUES
           (" + aclsItemTransferStock.ID + ",'" + dr["Type"].ToString() + "','" + dr["ItemsID"].ToString() + "','" +
                                          dr["TransferQty"].ToString() + "','" + dr["Price"].ToString() + "','" +
                                          dr["BranchSalesPrice"].ToString() + "')";
                    command.ExecuteNonQuery();

                    command.CommandText = @"Select COUNT(*) from ItemSalesStockBranchWise WHERE ItemsID='" +
                                          dr["ItemsID"].ToString() + "' AND BranchID='" + aclsItemTransferStock.BranchId +
                                          "' ";
                    int Chk = Convert.ToInt32(command.ExecuteScalar());
                    if (Chk > 0)
                    {
                        command.CommandText = @"UPDATE [ItemSalesStockBranchWise]
                         SET [Quantity] =ISNULL([Quantity],0)+" + Convert.ToDouble(dr["TransferQty"].ToString()) +
                                              " ,[UpdateBy] ='" + aclsItemTransferStock.LoginBy +
                                              "' ,[UpdateDate] =GETDATE()  WHERE [BranchID]='" +
                                              aclsItemTransferStock.BranchId + "' AND [ItemsID]='" +
                                              dr["ItemsID"].ToString() + "'";
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        command.CommandText = @"INSERT INTO [ItemSalesStockBranchWise]
           ([BranchID],[ItemsID] ,[Quantity] ,[UpdateBy] ,[UpdateDate])
     VALUES
           ('" + aclsItemTransferStock.BranchId + "','" + dr["ItemsID"].ToString() + "','" +
                                              dr["TransferQty"].ToString() + "','" + aclsItemTransferStock.LoginBy +
                                              "',GETDATE())";
                        command.ExecuteNonQuery();
                    }

                    command.CommandText = @"UPDATE [ItemSalesStock]
                         SET [Quantity] =ISNULL([Quantity],0)-" + Convert.ToDouble(dr["TransferQty"].ToString()) +
                                          "  WHERE [Flag]='" +
                                          dr["Type"].ToString() + "' AND ID='" + dr["ItemsID"].ToString() + "'";
                    command.ExecuteNonQuery();

                }
            }
            //********************* Transfer Total (Show Purchase Price) *********//

            command.CommandText = "SP_PV_UnitPrice_All_Transfer";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@MstID", Convert.ToInt32(aclsItemTransferStock.ID));
            command.Parameters.AddWithValue("@Rate", Convert.ToDecimal(CurrencyRate));
            double PurchasePrice = Convert.ToDouble(command.ExecuteScalar());

            //******************* Update Journal Voucher **********//

            //******* Vucher One ******//

            command.CommandType = CommandType.Text;
            command.CommandText = VouchManager.SaveVoucherMst(vmst, 2);
            command.ExecuteNonQuery();
            command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" + vmst.VchSysNo + "')";
            command.ExecuteNonQuery();

            VouchDtl vdtl;
            for (int j = 0; j < 4; j++)
            {
                if (j == 0)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = vmst.VchSysNo;
                    vdtl.ValueDate = aclsItemTransferStock.TransferDate;
                    vdtl.LineNo = "1";
                    vdtl.GlCoaCode = "1-" + dtBranchFixCode.Rows[0]["Gl_CoaCode"].ToString();
                    vdtl.Particulars = dtBranchFixCode.Rows[0]["Gl_CoaDesc"].ToString();
                    vdtl.AccType = VouchManager.getAccType("1-" + dtBranchFixCode.Rows[0]["Gl_CoaCode"].ToString());
                    vdtl.AmountDr = Convert.ToDouble(aclsItemTransferStock.TotalAmount).ToString().Replace(",", "");
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
                    vdtl.ValueDate = aclsItemTransferStock.TransferDate;
                    vdtl.LineNo = "2";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["MainOfficeTransfer"].ToString(); //**** Gl_Main_OfficeCode *******//
                    vdtl.Particulars = dtFixCode.Rows[0]["MainOfficeTransferDesc"].ToString();
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["MainOfficeTransfer"].ToString()); //**** Gl_Main_OfficeCode *******//
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = Convert.ToDouble(aclsItemTransferStock.TotalAmount).ToString().Replace(",", "");
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                }
                else if (j == 2)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = vmst.VchSysNo;
                    vdtl.ValueDate = aclsItemTransferStock.TransferDate;
                    vdtl.LineNo = "3";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString(); ;
                    vdtl.Particulars = "Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"].ToString());
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = PurchasePrice.ToString().Replace(",", "");
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
                    vdtl.ValueDate = aclsItemTransferStock.TransferDate;
                    vdtl.LineNo = "4";
                    vdtl.GlCoaCode = dtBranchFixCode.Rows[0]["BR_Closing_Stock"].ToString();
                    vdtl.Particulars = dtBranchFixCode.Rows[0]["BR_Closing_Stock_Des"].ToString();
                    vdtl.AccType = VouchManager.getAccType(dtBranchFixCode.Rows[0]["BR_Closing_Stock"].ToString());
                    vdtl.AmountDr = Convert.ToDouble(aclsItemTransferStock.TotalAmount).ToString().Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                }
            }
            transaction.Commit();
            //return PurchasePrice;
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

    public void DeleteInfo(clsItemTransferStock aclsItemTransferStock, DataTable dtOld)
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

            //******************* Auto voucher delete off  ******************//

            //********************* Jurnal - 1 delete ***********//

            command.CommandText = @"select t1.VCH_SYS_NO  from [GL_TRANS_MST] t1 where t1.SERIAL_NO='" + aclsItemTransferStock.Code + "' and t1.PAYEE='IT' ";
            int VCH_SYS_NO = Convert.ToInt32(command.ExecuteScalar());
            command.CommandText = @"DELETE FROM [GL_TRANS_MST]  WHERE VCH_SYS_NO='" + VCH_SYS_NO + "'";
            command.ExecuteNonQuery();
            command.CommandText = @"DELETE FROM [GL_TRANS_DTL]  WHERE VCH_SYS_NO='" + VCH_SYS_NO + "'";
            command.ExecuteNonQuery();

            

            command.CommandText = @"UPDATE [ItemStockTransferDtl]
              SET  [DeleteBy] ='" + aclsItemTransferStock.LoginBy + "' ,[DeleteDate] =GETDATE() WHERE [MstID]='" +
                                  aclsItemTransferStock.ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE [ItemStockTransferMst]
                   SET [DeleteBy] = '" + aclsItemTransferStock.LoginBy + "',[DeleteDate] = GetDate() WHERE ID='" +
                                  aclsItemTransferStock.ID + "'";
            command.ExecuteNonQuery();

            //********************** Old List *******************//

            foreach (DataRow row in dtOld.Rows)
            {
                if (!string.IsNullOrEmpty(row["item_code"].ToString()))
                {
                    command.CommandText = @"UPDATE [ItemSalesStockBranchWise]
                    SET [Quantity] =ISNULL([Quantity],0)-" +Convert.ToDouble(row["TransferQty"].ToString()) + " ,[UpdateBy] ='" + aclsItemTransferStock.LoginBy +
                                          "' ,[UpdateDate] =GETDATE()  WHERE [BranchID]='" +
                                          aclsItemTransferStock.BranchId + "' AND [ItemsID]='" +
                                          row["ItemsID"].ToString() + "'";
                        command.ExecuteNonQuery();


                    command.CommandText = @"UPDATE [ItemSalesStock]
                         SET [Quantity] =isnull([Quantity],0)+" +
                                          Convert.ToDouble(row["TransferQty"].ToString().Replace("'", "")) +
                                          ",Price=isnull(Price,0)+" +
                                          Convert.ToDouble(row["PurchasePrice"].ToString().Replace("'", "")) +
                                          ",UpdateBy='" + aclsItemTransferStock.LoginBy +
                                          "',UpdateDate=GETDATE() WHERE [Flag]='" + row["Type"].ToString() +
                                          "' AND ID='" + row["ItemsID"].ToString() + "'";
                    command.ExecuteNonQuery();

                    if (row["Type"].ToString().Equals("1"))
                    {
                        command.CommandText = @"UPDATE Item
				     	 SET SalesClosingQty= (ISNULL(SalesClosingQty,0)-convert(decimal," + row["TransferQty"].ToString().Replace(",", "") +
                                              "))  WHERE ID = " + row["ItemsID"].ToString();
                        command.ExecuteNonQuery();
                    }
                    else if (row["Type"].ToString().Equals("2"))
                    {
                        command.CommandText = @"UPDATE ItemSalesStock
					    SET SalesClosingQty= (ISNULL(SalesClosingQty,0)-convert(decimal," + row["TransferQty"].ToString().Replace(",", "") + "))	WHERE ID = " +
                                              row["ID"].ToString();
                        command.ExecuteNonQuery();
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

    public DataTable GetShowItemsDetails(string Parameter)
    {
        String connectionString = DataManager.OraConnString();
        string query =
            @"SELECT t1.[ID] 
      ,t3.Code AS[item_code]
	  ,t3.Name AS[item_desc]
      ,t1.[Type]
      ,convert(decimal(18,0),t2.Quantity) AS[StockQty]
      ,convert(decimal(18,0),t1.[TransferQuantity]) AS[TransferQty]  
      ,convert(decimal(18,2),ISNULL(t1.TransferPrice,0)) AS [Price]  
      ,convert(decimal(18,0),isnull(ReceivedQty,0)) AS ReceivedQty
 ,ISNULL(PvUnitPrice,0) AS[PurchasePrice]
,ISNULL(BranchSalesPrice,0) AS BranchSalesPrice
,t2.ID ItemsID
,convert(decimal(18,2),t1.[TransferQuantity]*ISNULL(t1.TransferPrice,0)) AS[TotalAmount]
  FROM [dbo].[ItemStockTransferDtl] t1 INNER JOIN ItemStockTransferMst tt on tt.ID=t1.MstID AND tt.DeleteBy IS NULL INNER JOIN ItemSalesStock t2 on t2.ID=t1.ItemId INNER JOIN Item t3 on t3.ID=t2.ItemsID " +
            Parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransfer");
        return dt;
    }

    public void UpdateBranchInfoExcelRecord(string MstID, string LoginBy)
    {
        string con = DataManager.OraConnString();
        string query = @"UPDATE [ItemStockTransferMst]
   SET [ExcelUser] = '" + LoginBy + "',[ExcelFlag] =1 WHERE ID='" + MstID + "'";
        DataManager.ExecuteNonQuery(con, query);
    }

    public static void GetUpdateStock(DataTable dt, string LoginBy,string BranchID,string Date,string Remarks)
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

            SqlCommand command1 = new SqlCommand();
            command1.Connection = connection;
            command1.Transaction = transaction;
            
            foreach (DataRow drr in dt.Rows)
            {
                string ID = "";
                string[] Code = null;
                if (!string.IsNullOrEmpty(drr["F1"].ToString().Trim()))
                {
                    if (!drr["F1"].ToString().Equals("Code"))
                    {
                        command.CommandText = @"INSERT INTO [ItemsBranchWiseSalesSummery]
           ([Code],[Name],[UnitPrice],[Quantity] ,[TaxRate] ,[DiscountAmount] ,[SalePrice] ,[TotalPrice],Addby,AddDate,Remarks,[Date],BranchID)
     VALUES
           ('" + drr["F1"].ToString() + "','" + drr["F2"].ToString() + "','" + drr["F3"].ToString() + "','" + drr["F4"].ToString() + "','" + drr["F5"].ToString() + "','" + drr["F6"].ToString() + "','" + drr["F7"].ToString() + "','" + drr["F8"].ToString() + "','" + LoginBy + "',GETDATE(),'" + Remarks.Replace("'", "") + "',convert(date,'" + Date + "',103),'" + BranchID + "')";
                        command.ExecuteNonQuery();
                         Code = drr["F1"].ToString().Split('-');
                        if (Code != null)
                        {
                            ID = Code[0].ToString().Trim();
                            command1.CommandText = @"Update ItemSalesStockBranchWise set [Quantity]=([Quantity]-" + Convert.ToDecimal(drr["F4"]) + ") ,[UpdateBy]='" + LoginBy + "',[UpdateDate]=GETDATE() where [BranchID]='" + BranchID + "' AND [ItemsID]='" + ID + "' ";
                            command1.ExecuteNonQuery();
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

    public DataTable GetShowTransferItemReport(string CurrentDate,string StartDate,string EndDate,
               string CustomerID,string rbReportType)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        sqlCon.Open();
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = new SqlCommand("SP_TransferItem", sqlCon);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.AddWithValue("@ID", null);
        if (!string.IsNullOrEmpty(CustomerID))
        {
            da.SelectCommand.Parameters.AddWithValue("@BranchID", CustomerID);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@BranchID", null);
        }
        if (!string.IsNullOrEmpty(StartDate))
        {
            da.SelectCommand.Parameters.AddWithValue("@FromDate", StartDate);
            da.SelectCommand.Parameters.AddWithValue("@ToDate", EndDate);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@FromDate", null);
            da.SelectCommand.Parameters.AddWithValue("@ToDate", null);
        }
        da.SelectCommand.CommandTimeout = 500;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_TransferItem");
        DataTable dt = ds.Tables["SP_TransferItem"];
        return dt;
    }

    public DataTable GetTransferHistoryForSearch(string Code, string BranchID, string FromDate, string ToDate)
    {
        String connectionString = DataManager.OraConnString();
        string Parameter = "";
        if (Code != "")
        {
            Parameter = Parameter + " And  t1.Code='" + Code + "' ";
        }
        else
        {
            if (BranchID != "" && BranchID != "0")
            {
                Parameter = Parameter + " And  t2.ID=" + BranchID + " ";
            }

            if (FromDate != "" && ToDate != "")
            {
                Parameter = Parameter + " And Convert(date,t1.[TransferDate],103) between Convert(date,'" + FromDate + "',103) AND Convert(date,'" + ToDate + "',103) ";
            }
        }
        string query = @"SELECT top(100) t1.[ID],t1.[BranchID],t2.BranchName,convert(nvarchar,t1.[TransferDate],103)TransferDate,t1.[Remark] ,tot.Qty,tot.Total  AS Total  ,t1.Code  ,CASE WHEN t1.ReceivedBy IS NULL then 'Not Received' else 'Received' end as [Received]
  FROM [ItemStockTransferMst] t1  INNER JOIN BranchInfo t2 on t2.ID=t1.BranchID 
  inner join (select MstID,SUM(ISNULL(TransferQuantity,0))AS Qty, convert(decimal(18,2),SUM(ISNULL(TransferPrice,0)*ISNULL(TransferQuantity,0))) AS[Total] from ItemStockTransferDtl WHERE DeleteBy IS NULL GROUP BY MstID) tot on tot.MstID=t1.ID  WHERE t1.DeleteBy IS NULL " + Parameter + " ORDER BY convert(date,t1.[TransferDate],103) Desc,t1.ID DESC";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransfer");
        return dt;
    }


    //***************** Items Stock Out **************//

    public void SaveItemsStockOutInformation(clsItemTransferStock aclsItemTransferStock, DataTable dtItemsDtl,
        VouchMst vmst, string CurrencyRate, string UserType)
    {
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        connection.Open();
        transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        command.CommandText = @"INSERT INTO [ItemStockOutMst]
           ([Date],[Remark],[AddBy],[AddDate],Code,StockType)
     VALUES
           (CONVERT(DATE,'" + aclsItemTransferStock.TransferDate + "',103),'" +
                              aclsItemTransferStock.Remark.Replace("'", "") + "','" + aclsItemTransferStock.LoginBy +
                              "',GETDATE(),'" + aclsItemTransferStock.Code + "','" + aclsItemTransferStock.StockType +
                              "')";
        command.ExecuteNonQuery();
        command.CommandText = @"update dbo.FixGlCoaCode set StockOutID=StockOutID+1 ";
        command.ExecuteNonQuery();

        command.CommandText = @"select top(1)ID from [ItemStockOutMst] ORDER BY ID DESC";
        int MstID = Convert.ToInt32(command.ExecuteScalar());
        foreach (DataRow dr in dtItemsDtl.Rows)
        {
            if (!string.IsNullOrEmpty(dr["item_code"].ToString()))
            {
                if (!string.IsNullOrEmpty(dr["OutQty"].ToString()))
                {
                    if (Convert.ToDouble(dr["OutQty"]) > 0)
                    {
                        command.CommandText = @"INSERT INTO [ItemStockOutDtl]
                       ([MstID],[ItemId],[Quantity],Price,Type)
                          VALUES
                       (" + MstID + ",'" + dr["ItemsID"].ToString() + "','" +
                                              dr["OutQty"].ToString().Replace(",", "") +
                                              "','0','" +
                                              dr["Type"].ToString().Replace(",", "") + "')";
                        command.ExecuteNonQuery();

                        command.CommandText = @"UPDATE [ItemSalesStock]
                             SET [Quantity] =ISNULL([Quantity],0)-" + Convert.ToDouble(dr["OutQty"].ToString()) +
                                              " WHERE [Flag]='" +
                                              dr["Type"].ToString() + "' AND ID='" + dr["ItemsID"].ToString() + "' ";
                        command.ExecuteNonQuery();

//                        if (dr["Type"].ToString().Equals("1"))
//                        {
//                            command.CommandText = @"UPDATE Item
//				     	 SET SalesClosingQty= (ISNULL(SalesClosingQty,0)+convert(decimal," +
//                                                  dr["OutQty"].ToString().Replace(",", "") +
//                                                  "))  WHERE ID = " + dr["ItemsID"].ToString();
//                            command.ExecuteNonQuery();
//                        }
//                        else if (dr["Type"].ToString().Equals("2"))
//                        {
//                            command.CommandText = @"UPDATE ItemSalesStock
//					    SET SalesClosingQty= (ISNULL(SalesClosingQty,0)+convert(decimal," +
//                                                  dr["OutQty"].ToString().Replace(",", "") + "))	WHERE ID = " +
//                                                  dr["ID"].ToString();
//                            command.ExecuteNonQuery();
//                        }
                    }
                }
            }
        }
        //********************* Transfer Total (Show Purchase Price) *********//

        command.CommandText = "SP_PV_UnitPrice_All_Dmage_Short";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@MstID", Convert.ToInt32(MstID));
        command.Parameters.AddWithValue("@Rate", Convert.ToDecimal(CurrencyRate));
        double PurchasePrice = Convert.ToDouble(command.ExecuteScalar());

        command.CommandType = CommandType.Text;
        command.CommandText = @"UPDATE [dbo].[ItemStockOutMst]
            SET [Total] ='" + PurchasePrice + "'  WHERE ID='" + MstID + "' ";
        command.ExecuteNonQuery();

        //***************************  Jurnal Voucher ********************************// 
        //******* Vucher One ******//
        vmst.ControlAmt = PurchasePrice.ToString();
        command.CommandText = VouchManager.SaveVoucherMst(vmst, 1);
        command.ExecuteNonQuery();
        VouchDtl vdtl;
        for (int j = 0; j < 2; j++)
        {
            if (j == 0)
            {
                vdtl = new VouchDtl();
                vdtl.VchSysNo = vmst.VchSysNo;
                vdtl.ValueDate = aclsItemTransferStock.TransferDate;
                vdtl.LineNo = "1";
                vdtl.GlCoaCode = dtFixCode.Rows[0]["PH_DamagerCoa"].ToString();
                vdtl.Particulars = dtFixCode.Rows[0]["PH_DamagerCoa_Name"].ToString();
                vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["PH_DamagerCoa"].ToString());
                vdtl.AmountDr = Convert.ToDouble(PurchasePrice).ToString().Replace(",", "");
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
                vdtl.ValueDate = aclsItemTransferStock.TransferDate;
                vdtl.LineNo = "2";
                vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString(); //**** Gl_Main_OfficeCode *******//
                vdtl.Particulars = "Closing Stock";
                vdtl.AccType =
                    VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"]
                        .ToString()); //**** Gl_Main_OfficeCode *******//
                vdtl.AmountDr = "0";
                vdtl.AmountCr = Convert.ToDouble(PurchasePrice).ToString().Replace(",", "");
                vdtl.Status = vmst.Status;
                vdtl.BookName = vmst.BookName;
                BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
            }
        }

        transaction.Commit();
        //transaction.Commit();
        connection.Close();
    }

    public DataTable GetShowItemsStockOutDetails(string Parameter)
    {
        String connectionString = DataManager.OraConnString();
        string query =
            @"SELECT tt2.[ID] AS[ItemsID]
      ,t3.Code AS[item_code]
      ,t3.StyleNo as StyleNo
	  ,t3.Name AS[item_desc]     
      ,tt2.Quantity AS[StockQty]
      ,isnull(t1.Quantity,0) AS[OutQty]  
      ,convert(decimal (18,2),ISNULL(t1.Price,0)) AS [Price]  
      ,t4.Name AS[Catagory]
      ,t5.Name AS[SubCatagory]
      ,t6.Name AS UMO 
      ,tt2.Flag  AS Type
      ,t3.ID AS[I_id]      
      FROM dbo.ItemStockOutDtl t1 
      INNER JOIN dbo.ItemStockOutMst tt on tt.ID=t1.MstID AND tt.DeleteBy IS NULL 
	  inner join ItemSalesStock tt2 on tt2.ID=t1.ItemId
      INNER JOIN Item t3 on t3.ID=tt2.ItemsID
      left join Category t4 on t4.ID=t3.CategoryID
      left join SubCategory t5 on t5.ID=t3.SubCategoryID
      left join UOM t6 on t6.ID=t3.UOMID " +
            Parameter + " Order By t1.ID ASC";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransfer");
        return dt;
    }

    public DataTable GetShowItemsStockOut(string ItemID)
    {
        String connectionString = DataManager.OraConnString();
        string Parameter = "";
        if (!string.IsNullOrEmpty(ItemID))
        {
            Parameter = " and t1.ID=" + ItemID;
        }
        string query =
            @"SELECT top(100) t1.[ID]      
      ,convert(nvarchar,t1.[Date],103) AS[StockOutDate]
      ,t1.[Remark]  
      ,t1.Code    
      ,t2.Qty AS[OutQuantity]
  FROM [ItemStockOutMst] t1
  inner join (select MstID,sum(Quantity) AS[Qty] from dbo.ItemStockOutDtl group by MstID) t2 on t2.MstID=t1.[ID] where DeleteBy IS NULL  " + Parameter + " Order by t1.[ID] desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransfer");
        return dt;
    }

    public void UpdateItemnStockInfo(clsItemTransferStock aclsItemTransferStock, DataTable dtItemsDtl, DataTable OldDt, VouchMst vmst, string CurrencyRate, string UserType)
    {
        //try
        //{
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        connection.Open();
        transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        command.CommandText = @"UPDATE [ItemStockOutMst]
             SET [UpdateBy] = '" + aclsItemTransferStock.LoginBy + "',[Date]=Convert(date,'" +
                              aclsItemTransferStock.TransferDate + "',103),[UpdateDate] = GetDate()" +
                              ",Remark='" + aclsItemTransferStock.Remark.Replace("'", "") + "' " +
                              " WHERE ID='" + aclsItemTransferStock.ID + "'";
        command.ExecuteNonQuery();

//        command.CommandText = @"UPDATE [ItemStockOutDtl]
//                SET  [DeleteBy] ='" + aclsItemTransferStock.LoginBy + "' ,[DeleteDate] =GETDATE() WHERE [MstID]='" +
//                              aclsItemTransferStock.ID + "'";
//        command.ExecuteNonQuery();

        command.CommandText = @"delete from [ItemStockOutDtl]  WHERE [MstID]='" +
                              aclsItemTransferStock.ID + "'";
        command.ExecuteNonQuery();

        //********************** Old List *******************//

        foreach (DataRow row in OldDt.Rows)
        {
            if (!string.IsNullOrEmpty(row["item_code"].ToString()))
            {
                command.CommandText = @"UPDATE [ItemSalesStock]
                         SET [Quantity] =ISNULL([Quantity],0)+" + Convert.ToDouble(row["OutQty"].ToString()) + "" +
                                      ",UpdateBy='" +
                                      aclsItemTransferStock.LoginBy + "',UpdateDate=GETDATE() WHERE [Flag]='" +
                                      row["Type"].ToString() + "' AND ID='" + row["ItemsID"].ToString() + "'";
                command.ExecuteNonQuery();

                if (row["Type"].ToString().Equals("1"))
                {
                    command.CommandText = @"UPDATE Item
				     	 SET SalesClosingQty= (ISNULL(SalesClosingQty,0)-convert(decimal," +
                                          row["OutQty"].ToString().Replace(",", "") +
                                          "))  WHERE ID = " + row["ItemsID"].ToString();
                    command.ExecuteNonQuery();
                }
                else if (row["Type"].ToString().Equals("2"))
                {
                    command.CommandText = @"UPDATE ItemSalesStock
					    SET SalesClosingQty= (ISNULL(SalesClosingQty,0)-convert(decimal," +
                                          row["OutQty"].ToString().Replace(",", "") + "))	WHERE ID = " +
                                          row["I_id"].ToString();
                    command.ExecuteNonQuery();
                }
            }
        }

        //********************** New List *******************//
        foreach (DataRow dr in dtItemsDtl.Rows)
        {
            if (!string.IsNullOrEmpty(dr["item_code"].ToString()))
            {
                if (!string.IsNullOrEmpty(dr["OutQty"].ToString()))
                {
                    if (Convert.ToDouble(dr["OutQty"]) > 0)
                    {
                        command.CommandText = @"INSERT INTO [ItemStockOutDtl]
                       ([MstID],[ItemId],[Quantity],Price,Type)
                          VALUES
                       (" + aclsItemTransferStock.ID + ",'" + dr["ItemsID"].ToString() + "','" +
                                              dr["OutQty"].ToString().Replace(",", "") +
                                              "','0','" +
                                              dr["Type"].ToString().Replace(",", "") + "')";
                        command.ExecuteNonQuery();

                        command.CommandText = @"UPDATE [ItemSalesStock]
                             SET [Quantity] =ISNULL([Quantity],0)-" + Convert.ToDouble(dr["OutQty"].ToString()) +
                                              " WHERE [Flag]='" +
                                              dr["Type"].ToString() + "' AND ID='" + dr["ItemsID"].ToString() + "' ";
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        //********************* Transfer Total (Show Purchase Price) **************//

        command.CommandText = "SP_PV_UnitPrice_All_Dmage_Short";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@MstID", Convert.ToInt32(aclsItemTransferStock.ID));
        command.Parameters.AddWithValue("@Rate", Convert.ToDecimal(CurrencyRate));
        double PurchasePrice = Convert.ToDouble(command.ExecuteScalar());

        command.CommandType = CommandType.Text;
        command.CommandText = @"UPDATE [dbo].[ItemStockOutMst]
            SET [Total] ='" + PurchasePrice + "'  WHERE ID='" + aclsItemTransferStock.ID + "' ";
        command.ExecuteNonQuery();

        //***************************  Jurnal Voucher ********************************// 
        //******* Vucher One ******//
        vmst.ControlAmt = PurchasePrice.ToString();
        command.CommandText = VouchManager.SaveVoucherMst(vmst, 1);
        command.ExecuteNonQuery();
        VouchDtl vdtl;
        for (int j = 0; j < 2; j++)
        {
            if (j == 0)
            {
                vdtl = new VouchDtl();
                vdtl.VchSysNo = vmst.VchSysNo;
                vdtl.ValueDate = aclsItemTransferStock.TransferDate;
                vdtl.LineNo = "1";
                vdtl.GlCoaCode = dtFixCode.Rows[0]["PH_DamagerCoa"].ToString();
                vdtl.Particulars = dtFixCode.Rows[0]["PH_DamagerCoa_Name"].ToString();
                vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["PH_DamagerCoa"].ToString());
                vdtl.AmountDr = Convert.ToDouble(PurchasePrice).ToString().Replace(",", "");
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
                vdtl.ValueDate = aclsItemTransferStock.TransferDate;
                vdtl.LineNo = "2";
                vdtl.GlCoaCode = dtFixCode.Rows[0]["Closing_Stock"].ToString(); //**** Gl_Main_OfficeCode *******//
                vdtl.Particulars = "Closing Stock";
                vdtl.AccType =
                    VouchManager.getAccType(dtFixCode.Rows[0]["Closing_Stock"]
                        .ToString()); //**** Gl_Main_OfficeCode *******//
                vdtl.AmountDr = "0";
                vdtl.AmountCr = Convert.ToDouble(PurchasePrice).ToString().Replace(",", "");
                vdtl.Status = vmst.Status;
                vdtl.BookName = vmst.BookName;
                BankAndCashBlanceCheck.GetBanlanceConvertion(vdtl, vdtl.AmountDr, vdtl.AmountCr);
                VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
            }
        }
        transaction.Commit();
        connection.Close();
    }

    public void DeleteItemnStockInfo(clsItemTransferStock aclsItemTransferStock, DataTable dtOld, VouchMst vmst, string CurrencyRate, string UserType)
    {
        //try
        //{
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        DataTable dtVoucherDtl = VouchManager.GetVouchDtl(vmst.VchSysNo, UserType);
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        connection.Open();
        transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        command.CommandText = @"UPDATE [ItemStockOutMst]
             SET [DeleteBy] = '" + aclsItemTransferStock.LoginBy + "',[DeleteDate] = GetDate() WHERE ID='" + aclsItemTransferStock.ID + "'";
        command.ExecuteNonQuery();
       

        //********************** Old List *******************//

        foreach (DataRow row in dtOld.Rows)
        {
            if (!string.IsNullOrEmpty(row["item_code"].ToString()))
            {
                command.CommandText = @"UPDATE [ItemSalesStock]
                         SET [Quantity] =ISNULL([Quantity],0)+" + Convert.ToDouble(row["OutQty"].ToString()) + "" +
                                      ",UpdateBy='" +
                                      aclsItemTransferStock.LoginBy + "',UpdateDate=GETDATE() WHERE [Flag]='" +
                                      row["Type"].ToString() + "' AND ID='" + row["ItemsID"].ToString() + "'";
                command.ExecuteNonQuery();

                if (row["Type"].ToString().Equals("1"))
                {
                    command.CommandText = @"UPDATE Item
				     	 SET SalesClosingQty= (ISNULL(SalesClosingQty,0)-convert(decimal," +
                                          row["OutQty"].ToString().Replace(",", "") +
                                          "))  WHERE ID = " + row["ItemsID"].ToString();
                    command.ExecuteNonQuery();
                }
                else if (row["Type"].ToString().Equals("2"))
                {
                    command.CommandText = @"UPDATE ItemSalesStock
					    SET SalesClosingQty= (ISNULL(SalesClosingQty,0)-convert(decimal," +
                                          row["OutQty"].ToString().Replace(",", "") + "))	WHERE ID = " +
                                          row["I_id"].ToString();
                    command.ExecuteNonQuery();
                }
            }
        }

        //***************************  Jurnal Voucher ********************************// 
        //******* Vucher One ******//
        VouchManager.DeleteVouchMstWithAutoVoucher(vmst, aclsItemTransferStock.LoginBy, UserType, command, dtVoucherDtl);
        transaction.Commit();
        connection.Close();
    }

    public DataTable GetShowItemsDetails(string Parameter, string ItemType)
    {
        String connectionString = DataManager.OraConnString();
        string query =
            @"SELECT t1.MstID,t.[ID] AS[ItemsID]
      ,t1.Code AS[item_code]
      ,t.StyleNo as StyleNo,t1.Barcode
	  ,t.Name AS[ItemsName]     
      ,st.ClosingStock AS[StockQty]
      ,t8.ColorName
      ,t9.SizeName
      ,isnull(t1.[TransferQuantity],0) AS[TransferQty]  
      ,convert(decimal (18,2),ISNULL(t1.TransferPrice,0)) AS [Price]  
      ,t4.Name AS[Catagory]
      ,t5.Name AS[SubCatagory],isnull(t1.Discount,0) as Discount
      ,t6.Name AS UMO
      ,isnull(t1.ReceivedQuantity,0) as ReceivedQuantity
      ,convert(decimal (18,2),isnull(t1.BranchSalesPrice,0)) as BranchSalesPrice
      FROM [dbo].[ItemStockTransferDtl] t1 
      INNER JOIN ItemStockTransferMst tt on tt.ID=t1.MstID AND tt.DeleteBy IS NULL 
      INNER JOIN Item t on t.ID=t1.ItemId and t.DeleteBy is null
       inner join ItemStock st on st.Barcode=t1.Barcode 
       
     left join Category t4 on t4.ID=t.CategoryID
      left join SubCategory t5 on t5.ID=t.SubCategoryID
      left join UOM t6 on t6.ID=t.UOMID 
      left join ColorInfo t8 on t8.ID=t.ItemColor
                      left join SizeInfo t9 on t9.ID=t.ItemSize " + Parameter+"   Order By t.Name ASC";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransfer");
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



    public void DeleteInfo(clsItemTransferStock aclsItemTransferStock)
    {
        string con = DataManager.OraConnString();
        string query = @"UPDATE [ItemStockTransferMst]
   SET [DeleteBy] = '" + aclsItemTransferStock.LoginBy + "',[DeleteDate] = GetDate() WHERE ID='" + aclsItemTransferStock.ID + "'";
        DataManager.ExecuteNonQuery(con, query);
    }


    public void UpdateBranchInfo(clsItemTransferStock aclsItemTransferStock, DataTable dt, DataTable OldDt, VouchMst vmst)
    {

        //try
        //{
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
        connection.Open();
        transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;

        command.CommandText = @"UPDATE [ItemStockTransferMst]
             SET [UpdateBy] = '" + aclsItemTransferStock.LoginBy + "',CartonNo='" + aclsItemTransferStock.CartonNo + "',BranchID='" + aclsItemTransferStock.BranchId + "',[TransferDate]=Convert(datetime,'" + aclsItemTransferStock.TransferDate + "'+' '+CONVERT(VARCHAR(8),GETDATE(),108),103),[UpdateDate] = GetDate()" +
                              ",Remark='" + aclsItemTransferStock.Remark.Replace("'", "") + "' " +
                              " WHERE ID='" + aclsItemTransferStock.ID + "'";
        command.ExecuteNonQuery();

        command.CommandText = @"UPDATE [ItemStockTransferDtl]
                SET  [DeleteBy] ='" + aclsItemTransferStock.LoginBy + "' ,[DeleteDate] =GETDATE() WHERE [MstID]='" +
                              aclsItemTransferStock.ID + "'";
        command.ExecuteNonQuery();

        //********************** Old List *******************//

        //foreach (DataRow rowold in OldDt.Rows)
        //{
        //    if (!string.IsNullOrEmpty(rowold["item_code"].ToString()))
        //    {
        //        command.CommandText = @"UPDATE [Item] SET [ClosingStock] =ISNULL([ClosingStock],0)+'" +
        //                              rowold["TransferQty"].ToString().Replace(",", "") + "'  WHERE ID='" +
        //                              rowold["ItemsID"].ToString() + "' ";
        //        command.ExecuteNonQuery();
        //    }
        //}

        //********************** New List *******************//
        decimal TotalAmount = 0;

        foreach (DataRow dr in dt.Rows)
        {
            if (!string.IsNullOrEmpty(dr["item_code"].ToString()))
            {
                command.CommandText = @"Select COUNT(*) from ItemStockTransferDtl where MstID='" +
                                      aclsItemTransferStock.ID + "' AND  ItemId='" + dr["ItemsID"].ToString() + "' ";
                int CheckCount = Convert.ToInt32(command.ExecuteScalar());
                if (CheckCount > 0)
                {
                    command.CommandText = @"Update ItemStockTransferDtl set CartonNo='" + aclsItemTransferStock.CartonNo + "',[TransferQuantity]='" +
                                          dr["TransferQty"].ToString().Replace(",", "") + "',TransferPrice='" +
                                          dr["Price"].ToString().Replace(",", "") +
                                          "',BranchSalesPrice='" + dr["BranchSalesPrice"].ToString().Replace(",", "") + "',Code='" + dr["item_code"].ToString().Replace(",", "") + "',StyleNo='" + dr["StyleNo"].ToString().Replace(",", "") + "',Discount='" + dr["Discount"].ToString() + "',DeleteBy=NULL,DeleteDate=NULL where MstID='" +
                                          aclsItemTransferStock.ID + "' AND ItemId='" + dr["ItemsID"].ToString() +
                                          "' ";
                    command.ExecuteNonQuery();

                    //************ Change Stock ********//

                    //command.CommandText = @"UPDATE [Item] SET [ClosingStock] =ISNULL([ClosingStock],0)-'" +
                    //                  dr["TransferQty"].ToString().Replace(",", "") + "'  WHERE ID='" +
                    //                  dr["ItemsID"].ToString() + "' ";
                    //command.ExecuteNonQuery();

                }
                else
                {
                    command.CommandText = @"INSERT INTO [ItemStockTransferDtl]
                   ([MstID],[ItemId],[TransferQuantity],TransferPrice,BranchSalesPrice,BranchID,Code,StyleNo,Discount,[Type])
                      VALUES
                   ('" + aclsItemTransferStock.ID + "','" + dr["ItemsID"].ToString() + "','" + dr["TransferQty"].ToString().Replace(",", "") +
                                              "','" +
                                              dr["Price"].ToString().Replace(",", "") + "','" +
                                              dr["BranchSalesPrice"].ToString().Replace(",", "") + "','" +
                                              aclsItemTransferStock.BranchId + "','" +
                                              dr["item_code"].ToString().Replace(",", "") + "','" +
                                              dr["StyleNo"].ToString().Replace(",", "") + "','" + dr["Discount"].ToString() + "','" + aclsItemTransferStock.TransferType + "')";
                    command.ExecuteNonQuery();
                    //TotalAmount += Convert.ToDecimal(dr["Price"]) * Convert.ToDecimal(dr["TransferQty"]);
                    //************ Change Stock ********//

                    //command.CommandText = @"UPDATE [Item] SET [ClosingStock] =ISNULL([ClosingStock],0)-'" +
                    //                 dr["TransferQty"].ToString().Replace(",", "") + "'  WHERE ID='" +
                    //                 dr["ItemsID"].ToString() + "' ";
                    //command.ExecuteNonQuery();
                }

                TotalAmount += Convert.ToDecimal(dr["Price"]) * Convert.ToDecimal(dr["TransferQty"]);

            }
        }

        //******* Vucher One ******//
        command.CommandText = @"select PurchaseCoaCode from [BranchInfo] where ID='" + aclsItemTransferStock.BranchId + "' ";
        string CoaCode = command.ExecuteScalar().ToString();


        vmst.UpdateDate = DateTime.Now.ToString("dd/MM/yyyy");
        vmst.ControlAmt = TotalAmount.ToString();

        command.CommandType = CommandType.Text;
        command.CommandText = VouchManager.SaveVoucherMst(vmst, 2);
        command.ExecuteNonQuery();
        command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" + vmst.VchSysNo + "')";
        command.ExecuteNonQuery();

        VouchDtl vdtl;
        for (int j = 0; j < 3; j++)
        {
            if (j == 0)
            {
                //DataRow 
                vdtl = new VouchDtl();
                vdtl.VchSysNo = vmst.VchSysNo;
                vdtl.ValueDate = vmst.ValueDate;
                vdtl.LineNo = "1";
                vdtl.GlCoaCode = dtFixCode.Rows[0]["TransferCode"].ToString(); //**** TransferCode *******//
                vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["TransferCode"].ToString()); //**** Purchase Code *******//
                vdtl.Particulars = "Item Transfer ";
                vdtl.AmountDr = vmst.ControlAmt.Replace(",", "");
                vdtl.AmountCr = "0";
                vdtl.Status = vmst.Status;
                vdtl.BookName = vmst.BookName;
                vdtl.AUTHO_USER = aclsItemTransferStock.LoginBy;
                VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
            }
            else if (j == 1)
            {
                vdtl = new VouchDtl();
                vdtl.VchSysNo = vmst.VchSysNo;
                vdtl.ValueDate = vmst.ValueDate;
                vdtl.LineNo = "2";
                vdtl.GlCoaCode = CoaCode;
                vdtl.Particulars = "Branch Purchase ";
                vdtl.AccType = VouchManager.getAccType(CoaCode);
                vdtl.AmountDr = "0";
                vdtl.AmountCr = vmst.ControlAmt.Replace(",", "");
                vdtl.Status = vmst.Status;
                vdtl.BookName = vmst.BookName;
                vdtl.AUTHO_USER = aclsItemTransferStock.LoginBy;
                VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
            }
            else if (j == 2)
            {
                vdtl = new VouchDtl();
                vdtl.VchSysNo = vmst.VchSysNo;
                vdtl.ValueDate = vmst.ValueDate;
                vdtl.LineNo = "3";
                vdtl.GlCoaCode = dtFixCode.Rows[0]["ClosingStock"].ToString(); ;
                vdtl.Particulars = "Closing Stock";
                vdtl.AccType = VouchManager.getAccType(vdtl.GlCoaCode);
                vdtl.AmountDr = "0";
                vdtl.AmountCr = vmst.ControlAmt.Replace(",", "");
                vdtl.AUTHO_USER = "CS";
                vdtl.Status = vmst.Status;
                vdtl.BookName = vmst.BookName;
                vdtl.AUTHO_USER = aclsItemTransferStock.LoginBy;
                VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
            }
        }
        transaction.Commit();
        connection.Close();

        //}
        //catch (Exception ex)
        //{
        //    throw new Exception(ex.Message);
        //}


    }

    public int SaveInformation(clsItemTransferStock aclsItemTransferStock, DataTable dt, VouchMst vmst, VouchMst vmstPayment)
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
            //command.CommandText = @"SELECT [ID]  FROM [dbo].[BranchInfo] where [MainBranch]=1";
            //int Headoffice = Convert.ToInt32(command.ExecuteScalar());

            //command.CommandText = @"select top(1)ID from [ItemStockTransferMst] ORDER BY ID DESC";
            //int MstID = Convert.ToInt32(command.ExecuteScalar());



            command.CommandText = @"INSERT INTO [ItemStockTransferMst]
           ([BranchID],[TransferDate],[Remark],[AddBy],[AddDate],Code,CartonNo,ChallanNo,TransferType)
     VALUES
           ('" + aclsItemTransferStock.BranchId + "',CONVERT(datetime,'" + aclsItemTransferStock.TransferDate + "'+' '+CONVERT(VARCHAR(8),GETDATE(),108),103),'" +
                                  aclsItemTransferStock.Remark.Replace("'", "") + "','" + aclsItemTransferStock.LoginBy +
                                  "',GETDATE(),'" + aclsItemTransferStock.Code + "','" + aclsItemTransferStock.CartonNo + "','" + aclsItemTransferStock.ChallanNo + "','" + aclsItemTransferStock.TransferType + "')";
            command.ExecuteNonQuery();
            command.CommandText = @"select top(1)ID from [ItemStockTransferMst] ORDER BY ID DESC";
           int  MstID = Convert.ToInt32(command.ExecuteScalar());
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

                            command.CommandText = @"INSERT INTO [ItemStockTransferDtl]([MstID],[ItemId],[TransferQuantity],TransferPrice,BranchSalesPrice,BranchID,Code,StyleNo,Discount,[Type],Barcode) VALUES
('" + MstID + "','" + ItemId + "','" +TransferQuantity+   "','" +TransferPrice + "','" + BranchSalesPrice + "','" + BranchID+ "','" +Code + "','" +StyleNo + "','" + Discount + "','" +Type + "','" +Barcode + "')";
                            command.CommandTimeout = 500;
                            command.ExecuteNonQuery();

                            command.CommandText = @"select top(1)ID from ItemStock where ItemID='" + ItemId + "' and BranceId='" + BranchID + "' and barcode='" + Barcode + "'";
                            int ItemStockId = Convert.ToInt32(command.ExecuteScalar());

                            if (ItemStockId == 0)
                            {
                                command.CommandText = @"INSERT INTO [ItemStock]([ItemId],[ClosingStock],ClosingAmount,CostPrice,ItemsPrice,BranceId,ItemCode,Barcode,[AddBy],[AddDate]) VALUES
('" + ItemId + "','" + TransferQuantity + "'," + TransferPrice + "*" + TransferQuantity + ",'" + TransferPrice + "','" + BranchSalesPrice + "','" + BranchID + "','" + Code + "','" + Barcode + "','" + aclsItemTransferStock.LoginBy + "',GETDATE())";
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                command.CommandText = @"Update ItemStock set  ClosingStock=ClosingStock + " + TransferQuantity + ",ClosingAmount=" + TransferPrice + "*(" + TransferQuantity + "+" + TransferQuantity + ") where ID='" + ItemStockId + "' ";
                                command.ExecuteNonQuery();
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
                            //command.CommandText = @"UPDATE [Item] SET [ClosingStock] =ISNULL([ClosingStock],0)-'" +
                            //                      dr["TransferQty"].ToString().Replace(",", "") + "'  WHERE ID='" +
                            //                      dr["ItemsID"].ToString() + "' ";
                            //command.ExecuteNonQuery();
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

            VouchDtl vdtl;
            for (int j = 0; j < 4; j++)
            {
                if (j == 0)
                {
                    //DataRow 
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = vmst.VchSysNo;
                    vdtl.ValueDate = vmst.ValueDate;
                    vdtl.LineNo = "1";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["TransferCode"].ToString(); //**** TransferCode *******//
                    vdtl.AccType = VouchManager.getAccType(dtFixCode.Rows[0]["TransferCode"].ToString()); //**** Purchase Code *******//
                    vdtl.Particulars = "Item Transfer ";
                    vdtl.AmountDr = TotalSaleAmount.ToString().Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    vdtl.AUTHO_USER = aclsItemTransferStock.LoginBy;
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                }
                else if (j == 1)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = vmst.VchSysNo;
                    vdtl.ValueDate = vmst.ValueDate;
                    vdtl.LineNo = "2";
                    vdtl.GlCoaCode = CoaCode;
                    vdtl.Particulars = "Branch Purchase ";
                    vdtl.AccType = VouchManager.getAccType(CoaCode);
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = vmst.ControlAmt.Replace(",", "");
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    vdtl.AUTHO_USER = aclsItemTransferStock.LoginBy;
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                }
                else if (j == 2)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = vmst.VchSysNo;
                    vdtl.ValueDate = vmst.ValueDate;
                    vdtl.LineNo = "3";
                    vdtl.GlCoaCode = dtFixCode.Rows[0]["ClosingStock"].ToString(); ;
                    vdtl.Particulars = "Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(vdtl.GlCoaCode);
                    vdtl.AmountDr = "0";
                    vdtl.AmountCr = vmst.ControlAmt.Replace(",", "");
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    vdtl.AUTHO_USER = aclsItemTransferStock.LoginBy;
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                }
                else if (j == 3)
                {
                    vdtl = new VouchDtl();
                    vdtl.VchSysNo = vmst.VchSysNo;
                    vdtl.ValueDate = vmst.ValueDate;
                    vdtl.LineNo = "4";
                    vdtl.GlCoaCode = ClosingCoaCode;
                    vdtl.Particulars = "Branch Closing Stock";
                    vdtl.AccType = VouchManager.getAccType(ClosingCoaCode);
                    vdtl.AmountDr = vmst.ControlAmt.Replace(",", "");
                    vdtl.AmountCr = "0";
                    vdtl.AUTHO_USER = "CS";
                    vdtl.Status = vmst.Status;
                    vdtl.BookName = vmst.BookName;
                    vdtl.AUTHO_USER = aclsItemTransferStock.LoginBy;
                    VouchManager.CreateVouchDtlForAutoVoucher(vmst, vdtl, command);
                }
            }


            command.CommandText = @"Update ItemStockTransferMst set  AutoVoucher=1 where ID='" + MstID + "' ";
            command.ExecuteNonQuery();

            command.CommandText = @"Update FixValue set  TransferID=TransferID+1  ";
            command.ExecuteNonQuery();

            transaction.Commit();
            connection.Close();

            //********************** Insert Pos *************//

            //            SqlConnection conPos = new SqlConnection(DataManager.OraConnString_Pos());
            //            SqlTransaction TranPos;
            //            conPos.Open();
            //            TranPos = conPos.BeginTransaction();
            //            SqlCommand command_Pos = new SqlCommand();
            //            command_Pos.Connection = conPos;
            //            command_Pos.Transaction = TranPos;

            //            command_Pos.CommandText = @"INSERT INTO [ItemStockTransferMst]
            //           (Mst_ID,[BranchID],[TransferDate],[Remark],[AddBy],[AddDate])
            //     VALUES
            //           ('" + MstID + "','" + aclsItemTransferStock.BranchId + "',CONVERT(DATE,'" + aclsItemTransferStock.TransferDate + "',103),'" + aclsItemTransferStock.Remark.Replace("'", "") + "','" + aclsItemTransferStock.LoginBy + "',GETDATE())";
            //            command_Pos.ExecuteNonQuery();
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            if (!string.IsNullOrEmpty(dr["item_code"].ToString()))
            //            {
            //                command_Pos.CommandText = @"SELECT COUNT(*) FROM [Item] where UPPER([Code])=UPPER('" +
            //                                          dr["item_code"].ToString() + "')";
            //                int ItemsCode = Convert.ToInt32(command_Pos.ExecuteScalar());
            //                if (ItemsCode <= 0)
            //                {
            //                    int catagoryID = 0;
            //                    int SubCatagoryID = 0;
            //************** Check Catagory ******************//
            //                    if (dr["Catagory"].ToString().ToUpper().Equals("NONE"))
            //                    {
            //                        catagoryID = 122;
            //                    }
            //                    else
            //                    {
            //                        command_Pos.CommandText = @"select ID from Category WHERE UPPER(Name)=UPPER('" +
            //                                                  dr["Catagory"].ToString() + "')";
            //                        catagoryID = Convert.ToInt32(command_Pos.ExecuteScalar());
            //                        if (catagoryID == 0)
            //                        {
            //                            command_Pos.CommandText = @"INSERT INTO [Category] ([Name],Code,Active)
            //                               VALUES ('" + dr["Catagory"].ToString() + "'," + MstID + ",'True')";
            //                            command_Pos.ExecuteNonQuery();

            //                            command_Pos.CommandText = "select top(1)ID from Category order by ID desc";
            //                            catagoryID = Convert.ToInt32(command_Pos.ExecuteScalar());
            //                        }
            //                    }
            //************** Check Sub_Catagory ******************//
            //                    if (dr["SubCatagory"].ToString().ToUpper().Equals("NONE"))
            //                    {
            //                        SubCatagoryID = 477;
            //                    }
            //                    else
            //                    {
            //                        command_Pos.CommandText = @"select ID from SubCategory WHERE UPPER(Name)=UPPER('" +
            //                                                  dr["SubCatagory"].ToString() + "')";
            //                        SubCatagoryID = Convert.ToInt32(command_Pos.ExecuteScalar());
            //                        if (SubCatagoryID == 0)
            //                        {
            //                            command_Pos.CommandText = @"INSERT INTO [SubCategory] ([Name],Code,Active,CategoryID)
            //                               VALUES ('" + dr["SubCatagory"].ToString() + "'," + MstID + ",'True'," + catagoryID + ")";
            //                            command_Pos.ExecuteNonQuery();

            //                            command_Pos.CommandText = "select top(1)ID from SubCategory order by ID desc";
            //                            SubCatagoryID = Convert.ToInt32(command_Pos.ExecuteScalar());
            //                        }
            //                    }
            //************** Check UMO ******************//

            //                    command_Pos.CommandText = @"select ID from UOM WHERE UPPER(Name)=UPPER('" +
            //                                              dr["UMO"].ToString() + "')";
            //                    int UMO_ID = Convert.ToInt32(command_Pos.ExecuteScalar());
            //                    if (UMO_ID == 0)
            //                    {
            //                        command_Pos.CommandText = @"INSERT INTO [UOM] ([Name],Active)
            //                               VALUES ('" + dr["UMO"].ToString() + "','True')";
            //                        command_Pos.ExecuteNonQuery();

            //                        command_Pos.CommandText = "select top(1)ID from UOM order by ID desc";
            //                        UMO_ID = Convert.ToInt32(command_Pos.ExecuteScalar());
            //                    }

            //                    command_Pos.CommandText = @"INSERT INTO [Item] ([Code],[Name],[UOMID],[Currency],[ClosingStock],[ClosingAmount],[CategoryID],                                            [SubCategoryID],[TaxCategoryID],Active,CreatedBy,CreatedDate,[ItemSize],[ItemColor],[OpeningStock],[OpeningAmount],[Discounted],[DiscountAmount],StyleNo,BranchSalesPrice)
            //                         VALUES
            //                        ('" + dr["item_code"].ToString() + "','" + dr["item_desc"].ToString() + "','" + UMO_ID +
            //                                              "','BD',0,0," + catagoryID + "," +
            //                                              SubCatagoryID + ",'1','True','1',GETDATE(),0,0,0,0,'False',0,'" + dr["StyleNo"].ToString().Replace("'", "") + "','" + dr["BranchSalesPrice"].ToString().Replace(",", "") + "')";
            //                    command_Pos.ExecuteNonQuery();
            //}
            //else
            //{
            //    command_Pos.CommandText = @"SELECT ID FROM [Item] where UPPER([Code])=UPPER('" +
            //                              dr["item_code"].ToString() + "')";
            //    int ItemsID = Convert.ToInt32(command_Pos.ExecuteScalar());
            //    command_Pos.CommandText = @"update Item set UnitPrice='" + dr["Price"].ToString().Replace(",", "") +
            //                              "',BranchSalesPrice='" + dr["BranchSalesPrice"].ToString().Replace(",", "") + "' where ID=" + ItemsID + " ";
            //    command_Pos.ExecuteNonQuery();
            //}
            //                command_Pos.CommandText = @"INSERT INTO [ItemStockTransferDtl]
            //                     ([MstID],[ItemId],[TransferQuantity],TransferPrice,Code,BranchSalePrice)
            //                     VALUES
            //                        (" + MstID + ",'" + dr["ItemsID"].ToString() + "','" +
            //                                          dr["TransferQty"].ToString().Replace(",", "") + "','" +
            //                                          dr["Price"].ToString().Replace(",", "") + "','" + dr["item_code"].ToString() + "','" +
            //                                          dr["BranchSalesPrice"].ToString().Replace(",", "") + "')";
            //                command_Pos.ExecuteNonQuery();

            //command_Pos.CommandText = @"UPDATE [Item] SET [ClosingStock]=ISNULL([ClosingStock],0)+" +
            //                          dr["TransferQty"].ToString().Replace(",", "") + ",BranchSalesPrice='" + dr["BranchSalesPrice"].ToString().Replace(",", "") + "' ,UnitPrice='" + dr["Price"].ToString().Replace(",", "") + "' WHERE [Code]='" +
            //                          dr["item_code"].ToString() + "'";
            //        //command_Pos.ExecuteNonQuery();
            //    }
            //}
            //TranPos.Commit();
            //conPos.Close();
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
            //finally
            //{
            //    if (connection.State == ConnectionState.Open)
            //        connection.Close();
            //}
            return 1;
        //}
        //catch
        //{
        //    transaction.Rollback();
        //    connection.Close();
        //    return 0;
        //}

    }


    public DataTable getShowTransfrerDetails(string ID)
    {
        String connectionString = DataManager.OraConnString();
        string query = @"SELECT t.[ID] AS[ItemsID]
      ,t.ItemCode AS[item_code]
	  ,t3.Name AS[item_desc] 
	  ,t5.ColorName
	  ,t6.SizeName    
      ,Isnull(tt1.ClosingStock,0)+ISNULL(tt1.CartonStock,0) AS[StockQty]
      ,t1.[TransferQuantity] AS[TransferQty]  
      ,ISNULL(t1.TransferPrice,0) AS [Price]  
  FROM [dbo].[ItemStockTransferDtl] t1 
  INNER JOIN ItemStockTransferMst tt on tt.ID=t1.MstID AND tt.DeleteBy 
  IS NULL INNER JOIN ItemPurOrderDtl t on t.ID=t1.ItemId inner join ItemStock  tt1 on tt1.ItemID=t.ID
  and t.DeleteBy is null inner join Item t3 on t3.ID=t.ItemID 
  left join Brand t4 on t3.Brand=t4.ID left join ColorInfo t5 on t5.Code=t.ColorCode left join SizeInfo t6 on 
t6.Code=t.SizeCode
  where t1.MstID='" + ID + "'  and t1.DeleteBy IS NULL ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransfer");
        return dt;
    }


    public DataTable getShowTransferDetailsOnReport(string Mst_ID, string connectionString)
    {

        string Query =
            @"select t1.[ID], row_number() OVER (ORDER BY t1.ID) as SL,t1.[BranchID],t1.ChallanNo,t2.BranchName,CONVERT(NVARCHAR,t1.[TransferDate],103)TransferDate,
t1.[Remark],st.NewItemCode as ItemCode,t5.[Name] AS ItemsName,st.NewItemCode+' - '+t5.[Name] AS Items,t6.Name AS Catagory,
t7.Name AS SubCat,t3.BranchSalesPrice AS [UnitPrice],t2.VatRegNo,ISNULL(t3.TransferQuantity,0) AS Qty ,Isnull((select isnull(Rate,0) from dbo.TaxCategory where [Type]='S' ),0) as VatRate
,t8.ColorName,t9.SizeName,t2.Address1 ,t2.VatRegNo,t2.Phone,fb.Name as FabricsType,gd.Name as ProductType,t.DesigNo, ISNULL(t3.TransferQuantity,0)*t3.BranchSalesPrice as Total,(ISNULL(t3.TransferQuantity,0)*t3.BranchSalesPrice)*(Isnull((select isnull(Rate,0) from dbo.TaxCategory where [Type]='S' ),0)/100) as Vat
from
ItemStockTransferDtl t3 inner join [ItemStockTransferMst] t1 on t1.ID=t3.MstID inner join 
 BranchInfo t2 on t2.ID=t1.BranchID inner join ItemStock st on st.NewItemCode=t3.Code inner join  dbo.ItemPurOrderDtl AS t on t.ID=t3.ItemId INNER JOIN
                      dbo.Item AS t5 on t5.ID=t.ItemID left join 
 Category t6 on t6.ID=t.CategoryID left join SubCategory t7 on t7.ID=t.SubCategeoryID
 left join ColorInfo t8 on t8.Code=t.ColorCode left join FabricsType fb on fb.ID=t.FabricsType
 left join GoodsType gd on gd.ID=t.GoodsTypeID
                      left join SizeInfo t9 on t9.Code=t.SizeCode 
 where t3.DeleteBy IS NULL AND t1.ID='" + Mst_ID + "' ORDER BY  t5.[Name] ASC ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "ItemStockTransfer");
        return dt;
    }





    public static DataTable getShowTransferMstOnPos(string SearchDate, string ToDate,string BranchId)
    {

        string parameter = "";
        if (SearchDate != "")
        {
            parameter += "  AND convert(date,[TransferDate],103) between convert(date,'" + SearchDate.ToString() + "',103) and convert(date,'" + ToDate.ToString() + "',103) ";
        }

        String connectionString = DataManager.OraConnString();
        string query =
            @"SELECT t1.[ID],t1.MStID,[BranchID],TransferFromBranchID,ReceivedBy,(Select SUM(TransferQuantity) from ItemStockTransferDtl where ItemStockTransferDtl.MstID=t1.MStID) as TotalQty,(Select SUM(TransferQuantity*BranchSalesPrice) from dbo.ItemStockreceivedDtl where ItemStockreceivedDtl.MstID=t1.MStID) as TotalAmount,case when TransferFromBranchID is null or  TransferFromBranchID=0  then 'Head Office' else   t2.BranchName end as BranchName,convert(nvarchar,[TransferDate],103) AS TransferDate,[Remark],Code ,CASE WHEN ReceivedBy IS NULL THEN 'Not Received' ELSE 'Recived' END AS [Status]     
            FROM ItemStockReceivedMst t1 left join branchInfo t2 on t2.ID=t1.TransferFromBranchID
             WHERE t1.[DeleteBy] IS NULL and t1.BranchId='"+BranchId+"'  " + parameter + " order by t1.ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransferMst");
        return dt;
    }

public static DataTable getShowTransferDtlPos(short MstID)
{
    String connectionString = DataManager.OraConnString();
    string query =
        @"      SELECT t1.ID,t1.MstID,t1.Barcode,t2.Code,t2.ID AS [ItemId],t1.Discount,t2.Name,t4.ColorName as Color ,t5.SizeName as Size,Isnull(t1.[TransferQuantity],0) as TransferQuantity,t1.[TransferPrice],t1.BranchSalesPrice ,CASE WHEN t1.ReceivedQuantity IS NULL OR t1.ReceivedQuantity=0 then t1.[TransferQuantity] else t1.ReceivedQuantity end as ReceivedQuantity ,t1.ReceivedQuantity as OldReceivedQuantity,t1.[DtlID] as DtlID,t3.BranchID as BranchID,t2.StyleNo as StyleNo
  FROM dbo.ItemStockreceivedDtl t1 left join Item t2 on t2.Code=t1.Code left join dbo.ColorInfo t4 on t4.id=t2.ItemColor left join SizeInfo t5 on t5.ID=SizeID
  inner join dbo.ItemStockReceivedMst t3 on t3.MStID=t1.MstID where t1.MstID='"+MstID+"'  AND t1.[DeleteBy] IS NULL";
    DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ItemStockTransferMst");
    return dt;
    //AND t1.[ReceivedQuantity] IS NULL
}



public static int UpdateTranseferMst(string Id, string loginBy)
{

    String connectionString = DataManager.OraConnString();
    SqlConnection sqlCon = new SqlConnection(connectionString);
    sqlCon.Open();
    string query = "Update   " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].ItemStockTransferMst set LocalUpload='1',ReceivedBy='" + loginBy + "',ReceivedDate=GetDate() where ID='" + Id + "'";
    var command=new SqlCommand(query,sqlCon);

    int Check = command.ExecuteNonQuery();
    sqlCon.Close();

    return Check;
}

public static int UpdateTranseferDtl(string Id, string loginBy)
{
    String connectionString = DataManager.OraConnString();
    SqlConnection sqlCon = new SqlConnection(connectionString);
    sqlCon.Open();
    string query = "Update   " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].ItemStockTransferDtl set ReceivedQuantity=TransferQuantity where MstID='" + Id + "'";
    var command = new SqlCommand(query, sqlCon);

    int Check = command.ExecuteNonQuery();
    sqlCon.Close();

    return Check;
}

public static int getSupplier()
{
    String connectionString = DataManager.OraConnString();
    SqlConnection sqlCon = new SqlConnection(connectionString);
    sqlCon.Open();
    string query = "select COUNT(Id) from Supplier";
    var command = new SqlCommand(query, sqlCon);

    int Check = Convert.ToInt32(command.ExecuteScalar());
    sqlCon.Close();

    return Check;
}

public static DataTable GetMainBrancSupplier()
{

    String connectionString = DataManager.OraConnString();
    string query = @"Select * FROM " + ConfigurationManager.AppSettings["DataBase"] + ".[dbo].[GL_SET_OF_BOOKS]   ";
    DataTable dt = DataManager.ExecuteQuery(connectionString, query, "GL_SET_OF_BOOKS");
    return dt;

}



public static DataTable IsExist(ClsItemDetailsInfo aClsItemDetailsInfoObj)
{
    string ConnectionString = DataManager.OraConnString();
    string query = "";

    if (string.IsNullOrEmpty(aClsItemDetailsInfoObj.Id))
    {
       query="select * from Item Where  UOMID='"+aClsItemDetailsInfoObj.Umo+"' and CategoryID='"+aClsItemDetailsInfoObj.Catagory+"' and SubCategoryID='"+aClsItemDetailsInfoObj.SubCatagory+"' and TaxCategoryID='"+aClsItemDetailsInfoObj.Text+"' and Brand='"+aClsItemDetailsInfoObj.Brand+"' and DeptID='"+aClsItemDetailsInfoObj.DepID+"' and SizeID='"
           + aClsItemDetailsInfoObj.SizeID + "' and ItemSetupID='" + aClsItemDetailsInfoObj.ItemSetupID + "'";
    }
    else
    {
        query = "select * from Item Where Id!='"+aClsItemDetailsInfoObj.Id+"'  and UOMID='" + aClsItemDetailsInfoObj.Umo + "' and CategoryID='" + aClsItemDetailsInfoObj.Catagory + "' and SubCategoryID='" + aClsItemDetailsInfoObj.SubCatagory + "' and TaxCategoryID='" + aClsItemDetailsInfoObj.Text + "' and Brand='" + aClsItemDetailsInfoObj.Brand + "' and DeptID='" + aClsItemDetailsInfoObj.DepID + "' and SizeID='"
                + aClsItemDetailsInfoObj.SizeID + "' and ItemSetupID='" + aClsItemDetailsInfoObj.ItemSetupID + "'";  
    }

    var data = DataManager.ExecuteQuery(ConnectionString, query, "Item");
    return data;

}
}
