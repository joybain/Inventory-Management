using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Delve;

/// <summary>
/// Summary description for RejectItemManager
/// </summary>
public class RejectItemManager
{
	public RejectItemManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    

public int Save(System.Data.DataTable dtDtlGrid,string loginby,string BranchId)
{


    decimal PurchasePrice = 0;
    int MstID = 0;
    SqlConnection connection = new SqlConnection(DataManager.OraConnString());
    SqlTransaction transaction;
    DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
    connection.Open();
    transaction = connection.BeginTransaction();
    try
    {
        string FixExDate = "";
        string ExDateValue = "";

        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;
        foreach (DataRow dr in dtDtlGrid.Rows)
        {
            if (!string.IsNullOrEmpty(dr["ExpireDate"].ToString()))
            { 
                FixExDate = ",ExpireDate";
                ExDateValue = ",convert(date,'" + dr["ExpireDate"].ToString() + "',103)";
            }
            command.CommandText = @"insert into dbo.ItemRejectStockDtl( Code, Barcode, ItemId, ItemSize, ItemColor, CategoryId, SubCategoryId, CostPrice, SPrice, Qty, AddBy, AddDate, BranchId "+FixExDate+")values('" + dr["Code"].ToString() + "','" + dr["Barcode"].ToString() + "','" + dr["ItemId"].ToString() + "','" + dr["ItemSize"].ToString() + "','" + dr["ItemColor"].ToString() + "','" + dr["CategoryId"].ToString() + "','" + dr["SubCategoryId"].ToString() + "','" + dr["CostPrice"].ToString() + "','" + dr["SPrice"].ToString() + "', '" + dr["Qty"].ToString() + "','" + loginby + "',GetDate() ,'" + BranchId + "' "+ExDateValue+")";
        MstID=    command.ExecuteNonQuery();

    
            command.CommandText = @"select Count(Id) from dbo.ItemRejectStockMst where Barcode='" + dr["Barcode"].ToString() + "'";
            int Check = Convert.ToInt32(command.ExecuteScalar());
            if (Check>0)
            {
                command.CommandText = @"update ItemRejectStockMst set Qty=Qty+'" + Convert.ToInt32(dr["Qty"].ToString()) +
                                      "'where Barcode='" + dr["Barcode"].ToString() + "' ";
                command.ExecuteNonQuery();
            }

            else
            {

                command.CommandText = @"insert into dbo.ItemRejectStockMst( Code, Barcode, ItemId, ItemSize, ItemColor, CategoryId, SubCategoryId, CostPrice, SPrice, Qty,  BranchId "+FixExDate+")values('" + dr["Code"].ToString() + "','" + dr["Barcode"].ToString() + "','" + dr["ItemId"].ToString() + "','" + dr["ItemSize"].ToString() + "','" + dr["ItemColor"].ToString() + "','" + dr["CategoryId"].ToString() + "','" + dr["SubCategoryId"].ToString() + "','" + dr["CostPrice"].ToString() + "','" + dr["SPrice"].ToString() + "', '" + dr["Qty"].ToString() + "','" + BranchId + "' "+ExDateValue+")";
                command.ExecuteNonQuery();
            }

        }




     

       

        transaction.Commit();
    }
    catch (Exception ex)
    {
        transaction.Rollback();
        throw new Exception(ex.Message);
    }
    finally
    {
        if (connection.State == ConnectionState.Open)
            connection.Close();
    }

    return MstID;


}

public DataTable getShowBranchItemsInfo(string ItemsID, string CatagoryID, string SubCatagoryID, string BrandID, string DepartmentID, string FormDate, string ToDate, string ItemSetupID, string Barcode, string BranchId)
{
    using (SqlConnection conn = new SqlConnection(DataManager.OraConnString()))
    {
        SqlCommand sqlComm = new SqlCommand("[SP_ItemsBranchRejectStockDetails]", conn);
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
        da.Fill(ds, "[SP_ItemsBranchRejectStockDetails]");
        DataTable dtStokDtl = ds.Tables["[SP_ItemsBranchRejectStockDetails]"];
        return dtStokDtl;
    }
}
private int FindItem(string Barcode)
{
    String connectionString = DataManager.OraConnString();
    SqlConnection sqlCon = new SqlConnection(connectionString);
    sqlCon.Open();
    string query = "select Count(Id) from dbo.ItemRejectStockMst where Barcode='"+Barcode+"'";
    var command = new SqlCommand(query, sqlCon);

    int Check = Convert.ToInt32(command.ExecuteScalar());
    sqlCon.Close();

    return Check;
}
}