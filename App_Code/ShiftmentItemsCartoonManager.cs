using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Delve;
using System.Data.Common;

/// <summary>
/// Summary description for ShiftmentItemsCartoonManager
/// </summary>
public class ShiftmentItemsCartoonManager
{
	public ShiftmentItemsCartoonManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static void SaveItemsCurtoonInformation(ShiftmentItemsCartoon aItemsCartoon,DataTable dtt)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transection;
        try
        {
            //string SelectQuery = @"SELECT [MasterId],[ColorID_SizeID],[Type] FROM [Temp_ShiftmentItemsColorSize] ";
            DataTable dtColSz = null;
                //DataManager.ExecuteQuery(DataManager.OraConnString(), SelectQuery, "Temp_ShiftmentItemsColorSize");
            //string SelectQuery1 = @"SELECT [MasterId],[ColorID],[SizeID],[Quantity] FROM [Temp_ShiftmentItemsQuantity] ";
            DataTable dtQty = null;
              //  DataManager.ExecuteQuery(DataManager.OraConnString(), SelectQuery1, "Temp_ShiftmentItemsColorSize");
            // string Query = @"SELECT [MasterId],[Image],ImageName FROM [Temp_ShiftmentItemsImage] ";
            DataTable dtImage = null;
                //DataManager.ExecuteQuery(DataManager.OraConnString(), Query, "Temp_ShiftmentItemsImage");           
            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;
            SqlCommand command1 = new SqlCommand();
            command1.Connection = connection;
            command1.Transaction = transection;
            SqlCommand command2 = new SqlCommand();
            command2.Connection = connection;
            command2.Transaction = transection;

            SqlCommand command3 = new SqlCommand();
            command3.Connection = connection;
            command3.Transaction = transection;

            command.CommandText = @"INSERT INTO [ShiftmentBoxingMst]
           ([CartoonNo],[ShiftmentID] ,[Remarks] ,[AddDate] ,[AddBy])
     VALUES
           ('" + aItemsCartoon.CartoonNO + "','" + aItemsCartoon.ShiftmentNo + "','" + aItemsCartoon.Remarks + "',GETDATE(),'" + aItemsCartoon.LoginBy + "')";
            command.ExecuteNonQuery();
            command.CommandText = @"SELECT top(1)[ID]  FROM [ShiftmentBoxingMst] order by ID desc";
            int MstID = Convert.ToInt32(command.ExecuteScalar());
            foreach (DataRow dr in dtt.Rows)
            {
                if (dr["item_code"].ToString() != "")
                {
                    dtColSz = null;
                    dtQty = null;
                    dtImage = null;
                    command2.CommandText = @"SELECT [MasterId],[ColorID_SizeID],[Type] FROM [Temp_ShiftmentItemsColorSize] where MasterId='" + dr["item_code"].ToString() + "'  ";
                    using (SqlDataReader drColorSize = command2.ExecuteReader())
                    {
                        dtColSz = new DataTable();
                        dtColSz.Load(drColorSize);                       
                    }
                    command3.CommandText = @"SELECT [MasterId],[ColorID],[SizeID],[Quantity] FROM [Temp_ShiftmentItemsQuantity] where MasterId='" + dr["item_code"].ToString() + "' ";
                    using (SqlDataReader drQty = command3.ExecuteReader())
                    {
                        dtQty = new DataTable();
                        dtQty.Load(drQty);
                    }
                    command2.CommandText = @"SELECT [MasterId],[Image],ImageName FROM [Temp_ShiftmentItemsImage] where MasterId='" + dr["item_code"].ToString() + "' ";
                    using (SqlDataReader drImage = command2.ExecuteReader())
                    {
                        dtImage = new DataTable();
                        dtImage.Load(drImage);
                    }

                    command.CommandText = @"INSERT INTO [ShiftmentBoxingItemsDtl]
           ([MasterID],[ItemsID],[Quantity],[Color])
     VALUES
           ('" + MstID + "','" + dr["item_code"].ToString() + "','" + dr["qty"].ToString() + "','" + dr["Color"].ToString() + "')";
                    command.ExecuteNonQuery();
                    if (dtColSz.Rows.Count > 0)
                    {
                        DataRow[] Col = dtColSz.Select("MasterId =" + dr["item_code"].ToString());
                        if (Col != null)
                        {
                            foreach (DataRow dr1 in Col)
                            {
                                command.CommandText = @"INSERT INTO [ShiftmentItemsBoxingColorSize]
                             ([BoxingItemsID],ItemsID,[ColorID_SizeID],[Type])
                            VALUES ('" + MstID + "','" + dr["item_code"].ToString() + "','" + dr1["ColorID_SizeID"].ToString() + "','" + dr1["Type"].ToString() + "')";
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    if (dtQty.Rows.Count > 0)
                    {
                        DataRow[] Qty = dtQty.Select("MasterId =" + dr["item_code"].ToString());
                        if (Qty != null)
                        {
                            foreach (DataRow dr3 in Qty)
                            {
                                if (dr3["Quantity"].ToString() != "" || dr3["Quantity"].ToString() != "0")
                                {
                                    command1.CommandText = @"INSERT INTO [ShiftmentBoxingItemsQuantity]
                ([BoxingItemsID],ItemsID,[ColorID],[SizeID],[Quantity])
                VALUES
                ('" + MstID + "','" + dr["item_code"].ToString() + "','" + dr3["ColorID"].ToString() + "','" + dr3["SizeID"].ToString() + "','" + dr3["Quantity"].ToString() + "')";
                                    command1.ExecuteNonQuery();
                                }
                            }
                        }
                    }                    
                    command.CommandText = @"delete from Temp_ShiftmentItemsColorSize where MasterId='" + dr["item_code"].ToString() + "' ";
                    command.ExecuteNonQuery();
                    command.CommandText = @"delete from Temp_ShiftmentItemsQuantity where MasterId='" + dr["item_code"].ToString() + "' ";
                    command.ExecuteNonQuery();
                    command.CommandText = @"delete from Temp_ShiftmentItemsImage where MasterId='" + dr["item_code"].ToString() + "' ";
                    command.ExecuteNonQuery();
                }
            }
            transection.Commit();
            connection.Close();
            if (dtImage != null)
            {
                foreach (DataRow dr in dtt.Rows)
                {
                    if (dr["item_code"].ToString() != "")
                    {                       
                        DataRow[] Img = dtImage.Select("MasterId =" + dr["item_code"].ToString());
                        if (Img != null)
                        {
                            if (Img.Length > 0)
                            {
                                foreach (DataRow dr11 in Img)
                                {
                                    connection.Open();
                                    string ImgQuery = @"INSERT INTO [ShiftmentBoxingItemsImage] ([BoxingItemsID],ItemsID,ImageName,[Image])
                            VALUES ('" + MstID + "','" + dr["item_code"].ToString() + "','" + dr11["ImageName"].ToString() + "',@img)";                         
                                    SqlCommand cmnd = new SqlCommand(ImgQuery, connection);
                                    SqlParameter img = new SqlParameter();
                                    img.SqlDbType = SqlDbType.VarBinary;
                                    img.ParameterName = "img";
                                    img.Value = dr11["Image"];
                                    cmnd.Parameters.Add(img);
                                    if (dr11["Image"] == null)
                                    {
                                        cmnd.Parameters.Remove(img);
                                    }
                                    cmnd.ExecuteNonQuery();
                                    connection.Close();
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            //connection.Close();
        }
    }
    public static void UpdateCurtoonInformation(ShiftmentItemsCartoon aItemsCartoon,DataTable ddt)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transection;
        try
        {
            //string SelectQuery = @"SELECT [MasterId],[ColorID_SizeID],[Type] FROM [Temp_ShiftmentItemsColorSize] ";
            //DataTable dtColSz = DataManager.ExecuteQuery(DataManager.OraConnString(), SelectQuery, "Temp_ShiftmentItemsColorSize");
            //string SelectQuery1 = @"SELECT [MasterId],[ColorID],[SizeID],[Quantity] FROM [Temp_ShiftmentItemsQuantity] ";
            //DataTable dtQty = DataManager.ExecuteQuery(DataManager.OraConnString(), SelectQuery1, "Temp_ShiftmentItemsColorSize");
            //string Query = @"SELECT [MasterId],[Image],ImageName FROM [Temp_ShiftmentItemsImage] ";
            //DataTable dtImage = DataManager.ExecuteQuery(DataManager.OraConnString(), Query, "Temp_ShiftmentItemsImage");            
            DataTable dtColSz = null;            
            DataTable dtQty = null;            
            DataTable dtImage = null;           

            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;

            SqlCommand command1 = new SqlCommand();
            command1.Connection = connection;
            command1.Transaction = transection;

            SqlCommand command2 = new SqlCommand();
            command2.Connection = connection;
            command2.Transaction = transection;

            SqlCommand command3 = new SqlCommand();
            command3.Connection = connection;
            command3.Transaction = transection;

            command.CommandText = @"UPDATE [ShiftmentBoxingMst]
   SET [Remarks] ='" + aItemsCartoon.Remarks + "' ,[UpdateDate] =GETDATE() ,[UpdateBy] = '" + aItemsCartoon.LoginBy + "' WHERE ID='" + aItemsCartoon.ID + "' ";
            command.ExecuteNonQuery();

            command.CommandText = @"delete from ShiftmentBoxingItemsDtl where MasterID='" + aItemsCartoon.ID + "'";
            command.ExecuteNonQuery();            

            foreach (DataRow dr in ddt.Rows)
            {
                if (dr["item_code"].ToString() != "")
                {
                    dtColSz = null;
                    dtQty = null;
                    dtImage = null;
                    command2.CommandText = @"SELECT [MasterId],[ColorID_SizeID],[Type] FROM [Temp_ShiftmentItemsColorSize] where MasterId='" + dr["item_code"].ToString() + "'  ";
                    using (SqlDataReader drColorSize = command2.ExecuteReader())
                    {
                        dtColSz = new DataTable();
                        dtColSz.Load(drColorSize);
                    }
                    command3.CommandText = @"SELECT [MasterId],[ColorID],[SizeID],[Quantity] FROM [Temp_ShiftmentItemsQuantity] where MasterId='" + dr["item_code"].ToString() + "' ";
                    using (SqlDataReader drQty = command3.ExecuteReader())
                    {
                        dtQty = new DataTable();
                        dtQty.Load(drQty);
                    }
                    command2.CommandText = @"SELECT [MasterId],[Image],ImageName FROM [Temp_ShiftmentItemsImage] where MasterId='" + dr["item_code"].ToString() + "' ";
                    using (SqlDataReader drImage = command2.ExecuteReader())
                    {
                        dtImage = new DataTable();
                        dtImage.Load(drImage);
                    }

                     DataRow[] ColChk = dtColSz.Select("MasterId =" + dr["item_code"].ToString());
                     if (ColChk.Length > 0)
                     {
                         command.CommandText = @"delete from ShiftmentItemsBoxingColorSize where BoxingItemsID='" + aItemsCartoon.ID + "' and ItemsID='" + dr["item_code"].ToString() + "' ";
                         command.ExecuteNonQuery();
                     }
                     DataRow[] Qtychk = dtQty.Select("MasterId =" + dr["item_code"].ToString());
                     if (Qtychk.Length > 0)
                     {
                         command.CommandText = @"delete from ShiftmentBoxingItemsQuantity where BoxingItemsID='" + aItemsCartoon.ID + "' and ItemsID='" + dr["item_code"].ToString() + "' ";
                         command.ExecuteNonQuery();
                     }
                    DataRow[] Imgchk = dtImage.Select("MasterId =" + dr["item_code"].ToString());
                    if (Imgchk.Length > 0)
                    {
                        command.CommandText = @"delete from ShiftmentBoxingItemsImage where BoxingItemsID='" + aItemsCartoon.ID + "' and ItemsID='" + dr["item_code"].ToString() + "' ";
                        command.ExecuteNonQuery();
                    }

                    command.CommandText = @"INSERT INTO [ShiftmentBoxingItemsDtl]
                    ([MasterID],[ItemsID],[Quantity],[Color])
                      VALUES
                    ('" + aItemsCartoon.ID + "','" + dr["item_code"].ToString() + "','" + dr["qty"].ToString() + "','" + dr["Color"].ToString() + "')";
                    command.ExecuteNonQuery();

                    if (dtColSz.Rows.Count > 0)
                    {
                        DataRow[] Col = dtColSz.Select("MasterId =" + dr["item_code"].ToString());
                        if (Col.Length > 0)
                        {
                            foreach (DataRow dr1 in Col)
                            {
                             command.CommandText = @"INSERT INTO [ShiftmentItemsBoxingColorSize]
                             ([BoxingItemsID],ItemsID,[ColorID_SizeID],[Type])
                             VALUES ('" + aItemsCartoon.ID + "','" + dr["item_code"].ToString() + "','" + dr1["ColorID_SizeID"].ToString() + "','" + dr1["Type"].ToString() + "')";
                             command.ExecuteNonQuery();
                            }
                        }
                    }
                    if (dtQty.Rows.Count > 0)
                    {
                        DataRow[] Qty = dtQty.Select("MasterId =" + dr["item_code"].ToString());
                        if (Qty.Length > 0)
                        {
                            foreach (DataRow dr3 in Qty)
                            {
                                if (dr3["Quantity"].ToString() != "" || dr3["Quantity"].ToString() != "0")
                                {
                                    command1.CommandText = @"INSERT INTO [ShiftmentBoxingItemsQuantity]
                                    ([BoxingItemsID],ItemsID,[ColorID],[SizeID],[Quantity])
                                      VALUES
                                   ('" + aItemsCartoon.ID + "','" + dr["item_code"].ToString() + "','" + dr3["ColorID"].ToString() + "','" + dr3["SizeID"].ToString() + "','" + dr3["Quantity"].ToString() + "')";
                                    command1.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    command.CommandText = @"delete from Temp_ShiftmentItemsColorSize where MasterId='" + dr["item_code"].ToString() + "' ";
                    command.ExecuteNonQuery();
                    command.CommandText = @"delete from Temp_ShiftmentItemsQuantity where MasterId='" + dr["item_code"].ToString() + "' ";
                    command.ExecuteNonQuery();
                    command.CommandText = @"delete from Temp_ShiftmentItemsImage where MasterId='" + dr["item_code"].ToString() + "' ";
                    command.ExecuteNonQuery();
                }
            }

            transection.Commit();
            connection.Close();
            if (dtImage != null)
            {
                foreach (DataRow dr in ddt.Rows)
                {
                    if (dr["item_code"].ToString() != "")
                    {
                        DataRow[] Img = dtImage.Select("MasterId =" + dr["item_code"].ToString());
                        if (Img != null)
                        {
                            if (Img.Length > 0)
                            {
                                foreach (DataRow dr11 in Img)
                                {
                                    connection.Open();
                                    string ImgQuery = @"INSERT INTO [ShiftmentBoxingItemsImage] ([BoxingItemsID],ItemsID,ImageName,[Image])
                            VALUES ('" + aItemsCartoon.ID + "','" + dr["item_code"].ToString() + "','" + dr11["ImageName"].ToString() + "',@img)";
                                    SqlCommand cmnd = new SqlCommand(ImgQuery, connection);
                                    SqlParameter img = new SqlParameter();
                                    img.SqlDbType = SqlDbType.VarBinary;
                                    img.ParameterName = "img";
                                    img.Value = dr11["Image"];
                                    cmnd.Parameters.Add(img);
                                    if (dr11["Image"] == null)
                                    {
                                        cmnd.Parameters.Remove(img);
                                    }
                                    cmnd.ExecuteNonQuery();
                                    connection.Close();
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
       
    }

    public static void DeleteCartoonItems(string ID)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transection;
        try
        {           

            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;

            SqlCommand command1 = new SqlCommand();
            command1.Connection = connection;
            command1.Transaction = transection;

            command.CommandText = @"delete from [ShiftmentBoxingMst] WHERE ID='" + ID + "'";
            command.ExecuteNonQuery();
            command.CommandText = @"delete from ShiftmentBoxingItemsDtl where MasterID='" + ID + "'";
            command.ExecuteNonQuery();
            command.CommandText = @"delete from ShiftmentItemsBoxingColorSize where BoxingItemsID='" + ID + "' ";
            command.ExecuteNonQuery();
            command.CommandText = @"delete from ShiftmentBoxingItemsQuantity where BoxingItemsID='" + ID + "' ";
            command.ExecuteNonQuery();
            command1.CommandText = @"delete from ShiftmentBoxingItemsImage where BoxingItemsID='" + ID + "' ";
            command1.ExecuteNonQuery();           

            transection.Commit();
            connection.Close();
            
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
       
    }

    public static DataTable GetShowCartoonItemsDetails(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string Parameter = ""; //t1.ReceiveFlag IS NULL and
         if (ID != "")
         { Parameter = "WHERE  t1.ID='" + ID + "' order by convert(int,t1.ShiftmentID) desc,convert (int,t1.CartoonNo) desc"; }
         else { Parameter = " where t1.ReceiveFlag IS NULL order by convert(int,t1.ShiftmentID) desc,convert (int,t1.CartoonNo) desc"; }

        string query = @"SELECT t1.[ID]
      ,t1.[CartoonNo]
      ,t1.[ShiftmentID]
      ,t1.[Remarks]
      ,t2.ShiftmentNO
     ,(Select SUM(t2.Quantity) from   ShiftmentBoxingItemsDtl t2 where t2.MasterID=t1.ID) AS Qty  FROM [ShiftmentBoxingMst] t1  inner join ShiftmentAssigen t2 on t2.ID=t1.ShiftmentID  " + Parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Client");
        return dt;
    }

    public static List<ShiftmentItemsCartoon> GetShoItemsDtl(string ID)
    {        
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            connection.Open();
            string selectQuery = @"SELECT t1.[ItemsID]
      ,t2.Name
      ,t1.[Quantity]
      ,t1.[Color]
      FROM [ShiftmentBoxingItemsDtl] t1 inner join Item t2 on t2.ID=t1.ItemsID where t1.[MasterID]='" + ID + "'";
            SqlCommand command = new SqlCommand(selectQuery, connection);
            SqlDataReader reader = command.ExecuteReader();
            List<ShiftmentItemsCartoon> TRList = new List<ShiftmentItemsCartoon>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ShiftmentItemsCartoon aBkBtl = new ShiftmentItemsCartoon();
                    aBkBtl.ItemsID = reader["ItemsID"].ToString();
                    aBkBtl.ItemsName = reader["Name"].ToString();
                    aBkBtl.Qty = reader["Quantity"].ToString();
                    aBkBtl.Color = reader["Color"].ToString();
                    TRList.Add(aBkBtl);
                }
            }
            connection.Close();
            return TRList;
        
    }
    //***************** Cartton Boxing Image Set ******************//
    public static DataTable GetShowCartoonItems(string CartoonNo, string ShiftmentNo,string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string Parameter = "";
        if (CartoonNo == "" && ShiftmentNo == "" && ID != "")
        { Parameter = "WHERE t1.ID='" + ID + "' order by ID desc "; }
        else if (CartoonNo != "" && ShiftmentNo == "" && ID == "")
        { Parameter = "WHERE t2.CartoonNo='" + CartoonNo + "' order by ID desc "; }
        else if (CartoonNo == "" && ShiftmentNo != "" && ID == "")
        { Parameter = "WHERE t2.ShiftmentID='" + ShiftmentNo + "' order by ID desc "; }
        else if (CartoonNo != "" && ShiftmentNo != "" && ID == "")
        { Parameter = "WHERE t2.ShiftmentID='" + ShiftmentNo + "' AND t2.CartoonNo='" + CartoonNo + "' order by ID desc "; }
        else { Parameter = " order by ID desc ";  }

        string query = @" SELECT t1.ID,t2.CartoonNo,t2.ShiftmentID,t3.ShiftmentNO
      ,t1.[ItemsID],t4.Name AS ItemsName
      ,t1.[Quantity]
      ,t1.[Color]      
  FROM [ShiftmentBoxingItemsDtl] t1 inner join ShiftmentItems t5 on t5.ID=t1.ItemsID inner join ShiftmentBoxingMst t2 on t2.ID=t1.MasterID inner join ShiftmentAssigen t3 on t3.ID=t2.ShiftmentID inner join Item t4 on t4.ID=t5.ItemsID  " + Parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Client");
        return dt;
      
    }

    public static void SaveItemShiftmentForCartoon(ShiftmentItemsCartoon aShiftmentItemsCartoon, DataTable dtImg, DataTable dtColor, DataTable dtSize, DataTable dtQt)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transection;
        try
        {
            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;

            SqlCommand command1 = new SqlCommand();
            command1.Connection = connection;
            command1.Transaction = transection;          

            foreach (DataRow dr in dtColor.Rows)
            {
                command.CommandText = @"INSERT INTO [ShiftmentItemsBoxingColorSize]
               ([BoxingItemsID],[ColorID_SizeID],[Type])
               VALUES ('" + aShiftmentItemsCartoon.ID + "','" + dr["ID"].ToString() + "',0)";
                command.ExecuteNonQuery();
            }
            foreach (DataRow dr in dtSize.Rows)
            {
                command.CommandText = @"INSERT INTO [ShiftmentItemsBoxingColorSize]
               ([BoxingItemsID],[ColorID_SizeID],[Type])
               VALUES ('" + aShiftmentItemsCartoon.ID + "','" + dr["ID"].ToString() + "',1)";
                command.ExecuteNonQuery();
            }
            foreach (DataRow dr in dtQt.Rows)
            {
                if (dr["Quantity"].ToString() != "")
                {
                    command1.CommandText = @"INSERT INTO [ShiftmentBoxingItemsQuantity]
                ([BoxingItemsID],[ColorID],[SizeID],[Quantity])
                VALUES
                ('" + aShiftmentItemsCartoon.ID + "','" + dr["ColorID"].ToString() + "','" + dr["SizeID"].ToString() + "','" + dr["Quantity"].ToString() + "')";
                    command1.ExecuteNonQuery();
                }
            }
            transection.Commit();
            connection.Close();

            if (dtImg != null)
            {
                foreach (DataRow dr in dtImg.Rows)
                {
                    connection.Open();
                    string ImgQuery = @"INSERT INTO [ShiftmentBoxingItemsImage]
                                       ([BoxingItemsID],[Image])
                                       VALUES ('" + aShiftmentItemsCartoon.ID + "',@img)";
                    SqlCommand cmnd = new SqlCommand(ImgQuery, connection);
                    SqlParameter img = new SqlParameter();
                    img.SqlDbType = SqlDbType.VarBinary;
                    img.ParameterName = "img";
                    img.Value = dr["Image"];
                    cmnd.Parameters.Add(img);
                    if (dr["Image"] == null)
                    {
                        cmnd.Parameters.Remove(img);
                    }
                    cmnd.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static DataTable getShiftmentItemsCartoonItemsQuantity(string ID,string ItemsID)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlDataAdapter da = new SqlDataAdapter();
        connection.Open();      
        da.SelectCommand = new SqlCommand("SP_ShiftmentCartoonItemsQuantity", connection); 
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.AddWithValue("@MasterID", ID);
        da.SelectCommand.Parameters.AddWithValue("@itemsID", ItemsID);
        //da.SelectCommand.CommandTimeout = 120;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_ShiftmentCartoonItemsQuantity");
        DataTable table = ds.Tables["SP_ShiftmentCartoonItemsQuantity"];
        connection.Close();
        return table;
    }
    public static DataTable getShiftmentItemsCartoonItemsQuantity(string ItemsID)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlDataAdapter da = new SqlDataAdapter();
        connection.Open();
        da.SelectCommand = new SqlCommand("SP_ShiftmentCartoonItemsQuantityPartyWise", connection);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;       
        da.SelectCommand.Parameters.AddWithValue("@itemsID", ItemsID);        
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_ShiftmentCartoonItemsQuantityPartyWise");
        DataTable table = ds.Tables["SP_ShiftmentCartoonItemsQuantityPartyWise"];
        connection.Close();
        return table;
    }
    // ********************* Style & Shiftment Wise Report **************************//
    public static DataTable getShiftmentItemsCartoonStyleWise(string ItemsID)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlDataAdapter da = new SqlDataAdapter();
        connection.Open();
        da.SelectCommand = new SqlCommand("SP_ShiftmentCartoonItemsQuantityStyleWise", connection);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.AddWithValue("@itemsID", ItemsID);
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_ShiftmentCartoonItemsQuantityStyleWise");
        DataTable table = ds.Tables["SP_ShiftmentCartoonItemsQuantityStyleWise"];
        connection.Close();
        return table;
    }
    public static void UpdateItemShiftmentForCartoon(ShiftmentItemsCartoon aShiftmentItemsCartoon, DataTable dtImg, DataTable dtColor, DataTable dtSize, DataTable dtQt)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transection;
        try
        {
            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;

            SqlCommand command1 = new SqlCommand();
            command1.Connection = connection;
            command1.Transaction = transection;

            command.CommandText = @"delete from ShiftmentItemsBoxingColorSize where BoxingItemsID='" + aShiftmentItemsCartoon.ID + "' ";
            command.ExecuteNonQuery();
            foreach (DataRow dr in dtColor.Rows)
            {
                command.CommandText = @"INSERT INTO [ShiftmentItemsBoxingColorSize]
               ([BoxingItemsID],[ColorID_SizeID],[Type])
               VALUES ('" + aShiftmentItemsCartoon.ID + "','" + dr["ID"].ToString() + "',0)";
                command.ExecuteNonQuery();
            }
            //command.CommandText = @"delete from ShiftmentItemsBoxingColorSize where BoxingItemsID='" + aShiftmentItemsCartoon.ID + "' ";
            //command.ExecuteNonQuery();
            foreach (DataRow dr in dtSize.Rows)
            {
                command.CommandText = @"INSERT INTO [ShiftmentItemsBoxingColorSize]
               ([BoxingItemsID],[ColorID_SizeID],[Type])
               VALUES ('" + aShiftmentItemsCartoon.ID + "','" + dr["ID"].ToString() + "',1)";
                command.ExecuteNonQuery();
            }
            command.CommandText = @"delete from ShiftmentBoxingItemsQuantity where BoxingItemsID='" + aShiftmentItemsCartoon.ID + "' ";
            command.ExecuteNonQuery();
            foreach (DataRow dr in dtQt.Rows)
            {
                if (dr["Quantity"].ToString() != "")
                {
                    command1.CommandText = @"INSERT INTO [ShiftmentBoxingItemsQuantity]
                ([BoxingItemsID],[ColorID],[SizeID],[Quantity])
                VALUES
                ('" + aShiftmentItemsCartoon.ID + "','" + dr["ColorID"].ToString() + "','" + dr["SizeID"].ToString() + "','" + dr["Quantity"].ToString() + "')";
                    command1.ExecuteNonQuery();
                }
            }

            command.CommandText = @"delete from ShiftmentBoxingItemsImage where BoxingItemsID='" + aShiftmentItemsCartoon.ID + "' ";
            command.ExecuteNonQuery();

            transection.Commit();
            connection.Close();

            if (dtImg != null)
            {
                foreach (DataRow dr in dtImg.Rows)
                {
                    connection.Open();
                    string ImgQuery = @"INSERT INTO [ShiftmentBoxingItemsImage]
                                       ([BoxingItemsID],[Image])
                                       VALUES ('" + aShiftmentItemsCartoon.ID + "',@img)";
                    SqlCommand cmnd = new SqlCommand(ImgQuery, connection);
                    SqlParameter img = new SqlParameter();
                    img.SqlDbType = SqlDbType.VarBinary;
                    img.ParameterName = "img";
                    img.Value = dr["Image"];
                    cmnd.Parameters.Add(img);
                    if (dr["Image"] == null)
                    {
                        cmnd.Parameters.Remove(img);
                    }
                    cmnd.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static void DeleteItemShiftmentForCartoon(ShiftmentItemsCartoon aShiftmentItemsCartoon)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transection;
        try
        {
            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;           

            command.CommandText = @"delete from ShiftmentItemsBoxingColorSize where BoxingItemsID='" + aShiftmentItemsCartoon.ID + "' ";
            command.ExecuteNonQuery();            
            
            command.CommandText = @"delete from ShiftmentBoxingItemsQuantity where BoxingItemsID='" + aShiftmentItemsCartoon.ID + "' ";
            command.ExecuteNonQuery();            

            command.CommandText = @"delete from ShiftmentBoxingItemsImage where BoxingItemsID='" + aShiftmentItemsCartoon.ID + "' ";
            command.ExecuteNonQuery();

            transection.Commit();
            connection.Close();           
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public static DataTable getShowShiftmentItemsCartoon(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT convert(nvarchar,t1.[ItemsID]) AS [item_code]
      ,t2.Name AS [item_desc]
      ,t1.[Quantity] AS [qty]
      ,convert(nvarchar,t1.[Color]) AS [Color]
      FROM [ShiftmentBoxingItemsDtl] t1 inner join ShiftmentItems t3 on t3.ID=t1.ItemsID inner join Item t2 on t2.ID=t3.ItemsID where t1.[MasterID]='" + ID + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Client");
        return dt;
    }

    public static void SaveTempItemShiftmentForCartoon(string ItemsShiftmentID, DataTable dtImg, DataTable dtColor, DataTable dtSize, DataTable dtQt)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transection;
        try
        {
            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;

            SqlCommand command1 = new SqlCommand();
            command1.Connection = connection;
            command1.Transaction = transection;

            command1.CommandText = @"delete from Temp_ShiftmentItemsColorSize where MasterId='" + ItemsShiftmentID + "' ";
            command1.ExecuteNonQuery();
            command1.CommandText = @"delete from Temp_ShiftmentItemsQuantity where MasterId='" + ItemsShiftmentID + "' ";
            command1.ExecuteNonQuery();
            command1.CommandText = @"delete from Temp_ShiftmentItemsImage where MasterId='" + ItemsShiftmentID + "' ";
            command1.ExecuteNonQuery();

            foreach (DataRow dr in dtColor.Rows)
            {
                command.CommandText = @"INSERT INTO [Temp_ShiftmentItemsColorSize]
               ([MasterId],[ColorID_SizeID],[Type])
                 VALUES
               ('" + ItemsShiftmentID + "','" + dr["ID"].ToString() + "',0)";
                command.ExecuteNonQuery();
            }
            foreach (DataRow dr in dtSize.Rows)
            {
                command.CommandText = @"INSERT INTO [Temp_ShiftmentItemsColorSize]
               ([MasterId],[ColorID_SizeID],[Type])
                 VALUES
               ('" + ItemsShiftmentID + "','" + dr["ID"].ToString() + "',1)";
                command.ExecuteNonQuery();
            }
            foreach (DataRow dr in dtQt.Rows)
            {
                if (dr["Quantity"].ToString() != "")
                {
                   command1.CommandText = @"INSERT INTO [Temp_ShiftmentItemsQuantity]
                   ([MasterId],[ColorID],[SizeID],[Quantity])
                     VALUES
                    ('" + ItemsShiftmentID + "','" + dr["ColorID"].ToString() + "','" + dr["SizeID"].ToString() + "','" + dr["Quantity"].ToString() + "')";
                   command1.ExecuteNonQuery();
                }
            }
            transection.Commit();
            connection.Close();

            if (dtImg != null)
            {
                foreach (DataRow dr in dtImg.Rows)
                {
                    connection.Open();
                    string ImgQuery = @"INSERT INTO [Temp_ShiftmentItemsImage]
                   ([MasterId],ImageName,[Image])
                      VALUES
                   ('" + ItemsShiftmentID + "','" + dr["ImageName"].ToString() +"',@img)";
                    SqlCommand cmnd = new SqlCommand(ImgQuery, connection);
                    SqlParameter img = new SqlParameter();
                    img.SqlDbType = SqlDbType.VarBinary;
                    img.ParameterName = "img";
                    img.Value = dr["Image"];
                    cmnd.Parameters.Add(img);
                    if (dr["Image"] == null)
                    {
                        cmnd.Parameters.Remove(img);
                    }
                    cmnd.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static DataTable getTemp_ShiftmentItemsCartoonItemsQuantity(string ID, string p_2)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlDataAdapter da = new SqlDataAdapter();
        connection.Open();
        da.SelectCommand = new SqlCommand("SP_Temp_CartoonItemsQuantity", connection);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.AddWithValue("@MasterID", ID);
        //da.SelectCommand.CommandTimeout = 120;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_Temp_CartoonItemsQuantity");
        DataTable table = ds.Tables["SP_Temp_CartoonItemsQuantity"];
        connection.Close();
        return table;
    }

    public static void UpdateTempItemShiftmentForCartoon(string ItemsShiftmentID, DataTable dtImg, DataTable dtColor, DataTable dtSize, DataTable dtQt)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transection;
        try
        {
            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;

            SqlCommand command1 = new SqlCommand();
            command1.Connection = connection;
            command1.Transaction = transection;

            command.CommandText = @"delete from Temp_ShiftmentItemsColorSize where MasterId='" + ItemsShiftmentID + "' ";
            command.ExecuteNonQuery();
            foreach (DataRow dr in dtColor.Rows)
            {
                command.CommandText = @"INSERT INTO [Temp_ShiftmentItemsColorSize]
               ([MasterId],[ColorID_SizeID],[Type])
                 VALUES
               ('" + ItemsShiftmentID + "','" + dr["ID"].ToString() + "',0)";
                command.ExecuteNonQuery();
            }            
            foreach (DataRow dr in dtSize.Rows)
            {
                command.CommandText = @"INSERT INTO [Temp_ShiftmentItemsColorSize]
               ([MasterId],[ColorID_SizeID],[Type])
                 VALUES
               ('" + ItemsShiftmentID + "','" + dr["ID"].ToString() + "',1)";
                command.ExecuteNonQuery();
            }
            command.CommandText = @"delete from Temp_ShiftmentItemsQuantity where MasterId='" + ItemsShiftmentID + "' ";
            command.ExecuteNonQuery();
            foreach (DataRow dr in dtQt.Rows)
            {
                if (dr["Quantity"].ToString() != "")
                {
                    command1.CommandText = @"INSERT INTO [Temp_ShiftmentItemsQuantity]
                   ([MasterId],[ColorID],[SizeID],[Quantity])
                     VALUES
                    ('" + ItemsShiftmentID + "','" + dr["ColorID"].ToString() + "','" + dr["SizeID"].ToString() + "','" + dr["Quantity"].ToString() + "')";
                    command1.ExecuteNonQuery();
                }
            }

            command.CommandText = @"delete from Temp_ShiftmentItemsImage where MasterId='" + ItemsShiftmentID + "' ";
            command.ExecuteNonQuery();

            transection.Commit();
            connection.Close();
            
            if (dtImg != null)
            {
                foreach (DataRow dr in dtImg.Rows)
                {
                    connection.Open();
                    string ImgQuery = @"INSERT INTO [Temp_ShiftmentItemsImage]
                   ([MasterId],[Image])
                      VALUES
                   ('" + ItemsShiftmentID + "',@img)";
                    SqlCommand cmnd = new SqlCommand(ImgQuery, connection);
                    SqlParameter img = new SqlParameter();
                    img.SqlDbType = SqlDbType.VarBinary;
                    img.ParameterName = "img";
                    img.Value = dr["Image"];
                    cmnd.Parameters.Add(img);
                    if (dr["Image"] == null)
                    {
                        cmnd.Parameters.Remove(img);
                    }
                    cmnd.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static void DeleteTempTable(string ItemsId)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transection;
        try
        {
            connection.Open();
            transection = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transection;

            SqlCommand command1 = new SqlCommand();
            command1.Connection = connection;
            command1.Transaction = transection;

            if (!string.IsNullOrEmpty(ItemsId))
            {
                command1.CommandText = @"delete from Temp_ShiftmentItemsColorSize where MasterId='" + ItemsId + "' ";
                command1.ExecuteNonQuery();
                command1.CommandText = @"delete from Temp_ShiftmentItemsQuantity where MasterId='" + ItemsId + "' ";
                command1.ExecuteNonQuery();
                command1.CommandText = @"delete from Temp_ShiftmentItemsImage where MasterId='" + ItemsId + "' ";
                command1.ExecuteNonQuery();
            }
            else
            {
                command.CommandText = @"truncate table Temp_ShiftmentItemsColorSize";
                command.ExecuteNonQuery();
                command.CommandText = @"truncate table Temp_ShiftmentItemsQuantity";
                command.ExecuteNonQuery();
                command.CommandText = @"truncate table Temp_ShiftmentItemsImage";
                command.ExecuteNonQuery();
            }

            transection.Commit();
            connection.Close();            
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static DataTable GetShowShiftmentItems(string CartoonID, string Shiftment)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string Parameter = "";
        if (CartoonID != "" && Shiftment == "")
        { Parameter = "WHERE t1.CartoonNo='" + CartoonID + "' order by ID desc "; }
        else if (CartoonID == "" && Shiftment != "")
        { Parameter = "WHERE t2.ShiftmentNO='" + Shiftment + "' order by ID desc "; }
        else if (CartoonID != "" && Shiftment != "")
        { Parameter = "WHERE t1.CartoonNo='" + CartoonID + "' and t2.ShiftmentNO='" + Shiftment + "' order by ID desc "; }
        else if (CartoonID == "" && Shiftment == "")
        { Parameter = " order by ID desc "; }
        else { Parameter = " order by ID desc "; }

        string query = @"SELECT t1.[ID],CONVERT(NVARCHAR,t1.[AddDate],103) AS AddDate,t1.[CartoonNo],t1.[ShiftmentID],t1.[Remarks],t2.ShiftmentNO,CASE when t1.ReceiveFlag=1 then 'Receive' else 'Sent'  end AS Flag FROM [ShiftmentBoxingMst] t1 inner join dbo.ShiftmentAssigen t2 on t2.ID=t1.ShiftmentID  " + Parameter;
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Client");
        return dt;
    }
    public static DataTable GetShowShiftmentItems(string CartoonID, string CartoonID1, string Shiftment, string falg)
    {
        using (SqlConnection conn = new SqlConnection(DataManager.OraConnString())){
            SqlCommand sqlComm = new SqlCommand("SP_CartonReceived", conn);
            if (!string.IsNullOrEmpty(CartoonID))
            {
                sqlComm.Parameters.AddWithValue("@fromCarton", CartoonID);
                sqlComm.Parameters.AddWithValue("@toCarton", CartoonID1);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@fromCarton", null);
                sqlComm.Parameters.AddWithValue("@toCarton", null);
            }
            if (!string.IsNullOrEmpty(Shiftment))
            {
                sqlComm.Parameters.AddWithValue("@ShipmentID", Shiftment);
            }
            else
            {
                sqlComm.Parameters.AddWithValue("@ShipmentID", null);
            }
            sqlComm.Parameters.AddWithValue("@flag", falg);
            sqlComm.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            da.SelectCommand = sqlComm;
            da.Fill(ds, "SP_CartonReceived");
            DataTable dtCartonReceived = ds.Tables["SP_CartonReceived"];
            return dtCartonReceived;
        }
    }
    public static void GetItemsCartoonReceive(string ID,string UserID)
    {
        DataTable ddt = IdManager.GetShowDataTable("SELECT t3.ID,t3.Name ,ISNULL(t3.UnitPrice,0)UnitPrice ,ISNULL(t1.[Quantity],0)Quantity FROM [ShiftmentBoxingItemsDtl] t1 inner join ShiftmentItems t2 on t2.ID=t1.ItemsID inner join Item t3 on t3.ID=t2.ItemsID where t1.MasterID ='" + ID + "' ");

        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();

            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            foreach (DataRow dr in ddt.Rows)
            {

                command.CommandText = @"UPDATE [ShiftmentBoxingMst] SET [ReceiveFlag] =1  WHERE ID='"+ID+"'";
                command.ExecuteNonQuery();
                command.CommandText = @"SELECT COUNT(*) FROM [ItemSalesStock] where [ItemsID]='" + dr["ID"].ToString() + "'";
                int Count = Convert.ToInt32(command.ExecuteScalar());
                if (Count > 0)
                {
                    command.CommandText = @"update [ItemSalesStock] set [Quantity]=[Quantity]+(" + dr["Quantity"].ToString().Replace(",", "") + "),Price=" + dr["UnitPrice"].ToString().Replace(",", "") + "  where [ItemsID]='" + dr["ID"].ToString() + "'";
                    command.ExecuteNonQuery();
                }
                else
                {
                    command.CommandText = @"INSERT INTO [ItemSalesStock]
                    ([ItemsID],[Quantity],[Price],[UpdateBy],[UpdateDate])
                    VALUES
                   ('" + dr["ID"].ToString() + "','" + dr["Quantity"].ToString() + "','" + dr["UnitPrice"].ToString() + "','" + UserID + "',GETDATE())";
                    command.ExecuteNonQuery();
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

    public static void GetItemsCartoonReceive(string ID, string UserID, DataTable dt, string shiftmentId,
        DataTable dtCartoonList)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        foreach (DataRow drRow in dtCartoonList.Rows)
        {
            DataTable ddt =
                IdManager.GetShowDataTable(
                    "SELECT t3.ID,t3.Name ,ISNULL(t3.UnitPrice,0)UnitPrice ,ISNULL(t1.[Quantity],0)Quantity FROM [ShiftmentBoxingItemsDtl] t1 inner join ShiftmentItems t2 on t2.ID=t1.ItemsID inner join Item t3 on t3.ID=t2.ItemsID where t1.MasterID ='" +
                    drRow["ID"].ToString() + "' ");
            double totQty = 0, totLassQty = 0, AccessQty = 0;
            connection.Open();
            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(row["Batstock"].ToString()))
                    {
                        totQty = Convert.ToDouble(row["Batstock"]);
                    }

                    if (!string.IsNullOrEmpty(row["LassQty"].ToString()))
                    {
                        totLassQty = Convert.ToDouble(row["LassQty"]);
                    }
                    if (!string.IsNullOrEmpty(row["AccessQty"].ToString()))
                    {
                        AccessQty = Convert.ToDouble(row["AccessQty"]);
                    }
                    if (totQty != 0 || totLassQty != 0 || AccessQty != 0)
                    {
                        command.CommandText = @"INSERT INTO [ItemBadStock]
           ([ShiftmentBoxingMstID],[ItemsID] ,[ColorID] ,[Quantity] ,[Lost_Qty],[Access_Qty],AddBy,AddDate,ShiftmentID)
           VALUES
           ('" + ID + "','" + row["ItemsID"].ToString() + "','" + row["ColorID"].ToString() + "','" +
                                              row["Batstock"].ToString() + "','" + row["LassQty"].ToString() + "','" +
                                              row["AccessQty"].ToString() + "','" + UserID + "',GETDATE(),'" + row["ShiftmentID"].ToString() + "')";
                        command.ExecuteNonQuery();
                    }
                }
            }
            foreach (DataRow dr in ddt.Rows)
            {

                command.CommandText =
                    @"UPDATE [ShiftmentBoxingMst] SET [ReceiveFlag] =1,ReceivedDate=GetDate(),ReceivedBy='" + UserID +
                    "'  WHERE ID='" + drRow["ID"].ToString() + "' ";
                command.ExecuteNonQuery();
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        totQty = (from DataRow dr1 in dt.AsEnumerable()
                            where dr1["ItemsID"].ToString() == dr["ID"].ToString()
                            select Convert.ToDouble(dr1["Batstock"])).Sum();
                        totLassQty = (from DataRow dr1 in dt.AsEnumerable()
                            where dr1["ItemsID"].ToString() == dr["ID"].ToString()
                            select Convert.ToDouble(dr1["LassQty"])).Sum();
                        AccessQty = (from DataRow dr1 in dt.AsEnumerable()
                            where dr1["ItemsID"].ToString() == dr["ID"].ToString()
                            select Convert.ToDouble(dr1["AccessQty"])).Sum();
                    }
                }
                command.CommandText = @"SELECT COUNT(*) FROM [ItemSalesStock] where [ItemsID]='" + dr["ID"].ToString() +
                                      "' and Flag=1 and Type='" + shiftmentId + "' ";
                int Count = Convert.ToInt32(command.ExecuteScalar());
                if (Count > 0)
                {
                    double totQtyFin = (Convert.ToDouble(dr["Quantity"].ToString().Replace(",", "")) + AccessQty -
                                        (totQty + totLassQty));

                    command.CommandText = @"update [ItemSalesStock] set [Quantity]=[Quantity]+(" + totQtyFin +
                                          "),Price=isnull(Price,0)+" +
                                          Convert.ToDouble(dr["UnitPrice"].ToString().Replace(",", ""))*totQtyFin +
                                          "  where [ItemsID]='" +
                                          dr["ID"].ToString() + "' and Flag=1 and Type='" + shiftmentId + "' ";
                    command.ExecuteNonQuery();
                }
                else
                {
                    double totQtyFin = (Convert.ToDouble(dr["Quantity"].ToString().Replace(",", "")) + AccessQty -
                                        (totQty + totLassQty));
                    command.CommandText = @"INSERT INTO [ItemSalesStock]
                    ([ItemsID],[Quantity],[Price],[UpdateBy],[UpdateDate],Type,Flag)
                     VALUES
                    ('" + dr["ID"].ToString() + "','" + totQtyFin + "','" + dr["UnitPrice"].ToString() + "','" + UserID +
                                          "',GETDATE(),'" + shiftmentId + "',1)";
                    command.ExecuteNonQuery();
                }

            }
            transaction.Commit();
            connection.Close();
        }
    }

    public static DataTable GetShowSendAnReceiveItemsList(string ShiftmentID, string Cartoon1, string Cartoon2,string SR)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string Parameter = "";
        if (!string.IsNullOrEmpty(ShiftmentID) && string.IsNullOrEmpty(Cartoon1) && string.IsNullOrEmpty(Cartoon2))
        {
            if (SR.Equals("0"))
            {
                Parameter = " WHERE t1.ReceiveFlag IS NULL AND t1.[ShiftmentID]='" + ShiftmentID + "' ";
            }
            else
            {
                Parameter = " WHERE t1.ReceiveFlag=" + SR + " AND t1.[ShiftmentID]='" + ShiftmentID + "' ";
            }
        }
        else if (string.IsNullOrEmpty(ShiftmentID) && !string.IsNullOrEmpty(Cartoon1) && !string.IsNullOrEmpty(Cartoon2))
        {
            if (SR.Equals("0"))
            {
                Parameter = " where t1.ReceiveFlag IS NULL and t1.[CartoonNo] between '" + Cartoon1 + "' and '" + Cartoon2 + "' ";
            }
            else
            {
                Parameter = " where t1.ReceiveFlag=" + SR + " and t1.[CartoonNo] between '" + Cartoon1 + "' and '" + Cartoon2 + "' ";
            }
        }
        else if (!string.IsNullOrEmpty(ShiftmentID) && !string.IsNullOrEmpty(Cartoon1) && !string.IsNullOrEmpty(Cartoon2))
        {
            if (SR.Equals("0"))
            {
                Parameter = " where t1.ReceiveFlag IS NULL and t1.[ShiftmentID]='" + ShiftmentID + "' AND t1.[CartoonNo] between '" + Cartoon1 + "' and '" + Cartoon2 + "' ";
            }
            else
            {
                Parameter = " where t1.ReceiveFlag=" + SR + " and t1.[ShiftmentID]='" + ShiftmentID + "' AND t1.[CartoonNo] between '" + Cartoon1 + "' and '" + Cartoon2 + "' ";
            }
        }
        else
        {
            if (SR.Equals("0"))
            {
                Parameter = " where t1.ReceiveFlag IS NULL ";
            }
            else
            {
                Parameter = " where t1.ReceiveFlag=" + SR + " ";
            }
        }

        string query =
            @"SELECT ROW_NUMBER() OVER (ORDER BY t1.[ID]) AS [SL NO],t1.[ID],CONVERT(NVARCHAR,t1.[AddDate],103) AS AddDate,t1.[CartoonNo],t1.[ShiftmentID],t1.[Remarks],t2.ShiftmentNO,CASE when t1.ReceiveFlag=1 then 'Receive' else 'Send'  end AS Flag,t3.tot_Qty FROM [ShiftmentBoxingMst] t1 inner join dbo.ShiftmentAssigen t2 on t2.ID=t1.ShiftmentID INNER JOIN (select t1.ID,t1.CartoonNo,sum(t2.Quantity)AS[tot_Qty] from [ShiftmentBoxingMst] t1 inner join [ShiftmentBoxingItemsDtl] t2 on t2.MasterID=t1.ID GROUP BY t1.ID,t1.CartoonNo) t3 on t3.ID=t1.ID " +
            Parameter + " order by t2.ShiftmentNO,CONVERT(int,t1.[CartoonNo]) ASC";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Client");
        return dt;
    }

    public static DataTable GetShowAllItemsStock(string ItemsID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.[ID],t1.[Code],t1.[Name],t1.[UOMID],t1.[UnitPrice],t1.[ShortName],t1.[ClosingStock] AS[BD_Stock] ,t2.PH_Stock ,t3.In_Tr
  FROM [Item] t1 
  LEFT JOIN (SELECT [ItemsID],SUM([Quantity]) AS[PH_Stock] FROM [ItemSalesStock] GROUP BY [ItemsID]) t2 on t2.ItemsID=t1.ID
  LEFT JOIN (select t2.ItemsID,t3.Name,sum(t1.Quantity) AS[In_Tr] from [ShiftmentBoxingItemsDtl] t1
  INNER join ShiftmentItems t2 on t2.ID=t1.ItemsID INNER JOIN Item t3 on t3.ID=t2.ItemsID INNER JOIN [ShiftmentBoxingMst] t4 on t4.ID=t1.MasterID  WHERE t4.ReceiveFlag IS NULL GROUP BY t2.ItemsID,t3.Name ) t3 on t3.ItemsID=t1.ID";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Client");
        return dt;
    }

    public static DataTable GetShiftmentCartoonBySearch(string CartoonNo, string ShiftmentNo)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string Parameter = "";
        if (!string.IsNullOrEmpty(CartoonNo) && string.IsNullOrEmpty(ShiftmentNo))
        {
            Parameter = "where t1.CartoonNo=" + CartoonNo + " order by convert(int,t1.ShiftmentID) desc,t1.CartoonNo";
        }
        else if (string.IsNullOrEmpty(CartoonNo) && !string.IsNullOrEmpty(ShiftmentNo))
        {
            Parameter = " where t1.[ShiftmentID]='" + ShiftmentNo + "' ";
        }
        else if (!string.IsNullOrEmpty(CartoonNo) && !string.IsNullOrEmpty(ShiftmentNo))
        {
            Parameter = " where t1.CartoonNo=" + CartoonNo + " and t1.[ShiftmentID]='" + ShiftmentNo + "' ";
        }
        string query = @"SELECT t1.[ID]
      ,t1.[CartoonNo]
      ,t1.[ShiftmentID]
      ,t1.[Remarks]
      ,t2.ShiftmentNO
     ,(Select SUM(t2.Quantity) from   ShiftmentBoxingItemsDtl t2 where t2.MasterID=t1.ID) AS Qty  FROM [ShiftmentBoxingMst] t1  inner join ShiftmentAssigen t2 on t2.ID=t1.ShiftmentID " + Parameter + " ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ShiftmentBoxingMst");
        return dt;
    }

    public static byte[] GetShipmentImage(string ShipmentID, string ItemsID)
    {
        byte[] img = null;
        String ConnectionString = DataManager.OraConnString();
        SqlConnection myConnection = new SqlConnection(ConnectionString);
        string Query = @"select top(1) [Image] from ShiftmentBoxingItemsImage t1
        inner join ShiftmentBoxingMst t2 on t2.ID=t1.BoxingItemsID
        where t2.ShiftmentID='" + ShipmentID + "' and t1.ItemsID='" + ItemsID + "' ";
        myConnection.Open();
        SqlCommand myCommand = new SqlCommand(Query, myConnection);
        object maxValue = myCommand.ExecuteScalar();
        myConnection.Close();
        if (maxValue != System.DBNull.Value)
        {
            img = (byte[])maxValue;
        }
        return img;
    }

    public void GetCancelReceivedShipment(DataTable dt,string LoginBy)
    {
        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;

        connection.Open();
        transaction = connection.BeginTransaction();
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction;
        try
        {
            foreach (DataRow drRow in dt.Rows)
            {
                DataTable ddtItems = IdManager.GetShowDataTable(
                    @"SELECT t3.ID,t3.Name ,ISNULL(t3.UnitPrice,0)UnitPrice ,ISNULL(t1.[Quantity],0)Quantity FROM [ShiftmentBoxingItemsDtl] t1 inner join ShiftmentItems t2 on t2.ID=t1.ItemsID inner join Item t3 on t3.ID=t2.ItemsID where t1.MasterID ='" +
                    drRow["ID"].ToString() + "' ");
                foreach (DataRow drItems in ddtItems.Rows)
                {
                    command.CommandText = @"UPDATE [dbo].[ItemSalesStock]
                SET  [Quantity] =isnull([Quantity],0)-" + drItems["Quantity"].ToString() + " WHERE [ItemsID]='" +
                                          drItems["ID"].ToString() + "' and [Type]='" +
                                          drRow["ShiftmentID"].ToString() +
                                          "' and [Flag]=1";
                    command.ExecuteNonQuery();
                }

                command.CommandText = @"UPDATE [dbo].[ShiftmentBoxingMst]
            SET [ReceiveFlag] =NULL ,[ReceivedDate] =NULL ,[ReceivedBy] =NULL ,[ReceivedCancelBy] ='" + LoginBy +
                                      "' ,[ReceivedCancelDate] =GETDATE() WHERE ID='" + drRow["ID"].ToString() + "' ";
                command.ExecuteNonQuery();
                transaction.Commit();
                connection.Close();
            }
        }
        catch
        {
            transaction.Rollback();
        }
    }
}