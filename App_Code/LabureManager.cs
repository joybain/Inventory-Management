using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Delve;
using System.Data.SqlClient;
using System.Data;

/// <summary>
/// Summary description for LabureManager
/// </summary>
public class LabureManager
{
	public LabureManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static Labure GetLabure(string Lab)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = "select [ID] ,[Code] ,[Name] ,[ContactTitle],[ContactName] ,[Designation],[Email],[Phone],[Fax],[Website],[Mobile],[Address1],[Address2],[City],[State],[PostalCode],[Country],[SupplierGroupID],[Active],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[Gl_CoaCode],Convert(nvarchar,[JoinDate],103) as JoinDate,Convert(nvarchar,[RetairedDate],103) as RetairedDate ,[NIDImage],[CurrentImage] from Labure where ID='" + Lab + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new Labure(dt.Rows[0]);
    }

    public static void UpdateSupplier(Labure Lab)
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

            string variables = @"[Code] ='" + Lab.SupCode + "' ,[Name] = '" + Lab.ComName + "' ,[ContactName] ='" + Lab.SupName + "'  ,[Designation] ='" + Lab.Designation + "'  ,[Email] ='" + Lab.Email + "' ,[Phone] = '" + Lab.SupPhone + "' ,[Fax] = '" + Lab.Fax + "' ,[Mobile] ='" + Lab.SupMobile + "'  ,[Address1] ='" + Lab.SupAddr1 + "' ,[Address2] ='" + Lab.SupAddr2 + "'  ,[City] ='" + Lab.City + "'  ,[State] ='" + Lab.State + "' ,[PostalCode] ='" + Lab.PostCode + "' ,[Country] ='" + Lab.Country + "' ,[SupplierGroupID] = '" + Lab.SupGroup + "' ,[Active] ='" + Lab.Active + "'  ,[ModifiedBy] = '" + Lab.LoginBy + "',[ModifiedDate] =GetDate(),[JoinDate]=Convert(date,'" + Lab.JoinDate + "',103),[RetairedDate]=Convert(date,'" + Lab.RetairedDate + "',103) ";


            if (Lab.NIDImage != null)
            {
                if (Lab.NIDImage.Length > 0)
                {
                    variables = variables + ",[NIDImage]=@img1";
                   
                }
            }
            if (Lab.CurrentImage != null)
            {
                if (Lab.CurrentImage.Length > 0)
                {
                    variables = variables + ",[CurrentImage]=@img2";
                    
                }
            }

            command.CommandText = " UPDATE  [Labure]  SET  " + variables + "  WHERE ID='" + Lab.ID + "' ";

            SqlParameter img = new SqlParameter();
            img.SqlDbType = SqlDbType.VarBinary;
            img.ParameterName = "img1";
            if (Lab.NIDImage != null)
            {
                img.Value = Lab.NIDImage;
            }

            SqlParameter img2 = new SqlParameter();
            img2.SqlDbType = SqlDbType.VarBinary;
            img2.ParameterName = "img2";

            if (Lab.CurrentImage != null)
            {
                img2.Value = Lab.CurrentImage;
            }



            
                command.Parameters.Add(img);
                command.Parameters.Add(img2);
                if (Lab.NIDImage == null)
                {
                    command.Parameters.Remove(img);
                }
                else
                {
                    if (Lab.NIDImage.Length == 0)
                    {
                        command.Parameters.Remove(img);
                    }
                }
                if (Lab.CurrentImage == null)
                {
                    command.Parameters.Remove(img2);
                }
                else
                {
                    if (Lab.CurrentImage.Length == 0)
                    {
                        command.Parameters.Remove(img2);
                    }
                }
               
                command.ExecuteNonQuery();
            
         

            command.CommandText = @"UPDATE [GL_SEG_COA] SET [SEG_COA_DESC] ='" + Lab.SupName + "'  WHERE [SEG_COA_CODE]='" + Lab.GlCoa + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE [GL_COA] SET [COA_DESC] ='" + Lab.CoaDesc + "," + Lab.SupName + "," + Lab.SupMobile + "' where [GL_COA_CODE]='1-" + Lab.GlCoa + "'";
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

    public static void CreateLabure(Labure Lab)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string variables = @"[Code],[Name],[ContactName],[Designation],[Email],[Phone],[Fax],[Mobile],[Address1],[Address2],[City],[State],[PostalCode],[Country],[SupplierGroupID]
           ,[Active],[CreatedBy],[CreatedDate],Gl_CoaCode,[JoinDate] ,[RetairedDate] ";
    string values="'" + Lab.SupCode + "','" + Lab.ComName + "','" + Lab.SupName + "','" + Lab.Designation + "','" + Lab.Email + "','" + Lab.SupPhone + "','" + Lab.Fax + "','" + Lab.SupMobile + "','" + Lab.SupAddr1 + "','" + Lab.SupAddr2 + "','" + Lab.City + "','" + Lab.State + "','" + Lab.PostCode + "','" + Lab.Country + "','" + Lab.SupGroup + "','" + Lab.Active + "','" + Lab.LoginBy + "',GetDate(),'" + Lab.GlCoa + "',convert(date,'" + Lab.JoinDate + "',103),convert(date,'" + Lab.RetairedDate + "',103)";



    if (Lab.NIDImage != null)
            {
                if (Lab.NIDImage.Length > 0)
                {
                    variables = variables + ",[NIDImage]";
                    values = values + ",@img1";
                }
            }
    if (Lab.CurrentImage != null)
            {
                if (Lab.CurrentImage.Length > 0)
                {
                    variables = variables + ",[CurrentImage]";
                    values = values + ",@sig";
                }
            }

       string query = " INSERT INTO [Labure] (" + variables + ")  values ( " + values + " )";

        SqlParameter img = new SqlParameter();
        img.SqlDbType = SqlDbType.VarBinary;
        img.ParameterName = "img1";
        if (Lab.NIDImage != null)
        {
            img.Value = Lab.NIDImage;
        }
        
        SqlParameter img2 = new SqlParameter();
        img2.SqlDbType = SqlDbType.VarBinary;
        img2.ParameterName = "img2";

        if (Lab.CurrentImage != null)
        {
            img2.Value = Lab.CurrentImage;
        }
       


        using (SqlCommand cmnd = new SqlCommand(query, sqlCon))
        {
            cmnd.Parameters.Add(img);
            cmnd.Parameters.Add(img2);
            if (Lab.NIDImage == null)
            {
                cmnd.Parameters.Remove(img);
            }
            else
            {
                if (Lab.NIDImage.Length == 0)
                {
                    cmnd.Parameters.Remove(img);
                }
            }
            if (Lab.CurrentImage == null)
            {
                cmnd.Parameters.Remove(img2);
            }
            else
            {
                if (Lab.CurrentImage.Length == 0)
                {
                    cmnd.Parameters.Remove(img2);
                }
            }
            sqlCon.Open();
            cmnd.ExecuteNonQuery();
        }
       // DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void DeleteSupplier(Labure Lab)
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

            command.CommandText = @"DELETE FROM [Labure] WHERE ID='" + Lab.ID + "' ";
            command.ExecuteNonQuery();

            command.CommandText = @"delete from GL_SEG_COA where SEG_COA_CODE='" + Lab.GlCoa + "' ";
            command.ExecuteNonQuery();

            command.CommandText = @"delete from GL_COA where COA_NATURAL_CODE='" + Lab.GlCoa + "' ";
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

    public static DataTable GetLabour()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = "select * from Labure order by ID ";

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Labure");
        return dt;
    }

    public static DataTable getLabRpt(string LABID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.[ID]
      ,t1.[Code]
      ,t1.[Name]
      ,t1.[ContactTitle]
      ,t1.[ContactName]
      ,t1.[Designation]
      ,t1.[Email]
      ,t1.[Phone]
      ,t1.[Fax]
      ,t1.[Website]
      ,t1.[Mobile]
      ,t1.[Address1]
      ,t1.[Address2]
      ,t1.[City]
      ,t1.[State]
      ,t1.[PostalCode]
      ,t1.[Country]
      ,t2.COUNTRY_DESC
      ,t1.[SupplierGroupID]
      ,t1.[Active]
      ,t1.[CreatedBy]
      ,t1.[CreatedDate]
      ,t1.[ModifiedBy]
      ,t1.[ModifiedDate]
      ,t1.[Gl_CoaCode]
      ,Convert(nvarchar,t1.[JoinDate],103) as JoinDate
      ,Convert(nvarchar,t1.[RetairedDate],103) as RetairedDate
      ,t1.[NIDImage]
      ,t1.[CurrentImage]
  FROM [Labure] t1 inner join COUNTRY_INFO t2 on t2.COUNTRY_CODE=t1.Country where t1.ID='" + LABID+"' ";

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Labure");
        return dt;
    }
}